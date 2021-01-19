Option Strict Off
Option Explicit On
Module Sound
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�a�f�l�����ʉ��Đ��p�̃��W���[��
	
	'MCI����pAPI
	Declare Function mciSendString Lib "winmm.dll"  Alias "mciSendStringA"(ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer
	
	'WAVE�Đ��pAPI
	Declare Function sndPlaySound Lib "winmm.dll"  Alias "sndPlaySoundA"(ByVal lpszSoundName As String, ByVal uFlags As Integer) As Integer
	
	Public Const SND_SYNC As Integer = &H0 '�Đ��I����A�����߂�
	Public Const SND_ASYNC As Integer = &H1 '�֐����s��A�����߂�
	Public Const SND_NODEFAULT As Integer = &H2 '�w�肵��WAVE�t�@�C����������Ȃ������ꍇ�A
	'�f�t�H���g��WAVE�t�@�C�����Đ����Ȃ�
	Public Const SND_MEMORY As Integer = &H4 '�������t�@�C����WAVE�����s����
	Public Const SND_LOOP As Integer = &H8 '��~�𖽗߂���܂ōĐ����J��Ԃ��B
	Public Const SND_NOSTOP As Integer = &H10 '����Wave�t�@�C�����Đ����̏ꍇ�A�Đ��𒆎~����
	
	
	'���ݍĐ�����Ă���a�f�l�̃t�@�C����
	Public BGMFileName As String
	'�a�f�l�����s�[�g�Đ�����H
	Public RepeatMode As Boolean
	'�퓬���ɂ��a�f�l��ύX���Ȃ��H
	Public KeepBGM As Boolean
	'�{�X�p�a�f�l�����t��
	Public BossBGM As Boolean
	
	'Wave�t�@�C���̍Đ����s�����H
	Public IsWavePlayed As Boolean
	
	'MIDI�t�@�C���̃T�[�`�p�X�̏��������������Ă���H
	Private IsMidiSearchPathInitialized As Boolean
	
	'MIDI�Đ����@�̎�i
	Public UseMCI As Boolean
	Public UseDirectMusic As Boolean
	
	'WAV�Đ����@�̎�i
	Public UseDirectSound As Boolean
	
	'MP3�Đ����̉���
	Public MP3Volume As Short
	
	'DirectMusic�p�ϐ�
	'UPGRADE_ISSUE: DirectX7 �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DXObject As DirectX7
	'UPGRADE_ISSUE: DirectMusicLoader �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMLoader As DirectMusicLoader
	'UPGRADE_ISSUE: DirectMusicPerformance �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMPerformance As DirectMusicPerformance
	'UPGRADE_ISSUE: DirectMusicSegment �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMSegment As DirectMusicSegment
	
	'VBMP3.dll�̏��������������Ă���H
	Private IsMP3Supported As Boolean
	
	'DirectSound�p�ϐ�
	'UPGRADE_ISSUE: DirectSound �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DSObject As DirectSound
	'UPGRADE_ISSUE: DirectSoundBuffer �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DSBuffer As DirectSoundBuffer
	
	
	'�a�f�l�̍Đ����J�n����
	Public Sub StartBGM(ByRef bgm_name As String, Optional ByVal is_repeat_mode As Boolean = True)
		Dim fname0, fname, fname2 As String
		Dim i As Short
		
		'�a�f�l���Œ蒆�H
		If KeepBGM Then
			Exit Sub
		End If
		
		'�_�~�[�̃t�@�C�����H
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'�t�@�C�����̖{�̕����𔲂��o��
		fname0 = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname0, "\") > 0 Then
			fname0 = Mid(fname0, InStr2(fname0, "\") + 1)
		End If
		
		'�����a�f�l�����t���ł���Ή��t���p��
		If Len(BGMFileName) > 0 Then
			If InStr(BGMFileName, "\" & fname0 & ".") > 0 Then
				If BGMStatus() = "playing" Then
					Exit Sub
				End If
			End If
		End If
		
		'�t�@�C��������
		bgm_name = "(" & bgm_name & ")"
		fname = SearchMidiFile(bgm_name)
		
		'�t�@�C�������������H
		If Len(fname) = 0 Then
			Exit Sub
		End If
		
		'���t���X�g�b�v
		StopBGM()
		
		'�����a�f�l�Ƀo���G�[�V����������΃����_���őI��
		i = 1
		If InStr(fname, ScenarioPath) > 0 Then
			'�V�i���I���Ƀt�@�C�������������ꍇ�̓o���G�[�V�������V�i���I������̂ݑI��
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While InStr(fname2, ScenarioPath) > 0
		Else
			'�����łȂ���Η�������I��
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While fname2 <> ""
		End If
		
		i = Int(((i - 1) * Rnd()) + 1)
		If i > 1 Then
			fname = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
		End If
		
		'�a�f�l��A�����t�H
		RepeatMode = is_repeat_mode
		
		'�t�@�C�������[�h���A���t�J�n
		LoadBGM(fname)
		
		'���s�[�g�Đ��������s�����߂̃^�C�}�[���N��
		'UPGRADE_ISSUE: Control Timer1 �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		MainForm.Timer1.Enabled = True
	End Sub
	
	'�a�f�l�̃t�@�C����ǂݍ���
	Private Sub LoadBGM(ByRef fname As String)
		Dim ret As Integer
		Dim cmd As String
		Dim mp3_data As InputInfo
		
		'�t�@�C���̎�ނɉ������������s��
		Select Case LCase(Right(fname, 4))
			Case ".mid"
				'MIDI�t�@�C��
				
				'MIDI�����t����̂����߂āH
				If Not UseDirectMusic And Not UseMCI Then
					'DirectMusic�̏����������݂�
					InitDirectMusic()
				End If
				
				'�������Z�b�g
				ResetBGM()
				
				'DirectMusic���g���H
				If UseDirectMusic Then
					'�t�@�C�������[�h
					On Error GoTo ErrorHandler
					
					'UPGRADE_WARNING: �I�u�W�F�N�g DMLoader.LoadSegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					DMSegment = DMLoader.LoadSegment(fname)
					If Err.Number <> 0 Then
						ErrorMessage("LoadSegment failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetStandardMidiFile �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMSegment.SetStandardMidiFile()
					If Err.Number <> 0 Then
						ErrorMessage("SetStandardMidiFile failed (" & VB6.Format(Err.Number) & ")")
					End If
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetMasterAutoDownload �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMPerformance.SetMasterAutoDownload(True)
					If Err.Number <> 0 Then
						ErrorMessage("SetMasterAutoDownload failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'���[�v���t�̐ݒ�
					'�J��Ԃ��͈͂�ݒ�
					'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetLoopPoints �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMSegment.SetLoopPoints(0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("SetLoopPoints failed (" & VB6.Format(Err.Number) & ")")
					End If
					'�J��Ԃ��񐔂�ݒ�
					If RepeatMode Then
						'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetRepeats �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						Call DMSegment.SetRepeats(-1)
					Else
						'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetRepeats �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						Call DMSegment.SetRepeats(0)
					End If
					If Err.Number <> 0 Then
						ErrorMessage("SetRepeats failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'���t�J�n
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.PlaySegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMPerformance.PlaySegment(DMSegment, 0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("PlaySegment failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					BGMFileName = fname
					Exit Sub
				End If
				
				If GetWinVersion() >= 500 Then
					cmd = " type mpegvideo alias bgm wait"
				Else
					cmd = " type sequencer alias bgm wait"
				End If
				
			Case ".wav"
				'WAVE�t�@�C��
				cmd = " type waveaudio alias bgm wait"
				
			Case ".mp3"
				'MP3�t�@�C��
				
				'VBMP3.dll��������
				If Not IsMP3Supported Then
					InitVBMP3()
					
					If Not IsMP3Supported Then
						'VBMP3.dll�����p�s�\
						Exit Sub
					End If
				End If
				
				'���t���~
				Call vbmp3_stop()
				'�t�@�C�������
				Call vbmp3_close()
				
				'�t�@�C����ǂݍ���
				If vbmp3_open(fname, mp3_data) Then
					'                '�J��Ԃ��Đ����͎G��������Ȃ��悤�t�F�[�h�A�E�g������
					'                If RepeatMode Then
					'                    Call vbmp3_setFadeOut(1)
					'                Else
					'                    Call vbmp3_setFadeOut(0)
					'                End If
					
					'���t�J�n
					Call vbmp3_play()
					BGMFileName = fname
				End If
				Exit Sub
				
			Case Else
				'���T�|�[�g�̃t�@�C���`��
				Exit Sub
		End Select
		
		'�t�@�C�����J��
		cmd = "open " & Chr(34) & fname & Chr(34) & cmd
		ret = mciSendString(cmd, vbNullString, 0, 0)
		If ret <> 0 Then
			'�J���Ȃ�����
			Exit Sub
		End If
		
		'���t�J�n
		ret = mciSendString("play bgm", vbNullString, 0, 0)
		If ret <> 0 Then
			'���t�ł��Ȃ�����
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
			Exit Sub
		End If
		
		'���t���Ă���BGM�̃t�@�C�������L�^
		BGMFileName = fname
		Exit Sub
		
ErrorHandler: 
		If UseDirectMusic Then
			'DirectMusic���g�p�ł��Ȃ��ꍇ��MCI���g���ă��g���C
			UseDirectMusic = False
			UseMCI = True
			LoadBGM(fname)
		End If
	End Sub
	
	'�a�f�l�����X�^�[�g������
	Public Sub RestartBGM()
		Dim ret As Integer
		
		'��~���łȂ���Ή������Ȃ�
		If BGMStatus() <> "stopped" Then
			Exit Sub
		End If
		
		'���X�^�[�g
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid"
				'MIDI�t�@�C��
				If UseMCI Then
					ret = mciSendString("seek bgm to start wait", vbNullString, 0, 0)
					If ret <> 0 Then
						Exit Sub
					End If
					ret = mciSendString("play bgm", vbNullString, 0, 0)
				End If
			Case ".wav"
				'WAVE�t�@�C��
				ret = mciSendString("seek bgm to start wait", vbNullString, 0, 0)
				If ret <> 0 Then
					Exit Sub
				End If
				ret = mciSendString("play bgm", vbNullString, 0, 0)
			Case ".mp3"
				'MP3�t�@�C��
				If vbmp3_getState(ret) = 2 Then
					Call vbmp3_restart()
				Else
					Call vbmp3_play()
				End If
		End Select
	End Sub
	
	'�a�f�l���~����
	Public Sub StopBGM(Optional ByVal by_force As Boolean = False)
		Dim ret As Integer
		
		'�a�f�l���Œ蒆�H
		If Not by_force And KeepBGM Then
			Exit Sub
		End If
		
		'�����I�ɒ�~����̂łȂ���Ή��t���łȂ�����Ȃɂ����Ȃ�
		If Not by_force And Len(BGMFileName) = 0 Then
			Exit Sub
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDI�t�@�C��
				If UseDirectMusic Then
					'���t���~
					On Error Resume Next
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMPerformance.Stop(DMSegment, Nothing, 0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("DMPerformance.Stop failed (" & VB6.Format(Err.Number) & ")")
					End If
				Else
					'���t���~
					ret = mciSendString("stop bgm wait", vbNullString, 0, 0)
					'�t�@�C�������
					ret = mciSendString("close bgm wait", vbNullString, 0, 0)
				End If
			Case ".wav"
				'WAVE�t�@�C��
				'���t���~
				ret = mciSendString("stop bgm wait", vbNullString, 0, 0)
				'�t�@�C�������
				ret = mciSendString("close bgm wait", vbNullString, 0, 0)
			Case ".mp3"
				'MP3�t�@�C��
				'���t���~
				Call vbmp3_stop()
				'�t�@�C�������
				Call vbmp3_close()
		End Select
		
		BGMFileName = ""
		RepeatMode = False
		
		'���s�[�g�Đ��������s�����߂̃^�C�}�[���~
		'UPGRADE_ISSUE: Control Timer1 �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		MainForm.Timer1.Enabled = False
	End Sub
	
	'�l�h�c�h����������������(MCI���g�p����ꍇ�̂�)
	Private Sub ResetBGM()
		Dim ret As Integer
		Dim fname, cmd As String
		
		'�����̎�ނɉ����������������pMIDI�t�@�C����I��
		Select Case MidiResetType
			Case "GM"
				If UseDirectMusic Then
					'DirectMusic���g����GM���Z�b�g���\
					On Error GoTo ErrorHandler
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Reset �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMPerformance.Reset(0)
					Exit Sub
				End If
				fname = AppPath & "Midi\Reset(GM).mid"
			Case "GS"
				fname = AppPath & "Midi\Reset(GS).mid"
			Case "XG"
				fname = AppPath & "Midi\Reset(XG).mid"
			Case Else
				Exit Sub
		End Select
		
		'�t�@�C���������Ƃ���H
		If Not FileExists(fname) Then
			Exit Sub
		End If
		
		BGMFileName = ""
		
		If UseDirectMusic Then
			'DirectMusic���g���ꍇ
			On Error GoTo ErrorHandler
			
			'�t�@�C�������[�h
			'UPGRADE_WARNING: �I�u�W�F�N�g DMLoader.LoadSegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DMSegment = DMLoader.LoadSegment(fname)
			
			'MIDI�Đ��̂��ߊe��p�����[�^��ݒ�
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetStandardMidiFile �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetStandardMidiFile()
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetMasterAutoDownload �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.SetMasterAutoDownload(True)
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetLoopPoints �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetLoopPoints(0, 0)
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetRepeats �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetRepeats(0)
			
			'�������Z�b�g�pMIDI�t�@�C���̉��t�J�n
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.PlaySegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.PlaySegment(DMSegment, 0, 0)
			
			'���t���I���܂ő҂�
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.IsPlaying �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Do While DMPerformance.IsPlaying(DMSegment, Nothing)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'���t���~
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.Stop(DMSegment, Nothing, 0, 0)
		Else
			'MCI���g���ꍇ
			
			'�t�@�C�����I�[�v��
			cmd = "open " & Chr(34) & fname & Chr(34) & " type sequencer alias bgm wait"
			ret = mciSendString(cmd, vbNullString, 0, 0)
			If ret <> 0 Then
				Exit Sub
			End If
			
			'�������Z�b�g�pMIDI�t�@�C�������t
			ret = mciSendString("play bgm wait", vbNullString, 0, 0)
			
			'�t�@�C�����N���[�Y
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
		End If
		
		Exit Sub
		
ErrorHandler: 
		'DirectMusic�g�p���ɃG���[�����������̂�MCI���g��
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'�a�f�l���Đ����H
	Private Function BGMStatus() As String
		Dim retstr As String
		Dim ret, sec As Integer
		
		'�a�f�l�����t���łȂ���΋󕶎����Ԃ�
		If Len(BGMFileName) = 0 Then
			Exit Function
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDI�t�@�C��
				If UseDirectMusic Then
					'DirectMusic���g���ꍇ
					On Error Resume Next
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.IsPlaying �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If DMPerformance.IsPlaying(DMSegment, Nothing) Then
						BGMStatus = "playing"
					Else
						BGMStatus = "stopped"
					End If
					If Err.Number <> 0 Then
						ErrorMessage("DMPerformance.IsPlaying failed (" & VB6.Format(Err.Number) & ")")
					End If
				Else
					'MCI���g���ꍇ
					
					'���ʂ�ۑ�����̈���m��
					retstr = Space(120)
					
					'�Đ��󋵂��Q��
					ret = mciSendString("status bgm mode", retstr, 120, 0)
					If ret <> 0 Then
						Exit Function
					End If
					
					'API�̌��ʂ�NULL�^�[�~�l�C�g
					ret = InStr(retstr, Chr(0))
					BGMStatus = Left(retstr, ret - 1)
				End If
				
			Case ".wav"
				'WAVE�t�@�C��
				
				'���ʂ�ۑ�����̈���m��
				retstr = Space(120)
				
				ret = mciSendString("status bgm mode", retstr, 120, 0)
				If ret <> 0 Then
					Exit Function
				End If
				
				'API�̌��ʂ�NULL�^�[�~�l�C�g
				ret = InStr(retstr, Chr(0))
				BGMStatus = Left(retstr, ret - 1)
				
			Case ".mp3"
				'MP3�̍Đ���ԂƍĐ����Ԃ̎擾
				ret = vbmp3_getState(sec)
				
				Select Case ret
					Case 0
						'��~��
						BGMStatus = "stopped"
					Case 1
						'�Đ���
						BGMStatus = "playing"
					Case 2
						'�ꎞ��~��
						BGMStatus = "stopped"
				End Select
		End Select
	End Function
	
	'�a�f�l��ύX���� (�w�肵���a�f�l�����łɉ��t���Ȃ�Ȃɂ����Ȃ�)
	Public Sub ChangeBGM(ByRef bgm_name As String)
		Dim fname, fname2 As String
		
		'�a�f�l�Œ蒆�H
		If KeepBGM Or BossBGM Then
			Exit Sub
		End If
		
		'�������t�@�C�����H
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'�t�@�C�����̖{�̕����𔲂��o��
		fname = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname, "\") > 0 Then
			fname = Mid(fname, InStr2(fname, "\") + 1)
		End If
		
		'���ɓ���MIDI�����t����Ă���΂��̂܂܉��t��������
		If Len(BGMFileName) > 0 Then
			If InStr(BGMFileName, "\" & fname & ".") > 0 Then
				Exit Sub
			End If
		End If
		
		'�ԍ��Ⴂ�H
		If Len(BGMFileName) > 5 Then
			fname2 = Left(BGMFileName, Len(BGMFileName) - 4)
			If InStr2(fname2, "\") > 0 Then
				fname2 = Mid(fname2, InStr2(fname2, "\") + 1)
			End If
			If Len(fname2) > 4 Then
				Select Case Right(fname2, 3)
					Case "(2)", "(3)", "(4)", "(5)", "(6)", "(7)", "(8)", "(9)"
						fname2 = Left(fname2, Len(fname2) - 3)
				End Select
			End If
			If fname = fname2 Then
				Exit Sub
			End If
		End If
		
		'�J��Ԃ����t�ɐݒ�
		RepeatMode = True
		
		'���t�J�n
		StartBGM(bgm_name)
	End Sub
	
	
	'DirectMusic�̏�����
	Public Sub InitDirectMusic()
		Dim DMUS_PC_GMINHARDWARE As Object
		Dim DMUS_PC_GSINHARDWARE As Object
		Dim DMUS_PC_XGINHARDWARE As Object
		Dim DMUS_PC_EXTERNAL As Object
		Dim port_id As Short
		'UPGRADE_ISSUE: DMUS_PORTCAPS �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim portcaps As DMUS_PORTCAPS
		Dim i As Short
		
		On Error GoTo ErrorHandler
		
		'�t���O��ݒ�
		UseDirectMusic = True
		UseMCI = False
		
		'DirectX�I�u�W�F�N�g�쐬
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'Loader�쐬
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectMusicLoaderCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DMLoader = DXObject.DirectMusicLoaderCreate
		
		'�T�[�`�p�X�ݒ�(�s�v�H)
		'UPGRADE_WARNING: �I�u�W�F�N�g DMLoader.SetSearchDirectory �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMLoader.SetSearchDirectory(AppPath & "Midi")
		
		'Performance�쐬
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectMusicPerformanceCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DMPerformance = DXObject.DirectMusicPerformanceCreate
		
		'Performance������
		'DirectSound�ƕ��p���鎞�́A�ŏ��̈�����
		'DirectSound�̃I�u�W�F�N�g�����Ă���
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Init �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMPerformance.Init(DSObject, MainForm.Handle.ToInt32)
		
		'MIDI�����ꗗ���쐬
		CreateMIDIPortListFile()
		
		'�|�[�g�ݒ�
		port_id = StrToLng(ReadIni("Option", "MIDIPortID"))
		
		'�g�p�|�[�g�ԍ����w�肳��Ă����ꍇ
		If port_id > 0 Then
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If port_id > DMPerformance.GetPortCount Then
				ErrorMessage("MIDIPortID�ɐ�����MIDI�|�[�g���ݒ肳��Ă��܂���B")
				End
			End If
			
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.SetPort(port_id, 1)
			Exit Sub
		End If
		
		'�w�肪�Ȃ��̂�SRC���Ō�������
		
		'MIDI�}�b�p�[������΂�����g��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "MIDI �}�b�p�[") > 0 Or InStr(DMPerformance.GetPortName(i), "MIDI Mapper") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'�܂��͊O��MIDI������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCaps �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: �I�u�W�F�N�g portcaps.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If portcaps.lFlags And DMUS_PC_EXTERNAL Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'����XG�Ή��n�[�h������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCaps �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: �I�u�W�F�N�g portcaps.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If portcaps.lFlags And DMUS_PC_XGINHARDWARE Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'����GS�Ή��n�[�h������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCaps �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: �I�u�W�F�N�g portcaps.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If portcaps.lFlags And DMUS_PC_GSINHARDWARE Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'����XG�Ή��\�t�g������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "XG ") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'����GS�Ή��\�t�g������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "GS ") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'����GM�Ή��n�[�h������{��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCaps �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: �I�u�W�F�N�g portcaps.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If portcaps.lFlags And DMUS_PC_GMINHARDWARE Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'������߂ăf�t�H���g�|�[�g���g��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMPerformance.SetPort(-1, 1)
		
		Exit Sub
		
ErrorHandler: 
		
		'DirectMusic���������ɃG���[�����������̂�MCI���g��
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'DirectX�I�u�W�F�N�g���쐬����
	Private Function CreateDirectXObject() As DirectX7
		'UPGRADE_ISSUE: DirectX7 �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim new_obj As New DirectX7
		
		CreateDirectXObject = new_obj
	End Function
	
	'���p�\��MIDI�����̈ꗗ���쐬����
	Private Sub CreateMIDIPortListFile()
		Dim f, i As Short
		Dim pname As String
		
		On Error GoTo ErrorHandler
		
		f = FreeFile
		FileOpen(f, AppPath & "Midi\MIDI�������X�g.txt", OpenMode.Output, OpenAccess.Write)
		
		PrintLine(f, ";DirectMusic�ŗ��p�\��MIDI�����̃��X�g�ł��B")
		PrintLine(f, ";Src.ini��MIDIPortID�Ɏg�p�����������̔ԍ����w�肵�ĉ������B")
		PrintLine(f, "")
		
		'�e�|�[�g�̖��̂��Q��
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			pname = DMPerformance.GetPortName(i)
			If InStr(pname, "[") > 0 Then
				pname = Left(pname, InStr(pname, "[") - 1)
			End If
			PrintLine(f, VB6.Format(i) & ":" & pname)
		Next 
		
		FileClose(f)
		
ErrorHandler: 
		'�G���[����
	End Sub
	
	
	'VBMP3��������
	Private Sub InitVBMP3()
		Dim opt As VBMP3_OPTION
		Dim buf As String
		
		If Not FileExists(AppPath & "VBMP3.dll") Then
			Exit Sub
		End If
		
		Call vbmp3_init()
		
		Call vbmp3_setVolume(MP3Volume, MP3Volume)
		
		opt.inputBlock = 30
		buf = ReadIni("Option", "MP3OutputBlock")
		opt.outputBlock = MinLng(StrToLng(buf), 30)
		
		buf = ReadIni("Option", "MP3InputSleep")
		opt.inputSleep = MaxLng(StrToLng(buf), 0)
		opt.outputSleep = 0
		
		Call vbmp3_setVbmp3Option(opt)
		
		IsMP3Supported = True
	End Sub
	
	
	'�eMidi�t�H���_����w�肳�ꂽMIDI�t�@�C������������
	Public Function SearchMidiFile(ByRef midi_name As String) As String
		Dim fname, fname_mp3 As String
		Static scenario_midi_dir_exists As Boolean
		Static extdata_midi_dir_exists As Boolean
		Static extdata2_midi_dir_exists As Boolean
		Static is_mp3_available As Boolean
		'Static fpath_history As New Collection     DEL MARGE
		Dim j, i, num As Short
		Dim buf, buf2 As String
		Dim sub_folder As String
		
		'���߂Ď��s����ۂɁA�e�t�H���_��Midi�t�H���_�����邩�`�F�b�N
		If Not IsMidiSearchPathInitialized Then
			If Len(ScenarioPath) > 0 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ScenarioPath & "Midi", FileAttribute.Directory)) > 0 Then
					scenario_midi_dir_exists = True
				End If
			End If
			If Len(ExtDataPath) > 0 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath & "Midi", FileAttribute.Directory)) > 0 Then
					extdata_midi_dir_exists = True
				End If
			End If
			If Len(ExtDataPath2) > 0 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath2 & "Midi", FileAttribute.Directory)) > 0 Then
					extdata2_midi_dir_exists = True
				End If
			End If
			
			'MP3�����t�\���ǂ��������ׂĂ���
			If FileExists(AppPath & "VBMP3.dll") Then
				is_mp3_available = True
			End If
			
			IsMidiSearchPathInitialized = True
		End If
		
		'�_�~�[�̃t�@�C�����H
		If Len(midi_name) < 5 Then
			Exit Function
		End If
		
		'����1�Ƃ��ēn���ꂽ����������X�g�Ƃ��Ĉ����A�����珇��MIDI������
		num = ListLength(midi_name)
		i = 1
		Do While i <= num
			'�X�y�[�X���܂ރt�@�C�����ւ̑Ή�
			buf = ""
			For j = i To num
				buf2 = LCase(ListIndex(midi_name, j))
				
				'�S�̂�()�ň͂܂�Ă���ꍇ��()���O��
				If Left(buf2, 1) = "(" And Right(buf2, 1) = ")" Then
					buf2 = Mid(buf2, 2, Len(buf2) - 2)
				End If
				
				buf = buf & " " & buf2
				
				If Right(buf, 4) = ".mid" Then
					Exit For
				End If
			Next 
			buf = Trim(buf)
			
			'������MP3�t�@�C��������ꍇ��MIDI�t�@�C���̑����MP3�t�@�C�����g��
			If is_mp3_available Then
				fname_mp3 = Left(buf, Len(buf) - 4) & ".mp3"
			End If
			
			'�t���p�X�ł̎w��H
			If InStr(buf, ":") = 2 Then
				If is_mp3_available Then
					If FileExists(fname_mp3) Then
						SearchMidiFile = fname_mp3
						Exit Function
					End If
				End If
				If FileExists(buf) Then
					SearchMidiFile = buf
				End If
				Exit Function
			End If
			
			' DEL START MARGE
			'        '�������������Ă݂�
			'        On Error GoTo NotFound
			'        fname = fpath_history.Item(buf)
			'
			'        '������Ƀt�@�C���𔭌�
			'        SearchMidiFile = fname
			'        Exit Function
			
			'NotFound:
			'        '�����ɂȂ�����
			'        On Error GoTo 0
			' DEL END MARGE
			
			'�T�u�t�H���_�w�肠��H
			If InStr(buf, "_") > 0 Then
				sub_folder = Left(buf, InStr(buf, "_") - 1) & "\"
			End If
			
			'�V�i���I����Midi�t�H���_
			If scenario_midi_dir_exists Then
				If is_mp3_available Then
					If sub_folder <> "" Then
						fname = ScenarioPath & "Midi\" & sub_folder & fname_mp3
						If FileExists(fname) Then
							SearchMidiFile = fname
							'                        fpath_history.Add fname, buf   DEL MARGE
							Exit Function
						End If
					End If
					
					fname = ScenarioPath & "Midi\" & fname_mp3
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				If sub_folder <> "" Then
					fname = ScenarioPath & "Midi\" & sub_folder & buf
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				fname = ScenarioPath & "Midi\" & buf
				If FileExists(fname) Then
					SearchMidiFile = fname
					'                fpath_history.Add fname, buf   DEL MARGE
					Exit Function
				End If
			End If
			
			'ExtDataPath����Midi�t�H���_
			If extdata_midi_dir_exists Then
				If is_mp3_available Then
					If sub_folder <> "" Then
						fname = ExtDataPath & "Midi\" & sub_folder & fname_mp3
						If FileExists(fname) Then
							SearchMidiFile = fname
							'                        fpath_history.Add fname, buf   DEL MARGE
							Exit Function
						End If
					End If
					
					fname = ExtDataPath & "Midi\" & fname_mp3
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				If sub_folder <> "" Then
					fname = ExtDataPath & "Midi\" & sub_folder & buf
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				fname = ExtDataPath & "Midi\" & buf
				If FileExists(fname) Then
					SearchMidiFile = fname
					'                fpath_history.Add fname, buf   DEL MARGE
					Exit Function
				End If
			End If
			
			'ExtDataPath2����Midi�t�H���_
			If extdata2_midi_dir_exists Then
				If is_mp3_available Then
					If sub_folder <> "" Then
						fname = ExtDataPath2 & "Midi\" & sub_folder & fname_mp3
						If FileExists(fname) Then
							SearchMidiFile = fname
							'                        fpath_history.Add fname, buf   DEL MARGE
							Exit Function
						End If
					End If
					
					fname = ExtDataPath2 & "Midi\" & fname_mp3
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				If sub_folder <> "" Then
					fname = ExtDataPath2 & "Midi\" & sub_folder & buf
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				fname = ExtDataPath2 & "Midi\" & buf
				If FileExists(fname) Then
					SearchMidiFile = fname
					'                fpath_history.Add fname, buf   DEL MARGE
					Exit Function
				End If
			End If
			
			'�{�̑���Midi�t�H���_
			If is_mp3_available Then
				If sub_folder <> "" Then
					fname = AppPath & "Midi\" & sub_folder & fname_mp3
					If FileExists(fname) Then
						SearchMidiFile = fname
						'                    fpath_history.Add fname, buf   DEL MARGE
						Exit Function
					End If
				End If
				
				fname = AppPath & "Midi\" & fname_mp3
				If FileExists(fname) Then
					SearchMidiFile = fname
					'                fpath_history.Add fname, buf   DEL MARGE
					Exit Function
				End If
			End If
			
			If sub_folder <> "" Then
				fname = AppPath & "Midi\" & sub_folder & buf
				If FileExists(fname) Then
					SearchMidiFile = fname
					'                fpath_history.Add fname, buf   DEL MARGE
					Exit Function
				End If
			End If
			
			fname = AppPath & "Midi\" & buf
			If FileExists(fname) Then
				SearchMidiFile = fname
				'            fpath_history.Add fname, buf   DEL MARGE
				Exit Function
			End If
			
			i = j + 1
		Loop 
	End Function
	
	'MIDI�t�@�C���̃T�[�`�p�X�����Z�b�g����
	Public Sub ResetMidiSearchPath()
		IsMidiSearchPathInitialized = False
	End Sub
	
	
	'�a�f�l�Ɋ��蓖�Ă�ꂽMIDI�t�@�C������Ԃ�
	Public Function BGMName(ByRef bgm_name As String) As String
		Dim vname As String
		
		'RenameBGM�R�}���h��MIDI�t�@�C�����ݒ肳��Ă���΂�������g�p
		vname = "BGM(" & bgm_name & ")"
		If IsGlobalVariableDefined(vname) Then
			'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			BGMName = GlobalVariableList.Item(vname).StringValue
			Exit Function
		End If
		
		'�����łȂ����Src.ini�Őݒ肳��Ă���t�@�C�����g�p
		BGMName = ReadIni("BGM", bgm_name)
		
		'Src.ini�ł��ݒ肳��Ă��Ȃ���ΕW���̃t�@�C�����g�p
		If BGMName = "" Then
			BGMName = bgm_name & ".mid"
		End If
	End Function
	
	
	'DirectSound�̏�����
	Public Sub InitDirectSound()
		Dim DSSCL_PRIORITY As Object
		'On Error GoTo ErrorHandler
		Exit Sub
		
		'�t���O��ݒ�
		UseDirectSound = True
		
		'DirectX�I�u�W�F�N�g�쐬
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'DirectSound�I�u�W�F�N�g�쐬
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectSoundCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DSObject = DXObject.DirectSoundCreate("")
		
		'�T�E���h�f�o�C�X�̋������x����ݒ�
		'UPGRADE_WARNING: �I�u�W�F�N�g DSObject.SetCooperativeLevel �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DSObject.SetCooperativeLevel(MainForm.Handle.ToInt32, DSSCL_PRIORITY)
		
		Exit Sub
		
ErrorHandler: 
		
		UseDirectSound = False
	End Sub
	
	'Wave�t�@�C�����Đ�����
	Public Sub PlayWave(ByRef wave_name As String)
		Dim DSBPLAY_DEFAULT As Object
		Dim DSBCAPS_STATIC As Object
		Dim DSBCAPS_CTRLVOLUME As Object
		Dim DSBCAPS_CTRLPAN As Object
		Dim DSBCAPS_CTRLFREQUENCY As Object
		Dim ret As Integer
		Dim fname As String
		Dim mp3_data As InputInfo
		Static init_play_wave As Boolean
		Static scenario_sound_dir_exists As Boolean
		Static extdata_sound_dir_exists As Boolean
		Static extdata2_sound_dir_exists As Boolean
		
		'���߂Ď��s����ۂɁA�e�t�H���_��Sound�t�H���_�����邩�`�F�b�N
		If Not init_play_wave Then
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ScenarioPath & "Sound", FileAttribute.Directory)) > 0 Then
				scenario_sound_dir_exists = True
			End If
			If Len(ExtDataPath) > 0 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath & "Sound", FileAttribute.Directory)) > 0 Then
					extdata_sound_dir_exists = True
				End If
			End If
			If Len(ExtDataPath2) > 0 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath2 & "Sound", FileAttribute.Directory)) > 0 Then
					extdata2_sound_dir_exists = True
				End If
			End If
			init_play_wave = True
		End If
		
		'����ȃt�@�C����
		Select Case LCase(wave_name)
			Case "-.wav", "-.mp3"
				'�Đ����L�����Z��
				Exit Sub
			Case "null.wav"
				'WAVE�Đ����~
				StopWave()
				Exit Sub
			Case "null.mp3"
				'MP3�Đ����~
				If LCase(Right(BGMFileName, 4)) = ".mp3" Then
					StopBGM(True)
				Else
					'���t���~
					Call vbmp3_stop()
					'�t�@�C�������
					Call vbmp3_close()
				End If
				Exit Sub
		End Select
		
		'�e�t�H���_���`�F�b�N
		
		'�V�i���I����Sound�t�H���_
		If scenario_sound_dir_exists Then
			fname = ScenarioPath & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'ExtDataPath����Sound�t�H���_
		If extdata_sound_dir_exists Then
			fname = ExtDataPath & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'ExtDataPath2����Sound�t�H���_
		If extdata2_sound_dir_exists Then
			fname = ExtDataPath2 & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'�{�̑���Sound�t�H���_
		fname = AppPath & "Sound\" & wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'��Ε\�L�H
		fname = wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'������Ȃ�����
		Exit Sub
		
FoundWave: 
		
		'UPGRADE_ISSUE: WAVEFORMATEX �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim wf As WAVEFORMATEX
		'UPGRADE_ISSUE: DSBUFFERDESC �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim dsbd As DSBUFFERDESC
		If LCase(Right(fname, 4)) = ".mp3" Then
			'���ʉ���MP3�t�@�C��
			
			'VBMP3.dll��������
			If Not IsMP3Supported Then
				InitVBMP3()
				
				If Not IsMP3Supported Then
					'VBMP3.dll�����p�s�\
					Exit Sub
				End If
			End If
			
			'MP3�Đ����~
			If LCase(Right(BGMFileName, 4)) = ".mp3" Then
				StopBGM(True)
			Else
				'���t���~
				Call vbmp3_stop()
				'�t�@�C�������
				Call vbmp3_close()
			End If
			
			'�t�@�C����ǂݍ���
			If vbmp3_open(fname, mp3_data) Then
				'�Đ��J�n
				Call vbmp3_play()
			End If
		ElseIf UseDirectSound Then 
			'DirectSound���g���ꍇ
			
			
			'�Đ����̏ꍇ�͍Đ����X�g�b�v
			If Not DSBuffer Is Nothing Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DSBuffer.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				DSBuffer.Stop()
				'UPGRADE_NOTE: �I�u�W�F�N�g DSBuffer ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				DSBuffer = Nothing
			End If
			
			'�T�E���h�o�b�t�@��WAV�t�@�C����ǂݍ���
			'UPGRADE_WARNING: �I�u�W�F�N�g dsbd.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			dsbd.lFlags = DSBCAPS_CTRLFREQUENCY Or DSBCAPS_CTRLPAN Or DSBCAPS_CTRLVOLUME Or DSBCAPS_STATIC
			'UPGRADE_WARNING: �I�u�W�F�N�g DSObject.CreateSoundBufferFromFile �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DSBuffer = DSObject.CreateSoundBufferFromFile(fname, dsbd, wf)
			
			'WAVE���Đ�
			'UPGRADE_WARNING: �I�u�W�F�N�g DSBuffer.Play �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DSBuffer.Play(DSBPLAY_DEFAULT)
		Else
			'API���g���ꍇ
			
			'WAVE���Đ�
			ret = sndPlaySound(fname, SND_ASYNC + SND_NODEFAULT)
		End If
		
		'���ʉ��Đ��̃t���O�𗧂Ă�
		IsWavePlayed = True
	End Sub
	
	'Wave�t�@�C���̍Đ����I������
	Public Sub StopWave()
		Dim ret As Integer
		
		If UseDirectSound Then
			If Not DSBuffer Is Nothing Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DSBuffer.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				DSBuffer.Stop()
			End If
		Else
			ret = sndPlaySound(vbNullString, 0)
		End If
	End Sub
	
	
	'�{���W���[���̉���������s��
	Public Sub FreeSoundModule()
		'BGM���t�̒�~
		KeepBGM = False
		BossBGM = False
		StopBGM(True)
		
		'����������
		ResetBGM()
		
		'WAVE�t�@�C���Đ��̒�~
		StopWave()
		
		'DirectMusic�̉��
		If UseDirectMusic Then
			'���t��~
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.CloseDown �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DMPerformance.CloseDown()
			
			'�I�u�W�F�N�g�̉��
			'UPGRADE_NOTE: �I�u�W�F�N�g DMLoader ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMLoader = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DMPerformance ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMPerformance = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DMSegment ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMSegment = Nothing
		End If
		
		'DirectSound�̉��
		If UseDirectSound Then
			'�I�u�W�F�N�g�̉��
			'UPGRADE_NOTE: �I�u�W�F�N�g DSObject ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DSObject = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DSBuffer ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DSBuffer = Nothing
		End If
		
		'DirectX�̉��
		'UPGRADE_NOTE: �I�u�W�F�N�g DXObject ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		DXObject = Nothing
		
		'VBMP3.DLL�̉��
		If IsMP3Supported Then
			Call vbmp3_stop()
			Call vbmp3_free()
		End If
	End Sub
End Module