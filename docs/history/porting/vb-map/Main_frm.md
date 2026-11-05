# Main.frm

VB 版の `SRC/SRC_20121125/Main.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub FlashObject_GetFlashEvent | 751 | 3 | （見つからない） | 未確認 |  |
| Sub Form_KeyDown | 756 | 55 | （見つからない） | 未確認 |  |
| Sub Form_MouseMove | 813 | 7 | （見つからない） | 未確認 |  |
| Sub Form_Unload | 822 | 30 | （見つからない） | 未確認 |  |
| Sub HScroll_Change | 854 | 13 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.HScroll_Change | 未確認 |  |
| Sub mnuMapCommandItem_Click | 869 | 10 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.mnuMapCommandItem_Click | 未確認 |  |
| Sub mnuUnitCommandItem_Click | 881 | 10 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.mnuUnitCommandItem_Click | 未確認 |  |
| Sub picFace_Click | 893 | 30 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picFace_Click<br>`SRCSharpForm/Forms/Message.cs` frmMessage.picFace_Click | 未確認 |  |
| Sub picMain_DblClick | 925 | 24 | （見つからない） | 未確認 |  |
| Sub picMain_MouseDown | 951 | 90 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseDown | 未確認 |  |
| Sub picMain_MouseMove | 1043 | 152 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseMove | 未確認 |  |
| Sub picMain_MouseUp | 1197 | 9 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.picMain_MouseUp | 未確認 |  |
| Sub Timer1_Timer | 1208 | 7 | （見つからない） | 未確認 |  |
| Sub VScroll_Change | 1217 | 17 | `SRCSharpForm/Forms/Main.cs` SharpMenuRenderer.VScroll_Change | 未確認 |  |
