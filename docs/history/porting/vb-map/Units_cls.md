# Units.cls

VB 版の `SRC/SRC_20121125/Units.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 26 | 3 | `SRCCore/Units/Units.cs` Units.IDCount | 一致 | フィールドの初期値を 0 にしている |
| Sub Class_Terminate | 31 | 10 | （なし） | SRC#で不要 | コレクションを空にするだけ。GC が受け持つ |
| Function NewEnum | 43 | 5 | `SRCCore/Units/Units.cs` Units.Items | 一致 | For Each の代わり |
| Function Add | 51 | 219 | `SRCCore/Units/Units.cs` Units.Add | 修正済み | パーツ合体先の形態を、VB と同じくリストの最初の要素ではなくデータ全体で探すようにした（#1498）。ハイパーモード・パーツ合体の先が空のときのメッセージは、VB の uname の書き間違い（「「」が見つかりません」になる）に合わせず「指定されていません」のまま。合体・分離の先がもうあるときの Exit Function は break にした。結果は同じ |
| Sub Add2 | 272 | 3 | `SRCCore/Units/Units.cs` Units.Add2 | 一致 |  |
| Function CreateID | 277 | 6 | `SRCCore/Units/Units.cs` Units.CreateID | 一致 | VB と同じく IsDefined（名前でも探す）で確かめる |
| Function Count | 285 | 3 | `SRCCore/Units/Units.cs` Units.Count | 一致 |  |
| Sub Delete | 290 | 3 | `SRCCore/Units/Units.cs` Units.Delete | 一致 |  |
| Function Item | 295 | 23 | `SRCCore/Units/Units.cs` Units.Item | 一致 | ID で見つかれば状態を見ずに返す。見つからなければ、破棄されていないユニットを名前で探す |
| Function Item2 | 320 | 8 | `SRCCore/Units/Units.cs` Units.Item2 | 一致 |  |
| Function IsDefined | 330 | 24 | `SRCCore/Units/Units.cs` Units.IsDefined | 一致 |  |
| Function IsDefined2 | 356 | 11 | `SRCCore/Units/Units.cs` Units.IsDefined2 | 一致 |  |
| Sub Update | 369 | 332 | `SRCCore/Units/Units.cs` Units.Update・Units.CanSplitOnUpdate・Units.CanCombineOnUpdate | 一致 | 母艦から降ろす・修理と治療（費用と表示）・待機に戻す・3 段階の形態の処理・暴走時パイロット・ダミーパイロット・一時的な形態・破棄と非味方の削除・Reset・Update(True) を分岐ごとに突き合わせた。形態が変わる処理と削除は複製した一覧を回す |
| Sub Save | 704 | 33 | `SRCCore/SRC.save.cs` SRC.SaveData | SRC#で不要 | JSON のセーブ形式 |
| Sub Load | 740 | 64 | `SRCCore/SRC.save.cs` SRC.LoadData | SRC#で不要 | JSON のセーブ形式。旧形式の ID の変換（SaveDataVersion<10700）は要らない。VB は読み込んだユニットを味方にし FullRecover してから LoadLinkInfo の Update(True) と Reset を呼ぶ。C# は LoadData の中で UList.Update を呼んでいて、特殊能力を読み込む前に一時的な形態を消す処理が走り、変形の形態などが消えていた。VB と同じく各ユニットの Update(True) と Reset だけを呼ぶようにした（#1579）。VB の SaveData の前には必ず UList.Update があるので、ユニットは味方だけ |
| Sub LoadLinkInfo | 806 | 84 | `SRCCore/SRC.save.cs` SRC.LoadData | SRC#で不要 | 他形態・パイロット・サポート・アイテムのつながりは JSON（ID）で戻る。アイテムの持ち主は、VB と同じく装備しているユニット（AddItem0）にする。LoadData でアイテムの持ち主を空にしてから、Unit.Restore で設定する（#1579。合体した後などは Item.Unit が空のまま装備していることがある）。VB は持ち主が空のアイテムだけを CurrentForm に AddItem0 するが、C# は装備していた形態に付け、いくつかのユニットが同じアイテムを持つときは後のユニットが持ち主になる（JSON は装備していた形態で書くので、ふつうは同じ）。旧形式の互換は要らない |
| Sub Dump | 893 | 9 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON のセーブ形式 |
| Sub Restore | 904 | 20 | `SRCCore/Units/Units.cs` Units.Restore | SRC#で不要 | JSON から読み込み、各ユニットの SRC の参照を戻す |
| Sub RestoreLinkInfo | 926 | 10 | `SRCCore/SRC.save.cs` SRC.RestoreData | SRC#で不要 | JSON のセーブ形式。各ユニットの Unit.RestoreLinkInfo は呼んでいる |
| Sub RestoreParameter | 938 | 10 | （なし） | SRC#で不要 | ユニットのＨＰ・ＥＮは JSON の値をそのまま使う |
| Sub Clear | 951 | 7 | `SRCCore/Units/Units.cs` Units.Clear | 一致 | IDCount は VB と同じく戻さない |
| Sub ClearUnitBitmap | 960 | 19 | （なし） | SRC#で不要 | picUnitBitmap（VB の画面部品）の後始末。SRC# はユニットを描くたびに描画モードを掛ける |
| Sub CheckAutoHyperMode | 982 | 7 | `SRCCore/Units/Units.cs` Units.CheckAutoHyperMode | 一致 |  |
| Sub CheckAutoNormalMode | 991 | 15 | `SRCCore/Units/Units.cs` Units.CheckAutoNormalMode | 一致 |  |
| Sub Clean | 1008 | 40 | `SRCCore/Units/Units.cs` Units.Clean | 一致 | パイロットとサポートの Alive を AllRawPilots でまとめて False にする |
