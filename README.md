# SRC - Simulation RPG Construction -

SRCを眺めたり弄ってみたりするためのリポジトリ。

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
        - .NET Standard
    - [SRCDataLinter](SRC.Sharp/SRCDataLinter)
        - SRCデータのバリデータ
        - .NET 5
        - GitHub Action: https://github.com/7474/SRC-DataLinter
    - [SRCTestForm](./SRC.Sharp/SRCTestForm)
        - 動作の確認用フォーム
        - データの閲覧とWindows Forms実装のSRC#Sharp仮実行を行える
        - .NET 5
    - [SRCTestBlazor](./SRC.Sharp/SRCTestBlazor)
        - 動作確認用Blazor WebAssemblyアプリケーション
        - データの閲覧を行える
        - .NET 5
        - 動作URL: https://7474.github.io/SRC/

## Convert log

- VB6 -> VB.NET
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
- short（VB6のint）は int にする
