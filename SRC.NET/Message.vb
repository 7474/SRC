Option Strict Off
Option Explicit On
Friend Class frmMessage
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'���b�Z�[�W�E�B���h�E�̃t�H�[��
	
	'�t�H�[������N���b�N
	Private Sub frmMessage_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Click
		IsFormClicked = True
	End Sub
	
	'�t�H�[������_�u���N���b�N
	Private Sub frmMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.DoubleClick
		IsFormClicked = True
	End Sub
	
	'�t�H�[����ŃL�[������
	Private Sub frmMessage_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		IsFormClicked = True
	End Sub
	
	'�t�H�[����Ń}�E�X�{�^��������
	Private Sub frmMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
	
	'�t�H�[�������
	Private Sub frmMessage_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		
		'SRC���I�����邩�m�F
		ret = MsgBox("SRC���I�����܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "�I��")
		
		Select Case ret
			Case 1
				'SRC���I��
				Hide()
				TerminateSRC()
			Case 2
				'�I�����L�����Z��
				'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
				Cancel = 1
		End Select
	End Sub
	
	'�p�C���b�g��ʏ�ŃN���b�N
	Private Sub picFace_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picFace.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		'�������b�Z�[�W���胂�[�h�Ɉڍs
		If MessageWait < 10000 Then
			AutoMessageMode = Not AutoMessageMode
		End If
		IsFormClicked = True
	End Sub
	
	'���b�Z�[�W����Ń_�u���N���b�N
	Private Sub picMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMessage.DoubleClick
		IsFormClicked = True
	End Sub
	
	'���b�Z�[�W����Ń}�E�X�{�^��������
	Private Sub picMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMessage.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
End Class