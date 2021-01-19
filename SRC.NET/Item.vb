Option Strict Off
Option Explicit On
Friend Class Item
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public ID As String
	'Invalid_string_refer_to_original_code
	Public Data As ItemData
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Unit は Unit_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Unit_Renamed As Unit
	'Invalid_string_refer_to_original_code
	Public Exist As Boolean
	'Invalid_string_refer_to_original_code
	Public Activated As Boolean
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Exist = True
		Activated = True
		'UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Data = Nothing
		'UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Unit_Renamed = Nothing
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		'UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Data = Nothing
		'UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Unit_Renamed = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'蜷咲ｧｰ
	
	Public Property Name() As String
		Get
			Name = Data.Name
		End Get
		Set(ByVal Value As String)
			Data = IDList.Item(Value)
		End Set
	End Property
	
	'諢帷ｧｰ
	Public Function Nickname() As String
		Dim u As Unit
		
		Nickname = Data.Nickname
		
		'Invalid_string_refer_to_original_code
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(Nickname)
		SelectedUnitForEvent = u
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function KanaName() As String
		Dim u As Unit
		
		KanaName = Data.KanaName
		
		'Invalid_string_refer_to_original_code
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(KanaName)
		SelectedUnitForEvent = u
	End Function
	
	'繧ｯ繝ｩ繧ｹ
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Function Class_Renamed() As String
		Class_Renamed = Data.Class_Renamed
	End Function
	
	Public Function Class0() As String
		Dim i, n As Short
		
		Class0 = Data.Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If Right(Class0, 3) = "蟆ら畑)" Then
			n = 1
			i = Len(Class0) - 2
			Do 
				i = i - 1
				Select Case Mid(Class0, i, 1)
					Case "("
						n = n - 1
						If n = 0 Then
							Class0 = Left(Class0, i - 1)
							Exit Do
						End If
					Case ")"
						n = n + 1
				End Select
			Loop While i > 0
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Part() As String
		Part = Data.Part
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function HP() As Integer
		HP = Data.HP
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EN() As Short
		EN = Data.EN
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Armor() As Integer
		Armor = Data.Armor
	End Function
	
	'驕句虚諤ｧ菫ｮ豁｣蛟､
	Public Function Mobility() As Short
		Mobility = Data.Mobility
	End Function
	
	'遘ｻ蜍募鴨菫ｮ豁｣蛟､
	Public Function Speed() As Short
		Speed = Data.Speed
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function CountFeature() As Short
		CountFeature = Data.CountFeature
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Feature(ByRef Index As Object) As String
		Feature = Data.Feature(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureName(ByRef Index As Object) As String
		FeatureName = Data.FeatureName(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureLevel(ByRef Index As Object) As Double
		FeatureLevel = Data.FeatureLevel(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureData(ByRef Index As Object) As String
		FeatureData = Data.FeatureData(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureNecessarySkill(ByRef Index As Object) As String
		FeatureNecessarySkill = Data.FeatureNecessarySkill(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		IsFeatureAvailable = Data.IsFeatureAvailable(fname)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = Data.Weapon(Index)
	End Function
	
	'豁ｦ蝎ｨ縺ｮ邱乗焚
	Public Function CountWeapon() As Short
		CountWeapon = Data.CountWeapon
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = Data.Ability(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function CountAbility() As Short
		CountAbility = Data.CountAbility
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Size() As Short
		Size = Data.Size
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function IsAvailable(ByRef u As Unit) As Boolean
		Dim j, i, k As Short
		Dim iclass As String
		Dim sname, fdata As String
		
		IsAvailable = False
		
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined("Disable(" & Name & ")") Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case Part
			Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Exit Function
				'End If
			Case "閧ｩ", "荳｡閧ｩ"
				If InStr(u.FeatureData("Invalid_string_refer_to_original_code"), "閧ｩ") = 0 Then
					Exit Function
				End If
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Exit Function
				'End If
			Case "鬆ｭ"
				If InStr(u.FeatureData("Invalid_string_refer_to_original_code"), "鬆ｭ") = 0 Then
					Exit Function
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		Select Case Part
			Case "豁ｦ蝎ｨ", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				iclass = u.WeaponProficiency() & "Invalid_string_refer_to_original_code"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case "逶ｾ", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				iclass = u.ArmorProficiency() & "Invalid_string_refer_to_original_code"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case Else
				'Invalid_string_refer_to_original_code
				IsAvailable = True
		End Select
		
		If Not IsAvailable Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If Not IsFeatureAvailable("Invalid_string_refer_to_original_code") And Not IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
			Exit Function
		End If
		
		With u
			'Invalid_string_refer_to_original_code
			For i = 1 To CountFeature
				Select Case Feature(i)
					Case "Invalid_string_refer_to_original_code"
						If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'Invalid_string_refer_to_original_code
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'Invalid_string_refer_to_original_code
							If Right(sname, 2) = "Invalid_string_refer_to_original_code" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'Invalid_string_refer_to_original_code
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
									Case Else
										GoTo NextLoop1
								End Select
								
								'蟆∝魂縺吶ｋ閭ｽ蜉帛錐
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'Invalid_string_refer_to_original_code
								If fdata = sname Then
									GoTo NextLoop
								End If
								If .CountPilot > 0 Then
									If ALDList.IsDefined(fdata) Then
										With ALDList.Item(fdata)
											For k = 1 To .Count
												If .AliasType(k) = sname Then
													GoTo NextLoop
												End If
											Next 
										End With
									ElseIf .MainPilot.SkillType(fdata) = sname Then 
										GoTo NextLoop
									End If
								End If
NextLoop1: 
							Next 
							
							'Invalid_string_refer_to_original_code
							IsAvailable = False
							Exit Function
						End If
					Case "Invalid_string_refer_to_original_code"
						If .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'Invalid_string_refer_to_original_code
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'Invalid_string_refer_to_original_code
							If Right(sname, 2) = "Invalid_string_refer_to_original_code" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'Invalid_string_refer_to_original_code
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
									Case Else
										GoTo NextLoop2
								End Select
								
								'莉伜刈縺吶ｋ閭ｽ蜉帛錐
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'Invalid_string_refer_to_original_code
								If fdata = sname Then
									GoTo NextLoop
								End If
								If .CountPilot > 0 Then
									If ALDList.IsDefined(fdata) Then
										With ALDList.Item(fdata)
											For k = 1 To .Count
												If .AliasType(k) = sname Then
													GoTo NextLoop
												End If
											Next 
										End With
									ElseIf .MainPilot.SkillType(fdata) = sname Then 
										GoTo NextLoop
									End If
								End If
NextLoop2: 
							Next 
							
							'Invalid_string_refer_to_original_code
							IsAvailable = False
							Exit Function
						End If
				End Select
NextLoop: 
			Next 
		End With
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Dump()
		WriteLine(SaveDataFileNumber, Name, ID, Exist)
		If Unit_Renamed Is Nothing Then
			WriteLine(SaveDataFileNumber, "-")
		Else
			WriteLine(SaveDataFileNumber, Unit_Renamed.ID)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Restore()
		Dim sbuf As String
		Dim bbuf As Boolean
		
		'Name, ID, Exist
		Input(SaveDataFileNumber, sbuf)
		Name = sbuf
		Input(SaveDataFileNumber, sbuf)
		ID = sbuf
		Input(SaveDataFileNumber, bbuf)
		Exist = bbuf
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreLinkInfo()
		Dim sbuf As String
		
		'Name, ID, Exist
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		Input(SaveDataFileNumber, sbuf)
		If UList.IsDefined(sbuf) Then
			Unit_Renamed = UList.Item(sbuf)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreParameter()
		Dim sbuf As String
		
		'Name, ID, Exist
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
End Class