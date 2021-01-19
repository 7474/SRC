Option Strict Off
Option Explicit On
Friend Class frmToolTip
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�c�[���`�b�v�p�t�H�[��
	
	'�t�H�[�������[�h
	Private Sub frmToolTip_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim ret As Integer
		
		'��Ɏ�O�ɕ\��
		ret = SetWindowPos(Me.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
	End Sub
	
	'�c�[���`�b�v��\��
	Public Sub ShowToolTip(ByRef msg As String)
		Dim ret As Integer
		Dim PT As POINTAPI
		Dim tw As Short
		Static cur_msg As String
		
		tw = VB6.TwipsPerPixelX
		
		If msg <> cur_msg Then
			cur_msg = msg
			With Me.picMessage
				'���b�Z�[�W���ɃT�C�Y�����킹��
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.TextWidth �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				.Width = VB6.TwipsToPixelsX((.TextWidth(msg) + 6) * tw)
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.TextHeight �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				.Height = VB6.TwipsToPixelsY((.TextHeight(msg) + 4) * tw)
				Me.Width = .Width
				Me.Height = .Height
				
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				.Cls()
				
				.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(200, 200, 200))
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				frmToolTip.picMessage.Line (0, 0) - (VB6.PixelsToTwipsX(.Width) \ tw, 0)
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				frmToolTip.picMessage.Line (0, 0) - (0, VB6.PixelsToTwipsX(.Width) \ tw)
				.ForeColor = System.Drawing.Color.Black
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				frmToolTip.picMessage.Line (0, (VB6.PixelsToTwipsY(.Height) - 1) \ tw) - (VB6.PixelsToTwipsX(.Width) \ tw, (VB6.PixelsToTwipsY(.Height) - 1) \ tw)
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				frmToolTip.picMessage.Line ((VB6.PixelsToTwipsX(.Width) - 1) \ tw, 0) - ((VB6.PixelsToTwipsX(.Width) - 1) \ tw, VB6.PixelsToTwipsY(.Height) \ tw)
				
				'���b�Z�[�W����������
				'UPGRADE_ISSUE: PictureBox �v���p�e�B picMessage.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				.CurrentX = 3
				'UPGRADE_ISSUE: PictureBox �v���p�e�B picMessage.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				.CurrentY = 2
				'UPGRADE_ISSUE: PictureBox ���\�b�h picMessage.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				frmToolTip.picMessage.Print(msg)
				
				.ForeColor = System.Drawing.Color.White
				.Refresh()
			End With
		End If
		
		'�t�H�[���̈ʒu��ݒ�
		ret = GetCursorPos(PT)
		Me.Left = VB6.TwipsToPixelsX(PT.X * tw + 0)
		Me.Top = VB6.TwipsToPixelsY((PT.Y + 24) * tw)
		
		'�t�H�[�����A�N�e�B�u�ŕ\��
		ret = ShowWindow(Me.Handle.ToInt32, SW_SHOWNA)
	End Sub
End Class