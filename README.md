# SRC -Simulation RPG Construction-

SRCを眺めたり弄ってみたりするためのリポジトリ。

## Original

- http://www.src-srpg.jpn.org/
- http://www.src-srpg.jpn.org/development_beta.shtml

## Solution/Project

- [SRC_20121125](./SRC_20121125)
    - 元にしたSRCのコピー
    - 参照用にUTF-8に文字コードを変更してある
- [SRC.NET](./SRC.NET)
    - SRC_20121125をツールで.NETにコンバートしたもの
- [SRC.Sharp](./SRC.Sharp)
    - SRCのC#実装の一部
    - [SRCCore](./SRC.Sharp/SRCCore)
        - SRCのコア部分
        - .NET Standard
    - [SRCTestForm](./SRC.Sharp/SRCTestForm)
        - 動作の確認用フォーム
        - .NET 5

## Convert log

- VB6 -> VB.NET
    - Visual Basic 2008 Express Edition
- VB.NET -> C#
    - Visual Studio 2019 + Code Converter (VB - C#)
    - https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.CodeConverter
    - https://github.com/icsharpcode/CodeConverter

### memo

- 基本的に愚直に変換する
    - static な空間もとりあえずそのまま使う
- short（VB6のint）は int にする
