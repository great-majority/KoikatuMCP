using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using KKStudioSocket.Models.Requests;

namespace KoikatuMCP.Services;

public class WebSocketService : IDisposable
{
    private readonly ILogger<WebSocketService> _logger;
    private readonly string _uri;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _logger = logger;
        _uri = Environment.GetEnvironmentVariable("KKSTUDIOSOCKET_URL") ?? "ws://127.0.0.1:8765/ws";
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // Use original property names (lowercase)
            WriteIndented = true,
            IncludeFields = true, // Include public fields for serialization
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Exclude null values from JSON
        };
    }

    public async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(TRequest request, int timeoutMs = 5000)
        where TRequest : BaseCommand
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebSocketService));

        ClientWebSocket webSocket = new ClientWebSocket();
        try
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            await webSocket.ConnectAsync(new Uri(_uri), cancellationTokenSource.Token);
            _logger.LogInformation("Connected to KKStudioSocket WebSocket server at {Uri}", _uri);

            return await SendAndReceiveAsync<TRequest, TResponse>(webSocket, request, timeoutMs, cancellationTokenSource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket communication failed - Type: {ExceptionType}, Message: {Message}, Request: {RequestType}",
                ex.GetType().Name, ex.Message, typeof(TRequest).Name);
            throw;
        }
        finally
        {
            await CloseWebSocketAsync(webSocket);
            webSocket.Dispose();
        }
    }


    private async Task<TResponse?> SendAndReceiveAsync<TRequest, TResponse>(
        ClientWebSocket webSocket,
        TRequest request,
        int timeoutMs,
        CancellationTokenSource cancellationTokenSource)
        where TRequest : BaseCommand
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var messageBytes = Encoding.UTF8.GetBytes(json);

        _logger.LogInformation("SENT: {Data}", json);

        using var timeoutCts = new CancellationTokenSource(timeoutMs);
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationTokenSource.Token,
            timeoutCts.Token);

        try
        {
            // Send message
            _logger.LogDebug("Sending message to WebSocket...");
            await webSocket.SendAsync(
                new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text,
                true,
                combinedCts.Token);
            _logger.LogDebug("Message sent successfully");

            // Receive response
            _logger.LogDebug("Waiting for response...");
            var responseJson = await ReceiveMessageAsync(webSocket, combinedCts.Token);
            _logger.LogDebug("Response received successfully");

            // Log response but truncate large image data only for screenshot commands
            if (typeof(TRequest) == typeof(ScreenshotCommand))
            {
                var logData = TruncateImageDataForLog(responseJson);
                _logger.LogInformation("RECEIVED (processed): {Data}", logData);
            }
            else
            {
                // Log raw response
                _logger.LogInformation("RECEIVED: {Data}", responseJson);
            }


            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)responseJson;
            }

            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (OperationCanceledException ex) when (timeoutCts.Token.IsCancellationRequested)
        {
            _logger.LogError("WebSocket request timed out after {TimeoutMs}ms for {RequestType}", timeoutMs, typeof(TRequest).Name);
            throw new TimeoutException($"WebSocket request timed out after {timeoutMs}ms", ex);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "WebSocket error during {RequestType} - State: {State}, CloseStatus: {CloseStatus}",
                typeof(TRequest).Name, webSocket.State, webSocket.CloseStatus);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {RequestType} communication - Type: {ExceptionType}",
                typeof(TRequest).Name, ex.GetType().Name);
            throw;
        }
    }

    private string TruncateImageDataForLog(string responseJson)
    {
        try
        {
            // Parse JSON to check if it contains screenshot data
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("type", out var typeElement) &&
                typeElement.GetString() == "success" &&
                root.TryGetProperty("data", out var dataElement) &&
                dataElement.TryGetProperty("image", out var imageElement))
            {
                var imageData = imageElement.GetString();
                if (!string.IsNullOrEmpty(imageData) && imageData.Length > 100)
                {
                    // Create a truncated version for logging
                    var truncatedResponse = responseJson.Replace(imageData, $"[BASE64_IMAGE_DATA_TRUNCATED:{imageData.Length}_chars]");
                    return truncatedResponse;
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, return original
        }

        return responseJson;
    }

    private async Task<string> ReceiveMessageAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];
        var messageBuilder = new List<byte>();
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                throw new WebSocketException("WebSocket connection was closed by the server");
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                throw new WebSocketException($"Unexpected message type: {result.MessageType}");
            }

            messageBuilder.AddRange(buffer.Take(result.Count));
        } while (!result.EndOfMessage);

        return Encoding.UTF8.GetString(messageBuilder.ToArray());
    }

    private async Task CloseWebSocketAsync(ClientWebSocket webSocket)
    {
        try
        {
            if (webSocket?.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _logger.LogDebug("WebSocket connection closed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error while closing WebSocket (expected during cleanup)");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}