VERSION 5.00
Begin VB.Form frmMultiColumnListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '固定(実線)
   Caption         =   "MultiColumListBox"
   ClientHeight    =   6690
   ClientLeft      =   1080
   ClientTop       =   1740
   ClientWidth     =   10050
   ClipControls    =   0   'False
   Icon            =   "MultiColumListBox.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   446
   ScaleMode       =   3  'ﾋﾟｸｾﾙ
   ScaleWidth      =   670
   Begin VB.ListBox lstItems 
      BackColor       =   &H00FFFFFF&
      Columns         =   4
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
      Height          =   6060
      ItemData        =   "MultiColumListBox.frx":030A
      Left            =   120
      List            =   "MultiColumListBox.frx":030C
      TabIndex        =   0
      Top             =   120
      Width           =   9810
   End
   Begin VB.Label labCaption 
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '実線
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
      Height          =   345
      Left            =   120
      TabIndex        =   1
      Top             =   6240
      Width           =   9810
   End
End
Attribute VB_Name = "frmMultiColumnListBox"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'多段のリストボックスのフォーム

'フォームを表示
Private Sub Form_Activate()
    SelectedItem = 0
    labCaption.Caption = ""
End Sub

'フォームをロード
Private Sub Form_Load()
Dim ret As Long
    
    '常に手前に表示
    ret = SetWindowPos(hwnd, -1, 0, 0, 0, 0, &H3)
End Sub

'フォームを閉じる
Private Sub Form_Unload(Cancel As Integer)
    TopItem = lstItems.TopIndex + 1
    IsFormClicked = True
    If Not IsMordal And Visible Then
        Cancel = 1
    End If
    Hide
End Sub

'フォーム上でマウスボタンを押す
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            '選択
            If Not Visible Then
                Exit Sub
            End If
            If lstItems.ListIndex < 0 _
                Or ListItemFlag(lstItems.ListIndex + 1) _
            Then
                Exit Sub
            End If
            SelectedItem = lstItems.ListIndex + 1
            TopItem = lstItems.TopIndex + 1
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
        Case 2
            'キャンセル
            SelectedItem = 0
            TopItem = lstItems.TopIndex + 1
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
    End Select
End Sub

'フォーム上でマウスボタンを押す
Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    If Button = 2 Then
        'キャンセルのみ受け付け
        SelectedItem = 0
        TopItem = lstItems.TopIndex
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'リストボックス上でマウスカーソルを移動
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim itm As Integer
Dim lines As Integer
    
    With lstItems
        'リストボックスの行数
        lines = 25
        If .ListCount > lines * .Columns Then
            lines = lines - 1
        End If
        
        'マウスカーソルがあるアイテムを算出
        itm = (((X * ScaleWidth) \ Width) \ (.Width \ .Columns)) * lines
        itm = itm + ((Y * ScaleWidth) \ Width + 1) \ 16
        itm = itm + .TopIndex
        
        'カーソル上のアイテムをハイライト表示
        If itm < 0 Or itm >= .ListCount Then
            .ListIndex = -1
            Exit Sub
        End If
        If .ListIndex = itm Then
            Exit Sub
        End If
        .ListIndex = itm
        
        '解説の表示
        labCaption.Caption = ListItemComment(itm + 1)
    End With
End Sub
