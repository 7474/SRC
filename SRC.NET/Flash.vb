Option Strict Off
Option Explicit On
Module Flash
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Flashファイルの再生
	Public Sub PlayFlash(ByRef fname As String, ByRef fx As Short, ByRef fy As Short, ByRef fw As Short, ByRef fh As Short, ByRef opt As String)
		Dim i As Short
		Dim is_VisibleEnd As Boolean
		
		'Invalid_string_refer_to_original_code
		If Not IsFlashAvailable Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Exit Sub
		End If
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Enable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If Not frmMain.FlashObject.Enable Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Exit Sub
		End If
		
		is_VisibleEnd = False
		
		For i = 1 To LLength(opt)
			Select Case LIndex(opt, i)
				Case "保持"
					is_VisibleEnd = True
			End Select
		Next 
		
		With frmMain.FlashObject
			'一旦非表示
			'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.Visible �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			.Visible = False
			
			'Invalid_string_refer_to_original_code
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
	
	'表示したままのFlashを消去する
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
	
	'Invalid_string_refer_to_original_code
	' Flashのアクションの「GetURL」で
	'　1.「URL」に"FSCommand:"
	'Invalid_string_refer_to_original_code
	'を指定すると、そのアクションが実行されたときに
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub GetEvent(ByVal fpara As String)
		Dim buf As String
		Dim i, j As Short
		Dim funcname, funcpara As String
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		
		'再生を一時停止
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.StopMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		frmMain.FlashObject.StopMovie()
		
		funcname = ""
		funcpara = ""
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If funcname = "" Then
			funcname = ListIndex(fpara, 1)
			buf = ListTail(fpara, 2)
		End If
		'サブルーチンの引数を記録
		For j = 1 To ListLength(buf)
			funcpara = funcpara & ", " & ListIndex(buf, j)
		Next 
		
		'Invalid_string_refer_to_original_code
		buf = "Call(" & funcname & funcpara & ")"
		'Invalid_string_refer_to_original_code
		CallFunction(buf, etype, str_result, num_result)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g frmMain.FlashObject.PlayMovie �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		frmMain.FlashObject.PlayMovie()
	End Sub
End Module