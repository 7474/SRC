# SRC#Studio

データ・シナリオの作成支援の画面（判断は [docs/decisions/2026-11-14-authoring-environment.md](../../docs/decisions/2026-11-14-authoring-environment.md)）。今はマップ作成ツールを持つ（7474/SRC#1641）。

同じ画面を 2 か所で動かす。

| 動かし方 | 場所 | ファイルの読み書き |
|---|---|---|
| スタンドアロン（Web アプリ） | GitHub Pages の [`/SRC/studio/`](https://7474.github.io/SRC/studio/) | Chrome・Edge は File System Access API でフォルダを直接読み書きする。ほかのブラウザ（Firefox・Safari）はフォルダを読み込み、保存はダウンロード |
| VS Code 拡張 | `.map` のカスタムエディタ（`srcsharp.mapEditor`）。`.vsix` の `studio/` に同じものを入れる | 拡張（`SRCSharpVSCode/src/mapEditor.ts`）が読み書きし、メッセージで画面に渡す。保存・元に戻す・未保存の印は VS Code のもの |

## 構成

| ファイル | 役割 |
|---|---|
| `Program.cs` | 計算（.NET の WebAssembly。`JSExport`）。マップの読み書き（`SRCAuthoring.Maps.MapDocument`）、地形データの読み込みとマップの画像の探し方（`TerrainCatalog`）を SRCCore と同じ処理で行う。スレッド・AOT を使わない |
| `wwwroot/studio-core.js` | .NET の実行環境の読み込みと、計算の呼び出し |
| `wwwroot/map-editor.js` | マップ作成ツールの画面（canvas）。ファイルの読み書きは使う側に任せる |
| `wwwroot/index.html`・`studio.js`・`folders.js` | スタンドアロン。フォルダを開き、マップを選んで編集し、保存する |
| `wwwroot/vscode.html`・`vscode-boot.js`・`vscode.js` | VS Code の Webview。拡張とメッセージでやり取りする |
| `wwwroot/i18n.js` | 画面の文字列（日本語と英語。ほかの言語は英語） |

## マップ作成ツール

- 地形データは SRCCore と同じく、SRC 本体の `Data/System/terrain.txt` を読み、シナリオの `Data/System/terrain.txt` があれば重ねる
- マップの画像は SRCCore の `Map.SearchMapImageFile` と同じく、フォルダ（シナリオ → SRC 本体）ごとに `Bitmap/Map/名前/名前0000.bmp`・`Bitmap/Map/名前0000.bmp`・`Bitmap/Map/名前0.bmp` の順に探す。無ければ地形タイプの色と番号で描く
- SRC 本体のフォルダを開かなければ、同梱の最小の汎用データ（`SRC.Sharp/Content/basic-data`、CC0）の地形と画像を使う
- 操作: 描く（右クリックはスポイト）・四角・塗りつぶし・スポイト・写す・貼る、地形とレイヤーの切り替え、レイヤーを消す、元に戻す・やり直す、拡大・縮小、マス目、大きさの変更。マウスを置いたマスの座標と地形を下に出す
- 書き出しは、読んだファイルの書き方（改行・1 行目・予約行・最後の改行）を保つ。変えていないマップは 1 バイトも変わらない（`SRCAuthoringTests` の `MapDocumentTests` と E2E で確かめる）

## 作る

```sh
tools/studio_bundle.sh out/studio
```

`dotnet publish` した `wwwroot` に、最小の汎用データの地形と画像（`basic-data/` と目次 `files.json`）を足す。`out/studio` を配信するか、VS Code 拡張の `studio/` に置く（CI・`Publish-VSCode.yml` は `tools/studio_bundle.sh studio`）。

## テスト

- `SRCAuthoringTests/MapDocumentTests.cs`: マップの読み書き（リポジトリの全てのマップが変わらない・SRCCore と同じに読める）・地形データ・画像の探し方
- `SRCSharpWeb.E2E/tests/studio.spec.js`: スタンドアロン（`?fs=input` で、フォルダの読み込みとダウンロードの道を通す）
- `SRCSharpVSCode/test/extension.test.js`: VS Code でカスタムエディタが開き、画面の WebAssembly が地形とマップを読めること
