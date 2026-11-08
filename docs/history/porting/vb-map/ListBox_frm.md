# ListBox.frm

VB 版の `SRC/SRC_20121125/ListBox.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_KeyDown | 573 | 38 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxKeyPressed<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.frmListBox_KeyDown | 一致 | SRCRender は Enter で選び、Esc でキャンセルする（SRC# のキー操作）。Esc のときに VB が TopItem に TopIndex を（+1 せずに）入れる点は、0 と 1 の始まりの書き間違いとみなし、SRCRender は +1 にそろえている |
| Sub Form_Load | 613 | 8 | `SRCRender/Compat/ListWindow.cs` ListWindow.ListWindow<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.frmListBox_Load | 一致 | HorizontalSize・VerticalSize を "M" にする。SetWindowPos は SRC# では要らない |
| Sub Form_Unload | 623 | 8 | `SRCSharpForm/Forms/ListBox.cs` frmListBox.frmListBox_FormClosing | 一致 | SRCRender のリストには閉じるボタンが無い |
| Sub lstItems_DblClick | 633 | 34 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerPressed<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.lstItems_DoubleClick | 一致 | ダブルクリックの 2 回目も押したときと同じ選択になり、「連続で選択」と同じ結果になる |
| Sub lstItems_MouseDown | 669 | 36 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerPressed・SelectListItem・CancelListBox<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.lstItems_MouseDown | 一致 | 無効な項目は選べない。SRCRender は右クリックをリストの外でもキャンセルにする（SRC# の操作のしやすさ） |
| Sub labCaption_MouseDown | 707 | 27 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerPressed<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.labCaption_MouseDown | 修正済み | SRCRender で、メインウィンドウを表示しているときだけ表示するユニットを入れ替えるようにした（MainForm.Visible の判定が抜けていた。#1498） |
| Sub Form_MouseDown | 736 | 12 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerPressed<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.frmListBox_MouseDown | 一致 | 右クリックでキャンセルする。TopItem に +1 しない点は Form_KeyDown と同じく合わせない |
| Sub lstItems_MouseMove | 750 | 22 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerMoved・OnListSelectionChanged<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.lstItems_MouseMove | 修正済み | SRCRender で、最後の項目より下では ListIndex = -1 と同じくハイライトを外すようにした（#1498）。上下キーで動かしたときもコメント欄を更新するのは SRC# のキー操作 |
| Sub Timer1_Timer | 774 | 14 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ShowItems・UpdateTimeLimitBar<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.Timer1_Tick | 一致 | 制限時間が来たらキャンセルとして閉じる。SRCRender のバーはミリ秒で伸びる |
| Sub Timer2_Timer | 790 | 39 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.OnListSelectionChanged<br>`SRCSharpForm/Forms/ListBox.cs` frmListBox.Timer2_Tick | 修正済み | SRCRender は一致。SRCSharpForm は処理が空だったので、選んでいる項目のユニットをステータスに出す処理を入れた（#1498） |
