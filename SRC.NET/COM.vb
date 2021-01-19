Option Strict Off
Option Explicit On
Module COM
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Public Sub OperateUnit()
		Dim j, i, tmp As Short
		Dim w, tw As Short
		Dim wname, twname As String
		Dim u As Unit
		Dim tmp_w As Short
		Dim prob, dmg As Integer
		Dim tprob, tdmg As Integer
		Dim max_prob, max_dmg As Integer
		Dim max_range, min_range As Short
		Dim dst_x, dst_y As Short
		Dim new_x, new_y As Short
		Dim new_x0, new_y0 As Short
		Dim tx, ty As Short
		Dim new_locations_value As Short
		Dim distance As Short
		Dim def_mode As String
		Dim is_suiside As Boolean
		Dim moved, transfered As Boolean
		Dim mmode As String
		Dim searched_enemy, searched_nearest_enemy As Boolean
		Dim guard_unit_mode As Boolean
		Dim buf As String
		Dim partners() As Unit
		Dim prev_money, earnings As Integer
		Dim BGM As String
		Dim attack_target As Unit
		Dim attack_target_hp_ratio As Double
		Dim defense_target As Unit
		Dim defense_target_hp_ratio As Double
		Dim defense_target2 As Unit
		Dim defense_target2_hp_ratio As Double
		Dim support_attack_done As Boolean
		Dim w2 As Short
		Dim indirect_attack As Boolean
		Dim is_p_weapon As Boolean
		Dim took_action As Boolean
		
		SelectedUnitForEvent = SelectedUnit
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SelectedTarget ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SelectedTargetForEvent ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SelectedTargetForEvent = Nothing
		SelectedWeapon = 0
		SelectedTWeapon = 0
		SelectedAbility = 0
		SelectedUnitMoveCost = 0
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.Update()
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.MaxAction = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.IsConditionSatisfied("è¸Šã‚Š") Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsOptionDefined("æ•µãƒ¦ãƒ‹ãƒƒãƒˆã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼ä½¿ç”¨") Or IsOptionDefined("æ•µãƒ¦ãƒ‹ãƒƒãƒˆç²¾ç¥ã‚³ãƒãƒ³ãƒ‰ä½¿ç”¨") Then
			TrySpecialPower(SelectedUnit.MainPilot)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			For i = 2 To SelectedUnit.CountPilot
				TrySpecialPower(SelectedUnit.Pilot(i))
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
			For i = 1 To SelectedUnit.CountSupport
				TrySpecialPower(SelectedUnit.Support(i))
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			TrySpecialPower(SelectedUnit.AdditionalSupport)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		TryHyperMode()
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If LLength(.Mode) = 2 Then
				dst_x = CShort(LIndex(.Mode, 1))
				dst_y = CShort(LIndex(.Mode, 2))
				If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
					GoTo Move
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .Mode = "Invalid_string_refer_to_original_code" Then
				GoTo Move
			End If
			
			'Invalid_string_refer_to_original_code
			If Not PList.IsDefined(.Mode) Then
				GoTo TryBattleTransform
			End If
			If PList.Item(.Mode).Unit_Renamed Is Nothing Then
				GoTo TryBattleTransform
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			GoTo TryBattleTransform
			'End If
			SelectedTarget = PList.Item(.Mode).Unit_Renamed
			AreaInSpeed(SelectedUnit)
			If Not .IsAlly(SelectedTarget) Then
				'Invalid_string_refer_to_original_code
				w = SelectWeapon(SelectedUnit, SelectedTarget, "ç§»å‹•å¯èƒ½")
				If w = 0 Then
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			End If
			
			If tmp_w > 0 Then
				'Invalid_string_refer_to_original_code
				If distance > System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
					'Invalid_string_refer_to_original_code
					SelectedTarget = u
					w = tmp_w
					distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y)
					max_prob = prob
					max_dmg = dmg
				ElseIf distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then 
					'ä»Šã¾ã§ã«è¦‹ã¤ã‹ã£ãŸãƒ¦ãƒ‹ãƒƒãƒˆã¨ä½ç½®ãŒå¤‰ã‚ã‚‰ãªã‘ã‚Œã°
					'Invalid_string_refer_to_original_code
					If prob > max_prob And prob > 50 Then
						SelectedTarget = u
						w = tmp_w
						max_prob = prob
					ElseIf max_prob = 0 And dmg > max_dmg Then 
						SelectedTarget = u
						w = tmp_w
						max_dmg = dmg
					End If
				End If
			End If
		End With
		'Next
		
		If w = 0 Then
			'Invalid_string_refer_to_original_code
			GoTo Move
		Else
			'Invalid_string_refer_to_original_code
			GoTo AttackEnemy
		End If
		'End If
		'End With
		
TryBattleTransform: 
		'æˆ¦é—˜å½¢æ…‹ã¸ã®å¤‰å½¢ãŒå¯èƒ½ã§ã‚ã‚Œã°å¤‰å½¢
		If TryBattleTransform() Then
			transfered = True
			'Invalid_string_refer_to_original_code
			If w > 0 Then
				w = SelectWeapon(SelectedUnit, SelectedTarget, "ç§»å‹•å¯èƒ½")
				If w = 0 Then
					'Invalid_string_refer_to_original_code
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		TryInstantAbility()
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		With SelectedUnit
			If .HP = 0 Or .MaxAction = 0 Then
				GoTo EndOfOperation
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not SelectedTarget Is Nothing Then
			GoTo AttackEnemy
		End If
		
		'Invalid_string_refer_to_original_code
		If TrySummonning() Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'Invalid_string_refer_to_original_code
		If TryFix(moved) Then
			GoTo EndOfOperation
		End If
		
		'Invalid_string_refer_to_original_code
		If TryMapHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'Invalid_string_refer_to_original_code
		If TryHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
TryMapAttack: 
		'Invalid_string_refer_to_original_code
		If TryMapAttack(moved) Then
			GoTo EndOfOperation
		End If
		
SearchNearestEnemyWithinRange: 
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			AreaInSpeed(SelectedUnit)
			
			'Invalid_string_refer_to_original_code
			If guard_unit_mode Then
				With PList.Item(.Mode).Unit_Renamed
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If Not MaskData(i, j) Then
								If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > 1 Then
									MaskData(i, j) = True
								End If
							End If
						Next 
					Next 
				End With
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SelectedTarget ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SelectedTarget = Nothing
			w = 0
			max_prob = 0
			max_dmg = 0
			For	Each u In UList
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextLoop
				'End If
				
				'Invalid_string_refer_to_original_code
				If .IsAlly(u) Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				Select Case .Mode
					Case "å‘³æ–¹"
						If u.Party <> "å‘³æ–¹" And u.Party <> "Invalid_string_refer_to_original_code" Then
							GoTo NextLoop
						End If
					Case "Invalid_string_refer_to_original_code", "æ•µ", "Invalid_string_refer_to_original_code"
						If u.Party <> .Mode Then
							GoTo NextLoop
						End If
				End Select
				
				'Invalid_string_refer_to_original_code
				If SelectedUnit.CurrentForm Is u.CurrentForm Then
					GoTo NextLoop
				End If
				
				'éš ã‚Œèº«ä¸­
				If u.IsUnderSpecialPowerEffect("éš ã‚Œèº«") Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If u.IsFeatureLevelSpecified("Invalid_string_refer_to_original_code") Then
					If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > u.FeatureLevel("Invalid_string_refer_to_original_code") Then
						GoTo NextLoop
					End If
				Else
					If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > 3 Then
						GoTo NextLoop
					End If
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				If moved Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					tmp_w = SelectWeapon(SelectedUnit, u, "ç§»å‹•å¯èƒ½", prob, dmg)
				End If
				If tmp_w <= 0 Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				If .MainPilot.TacticalTechnique >= 150 Then
					If Not u.LookForSupportGuard(SelectedUnit, tmp_w) Is Nothing Then
						'Invalid_string_refer_to_original_code
						prob = 0
						'Invalid_string_refer_to_original_code
						dmg = dmg \ 2
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'And Not .IsConditionSatisfied("æš´èµ°") _
				'And Not .IsConditionSatisfied("æ··ä¹±") _
				'And Not .IsConditionSatisfied("ç‹‚æˆ¦å£«") _
				'And Not indirect_attack _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				tw = SelectWeapon(u, SelectedUnit, "åæ’ƒ", tprob, tdmg)
				If prob < 80 And tprob > prob Then
					GoTo NextLoop
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If prob > 50 Then
					'Invalid_string_refer_to_original_code
					If .MainPilot.TacticalTechnique >= 150 Then
						With u
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							prob = 1.5 * prob
						End With
					Else
						'Invalid_string_refer_to_original_code
						For i = 1 To .CountAbility
							With .Ability(i)
								If .MaxRange > 0 Then
									If .CountEffect > 0 Then
										If .EffectType(1) = "å›å¾©" Then
											prob = 1.5 * prob
											Exit For
										End If
									End If
								End If
							End With
						Next 
					End If
				End If
			Next u
		End With
		'End If
		
		If prob > max_prob Then
			SelectedTarget = u
			w = tmp_w
			max_prob = prob
		End If
		'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		Dim list() As String
		Dim caption_msg As String
		Dim hit_prob, crit_prob As Short
		Dim get_reward As Boolean
		'Invalid_string_refer_to_original_code
		tw = 0
		For i = 1 To u.CountWeapon
			If u.IsWeaponAvailable(i, "ç§»å‹•å‰") And Not u.IsWeaponClassifiedAs(i, "Invalid_string_refer_to_original_code") Then
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			Else
				'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'End If
		Next 
		
		'Invalid_string_refer_to_original_code
		If indirect_attack Then
			tw = 0
		End If
		
		'Invalid_string_refer_to_original_code
		If u.MaxAction = 0 Or u.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
			tw = 0
		End If
		
		'Invalid_string_refer_to_original_code
		If tw = 0 Then
			dmg = 1.5 * dmg
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End With
		'End If
		
		If dmg >= max_dmg Then
			'Invalid_string_refer_to_original_code
			SelectedTarget = u
			w = tmp_w
			max_dmg = dmg
		End If
		'End If
NextLoop: 
		'Next
		
		'Invalid_string_refer_to_original_code
		If SelectedTarget Is Nothing Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			GoTo EndOfOperation
		End If
		
		If moved Then
			'Invalid_string_refer_to_original_code
			GoTo EndOfOperation
		End If
		
		If searched_enemy Then
			'Invalid_string_refer_to_original_code
			GoTo EndOfOperation
		End If
		
		'ä¸€åº¦ç´¢æ•µã‚’ã—ãŸã“ã¨ã‚’è¨˜éŒ²
		searched_enemy = True
		
		'Invalid_string_refer_to_original_code
		GoTo SearchNearestEnemy
		'End If
		searched_enemy = True
		'End With
		
AttackEnemy: 
		'Invalid_string_refer_to_original_code
		
		'æ•µã‚’Update
		SelectedTarget.Update()
		
		'æ•µã®ä½ç½®ã‚’è¨˜éŒ²ã—ã¦ãŠã
		tx = SelectedTarget.X
		ty = SelectedTarget.Y
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			If .IsTargetWithinRange(w, SelectedTarget) Then
				new_locations_value = TerrainEffectForHPRecover(.X, .Y) + TerrainEffectForENRecover(.X, .Y) + 100 * .LookForSupport(.X, .Y, True)
				If .Area <> "ç©ºä¸­" Then
					'Invalid_string_refer_to_original_code
					new_locations_value = new_locations_value + TerrainEffectForHit(.X, .Y) + TerrainEffectForDamage(.X, .Y)
				End If
				new_x = .X
				new_y = .Y
			Else
				new_locations_value = -1000
				new_x = 0
				new_y = 0
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			max_range = .WeaponMaxRange(w)
			min_range = .Weapon(w).MinRange
			For i = MaxLng(1, tx - max_range) To MinLng(tx + max_range, MapWidth)
				For j = MaxLng(1, ty - (max_range - System.Math.Abs(tx - i))) To MinLng(ty + (max_range - System.Math.Abs(tx - i)), MapHeight)
					If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) >= min_range Then
						tmp = TerrainEffectForHPRecover(i, j) + TerrainEffectForENRecover(i, j) + 100 * .LookForSupport(i, j, True)
						
						If .Area <> "ç©ºä¸­" Then
							'Invalid_string_refer_to_original_code
							tmp = tmp + TerrainEffectForHit(i, j) + TerrainEffectForDamage(i, j)
							
							'Invalid_string_refer_to_original_code
							If TerrainClass(i, j) = "æ°´" Then
								If .IsTransAvailable("æ°´") Then
									tmp = tmp + 100
								Else
									tmp = -1000
								End If
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = tmp - System.Math.Sqrt(System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2)
						
						If new_locations_value < tmp Then
							new_locations_value = tmp
							new_x = i
							new_y = j
						End If
					End If
				Next 
			Next 
			
			If new_x = 0 And new_y = 0 Then
				'Invalid_string_refer_to_original_code
				If searched_nearest_enemy Then
					'Invalid_string_refer_to_original_code
					GoTo EndOfOperation
				End If
				GoTo SearchNearestEnemy
			End If
			
			'Invalid_string_refer_to_original_code
			If new_x <> .X Or new_y <> .Y Then
				.Move(new_x, new_y)
				SelectedUnitMoveCost = TotalMoveCost(new_x, new_y)
				moved = True
				
				'Invalid_string_refer_to_original_code
				If .EN = 0 Then
					If .MaxAction = 0 Then
						GoTo EndOfOperation
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				If TryMapAttack(True) Then
					GoTo EndOfOperation
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If w = 0 Then
					'Invalid_string_refer_to_original_code
					GoTo EndOfOperation
				End If
			End If
			'End If
			
			'ãƒ¦ãƒ‹ãƒƒãƒˆã‚’ä¸­å¤®è¡¨ç¤º
			Center(.X, .Y)
			
			'ãƒã‚¤ãƒ©ã‚¤ãƒˆè¡¨ç¤ºã‚’è¡Œã†
			If Not BattleAnimation Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'            AreaInRange .X, .Y, _
				''                .Weapon(w).MinRange, _
				''                .WeaponMaxRange(w), _
				'Invalid_string_refer_to_original_code
				AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
			End If
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .WeaponMaxRange(w) = 1 Then
				.CombinationPartner("Invalid_string_refer_to_original_code")
				SelectedTarget.X( , SelectedTarget.Y)
			Else
				.CombinationPartner("Invalid_string_refer_to_original_code")
			End If
			If Not BattleAnimation Then
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.X, .Y) = False
					End With
				Next 
			End If
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			If Not BattleAnimation Then
				'Invalid_string_refer_to_original_code
				MaskData(.X, .Y) = False
				MaskData(SelectedTarget.X, SelectedTarget.Y) = False
				
				'ãƒã‚¤ãƒ©ã‚¤ãƒˆè¡¨ç¤ºã‚’å®Ÿæ–½
				MaskScreen()
			Else
				'Invalid_string_refer_to_original_code
				RefreshScreen()
			End If
			
			'Invalid_string_refer_to_original_code
			If Not KeepEnemyBGM Then
				BGM = ""
				
				'Invalid_string_refer_to_original_code
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") And InStr(.MainPilot.Name, "(ã‚¶ã‚³)") = 0 Then
					BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
				End If
				
				BossBGM = False
				If Len(BGM) > 0 Then
					'Invalid_string_refer_to_original_code
					ChangeBGM(BGM)
					BossBGM = True
				Else
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
					If SelectedTarget.Party = "å‘³æ–¹" Or (SelectedTarget.Party = "Invalid_string_refer_to_original_code" And .Party <> "Invalid_string_refer_to_original_code") Then
						'Invalid_string_refer_to_original_code
						If SelectedTarget.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
							BGM = SearchMidiFile(SelectedTarget.FeatureData("Invalid_string_refer_to_original_code"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(SelectedTarget.MainPilot.BGM)
						End If
					Else
						'Invalid_string_refer_to_original_code
						If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
							BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(.MainPilot.BGM)
						End If
					End If
					If Len(BGM) = 0 Then
						BGM = BGMName("default")
					End If
					
					'Invalid_string_refer_to_original_code
					ChangeBGM(BGM)
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			is_p_weapon = .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			
			'Invalid_string_refer_to_original_code
			def_mode = ""
			UseSupportGuard = True
			If SelectedTarget.MaxAction = 0 Then
				'Invalid_string_refer_to_original_code
				
				tw = -1
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				def_mode = "é˜²å¾¡"
			End If
			
			'UPGRADE_ISSUE: Control mnuMapCommandItem ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If BattleAnimation Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'                AreaInRange .X, .Y, _
				''                    .Weapon(w).MinRange, _
				''                    .WeaponMaxRange(w), _
				'Invalid_string_refer_to_original_code
				AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.X, .Y) = False
					End With
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			MaskData(.X, .Y) = False
			MaskData(SelectedTarget.X, SelectedTarget.Y) = False
			
			'ãƒã‚¤ãƒ©ã‚¤ãƒˆè¡¨ç¤ºã‚’å®Ÿæ–½
			MaskScreen()
			'End If
			
			
			hit_prob = .HitProbability(w, SelectedTarget, True)
			crit_prob = .CriticalProbability(w, SelectedTarget)
			caption_msg = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'& Format$(.WeaponPower(w, ""))
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				caption_msg = caption_msg & "Invalid_string_refer_to_original_code" & VB6.Format(MinLng(hit_prob, 100)) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			
			ReDim list(3)
			
			If IsAbleToCounterAttack(SelectedTarget, SelectedUnit) And Not indirect_attack Then
				list(1) = "åæ’ƒ"
			Else
				list(1) = "Invalid_string_refer_to_original_code"
			End If
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				list(2) = "Invalid_string_refer_to_original_code"
				& Format$(MinLng(hit_prob, 100)) _
				Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				list(3) = "Invalid_string_refer_to_original_code"
				& Format$(MinLng(hit_prob \ 2, 100)) _
				Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Else
				list(2) = "é˜²å¾¡"
				list(3) = "å›é¿"
			End If
			
			'Invalid_string_refer_to_original_code
			SupportGuardUnit = SelectedTarget.LookForSupportGuard(SelectedUnit, w)
			If Not SupportGuardUnit Is Nothing Then
				ReDim Preserve list(4)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				list(4) = "Invalid_string_refer_to_original_code" & SupportGuardUnit.Nickname & ")"
			Else
				list(4) = "Invalid_string_refer_to_original_code" & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
			End If
			UseSupportGuard = True
			'End If
			
			AddPartsToListBox()
			Do 
				'Invalid_string_refer_to_original_code
				With SelectedTarget
					ReDim ListItemFlag(UBound(list))
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
					If list(1) = "åæ’ƒ" Then
						ListItemFlag(1) = False
						tw = -1
					Else
						ListItemFlag(1) = True
						tw = 0
					End If
					
					'Invalid_string_refer_to_original_code
					If .IsFeatureAvailable("é˜²å¾¡ä¸å¯") Then
						ListItemFlag(2) = True
					Else
						ListItemFlag(2) = False
					End If
					
					'Invalid_string_refer_to_original_code
					If .IsFeatureAvailable("å›é¿ä¸å¯") Or .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
						ListItemFlag(3) = True
					Else
						ListItemFlag(3) = False
					End If
					
					'Invalid_string_refer_to_original_code
					TopItem = 1
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End With
				
				Select Case i
					Case 1
						'Invalid_string_refer_to_original_code
						buf = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'& Format$(.WeaponPower(w, ""))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
							buf = buf & "Invalid_string_refer_to_original_code" & VB6.Format(MinLng(hit_prob, 100)) & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						End If
						With SelectedTarget.MainPilot
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							If .HasMana() Then
								buf = buf & Term("é­”åŠ›", SelectedTarget) & VB6.Format(.Shooting)
							Else
								buf = buf & Term("Invalid_string_refer_to_original_code", SelectedTarget) & VB6.Format(.Shooting)
							End If
						End With
						
						tw = WeaponListBox(SelectedTarget, buf, "åæ’ƒ")
						
						If tw = 0 Then
							i = 0
						End If
					Case 2
						'Invalid_string_refer_to_original_code
						def_mode = "é˜²å¾¡"
					Case 3
						'Invalid_string_refer_to_original_code
						def_mode = "å›é¿"
					Case 4
						'Invalid_string_refer_to_original_code
						UseSupportGuard = Not UseSupportGuard
						If UseSupportGuard Then
							list(4) = "Invalid_string_refer_to_original_code"
						Else
							list(4) = "Invalid_string_refer_to_original_code"
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						list(4) = list(4) & SupportGuardUnit.Nickname & ")"
						list(4) = list(4) & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
						'End If
						i = 0
					Case Else
						'Invalid_string_refer_to_original_code
						If ListItemFlag(1) And ListItemFlag(2) And ListItemFlag(3) Then
							Exit Do
						End If
				End Select
			Loop While i = 0
			
			'Invalid_string_refer_to_original_code
			frmListBox.Hide()
			RemovePartsOnListBox()
			
			'ãƒã‚¤ãƒ©ã‚¤ãƒˆè¡¨ç¤ºã‚’æ¶ˆå»
			If BattleAnimation Then
				RefreshScreen()
			End If
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			tw = SelectWeapon(SelectedTarget, SelectedUnit, "åæ’ƒ")
			If indirect_attack Then
				tw = 0
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense() ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			def_mode = SelectDefense(SelectedUnit, w, SelectedTarget, tw)
			If def_mode <> "" Then
				tw = -1
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not KeepEnemyBGM Then
			With SelectedTarget
				If .Party = "å‘³æ–¹" And tw > 0 And .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = .Weapon(tw).Name Then
							'Invalid_string_refer_to_original_code
							BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
							If Len(BGM) > 0 Then
								'Invalid_string_refer_to_original_code
								BossBGM = False
								ChangeBGM(BGM)
							End If
							Exit For
						End If
					Next 
				End If
			End With
		End If
		
		SelectedWeapon = w
		SelectedTWeapon = tw
		SelectedDefenseOption = def_mode
		
		wname = SelectedUnit.Weapon(w).Name
		SelectedWeaponName = wname
		If tw > 0 Then
			twname = SelectedTarget.Weapon(tw).Name
			SelectedTWeaponName = twname
		Else
			SelectedTWeaponName = ""
		End If
		
		'Invalid_string_refer_to_original_code
		'æˆ¦é—˜å‰ã«ä¸€æ—¦ã‚¯ãƒªã‚¢
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportGuardUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportGuardUnit2 ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SupportGuardUnit2 = Nothing
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		HandleEvent("ä½¿ç”¨", SelectedUnit.MainPilot.ID, wname)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		If tw > 0 Then
			twname = SelectedTarget.Weapon(tw).Name
			SaveSelections()
			SwapSelections()
			HandleEvent("ä½¿ç”¨", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Stage = "Invalid_string_refer_to_original_code" Then
			OpenMessageForm(SelectedTarget, SelectedUnit)
		Else
			OpenMessageForm(SelectedUnit, SelectedTarget)
		End If
		
		'Invalid_string_refer_to_original_code
		AttackUnit = SelectedUnit
		attack_target = SelectedUnit
		attack_target_hp_ratio = SelectedUnit.HP / SelectedUnit.MaxHP
		defense_target = SelectedTarget
		defense_target_hp_ratio = SelectedTarget.HP / SelectedTarget.MaxHP
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg defense_target2 ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		defense_target2 = Nothing
		'Invalid_string_refer_to_original_code
		'    Set SupportAttackUnit = Nothing
		'    Set SupportGuardUnit = Nothing
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			'        If tw > 0 And .MaxAction > 0 Then
			'Invalid_string_refer_to_original_code
			If .MaxAction > 0 And .IsWeaponAvailable(tw, "ç§»å‹•å‰") Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				def_mode = "Invalid_string_refer_to_original_code"
				.Attack(tw, SelectedUnit, "Invalid_string_refer_to_original_code")
				SelectedTarget = .CurrentForm
				'Invalid_string_refer_to_original_code_
				'Or .MainPilot.SkillLevel("å…ˆèª­ã¿") >= Dice(16) _
				'Or .IsUnderSpecialPowerEffect("ã‚«ã‚¦ãƒ³ã‚¿ãƒ¼") _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				def_mode = "Invalid_string_refer_to_original_code"
				.Attack(tw, SelectedUnit, "ã‚«ã‚¦ãƒ³ã‚¿ãƒ¼", "")
				SelectedTarget = .CurrentForm
			ElseIf .MaxCounterAttack > .UsedCounterAttack Then 
				def_mode = "Invalid_string_refer_to_original_code"
				.UsedCounterAttack = .UsedCounterAttack + 1
				.Attack(tw, SelectedUnit, "ã‚«ã‚¦ãƒ³ã‚¿ãƒ¼", "")
				SelectedTarget = .CurrentForm
			End If
			
			'Invalid_string_refer_to_original_code
			If Not SupportGuardUnit Is Nothing Then
				attack_target = SupportGuardUnit
				attack_target_hp_ratio = SupportGuardUnitHPRatio
			End If
			'End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SupportAttackUnit = .LookForSupportAttack(SelectedTarget)
			
			'Invalid_string_refer_to_original_code
			If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SupportAttackUnit = Nothing
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SupportAttackUnit = Nothing
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .Master.Party = SelectedTarget.Party Then
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SupportAttackUnit = Nothing
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("è¸Šã‚Š") Then
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SupportAttackUnit = Nothing
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			''            And .MaxAction(True) > 0 _
			'Invalid_string_refer_to_original_code_
			''        Then
			'Invalid_string_refer_to_original_code_
			'And .MaxAction(True) > 0 _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If w > .CountWeapon Then
				w = -1
			ElseIf wname <> .Weapon(w).Name Then 
				w = -1
			ElseIf moved Then 
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				w = -1
			End If
			If Not .IsWeaponAvailable(w, "ç§»å‹•å‰") Then
				w = -1
			End If
			'End If
			If w > 0 Then
				If Not .IsTargetWithinRange(w, SelectedTarget) Then
					w = 0
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .MaxAction(True) = 0 Then
				w = -1
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			w = -1
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .Master.Party = SelectedTarget.Party Then
				w = -1
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("è¸Šã‚Š") Then
				w = -1
			End If
			
			If w > 0 Then
				'Invalid_string_refer_to_original_code
				If .IsWeaponClassifiedAs(w, "è‡ª") Then
					is_suiside = True
				End If
				
				If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
					'Invalid_string_refer_to_original_code
					.Attack(w, SelectedTarget, "Invalid_string_refer_to_original_code")
				Else
					'Invalid_string_refer_to_original_code
					.Attack(w, SelectedTarget, "", def_mode)
				End If
			ElseIf w = 0 Then 
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.PlayAnimation("Invalid_string_refer_to_original_code")
			Else
				.SpecialEffect("Invalid_string_refer_to_original_code")
			End If
			.PilotMessage("Invalid_string_refer_to_original_code")
			'End If
			w = -1
			'End If
			SelectedUnit = .CurrentForm
			
			'Invalid_string_refer_to_original_code
			If Not SupportGuardUnit Is Nothing Then
				defense_target2 = SupportGuardUnit
				defense_target2_hp_ratio = SupportGuardUnitHPRatio
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not SupportAttackUnit Is Nothing Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SupportAttackUnit = Nothing
		End If
		'End If
		If Not SupportAttackUnit Is Nothing Then
			If SelectedUnit.MaxSyncAttack > SelectedUnit.UsedSyncAttack Then
				With SupportAttackUnit
					'Invalid_string_refer_to_original_code
					w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "Invalid_string_refer_to_original_code")
					
					If w2 > 0 Then
						'Invalid_string_refer_to_original_code
						MaskData(.X, .Y) = False
						If Not BattleAnimation Then
							MaskScreen()
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						.PlayAnimation("Invalid_string_refer_to_original_code")
					End If
					UpdateMessageForm(SelectedTarget, SupportAttackUnit)
					.Attack(w2, SelectedTarget, "Invalid_string_refer_to_original_code")
				End With
			End If
			'End With
			
			'å¾Œå§‹æœ«
			With SupportAttackUnit.CurrentForm
				If w2 > 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					.PlayAnimation("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				.UsedSupportAttack = .UsedSupportAttack + 1
				
				'Invalid_string_refer_to_original_code
				SelectedUnit.UsedSyncAttack = SelectedUnit.UsedSyncAttack + 1
			End With
		End If
		'End With
		
		support_attack_done = True
		
		'Invalid_string_refer_to_original_code
		'å…¥ã‚Œæ›¿ãˆã¦è¨˜éŒ²
		If Not SupportGuardUnit Is Nothing Then
			defense_target = SupportGuardUnit
			defense_target_hp_ratio = SupportGuardUnitHPRatio
		End If
		'End If
		'End If
		
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			If tw > 0 Then
				If tw > .CountWeapon Then
					tw = -1
				ElseIf twname <> .Weapon(tw).Name Or Not .IsWeaponAvailable(tw, "ç§»å‹•å‰") Then 
					tw = -1
				End If
			End If
			If tw > 0 Then
				If Not .IsTargetWithinRange(tw, SelectedUnit) Then
					'Invalid_string_refer_to_original_code
					tw = 0
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .MaxAction = 0 Then
				tw = -1
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			tw = -1
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .Master.Party = SelectedUnit.Party Then
				tw = -1
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("è¸Šã‚Š") Then
				tw = -1
			End If
			
			If tw > 0 And def_mode = "" Then
				'åæ’ƒã‚’å®Ÿæ–½
				.Attack(tw, SelectedUnit, "", "")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectedTarget = .CurrentForm
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SelectedUnit = SelectedUnit.CurrentForm
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'                    If Not SupportGuardUnit Is Nothing Then
			'                        Set attack_target = SupportGuardUnit
			'                        attack_target_hp_ratio = SupportGuardUnitHPRatio
			'                    End If
			If Not SupportGuardUnit2 Is Nothing Then
				attack_target = SupportGuardUnit2
				attack_target_hp_ratio = SupportGuardUnitHPRatio2
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.PlayAnimation("Invalid_string_refer_to_original_code")
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'End If
			.PilotMessage("Invalid_string_refer_to_original_code")
			tw = -1
			'End If
			tw = -1
			'End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not SupportAttackUnit Is Nothing Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or support_attack_done _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SupportAttackUnit ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SupportAttackUnit = Nothing
		End If
		'End If
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit
				'Invalid_string_refer_to_original_code
				w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "Invalid_string_refer_to_original_code")
				
				If w2 > 0 Then
					'Invalid_string_refer_to_original_code
					MaskData(.X, .Y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					.PlayAnimation("Invalid_string_refer_to_original_code")
				End If
				UpdateMessageForm(SelectedTarget, SupportAttackUnit)
				.Attack(w2, SelectedTarget, "Invalid_string_refer_to_original_code")
			End With
		End If
		'End With
		
		'å¾Œå§‹æœ«
		With SupportAttackUnit.CurrentForm
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'End If
			
			'Invalid_string_refer_to_original_code
			If w2 > 0 Then
				.UsedSupportAttack = .UsedSupportAttack + 1
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		'å…¥ã‚Œæ›¿ãˆã¦è¨˜éŒ²
		If Not SupportGuardUnit Is Nothing Then
			defense_target = SupportGuardUnit
			defense_target_hp_ratio = SupportGuardUnitHPRatio
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		SelectedUnit = SelectedUnit.CurrentForm
		With SelectedTarget
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			get_reward = True
			'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'And Not .IsConditionSatisfied("æ··ä¹±") _
			'And Not .IsConditionSatisfied("æš´èµ°") _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			get_reward = True
			'End If
			'End If
			
			If get_reward Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				.GetExp(SelectedUnit, "Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				prev_money = Money
				
				'Invalid_string_refer_to_original_code
				earnings = SelectedUnit.Value \ 2
				
				'Invalid_string_refer_to_original_code
				If .IsUnderSpecialPowerEffect("Invalid_string_refer_to_original_code") Then
					earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("Invalid_string_refer_to_original_code"))
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			IncrMoney(earnings)
			
			If Money > prev_money Then
				DisplaySysMessage(VB6.Format(Money - prev_money) & "ã®" & Term("Invalid_string_refer_to_original_code", SelectedUnit) & "Invalid_string_refer_to_original_code")
			End If
			.GetExp(SelectedUnit, "Invalid_string_refer_to_original_code")
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			If earnings > 0 Then
				.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		SelectedUnit = SelectedUnit.CurrentForm
		With SelectedUnit
			If Not .Summoner Is Nothing Then
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'And Not .IsConditionSatisfied("æ··ä¹±") _
				'And Not .IsConditionSatisfied("æš´èµ°") _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				earnings = SelectedTarget.Value \ 2
				
				'Invalid_string_refer_to_original_code
				If .IsUnderSpecialPowerEffect("Invalid_string_refer_to_original_code") Then
					earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("Invalid_string_refer_to_original_code"))
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			IncrMoney(earnings)
			If earnings > 0 Then
				DisplaySysMessage(VB6.Format(earnings) & "ã®" & Term("Invalid_string_refer_to_original_code", SelectedTarget) & "Invalid_string_refer_to_original_code")
			End If
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
			'End If
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			If .IsUnderSpecialPowerEffect("æ•µç ´å£Šæ™‚å†è¡Œå‹•") Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.UsedAction = .UsedAction - 1
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			If earnings > 0 Then
				.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			End If
			'End If
		End With
		
		CloseMessageForm()
		
		RedrawScreen()
		
		'Invalid_string_refer_to_original_code
		With attack_target.CurrentForm
			.UpdateCondition()
			.Update()
		End With
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit.CurrentForm
				.UpdateCondition()
				.Update()
			End With
		End If
		With defense_target.CurrentForm
			.UpdateCondition()
			.Update()
		End With
		If Not defense_target2 Is Nothing Then
			With defense_target2.CurrentForm
				.UpdateCondition()
				.Update()
			End With
		End If
		
		If SelectedWeapon <= 0 Then
			SelectedWeaponName = ""
		End If
		If SelectedTWeapon <= 0 Then
			SelectedTWeaponName = ""
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		With attack_target.CurrentForm
			If .CountPilot > 0 Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				HandleEvent("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				HandleEvent("Invalid_string_refer_to_original_code")
			End If
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		SaveSelections()
		SwapSelections()
		
		'Invalid_string_refer_to_original_code
		With defense_target.CurrentForm
			If .CountPilot > 0 Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				HandleEvent("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				HandleEvent("Invalid_string_refer_to_original_code")
			End If
			'End If
		End With
		
		If IsScenarioFinished Then
			RestoreSelections()
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Not defense_target2 Is Nothing Then
			If Not defense_target2.CurrentForm Is defense_target.CurrentForm Then
				With defense_target2.CurrentForm
					If .CountPilot > 0 Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						HandleEvent("Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						HandleEvent("Invalid_string_refer_to_original_code")
					End If
				End With
			End If
			'End With
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		RestoreSelections()
		
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SaveSelections()
		SwapSelections()
		HandleEvent("Invalid_string_refer_to_original_code")
		RestoreSelections()
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .X <> tx Or .Y <> ty Then
				HandleEvent("é€²å…¥", .MainPilot.ID, .X, .Y)
				If IsScenarioFinished Or IsCanceled Then
					ReDim SelectedPartners(0)
					Exit Sub
				End If
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If SelectedUnit.MainPilot.IsSkillAvailable("éŠæ’ƒ") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
			'Invalid_string_refer_to_original_code
			If SelectedUnitMoveCost > 0 Then
				HandleEvent("é€²å…¥", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
				If IsScenarioFinished Then
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
		End If
		
		took_action = True
		
		AreaInSpeed(SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		If LLength((SelectedUnit.Mode)) = 2 Then
			dst_x = CShort(LIndex((SelectedUnit.Mode), 1))
			dst_y = CShort(LIndex((SelectedUnit.Mode), 2))
			If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
				GoTo Move
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		SafetyPoint(SelectedUnit, dst_x, dst_y)
		
		GoTo Move
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code
		GoTo EndOfOperation
		
SearchNearestEnemy: 
		
		'Invalid_string_refer_to_original_code
		searched_nearest_enemy = True
		SelectedTarget = SearchNearestEnemy(SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		If SelectedTarget Is Nothing Then
			GoTo EndOfOperation
		End If
		
		'Invalid_string_refer_to_original_code
		dst_x = SelectedTarget.X
		dst_y = SelectedTarget.Y
		
Move: 
		
		'ç›®æ¨™åœ°ç‚¹
		SelectedX = dst_x
		SelectedY = dst_y
		
		'ç§»å‹•å½¢æ…‹ã¸ã®å¤‰å½¢ãŒå¯èƒ½ã§ã‚ã‚Œã°å¤‰å½¢
		If Not transfered Then
			If TryMoveTransform() Then
				transfered = True
			End If
		End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			tmp = 40
			'End If
			'Invalid_string_refer_to_original_code_
			'And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) _
			'And SelectedUnitMoveCost = 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			mmode = "Invalid_string_refer_to_original_code"
			.EN = .EN - tmp
			AreaInTeleport(SelectedUnit)
			GoTo MoveAreaSelected
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			tmp = 0
			'End If
			'Invalid_string_refer_to_original_code_
			'And .Area <> "ç©ºä¸­" _
			'Invalid_string_refer_to_original_code_
			'And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) _
			'And SelectedUnitMoveCost = 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			mmode = "Invalid_string_refer_to_original_code"
			.EN = .EN - tmp
			AreaInSpeed(SelectedUnit, True)
			GoTo MoveAreaSelected
			'End If
			
			'Invalid_string_refer_to_original_code
			mmode = ""
			AreaInSpeed(SelectedUnit)
			
MoveAreaSelected: 
			
			'Invalid_string_refer_to_original_code
			If guard_unit_mode Then
				With PList.Item(.Mode).Unit_Renamed
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If Not MaskData(i, j) Then
								If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > 1 Then
									MaskData(i, j) = True
								End If
							End If
						Next 
					Next 
				End With
			End If
			
			If .Mode = "Invalid_string_refer_to_original_code" Then
				'Invalid_string_refer_to_original_code
				SafetyPoint(SelectedUnit, dst_x, dst_y)
				new_x = dst_x
				new_y = dst_y
			ElseIf .IsConditionSatisfied("æ··ä¹±") Then 
				'Invalid_string_refer_to_original_code
				dst_x = .X + Dice(.Speed + 1) - Dice(.Speed + 1)
				dst_y = .Y + Dice(.Speed + 1) - Dice(.Speed + 1)
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x, new_y)
			Else
				'Invalid_string_refer_to_original_code
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x0, new_y0)
				
				'Invalid_string_refer_to_original_code
				tmp = System.Math.Abs(dst_x - new_x0) + System.Math.Abs(dst_y - new_y0)
				If tmp <= 5 Then
					If Not MapDataForUnit(dst_x, dst_y) Is Nothing Then
						If Not .IsEnemy(MapDataForUnit(dst_x, dst_y)) Then
							tmp = 1000
						End If
					Else
						tmp = 1000
					End If
				End If
				
				new_x = new_x0
				new_y = new_y0
				new_locations_value = -1
				If tmp <= 5 Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					For i = 0 To 12
						Select Case i
							Case 0
								tx = new_x0
								ty = new_y0
							Case 1
								tx = new_x0 + 1
								ty = new_y0
							Case 2
								tx = new_x0 - 1
								ty = new_y0
							Case 3
								tx = new_x0
								ty = new_y0 + 1
							Case 4
								tx = new_x0
								ty = new_y0 - 1
							Case 5
								tx = new_x0 + 1
								ty = new_y0 + 1
							Case 6
								tx = new_x0 - 1
								ty = new_y0 + 1
							Case 7
								tx = new_x0 + 1
								ty = new_y0 - 1
							Case 8
								tx = new_x0 - 1
								ty = new_y0 - 1
							Case 9
								tx = new_x0 + 2
								ty = new_y0
							Case 10
								tx = new_x0 - 2
								ty = new_y0
							Case 11
								tx = new_x0
								ty = new_y0 + 2
							Case 12
								tx = new_x0
								ty = new_y0 - 2
						End Select
						
						
						If 1 <= tx And tx <= MapWidth And 1 <= ty And ty <= MapHeight Then
							If Not MaskData(tx, ty) And System.Math.Abs(dst_x - tx) + System.Math.Abs(dst_y - ty) < System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								tmp = TerrainEffectForHPRecover(tx, ty) + TerrainEffectForENRecover(tx, ty) + 100 * .LookForSupport(tx, ty)
								
								'Invalid_string_refer_to_original_code
								If .Area <> "ç©ºä¸­" Then
									tmp = tmp + TerrainEffectForHit(tx, ty) + TerrainEffectForDamage(tx, ty)
									'Invalid_string_refer_to_original_code
									If TerrainClass(tx, ty) = "æ°´" Then
										If .IsTransAvailable("æ°´") Then
											tmp = tmp + 100
										End If
									End If
								End If
								
								If tmp > new_locations_value Then
									new_x = tx
									new_y = ty
									new_locations_value = tmp
								End If
							End If
						End If
					Next 
				Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					For i = 0 To 4
						Select Case i
							Case 0
								tx = new_x0
								ty = new_y0
							Case 1
								tx = new_x0 + 1
								ty = new_y0
							Case 2
								tx = new_x0 - 1
								ty = new_y0
							Case 3
								tx = new_x0
								ty = new_y0 + 1
							Case 4
								tx = new_x0
								ty = new_y0 - 1
						End Select
						
						If 1 <= tx And tx <= MapWidth And 1 <= ty And ty <= MapHeight Then
							If Not MaskData(tx, ty) And System.Math.Abs(dst_x - tx) + System.Math.Abs(dst_y - ty) < System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								tmp = .LookForSupport(tx, ty)
								If tmp > new_locations_value Then
									new_x = tx
									new_y = ty
									new_locations_value = tmp
								End If
							End If
						End If
					Next 
				End If
			End If
			
			If new_x < 1 Or MapWidth < new_x Or new_y < 1 Or MapHeight < new_y Then
				'Invalid_string_refer_to_original_code
				GoTo EndOfOperation
			End If
			
			'Invalid_string_refer_to_original_code
			If .X <> new_x Or .Y <> new_y Then
				Select Case mmode
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						OpenMessageForm()
						.PilotMessage("Invalid_string_refer_to_original_code")
						CloseMessageForm()
				End Select
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			PlayWave("Whiz.wav")
			'End If
			.Move(new_x, new_y, True, False, True)
			SelectedUnitMoveCost = 1000
			RedrawScreen()
			'UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'OpenMessageForm()
				'.PilotMessage("Invalid_string_refer_to_original_code")
				'CloseMessageForm()
				'End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'.PlayAnimation("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'.SpecialEffect("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'PlayWave("Swing.wav")
				'End If
				'.Move(new_x, new_y, True, False, True)
				'SelectedUnitMoveCost = 1000
				'RedrawScreen()
				''UPGRADE_WARNING: OperateUnit ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		End With
		
EndOfOperation: 
		
		'Invalid_string_refer_to_original_code
		
		ReDim SelectedPartners(0)
		
		If moved Then
			'Invalid_string_refer_to_original_code
			SelectedUnit.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub TryHyperMode()
		Dim uname As String
		Dim u As Unit
		Dim fname, fdata As String
		Dim flevel As Double
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
			'End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Or .HP > .MaxHP \ 4) _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			uname = LIndex(fdata, 2)
			u = .OtherForm(uname)
			
			'Invalid_string_refer_to_original_code
			If u.IsConditionSatisfied("Invalid_string_refer_to_original_code") Or Not u.IsAbleToEnter(.X, .Y) Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			If u.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				If Not PList.IsDefined(u.FeatureData("Invalid_string_refer_to_original_code")) Then
					PList.Add(u.FeatureData("Invalid_string_refer_to_original_code"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("Invalid_string_refer_to_original_code" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				OpenMessageForm()
				.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
				CloseMessageForm()
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				OpenMessageForm()
				.PilotMessage("Invalid_string_refer_to_original_code")
				CloseMessageForm()
			End If
			
			'ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			If .IsAnimationDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				.PlayAnimation("Invalid_string_refer_to_original_code" & uname & ")")
			ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				.PlayAnimation("Invalid_string_refer_to_original_code" & fname & ")")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.PlayAnimation("Invalid_string_refer_to_original_code")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & uname & ")")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & fname & ")")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.SpecialEffect("Invalid_string_refer_to_original_code")
			End If
			
			'Invalid_string_refer_to_original_code
			.Transform(uname)
		End With
		
		'Invalid_string_refer_to_original_code
		With u.CurrentForm
			HandleEvent("Invalid_string_refer_to_original_code")
		End With
		
		'Invalid_string_refer_to_original_code
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
	End Sub
	
	'æˆ¦é—˜å½¢æ…‹ã¸ã®å¤‰å½¢ãŒå¯èƒ½ã§ã‚ã‚Œã°å¤‰å½¢ã™ã‚‹
	Public Function TryBattleTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim flag As Boolean
		Dim xx, yy As Short
		Dim i, j As Short
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Function
			'End If
			
			'Invalid_string_refer_to_original_code
			If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
				'Invalid_string_refer_to_original_code
				Exit Function
			End If
			
			'æœ€ã‚‚é‹å‹•æ€§ãŒé«˜ã„å½¢æ…‹ã«å¤‰å½¢
			u = SelectedUnit
			xx = .X
			yy = .Y
			For i = 2 To LLength(.FeatureData("å¤‰å½¢"))
				uname = LIndex(.FeatureData("å¤‰å½¢"), i)
				With .OtherForm(uname)
					'Invalid_string_refer_to_original_code
					If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextForm
					'End If
					
					'Invalid_string_refer_to_original_code
					Select Case TerrainClass(xx, yy)
						Case "æ°´", "æ·±æµ·"
							'Invalid_string_refer_to_original_code
							If InStr(.Data.Transportation, "æ°´") > 0 Then
								If InStr(u.Data.Transportation, "æ°´") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "æ°´") > 0 Then
								If InStr(.Data.Transportation, "æ°´") = 0 Then
									GoTo NextForm
								End If
							End If
							
							'Invalid_string_refer_to_original_code
							If InStr(.Data.Transportation, "ç©º") > 0 Then
								If InStr(u.Data.Transportation, "ç©º") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "ç©º") > 0 Then
								If InStr(.Data.Transportation, "ç©º") = 0 Then
									GoTo NextForm
								End If
							End If
					End Select
					
					'Invalid_string_refer_to_original_code
					If .Data.Mobility < u.Data.Mobility Then
						GoTo NextForm
					ElseIf .Data.Mobility = u.Data.Mobility Then 
						'Invalid_string_refer_to_original_code
						If .Data.CountWeapon = 0 Then
							'Invalid_string_refer_to_original_code
							GoTo NextForm
						ElseIf u.Data.CountWeapon > 0 Then 
							If .Data.Weapon(.Data.CountWeapon).Power < u.Data.Weapon(u.Data.CountWeapon).Power Then
								GoTo NextForm
							ElseIf .Data.Weapon(.Data.CountWeapon).Power = u.Data.Weapon(u.Data.CountWeapon).Power Then 
								'Invalid_string_refer_to_original_code
								If .Data.Armor <= u.Data.Armor Then
									GoTo NextForm
								End If
							End If
						End If
					End If
				End With
				u = .OtherForm(uname)
NextForm: 
			Next 
			
			'Invalid_string_refer_to_original_code
			If u Is SelectedUnit Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			uname = u.Name
			
			'Invalid_string_refer_to_original_code
			If u.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				If Not PList.IsDefined(u.FeatureData("Invalid_string_refer_to_original_code")) Then
					If Not PDList.IsDefined(u.FeatureData("Invalid_string_refer_to_original_code")) Then
						ErrorMessage(uname & "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("Invalid_string_refer_to_original_code"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("å¤‰å½¢(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
				CloseMessageForm()
			End If
			
			'ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			If .IsAnimationDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("å¤‰å½¢(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("å¤‰å½¢(" & uname & ")") Then 
				.PlayAnimation("å¤‰å½¢(" & uname & ")")
			ElseIf .IsAnimationDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				.PlayAnimation("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & uname & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
			End If
			
			'å¤‰å½¢
			.Transform(uname)
		End With
		
		'Invalid_string_refer_to_original_code
		With u.CurrentForm
			HandleEvent("å¤‰å½¢", .MainPilot.ID, .Name)
		End With
		
		'Invalid_string_refer_to_original_code
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryBattleTransform = True
	End Function
	
	'ç§»å‹•å½¢æ…‹ã¸ã®å¤‰å½¢ãŒå¯èƒ½ã§ã‚ã‚Œã°å¤‰å½¢ã™ã‚‹
	Private Function TryMoveTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim xx, yy As Short
		Dim tx, ty As Short
		Dim speed1, speed2 As Short
		Dim i As Short
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Function
			'End If
			
			xx = .X
			yy = .Y
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If System.Math.Abs(SelectedX - xx) > System.Math.Abs(SelectedY - yy) Then
				If SelectedX > xx Then
					tx = xx + 1
				Else
					tx = xx - 1
				End If
				ty = yy
			Else
				tx = xx
				If SelectedY > ty Then
					ty = yy + 1
				Else
					ty = yy - 1
				End If
			End If
			
			'æœ€ã‚‚ç§»å‹•åŠ›ãŒé«˜ã„å½¢æ…‹ã«å¤‰å½¢
			u = SelectedUnit
			For i = 2 To LLength(.FeatureData("å¤‰å½¢"))
				uname = LIndex(.FeatureData("å¤‰å½¢"), i)
				With .OtherForm(uname)
					'Invalid_string_refer_to_original_code
					If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'Invalid_string_refer_to_original_code
					If u.IsAbleToEnter(tx, ty) And Not .IsAbleToEnter(tx, ty) Then
						GoTo NextForm
					End If
					
					'Invalid_string_refer_to_original_code
					speed1 = .Data.Speed
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'End If
					'Invalid_string_refer_to_original_code
					Select Case TerrainClass(xx, yy)
						Case "æ°´", "æ·±æµ·"
							If InStr(.Data.Transportation, "æ°´") > 0 Or InStr(.Data.Transportation, "ç©º") > 0 Then
								speed1 = speed1 + 1
							End If
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							'Invalid_string_refer_to_original_code
						Case Else
							If InStr(.Data.Transportation, "ç©º") > 0 Then
								speed1 = speed1 + 1
							End If
					End Select
					
					speed2 = u.Data.Speed
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'End If
					'Invalid_string_refer_to_original_code
					Select Case TerrainClass(xx, yy)
						Case "æ°´", "æ·±æµ·"
							If InStr(u.Data.Transportation, "æ°´") > 0 Or InStr(u.Data.Transportation, "ç©º") > 0 Then
								speed2 = speed2 + 1
							End If
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							'Invalid_string_refer_to_original_code
						Case Else
							If InStr(u.Data.Transportation, "ç©º") > 0 Then
								speed2 = speed2 + 1
							End If
					End Select
					
					If speed2 > speed1 Then
						GoTo NextForm
					ElseIf speed2 = speed1 Then 
						'Invalid_string_refer_to_original_code
						If u.Data.Armor >= .Data.Armor Then
							GoTo NextForm
						End If
					End If
				End With
				u = .OtherForm(uname)
NextForm: 
			Next 
			
			'Invalid_string_refer_to_original_code
			If SelectedUnit Is u Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			uname = u.Name
			
			'Invalid_string_refer_to_original_code
			If u.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				If Not PList.IsDefined(u.FeatureData("Invalid_string_refer_to_original_code")) Then
					If Not PDList.IsDefined(u.FeatureData("Invalid_string_refer_to_original_code")) Then
						ErrorMessage(uname & "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("Invalid_string_refer_to_original_code"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("å¤‰å½¢(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				OpenMessageForm()
				.PilotMessage("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
				CloseMessageForm()
			End If
			
			'ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			If .IsAnimationDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("å¤‰å½¢(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("å¤‰å½¢(" & uname & ")") Then 
				.PlayAnimation("å¤‰å½¢(" & uname & ")")
			ElseIf .IsAnimationDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				.PlayAnimation("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & uname & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")") Then 
				.SpecialEffect("å¤‰å½¢(" & .FeatureName("å¤‰å½¢") & ")")
			End If
			
			'å¤‰å½¢
			.Transform(uname)
		End With
		
		'Invalid_string_refer_to_original_code
		With u.CurrentForm
			HandleEvent("å¤‰å½¢", .MainPilot.ID, .Name)
		End With
		
		'Invalid_string_refer_to_original_code
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryMoveTransform = True
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub TryInstantAbility()
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		'Invalid_string_refer_to_original_code
		If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountAbility
				'Invalid_string_refer_to_original_code
				If Not .IsAbilityUseful(i, "ç§»å‹•å‰") Then
					GoTo NextAbility
				End If
				
				'Invalid_string_refer_to_original_code
				If .AbilityENConsumption(i) > 0 Then
					If .AbilityENConsumption(i) >= .EN \ 2 Then
						GoTo NextAbility
					End If
				End If
				
				With .Ability(i)
					'Invalid_string_refer_to_original_code
					If .MaxRange <> 0 Then
						GoTo NextAbility
					End If
					
					'Invalid_string_refer_to_original_code
					For j = 1 To .CountEffect
						If .EffectType(j) = "å†è¡Œå‹•" Then
							Exit For
						End If
					Next 
					If j > .CountEffect Then
						GoTo NextAbility
					End If
					
					'Invalid_string_refer_to_original_code
					For j = 1 To .CountEffect
						'Invalid_string_refer_to_original_code_
						'Or .EffectType(j) = "ä»˜åŠ " _
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						'Invalid_string_refer_to_original_code
						SelectedAbility = i
						GoTo UseInstantAbility
						'End If
					Next 
				End With
NextAbility: 
			Next 
			
			'Invalid_string_refer_to_original_code
			Exit Sub
			
UseInstantAbility: 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners)
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'Invalid_string_refer_to_original_code
			HandleEvent("ä½¿ç”¨", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		'End If
		
		'è¡Œå‹•æ•°ã‚’æ¶ˆè²»ã—ã¦ãŠã
		SelectedUnit.UseAction()
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function TrySummonning() As Boolean
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountAbility
				If .IsAbilityAvailable(i, "ç§»å‹•å‰") Then
					For j = 1 To .Ability(i).CountEffect
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						SelectedAbility = i
						GoTo UseSummonning
					Next 
				End If
			Next 
			'End If
			'Next
			
			'Invalid_string_refer_to_original_code
			Exit Function
			
UseSummonning: 
			
			TrySummonning = True
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'Invalid_string_refer_to_original_code
			HandleEvent("ä½¿ç”¨", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners)
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			
			'Invalid_string_refer_to_original_code
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TryMapHealing(ByRef moved As Boolean) As Boolean
		Dim a As Short
		Dim apower As Integer
		Dim max_range, min_range As Short
		Dim xx, tx, ty, yy As Short
		Dim y1, x1, x2, y2 As Short
		Dim i, j As Short
		Dim num As Short
		Dim score, max_score As Integer
		Dim p As Pilot
		Dim t As Unit
		Dim partners() As Unit
		
		With SelectedUnit
			SelectedAbility = 0
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("ç‹‚æˆ¦å£«") Then
				Exit Function
			End If
			
			p = .MainPilot
			
			a = .CountAbility()
			Do While a > 0
				'Invalid_string_refer_to_original_code
				If Not .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
					GoTo NextAbility
				End If
				
				'Invalid_string_refer_to_original_code
				If moved Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextAbility
				End If
				If Not .IsAbilityAvailable(a, "ç§»å‹•å‰") Then
					GoTo NextAbility
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "å›å¾©" Then
						'Invalid_string_refer_to_original_code
						If .IsSpellAbility(a) Then
							apower = 5 * .Ability(a).EffectLevel(i) * p.Shooting
						Else
							apower = 500 * .Ability(a).EffectLevel(i)
						End If
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					'Invalid_string_refer_to_original_code
					GoTo NextAbility
				End If
				
				max_range = .AbilityMaxRange(a)
				min_range = .AbilityMinRange(a)
				
				x1 = MaxLng(.X - max_range, 1)
				x2 = MinLng(.X + max_range, MapWidth)
				y1 = MaxLng(.Y - max_range, 1)
				y2 = MinLng(.Y + max_range, MapHeight)
				
				'Invalid_string_refer_to_original_code
				num = 0
				score = 0
				If .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					'                AreaInRange .X, .Y, min_range, max_range, .Party
					AreaInRange(.X, .Y, max_range, min_range, .Party)
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
					If .IsAbilityClassifiedAs(a, "æ´") Then
						MaskData(.X, .Y) = True
					End If
					
					'Invalid_string_refer_to_original_code
					For i = x1 To x2
						For j = y1 To y2
							If MaskData(i, j) Then
								GoTo NextUnit1
							End If
							
							t = MapDataForUnit(i, j)
							If t Is Nothing Then
								GoTo NextUnit1
							End If
							
							'Invalid_string_refer_to_original_code
							If Not .IsAbilityApplicable(a, t) Then
								GoTo NextUnit1
							End If
							
							With t
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								GoTo NextUnit1
							End With
						Next 
					Next 
				End If
				
				If 100 * .HP \ .MaxHP < 90 Then
					num = num + 1
				End If
				score = score + 100 * MinLng(.MaxHP - .HP, apower) \ .MaxHP
			Loop 
		End With
NextUnit1: 
		'Next
		'Next
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim tmp_num, tmp_score As Short
		Dim mlv As Short
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		
		'Invalid_string_refer_to_original_code
		For xx = x1 To x2
			For yy = y1 To y2
				'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				
				'Invalid_string_refer_to_original_code
				tmp_num = 0
				tmp_score = 0
				For i = MaxLng(xx - mlv, 1) To MinLng(xx + mlv, MapWidth)
					For j = MaxLng(yy - mlv, 1) To MinLng(yy + mlv, MapHeight)
						If MaskData(i, j) Then
							GoTo NextUnit2
						End If
						
						t = MapDataForUnit(i, j)
						If t Is Nothing Then
							GoTo NextUnit2
						End If
						
						'Invalid_string_refer_to_original_code
						'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
						
						With t
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							GoTo NextUnit2
							'End If
							
							If 100 * .HP \ .MaxHP < 90 Then
								tmp_num = tmp_num + 1
							End If
							tmp_score = tmp_score + 100 * MinLng(.MaxHP - .HP, apower) \ .MaxHP
						End With
NextUnit2: 
					Next 
				Next 
				If tmp_num > 2 And tmp_score > score Then
					num = tmp_num
					score = tmp_score
					tx = xx
					ty = yy
				End If
NextPoint: 
			Next 
		Next 
		'End If
		
		If num > 1 And score > max_score Then
			SelectedAbility = a
			max_score = score
		End If
		
NextAbility: 
		a = a - 1
		'Loop
		
		If SelectedAbility = 0 Then
			'Invalid_string_refer_to_original_code
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		ReDim SelectedPartners(0)
		ReDim partners(0)
		'End If
		
		'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: TryMapHealing ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		'End With
		
		TryMapHealing = True
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TryHealing(ByRef moved As Boolean, Optional ByRef t As Unit = Nothing) As Boolean
		Dim i, a, j As Short
		Dim aname As String
		Dim apower As Integer
		Dim max_power As Integer
		Dim max_range As Short
		Dim dmg, max_dmg As Integer
		Dim new_x, new_y As Short
		Dim distance As Integer
		Dim is_able_to_move, dont_move, sa_is_able_to_move As Boolean
		Dim u As Unit
		Dim partners() As Unit
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("ç‹‚æˆ¦å£«") Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SelectedTarget ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SelectedTarget = Nothing
			max_dmg = 80
			SelectedAbility = 0
			max_power = 0
			
			'Invalid_string_refer_to_original_code
			dont_move = moved Or .Mode = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			If Not dont_move Then
				AreaInSpeed(SelectedUnit)
			End If
			
			For a = 1 To .CountAbility
				'Invalid_string_refer_to_original_code
				If moved Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextHealingSkill
				End If
				If Not .IsAbilityAvailable(a, "ç§»å‹•å‰") Then
					GoTo NextHealingSkill
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				If .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
					GoTo NextHealingSkill
				End If
				
				'Invalid_string_refer_to_original_code
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "å›å¾©" Then
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					GoTo NextHealingSkill
				End If
				
				'Invalid_string_refer_to_original_code
				If .IsSpellAbility(a) Then
					apower = CInt(5 * .Ability(a).EffectLevel(i) * .MainPilot.Shooting)
				Else
					apower = 500 * .Ability(a).EffectLevel(i)
				End If
				
				'Invalid_string_refer_to_original_code
				If apower <= 0 Then
					GoTo NextHealingSkill
				End If
				
				'Invalid_string_refer_to_original_code
				For	Each u In UList
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextHealingTarget
					'End If
					
					'Invalid_string_refer_to_original_code
					If Not .IsAlly(u) Then
						GoTo NextHealingTarget
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextHealingTarget
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					dmg = 100 * u.HP \ u.MaxHP
					
					'Invalid_string_refer_to_original_code
					If Not u Is SelectedUnit Then
						If u.BossRank >= 0 Then
							dmg = 100 - 2 * (100 - dmg)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If dmg > max_dmg Then
						GoTo NextHealingTarget
					End If
					
					'Invalid_string_refer_to_original_code
					If .AbilityMaxRange(a) = 1 Or .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
						is_able_to_move = True
					Else
						is_able_to_move = False
					End If
					If .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
						is_able_to_move = False
					End If
					If dont_move Then
						is_able_to_move = False
					End If
					Select Case .Area
						Case "ç©ºä¸­", "Invalid_string_refer_to_original_code"
							If .EN - .AbilityENConsumption(a) < 5 Then
								is_able_to_move = False
							End If
					End Select
					
					'Invalid_string_refer_to_original_code
					If is_able_to_move Then
						If Not .IsTargetReachableForAbility(a, u) Then
							GoTo NextHealingTarget
						End If
					Else
						If Not .IsTargetWithinAbilityRange(a, u) Then
							GoTo NextHealingTarget
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If Not .IsAbilityApplicable(a, u) Then
						GoTo NextHealingTarget
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextHealingTarget
					'End If
					
					'Invalid_string_refer_to_original_code
					If Not u Is SelectedTarget Then
						'Invalid_string_refer_to_original_code
						SelectedTarget = u
						max_dmg = dmg
						
						'Invalid_string_refer_to_original_code
						SelectedAbility = 0
						max_power = 0
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If max_power < u.MaxHP - u.HP Then
						'Invalid_string_refer_to_original_code
						If apower < max_power Then
							'Invalid_string_refer_to_original_code
							GoTo NextHealingTarget
						ElseIf apower = max_power Then 
							'Invalid_string_refer_to_original_code
							If .Ability(a).ENConsumption > .Ability(SelectedAbility).ENConsumption Then
								GoTo NextHealingTarget
							End If
							If .Ability(a).Stock < .Ability(SelectedAbility).Stock Then
								GoTo NextHealingTarget
							End If
						End If
					ElseIf SelectedAbility > 0 Then 
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If apower >= u.MaxHP - u.HP Then
							GoTo NextHealingTarget
						End If
						'Invalid_string_refer_to_original_code
						If .Ability(a).ENConsumption > .Ability(SelectedAbility).ENConsumption Then
							GoTo NextHealingTarget
						End If
						If .Ability(a).Stock < .Ability(SelectedAbility).Stock Then
							GoTo NextHealingTarget
						End If
					End If
					
					SelectedAbility = a
					max_power = apower
					sa_is_able_to_move = is_able_to_move
NextHealingTarget: 
				Next u
NextHealingSkill: 
			Next 
			
			'Invalid_string_refer_to_original_code
			If SelectedAbility = 0 Then
				Exit Function
			End If
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			TryHealing = True
			
			'Invalid_string_refer_to_original_code
			If Not SelectedTarget Is SelectedUnit And sa_is_able_to_move Then
				new_x = .X
				new_y = .Y
				max_range = .AbilityMaxRange(SelectedAbility)
				With SelectedTarget
					'Invalid_string_refer_to_original_code
					If System.Math.Abs(.X - new_x) + System.Math.Abs(.Y - new_y) <= max_range Then
						distance = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
					Else
						distance = 10000
					End If
					
					'Invalid_string_refer_to_original_code
					For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
						For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
							If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) <= max_range Then
								With SelectedUnit
									If System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2 < distance Then
										new_x = i
										new_y = j
										distance = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
									End If
								End With
							End If
						Next 
					Next 
				End With
				
				If new_x <> .X Or new_y <> .Y Then
					'Invalid_string_refer_to_original_code
					.Move(new_x, new_y)
					moved = True
				End If
			End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners)
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			
			'Invalid_string_refer_to_original_code
			HandleEvent("ä½¿ç”¨", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
			
			If SelectedTarget Is SelectedUnit Then
				OpenMessageForm(SelectedUnit)
			Else
				OpenMessageForm(SelectedTarget, SelectedUnit)
			End If
			
			'Invalid_string_refer_to_original_code
			.ExecuteAbility(SelectedAbility, SelectedTarget)
			SelectedUnit = .CurrentForm
		End With
		
		CloseMessageForm()
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If SelectedUnit.CountPilot > 0 Then
			HandleEvent("Invalid_string_refer_to_original_code")
		End If
		ReDim SelectedPartners(0)
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.CountPilot > 0 Then
			HandleEvent("Invalid_string_refer_to_original_code")
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TryFix(ByRef moved As Boolean, Optional ByRef t As Unit = Nothing) As Boolean
		Dim TmpMaskData() As Boolean
		Dim j, i, k As Short
		Dim new_x, new_y As Short
		Dim max_dmg As Integer
		Dim tmp As Integer
		Dim u As Unit
		Dim fname As String
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If Not .IsFeatureAvailable("Invalid_string_refer_to_original_code") Or .Area = "åœ°ä¸­" Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("ç‹‚æˆ¦å£«") Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			If .X > 1 Then
				MaskData(.X - 1, .Y) = False
			End If
			If .X < MapWidth Then
				MaskData(.X + 1, .Y) = False
			End If
			If .Y > 1 Then
				MaskData(.X, .Y - 1) = False
			End If
			If .Y < MapHeight Then
				MaskData(.X, .Y + 1) = False
			End If
			'Invalid_string_refer_to_original_code
			ReDim TmpMaskData(MapWidth + 1, MapHeight + 1)
			AreaInSpeed(SelectedUnit)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					TmpMaskData(i, j) = MaskData(i, j)
				Next 
			Next 
			For i = 0 To MapWidth
				TmpMaskData(i, 0) = True
				TmpMaskData(i, MapHeight + 1) = True
			Next 
			For i = 0 To MapHeight
				TmpMaskData(0, i) = True
				TmpMaskData(MapWidth + 1, i) = True
			Next 
			For i = MaxLng(.X - (.Speed + 1), 1) To MinLng(.X + (.Speed + 1), MapWidth)
				For j = MaxLng(.Y - (.Speed + 1), 1) To MinLng(.Y + (.Speed + 1), MapHeight)
					MaskData(i, j) = TmpMaskData(i, j) And TmpMaskData(i - 1, j) And TmpMaskData(i + 1, j) And TmpMaskData(i, j - 1) And TmpMaskData(i, j + 1)
				Next 
			Next 
			MaskData(.X, .Y) = True
			'End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg SelectedTarget ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			SelectedTarget = Nothing
			max_dmg = 90
			For i = MaxLng(.X - (.Speed + 1), 1) To MinLng(.X + (.Speed + 1), MapWidth)
				For j = MaxLng(.Y - (.Speed + 1), 1) To MinLng(.Y + (.Speed + 1), MapHeight)
					If MaskData(i, j) Then
						GoTo NextFixTarget
					End If
					
					u = MapDataForUnit(i, j)
					If u Is Nothing Then
						GoTo NextFixTarget
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextFixTarget
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If 100 * u.HP \ u.MaxHP > max_dmg Then
						GoTo NextFixTarget
					End If
					
					'Invalid_string_refer_to_original_code
					If Not .IsAlly(u) Then
						GoTo NextFixTarget
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextFixTarget
					'End If
					
					'Invalid_string_refer_to_original_code
					If u.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						For k = 2 To CInt(u.FeatureData("Invalid_string_refer_to_original_code"))
							fname = LIndex(u.FeatureData("Invalid_string_refer_to_original_code"), k)
							If Left(fname, 1) = "!" Then
								fname = Mid(fname, 2)
								If fname <> .FeatureName0("Invalid_string_refer_to_original_code") Then
									GoTo NextFixTarget
								End If
							Else
								If fname = .FeatureName0("Invalid_string_refer_to_original_code") Then
									GoTo NextFixTarget
								End If
							End If
						Next 
					End If
					
					SelectedTarget = u
					max_dmg = 100 * u.HP \ u.MaxHP
NextFixTarget: 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			new_x = .X
			new_y = .Y
			With SelectedTarget
				'Invalid_string_refer_to_original_code
				If System.Math.Abs(.X - new_x) + System.Math.Abs(.Y - new_y) = 1 Then
					tmp = 1
				Else
					tmp = 10000
				End If
				
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = TmpMaskData(i, j)
					Next 
				Next 
				
				'Invalid_string_refer_to_original_code
				For i = MaxLng(.X - 1, 1) To MinLng(.X + 1, MapWidth)
					For j = MaxLng(.Y - 1, 1) To MinLng(.Y + 1, MapHeight)
						If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) = 1 Then
							With SelectedUnit
								If System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2 < tmp Then
									new_x = i
									new_y = j
									tmp = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
								End If
							End With
						End If
					Next 
				Next 
			End With
			
			If new_x <> .X Or new_y <> .Y Then
				'Invalid_string_refer_to_original_code
				.Move(new_x, new_y)
				moved = True
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'Invalid_string_refer_to_original_code
			OpenMessageForm(SelectedTarget, SelectedUnit)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.PilotMessage("Invalid_string_refer_to_original_code")
			'End If
			
			'ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'End If
			
			DisplaySysMessage(.Nickname & "ã¯[" & SelectedTarget.Nickname & "]ã«[" & .FeatureName0("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			tmp = SelectedTarget.HP
			Select Case .FeatureLevel("Invalid_string_refer_to_original_code")
				Case 1, -1
					SelectedTarget.RecoverHP(30 + 3 * .MainPilot.SkillLevel("Invalid_string_refer_to_original_code"))
				Case 2
					SelectedTarget.RecoverHP(50 + 5 * .MainPilot.SkillLevel("Invalid_string_refer_to_original_code"))
				Case 3
					SelectedTarget.RecoverHP(100)
			End Select
			DrawSysString(SelectedTarget.X, SelectedTarget.Y, "+" & VB6.Format(SelectedTarget.HP - tmp))
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "Invalid_string_refer_to_original_code" & VB6.Format(SelectedTarget.HP - tmp) & "Invalid_string_refer_to_original_code")
		End With
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
		
		If MessageWait < 10000 Then
			Sleep(MessageWait)
		End If
		
		CloseMessageForm()
		
		'Invalid_string_refer_to_original_code
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		TryFix = True
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TryMapAttack(ByRef moved As Boolean) As Boolean
		Dim w As Short
		Dim xx, tx, ty, yy As Short
		Dim y1, x1, x2, y2 As Short
		Dim i, j As Short
		Dim enemy_num As Short
		Dim score, score_limit As Short
		Dim t As Unit
		Dim direction As String
		Dim min_range, max_range, lv As Short
		Dim partners() As Unit
		
		With SelectedUnit
			SaveSelections()
			
			'Invalid_string_refer_to_original_code
			score_limit = 1
			For i = 1 To .CountWeapon
				'Invalid_string_refer_to_original_code
				If Not .IsWeaponClassifiedAs(i, "Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					'                score_limit = 2
					'                Exit For
					If .IsWeaponAvailable(i, "ç§»å‹•å‰") Then
						score_limit = 2
						Exit For
					End If
					'Invalid_string_refer_to_original_code
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			w = .CountWeapon
			Do While w > 0
				SelectedWeapon = w
				SelectedTWeapon = 0
				
				'Invalid_string_refer_to_original_code
				If Not .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then
					GoTo NextWeapon
				End If
				
				'Invalid_string_refer_to_original_code
				If moved Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextWeapon
				End If
				If Not .IsWeaponAvailable(w, "ç§»å‹•å‰") Then
					GoTo NextWeapon
				End If
				'End If
				
				'Invalid_string_refer_to_original_code
				If .BossRank >= 0 Then
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If .HP > .MaxHP \ 4 Then
						GoTo NextWeapon
					End If
				End If
				'End If
				
				max_range = .WeaponMaxRange(w)
				min_range = .Weapon(w).MinRange
				
				x1 = MaxLng(.X - max_range, 1)
				y1 = MaxLng(.Y - max_range, 1)
				x2 = MinLng(.X + max_range, MapWidth)
				y2 = MinLng(.Y + max_range, MapHeight)
				
				'Invalid_string_refer_to_original_code
				If .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'Invalid_string_refer_to_original_code
						AreaInLine(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'Invalid_string_refer_to_original_code
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'Invalid_string_refer_to_original_code
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = MaxLng(.Y - max_range, 1)
								Case "S"
									tx = .X
									ty = MinLng(.Y + max_range, MapHeight)
								Case "W"
									tx = MaxLng(.X - max_range, 1)
									ty = .Y
								Case "E"
									tx = MinLng(.X + max_range, MapWidth)
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
				ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'Invalid_string_refer_to_original_code
						AreaInCone(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'Invalid_string_refer_to_original_code
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'Invalid_string_refer_to_original_code
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = .Y - 1
								Case "S"
									tx = .X
									ty = .Y + 1
								Case "W"
									tx = .X - 1
									ty = .Y
								Case "E"
									tx = .X + 1
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MaskData(.X, .Y) = True
						
						'Invalid_string_refer_to_original_code
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'Invalid_string_refer_to_original_code
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = .Y - 1
								Case "S"
									tx = .X
									ty = .Y + 1
								Case "W"
									tx = .X - 1
									ty = .Y
								Case "E"
									tx = .X + 1
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
				ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'                AreaInRange .X, .Y, min_range, max_range, "ã™ã¹ã¦"
					AreaInRange(.X, .Y, max_range, min_range, "ã™ã¹ã¦")
					'Invalid_string_refer_to_original_code
					MaskData(.X, .Y) = True
					
					'Invalid_string_refer_to_original_code
					enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
					
					'Invalid_string_refer_to_original_code
					If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
						tx = .X
						ty = .Y
						GoTo FoundWeapon
					End If
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'Invalid_string_refer_to_original_code
								If lv > 0 Then
									'Invalid_string_refer_to_original_code
									'                                AreaInRange xx, yy, 1, lv, "ã™ã¹ã¦"
									AreaInRange(xx, yy, lv, 1, "ã™ã¹ã¦")
									'Invalid_string_refer_to_original_code
								Else
									For i = 1 To MapWidth
										For j = 1 To MapHeight
											MaskData(i, j) = True
										Next 
									Next 
									MaskData(xx, yy) = False
								End If
								MaskData(.X, .Y) = True
								
								'Invalid_string_refer_to_original_code
								enemy_num = CountTargetInRange(w, xx - lv, yy - lv, xx + lv, yy + lv)
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And lv = 0) Then
						GoTo FoundWeapon
					End If
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'Invalid_string_refer_to_original_code
								AreaInPointToPoint(.X, .Y, xx, yy)
								MaskData(.X, .Y) = True
								
								'Invalid_string_refer_to_original_code
								enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'Invalid_string_refer_to_original_code
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Then
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextWeapon
				End If
				
				score = 0
				For xx = x1 To x2
					For yy = y1 To y2
						If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range And MapDataForUnit(xx, yy) Is Nothing And .IsAbleToEnter(xx, yy) Then
							'Invalid_string_refer_to_original_code
							AreaInPointToPoint(.X, .Y, xx, yy)
							MaskData(.X, .Y) = True
							
							'Invalid_string_refer_to_original_code
							enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
							
							If enemy_num > score Then
								'Invalid_string_refer_to_original_code
								AreaInMoveAction(SelectedUnit, max_range)
								If Not MaskData(xx, yy) Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						End If
					Next 
				Next 
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And max_range = 2) Then
					GoTo FoundWeapon
				End If
				'End If
NextWeapon: 
				w = w - 1
			Loop 
			
			'Invalid_string_refer_to_original_code
			
			RestoreSelections()
			TryMapAttack = False
			
			Exit Function
			
FoundWeapon: 
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.CombinationPartner("Invalid_string_refer_to_original_code")
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			
			'Invalid_string_refer_to_original_code
			.MapAttack(w, tx, ty)
			
			'Invalid_string_refer_to_original_code
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.UseAction()
				Next 
			End If
			ReDim SelectedPartners(0)
			
			RestoreSelections()
			TryMapAttack = True
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function CountTargetInRange(ByVal w As Short, ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short) As Short
		Dim i, j As Short
		Dim t As Unit
		Dim is_ally_involved As Boolean
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			For i = MaxLng(x1, 1) To MinLng(x2, MapWidth)
				For j = MaxLng(y1, 1) To MinLng(y2, MapHeight)
					'Invalid_string_refer_to_original_code
					If MaskData(i, j) Then
						GoTo NextPoint
					End If
					
					t = MapDataForUnit(i, j)
					
					'Invalid_string_refer_to_original_code
					If t Is Nothing Then
						GoTo NextPoint
					End If
					
					'Invalid_string_refer_to_original_code
					If .HitProbability(w, t, False) = 0 Then
						GoTo NextPoint
					ElseIf .ExpDamage(w, t, False) <= 10 Then 
						If .IsNormalWeapon(w) Then
							GoTo NextPoint
						ElseIf .CriticalProbability(w, t) <= 1 And .WeaponLevel(w, "Invalid_string_refer_to_original_code") = 0 And .WeaponLevel(w, "å¹") = 0 Then 
							GoTo NextPoint
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If .IsAlly(t) Then
						'Invalid_string_refer_to_original_code
						is_ally_involved = True
						GoTo NextPoint
					End If
					
					'Invalid_string_refer_to_original_code
					Select Case .Mode
						Case "å‘³æ–¹", "Invalid_string_refer_to_original_code"
							If t.Party <> "å‘³æ–¹" And t.Party <> "Invalid_string_refer_to_original_code" Then
								GoTo NextPoint
							End If
						Case "æ•µ"
							If t.Party <> "æ•µ" Then
								GoTo NextPoint
							End If
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							GoTo NextPoint
							'End If
					End Select
					
					'Invalid_string_refer_to_original_code
					If t.IsUnderSpecialPowerEffect("éš ã‚Œèº«") Then
						GoTo NextPoint
					End If
					If t.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If t.IsFeatureLevelSpecified("Invalid_string_refer_to_original_code") Then
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("Invalid_string_refer_to_original_code") Then
								GoTo NextPoint
							End If
						Else
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
								GoTo NextPoint
							End If
						End If
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					CountTargetInRange = CountTargetInRange + 1
NextPoint: 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CountTargetInRange = 0
			'End If
		End With
	End Function
	
	'ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼ã‚’ä½¿ç”¨ã™ã‚‹
	Public Sub TrySpecialPower(ByRef p As Pilot)
		Dim slist As String
		Dim sd As SpecialPowerData
		Dim i, tnum As Short
		
		SelectedPilot = p
		
		'Invalid_string_refer_to_original_code
		If InStr(p.Name, "(ã‚¶ã‚³)") > 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Dice(100) > p.TacticalTechnique0 - 100 Then
			Exit Sub
		End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or .IsConditionSatisfied("ç‹‚æˆ¦å£«") _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				Exit Sub
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		slist = ""
		For i = 1 To p.CountSpecialPower
			SelectedSpecialPower = p.SpecialPower(i)
			sd = SPDList.Item(SelectedSpecialPower)
			
			'Invalid_string_refer_to_original_code
			If p.SP < p.SpecialPowerCost(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			'Invalid_string_refer_to_original_code
			If SelectedUnit.IsSpecialPowerInEffect(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			sd = SPDList.Item(SelectedSpecialPower)
			
			With sd
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				Select Case .TargetType
					Case "å‘³æ–¹", "æ•µ", "Invalid_string_refer_to_original_code"
						GoTo NextSpecialPower
				End Select
				
				'Invalid_string_refer_to_original_code
				tnum = .CountTarget(p)
				If tnum = 0 Then
					GoTo NextSpecialPower
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				Select Case .TargetType
					Case "å…¨å‘³æ–¹", "å…¨æ•µ"
						If tnum < 3 Then
							GoTo NextSpecialPower
						End If
				End Select
				
				'Invalid_string_refer_to_original_code
				
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(Invalid_string_refer_to_original_code) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsEffectAvailable("Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If SelectedUnit.HP < 0.7 * SelectedUnit.MaxHP Then
						GoTo AddSpecialPower
					End If
				ElseIf .TargetType = "å…¨å‘³æ–¹" Then 
					If Turn >= 3 Then
						GoTo AddSpecialPower
					End If
				End If
				'End If
				
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(Invalid_string_refer_to_original_code) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsEffectAvailable("Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If SelectedUnit.EN < 0.3 * SelectedUnit.MaxEN Then
						GoTo AddSpecialPower
					End If
				ElseIf .TargetType = "å…¨å‘³æ–¹" Then 
					If Turn >= 4 Then
						GoTo AddSpecialPower
					End If
				End If
				'End If
				
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(æ°—åŠ›å¢—åŠ ) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsEffectAvailable("æ°—åŠ›å¢—åŠ ") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If p.Morale < p.MaxMorale Then
						If p.CountSpecialPower = 1 Or p.SP > p.MaxSP \ 2 Then
							GoTo AddSpecialPower
						End If
					End If
				ElseIf .TargetType = "å…¨å‘³æ–¹" Then 
					GoTo AddSpecialPower
				End If
				'End If
				
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(è¡Œå‹•æ•°å¢—åŠ ) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsEffectAvailable("è¡Œå‹•æ•°å¢—åŠ ") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If DistanceFromNearestEnemy(SelectedUnit) <= 5 Then
						GoTo AddSpecialPower
					End If
				End If
				'End If
				
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(å¾©æ´») ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsEffectAvailable("å¾©æ´»") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo AddSpecialPower
				End If
				'End If
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Or IsSPEffectUseful(sd, "éš ã‚Œèº«") _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo AddSpecialPower
				'End If
				'UPGRADE_WARNING: TrySpecialPower ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo AddSpecialPower
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If DistanceFromNearestEnemy(SelectedUnit) <= 5 Or .Duration = "é˜²å¾¡" Then
					GoTo AddSpecialPower
				End If
				'UPGRADE_WARNING: TrySpecialPower ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo AddSpecialPower
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
					GoTo AddSpecialPower
				End If
				'UPGRADE_WARNING: TrySpecialPower ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo AddSpecialPower
				'End If
				'End If
				
				If IsSPEffectUseful(sd, "Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Select Case DistanceFromNearestEnemy(SelectedUnit)
						Case 5, 6
							GoTo AddSpecialPower
					End Select
				ElseIf .TargetType = "å…¨å‘³æ–¹" Then 
					GoTo AddSpecialPower
				End If
				'End If
				
				'Invalid_string_refer_to_original_code_
				'Or .IsEffectAvailable("ãƒ©ãƒ³ãƒ€ãƒ ãƒ€ãƒ¡ãƒ¼ã‚¸") _
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Or .IsEffectAvailable("æŒ‘ç™º") _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .TargetType = "å…¨æ•µ" Then
					GoTo AddSpecialPower
				End If
				'End If
				
				'Invalid_string_refer_to_original_code_
				'Or .IsEffectAvailable("è¢«ãƒ€ãƒ¡ãƒ¼ã‚¸å¢—åŠ ") _
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .TargetType = "å…¨æ•µ" Then
					If Turn >= 3 Then
						GoTo AddSpecialPower
					End If
				End If
				'End If
			End With
			
			'Invalid_string_refer_to_original_code
			GoTo NextSpecialPower
			
AddSpecialPower: 
			
			'ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼ã‚’å€™è£œãƒªã‚¹ãƒˆã«è¿½åŠ 
			slist = slist & " " & SelectedSpecialPower
			
NextSpecialPower: 
		Next 
		
		'Invalid_string_refer_to_original_code
		If slist = "" Then
			SelectedSpecialPower = ""
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		SelectedSpecialPower = LIndex(slist, Dice(LLength(slist)))
		
		'Invalid_string_refer_to_original_code
		HandleEvent("ä½¿ç”¨", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		p.UseSpecialPower(SelectedSpecialPower)
		SelectedUnit = SelectedUnit.CurrentForm
		
		'Invalid_string_refer_to_original_code
		If Not IsRButtonPressed Then
			DisplayUnitStatus(SelectedUnit)
		End If
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		
		SelectedSpecialPower = ""
	End Sub
	
	Private Function IsSPEffectUseful(ByRef sd As SpecialPowerData, ByRef ename As String) As Boolean
		With sd
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg sd.IsEffectAvailable(ename) ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .IsEffectAvailable(ename) Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If Not SelectedUnit.IsSpecialPowerInEffect(ename) Then
					IsSPEffectUseful = True
				End If
			Else
				IsSPEffectUseful = True
			End If
			'End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function SelectWeapon(ByRef u As Unit, ByRef t As Unit, Optional ByRef amode As String = "", Optional ByRef max_prob As Integer = 0, Optional ByRef max_dmg As Integer = 0) As Short
		Dim smode As String
		Dim use_true_value, is_move_attack As Boolean
		Dim prob, destroy_prob As Integer
		Dim ct_prob As Short
		Dim sp_prob As Double
		Dim dmg, exp_dmg As Integer
		Dim dmg_mod As Double
		Dim su As Unit
		Dim support_dmg, support_prob, support_exp_dmg As Integer
		Dim w As Short
		Dim wclass, wattr As String
		Dim max_destroy_prob, max_exp_dmg As Integer
		Dim i, j As Short
		Dim checku As Unit
		Dim checkwc As String
		
		With u
			'å¾¡ä¸»äººã•ã¾ã«ã¯ã•ã‹ã‚‰ãˆã¾ã›ã‚“
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .Master Is t Then
				SelectWeapon = -1
				Exit Function
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("è¸Šã‚Š") Then
				SelectWeapon = -1
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If .Party = "å‘³æ–¹" Then
				use_true_value = True
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			smode = "Invalid_string_refer_to_original_code"
			smode = "ç§»å‹•å‰"
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			su = .LookForSupportAttack(t)
			If Not su Is Nothing Then
				w = SelectWeapon(su, t, "Invalid_string_refer_to_original_code", support_prob, support_exp_dmg)
				If w > 0 Then
					With su
						support_prob = MinLng(.HitProbability(w, t, use_true_value), 100)
						
						dmg_mod = 1
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						dmg_mod = 0.7
					End With
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'And .IsNormalWeapon(w) _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				dmg_mod = 1.2 * dmg_mod
			Else
				dmg_mod = 1.5 * dmg_mod
			End If
			'End If
			
			support_dmg = .ExpDamage(w, t, use_true_value, dmg_mod)
		End With
		'End If
		'End If
		'End If
		
		SelectWeapon = 0
		max_destroy_prob = 0
		max_exp_dmg = -1
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End If
		'åŠ¹å±æ€§
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		checkwc = Mid(wattr, 2)
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End If
		'End If
		'End If
		'End With
		'Next
		'End If
		'Next
		'End If
		'å‰‹å±æ€§
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		checkwc = Mid(wattr, 2)
		Select Case checkwc
			Case "ã‚ª"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextAttribute
				'End If
			Case "ã‚·"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextAttribute
				'End If
			Case "ã‚µ"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextAttribute
				'End If
			Case "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextAttribute
				'End If
			Case "æŠ€"
				'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		End Select
		
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End If
		'End Select
NextAttribute: 
		'Next
		'End With
		If sp_prob > 1 Then
			sp_prob = System.Math.Sqrt(sp_prob)
		End If
		sp_prob = sp_prob * ct_prob
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sp_prob = sp_prob + 50 * dmg \ t.MaxHP
		'End If
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		
		'Invalid_string_refer_to_original_code
		If InStr(amode, "åæ’ƒ") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or .UsedCounterAttack < .MaxCounterAttack _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			sp_prob = 1.5 * sp_prob
		End If
		'End If
		If sp_prob > 100 Then
			sp_prob = 100
		End If
		
		'Invalid_string_refer_to_original_code
		If dmg = 0 And ct_prob < 30 Then
			sp_prob = sp_prob / 5
		End If
		
		'Invalid_string_refer_to_original_code
		If dmg = 0 And sp_prob = 0 Then
			GoTo NextWeapon
		End If
		
		If prob > 0 Then
			If sp_prob > 0 Then
				'Invalid_string_refer_to_original_code
				exp_dmg = dmg + MaxLng(t.HP - dmg, 0) * sp_prob \ 100
			End If
		End If
		'End If
		'End If
		exp_dmg = exp_dmg * 0.01 * MinLng(prob, 100)
		'Invalid_string_refer_to_original_code
		prob = 1
		exp_dmg = (dmg \ 10 + MaxLng(t.HP - dmg \ 10, 0) * sp_prob \ 100) \ 10
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not is_move_attack Then
			exp_dmg = exp_dmg + support_exp_dmg
		End If
		
		'Invalid_string_refer_to_original_code
		destroy_prob = 0
		With t
			If .Party = "å‘³æ–¹" And Not .IsFeatureAvailable("é˜²å¾¡ä¸å¯") Then
				If dmg >= 2 * .HP Then
					destroy_prob = MinLng(prob, 100)
				End If
				'Invalid_string_refer_to_original_code
				If Not is_move_attack Then
					If support_dmg >= 2 * .HP Then
						destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob \ 100
					ElseIf dmg + support_dmg >= 2 * .HP Then 
						destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob \ 10000
					End If
				End If
			Else
				If dmg >= .HP Then
					destroy_prob = MinLng(prob, 100)
				End If
				'Invalid_string_refer_to_original_code
				If Not is_move_attack Then
					If support_dmg >= .HP Then
						destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob \ 100
					ElseIf dmg + support_dmg >= .HP Then 
						destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob \ 10000
					End If
				End If
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If InStr(amode, "åæ’ƒ") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or .UsedCounterAttack < .MaxCounterAttack _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			destroy_prob = 1.5 * destroy_prob
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End If
		
		If destroy_prob >= 100 Then
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		ElseIf destroy_prob > 50 Then 
			'Invalid_string_refer_to_original_code
			If destroy_prob > max_destroy_prob Then
				SelectWeapon = w
				max_destroy_prob = destroy_prob
				max_exp_dmg = exp_dmg
			ElseIf destroy_prob = max_destroy_prob Then 
				If exp_dmg > max_exp_dmg Then
					SelectWeapon = w
					max_destroy_prob = destroy_prob
					max_exp_dmg = exp_dmg
				End If
			End If
		Else
			'Invalid_string_refer_to_original_code
			If max_destroy_prob <= 50 Then
				If exp_dmg > max_exp_dmg Then
					SelectWeapon = w
					max_destroy_prob = destroy_prob
					max_exp_dmg = exp_dmg
				End If
			End If
		End If
NextWeapon: 
		'Next
		
		'Invalid_string_refer_to_original_code
		If SelectWeapon > 0 Then
			'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		End If
		
		'Invalid_string_refer_to_original_code
		If max_destroy_prob > 50 Then
			max_prob = max_destroy_prob
		Else
			max_prob = 0
		End If
		'UPGRADE_WARNING: SelectWeapon ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SelectDefense(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByRef tw As Short) As Object
		Dim prob, dmg As Integer
		Dim tprob, tdmg As Integer
		Dim is_target_inferior As Boolean
		
		'Invalid_string_refer_to_original_code
		If u.IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then
			Exit Function
		End If
		
		With t
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("è¸Šã‚Š") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "å›é¿"
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("ç‹‚æˆ¦å£«") Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Function
			'End If
			
			If .Party <> "å‘³æ–¹" Then
				'Invalid_string_refer_to_original_code
				'é˜²å¾¡è¡Œå‹•ã‚’è¡Œã†
				If Not IsOptionDefined("æ•µãƒ¦ãƒ‹ãƒƒãƒˆé˜²å¾¡ä½¿ç”¨") Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				With .MainPilot
					If InStr(.Name, "(ã‚¶ã‚³)") > 0 Or .TacticalTechnique < 160 Then
						Exit Function
					End If
				End With
			End If
			
			'Invalid_string_refer_to_original_code
			If .MaxAction = 0 Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "é˜²å¾¡"
			End If
			Exit Function
			'End If
			
			'Invalid_string_refer_to_original_code
			dmg = u.ExpDamage(w, t, True)
			prob = MinLng(u.HitProbability(w, t, True), 100)
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			prob = 0
			'End If
			
			'Invalid_string_refer_to_original_code
			If Not .LookForSupportGuard(u, w) Is Nothing Then
				prob = 0
			End If
			
			'Invalid_string_refer_to_original_code
			If tw > 0 Then
				tdmg = .ExpDamage(tw, u, True)
				tprob = MinLng(.HitProbability(tw, u, True), 100)
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				prob = 0
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .Party = "å‘³æ–¹" Then
				'Invalid_string_refer_to_original_code
				If dmg * prob > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'Invalid_string_refer_to_original_code
				If u.IsUnderSpecialPowerEffect("ãƒ€ãƒ¡ãƒ¼ã‚¸å¢—åŠ ") Then
					If 2 * dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			Else
				'Invalid_string_refer_to_original_code
				If dmg * prob \ 2 > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'Invalid_string_refer_to_original_code
				If u.IsUnderSpecialPowerEffect("ãƒ€ãƒ¡ãƒ¼ã‚¸å¢—åŠ ") Then
					If dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If dmg >= .HP And prob > 10 Then
				is_target_inferior = True
			End If
			
			If tw > 0 Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Or .MaxCounterAttack > .UsedCounterAttack _
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If tdmg >= u.HP And tprob > 70 Then
					'Invalid_string_refer_to_original_code
					is_target_inferior = False
				End If
			End If
			'End If
			'Invalid_string_refer_to_original_code
			is_target_inferior = True
			'End If
			
			If Not is_target_inferior Then
				'Invalid_string_refer_to_original_code
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If dmg > .HP And dmg \ 2 < .HP And Not .IsFeatureAvailable("é˜²å¾¡ä¸å¯") And Not u.IsWeaponClassifiedAs(w, "æ®º") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "é˜²å¾¡"
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If prob < 50 And Not .IsFeatureAvailable("å›é¿ä¸å¯") And Not .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "å›é¿"
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If dmg \ 2 < .HP And Not .IsFeatureAvailable("é˜²å¾¡ä¸å¯") And Not u.IsWeaponClassifiedAs(w, "æ®º") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "é˜²å¾¡"
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If Not .IsFeatureAvailable("å›é¿ä¸å¯") And Not .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "å›é¿"
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If Not .IsFeatureAvailable("é˜²å¾¡ä¸å¯") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg SelectDefense ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				SelectDefense = "é˜²å¾¡"
			End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsAbleToCounterAttack(ByRef u As Unit, ByRef t As Unit) As Boolean
		Dim i, w, idx As Short
		Dim buf, wclass, ch As String
		
		With u
			For w = 1 To .CountWeapon
				'Invalid_string_refer_to_original_code
				If Not .IsWeaponAvailable(w, "ç§»å‹•å‰") Then
					GoTo NextWeapon
				End If
				
				'Invalid_string_refer_to_original_code
				If .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then
					GoTo NextWeapon
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				GoTo NextWeapon
				'End If
				
				'Invalid_string_refer_to_original_code
				If Not .IsTargetWithinRange(w, t) Then
					GoTo NextWeapon
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If .Damage(w, t, True) > 0 Then
					IsAbleToCounterAttack = True
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If Not .IsNormalWeapon(w) Then
					If .CriticalProbability(w, t) > 0 Then
						IsAbleToCounterAttack = True
						Exit Function
					End If
				End If
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'            If .WeaponAdaption(w, t.Area) = 0 Then
				'                GoTo NextWeapon
				'            End If
				'
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'Invalid_string_refer_to_original_code
				'                        If InStrNotNest(wclass, ch) > 0 Then
				'                            Exit For
				'                        End If
				'                    End If
				'                Next
				'                If i > Len(buf) Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'            If idx > 0 Then
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'Invalid_string_refer_to_original_code
				'                        If InStrNotNest(wclass, ch) > idx Then
				'                            Exit For
				'                        End If
				'                    End If
				'                Next
				'                If i > Len(buf) Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'Invalid_string_refer_to_original_code
				'            If .IsWeaponClassifiedAs(w, "å¯¾") Then
				'                If t.MainPilot.Level Mod .WeaponLevel(w, "å¯¾") <> 0 Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'Invalid_string_refer_to_original_code
				'            IsAbleToCounterAttack = True
				'            Exit Function
				'Invalid_string_refer_to_original_code
NextWeapon: 
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		IsAbleToCounterAttack = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SearchNearestEnemy(ByRef u As Unit) As Unit
		Dim distance As Short
		Dim i, j As Short
		Dim t As Unit
		
		distance = 1000
		
		With u
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					t = MapDataForUnit(i, j)
					
					If t Is Nothing Then
						GoTo NexLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If distance <= System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) Then
						GoTo NexLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If .IsAlly(t) Then
						GoTo NexLoop
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If t.Party <> .Mode Then
						GoTo NexLoop
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					If t.IsUnderSpecialPowerEffect("éš ã‚Œèº«") Or t.Area = "åœ°ä¸­" Then
						GoTo NexLoop
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If t.IsFeatureLevelSpecified("Invalid_string_refer_to_original_code") Then
						If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("Invalid_string_refer_to_original_code") Then
							GoTo NexLoop
						End If
					Else
						If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
							GoTo NexLoop
						End If
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					SearchNearestEnemy = t
					distance = System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y)
					
NexLoop: 
				Next 
			Next 
		End With
	End Function
	
	'æœ€ã‚‚è¿‘ã„æ•µãƒ¦ãƒ‹ãƒƒãƒˆã¸ã®è·é›¢ã‚’è¿”ã™
	Private Function DistanceFromNearestEnemy(ByRef u As Unit) As Short
		Dim t As Unit
		
		t = SearchNearestEnemy(u)
		
		If Not t Is Nothing Then
			DistanceFromNearestEnemy = System.Math.Abs(u.X - t.X) + System.Math.Abs(u.Y - t.Y)
		Else
			DistanceFromNearestEnemy = 1000
		End If
	End Function
End Module