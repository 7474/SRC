# SRC - Simulation RPG Construction -

SRCを眺めたり弄ってみたりするためのリポジトリ。

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
    - SRCのC#実装の一部
    - [SRCCore](./SRC.Sharp/SRCCore)
        - SRCのコア部分
        - .NET Standard 2.1
    - [SRCDataLinter](SRC.Sharp/SRCDataLinter)
        - SRCデータのバリデータ
        - .NET 5
        - GitHub Action: https://github.com/7474/SRC-DataLinter
        - Docker Image: https://hub.docker.com/r/koudenpa/srcdatalinter
    - [SRCSharpForm](./SRC.Sharp/SRCSharpForm)
        - Windows Forms実装のSRC#Sharp
        - SRC#Sharpの仮実行を行える
        - .NET 5
    - [SRCTestForm](./SRC.Sharp/SRCTestForm)
        - 動作の確認用フォーム
        - データの閲覧を行える
        - .NET 5
    - [SRCTestBlazor](./SRC.Sharp/SRCTestBlazor)
        - 動作確認用Blazor WebAssemblyアプリケーション
        - データの閲覧を行える
        - .NET 5
        - 動作URL: https://7474.github.io/SRC/

### SRC#Form 簡易実行手順

1. Windows 10 64bit版に[.NET 5 ランタイムをインストール](https://docs.microsoft.com/ja-jp/dotnet/core/install/windows?tabs=net50)する
1. 構築済のSRCフォルダ内のファイルの文字コードをShift_JISからUTF-8に変換する
1. 同様に実行するシナリオのフォルダ内のファイルの文字コードをShift_JISからUTF-8に変換する
1. [Release](./releases) から SRCSharpForm.zip をダウンロード、ないし SRCSharpForm をビルドして実行ファイル（ SRCSharpForm.exe （ビルドした場合は加えて付随するDLL））を得る
1. 文字コードを変換したSRCフォルダ内に実行ファイルをコピーする
1. SRCSharpForm.exe を実行する

koudenpa はSRCに付属のサンプルシナリオで動作確認を試しています。

## Convert log

- VB5 -> VB.NET
    - Visual Basic 2008 Express Edition
- VB.NET -> C#
    - Visual Studio 2019 + Code Converter (VB - C#)
    - https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.CodeConverter
    - https://github.com/icsharpcode/CodeConverter
- HLP -> CHM
    - HLP 形式ヘルプを CHM 形式のヘルプに変換
    - http://mrxray.on.coocan.jp/Delphi/Others/Win32HLP2ChmHLP.htm

### memo

- 基本的に愚直に変換する
    - static な空間は一応インスタンスにする
- 実行環境依存な処理はSRCCoreには含めない
    - GUIプロジェクトに実装する
- short（VBのint）は int にする
- 15Twip = 1Pixel
