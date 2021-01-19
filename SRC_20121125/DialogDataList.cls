VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DialogDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全ダイアログデータを管理するリストのクラス

'ダイアログデータのコレクション
Private colDialogDataList As New Collection

'クラスを解放
Private Sub Class_Terminate()
Dim i As Integer

    With colDialogDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colDialogDataList = Nothing
End Sub

'ダイアログデータを追加
Public Function Add(dname As String) As DialogData
Dim new_dd As New DialogData

    new_dd.Name = dname
    colDialogDataList.Add new_dd, dname
    Set Add = new_dd
End Function

'ダイアログデータの総数
Public Function Count() As Integer
    Count = colDialogDataList.Count
End Function

Public Sub Delete(Index As Variant)
    colDialogDataList.Remove Index
End Sub

'ダイアログデータを返す
Public Function Item(Index As Variant) As DialogData
    On Error GoTo ErrorHandler
    
    Set Item = colDialogDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item = Nothing
End Function

'指定したダイアログデータが定義されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim dummy As DialogData

    On Error GoTo ErrorHandler
    Set dummy = colDialogDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim i As Integer, ret As Integer
Dim line_num As Long
Dim line_buf As String
Dim pilot_list As String
Dim d As Dialog, dd As DialogData
Dim err_msg As String
Dim pname As String, msg As String

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
        
        Set dd = Nothing
        
        'パイロット名一覧
        If LLength(line_buf) = 0 Then
            Error 0
        End If
        pilot_list = ""
        For i = 1 To LLength(line_buf)
            pilot_list = pilot_list & " " & LIndex(line_buf, i)
        Next
        pilot_list = Trim$(pilot_list)
        
        If IsDefined(pilot_list) Then
            Delete pilot_list
        End If
        Set dd = Add(pilot_list)
        
        GetLine FileNumber, line_buf, line_num
        Do While Len(line_buf) > 0
            'シチューション
            Set d = dd.AddDialog(line_buf)
            
            Do While True
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
                
                '話者
                ret = InStr(line_buf, ",")
                If ret = 0 Then
                    Exit Do
                End If
                pname = Left$(line_buf, ret - 1)
                
                '指定した話者のデータが存在するかチェック。
                'ただし合体技のパートナーは場合は他の作品のパイロットであることも
                'あるので話者チェックを行わない。
                If Left$(pname, 1) <> "@" Then
                    If Not PDList.IsDefined(pname) _
                        And Not NPDList.IsDefined(pname) _
                        And pname <> "システム" _
                    Then
                        err_msg = "パイロット「" & pname & "」が定義されていません。"
                        Error 0
                    End If
                End If
                
                'メッセージ
                If Len(line_buf) = ret Then
                    err_msg = "メッセージが定義されていません。"
                    Error 0
                End If
                msg = Trim$(Mid$(line_buf, ret + 1))
                
                d.AddMessage pname, msg
            Loop
        Loop
    Loop
    
ErrorHandler:
    If line_num = 0 Then
        ErrorMessage fname & "が開けません。"
    Else
        Close #FileNumber
        If dd Is Nothing Then
            DataErrorMessage err_msg, fname, line_num, line_buf, ""
        Else
            DataErrorMessage err_msg, fname, line_num, line_buf, dd.Name
        End If
    End If
    TerminateSRC
End Sub


