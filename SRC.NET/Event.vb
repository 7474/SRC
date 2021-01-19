Option Strict Off
Option Explicit On
'UPGRADE_NOTE: Event �� Event_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
Module Event_Renamed
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'イベントデータ
	Public EventData() As String
	'Invalid_string_refer_to_original_code
	Public EventCmd() As CmdData
	'Invalid_string_refer_to_original_code
	Public EventFileID() As Short
	'Invalid_string_refer_to_original_code
	Public EventLineNum() As Short
	'Invalid_string_refer_to_original_code
	Public EventFileNames() As String
	'Invalid_string_refer_to_original_code
	Public AdditionalEventFileNames() As String
	
	'Invalid_string_refer_to_original_code
	Private SysEventDataSize As Integer
	'Invalid_string_refer_to_original_code
	Private SysEventFileNum As Short
	'Invalid_string_refer_to_original_code
	Private ScenarioLibChecked As Boolean
	
	'Invalid_string_refer_to_original_code
	Public colEventLabelList As New Collection
	Private colSysNormalLabelList As New Collection
	Private colNormalLabelList As New Collection
	
	
	'変数用のコレクション
	Public GlobalVariableList As New Collection
	Public LocalVariableList As New Collection
	
	'現在の行番号
	Public CurrentLineNum As Integer
	
	'Invalid_string_refer_to_original_code
	Public SelectedUnitForEvent As Unit
	Public SelectedTargetForEvent As Unit
	
	'Invalid_string_refer_to_original_code
	Public EventQue() As String
	'現在実行中のイベントラベル
	Public CurrentLabel As Integer
	
	'Askコマンドで選択した選択肢
	Public SelectedAlternative As String
	
	'関数呼び出し用変数
	
	'最大呼び出し階層数
	Public Const MaxCallDepth As Short = 50
	'引数の最大数
	Public Const MaxArgIndex As Short = 200
	'サブルーチンローカル変数の最大数
	Public Const MaxVarIndex As Short = 2000
	
	'呼び出し履歴
	Public CallDepth As Short
	Public CallStack(MaxCallDepth) As Integer
	'Invalid_string_refer_to_original_code
	Public ArgIndex As Short
	Public ArgIndexStack(MaxCallDepth) As Short
	Public ArgStack(MaxArgIndex) As String
	'Invalid_string_refer_to_original_code
	Public UpVarLevel As Short
	Public UpVarLevelStack(MaxCallDepth) As Short
	'Invalid_string_refer_to_original_code
	Public VarIndex As Short
	Public VarIndexStack(MaxCallDepth) As Short
	Public VarStack(MaxVarIndex) As VarData
	'Invalid_string_refer_to_original_code
	Public ForIndex As Short
	Public ForIndexStack(MaxCallDepth) As Short
	Public ForLimitStack(MaxCallDepth) As Integer
	
	'ForEachコマンド用変数
	Public ForEachIndex As Short
	Public ForEachSet() As String
	
	'Invalid_string_refer_to_original_code
	Public LastUnitName As String
	Public LastPilotID() As String
	
	'Wait開始時刻
	Public WaitStartTime As Integer
	Public WaitTimeCount As Integer
	
	'Invalid_string_refer_to_original_code
	Public BaseX As Integer
	Public BaseY As Integer
	Private SavedBaseX(10) As Integer
	Private SavedBaseY(10) As Integer
	Private BasePointIndex As Integer
	
	'Invalid_string_refer_to_original_code
	Public ObjColor As Integer
	'Invalid_string_refer_to_original_code
	Public ObjDrawWidth As Integer
	'Invalid_string_refer_to_original_code
	Public ObjFillColor As Integer
	'Invalid_string_refer_to_original_code
	Public ObjFillStyle As Integer
	'Invalid_string_refer_to_original_code
	Public ObjDrawOption As String
	
	'Invalid_string_refer_to_original_code
	Public Structure HotPoint
		Dim Name As String
		'UPGRADE_NOTE: Left �� Left_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
		Dim Left_Renamed As Short
		Dim Top As Short
		Dim width As Short
		Dim Height As Short
		Dim Caption As String
	End Structure
	Public HotPointList() As HotPoint
	
	'Invalid_string_refer_to_original_code
	Public EventErrorMessage As String
	
	'Invalid_string_refer_to_original_code
	Public IsUnitCenter As Boolean
	
	
	'Invalid_string_refer_to_original_code
	Enum CmdType
		NullCmd = 0
		NopCmd
		ArcCmd
		ArrayCmd
		AskCmd
		AttackCmd
		AutoTalkCmd
		BossRankCmd
		BreakCmd
		CallCmd
		ReturnCmd
		CallInterMissionCommandCmd
		CancelCmd
		CenterCmd
		ChangeAreaCmd
		ChangeLayerCmd
		ChangeMapCmd
		ChangeModeCmd
		ChangePartyCmd
		ChangeTerrainCmd
		ChangeUnitBitmapCmd
		ChargeCmd
		CircleCmd
		ClearEventCmd
		ClearImageCmd
		ClearLayerCmd
		ClearObjCmd
		ClearPictureCmd
		ClearSkillCmd
		ClearSpecialPowerCmd
		ClearStatusCmd
		CloseCmd
		ClsCmd
		ColorCmd
		ColorFilterCmd
		CombineCmd
		ConfirmCmd
		ContinueCmd
		CopyArrayCmd
		CopyFileCmd
		CreateCmd
		CreateFolderCmd
		DebugCmd
		DestroyCmd
		DisableCmd
		DoCmd
		LoopCmd
		DrawOptionCmd
		DrawWidthCmd
		EnableCmd
		EquipCmd
		EscapeCmd
		ExchangeItemCmd
		ExecCmd
		ExitCmd
		ExplodeCmd
		ExpUpCmd
		FadeInCmd
		FadeOutCmd
		FillColorCmd
		FillStyleCmd
		FinishCmd
		FixCmd
		FontCmd
		ForCmd
		ForEachCmd
		NextCmd
		ForgetCmd
		GameClearCmd
		GameOverCmd
		FreeMemoryCmd
		GetOffCmd
		GlobalCmd
		GotoCmd
		HideCmd
		HotPointCmd
		IfCmd
		ElseCmd
		ElseIfCmd
		EndIfCmd
		IncrCmd
		IncreaseMoraleCmd
		InputCmd
		IntermissionCommandCmd
		ItemCmd
		JoinCmd
		KeepBGMCmd
		LandCmd
		LaunchCmd
		LeaveCmd
		LevelUpCmd
		LineCmd
		LineReadCmd
		LoadCmd
		LocalCmd
		MakePilotListCmd
		MakeUnitListCmd
		MapAbilityCmd
		MapAttackCmd
		MoneyCmd
		MonotoneCmd
		MoveCmd
		NightCmd
		NoonCmd
		OpenCmd
		OptionCmd
		OrganizeCmd
		OvalCmd
		PaintPictureCmd
		PaintStringCmd
		PaintStringRCmd
		PaintSysStringCmd
		PilotCmd
		PlayMIDICmd
		PlaySoundCmd
		PolygonCmd
		PrintCmd
		PSetCmd
		QuestionCmd
		QuickLoadCmd
		QuitCmd
		RankUpCmd
		ReadCmd
		RecoverENCmd
		RecoverHPCmd
		RecoverPlanaCmd
		RecoverSPCmd
		RedrawCmd
		RefreshCmd
		ReleaseCmd
		RemoveFileCmd
		RemoveFolderCmd
		RemoveItemCmd
		RemovePilotCmd
		RemoveUnitCmd
		RenameBGMCmd
		RenameFileCmd
		RenameTermCmd
		ReplacePilotCmd
		RequireCmd
		RestoreEventCmd
		RideCmd
		SaveDataCmd
		SelectCmd
		SelectTargetCmd
		SepiaCmd
		SetCmd
		SetSkillCmd
		SetBulletCmd
		SetMessageCmd
		SetRelationCmd
		SetStatusStringColorCmd
		SetStatusCmd
		SetStockCmd
		SetWindowColorCmd
		SetWindowFrameWidthCmd
		ShowCmd
		ShowImageCmd
		ShowUnitStatusCmd
		SkipCmd
		SortCmd
		SpecialPowerCmd
		SplitCmd
		StartBGMCmd
		StopBGMCmd
		StopSummoningCmd
		SupplyCmd
		SunsetCmd
		SwapCmd
		SwitchCmd
		CaseCmd
		CaseElseCmd
		EndSwCmd
		TalkCmd
		EndCmd
		SuspendCmd
		TelopCmd
		TransformCmd
		UnitCmd
		UnsetCmd
		UpgradeCmd
		UpVarCmd
		UseAbilityCmd
		WaitCmd
		WaterCmd
		WhiteInCmd
		WhiteOutCmd
		WriteCmd
		PlayFlashCmd
		ClearFlashCmd
	End Enum
	
	'Invalid_string_refer_to_original_code
	Enum LabelType
		NormalLabel = 0
		PrologueEventLabel
		StartEventLabel
		EpilogueEventLabel
		TurnEventLabel
		DamageEventLabel
		DestructionEventLabel
		TotalDestructionEventLabel
		AttackEventLabel
		AfterAttackEventLabel
		TalkEventLabel
		ContactEventLabel
		EnterEventLabel
		EscapeEventLabel
		LandEventLabel
		UseEventLabel
		AfterUseEventLabel
		TransformEventLabel
		CombineEventLabel
		SplitEventLabel
		FinishEventLabel
		LevelUpEventLabel
		RequirementEventLabel
		ResumeEventLabel
		MapCommandEventLabel
		UnitCommandEventLabel
		EffectEventLabel
	End Enum
	
	
	'Invalid_string_refer_to_original_code
	Public Sub InitEventData()
		Dim i As Integer
		
		ReDim Titles(0)
		ReDim EventData(0)
		ReDim EventCmd(50000)
		ReDim EventQue(0)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		For i = 1 To UBound(EventCmd)
			EventCmd(i) = New CmdData
			EventCmd(i).LineNum = i
		Next 
		
		'Invalid_string_refer_to_original_code
		LoadEventData("", "Invalid_string_refer_to_original_code")
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadEventData(ByRef fname As String, Optional ByRef load_mode As String = "")
		Dim buf, buf2 As String
		Dim tname, tfolder As String
		Dim new_titles() As String
		Dim i, num As Integer
		Dim j As Short
		Dim CmdStack(50) As CmdType
		Dim CmdStackIdx As Short
		Dim CmdPosStack(50) As Integer
		Dim CmdPosStackIdx As Short
		Dim error_found As Boolean
		Dim sys_event_data_size As Integer
		Dim sys_event_file_num As Integer
		
		'Invalid_string_refer_to_original_code
		ReDim Preserve EventData(SysEventDataSize)
		ReDim Preserve EventFileID(SysEventDataSize)
		ReDim Preserve EventLineNum(SysEventDataSize)
		ReDim Preserve EventFileNames(SysEventFileNum)
		ReDim AdditionalEventFileNames(0)
		CurrentLineNum = SysEventDataSize
		CallDepth = 0
		ArgIndex = 0
		UpVarLevel = 0
		VarIndex = 0
		If VarStack(1) Is Nothing Then
			For i = 1 To UBound(VarStack)
				VarStack(i) = New VarData
			Next 
		End If
		ForIndex = 0
		ReDim new_titles(0)
		ReDim HotPointList(0)
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'Invalid_string_refer_to_original_code
		With colNormalLabelList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		i = 1
		With colEventLabelList
			Do While i <= .Count()
				'UPGRADE_WARNING: �I�u�W�F�N�g colEventLabelList.Item(i).LineNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .Item(i).LineNum > SysEventDataSize Then
					.Remove(i)
				Else
					i = i + 1
				End If
			Loop 
		End With
		
		'Invalid_string_refer_to_original_code
		If LCase(ReadIni("Option", "DebugMode")) = "on" Then
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				DefineGlobalVariable("Invalid_string_refer_to_original_code")
			End If
			SetVariableAsLong("Invalid_string_refer_to_original_code", 1)
		End If
		
		'Invalid_string_refer_to_original_code
		If load_mode = "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then
				LoadEventData2(ExtDataPath & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then 
				LoadEventData2(ExtDataPath2 & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then 
				LoadEventData2(AppPath & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(ExtDataPath & "Invalid_string_refer_to_original_code") Then 
				LoadEventData2(ExtDataPath & "Invalid_string_refer_to_original_code")
			ElseIf FileExists(ExtDataPath2 & "Invalid_string_refer_to_original_code") Then 
				LoadEventData2(ExtDataPath2 & "Invalid_string_refer_to_original_code")
			ElseIf FileExists(AppPath & "Invalid_string_refer_to_original_code") Then 
				LoadEventData2(AppPath & "Invalid_string_refer_to_original_code")
			End If
			
			'Invalid_string_refer_to_original_code
			If LCase(ReadIni("Option", "BattleAnimation")) <> "off" Then
				BattleAnimation = True
			End If
			If FileExists(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve") Then
				LoadEventData2(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve")
			ElseIf FileExists(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve") Then 
				LoadEventData2(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve")
			ElseIf FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then 
				LoadEventData2(AppPath & "Lib\汎用戦闘アニメ\include.eve")
			Else
				'Invalid_string_refer_to_original_code
				BattleAnimation = False
			End If
			
			'Invalid_string_refer_to_original_code
			sys_event_data_size = UBound(EventData)
			sys_event_file_num = UBound(EventFileNames)
		ElseIf Not ScenarioLibChecked Then 
			'Invalid_string_refer_to_original_code
			
			ScenarioLibChecked = True
			
			If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") Or FileExists(ScenarioPath & "Invalid_string_refer_to_original_code") Or FileExists(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve") Then
				'Invalid_string_refer_to_original_code
				ReDim EventData(0)
				ReDim EventFileID(0)
				ReDim EventLineNum(0)
				ReDim EventFileNames(0)
				CurrentLineNum = 0
				SysEventDataSize = 0
				SysEventFileNum = 0
				With colSysNormalLabelList
					For i = 1 To .Count()
						.Remove(1)
					Next 
				End With
				With colNormalLabelList
					For i = 1 To .Count()
						.Remove(1)
					Next 
				End With
				With colEventLabelList
					For i = 1 To .Count()
						.Remove(1)
					Next 
				End With
				
				'Invalid_string_refer_to_original_code
				If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") Then
					LoadEventData2(ScenarioPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ScenarioPath & "Invalid_string_refer_to_original_code") Then 
					LoadEventData2(ScenarioPath & "Invalid_string_refer_to_original_code")
				ElseIf FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(ExtDataPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(ExtDataPath2 & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(AppPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ExtDataPath & "Invalid_string_refer_to_original_code") Then 
					LoadEventData2(ExtDataPath & "Invalid_string_refer_to_original_code")
				ElseIf FileExists(ExtDataPath2 & "Invalid_string_refer_to_original_code") Then 
					LoadEventData2(ExtDataPath2 & "Invalid_string_refer_to_original_code")
				ElseIf FileExists(AppPath & "Invalid_string_refer_to_original_code") Then 
					LoadEventData2(AppPath & "Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				If LCase(ReadIni("Option", "BattleAnimation")) <> "off" Then
					BattleAnimation = True
				End If
				If FileExists(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve") Then
					LoadEventData2(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve")
				ElseIf FileExists(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve") Then 
					LoadEventData2(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve") Then 
					LoadEventData2(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve")
				ElseIf FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then 
					LoadEventData2(AppPath & "Lib\汎用戦闘アニメ\include.eve")
				Else
					'Invalid_string_refer_to_original_code
					BattleAnimation = False
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If FileExists(ScenarioPath & "Lib\include.eve") Then
				LoadEventData2(ScenarioPath & "Lib\include.eve")
			End If
			
			'Invalid_string_refer_to_original_code
			sys_event_data_size = UBound(EventData)
			sys_event_file_num = UBound(EventFileNames)
			
			'Invalid_string_refer_to_original_code
			LoadEventData2(fname)
		Else
			'Invalid_string_refer_to_original_code
			LoadEventData2(fname)
		End If
		
		'エラー表示用にサイズを大きく取っておく
		ReDim Preserve EventData(UBound(EventData) + 1)
		ReDim Preserve EventLineNum(UBound(EventData))
		EventData(UBound(EventData)) = ""
		EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
		
		'Invalid_string_refer_to_original_code
		For i = SysEventDataSize + 1 To UBound(EventData)
			If Left(EventData(i), 1) = "@" Then
				tname = Mid(EventData(i), 2)
				
				'Invalid_string_refer_to_original_code
				For j = 1 To UBound(Titles)
					If tname = Titles(j) Then
						Exit For
					End If
				Next 
				
				If j > UBound(Titles) Then
					'フォルダを検索
					tfolder = SearchDataFolder(tname)
					If Len(tfolder) = 0 Then
						DisplayEventErrorMessage(i, "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						ReDim Preserve new_titles(UBound(new_titles) + 1)
						ReDim Preserve Titles(UBound(Titles) + 1)
						new_titles(UBound(new_titles)) = tname
						Titles(UBound(Titles)) = tname
					End If
				End If
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		If load_mode <> "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(Titles)
				tfolder = SearchDataFolder(Titles(i))
				If FileExists(tfolder & "\include.eve") Then
					LoadEventData2(tfolder & "\include.eve")
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			If FileExists(ScenarioPath & "Data\include.eve") Then
				LoadEventData2(ScenarioPath & "Data\include.eve")
			ElseIf FileExists(ExtDataPath & "Data\include.eve") Then 
				LoadEventData2(ExtDataPath & "Data\include.eve")
			ElseIf FileExists(ExtDataPath2 & "Data\include.eve") Then 
				LoadEventData2(ExtDataPath2 & "Data\include.eve")
			ElseIf FileExists(AppPath & "Data\include.eve") Then 
				LoadEventData2(AppPath & "Data\include.eve")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		For i = SysEventDataSize + 1 To UBound(EventData) - 1
			If Right(EventData(i), 1) = "_" Then
				EventData(i + 1) = Left(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
				EventData(i) = " "
			End If
		Next 
		
		'ラベルの登録
		num = CurrentLineNum
		If load_mode = "Invalid_string_refer_to_original_code" Then
			For CurrentLineNum = 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				If Right(buf, 1) = ":" Then
					AddSysLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
				End If
			Next 
		ElseIf sys_event_data_size > 0 Then 
			'Invalid_string_refer_to_original_code
			For CurrentLineNum = 1 To sys_event_data_size
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddSysLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "Invalid_string_refer_to_original_code"
						DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
						error_found = True
				End Select
			Next 
			For CurrentLineNum = sys_event_data_size + 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "Invalid_string_refer_to_original_code"
						DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
						error_found = True
				End Select
			Next 
		Else
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "Invalid_string_refer_to_original_code"
						DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
						error_found = True
				End Select
			Next 
		End If
		CurrentLineNum = num
		
		'Invalid_string_refer_to_original_code
		If UBound(EventData) > UBound(EventCmd) Then
			num = UBound(EventCmd)
			ReDim Preserve EventCmd(UBound(EventData))
			For i = num + 1 To UBound(EventCmd)
				EventCmd(i) = New CmdData
				EventCmd(i).LineNum = i
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		If load_mode <> "Invalid_string_refer_to_original_code" Then
			
			'Invalid_string_refer_to_original_code
			'制御構造
			CmdStackIdx = 0
			CmdPosStackIdx = 0
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				If EventCmd(CurrentLineNum) Is Nothing Then
					EventCmd(CurrentLineNum) = New CmdData
					EventCmd(CurrentLineNum).LineNum = CurrentLineNum
				End If
				With EventCmd(CurrentLineNum)
					'Invalid_string_refer_to_original_code
					If Not .Parse(EventData(CurrentLineNum)) Then
						error_found = True
					End If
					
					'Invalid_string_refer_to_original_code
					If .ArgNum = -1 Then
						Select Case CmdStack(CmdStackIdx)
							Case CmdType.AskCmd, CmdType.AutoTalkCmd, CmdType.QuestionCmd, CmdType.TalkCmd
								'Invalid_string_refer_to_original_code
							Case Else
								DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								error_found = True
						End Select
					End If
					
					'Invalid_string_refer_to_original_code
					Select Case .Name
						Case CmdType.IfCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If .GetArg(4) = "then" Then
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.IfCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.ElseIfCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) <> CmdType.IfCmd Then
								DisplayEventErrorMessage(CurrentLineNum, "ElseIfに対応するIfがありません")
								error_found = True
								
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.IfCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.ElseCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								DisplayEventErrorMessage(CurrentLineNum, "Elseに対応するIfがありません")
								error_found = True
								
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.IfCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.EndIfCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) = CmdType.IfCmd Then
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
							Else
								DisplayEventErrorMessage(CurrentLineNum, "EndIfに対応するIfがありません")
								error_found = True
							End If
							
						Case CmdType.DoCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							CmdStackIdx = CmdStackIdx + 1
							CmdPosStackIdx = CmdPosStackIdx + 1
							CmdStack(CmdStackIdx) = CmdType.DoCmd
							CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							
						Case CmdType.LoopCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) = CmdType.DoCmd Then
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
							Else
								DisplayEventErrorMessage(CurrentLineNum, "Loopに対応するDoがありません")
								error_found = True
							End If
							
						Case CmdType.ForCmd, CmdType.ForEachCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							CmdStackIdx = CmdStackIdx + 1
							CmdPosStackIdx = CmdPosStackIdx + 1
							CmdStack(CmdStackIdx) = .Name
							CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							
						Case CmdType.NextCmd
							If .ArgNum = 1 Or .ArgNum = 2 Then
								If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
									num = CmdPosStack(CmdPosStackIdx)
									DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
									CmdStackIdx = CmdStackIdx - 1
									CmdPosStackIdx = CmdPosStackIdx - 1
									error_found = True
								End If
								
								Select Case CmdStack(CmdStackIdx)
									Case CmdType.ForCmd, CmdType.ForEachCmd
										CmdStackIdx = CmdStackIdx - 1
										CmdPosStackIdx = CmdPosStackIdx - 1
									Case Else
										DisplayEventErrorMessage(CurrentLineNum, "Nextに対応するコマンドがありません")
										error_found = True
								End Select
							Else
								If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
									Select Case CmdStack(CmdStackIdx)
										Case CmdType.ForCmd, CmdType.ForEachCmd
											CmdStackIdx = CmdStackIdx - 1
											CmdPosStackIdx = CmdPosStackIdx - 1
										Case Else
											DisplayEventErrorMessage(CurrentLineNum, "Nextに対応するコマンドがありません")
											error_found = True
									End Select
								End If
							End If
							
						Case CmdType.SwitchCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								error_found = True
							End If
							
							CmdStackIdx = CmdStackIdx + 1
							CmdPosStackIdx = CmdPosStackIdx + 1
							CmdStack(CmdStackIdx) = CmdType.SwitchCmd
							CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							
						Case CmdType.CaseCmd, CmdType.CaseElseCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) <> CmdType.SwitchCmd Then
								DisplayEventErrorMessage(CurrentLineNum, "Caseに対応するSwitchがありません")
								error_found = True
								
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.SwitchCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.EndSwCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							If CmdStack(CmdStackIdx) = CmdType.SwitchCmd Then
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
							Else
								DisplayEventErrorMessage(CurrentLineNum, "EndSwに対応するSwitchがありません")
								error_found = True
							End If
							
						Case CmdType.TalkCmd, CmdType.AutoTalkCmd
							If CmdStack(CmdStackIdx) <> .Name Then
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = .Name
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.AskCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							i = .ArgNum
							Do While i > 1
								Select Case .GetArg(i)
									Case "通常"
									Case "拡大"
									Case "連続表示"
									Case "キャンセル可"
									Case "Invalid_string_refer_to_original_code"
										i = 3
										Exit Do
									Case Else
										Exit Do
								End Select
								i = i - 1
							Loop 
							If i < 3 Then
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.AskCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.QuestionCmd
							If CmdStack(CmdStackIdx) = CmdType.TalkCmd Then
								num = CmdPosStack(CmdPosStackIdx)
								DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
								CmdStackIdx = CmdStackIdx - 1
								CmdPosStackIdx = CmdPosStackIdx - 1
								error_found = True
							End If
							
							i = .ArgNum
							Do While i > 1
								Select Case .GetArg(.ArgNum)
									Case "通常"
									Case "拡大"
									Case "連続表示"
									Case "キャンセル可"
									Case "Invalid_string_refer_to_original_code"
										i = 4
										Exit Do
									Case Else
										Exit Do
								End Select
								i = i - 1
							Loop 
							If i < 4 Then
								CmdStackIdx = CmdStackIdx + 1
								CmdPosStackIdx = CmdPosStackIdx + 1
								CmdStack(CmdStackIdx) = CmdType.QuestionCmd
								CmdPosStack(CmdPosStackIdx) = CurrentLineNum
							End If
							
						Case CmdType.EndCmd
							Select Case CmdStack(CmdStackIdx)
								Case CmdType.TalkCmd, CmdType.AutoTalkCmd, CmdType.AskCmd, CmdType.QuestionCmd
									CmdStackIdx = CmdStackIdx - 1
									CmdPosStackIdx = CmdPosStackIdx - 1
								Case Else
									DisplayEventErrorMessage(CurrentLineNum, "Endに対応するTalkがありません")
									error_found = True
							End Select
							
						Case CmdType.SuspendCmd
							Select Case CmdStack(CmdStackIdx)
								Case CmdType.TalkCmd, CmdType.AutoTalkCmd
									CmdStackIdx = CmdStackIdx - 1
									CmdPosStackIdx = CmdPosStackIdx - 1
								Case Else
									DisplayEventErrorMessage(CurrentLineNum, "Suspendに対応するTalkがありません")
									error_found = True
							End Select
							
						Case CmdType.ExitCmd, CmdType.PlaySoundCmd, CmdType.WaitCmd
							Select Case CmdStack(CmdStackIdx)
								Case CmdType.TalkCmd, CmdType.AutoTalkCmd, CmdType.AskCmd, CmdType.QuestionCmd
									num = CmdPosStack(CmdPosStackIdx)
									DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
									CmdStackIdx = CmdStackIdx - 1
									CmdPosStackIdx = CmdPosStackIdx - 1
									error_found = True
							End Select
							
						Case CmdType.NopCmd
							If EventData(CurrentLineNum) = " " Then
								'Invalid_string_refer_to_original_code
								EventData(CurrentLineNum) = ""
							Else
								Select Case CmdStack(CmdStackIdx)
									Case CmdType.TalkCmd, CmdType.AutoTalkCmd, CmdType.AskCmd, CmdType.QuestionCmd
										If CurrentLineNum = UBound(EventData) Then
											num = CmdPosStack(CmdPosStackIdx)
											DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
											CmdStackIdx = CmdStackIdx - 1
											CmdPosStackIdx = CmdPosStackIdx - 1
											error_found = True
										Else
											buf = LCase(ListIndex(EventData(CurrentLineNum + 1), 1))
											Select Case CmdStack(CmdStackIdx)
												Case CmdType.TalkCmd
													buf2 = "talk"
												Case CmdType.AutoTalkCmd
													buf2 = "autotalk"
												Case CmdType.AskCmd
													buf2 = "ask"
												Case CmdType.QuestionCmd
													buf2 = "question"
												Case Else
													buf2 = ""
											End Select
											'UPGRADE_ISSUE: �萔 vbFromUnicode �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
											'UPGRADE_ISSUE: LenB �֐��̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ���N���b�N���Ă��������B
											If buf <> buf2 And buf <> "end" And buf <> "suspend" And Len(buf) = LenB(StrConv(buf, vbFromUnicode)) Then
												num = CmdPosStack(CmdPosStackIdx)
												DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
												CmdStackIdx = CmdStackIdx - 1
												CmdPosStackIdx = CmdPosStackIdx - 1
												error_found = True
											End If
										End If
								End Select
							End If
							
					End Select
				End With
			Next 
			
			'Invalid_string_refer_to_original_code
			If CmdStackIdx > 0 Then
				num = CmdPosStack(CmdPosStackIdx)
				Select Case CmdStack(CmdStackIdx)
					Case CmdType.AskCmd
						DisplayEventErrorMessage(num, "Askに対応するEndがありません")
					Case CmdType.AutoTalkCmd
						DisplayEventErrorMessage(num, "AutoTalkに対応するEndがありません")
					Case CmdType.DoCmd
						DisplayEventErrorMessage(num, "Doに対応するLoopがありません")
					Case CmdType.ForCmd
						DisplayEventErrorMessage(num, "Forに対応するNextがありません")
					Case CmdType.ForEachCmd
						DisplayEventErrorMessage(num, "ForEachに対応するNextがありません")
					Case CmdType.IfCmd
						DisplayEventErrorMessage(num, "Ifに対応するEndIfがありません")
					Case CmdType.QuestionCmd
						DisplayEventErrorMessage(num, "Questionに対応するEndがありません")
					Case CmdType.SwitchCmd
						DisplayEventErrorMessage(num, "Switchに対応するEndSwがありません")
					Case CmdType.TalkCmd
						DisplayEventErrorMessage(num, "Talkに対応するEndがありません")
				End Select
				error_found = True
			End If
			
			'Invalid_string_refer_to_original_code
			If error_found Then
				TerminateSRC()
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				With EventCmd(CurrentLineNum)
					Select Case .Name
						Case CmdType.CreateCmd
							If .ArgNum < 8 Then
								DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								error_found = True
							End If
						Case CmdType.PilotCmd
							If .ArgNum < 3 Then
								DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								error_found = True
							End If
						Case CmdType.UnitCmd
							If .ArgNum <> 3 Then
								DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								error_found = True
							End If
					End Select
				End With
			Next 
			
			'Invalid_string_refer_to_original_code
			If error_found Then
				TerminateSRC()
			End If
			
			'Invalid_string_refer_to_original_code
		Else
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If CurrentLineNum > UBound(EventCmd) Then
				ReDim Preserve EventCmd(CurrentLineNum)
				i = CurrentLineNum
				Do While EventCmd(i) Is Nothing
					EventCmd(i) = New CmdData
					EventCmd(i).LineNum = i
					i = i - 1
				Loop 
			End If
			
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If sys_event_data_size > 0 Then
			SysEventDataSize = sys_event_data_size
			SysEventFileNum = sys_event_file_num
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case load_mode
			Case "リストア"
				ADList.AddDefaultAnimation()
				Exit Sub
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Exit Sub
		End Select
		
		'Invalid_string_refer_to_original_code
		If fname = "" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		num = 2 * UBound(new_titles)
		If IsLocalDataLoaded Then
			If num > 0 Then
				num = num + 2
			End If
		Else
			num = num + 2
		End If
		If FileExists(Left(fname, Len(fname) - 4) & ".map") Then
			num = num + 1
		End If
		If num = 0 And IsLocalDataLoaded Then
			'Invalid_string_refer_to_original_code
			ADList.AddDefaultAnimation()
			Exit Sub
		End If
		
		'ロード画面を表示
		OpenNowLoadingForm()
		
		'Invalid_string_refer_to_original_code
		SetLoadImageSize(num)
		
		'Invalid_string_refer_to_original_code
		For i = 1 To UBound(new_titles)
			IncludeData(new_titles(i))
		Next 
		
		'Invalid_string_refer_to_original_code
		If Not IsLocalDataLoaded Or UBound(new_titles) > 0 Then
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
		End If
		
		'Invalid_string_refer_to_original_code
		ADList.AddDefaultAnimation()
		
		'Invalid_string_refer_to_original_code
		If FileExists(Left(fname, Len(fname) - 4) & ".map") Then
			LoadMapData(Left(fname, Len(fname) - 4) & ".map")
			SetupBackground()
			RedrawScreen()
			DisplayLoadingProgress()
		End If
		
		'ロード画面を閉じる
		CloseNowLoadingForm()
	End Sub
	
	'イベントファイルの読み込み
	Public Sub LoadEventData2(ByRef fname As String, Optional ByVal lnum As Integer = 0)
		Dim FileNumber, CurrentLineNum2 As Short
		Dim i As Short
		Dim buf, fname2 As String
		Dim fid As Short
		Dim in_single_quote, in_double_quote As Boolean
		
		If fname = "" Then
			Exit Sub
		End If
		
		'イベントファイル名を記録しておく (エラー表示用)
		ReDim Preserve EventFileNames(UBound(EventFileNames) + 1)
		EventFileNames(UBound(EventFileNames)) = fname
		fid = UBound(EventFileNames)
		
		On Error GoTo ErrorHandler
		
		'Invalid_string_refer_to_original_code
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		'Invalid_string_refer_to_original_code
		If lnum > 0 Then
			CurrentLineNum = lnum
		End If
		CurrentLineNum2 = 0
		
		'Invalid_string_refer_to_original_code
		Do Until EOF(FileNumber)
			CurrentLineNum = CurrentLineNum + 1
			CurrentLineNum2 = CurrentLineNum2 + 1
			
			'Invalid_string_refer_to_original_code
			ReDim Preserve EventData(CurrentLineNum)
			ReDim Preserve EventFileID(CurrentLineNum)
			ReDim Preserve EventLineNum(CurrentLineNum)
			
			'Invalid_string_refer_to_original_code
			buf = LineInput(FileNumber)
			TrimString(buf)
			
			'コメントを削除
			If Left(buf, 1) = "#" Then
				buf = " "
			ElseIf InStr(buf, "//") > 0 Then 
				in_single_quote = False
				in_double_quote = False
				For i = 1 To Len(buf)
					Select Case Mid(buf, i, 1)
						Case "`"
							'Invalid_string_refer_to_original_code
							If Not in_double_quote Then
								in_single_quote = Not in_single_quote
							End If
						Case """"
							'Invalid_string_refer_to_original_code
							If Not in_single_quote Then
								in_double_quote = Not in_double_quote
							End If
						Case "/"
							'Invalid_string_refer_to_original_code
							If Not in_double_quote And Not in_single_quote Then
								If i > 1 Then
									If Mid(buf, i - 1, 1) = "/" Then
										buf = Left(buf, i - 2)
										If buf = "" Then
											buf = " "
										End If
										Exit For
									End If
								End If
							End If
					End Select
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			EventData(CurrentLineNum) = buf
			EventFileID(CurrentLineNum) = fid
			EventLineNum(CurrentLineNum) = CurrentLineNum2
			
			'Invalid_string_refer_to_original_code
			If Left(buf, 1) = "<" Then
				If InStr(buf, ">") = Len(buf) And buf <> "<>" Then
					CurrentLineNum = CurrentLineNum - 1
					fname2 = Mid(buf, 2, Len(buf) - 2)
					If fname2 <> "Lib\スペシャルパワー.eve" And fname2 <> "Lib\汎用戦闘アニメ\include.eve" And fname2 <> "Lib\include.eve" Then
						'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
						If Len(Dir(ScenarioPath & fname2)) > 0 Then
							LoadEventData2(ScenarioPath & fname2)
							'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
						ElseIf Len(Dir(ExtDataPath & fname2)) > 0 Then 
							LoadEventData2(ExtDataPath & fname2)
							'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
						ElseIf Len(Dir(ExtDataPath2 & fname2)) > 0 Then 
							LoadEventData2(ExtDataPath2 & fname2)
							'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
						ElseIf Len(Dir(AppPath & fname2)) > 0 Then 
							LoadEventData2(AppPath & fname2)
						End If
					End If
				End If
			End If
		Loop 
		
		'ファイルを閉じる
		FileClose(FileNumber)
		
		Exit Sub
		
ErrorHandler: 
		If Len(buf) = 0 Then
			ErrorMessage(fname & "が開けません")
		Else
			ErrorMessage(fname & "のロード中にエラーが発生しました" & vbCr & VB6.Format(CurrentLineNum2) & "Invalid_string_refer_to_original_code")
		End If
		TerminateSRC()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_WARNING: ParamArray Args �� ByRef ���� ByVal �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' ���N���b�N���Ă��������B
	Public Sub HandleEvent(ParamArray ByVal Args() As Object)
		Dim event_que_idx As Short
		Dim ret As Integer
		Dim i As Short
		Dim flag As Boolean
		Dim prev_is_gui_locked As Boolean
		Dim prev_call_depth As Short
		Dim uparty As String
		Dim u As Unit
		Dim main_event_done As Boolean
		
		'Invalid_string_refer_to_original_code
		prev_is_gui_locked = IsGUILocked
		If Not IsGUILocked Then
			LockGUI()
		End If
		
		'Invalid_string_refer_to_original_code
		'(SearchLabel()実行時の式計算用にあらかじめ設定しておく)
		SelectedUnitForEvent = SelectedUnit
		'Invalid_string_refer_to_original_code
		If UBound(Args) > 0 Then
			If PList.IsDefined(Args(1)) Then
				With PList.Item(Args(1))
					If Not .Unit_Renamed Is Nothing Then
						SelectedUnitForEvent = .Unit_Renamed
					End If
				End With
			End If
		End If
		SelectedTargetForEvent = SelectedTarget
		
		'Invalid_string_refer_to_original_code
		ReDim Preserve EventQue(UBound(EventQue) + 1)
		event_que_idx = UBound(EventQue)
		Select Case Args(0)
			Case "プロローグ"
				EventQue(UBound(EventQue)) = "プロローグ"
				Stage = "プロローグ"
			Case "エピローグ"
				EventQue(UBound(EventQue)) = "エピローグ"
				Stage = "エピローグ"
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1)
				With PList.Item(Args(1))
					uparty = .Party
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'Invalid_string_refer_to_original_code
							' MOD START MARGE
							'                        For i = 1 To .CountUnitOnBoard
							'                            Set u = .UnitOnBoard(1)
							'                            .UnloadUnit u.ID
							'Invalid_string_refer_to_original_code
							'                            u.HP = 0
							'                            ReDim Preserve EventQue(UBound(EventQue) + 1)
							'                            EventQue(UBound(EventQue)) = _
							'Invalid_string_refer_to_original_code
							'                        Next
							Do While .CountUnitOnBoard > 0
								u = .UnitOnBoard(1)
								.UnloadUnit((u.ID))
								u.Status_Renamed = "Invalid_string_refer_to_original_code"
								u.HP = 0
								ReDim Preserve EventQue(UBound(EventQue) + 1)
								EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & u.MainPilot.ID
							Loop 
							' MOD END MARGE
							uparty = .Party0
						End With
					End If
				End With
				
				'Invalid_string_refer_to_original_code
				flag = False
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						flag = True
						Exit For
						'End If
					End With
				Next u
				If Not flag Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & uparty
				End If
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1)
				With PList.Item(Args(1))
					uparty = .Party
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'Invalid_string_refer_to_original_code
							For i = 1 To .CountUnitOnBoard
								u = .UnitOnBoard(i)
								.UnloadUnit((u.ID))
								u.Status_Renamed = "Invalid_string_refer_to_original_code"
								u.HP = 0
								ReDim Preserve EventQue(UBound(EventQue) + 1)
								EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & u.MainPilot.ID
							Next 
							uparty = .Party0
						End With
					End If
				End With
			Case "ターン"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "ターン 全 " & Args(2)
				ReDim Preserve EventQue(UBound(EventQue) + 1)
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "ターン " & VB6.Format(Args(1)) & " " & Args(2)
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & VB6.Format(Args(2))
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & Args(2)
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & Args(2)
			Case "会話"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "会話 " & Args(1) & " " & Args(2)
			Case "接触"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "接触 " & Args(1) & " " & Args(2)
			Case "進入"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " & VB6.Format(Args(2)) & " " & VB6.Format(Args(3))
				ReDim Preserve EventQue(UBound(EventQue) + 1)
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " & TerrainName(CShort(Args(2)), CShort(Args(3)))
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If Args(2) = 1 Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " W"
					'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				ElseIf Args(2) = MapWidth Then 
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " E"
				End If
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(3) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If Args(3) = 1 Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " N"
					'UPGRADE_WARNING: �I�u�W�F�N�g Args(3) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				ElseIf Args(3) = MapHeight Then 
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " S"
				End If
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1)
			Case "使用"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "使用 " & Args(1) & " " & Args(2)
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & Args(2)
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1)
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: �I�u�W�F�N�g Args(2) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & Args(2)
				If Not IsEventDefined(EventQue(UBound(EventQue))) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = "Invalid_string_refer_to_original_code" & Args(1) & " " & PList.Item(Args(2)).Unit_Renamed.Name
				End If
			Case Else
				'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				EventQue(UBound(EventQue)) = Args(0)
				For i = 1 To UBound(Args)
					'UPGRADE_WARNING: �I�u�W�F�N�g Args(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
				Next 
		End Select
		
		If CallDepth > MaxCallDepth Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CallDepth = MaxCallDepth
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		ArgIndexStack(CallDepth) = ArgIndex
		VarIndexStack(CallDepth) = VarIndex
		ForIndexStack(CallDepth) = ForIndex
		SaveBasePoint()
		
		'Invalid_string_refer_to_original_code
		prev_call_depth = CallDepth
		CallDepth = CallDepth + 1
		
		'Invalid_string_refer_to_original_code
		i = event_que_idx
		IsCanceled = False
		Do 
			'Debug.Print "HandleEvent (" & EventQue(i) & ")"
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			uparty = LIndex(EventQue(i), 2)
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					GoTo NextLoop
					'End If
				End With
			Next u
			'End If
			
			CurrentLabel = 0
			main_event_done = False
			Do While True
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				SelectedUnitForEvent = SelectedUnit
				'Invalid_string_refer_to_original_code
				If UBound(Args) > 0 Then
					If PList.IsDefined(Args(1)) Then
						With PList.Item(Args(1))
							If Not .Unit_Renamed Is Nothing Then
								SelectedUnitForEvent = .Unit_Renamed
							End If
						End With
					End If
				End If
				SelectedTargetForEvent = SelectedTarget
				
				'Invalid_string_refer_to_original_code
				Do 
					If IsNumeric(EventQue(i)) Then
						If CurrentLabel = 0 Then
							ret = CInt(EventQue(i))
						Else
							ret = 0
						End If
					Else
						ret = SearchLabel(EventQue(i), CurrentLabel + 1)
					End If
					If ret = 0 Then
						GoTo NextLoop
					End If
					
					CurrentLabel = ret
					
					If Asc(EventData(ret)) <> 42 Then '*
						'Invalid_string_refer_to_original_code
						If main_event_done Then
							ret = 0
						Else
							main_event_done = True
						End If
					End If
				Loop While ret = 0
				
				'Invalid_string_refer_to_original_code
				If Left(EventData(ret), 1) <> "*" Then
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'画面をクリア
					If MainForm.Visible = True Then
						ClearUnitStatus()
						RedrawScreen()
					End If
					
					'Invalid_string_refer_to_original_code
					If frmMessage.Visible = True Then
						CloseMessageForm()
					End If
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				ret = ret + 1
				
				System.Windows.Forms.Application.DoEvents()
				
				'Invalid_string_refer_to_original_code
				Do 
					CurrentLineNum = ret
					If CurrentLineNum > UBound(EventCmd) Then
						GoTo ExitLoop
					End If
					ret = EventCmd(CurrentLineNum).Exec
				Loop While ret > 0
				
				'Invalid_string_refer_to_original_code
				If IsScenarioFinished Or IsCanceled Then
					GoTo ExitLoop
				End If
			Loop 
NextLoop: 
			i = i + 1
		Loop While i <= UBound(EventQue)
ExitLoop: 
		
		If CallDepth >= 0 Then
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			CallDepth = prev_call_depth
			
			'イベント実行前の状態に復帰
			ArgIndex = ArgIndexStack(CallDepth)
			VarIndex = VarIndexStack(CallDepth)
			ForIndex = ForIndexStack(CallDepth)
		Else
			ArgIndex = 0
			VarIndex = 0
			ForIndex = 0
		End If
		
		'Invalid_string_refer_to_original_code
		ReDim Preserve EventQue(MinLng(event_que_idx - 1, UBound(EventQue)))
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.ForeColor = RGB(255, 255, 255)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			With .Font
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Size = 16
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Name = "Invalid_string_refer_to_original_code"
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Bold = True
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Italic = False
			End With
			PermanentStringMode = False
			KeepStringMode = False
		End With
		
		'Invalid_string_refer_to_original_code
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'Invalid_string_refer_to_original_code
		RestoreBasePoint()
		
		'Invalid_string_refer_to_original_code
		If Not prev_is_gui_locked Then
			UnlockGUI()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_WARNING: ParamArray Args �� ByRef ���� ByVal �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' ���N���b�N���Ă��������B
	Public Sub RegisterEvent(ParamArray ByVal Args() As Object)
		Dim i As Short
		
		ReDim Preserve EventQue(UBound(EventQue) + 1)
		'UPGRADE_WARNING: �I�u�W�F�N�g Args() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EventQue(UBound(EventQue)) = Args(0)
		For i = 1 To UBound(Args)
			'UPGRADE_WARNING: �I�u�W�F�N�g Args(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
		Next 
	End Sub
	
	
	'ラベルの検索
	Public Function SearchLabel(ByRef lname As String, Optional ByVal start As Integer = 0) As Integer
		Dim ltype As LabelType
		Dim llen As Short
		Dim litem() As String
		Dim lnum(4) As String
		Dim is_unit(4) As Boolean
		Dim is_num(4) As Boolean
		Dim is_condition(4) As Boolean
		Dim str2, str1, lname2 As String
		Dim i As Integer
		Dim lab As LabelData
		Dim tmp_u As Unit
		Dim revrersible, reversed As Boolean
		
		'Invalid_string_refer_to_original_code
		llen = ListSplit(lname, litem)
		
		'Invalid_string_refer_to_original_code
		Select Case litem(1)
			Case "プロローグ"
				ltype = LabelType.PrologueEventLabel
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.StartEventLabel
			Case "エピローグ"
				ltype = LabelType.EpilogueEventLabel
			Case "ターン"
				ltype = LabelType.TurnEventLabel
				If IsNumeric(litem(2)) Then
					is_num(2) = True
				End If
				lnum(2) = CStr(StrToLng(litem(2)))
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.DamageEventLabel
				is_unit(2) = True
				is_num(3) = True
				lnum(3) = CStr(StrToLng(litem(3)))
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				ltype = LabelType.DestructionEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.TotalDestructionEventLabel
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.AttackEventLabel
				revrersible = True
				is_unit(2) = True
				is_unit(3) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.AfterAttackEventLabel
				revrersible = True
				is_unit(2) = True
				is_unit(3) = True
			Case "会話"
				ltype = LabelType.TalkEventLabel
				is_unit(2) = True
				is_unit(3) = True
			Case "接触"
				ltype = LabelType.ContactEventLabel
				revrersible = True
				is_unit(2) = True
				is_unit(3) = True
			Case "進入"
				ltype = LabelType.EnterEventLabel
				is_unit(2) = True
				If llen = 4 Then
					is_num(3) = True
					is_num(4) = True
					lnum(3) = CStr(StrToLng(litem(3)))
					lnum(4) = CStr(StrToLng(litem(4)))
				End If
			Case "脱出"
				ltype = LabelType.EscapeEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.LandEventLabel
				is_unit(2) = True
			Case "使用"
				ltype = LabelType.UseEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.AfterUseEventLabel
				is_unit(2) = True
			Case "変形"
				ltype = LabelType.TransformEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.CombineEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.SplitEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.FinishEventLabel
				is_unit(2) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.LevelUpEventLabel
				is_unit(2) = True
			Case "勝利条件"
				ltype = LabelType.RequirementEventLabel
			Case "再開"
				ltype = LabelType.ResumeEventLabel
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.MapCommandEventLabel
				is_condition(3) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.UnitCommandEventLabel
				is_condition(4) = True
			Case "Invalid_string_refer_to_original_code"
				ltype = LabelType.EffectEventLabel
			Case Else
				ltype = LabelType.NormalLabel
		End Select
		
		'Invalid_string_refer_to_original_code
		For	Each lab In colEventLabelList
			With lab
				'Invalid_string_refer_to_original_code
				If ltype <> .Name Then
					GoTo NextLabel
				End If
				
				'Invalid_string_refer_to_original_code
				If Not .Enable Then
					GoTo NextLabel
				End If
				
				'Invalid_string_refer_to_original_code
				If .LineNum < start Then
					GoTo NextLabel
				End If
				
				'Invalid_string_refer_to_original_code
				If llen <> .CountPara Then
					If ltype <> LabelType.MapCommandEventLabel And ltype <> LabelType.UnitCommandEventLabel Then
						GoTo NextLabel
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				reversed = False
CheckPara: 
				For i = 2 To llen
					'Invalid_string_refer_to_original_code
					If is_condition(i) Then
						Exit For
					End If
					
					'Invalid_string_refer_to_original_code
					str1 = litem(i)
					If reversed Then
						str2 = .Para(5 - i)
					Else
						str2 = .Para(i)
					End If
					
					'Invalid_string_refer_to_original_code
					If str2 = "全" Then
						'Invalid_string_refer_to_original_code
						If ltype <> LabelType.TurnEventLabel Or i <> 2 Then
							GoTo NextPara
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If is_num(i) Then
						If IsNumeric(str2) Then
							If CDbl(lnum(i)) = CInt(str2) Then
								GoTo NextPara
							ElseIf ltype = LabelType.DamageEventLabel Then 
								'Invalid_string_refer_to_original_code
								If CDbl(lnum(i)) > CInt(str2) Then
									Exit For
								End If
							End If
						End If
						GoTo NextLabel
					End If
					
					'Invalid_string_refer_to_original_code
					If is_unit(i) Then
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If PList.IsDefined(str1) Then
							str1 = PList.Item(str1).Party
						End If
					End If
					PList.IsDefined(str2)
					'Invalid_string_refer_to_original_code
					With PList.Item(str2)
						If str2 = .Data.Name Or str2 = .Data.Nickname Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							str2 = .Name
							If PList.IsDefined(str1) Then
								str1 = PList.Item(str1).Name
							End If
						Else
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							If PList.IsDefined(str1) Then
								str1 = PList.Item(str1).ID
							End If
							If InStr(str1, ":") > 0 Then
								str1 = Left(str1, InStr(str1, ":") - 1)
							End If
						End If
					End With
					PDList.IsDefined(str2)
					'Invalid_string_refer_to_original_code
					str2 = PDList.Item(str2).Name
					If PList.IsDefined(str1) Then
						str1 = PList.Item(str1).Name
					End If
					UDList.IsDefined(str2)
					'Invalid_string_refer_to_original_code
					If PList.IsDefined(str1) Then
						With PList.Item(str1)
							If Not .Unit_Renamed Is Nothing Then
								str1 = .Unit_Renamed.Name
							End If
						End With
					End If
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If PList.IsDefined(str1) Then
						str1 = PList.Item(str1).ID
					End If
					If InStr(str1, ":") > 0 Then
						str1 = Left(str1, InStr(str1, ":") - 1)
					End If
					If InStr(str2, ":") > 0 Then
						str2 = Left(str2, InStr(str2, ":") - 1)
					End If
					'End If
					'End If
					
					'Invalid_string_refer_to_original_code
					If str1 <> str2 Then
						If revrersible And Not reversed Then
							'Invalid_string_refer_to_original_code
							lname2 = litem(1) & " " & ListIndex(.Data, 3) & " " & ListIndex(.Data, 2)
							If .AsterNum > 0 Then
								lname2 = "*" & lname2
							End If
							If FindLabel(lname2) = 0 Then
								'Invalid_string_refer_to_original_code
								reversed = True
								GoTo CheckPara
							End If
						End If
						GoTo NextLabel
					End If
NextPara: 
				Next 
				
				'Invalid_string_refer_to_original_code
				SearchLabel = .LineNum
				
				'Invalid_string_refer_to_original_code
				If reversed Then
					tmp_u = SelectedUnitForEvent
					SelectedUnitForEvent = SelectedTargetForEvent
					SelectedTargetForEvent = tmp_u
				End If
				Exit Function
			End With
NextLabel: 
		Next lab
		
		SearchLabel = 0
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function IsEventDefined(ByRef lname As String, Optional ByVal normal_event_only As Boolean = False) As Boolean
		Dim i, ret As Integer
		
		'Invalid_string_refer_to_original_code
		i = 0
		Do While 1
			ret = SearchLabel(lname, i + 1)
			If ret = 0 Then
				Exit Function
			End If
			
			If normal_event_only Then
				'Invalid_string_refer_to_original_code
				If Asc(EventData(ret)) <> 42 Then '*
					IsEventDefined = True
					Exit Function
				End If
			Else
				IsEventDefined = True
				Exit Function
			End If
			i = ret
		Loop 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsLabelDefined(ByRef Index As Object) As Boolean
		Dim lab As LabelData
		
		On Error GoTo ErrorHandler
		lab = colEventLabelList.Item(Index)
		IsLabelDefined = True
		Exit Function
		
ErrorHandler: 
		IsLabelDefined = False
	End Function
	
	'ラベルを追加
	Public Sub AddLabel(ByRef lname As String, ByVal lnum As Integer)
		Dim new_label As New LabelData
		Dim lname2 As String
		Dim i As Short
		
		On Error GoTo ErrorHandler
		
		With new_label
			.Data = lname
			.LineNum = lnum
			.Enable = True
			
			If .Name = LabelType.NormalLabel Then
				'通常ラベルを追加
				If FindNormalLabel0(lname) = 0 Then
					colNormalLabelList.Add(new_label, lname)
				End If
			Else
				'イベントラベルを追加
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				lname2 = ListIndex(lname, 1)
				For i = 2 To ListLength(lname)
					lname2 = lname2 & " " & ListIndex(lname, i)
				Next 
				
				If Not IsLabelDefined(lname2) Then
					colEventLabelList.Add(new_label, lname2)
				Else
					colEventLabelList.Add(new_label, lname2 & "(" & VB6.Format(lnum) & ")")
				End If
			End If
		End With
		
		Exit Sub
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AddSysLabel(ByRef lname As String, ByVal lnum As Integer)
		Dim new_label As New LabelData
		Dim lname2 As String
		Dim i As Short
		
		On Error GoTo ErrorHandler
		
		With new_label
			.Data = lname
			.LineNum = lnum
			.Enable = True
			
			If .Name = LabelType.NormalLabel Then
				'通常ラベルを追加
				If FindSysNormalLabel(lname) = 0 Then
					colSysNormalLabelList.Add(new_label, lname)
				Else
					'UPGRADE_WARNING: �I�u�W�F�N�g colSysNormalLabelList.Item().LineNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					colSysNormalLabelList.Item(lname).LineNum = lnum
				End If
			Else
				'イベントラベルを追加
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				lname2 = ListIndex(lname, 1)
				For i = 2 To ListLength(lname)
					lname2 = lname2 & " " & ListIndex(lname, i)
				Next 
				
				If Not IsLabelDefined(lname2) Then
					colEventLabelList.Add(new_label, lname2)
				Else
					colEventLabelList.Add(new_label, lname2 & "(" & VB6.Format(lnum) & ")")
				End If
			End If
		End With
		
		Exit Sub
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
	End Sub
	
	'ラベルを消去
	Public Sub ClearLabel(ByVal lnum As Integer)
		Dim lab As LabelData
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		For	Each lab In colEventLabelList
			With lab
				If .LineNum = lnum Then
					.Enable = False
					Exit Sub
				End If
			End With
		Next lab
		
		'Invalid_string_refer_to_original_code
		For i = 1 To 10
			For	Each lab In colEventLabelList
				With lab
					If .LineNum = lnum - i Or .LineNum = lnum + i Then
						.Enable = False
						Exit Sub
					End If
				End With
			Next lab
		Next 
	End Sub
	
	'ラベルを復活
	Public Sub RestoreLabel(ByRef lname As String)
		Dim lab As LabelData
		
		For	Each lab In colEventLabelList
			With lab
				If .Data = lname Then
					.Enable = True
					Exit Sub
				End If
			End With
		Next lab
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function FindLabel(ByRef lname As String) As Integer
		Dim lname2 As String
		Dim i As Short
		
		'通常ラベルから検索
		FindLabel = FindNormalLabel(lname)
		If FindLabel > 0 Then
			Exit Function
		End If
		
		'イベントラベルから検索
		FindLabel = FindEventLabel(lname)
		If FindLabel > 0 Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		lname2 = ListIndex(lname, 1)
		For i = 2 To ListLength(lname)
			lname2 = lname2 & " " & ListIndex(lname, i)
		Next 
		
		'イベントラベルから検索
		FindLabel = FindEventLabel(lname2)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FindEventLabel(ByRef lname As String) As Integer
		Dim lab As LabelData
		
		On Error GoTo NotFound
		lab = colEventLabelList.Item(lname)
		FindEventLabel = lab.LineNum
		Exit Function
		
NotFound: 
		FindEventLabel = 0
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FindNormalLabel(ByRef lname As String) As Integer
		FindNormalLabel = FindNormalLabel0(lname)
		If FindNormalLabel = 0 Then
			FindNormalLabel = FindSysNormalLabel(lname)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function FindNormalLabel0(ByRef lname As String) As Integer
		Dim lab As LabelData
		
		On Error GoTo NotFound
		lab = colNormalLabelList.Item(lname)
		FindNormalLabel0 = lab.LineNum
		Exit Function
		
NotFound: 
		FindNormalLabel0 = 0
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function FindSysNormalLabel(ByRef lname As String) As Integer
		Dim lab As LabelData
		
		On Error GoTo NotFound
		lab = colSysNormalLabelList.Item(lname)
		FindSysNormalLabel = lab.LineNum
		Exit Function
		
NotFound: 
		FindSysNormalLabel = 0
	End Function
	
	
	'イベントデータの消去
	'Invalid_string_refer_to_original_code
	Public Sub ClearEventData()
		Dim i As Short
		
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedUnitForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedUnitForEvent = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		
		ReDim Preserve EventData(SysEventDataSize)
		
		With colNormalLabelList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		i = 1
		With colEventLabelList
			Do While i <= .Count()
				'UPGRADE_WARNING: �I�u�W�F�N�g colEventLabelList.Item(i).LineNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .Item(i).LineNum > SysEventDataSize Then
					.Remove(i)
				Else
					i = i + 1
				End If
			Loop 
		End With
		
		ReDim EventQue(0)
		
		CallDepth = 0
		ArgIndex = 0
		VarIndex = 0
		ForIndex = 0
		UpVarLevel = 0
		ReDim HotPointList(0)
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		IsPictureVisible = False
		IsCursorVisible = False
		
		PaintedAreaX1 = MainPWidth
		PaintedAreaY1 = MainPHeight
		PaintedAreaX2 = -1
		PaintedAreaY2 = -1
		
		With LocalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
	End Sub
	
	'グローバル変数を含めたイベントデータの全消去
	Public Sub ClearAllEventData()
		Dim i As Short
		
		ClearEventData()
		
		With GlobalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		DefineGlobalVariable("Invalid_string_refer_to_original_code")
		DefineGlobalVariable("Invalid_string_refer_to_original_code")
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub DumpEventData()
		Dim lab As LabelData
		Dim i As Short
		
		'グローバル変数
		SaveGlobalVariables()
		'ローカル変数
		SaveLocalVariables()
		
		'イベント用ラベル
		WriteLine(SaveDataFileNumber, colEventLabelList.Count())
		For	Each lab In colEventLabelList
			WriteLine(SaveDataFileNumber, lab.Enable)
		Next lab
		
		'Requireコマンドで追加されたイベントファイル
		WriteLine(SaveDataFileNumber, UBound(AdditionalEventFileNames))
		For i = 1 To UBound(AdditionalEventFileNames)
			WriteLine(SaveDataFileNumber, AdditionalEventFileNames(i))
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreEventData()
		Dim lab As LabelData
		Dim num As Short
		Dim lenable As Boolean
		Dim fname As String
		Dim file_head As Integer
		Dim i As Integer
		Dim j As Short
		Dim buf As String
		
		'グローバル変数
		LoadGlobalVariables()
		'ローカル変数
		LoadLocalVariables()
		
		'イベント用ラベル
		Input(SaveDataFileNumber, num)
		' MOD START MARGE
		'    i = 1
		'    For Each lab In colEventLabelList
		'        If i <= num Then
		'            Input #SaveDataFileNumber, lenable
		'            lab.Enable = lenable
		'        Else
		'            lab.Enable = True
		'        End If
		'        i = i + 1
		'    Next
		'    Do While i <= num
		'        Input #SaveDataFileNumber, buf
		'        i = i + 1
		'    Loop
		Dim label_enabled(num) As Object
		For i = 1 To num
			Input(SaveDataFileNumber, label_enabled(i))
		Next 
		' MOD END MARGE
		
		'Requireコマンドで追加されたイベントファイル
		If SaveDataVersion > 20003 Then
			file_head = UBound(EventData) + 1
			
			' MOD START MARGE
			'Invalid_string_refer_to_original_code
			'        Input #SaveDataFileNumber, num
			'        If num = 0 Then
			'            Exit Sub
			'        End If
			'        ReDim AdditionalEventFileNames(num)
			'        For i = 1 To num
			'            Input #SaveDataFileNumber, fname
			'            AdditionalEventFileNames(i) = fname
			'            If InStr(fname, ":") = 0 Then
			'                fname = ScenarioPath & fname
			'            End If
			'
			'Invalid_string_refer_to_original_code
			'            For j = 1 To UBound(EventFileNames)
			'               If fname = EventFileNames(j) Then
			'                   GoTo NextEventFile
			'               End If
			'            Next
			'
			'            LoadEventData2 fname, UBound(EventData)
			'NextEventFile:
			'        Next
			'
			'        'エラー表示用にサイズを大きく取っておく
			'        ReDim Preserve EventData(UBound(EventData) + 1)
			'        ReDim Preserve EventLineNum(UBound(EventData))
			'        EventData(UBound(EventData)) = ""
			'        EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
			'
			'Invalid_string_refer_to_original_code
			'        For i = file_head To UBound(EventData) - 1
			'            If Right$(EventData(i), 1) = "_" Then
			'                EventData(i + 1) = _
			''                    Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
			'                EventData(i) = " "
			'            End If
			'        Next
			'
			'        'ラベルを登録
			'        For i = file_head To UBound(EventData)
			'            buf = EventData(i)
			'            If Right$(buf, 1) = ":" Then
			'                AddLabel Left$(buf, Len(buf) - 1), i
			'            End If
			'        Next
			'
			'Invalid_string_refer_to_original_code
			'        If UBound(EventData) > UBound(EventCmd) Then
			'            ReDim Preserve EventCmd(UBound(EventData))
			'            i = UBound(EventData)
			'            Do While EventCmd(i) Is Nothing
			'                Set EventCmd(i) = New CmdData
			'                EventCmd(i).LineNum = i
			'                i = i - 1
			'            Loop
			'        End If
			'        For i = file_head To UBound(EventData)
			'            EventCmd(i).Name = NullCmd
			'        Next
			'    End If
			'追加するイベントファイル数
			Input(SaveDataFileNumber, num)
			
			If num > 0 Then
				'Invalid_string_refer_to_original_code
				ReDim AdditionalEventFileNames(num)
				For i = 1 To num
					Input(SaveDataFileNumber, fname)
					AdditionalEventFileNames(i) = fname
					If InStr(fname, ":") = 0 Then
						fname = ScenarioPath & fname
					End If
					
					'Invalid_string_refer_to_original_code
					For j = 1 To UBound(EventFileNames)
						If fname = EventFileNames(j) Then
							GoTo NextEventFile
						End If
					Next 
					
					LoadEventData2(fname, UBound(EventData))
NextEventFile: 
				Next 
				
				'エラー表示用にサイズを大きく取っておく
				ReDim Preserve EventData(UBound(EventData) + 1)
				ReDim Preserve EventLineNum(UBound(EventData))
				EventData(UBound(EventData)) = ""
				EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
				
				'Invalid_string_refer_to_original_code
				For i = file_head To UBound(EventData) - 1
					If Right(EventData(i), 1) = "_" Then
						EventData(i + 1) = Left(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
						EventData(i) = " "
					End If
				Next 
				
				'ラベルを登録
				For i = file_head To UBound(EventData)
					buf = EventData(i)
					If Right(buf, 1) = ":" Then
						AddLabel(Left(buf, Len(buf) - 1), i)
					End If
				Next 
				
				'Invalid_string_refer_to_original_code
				If UBound(EventData) > UBound(EventCmd) Then
					ReDim Preserve EventCmd(UBound(EventData))
					i = UBound(EventData)
					Do While EventCmd(i) Is Nothing
						EventCmd(i) = New CmdData
						EventCmd(i).LineNum = i
						i = i - 1
					Loop 
				End If
				For i = file_head To UBound(EventData)
					EventCmd(i).Name = CmdType.NullCmd
				Next 
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		i = 1
		num = UBound(label_enabled)
		For	Each lab In colEventLabelList
			If i <= num Then
				'UPGRADE_WARNING: �I�u�W�F�N�g label_enabled(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				lab.Enable = label_enabled(i)
			Else
				lab.Enable = True
			End If
			i = i + 1
		Next lab
		' MOD END MARGE
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub SkipEventData()
		Dim i, num As Short
		Dim dummy As String
		
		'グローバル変数
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		'ローカル変数
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			dummy = LineInput(SaveDataFileNumber)
		Next 
		
		'Requireコマンドで読み込んだイベントデータ
		If SaveDataVersion > 20003 Then
			Input(SaveDataFileNumber, num)
			For i = 1 To num
				dummy = LineInput(SaveDataFileNumber)
			Next 
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveGlobalVariables()
		Dim var As VarData
		
		WriteLine(SaveDataFileNumber, GlobalVariableList.Count())
		For	Each var In GlobalVariableList
			With var
				If .VariableType = Expression.ValueType.StringType Then
					WriteLine(SaveDataFileNumber, .Name, .StringValue)
				Else
					WriteLine(SaveDataFileNumber, .Name, VB6.Format(.NumericValue))
				End If
			End With
		Next var
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadGlobalVariables()
		Dim num, j, i, k, idx As Short
		Dim vvalue, vname, buf As String
		Dim aname As String
		' ADD START MARGE
		Dim is_number As Boolean
		' ADD END MARGE
		'Invalid_string_refer_to_original_code
		With GlobalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num)
		
		'Invalid_string_refer_to_original_code
		Dim vname2 As String
		For i = 1 To num
			Input(SaveDataFileNumber, vname)
			buf = LineInput(SaveDataFileNumber)
			' MOD START MARGE
			'        vvalue = Mid$(buf, 2, Len(buf) - 2)
			'        ReplaceString vvalue, """""", """"
			If Left(buf, 1) = """" Then
				is_number = False
				vvalue = Mid(buf, 2, Len(buf) - 2)
				ReplaceString(vvalue, """""", """")
			Else
				is_number = True
				vvalue = buf
			End If
			' MOD END MARGE
			
			If SaveDataVersion < 10724 Then
				'Invalid_string_refer_to_original_code
				If Left(vname, 8) = "Ability(" Then
					idx = InStr(vname, ",")
					If idx > 0 Then
						'Invalid_string_refer_to_original_code
						aname = Mid(vname, idx + 1, Len(vname) - idx - 1)
						If ALDList.IsDefined(aname) Then
							vname = Left(vname, idx) & ALDList.Item(aname).AliasType(1) & ")"
							If LLength(vvalue) = 1 Then
								vvalue = vvalue & " " & aname
							End If
						End If
					Else
						'Invalid_string_refer_to_original_code
						buf = ""
						For j = 1 To LLength(vvalue)
							aname = LIndex(vvalue, j)
							If ALDList.IsDefined(aname) Then
								aname = ALDList.Item(aname).AliasType(1)
							End If
							buf = buf & " " & aname
						Next 
						vvalue = Trim(buf)
					End If
				End If
			End If
			
			If SaveDataVersion < 10730 Then
				'Invalid_string_refer_to_original_code
				If Left(vname, 8) = "Ability(" Then
					idx = InStr(vname, ",")
					If idx > 0 Then
						vname2 = Left(vname, idx - 1) & ")"
						aname = Mid(vname, idx + 1, Len(vname) - idx - 1)
						If Not IsGlobalVariableDefined(vname2) Then
							DefineGlobalVariable(vname2)
							'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
							GlobalVariableList.Item(vname2).StringValue = aname
						End If
					End If
				End If
			End If
			
			If SaveDataVersion < 10731 Then
				'Invalid_string_refer_to_original_code
				If Left(vname, 8) = "Ability(" Then
					If Right(vname, 5) = ",非表示)" Then
						GoTo NextVariable
					End If
				End If
			End If
			
			If SaveDataVersion < 10732 Then
				'Invalid_string_refer_to_original_code
				If Left(vname, 8) = "Ability(" Then
					If InStr(vname, ",") = 0 Then
						buf = ""
						For j = 1 To LLength(vvalue)
							aname = LIndex(vvalue, j)
							If aname <> "非表示" Then
								For k = 1 To LLength(buf)
									If LIndex(buf, k) = aname Then
										Exit For
									End If
								Next 
								If k > LLength(buf) Then
									buf = buf & " " & aname
								End If
							End If
						Next 
						vvalue = Trim(buf)
					End If
				End If
			End If
			
			If SaveDataVersion < 20027 Then
				'Invalid_string_refer_to_original_code
				If Left(vname, 8) = "Ability(" Then
					If LIndex(vvalue, 1) = "0" Then
						If LIndex(vvalue, 2) = "解説" Then
							vvalue = VB6.Format(DEFAULT_LEVEL) & " 解説 " & ListTail(vvalue, 3)
						End If
					End If
				End If
			End If
			
			If Not IsGlobalVariableDefined(vname) Then
				DefineGlobalVariable(vname)
			End If
			With GlobalVariableList.Item(vname)
				'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				.StringValue = vvalue
				' MOD START MARGE
				'            If IsNumber(vvalue) Then
				If is_number Then
					' MOD END MARGE
					'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().VariableType �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.VariableType = Expression.ValueType.NumericType
					'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.NumericValue = CDbl(vvalue)
				Else
					'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().VariableType �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.VariableType = Expression.ValueType.StringType
				End If
			End With
NextVariable: 
		Next 
		'ADD START 240a
		'Invalid_string_refer_to_original_code
		SetNewGUIMode()
		'ADD  END  240a
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveLocalVariables()
		Dim var As VarData
		
		WriteLine(SaveDataFileNumber, LocalVariableList.Count())
		For	Each var In LocalVariableList
			With var
				If .VariableType = Expression.ValueType.StringType Then
					WriteLine(SaveDataFileNumber, .Name, .StringValue)
				Else
					WriteLine(SaveDataFileNumber, .Name, VB6.Format(.NumericValue))
				End If
				If InStr(.Name, """") > 0 Then
					ErrorMessage(.Name)
				End If
			End With
		Next var
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadLocalVariables()
		Dim i, num As Short
		' MOD START MARGE
		'Dim vname As String, vvalue As String
		Dim vvalue, vname, buf As String
		Dim is_number As Boolean
		' MOD END MARGE
		'Invalid_string_refer_to_original_code
		With LocalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'Invalid_string_refer_to_original_code
			' MOD START MARGE
			'        Input #SaveDataFileNumber, vname, vvalue
			Input(SaveDataFileNumber, vname)
			buf = LineInput(SaveDataFileNumber)
			If Left(buf, 1) = """" Then
				is_number = False
				vvalue = Mid(buf, 2, Len(buf) - 2)
				ReplaceString(vvalue, """""", """")
			Else
				is_number = True
				vvalue = buf
			End If
			' MOD END MARGE
			
			If SaveDataVersion < 10731 Then
				'ClearSkillのバグで設定された変数を削除
				If Left(vname, 8) = "Ability(" Then
					If vname = vvalue Then
						GoTo NextVariable
					End If
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If Not IsLocalVariableDefined(vname) Then
				DefineLocalVariable(vname)
			End If
			With LocalVariableList.Item(vname)
				'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				.StringValue = vvalue
				' MOD START MARGE
				'            If IsNumber(vvalue) Then
				If is_number Then
					' MOD END MARGE
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().VariableType �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.VariableType = Expression.ValueType.NumericType
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.NumericValue = CDbl(vvalue)
				Else
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().VariableType �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					.VariableType = Expression.ValueType.StringType
				End If
			End With
NextVariable: 
		Next 
	End Sub
	
	
	'イベントエラー表示
	Public Sub DisplayEventErrorMessage(ByVal lnum As Integer, ByVal msg As String)
		Dim buf As String
		
		'Invalid_string_refer_to_original_code
		buf = EventFileNames(EventFileID(lnum)) & "Invalid_string_refer_to_original_code"
		& EventLineNum(lnum) & "行目" & vbCr & vbLf _
		& msg & vbCr & vbLf
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		'Invalid_string_refer_to_original_code
		If lnum > 1 Then
			buf = buf & EventLineNum(lnum - 1) & ": " & EventData(lnum - 1) & vbCr & vbLf
		End If
		buf = buf & EventLineNum(lnum) & ": " & EventData(lnum) & vbCr & vbLf
		If lnum < UBound(EventData) Then
			buf = buf & EventLineNum(lnum + 1) & ": " & EventData(lnum + 1) & vbCr & vbLf
		End If
		
		ErrorMessage(buf)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub MakeUnitList(Optional ByRef smode As String = "")
		Dim u As Unit
		Dim p As Pilot
		Dim xx, yy As Short
		Dim key_list() As Integer
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim unit_list() As Unit
		Dim i, j As Short
		Static key_type As String
		
		'Invalid_string_refer_to_original_code
		If smode <> "" Then
			key_type = smode
		End If
		If key_type = "" Then
			key_type = "Invalid_string_refer_to_original_code"
		End If
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'あらかじめ撤退させておく
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				.Escape()
				'End If
			End With
		Next u
		
		'マップをクリア
		LoadMapData("")
		SetupBackground("", "Invalid_string_refer_to_original_code")
		
		'Invalid_string_refer_to_original_code
		If key_type <> "名称" Then
			'Invalid_string_refer_to_original_code
			ReDim unit_list(UList.Count)
			ReDim key_list(UList.Count)
			i = 0
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					i = i + 1
					unit_list(i) = u
					
					'Invalid_string_refer_to_original_code
					Select Case key_type
						Case "ランク"
							key_list(i) = .Rank
						Case "Invalid_string_refer_to_original_code"
							key_list(i) = .HP
						Case "Invalid_string_refer_to_original_code"
							key_list(i) = .EN
						Case "Invalid_string_refer_to_original_code"
							key_list(i) = .Armor
						Case "運動性"
							key_list(i) = .Mobility
						Case "移動力"
							key_list(i) = .Speed
						Case "Invalid_string_refer_to_original_code"
							For j = 1 To .CountWeapon
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								If .WeaponPower(j, "") > key_list(i) Then
									key_list(i) = .WeaponPower(j, "")
								End If
							Next 
					End Select
				End With
			Next u
		End If
		'Next
		'UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'Case "Invalid_string_refer_to_original_code"
			''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
			'Case "レベル"
				''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
				'Case "Invalid_string_refer_to_original_code"
					''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
					'Case "Invalid_string_refer_to_original_code"
						''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
						'Case "Invalid_string_refer_to_original_code"
							''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
							'Case "命中"
								''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
								'Case "回避"
									''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
									'Case "Invalid_string_refer_to_original_code"
										''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
										'Case "Invalid_string_refer_to_original_code"
											''UPGRADE_WARNING: MakeUnitList �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
											'End Select
											'End If
											'End With
											'Next
											''ReDim Preserve unit_list(i)
											''ReDim Preserve key_list(i)
											'
											'Invalid_string_refer_to_original_code
											'For 'i = 1 To UBound(key_list) - 1
												'max_item = i
												'max_value = key_list(i)
												'For 'j = i + 1 To UBound(unit_list)
													'If key_list(j) > max_value Then
														'max_item = j
														'max_value = key_list(j)
													'End If
												'Next 
												'If max_item <> i Then
													'u = unit_list(i)
													'unit_list(i) = unit_list(max_item)
													'unit_list(max_item) = u
													'
													'max_value = key_list(max_item)
													'key_list(max_item) = key_list(i)
													'key_list(i) = max_value
												'End If
											'Next 
											'Invalid_string_refer_to_original_code
											''ReDim unit_list(UList.Count)
											'Dim strkey_list(UList.Count) As Object
											'i = 0
											'For	Each u In UList
												'With u
													'Invalid_string_refer_to_original_code
													'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
													'i = i + 1
													'unit_list(i) = u
													'Invalid_string_refer_to_original_code
													'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
													''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
													'strkey_list(i) = .MainPilot.KanaName
													''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
													'strkey_list(i) = .KanaName
													'End If
													'End If
												'End With
											'Next u
											''ReDim Preserve unit_list(i)
											''ReDim Preserve strkey_list(i)
											'
											'Invalid_string_refer_to_original_code
											'For 'i = 1 To UBound(strkey_list) - 1
												'max_item = i
												''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
												'max_str = strkey_list(i)
												'For 'j = i + 1 To UBound(strkey_list)
													''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
													'If StrComp(strkey_list(j), max_str, 1) = -1 Then
														'max_item = j
														''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
														'max_str = strkey_list(j)
													'End If
												'Next 
												'If max_item <> i Then
													'u = unit_list(i)
													'unit_list(i) = unit_list(max_item)
													'unit_list(max_item) = u
													'
													''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
													''UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(max_item) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
													'strkey_list(max_item) = strkey_list(i)
												'End If
											'Next 
											'End If
											'
											'Font Regular 9pt 背景
											''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
											'With MainForm.picMain(0).Font
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Size = 9
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Bold = False
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Italic = False
											'End With
											'PermanentStringMode = True
											'HCentering = False
											'VCentering = False
											'
											'Invalid_string_refer_to_original_code
											'xx = 1
											'yy = 1
											'For 'i = 1 To UBound(unit_list)
												'u = unit_list(i)
												'With u
													'Invalid_string_refer_to_original_code
													'If xx > 15 Then
														'xx = 1
														'yy = yy + 1
														'If yy > 40 Then
															'Invalid_string_refer_to_original_code
															'Exit For
														'End If
													'End If
													'
													'Invalid_string_refer_to_original_code
													'If .CountPilot = 0 Then
														'p = PList.Add("Invalid_string_refer_to_original_code", 1, "味方")
														'p.Ride(u)
													'End If
													'
													'Invalid_string_refer_to_original_code
													'.UsedAction = 0
													'.StandBy(xx, yy)
													'
													'Invalid_string_refer_to_original_code
													'.AddCondition("Invalid_string_refer_to_original_code")
													'
													'Invalid_string_refer_to_original_code
													'DrawString(.Nickname, 32 * xx + 2, 32 * yy - 31)
													'
													'Invalid_string_refer_to_original_code
													'Select Case key_type
														'Case "ランク"
															'DrawString("RK" & VB6.Format(key_list(i)) & " " & Term("HP", u) & VB6.Format(.HP) & " " & Term("EN", u) & VB6.Format(.EN), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "名称"
															'DrawString(Term("HP", u) & VB6.Format(.HP) & " " & Term("EN", u) & VB6.Format(.EN), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'DrawString(Term("Invalid_string_refer_to_original_code", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "運動性"
															'DrawString(Term("運動性", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "移動力"
															'DrawString(Term("移動力", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'DrawString("Invalid_string_refer_to_original_code" & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'DrawString("Invalid_string_refer_to_original_code")
															'32 * xx + 2, 32 * yy - 15
															'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
														'Case "レベル"
															'DrawString("Lv" & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'DrawString(Term("SP", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'Invalid_string_refer_to_original_code_
															'32 * xx + 2, 32 * yy - 15
															'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
														'Case "Invalid_string_refer_to_original_code"
															'If .MainPilot.HasMana() Then
																'DrawString(Term("魔力", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
															'Else
																'DrawString(Term("Invalid_string_refer_to_original_code", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
															'End If
														'Case "命中"
															'DrawString(Term("命中", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "回避"
															'DrawString(Term("回避", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
														'Case "Invalid_string_refer_to_original_code"
															'Invalid_string_refer_to_original_code_
															'32 * xx + 2, 32 * yy - 15
															'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
														'Case "Invalid_string_refer_to_original_code"
															'Invalid_string_refer_to_original_code_
															'32 * xx + 2, 32 * yy - 15
															'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
													'End Select
													'
													'Invalid_string_refer_to_original_code
													'xx = xx + 5
												'End With
											'Next 
											'
											'Invalid_string_refer_to_original_code
											''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
											'With MainForm.picMain(0).Font
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Size = 16
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Bold = True
												''UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
												'.Italic = False
											'End With
											'PermanentStringMode = False
											'
											'RedrawScreen()
											'
											'Invalid_string_refer_to_original_code
											''UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
											'System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveBasePoint()
		BasePointIndex = BasePointIndex + 1
		If BasePointIndex > UBound(SavedBaseX) Then
			BasePointIndex = 0
		End If
		SavedBaseX(BasePointIndex) = BaseX
		SavedBaseY(BasePointIndex) = BaseY
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreBasePoint()
		If BasePointIndex <= 0 Then
			BasePointIndex = UBound(SavedBaseX)
		End If
		BaseX = SavedBaseX(BasePointIndex)
		BaseY = SavedBaseY(BasePointIndex)
		BasePointIndex = BasePointIndex - 1
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ResetBasePoint()
		Dim i As Short
		
		BaseX = 0
		BaseY = 0
		BasePointIndex = 0
		For i = 1 To UBound(SavedBaseX)
			SavedBaseX(i) = 0
			SavedBaseY(i) = 0
		Next 
	End Sub
End Module