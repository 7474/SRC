Option Strict Off
Option Explicit On
Module Sound
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ＢＧＭ＆効果音再生用のモジュール
	
	'MCI制御用API
	Declare Function mciSendString Lib "winmm.dll"  Alias "mciSendStringA"(ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer
	
	'WAVE再生用API
	Declare Function sndPlaySound Lib "winmm.dll"  Alias "sndPlaySoundA"(ByVal lpszSoundName As String, ByVal uFlags As Integer) As Integer
	
	Public Const SND_SYNC As Integer = &H0 '再生終了後、制御を戻す
	Public Const SND_ASYNC As Integer = &H1 '関数実行後、制御を戻す
	Public Const SND_NODEFAULT As Integer = &H2 '指定したWAVEファイルが見つからなかった場合、
	'デフォルトのWAVEファイルを再生しない
	Public Const SND_MEMORY As Integer = &H4 'メモリファイルのWAVEを実行する
	Public Const SND_LOOP As Integer = &H8 '停止を命令するまで再生を繰り返す。
	Public Const SND_NOSTOP As Integer = &H10 '現在Waveファイルが再生中の場合、再生を中止する
	
	
	'現在再生されているＢＧＭのファイル名
	Public BGMFileName As String
	'ＢＧＭをリピート再生する？
	Public RepeatMode As Boolean
	'戦闘時にもＢＧＭを変更しない？
	Public KeepBGM As Boolean
	'ボス用ＢＧＭを演奏中
	Public BossBGM As Boolean
	
	'Waveファイルの再生を行った？
	Public IsWavePlayed As Boolean
	
	'MIDIファイルのサーチパスの初期化が完了している？
	Private IsMidiSearchPathInitialized As Boolean
	
	'MIDI再生方法の手段
	Public UseMCI As Boolean
	Public UseDirectMusic As Boolean
	
	'WAV再生方法の手段
	Public UseDirectSound As Boolean
	
	'MP3再生時の音量
	Public MP3Volume As Short
	
	'DirectMusic用変数
	'UPGRADE_ISSUE: DirectX7 オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DXObject As DirectX7
	'UPGRADE_ISSUE: DirectMusicLoader オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DMLoader As DirectMusicLoader
	'UPGRADE_ISSUE: DirectMusicPerformance オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DMPerformance As DirectMusicPerformance
	'UPGRADE_ISSUE: DirectMusicSegment オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DMSegment As DirectMusicSegment
	
	'VBMP3.dllの初期化が完了している？
	Private IsMP3Supported As Boolean
	
	'DirectSound用変数
	'UPGRADE_ISSUE: DirectSound オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DSObject As DirectSound
	'UPGRADE_ISSUE: DirectSoundBuffer オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
	Private DSBuffer As DirectSoundBuffer
	
	
	'ＢＧＭの再生を開始する
	Public Sub StartBGM(ByRef bgm_name As String, Optional ByVal is_repeat_mode As Boolean = True)
		Dim fname0, fname, fname2 As String
		Dim i As Short
		
		'ＢＧＭを固定中？
		If KeepBGM Then
			Exit Sub
		End If
		
		'ダミーのファイル名？
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'ファイル名の本体部分を抜き出す
		fname0 = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname0, "\") > 0 Then
			fname0 = Mid(fname0, InStr2(fname0, "\") + 1)
		End If
		
		'同じＢＧＭを演奏中であれば演奏を継続
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
		
		'ファイルが見つかった？
		If Len(fname) = 0 Then
			Exit Sub
		End If
		
		'演奏をストップ
		StopBGM()
		
		'同じＢＧＭにバリエーションがあればランダムで選択
		i = 1
		If InStr(fname, ScenarioPath) > 0 Then
			'シナリオ側にファイルが見つかった場合はバリエーションもシナリオ側からのみ選択
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While InStr(fname2, ScenarioPath) > 0
		Else
			'そうでなければ両方から選択
			Do 
				i = i + 1
				fname2 = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
			Loop While fname2 <> ""
		End If
		
		i = Int(((i - 1) * Rnd()) + 1)
		If i > 1 Then
			fname = SearchMidiFile("(" & fname0 & "(" & VB6.Format(i) & ")" & Right(fname, 4) & ")")
		End If
		
		'ＢＧＭを連続演奏？
		RepeatMode = is_repeat_mode
		
		'ファイルをロードし、演奏開始
		LoadBGM(fname)
		
		'リピート再生処理を行うためのタイマーを起動
		'UPGRADE_ISSUE: Control Timer1 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.Timer1.Enabled = True
	End Sub
	
	'ＢＧＭのファイルを読み込む
	Private Sub LoadBGM(ByRef fname As String)
		Dim ret As Integer
		Dim cmd As String
		Dim mp3_data As InputInfo
		
		'ファイルの種類に応じた処理を行う
		Select Case LCase(Right(fname, 4))
			Case ".mid"
				'MIDIファイル
				
				'MIDIを演奏するのが初めて？
				If Not UseDirectMusic And Not UseMCI Then
					'DirectMusicの初期化を試みる
					InitDirectMusic()
				End If
				
				'音源リセット
				ResetBGM()
				
				'DirectMusicを使う？
				If UseDirectMusic Then
					'ファイルをロード
					On Error GoTo ErrorHandler
					
					'UPGRADE_WARNING: オブジェクト DMLoader.LoadSegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					DMSegment = DMLoader.LoadSegment(fname)
					If Err.Number <> 0 Then
						ErrorMessage("LoadSegment failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'UPGRADE_WARNING: オブジェクト DMSegment.SetStandardMidiFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					Call DMSegment.SetStandardMidiFile()
					If Err.Number <> 0 Then
						ErrorMessage("SetStandardMidiFile failed (" & VB6.Format(Err.Number) & ")")
					End If
					'UPGRADE_WARNING: オブジェクト DMPerformance.SetMasterAutoDownload の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					Call DMPerformance.SetMasterAutoDownload(True)
					If Err.Number <> 0 Then
						ErrorMessage("SetMasterAutoDownload failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'ループ演奏の設定
					'繰り返し範囲を設定
					'UPGRADE_WARNING: オブジェクト DMSegment.SetLoopPoints の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					Call DMSegment.SetLoopPoints(0, 0)
					If Err.Number <> 0 Then
						ErrorMessage("SetLoopPoints failed (" & VB6.Format(Err.Number) & ")")
					End If
					'繰り返し回数を設定
					If RepeatMode Then
						'UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						Call DMSegment.SetRepeats(-1)
					Else
						'UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						Call DMSegment.SetRepeats(0)
					End If
					If Err.Number <> 0 Then
						ErrorMessage("SetRepeats failed (" & VB6.Format(Err.Number) & ")")
					End If
					
					'演奏開始
					'UPGRADE_WARNING: オブジェクト DMPerformance.PlaySegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
				
				'VBMP3.dllを初期化
				If Not IsMP3Supported Then
					InitVBMP3()
					
					If Not IsMP3Supported Then
						'VBMP3.dllが利用不能
						Exit Sub
					End If
				End If
				
				'演奏を停止
				Call vbmp3_stop()
				'ファイルを閉じる
				Call vbmp3_close()
				
				'ファイルを読み込む
				If vbmp3_open(fname, mp3_data) Then
					'                '繰り返し再生時は雑音が入らないようフェードアウトさせる
					'                If RepeatMode Then
					'                    Call vbmp3_setFadeOut(1)
					'                Else
					'                    Call vbmp3_setFadeOut(0)
					'                End If
					
					'演奏開始
					Call vbmp3_play()
					BGMFileName = fname
				End If
				Exit Sub
				
			Case Else
				'未サポートのファイル形式
				Exit Sub
		End Select
		
		'ファイルを開く
		cmd = "open " & Chr(34) & fname & Chr(34) & cmd
		ret = mciSendString(cmd, vbNullString, 0, 0)
		If ret <> 0 Then
			'開けなかった
			Exit Sub
		End If
		
		'演奏開始
		ret = mciSendString("play bgm", vbNullString, 0, 0)
		If ret <> 0 Then
			'演奏できなかった
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
			Exit Sub
		End If
		
		'演奏しているBGMのファイル名を記録
		BGMFileName = fname
		Exit Sub
		
ErrorHandler: 
		If UseDirectMusic Then
			'DirectMusicが使用できない場合はMCIを使ってリトライ
			UseDirectMusic = False
			UseMCI = True
			LoadBGM(fname)
		End If
	End Sub
	
	'ＢＧＭをリスタートさせる
	Public Sub RestartBGM()
		Dim ret As Integer
		
		'停止中でなければ何もしない
		If BGMStatus() <> "stopped" Then
			Exit Sub
		End If
		
		'リスタート
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
	
	'ＢＧＭを停止する
	Public Sub StopBGM(Optional ByVal by_force As Boolean = False)
		Dim ret As Integer
		
		'ＢＧＭを固定中？
		If Not by_force And KeepBGM Then
			Exit Sub
		End If
		
		'強制的に停止するのでなければ演奏中でない限りなにもしない
		If Not by_force And Len(BGMFileName) = 0 Then
			Exit Sub
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDIファイル
				If UseDirectMusic Then
					'演奏を停止
					On Error Resume Next
					'UPGRADE_WARNING: オブジェクト DMPerformance.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
		
		'リピート再生処理を行うためのタイマーを停止
		'UPGRADE_ISSUE: Control Timer1 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.Timer1.Enabled = False
	End Sub
	
	'ＭＩＤＩ音源を初期化する(MCIを使用する場合のみ)
	Private Sub ResetBGM()
		Dim ret As Integer
		Dim fname, cmd As String
		
		'音源の種類に応じた音源初期化用MIDIファイルを選択
		Select Case MidiResetType
			Case "GM"
				If UseDirectMusic Then
					'DirectMusicを使えばGMリセットが可能
					On Error GoTo ErrorHandler
					'UPGRADE_WARNING: オブジェクト DMPerformance.Reset の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
		
		'ファイルがちゃんとある？
		If Not FileExists(fname) Then
			Exit Sub
		End If
		
		BGMFileName = ""
		
		If UseDirectMusic Then
			'DirectMusicを使う場合
			On Error GoTo ErrorHandler
			
			'ファイルをロード
			'UPGRADE_WARNING: オブジェクト DMLoader.LoadSegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			DMSegment = DMLoader.LoadSegment(fname)
			
			'MIDI再生のため各種パラメータを設定
			'UPGRADE_WARNING: オブジェクト DMSegment.SetStandardMidiFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMSegment.SetStandardMidiFile()
			'UPGRADE_WARNING: オブジェクト DMPerformance.SetMasterAutoDownload の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.SetMasterAutoDownload(True)
			'UPGRADE_WARNING: オブジェクト DMSegment.SetLoopPoints の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMSegment.SetLoopPoints(0, 0)
			'UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMSegment.SetRepeats(0)
			
			'音源リセット用MIDIファイルの演奏開始
			'UPGRADE_WARNING: オブジェクト DMPerformance.PlaySegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.PlaySegment(DMSegment, 0, 0)
			
			'演奏が終わるまで待つ
			'UPGRADE_WARNING: オブジェクト DMPerformance.IsPlaying の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Do While DMPerformance.IsPlaying(DMSegment, Nothing)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'演奏を停止
			'UPGRADE_WARNING: オブジェクト DMPerformance.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.Stop(DMSegment, Nothing, 0, 0)
		Else
			'MCIを使う場合
			
			'ファイルをオープン
			cmd = "open " & Chr(34) & fname & Chr(34) & " type sequencer alias bgm wait"
			ret = mciSendString(cmd, vbNullString, 0, 0)
			If ret <> 0 Then
				Exit Sub
			End If
			
			'音源リセット用MIDIファイルを演奏
			ret = mciSendString("play bgm wait", vbNullString, 0, 0)
			
			'ファイルをクローズ
			ret = mciSendString("close bgm wait", vbNullString, 0, 0)
		End If
		
		Exit Sub
		
ErrorHandler: 
		'DirectMusic使用時にエラーが発生したのでMCIを使う
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'ＢＧＭを再生中？
	Private Function BGMStatus() As String
		Dim retstr As String
		Dim ret, sec As Integer
		
		'ＢＧＭを演奏中でなければ空文字列を返す
		If Len(BGMFileName) = 0 Then
			Exit Function
		End If
		
		Select Case LCase(Right(BGMFileName, 4))
			Case ".mid", ""
				'MIDIファイル
				If UseDirectMusic Then
					'DirectMusicを使う場合
					On Error Resume Next
					'UPGRADE_WARNING: オブジェクト DMPerformance.IsPlaying の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If DMPerformance.IsPlaying(DMSegment, Nothing) Then
						BGMStatus = "playing"
					Else
						BGMStatus = "stopped"
					End If
					If Err.Number <> 0 Then
						ErrorMessage("DMPerformance.IsPlaying failed (" & VB6.Format(Err.Number) & ")")
					End If
				Else
					'MCIを使う場合
					
					'結果を保存する領域を確保
					retstr = Space(120)
					
					'再生状況を参照
					ret = mciSendString("status bgm mode", retstr, 120, 0)
					If ret <> 0 Then
						Exit Function
					End If
					
					'APIの結果はNULLターミネイト
					ret = InStr(retstr, Chr(0))
					BGMStatus = Left(retstr, ret - 1)
				End If
				
			Case ".wav"
				'WAVEファイル
				
				'結果を保存する領域を確保
				retstr = Space(120)
				
				ret = mciSendString("status bgm mode", retstr, 120, 0)
				If ret <> 0 Then
					Exit Function
				End If
				
				'APIの結果はNULLターミネイト
				ret = InStr(retstr, Chr(0))
				BGMStatus = Left(retstr, ret - 1)
				
			Case ".mp3"
				'MP3の再生状態と再生時間の取得
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
	
	'ＢＧＭを変更する (指定したＢＧＭをすでに演奏中ならなにもしない)
	Public Sub ChangeBGM(ByRef bgm_name As String)
		Dim fname, fname2 As String
		
		'ＢＧＭ固定中？
		If KeepBGM Or BossBGM Then
			Exit Sub
		End If
		
		'正しいファイル名？
		If Len(bgm_name) < 5 Then
			Exit Sub
		End If
		
		'ファイル名の本体部分を抜き出す
		fname = Left(bgm_name, Len(bgm_name) - 4)
		If InStr2(fname, "\") > 0 Then
			fname = Mid(fname, InStr2(fname, "\") + 1)
		End If
		
		'既に同じMIDIが演奏されていればそのまま演奏し続ける
		If Len(BGMFileName) > 0 Then
			If InStr(BGMFileName, "\" & fname & ".") > 0 Then
				Exit Sub
			End If
		End If
		
		'番号違い？
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
		
		'繰り返し演奏に設定
		RepeatMode = True
		
		'演奏開始
		StartBGM(bgm_name)
	End Sub
	
	
	'DirectMusicの初期化
	Public Sub InitDirectMusic()
		Dim DMUS_PC_GMINHARDWARE As Object
		Dim DMUS_PC_GSINHARDWARE As Object
		Dim DMUS_PC_XGINHARDWARE As Object
		Dim DMUS_PC_EXTERNAL As Object
		Dim port_id As Short
		'UPGRADE_ISSUE: DMUS_PORTCAPS オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		Dim portcaps As DMUS_PORTCAPS
		Dim i As Short
		
		On Error GoTo ErrorHandler
		
		'フラグを設定
		UseDirectMusic = True
		UseMCI = False
		
		'DirectXオブジェクト作成
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'Loader作成
		'UPGRADE_WARNING: オブジェクト DXObject.DirectMusicLoaderCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		DMLoader = DXObject.DirectMusicLoaderCreate
		
		'サーチパス設定(不要？)
		'UPGRADE_WARNING: オブジェクト DMLoader.SetSearchDirectory の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		Call DMLoader.SetSearchDirectory(AppPath & "Midi")
		
		'Performance作成
		'UPGRADE_WARNING: オブジェクト DXObject.DirectMusicPerformanceCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		DMPerformance = DXObject.DirectMusicPerformanceCreate
		
		'Performance初期化
		'DirectSoundと併用する時は、最初の引数に
		'DirectSoundのオブジェクトを入れておく
		'UPGRADE_WARNING: オブジェクト DMPerformance.Init の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		Call DMPerformance.Init(DSObject, MainForm.Handle.ToInt32)
		
		'MIDI音源一覧を作成
		CreateMIDIPortListFile()
		
		'ポート設定
		port_id = StrToLng(ReadIni("Option", "MIDIPortID"))
		
		'使用ポート番号を指定されていた場合
		If port_id > 0 Then
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If port_id > DMPerformance.GetPortCount Then
				ErrorMessage("MIDIPortIDに正しいMIDIポートが設定されていません。")
				End
			End If
			
			'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.SetPort(port_id, 1)
			Exit Sub
		End If
		
		'指定がないのでSRC側で検索する
		
		'MIDIマッパーがあればそれを使う
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If InStr(DMPerformance.GetPortName(i), "MIDI マッパー") > 0 Or InStr(DMPerformance.GetPortName(i), "MIDI Mapper") > 0 Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'まずは外部MIDI音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If portcaps.lFlags And DMUS_PC_EXTERNAL Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にXG対応ハード音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If portcaps.lFlags And DMUS_PC_XGINHARDWARE Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にGS対応ハード音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If portcaps.lFlags And DMUS_PC_GSINHARDWARE Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にXG対応ソフト音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If InStr(DMPerformance.GetPortName(i), "XG ") > 0 Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にGS対応ソフト音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If InStr(DMPerformance.GetPortName(i), "GS ") > 0 Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'次にGM対応ハード音源を捜す
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Call DMPerformance.GetPortCaps(i, portcaps)
			'UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If portcaps.lFlags And DMUS_PC_GMINHARDWARE Then
				'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Call DMPerformance.SetPort(i, 1)
				Exit Sub
			End If
		Next 
		
		'あきらめてデフォルトポートを使う
		'UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		Call DMPerformance.SetPort(-1, 1)
		
		Exit Sub
		
ErrorHandler: 
		
		'DirectMusic初期化時にエラーが発生したのでMCIを使う
		UseDirectMusic = False
		UseMCI = True
	End Sub
	
	'DirectXオブジェクトを作成する
	Private Function CreateDirectXObject() As DirectX7
		'UPGRADE_ISSUE: DirectX7 オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		Dim new_obj As New DirectX7
		
		CreateDirectXObject = new_obj
	End Function
	
	'利用可能なMIDI音源の一覧を作成する
	Private Sub CreateMIDIPortListFile()
		Dim f, i As Short
		Dim pname As String
		
		On Error GoTo ErrorHandler
		
		f = FreeFile
		FileOpen(f, AppPath & "Midi\MIDI音源リスト.txt", OpenMode.Output, OpenAccess.Write)
		
		PrintLine(f, ";DirectMusicで利用可能なMIDI音源のリストです。")
		PrintLine(f, ";Src.iniのMIDIPortIDに使用したい音源の番号を指定して下さい。")
		PrintLine(f, "")
		
		'各ポートの名称を参照
		'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		For i = 1 To DMPerformance.GetPortCount
			'UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			pname = DMPerformance.GetPortName(i)
			If InStr(pname, "[") > 0 Then
				pname = Left(pname, InStr(pname, "[") - 1)
			End If
			PrintLine(f, VB6.Format(i) & ":" & pname)
		Next 
		
		FileClose(f)
		
ErrorHandler: 
		'エラー発生
	End Sub
	
	
	'VBMP3を初期化
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
	
	
	'各Midiフォルダから指定されたMIDIファイルを検索する
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
		
		'初めて実行する際に、各フォルダにMidiフォルダがあるかチェック
		If Not IsMidiSearchPathInitialized Then
			If Len(ScenarioPath) > 0 Then
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Len(Dir(ScenarioPath & "Midi", FileAttribute.Directory)) > 0 Then
					scenario_midi_dir_exists = True
				End If
			End If
			If Len(ExtDataPath) > 0 Then
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Len(Dir(ExtDataPath & "Midi", FileAttribute.Directory)) > 0 Then
					extdata_midi_dir_exists = True
				End If
			End If
			If Len(ExtDataPath2) > 0 Then
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Len(Dir(ExtDataPath2 & "Midi", FileAttribute.Directory)) > 0 Then
					extdata2_midi_dir_exists = True
				End If
			End If
			
			'MP3が演奏可能かどうかも調べておく
			If FileExists(AppPath & "VBMP3.dll") Then
				is_mp3_available = True
			End If
			
			IsMidiSearchPathInitialized = True
		End If
		
		'ダミーのファイル名？
		If Len(midi_name) < 5 Then
			Exit Function
		End If
		
		'引数1として渡された文字列をリストとして扱い、左から順にMIDIを検索
		num = ListLength(midi_name)
		i = 1
		Do While i <= num
			'スペースを含むファイル名への対応
			buf = ""
			For j = i To num
				buf2 = LCase(ListIndex(midi_name, j))
				
				'全体が()で囲まれている場合は()を外す
				If Left(buf2, 1) = "(" And Right(buf2, 1) = ")" Then
					buf2 = Mid(buf2, 2, Len(buf2) - 2)
				End If
				
				buf = buf & " " & buf2
				
				If Right(buf, 4) = ".mid" Then
					Exit For
				End If
			Next 
			buf = Trim(buf)
			
			'同名のMP3ファイルがある場合はMIDIファイルの代わりにMP3ファイルを使う
			If is_mp3_available Then
				fname_mp3 = Left(buf, Len(buf) - 4) & ".mp3"
			End If
			
			'フルパスでの指定？
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
			'        '履歴を検索してみる
			'        On Error GoTo NotFound
			'        fname = fpath_history.Item(buf)
			'
			'        '履歴上にファイルを発見
			'        SearchMidiFile = fname
			'        Exit Function
			
			'NotFound:
			'        '履歴になかった
			'        On Error GoTo 0
			' DEL END MARGE
			
			'サブフォルダ指定あり？
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
			
			'本体側のMidiフォルダ
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
	
	'MIDIファイルのサーチパスをリセットする
	Public Sub ResetMidiSearchPath()
		IsMidiSearchPathInitialized = False
	End Sub
	
	
	'ＢＧＭに割り当てられたMIDIファイル名を返す
	Public Function BGMName(ByRef bgm_name As String) As String
		Dim vname As String
		
		'RenameBGMコマンドでMIDIファイルが設定されていればそちらを使用
		vname = "BGM(" & bgm_name & ")"
		If IsGlobalVariableDefined(vname) Then
			'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			BGMName = GlobalVariableList.Item(vname).StringValue
			Exit Function
		End If
		
		'そうでなければSrc.iniで設定されているファイルを使用
		BGMName = ReadIni("BGM", bgm_name)
		
		'Src.iniでも設定されていなければ標準のファイルを使用
		If BGMName = "" Then
			BGMName = bgm_name & ".mid"
		End If
	End Function
	
	
	'DirectSoundの初期化
	Public Sub InitDirectSound()
		Dim DSSCL_PRIORITY As Object
		'On Error GoTo ErrorHandler
		Exit Sub
		
		'フラグを設定
		UseDirectSound = True
		
		'DirectXオブジェクト作成
		If DXObject Is Nothing Then
			DXObject = CreateDirectXObject()
		End If
		
		'DirectSoundオブジェクト作成
		'UPGRADE_WARNING: オブジェクト DXObject.DirectSoundCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		DSObject = DXObject.DirectSoundCreate("")
		
		'サウンドデバイスの協調レベルを設定
		'UPGRADE_WARNING: オブジェクト DSObject.SetCooperativeLevel の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		DSObject.SetCooperativeLevel(MainForm.Handle.ToInt32, DSSCL_PRIORITY)
		
		Exit Sub
		
ErrorHandler: 
		
		UseDirectSound = False
	End Sub
	
	'Waveファイルを再生する
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
		
		'初めて実行する際に、各フォルダにSoundフォルダがあるかチェック
		If Not init_play_wave Then
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Sound", FileAttribute.Directory)) > 0 Then
				scenario_sound_dir_exists = True
			End If
			If Len(ExtDataPath) > 0 Then
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Len(Dir(ExtDataPath & "Sound", FileAttribute.Directory)) > 0 Then
					extdata_sound_dir_exists = True
				End If
			End If
			If Len(ExtDataPath2) > 0 Then
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Len(Dir(ExtDataPath2 & "Sound", FileAttribute.Directory)) > 0 Then
					extdata2_sound_dir_exists = True
				End If
			End If
			init_play_wave = True
		End If
		
		'特殊なファイル名
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
		
		'各フォルダをチェック
		
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
		
		'本体側のSoundフォルダ
		fname = AppPath & "Sound\" & wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'絶対表記？
		fname = wave_name
		If FileExists(fname) Then
			GoTo FoundWave
		End If
		
		'見つからなかった
		Exit Sub
		
FoundWave: 
		
		'UPGRADE_ISSUE: WAVEFORMATEX オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		Dim wf As WAVEFORMATEX
		'UPGRADE_ISSUE: DSBUFFERDESC オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		Dim dsbd As DSBUFFERDESC
		If LCase(Right(fname, 4)) = ".mp3" Then
			'効果音はMP3ファイル
			
			'VBMP3.dllを初期化
			If Not IsMP3Supported Then
				InitVBMP3()
				
				If Not IsMP3Supported Then
					'VBMP3.dllが利用不能
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
				'再生開始
				Call vbmp3_play()
			End If
		ElseIf UseDirectSound Then 
			'DirectSoundを使う場合
			
			
			'再生中の場合は再生をストップ
			If Not DSBuffer Is Nothing Then
				'UPGRADE_WARNING: オブジェクト DSBuffer.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				DSBuffer.Stop()
				'UPGRADE_NOTE: オブジェクト DSBuffer をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				DSBuffer = Nothing
			End If
			
			'サウンドバッファにWAVファイルを読み込む
			'UPGRADE_WARNING: オブジェクト dsbd.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			dsbd.lFlags = DSBCAPS_CTRLFREQUENCY Or DSBCAPS_CTRLPAN Or DSBCAPS_CTRLVOLUME Or DSBCAPS_STATIC
			'UPGRADE_WARNING: オブジェクト DSObject.CreateSoundBufferFromFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			DSBuffer = DSObject.CreateSoundBufferFromFile(fname, dsbd, wf)
			
			'WAVEを再生
			'UPGRADE_WARNING: オブジェクト DSBuffer.Play の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			DSBuffer.Play(DSBPLAY_DEFAULT)
		Else
			'APIを使う場合
			
			'WAVEを再生
			ret = sndPlaySound(fname, SND_ASYNC + SND_NODEFAULT)
		End If
		
		'効果音再生のフラグを立てる
		IsWavePlayed = True
	End Sub
	
	'Waveファイルの再生を終了する
	Public Sub StopWave()
		Dim ret As Integer
		
		If UseDirectSound Then
			If Not DSBuffer Is Nothing Then
				'UPGRADE_WARNING: オブジェクト DSBuffer.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				DSBuffer.Stop()
			End If
		Else
			ret = sndPlaySound(vbNullString, 0)
		End If
	End Sub
	
	
	'本モジュールの解放処理を行う
	Public Sub FreeSoundModule()
		'BGM演奏の停止
		KeepBGM = False
		BossBGM = False
		StopBGM(True)
		
		'音源初期化
		ResetBGM()
		
		'WAVEファイル再生の停止
		StopWave()
		
		'DirectMusicの解放
		If UseDirectMusic Then
			'演奏停止
			'UPGRADE_WARNING: オブジェクト DMPerformance.CloseDown の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			DMPerformance.CloseDown()
			
			'オブジェクトの解放
			'UPGRADE_NOTE: オブジェクト DMLoader をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DMLoader = Nothing
			'UPGRADE_NOTE: オブジェクト DMPerformance をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DMPerformance = Nothing
			'UPGRADE_NOTE: オブジェクト DMSegment をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DMSegment = Nothing
		End If
		
		'DirectSoundの解放
		If UseDirectSound Then
			'オブジェクトの解放
			'UPGRADE_NOTE: オブジェクト DSObject をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DSObject = Nothing
			'UPGRADE_NOTE: オブジェクト DSBuffer をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DSBuffer = Nothing
		End If
		
		'DirectXの解放
		'UPGRADE_NOTE: オブジェクト DXObject をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		DXObject = Nothing
		
		'VBMP3.DLLの解放
		If IsMP3Supported Then
			Call vbmp3_stop()
			Call vbmp3_free()
		End If
	End Sub
End Module