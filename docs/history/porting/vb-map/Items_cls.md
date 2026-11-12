# Items.cls

VB 版の `SRC/SRC_20121125/Items.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 24 | 10 | （なし） | SRC#で不要 | コレクションを空にするだけ。GC が受け持つ |
| Function NewEnum | 36 | 5 | `SRCCore/Items/Items.cs` Items.List | 一致 | For Each の代わり |
| Function Add | 44 | 16 | `SRCCore/Items/Items.cs` Items.Add | 一致 | ID は渡された名前で、名前は IDList の正式な名前で作る（VB と同じ） |
| Function CreateID | 62 | 6 | `SRCCore/Items/Items.cs` Items.CreateID | 一致 | IsDefined2（ID だけ）で確かめる |
| Function Count | 70 | 3 | `SRCCore/Items/Items.cs` Items.Count | 一致 |  |
| Sub Delete | 75 | 3 | `SRCCore/Items/Items.cs` Items.Delete | 一致 |  |
| Function Item | 80 | 25 | `SRCCore/Items/Items.cs` Items.Item | 一致 | ID で見つかっても破棄されていれば、名前で探し直す。キーは SrcCollection で大文字・小文字を区別しない。名前の比較は大文字・小文字を区別する（VB と同じ）。数値の Index は使われていないので受けない |
| Function IsDefined | 107 | 26 | `SRCCore/Items/Items.cs` Items.IsDefined | 一致 | Item と同じ判定 |
| Function IsDefined2 | 135 | 11 | `SRCCore/Items/Items.cs` Items.IsDefined2 | 一致 |  |
| Sub Update | 148 | 21 | `SRCCore/Items/Items.cs` Items.Update | 一致 | 破棄されたアイテムは複製した一覧を回して消す。C# では Item.Unit がいつも UList から引くので、リンクの付け直しは同じ結果になる。Units.Update で消したユニットのアイテムは、VB では Items.Update までユニットを指したまま、C# では消した時点で空になるが、間の PList.Update はアイテムのユニットを見ないので影響は無い |
| Sub Save | 172 | 16 | `SRCCore/SRC.save.cs` SRC.SaveData | SRC#で不要 | JSON のセーブ形式。IDCount と colItems（ID・名前・UnitId）を保存する |
| Sub Load | 190 | 34 | `SRCCore/SRC.save.cs` SRC.LoadData | SRC#で不要 | JSON のセーブ形式。読み込んだ後の状態（名前・ID・装備しているユニット）は VB の Load と Units.LoadLinkInfo の結果と同じ。装備しているユニットは、VB と同じく JSON の UnitId を使わず、ユニットが装備しているアイテムから付け直す（#1579）。アイテムのデータが定義されていないとき、VB は「…のデータが定義されていません」で止まり、C# は「ロード中にエラーが発生しました」で止まる（メッセージだけの違い） |
| Sub LoadLinkInfo | 226 | 19 | （なし） | SRC#で不要 | VB も読み飛ばすだけ（アイテムとユニットのリンクは Units.LoadLinkInfo が付ける） |
| Sub Dump | 248 | 8 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON のセーブ形式 |
| Sub Restore | 258 | 20 | `SRCCore/Items/Items.cs` Items.Restore | SRC#で不要 | JSON から読み込み、各アイテムの SRC の参照を戻す |
| Sub RestoreLinkInfo | 280 | 10 | （なし） | SRC#で不要 | JSON の Item.UnitId で引くので要らない。その後の Unit.RestoreLinkInfo の AddItem0 に当たる設定は Unit.Restore で行う（#1579） |
| Sub RestoreParameter | 292 | 10 | （なし） | SRC#で不要 | VB も読み飛ばすだけ |
| Sub Clear | 305 | 7 | `SRCCore/Items/Items.cs` Items.Clear | 一致 | IDCount は VB と同じく戻さない |
