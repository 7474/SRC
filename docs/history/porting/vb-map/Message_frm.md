# Message.frm

VB 版の `SRC/SRC_20121125/Message.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_Click | 297 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.frmMessage_Click | 一致 | IsFormClicked = True |
| Sub Form_DblClick | 302 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.frmMessage_DoubleClick | 一致 | 同上 |
| Sub Form_KeyDown | 307 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.KeyPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.frmMessage_KeyDown | 一致 | SRCSharpForm はどのキーでも送る。SRCRender は Enter・Esc で送る（矢印はキー操作に使う） |
| Sub Form_MouseDown | 312 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed | 一致 | どのボタンでも IsFormClicked |
| Sub Form_Unload | 317 | 17 | `SRCSharpForm/Forms/Message.cs` frmMessage.frmMessage_FormClosing | 一致 | SRCRender のメッセージウィンドウには閉じるボタンが無い |
| Sub picFace_MouseDown | 336 | 7 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.picFace_Click | 修正済み | SRCRender は左ボタンだけ受けていたので、右ボタンでも自動送りを切り替えるようにした（#1498）。MessageWait < 10000 の判定は一致 |
| Sub picMessage_DblClick | 345 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.picMessage_DoubleClick | 一致 |  |
| Sub picMessage_MouseDown | 350 | 3 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Message.cs` frmMessage.picMessage_Click | 一致 |  |
