VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "NonPilotDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全ノンパイロットデータを管理するリストのクラス

'ノンパイロットデータのコレクション
Private colNonPilotDataList As New Collection

'クラスの初期化
Private Sub Class_Initialize()
Dim npd As New NonPilotData
    
    'Talkコマンド用
    With npd
        .Name = "ナレーター"
        .Nickname = "ナレーター"
        .Bitmap = ".bmp"
    End With
    colNonPilotDataList.Add npd, npd.Name
End Sub

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer

    With colNonPilotDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colNonPilotDataList = Nothing
End Sub

'ノンパイロットデータリストにデータを追加
Public Function Add(pname As String) As NonPilotData
Dim new_pilot_data As New NonPilotData

    new_pilot_data.Name = pname
    colNonPilotDataList.Add new_pilot_data, pname
    Set Add = new_pilot_data
End Function

'ノンパイロットデータリストに登録されているデータの総数
Public Function Count() As Integer
    Count = colNonPilotDataList.Count
End Function

'ノンパイロットデータリストからデータを削除
Public Sub Delete(Index As Variant)
    colNonPilotDataList.Remove Index
End Sub

'ノンパイロットデータリストからデータを取り出す
Public Function Item(Index As Variant) As NonPilotData
Dim pd As NonPilotData
Dim pname As String

    On Error GoTo ErrorHandler
    Set Item = colNonPilotDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    pname = CStr(Index)
    For Each pd In colNonPilotDataList
        If pd.Nickname0 = pname Then
            Set Item = pd
            Exit Function
        End If
    Next
End Function

'ノンパイロットデータリストに指定したデータが定義されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim pd As NonPilotData
Dim pname As String

    On Error GoTo ErrorHandler
    Set pd = colNonPilotDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    pname = CStr(Index)
    For Each pd In colNonPilotDataList
        If pd.Nickname0 = pname Then
            IsDefined = True
            Exit Function
        End If
    Next
    IsDefined = False
End Function

'ノンパイロットデータリストに指定したデータが定義されているか？ (愛称は見ない)
Public Function IsDefined2(Index As Variant) As Boolean
Dim pd As NonPilotData

    On Error GoTo ErrorHandler
    Set pd = colNonPilotDataList.Item(Index)
    IsDefined2 = True
    Exit Function
    
ErrorHandler:
    IsDefined2 = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim line_num As Long
Dim line_buf As String, buf As String, buf2 As String
Dim ret As Integer
Dim pd As New NonPilotData
Dim data_name As String
Dim err_msg As String
    
    On Error GoTo ErrorHandler
    
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    line_num = 0
    
    Do While True
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
            'すでに定義されているノンパイロットのデータであれば置き換える
            If Item(data_name).Name = data_name Then
                Set pd = Item(data_name)
            Else
                Set pd = Add(data_name)
            End If
        Else
            Set pd = Add(data_name)
        End If
        
        With pd
            '愛称、ビットマップ
            GetLine FileNumber, line_buf, line_num
            
            '愛称
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "ビットマップの設定が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            If buf2 <> "" Then
                .Nickname = buf2
            Else
                err_msg = "愛称の設定が間違っています。"
                Error 0
            End If
            
            'ビットマップ
            buf2 = Trim$(buf)
            If LCase$(Right$(buf2, 4)) = ".bmp" Then
                .Bitmap = buf2
            Else
                DataErrorMessage "ビットマップの設定が間違っています。", _
                    fname, line_num, line_buf, .Name
            End If
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

