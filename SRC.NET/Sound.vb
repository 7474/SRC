Option Strict Off
Option Explicit On
Module Sound
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'MCI制御用API
	Declare Function mciSendString Lib "winmm.dll"  Alias "mciSendStringA"(ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer
	
	'WAVE再生用API
	Declare Function sndPlaySound Lib "winmm.dll"  Alias "sndPlaySoundA"(ByVal lpszSoundName As String, ByVal uFlags As Integer) As Integer
	
	Public Const SND_SYNC As Integer = &H0 'Invalid_string_refer_to_original_code
	Public Const SND_ASYNC As Integer = &H1 'Invalid_string_refer_to_original_code
	Public Const SND_NODEFAULT As Integer = &H2 'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Const SND_MEMORY As Integer = &H4 'Invalid_string_refer_to_original_code
	Public Const SND_LOOP As Integer = &H8 'Invalid_string_refer_to_original_code
	Public Const SND_NOSTOP As Integer = &H10 'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Public BGMFileName As String
	'Invalid_string_refer_to_original_code
	Public RepeatMode As Boolean
	'Invalid_string_refer_to_original_code
	Public KeepBGM As Boolean
	'Invalid_string_refer_to_original_code
	Public BossBGM As Boolean
	
	'Invalid_string_refer_to_original_code
	Public IsWavePlayed As Boolean
	
	'Invalid_string_refer_to_original_code
	Private IsMidiSearchPathInitialized As Boolean
	
	'Invalid_string_refer_to_original_code
	Public UseMCI As Boolean
	Public UseDirectMusic As Boolean
	
	'Invalid_string_refer_to_original_code
	Public UseDirectSound As Boolean
	
	'Invalid_string_refer_to_original_code
	Public MP3Volume As Short
	
	'DirectMusic用変数
	'UPGRADE_ISSUE: DirectX7 �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DXObject As DirectX7
	'UPGRADE_ISSUE: DirectMusicLoader �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMLoader As DirectMusicLoader
	'UPGRADE_ISSUE: DirectMusicPerformance �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMPerformance As DirectMusicPerformance
	'UPGRADE_ISSUE: DirectMusicSegment �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DMSegment As DirectMusicSegment
	
	'Invalid_string_refer_to_original_code
	Private IsMP3Supported As Boolean
	
	'DirectSound用変数
	'UPGRADE_ISSUE: DirectSound �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DSObject As DirectSound
	'UPGRADE_ISSUE: DirectSoundBuffer �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
	Private DSBuffer As DirectSoundBuffer
	
	
	'Invalid_string_refer_to_original_code
	Public Sub StartBGM(ByRef bgm_name As String, Optional ByVal is_repeat_mode As Boolean = True)
		Dim fname0, fname, fname2 As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If KeepBGM Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		fname0 = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname0, "\") > 0 Then
			fname0 = Mid(fname0, InStr2(fname0, "\") + 1)
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(BGMFileName) > 0 Then
			If InStr(BGMFileName, "\" & fname0 & ".") > 0 Then
				If BGMStatus() = "playing" Then
					Exit Sub
				End If
			End If
		End If
		
		'ファイルを検索
		bgm_name = "(" & bgm_name & ")"
		fname = SearchMidiFile(bgm_name)
		
		'Invalid_string_refer_to_original_code
		If Len(fname) = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		StopBGM()
		
		'Invalid_string_refer_to_original_code
		i = 1
		If InStr(fname, ScenarioPath) > 0 Then
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While InStr(fname2, ScenarioPath) > 0
		Else
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While fname2 <> ""
		End If
		
		i = Int(((i - 1) * Rnd()) + 1)
		If i > 1 Then
			fname = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
		End If
		
		'Invalid_string_refer_to_original_code
		RepeatMode = is_repeat_mode
		
		'Invalid_string_refer_to_original_code
		LoadBGM(fname)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Control Timer1 �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		MainForm.Timer1.Enabled = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub LoadBGM(ByRef fname As String)
		Dim ret As Integer
		Dim cmd As String
		Dim mp3_data As InputInfo
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(Right(fname, 4))
			Case ".mid"
				'MIDIファイル
				
				'Invalid_string_refer_to_original_code
				If Not UseDirectMusic And Not UseMCI Then
					'Invalid_string_refer_to_original_code
					InitDirectMusic()
				End If
				
				'Invalid_string_refer_to_original_code
				ResetBGM()
				
				'Invalid_string_refer_to_original_code
				If UseDirectMusic Then
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetLoopPoints �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMSegment.SetLoopPoints(0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("SetLoopPoints failed (" & VB6.Format(Err.Number) & ")")
					End If
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
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
				'WAVEファイル
				cmd = " type waveaudio alias bgm wait"
				
			Case ".mp3"
				'MP3ファイル
				
				'Invalid_string_refer_to_original_code
				If Not IsMP3Supported Then
					InitVBMP3()
					
					If Not IsMP3Supported Then
						'Invalid_string_refer_to_original_code
						Exit Sub
					End If
				End If
				
				'演奏を停止
				Call vbmp3_stop()
				'ファイルを閉じる
				Call vbmp3_close()
				
				'ファイルを読み込む
				If vbmp3_open(fname, mp3_data) Then
					'Invalid_string_refer_to_original_code
					'                If RepeatMode Then
					'                    Call vbmp3_setFadeOut(1)
					'                Else
					'                    Call vbmp3_setFadeOut(0)
					'                End If
					
					'Invalid_string_refer_to_original_code
					Call vbmp3_play()
					BGMFileName = fname
				End If
				Exit Sub
				
			Case Else
				'Invalid_string_refer_to_original_code
				Exit Sub
		End Select
		
		'Invalid_string_refer_to_original_code
		cmd = "open " & Chr(34) & fname & Chr(34) & cmd
		ret = mciSendString(cmd, vbNullString, 0, 0)
		If ret <> 0 Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		ret = mciSendString("play bgm", vbNullString, 0, 0)
		If ret <> 0 Then
			'Invalid_string_refer_to_original_code
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		BGMFileName = fname
		Exit Sub
		
ErrorHandler: 
		If UseDirectMusic Then
			'Invalid_string_refer_to_original_code
			UseDirectMusic = False
			UseMCI = True
			LoadBGM(fname)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestartBGM()
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If BGMStatus() <> "stopped" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid"
				'MIDIファイル
				If UseMCI Then
					ret = mciSendString("seek bgm to start wait", vbNullString, 0, 0)
					If ret <> 0 Then
						Exit Sub
					End If
					ret = mciSendString("play bgm", vbNullString, 0, 0)
				End If
			Case ".wav"
				'WAVEファイル
				ret = mciSendString("seek bgm to start wait", vbNullString, 0, 0)
				If ret <> 0 Then
					Exit Sub
				End If
				ret = mciSendString("play bgm", vbNullString, 0, 0)
			Case ".mp3"
				'MP3ファイル
				If vbmp3_getState(ret) = 2 Then
					Call vbmp3_restart()
				Else
					Call vbmp3_play()
				End If
		End Select
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub StopBGM(Optional ByVal by_force As Boolean = False)
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If Not by_force And KeepBGM Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Not by_force And Len(BGMFileName) = 0 Then
			Exit Sub
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDIファイル
				If UseDirectMusic Then
					'演奏を停止
					On Error Resume Next
					'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					Call DMPerformance.Stop(DMSegment, Nothing, 0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("DMPerformance.Stop failed (" & VB6.Format(Err.Number) & ")")
					End If
				Else
					'演奏を停止
					ret = mciSendString("stop bgm wait", vbNullString, 0, 0)
					'ファイルを閉じる
					ret = mciSendString("close bgm wait", vbNullString, 0, 0)
				End If
			Case ".wav"
				'WAVEファイル
				'演奏を停止
				ret = mciSendString("stop bgm wait", vbNullString, 0, 0)
				'ファイルを閉じる
				ret = mciSendString("close bgm wait", vbNullString, 0, 0)
			Case ".mp3"
				'MP3ファイル
				'演奏を停止
				Call vbmp3_stop()
				'ファイルを閉じる
				Call vbmp3_close()
		End Select
		
		BGMFileName = ""
		RepeatMode = False
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Control Timer1 �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		MainForm.Timer1.Enabled = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub ResetBGM()
		Dim ret As Integer
		Dim fname, cmd As String
		
		'Invalid_string_refer_to_original_code
		Select Case MidiResetType
			Case "GM"
				If UseDirectMusic Then
					'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If Not FileExists(fname) Then
			Exit Sub
		End If
		
		BGMFileName = ""
		
		If UseDirectMusic Then
			'Invalid_string_refer_to_original_code
			On Error GoTo ErrorHandler
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g DMLoader.LoadSegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DMSegment = DMLoader.LoadSegment(fname)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetStandardMidiFile �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetStandardMidiFile()
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetMasterAutoDownload �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.SetMasterAutoDownload(True)
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetLoopPoints �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetLoopPoints(0, 0)
			'UPGRADE_WARNING: �I�u�W�F�N�g DMSegment.SetRepeats �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMSegment.SetRepeats(0)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.PlaySegment �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.PlaySegment(DMSegment, 0, 0)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.IsPlaying �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Do While DMPerformance.IsPlaying(DMSegment, Nothing)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'演奏を停止
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.Stop(DMSegment, Nothing, 0, 0)
		Else
			'Invalid_string_refer_to_original_code
			
			'ファイルをオープン
			cmd = "open " & Chr(34) & fname & Chr(34) & " type sequencer alias bgm wait"
			ret = mciSendString(cmd, vbNullString, 0, 0)
			If ret <> 0 Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			ret = mciSendString("play bgm wait", vbNullString, 0, 0)
			
			'ファイルをクローズ
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
		End If
		
		Exit Sub
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Function BGMStatus() As String
		Dim retstr As String
		Dim ret, sec As Integer
		
		'Invalid_string_refer_to_original_code
		If Len(BGMFileName) = 0 Then
			Exit Function
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDIファイル
				If UseDirectMusic Then
					'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
					retstr = Space(120)
					
					'Invalid_string_refer_to_original_code
					ret = mciSendString("status bgm mode", retstr, 120, 0)
					If ret <> 0 Then
						Exit Function
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(retstr, Chr(0))
					BGMStatus = Left(retstr, ret - 1)
				End If
				
			Case ".wav"
				'WAVEファイル
				
				'Invalid_string_refer_to_original_code
				retstr = Space(120)
				
				ret = mciSendString("status bgm mode", retstr, 120, 0)
				If ret <> 0 Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(retstr, Chr(0))
				BGMStatus = Left(retstr, ret - 1)
				
			Case ".mp3"
				'Invalid_string_refer_to_original_code
				ret = vbmp3_getState(sec)
				
				Select Case ret
					Case 0
						'停止中
						BGMStatus = "stopped"
					Case 1
						'再生中
						BGMStatus = "playing"
					Case 2
						'一時停止中
						BGMStatus = "stopped"
				End Select
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub ChangeBGM(ByRef bgm_name As String)
		Dim fname, fname2 As String
		
		'Invalid_string_refer_to_original_code
		If KeepBGM Or BossBGM Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		fname = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname, "\") > 0 Then
			fname = Mid(fname, InStr2(fname, "\") + 1)
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(BGMFileName) > 0 Then
			If InStr(BGMFileName, "\" & fname & ".") > 0 Then
				Exit Sub
			End If
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		RepeatMode = True
		
		'Invalid_string_refer_to_original_code
		StartBGM(bgm_name)
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		UseDirectMusic = True
		UseMCI = False
		
		'Invalid_string_refer_to_original_code
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectMusicLoaderCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DMLoader = DXObject.DirectMusicLoaderCreate
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DMLoader.SetSearchDirectory �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMLoader.SetSearchDirectory(AppPath & "Midi")
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectMusicPerformanceCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DMPerformance = DXObject.DirectMusicPerformanceCreate
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'DirectSoundのオブジェクトを入れておく
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.Init �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMPerformance.Init(DSObject, MainForm.Handle.ToInt32)
		
		'Invalid_string_refer_to_original_code
		CreateMIDIPortListFile()
		
		'Invalid_string_refer_to_original_code
		port_id = StrToLng(ReadIni("Option", "MIDIPortID"))
		
		'Invalid_string_refer_to_original_code
		If port_id > 0 Then
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If port_id > DMPerformance.GetPortCount Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				End
			End If
			
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Call DMPerformance.SetPort(port_id, 1)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "Invalid_string_refer_to_original_code") > 0 Or InStr(DMPerformance.GetPortName(i), "MIDI Mapper") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
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
		
		'次にXG対応ハード音源を捜す
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
		
		'次にGS対応ハード音源を捜す
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
		
		'次にXG対応ソフト音源を捜す
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "XG ") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にGS対応ソフト音源を捜す
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortCount �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.GetPortName �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If InStr(DMPerformance.GetPortName(i), "GS ") > 0 Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にGM対応ハード音源を捜す
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
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.SetPort �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Call DMPerformance.SetPort(-1, 1)
		
		Exit Sub
		
ErrorHandler: 
		
		'Invalid_string_refer_to_original_code
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Function CreateDirectXObject() As DirectX7
		'UPGRADE_ISSUE: DirectX7 �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim new_obj As New DirectX7
		
		CreateDirectXObject = new_obj
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Sub CreateMIDIPortListFile()
		Dim f, i As Short
		Dim pname As String
		
		On Error GoTo ErrorHandler
		
		f = FreeFile
		FileOpen(f, AppPath & "Invalid_string_refer_to_original_code", OpenMode.Output, OpenAccess.Write)
		
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "")
		
		'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If FileExists(AppPath & "VBMP3.dll") Then
				is_mp3_available = True
			End If
			
			IsMidiSearchPathInitialized = True
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(midi_name) < 5 Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		num = ListLength(midi_name)
		i = 1
		Do While i <= num
			'Invalid_string_refer_to_original_code
			buf = ""
			For j = i To num
				buf2 = LCase(ListIndex(midi_name, j))
				
				'Invalid_string_refer_to_original_code
				If Left(buf2, 1) = "(" And Right(buf2, 1) = ")" Then
					buf2 = Mid(buf2, 2, Len(buf2) - 2)
				End If
				
				buf = buf & " " & buf2
				
				If Right(buf, 4) = ".mid" Then
					Exit For
				End If
			Next 
			buf = Trim(buf)
			
			'Invalid_string_refer_to_original_code
			If is_mp3_available Then
				fname_mp3 = Left(buf, Len(buf) - 4) & ".mp3"
			End If
			
			'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			'        On Error GoTo NotFound
			'        fname = fpath_history.Item(buf)
			'
			'Invalid_string_refer_to_original_code
			'        SearchMidiFile = fname
			'        Exit Function
			
			'NotFound:
			'Invalid_string_refer_to_original_code
			'        On Error GoTo 0
			' DEL END MARGE
			
			'Invalid_string_refer_to_original_code
			If InStr(buf, "_") > 0 Then
				sub_folder = Left(buf, InStr(buf, "_") - 1) & "\"
			End If
			
			'シナリオ側のMidiフォルダ
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
			
			'ExtDataPath側のMidiフォルダ
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
			
			'ExtDataPath2側のMidiフォルダ
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
			
			'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub ResetMidiSearchPath()
		IsMidiSearchPathInitialized = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Function BGMName(ByRef bgm_name As String) As String
		Dim vname As String
		
		'Invalid_string_refer_to_original_code
		vname = "BGM(" & bgm_name & ")"
		If IsGlobalVariableDefined(vname) Then
			'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			BGMName = GlobalVariableList.Item(vname).StringValue
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		BGMName = ReadIni("BGM", bgm_name)
		
		'Invalid_string_refer_to_original_code
		If BGMName = "" Then
			BGMName = bgm_name & ".mid"
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub InitDirectSound()
		Dim DSSCL_PRIORITY As Object
		'On Error GoTo ErrorHandler
		Exit Sub
		
		'Invalid_string_refer_to_original_code
		UseDirectSound = True
		
		'Invalid_string_refer_to_original_code
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DXObject.DirectSoundCreate �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DSObject = DXObject.DirectSoundCreate("")
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g DSObject.SetCooperativeLevel �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		DSObject.SetCooperativeLevel(MainForm.Handle.ToInt32, DSSCL_PRIORITY)
		
		Exit Sub
		
ErrorHandler: 
		
		UseDirectSound = False
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(wave_name)
			Case "-.wav", "-.mp3"
				'再生をキャンセル
				Exit Sub
			Case "null.wav"
				'WAVE再生を停止
				StopWave()
				Exit Sub
			Case "null.mp3"
				'MP3再生を停止
				If LCase(Right(BGMFileName, 4)) = ".mp3" Then
					StopBGM(True)
				Else
					'演奏を停止
					Call vbmp3_stop()
					'ファイルを閉じる
					Call vbmp3_close()
				End If
				Exit Sub
		End Select
		
		'Invalid_string_refer_to_original_code
		
		'シナリオ側のSoundフォルダ
		If scenario_sound_dir_exists Then
			fname = ScenarioPath & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'ExtDataPath側のSoundフォルダ
		If extdata_sound_dir_exists Then
			fname = ExtDataPath & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'ExtDataPath2側のSoundフォルダ
		If extdata2_sound_dir_exists Then
			fname = ExtDataPath2 & "Sound\" & wave_name
			If FileExists(fname) Then
				GoTo FoundWave
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		fname = AppPath & "Sound\" & wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'Invalid_string_refer_to_original_code
		fname = wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'Invalid_string_refer_to_original_code
		Exit Sub
		
FoundWave: 
		
		'UPGRADE_ISSUE: WAVEFORMATEX �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim wf As WAVEFORMATEX
		'UPGRADE_ISSUE: DSBUFFERDESC �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		Dim dsbd As DSBUFFERDESC
		If LCase(Right(fname, 4)) = ".mp3" Then
			'効果音はMP3ファイル
			
			'Invalid_string_refer_to_original_code
			If Not IsMP3Supported Then
				InitVBMP3()
				
				If Not IsMP3Supported Then
					'Invalid_string_refer_to_original_code
					Exit Sub
				End If
			End If
			
			'MP3再生を停止
			If LCase(Right(BGMFileName, 4)) = ".mp3" Then
				StopBGM(True)
			Else
				'演奏を停止
				Call vbmp3_stop()
				'ファイルを閉じる
				Call vbmp3_close()
			End If
			
			'ファイルを読み込む
			If vbmp3_open(fname, mp3_data) Then
				'Invalid_string_refer_to_original_code
				Call vbmp3_play()
			End If
		ElseIf UseDirectSound Then 
			'Invalid_string_refer_to_original_code
			
			
			'Invalid_string_refer_to_original_code
			If Not DSBuffer Is Nothing Then
				'UPGRADE_WARNING: �I�u�W�F�N�g DSBuffer.Stop �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				DSBuffer.Stop()
				'UPGRADE_NOTE: �I�u�W�F�N�g DSBuffer ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				DSBuffer = Nothing
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g dsbd.lFlags �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			dsbd.lFlags = DSBCAPS_CTRLFREQUENCY Or DSBCAPS_CTRLPAN Or DSBCAPS_CTRLVOLUME Or DSBCAPS_STATIC
			'UPGRADE_WARNING: �I�u�W�F�N�g DSObject.CreateSoundBufferFromFile �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DSBuffer = DSObject.CreateSoundBufferFromFile(fname, dsbd, wf)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g DSBuffer.Play �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DSBuffer.Play(DSBPLAY_DEFAULT)
		Else
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ret = sndPlaySound(fname, SND_ASYNC + SND_NODEFAULT)
		End If
		
		'Invalid_string_refer_to_original_code
		IsWavePlayed = True
	End Sub
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	Public Sub FreeSoundModule()
		'Invalid_string_refer_to_original_code
		KeepBGM = False
		BossBGM = False
		StopBGM(True)
		
		'Invalid_string_refer_to_original_code
		ResetBGM()
		
		'WAVEファイル再生の停止
		StopWave()
		
		'DirectMusicの解放
		If UseDirectMusic Then
			'演奏停止
			'UPGRADE_WARNING: �I�u�W�F�N�g DMPerformance.CloseDown �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			DMPerformance.CloseDown()
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: �I�u�W�F�N�g DMLoader ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMLoader = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DMPerformance ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMPerformance = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DMSegment ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DMSegment = Nothing
		End If
		
		'DirectSoundの解放
		If UseDirectSound Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: �I�u�W�F�N�g DSObject ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DSObject = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g DSBuffer ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DSBuffer = Nothing
		End If
		
		'DirectXの解放
		'UPGRADE_NOTE: �I�u�W�F�N�g DXObject ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		DXObject = Nothing
		
		'VBMP3.DLLの解放
		If IsMP3Supported Then
			Call vbmp3_stop()
			Call vbmp3_free()
		End If
	End Sub
End Module