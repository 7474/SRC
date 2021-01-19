VERSION 5.00
Begin VB.Form frmToolTip 
   BorderStyle     =   0  '�Ȃ�
   ClientHeight    =   1710
   ClientLeft      =   0
   ClientTop       =   0
   ClientWidth     =   3465
   ClipControls    =   0   'False
   LinkTopic       =   "Form2"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   1710
   ScaleWidth      =   3465
   ShowInTaskbar   =   0   'False
   Visible         =   0   'False
   Begin VB.PictureBox picMessage 
      AutoRedraw      =   -1  'True
      BackColor       =   &H80000018&
      BorderStyle     =   0  '�Ȃ�
      ClipControls    =   0   'False
      ForeColor       =   &H00000000&
      Height          =   1560
      Left            =   0
      ScaleHeight     =   104
      ScaleMode       =   3  '�߸��
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
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�c�[���`�b�v�p�t�H�[��

'�t�H�[�������[�h
Private Sub Form_Load()
Dim ret As Long
    
    '��Ɏ�O�ɕ\��
    ret = SetWindowPos(frmToolTip.hwnd, -1, 0, 0, 0, 0, &H3)
End Sub

'�c�[���`�b�v��\��
Public Sub ShowToolTip(msg As String)
Dim ret As Long
Dim PT As POINTAPI
Dim tw As Integer
Static cur_msg As String
    
    tw = Screen.TwipsPerPixelX
    
    If msg <> cur_msg Then
        cur_msg = msg
        With frmToolTip.picMessage
            '���b�Z�[�W���ɃT�C�Y�����킹��
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
            
            '���b�Z�[�W����������
            .CurrentX = 3
            .CurrentY = 2
            frmToolTip.picMessage.Print msg
            
            .ForeColor = vbWhite
            .Refresh
        End With
    End If
    
    '�t�H�[���̈ʒu��ݒ�
    ret = GetCursorPos(PT)
    frmToolTip.Left = PT.X * tw + 0
    frmToolTip.Top = (PT.Y + 24) * tw
    
    '�t�H�[�����A�N�e�B�u�ŕ\��
    ret = ShowWindow(frmToolTip.hwnd, SW_SHOWNA)
End Sub

