VERSION 5.00
Begin VB.Form frmListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '�Œ�(����)
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
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   137
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   654
   Begin VB.Timer Timer2 
      Interval        =   100
      Left            =   5400
      Top             =   1320
   End
   Begin VB.TextBox txtComment 
      Enabled         =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �S�V�b�N"
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
      Height          =   195
      Left            =   5775
      TabIndex        =   22
      Text            =   "100"
      Top             =   345
      Visible         =   0   'False
      Width           =   375
   End
   Begin VB.TextBox txtMorale1 
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
      Height          =   195
      Left            =   885
      TabIndex        =   20
      Text            =   "100"
      Top             =   330
      Visible         =   0   'False
      Width           =   375
   End
   Begin VB.TextBox txtLevel2 
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
      Height          =   195
      Left            =   5865
      TabIndex        =   19
      Text            =   "99"
      Top             =   105
      Visible         =   0   'False
      Width           =   255
   End
   Begin VB.TextBox txtLevel1 
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
      Height          =   195
      Left            =   990
      TabIndex        =   17
      Text            =   "99"
      Top             =   90
      Visible         =   0   'False
      Width           =   255
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
      ScaleMode       =   3  '�߸��
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   75
      TabIndex        =   9
      Top             =   390
      Visible         =   0   'False
      Width           =   1185
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
      Height          =   195
      Left            =   3945
      TabIndex        =   8
      Text            =   "999/999"
      Top             =   120
      Visible         =   0   'False
      Width           =   855
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
      ScaleMode       =   3  '�߸��
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
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   108
      TabIndex        =   5
      Top             =   405
      Visible         =   0   'False
      Width           =   1680
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
      Height          =   195
      Left            =   7095
      TabIndex        =   4
      Text            =   "99999/99999"
      Top             =   120
      Visible         =   0   'False
      Width           =   1320
   End
   Begin VB.PictureBox picUnit2 
      Appearance      =   0  '�ׯ�
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   6180
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   32
      TabIndex        =   3
      Top             =   75
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.PictureBox picUnit1 
      Appearance      =   0  '�ׯ�
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000005&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H80000008&
      Height          =   480
      Left            =   1275
      ScaleHeight     =   32
      ScaleMode       =   3  '�߸��
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
         Name            =   "�l�r ����"
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
      FillStyle       =   0  '�h��Ԃ�
      ForeColor       =   &H00800000&
      Height          =   195
      Left            =   90
      ScaleHeight     =   9
      ScaleMode       =   3  '�߸��
      ScaleWidth      =   639
      TabIndex        =   25
      Top             =   1845
      Visible         =   0   'False
      Width           =   9645
   End
   Begin VB.Label labCaption 
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '����
      BeginProperty Font 
         Name            =   "�l�r ����"
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
         Name            =   "�l�r �o����"
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
         Name            =   "�l�r �o����"
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
         Name            =   "�l�r �o����"
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
      BorderStyle     =   1  '����
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
         Name            =   "�l�r �o����"
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
      BorderStyle     =   1  '����
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
         Name            =   "�l�r �o����"
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
         Name            =   "�l�r �o����"
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
         Name            =   "�l�r �o����"
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
         Name            =   "�l�r �o����"
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
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���X�g�{�b�N�X�̃t�H�[��

'���X�g�{�b�N�X�̃T�C�Y (�ʏ�:M ��^:L)
'��
Public HorizontalSize As String
'����
Public VerticalSize As String

'Question�R�}���h�p�ϐ�
Public CurrentTime As Integer
Public TimeLimit As Integer

'�Ō�ɑI�����ꂽ�A�C�e��
Private LastSelectedItem As Integer

'���X�g�{�b�N�X�ւ̃L�[����
Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    '���ɃE�B���h�E���B��Ă���ꍇ�͖���
    If Not Visible Then
        Exit Sub
    End If
    
    Select Case KeyCode
        Case vbKeyUp
        Case vbKeyDown
        Case vbKeyLeft, vbKeyRight
        Case vbKeyEscape, vbKeyDelete, vbKeyBack
            '�L�����Z��
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
        Case Else
            '�I��
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

'���X�g�{�b�N�X���J��
Private Sub Form_Load()
Dim ret As Long
    
    '���X�g�{�b�N�X����Ɏ�O�ɕ\��
    ret = SetWindowPos(hwnd, -1, 0, 0, 0, 0, &H3)
    HorizontalSize = "M"
    VerticalSize = "M"
End Sub

'���X�g�{�b�N�X�����
Private Sub Form_Unload(Cancel As Integer)
    TopItem = lstItems.TopIndex + 1
    IsFormClicked = True
    If Not IsMordal And Visible Then
        Cancel = 1
    End If
    Hide
End Sub

'���ڂ��_�u���N���b�N
Private Sub lstItems_DblClick()
    '�����ȃA�C�e�����I������Ă���H
    If lstItems.ListIndex < 0 Then
        Exit Sub
    End If
    If UBound(ListItemFlag) >= lstItems.ListIndex + 1 Then
        If ListItemFlag(lstItems.ListIndex + 1) Then
            Exit Sub
        End If
    End If
    
    If LastSelectedItem <> 0 Then
        '�A���őI��
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
        '�A���ŃL�����Z��
        SelectedItem = 0
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex + 1
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'�}�E�X�ŃN���b�N
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            '�I��
            If Not Visible Then
                Exit Sub
            End If
            
            '�����ȃA�C�e�����I������Ă���H
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
            '�L�����Z��
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex + 1
            IsFormClicked = True
    End Select
End Sub

'�L���v�V�����������N���b�N
Private Sub labCaption_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            '���j�b�g�X�e�[�^�X��\�����Ă��郆�j�b�g�����ւ�
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
            '�L�����Z��
            SelectedItem = 0
            LastSelectedItem = SelectedItem
            TopItem = lstItems.TopIndex
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
    End Select
End Sub

'���X�g�{�b�N�X�̒[���N���b�N
Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    If Button = 2 Then
        '�L�����Z��
        SelectedItem = 0
        LastSelectedItem = SelectedItem
        TopItem = lstItems.TopIndex
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'���X�g�{�b�N�X��Ń}�E�X�J�[�\���𓮂���
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim itm As Integer
    
    '�J�[�\��������A�C�e�����Z�o
    itm = ((Y * ScaleWidth) \ Width + 1) \ 16
    itm = itm + lstItems.TopIndex
    
    '�J�[�\����̃A�C�e�����n�C���C�g�\��
    If itm < 0 Or itm >= lstItems.ListCount Then
        lstItems.ListIndex = -1
        Exit Sub
    End If
    If lstItems.ListIndex = itm Then
        Exit Sub
    End If
    lstItems.ListIndex = itm
    
    '�R�����g�����X�V
    If txtComment.Enabled Then
        txtComment.Text = ListItemComment(itm + 1)
    End If
End Sub

'Question�R�}���h�Ή�
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

'�I������Ă���A�C�e���ɑΉ����郆�j�b�g�̃X�e�[�^�X�\��
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
    
    '�I�����ꂽ���j�b�g�����݂���H
    If Not UList.IsDefined2(ListItemID(lstItems.ListIndex + 1)) Then
        Exit Sub
    End If
    
    Set u = UList.Item2(ListItemID(lstItems.ListIndex + 1))
    
    '�I�����ꂽ���j�b�g�Ƀp�C���b�g������Ă���H
    If u.CountPilot = 0 Then
        Exit Sub
    End If
    
    '���ɕ\�����Ă���H
    If DisplayedUnit Is u Then
        Exit Sub
    End If
    
    '�I�����ꂽ���j�b�g���X�e�[�^�X�E�B���h�E�ɕ\��
    DisplayUnitStatus u
    
ErrorHandler:
End Sub

