VERSION 5.00
Begin VB.Form frmTitle 
   BorderStyle     =   0  'なし
   Caption         =   "SRC"
   ClientHeight    =   3495
   ClientLeft      =   2700
   ClientTop       =   2955
   ClientWidth     =   5790
   ClipControls    =   0   'False
   Icon            =   "Title.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'Z ｵｰﾀﾞｰ
   ScaleHeight     =   233
   ScaleMode       =   3  'ﾋﾟｸｾﾙ
   ScaleWidth      =   386
   ShowInTaskbar   =   0   'False
   Begin VB.PictureBox Picture1 
      AutoSize        =   -1  'True
      BorderStyle     =   0  'なし
      Height          =   600
      Left            =   2520
      Picture         =   "Title.frx":030A
      ScaleHeight     =   600
      ScaleWidth      =   3000
      TabIndex        =   0
      Top             =   1320
      Width           =   3000
   End
   Begin VB.Frame Frame1 
      Height          =   3015
      Left            =   360
      TabIndex        =   1
      Top             =   120
      Width           =   5055
      Begin VB.Image Image1 
         Height          =   1440
         Left            =   240
         Picture         =   "Title.frx":268C
         Stretch         =   -1  'True
         Top             =   840
         Width           =   1440
      End
      Begin VB.Label labAuthor 
         Alignment       =   1  '右揃え
         Caption         =   "Kei Sakamoto / Inui Tetsuyuki"
         BeginProperty Font 
            Name            =   "Times New Roman"
            Size            =   11.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   1920
         TabIndex        =   3
         Top             =   2520
         Width           =   2895
      End
      Begin VB.Label labVersion 
         Alignment       =   1  '右揃え
         Caption         =   "Ver 1.7.*"
         BeginProperty Font 
            Name            =   "Times New Roman"
            Size            =   15.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   2160
         TabIndex        =   2
         Top             =   2040
         Width           =   2655
      End
   End
   Begin VB.Label labLicense 
      Alignment       =   2  '中央揃え
      Caption         =   "This program is distributed under the terms of GPL"
      Height          =   255
      Left            =   120
      TabIndex        =   4
      Top             =   3240
      Width           =   5535
   End
End
Attribute VB_Name = "frmTitle"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'タイトル画面用フォーム

Private Sub Form_Load()
    With App
        labVersion.Caption = "Ver " & .Major & "." & .Minor & "." & .Revision & "a"
    End With
End Sub

