VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Items"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'アイテムＩＤ作成用カウンタ
Private IDCount As Long

'アイテム一覧
Private colItems As New Collection

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colItems
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colItems = Nothing
End Sub

'ForEach用関数
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = colItems.[_NewEnum]
End Function


'リストにアイテムを追加
Public Function Add(Name As String) As Item
Dim new_item As Item
    
    If Not IDList.IsDefined(Name) Then
        Set Add = Nothing
        Exit Function
    End If
    
    Set new_item = New Item
    Set Add = new_item
    With new_item
        .Name = Name
        .ID = CreateID(Name)
    End With
    colItems.Add new_item, new_item.ID
End Function

'新しいアイテムＩＤを作成
Private Function CreateID(iname As String) As String
    Do
        IDCount = IDCount + 1
    Loop Until Not IsDefined2(iname & "_" & Format$(IDCount))
    CreateID = iname & "_" & Format$(IDCount)
End Function

'リストに登録されているアイテムの総数
Public Function Count() As Integer
    Count = colItems.Count
End Function

'リストからアイテムを削除
Public Sub Delete(Index As Variant)
    colItems.Remove Index
End Sub

'指定されたアイテムを検索
Public Function Item(Index As Variant) As Item
Dim it As Item
Dim iname As String

    On Error GoTo ErrorHandler
    Set Item = colItems.Item(Index)
    
    '破棄されていない？
    If Item.Exist Then
        Exit Function
    End If

ErrorHandler:
    '見つからなければアイテム名で検索
    iname = CStr(Index)
    For Each it In colItems
        With it
            If .Name = iname And .Exist Then
                Set Item = it
                Exit Function
            End If
        End With
    Next
    Set Item = Nothing
End Function

'指定されたアイテムが登録されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim it As Item
Dim iname As String

    On Error GoTo ErrorHandler
    Set it = colItems.Item(Index)
    
    '破棄されたアイテムは登録されていないとみなす
    If it.Exist Then
        IsDefined = True
        Exit Function
    End If
    
ErrorHandler:
    '見つからなければアイテム名で検索
    iname = CStr(Index)
    For Each it In colItems
        With it
            If .Name = iname And .Exist Then
                IsDefined = True
                Exit Function
            End If
        End With
    Next
    IsDefined = False
End Function

'アイテム名とExitフラグを無視してアイテムを検索
Public Function IsDefined2(Index As Variant) As Boolean
Dim it As Item

    On Error GoTo ErrorHandler
    Set it = colItems.Item(Index)
    IsDefined2 = True
    Exit Function

ErrorHandler:
    IsDefined2 = False
End Function

'リストに登録されたアイテムをアップデート
Public Sub Update()
Dim it As Item
    
    '破棄されたアイテムを削除
    For Each it In colItems
        With it
            If Not .Exist Then
                colItems.Remove .ID
            End If
        End With
    Next
    
    'リンクデータの整合性を取る
    For Each it In colItems
        With it
            If Not .Unit Is Nothing Then
                Set .Unit = UList.Item(.Unit.ID)
            End If
        End With
    Next
End Sub


'データをファイルにセーブ
Public Sub Save()
Dim i As Integer

    Write #SaveDataFileNumber, IDCount
    Write #SaveDataFileNumber, Count
    For i = 1 To Count
        With Item(i)
            Write #SaveDataFileNumber, .Name
            If .Unit Is Nothing Then
                Write #SaveDataFileNumber, .ID, "-"
            Else
                Write #SaveDataFileNumber, .ID, .Unit.ID
            End If
        End With
    Next
End Sub

'データをファイルからロード
Public Sub Load()
Dim num As Integer
Dim new_item As Item
Dim iname As String
Dim iid As String
Dim i As Integer
Dim dummy As String

    If EOF(SaveDataFileNumber) Then
        Exit Sub
    End If
    
    Input #SaveDataFileNumber, IDCount
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Set new_item = New Item
        With new_item
            'Name
            Input #SaveDataFileNumber, iname
            'ID, Unit
            Input #SaveDataFileNumber, iid, dummy
            
            If Not IDList.IsDefined(iname) Then
                ErrorMessage iname & "のデータが定義されていません"
                StopBGM
                End
            End If
            
            .Name = iname
            .ID = iid
        End With
        colItems.Add new_item, iid
    Next
End Sub

'リンク情報をファイルからロード
Public Sub LoadLinkInfo()
Dim num As Integer, i As Integer
Dim dummy As String

    If EOF(SaveDataFileNumber) Then
        Exit Sub
    End If
    
    'IDCount
    Line Input #SaveDataFileNumber, dummy
    
    Input #SaveDataFileNumber, num
    For i = 1 To num
        'Name
        Line Input #SaveDataFileNumber, dummy
        'ID, Unit
        Line Input #SaveDataFileNumber, dummy
    Next
End Sub


'一時中断用データをファイルにセーブする
Public Sub Dump()
Dim it As Item

    Write #SaveDataFileNumber, Count
    For Each it In colItems
        it.Dump
    Next
End Sub

'一時中断用データをファイルからロードする
Public Sub Restore()
Dim i As Integer, num As Integer
Dim it As Item
    
    With colItems
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        Set it = New Item
        With it
            .Restore
            colItems.Add it, .ID
        End With
    Next
End Sub

'一時中断用データのリンク情報をファイルからロードする
Public Sub RestoreLinkInfo()
Dim it As Item
Dim num As Integer

    Input #SaveDataFileNumber, num
    
    For Each it In colItems
        it.RestoreLinkInfo
    Next
End Sub

'一時中断用データのパラメータ情報をファイルからロードする
Public Sub RestoreParameter()
Dim it As Item
Dim num As Integer

    Input #SaveDataFileNumber, num
    
    For Each it In colItems
        it.RestoreParameter
    Next
End Sub


'リストをクリア
Public Sub Clear()
Dim i As Integer

    For i = 1 To Count
        Delete 1
    Next
End Sub


