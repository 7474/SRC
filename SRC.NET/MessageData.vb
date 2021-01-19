Option Strict Off
Option Explicit On
Friend Class MessageData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Name As String
	
	'Invalid_string_refer_to_original_code
	Private intMessageNum As Short
	'繧ｷ繝√Η繧ｨ繝ｼ繧ｷ繝ｧ繝ｳ
	Private strSituation() As String
	'Invalid_string_refer_to_original_code
	Private strMessage() As String
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		intMessageNum = 0
		ReDim strSituation(0)
		ReDim strMessage(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AddMessage(ByRef sit As String, ByRef msg As String)
		intMessageNum = intMessageNum + 1
		ReDim Preserve strSituation(intMessageNum)
		ReDim Preserve strMessage(intMessageNum)
		strSituation(intMessageNum) = sit
		strMessage(intMessageNum) = msg
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function CountMessage() As Integer
		CountMessage = intMessageNum
	End Function
	
	'繧ｷ繝√Η繧ｨ繝ｼ繧ｷ繝ｧ繝ｳ
	Public Function Situation(ByVal idx As Integer) As String
		Situation = strSituation(idx)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Message(ByVal idx As Integer) As String
		Message = strMessage(idx)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SelectMessage(ByRef msg_situation As String, Optional ByRef u As Unit = Nothing) As String
		Dim situations() As String
		Dim sub_situations() As String
		Dim list0() As Short
		Dim list0_num As Short
		Dim tlist() As Short
		Dim tlist_num As Short
		Dim list() As Short
		Dim list_num As Short
		Dim j, i, k As Short
		Dim found As Boolean
		Dim t As Unit
		Dim w, tw As Short
		
		'Invalid_string_refer_to_original_code
		ReDim list0(300)
		ReDim tlist(100)
		ReDim list(200)
		
		'Invalid_string_refer_to_original_code
		Select Case msg_situation
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				ReDim situations(2)
				situations(2) = "Invalid_string_refer_to_original_code"
			Case Else
				ReDim situations(1)
		End Select
		situations(1) = msg_situation
		
		'Invalid_string_refer_to_original_code
		list0_num = 0
		For i = 1 To intMessageNum
			For j = 1 To UBound(situations)
				If Left(strSituation(i), Len(situations(j))) = situations(j) Then
					list0_num = list0_num + 1
					If list0_num > UBound(list0) Then
						ReDim Preserve list0(list0_num)
					End If
					list0(list0_num) = i
					Exit For
				End If
			Next 
		Next 
		If list0_num = 0 Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If u Is Nothing Then
			GoTo SkipMessagesWithTarget
		End If
		If u Is SelectedUnit Then
			t = SelectedTarget
		ElseIf u Is SelectedTarget Then 
			t = SelectedUnit
		End If
		If t Is Nothing Then
			GoTo SkipMessagesWithTarget
		End If
		
		'Invalid_string_refer_to_original_code
		tlist_num = 0
		For i = 1 To list0_num
			If InStr(strSituation(list0(i)), "(蟇ｾ") > 0 Then
				tlist_num = tlist_num + 1
				If tlist_num > UBound(tlist) Then
					ReDim Preserve tlist(tlist_num)
				End If
				tlist(tlist_num) = list0(i)
			End If
		Next 
		If tlist_num = 0 Then
			'Invalid_string_refer_to_original_code
			GoTo SkipMessagesWithTarget
		End If
		
		'Invalid_string_refer_to_original_code
		If t Is u Then
			list_num = 0
			For i = 1 To tlist_num
				For j = 1 To UBound(situations)
					If strSituation(tlist(i)) = situations(j) & "Invalid_string_refer_to_original_code" Then
						list_num = list_num + 1
						If list_num > UBound(list) Then
							ReDim Preserve list(list_num)
						End If
						list(list_num) = tlist(i)
						Exit For
					End If
				Next 
			Next 
			If list_num > 0 Then
				SelectMessage = strMessage(list(Dice(list_num)))
				Exit Function
			End If
		End If
		
		Dim wclass, ch As String
		With t
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			GoTo SkipMessagesWithTarget
			'End If
			
			ReDim sub_situations(8)
			'Invalid_string_refer_to_original_code
			sub_situations(1) = "(蟇ｾ" & .MainPilot.Name & ")"
			'Invalid_string_refer_to_original_code
			sub_situations(2) = "(蟇ｾ" & .MainPilot.Nickname & ")"
			'蟇ｾ繝ｦ繝九ャ繝亥錐遘ｰ
			sub_situations(3) = "(蟇ｾ" & .Name & ")"
			'Invalid_string_refer_to_original_code
			sub_situations(4) = "(蟇ｾ" & .Nickname & ")"
			'蟇ｾ繝ｦ繝九ャ繝医け繝ｩ繧ｹ
			sub_situations(5) = "(蟇ｾ" & .Class0 & ")"
			'蟇ｾ繝ｦ繝九ャ繝医し繧､繧ｺ
			sub_situations(6) = "(蟇ｾ" & .Size & ")"
			'Invalid_string_refer_to_original_code
			sub_situations(7) = "(蟇ｾ" & TerrainName(.X, .Y) & ")"
			'蟇ｾ繧ｨ繝ｪ繧｢
			sub_situations(8) = "(蟇ｾ" & .Area & ")"
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .FeatureData("Invalid_string_refer_to_original_code") & ")"
			End If
			
			'蟇ｾ諤ｧ蛻･
			Select Case .MainPilot.Sex
				Case "逕ｷ諤ｧ"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ逕ｷ諤ｧ)"
				Case "螂ｳ諤ｧ"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ螂ｳ諤ｧ)"
			End Select
			
			'Invalid_string_refer_to_original_code
			With .MainPilot
				For i = 1 To .CountSkill
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .SkillName0(i) & ")"
					If sub_situations(UBound(sub_situations)) = "(蟇ｾ髱櫁｡ｨ遉ｺ)" Then
						sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .Skill(i) & ")"
					End If
				Next 
			End With
			For i = 1 To .CountFeature
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .FeatureName0(i) & ")"
				If sub_situations(UBound(sub_situations)) = "(蟇ｾ)" Then
					sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .Feature(i) & ")"
				End If
			Next 
			
			'蟇ｾ蠑ｱ轤ｹ
			If Len(.strWeakness) > 0 Then
				For i = 1 To Len(.strWeakness)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ蠑ｱ轤ｹ=" & GetClassBundle(.strWeakness, i) & ")"
				Next 
			End If
			
			'蟇ｾ譛牙柑
			If Len(.strEffective) > 0 Then
				For i = 1 To Len(.strEffective)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ譛牙柑=" & GetClassBundle(.strEffective, i) & ")"
				Next 
			End If
			
			'蟇ｾ繧ｶ繧ｳ
			If InStr(.MainPilot.Name, "(繧ｶ繧ｳ)") > 0 And (u.MainPilot.Technique > .MainPilot.Technique Or u.HP > .HP \ 2) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(蟇ｾ繧ｶ繧ｳ)"
			End If
			
			'蟇ｾ蠑ｷ謨ｵ
			If .BossRank >= 0 Or (InStr(.MainPilot.Name, "(繧ｶ繧ｳ)") = 0 And u.MainPilot.Technique <= .MainPilot.Technique) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(蟇ｾ蠑ｷ謨ｵ)"
			End If
			
			'Invalid_string_refer_to_original_code
			w = 0
			If SelectedUnit Is u Then
				If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
					w = SelectedWeapon
				End If
			ElseIf SelectedTarget Is u Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= u.CountWeapon Then
					w = SelectedTWeapon
				End If
			End If
			
			If w > 0 Then
				'蟇ｾ轢墓ｭｻ
				If .HP <= u.Damage(w, t, u.Party = "蜻ｳ譁ｹ") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(蟇ｾ轢墓ｭｻ)"
				End If
				
				Select Case u.HitProbability(w, t, u.Party = "蜻ｳ譁ｹ")
					Case Is < 50
						'Invalid_string_refer_to_original_code
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "Invalid_string_refer_to_original_code"
					Case Is >= 100
						'Invalid_string_refer_to_original_code
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "Invalid_string_refer_to_original_code"
				End Select
			End If
			
			'Invalid_string_refer_to_original_code
			tw = 0
			If SelectedUnit Is t Then
				If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
					tw = SelectedWeapon
				End If
			ElseIf SelectedTarget Is t Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= .CountWeapon Then
					tw = SelectedTWeapon
				End If
			End If
			
			If tw > 0 Then
				'Invalid_string_refer_to_original_code
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(蟇ｾ" & .Weapon(tw).Name & ")"
				
				'蟇ｾ豁ｦ蝎ｨ螻樊ｧ
				wclass = .WeaponClass(tw)
				For i = 1 To Len(wclass)
					ch = GetClassBundle(wclass, i)
					Select Case ch
						Case CStr(0) To CStr(127)
						Case Else
							ReDim Preserve sub_situations(UBound(sub_situations) + 1)
							sub_situations(UBound(sub_situations)) = "(蟇ｾ" & ch & "螻樊ｧ)"
					End Select
				Next 
				
				Select Case .HitProbability(tw, u, .Party = "蜻ｳ譁ｹ")
					Case Is > 75
						'Invalid_string_refer_to_original_code
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "Invalid_string_refer_to_original_code"
					Case Is < 25
						'Invalid_string_refer_to_original_code
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "Invalid_string_refer_to_original_code"
				End Select
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		list_num = 0
		For i = 1 To tlist_num
			found = False
			For j = 1 To UBound(situations)
				For k = 1 To UBound(sub_situations)
					If strSituation(tlist(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = tlist(i)
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		If list_num > 0 Then
			SelectMessage = strMessage(list(Dice(list_num)))
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or msg_situation = "螽∝悸" _
			'Or u.Party = t.Party _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Exit Function
		End If
		'End If
		
SkipMessagesWithTarget: 
		
		'Invalid_string_refer_to_original_code
		If Not u Is Nothing Then
			ReDim sub_situations(3)
			With u
				sub_situations(1) = "(" & .Name & ")"
				sub_situations(2) = "(" & .Nickname0 & ")"
				sub_situations(3) = "(" & .Class0 & ")"
				Select Case msg_situation
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If SelectedUnit Is u Then
							'Invalid_string_refer_to_original_code
							If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
								ReDim Preserve sub_situations(4)
								sub_situations(4) = "(" & .WeaponNickname(SelectedWeapon) & ")"
							End If
						End If
				End Select
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(" & .FeatureData("Invalid_string_refer_to_original_code") & ")"
				End If
			End With
		Else
			ReDim sub_situations(0)
		End If
		
		'Invalid_string_refer_to_original_code
		list_num = 0
		For i = 1 To list0_num
			found = False
			For j = 1 To UBound(situations)
				If strSituation(list0(i)) = situations(j) Then
					found = True
					Exit For
				End If
				For k = 1 To UBound(sub_situations)
					If strSituation(list0(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = list0(i)
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		If list_num > 0 Then
			SelectMessage = strMessage(list(Dice(list_num)))
		End If
	End Function
End Class