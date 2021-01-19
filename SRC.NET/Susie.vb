Option Strict Off
Option Explicit On
Module Susie
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Susieプラグインを利用して画像ファイルを読み込むためのモジュール
	
	'Susie 32-bit Plug-in API
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	Private Declare Function GetPNGPicture Lib "ifpng.spi"  Alias "GetPicture"(ByRef buf As Any, ByVal length As Integer, ByVal flag As Integer, ByRef pHBInfo As Integer, ByRef pHBm As Integer, ByVal lpProgressCallback As Any, ByVal lData As Integer) As Integer
	
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	Private Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As Any, ByVal wUsage As Integer) As Integer
	
	Private Declare Function LocalFree Lib "kernel32" (ByVal hMem As Integer) As Integer
	Private Declare Function LocalLock Lib "kernel32" (ByVal hMem As Integer) As Integer
	Private Declare Function LocalUnlock Lib "kernel32" (ByVal hMem As Integer) As Integer
	
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	'UPGRADE_ISSUE: �p�����[�^ 'As Any' �̐錾�̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ���N���b�N���Ă��������B
	Private Declare Sub MoveMemory Lib "kernel32"  Alias "RtlMoveMemory"(ByRef dest As Any, ByRef Source As Any, ByVal length As Integer)
	
	'画像ファイルを読み込む関数
	Public Function LoadPicture2(ByRef pic As System.Windows.Forms.PictureBox, ByRef fname As String) As Boolean
		Dim HBInfo, HBm As Integer
		Dim lpHBInfo, lpHBm As Integer
		'UPGRADE_WARNING: �\���� bmi �̔z��́A�g�p����O�ɏ���������K�v������܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"' ���N���b�N���Ă��������B
		Dim bmi As BITMAPINFO
		Dim ret As Integer
		
		On Error GoTo ErrorHandler
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(Right(fname, 4))
			Case ".bmp", ".jpg", ".gif"
				'Invalid_string_refer_to_original_code
				pic.Image = System.Drawing.Image.FromFile(fname)
				LoadPicture2 = True
				Exit Function
			Case ".png"
				'Invalid_string_refer_to_original_code
				ret = GetPNGPicture(fname, 0, 0, HBInfo, HBm, 0, 0)
			Case Else
				'Invalid_string_refer_to_original_code
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
				pic.Image = System.Drawing.Image.FromFile("")
				Exit Function
		End Select
		
		'Invalid_string_refer_to_original_code
		If ret <> 0 Then
			ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		lpHBInfo = LocalLock(HBInfo)
		lpHBm = LocalLock(HBm)
		
		'Invalid_string_refer_to_original_code
		pic.Image = System.Drawing.Image.FromFile("")
		
		With pic
			'ピクチャボックスのサイズ変更
			'UPGRADE_WARNING: �I�u�W�F�N�g bmi �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call MoveMemory(bmi, lpHBInfo, Len(bmi))
			.Width = VB6.TwipsToPixelsX(bmi.bmiHeader.biWidth)
			.Height = VB6.TwipsToPixelsY(bmi.bmiHeader.biHeight)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.Image �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.hDC �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			ret = SetDIBits(.hDC, CInt(CObj(.Image)), 0, VB6.PixelsToTwipsY(.Height), lpHBm, lpHBInfo, 0)
		End With
		
		'Invalid_string_refer_to_original_code
		Call LocalUnlock(HBInfo)
		Call LocalUnlock(HBm)
		
		'メモリハンドルの解放
		Call LocalFree(HBInfo)
		Call LocalFree(HBm)
		
		'Invalid_string_refer_to_original_code
		If ret <> 0 Then
			LoadPicture2 = True
		End If
		
		Exit Function
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		Select Case LCase(Right(fname, 4))
			Case ".bmp", ".jpg", ".gif"
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case ".png"
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		End Select
	End Function
End Module