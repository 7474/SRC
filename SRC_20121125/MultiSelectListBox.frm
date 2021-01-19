VERSION 5.00
Begin VB.Form frmMultiSelectListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '固定ﾀﾞｲｱﾛｸﾞ
   Caption         =   "MultiSelectListBox"
   ClientHeight    =   4965
   ClientLeft      =   900
   ClientTop       =   2475
   ClientWidth     =   7455
   ClipControls    =   0   'False
   BeginProperty Font 
      Name            =   "ＭＳ ゴシック"
      Size            =   12
      Charset         =   128
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   Icon            =   "MultiSelectListBox.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   331
   ScaleMode       =   3  'ﾋﾟｸｾﾙ
   ScaleWidth      =   497
   Begin VB.CommandButton cmdResume 
      BackColor       =   &H00C0C0C0&
      Caption         =   "マップを見る"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   1440
      TabIndex        =   7
      TabStop         =   0   'False
      Top             =   4440
      Width           =   2055
   End
   Begin VB.CommandButton cmdSort 
      BackColor       =   &H00C0C0C0&
      Caption         =   "名称順に並べ替え"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   5040
      TabIndex        =   6
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2175
   End
   Begin VB.CommandButton cmdSelectAll2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "最後から選択"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   2520
      Style           =   1  'ｸﾞﾗﾌｨｯｸｽ
      TabIndex        =   5
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2415
   End
   Begin VB.CommandButton cmdSelectAll 
      BackColor       =   &H00C0C0C0&
      Caption         =   "先頭から選択"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   120
      Style           =   1  'ｸﾞﾗﾌｨｯｸｽ
      TabIndex        =   4
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2295
   End
   Begin VB.Timer Timer1 
      Interval        =   100
      Left            =   6720
      Top             =   2040
   End
   Begin VB.CommandButton cmdFinish 
      BackColor       =   &H00C0C0C0&
      Caption         =   "終了"
      Enabled         =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   3960
      Style           =   1  'ｸﾞﾗﾌｨｯｸｽ
      TabIndex        =   1
      TabStop         =   0   'False
      Top             =   4440
      Width           =   2055
   End
   Begin VB.ListBox lstItems 
      BackColor       =   &H00FFFFFF&
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   12
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   3420
      Left            =   120
      TabIndex        =   0
      Top             =   480
      Width           =   7185
   End
   Begin VB.Label lblNumber 
      Alignment       =   2  '中央揃え
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '実線
      Caption         =   "Label1"
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   15.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   435
      Left            =   6360
      TabIndex        =   3
      Top             =   4440
      Width           =   855
   End
   Begin VB.Label lblCaption 
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '実線
      Caption         =   "Label1"
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   12
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   330
      Left            =   120
      TabIndex        =   2
      Top             =   90
      Width           =   7185
   End
End
Attribute VB_Name = "frmMultiSelectListBox"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'出撃ユニット選択用リストボックスのフォーム

'選択されたユニットの数
Private SelectedItemNum As Integer
'ユニットが選択されたかどうかを示すフラグ
Private ItemFlag() As Boolean

'選択終了ボタンをクリック
Private Sub cmdFinish_Click()
Dim i As Integer

    For i = 1 To UBound(ListItemFlag)
        ListItemFlag(i) = ItemFlag(i - 1)
    Next
    ClearUnitStatus
    Unload frmMultiSelectListBox
End Sub

'マップを見るボタンをクリック
Private Sub cmdResume_Click()
Dim i As Integer

    For i = 1 To UBound(ListItemFlag)
        ListItemFlag(i) = False
    Next
    ClearUnitStatus
    Unload frmMultiSelectListBox
End Sub

'「先頭から選択」ボタンをクリック
Private Sub cmdSelectAll_Click()
Dim i As Integer

    lstItems.Visible = False
    For i = 1 To lstItems.ListCount
        ItemFlag(i - 1) = False
        lstItems.list(i - 1) = "　" & Mid$(lstItems.list(i - 1), 2)
    Next
    For i = 1 To MinLng(MaxListItem, lstItems.ListCount)
        If Not ItemFlag(i - 1) Then
            ItemFlag(i - 1) = True
            lstItems.list(i - 1) = "○" & Mid$(lstItems.list(i - 1), 2)
        End If
    Next
    lstItems.TopIndex = 0
    lstItems.Visible = True
    
    SelectedItemNum = 0
    For i = 0 To lstItems.ListCount - 1
        If ItemFlag(i) Then
            SelectedItemNum = SelectedItemNum + 1
        End If
    Next
    lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
    
    If SelectedItemNum > 0 _
        And SelectedItemNum <= MaxListItem _
    Then
        If Not cmdFinish.Enabled Then
            cmdFinish.Enabled = True
        End If
    Else
        If cmdFinish.Enabled Then
            cmdFinish.Enabled = False
        End If
    End If
End Sub

'「最後から選択」ボタンをクリック
Private Sub cmdSelectAll2_Click()
Dim i As Integer

    lstItems.Visible = False
    For i = 1 To lstItems.ListCount
        ItemFlag(i - 1) = False
        lstItems.list(i - 1) = "　" & Mid$(lstItems.list(i - 1), 2)
    Next
    For i = 1 To MinLng(MaxListItem, lstItems.ListCount)
        If Not ItemFlag(lstItems.ListCount - i) Then
            ItemFlag(lstItems.ListCount - i) = True
            lstItems.list(lstItems.ListCount - i) = _
                "○" & Mid$(lstItems.list(lstItems.ListCount - i), 2)
        End If
    Next
    lstItems.TopIndex = MaxLng(lstItems.ListCount - 14, 0)
    lstItems.Visible = True
    
    SelectedItemNum = 0
    For i = 0 To lstItems.ListCount - 1
        If ItemFlag(i) Then
            SelectedItemNum = SelectedItemNum + 1
        End If
    Next
    lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
    
    If SelectedItemNum > 0 _
        And SelectedItemNum <= MaxListItem _
    Then
        If Not cmdFinish.Enabled Then
            cmdFinish.Enabled = True
        End If
    Else
        If cmdFinish.Enabled Then
            cmdFinish.Enabled = False
        End If
    End If
End Sub

'「～順」ボタンをクリック
Private Sub cmdSort_Click()
Dim item_list() As String, key_list() As Long, strkey_list() As String
Dim max_item As Integer, max_value As Long, max_str As String
Dim i As Integer, j As Integer, buf As String, flag As Boolean
    
    '現在のリスト表示内容をコピー
    With lstItems
        ReDim item_list(.ListCount)
        For i = 1 To .ListCount
            item_list(i) = .list(i - 1)
        Next
    End With
    
    If cmdSort.Caption = "レベル順に並べ替え" Then
        'メインパイロットのレベル順に並べ替え
        ReDim key_list(UBound(item_list))
        With UList
            For i = 1 To UBound(item_list)
                With .Item(ListItemID(i)).MainPilot
                    key_list(i) = 500 * CLng(.Level) + CLng(.Exp)
                End With
            Next
        End With
        For i = 1 To UBound(item_list) - 1
            max_item = i
            max_value = key_list(i)
            For j = i + 1 To UBound(item_list)
                If key_list(j) > max_value Then
                    max_item = j
                    max_value = key_list(j)
                End If
            Next
            If max_item <> i Then
                buf = item_list(i)
                item_list(i) = item_list(max_item)
                item_list(max_item) = buf
                
                buf = ListItemID(i)
                ListItemID(i) = ListItemID(max_item)
                ListItemID(max_item) = buf
                
                flag = ItemFlag(i - 1)
                ItemFlag(i - 1) = ItemFlag(max_item - 1)
                ItemFlag(max_item - 1) = flag
                
                key_list(max_item) = key_list(i)
            End If
        Next
        '並べ替え方法をトグルで切り替え
        cmdSort.Caption = "名称順に並べ替え"
    Else
        'ユニットの名称順に並べ替え
        ReDim strkey_list(UBound(item_list))
        With UList
            For i = 1 To UBound(item_list)
                strkey_list(i) = .Item(ListItemID(i)).KanaName
            Next
        End With
        For i = 1 To UBound(item_list) - 1
            max_item = i
            max_str = strkey_list(i)
            For j = i + 1 To UBound(item_list)
                If StrComp(strkey_list(j), max_str, 1) = -1 Then
                    max_item = j
                    max_str = strkey_list(j)
                End If
            Next
            If max_item <> i Then
                buf = item_list(i)
                item_list(i) = item_list(max_item)
                item_list(max_item) = buf
                
                buf = ListItemID(i)
                ListItemID(i) = ListItemID(max_item)
                ListItemID(max_item) = buf
                
                flag = ItemFlag(i - 1)
                ItemFlag(i - 1) = ItemFlag(max_item - 1)
                ItemFlag(max_item - 1) = flag
                
                strkey_list(max_item) = strkey_list(i)
            End If
        Next
        '並べ替え方法をトグルで切り替え
        cmdSort.Caption = "レベル順に並べ替え"
    End If
    
    'リスト表示を更新する
    With lstItems
        .Visible = False
        For i = 1 To .ListCount
            .list(i - 1) = item_list(i)
        Next
        .TopIndex = 0
        .Visible = True
    End With
End Sub

'フォームを表示
Private Sub Form_Activate()
    SelectedItemNum = 0
    lblNumber = "0/" & Format$(MaxListItem)
    ReDim ItemFlag(lstItems.ListCount)
    If lstItems.ListCount > 0 Then
        DisplayUnitStatus UList.Item(ListItemID(1))
    End If
End Sub

'リストボックス上でダブルクリック
Private Sub lstItems_DblClick()
Dim i As Integer

    i = lstItems.ListIndex
    If i >= 0 Then
        If ItemFlag(i) Then
            '選択取り消し
            
            '選択されたユニット数を減らす
            SelectedItemNum = SelectedItemNum - 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = False
            
            '選択状態の表示を更新
            lstItems.list(i) = "　" & Mid$(lstItems.list(i), 2)
            
            '選択終了が可能か判定
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        Else
            '選択
            
            '選択されたユニット数を増やす
            SelectedItemNum = SelectedItemNum + 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = True
            
            '選択状態の表示を更新
            lstItems.list(i) = "○" & Mid$(lstItems.list(i), 2)
            
            '選択終了が可能か判定
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        End If
    End If
End Sub

'リストボックス上でマウスボタンを押す
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim i As Integer
    
    '左クリック以外は無視
    If Button <> 1 Then
        Exit Sub
    End If
    
    i = lstItems.ListIndex
    If i >= 0 Then
        If ItemFlag(i) Then
            '選択取り消し
            
            '選択されたユニット数を減らす
            SelectedItemNum = SelectedItemNum - 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = False
            
            '選択状態の表示を更新
            lstItems.list(i) = "　" & Mid$(lstItems.list(i), 2)
            
            '選択終了が可能か判定
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        Else
            '選択
            
            '選択されたユニット数を増やす
            SelectedItemNum = SelectedItemNum + 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = True
            
            '選択状態の表示を更新
            lstItems.list(i) = "○" & Mid$(lstItems.list(i), 2)
            
            '選択終了が可能か判定
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        End If
    End If
End Sub

'リストボックス上でマウスカーソルを移動
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Dim itm As Integer
    
    'カーソルがあるアイテムを算出
    itm = ((Y * ScaleWidth) \ Width + 1) \ 16
    itm = itm + lstItems.TopIndex
    
    'カーソルがあるアイテムをハイライト表示
    If itm < 0 Or itm >= lstItems.ListCount Then
        lstItems.ListIndex = -1
        Exit Sub
    End If
    If lstItems.ListIndex = itm Then
        Exit Sub
    End If
    lstItems.ListIndex = itm
End Sub

'カーソルが指すユニットを一定時間ごとに調べてステータスウィンドウに表示
Private Sub Timer1_Timer()
Dim u As Unit

    If Not Visible Then
        Exit Sub
    End If
    
    If lstItems.ListIndex = -1 Then
        Exit Sub
    End If
    
    Set u = UList.Item(ListItemID(lstItems.ListIndex + 1))
    
    If Not DisplayedUnit Is u Then
'ユニット選択中だけ
        If CommandState = "ユニット選択" Then
           DisplayUnitStatus u
        End If
    End If
End Sub
