Attribute VB_Name = "Graphics"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'画像処理を行うモジュール

' BITMAPINFO構造体
Public Type BITMAPINFOHEADER
    biSize          As Long 'bmiHeaderのサイズ
    biWidth         As Long 'ビットマップの幅を表すピクセル数
    biHeight        As Long 'ビットマップの高さを表すピクセル数
    biPlanes        As Integer '常に１
    biBitCount      As Integer 'ピクセルあたりのビット数
    biCompression   As Long '圧縮の種類
    biSizeImage     As Long '画像データのサイズを表すバイト数
    biXPelsPerMeter As Long '水平方向の解像度を表すメートルあたりのピクセル数
    biYPelsPerMeter As Long '垂直方向の解像度を表すメートルあたりのピクセル数
    biClrUsed       As Long 'ビットマップが実際に使用する色の数
    biClrImportant  As Long '重要な色の数(0の場合はすべての色が重要)
End Type

' パレットエントリ構造体
Public Type RGBQUAD
    rgbBlue         As Byte
    rgbGreen        As Byte
    rgbRed          As Byte
    rgbReserved     As Byte
End Type

' ビットマップ情報
Public Type BITMAPINFO
    bmiHeader As BITMAPINFOHEADER
    bmiColors(0 To 255)  As RGBQUAD
End Type

Declare Function StretchDIBits Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, ByVal dx As Long, ByVal dy As Long, _
    ByVal SrcX As Long, ByVal SrcY As Long, _
    ByVal wSrcWidth As Long, ByVal wSrcHeight As Long, _
    ByVal lpBits As Long, lpBitsInfo As BITMAPINFO, ByVal wUsage As Long, _
    ByVal dwRop As Long) As Long
Declare Function SelectObject Lib "gdi32" (ByVal hDC As Long, _
    ByVal hObject As Long) As Long
Declare Function DeleteObject Lib "gdi32" (ByVal hObject As Long) As Long
Declare Function CreateDIBSection Lib "gdi32" (ByVal hDC As Long, _
    pBitmapInfo As BITMAPINFO, ByVal un As Long, lplpVoid As Long, _
    ByVal handle As Long, ByVal dw As Long) As Long
Declare Function CreateCompatibleDC Lib "gdi32" (ByVal hDC As Long) As Long
Declare Function BitBlt Lib "gdi32" (ByVal hdest_dc As Long, _
    ByVal X As Long, ByVal Y As Long, _
    ByVal nWidth As Long, ByVal nHeight As Long, ByVal hsrc_dc As Long, _
    ByVal xsrc As Long, ByVal ysrc As Long, ByVal dwRop As Long) As Long
Declare Function DeleteDC Lib "gdi32" (ByVal hDC As Long) As Long
Declare Function CreateBitmap Lib "gdi32" (ByVal nWidth As Long, ByVal nHeight As Long, _
    ByVal nPlanes As Long, ByVal nBitCount As Long, lpBits As Any) As Long
Declare Function SetTextColor Lib "gdi32" (ByVal hDC As Long, _
    ByVal crColor As Long) As Long
Declare Function SetBkColor Lib "gdi32" (ByVal hDC As Long, _
    ByVal crColor As Long) As Long

Const SRCCOPY = &HCC0020
Const DIB_RGB_COLORS = 0
Const BI_RGB = 0


' ビットマップ構造体
Public Type Bitmap
    bmType As Long
    bmWidth As Long
    bmHeight As Long
    bmWidthBytes As Long
    bmPlanes As Integer
    bmBitsPixel As Integer
    bmBits As Long
End Type

Public Declare Function GetObject Lib "gdi32" Alias "GetObjectA" (ByVal hObject As Long, _
    ByVal nCount As Long, lpObject As Any) As Long

Public Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Long, _
    ByVal hBitmap As Long, ByVal nStartScan As Long, ByVal nNumScans As Long, _
    lpBits As Any, lpBI As BITMAPINFOHEADER, ByVal wUsage As Long) As Long
Public Declare Function GetDIBits Lib "gdi32" (ByVal aHDC As Long, _
    ByVal hBitmap As Long, ByVal nStartScan As Long, ByVal nNumScans As Long, _
    lpBits As Any, lpBI As BITMAPINFOHEADER, ByVal wUsage As Long) As Long
Public Declare Function CreateDIBitmap Lib "gdi32" (ByVal hDC As Long, _
    lpInfoHeader As BITMAPINFOHEADER, ByVal dwUsage As Long, lpInitBits As Any, _
    lpInitInfo As BITMAPINFOHEADER, ByVal wUsage As Long) As Long

'指定位置のピクセル色を取得する
Public Declare Function GetPixel Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long) As Long

' RGB構造体
Public Type RGBq
    Blue As Byte
    Green As Byte
    Red As Byte
End Type

Dim PixBuf() As RGBq 'ピクセルの色情報配列
Dim PixBuf2() As RGBq 'ピクセルの色情報配列
Dim PixWidth As Long  'バッファの内容の幅
Dim PixHeight As Long 'バッファの内容の高さ
Dim PicWidth As Long  '画像の幅
Dim PicHeight As Long '画像の高さ

'フェードイン＆アウト用変数
Dim BmpInfo() As BITMAPINFO
Dim NewDC As Long
Dim MemDC As Long
Dim OrigPicDC As Long
Dim lpBit As Long

Dim FadeCMap() As Byte


'
'フェードイン＆フェードアウト
'
Public Sub InitFade(pic As PictureBox, _
    ByVal times As Long, Optional ByVal white_out As Boolean)
Dim r As Long, g As Long, b As Long
Dim i As Long, j As Long, k As Long, l As Long
Dim tx As Long, ty As Long
Dim ret As Long
Dim cmap(255) As RGBq
Dim rgb As Long
    
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
            tx = Dice(.Width) - 1
            ty = Dice(.Height) - 1
            
            rgb = GetPixel(.hDC, tx, ty)
            
            If rgb <> 0 Then
                r = rgb Mod &H100
                rgb = rgb - r
                g = rgb Mod &H10000
                rgb = rgb - g
                g = g \ &H100
                b = rgb \ &H10000
                
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
            tx = Dice(.Width) - 1
            ty = Dice(.Height) - 1
            
            rgb = GetPixel(.hDC, tx, ty)
            
            If rgb <> 0 Then
                r = rgb Mod &H100
                rgb = rgb - r
                g = rgb Mod &H10000
                rgb = rgb - g
                g = g \ &H100
                b = rgb \ &H10000
                
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
    
    rgb = ObjColor
    
    r = rgb Mod &H100
    rgb = rgb - r
    g = rgb Mod &H10000
    rgb = rgb - g
    g = g \ &H100
    b = rgb \ &H10000
    
    With cmap(i)
        .Red = r
        .Green = g
        .Blue = b
    End With
    
    'BmpInfoをカラーパレットを変えながらtimes+1個作成
    ReDim BmpInfo(times) As BITMAPINFO
    For i = 0 To times
        With BmpInfo(i).bmiHeader
            .biSize = Len(BmpInfo(i).bmiHeader)
            .biWidth = pic.Width
            .biHeight = pic.Height
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
    NewDC = CreateDIBSection(MainForm.hDC, BmpInfo(times), DIB_RGB_COLORS, lpBit, 0, 0)
    
    'メモリDCの作成
    MemDC = CreateCompatibleDC(pic.hDC)
    
    'メモリDCにDIBSectionを選択し、元のビットマップのハンドルを保存
    OrigPicDC = SelectObject(MemDC, NewDC)
    
    'BitBltを使って元の画像をlpBitに反映
    ret = BitBlt(MemDC, 0, 0, pic.Width, pic.Height, pic.hDC, 0, 0, SRCCOPY)
    
End Sub

Public Sub DoFade(pic As PictureBox, ByVal times As Long)
Dim ret As Long
    
    '範囲外の場合は抜ける
    If times < 0 Or UBound(BmpInfo) < times Then
        Exit Sub
    End If
    
    'BmpInfoを変更してカラーパレットを変更
    With pic
        ret = StretchDIBits(.hDC, _
            0, 0, .Width, .Height, _
            0, 0, .Width, .Height, _
            lpBit, BmpInfo(times), DIB_RGB_COLORS, SRCCOPY)
    End With
End Sub

Public Sub FinishFade()
Dim ret As Long
    
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
Public Sub MakeMask(src_dc As Long, dest_dc As Long, w As Long, h As Long, tcolor As Long)
Dim mask_dc As Long
Dim mask_bmp As Long, orig_mask_bmp As Long
Dim ret As Long

    'メモリDCの作成
    mask_dc = CreateCompatibleDC(src_dc)
    'モノクロビットマップの作成
    mask_bmp = CreateBitmap(w, h, 1, 1, ByVal 0)
    'メモリDCにビットマップを選択し元のビットマップのハンドルを保存
    orig_mask_bmp = SelectObject(mask_dc, mask_bmp)
    
    '背景色(=透明色)の設定
    ret = SetBkColor(src_dc, tcolor)
        
    ret = BitBlt(mask_dc, 0, 0, w, h, src_dc, 0, 0, SRCCOPY)
    
    '背景色を白に戻す
    If tcolor <> vbWhite Then
        ret = SetBkColor(dest_dc, vbWhite)
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
Public Sub GetImage(pic As PictureBox)
Dim pic_bmp As Long, tmp_bmp As Long
Dim bm_info As BITMAPINFOHEADER
Dim ret As Long
Dim mem_dc As Long
Dim bmp As Bitmap
    
    With pic
        ret = GetObject(.Image, 24, bmp)
        PixWidth = bmp.bmWidth
        PixHeight = bmp.bmHeight
        PicWidth = .Width
        PicHeight = .Height
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
    ret = GetDIBits(mem_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
    tmp_bmp = SelectObject(mem_dc, pic_bmp)
    
    ret = DeleteObject(tmp_bmp)
End Sub

'PixBufの画像イメージをpicに描き込む
Public Sub SetImage(pic As PictureBox)
Dim pic_bmp As Long, tmp_bmp As Long
Dim bm_info As BITMAPINFOHEADER
Dim pic_dc As Long
Dim ret As Long
    
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
    
    ret = SetDIBits(pic_dc, pic_bmp, 0, PixHeight, PixBuf(0), bm_info, 0)
    tmp_bmp = SelectObject(pic_dc, pic_bmp)
    
    ret = DeleteObject(tmp_bmp)
    pic.Refresh
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
Dim i As Long
    
    ReDim PixBuf2(PixWidth * PixHeight)
    For i = 0 To PixWidth * PixHeight - 1
        PixBuf2(i) = PixBuf(i)
    Next
End Sub

'マスク画像の作成
Public Sub CreateMask(tcolor As Long)
Dim i As Long, j As Long, k As Long

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
Public Sub CreateImage(tcolor As Long)
Dim i As Long, j As Long, k As Long

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
Public Sub FadeInit(ByVal num As Long)
Dim i As Integer, j As Integer
    
    ReDim FadeCMap(num, 255)
    
    For i = 1 To num
        For j = 0 To 255
            FadeCMap(i, j) = j * i \ num
        Next
    Next
End Sub

'フェードイン・アウト実行
Public Sub FadeInOut(ByVal ind As Long, ByVal num As Long)
Dim i As Long, r As Long, g As Long, b As Long
    
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
Public Sub WhiteInOut(ByVal ind As Long, ByVal num As Long)
Dim i As Long, r As Long, g As Long, b As Long
    
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
Public Sub Bright(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
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
Public Sub Dark(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
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
Public Sub Monotone(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                If r <> .Red Or g <> .Green Or b <> .Blue Then
                    rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                    .Red = rgb
                    .Green = rgb
                    .Blue = rgb
                End If
            End With
        Next
    Else
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                .Red = rgb
                .Green = rgb
                .Blue = rgb
            End With
        Next
    End If
End Sub

'画像をセピア色に
Public Sub Sepia(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                If r <> .Red Or g <> .Green Or b <> .Blue Then
                    rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                    .Red = MinLng(1.1 * rgb, 255)
                    .Green = 0.9 * rgb
                    .Blue = 0.7 * rgb
                End If
            End With
        Next
    Else
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                .Red = MinLng(1.1 * rgb, 255)
                .Green = 0.9 * rgb
                .Blue = 0.7 * rgb
            End With
        Next
    End If
End Sub

'画像を夕焼け風に
Public Sub Sunset(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                If r <> .Red Or g <> .Green Or b <> .Blue Then
                    rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                    .Red = MinLng(0.2 * .Red + 1.3 * rgb, 255)
                    .Green = 0.2 * .Green + 0.4 * rgb
                    .Blue = 0.2 * .Blue + 0.2 * rgb
                End If
            End With
        Next
    Else
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                .Red = MinLng(0.2 * .Red + 1.3 * rgb, 255)
                .Green = 0.2 * .Green + 0.4 * rgb
                .Blue = 0.2 * .Blue + 0.2 * rgb
            End With
        Next
    End If
End Sub

'画像を水中風に
Public Sub Water(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                If r <> .Red Or g <> .Green Or b <> .Blue Then
                    rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                    .Red = 0.6 * rgb
                    .Green = 0.8 * rgb
                    .Blue = rgb
                End If
            End With
        Next
    Else
        For i = 0 To PixWidth * PixHeight - 1
            With PixBuf(i)
                rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue
                .Red = 0.6 * rgb
                .Green = 0.8 * rgb
                .Blue = rgb
            End With
        Next
    End If
End Sub

'画像を左右反転
Public Sub HReverse()
Dim i As Long, j As Long
Dim tmp As RGBq
    
    For i = 0 To PicHeight - 1
        For j = 0 To PicWidth \ 2 - 1
            tmp = PixBuf(PicWidth * i + j)
            PixBuf(PicWidth * i + j) = PixBuf(PicWidth * i + PicWidth - j - 1)
            PixBuf(PicWidth * i + PicWidth - j - 1) = tmp
        Next j
    Next i
End Sub

'画像を上下反転
Public Sub VReverse()
Dim i As Long, j As Long
Dim tmp As RGBq
    
    For i = 0 To PicHeight \ 2 - 1
        For j = 0 To PicWidth - 1
            tmp = PixBuf(PicWidth * i + j)
            PixBuf(PicWidth * i + j) = PixBuf(PicWidth * (PicHeight - i - 1) + j)
            PixBuf(PicWidth * (PicHeight - i - 1) + j) = tmp
        Next j
    Next i
End Sub

'画像をネガポジ反転
Public Sub NegPosReverse(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
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
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    '背景色をRGBに分解
    rgb = BGColor
    r = rgb Mod &H100
    rgb = rgb - r
    g = rgb Mod &H10000
    rgb = rgb - g
    g = g \ &H100
    b = rgb \ &H10000
    
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
Public Sub Rotate(ByVal angle As Long, Optional ByVal do_same As Boolean)
Dim i As Long, j As Long
Dim xsrc As Long, ysrc As Long
Dim xsrc0 As Double, ysrc0 As Double
Dim xbase As Double, ybase As Double
Dim xoffset As Double, yoffset As Double
Dim rad As Double, dsin As Double, dcos As Double
Dim bg As RGBq, rgb As Long, r As Long, g As Long, b As Long

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
                    CopyImage
                    For i = 0 To PicHeight - 1
                        For j = 0 To PicWidth - 1
                            PixBuf(PicWidth * i + j) = _
                                PixBuf2(PicWidth * (PicWidth - j - 1) + i)
                        Next
                    Next
                    Exit Sub
                End If
            Case 180
                CopyImage
                For i = 0 To PicHeight - 1
                    For j = 0 To PicWidth - 1
                        PixBuf(PicWidth * i + j) = _
                            PixBuf2(PicWidth * (PicHeight - i - 1) + PicWidth - j - 1)
                    Next
                Next
                Exit Sub
            Case 270
                If PicWidth = PicHeight Then
                    CopyImage
                    For i = 0 To PicHeight - 1
                        For j = 0 To PicWidth - 1
                            PixBuf(PicWidth * i + j) = _
                                PixBuf2(PicWidth * j + PicHeight - i - 1)
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
    dsin = Sin(rad)
    dcos = Cos(rad)
    
    '背景色をRGBに分解
    rgb = BGColor
    r = rgb Mod &H100
    rgb = rgb - r
    g = rgb Mod &H10000
    rgb = rgb - g
    g = g \ &H100
    b = rgb \ &H10000
    '合成
    With bg
        .Red = r
        .Green = g
        .Blue = b
    End With
    
    '画像データのコピーを取っておく
    CopyImage
    
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
            xsrc = CLng(xsrc0 + xoffset * dsin)
            ysrc = CLng(ysrc0 - xoffset * dcos)
            
            If xsrc < 0 Or PicWidth <= xsrc _
                Or ysrc < 0 Or PicHeight <= ysrc _
            Then
                '範囲外のピクセルの場合は背景色で描画
                PixBuf(PicWidth * i + j) = bg
            Else
                PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * ysrc + xsrc)
            End If
        Next
    Next
End Sub

'透過率trans_parでfcolorによる半透明描画を行う
Public Sub ColorFilter(fcolor As Long, trans_par As Double, Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
Dim r2 As Byte, g2 As Byte, b2 As Byte
Dim tratio As Long
    
    '透過率をパーセントに直す
    tratio = MinLng(MaxLng(100 * trans_par, 0), 100)
    
    If tratio = 0 Then
        '透過しない場合はそのまま終了
        Exit Sub
    End If
    
    '半透明描画色をRGBに分解
    rgb = fcolor
    r = rgb Mod &H100
    rgb = rgb - r
    g = rgb Mod &H10000
    rgb = rgb - g
    g = g \ &H100
    b = rgb \ &H10000
    r2 = r
    g2 = g
    b2 = b
    
    If is_transparent Then
        '背景色をRGBに分解
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
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

