Attribute VB_Name = "Flash"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'Flashファイルの再生
Public Sub PlayFlash(fname As String, _
    fx As Integer, fy As Integer, fw As Integer, fh As Integer, _
    opt As String)
Dim i As Integer
Dim is_VisibleEnd As Boolean
    
    'FLASHが使用できない場合はエラー
    If Not IsFlashAvailable Then
        ErrorMessage "Flashファイルの読み込み中にエラーが発生しました。" & vbCrLf _
            & "「Macromedia Flash Player」がインストールされていません。" & vbCrLf _
            & "次のURLから、最新版のFlash Playerをインストールしてください。" & vbCrLf _
            & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
        Exit Sub
    End If
    If Not frmMain.FlashObject.Enable Then
        ErrorMessage "Flashファイルの読み込み中にエラーが発生しました。" & vbCrLf _
            & "「Macromedia Flash Player」がインストールされていません。" & vbCrLf _
            & "次のURLから、最新版のFlash Playerをインストールしてください。" & vbCrLf _
            & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
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
        .Visible = False
    
        'Flashオブジェクトの位置・サイズ設定
        .Left = fx
        .Top = fy
        .Width = fw
        .Height = fh
        .Visible = True
        .ZOrder
        
        .LoadMovie ScenarioPath & fname
            
        Do While .Playing And Not IsRButtonPressed(True)
            DoEvents
        Loop
        
        frmMain.FlashObject.StopMovie
            
        If Not is_VisibleEnd Then
            .ClearMovie
            .Visible = False
        End If
    End With
End Sub

'表示したままのFlashを消去する
Public Sub ClearFlash()
    If Not IsFlashAvailable Then Exit Sub
    If Not frmMain.FlashObject.Enable Then Exit Sub
    
    With frmMain.FlashObject
        .ClearMovie
        .Visible = False
    End With
End Sub

'Flashファイルからイベントを取得
' Flashのアクションの「GetURL」で
'　1.「URL」に"FSCommand:"
'　2.「ターゲット」に「サブルーチン名 [引数1 [引数2 […]]」
'を指定すると、そのアクションが実行されたときに
'ターゲットのサブルーチンが実行される。
'サブルーチンを実行している間、Flashの再生は停止する。
Public Sub GetEvent(ByVal fpara As String)
Dim buf As String, i As Integer, j As Integer
Dim funcname As String, funcpara As String
Dim etype As ValueType, str_result As String, num_result As Double

    '再生を一時停止
    frmMain.FlashObject.StopMovie
        
    funcname = ""
    funcpara = ""
    
    '念のためにFlashから渡されたパラメータ全てを解析
    '一番最初に見つかった文字列を、呼び出すサブルーチン名とする
    If funcname = "" Then
        funcname = ListIndex(fpara, 1)
        buf = ListTail(fpara, 2)
    End If
    'サブルーチンの引数を記録
    For j = 1 To ListLength(buf)
        funcpara = funcpara & ", " & ListIndex(buf, j)
    Next
    
    'サブルーチン名と引数から、Call関数の呼び出しの文字列を生成
    buf = "Call(" & funcname & funcpara & ")"
    '式として生成した文字列を実行
    CallFunction buf, etype, str_result, num_result
    
    '再生を再開
    frmMain.FlashObject.PlayMovie
End Sub
