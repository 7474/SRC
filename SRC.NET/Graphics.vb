Option Strict Off
Option Explicit On
Module Graphics
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'画像処理を行うモジュール
	
	' BITMAPINFO構造体
	Public Structure BITMAPINFOHEADER
		Dim biSize As Integer 'bmiHeaderのサイズ
		Dim biWidth As Integer 'ビットマップの幅を表すピクセル数
		Dim biHeight As Integer 'ビットマップの高さを表すピクセル数
		Dim biPlanes As Short '常に１
		Dim biBitCount As Short 'ピクセルあたりのビット数
		Dim biCompression As Integer '圧縮の種類
		Dim biSizeImage As Integer '画像データのサイズを表すバイト数
		Dim biXPelsPerMeter As Integer '水平方向の解像度を表すメートルあたりのピクセル数
		Dim biYPelsPerMeter As Integer '垂直方向の解像度を表すメートルあたりのピクセル数
		Dim biClrUsed As Integer 'ビットマップが実際に使用する色の数
		Dim biClrImportant As Integer '重要な色の数(0の場合はすべての色が重要)
	End Structure
	
	' パレットエントリ構造体
	Public Structure RGBQUAD
		Dim rgbBlue As Byte
		Dim rgbGreen As Byte
		Dim rgbRed As Byte
		Dim rgbReserved As Byte
	End Structure
	
	' ビットマップ情報
	Public Structure BITMAPINFO
		Dim bmiHeader As BITMAPINFOHEADER
		<VBFixedArray(255)> Dim bmiColors() As RGBQUAD
		
		'UPGRADE_TODO: この構造体のインスタンスを初期化するには、"Initialize" を呼び出さなければなりません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"' をクリックしてください。
		Public Sub Initialize()
			ReDim bmiColors(255)
		End Sub
	End Structure
	
	'UPGRADE_WARNING: 構造体 BITMAPINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function StretchDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal SrcX As Integer, ByVal SrcY As Integer, ByVal wSrcWidth As Integer, ByVal wSrcHeight As Integer, ByVal lpBits As Integer, ByRef lpBitsInfo As BITMAPINFO, ByVal wUsage As Integer, ByVal dwRop As Integer) As Integer
	Declare Function SelectObject Lib "gdi32" (ByVal hDC As Integer, ByVal hObject As Integer) As Integer
	Declare Function DeleteObject Lib "gdi32" (ByVal hObject As Integer) As Integer
	'UPGRADE_WARNING: 構造体 BITMAPINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function CreateDIBSection Lib "gdi32" (ByVal hDC As Integer, ByRef pBitmapInfo As BITMAPINFO, ByVal un As Integer, ByRef lplpVoid As Integer, ByVal handle As Integer, ByVal dw As Integer) As Integer
	Declare Function CreateCompatibleDC Lib "gdi32" (ByVal hDC As Integer) As Integer
	Declare Function BitBlt Lib "gdi32" (ByVal hdest_dc As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hsrc_dc As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal dwRop As Integer) As Integer
	Declare Function DeleteDC Lib "gdi32" (ByVal hDC As Integer) As Integer
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Declare Function CreateBitmap Lib "gdi32" (ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal nPlanes As Integer, ByVal nBitCount As Integer, ByRef lpBits As Any) As Integer
	Declare Function SetTextColor Lib "gdi32" (ByVal hDC As Integer, ByVal crColor As Integer) As Integer
	Declare Function SetBkColor Lib "gdi32" (ByVal hDC As Integer, ByVal crColor As Integer) As Integer
	
	Const SRCCOPY As Integer = &HCC0020
	Const DIB_RGB_COLORS As Short = 0
	Const BI_RGB As Short = 0
	
	
	' ビットマップ構造体
	Public Structure Bitmap
		Dim bmType As Integer
		Dim bmWidth As Integer
		Dim bmHeight As Integer
		Dim bmWidthBytes As Integer
		Dim bmPlanes As Short
		Dim bmBitsPixel As Short
		Dim bmBits As Integer
	End Structure
	
	'UPGRADE_NOTE: GetObject は GetObject_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Public Declare Function GetObject_Renamed Lib "gdi32"  Alias "GetObjectA"(ByVal hObject As Integer, ByVal nCount As Integer, ByRef lpObject As Any) As Integer
	
	'UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Public Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	'UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Public Declare Function GetDIBits Lib "gdi32" (ByVal aHDC As Integer, ByVal hBitmap As Integer, ByVal nStartScan As Integer, ByVal nNumScans As Integer, ByRef lpBits As Any, ByRef lpBI As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	'UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	'UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Public Declare Function CreateDIBitmap Lib "gdi32" (ByVal hDC As Integer, ByRef lpInfoHeader As BITMAPINFOHEADER, ByVal dwUsage As Integer, ByRef lpInitBits As Any, ByRef lpInitInfo As BITMAPINFOHEADER, ByVal wUsage As Integer) As Integer
	
	'指定位置のピクセル色を取得する
	Public Declare Function GetPixel Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer) As Integer
	
	' RGB構造体
	Public Structure RGBq
		Dim Blue As Byte
		Dim Green As Byte
		Dim Red As Byte
	End Structure
	
	Dim PixBuf() As RGBq 'ピクセルの色情報配列
	Dim PixBuf2() As RGBq 'ピクセルの色情報配列
	Dim PixWidth As Integer 'バッファの内容の幅
	Dim PixHeight As Integer 'バッファの内容の高さ
	Dim PicWidth As Integer '画像の幅
	Dim PicHeight As Integer '画像の高さ
	
	'フェードイン＆アウト用変数
	Dim BmpInfo() As BITMAPINFO
	Dim NewDC As Integer
	Dim MemDC As Integer
	Dim OrigPicDC As Integer
	Dim lpBit As Integer
	
	Dim FadeCMap() As Byte
	
	
	'
	'フェードイン＆フェードアウト
	'
	Public Sub InitFade(ByRef pic As System.Windows.Forms.PictureBox, ByVal times As Integer, Optional ByVal white_out As Boolean = False)
		Dim g, r, b As Integer
		Dim k, i, j, l As Integer
		Dim tx, ty As Integer
		Dim ret As Integer
		Dim cmap(255) As RGBq
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim rgb_Renamed As Integer
		
		'フェード処理は画像を256色に変換して行う
		'このための256色のカラーマップを作成する
		
		'まずは決め打ちで0～195番の色を作成
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
		
		'196～255番の色は元画像の色をサンプリングして作成
		With pic
			j = 0
			Do While i <= 220
				tx = Dice(VB6.PixelsToTwipsX(.Width)) - 1
				ty = Dice(VB6.PixelsToTwipsY(.Height)) - 1
				
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
				
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
		
		'BmpInfoをカラーパレットを変えながらtimes+1個作成
		'UPGRADE_WARNING: 配列 BmpInfo で各要素を初期化する必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"' をクリックしてください。
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
			
			'カラーパレット設定
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
		
		'DIBとウインドウDCからDIBSectionを作成
		'UPGRADE_ISSUE: Form プロパティ MainForm.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		NewDC = CreateDIBSection(MainForm.hDC, BmpInfo(times), DIB_RGB_COLORS, lpBit, 0, 0)
		
		'メモリDCの作成
		'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		MemDC = CreateCompatibleDC(pic.hDC)
		
		'メモリDCにDIBSectionを選択し、元のビットマップのハンドルを保存
		OrigPicDC = SelectObject(MemDC, NewDC)
		
		'BitBltを使って元の画像をlpBitに反映
		'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		ret = BitBlt(MemDC, 0, 0, VB6.PixelsToTwipsX(pic.Width), VB6.PixelsToTwipsY(pic.Height), pic.hDC, 0, 0, SRCCOPY)
		
	End Sub
	
	Public Sub DoFade(ByRef pic As System.Windows.Forms.PictureBox, ByVal times As Integer)
		Dim ret As Integer
		
		'範囲外の場合は抜ける
		If times < 0 Or UBound(BmpInfo) < times Then
			Exit Sub
		End If
		
		'BmpInfoを変更してカラーパレットを変更
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchDIBits(.hDC, 0, 0, VB6.PixelsToTwipsX(.Width), VB6.PixelsToTwipsY(.Height), 0, 0, VB6.PixelsToTwipsX(.Width), VB6.PixelsToTwipsY(.Height), lpBit, BmpInfo(times), DIB_RGB_COLORS, SRCCOPY)
		End With
	End Sub
	
	Public Sub FinishFade()
		Dim ret As Integer
		
		'元のビットマップのハンドルを選択
		ret = SelectObject(MemDC, OrigPicDC)
		'デバイスコンテキスト開放
		ret = DeleteDC(MemDC)
		'ビットマップ開放
		ret = DeleteObject(NewDC)
	End Sub
	
	
	'
	' マスク作成用のサブルーチン
	'
	Public Sub MakeMask(ByRef src_dc As Integer, ByRef dest_dc As Integer, ByRef w As Integer, ByRef h As Integer, ByRef tcolor As Integer)
		Dim mask_dc As Integer
		Dim mask_bmp, orig_mask_bmp As Integer
		Dim ret As Integer
		
		'メモリDCの作成
		mask_dc = CreateCompatibleDC(src_dc)
		'モノクロビットマップの作成
		mask_bmp = CreateBitmap(w, h, 1, 1, 0)
		'メモリDCにビットマップを選択し元のビットマップのハンドルを保存
		orig_mask_bmp = SelectObject(mask_dc, mask_bmp)
		
		'背景色(=透明色)の設定
		ret = SetBkColor(src_dc, tcolor)
		
		ret = BitBlt(mask_dc, 0, 0, w, h, src_dc, 0, 0, SRCCOPY)
		
		'背景色を白に戻す
		If tcolor <> System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
			ret = SetBkColor(dest_dc, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
		End If
		
		ret = BitBlt(dest_dc, 0, 0, w, h, mask_dc, 0, 0, SRCCOPY)
		
		'元のビットマップのハンドルを選択
		ret = SelectObject(mask_dc, orig_mask_bmp)
		'デバイスコンテキスト開放
		ret = DeleteDC(mask_dc)
		'ビットマップ開放
		ret = DeleteObject(mask_bmp)
	End Sub
	
	'画像イメージpicをPixBufに収得
	Public Sub GetImage(ByRef pic As System.Windows.Forms.PictureBox)
		Dim pic_bmp, tmp_bmp As Integer
		Dim bm_info As BITMAPINFOHEADER
		Dim ret As Integer
		Dim mem_dc As Integer
		Dim bmp As Bitmap
		
		With pic
			'UPGRADE_WARNING: オブジェクト bmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.Image はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = GetObject_Renamed(CInt(CObj(.Image)), 24, bmp)
			PixWidth = bmp.bmWidth
			PixHeight = bmp.bmHeight
			PicWidth = VB6.PixelsToTwipsX(.Width)
			PicHeight = VB6.PixelsToTwipsY(.Height)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
		'UPGRADE_WARNING: オブジェクト PixBuf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ret = GetDIBits(mem_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
		tmp_bmp = SelectObject(mem_dc, pic_bmp)
		
		ret = DeleteObject(tmp_bmp)
	End Sub
	
	'PixBufの画像イメージをpicに描き込む
	Public Sub SetImage(ByRef pic As System.Windows.Forms.PictureBox)
		Dim pic_bmp, tmp_bmp As Integer
		Dim bm_info As BITMAPINFOHEADER
		Dim pic_dc As Integer
		Dim ret As Integer
		
		'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
		
		'UPGRADE_WARNING: オブジェクト PixBuf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ret = SetDIBits(pic_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
		tmp_bmp = SelectObject(pic_dc, pic_bmp)
		
		ret = DeleteObject(tmp_bmp)
		pic.Refresh()
		'ReDim PixBuf(0)
	End Sub
	
	'PixBufの内容を消去
	Public Sub ClearImage()
		If UBound(PixBuf) > 0 Then
			ReDim PixBuf(0)
		End If
		ReDim PixBuf2(0)
	End Sub
	
	'PixBufの内容をPixBuf2にコピー
	Public Sub CopyImage()
		Dim i As Integer
		
		ReDim PixBuf2(PixWidth * PixHeight)
		For i = 0 To PixWidth * PixHeight - 1
			'UPGRADE_WARNING: オブジェクト PixBuf2(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			PixBuf2(i) = PixBuf(i)
		Next 
	End Sub
	
	'マスク画像の作成
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
	
	'描き込み画像の作成
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
	
	'フェード処理の初期化
	Public Sub FadeInit(ByVal num As Integer)
		Dim i, j As Short
		
		ReDim FadeCMap(num, 255)
		
		For i = 1 To num
			For j = 0 To 255
				FadeCMap(i, j) = j * i \ num
			Next 
		Next 
	End Sub
	
	'フェードイン・アウト実行
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
	
	'ホワイトイン・アウト実行
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
	
	'画像を明るく
	Public Sub Bright(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像を暗く
	Public Sub Dark(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像を白黒に
	Public Sub Monotone(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像をセピア色に
	Public Sub Sepia(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像を夕焼け風に
	Public Sub Sunset(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像を水中風に
	Public Sub Water(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像を左右反転
	Public Sub HReverse()
		Dim i, j As Integer
		Dim tmp As RGBq
		
		For i = 0 To PicHeight - 1
			For j = 0 To PicWidth \ 2 - 1
				'UPGRADE_WARNING: オブジェクト tmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				tmp = PixBuf(PicWidth * i + j)
				'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				PixBuf(PicWidth * i + j) = PixBuf(PicWidth * i + PicWidth - j - 1)
				'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + PicWidth - j - 1) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				PixBuf(PicWidth * i + PicWidth - j - 1) = tmp
			Next j
		Next i
	End Sub
	
	'画像を上下反転
	Public Sub VReverse()
		Dim i, j As Integer
		Dim tmp As RGBq
		
		For i = 0 To PicHeight \ 2 - 1
			For j = 0 To PicWidth - 1
				'UPGRADE_WARNING: オブジェクト tmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				tmp = PixBuf(PicWidth * i + j)
				'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				PixBuf(PicWidth * i + j) = PixBuf(PicWidth * (PicHeight - i - 1) + j)
				'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * PicHeight - i - 1 + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				PixBuf(PicWidth * (PicHeight - i - 1) + j) = tmp
			Next j
		Next i
	End Sub
	
	'画像をネガポジ反転
	Public Sub NegPosReverse(Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		If is_transparent Then
			'背景色をRGBに分解
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
	
	'画像からシルエット抽出
	Public Sub Silhouette()
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		
		'背景色をRGBに分解
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
	
	'画像を右方向にangle度回転させる
	'do_sameがTrueの場合は回転角度が90度の倍数である際の描画最適化を行わない
	Public Sub Rotate(ByVal angle As Integer, Optional ByVal do_same As Boolean = False)
		Dim i, j As Integer
		Dim xsrc, ysrc As Integer
		Dim xsrc0, ysrc0 As Double
		Dim xbase, ybase As Double
		Dim xoffset, yoffset As Double
		Dim dsin, rad, dcos As Double
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim bg As RGBq
		Dim g, rgb_Renamed, r, b As Integer
		
		'360度で一回転
		angle = angle Mod 360
		'負の場合は正の角度に
		If angle < 0 Then
			angle = 360 + angle
		End If
		
		'回転角度が90度の倍数である場合は処理が簡単。
		'ただし、90度以外の角度で連続回転させる場合は、処理時間を一定にするため
		'この最適化は使わない。
		If Not do_same Then
			Select Case angle
				Case 0
					Exit Sub
				Case 90
					If PicWidth = PicHeight Then
						CopyImage()
						For i = 0 To PicHeight - 1
							For j = 0 To PicWidth - 1
								'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
								PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * (PicWidth - j - 1) + i)
							Next 
						Next 
						Exit Sub
					End If
				Case 180
					CopyImage()
					For i = 0 To PicHeight - 1
						For j = 0 To PicWidth - 1
							'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * (PicHeight - i - 1) + PicWidth - j - 1)
						Next 
					Next 
					Exit Sub
				Case 270
					If PicWidth = PicHeight Then
						CopyImage()
						For i = 0 To PicHeight - 1
							For j = 0 To PicWidth - 1
								'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
								PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * j + PicHeight - i - 1)
							Next 
						Next 
						Exit Sub
					End If
			End Select
		End If
		
		'任意の角度の場合は三角関数を使う必要がある
		
		'座標の計算は画像の中心を座標原点にして行う
		xbase = (PicWidth - 1) / 2
		ybase = (PicHeight - 1) / 2
		
		'回転によるベクトル
		angle = 90 - angle
		rad = CDbl(angle) * 3.14159265 / 180
		dsin = System.Math.Sin(rad)
		dcos = System.Math.Cos(rad)
		
		'背景色をRGBに分解
		rgb_Renamed = BGColor
		r = rgb_Renamed Mod &H100
		rgb_Renamed = rgb_Renamed - r
		g = rgb_Renamed Mod &H10000
		rgb_Renamed = rgb_Renamed - g
		g = g \ &H100
		b = rgb_Renamed \ &H10000
		'合成
		With bg
			.Red = r
			.Green = g
			.Blue = b
		End With
		
		'画像データのコピーを取っておく
		CopyImage()
		
		'各ピクセルに対して回転処理
		For i = 0 To PicHeight - 1
			yoffset = i - ybase
			xsrc0 = xbase + yoffset * dcos
			ysrc0 = ybase + yoffset * dsin
			
			For j = 0 To PicWidth - 1
				xoffset = j - xbase
				
				'本当は下記の式で一度に計算できるが、高速化のため式を分割
				'xsrc = xbase + xoffset * dsin + yoffset * dcos
				'ysrc = ybase - xoffset * dcos + yoffset * dsin
				xsrc = CInt(xsrc0 + xoffset * dsin)
				ysrc = CInt(ysrc0 - xoffset * dcos)
				
				If xsrc < 0 Or PicWidth <= xsrc Or ysrc < 0 Or PicHeight <= ysrc Then
					'範囲外のピクセルの場合は背景色で描画
					'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					PixBuf(PicWidth * i + j) = bg
				Else
					'UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * ysrc + xsrc)
				End If
			Next 
		Next 
	End Sub
	
	'透過率trans_parでfcolorによる半透明描画を行う
	Public Sub ColorFilter(ByRef fcolor As Integer, ByRef trans_par As Double, Optional ByVal is_transparent As Boolean = False)
		'UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim i, rgb_Renamed As Integer
		Dim g, r, b As Integer
		Dim g2, r2, b2 As Byte
		Dim tratio As Integer
		
		'透過率をパーセントに直す
		tratio = MinLng(MaxLng(100 * trans_par, 0), 100)
		
		If tratio = 0 Then
			'透過しない場合はそのまま終了
			Exit Sub
		End If
		
		'半透明描画色をRGBに分解
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
			'背景色をRGBに分解
			rgb_Renamed = BGColor
			r = rgb_Renamed Mod &H100
			rgb_Renamed = rgb_Renamed - r
			g = rgb_Renamed Mod &H10000
			rgb_Renamed = rgb_Renamed - g
			g = g \ &H100
			b = rgb_Renamed \ &H10000
			
			'背景色と半透明描画色が同一だった場合、半透明描画色を背景色から少しずらす
			'ただしこの処理が可能なのは背景色が白等の場合のみ
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