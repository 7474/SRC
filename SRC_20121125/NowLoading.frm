VERSION 5.00
Begin VB.Form frmNowLoading 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '固定ﾀﾞｲｱﾛｸﾞ
   Caption         =   "SRC"
   ClientHeight    =   1320
   ClientLeft      =   1140
   ClientTop       =   1605
   ClientWidth     =   3210
   ClipControls    =   0   'False
   ForeColor       =   &H00000000&
   Icon            =   "NowLoading.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   1320
   ScaleWidth      =   3210
   Begin VB.PictureBox picBar 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      FillColor       =   &H00800000&
      FillStyle       =   0  '塗りつぶし
      ForeColor       =   &H00800000&
      Height          =   195
      Left            =   240
      ScaleHeight     =   9
      ScaleMode       =   3  'ﾋﾟｸｾﾙ
      ScaleWidth      =   179
      TabIndex        =   1
      Top             =   840
      Width           =   2745
   End
   Begin VB.Label Label1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "Now Loading ..."
      BeginProperty Font 
         Name            =   "Times New Roman"
         Size            =   18
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   360
      TabIndex        =   0
      Top             =   240
      Width           =   2535
   End
End
Attribute VB_Name = "frmNowLoading"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'データロードの進行状況を示すフォーム

'データ総数
Public Size As Integer
'読み込み終えたデータの数
Public Value As Integer

'ロードを１段階進行させる
Public Sub Progress()
    Value = Value + 1
    picBar.Cls
    picBar.Line (0, 0)-(picBar.ScaleWidth * Value \ Size, picBar.ScaleHeight), , BF
    Refresh
End Sub
