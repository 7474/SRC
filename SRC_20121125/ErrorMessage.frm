VERSION 5.00
Begin VB.Form frmErrorMessage 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '�Œ��޲�۸�
   Caption         =   "�G���["
   ClientHeight    =   1770
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   10065
   Icon            =   "ErrorMessage.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1770
   ScaleWidth      =   10065
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows �̊���l
   Begin VB.TextBox txtMessage 
      BackColor       =   &H00FFFFFF&
      BeginProperty Font 
         Name            =   "�l�r �S�V�b�N"
         Size            =   12
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   1590
      Left            =   90
      MultiLine       =   -1  'True
      TabIndex        =   0
      Text            =   "ErrorMessage.frx":030A
      Top             =   90
      Width           =   9885
   End
End
Attribute VB_Name = "frmErrorMessage"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�G���[���b�Z�[�W�\���p�t�H�[��
