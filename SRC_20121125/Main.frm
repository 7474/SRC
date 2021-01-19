VERSION 5.00
Object = "{056DD990-C612-44AF-A674-4B3C157D1360}#6.0#0"; "FlashControl.ocx"
Begin VB.Form frmMain 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '固定(実線)
   Caption         =   "SRC開発版"
   ClientHeight    =   4410
   ClientLeft      =   1215
   ClientTop       =   3270
   ClientWidth     =   7620
   ClipControls    =   0   'False
   BeginProperty Font 
      Name            =   "ＭＳ 明朝"
      Size            =   9.75
      Charset         =   128
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   Icon            =   "Main.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Main"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   294
   ScaleMode       =   3  'ﾋﾟｸｾﾙ
   ScaleWidth      =   508
   Visible         =   0   'False
   Begin VB.PictureBox picStretchedTmp 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   1
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   21
      Top             =   3000
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picStretchedTmp 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   0
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   20
      Top             =   2280
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picMain 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   15.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   480
      Index           =   1
      Left            =   1440
      MouseIcon       =   "Main.frx":030A
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   13
      Top             =   120
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.PictureBox picBuf 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      FillStyle       =   0  '塗りつぶし
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   0
      Left            =   3360
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   19
      Top             =   720
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   2
      Left            =   3600
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   18
      Top             =   3000
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   1
      Left            =   2880
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   17
      Top             =   3600
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picFace 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      ClipControls    =   0   'False
      Height          =   1020
      Left            =   120
      ScaleHeight     =   64
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   64
      TabIndex        =   16
      Top             =   2520
      Width           =   1020
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   0
      Left            =   2880
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   15
      Top             =   3000
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picMaskedBack 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00FFFFFF&
      Height          =   480
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   14
      Top             =   120
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   1000
      Left            =   2940
      Top             =   2460
   End
   Begin VB.PictureBox picMask2 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      Height          =   480
      Left            =   120
      Picture         =   "Main.frx":0614
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   12
      Top             =   1635
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picNeautral 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      Height          =   480
      Left            =   2640
      Picture         =   "Main.frx":0E56
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   11
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picEnemy 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      Height          =   480
      Left            =   1860
      Picture         =   "Main.frx":1698
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   10
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picUnit 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      Height          =   480
      Left            =   1140
      Picture         =   "Main.frx":1EDA
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   9
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picPilotStatus 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      Height          =   495
      Left            =   6240
      ScaleHeight     =   33
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   8
      Top             =   3000
      Width           =   1215
   End
   Begin VB.PictureBox picUnitStatus 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      FillStyle       =   0  '塗りつぶし
      BeginProperty Font 
         Name            =   "ＭＳ 明朝"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   5640
      ScaleHeight     =   33
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   7
      Top             =   3240
      Width           =   1215
   End
   Begin VB.PictureBox picUnitBitmap 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   1440
      Left            =   1320
      ScaleHeight     =   96
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   6
      Top             =   2520
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.HScrollBar HScroll 
      Enabled         =   0   'False
      Height          =   255
      LargeChange     =   4
      Left            =   900
      Max             =   20
      Min             =   1
      TabIndex        =   5
      TabStop         =   0   'False
      Top             =   1020
      Value           =   1
      Width           =   735
   End
   Begin VB.VScrollBar VScroll 
      Enabled         =   0   'False
      Height          =   735
      LargeChange     =   4
      Left            =   1740
      Max             =   20
      Min             =   1
      TabIndex        =   4
      TabStop         =   0   'False
      Top             =   840
      Value           =   1
      Width           =   255
   End
   Begin VB.PictureBox picMask 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   480
      Left            =   120
      Picture         =   "Main.frx":271C
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   3
      Top             =   900
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picTmp 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      FillStyle       =   0  '塗りつぶし
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   480
      Left            =   3600
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   32
      TabIndex        =   2
      Top             =   2280
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picBack 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   480
      Left            =   5760
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   1
      Top             =   120
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.PictureBox picMain 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   15.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   480
      Index           =   0
      Left            =   120
      MouseIcon       =   "Main.frx":2F5E
      ScaleHeight     =   32
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   81
      TabIndex        =   0
      Top             =   120
      Width           =   1215
      Begin FlashControl.FlashObject FlashObject 
         Height          =   495
         Left            =   0
         TabIndex        =   22
         Top             =   0
         Visible         =   0   'False
         Width           =   495
         _ExtentX        =   873
         _ExtentY        =   873
      End
   End
   Begin VB.Menu mnuUnitCommand 
      Caption         =   "ユニットコマンド"
      Visible         =   0   'False
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "移動"
         Index           =   0
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "テレポート"
         Index           =   1
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "ジャンプ"
         Index           =   2
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "会話"
         Index           =   3
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "攻撃"
         Index           =   4
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "修理"
         Index           =   5
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "補給"
         Index           =   6
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "アビリティ"
         Index           =   7
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "チャージ"
         Index           =   8
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "スペシャルパワー"
         Index           =   9
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "変形"
         Index           =   10
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "分離"
         Index           =   11
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "合体"
         Index           =   12
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "ハイパーモード"
         Index           =   13
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "地上"
         Index           =   14
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "空中"
         Index           =   15
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "地中"
         Index           =   16
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "水中"
         Index           =   17
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "発進"
         Index           =   18
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "アイテム"
         Index           =   19
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "召喚解除"
         Index           =   20
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "命令"
         Index           =   21
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "特殊能力一覧"
         Index           =   22
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "武装一覧"
         Index           =   23
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "アビリティ一覧"
         Index           =   24
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   25
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   26
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   27
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   28
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   29
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   30
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   31
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   32
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   33
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   ""
         Index           =   34
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "待機"
         Index           =   35
      End
   End
   Begin VB.Menu mnuMapCommand 
      Caption         =   "マップコマンド"
      Visible         =   0   'False
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "ターン終了"
         Index           =   0
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "中断"
         Index           =   1
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "部隊表"
         Index           =   2
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "スペシャルパワー検索"
         Index           =   3
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "全体マップ"
         Index           =   4
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "作戦目的"
         Index           =   5
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   6
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   7
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   8
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   9
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   10
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   11
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   12
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   13
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   14
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   ""
         Index           =   15
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "自動反撃モード"
         Index           =   16
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "設定変更"
         Index           =   17
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "リスタート"
         Index           =   18
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "クイックロード"
         Index           =   19
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "クイックセーブ"
         Index           =   20
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'メインウィンドウのフォーム

'マップウィンドウがドラッグされているか？
Private IsDragging As Boolean

Private Sub FlashObject_GetFlashEvent(ByVal FunctionParameter As String)
    GetEvent FunctionParameter
End Sub

'フォーム上でキーを押す
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    'ＧＵＩをロック中？
    If IsGUILocked Then
        'リストボックス表示中はキャンセル動作とみなす
        If frmListBox.Visible Then
            SelectedItem = 0
            TopItem = frmListBox.lstItems.TopIndex + 1
            If IsFormClicked Then
                frmListBox.Hide
            End If
            IsFormClicked = True
        End If
        
        'メッセージ表示中はメッセージ送りとみなす
        If frmMessage.Visible Then
            IsFormClicked = True
        End If
        
        'クリック待ちであれば待ちを解除
        If WaitClickMode Then
            IsFormClicked = True
        End If
        Exit Sub
    End If
    
    If Shift = 0 Then
        '方向キーを押した場合はマップを動かす
        Select Case KeyCode
            Case vbKeyLeft
                If MapX > 1 Then
                    MapX = MapX - 1
                    RefreshScreen
                End If
            Case vbKeyUp
                If MapY > 1 Then
                    MapY = MapY - 1
                    RefreshScreen
                End If
            Case vbKeyRight
                If MapX < HScroll.max Then
                    MapX = MapX + 1
                    RefreshScreen
                End If
            Case vbKeyDown
                If MapY < VScroll.max Then
                    MapY = MapY + 1
                    RefreshScreen
                End If
            Case vbKeyEscape, vbKeyDelete, vbKeyBack
                picMain_MouseDown 0, 2, 0, MouseX, MouseY
            Case Else
                picMain_MouseDown 0, 1, 0, MouseX, MouseY
        End Select
    End If
End Sub

'フォーム上でマウスを動かす
Private Sub Form_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
    'ツールチップを消す
    frmToolTip.Hide
    If picMain(0).MousePointer = 99 Then
        picMain(0).MousePointer = 0
    End If
End Sub

'フォームを閉じる
Private Sub Form_Unload(Cancel As Integer)
Dim ret As Integer
Dim IsErrorMessageVisible As Boolean

    'エラーメッセージのダイアログは一番上に重ねられるため消去する必要がある
    If Not frmErrorMessage Is Nothing Then
        IsErrorMessageVisible = frmErrorMessage.Visible
    End If
    If IsErrorMessageVisible Then
        frmErrorMessage.Hide
    End If
    
    'SRCの終了を確認
    ret = MsgBox("SRCを終了しますか？", _
        vbOKCancel + vbQuestion, "終了")
    
    Select Case ret
        Case 1
            'SRCを終了
            TerminateSRC
        Case 2
            '終了をキャンセル
            Cancel = 1
    End Select
    
    'エラーメッセージを表示
    If IsErrorMessageVisible Then
        frmErrorMessage.Show
    End If
End Sub

'マップ画面の横スクロールバーを操作
Private Sub HScroll_Change()
    MapX = HScroll.Value
    
    'ステータス表示中はスクロールバーを中央に固定
    If MapFileName = "" Then
        MapX = 8
    End If
    
    '画面書き換え
    If frmMain.Visible Then
        RefreshScreen
    End If
End Sub

'マップコマンドメニューをクリック
Private Sub mnuMapCommandItem_Click(Index As Integer)
    If GetAsyncKeyState(RButtonID) = 1 Then
        '右ボタンでキャンセル
        CancelCommand
        Exit Sub
    End If
    
    'マップコマンドを実行
    MapCommand Index
End Sub

'ユニットコマンドメニューをクリック
Private Sub mnuUnitCommandItem_Click(Index As Integer)
    If GetAsyncKeyState(RButtonID) = 1 Then
        '右ボタンでキャンセル
        CancelCommand
        Exit Sub
    End If
    
    'ユニットコマンドを実行
    UnitCommand Index
End Sub

'ステータスウィンドウのパイロット画像上をクリック
Private Sub picFace_Click()
Dim n As Integer

    'ＧＵＩのロック中は無視
    If IsGUILocked Then
        Exit Sub
    End If
    
    'ステータスウィンドウで表示しているパイロットを変更
    If DisplayedUnit Is Nothing Then
        Exit Sub
    End If
    With DisplayedUnit
        If .CountPilot = 0 Then
            Exit Sub
        End If
        
        DisplayedPilotInd = DisplayedPilotInd + 1
        
        n = .CountPilot + .CountSupport
        If .IsFeatureAvailable("追加サポート") Then
            n = n + 1
        End If
        If DisplayedPilotInd > n Then
            DisplayedPilotInd = 1
        End If
        
        DisplayUnitStatus DisplayedUnit, DisplayedPilotInd
    End With
End Sub

'マップ画面上でダブルクリック
Private Sub picMain_DblClick(Index As Integer)
    If IsGUILocked Then
        'ＧＵＩクロック中は単なるクリックとみなす
        If frmMessage.Visible Then
            IsFormClicked = True
        End If
        If WaitClickMode Then
            IsFormClicked = True
        End If
        Exit Sub
    Else
        'キャンセルの場合はキャンセルを連続実行
        If MouseButton = 2 Then
            Select Case CommandState
                Case "マップコマンド"
                    CommandState = "ユニット選択"
                Case "ユニット選択"
                    ProceedCommand True
                Case Else
                    CancelCommand
            End Select
        End If
    End If
End Sub

'マップ画面上でマウスをクリック
Private Sub picMain_MouseDown(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
Dim xx As Integer, yy As Integer
    
    '押されたマウスボタンの種類＆カーソルの座標を記録
    MouseButton = Button
    MouseX = X
    MouseY = Y
    
    'ＧＵＩロック中は単なるクリックとして処理
    If IsGUILocked Then
        If frmMessage.Visible Then
            IsFormClicked = True
        End If
        If WaitClickMode Then
            IsFormClicked = True
        End If
        Exit Sub
    End If
    
    Select Case Button
        Case 1
            '左クリック
            PrevMapX = MapX
            PrevMapY = MapY
            PrevMouseX = X
            PrevMouseY = Y
            Select Case CommandState
                Case "マップコマンド"
                    CommandState = "ユニット選択"
                Case "ユニット選択"
                    xx = PixelToMapX(X)
                    yy = PixelToMapY(Y)
                    If xx < 1 Or MapWidth < xx _
                        Or yy < 1 Or MapHeight < yy _
                    Then
                        IsDragging = True
                    ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then
                        ProceedCommand
                    Else
                        IsDragging = True
                    End If
                Case "ターゲット選択", "移動後ターゲット選択"
                    xx = PixelToMapX(X)
                    yy = PixelToMapY(Y)
                    If xx < 1 Or MapWidth < xx _
                        Or yy < 1 Or MapHeight < yy _
                    Then
                        IsDragging = True
                    ElseIf Not MaskData(xx, yy) Then
                        ProceedCommand
                    Else
                        IsDragging = True
                    End If
' MOD START MARGE
'                Case "コマンド選択", "移動後コマンド選択"
                Case "コマンド選択"
' MOD  END  MARGE
                    CancelCommand
' ADD START MARGE
                    'もし新しいクリック地点がユニットなら、ユニット選択の処理を進める
                    xx = PixelToMapX(X)
                    yy = PixelToMapY(Y)
                    If xx < 1 Or MapWidth < xx _
                        Or yy < 1 Or MapHeight < yy _
                    Then
                        IsDragging = True
                    ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then
                        ProceedCommand
                    Else
                        IsDragging = True
                    End If
                Case "移動後コマンド選択"
                    CancelCommand
' ADD  END  MARGE
                Case Else
                    ProceedCommand
            End Select
        Case 2
            '右クリック
            Select Case CommandState
                Case "マップコマンド"
                    CommandState = "ユニット選択"
                Case "ユニット選択"
                    ProceedCommand True
                Case Else
                    CancelCommand
            End Select
    End Select
End Sub

'マップ画面上でマウスカーソルを移動
Private Sub picMain_MouseMove(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
Static LastMouseX As Integer, LastMouseY As Integer
Static LastMapX As Integer, LastMapY As Integer
Static LastHostSpot As String
Dim xx As Integer, yy As Integer
Dim i As Integer
    
    '前回のマウス位置を記録
    LastMouseX = MouseX
    LastMouseY = MouseY
    
    '現在のマウス位置を記録
    MouseX = X
    MouseY = Y
    
    'ＧＵＩロック中？
    If IsGUILocked Then
        If Not WaitClickMode Then
            Exit Sub
        End If
        
        'ホットポイントが定義されている場合はツールチップを変更
        For i = 1 To UBound(HotPointList)
            With HotPointList(i)
                If .Left <= MouseX And MouseX < .Left + .width _
                    And .Top <= MouseY And MouseY < .Top + .Height _
                Then
                    If .Caption = "非表示" Or .Caption = "" Then
                        Exit For
                    End If
                    
                    If .Name <> LastHostSpot And LastHostSpot <> "" Then
                        Exit For
                    End If
                    
                    'ツールチップの表示
                    frmToolTip.ShowToolTip .Caption
                    
                    With picMain(0)
                        If .MousePointer <> 99 Then
                            .Refresh
                            .MousePointer = 99
                        End If
                    End With
                    
                    LastHostSpot = .Name
                    Exit Sub
                End If
            End With
        Next
        
        'ホットポイント上にカーソルがなければツールチップを消す
        frmToolTip.Hide
        LastHostSpot = ""
        picMain(0).MousePointer = 0
        Exit Sub
    End If
    
    'マップが設定されていない場合はこれ以降の判定は不要
    If MapWidth < 15 Or MapHeight < 15 Then
        Exit Sub
    End If
    
    'カーソル上にユニットがいればステータスウィンドウにそのユニットを表示
    xx = PixelToMapX(X)
    yy = PixelToMapY(Y)
'MOD START 240a
'    If MainWidth = 15 Then
    If Not NewGUIMode Then
'MOD  END
        If 1 <= xx And xx <= MapWidth _
            And 1 <= yy And yy <= MapHeight _
        Then
'MOD START 240a
'            If Not MapDataForUnit(xx, yy) Is Nothing Then
'                InstantUnitStatusDisplay xx, yy
'            End If
            If MapDataForUnit(xx, yy) Is Nothing Then
                If Not MapFileName = "" Then
                    'ユニットがいない、かつステータス表示でなければ地形情報を表示
                    DisplayGlobalStatus
                End If
            Else
                InstantUnitStatusDisplay xx, yy
            End If
'MOD  END
'ADD START 240a
        Else
            'マップ外にカーソルがある場合
            DisplayGlobalStatus
'ADD  END
        End If
    Else
'ADD ユニット選択追加・移動時も表示 240a
'        If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択") _
'            And (SelectedCommand <> "移動" _
'                And SelectedCommand <> "テレポート" _
'                And SelectedCommand <> "ジャンプ") _
'        Then
        If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択" Or CommandState = "ユニット選択") Then
            If 1 <= xx And xx <= MapWidth _
                And 1 <= yy And yy <= MapHeight _
            Then
                If Not MapDataForUnit(xx, yy) Is Nothing Then
                    Me.picMain(0).Refresh
'                    RedrawScreen
                    InstantUnitStatusDisplay xx, yy
'ADD Else
                Else
                    ClearUnitStatus
                End If
            End If
        ElseIf MouseX <> LastMouseX Or MouseY <> LastMouseY Then
            ClearUnitStatus
        End If
    End If
    
    'マップをドラッグ中？
    If IsDragging And Button = 1 Then
        'Ｘ軸の移動量を算出
        MapX = PrevMapX - (X - PrevMouseX) \ 32
        If MapX < 1 Then
            MapX = 1
        ElseIf MapX > HScroll.max Then
            MapX = HScroll.max
        End If
        
        'Ｙ軸の移動量を算出
        MapY = PrevMapY - (Y - PrevMouseY) \ 32
        If MapY < 1 Then
            MapY = 1
        ElseIf MapY > VScroll.max Then
            MapY = VScroll.max
        End If
        
        If MapFileName = "" Then
            'ステータス画面の場合は移動量を限定
            MapX = 8
            If MapY < 8 Then
                MapY = 8
            ElseIf MapY > MapHeight - 7 Then
                MapY = MapHeight - 7
            End If
        End If
        
        'マップ画面を新しい座標で更新
        If Not MapX = LastMapX Or Not MapY = LastMapY Then
            RefreshScreen
        End If
    End If
End Sub

'マップ画面上でマウスボタンを離す
Private Sub picMain_MouseUp(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
    
    If IsGUILocked Then
        Exit Sub
    End If
    'マップ画面のドラッグを解除
    IsDragging = False
End Sub

'ＢＧＭ連続再生用タイマー
Private Sub Timer1_Timer()
    If BGMFileName <> "" Then
        If RepeatMode Then
            RestartBGM
        End If
    End If
End Sub

'マップウィンドウの縦スクロールを操作
Private Sub VScroll_Change()
    MapY = VScroll.Value
    
    If MapFileName = "" Then
        'ステータス画面の場合は移動量を制限
        If MapY < 8 Then
            MapY = 8
        ElseIf MapY > MapHeight - 7 Then
            MapY = MapHeight - 7
        End If
    End If
    
    'マップ画面を更新
    If frmMain.Visible Then
        RefreshScreen
    End If
End Sub
