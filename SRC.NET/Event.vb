Option Strict Off
Option Explicit On
'UPGRADE_NOTE: Event は Event_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
Module Event_Renamed
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'イベントデータの各種処理を行うモジュール
	
	'イベントデータ
	Public EventData() As String
	'イベントコマンドリスト
	Public EventCmd() As CmdData
	'個々の行がどのイベントファイルに属しているか
	Public EventFileID() As Short
	'個々の行がイベントファイルの何行目に位置するか
	Public EventLineNum() As Short
	'イベントファイルのファイル名リスト
	Public EventFileNames() As String
	'Requireコマンドで追加されたイベントファイルのファイル名リスト
	Public AdditionalEventFileNames() As String
	
	'システム側のイベントデータのサイズ(行数)
	Private SysEventDataSize As Integer
	'システム側のイベントファイル数
	Private SysEventFileNum As Short
	'シナリオ添付のシステムファイルがチェックされたかどうか
	Private ScenarioLibChecked As Boolean
	
	'ラベルのリスト
	Public colEventLabelList As New Collection
	Private colSysNormalLabelList As New Collection
	Private colNormalLabelList As New Collection
	
	
	'変数用のコレクション
	Public GlobalVariableList As New Collection
	Public LocalVariableList As New Collection
	
	'現在の行番号
	Public CurrentLineNum As Integer
	
	'イベントで選択されているユニット・ターゲット
	Public SelectedUnitForEvent As Unit
	Public SelectedTargetForEvent As Unit
	
	'イベント呼び出しのキュー
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
	'引数スタック
	Public ArgIndex As Short
	Public ArgIndexStack(MaxCallDepth) As Short
	Public ArgStack(MaxArgIndex) As String
	'UpVarコマンドによって引数が何段階シフトしているか
	Public UpVarLevel As Short
	Public UpVarLevelStack(MaxCallDepth) As Short
	'サブルーチンローカル変数スタック
	Public VarIndex As Short
	Public VarIndexStack(MaxCallDepth) As Short
	Public VarStack(MaxVarIndex) As VarData
	'Forインデックス用スタック
	Public ForIndex As Short
	Public ForIndexStack(MaxCallDepth) As Short
	Public ForLimitStack(MaxCallDepth) As Integer
	
	'ForEachコマンド用変数
	Public ForEachIndex As Short
	Public ForEachSet() As String
	
	'Rideコマンド用パイロット搭乗履歴
	Public LastUnitName As String
	Public LastPilotID() As String
	
	'Wait開始時刻
	Public WaitStartTime As Integer
	Public WaitTimeCount As Integer
	
	'描画基準座標
	Public BaseX As Integer
	Public BaseY As Integer
	Private SavedBaseX(10) As Integer
	Private SavedBaseY(10) As Integer
	Private BasePointIndex As Integer
	
	'オブジェクトの色
	Public ObjColor As Integer
	'オブジェクトの線の太さ
	Public ObjDrawWidth As Integer
	'オブジェクトの背景色
	Public ObjFillColor As Integer
	'オブジェクトの背景描画方法
	Public ObjFillStyle As Integer
	'オブジェクトの描画方法
	Public ObjDrawOption As String
	
	'ホットポイント
	Public Structure HotPoint
		Dim Name As String
		'UPGRADE_NOTE: Left は Left_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim Left_Renamed As Short
		Dim Top As Short
		Dim width As Short
		Dim Height As Short
		Dim Caption As String
	End Structure
	Public HotPointList() As HotPoint
	
	'イベントコマンドエラーメッセージ
	Public EventErrorMessage As String
	
	'ユニットがセンタリングされたか？
	Public IsUnitCenter As Boolean
	
	
	'イベントコマンドの種類
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
	
	'イベントラベルの種類
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
	
	
	'イベントデータを初期化
	Public Sub InitEventData()
		Dim i As Integer
		
		ReDim Titles(0)
		ReDim EventData(0)
		ReDim EventCmd(50000)
		ReDim EventQue(0)
		
		'オブジェクトの生成には時間がかかるので、
		'あらかじめCmdDataオブジェクトを生成しておく。
		For i = 1 To UBound(EventCmd)
			EventCmd(i) = New CmdData
			EventCmd(i).LineNum = i
		Next 
		
		'本体側のシナリオデータをチェックする
		LoadEventData("", "システム")
	End Sub
	
	'イベントファイルのロード
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
		
		'データの初期化
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
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'ラベルの初期化
		With colNormalLabelList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		i = 1
		With colEventLabelList
			Do While i <= .Count()
				'UPGRADE_WARNING: オブジェクト colEventLabelList.Item(i).LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .Item(i).LineNum > SysEventDataSize Then
					.Remove(i)
				Else
					i = i + 1
				End If
			Loop 
		End With
		
		'デバッグモードの設定
		If LCase(ReadIni("Option", "DebugMode")) = "on" Then
			If Not IsOptionDefined("デバッグ") Then
				DefineGlobalVariable("Option(デバッグ)")
			End If
			SetVariableAsLong("Option(デバッグ)", 1)
		End If
		
		'システム側のイベントデータのロード
		If load_mode = "システム" Then
			'本体側のシステムデータをチェック
			
			'スペシャルパワーアニメ用インクルードファイルをダウンロード
			If FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then
				LoadEventData2(ExtDataPath & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then 
				LoadEventData2(ExtDataPath2 & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then 
				LoadEventData2(AppPath & "Lib\スペシャルパワー.eve")
			ElseIf FileExists(ExtDataPath & "Lib\精神コマンド.eve") Then 
				LoadEventData2(ExtDataPath & "Lib\精神コマンド.eve")
			ElseIf FileExists(ExtDataPath2 & "Lib\精神コマンド.eve") Then 
				LoadEventData2(ExtDataPath2 & "Lib\精神コマンド.eve")
			ElseIf FileExists(AppPath & "Lib\精神コマンド.eve") Then 
				LoadEventData2(AppPath & "Lib\精神コマンド.eve")
			End If
			
			'汎用戦闘アニメ用インクルードファイルをダウンロード
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
				'戦闘アニメ表示切り替えコマンドを非表示に
				BattleAnimation = False
			End If
			
			'システム側のイベントデータの総行数＆ファイル数を記録しておく
			sys_event_data_size = UBound(EventData)
			sys_event_file_num = UBound(EventFileNames)
		ElseIf Not ScenarioLibChecked Then 
			'シナリオ側のシステムデータをチェック
			
			ScenarioLibChecked = True
			
			If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") Or FileExists(ScenarioPath & "Lib\精神コマンド.eve") Or FileExists(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve") Then
				'システムデータのロードをやり直す
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
				
				'スペシャルパワーアニメ用インクルードファイルをダウンロード
				If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") Then
					LoadEventData2(ScenarioPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ScenarioPath & "Lib\精神コマンド.eve") Then 
					LoadEventData2(ScenarioPath & "Lib\精神コマンド.eve")
				ElseIf FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(ExtDataPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(ExtDataPath2 & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then 
					LoadEventData2(AppPath & "Lib\スペシャルパワー.eve")
				ElseIf FileExists(ExtDataPath & "Lib\精神コマンド.eve") Then 
					LoadEventData2(ExtDataPath & "Lib\精神コマンド.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\精神コマンド.eve") Then 
					LoadEventData2(ExtDataPath2 & "Lib\精神コマンド.eve")
				ElseIf FileExists(AppPath & "Lib\精神コマンド.eve") Then 
					LoadEventData2(AppPath & "Lib\精神コマンド.eve")
				End If
				
				'汎用戦闘アニメ用インクルードファイルをダウンロード
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
					'戦闘アニメ表示切り替えコマンドを非表示に
					BattleAnimation = False
				End If
			End If
			
			'シナリオ添付の汎用インクルードファイルをダウンロード
			If FileExists(ScenarioPath & "Lib\include.eve") Then
				LoadEventData2(ScenarioPath & "Lib\include.eve")
			End If
			
			'システム側のイベントデータの総行数＆ファイル数を記録しておく
			sys_event_data_size = UBound(EventData)
			sys_event_file_num = UBound(EventFileNames)
			
			'シナリオ側のイベントデータのロード
			LoadEventData2(fname)
		Else
			'シナリオ側のイベントデータのロード
			LoadEventData2(fname)
		End If
		
		'エラー表示用にサイズを大きく取っておく
		ReDim Preserve EventData(UBound(EventData) + 1)
		ReDim Preserve EventLineNum(UBound(EventData))
		EventData(UBound(EventData)) = ""
		EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
		
		'データ読みこみ指定
		For i = SysEventDataSize + 1 To UBound(EventData)
			If Left(EventData(i), 1) = "@" Then
				tname = Mid(EventData(i), 2)
				
				'既にそのデータが読み込まれているかチェック
				For j = 1 To UBound(Titles)
					If tname = Titles(j) Then
						Exit For
					End If
				Next 
				
				If j > UBound(Titles) Then
					'フォルダを検索
					tfolder = SearchDataFolder(tname)
					If Len(tfolder) = 0 Then
						DisplayEventErrorMessage(i, "データ「" & tname & "」のフォルダが見つかりません")
					Else
						ReDim Preserve new_titles(UBound(new_titles) + 1)
						ReDim Preserve Titles(UBound(Titles) + 1)
						new_titles(UBound(new_titles)) = tname
						Titles(UBound(Titles)) = tname
					End If
				End If
			End If
		Next 
		
		'各作品データのinclude.eveを読み込む
		If load_mode <> "システム" Then
			'作品毎のインクルードファイル
			For i = 1 To UBound(Titles)
				tfolder = SearchDataFolder(Titles(i))
				If FileExists(tfolder & "\include.eve") Then
					LoadEventData2(tfolder & "\include.eve")
				End If
			Next 
			
			'汎用Dataインクルードファイルをロード
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
		
		'複数行に分割されたコマンドを結合
		For i = SysEventDataSize + 1 To UBound(EventData) - 1
			If Right(EventData(i), 1) = "_" Then
				EventData(i + 1) = Left(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
				EventData(i) = " "
			End If
		Next 
		
		'ラベルの登録
		num = CurrentLineNum
		If load_mode = "システム" Then
			For CurrentLineNum = 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				If Right(buf, 1) = ":" Then
					AddSysLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
				End If
			Next 
		ElseIf sys_event_data_size > 0 Then 
			'システム側へのイベントデータの追加があった場合
			For CurrentLineNum = 1 To sys_event_data_size
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddSysLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "："
						DisplayEventErrorMessage(CurrentLineNum, "ラベルの末尾が全角文字になっています")
						error_found = True
				End Select
			Next 
			For CurrentLineNum = sys_event_data_size + 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "："
						DisplayEventErrorMessage(CurrentLineNum, "ラベルの末尾が全角文字になっています")
						error_found = True
				End Select
			Next 
		Else
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				buf = EventData(CurrentLineNum)
				Select Case Right(buf, 1)
					Case ":"
						AddLabel(Left(buf, Len(buf) - 1), CurrentLineNum)
					Case "："
						DisplayEventErrorMessage(CurrentLineNum, "ラベルの末尾が全角文字になっています")
						error_found = True
				End Select
			Next 
		End If
		CurrentLineNum = num
		
		'コマンドデータ配列を設定
		If UBound(EventData) > UBound(EventCmd) Then
			num = UBound(EventCmd)
			ReDim Preserve EventCmd(UBound(EventData))
			For i = num + 1 To UBound(EventCmd)
				EventCmd(i) = New CmdData
				EventCmd(i).LineNum = i
			Next 
		End If
		
		'書式チェックはシナリオ側にのみ実施
		If load_mode <> "システム" Then
			
			'構文解析と書式チェックその１
			'制御構造
			CmdStackIdx = 0
			CmdPosStackIdx = 0
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				If EventCmd(CurrentLineNum) Is Nothing Then
					EventCmd(CurrentLineNum) = New CmdData
					EventCmd(CurrentLineNum).LineNum = CurrentLineNum
				End If
				With EventCmd(CurrentLineNum)
					'コマンドの構文解析
					If Not .Parse(EventData(CurrentLineNum)) Then
						error_found = True
					End If
					
					'リスト長がマイナスのときは括弧の対応が取れていない
					If .ArgNum = -1 Then
						Select Case CmdStack(CmdStackIdx)
							Case CmdType.AskCmd, CmdType.AutoTalkCmd, CmdType.QuestionCmd, CmdType.TalkCmd
								'これらのコマンドの入力の場合は無視する
							Case Else
								DisplayEventErrorMessage(CurrentLineNum, "括弧の対応が取れていません")
								error_found = True
						End Select
					End If
					
					'コマンドに応じて制御構造をチェック
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
									Case "終了"
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
									Case "終了"
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
								'"_"で消去された行。Talk中の改行に対応するためのダミーの空白
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
											'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
											'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
			
			'ファイルの末尾まで読んでもコマンドの終わりがなかった？
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
			
			'書式エラーが見つかった場合はSRCを終了
			If error_found Then
				TerminateSRC()
			End If
			
			'書式チェックその２
			'主なコマンドの引数の数をチェック
			For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
				With EventCmd(CurrentLineNum)
					Select Case .Name
						Case CmdType.CreateCmd
							If .ArgNum < 8 Then
								DisplayEventErrorMessage(CurrentLineNum, "Createコマンドのパラメータ数が違います")
								error_found = True
							End If
						Case CmdType.PilotCmd
							If .ArgNum < 3 Then
								DisplayEventErrorMessage(CurrentLineNum, "Pilotコマンドのパラメータ数が違います")
								error_found = True
							End If
						Case CmdType.UnitCmd
							If .ArgNum <> 3 Then
								DisplayEventErrorMessage(CurrentLineNum, "Unitコマンドのパラメータ数が違います")
								error_found = True
							End If
					End Select
				End With
			Next 
			
			'書式エラーが見つかった場合はSRCを終了
			If error_found Then
				TerminateSRC()
			End If
			
			'シナリオ側のイベントデータの場合はここまでスキップ
		Else
			
			'システム側のイベントデータの場合の処理
			
			'CmdDataクラスのインスタンスの生成のみ行っておく
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
		
		'イベントデータの読み込みが終了したのでシステム側イベントデータのサイズを決定。
		'システム側イベントデータは読み込みを一度だけやればよい。
		If sys_event_data_size > 0 Then
			SysEventDataSize = sys_event_data_size
			SysEventFileNum = sys_event_file_num
		End If
		
		'クイックロードやリスタートの場合はシナリオデータの再ロードのみ
		Select Case load_mode
			Case "リストア"
				ADList.AddDefaultAnimation()
				Exit Sub
			Case "システム", "クイックロード", "リスタート"
				Exit Sub
		End Select
		
		'追加されたシステム側イベントデータをチェックする場合はここで終了
		If fname = "" Then
			Exit Sub
		End If
		
		'ロードするデータ数をカウント
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
			'デフォルトの戦闘アニメデータを設定
			ADList.AddDefaultAnimation()
			Exit Sub
		End If
		
		'ロード画面を表示
		OpenNowLoadingForm()
		
		'ロードサイズを設定
		SetLoadImageSize(num)
		
		'使用しているタイトルのデータをロード
		For i = 1 To UBound(new_titles)
			IncludeData(new_titles(i))
		Next 
		
		'ローカルデータの読みこみ
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
		
		'デフォルトの戦闘アニメデータを設定
		ADList.AddDefaultAnimation()
		
		'マップデータをロード
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
		
		'ファイルを開く
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		'行番号の設定
		If lnum > 0 Then
			CurrentLineNum = lnum
		End If
		CurrentLineNum2 = 0
		
		'各行の読み込み
		Do Until EOF(FileNumber)
			CurrentLineNum = CurrentLineNum + 1
			CurrentLineNum2 = CurrentLineNum2 + 1
			
			'データ領域確保
			ReDim Preserve EventData(CurrentLineNum)
			ReDim Preserve EventFileID(CurrentLineNum)
			ReDim Preserve EventLineNum(CurrentLineNum)
			
			'行の読み込み
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
							'シングルクオート
							If Not in_double_quote Then
								in_single_quote = Not in_single_quote
							End If
						Case """"
							'ダブルクオート
							If Not in_single_quote Then
								in_double_quote = Not in_double_quote
							End If
						Case "/"
							'コメント？
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
			
			'行を保存
			EventData(CurrentLineNum) = buf
			EventFileID(CurrentLineNum) = fid
			EventLineNum(CurrentLineNum) = CurrentLineNum2
			
			'他のイベントファイルの読み込み
			If Left(buf, 1) = "<" Then
				If InStr(buf, ">") = Len(buf) And buf <> "<>" Then
					CurrentLineNum = CurrentLineNum - 1
					fname2 = Mid(buf, 2, Len(buf) - 2)
					If fname2 <> "Lib\スペシャルパワー.eve" And fname2 <> "Lib\汎用戦闘アニメ\include.eve" And fname2 <> "Lib\include.eve" Then
						'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
						If Len(Dir(ScenarioPath & fname2)) > 0 Then
							LoadEventData2(ScenarioPath & fname2)
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
						ElseIf Len(Dir(ExtDataPath & fname2)) > 0 Then 
							LoadEventData2(ExtDataPath & fname2)
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
						ElseIf Len(Dir(ExtDataPath2 & fname2)) > 0 Then 
							LoadEventData2(ExtDataPath2 & fname2)
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
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
			ErrorMessage(fname & "のロード中にエラーが発生しました" & vbCr & VB6.Format(CurrentLineNum2) & "行目のイベントデータが不正です")
		End If
		TerminateSRC()
	End Sub
	
	
	'イベントの実行
	'UPGRADE_WARNING: ParamArray Args が ByRef から ByVal に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' をクリックしてください。
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
		
		'画面入力をロック
		prev_is_gui_locked = IsGUILocked
		If Not IsGUILocked Then
			LockGUI()
		End If
		
		'現在選択されているユニット＆ターゲットをイベント用に設定
		'(SearchLabel()実行時の式計算用にあらかじめ設定しておく)
		SelectedUnitForEvent = SelectedUnit
		'引数に指定されたユニットを優先
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
		
		'イベントキューを作成
		ReDim Preserve EventQue(UBound(EventQue) + 1)
		event_que_idx = UBound(EventQue)
		Select Case Args(0)
			Case "プロローグ"
				EventQue(UBound(EventQue)) = "プロローグ"
				Stage = "プロローグ"
			Case "エピローグ"
				EventQue(UBound(EventQue)) = "エピローグ"
				Stage = "エピローグ"
			Case "破壊"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "破壊 " & Args(1)
				With PList.Item(Args(1))
					uparty = .Party
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'格納されていたユニットも破壊しておく
							' MOD START MARGE
							'                        For i = 1 To .CountUnitOnBoard
							'                            Set u = .UnitOnBoard(1)
							'                            .UnloadUnit u.ID
							'                            u.Status = "破壊"
							'                            u.HP = 0
							'                            ReDim Preserve EventQue(UBound(EventQue) + 1)
							'                            EventQue(UBound(EventQue)) = _
							''                                "破壊 " & u.MainPilot.ID
							'                        Next
							Do While .CountUnitOnBoard > 0
								u = .UnitOnBoard(1)
								.UnloadUnit((u.ID))
								u.Status_Renamed = "破壊"
								u.HP = 0
								ReDim Preserve EventQue(UBound(EventQue) + 1)
								EventQue(UBound(EventQue)) = "マップ攻撃破壊 " & u.MainPilot.ID
							Loop 
							' MOD END MARGE
							uparty = .Party0
						End With
					End If
				End With
				
				'全滅の判定
				flag = False
				For	Each u In UList
					With u
						If .Party0 = uparty And .Status_Renamed = "出撃" And Not .IsConditionSatisfied("憑依") Then
							flag = True
							Exit For
						End If
					End With
				Next u
				If Not flag Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					EventQue(UBound(EventQue)) = "全滅 " & uparty
				End If
			Case "マップ攻撃破壊"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "マップ攻撃破壊 " & Args(1)
				With PList.Item(Args(1))
					uparty = .Party
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'格納されていたユニットも破壊しておく
							For i = 1 To .CountUnitOnBoard
								u = .UnitOnBoard(i)
								.UnloadUnit((u.ID))
								u.Status_Renamed = "破壊"
								u.HP = 0
								ReDim Preserve EventQue(UBound(EventQue) + 1)
								EventQue(UBound(EventQue)) = "マップ攻撃破壊 " & u.MainPilot.ID
							Next 
							uparty = .Party0
						End With
					End If
				End With
			Case "ターン"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "ターン 全 " & Args(2)
				ReDim Preserve EventQue(UBound(EventQue) + 1)
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "ターン " & VB6.Format(Args(1)) & " " & Args(2)
			Case "損傷率"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "損傷率 " & Args(1) & " " & VB6.Format(Args(2))
			Case "攻撃"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "攻撃 " & Args(1) & " " & Args(2)
			Case "攻撃後"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "攻撃後 " & Args(1) & " " & Args(2)
			Case "会話"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "会話 " & Args(1) & " " & Args(2)
			Case "接触"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "接触 " & Args(1) & " " & Args(2)
			Case "進入"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " & VB6.Format(Args(2)) & " " & VB6.Format(Args(3))
				ReDim Preserve EventQue(UBound(EventQue) + 1)
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " & TerrainName(CShort(Args(2)), CShort(Args(3)))
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If Args(2) = 1 Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " W"
					'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				ElseIf Args(2) = MapWidth Then 
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " E"
				End If
				'UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If Args(3) = 1 Then
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " N"
					'UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				ElseIf Args(3) = MapHeight Then 
					ReDim Preserve EventQue(UBound(EventQue) + 1)
					'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " S"
				End If
			Case "収納"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "収納 " & Args(1)
			Case "使用"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "使用 " & Args(1) & " " & Args(2)
			Case "使用後"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "使用後 " & Args(1) & " " & Args(2)
			Case "行動終了"
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "行動終了 " & Args(1)
			Case "ユニットコマンド"
				'UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = "ユニットコマンド " & Args(1) & " " & Args(2)
				If Not IsEventDefined(EventQue(UBound(EventQue))) Then
					'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = "ユニットコマンド " & Args(1) & " " & PList.Item(Args(2)).Unit_Renamed.Name
				End If
			Case Else
				'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				EventQue(UBound(EventQue)) = Args(0)
				For i = 1 To UBound(Args)
					'UPGRADE_WARNING: オブジェクト Args(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
				Next 
		End Select
		
		If CallDepth > MaxCallDepth Then
			ErrorMessage("サブルーチンの呼び出し階層が" & VB6.Format(MaxCallDepth) & "を超えているため、イベントの処理が出来ません")
			CallDepth = MaxCallDepth
			Exit Sub
		End If
		
		'現在の状態を保存
		ArgIndexStack(CallDepth) = ArgIndex
		VarIndexStack(CallDepth) = VarIndex
		ForIndexStack(CallDepth) = ForIndex
		SaveBasePoint()
		
		'呼び出し階層数をインクリメント
		prev_call_depth = CallDepth
		CallDepth = CallDepth + 1
		
		'各イベントを発生させる
		i = event_que_idx
		IsCanceled = False
		Do 
			'Debug.Print "HandleEvent (" & EventQue(i) & ")"
			
			'前のイベントで他のユニットが出現している可能性があるので
			'本当に全滅したのか判定
			If LIndex(EventQue(i), 1) = "全滅" Then
				uparty = LIndex(EventQue(i), 2)
				For	Each u In UList
					With u
						If .Party0 = uparty And .Status_Renamed = "出撃" And Not .IsConditionSatisfied("憑依") Then
							GoTo NextLoop
						End If
					End With
				Next u
			End If
			
			CurrentLabel = 0
			main_event_done = False
			Do While True
				'現在選択されているユニット＆ターゲットをイベント用に設定
				'SearchLabel()で入れ替えられる可能性があるので、毎回設定し直す必要あり
				SelectedUnitForEvent = SelectedUnit
				'引数に指定されたユニットを優先
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
				
				'実行するイベントラベルを探す
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
						'常時イベントではないイベントは１度しか実行しない
						If main_event_done Then
							ret = 0
						Else
							main_event_done = True
						End If
					End If
				Loop While ret = 0
				
				'戦闘後のイベント実行前にはいくつかの後始末が必要
				If Left(EventData(ret), 1) <> "*" Then
					'UPGRADE_WARNING: オブジェクト Args(0) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If Args(0) = "破壊" Or Args(0) = "損傷率" Or Args(0) = "攻撃後" Or Args(0) = "全滅" Then
						'画面をクリア
						If MainForm.Visible = True Then
							ClearUnitStatus()
							RedrawScreen()
						End If
						
						'メッセージウィンドウを閉じる
						If frmMessage.Visible = True Then
							CloseMessageForm()
						End If
					End If
				End If
				
				'ラベルの行は実行しても無駄なので
				ret = ret + 1
				
				System.Windows.Forms.Application.DoEvents()
				
				'イベントの各コマンドを実行
				Do 
					CurrentLineNum = ret
					If CurrentLineNum > UBound(EventCmd) Then
						GoTo ExitLoop
					End If
					ret = EventCmd(CurrentLineNum).Exec
				Loop While ret > 0
				
				'ステージが終了 or キャンセル？
				If IsScenarioFinished Or IsCanceled Then
					GoTo ExitLoop
				End If
			Loop 
NextLoop: 
			i = i + 1
		Loop While i <= UBound(EventQue)
ExitLoop: 
		
		If CallDepth >= 0 Then
			'呼び出し階層数を元に戻す
			'（サブルーチン内でExitが呼ばれることがあるので単純に-1出来ない）
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
		
		'イベントキューを元に戻す
		ReDim Preserve EventQue(MinLng(event_que_idx - 1, UBound(EventQue)))
		
		'フォント設定をデフォルトに戻す
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.ForeColor = RGB(255, 255, 255)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .Font
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Size = 16
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Name = "ＭＳ Ｐ明朝"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Bold = True
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Italic = False
			End With
			PermanentStringMode = False
			KeepStringMode = False
		End With
		
		'オブジェクト色をデフォルトに戻す
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'描画の基準座標位置を元に戻す
		RestoreBasePoint()
		
		'画面入力のロックを解除
		If Not prev_is_gui_locked Then
			UnlockGUI()
		End If
	End Sub
	
	'イベントを登録しておき、後で実行
	'UPGRADE_WARNING: ParamArray Args が ByRef から ByVal に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' をクリックしてください。
	Public Sub RegisterEvent(ParamArray ByVal Args() As Object)
		Dim i As Short
		
		ReDim Preserve EventQue(UBound(EventQue) + 1)
		'UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		EventQue(UBound(EventQue)) = Args(0)
		For i = 1 To UBound(Args)
			'UPGRADE_WARNING: オブジェクト Args(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
		
		'ラベルの各要素をあらかじめ解析
		llen = ListSplit(lname, litem)
		
		'ラベルの種類を判定
		Select Case litem(1)
			Case "プロローグ"
				ltype = LabelType.PrologueEventLabel
			Case "スタート"
				ltype = LabelType.StartEventLabel
			Case "エピローグ"
				ltype = LabelType.EpilogueEventLabel
			Case "ターン"
				ltype = LabelType.TurnEventLabel
				If IsNumeric(litem(2)) Then
					is_num(2) = True
				End If
				lnum(2) = CStr(StrToLng(litem(2)))
			Case "損傷率"
				ltype = LabelType.DamageEventLabel
				is_unit(2) = True
				is_num(3) = True
				lnum(3) = CStr(StrToLng(litem(3)))
			Case "破壊", "マップ攻撃破壊"
				ltype = LabelType.DestructionEventLabel
				is_unit(2) = True
			Case "全滅"
				ltype = LabelType.TotalDestructionEventLabel
			Case "攻撃"
				ltype = LabelType.AttackEventLabel
				revrersible = True
				is_unit(2) = True
				is_unit(3) = True
			Case "攻撃後"
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
			Case "収納"
				ltype = LabelType.LandEventLabel
				is_unit(2) = True
			Case "使用"
				ltype = LabelType.UseEventLabel
				is_unit(2) = True
			Case "使用後"
				ltype = LabelType.AfterUseEventLabel
				is_unit(2) = True
			Case "変形"
				ltype = LabelType.TransformEventLabel
				is_unit(2) = True
			Case "合体"
				ltype = LabelType.CombineEventLabel
				is_unit(2) = True
			Case "分離"
				ltype = LabelType.SplitEventLabel
				is_unit(2) = True
			Case "行動終了"
				ltype = LabelType.FinishEventLabel
				is_unit(2) = True
			Case "レベルアップ"
				ltype = LabelType.LevelUpEventLabel
				is_unit(2) = True
			Case "勝利条件"
				ltype = LabelType.RequirementEventLabel
			Case "再開"
				ltype = LabelType.ResumeEventLabel
			Case "マップコマンド"
				ltype = LabelType.MapCommandEventLabel
				is_condition(3) = True
			Case "ユニットコマンド"
				ltype = LabelType.UnitCommandEventLabel
				is_condition(4) = True
			Case "特殊効果"
				ltype = LabelType.EffectEventLabel
			Case Else
				ltype = LabelType.NormalLabel
		End Select
		
		'各ラベルについて一致しているかチェック
		For	Each lab In colEventLabelList
			With lab
				'ラベルの種類が一致している？
				If ltype <> .Name Then
					GoTo NextLabel
				End If
				
				'ClearEventされていない？
				If Not .Enable Then
					GoTo NextLabel
				End If
				
				'検索開始行より後ろ？
				If .LineNum < start Then
					GoTo NextLabel
				End If
				
				'パラメータ数が一致している？
				If llen <> .CountPara Then
					If ltype <> LabelType.MapCommandEventLabel And ltype <> LabelType.UnitCommandEventLabel Then
						GoTo NextLabel
					End If
				End If
				
				'各パラメータが一致している？
				reversed = False
CheckPara: 
				For i = 2 To llen
					'コマンド関連ラベルの最後のパラメータは条件式なのでチェックを省く
					If is_condition(i) Then
						Exit For
					End If
					
					'比較するパラメータ
					str1 = litem(i)
					If reversed Then
						str2 = .Para(5 - i)
					Else
						str2 = .Para(i)
					End If
					
					'「全」は全てに一致
					If str2 = "全" Then
						'だだし、「ターン 全」が２回実行されるのは防ぐ
						If ltype <> LabelType.TurnEventLabel Or i <> 2 Then
							GoTo NextPara
						End If
					End If
					
					'数値として比較？
					If is_num(i) Then
						If IsNumeric(str2) Then
							If CDbl(lnum(i)) = CInt(str2) Then
								GoTo NextPara
							ElseIf ltype = LabelType.DamageEventLabel Then 
								'損傷率ラベルの処理
								If CDbl(lnum(i)) > CInt(str2) Then
									Exit For
								End If
							End If
						End If
						GoTo NextLabel
					End If
					
					'ユニット指定として比較？
					If is_unit(i) Then
						If str2 = "味方" Or str2 = "ＮＰＣ" Or str2 = "敵" Or str2 = "中立" Then
							'陣営名で比較
							If str1 <> "味方" And str1 <> "ＮＰＣ" And str1 <> "敵" And str1 <> "中立" Then
								If PList.IsDefined(str1) Then
									str1 = PList.Item(str1).Party
								End If
							End If
						ElseIf PList.IsDefined(str2) Then 
							'パイロットで比較
							With PList.Item(str2)
								If str2 = .Data.Name Or str2 = .Data.Nickname Then
									'グループＩＤが付けられていない場合は
									'パイロット名で比較
									str2 = .Name
									If PList.IsDefined(str1) Then
										str1 = PList.Item(str1).Name
									End If
								Else
									'グループＩＤが付けられている場合は
									'グループＩＤで比較
									If PList.IsDefined(str1) Then
										str1 = PList.Item(str1).ID
									End If
									If InStr(str1, ":") > 0 Then
										str1 = Left(str1, InStr(str1, ":") - 1)
									End If
								End If
							End With
						ElseIf PDList.IsDefined(str2) Then 
							'パイロット名で比較
							str2 = PDList.Item(str2).Name
							If PList.IsDefined(str1) Then
								str1 = PList.Item(str1).Name
							End If
						ElseIf UDList.IsDefined(str2) Then 
							'ユニット名で比較
							If PList.IsDefined(str1) Then
								With PList.Item(str1)
									If Not .Unit_Renamed Is Nothing Then
										str1 = .Unit_Renamed.Name
									End If
								End With
							End If
						Else
							'グループＩＤが付けられているおり、なおかつ同じＩＤの
							'２番目以降のユニットの場合はグループＩＤで比較
							If PList.IsDefined(str1) Then
								str1 = PList.Item(str1).ID
							End If
							If InStr(str1, ":") > 0 Then
								str1 = Left(str1, InStr(str1, ":") - 1)
							End If
							If InStr(str2, ":") > 0 Then
								str2 = Left(str2, InStr(str2, ":") - 1)
							End If
						End If
					End If
					
					'一致したか？
					If str1 <> str2 Then
						If revrersible And Not reversed Then
							'対象と相手を入れ替えたイベントラベルが存在するか判定
							lname2 = litem(1) & " " & ListIndex(.Data, 3) & " " & ListIndex(.Data, 2)
							If .AsterNum > 0 Then
								lname2 = "*" & lname2
							End If
							If FindLabel(lname2) = 0 Then
								'対象と相手を入れ替えて判定し直す
								reversed = True
								GoTo CheckPara
							End If
						End If
						GoTo NextLabel
					End If
NextPara: 
				Next 
				
				'ここまでたどり付けばラベルは一致している
				SearchLabel = .LineNum
				
				'対象と相手を入れ替えて一致した場合はグローバル変数も入れ替え
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
	
	'指定したイベントへのイベントラベルが定義されているか
	'常時イベントではない通常イベントのみを探す場合は
	' normal_event_only = True を指定する
	Public Function IsEventDefined(ByRef lname As String, Optional ByVal normal_event_only As Boolean = False) As Boolean
		Dim i, ret As Integer
		
		'イベントラベルを探す
		i = 0
		Do While 1
			ret = SearchLabel(lname, i + 1)
			If ret = 0 Then
				Exit Function
			End If
			
			If normal_event_only Then
				'常時イベントではない通常イベントのみを探す場合
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
	
	'ラベルが定義されているか
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
				
				'パラメータ間の文字列の違いによる不一致をなくすため、
				'文字列を半角スペース一文字に直しておく
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
		'通常ラベルが重複定義されている場合は無視
	End Sub
	
	'システム側のラベルを追加
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
					'UPGRADE_WARNING: オブジェクト colSysNormalLabelList.Item().LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					colSysNormalLabelList.Item(lname).LineNum = lnum
				End If
			Else
				'イベントラベルを追加
				
				'パラメータ間の文字列の違いによる不一致をなくすため、
				'文字列を半角スペース一文字に直しておく
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
		'通常ラベルが重複定義されている場合は無視
	End Sub
	
	'ラベルを消去
	Public Sub ClearLabel(ByVal lnum As Integer)
		Dim lab As LabelData
		Dim i As Short
		
		'行番号lnumにあるラベルを探す
		For	Each lab In colEventLabelList
			With lab
				If .LineNum = lnum Then
					.Enable = False
					Exit Sub
				End If
			End With
		Next lab
		
		'lnum行目になければその周りを探す
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
	
	'ラベルを探す
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
		
		'パラメータ間の文字列の違いで一致しなかった可能性があるので
		'文字列を半角スペース一文字のみにして検索してみる
		lname2 = ListIndex(lname, 1)
		For i = 2 To ListLength(lname)
			lname2 = lname2 & " " & ListIndex(lname, i)
		Next 
		
		'イベントラベルから検索
		FindLabel = FindEventLabel(lname2)
	End Function
	
	'イベントラベルを探す
	Public Function FindEventLabel(ByRef lname As String) As Integer
		Dim lab As LabelData
		
		On Error GoTo NotFound
		lab = colEventLabelList.Item(lname)
		FindEventLabel = lab.LineNum
		Exit Function
		
NotFound: 
		FindEventLabel = 0
	End Function
	
	'通常ラベルを探す
	Public Function FindNormalLabel(ByRef lname As String) As Integer
		FindNormalLabel = FindNormalLabel0(lname)
		If FindNormalLabel = 0 Then
			FindNormalLabel = FindSysNormalLabel(lname)
		End If
	End Function
	
	'シナリオ側の通常ラベルを探す
	Private Function FindNormalLabel0(ByRef lname As String) As Integer
		Dim lab As LabelData
		
		On Error GoTo NotFound
		lab = colNormalLabelList.Item(lname)
		FindNormalLabel0 = lab.LineNum
		Exit Function
		
NotFound: 
		FindNormalLabel0 = 0
	End Function
	
	'システム側の通常ラベルを探す
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
	'ただしグローバル変数のデータは残しておく
	Public Sub ClearEventData()
		Dim i As Short
		
		'UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedUnitForEvent = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
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
				'UPGRADE_WARNING: オブジェクト colEventLabelList.Item(i).LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
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
		
		DefineGlobalVariable("次ステージ")
		DefineGlobalVariable("セーブデータファイル名")
	End Sub
	
	
	'一時中断用データをファイルにセーブする
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
	
	'一時中断用データをファイルからロードする
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
			'        'イベントファイルをロード
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
			'            '既に読み込まれている場合はスキップ
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
			'        '複数行に分割されたコマンドを結合
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
			'        'コマンドデータ配列を設定
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
				'イベントファイルをロード
				ReDim AdditionalEventFileNames(num)
				For i = 1 To num
					Input(SaveDataFileNumber, fname)
					AdditionalEventFileNames(i) = fname
					If InStr(fname, ":") = 0 Then
						fname = ScenarioPath & fname
					End If
					
					'既に読み込まれている場合はスキップ
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
				
				'複数行に分割されたコマンドを結合
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
				
				'コマンドデータ配列を設定
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
		
		'イベント用ラベルを設定
		i = 1
		num = UBound(label_enabled)
		For	Each lab In colEventLabelList
			If i <= num Then
				'UPGRADE_WARNING: オブジェクト label_enabled(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				lab.Enable = label_enabled(i)
			Else
				lab.Enable = True
			End If
			i = i + 1
		Next lab
		' MOD END MARGE
	End Sub
	
	'一時中断用データのイベントデータ部分を読み飛ばす
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
		
		'ラベル情報
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
	
	'グローバル変数をファイルにセーブ
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
	
	'グローバル変数をファイルからロード
	Public Sub LoadGlobalVariables()
		Dim num, j, i, k, idx As Short
		Dim vvalue, vname, buf As String
		Dim aname As String
		' ADD START MARGE
		Dim is_number As Boolean
		' ADD END MARGE
		'グローバル変数を全削除
		With GlobalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'グローバル変数の総数を読み出し
		Input(SaveDataFileNumber, num)
		
		'各変数の値を読み出し
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
				'SetSkillコマンドのセーブデータをエリアスに対応させる
				If Left(vname, 8) = "Ability(" Then
					idx = InStr(vname, ",")
					If idx > 0 Then
						'個々の能力定義
						aname = Mid(vname, idx + 1, Len(vname) - idx - 1)
						If ALDList.IsDefined(aname) Then
							vname = Left(vname, idx) & ALDList.Item(aname).AliasType(1) & ")"
							If LLength(vvalue) = 1 Then
								vvalue = vvalue & " " & aname
							End If
						End If
					Else
						'必要技能用の能力一覧
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
				'ラーニングした特殊能力が使えないバグに対応
				If Left(vname, 8) = "Ability(" Then
					idx = InStr(vname, ",")
					If idx > 0 Then
						vname2 = Left(vname, idx - 1) & ")"
						aname = Mid(vname, idx + 1, Len(vname) - idx - 1)
						If Not IsGlobalVariableDefined(vname2) Then
							DefineGlobalVariable(vname2)
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							GlobalVariableList.Item(vname2).StringValue = aname
						End If
					End If
				End If
			End If
			
			If SaveDataVersion < 10731 Then
				'不必要な非表示能力に対するSetSkillを削除
				If Left(vname, 8) = "Ability(" Then
					If Right(vname, 5) = ",非表示)" Then
						GoTo NextVariable
					End If
				End If
			End If
			
			If SaveDataVersion < 10732 Then
				'不必要な非表示能力に対するSetSkillと能力名のダブりを削除
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
				'エリアスされた能力をSetSkillした際にエリアスに含まれる解説が無効になるバグへの対処
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
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.StringValue = vvalue
				' MOD START MARGE
				'            If IsNumber(vvalue) Then
				If is_number Then
					' MOD END MARGE
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.VariableType = Expression.ValueType.NumericType
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.NumericValue = CDbl(vvalue)
				Else
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.VariableType = Expression.ValueType.StringType
				End If
			End With
NextVariable: 
		Next 
		'ADD START 240a
		'Optionを全て読み込んだら、新ＧＵＩが有効になっているか確認する
		SetNewGUIMode()
		'ADD  END  240a
	End Sub
	
	'ローカル変数をファイルにセーブ
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
	
	'ローカル変数をファイルからロード
	Public Sub LoadLocalVariables()
		Dim i, num As Short
		' MOD START MARGE
		'Dim vname As String, vvalue As String
		Dim vvalue, vname, buf As String
		Dim is_number As Boolean
		' MOD END MARGE
		'ローカル変数を全削除
		With LocalVariableList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'ローカル変数の総数を読み出し
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'変数の値を読み出し
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
			
			'変数の値を設定
			If Not IsLocalVariableDefined(vname) Then
				DefineLocalVariable(vname)
			End If
			With LocalVariableList.Item(vname)
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.StringValue = vvalue
				' MOD START MARGE
				'            If IsNumber(vvalue) Then
				If is_number Then
					' MOD END MARGE
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.VariableType = Expression.ValueType.NumericType
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.NumericValue = CDbl(vvalue)
				Else
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.VariableType = Expression.ValueType.StringType
				End If
			End With
NextVariable: 
		Next 
	End Sub
	
	
	'イベントエラー表示
	Public Sub DisplayEventErrorMessage(ByVal lnum As Integer, ByVal msg As String)
		Dim buf As String
		
		'エラーが起こったファイル、行番号、エラーメッセージを表示
		buf = EventFileNames(EventFileID(lnum)) & "：" & EventLineNum(lnum) & "行目" & vbCr & vbLf & msg & vbCr & vbLf
		
		'エラーが起こった行とその前後の行の内容を表示
		If lnum > 1 Then
			buf = buf & EventLineNum(lnum - 1) & ": " & EventData(lnum - 1) & vbCr & vbLf
		End If
		buf = buf & EventLineNum(lnum) & ": " & EventData(lnum) & vbCr & vbLf
		If lnum < UBound(EventData) Then
			buf = buf & EventLineNum(lnum + 1) & ": " & EventData(lnum + 1) & vbCr & vbLf
		End If
		
		ErrorMessage(buf)
	End Sub
	
	'インターミッションコマンド「ユニットリスト」におけるユニットリストを作成する
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
		
		'リストのソート項目を設定
		If smode <> "" Then
			key_type = smode
		End If
		If key_type = "" Then
			key_type = "ＨＰ"
		End If
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'あらかじめ撤退させておく
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					.Escape()
				End If
			End With
		Next u
		
		'マップをクリア
		LoadMapData("")
		SetupBackground("", "ステータス")
		
		'ユニット一覧を作成
		If key_type <> "名称" Then
			'配列作成
			ReDim unit_list(UList.Count)
			ReDim key_list(UList.Count)
			i = 0
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Or .Status_Renamed = "待機" Then
						i = i + 1
						unit_list(i) = u
						
						'ソートする項目にあわせてソートの際の優先度を決定
						Select Case key_type
							Case "ランク"
								key_list(i) = .Rank
							Case "ＨＰ"
								key_list(i) = .HP
							Case "ＥＮ"
								key_list(i) = .EN
							Case "装甲"
								key_list(i) = .Armor
							Case "運動性"
								key_list(i) = .Mobility
							Case "移動力"
								key_list(i) = .Speed
							Case "最大攻撃力"
								For j = 1 To .CountWeapon
									If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
										If .WeaponPower(j, "") > key_list(i) Then
											key_list(i) = .WeaponPower(j, "")
										End If
									End If
								Next 
							Case "最長射程"
								For j = 1 To .CountWeapon
									If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
										If .WeaponMaxRange(j) > key_list(i) Then
											key_list(i) = .WeaponMaxRange(j)
										End If
									End If
								Next 
							Case "レベル"
								key_list(i) = .MainPilot.Level
							Case "ＳＰ"
								key_list(i) = .MainPilot.MaxSP
							Case "格闘"
								key_list(i) = .MainPilot.Infight
							Case "射撃"
								key_list(i) = .MainPilot.Shooting
							Case "命中"
								key_list(i) = .MainPilot.Hit
							Case "回避"
								key_list(i) = .MainPilot.Dodge
							Case "技量"
								key_list(i) = .MainPilot.Technique
							Case "反応"
								key_list(i) = .MainPilot.Intuition
						End Select
					End If
				End With
			Next u
			ReDim Preserve unit_list(i)
			ReDim Preserve key_list(i)
			
			'ソート
			For i = 1 To UBound(key_list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(unit_list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					u = unit_list(i)
					unit_list(i) = unit_list(max_item)
					unit_list(max_item) = u
					
					max_value = key_list(max_item)
					key_list(max_item) = key_list(i)
					key_list(i) = max_value
				End If
			Next 
		Else
			'配列作成
			ReDim unit_list(UList.Count)
			Dim strkey_list(UList.Count) As Object
			i = 0
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Or .Status_Renamed = "待機" Then
						i = i + 1
						unit_list(i) = u
						If IsOptionDefined("等身大基準") Then
							'UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							strkey_list(i) = .MainPilot.KanaName
						Else
							'UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							strkey_list(i) = .KanaName
						End If
					End If
				End With
			Next u
			ReDim Preserve unit_list(i)
			ReDim Preserve strkey_list(i)
			
			'ソート
			For i = 1 To UBound(strkey_list) - 1
				max_item = i
				'UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					'UPGRADE_WARNING: オブジェクト strkey_list() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						'UPGRADE_WARNING: オブジェクト strkey_list(j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					u = unit_list(i)
					unit_list(i) = unit_list(max_item)
					unit_list(max_item) = u
					
					'UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					'UPGRADE_WARNING: オブジェクト strkey_list(max_item) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Font Regular 9pt 背景
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0).Font
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Size = 9
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Bold = False
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Italic = False
		End With
		PermanentStringMode = True
		HCentering = False
		VCentering = False
		
		'ユニットのリストを作成
		xx = 1
		yy = 1
		For i = 1 To UBound(unit_list)
			u = unit_list(i)
			With u
				'ユニット出撃位置を折り返す
				If xx > 15 Then
					xx = 1
					yy = yy + 1
					If yy > 40 Then
						'ユニット数が多すぎるため、一部のパイロットが表示出来ません
						Exit For
					End If
				End If
				
				'パイロットが乗っていない場合はダミーパイロットを乗せる
				If .CountPilot = 0 Then
					p = PList.Add("ステータス表示用ダミーパイロット(ザコ)", 1, "味方")
					p.Ride(u)
				End If
				
				'出撃
				.UsedAction = 0
				.StandBy(xx, yy)
				
				'プレイヤーが操作できないように
				.AddCondition("非操作", -1)
				
				'ユニットの愛称を表示
				DrawString(.Nickname, 32 * xx + 2, 32 * yy - 31)
				
				'ソート項目にあわせてユニットのステータスを表示
				Select Case key_type
					Case "ランク"
						DrawString("RK" & VB6.Format(key_list(i)) & " " & Term("HP", u) & VB6.Format(.HP) & " " & Term("EN", u) & VB6.Format(.EN), 32 * xx + 2, 32 * yy - 15)
					Case "ＨＰ", "ＥＮ", "名称"
						DrawString(Term("HP", u) & VB6.Format(.HP) & " " & Term("EN", u) & VB6.Format(.EN), 32 * xx + 2, 32 * yy - 15)
					Case "装甲"
						DrawString(Term("装甲", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "運動性"
						DrawString(Term("運動性", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "移動力"
						DrawString(Term("移動力", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "最大攻撃力"
						DrawString("攻撃力" & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "最長射程"
						DrawString("射程" & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "レベル"
						DrawString("Lv" & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "ＳＰ"
						DrawString(Term("SP", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "格闘"
						DrawString(Term("格闘", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "射撃"
						If .MainPilot.HasMana() Then
							DrawString(Term("魔力", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
						Else
							DrawString(Term("射撃", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
						End If
					Case "命中"
						DrawString(Term("命中", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "回避"
						DrawString(Term("回避", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "技量"
						DrawString(Term("技量", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "反応"
						DrawString(Term("反応", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
				End Select
				
				'表示位置を右に5マスずらす
				xx = xx + 5
			End With
		Next 
		
		'フォントの設定を戻しておく
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0).Font
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Size = 16
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Bold = True
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Italic = False
		End With
		PermanentStringMode = False
		
		RedrawScreen()
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	
	'描画の基準座標位置を保存
	Public Sub SaveBasePoint()
		BasePointIndex = BasePointIndex + 1
		If BasePointIndex > UBound(SavedBaseX) Then
			BasePointIndex = 0
		End If
		SavedBaseX(BasePointIndex) = BaseX
		SavedBaseY(BasePointIndex) = BaseY
	End Sub
	
	'描画の基準座標位置を復元
	Public Sub RestoreBasePoint()
		If BasePointIndex <= 0 Then
			BasePointIndex = UBound(SavedBaseX)
		End If
		BaseX = SavedBaseX(BasePointIndex)
		BaseY = SavedBaseY(BasePointIndex)
		BasePointIndex = BasePointIndex - 1
	End Sub
	
	'描画の基準座標位置をリセット
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