Option Strict Off
Option Explicit On
Friend Class frmMessage
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMessage_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Click
		IsFormClicked = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.DoubleClick
		IsFormClicked = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMessage_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		IsFormClicked = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
	
	'フォームを閉じる
	Private Sub frmMessage_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		Select Case ret
			Case 1
				'Invalid_string_refer_to_original_code
				Hide()
				TerminateSRC()
			Case 2
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
				Cancel = 1
		End Select
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picFace_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picFace.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		'Invalid_string_refer_to_original_code
		If MessageWait < 10000 Then
			AutoMessageMode = Not AutoMessageMode
		End If
		IsFormClicked = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMessage.DoubleClick
		IsFormClicked = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMessage.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
End Class