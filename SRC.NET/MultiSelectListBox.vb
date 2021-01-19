Option Strict Off
Option Explicit On
Friend Class frmMultiSelectListBox
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private SelectedItemNum As Short
	'Invalid_string_refer_to_original_code
	Private ItemFlag() As Boolean
	
	'Invalid_string_refer_to_original_code
	Private Sub cmdFinish_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdFinish.Click
		Dim i As Short
		
		For i = 1 To UBound(ListItemFlag)
			ListItemFlag(i) = ItemFlag(i - 1)
		Next 
		ClearUnitStatus()
		Me.Close()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub cmdResume_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdResume.Click
		Dim i As Short
		
		For i = 1 To UBound(ListItemFlag)
			ListItemFlag(i) = False
		Next 
		ClearUnitStatus()
		Me.Close()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub cmdSelectAll_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSelectAll.Click
		Dim i As Short
		
		lstItems.Visible = False
		For i = 1 To lstItems.Items.Count
			ItemFlag(i - 1) = False
			VB6.SetItemString(lstItems, i - 1, "ã€€" & Mid(VB6.GetItemString(lstItems, i - 1), 2))
		Next 
		For i = 1 To MinLng(MaxListItem, lstItems.Items.Count)
			If Not ItemFlag(i - 1) Then
				ItemFlag(i - 1) = True
				VB6.SetItemString(lstItems, i - 1, "Invalid_string_refer_to_original_code")
			End If
		Next 
		lstItems.TopIndex = 0
		lstItems.Visible = True
		
		SelectedItemNum = 0
		For i = 0 To lstItems.Items.Count - 1
			If ItemFlag(i) Then
				SelectedItemNum = SelectedItemNum + 1
			End If
		Next 
		lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
		
		If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
			If Not cmdFinish.Enabled Then
				cmdFinish.Enabled = True
			End If
		Else
			If cmdFinish.Enabled Then
				cmdFinish.Enabled = False
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub cmdSelectAll2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSelectAll2.Click
		Dim i As Short
		
		lstItems.Visible = False
		For i = 1 To lstItems.Items.Count
			ItemFlag(i - 1) = False
			VB6.SetItemString(lstItems, i - 1, "ã€€" & Mid(VB6.GetItemString(lstItems, i - 1), 2))
		Next 
		For i = 1 To MinLng(MaxListItem, lstItems.Items.Count)
			If Not ItemFlag(lstItems.Items.Count - i) Then
				ItemFlag(lstItems.Items.Count - i) = True
				VB6.SetItemString(lstItems, lstItems.Items.Count - i, "Invalid_string_refer_to_original_code")
			End If
		Next 
		lstItems.TopIndex = MaxLng(lstItems.Items.Count - 14, 0)
		lstItems.Visible = True
		
		SelectedItemNum = 0
		For i = 0 To lstItems.Items.Count - 1
			If ItemFlag(i) Then
				SelectedItemNum = SelectedItemNum + 1
			End If
		Next 
		lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
		
		If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
			If Not cmdFinish.Enabled Then
				cmdFinish.Enabled = True
			End If
		Else
			If cmdFinish.Enabled Then
				cmdFinish.Enabled = False
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub cmdSort_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSort.Click
		Dim item_list() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim i, j As Short
		Dim buf As String
		Dim flag As Boolean
		
		'Invalid_string_refer_to_original_code
		With lstItems
			ReDim item_list(.Items.Count)
			For i = 1 To .Items.Count
				item_list(i) = VB6.GetItemString(lstItems, i - 1)
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		ReDim key_list(UBound(item_list))
		With UList
			For i = 1 To UBound(item_list)
				With .Item(ListItemID(i)).MainPilot
					key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
				End With
			Next 
		End With
		For i = 1 To UBound(item_list) - 1
			max_item = i
			max_value = key_list(i)
			For j = i + 1 To UBound(item_list)
				If key_list(j) > max_value Then
					max_item = j
					max_value = key_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = item_list(i)
				item_list(i) = item_list(max_item)
				item_list(max_item) = buf
				
				buf = ListItemID(i)
				ListItemID(i) = ListItemID(max_item)
				ListItemID(max_item) = buf
				
				flag = ItemFlag(i - 1)
				ItemFlag(i - 1) = ItemFlag(max_item - 1)
				ItemFlag(max_item - 1) = flag
				
				key_list(max_item) = key_list(i)
			End If
		Next 
		'Invalid_string_refer_to_original_code
		cmdSort.Text = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		ReDim strkey_list(UBound(item_list))
		With UList
			For i = 1 To UBound(item_list)
				strkey_list(i) = .Item(ListItemID(i)).KanaName
			Next 
		End With
		For i = 1 To UBound(item_list) - 1
			max_item = i
			max_str = strkey_list(i)
			For j = i + 1 To UBound(item_list)
				If StrComp(strkey_list(j), max_str, 1) = -1 Then
					max_item = j
					max_str = strkey_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = item_list(i)
				item_list(i) = item_list(max_item)
				item_list(max_item) = buf
				
				buf = ListItemID(i)
				ListItemID(i) = ListItemID(max_item)
				ListItemID(max_item) = buf
				
				flag = ItemFlag(i - 1)
				ItemFlag(i - 1) = ItemFlag(max_item - 1)
				ItemFlag(max_item - 1) = flag
				
				strkey_list(max_item) = strkey_list(i)
			End If
		Next 
		'Invalid_string_refer_to_original_code
		cmdSort.Text = "Invalid_string_refer_to_original_code"
		'End If
		
		'ãƒªã‚¹ãƒˆè¡¨ç¤ºã‚’æ›´æ–°ã™ã‚‹
		With lstItems
			.Visible = False
			For i = 1 To .Items.Count
				VB6.SetItemString(lstItems, i - 1, item_list(i))
			Next 
			.TopIndex = 0
			.Visible = True
		End With
	End Sub
	
	'ãƒ•ã‚©ãƒ¼ãƒ ã‚’è¡¨ç¤º
	'UPGRADE_WARNING: Form ƒCƒxƒ“ƒg frmMultiSelectListBox.Activate ‚É‚ÍV‚µ‚¢“®ì‚ªŠÜ‚Ü‚ê‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Private Sub frmMultiSelectListBox_Activated(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Activated
		SelectedItemNum = 0
		lblNumber.Text = "0/" & VB6.Format(MaxListItem)
		ReDim ItemFlag(lstItems.Items.Count)
		If lstItems.Items.Count > 0 Then
			DisplayUnitStatus(UList.Item(ListItemID(1)))
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub lstItems_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstItems.DoubleClick
		Dim i As Short
		
		i = lstItems.SelectedIndex
		If i >= 0 Then
			If ItemFlag(i) Then
				'é¸æŠå–ã‚Šæ¶ˆã—
				
				'Invalid_string_refer_to_original_code
				SelectedItemNum = SelectedItemNum - 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = False
				
				'Invalid_string_refer_to_original_code
				VB6.SetItemString(lstItems, i, "ã€€" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'Invalid_string_refer_to_original_code
				If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
					If Not cmdFinish.Enabled Then
						cmdFinish.Enabled = True
					End If
				Else
					If cmdFinish.Enabled Then
						cmdFinish.Enabled = False
					End If
				End If
			Else
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				SelectedItemNum = SelectedItemNum + 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = True
				
				'Invalid_string_refer_to_original_code
				VB6.SetItemString(lstItems, i, "Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
					If Not cmdFinish.Enabled Then
						cmdFinish.Enabled = True
					End If
				Else
					If cmdFinish.Enabled Then
						cmdFinish.Enabled = False
					End If
				End If
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub lstItems_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If Button <> 1 Then
			Exit Sub
		End If
		
		i = lstItems.SelectedIndex
		If i >= 0 Then
			If ItemFlag(i) Then
				'é¸æŠå–ã‚Šæ¶ˆã—
				
				'Invalid_string_refer_to_original_code
				SelectedItemNum = SelectedItemNum - 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = False
				
				'Invalid_string_refer_to_original_code
				VB6.SetItemString(lstItems, i, "ã€€" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'Invalid_string_refer_to_original_code
				If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
					If Not cmdFinish.Enabled Then
						cmdFinish.Enabled = True
					End If
				Else
					If cmdFinish.Enabled Then
						cmdFinish.Enabled = False
					End If
				End If
			Else
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				SelectedItemNum = SelectedItemNum + 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = True
				
				'Invalid_string_refer_to_original_code
				VB6.SetItemString(lstItems, i, "Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				If SelectedItemNum > 0 And SelectedItemNum <= MaxListItem Then
					If Not cmdFinish.Enabled Then
						cmdFinish.Enabled = True
					End If
				Else
					If cmdFinish.Enabled Then
						cmdFinish.Enabled = False
					End If
				End If
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub lstItems_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim itm As Short
		
		'Invalid_string_refer_to_original_code
		itm = ((Y * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width) + 1) \ 16
		itm = itm + lstItems.TopIndex
		
		'Invalid_string_refer_to_original_code
		If itm < 0 Or itm >= lstItems.Items.Count Then
			lstItems.SelectedIndex = -1
			Exit Sub
		End If
		If lstItems.SelectedIndex = itm Then
			Exit Sub
		End If
		lstItems.SelectedIndex = itm
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		Dim u As Unit
		
		If Not Visible Then
			Exit Sub
		End If
		
		If lstItems.SelectedIndex = -1 Then
			Exit Sub
		End If
		
		u = UList.Item(ListItemID(lstItems.SelectedIndex + 1))
		
		If Not DisplayedUnit Is u Then
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			DisplayUnitStatus(u)
		End If
		'End If
	End Sub
End Class