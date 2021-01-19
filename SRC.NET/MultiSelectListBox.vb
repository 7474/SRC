Option Strict Off
Option Explicit On
Friend Class frmMultiSelectListBox
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�o�����j�b�g�I��p���X�g�{�b�N�X�̃t�H�[��
	
	'�I�����ꂽ���j�b�g�̐�
	Private SelectedItemNum As Short
	'���j�b�g���I�����ꂽ���ǂ����������t���O
	Private ItemFlag() As Boolean
	
	'�I���I���{�^�����N���b�N
	Private Sub cmdFinish_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdFinish.Click
		Dim i As Short
		
		For i = 1 To UBound(ListItemFlag)
			ListItemFlag(i) = ItemFlag(i - 1)
		Next 
		ClearUnitStatus()
		Me.Close()
	End Sub
	
	'�}�b�v������{�^�����N���b�N
	Private Sub cmdResume_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdResume.Click
		Dim i As Short
		
		For i = 1 To UBound(ListItemFlag)
			ListItemFlag(i) = False
		Next 
		ClearUnitStatus()
		Me.Close()
	End Sub
	
	'�u�擪����I���v�{�^�����N���b�N
	Private Sub cmdSelectAll_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSelectAll.Click
		Dim i As Short
		
		lstItems.Visible = False
		For i = 1 To lstItems.Items.Count
			ItemFlag(i - 1) = False
			VB6.SetItemString(lstItems, i - 1, "�@" & Mid(VB6.GetItemString(lstItems, i - 1), 2))
		Next 
		For i = 1 To MinLng(MaxListItem, lstItems.Items.Count)
			If Not ItemFlag(i - 1) Then
				ItemFlag(i - 1) = True
				VB6.SetItemString(lstItems, i - 1, "��" & Mid(VB6.GetItemString(lstItems, i - 1), 2))
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
	
	'�u�Ōォ��I���v�{�^�����N���b�N
	Private Sub cmdSelectAll2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSelectAll2.Click
		Dim i As Short
		
		lstItems.Visible = False
		For i = 1 To lstItems.Items.Count
			ItemFlag(i - 1) = False
			VB6.SetItemString(lstItems, i - 1, "�@" & Mid(VB6.GetItemString(lstItems, i - 1), 2))
		Next 
		For i = 1 To MinLng(MaxListItem, lstItems.Items.Count)
			If Not ItemFlag(lstItems.Items.Count - i) Then
				ItemFlag(lstItems.Items.Count - i) = True
				VB6.SetItemString(lstItems, lstItems.Items.Count - i, "��" & Mid(VB6.GetItemString(lstItems, lstItems.Items.Count - i), 2))
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
	
	'�u�`���v�{�^�����N���b�N
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
		
		'���݂̃��X�g�\�����e���R�s�[
		With lstItems
			ReDim item_list(.Items.Count)
			For i = 1 To .Items.Count
				item_list(i) = VB6.GetItemString(lstItems, i - 1)
			Next 
		End With
		
		If cmdSort.Text = "���x�����ɕ��בւ�" Then
			'���C���p�C���b�g�̃��x�����ɕ��בւ�
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
			'���בւ����@���g�O���Ő؂�ւ�
			cmdSort.Text = "���̏��ɕ��בւ�"
		Else
			'���j�b�g�̖��̏��ɕ��בւ�
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
			'���בւ����@���g�O���Ő؂�ւ�
			cmdSort.Text = "���x�����ɕ��בւ�"
		End If
		
		'���X�g�\�����X�V����
		With lstItems
			.Visible = False
			For i = 1 To .Items.Count
				VB6.SetItemString(lstItems, i - 1, item_list(i))
			Next 
			.TopIndex = 0
			.Visible = True
		End With
	End Sub
	
	'�t�H�[����\��
	'UPGRADE_WARNING: Form �C�x���g frmMultiSelectListBox.Activate �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub frmMultiSelectListBox_Activated(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Activated
		SelectedItemNum = 0
		lblNumber.Text = "0/" & VB6.Format(MaxListItem)
		ReDim ItemFlag(lstItems.Items.Count)
		If lstItems.Items.Count > 0 Then
			DisplayUnitStatus(UList.Item(ListItemID(1)))
		End If
	End Sub
	
	'���X�g�{�b�N�X��Ń_�u���N���b�N
	Private Sub lstItems_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstItems.DoubleClick
		Dim i As Short
		
		i = lstItems.SelectedIndex
		If i >= 0 Then
			If ItemFlag(i) Then
				'�I��������
				
				'�I�����ꂽ���j�b�g�������炷
				SelectedItemNum = SelectedItemNum - 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = False
				
				'�I����Ԃ̕\�����X�V
				VB6.SetItemString(lstItems, i, "�@" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'�I���I�����\������
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
				'�I��
				
				'�I�����ꂽ���j�b�g���𑝂₷
				SelectedItemNum = SelectedItemNum + 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = True
				
				'�I����Ԃ̕\�����X�V
				VB6.SetItemString(lstItems, i, "��" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'�I���I�����\������
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
	
	'���X�g�{�b�N�X��Ń}�E�X�{�^��������
	Private Sub lstItems_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim i As Short
		
		'���N���b�N�ȊO�͖���
		If Button <> 1 Then
			Exit Sub
		End If
		
		i = lstItems.SelectedIndex
		If i >= 0 Then
			If ItemFlag(i) Then
				'�I��������
				
				'�I�����ꂽ���j�b�g�������炷
				SelectedItemNum = SelectedItemNum - 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = False
				
				'�I����Ԃ̕\�����X�V
				VB6.SetItemString(lstItems, i, "�@" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'�I���I�����\������
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
				'�I��
				
				'�I�����ꂽ���j�b�g���𑝂₷
				SelectedItemNum = SelectedItemNum + 1
				lblNumber.Text = VB6.Format(SelectedItemNum) & "/" & VB6.Format(MaxListItem)
				ItemFlag(i) = True
				
				'�I����Ԃ̕\�����X�V
				VB6.SetItemString(lstItems, i, "��" & Mid(VB6.GetItemString(lstItems, i), 2))
				
				'�I���I�����\������
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
	
	'���X�g�{�b�N�X��Ń}�E�X�J�[�\�����ړ�
	Private Sub lstItems_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim itm As Short
		
		'�J�[�\��������A�C�e�����Z�o
		itm = ((Y * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width) + 1) \ 16
		itm = itm + lstItems.TopIndex
		
		'�J�[�\��������A�C�e�����n�C���C�g�\��
		If itm < 0 Or itm >= lstItems.Items.Count Then
			lstItems.SelectedIndex = -1
			Exit Sub
		End If
		If lstItems.SelectedIndex = itm Then
			Exit Sub
		End If
		lstItems.SelectedIndex = itm
	End Sub
	
	'�J�[�\�����w�����j�b�g����莞�Ԃ��Ƃɒ��ׂăX�e�[�^�X�E�B���h�E�ɕ\��
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
			'���j�b�g�I�𒆂���
			If CommandState = "���j�b�g�I��" Then
				DisplayUnitStatus(u)
			End If
		End If
	End Sub
End Class