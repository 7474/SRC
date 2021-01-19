Attribute VB_Name = "Graphics"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�摜�������s�����W���[��

' BITMAPINFO�\����
Public Type BITMAPINFOHEADER
    biSize          As Long 'bmiHeader�̃T�C�Y
    biWidth         As Long '�r�b�g�}�b�v�̕���\���s�N�Z����
    biHeight        As Long '�r�b�g�}�b�v�̍�����\���s�N�Z����
    biPlanes        As Integer '��ɂP
    biBitCount      As Integer '�s�N�Z��������̃r�b�g��
    biCompression   As Long '���k�̎��
    biSizeImage     As Long '�摜�f�[�^�̃T�C�Y��\���o�C�g��
    biXPelsPerMeter As Long '���������̉𑜓x��\�����[�g��������̃s�N�Z����
    biYPelsPerMeter As Long '���������̉𑜓x��\�����[�g��������̃s�N�Z����
    biClrUsed       As Long '�r�b�g�}�b�v�����ۂɎg�p����F�̐�
    biClrImportant  As Long '�d�v�ȐF�̐�(0�̏ꍇ�͂��ׂĂ̐F���d�v)
End Type

' �p���b�g�G���g���\����
Public Type RGBQUAD
    rgbBlue         As Byte
    rgbGreen        As Byte
    rgbRed          As Byte
    rgbReserved     As Byte
End Type

' �r�b�g�}�b�v���
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


' �r�b�g�}�b�v�\����
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

'�w��ʒu�̃s�N�Z���F���擾����
Public Declare Function GetPixel Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long) As Long

' RGB�\����
Public Type RGBq
    Blue As Byte
    Green As Byte
    Red As Byte
End Type

Dim PixBuf() As RGBq '�s�N�Z���̐F���z��
Dim PixBuf2() As RGBq '�s�N�Z���̐F���z��
Dim PixWidth As Long  '�o�b�t�@�̓��e�̕�
Dim PixHeight As Long '�o�b�t�@�̓��e�̍���
Dim PicWidth As Long  '�摜�̕�
Dim PicHeight As Long '�摜�̍���

'�t�F�[�h�C�����A�E�g�p�ϐ�
Dim BmpInfo() As BITMAPINFO
Dim NewDC As Long
Dim MemDC As Long
Dim OrigPicDC As Long
Dim lpBit As Long

Dim FadeCMap() As Byte


'
'�t�F�[�h�C�����t�F�[�h�A�E�g
'
Public Sub InitFade(pic As PictureBox, _
    ByVal times As Long, Optional ByVal white_out As Boolean)
Dim r As Long, g As Long, b As Long
Dim i As Long, j As Long, k As Long, l As Long
Dim tx As Long, ty As Long
Dim ret As Long
Dim cmap(255) As RGBq
Dim rgb As Long
    
    '�t�F�[�h�����͉摜��256�F�ɕϊ����čs��
    '���̂��߂�256�F�̃J���[�}�b�v���쐬����
    
    '�܂��͌��ߑł���0�`195�Ԃ̐F���쐬
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
    
    '196�`255�Ԃ̐F�͌��摜�̐F���T���v�����O���č쐬
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
    
    'BmpInfo���J���[�p���b�g��ς��Ȃ���times+1�쐬
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
        
        '�J���[�p���b�g�ݒ�
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
    
    'DIB�ƃE�C���h�EDC����DIBSection���쐬
    NewDC = CreateDIBSection(MainForm.hDC, BmpInfo(times), DIB_RGB_COLORS, lpBit, 0, 0)
    
    '������DC�̍쐬
    MemDC = CreateCompatibleDC(pic.hDC)
    
    '������DC��DIBSection��I�����A���̃r�b�g�}�b�v�̃n���h����ۑ�
    OrigPicDC = SelectObject(MemDC, NewDC)
    
    'BitBlt���g���Č��̉摜��lpBit�ɔ��f
    ret = BitBlt(MemDC, 0, 0, pic.Width, pic.Height, pic.hDC, 0, 0, SRCCOPY)
    
End Sub

Public Sub DoFade(pic As PictureBox, ByVal times As Long)
Dim ret As Long
    
    '�͈͊O�̏ꍇ�͔�����
    If times < 0 Or UBound(BmpInfo) < times Then
        Exit Sub
    End If
    
    'BmpInfo��ύX���ăJ���[�p���b�g��ύX
    With pic
        ret = StretchDIBits(.hDC, _
            0, 0, .Width, .Height, _
            0, 0, .Width, .Height, _
            lpBit, BmpInfo(times), DIB_RGB_COLORS, SRCCOPY)
    End With
End Sub

Public Sub FinishFade()
Dim ret As Long
    
    '���̃r�b�g�}�b�v�̃n���h����I��
    ret = SelectObject(MemDC, OrigPicDC)
    '�f�o�C�X�R���e�L�X�g�J��
    ret = DeleteDC(MemDC)
    '�r�b�g�}�b�v�J��
    ret = DeleteObject(NewDC)
End Sub


'
' �}�X�N�쐬�p�̃T�u���[�`��
'
Public Sub MakeMask(src_dc As Long, dest_dc As Long, w As Long, h As Long, tcolor As Long)
Dim mask_dc As Long
Dim mask_bmp As Long, orig_mask_bmp As Long
Dim ret As Long

    '������DC�̍쐬
    mask_dc = CreateCompatibleDC(src_dc)
    '���m�N���r�b�g�}�b�v�̍쐬
    mask_bmp = CreateBitmap(w, h, 1, 1, ByVal 0)
    '������DC�Ƀr�b�g�}�b�v��I�������̃r�b�g�}�b�v�̃n���h����ۑ�
    orig_mask_bmp = SelectObject(mask_dc, mask_bmp)
    
    '�w�i�F(=�����F)�̐ݒ�
    ret = SetBkColor(src_dc, tcolor)
        
    ret = BitBlt(mask_dc, 0, 0, w, h, src_dc, 0, 0, SRCCOPY)
    
    '�w�i�F�𔒂ɖ߂�
    If tcolor <> vbWhite Then
        ret = SetBkColor(dest_dc, vbWhite)
    End If
    
    ret = BitBlt(dest_dc, 0, 0, w, h, mask_dc, 0, 0, SRCCOPY)
    
    '���̃r�b�g�}�b�v�̃n���h����I��
    ret = SelectObject(mask_dc, orig_mask_bmp)
    '�f�o�C�X�R���e�L�X�g�J��
    ret = DeleteDC(mask_dc)
    '�r�b�g�}�b�v�J��
    ret = DeleteObject(mask_bmp)
End Sub

'�摜�C���[�Wpic��PixBuf�Ɏ���
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

'PixBuf�̉摜�C���[�W��pic�ɕ`������
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

'PixBuf�̓��e������
Public Sub ClearImage()
    If UBound(PixBuf) > 0 Then
        ReDim PixBuf(0)
    End If
    ReDim PixBuf2(0)
End Sub

'PixBuf�̓��e��PixBuf2�ɃR�s�[
Public Sub CopyImage()
Dim i As Long
    
    ReDim PixBuf2(PixWidth * PixHeight)
    For i = 0 To PixWidth * PixHeight - 1
        PixBuf2(i) = PixBuf(i)
    Next
End Sub

'�}�X�N�摜�̍쐬
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

'�`�����݉摜�̍쐬
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

'�t�F�[�h�����̏�����
Public Sub FadeInit(ByVal num As Long)
Dim i As Integer, j As Integer
    
    ReDim FadeCMap(num, 255)
    
    For i = 1 To num
        For j = 0 To 255
            FadeCMap(i, j) = j * i \ num
        Next
    Next
End Sub

'�t�F�[�h�C���E�A�E�g���s
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

'�z���C�g�C���E�A�E�g���s
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

'�摜�𖾂邭
Public Sub Bright(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜���Â�
Public Sub Dark(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜�𔒍���
Public Sub Monotone(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜���Z�s�A�F��
Public Sub Sepia(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜��[�Ă�����
Public Sub Sunset(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜�𐅒�����
Public Sub Water(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜�����E���]
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

'�摜���㉺���]
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

'�摜���l�K�|�W���]
Public Sub NegPosReverse(Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    If is_transparent Then
        '�w�i�F��RGB�ɕ���
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

'�摜����V���G�b�g���o
Public Sub Silhouette()
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
    
    '�w�i�F��RGB�ɕ���
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

'�摜���E������angle�x��]������
'do_same��True�̏ꍇ�͉�]�p�x��90�x�̔{���ł���ۂ̕`��œK�����s��Ȃ�
Public Sub Rotate(ByVal angle As Long, Optional ByVal do_same As Boolean)
Dim i As Long, j As Long
Dim xsrc As Long, ysrc As Long
Dim xsrc0 As Double, ysrc0 As Double
Dim xbase As Double, ybase As Double
Dim xoffset As Double, yoffset As Double
Dim rad As Double, dsin As Double, dcos As Double
Dim bg As RGBq, rgb As Long, r As Long, g As Long, b As Long

    '360�x�ň��]
    angle = angle Mod 360
    '���̏ꍇ�͐��̊p�x��
    If angle < 0 Then
        angle = 360 + angle
    End If
    
    '��]�p�x��90�x�̔{���ł���ꍇ�͏������ȒP�B
    '�������A90�x�ȊO�̊p�x�ŘA����]������ꍇ�́A�������Ԃ����ɂ��邽��
    '���̍œK���͎g��Ȃ��B
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
    
    '�C�ӂ̊p�x�̏ꍇ�͎O�p�֐����g���K�v������
    
    '���W�̌v�Z�͉摜�̒��S�����W���_�ɂ��čs��
    xbase = (PicWidth - 1) / 2
    ybase = (PicHeight - 1) / 2
    
    '��]�ɂ��x�N�g��
    angle = 90 - angle
    rad = CDbl(angle) * 3.14159265 / 180
    dsin = Sin(rad)
    dcos = Cos(rad)
    
    '�w�i�F��RGB�ɕ���
    rgb = BGColor
    r = rgb Mod &H100
    rgb = rgb - r
    g = rgb Mod &H10000
    rgb = rgb - g
    g = g \ &H100
    b = rgb \ &H10000
    '����
    With bg
        .Red = r
        .Green = g
        .Blue = b
    End With
    
    '�摜�f�[�^�̃R�s�[������Ă���
    CopyImage
    
    '�e�s�N�Z���ɑ΂��ĉ�]����
    For i = 0 To PicHeight - 1
        yoffset = i - ybase
        xsrc0 = xbase + yoffset * dcos
        ysrc0 = ybase + yoffset * dsin
        
        For j = 0 To PicWidth - 1
            xoffset = j - xbase
            
            '�{���͉��L�̎��ň�x�Ɍv�Z�ł��邪�A�������̂��ߎ��𕪊�
            'xsrc = xbase + xoffset * dsin + yoffset * dcos
            'ysrc = ybase - xoffset * dcos + yoffset * dsin
            xsrc = CLng(xsrc0 + xoffset * dsin)
            ysrc = CLng(ysrc0 - xoffset * dcos)
            
            If xsrc < 0 Or PicWidth <= xsrc _
                Or ysrc < 0 Or PicHeight <= ysrc _
            Then
                '�͈͊O�̃s�N�Z���̏ꍇ�͔w�i�F�ŕ`��
                PixBuf(PicWidth * i + j) = bg
            Else
                PixBuf(PicWidth * i + j) = PixBuf2(PicWidth * ysrc + xsrc)
            End If
        Next
    Next
End Sub

'���ߗ�trans_par��fcolor�ɂ�锼�����`����s��
Public Sub ColorFilter(fcolor As Long, trans_par As Double, Optional ByVal is_transparent As Boolean)
Dim i As Long, rgb As Long
Dim r As Long, g As Long, b As Long
Dim r2 As Byte, g2 As Byte, b2 As Byte
Dim tratio As Long
    
    '���ߗ����p�[�Z���g�ɒ���
    tratio = MinLng(MaxLng(100 * trans_par, 0), 100)
    
    If tratio = 0 Then
        '���߂��Ȃ��ꍇ�͂��̂܂܏I��
        Exit Sub
    End If
    
    '�������`��F��RGB�ɕ���
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
        '�w�i�F��RGB�ɕ���
        rgb = BGColor
        r = rgb Mod &H100
        rgb = rgb - r
        g = rgb Mod &H10000
        rgb = rgb - g
        g = g \ &H100
        b = rgb \ &H10000
        
        '�w�i�F�Ɣ������`��F�����ꂾ�����ꍇ�A�������`��F��w�i�F���班�����炷
        '���������̏������\�Ȃ͔̂w�i�F�������̏ꍇ�̂�
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

