# SRC#（Simulation RPG Construction Sharp）

[SRC（Simulation RPG Construction）](http://www.src-srpg.jpn.org/) の C# .NET への移植版です。SRC#（SRCシャープ）と呼びます。

元の SRC（VB6）のシナリオとデータをそのまま動かすことを目指しています。Windows に加えて macOS・Linux・ブラウザでも動きます。

## 使う

| 用途 | ソフトウェア | 入手・実行 |
|---|---|---|
| ブラウザで遊ぶ | SRC#Web | https://7474.github.io/SRC/web/ （開発版）。自分のサイトに置くこともできる（[外部のサイトに置く](./docs/gui/web-hosting.md)） |
| Windows・macOS・Linux で遊ぶ | SRC#Avalonia | [Releases](https://github.com/7474/SRC/releases) の `SRCSharpAvalonia-<OS>.zip`・[簡易実行手順](#srcavalonia)・[ドキュメント](./docs/gui/avalonia.md) |
| Windows（x64）で遊ぶ | SRC#Form | [Releases](https://github.com/7474/SRC/releases) の `SRCSharpForm.zip`・[簡易実行手順](#srcform) |
| データを見る | データビューア | https://7474.github.io/SRC/ ・ https://srcv.7474.jp/ |
| データ・シナリオを作る | `srcsharp`（lint・自動プレイ・バランス集計） | [作成支援ツール](./docs/tools/authoring.md) |
| シナリオを VS Code で書く | VS Code 拡張 | Marketplace の [SRC](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode)（`koudenpa.src-vscode`）・[使い方](./SRC.Sharp/SRCSharpVSCode/README.md) |
| CI でデータを検査する | SRCDataLinter | [GitHub Action](https://github.com/7474/SRC-DataLinter)・[Docker イメージ](https://hub.docker.com/r/koudenpa/srcdatalinter) |

ヘルプは https://srch.7474.jp/ にあります。

表示言語は、日本語を既定に 15 の言語から選べます（[多言語対応](./docs/i18n.md)）。

### SRC#Avalonia

1. [Releases](https://github.com/7474/SRC/releases) から、OS に合った zip を取って展開する。.NET を入れる必要はない（自己完結の単一ファイル）。
    - Windows: `SRCSharpAvalonia-win-x64.zip`（`SRCSharpAvalonia.exe`）
    - Linux: `SRCSharpAvalonia-linux-x64.zip`
    - macOS: `SRCSharpAvalonia-osx-arm64.zip`（Apple シリコン）・`SRCSharpAvalonia-osx-x64.zip`（Intel）。署名していないため、初回は Finder で右クリック→「開く」で起動する。
1. `SRCSharpAvalonia <構築済みの SRC フォルダ内のシナリオ（.eve）>` を実行する。
    - ソースから動かすときは、[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) を入れて `dotnet run --project SRC.Sharp/SRCSharpAvalonia -- <シナリオ（.eve）>` を実行する。
    - MIDI を鳴らすには SoundFont（.sf2）が要る（[SoundFont](./docs/gui/avalonia.md#soundfont)）。

### SRC#Form

1. Windows（x64）に [.NET 10 デスクトップ ランタイム](https://dotnet.microsoft.com/download/dotnet/10.0) の x64 版を入れる。
    - 配布している実行ファイルは x64 版だけ。Arm の Windows でも x64 のエミュレーションで動くはずだが、ランタイムも x64 版が要る（[Arm ベースの Windows PC](https://learn.microsoft.com/dotnet/core/install/windows#arm-based-windows-pcs)）。
1. [Releases](https://github.com/7474/SRC/releases) から `SRCSharpForm.zip` を取る。または `SRC.Sharp/SRCSharpForm` をビルドし、実行ファイル（`SRCSharpForm.exe`）と付随する DLL を得る。
1. 構築済みの SRC フォルダに、実行ファイルをコピーする。
1. `SRCSharpForm.exe` を実行する。

## SRC 派生ソフトウェアとしての規約

本リポジトリで開発・配布しているソフトウェアを使うときは、SRC 派生版ソフトウェアの利用における基本的規則を守ってください。

- SRC 公式サイトの[派生版解説ページ](http://www.src-srpg.jpn.org/development_hasei.shtml)
  - [規約（形式１）](http://www.src-srpg.jpn.org/hasei_kiyaku1.html)（[本リポジトリへの転記](src_hasei_kiyaku1.md)）
  - [規約（形式２）](http://www.src-srpg.jpn.org/hasei_kiyaku2.html)（[本リポジトリへの転記](src_hasei_kiyaku2.md)）

## ライセンス

- 全体を GPL-3.0（[LICENSE](./LICENSE)）とする。SRC# 本体とその周辺（ツール・テスト・ドキュメント・VS Code 拡張など）は、元の SRC の意向を尊重してコピーレフトにする
- 例外として、元の SRC から派生していない遊べるシナリオと素材（`SRC.Sharp/Content`）は、コンテンツを [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/deed.ja)、プログラムを MIT で配布する。該当するフォルダに `LICENSE` を置くか、ファイルの先頭に書いている

| パス | ライセンス |
|---|---|
| [SRC.Sharp/Content](./SRC.Sharp/Content) | CC0 1.0（`make.py` は MIT、一部は GPL-3.0。[内訳](./SRC.Sharp/Content/README.md#ライセンス)） |

## リポジトリの構成

| パス | 内容 |
|---|---|
| [SRC.Sharp](./SRC.Sharp) | SRC#（C# の実装。ソリューションは `SRC.Sharp.sln`） |
| [SRC.Sharp.Help](./SRC.Sharp.Help) | SRC# のヘルプのサイト（https://srch.7474.jp/ ） |
| [docs](./docs) | 設計・方針などのドキュメント（[一覧](./docs/README.md)） |
| [SRC/SRC_20121125](./SRC/SRC_20121125) | 移植の元にした SRC（VB6）のソース。参照用に文字コードを UTF-8 にしてある |
| [SRC/Help](./SRC/Help)・[SRC/HelpChm](./SRC/HelpChm) | SRC Ver2.2.33 のヘルプのプロジェクトと、その CHM |
| [SRC.NET](./SRC.NET) | SRC_20121125 をツールで .NET に変換したもの（移植の参考） |

### SRC.Sharp のプロジェクト

| プロジェクト | 内容 | 対象 |
|---|---|---|
| [SRCCore](./SRC.Sharp/SRCCore) | SRC のコア（データ・イベント・式・戦闘など） | .NET Standard 2.1 |
| [SRCRender](./SRC.Sharp/SRCRender) | SkiaSharp による画面の描画と、UI フレームワークに依存しない GUI の実装 | .NET Standard 2.1 |
| [SRCSound](./SRC.Sharp/SRCSound) | OpenAL による音声の再生（WAV・MP3・MIDI） | .NET Standard 2.1 |
| [SRCSharpAvalonia](./SRC.Sharp/SRCSharpAvalonia) | SRC#Avalonia（Windows・macOS・Linux） | .NET 10 |
| [SRCSharpWeb](./SRC.Sharp/SRCSharpWeb) | SRC#Web（Blazor WebAssembly、[ドキュメント](./docs/gui/web.md)） | .NET 10 |
| [SRCSharpForm](./SRC.Sharp/SRCSharpForm) | SRC#Form（Windows Forms、Windows） | .NET 10 |
| [SRCTestBlazor](./SRC.Sharp/SRCTestBlazor) | データビューア（Blazor WebAssembly） | .NET 10 |
| [SRCTestForm](./SRC.Sharp/SRCTestForm) | 動作確認用のフォーム（データの閲覧、Windows） | .NET 10 |
| [SRCDataLinter](./SRC.Sharp/SRCDataLinter) | SRC のデータのバリデータ | .NET 10 |
| [SRCPlay](./SRC.Sharp/SRCPlay)・[SRCAuthoring](./SRC.Sharp/SRCAuthoring)・[SRCDebugAdapter](./SRC.Sharp/SRCDebugAdapter) | 自動プレイ・Language Server・デバッガの部品 | .NET 10 |
| [SRCSharpTools](./SRC.Sharp/SRCSharpTools) | 作成支援ツール `srcsharp`（.NET tool） | .NET 10 |
| [SRCSharpVSCode](./SRC.Sharp/SRCSharpVSCode) | VS Code 拡張 | VS Code |

バージョンは、アプリが `3.x.x`、ライブラリ・ツールが `0.x.x` です。元の SRC と区別するため、アプリはメジャーを 3 にしています（[バージョンの管理](./docs/versioning.md)）。

## 開発

- 貢献の方法は [CONTRIBUTING.md](./CONTRIBUTING.md) を参照してください。
- SRC に付属のサンプルシナリオや https://github.com/7474/SRC-SharpTestScenario で動作を確かめながら作っています。

## 元の SRC

- http://www.src-srpg.jpn.org/
- http://www.src-srpg.jpn.org/development_beta.shtml
