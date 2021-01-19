Option Strict Off
Option Explicit On
Module Susie
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'Susieプラグインを利用して画像ファイルを読み込むためのモジュール
	
	'Susie 32-bit Plug-in API
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Private Declare Function GetPNGPicture Lib "ifpng.spi"  Alias "GetPicture"(ByRef buf As Any, ByVal length As Integer, ByVal flag As Integer, ByRef pHBInfo As Integer, ByRef pHBm As Integer, ByVal lpProgressCallback As Any, ByVal lData As Integer) As Integer
	
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Private Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As Any, ByVal wUsage As Integer) As Integer
	
	Private Declare Function LocalFree Lib "kernel32" (ByVal hMem As Integer) As Integer
	Private Declare Function LocalLock Lib "kernel32" (ByVal hMem As Integer) As Integer
	Private Declare Function LocalUnlock Lib "kernel32" (ByVal hMem As Integer) As Integer
	
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Private Declare Sub MoveMemory Lib "kernel32"  Alias "RtlMoveMemory"(ByRef dest As Any, ByRef Source As Any, ByVal length As Integer)
	
	'画像ファイルを読み込む関数
	Public Function LoadPicture2(ByRef pic As System.Windows.Forms.PictureBox, ByRef fname As String) As Boolean
		Dim HBInfo, HBm As Integer
		Dim lpHBInfo, lpHBm As Integer
		'UPGRADE_WARNING: 構造体 bmi の配列は、使用する前に初期化する必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"' をクリックしてください。
		Dim bmi As BITMAPINFO
		Dim ret As Integer
		
		On Error GoTo ErrorHandler
		
		'画像の取得
		Select Case LCase(Right(fname, 4))
			Case ".bmp", ".jpg", ".gif"
				'Susieプラグインを使わずにロード
				pic.Image = System.Drawing.Image.FromFile(fname)
				LoadPicture2 = True
				Exit Function
			Case ".png"
				'PNGファイル用SusieプラグインAPIを実行
				ret = GetPNGPicture(fname, 0, 0, HBInfo, HBm, 0, 0)
			Case Else
				'未サポートのファイル形式
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の画像フォーマットはサポートされていません。")
				pic.Image = System.Drawing.Image.FromFile("")
				Exit Function
		End Select
		
		'読み込みに成功した？
		If ret <> 0 Then
			ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
			Exit Function
		End If
		
		'メモリのロック
		lpHBInfo = LocalLock(HBInfo)
		lpHBm = LocalLock(HBm)
		
		'なぜか画像を一旦消去しておく必要あり
		pic.Image = System.Drawing.Image.FromFile("")
		
		With pic
			'ピクチャボックスのサイズ変更
			'UPGRADE_WARNING: オブジェクト bmi の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call MoveMemory(bmi, lpHBInfo, Len(bmi))
			.Width = VB6.TwipsToPixelsX(bmi.bmiHeader.biWidth)
			.Height = VB6.TwipsToPixelsY(bmi.bmiHeader.biHeight)
			
			'画像の表示
			'UPGRADE_ISSUE: PictureBox プロパティ pic.Image はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = SetDIBits(.hDC, CInt(CObj(.Image)), 0, VB6.PixelsToTwipsY(.Height), lpHBm, lpHBInfo, 0)
		End With
		
		'メモリのロック解除
		Call LocalUnlock(HBInfo)
		Call LocalUnlock(HBm)
		
		'メモリハンドルの解放
		Call LocalFree(HBInfo)
		Call LocalFree(HBm)
		
		'画像の読み出しに成功したかどうかを返す
		If ret <> 0 Then
			LoadPicture2 = True
		End If
		
		Exit Function
		
ErrorHandler: 
		'エラー処理
		Select Case LCase(Right(fname, 4))
			Case ".bmp", ".jpg", ".gif"
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
			Case ".png"
				ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "PNGファイル用Susie Plug-inがインストールされていません。")
		End Select
	End Function
End Module