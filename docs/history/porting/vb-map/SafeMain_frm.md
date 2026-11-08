# SafeMain.frm

VB 版の `SRC/SRC_20121125/SafeMain.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_KeyDown | 741 | 55 | `SRCRender/ScreenGUI.cs` ScreenGUI.KeyPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.frmMain_KeyDown | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub Form_MouseMove | 798 | 7 | `SRCRender/ScreenGUI.cs` ScreenGUI.UpdateTooltip<br>`SRCSharpForm/Forms/Main.cs` frmMain.frmMain_MouseMove | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub Form_Unload | 807 | 30 | `SRCSharpForm/Forms/Main.cs` frmMain.frmMain_FormClosing | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub HScroll_Change | 839 | 13 | `SRCSharpForm/Forms/Main.cs` frmMain.HScroll_Change | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub mnuMapCommandItem_Click | 854 | 10 | `SRCRender/ScreenGUI.cs` ScreenGUI.ChooseMenu<br>`SRCSharpForm/Forms/Main.cs` frmMain.mnuMapCommandItem_Click | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub mnuUnitCommandItem_Click | 866 | 10 | `SRCRender/ScreenGUI.cs` ScreenGUI.ChooseMenu<br>`SRCSharpForm/Forms/Main.cs` frmMain.mnuUnitCommandItem_Click | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub picFace_Click | 878 | 30 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed・`SRCRender/ScreenGUI.status.cs` ScreenGUI.ChangeDisplayedPilot<br>`SRCSharpForm/Forms/Main.cs` frmMain.picFace_Click | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub picFace_DblClick | 910 | 15 | （なし） | SRC#で不要 | SafeMain だけにある（追加サポートを数えずにパイロットを回す）。SRC# は Main.frm の picFace_Click に合わせている |
| Sub picMain_DblClick | 927 | 24 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_DoubleClick | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub picMain_MouseDown | 953 | 71 | `SRCCore/Commands/Command.process.cs` Command.ProceedInput・`SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseDown | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub picMain_MouseMove | 1026 | 127 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerMoved・HoverMap・DragMap・UpdateTooltip<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseMove | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub picMain_MouseUp | 1155 | 9 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerReleased<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseUp | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub Timer1_Timer | 1166 | 7 | （なし） | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
| Sub VScroll_Change | 1175 | 17 | `SRCSharpForm/Forms/Main.cs` frmMain.VScroll_Change | SRC#で不要 | Main.frm の同名の手続きに合わせた（SafeMain は Flash 無しの環境のための Main.frm の古い複製で、SRC# は Main.frm の側に合わせている） |
