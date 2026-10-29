# フロントエンドにサポート階層を設け、SRC#Form を維持の階層にする

- 日付: 2026-10-29
- 状態: 提案（[#1337](https://github.com/7474/SRC/issues/1337) で検討中）

## 背景

- SRC# には、ゲームをプレイするフロントエンドが 3 つある（[GUI の構成](../gui/README.md)）
  - SRC#Web（`SRCSharpWeb`、ブラウザ）と SRC#Avalonia（`SRCSharpAvalonia`、Windows・macOS・Linux）は、共通の `SRCRender`・`SRCSound` の上の薄い層
  - SRC#Form（`SRCSharpForm`、Windows Forms）は元の SRC（VB6）の画面をそのまま移したもので、`SRCRender` を使わない
- [GUI の判断](./2026-10-12-gui-frontend-architecture.md)では、SRC#Form を「新しいフロントエンドの機能が揃うまでの比較対象・利用者の移行先」として残した。どこまで手を入れるかは決めていなかった
- 新しい機能・修正を 3 つのフロントエンドすべてに入れると、SRC#Form の分だけ実装と確認が増える。SRC#Form は Windows でしか動かず、CI でもビルドと発行しか確かめていない（[テストと CI の設計](../testing.md)）
- 一方で SRC#Form は VB 版との見た目の移植度が高く、`SRCRender` の表示を合わせる基準にもなっている（`SRCRender/Compat`）

## 判断

### 階層の定義

| 階層 | 意味 | 新機能 | 不具合の修正 | CI | リリース物 |
|---|---|---|---|---|---|
| 推奨 | 開発・検証の軸。利用者に最初に案内する | 先に対応する | 対応する | ビルドとテスト（E2E を含む） | 出す |
| 維持 | 削除はしないが、手を入れる範囲を絞る | 後追い、または対応しない | 重大な不具合と、VB 版との互換性に関わる不具合を、可能な範囲で直す | ビルドと発行の確認 | 出す |
| 非推奨 | 削除を予定する。新しい利用を勧めない | 対応しない | 原則として直さない | 外してよい | 出さなくてよい |

### 各フロントエンドへの割り当て

| フロントエンド | 階層 |
|---|---|
| SRC#Web（`SRCSharpWeb`） | 推奨 |
| SRC#Avalonia（`SRCSharpAvalonia`） | 推奨 |
| SRC#Form（`SRCSharpForm`） | 維持 |

データビューア（`SRCTestBlazor`）・動作確認用のフォーム（`SRCTestForm`）はプレイ用のフロントエンドではないため、この階層の対象にしない。

### SRC#Form に引き続き行うこと

- 修正するもの: 起動できない・進行できない・データが壊れるなどの重大な不具合と、VB 版と動作・表示が違う不具合。`SRCCore` の修正で直るものはすべてのフロントエンドで直る
- 見た目の基準: VB 版との見た目の突き合わせは SRC#Form で続け、`SRCRender` の表示を合わせる参照にする
- 新機能: 推奨のフロントエンドを先に作る。SRC#Form への対応は後追いとし、Windows Forms で実装の費用が大きいものは対応しない。`IGUI` にメンバーを足すときは、SRC#Form では何もしない実装を置いてよい
- 元の SRC 2.3 以降（特に新 GUI）の機能は、これまでどおり SRC#Form では原則として受け付けない（[CONTRIBUTING.md](../../CONTRIBUTING.md)）

### CI・リリース物

- CI の `build-and-test-srcs`（Windows）はソリューション全体のビルドと SRCCore の Windows でのテストも兼ねるため、残す。SRC#Form の発行の確認（`Try Publish`）も残す
- リリースの `SRCSharpForm.zip` は出し続け、README で「維持」の階層だと案内する

### 利用者への告知

- README の「使う」の表と、[GUI の構成](../gui/README.md) に各フロントエンドの階層を書く
- 次のリリースのリリースノートに、階層を設けたことと SRC#Form の扱いを書く
- SRC#Web のリリース版のサイト（[7474/SRC-Site](https://github.com/7474/SRC-Site)）での案内は、そのリポジトリで行う

### SRC#Avalonia の位置づけ

- 推奨とする。ただし、SRC#Avalonia で効果音が鳴らないという報告（環境依存の可能性があり未確認）があるため、確認と修正は別の Issue で行う。確認の結果、推奨の水準に届かないと分かった場合はこの記録を見直す

## 理由

- SRC#Web と SRC#Avalonia は `SRCRender`・`SRCSound` を共有しており、1 回の実装で Windows・macOS・Linux・ブラウザに届く。ヘッドレスな E2E・VRT も `SRCRender` の上で動く（[ヘッドレスな E2E・VRT](./2026-10-12-headless-e2e-vrt.md)）
- SRC#Form は Windows 専用で、変更の確認を CI で自動化できる範囲が狭い。すべての変更を追うと、推奨のフロントエンドの開発が遅れる
- 削除せず「維持」にするのは、VB 版との見た目の移植度が高く、Windows の利用者の選択肢と、表示を合わせる基準として価値があるため

## 影響

- 新機能の PR は、SRC#Form に対応していなくても受け付ける
- SRC#Form だけで起きる不具合の報告は、重大さと VB 版との互換性で対応するかを決める
- 将来 SRC#Form を「非推奨」に下げる場合は、この記録を置き換える新しい記録を書く

## 参照

- [#1337 SRC#Form のサポート階層を下げ、Web 版を含むマルチプラットフォームを軸にする方針の整理](https://github.com/7474/SRC/issues/1337)
- [GUI を SRCRender と新しいフロントエンド（Avalonia・Web）に移し、WinForms 版は残す](./2026-10-12-gui-frontend-architecture.md)
- [GUI の構成](../gui/README.md)
