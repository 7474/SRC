VERSION 5.00
Begin VB.Form frmConfiguration 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '�Œ��޲�۸�
   Caption         =   "�ݒ�ύX"
   ClientHeight    =   6075
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   5190
   ForeColor       =   &H00000000&
   Icon            =   "Configuration.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   6075
   ScaleWidth      =   5190
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows �̊���l
   Begin VB.CheckBox chkExtendedAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�퓬�A�j���̊g���@�\���g�p����"
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   720
      TabIndex        =   18
      Top             =   960
      Width           =   3495
   End
   Begin VB.CheckBox chkMoveAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�ړ��A�j����\������"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   17
      Top             =   2280
      Width           =   3735
   End
   Begin VB.CheckBox chkWeaponAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "���폀���A�j���������I��\������"
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   720
      TabIndex        =   16
      Top             =   1440
      Width           =   3495
   End
   Begin VB.TextBox txtMP3Volume 
      Alignment       =   2  '��������
      BackColor       =   &H00FFFFFF&
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   285
      Left            =   1305
      TabIndex        =   11
      Text            =   "100"
      Top             =   4905
      Width           =   495
   End
   Begin VB.HScrollBar hscMP3Volume 
      Height          =   255
      LargeChange     =   10
      Left            =   1920
      Max             =   100
      TabIndex        =   12
      Top             =   4920
      Value           =   50
      Width           =   2295
   End
   Begin VB.ComboBox cboMidiReset 
      BackColor       =   &H00FFFFFF&
      ForeColor       =   &H00000000&
      Height          =   300
      Left            =   2520
      TabIndex        =   10
      Top             =   4440
      Width           =   1725
   End
   Begin VB.CheckBox chkUseDirectMusic 
      BackColor       =   &H00C0C0C0&
      Caption         =   "MIDI���t��DirectMusic���g�p���� (�v�ċN��)"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   9
      Top             =   4080
      Width           =   4215
   End
   Begin VB.CommandButton cmdCancel 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      Caption         =   "�L�����Z��"
      Height          =   375
      Left            =   3240
      Style           =   1  '���̨���
      TabIndex        =   14
      Top             =   5400
      Width           =   1455
   End
   Begin VB.CommandButton cmdOK 
      Appearance      =   0  '�ׯ�
      BackColor       =   &H00C0C0C0&
      Caption         =   "OK"
      Height          =   375
      Left            =   1680
      Style           =   1  '���̨���
      TabIndex        =   13
      Top             =   5400
      Width           =   1455
   End
   Begin VB.CheckBox chkKeepEnemyBGM 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�G�t�F�C�Y���ɂa�f�l��ύX���Ȃ�"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   8
      Top             =   3720
      Width           =   3735
   End
   Begin VB.ComboBox cboMessageSpeed 
      BackColor       =   &H00FFFFFF&
      ForeColor       =   &H00000000&
      Height          =   300
      Left            =   2160
      TabIndex        =   2
      Top             =   240
      Width           =   2055
   End
   Begin VB.CheckBox chkAutoMoveCursor 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�}�E�X�J�[�\���������I�Ɉړ�����"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   5
      Top             =   2640
      Width           =   3735
   End
   Begin VB.CheckBox chkShowSquareLine 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�}�X�ڂ�\������ (�v�ċN��)"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   6
      Top             =   3000
      Width           =   3975
   End
   Begin VB.CheckBox chkShowTurn 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�����t�F�C�Y�J�n���Ƀ^�[���\�����s��"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   7
      Top             =   3360
      Width           =   3735
   End
   Begin VB.CheckBox chkSpecialPowerAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�X�y�V�����p���[�A�j����\������"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   4
      Top             =   1920
      Width           =   3735
   End
   Begin VB.CheckBox chkBattleAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "�퓬�A�j����\������"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   3
      Top             =   600
      Width           =   3735
   End
   Begin VB.Label labMP3Volume 
      BackStyle       =   0  '����
      Caption         =   "MP3����"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   495
      TabIndex        =   15
      Top             =   4950
      Width           =   735
   End
   Begin VB.Label labMidiReset 
      BackStyle       =   0  '����
      Caption         =   "MIDI�������Z�b�g�̎��"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   495
      TabIndex        =   1
      Top             =   4515
      Width           =   2880
   End
   Begin VB.Label labMessageSpeed 
      BackStyle       =   0  '����
      Caption         =   "���b�Z�[�W�X�s�[�h"
      BeginProperty Font 
         Name            =   "�l�r �o�S�V�b�N"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   490
      TabIndex        =   0
      Top             =   295
      Width           =   1935
   End
End
Attribute VB_Name = "frmConfiguration"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�}�b�v�R�}���h�u�ݒ�ύX�v�p�_�C�A���O


'MP3Volume���L�^
Private SavedMP3Volume As Integer

'�퓬�A�j��On�EOff�؂�ւ�
Private Sub chkBattleAnimation_Click()
    '�퓬�A�j����\�����Ȃ��ꍇ�͊g���퓬�A�j���A����A�j���I���̍��ڂ�I��s�\�ɂ���
    If chkBattleAnimation.Value = 1 Then
        chkExtendedAnimation.Enabled = True
        chkWeaponAnimation.Enabled = True
    Else
        chkExtendedAnimation.Enabled = False
        chkWeaponAnimation.Enabled = False
    End If
End Sub

'�L�����Z���{�^���������ꂽ
Private Sub cmdCancel_Click()
    '�_�C�A���O�����
    Hide
    
    'MP3���ʂ݂̂��̏�ŕύX���Ă���̂Ō��ɖ߂��K�v������
    MP3Volume = SavedMP3Volume
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

'OK�{�^���������ꂽ
Private Sub cmdOK_Click()
    '�e��ݒ��ύX
    
    '���b�Z�[�W�X�s�[�h
    Select Case cboMessageSpeed.Text
        Case "�_�̗̈�"
            MessageWait = 0
        Case "������"
            MessageWait = 200
        Case "����"
            MessageWait = 400
        Case "����"
            MessageWait = 700
        Case "�ᑬ"
            MessageWait = 1000
        Case "�蓮����"
            MessageWait = 10000000
    End Select
    WriteIni "Option", "MessageWait", Format$(MessageWait)
    
    '�퓬�A�j���\��
    If chkBattleAnimation.Value = 1 Then
        BattleAnimation = True
        WriteIni "Option", "BattleAnimation", "On"
    Else
        BattleAnimation = False
        WriteIni "Option", "BattleAnimation", "Off"
    End If
    
    '�g��퓬�A�j���\��
    If chkExtendedAnimation.Value = 1 Then
        ExtendedAnimation = True
        WriteIni "Option", "ExtendedAnimation", "On"
    Else
        ExtendedAnimation = False
        WriteIni "Option", "Extendednimation", "Off"
    End If
    
    '���폀���A�j���\��
    If chkWeaponAnimation.Value = 1 Then
        WeaponAnimation = True
        WriteIni "Option", "WeaponAnimation", "On"
    Else
        WeaponAnimation = False
        WriteIni "Option", "WeaponAnimation", "Off"
    End If
    
    '�ړ��A�j���\��
    If chkMoveAnimation.Value = 1 Then
        MoveAnimation = True
        WriteIni "Option", "MoveAnimation", "On"
    Else
        MoveAnimation = False
        WriteIni "Option", "MoveAnimation", "Off"
    End If
    
    '�X�y�V�����p���[�A�j���\��
    If chkSpecialPowerAnimation.Value = 1 Then
        SpecialPowerAnimation = True
        WriteIni "Option", "SpecialPowerAnimation", "On"
    Else
        SpecialPowerAnimation = False
        WriteIni "Option", "SpecialPowerAnimation", "Off"
    End If
    
    '�}�E�X�J�[�\���̎����ړ�
    If chkAutoMoveCursor.Value Then
        AutoMoveCursor = True
        WriteIni "Option", "AutoMoveCursor", "On"
    Else
        AutoMoveCursor = False
        WriteIni "Option", "AutoMoveCursor", "Off"
    End If
    
    '�}�X�ڂ̕\��
    If chkShowSquareLine.Value Then
        ShowSquareLine = True
        WriteIni "Option", "Square", "On"
    Else
        ShowSquareLine = False
        WriteIni "Option", "Square", "Off"
    End If
    
    '�����t�F�C�Y�J�n���̃^�[���\��
    If chkShowTurn.Value Then
        WriteIni "Option", "Turn", "On"
    Else
        WriteIni "Option", "Turn", "Off"
    End If
    
    '�G�t�F�C�Y���ɂa�f�l��ύX���Ȃ�
    If chkKeepEnemyBGM.Value Then
        KeepEnemyBGM = True
        WriteIni "Option", "KeepEnemyBGM", "On"
    Else
        KeepEnemyBGM = False
        WriteIni "Option", "KeepEnemyBGM", "Off"
    End If
    
    'MIDI���t��DirectMusic���g�p����
    If chkUseDirectMusic.Value Then
        WriteIni "Option", "UseDirectMusic", "On"
    Else
        WriteIni "Option", "UseDirectMusic", "Off"
    End If
    
    'MIDI�������Z�b�g�̎��
    MidiResetType = cboMidiReset.Text
    WriteIni "Option", "MidiReset", cboMidiReset.Text
    
    'MP3�Đ�����
    WriteIni "Option", "MP3Volume", Format$(MP3Volume)
    
    '�_�C�A���O�����
    Hide
End Sub

Private Sub Form_Load()
    '�_�C�A���O��������
    
    '���b�Z�[�W�X�s�[�h
    cboMessageSpeed.AddItem "�蓮����"
    cboMessageSpeed.AddItem "�ᑬ"
    cboMessageSpeed.AddItem "����"
    cboMessageSpeed.AddItem "����"
    cboMessageSpeed.AddItem "������"
    cboMessageSpeed.AddItem "�_�̗̈�"
    Select Case MessageWait
        Case 0
            cboMessageSpeed.Text = "�_�̗̈�"
        Case 200
            cboMessageSpeed.Text = "������"
        Case 400
            cboMessageSpeed.Text = "����"
        Case 700
            cboMessageSpeed.Text = "����"
        Case 1000
            cboMessageSpeed.Text = "�ᑬ"
        Case 10000000
            cboMessageSpeed.Text = "�蓮����"
    End Select
    
    '�퓬�A�j���\��
    If BattleAnimation Then
        chkBattleAnimation.Value = 1
    Else
        chkBattleAnimation.Value = 0
    End If
    If Not FileExists(AppPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
        chkBattleAnimation.Value = 2 '����
    End If
    
    '�g���퓬�A�j���\��
    If ExtendedAnimation Then
        chkExtendedAnimation.Value = 1
    Else
        chkExtendedAnimation.Value = 0
    End If
    If chkBattleAnimation.Value = 1 Then
        chkExtendedAnimation.Enabled = True
    Else
        chkExtendedAnimation.Enabled = False
    End If
    
    '���폀���A�j���\��
    If WeaponAnimation Then
        chkWeaponAnimation.Value = 1
    Else
        chkWeaponAnimation.Value = 0
    End If
    If chkBattleAnimation.Value = 1 Then
        chkWeaponAnimation.Enabled = True
    Else
        chkWeaponAnimation.Enabled = False
    End If
    
    '�ړ��A�j���\��
    If MoveAnimation Then
        chkMoveAnimation.Value = 1
    Else
        chkMoveAnimation.Value = 0
    End If
    
    '�X�y�V�����p���[�A�j���\��
    If SpecialPowerAnimation Then
        chkSpecialPowerAnimation.Value = 1
    Else
        chkSpecialPowerAnimation.Value = 0
    End If
    
    '�}�E�X�J�[�\���̎����ړ�
    If AutoMoveCursor Then
        chkAutoMoveCursor.Value = 1
    Else
        chkAutoMoveCursor.Value = 0
    End If
    
    '�}�X�ڂ̕\��
    If ShowSquareLine Then
        chkShowSquareLine.Value = 1
    Else
        chkShowSquareLine.Value = 0
    End If
    
    '�����t�F�C�Y�J�n���̃^�[���\��
    If LCase$(ReadIni("Option", "Turn")) = "on" Then
        chkShowTurn.Value = 1
    Else
        chkShowTurn.Value = 0
    End If
    
    '�G�t�F�C�Y���ɂa�f�l��ύX���Ȃ�
    If KeepEnemyBGM Then
        chkKeepEnemyBGM.Value = 1
    Else
        chkKeepEnemyBGM.Value = 0
    End If
    
    'MIDI���t��DirectMusic���g�p����
    If LCase$(ReadIni("Option", "UseDirectMusic")) = "on" Then
        chkUseDirectMusic.Value = 1
    Else
        chkUseDirectMusic.Value = 0
    End If
    
    'MIDI�������Z�b�g�̎��
    cboMidiReset.AddItem "None"
    cboMidiReset.AddItem "GM"
    cboMidiReset.AddItem "GS"
    cboMidiReset.AddItem "XG"
    cboMidiReset.Text = MidiResetType
    
    'MP3����
    SavedMP3Volume = MP3Volume
    txtMP3Volume = Format$(MP3Volume)
End Sub

'MP3���ʕύX
Private Sub hscMP3Volume_Change()
    MP3Volume = hscMP3Volume.Value
    txtMP3Volume.Text = Format$(MP3Volume)
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

Private Sub hscMP3Volume_Scroll()
    MP3Volume = hscMP3Volume.Value
    txtMP3Volume.Text = Format$(MP3Volume)
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

Private Sub txtMP3Volume_Change()
    MP3Volume = StrToLng(txtMP3Volume.Text)
    
    If MP3Volume < 0 Then
        MP3Volume = 0
        txtMP3Volume.Text = "0"
    ElseIf MP3Volume > 100 Then
        MP3Volume = 100
        txtMP3Volume.Text = "100"
    End If
    
    hscMP3Volume.Value = MP3Volume
    
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub
