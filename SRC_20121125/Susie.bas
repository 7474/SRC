Attribute VB_Name = "Susie"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'Susieプラグインを利用して画像ファイルを読み込むためのモジュール

'Susie 32-bit Plug-in API
Private Declare Function GetPNGPicture Lib "ifpng.spi" Alias "GetPicture" (buf As Any, _
   ByVal length As Long, ByVal flag As Long, pHBInfo As Long, pHBm As Long, _
   ByVal lpProgressCallback As Any, ByVal lData As Long) As Long

Private Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Long, ByVal hBitmap As Long, _
    ByVal nStartScan As Long, ByVal nNumScans As Long, _
    lpBits As Any, lpBI As Any, ByVal wUsage As Long) As Long

Private Declare Function LocalFree Lib "kernel32" (ByVal hMem As Long) As Long
Private Declare Function LocalLock Lib "kernel32" (ByVal hMem As Long) As Long
Private Declare Function LocalUnlock Lib "kernel32" (ByVal hMem As Long) As Long

Private Declare Sub MoveMemory Lib "kernel32" Alias "RtlMoveMemory" (dest As Any, _
   Source As Any, ByVal length As Long)

'画像ファイルを読み込む関数
Public Function LoadPicture2(pic As PictureBox, fname As String) As Boolean
Dim HBInfo As Long, HBm As Long
Dim lpHBInfo As Long, lpHBm As Long
Dim bmi As BITMAPINFO
Dim ret As Long
    
    On Error GoTo ErrorHandler
    
    '画像の取得
    Select Case LCase$(Right$(fname, 4))
        Case ".bmp", ".jpg", ".gif"
            'Susieプラグインを使わずにロード
            pic = LoadPicture(fname)
            LoadPicture2 = True
            Exit Function
        Case ".png"
            'PNGファイル用SusieプラグインAPIを実行
            ret = GetPNGPicture(ByVal fname, 0, 0, HBInfo, HBm, ByVal 0&, 0)
        Case Else
            '未サポートのファイル形式
            ErrorMessage "画像ファイル" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "の画像フォーマットはサポートされていません。"
            pic = LoadPicture("")
            Exit Function
    End Select
    
    '読み込みに成功した？
    If ret <> 0 Then
        ErrorMessage "画像ファイル" & vbCr & vbLf _
            & fname & vbCr & vbLf _
            & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
            & "画像ファイルが壊れていないか確認して下さい。"
        Exit Function
    End If
    
    'メモリのロック
    lpHBInfo = LocalLock(HBInfo)
    lpHBm = LocalLock(HBm)
    
    'なぜか画像を一旦消去しておく必要あり
    pic = LoadPicture("")
    
    With pic
        'ピクチャボックスのサイズ変更
        Call MoveMemory(bmi, ByVal lpHBInfo, Len(bmi))
        .Width = bmi.bmiHeader.biWidth
        .Height = bmi.bmiHeader.biHeight
        
        '画像の表示
        ret = SetDIBits(.hDC, .Image, 0, .Height, ByVal lpHBm, ByVal lpHBInfo, 0)
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
    Select Case LCase$(Right$(fname, 4))
        Case ".bmp", ".jpg", ".gif"
            ErrorMessage "画像ファイル" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
                & "画像ファイルが壊れていないか確認して下さい。"
        Case ".png"
            ErrorMessage "画像ファイル" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
                & "PNGファイル用Susie Plug-inがインストールされていません。"
    End Select
End Function

