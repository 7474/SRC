Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Module SRC
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public PDList As New PilotDataList
	'Invalid_string_refer_to_original_code
	Public NPDList As New NonPilotDataList
	'Invalid_string_refer_to_original_code
	Public UDList As New UnitDataList
	'Invalid_string_refer_to_original_code
	Public IDList As New ItemDataList
	'Invalid_string_refer_to_original_code
	Public MDList As New MessageDataList
	'Invalid_string_refer_to_original_code
	Public EDList As New MessageDataList
	'Invalid_string_refer_to_original_code
	Public ADList As New MessageDataList
	'Invalid_string_refer_to_original_code
	Public EADList As New MessageDataList
	'Invalid_string_refer_to_original_code
	Public DDList As New DialogDataList
	'Invalid_string_refer_to_original_code
	Public SPDList As New SpecialPowerDataList
	'Invalid_string_refer_to_original_code
	Public ALDList As New AliasDataList
	'Invalid_string_refer_to_original_code
	Public TDList As New TerrainDataList
	'Invalid_string_refer_to_original_code
	Public BCList As New BattleConfigDataList
	
	
	'Invalid_string_refer_to_original_code
	Public PList As New Pilots
	'Invalid_string_refer_to_original_code
	Public UList As New Units
	'Invalid_string_refer_to_original_code
	Public IList As New Items
	
	'Invalid_string_refer_to_original_code
	Public ScenarioFileName As String
	'Invalid_string_refer_to_original_code
	Public ScenarioPath As String
	'Invalid_string_refer_to_original_code
	Public SaveDataFileNumber As Short
	'Invalid_string_refer_to_original_code
	Public SaveDataVersion As Integer
	
	'Invalid_string_refer_to_original_code
	Public IsScenarioFinished As Boolean
	'Invalid_string_refer_to_original_code
	Public IsSubStage As Boolean
	'Invalid_string_refer_to_original_code
	Public IsCanceled As Boolean
	
	'Invalid_string_refer_to_original_code
	Public Stage As String
	'ターン数
	Public Turn As Short
	'総ターン数
	Public TotalTurn As Integer
	'Invalid_string_refer_to_original_code
	Public Money As Integer
	'Invalid_string_refer_to_original_code
	Public Titles() As String
	'Invalid_string_refer_to_original_code
	Public IsLocalDataLoaded As Boolean
	
	'Invalid_string_refer_to_original_code
	Public LastSaveDataFileName As String
	'Invalid_string_refer_to_original_code
	Public IsRestartSaveDataAvailable As Boolean
	'Invalid_string_refer_to_original_code
	Public IsQuickSaveDataAvailable As Boolean
	
	'Invalid_string_refer_to_original_code
	'マス目の表示をするか
	Public ShowSquareLine As Boolean
	'Invalid_string_refer_to_original_code
	Public KeepEnemyBGM As Boolean
	'Invalid_string_refer_to_original_code
	Public ExtDataPath As String
	Public ExtDataPath2 As String
	'Invalid_string_refer_to_original_code
	Public MidiResetType As String
	'Invalid_string_refer_to_original_code
	Public AutoMoveCursor As Boolean
	'Invalid_string_refer_to_original_code
	Public SpecialPowerAnimation As Boolean
	'Invalid_string_refer_to_original_code
	Public BattleAnimation As Boolean
	'Invalid_string_refer_to_original_code
	Public WeaponAnimation As Boolean
	'Invalid_string_refer_to_original_code
	Public ExtendedAnimation As Boolean
	'Invalid_string_refer_to_original_code
	Public MoveAnimation As Boolean
	'Invalid_string_refer_to_original_code
	Public ImageBufferSize As Short
	'Invalid_string_refer_to_original_code
	Public MaxImageBufferByteSize As Integer
	'Invalid_string_refer_to_original_code
	Public KeepStretchedImage As Boolean
	'Invalid_string_refer_to_original_code
	Public UseTransparentBlt As Boolean
	
	'SRC.exeのある場所
	Public AppPath As String
	
	'Invalid_string_refer_to_original_code
	Public Const DEFAULT_LEVEL As Short = -1000
	
	'UPGRADE_WARNING: Sub Main() �����������Ƃ��ɃA�v���P�[�V�����͏I�����܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E08DDC71-66BA-424F-A612-80AF11498FF8"' ���N���b�N���Ă��������B
	Public Sub Main()
		Dim fname As String
		Dim i As Short
		Dim buf As String
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: App �v���p�e�B App.PrevInstance �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"' ���N���b�N���Ă��������B
		If App.PrevInstance Then
			End
		End If
		
		'Invalid_string_refer_to_original_code
		AppPath = My.Application.Info.DirectoryPath
		If Right(AppPath, 1) <> "\" Then
			AppPath = AppPath & "\"
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Bitmap", FileAttribute.Directory)) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		End
		'End If
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Bitmap\Bitmap", FileAttribute.Directory)) > 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			& AppPath & "Bitmap\Bitmap" & vbCr & vbLf _
			Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Bitmap\Event", FileAttribute.Directory)) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Bitmap\Map", FileAttribute.Directory)) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Bitmap\Map\plain\plain0000.bmp")) = 0 And Len(Dir(AppPath & "Bitmap\Map\plain0000.bmp")) = 0 And Len(Dir(AppPath & "Bitmap\Map\plain0.bmp")) = 0 Then
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(AppPath & "Bitmap\Map\Map", FileAttribute.Directory)) > 0 Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				& AppPath & "Bitmap\Map\Map" & vbCr & vbLf _
				Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End
			End If
			
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(AppPath & "Bitmap\Map\*", FileAttribute.Normal)) = 0 Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End
			End If
			
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		
		'効果音
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Sound", FileAttribute.Directory)) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		End
		'End If
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Sound\Sound", FileAttribute.Directory)) > 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			& AppPath & "Sound\Sound" & vbCr & vbLf _
			Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Len(Dir(AppPath & "Sound\*", FileAttribute.Normal)) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End
		End If
		
		'メインウィンドウのロードとFlashの登録を実施
		LoadMainFormAndRegisterFlash()
		
		'Invalid_string_refer_to_original_code
		If Not FileExists(AppPath & "Src.ini") Then
			CreateIniFile()
		End If
		
		'Invalid_string_refer_to_original_code
		Randomize()
		
		'時間解像度を変更する
		Call timeBeginPeriod(1)
		
		'Invalid_string_refer_to_original_code
		If LCase(ReadIni("Option", "FullScreen")) = "on" Then
			ChangeDisplaySize(800, 600)
		End If
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'タイトル画面を表示
		OpenTitleForm()
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(ReadIni("Option", "UseDirectSound"))
			Case "on"
				'Invalid_string_refer_to_original_code
				InitDirectSound()
			Case "off"
				UseDirectSound = False
			Case Else
				'Invalid_string_refer_to_original_code
				InitDirectSound()
				'Invalid_string_refer_to_original_code
				'            If UseDirectSound Then
				'                WriteIni "Option", "UseDirectSound", "On"
				'            Else
				'                WriteIni "Option", "UseDirectSound", "Off"
				'            End If
		End Select
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(ReadIni("Option", "UseDirectMusic"))
			Case "on"
				'Invalid_string_refer_to_original_code
				InitDirectMusic()
			Case "off"
				UseMCI = True
			Case Else
				If GetWinVersion() >= 500 Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					InitDirectMusic()
					'Invalid_string_refer_to_original_code
					If UseDirectMusic Then
						WriteIni("Option", "UseDirectMusic", "On")
					Else
						WriteIni("Option", "UseDirectMusic", "Off")
					End If
				Else
					'Invalid_string_refer_to_original_code
					UseMCI = True
					WriteIni("Option", "UseDirectMusic", "Off")
				End If
		End Select
		If ReadIni("Option", "MIDIPortID") = "" Then
			WriteIni("Option", "MIDIPortID", "0")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "MP3Volume")
		If buf = "" Then
			WriteIni("Option", "MP3Volume", "50")
			MP3Volume = 50
		Else
			MP3Volume = StrToLng(buf)
			If MP3Volume < 0 Then
				WriteIni("Option", "MP3Volume", "0")
				MP3Volume = 0
			ElseIf MP3Volume > 100 Then 
				WriteIni("Option", "MP3Volume", "100")
				MP3Volume = 100
			End If
		End If
		
		'MP3の出力フレーム数
		buf = ReadIni("Option", "MP3OutputBlock")
		If buf = "" Then
			WriteIni("Option", "MP3OutputBlock", "20")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "MP3InputSleep")
		If buf = "" Then
			WriteIni("Option", "MP3InputSleep", "5")
		End If
		
		'Invalid_string_refer_to_original_code
		If ReadIni("BGM", "Opening") = "" Then
			WriteIni("BGM", "Opening", "Opening.mid")
		End If
		If ReadIni("BGM", "Map1") = "" Then
			WriteIni("BGM", "Map1", "Map1.mid")
		End If
		If ReadIni("BGM", "Map2") = "" Then
			WriteIni("BGM", "Map2", "Map2.mid")
		End If
		If ReadIni("BGM", "Map3") = "" Then
			WriteIni("BGM", "Map3", "Map3.mid")
		End If
		If ReadIni("BGM", "Map4") = "" Then
			WriteIni("BGM", "Map4", "Map4.mid")
		End If
		If ReadIni("BGM", "Map5") = "" Then
			WriteIni("BGM", "Map5", "Map5.mid")
		End If
		If ReadIni("BGM", "Map6") = "" Then
			WriteIni("BGM", "Map6", "Map6.mid")
		End If
		If ReadIni("BGM", "Briefing") = "" Then
			WriteIni("BGM", "Briefing", "Briefing.mid")
		End If
		If ReadIni("BGM", "Intermission") = "" Then
			WriteIni("BGM", "Intermission", "Intermission.mid")
		End If
		If ReadIni("BGM", "Subtitle") = "" Then
			WriteIni("BGM", "Subtitle", "Subtitle.mid")
		End If
		If ReadIni("BGM", "End") = "" Then
			WriteIni("BGM", "End", "End.mid")
		End If
		If ReadIni("BGM", "default") = "" Then
			WriteIni("BGM", "default", "default.mid")
		End If
		
		
		'Invalid_string_refer_to_original_code
		If Left(VB.Command(), 1) = """" Then
			fname = Mid(VB.Command(), 2, Len(VB.Command()) - 2)
		Else
			fname = VB.Command()
		End If
		
		If LCase(Right(fname, 4)) <> ".src" And LCase(Right(fname, 4)) <> ".eve" Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			i = 0
			ScenarioPath = ReadIni("Log", "LastFolder")
			On Error GoTo ErrorHandler
			If ScenarioPath = "" Then
				ScenarioPath = AppPath
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			ElseIf Dir(ScenarioPath, FileAttribute.Directory) = "." Then 
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Dir(ScenarioPath & "*.src") <> "" Then
					i = 3
				End If
				If InStr(ScenarioPath, "Invalid_string_refer_to_original_code") > 0 Then
					i = 2
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				i = 2
			End If
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Dir(ScenarioPath & "test.eve") <> "" Then
				i = 2
			End If
		Else
			ScenarioPath = AppPath
		End If
		On Error GoTo 0
		GoTo SkipErrorHandler
		
ErrorHandler: 
		ScenarioPath = AppPath
		
SkipErrorHandler: 
		If Right(ScenarioPath, 1) <> "\" Then
			ScenarioPath = ScenarioPath & "\"
		End If
		
		'Invalid_string_refer_to_original_code
		ExtDataPath = ReadIni("Option", "ExtDataPath")
		If Len(ExtDataPath) > 0 Then
			If Right(ExtDataPath, 1) <> "\" Then
				ExtDataPath = ExtDataPath & "\"
			End If
		End If
		ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
		If Len(ExtDataPath2) > 0 Then
			If Right(ExtDataPath2, 1) <> "\" Then
				ExtDataPath2 = ExtDataPath2 & "\"
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		StopBGM(True)
		StartBGM(BGMName("Opening"), True)
		
		'Invalid_string_refer_to_original_code
		InitEventData()
		
		'タイトル画面を閉じる
		CloseTitleForm()
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		'Invalid_string_refer_to_original_code
		ResetMidiSearchPath()
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		'Invalid_string_refer_to_original_code
		If fname = "" Then
			TerminateSRC()
			End
		End If
		
		'Invalid_string_refer_to_original_code
		If InStr(fname, "\") > 0 Then
			For i = 1 To Len(fname)
				If Mid(fname, Len(fname) - i + 1, 1) = "\" Then
					Exit For
				End If
			Next 
			ScenarioPath = Left(fname, Len(fname) - i)
		Else
			ScenarioPath = AppPath
		End If
		If Right(ScenarioPath, 1) <> "\" Then
			ScenarioPath = ScenarioPath & "\"
		End If
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		ExtDataPath = ReadIni("Option", "ExtDataPath")
		If Len(ExtDataPath) > 0 Then
			If Right(ExtDataPath, 1) <> "\" Then
				ExtDataPath = ExtDataPath & "\"
			End If
		End If
		ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
		If Len(ExtDataPath2) > 0 Then
			If Right(ExtDataPath2, 1) <> "\" Then
				ExtDataPath2 = ExtDataPath2 & "\"
			End If
		End If
		' ADD  END  MARGE
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		If fname = "" Then
			TerminateSRC()
			End
		End If
		
		'Invalid_string_refer_to_original_code
		If InStr(fname, "\") > 0 Then
			For i = 1 To Len(fname)
				If Mid(fname, Len(fname) - i + 1, 1) = "\" Then
					Exit For
				End If
			Next 
			ScenarioPath = Left(fname, Len(fname) - i)
		Else
			ScenarioPath = AppPath
		End If
		If Right(ScenarioPath, 1) <> "\" Then
			ScenarioPath = ScenarioPath & "\"
		End If
		
		'Invalid_string_refer_to_original_code
		ExtDataPath = ReadIni("Option", "ExtDataPath")
		If Len(ExtDataPath) > 0 Then
			If Right(ExtDataPath, 1) <> "\" Then
				ExtDataPath = ExtDataPath & "\"
			End If
		End If
		ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
		If Len(ExtDataPath2) > 0 Then
			If Right(ExtDataPath2, 1) <> "\" Then
				ExtDataPath2 = ExtDataPath2 & "\"
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		StopBGM(True)
		StartBGM(BGMName("Opening"), True)
		
		InitEventData()
		
		CloseTitleForm()
		
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		fname = ScenarioPath & Dir(fname)
		If Not FileExists(fname) Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			TerminateSRC()
		End If
		If InStr(fname, "不要ファイル削除") = 0 And InStr(fname, "Invalid_string_refer_to_original_code") = 0 Then
			'開いたフォルダをSrc.iniにセーブしておく
			WriteIni("Log", "LastFolder", ScenarioPath)
		End If
		
		'Invalid_string_refer_to_original_code
		
		'スペシャルパワーアニメ
		buf = ReadIni("Option", "SpecialPowerAnimation")
		If buf = "" Then
			buf = ReadIni("Option", "MindEffect")
			If buf <> "" Then
				WriteIni("Option", "SpecialPowerAnimation", buf)
			End If
		End If
		If buf <> "" Then
			If LCase(buf) = "on" Then
				SpecialPowerAnimation = True
			Else
				SpecialPowerAnimation = False
			End If
		Else
			If SpecialPowerAnimation Then
				WriteIni("Option", "SpecialPowerAnimation", "On")
			Else
				WriteIni("Option", "SpecialPowerAnimation", "Off")
			End If
		End If
		
		'戦闘アニメ
		buf = LCase(ReadIni("Option", "BattleAnimation"))
		If buf <> "" Then
			If buf = "on" Then
				BattleAnimation = True
			Else
				BattleAnimation = False
			End If
		Else
			If BattleAnimation Then
				WriteIni("Option", "BattleAnimation", "On")
			Else
				WriteIni("Option", "BattleAnimation", "Off")
			End If
		End If
		
		'拡大戦闘アニメ
		buf = LCase(ReadIni("Option", "ExtendedAnimation"))
		If buf <> "" Then
			If buf = "on" Then
				ExtendedAnimation = True
			Else
				ExtendedAnimation = False
			End If
		Else
			ExtendedAnimation = True
			WriteIni("Option", "ExtendedAnimation", "On")
		End If
		
		'武器準備アニメ
		buf = LCase(ReadIni("Option", "WeaponAnimation"))
		If buf <> "" Then
			If buf = "on" Then
				WeaponAnimation = True
			Else
				WeaponAnimation = False
			End If
		Else
			WeaponAnimation = True
			WriteIni("Option", "WeaponAnimation", "On")
		End If
		
		'移動アニメ
		buf = LCase(ReadIni("Option", "MoveAnimation"))
		If buf <> "" Then
			If buf = "on" Then
				MoveAnimation = True
			Else
				MoveAnimation = False
			End If
		Else
			MoveAnimation = True
			WriteIni("Option", "MoveAnimation", "On")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "MessageWait")
		If IsNumeric(buf) Then
			MessageWait = CInt(buf)
			If MessageWait > 10000000 Then
				MessageWait = 10000000
			End If
		Else
			MessageWait = 700
			WriteIni("Option", "MessageWait", "700")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "Square")
		If buf <> "" Then
			If LCase(buf) = "on" Then
				ShowSquareLine = True
			Else
				ShowSquareLine = False
			End If
		Else
			ShowSquareLine = False
			WriteIni("Option", "Square", "Off")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "KeepEnemyBGM")
		If buf <> "" Then
			If LCase(buf) = "on" Then
				KeepEnemyBGM = True
			Else
				KeepEnemyBGM = False
			End If
		Else
			KeepEnemyBGM = False
			WriteIni("Option", "KeepEnemyBGM", "Off")
		End If
		
		'Invalid_string_refer_to_original_code
		MidiResetType = ReadIni("Option", "MidiReset")
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "AutoDefense")
		If buf = "" Then
			buf = ReadIni("Option", "AutoDeffence")
			If buf <> "" Then
				WriteIni("Option", "AutoDefense", buf)
			End If
		End If
		If buf <> "" Then
			If LCase(buf) = "on" Then
				'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = True
			Else
				'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
			End If
		Else
			'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
			WriteIni("Option", "AutoDefense", "Off")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "AutoMoveCursor")
		If buf <> "" Then
			If LCase(buf) = "on" Then
				AutoMoveCursor = True
			Else
				AutoMoveCursor = False
			End If
		Else
			AutoMoveCursor = True
			WriteIni("Option", "AutoMoveCursor", "On")
		End If
		
		'Invalid_string_refer_to_original_code
		LoadForms()
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "ImageBufferNum")
		If IsNumeric(buf) Then
			ImageBufferSize = CShort(buf)
			If ImageBufferSize < 5 Then
				'Invalid_string_refer_to_original_code
				ImageBufferSize = 5
			End If
		Else
			'Invalid_string_refer_to_original_code
			ImageBufferSize = 64
			WriteIni("Option", "ImageBufferNum", "64")
		End If
		
		'Invalid_string_refer_to_original_code
		MakePicBuf()
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "MaxImageBufferSize")
		If IsNumeric(buf) Then
			MaxImageBufferByteSize = CDbl(buf) * 1024 * 1024
			If MaxImageBufferByteSize < CInt(1) * 1024 * 1024 Then
				'Invalid_string_refer_to_original_code
				MaxImageBufferByteSize = CInt(1) * 1024 * 1024
			End If
		Else
			'Invalid_string_refer_to_original_code
			MaxImageBufferByteSize = CInt(8) * 1024 * 1024
			WriteIni("Option", "MaxImageBufferSize", "8")
		End If
		
		'Invalid_string_refer_to_original_code
		buf = ReadIni("Option", "KeepStretchedImage")
		If buf <> "" Then
			If LCase(buf) = "on" Then
				KeepStretchedImage = True
			Else
				KeepStretchedImage = False
			End If
		Else
			'UPGRADE_WARNING: �I�u�W�F�N�g IsBitBltFasterThanStretchBlt() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If IsBitBltFasterThanStretchBlt() Then
				KeepStretchedImage = True
				WriteIni("Option", "KeepStretchedImage", "On")
			Else
				KeepStretchedImage = False
				WriteIni("Option", "KeepStretchedImage", "Off")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If GetWinVersion() >= 500 Then
			buf = ReadIni("Option", "UseTransparentBlt")
			If buf <> "" Then
				If LCase(buf) = "on" Then
					UseTransparentBlt = True
				Else
					UseTransparentBlt = False
				End If
			Else
				UseTransparentBlt = True
				WriteIni("Option", "UseTransparentBlt", "On")
			End If
		End If
		
		
		'Invalid_string_refer_to_original_code
		If GetSystemMetrics(SM_SWAPBUTTON) = 0 Then
			'右利き用
			RButtonID = &H2
			LButtonID = &H1
		Else
			'左利き用
			RButtonID = &H1
			LButtonID = &H2
		End If
		
		
		ReDim ListItemComment(0)
		
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Data\System\alias.txt") Then
			ALDList.Load(ScenarioPath & "Data\System\alias.txt")
		ElseIf FileExists(AppPath & "Data\System\alias.txt") Then 
			ALDList.Load(AppPath & "Data\System\alias.txt")
		End If
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Data\System\sp.txt") Then
			SPDList.Load(ScenarioPath & "Data\System\sp.txt")
		ElseIf FileExists(ScenarioPath & "Data\System\mind.txt") Then 
			SPDList.Load(ScenarioPath & "Data\System\mind.txt")
		ElseIf FileExists(AppPath & "Data\System\sp.txt") Then 
			SPDList.Load(AppPath & "Data\System\sp.txt")
		ElseIf FileExists(AppPath & "Data\System\mind.txt") Then 
			SPDList.Load(AppPath & "Data\System\mind.txt")
		End If
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Data\System\item.txt") Then
			IDList.Load(ScenarioPath & "Data\System\item.txt")
		ElseIf FileExists(AppPath & "Data\System\item.txt") Then 
			IDList.Load(AppPath & "Data\System\item.txt")
		End If
		'Invalid_string_refer_to_original_code
		If FileExists(AppPath & "Data\System\terrain.txt") Then
			TDList.Load(AppPath & "Data\System\terrain.txt")
		Else
			ErrorMessage("Invalid_string_refer_to_original_code")
			TerminateSRC()
		End If
		If FileExists(ScenarioPath & "Data\System\terrain.txt") Then
			TDList.Load(ScenarioPath & "Data\System\terrain.txt")
		End If
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Data\System\battle.txt") Then
			BCList.Load(ScenarioPath & "Data\System\battle.txt")
		ElseIf FileExists(AppPath & "Data\System\battle.txt") Then 
			BCList.Load(AppPath & "Data\System\battle.txt")
		End If
		
		'Invalid_string_refer_to_original_code
		InitMap()
		
		'Invalid_string_refer_to_original_code
		RndSeed = Int(1000000 * Rnd())
		RndReset()
		
		If LCase(Right(fname, 4)) = ".src" Then
			SaveDataFileNumber = FreeFile
			FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
			'Invalid_string_refer_to_original_code
			Input(SaveDataFileNumber, buf)
			'Invalid_string_refer_to_original_code
			If IsNumeric(buf) Then
				If CInt(buf) > 10000 Then
					'Invalid_string_refer_to_original_code
					Input(SaveDataFileNumber, buf)
				End If
			End If
			FileClose(SaveDataFileNumber)
			
			'Invalid_string_refer_to_original_code
			If IsNumeric(buf) Then
				'セーブデータの読み込み
				OpenNowLoadingForm()
				LoadData(fname)
				CloseNowLoadingForm()
				
				'インターミッション
				InterMissionCommand(True)
				
				If Not IsSubStage Then
					If GetValueAsString("Invalid_string_refer_to_original_code") = "" Then
						ErrorMessage("Invalid_string_refer_to_original_code")
						TerminateSRC()
					End If
					
					StartScenario(GetValueAsString("Invalid_string_refer_to_original_code"))
				Else
					IsSubStage = False
				End If
			Else
				'Invalid_string_refer_to_original_code
				LockGUI()
				
				RestoreData(fname, False)
				
				'Invalid_string_refer_to_original_code
				RedrawScreen()
				DisplayGlobalStatus()
				
				UnlockGUI()
			End If
		ElseIf LCase(Right(fname, 4)) = ".eve" Then 
			'Invalid_string_refer_to_original_code
			StartScenario(fname)
		Else
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			TerminateSRC()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CreateIniFile()
		Dim f As Short
		
		On Error GoTo ErrorHandler
		
		f = FreeFile
		FileOpen(f, AppPath & "Src.ini", OpenMode.Output, OpenAccess.Write)
		
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "")
		PrintLine(f, "[Option]")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MessageWait=700")
		PrintLine(f, "")
		PrintLine(f, ";ターン数の表示 [On|Off]")
		PrintLine(f, "Turn=Off")
		PrintLine(f, "")
		PrintLine(f, ";マス目の表示 [On|Off]")
		PrintLine(f, "Square=Off")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "KeepEnemyBGM=Off")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "AutoDefense=Off")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "AutoMoveCursor=On")
		PrintLine(f, "")
		PrintLine(f, ";スペシャルパワーアニメ [On|Off]")
		PrintLine(f, "SpecialPowerAnimation=On")
		PrintLine(f, "")
		PrintLine(f, ";戦闘アニメ [On|Off]")
		PrintLine(f, "BattleAnimation=On")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "ExtendedAnimation=On")
		PrintLine(f, "")
		PrintLine(f, ";武器準備アニメの自動選択表示 [On|Off]")
		PrintLine(f, "WeaponAnimation=On")
		PrintLine(f, "")
		PrintLine(f, ";移動アニメ [On|Off]")
		PrintLine(f, "MoveAnimation=On")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MidiReset=None")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		If GetWinVersion() >= 500 Then
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			InitDirectMusic()
			'Invalid_string_refer_to_original_code
			If UseDirectMusic Then
				PrintLine(f, "UseDirectMusic=On")
			Else
				PrintLine(f, "UseDirectMusic=Off")
			End If
		Else
			'Invalid_string_refer_to_original_code
			UseMCI = True
			PrintLine(f, "UseDirectMusic=Off")
		End If
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MIDIPortID=0")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MP3Volume=50")
		PrintLine(f, "")
		PrintLine(f, ";MP3の出力フレーム数")
		PrintLine(f, "MP3OutputBlock=20")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MP3IutputSleep=5")
		PrintLine(f, "")
		'Invalid_string_refer_to_original_code
		'    Print #f, "UseDirectSound=On"
		'    Print #f, ""
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "ImageBufferNum=64")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "MaxImageBufferSize=8")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "KeepStretchedImage=")
		PrintLine(f, "")
		If GetWinVersion() >= 500 Then
			PrintLine(f, "Invalid_string_refer_to_original_code")
			PrintLine(f, "UseTransparentBlt=On")
			PrintLine(f, "")
		End If
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "ExtDataPath=")
		PrintLine(f, "ExtDataPath2=")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "DebugMode=Off")
		PrintLine(f, "")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "NewGUI=Off")
		PrintLine(f, "")
		PrintLine(f, "[Log]")
		PrintLine(f, ";前回使用したフォルダ")
		PrintLine(f, "LastFolder=")
		PrintLine(f, "")
		PrintLine(f, "[BGM]")
		PrintLine(f, ";SRC起動時")
		PrintLine(f, "Opening=Opening.mid")
		PrintLine(f, ";味方フェイズ開始時")
		PrintLine(f, "Map1=Map1.mid")
		PrintLine(f, ";敵フェイズ開始時")
		PrintLine(f, "Map2=Map2.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Map3=Map3.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Map4=Map4.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Map5=Map5.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Map6=Map6.mid")
		PrintLine(f, ";プロローグ・エピローグ開始時")
		PrintLine(f, "Briefing=Briefing.mid")
		PrintLine(f, ";インターミッション開始時")
		PrintLine(f, "Intermission=Intermission.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "Subtitle=Subtitle.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "End=End.mid")
		PrintLine(f, "Invalid_string_refer_to_original_code")
		PrintLine(f, "default=default.mid")
		PrintLine(f, "")
		
		FileClose(f)
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
	End Sub
	
	'KeepStretchedImageを使用すべきか決定するため、BitBltと
	'Invalid_string_refer_to_original_code
	Private Function IsBitBltFasterThanStretchBlt() As Object
		Dim stime, etime As Integer
		Dim bb_time, sb_time As Integer
		Dim ret As Integer
		Dim i As Short
		
		With MainForm
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			With .picStretchedTmp(0)
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.width = 128
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Height = 128
			End With
			'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			With .picStretchedTmp(1)
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.width = 128
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Height = 128
			End With
			
			'Invalid_string_refer_to_original_code
			stime = timeGetTime()
			For i = 1 To 5
				'UPGRADE_ISSUE: Control picUnit �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				ret = StretchBlt(.picStretchedTmp(0).hDC, 0, 0, 480, 480, .picUnit.hDC, 0, 0, 32, 32, SRCCOPY)
			Next 
			etime = timeGetTime()
			sb_time = etime - stime
			
			'Invalid_string_refer_to_original_code
			stime = timeGetTime()
			For i = 1 To 5
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				ret = GUI.BitBlt(.picStretchedTmp(1).hDC, 0, 0, 480, 480, .picStretchedTmp(0).hDC, 0, 0, SRCCOPY)
			Next 
			etime = timeGetTime()
			bb_time = etime - stime
			
			'描画領域を開放
			'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			With .picStretchedTmp(0)
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Picture = Nothing
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.width = 32
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Height = 32
			End With
			'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			With .picStretchedTmp(1)
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Picture = Nothing
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.width = 32
				'UPGRADE_ISSUE: Control picStretchedTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Height = 32
			End With
		End With
		
		'Invalid_string_refer_to_original_code
		If 2 * bb_time < sb_time Then
			'UPGRADE_WARNING: �I�u�W�F�N�g IsBitBltFasterThanStretchBlt �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			IsBitBltFasterThanStretchBlt = True
		Else
			'UPGRADE_WARNING: �I�u�W�F�N�g IsBitBltFasterThanStretchBlt �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			IsBitBltFasterThanStretchBlt = False
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub StartScenario(ByVal fname As String)
		Dim i As Short
		Dim ret As Integer
		Dim sf As System.Drawing.Font
		
		'ファイルを検索
		If Len(fname) = 0 Then
			TerminateSRC()
			End
		ElseIf FileExists(ScenarioPath & fname) Then 
			fname = ScenarioPath & fname
		ElseIf FileExists(AppPath & fname) Then 
			fname = AppPath & fname
		End If
		
		'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
		If Dir(fname, FileAttribute.Normal) = "" Then
			MsgBox(fname & "が見つかりません")
			TerminateSRC()
		End If
		
		'Invalid_string_refer_to_original_code
		If My.Application.Info.Version.Minor Mod 2 = 0 Then
			MainForm.Text = "SRC"
		Else
			MainForm.Text = "Invalid_string_refer_to_original_code"
		End If
		
		ScenarioFileName = fname
		
		If Not IsSubStage Then
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ScenarioPath & "Date", FileAttribute.Directory)) > 0 Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				& ScenarioPath & "Date" & vbCr & vbLf _
				Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				TerminateSRC()
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			TerminateSRC()
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		TerminateSRC()
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		TerminateSRC()
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		TerminateSRC()
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		TerminateSRC()
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not IsGlobalVariableDefined("Invalid_string_refer_to_original_code") Then
			DefineGlobalVariable("Invalid_string_refer_to_original_code")
		End If
		SetVariableAsString("Invalid_string_refer_to_original_code", "")
		For i = 1 To Len(fname)
			If Mid(fname, Len(fname) - i + 1, 1) = "\" Then
				Exit For
			End If
		Next 
		SetVariableAsString("Invalid_string_refer_to_original_code", Mid(fname, Len(fname) - i + 2))
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		DefineGlobalVariable("Invalid_string_refer_to_original_code")
		'End If
		SetVariableAsString("Invalid_string_refer_to_original_code")
		Mid$(fname, Len(fname) - i + 2, i - 5) & "までクリア.src"
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		'ウィンドウのタイトルにシナリオファイル名を表示
		MainForm.Text = MainForm.Text & " - " & Mid(fname, Len(fname) - i + 2, i - 5)
		'End If
		
		'画面をクリアしておく
		With MainForm
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ret = PatBlt(.picMain(0).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ret = PatBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
		End With
		ScreenIsSaved = True
		
		'イベントデータの読み込み
		LoadEventData(fname)
		
		'Invalid_string_refer_to_original_code
		Turn = 0
		IsScenarioFinished = False
		IsPictureVisible = False
		IsCursorVisible = False
		LastSaveDataFileName = ""
		IsRestartSaveDataAvailable = False
		IsQuickSaveDataAvailable = False
		CommandState = "Invalid_string_refer_to_original_code"
		ReDim SelectedPartners(0)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.ForeColor = RGB(255, 255, 255)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			If .Font.Name <> "Invalid_string_refer_to_original_code" Then
				sf = System.Windows.Forms.Control.DefaultFont.Clone()
				'UPGRADE_WARNING: Windows �t�H�[���ł́ATrueType ����� OpenType �t�H���g�݂̂��T�|�[�g����܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' ���N���b�N���Ă��������B
				sf = VB6.FontChangeName(sf, "Invalid_string_refer_to_original_code")
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Font = sf
			End If
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Font.Size = 16
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Font.Bold = True
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Font.Italic = False
			PermanentStringMode = False
			KeepStringMode = False
		End With
		
		'Invalid_string_refer_to_original_code
		ResetBasePoint()
		
		'Invalid_string_refer_to_original_code
		If Not IsSubStage Then
			UList.ClearUnitBitmap()
		End If
		
		LockGUI()
		
		If MapWidth = 1 Then
			SetMapSize(15, 15)
		End If
		
		'プロローグ
		Stage = "プロローグ"
		If Not IsSubStage And IsEventDefined("プロローグ", True) Then
			StopBGM()
			StartBGM(BGMName("Briefing"))
		End If
		HandleEvent("プロローグ")
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ErrorMessage("Invalid_string_refer_to_original_code")
		TerminateSRC()
		'End If
		
		IsPictureVisible = False
		IsCursorVisible = False
		Stage = "味方"
		StopBGM()
		
		'Invalid_string_refer_to_original_code
		If InStr(fname, "Invalid_string_refer_to_original_code") = 0 And InStr(fname, "Invalid_string_refer_to_original_code") = 0 Then
			DumpData(ScenarioPath & "Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		IsSubStage = False
		
		ClearUnitStatus()
		If Not MainForm.Visible Then
			MainForm.Show()
			MainForm.Refresh()
		End If
		RedrawScreen()
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			UnlockGUI()
			Exit Sub
		End If
		
		IsPictureVisible = False
		IsCursorVisible = False
		
		'Invalid_string_refer_to_original_code
		IsQuickSaveDataAvailable = False
		
		StartTurn("味方")
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub StartTurn(ByRef uparty As String)
		Dim num, i, phase As Short
		Dim u As Unit
		
		Stage = uparty
		BossBGM = False
		
		If uparty = "味方" Then
			Do 
				'味方フェイズ
				Stage = "味方"
				
				'ターン数を進める
				If MapFileName <> "" Then
					Turn = Turn + 1
					TotalTurn = TotalTurn + 1
				End If
				
				'状態回復
				For	Each SelectedUnit In UList
					With SelectedUnit
						Select Case .Status
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								If .Party = uparty Then
									If MapFileName = "" Then
										.UsedAction = 0
									Else
										.Rest()
									End If
									If IsScenarioFinished Then
										UnlockGUI()
										Exit Sub
									End If
								Else
									.UsedAction = 0
								End If
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								.UsedAction = 0
						End Select
					End With
				Next SelectedUnit
				
				'味方が敵にかけたスペシャルパワーを解除
				For	Each u In UList
					With u
						Select Case .Status
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								.RemoveSpecialPowerInEffect("敵ターン")
						End Select
					End With
				Next u
				RedrawScreen()
				
				'Invalid_string_refer_to_original_code
				If MapFileName <> "" Then
					Select Case TerrainClass(1, 1)
						Case "Invalid_string_refer_to_original_code"
							StartBGM(BGMName("Map3"))
						Case "Invalid_string_refer_to_original_code"
							StartBGM(BGMName("Map5"))
						Case Else
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							StartBGM(BGMName("Map3"))
					End Select
				Else
					StartBGM(BGMName("Map1"))
				End If
				'End Select
			Loop 
		End If
		
		'Invalid_string_refer_to_original_code
		IsUnitCenter = False
		HandleEvent("ターン", Turn, "味方")
		If IsScenarioFinished Then
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		num = 0
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code_
				'And .Action > 0 _
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				num = num + 1
				'End If
			End With
		Next u
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Exit Do
		'End If
		
		'Invalid_string_refer_to_original_code
		num = 0
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				num = num + 1
				'End If
			End With
		Next u
		If num = 0 Then
			Exit Do
		End If
		
		'敵フェイズ
		StartTurn("敵")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		'中立フェイズ
		StartTurn("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		StartTurn("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		'UPGRADE_WARNING: StartTurn �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'Loop
		Dim max_lv As Short
		Dim max_unit As Unit
		'Invalid_string_refer_to_original_code
		
		'状態回復
		For	Each SelectedUnit In UList
			With SelectedUnit
				Select Case .Status
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If .Party = uparty Then
							.Rest()
							If IsScenarioFinished Then
								UnlockGUI()
								Exit Sub
							End If
						Else
							.UsedAction = 0
						End If
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.UsedAction = 0
				End Select
			End With
		Next SelectedUnit
		
		'敵ユニットが味方にかけたスペシャルパワーを解除
		For	Each u In UList
			With u
				Select Case .Status
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.RemoveSpecialPowerInEffect("敵ターン")
				End Select
			End With
		Next u
		RedrawScreen()
		
		'Invalid_string_refer_to_original_code
		Select Case TerrainClass(1, 1)
			Case "Invalid_string_refer_to_original_code"
				If Stage = "Invalid_string_refer_to_original_code" Then
					StartBGM(BGMName("Map3"))
				Else
					StartBGM(BGMName("Map4"))
				End If
			Case "Invalid_string_refer_to_original_code"
				If Stage = "Invalid_string_refer_to_original_code" Then
					StartBGM(BGMName("Map5"))
				Else
					StartBGM(BGMName("Map6"))
				End If
			Case Else
				If Stage = "Invalid_string_refer_to_original_code" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					StartBGM(BGMName("Map3"))
				Else
					StartBGM(BGMName("Map1"))
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				StartBGM(BGMName("Map4"))
				StartBGM(BGMName("Map2"))
				'End If
				'End If
		End Select
		
		'Invalid_string_refer_to_original_code
		HandleEvent("ターン", Turn, uparty)
		If IsScenarioFinished Then
			UnlockGUI()
			Exit Sub
		End If
		'End If
		
		If uparty = "味方" Then
			'Invalid_string_refer_to_original_code
			
			'ターン数を表示
			If Turn > 1 And IsOptionDefined("Invalid_string_refer_to_original_code") Then
				DisplayTelop("ターン" & VB6.Format(Turn))
			End If
			
			'Invalid_string_refer_to_original_code
			'ユニットを中央に配置
			If MapFileName <> "" And Not IsUnitCenter Then
				
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If .IsFeatureAvailable("母艦") Then
							Center(.X, .Y)
							DisplayUnitStatus(u)
							RedrawScreen()
							UnlockGUI()
							Exit Sub
						End If
					End With
				Next u
			End If
			'End With
			'Next
			
			max_lv = 0
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					If .MainPilot.Level > max_lv Then
						max_unit = u
						max_lv = .MainPilot.Level
					End If
				End With
			Next u
		End If
		'End With
		'Next
		If Not max_unit Is Nothing Then
			Center(max_unit.X, max_unit.Y)
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If MapFileName <> "" Then
			DisplayGlobalStatus()
		End If
		
		'Invalid_string_refer_to_original_code
		RedrawScreen()
		System.Windows.Forms.Application.DoEvents()
		UnlockGUI()
		Exit Sub
		'End If
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code
		For phase = 1 To 5
			For i = 1 To UList.Count
				'Invalid_string_refer_to_original_code
				SelectedUnit = UList.Item(i)
				
				With SelectedUnit
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					GoTo NextLoop
					'End If
					
					If .Action = 0 Then
						GoTo NextLoop
					End If
					
					If .Party <> uparty Then
						GoTo NextLoop
					End If
					
					u = SelectedUnit
					
					'Invalid_string_refer_to_original_code
					If PList.IsDefined(.Mode) Then
						With PList.Item(.Mode)
							If Not .Unit Is Nothing Then
								If .Unit.Party = uparty Then
									u = .Unit
								End If
							End If
						End With
					End If
					If PList.IsDefined((u.Mode)) Then
						With PList.Item((u.Mode))
							If Not .Unit Is Nothing Then
								If .Unit.Party = uparty Then
									u = .Unit
								End If
							End If
						End With
					End If
					
					With u
						Select Case phase
							Case 1
								'Invalid_string_refer_to_original_code
								If .BossRank >= 0 Then
									GoTo NextLoop
								End If
								With .MainPilot
									'Invalid_string_refer_to_original_code_
									'Or .IsSkillAvailable("援護防御") _
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
									GoTo NextLoop
									'End If
								End With
							Case 2
								'Invalid_string_refer_to_original_code
								With .MainPilot
									'Invalid_string_refer_to_original_code_
									'Or .IsSkillAvailable("援護防御") _
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
									GoTo NextLoop
									'End If
								End With
							Case 3
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								GoTo NextLoop
								'End If
							Case 4
								'Invalid_string_refer_to_original_code
								If .BossRank >= 0 Then
									GoTo NextLoop
								End If
							Case 5
								'Invalid_string_refer_to_original_code
						End Select
					End With
				End With
				
				Do While SelectedUnit.Action > 0
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Exit Do
					'End If
					
					'Invalid_string_refer_to_original_code
					If SelectedUnit.Party <> uparty Then
						Exit Do
					End If
					
					If Not IsRButtonPressed Then
						DisplayUnitStatus(SelectedUnit)
						Center(SelectedUnit.X, SelectedUnit.Y)
						RedrawScreen()
						System.Windows.Forms.Application.DoEvents()
					End If
					
					IsCanceled = False 'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
					OperateUnit()
					
					If IsScenarioFinished Then
						Exit Sub
					End If
					
					'Invalid_string_refer_to_original_code
					UList.CheckAutoHyperMode()
					UList.CheckAutoNormalMode()
					
					'Invalid_string_refer_to_original_code
					If IsCanceled Then
						If SelectedUnit Is Nothing Then
							Exit Do
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Exit Do
					End If
					IsCanceled = False
					'End If
					
					'Invalid_string_refer_to_original_code
					SelectedUnit.UseAction()
					
					'Invalid_string_refer_to_original_code
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Not MapDataForUnit(.X - 1, .Y) Is Nothing Then
							SelectedTarget = MapDataForUnit(.X - 1, .Y)
							HandleEvent("接触", .MainPilot.ID, MapDataForUnit(.X - 1, .Y).MainPilot.ID)
							If IsScenarioFinished Then
								Exit Sub
							End If
						End If
						'End If
					End With
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Not MapDataForUnit(.X + 1, .Y) Is Nothing Then
							SelectedTarget = MapDataForUnit(.X + 1, .Y)
							HandleEvent("接触", .MainPilot.ID, MapDataForUnit(.X + 1, .Y).MainPilot.ID)
							If IsScenarioFinished Then
								Exit Sub
							End If
						End If
						'End If
					End With
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Not MapDataForUnit(.X, .Y - 1) Is Nothing Then
							SelectedTarget = MapDataForUnit(.X, .Y - 1)
							HandleEvent("接触", .MainPilot.ID, MapDataForUnit(.X, .Y - 1).MainPilot.ID)
							If IsScenarioFinished Then
								Exit Sub
							End If
						End If
						'End If
					End With
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Not MapDataForUnit(.X, .Y + 1) Is Nothing Then
							SelectedTarget = MapDataForUnit(.X, .Y + 1)
							HandleEvent("接触", .MainPilot.ID, MapDataForUnit(.X, .Y + 1).MainPilot.ID)
							If IsScenarioFinished Then
								Exit Sub
							End If
						End If
						'End If
					End With
					
					'Invalid_string_refer_to_original_code
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						HandleEvent("進入", .MainPilot.ID, .X, .Y)
						If IsScenarioFinished Then
							Exit Sub
						End If
						'End If
					End With
					
					'Invalid_string_refer_to_original_code
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						HandleEvent("Invalid_string_refer_to_original_code")
						If IsScenarioFinished Then
							Exit Sub
						End If
						'End If
					End With
				Loop 
NextLoop: 
			Next 
		Next 
		
		'Invalid_string_refer_to_original_code
		ClearUnitStatus()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub GameOver()
		Dim fname As String
		
		KeepBGM = False
		BossBGM = False
		StopBGM()
		MainForm.Hide()
		
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Data\System\GameOver.eve") Then
			fname = ScenarioPath & "Data\System\GameOver.eve"
			If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
				NPDList.Load(ScenarioPath & "Data\System\non_pilot.txt")
			End If
		ElseIf FileExists(AppPath & "Data\System\GameOver.eve") Then 
			fname = AppPath & "Data\System\GameOver.eve"
			If FileExists(AppPath & "Data\System\non_pilot.txt") Then
				NPDList.Load(AppPath & "Data\System\non_pilot.txt")
			End If
		Else
			'Invalid_string_refer_to_original_code
			TerminateSRC()
		End If
		
		'GameOver.eveを読み込み
		ClearEventData()
		LoadEventData(fname)
		ScenarioFileName = fname
		
		If Not IsEventDefined("プロローグ") Then
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
			TerminateSRC()
		End If
		
		'GameOver.eveのプロローグイベントを実施
		HandleEvent("プロローグ")
	End Sub
	
	'ゲームクリア
	Public Sub GameClear()
		TerminateSRC()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ExitGame()
		Dim fname As String
		
		KeepBGM = False
		BossBGM = False
		StopBGM()
		
		'Invalid_string_refer_to_original_code
		MainForm.Hide()
		If FileExists(ScenarioPath & "Data\System\Exit.eve") Then
			fname = ScenarioPath & "Data\System\Exit.eve"
			If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
				NPDList.Load(ScenarioPath & "Data\System\non_pilot.txt")
			End If
		ElseIf FileExists(AppPath & "Data\System\Exit.eve") Then 
			fname = AppPath & "Data\System\Exit.eve"
			If FileExists(AppPath & "Data\System\non_pilot.txt") Then
				NPDList.Load(AppPath & "Data\System\non_pilot.txt")
			End If
		Else
			'Invalid_string_refer_to_original_code
			TerminateSRC()
		End If
		
		'Exit.eveを読み込み
		ClearEventData()
		LoadEventData(fname)
		
		If Not IsEventDefined("プロローグ") Then
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
			TerminateSRC()
		End If
		
		'Exit.eveのプロローグイベントを実施
		HandleEvent("プロローグ")
		
		'Invalid_string_refer_to_original_code
		TerminateSRC()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub TerminateSRC()
		Dim i, j As Short
		
		'ウィンドウを閉じる
		If Not MainForm Is Nothing Then
			MainForm.Hide()
		End If
		'UPGRADE_ISSUE: Load �X�e�[�g�����g �̓T�|�[�g����Ă��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' ���N���b�N���Ă��������B
		Load(frmMessage)
		If frmMessage.Visible Then
			CloseMessageForm()
		End If
		'UPGRADE_ISSUE: Load �X�e�[�g�����g �̓T�|�[�g����Ă��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' ���N���b�N���Ă��������B
		Load(frmListBox)
		If frmListBox.Visible Then
			frmListBox.Hide()
		End If
		'UPGRADE_ISSUE: Load �X�e�[�g�����g �̓T�|�[�g����Ă��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' ���N���b�N���Ă��������B
		Load(frmNowLoading)
		If frmNowLoading.Visible Then
			frmNowLoading.Hide()
		End If
		System.Windows.Forms.Application.DoEvents()
		
		'Invalid_string_refer_to_original_code
		Call timeEndPeriod(1)
		
		'Invalid_string_refer_to_original_code
		If ReadIni("Option", "FullScreen") = "On" Then
			ChangeDisplaySize(0, 0)
		End If
		
		'Invalid_string_refer_to_original_code
		FreeSoundModule()
		
		'Invalid_string_refer_to_original_code
		
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedPilot = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g DisplayedUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		DisplayedUnit = Nothing
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		
		With GlobalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g GlobalVariableList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		GlobalVariableList = Nothing
		
		With LocalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g LocalVariableList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		LocalVariableList = Nothing
		
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedUnitForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedUnitForEvent = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		
		'UPGRADE_NOTE: �I�u�W�F�N�g UList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		UList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g PList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		PList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g IList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		IList = Nothing
		
		'Invalid_string_refer_to_original_code
		System.Windows.Forms.Application.DoEvents()
		
		'UPGRADE_NOTE: �I�u�W�F�N�g PDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		PDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g NPDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		NPDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g UDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		UDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g IDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		IDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g MDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		MDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g EDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		EDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g ADList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		ADList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g EADList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		EADList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g DDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		DDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SPDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SPDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g ALDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		ALDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g TDList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		TDList = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g BCList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		BCList = Nothing
		
		End
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveData(ByRef fname As String)
		Dim i As Short
		Dim num As Integer
		
		On Error GoTo ErrorHandler
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write)
		
		'UPGRADE_ISSUE: App �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		With App
			num = 10000 * My.Application.Info.Version.Major + 100 * My.Application.Info.Version.Minor + My.Application.Info.Version.Revision
		End With
		WriteLine(SaveDataFileNumber, num)
		
		WriteLine(SaveDataFileNumber, UBound(Titles))
		For i = 1 To UBound(Titles)
			WriteLine(SaveDataFileNumber, Titles(i))
		Next 
		
		WriteLine(SaveDataFileNumber, GetValueAsString("Invalid_string_refer_to_original_code"))
		
		WriteLine(SaveDataFileNumber, TotalTurn)
		WriteLine(SaveDataFileNumber, Money)
		WriteLine(SaveDataFileNumber, 0) 'Invalid_string_refer_to_original_code
		
		SaveGlobalVariables()
		PList.Save()
		UList.Save()
		IList.Save()
		
		FileClose(SaveDataFileNumber)
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("セーブ中にエラーが発生しました")
		FileClose(SaveDataFileNumber)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadData(ByRef fname As String)
		Dim i, num As Short
		Dim fname2 As String
		Dim dummy As String
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
		
		Input(SaveDataFileNumber, SaveDataVersion)
		
		If SaveDataVersion > 10000 Then
			Input(SaveDataFileNumber, num)
		Else
			num = SaveDataVersion
		End If
		
		SetLoadImageSize(num * 2 + 5)
		
		ReDim Titles(num)
		For i = 1 To num
			Input(SaveDataFileNumber, Titles(i))
			IncludeData(Titles(i))
		Next 
		
		If FileExists(ScenarioPath & "Data\alias.txt") Then
			ALDList.Load(ScenarioPath & "Data\alias.txt")
		End If
		If FileExists(ScenarioPath & "Data\sp.txt") Then
			SPDList.Load(ScenarioPath & "Data\sp.txt")
		ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then 
			SPDList.Load(ScenarioPath & "Data\mind.txt")
		End If
		If FileExists(ScenarioPath & "Data\pilot.txt") Then
			PDList.Load(ScenarioPath & "Data\pilot.txt")
		End If
		If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
			NPDList.Load(ScenarioPath & "Data\non_pilot.txt")
		End If
		If FileExists(ScenarioPath & "Data\robot.txt") Then
			UDList.Load(ScenarioPath & "Data\robot.txt")
		End If
		If FileExists(ScenarioPath & "Data\unit.txt") Then
			UDList.Load(ScenarioPath & "Data\unit.txt")
		End If
		
		DisplayLoadingProgress()
		
		If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
			MDList.Load(ScenarioPath & "Data\pilot_message.txt")
		End If
		If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
			DDList.Load(ScenarioPath & "Data\pilot_dialog.txt")
		End If
		If FileExists(ScenarioPath & "Data\effect.txt") Then
			EDList.Load(ScenarioPath & "Data\effect.txt")
		End If
		If FileExists(ScenarioPath & "Data\animation.txt") Then
			ADList.Load(ScenarioPath & "Data\animation.txt")
		End If
		If FileExists(ScenarioPath & "Data\ext_animation.txt") Then
			EADList.Load(ScenarioPath & "Data\ext_animation.txt")
		End If
		If FileExists(ScenarioPath & "Data\item.txt") Then
			IDList.Load(ScenarioPath & "Data\item.txt")
		End If
		
		DisplayLoadingProgress()
		
		IsLocalDataLoaded = True
		
		Input(SaveDataFileNumber, fname2)
		Input(SaveDataFileNumber, TotalTurn)
		Input(SaveDataFileNumber, Money)
		Input(SaveDataFileNumber, num) 'Invalid_string_refer_to_original_code
		
		LoadGlobalVariables()
		If Not IsGlobalVariableDefined("Invalid_string_refer_to_original_code") Then
			DefineGlobalVariable("Invalid_string_refer_to_original_code")
		End If
		SetVariableAsString("Invalid_string_refer_to_original_code", fname2)
		
		PList.Load()
		UList.Load()
		IList.Load()
		
		FileClose(SaveDataFileNumber)
		
		'Invalid_string_refer_to_original_code
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
		
		If SaveDataVersion > 10000 Then
			Input(SaveDataFileNumber, dummy)
		End If
		Input(SaveDataFileNumber, num)
		ReDim Titles(num)
		For i = 1 To num
			Input(SaveDataFileNumber, Titles(i))
		Next 
		Input(SaveDataFileNumber, dummy)
		Input(SaveDataFileNumber, TotalTurn)
		Input(SaveDataFileNumber, Money)
		Input(SaveDataFileNumber, num) 'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num) 'Invalid_string_refer_to_original_code
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		PList.LoadLinkInfo()
		UList.LoadLinkInfo()
		IList.LoadLinkInfo()
		
		FileClose(SaveDataFileNumber)
		
		DisplayLoadingProgress()
		
		'Invalid_string_refer_to_original_code
		For	Each u In UList
			u.Reset_Renamed()
		Next u
		
		DisplayLoadingProgress()
		
		'Invalid_string_refer_to_original_code
		LoadEventData("")
		
		DisplayLoadingProgress()
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("ロード中にエラーが発生しました")
		FileClose(SaveDataFileNumber)
		TerminateSRC()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub DumpData(ByRef fname As String)
		Dim i As Short
		Dim num As Integer
		
		On Error GoTo ErrorHandler
		
		'Invalid_string_refer_to_original_code
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write)
		
		'UPGRADE_ISSUE: App �I�u�W�F�N�g �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ���N���b�N���Ă��������B
		With App
			num = 10000 * My.Application.Info.Version.Major + 100 * My.Application.Info.Version.Minor + My.Application.Info.Version.Revision
		End With
		WriteLine(SaveDataFileNumber, num)
		
		WriteLine(SaveDataFileNumber, Mid(ScenarioFileName, Len(ScenarioPath) + 1))
		
		WriteLine(SaveDataFileNumber, UBound(Titles))
		For i = 1 To UBound(Titles)
			WriteLine(SaveDataFileNumber, Titles(i))
		Next 
		
		WriteLine(SaveDataFileNumber, Turn)
		WriteLine(SaveDataFileNumber, TotalTurn)
		WriteLine(SaveDataFileNumber, Money)
		
		DumpEventData()
		
		PList.Dump()
		IList.Dump()
		UList.Dump()
		
		DumpMapData()
		
		'Invalid_string_refer_to_original_code
		If InStr(LCase(BGMFileName), "\midi\") > 0 Then
			WriteLine(SaveDataFileNumber, Mid(BGMFileName, InStr(LCase(BGMFileName), "\midi\") + 6))
		ElseIf InStr(BGMFileName, "\") > 0 Then 
			WriteLine(SaveDataFileNumber, Mid(BGMFileName, InStr(BGMFileName, "\") + 1))
		Else
			WriteLine(SaveDataFileNumber, BGMFileName)
		End If
		WriteLine(SaveDataFileNumber, RepeatMode)
		WriteLine(SaveDataFileNumber, KeepBGM)
		WriteLine(SaveDataFileNumber, BossBGM)
		
		WriteLine(SaveDataFileNumber, RndSeed)
		WriteLine(SaveDataFileNumber, RndIndex)
		
		FileClose(SaveDataFileNumber)
		
		LastSaveDataFileName = fname
		If InStr(fname, "Invalid_string_refer_to_original_code") > 0 Then
			IsRestartSaveDataAvailable = True
		ElseIf InStr(fname, "Invalid_string_refer_to_original_code") > 0 Then 
			IsQuickSaveDataAvailable = True
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("セーブ中にエラーが発生しました")
		FileClose(SaveDataFileNumber)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreData(ByRef fname As String, ByRef quick_load As Boolean)
		Dim i, num As Short
		Dim fname2 As String
		Dim dummy As String
		Dim u As Unit
		Dim scenario_file_is_different As Boolean
		
		On Error GoTo ErrorHandler
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		If quick_load Then
			If IsOptionDefined("Invalid_string_refer_to_original_code") Then
				LoadEventData(ScenarioFileName, "Invalid_string_refer_to_original_code")
			End If
		End If
		
		If Not quick_load Then
			OpenNowLoadingForm()
		End If
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
		
		Input(SaveDataFileNumber, fname2)
		
		If IsNumeric(fname2) Then
			SaveDataVersion = CInt(fname2)
			Input(SaveDataFileNumber, fname2)
		Else
			SaveDataVersion = 1
		End If
		
		'Invalid_string_refer_to_original_code
		If ScenarioFileName <> ScenarioPath & fname2 Then
			MainForm.Text = "SRC - " & Left(fname2, Len(fname2) - 4)
			ScenarioFileName = ScenarioPath & fname2
			scenario_file_is_different = True
		End If
		
		Input(SaveDataFileNumber, num)
		
		'Invalid_string_refer_to_original_code
		If Not quick_load Then
			SetLoadImageSize(num * 2 + 5)
			
			ReDim Titles(num)
			For i = 1 To num
				Input(SaveDataFileNumber, Titles(i))
				IncludeData(Titles(i))
			Next 
			
			If FileExists(ScenarioPath & "Data\alias.txt") Then
				ALDList.Load(ScenarioPath & "Data\alias.txt")
			End If
			If FileExists(ScenarioPath & "Data\sp.txt") Then
				SPDList.Load(ScenarioPath & "Data\sp.txt")
			ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then 
				SPDList.Load(ScenarioPath & "Data\mind.txt")
			End If
			If FileExists(ScenarioPath & "Data\pilot.txt") Then
				PDList.Load(ScenarioPath & "Data\pilot.txt")
			End If
			If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
				NPDList.Load(ScenarioPath & "Data\non_pilot.txt")
			End If
			If FileExists(ScenarioPath & "Data\robot.txt") Then
				UDList.Load(ScenarioPath & "Data\robot.txt")
			End If
			If FileExists(ScenarioPath & "Data\unit.txt") Then
				UDList.Load(ScenarioPath & "Data\unit.txt")
			End If
			
			DisplayLoadingProgress()
			
			If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
				MDList.Load(ScenarioPath & "Data\pilot_message.txt")
			End If
			If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
				DDList.Load(ScenarioPath & "Data\pilot_dialog.txt")
			End If
			If FileExists(ScenarioPath & "Data\effect.txt") Then
				EDList.Load(ScenarioPath & "Data\effect.txt")
			End If
			If FileExists(ScenarioPath & "Data\animation.txt") Then
				ADList.Load(ScenarioPath & "Data\animation.txt")
			End If
			If FileExists(ScenarioPath & "Data\ext_animation.txt") Then
				EADList.Load(ScenarioPath & "Data\ext_animation.txt")
			End If
			If FileExists(ScenarioPath & "Data\item.txt") Then
				IDList.Load(ScenarioPath & "Data\item.txt")
			End If
			
			DisplayLoadingProgress()
			IsLocalDataLoaded = True
			
			LoadEventData(ScenarioFileName, "リストア")
			
			DisplayLoadingProgress()
		Else
			For i = 1 To num
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			If scenario_file_is_different Then
				LoadEventData(ScenarioFileName, "リストア")
			End If
		End If
		
		Input(SaveDataFileNumber, Turn)
		Input(SaveDataFileNumber, TotalTurn)
		Input(SaveDataFileNumber, Money)
		
		RestoreEventData()
		
		PList.Restore()
		IList.Restore()
		UList.Restore()
		
		'MOD START 240a
		'    RestoreMapData
		'Invalid_string_refer_to_original_code
		'    Input #SaveDataFileNumber, fname2
		'Invalid_string_refer_to_original_code
		fname2 = RestoreMapData
		'MOD  END  240a
		fname2 = SearchMidiFile("(" & fname2 & ")")
		If fname2 <> "" Then
			KeepBGM = False
			BossBGM = False
			ChangeBGM(fname2)
			Input(SaveDataFileNumber, RepeatMode)
			Input(SaveDataFileNumber, KeepBGM)
			Input(SaveDataFileNumber, BossBGM)
		Else
			StopBGM()
			dummy = LineInput(SaveDataFileNumber)
			dummy = LineInput(SaveDataFileNumber)
			dummy = LineInput(SaveDataFileNumber)
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And Not EOF(SaveDataFileNumber) _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Input(SaveDataFileNumber, RndSeed)
		RndReset()
		Input(SaveDataFileNumber, RndIndex)
		'End If
		
		If Not quick_load Then
			DisplayLoadingProgress()
		End If
		
		FileClose(SaveDataFileNumber)
		
		'Invalid_string_refer_to_original_code
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
		
		'SaveDataVersion
		If SaveDataVersion > 10000 Then
			dummy = LineInput(SaveDataFileNumber)
		End If
		
		'ScenarioFileName
		dummy = LineInput(SaveDataFileNumber)
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		
		'Turn
		dummy = LineInput(SaveDataFileNumber)
		'TotalTurn
		dummy = LineInput(SaveDataFileNumber)
		'Money
		dummy = LineInput(SaveDataFileNumber)
		
		SkipEventData()
		
		PList.RestoreLinkInfo()
		IList.RestoreLinkInfo()
		UList.RestoreLinkInfo()
		
		FileClose(SaveDataFileNumber)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		
		SaveDataFileNumber = FreeFile
		FileOpen(SaveDataFileNumber, fname, OpenMode.Input)
		
		'SaveDataVersion
		If SaveDataVersion > 10000 Then
			dummy = LineInput(SaveDataFileNumber)
		End If
		
		'ScenarioFileName
		dummy = LineInput(SaveDataFileNumber)
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		
		'Turn
		dummy = LineInput(SaveDataFileNumber)
		'TotalTurn
		dummy = LineInput(SaveDataFileNumber)
		'Money
		dummy = LineInput(SaveDataFileNumber)
		
		SkipEventData()
		
		PList.RestoreParameter()
		IList.RestoreParameter()
		UList.RestoreParameter()
		
		PList.UpdateSupportMod()
		
		'Invalid_string_refer_to_original_code
		Dim map_x, map_y As Short
		If IsMapDirty Then
			
			map_x = MapX
			map_y = MapY
			
			SetupBackground(MapDrawMode, "Invalid_string_refer_to_original_code")
			
			MapX = map_x
			MapY = map_y
			
			'Invalid_string_refer_to_original_code
			HandleEvent("再開")
			
			IsMapDirty = False
		End If
		
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		
		'Invalid_string_refer_to_original_code
		For	Each u In UList
			With u
				If .BitmapID = 0 Then
					.BitmapID = MakeUnitBitmap(u)
				End If
			End With
		Next u
		
		'画面更新
		Center(MapX, MapY)
		
		FileClose(SaveDataFileNumber)
		
		If Not quick_load Then
			DisplayLoadingProgress()
		End If
		
		If Not quick_load Then
			CloseNowLoadingForm()
		End If
		
		If Not quick_load Then
			MainForm.Show()
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ClearUnitStatus()
		If Not MainForm.Visible Then
			MainForm.Show()
			MainForm.Refresh()
		End If
		RedrawScreen()
		
		If Turn = 0 Then
			HandleEvent("Invalid_string_refer_to_original_code")
			
			' MOD START MARGE
			'        StartTurn "味方"
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If Turn = 0 Then
				StartTurn("味方")
			End If
			' MOD END MARGE
		Else
			CommandState = "Invalid_string_refer_to_original_code"
			Stage = "味方"
		End If
		
		LastSaveDataFileName = fname
		If InStr(fname, "Invalid_string_refer_to_original_code") > 0 Then
			IsRestartSaveDataAvailable = True
		ElseIf InStr(fname, "Invalid_string_refer_to_original_code") > 0 Then 
			IsQuickSaveDataAvailable = True
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("ロード中にエラーが発生しました")
		FileClose(SaveDataFileNumber)
		TerminateSRC()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'旧形式）ユニット名称+数値
	'新形式）ユニット名称+":"+数値
	Public Sub ConvertUnitID(ByRef ID As String)
		Dim i As Short
		
		If InStr(ID, ":") > 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		i = Len(ID)
		Do While i > 0
			Select Case Asc(Mid(ID, i, 1))
				Case 48 To 57
					'0-9
				Case Else
					Exit Do
			End Select
			i = i - 1
		Loop 
		
		'Invalid_string_refer_to_original_code
		ID = Left(ID, i) & ":" & Mid(ID, i + 1)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub IncludeData(ByRef new_title As String)
		Dim fpath As String
		
		'Invalid_string_refer_to_original_code
		If frmNowLoading.Visible Then
			DisplayLoadingProgress()
		End If
		
		'Invalid_string_refer_to_original_code
		fpath = SearchDataFolder(new_title)
		
		If Len(fpath) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			TerminateSRC()
		End If
		
		On Error GoTo ErrorHandler
		
		If FileExists(fpath & "\alias.txt") Then
			ALDList.Load(fpath & "\alias.txt")
		End If
		If FileExists(fpath & "\sp.txt") Then
			SPDList.Load(fpath & "\sp.txt")
		ElseIf FileExists(fpath & "\mind.txt") Then 
			SPDList.Load(fpath & "\mind.txt")
		End If
		If FileExists(fpath & "\pilot.txt") Then
			PDList.Load(fpath & "\pilot.txt")
		End If
		If FileExists(fpath & "\non_pilot.txt") Then
			NPDList.Load(fpath & "\non_pilot.txt")
		End If
		If FileExists(fpath & "\robot.txt") Then
			UDList.Load(fpath & "\robot.txt")
		End If
		If FileExists(fpath & "\unit.txt") Then
			UDList.Load(fpath & "\unit.txt")
		End If
		
		'Invalid_string_refer_to_original_code
		If frmNowLoading.Visible Then
			DisplayLoadingProgress()
		End If
		
		If FileExists(fpath & "\pilot_message.txt") Then
			MDList.Load(fpath & "\pilot_message.txt")
		End If
		If FileExists(fpath & "\pilot_dialog.txt") Then
			DDList.Load(fpath & "\pilot_dialog.txt")
		End If
		If FileExists(fpath & "\effect.txt") Then
			EDList.Load(fpath & "\effect.txt")
		End If
		If FileExists(fpath & "\animation.txt") Then
			ADList.Load(fpath & "\animation.txt")
		End If
		If FileExists(fpath & "\ext_animation.txt") Then
			EADList.Load(fpath & "\ext_animation.txt")
		End If
		If FileExists(fpath & "\item.txt") Then
			IDList.Load(fpath & "\item.txt")
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("Invalid_string_refer_to_original_code")
		TerminateSRC()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function SearchDataFolder(ByRef fname As String) As String
		Dim fname2 As String
		Static init_search_data_folder As Boolean
		Static scenario_data_dir_exists As Boolean
		Static extdata_data_dir_exists As Boolean
		Static extdata2_data_dir_exists As Boolean
		Static src_data_dir_exists As Boolean
		
		'Invalid_string_refer_to_original_code
		If Not init_search_data_folder Then
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ScenarioPath & "Data", FileAttribute.Directory)) > 0 Then
				scenario_data_dir_exists = True
			End If
			If Len(ExtDataPath) > 0 And ScenarioPath <> ExtDataPath Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath & "Data", FileAttribute.Directory)) > 0 Then
					extdata_data_dir_exists = True
				End If
			End If
			If Len(ExtDataPath2) > 0 And ScenarioPath <> ExtDataPath2 Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(ExtDataPath2 & "Data", FileAttribute.Directory)) > 0 Then
					extdata2_data_dir_exists = True
				End If
			End If
			If ScenarioPath <> AppPath Then
				'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If Len(Dir(AppPath & "Data", FileAttribute.Directory)) > 0 Then
					src_data_dir_exists = True
				End If
			End If
			init_search_data_folder = True
		End If
		
		'フォルダを検索
		fname2 = "Data\" & fname
		If scenario_data_dir_exists Then
			SearchDataFolder = ScenarioPath & fname2
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(SearchDataFolder, FileAttribute.Directory)) > 0 Then
				Exit Function
			End If
		End If
		If extdata_data_dir_exists Then
			SearchDataFolder = ExtDataPath & fname2
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(SearchDataFolder, FileAttribute.Directory)) > 0 Then
				Exit Function
			End If
		End If
		If extdata2_data_dir_exists Then
			SearchDataFolder = ExtDataPath2 & fname2
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(SearchDataFolder, FileAttribute.Directory)) > 0 Then
				Exit Function
			End If
		End If
		If src_data_dir_exists Then
			SearchDataFolder = AppPath & fname2
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(SearchDataFolder, FileAttribute.Directory)) > 0 Then
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		SearchDataFolder = ""
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub IncrMoney(ByVal earnings As Integer)
		Money = MinLng(Money + earnings, 999999999)
		Money = MaxLng(Money, 0)
	End Sub
End Module