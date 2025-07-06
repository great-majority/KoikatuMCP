# KoikatuMCP

[![Build and Test](https://github.com/great-majority/KoikatuMCP/actions/workflows/ci.yml/badge.svg)](https://github.com/great-majority/KoikatuMCP/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

ClaudeなどのLLMチャットでKoikatsu Studioを制御できるようにするModel Context Protocol (MCP) サーバーです。

## 必要な環境

- **Koikatsu (KK)** または **Koikatsu Sunshine (KKS)**
- **[KKStudioSocket](https://github.com/great-majority/KKStudioSocket)** プラグイン
- **Claude等のMCPクライアント対応LLMシステム**

## インストール

### 1. KKStudioSocketプラグインのインストール
[KKStudioSocketインストールガイド](https://github.com/great-majority/KKStudioSocket?tab=readme-ov-file#-installation)に従ってWebSocketプラグインをセットアップしてください。

### 2. KoikatuMCPのダウンロード
1. [Releases](https://github.com/great-majority/KoikatuMCP/releases)から最新版をダウンロード
2. ZIPファイルを任意の場所に展開
3. アーカイブに含まれるファイル:
   - `KoikatuMCP.exe` (単一実行ファイル)
   - `README.md` (このファイル)
   - `LICENSE` (ライセンス情報)

## Claude Desktopでの設定

Claude DesktopのMCP設定に以下の設定を追加してください：

### Claude Desktop設定経由
1. `KoikatuMCP.exe`のパスを確認 (展開したフォルダ内の`KoikatuMCP.exe`ファイルを右クリック → 「パスのコピー」を選択)
2. Claude Desktopを開く
3. **設定** → **開発者** → **設定を編集**に移動
4. `%APPDATA%\Claude`でエクスプローラーが開かれるので、テキストエディターで`claude_desktop_config.json`を開く
5. 以下のように記述し、ファイルを保存:

```json
{
  "mcpServers": {
    "koikatu-mcp": {
      "command": "KoikatuMCP.exeのフルパスをここに貼り付け",
      "env": {
        "KKSTUDIOSOCKET_URL": "ws://127.0.0.1:8765/ws"
      }
    }
  }
}
```

⚠️ 注意: パス内のバックスラッシュは二個書いてエスケープする必要があります。
例:
```
❌ 間違い: "C:\Users\user\Downloads\KoikatuMCP.exe"  
✅ 正しい: "C:\\Users\\user\\Downloads\\KoikatuMCP.exe"
```

### 設定ファイル経由 (Claude Code、Gemini CLI等)
`claude_desktop_config.json`に追加:
```json
{
  "mcpServers": {
    "koikatu-mcp": {
      "command": "C:\\path\\to\\KoikatuMCP.exe"
    }
  }
}
```

## 使用方法

### 1. Koikatsu Studioの起動
1. KoikatsuまたはKoikatsu Sunshine Studioを起動
2. KKStudioSocketが有効であることを確認 (ポート8765でWebSocketサーバーが動作)

`output_log.txt`に以下の行が含まれるかどうかで、KKStudioSocketが正しく読み込まれているかチェックできます:
```
[Info   :   BepInEx] Loading [KKStudioSocket 1.0.0]
[Info   :KKStudioSocket] Awaking KKStudioSocket v1.0.0
```

### 2. Claude Desktopの起動
MCP設定が完了していれば、会話を開始した際にKoikatuMCPが自動的に接続されます。

### 3. 言葉でLLMチャットに話してみる

#### 接続確認

```
> 接続確認のために、pingしてください！

● KKStudioSocketへの接続確認を実行します！

● koikatu-mcp:ping (MCP)(message: "接続確認テスト")
  ⎿  ✅ Ping successful! Server responded with: 接続確認テスト

● ✅ 接続成功！

  KKStudioSocketとの接続が正常に確立されています。WebSocketサーバーが正しく動作しており、KoikatuMCPがKoikatsu Studioと通信できる状態です。

  これで、シーンの操作、アイテムの追加、カメラ制御などの全ての機能が利用可能になりました！
```

#### 配置されているアイテムを調べさせる

```
> 現在のシーンにはどのようなアイテムが配置されていますか？

● 現在のシーンに配置されているアイテムを確認するために、シーンツリーを取得してみましょう。

● koikatu-mcp:tree (MCP)(depth: 2)
  ⎿  🌲 Scene Tree:
     📦 フォルダー (ID: 127, Type: OCIFolder)
        🎯 Position: (3.79, -1.54, -16.95)
     … +821 lines (ctrl+r to expand)

● 現在のシーンには以下のアイテムが配置されています：

  主要なオブジェクト

  1. キャラクター

  - 篠崎 遥 (ID: 0) - 女性キャラクター
    - 位置: (0.00, 0.00, 0.00)

  2. アイテム
...
```

## 利用可能なコマンド

### シーン管理
- オブジェクトの追加 (アイテム、ライト、カメラ、キャラクター)
- オブジェクトプロパティの変更 (位置、回転、スケール、色)
- シーンからオブジェクトの削除
- シーンの階層構造の取得

### カメラ制御
- カメラの位置と向きの設定
- カメラビューの切り替え
- スクリーンショットの撮影

### アイテムリストの確認
- 利用可能なアイテムカタログの閲覧
- 特定アイテムの検索
- 詳細なアイテム情報の取得

### オブジェクト操作
- オブジェクトの階層作成のためのアタッチ/デタッチ
- 可視性と外観の変更
- ライティングプロパティの調整

## ライセンス

このプロジェクトはMITライセンスの下で公開されています - 詳細は[LICENSE](LICENSE)ファイルを参照してください。

## リンク

- [KKStudioSocketプラグイン](https://github.com/great-majority/KKStudioSocket)
- [Model Context Protocol](https://modelcontextprotocol.io/)
- [コントリビューションガイド](CONTRIBUTING.md)