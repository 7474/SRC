VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "SpecialPowerDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全スペシャルパワーデータを管理するリストのクラス

'スペシャルパワーデータのコレクション
Private colSpecialPowerDataList As New Collection


'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colSpecialPowerDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colSpecialPowerDataList = Nothing
End Sub

'スペシャルパワーデータリストにデータを追加
Public Function Add(sname As String) As SpecialPowerData
Dim new_data As New SpecialPowerData

    new_data.Name = sname
    colSpecialPowerDataList.Add new_data, sname
    Set Add = new_data
End Function

'スペシャルパワーデータリストに登録されているデータの総数
Public Function Count() As Integer
    Count = colSpecialPowerDataList.Count
End Function

'スペシャルパワーデータリストから指定したデータを削除
Public Sub Delete(Index As Variant)
    colSpecialPowerDataList.Remove Index
End Sub

'スペシャルパワーデータリストから指定したデータを取り出す
Public Function Item(Index As Variant) As SpecialPowerData
    On Error GoTo ErrorHandler
    
    Set Item = colSpecialPowerDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item = Nothing
End Function

'スペシャルパワーデータリストに指定したデータが登録されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim dummy As SpecialPowerData

    On Error GoTo ErrorHandler
    Set dummy = colSpecialPowerDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim ret As Integer, line_num As Long
Dim line_buf As String, buf As String, buf2 As String
Dim sd As SpecialPowerData
Dim data_name As String
Dim err_msg As String

    On Error GoTo ErrorHandler
    
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    line_num = 0
    
    Do While True
        data_name = ""
        
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
            Delete data_name
        End If
        Set sd = Add(data_name)
        
        With sd
            '読み仮名
            ret = InStr(buf, ",")
            If ret > 0 Then
                err_msg = "読み仮名の後に余分なデータが指定されています。"
                Error 0
            End If
            .KanaName = Trim$(buf)
            If .KanaName = "" Then
                .KanaName = StrToHiragana(data_name)
            End If
            
            '短縮形, 消費ＳＰ, 対象, 効果時間, 適用条件, 使用条件, アニメ
            GetLine FileNumber, line_buf, line_num
            
            '短縮形
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "消費ＳＰが抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            .ShortName = buf2
            
            '消費ＳＰ
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "対象が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .SPConsumption = CInt(buf2)
            Else
                DataErrorMessage "消費ＳＰの設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '対象
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "効果時間が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                err_msg = "対象が間違っています。"
                Error 0
            End If
            .TargetType = buf2
            
            '効果時間
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "効果時間が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                err_msg = "効果時間が間違っています。"
                Error 0
            End If
            .Duration = buf2
            
            '適用条件
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "使用条件が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                err_msg = "適用条件が間違っています。"
                Error 0
            End If
            .NecessaryCondition = buf2
            
            '使用条件, アニメ
            ret = InStr(buf, ",")
            If ret > 0 Then
                .Animation = Trim$(Mid$(buf, InStr(buf, ",") + 1))
            Else
                .Animation = .Name
            End If
            
            '効果
            GetLine FileNumber, line_buf, line_num
            If Len(line_buf) = 0 Then
                err_msg = "効果が指定されていません。"
                Error 0
            End If
            .SetEffect line_buf
            
            '解説
            GetLine FileNumber, line_buf, line_num
            If Len(line_buf) = 0 Then
                err_msg = "解説が指定されていません。"
                Error 0
            End If
            .Comment = line_buf
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

