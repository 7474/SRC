VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ItemDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全アイテムデータを管理するリストのクラス

'アイテムデータのコレクション
Private colItemDataList As New Collection


'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colItemDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colItemDataList = Nothing
End Sub

'アイテムデータリストにデータを追加
Public Function Add(new_name As String) As ItemData
Dim new_Item_data As New ItemData

    new_Item_data.Name = new_name
    colItemDataList.Add new_Item_data, new_name
    Set Add = new_Item_data
End Function

'アイテムデータリストに登録されているデータの総数
Public Function Count() As Integer
    Count = colItemDataList.Count
End Function

'アイテムデータリストから指定したデータを削除
Public Sub Delete(Index As Variant)
    colItemDataList.Remove Index
End Sub

'アイテムデータリストから指定したデータを取り出す
Public Function Item(Index As Variant) As ItemData
    On Error GoTo ErrorHandler
    
    Set Item = colItemDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item = Nothing
End Function

'アイテムデータリストに指定したデータが登録されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim dummy As ItemData

    On Error GoTo ErrorHandler
    Set dummy = colItemDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim i As Integer, j As Integer, n As Long, line_num As Long
Dim line_buf As String, buf As String, buf2 As String
Dim ret As Integer
Dim nd As ItemData, wd As WeaponData, sd As AbilityData
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
        '空行をスキップ
        Do
            If EOF(FileNumber) Then
                Close #FileNumber
                Exit Sub
            End If
            GetLine FileNumber, line_buf, line_num
        Loop While Len(line_buf) = 0
        
        If InStr(line_buf, ",") > 0 Then
            err_msg = "名称の設定が抜けています。"
            Error 0
        End If
        
        '名称
        data_name = line_buf
        
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
            Delete data_name
        End If
        Set nd = Add(data_name)
        
        With nd
            '愛称、読み仮名、アイテムクラス、装備個所
            GetLine FileNumber, line_buf, line_num
            
            '書式チェックのため、コンマの数を数えておく
            comma_num = 0
            For i = 1 To Len(line_buf)
                If Mid$(line_buf, i, 1) = "," Then
                    comma_num = comma_num + 1
                End If
            Next
            
            If comma_num < 2 Then
                err_msg = "設定に抜けがあります。"
                Error 0
            ElseIf comma_num > 3 Then
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
            If comma_num = 3 Then
                ret = InStr(buf, ",")
                buf2 = Trim$(Left$(buf, ret - 1))
                buf = Mid$(buf, ret + 1)
                .KanaName = buf2
            Else
                .KanaName = StrToHiragana(.Nickname)
            End If
            
            'アイテムクラス
            ret = InStr(buf, ",")
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            .Class = buf2
            
            '装備個所
            buf2 = Trim$(buf)
            If Len(buf2) = 0 Then
                err_msg = "装備個所の設定が抜けています。"
                Error 0
            End If
            .Part = buf2
            
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
                        
                        If i = 1 Then
                            If IsNumeric(buf2) Then
                                Exit Do
                            End If
                        End If
                        
                        buf = Trim$(Mid$(buf, ret + 1))
                    Else
                        buf2 = buf
                        buf = ""
                    End If
                    
                    If buf2 <> "" Then
                        .AddFeature buf2
                    Else
                        DataErrorMessage "行頭から" & Format$(i) & _
                            "番目の特殊能力の設定が間違っています。", _
                            fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
                    End If
                Loop
                
                i = i + 1
                buf2 = Trim$(buf)
                If buf2 <> "" Then
                    .AddFeature buf2
                Else
                    DataErrorMessage Format$(i) & "番目の特殊能力の設定が間違っています。", _
                        fname, line_num, line_buf, data_name
                End If
                
                GetLine FileNumber, line_buf, line_num
            Else
                err_msg = "特殊能力の設定が抜けています。"
                Error 0
            End If
            
            '最大ＨＰ修正値, 最大ＥＮ修正値, 装甲修正値, 運動性修正値, 移動力修正値
            
            '最大ＨＰ修正値
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "最大ＥＮ修正値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            If IsNumeric(buf2) Then
                .HP = CLng(buf2)
            Else
                DataErrorMessage "最大ＨＰ修正値の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '最大ＥＮ修正値
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "装甲修正値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .EN = CLng(buf2)
            Else
                DataErrorMessage "最大ＥＮ修正値の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '装甲修正値
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "運動性修正値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .Armor = CLng(buf2)
            Else
                DataErrorMessage "装甲修正値の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '運動性修正値
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "移動力修正値の設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .Mobility = CLng(buf2)
            Else
                DataErrorMessage "運動性修正値の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '移動力修正値
            buf2 = Trim$(buf)
            If Len(buf2) = 0 Then
                err_msg = "移動力修正値の設定が抜けています。"
                Error 0
            End If
            If IsNumeric(buf2) Then
                .Speed = CLng(buf2)
            Else
                DataErrorMessage "移動力修正値の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            If EOF(FileNumber) Then
                Close #FileNumber
                Exit Sub
            End If
            
            '武器データ
            GetLine FileNumber, line_buf, line_num
            Do While Len(line_buf) > 0 And Left$(line_buf, 1) <> "*" And line_buf <> "==="
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
                        fname, line_num, line_buf, data_name
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
                    wd.MinRange = MinLng(CLng(buf2), 99)
                Else
                    DataErrorMessage wname & "の最小射程の設定が間違っています。", _
                        fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
                    If LLength(buf2) > 1 Then
                        buf = LIndex(buf2, 2) & "," & buf
                        wd.Critical = StrToLng(LIndex(buf2, 1))
                    End If
                End If
                
                '武器属性
                buf = Trim$(buf)
                If Len(buf) = 0 Then
                    DataErrorMessage wname & "の武器属性の設定が間違っています。", _
                        fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
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
            Do While Len(line_buf) > 0 And Left$(line_buf, 1) <> "*"
                'アビリティ名
                ret = InStr(line_buf, ",")
                If ret = 0 Then
                    err_msg = "アビリティデータの終りには空行を置いてください。"
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
                        fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                            fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
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
                        fname, line_num, line_buf, data_name
                End If
                
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
            Loop
            
SkipRest:
            
            '解説
            Do While Left$(line_buf, 1) = "*"
                If Len(.Comment) > 0 Then
                    .Comment = .Comment & vbCr & vbLf
                End If
                .Comment = .Comment & Mid$(line_buf, 2)
                If EOF(FileNumber) Then
                    Exit Do
                End If
                GetLine FileNumber, line_buf, line_num
            Loop
        End With
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

