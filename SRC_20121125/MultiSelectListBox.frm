VERSION 5.00
Begin VB.Form frmMultiSelectListBox 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '�Œ��޲�۸�
   Caption         =   "MultiSelectListBox"
   ClientHeight    =   4965
   ClientLeft      =   900
   ClientTop       =   2475
   ClientWidth     =   7455
   ClipControls    =   0   'False
   BeginProperty Font 
      Name            =   "�l�r �S�V�b�N"
      Size            =   12
      Charset         =   128
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   Icon            =   "MultiSelectListBox.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'Z ���ް
   ScaleHeight     =   331
   ScaleMode       =   3  '�߸��
   ScaleWidth      =   497
   Begin VB.CommandButton cmdResume 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�}�b�v������"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   1440
      TabIndex        =   7
      TabStop         =   0   'False
      Top             =   4440
      Width           =   2055
   End
   Begin VB.CommandButton cmdSort 
      BackColor       =   &H00C0C0C0&
      Caption         =   "���̏��ɕ��בւ�"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   5040
      TabIndex        =   6
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2175
   End
   Begin VB.CommandButton cmdSelectAll2 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�Ōォ��I��"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   2520
      Style           =   1  '���̨���
      TabIndex        =   5
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2415
   End
   Begin VB.CommandButton cmdSelectAll 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�擪����I��"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   120
      Style           =   1  '���̨���
      TabIndex        =   4
      TabStop         =   0   'False
      Top             =   3960
      Width           =   2295
   End
   Begin VB.Timer Timer1 
      Interval        =   100
      Left            =   6720
      Top             =   2040
   End
   Begin VB.CommandButton cmdFinish 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�I��"
      Enabled         =   0   'False
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   435
      Left            =   3960
      Style           =   1  '���̨���
      TabIndex        =   1
      TabStop         =   0   'False
      Top             =   4440
      Width           =   2055
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
      Height          =   3420
      Left            =   120
      TabIndex        =   0
      Top             =   480
      Width           =   7185
   End
   Begin VB.Label lblNumber 
      Alignment       =   2  '��������
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '����
      Caption         =   "Label1"
      BeginProperty Font 
         Name            =   "�l�r ����"
         Size            =   15.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   435
      Left            =   6360
      TabIndex        =   3
      Top             =   4440
      Width           =   855
   End
   Begin VB.Label lblCaption 
      BackColor       =   &H00FFFFFF&
      BorderStyle     =   1  '����
      Caption         =   "Label1"
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
      Height          =   330
      Left            =   120
      TabIndex        =   2
      Top             =   90
      Width           =   7185
   End
End
Attribute VB_Name = "frmMultiSelectListBox"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�o�����j�b�g�I��p���X�g�{�b�N�X�̃t�H�[��

'�I�����ꂽ���j�b�g�̐�
Private SelectedItemNum As Integer
'���j�b�g���I�����ꂽ���ǂ����������t���O
Private ItemFlag() As Boolean

'�I���I���{�^�����N���b�N
Private Sub cmdFinish_Click()
Dim i As Integer

    For i = 1 To UBound(ListItemFlag)
        ListItemFlag(i) = ItemFlag(i - 1)
    Next
    ClearUnitStatus
    Unload frmMultiSelectListBox
End Sub

'�}�b�v������{�^�����N���b�N
Private Sub cmdResume_Click()
Dim i As Integer

    For i = 1 To UBound(ListItemFlag)
        ListItemFlag(i) = False
    Next
    ClearUnitStatus
    Unload frmMultiSelectListBox
End Sub

'�u�擪����I���v�{�^�����N���b�N
Private Sub cmdSelectAll_Click()
Dim i As Integer

    lstItems.Visible = False
    For i = 1 To lstItems.ListCount
        ItemFlag(i - 1) = False
        lstItems.list(i - 1) = "�@" & Mid$(lstItems.list(i - 1), 2)
    Next
    For i = 1 To MinLng(MaxListItem, lstItems.ListCount)
        If Not ItemFlag(i - 1) Then
            ItemFlag(i - 1) = True
            lstItems.list(i - 1) = "��" & Mid$(lstItems.list(i - 1), 2)
        End If
    Next
    lstItems.TopIndex = 0
    lstItems.Visible = True
    
    SelectedItemNum = 0
    For i = 0 To lstItems.ListCount - 1
        If ItemFlag(i) Then
            SelectedItemNum = SelectedItemNum + 1
        End If
    Next
    lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
    
    If SelectedItemNum > 0 _
        And SelectedItemNum <= MaxListItem _
    Then
        If Not cmdFinish.Enabled Then
            cmdFinish.Enabled = True
        End If
    Else
        If cmdFinish.Enabled Then
            cmdFinish.Enabled = False
        End If
    End If
End Sub

'�u�Ōォ��I���v�{�^�����N���b�N
Private Sub cmdSelectAll2_Click()
Dim i As Integer

    lstItems.Visible = False
    For i = 1 To lstItems.ListCount
        ItemFlag(i - 1) = False
        lstItems.list(i - 1) = "�@" & Mid$(lstItems.list(i - 1), 2)
    Next
    For i = 1 To MinLng(MaxListItem, lstItems.ListCount)
        If Not ItemFlag(lstItems.ListCount - i) Then
            ItemFlag(lstItems.ListCount - i) = True
            lstItems.list(lstItems.ListCount - i) = _
                "��" & Mid$(lstItems.list(lstItems.ListCount - i), 2)
        End If
    Next
    lstItems.TopIndex = MaxLng(lstItems.ListCount - 14, 0)
    lstItems.Visible = True
    
    SelectedItemNum = 0
    For i = 0 To lstItems.ListCount - 1
        If ItemFlag(i) Then
            SelectedItemNum = SelectedItemNum + 1
        End If
    Next
    lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
    
    If SelectedItemNum > 0 _
        And SelectedItemNum <= MaxListItem _
    Then
        If Not cmdFinish.Enabled Then
            cmdFinish.Enabled = True
        End If
    Else
        If cmdFinish.Enabled Then
            cmdFinish.Enabled = False
        End If
    End If
End Sub

'�u�`���v�{�^�����N���b�N
Private Sub cmdSort_Click()
Dim item_list() As String, key_list() As Long, strkey_list() As String
Dim max_item As Integer, max_value As Long, max_str As String
Dim i As Integer, j As Integer, buf As String, flag As Boolean
    
    '���݂̃��X�g�\�����e���R�s�[
    With lstItems
        ReDim item_list(.ListCount)
        For i = 1 To .ListCount
            item_list(i) = .list(i - 1)
        Next
    End With
    
    If cmdSort.Caption = "���x�����ɕ��בւ�" Then
        '���C���p�C���b�g�̃��x�����ɕ��בւ�
        ReDim key_list(UBound(item_list))
        With UList
            For i = 1 To UBound(item_list)
                With .Item(ListItemID(i)).MainPilot
                    key_list(i) = 500 * CLng(.Level) + CLng(.Exp)
                End With
            Next
        End With
        For i = 1 To UBound(item_list) - 1
            max_item = i
            max_value = key_list(i)
            For j = i + 1 To UBound(item_list)
                If key_list(j) > max_value Then
                    max_item = j
                    max_value = key_list(j)
                End If
            Next
            If max_item <> i Then
                buf = item_list(i)
                item_list(i) = item_list(max_item)
                item_list(max_item) = buf
                
                buf = ListItemID(i)
                ListItemID(i) = ListItemID(max_item)
                ListItemID(max_item) = buf
                
                flag = ItemFlag(i - 1)
                ItemFlag(i - 1) = ItemFlag(max_item - 1)
                ItemFlag(max_item - 1) = flag
                
                key_list(max_item) = key_list(i)
            End If
        Next
        '���בւ����@���g�O���Ő؂�ւ�
        cmdSort.Caption = "���̏��ɕ��בւ�"
    Else
        '���j�b�g�̖��̏��ɕ��בւ�
        ReDim strkey_list(UBound(item_list))
        With UList
            For i = 1 To UBound(item_list)
                strkey_list(i) = .Item(ListItemID(i)).KanaName
            Next
        End With
        For i = 1 To UBound(item_list) - 1
            max_item = i
            max_str = strkey_list(i)
            For j = i + 1 To UBound(item_list)
                If StrComp(strkey_list(j), max_str, 1) = -1 Then
                    max_item = j
                    max_str = strkey_list(j)
                End If
            Next
            If max_item <> i Then
                buf = item_list(i)
                item_list(i) = item_list(max_item)
                item_list(max_item) = buf
                
                buf = ListItemID(i)
                ListItemID(i) = ListItemID(max_item)
                ListItemID(max_item) = buf
                
                flag = ItemFlag(i - 1)
                ItemFlag(i - 1) = ItemFlag(max_item - 1)
                ItemFlag(max_item - 1) = flag
                
                strkey_list(max_item) = strkey_list(i)
            End If
        Next
        '���בւ����@���g�O���Ő؂�ւ�
        cmdSort.Caption = "���x�����ɕ��בւ�"
    End If
    
    '���X�g�\�����X�V����
    With lstItems
        .Visible = False
        For i = 1 To .ListCount
            .list(i - 1) = item_list(i)
        Next
        .TopIndex = 0
        .Visible = True
    End With
End Sub

'�t�H�[����\��
Private Sub Form_Activate()
    SelectedItemNum = 0
    lblNumber = "0/" & Format$(MaxListItem)
    ReDim ItemFlag(lstItems.ListCount)
    If lstItems.ListCount > 0 Then
        DisplayUnitStatus UList.Item(ListItemID(1))
    End If
End Sub

'���X�g�{�b�N�X��Ń_�u���N���b�N
Private Sub lstItems_DblClick()
Dim i As Integer

    i = lstItems.ListIndex
    If i >= 0 Then
        If ItemFlag(i) Then
            '�I��������
            
            '�I�����ꂽ���j�b�g�������炷
            SelectedItemNum = SelectedItemNum - 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = False
            
            '�I����Ԃ̕\�����X�V
            lstItems.list(i) = "�@" & Mid$(lstItems.list(i), 2)
            
            '�I���I�����\������
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        Else
            '�I��
            
            '�I�����ꂽ���j�b�g���𑝂₷
            SelectedItemNum = SelectedItemNum + 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = True
            
            '�I����Ԃ̕\�����X�V
            lstItems.list(i) = "��" & Mid$(lstItems.list(i), 2)
            
            '�I���I�����\������
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        End If
    End If
End Sub

'���X�g�{�b�N�X��Ń}�E�X�{�^��������
Private Sub lstItems_MouseDown(Button As Integer, Shift As Integer, X As Single, Y As Single)
Dim i As Integer
    
    '���N���b�N�ȊO�͖���
    If Button <> 1 Then
        Exit Sub
    End If
    
    i = lstItems.ListIndex
    If i >= 0 Then
        If ItemFlag(i) Then
            '�I��������
            
            '�I�����ꂽ���j�b�g�������炷
            SelectedItemNum = SelectedItemNum - 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = False
            
            '�I����Ԃ̕\�����X�V
            lstItems.list(i) = "�@" & Mid$(lstItems.list(i), 2)
            
            '�I���I�����\������
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        Else
            '�I��
            
            '�I�����ꂽ���j�b�g���𑝂₷
            SelectedItemNum = SelectedItemNum + 1
            lblNumber = Format$(SelectedItemNum) & "/" & Format$(MaxListItem)
            ItemFlag(i) = True
            
            '�I����Ԃ̕\�����X�V
            lstItems.list(i) = "��" & Mid$(lstItems.list(i), 2)
            
            '�I���I�����\������
            If SelectedItemNum > 0 _
                And SelectedItemNum <= MaxListItem _
            Then
                If Not cmdFinish.Enabled Then
                    cmdFinish.Enabled = True
                End If
            Else
                If cmdFinish.Enabled Then
                    cmdFinish.Enabled = False
                End If
            End If
        End If
    End If
End Sub

'���X�g�{�b�N�X��Ń}�E�X�J�[�\�����ړ�
Private Sub lstItems_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Dim itm As Integer
    
    '�J�[�\��������A�C�e�����Z�o
    itm = ((Y * ScaleWidth) \ Width + 1) \ 16
    itm = itm + lstItems.TopIndex
    
    '�J�[�\��������A�C�e�����n�C���C�g�\��
    If itm < 0 Or itm >= lstItems.ListCount Then
        lstItems.ListIndex = -1
        Exit Sub
    End If
    If lstItems.ListIndex = itm Then
        Exit Sub
    End If
    lstItems.ListIndex = itm
End Sub

'�J�[�\�����w�����j�b�g����莞�Ԃ��Ƃɒ��ׂăX�e�[�^�X�E�B���h�E�ɕ\��
Private Sub Timer1_Timer()
Dim u As Unit

    If Not Visible Then
        Exit Sub
    End If
    
    If lstItems.ListIndex = -1 Then
        Exit Sub
    End If
    
    Set u = UList.Item(ListItemID(lstItems.ListIndex + 1))
    
    If Not DisplayedUnit Is u Then
'���j�b�g�I�𒆂���
        If CommandState = "���j�b�g�I��" Then
           DisplayUnitStatus u
        End If
    End If
End Sub
