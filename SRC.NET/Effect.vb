Option Strict Off
Option Explicit On
Module Effect
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Private WeaponInHand As String
	
	'Invalid_string_refer_to_original_code
	Private CurrentWeaponType As String
	
	
	'æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡å†ç”Ÿç”¨ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³
	Public Sub ShowAnimation(ByRef aname As String)
		Dim buf As String
		Dim ret As Double
		Dim i As Short
		Dim expr As String
		
		If Not BattleAnimation Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		expr = LIndex(aname, 1)
		If InStr(expr, "æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡_") <> 1 Then
			expr = "æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡_" & LIndex(aname, 1)
		End If
		If FindNormalLabel(expr) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
		End If
		expr = "Call(`" & expr & "`"
		For i = 2 To LLength(aname)
			expr = expr & ",`" & LIndex(aname, i) & "`"
		Next 
		expr = expr & ")"
		
		'Invalid_string_refer_to_original_code
		IsPictureDrawn = False
		
		'Invalid_string_refer_to_original_code
		SaveMessageFormStatus()
		
		'æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡å†ç”Ÿ
		SaveBasePoint()
		CallFunction(expr, Expression.ValueType.StringType, buf, ret)
		RestoreBasePoint()
		
		'Invalid_string_refer_to_original_code
		KeepMessageFormStatus()
		
		'ç”»åƒã‚’æ¶ˆå»ã—ã¦ãŠã
		If IsPictureDrawn And LCase(buf) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			MainForm.picMain(0).Refresh()
		End If
		
		Exit Sub
		
ErrorHandler: 
		
		'Invalid_string_refer_to_original_code
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponEffect(ByRef u As Unit, ByVal w As Short)
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			PrepareWeaponAnimation(u, w)
		Else
			PrepareWeaponSound(u, w)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wclass, wname, wtype As String
		Dim double_weapon As Boolean
		Dim sname, aname, cname As String
		Dim with_face_up As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Exit Sub
		'End If
		
		With u
			'Invalid_string_refer_to_original_code
			If .CountWeapon >= 4 And w >= .CountWeapon - 1 And .Weapon(w).Power >= 1800 And ((.Weapon(w).Bullet > 0 And .Weapon(w).Bullet <= 4) Or .Weapon(w).ENConsumption >= 35) Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'            with_face_up = True
			End If
			
			'Invalid_string_refer_to_original_code
			If .Data.Transportation = "ç©º" Then
				WeaponInHand = ""
				GoTo SkipWeaponAnimation
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			WeaponInHand = ""
			GoTo SkipWeaponAnimation
			'End If
			
			wname = .WeaponNickname(w)
			wclass = .Weapon(w).Class_Renamed
		End With
		
		'Invalid_string_refer_to_original_code
		' MOD START MARGE
		'    If Not WeaponAnimation Or IsOptionDefined("æ­¦å™¨æº–å‚™ã‚¢ãƒ‹ãƒ¡éè¡¨ç¤º") Then
		If (Not WeaponAnimation And Not ExtendedAnimation) Or IsOptionDefined("æ­¦å™¨æº–å‚™ã‚¢ãƒ‹ãƒ¡éè¡¨ç¤º") Then
			' MOD END MARGE
			WeaponInHand = ""
			GoTo SkipWeaponAnimation
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "BeamSaber.wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "æ¥") = 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		wtype = CheckWeaponType(wname, wclass)
		If wtype = "æ‰‹è£å‰£" Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		If wtype <> "" Then
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "æ­¦") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "æ­¦å™¨") _
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype <> "" Then
						GoTo FoundWeaponType
					End If
					wtype = CheckWeaponType(.Class0, "")
					If wtype <> "" Then
						GoTo FoundWeaponType
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		GoTo SkipShootingWeapon
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "æ¥") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipShootingWeapon
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		If Not IsBeamWeapon(wname, wclass, cname) Then
			GoTo SkipBeamWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚¤ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "å¤§") > 0 Or Left(wname, 2) = "ã‚®ã‚¬" Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¤") > 0 Or InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 Then 
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			If InStr(wname, "ãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
			End If
		ElseIf CountAttack0(u, w) >= 4 Then 
			wtype = "ãƒã‚·ãƒ³ã‚¬ãƒ³"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "ãƒ¬ãƒ¼ã‚¶ãƒ¼ã‚¬ãƒ³"
		Else
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
		End If
		GoTo FoundWeaponType
		'End If
		
SkipBeamWeapon: 
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ­ãƒ³ã‚°ãƒœã‚¦") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¯ãƒ­ã‚¹ãƒœã‚¦") > 0 Or InStr(wname, "ãƒœã‚¦ã‚¬ãƒ³") > 0 Then
			wtype = "ã‚¯ãƒ­ã‚¹ãƒœã‚¦"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 Then
			wtype = "ãƒã‚ºãƒ¼ã‚«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ˜ãƒ“ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
		wtype = "ãƒã‚·ãƒ³ã‚¬ãƒ³"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¬ãƒˆãƒªãƒ³ã‚°") > 0 Then
			wtype = "ã‚¬ãƒˆãƒªãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ”ã‚¹ãƒˆãƒ«"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ãƒªãƒœãƒ«ãƒ´ã‚¡ãƒ¼") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ã‚¬ãƒ³" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ©ã‚¤ãƒ•ãƒ«"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "å¯¾æˆ¦è»Šãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
			wtype = "å¯¾æˆ¦è»Šãƒ©ã‚¤ãƒ•ãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "å¯¾ç‰©ãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
			wtype = "å¯¾ç‰©ãƒ©ã‚¤ãƒ•ãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ¶ˆç«å™¨") > 0 Then
			wtype = "æ¶ˆç«å™¨"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ”¾æ°´") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
SkipShootingWeapon: 
		
		'Invalid_string_refer_to_original_code
		WeaponInHand = ""
		GoTo SkipWeaponAnimation
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		WeaponInHand = wtype
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "æº–å‚™"
		
		'è‰²
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			aname = aname & " ãƒ”ãƒ³ã‚¯"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			aname = aname & " ã‚°ãƒªãƒ¼ãƒ³"
		ElseIf InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 Then 
			aname = aname & " ãƒ–ãƒ«ãƒ¼"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			aname = aname & " ã‚¤ã‚¨ãƒ­ãƒ¼"
		End If
		'End If
		
		'åŠ¹æœéŸ³
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'Invalid_string_refer_to_original_code
		If double_weapon Then
			aname = aname & "Invalid_string_refer_to_original_code"
		End If
		
		'æº–å‚™ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
		ShowAnimation(aname)
		
SkipWeaponAnimation: 
		
		'Invalid_string_refer_to_original_code
		
		If with_face_up Then
			'Invalid_string_refer_to_original_code
			aname = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			If InStrNotNest(wclass, "ã‚µ") > 0 Then
				aname = aname & " è¡æ’ƒ"
			End If
			
			'Invalid_string_refer_to_original_code
			ShowAnimation(aname)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Function CheckWeaponType(ByRef wname As String, ByRef wclass As String) As String
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 _
		'Or InStr(wname, "ãƒ–ãƒ©ã‚¹ã‚¿ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å‰£") > 0 _
		'Or InStr(wname, "åˆ€") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'UPGRADE_WARNING: CheckWeaponType ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ€ã‚¬ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 3) = "ã‚¹ãƒ­ãƒ¼" Or Right$(wname, 3) = "ã‚¹ãƒ­ã‚¦" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "å¤§å‰£"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "æ‰‹è£å‰£") > 0 Then
			CheckWeaponType = "æ‰‹è£å‰£"
			Exit Function
		End If
		
		If Right(wname, 1) = "å‰£" And (Len(wname) <= 3 Or Right(wname, 2) = "ã®å‰£") Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CheckWeaponType = "é»’å‰£"
		Else
			CheckWeaponType = "å‰£"
		End If
		Exit Function
		'End If
		
		If InStr(wname, "ã‚½ãƒ¼ãƒ‰ãƒ–ãƒ¬ã‚¤ã‚«ãƒ¼") > 0 Then
			CheckWeaponType = "ã‚½ãƒ¼ãƒ‰ãƒ–ãƒ¬ã‚¤ã‚«ãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ¬ã‚¤ãƒ”ã‚¢") > 0 Then
			CheckWeaponType = "ãƒ¬ã‚¤ãƒ”ã‚¢"
			Exit Function
		End If
		
		If InStr(wname, "ã‚·ãƒŸã‚¿ãƒ¼") > 0 Or InStr(wname, "ã‚µãƒ¼ãƒ™ãƒ«") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "ã‚·ãƒŸã‚¿ãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "ãƒŠã‚®ãƒŠã‚¿"
		Exit Function
		'End If
		
		If InStr(wname, "ç«¹åˆ€") > 0 Then
			CheckWeaponType = "ç«¹åˆ€"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "å°å¤ªåˆ€") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If wname = "åˆ€" Or wname = "æ—¥æœ¬åˆ€" Or InStr(wname, "å¤ªåˆ€") > 0 Then
			CheckWeaponType = "æ—¥æœ¬åˆ€"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "åæ‰‹") > 0 Then
			CheckWeaponType = "åæ‰‹"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "æ–§") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "ãƒãƒˆãƒ«") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ã‚¹ãƒ‘ãƒŠ") > 0 Then
			CheckWeaponType = "ã‚¹ãƒ‘ãƒŠ"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ¡ã‚¤ã‚¹") > 0 Then
			CheckWeaponType = "ãƒ¡ã‚¤ã‚¹"
			Exit Function
		End If
		
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If Right(wname, 3) = "ãƒ¢ãƒ¼ãƒ«" Then
			CheckWeaponType = "ãƒ¢ãƒ¼ãƒ«"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "é­"
		Exit Function
		'End If
		
		If wname = "ã‚µã‚¤" Then
			CheckWeaponType = "ã‚µã‚¤"
			Exit Function
		End If
		
		If InStr(wname, "ãƒˆãƒ³ãƒ•ã‚¡ãƒ¼") > 0 Then
			CheckWeaponType = "ãƒˆãƒ³ãƒ•ã‚¡ãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "ã‚¯ãƒ­ãƒ¼"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		
		If InStr(wname, "ãƒ¢ãƒ¼ãƒ‹ãƒ³ã‚°ã‚¹ã‚¿ãƒ¼") > 0 Then
			CheckWeaponType = "ãƒ¢ãƒ¼ãƒ‹ãƒ³ã‚°ã‚¹ã‚¿ãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ•ãƒ¬ã‚¤ãƒ«") > 0 Then
			CheckWeaponType = "ãƒ•ãƒ¬ã‚¤ãƒ«"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ãƒŒãƒ³ãƒãƒ£ã‚¯") > 0 Then
			CheckWeaponType = "ãƒŒãƒ³ãƒãƒ£ã‚¯"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 Then
			CheckWeaponType = "ãƒã‚§ãƒ¼ãƒ³"
			Exit Function
		End If
		
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ãƒãƒ£ã‚¯ãƒ©ãƒ ") > 0 Then
			CheckWeaponType = "ãƒãƒ£ã‚¯ãƒ©ãƒ "
			Exit Function
		End If
		
		If InStr(wname, "ã‚½ãƒ¼ã‚µãƒ¼") > 0 Then
			CheckWeaponType = "ã‚½ãƒ¼ã‚µãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "ã‚¯ãƒŠã‚¤") > 0 Then
			CheckWeaponType = "ã‚¯ãƒŠã‚¤"
			Exit Function
		End If
		
		If InStr(wname, "çŸ³") > 0 Or InStr(wname, "ç¤«") > 0 Then
			CheckWeaponType = "çŸ³"
			Exit Function
		End If
		
		If InStr(wname, "å²©") > 0 Then
			CheckWeaponType = "å²©"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "æ‰‹æ¦´å¼¾") > 0 Then
			CheckWeaponType = "æ‰‹æ¦´å¼¾"
			Exit Function
		End If
		
		If InStr(wname, "ãƒãƒ†ãƒˆã‚¹ãƒãƒƒã‚·ãƒ£ãƒ¼") > 0 Then
			CheckWeaponType = "ãƒãƒ†ãƒˆã‚¹ãƒãƒƒã‚·ãƒ£ãƒ¼"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If InStr(wname, "æŠ•ã’") > 0 Then
				CheckWeaponType = "Invalid_string_refer_to_original_code"
				Exit Function
			End If
		End If
		
		If InStr(wname, "ç«ç‚ç“¶") > 0 Then
			CheckWeaponType = "ç«ç‚ç“¶"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "æ‰‹éŒ ") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "æœ­") > 0 Then
			CheckWeaponType = "ãŠæœ­"
			Exit Function
		End If
		
		
		If InStr(wname, "ãƒªãƒœãƒ³") > 0 Then
			CheckWeaponType = "ãƒªãƒœãƒ³"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		
		If InStr(wname, "ã‚«ã‚¿ãƒ­ã‚°") > 0 Then
			CheckWeaponType = "ã‚«ã‚¿ãƒ­ã‚°"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ•ãƒ©ã‚¤ãƒ‘ãƒ³") > 0 Then
			CheckWeaponType = "ãƒ•ãƒ©ã‚¤ãƒ‘ãƒ³"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "å”å‚˜") > 0 Then
			CheckWeaponType = "å”å‚˜"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStr(wname, "Invalid_string_refer_to_original_code") = 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ã‚®ã‚¿ãƒ¼") > 0 Then
			CheckWeaponType = "ã‚®ã‚¿ãƒ¼"
			Exit Function
		End If
		
		If InStr(wname, "ãƒãƒªã‚»ãƒ³") > 0 Then
			CheckWeaponType = "ãƒãƒªã‚»ãƒ³"
			Exit Function
		End If
		
		If wname = "Invalid_string_refer_to_original_code" Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¸ãƒ£ãƒ™ãƒªãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ã‚¹ãƒ”ã‚¢") > 0 Then
			CheckWeaponType = "ã‚¹ãƒ”ã‚¢"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ãƒ©ãƒ³ã‚¹") > 0 Or InStr(wname, "ãƒ©ãƒ³ã‚µãƒ¼") > 0 Then
			CheckWeaponType = "ãƒ©ãƒ³ã‚¹"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ‘ã‚¤ã‚¯") > 0 Then
			CheckWeaponType = "ãƒ©ãƒ³ã‚¹"
			Exit Function
		End If
		
		If InStr(wname, "ã‚¨ã‚¹ãƒˆãƒƒã‚¯") > 0 Then
			CheckWeaponType = "ã‚¨ã‚¹ãƒˆãƒƒã‚¯"
			Exit Function
		End If
		
		If wname = "Invalid_string_refer_to_original_code" Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ãƒ‰ãƒªãƒ«") > 0 Then
			CheckWeaponType = "ãƒ‰ãƒªãƒ«"
			Exit Function
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 _
		'Or InStr(wname, "ãƒ–ãƒ©ã‚¹ã‚¿ãƒ¼") > 0 _
		'Or InStr(wname, "é«˜å‘¨æ³¢") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or wname = "ãƒ©ãƒ³ã‚µãƒ¼" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("BeamSaber.wav")
		'End If
		'End If
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackEffect(ByRef u As Unit, ByVal w As Short)
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			AttackAnimation(u, w)
		Else
			AttackSound(u, w)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, bmpname, cname0 As String
		Dim sname, sname0 As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim is_handy_weapon As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ShowAnimation("Invalid_string_refer_to_original_code")
		Exit Sub
		'End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é€£") > 0 Or InStrNotNest(wclass, "é€£") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		double_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		combo_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "æ¥") = 0 _
		'And InStrNotNest(wclass, "æ ¼") = 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 3) = "ã‚¹ãƒ­ãƒ¼" Or Right$(wname, 3) = "ã‚¹ãƒ­ã‚¦" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "çªæ’ƒ") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ãƒãƒ£ãƒ¼ã‚¸") > 0 Then
			Select Case WeaponInHand
				Case ""
					'Invalid_string_refer_to_original_code
				Case Else
					wtype = WeaponInHand & "çªæ’ƒ"
					GoTo FoundWeaponType
			End Select
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 5) = "ã‚¹ãƒˆãƒ©ã‚¤ã‚¯" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒŠãƒƒã‚¯ãƒ«") > 0 Or InStr(wname, "ãƒ–ãƒ­ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æ®´") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚·ãƒŸã‚¿ãƒ¼") > 0 Or InStr(wname, "ã‚µãƒ¼ãƒ™ãƒ«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¯ãƒ­ãƒ¼") > 0 Or InStr(wname, "çˆª") > 0 _
		'Or InStr(wname, "ã²ã£ã‹ã") > 0 _
		'Or InStr(wname, "ã‚¢ãƒ¼ãƒ ") > 0 _
		'Or Right$(wname, 1) = "å°¾" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "å›è»¢") > 0 Then 
			wtype = "ç™½å…µå›è»¢"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "æŒ¯ã‚Šä¸Šã’"
		Else
			wtype = "ç™½å…µæ­¦å™¨"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "å›è»¢") > 0 Then 
			wtype = "ç™½å…µå›è»¢"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "æŒ¯ã‚Šä¸Šã’"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'å¤§ããæŒ¯ã‚Šã¾ã‚ã™æ­¦å™¨
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒ¢ãƒ¼ãƒ‹ãƒ³ã‚°ã‚¹ã‚¿ãƒ¼") > 0 Then
			wtype = "ãƒ¢ãƒ¼ãƒ‹ãƒ³ã‚°ã‚¹ã‚¿ãƒ¼"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒ•ãƒ¬ã‚¤ãƒ«") > 0 Then
			wtype = "ãƒ•ãƒ¬ã‚¤ãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 And InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ã‚½ãƒ¼") = 0 Then
			wtype = "ãƒã‚§ãƒ¼ãƒ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒŒãƒ³ãƒãƒ£ã‚¯") > 0 Then
			wtype = "ãƒŒãƒ³ãƒãƒ£ã‚¯"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'çªãåˆºã™æ­¦å™¨
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ©ãƒ³ã‚¹") > 0 Or InStr(wname, "ãƒ©ãƒ³ã‚µãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¸ãƒ£ãƒ™ãƒªãƒ³") > 0 _
		'Or InStr(wname, "ãƒ¬ã‚¤ãƒ”ã‚¢") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'ç‰¹æ®Šãªæ ¼é—˜æ­¦å™¨
		
		If InStr(wname, "ãƒ‰ãƒªãƒ«") > 0 Then
			wtype = "ãƒ‰ãƒªãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ã‚½ãƒ¼") > 0 Then
			wtype = "ãƒã‚§ãƒ¼ãƒ³ã‚½ãƒ¼"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "æ­¦") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "æ­¦å™¨") _
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype = "" Then
						wtype = CheckWeaponType(.Class0, "")
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		Select Case wtype
			Case "ã‚¹ãƒ”ã‚¢", "ãƒ©ãƒ³ã‚¹", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'"ã‚¨ã‚¹ãƒˆãƒƒã‚¯"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
			Case Else
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "å›è»¢") > 0 Then 
					wtype = "ç™½å…µå›è»¢"
				ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
					wtype = "æŒ¯ã‚Šä¸Šã’"
				Else
					wtype = "ç™½å…µæ­¦å™¨"
				End If
		End Select
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "æ¥") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipThrowingWeapon
		'End If
		
		'æŠ•æ“²æ­¦å™¨
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¯ãƒŠã‚¤") > 0 Or InStr(wname, "è‹¦ç„¡") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "çŸ³") > 0 Or InStr(wname, "ç¤«") > 0 Then
			wtype = "çŸ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "å²©") > 0 Then
			wtype = "å²©"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If InStr(wname, "æŠ•ã’") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
				GoTo FoundWeaponType
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "æ–§") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "ãƒãƒˆãƒ«") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¤§éŒæŠ•æ“²"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "éŒæŠ•æ“²"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒãƒ£ã‚¯ãƒ©ãƒ ") > 0 Then
			wtype = "ãƒãƒ£ã‚¯ãƒ©ãƒ "
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ‰‹è£å‰£") > 0 Then
			wtype = "æ‰‹è£å‰£"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ‰‹æ¦´å¼¾") > 0 Then
			wtype = "æ‰‹æ¦´å¼¾"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ç«ç‚ç“¶") > 0 Then
			wtype = "ç«ç‚ç“¶"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ‰‹éŒ ") > 0 Then
			wtype = "æ‰‹éŒ "
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "æœ­") > 0 Then
			wtype = "ãŠæœ­"
			GoTo FoundWeaponType
		End If
		
		'å¼“çŸ¢
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ­ãƒ³ã‚°ãƒœã‚¦") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¼“çŸ¢"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "çŸ¢") > 0 Or InStr(wname, "ã‚¢ãƒ­ãƒ¼") > 0 Then
			If CountAttack0(u, w) > 1 Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "çŸ¢"
			End If
			GoTo FoundWeaponType
		End If
		
		'é è·é›¢ç³»ã®æ ¼é—˜æ­¦å™¨
		
		'æŒ¯ã‚‹æ­¦å™¨
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ç™½å…µæ­¦å™¨"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'å¤§ããæŒ¯ã‚Šã¾ã‚ã™æ­¦å™¨
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 Then
			wtype = "ãƒã‚§ãƒ¼ãƒ³"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
SkipThrowingWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		is_handy_weapon = True
		
		'Invalid_string_refer_to_original_code
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			GoTo SkipNormalHandWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "ã‚¯ãƒ­ã‚¹ãƒœã‚¦") > 0 Or InStr(wname, "ãƒœã‚¦ã‚¬ãƒ³") > 0 Then
			wtype = "ã‚¯ãƒ­ã‚¹ãƒœã‚¦"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 Then
			wtype = "ãƒã‚ºãƒ¼ã‚«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "å¯¾æˆ¦è»Šãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
			wtype = "å¯¾æˆ¦è»Šãƒ©ã‚¤ãƒ•ãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "å¯¾ç‰©ãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
			wtype = "å¯¾ç‰©ãƒ©ã‚¤ãƒ•ãƒ«"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ”ã‚¹ãƒˆãƒ«"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ãƒªãƒœãƒ«ãƒ´ã‚¡ãƒ¼") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ©ã‚¤ãƒ•ãƒ«"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ˜ãƒ“ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
		wtype = "ãƒã‚·ãƒ³ã‚¬ãƒ³"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¬ãƒˆãƒªãƒ³ã‚°") > 0 Then
			wtype = "ã‚¬ãƒˆãƒªãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒ¬ãƒ¼ãƒ«ã‚¬ãƒ³") > 0 Or InStr(wname, "ãƒªãƒ‹ã‚¢ã‚¬ãƒ³") > 0 Then
			PlayWave("Thunder.wav")
			Sleep(300)
			wtype = "ã‚­ãƒ£ãƒãƒ³ç ²"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If Right(wname, 2) = "ã‚¬ãƒ³" Then
			wtype = "ãƒ©ã‚¤ãƒ•ãƒ«"
			GoTo FoundWeaponType
		End If
		
		GoTo SkipHandWeapon
		
SkipNormalHandWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚¤ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "å¤§") > 0 Or Left(wname, 2) = "ã‚®ã‚¬" Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¤") > 0 Or InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 Then 
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			If InStr(wname, "ãƒ©ã‚¤ãƒ•ãƒ«") > 0 Then
				bmpname = "Weapon\EFFECT_BusterRifle01.bmp"
			End If
		ElseIf CountAttack0(u, w) >= 4 Then 
			wtype = "ãƒ¬ãƒ¼ã‚¶ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
			bmpname = "Weapon\EFFECT_Rifle01.bmp"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "ãƒ¬ãƒ¼ã‚¶ãƒ¼ã‚¬ãƒ³"
		Else
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
		End If
		
		If wtype = "ãƒã‚¹ã‚¿ãƒ¼" Then
			wtype0 = "ç²’å­é›†ä¸­"
		End If
		
		GoTo FoundWeaponType
		'End If
		
SkipHandWeapon: 
		
		'Invalid_string_refer_to_original_code
		is_handy_weapon = False
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "ãƒŸã‚µã‚¤ãƒ«") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "ãƒŸã‚µã‚¤ãƒ«"
			
			If InStr(wname, "ãƒ‰ãƒªãƒ«") > 0 Then
				wtype = "ãƒ‰ãƒªãƒ«ãƒŸã‚µã‚¤ãƒ«"
				GoTo FoundWeaponType
			End If
			
			attack_times = CountAttack0(u, w)
			
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "å¯¾è‰¦") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
			attack_times = 1
		ElseIf InStr(wname, "å°å‹") > 0 Then 
			wtype = "å°å‹ãƒŸã‚µã‚¤ãƒ«"
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "ãƒã‚¤ã‚¯ãƒ­") > 0 Or InStr(wname, "ã‚¹ãƒ—ãƒ¬ãƒ¼") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "å°å‹ãƒŸã‚µã‚¤ãƒ«"
			attack_times = 6
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		
		bmpname = "Bullet\EFFECT_BazookaBullet01.bmp"
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If u.Weapon(w).MaxRange = 1 Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
				attack_times = CountAttack0(u, w)
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "å…‰å­é­šé›·") > 0 Then
			wtype = "å…‰å­é­šé›·"
			GoTo FoundWeaponType
		End If
		
		'(æ€ªå…‰ç·šç³»)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "æ¶ˆç«") > 0 Then
			wtype = "æ¶ˆç«å™¨"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or Right(wname, 1) = "æ¶²" Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Bow.wav"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			bmpname = "Bullet\EFFECT_Venom01.bmp"
		Else
			bmpname = "Bullet\EFFECT_WaterShot01.bmp"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç¸®é€€") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "é‡åŠ›å¼¾"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "è½é›·") > 0 Or Right(wname, 2) = "ç¨²å¦»" Then
			wtype = "è½é›·"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "é›·") > 0 Or InStr(wname, "ãƒ©ã‚¤ãƒˆãƒ‹ãƒ³ã‚°") > 0 Or InStr(wname, "ã‚µãƒ³ãƒ€ãƒ¼") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If u.Weapon(w).MaxRange = 1 Then
				wtype = "Invalid_string_refer_to_original_code"
				sname = "Thunder.wav"
			Else
				wtype = "è½é›·"
			End If
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Thunder.wav"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¨ãƒãƒ«ã‚®ãƒ¼å¼¾") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ³¡") > 0 Or InStr(wname, "ãƒãƒ–ãƒ«") > 0 Then
			wtype = "æ³¡"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚½ãƒ‹ãƒƒã‚¯") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å«ã³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "éŸ³æ³¢"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "éŸ³ç¬¦"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		If CountAttack0(u, w) > 1 Then
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "æ´¥æ³¢") > 0 Or InStr(wname, "ãƒ€ã‚¤ãƒ€ãƒ«") > 0 Then
			wtype = "æ´¥æ³¢"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "æµæ˜Ÿ"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "éš•çŸ³") > 0 Then
			wtype = "éš•çŸ³"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ç«œå·»"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã¤ã‚‰ã‚‰") > 0 Then
			wtype = "æ°·å¼¾"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ã¤ã¶ã¦") > 0 Then
			wtype = "å²©å¼¾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¹é›ª"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¼·é¢¨"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é¢¨") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "é¢¨"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ã‚¬ã‚¹" Or Right$(wname, 1) = "éœ§" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "Invalid_string_refer_to_original_code"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ç«ç‚å¼¾") > 0 Then
			wtype = "ç«ç‚å¼¾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "AntiShipMissile.wav"
		GoTo FoundWeaponType
		'End If
		
		If Right(wname, 5) = "ãƒ•ã‚¡ã‚¤ã‚¢ãƒ¼" Or Right(wname, 5) = "ãƒ•ã‚¡ã‚¤ãƒ¤ãƒ¼" Or Right(wname, 4) = "ãƒ•ã‚¡ã‚¤ã‚¢" Or Right(wname, 4) = "ãƒ•ã‚¡ã‚¤ãƒ¤" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
			sname = "AntiShipMissile.wav"
		End If
		GoTo FoundWeaponType
		'End If
		'End If
		
		If InStr(wname, "æ¯") > 0 Or Right(wname, 3) = "ãƒ–ãƒ¬ã‚¹" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Breath.wav"
			
			Select Case SpellColor(wname, wclass)
				Case "èµ¤", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					cname = SpellColor(wname, wclass)
			End Select
			
			GoTo FoundWeaponType
		End If
		'End If
		
		If InStr(wname, "ã‚¨ãƒãƒ«ã‚®ãƒ¼æ³¢") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "è¡æ’ƒ") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			cname = "ç™½"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "æ°—å¼¾") > 0 Then
			wtype = "æ°—å¼¾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "æ°—æ–¬"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'        cname = SpellColor(wname, wclass)
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Whiz.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "ãƒã‚¤ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "å¤§") > 0 Or Left(wname, 2) = "ã‚®ã‚¬" Then
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf InStr(wname, "ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¤") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "å¯¾ç©º") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf InStr(wname, "ãƒ©ãƒ³ãƒãƒ£ãƒ¼") > 0 Or InStr(wname, "ã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "ã‚«ãƒãƒ³") > 0 Or InStr(wname, "ç ²") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "å°ãƒ“ãƒ¼ãƒ "
			End If
			
			If wtype = "Invalid_string_refer_to_original_code" Then
				wtype0 = "ç²’å­é›†ä¸­"
			End If
			
			Select Case wtype
				Case "å°ãƒ“ãƒ¼ãƒ ", "Invalid_string_refer_to_original_code"
					If double_weapon Then
						wtype = "Invalid_string_refer_to_original_code" & wtype
					End If
			End Select
			
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 Then
			wtype = "ãƒãƒ«ã‚«ãƒ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "æ©ŸéŠƒ") > 0 Or InStr(wname, "æ©Ÿé–¢ç ²") > 0 Then
			wtype = "æ©Ÿé–¢ç ²"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ã‚¬ãƒ³") > 0 Or InStr(wname, "ã‚¬ãƒ³ãƒ©ãƒ³ãƒãƒ£ãƒ¼") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚·ãƒ³ã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "ã‚ªãƒ¼ãƒˆã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "é‡æ©Ÿé–¢ç ²"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒ™ã‚¢ãƒªãƒ³ã‚°") > 0 Or InStr(wname, "ã‚¯ãƒ¬ã‚¤ãƒ¢ã‚¢") > 0 Then
			wtype = "ãƒ™ã‚¢ãƒªãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "ç ²") > 0 Or InStr(wname, "ã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "ã‚«ãƒãƒ³") > 0 Or InStr(wname, "å¼¾") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			PlayWave("Thunder.wav")
			Sleep(300)
		End If
		
		wtype = "ã‚­ãƒ£ãƒãƒ³ç ²"
		
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
SkipShootingWeapon: 
		
		'Invalid_string_refer_to_original_code
		wtype = "Invalid_string_refer_to_original_code"
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Select Case wtype
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "å°ãƒ“ãƒ¼ãƒ "
			Case "ãƒ¬ãƒ¼ã‚¶ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
				wtype = "Invalid_string_refer_to_original_code"
			Case "ãƒ¬ãƒ¼ã‚¶ãƒ¼ã‚¬ãƒ³"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "ãƒã‚·ãƒ³ã‚¬ãƒ³"
				wtype = "æ©Ÿé–¢ç ²"
			Case "ãƒ˜ãƒ“ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
				wtype = "é‡æ©Ÿé–¢ç ²"
			Case "ã‚¬ãƒˆãƒªãƒ³ã‚°"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "ãƒ™ã‚¢ãƒªãƒ³ã‚°"
			Case Else
				'æ‰‹æŒã¡æ­¦å™¨ã®ç”»åƒã‚’ç©ºã«ã™ã‚‹
				bmpname = "-.bmp"
		End Select
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			Select Case wtype
				Case "çŸ¢", "å°å‹ãƒŸã‚µã‚¤ãƒ«", "ãƒŸã‚µã‚¤ãƒ«", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'"å¼·é¢¨", "ç«œå·»", "æ´¥æ³¢", "æ³¡", _
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = "Invalid_string_refer_to_original_code" & wtype
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = "Invalid_string_refer_to_original_code"
				Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					wtype = "Invalid_string_refer_to_original_code"
				Case "Invalid_string_refer_to_original_code"
					wtype = "Invalid_string_refer_to_original_code"
				Case "é‡åŠ›å¼¾"
					wtype = "Invalid_string_refer_to_original_code"
				Case Else
					If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
						wtype = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						wtype = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Or InStr(wname, "ã‚¯ã‚¨ã‚¤ã‚¯") > 0 _
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						wtype = "Invalid_string_refer_to_original_code"
						sname = " Explode(Far).wav"
					ElseIf InStr(wname, "æ ¸") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then 
						wtype = "Invalid_string_refer_to_original_code"
					End If
			End Select
		End If
		
		
		'Invalid_string_refer_to_original_code
		CurrentWeaponType = wtype
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "èµ¤") > 0 Then
			cname = "èµ¤"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "ç™½"
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(wtype0) > 0 Then
			'Invalid_string_refer_to_original_code
			aname = wtype0 & "æº–å‚™"
			
			'è‰²
			If Len(cname0) > 0 Then
				aname = aname & " " & cname0
			ElseIf Len(cname) > 0 Then 
				aname = aname & " " & cname
			End If
			
			'åŠ¹æœéŸ³
			If Len(sname0) > 0 Then
				aname = aname & " " & sname0
			End If
			
			'æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			ShowAnimation(aname)
		End If
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(bmpname) > 0 Then
			aname = aname & " " & bmpname
		End If
		
		'è‰²
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'åŠ¹æœéŸ³
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'Invalid_string_refer_to_original_code
		ShowAnimation(aname)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or u.IsWeaponClassifiedAs(w, "æ¥") _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Exit Sub
		'End If
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			Exit Sub
		End If
		If InStrNotNest(wclass, "æ­¦") > 0 Then
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				Exit Sub
			End If
		End If
		
		'åŠ¹æœéŸ³ã®å†ç”Ÿå›æ•°
		num = CountAttack(u, w)
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "ä¸»ç ²") > 0 Or InStr(wname, "å‰¯ç ²") > 0 Then
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Cannon.wav"
			End If
		ElseIf InStr(wname, "å¯¾ç©ºç ²") > 0 Then 
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Beam.wav"
				num = 4
			Else
				sname = "MachineCannon.wav"
			End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "è¡æ’ƒæ³¢") > 0 Or InStr(wname, "é›»ç£æ³¢") > 0 _
			'Or InStr(wname, "é›»æ³¢") > 0 Or InStr(wname, "éŸ³æ³¢") > 0 _
			'Or InStr(wname, "ç£åŠ›") > 0 _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "ã‚¨ãƒãƒ«ã‚®ãƒ¼") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			sname = "LaserGun.wav"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "ã‚¤ã‚ªãƒ³") > 0 Or InStr(wname, "ãƒ—ãƒ­ãƒŸãƒãƒ³ã‚¹") > 0 _
			'Or InStr(wname, "ãƒã‚¤ãƒ‰ãƒ­") > 0 Or InStr(wname, "ã‚¤ãƒ³ãƒ‘ãƒ«ã‚¹") > 0 _
			'Or InStr(wname, "ãƒ•ãƒ¬ã‚¤ãƒ ") > 0 Or InStr(wname, "ã‚µãƒ³ã‚·ãƒ£ã‚¤ãƒ³") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			sname = "Beam.wav"
		ElseIf InStr(wname, "ã‚·ãƒ¥ãƒ¼ã‚¿ãƒ¼") > 0 Then 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			sname = "Missile.wav"
		Else
			sname = "Beam.wav"
		End If
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "LaserGun.wav"
		End If
		If InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 Or InStr(wname, "ãƒã‚·ãƒ³ã‚¬ãƒ³") > 0 Then
			num = 4
		End If
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æ©ŸéŠƒ") > 0 _
		'Or InStr(wname, "ãƒã‚·ãƒ³ã‚¬ãƒ³") > 0 _
		'Or InStr(wname, "ã‚¢ã‚µãƒ«ãƒˆãƒ©ã‚¤ãƒ•ãƒ«") > 0 _
		'Or InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ãƒ©ã‚¤ãƒ•ãƒ«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚¦ãƒ©ãƒ¼ç ²") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "MachineGun.wav"
		End If
		num = 1
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "MachineCannon.wav"
		End If
		num = 1
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 _
		'Or InStr(wname, "ã‚¬ãƒˆãƒªãƒ³ã‚°") > 0 _
		'Or InStr(wname, "ãƒãƒ³ãƒ‰ãƒ¬ãƒ¼ãƒ«ã‚¬ãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "GunPod.wav"
		End If
		num = 1
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Thunder.wav")
		Sleep(300)
		PlayWave("Cannon.wav")
		For i = 2 To num
			Sleep(130)
			PlayWave("Cannon.wav")
		Next 
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Rifle.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒŠãƒ‘ãƒ¼ãƒ ") > 0 _
		'Or InStr(wname, "ã‚¯ãƒ¬ã‚¤ãƒ¢ã‚¢") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç„¡åå‹•ç ²") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Bazooka.wav"
		End If
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "FastGun.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¢ãƒ­ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒœã‚¦ã‚¬ãƒ³") > 0 _
		'Or InStr(wname, "ãƒ­ãƒ³ã‚°ãƒœã‚¦") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é«ª") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Bow.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Swing.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Bomb.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Explode.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "MicroMissile.wav"
		num = 1
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "MicroMissile.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ©ãƒ³ãƒãƒ£ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Missile.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Cannon.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Gun.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¹ãƒ©ã‚¤ã‚µãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Saber.wav"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Shock(Low).wav"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒãƒªã‚±ãƒ¼ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚µã‚¤ã‚¯ãƒ­ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç«œå·»") > 0 _
		'Or InStr(wname, "æ¸¦å·»") > 0 _
		'Or InStr(wname, "å°é¢¨") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å¹é›ª") > 0 _
		'Or InStr(wname, "ãƒ•ãƒªãƒ¼ã‚¶ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Storm.wav"
		num = 1
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Swing.wav"
		num = 5
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç¨²å¦»") > 0 _
		'Or InStr(wname, "æ”¾é›»") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é›·") > 0 _
		'Or InStrNotNest(wclass, "é›·") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Thunder.wav"
		num = 1
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "AntiShipMissile.wav"
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Fire.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚µã‚¤ã‚³ã‚­ãƒã‚·ã‚¹") > 0 _
		'Or InStr(wname, "ç³¸") > 0 _
		'Or InStr(wname, "ã‚¢ãƒ³ã‚«ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Whiz.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Bubble.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Shower.wav"
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If InStrNotNest(wclass, "ç«") > 0 Then
			sname = "AntiShipMissile.wav"
		ElseIf InStrNotNest(wclass, "å†·") > 0 Then 
			sname = "Storm.wav"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			sname = "GunPod.wav"
		ElseIf InStrNotNest(wclass, "æ°´") > 0 Then 
			sname = "Hide.wav"
		Else
			sname = "AntiShipMissile.wav"
		End If
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "MultipleRocketLauncher(Light).wav"
		num = 1
		'UPGRADE_WARNING: AttackSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		sname = "Beam.wav"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		sname = "Gun.wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		If sname = "" Then
			'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
			IsWavePlayed = False
			Exit Sub
		End If
		
		For i = 1 To num
			PlayWave(sname)
			
			'ã‚¦ã‚§ã‚¤ãƒˆã‚’å…¥ã‚Œã‚‹
			Sleep(130)
			If sname = "Swing.wav" Then
				Sleep(150)
			End If
		Next 
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub HitEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, Optional ByVal hit_count As Short = 0)
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			HitAnimation(u, w, t, hit_count)
		Else
			HitSound(u, w, t, hit_count)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub HitAnimation(ByRef u As Unit, ByVal w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, sname As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ShowAnimation("ãƒ€ãƒ¡ãƒ¼ã‚¸å‘½ä¸­")
		Exit Sub
		'End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			If u.WeaponPower(w, "") = 0 Then
				Exit Sub
			End If
			
			wtype = "ãƒ€ãƒ¡ãƒ¼ã‚¸"
			
			If IsBeamWeapon(wname, wclass, cname) Or InStr(wname, "ãƒŸã‚µã‚¤ãƒ«") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Explode.wav"
			End If
			
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "ãƒ€ãƒ–ãƒ«") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ã‚³ãƒ³ãƒ“ãƒãƒ¼ã‚·ãƒ§ãƒ³") > 0 Or InStr(wname, "é€£") > 0 Or InStrNotNest(wclass, "é€£") > 0 Then
			double_attack = True
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		combo_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "æ¥") = 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "çªæ’ƒ") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ãƒãƒ£ãƒ¼ã‚¸") > 0 Then
			Select Case WeaponInHand
				Case ""
					'Invalid_string_refer_to_original_code
				Case Else
					wtype = WeaponInHand & "çªæ’ƒ"
					GoTo FoundWeaponType
			End Select
		End If
		
		'æ‰“æ’ƒç³»
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 5) = "ã‚¹ãƒˆãƒ©ã‚¤ã‚¯" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'é€šå¸¸æ‰“æ’ƒ
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ“ãƒ³ã‚¿") > 0 _
		'Or InStr(wname, "æ®´") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒˆãƒ³ãƒ•ã‚¡ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¢ãƒ¼ãƒ‹ãƒ³ã‚°ã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "ãƒ•ãƒ¬ã‚¤ãƒ«") > 0 _
		'Or InStr(wname, "ãƒŒãƒ³ãƒãƒ£ã‚¯") > 0 Or InStr(wname, "ä¸‰ç¯€æ ¹") > 0 _
		'Or (InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 And InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ã‚½ãƒ¼") = 0) _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç«¹åˆ€") > 0 _
		'Or InStr(wname, "ãƒãƒªã‚»ãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "æ‰“æ’ƒ"
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç«¹åˆ€") > 0 _
		'Or InStr(wname, "ãƒãƒªã‚»ãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Whip.wav"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ“ãƒ³ã‚¿") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Slap.wav"
		'End If
		
		GoTo FoundWeaponType
		'End If
		
		'å¼·æ‰“æ’ƒ
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¢ãƒ¼ãƒ«") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Bazooka.wav")
		'End If
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒãƒ³ã‚«ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Crash.wav"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒãƒ³ã‚«ãƒ¼") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Crash.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "æ‰“æ’ƒ"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 _
		'Or InStr(wname, "ãƒ–ãƒ©ã‚¹ã‚¿ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ã‚°ãƒªãƒ¼ãƒ³"
		'UPGRADE_WARNING: HitAnimation ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ãƒ–ãƒ«ãƒ¼"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ã‚¤ã‚¨ãƒ­ãƒ¼"
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å‰£") > 0 _
		'Or InStr(wname, "åˆ€") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		wtype = "Invalid_string_refer_to_original_code"
		'End If
		
		If double_weapon Then
			wtype = "ãƒ€ãƒ–ãƒ«" & wtype
		ElseIf InStr(wname, "å›è»¢") > 0 Then 
			wtype = "å›è»¢" & wtype
		End If
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ€ã‚¬ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒŠã‚®ãƒŠã‚¿") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚·ãƒŸã‚¿ãƒ¼") > 0 Or InStr(wname, "ã‚µãƒ¼ãƒ™ãƒ«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "åˆ€") > 0 Or InStr(wname, "æ–¬") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_weapon Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "ç«") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "é›·") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "å†·") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "å”ç«¹å‰²") > 0 Or InStr(wname, "ç¸¦") > 0 Then 
			wtype = "å”ç«¹å‰²"
		ElseIf InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "æ¨ª") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "æ–¬") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "æ–¬ã‚Šä¸Šã’"
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "æ­»") > 0 _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'åˆºçªç³»
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ©ãƒ³ã‚¹") > 0 Or InStr(wname, "ãƒ©ãƒ³ã‚µãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¸ãƒ£ãƒ™ãƒªãƒ³") > 0 _
		'Or InStr(wname, "ãƒ¬ã‚¤ãƒ”ã‚¢") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "çˆª") > 0 Or InStr(wname, "ã‚¯ãƒ­ãƒ¼") > 0 Or InStr(wname, "ã²ã£ã‹ã") > 0 Then
			If InStr(wname, "ã‚¢ãƒ¼ãƒ ") > 0 Then
				wtype = "æ‰“æ’ƒ"
				sname = "Crash.wav"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å™›ã¿ä»˜ã"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒ‰ãƒªãƒ«") > 0 Then
			wtype = "ãƒ‰ãƒªãƒ«"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒªãƒœãƒ³") > 0 Then
			wtype = "ãƒªãƒœãƒ³"
			GoTo FoundWeaponType
		End If
		
		'æ´ã¿ç³»
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "æŠ•ã’") > 0 Or wname = "è¿”ã—" Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒ–ãƒªãƒ¼ã‚«ãƒ¼") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æŠ˜ã‚Š") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¸ãƒ£ã‚¤ã‚¢ãƒ³ãƒˆã‚¹ã‚¤ãƒ³ã‚°") > 0 Then
			wtype = "ã‚¸ãƒ£ã‚¤ã‚¢ãƒ³ãƒˆã‚¹ã‚¤ãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒ–ãƒ¬ãƒ¼ãƒ³ãƒã‚¹ã‚¿ãƒ¼") > 0 Then
			wtype = "ãƒ–ãƒ¬ãƒ¼ãƒ³ãƒã‚¹ã‚¿ãƒ¼"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "æ­¦") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "æ­¦å™¨") _
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype = "" Then
						wtype = CheckWeaponType(.Class0, "")
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		Select Case wtype
			Case "ã‚¹ãƒ”ã‚¢", "ãƒ©ãƒ³ã‚¹", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'"ã‚¨ã‚¹ãƒˆãƒƒã‚¯"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
			Case Else
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_weapon Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "ç«") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "é›·") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "å†·") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
					wtype = "æ–¬ã‚Šä¸Šã’"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
		End Select
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "æ¥") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "æ‰“æ’ƒ"
		End If
		GoTo FoundWeaponType
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "æ–§") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ã‚½ãƒ¼ã‚µãƒ¼") > 0 Or InStr(wname, "ãƒãƒ£ã‚¯ãƒ©ãƒ ") > 0 Then
			wtype = "ãƒ€ãƒ¡ãƒ¼ã‚¸"
			sname = "Saber.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "çŸ³") > 0 Or InStr(wname, "ç¤«") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "æ‰“æ’ƒ"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æ‰‹è£å‰£") > 0 Or InStr(wname, "ã‚¯ãƒŠã‚¤") > 0 _
		'Or InStr(wname, "è‹¦ç„¡") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			GoTo SkipNormalWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ­ãƒ³ã‚°ãƒœã‚¦") > 0 _
		'Or InStr(wname, "ãƒœã‚¦ã‚¬ãƒ³") > 0 _
		'Or InStr(wname, "çŸ¢") > 0 Or InStr(wname, "ã‚¢ãƒ­ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "çŸ¢"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 Then
			wtype = "ãƒãƒ«ã‚«ãƒ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ã‚¬ãƒˆãƒªãƒ³ã‚°") > 0 Or InStr(wname, "ãƒã‚§ãƒ¼ãƒ³ã‚¬ãƒ³") Or InStr(wname, "ã‚¬ãƒ³ãƒ©ãƒ³ãƒãƒ£ãƒ¼") Then
			wtype = "ã‚¬ãƒˆãƒªãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ãƒ˜ãƒ“ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
		wtype = "ãƒã‚·ãƒ³ã‚¬ãƒ³"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "æ©ŸéŠƒ") > 0 Or InStr(wname, "æ©Ÿé–¢ç ²") > 0 Then
			wtype = "ãƒã‚·ãƒ³ã‚¬ãƒ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒã‚·ãƒ³ã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "ã‚ªãƒ¼ãƒˆã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "ãƒ˜ãƒ“ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "æ•£å¼¾") > 0 Or InStr(wname, "æ‹¡æ•£ãƒã‚ºãƒ¼ã‚«") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ãƒ™ã‚¢ãƒªãƒ³ã‚°") > 0 Or InStr(wname, "ã‚¯ãƒ¬ã‚¤ãƒ¢ã‚¢") > 0 Then
			wtype = "ãƒ™ã‚¢ãƒªãƒ³ã‚°"
			GoTo FoundWeaponType
		End If
		
SkipNormalWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			If InStr(CurrentWeaponType, "Invalid_string_refer_to_original_code") > 0 Or InStr(CurrentWeaponType, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 Then
				'Invalid_string_refer_to_original_code
				Select Case CurrentWeaponType
					Case "Invalid_string_refer_to_original_code"
						wtype = "å°ãƒ“ãƒ¼ãƒ "
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "ãƒ¬ãƒ¼ã‚¶ãƒ¼ã‚¬ãƒ³"
						wtype = "Invalid_string_refer_to_original_code"
					Case "ãƒ¬ãƒ¼ã‚¶ãƒ¼ãƒã‚·ãƒ³ã‚¬ãƒ³"
						wtype = "Invalid_string_refer_to_original_code"
					Case Else
						wtype = CurrentWeaponType
				End Select
			Else
				If InStr(wname, "ãƒã‚¤ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "å¤§") > 0 Or Left(wname, 2) = "ã‚®ã‚¬" Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "ãƒ¡ã‚¬") > 0 Or InStr(wname, "ãƒã‚¤") > 0 Or InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "å¯¾ç©º") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "ãƒ©ãƒ³ãƒãƒ£ãƒ¼") > 0 Or InStr(wname, "ã‚­ãƒ£ãƒãƒ³") > 0 Or InStr(wname, "ã‚«ãƒãƒ³") > 0 Or InStr(wname, "ç ²") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "å°ãƒ“ãƒ¼ãƒ "
				End If
				
				Select Case wtype
					Case "å°ãƒ“ãƒ¼ãƒ ", "Invalid_string_refer_to_original_code"
						If double_weapon Then
							wtype = "Invalid_string_refer_to_original_code" & wtype
						End If
				End Select
				
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				wtype = "Invalid_string_refer_to_original_code"
			End If
			
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		End If
		'End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ©ã‚¤ãƒ•ãƒ«") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or CurrentWeaponType = "Invalid_string_refer_to_original_code" Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "åå¿œå¼¾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚¤ãƒ³") > 0 Or InStr(wname, "ãƒœãƒ ") > 0 _
		'Or InStr(wname, "é­šé›·") > 0 Or InStr(wname, "æ©Ÿé›·") > 0 _
		'Or InStr(wname, "ãƒã‚ºãƒ¼ã‚«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚¤ã‚¯ãƒ­") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		wtype = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			GoTo FoundWeaponType
		End If
		
		attack_times = CountAttack0(u, w)
		If InStrNotNest(wclass, "é€£") > 0 Then
			attack_times = hit_count
		End If
		
		If attack_times = 1 Then
			attack_times = 0
			GoTo FoundWeaponType
		End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "é›·") > 0 Or InStr(wname, "ãƒ©ã‚¤ãƒˆãƒ‹ãƒ³ã‚°") > 0 Or InStr(wname, "ã‚µãƒ³ãƒ€ãƒ¼") > 0 Or Right(wname, 2) = "ç¨²å¦»" Or InStrNotNest(wclass, "é›»") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "æ”¾é›»"
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¹é›ª"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "å¼·é¢¨"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é¢¨") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "é¢¨"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç«œå·»") > 0 Or InStr(wname, "æ¸¦å·»") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "ç«œå·»"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "æ³¡") > 0 Or InStr(wname, "ãƒãƒ–ãƒ«") > 0 Or InStr(wname, "æ¶ˆç«") > 0 Then
			wtype = "æ³¡"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç¸®é€€") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "é‡åŠ›åœ§ç¸®"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ã‚¹ãƒ­ã‚¦") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ã‚¬ã‚¹" Or Right$(wname, 1) = "éœ§" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "Invalid_string_refer_to_original_code"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ç«ç‚å¼¾") > 0 Then
			wtype = "ç«ç‚å¼¾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If Right(wname, 5) = "ãƒ•ã‚¡ã‚¤ã‚¢ãƒ¼" Or Right(wname, 5) = "ãƒ•ã‚¡ã‚¤ãƒ¤ãƒ¼" Or Right(wname, 4) = "ãƒ•ã‚¡ã‚¤ã‚¢" Or Right(wname, 4) = "ãƒ•ã‚¡ã‚¤ãƒ¤" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		'End If
		
		If InStr(wname, "æ¯") > 0 Or Right(wname, 3) = "ãƒ–ãƒ¬ã‚¹" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			wtype = "Invalid_string_refer_to_original_code"
			
			Select Case SpellColor(wname, wclass)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					cname = SpellColor(wname, wclass)
					sname = "Breath.wav"
			End Select
			
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 1) = "æ¶²" Or Right$(wname, 1) = "é…¸" _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "é£›æ²«"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "Invalid_string_refer_to_original_code"
		'UPGRADE_WARNING: HitAnimation ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ç™½"
		cname = "Invalid_string_refer_to_original_code"
		'End If
		sname = "Splash.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If u.WeaponPower(w, "") = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		wtype = "ãƒ€ãƒ¡ãƒ¼ã‚¸"
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		Select Case wtype
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If InStrNotNest(wclass, "å¹") > 0 Or InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
					wtype = "æ‰“æ’ƒ"
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "èµ¤") > 0 Then
			cname = "èµ¤"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "ç™½"
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(wtype0) > 0 Then
			'Invalid_string_refer_to_original_code
			aname = wtype0 & "å‘½ä¸­"
			
			'è‰²
			If Len(cname) > 0 Then
				aname = aname & " " & cname
			End If
			
			'å‘½ä¸­ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
			ShowAnimation(aname)
		End If
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "å‘½ä¸­"
		
		'è‰²
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'åŠ¹æœéŸ³
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'å‘½ä¸­æ•°
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'å‘½ä¸­ã‚¢ãƒ‹ãƒ¡è¡¨ç¤º
		ShowAnimation(aname)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub HitSound(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wname, wclass As String
		Dim num, i As Short
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'åŠ¹æœéŸ³ã®å†ç”Ÿå›æ•°
		num = CountAttack(u, w)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "æ¥") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		Sleep(200)
		PlayWave("Sword.wav")
		For i = 2 To num
			Sleep(200)
			PlayWave("Sword.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ‰ãƒªãƒ«") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Drill.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚¹ã‚¿ãƒ¼") > 0 Or InStr(wname, "ãƒ–ãƒ©ã‚¹ã‚¿ãƒ¼") > 0 _
		'Or InStr(wname, "ã‚¯ãƒ­ãƒ¼") > 0 Or InStr(wname, "ã‚¸ã‚¶ãƒ¼ã‚¹") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or (InStr(wname, "å‰£") > 0 And InStr(wname, "æ‰‹è£å‰£") = 0) _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç¾½") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("Slash.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Slash.wav")
		Next 
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æ‰‹è£å‰£") > 0 _
		'Or InStr(wname, "è‹¦ç„¡") > 0 Or InStr(wname, "ã‚¯ãƒŠã‚¤") > 0 _
		'Or (InStr(wname, "çªã") > 0 _
		'And InStr(wname, "æ‹³") = 0 And InStr(wname, "é ­") = 0) _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ¬ãƒ¼ã‚¶ãƒ¼") > 0 _
		'Or InStr(wname, "ãƒ©ãƒ³ã‚µãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("Stab.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Stab.wav")
		Next 
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If Not t.IsHero Then
			PlayWave("Saber.wav")
			For i = 2 To num
				Sleep(350)
				PlayWave("Saber.wav")
			Next 
		Else
			PlayWave("Stab.wav")
			For i = 2 To num
				Sleep(350)
				PlayWave("Stab.wav")
			Next 
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æŒ¯å‹•æ‹³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Combo.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒŠãƒƒã‚¯ãƒ«") > 0 Or InStr(wname, "ãƒ–ãƒ­ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "é ­çªã") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "è¹´") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "çŸ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å°»å°¾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Punch.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Punch.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "çªæ’ƒ") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "è»Šè¼ª") > 0 _
		'Or InStr(wname, "ã‚­ãƒ£ã‚¿ãƒ”ãƒ©") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Crash.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Bazooka.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Bazooka.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Shock(Low).wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Slap.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Slap.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "çŸ¢") > 0 _
		'Or InStr(wname, "ã‚¢ãƒ­ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒœã‚¦ã‚¬ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ­ãƒ³ã‚°ãƒœã‚¦") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Stab.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Stab.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å°¾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç³¸") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Whip.wav")
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		Sleep(500)
		PlayWave("Shock(Low).wav")
		For i = 2 To num
			Sleep(700)
			PlayWave("Swing.wav")
			Sleep(500)
			PlayWave("Shock(Low).wav")
		Next 
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		Sleep(700)
		PlayWave("Swing.wav")
		Sleep(500)
		PlayWave("Swing.wav")
		Sleep(300)
		PlayWave("Shock(Low).wav")
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "æŠ˜ã‚Š") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "çµã‚") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("BreakOff.wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Explode(Nuclear).wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Explode(Small).wav")
		For i = 2 To num
			Sleep(130)
			PlayWave("Explode(Small).wav")
		Next 
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'ç„¡éŸ³
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		PlayWave("Punch.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Punch.wav")
		Next 
		If Not t.IsHero Then
			PlayWave("Explode(Small).wav")
			For i = 2 To num
				Sleep(130)
				PlayWave("Explode(Small).wav")
			Next 
		End If
		'End If
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒãƒªã‚±ãƒ¼ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚µã‚¤ã‚¯ãƒ­ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç«œå·»") > 0 _
		'Or InStr(wname, "æ¸¦å·»") > 0 _
		'Or InStr(wname, "å°é¢¨") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Inori.wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Fire.wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Crash.wav")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Charge.wav")
		'UPGRADE_WARNING: HitSound ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Explode(Nuclear).wav")
		If Not t.IsHero Then
			PlayWave("Explode(Small).wav")
			For i = 2 To num
				Sleep(130)
				PlayWave("Explode(Small).wav")
			Next 
		End If
		'End If
		'End If
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DodgeEffect(ByRef u As Unit, ByRef w As Short)
		Dim wname, wclass As String
		Dim sname As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If u.IsSpecialEffectDefined(wname & "(å›é¿)") Then
			u.SpecialEffect(wname & "(å›é¿)")
			Exit Sub
		End If
		
		If BattleAnimation Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		sname = u.SpecialEffectData(wname)
		If InStr(sname, ";") > 0 Then
			sname = Mid(sname, InStr(sname, ";"))
		End If
		If sname = "Swing.wav" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "æ¥") _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚§ãƒ¼ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "å°¾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ç³¸") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Swing.wav")
		'End If
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ParryEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		num = CountAttack(u, w)
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¢ã‚µãƒ«ãƒˆãƒ©ã‚¤ãƒ•ãƒ«") > 0 _
		'Or InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		num = 4
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ©ãƒ³ã‚µãƒ¼") > 0 Or InStr(wname, "ãƒ€ã‚¬ãƒ¼") > 0 _
		'Or InStr(wname, "å‰£") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Sword.wav"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		sname = "Explode(Small).wav"
		InStrNotNest(wclass, "Invalid_string_refer_to_original_code")
		sname = "BeamCoat.wav"
		sname = "Explode(Small).wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		PlayWave("Saber.wav")
		Sleep(100)
		PlayWave(sname)
		For i = 2 To num
			Sleep(130)
			PlayWave("Saber.wav")
			Sleep(100)
			PlayWave(sname)
		Next 
		
		'ãƒ•ãƒ©ã‚°ã‚’ã‚¯ãƒªã‚¢
		IsWavePlayed = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ShieldEffect(ByRef u As Unit)
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ShowAnimation("Invalid_string_refer_to_original_code")
		Exit Sub
		'End If
		
		'Invalid_string_refer_to_original_code
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			ShowAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			ShowAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			ShowAnimation("Invalid_string_refer_to_original_code")
			ShowAnimation("Invalid_string_refer_to_original_code")
			'End If
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AbsorbEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wclass, wname, cname As String
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		PlayWave("Charge.wav")
		Exit Sub
		'End If
		
		With u.Weapon(w)
			wname = .Nickname
			wclass = .Class_Renamed
		End With
		
		'Invalid_string_refer_to_original_code
		cname = SpellColor(wname, wclass)
		If cname = "" Then
			IsBeamWeapon(wname, wclass, cname)
		End If
		
		'ã‚¢ãƒ‹ãƒ¡ã‚’è¡¨ç¤º
		ShowAnimation("Invalid_string_refer_to_original_code" & cname)
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: ctype ‚Í ctype_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Sub CriticalEffect(ByRef ctype_Renamed As String, ByVal w As Short, ByVal ignore_death As Boolean)
		Dim aname, sname As String
		Dim i As Short
		
		If Len(ctype_Renamed) = 0 Then
			ShowAnimation("Invalid_string_refer_to_original_code")
		Else
			For i = 1 To LLength(ctype_Renamed)
				aname = LIndex(ctype_Renamed, i) & "Invalid_string_refer_to_original_code"
				
				If aname = "Invalid_string_refer_to_original_code" And ignore_death Then
					GoTo NextLoop
				End If
				
				If FindNormalLabel("æˆ¦é—˜ã‚¢ãƒ‹ãƒ¡_" & aname) = 0 Then
					GoTo NextLoop
				End If
				
				sname = ""
				
				If aname = "Invalid_string_refer_to_original_code" Then
					If SelectedUnit.IsWeaponClassifiedAs(w, "å†·") Then
						'Invalid_string_refer_to_original_code
						sname = "-.wav"
					End If
				End If
				
				If sname <> "" Then
					ShowAnimation(aname & " " & sname)
				Else
					ShowAnimation(aname)
				End If
NextLoop: 
			Next 
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Private Function CountAttack(ByRef u As Unit, ByVal w As Short, Optional ByVal hit_count As Short = 0) As Short
		'Invalid_string_refer_to_original_code
		If MessageWait <= 200 Then
			CountAttack = 1
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If hit_count > 0 And InStr(u.Weapon(w).Class_Renamed, "é€£") > 0 Then
			CountAttack = hit_count
			Exit Function
		End If
		
		CountAttack = MinLng(CountAttack0(u, w), 4)
	End Function
	
	Private Function CountAttack0(ByRef u As Unit, ByVal w As Short) As Short
		Dim wname, wclass As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "é€£") > 0 Then
			CountAttack0 = u.WeaponLevel(w, "é€£")
			Exit Function
		End If
		
		If InStr(wname, "é€£") > 0 Then
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "äºŒåé€£") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "åå››é€£") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "åäºŒé€£") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ä¸€é€£") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ä¹é€£") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "å…«é€£") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "å…­é€£") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "äº”é€£") > 0 Then
				CountAttack0 = 4
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "å››é€£") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ä¸‰é€£") > 0 Then
				CountAttack0 = 3
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "äºŒé€£") > 0 Then
				CountAttack0 = 2
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "å¤šé€£") > 0 _
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			CountAttack0 = 3
			Exit Function
		End If
		
		CountAttack0 = 2
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ•ãƒ«ãƒ•ã‚¡ã‚¤ã‚¢") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ‘ãƒ©ãƒ¬ãƒ«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ•ã‚¡ãƒ³ãƒãƒ«") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CountAttack0 = 4
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒãƒ«ã‚«ãƒ³") > 0 _
		'Or InStr(wname, "ã‚¬ãƒˆãƒªãƒ³ã‚°") > 0 _
		'Or (InStr(wname, "ãƒ‘ãƒ«ã‚¹") > 0 And InStr(wname, "ã‚¤ãƒ³ãƒ‘ãƒ«ã‚¹") = 0) _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒŸã‚µã‚¤ãƒ«ãƒ©ãƒ³ãƒãƒ£ãƒ¼") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CountAttack0 = 4
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CountAttack0 = 3
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		CountAttack0 = 2
		Exit Function
		'End If
		
		CountAttack0 = 1
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function IsBeamWeapon(ByRef wname As String, ByVal wclass As String, ByRef cname As String) As Boolean
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			IsBeamWeapon = True
		Else
			If Right(wname, 2) = "ã‚¬ã‚¹" Then
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ–ãƒ©ã‚¹ã‚¿ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ã‚¤ã‚¨ãƒ­ãƒ¼"
		cname = "ãƒ”ãƒ³ã‚¯"
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ•ãƒªãƒ¼ã‚¶ãƒ¼") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "ãƒ–ãƒ«ãƒ¼"
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒ‹ãƒ¥ãƒ¼ãƒˆãƒ­ãƒ³") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "ã‚°ãƒªãƒ¼ãƒ³"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "ã‚ªãƒ¬ãƒ³ã‚¸"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "ã‚¤ã‚¨ãƒ­ãƒ¼"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		IsBeamWeapon = True
		cname = "Invalid_string_refer_to_original_code"
		'End If
		
		If cname = "" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			cname = "ã‚¤ã‚¨ãƒ­ãƒ¼"
		Else
			cname = "ãƒ”ãƒ³ã‚¯"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		cname = "ãƒ–ãƒ«ãƒ¼"
		'End If
		'End If
		
		If Not IsBeamWeapon And cname <> "" Then
			'Invalid_string_refer_to_original_code_
			'Or Right$(wname, 1) = "ç ²" _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			IsBeamWeapon = True
		End If
		'End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function SpellColor(ByRef wname As String, ByVal wclass As String) As String
		Dim sclass As String
		Dim i As Short
		
		sclass = wname & wclass
		
		'Invalid_string_refer_to_original_code
		For i = 1 To Len(sclass)
			Select Case Mid(sclass, i, 1)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "èµ¤"
					Exit Function
				Case "æ°´", "æµ·", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "é¢¨", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "é‚ª", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					SpellColor = "ç™½"
					Exit Function
				Case "æ—¥", "é™½"
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
			End Select
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "èµ¤"
		Exit Function
		'End If
		
		If InStr(wname, "ã‚¦ã‚©ãƒ¼ã‚¿ãƒ¼") > 0 Or InStr(wname, "ã‚¢ã‚¯ã‚¢") > 0 Then
			SpellColor = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ãƒã‚¤ã‚ºãƒ³") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚·ãƒ£ãƒ‰ã‚¦") > 0 _
		'Or InStr(wname, "ã‚«ãƒ¼ã‚¹") > 0 _
		'Or InStr(wname, "ã‚«ãƒ¼ã‚º") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ã‚¢ã‚¤ã‚¹") > 0 _
		'Or InStr(wname, "ãƒ•ãƒªãƒ¼ã‚º") > 0 _
		'Then
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		SpellColor = "ç™½"
		Exit Function
		'End If
		
		If InStr(wname, "ã‚µãƒ³") Then
			SpellColor = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
	End Function
	
	
	'ç ´å£Šã‚¢ãƒ‹ãƒ¡ãƒ¼ã‚·ãƒ§ãƒ³ã‚’è¡¨ç¤ºã™ã‚‹
	Public Sub DieAnimation(ByRef u As Unit)
		Dim i As Short
		Dim PT As POINTAPI
		Dim fname, draw_mode As String
		
		With u
			EraseUnitBitmap(.X, .Y)
			
			'Invalid_string_refer_to_original_code
			If Not .IsHero Then
				ExplodeAnimation(.Size, .X, .Y)
				Exit Sub
			End If
			
			GetCursorPos(PT)
			
			'Invalid_string_refer_to_original_code
			If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
				With frmMessage
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'Invalid_string_refer_to_original_code
							Exit Sub
						End If
					End If
				End With
			End If
			
			'Invalid_string_refer_to_original_code
			If System.Windows.Forms.Form.ActiveForm Is MainForm Then
				With MainForm
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'Invalid_string_refer_to_original_code
							Exit Sub
						End If
					End If
				End With
			End If
			
			'å€’ã‚Œã‚‹éŸ³
			Select Case .Area
				Case "Invalid_string_refer_to_original_code"
					PlayWave("FallDown.wav")
				Case "ç©ºä¸­"
					If MessageWait > 0 Then
						PlayWave("Bomb.wav")
						Sleep(500)
					End If
					If TerrainClass(.X, .Y) = "æ°´" Or TerrainClass(.X, .Y) = "æ·±æµ·" Then
						PlayWave("Splash.wav")
					Else
						PlayWave("FallDown.wav")
					End If
			End Select
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If MessageWait = 0 Then
				Exit Sub
			End If
			
			Select Case .Party0
				Case "å‘³æ–¹", "Invalid_string_refer_to_original_code"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Ally)"
				Case "æ•µ"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Enemy)"
				Case "Invalid_string_refer_to_original_code"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Neutral)"
			End Select
			If FileExists(ScenarioPath & fname & ".bmp") Then
				fname = ScenarioPath & fname
			Else
				fname = AppPath & fname
			End If
			If Not FileExists(fname & "01.bmp") Then
				Exit Sub
			End If
			
			Select Case MapDrawMode
				Case "Invalid_string_refer_to_original_code"
					draw_mode = "Invalid_string_refer_to_original_code"
				Case Else
					draw_mode = MapDrawMode
			End Select
			
			For i = 1 To 6
				DrawPicture(fname & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, draw_mode)
				DrawPicture("Unit\" & .Bitmap, MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "é€é " & draw_mode)
				DrawPicture(fname & "0" & VB6.Format(i) & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "é€é " & draw_mode)
				'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				MainForm.picMain(0).Refresh()
				Sleep(50)
			Next 
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			MainForm.picMain(0).Refresh()
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ExplodeAnimation(ByRef tsize As String, ByVal tx As Short, ByVal ty As Short)
		Dim i As Short
		Dim PT As POINTAPI
		Static init_explode_animation As Boolean
		Static explode_image_path As String
		Static explode_image_num As Short
		
		'Invalid_string_refer_to_original_code
		If Not init_explode_animation Then
			'Invalid_string_refer_to_original_code
			If FileExists(ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then
				explode_image_path = ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			ElseIf FileExists(ScenarioPath & "Bitmap\Event\Explode01.bmp") Then 
				explode_image_path = ScenarioPath & "Bitmap\Event\Explode"
			ElseIf FileExists(AppPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then 
				explode_image_path = AppPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			Else
				explode_image_path = AppPath & "Bitmap\Event\Explode"
			End If
			
			'Invalid_string_refer_to_original_code
			i = 2
			Do While FileExists(explode_image_path & VB6.Format(i, "00") & ".bmp")
				i = i + 1
			Loop 
			explode_image_num = i - 1
		End If
		
		GetCursorPos(PT)
		
		'Invalid_string_refer_to_original_code
		If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
			With frmMessage
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						Exit Sub
					End If
				End If
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		If System.Windows.Forms.Form.ActiveForm Is MainForm Then
			With MainForm
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						Exit Sub
					End If
				End If
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case tsize
			Case "XL", "LL"
				PlayWave("Explode(Far).wav")
			Case "L", "M", "S", "SS"
				PlayWave("Explode.wav")
		End Select
		
		'Invalid_string_refer_to_original_code
		If MessageWait = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If InStr(explode_image_path, "\Anime\") > 0 Then
			'Invalid_string_refer_to_original_code
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 56, MapToPixelY(ty) - 56, 144, 144, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 40, MapToPixelY(ty) - 40, 112, 112, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 24, MapToPixelY(ty) - 24, 80, 80, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			MainForm.picMain(0).Refresh()
		Else
			'æ±ç”¨ã‚¤ãƒ™ãƒ³ãƒˆç”»åƒç‰ˆã®ç”»åƒã‚’ä½¿ç”¨
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 32, MapToPixelY(ty) - 32, 96, 96, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 16, MapToPixelY(ty) - 16, 64, 64, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx), MapToPixelY(ty), 32, 32, 0, 0, 0, 0, "é€é")
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			MainForm.picMain(0).Refresh()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub NegateEffect(ByRef u As Unit, ByRef t As Unit, ByVal w As Short, ByRef wname As String, ByVal dmg As Integer, ByRef fname As String, ByRef fdata As String, ByVal ecost As Short, ByRef msg As String, ByVal be_quiet As Boolean)
		Dim defined As Boolean
		
		If LIndex(fdata, 1) = "Invalid_string_refer_to_original_code" Or LIndex(fdata, 2) = "Invalid_string_refer_to_original_code" Or LIndex(fdata, 3) = "Invalid_string_refer_to_original_code" Then
			If Not be_quiet Then
				If t.IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then
					t.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
				Else
					t.PilotMessage("Invalid_string_refer_to_original_code")
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			t.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			t.SpecialEffect("Invalid_string_refer_to_original_code")
		ElseIf dmg < 0 Then 
			AbsorbEffect(u, w, t)
		ElseIf BattleAnimation Then 
			ShowAnimation("Invalid_string_refer_to_original_code" & fname)
		ElseIf Not IsWavePlayed Then 
			PlayWave("BeamCoat.wav")
		End If
		
		If u.IsAnimationDefined(wname & "Invalid_string_refer_to_original_code") Then
			u.PlayAnimation(wname & "Invalid_string_refer_to_original_code")
		ElseIf u.IsSpecialEffectDefined(wname & "Invalid_string_refer_to_original_code") Then 
			u.SpecialEffect(wname & "Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		t.SysMessage("Invalid_string_refer_to_original_code")
		'UPGRADE_WARNING: NegateEffect ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		End If
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "ã®[" & fname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "ã®[" & fname & "Invalid_string_refer_to_original_code")
		End If
		'End If
		If Not be_quiet Then
			If t.IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then
				t.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
			Else
				t.PilotMessage("Invalid_string_refer_to_original_code")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		t.PlayAnimation("Invalid_string_refer_to_original_code")
		defined = True
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		t.SpecialEffect("Invalid_string_refer_to_original_code")
		defined = True
		'UPGRADE_WARNING: NegateEffect ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		AbsorbEffect(u, w, t)
		defined = True
		'UPGRADE_WARNING: NegateEffect ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If InStr(fdata, "ãƒãƒªã‚¢ç„¡åŠ¹åŒ–ç„¡åŠ¹") = 0 Or ecost > 0 Then
			If fname = "ãƒãƒªã‚¢" Then
				ShowAnimation("Invalid_string_refer_to_original_code")
			ElseIf fname = "" Then 
				ShowAnimation("Invalid_string_refer_to_original_code")
			Else
				ShowAnimation("Invalid_string_refer_to_original_code" & fname)
			End If
			defined = True
		End If
		'End If
		
		If u.IsAnimationDefined(wname & "Invalid_string_refer_to_original_code") Then
			u.PlayAnimation(wname & "Invalid_string_refer_to_original_code")
			defined = True
		ElseIf u.IsSpecialEffectDefined(wname & "Invalid_string_refer_to_original_code") Then 
			u.SpecialEffect(wname & "Invalid_string_refer_to_original_code")
			defined = True
		End If
		
		If Not defined Then
			HitEffect(u, w, t)
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		t.SysMessage("Invalid_string_refer_to_original_code")
		'UPGRADE_WARNING: NegateEffect ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		End If
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "ã®[" & fname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "ã®[" & fname & "Invalid_string_refer_to_original_code")
		End If
		'End If
		'End If
	End Sub
End Module