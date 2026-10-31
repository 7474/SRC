# E2E テスト / E2E Testing

SRC# の E2E テスト（シナリオを実際にプレイして挙動を確認するテスト）と VRT（ビジュアルリグレッションテスト）の仕組み。テスト全体の層と CI での扱いは [テストと CI の設計](../testing.md)、この方式を選んだ理由は [判断の記録](../decisions/2026-10-12-headless-e2e-vrt.md) にある。

## 方式

- **コマンドと運用フローを網羅するテスト用データ・シナリオ（コーパス）を、入力スクリプトに沿ってヘッドレスでプレイする**
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

- 置き場所は `SRC.Sharp/E2E/`（データ・シナリオ・入力スクリプト）。基準画像は `SRCRenderTests/Baselines/` に置く
- 乱数を使うシナリオは乱数系列を固定して実行する（`GeneralLib.RndSeed`）
- **コマンド網羅率**: master への push のとき、CI の `coverage` ジョブが `SRCRenderTests` のカバレッジから、コーパスと画面のテストで実行したイベントコマンドの割合を `.github/scripts/command-coverage.py` で集計し、ジョブのサマリーに出す（テストは失敗させない）。カバレッジの収集は後処理に数分かかるため PR では行わない

### 入力スクリプト（PlayScript）

コーパスは `SRC.Sharp/E2E/ケース名/` に置く。ケースのフォルダをシナリオのフォルダとして使い、`test.eve` から始めて `play.txt` の手順でプレイする（`SRCRenderTests` の `E2ECorpusTests`）。素材はテスト用の最小構成を使い、データ（ユニット・パイロット等）は自作の最小のものをケースに含める。フォルダを追加するだけでテストが増える。

PlayScript の実装は `SRC.Sharp/SRCPlay` にある。作成支援ツールの `srcsharp play --script` やデバッガでも同じ書式を使う（[作成支援ツール](../tools/authoring.md)）。

| コマンド | 内容 |
|---|---|
| `auto [秒]` | メッセージ・ダイアログ・リストを自動で進め、マップで操作を受け付ける状態になるまで待つ |
| `wait ready\|message\|list\|menu\|dialog\|click [文字列]` | 各状態になるまで待つ |
| `click X Y [right]` / `cell X Y [right]` | 画面座標・マップのマスをクリックする |
| `menu 項目` / `list 文字列\|#番号` / `dialog ボタン` / `key キー` | メニュー・リスト・ダイアログ・キーの操作 |
| `phase` / `turns 回数` | 味方フェイズを簡単な方針で自動で行う（攻撃できれば攻撃、できなければ敵へ近づいて待機、最後にターン終了）。`turns` は「auto → phase」を繰り返す |
| `choose 見出し 項目` | `auto`・`turns` で、見出し（または説明）にその文字列を含むリストが出たら、その文字列を含む項目を選ぶ。インターミッション（`choose コマンド 次のステージへ`）やシナリオ独自の選択肢の答えを決める。規則の無いリストは先頭の有効な項目を選び、同じリストが繰り返し出る場合は順に別の項目を選び、すべて選んでも抜けられなければキャンセルする |
| `choose1 見出し 項目` | `choose` と同じだが一度だけ使う（先に追加したものから順に使う）。`choose` より優先する |
| `chooseseq 見出し 項目1\|項目2\|...` | 見出しの合うリストが出るたびに、項目を順に選ぶ（最後まで選んだら先頭に戻る。今の位置の項目が無ければ次にある項目）。インターミッションで毎回同じ手順を踏むのに使う |
| 見出し `クリック` / `ホットポイント` | `choose`・`choose1`・`chooseseq` の見出しに `クリック` を指定すると、スクリプトが描いたクリック領域（`MousePoint.eve` の `MP_ClickPoint`）を ID で選ぶ。`ホットポイント` を指定すると、`HotPoint` を名前で選ぶ（`choose` の項目は、見出しによらず、ホットポイントの名前か説明に含まれるものにも合う）。`choose1`・`chooseseq` は名前と一致するもので、項目の末尾が `*` なら前方一致 |
| `points` | 今のクリック領域の ID を記録する（規則を書くための調査用。止まって押した時にも記録する） |
| `policy smart\|advance\|hold` | `phase` の方針。`smart`（既定）は SRC の CPU と同じ見積もり（`COM.SelectWeapon`）で目標と武器を選び、移動先は移動後の攻撃の見込みと敵から受ける危険で決める。HP が 35% を下回ったら、倒しきれる見込みが高い敵がいなければ下がる。行動の前に、自分に使うスペシャルパワーのうち回復・防御（HP が減ったとき）と命中を上げるもの（ボスやネームドの敵への命中率が 70% 未満のとき）を使う。`advance` は攻撃できなければ最も近い敵へ近づく、`hold` は移動せず届く敵だけ攻撃する |
| `autodefense on\|off` | 敵の攻撃への反撃・防御・回避を SRC に任せる（設定「自動反撃」。CPU と同じ判断で選ぶ） |
| `set 変数 値` | SRC の変数に値を設定する（検証の前提を整える。例: `set 設定[難易度] ベリーイージー`） |
| `print 式` | SRC の式の値を記録する（例: `print ダンジョン階数`） |
| `stall 秒` | 待ちの状態でないまま進まない（`KeyState` でクリックを待つ演出など）ときにボタンを押すまでの時間（既定 5 秒）。押したボタンは、`KeyState` に読まれた時点（最長 300ms）で離す。`--skip-wait` のときは、直近に `KeyState` が読まれている（スクリプトが入力を待っている）ときだけ押し、読まれていなければ処理中とみなして 5 秒まで押さない |
| `snapshot 名前` | 画面を保存する（`TestResults/e2e/ケース名/`、CI の成果物 `screenshots`） |
| `expect finished\|message 文字列\|log 文字列\|error none` | シナリオの終了・表示されたメッセージ・操作の記録・エラーの有無を確かめる。`log` はスクリプトの行の記録（`SCRIPT` の行）を見ない |
| `expect eval 式 演算子 式` | SRC の式を評価して比べる（例: `expect eval Damage(標的パイロット) > 0`）。両辺が数値なら数値として、どちらかが数値でなければ文字列として比べる（文字列は `=`・`<>` だけ。例: `expect eval Unit(テストパイロット) = テスト機`） |

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
| `battle-advance-adjacent` | `policy advance` で、攻撃できない味方が敵の隣にいて、近づけるマスが無いときに、移動せず待機してターンを進める |
| `sound` | `StartBGM`（MIDI）と `PlaySound`（WAV）。音声ファイルはこのケースのために作ったもの |
| `intermission-save` | `Continue` でインターミッションに入り、データセーブしてから次のステージへ進んで終わるまで。SRC#Web ではブラウザに残ったセーブデータから再開できることも確かめる |
| `save-compat` | クイックセーブ・データセーブ・リスタートの 3 種類のセーブを作りながら最後まで進める。`SaveCompatibilityTests` が、版ごとのセーブ（`E2E/saves/v<版>/`）のすべてから再開できることを確かめる（[テストと CI の設計](../testing.md)） |
| `vrt-screens` | 画面の VRT のためのシナリオ。顔の絵つきのメッセージ・ユニットのコマンドとステータス・特殊能力一覧とその解説・武装一覧で snapshot を撮る（`GameScreenVrtTests`） |
| `intermission-commands` | インターミッションのコマンド（ユニットステータス・パイロットステータス・ユーザー定義のコマンド・機体改造）を開き、毎回インターミッションへ戻れるまで。ステータス表示は `SRC.Sharp/Content/basic-data/Lib` を AppPath に置いて使う（E2E のコーパスはすべて同じ Lib を重ねる）。SRC#Web では仮想パッドと同じキー操作で戻る |
| `cmd-unexec-*` | ほかのケースでは実行しないイベントコマンド 16 種（`Read`・`Write`・`Exec`・`SaveData`・`Suspend`・`CallIntermissionCommand`・`ShowImage`・`ClearImage`・`ChangeLayer`・`ClearLayer`・`PaintSysString`・`SetWindowColor`・`SetWindowFrameWidth`・`SetStatusStringColor`・`Debug`・`ElseIf`）。結果を変数・ファイル・`Info` で確かめる |
| `cmd-branch-*` | 引数の形で処理が分かれるイベントコマンド（`RankUp`・`ChangeMode`・`RemoveUnit`・`RemovePilot`・`Destroy`・`RemoveItem`・`UseAbility`・`MapAbility`・`SpecialPower`・`If`・`CopyArray`・`SetSkill`・`Print`・`FillStyle`・`PaintString`）の引数の形ごとの分岐 |
| `cmd-unit-condition`・`cmd-pilot-upgrade`・`cmd-party-leave`・`cmd-map-attack` | ユニット・パイロット・陣営・マップを操作するイベントコマンド（`SetStatus`・`ClearStatus`・`Finish`・`ChangeUnitBitmap`・`SetBullet`・`SetStock`・`BossRank`・`Enable`・`Upgrade`・`ClearSkill`・`RecoverPlana`・`Release`・`RenameFile`・`Leave`・`Join`・`ChangeParty`・`ChangeTerrain`・`ChangeArea`・`MapAttack`）を引数の形ごとに実行し、結果を `Condition`・`Action`・`Info`・`Status`・`Area` などで確かめる |
| `unit-cmd-*` | ユニット・マップのコマンドをマスのクリックとメニューで実行する（アビリティ・修理・補給・スペシャルパワー・変形・合体・分離・母艦への収納と発進・会話・チャージ・援護攻撃・テレポート・ジャンプ・召喚・命令・マップのメニュー・クイックセーブ／ロード・ユニットステータスでの換装など） |

## VRT

- `SRCRender` は画面全体（マップ・メッセージ・選択肢・ステータス）を SkiaSharp だけで描くため、UI フレームワークや OS の描画系に依存せず、同じ入力から同じ画像が得られる
- 基準画像は `SRCRenderTests/Baselines/*.png`。`VisualAssert.MatchesBaseline` で比較する
  - チャンネルごとに ±24、不一致ピクセル 0.2% までを許容（アンチエイリアスの揺れの吸収）
  - 差分があると `TestResults/vrt/` に `*.actual.png` と `*.diff.png`（差分をマゼンタで表示）を書き出す。CI では成果物 `screenshots` として保存する
  - 基準画像の更新は `SRC_VRT_UPDATE=1 dotnet test SRC.Sharp/SRCRenderTests` で行い、差分をレビューしてコミットする
  - コーパスの `snapshot` で撮った画面も VRT にできる。`GameScreenVrtTests` は `vrt-screens` の snapshot（顔の絵つきのメッセージ・ステータスの欄・特殊能力一覧とその解説・武装一覧）を `Baselines/vrt-screens-<名前>.png` と比べる。`PlayScript.SnapshotTaken` で撮った画面を受け取る（#1358）
- **フォントを固定する**: 基準画像は Ubuntu 24.04 + `fonts-noto-cjk`（Noto Sans CJK JP / Noto Sans Mono CJK JP）で作成している。CI の Linux ジョブも同じフォントを入れる。Noto CJK が無い環境（Windows など）では比較せず Inconclusive とする
- **撮影前に描画を待つ**: UI からの入力はゲームスレッドで処理して描画するため、`ScreenScenarioRunner.SnapshotPng` はそれまでに渡した処理が終わるのを待ってから撮る
- 対象: メッセージ・選択肢・`Paint` 系コマンド・素材なしの表示・リスト（多段・出撃選択）・設定変更・メニュー・メッセージボックス・ホットポイント・ドラッグ後のマップ
- 未対応: 戦闘画面・移動アニメーションの VRT

## テストのプロジェクト

| プロジェクト | 内容 | 実行環境 |
|---|---|---|
| `SRCCoreTests` | ユニットテスト。`E2E/HeadlessScenarioTests` は GUI 呼び出しの記録を、ゲームスレッドあり／なしで比較 | Windows / Linux |
| `SRCRenderTests` | `ScreenGUI` でシナリオをプレイし、画面の状態と VRT を検証。リスト・設定変更・メニュー・ダイアログ・マップ操作・文字入力（`IScreenDialogHost`）・`KeyState` | Linux（CI の Windows ではフォントのテストだけ。VRT は Linux のみ） |
| `SRCSharpAvaloniaTests` | SRC#Avalonia のウィンドウを `Avalonia.Headless` で動かし、マウス入力でシナリオを進める。文字入力ダイアログ。起動の引数と `--smoke` の終了コード（0: 始まった・2: 始まらない・3: ファイルが無い）、効果音の再生の呼び出し（`AppTests`）。描画結果を `TestResults/avalonia/` に保存（CI の成果物 `screenshots-avalonia`） | Linux（ローカルでは Windows でも動く） |
| `SRCSoundTests` | 音声の復号と、OpenAL Soft の null 出力（`ALSOFT_DRIVERS=null`）での再生 | Linux（ローカルでは Windows でも動く。出力を開けない環境では再生のテストは Inconclusive） |
| `SRCAuthoringTests` | 作成支援ツール（`srcsharp` の LSP・DAP・lint・バランス集計）。`srcsharp` の各コマンド（`lint`・`play`・`balance`・`help`・`--version`・不明なコマンド）の終了コードと出力は、`Program.Main` をテストのプロセスの中で呼んで確かめる（`CliTests`） | Linux（ローカルでは Windows でも動く） |
| `SRCSharpWebTests` | SRC#Web のうちブラウザに依存しない処理（セーブの同期・zip の展開・カタログの読み取り）。SRCSharpWeb のファイルを取り込んでテストする | Linux（パスの区切りを `/` として扱うため） |
| `SRCSharpWeb.E2E` | Playwright。発行した SRC#Web を Chromium でプレイする（[SRC#Web](./web.md)）。前の版がブラウザに保存したセーブからの再開（`fixtures/`） | ブラウザ（CI は Linux） |
| `SRCSharpWeb.E2E/tests/deployed.spec.js` | Playwright。配信したデータ閲覧画面（`DEPLOYED_URL`） | ブラウザ（配信した後） |

`SRCRenderTests` は、待ち時間の長いシナリオのテストが多いため、メソッド単位で並べて動かす（`AssemblyInfo.cs` の `Parallelize`、ワーカーは CPU の数）。プロセス全体の UI カルチャを変えるテスト（`UiLanguageTests`）は `[DoNotParallelize]` にしている。テストを足すときは、静的な状態やプロセス全体の設定を変えないようにし、変える場合は `[DoNotParallelize]` を付ける（[MSTest の並列実行](https://learn.microsoft.com/dotnet/core/testing/unit-testing-mstest-writing-tests-controlling-execution#parallelization-attributes)）。

CI（`.github/workflows/CI.yml`）の構成と、各テストの責務・マージの条件は [テストと CI の設計](../testing.md) にまとめている。

## フロントエンドごとの UI テスト

シナリオの挙動と見た目は `SRCRender` 側で網羅し、フロントエンドのテストは「画面が表示され、入力が `ScreenGUI` に届くか」に絞る。

| 対象 | 方針 | 根拠 |
|---|---|---|
| SRC#Avalonia | `Avalonia.Headless` で起動・入力・描画・文字入力ダイアログを確認する | [Headless Testing with XUnit](https://docs.avaloniaui.net/docs/testing/headless-xunit)、[Headless Testing Platform](https://docs.avaloniaui.net/docs/concepts/headless/) |
| SRC#Form | UI テストは無い。作る場合は FlaUI（UIA3）で、対象は起動・終了程度にする | WinAppDriver は大きな更新がなく保守モードとされる（[TestGuild](https://testguild.com/automation-tools-desktop/)、[FlaUI.WebDriver](https://github.com/FlaUI/FlaUI.WebDriver)） |
| SRCDataViewer | 配信の前は `viewer.spec.js`、配信した後は `deployed.spec.js`（どちらも Playwright。Cypress から移した） | — |
