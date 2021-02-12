VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AliasDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全エリアスデータを管理するリストのクラス


'エリアスデータのコレクション
Private colAliasDataList As New Collection


'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer

    With colAliasDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colAliasDataList = Nothing
End Sub

'エリアスデータリストにデータを追加
Public Function Add(aname As String) As AliasData
Dim ad As New AliasData

    ad.Name = aname
    colAliasDataList.Add ad, aname
    Set Add = ad
End Function

'エリアスデータリストに登録されているデータの総数
Public Function Count() As Integer
    Count = colAliasDataList.Count
End Function

'エリアスデータリストからデータを削除
Public Sub Delete(Index As Variant)
    colAliasDataList.Remove Index
End Sub

'エリアスデータリストからデータを取り出す
Public Function Item(Index As Variant) As AliasData
    Set Item = colAliasDataList.Item(Index)
End Function

'エリアスデータリストに指定したデータが定義されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim ad As AliasData

    On Error GoTo ErrorHandler
    Set ad = colAliasDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim line_num As Long
Dim line_buf As String
Dim ad As AliasData
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
        
        If InStr(line_buf, ",") > 0 Then
            err_msg = "名称の設定が抜けています。"
            Error 0
        End If
        
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
            'すでに定義されているエリアスのデータであれば置き換える
            Delete data_name
        End If
        Set ad = Add(data_name)
        
        With ad
            Do While True
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
                
                If Len(line_buf) = 0 Then
                    Exit Do
                End If
                
                .AddAlias line_buf
            Loop
            If .Count = 0 Then
                err_msg = "エリアス対象のデータが定義されていません。"
                Error 0
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

'ForEach用関数
Public Function NewEnum() As IUnknown
    Set NewEnum = colAliasDataList.[_NewEnum]
End Function

