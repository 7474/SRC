# SRC（Simulation RPG Construction）

SRC（Simulation RPG Construction）の C# .NET への移植版 SRC#（Simulation RPG Construction Sharp）。

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
        - .NET 8
        - GitHub Action: https://github.com/7474/SRC-DataLinter
        - Docker Image: https://hub.docker.com/r/koudenpa/srcdatalinter
            - ![Docker Cloud Build Status](https://img.shields.io/docker/cloud/build/koudenpa/srcdatalinter)
    - [SRCSharpForm](./SRC.Sharp/SRCSharpForm)
        - Windows Forms実装のSRC#Form
        - SRC#Formの仮実行を行える
        - .NET 8
        - 元のSRCとの区別の便宜上、バージョンは `3.x.x` としている
            - SRCCore と比べてメジャーバージョンが +3
        - HelpURL: https://srch.7474.jp/
    - [SRCTestForm](./SRC.Sharp/SRCTestForm)
        - 動作の確認用フォーム
        - データの閲覧を行える
        - .NET 8
    - [SRCTestBlazor](./SRC.Sharp/SRCTestBlazor)
        - 動作確認用Blazor WebAssemblyアプリケーション
        - データの閲覧を行える
        - .NET 8
        - 動作URL:
            - https://7474.github.io/SRC/
            - https://srcv.7474.jp/

### SRC#Form 簡易実行手順

1. Windows 10 64bit版に[.NET 8 ランタイムをインストール](https://docs.microsoft.com/ja-jp/dotnet/core/install/windows)する
1. [Release](https://github.com/7474/SRC/releases) から SRCSharpForm.zip をダウンロード、ないし SRCSharpForm をビルドして実行ファイル（ SRCSharpForm.exe （ビルドした場合は加えて付随するDLL））を得る
1. 構築済のSRCフォルダ内に実行ファイルをコピーする
1. SRCSharpForm.exe を実行する

SRCに付属のサンプルシナリオや https://github.com/7474/SRC-SharpTestScenario で動作確認しながら作っています。

## 📋 移植完了計画 / Migration Completion Plan

SRC#のC#移植を完了するための計画を策定しました。TODOコメントとコメントアウトコードを分析し、約70個のIssueに分類しています。

### ドキュメント / Documentation

- **[📖 クイックスタートガイド](./docs/quick-start.md)** - まずはここから！
- **[📋 移植完了計画](./docs/migration-plan.md)** - プロジェクト全体の概要
- **[📝 個別Issue詳細](./docs/issue-breakdown.md)** - 約70個の具体的なIssueリスト
- **[⚙️ GitHub Projects設定ガイド](./docs/github-projects-setup.md)** - プロジェクト管理の設定方法
- **[📚 docs/README.md](./docs/README.md)** - ドキュメント全体の目次

### 統計 / Statistics

- **TODOコメント**: 155+
- **Epic数**: 8個（戦闘システム、ユニット・パイロット、GUI・UI、イベント・コマンド、データ管理、VB6レガシー、パフォーマンス、バグ修正）
- **推定Issue数**: 約70個
- **推定作業量**: 18,000-25,000行の変更
- **推定期間**: 12-18ヶ月

### クイックスタート / Quick Start

```bash
# 1. ラベル作成
bash docs/scripts/create-labels.sh

# 2. マイルストーン作成
bash docs/scripts/create-milestones.sh

# 3. 進捗確認
bash docs/scripts/progress-report.sh
```

詳細は[クイックスタートガイド](./docs/quick-start.md)を参照してください。
