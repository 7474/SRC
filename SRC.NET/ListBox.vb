Option Strict Off
Option Explicit On
Friend Class frmListBox
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'���X�g�{�b�N�X�̃t�H�[��
	
	'���X�g�{�b�N�X�̃T�C�Y (�ʏ�:M ��^:L)
	'��
	Public HorizontalSize As String
	'����
	Public VerticalSize As String
	
	'Question�R�}���h�p�ϐ�
	Public CurrentTime As Short
	Public TimeLimit As Short
	
	'�Ō�ɑI�����ꂽ�A�C�e��
	Private LastSelectedItem As Short
	
	'���X�g�{�b�N�X�ւ̃L�[����
	Private Sub frmListBox_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		'���ɃE�B���h�E���B��Ă���ꍇ�͖���
		If Not Visible Then
			Exit Sub
		End If
		
		Select Case KeyCode
			Case System.Windows.Forms.Keys.Up
			Case System.Windows.Forms.Keys.Down
			Case System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.Right
			Case System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.Back
				'�L�����Z��
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
			Case Else
				'�I��
				If lstItems.SelectedIndex < 0 Then
					Exit Sub
				End If
				If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
					If ListItemFlag(lstItems.SelectedIndex + 1) Then
						Exit Sub
					End If
				End If
				SelectedItem = lstItems.SelectedIndex + 1
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
		End Select
	End Sub
	
	'���X�g�{�b�N�X���J��
	Private Sub frmListBox_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim ret As Integer
		
		'���X�g�{�b�N�X����Ɏ�O�ɕ\��
		ret = SetWindowPos(Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
		HorizontalSize = "M"
		VerticalSize = "M"
	End Sub
	
	'���X�g�{�b�N�X�����
	Private Sub frmListBox_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		TopItem = lstItems.TopIndex + 1
		IsFormClicked = True
		If Not IsMordal And Visible Then
			'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
			Cancel = 1
		End If
		Hide()
	End Sub
	
	'���ڂ��_�u���N���b�N
	Private Sub lstItems_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstItems.DoubleClick
		'�����ȃA�C�e�����I������Ă���H
		If lstItems.SelectedIndex < 0 Then
			Exit Sub
		End If
		If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
			If ListItemFlag(lstItems.SelectedIndex + 1) Then
				Exit Sub
			End If
		End If
		
		If LastSelectedItem <> 0 Then
			'�A���őI��
			If Not Visible Then
				Exit Sub
			End If
			SelectedItem = lstItems.SelectedIndex + 1
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex + 1
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		Else
			'�A���ŃL�����Z��
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex + 1
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		End If
	End Sub
	
	'�}�E�X�ŃN���b�N
	Private Sub lstItems_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Select Case Button
			Case 1
				'�I��
				If Not Visible Then
					Exit Sub
				End If
				
				'�����ȃA�C�e�����I������Ă���H
				If lstItems.SelectedIndex < 0 Then
					Exit Sub
				End If
				If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
					If ListItemFlag(lstItems.SelectedIndex + 1) Then
						Exit Sub
					End If
				End If
				
				SelectedItem = lstItems.SelectedIndex + 1
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				IsFormClicked = True
				
				If CurrentTime > 0 Then
					Timer1.Enabled = False
					Hide()
				End If
				
			Case 2
				'�L�����Z��
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				IsFormClicked = True
		End Select
	End Sub
	
	'�L���v�V�����������N���b�N
	Private Sub labCaption_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles labCaption.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Select Case Button
			Case 1
				'���j�b�g�X�e�[�^�X��\�����Ă��郆�j�b�g�����ւ�
				If MainForm.Visible Then
					If Not DisplayedUnit Is Nothing And Not SelectedUnit Is Nothing And Not SelectedTarget Is Nothing Then
						If DisplayedUnit.ID = SelectedUnit.ID Then
							DisplayUnitStatus(SelectedTarget)
						Else
							DisplayUnitStatus(SelectedUnit)
						End If
					End If
				End If
			Case 2
				'�L�����Z��
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
		End Select
	End Sub
	
	'���X�g�{�b�N�X�̒[���N���b�N
	Private Sub frmListBox_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		If Button = 2 Then
			'�L�����Z��
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		End If
	End Sub
	
	'���X�g�{�b�N�X��Ń}�E�X�J�[�\���𓮂���
	Private Sub lstItems_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim itm As Short
		
		'�J�[�\��������A�C�e�����Z�o
		itm = ((Y * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width) + 1) \ 16
		itm = itm + lstItems.TopIndex
		
		'�J�[�\����̃A�C�e�����n�C���C�g�\��
		If itm < 0 Or itm >= lstItems.Items.Count Then
			lstItems.SelectedIndex = -1
			Exit Sub
		End If
		If lstItems.SelectedIndex = itm Then
			Exit Sub
		End If
		lstItems.SelectedIndex = itm
		
		'�R�����g�����X�V
		If txtComment.Enabled Then
			txtComment.Text = ListItemComment(itm + 1)
		End If
	End Sub
	
	'Question�R�}���h�Ή�
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		CurrentTime = CurrentTime + 1
		'UPGRADE_ISSUE: PictureBox ���\�b�h picBar.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		picBar.Cls()
		'UPGRADE_ISSUE: PictureBox ���\�b�h picBar.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		picBar.Line (0, 0) - (picBar.ClientRectangle.Width * CurrentTime \ TimeLimit, picBar.ClientRectangle.Height), BF
		picBar.Refresh()
		
		If CurrentTime >= TimeLimit Then
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex
			Hide()
			Timer1.Enabled = False
		End If
	End Sub
	
	'�I������Ă���A�C�e���ɑΉ����郆�j�b�g�̃X�e�[�^�X�\��
	Private Sub Timer2_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer2.Tick
		Dim u As Unit
		
		If Not Visible Or Not MainForm.Visible Then
			Exit Sub
		End If
		
		If lstItems.SelectedIndex = -1 Then
			Exit Sub
		End If
		
		On Error GoTo ErrorHandler
		
		If lstItems.SelectedIndex >= UBound(ListItemID) Then
			Exit Sub
		End If
		
		'�I�����ꂽ���j�b�g�����݂���H
		If Not UList.IsDefined2(ListItemID(lstItems.SelectedIndex + 1)) Then
			Exit Sub
		End If
		
		u = UList.Item2(ListItemID(lstItems.SelectedIndex + 1))
		
		'�I�����ꂽ���j�b�g�Ƀp�C���b�g������Ă���H
		If u.CountPilot = 0 Then
			Exit Sub
		End If
		
		'���ɕ\�����Ă���H
		If DisplayedUnit Is u Then
			Exit Sub
		End If
		
		'�I�����ꂽ���j�b�g���X�e�[�^�X�E�B���h�E�ɕ\��
		DisplayUnitStatus(u)
		
ErrorHandler: 
	End Sub
End Class