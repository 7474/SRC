Option Strict Off
Option Explicit On
Friend Class PilotData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'蜷咲ｧｰ
	Public Name As String
	'諤ｧ蛻･
	Public Sex As String
	'繧ｯ繝ｩ繧ｹ
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Class_Renamed As String
	'Invalid_string_refer_to_original_code
	Public Adaption As String
	'邨碁ｨ灘､
	Public ExpValue As Short
	'Invalid_string_refer_to_original_code
	Public Infight As Short
	'Invalid_string_refer_to_original_code
	Public Shooting As Short
	'蜻ｽ荳ｭ
	Public Hit As Short
	'蝗樣∩
	Public Dodge As Short
	'Invalid_string_refer_to_original_code
	Public Intuition As Short
	'Invalid_string_refer_to_original_code
	Public Technique As Short
	'諤ｧ譬ｼ
	Public Personality As String
	'Invalid_string_refer_to_original_code
	Public SP As Short
	'Invalid_string_refer_to_original_code
	Public BGM As String
	
	'諢帷ｧｰ
	Private proNickname As String
	'Invalid_string_refer_to_original_code
	Private proKanaName As String
	
	'Invalid_string_refer_to_original_code
	Private proBitmap As String
	'Invalid_string_refer_to_original_code
	Public IsBitmapMissing As Boolean
	
	'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ
	Private SpecialPowerName() As String
	Private SpecialPowerNecessaryLevel() As Short
	Private SpecialPowerSPConsumption() As Short
	
	'Invalid_string_refer_to_original_code
	Public colSkill As New Collection
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public colFeature As Collection
	'Invalid_string_refer_to_original_code
	Private colWeaponData As Collection
	'Invalid_string_refer_to_original_code
	Private colAbilityData As Collection
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		ReDim SpecialPowerName(0)
		ReDim SpecialPowerNecessaryLevel(0)
		ReDim SpecialPowerSPConsumption(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colSkill をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colSkill = Nothing
		
		If Not colFeature Is Nothing Then
			With colFeature
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: オブジェクト colFeature をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			colFeature = Nothing
		End If
		
		If Not colWeaponData Is Nothing Then
			With colWeaponData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: オブジェクト colWeaponData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			colWeaponData = Nothing
		End If
		
		If Not colAbilityData Is Nothing Then
			With colAbilityData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: オブジェクト colAbilityData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			colAbilityData = Nothing
		End If
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'諢帷ｧｰ
	
	Public Property Nickname() As String
		Get
			Nickname = proNickname
			If InStr(Nickname, "荳ｻ莠ｺ蜈ｬ") = 1 Or InStr(Nickname, "繝偵Ο繧､繝ｳ") = 1 Then
				Nickname = GetValueAsString(Nickname & "諢帷ｧｰ")
			End If
			ReplaceSubExpression(Nickname)
		End Get
		Set(ByVal Value As String)
			proNickname = Value
		End Set
	End Property
	
	'Invalid_string_refer_to_original_code
	
	Public Property KanaName() As String
		Get
			KanaName = proKanaName
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			KanaName = StrToHiragana(GetValueAsString(KanaName & "諢帷ｧｰ"))
			'End If
			'End If
			ReplaceSubExpression(KanaName)
		End Get
		Set(ByVal Value As String)
			proKanaName = Value
		End Set
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property Bitmap0() As String
		Get
			Bitmap0 = proBitmap
		End Get
	End Property
	
	
	Public Property Bitmap() As String
		Get
			If IsBitmapMissing Then
				Bitmap = "-.bmp"
			Else
				Bitmap = proBitmap
			End If
		End Get
		Set(ByVal Value As String)
			proBitmap = Value
		End Set
	End Property
	
	
	'Invalid_string_refer_to_original_code
	Public Sub AddSkill(ByRef sname As String, ByVal slevel As Double, ByVal sdata As String, ByVal lv As Short)
		Dim sd As SkillData
		Dim i As Short
		Static last_sname, last_sdata As String
		
		'Invalid_string_refer_to_original_code
		If last_sname = sname And Len(sdata) = 0 Then
			sdata = last_sdata
		End If
		
		last_sname = sname
		last_sdata = sdata
		
		'Invalid_string_refer_to_original_code
		If ALDList.IsDefined(sname) Then
			If LIndex(sdata, 1) <> "隗｣隱ｬ" Then
				With ALDList.Item(sname)
					For i = 1 To .Count
						'Invalid_string_refer_to_original_code
						sd = New SkillData
						
						sd.Name = .AliasType(i)
						If LIndex(.AliasData(i), 1) = "隗｣隱ｬ" Then
							If sdata <> "" Then
								sd.Name = LIndex(sdata, 1)
							End If
						End If
						
						If .AliasLevelIsPlusMod(i) Then
							If slevel = DEFAULT_LEVEL Then
								slevel = 1
							End If
							sd.Level = slevel + .AliasLevel(i)
						ElseIf .AliasLevelIsMultMod(i) Then 
							If slevel = DEFAULT_LEVEL Then
								slevel = 1
							End If
							sd.Level = slevel * .AliasLevel(i)
						ElseIf slevel <> DEFAULT_LEVEL Then 
							sd.Level = slevel
						Else
							sd.Level = .AliasLevel(i)
						End If
						
						sd.StrData = .AliasData(i)
						If sdata <> "" Then
							If .AliasData(i) <> "髱櫁｡ｨ遉ｺ" And LIndex(.AliasData(i), 1) <> "隗｣隱ｬ" Then
								sd.StrData = Trim(sdata & " " & ListTail(.AliasData(i), 2))
							End If
						End If
						If .AliasLevelIsPlusMod(i) Or .AliasLevelIsMultMod(i) Then
							sd.StrData = sd.StrData & "Lv" & VB6.Format(slevel)
						End If
						
						sd.NecessaryLevel = lv
						
						colSkill.Add(sd, sname & VB6.Format(colSkill.Count()))
					Next 
				End With
				Exit Sub
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		sd = New SkillData
		With sd
			.Name = sname
			.Level = slevel
			.StrData = sdata
			.NecessaryLevel = lv
		End With
		colSkill.Add(sd, sname & VB6.Format(colSkill.Count()))
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Skill(ByVal lv As Short) As String
		Dim skill_num As Short
		Dim skill_name(32) As String
		Dim sd As SkillData
		Dim i As Short
		
		skill_num = 0
		For	Each sd In colSkill
			With sd
				If lv >= .NecessaryLevel Then
					For i = 1 To skill_num
						If .Name = skill_name(i) Then
							GoTo NextLoop
						End If
					Next 
					skill_num = skill_num + 1
					skill_name(skill_num) = .Name
				End If
			End With
NextLoop: 
		Next sd
		
		Skill = ""
		For i = 1 To skill_num
			Skill = Skill & skill_name(i) & " "
		Next 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsSkillAvailable(ByVal lv As Short, ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		For	Each sd In colSkill
			With sd
				If sname = .Name Then
					If lv >= .NecessaryLevel Then
						IsSkillAvailable = True
						Exit Function
					End If
				End If
			End With
		Next sd
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillLevel(ByVal lv As Short, ByRef sname As String) As Double
		Dim sd As SkillData
		Dim lv2 As Short
		
		lv2 = 0
		For	Each sd In colSkill
			With sd
				If sname = .Name Then
					If .NecessaryLevel > lv Then
						Exit For
					End If
					If .NecessaryLevel > lv2 Then
						lv2 = .NecessaryLevel
						SkillLevel = .Level
					End If
				End If
			End With
		Next sd
		
		If SkillLevel = DEFAULT_LEVEL Then
			SkillLevel = 1
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillLevel2(ByVal lv As Short, ByRef sname As String) As Short
		Dim sd As SkillData
		Dim lv2 As Short
		
		lv2 = 0
		For	Each sd In colSkill
			With sd
				If sname = .Name Or sname = .StrData Then
					If .NecessaryLevel > lv Then
						Exit For
					End If
					If .NecessaryLevel > lv2 Then
						lv2 = .NecessaryLevel
						SkillLevel2 = .Level
					End If
				End If
			End With
		Next sd
		
		If SkillLevel2 = DEFAULT_LEVEL Then
			SkillLevel2 = 1
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillData(ByVal lv As Short, ByRef sname As String) As String
		Dim sd As SkillData
		Dim lv2 As Short
		
		lv2 = 0
		For	Each sd In colSkill
			With sd
				If sname = .Name Then
					If .NecessaryLevel > lv Then
						Exit For
					End If
					If .NecessaryLevel > lv2 Then
						lv2 = .NecessaryLevel
						SkillData = .StrData
					End If
				End If
			End With
		Next sd
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillName(ByVal lv As Short, ByRef sname As String) As String
		Dim sd As SkillData
		Dim lv2 As Short
		
		SkillName = sname
		
		lv2 = 0
		For	Each sd In colSkill
			With sd
				If sname = .Name Then
					If .NecessaryLevel > lv Then
						Exit For
					End If
					If .NecessaryLevel > lv2 Then
						lv2 = .NecessaryLevel
						If Len(.StrData) > 0 Then
							SkillName = LIndex(.StrData, 1)
							Select Case SkillName
								Case "髱櫁｡ｨ遉ｺ"
									Exit Function
								Case "隗｣隱ｬ"
									SkillName = "髱櫁｡ｨ遉ｺ"
									Exit Function
							End Select
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							GoTo NextLoop
						End If
					Else
						SkillName = sname
					End If
					
					'Invalid_string_refer_to_original_code_
					'And sname <> "髴雁鴨" _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If .Level <> DEFAULT_LEVEL And InStr(SkillName, "Lv") = 0 Then
						SkillName = SkillName & "Lv" & VB6.Format(.Level)
					End If
				End If
				'End If
				'End If
			End With
NextLoop: 
		Next sd
		
		'繝ｬ繝吶Ν髱櫁｡ｨ遉ｺ逕ｨ縺ｮ諡ｬ蠑ｧ繧貞炎髯､
		If Left(SkillName, 1) = "(" Then
			SkillName = Mid(SkillName, 2, Len(SkillName) - 2)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillType(ByRef sname As String) As String
		Dim sd As SkillData
		
		For	Each sd In colSkill
			With sd
				If sname = .Name Or sname = .StrData Then
					SkillType = .Name
					Exit Function
				End If
			End With
		Next sd
		
		'Invalid_string_refer_to_original_code
		SkillType = sname
	End Function
	
	
	'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ繧定ｿｽ蜉
	Public Sub AddSpecialPower(ByRef sname As String, ByVal lv As Short, ByVal sp_consumption As Short)
		ReDim Preserve SpecialPowerName(UBound(SpecialPowerName) + 1)
		ReDim Preserve SpecialPowerNecessaryLevel(UBound(SpecialPowerName))
		ReDim Preserve SpecialPowerSPConsumption(UBound(SpecialPowerName))
		SpecialPowerName(UBound(SpecialPowerName)) = sname
		SpecialPowerNecessaryLevel(UBound(SpecialPowerName)) = lv
		SpecialPowerSPConsumption(UBound(SpecialPowerName)) = sp_consumption
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function CountSpecialPower(ByVal lv As Short) As Short
		Dim i As Short
		
		For i = 1 To UBound(SpecialPowerName)
			If SpecialPowerNecessaryLevel(i) <= lv Then
				CountSpecialPower = CountSpecialPower + 1
			End If
		Next 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SpecialPower(ByVal lv As Short, ByVal idx As Short) As String
		Dim i, n As Short
		
		n = 0
		For i = 1 To UBound(SpecialPowerName)
			If SpecialPowerNecessaryLevel(i) <= lv Then
				n = n + 1
				If idx = n Then
					SpecialPower = SpecialPowerName(i)
					Exit Function
				End If
			End If
		Next 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsSpecialPowerAvailable(ByVal lv As Short, ByRef sname As String) As Boolean
		Dim i As Short
		
		For i = 1 To UBound(SpecialPowerName)
			If SpecialPowerName(i) = sname Then
				If SpecialPowerNecessaryLevel(i) <= lv Then
					IsSpecialPowerAvailable = True
				End If
				Exit Function
			End If
		Next 
		
		IsSpecialPowerAvailable = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SpecialPowerCost(ByVal sname As String) As Short
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		For i = 1 To UBound(SpecialPowerName)
			If SpecialPowerName(i) = sname Then
				SpecialPowerCost = SpecialPowerSPConsumption(i)
				Exit For
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If SpecialPowerCost = 0 Then
			SpecialPowerCost = SPDList.Item(sname).SPConsumption
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Clear()
		Dim i As Short
		
		ReDim SpecialPowerName(0)
		ReDim SpecialPowerNecessaryLevel(0)
		ReDim SpecialPowerSPConsumption(0)
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		If Not colFeature Is Nothing Then
			With colFeature
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
		End If
		If Not colWeaponData Is Nothing Then
			With colWeaponData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
		End If
		If Not colAbilityData Is Nothing Then
			With colAbilityData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub AddFeature(ByRef fdef As String)
		Dim fd As FeatureData
		Dim ftype, fdata As String
		Dim flevel As Double
		Dim nskill, ncondition As String
		Dim i, j As Short
		Dim buf As String
		
		If colFeature Is Nothing Then
			colFeature = New Collection
		End If
		
		'Invalid_string_refer_to_original_code
		If Right(fdef, 1) = ")" Then
			i = InStr(fdef, " (")
			If i > 0 Then
				nskill = Trim(Mid(fdef, i + 2, Len(fdef) - i - 2))
				buf = Trim(Left(fdef, i))
			ElseIf Left(fdef, 1) = "(" Then 
				nskill = Trim(Mid(fdef, 2, Len(fdef) - 2))
				buf = ""
			Else
				buf = fdef
			End If
		Else
			buf = fdef
		End If
		
		'Invalid_string_refer_to_original_code
		If Right(buf, 1) = ">" Then
			i = InStr(buf, " <")
			If i > 0 Then
				ncondition = Trim(Mid(buf, i + 2, Len(buf) - i - 2))
				buf = Trim(Left(buf, i))
			ElseIf Left(buf, 1) = "<" Then 
				ncondition = Trim(Mid(buf, 2, Len(buf) - 2))
				buf = ""
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		flevel = DEFAULT_LEVEL
		i = InStr(buf, "Lv")
		j = InStr(buf, "=")
		If i > 0 And j > 0 And i > j Then
			i = 0
		End If
		If i > 0 Then
			ftype = Left(buf, i - 1)
			If j > 0 Then
				flevel = CDbl(Mid(buf, i + 2, j - (i + 2)))
				fdata = Mid(buf, j + 1)
			Else
				flevel = CDbl(Mid(buf, i + 2))
			End If
		ElseIf j > 0 Then 
			ftype = Left(buf, j - 1)
			fdata = Mid(buf, j + 1)
		Else
			ftype = buf
		End If
		
		'Invalid_string_refer_to_original_code
		If Left(fdata, 1) = """" Then
			If Right(fdata, 1) = """" Then
				fdata = Mid(fdata, 2, Len(fdata) - 2)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If ALDList.IsDefined(ftype) Then
			If LIndex(fdata, 1) <> "隗｣隱ｬ" Then
				With ALDList.Item(ftype)
					For i = 1 To .Count
						fd = New FeatureData
						
						'Invalid_string_refer_to_original_code
						fd.Name = .AliasType(i)
						If .AliasType(i) <> ftype Then
							If .AliasLevelIsPlusMod(i) Then
								If flevel = DEFAULT_LEVEL Then
									flevel = 1
								End If
								If .AliasLevel(i) = DEFAULT_LEVEL Then
									fd.Level = flevel + 1
								Else
									fd.Level = flevel + .AliasLevel(i)
								End If
							ElseIf .AliasLevelIsMultMod(i) Then 
								If flevel = DEFAULT_LEVEL Then
									flevel = 1
								End If
								If .AliasLevel(i) = DEFAULT_LEVEL Then
									fd.Level = flevel
								Else
									fd.Level = flevel * .AliasLevel(i)
								End If
							ElseIf flevel <> DEFAULT_LEVEL Then 
								fd.Level = flevel
							Else
								fd.Level = .AliasLevel(i)
							End If
							If fdata <> "" And InStr(.AliasData(i), "髱櫁｡ｨ遉ｺ") <> 1 Then
								fd.StrData = fdata & " " & ListTail(.AliasData(i), LLength(fdata) + 1)
							Else
								fd.StrData = .AliasData(i)
							End If
							If .AliasLevelIsMultMod(i) Then
								buf = fd.StrData
								ReplaceString(buf, "Lv1", "Lv" & VB6.Format(flevel))
								fd.StrData = buf
							End If
						Else
							'Invalid_string_refer_to_original_code
							If fdata <> "" And LIndex(fdata, 1) <> "髱櫁｡ｨ遉ｺ" Then
								fd.Name = LIndex(fdata, 1)
							End If
							fd.StrData = .AliasData(i)
						End If
						If nskill <> "" Then
							fd.NecessarySkill = nskill
						Else
							fd.NecessarySkill = .AliasNecessarySkill(i)
						End If
						If ncondition <> "" Then
							fd.NecessaryCondition = ncondition
						Else
							fd.NecessaryCondition = .AliasNecessaryCondition(i)
						End If
						
						'Invalid_string_refer_to_original_code
						If IsFeatureAvailable((fd.Name)) Then
							colFeature.Add(fd, fd.Name & VB6.Format(CountFeature))
						Else
							colFeature.Add(fd, fd.Name)
						End If
					Next 
				End With
				Exit Sub
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		fd = New FeatureData
		With fd
			.Name = ftype
			.Level = flevel
			.StrData = fdata
			.NecessarySkill = nskill
			.NecessaryCondition = ncondition
		End With
		If IsFeatureAvailable(ftype) Then
			colFeature.Add(fd, ftype & VB6.Format(CountFeature))
		Else
			colFeature.Add(fd, ftype)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function CountFeature() As Short
		If colFeature Is Nothing Then
			Exit Function
		End If
		CountFeature = colFeature.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Feature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		Feature = fd.Name
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureName(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		With fd
			If Len(.StrData) > 0 Then
				FeatureName = ListIndex(.StrData, 1)
			ElseIf .Level <> DEFAULT_LEVEL Then 
				FeatureName = .Name & "Lv" & VB6.Format(.Level)
			Else
				FeatureName = .Name
			End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureLevel(ByRef Index As Object) As Double
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureLevel = fd.Level
		If FeatureLevel = DEFAULT_LEVEL Then
			FeatureLevel = 1
		End If
		Exit Function
		
ErrorHandler: 
		FeatureLevel = 0
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		FeatureData = ""
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		If colFeature Is Nothing Then
			Exit Function
		End If
		For	Each fd In colFeature
			If fd.Name = fname Then
				IsFeatureAvailable = True
				Exit Function
			End If
		Next fd
		IsFeatureAvailable = False
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = colWeaponData.Item(Index)
	End Function
	
	'豁ｦ蝎ｨ縺ｮ邱乗焚
	Public Function CountWeapon() As Short
		If colWeaponData Is Nothing Then
			Exit Function
		End If
		CountWeapon = colWeaponData.Count()
	End Function
	
	'豁ｦ蝎ｨ繧定ｿｽ蜉
	Public Function AddWeapon(ByRef wname As String) As WeaponData
		Dim new_wdata As New WeaponData
		
		If colWeaponData Is Nothing Then
			colWeaponData = New Collection
		End If
		new_wdata.Name = wname
		colWeaponData.Add(new_wdata, wname & VB6.Format(CountWeapon))
		AddWeapon = new_wdata
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = colAbilityData.Item(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function CountAbility() As Short
		If colAbilityData Is Nothing Then
			Exit Function
		End If
		CountAbility = colAbilityData.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function AddAbility(ByRef aname As String) As AbilityData
		Dim new_adata As New AbilityData
		
		If colAbilityData Is Nothing Then
			colAbilityData = New Collection
		End If
		new_adata.Name = aname
		colAbilityData.Add(new_adata, aname & VB6.Format(CountAbility))
		AddAbility = new_adata
	End Function
End Class