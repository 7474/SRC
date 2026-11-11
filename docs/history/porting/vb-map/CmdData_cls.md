# CmdData.cls

VB 版の `SRC/SRC_20121125/CmdData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Property Get Name | 36 | 6 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Name | 一致 | VB は NullCmd なら遅れて Parse する。C# は読み込み時にすべて解析する |
| Property Let Name | 43 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Name | SRC#で不要 | VB は解析し直すために NullCmd を入れる用途だけ。C# は CmdData を作り直す |
| Function Parse | 48 | 831 | `SRCCore/CmdDatas/CmdParser.cs` CmdParser.Parse<br>`SRCCore/CmdDatas/CmdData.cs` CmdData.ParseArgs<br>`SRCCore/CmdDatas/Commands/Control/AIfCmd.cs` AIfCmd.PrepareArgs | 修正済み | 分割を ListSplitStrict に（閉じていない行は ArgNum=-1）。代入式の判定を ArgNum で。解析のエラーは書式エラーの文言を行付きで出し、構文チェックの後に終了する。If の項数・Goto のラベル・Then の無い行を VB に合わせた。PaintString の引用符の無い 1 引数を項として評価する。PaintString の座標指定の確定の判定は ExecPaintStringCmd の行を見る |
| Sub DebugMsg | 880 | 11 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Exec | SRC#で不要 | VB は Debug.Print だけで、呼び出しもコメントアウトされている |
| Function Exec | 893 | 418 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Exec | 修正済み | End・Suspend を単体で実行したら「…というコマンドは存在しません」にした。GUINotImplementedException を警告にして進むのとデバッガのフックは SRC# の利便性 |
| Function GetArg | 1314 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArg | 違いあり | 範囲外の参照で VB は「イベントデータが不正です」、C# は空文字列（#1511） |
| Function GetArgAsString | 1319 | 10 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgAsString | 一致 | 範囲外は GetArg と同じ（#1511） |
| Function GetArgAsLong | 1331 | 10 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgAsLong | 一致 | 範囲外は GetArg と同じ（#1511） |
| Function GetArgAsDouble | 1343 | 10 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgAsDouble | 一致 | 範囲外は GetArg と同じ（#1511） |
| Function GetArgAsUnit | 1355 | 21 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgAsUnit | 一致 |  |
| Function GetArgAsPilot | 1378 | 10 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgAsPilot | 一致 |  |
| Function GetArgsType | 1391 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgRaw | 一致 |  |
| Sub SetArgsType | 1396 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.GetArgRaw | 一致 |  |
| Function ExecArcCmd | 1401 | 114 | `SRCCore/CmdDatas/Commands/Screan/ArcCmd.cs` ArcCmd | 修正済み | 描く向きと扇形を VB の Circle に合わせた（開始から終了へ反時計回り、0 度をまたぐ・一周する・負の角度で中心へ線を引き扇形を塗る）。色のエラーの文言を「色指定が不正です」に。色の「#RGB」は受け付ける（SRC# の利便性）。角度の `% 360` は残した（VB は ±2π を超えると実行時エラー） |
| Function ExecArrayCmd | 1516 | 91 | `SRCCore/CmdDatas/Commands/Variable/ArrayCmd.cs` ArrayCmd | 修正済み | サブルーチンローカル変数を数値型の 0 で作り直す。要素の Trim は TrimString と同じ文字だけを除く |
| Function ExecAskCmd | 1608 | 225 | `SRCCore/CmdDatas/Commands/Input/AskCmd.cs` AskCmd | 修正済み | 配列の書式で選択肢が無いときも SelectedAlternative を "0" に |
| Function ExecAttackCmd | 1834 | 156 | `SRCCore/CmdDatas/Commands/Unit/AttackCmd.cs` AttackCmd | 修正済み | 「自動」で反撃の武器が無いと例外になっていた（VB と同じく選び直す）。反撃の武器は攻撃の後に番号で引く |
| Function ExecAutoTalkCmd | 1991 | 223 | `SRCCore/CmdDatas/Commands/Talk/AutoTalkCmd.cs` AutoTalkCmd<br>`SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs` ATalkCmd.ProcessTalk | 修正済み | エラーの文言を AutoTalk の名前で。End で「枠外」を戻さない。座標指定の CenterUnit に話者名を渡さない。引数の無い行は MessageFormVisible で判定。中央表示しない話者の行ではメッセージウィンドウを開かない。Talk と AutoTalk の混在はエラーのまま（VB はメッセージとして表示する書き誤りの見逃し） |
| Sub CenterUnit | 2216 | 97 | `SRCCore/CmdDatas/CmdData.utils.cs` CmdData.CenterUnit | 修正済み | 出撃中の話者を中央表示した後に return が無く、母艦へも移っていた |
| Function ExecBossRankCmd | 2314 | 36 | `SRCCore/CmdDatas/Commands/Unit/BossRankCmd.cs` BossRankCmd | 一致 |  |
| Function ExecBreakCmd | 2351 | 30 | `SRCCore/CmdDatas/Commands/Control/BreakCmd.cs` BreakCmd | 一致 |  |
| Function ExecCallCmd | 2382 | 63 | `SRCCore/CmdDatas/Commands/Control/CallCmd.cs` CallCmd | 修正済み | 階層と引数の数の上限の判定を VB と同じ `>` に（1 つ少なかった） |
| Function ExecReturnCmd | 2446 | 20 | `SRCCore/CmdDatas/Commands/Control/ReturnCmd.cs` ReturnCmd | 修正済み | イベントの最上位の Return（CallDepth = 1 And CallStack(1) = 0）をエラーにする |
| Function ExecCallInterMissionCommandCmd | 2467 | 123 | `SRCCore/CmdDatas/Commands/Other/CallIntermissionCommandCmd.cs` CallIntermissionCommandCmd | 修正済み | ステータス表示の eve が無いとき、エラーを出して次の行へ進む（何もせずに終わっていた）。表示の後に IsScenarioFinished と -1 を返すのは SRC# の構成による |
| Function ExecCancelCmd | 2591 | 4 | `SRCCore/CmdDatas/Commands/Event/CancelCmd.cs` CancelCmd | 一致 |  |
| Function ExecCenterCmd | 2596 | 49 | `SRCCore/CmdDatas/Commands/Screan/CenterCmd.cs` CenterCmd | 一致 |  |
| Function ExecChangeAreaCmd | 2646 | 60 | `SRCCore/CmdDatas/Commands/Unit/ChangeAreaCmd.cs` ChangeAreaCmd | 修正済み | マップの外のユニットで例外になっていた。地形での制限をせずに Area を設定する |
| Function ExecChangeLayerCmd | 2710 | 177 | `SRCCore/CmdDatas/Commands/Map/ChangeLayerCmd.cs` ChangeLayerCmd | 一致 | VB の引数エラーの文言「ChangeTerrainコマンドの…」はコピーの誤りなので再現しない |
| Function ExecChangeMapCmd | 2889 | 55 | `SRCCore/CmdDatas/Commands/Map/ChangeMapCmd.cs` ChangeMapCmd | 一致 |  |
| Function ExecChangeModeCmd | 2945 | 113 | `SRCCore/CmdDatas/Commands/Unit/ChangeModeCmd.cs` ChangeModeCmd | 修正済み | グループ ID の対象は「名前:2」「名前:3」…と生きているパイロットが続く間だけ（VB の Do While） |
| Function ExecChangePartyCmd | 3059 | 53 | `SRCCore/CmdDatas/Commands/Unit/ChangePartyCmd.cs` ChangePartyCmd | 一致 |  |
| Function ExecChangeTerrainCmd | 3113 | 155 | `SRCCore/CmdDatas/Commands/Map/ChangeTerrainCmd.cs` ChangeTerrainCmd | 修正済み | 画像が見つからないときのエラーに「.bmp」を足した。描き直しでレイヤーも描く（VB でレイヤーが見た目から消えるのは不具合なので再現しない） |
| Function ExecChangeUnitBitmapCmd | 3269 | 46 | `SRCCore/CmdDatas/Commands/Unit/ChangeUnitBitmapCmd.cs` ChangeUnitBitmapCmd | 一致 | .bmp 以外の画像も受け付ける（SRC# の利便性） |
| Function ExecChargeCmd | 3316 | 17 | `SRCCore/CmdDatas/Commands/Unit/ChargeCmd.cs` ChargeCmd | 一致 |  |
| Function ExecCircleCmd | 3334 | 99 | `SRCCore/CmdDatas/Commands/Screan/CircleCmd.cs` CircleCmd | 一致 | 色の「#RGB」も受け付ける（SRC# の利便性） |
| Function ExecClearEventCmd | 3434 | 20 | `SRCCore/CmdDatas/Commands/Event/ClearEventCmd.cs` ClearEventCmd | 一致 | 行番号は 0 から数え、見つからないと -1。Event.CurrentLabel の初期値 0 は Event.bas の突き合わせで見る |
| Function ExecClearImageCmd | 3456 | 5 | `SRCCore/CmdDatas/Commands/Screan/ClearImageCmd.cs` ClearImageCmd | 一致 |  |
| Function ExecClearLayerCmd | 3469 | 195 | `SRCCore/CmdDatas/Commands/Map/ClearLayerCmd.cs` ClearLayerCmd | 修正済み | オプションが空文字列のとき、通常と同じに消す（エラーにしていた） |
| Function ExecClearObjCmd | 3666 | 66 | `SRCCore/CmdDatas/Commands/Screan/ClearObjCmd.cs` ClearObjCmd | 修正済み | 同じ名前のホットポイントは最初の 1 つだけを消す |
| Function ExecClearPictureCmd | 3733 | 15 | `SRCCore/CmdDatas/Commands/Screan/ClearPictureCmd.cs` ClearPictureCmd | 一致 |  |
| Function ExecClearSkillCmd | 3749 | 84 | `SRCCore/CmdDatas/Commands/Pilot/ClearSkillCmd.cs` ClearSkillCmd | 一致 |  |
| Function ExecClearSpecialPowerCmd | 3834 | 24 | `SRCCore/CmdDatas/Commands/Unit/ClearSpecialPowerCmd.cs` ClearSpecialPowerCmd | 一致 |  |
| Function ExecClearStatusCmd | 3859 | 28 | `SRCCore/CmdDatas/Commands/Unit/ClearStatusCmd.cs` ClearStatusCmd | 一致 |  |
| Function ExecCloseCmd | 3888 | 10 | `SRCCore/CmdDatas/Commands/File/CloseCmd.cs` CloseCmd | 一致 |  |
| Function ExecClsCmd | 3899 | 44 | `SRCCore/CmdDatas/Commands/Screan/ClsCmd.cs` ClsCmd | 一致 | 色の「#RGB」・色の名前も受け付ける（SRC# の利便性） |
| Function ExecColorCmd | 3944 | 27 | `SRCCore/CmdDatas/Commands/Screan/ColorCmd.cs` ColorCmd | 一致 | 色の指定は Cls と同じ |
| Function ExecColorFilterCmd | 3972 | 77 | `SRCCore/CmdDatas/Commands/Map/ColorFilterCmd.cs` ColorFilterCmd | 修正済み | SetupBackground の前の MapX・MapY を覚えて Center で戻す（マップの表示が中央に動いていた） |
| Function ExecCombineCmd | 4050 | 58 | `SRCCore/CmdDatas/Commands/Unit/CombineCmd.cs` CombineCmd | 一致 |  |
| Function ExecConfirmCmd | 4109 | 22 | `SRCCore/CmdDatas/Commands/Input/ConfirmCmd.cs` ConfirmCmd | 一致 |  |
| Function ExecContinueCmd | 4132 | 162 | `SRCCore/CmdDatas/Commands/Stage/ContinueCmd.cs` ContinueCmd | 一致 |  |
| Function ExecCopyArrayCmd | 4295 | 113 | `SRCCore/CmdDatas/Commands/Variable/CopyArrayCmd.cs` CopyArrayCmd | 一致 | コピー先が「$」で始まると name1 が上書きされる VB の書き誤り（`name1 = Mid$(name2, 2)`）も写している。直すかは要判断 |
| Function ExecCopyFileCmd | 4409 | 46 | `SRCCore/CmdDatas/Commands/File/CopyFileCmd.cs` CopyFileCmd | 一致 |  |
| Function ExecCreateCmd | 4456 | 135 | `SRCCore/CmdDatas/Commands/Unit/CreateCmd.cs` CreateCmd | 修正済み | Center・RefreshScreen を、VB と同じく FullRecover・StandBy の前にした |
| Function ExecCreateFolderCmd | 4592 | 29 | `SRCCore/CmdDatas/Commands/File/CreateFolderCmd.cs` CreateFolderCmd | 一致 |  |
| Function ExecDebugCmd | 4622 | 13 | `SRCCore/CmdDatas/Commands/Other/DebugCmd.cs` DebugCmd | SRC#で不要 | VB の Debug.Print はコンパイルした実行ファイルでは何もしない。SRC# はログに出す（利便性） |
| Function ExecDestroyCmd | 4636 | 73 | `SRCCore/CmdDatas/Commands/Unit/DestroyCmd.cs` DestroyCmd | 一致 |  |
| Function ExecDisableCmd | 4710 | 72 | `SRCCore/CmdDatas/Commands/Unit/DisableCmd.cs` DisableCmd | 一致 |  |
| Function ExecDoCmd | 4783 | 46 | `SRCCore/CmdDatas/Commands/Control/DoCmd.cs` DoCmd | 一致 |  |
| Function ExecLoopCmd | 4830 | 46 | `SRCCore/CmdDatas/Commands/Control/LoopCmd.cs` LoopCmd | 一致 |  |
| Function ExecDrawOptionCmd | 4877 | 10 | `SRCCore/CmdDatas/Commands/Screan/DrawOptionCmd.cs` DrawOptionCmd | 一致 |  |
| Function ExecDrawWidthCmd | 4888 | 10 | `SRCCore/CmdDatas/Commands/Screan/DrawWidthCmd.cs` DrawWidthCmd | 一致 |  |
| Function ExecEnableCmd | 4899 | 49 | `SRCCore/CmdDatas/Commands/Unit/EnableCmd.cs` EnableCmd | 一致 |  |
| Function ExecEquipCmd | 4949 | 131 | `SRCCore/CmdDatas/Commands/Item/EquipCmd.cs` EquipCmd | 修正済み | ユニット画像が変わったときだけ再描画し、指揮・階級・広域サポートのレベルが変わったときだけ支援効果を更新する |
| Function ExecEscapeCmd | 5081 | 109 | `SRCCore/CmdDatas/Commands/Unit/EscapeCmd.cs` EscapeCmd | 一致 |  |
| Function ExecExecCmd | 5191 | 169 | `SRCCore/CmdDatas/Commands/Stage/ExecCmd.cs` ExecCmd | 一致 |  |
| Function ExecExitCmd | 5361 | 3 | `SRCCore/CmdDatas/Commands/Event/ExitCmd.cs` ExitCmd | 一致 |  |
| Function ExecExchangeItemCmd | 5365 | 20 | `SRCCore/CmdDatas/Commands/Unit/ExchangeItemCmd.cs` ExchangeItemCmd | 一致 |  |
| Function ExecExplodeCmd | 5386 | 22 | `SRCCore/CmdDatas/Commands/Screan/ExplodeCmd.cs` ExplodeCmd | 一致 |  |
| Function ExecExpUpCmd | 5409 | 61 | `SRCCore/CmdDatas/Commands/Pilot/ExpUpCmd.cs` ExpUpCmd | 一致 |  |
| Function ExecFadeInCmd | 5471 | 75 | `SRCCore/CmdDatas/Commands/Screan/FadeInCmd.cs` FadeInCmd<br>`SRCRender/ScreenGUI.map.cs` ScreenGUI.TransionScrean | 修正済み | 右ボタンで省略したときはマップを描き直さず画面を表示し直すだけ。右ボタンを確かめる間隔（VB は 4 コマごと）と 0 段階のとき（VB は 0 での割り算）は違う |
| Function ExecFadeOutCmd | 5547 | 53 | `SRCCore/CmdDatas/Commands/Screan/FadeOutCmd.cs` FadeOutCmd | 一致 | 右ボタンを確かめる間隔は FadeIn と同じ違い |
| Function ExecFillColorCmd | 5601 | 27 | `SRCCore/CmdDatas/Commands/Screan/FillColorCmd.cs` FillColorCmd | 一致 | 色の「#RGB」・色の名前も受け付ける（SRC# の利便性） |
| Function ExecFillStyleCmd | 5629 | 30 | `SRCCore/CmdDatas/Commands/Screan/FillStyleCmd.cs` FillStyleCmd | 一致 |  |
| Function ExecFinishCmd | 5660 | 31 | `SRCCore/CmdDatas/Commands/Unit/FinishCmd.cs` FinishCmd | 一致 |  |
| Function ExecFixCmd | 5692 | 30 | `SRCCore/CmdDatas/Commands/Unit/FixCmd.cs` FixCmd | 一致 |  |
| Function ExecFontCmd | 5723 | 85 | `SRCCore/CmdDatas/Commands/Screan/FontCmd.cs` FontCmd | 修正済み | 引数なしで既定の書式に戻す。大きさを CInt と同じく偶数丸めで整数にし、数値でなければ「イベントデータが不正です」 |
| Function ExecForCmd | 5809 | 57 | `SRCCore/CmdDatas/Commands/Control/ForCmd.cs` ForCmd | 一致 | 入れ子が MaxCallDepth を超えたとき VB は範囲外に書き込み（範囲チェックなし）、C# は例外 |
| Function ExecForEachCmd | 5867 | 491 | `SRCCore/CmdDatas/Commands/Control/ForEachCmd.cs` ForEachCmd | 修正済み | パイロット一覧・ユニット一覧の並び順を VB の選択ソートに合わせた（値が同じ要素の順）。名称の比較を vbTextCompare と同じに。リストに対する ForEach を ListLength・ListIndex で分ける。ListLength が -1 だと VB は ReDim ForEachSet(-1) の実行時エラーになるので、「イベントデータが不正です」でイベントを止める（#1534 で直した） |
| Function ExecNextCmd | 6359 | 71 | `SRCCore/CmdDatas/Commands/Control/NextCmd.cs` NextCmd | 一致 |  |
| Function ExecForgetCmd | 6431 | 24 | `SRCCore/CmdDatas/Commands/Other/ForgetCmd.cs` ForgetCmd | 一致 |  |
| Function ExecFreeMemoryCmd | 6456 | 6 | `SRCCore/CmdDatas/Commands/Other/FreeMemoryCmd.cs` FreeMemoryCmd | 一致 |  |
| Function ExecGameClearCmd | 6463 | 8 | `SRCCore/CmdDatas/Commands/Stage/GameClearCmd.cs` GameClearCmd | 一致 |  |
| Function ExecGameOverCmd | 6472 | 10 | `SRCCore/CmdDatas/Commands/Stage/GameOverCmd.cs` GameOverCmd | 一致 |  |
| Function ExecGetOffCmd | 6483 | 35 | `SRCCore/CmdDatas/Commands/Pilot/GetOffCmd.cs` GetOffCmd | 一致 |  |
| Function ExecGlobalCmd | 6519 | 20 | `SRCCore/CmdDatas/Commands/Variable/GlobalCmd.cs` GlobalCmd | 一致 |  |
| Function ExecGotoCmd | 6540 | 26 | `SRCCore/CmdDatas/Commands/Control/GotoCmd.cs` GotoCmd | 一致 |  |
| Function ExecHideCmd | 6567 | 5 | `SRCCore/CmdDatas/Commands/Screan/HideCmd.cs` HideCmd | 一致 |  |
| Function ExecHotPointCmd | 6573 | 37 | `SRCCore/CmdDatas/Commands/Screan/HotPointCmd.cs` HotPointCmd | 一致 |  |
| Function ExecIfCmd | 6611 | 201 | `SRCCore/CmdDatas/Commands/Control/IfCmd.cs` IfCmd | 修正済み | `If … Goto` でラベルが見つからないとき、式として評価した値でも探す（VB の FindLabel(GetArgAsString(5))） |
| Function ExecElseCmd | 6813 | 25 | `SRCCore/CmdDatas/Commands/Control/AIfCmd.cs` AIfCmd.ToEnd | 一致 | Else・ElseIf のどちらからも呼ぶ |
| Function ExecIncrCmd | 6839 | 17 | `SRCCore/CmdDatas/Commands/Variable/IncrCmd.cs` IncrCmd | 一致 |  |
| Function ExecIncreaseMoraleCmd | 6857 | 25 | `SRCCore/CmdDatas/Commands/Pilot/IncreaseMoraleCmd.cs` IncreaseMoraleCmd | 一致 |  |
| Function ExecInputCmd | 6883 | 18 | `SRCCore/CmdDatas/Commands/Input/InputCmd.cs` InputCmd | 一致 |  |
| Function ExecIntermissionCommandCmd | 6904 | 31 | `SRCCore/CmdDatas/Commands/Other/IntermissionCommandCmd.cs` IntermissionCommandCmd | 一致 |  |
| Function ExecItemCmd | 6936 | 18 | `SRCCore/CmdDatas/Commands/Item/ItemCmd.cs` ItemCmd | 一致 |  |
| Function ExecJoinCmd | 6955 | 66 | `SRCCore/CmdDatas/Commands/Unit/JoinCmd.cs` JoinCmd | 修正済み | ユニット名で探したユニットのいまの形態の名前が違えば何もしない（VB の Set u = Nothing）。ID 指定・見つからないときの VB の実行時エラー（Set の抜け・Nothing の参照）は再現しない |
| Function ExecKeepBGMCmd | 7022 | 10 | `SRCCore/CmdDatas/Commands/Sound/KeepBGMCmd.cs` KeepBGMCmd | 一致 |  |
| Function ExecLandCmd | 7033 | 28 | `SRCCore/CmdDatas/Commands/Unit/LandCmd.cs` LandCmd | 一致 |  |
| Function ExecLaunchCmd | 7062 | 89 | `SRCCore/CmdDatas/Commands/Unit/LaunchCmd.cs` LaunchCmd | 一致 |  |
| Function ExecLeaveCmd | 7152 | 81 | `SRCCore/CmdDatas/Commands/Unit/LeaveCmd.cs` LeaveCmd | 修正済み | ユニット名で探したユニットのいまの形態の名前が違えば u を null に（VB と同じく「イベントデータが不正です」） |
| Function ExecLevelUpCmd | 7234 | 65 | `SRCCore/CmdDatas/Commands/Pilot/LevelUpCmd.cs` LevelUpCmd | 一致 |  |
| Function ExecLineCmd | 7300 | 119 | `SRCCore/CmdDatas/Commands/Screan/LineCmd.cs` LineCmd | 一致 | 色の「#RGB」も受け付ける（SRC# の利便性） |
| Function ExecLineReadCmd | 7420 | 13 | `SRCCore/CmdDatas/Commands/File/LineReadCmd.cs` LineReadCmd | 一致 | VB の Line Input # は CR・CRLF だけで区切るが、C# の ReadLine は LF だけの行でも区切る（UTF-8 のファイルを扱う SRC# の利便性として残す） |
| Function ExecLoadCmd | 7434 | 114 | `SRCCore/CmdDatas/Commands/Other/LoadCmd.cs` LoadCmd | 修正済み | タイトルごとに IncludeData と include.eve を交互に読み、include.eve は EventFileNames を見ずに読む。読み直すローカルデータを VB と同じ範囲にした（effect・animation・ext_animation と AddDefaultAnimation をやめた）。新しい行だけのラベルを登録して構文解析する（Event.RegisterAndParseAddedEventData。前からある行の EventCmd は作り直さず、制御構造と全角ラベルはチェックしない）。「_」の行の結合は LoadEventData と同じく SrcEveReader がファイルごとに行う（#1518） |
| Function ExecLocalCmd | 7549 | 96 | `SRCCore/CmdDatas/Commands/Control/LocalCmd.cs` LocalCmd | 一致 |  |
| Function ExecMakePilotListCmd | 7646 | 260 | `SRCCore/CmdDatas/Commands/Other/MakePilotListCmd.cs` MakePilotListCmd | 修正済み | 並べ替えを VB と同じ選択ソートに（同じ値の並び）。名称は vbTextCompare と同じ比較。追加パイロットはユニットの FeatureData("追加パイロット") の名前で判断。能力値は並べ替えのときの値を表示 |
| Function ExecMakeUnitListCmd | 7907 | 5 | `SRCCore/CmdDatas/Commands/Other/MakeUnitListCmd.cs` MakeUnitListCmd | 一致 | 本体は Event.MakeUnitList（Event.bas の行で見る） |
| Function ExecMapAbilityCmd | 7913 | 84 | `SRCCore/CmdDatas/Commands/Unit/MapAbilityCmd.cs` MapAbilityCmd | 一致 |  |
| Function ExecMapAttackCmd | 7998 | 114 | `SRCCore/CmdDatas/Commands/Unit/MapAttackCmd.cs` MapAttackCmd | 一致 |  |
| Function ExecMoneyCmd | 8113 | 10 | `SRCCore/CmdDatas/Commands/Other/MoneyCmd.cs` MoneyCmd | 一致 |  |
| Function ExecMonotoneCmd | 8124 | 57 | `SRCCore/CmdDatas/Commands/Map/MonotoneCmd.cs` MonotoneCmd | 修正済み | SetupBackground の後に表示位置を戻す（ColorFilter と同じ） |
| Function ExecMoveCmd | 8182 | 78 | `SRCCore/CmdDatas/Commands/Unit/MoveCmd.cs` MoveCmd | 一致 |  |
| Function ExecNightCmd | 8261 | 57 | `SRCCore/CmdDatas/Commands/Map/NightCmd.cs` NightCmd | 修正済み | SetupBackground の後に表示位置を戻す |
| Function ExecNoonCmd | 8319 | 57 | `SRCCore/CmdDatas/Commands/Map/NoonCmd.cs` NoonCmd | 修正済み | SetupBackground の後に表示位置を戻す。引数は「非同期」1 つだけを受け付け、ほかはエラー（VB の Select Case ArgNum） |
| Function ExecOpenCmd | 8377 | 45 | `SRCCore/CmdDatas/Commands/File/OpenCmd.cs` OpenCmd | 一致 | ScenarioPath は OpenSafe がシナリオのフォルダを起点に解決する |
| Function ExecOptionCmd | 8423 | 30 | `SRCCore/CmdDatas/Commands/Other/OptionCmd.cs` OptionCmd | 一致 | 新ＧＵＩの LoadForms は SRC# では不要 |
| Function ExecOrganizeCmd | 8454 | 284 | `SRCCore/CmdDatas/Commands/Unit/OrganizeCmd.cs` OrganizeCmd | 修正済み | 候補が無くても CheckAutoHyperMode を呼ぶ。レベルの並べ替えを VB と同じ選択ソートに |
| Function ExecOvalCmd | 8739 | 102 | `SRCCore/CmdDatas/Commands/Screan/OvalCmd.cs` OvalCmd | 一致 | 色の「#RGB」も受け付ける（SRC# の利便性） |
| Function ExecPaintPictureCmd | 8842 | 157 | `SRCCore/CmdDatas/Commands/Screan/PaintPictureCmd.cs` PaintPictureCmd | 一致 |  |
| Function ExecPaintStringCmd | 9000 | 70 | `SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs` PaintStringCmd | 修正済み | 座標指定が確定しているかを構文解析のときの項（「-」・IsNumeric・「$」「(」で始まる式）で決め、確定していれば数値でない値は「イベントデータが不正です」 |
| Function ExecPaintSysStringCmd | 9071 | 18 | `SRCCore/CmdDatas/Commands/Screan/PaintSysStringCmd.cs` PaintSysStringCmd | 一致 |  |
| Function ExecPilotCmd | 9090 | 44 | `SRCCore/CmdDatas/Commands/Pilot/PilotCmd.cs` PilotCmd | 一致 |  |
| Function ExecPlayMIDICmd | 9135 | 39 | `SRCCore/CmdDatas/Commands/Sound/ABGMCmd.cs` ABGMCmd | 一致 | ファイル名を VB と同じく ListTail(…, 2)・ListLength で読む（#1534 で直した） |
| Function ExecPlaySoundCmd | 9175 | 12 | `SRCCore/CmdDatas/Commands/Sound/PlaySoundCmd.cs` PlaySoundCmd | 一致 |  |
| Function ExecPolygonCmd | 9188 | 109 | `SRCCore/CmdDatas/Commands/Screan/PolygonCmd.cs` PolygonCmd | 一致 |  |
| Function ExecPrintCmd | 9298 | 50 | `SRCCore/CmdDatas/Commands/File/PrintCmd.cs` PrintCmd | 一致 |  |
| Function ExecPSetCmd | 9349 | 94 | `SRCCore/CmdDatas/Commands/Screan/PSetCmd.cs` PSetCmd | 修正済み | 画面の外なら色指定を確かめずに終わる。色の「#RGB」も受け付ける（SRC# の利便性） |
| Function ExecQuestionCmd | 9444 | 53 | `SRCCore/CmdDatas/Commands/Input/QuestionCmd.cs` QuestionCmd | 修正済み | 終わりの判定を式置換の後の行が "end" かに（「End x」は選択肢）。引数の数は選択肢があるときだけ確かめる。End が無いときは VB の判定の誤りを再現せずエラー |
| Function ExecQuickLoadCmd | 9498 | 36 | `SRCCore/CmdDatas/Commands/Other/QuickLoadCmd.cs` QuickLoadCmd | 一致 | 最後のセーブがクイックセーブならクイックセーブを読むように直した（#1530） |
| Function ExecQuitCmd | 9535 | 3 | `SRCCore/CmdDatas/Commands/Stage/QuitCmd.cs` QuitCmd | 一致 |  |
| Function ExecRankUpCmd | 9539 | 118 | `SRCCore/CmdDatas/Commands/Unit/RankUpCmd.cs` RankUpCmd | 一致 | MaxHP・MaxEN が 0 のとき VB は 0 での割り算のエラー、C# は割合を 100 にする |
| Function ExecReadCmd | 9658 | 18 | `SRCCore/CmdDatas/Commands/File/ReadCmd.cs` ReadCmd | 修正済み | ファイルの終わりで読むとエラー（Input # のエラー 62） |
| Function ExecRecoverENCmd | 9677 | 30 | `SRCCore/CmdDatas/Commands/Unit/RecoverENCmd.cs` RecoverENCmd | 一致 |  |
| Function ExecRecoverHPCmd | 9708 | 27 | `SRCCore/CmdDatas/Commands/Unit/RecoverHPCmd.cs` RecoverHPCmd | 一致 |  |
| Function ExecRecoverPlanaCmd | 9736 | 50 | `SRCCore/CmdDatas/Commands/Pilot/RecoverPlanaCmd.cs` RecoverPlanaCmd | 一致 |  |
| Function ExecRecoverSPCmd | 9787 | 31 | `SRCCore/CmdDatas/Commands/Pilot/RecoverSPCmd.cs` RecoverSPCmd | 一致 |  |
| Function ExecRedrawCmd | 9819 | 12 | `SRCCore/CmdDatas/Commands/Screan/RedrawCmd.cs` RedrawCmd | 一致 |  |
| Function ExecRefreshCmd | 9832 | 4 | `SRCCore/CmdDatas/Commands/Screan/RefreshCmd.cs` RefreshCmd | 一致 |  |
| Function ExecReleaseCmd | 9837 | 30 | `SRCCore/CmdDatas/Commands/Unit/ReleaseCmd.cs` ReleaseCmd | 一致 |  |
| Function ExecRemoveFileCmd | 9868 | 25 | `SRCCore/CmdDatas/Commands/File/RemoveFileCmd.cs` RemoveFileCmd | 一致 |  |
| Function ExecRemoveFolderCmd | 9894 | 34 | `SRCCore/CmdDatas/Commands/File/RemoveFolderCmd.cs` RemoveFolderCmd | 修正済み | 「..」のエラーの文言を VB の「ファイル指定に…」に。ファイルのパスを指定したとき VB は DeleteFolder のエラー、C# は何もしない |
| Function ExecRemoveItemCmd | 9929 | 337 | `SRCCore/CmdDatas/Commands/Item/RemoveItemCmd.cs` RemoveItemCmd | 修正済み | ユニット名で指定したときは CurrentForm を使う。描き直しはユニット画像を持つアイテムを外したときだけ（外したユニットを描き直す。VB が SelectedUnitForEvent を描き直す誤りは再現しない）。すべて外すときは Item(1) を外すことを繰り返す |
| Function ExecRemovePilotCmd | 10267 | 96 | `SRCCore/CmdDatas/Commands/Pilot/RemovePilotCmd.cs` RemovePilotCmd | 修正済み | メインでないパイロットをパイロット・サポートの順に探し、見つかったときだけ外して Unit を null にする |
| Function ExecRemoveUnitCmd | 10364 | 106 | `SRCCore/CmdDatas/Commands/Unit/RemoveUnitCmd.cs` RemoveUnitCmd | 一致 |  |
| Function ExecRenameBGMCmd | 10471 | 25 | `SRCCore/CmdDatas/Commands/Sound/RenameBGMCmd.cs` RenameBGMCmd | 一致 |  |
| Function ExecRenameFileCmd | 10497 | 43 | `SRCCore/CmdDatas/Commands/File/RenameFileCmd.cs` RenameFileCmd | 修正済み | エラーのメッセージに ScenarioPath を付けたファイル名を出す |
| Function ExecRenameTermCmd | 10541 | 23 | `SRCCore/CmdDatas/Commands/Other/RenameTermCmd.cs` RenameTermCmd | 一致 |  |
| Function ExecReplacePilotCmd | 10565 | 63 | `SRCCore/CmdDatas/Commands/Pilot/ReplacePilotCmd.cs` ReplacePilotCmd | 修正済み | 乗せ換えの後に全パイロットの支援効果を更新する（VB は Nothing を渡す）。追加サポートを指定したときの VB のエラー 5 は再現しない |
| Function ExecRequireCmd | 10629 | 86 | `SRCCore/CmdDatas/Commands/Other/RequireCmd.cs` RequireCmd | 一致 | 読み込み・ラベルの登録・構文解析は Load と共通（#1518） |
| Function ExecRestoreEventCmd | 10716 | 10 | `SRCCore/CmdDatas/Commands/Event/RestoreEventCmd.cs` RestoreEventCmd | 一致 |  |
| Function ExecRideCmd | 10727 | 138 | `SRCCore/CmdDatas/Commands/Pilot/RideCmd.cs` RideCmd | 一致 |  |
| Function ExecSaveDataCmd | 10866 | 51 | `SRCCore/CmdDatas/Commands/Other/SaveDataCmd.cs` SaveDataCmd | 修正済み | ファイル名が空ならセーブしない（例外になっていた）。相対パスの起点は SRC# ではシナリオのフォルダ |
| Function ExecSelectCmd | 10918 | 10 | `SRCCore/CmdDatas/Commands/Unit/SelectCmd.cs` SelectCmd | 一致 |  |
| Function ExecSelectTargetCmd | 10929 | 12 | `SRCCore/CmdDatas/Commands/Unit/SelectTargetCmd.cs` SelectTargetCmd | 一致 |  |
| Function ExecSepiaCmd | 10942 | 57 | `SRCCore/CmdDatas/Commands/Map/SepiaCmd.cs` SepiaCmd | 修正済み | SetupBackground の後に表示位置を戻す |
| Function ExecSetCmd | 11000 | 37 | `SRCCore/CmdDatas/Commands/Variable/SetCmd.cs` SetCmd | 一致 |  |
| Function ExecSetBulletCmd | 11038 | 43 | `SRCCore/CmdDatas/Commands/Unit/SetBulletCmd.cs` SetBulletCmd | 一致 |  |
| Function ExecSetMessageCmd | 11082 | 67 | `SRCCore/CmdDatas/Commands/Unit/SetMessageCmd.cs` SetMessageCmd | 一致 |  |
| Function ExecSetRelationCmd | 11150 | 61 | `SRCCore/CmdDatas/Commands/Pilot/SetRelationCmd.cs` SetRelationCmd | 一致 |  |
| Function ExecSetSkillCmd | 11212 | 150 | `SRCCore/CmdDatas/Commands/Pilot/SetSkillCmd.cs` SetSkillCmd | 一致 |  |
| Function ExecSetStatusCmd | 11363 | 37 | `SRCCore/CmdDatas/Commands/Unit/SetStatusCmd.cs` SetStatusCmd | 一致 |  |
| Function ExecSetStatusStringColor | 11402 | 68 | `SRCCore/CmdDatas/Commands/Other/SetStatusStringColorCmd.cs` SetStatusStringColorCmd | 一致 | 変数の保存は同じ。VB で色が効くのは新ＧＵＩのときだけで、SRC# は新ＧＵＩに対応しない（#1520 は #1537 で閉じた） |
| Function ExecSetStockCmd | 11472 | 43 | `SRCCore/CmdDatas/Commands/Unit/SetStockCmd.cs` SetStockCmd | 一致 |  |
| Function ExecSetWindowColor | 11516 | 76 | `SRCCore/CmdDatas/Commands/Other/SetWindowColorCmd.cs` SetWindowColorCmd | 一致 | 変数の保存は同じ。VB で色が効くのは新ＧＵＩのときだけで、SRC# は新ＧＵＩに対応しない（#1520 は #1537 で閉じた） |
| Function ExecSetWindowFrameWidth | 11593 | 19 | `SRCCore/CmdDatas/Commands/Other/SetWindowFrameWidthCmd.cs` SetWindowFrameWidthCmd | 一致 | 変数の保存は同じ。VB で枠の幅が効くのは新ＧＵＩのときだけで、SRC# は新ＧＵＩに対応しない（#1520 は #1537 で閉じた）。VB の引数エラーの文言「SetWindowColorコマンドの…」の書き誤りは再現しない |
| Function ExecShowCmd | 11614 | 15 | `SRCCore/CmdDatas/Commands/Screan/ShowCmd.cs` ShowCmd | 一致 |  |
| Function ExecShowImageCmd | 11631 | 32 | `SRCCore/CmdDatas/Commands/Screan/ShowImageCmd.cs` ShowImageCmd | 一致 | 実装済み（ヘルプの「未実装」を直した）。引数 1・3 個のとき VB は範囲外の引数を読む（範囲チェックなしで値が定まらない） |
| Function ExecShowUnitStatusCmd | 11664 | 25 | `SRCCore/CmdDatas/Commands/Unit/ShowUnitStatusCmd.cs` ShowUnitStatusCmd | 一致 |  |
| Function ExecSkipCmd | 11690 | 21 | `SRCCore/CmdDatas/Commands/Control/SkipCmd.cs` SkipCmd | 一致 |  |
| Function ExecSortCmd | 11712 | 239 | `SRCCore/CmdDatas/Commands/Variable/SortCmd.cs` SortCmd | 修正済み | VB と同じ交換ソート（同じ値の順）。文字の比較は vbTextCompare 相当、全角の数字も数値として読む、配列名の前方一致は大文字・小文字を区別する |
| Function ExecSpecialPowerCmd | 11952 | 90 | `SRCCore/CmdDatas/Commands/Unit/SpecialPowerCmd.cs` SpecialPowerCmd | 一致 |  |
| Function ExecSplitCmd | 12043 | 49 | `SRCCore/CmdDatas/Commands/Unit/SplitCmd.cs` SplitCmd | 修正済み | #1521（#1528）。VB の ExecSplitCmd と同じく今の選択だけを書き換え、退避した選択は書き換えない |
| Function ExecStartBGMCmd | 12093 | 39 | `SRCCore/CmdDatas/Commands/Sound/StartBGMCmd.cs` StartBGMCmd<br>`SRCCore/CmdDatas/Commands/Sound/ABGMCmd.cs` ABGMCmd | 一致 | ファイル名を VB と同じく ListTail(…, 2)・ListLength で読む（#1534 で直した） |
| Function ExecStopBGMCmd | 12133 | 9 | `SRCCore/CmdDatas/Commands/Sound/StopBGMCmd.cs` StopBGMCmd | 一致 |  |
| Function ExecStopSummoningCmd | 12143 | 18 | `SRCCore/CmdDatas/Commands/Unit/StopSummoningCmd.cs` StopSummoningCmd | 一致 |  |
| Function ExecSunsetCmd | 12162 | 57 | `SRCCore/CmdDatas/Commands/Map/SunsetCmd.cs` SunsetCmd | 修正済み | SetupBackground の後に表示位置を戻す |
| Function ExecSupplyCmd | 12220 | 19 | `SRCCore/CmdDatas/Commands/Unit/SupplyCmd.cs` SupplyCmd | 一致 |  |
| Function ExecSwapCmd | 12240 | 87 | `SRCCore/CmdDatas/Commands/Variable/SwapCmd.cs` SwapCmd | 一致 |  |
| Function ExecSwitchCmd | 12328 | 57 | `SRCCore/CmdDatas/Commands/Control/SwitchCmd.cs` SwitchCmd | 一致 |  |
| Function ExecCaseCmd | 12386 | 21 | `SRCCore/CmdDatas/Commands/Control/SwitchCmd.cs` CaseCmd | 一致 |  |
| Function ExecTalkCmd | 12408 | 216 | `SRCCore/CmdDatas/Commands/Talk/TalkCmd.cs` TalkCmd<br>`SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs` ATalkCmd.ProcessTalk | 一致 | Talk と AutoTalk の混在をエラーにするのは #1503 の判断のまま |
| Function ExecTelopCmd | 12625 | 25 | `SRCCore/CmdDatas/Commands/Screan/TelopCmd.cs` TelopCmd | 一致 |  |
| Function ExecTransformCmd | 12651 | 43 | `SRCCore/CmdDatas/Commands/Unit/TransformCmd.cs` TransformCmd | 一致 | 退避した選択も更新するのは、VB の RestoreSelections が .CurrentForm で戻すことの代わり |
| Function ExecUnitCmd | 12695 | 30 | `SRCCore/CmdDatas/Commands/Unit/UnitCmd.cs` UnitCmd | 修正済み | 「のユニットデータが不正です」のエラーにユニット名を付ける |
| Function ExecUnsetCmd | 12726 | 5 | `SRCCore/CmdDatas/Commands/Variable/UnSetCmd.cs` UnSetCmd | 一致 |  |
| Function ExecUpgradeCmd | 12732 | 181 | `SRCCore/CmdDatas/Commands/Unit/UpgradeCmd.cs` UpgradeCmd | 修正済み | 格納中のユニットは母艦から降ろし、Land で格納し直す（状態「格納」・座標・地形が設定されていなかった） |
| Function ExecUpvarCmd | 12914 | 4 | `SRCCore/CmdDatas/Commands/Control/UpVarCmd.cs` UpVarCmd | 一致 |  |
| Function ExecUseAbilityCmd | 12919 | 98 | `SRCCore/CmdDatas/Commands/Unit/UseAbilityCmd.cs` UseAbilityCmd | 一致 |  |
| Function ExecWaitCmd | 13018 | 116 | `SRCCore/CmdDatas/Commands/Other/WaitCmd.cs` WaitCmd | 修正済み | Wait Click を右ボタンで抜けたら MouseButton を None に |
| Function ExecWaterCmd | 13135 | 57 | `SRCCore/CmdDatas/Commands/Map/WaterCmd.cs` WaterCmd | 修正済み | SetupBackground の後に表示位置を戻す |
| Function ExecWhiteInCmd | 13193 | 69 | `SRCCore/CmdDatas/Commands/Screan/WhiteInCmd.cs` WhiteInCmd | 修正済み | 最初に右ボタンを見て省略する処理（VB に無い）を除いた |
| Function ExecWhiteOutCmd | 13263 | 53 | `SRCCore/CmdDatas/Commands/Screan/WhiteOutCmd.cs` WhiteOutCmd | 一致 | 右ボタンを確かめる間隔は FadeIn と同じ違い |
| Function ExecWriteCmd | 13317 | 16 | `SRCCore/CmdDatas/Commands/File/WriteCmd.cs` WriteCmd | 一致 |  |
| Function ExecPlayFlashCmd | 13335 | 36 | `SRCCore/CmdDatas/CmdParser.cs` CmdParser.Parse（NotSupportedCmd） | SRC#で不要 | Flash は SRC# では使えない |
| Function ExecClearFlashCmd | 13373 | 5 | （無し。CmdParser で NotSupportedCmd） | SRC#で不要 | Flash は SRC# では使えない（SRCとの互換性に記載） |
