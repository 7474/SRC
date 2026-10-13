# E2E テスト方針 / E2E Testing

> 調査日: 2026-10-12（更新: 2026-10-12）

SRC# の E2E テスト（シナリオを実際にプレイして挙動を確認するテスト）と VRT（ビジュアルリグレッションテスト）の方針。

## 結論

- **全コマンドと運用フローを網羅するテスト用データ・シナリオを用意し、入力スクリプトに沿ってヘッドレスでプレイする**
- 検証は2系統で行う
  - **挙動**: 画面の状態（メッセージ、選択肢、マップ、ユニット）と、GUI 呼び出しの記録
  - **見た目（VRT）**: `SRCRender` が描画した画面を基準画像と比較する
- すべて Linux の CI で実行する。フロントエンド固有の UI テストは最小限にする

## 構成

```mermaid
flowchart LR
  subgraph Corpus[テスト用コーパス]
    Data[データ<br/>ユニット・パイロット・地形・マップ]
    Eve[シナリオ .eve<br/>コマンド別・運用フロー別]
    Play[入力スクリプト<br/>クリック・選択・マップ操作]
  end
  subgraph Runner[実行]
    Session[ScreenSession<br/>ゲームスレッドで SRC.Execute]
    GUI[ScreenGUI<br/>画面の状態と待ち]
    Renderer[ScreenRenderer<br/>SkiaSharp で描画]
  end
  subgraph Check[検証]
    State[画面の状態・GUI 呼び出しの記録]
    VRT[VRT<br/>基準画像との比較]
    Coverage[コマンド網羅率]
  end
  Data --> Session
  Eve --> Session
  Play --> GUI
  Session --> GUI --> Renderer
  GUI --> State
  Renderer --> VRT
  Session --> Coverage
```

## テスト用コーパス

| 区分 | 内容 | 例 |
|---|---|---|
| コマンド別 | 各イベントコマンドを1つ以上実行するシナリオ。ヘルプの記載に沿った期待値を持つ | `Talk` / `Ask` / `Set` / `If` / `ChangeMap` / `Create` / `Attack` / `Paint` 系 |
| 運用フロー | 実際のプレイの流れを通しで実行する | プロローグ → 出撃 → 戦闘 → 勝利条件 → エピローグ → インターミッション → 次ステージ、セーブ／ロード、クイックセーブ、中断データ |
| データ | 上記を動かすための最小データ | ユニット・パイロット・武器・アビリティ・地形・マップ・アイテム |
| 互換性 | 元の SRC との挙動差の回帰確認 | [SRC-SharpTestScenario](https://github.com/7474/SRC-SharpTestScenario) のシナリオ |

- 置き場所は `SRC.Sharp/E2E/`（データ・シナリオ・入力スクリプト・基準画像）とする想定
- 乱数を使うシナリオは乱数系列を固定して実行する（`GeneralLib.RndSeed`）
- **網羅率の検査**: 実行したコマンドの種類（`CmdType`）を記録し、コーパス全体で未実行のコマンドがあればテストを失敗させる。これでコマンドの追加・移植漏れを検出できる

### 入力スクリプト（PlayScript）

コーパスは `SRC.Sharp/E2E/ケース名/` に置く。ケースのフォルダをシナリオのフォルダとして使い、`test.eve` から始めて `play.txt` の手順でプレイする（`SRCRenderTests` の `E2ECorpusTests`）。素材はテスト用の最小構成を使い、データ（ユニット・パイロット等）は自作の最小のものをケースに含める。フォルダを追加するだけでテストが増える。

| コマンド | 内容 |
|---|---|
| `auto [秒]` | メッセージ・ダイアログ・リストを自動で進め、マップで操作を受け付ける状態になるまで待つ |
| `wait ready\|message\|list\|menu\|dialog\|click [文字列]` | 各状態になるまで待つ |
| `click X Y [right]` / `cell X Y [right]` | 画面座標・マップのマスをクリックする |
| `menu 項目` / `list 文字列\|#番号` / `dialog ボタン` / `key キー` | メニュー・リスト・ダイアログ・キーの操作 |
| `phase` / `turns 回数` | 味方フェイズを簡単な方針で自動で行う（攻撃できれば攻撃、できなければ敵へ近づいて待機、最後にターン終了）。`turns` は「auto → phase」を繰り返す |
| `choose 見出し 項目` | `auto`・`turns` で、見出し（または説明）にその文字列を含むリストが出たら、その文字列を含む項目を選ぶ。インターミッション（`choose コマンド 次のステージへ`）やシナリオ独自の選択肢の答えを決める。規則の無いリストは先頭の有効な項目を選び、同じリストが繰り返し出る場合は順に別の項目を選び、すべて選んでも抜けられなければキャンセルする |
| `policy advance\|hold` | `phase` の方針。`advance` は攻撃できなければ敵へ近づく（既定）、`hold` は移動せず届く敵だけ攻撃する |
| `print 式` | SRC の式の値を記録する（例: `print ダンジョン階数`） |
| `stall 秒` | 待ちの状態でないまま進まない（`KeyState` でクリックを待つ演出など）ときにボタンを押すまでの時間（既定 5 秒） |
| `snapshot 名前` | 画面を保存する（`TestResults/e2e/ケース名/`、CI の成果物 `screenshots`） |
| `expect finished\|message 文字列\|log 文字列\|error none` | シナリオの終了・表示されたメッセージ・操作の記録・エラーの有無を確かめる |
| `expect eval 式 演算子 式` | SRC の式を評価して比べる（例: `expect eval Damage(標的パイロット) > 0`） |

```text
# battle-basic/play.txt
auto 60
expect message 戦闘開始
turns 3
expect log 攻撃: 標的機
expect eval ターン数 >= 3
expect eval Damage(標的パイロット) > 0
```

`phase` は実際のシナリオ（ロボッとダンジョンＢ！ など）の自動プレイにも使える。シナリオ独自のメニュー（マウスの位置で選ぶもの）は `click` で操作する。

現在のケース:

| ケース | 内容 |
|---|---|
| `battle-basic` | 出撃・移動・待機・ターン終了・敵フェイズ（攻撃と反撃）を 3 ターン |
| `battle-victory` | 敵を倒し「全滅 敵」イベントで勝利してシナリオが終わるまで |
| `intermission-save` | `Continue` でインターミッションに入り、データセーブしてから次のステージへ進んで終わるまで。SRC#Web ではブラウザに残ったセーブデータから再開できることも確かめる |

## VRT

- `SRCRender` は画面全体（マップ・メッセージ・選択肢・ステータス）を SkiaSharp だけで描くため、UI フレームワークや OS の描画系に依存せず、同じ入力から同じ画像が得られる
- 基準画像は `SRCRenderTests/Baselines/*.png`。`VisualAssert.MatchesBaseline` で比較する
  - チャンネルごとに ±24、不一致ピクセル 0.2% までを許容（アンチエイリアスの揺れの吸収）
  - 差分があると `TestResults/vrt/` に `*.actual.png` と `*.diff.png`（差分をマゼンタで表示）を書き出す。CI では成果物 `screenshots` として保存する
  - 基準画像の更新は `SRC_VRT_UPDATE=1 dotnet test SRC.Sharp/SRCRenderTests` で行い、差分をレビューしてコミットする
- **フォントを固定する**: 基準画像は Ubuntu 24.04 + `fonts-noto-cjk`（Noto Sans CJK JP / Noto Sans Mono CJK JP）で作成している。CI の Linux ジョブも同じフォントを入れる。Noto CJK が無い環境（Windows など）では比較せず Inconclusive とする
- **撮影前に描画を待つ**: UI からの入力はゲームスレッドで処理して描画するため、`ScreenScenarioRunner.SnapshotPng` はそれまでに渡した処理が終わるのを待ってから撮る
- 対象: メッセージ・選択肢・`Paint` 系コマンド・素材なしの表示・リスト（多段・出撃選択）・設定変更・メニュー・メッセージボックス・ホットポイント・ドラッグ後のマップ
- 今後の対象: 戦闘画面・移動アニメーション（ユニット・パイロットのテスト用データを用意した後）

## 現在のテスト

| プロジェクト | 内容 | 実行環境 |
|---|---|---|
| `SRCCoreTests` | ユニットテスト。`E2E/HeadlessScenarioTests` は GUI 呼び出しの記録を、ゲームスレッドあり／なしで比較 | Windows / Linux |
| `SRCRenderTests` | `ScreenGUI` でシナリオをプレイし、画面の状態と VRT を検証。リスト・設定変更・メニュー・ダイアログ・マップ操作・文字入力（`IScreenDialogHost`）・`KeyState` | Windows / Linux（VRT は Linux のみ） |
| `SRCSharpAvaloniaTests` | SRC#Avalonia のウィンドウを `Avalonia.Headless` で動かし、マウス入力でシナリオを進める。文字入力ダイアログ。描画結果を `TestResults/avalonia/` に保存 | Windows / Linux |
| `SRCSoundTests` | 音声の復号と、OpenAL Soft の null 出力（`ALSOFT_DRIVERS=null`）での再生 | Windows / Linux（出力を開けない環境では再生のテストは Inconclusive） |
| `SRCSharpFormTestWinAppDriver` | SRC#Form の起動確認 | Windows のみ |
| `SRCTestBlazor/E2E` | Cypress。データ閲覧画面 | ブラウザ |

## フロントエンドごとの UI テスト

シナリオの挙動と見た目は `SRCRender` 側で網羅し、フロントエンドのテストは「画面が表示され、入力が `ScreenGUI` に届くか」に絞る。

| 対象 | 方針 | 根拠 |
|---|---|---|
| SRC#Avalonia | `Avalonia.Headless` で起動・入力・描画・文字入力ダイアログを確認する | [Headless Testing with XUnit](https://docs.avaloniaui.net/docs/testing/headless-xunit)、[Headless Testing Platform](https://docs.avaloniaui.net/docs/concepts/headless/) |
| SRC#Form | WinAppDriver の継続は避け、必要なら FlaUI（UIA3）に置き換える。対象は起動・終了程度 | WinAppDriver は大きな更新がなく保守モードとされる（[TestGuild](https://testguild.com/automation-tools-desktop/)、[FlaUI.WebDriver](https://github.com/FlaUI/FlaUI.WebDriver)）。WinForms 版は移行後に縮退させる前提 |
| SRCTestBlazor | 既存の Cypress を継続 | — |

## 次の作業

1. ~~`SRC.Sharp/E2E/` にコーパスの置き場所と、テキスト形式の入力スクリプトの実行器を作る~~（対応済み）
2. コマンド網羅率の検査を入れ、未実行のコマンドを一覧化する
3. ~~ユニット・パイロットの最小データを用意し、出撃・移動・戦闘・勝利条件までの運用フローを通す~~（`battle-basic` / `battle-victory`）
4. セーブ／ロード、インターミッション、次ステージへの遷移を通す
