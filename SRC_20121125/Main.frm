VERSION 5.00
Object = "{056DD990-C612-44AF-A674-4B3C157D1360}#6.0#0"; "FlashControl.ocx"
Begin VB.Form frmMain 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '�Œ�(����)
   Caption         =   "SRC�J����"
   ClientHeight    =   4410
   ClientLeft      =   1215
   ClientTop       =   3270
   ClientWidth     =   7620
   ClipControls    =   0   'False
   BeginProperty Font 
      Name            =   "�l�r ����"
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
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   294
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   508
   Visible         =   0   'False
   Begin VB.PictureBox picStretchedTmp 
      AutoRedraw      =   -1  'True
      AutoSize        =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   1
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
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
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   0
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   20
      Top             =   2280
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picMain 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o����"
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
      ScaleMode       =   3  '�߸��
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
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      FillStyle       =   0  '�h��Ԃ�
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   19
      Top             =   720
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   2
      Left            =   3600
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   18
      Top             =   3000
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   1
      Left            =   2880
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   64
      TabIndex        =   16
      Top             =   2520
      Width           =   1020
   End
   Begin VB.PictureBox picTmp32 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   480
      Index           =   0
      Left            =   2880
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   15
      Top             =   3000
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picMaskedBack 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00FFFFFF&
      Height          =   480
      Left            =   4320
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
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
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      Height          =   480
      Left            =   120
      Picture         =   "Main.frx":0614
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   12
      Top             =   1635
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picNeautral 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      Height          =   480
      Left            =   2640
      Picture         =   "Main.frx":0E56
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   11
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picEnemy 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      Height          =   480
      Left            =   1860
      Picture         =   "Main.frx":1698
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   10
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picUnit 
      AutoRedraw      =   -1  'True
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      Height          =   480
      Left            =   1140
      Picture         =   "Main.frx":1EDA
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   9
      Top             =   1740
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picPilotStatus 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      Height          =   495
      Left            =   6240
      ScaleHeight     =   33
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   81
      TabIndex        =   8
      Top             =   3000
      Width           =   1215
   End
   Begin VB.PictureBox picUnitStatus 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00C0C0C0&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      FillStyle       =   0  '�h��Ԃ�
      BeginProperty Font 
         Name            =   "�l�r ����"
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   81
      TabIndex        =   7
      Top             =   3240
      Width           =   1215
   End
   Begin VB.PictureBox picUnitBitmap 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
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
      ScaleMode       =   3  '�߸��
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
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
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
      ScaleMode       =   3  '�߸��
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
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      FillStyle       =   0  '�h��Ԃ�
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   2
      Top             =   2280
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picBack 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   81
      TabIndex        =   1
      Top             =   120
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.PictureBox picMain 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o����"
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
      ScaleMode       =   3  '�߸��
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
      Caption         =   "���j�b�g�R�}���h"
      Visible         =   0   'False
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�ړ�"
         Index           =   0
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�e���|�[�g"
         Index           =   1
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�W�����v"
         Index           =   2
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "��b"
         Index           =   3
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�U��"
         Index           =   4
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�C��"
         Index           =   5
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�⋋"
         Index           =   6
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�A�r���e�B"
         Index           =   7
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�`���[�W"
         Index           =   8
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�X�y�V�����p���["
         Index           =   9
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�ό`"
         Index           =   10
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "����"
         Index           =   11
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "����"
         Index           =   12
         Visible         =   0   'False
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�n�C�p�[���[�h"
         Index           =   13
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�n��"
         Index           =   14
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "��"
         Index           =   15
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�n��"
         Index           =   16
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "����"
         Index           =   17
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "���i"
         Index           =   18
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�A�C�e��"
         Index           =   19
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "��������"
         Index           =   20
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "����"
         Index           =   21
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "����\�͈ꗗ"
         Index           =   22
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�����ꗗ"
         Index           =   23
      End
      Begin VB.Menu mnuUnitCommandItem 
         Caption         =   "�A�r���e�B�ꗗ"
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
         Caption         =   "�ҋ@"
         Index           =   35
      End
   End
   Begin VB.Menu mnuMapCommand 
      Caption         =   "�}�b�v�R�}���h"
      Visible         =   0   'False
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�^�[���I��"
         Index           =   0
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "���f"
         Index           =   1
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�����\"
         Index           =   2
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�X�y�V�����p���[����"
         Index           =   3
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�S�̃}�b�v"
         Index           =   4
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "���ړI"
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
         Caption         =   "�����������[�h"
         Index           =   16
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�ݒ�ύX"
         Index           =   17
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "���X�^�[�g"
         Index           =   18
         Visible         =   0   'False
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�N�C�b�N���[�h"
         Index           =   19
      End
      Begin VB.Menu mnuMapCommandItem 
         Caption         =   "�N�C�b�N�Z�[�u"
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
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���C���E�B���h�E�̃t�H�[��

'�}�b�v�E�B���h�E���h���b�O����Ă��邩�H
Private IsDragging As Boolean

Private Sub FlashObject_GetFlashEvent(ByVal FunctionParameter As String)
    GetEvent FunctionParameter
End Sub

'�t�H�[����ŃL�[������
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    '�f�t�h�����b�N���H
    If IsGUILocked Then
        '���X�g�{�b�N�X�\�����̓L�����Z������Ƃ݂Ȃ�
        If frmListBox.Visible Then
            SelectedItem = 0
            TopItem = frmListBox.lstItems.TopIndex + 1
            If IsFormClicked Then
                frmListBox.Hide
            End If
            IsFormClicked = True
        End If
        
        '���b�Z�[�W�\�����̓��b�Z�[�W����Ƃ݂Ȃ�
        If frmMessage.Visible Then
            IsFormClicked = True
        End If
        
        '�N���b�N�҂��ł���Α҂�������
        If WaitClickMode Then
            IsFormClicked = True
        End If
        Exit Sub
    End If
    
    If Shift = 0 Then
        '�����L�[���������ꍇ�̓}�b�v�𓮂���
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

'�t�H�[����Ń}�E�X�𓮂���
Private Sub Form_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
    '�c�[���`�b�v������
    frmToolTip.Hide
    If picMain(0).MousePointer = 99 Then
        picMain(0).MousePointer = 0
    End If
End Sub

'�t�H�[�������
Private Sub Form_Unload(Cancel As Integer)
Dim ret As Integer
Dim IsErrorMessageVisible As Boolean

    '�G���[���b�Z�[�W�̃_�C�A���O�͈�ԏ�ɏd�˂��邽�ߏ�������K�v������
    If Not frmErrorMessage Is Nothing Then
        IsErrorMessageVisible = frmErrorMessage.Visible
    End If
    If IsErrorMessageVisible Then
        frmErrorMessage.Hide
    End If
    
    'SRC�̏I�����m�F
    ret = MsgBox("SRC���I�����܂����H", _
        vbOKCancel + vbQuestion, "�I��")
    
    Select Case ret
        Case 1
            'SRC���I��
            TerminateSRC
        Case 2
            '�I�����L�����Z��
            Cancel = 1
    End Select
    
    '�G���[���b�Z�[�W��\��
    If IsErrorMessageVisible Then
        frmErrorMessage.Show
    End If
End Sub

'�}�b�v��ʂ̉��X�N���[���o�[�𑀍�
Private Sub HScroll_Change()
    MapX = HScroll.Value
    
    '�X�e�[�^�X�\�����̓X�N���[���o�[�𒆉��ɌŒ�
    If MapFileName = "" Then
        MapX = 8
    End If
    
    '��ʏ�������
    If frmMain.Visible Then
        RefreshScreen
    End If
End Sub

'�}�b�v�R�}���h���j���[���N���b�N
Private Sub mnuMapCommandItem_Click(Index As Integer)
    If GetAsyncKeyState(RButtonID) = 1 Then
        '�E�{�^���ŃL�����Z��
        CancelCommand
        Exit Sub
    End If
    
    '�}�b�v�R�}���h�����s
    MapCommand Index
End Sub

'���j�b�g�R�}���h���j���[���N���b�N
Private Sub mnuUnitCommandItem_Click(Index As Integer)
    If GetAsyncKeyState(RButtonID) = 1 Then
        '�E�{�^���ŃL�����Z��
        CancelCommand
        Exit Sub
    End If
    
    '���j�b�g�R�}���h�����s
    UnitCommand Index
End Sub

'�X�e�[�^�X�E�B���h�E�̃p�C���b�g�摜����N���b�N
Private Sub picFace_Click()
Dim n As Integer

    '�f�t�h�̃��b�N���͖���
    If IsGUILocked Then
        Exit Sub
    End If
    
    '�X�e�[�^�X�E�B���h�E�ŕ\�����Ă���p�C���b�g��ύX
    If DisplayedUnit Is Nothing Then
        Exit Sub
    End If
    With DisplayedUnit
        If .CountPilot = 0 Then
            Exit Sub
        End If
        
        DisplayedPilotInd = DisplayedPilotInd + 1
        
        n = .CountPilot + .CountSupport
        If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
            n = n + 1
        End If
        If DisplayedPilotInd > n Then
            DisplayedPilotInd = 1
        End If
        
        DisplayUnitStatus DisplayedUnit, DisplayedPilotInd
    End With
End Sub

'�}�b�v��ʏ�Ń_�u���N���b�N
Private Sub picMain_DblClick(Index As Integer)
    If IsGUILocked Then
        '�f�t�h�N���b�N���͒P�Ȃ�N���b�N�Ƃ݂Ȃ�
        If frmMessage.Visible Then
            IsFormClicked = True
        End If
        If WaitClickMode Then
            IsFormClicked = True
        End If
        Exit Sub
    Else
        '�L�����Z���̏ꍇ�̓L�����Z����A�����s
        If MouseButton = 2 Then
            Select Case CommandState
                Case "�}�b�v�R�}���h"
                    CommandState = "���j�b�g�I��"
                Case "���j�b�g�I��"
                    ProceedCommand True
                Case Else
                    CancelCommand
            End Select
        End If
    End If
End Sub

'�}�b�v��ʏ�Ń}�E�X���N���b�N
Private Sub picMain_MouseDown(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
Dim xx As Integer, yy As Integer
    
    '�����ꂽ�}�E�X�{�^���̎�ށ��J�[�\���̍��W���L�^
    MouseButton = Button
    MouseX = X
    MouseY = Y
    
    '�f�t�h���b�N���͒P�Ȃ�N���b�N�Ƃ��ď���
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
            '���N���b�N
            PrevMapX = MapX
            PrevMapY = MapY
            PrevMouseX = X
            PrevMouseY = Y
            Select Case CommandState
                Case "�}�b�v�R�}���h"
                    CommandState = "���j�b�g�I��"
                Case "���j�b�g�I��"
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
                Case "�^�[�Q�b�g�I��", "�ړ���^�[�Q�b�g�I��"
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
'                Case "�R�}���h�I��", "�ړ���R�}���h�I��"
                Case "�R�}���h�I��"
' MOD  END  MARGE
                    CancelCommand
' ADD START MARGE
                    '�����V�����N���b�N�n�_�����j�b�g�Ȃ�A���j�b�g�I���̏�����i�߂�
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
                Case "�ړ���R�}���h�I��"
                    CancelCommand
' ADD  END  MARGE
                Case Else
                    ProceedCommand
            End Select
        Case 2
            '�E�N���b�N
            Select Case CommandState
                Case "�}�b�v�R�}���h"
                    CommandState = "���j�b�g�I��"
                Case "���j�b�g�I��"
                    ProceedCommand True
                Case Else
                    CancelCommand
            End Select
    End Select
End Sub

'�}�b�v��ʏ�Ń}�E�X�J�[�\�����ړ�
Private Sub picMain_MouseMove(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
Static LastMouseX As Integer, LastMouseY As Integer
Static LastMapX As Integer, LastMapY As Integer
Static LastHostSpot As String
Dim xx As Integer, yy As Integer
Dim i As Integer
    
    '�O��̃}�E�X�ʒu���L�^
    LastMouseX = MouseX
    LastMouseY = MouseY
    
    '���݂̃}�E�X�ʒu���L�^
    MouseX = X
    MouseY = Y
    
    '�f�t�h���b�N���H
    If IsGUILocked Then
        If Not WaitClickMode Then
            Exit Sub
        End If
        
        '�z�b�g�|�C���g����`����Ă���ꍇ�̓c�[���`�b�v��ύX
        For i = 1 To UBound(HotPointList)
            With HotPointList(i)
                If .Left <= MouseX And MouseX < .Left + .width _
                    And .Top <= MouseY And MouseY < .Top + .Height _
                Then
                    If .Caption = "��\��" Or .Caption = "" Then
                        Exit For
                    End If
                    
                    If .Name <> LastHostSpot And LastHostSpot <> "" Then
                        Exit For
                    End If
                    
                    '�c�[���`�b�v�̕\��
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
        
        '�z�b�g�|�C���g��ɃJ�[�\�����Ȃ���΃c�[���`�b�v������
        frmToolTip.Hide
        LastHostSpot = ""
        picMain(0).MousePointer = 0
        Exit Sub
    End If
    
    '�}�b�v���ݒ肳��Ă��Ȃ��ꍇ�͂���ȍ~�̔���͕s�v
    If MapWidth < 15 Or MapHeight < 15 Then
        Exit Sub
    End If
    
    '�J�[�\����Ƀ��j�b�g������΃X�e�[�^�X�E�B���h�E�ɂ��̃��j�b�g��\��
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
                    '���j�b�g�����Ȃ��A���X�e�[�^�X�\���łȂ���Βn�`����\��
                    DisplayGlobalStatus
                End If
            Else
                InstantUnitStatusDisplay xx, yy
            End If
'MOD  END
'ADD START 240a
        Else
            '�}�b�v�O�ɃJ�[�\��������ꍇ
            DisplayGlobalStatus
'ADD  END
        End If
    Else
'ADD ���j�b�g�I��ǉ��E�ړ������\�� 240a
'        If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��") _
'            And (SelectedCommand <> "�ړ�" _
'                And SelectedCommand <> "�e���|�[�g" _
'                And SelectedCommand <> "�W�����v") _
'        Then
        If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��" Or CommandState = "���j�b�g�I��") Then
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
    
    '�}�b�v���h���b�O���H
    If IsDragging And Button = 1 Then
        '�w���̈ړ��ʂ��Z�o
        MapX = PrevMapX - (X - PrevMouseX) \ 32
        If MapX < 1 Then
            MapX = 1
        ElseIf MapX > HScroll.max Then
            MapX = HScroll.max
        End If
        
        '�x���̈ړ��ʂ��Z�o
        MapY = PrevMapY - (Y - PrevMouseY) \ 32
        If MapY < 1 Then
            MapY = 1
        ElseIf MapY > VScroll.max Then
            MapY = VScroll.max
        End If
        
        If MapFileName = "" Then
            '�X�e�[�^�X��ʂ̏ꍇ�͈ړ��ʂ�����
            MapX = 8
            If MapY < 8 Then
                MapY = 8
            ElseIf MapY > MapHeight - 7 Then
                MapY = MapHeight - 7
            End If
        End If
        
        '�}�b�v��ʂ�V�������W�ōX�V
        If Not MapX = LastMapX Or Not MapY = LastMapY Then
            RefreshScreen
        End If
    End If
End Sub

'�}�b�v��ʏ�Ń}�E�X�{�^���𗣂�
Private Sub picMain_MouseUp(Index As Integer, Button As Integer, Shift As Integer, _
    X As Single, Y As Single)
    
    If IsGUILocked Then
        Exit Sub
    End If
    '�}�b�v��ʂ̃h���b�O������
    IsDragging = False
End Sub

'�a�f�l�A���Đ��p�^�C�}�[
Private Sub Timer1_Timer()
    If BGMFileName <> "" Then
        If RepeatMode Then
            RestartBGM
        End If
    End If
End Sub

'�}�b�v�E�B���h�E�̏c�X�N���[���𑀍�
Private Sub VScroll_Change()
    MapY = VScroll.Value
    
    If MapFileName = "" Then
        '�X�e�[�^�X��ʂ̏ꍇ�͈ړ��ʂ𐧌�
        If MapY < 8 Then
            MapY = 8
        ElseIf MapY > MapHeight - 7 Then
            MapY = MapHeight - 7
        End If
    End If
    
    '�}�b�v��ʂ��X�V
    If frmMain.Visible Then
        RefreshScreen
    End If
End Sub
