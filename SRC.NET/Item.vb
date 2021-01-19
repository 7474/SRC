Option Strict Off
Option Explicit On
Friend Class Item
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'作成されたアイテムのクラス
	
	'識別子
	Public ID As String
	'アイテムデータへのポインタ
	Public Data As ItemData
	'アイテムを装備しているユニット
	'UPGRADE_NOTE: Unit は Unit_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Unit_Renamed As Unit
	'アイテムが存在しているか？ (RemoveItemされていないか？)
	Public Exist As Boolean
	'アイテムが効力を発揮できているか？ (必要技能や武器クラス＆防具クラスを満たしているか？)
	Public Activated As Boolean
	
	'クラスの初期化
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
	
	'クラスの解放
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
	
	'名称
	
	Public Property Name() As String
		Get
			Name = Data.Name
		End Get
		Set(ByVal Value As String)
			Data = IDList.Item(Value)
		End Set
	End Property
	
	'愛称
	Public Function Nickname() As String
		Dim u As Unit
		
		Nickname = Data.Nickname
		
		'愛称内の式置換のため、デフォルトユニットを一時的に変更する
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(Nickname)
		SelectedUnitForEvent = u
	End Function
	
	'読み仮名
	Public Function KanaName() As String
		Dim u As Unit
		
		KanaName = Data.KanaName
		
		'読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(KanaName)
		SelectedUnitForEvent = u
	End Function
	
	'クラス
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Function Class_Renamed() As String
		Class_Renamed = Data.Class_Renamed
	End Function
	
	Public Function Class0() As String
		Dim i, n As Short
		
		Class0 = Data.Class_Renamed
		
		'専用指定を削除
		If Right(Class0, 3) = "専用)" Then
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
	
	'装備個所
	Public Function Part() As String
		Part = Data.Part
	End Function
	
	'ＨＰ修正値
	Public Function HP() As Integer
		HP = Data.HP
	End Function
	
	'ＥＮ修正値
	Public Function EN() As Short
		EN = Data.EN
	End Function
	
	'装甲修正値
	Public Function Armor() As Integer
		Armor = Data.Armor
	End Function
	
	'運動性修正値
	Public Function Mobility() As Short
		Mobility = Data.Mobility
	End Function
	
	'移動力修正値
	Public Function Speed() As Short
		Speed = Data.Speed
	End Function
	
	'特殊能力総数
	Public Function CountFeature() As Short
		CountFeature = Data.CountFeature
	End Function
	
	'特殊能力
	Public Function Feature(ByRef Index As Object) As String
		Feature = Data.Feature(Index)
	End Function
	
	'特殊能力の名称
	Public Function FeatureName(ByRef Index As Object) As String
		FeatureName = Data.FeatureName(Index)
	End Function
	
	'特殊能力のレベル
	Public Function FeatureLevel(ByRef Index As Object) As Double
		FeatureLevel = Data.FeatureLevel(Index)
	End Function
	
	'特殊能力のデータ
	Public Function FeatureData(ByRef Index As Object) As String
		FeatureData = Data.FeatureData(Index)
	End Function
	
	'特殊能力の必要技能
	Public Function FeatureNecessarySkill(ByRef Index As Object) As String
		FeatureNecessarySkill = Data.FeatureNecessarySkill(Index)
	End Function
	
	'指定した特殊能力を持っているか？
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		IsFeatureAvailable = Data.IsFeatureAvailable(fname)
	End Function
	
	'武器データ
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = Data.Weapon(Index)
	End Function
	
	'武器の総数
	Public Function CountWeapon() As Short
		CountWeapon = Data.CountWeapon
	End Function
	
	'アビリティデータ
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = Data.Ability(Index)
	End Function
	
	'アビリティの総数
	Public Function CountAbility() As Short
		CountAbility = Data.CountAbility
	End Function
	
	'サイズ(アイテムが消費するアイテムスロット数)
	Public Function Size() As Short
		Size = Data.Size
	End Function
	
	
	'アイテムが使用可能か？
	Public Function IsAvailable(ByRef u As Unit) As Boolean
		Dim j, i, k As Short
		Dim iclass As String
		Dim sname, fdata As String
		
		IsAvailable = False
		
		'イベントコマンド「Disable」
		If IsGlobalVariableDefined("Disable(" & Name & ")") Then
			Exit Function
		End If
		
		'装備個所に適合しているか
		Select Case Part
			Case "片手", "両手", "盾"
				If InStr(u.FeatureData("装備個所"), "腕") = 0 Then
					Exit Function
				End If
			Case "肩", "両肩"
				If InStr(u.FeatureData("装備個所"), "肩") = 0 Then
					Exit Function
				End If
			Case "体"
				If InStr(u.FeatureData("装備個所"), "体") = 0 Then
					Exit Function
				End If
			Case "頭"
				If InStr(u.FeatureData("装備個所"), "頭") = 0 Then
					Exit Function
				End If
		End Select
		
		'武器クラス or 防具クラスに属しているか？
		Select Case Part
			Case "武器", "片手", "両手"
				iclass = u.WeaponProficiency() & " 固定 汎用"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case "盾", "体", "頭"
				iclass = u.ArmorProficiency() & " 固定 汎用"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case Else
				'その他のアイテムは常に利用可能
				IsAvailable = True
		End Select
		
		If Not IsAvailable Then
			Exit Function
		End If
		
		'技能チェックが必要？
		If Not IsFeatureAvailable("必要技能") And Not IsFeatureAvailable("不必要技能") Then
			Exit Function
		End If
		
		With u
			'必要技能をチェック
			For i = 1 To CountFeature
				Select Case Feature(i)
					Case "必要技能"
						If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'アイテム自身により必要技能に指定された能力が封印されていた場合は
							'必要技能を満たしていると判定させるため、チェックする必要がある。
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'既に装備しているのでなければ装備しない
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'必要技能が「～装備」？
							If Right(sname, 2) = "装備" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'封印する能力が必要技能になっている？
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "パイロット能力付加", "パイロット能力強化"
									Case Else
										GoTo NextLoop1
								End Select
								
								'封印する能力名
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'必要技能と封印する能力が一致している？
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
							
							'必要技能が満たされていなかった
							IsAvailable = False
							Exit Function
						End If
					Case "不必要技能"
						If .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'アイテム自身により不必要技能が満たされている場合は不必要技能を
							'無視させるため、チェックする必要がある。
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'既に装備しているのでなければ装備しない
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'不必要技能が「～装備」？
							If Right(sname, 2) = "装備" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'付加する能力が不必要技能になっている？
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "パイロット能力付加", "パイロット能力強化"
									Case Else
										GoTo NextLoop2
								End Select
								
								'付加する能力名
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'必要技能と付加する能力が一致している？
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
							
							'不必要技能が満たされていた
							IsAvailable = False
							Exit Function
						End If
				End Select
NextLoop: 
			Next 
		End With
	End Function
	
	
	'一時中断用データをファイルにセーブする
	Public Sub Dump()
		WriteLine(SaveDataFileNumber, Name, ID, Exist)
		If Unit_Renamed Is Nothing Then
			WriteLine(SaveDataFileNumber, "-")
		Else
			WriteLine(SaveDataFileNumber, Unit_Renamed.ID)
		End If
	End Sub
	
	'一時中断用データをファイルからロードする
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
	
	'一時中断用データのリンク情報をファイルからロードする
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
	
	''一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim sbuf As String
		
		'Name, ID, Exist
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
End Class