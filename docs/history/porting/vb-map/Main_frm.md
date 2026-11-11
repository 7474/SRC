# Main.frm

VB 版の `SRC/SRC_20121125/Main.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub FlashObject_GetFlashEvent | 751 | 3 | （なし） | SRC#で不要 | Flash は SRC# では使わない |
| Sub Form_KeyDown | 756 | 55 | `SRCRender/ScreenGUI.cs` ScreenGUI.KeyPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.frmMain_KeyDown | 修正済み | SRCSharpForm で、Esc・Delete・BackSpace はマウスの位置の右クリック、ほかのキーは左クリックとして ProceedInput を呼ぶようにした（何も起きていなかった。#1498）。SRCRender は矢印でマップカーソルを動かし、Enter・Esc でクリック・右クリックする（SRC# のキー操作） |
| Sub Form_MouseMove | 813 | 7 | `SRCRender/ScreenGUI.cs` ScreenGUI.UpdateTooltip<br>`SRCSharpForm/Forms/Main.cs` frmMain.frmMain_MouseMove | 一致 | ツールチップを消し、カーソルを戻す |
| Sub Form_Unload | 822 | 30 | `SRCSharpForm/Forms/Main.cs` frmMain.frmMain_FormClosing | 一致 | 確認して TerminateSRC。SRCRender ではウィンドウを閉じる確認はホスト（Avalonia・Web）の側 |
| Sub HScroll_Change | 854 | 13 | `SRCSharpForm/Forms/Main.cs` frmMain.HScroll_Change | 一致 | SRCRender にはマップのスクロールバーが無い（ドラッグとマップカーソルで動かす） |
| Sub mnuMapCommandItem_Click | 869 | 10 | `SRCRender/ScreenGUI.cs` ScreenGUI.ChooseMenu<br>`SRCSharpForm/Forms/Main.cs` frmMain.mnuMapCommandItem_Click | 一致 | 右ボタンで選んだらキャンセル |
| Sub mnuUnitCommandItem_Click | 881 | 10 | `SRCRender/ScreenGUI.cs` ScreenGUI.ChooseMenu<br>`SRCSharpForm/Forms/Main.cs` frmMain.mnuUnitCommandItem_Click | 一致 | 同上 |
| Sub picFace_Click | 893 | 30 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed・`SRCRender/ScreenGUI.status.cs` ScreenGUI.ChangeDisplayedPilot<br>`SRCSharpForm/Forms/Main.cs` frmMain.picFace_Click | 修正済み | SRCRender でダブルクリックの 2 回目にもう 1 人進んでいたのを直した（VB は Click が 1 回だけ。#1498） |
| Sub picMain_DblClick | 925 | 24 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_DoubleClick | 修正済み | SRCRender は 2 回目の押下で ProceedInput も呼んでいたので、右ダブルクリックでキャンセルが 3 回になっていた。VB と同じく 2 回目は DblClick だけにした（#1498） |
| Sub picMain_MouseDown | 951 | 90 | `SRCCore/Commands/Command.process.cs` Command.ProceedInput・`SRCRender/ScreenGUI.cs` ScreenGUI.PointerPressed<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseDown | 一致 | CommandState ごとの分岐は ProceedInput と一致。VB はユニットの無いマス・マップの外を押したときだけドラッグを始めるが、SRC# は左ボタンを押せばいつも始める（押した時点の CommandState を UI スレッドから読めないため。結果の違いは小さい） |
| Sub picMain_MouseMove | 1043 | 152 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerMoved・HoverMap・DragMap・UpdateTooltip<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseMove | 修正済み | (1) ドラッグの移動量を VB の \ と同じく VBMath.IntDiv で求める（右・下へのドラッグが 1 マス早く動いた） (2) マップ外のマスでは DisplayGlobalStatus を出す（抜けていた）。両方とも SRCRender・SRCSharpForm を直した（#1498）。SRCRender は同じマスの上では表示し直さない（VB は動かすたびに呼ぶ） |
| Sub picMain_MouseUp | 1197 | 9 | `SRCRender/ScreenGUI.cs` ScreenGUI.PointerReleased<br>`SRCSharpForm/Forms/Main.cs` frmMain.picMain_MouseUp | 一致 | ドラッグを解除する |
| Sub Timer1_Timer | 1208 | 7 | （なし） | SRC#で不要 | ＢＧＭの繰り返しは Player が行う（Sound.bas の RestartBGM の行と同じ） |
| Sub VScroll_Change | 1217 | 17 | `SRCSharpForm/Forms/Main.cs` frmMain.VScroll_Change | 一致 | SRCRender にはスクロールバーが無い |
