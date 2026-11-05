# ListBox.frm

VB 版の `SRC/SRC_20121125/ListBox.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_KeyDown | 573 | 38 | （見つからない） | 未確認 |  |
| Sub Form_Load | 613 | 8 | （見つからない） | 未確認 |  |
| Sub Form_Unload | 623 | 8 | （見つからない） | 未確認 |  |
| Sub lstItems_DblClick | 633 | 34 | （見つからない） | 未確認 |  |
| Sub lstItems_MouseDown | 669 | 36 | `SRCSharpForm/Forms/ListBox.cs` frmListBox.lstItems_MouseDown<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.lstItems_MouseDown | 未確認 |  |
| Sub labCaption_MouseDown | 707 | 27 | `SRCSharpForm/Forms/ListBox.cs` frmListBox.labCaption_MouseDown | 未確認 |  |
| Sub Form_MouseDown | 736 | 12 | （見つからない） | 未確認 |  |
| Sub lstItems_MouseMove | 750 | 22 | `SRCSharpForm/Forms/ListBox.cs` frmListBox.lstItems_MouseMove<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.lstItems_MouseMove | 未確認 |  |
| Sub Timer1_Timer | 774 | 14 | （見つからない） | 未確認 |  |
| Sub Timer2_Timer | 790 | 39 | （見つからない） | 未確認 |  |
