VERSION 5.00
Begin VB.Form frmMessage 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '�Œ�(����)
   Caption         =   "���b�Z�[�W"
   ClientHeight    =   1770
   ClientLeft      =   1395
   ClientTop       =   1515
   ClientWidth     =   7620
   ClipControls    =   0   'False
   FillStyle       =   0  '�h��Ԃ�
   BeginProperty Font 
      Name            =   "�l�r �o����"
      Size            =   12
      Charset         =   128
      Weight          =   700
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   FontTransparent =   0   'False
   ForeColor       =   &H00000000&
   Icon            =   "Message.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   118
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   508
   Begin VB.PictureBox picFace 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      ClipControls    =   0   'False
      Height          =   1020
      Left            =   120
      ScaleHeight     =   64
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   64
      TabIndex        =   15
      Top             =   645
      Width           =   1020
   End
   Begin VB.PictureBox picUnit1 
      Appearance      =   0  '�ׯ�
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   120
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   14
      Top             =   60
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picUnit2 
      Appearance      =   0  '�ׯ�
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   3900
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   13
      Top             =   75
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.TextBox txtHP2 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      BeginProperty Font 
         Name            =   "�l�r ����"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   195
      Left            =   4845
      TabIndex        =   10
      Text            =   "99999/99999"
      Top             =   150
      Width           =   1320
   End
   Begin VB.PictureBox picHP2 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   4455
      ScaleHeight     =   4
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   112
      TabIndex        =   9
      Top             =   420
      Width           =   1740
   End
   Begin VB.PictureBox picEN2 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   6270
      ScaleHeight     =   4
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   78
      TabIndex        =   8
      Top             =   420
      Width           =   1230
   End
   Begin VB.TextBox txtEN2 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      BeginProperty Font 
         Name            =   "�l�r ����"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   195
      Left            =   6645
      TabIndex        =   7
      Text            =   "999/999"
      Top             =   150
      Width           =   855
   End
   Begin VB.TextBox txtEN1 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      BeginProperty Font 
         Name            =   "�l�r ����"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   195
      Left            =   2880
      TabIndex        =   6
      Text            =   "999/999"
      Top             =   150
      Width           =   855
   End
   Begin VB.PictureBox picEN1 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   2490
      ScaleHeight     =   4
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   79
      TabIndex        =   5
      Top             =   420
      Width           =   1245
   End
   Begin VB.PictureBox picHP1 
      AutoRedraw      =   -1  'True
      BackColor       =   &H000000C0&
      ClipControls    =   0   'False
      ForeColor       =   &H0000C000&
      Height          =   120
      Left            =   675
      ScaleHeight     =   4
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   112
      TabIndex        =   3
      Top             =   420
      Width           =   1740
   End
   Begin VB.TextBox txtHP1 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      BeginProperty Font 
         Name            =   "�l�r ����"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   195
      Left            =   1080
      TabIndex        =   2
      Text            =   "99999/99999"
      Top             =   150
      Width           =   1320
   End
   Begin VB.PictureBox picMessage 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o����"
         Size            =   12
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   1050
      Left            =   1260
      ScaleHeight     =   66
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   413
      TabIndex        =   0
      Top             =   630
      Width           =   6255
   End
   Begin VB.Label labHP2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "HP"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   4440
      TabIndex        =   12
      Top             =   120
      Width           =   330
   End
   Begin VB.Label labEN2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "EN"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   6255
      TabIndex        =   11
      Top             =   120
      Width           =   330
   End
   Begin VB.Label labEN1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "EN"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   2475
      TabIndex        =   4
      Top             =   120
      Width           =   330
   End
   Begin VB.Label labHP1 
      BackColor       =   &H00C0C0C0&
      Caption         =   "HP"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   660
      TabIndex        =   1
      Top             =   120
      Width           =   330
   End
End
Attribute VB_Name = "frmMessage"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���b�Z�[�W�E�B���h�E�̃t�H�[��

'�t�H�[������N���b�N
Private Sub Form_Click()
    IsFormClicked = True
End Sub

'�t�H�[������_�u���N���b�N
Private Sub Form_DblClick()
    IsFormClicked = True
End Sub

'�t�H�[����ŃL�[������
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    IsFormClicked = True
End Sub

'�t�H�[����Ń}�E�X�{�^��������
Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    IsFormClicked = True
End Sub

'�t�H�[�������
Private Sub Form_Unload(Cancel As Integer)
Dim ret As Integer
    
    'SRC���I�����邩�m�F
    ret = MsgBox("SRC���I�����܂����H", _
        vbOKCancel + vbQuestion, "�I��")
    
    Select Case ret
        Case 1
            'SRC���I��
            Hide
            TerminateSRC
        Case 2
            '�I�����L�����Z��
            Cancel = 1
    End Select
End Sub

'�p�C���b�g��ʏ�ŃN���b�N
Private Sub picFace_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    '�������b�Z�[�W���胂�[�h�Ɉڍs
    If MessageWait < 10000 Then
        AutoMessageMode = Not AutoMessageMode
    End If
    IsFormClicked = True
End Sub

'���b�Z�[�W����Ń_�u���N���b�N
Private Sub picMessage_DblClick()
    IsFormClicked = True
End Sub

'���b�Z�[�W����Ń}�E�X�{�^��������
Private Sub picMessage_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    IsFormClicked = True
End Sub
