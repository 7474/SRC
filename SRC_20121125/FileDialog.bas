Attribute VB_Name = "FileDialog"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'APIだけでファイルダイアログを実現するためのモジュール

' OPENFILENAME構造体
Private Type OPENFILENAME
    lStructSize         As Long
    hwndOwner           As Long
    hInstance           As Long
    lpstrFilter         As String
    lpstrCustomFilter   As String
    nMaxCustFilter      As Long
    iFilterIndex        As Long
    lpstrFile           As String
    nMaxFile            As Long
    lpstrFileTitle      As String
    nMaxFileTitle       As Long
    lpstrInitialDir     As String
    lpstrTitle          As String
    flags               As Long
    nFileOffset         As Integer
    nFileExtension      As Integer
    lpstrDefExt         As String
    lCustData           As Long
    lpfnHook            As Long
    lpTemplateName      As String
End Type

'「ファイルを開く」のダイアログを作成
Private Declare Function GetOpenFileName Lib "comdlg32.dll" Alias "GetOpenFileNameA" _
    (f As OPENFILENAME) As Long

'「ファイルを保存」のダイアログを作成
Private Declare Function GetSaveFileName Lib "comdlg32.dll" Alias "GetSaveFileNameA" _
    (f As OPENFILENAME) As Long

'ファイルロード用のダイアログを表示するための関数
Public Function LoadFileDialog(dtitle As String, _
    fpath As String, default_file As String, _
    ByVal default_filter As Integer, _
    ftype As String, fsuffix As String, _
    Optional ftype2 As String, Optional fsuffix2 As String, _
    Optional ftype3 As String, Optional fsuffix3 As String) As String
Dim f As OPENFILENAME
Dim fname As String, ftitle As String
Dim ret As Long
    
    fname = default_file & String$(1024 - Len(default_file), vbNullChar)
    ftitle = Space$(1024)
    
    ' OPENFILENAME構造体の初期化
    With f
        .hwndOwner = MainForm.hwnd
        If ftype3 <> "" Then
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar _
                & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix3 & vbNullChar
        ElseIf ftype2 <> "" Then
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        Else
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
        End If
        .iFilterIndex = default_filter
        .lpstrFile = fname
        .nMaxFile = 1024
        .lpstrFileTitle = ftitle
        .nMaxFileTitle = 1024
        .lpstrInitialDir = fpath
        .lpstrTitle = dtitle
        .flags = &H201804
        .lpstrDefExt = ""
        .lStructSize = Len(f)
    End With
    
    ret = GetOpenFileName(f)
    
    Select Case ret
        Case 0
            'キャンセルされた
            LoadFileDialog = ""
        Case 1
            'ファイルを選択
            LoadFileDialog = f.lpstrFile
            ' vbNullChar までで切り出す。
            LoadFileDialog = Left$(LoadFileDialog, InStr(LoadFileDialog, vbNullChar) - 1)
    End Select
End Function

'ファイルセーブ用のダイアログを表示するための関数
Public Function SaveFileDialog(dtitle As String, _
    fpath As String, default_file As String, _
    ByVal default_filter As Integer, _
    ftype As String, fsuffix As String, _
    Optional ftype2 As String, Optional fsuffix2 As String, _
    Optional ftype3 As String, Optional fsuffix3 As String) As String
Dim f As OPENFILENAME
Dim fname As String, ftitle As String
Dim ret As Long
    
    fname = default_file & String$(1024 - Len(default_file), vbNullChar)
    ftitle = Space$(1024)
    
    ' OPENFILENAME構造体の初期化
    With f
        .hwndOwner = MainForm.hwnd
        If ftype3 <> "" Then
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar _
                & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        ElseIf ftype2 <> "" Then
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        Else
            .lpstrFilter = _
                "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
        End If
        .iFilterIndex = default_filter
        .lpstrFile = fname
        .nMaxFile = 1024
        .lpstrFileTitle = ftitle
        .nMaxFileTitle = 1024
        .lpstrInitialDir = fpath
        .lpstrTitle = dtitle
        .flags = &H201804
        .lpstrDefExt = ""
        .lStructSize = Len(f)
    End With
    
    ret = GetSaveFileName(f)
    
    Select Case ret
        Case 0
            'キャンセルされた
            SaveFileDialog = ""
        Case 1
            'ファイルを選択
            SaveFileDialog = f.lpstrFile
            ' vbNullChar までで切り出す。
            SaveFileDialog = Left$(SaveFileDialog, InStr(SaveFileDialog, vbNullChar) - 1)
    End Select
End Function

