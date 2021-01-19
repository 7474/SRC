VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BattleConfigDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'バトルコンフィグデータを管理するクラス
' --- ダメージ計算、命中率算出など、バトルに関連するエリアスの定義を設定します。

'バトルコンフィグデータのコレクション
Private colBattleConfigData As New Collection


'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer

    With colBattleConfigData
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colBattleConfigData = Nothing
End Sub

'バトルコンフィグデータリストにデータを追加
Public Function Add(cname As String) As BattleConfigData
Dim cd As New BattleConfigData

    cd.Name = cname
    colBattleConfigData.Add cd, cname
    Set Add = cd
End Function

'バトルコンフィグデータリストに登録されているデータの総数
Public Function Count() As Integer
    Count = colBattleConfigData.Count
End Function

'バトルコンフィグデータリストからデータを削除
Public Sub Delete(Index As Variant)
    colBattleConfigData.Remove Index
End Sub

'バトルコンフィグデータリストからデータを取り出す
Public Function Item(Index As String) As BattleConfigData
    Set Item = colBattleConfigData.Item(Index)
End Function

'バトルコンフィグデータリストに指定したデータが定義されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim cd As BattleConfigData

    On Error GoTo ErrorHandler
    Set cd = colBattleConfigData.Item(Index)
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
Dim cd As BattleConfigData
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
        
        data_name = line_buf
        
        If IsDefined(data_name) Then
            'すでに定義されているエリアスのデータであれば置き換える
            Delete data_name
        End If
        Set cd = Add(data_name)
        
        With cd
            Do While True
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
                
                If Len(line_buf) = 0 Then
                    Exit Do
                End If
                
                .ConfigCalc = line_buf
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

