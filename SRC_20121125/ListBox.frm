VERSION 5.00
Begin VB.Form frmListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '固定(実線)
   Caption         =   "ListBox"
   ClientHeight    =   2055
   ClientLeft      =   1080
   ClientTop       =   1740
   ClientWidth     =   9810
   ClipControls    =   0   'False
   Icon            =   "ListBox.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   137
   ScaleMode       =   3  'ﾋﾟｸｾﾙ
   ScaleWidth      =   654
   Begin VB.Timer Timer2 
      Interval        =   100
      Left            =   5400
      Top             =   1320
   End
   Begin VB.TextBox txtComment 
      Enabled         =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ ゴシック"
         Size            =   12
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   570
      Left            =   90
      MultiLine       =   -1  'True
      TabIndex        =   24
      TabStop         =   0   'False
      Top             =   2115
      Visible         =   0   'False
      Width           =   9555
   End
   Begin VB.TextBox txtMorale2 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   5775
      TabIndex        =   22
      Text            =   "100"
      Top             =   345
      Visible         =   0   'False
      Width           =   375
   End
   Begin VB.TextBox txtMorale1 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   885
      TabIndex        =   20
      Text            =   "100"
      Top             =   330
      Visible         =   0   'False
      Width           =   375
   End
   Begin VB.TextBox txtLevel2 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   5865
      TabIndex        =   19
      Text            =   "99"
      Top             =   105
      Visible         =   0   'False
      Width           =   255
   End
   Begin VB.TextBox txtLevel1 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   990
      TabIndex        =   17
      Text            =   "99"
      Top             =   90
      Visible         =   0   'False
      Width           =   255
   End
   Begin VB.TextBox txtHP1 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   2220
      TabIndex        =   11
      Text            =   "99999/99999"
      Top             =   120
      Visible         =   0   'False
      Width           =   1320
   End
   Begin VB.PictureBox picHP1 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   1830
      ScaleHeight     =   4
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   110
      TabIndex        =   10
      Top             =   390
      Visible         =   0   'False
      Width           =   1710
   End
   Begin VB.PictureBox picEN1 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   3600
      ScaleHeight     =   4
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   75
      TabIndex        =   9
      Top             =   390
      Visible         =   0   'False
      Width           =   1185
   End
   Begin VB.TextBox txtEN1 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   3945
      TabIndex        =   8
      Text            =   "999/999"
      Top             =   120
      Visible         =   0   'False
      Width           =   855
   End
   Begin VB.TextBox txtEN2 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   8805
      TabIndex        =   7
      Text            =   "999/999"
      Top             =   120
      Visible         =   0   'False
      Width           =   855
   End
   Begin VB.PictureBox picEN2 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   8475
      ScaleHeight     =   4
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   74
      TabIndex        =   6
      Top             =   405
      Visible         =   0   'False
      Width           =   1170
   End
   Begin VB.PictureBox picHP2 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   6735
      ScaleHeight     =   4
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   108
      TabIndex        =   5
      Top             =   405
      Visible         =   0   'False
      Width           =   1680
   End
   Begin VB.TextBox txtHP2 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   195
      Left            =   7095
      TabIndex        =   4
      Text            =   "99999/99999"
      Top             =   120
      Visible         =   0   'False
      Width           =   1320
   End
   Begin VB.PictureBox picUnit2 
      Appearance      =   0  'ﾌﾗｯﾄ
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   6180
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   3
      Top             =   75
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picUnit1 
      Appearance      =   0  'ﾌﾗｯﾄ
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   1275
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   2
      Top             =   60
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   100
      Left            =   4620
      Top             =   1275
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
      Height          =   1500
      Left            =   90
      TabIndex        =   0
      Top             =   480
      Width           =   9645
   End
   Begin VB.PictureBox picBar 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      ClipControls    =   0   'False
      FillColor       =   &H00800000&
      FillStyle       =   0  '塗りつぶし
      ForeColor       =   &H00800000&
      Height          =   195
      Left            =   90
      ScaleHeight     =   9
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   639
      TabIndex        =   25
      Top             =   1845
      Visible         =   0   'False
      Width           =   9645
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
      Left            =   90
      TabIndex        =   1
      Top             =   75
      Width           =   9645
   End
   Begin VB.Label labMorale2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "M"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   11.25
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   5580
      TabIndex        =   23
      Top             =   330
      Visible         =   0   'False
      Width           =   180
   End
   Begin VB.Label labMorale1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "M"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   11.25
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   690
      TabIndex        =   21
      Top             =   300
      Visible         =   0   'False
      Width           =   180
   End
   Begin VB.Label labLevel2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "Lv"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   5550
      TabIndex        =   18
      Top             =   60
      Visible         =   0   'False
      Width           =   300
   End
   Begin VB.Image imgPilot2 
      BorderStyle     =   1  '実線
      Height          =   540
      Left            =   4965
      Stretch         =   -1  'True
      Top             =   45
      Visible         =   0   'False
      Width           =   540
   End
   Begin VB.Label labLevel1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "Lv"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   675
      TabIndex        =   16
      Top             =   60
      Visible         =   0   'False
      Width           =   300
   End
   Begin VB.Image imgPilot1 
      BorderStyle     =   1  '実線
      Height          =   540
      Left            =   90
      Stretch         =   -1  'True
      Top             =   45
      Visible         =   0   'False
      Width           =   540
   End
   Begin VB.Label labHP1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "HP"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   1815
      TabIndex        =   15
      Top             =   90
      Visible         =   0   'False
      Width           =   345
   End
   Begin VB.Label labEN1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "EN"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   3570
      TabIndex        =   14
      Top             =   90
      Visible         =   0   'False
      Width           =   330
   End
   Begin VB.Label labEN2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "EN"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   8445
      TabIndex        =   13
      Top             =   90
      Visible         =   0   'False
      Width           =   375
   End
   Begin VB.Label labHP2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "HP"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   12
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   6720
      TabIndex        =   12
      Top             =   90
      Visible         =   0   'False
      Width           =   375
   End
End
Attribute VB_Name = "frmListBox"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'リストボックスのフォーム

'リストボックスのサイズ (通常:M 大型:L)
'幅
Public HorizontalSize As String
'高さ
Public VerticalSize As String

'Questionコマンド用変数
Public CurrentTime As Integer
Public TimeLimit As Integer

'最後に選択されたアイテム
Private LastSelectedItem As Integer

'リストボックスへのキー入力
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    '既にウィンドウが隠れている場合は無視
    If Not Visible Then
        Exit Sub
    End If
    
    Select Case KeyCode
        Case vbKeyUp
        Case vbKeyDown
        Case vbKeyLeft, vbKeyRight
        Case vbKeyEscape, vbKeyDelete, vbKeyBack
            'キャンセル
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
        Case Else
            '選択
            If lstItems.ListIndex < 0 Then
                Exit Sub
            End If
            If UBound(ListItemFlag) >= lstItems.ListIndex + 1 Then
                If ListItemFlag(lstItems.ListIndex + 1) Then
                    Exit Sub
                End If
            End If
            SelectedItem = lstItems.ListIndex + 1
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex + 1
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
    End Select
End Sub

'リストボックスを開く
Private Sub Form_Load()
Dim ret As Long
    
    'リストボックスを常に手前に表示
    ret = SetWindowPos(hwnd, -1, 0, 0, 0, 0, &H3)
    HorizontalSize = "M"
    VerticalSize = "M"
End Sub

'リストボックスを閉じる
Private Sub Form_Unload(Cancel As Integer)
    TopItem = lstItems.TopIndex + 1
    IsFormClicked = True
    If Not IsMordal And Visible Then
        Cancel = 1
    End If
    Hide
End Sub

'項目をダブルクリック
Private Sub lstItems_DblClick()
    '無効なアイテムが選択されている？
    If lstItems.ListIndex < 0 Then
        Exit Sub
    End If
    If UBound(ListItemFlag) >= lstItems.ListIndex + 1 Then
        If ListItemFlag(lstItems.ListIndex + 1) Then
            Exit Sub
        End If
    End If
    
    If LastSelectedItem <> 0 Then
        '連続で選択
        If Not Visible Then
            Exit Sub
        End If
        SelectedItem = lstItems.ListIndex + 1
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex + 1
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    Else
        '連続でキャンセル
        SelectedItem = 0
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex + 1
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'マウスでクリック
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            '選択
            If Not Visible Then
                Exit Sub
            End If
            
            '無効なアイテムが選択されている？
            If lstItems.ListIndex < 0 Then
                Exit Sub
            End If
            If UBound(ListItemFlag) >= lstItems.ListIndex + 1 Then
                If ListItemFlag(lstItems.ListIndex + 1) Then
                    Exit Sub
                End If
            End If
            
            SelectedItem = lstItems.ListIndex + 1
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex + 1
            IsFormClicked = True
            
            If CurrentTime > 0 Then
                Timer1.Enabled = False
                Hide
            End If
            
        Case 2
            'キャンセル
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex + 1
            IsFormClicked = True
    End Select
End Sub

'キャプション部分をクリック
Private Sub labCaption_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            'ユニットステータスを表示しているユニットを入れ替え
            If MainForm.Visible Then
                If Not DisplayedUnit Is Nothing _
                    And Not SelectedUnit Is Nothing _
                    And Not SelectedTarget Is Nothing _
                Then
                    If DisplayedUnit.ID = SelectedUnit.ID Then
                        DisplayUnitStatus SelectedTarget
                    Else
                        DisplayUnitStatus SelectedUnit
                    End If
                End If
            End If
        Case 2
            'キャンセル
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
    End Select
End Sub

'リストボックスの端をクリック
Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    If Button = 2 Then
        'キャンセル
        SelectedItem = 0
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'リストボックス上でマウスカーソルを動かす
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim itm As Integer
    
    'カーソルがあるアイテムを算出
    itm = ((Y * ScaleWidth) \ Width + 1) \ 16
    itm = itm + lstItems.TopIndex
    
    'カーソル上のアイテムをハイライト表示
    If itm < 0 Or itm >= lstItems.ListCount Then
        lstItems.ListIndex = -1
        Exit Sub
    End If
    If lstItems.ListIndex = itm Then
        Exit Sub
    End If
    lstItems.ListIndex = itm
    
    'コメント欄を更新
    If txtComment.Enabled Then
        txtComment.Text = ListItemComment(itm + 1)
    End If
End Sub

'Questionコマンド対応
Private Sub Timer1_Timer()
    CurrentTime = CurrentTime + 1
    picBar.Cls
    picBar.Line (0, 0)-(picBar.ScaleWidth * CurrentTime \ TimeLimit, picBar.ScaleHeight), , BF
    picBar.Refresh
    
    If CurrentTime >= TimeLimit Then
        SelectedItem = 0
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex
        Hide
        Timer1.Enabled = False
    End If
End Sub

'選択されているアイテムに対応するユニットのステータス表示
Private Sub Timer2_Timer()
Dim u As Unit

    If Not Visible Or Not MainForm.Visible Then
        Exit Sub
    End If
    
    If lstItems.ListIndex = -1 Then
        Exit Sub
    End If
    
    On Error GoTo ErrorHandler
    
    If lstItems.ListIndex >= UBound(ListItemID) Then
        Exit Sub
    End If
    
    '選択されたユニットが存在する？
    If Not UList.IsDefined2(ListItemID(lstItems.ListIndex + 1)) Then
        Exit Sub
    End If
    
    Set u = UList.Item2(ListItemID(lstItems.ListIndex + 1))
    
    '選択されたユニットにパイロットが乗っている？
    If u.CountPilot = 0 Then
        Exit Sub
    End If
    
    '既に表示している？
    If DisplayedUnit Is u Then
        Exit Sub
    End If
    
    '選択されたユニットをステータスウィンドウに表示
    DisplayUnitStatus u
    
ErrorHandler:
End Sub

