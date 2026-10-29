# 移植の記録 / Porting Archive

> 記録として残している文書です（2026-10-20 時点）。VB から C# への移植は 2026-10-20 に完了とした（[判断の記録](../../decisions/2026-10-20-porting-complete.md)）。コードの規約は [CONTRIBUTING.md](../../../CONTRIBUTING.md)、テストは [テストと CI の設計](../../testing.md) を参照してください。

SRC#（`SRC.Sharp`）は、元の SRC（VB5。`SRC/SRC_20121125`）を C# に移植したものです。この文書は、移植を完了としたときの状態・移植の規約・品質の確かめ方をまとめたものです。

## このフォルダの文書

| 文書 | 内容 |
|---|---|
| [porting-assessment.md](./porting-assessment.md) | 移植状況の総合評価（2026-02-20） |
| [porting-quality-plan.md](./porting-quality-plan.md) | 移植精度の評価・向上計画（2026-02-20） |
| [migration-plan.md](./migration-plan.md) | 移植完了計画と、TODO の分類（2026-02） |
| [issue-breakdown.md](./issue-breakdown.md) | Epic ごとの Issue の分解（2026-02） |
| [coverage-porting-bugs.md](./coverage-porting-bugs.md) | カバレッジを上げる過程で見つけた移植の不具合と、その修正（2026-10） |
| [../conversion-log.md](../conversion-log.md) | VB から C# に変換したときに使ったツール |

## 状態

- 元の SRC のシナリオとデータをそのまま動かすことを目標にしている
- コードに残っている課題は `// TODO` として書いてある精査・最適化・設計の見直しで、一覧は[下の表](#残っている-todo)のとおり
- 元の SRC の動作を期待値にしたテストで見つかった、元の SRC と動作が違う箇所（移植の不具合）で直していないものは無い（[既知の移植の不具合](#既知の移植の不具合)）

### 残っている TODO

`grep -rn "// TODO" SRC.Sharp --include=*.cs` で一覧できる（テストのプロジェクトを除く）。

| プロジェクト | ファイル | 内容 |
|---|---|---|
| SRCCore | `COM.cs` | 武器の選択に失敗する場合がある（起きたら警告のログを出し、そのターンの行動を中断する） |
| SRCCore | `CmdDatas/CmdParser.cs` | Talk の中身が壊れる場面があるかもしれない |
| SRCCore | `CmdDatas/Commands/Other/MakePilotListCmd.cs` | パイロットの愛称の描画を GUI に切り出す（セルの大きさ 32px をコアで決め打ちしている） |
| SRCCore | `CmdDatas/Commands/Screan/PaintStringCmd.cs` | 速くするため、PaintString の文をあらかじめ構文解析しておく |
| SRCCore | `Config/LocalFileConfig.cs` | 設定の項目の説明を設定ファイルに書けるようにする |
| SRCCore | `Expressions/Expression.replace.cs` | `ref` の引数を戻り値か `out` に変えるかを決める |
| SRCCore | `Lib/GeneralLib.cs` | バイト長の関数と統合する |
| SRCCore | `SRC.save.cs` | セーブ・ロードの元の SRC との互換性を精査する |
| SRCCore | `Sound.cs` | 一度探したファイルを探し直している（絶対パスを返す API を作れば省ける） |
| SRCCore | `Units/Unit.pilot.cs` | 呼び出し元で追加サポートを扱っていたかを精査する |
| SRCRender | `ScreenGUI.status.cs` | 入力系と描画系のインタフェースの切り方を見直す |
| SRCSharpForm | `Forms/Main.guimap.cs`・`Forms/Main.guiscrean.cs`・`Forms/Main.guistatus.cs` | 入力系と描画系のインタフェースの切り方を見直す |
| SRCSharpForm | `Forms/Main.guimap.cs` | ユニットのタイルの読み込み元を変える。地形ユニットのフィルタを実装する |
| SRCSharpForm | `Forms/Main.guiscrean.cs` | FillStyle は単色の塗りつぶしだけで、透過・ハッチなどに対応していない |
| SRCSharpForm | `Forms/Main.cs` | 描画の課題を見直す（[7474/SRC#816 のレビュー](https://github.com/7474/SRC/pull/816#discussion_r2867096631)） |
| SRCSharpForm | `Resoruces/ImageBuffer.cs` | アルファチャネルの扱いを精査する |
| SRCSharpForm | `RootForm.cs` | 設定の変更を反映する（いまは SoundVolume だけ反映する） |

## 移植の規約

- 元の SRC のコードを愚直に変換する。VB のモジュール（static な空間）は、C# ではインスタンスにする
- 実行環境に依存する処理（画面・入力・音声・ファイルの選択など）は `SRCCore` に入れず、GUI のプロジェクト（`SRCRender`・`SRCSharpForm` など）に実装する。`SRCCore` からは `IGUI` などのインタフェースを通して呼ぶ
- VB の `Integer`（16 ビット）は `int` にする
- 座標は 15 Twip = 1 ピクセルで換算する
- VB で Double を Integer・Long に代入する箇所は銀行丸め（偶数丸め）になる。C# の `(int)` キャスト（切り捨て）ではなく、`SRCCore/VB/VBMath.cs`・`SRCCore/VB/Conversions.cs` の関数（`VBMath.CLng` など）を使う
- 元の SRC の動作を仕様とする。動作を確かめるときは `SRC/SRC_20121125` の該当するコードと照らし合わせる

## 既知の移植の不具合

元の SRC と動作が違う箇所を見つけたら、次のようにする。

1. 元の SRC の動作を期待値にしたテストを `SRC.Sharp/SRCCoreTests` に書く
2. すぐに直せない場合は、そのテストに `[Ignore("移植の不具合: <概要>")]` を付けて残す
3. 直したら `[Ignore]` を外し、テストが通ることを確かめる

直していない移植の不具合は `grep -rn 'Ignore("移植の不具合' SRC.Sharp/SRCCoreTests` で一覧できる。いまは 0 件。

## 品質の確かめ方

移植の正しさは、ユニットテスト（`SRCCoreTests`）と、テスト用のシナリオをヘッドレスでプレイするシナリオ E2E・VRT（`SRCRenderTests`）で確かめる。テストの層ごとの責務と CI の構成は [テストと CI の設計](../../testing.md)、シナリオ E2E・VRT の詳細は [E2E テスト方針](../../gui/e2e-testing.md) を参照。

元の SRC との挙動の違いを確かめるためのシナリオは [SRC-SharpTestScenario](https://github.com/7474/SRC-SharpTestScenario) にある。

## 関連

- [判断の記録: 移植を TODO の消化から品質の検証に移す](../../decisions/2026-02-21-porting-quality-phase.md)
- [判断の記録: 元の SRC の動作を期待値にしたテストで移植の不具合を記録する](../../decisions/2026-10-15-vb-behavior-as-test-oracle.md)
- [判断の記録: VB から C# への移植を完了とする](../../decisions/2026-10-20-porting-complete.md)
