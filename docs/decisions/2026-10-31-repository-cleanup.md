# 役目を終えたコード・ツール・ワークフローを消す

- 日付: 2026-10-31
- 状態: 採用

## 背景

[移植の完了](./2026-10-20-porting-complete.md) の後も、移植やヘルプの変換のために作ったものが残っていた。2026-10-31 に、ワークフローの実行の記録・参照・更新の履歴を調べた。

| 対象 | 調べた結果 |
|---|---|
| `SRC.NET/`（100 ファイル） | SRC_20121125 をツールで変換した C#。移植の参考。どこからも参照されていない。互換性の正は VB 版のソース（`SRC/SRC_20121125`） |
| `memo/` | `SRC.NET` を grep した結果などの、移植の作業のメモ |
| `tools/ReplaceArgx`・`tools/Template` | 移植のときのコードの一括置換と、コマンド・関数のひな形の生成。`ReplaceArgx` は作者の Windows のパスを決め打ちしている |
| `tools/ToMackerel`・`CodeInfo.yml` | master への push ごとに、`// TODO` と `NotImplementedException` の数を Mackerel に送っていた。移植の進み具合の指標で、移植の完了で役目を終えた |
| `SRC.Sharp.Help/AddJekyllHeader`・`ExampleConv`・`HelpConverter` | 元の SRC のヘルプ（HTML）を Markdown にする 1 回きりの変換。`AddJekyllHeader`・`ExampleConv` は作者の Windows のパスを決め打ちしている。ヘルプは今は直接書いている。`HelpConverter` は CI でビルドしておらず、Dependabot の更新を人がマージする必要があった |
| `SRC.Sharp/SRCTestForm` | データを見るための Windows のフォーム。2021 年から、フレームワークと依存パッケージの更新しかしていない。データを見るのは[データビューア](../gui/dataviewer.md)（`SRCTestBlazor`）が受け持っている |
| `label.yml`・`labeler.yml` | PR に SRCCore・SRC#Form・SRCTestBlazor・SRCDataLinter のラベルを付けていた。ほかのプロジェクト（SRC#Web・SRC#Avalonia・作成支援ツールなど）には付かず、ラベルを使うものも無い |

ほかのワークフロー（CI・GitHub Pages と Static Web Apps への配信・プレビュー環境の片付け・公開・Dependabot の自動マージ）は、直近の実行が成功していて、役目もある。

## 判断

上の表のものを消す。参照（README・ソリューション・Dependabot・CI の文書だけの判定・テストの文書）も直す。

## 理由

- 残っていると、終わった作業が続いているように見え、Dependabot の更新とビルドの手間だけがかかる
- 消したものは git の履歴から取り出せる

## 影響

- Mackerel のサービス `SRCS` に `TodoCount`・`NotImplCount` が送られなくなる。カバレッジ（`CI.yml` の `coverage`）は今までどおり送る
- `tools/` は `release.py` だけになった

## 参照

- [VB から C# への移植を完了とする](./2026-10-20-porting-complete.md)
- [フロントエンドにサポート階層を設け、SRC#Form を維持の階層にする](./2026-10-29-frontend-support-tiers.md)
