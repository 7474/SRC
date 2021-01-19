VERSION 5.00
Begin VB.Form frmToolTip 
   BorderStyle     =   0  'なし
   ClientHeight    =   1710
   ClientLeft      =   0
   ClientTop       =   0
   ClientWidth     =   3465
   ClipControls    =   0   'False
   LinkTopic       =   "Form2"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   1710
   ScaleWidth      =   3465
   ShowInTaskbar   =   0   'False
   Visible         =   0   'False
   Begin VB.PictureBox picMessage 
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000018&
      BorderStyle     =   0  'なし
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   1560
      Left            =   0
      ScaleHeight     =   104
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   193
      TabIndex        =   0
      Top             =   0
      Width           =   2895
   End
End
Attribute VB_Name = "frmToolTip"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ツールチップ用フォーム

'フォームをロード
Private Sub Form_Load()
Dim ret As Long
    
    '常に手前に表示
    ret = SetWindowPos(frmToolTip.hwnd, -1, 0, 0, 0, 0, &H3)
End Sub

'ツールチップを表示
Public Sub ShowToolTip(msg As String)
Dim ret As Long
Dim PT As POINTAPI
Dim tw As Integer
Static cur_msg As String
    
    tw = Screen.TwipsPerPixelX
    
    If msg <> cur_msg Then
        cur_msg = msg
        With frmToolTip.picMessage
            'メッセージ長にサイズを合わせる
            .Width = (.TextWidth(msg) + 6) * tw
            .Height = (.TextHeight(msg) + 4) * tw
            frmToolTip.Width = .Width
            frmToolTip.Height = .Height
            
            .Cls
            
            .ForeColor = rgb(200, 200, 200)
            frmToolTip.picMessage.Line (0, 0)-(.Width \ tw, 0)
            frmToolTip.picMessage.Line (0, 0)-(0, .Width \ tw)
            .ForeColor = vbBlack
            frmToolTip.picMessage.Line (0, (.Height - 1) \ tw)-(.Width \ tw, (.Height - 1) \ tw)
            frmToolTip.picMessage.Line ((.Width - 1) \ tw, 0)-((.Width - 1) \ tw, .Height \ tw)
            
            'メッセージを書き込み
            .CurrentX = 3
            .CurrentY = 2
            frmToolTip.picMessage.Print msg
            
            .ForeColor = vbWhite
            .Refresh
        End With
    End If
    
    'フォームの位置を設定
    ret = GetCursorPos(PT)
    frmToolTip.Left = PT.X * tw + 0
    frmToolTip.Top = (PT.Y + 24) * tw
    
    'フォームを非アクティブで表示
    ret = ShowWindow(frmToolTip.hwnd, SW_SHOWNA)
End Sub

