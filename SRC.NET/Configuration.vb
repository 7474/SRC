Option Strict Off
Option Explicit On
Friend Class frmConfiguration
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�}�b�v�R�}���h�u�ݒ�ύX�v�p�_�C�A���O
	
	
	'MP3Volume���L�^
	Private SavedMP3Volume As Short
	
	'�퓬�A�j��On�EOff�؂�ւ�
	'UPGRADE_WARNING: �C�x���g chkBattleAnimation.CheckStateChanged �́A�t�H�[�������������ꂽ�Ƃ��ɔ������܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' ���N���b�N���Ă��������B
	Private Sub chkBattleAnimation_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkBattleAnimation.CheckStateChanged
		'�퓬�A�j����\�����Ȃ��ꍇ�͊g���퓬�A�j���A����A�j���I���̍��ڂ�I��s�\�ɂ���
		If chkBattleAnimation.CheckState = 1 Then
			chkExtendedAnimation.Enabled = True
			chkWeaponAnimation.Enabled = True
		Else
			chkExtendedAnimation.Enabled = False
			chkWeaponAnimation.Enabled = False
		End If
	End Sub
	
	'�L�����Z���{�^���������ꂽ
	Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
		Dim IsMP3Supported As Object
		'�_�C�A���O�����
		Hide()
		
		'MP3���ʂ݂̂��̏�ŕύX���Ă���̂Ō��ɖ߂��K�v������
		MP3Volume = SavedMP3Volume
		'UPGRADE_WARNING: �I�u�W�F�N�g IsMP3Supported �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'OK�{�^���������ꂽ
	Private Sub cmdOK_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOK.Click
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
		WriteIni("Option", "MessageWait", VB6.Format(MessageWait))
		
		'�퓬�A�j���\��
		If chkBattleAnimation.CheckState = 1 Then
			BattleAnimation = True
			WriteIni("Option", "BattleAnimation", "On")
		Else
			BattleAnimation = False
			WriteIni("Option", "BattleAnimation", "Off")
		End If
		
		'�g��퓬�A�j���\��
		If chkExtendedAnimation.CheckState = 1 Then
			ExtendedAnimation = True
			WriteIni("Option", "ExtendedAnimation", "On")
		Else
			ExtendedAnimation = False
			WriteIni("Option", "Extendednimation", "Off")
		End If
		
		'���폀���A�j���\��
		If chkWeaponAnimation.CheckState = 1 Then
			WeaponAnimation = True
			WriteIni("Option", "WeaponAnimation", "On")
		Else
			WeaponAnimation = False
			WriteIni("Option", "WeaponAnimation", "Off")
		End If
		
		'�ړ��A�j���\��
		If chkMoveAnimation.CheckState = 1 Then
			MoveAnimation = True
			WriteIni("Option", "MoveAnimation", "On")
		Else
			MoveAnimation = False
			WriteIni("Option", "MoveAnimation", "Off")
		End If
		
		'�X�y�V�����p���[�A�j���\��
		If chkSpecialPowerAnimation.CheckState = 1 Then
			SpecialPowerAnimation = True
			WriteIni("Option", "SpecialPowerAnimation", "On")
		Else
			SpecialPowerAnimation = False
			WriteIni("Option", "SpecialPowerAnimation", "Off")
		End If
		
		'�}�E�X�J�[�\���̎����ړ�
		If chkAutoMoveCursor.CheckState Then
			AutoMoveCursor = True
			WriteIni("Option", "AutoMoveCursor", "On")
		Else
			AutoMoveCursor = False
			WriteIni("Option", "AutoMoveCursor", "Off")
		End If
		
		'�}�X�ڂ̕\��
		If chkShowSquareLine.CheckState Then
			ShowSquareLine = True
			WriteIni("Option", "Square", "On")
		Else
			ShowSquareLine = False
			WriteIni("Option", "Square", "Off")
		End If
		
		'�����t�F�C�Y�J�n���̃^�[���\��
		If chkShowTurn.CheckState Then
			WriteIni("Option", "Turn", "On")
		Else
			WriteIni("Option", "Turn", "Off")
		End If
		
		'�G�t�F�C�Y���ɂa�f�l��ύX���Ȃ�
		If chkKeepEnemyBGM.CheckState Then
			KeepEnemyBGM = True
			WriteIni("Option", "KeepEnemyBGM", "On")
		Else
			KeepEnemyBGM = False
			WriteIni("Option", "KeepEnemyBGM", "Off")
		End If
		
		'MIDI���t��DirectMusic���g�p����
		If chkUseDirectMusic.CheckState Then
			WriteIni("Option", "UseDirectMusic", "On")
		Else
			WriteIni("Option", "UseDirectMusic", "Off")
		End If
		
		'MIDI�������Z�b�g�̎��
		MidiResetType = cboMidiReset.Text
		WriteIni("Option", "MidiReset", (cboMidiReset.Text))
		
		'MP3�Đ�����
		WriteIni("Option", "MP3Volume", VB6.Format(MP3Volume))
		
		'�_�C�A���O�����
		Hide()
	End Sub
	
	Private Sub frmConfiguration_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		'�_�C�A���O��������
		
		'���b�Z�[�W�X�s�[�h
		cboMessageSpeed.Items.Add("�蓮����")
		cboMessageSpeed.Items.Add("�ᑬ")
		cboMessageSpeed.Items.Add("����")
		cboMessageSpeed.Items.Add("����")
		cboMessageSpeed.Items.Add("������")
		cboMessageSpeed.Items.Add("�_�̗̈�")
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
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If Not FileExists(AppPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Indeterminate '����
		End If
		
		'�g���퓬�A�j���\��
		If ExtendedAnimation Then
			chkExtendedAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkExtendedAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If chkBattleAnimation.CheckState = 1 Then
			chkExtendedAnimation.Enabled = True
		Else
			chkExtendedAnimation.Enabled = False
		End If
		
		'���폀���A�j���\��
		If WeaponAnimation Then
			chkWeaponAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkWeaponAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If chkBattleAnimation.CheckState = 1 Then
			chkWeaponAnimation.Enabled = True
		Else
			chkWeaponAnimation.Enabled = False
		End If
		
		'�ړ��A�j���\��
		If MoveAnimation Then
			chkMoveAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkMoveAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'�X�y�V�����p���[�A�j���\��
		If SpecialPowerAnimation Then
			chkSpecialPowerAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkSpecialPowerAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'�}�E�X�J�[�\���̎����ړ�
		If AutoMoveCursor Then
			chkAutoMoveCursor.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkAutoMoveCursor.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'�}�X�ڂ̕\��
		If ShowSquareLine Then
			chkShowSquareLine.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkShowSquareLine.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'�����t�F�C�Y�J�n���̃^�[���\��
		If LCase(ReadIni("Option", "Turn")) = "on" Then
			chkShowTurn.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkShowTurn.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'�G�t�F�C�Y���ɂa�f�l��ύX���Ȃ�
		If KeepEnemyBGM Then
			chkKeepEnemyBGM.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkKeepEnemyBGM.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'MIDI���t��DirectMusic���g�p����
		If LCase(ReadIni("Option", "UseDirectMusic")) = "on" Then
			chkUseDirectMusic.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkUseDirectMusic.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'MIDI�������Z�b�g�̎��
		cboMidiReset.Items.Add("None")
		cboMidiReset.Items.Add("GM")
		cboMidiReset.Items.Add("GS")
		cboMidiReset.Items.Add("XG")
		cboMidiReset.Text = MidiResetType
		
		'MP3����
		SavedMP3Volume = MP3Volume
		txtMP3Volume.Text = VB6.Format(MP3Volume)
	End Sub
	
	'MP3���ʕύX
	'UPGRADE_NOTE: hscMP3Volume.Change �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	'UPGRADE_WARNING: HScrollBar �C�x���g hscMP3Volume.Change �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub hscMP3Volume_Change(ByVal newScrollValue As Integer)
		Dim IsMP3Supported As Object
		MP3Volume = newScrollValue
		txtMP3Volume.Text = VB6.Format(MP3Volume)
		'UPGRADE_WARNING: �I�u�W�F�N�g IsMP3Supported �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'UPGRADE_NOTE: hscMP3Volume.Scroll �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	Private Sub hscMP3Volume_Scroll_Renamed(ByVal newScrollValue As Integer)
		Dim IsMP3Supported As Object
		MP3Volume = newScrollValue
		txtMP3Volume.Text = VB6.Format(MP3Volume)
		'UPGRADE_WARNING: �I�u�W�F�N�g IsMP3Supported �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'UPGRADE_WARNING: �C�x���g txtMP3Volume.TextChanged �́A�t�H�[�������������ꂽ�Ƃ��ɔ������܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' ���N���b�N���Ă��������B
	Private Sub txtMP3Volume_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtMP3Volume.TextChanged
		Dim IsMP3Supported As Object
		MP3Volume = StrToLng((txtMP3Volume.Text))
		
		If MP3Volume < 0 Then
			MP3Volume = 0
			txtMP3Volume.Text = "0"
		ElseIf MP3Volume > 100 Then 
			MP3Volume = 100
			txtMP3Volume.Text = "100"
		End If
		
		hscMP3Volume.Value = MP3Volume
		
		'UPGRADE_WARNING: �I�u�W�F�N�g IsMP3Supported �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	Private Sub hscMP3Volume_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles hscMP3Volume.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.ThumbTrack
				hscMP3Volume_Scroll_Renamed(eventArgs.newValue)
			Case System.Windows.Forms.ScrollEventType.EndScroll
				hscMP3Volume_Change(eventArgs.newValue)
		End Select
	End Sub
End Class