Attribute VB_Name = "GeneralLib"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'汎用的な処理を行うモジュール

'iniファイルの読み出し
Declare Function GetPrivateProfileString Lib "kernel32" _
    Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, _
    ByVal lpKeyName As Any, ByVal lpDefault As String, _
    ByVal lpReturnedString As String, ByVal nSize As Long, _
    ByVal lpFileName As String) As Long

'iniファイルへの書き込み
Declare Function WritePrivateProfileString Lib "kernel32" _
    Alias "WritePrivateProfileStringA" _
    (ByVal lpApplicationName As String, ByVal lpKeyName As Any, _
    ByVal lpString As String, ByVal lpFileName As String) As Long

'Windowsが起動してからの時間を返す(ミリ秒)
Declare Function timeGetTime Lib "winmm.dll" () As Long

'時間処理の解像度を変更する
Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Long) As Long
Declare Function timeEndPeriod Lib "winmm.dll" (ByVal uPeriod As Long) As Long

'ファイル属性を返す
Declare Function GetFileAttributes Lib "kernel32" Alias "GetFileAttributesA" _
    (ByVal lpFileName As String) As Long

'OSのバージョン情報を返す
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


'乱数発生用シード値
Public RndSeed As Long

'乱数系列
Private RndHistory(4096) As Single

'乱数系列の中で現在使用している値のインデックス
Public RndIndex As Integer

'乱数系列のリセット
Public Sub RndReset()
Dim i As Integer

    Randomize RndSeed
    
    '乱数系列のセーブ＆ロードが出来るよう乱数系列をあらかじめ
    '配列に保存して確定させる
    For i = 1 To UBound(RndHistory)
        RndHistory(i) = Rnd
    Next
    
    RndIndex = 0
End Sub

' 1～max の乱数を返す
Public Function Dice(ByVal max As Long) As Long
    If max <= 1 Then
        Dice = max
        Exit Function
    End If
    
    If IsOptionDefined("乱数系列非保存") Then
        Dice = Int((max * Rnd) + 1)
        Exit Function
    End If
    
    RndIndex = RndIndex + 1
    If RndIndex > UBound(RndHistory) Then
        RndIndex = 1
    End If
    
    Dice = Int((max * RndHistory(RndIndex)) + 1)
End Function


' リスト list から idx 番目の要素を返す
Public Function LIndex(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer
Dim list_len As Integer
Dim begin As Integer

    'idxが正の数でなければ空文字列を返す
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx番目の要素まで読み飛ばす
    n = 0
    i = 0
    Do While True
        '空白を読み飛ばす
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '要素数を１つ増やす
        n = n + 1
        
        '求める要素？
        If n = idx Then
            Exit Do
        End If
        
        '要素を読み飛ばす
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop Until Mid$(list, i, 1) = " "
    Loop
    
    '求める要素を読み込む
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

' リスト list の要素数を返す
Public Function LLength(list As String) As Integer
Dim i As Integer
Dim list_len As Integer

    LLength = 0
    list_len = Len(list)
    
    i = 0
    Do While True
        '空白を読み飛ばす
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '要素数を１つ増やす
        LLength = LLength + 1
        
        '要素を読み飛ばす
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop Until Mid$(list, i, 1) = " "
    Loop
End Function

' リスト list から、リストの要素の配列 larray を作成し、
' リストの要素数を返す
Public Function LSplit(list As String, larray() As String) As Integer
Dim i As Integer
Dim list_len As Integer
Dim begin As Integer
    
    LSplit = 0
    list_len = Len(list)
    
    ReDim larray(0)
    i = 0
    Do While True
        '空白を読み飛ばす
        Do
            i = i + 1
            If i > list_len Then
                Exit Function
            End If
        Loop While Mid$(list, i, 1) = " "
        
        '要素数を１つ増やす
        LSplit = LSplit + 1
        
        '要素を読み込む
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

'文字列 ch が空白かどうか調べる
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

'リスト list に要素 str を追加
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

'リスト list に str が登場する位置を返す
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


' リスト list から idx 番目の要素を返す (括弧を考慮)
Public Function ListIndex(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer, ch As Integer
Dim list_len As Integer, paren As Integer, begin As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    'idxが正の数でなければ空文字列を返す
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx番目の要素まで読み飛ばす
    n = 0
    i = 0
    Do While True
        '空白を読み飛ばす
        Do While True
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '空白でない？
            Select Case ch
                Case 9, 32
                    'スキップ
                Case Else
                    Exit Do
            End Select
        Loop
        
        '要素数を１つ増やす
        n = n + 1
        
        '求める要素？
        If n = idx Then
            Exit Do
        End If
        
        '要素を読み飛ばす
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
                            '括弧の対応が取れていない
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '要素の末尾か判定
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '空白？
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
    
    '求める要素を読み込む
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
                        '括弧の対応が取れていない
                        Exit Function
                    End If
                Case 96 ' "`"
                    in_single_quote = True
                Case 34 ' """"
                    in_double_quote = True
            End Select
        End If
        
        i = i + 1
        
        '文字列の終り？
        If i > list_len Then
            ListIndex = Mid$(list, begin)
            Exit Function
        End If
        
        '次の文字
        ch = Asc(Mid$(list, i, 1))
        
        '要素の末尾か判定
        If Not in_single_quote _
            And Not in_double_quote _
            And paren = 0 _
        Then
            '空白？
            Select Case ch
                Case 9, 32
                    Exit Do
            End Select
        End If
    Loop
    
    ListIndex = Mid$(list, begin, i - begin)
End Function

' リスト list の要素数を返す (括弧を考慮)
Public Function ListLength(list As String) As Integer
Dim i As Integer, ch As Integer
Dim list_len As Integer, paren As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    ListLength = 0
    list_len = Len(list)
    
    i = 0
    Do While True
        '空白を読み飛ばす
        Do While True
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '空白でない？
            Select Case ch
                Case 9, 32
                    'スキップ
                Case Else
                    Exit Do
            End Select
        Loop
        
        '要素数を１つ増やす
        ListLength = ListLength + 1
        
        '要素を読み飛ばす
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
                            '括弧の対応が取れていない
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
            
            '文字列の終り？
            If i > list_len Then
                'クォートや括弧の対応が取れている？
                If in_single_quote _
                    Or in_double_quote _
                    Or paren <> 0 _
                Then
                    ListLength = -1
                End If
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '要素の末尾か判定
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '空白？
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
End Function

' リスト list から、リストの要素の配列 larray を作成し、
' リストの要素数を返す (括弧を考慮)
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
        '空白を読み飛ばす
        Do While True
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                ListSplit = n
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '空白でない？
            Select Case ch
                Case 9, 32
                    'スキップ
                Case Else
                    Exit Do
            End Select
        Loop
        
        '要素数を１つ増やす
        n = n + 1
        
        '要素を読み込む
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
                            '括弧の対応が取れていない
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
            
            '文字列の終り？
            If i > list_len Then
                larray(n) = Mid$(list, begin)
                'クォートや括弧の対応が取れている？
                If Not in_single_quote _
                    And Not in_double_quote _
                    And paren = 0 _
                Then
                    ListSplit = n
                End If
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '要素の末尾か判定
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '空白？
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
        larray(n) = Mid$(list, begin, i - begin)
    Loop
End Function

' リスト list から idx 番目以降の全要素を返す (括弧を考慮)
Public Function ListTail(list As String, ByVal idx As Integer) As String
Dim i As Integer, n As Integer, ch As Integer
Dim list_len As Integer, paren As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean

    'idxが正の数でなければ空文字列を返す
    If idx < 1 Then
        Exit Function
    End If
    
    list_len = Len(list)
    
    'idx番目の要素まで読み飛ばす
    n = 0
    i = 0
    Do While True
        '空白を読み飛ばす
        Do While True
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '空白でない？
            Select Case ch
                Case 9, 32
                    'スキップ
                Case Else
                    Exit Do
            End Select
        Loop
        
        '要素数を１つ増やす
        n = n + 1
        
        '求める要素？
        If n = idx Then
            Exit Do
        End If
        
        '要素を読み飛ばす
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
                            '括弧の対応が取れていない
                            Exit Function
                        End If
                    Case 96 ' "`"
                        in_single_quote = True
                    Case 34 ' """"
                        in_double_quote = True
                End Select
            End If
            
            i = i + 1
            
            '文字列の終り？
            If i > list_len Then
                Exit Function
            End If
            
            '次の文字
            ch = Asc(Mid$(list, i, 1))
            
            '要素の末尾か判定
            If Not in_single_quote _
                And Not in_double_quote _
                And paren = 0 _
            Then
                '空白？
                Select Case ch
                    Case 9, 32
                        Exit Do
                End Select
            End If
        Loop
    Loop
    
    ListTail = Mid$(list, i)
End Function


'タブを考慮したTrim
Public Sub TrimString(str As String)
Dim i As Integer, j As Integer, lstr As Integer
    
    lstr = Len(str)
    i = 1
    j = lstr
    
    '先頭の空白を検索
    Do While i <= j
        Select Case Asc(Mid$(str, i))
            Case 9, 32, -32448
                i = i + 1
            Case Else
                Exit Do
        End Select
    Loop
    
    '末尾の空白を検索
    Do While i < j
        Select Case Asc(Mid$(str, j))
            Case 9, 32, -32448
                j = j - 1
            Case Else
                Exit Do
        End Select
    Loop
    
    '空白があれば置き換え
    If i <> 1 Or j <> lstr Then
        str = Mid$(str, i, j - i + 1)
    End If
End Sub

'文字列 str 中に str2 が出現する位置を末尾から検索
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


'文字列をDoubleに変換
Public Function StrToDbl(expr As String) As Double
    If IsNumeric(expr) Then
        StrToDbl = CDbl(expr)
    End If
End Function

'文字列をLongに変換
Public Function StrToLng(expr As String) As Long
    On Error GoTo ErrorHandler
    If IsNumeric(expr) Then
        StrToLng = CLng(expr)
    End If
    Exit Function
ErrorHandler:
End Function

'文字列をひらがなに変換
'ひらがなへの変換は日本語以外のOSを使うとエラーが発生するようなので
'エラーをトラップする必要がある
Public Function StrToHiragana(str As String) As String
    On Error GoTo ErrorHandler
    
    StrToHiragana = StrConv(str, vbHiragana)
    
    Exit Function
    
ErrorHandler:
    
    StrToHiragana = str
End Function


'aとbの最大値を返す
Public Function MaxLng(ByVal a As Long, ByVal b As Long) As Long
    If a > b Then
        MaxLng = a
    Else
        MaxLng = b
    End If
End Function

'aとbの最小値を返す
Public Function MinLng(ByVal a As Long, ByVal b As Long) As Long
    If a < b Then
        MinLng = a
    Else
        MinLng = b
    End If
End Function

'aとbの最大値を返す (Double)
Public Function MaxDbl(ByVal a As Double, ByVal b As Double) As Double
    If a > b Then
        MaxDbl = a
    Else
        MaxDbl = b
    End If
End Function

'aとbの最小値を返す (Double)
Public Function MinDbl(ByVal a As Double, ByVal b As Double) As Double
    If a < b Then
        MinDbl = a
    Else
        MinDbl = b
    End If
End Function


'文字列 buf の長さが length になるように左側にスペースを付加する
Public Function LeftPaddedString(buf As String, ByVal length As Integer) As String
    LeftPaddedString = Space$(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0)) & buf
End Function

'文字列 buf の長さが length になるように右側にスペースを付加する
Public Function RightPaddedString(buf As String, ByVal length As Integer) As String
    RightPaddedString = buf & Space$(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0))
End Function


' Src.ini ファイルの ini_section から ini_entry の値を読み出す
Public Function ReadIni(ini_section As String, ini_entry As String) As String
Dim s As String * 1024
Dim ret As Long
    
    'シナリオ側に Src.ini ファイルがあればそちらを優先
    If FileExists(ScenarioPath & "Src.ini") Then
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, ScenarioPath & "Src.ini")
    End If
    
    'シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
    If ret = 0 Then
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, AppPath & "Src.ini")
    End If
    
    '不要部分を削除
    ReadIni = Left$(s, InStr(s, vbNullChar) - 1)
End Function


' Src.ini ファイルの ini_section の ini_entry に値 ini_data を書き込む
Public Sub WriteIni(ini_section As String, ini_entry As String, ini_data As String)
Dim s As String * 1024
Dim ret As Long

    'LastFolderの設定のみは必ず本体側の Src.ini に書き込む
    If ini_entry = "LastFolder" Then
        ret = WritePrivateProfileString(ini_section, ini_entry, _
            ini_data, AppPath & "Src.ini")
        Exit Sub
    End If
    
    'シナリオ側に Src.ini ファイルがあればそちらを優先
    If Len(ScenarioPath) > 0 And FileExists(ScenarioPath & "Src.ini") Then
        'エントリが存在するかチェック
        ret = GetPrivateProfileString(ini_section, ini_entry, _
            "", s, 1024, ScenarioPath & "Src.ini")
        If ret > 1 Then
            ret = WritePrivateProfileString(ini_section, ini_entry, _
                ini_data, ScenarioPath & "Src.ini")
        End If
    End If
    
    'シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
    If ret = 0 Then
        ret = WritePrivateProfileString(ini_section, ini_entry, _
            ini_data, AppPath & "Src.ini")
    End If
End Sub


'文字列 s1 中の s2 を s3 に置換
'置換を行ったときにはTrueを返す
Public Function ReplaceString(s1 As String, s2 As String, s3 As String) As Boolean
Dim buf As String
Dim len2 As Integer, len3 As Integer
Dim idx As Integer

    idx = InStr(s1, s2)
    
    '置換が必要？
    If idx = 0 Then
       Exit Function
    End If
    
    len2 = Len(s2)
    len3 = Len(s3)
    
    '&は遅いので出来るだけMidを使う
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


'ファイル fname が存在するか判定
Public Function FileExists(fname As String) As Boolean
    If GetFileAttributes(fname) <> -1 Then
        FileExists = True
    End If
End Function


'データファイルfnumからデータを一行読み込み、line_bufに格納するとともに
'行番号line_numを更新する。
'行頭に「#」がある場合は行の読み飛ばしを行う。
'行中に「//」がある場合、そこからはコメントと見なして無視する。
'行末に「_」がある場合は行の結合を行う。
Public Sub GetLine(fnum As Integer, line_buf As String, line_num As Long)
Dim buf As String, idx As Integer

    line_buf = ""
    Do Until EOF(fnum)
        line_num = line_num + 1
        Line Input #fnum, buf
        
        'コメント行はスキップ
        If Left$(buf, 1) = "#" Then
            GoTo NextLine
        End If
        
        'コメント部分を削除
        idx = InStr(buf, "//")
        If idx > 0 Then
            buf = Left$(buf, idx - 1)
        End If
        
        '行末が「_」でなければ行の読み込みを完了
        If Right$(buf, 1) <> "_" Then
            TrimString buf
            line_buf = line_buf & buf
            Exit Do
        End If
        
        '行末が「_」の場合は行を結合
        TrimString buf
        line_buf = line_buf & Left$(buf, Len(buf) - 1)
        
NextLine:
    Loop
    
    ReplaceString line_buf, "，", ", "
End Sub


'Windowsのバージョンを判定する
Public Function GetWinVersion() As Integer
Dim vinfo As OSVERSIONINFO
Dim ret As Long
    
    With vinfo
        ' dwOSVersionInfoSizeに構造体のサイズをセットする。
        .dwOSVersionInfoSize = Len(vinfo)
        
        ' OSのバージョン情報を得る。
        ret = GetVersionEx(vinfo)
        If ret = 0 Then
            Exit Function
        End If
        
        GetWinVersion = .dwMajorVersion * 100 + .dwMinorVersion
    End With
End Function


'数値を指数表記を使わずに文字列表記する
Public Function FormatNum(ByVal n As Double) As String
    If n = Int(n) Then
        FormatNum = Format$(n, "0")
    Else
        FormatNum = Format$(n, "0.#######################################################################")
    End If
End Function


'文字列 str が数値かどうか調べる
Public Function IsNumber(str As String) As Boolean
    If Not IsNumeric(str) Then
        Exit Function
    End If
    
    '"(1)"のような文字列が数値と判定されてしまうのを防ぐ
    If Asc(str) = 40 Then
        Exit Function
    End If
    
    IsNumber = True
End Function


'武器属性処理用の関数群。

'属性を一つ取得する。複数文字の属性もひとつとして取得する。
'ただしＭは防御特性において単体文字として扱われるために除く。
'検索文字列 aname
'検索位置 idx (検索終了位置を返す)
'取得文字長 length (特殊効果数カウント用。基本的に0(属性取得)か1(属性頭文字取得))
Public Function GetClassBundle(aname As String, idx As Integer, _
    Optional ByVal length As Integer = 0) As String
Dim i As Integer, ch As String
    
    i = idx
    ch = Mid$(aname, i, 1)
    '弱、効、剋があればその次の文字まで一緒に取得する。
    '入れ子可能なため弱、効、剋が続く限りループ
    Do While ch = "弱" Or ch = "効" Or ch = "剋"
        '属性指定の最後の文字が弱効剋だった場合、属性なし
        If i >= Len(aname) Then GoTo NotFoundClass
        i = i + 1
        ch = Mid$(aname, i, 1)
    Loop
    '低があればその次の文字まで一緒に取得する。
    If ch = "低" Then
        i = i + 1
        'midの開始位置指定は文字数を超えていても大丈夫なはずですが念の為
        If i > Len(aname) Then
            GoTo NotFoundClass
        End If
        ch = Mid$(aname, i, 1)
        If ch <> "攻" And ch <> "防" And ch <> "運" And ch <> "移" Then
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

'InStrと同じ動作。ただし見つかった文字の前に「弱」「効」「剋」があった場合、別属性と判定する)
Public Function InStrNotNest(string1 As String, string2 As String, _
    Optional ByVal start As Integer = 1) As Integer
Dim i As Integer, c As String

    i = InStr(start, string1, string2)
    '先頭一致か、一致なしのとき、ここで取得
    If i <= 1 Then
        InStrNotNest = i
    Else
        Do While i > 0
            c = Mid$(string1, i - 1, 1)
            '検知した文字の前の文字が弱効剋でなかったら、InStrの結果を返す
            If c <> "弱" And c <> "効" And c <> "剋" Then
                Exit Do
            End If
            '検知した文字の前の文字が弱効剋だったら、再度文字列を探索する
            If i < Len(string1) Then
                i = InStr(i + 1, string1, string2)
            Else
                i = 0
            End If
        Loop
        InStrNotNest = i
    End If
End Function

