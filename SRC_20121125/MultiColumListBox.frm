VERSION 5.00
Begin VB.Form frmMultiColumnListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   1  '�Œ�(����)
   Caption         =   "MultiColumListBox"
   ClientHeight    =   6690
   ClientLeft      =   1080
   ClientTop       =   1740
   ClientWidth     =   10050
   ClipControls    =   0   'False
   Icon            =   "MultiColumListBox.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   446
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   670
   Begin VB.ListBox lstItems 
      BackColor       =   &H00FFFFFF&
      Columns         =   4
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
      Height          =   6060
      ItemData        =   "MultiColumListBox.frx":030A
      Left            =   120
      List            =   "MultiColumListBox.frx":030C
      TabIndex        =   0
      Top             =   120
      Width           =   9810
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
      Left            =   120
      TabIndex        =   1
      Top             =   6240
      Width           =   9810
   End
End
Attribute VB_Name = "frmMultiColumnListBox"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���i�̃��X�g�{�b�N�X�̃t�H�[��

'�t�H�[����\��
Private Sub Form_Activate()
    SelectedItem = 0
    labCaption.Caption = ""
End Sub

'�t�H�[�������[�h
Private Sub Form_Load()
Dim ret As Long
    
    '��Ɏ�O�ɕ\��
    ret = SetWindowPos(hwnd, -1, 0, 0, 0, 0, &H3)
End Sub

'�t�H�[�������
Private Sub Form_Unload(Cancel As Integer)
    TopItem = lstItems.TopIndex + 1
    IsFormClicked = True
    If Not IsMordal And Visible Then
        Cancel = 1
    End If
    Hide
End Sub

'�t�H�[����Ń}�E�X�{�^��������
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case Button
        Case 1
            '�I��
            If Not Visible Then
                Exit Sub
            End If
            If lstItems.ListIndex < 0 _
                Or ListItemFlag(lstItems.ListIndex + 1) _
            Then
                Exit Sub
            End If
            SelectedItem = lstItems.ListIndex + 1
            TopItem = lstItems.TopIndex + 1
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
        Case 2
            '�L�����Z��
            SelectedItem = 0
            TopItem = lstItems.TopIndex + 1
            If IsFormClicked Then
                Hide
            End If
            IsFormClicked = True
    End Select
End Sub

'�t�H�[����Ń}�E�X�{�^��������
Private Sub Form_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
    If Button = 2 Then
        '�L�����Z���̂ݎ󂯕t��
        SelectedItem = 0
        TopItem = lstItems.TopIndex
        If IsFormClicked Then
            Hide
        End If
        IsFormClicked = True
    End If
End Sub

'���X�g�{�b�N�X��Ń}�E�X�J�[�\�����ړ�
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim itm As Integer
Dim lines As Integer
    
    With lstItems
        '���X�g�{�b�N�X�̍s��
        lines = 25
        If .ListCount > lines * .Columns Then
            lines = lines - 1
        End If
        
        '�}�E�X�J�[�\��������A�C�e�����Z�o
        itm = (((X * ScaleWidth) \ Width) \ (.Width \ .Columns)) * lines
        itm = itm + ((Y * ScaleWidth) \ Width + 1) \ 16
        itm = itm + .TopIndex
        
        '�J�[�\����̃A�C�e�����n�C���C�g�\��
        If itm < 0 Or itm >= .ListCount Then
            .ListIndex = -1
            Exit Sub
        End If
        If .ListIndex = itm Then
            Exit Sub
        End If
        .ListIndex = itm
        
        '����̕\��
        labCaption.Caption = ListItemComment(itm + 1)
    End With
End Sub
