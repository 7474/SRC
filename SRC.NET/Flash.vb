Option Strict Off
Option Explicit On
Module Flash
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'Flash�t�@�C���̍Đ�
	Public Sub PlayFlash(ByRef fname As String, ByRef fx As Short, ByRef fy As Short, ByRef fw As Short, ByRef fh As Short, ByRef opt As String)
		Dim i As Short
		Dim is_VisibleEnd As Boolean
		
		'FLASH���g�p�ł��Ȃ��ꍇ�̓G���[
		If Not IsFlashAvailable Then
			ErrorMessage("Flash�t�@�C���̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCrLf & "�uMacromedia Flash Player�v���C���X�g�[������Ă��܂���B" & vbCrLf & "����URL����A�ŐV�ł�Flash Player���C���X�g�[�����Ă��������B" & vbCrLf & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese")
			Exit Sub
		End If
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Enable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If Not frmMain.FlashObject.Enable Then
			ErrorMessage("Flash�t�@�C���̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCrLf & "�uMacromedia Flash Player�v���C���X�g�[������Ă��܂���B" & vbCrLf & "����URL����A�ŐV�ł�Flash Player���C���X�g�[�����Ă��������B" & vbCrLf & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese")
			Exit Sub
		End If
		
		is_VisibleEnd = False
		
		For i = 1 To LLength(opt)
			Select Case LIndex(opt, i)
				Case "�ێ�"
					is_VisibleEnd = True
			End Select
		Next 
		
		With frmMain.FlashObject
			'��U��\��
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Visible �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Visible = False
			
			'Flash�I�u�W�F�N�g�̈ʒu�E�T�C�Y�ݒ�
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Left �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Left = fx
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Top �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Top = fy
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Width �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Width = fw
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Height �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Height = fh
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Visible �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Visible = True
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.ZOrder �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.ZOrder()
			
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.LoadMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.LoadMovie(ScenarioPath & fname)
			
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Playing �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Do While .Playing And Not IsRButtonPressed(True)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.StopMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			frmMain.FlashObject.StopMovie()
			
			If Not is_VisibleEnd Then
				'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.ClearMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				.ClearMovie()
				'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Visible �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				.Visible = False
			End If
		End With
	End Sub
	
	'�\�������܂܂�Flash����������
	Public Sub ClearFlash()
		If Not IsFlashAvailable Then Exit Sub
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Enable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If Not frmMain.FlashObject.Enable Then Exit Sub
		
		With frmMain.FlashObject
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.ClearMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.ClearMovie()
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Visible �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Visible = False
		End With
	End Sub
	
	'Flash�t�@�C������C�x���g���擾
	' Flash�̃A�N�V�����́uGetURL�v��
	'�@1.�uURL�v��"FSCommand:"
	'�@2.�u�^�[�Q�b�g�v�Ɂu�T�u���[�`���� [����1 [����2 [�c]]�v
	'���w�肷��ƁA���̃A�N�V���������s���ꂽ�Ƃ���
	'�^�[�Q�b�g�̃T�u���[�`�������s�����B
	'�T�u���[�`�������s���Ă���ԁAFlash�̍Đ��͒�~����B
	Public Sub GetEvent(ByVal fpara As String)
		Dim buf As String
		Dim i, j As Short
		Dim funcname, funcpara As String
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		
		'�Đ����ꎞ��~
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.StopMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		frmMain.FlashObject.StopMovie()
		
		funcname = ""
		funcpara = ""
		
		'�O�̂��߂�Flash����n���ꂽ�p�����[�^�S�Ă����
		'��ԍŏ��Ɍ���������������A�Ăяo���T�u���[�`�����Ƃ���
		If funcname = "" Then
			funcname = ListIndex(fpara, 1)
			buf = ListTail(fpara, 2)
		End If
		'�T�u���[�`���̈������L�^
		For j = 1 To ListLength(buf)
			funcpara = funcpara & ", " & ListIndex(buf, j)
		Next 
		
		'�T�u���[�`�����ƈ�������ACall�֐��̌Ăяo���̕�����𐶐�
		buf = "Call(" & funcname & funcpara & ")"
		'���Ƃ��Đ�����������������s
		CallFunction(buf, etype, str_result, num_result)
		
		'�Đ����ĊJ
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.PlayMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		frmMain.FlashObject.PlayMovie()
	End Sub
End Module