Option Strict Off
Option Explicit On
Friend Class frmMultiColumnListBox
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'フォームを表示
	'UPGRADE_WARNING: Form �C�x���g frmMultiColumnListBox.Activate �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub frmMultiColumnListBox_Activated(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Activated
		SelectedItem = 0
		labCaption.Text = ""
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMultiColumnListBox_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim ret As Integer
		
		'常に手前に表示
		ret = SetWindowPos(Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
	End Sub
	
	'フォームを閉じる
	Private Sub frmMultiColumnListBox_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		TopItem = lstItems.TopIndex + 1
		IsFormClicked = True
		If Not IsMordal And Visible Then
			'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
			Cancel = 1
		End If
		Hide()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub lstItems_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Select Case Button
			Case 1
				'Invalid_string_refer_to_original_code
				If Not Visible Then
					Exit Sub
				End If
				If lstItems.SelectedIndex < 0 Or ListItemFlag(lstItems.SelectedIndex + 1) Then
					Exit Sub
				End If
				SelectedItem = lstItems.SelectedIndex + 1
				TopItem = lstItems.TopIndex + 1
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
			Case 2
				'キャンセル
				SelectedItem = 0
				TopItem = lstItems.TopIndex + 1
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
		End Select
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMultiColumnListBox_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		If Button = 2 Then
			'キャンセルのみ受け付け
			SelectedItem = 0
			TopItem = lstItems.TopIndex
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub lstItems_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim itm As Short
		Dim lines As Short
		
		With lstItems
			'Invalid_string_refer_to_original_code
			lines = 25
			'UPGRADE_ISSUE: ListBox �v���p�e�B lstItems.Columns �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			If .Items.Count > lines * .Columns Then
				lines = lines - 1
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ListBox �v���p�e�B lstItems.Columns �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			itm = (((X * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width)) \ (.Width \ .Columns)) * lines
			itm = itm + ((Y * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width) + 1) \ 16
			itm = itm + .TopIndex
			
			'Invalid_string_refer_to_original_code
			If itm < 0 Or itm >= .Items.Count Then
				.SelectedIndex = -1
				Exit Sub
			End If
			If .SelectedIndex = itm Then
				Exit Sub
			End If
			.SelectedIndex = itm
			
			'解説の表示
			labCaption.Text = ListItemComment(itm + 1)
		End With
	End Sub
End Class