# E2E テストと VRT を SRCRender 上のヘッドレスなプレイで行い、フロントエンドの UI テストは最小にする

- 日付: 2026-10-12
- 状態: 採用

## 背景

SRC# のシナリオの挙動と画面を自動で確かめる手段が無かった。SRC#Form の UI テストは WinAppDriver（`SRCSharpFormTestWinAppDriver`）で起動を確かめるだけで、WinAppDriver は大きな更新が無く保守モードとされる（[TestGuild](https://testguild.com/automation-tools-desktop/)）。フロントエンドは SRC#Form に加えて SRC#Avalonia・SRC#Web を作る方針だった（[GUI の構成の判断](./2026-10-12-gui-frontend-architecture.md)）。

## 判断

- コマンドと運用フローを網羅するテスト用のデータ・シナリオ（コーパス、`SRC.Sharp/E2E/`）を用意し、テキストの入力スクリプト（PlayScript）に沿って `SRCRender` の `ScreenGUI` でヘッドレスにプレイする
- 挙動は画面の状態と GUI 呼び出しの記録で、見た目は `SRCRender` が描いた画面を基準画像と比べて（VRT）確かめる
- すべて Linux の CI で動かす。VRT の基準のフォントは Ubuntu 24.04 + `fonts-noto-cjk` に固定する
- フロントエンドのテストは「画面が出て、入力が `ScreenGUI` に届くか」に絞る。SRC#Avalonia は `Avalonia.Headless`、SRC#Web は Playwright。SRC#Form の UI テストを作る場合は WinAppDriver ではなく FlaUI（UIA3）で起動・終了程度にする

## 理由

- `SRCRender` は画面全体を SkiaSharp だけで描くため、UI フレームワークや OS の描画系に依存せず、同じ入力から同じ画像が得られる。フロントエンドごとに同じシナリオのテストを持つ必要が無い
- ヘッドレスなら Linux の CI で速く動かせ、ゲームスレッドあり・なしの GUI 呼び出しを比べられる
- 入力スクリプトは `srcsharp play` やデバッガでも同じ書式で使え、シナリオの作者の自動プレイにも役立つ
- WinAppDriver は保守されておらず、続ける価値が小さい（[FlaUI.WebDriver](https://github.com/FlaUI/FlaUI.WebDriver)）

## 影響

- `SRCRenderTests`（`E2ECorpusTests` など）と `SRCSharpAvaloniaTests`・`SRCSharpWeb.E2E` を作った
- WinAppDriver のテストは CI で動かないまま残り、2026-10-20 に消した（[CI の必須チェックと自動マージ](./2026-10-20-ci-gate-and-auto-merge.md)）

## 参照

- [E2E テスト](../gui/e2e-testing.md)・[テストと CI の設計](../testing.md)
- [Headless Testing with XUnit](https://docs.avaloniaui.net/docs/testing/headless-xunit)
