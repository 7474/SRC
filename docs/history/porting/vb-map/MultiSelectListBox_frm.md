# MultiSelectListBox.frm

VB 版の `SRC/SRC_20121125/MultiSelectListBox.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub cmdFinish_Click | 211 | 9 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectPointerPressed<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.cmdFinish_Click | 一致 | 選択の印は項目が持つので ItemFlag の写しは要らない |
| Sub cmdResume_Click | 222 | 9 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectResume<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.cmdResume_Click | 一致 |  |
| Sub cmdSelectAll_Click | 233 | 37 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectPointerPressed<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.cmdSelectAll_Click | 一致 | 先頭から MaxListItem 個、TopIndex = 0 |
| Sub cmdSelectAll2_Click | 272 | 38 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectPointerPressed<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.cmdSelectAll2_Click | 一致 | 最後から、TopIndex = MaxLng(ListCount - 14, 0) |
| Sub cmdSort_Click | 312 | 97 | `SRCCore/UIInterface/MultiSelectListBoxSort.cs` MultiSelectListBoxSort.SortByLevel・SortByName<br>`SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectPointerPressed<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.cmdSort_Click | 修正済み | 向きが逆だった。VB のレベル順は 500 × Level + Exp の大きい順、名称順は KanaName の小さい順（vbTextCompare）。VB と同じ選択ソートにし、呼び出し元の一覧そのものを並べ替える（出撃させる順も並べ替えた順になる）。並べ替えたら TopIndex = 0（#1498） |
| Sub Form_Activate | 411 | 8 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectListBox<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.frmMultiSelectListBox_Activated | 一致 | 先頭のユニットを表示、0/最大数 |
| Sub lstItems_DblClick | 421 | 54 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectPointerPressed<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox._lstItems_DoubleClick | 一致 | VB は MouseDown と DblClick で 2 回切り替わる。SRC# も 2 回目の押下で切り替わる |
| Sub lstItems_MouseDown | 477 | 59 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectToggle<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox._lstItems_MouseDown | 一致 | 左ボタンだけ |
| Sub lstItems_MouseMove | 538 | 17 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBoxPointerMoved<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox._lstItems_MouseMove | 修正済み | 項目の無いところではハイライトを外す（#1498） |
| Sub Timer1_Timer | 557 | 20 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.OnListSelectionChanged<br>`SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox._lstItems_MouseMove | 一致 | ユニット選択中だけ、カーソルの項目のユニットを表示する |
