Option Strict Off
Option Explicit On
Friend Class UnitData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ユニットデータのクラス
	
	'名称
	Public Name As String
	'識別子
	Public ID As Integer
	'クラス
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Class_Renamed As String
	'パイロット数 (マイナスの場合は括弧つきの指定)
	Public PilotNum As Short
	'アイテム数
	Public ItemNum As Short
	'地形適応
	Public Adaption As String
	'ＨＰ
	Public HP As Integer
	'ＥＮ
	Public EN As Short
	'移動タイプ
	Public Transportation As String
	'移動力
	Public Speed As Short
	'サイズ
	Public Size As String
	'装甲
	Public Armor As Integer
	'運動性
	Public Mobility As Short
	'修理費
	Public Value As Integer
	'経験値
	Public ExpValue As Short
	
	'愛称
	Private proNickname As String
	'読み仮名
	Private proKanaName As String
	
	'ビットマップ名
	Private proBitmap As String
	'ビットマップが存在するか
	Public IsBitmapMissing As Boolean
	
	'特殊能力
	Public colFeature As Collection
	'武器データ
	Private colWeaponData As Collection
	'アビリティデータ
	Private colAbilityData As Collection
	
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
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
	
	'愛称
	
	Public Property Nickname() As String
		Get
			Nickname = proNickname
			If InStr(Nickname, "主人公") = 1 Or InStr(Nickname, "ヒロイン") = 1 Then
				Nickname = GetValueAsString(Nickname & "愛称")
			End If
			ReplaceSubExpression(Nickname)
		End Get
		Set(ByVal Value As String)
			proNickname = Value
		End Set
	End Property
	
	'読み仮名
	
	Public Property KanaName() As String
		Get
			KanaName = proKanaName
			If InStr(KanaName, "主人公") = 1 Or InStr(KanaName, "ヒロイン") = 1 Or InStr(KanaName, "ひろいん") = 1 Then
				If IsVariableDefined(KanaName & "読み仮名") Then
					KanaName = GetValueAsString(KanaName & "読み仮名")
				Else
					KanaName = StrToHiragana(GetValueAsString(KanaName & "愛称"))
				End If
			End If
			ReplaceSubExpression(KanaName)
		End Get
		Set(ByVal Value As String)
			proKanaName = Value
		End Set
	End Property
	
	'ビットマップ
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
	
	
	'特殊能力を追加
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
		
		'必要技能の切り出し
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
		
		'必要条件の切り出し
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
		
		'特殊能力の種類、レベル、データを切り出し
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
		
		'データが「"」で囲まれている場合、「"」を削除
		If Left(fdata, 1) = """" Then
			If Right(fdata, 1) = """" Then
				fdata = Mid(fdata, 2, Len(fdata) - 2)
			End If
		End If
		
		'エリアスが定義されている？
		If ALDList.IsDefined(ftype) Then
			If LIndex(fdata, 1) <> "解説" Then
				With ALDList.Item(ftype)
					For i = 1 To .Count
						fd = New FeatureData
						
						'エリアスの定義に従って特殊能力定義を置き換える
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
							If fdata <> "" And InStr(.AliasData(i), "非表示") <> 1 Then
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
							'特殊能力解説の定義
							If fdata <> "" And LIndex(fdata, 1) <> "非表示" Then
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
						
						'特殊能力を登録
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
		
		'特殊能力を登録
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
	
	'特殊能力の総数
	Public Function CountFeature() As Short
		If colFeature Is Nothing Then
			Exit Function
		End If
		CountFeature = colFeature.Count()
	End Function
	
	'特殊能力
	Public Function Feature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		Feature = fd.Name
	End Function
	
	'特殊能力の名称
	Public Function FeatureName(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		With fd
			If Len(.StrData) > 0 Then
				FeatureName = ListIndex(.StrData, 1)
			ElseIf .Level > 0 Then 
				FeatureName = .Name & "Lv" & VB6.Format(.Level)
			Else
				FeatureName = .Name
			End If
		End With
	End Function
	
	'特殊能力のレベル
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
	
	'特殊能力のデータ
	Public Function FeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		FeatureData = ""
	End Function
	
	'特殊能力の必要技能
	Public Function FeatureNecessarySkill(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureNecessarySkill = fd.NecessarySkill
		Exit Function
		
ErrorHandler: 
		FeatureNecessarySkill = ""
	End Function
	
	'指定した特殊能力を持っているか？
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(fname)
		IsFeatureAvailable = True
		Exit Function
		
ErrorHandler: 
		IsFeatureAvailable = False
	End Function
	
	'指定した特殊能力がレベル指定されているか？
	Public Function IsFeatureLevelSpecified(ByRef Index As Object) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		
		If fd.Level = DEFAULT_LEVEL Then
			IsFeatureLevelSpecified = False
		Else
			IsFeatureLevelSpecified = True
		End If
		Exit Function
		
ErrorHandler: 
		IsFeatureLevelSpecified = False
	End Function
	
	'武器を追加
	Public Function AddWeapon(ByRef wname As String) As WeaponData
		Dim new_wdata As New WeaponData
		
		If colWeaponData Is Nothing Then
			colWeaponData = New Collection
		End If
		new_wdata.Name = wname
		colWeaponData.Add(new_wdata, wname & VB6.Format(CountWeapon))
		AddWeapon = new_wdata
	End Function
	
	'武器の総数
	Public Function CountWeapon() As Short
		If colWeaponData Is Nothing Then
			Exit Function
		End If
		CountWeapon = colWeaponData.Count()
	End Function
	
	'武器データ
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = colWeaponData.Item(Index)
	End Function
	
	'アビリティを追加
	Public Function AddAbility(ByRef aname As String) As AbilityData
		Dim new_sadata As New AbilityData
		
		If colAbilityData Is Nothing Then
			colAbilityData = New Collection
		End If
		new_sadata.Name = aname
		colAbilityData.Add(new_sadata, aname & VB6.Format(CountAbility))
		AddAbility = new_sadata
	End Function
	
	'アビリティの総数
	Public Function CountAbility() As Short
		If colAbilityData Is Nothing Then
			Exit Function
		End If
		CountAbility = colAbilityData.Count()
	End Function
	
	'アビリティデータ
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = colAbilityData.Item(Index)
	End Function
	
	'特殊能力、武器データ、アビリティデータを削除する
	Public Sub Clear()
		Dim i As Short
		
		If Not colFeature Is Nothing Then
			For i = 1 To colFeature.Count()
				colFeature.Remove(1)
			Next 
		End If
		If Not colWeaponData Is Nothing Then
			For i = 1 To colWeaponData.Count()
				colWeaponData.Remove(1)
			Next 
		End If
		If Not colAbilityData Is Nothing Then
			For i = 1 To colAbilityData.Count()
				colAbilityData.Remove(1)
			Next 
		End If
	End Sub
End Class