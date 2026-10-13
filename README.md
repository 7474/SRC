# SRC（Simulation RPG Construction）

SRC（Simulation RPG Construction）の C# .NET への移植版 SRC#（SRCシャープ、Simulation RPG Construction Sharp）。

## SRC派生ソフトウェア

本リポジトリで開発・配布しているソフトウェアの使用にあたってはSRC派生版ソフトウェアの利用における基本的規則を遵守してください。

SRC公式サイト[派生版解説ページ](http://www.src-srpg.jpn.org/development_hasei.shtml)内のSRC派生版ソフトウェアの利用における基本的規則
- [規約(形式１)](http://www.src-srpg.jpn.org/hasei_kiyaku1.html)
- [規約(形式２)](http://www.src-srpg.jpn.org/hasei_kiyaku2.html)

本リポジトリへの転記
- [規約(形式１)](src_hasei_kiyaku1.md)
- [規約(形式２)](src_hasei_kiyaku2.md)

## Original

- http://www.src-srpg.jpn.org/
- http://www.src-srpg.jpn.org/development_beta.shtml

## Solution/Project

- [SRC](./SRC)
    - [SRC_20121125](./SRC/SRC_20121125)
        - 元にしたSRCのコピー
        - 参照用にUTF-8に文字コードを変更してある
    - [Help](./SRC/Help)
        - SRC Ver2.2.33のヘルププロジェクトのコピー
    - [HelpChm](./SRC/HelpChm)
        - HelpをCHM形式に変換したもの
- [SRC.NET](./SRC.NET)
    - SRC_20121125をツールで.NETにコンバートしたもの
- [SRC.Sharp](./SRC.Sharp)
    - SRCのC#実装の一部、SRC#
    - [SRCCore](./SRC.Sharp/SRCCore)
        - SRCのコア部分
        - .NET Standard 2.1
    - [SRCDataLinter](SRC.Sharp/SRCDataLinter)
        - SRCデータのバリデータ
        - .NET 10
        - GitHub Action: https://github.com/7474/SRC-DataLinter
        - Docker Image: https://hub.docker.com/r/koudenpa/srcdatalinter
            - イメージは GitHub Actions でビルドして Docker Hub に置く（`srcsharp` も入っている）
    - [SRCSharpForm](./SRC.Sharp/SRCSharpForm)
        - Windows Forms実装のSRC#Form
        - SRC#Formの仮実行を行える
        - .NET 10
        - 元のSRCとの区別の便宜上、バージョンは `3.x.x` としている（[バージョンの管理](./docs/versioning.md)）
            - SRCCore と比べてメジャーバージョンが +3
        - HelpURL: https://srch.7474.jp/
    - [SRCRender](./SRC.Sharp/SRCRender)
        - SkiaSharp で SRC の画面を描画するライブラリと、UI フレームワークに依存しない GUI 実装
        - .NET Standard 2.1
    - [SRCSound](./SRC.Sharp/SRCSound)
        - OpenAL による音声再生（WAV・MP3・MIDI）
        - .NET Standard 2.1
    - [SRCSharpAvalonia](./SRC.Sharp/SRCSharpAvalonia)
        - Avalonia UI 実装の SRC#Avalonia
        - Windows / macOS / Linux で動作する
        - .NET 10
        - [ドキュメント](./docs/gui/avalonia.md)
    - [SRCSharpWeb](./SRC.Sharp/SRCSharpWeb)
        - ブラウザで動く SRC#Web（Blazor WebAssembly）
        - .NET 10
        - [ドキュメント](./docs/gui/web.md)・[外部のサイトに置く](./docs/gui/web-hosting.md)
        - 動作URL（開発版）: https://7474.github.io/SRC/web/
    - [SRCSharpTools](./SRC.Sharp/SRCSharpTools)
        - データ・シナリオの作成支援ツール `srcsharp`（lint・自動プレイ・バランス集計・Language Server・デバッガ）
        - .NET 10（.NET tool）
        - [ドキュメント](./docs/tools/authoring.md)
    - [SRCSharpVSCode](./SRC.Sharp/SRCSharpVSCode)
        - シナリオ（`*.eve`）の編集支援とイベントのデバッガの VS Code 拡張
        - [使い方](./SRC.Sharp/SRCSharpVSCode/README.md)
    - [SRCTestForm](./SRC.Sharp/SRCTestForm)
        - 動作の確認用フォーム
        - データの閲覧を行える
        - .NET 10
    - [SRCTestBlazor](./SRC.Sharp/SRCTestBlazor)
        - 動作確認用Blazor WebAssemblyアプリケーション
        - データの閲覧を行える
        - .NET 10
        - 動作URL:
            - https://7474.github.io/SRC/
            - https://srcv.7474.jp/

### SRC#Form 簡易実行手順

1. Windows 10 64bit版に[.NET 10 ランタイムをインストール](https://docs.microsoft.com/ja-jp/dotnet/core/install/windows)する
1. [Release](https://github.com/7474/SRC/releases) から SRCSharpForm.zip をダウンロード、ないし SRCSharpForm をビルドして実行ファイル（ SRCSharpForm.exe （ビルドした場合は加えて付随するDLL））を得る
1. 構築済のSRCフォルダ内に実行ファイルをコピーする
1. SRCSharpForm.exe を実行する

### SRC#Avalonia 簡易実行手順

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) をインストールする
1. `dotnet run --project SRC.Sharp/SRCSharpAvalonia -- <構築済のSRCフォルダ内のシナリオ(.eve)>` を実行する
    - MIDI を鳴らすには SoundFont（.sf2）が必要。[SRC#Avalonia のドキュメント](./docs/gui/avalonia.md#soundfont)を参照

表示言語は日本語を既定に 15 の言語に切り替えられる（[多言語対応](./docs/i18n.md)）。

ドキュメントの一覧は [docs/README.md](./docs/README.md) を参照。

SRCに付属のサンプルシナリオや https://github.com/7474/SRC-SharpTestScenario で動作確認しながら作っています。
