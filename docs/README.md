# SRC# ドキュメント / Documentation

このディレクトリには SRC#（Simulation RPG Construction Sharp）プロジェクトのドキュメントが含まれています。

## 📚 ドキュメント一覧 / Document Index

### 🔧 移植・品質向上 / Porting & Quality Improvement

移植作業のTODO消化フェーズは完了し、現在は**品質検証・精度向上フェーズ**にあります。

- **[移植ドキュメント](./porting/README.md)** — 移植計画・評価・品質向上計画など

### 🖥️ GUI / GUI

- **[GUI 実装移行の検討](./gui/gui-migration-study.md)** — WinForms から新規 GUI 実装へ移行する場合の候補比較と移行手順
- **[SRC#Avalonia](./gui/avalonia.md)** — Windows / macOS / Linux で動くフロントエンドの構成・WinForms 版との対応・音声・実行方法
- **[E2E テスト方針](./gui/e2e-testing.md)** — テスト用コーパスをヘッドレスでプレイする E2E テストと VRT の方針
- **[バージョンの管理](./versioning.md)** — SRC# のバージョンの付け方とリリースの手順
- **[SRC#Web（ブラウザ版）](./gui/web.md)** — 静的サイトで動くブラウザ版の設計（マルチスレッド WebAssembly・ブラウザ内保存）
- **[SRC#Web を外部のサイトに置く](./gui/web-hosting.md)** — サイトの設定・カタログ・素材の同梱・URL での起動
- **[Static Web Apps のワークフロー](./swa-workflows.md)** — SRC#DataViewer・SRC#Help のデプロイと、残ったプレビュー環境の定期削除

### 🛠️ 作成支援ツール / Authoring Tools

- **[作成支援ツール](./tools/authoring.md)** — データの lint・バランス集計、シナリオの自動プレイ・イベントのデバッガ（CLI `srcsharp` と VS Code 拡張）

### 📦 素材 / Assets

- **[素材データの再配布の検討](./assets/redistribution-study.md)** — 汎用グラフィック集・戦闘アニメ・効果音などの再配布可否と、素材なしで動かす対応

## 🔗 関連リンク / Related Links

- [CONTRIBUTING.md](../CONTRIBUTING.md) — 貢献ガイドライン
- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub](https://github.com/7474/SRC)
- [SRC# デモ](https://7474.github.io/SRC/)
