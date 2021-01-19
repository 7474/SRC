Attribute VB_Name = "GeneralLib"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�ėp�I�ȏ������s�����W���[��

'ini�t�@�C���̓ǂݏo��
Declare Function GetPrivateProfileString Lib "kernel32" _
    Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, _
    ByVal lpKeyName As Any, ByVal lpDefault As String, _
    ByVal lpReturnedString As String, ByVal nSize As Long, _
    ByVal lpFileName As String) As Long

'ini�t�@�C���ւ̏�������
Declare Function WritePrivateProfileString Lib "kernel32" _
    Alias "WritePrivateProfileStringA" _
    (ByVal lpApplicationName As String, ByVal lpKeyName As Any, _
    ByVal lpString As String, ByVal lpFileName As String) As Long

'Windows���N�����Ă���̎��Ԃ�Ԃ�(�~���b)
Declare Function timeGetTime Lib "winmm.dll" () As Long

'���ԏ����̉𑜓x��ύX����
Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Long) As Long
Declare Function timeEndPeriod Lib "winmm.dll" (ByVal uPeriod As Long) As Long

'�t�@�C��������Ԃ�
Declare Function GetFileAttributes Lib "kernel32" Alias "GetFileAttributesA" _
    (ByVal lpFileName As String) As Long

'OS�̃o�[�W��������Ԃ�
Type OSVERSIONINFO
    dwOSVersionInfoSize As Long
    dwMajorVersion      As Long
    dwMinorVersion      As Long
    dwBuildNumber       As Long
    dwPlatformId        As Long
    szCSDVersion        As String * 128
End Type

Declare Function GetVersionEx Lib "kernel32" Alias "GetVersionExA" _
    (lpVersionInformation As OSVERSIONINFO) As Long
Const VER_PLATFORM_WIN32_NT = 2


'���������p�V�[�h�l
Public RndSeed As Long

'�����n��
Private RndHistory(4096) As Single

'�����n��̒��Ō��ݎg�p���Ă���l�̃C���f�b�N�X
Public RndIndex As Integer

'�����n��̃��Z�b�g
Public Sub RndReset()
Dim i As Integer

    Randomize RndSeed
    
    '�����n��̃Z�[�u�����[�h���o����悤�����n������炩����
    '�z��ɕۑ����Ċm�肳����
    For i = 1 To UBound(RndHistory)
        RndHistory(i) = Rnd
    Next
    
    RndIndex = 0
End Sub

' 1�`max �̗�����Ԃ�
Public Function Dice(ByVal max As Long) As Long
    If max <= 1 Then
        Dice = max
        Exit Function
    End If
    
    If IsOptionDefined("�����n���ۑ�") Then
        Dice = Int((max * Rnd) + 1)
        Exit Function
    End If
    
    RndIndex = RndIndex + 1
    If RndIndex > UBound(RndHistory) Then
        RndIndex = 1
    End If
    
    Dice = Int((max * RndHistory(RndIndex)) + 1)
End Function


' ���X�g list ���� idx �Ԗڂ̗v�f��Ԃ�
Public Function LIndex(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer
Dim list_len As Integer
Dim begin As Integer

    'idx�����̐��łȂ���΋󕶎����Ԃ�
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx�Ԗڂ̗v�f�܂œǂݔ�΂�
    n = 0
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '�v�f�����P���₷
        n = n + 1
        
        '���߂�v�f�H
        If n = idx Then
            Exit Do
        End If
        
        '�v�f��ǂݔ�΂�
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop Until Mid$(list, i, 1) = " "
    Loop
    
    '���߂�v�f��ǂݍ���
    begin = i
    Do
        i = i + 1
        If i > list_len Then
            LIndex = Mid$(list, begin)
            Exit Function
        End If
    Loop Until Mid$(list, i, 1) = " "
    
    LIndex = Mid$(list, begin, i - begin)
End Function

' ���X�g list �̗v�f����Ԃ�
Public Function LLength(list As String) As Integer
Dim i As Integer
Dim list_len As Integer

    LLength = 0
    list_len = Len(list)
    
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '�v�f�����P���₷
        LLength = LLength + 1
        
        '�v�f��ǂݔ�΂�
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop Until Mid$(list, i, 1) = " "
    Loop
End Function

' ���X�g list ����A���X�g�̗v�f�̔z�� larray ���쐬���A
' ���X�g�̗v�f����Ԃ�
Public Function LSplit(list As String, larray() As String) As Integer
Dim i As Integer
Dim list_len As Integer
Dim begin As Integer
    
    LSplit = 0
    list_len = Len(list)
    
    ReDim larray(0)
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '�v�f�����P���₷
        LSplit = LSplit + 1
        
        '�v�f��ǂݍ���
        ReDim Preserve larray(LSplit)
        begin = i
        Do
            i = i + 1
            If i > list_len Then
                larray(LSplit) = Mid$(list, begin)
                Exit Function
            End If
        Loop Until Mid$(list, i, 1) = " "
        larray(LSplit) = Mid$(list, begin, i - begin)
    Loop
End Function

'������ ch ���󔒂��ǂ������ׂ�
Public Function IsSpace(ch As String) As Boolean
    If Len(ch) = 0 Then
        IsSpace = True
        Exit Function
    End If
    
    Select Case Asc(ch)
        Case 9, 13, 32, 160
            IsSpace = True
    End Select
End Function

'���X�g list �ɗv�f str ��ǉ�
Public Sub LAppend(list As String, str As String)
    list = Trim$(list)
    str = Trim$(str)
    If list <> "" Then
        If str <> "" Then
            list = list & " " & str
        End If
    Else
        If str <> "" Then
            list = str
        End If
    End If
End Sub

'���X�g list �� str ���o�ꂷ��ʒu��Ԃ�
Public Function SearchList(list As String, str As String) As Integer
Dim i As Integer
    
    For i = 1 To LLength(list)
        If LIndex(list, i) = str Then
            SearchList = i
            Exit Function
        End If
    Next
    
    SearchList = 0
End Function


' ���X�g list ���� idx �Ԗڂ̗v�f��Ԃ� (���ʂ��l��)
Public Function ListIndex(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer, ch As Integer
Dim list_len As Integer, paren As Integer, begin As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    'idx�����̐��łȂ���΋󕶎����Ԃ�
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx�Ԗڂ̗v�f�܂œǂݔ�΂�
    n = 0
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do While True
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�󔒂łȂ��H
            Select Case ch
                Case 9, 32
                    '�X�L�b�v
                Case Else
                    Exit Do
            End Select
        Loop
        
        '�v�f�����P���₷
        n = n + 1
        
        '���߂�v�f�H
        If n = idx Then
            Exit Do
        End If
        
        '�v�f��ǂݔ�΂�
        Do While True
            If in_single_quote Then
                If ch = 96 Then ' "`"
                   in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If ch = 34 Then ' """"
                    in_double_quote = False
                End If
            Else
                Select Case ch
                    Case 40, 91 ' "(", "["
                        paren = paren + 1
                    Case 41, 93 ' ")", "]"
                        paren = paren - 1
                        If paren < 0 Then
                            '���ʂ̑Ή������Ă��Ȃ�
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�v�f�̖���������
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '�󔒁H
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
    
    '���߂�v�f��ǂݍ���
    begin = i
    Do While True
        If in_single_quote Then
            If ch = 96 Then ' "`"
               in_single_quote = False
            End If
        ElseIf in_double_quote Then
            If ch = 34 Then ' """"
                in_double_quote = False
            End If
        Else
            Select Case ch
                Case 40, 91 ' "(", "["
                    paren = paren + 1
                Case 41, 93 ' ")", "]"
                    paren = paren - 1
                    If paren < 0 Then
                        '���ʂ̑Ή������Ă��Ȃ�
                        Exit Function
                    End If
                Case 96 ' "`"
                    in_single_quote = True
                Case 34 ' """"
                    in_double_quote = True
            End Select
        End If
        
        i = i + 1
        
        '������̏I��H
        If i > list_len Then
            ListIndex = Mid$(list, begin)
            Exit Function
        End If
        
        '���̕���
        ch = Asc(Mid$(list, i, 1))
        
        '�v�f�̖���������
        If Not in_single_quote _
            And Not in_double_quote _
            And paren = 0 _
        Then
            '�󔒁H
            Select Case ch
                Case 9, 32
                    Exit Do
            End Select
        End If
    Loop
    
    ListIndex = Mid$(list, begin, i - begin)
End Function

' ���X�g list �̗v�f����Ԃ� (���ʂ��l��)
Public Function ListLength(list As String) As Integer
Dim i As Integer, ch As Integer
Dim list_len As Integer, paren As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    ListLength = 0
    list_len = Len(list)
    
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do While True
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�󔒂łȂ��H
            Select Case ch
                Case 9, 32
                    '�X�L�b�v
                Case Else
                    Exit Do
            End Select
        Loop
        
        '�v�f�����P���₷
        ListLength = ListLength + 1
        
        '�v�f��ǂݔ�΂�
        Do While True
            If in_single_quote Then
                If ch = 96 Then ' "`"
                   in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If ch = 34 Then ' """"
                    in_double_quote = False
                End If
            Else
                Select Case ch
                    Case 40, 91 ' "(", "["
                        paren = paren + 1
                    Case 41, 93 ' ")", "]"
                        paren = paren - 1
                        If paren < 0 Then
                            '���ʂ̑Ή������Ă��Ȃ�
                            ListLength = -1
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                '�N�H�[�g�⊇�ʂ̑Ή������Ă���H
                If in_single_quote _
                    Or in_double_quote _
                    Or paren <> 0 _
                Then
                    ListLength = -1
                End If
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�v�f�̖���������
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '�󔒁H
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
End Function

' ���X�g list ����A���X�g�̗v�f�̔z�� larray ���쐬���A
' ���X�g�̗v�f����Ԃ� (���ʂ��l��)
Public Function ListSplit(list As String, larray() As String) As Integer
Dim i As Integer, n As Integer, ch As Integer
Dim list_len As Integer, paren As Integer, begin As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
    
    ListSplit = -1
    list_len = Len(list)
    
    ReDim larray(0)
    n = 0
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do While True
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                ListSplit = n
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�󔒂łȂ��H
            Select Case ch
                Case 9, 32
                    '�X�L�b�v
                Case Else
                    Exit Do
            End Select
        Loop
        
        '�v�f�����P���₷
        n = n + 1
        
        '�v�f��ǂݍ���
        ReDim Preserve larray(n)
        begin = i
        Do While True
            If in_single_quote Then
                If ch = 96 Then ' "`"
                   in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If ch = 34 Then ' """"
                    in_double_quote = False
                End If
            Else
                Select Case ch
                    Case 40, 91 ' "(", "["
                        paren = paren + 1
                    Case 41, 93 ' ")", "]"
                        paren = paren - 1
                        If paren < 0 Then
                            '���ʂ̑Ή������Ă��Ȃ�
                            larray(n) = Mid$(list, begin)
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                larray(n) = Mid$(list, begin)
                '�N�H�[�g�⊇�ʂ̑Ή������Ă���H
                If Not in_single_quote _
                    And Not in_double_quote _
                    And paren = 0 _
                Then
                    ListSplit = n
                End If
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�v�f�̖���������
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '�󔒁H
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
        larray(n) = Mid$(list, begin, i - begin)
    Loop
End Function

' ���X�g list ���� idx �Ԗڈȍ~�̑S�v�f��Ԃ� (���ʂ��l��)
Public Function ListTail(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer, ch As Integer
Dim list_len As Integer, paren As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    'idx�����̐��łȂ���΋󕶎����Ԃ�
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx�Ԗڂ̗v�f�܂œǂݔ�΂�
    n = 0
    i = 0
    Do While True
        '�󔒂�ǂݔ�΂�
        Do While True
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�󔒂łȂ��H
            Select Case ch
                Case 9, 32
                    '�X�L�b�v
                Case Else
                    Exit Do
            End Select
        Loop
        
        '�v�f�����P���₷
        n = n + 1
        
        '���߂�v�f�H
        If n = idx Then
            Exit Do
        End If
        
        '�v�f��ǂݔ�΂�
        Do While True
            If in_single_quote Then
                If ch = 96 Then ' "`"
                   in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If ch = 34 Then ' """"
                    in_double_quote = False
                End If
            Else
                Select Case ch
                    Case 40, 91 ' "(", "["
                        paren = paren + 1
                    Case 41, 93 ' ")", "]"
                        paren = paren - 1
                        If paren < 0 Then
                            '���ʂ̑Ή������Ă��Ȃ�
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '������̏I��H
            If i > list_len Then
                Exit Function
            End If
            
            '���̕���
            ch = Asc(Mid$(list, i, 1))
            
            '�v�f�̖���������
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '�󔒁H
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
    
    ListTail = Mid$(list, i)
End Function


'�^�u���l������Trim
Public Sub TrimString(str As String)
Dim i As Integer, j As Integer, lstr As Integer
    
    lstr = Len(str)
    i = 1
    j = lstr
    
    '�擪�̋󔒂�����
    Do While i <= j
        Select Case Asc(Mid$(str, i))
            Case 9, 32, -32448
                i = i + 1
            Case Else
                Exit Do
        End Select
    Loop
    
    '�����̋󔒂�����
    Do While i < j
        Select Case Asc(Mid$(str, j))
            Case 9, 32, -32448
                j = j - 1
            Case Else
                Exit Do
        End Select
    Loop
    
    '�󔒂�����Βu������
    If i <> 1 Or j <> lstr Then
        str = Mid$(str, i, j - i + 1)
    End If
End Sub

'������ str ���� str2 ���o������ʒu�𖖔����猟��
Public Function InStr2(str As String, str2 As String) As Integer
Dim slen As Integer, i As Integer
    
    slen = Len(str2)
    i = Len(str) - slen + 1
    Do While i > 0
        If Mid$(str, i, slen) = str2 Then
            InStr2 = i
            Exit Function
        End If
        i = i - 1
    Loop
End Function


'�������Double�ɕϊ�
Public Function StrToDbl(expr As String) As Double
    If IsNumeric(expr) Then
        StrToDbl = CDbl(expr)
    End If
End Function

'�������Long�ɕϊ�
Public Function StrToLng(expr As String) As Long
    On Error GoTo ErrorHandler
    If IsNumeric(expr) Then
        StrToLng = CLng(expr)
    End If
    Exit Function
ErrorHandler:
End Function

'��������Ђ炪�Ȃɕϊ�
'�Ђ炪�Ȃւ̕ϊ��͓��{��ȊO��OS���g���ƃG���[����������悤�Ȃ̂�
'�G���[���g���b�v����K�v������
Public Function StrToHiragana(str As String) As String
    On Error GoTo ErrorHandler
    
    StrToHiragana = StrConv(str, vbHiragana)
    
    Exit Function
    
ErrorHandler:
    
    StrToHiragana = str
End Function


'a��b�̍ő�l��Ԃ�
Public Function MaxLng(ByVal a As Long, ByVal b As Long) As Long
    If a > b Then
        MaxLng = a
    Else
        MaxLng = b
    End If
End Function

'a��b�̍ŏ��l��Ԃ�
Public Function MinLng(ByVal a As Long, ByVal b As Long) As Long
    If a < b Then
        MinLng = a
    Else
        MinLng = b
    End If
End Function

'a��b�̍ő�l��Ԃ� (Double)
Public Function MaxDbl(ByVal a As Double, ByVal b As Double) As Double
    If a > b Then
        MaxDbl = a
    Else
        MaxDbl = b
    End If
End Function

'a��b�̍ŏ��l��Ԃ� (Double)
Public Function MinDbl(ByVal a As Double, ByVal b As Double) As Double
    If a < b Then
        MinDbl = a
    Else
        MinDbl = b
    End If
End Function


'������ buf �̒����� length �ɂȂ�悤�ɍ����ɃX�y�[�X��t������
Public Function LeftPaddedString(buf As String, ByVal length As Integer) As String
    LeftPaddedString = Space$(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0)) & buf
End Function

'������ buf �̒����� length �ɂȂ�悤�ɉE���ɃX�y�[�X��t������
Public Function RightPaddedString(buf As String, ByVal length As Integer) As String
    RightPaddedString = buf & Space$(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0))
End Function


' Src.ini �t�@�C���� ini_section ���� ini_entry �̒l��ǂݏo��
Public Function ReadIni(ini_section As String, ini_entry As String) As String
Dim s As String * 1024
Dim ret As Long
    
    '�V�i���I���� Src.ini �t�@�C��������΂������D��
    If FileExists(ScenarioPath & "Src.ini") Then
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, ScenarioPath & "Src.ini")
    End If
    
    '�V�i���I�� Src.ini �ɃG���g����������Ζ{�̑�����ǂݏo��
    If ret = 0 Then
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, AppPath & "Src.ini")
    End If
    
    '�s�v�������폜
    ReadIni = Left$(s, InStr(s, vbNullChar) - 1)
End Function


' Src.ini �t�@�C���� ini_section �� ini_entry �ɒl ini_data ����������
Public Sub WriteIni(ini_section As String, ini_entry As String, ini_data As String)
Dim s As String * 1024
Dim ret As Long

    'LastFolder�̐ݒ�݂͕̂K���{�̑��� Src.ini �ɏ�������
    If ini_entry = "LastFolder" Then
        ret = WritePrivateProfileString(ini_section, ini_entry, _
            ini_data, AppPath & "Src.ini")
        Exit Sub
    End If
    
    '�V�i���I���� Src.ini �t�@�C��������΂������D��
    If Len(ScenarioPath) > 0 And FileExists(ScenarioPath & "Src.ini") Then
        '�G���g�������݂��邩�`�F�b�N
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, ScenarioPath & "Src.ini")
        If ret > 1 Then
            ret = WritePrivateProfileString(ini_section, ini_entry, _
                ini_data, ScenarioPath & "Src.ini")
        End If
    End If
    
    '�V�i���I�� Src.ini �ɃG���g����������Ζ{�̑�����ǂݏo��
    If ret = 0 Then
        ret = WritePrivateProfileString(ini_section, ini_entry, _
            ini_data, AppPath & "Src.ini")
    End If
End Sub


'������ s1 ���� s2 �� s3 �ɒu��
'�u�����s�����Ƃ��ɂ�True��Ԃ�
Public Function ReplaceString(s1 As String, s2 As String, s3 As String) As Boolean
Dim buf As String
Dim len2 As Integer, len3 As Integer
Dim idx As Integer

    idx = InStr(s1, s2)
    
    '�u�����K�v�H
    If idx = 0 Then
       Exit Function
    End If
    
    len2 = Len(s2)
    len3 = Len(s3)
    
    '&�͒x���̂ŏo���邾��Mid���g��
    If len2 = len3 Then
        Do
            Mid(s1, idx, len2) = s3
            idx = InStr(s1, s2)
        Loop While idx > 0
    Else
        buf = s1
        s1 = ""
        Do
            s1 = s1 & Left$(buf, idx - 1) & s3
            buf = Mid$(buf, idx + len2)
            idx = InStr(buf, s2)
        Loop While idx > 0
        s1 = s1 & buf
    End If
    
    ReplaceString = True
End Function


'�t�@�C�� fname �����݂��邩����
Public Function FileExists(fname As String) As Boolean
    If GetFileAttributes(fname) <> -1 Then
        FileExists = True
    End If
End Function


'�f�[�^�t�@�C��fnum����f�[�^����s�ǂݍ��݁Aline_buf�Ɋi�[����ƂƂ���
'�s�ԍ�line_num���X�V����B
'�s���Ɂu#�v������ꍇ�͍s�̓ǂݔ�΂����s���B
'�s���Ɂu//�v������ꍇ�A��������̓R�����g�ƌ��Ȃ��Ė�������B
'�s���Ɂu_�v������ꍇ�͍s�̌������s���B
Public Sub GetLine(fnum As Integer, line_buf As String, line_num As Long)
Dim buf As String, idx As Integer

    line_buf = ""
    Do Until EOF(fnum)
        line_num = line_num + 1
        Line Input #fnum, buf
        
        '�R�����g�s�̓X�L�b�v
        If Left$(buf, 1) = "#" Then
            GoTo NextLine
        End If
        
        '�R�����g�������폜
        idx = InStr(buf, "//")
        If idx > 0 Then
            buf = Left$(buf, idx - 1)
        End If
        
        '�s�����u_�v�łȂ���΍s�̓ǂݍ��݂�����
        If Right$(buf, 1) <> "_" Then
            TrimString buf
            line_buf = line_buf & buf
            Exit Do
        End If
        
        '�s�����u_�v�̏ꍇ�͍s������
        TrimString buf
        line_buf = line_buf & Left$(buf, Len(buf) - 1)
        
NextLine:
    Loop
    
    ReplaceString line_buf, "�C", ", "
End Sub


'Windows�̃o�[�W�����𔻒肷��
Public Function GetWinVersion() As Integer
Dim vinfo As OSVERSIONINFO
Dim ret As Long
    
    With vinfo
        ' dwOSVersionInfoSize�ɍ\���̂̃T�C�Y���Z�b�g����B
        .dwOSVersionInfoSize = Len(vinfo)
        
        ' OS�̃o�[�W�������𓾂�B
        ret = GetVersionEx(vinfo)
        If ret = 0 Then
            Exit Function
        End If
        
        GetWinVersion = .dwMajorVersion * 100 + .dwMinorVersion
    End With
End Function


'���l���w���\�L���g�킸�ɕ�����\�L����
Public Function FormatNum(ByVal n As Double) As String
    If n = Int(n) Then
        FormatNum = Format$(n, "0")
    Else
        FormatNum = Format$(n, "0.#######################################################################")
    End If
End Function


'������ str �����l���ǂ������ׂ�
Public Function IsNumber(str As String) As Boolean
    If Not IsNumeric(str) Then
        Exit Function
    End If
    
    '"(1)"�̂悤�ȕ����񂪐��l�Ɣ��肳��Ă��܂��̂�h��
    If Asc(str) = 40 Then
        Exit Function
    End If
    
    IsNumber = True
End Function


'���푮�������p�̊֐��Q�B

'��������擾����B���������̑������ЂƂƂ��Ď擾����B
'�������l�͖h������ɂ����ĒP�̕����Ƃ��Ĉ����邽�߂ɏ����B
'���������� aname
'�����ʒu idx (�����I���ʒu��Ԃ�)
'�擾������ length (������ʐ��J�E���g�p�B��{�I��0(�����擾)��1(�����������擾))
Public Function GetClassBundle(aname As String, idx As Integer, _
    Optional ByVal length As Integer = 0) As String
Dim i As Integer, ch As String
    
    i = idx
    ch = Mid$(aname, i, 1)
    '��A���A��������΂��̎��̕����܂ňꏏ�Ɏ擾����B
    '����q�\�Ȃ��ߎ�A���A�����������胋�[�v
    Do While ch = "��" Or ch = "��" Or ch = "��"
        '�����w��̍Ō�̕�����������������ꍇ�A�����Ȃ�
        If i >= Len(aname) Then GoTo NotFoundClass
        i = i + 1
        ch = Mid$(aname, i, 1)
    Loop
    '�Ⴊ����΂��̎��̕����܂ňꏏ�Ɏ擾����B
    If ch = "��" Then
        i = i + 1
        'mid�̊J�n�ʒu�w��͕������𒴂��Ă��Ă����v�Ȃ͂��ł����O�̈�
        If i > Len(aname) Then
            GoTo NotFoundClass
        End If
        ch = Mid$(aname, i, 1)
        If ch <> "�U" And ch <> "�h" And ch <> "�^" And ch <> "��" Then
            GoTo NotFoundClass
        End If
    End If
    If length = 0 Then
        GetClassBundle = Mid$(aname, idx, i - idx + 1)
    Else
        GetClassBundle = Mid$(aname, idx, length)
    End If

NotFoundClass:
    idx = i
End Function

'InStr�Ɠ�������B�������������������̑O�Ɂu��v�u���v�u���v���������ꍇ�A�ʑ����Ɣ��肷��)
Public Function InStrNotNest(string1 As String, string2 As String, _
    Optional ByVal start As Integer = 1) As Integer
Dim i As Integer, c As String

    i = InStr(start, string1, string2)
    '�擪��v���A��v�Ȃ��̂Ƃ��A�����Ŏ擾
    If i <= 1 Then
        InStrNotNest = i
    Else
        Do While i > 0
            c = Mid$(string1, i - 1, 1)
            '���m���������̑O�̕�����������łȂ�������AInStr�̌��ʂ�Ԃ�
            If c <> "��" And c <> "��" And c <> "��" Then
                Exit Do
            End If
            '���m���������̑O�̕������������������A�ēx�������T������
            If i < Len(string1) Then
                i = InStr(i + 1, string1, string2)
            Else
                i = 0
            End If
        Loop
        InStrNotNest = i
    End If
End Function

