VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "MessageDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全メッセージデータ(または特殊効果データ)を管理するコレクションクラス

'メッセージデータ(または特殊効果データ)一覧
Private colMessageDataList As New Collection

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colMessageDataList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colMessageDataList = Nothing
End Sub

'メッセージデータの追加
Public Function Add(mname As String) As MessageData
Dim new_md As New MessageData

    new_md.Name = mname
    colMessageDataList.Add new_md, mname
    Set Add = new_md
End Function

'メッセージデータの総数
Public Function Count() As Integer
    Count = colMessageDataList.Count
End Function

Public Sub Delete(Index As Variant)
    colMessageDataList.Remove Index
End Sub

'メッセージデータの検索
Public Function Item(Index As Variant) As MessageData
    On Error GoTo ErrorHandler
    
    Set Item = colMessageDataList.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item = Nothing
End Function

'メッセージデータが登録されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim md As MessageData

    On Error GoTo ErrorHandler
    Set md = colMessageDataList.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    IsDefined = False
End Function

'メッセージデータをファイルからロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim ret As Integer, line_num As Long
Dim line_buf As String, md As MessageData
Dim is_effect As Boolean
Dim sname As String, msg As String
Dim data_name As String
Dim err_msg As String

    '特殊効果データor戦闘アニメデータか？
    If InStr(LCase$(fname), "effect.txt") > 0 _
        Or InStr(LCase$(fname), "animation.txt") > 0 _
    Then
        is_effect = True
    End If
    
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
        
        '重複して定義されたデータの場合
        If IsDefined(data_name) Then
            If Not is_effect Then
                'パイロットメッセージの場合は後から定義されたものを優先
                Delete data_name
                Set md = Add(data_name)
            Else
                '特殊効果データの場合は既存のものに追加
                Set md = Item(data_name)
            End If
        Else
            Set md = Add(data_name)
        End If
        
        With md
            GetLine FileNumber, line_buf, line_num
            Do While Len(line_buf) > 0
                ret = InStr(line_buf, ",")
                
                If ret < 2 Then
                    Error 0
                End If
                
                sname = Left$(line_buf, ret - 1)
                msg = Trim$(Mid$(line_buf, ret + 1))
                
                If Len(sname) = 0 Then
                    err_msg = "シチュエーションの指定が抜けています。"
                    Error 0
                End If
                
                .AddMessage sname, msg
                
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
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
        DataErrorMessage "", fname, line_num, line_buf, data_name
    End If
    TerminateSRC
End Sub

'デフォルトの戦闘アニメデータを定義
Public Sub AddDefaultAnimation()
Dim md As MessageData
    
    'アニメデータが用意されていない場合は Data\System\animation.txt を読み込む
    If Count() = 0 Then
        If FileExists(AppPath & "Data\System\animation.txt") Then
            Load AppPath & "Data\System\animation.txt"
        End If
    End If
    
    If IsDefined("汎用") Then
        Set md = Item("汎用")
    Else
        Set md = Add("汎用")
    End If
    
    With md
        If FindNormalLabel("戦闘アニメ_回避発動") > 0 Then
            If .SelectMessage("回避") = "" Then
                .AddMessage "回避", "回避"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_切り払い発動") > 0 Then
            If .SelectMessage("切り払い") = "" Then
                .AddMessage "切り払い", "切り払い"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_迎撃発動") > 0 Then
            If .SelectMessage("迎撃") = "" Then
                .AddMessage "迎撃", "迎撃"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_ダミー発動") > 0 Then
            If .SelectMessage("ダミー") = "" Then
                .AddMessage "ダミー", "ダミー"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_修理装置発動") > 0 Then
            If .SelectMessage("修理") = "" Then
                .AddMessage "修理", "修理装置"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_補給装置発動") > 0 Then
            If .SelectMessage("補給") = "" Then
                .AddMessage "補給", "補給装置"
            End If
        End If
        If FindNormalLabel("戦闘アニメ_終了発動") > 0 Then
            If .SelectMessage("終了") = "" Then
                .AddMessage "終了", "終了"
            End If
        End If
    End With
End Sub
