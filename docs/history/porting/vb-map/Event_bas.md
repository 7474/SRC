# Event.bas

VB 版の `SRC/SRC_20121125/Event.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitEventData | 359 | 18 | `SRCCore/Events/Event.data.cs` Event.InitEventData | 一致 | EventCmd を先に作っておく処理は SRC# では要らない（EventCmd は ParseCommand で作る） |
| Sub LoadEventData | 379 | 883 | `SRCCore/Events/Event.data.cs` Event.LoadEventData | 違いあり | C# は RegisterLabel・ParseCommand・ValidateCommandArgs・LoadData・LoadTitles に分かれる。直したもの（#1498）: (1) ラベルを登録する間は CurrentLineNum を登録する行にし、終わったら元に戻す (2) Talk・AutoTalk・Ask・Question の中の空行の次の行の最初の語が、対応するコマンド・End・Suspend 以外で半角文字だけなら「Talkに対応するEndがありません」とする（抜けていた。最後の行の次は空行とみなす） (3) Exit・PlaySound・Wait のメッセージを VB と同じ「Talkに対応するEndがありません」にする (4) 引数の数の書式チェックはシナリオ側の行だけにする (5) load_mode が「システム」のときは、ラベルの末尾の全角の「：」を調べない (6) EventFileNames にはシステム側のファイルを SysEventFileNum 個だけ残す。SRC# の都合で合わせないもの: シナリオの Lib をいつも読み直す（読み直しても同じデータ）・行の結合とコメントは SrcEveReader が行い " " の行を残さない・ラベルは毎回全部の行を登録し直す・SkipExternalSourceLoad・ロード画面の進み具合の数。残る違い: システム側のイベントラベルの Enable が LoadEventData のたびに戻る（#1529） |
| Sub LoadEventData2 | 1264 | 119 | `SRCCore/Events/Event.data.cs` Event.LoadEventData2 | 修正済み | 1 行も読めないうちのエラーは「が開けません」と表示する（#1498）。引数 lnum の代わりに EventDataSource を取り、行はいつも末尾に足す（VB で lnum を渡すのも末尾から足すとき）。コメント行と結合した行を EventData に入れないのは SrcEveReader の作り。読み込むファイル名の比較に PathEquals と Trim を使うのは SRC# の都合 |
| Sub HandleEvent | 1386 | 339 | `SRCCore/Events/Event.handle.cs` Event.HandleEvent | 修正済み | (1) イベントデータの末尾を越えて実行したら、キューに残ったイベントも実行せずに終わる（VB の GoTo ExitLoop）。これまでは次のラベル・次のキューへ進んでいた (2) ターンのキューの文字列を "ターン 全 味方" の形にした（"ターン " と Join で空白が 2 つ。SearchLabel は ListSplit なので動作は同じ） (3) CurrentLabel の初期値を -1 にした（VB の 0 ＝ラベルが無い。#1503 の手がかり。Event.props.cs）（#1498）。マップ攻撃破壊で格納ユニットを降ろす VB の For i = 1 To .CountUnitOnBoard と UnitOnBoard(i) は、2 体以上だと 1 つおきになり範囲外を参照する VB の不具合なので、破壊と同じくすべて降ろす C# のまま。マップ攻撃破壊で全滅を判定しないのは VB と同じ（全滅は Unit.attackmap.cs が HandleEvent する。VB: Unit.cls）。呼び出し階層を超えたときにキューを戻さずロックも外さずに抜けるのは VB と同じ。CallDepth < 0 の分岐は通らない（VarIndex は C# の空の -1 にした） |
| Sub RegisterEvent | 1727 | 9 | `SRCCore/Events/Event.handle.cs` Event.RegisterEvent | 一致 |  |
| Function SearchLabel | 1739 | 273 | `SRCCore/Events/Event.label.cs` Event.SearchLabel | 一致 | パラメータの比較は Event.IsMatch に切り出してある。入れ替えて比べ直す流れ・数値と CLng の比較・損傷率の扱いは VB と一致する。比べる数は VB と同じく ListSplit の戻り値（llen）にした（#1498。動作は変わらない）。見つからないときの戻り値は 0 ではなく -1（SRC# の行番号は 0 始まり） |
| Function IsEventDefined | 2016 | 25 | `SRCCore/Events/Event.label.cs` Event.IsEventDefined | 一致 | Asc(EventData(ret)) <> 42 は EventDataLine.IsAlwaysEventLabel で見る。行番号は 0 始まり |
| Function IsLabelDefined | 2043 | 11 | `SRCCore/Events/Event.label.cs` Event.IsLabelDefined | 一致 | SrcCollection のキーの比較は VB の Collection と同じ。番号での指定は使われていない |
| Sub AddLabel | 2056 | 41 | `SRCCore/Events/Event.label.cs` Event.AddLabel | 修正済み | ラベル名の正規化を VB と同じにした（括弧の対応が取れていなければ ListLength が -1 なので、最初の要素だけになる）（#1498）。登録済みの行を無視するのは、全部の行を登録し直す SRC# の都合 |
| Sub AddSysLabel | 2099 | 43 | `SRCCore/Events/Event.label.cs` Event.AddSysLabel | 修正済み | 「名前(行番号)」のキーも登録済みなら、VB の On Error と同じく無視する（例外になっていた）。正規化は AddLabel と同じ（#1498） |
| Sub ClearLabel | 2144 | 25 | `SRCCore/Events/Event.label.cs` Event.ClearLabel | 一致 | 前後 10 行を探す処理も同じ。ただし SRC# の EventData にはコメント行・結合した行が無いので、行の数え方が VB と違う |
| Sub RestoreLabel | 2171 | 12 | `SRCCore/Events/Event.label.cs` Event.RestoreLabel | 一致 |  |
| Function FindLabel | 2185 | 26 | `SRCCore/Events/Event.label.cs` Event.FindLabel | 修正済み | 2 回目に探すときの正規化を VB と同じにした（#1498）。見つからないときの戻り値は -1 |
| Function FindEventLabel | 2213 | 11 | `SRCCore/Events/Event.label.cs` Event.FindEventLabel | 一致 | 見つからないときの戻り値は -1 |
| Function FindNormalLabel | 2226 | 6 | `SRCCore/Events/Event.label.cs` Event.FindNormalLabel | 一致 |  |
| Function FindNormalLabel0 | 2234 | 11 | `SRCCore/Events/Event.label.cs` Event.FindNormalLabel0 | 一致 |  |
| Function FindSysNormalLabel | 2247 | 11 | `SRCCore/Events/Event.label.cs` Event.FindSysNormalLabel | 一致 |  |
| Sub ClearEventData | 2262 | 52 | `SRCCore/Events/Event.data2.cs` Event.ClearEventData | 修正済み | HotPointList を空の List にした。固定長の配列 new HotPoint[1] にしていたので、後の HotPoint コマンドの Add が例外になり、要素も 1 つ残っていた（#1498）。VarIndex = -1 は C# の空（VB の 0）。ラベルの消し方（システムのデータより後の行のラベルを消す）は VB と同じ |
| Sub ClearAllEventData | 2316 | 14 | （見つからない） | SRC#で不要 | VB でも呼ばれていない。コメントアウトされていた C# は消した |
| Sub DumpEventData | 2333 | 20 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON のセーブ（SRCSuspendData の GlobalVariableList・LocalVariableList・DisableEventLabels・AdditionalEventFileNames）に置き換えた |
| Sub RestoreEventData | 2355 | 176 | `SRCCore/Events/Event.cs` Event.Restore | 違いあり | JSON のセーブから Require のファイルを読み直し、ラベルの有効・無効を戻す。VB はラベルの有効・無効を colEventLabelList の順番で持つが、C# は無効なラベルの文字列（Data の Distinct）で持つので、同じ文字列のイベントラベルが複数あり 1 つだけ ClearEvent したとき、再開後はすべて無効になる（#1529） |
| Sub SkipEventData | 2533 | 29 | （見つからない） | SRC#で不要 | JSON のセーブは 1 度に読むので読み飛ばしは要らない |
| Sub SaveGlobalVariables | 2564 | 14 | `SRCCore/SRC.save.cs` SRC.SaveData・SRC.DumpData | SRC#で不要 | JSON の GlobalVariableList に置き換えた。VB は数値を Format$ で書くが JSON は double をそのまま持つ |
| Sub LoadGlobalVariables | 2580 | 143 | `SRCCore/SRC.save.cs` SRC.LoadData・SRC.RestoreData | SRC#で不要 | JSON の GlobalVariableList に置き換えた。古い版のセーブ（SaveDataVersion < 20027）の変換と SetNewGUIMode は SRC# では要らない |
| Sub SaveLocalVariables | 2725 | 17 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON の LocalVariableList に置き換えた。名前に " があるときの ErrorMessage は VB のセーブ形式のためのもの |
| Sub LoadLocalVariables | 2744 | 61 | `SRCCore/SRC.save.cs` SRC.RestoreData | SRC#で不要 | JSON の LocalVariableList に置き換えた。古い版のセーブ（< 10731）の変換は要らない |
| Sub DisplayEventErrorMessage | 2808 | 19 | `SRCCore/Events/Event.ui.cs` Event.DisplayEventErrorMessage | 一致 | 前後の行の表示の条件は 0 始まりにして同じ。ファイル名は EventDataLine.File。DataErrors への記録とデバッガへの通知は SRC# の都合 |
| Sub MakeUnitList | 2829 | 283 | `SRCCore/Events/Event.unitlist.cs` Event.MakeUnitList | 修正済み | 並べ替えを VB と同じ選択ソートにした。数値は「>」で大きいものを前と入れ替えるので、同じ値の並びが VB と同じになる（これまでは安定なソート）。「名称」は読み仮名を StrComp(…, vbTextCompare) で比べる（これまではカルチャに依存する OrderBy）。読み仮名は「名称」のときだけ見る（等身大基準でパイロットの乗っていないユニットがあると、ＨＰなどの一覧でも MainPilot で落ちていた）（#1498）。フォントを戻す ResetDrawString は色も白に戻すが、VB の描画の色は変わっていないので同じ |
| Sub SaveBasePoint | 3115 | 8 | `SRCCore/Events/Event.draw.cs` Event.SaveBasePoint | 一致 |  |
| Sub RestoreBasePoint | 3125 | 8 | `SRCCore/Events/Event.draw.cs` Event.RestoreBasePoint | 一致 |  |
| Sub ResetBasePoint | 3135 | 11 | `SRCCore/Events/Event.draw.cs` Event.ResetBasePoint | 一致 | C# は 0 番目も 0 にするが、RestoreBasePoint は 0 番目を読まないので同じ |
