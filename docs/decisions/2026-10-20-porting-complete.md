# VB から C# への移植を完了とする

- 日付: 2026-10-20
- 状態: 採用

## 背景

移植は、TODO の消化（[移植を TODO の消化から品質の検証に移す](./2026-02-21-porting-quality-phase.md)）の後、元の SRC の動作を期待値にしたテストで移植の不具合を洗い出して直す段階（[元の SRC の動作を期待値にしたテストで移植の不具合を記録する](./2026-10-15-vb-behavior-as-test-oracle.md)）に進んでいた。2026-10-20 に、まだ移植の作業として扱うべきものが残っているかを確かめた。

| 観点 | 結果 |
|---|---|
| イベントコマンド | `SRCCore/CmdDatas/CmdParser.cs` で、すべてのコマンドに実装がある。`NotImplementedCmd` を返すコマンドは無い。`PlayFlash`・`ClearFlash` だけは Flash の実行環境が無くなったため `NotSupportedCmd`（対応しない） |
| `// TODO` | SRCCore 10 件・SRCRender 1 件・SRCSharpForm 6 ファイル（コメントアウトした変換前のコードに残る `TODO ERROR` は除く）。どれも精査・最適化・設計の見直し（セーブの互換性の精査、PaintString の事前の構文解析、SRC#Form の描画の課題など）で、移植していない機能ではない |
| 移植の不具合 | 元の SRC と動作が違い、直していないもの（`[Ignore("移植の不具合…")]` のテスト）は 0 件 |
| テスト | `SRCCoreTests` の `[TestMethod]` は 10,026、`SRCRenderTests` は 62。シナリオ E2E のコーパスは 25 ケース（[テストと CI の設計](../testing.md)） |
| Issue | 移植の作業の Issue は無い。開いているのは、フロントエンドの改善（高 DPI・高速化・配布）とその他の機能の追加 |
| リリース | SRC# 3.5.4 まで、SRC#Form・SRC#Avalonia・SRC#Web を公開している |

## 判断

- VB から C# への移植は完了とする。以後は、元の SRC と動作が違う箇所が見つかったら、移植の作業ではなく通常の不具合（[不具合の報告](https://github.com/7474/SRC/issues/new?template=bug-report.yml)）として扱う
- 移植のための文書（計画・評価・Epic の分解・不具合の一覧・移植の現状）は、過去の施策の記録として `docs/history/porting/` に置く
- 移植の作業のための Issue テンプレート（Epic・移行タスク・TODO に基づく機能の実装・バグ修正）は削除する
- コードを書くときの規約（VB からの変換の名残を含む）は [CONTRIBUTING.md](../../CONTRIBUTING.md) に置く
- 残った `// TODO` は、コードの改善の課題としてコードに残す

## 理由

- 未移植の機能が無く、見つかった移植の不具合も直し終えている
- 移植の作業のための文書とテンプレートが残っていると、終わった作業が続いているように見え、現状の文書と食い違う（テンプレートが付けるラベル `migration`・`type:epic` なども、もう無い）

## 影響

- `docs/porting.md` を [docs/history/porting/README.md](../history/porting/README.md) に移した
- `.github/ISSUE_TEMPLATE/` は `bug-report.yml` だけになった

## 参照

- [移植の記録](../history/porting/README.md)
