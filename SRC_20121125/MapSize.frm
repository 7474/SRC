VERSION 5.00
Begin VB.Form frmMapSize 
   BorderStyle     =   3  '�Œ��޲�۸�
   Caption         =   "�}�b�v�T�C�Y"
   ClientHeight    =   1545
   ClientLeft      =   2835
   ClientTop       =   3480
   ClientWidth     =   3750
   Icon            =   "MapSize.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   103
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   250
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  '��ʂ̒���
   Begin VB.TextBox txtMapWidth 
      Height          =   345
      Left            =   1275
      TabIndex        =   1
      Top             =   135
      Width           =   2325
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   390
      Left            =   495
      TabIndex        =   4
      Top             =   1020
      Width           =   1140
   End
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "��ݾ�"
      Height          =   390
      Left            =   2100
      TabIndex        =   5
      Top             =   1020
      Width           =   1140
   End
   Begin VB.TextBox txtMapHeight 
      Height          =   345
      IMEMode         =   3  '�̌Œ�
      Left            =   1275
      TabIndex        =   3
      Top             =   540
      Width           =   2325
   End
   Begin VB.Label lblLabels 
      Caption         =   "�}�b�v�̕�"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   270
      Index           =   0
      Left            =   120
      TabIndex        =   0
      Top             =   240
      Width           =   1080
   End
   Begin VB.Label lblLabels 
      Caption         =   "�}�b�v�̍���"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   270
      Index           =   1
      Left            =   120
      TabIndex        =   2
      Top             =   600
      Width           =   1080
   End
End
Attribute VB_Name = "frmMapSize"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�}�b�v�T�C�Y�̓��͗p�t�H�[��

Private Sub cmdCancel_Click()
    Me.Hide
End Sub

Private Sub cmdOK_Click()
    If IsNumeric(txtMapWidth) And IsNumeric(txtMapHeight) Then
        If CInt(txtMapWidth) >= 15 And CInt(txtMapHeight) >= 15 _
            And CInt(txtMapWidth) <= 50 And CInt(txtMapHeight) <= 50 _
        Then
            SetMapSize CInt(txtMapWidth), CInt(txtMapHeight)
            Me.Hide
            Exit Sub
        End If
    End If
        
    MsgBox "�}�b�v�T�C�Y������������܂���B������x���͂��Ă�������", , "�}�b�v�T�C�Y"
    txtMapWidth.SetFocus
End Sub

Private Sub Form_Load()
    txtMapWidth = Format$(MapWidth)
    txtMapHeight = Format$(MapHeight)
End Sub
