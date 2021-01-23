VERSION 5.00
Begin VB.Form frmTelop 
   BackColor       =   &H00FFFFFF&
   BorderStyle     =   0  'なし
   ClientHeight    =   855
   ClientLeft      =   1965
   ClientTop       =   3165
   ClientWidth     =   5670
   ClipControls    =   0   'False
   Icon            =   "Telop.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   855
   ScaleWidth      =   5670
   ShowInTaskbar   =   0   'False
   Begin VB.Label Label1 
      Alignment       =   2  '中央揃え
      BackStyle       =   0  '透明
      Caption         =   "シナリオタイトル"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐ明朝"
         Size            =   15.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   -1  'True
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   855
      Left            =   120
      TabIndex        =   0
      Top             =   240
      Width           =   5355
      WordWrap        =   -1  'True
   End
End
Attribute VB_Name = "frmTelop"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'Telopコマンド用フォーム
