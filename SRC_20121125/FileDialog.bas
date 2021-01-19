Attribute VB_Name = "FileDialog"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'API�����Ńt�@�C���_�C�A���O���������邽�߂̃��W���[��

' OPENFILENAME�\����
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

'�u�t�@�C�����J���v�̃_�C�A���O���쐬
Private Declare Function GetOpenFileName Lib "comdlg32.dll" Alias "GetOpenFileNameA" _
    (f As OPENFILENAME) As Long

'�u�t�@�C����ۑ��v�̃_�C�A���O���쐬
Private Declare Function GetSaveFileName Lib "comdlg32.dll" Alias "GetSaveFileNameA" _
    (f As OPENFILENAME) As Long

'�t�@�C�����[�h�p�̃_�C�A���O��\�����邽�߂̊֐�
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
    
    ' OPENFILENAME�\���̂̏�����
    With f
        .hwndOwner = MainForm.hwnd
        If ftype3 <> "" Then
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar _
                & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix3 & vbNullChar
        ElseIf ftype2 <> "" Then
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        Else
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
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
            '�L�����Z�����ꂽ
            LoadFileDialog = ""
        Case 1
            '�t�@�C����I��
            LoadFileDialog = f.lpstrFile
            ' vbNullChar �܂łŐ؂�o���B
            LoadFileDialog = Left$(LoadFileDialog, InStr(LoadFileDialog, vbNullChar) - 1)
    End Select
End Function

'�t�@�C���Z�[�u�p�̃_�C�A���O��\�����邽�߂̊֐�
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
    
    ' OPENFILENAME�\���̂̏�����
    With f
        .hwndOwner = MainForm.hwnd
        If ftype3 <> "" Then
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar _
                & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        ElseIf ftype2 <> "" Then
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
                & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar _
                & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
        Else
            .lpstrFilter = _
                "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar _
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
            '�L�����Z�����ꂽ
            SaveFileDialog = ""
        Case 1
            '�t�@�C����I��
            SaveFileDialog = f.lpstrFile
            ' vbNullChar �܂łŐ؂�o���B
            SaveFileDialog = Left$(SaveFileDialog, InStr(SaveFileDialog, vbNullChar) - 1)
    End Select
End Function

