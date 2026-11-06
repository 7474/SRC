# Command.bas

VB 版の `SRC/SRC_20121125/Command.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub ProceedCommand | 156 | 1787 | `SRCCore/Commands/Command.process.cs` Command.ProceedCommand | 修正済み | C# は ProceedUnitSelect・ProceedCommandSelect・ProceedAfterMoveCommandSelect・ProceedTargetSelect・ProceedMapAttack に分かれる（補助は AddFixSupplyCommand・AddUserUnitCommand・FirstCombineFeatureForMenu）。直したもの: マップコマンドの並びを Main.frm の番号の順にし、ステータス表示のステージでも部隊表を出す／合体はメニューに 1 項目だけ出し、名称は能力ごとに見る／パーツ分離は分離の項目を置き換える／ステータス表示で変形・分離・パーツ分離の名称が空なら既定の名前にし、パートナーが作られていない合体で落ちない／修理・補給のメニューは HP・EN・弾数だけを見る（修理不可は見ない。ゾンビを除くのは移動前だけ）／召喚解除は全部の「合体」能力の合体先について、IsDefined と CurrentForm.Status で見る／チャージの表示名は「チャージ」のまま（Term を使わない）／味方以外のユニットコマンドの対象を GetValueAsString で評価する／ハイパーモードの行動不能は変身後の形態で見る（IsAvailable も見るので動作は同じ）／新ＧＵＩでないときのユニットステータスの表示（コマンド選択のとき・移動後に EN が変わったとき）を戻す。残る違い: 召喚解除で召喚ユニットが複数いるときの順序（下の 5） |
| Sub CancelCommand | 1945 | 139 | `SRCCore/Commands/Command.process.cs` Command.CancelCommand | 修正済み | 移動後コマンド選択のキャンセルで、発進でなければ DisplayUnitStatus を呼ぶようにした（VB は新ＧＵＩでないとき。SRC# は新ＧＵＩに対応していない）。新ＧＵＩのときだけの ClearUnitStatus は SRC# では要らない。そのほかの分岐は一致 |
| Sub UnitCommand | 2087 | 252 | `SRCCore/Commands/Command.unitcommand.cs` Command.UnitCommand | 一致 | VB は 命令／換装、ユニットコマンド 1〜10 をメニューの文字列と番号で分ける。C# は ExchangeFormCmdID・UnitCommandCmdID（LabelData の EventDataId ＝ VB の LineNum）で分ける。これは SRC# の都合で、結果は同じ。召喚解除とユーザー定義のコマンドは DismissCommand・UserDefineUnitCommand に切り出してあり、分岐は一致する。DoEvents は SRC# では要らない |
| Sub StartMoveCommand | 2344 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartMoveCommand | 修正済み | Option「大型マップ」では Center しない（判定がコメントアウトされていた）。NewGUIMode の分岐は SRC# では要らない |
| Sub FinishMoveCommand | 2365 | 104 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishMoveCommand | 一致 | 着艦か合体かの確認は ConfirmMoveFinish(u, true)。移動先が母艦で、自身も母艦なら「合体」と聞く |
| Sub StartTeleportCommand | 2474 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartTeleportCommand | 一致 | NewGUIMode の分岐は SRC# では要らない |
| Sub FinishTeleportCommand | 2495 | 126 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishTeleportCommand | 修正済み | 移動先が母艦なら、自身が母艦でも「着艦しますか？」と聞く。移動と共通の ConfirmMoveFinish が、自身の母艦も見ていた |
| Sub StartJumpCommand | 2626 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartJumpCommand | 一致 | NewGUIMode の分岐は SRC# では要らない |
| Sub FinishJumpCommand | 2647 | 122 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishJumpCommand | 修正済み | テレポートと同じ（移動先が母艦なら必ず「着艦」と聞く） |
| Sub StartAttackCommand | 2774 | 246 | `SRCCore/Commands/Command.attack.cs` Command.StartAttackCommand | 一致 | 射程 1 の合体技の判定は Map.AdjacentUnit で取る。順番は違うが、マスクの結果は同じ |
| Sub FinishAttackCommand | 3024 | 866 | `SRCCore/Commands/Command.attack.cs` Command.FinishAttackCommand | 修正済み | (1) 統率の同時援護攻撃で、本体の攻撃を "統率" で行う（コメントアウトされていて、いつも "" で攻撃していた）。(2) 武器の名前の再確認は今の SelectedWeapon 番目の武器と比べ、違えば -1。Copilot が足した「名前で探し直す」処理はやめた。これまでの比較は取っておいた武器の物と比べていたので、いつも同じになっていた。以降の判定・攻撃も今の番号の武器で行う。(3) 先制反撃の判定は 短絡しない Or で評価し、武器が「先」でも Dice(16) を振る（VB の Or は短絡しない）。(4) サポートアタックを止める合体技の判定も、今の番号の武器で見る。反撃で憑依のとき SelectedWeapon を -1 にする VB の書き誤りは、VB と同じにしてある |
| Sub MapAttackCommand | 3894 | 169 | `SRCCore/Commands/Command.attack.cs` Command.MapAttackCommand | 一致 |  |
| Sub StartAbilityCommand | 4069 | 296 | `SRCCore/Commands/Command.unitability.cs` Command.StartAbilityCommand | 一致 | 射程0の即時実行、効果範囲、射程1の合体技のマスク、カーソルの自動移動まで VB と同じ |
| Sub FinishAbilityCommand | 4369 | 184 | `SRCCore/Commands/Command.unitability.cs` Command.FinishAbilityCommand | 修正済み | 破壊イベントを実行前の形態ではなく、VB と同じく実行後の SelectedUnit で判定するようにした（#1498）。VB が新ＧＵＩのときに行う ClearUnitStatus は SRC# では要らない |
| Sub MapAbilityCommand | 4557 | 162 | `SRCCore/Commands/Command.unitability.cs` Command.MapAbilityCommand | 修正済み | 合体技のパートナーは射程に関わらず座標を渡さずに探す。これまでは射程1のとき SelectedTarget の座標を渡していた（#1498）。LockGUI を呼ぶ位置の違いは、ProceedCommand の最後でロックを外すので SRC# の都合 |
| Sub StartSpecialPowerCommand | 4724 | 505 | `SRCCore/Commands/Command.unitsp.cs` Command.StartSpecialPowerCommand | 修正済み | 味方スペシャルパワー実行かどうかを EffectType(1) だけで判定し、一覧から除くものの判定も同じにした。一覧の並びは、読み仮名を vbTextCompare で比べる VB の選択ソートに合わせた（これまでは OrderBy で、カルチャに依存していた）（#1498） |
| Sub FinishSpecialPowerCommand | 5233 | 67 | `SRCCore/Commands/Command.unitsp.cs` Command.FinishSpecialPowerCommand | 一致 |  |
| Sub StartFixCommand | 5305 | 78 | `SRCCore/Commands/Command.unitsupply.cs` Command.StartFixCommand | 一致 | 判定は Unit.CanFix で行う。VB の `For k = 2 To .FeatureData("修理不可")` は文字列を上限にしており、型の不一致（エラー 13）になる VB の不具合。C# は LLength の意図どおりに動くので、そのままにする |
| Sub FinishFixCommand | 5387 | 67 | `SRCCore/Commands/Command.unitsupply.cs` Command.FinishFixCommand | 一致 |  |
| Sub StartSupplyCommand | 5459 | 71 | `SRCCore/Commands/Command.unitsupply.cs` Command.StartSupplyCommand | 一致 | 判定は Unit.CanSupply |
| Sub FinishSupplyCommand | 5534 | 54 | `SRCCore/Commands/Command.unitsupply.cs` Command.FinishSupplyCommand | 一致 |  |
| Sub ChargeCommand | 5593 | 104 | `SRCCore/Commands/Command.unitxxx.cs` Command.ChargeCommand | 修正済み | IsWeaponAvailable("チャージ") で使えるＣ・合の武器でパートナーを探し、見つからないときだけアビリティで探す（IsAbilityAvailable("チャージ") も確かめる）。これまではアビリティが武器のパートナーを上書きしていた（#1498） |
| Sub StartTalkCommand | 5701 | 44 | `SRCCore/Commands/Command.unitxxx.cs` Command.StartTalkCommand | 一致 | VB の不具合（カーソルの自動移動で、会話できないためマスクしたユニットに t を入れる）も同じ |
| Sub FinishTalkCommand | 5749 | 32 | `SRCCore/Commands/Command.unitxxx.cs` Command.FinishTalkCommand | 一致 |  |
| Sub TransformCommand | 5785 | 234 | `SRCCore/Commands/Command.unitform.cs` Command.TransformCommand | 修正済み | ステータス表示の変形先の一覧を、VB と同じく Nickname にした（マップ上では Nickname0 のまま）（#1498）。最後の RedrawScreen は SRC# の都合（下の todo） |
| Sub HyperModeCommand | 6023 | 164 | `SRCCore/Commands/Command.unitform.cs` Command.HyperModeCommand | 修正済み | メッセージの候補に「ハイパーモード」を足した（#1498） |
| Sub CancelTransformationCommand | 6191 | 58 | `SRCCore/Commands/Command.unitform.cs` Command.CancelTransformationCommand | 一致 |  |
| Sub SplitCommand | 6253 | 204 | `SRCCore/Commands/Command.unitform.cs` Command.SplitCommand | 一致 | 最後の RedrawScreen は SRC# の都合 |
| Sub CombineCommand | 6461 | 149 | `SRCCore/Commands/Command.unitform.cs` Command.CombineCommand | 修正済み | 合体の候補（`SRCCore/Extensions/UnitExtension.cs` CombineFeatures で forMenu が偽のとき）で、能力の名称が非表示か（FeatureName）と、合体後のユニットが行動不能かを調べないようにした。メニュー（forMenu が真のとき）の判定は変えていない（#1498） |
| Sub ExchangeFormCommand | 6614 | 121 | `SRCCore/Commands/Command.unitform.cs` Command.ExchangeFormCommand | 修正済み | 最大攻撃力・最大射程に合体技を数えないようにした（前は IsDisplayFor で、使える合体技を数えていた）。特殊能力のコメントは各名称の後に空白を置く |
| Sub StartLaunchCommand | 6740 | 93 | `SRCCore/Commands/Command.launch.cs` Command.StartLaunchCommand | 一致 | 母艦の代わりに発進するユニットを表示する BitmapID の処理は SRC# では不要（画像は描画のときに取る） |
| Sub FinishLaunchCommand | 6837 | 70 | `SRCCore/Commands/Command.launch.cs` Command.FinishLaunchCommand | 修正済み | 母艦の位置を PrevUnitX・PrevUnitY に記録するのが抜けていた（発進の後にキャンセルしても母艦に戻らなかった）。確認の文は、目的地が母艦なら発進するユニットが母艦でも「着艦」にする。母艦を描き直す（PaintUnitBitmap） |
| Sub StartOrderCommand | 6912 | 71 | `SRCCore/Commands/Command.unitxxx.cs` Command.StartOrderCommand | 一致 |  |
| Sub FinishOrderCommand | 6987 | 16 | `SRCCore/Commands/Command.unitxxx.cs` Command.FinishOrderCommand | 一致 |  |
| Sub FeatureListCommand | 7008 | 267 | `SRCCore/Commands/Command.preview.cs` Command.FeatureListCommand | 修正済み | アビリティで付加された能力は、ユニットの FeatureName0 が空なら出さない（前はパイロットの SkillName0 を見ていた）。VB の不具合 2 つは直していない（5 を参照） |
| Sub WeaponListCommand | 7279 | 160 | `SRCCore/Commands/Command.preview.cs` Command.WeaponListCommand | 修正済み | 属性のレベルを全角で表示する（StrConv(alevel, vbWide)） |
| Sub AbilityListCommand | 7443 | 155 | `SRCCore/Commands/Command.preview.cs` Command.AbilityListCommand | 修正済み | AttributeName に is_ability=True を渡す。属性のレベルを全角にする。射程範囲は AreaInReachable・AreaInRange とも「すべて」で選ぶ（前は「〜の敵」） |
| Sub ShowAreaInSpeedCommand | 7602 | 14 | `SRCCore/Commands/Command.preview.cs` Command.ShowAreaInSpeedCommand | 一致 | 新 GUI のときの ClearUnitStatus は SRC# では不要（新 GUI に対応していない） |
| Sub ShowAreaInRangeCommand | 7620 | 38 | `SRCCore/Commands/Command.preview.cs` Command.ShowAreaInRangeCommand | 一致 | 同上 |
| Sub WaitCommand | 7664 | 139 | `SRCCore/Commands/Command.unitcommand.cs` Command.WaitCommand | 修正済み | 最後のステータスウィンドウの更新がコメントアウトされていたのを戻した（出撃中で MainWidth = 15 なら DisplayUnitStatus、それ以外は ClearUnitStatus） |
| Sub MapCommand | 7806 | 51 | `SRCCore/Commands/Command.mapcommend.cs` Command.MapCommand | 一致 | マップコマンドのラベルの持ち方・自動反撃の設定の保存先（SystemConfig）・オートプレイは SRC# の都合 |
| Sub EndTurnCommand | 7861 | 74 | `SRCCore/Commands/Command.mapcommend.cs` Command.EndTurnCommand | 修正済み | 行動終了イベントを、ユニットごとにその時点で判定してから行う（前は最初に一覧を作っていた）。回り終えたら SelectedUnit を Nothing にする |
| Sub UnitListCommand | 7939 | 304 | `SRCCore/Commands/Command.mapcommend.cs` Command.UnitListCommand | 修正済み | 並べ替えを VB と同じ選択ソートにした（安定でないので同じ値の順も合わせる。文字列は vbTextCompare）。Term を変えたときの VB の不具合は直していない（5 を参照） |
| Sub SearchSpecialPowerCommand | 8247 | 216 | `SRCCore/Commands/Command.mapcommend.cs` Command.SearchSpecialPowerCommand | 修正済み | スペシャルパワーを読み仮名の選択ソート（vbTextCompare）で並べる。パイロットの一覧は VB の条件で作る（憑依を見ない。Pilot(1) がメインパイロットでないパイロットを除く）。持っているスペシャルパワーをすべて並べる |
| Sub RestartCommand | 8467 | 17 | `SRCCore/Commands/Command.mapcommend.cs` Command.RestartCommand | 一致 | 保存の形式（.srcq）は SRC# の都合 |
| Sub QuickLoadCommand | 8488 | 21 | `SRCCore/Commands/Command.mapcommend.cs` Command.QuickLoadCommand | 一致 | ストリームで読む |
| Sub QuickSaveCommand | 8513 | 16 | `SRCCore/Commands/Command.mapcommend.cs` Command.QuickSaveCommand | 一致 | ストリームで書く |
| Sub DumpCommand | 8534 | 57 | `SRCCore/Commands/Command.mapcommend.cs` Command.DumpCommand | 一致 | VB にある「シナリオフォルダの外への保存」の確認は行わない。保存先は GUI の SelectSaveStream が決め、SRCCore はファイル名を受け取らないため |
| Sub GlobalMapCommand | 8596 | 126 | `SRCCore/Commands/Command.mapcommend.cs` Command.GlobalMapCommand | 一致 | 縮小マップとユニット数の表示は GUI.DisplayGlobalMap（SRCRender・SRCSharpForm）にあり、VB と同じ |
| Sub SaveSelections | 8725 | 34 | `SRCCore/Commands/Command.state.cs` Command.SaveSelections | 一致 | 全部の項目を Clone して退避する。戻す項目を RestoreSelections で VB に合わせたので、結果は同じ |
| Sub RestoreSelections | 8761 | 57 | `SRCCore/Commands/Command.state.cs` Command.RestoreSelections | 修正済み | #1521。退避したユニット・ターゲット（イベント用を含む）の CurrentForm を選ぶ。VB が記録する 13 の項目だけを戻し、コマンド・パイロット・アイテム・スペシャルパワー・合体技のパートナーは戻さない |
| Sub SwapSelections | 8820 | 20 | `SRCCore/Commands/Command.state.cs` Command.SwapSelections | 一致 |  |
