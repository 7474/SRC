# SRC# ドキュメント / Documentation

SRC#（Simulation RPG Construction Sharp）の設計・運用のドキュメントです。

- この一覧の文書には**現状だけ**を書きます
- なぜそうしたか（経緯・比較・判断の理由）は [判断の記録](./decisions/README.md) に、過去の計画・調査・移植の作業の記録は [履歴](./history/README.md) に置きます

## 📚 ドキュメント一覧 / Document Index

### 🖥️ GUI / GUI

- **[GUI の構成](./gui/README.md)** — フロントエンド（SRC#Form・SRC#Avalonia・SRC#Web）と、共通の SRCCore・SRCRender・SRCSound の関係
- **[SRC#Avalonia](./gui/avalonia.md)** — Windows / macOS / Linux で動くフロントエンドの構成・WinForms 版との対応・音声・実行方法
- **[SRC#Web（ブラウザ版）](./gui/web.md)** — 静的サイトで動くブラウザ版の設計（マルチスレッド WebAssembly・ブラウザ内保存・タッチ操作・仮想パッド・システムメニュー）
- **[SRC#Web を外部のサイトに置く](./gui/web-hosting.md)** — リリースのパッケージ・サイトの設定・カタログ・素材の同梱・URL での起動
- **[データビューア](./gui/dataviewer.md)** — データビューア（SRC#DataViewer）の頁と、置いたサイトの zip（シナリオ・素材・パック）のデータ・画像・音を読む設定

### 🧪 テスト / Testing

- **[テストと CI の設計](./testing.md)** — ユニット・シナリオ E2E・VRT・フロントエンド E2E の責務、CI の構成と必須チェック、Dependabot の自動マージ、不安定なテストの扱い
- **[E2E テスト](./gui/e2e-testing.md)** — テスト用コーパスをヘッドレスでプレイするシナリオ E2E・VRT と、入力スクリプト（PlayScript）の書式

### 🌐 多言語対応 / Internationalization

- **[多言語対応](./i18n.md)** — 画面の文字列のリソース化と、15 の表示言語の切り替え（SRC#Avalonia・SRC#Web・データビューア・`srcsharp`）

### 🚀 リリース・運用 / Release & Operations

- **[運用の全体像](./operations.md)** — ワークフローのきっかけと出る先、公開しているサイト・パッケージ、7474/SRC-Site とのつながり、Secrets、人がやること（図あり）
- **[バージョンの管理](./versioning.md)** — SRC# のバージョンの付け方とリリースの手順（NuGet・GitHub Releases・Docker Hub）
- **[リリースの案内](./releases/README.md)** — 版ごとの GitHub Releases の本文と、その書き方
- **[ログの設計](./logging.md)** — 各アプリ・ツールのログの出力先・レベル・設定（`srcs.json`・引数・環境変数）・カテゴリ
- **[Static Web Apps のワークフロー](./swa-workflows.md)** — SRC#DataViewer・SRC#Help のデプロイと、残ったプレビュー環境の定期削除

### 🛠️ 作成支援ツール / Authoring Tools

- **[作成支援ツール](./tools/authoring.md)** — データの lint・バランス集計、シナリオの自動プレイ・イベントのデバッガ（CLI `srcsharp` と VS Code 拡張）
- **[VS Code 拡張の使い方](../SRC.Sharp/SRCSharpVSCode/README.md)** — 導入・設定・デバッガの構成・文字コード・トラブルシューティング

### 📦 素材 / Assets

- **[素材](./assets.md)** — リポジトリに含める素材・含めない素材と、素材が無いときの動作

### 🗂️ 判断の記録・履歴 / Decisions & History

- **[判断の記録](./decisions/README.md)** — 設計・運用の判断（背景・判断・理由・影響）
- **[履歴](./history/README.md)** — 移植の作業の記録（計画・評価・不具合の一覧）と、過去の調査

## 🔗 関連リンク / Related Links

- [CONTRIBUTING.md](../CONTRIBUTING.md) — 貢献ガイドライン・コードの規約
- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub](https://github.com/7474/SRC)
- [SRC# デモ（データビューア）](https://7474.github.io/SRC/)
- [SRC#Web（ブラウザ版、リリース版）](https://src.koudenpa.net/)（[7474/SRC-Site](https://github.com/7474/SRC-Site)）
- [SRC#Web（ブラウザ版、開発版）](https://7474.github.io/SRC/web/)
