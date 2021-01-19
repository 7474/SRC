Option Strict Off
Option Explicit On
Module Help
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Public Sub SkillHelp(ByRef p As Pilot, ByRef sindex As String)
		Dim stype, sname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		'Invalid_string_refer_to_original_code
		If IsNumeric(sindex) Then
			sname = p.SkillName(CShort(sindex))
		Else
			'Invalid_string_refer_to_original_code
			If InStr(sindex, "Lv") > 0 Then
				stype = Left(sindex, InStr(sindex, "Lv") - 1)
			Else
				stype = sindex
			End If
			sname = p.SkillName(stype)
		End If
		
		msg = SkillHelpMessage(p, sindex)
		
		'è§£èª¬ã®è¡¨ç¤º
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & sname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function SkillHelpMessage(ByRef p As Pilot, ByRef sindex As String) As String
		Dim sname, stype, sname0 As String
		Dim slevel As Double
		Dim sdata As String
		Dim is_level_specified As Boolean
		Dim msg As String
		Dim u, u2 As Unit
		Dim uname, fdata As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		With p
			If IsNumeric(sindex) Then
				stype = .Skill(CShort(sindex))
				slevel = .SkillLevel(CShort(sindex))
				sdata = .SkillData(CShort(sindex))
				sname = .SkillName(CShort(sindex))
				sname0 = .SkillName0(CShort(sindex))
				is_level_specified = .IsSkillLevelSpecified(CShort(sindex))
			Else
				'Invalid_string_refer_to_original_code
				If InStr(sindex, "Lv") > 0 Then
					stype = Left(sindex, InStr(sindex, "Lv") - 1)
				Else
					stype = sindex
				End If
				stype = .SkillType(stype)
				slevel = .SkillLevel(stype)
				sdata = .SkillData(stype)
				sname = .SkillName(stype)
				sname0 = .SkillName0(stype)
				is_level_specified = .IsSkillLevelSpecified(stype)
			End If
			
			'Invalid_string_refer_to_original_code
			u = .Unit_Renamed
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If IsLocalVariableDefined("Invalid_string_refer_to_original_code" & .ID & "]") Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				uname = LocalVariableList.Item("Invalid_string_refer_to_original_code" & .ID & "]").StringValue
				If uname <> "" Then
					u2 = u
					u = UList.Item(uname)
				End If
			End If
			'End If
		End With
		
		Select Case stype
			Case "ã‚ªãƒ¼ãƒ©"
				If u.FeatureName0("ãƒãƒªã‚¢") = "ã‚ªãƒ¼ãƒ©ãƒãƒªã‚¢" Then
					msg = "Invalid_string_refer_to_original_code" & u.FeatureName0("ã‚ªãƒ¼ãƒ©ãƒãƒªã‚¢") & "ã®å¼·åº¦ã«" & VB6.Format(CInt(100 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				If u.IsFeatureAvailable("ã‚ªãƒ¼ãƒ©å¤‰æ›å™¨") Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				If slevel > 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If slevel > 3 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel \ 4)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				If slevel > 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If slevel > 3 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel \ 4)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "ç²¾ç¥ä¸å®‰å®šã«ã‚ˆã‚Š" & Term("Invalid_string_refer_to_original_code", u) & "æ¶ˆè²»é‡ãŒ20%å¢—åŠ ã™ã‚‹"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				If slevel > 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "è¿æ’ƒ"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsFeatureAvailable("ç›¾") Then
					msg = "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(100 * slevel + 400)) & "Invalid_string_refer_to_original_code")
				Else
					msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ•µã‚’å€’ã—ãŸæ™‚ã«å¾—ã‚‰ã‚Œã‚‹" & Term("Invalid_string_refer_to_original_code")
				If Not is_level_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
				ElseIf slevel >= 0 Then 
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & p.SkillName0("å†ç”Ÿ") & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsHero Then
					msg = "åŒèª¿ã«ã‚ˆã‚Š"
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & Term("é‹å‹•æ€§", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "éœŠåŠ›"
				msg = "ç¾åœ¨ã®" & sname0 & "å€¤ã«ã‚ã‚ã›ã¦" & Term("Invalid_string_refer_to_original_code", u) & "ãƒ»" & Term("Invalid_string_refer_to_original_code", u) & "ãƒ»" & Term("Invalid_string_refer_to_original_code", u) & "ãƒ»" & Term("ç§»å‹•åŠ›", u) & "Invalid_string_refer_to_original_code"
				
			Case "éœŠåŠ›æˆé•·"
				If slevel >= 0 Then
					msg = p.SkillName0("éœŠåŠ›") & "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = p.SkillName0("éœŠåŠ›") & "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "åº•åŠ›"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "è¦šæ‚Ÿ"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "ç´ è³ª"
				If Not is_level_specified Then
					msg = "Invalid_string_refer_to_original_code"
				ElseIf slevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "å†ç”Ÿ", "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒï¼ã«ãªã£ãŸæ™‚ã«" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "æ‚Ÿã‚Š"
				msg = Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "ã«ãã‚Œãã‚Œ +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã«ãã‚Œãã‚Œ " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "æŠ€"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼", u) & "ã®" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "é—˜äº‰æœ¬èƒ½"
				If p.MinMorale > 100 Then
					If Not p.IsSkillLevelSpecified("é—˜äº‰æœ¬èƒ½") Then
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					ElseIf slevel >= 0 Then 
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					End If
				Else
					If Not p.IsSkillLevelSpecified("é—˜äº‰æœ¬èƒ½") Then
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					ElseIf slevel >= 0 Then 
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((100 + 5 * slevel) & "Invalid_string_refer_to_original_code")
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((100 + 5 * slevel) & "Invalid_string_refer_to_original_code")
					End If
				End If
				
			Case "æ½œåœ¨åŠ›é–‹æ”¾"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If InStr(sname, "éšç´šLv") = 0 Then
					msg = "éšç´šãƒ¬ãƒ™ãƒ«" & StrConv(VB6.Format(CInt(slevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("å›é¿", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(3 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(3 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u) & "ãƒ»" & Term("å›é¿", u)
				If slevel >= 0 Then
					msg = msg & "ã« +" & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "ã« " & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "æ´è­·"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "æ´è­·é˜²å¾¡"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = sdata & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "ã‚«ã‚¦ãƒ³ã‚¿ãƒ¼"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				If LLength(sdata) = 2 Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "å…ˆèª­ã¿"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(5 * slevel)) & "Invalid_string_refer_to_original_code")
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(5 * System.Math.Abs(slevel))) & "Invalid_string_refer_to_original_code")
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ¯ã‚¿ãƒ¼ãƒ³" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & VB6.Format(p.Level \ 8 + 5) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u) & "ã®å¢—åŠ é‡ãŒ"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "ã®å¢—åŠ é‡ãŒ"
				End If
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "å‘½ä¸­æˆé•·"
				msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u) & "ã®å¢—åŠ é‡ãŒ" & VB6.Format(slevel + 2) & "Invalid_string_refer_to_original_code"
				
			Case "å›é¿æˆé•·"
				msg = "Invalid_string_refer_to_original_code" & Term("å›é¿", u) & "ã®å¢—åŠ é‡ãŒ" & VB6.Format(slevel + 2) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "é˜²å¾¡æˆé•·"
				'Invalid_string_refer_to_original_code
				msg = "Invalid_string_refer_to_original_code" & Term("é˜²å¾¡", u) & "ã®å¢—åŠ é‡ãŒ"
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "ç²¾ç¥çµ±ä¸€"
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "ã®20%æœªæº€(" & VB6.Format(p.MaxSP \ 5) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "æå‚·æ™‚æ°—åŠ›å¢—åŠ "
				If slevel >= -1 Then
					msg = "ãƒ€ãƒ¡ãƒ¼ã‚¸ã‚’å—ã‘ãŸéš›ã«" & Term("æ°—åŠ›", u) & "+" & VB6.Format(CInt(slevel + 1)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "ãƒ€ãƒ¡ãƒ¼ã‚¸ã‚’å—ã‘ãŸéš›ã«" & Term("æ°—åŠ›", u) & VB6.Format(CInt(slevel + 1)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "å‘½ä¸­æ™‚æ°—åŠ›å¢—åŠ "
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "å¤±æ•—æ™‚æ°—åŠ›å¢—åŠ "
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "å›é¿æ™‚æ°—åŠ›å¢—åŠ "
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "èµ·æ­»å›ç”Ÿ"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If slevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(10 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(10 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "å¾—æ„æŠ€"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "ä¸å¾—æ‰‹"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "ãƒãƒ³ã‚¿ãƒ¼"
				msg = "Invalid_string_refer_to_original_code"
				For i = 2 To LLength(sdata)
					If i = 3 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					ElseIf 3 > 2 Then 
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & LIndex(sdata, i)
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼", u)
				For i = 2 To LLength(sdata)
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Next 
				msg = msg & "ã®" & Term("Invalid_string_refer_to_original_code", u) & "æ¶ˆè²»é‡ãŒ"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "ã‚’ä½¿ã£ãŸéš›ã®" & Term("Invalid_string_refer_to_original_code", u) & "å›å¾©é‡ãŒ "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "è£œçµ¦"
				If IsOptionDefined("ç§»å‹•å¾Œè£œçµ¦ä¸å¯") Then
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "è£œçµ¦" & Term("Invalid_string_refer_to_original_code", u) & "ã‚’ä½¿ã£ãŸéš›ã®" & Term("Invalid_string_refer_to_original_code", u) & "å›å¾©é‡ãŒ "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "æ°—åŠ›ä¸Šé™"
				i = 150
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "æ°—åŠ›ä¸‹é™"
				i = 50
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
				' ADD START MARGE
			Case "éŠæ’ƒ"
				msg = "ç§»å‹•å¾Œä½¿ç”¨å¯èƒ½ãªæ­¦å™¨ãƒ»" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				' ADD END MARGE
				
			Case Else
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				With p
					sdata = .SkillData(sname0)
					If ListIndex(sdata, 1) = "è§£èª¬" Then
						msg = ListIndex(sdata, ListLength(sdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
						SkillHelpMessage = msg
						Exit Function
					End If
				End With
				
				'Invalid_string_refer_to_original_code
				With u
					For i = 1 To .CountFeature
						If .Feature(i) = stype Then
							fdata = .FeatureData(i)
							If ListIndex(fdata, 1) = "è§£èª¬" Then
								msg = ListIndex(fdata, ListLength(fdata))
							End If
						End If
					Next 
				End With
				If Not u2 Is Nothing Then
					With u2
						For i = 1 To .CountFeature
							If .Feature(i) = stype Then
								fdata = .FeatureData(i)
								If ListIndex(fdata, 1) = "è§£èª¬" Then
									msg = ListIndex(fdata, ListLength(fdata))
								End If
							End If
						Next 
					End With
				End If
				
				If msg = "" Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		With p
			sdata = .SkillData(sname0)
			If ListIndex(sdata, 1) = "è§£èª¬" Then
				msg = ListIndex(sdata, ListLength(sdata))
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		With u
			For i = 1 To .CountFeature
				If .Feature(i) = sname0 Then
					fdata = .FeatureData(i)
					If ListIndex(fdata, 1) = "è§£èª¬" Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
				End If
			Next 
		End With
		If Not u2 Is Nothing Then
			With u2
				For i = 1 To .CountFeature
					If .Feature(i) = sname0 Then
						fdata = .FeatureData(i)
						If ListIndex(fdata, 1) = "è§£èª¬" Then
							msg = ListIndex(fdata, ListLength(fdata))
							If Left(msg, 1) = """" Then
								msg = Mid(msg, 2, Len(msg) - 2)
							End If
						End If
					End If
				Next 
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "ãƒ¬ãƒ™ãƒ«")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		'End If
		
		SkillHelpMessage = msg
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub FeatureHelp(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean)
		Dim fname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If findex = "æ­¦å™¨ãƒ»é˜²å…·ã‚¯ãƒ©ã‚¹" Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				fname = .AllFeatureName(CShort(findex))
			Else
				fname = .AllFeatureName(findex)
			End If
		End With
		
		msg = FeatureHelpMessage(u, findex, is_additional)
		
		'è§£èª¬ã®è¡¨ç¤º
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & fname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureHelpMessage(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean) As String
		Dim fid As Short
		Dim fname, ftype, fname0 As String
		Dim fdata, opt As String
		Dim flevel, lv_mod As Double
		Dim flevel_specified As Boolean
		Dim msg As String
		Dim i, idx As Short
		Dim buf As String
		Dim prob As Short
		Dim p As Pilot
		Dim sname As String
		Dim slevel As Double
		Dim uname As String
		
		With u
			'Invalid_string_refer_to_original_code
			p = .MainPilot
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If findex = "æ­¦å™¨ãƒ»é˜²å…·ã‚¯ãƒ©ã‚¹" Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				ftype = findex
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				fid = CShort(findex)
				ftype = .AllFeature(fid)
				fname = .AllFeatureName(fid)
				fdata = .AllFeatureData(fid)
				flevel = .AllFeatureLevel(fid)
				flevel_specified = .AllFeatureLevelSpecified(fid)
			Else
				ftype = .AllFeature(findex)
				fname = .AllFeatureName(findex)
				fdata = .AllFeatureData(findex)
				flevel = .AllFeatureLevel(findex)
				flevel_specified = .AllFeatureLevelSpecified(findex)
				For fid = 1 To .CountFeature
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg findex ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If .AllFeature(fid) = findex Then
						Exit For
					End If
				Next 
			End If
			If InStr(fname, "Lv") > 0 Then
				fname0 = Left(fname, InStr(fname, "Lv") - 1)
			Else
				fname0 = fname
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case ftype
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					For i = 1 To u.CountAllFeature
						If i <> fid And .AllFeature(i) = ftype And .AllFeatureData(i) = fdata Then
							flevel = flevel + .AllFeatureLevel(i)
						End If
					Next 
			End Select
		End With
		
		Select Case ftype
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				If p.IsSkillAvailable("Invalid_string_refer_to_original_code") Then
					prob = (p.SkillLevel("Invalid_string_refer_to_original_code") + 1) * 100 \ 16
				End If
				msg = "(" & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				If flevel > 0 Then
					msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				If p.IsSkillAvailable("Invalid_string_refer_to_original_code") Then
					prob = (p.SkillLevel("Invalid_string_refer_to_original_code") + 2) * 100 \ 16
				End If
				msg = "(" & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "ç›¾"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				slevel = p.SkillLevel("Invalid_string_refer_to_original_code")
				If slevel > 0 Then
					slevel = 100 * slevel + 400
				End If
				msg = VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "ãƒãƒªã‚¢"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & "ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "ç™ºå‹•æ™‚ã«10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "éš£æ¥ã™ã‚‹å‘³æ–¹ãƒ¦ãƒ‹ãƒƒãƒˆã«å¯¾ã™ã‚‹"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";ç™ºå‹•æ™‚ã«" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "éš£æ¥ã™ã‚‹å‘³æ–¹ãƒ¦ãƒ‹ãƒƒãƒˆã«å¯¾ã™ã‚‹"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";ç™ºå‹•æ™‚ã«" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";ç™ºå‹•æ™‚ã«10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";ç™ºå‹•æ™‚ã«10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "éš£æ¥ã™ã‚‹å‘³æ–¹ãƒ¦ãƒ‹ãƒƒãƒˆã«å¯¾ã™ã‚‹"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";ç™ºå‹•æ™‚ã«" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "å½“ã¦èº«æŠ€"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(500 * flevel)) & "ã¾ã§ã®"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(500 * flevel)) & "ã¾ã§ã®"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "é˜»æ­¢"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(500 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
						msg = msg & "Invalid_string_refer_to_original_code"
						' MOD END MARGE
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "éš£æ¥ã™ã‚‹å‘³æ–¹ãƒ¦ãƒ‹ãƒƒãƒˆã«å¯¾ã™ã‚‹"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(500 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
						msg = msg & buf & "Invalid_string_refer_to_original_code"
						' MOD END MARGE
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "èåˆ"
				prob = flevel * 100 \ 16
				msg = VB6.Format(flevel) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "å¤‰æ›"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = msg & VB6.Format(0.01 * flevel)
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "å…¨" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ãƒ€ãƒ¡ãƒ¼ã‚¸" & VB6.Format(CInt(500 * flevel)) & "ã¾ã§ã®"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "ç›¸æ®º"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Case "è¿‘æ¥ç„¡åŠ¹"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "æ‰‹å‹•"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¤‰åŒ–(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "éœŠåŠ›"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "ã‚ªãƒ¼ãƒ©"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "ãƒ¬ãƒ™ãƒ«ã«ã‚ˆã‚Šå¼·åº¦ãŒå¢—åŠ (+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "éœŠåŠ›å›å¾©"
				sname = p.SkillName0("éœŠåŠ›")
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & sname & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & sname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "éœŠåŠ›æ¶ˆè²»"
				sname = p.SkillName0("éœŠåŠ›")
				msg = "æ¯ã‚¿ãƒ¼ãƒ³æœ€å¤§" & sname & "ã®" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & sname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 2)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 2), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				If LIndex(fdata, 4) = "æ‰‹å‹•" Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				If LIndex(fdata, 5) = "æ‰‹å‹•" Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = buf & "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "å…¨" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 - 10 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 - 10 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("æ°—åŠ›", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "æŠµæŠ—åŠ›"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(-10 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u)
				Select Case flevel
					Case 1
						msg = msg & "ã‚’æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Case 2
						msg = msg & "ã‚’æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Case 3
						msg = msg & "Invalid_string_refer_to_original_code"
				End Select
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				If IsOptionDefined("ç§»å‹•å¾Œè£œçµ¦ä¸å¯") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				For i = 2 To CInt(fdata)
					buf = LIndex(fdata, i)
					If Left(buf, 1) = "!" Then
						buf = Mid(buf, 2)
						msg = msg & buf & "ä»¥å¤–ã§ã¯" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & buf & "ã§ã¯" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Next 
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "éœŠåŠ›å¤‰æ›å™¨"
				sname = p.SkillName0("éœŠåŠ›")
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "ã‚ªãƒ¼ãƒ©å¤‰æ›å™¨"
				sname = p.SkillName0("ã‚ªãƒ¼ãƒ©")
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = sname & "ãƒ¬ãƒ™ãƒ«ã”ã¨ã«" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = "æ•µã‹ã‚‰" & StrConv(VB6.Format(flevel), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If LLength(fdata) > 1 Then
					If CShort(LIndex(fdata, 2)) > 0 Then
						msg = msg & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "40" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("ç§»å‹•åŠ›", u) & VB6.Format(u.Speed + flevel) & "Invalid_string_refer_to_original_code"
				If LLength(fdata) > 1 Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";" & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				
			Case "æ°´æ³³"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "é€²å…¥ä¸å¯"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "è¿½åŠ ç§»å‹•åŠ›"
				msg = LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If flevel >= 0 Then
					msg = msg & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "æ¯è‰¦"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "æ ¼ç´ä¸å¯"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "å¤‰å½¢"
				If u.IsHero Then
					buf = "å¤‰åŒ–"
				Else
					buf = "å¤‰å½¢"
				End If
				If LLength(fdata) > 2 Then
					msg = "Invalid_string_refer_to_original_code" & buf & "; "
					For i = 2 To LLength(fdata)
						If u.OtherForm(LIndex(fdata, i)).IsAvailable() Then
							If u.Nickname = UDList.Item(LIndex(fdata, i)).Nickname Then
								uname = UDList.Item(LIndex(fdata, i)).Name
								If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
									uname = Left(uname, Len(uname) - 5)
								ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
									uname = Left(uname, Len(uname) - 5) & ")"
								ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
									uname = Left(uname, Len(uname) - 5)
								End If
							Else
								uname = UDList.Item(LIndex(fdata, i)).Nickname
							End If
							msg = msg & uname & "  "
						End If
					Next 
				Else
					If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
						uname = UDList.Item(LIndex(fdata, 2)).Name
					Else
						uname = UDList.Item(LIndex(fdata, 2)).Nickname
					End If
					If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
						uname = Left(uname, Len(uname) - 5)
					ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
						uname = Left(uname, Len(uname) - 5) & ")"
					ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
						uname = Left(uname, Len(uname) - 5)
					End If
					msg = "<B>" & uname & "</B>ã«" & buf & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If flevel_specified Then
					msg = msg & ";ãƒ¦ãƒ‹ãƒƒãƒˆç ´å£Šæ™‚ã«" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(fdata).Nickname Then
					uname = UDList.Item(fdata).Name
				Else
					uname = UDList.Item(fdata).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "Invalid_string_refer_to_original_code" & uname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				If u.Nickname <> uname Then
					uname = "<B>" & uname & "</B>"
				Else
					uname = ""
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = Term("æ°—åŠ›", u) & VB6.Format(100 + 10 * flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_WARNING: FeatureHelpMessage ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				msg = Term("æ°—åŠ›", u) & VB6.Format(100 + 10 * flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = Term("Invalid_string_refer_to_original_code", u) & "ãŒæœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				If u.IsHero Then
					msg = msg & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsHero Then
					buf = "Invalid_string_refer_to_original_code"
				Else
					buf = "Invalid_string_refer_to_original_code"
				End If
				If LLength(fdata) > 3 Then
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = "Invalid_string_refer_to_original_code" & UDList.Item(LIndex(fdata, 2)).Nickname & "</B>ã«" & buf & "; "
					Else
						msg = "Invalid_string_refer_to_original_code" & LIndex(fdata, 2) & "</B>ã«" & buf & "; "
					End If
					
					For i = 3 To LLength(fdata)
						If UDList.IsDefined(LIndex(fdata, i)) Then
							msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
						Else
							msg = msg & LIndex(fdata, i) & "  "
						End If
					Next 
				Else
					If UDList.IsDefined(LIndex(fdata, 3)) Then
						msg = UDList.Item(LIndex(fdata, 3)).Nickname & "ã¨åˆä½“ã—"
					Else
						msg = LIndex(fdata, 3) & "ã¨åˆä½“ã—"
					End If
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = msg & UDList.Item(LIndex(fdata, 2)).Nickname & "ã«" & buf
					Else
						msg = msg & LIndex(fdata, 2) & "ã«" & buf
					End If
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				For i = 2 To LLength(fdata)
					If UDList.IsDefined(LIndex(fdata, i)) Then
						msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
					Else
						msg = msg & LIndex(fdata, i) & "  "
					End If
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LLength(fdata) = 2 Then
					If Not PDList.IsDefined(LIndex(fdata, 2)) Then
						ErrorMessage("Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Exit Function
					End If
					msg = PDList.Item(LIndex(fdata, 2)).Nickname & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					For i = 2 To LLength(fdata)
						If Not PDList.IsDefined(LIndex(fdata, 2)) Then
							ErrorMessage("Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							Exit Function
						End If
						msg = msg & PDList.Item(LIndex(fdata, i)).Nickname & "  "
					Next 
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = msg & VB6.Format(100 - 5 * flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(100 - 5 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = msg & "Invalid_string_refer_to_original_code" & buf & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				msg = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "é˜²å¾¡ä¸å¯"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "å›é¿ä¸å¯"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					If flevel >= 0 Then
						msg = "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				Else
					If flevel >= 0 Then
						msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("å‘½ä¸­", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & "æ°—åŠ›" & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("å›é¿", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("å›é¿", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "æœ€å¤§" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("é‹å‹•æ€§", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = Term("é‹å‹•æ€§", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("ç§»å‹•åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = Term("ç§»å‹•åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "æ­¦å™¨ãƒ»é˜²å…·ã‚¯ãƒ©ã‚¹"
				fdata = Trim(u.WeaponProficiency)
				If fdata <> "" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
				fdata = Trim(u.ArmorProficiency)
				If fdata <> "" Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 3) <> "å…¨" Then
					buf = LIndex(fdata, 3)
					If Left(buf, 1) = "@" Then
						msg = Mid(buf, 2) & "ã«ã‚ˆã‚‹"
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "%)ã§"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "%)ã§"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";ç™ºå‹•æ™‚ã«" & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";ç™ºå‹•æ™‚ã«" & Mid(LIndex(fdata, 5), 2) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("æ°—åŠ›", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				If InStr(fdata, "é€£é–ä¸å¯") > 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.FeatureLevel("Invalid_string_refer_to_original_code") < 0 Then
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel = 1 Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel = 1 Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If LLength(fdata) < 2 Then
					buf = "1"
				Else
					buf = LIndex(fdata, 2)
				End If
				
				If flevel = 1 Then
					msg = msg & buf & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & buf & "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
				' ADD START MARGE
			Case "Invalid_string_refer_to_original_code"
				If LLength(fdata) > 1 Then
					For i = 2 To LLength(fdata)
						If i > 2 Then
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & LIndex(fdata, i)
					Next 
					msg = msg & "ã®"
				Else
					msg = msg & "å…¨åœ°å½¢ã®"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				' ADD END MARGE
				
			Case Else
				If is_additional Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					msg = SkillHelpMessage(u.MainPilot, ftype)
					If Len(msg) > 0 Then
						Exit Function
					End If
					
					'Invalid_string_refer_to_original_code
					If Len(fdata) > 0 Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If Len(msg) = 0 Then
						Exit Function
					End If
				ElseIf Len(fdata) > 0 Then 
					'Invalid_string_refer_to_original_code
					msg = ListIndex(fdata, ListLength(fdata))
					If Left(msg, 1) = """" Then
						msg = Mid(msg, 2, Len(msg) - 2)
					End If
				ElseIf ListIndex(u.AllFeatureData(fname), 1) <> "è§£èª¬" Then 
					'Invalid_string_refer_to_original_code
					Exit Function
				End If
				
		End Select
		
		fdata = u.AllFeatureData(fname0)
		If ListIndex(fdata, 1) = "è§£èª¬" Then
			'Invalid_string_refer_to_original_code
			msg = ListTail(fdata, 2)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		'End If
		
		FeatureHelpMessage = msg
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function AttributeName(ByRef u As Unit, ByRef atr As String, Optional ByVal is_ability As Boolean = False) As String
		Dim fdata As String
		
		Select Case atr
			Case "å…¨"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ ¼"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "ä½æ”¹é€ æ­¦å™¨"
			Case "æ”¹"
				AttributeName = "ä½æ”¹é€ æ­¦å™¨"
			Case "æ”»"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "åæ’ƒå°‚ç”¨"
			Case "æ­¦"
				AttributeName = "æ ¼é—˜æ­¦å™¨"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ¥"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ã‚ª"
				AttributeName = "ã‚ªãƒ¼ãƒ©æŠ€"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ã‚·"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ã‚µ"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å¸"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "å¥ª"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "è²«"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ç„¡"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "å°å°æŠ€"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "é™å®šæŠ€"
			Case "æ®º"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æµ¸"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ç ´"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "â™€"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "åˆä½“æŠ€"
			Case "å…±"
				If Not is_ability Then
					AttributeName = "å¼¾è–¬å…±æœ‰æ­¦å™¨"
				Else
					AttributeName = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ°¸"
				AttributeName = "æ°¸ç¶šæ­¦å™¨"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æŠ€"
				AttributeName = "æŠ€"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "éŸ³"
				If Not is_ability Then
					AttributeName = "Invalid_string_refer_to_original_code"
				Else
					AttributeName = "éŸ³æ³¢" & Term("Invalid_string_refer_to_original_code", u)
				End If
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å¤±"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "éŠ­"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "æ¶ˆè€—æŠ€"
			Case "è‡ª"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "å¤‰å½¢æŠ€"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "åŠ£"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ä¸­"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "çŸ³"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ç—º"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "çœ "
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ä¹±"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ç›²"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ’¹"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ­¢"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "é™¤"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å³"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "è„±"
				AttributeName = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
			Case "ä½æ”»"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ä½é˜²"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ä½é‹"
				AttributeName = Term("é‹å‹•æ€§", u) & "Invalid_string_refer_to_original_code"
			Case "ä½ç§»"
				AttributeName = Term("ç§»å‹•åŠ›", u) & "Invalid_string_refer_to_original_code"
			Case "ç²¾"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "é€£"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å¹"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "è»¢"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "æš—æ®ºæŠ€"
			Case "å°½"
				AttributeName = "å…¨" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "ç›—ã¿"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "è¿½"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ç©º"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å›º"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "è¡°"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ã‚¾"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å®³"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "ãƒ©ãƒ¼ãƒ‹ãƒ³ã‚°"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "å¤‰åŒ–"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ´"
				AttributeName = "æ”¯æ´å°‚ç”¨" & Term("Invalid_string_refer_to_original_code", u)
			Case "é›£"
				AttributeName = "é«˜é›£åº¦" & Term("Invalid_string_refer_to_original_code", u)
			Case "åœ°", "æ°´", "ç«", "é¢¨", "å†·", "é›·", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				AttributeName = atr & "å±æ€§"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				AttributeName = atr & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "å¯¾"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ãƒ©"
				AttributeName = "ãƒ©ãƒ¼ãƒ‹ãƒ³ã‚°å¯èƒ½æŠ€"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "ä½¿ç”¨ç¦æ­¢"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "æ•£"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case Else
				If Left(atr, 1) = "å¼±" Then
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
				ElseIf Left(atr, 1) = "åŠ¹" Then 
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
				End If
		End Select
		
		If Not u Is Nothing Then
			fdata = u.FeatureData(atr)
			If ListIndex(fdata, 1) = "è§£èª¬" Then
				'Invalid_string_refer_to_original_code
				AttributeName = ListIndex(fdata, 2)
				Exit Function
			End If
		End If
		
		If is_ability Then
			'Invalid_string_refer_to_original_code_
			'Or Right$(AttributeName, 2) = "æ­¦å™¨" _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			AttributeName = Left(AttributeName, Len(AttributeName) - 2) & Term("Invalid_string_refer_to_original_code", u)
		End If
		'End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub AttributeHelp(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, Optional ByVal is_ability As Boolean = False)
		Dim msg, aname As String
		Dim prev_mode As Boolean
		
		msg = AttributeHelpMessage(u, atr, idx, is_ability)
		
		'è§£èª¬ã®è¡¨ç¤º
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			If InStr(atr, "L") > 0 Then
				aname = AttributeName(u, Left(atr, InStr(atr, "L") - 1), is_ability) & "ãƒ¬ãƒ™ãƒ«" & StrConv(VB6.Format(Mid(atr, InStr(atr, "L") + 1)), VbStrConv.Wide)
			Else
				aname = AttributeName(u, atr, is_ability)
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & aname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function AttributeHelpMessage(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, ByVal is_ability As Boolean) As String
		Dim atype As String
		Dim alevel As Double
		Dim msg, whatsthis As String
		Dim wanickname, waname, uname As String
		Dim p As Pilot
		Dim i, j As Short
		Dim buf As String
		Dim fdata As String
		
		'Invalid_string_refer_to_original_code
		If InStr(atr, "L") > 0 Then
			atype = Left(atr, InStr(atr, "L") - 1)
			alevel = CDbl(Mid(atr, InStr(atr, "L") + 1))
		Else
			atype = atr
			alevel = DEFAULT_LEVEL
		End If
		
		With u
			'Invalid_string_refer_to_original_code
			If Not is_ability Then
				waname = .Weapon(idx).Name
				wanickname = .WeaponNickname(idx)
				whatsthis = "Invalid_string_refer_to_original_code"
			Else
				waname = .Ability(idx).Name
				wanickname = .AbilityNickname(idx)
				whatsthis = Term("Invalid_string_refer_to_original_code", u)
			End If
			
			'Invalid_string_refer_to_original_code
			p = .MainPilot
		End With
		
		Select Case atype
			Case "æ ¼"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "æ”»"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				msg = "Invalid_string_refer_to_original_code"
			Case "æ”¹"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "æ­¦"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "æ¥"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "ã‚ª"
				msg = "Invalid_string_refer_to_original_code" & p.SkillName0("ã‚ªãƒ¼ãƒ©") & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "ã‚·"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "ã‚µ"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "å¸"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "å¥ª"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "è²«"
				If alevel > 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "ã‚’æœ¬æ¥ã®" & VB6.Format(100 - 10 * alevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "ç„¡"
				msg = "Invalid_string_refer_to_original_code"
			Case "æµ¸"
				msg = "Invalid_string_refer_to_original_code"
			Case "ç ´"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "æ•µã®" & p.SkillName0("å†ç”Ÿ") & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "æ®º"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "â™€"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "ä½¿ç”¨ã™ã‚‹ã¨" & VB6.Format(alevel) & "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					For i = 1 To u.CountWeapon
						If i <> idx And wanickname = u.WeaponNickname(i) Then
							msg = msg & "Invalid_string_refer_to_original_code"
							Exit For
						End If
					Next 
					If u.IsWeaponClassifiedAs(idx, "å…±") And u.Weapon(idx).Bullet = 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				Else
					For i = 1 To u.CountAbility
						If i <> idx And wanickname = u.AbilityNickname(i) Then
							msg = msg & "åŒåã®" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
							Exit For
						End If
					Next 
					If u.IsAbilityClassifiedAs(idx, "å…±") And u.Ability(idx).Stock = 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To u.CountFeature
					If u.Feature(i) = "åˆä½“æŠ€" And LIndex(u.FeatureData(i), 1) = waname Then
						Exit For
					End If
				Next 
				If i > u.CountFeature Then
					ErrorMessage(u.Name & "Invalid_string_refer_to_original_code")
					& "ã€ã«å¯¾å¿œã—ãŸåˆä½“æŠ€èƒ½åŠ›ãŒã‚ã‚Šã¾ã›ã‚“"
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Exit Function
				End If
				If LLength(u.FeatureData(i)) = 2 Then
					uname = LIndex(u.FeatureData(i), 2)
					If UDList.IsDefined(uname) Then
						uname = UDList.Item(uname).Nickname
					End If
					If uname = u.Nickname Then
						msg = "Invalid_string_refer_to_original_code" & uname & "Invalid_string_refer_to_original_code"
					Else
						msg = uname & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "Invalid_string_refer_to_original_code"
					For j = 2 To LLength(u.FeatureData(i))
						uname = LIndex(u.FeatureData(i), j)
						If UDList.IsDefined(uname) Then
							uname = UDList.Item(uname).Nickname
						End If
						msg = msg & uname & "  "
					Next 
				End If
			Case "å…±"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "å›æ•°åˆ¶ã®" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "æ°¸"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If buf = "éè¡¨ç¤º" Then
					buf = "Invalid_string_refer_to_original_code"
				End If
				msg = buf & "æŠ€èƒ½ã«ã‚ˆã£ã¦" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				If is_ability Then
					msg = msg & "Invalid_string_refer_to_original_code" & Term("é­”åŠ›", u) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "æŠ€"
				buf = p.SkillName0("æŠ€")
				If buf = "éè¡¨ç¤º" Then
					buf = "æŠ€"
				End If
				msg = buf & "æŠ€èƒ½ã«ã‚ˆã£ã¦" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "éŸ³"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "ä½¿ç”¨æ™‚ã«æ°—åŠ›" & VB6.Format(5 * alevel) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = "ä½¿ç”¨æ™‚ã«" & VB6.Format(5 * alevel) & p.SkillName0("éœŠåŠ›") & "Invalid_string_refer_to_original_code"
			Case "å¤±"
				msg = "ä½¿ç”¨æ™‚ã«" & VB6.Format(alevel * u.MaxHP \ 10) & "ã®" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "éŠ­"
				msg = "ä½¿ç”¨æ™‚ã«" & VB6.Format(MaxLng(alevel, 1) * u.Value \ 10) & "ã®" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "å°½"
				If Not is_ability Then
					If alevel > 0 Then
						msg = "å…¨" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "(æ®‹ã‚Š" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Else
						msg = "å…¨" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "ä½¿ç”¨å¾Œã«" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "è‡ª"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If u.IsFeatureAvailable("å¤‰å½¢æŠ€") Then
					For i = 1 To u.CountFeature
						If u.Feature(i) = "å¤‰å½¢æŠ€" And LIndex(u.FeatureData(i), 1) = waname Then
							uname = LIndex(u.FeatureData(i), 2)
							Exit For
						End If
					Next 
				End If
				If uname = "" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				If UDList.IsDefined(uname) Then
					With UDList.Item(uname)
						If u.Nickname <> .Nickname Then
							uname = .Nickname
						Else
							uname = .Name
						End If
					End With
				End If
				msg = "ä½¿ç”¨å¾Œã«" & uname & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "åŠ£"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "ä¸­"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "çŸ³"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ç—º"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "çœ "
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ä¹±"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "ç›²"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "æ’¹"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "æ­¢"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "é™¤"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "å³"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = "Invalid_string_refer_to_original_code"
					StrConv(Format$(CInt(alevel)), vbWide) & "ã‚¿ãƒ¼ãƒ³å¾Œã«" & _
					Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "è„±"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				ElseIf alevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
				ElseIf alevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "ä½æ”»"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ä½é˜²"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ä½é‹"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("é‹å‹•æ€§", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ä½ç§»"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("ç§»å‹•åŠ›", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "å¹"
				If alevel > 0 Then
					msg = "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆã‚’" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆã‚’" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "è»¢"
				msg = "Invalid_string_refer_to_original_code" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
			Case "é€£"
				msg = VB6.Format(alevel) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = VB6.Format(100 * alevel \ 16) & "Invalid_string_refer_to_original_code"
			Case "ç²¾"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "æ´"
				msg = "Invalid_string_refer_to_original_code"
			Case "é›£"
				msg = VB6.Format(10 * alevel) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "ç›—ã‚ã‚‹ã‚‚ã®ã¯é€šå¸¸ã¯" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "è¿½"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("è·é›¢ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "ç©º"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("é«˜åº¦ä¿®æ­£") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "å›º"
				msg = "Invalid_string_refer_to_original_code" & Term("æ°—åŠ›", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "è¡°"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "ã‚’ç¾åœ¨å€¤ã® "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "ã‚’ç¾åœ¨å€¤ã® "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ã‚¾"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "å®³"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = msg & VB6.Format(100 + 10 * (alevel + 2))
				msg = msg & VB6.Format(100 + 25 * (alevel + 2))
				'End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "åœ°", "æ°´", "ç«", "é¢¨", "å†·", "é›·", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Select Case atype
					Case "æ°´", "ç«", "é¢¨"
						msg = atype & "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						msg = atype & "Invalid_string_refer_to_original_code"
					Case "åœ°"
						msg = "Invalid_string_refer_to_original_code"
					Case "å†·"
						msg = "å†·æ°—ã«ã‚ˆã‚‹"
					Case "é›·"
						msg = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						msg = "Invalid_string_refer_to_original_code"
					Case "æœ¨"
						msg = "Invalid_string_refer_to_original_code"
				End Select
				msg = msg & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "é­”æ³•ã«ã‚ˆã‚‹" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				msg = atype & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "ç€•æ­»æ™‚ã«ã®ã¿ä½¿ç”¨å¯èƒ½ãª" & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "å¯¾"
				If Not is_ability Then
					whatsthis = "Invalid_string_refer_to_original_code"
				End If
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Case "ãƒ©"
				If Not is_ability Then
					whatsthis = "Invalid_string_refer_to_original_code"
				End If
				msg = "ãƒ©ãƒ¼ãƒ‹ãƒ³ã‚°ãŒå¯èƒ½ãª" & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
			Case "æ•£"
				msg = "Invalid_string_refer_to_original_code"
			Case Else
				'å¼±ã€åŠ¹ã€å‰‹å±æ€§
				Select Case Left(atype, 1)
					Case "å¼±"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code" & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
					Case "åŠ¹"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code" & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code"
						Select Case Mid(atype, 2)
							Case "ã‚ª"
								msg = msg & "ã‚ªãƒ¼ãƒ©"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "ã‚·"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "ã‚µ"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "éœŠåŠ›"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "æŠ€"
								msg = msg & "æŠ€"
							Case Else
								msg = msg & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						End Select
						msg = msg & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ã‚¿ãƒ¼ãƒ³"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
				End Select
		End Select
		
		fdata = u.FeatureData(atype)
		If ListIndex(fdata, 1) = "è§£èª¬" Then
			'Invalid_string_refer_to_original_code
			msg = ListTail(fdata, 3)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'End If
		
		AttributeHelpMessage = msg
	End Function
End Module