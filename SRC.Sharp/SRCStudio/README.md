# SRC#Studio

データ・シナリオの作成支援の画面（判断は [docs/decisions/2026-11-14-authoring-environment.md](../../docs/decisions/2026-11-14-authoring-environment.md)）。かんたん作成（7474/SRC#1659。[docs/tools/quick-create.md](../../docs/tools/quick-create.md)）、マップ作成ツール（7474/SRC#1641）、データのプレビュー（7474/SRC#1643）を持つ。かんたん作成はスタンドアロンだけ（VS Code では F5 で遊ぶ）。

作成環境の主は VS Code 拡張で、SRC#Studio はその中の画面であると同時に、VS Code を使わずに単独でも使える部品（[作成支援ツール](../../docs/tools/authoring.md) の「VS Code 拡張と、単独で使える部品」）。同じ画面を 2 か所で動かす。

| 動かし方 | 場所 | ファイルの読み書き |
|---|---|---|
| スタンドアロン（Web アプリ） | GitHub Pages の [`/SRC/studio/`](https://7474.github.io/SRC/studio/) | Chrome・Edge は File System Access API でフォルダを直接読み書きする。ほかのブラウザ（Firefox・Safari）はフォルダを読み込み、保存はダウンロード |
| VS Code 拡張 | `.map` のカスタムエディタ（`srcsharp.mapEditor`）。`.vsix` の `studio/` に同じものを入れる | 拡張（`SRCSharpVSCode/src/mapEditor.ts`）が読み書きし、メッセージで画面に渡す。保存・元に戻す・未保存の印は VS Code のもの |

## 構成

| ファイル | 役割 |
|---|---|
| `Program.cs` | 計算（.NET の WebAssembly。`JSExport`）。マップの読み書き（`SRCAuthoring.Maps.MapDocument`）、自動接続（`MapAutoConnect`）、地形データの読み込みとマップの画像の探し方（`TerrainCatalog`）を SRCCore と同じ処理で行う。スレッド・AOT を使わない |
| `wwwroot/studio-core.js` | .NET の実行環境の読み込みと、計算の呼び出し |
| `wwwroot/map-editor.js` | マップ作成ツールの画面（canvas）。ファイルの読み書きは使う側に任せる |
| `wwwroot/index.html`・`studio.js`・`folders.js` | スタンドアロン。フォルダを開き、マップを選んで編集し、保存する |
| `wwwroot/preview.js` | データのプレビューの画面 |
| `wwwroot/assets.js` | 素材の一覧（揃っているかの管理。7474/SRC#1654）の画面 |
| `wwwroot/table-edit.js` | 表で直す（ユニット・パイロットの能力値・地形適応と、武器・アビリティの行を欄ごとに書き換える） |
| `wwwroot/compare.js` | 比べる（7474/SRC-Data の Pages の評価の JSON の中で上位何 %） |
| `wwwroot/vscode.html`・`vscode-boot.js`・`vscode.js` | VS Code の Webview（マップ作成ツール）。拡張とメッセージでやり取りする |
| `wwwroot/vscode-preview.html`・`vscode-preview.js` | VS Code の Webview（データのプレビュー。拡張の `src/preview.ts`） |
| `wwwroot/quick.js` | かんたん作成の画面（作品・ユニット・配置・セリフ・場面のサンプル・下書き） |
| `wwwroot/zip.js` | zip を作る（圧縮しない） |
| `wwwroot/web-handoff.js` | サイトごとの設定（`studio.json`）と、同じサイトの SRC#Web への受け渡し（IndexedDB と `?stored=`） |
| `wwwroot/studio.json` | サイトごとの設定（SRC#Web の場所・素材・作品のデータ・場面のカタログ）。置く側が書き換える |
| `wwwroot/i18n.js` | 画面の文字列（日本語と英語。ほかの言語は英語） |

## マップ作成ツール

- 地形データは SRCCore と同じく、SRC 本体の `Data/System/terrain.txt` を読み、シナリオの `Data/System/terrain.txt` があれば重ねる
- マップの画像は SRCCore の `Map.SearchMapImageFile` と同じく、フォルダ（シナリオ → SRC 本体）ごとに `Bitmap/Map/名前/名前0000.bmp`・`Bitmap/Map/名前0000.bmp`・`Bitmap/Map/名前0.bmp` の順に探す。無ければ地形タイプの色と番号で描く
- SRC 本体のフォルダを開かなければ、同梱の最小の汎用データ（`SRC.Sharp/Content/basic-data`）の地形・画像・スペシャルパワーを使う（画像・地形は CC0、`sp.txt` は GPL-3.0）
- 操作: 描く（右クリックはスポイト）・四角・塗りつぶし・スポイト・写す・貼る、地形とレイヤーの切り替え、レイヤーを消す、自動接続（ON で描くと境目・変種を自動で選ぶ）・つなぎ直し（全体をつなぎ直す。元に戻す可）、元に戻す・やり直す、拡大・縮小、マス目、大きさの変更。マウスを置いたマスの座標と地形を下に出す
- 書き出しは、読んだファイルの書き方（改行・1 行目・予約行・最後の改行）を保つ。変えていないマップは 1 バイトも変わらない（`SRCAuthoringTests` の `MapDocumentTests` と E2E で確かめる）

## 作る

```sh
tools/studio_bundle.sh out/studio
```

`dotnet publish` した `wwwroot` に、最小の汎用データの地形と画像（`basic-data/` と目次 `files.json`）を足す。`out/studio` を配信するか、VS Code 拡張の `studio/` に置く（CI・`Publish-VSCode.yml` は `tools/studio_bundle.sh studio`）。

## テスト

- `SRCAuthoringTests/MapDocumentTests.cs`・`MapAutoConnectTests.cs`: マップの読み書き（リポジトリの全てのマップが変わらない・SRCCore と同じに読める）・自動接続・地形データ・画像の探し方
- `SRCRenderTests/MapChipVrtTests.cs`: `basic-data` のマップチップ一覧・描画画面（1x / 2x）・青ユニット枠との視認性（コントラスト・色覚特性）
- `SRCSharpWeb.E2E/tests/studio.spec.js`: スタンドアロン（`?fs=input` で、フォルダの読み込みとダウンロードの道を通す）・自動接続とつなぎ直し・かんたん作成（7474/SRC-Data の代わりを返す）
- `SRCAuthoringTests/QuickBattleTemplateTests.cs`・`CliTests.cs`: かんたん作成のイベント（作ったシナリオが lint を通り、最後まで遊べる）
- `SRCSharpVSCode/test/extension.test.js`: VS Code でカスタムエディタが開き、画面の WebAssembly が地形とマップを読めること

