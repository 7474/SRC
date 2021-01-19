Option Strict Off
Option Explicit On
Module InterMission
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'繧､繝ｳ繧ｿ繝ｼ繝溘ャ繧ｷ繝ｧ繝ｳ
	Public Sub InterMissionCommand(Optional ByVal skip_update As Boolean = False)
		Dim cmd_list() As String
		Dim name_list() As String
		Dim j, i, ret As Short
		Dim buf As String
		Dim u As Unit
		Dim var As VarData
		Dim fname, save_path As String
		
		Stage = "繧､繝ｳ繧ｿ繝ｼ繝溘ャ繧ｷ繝ｧ繝ｳ"
		IsSubStage = False
		
		'Invalid_string_refer_to_original_code
		KeepBGM = False
		BossBGM = False
		If InStr(BGMFileName, "\" & BGMName("Intermission")) = 0 Then
			StopBGM()
			StartBGM(BGMName("Intermission"))
		End If
		
		'繝槭ャ繝励ｒ繧ｯ繝ｪ繧｢
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		
		'Invalid_string_refer_to_original_code
		If Not skip_update Then
			UList.Update()
			PList.Update()
			IList.Update()
		End If
		ClearEventData()
		ClearMap()
		
		'驕ｸ謚樒畑繝繧､繧｢繝ｭ繧ｰ繧呈僑螟ｧ
		EnlargeListBoxHeight()
		
		Do While True
			'Invalid_string_refer_to_original_code
			
			ReDim cmd_list(0)
			ReDim ListItemFlag(0)
			ReDim ListItemID(0)
			cmd_list(0) = "繧ｭ繝｣繝ｳ繧ｻ繝ｫ"
			
			'Invalid_string_refer_to_original_code
			If GetValueAsString("Invalid_string_refer_to_original_code") <> "" Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			End If
			
			'Invalid_string_refer_to_original_code
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			End If
			
			'Invalid_string_refer_to_original_code
			If Not IsOptionDefined("謾ｹ騾荳榊庄") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			Else
				cmd_list(UBound(cmd_list)) = "讖滉ｽ捺隼騾"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If Left(.Class_Renamed, 1) = "(" Then
							cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
							Exit For
						End If
					End With
				Next u
			End If
			'End With
			'Next
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If IsOptionDefined("荵励ｊ謠帙∴") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "荵励ｊ謠帙∴"
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					Exit For
					'End If
				End With
			Next u
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			Exit For
			'End If
			'End If
			'End If
			'End With
			'Next
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			For	Each var In GlobalVariableList
				If InStr(var.Name, "IntermissionCommand(") = 1 Then
					ret = Len("IntermissionCommand(")
					buf = Mid(var.Name, ret + 1, Len(var.Name) - ret - 1)
					buf = GetValueAsString(buf)
					FormatMessage(buf)
					ReDim Preserve cmd_list(UBound(cmd_list) + 1)
					ReDim Preserve ListItemFlag(UBound(cmd_list))
					ReDim Preserve ListItemID(UBound(cmd_list))
					cmd_list(UBound(cmd_list)) = buf
					ListItemID(UBound(cmd_list)) = var.Name
				End If
			Next var
			
			'Invalid_string_refer_to_original_code
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "Invalid_string_refer_to_original_code"
			
			'繧､繝ｳ繧ｿ繝ｼ繝溘ャ繧ｷ繝ｧ繝ｳ縺ｮ繧ｳ繝槭Φ繝牙錐遘ｰ縺ｫ繧ｨ繝ｪ繧｢繧ｹ繧帝←逕ｨ
			ReDim name_list(UBound(cmd_list))
			For i = 1 To UBound(name_list)
				name_list(i) = cmd_list(i)
				With ALDList
					For j = 1 To .Count
						With .Item(j)
							If .AliasType(1) = cmd_list(i) Then
								name_list(i) = .Name
								Exit For
							End If
						End With
					Next 
				End With
			Next 
			
			'Invalid_string_refer_to_original_code
			TopItem = 1
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			
			'Invalid_string_refer_to_original_code
			Select Case cmd_list(ret)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					
					UList.Update() 'Invalid_string_refer_to_original_code
					
					frmListBox.Hide()
					ReduceListBoxHeight()
					StopBGM()
					Exit Sub
					'End If
					
				Case "Invalid_string_refer_to_original_code"
					'荳譌ｦ縲悟ｸｸ縺ｫ謇句燕縺ｫ陦ｨ遉ｺ縲阪ｒ隗｣髯､
					If frmListBox.Visible Then
						ret = SetWindowPos(frmListBox.Handle.ToInt32, -2, 0, 0, 0, 0, &H3)
					End If
					
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					
					'Invalid_string_refer_to_original_code
					If frmListBox.Visible Then
						ret = SetWindowPos(frmListBox.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
					End If
					
					'Invalid_string_refer_to_original_code
					If fname = "" Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If InStr(fname, "\") > 0 Then
						save_path = Left(fname, InStr2(fname, "\"))
					End If
					'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
					If Dir(save_path) <> Dir(ScenarioPath) Then
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextLoop
					End If
					'End If
					
					If fname <> "" Then
						UList.Update() 'Invalid_string_refer_to_original_code
						SaveData(fname)
					End If
					
				Case "讖滉ｽ捺隼騾", "Invalid_string_refer_to_original_code"
					RankUpCommand()
					
				Case "荵励ｊ謠帙∴"
					ExchangeUnitCommand()
					
				Case "Invalid_string_refer_to_original_code"
					ExchangeItemCommand()
					
				Case "Invalid_string_refer_to_original_code"
					ExchangeFormCommand()
					
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					frmListBox.Hide()
					ReduceListBoxHeight()
					ExitGame()
					'End If
					
				Case "Invalid_string_refer_to_original_code"
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					If FileExists(ScenarioPath & "Invalid_string_refer_to_original_code") Then
						StartScenario(ScenarioPath & "Invalid_string_refer_to_original_code")
					ElseIf FileExists(ExtDataPath & "Invalid_string_refer_to_original_code") Then 
						StartScenario(ExtDataPath & "Invalid_string_refer_to_original_code")
					ElseIf FileExists(ExtDataPath2 & "Invalid_string_refer_to_original_code") Then 
						StartScenario(ExtDataPath2 & "Invalid_string_refer_to_original_code")
					Else
						StartScenario(AppPath & "Invalid_string_refer_to_original_code")
					End If
					'Invalid_string_refer_to_original_code
					IsSubStage = True
					Exit Sub
					
				Case "Invalid_string_refer_to_original_code"
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					If FileExists(ScenarioPath & "Invalid_string_refer_to_original_code") Then
						StartScenario(ScenarioPath & "Invalid_string_refer_to_original_code")
					ElseIf FileExists(ExtDataPath & "Invalid_string_refer_to_original_code") Then 
						StartScenario(ExtDataPath & "Invalid_string_refer_to_original_code")
					ElseIf FileExists(ExtDataPath2 & "Invalid_string_refer_to_original_code") Then 
						StartScenario(ExtDataPath2 & "Invalid_string_refer_to_original_code")
					Else
						StartScenario(AppPath & "Invalid_string_refer_to_original_code")
					End If
					'Invalid_string_refer_to_original_code
					IsSubStage = True
					Exit Sub
					
				Case "繧ｭ繝｣繝ｳ繧ｻ繝ｫ"
					'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
					
					'Invalid_string_refer_to_original_code
				Case Else
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					StartScenario(GetValueAsString(ListItemID(ret)))
					If IsSubStage Then
						'Invalid_string_refer_to_original_code
						KeepBGM = False
						BossBGM = False
						ChangeBGM(BGMName("Intermission"))
						UList.Update()
						PList.Update()
						IList.Update()
						ClearEventData()
						If MapWidth > 1 Then
							ClearMap()
						End If
						IsSubStage = False
						EnlargeListBoxHeight()
					Else
						'Invalid_string_refer_to_original_code
						IsSubStage = True
						Exit Sub
					End If
			End Select
NextLoop: 
		Loop 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RankUpCommand()
		Dim k, i, j, urank As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim cost As Integer
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim use_max_rank As Boolean
		Dim name_width As Short
		
		TopItem = 1
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		sort_mode = "繝ｬ繝吶Ν"
		sort_mode = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		For	Each u In UList
			If u.IsFeatureAvailable("譛螟ｧ謾ｹ騾謨ｰ") Then
				use_max_rank = True
				Exit For
			End If
		Next u
		
		'Invalid_string_refer_to_original_code
		name_width = 33
		If use_max_rank Then
			name_width = name_width - 2
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		name_width = name_width + 8
		'End If
		
		'Invalid_string_refer_to_original_code
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemFlag(1)
		ReDim ListItemComment(1)
		list(1) = "笆ｽ荳ｦ縺ｹ譖ｿ縺遺命"
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextLoop
				'End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemFlag(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'Invalid_string_refer_to_original_code
				cost = RankUpCost(u)
				If cost > Money Or cost > 10000000 Then
					ListItemFlag(UBound(list)) = True
				End If
				
				'繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ
				If use_max_rank Then
					list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
					If MaxRank(u) > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
					Else
						list(UBound(list)) = list(UBound(list)) & "--"
					End If
				Else
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				If cost < 10000000 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(cost), 7)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("----", 7)
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .CountPilot > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .CountPilot > 0 Then
					list(UBound(list)) = list(UBound(list)) & "  " & .MainPilot.Nickname
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				For k = 1 To .CountItem
					With .Item(k)
						'Invalid_string_refer_to_original_code_
						'And .Part <> "髱櫁｡ｨ遉ｺ" _
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						'End If
					End With
				Next 
				
				'Invalid_string_refer_to_original_code
				id_list(UBound(list)) = .ID
			End With
NextLoop: 
		Next u
		
Beginning: 
		
		'Invalid_string_refer_to_original_code
		If InStr(sort_mode, "蜷咲ｧｰ") = 0 Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "驕句虚諤ｧ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "繝ｬ繝吶Ν"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'繧ｭ繝ｼ繧剃ｽｿ縺｣縺ｦ荳ｦ縺ｹ謠帙∴
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "蜷咲ｧｰ", "繝ｦ繝九ャ繝亥錐遘ｰ"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'繧ｭ繝ｼ繧剃ｽｿ縺｣縺ｦ荳ｦ縺ｹ謠帙∴
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If use_max_rank Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		Else
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		End If
		If use_max_rank Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		Else
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		End If
		'End If
		
		Select Case ret
			Case 0
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				Exit Sub
			Case 1
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				sort_mode_type(1) = "蜷咲ｧｰ"
				sort_mode_list(1) = "蜷咲ｧｰ"
				sort_mode_type(2) = "繝ｬ繝吶Ν"
				sort_mode_list(2) = "繝ｬ繝吶Ν"
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "Invalid_string_refer_to_original_code"
				sort_mode_list(4) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(5) = "Invalid_string_refer_to_original_code"
				sort_mode_list(5) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(6) = "驕句虚諤ｧ"
				sort_mode_list(6) = Term("驕句虚諤ｧ")
				sort_mode_type(7) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(7) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(1) = "Invalid_string_refer_to_original_code"
				sort_mode_list(1) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(2) = "Invalid_string_refer_to_original_code"
				sort_mode_list(2) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "驕句虚諤ｧ"
				sort_mode_list(4) = Term("驕句虚諤ｧ")
				sort_mode_type(5) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(5) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				sort_mode_list(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				sort_mode_type(7) = "Invalid_string_refer_to_original_code"
				sort_mode_list(7) = "Invalid_string_refer_to_original_code"
				'End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'繧ｽ繝ｼ繝域婿豕輔ｒ螟画峩縺励※蜀崎｡ｨ遉ｺ
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo Beginning
		End Select
		
		'謾ｹ騾縺吶ｋ繝ｦ繝九ャ繝医ｒ讀懃ｴ｢
		u = UList.Item(id_list(ret))
		
		'Invalid_string_refer_to_original_code
		If u.IsHero Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			GoTo Beginning
		End If
		'Invalid_string_refer_to_original_code_
		'vbOKCancel + vbQuestion, "謾ｹ騾") <> 1 _
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		GoTo Beginning
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code
		IncrMoney(-RankUpCost(u))
		
		'Invalid_string_refer_to_original_code
		With u
			.Rank = .Rank + 1
			.HP = .MaxHP
			.EN = .MaxEN
			
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountOtherForm
				.OtherForm(i).Rank = .Rank
				.OtherForm(i).HP = .OtherForm(i).MaxHP
				.OtherForm(i).EN = .OtherForm(i).MaxEN
			Next 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To .CountFeature
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				buf = LIndex(.FeatureData(i), 2)
				If LLength(.FeatureData(i)) = 3 Then
					If UDList.IsDefined(buf) Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						Exit For
					End If
				End If
				If UDList.IsDefined(buf) Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					Exit For
				End If
				'End If
				'End If
				'End If
			Next 
			If i <= .CountFeature Then
				urank = .Rank
				buf = UDList.Item(LIndex(.FeatureData(i), 2)).FeatureData("Invalid_string_refer_to_original_code")
				For i = 2 To LLength(buf)
					If Not UList.IsDefined(LIndex(buf, i)) Then
						GoTo NextForm
					End If
					
					With UList.Item(LIndex(buf, i))
						.Rank = MaxLng(urank, .Rank)
						.HP = .MaxHP
						.EN = .MaxEN
						For j = 1 To .CountOtherForm
							.OtherForm(j).Rank = .Rank
							.OtherForm(j).HP = .OtherForm(j).MaxHP
							.OtherForm(j).EN = .OtherForm(j).MaxEN
						Next 
						
						For j = 1 To UBound(id_list)
							If .CurrentForm.ID = id_list(j) Then
								Exit For
							End If
						Next 
						
						If j > UBound(id_list) Then
							GoTo NextForm
						End If
						
						If use_max_rank Then
							list(j) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
							If MaxRank(u) > 0 Then
								list(j) = list(j) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
							Else
								list(j) = list(j) & "--"
							End If
						Else
							If .Rank < 10 Then
								list(j) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
							Else
								list(j) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
							End If
						End If
						
						If RankUpCost(u) < 1000000 Then
							list(j) = list(j) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
						Else
							list(j) = list(j) & LeftPaddedString("----", 7)
						End If
						
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If .CountPilot > 0 Then
							list(j) = list(j) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
						End If
					End With
				Next 
			End If
			list(j) = list(j) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .CountPilot > 0 Then
				list(j) = list(j) & "  " & .MainPilot.Nickname
			End If
			'End If
		End With
NextForm: 
		'Next
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		
		If use_max_rank Then
			'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
			If MaxRank(u) > 0 Then
				list(ret) = list(ret) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
			Else
				list(ret) = list(ret) & "--"
			End If
		Else
			'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		End If
		
		If RankUpCost(u) < 10000000 Then
			list(ret) = list(ret) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
		Else
			list(ret) = list(ret) & LeftPaddedString("----", 7)
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_WARNING: RankUpCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		'End With
		
		'Invalid_string_refer_to_original_code
		For i = 2 To UBound(list)
			cost = RankUpCost(UList.Item(id_list(i)))
			If cost > Money Or cost > 10000000 Then
				ListItemFlag(i) = True
			Else
				ListItemFlag(i) = False
			End If
		Next 
		
		GoTo Beginning
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function RankUpCost(ByRef u As Unit) As Integer
		With u
			'Invalid_string_refer_to_original_code
			If .Rank >= MaxRank(u) Then
				RankUpCost = 999999999
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				RankUpCost = 999999999
				Exit Function
			End If
			'End If
			
			If IsOptionDefined("菴取隼騾雋ｻ") Then
				'Invalid_string_refer_to_original_code
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 30000
					Case 4
						RankUpCost = 40000
					Case 5
						RankUpCost = 50000
					Case 6
						RankUpCost = 60000
					Case 7
						RankUpCost = 70000
					Case 8
						RankUpCost = 80000
					Case 9
						RankUpCost = 100000
					Case 10
						RankUpCost = 120000
					Case 11
						RankUpCost = 140000
					Case 12
						RankUpCost = 160000
					Case 13
						RankUpCost = 180000
					Case 14
						RankUpCost = 200000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			ElseIf IsOptionDefined("Invalid_string_refer_to_original_code") Then 
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 120000
					Case 6
						RankUpCost = 160000
					Case 7
						RankUpCost = 200000
					Case 8
						RankUpCost = 250000
					Case 9
						RankUpCost = 300000
					Case 10
						RankUpCost = 350000
					Case 11
						RankUpCost = 400000
					Case 12
						RankUpCost = 450000
					Case 13
						RankUpCost = 500000
					Case 14
						RankUpCost = 550000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			Else
				'Invalid_string_refer_to_original_code
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 150000
					Case 6
						RankUpCost = 200000
					Case 7
						RankUpCost = 300000
					Case 8
						RankUpCost = 400000
					Case 9
						RankUpCost = 500000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("謾ｹ騾雋ｻ菫ｮ豁｣") Then
				RankUpCost = RankUpCost * (1# + .FeatureLevel("謾ｹ騾雋ｻ菫ｮ豁｣") / 10)
			End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function MaxRank(ByRef u As Unit) As Integer
		If IsOptionDefined("Invalid_string_refer_to_original_code") Then
			'Invalid_string_refer_to_original_code
			MaxRank = 5
		ElseIf IsOptionDefined("Invalid_string_refer_to_original_code") Then 
			'Invalid_string_refer_to_original_code
			MaxRank = 15
		Else
			'Invalid_string_refer_to_original_code
			MaxRank = 10
		End If
		
		With u
			'Invalid_string_refer_to_original_code
			If IsGlobalVariableDefined("Disable(" & .Name & ",謾ｹ騾)") Then
				MaxRank = 0
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("譛螟ｧ謾ｹ騾謨ｰ") Then
				MaxRank = MinLng(MaxRank, .FeatureLevel("譛螟ｧ謾ｹ騾謨ｰ"))
			End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub ExchangeUnitCommand()
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode, sort_mode2 As String
		Dim sort_mode_type() As String
		Dim sort_mode_list() As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim p As Pilot
		Dim pname As String
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim is_support As Boolean
		Dim caption_str As String
		Dim top_item As Short
		
		top_item = 1
		
		'Invalid_string_refer_to_original_code
		sort_mode = "繝ｬ繝吶Ν"
		sort_mode2 = "蜷咲ｧｰ"
		
Beginning: 
		
		'Invalid_string_refer_to_original_code
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "笆ｽ荳ｦ縺ｹ譖ｿ縺遺命"
		For	Each p In PList
			With p
				If .Party <> "蜻ｳ譁ｹ" Or .Away Or IsGlobalVariableDefined("Fix(" & .Name & ")") Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				If .IsAdditionalPilot Or .IsAdditionalSupport Then
					GoTo NextLoop
				End If
				
				is_support = False
				If Not .Unit_Renamed Is Nothing Then
					'Invalid_string_refer_to_original_code
					If .Unit_Renamed.CountSupport > 1 Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If .Unit_Renamed.CountSupport = 1 Then
						If .ID = .Unit_Renamed.Support(1).ID Then
							is_support = True
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If Not is_support Then
						'Invalid_string_refer_to_original_code
						If .Unit_Renamed.Data.PilotNum <> 1 And System.Math.Abs(.Unit_Renamed.Data.PilotNum) <> 2 Then
							GoTo NextLoop
						End If
					End If
				End If
				
				If is_support Then
					'Invalid_string_refer_to_original_code
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'Invalid_string_refer_to_original_code
					list(UBound(list)) = RightPaddedString("*" & .Nickname, 25) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4)
					
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'Invalid_string_refer_to_original_code
							list(UBound(list)) = list(UBound(list)) & "  " & RightPaddedString(.Nickname0, 29) & "(" & .MainPilot.Nickname & ")"
							
							'Invalid_string_refer_to_original_code
							For k = 1 To .CountItem
								With .Item(k)
									'Invalid_string_refer_to_original_code_
									'And .Part <> "髱櫁｡ｨ遉ｺ" _
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
								End With
							Next 
						End With
					End If
				End If
			End With
		Next p
		'End With
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'Invalid_string_refer_to_original_code
		ReDim Preserve list(UBound(list) + 1)
		ReDim Preserve id_list(UBound(list))
		ReDim Preserve ListItemComment(UBound(list))
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'Invalid_string_refer_to_original_code
		ReDim Preserve list(UBound(list) + 1)
		ReDim Preserve id_list(UBound(list))
		ReDim Preserve ListItemComment(UBound(list))
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		'End With
NextLoop: 
		'Next
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'Invalid_string_refer_to_original_code
		If sort_mode = "繝ｬ繝吶Ν" Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim key_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					With .Item(id_list(i))
						key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
					End With
				Next 
			End With
			
			'繝ｬ繝吶Ν繧剃ｽｿ縺｣縺ｦ荳ｦ縺ｹ謠帙∴
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim strkey_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		TopItem = top_item
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		caption_str = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		caption_str = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		top_item = TopItem
		
		Select Case ret
			Case 0
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				Exit Sub
			Case 1
				'Invalid_string_refer_to_original_code
				ReDim sort_mode_list(2)
				sort_mode_list(1) = "繝ｬ繝吶Ν"
				sort_mode_list(2) = "蜷咲ｧｰ"
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'繧ｽ繝ｼ繝域婿豕輔ｒ螟画峩縺励※蜀崎｡ｨ遉ｺ
				If ret > 0 Then
					sort_mode = sort_mode_list(ret)
				End If
				GoTo SortAgain
		End Select
		
		'Invalid_string_refer_to_original_code
		p = PList.Item(id_list(ret))
		
		'Invalid_string_refer_to_original_code
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "笆ｽ荳ｦ縺ｹ譖ｿ縺遺命"
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextUnit
				'End If
				
				If .CountSupport > 1 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					GoTo NextUnit
				End If
				'End If
				
				If u Is p.Unit_Renamed Then
					GoTo NextUnit
				End If
				
				If Not p.IsAbleToRide(u) Then
					GoTo NextUnit
				End If
				
				'Invalid_string_refer_to_original_code
				If Not p.IsSupport(u) Then
					If .Data.PilotNum <> 1 And System.Math.Abs(.Data.PilotNum) <> 2 Then
						GoTo NextUnit
					End If
				End If
				
				If .CountPilot > 0 Then
					If IsGlobalVariableDefined("Fix(" & .Pilot(1).Name & ")") And Not p.IsSupport(u) Then
						'Invalid_string_refer_to_original_code
						'髯舌ｊ荵励ｊ謠帙∴荳榊庄
						GoTo NextUnit
					End If
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'Invalid_string_refer_to_original_code
					If .CountPilot < System.Math.Abs(.Data.PilotNum) Then
						list(UBound(list)) = "-"
					Else
						list(UBound(list)) = " "
					End If
					
					list(UBound(list)) = list(UBound(list)) & RightPaddedString(.Nickname0, 35) & RightPaddedString(.MainPilot.Nickname, 21)
					If .Rank < 10 Then
						list(UBound(list)) = list(UBound(list)) & " " & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & VB6.Format(.Rank)
					End If
					If .CountSupport > 0 Then
						list(UBound(list)) = list(UBound(list)) & " (" & .Support(1).Nickname & ")"
					End If
					
					'Invalid_string_refer_to_original_code
					For j = 1 To .CountItem
						With .Item(j)
							'Invalid_string_refer_to_original_code_
							'And .Part <> "髱櫁｡ｨ遉ｺ" _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						End With
					Next 
				End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'Invalid_string_refer_to_original_code
		
		ReDim Preserve list(UBound(list) + 1)
		ReDim Preserve id_list(UBound(list))
		ReDim Preserve ListItemComment(UBound(list))
		
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeUnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		'End With
NextUnit: 
		'Next
		ReDim ListItemFlag(UBound(list))
		
SortAgain2: 
		
		'Invalid_string_refer_to_original_code
		If InStr(sort_mode2, "蜷咲ｧｰ") = 0 Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode2
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "驕句虚諤ｧ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
				End Select
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim strkey_list(UBound(list))
			With UList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		TopItem = 1
		u = p.Unit_Renamed
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		caption_str = "Invalid_string_refer_to_original_code" & Term("繝ｩ繝ｳ繧ｯ")
		caption_str = "Invalid_string_refer_to_original_code" & Term("繝ｩ繝ｳ繧ｯ")
		'End If
		If Not u Is Nothing Then
			If u.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End If
		Else
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		End If
		
		Select Case ret
			Case 0
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				Exit Sub
			Case 1
				'Invalid_string_refer_to_original_code
				ReDim sort_mode_type(6)
				ReDim sort_mode_list(6)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				sort_mode_type(1) = "蜷咲ｧｰ"
				sort_mode_list(1) = "蜷咲ｧｰ"
				sort_mode_type(2) = "Invalid_string_refer_to_original_code"
				sort_mode_list(2) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "Invalid_string_refer_to_original_code"
				sort_mode_list(4) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(5) = "驕句虚諤ｧ"
				sort_mode_list(5) = Term("驕句虚諤ｧ")
				sort_mode_type(6) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(6) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(1) = "Invalid_string_refer_to_original_code"
				sort_mode_list(1) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(2) = "Invalid_string_refer_to_original_code"
				sort_mode_list(2) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "驕句虚諤ｧ"
				sort_mode_list(4) = Term("驕句虚諤ｧ")
				sort_mode_type(5) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(5) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				sort_mode_list(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				'End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'繧ｽ繝ｼ繝域婿豕輔ｒ螟画峩縺励※蜀崎｡ｨ遉ｺ
				If ret > 0 Then
					sort_mode2 = sort_mode_type(ret)
				End If
				GoTo SortAgain2
		End Select
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			GoTo Beginning
		End If
		
		u = UList.Item(id_list(ret))
		
		'Invalid_string_refer_to_original_code
		p.GetOff()
		
		'荵励ｊ謠帙∴
		With u
			If Not p.IsSupport(u) Then
				'Invalid_string_refer_to_original_code
				If .CountPilot = .Data.PilotNum Then
					.Pilot(1).GetOff()
				End If
			Else
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountSupport
					.Support(1).GetOff()
				Next 
			End If
		End With
		p.Ride(UList.Item(id_list(ret)))
		
		GoTo Beginning
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ExchangeItemCommand(Optional ByRef selected_unit As Unit = Nothing, Optional ByRef selected_part As String = "")
		Dim j, i, k As Short
		Dim inum, inum2 As Short
		Dim list() As String
		Dim id_list() As String
		Dim iid As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim caption_str As String
		Dim u As Unit
		Dim it As Item
		Dim iname As String
		Dim buf As String
		Dim ret As Short
		Dim part_list() As String
		Dim part_item() As String
		Dim arm_point, shoulder_point As Short
		Dim ipart As String
		Dim empty_slot As Short
		Dim is_right_hand_available As Boolean
		Dim is_left_hand_available As Boolean
		Dim item_list() As String
		Dim top_item1, top_item2 As Short
		
		top_item1 = 1
		top_item2 = 1
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		sort_mode = "繝ｬ繝吶Ν"
		Dim tmp_part_list() As String
		sort_mode = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If Not selected_unit Is Nothing Then
			EnlargeListBoxHeight()
			ReduceListBoxWidth()
			
			u = selected_unit
			If MainForm.Visible Then
				If Not u Is DisplayedUnit Then
					DisplayUnitStatus(u)
				End If
			End If
			
			GoTo MakeEquipedItemList
		End If
		
Beginning: 
		
		'Invalid_string_refer_to_original_code
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "笆ｽ荳ｦ縺ｹ譖ｿ縺遺命"
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextUnit
				'End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'Invalid_string_refer_to_original_code
				inum = 0
				inum2 = 0
				For i = 1 To .CountItem
					With .Item(i)
						'Invalid_string_refer_to_original_code_
						'And .Part <> "髱櫁｡ｨ遉ｺ" _
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						inum = inum + .Size
						inum2 = inum2 + .Size
						'End If
						'End If
					End With
				Next 
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				list(UBound(list)) = RightPaddedString(.Nickname0, 39)
				list(UBound(list)) = RightPaddedString(.Nickname0, 31)
				'End If
				list(UBound(list)) = list(UBound(list)) & VB6.Format(inum) & "/" & VB6.Format(.MaxItemNum)
				If inum2 > 0 Then
					list(UBound(list)) = list(UBound(list)) & "(" & VB6.Format(inum2) & ")   "
				Else
					list(UBound(list)) = list(UBound(list)) & "      "
				End If
				If .Rank < 10 Then
					list(UBound(list)) = list(UBound(list)) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
				Else
					list(UBound(list)) = list(UBound(list)) & VB6.Format(.Rank)
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .CountPilot > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
				End If
				'End If
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .CountPilot > 0 Then
					list(UBound(list)) = list(UBound(list)) & " " & .MainPilot.Nickname
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				id_list(UBound(list)) = .ID
			End With
NextUnit: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'Invalid_string_refer_to_original_code
		If InStr(sort_mode, "蜷咲ｧｰ") = 0 Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "驕句虚諤ｧ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "繝ｬ繝吶Ν"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "蜷咲ｧｰ", "繝ｦ繝九ャ繝亥錐遘ｰ"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		TopItem = top_item1
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'& Term("驕句虚"), _
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		top_item1 = TopItem
		
		Select Case ret
			Case 0
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				Exit Sub
			Case 1
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				sort_mode_type(1) = "蜷咲ｧｰ"
				sort_mode_list(1) = "蜷咲ｧｰ"
				sort_mode_type(2) = "繝ｬ繝吶Ν"
				sort_mode_list(2) = "繝ｬ繝吶Ν"
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "Invalid_string_refer_to_original_code"
				sort_mode_list(4) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(5) = "Invalid_string_refer_to_original_code"
				sort_mode_list(5) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(6) = "驕句虚諤ｧ"
				sort_mode_list(6) = Term("驕句虚諤ｧ")
				sort_mode_type(7) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(7) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(1) = "Invalid_string_refer_to_original_code"
				sort_mode_list(1) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(2) = "Invalid_string_refer_to_original_code"
				sort_mode_list(2) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(3) = "Invalid_string_refer_to_original_code"
				sort_mode_list(3) = Term("Invalid_string_refer_to_original_code")
				sort_mode_type(4) = "驕句虚諤ｧ"
				sort_mode_list(4) = Term("驕句虚諤ｧ")
				sort_mode_type(5) = "繝ｦ繝九ャ繝医Λ繝ｳ繧ｯ"
				sort_mode_list(5) = Term("繝ｩ繝ｳ繧ｯ")
				sort_mode_type(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				sort_mode_list(6) = "繝ｦ繝九ャ繝亥錐遘ｰ"
				sort_mode_type(7) = "Invalid_string_refer_to_original_code"
				sort_mode_list(7) = "Invalid_string_refer_to_original_code"
				'End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'繧ｽ繝ｼ繝域婿豕輔ｒ螟画峩縺励※蜀崎｡ｨ遉ｺ
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo SortAgain
		End Select
		
		'Invalid_string_refer_to_original_code
		u = UList.Item(id_list(ret))
		
MakeEquipedItemList: 
		
		'Invalid_string_refer_to_original_code
		With u
			Do While True
				'Invalid_string_refer_to_original_code
				ReDim part_list(0)
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					buf = .FeatureData("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					arm_point = UBound(part_list) + 1
					ReDim Preserve part_list(UBound(part_list) + 2)
					part_list(1) = "Invalid_string_refer_to_original_code"
					part_list(2) = "Invalid_string_refer_to_original_code"
				End If
				If InStr(buf, "閧ｩ") > 0 Then
					shoulder_point = UBound(part_list) + 1
					ReDim Preserve part_list(UBound(part_list) + 2)
					part_list(UBound(part_list) - 1) = "蜿ｳ閧ｩ"
					part_list(UBound(part_list)) = "蟾ｦ閧ｩ"
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim Preserve part_list(UBound(part_list) + 1)
				part_list(UBound(part_list)) = "Invalid_string_refer_to_original_code"
				'End If
				If InStr(buf, "鬆ｭ") > 0 Then
					ReDim Preserve part_list(UBound(part_list) + 1)
					part_list(UBound(part_list)) = "鬆ｭ"
				End If
				'End If
				For i = 1 To .CountFeature
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					ipart = .FeatureData(i)
					Select Case ipart
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'Invalid_string_refer_to_original_code
						Case Else
							For j = 1 To UBound(part_list)
								If part_list(j) = ipart Then
									Exit For
								End If
							Next 
							If j > UBound(part_list) Then
								ReDim Preserve part_list(UBound(part_list) + .ItemSlotSize(ipart))
								For j = UBound(part_list) - .ItemSlotSize(ipart) + 1 To UBound(part_list)
									part_list(j) = ipart
								Next 
							End If
					End Select
					'End If
				Next 
				
				ReDim Preserve part_list(UBound(part_list) + .MaxItemNum)
				If .IsHero Then
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "Invalid_string_refer_to_original_code"
					Next 
				Else
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "Invalid_string_refer_to_original_code"
					Next 
				End If
				
				'Invalid_string_refer_to_original_code
				If selected_part <> "" Then
					
					ReDim tmp_part_list(UBound(part_list))
					For i = 1 To UBound(part_list)
						tmp_part_list(i) = part_list(i)
					Next 
					
					ReDim part_list(0)
					arm_point = 0
					shoulder_point = 0
					For i = 1 To UBound(tmp_part_list)
						'Invalid_string_refer_to_original_code_
						'Or selected_part = "逶ｾ") _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Or ((selected_part = "閧ｩ" _
						'Or selected_part = "荳｡閧ｩ") _
						'And (tmp_part_list(i) = "蜿ｳ閧ｩ" _
						'Or tmp_part_list(i) = "蟾ｦ閧ｩ")) _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						ReDim Preserve part_list(UBound(part_list) + 1)
						part_list(UBound(part_list)) = tmp_part_list(i)
						Select Case part_list(UBound(part_list))
							Case "Invalid_string_refer_to_original_code"
								arm_point = UBound(part_list)
							Case "蜿ｳ閧ｩ"
								shoulder_point = UBound(part_list)
						End Select
					Next 
				End If
				'Next
				'End If
				
				ReDim part_item(UBound(part_list))
				
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountItem
					With .Item(i)
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextEquipedItem
						'End If
						
						Select Case .Part
							Case "Invalid_string_refer_to_original_code"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point) = .ID
								part_item(arm_point + 1) = ":"
							Case "Invalid_string_refer_to_original_code"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(arm_point) = "" Then
									part_item(arm_point) = .ID
								Else
									part_item(arm_point + 1) = .ID
								End If
							Case "逶ｾ"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point + 1) = .ID
							Case "荳｡閧ｩ"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(shoulder_point) = .ID
							Case "閧ｩ"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(shoulder_point) = "" Then
									part_item(shoulder_point) = .ID
								Else
									part_item(shoulder_point + 1) = .ID
								End If
							Case "髱櫁｡ｨ遉ｺ"
								'Invalid_string_refer_to_original_code
							Case Else
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								For j = 1 To UBound(part_list)
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'And part_item(j) = "" _
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									part_item(j) = .ID
									For k = j + 1 To j + .Size - 1
										If k > UBound(part_item) Then
											Exit For
										End If
										part_item(k) = ":"
									Next 
									Exit For
									'End If
								Next 
								For j = 1 To UBound(part_list)
									If part_list(j) = .Part And part_item(j) = "" Then
										part_item(j) = .ID
										For k = j + 1 To j + .Size - 1
											If k > UBound(part_item) Then
												Exit For
											End If
											part_item(k) = ":"
										Next 
										Exit For
									End If
								Next 
								'End If
								If j > UBound(part_list) And selected_part = "" Then
									ReDim Preserve part_list(UBound(part_list) + 1)
									ReDim Preserve part_item(UBound(part_list))
									part_list(UBound(part_list)) = .Part
									part_item(UBound(part_list)) = .ID
								End If
						End Select
					End With
NextEquipedItem: 
				Next 
				
				ReDim list(UBound(part_list) + 1)
				ReDim id_list(UBound(list))
				ReDim ListItemComment(UBound(list))
				ReDim ListItemFlag(UBound(list))
				
				'Invalid_string_refer_to_original_code
				For i = 1 To UBound(part_item)
					Select Case part_item(i)
						Case ""
							list(i) = RightPaddedString("----", 23) & part_list(i)
						Case ":"
							list(i) = RightPaddedString(" :  ", 23) & part_list(i)
							ListItemComment(i) = ListItemComment(i - 1)
							ListItemFlag(i) = ListItemFlag(i - 1)
						Case Else
							With IList.Item(part_item(i))
								list(i) = RightPaddedString(.Nickname, 22) & " " & part_list(i)
								ListItemComment(i) = .Data.Comment
								id_list(i) = .ID
								For j = i + 1 To i + .Size - 1
									If j > UBound(part_item) Then
										Exit For
									End If
									id_list(j) = .ID
								Next 
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								ListItemFlag(i) = True
								For j = i + 1 To i + .Size - 1
									If j > UBound(part_item) Then
										Exit For
									End If
									ListItemFlag(j) = True
								Next 
								'End If
							End With
					End Select
				Next 
				list(UBound(list)) = "Invalid_string_refer_to_original_code"
				
				'Invalid_string_refer_to_original_code
				caption_str = "Invalid_string_refer_to_original_code" & .Nickname
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
				'End If
				caption_str = caption_str & "  " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxHP) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxEN) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.Armor) & " " & Term("驕句虚諤ｧ", u) & "=" & VB6.Format(.Mobility) & " " & Term("遘ｻ蜍募鴨", u) & "=" & VB6.Format(.Speed)
				TopItem = top_item2
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				top_item2 = TopItem
				If ret = 0 Then
					Exit Do
				End If
				
				'Invalid_string_refer_to_original_code
				If ret = UBound(list) Then
					list(UBound(list)) = "笆ｽ蜈ｨ縺ｦ螟悶☆笆ｽ"
					caption_str = "Invalid_string_refer_to_original_code" & .Nickname
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
				End If
				caption_str = caption_str & "  " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxHP) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxEN) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.Armor) & " " & Term("驕句虚諤ｧ", u) & "=" & VB6.Format(.Mobility) & " " & Term("遘ｻ蜍募鴨", u) & "=" & VB6.Format(.Speed)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If ret <> 0 Then
					If ret < UBound(list) Then
						'Invalid_string_refer_to_original_code
						If id_list(ret) <> "" Then
							.DeleteItem(id_list(ret), False)
						ElseIf LIndex(list(ret), 1) = ":" Then 
							.DeleteItem(id_list(ret - 1), False)
						End If
					Else
						'Invalid_string_refer_to_original_code
						For i = 1 To UBound(list) - 1
							If Not ListItemFlag(i) And id_list(i) <> "" Then
								.DeleteItem(id_list(i), False)
							End If
						Next 
					End If
					If MapFileName = "" Then
						.FullRecover()
					End If
					If MainForm.Visible Then
						DisplayUnitStatus(u)
					End If
				End If
				GoTo NextLoop2
				'End If
				
				'Invalid_string_refer_to_original_code
				iid = id_list(ret)
				If iid <> "" Then
					ipart = IList.Item(iid).Part
				Else
					ipart = LIndex(list(ret), 2)
				End If
				
				'Invalid_string_refer_to_original_code
				Select Case ipart
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						is_right_hand_available = True
						is_left_hand_available = True
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "Invalid_string_refer_to_original_code" Then
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									If is_right_hand_available Then
										is_right_hand_available = False
									Else
										is_left_hand_available = False
									End If
								End If
								'UPGRADE_WARNING: ExchangeItemCommand に変換されていないステートメントがあります。ソース コードを確認してください。
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								is_left_hand_available = False
								'End If
								'End If
							End With
						Next 
					Case "蜿ｳ閧ｩ", "蟾ｦ閧ｩ", "閧ｩ"
						empty_slot = 2
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "閧ｩ" Then
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									empty_slot = empty_slot - 1
								End If
								'End If
							End With
						Next 
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						empty_slot = .MaxItemNum
						For i = 1 To .CountItem
							With .Item(i)
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								empty_slot = empty_slot - .Size
								'End If
								'End If
							End With
						Next 
					Case Else
						empty_slot = 0
						For i = 1 To .CountFeature
							'Invalid_string_refer_to_original_code_
							'And .FeatureData(i) = ipart _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							empty_slot = empty_slot + .FeatureLevel(i)
							'End If
						Next 
						If empty_slot = 0 Then
							empty_slot = 1
						End If
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = ipart Then
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									empty_slot = empty_slot - .Size
								End If
								'End If
							End With
						Next 
				End Select
				
				Do While True
					'Invalid_string_refer_to_original_code
					ReDim item_list(0)
					For	Each it In IList
						With it
							If Not .Exist Then
								GoTo NextItem
							End If
							
							'Invalid_string_refer_to_original_code
							Select Case ipart
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									Select Case .Part
										Case "Invalid_string_refer_to_original_code"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "Invalid_string_refer_to_original_code"
											If u.IsFeatureAvailable("荳｡謇区戟縺｡") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												If Not is_right_hand_available Then
													GoTo NextItem
												End If
											End If
										Case "逶ｾ"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "逶ｾ"
									Select Case .Part
										Case "Invalid_string_refer_to_original_code"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "Invalid_string_refer_to_original_code"
											If u.IsFeatureAvailable("荳｡謇区戟縺｡") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												GoTo NextItem
											End If
										Case "逶ｾ"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "蜿ｳ閧ｩ", "蟾ｦ閧ｩ", "閧ｩ"
									If .Part <> "荳｡閧ｩ" And .Part <> "閧ｩ" Then
										GoTo NextItem
									End If
									If .Part = "荳｡閧ｩ" Then
										If empty_slot < 2 Then
											GoTo NextItem
										End If
									End If
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									GoTo NextItem
									'End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
								Case Else
									If .Part <> ipart Then
										GoTo NextItem
									End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
							End Select
							
							If Not .Unit_Renamed Is Nothing Then
								With .Unit_Renamed.CurrentForm
									'Invalid_string_refer_to_original_code
									If .Status_Renamed = "髮｢閼ｱ" Then
										GoTo NextItem
									End If
									
									'Invalid_string_refer_to_original_code
									If .Party <> "蜻ｳ譁ｹ" Then
										GoTo NextItem
									End If
								End With
								
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								GoTo NextItem
							End If
							'End If
							
							'Invalid_string_refer_to_original_code
							For i = 1 To UBound(item_list)
								If item_list(i) = .Name Then
									GoTo NextItem
								End If
							Next 
						End With
						
						'Invalid_string_refer_to_original_code
						If Not .IsAbleToEquip(it) Then
							GoTo NextItem
						End If
						
						ReDim Preserve item_list(UBound(item_list) + 1)
						item_list(UBound(item_list)) = it.Name
NextItem: 
					Next it
					
					'Invalid_string_refer_to_original_code
					ReDim list(UBound(item_list))
					ReDim strkey_list(UBound(item_list))
					ReDim id_list(UBound(item_list))
					ReDim ListItemFlag(UBound(item_list))
					ReDim ListItemComment(UBound(item_list))
					For i = 1 To UBound(item_list)
						iname = item_list(i)
						With IDList.Item(iname)
							list(i) = RightPaddedString(.Nickname, 22) & " "
							
							If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
								list(i) = list(i) & RightPaddedString(.Part & "[" & VB6.Format(.Size) & "]", 15)
							Else
								list(i) = list(i) & RightPaddedString(.Part, 15)
							End If
							
							'Invalid_string_refer_to_original_code
							inum = 0
							inum2 = 0
							For	Each it In IList
								With it
									If .Name = iname Then
										If .Exist Then
											If .Unit_Renamed Is Nothing Then
												inum = inum + 1
												inum2 = inum2 + 1
											Else
												If .Unit_Renamed.CurrentForm.Status_Renamed <> "髮｢閼ｱ" Then
													'Invalid_string_refer_to_original_code
													'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
													inum = inum + 1
												End If
											End If
										End If
									End If
									'End If
								End With
							Next it
							
							list(i) = list(i) & LeftPaddedString(VB6.Format(inum2), 2) & "/" & LeftPaddedString(VB6.Format(inum), 2)
							
							id_list(i) = .Name
							strkey_list(i) = .KanaName
							ListItemComment(i) = .Comment
						End With
					Next 
					
					'Invalid_string_refer_to_original_code
					For i = 1 To UBound(strkey_list) - 1
						max_item = i
						max_str = strkey_list(i)
						For j = i + 1 To UBound(strkey_list)
							If StrComp(strkey_list(j), max_str, 1) = -1 Then
								max_item = j
								max_str = strkey_list(j)
							End If
						Next 
						If max_item <> i Then
							buf = list(i)
							list(i) = list(max_item)
							list(max_item) = buf
							
							buf = id_list(i)
							id_list(i) = id_list(max_item)
							id_list(max_item) = buf
							
							buf = ListItemComment(i)
							ListItemComment(i) = ListItemComment(max_item)
							ListItemComment(max_item) = buf
							
							strkey_list(max_item) = strkey_list(i)
						End If
					Next 
					
					'Invalid_string_refer_to_original_code
					caption_str = "Invalid_string_refer_to_original_code" & .Nickname
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					'End If
					caption_str = caption_str & "  " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxHP) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxEN) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.Armor) & " " & Term("驕句虚諤ｧ", u) & "=" & VB6.Format(.Mobility) & " " & Term("遘ｻ蜍募鴨", u) & "=" & VB6.Format(.Speed)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					
					'Invalid_string_refer_to_original_code
					If ret = 0 Then
						Exit Do
					End If
					
					iname = id_list(ret)
					
					'Invalid_string_refer_to_original_code
					For	Each it In IList
						With it
							If .Name = iname And .Exist Then
								If .Unit_Renamed Is Nothing Then
									'Invalid_string_refer_to_original_code
									If iid <> "" Then
										u.DeleteItem(iid)
									End If
									'Invalid_string_refer_to_original_code
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									MsgBox(.Nickname & "Invalid_string_refer_to_original_code")
								End If
								u.AddItem(it)
								If MapFileName = "" Then
									u.FullRecover()
								End If
								If MainForm.Visible Then
									DisplayUnitStatus(u)
								End If
								Exit Do
							End If
							'End If
						End With
					Next it
					
					'Invalid_string_refer_to_original_code
					ReDim list(0)
					ReDim id_list(0)
					ReDim ListItemComment(0)
					inum = 0
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					For	Each it In IList
						With it
							If .Name <> iname Or Not .Exist Then
								GoTo NextItem2
							End If
							If .Unit_Renamed Is Nothing Then
								GoTo NextItem2
							End If
							With .Unit_Renamed.CurrentForm
								If .Status_Renamed = "髮｢閼ｱ" Then
									GoTo NextItem2
								End If
								If .Party <> "蜻ｳ譁ｹ" Then
									GoTo NextItem2
								End If
								
								ReDim Preserve list(UBound(list) + 1)
								ReDim Preserve id_list(UBound(list))
								ReDim Preserve ListItemComment(UBound(list))
								
								'Invalid_string_refer_to_original_code_
								'And .CountPilot > 0 _
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								list(UBound(list)) = RightPaddedString(.Nickname0, 36) & " " & .MainPilot.Nickname
								list(UBound(list)) = .Nickname0
								'End If
								id_list(UBound(list)) = it.ID
								
								For i = 1 To .CountItem
									With .Item(i)
										'Invalid_string_refer_to_original_code_
										'And .Part <> "髱櫁｡ｨ遉ｺ" _
										'Then
										'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
										ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
										'End If
									End With
								Next 
								
								inum = inum + 1
							End With
						End With
NextItem2: 
					Next it
					'End If
					ReDim ListItemFlag(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'Invalid_string_refer_to_original_code
					caption_str = IList.Item(id_list(1)).Nickname & "Invalid_string_refer_to_original_code" & .Nickname
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					'End If
					caption_str = caption_str & "  " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxHP) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.MaxEN) & " " & Term("Invalid_string_refer_to_original_code", u) & "=" & VB6.Format(.Armor) & " " & Term("驕句虚諤ｧ", u) & "=" & VB6.Format(.Mobility) & " " & Term("遘ｻ蜍募鴨", u) & "=" & VB6.Format(.Speed)
					TopItem = 1
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'End If
					
					'Invalid_string_refer_to_original_code
					If ret > 0 Then
						If iid <> "" Then
							.DeleteItem(iid)
						End If
						With IList.Item(id_list(ret))
							If Not .Unit_Renamed Is Nothing Then
								.Unit_Renamed.DeleteItem(.ID)
							End If
							
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							MsgBox(.Nickname & "Invalid_string_refer_to_original_code")
						End With
					End If
				Loop 
			Loop 
		End With
		'UPGRADE_WARNING: ExchangeItemCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		If MapFileName = "" Then
			'UPGRADE_WARNING: ExchangeItemCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		End If
		If MainForm.Visible Then
			DisplayUnitStatus(u)
		End If
		Exit Do
		'End If
NextLoop: 
		'Loop
NextLoop2: 
		'Loop
		'End With
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If Not selected_unit Is Nothing Then
			With frmListBox
				.Hide()
				If .txtComment.Enabled Then
					.txtComment.Enabled = False
					.txtComment.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
				End If
			End With
			ReduceListBoxHeight()
			EnlargeListBoxWidth()
			Exit Sub
		End If
		
		GoTo Beginning
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub ExchangeFormCommand()
	Private Sub ExchangeFormCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim key_list() As Integer
		Dim list2() As String
		Dim id_list2() As String
		Dim max_item, max_value As Short
		Dim u As Unit
		Dim buf As String
		Dim ret As Short
		Dim top_item As Short
		Dim farray() As String
		
Beginning: 
		
		top_item = 1
		
		'Invalid_string_refer_to_original_code
		ReDim list(0)
		ReDim id_list(0)
		ReDim ListItemComment(0)
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextLoop
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextLoop
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Exit For
				'End If
			End With
		Next u
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		GoTo NextLoop
		'End If
		
		ReDim Preserve list(UBound(list) + 1)
		ReDim Preserve id_list(UBound(list))
		ReDim Preserve ListItemComment(UBound(list))
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
NextLoop: 
		'Next
		ReDim ListItemFlag(UBound(list))
		
		'Invalid_string_refer_to_original_code
		ReDim key_list(UBound(list))
		With UList
			For i = 1 To UBound(list)
				key_list(i) = .Item(id_list(i)).MaxHP
			Next 
		End With
		For i = 1 To UBound(list) - 1
			max_item = i
			max_value = key_list(i)
			For j = i + 1 To UBound(list)
				If key_list(j) > max_value Then
					max_item = j
					max_value = key_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = list(i)
				list(i) = list(max_item)
				list(max_item) = buf
				
				buf = id_list(i)
				id_list(i) = id_list(max_item)
				id_list(max_item) = buf
				
				buf = ListItemComment(i)
				ListItemComment(i) = ListItemComment(max_item)
				ListItemComment(max_item) = buf
				
				key_list(max_item) = key_list(i)
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		TopItem = top_item
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		top_item = TopItem
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			Exit Sub
		End If
		
		'驕ｸ謚槭＆繧後◆繝ｦ繝九ャ繝医ｒ讀懃ｴ｢
		u = UList.Item(id_list(ret))
		
		'Invalid_string_refer_to_original_code
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ReDim list2(0)
			ReDim id_list2(0)
			ReDim ListItemComment(0)
			For i = 1 To LLength(buf)
				With .OtherForm(LIndex(buf, i))
					If .IsAvailable Then
						ReDim Preserve list2(UBound(list2) + 1)
						ReDim Preserve id_list2(UBound(list2))
						ReDim Preserve ListItemComment(UBound(list2))
						
						'Invalid_string_refer_to_original_code
						.Rank = u.Rank
						.BossRank = u.BossRank
						.Update()
						
						'Invalid_string_refer_to_original_code
						id_list2(UBound(list2)) = .Name
						If u.Nickname0 = .Nickname Then
							list2(UBound(list2)) = RightPaddedString(.Name, 27)
						Else
							list2(UBound(list2)) = RightPaddedString(.Nickname0, 27)
						End If
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5) & " " & .Data.Adaption
						
						'Invalid_string_refer_to_original_code
						max_value = 0
						For j = 1 To .CountWeapon
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							If .WeaponPower(j, "") > max_value Then
								max_value = .WeaponPower(j, "")
							End If
						Next 
					End If
				End With
			Next 
			list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 7)
			
			'Invalid_string_refer_to_original_code
			max_value = 0
			For j = 1 To .CountWeapon
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .WeaponMaxRange(j) > max_value Then
					max_value = .WeaponMaxRange(j)
				End If
				'End If
			Next 
			list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 5)
			
			'Invalid_string_refer_to_original_code
			ReDim farray(0)
			For j = 1 To .CountFeature
				If .FeatureName(j) <> "" Then
					'Invalid_string_refer_to_original_code
					For k = 1 To UBound(farray)
						If .FeatureName(j) = farray(k) Then
							Exit For
						End If
					Next 
					If k > UBound(farray) Then
						ListItemComment(UBound(list2)) = ListItemComment(UBound(list2)) & .FeatureName(j) & " "
						ReDim Preserve farray(UBound(farray) + 1)
						farray(UBound(farray)) = .FeatureName(j)
					End If
				End If
			Next 
			'End If
		End With
		'Next
		ReDim ListItemFlag(UBound(list2))
		
		'Invalid_string_refer_to_original_code
		TopItem = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			GoTo Beginning
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		
		GoTo Beginning
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function InStatusCommand() As Boolean
		If MapFileName = "" Then
			If InStr(ScenarioFileName, "Invalid_string_refer_to_original_code") > 0 Or InStr(ScenarioFileName, "Invalid_string_refer_to_original_code") > 0 Or IsSubStage Then
				InStatusCommand = True
			End If
		End If
	End Function
End Module