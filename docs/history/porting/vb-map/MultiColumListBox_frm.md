# MultiColumListBox.frm

VB 版の `SRC/SRC_20121125/MultiColumListBox.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_Activate | 74 | 4 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiColumnListBox<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.frmMultiColumnListBox_Activated | 一致 | SelectedItem = 0、解説を空にする |
| Sub Form_Load | 80 | 6 | `SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.frmMultiColumnListBox_Load | SRC#で不要 | SetWindowPos（いつも手前）だけ |
| Sub Form_Unload | 88 | 8 | `SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.frmMultiColumnListBox_FormClosing | 一致 | SRCRender には閉じるボタンが無い |
| Sub lstItems_MouseDown | 98 | 28 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiColumnPointerPressed・SelectMultiColumn・CancelMultiColumn<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.lstItems_MouseDown | 一致 |  |
| Sub Form_MouseDown | 128 | 11 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiColumnPointerPressed<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.frmMultiColumnListBox_MouseDown | 一致 | 右クリックでキャンセル。TopItem の +1 は ListBox.frm と同じ理由で合わせない |
| Sub lstItems_MouseMove | 141 | 30 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerMoved<br>`SRCSharpForm/Forms/MultiColumListBox.cs` frmMultiColumnListBox.lstItems_MouseMove | 修正済み | 項目の無いところではハイライトを外す（SRCRender・SRCSharpForm。#1498） |
