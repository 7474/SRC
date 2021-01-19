Option Strict Off
Option Explicit On
Module Graphics
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Structure BITMAPINFOHEADER
		Dim biSize As Integer 'bmiHeaderã®ã‚µã‚¤ã‚º
		Dim biWidth As Integer 'Invalid_string_refer_to_original_code
		Dim biHeight As Integer 'Invalid_string_refer_to_original_code
		Dim biPlanes As Short 'Invalid_string_refer_to_original_code
		Dim biBitCount As Short 'Invalid_string_refer_to_original_code
		Dim biCompression As Integer 'Invalid_string_refer_to_original_code
		Dim biSizeImage As Integer 'ç”»åƒãƒ‡ãƒ¼ã‚¿ã®ã‚µã‚¤ã‚ºã‚’è¡¨ã™ãƒã‚¤ãƒˆæ•°
		Dim biXPelsPerMeter As Integer 'Invalid_string_refer_to_original_code
		Dim biYPelsPerMeter As Integer 'Invalid_string_refer_to_original_code
		Dim biClrUsed As Integer 'Invalid_string_refer_to_original_code
		Dim biClrImportant As Integer 'Invalid_string_refer_to_original_code
	End Structure
	
	'Invalid_string_refer_to_original_code
	Public Structure RGBQUAD
		Dim rgbBlue As Byte
		Dim rgbGreen As Byte
		Dim rgbRed As Byte
		Dim rgbReserved As Byte
	End Structure
	
	'Invalid_string_refer_to_original_code
	Public Structure BITMAPINFO
		Dim bmiHeader As BITMAPINFOHEADER
		<VBFixedArray(255)> Dim bmiColors() As RGBQUAD
		
		'UPGRADE_TODO: ‚±‚Ì\‘¢‘Ì‚ÌƒCƒ“ƒXƒ^ƒ“ƒX‚ğ‰Šú‰»‚·‚é‚É‚ÍA"Initialize" ‚ğŒÄ‚Ño‚³‚È‚¯‚ê‚Î‚È‚è‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Public Sub Initialize()
			ReDim bmiColors(255)
		End Sub
	End Structure
	
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFO ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function StretchDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal SrcX As Integer, ByVal SrcY As Integer, ByVal wSrcWidth As Integer, ByVal wSrcHeight As Integer, ByVal lpBits As Integer, ByRef lpBitsInfo As BITMAPINFO, ByVal wUsage As Integer, ByVal dwRop As Integer) As Integer
	Declare Function SelectObject Lib "gdi32" (ByVal hDC As Integer, ByVal hObject As Integer) As Integer
	Declare Function DeleteObject Lib "gdi32" (ByVal hObject As Integer) As Integer
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFO ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function CreateDIBSection Lib "gdi32" (ByVal hDC As Integer, ByRef pBitmapInfo As BITMAPINFO, ByVal un As Integer, ByRef lplpVoid As Integer, ByVal handle As Integer, ByVal dw As Integer) As Integer
	Declare Function CreateCompatibleDC Lib "gdi32" (ByVal hDC As Integer) As Integer
	Declare Function BitBlt Lib "gdi32" (ByVal hdest_dc As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hsrc_dc As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal dwRop As Integer) As Integer
	Declare Function DeleteDC Lib "gdi32" (ByVal hDC As Integer) As Integer
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function CreateBitmap Lib "gdi32" (ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal nPlanes As Integer, ByVal nBitCount As Integer, ByRef lpBits As Any) As Integer
	Declare Function SetTextColor Lib "gdi32" (ByVal hDC As Integer, ByVal crColor As Integer) As Integer
	Declare Function SetBkColor Lib "gdi32" (ByVal hDC As Integer, ByVal crColor As Integer) As Integer
	
	Const SRCCOPY As Integer = &HCC0020
	Const DIB_RGB_COLORS As Short = 0
	Const BI_RGB As Short = 0
	
	
	'Invalid_string_refer_to_original_code
	Public Structure Bitmap
		Dim bmType As Integer
		Dim bmWidth As Integer
		Dim bmHeight As Integer
		Dim bmWidthBytes As Integer
		Dim bmPlanes As Short
		Dim bmBitsPixel As Short
		Dim bmBits As Integer
	End Structure
	
	'UPGRADE_NOTE: GetObject ‚Í GetObject_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Declare Function GetObject_Renamed Lib "gdi32"  Alias "GetObjectA"(ByVal hObject As Integer, ByVal nCount As Integer, ByRef lpObject As Any) As Integer
	
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFOHEADER ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFOHEADER ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Declare Function GetDIBits Lib "gdi32" (ByVal aHDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFOHEADER ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'UPGRADE_WARNING: \‘¢‘Ì BITMAPINFOHEADER ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Declare Function CreateDIBitmap Lib "gdi32" (ByVal hDC As Integer, ByRef lpInfoHeader As BITMAPINFOHEADER, ByVal dwUsage As Integer, ByRef lpInitBits As Any, ByRef lpInitInfo As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Public Declare Function GetPixel Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Public Structure RGBq
		Dim Blue As Byte
		Dim Green As Byte
		Dim Red As Byte
	End Structure
	
	Dim PixBuf() As RGBq 'Invalid_string_refer_to_original_code
	Dim PixBuf2() As RGBq 'Invalid_string_refer_to_original_code
	Dim PixWidth As Integer 'Invalid_string_refer_to_original_code
	Dim PixHeight As Integer 'Invalid_string_refer_to_original_code
	Dim PicWidth As Integer 'Invalid_string_refer_to_original_code
	Dim PicHeight As Integer 'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Dim BmpInfo() As BITMAPINFO
	Dim NewDC As Integer
	Dim MemDC As Integer
	Dim OrigPicDC As Integer
	Dim lpBit As Integer
	
	Dim FadeCMap() As Byte
	
	
	'
	'Invalid_string_refer_to_original_code
	'
	Public Sub InitFade(ByRef pic As System.Windows.Forms.PictureBox, ByVal times As Integer, Optional ByVal white_out As Boolean = False)
		Dim g, r, b As Integer
		Dim k, i, j, l As Integer
		Dim tx, ty As Integer
		Dim ret As Integer
		Dim cmap(255) As RGBq
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim rgb_Renamed As Integer
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		i = 0
		For j = 0 To 6
			Select Case j
				Case 0
					g = 0
				Case 1
					g = 40
				Case 2
					g = 95
				Case 3
					g = 130
				Case 4
					g = 180
				Case 5
					g = 220
				Case 6
					g = 255
			End Select
			
			For k = 0 To 6
				Select Case k
					Case 0
						r = 0
					Case 1
						r = 40
					Case 2
						r = 95
					Case 3
						r = 130
					Case 4
						r = 180
					Case 5
						r = 220
					Case 6
						r = 255
				End Select
				
				For l = 0 To 3
					Select Case l
						Case 0
							b = 0
						Case 1
							b = 85
						Case 2
							b = 170
						Case 3
							b = 255
					End Select
					
					With cmap(i)
						.Red = r
						.Green = g
						.Blue = b
					End With
					i = i + 1
				Next 
			Next 
		Next 
		
		'Invalid_string_refer_to_original_code
		With pic
			j = 0
			Do While i <= 220
				tx = Dice(VB6.PixelsToTwipsX(.Width)) - 1
				ty = Dice(VB6.PixelsToTwipsY(.Height)) - 1
				
				'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				rgb_Renamed = GetPixel(.hDC, tx, ty)
				
				If rgb_Renamed <> 0 Then
					r = rgb_Renamed Mod &H100
					rgb_Renamed = rgb_Renamed - r
					g = rgb_Renamed Mod &H10000
					rgb_Renamed = rgb_Renamed - g
					g = g \ &H100
					b = rgb_Renamed \ &H10000
					
					With cmap(i)
						.Red = r
						.Green = g
						.Blue = b
					End With
					i = i + 1
				End If
				
				j = j + 1
				If j > 100 Then
					Exit Do
				End If
			Loop 
		End With
		With pic
			j = 0
			Do While i <= 254
				tx = Dice(VB6.PixelsToTwipsX(.Width)) - 1
				ty = Dice(VB6.PixelsToTwipsY(.Height)) - 1
				
				'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				rgb_Renamed = GetPixel(.hDC, tx, ty)
				
				If rgb_Renamed <> 0 Then
					r = rgb_Renamed Mod &H100
					rgb_Renamed = rgb_Renamed - r
					g = rgb_Renamed Mod &H10000
					rgb_Renamed = rgb_Renamed - g
					g = g \ &H100
					b = rgb_Renamed \ &H10000
					
					With cmap(i)
						.Red = r
						.Green = g
						.Blue = b
					End With
					i = i + 1
				End If
				
				j = j + 1
				If j > 100 Then
					Exit Do
				End If
			Loop 
		End With
		
		rgb_Renamed = ObjColor
		
		r = rgb_Renamed Mod &H100
		rgb_Renamed = rgb_Renamed - r
		g = rgb_Renamed Mod &H10000
		rgb_Renamed = rgb_Renamed - g
		g = g \ &H100
		b = rgb_Renamed \ &H10000
		
		With cmap(i)
			.Red = r
			.Green = g
			.Blue = b
		End With
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ”z—ñ BmpInfo ‚ÅŠe—v‘f‚ğ‰Šú‰»‚·‚é•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReDim BmpInfo(times)
		For i = 0 To times
			With BmpInfo(i).bmiHeader
				.biSize = Len(BmpInfo(i).bmiHeader)
				.biWidth = VB6.PixelsToTwipsX(pic.Width)
				.biHeight = VB6.PixelsToTwipsY(pic.Height)
				.biPlanes = 1
				.biBitCount = 8
				.biCompression = BI_RGB
				.biSizeImage = 0
				.biXPelsPerMeter = 0
				.biYPelsPerMeter = 0
				.biClrUsed = 0
				.biClrImportant = 0
			End With
			
			'Invalid_string_refer_to_original_code
			If white_out Then
				For j = 0 To 255
					With cmap(j)
						r = .Red
						g = .Green
						b = .Blue
					End With
					With BmpInfo(i).bmiColors(j)
						.rgbBlue = r + (255 - r) * (times - i) \ times
						.rgbGreen = g + (255 - g) * (times - i) \ times
						.rgbRed = b + (255 - b) * (times - i) \ times
					End With
				Next 
			Else
				For j = 0 To 255
					With cmap(j)
						r = .Red
						g = .Green
						b = .Blue
					End With
					With BmpInfo(i).bmiColors(j)
						.rgbBlue = r * i \ times
						.rgbGreen = g * i \ times
						.rgbRed = b * i \ times
					End With
				Next 
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Form ƒvƒƒpƒeƒB MainForm.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		NewDC = CreateDIBSection(MainForm.hDC, BmpInfo(times), DIB_RGB_COLORS, lpBit, 0, 0)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		MemDC = CreateCompatibleDC(pic.hDC)
		
		'Invalid_string_refer_to_original_code
		OrigPicDC = SelectObject(MemDC, NewDC)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ret = BitBlt(MemDC, 0, 0, VB6.PixelsToTwipsX(pic.Width), VB6.PixelsToTwipsY(pic.Height), pic.hDC, 0, 0, SRCCOPY)
		
	End Sub
	
	Public Sub DoFade(ByRef pic As System.Windows.Forms.PictureBox, ByVal times As Integer)
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If times < 0 Or UBound(BmpInfo) < times Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		With pic
			'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			ret = StretchDIBits(.hDC, 0, 0, VB6.PixelsToTwipsX(.Width), VB6.PixelsToTwipsY(.Height), 0, 0, VB6.PixelsToTwipsX(.Width), VB6.PixelsToTwipsY(.Height), lpBit, BmpInfo(times), DIB_RGB_COLORS, SRCCOPY)
		End With
	End Sub
	
	Public Sub FinishFade()
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		ret = SelectObject(MemDC, OrigPicDC)
		'Invalid_string_refer_to_original_code
		ret = DeleteDC(MemDC)
		'Invalid_string_refer_to_original_code
		ret = DeleteObject(NewDC)
	End Sub
	
	
	'
	'Invalid_string_refer_to_original_code
	'
	Public Sub MakeMask(ByRef src_dc As Integer, ByRef dest_dc As Integer, ByRef w As Integer, ByRef h As Integer, ByRef tcolor As Integer)
		Dim mask_dc As Integer
		Dim mask_bmp, orig_mask_bmp As Integer
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		mask_dc = CreateCompatibleDC(src_dc)
		'Invalid_string_refer_to_original_code
		mask_bmp = CreateBitmap(w, h, 1, 1, 0)
		'Invalid_string_refer_to_original_code
		orig_mask_bmp = SelectObject(mask_dc, mask_bmp)
		
		'Invalid_string_refer_to_original_code
		ret = SetBkColor(src_dc, tcolor)
		
		ret = BitBlt(mask_dc, 0, 0, w, h, src_dc, 0, 0, SRCCOPY)
		
		'Invalid_string_refer_to_original_code
		If tcolor <> System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
			ret = SetBkColor(dest_dc, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
		End If
		
		ret = BitBlt(dest_dc, 0, 0, w, h, mask_dc, 0, 0, SRCCOPY)
		
		'Invalid_string_refer_to_original_code
		ret = SelectObject(mask_dc, orig_mask_bmp)
		'Invalid_string_refer_to_original_code
		ret = DeleteDC(mask_dc)
		'Invalid_string_refer_to_original_code
		ret = DeleteObject(mask_bmp)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub GetImage(ByRef pic As System.Windows.Forms.PictureBox)
		Dim pic_bmp, tmp_bmp As Integer
		Dim bm_info As BITMAPINFOHEADER
		Dim ret As Integer
		Dim mem_dc As Integer
		Dim bmp As Bitmap
		
		With pic
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg bmp ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.Image ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			ret = GetObject_Renamed(CInt(CObj(.Image)), 24, bmp)
			PixWidth = bmp.bmWidth
			PixHeight = bmp.bmHeight
			PicWidth = VB6.PixelsToTwipsX(.Width)
			PicHeight = VB6.PixelsToTwipsY(.Height)
			'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			mem_dc = .hDC
		End With
		
		With bm_info
			.biBitCount = 24
			.biClrUsed = 0
			.biCompression = 0
			.biHeight = -PixHeight
			.biWidth = PixWidth
			.biPlanes = 1
			.biSize = 40
			.biSizeImage = .biWidth * .biHeight * 3
		End With
		
		tmp_bmp = CreateDIBitmap(mem_dc, bm_info, 0, 0, bm_info, 0)
		pic_bmp = SelectObject(mem_dc, tmp_bmp)
		
		ReDim PixBuf(PixWidth * PixHeight)
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf() ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ret = GetDIBits(mem_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
		tmp_bmp = SelectObject(mem_dc, pic_bmp)
		
		ret = DeleteObject(tmp_bmp)
	End Sub
	
	'PixBufã®ç”»åƒã‚¤ãƒ¡ãƒ¼ã‚¸ã‚’picã«æãè¾¼ã‚€
	Public Sub SetImage(ByRef pic As System.Windows.Forms.PictureBox)
		Dim pic_bmp, tmp_bmp As Integer
		Dim bm_info As BITMAPINFOHEADER
		Dim pic_dc As Integer
		Dim ret As Integer
		
		'UPGRADE_ISSUE: PictureBox ƒvƒƒpƒeƒB pic.hDC ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		pic_dc = pic.hDC
		With bm_info
			.biBitCount = 24
			.biClrUsed = 0
			.biCompression = 0
			.biHeight = -PixHeight
			.biWidth = PixWidth
			.biPlanes = 1
			.biSize = 40
			.biSizeImage = .biWidth * .biHeight * 3
		End With
		
		tmp_bmp = CreateDIBitmap(pic_dc, bm_info, 0, 0, bm_info, 0)
		pic_bmp = SelectObject(pic_dc, tmp_bmp)
		
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf() ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ret = SetDIBits(pic_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
		tmp_bmp = SelectObject(pic_dc, pic_bmp)
		
		ret = DeleteObject(tmp_bmp)
		pic.Refresh()
		'ReDim PixBuf(0)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ClearImage()
		If UBound(PixBuf) > 0 Then
			ReDim PixBuf(0)
		End If
		ReDim PixBuf2(0)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CopyImage()
		Dim i As Integer
		
		ReDim PixBuf2(PixWidth * PixHeight)
		For i = 0 To PixWidth * PixHeight - 1
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf2(i) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			PixBuf2(i) = PixBuf(i)
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CreateMask(ByRef tcolor As Integer)
		Dim j, i, k As Integer
		
		For i = 0 To PicHeight - 1
			For j = 0 To PicWidth - 1
				With PixBuf(k)
					If tcolor = 256 * (256# * .Blue + .Green) + .Red Then
						.Green = 255
						.Blue = 255
						.Red = 255
					Else
						.Green = 0
						.Blue = 0
						.Red = 0
					End If
				End With
				k = k + 1
			Next j
		Next i
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CreateImage(ByRef tcolor As Integer)
		Dim j, i, k As Integer
		
		For i = 0 To PicHeight - 1
			For j = 0 To PicWidth - 1
				With PixBuf(k)
					If tcolor = 256 * (256# * .Blue + .Green) + .Red Then
						.Green = 0
						.Blue = 0
						.Red = 0
					End If
				End With
				k = k + 1
			Next j
		Next i
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub FadeInit(ByVal num As Integer)
		Dim i, j As Short
		
		ReDim FadeCMap(num, 255)
		
		For i = 1 To num
			For j = 0 To 255
				FadeCMap(i, j) = j * i \ num
			Next 
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub FadeInOut(ByVal ind As Integer, ByVal num As Integer)
		Dim g, i, r, b As Integer
		
		For i = 0 To PixWidth * PixHeight - 1
			With PixBuf2(i)
				r = .Red
				g = .Green
				b = .Blue
			End With
			With PixBuf(i)
				.Red = FadeCMap(ind, r)
				.Green = FadeCMap(ind, g)
				.Blue = FadeCMap(ind, b)
			End With
			'        With PixBuf(i)
			'            .Red = r * ind \ num
			'            .Green = g * ind \ num
			'            .Blue = b * ind \ num
			'        End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub WhiteInOut(ByVal ind As Integer, ByVal num As Integer)
		Dim g, i, r, b As Integer
		
		For i = 0 To PixWidth * PixHeight - 1
			With PixBuf2(i)
				r = .Red
				g = .Green
				b = .Blue
			End With
			With PixBuf(i)
				.Red = r + (255 - r) * (num - ind) \ num
				.Green = g + (255 - g) * (num - ind) \ num
				.Blue = b + (255 - b) * (num - ind) \ num
			End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Bright(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						If .Red <> 255 Then
							.Red = MinLng(.Red + 80, 254)
						End If
						If .Green <> 255 Then
							.Green = MinLng(.Green + 80, 254)
						End If
						If .Blue <> 255 Then
							.Blue = MinLng(.Blue + 80, 254)
						End If
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					.Red = MinLng(.Red + 80, 255)
					.Green = MinLng(.Green + 80, 255)
					.Blue = MinLng(.Blue + 80, 255)
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’æš—ã
	Public Sub Dark(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						.Red = .Red \ 2
						.Green = .Green \ 2
						.Blue = .Blue \ 2
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					.Red = .Red \ 2
					.Green = .Green \ 2
					.Blue = .Blue \ 2
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’ç™½é»’ã«
	Public Sub Monotone(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
						.Red = rgb_Renamed
						.Green = rgb_Renamed
						.Blue = rgb_Renamed
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
					.Red = rgb_Renamed
					.Green = rgb_Renamed
					.Blue = rgb_Renamed
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’ã‚»ãƒ”ã‚¢è‰²ã«
	Public Sub Sepia(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
						.Red = MinLng(1.1 * rgb_Renamed, 255)
						.Green = 0.9 * rgb_Renamed
						.Blue = 0.7 * rgb_Renamed
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
					.Red = MinLng(1.1 * rgb_Renamed, 255)
					.Green = 0.9 * rgb_Renamed
					.Blue = 0.7 * rgb_Renamed
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’å¤•ç„¼ã‘é¢¨ã«
	Public Sub Sunset(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
						.Red = MinLng(0.2 * .Red + 1.3 * rgb_Renamed, 255)
						.Green = 0.2 * .Green + 0.4 * rgb_Renamed
						.Blue = 0.2 * .Blue + 0.2 * rgb_Renamed
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
					.Red = MinLng(0.2 * .Red + 1.3 * rgb_Renamed, 255)
					.Green = 0.2 * .Green + 0.4 * rgb_Renamed
					.Blue = 0.2 * .Blue + 0.2 * rgb_Renamed
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’æ°´ä¸­é¢¨ã«
	Public Sub Water(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
						.Red = 0.6 * rgb_Renamed
						.Green = 0.8 * rgb_Renamed
						.Blue = rgb_Renamed
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					rgb_Renamed = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
					.Red = 0.6 * rgb_Renamed
					.Green = 0.8 * rgb_Renamed
					.Blue = rgb_Renamed
				End With
			Next 
		End If
	End Sub
	
	'ç”»åƒã‚’å·¦å³åè»¢
	Public Sub HReverse()
		Dim i, j As Integer
		Dim tmp As RGBq
		
		For i = 0 To PicHeight - 1
			For j = 0 To PicWidth \ 2 - 1
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg tmp ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				tmp = PixBuf(PicWidth * i + j)
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				PixBuf(PicWidth * i + j) = PixBuf(PicWidth * i + PicWidth - j - 1)
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + PicWidth - j - 1) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				PixBuf(PicWidth * i + PicWidth - j - 1) = tmp
			Next j
		Next i
	End Sub
	
	'ç”»åƒã‚’ä¸Šä¸‹åè»¢
	Public Sub VReverse()
		Dim i, j As Integer
		Dim tmp As RGBq
		
		For i = 0 To PicHeight \ 2 - 1
			For j = 0 To PicWidth - 1
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg tmp ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				tmp = PixBuf(PicWidth * i + j)
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				PixBuf(PicWidth * i + j) = PixBuf(PicWidth * (PicHeight - i - 1) + j)
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * PicHeight - i - 1 + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				PixBuf(PicWidth * (PicHeight - i - 1) + j) = tmp
			Next j
		Next i
	End Sub
	
	'ç”»åƒã‚’ãƒã‚¬ãƒã‚¸åè»¢
	Public Sub NegPosReverse(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					If r <> .Red Or g <> .Green Or b <> .Blue Then
						.Red = 255 - .Red
						.Green = 255 - .Green
						.Blue = 255 - .Blue
					End If
				End With
			Next 
		Else
			For i = 0 To PixWidth * PixHeight - 1
				With PixBuf(i)
					.Red = 255 - .Red
					.Green = 255 - .Green
					.Blue = 255 - .Blue
				End With
			Next 
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Silhouette()
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		'Invalid_string_refer_to_original_code
		rgb_Renamed = BGColor
		r = rgb_Renamed Mod &H100
		rgb_Renamed = rgb_Renamed - r
		g = rgb_Renamed Mod &H10000
		rgb_Renamed = rgb_Renamed - g
		g = g \ &H100
		b = rgb_Renamed \ &H10000
		
		For i = 0 To PixWidth * PixHeight - 1
			With PixBuf(i)
				If r = .Red And g = .Green And b = .Blue Then
					.Red = 255
					.Green = 255
					.Blue = 255
				Else
					.Red = 0
					.Green = 0
					.Blue = 0
				End If
			End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub Rotate(ByVal angle As Integer, Optional ByVal do_same As Boolean = False)
		Dim i, j As Integer
		Dim xsrc, ysrc As Integer
		Dim xsrc0, ysrc0 As Double
		Dim xbase, ybase As Double
		Dim xoffset, yoffset As Double
		Dim dsin, rad, dcos As Double
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim bg As RGBq
		Dim g, rgb_Renamed, r, b As Integer
		
		'360åº¦ã§ä¸€å›è»¢
		angle = angle Mod 360
		'Invalid_string_refer_to_original_code
		If angle < 0 Then
			angle = 360 + angle
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If Not do_same Then
			Select Case angle
				Case 0
					Exit Sub
				Case 90
					If PicWidth = PicHeight Then
						CopyImage()
						For i = 0 To PicHeight - 1
							For j = 0 To PicWidth - 1
								'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * (PicWidth - j - 1) + i)
							Next 
						Next 
						Exit Sub
					End If
				Case 180
					CopyImage()
					For i = 0 To PicHeight - 1
						For j = 0 To PicWidth - 1
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * (PicHeight - i - 1) + PicWidth - j - 1)
						Next 
					Next 
					Exit Sub
				Case 270
					If PicWidth = PicHeight Then
						CopyImage()
						For i = 0 To PicHeight - 1
							For j = 0 To PicWidth - 1
								'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * j + PicHeight - i - 1)
							Next 
						Next 
						Exit Sub
					End If
			End Select
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		xbase = (PicWidth - 1) / 2
		ybase = (PicHeight - 1) / 2
		
		'å›è»¢ã«ã‚ˆã‚‹ãƒ™ã‚¯ãƒˆãƒ«
		angle = 90 - angle
		rad = CDbl(angle) * 3.14159265 / 180
		dsin = System.Math.Sin(rad)
		dcos = System.Math.Cos(rad)
		
		'Invalid_string_refer_to_original_code
		rgb_Renamed = BGColor
		r = rgb_Renamed Mod &H100
		rgb_Renamed = rgb_Renamed - r
		g = rgb_Renamed Mod &H10000
		rgb_Renamed = rgb_Renamed - g
		g = g \ &H100
		b = rgb_Renamed \ &H10000
		'Invalid_string_refer_to_original_code
		With bg
			.Red = r
			.Green = g
			.Blue = b
		End With
		
		'Invalid_string_refer_to_original_code
		CopyImage()
		
		'Invalid_string_refer_to_original_code
		For i = 0 To PicHeight - 1
			yoffset = i - ybase
			xsrc0 = xbase + yoffset * dcos
			ysrc0 = ybase + yoffset * dsin
			
			For j = 0 To PicWidth - 1
				xoffset = j - xbase
				
				'Invalid_string_refer_to_original_code
				'xsrc = xbase + xoffset * dsin + yoffset * dcos
				'ysrc = ybase - xoffset * dcos + yoffset * dsin
				xsrc = CInt(xsrc0 + xoffset * dsin)
				ysrc = CInt(ysrc0 - xoffset * dcos)
				
				If xsrc < 0 Or PicWidth <= xsrc Or ysrc < 0 Or PicHeight <= ysrc Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					PixBuf(PicWidth * i + j) = bg
				Else
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg PixBuf(PicWidth * i + j) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * ysrc + xsrc)
				End If
			Next 
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ColorFilter(ByRef fcolor As Integer, ByRef trans_par As Double, Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb ‚Í rgb_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		Dim g2, r2, b2 As Byte
		Dim tratio As Integer
		
		'Invalid_string_refer_to_original_code
		tratio = MinLng(MaxLng(100 * trans_par, 0), 100)
		
		If tratio = 0 Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		rgb_Renamed = fcolor
		r = rgb_Renamed Mod &H100
		rgb_Renamed = rgb_Renamed - r
		g = rgb_Renamed Mod &H10000
		rgb_Renamed = rgb_Renamed - g
		g = g \ &H100
		b = rgb_Renamed \ &H10000
		r2 = r
		g2 = g
		b2 = b
		
		If is_transparent Then
			'Invalid_string_refer_to_original_code
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If r = r2 And g = g2 And b = b2 Then
				If r2 = 255 Then
					r2 = 254
				ElseIf g2 = 255 Then 
					g2 = 254
				ElseIf b2 = 255 Then 
					b2 = 254
				End If
			End If
			
			If trans_par = 100 Then
				For i = 0 To PixWidth * PixHeight - 1
					With PixBuf(i)
						If r <> .Red Or g <> .Green Or b <> .Blue Then
							.Red = r2
							.Green = g2
							.Blue = b2
						End If
					End With
				Next 
			Else
				For i = 0 To PixWidth * PixHeight - 1
					With PixBuf(i)
						If r <> .Red Or g <> .Green Or b <> .Blue Then
							.Red = (.Red * (100 - tratio) + r2 * tratio) \ 100
							.Green = (.Green * (100 - tratio) + g2 * tratio) \ 100
							.Blue = (.Blue * (100 - tratio) + b2 * tratio) \ 100
						End If
					End With
				Next 
			End If
		Else
			If trans_par = 100 Then
				For i = 0 To PixWidth * PixHeight - 1
					With PixBuf(i)
						.Red = r2
						.Green = g2
						.Blue = b2
					End With
				Next 
			Else
				For i = 0 To PixWidth * PixHeight - 1
					With PixBuf(i)
						.Red = (.Red * (100 - tratio) + r2 * tratio) \ 100
						.Green = (.Green * (100 - tratio) + g2 * tratio) \ 100
						.Blue = (.Blue * (100 - tratio) + b2 * tratio) \ 100
					End With
				Next 
			End If
		End If
	End Sub
End Module