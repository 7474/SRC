# SafeMain.frm

VB 版の `SRC/SRC_20121125/SafeMain.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_KeyDown | 741 | 55 | （見つからない） | 未確認 |  |
| Sub Form_MouseMove | 798 | 7 | （見つからない） | 未確認 |  |
| Sub Form_Unload | 807 | 30 | （見つからない） | 未確認 |  |
| Sub HScroll_Change | 839 | 13 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.HScroll_Change | 未確認 |  |
| Sub mnuMapCommandItem_Click | 854 | 10 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.mnuMapCommandItem_Click | 未確認 |  |
| Sub mnuUnitCommandItem_Click | 866 | 10 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.mnuUnitCommandItem_Click | 未確認 |  |
| Sub picFace_Click | 878 | 30 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picFace_Click<br>`SRCSharpForm/Forms/Message.cs` frmMessage.picFace_Click | 未確認 |  |
| Sub picFace_DblClick | 910 | 15 | （見つからない） | 未確認 |  |
| Sub picMain_DblClick | 927 | 24 | （見つからない） | 未確認 |  |
| Sub picMain_MouseDown | 953 | 71 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseDown | 未確認 |  |
| Sub picMain_MouseMove | 1026 | 127 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseMove | 未確認 |  |
| Sub picMain_MouseUp | 1155 | 9 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseUp | 未確認 |  |
| Sub Timer1_Timer | 1166 | 7 | （見つからない） | 未確認 |  |
| Sub VScroll_Change | 1175 | 17 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.VScroll_Change | 未確認 |  |
