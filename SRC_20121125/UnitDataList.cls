VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "UnitDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全ユニットデータを管理するリストのクラス

'ユニットデータ用コレクション
Private colUnitDataList As New Collection

'ID作成用変数
Private IDNum As Long

'クラスの初期化
Private Sub Class_Initialize()
Dim ud As New UnitData
    
    With ud
        .Name = "ステータス表示用ダミーユニット"
        .Nickname = "ユニット無し"
        .PilotNum = 1
        .Transportation = "陸"
        .Adaption = "AAAA"
        .Bitmap = ".bmp"
        .AddFeature "ダミーユニット=システム用非表示能力"
    End With
    colUnitDataList.Add ud, ud.Name
End Sub

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colUnitDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colUnitDataList = Nothing
End Sub

'ユニットデータリストにデータを追加
Public Function Add(uname As String) As UnitData
Dim ud As New UnitData
    
    With ud
        .Name = uname
        colUnitDataList.Add ud, uname
        IDNum = IDNum + 1
        .ID = IDNum
    End With
    
    Set Add = ud
End Function

'ユニットデータリストに登録されているデータ数
Public Function Count() As Integer
    Count = colUnitDataList.Count
End Function

'ユニットデータリストからデータを削除
Public Sub Delete(Index As Variant)
    colUnitDataList.Remove Index
End Sub

'ユニットデータリストからデータを取り出す
Public Function Item(Index As Variant) As UnitData
    On Error GoTo ErrorHandler
    
    Set Item = colUnitDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item = Nothing
End Function

'ユニットデータリストに登録されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim dummy As UnitData

    On Error GoTo ErrorHandler
    Set dummy = colUnitDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim i As Integer, j As Integer, k As Integer
Dim n As Long, line_num As Long
Dim line_buf As String, buf As String, buf2 As String
Dim ret As Integer
Dim ud As UnitData, wd As WeaponData, sd As AbilityData
Dim wname As String, sname As String
Dim data_name As String
Dim err_msg As String
Dim in_quote As Boolean
Dim comma_num As Integer
    
    On Error GoTo ErrorHandler
    
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    line_num = 0
    
    Do While True
        data_name = ""
        
        '空行をスキップ
        Do
            If EOF(FileNumber) Then
                Close #FileNumber
                Exit Sub
            End If
            GetLine FileNumber, line_buf, line_num
        Loop While Len(line_buf) = 0
        
        '名称
        ret = InStr(line_buf, ",")
        If ret > 0 Then
            data_name = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
        Else
            data_name = line_buf
            buf = ""
        End If
        
        If InStr(data_name, " ") > 0 Then
            err_msg = "名称に半角スペースは使用出来ません。"
            Error 0
        End If
        If InStr(data_name, "（") > 0 Or InStr(data_name, "）") > 0 Then
            err_msg = "名称に全角括弧は使用出来ません。"
            Error 0
        End If
        If InStr(data_name, """") > 0 Then
            err_msg = "名称に""は使用出来ません。"
            Error 0
        End If
        
        If IsDefined(data_name) Then
            Set ud = Item(data_name)
            ud.Clear
        Else
            Set ud = Add(data_name)
        End If
        
        With ud
            '読み仮名
            ret = InStr(buf, ",")
            If ret > 0 Then
                err_msg = "読み仮名の後に余分なデータが指定されています。"
                Error 0
            End If
            .KanaName = buf
            
            '愛称, 読み仮名, ユニットクラス, パイロット数, アイテム数
            GetLine FileNumber, line_buf, line_num
            
            '書式チェックのため、コンマの数を数えておく
            comma_num = 0
            For i = 1 To Len(line_buf)
                If Mid$(line_buf, i, 1) = "," Then
                    comma_num = comma_num + 1
                End If
            Next
            
            If comma_num < 3 Then
                err_msg = "設定に抜けがあります。"
                Error 0
            ElseIf comma_num > 4 Then
                err_msg = "余分な「,」があります。"
                Error 0
            End If
            
            '愛称
            If Len(line_buf) = 0 Then
                err_msg = "愛称の設定が抜けています。"
                Error 0
            End If
            ret = InStr(line_buf, ",")
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            .Nickname = buf2
            
            '読み仮名
            If comma_num = 4 Then
                ret = InStr(buf, ",")
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                .KanaName = buf2
            Else
                .KanaName = StrToHiragana(.Nickname)
            End If
            
            'ユニットクラス
            ret = InStr(buf, ",")
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If Not IsNumeric(buf2) Then
                .Class = buf2
            Else
                DataErrorMessage "ユニットクラスの設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .Class = "汎用"
            End If
            
            'パイロット数
            ret = InStr(buf, ",")
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If Left$(buf2, 1) <> "(" Then
                If IsNumeric(buf2) Then
                    .PilotNum = MinLng(CLng(buf2), 99)
                Else
                    DataErrorMessage "パイロット数の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    .PilotNum = 1
                End If
                If .PilotNum < 1 Then
                    DataErrorMessage "パイロット数の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    .PilotNum = 1
                End If
            Else
                If Right$(buf2, 1) <> ")" Then
                    Error 0
                End If
                buf2 = Mid$(buf2, 2, Len(buf2) - 2)
                If IsNumeric(buf2) Then
                    .PilotNum = MinLng(CLng(buf2), 99)
                Else
                    DataErrorMessage "パイロット数の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    .PilotNum = 1
                End If
                If .PilotNum < 1 Then
                    DataErrorMessage "パイロット数の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    .PilotNum = 1
                End If
                .PilotNum = -.PilotNum
            End If
            
            'アイテム数
            buf = Trim$(buf)
            If Len(buf) = 0 Then
                err_msg = "アイテム数の設定が抜けています。"
                Error 0
            End If
            If IsNumeric(buf) Then
                .ItemNum = MinLng(CLng(buf), 99)
            Else
                DataErrorMessage "アイテム数の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .ItemNum = 4
            End If
            
            '移動可能地形, 移動力, サイズ, 修理費, 経験値
            GetLine FileNumber, line_buf, line_num
            
            '移動可能地形
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "移動力の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            If Not IsNumeric(buf2) Then
                .Transportation = buf2
            Else
                DataErrorMessage "移動可能地形の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .Transportation = "陸"
            End If
            
            '移動力
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "サイズの設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .Speed = MinLng(CLng(buf2), 99)
            Else
                DataErrorMessage "移動力の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
            
            'サイズ
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "経験値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            Select Case buf2
                Case "XL", "LL", "L", "M", "S", "SS"
                    .Size = buf2
                Case Else
                    DataErrorMessage "サイズの設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    .Size = "M"
            End Select
            
            '修理費
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "経験値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .Value = MinLng(CLng(buf2), 9999999)
            Else
                DataErrorMessage "修理費の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
            
            '経験値
            buf = Trim$(buf)
            If Len(buf) = 0 Then
                err_msg = "経験値の設定が抜けています。"
                Error 0
            End If
            If IsNumeric(buf) Then
                .ExpValue = MinLng(CLng(buf), 9999)
            Else
                DataErrorMessage "経験値の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
            
            '特殊能力データ
            GetLine FileNumber, line_buf, line_num
            If line_buf = "特殊能力なし" Then
                GetLine FileNumber, line_buf, line_num
            ElseIf line_buf = "特殊能力" Then
                '新形式による特殊能力表記
                GetLine FileNumber, line_buf, line_num
                
                buf = line_buf
                i = 0
                Do While True
                    i = i + 1
                    
                    ret = 0
                    in_quote = False
                    For j = 1 To Len(buf)
                        Select Case Mid$(buf, j, 1)
                            Case ","
                                If Not in_quote Then
                                    ret = j
                                    Exit For
                                End If
                            Case """"
                                in_quote = Not in_quote
                        End Select
                    Next
                    
                    If ret > 0 Then
                        buf2 = Trim$(Left$(buf, ret - 1))
                        
                        If j = 1 Then
                            If IsNumeric(buf2) Then
                                Exit Do
                            End If
                        End If
                        
                        buf = Trim$(Mid$(buf, ret + 1))
                    Else
                        buf2 = buf
                        buf = ""
                    End If
                    
                    If IsNumeric(buf2) Then
                        Exit Do
                    ElseIf buf2 = "" Or IsNumeric(buf2) Then
                        DataErrorMessage "行頭から" & Format$(i) & _
                            "番目の特殊能力の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                    Else
                        .AddFeature buf2
                    End If
                    
                    If buf = "" Then
                        GetLine FileNumber, line_buf, line_num
                        buf = line_buf
                        i = 0
                    End If
                Loop
            ElseIf InStr(line_buf, "特殊能力,") = 1 Then
                '旧形式による特殊能力表記
                buf = Mid$(line_buf, 6)
                
                ret = 0
                in_quote = False
                For k = 1 To Len(buf)
                    Select Case Mid$(buf, k, 1)
                        Case ","
                            If Not in_quote Then
                                ret = k
                                Exit For
                            End If
                        Case """"
                            in_quote = Not in_quote
                    End Select
                Next
                
                i = 0
                Do While ret > 0
                    i = i + 1
                    buf2 = Trim$(Left$(buf, ret - 1))
                    buf = Mid$(buf, ret + 1)
                    ret = InStr(buf, ",")
                    If buf2 <> "" Then
                        .AddFeature buf2
                    Else
                        DataErrorMessage Format$(i) & "番目の特殊能力の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                    End If
                Loop
                
                i = i + 1
                buf2 = Trim$(buf)
                If buf2 <> "" Then
                    .AddFeature buf2
                Else
                    DataErrorMessage Format$(i) & "番目の特殊能力の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                End If
                
                GetLine FileNumber, line_buf, line_num
            Else
                err_msg = "特殊能力の設定が抜けています。"
                Error 0
            End If
            
            '最大ＨＰ
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "最大ＥＮの設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            If IsNumeric(buf2) Then
                .HP = MinLng(CLng(buf2), 9999999)
            Else
                DataErrorMessage "最大ＨＰの設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .HP = 1000
            End If
            
            '最大ＥＮ
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "装甲の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .EN = MinLng(CLng(buf2), 9999)
            Else
                DataErrorMessage "最大ＥＮの設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .EN = 100
            End If
            
            '装甲
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "運動性の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .Armor = MinLng(CLng(buf2), 99999)
            Else
                DataErrorMessage "装甲の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
            
            '運動性
            buf2 = Trim$(buf)
            If Len(buf2) = 0 Then
                err_msg = "運動性の設定が抜けています。"
                Error 0
            End If
            If IsNumeric(buf2) Then
                .Mobility = MinLng(CLng(buf2), 9999)
            Else
                DataErrorMessage "運動性の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
            
            '地形適応, ビットマップ
            GetLine FileNumber, line_buf, line_num
            
            '地形適応
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "ビットマップの設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            If Len(buf2) = 4 Then
                .Adaption = buf2
            Else
                DataErrorMessage "地形適応の設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .Adaption = "AAAA"
            End If
            
            'ビットマップ
            buf = Trim$(buf)
            If Len(buf) = 0 Then
                err_msg = "ビットマップの設定が抜けています。"
                Error 0
            End If
            If LCase$(Right$(buf, 4)) = ".bmp" Then
                .Bitmap = buf
            Else
                DataErrorMessage "ビットマップの設定が間違っています。", _
                    fname, line_num, line_buf, .Name
                .IsBitmapMissing = True
            End If
            
            If EOF(FileNumber) Then
                Close #FileNumber
                Exit Sub
            End If
            
            '武器データ
            GetLine FileNumber, line_buf, line_num
            Do While Len(line_buf) > 0 And line_buf <> "==="
                '武器名
                ret = InStr(line_buf, ",")
                If ret = 0 Then
                    err_msg = "武器データの終りには空行を置いてください。"
                    Error 0
                End If
                wname = Trim$(Left$(line_buf, ret - 1))
                buf = Mid$(line_buf, ret + 1)
                
                If wname = "" Then
                    err_msg = "武器名の設定が間違っています。"
                    Error 0
                End If
                
                '武器を登録
                Set wd = .AddWeapon(wname)
                
                '攻撃力
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の最小射程が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    wd.Power = MinLng(CLng(buf2), 99999)
                ElseIf buf = "-" Then
                    wd.Power = 0
                Else
                    DataErrorMessage wname & "の攻撃力の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.Power = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '最小射程
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の最大射程の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    wd.MinRange = CInt(buf2)
                Else
                    DataErrorMessage wname & "の最小射程の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    wd.MinRange = 1
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.MinRange = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '最大射程
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の命中率の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    wd.MaxRange = MinLng(CLng(buf2), 99)
                Else
                    DataErrorMessage wname & "の最大射程の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    wd.MaxRange = 1
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.MaxRange = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '命中率
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の弾数の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    n = CLng(buf2)
                    If n > 999 Then
                        n = 999
                    ElseIf n < -999 Then
                        n = -999
                    End If
                    wd.Precision = n
                Else
                    DataErrorMessage wname & "の命中率の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.Precision = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '弾数
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の消費ＥＮの設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        wd.Bullet = MinLng(CLng(buf2), 99)
                    Else
                        DataErrorMessage wname & "の弾数の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            wd.Bullet = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                '消費ＥＮ
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の必要気力が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        wd.ENConsumption = MinLng(CLng(buf2), 999)
                    Else
                        DataErrorMessage wname & "の消費ＥＮの設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            wd.ENConsumption = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                '必要気力
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の地形適応が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        n = CLng(buf2)
                        If n > 1000 Then
                            n = 1000
                        ElseIf n < 0 Then
                            n = 0
                        End If
                        wd.NecessaryMorale = n
                    Else
                        DataErrorMessage wname & "の必要気力の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            wd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                '地形適応
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "のクリティカル率が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If Len(buf2) = 4 Then
                    wd.Adaption = buf2
                Else
                    DataErrorMessage wname & "の地形適応の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    wd.Adaption = "----"
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.Adaption = LIndex(buf2, 1)
                    End If
                End If
                
                'クリティカル率
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = wname & "の武器属性が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    n = CLng(buf2)
                    If n > 999 Then
                        n = 999
                    ElseIf n < -999 Then
                        n = -999
                    End If
                    wd.Critical = n
                Else
                    DataErrorMessage wname & "のクリティカル率の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.Critical = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '武器属性
                buf = Trim$(buf)
                If Len(buf) = 0 Then
                    DataErrorMessage wname & "の武器属性の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                End If
                If Right$(buf, 1) = ")" Then
                    '必要技能
                    ret = InStr(buf, "> ")
                    If ret > 0 Then
                        If ret > 0 Then
                            wd.NecessarySkill = Mid$(buf, ret + 2)
                            buf = Trim$(Left$(buf, ret + 1))
                            ret = InStr(wd.NecessarySkill, "(")
                            wd.NecessarySkill = _
                                Mid$(wd.NecessarySkill, ret + 1, Len(wd.NecessarySkill) - ret - 1)
                        End If
                    Else
                        ret = InStr(buf, "(")
                        If ret > 0 Then
                            wd.NecessarySkill = Trim$(Mid$(buf, ret + 1, Len(buf) - ret - 1))
                            buf = Trim$(Left$(buf, ret - 1))
                        End If
                    End If
                End If
                If Right$(buf, 1) = ">" Then
                    '必要条件
                    ret = InStr(buf, "<")
                    If ret > 0 Then
                        wd.NecessaryCondition = Trim$(Mid$(buf, ret + 1, Len(buf) - ret - 1))
                        buf = Trim$(Left$(buf, ret - 1))
                    End If
                End If
                wd.Class = buf
                If wd.Class = "-" Then
                    wd.Class = ""
                End If
                If InStr(wd.Class, "Lv") > 0 Then
                    DataErrorMessage wname & "の属性のレベル指定が間違っています。", _
                        fname, line_num, line_buf, .Name
                End If
                
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
            Loop
            
            If line_buf <> "===" Then
                GoTo SkipRest
            End If
            
            'アビリティデータ
            GetLine FileNumber, line_buf, line_num
            Do While Len(line_buf) > 0
                'アビリティ名
                ret = InStr(line_buf, ",")
                If ret = 0 Then
                    err_msg = "アビリティデータの終りに空行を置いてください。"
                    Error 0
                End If
                sname = Trim$(Left$(line_buf, ret - 1))
                buf = Mid$(line_buf, ret + 1)
                
                If sname = "" Then
                    err_msg = "アビリティ名の設定が間違っています。"
                    Error 0
                End If
                
                'アビリティを登録
                Set sd = .AddAbility(sname)
                
                '効果
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = sname & "の射程の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                sd.SetEffect buf2
                
                '射程
                sd.MinRange = 0
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = sname & "の回数の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If IsNumeric(buf2) Then
                    sd.MaxRange = MinLng(CLng(buf2), 99)
                ElseIf buf2 = "-" Then
                    sd.MaxRange = 0
                Else
                    DataErrorMessage sname & "の射程の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        sd.MaxRange = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '回数
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = sname & "の消費ＥＮの設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        sd.Stock = MinLng(CLng(buf2), 99)
                    Else
                        DataErrorMessage sname & "の回数の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            sd.Stock = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                '消費ＥＮ
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = sname & "の必要気力の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        sd.ENConsumption = MinLng(CLng(buf2), 999)
                    Else
                        DataErrorMessage sname & "の消費ＥＮの設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            sd.ENConsumption = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                '必要気力
                ret = InStr(buf, ",")
                If ret = 0 Then
                    err_msg = sname & "のアビリティ属性の設定が抜けています。"
                    Error 0
                End If
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                If buf2 <> "-" Then
                    If IsNumeric(buf2) Then
                        n = CLng(buf2)
                        If n > 1000 Then
                            n = 1000
                        ElseIf n < 0 Then
                            n = 0
                        End If
                        sd.NecessaryMorale = n
                    Else
                        DataErrorMessage sname & "の必要気力の設定が間違っています。", _
                            fname, line_num, line_buf, .Name
                        If LLength(buf2) > 1 Then
                            buf = LIndex(buf2, 2) & "," & buf
                            sd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
                        End If
                    End If
                End If
                
                'アビリティ属性
                buf = Trim$(buf)
                If Len(buf) = 0 Then
                    DataErrorMessage sname & "のアビリティ属性の設定が間違っています。", _
                        fname, line_num, line_buf, .Name
                End If
                If Right$(buf, 1) = ")" Then
                    '必要技能
                    ret = InStr(buf, "> ")
                    If ret > 0 Then
                        If ret > 0 Then
                            sd.NecessarySkill = Mid$(buf, ret + 2)
                            buf = Trim$(Left$(buf, ret + 1))
                            ret = InStr(sd.NecessarySkill, "(")
                            sd.NecessarySkill = _
                                Mid$(sd.NecessarySkill, ret + 1, Len(sd.NecessarySkill) - ret - 1)
                        End If
                    Else
                        ret = InStr(buf, "(")
                        If ret > 0 Then
                            sd.NecessarySkill = Trim$(Mid$(buf, ret + 1, Len(buf) - ret - 1))
                            buf = Trim$(Left$(buf, ret - 1))
                        End If
                    End If
                End If
                If Right$(buf, 1) = ">" Then
                    '必要条件
                    ret = InStr(buf, "<")
                    If ret > 0 Then
                        sd.NecessaryCondition = Trim$(Mid$(buf, ret + 1, Len(buf) - ret - 1))
                        buf = Trim$(Left$(buf, ret - 1))
                    End If
                End If
                sd.Class = buf
                If sd.Class = "-" Then
                    sd.Class = ""
                End If
                If InStr(sd.Class, "Lv") > 0 Then
                    DataErrorMessage sname & "の属性のレベル指定が間違っています。", _
                        fname, line_num, line_buf, .Name
                End If
                
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
            Loop
        End With
SkipRest:
    Loop
    
ErrorHandler:
    'エラー処理
    If line_num = 0 Then
        ErrorMessage fname & "が開けません。"
    Else
        Close #FileNumber
        DataErrorMessage err_msg, fname, line_num, line_buf, data_name
    End If
    TerminateSRC
End Sub

