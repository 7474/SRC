Option Strict Off
Option Explicit On
Friend Class Unit
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'データ
	Public Data As UnitData
	
	'識別用ＩＤ
	Public ID As String
	
	'ビットマップID
	'同種のユニットは同じIDを共有
	Public BitmapID As Short
	
	'Ｘ座標
	Public x As Short
	'Ｙ座標
	Public y As Short
	
	'ユニットの場所（地上、水上、水中、空中、地中、宇宙）
	Public Area As String
	
	'使用済み行動数
	Public UsedAction As Short
	
	'思考モード
	Private strMode As String
	
	'ステータス
	'出撃：マップ上に出撃
	'他形態：他の形態に変形(ハイパーモード)中
	'破壊：破壊されている
	'破棄：イベントコマンド RemoveUnit などによりイベントで破壊されている
	'格納：母艦内に格納されている
	'待機：待機中
	'旧形態：分離ユニットが合体前に取っていた形態
	'離脱：Leaveコマンドにより戦線を離脱。Organizeコマンドでも表示されない
	'UPGRADE_NOTE: Status は Status_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Status_Renamed As String
	
	'ユニットに対して使用されているスペシャルパワー
	Private colSpecialPowerInEffect As New Collection
	
	'サポートアタック＆ガードの使用回数
	Public UsedSupportAttack As Short
	Public UsedSupportGuard As Short
	
	'同時援護攻撃の使用回数
	Public UsedSyncAttack As Short
	
	'カウンター攻撃の使用回数
	Public UsedCounterAttack As Short
	
	'ユニット名称
	Private strName As String
	'陣営
	Private strParty As String
	'ユニットランク
	Private intRank As Short
	'ボスランク
	Private intBossRank As Short
	'ＨＰ
	Private lngMaxHP As Integer
	Private lngHP As Integer
	'ＥＮ
	Private intMaxEN As Short
	Private intEN As Short
	'装甲
	Private lngArmor As Integer
	'運動性
	Private intMobility As Short
	'移動力
	Private intSpeed As Short
	
	'搭乗しているパイロット
	Private colPilot As New Collection
	
	'搭乗しているサポートパイロット
	Private colSupport As New Collection
	
	'関連するユニット
	'変形ユニットにおける他形態等
	Private colOtherForm As New Collection
	
	'格納したユニット
	Private colUnitOnBoard As New Collection
	
	'装備しているアイテム
	Private colItem As New Collection
	
	'現在の特殊ステータス
	Private colCondition As New Collection
	
	'各武器の残弾数
	Private dblBullet() As Double
	
	'アビリティの残り使用回数
	Private dblStock() As Double
	
	'特殊能力
	Private colFeature As New Collection
	
	'特殊能力(必要条件を満たさないものを含む)
	Private colAllFeature As New Collection
	
	'付加された特殊能力数
	Public AdditionalFeaturesNum As Short
	
	'地形適応
	Private strAdaption As String
	
	'攻撃への耐性
	Public strAbsorb As String
	Public strImmune As String
	Public strResist As String
	Public strWeakness As String
	Public strEffective As String
	Public strSpecialEffectImmune As String
	
	'武器データ
	Private WData() As WeaponData
	Private lngWeaponPower() As Integer
	Private intWeaponMaxRange() As Short
	Private intWeaponPrecision() As Short
	Private intWeaponCritical() As Short
	Private strWeaponClass() As String
	Private intMaxBullet() As Short
	
	'アビリティデータ
	Private adata() As AbilityData
	
	'選択したマップ攻撃の攻撃力
	Private SelectedMapAttackPower As Integer
	
	'選択したマップ攻撃の攻撃力
	Private IsMapAttackCanceled As Boolean
	
	'召喚したユニット
	Private colServant As New Collection
	
	'魅了・憑依したユニット
	Private colSlave As New Collection
	
	'召喚主
	Public Summoner As Unit
	
	'ご主人様
	Public Master As Unit
	
	'追加パイロット
	Public pltAdditionalPilot As Pilot
	
	'追加サポート
	Public pltAdditionalSupport As Pilot
	
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Status_Renamed = "待機"
		intBossRank = -1
		'UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Summoner = Nothing
		'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Master = Nothing
		ReDim WData(0)
		ReDim adata(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		'UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Data = Nothing
		
		With colPilot
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colPilot = Nothing
		
		With colSupport
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colSupport = Nothing
		
		With colOtherForm
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colOtherForm をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colOtherForm = Nothing
		
		With colItem
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colItem をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colItem = Nothing
		
		With colCondition
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colCondition をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colCondition = Nothing
		
		With colFeature
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colFeature をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colFeature = Nothing
		
		For i = 1 To UBound(WData)
			'UPGRADE_NOTE: オブジェクト WData() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			WData(i) = Nothing
		Next 
		
		For i = 1 To UBound(adata)
			'UPGRADE_NOTE: オブジェクト adata() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			adata(i) = Nothing
		Next 
		
		With colUnitOnBoard
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colUnitOnBoard をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colUnitOnBoard = Nothing
		
		With colServant
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colServant をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colServant = Nothing
		
		With colSlave
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colSlave をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colSlave = Nothing
		
		'UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Summoner = Nothing
		'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Master = Nothing
		'UPGRADE_NOTE: オブジェクト pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		pltAdditionalPilot = Nothing
		'UPGRADE_NOTE: オブジェクト pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		pltAdditionalSupport = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	' === 各種基本ステータス ===
	
	'ユニット名称
	
	Public Property Name() As String
		Get
			Name = strName
		End Get
		Set(ByVal Value As String)
			strName = Value
			Data = UDList.Item(Value)
			Update()
		End Set
	End Property
	
	'愛称
	Public ReadOnly Property Nickname0() As String
		Get
			Dim idx As Short
			Dim u As Unit
			
			Nickname0 = Data.Nickname
			
			'愛称変更能力による変更
			If IsFeatureAvailable("愛称変更") Then
				Nickname0 = FeatureData("愛称変更")
				idx = InStr(Nickname0, "$(愛称)")
				If idx > 0 Then
					Nickname0 = Left(Nickname0, idx - 1) & Data.Nickname & Mid(Nickname0, idx + 5)
				End If
				idx = InStr(Nickname0, "$(パイロット愛称)")
				If idx > 0 Then
					If CountPilot > 0 Then
						Nickname0 = Left(Nickname0, idx - 1) & MainPilot.Nickname(True) & Mid(Nickname0, idx + 10)
					Else
						Nickname0 = Left(Nickname0, idx - 1) & "○○" & Mid(Nickname0, idx + 10)
					End If
				End If
			End If
			
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Me
			ReplaceSubExpression(Nickname0)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'メッセージ中で表示する際の愛称は等身大基準ではパイロット名を使う
	Public ReadOnly Property Nickname() As String
		Get
			If IsOptionDefined("等身大基準") Then
				If CountPilot() > 0 Then
					With MainPilot
						If InStr(.Name, "(ザコ)") = 0 And InStr(.Name, "(汎用)") = 0 Then
							Nickname = MainPilot.Nickname
							Exit Property
						End If
					End With
				End If
			End If
			Nickname = Nickname0
		End Get
	End Property
	
	'読み仮名
	Public ReadOnly Property KanaName() As String
		Get
			Dim idx As Short
			Dim u As Unit
			
			KanaName = Data.KanaName
			
			'読み仮名変更能力による変更
			If IsFeatureAvailable("読み仮名変更") Then
				KanaName = FeatureData("読み仮名変更")
				idx = InStr(KanaName, "$(読み仮名)")
				If idx > 0 Then
					KanaName = Left(KanaName, idx - 1) & Data.KanaName & Mid(KanaName, idx + 5)
				End If
				idx = InStr(KanaName, "$(パイロット読み仮名)")
				If idx > 0 Then
					If CountPilot > 0 Then
						KanaName = Left(KanaName, idx - 1) & MainPilot.KanaName & Mid(KanaName, idx + 10)
					Else
						KanaName = Left(KanaName, idx - 1) & "○○" & Mid(KanaName, idx + 10)
					End If
				End If
			ElseIf IsFeatureAvailable("愛称変更") Then 
				KanaName = FeatureData("愛称変更")
				idx = InStr(KanaName, "$(愛称)")
				If idx > 0 Then
					KanaName = Left(KanaName, idx - 1) & Data.Nickname & Mid(KanaName, idx + 5)
				End If
				idx = InStr(KanaName, "$(パイロット愛称)")
				If idx > 0 Then
					If CountPilot > 0 Then
						KanaName = Left(KanaName, idx - 1) & MainPilot.Nickname & Mid(KanaName, idx + 10)
					Else
						KanaName = Left(KanaName, idx - 1) & "○○" & Mid(KanaName, idx + 10)
					End If
				End If
				KanaName = StrToHiragana(KanaName)
			End If
			
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Me
			ReplaceSubExpression(KanaName)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'ユニットランク
	
	Public Property Rank() As Short
		Get
			Rank = intRank
		End Get
		Set(ByVal Value As Short)
			Dim uname As String
			
			If intRank = Value Then
				Exit Property
			End If
			
			intRank = Value
			If intRank > 999 Then
				intRank = 999
			End If
			
			'パラメータを更新
			Update()
		End Set
	End Property
	
	'ボスランク
	
	Public Property BossRank() As Short
		Get
			BossRank = intBossRank
		End Get
		Set(ByVal Value As Short)
			If intBossRank = Value Then
				Exit Property
			End If
			
			intBossRank = Value
			
			'パラメータを更新
			Update()
		End Set
	End Property
	
	'ユニットクラス
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public ReadOnly Property Class_Renamed() As String
		Get
			Class_Renamed = Data.Class_Renamed
		End Get
	End Property
	
	'ユニットクラスから余分な指定を除いたもの
	Public ReadOnly Property Class0() As String
		Get
			Dim i, n As Short
			
			Class0 = Data.Class_Renamed
			
			'人間ユニット指定を削除
			If Left(Class0, 1) = "(" Then
				Class0 = Mid(Class0, 2, Len(Class0) - 2)
			End If
			
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
		End Get
	End Property
	
	
	'ユニットサイズ
	Public ReadOnly Property Size() As String
		Get
			If IsFeatureAvailable("サイズ変更") Then
				Size = FeatureData("サイズ変更")
				Exit Property
			End If
			
			Size = Data.Size
		End Get
	End Property
	
	
	'最大ＨＰ値
	Public ReadOnly Property MaxHP() As Integer
		Get
			MaxHP = lngMaxHP
			
			'パイロットによる修正
			If CountPilot > 0 Then
				'霊力変換器装備ユニットは霊力に応じて最大ＨＰが変化
				If IsFeatureAvailable("霊力変換器") Then
					MaxHP = MaxHP + 10 * PlanaLevel()
				End If
				
				'オーラ変換器装備ユニットはオーラレベルに応じて最大ＨＰが変化
				If IsFeatureAvailable("オーラ変換器") Then
					MaxHP = MaxHP + 100 * AuraLevel()
				End If
			End If
			
			'最大ＨＰは最低でも1
			If MaxHP < 1 Then
				MaxHP = 1
			End If
		End Get
	End Property
	
	'最大ＥＮ値
	Public ReadOnly Property MaxEN() As Short
		Get
			MaxEN = intMaxEN
			
			'パイロットによる修正
			If CountPilot > 0 Then
				'霊力変換器装備ユニットは霊力に応じて最大ＥＮが変化
				If IsFeatureAvailable("霊力変換器") Then
					MaxEN = MaxEN + 0.5 * PlanaLevel()
				End If
				
				'オーラ変換器装備ユニットはオーラレベルに応じて最大ＥＮが変化
				If IsFeatureAvailable("オーラ変換器") Then
					MaxEN = MaxEN + 10 * AuraLevel()
				End If
			End If
			
			'最大ＥＮは最低でも5
			If MaxEN < 5 Then
				MaxEN = 5
			End If
		End Get
	End Property
	
	'ＨＰ
	
	Public Property HP() As Integer
		Get
			HP = lngHP
		End Get
		Set(ByVal Value As Integer)
			lngHP = Value
			If lngHP > MaxHP Then
				lngHP = MaxHP
			ElseIf lngHP < 0 Then 
				lngHP = 0
			End If
		End Set
	End Property
	
	'ＥＮ
	
	Public Property EN() As Integer
		Get
			EN = intEN
		End Get
		Set(ByVal Value As Integer)
			intEN = Value
			
			If intEN > MaxEN Then
				intEN = MaxEN
			ElseIf intEN < 0 Then 
				intEN = 0
			End If
		End Set
	End Property
	
	
	'移動力
	Public ReadOnly Property Speed0() As Short
		Get
			Speed0 = intSpeed
		End Get
	End Property
	
	Public ReadOnly Property Speed() As Short
		Get
			Speed = Speed0
			
			'元々移動力が0の場合は0のまま
			If Speed = 0 And Not IsFeatureAvailable("テレポート") And Not IsFeatureAvailable("ジャンプ") Then
				Exit Property
			End If
			
			'特殊状態による移動力修正
			If IsUnderSpecialPowerEffect("移動力強化") Then
				Speed = Speed + SpecialPowerEffectLevel("移動力強化")
			ElseIf IsConditionSatisfied("移動力ＵＰ") Then 
				If IsOptionDefined("大型マップ") Then
					Speed = Speed + 2
				Else
					Speed = Speed + 1
				End If
			End If
			If IsConditionSatisfied("移動力ＤＯＷＮ") Then
				Speed = MaxLng(Speed \ 2, 1)
			End If
			
			'霊力による移動力ＵＰ
			If IsFeatureAvailable("霊力変換器") Then
				If CountPilot > 0 Then
					Speed = Speed + Int(0.01 * PlanaLevel())
				End If
			End If
			
			'スペシャルパワーによる移動力低下
			If IsUnderSpecialPowerEffect("移動力低下") Then
				Speed = MaxLng(Speed \ 2, 1)
			End If
			
			'移動不能の場合は移動力０
			If IsConditionSatisfied("移動不能") Then
				Speed = 0
			End If
			
			'ＥＮ切れにより移動できない場合
			If Status_Renamed = "出撃" Then
				Select Case Area
					Case "空中", "宇宙"
						If EN < 5 Then
							Speed = 0
						End If
					Case "地中"
						If EN < 10 Then
							Speed = 0
						End If
				End Select
			End If
		End Get
	End Property
	
	
	'移動形態
	Public ReadOnly Property Transportation() As String
		Get
			Transportation = Data.Transportation
			
			'特殊能力による移動可能地形追加
			If IsFeatureAvailable("空中移動") Then
				If InStr(Transportation, "空") = 0 Then
					Transportation = "空" & Transportation
				End If
			End If
			If IsFeatureAvailable("陸上移動") Then
				If InStr(Transportation, "陸") = 0 Then
					Transportation = "陸" & Transportation
				End If
			End If
			If IsFeatureAvailable("水中移動") Then
				If InStr(Transportation, "水") = 0 Then
					Transportation = "水" & Transportation
				End If
			End If
			If IsFeatureAvailable("地中移動") Then
				If InStr(Transportation, "地中") = 0 Then
					Transportation = Transportation & "地中"
				End If
			End If
			If IsFeatureAvailable("宇宙移動") Then
				If Transportation = "" Then
					Transportation = "宇宙"
				End If
			End If
		End Get
	End Property
	
	
	'地形適応
	Public ReadOnly Property Adaption(ByVal idx As Short) As Short
		Get
			Dim uad, pad As Short
			
			'ユニット側の地形適応を算出
			Select Case Mid(strAdaption, idx, 1)
				Case "S"
					uad = 5
				Case "A"
					uad = 4
				Case "B"
					uad = 3
				Case "C"
					uad = 2
				Case "D"
					uad = 1
				Case "-"
					Adaption = 0
					Exit Property
			End Select
			
			'パイロット側の地形適応を算出
			If CountPilot > 0 Then
				Select Case Mid(MainPilot.Adaption, idx, 1)
					Case "S"
						pad = 5
					Case "A"
						pad = 4
					Case "B"
						pad = 3
					Case "C"
						pad = 2
					Case "D"
						pad = 1
					Case "-"
						Adaption = 0
						Exit Property
				End Select
			Else
				pad = 4
			End If
			
			If IsOptionDefined("地形適応総和計算") Then
				'ユニットとパイロットの地形適応の総和から地形適応を決定
				Select Case uad + pad
					Case 9, 10
						Adaption = 5
					Case 7, 8
						Adaption = 4
					Case 5, 6
						Adaption = 3
					Case 3, 4
						Adaption = 2
					Case 1, 2
						Adaption = 1
					Case Else
						Adaption = 0
				End Select
			Else
				'ユニットとパイロットの地形適応の低い方を使用
				If uad > pad Then
					Adaption = pad
				Else
					Adaption = uad
				End If
			End If
		End Get
	End Property
	
	'地形適応による修正値
	Public ReadOnly Property AdaptionMod(ByVal idx As Short, Optional ByVal ad_mod As Short = 0) As Double
		Get
			Dim uad As Short
			
			uad = Adaption(idx)
			
			'元々属性がSでない限り、Sにはしない
			If uad = 5 Then
				uad = MinLng(uad + ad_mod, 5)
			Else
				uad = MinLng(uad + ad_mod, 4)
			End If
			
			'Optionコマンドの設定に応じて適応修正値を設定
			If IsOptionDefined("地形適応修正緩和") Then
				If IsOptionDefined("地形適応修正繰り下げ") Then
					Select Case uad
						Case 5
							AdaptionMod = 1.1
						Case 4
							AdaptionMod = 1
						Case 3
							AdaptionMod = 0.9
						Case 2
							AdaptionMod = 0.8
						Case 1
							AdaptionMod = 0.7
						Case Else
							AdaptionMod = 0
					End Select
				Else
					Select Case uad
						Case 5
							AdaptionMod = 1.2
						Case 4
							AdaptionMod = 1.1
						Case 3
							AdaptionMod = 1
						Case 2
							AdaptionMod = 0.9
						Case 1
							AdaptionMod = 0.8
						Case Else
							AdaptionMod = 0
					End Select
				End If
			Else
				If IsOptionDefined("地形適応修正繰り下げ") Then
					Select Case uad
						Case 5
							AdaptionMod = 1.2
						Case 4
							AdaptionMod = 1
						Case 3
							AdaptionMod = 0.8
						Case 2
							AdaptionMod = 0.6
						Case 1
							AdaptionMod = 0.4
						Case Else
							AdaptionMod = 0
					End Select
				Else
					Select Case uad
						Case 5
							AdaptionMod = 1.4
						Case 4
							AdaptionMod = 1.2
						Case 3
							AdaptionMod = 1
						Case 2
							AdaptionMod = 0.8
						Case 1
							AdaptionMod = 0.6
						Case Else
							AdaptionMod = 0
					End Select
				End If
			End If
		End Get
	End Property
	
	
	'装甲
	Public ReadOnly Property Armor(Optional ByVal ref_mode As String = "") As Integer
		Get
			Armor = lngArmor
			
			'ステータス表示用
			Select Case ref_mode
				Case "基本値"
					If IsConditionSatisfied("装甲劣化") Then
						Armor = Armor \ 2
					End If
					If IsConditionSatisfied("石化") Then
						Armor = 2 * Armor
					End If
					If IsConditionSatisfied("凍結") Then
						Armor = Armor \ 2
					End If
					Exit Property
				Case "修正値"
					Armor = 0
			End Select
			
			'パイロットによる修正
			If CountPilot > 0 Then
				'霊力による装甲修正
				If IsFeatureAvailable("霊力変換器") Then
					Armor = Armor + 5 * PlanaLevel()
				End If
				
				'サイキックドライブ装備ユニットは超能力レベルに応じて装甲が変化
				If IsFeatureAvailable("サイキックドライブ") Then
					Armor = Armor + 100 * PsychicLevel()
				End If
				
				'オーラ変換器装備ユニットはオーラレベルに応じて装甲が変化
				If IsFeatureAvailable("オーラ変換器") Then
					Armor = Armor + 50 * AuraLevel()
				End If
			End If
			
			'装甲が劣化している場合は装甲値は半減
			If IsConditionSatisfied("装甲劣化") Then
				Armor = Armor \ 2
			End If
			
			'石化しているユニットはとても固い……
			If IsConditionSatisfied("石化") Then
				Armor = 2 * Armor
			End If
			
			'凍っているユニットは脆くなる
			If IsConditionSatisfied("凍結") Then
				Armor = Armor \ 2
			End If
		End Get
	End Property
	
	'運動性
	Public ReadOnly Property Mobility(Optional ByVal ref_mode As String = "") As Short
		Get
			Mobility = intMobility
			
			Select Case ref_mode
				Case "基本値"
					Exit Property
				Case "修正値"
					Mobility = 0
			End Select
			
			'パイロットによる修正
			If CountPilot > 0 Then
				'サイキックドライブ装備ユニットは超能力レベルに応じて運動性が変化
				If IsFeatureAvailable("サイキックドライブ") Then
					Mobility = Mobility + 5 * PsychicLevel()
				End If
				
				'オーラ変換器装備ユニットはオーラレベルに応じて運動性が変化
				If IsFeatureAvailable("オーラ変換器") Then
					Mobility = Mobility + 2 * AuraLevel()
				End If
				
				'シンクロドライブ装備ユニットは同調率レベルに応じて運動性が変化
				If IsFeatureAvailable("シンクロドライブ") Then
					If MainPilot.SynchroRate > 0 Then
						Mobility = Mobility + (SyncLevel() - 50) \ 2
					End If
				End If
			End If
		End Get
	End Property
	
	'ビットマップ
	Public ReadOnly Property Bitmap(Optional ByVal use_orig As Boolean = False) As String
		Get
			If IsConditionSatisfied("ユニット画像") Then
				Bitmap = LIndex(ConditionData("ユニット画像"), 2)
				Exit Property
			End If
			
			If IsFeatureAvailable("ユニット画像") Then
				Bitmap = FeatureData("ユニット画像")
				Exit Property
			End If
			
			If use_orig Then
				Bitmap = Data.Bitmap0
			Else
				Bitmap = Data.Bitmap
			End If
		End Get
	End Property
	
	'修理費(獲得資金)
	Public ReadOnly Property Value() As Integer
		Get
			Value = Data.Value
			
			If IsFeatureAvailable("修理費修正") Then
				Value = MaxLng(Value + 1000 * FeatureLevel("修理費修正"), 0)
			End If
			
			If BossRank > 0 Then
				Value = Value * (1 + 0.5 * BossRank + 0.05 * Rank)
			Else
				Value = Value * (1 + 0.05 * Rank)
			End If
		End Get
	End Property
	
	'経験値
	Public ReadOnly Property ExpValue() As Integer
		Get
			ExpValue = Data.ExpValue
			
			If IsFeatureAvailable("経験値修正") Then
				ExpValue = MaxLng(ExpValue + 10 * FeatureLevel("経験値修正"), 0)
			End If
			
			If BossRank > 0 Then
				ExpValue = ExpValue + 20 * BossRank
			End If
		End Get
	End Property
	
	'残り行動数
	Public ReadOnly Property Action() As Short
		Get
			If MaxAction > 0 Then
				Action = MaxLng(MaxAction - UsedAction, 0)
			Else
				Action = 0
			End If
		End Get
	End Property
	
	
	' === 行動パターンを規定するパラメータ関連処理 ===
	
	'陣営
	Public ReadOnly Property Party0() As String
		Get
			Party0 = strParty
		End Get
	End Property
	
	
	Public Property Party() As String
		Get
			Party = strParty
			
			'魅了されている場合
			If IsConditionSatisfied("魅了") And Not Master Is Nothing Then
				Party = Master.Party
				If Party = "味方" Then
					'味方になっても自分では操作できない
					Party = "ＮＰＣ"
				End If
			End If
			
			'憑依されている場合
			If IsConditionSatisfied("憑依") And Not Master Is Nothing Then
				Party = Master.Party
			End If
			
			'コントロール不能の味方ユニットはＮＰＣとして扱う
			If Party = "味方" Then
				If IsConditionSatisfied("暴走") Or IsConditionSatisfied("混乱") Or IsConditionSatisfied("恐怖") Or IsConditionSatisfied("踊り") Or IsConditionSatisfied("狂戦士") Then
					Party = "ＮＰＣ"
				End If
			End If
		End Get
		Set(ByVal Value As String)
			strParty = Value
		End Set
	End Property
	
	'思考モード
	
	Public Property Mode() As String
		Get
			Dim i As Short
			
			If IsUnderSpecialPowerEffect("挑発") Then
				'挑発を最優先
				For i = 1 To CountSpecialPower
					'UPGRADE_WARNING: オブジェクト SpecialPower(i).IsEffectAvailable(挑発) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If SpecialPower(i).IsEffectAvailable("挑発") Then
						Mode = SpecialPowerData(i)
						Exit Property
					End If
				Next 
			End If
			
			If IsConditionSatisfied("暴走") Or IsConditionSatisfied("混乱") Or IsConditionSatisfied("憑依") Or IsConditionSatisfied("狂戦士") Then
				'正常な判断が出来ない場合は当初の目的を忘れてしまうため
				'常に通常モードとして扱う
				Mode = "通常"
				Exit Property
			End If
			
			If IsConditionSatisfied("恐怖") Then
				'恐怖にかられた場合は逃亡
				Mode = "逃亡"
				Exit Property
			End If
			
			If IsConditionSatisfied("踊り") Then
				'踊るのに忙しい……
				Mode = "固定"
				Exit Property
			End If
			
			Mode = strMode
		End Get
		Set(ByVal Value As String)
			strMode = Value
		End Set
	End Property
	
	'地形 area_name での移動が可能かどうか
	Public Function IsTransAvailable(ByRef area_name As String) As Boolean
		'移動可能地形に含まれているか？
		If InStr(Transportation, area_name) > 0 Then
			IsTransAvailable = True
		Else
			IsTransAvailable = False
		End If
		
		'水上移動の場合
		If area_name = "水上" Then
			If IsFeatureAvailable("水上移動") Or IsFeatureAvailable("ホバー移動") Then
				IsTransAvailable = True
			End If
		End If
	End Function
	
	
	' === ユニット用特殊能力関連処理 ===
	
	'特殊能力の総数
	Public Function CountFeature() As Short
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
		FeatureName = FeatureNameInt(Index, colFeature)
	End Function
	
	Private Function FeatureNameInt(ByRef Index As Object, ByRef feature_list As Collection) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = feature_list.Item(Index)
		With fd
			'非表示の能力
			Select Case .Name
				Case "ノーマルモード", "パーツ合体", "換装", "制限時間", "制御不可", "主形態", "他形態", "合体制限", "格闘武器", "迎撃武器", "合体技", "変形技", "ランクアップ", "追加パイロット", "暴走時パイロット", "追加サポート", "装備個所", "ハードポイント", "武器クラス", "防具クラス", "ＢＧＭ", "武器ＢＧＭ", "アビリティＢＧＭ", "合体ＢＧＭ", "分離ＢＧＭ", "変形ＢＧＭ", "ハイパーモードＢＧＭ", "ユニット画像", "パイロット画像", "パイロット愛称", "パイロット読み仮名", "性別", "性格変更", "吸収", "無効化", "耐性", "弱点", "有効", "特殊効果無効化", "アイテム所有", "レアアイテム所有", "ラーニング可能技", "改造費修正", "最大改造数", "パイロット能力付加", "パイロット能力強化", "非表示", "攻撃属性", "射程延長", "武器強化", "命中率強化", "ＣＴ率強化", "特殊効果発動率強化", "必要技能", "不必要技能", "ダミーユニット", "地形ユニット", "召喚解除コマンド名", "変身解除コマンド名", "１人乗り可能", "特殊効果", "戦闘アニメ", "パイロット地形適応変更", "メッセージクラス", "用語名", "発光"
					'ユニット用特殊能力
					FeatureNameInt = ""
					Exit Function
				Case "愛称変更", "読み仮名変更", "サイズ変更", "地形適応変更", "地形適応固定変更", "空中移動", "陸上移動", "水中移動", "宇宙移動", "地中移動", "修理費修正", "経験値修正", "最大弾数増加", "ＥＮ消費減少", "Ｖ−ＵＰ", "大型アイテム", "呪い"
					'アイテム用特殊能力
					FeatureNameInt = ""
					Exit Function
			End Select
			
			' ADD START MARGE
			'拡大画像能力は「拡大画像(文字列)」といった指定もあるので他の非表示能力と異なる
			'判定方法を使う
			If InStr(.Name, "拡大画像") = 1 Then
				FeatureNameInt = ""
				Exit Function
			End If
			' ADD END MARGE
			
			If Len(.StrData) > 0 Then
				'別名の指定あり
				FeatureNameInt = ListIndex(.StrData, 1)
				If FeatureNameInt = "非表示" Or FeatureNameInt = "解説" Then
					FeatureNameInt = ""
				End If
			ElseIf .Level = DEFAULT_LEVEL Then 
				'レベル指定なし
				FeatureNameInt = .Name
			ElseIf .Level >= 0 Then 
				'レベル指定あり
				FeatureNameInt = .Name & "Lv" & VB6.Format(.Level)
				If .Name = "射撃強化" Then
					If CountPilot > 0 Then
						If MainPilot.HasMana() Then
							FeatureNameInt = "魔力強化Lv" & VB6.Format(.Level)
						End If
					End If
				End If
			Else
				'マイナスのレベル指定
				Select Case .Name
					Case "格闘強化"
						FeatureNameInt = "格闘低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "射撃強化"
						FeatureNameInt = "射撃低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
						If CountPilot > 0 Then
							If MainPilot.HasMana() Then
								FeatureNameInt = "魔力低下Lv" & VB6.Format(System.Math.Abs(.Level))
							End If
						End If
					Case "命中強化"
						FeatureNameInt = "命中低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "回避強化"
						FeatureNameInt = "回避低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "技量強化"
						FeatureNameInt = "技量低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "反応強化"
						FeatureNameInt = "反応低下" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case Else
						FeatureNameInt = .Name & "Lv" & VB6.Format(.Level)
				End Select
			End If
		End With
		Exit Function
		
ErrorHandler: 
		'見つからなかった場合
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		FeatureNameInt = CStr(Index)
	End Function
	
	Public Function FeatureName0(ByRef Index As Object) As String
		FeatureName0 = FeatureName(Index)
		If InStr(FeatureName0, "Lv") > 0 Then
			FeatureName0 = Left(FeatureName0, InStr(FeatureName0, "Lv") - 1)
		End If
	End Function
	
	'特殊能力のレベル
	Public Function FeatureLevel(ByRef Index As Object) As Double
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		
		With fd
			FeatureLevel = .Level
			If .Level = DEFAULT_LEVEL Then
				FeatureLevel = 1
			End If
		End With
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
	
	'指定した特殊能力を所有しているか？
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(fname)
		IsFeatureAvailable = True
		Exit Function
		
ErrorHandler: 
		IsFeatureAvailable = False
	End Function
	
	'特殊能力にレベル指定がされている？
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
	
	'特殊能力の総数(必要条件を満たさないものを含む)
	Public Function CountAllFeature() As Short
		CountAllFeature = colAllFeature.Count()
	End Function
	
	'特殊能力(必要条件を満たさないものを含む)
	Public Function AllFeature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colAllFeature.Item(Index)
		AllFeature = fd.Name
	End Function
	
	'特殊能力の名称(必要条件を満たさないものを含む)
	Public Function AllFeatureName(ByRef Index As Object) As String
		AllFeatureName = FeatureNameInt(Index, colAllFeature)
	End Function
	
	'特殊能力のレベル(必要条件を満たさないものを含む)
	Public Function AllFeatureLevel(ByRef Index As Object) As Double
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		
		With fd
			AllFeatureLevel = .Level
			If .Level = DEFAULT_LEVEL Then
				AllFeatureLevel = 1
			End If
		End With
		Exit Function
		
ErrorHandler: 
		AllFeatureLevel = 0
	End Function
	
	'特殊能力のレベルが指定されているか(必要条件を満たさないものを含む)
	Public Function AllFeatureLevelSpecified(ByRef Index As Object) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		
		If fd.Level <> DEFAULT_LEVEL Then
			AllFeatureLevelSpecified = True
		End If
		Exit Function
		
ErrorHandler: 
		AllFeatureLevelSpecified = False
	End Function
	
	'特殊能力のデータ(必要条件を満たさないものを含む)
	Public Function AllFeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		AllFeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		AllFeatureData = ""
	End Function
	
	'特殊能力にレベル指定がされている？(必要条件を満たさないものを含む)
	Public Function IsAllFeatureLevelSpecified(ByRef Index As Object) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		
		If fd.Level = DEFAULT_LEVEL Then
			IsAllFeatureLevelSpecified = False
		Else
			IsAllFeatureLevelSpecified = True
		End If
		Exit Function
		
ErrorHandler: 
		IsAllFeatureLevelSpecified = False
	End Function
	
	'特殊能力が必要条件を満たしているか
	Public Function IsFeatureActivated(ByRef Index As Object) As Boolean
		Dim fd, fd2 As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		
		For	Each fd2 In colFeature
			If fd Is fd2 Then
				IsFeatureActivated = True
				Exit Function
			End If
		Next fd2
		
		IsFeatureActivated = False
		Exit Function
		
ErrorHandler: 
		IsFeatureActivated = False
	End Function
	
	
	' === ユニットステータスの更新処理 ===
	
	'ユニットの各種パラメータを更新するサブルーチン
	'パラメータや武器、アビリティデータ等が変化する際には必ず呼び出す必要がある。
	Public Sub Update(Optional ByVal without_refresh As Boolean = False)
		Dim prev_wdata() As WeaponData
		Dim prev_wbullets() As Double
		Dim prev_adata() As AbilityData
		Dim prev_astocks() As Double
		Dim l, j, i, k, num As Short
		Dim ch, buf As String
		Dim fd As FeatureData
		Dim itm As Item
		Dim stype, sdata As String
		Dim slevel As Double
		Dim stype2, sdata2 As String
		Dim slevel2 As Double
		Dim wname, wnickname As String
		Dim wnskill, wclass, wtype, sname As String
		Dim fdata As String
		Dim flen As Short
		Dim found, flag As Boolean
		Dim flags() As Boolean
		Dim with_not As Boolean
		Dim false_count As Short
		Dim uadaption(4) As Short
		Dim hp_ratio, en_ratio As Double
		Dim ubitmap As String
		Dim cnd As Condition
		Dim pmorale As Short
		Dim is_stable, is_uncontrollable, is_invisible As Boolean
		
		'ＨＰとＥＮの値を記録
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		
		'ユニット用画像ファイル名を記録しておく
		ubitmap = Bitmap
		
		'非表示かどうか記録しておく
		is_invisible = IsFeatureAvailable("非表示")
		
		'制御不可がどうかを記録しておく
		is_uncontrollable = IsFeatureAvailable("制御不可")
		
		'不安定がどうかを記録しておく
		is_stable = IsFeatureAvailable("不安定")
		
TryAgain: 
		
		'アイテムが現在の形態で効力を発揮してくれるか判定
		For	Each itm In colItem
			With itm
				.Activated = .IsAvailable(Me)
			End With
		Next itm
		
		'ランクアップによるデータ変更
		Do While Data.IsFeatureAvailable("ランクアップ")
			With Data
				If Rank < .FeatureLevel("ランクアップ") Then
					Exit Do
				End If
				If Not IsNecessarySkillSatisfied(.FeatureNecessarySkill("ランクアップ")) Then
					Exit Do
				End If
				fdata = .FeatureData("ランクアップ")
			End With
			
			With UDList
				If Not .IsDefined(fdata) Then
					ErrorMessage(Name & "のランクアップ先ユニット「" & fdata & "」のデータが定義されていません")
					TerminateSRC()
				End If
				
				Data = .Item(fdata)
			End With
		Loop 
		
		'特殊能力を更新
		
		'まず特殊能力リストをクリア
		With colFeature
			For	Each fd In colFeature
				.Remove(1)
			Next fd
		End With
		
		'付加された特殊能力
		For	Each cnd In colCondition
			With cnd
				If .Lifetime <> 0 Then
					If Right(.Name, 2) = "付加" Then
						fd = New FeatureData
						fd.Name = Left(.Name, Len(.Name) - 2)
						fd.Level = .Level
						fd.StrData = .StrData
						colFeature.Add(fd, fd.Name)
					End If
				End If
			End With
		Next cnd
		AdditionalFeaturesNum = colFeature.Count()
		
		'ユニットデータで定義されている特殊能力
		AddFeatures((Data.colFeature))
		
		'アイテムで得られた特殊能力
		For i = 1 To CountItem
			With Item(i)
				If .Activated Then
					AddFeatures((.Data.colFeature), True)
				End If
			End With
		Next 
		
		'パイロットデータで定義されている特殊能力
		If CountPilot > 0 Then
			If IsFeatureAvailable("追加パイロット") Then
				'特殊能力を付加する前に必要技能が満たされているかどうか判定
				UpdateFeatures("追加パイロット")
			End If
			AddFeatures((MainPilot.Data.colFeature))
			For i = 2 To CountPilot
				AddFeatures((Pilot(i).Data.colFeature))
			Next 
			For i = 1 To CountSupport
				AddFeatures((Support(i).Data.colFeature))
			Next 
			If IsFeatureAvailable("追加サポート") Then
				'特殊能力を付加する前に必要技能が満たされているかどうか判定
				UpdateFeatures("追加サポート")
				If IsFeatureAvailable("追加サポート") Then
					AddFeatures((AdditionalSupport.Data.colFeature))
				End If
			End If
		End If
		
		'パイロット能力付加＆強化の効果をクリア
		i = 1
		Do While i <= CountCondition
			Select Case Right(Condition(i), 3)
				Case "付加２", "強化２"
					DeleteCondition(i)
				Case Else
					i = i + 1
			End Select
		Loop 
		
		'パイロット能力付加
		found = False
		flag = False
		ReDim flags(colFeature.Count())
AddSkills: 
		i = 1
		For	Each fd In colFeature
			If flags(i) Then
				GoTo NextFeature
			End If
			With fd
				Select Case .Name
					Case "パイロット能力付加"
						'必要技能を満たしている？
						If Not IsNecessarySkillSatisfied(.NecessarySkill) Then
							found = True
							GoTo NextFeature
						End If
						'必要条件を満たしている？
						If Not IsNecessarySkillSatisfied(.NecessaryCondition) Then
							found = True
							GoTo NextFeature
						End If
						flags(i) = True
						
						'能力指定が「"」で囲まれている場合は「"」を削除
						If Asc(.StrData) = 34 Then '"
							buf = Mid(.StrData, 2, Len(.StrData) - 2)
						Else
							buf = .StrData
						End If
						
						'付加する特殊能力の種類、レベル、データを解析
						If InStr(buf, "=") > 0 Then
							sdata = Mid(buf, InStr(buf, "=") + 1)
							buf = Left(buf, InStr(buf, "=") - 1)
							If InStr(buf, "Lv") > 0 Then
								stype = Left(buf, InStr(buf, "Lv") - 1)
								If IsNumeric(Mid(buf, InStr(buf, "Lv") + 2)) Then
									slevel = CDbl(Mid(buf, InStr(buf, "Lv") + 2))
								Else
									slevel = 1
								End If
							Else
								stype = buf
								slevel = DEFAULT_LEVEL
							End If
						Else
							sdata = ""
							If InStr(buf, "Lv") > 0 Then
								stype = Left(buf, InStr(buf, "Lv") - 1)
								If IsNumeric(Mid(buf, InStr(buf, "Lv") + 2)) Then
									slevel = CDbl(Mid(buf, InStr(buf, "Lv") + 2))
								Else
									slevel = 1
								End If
							Else
								stype = buf
								slevel = DEFAULT_LEVEL
							End If
						End If
						
						'エリアスが定義されている？
						If ALDList.IsDefined(stype) Then
							With ALDList.Item(stype)
								For j = 1 To .Count
									'エリアスの定義に従って特殊能力定義を置き換える
									stype2 = .AliasType(j)
									
									If LIndex(.AliasData(j), 1) = "解説" Then
										'特殊能力の解説
										If sdata <> "" Then
											stype2 = LIndex(sdata, 1)
										End If
										slevel2 = DEFAULT_LEVEL
										sdata2 = .AliasData(j)
									Else
										'通常の能力
										If .AliasLevelIsPlusMod(j) Then
											If slevel = DEFAULT_LEVEL Then
												slevel = 1
											End If
											slevel2 = slevel + .AliasLevel(j)
										ElseIf .AliasLevelIsMultMod(j) Then 
											If slevel = DEFAULT_LEVEL Then
												slevel = 1
											End If
											slevel2 = slevel * .AliasLevel(j)
										ElseIf slevel <> DEFAULT_LEVEL Then 
											slevel2 = slevel
										Else
											slevel2 = .AliasLevel(j)
										End If
										
										sdata2 = .AliasData(j)
										If sdata <> "" Then
											If InStr(sdata2, "非表示") <> 1 Then
												sdata2 = sdata & " " & ListTail(sdata2, LLength(sdata) + 1)
											End If
										End If
										
										If .AliasLevelIsPlusMod(j) Or .AliasLevelIsMultMod(j) Then
											sdata2 = LIndex(sdata2, 1) & "Lv" & VB6.Format(slevel) & " " & ListTail(sdata2, 2)
											sdata2 = Trim(sdata2)
										End If
									End If
									
									'属性使用不能攻撃により使用不能になった技能を封印する。
									If ConditionLifetime(stype2 & "使用不能") > 0 Then
										GoTo NextFeature
									End If
									AddCondition(stype2 & "付加２", -1, slevel2, sdata2)
								Next 
							End With
						Else
							'属性使用不能攻撃により使用不能になった技能を封印する。
							If ConditionLifetime(stype & "使用不能") > 0 Then
								GoTo NextFeature
							End If
							AddCondition(stype & "付加２", -1, slevel, sdata)
						End If
						
					Case "パイロット能力強化"
						'必要技能を満たしている？
						If Not IsNecessarySkillSatisfied(.NecessarySkill) Then
							found = True
							GoTo NextFeature
						End If
						'必要条件を満たしている？
						If Not IsNecessarySkillSatisfied(.NecessaryCondition) Then
							found = True
							GoTo NextFeature
						End If
						flags(i) = True
						
						'能力指定が「"」で囲まれている場合は「"」を削除
						If Asc(.StrData) = 34 Then '"
							buf = Mid(.StrData, 2, Len(.StrData) - 2)
						Else
							buf = .StrData
						End If
						
						'強化する特殊能力の種類、レベル、データを解析
						If InStr(buf, "=") > 0 Then
							sdata = Mid(buf, InStr(buf, "=") + 1)
							buf = Left(buf, InStr(buf, "=") - 1)
							If InStr(buf, "Lv") > 0 Then
								stype = Left(buf, InStr(buf, "Lv") - 1)
								If IsNumeric(Mid(buf, InStr(buf, "Lv") + 2)) Then
									slevel = CDbl(Mid(buf, InStr(buf, "Lv") + 2))
								Else
									slevel = 1
								End If
							Else
								stype = buf
								slevel = 1
							End If
						Else
							sdata = ""
							If InStr(buf, "Lv") > 0 Then
								stype = Left(buf, InStr(buf, "Lv") - 1)
								If IsNumeric(Mid(buf, InStr(buf, "Lv") + 2)) Then
									slevel = CDbl(Mid(buf, InStr(buf, "Lv") + 2))
								Else
									slevel = 1
								End If
							Else
								stype = buf
								slevel = 1
							End If
						End If
						
						'エリアスが定義されている？
						If ALDList.IsDefined(stype) Then
							With ALDList.Item(stype)
								For j = 1 To .Count
									'エリアスの定義に従って特殊能力定義を置き換える
									stype2 = .AliasType(j)
									
									'属性使用不能攻撃により使用不能になった技能を封印する。
									If ConditionLifetime(stype2 & "使用不能") > 0 Then
										GoTo NextFeature
									End If
									If LIndex(.AliasData(j), 1) = "解説" Then
										'特殊能力の解説
										If sdata <> "" Then
											stype2 = LIndex(sdata, 1)
										End If
										slevel2 = DEFAULT_LEVEL
										sdata2 = .AliasData(j)
										'属性使用不能攻撃により使用不能になった技能を封印する。
										If ConditionLifetime(stype2 & "使用不能") > 0 Then
											GoTo NextFeature
										End If
										AddCondition(stype2 & "付加２", -1, slevel2, sdata2)
									Else
										'通常の能力
										If .AliasLevelIsMultMod(j) Then
											If slevel = DEFAULT_LEVEL Then
												slevel = 1
											End If
											slevel2 = slevel * .AliasLevel(j)
										ElseIf slevel <> DEFAULT_LEVEL Then 
											slevel2 = slevel
										Else
											slevel2 = .AliasLevel(j)
										End If
										
										sdata2 = .AliasData(j)
										If sdata <> "" Then
											If InStr(sdata2, "非表示") <> 1 Then
												sdata2 = sdata & " " & ListTail(sdata2, LLength(sdata) + 1)
											End If
										End If
										
										'強化するレベルは累積する
										If IsConditionSatisfied(stype2 & "強化２") Then
											slevel2 = slevel2 + ConditionLevel(stype2 & "強化２")
											DeleteCondition(stype2 & "強化２")
										End If
										
										AddCondition(stype2 & "強化２", -1, slevel2, sdata2)
									End If
								Next 
							End With
						Else
							'強化するレベルは累積する
							If IsConditionSatisfied(stype & "強化２") Then
								slevel = slevel + ConditionLevel(stype & "強化２")
								DeleteCondition(stype & "強化２")
							End If
							
							AddCondition(stype & "強化２", -1, slevel, sdata)
						End If
				End Select
			End With
NextFeature: 
			i = i + 1
		Next fd
		'必要技能＆必要条件付きのパイロット能力付加＆強化がある場合は付加や強化の結果、
		'必要技能＆必要条件が満たされることがあるので一度だけやり直す
		If Not flag And found Then
			flag = True
			GoTo AddSkills
		End If
		
		'パイロット用特殊能力の付加＆強化が完了したので必要技能の判定が可能になった。
		UpdateFeatures()
		
		'アイテムが必要技能を満たすか再度チェック。
		found = False
		For	Each itm In colItem
			With itm
				If .Activated <> .IsAvailable(Me) Then
					found = True
					Exit For
				End If
			End With
		Next itm
		If found Then
			'アイテムの使用可否が変化したので最初からやり直す
			GoTo TryAgain
		End If
		
		'ランクアップするか再度チェック。
		With Data
			If .IsFeatureAvailable("ランクアップ") Then
				If Rank >= .FeatureLevel("ランクアップ") Then
					If IsNecessarySkillSatisfied(.FeatureNecessarySkill("ランクアップ")) Then
						'ランクアップが可能になったので最初からやり直す
						GoTo TryAgain
					End If
				End If
			End If
		End With
		
		If CountPilot > 0 Then
			'パイロット能力をアップデート
			For i = 2 To CountPilot
				Pilot(i).Update()
			Next 
			For i = 1 To CountSupport
				Support(i).Update()
			Next 
			
			'メインパイロットは他のパイロットのサポートを受ける関係上
			'最後にアップデートする
			Pilot(1).Update()
			If Not MainPilot Is Pilot(1) Then
				MainPilot.Update()
			End If
		End If
		
		'ユニット画像用ファイル名に変化がある場合はユニット画像を更新
		If BitmapID <> 0 Then
			If ubitmap <> Bitmap Then
				BitmapID = MakeUnitBitmap(Me)
				For i = 1 To CountOtherForm
					OtherForm(i).BitmapID = 0
				Next 
				If Not without_refresh Then
					If Status_Renamed = "出撃" Then
						If Not IsPictureVisible And MapFileName <> "" Then
							PaintUnitBitmap(Me)
						End If
					End If
				End If
			End If
		End If
		
		'ユニットの表示、非表示が切り替わった場合
		If is_invisible <> IsFeatureAvailable("非表示") Then
			If Status_Renamed = "出撃" Then
				If Not IsPictureVisible And MapFileName <> "" Then
					BitmapID = MakeUnitBitmap(Me)
					If IsFeatureAvailable("非表示") Then
						EraseUnitBitmap(x, y, Not without_refresh)
					Else
						If Not without_refresh Then
							PaintUnitBitmap(Me)
						End If
					End If
				End If
			End If
		End If
		
		'各種パラメータ
		With Data
			lngMaxHP = .HP + 200 * CInt(Rank)
			intMaxEN = .EN + 10 * Rank
			lngArmor = .Armor + 100 * CInt(Rank)
			intMobility = .Mobility + 5 * Rank
			intSpeed = .Speed
		End With
		
		'ボスランクによる修正
		If IsHero Or IsOptionDefined("等身大基準") Then
			Select Case BossRank
				Case 1
					lngMaxHP = lngMaxHP + Data.HP
				Case 2
					lngMaxHP = lngMaxHP + Data.HP + 10000
				Case 3
					lngMaxHP = lngMaxHP + Data.HP + 20000
				Case 4
					lngMaxHP = lngMaxHP + Data.HP + 40000
				Case 5
					lngMaxHP = lngMaxHP + Data.HP + 80000
			End Select
			
			If BossRank > 0 Then
				lngArmor = lngArmor + 200 * BossRank
			End If
		Else
			Select Case BossRank
				Case 1
					lngMaxHP = lngMaxHP + 0.5 * Data.HP
				Case 2
					lngMaxHP = lngMaxHP + Data.HP
				Case 3
					lngMaxHP = lngMaxHP + Data.HP + 10000
				Case 4
					lngMaxHP = lngMaxHP + Data.HP + 20000
				Case 5
					lngMaxHP = lngMaxHP + Data.HP + 40000
			End Select
			
			If IsOptionDefined("BossRank装甲修正低下") Then
				If BossRank > 0 Then
					lngArmor = lngArmor + 300 * BossRank
				End If
			Else
				Select Case BossRank
					Case 1
						lngArmor = lngArmor + 300
					Case 2
						lngArmor = lngArmor + 600
					Case 3
						lngArmor = lngArmor + 1000
					Case 4
						lngArmor = lngArmor + 1500
					Case 5
						lngArmor = lngArmor + 2500
				End Select
			End If
		End If
		If BossRank > 0 Then
			intMaxEN = intMaxEN + 20 * BossRank
			intMobility = intMobility + 5 * BossRank
		End If
		
		'ＨＰ成長オプション
		If IsOptionDefined("ＨＰ成長") Then
			If CountPilot > 0 Then
				lngMaxHP = MinLng((lngMaxHP / 100) * (100 + MainPilot.Level), 9999999)
			End If
		End If
		
		'ＥＮ成長オプション
		If IsOptionDefined("ＥＮ成長") Then
			If CountPilot > 0 Then
				intMaxEN = MinLng((intMaxEN / 100) * (100 + MainPilot.Level), 9999)
			End If
		End If
		
		'特殊能力による修正
		If CountPilot > 0 Then
			pmorale = MainPilot.Morale
		Else
			pmorale = 100
		End If
		For	Each fd In colFeature
			With fd
				Select Case .Name
					'固定値による強化
					Case "ＨＰ強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngMaxHP = lngMaxHP + 200 * .Level
						End If
					Case "ＥＮ強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMaxEN = intMaxEN + 10 * .Level
						End If
					Case "装甲強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngArmor = lngArmor + 100 * .Level
						End If
					Case "運動性強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMobility = intMobility + 5 * .Level
						End If
					Case "移動力強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intSpeed = intSpeed + .Level
						End If
						'割合による強化
					Case "ＨＰ割合強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngMaxHP = lngMaxHP + Data.HP * .Level \ 20
						End If
					Case "ＥＮ割合強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMaxEN = intMaxEN + Data.EN * .Level \ 20
						End If
					Case "装甲割合強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngArmor = lngArmor + Data.Armor * .Level \ 20
						End If
					Case "運動性割合強化"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMobility = intMobility + Data.Mobility * .Level \ 20
						End If
				End Select
			End With
		Next fd
		
		'アイテムによる修正
		For	Each itm In colItem
			With itm
				If .Activated Then
					lngMaxHP = lngMaxHP + .HP
					intMaxEN = intMaxEN + .EN
					lngArmor = lngArmor + .Armor
					intMobility = intMobility + .Mobility
					intSpeed = intSpeed + .Speed
				End If
			End With
		Next itm
		
		'装備している「Ｖ−ＵＰ=ユニット」アイテムによる修正
		num = 0
		If IsConditionSatisfied("Ｖ−ＵＰ") Then
			Select Case FeatureData("Ｖ−ＵＰ")
				Case "全", "ユニット"
					num = num + 1
			End Select
		End If
		For	Each itm In colItem
			With itm
				If .IsFeatureAvailable("Ｖ−ＵＰ") Then
					Select Case .FeatureData("Ｖ−ＵＰ")
						Case "全", "ユニット"
							num = num + 1
					End Select
				End If
			End With
		Next itm
		If CountPilot > 0 Then
			With MainPilot.Data
				If .IsFeatureAvailable("Ｖ−ＵＰ") Then
					Select Case .FeatureData("Ｖ−ＵＰ")
						Case "全", "ユニット"
							num = num + 1
					End Select
				End If
			End With
		End If
		If num > 0 Then
			With Data
				lngMaxHP = lngMaxHP + 100 * num * (.ItemNum + 1)
				intMaxEN = intMaxEN + 20 * num * .ItemNum
				lngArmor = lngArmor + 50 * num * .ItemNum
				intMobility = intMobility + 5 * num * .ItemNum
			End With
		End If
		
		'追加移動力
		If IsFeatureAvailable("追加移動力") Then
			For	Each fd In colFeature
				With fd
					If .Name = "追加移動力" Then
						If Area = LIndex(.StrData, 2) Then
							intSpeed = intSpeed + .Level
						End If
					End If
				End With
			Next fd
			intSpeed = MaxLng(intSpeed, 0)
		End If
		
		'上限値を超えないように
		lngMaxHP = MinLng(lngMaxHP, 9999999)
		intMaxEN = MinLng(intMaxEN, 9999)
		lngArmor = MinLng(lngArmor, 99999)
		intMobility = MinLng(intMobility, 9999)
		intSpeed = MinLng(intSpeed, 99)
		
		'ＨＰ、ＥＮの最大値の変動に対応
		HP = MaxHP * hp_ratio / 100
		EN = MaxEN * en_ratio / 100
		
		'切り下げの結果ＨＰが0になることを防ぐ
		If hp_ratio > 0 Then
			If HP = 0 Then
				HP = 1
			End If
		End If
		
		'地形適応
		For i = 1 To 4
			Select Case Mid(Data.Adaption, i, 1)
				Case "S"
					uadaption(i) = 5
				Case "A"
					uadaption(i) = 4
				Case "B"
					uadaption(i) = 3
				Case "C"
					uadaption(i) = 2
				Case "D"
					uadaption(i) = 1
				Case "E", "-"
					uadaption(i) = 0
			End Select
		Next 
		
		'移動タイプ追加による地形適応修正
		If IsFeatureAvailable("空中移動") Then
			uadaption(1) = MaxLng(uadaption(1), 4)
		End If
		If IsFeatureAvailable("陸上移動") Then
			uadaption(2) = MaxLng(uadaption(2), 4)
		End If
		If IsFeatureAvailable("水中移動") Then
			uadaption(3) = MaxLng(uadaption(3), 4)
		End If
		If IsFeatureAvailable("宇宙移動") Then
			uadaption(4) = MaxLng(uadaption(4), 4)
		End If
		
		'地形適応変更能力による修正
		For	Each fd In colFeature
			With fd
				Select Case .Name
					Case "地形適応変更"
						For i = 1 To 4
							num = StrToLng(LIndex(.StrData, i))
							If num > 0 Then
								If uadaption(i) < 4 Then
									uadaption(i) = uadaption(i) + num
									'地形適応はAより高くはならない
									If uadaption(i) > 4 Then
										uadaption(i) = 4
									End If
								End If
							Else
								uadaption(i) = uadaption(i) + num
							End If
						Next 
					Case "地形適応固定変更"
						For i = 1 To 4
							num = StrToLng(LIndex(.StrData, i))
							If LIndex(.StrData, 5) = "強制" Then
								' 強制変更の場合
								If num >= 0 And num <= 5 Then
									uadaption(i) = num
								End If
							Else
								' 高いほうを優先する場合
								If num > uadaption(i) And num <= 5 Then
									uadaption(i) = num
								End If
							End If
						Next 
				End Select
			End With
		Next fd
		strAdaption = ""
		For i = 1 To 4
			Select Case uadaption(i)
				Case Is >= 5
					strAdaption = strAdaption & "S"
				Case 4
					strAdaption = strAdaption & "A"
				Case 3
					strAdaption = strAdaption & "B"
				Case 2
					strAdaption = strAdaption & "C"
				Case 1
					strAdaption = strAdaption & "D"
				Case Is <= 0
					strAdaption = strAdaption & "-"
			End Select
		Next 
		
		'空中に留まることが出来るかチェック
		If Status_Renamed = "出撃" And Area = "空中" And Not IsTransAvailable("空") Then
			'地上(水中)に戻す
			Select Case TerrainClass(x, y)
				Case "陸", "屋内"
					Area = "地上"
				Case "水", "深水"
					If IsTransAvailable("水上") Then
						Area = "水上"
					Else
						Area = "水中"
					End If
			End Select
			If Not without_refresh Then
				If Not IsPictureVisible And MapFileName <> "" Then
					PaintUnitBitmap(Me)
				End If
			End If
		End If
		
		'攻撃への耐性を更新
		strAbsorb = ""
		strImmune = ""
		strResist = ""
		strWeakness = ""
		strEffective = ""
		strSpecialEffectImmune = ""
		'特殊能力によって得られた耐性
		For	Each fd In colFeature
			With fd
				Select Case .Name
					Case "吸収"
						strAbsorb = strAbsorb & .StrData
					Case "無効化"
						strImmune = strImmune & .StrData
					Case "耐性"
						strResist = strResist & .StrData
					Case "弱点"
						strWeakness = strWeakness & .StrData
					Case "有効"
						strEffective = strEffective & .StrData
					Case "特殊効果無効化"
						strSpecialEffectImmune = strSpecialEffectImmune & .StrData
				End Select
			End With
		Next fd
		'弱点、有効付加属性攻撃による弱点、有効の付加
		For i = 1 To CountCondition
			If ConditionLifetime(i) <> 0 Then
				ch = Condition(i)
				Select Case Right(ch, 6)
					Case "属性弱点付加"
						strWeakness = strWeakness & Left(ch, Len(ch) - 6)
					Case "属性有効付加"
						strEffective = strEffective & Left(ch, Len(ch) - 6)
				End Select
			End If
		Next 
		'属性のダブりをなくす
		buf = ""
		For i = 1 To Len(strAbsorb)
			ch = GetClassBundle(strAbsorb, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strAbsorb = buf
		buf = ""
		For i = 1 To Len(strImmune)
			ch = GetClassBundle(strImmune, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strImmune = buf
		buf = ""
		For i = 1 To Len(strResist)
			ch = GetClassBundle(strResist, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strResist = buf
		buf = ""
		For i = 1 To Len(strWeakness)
			ch = GetClassBundle(strWeakness, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strWeakness = buf
		buf = ""
		For i = 1 To Len(strEffective)
			ch = GetClassBundle(strEffective, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strEffective = buf
		buf = ""
		For i = 1 To Len(strSpecialEffectImmune)
			ch = GetClassBundle(strSpecialEffectImmune, i)
			If InStrNotNest(buf, ch) = 0 Then
				buf = buf & ch
			End If
		Next 
		strSpecialEffectImmune = buf
		
		'武器データを更新
		ReDim prev_wdata(UBound(WData))
		ReDim prev_wbullets(UBound(WData))
		For i = 1 To UBound(WData)
			prev_wdata(i) = WData(i)
			prev_wbullets(i) = dblBullet(i)
		Next 
		With Data
			ReDim WData(.CountWeapon)
			For i = 1 To .CountWeapon
				WData(i) = .Weapon(i)
			Next 
		End With
		If CountPilot > 0 Then
			With MainPilot.Data
				For i = 1 To .CountWeapon
					ReDim Preserve WData(UBound(WData) + 1)
					WData(UBound(WData)) = .Weapon(i)
				Next 
			End With
			For i = 2 To CountPilot
				With Pilot(i).Data
					For j = 1 To .CountWeapon
						ReDim Preserve WData(UBound(WData) + 1)
						WData(UBound(WData)) = .Weapon(j)
					Next 
				End With
			Next 
			For i = 1 To CountSupport
				With Support(i).Data
					For j = 1 To .CountWeapon
						ReDim Preserve WData(UBound(WData) + 1)
						WData(UBound(WData)) = .Weapon(j)
					Next 
				End With
			Next 
			If IsFeatureAvailable("追加サポート") Then
				With AdditionalSupport.Data
					For i = 1 To .CountWeapon
						ReDim Preserve WData(UBound(WData) + 1)
						WData(UBound(WData)) = .Weapon(i)
					Next 
				End With
			End If
		End If
		For	Each itm In colItem
			With itm
				If .Activated Then
					For i = 1 To .CountWeapon
						ReDim Preserve WData(UBound(WData) + 1)
						WData(UBound(WData)) = .Weapon(i)
					Next 
				End If
			End With
		Next itm
		
		'武器属性を更新
		ReDim strWeaponClass(CountWeapon)
		For i = 1 To CountWeapon
			strWeaponClass(i) = Weapon(i).Class_Renamed
		Next 
		Dim hidden_attr As String
		Dim skipped As Boolean
		If IsFeatureAvailable("攻撃属性") Then
			For i = 1 To CountWeapon
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				'非表示の属性がある場合は一旦抜き出す
				If InStrNotNest(wclass, "|") > 0 Then
					strWeaponClass(i) = Left(wclass, InStrNotNest(wclass, "|") - 1)
					hidden_attr = Mid(wclass, InStrNotNest(wclass, "|") + 1)
				Else
					hidden_attr = ""
				End If
				
				For j = 1 To CountFeature
					If Feature(j) = "攻撃属性" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						
						If flen = 1 Then
							'武器指定がない場合はすべての武器に属性を付加
							flag = True
							k = 2
						ElseIf LIndex(fdata, 1) = "非表示" Then 
							'非表示指定がある場合 (武器指定がある場合を含む)
							If flen = 2 Then
								'武器指定無し
								flag = True
							Else
								'武器指定あり
								flag = False
							End If
							k = 3
						Else
							'武器指定がある場合
							flag = False
							k = 2
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						Do While k <= flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = True
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											sname = LIndex(wnskill, l)
											If InStr(sname, "Lv") > 0 Then
												sname = Left(sname, InStr(sname, "Lv") - 1)
											End If
											If sname = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
							
							k = k + 1
						Loop 
						
						'属性を追加
						If flag Or false_count = 0 Then
							buf = LIndex(fdata, 1)
							If buf = "非表示" Then
								'非表示の属性の場合
								hidden_attr = hidden_attr & LIndex(fdata, 2)
							Else
								'属性が重複しないように付加
								skipped = False
								For k = 1 To Len(buf)
									ch = GetClassBundle(buf, k)
									
									If Not IsNumeric(ch) And ch <> "L" And ch <> "." Then
										skipped = False
									End If
									
									If (InStrNotNest(strWeaponClass(i), ch) = 0 Or IsNumeric(ch) Or ch = "L" Or ch = ".") And Not skipped Then
										If ch = "魔" Then
											'魔属性を付加する場合は武器を魔法武器化する
											l = InStrNotNest(strWeaponClass(i), "武")
											l = MaxLng(InStrNotNest(strWeaponClass(i), "突"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "接"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "銃"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "実"), l)
											If l > 0 Then
												strWeaponClass(i) = Left(strWeaponClass(i), l - 1) & ch & Mid(strWeaponClass(i), l)
											Else
												strWeaponClass(i) = strWeaponClass(i) & ch
											End If
										Else
											strWeaponClass(i) = strWeaponClass(i) & ch
										End If
									Else
										skipped = True
									End If
								Next 
							End If
						End If
					End If
				Next 
				
				'非表示の属性を追加
				If Len(hidden_attr) > 0 Then
					strWeaponClass(i) = strWeaponClass(i) & "|" & hidden_attr
				End If
			Next 
		End If
		
		'武器攻撃力を更新
		ReDim lngWeaponPower(CountWeapon)
		
		'装備している「Ｖ−ＵＰ=武器」アイテムの個数をカウントしておく
		num = 0
		If IsConditionSatisfied("Ｖ−ＵＰ") Then
			Select Case FeatureData("Ｖ−ＵＰ")
				Case "全", "武器"
					num = num + 1
			End Select
		End If
		For	Each itm In colItem
			With itm
				If .Activated Then
					If .IsFeatureAvailable("Ｖ−ＵＰ") Then
						Select Case .FeatureData("Ｖ−ＵＰ")
							Case "全", "武器"
								num = num + 1
						End Select
					End If
				End If
			End With
		Next itm
		If CountPilot > 0 Then
			With MainPilot.Data
				If .IsFeatureAvailable("Ｖ−ＵＰ") Then
					Select Case .FeatureData("Ｖ−ＵＰ")
						Case "全", "武器"
							num = num + 1
					End Select
				End If
			End With
		End If
		num = num * Data.ItemNum
		
		For i = 1 To CountWeapon
			lngWeaponPower(i) = Weapon(i).Power
			
			'もともと攻撃力が0の武器は0に固定
			If lngWeaponPower(i) = 0 Then
				GoTo NextWeapon
			End If
			
			'武器強化による修正
			If IsFeatureAvailable("武器強化") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "武器強化" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							If IsWeaponClassifiedAs(i, "固") Then
								'ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
								'いずれかで行われた場合にのみ強化
								If wtype = "固" Or wname = wtype Or wnickname = wtype Then
									found = True
								End If
							Else
								Select Case wtype
									Case "全"
										found = True
									Case "物"
										If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
											found = True
										End If
									Case Else
										If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
											found = True
										Else
											'必要技能による指定
											For l = 1 To LLength(wnskill)
												sname = LIndex(wnskill, l)
												If InStr(sname, "Lv") > 0 Then
													sname = Left(sname, InStr(sname, "Lv") - 1)
												End If
												If sname = wtype Then
													found = True
													Exit For
												End If
											Next 
										End If
								End Select
							End If
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + 100 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			' ADD START MARGE
			'武器割合強化による修正
			If IsFeatureAvailable("武器割合強化") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "武器割合強化" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							If IsWeaponClassifiedAs(i, "固") Then
								'ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
								'いずれかで行われた場合にのみ強化
								If wtype = "固" Or wname = wtype Or wnickname = wtype Then
									found = True
								End If
							Else
								Select Case wtype
									Case "全"
										found = True
									Case "物"
										If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
											found = True
										End If
									Case Else
										If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
											found = True
										Else
											'必要技能による指定
											For l = 1 To LLength(wnskill)
												sname = LIndex(wnskill, l)
												If InStr(sname, "Lv") > 0 Then
													sname = Left(sname, InStr(sname, "Lv") - 1)
												End If
												If sname = wtype Then
													found = True
													Exit For
												End If
											Next 
										End If
								End Select
							End If
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + Weapon(i).Power * FeatureLevel(j) \ 20
						End If
					End If
				Next 
			End If
			' ADD END MARGE
			
			'ダメージ固定武器
			If IsWeaponClassifiedAs(i, "固") Then
				GoTo NextWeapon
			End If
			
			If IsWeaponClassifiedAs(i, "Ｒ") Then
				'低成長型の攻撃
				If IsWeaponLevelSpecified(i, "Ｒ") Then
					'レベル設定されている場合、増加量をレベル×１０×ランクにする
					lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "Ｒ") * CInt(Rank + num)
					'オ・シ・超と併用した場合
					If IsWeaponClassifiedAs(i, "オ") Or IsWeaponClassifiedAs(i, "超") Or IsWeaponClassifiedAs(i, "シ") Then
						lngWeaponPower(i) = lngWeaponPower(i) + 10 * (10 - WeaponLevel(i, "Ｒ")) * CInt(Rank + num)
						
						'オーラ技
						If IsWeaponClassifiedAs(i, "オ") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "Ｒ") * AuraLevel()
						End If
						
						'サイキック攻撃
						If IsWeaponClassifiedAs(i, "超") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "Ｒ") * PsychicLevel()
						End If
						
						'同調率対象攻撃
						If IsWeaponClassifiedAs(i, "シ") Then
							If CountPilot() > 0 Then
								If MainPilot.SynchroRate > 0 Then
									lngWeaponPower(i) = lngWeaponPower(i) + 15 * WeaponLevel(i, "Ｒ") * (SyncLevel() - 50) \ 10
								End If
							End If
						End If
					End If
				Else
					'レベル指定されていない場合は今までどおりランク×５０
					lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
					
					'オ・シ・超と併用した場合
					If IsWeaponClassifiedAs(i, "オ") Or IsWeaponClassifiedAs(i, "超") Or IsWeaponClassifiedAs(i, "シ") Then
						lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
						
						'オーラ技
						If IsWeaponClassifiedAs(i, "オ") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 50 * AuraLevel()
						End If
						
						'サイキック攻撃
						If IsWeaponClassifiedAs(i, "超") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 50 * PsychicLevel()
						End If
						
						'同調率対象攻撃
						If IsWeaponClassifiedAs(i, "シ") Then
							If CountPilot() > 0 Then
								If MainPilot.SynchroRate > 0 Then
									lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50) \ 2
								End If
							End If
						End If
					End If
				End If
			ElseIf IsWeaponClassifiedAs(i, "改") Then 
				'改属性＝オ・超・シ属性を無視したＲ属性
				If IsWeaponLevelSpecified(i, "改") Then
					'レベル設定されている場合、増加量をレベル×１０×ランクにする
					lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "改") * (Rank + num)
				Else
					'レベル指定がない場合、増加量は５０×ランク
					lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
				End If
				
				'オーラ技
				If IsWeaponClassifiedAs(i, "オ") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * AuraLevel()
				End If
				
				'サイキック攻撃
				If IsWeaponClassifiedAs(i, "超") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * PsychicLevel()
				End If
				
				'同調率対象攻撃
				If IsWeaponClassifiedAs(i, "シ") Then
					If CountPilot() > 0 Then
						If MainPilot.SynchroRate > 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50)
						End If
					End If
				End If
			Else
				'Ｒ、改属性が両方ともない場合
				lngWeaponPower(i) = lngWeaponPower(i) + 100 * CInt(Rank + num)
				
				'オーラ技
				If IsWeaponClassifiedAs(i, "オ") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * AuraLevel()
				End If
				
				'サイキック攻撃
				If IsWeaponClassifiedAs(i, "超") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * PsychicLevel()
				End If
				
				'同調率対象攻撃
				If IsWeaponClassifiedAs(i, "シ") Then
					If CountPilot() > 0 Then
						If MainPilot.SynchroRate > 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50)
						End If
					End If
				End If
			End If
			
			'ボスランクによる修正
			If BossRank > 0 Then
				lngWeaponPower(i) = lngWeaponPower(i) + MinLng(100 * BossRank, 300)
			End If
			
			'攻撃力の最高値は99999
			If lngWeaponPower(i) > 99999 Then
				lngWeaponPower(i) = 99999
			End If
			
			'最低値は1
			If lngWeaponPower(i) <= 0 Then
				lngWeaponPower(i) = 1
			End If
NextWeapon: 
		Next 
		
		'武器射程を更新
		ReDim intWeaponMaxRange(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponMaxRange(i) = Weapon(i).MaxRange
			
			'最大射程がもともと１ならそれ以上変化しない
			If intWeaponMaxRange(i) = 1 Then
				GoTo NextWeapon2
			End If
			
			'思念誘導攻撃のＮＴ能力による射程延長
			If InStrNotNest(strWeaponClass(i), "サ") > 0 Then
				If CountPilot() > 0 Then
					With MainPilot
						intWeaponMaxRange(i) = intWeaponMaxRange(i) + .SkillLevel("超感覚") \ 4 + .SkillLevel("知覚強化") \ 4
					End With
				End If
			End If
			
			'マップ攻撃には適用されない
			If InStrNotNest(strWeaponClass(i), "Ｍ") > 0 Then
				GoTo NextWeapon2
			End If
			
			'接近戦武器には適用されない
			If InStrNotNest(strWeaponClass(i), "武") > 0 Or InStrNotNest(strWeaponClass(i), "突") > 0 Or InStrNotNest(strWeaponClass(i), "接") > 0 Then
				GoTo NextWeapon2
			End If
			
			'有線式誘導攻撃には適用されない
			If InStrNotNest(strWeaponClass(i), "有") > 0 Then
				GoTo NextWeapon2
			End If
			
			'射程延長による修正
			If IsFeatureAvailable("射程延長") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "射程延長" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = True
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											sname = LIndex(wnskill, l)
											If InStr(sname, "Lv") > 0 Then
												sname = Left(sname, InStr(sname, "Lv") - 1)
											End If
											If sname = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponMaxRange(i) = intWeaponMaxRange(i) + FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'最低値は1
			If intWeaponMaxRange(i) <= 0 Then
				intWeaponMaxRange(i) = 1
			End If
NextWeapon2: 
		Next 
		
		'武器命中率を更新
		ReDim intWeaponPrecision(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponPrecision(i) = Weapon(i).Precision
			
			'武器強化による修正
			If IsFeatureAvailable("命中率強化") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "命中率強化" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = True
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											sname = LIndex(wnskill, l)
											If InStr(sname, "Lv") > 0 Then
												sname = Left(sname, InStr(sname, "Lv") - 1)
											End If
											If sname = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponPrecision(i) = intWeaponPrecision(i) + 5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
		Next 
		
		'武器のＣＴ率を更新
		ReDim intWeaponCritical(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponCritical(i) = Weapon(i).Critical
			
			'ＣＴ率強化による修正
			If IsFeatureAvailable("ＣＴ率強化") And IsNormalWeapon(i) Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "ＣＴ率強化" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = Not with_not
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											sname = LIndex(wnskill, l)
											If InStr(sname, "Lv") > 0 Then
												sname = Left(sname, InStr(sname, "Lv") - 1)
											End If
											If sname = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponCritical(i) = intWeaponCritical(i) + 5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'特殊効果発動率強化による修正
			If IsFeatureAvailable("特殊効果発動率強化") And Not IsNormalWeapon(i) Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "特殊効果発動率強化" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器を強化
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = True
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											buf = LIndex(wnskill, l)
											If InStr(buf, "Lv") > 0 Then
												buf = Left(buf, InStr(buf, "Lv") - 1)
											End If
											If buf = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponCritical(i) = intWeaponCritical(i) + 5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
		Next 
		
		'最大弾数を更新
		ReDim intMaxBullet(CountWeapon)
		'UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim rate_Renamed As Double
		For i = 1 To CountWeapon
			
			intMaxBullet(i) = Weapon(i).Bullet
			
			'最大弾数の増加率
			rate_Renamed = 0
			
			'ボスランクによる修正
			If intBossRank > 0 Then
				rate_Renamed = 0.2 * BossRank
			End If
			
			'最大弾数増加による修正
			If IsFeatureAvailable("最大弾数増加") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "最大弾数増加" Then
						fdata = FeatureData(j)
						
						'「"」を除去
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'武器指定がない場合はすべての武器の弾数を増加
						If flen = 0 Then
							flag = True
						End If
						
						'武器指定がある場合はそれぞれの指定をチェック
						false_count = 0
						For k = 1 To flen
							wtype = LIndex(fdata, k)
							
							If Left(wtype, 1) = "!" Then
								wtype = Mid(wtype, 2)
								with_not = True
							Else
								with_not = False
							End If
							
							found = False
							Select Case wtype
								Case "全"
									found = True
								Case "物"
									If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
										found = True
									End If
								Case Else
									If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
										found = True
									Else
										For l = 1 To LLength(wnskill)
											sname = LIndex(wnskill, l)
											If InStr(sname, "Lv") > 0 Then
												sname = Left(sname, InStr(sname, "Lv") - 1)
											End If
											If sname = wtype Then
												found = True
												Exit For
											End If
										Next 
									End If
							End Select
							
							If with_not Then
								'!指定あり
								If found Then
									'条件を満たした場合は適用しない
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!指定無しの条件を満たした
								flag = True
							Else
								'!指定無しの条件を満たさず
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							rate_Renamed = rate_Renamed + 0.5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'増加率に合わせて弾数を修正
			intMaxBullet(i) = (1 + rate_Renamed) * intMaxBullet(i)
			
			'最大値は99
			If intMaxBullet(i) > 99 Then
				intMaxBullet(i) = 99
			End If
			'最低値は0
			If intMaxBullet(i) < 0 Then
				intMaxBullet(i) = 0
			End If
		Next 
		
		'弾数を更新
		ReDim Preserve dblBullet(CountWeapon)
		ReDim flags(UBound(prev_wdata))
		For i = 1 To CountWeapon
			dblBullet(i) = 1
			For j = 1 To UBound(prev_wdata)
				If WData(i) Is prev_wdata(j) And Not flags(j) Then
					dblBullet(i) = prev_wbullets(j)
					flags(j) = True
					Exit For
				End If
			Next 
		Next 
		
		'アビリティデータを更新
		ReDim prev_adata(UBound(adata))
		ReDim prev_astocks(UBound(adata))
		For i = 1 To UBound(adata)
			prev_adata(i) = adata(i)
			prev_astocks(i) = dblStock(i)
		Next 
		With Data
			ReDim adata(.CountAbility)
			For i = 1 To .CountAbility
				adata(i) = .Ability(i)
			Next 
		End With
		If CountPilot > 0 Then
			With MainPilot.Data
				For i = 1 To .CountAbility
					ReDim Preserve adata(UBound(adata) + 1)
					adata(UBound(adata)) = .Ability(i)
				Next 
			End With
			For i = 2 To CountPilot
				With Pilot(i).Data
					For j = 1 To .CountAbility
						ReDim Preserve adata(UBound(adata) + 1)
						adata(UBound(adata)) = .Ability(j)
					Next 
				End With
			Next 
			For i = 1 To CountSupport
				With Support(i).Data
					For j = 1 To .CountAbility
						ReDim Preserve adata(UBound(adata) + 1)
						adata(UBound(adata)) = .Ability(j)
					Next 
				End With
			Next 
			If IsFeatureAvailable("追加サポート") Then
				With AdditionalSupport.Data
					For i = 1 To .CountAbility
						ReDim Preserve adata(UBound(adata) + 1)
						adata(UBound(adata)) = .Ability(i)
					Next 
				End With
			End If
		End If
		For	Each itm In colItem
			With itm
				If .Activated Then
					For i = 1 To .CountAbility
						ReDim Preserve adata(UBound(adata) + 1)
						adata(UBound(adata)) = .Ability(i)
					Next 
				End If
			End With
		Next itm
		
		'使用回数を更新
		ReDim Preserve dblStock(CountAbility)
		ReDim flags(UBound(prev_adata))
		For i = 1 To CountAbility
			dblStock(i) = 1
			For j = 1 To UBound(prev_adata)
				If adata(i) Is prev_adata(j) And Not flags(j) Then
					dblStock(i) = prev_astocks(j)
					flags(j) = True
					Exit For
				End If
			Next 
		Next 
		
		If Status_Renamed <> "出撃" Then
			Exit Sub
		End If
		
		'制御不能？
		If IsFeatureAvailable("制御不可") Then
			If Not is_uncontrollable Then
				AddCondition("暴走", -1)
			End If
		Else
			If is_uncontrollable Then
				If IsConditionSatisfied("暴走") Then
					DeleteCondition("暴走")
				End If
			End If
		End If
		
		'不安定？
		If IsFeatureAvailable("不安定") Then
			If Not is_stable Then
				If HP <= MaxHP \ 4 Then
					AddCondition("暴走", -1)
				End If
			End If
		Else
			If is_stable Then
				If IsConditionSatisfied("暴走") Then
					DeleteCondition("暴走")
				End If
			End If
		End If
	End Sub
	
	'特殊能力を登録
	Private Sub AddFeatures(ByRef fdc As Collection, Optional ByVal is_item As Boolean = False)
		Dim fd As FeatureData
		
		If fdc Is Nothing Then
			Exit Sub
		End If
		
		For	Each fd In fdc
			With fd
				'アイテムで指定された下記の能力はアイテムそのものの属性なので
				'ユニット側には追加しない
				If is_item Then
					Select Case .Name
						Case "必要技能", "不必要技能", "表示", "非表示", "呪い"
							GoTo NextFeature
					End Select
				End If
				
				'封印されている？
				If IsDisabled(.Name) Or IsDisabled(LIndex(.StrData, 1)) Then
					GoTo NextFeature
				End If
				
				'既にその能力が登録されている？
				If Not IsFeatureRegistered(.Name) Then
					colFeature.Add(fd, .Name)
				Else
					colFeature.Add(fd, .Name & ":" & VB6.Format(colFeature.Count()))
				End If
			End With
NextFeature: 
		Next fd
	End Sub
	
	'特殊能力を登録済み？
	Private Function IsFeatureRegistered(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = colFeature.Item(fname)
		IsFeatureRegistered = True
		Exit Function
		
ErrorHandler: 
		IsFeatureRegistered = False
	End Function
	
	'特殊能力を登録済み？(必要条件を満たさない特殊能力を含む)
	Private Function IsAllFeatureRegistered(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = colAllFeature.Item(fname)
		IsAllFeatureRegistered = True
		Exit Function
		
ErrorHandler: 
		IsAllFeatureRegistered = False
	End Function
	
	'特殊能力が必要条件を満たしているかどうか判定し、満たしていない能力を削除する
	'fnameが指定された場合、指定された特殊能力に対してのみ必要技能を判定
	Private Sub UpdateFeatures(Optional ByVal fname As String = "")
		Dim fd As FeatureData
		Dim farray() As FeatureData
		Dim i As Short
		Dim found As Boolean
		
		If fname <> "" Then
			'必要技能＆条件を満たしてない特殊能力を削除。
			found = False
			i = 1
			With colFeature
				Do While i <= .Count()
					'必要技能を満たしている？
					'UPGRADE_WARNING: オブジェクト colFeature.Item(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If fname = .Item(i).Name Then
						'UPGRADE_WARNING: オブジェクト colFeature.Item().NecessaryCondition の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						'UPGRADE_WARNING: オブジェクト colFeature.Item().NecessarySkill の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If Not IsNecessarySkillSatisfied(.Item(i).NecessarySkill) Or Not IsNecessarySkillSatisfied(.Item(i).NecessaryCondition) Then
							'必要技能＆条件を満たしていないので削除
							.Remove(i)
							found = True
						Else
							i = i + 1
						End If
					Else
						i = i + 1
					End If
				Loop 
			End With
		Else
			'必要技能を満たしてない特殊能力を削除。
			found = False
			i = 1
			With colFeature
				Do While i <= .Count()
					'必要技能を満たしている？
					'UPGRADE_WARNING: オブジェクト colFeature.Item().NecessarySkill の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If Not IsNecessarySkillSatisfied(.Item(i).NecessarySkill) Then
						'必要技能を満たしていないので削除
						.Remove(i)
						found = True
					Else
						i = i + 1
					End If
				Loop 
			End With
			
			'必要条件を適用する前の特殊能力を保存
			With colAllFeature
				For	Each fd In colAllFeature
					.Remove(1)
				Next fd
				For	Each fd In colFeature
					If Not IsAllFeatureRegistered((fd.Name)) Then
						.Add(fd, fd.Name)
					Else
						.Add(fd, fd.Name & ":" & VB6.Format(.Count() + 1))
					End If
				Next fd
			End With
			
			'必要条件を満たしてない特殊能力を削除。
			i = 1
			With colFeature
				Do While i <= .Count()
					'必要条件を満たしている？
					'UPGRADE_WARNING: オブジェクト colFeature.Item().NecessaryCondition の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If Not IsNecessarySkillSatisfied(.Item(i).NecessaryCondition) Then
						'必要条件を満たしていないので削除
						.Remove(i)
						found = True
					Else
						i = i + 1
					End If
				Loop 
			End With
		End If
		
		'特殊能力が削除された場合、特殊能力の保持判定が正しく行われるように特殊能力を
		'登録しなおす必要がある。
		If found Then
			With colFeature
				ReDim farray(.Count())
				For i = 1 To .Count()
					farray(i) = .Item(i)
				Next 
				For i = 1 To .Count()
					.Remove(1)
				Next 
				For i = 1 To UBound(farray)
					If Not IsFeatureRegistered((farray(i).Name)) Then
						.Add(farray(i), farray(i).Name)
					Else
						.Add(farray(i), farray(i).Name & ":" & VB6.Format(i))
					End If
				Next 
			End With
		End If
	End Sub
	
	
	' === 他形態関連処理 ===
	
	'他形態を登録
	Public Sub AddOtherForm(ByRef u As Unit)
		colOtherForm.Add(u, u.ID)
	End Sub
	
	'他形態を削除
	Public Sub DeleteOtherForm(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colOtherForm.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		'見つからなければユニット名称で検索
		For i = 1 To colOtherForm.Count()
			'UPGRADE_WARNING: オブジェクト colOtherForm(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If colOtherForm.Item(i).Name = Index Then
				colOtherForm.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'他形態の総数
	Public Function CountOtherForm() As Short
		CountOtherForm = colOtherForm.Count()
	End Function
	
	'他形態
	Public Function OtherForm(ByRef Index As Object) As Unit
		Dim u As Unit
		Dim uname As String
		Dim i As Short
		
		On Error GoTo ErrorHandler
		OtherForm = colOtherForm.Item(Index)
		Exit Function
		
ErrorHandler: 
		'見つからなければユニット名称で検索
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		uname = CStr(Index)
		
		For	Each u In colOtherForm
			If u.Name = uname Then
				OtherForm = u
				Exit Function
			End If
		Next u
		
		'該当するユニットがなければ作成して追加
		If UDList.IsDefined(uname) Then
			u = New Unit
			With u
				.Name = UDList.Item(uname).Name
				.Rank = Rank
				.BossRank = BossRank
				.Party = Party0
				.ID = UList.CreateID(uname)
				.Status_Renamed = "他形態"
				.x = x
				.y = y
				For i = 1 To CountOtherForm
					.AddOtherForm(OtherForm(i))
					OtherForm(i).AddOtherForm(u)
				Next 
				.AddOtherForm(Me)
				AddOtherForm(u)
			End With
			UList.Add2(u)
			OtherForm = u
		Else
			ErrorMessage("ユニットデータ「" & uname & "」が見つかりません")
		End If
	End Function
	
	'指定した他形態が登録されているか？
	Public Function IsOtherFormDefined(ByRef uname As String) As Boolean
		Dim u As Unit
		
		For	Each u In colOtherForm
			If u.Name = uname Then
				IsOtherFormDefined = True
				Exit Function
			End If
		Next u
		
		IsOtherFormDefined = False
	End Function
	
	'不要な形態を削除
	Public Sub DeleteTemporaryOtherForm()
		Dim uarray() As String
		Dim fname, fdata As String
		Dim k, i, j, n As Short
		
		'必要な形態の一覧を作成
		n = 1
		ReDim uarray(1)
		uarray(1) = Name
		For i = 1 To CountFeature
			fname = Feature(i)
			Select Case fname
				Case "変形"
					fdata = FeatureData(fname)
					n = n + LLength(fdata) - 1
					ReDim Preserve uarray(n)
					For j = 1 To LLength(fdata) - 1
						uarray(n - j + 1) = LIndex(fdata, j + 1)
					Next 
				Case "換装", "他形態"
					fdata = FeatureData(fname)
					n = n + LLength(fdata)
					ReDim Preserve uarray(n)
					For j = 1 To LLength(fdata)
						uarray(n - j + 1) = LIndex(fdata, j)
					Next 
				Case "ハイパーモード", "パーツ分離", "変形技"
					fdata = FeatureData(fname)
					n = n + 1
					ReDim Preserve uarray(n)
					uarray(n) = LIndex(fdata, 2)
				Case "ノーマルモード", "パーツ合体"
					fdata = FeatureData(fname)
					n = n + 1
					ReDim Preserve uarray(n)
					uarray(n) = LIndex(fdata, 1)
			End Select
		Next 
		
		'他形態から必要ない形態へのリンクを削除
		For i = 1 To CountOtherForm
			With OtherForm(i)
				If .Status_Renamed = "他形態" Then
					j = 1
					Do While j <= .CountOtherForm
						With .OtherForm(j)
							For k = 1 To n
								If .Name = uarray(k) Then
									Exit For
								End If
							Next 
						End With
						If k > n Then
							.DeleteOtherForm(j)
						Else
							j = j + 1
						End If
					Loop 
				End If
			End With
		Next 
		
		'必要ない形態を破棄し、リンクを削除
		i = 1
		Do While i <= CountOtherForm
			With OtherForm(i)
				For j = 1 To n
					If .Name = uarray(j) Then
						Exit For
					End If
				Next 
			End With
			If j > n Then
				OtherForm(i).Status_Renamed = "破棄"
				DeleteOtherForm(i)
			Else
				i = i + 1
			End If
		Loop 
	End Sub
	
	
	' === パイロット関連処理 ===
	
	'パイロットを追加
	Public Sub AddPilot(ByRef p As Pilot)
		colPilot.Add(p, p.ID)
	End Sub
	
	'パイロットを削除
	Public Sub DeletePilot(ByRef Index As Object)
		colPilot.Remove(Index)
	End Sub
	
	'パイロットの入れ替え
	Public Sub ReplacePilot(ByRef p As Pilot, ByRef Index As Object)
		Dim i As Short
		Dim prev_p As Pilot
		Dim pilot_list() As Pilot
		
		p.Unit_Renamed = Me
		
		prev_p = colPilot.Item(Index)
		
		ReDim pilot_list(colPilot.Count())
		
		For i = 1 To UBound(pilot_list)
			pilot_list(i) = colPilot.Item(i)
		Next 
		For i = 1 To UBound(pilot_list)
			colPilot.Remove(1)
		Next 
		For i = 1 To UBound(pilot_list)
			If pilot_list(i) Is prev_p Then
				colPilot.Add(p, p.ID)
			Else
				colPilot.Add(pilot_list(i), pilot_list(i).ID)
			End If
		Next 
		'UPGRADE_NOTE: オブジェクト prev_p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		prev_p.Unit_Renamed = Nothing
		prev_p.Alive = False
	End Sub
	
	'搭乗員数
	Public Function CountPilot() As Short
		CountPilot = colPilot.Count()
	End Function
	
	'パイロット
	Public Function Pilot(ByRef Index As Object) As Pilot
		Pilot = colPilot.Item(Index)
	End Function
	
	'メインパイロット
	'各判定にはこのパイロットの能力を用いる
	Public Function MainPilot(Optional ByVal without_update As Boolean = False) As Pilot
		Dim pname As String
		Dim p As Pilot
		Dim i As Short
		Dim need_update As Boolean
		
		'パイロットが乗っていない？
		If CountPilot = 0 Then
			If Not IsFeatureAvailable("追加パイロット") Then
				ErrorMessage("ユニット「" & Name & "」にパイロットが乗っていません")
				TerminateSRC()
			End If
		End If
		
		'破棄された場合はメインパイロットの変更を行わない
		If Status_Renamed = "破棄" Then
			MainPilot = colPilot.Item(1)
			Exit Function
		End If
		
		'能力コピー中は同じパイロットが複数のユニットのメインパイロットに使用されるのを防ぐため
		'追加パイロットと暴走時パイロットを使用しない
		If IsConditionSatisfied("能力コピー") Then
			MainPilot = colPilot.Item(1)
			Exit Function
		End If
		
		'暴走時の特殊パイロット
		If IsConditionSatisfied("暴走") Then
			If IsFeatureAvailable("暴走時パイロット") Then
				pname = FeatureData("暴走時パイロット")
				
				If PDList.IsDefined(pname) Then
					pname = PDList.Item(pname).Name
				Else
					ErrorMessage("暴走時パイロット「" & pname & "」のデータが定義されていません")
				End If
				
				If PList.IsDefined(pname) Then
					'既に暴走時パイロットが作成済み
					MainPilot = PList.Item(pname)
					With MainPilot
						.Unit_Renamed = Me
						.Morale = Pilot(1).Morale
						.Level = Pilot(1).Level
						.Exp = Pilot(1).Exp
						If Not without_update Then
							If Not .Unit_Renamed Is Me Then
								.Unit_Renamed = Me
								.Update()
								.UpdateSupportMod()
							End If
						End If
					End With
					Exit Function
				Else
					'暴走時パイロットが作成されていないので作成する
					MainPilot = PList.Add(pname, Pilot(1).Level, Party0)
					With MainPilot
						.Morale = Pilot(1).Morale
						.Exp = Pilot(1).Exp
						.Unit_Renamed = Me
						.Update()
						.UpdateSupportMod()
					End With
					Exit Function
				End If
			End If
		End If
		
		'追加パイロットがいれば、それを使用
		If IsFeatureAvailable("追加パイロット") Then
			pname = FeatureData("追加パイロット")
			
			If PDList.IsDefined(pname) Then
				pname = PDList.Item(pname).Name
			Else
				ErrorMessage("追加パイロット「" & pname & "」のデータが定義されていません")
			End If
			
			'登録済みのパイロットをまずチェック
			If Not pltAdditionalPilot Is Nothing Then
				If pltAdditionalPilot.Name = pname Then
					MainPilot = pltAdditionalPilot
					With pltAdditionalPilot
						If .IsAdditionalPilot And Not .Unit_Renamed Is Me Then
							.Unit_Renamed = Me
							.Party = Party0
							.Exp = Pilot(1).Exp
							If .Personality <> "機械" Then
								.Morale = Pilot(1).Morale
							End If
							If .Level <> Pilot(1).Level Then
								.Level = Pilot(1).Level
								.Update()
							End If
						End If
					End With
					Exit Function
				End If
			End If
			For i = 1 To CountOtherForm
				If Not OtherForm(i).pltAdditionalPilot Is Nothing Then
					With OtherForm(i).pltAdditionalPilot
						If .Name = pname Then
							pltAdditionalPilot = OtherForm(i).pltAdditionalPilot
							.Party = Party0
							.Unit_Renamed = Me
							If .IsAdditionalPilot And Not .Unit_Renamed Is Me Then
								.Level = Pilot(1).Level
								.Exp = Pilot(1).Exp
								If .Personality <> "機械" Then
									.Morale = Pilot(1).Morale
								End If
								.Update()
								.UpdateSupportMod()
							End If
							MainPilot = pltAdditionalPilot
							Exit Function
						End If
					End With
				End If
			Next 
			
			'次に搭乗しているパイロットから検索
			If CountPilot > 0 Then
				'単なるメインパイロットの交代として扱うため、IsAdditionalPilotのフラグは立てない
				For i = 1 To CountPilot
					If Pilot(i).Name = pname Then
						pltAdditionalPilot = Pilot(i)
						MainPilot = pltAdditionalPilot
						Exit Function
					End If
				Next 
			End If
			
			'既に作成されていればそれを使う
			'(ただし複数作成可能なパイロットで、他のユニットの追加パイロットとして登録済みの場合は除く)
			If PList.IsDefined(pname) Then
				p = PList.Item(pname)
				If Not p.IsAdditionalPilot Or (InStr(pname, "(ザコ)") = 0 And InStr(pname, "(汎用)") = 0) Then
					pltAdditionalPilot = p
					With pltAdditionalPilot
						.IsAdditionalPilot = True
						.Party = Party0
						.Level = Pilot(1).Level
						.Exp = Pilot(1).Exp
						If .Personality <> "機械" Then
							.Morale = Pilot(1).Morale
						End If
						If Not without_update Then
							If Not .Unit_Renamed Is Me Then
								.Unit_Renamed = Me
								.Update()
								.UpdateSupportMod()
							End If
						Else
							.Unit_Renamed = Me
						End If
					End With
					MainPilot = pltAdditionalPilot
					Exit Function
				End If
			End If
			
			'まだ作成されていないので作成する
			If CountPilot > 0 Then
				pltAdditionalPilot = PList.Add(pname, Pilot(1).Level, Party0)
				With pltAdditionalPilot
					.IsAdditionalPilot = True
					.Exp = Pilot(1).Exp
					If .Personality <> "機械" Then
						.Morale = Pilot(1).Morale
					End If
				End With
			Else
				pltAdditionalPilot = PList.Add(pname, 1, Party0)
				pltAdditionalPilot.IsAdditionalPilot = True
			End If
			With pltAdditionalPilot
				.Unit_Renamed = Me
				If Not without_update Then
					.Update()
					.UpdateSupportMod()
				End If
			End With
			MainPilot = pltAdditionalPilot
			Exit Function
		End If
		
		'そうでなければ第１パイロットを使用
		MainPilot = colPilot.Item(1)
	End Function
	
	
	'サポートパイロットを追加
	Public Sub AddSupport(ByRef p As Pilot)
		colSupport.Add(p, p.Name)
	End Sub
	
	'サポートパイロットを削除
	Public Sub DeleteSupport(ByRef Index As Object)
		colSupport.Remove(Index)
	End Sub
	
	'サポートパイロットの入れ替え
	Public Sub ReplaceSupport(ByRef p As Pilot, ByRef Index As Object)
		Dim i As Short
		Dim prev_p As Pilot
		Dim support_list() As Pilot
		
		p.Unit_Renamed = Me
		
		prev_p = colSupport.Item(Index)
		
		ReDim support_list(colSupport.Count())
		
		For i = 1 To UBound(support_list)
			support_list(i) = colSupport.Item(i)
		Next 
		For i = 1 To UBound(support_list)
			colSupport.Remove(1)
		Next 
		For i = 1 To UBound(support_list)
			If support_list(i).ID = prev_p.ID Then
				colSupport.Add(p, p.ID)
			Else
				colSupport.Add(support_list(i), support_list(i).ID)
			End If
		Next 
		'UPGRADE_NOTE: オブジェクト prev_p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		prev_p.Unit_Renamed = Nothing
		prev_p.Alive = False
	End Sub
	
	'総サポートパイロット数
	Public Function CountSupport() As Short
		CountSupport = colSupport.Count()
	End Function
	
	'サポート
	Public Function Support(ByRef Index As Object) As Pilot
		Support = colSupport.Item(Index)
	End Function
	
	'追加サポート
	Public Function AdditionalSupport() As Pilot
		Dim pname As String
		Dim p As Pilot
		Dim i As Short
		
		'追加サポートパイロットの名称
		pname = FeatureData("追加サポート")
		
		'追加サポートが存在しない？
		If pname = "" Then
			Exit Function
		End If
		
		'他にパイロットが乗っていない場合は無効
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'既に登録済みであるかチェック
		If Not pltAdditionalSupport Is Nothing Then
			If pltAdditionalSupport.Name = pname Then
				AdditionalSupport = pltAdditionalSupport
				pltAdditionalSupport.Unit_Renamed = Me
				Exit Function
			End If
		End If
		For i = 1 To CountOtherForm
			With OtherForm(i)
				If Not .pltAdditionalSupport Is Nothing Then
					If .pltAdditionalSupport.Name = pname Then
						.pltAdditionalSupport.Unit_Renamed = Me
						AdditionalSupport = .pltAdditionalSupport
						Exit Function
					End If
				End If
			End With
		Next 
		
		'既に作成されていればそれを使う
		'(ただし他のユニットの追加サポートとして登録済みの場合は除く)
		If PList.IsDefined(pname) Then
			p = PList.Item(pname)
			If Not p.IsAdditionalSupport Or (InStr(pname, "(ザコ)") = 0 And InStr(pname, "(汎用)") = 0) Then
				pltAdditionalSupport = p
				With pltAdditionalSupport
					.IsAdditionalSupport = True
					.Party = Party0
					.Unit_Renamed = Me
					.Level = Pilot(1).Level
					.Exp = Pilot(1).Exp
					If .Personality <> "機械" Then
						.Morale = Pilot(1).Morale
					End If
				End With
				AdditionalSupport = pltAdditionalSupport
				Exit Function
			End If
		End If
		
		'まだ作成されていないので作成する
		If Not PDList.IsDefined(pname) Then
			ErrorMessage("追加サポート「" & pname & "」のデータが定義されていません")
			Exit Function
		End If
		pltAdditionalSupport = PList.Add(pname, Pilot(1).Level, Party0)
		With pltAdditionalSupport
			.IsAdditionalSupport = True
			.Unit_Renamed = Me
			.Exp = Pilot(1).Exp
			If .Personality <> "機械" Then
				.Morale = Pilot(1).Morale
			End If
		End With
		AdditionalSupport = pltAdditionalSupport
	End Function
	
	'いずれかのパイロットが特殊能力 sname を持っているか判定
	Public Function IsSkillAvailable(ByRef sname As String) As Boolean
		Dim i As Short
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'メインパイロット
		If MainPilot.IsSkillAvailable(sname) Then
			IsSkillAvailable = True
			Exit Function
		End If
		
		'パイロット数が負の場合はメインパイロットの能力のみが有効
		If Data.PilotNum > 0 Then
			For i = 2 To CountPilot
				If Pilot(i).IsSkillAvailable(sname) Then
					IsSkillAvailable = True
					Exit Function
				End If
			Next 
		End If
		
		'サポート
		For i = 1 To CountSupport
			If Support(i).IsSkillAvailable(sname) Then
				IsSkillAvailable = True
				Exit Function
			End If
		Next 
		
		'追加サポート
		If IsFeatureAvailable("追加サポート") Then
			If AdditionalSupport.IsSkillAvailable(sname) Then
				IsSkillAvailable = True
				Exit Function
			End If
		End If
		
		IsSkillAvailable = False
	End Function
	
	'パイロット全員によるパイロット能力レベル
	Public Function SkillLevel(ByVal sname As String, Optional ByVal default_slevel As Double = 1) As Double
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'エリアスが設定されてるかチェック
		If ALDList.IsDefined(sname) Then
			sname = ALDList.Item(sname).AliasType(1)
		End If
		
		Select Case sname
			Case "同調率"
				SkillLevel = SyncLevel
			Case "霊力"
				SkillLevel = PlanaLevel
			Case "オーラ"
				SkillLevel = AuraLevel
			Case "超能力"
				SkillLevel = PsychicLevel
			Case "Ｓ防御", "切り払い"
				SkillLevel = MainPilot.SkillLevel(sname, CStr(1))
			Case "超感覚"
				If MaxSkillLevel("超感覚", 1) > MaxSkillLevel("知覚強化", 1) Then
					SkillLevel = MaxSkillLevel("超感覚", 1)
				Else
					SkillLevel = MaxSkillLevel("知覚強化", 1)
				End If
			Case Else
				SkillLevel = MaxSkillLevel(sname, default_slevel)
		End Select
	End Function
	
	'パイロット中での最も高いパイロット能力レベルを返す
	Private Function MaxSkillLevel(ByRef sname As String, ByVal default_slevel As Double) As Double
		Dim slevel As Double
		Dim i As Short
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'メインパイロット
		With MainPilot
			If .IsSkillLevelSpecified(sname) Then
				MaxSkillLevel = .SkillLevel(sname)
			ElseIf .IsSkillAvailable(sname) Then 
				MaxSkillLevel = default_slevel
			Else
				MaxSkillLevel = 0
			End If
		End With
		
		'パイロット数が負の場合はメインパイロットの能力のみが有効
		If Data.PilotNum > 0 Then
			For i = 2 To CountPilot
				With Pilot(i)
					If .IsSkillLevelSpecified(sname) Then
						slevel = .SkillLevel(sname)
					ElseIf .IsSkillAvailable(sname) Then 
						slevel = default_slevel
					Else
						slevel = 0
					End If
					If slevel > MaxSkillLevel Then
						MaxSkillLevel = slevel
					End If
				End With
			Next 
		End If
		
		'サポート
		For i = 1 To CountSupport
			With Support(i)
				If .IsSkillLevelSpecified(sname) Then
					slevel = .SkillLevel(sname)
				ElseIf .IsSkillAvailable(sname) Then 
					slevel = default_slevel
				Else
					slevel = 0
				End If
				If slevel > MaxSkillLevel Then
					MaxSkillLevel = slevel
				End If
			End With
		Next 
		
		'追加サポート
		If IsFeatureAvailable("追加サポート") Then
			With AdditionalSupport
				If .IsSkillLevelSpecified(sname) Then
					slevel = .SkillLevel(sname)
				ElseIf .IsSkillAvailable(sname) Then 
					slevel = default_slevel
				Else
					slevel = 0
				End If
				If slevel > MaxSkillLevel Then
					MaxSkillLevel = slevel
				End If
			End With
		End If
	End Function
	
	'ユニットのオーラ力レベル
	Public Function AuraLevel(Optional ByVal no_limit As Boolean = False) As Double
		Select Case CountPilot
			Case 0
				Exit Function
			Case 1
				AuraLevel = MainPilot.SkillLevel("オーラ")
			Case Else
				'パイロットが２名以上の場合は２人目のオーラ力を加算
				AuraLevel = MainPilot.SkillLevel("オーラ") + Pilot(2).SkillLevel("オーラ") / 2
		End Select
		
		'サポートのオーラ力を加算
		If IsFeatureAvailable("追加サポート") Then
			AuraLevel = AuraLevel + AdditionalSupport.SkillLevel("オーラ") / 2
		ElseIf CountSupport > 0 Then 
			AuraLevel = AuraLevel + Support(1).SkillLevel("オーラ") / 2
		End If
		
		'オーラ変換器レベルによる制限
		If IsFeatureAvailable("オーラ変換器") And Not no_limit Then
			If IsFeatureLevelSpecified("オーラ変換器") Then
				AuraLevel = MinDbl(AuraLevel, FeatureLevel("オーラ変換器"))
			End If
		End If
	End Function
	
	'ユニットの超能力レベル
	Public Function PsychicLevel(Optional ByVal no_limit As Boolean = False) As Double
		Select Case CountPilot
			Case 0
				Exit Function
			Case 1
				PsychicLevel = MainPilot.SkillLevel("超能力")
			Case Else
				'パイロットが２名以上の場合は２人目の超能力を加算
				PsychicLevel = MainPilot.SkillLevel("超能力") + Pilot(2).SkillLevel("超能力") / 2
		End Select
		
		'サポートのオーラ力を加算
		If IsFeatureAvailable("追加サポート") Then
			PsychicLevel = PsychicLevel + AdditionalSupport.SkillLevel("超能力") / 2
		ElseIf CountSupport > 0 Then 
			'サポートの超能力を加算
			PsychicLevel = PsychicLevel + Support(1).SkillLevel("超能力") / 2
		End If
		
		'サイキックドライブによる制限
		If IsFeatureAvailable("サイキックドライブ") And Not no_limit Then
			If IsFeatureLevelSpecified("サイキックドライブ") Then
				PsychicLevel = MinDbl(PsychicLevel, FeatureLevel("サイキックドライブ"))
			End If
		End If
	End Function
	
	'ユニットの同調率
	Public Function SyncLevel(Optional ByVal no_limit As Boolean = False) As Double
		If CountPilot = 0 Then
			Exit Function
		End If
		
		SyncLevel = MainPilot.SynchroRate
		
		'シンクロドライブレベルによる制限
		If IsFeatureAvailable("シンクロドライブ") And Not no_limit Then
			If IsFeatureLevelSpecified("シンクロドライブ") Then
				SyncLevel = MinDbl(SyncLevel, FeatureLevel("シンクロドライブ"))
			End If
		End If
	End Function
	
	'ユニットの霊力レベル
	Public Function PlanaLevel(Optional ByVal no_limit As Boolean = False) As Double
		If CountPilot = 0 Then
			Exit Function
		End If
		
		PlanaLevel = MainPilot.Plana
		
		'霊力変換器レベルによる制限
		If IsFeatureAvailable("霊力変換器") And Not no_limit Then
			If IsFeatureLevelSpecified("霊力変換器") Then
				PlanaLevel = MinDbl(PlanaLevel, FeatureLevel("霊力変換器"))
			End If
		End If
	End Function
	
	'パイロット全員からパイロット能力名を検索
	Public Function SkillName0(ByVal sname As String) As String
		Dim i As Short
		
		If ALDList.IsDefined(sname) Then
			sname = ALDList.Item(sname).AliasType(1)
		End If
		
		If CountPilot = 0 Then
			SkillName0 = sname
			Exit Function
		End If
		
		'メインパイロット
		SkillName0 = MainPilot.SkillName0(sname)
		If SkillName0 <> sname Then
			Exit Function
		End If
		
		'パイロット数が負の場合はメインパイロットの能力のみが有効
		If Data.PilotNum > 0 Then
			For i = 2 To CountPilot
				SkillName0 = Pilot(i).SkillName0(sname)
				If SkillName0 <> sname Then
					Exit Function
				End If
			Next 
		End If
		
		'サポート
		For i = 1 To CountSupport
			SkillName0 = Support(i).SkillName0(sname)
			If SkillName0 <> sname Then
				Exit Function
			End If
		Next 
		
		'追加サポート
		If IsFeatureAvailable("追加サポート") Then
			SkillName0 = AdditionalSupport.SkillName0(sname)
		End If
	End Function
	
	
	' === 行動数関連処理 ===
	
	'１ターンに可能な行動数
	Public Function MaxAction(Optional ByVal ignore_en As Boolean = False) As Short
		'ステータス異常？
		If IsConditionSatisfied("行動不能") Or IsConditionSatisfied("麻痺") Or IsConditionSatisfied("石化") Or IsConditionSatisfied("凍結") Or IsConditionSatisfied("睡眠") Or IsConditionSatisfied("チャージ") Or IsConditionSatisfied("消耗") Or IsUnderSpecialPowerEffect("行動不能") Then
			Exit Function
		End If
		
		'ＥＮ切れ？
		If Not ignore_en Then
			If EN = 0 Then
				If Not IsOptionDefined("ＥＮ０時行動可") Then
					Exit Function
				End If
			End If
		End If
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'２回行動可能？
		If IsOptionDefined("２回行動能力使用") Then
			If MainPilot.IsSkillAvailable("２回行動") Then
				MaxAction = 2
			Else
				MaxAction = 1
			End If
		Else
			If MainPilot.Intuition >= 200 Then
				MaxAction = 2
			Else
				MaxAction = 1
			End If
		End If
	End Function
	
	'行動数を消費
	Public Sub UseAction()
		Dim max_action As Short
		
		'２回行動可能？
		If CountPilot = 0 Then
			max_action = 1
		ElseIf IsOptionDefined("２回行動能力使用") Then 
			If MainPilot.IsSkillAvailable("２回行動") Then
				max_action = 2
			Else
				max_action = 1
			End If
		Else
			If MainPilot.Intuition >= 200 Then
				max_action = 2
			Else
				max_action = 1
			End If
		End If
		
		'最大行動数まで行動消費量をカウント
		UsedAction = MinLng(UsedAction + 1, max_action)
	End Sub
	
	
	' === スペシャルパワー関連処理 ===
	
	'影響下にあるスペシャルパワー一覧
	Public Function SpecialPowerInEffect() As String
		Dim cnd As Condition
		
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				If .ShortName = "非表示" Then
					'イベント専用
					GoTo NextSpecialPower
				End If
				
				If .Duration = "みがわり" Then
					'みがわりは別表示
					GoTo NextSpecialPower
				End If
				
				SpecialPowerInEffect = SpecialPowerInEffect & .ShortName
			End With
NextSpecialPower: 
		Next cnd
		
		'みがわりはかばってくれるユニットを表示する
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				If .Duration = "みがわり" Then
					If PList.IsDefined((cnd.StrData)) Then
						SpecialPowerInEffect = SpecialPowerInEffect & .ShortName & "(" & PList.Item((cnd.StrData)).Nickname & ")"
					End If
					Exit Function
				End If
			End With
		Next cnd
	End Function
	
	'ユニットがスペシャルパワー sname の影響下にあるかどうか
	Public Function IsSpecialPowerInEffect(ByRef sname As String) As Boolean
		Dim cnd As Condition
		
		If colSpecialPowerInEffect.Count() = 0 Then
			Exit Function
		End If
		
		On Error GoTo ErrorHandler
		cnd = colSpecialPowerInEffect.Item(sname)
		IsSpecialPowerInEffect = True
		Exit Function
ErrorHandler: 
	End Function
	
	'ユニットがスペシャルパワー効果 sptype の影響下にあるかどうか
	Public Function IsUnderSpecialPowerEffect(ByRef sptype As String) As Boolean
		Dim cnd As Condition
		Dim i As Short
		
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				For i = 1 To .CountEffect
					If .EffectType(i) = sptype Then
						IsUnderSpecialPowerEffect = True
						Exit Function
					End If
				Next 
			End With
		Next cnd
		IsUnderSpecialPowerEffect = False
	End Function
	
	'影響下にあるスペシャルパワーの総数
	Public Function CountSpecialPower() As Short
		CountSpecialPower = colSpecialPowerInEffect.Count()
	End Function
	
	'影響下にあるスペシャルパワー
	Public Function SpecialPower(ByRef Index As Object) As SpecialPowerData
		Dim cnd As Condition
		
		On Error GoTo ErrorHandler
		
		cnd = colSpecialPowerInEffect.Item(Index)
		SpecialPower = SPDList.Item((cnd.Name))
		
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト SpecialPower をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SpecialPower = Nothing
	End Function
	
	'スペシャルパワー mname の効果レベル
	Public Function SpecialPowerEffectLevel(ByRef sname As String) As Double
		Dim cnd As Condition
		Dim i As Short
		Dim lv As Double
		
		lv = DEFAULT_LEVEL
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				For i = 1 To .CountEffect
					If .EffectType(i) = sname Then
						If .EffectLevel(i) > lv Then
							lv = .EffectLevel(i)
						End If
						Exit For
					End If
				Next 
			End With
		Next cnd
		
		If lv <> DEFAULT_LEVEL Then
			SpecialPowerEffectLevel = lv
		End If
	End Function
	
	'スペシャルパワーのデータ
	Public Function SpecialPowerData(ByRef Index As Object) As String
		Dim cnd As Condition
		
		On Error GoTo ErrorHandler
		cnd = colSpecialPowerInEffect.Item(Index)
		SpecialPowerData = cnd.StrData
		Exit Function
		
ErrorHandler: 
	End Function
	
	'スペシャルパワー sname の効果を適用
	Public Sub MakeSpecialPowerInEffect(ByRef sname As String, Optional ByRef sdata As String = "")
		Dim cnd As New Condition
		
		'すでに使用されていればなにもしない
		If IsSpecialPowerInEffect(sname) Then
			Exit Sub
		End If
		
		With cnd
			.Name = sname
			.StrData = sdata
		End With
		
		colSpecialPowerInEffect.Add(cnd, sname)
	End Sub
	
	'持続時間が stype であるスペシャルパワーの効果を発動後、取り除く
	Public Sub RemoveSpecialPowerInEffect(ByRef stype As String)
		Dim sd As SpecialPowerData
		Dim i As Short
		Dim is_message_form_visible As Boolean
		Dim pid As String
		
		'メッセージウィンドウが表示されているか記録
		is_message_form_visible = frmMessage.Visible
		
		i = 1
		Do While i <= CurrentForm.CountSpecialPower
			sd = SpecialPower(i)
			
			'スペシャルパワーの持続期間が指定したものと一致しているかチェック
			If stype <> sd.Duration Then
				i = i + 1
				GoTo NextSP
			End If
			
			'持続期間が敵ターンの場合、スペシャルパワーをかけてきた敵のフェイズ
			'が来るまで効果を削除しない
			If stype = "敵ターン" Then
				If PList.IsDefined(SpecialPowerData((sd.Name))) Then
					With PList.Item(SpecialPowerData((sd.Name)))
						If Not .Unit_Renamed Is Nothing Then
							If .Unit_Renamed.CurrentForm.Party <> Stage Then
								i = i + 1
								GoTo NextSP
							End If
						End If
					End With
				End If
			End If
			
			'消去するスペシャルパワーの効果を発動
			If CurrentForm.Status_Renamed = "出撃" Then
				sd.Apply(CurrentForm.MainPilot, CurrentForm, False, True)
			End If
			
			'スペシャルパワーの効果を削除
			CurrentForm.RemoveSpecialPowerInEffect2(i)
NextSP: 
		Loop 
		
		'メッセージウィンドウが元から表示されていなければ閉じておく
		If Not is_message_form_visible And frmMessage.Visible Then
			CloseMessageForm()
		End If
	End Sub
	
	'スペシャルパワー sname の効果を取り除く
	Public Sub RemoveSpecialPowerInEffect2(ByRef Index As Object)
		On Error GoTo ErrorHandler
		colSpecialPowerInEffect.Remove(Index)
		Exit Sub
ErrorHandler: 
	End Sub
	
	'全てのスペシャルパワーの効果を取り除く
	Public Sub RemoveAllSpecialPowerInEffect()
		Dim i As Short
		
		With colSpecialPowerInEffect
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
	End Sub
	
	'スペシャルパワーの効果をユニット u にコピーする
	Public Sub CopySpecialPowerInEffect(ByRef u As Unit)
		Dim cnd As Condition
		
		For	Each cnd In colSpecialPowerInEffect
			With cnd
				u.MakeSpecialPowerInEffect(.Name, .StrData)
			End With
		Next cnd
	End Sub
	
	
	' === 特殊状態関連処理 ===
	
	'特殊状態を付加
	Public Sub AddCondition(ByRef cname As String, ByVal ltime As Short, Optional ByVal clevel As Double = DEFAULT_LEVEL, Optional ByRef cdata As String = "")
		Dim cnd As Condition
		Dim new_condition As New Condition
		
		'同じ特殊状態が既に付加されている？
		For	Each cnd In colCondition
			With cnd
				If .Name = cname Then
					If .Lifetime < 0 Or ltime < 0 Then
						.Lifetime = -1
					Else
						.Lifetime = MaxLng(.Lifetime, ltime)
					End If
					.Name = cname
					.Level = clevel
					.StrData = cdata
					Exit Sub
				End If
			End With
		Next cnd
		
		'特殊状態を付加
		With new_condition
			.Name = cname
			.Lifetime = ltime
			.Level = clevel
			.StrData = cdata
		End With
		
		colCondition.Add(new_condition, cname)
	End Sub
	
	'特殊状態を削除
	Public Sub DeleteCondition(ByRef Index As Object)
		With colCondition.Item(Index)
			colCondition.Remove(Index)
			
			'特殊能力付加の場合はユニットのステータスをアップデート
			'UPGRADE_WARNING: オブジェクト colCondition.Item().StrData の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			'UPGRADE_WARNING: オブジェクト colCondition.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If Right(.Name, 2) = "付加" And InStr(.StrData, "パイロット能力付加") = 0 Then
				Update()
			End If
		End With
	End Sub
	
	Public Sub DeleteCondition0(ByRef Index As Object)
		colCondition.Remove(Index)
	End Sub
	
	'付加された特殊状態の総数
	Public Function CountCondition() As Short
		CountCondition = colCondition.Count()
	End Function
	
	'特殊状態
	Public Function Condition(ByRef Index As Object) As String
		'UPGRADE_WARNING: オブジェクト colCondition.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		Condition = colCondition.Item(Index).Name
	End Function
	
	'特殊状態の残りターン数
	Public Function ConditionLifetime(ByRef Index As Object) As Short
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: オブジェクト colCondition.Item().Lifetime の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ConditionLifetime = colCondition.Item(Index).Lifetime
		Exit Function
		
ErrorHandler: 
		ConditionLifetime = -1
	End Function
	
	'指定した特殊能力が付加されているか？
	Public Function IsConditionSatisfied(ByRef Index As Object) As Boolean
		Dim ltime As Short
		
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: オブジェクト colCondition.Item().Lifetime の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ltime = colCondition.Item(Index).Lifetime
		IsConditionSatisfied = True
		Exit Function
		
ErrorHandler: 
		IsConditionSatisfied = False
	End Function
	
	'特殊状態のレベル
	Public Function ConditionLevel(ByRef Index As Object) As Double
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: オブジェクト colCondition.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ConditionLevel = colCondition.Item(Index).Level
		Exit Function
		
ErrorHandler: 
		ConditionLevel = 0
	End Function
	
	'特殊状態のレベルの変更
	Public Sub SetConditionLevel(ByRef Index As Object, ByVal lv As Double)
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: オブジェクト colCondition.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		colCondition.Item(Index).Level = lv
ErrorHandler: 
	End Sub
	
	'特殊能力のデータ
	Public Function ConditionData(ByRef Index As Object) As String
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: オブジェクト colCondition.Item().StrData の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ConditionData = colCondition.Item(Index).StrData
		Exit Function
		
ErrorHandler: 
		ConditionData = ""
	End Function
	
	'特殊能力の残りターン数を更新
	Public Sub UpdateCondition(Optional ByVal decrement_lifetime As Boolean = False)
		Dim cnd As Condition
		Dim update_is_necessary As Boolean
		Dim charge_complete As Boolean
		
		For	Each cnd In colCondition
			With cnd
				If decrement_lifetime Then
					'残りターン数を1減らす
					If .Lifetime > 0 Then
						.Lifetime = .Lifetime - 1
					End If
				End If
				
				If .Lifetime = 0 Then
					'残りターン数が0なら削除
					colCondition.Remove(.Name)
					
					Select Case .Name
						Case "魅了"
							'魅了を解除
							If Not Master Is Nothing Then
								Master.CurrentForm.DeleteSlave(ID)
								'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								Master = Nothing
							End If
							Mode = "通常"
						Case "チャージ"
							'チャージ完了
							charge_complete = True
						Case "活動限界"
							'活動限界時間切れ
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "は強制的に退却させられた。")
							CloseMessageForm()
							HandleEvent("破壊", MainPilot.ID)
						Case Else
							'特殊能力付加を解除
							If Right(.Name, 2) = "付加" Or Right(.Name, 2) = "強化" Then
								update_is_necessary = True
							End If
					End Select
				End If
			End With
		Next cnd
		
		'チャージ状態が終了したらチャージ完了状態にする
		If charge_complete Then
			AddCondition("チャージ完了", 1)
		End If
		
		'ユニットのステータス変化あり？
		If update_is_necessary Then
			Update()
		End If
	End Sub
	
	
	' === 武器関連処理 ===
	
	'武器
	Public Function Weapon(ByVal w As Short) As WeaponData
		Weapon = WData(w)
	End Function
	
	'武器の総数
	Public Function CountWeapon() As Short
		CountWeapon = UBound(WData)
	End Function
	
	'武器の愛称
	Public Function WeaponNickname(ByVal w As Short) As String
		Dim u As Unit
		
		'愛称内の式置換のため、デフォルトユニットを一時的に変更する
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Me
		WeaponNickname = WData(w).Nickname
		SelectedUnitForEvent = u
	End Function
	
	'武器の攻撃力
	'tarea は敵のいる地形
	Public Function WeaponPower(ByVal w As Short, ByRef tarea As String) As Integer
		Dim pat As Integer
		'攻撃補正一時保存
		Dim ed_atk As Double
		
		WeaponPower = lngWeaponPower(w)
		
		'「体」属性を持つ武器は残りＨＰに応じて攻撃力が増える
		If IsWeaponClassifiedAs(w, "体") Then
			WeaponPower = WeaponPower + (HP / MaxHP) * 100 * WeaponLevel(w, "体")
		End If
		
		'「尽」属性を持つ武器は残りＥＮに応じて攻撃力が増える
		If IsWeaponClassifiedAs(w, "尽") Then
			If EN >= WeaponENConsumption(w) Then
				WeaponPower = WeaponPower + (EN - WeaponENConsumption(w)) * WeaponLevel(w, "尽")
			End If
		End If
		
		'ダメージ固定武器
		Dim wad As Double
		If IsWeaponClassifiedAs(w, "固") Then
			
			'武器一覧の場合は攻撃力をそのまま表示
			If tarea = "" Then
				Exit Function
			End If
			
			'マップ攻撃は攻撃開始時に保存した攻撃力をそのまま使う
			If IsWeaponClassifiedAs(w, "Ｍ") Then
				If SelectedMapAttackPower > 0 Then
					WeaponPower = SelectedMapAttackPower
				End If
			End If
			
			'地形適応による修正のみを適用
			wad = WeaponAdaption(w, tarea)
			
			'地形適応修正繰り下げオプションの効果は適用しない
			If IsOptionDefined("地形適応修正繰り下げ") Then
				If IsOptionDefined("地形適応修正緩和") Then
					wad = wad + 0.1
				Else
					wad = wad + 0.2
				End If
			End If
			
			'地形適応がＡの場合に攻撃力と同じダメージを与えるようにする
			If IsOptionDefined("地形適応修正緩和") Then
				wad = wad - 0.1
			Else
				wad = wad - 0.2
			End If
			
			If wad > 0 Then
				WeaponPower = WeaponPower * wad
			Else
				WeaponPower = 0
			End If
			Exit Function
		End If
		
		'部隊ユニットはダメージを受けると攻撃力が低下
		If IsFeatureAvailable("部隊ユニット") Then
			WeaponPower = WeaponPower * (50 + 50 * HP / MaxHP) \ 100
		End If
		
		'標的のいる地形が設定されていないときは武器の一覧表示用なので各種補正を省く
		If tarea = "" Then
			Exit Function
		End If
		
		With MainPilot
			If BCList.IsDefined("攻撃補正") Then
				'バトルコンフィグデータの設定による修正
				If IsWeaponClassifiedAs(w, "複") Then
					pat = (.Infight + .Shooting) \ 2
				ElseIf IsWeaponClassifiedAs(w, "格闘系") Then 
					pat = .Infight
				Else
					pat = .Shooting
				End If
				
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				'UPGRADE_NOTE: オブジェクト BCVariable.DefUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				BCVariable.DefUnit = Nothing
				BCVariable.WeaponNumber = w
				BCVariable.AttackExp = pat
				BCVariable.WeaponPower = WeaponPower
				WeaponPower = BCList.Item("攻撃補正").Calculate()
			Else
				'パイロットの攻撃力による修正
				
				If IsWeaponClassifiedAs(w, "複") Then
					WeaponPower = WeaponPower * (.Infight + .Shooting) \ 200
				ElseIf IsWeaponClassifiedAs(w, "格闘系") Then 
					WeaponPower = WeaponPower * .Infight \ 100
				Else
					WeaponPower = WeaponPower * .Shooting \ 100
				End If
				
				'気力による修正
				If IsOptionDefined("気力効果小") Then
					WeaponPower = WeaponPower * (50 + (.Morale + .MoraleMod) \ 2) \ 100
				Else
					WeaponPower = WeaponPower * (.Morale + .MoraleMod) \ 100
				End If
				
			End If
			
			'覚悟
			If HP <= MaxHP \ 4 Then
				If .IsSkillAvailable("覚悟") Then
					If IsOptionDefined("ダメージ倍率低下") Then
						WeaponPower = 1.1 * WeaponPower
					Else
						WeaponPower = 1.2 * WeaponPower
					End If
				End If
			End If
		End With
		
		'マップ攻撃用に攻撃力算出
		If tarea = "初期値" Then
			Exit Function
		End If
		
		'マップ攻撃は攻撃開始時に保存した攻撃力をそのまま使う
		If IsWeaponClassifiedAs(w, "Ｍ") Then
			If SelectedMapAttackPower > 0 Then
				WeaponPower = SelectedMapAttackPower
			End If
		End If
		
		'地形補正
		If BCList.IsDefined("攻撃地形補正") Then
			'事前にデータを登録
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			'UPGRADE_NOTE: オブジェクト BCVariable.DefUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			BCVariable.DefUnit = Nothing
			BCVariable.WeaponNumber = w
			BCVariable.AttackExp = WeaponPower
			BCVariable.TerrainAdaption = WeaponAdaption(w, tarea)
			WeaponPower = BCList.Item("攻撃地形補正").Calculate()
		Else
			WeaponPower = WeaponPower * WeaponAdaption(w, tarea)
		End If
	End Function
	
	'武器 w の地形 tarea におけるダメージ修正値
	Public Function WeaponAdaption(ByVal w As Short, ByRef tarea As String) As Double
		Dim wad, uad, xad As Short
		Dim ind As Short
		
		'武器の地形適応値の計算に使用する適応値を決定
		Select Case tarea
			Case "空中"
				ind = 1
			Case "地上"
				If TerrainClass(x, y) = "月面" Then
					ind = 4
				Else
					ind = 2
				End If
			Case "水上"
				If Mid(Weapon(w).Adaption, 3, 1) = "A" Then
					ind = 3
				Else
					ind = 2
				End If
			Case "水中"
				ind = 3
			Case "宇宙"
				ind = 4
			Case "地中"
				WeaponAdaption = 0
				Exit Function
			Case Else
				xad = 4
				GoTo CalcAdaption
		End Select
		
		'武器の地形適応値
		Select Case Mid(Weapon(w).Adaption, ind, 1)
			Case "S"
				wad = 5
			Case "A"
				wad = 4
			Case "B"
				wad = 3
			Case "C"
				wad = 2
			Case "D"
				wad = 1
			Case "-"
				WeaponAdaption = 0
				Exit Function
		End Select
		
		'ユニットの地形適応値の計算に使用する適応値を決定
		If Not IsWeaponClassifiedAs(w, "武") And Not IsWeaponClassifiedAs(w, "突") And Not IsWeaponClassifiedAs(w, "接") Then
			'格闘戦以外の場合はユニットがいる地形を参照
			Select Case Area
				Case "空中"
					ind = 1
				Case "地上"
					If TerrainClass(x, y) = "月面" Then
						ind = 4
					Else
						ind = 2
					End If
				Case "水上"
					ind = 2
				Case "水中"
					ind = 3
				Case "宇宙"
					ind = 4
				Case "地中"
					WeaponAdaption = 0
					Exit Function
			End Select
			'ユニットの地形適応値
			uad = Adaption(ind)
		Else
			'格闘戦の場合はターゲットがいる地形を参照
			Select Case tarea
				Case "空中"
					uad = Adaption(1)
					'ジャンプ攻撃
					If IsWeaponClassifiedAs(w, "Ｊ") Then
						uad = uad + WeaponLevel(w, "Ｊ")
					End If
				Case "地上"
					If Adaption(2) > 0 Then
						uad = Adaption(2)
					Else
						'空中専用ユニットが地上のユニットに格闘戦をしかけられるようにする
						uad = MaxLng(Adaption(1) - 1, 0)
					End If
				Case "水上"
					'水中専用ユニットが水上のユニットに格闘戦をしかけられるようにする
					uad = MaxDbl(Adaption(2), Adaption(3))
					If uad <= 0 Then
						'空中専用ユニットが地上のユニットに格闘戦をしかけられるようにする
						uad = MaxLng(Adaption(1) - 1, 0)
					End If
				Case "水中"
					uad = Adaption(3)
				Case "宇宙"
					uad = Adaption(4)
					If Area = "地上" And TerrainClass(x, y) = "月面" Then
						'月面からのジャンプ攻撃
						If IsWeaponClassifiedAs(w, "Ｊ") Then
							uad = uad + WeaponLevel(w, "Ｊ")
						End If
					End If
				Case Else
					uad = Adaption(ind)
			End Select
		End If
		
		'地形適応が命中率に適応される場合、ユニットの地形適応は攻撃可否の判定にのみ用いる
		If IsOptionDefined("地形適応命中率修正") Then
			If uad > 0 Then
				xad = wad
				GoTo CalcAdaption
			Else
				WeaponAdaption = 0
				Exit Function
			End If
		End If
		
		'武器側とユニット側の地形適応の低い方を優先
		If uad > wad Then
			xad = wad
		Else
			xad = uad
		End If
		
CalcAdaption: 
		
		'Optionコマンドの設定に従って地形適応値を算出
		If IsOptionDefined("地形適応修正緩和") Then
			If IsOptionDefined("地形適応修正繰り下げ") Then
				Select Case xad
					Case 5
						WeaponAdaption = 1.1
					Case 4
						WeaponAdaption = 1
					Case 3
						WeaponAdaption = 0.9
					Case 2
						WeaponAdaption = 0.8
					Case 1
						WeaponAdaption = 0.7
					Case Else
						WeaponAdaption = 0
				End Select
			Else
				Select Case xad
					Case 5
						WeaponAdaption = 1.2
					Case 4
						WeaponAdaption = 1.1
					Case 3
						WeaponAdaption = 1
					Case 2
						WeaponAdaption = 0.9
					Case 1
						WeaponAdaption = 0.8
					Case Else
						WeaponAdaption = 0
				End Select
			End If
		Else
			If IsOptionDefined("地形適応修正繰り下げ") Then
				Select Case xad
					Case 5
						WeaponAdaption = 1.2
					Case 4
						WeaponAdaption = 1
					Case 3
						WeaponAdaption = 0.8
					Case 2
						WeaponAdaption = 0.6
					Case 1
						WeaponAdaption = 0.4
					Case Else
						WeaponAdaption = 0
				End Select
			Else
				Select Case xad
					Case 5
						WeaponAdaption = 1.4
					Case 4
						WeaponAdaption = 1.2
					Case 3
						WeaponAdaption = 1
					Case 2
						WeaponAdaption = 0.8
					Case 1
						WeaponAdaption = 0.6
					Case Else
						WeaponAdaption = 0
				End Select
			End If
		End If
	End Function
	
	'武器 w の最大射程
	Public Function WeaponMaxRange(ByVal w As Short) As Short
		WeaponMaxRange = intWeaponMaxRange(w)
		
		'最大射程がもともと１ならそれ以上変化しない
		If WeaponMaxRange = 1 Then
			Exit Function
		End If
		
		'マップ攻撃には適用されない
		If IsWeaponClassifiedAs(w, "Ｍ") Then
			Exit Function
		End If
		
		'接近戦武器には適用されない
		If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
			Exit Function
		End If
		
		'有線式誘導攻撃には適用されない
		If IsWeaponClassifiedAs(w, "有") Then
			Exit Function
		End If
		
		'スペシャルパワーによる射程延長
		If IsUnderSpecialPowerEffect("射程延長") Then
			WeaponMaxRange = WeaponMaxRange + SpecialPowerEffectLevel("射程延長")
		End If
	End Function
	
	'武器 w の消費ＥＮ
	Public Function WeaponENConsumption(ByVal w As Short) As Short
		'UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim rate_Renamed As Double
		Dim i As Short
		
		With Weapon(w)
			WeaponENConsumption = .ENConsumption
			
			'パイロットの能力によって術及び技の消費ＥＮは減少する
			If CountPilot > 0 Then
				'術に該当するか？
				If IsSpellWeapon(w) Then
					'術に該当する場合は術技能によってＥＮ消費量を変える
					Select Case MainPilot.SkillLevel("術")
						Case 1
						Case 2
							WeaponENConsumption = 0.9 * WeaponENConsumption
						Case 3
							WeaponENConsumption = 0.8 * WeaponENConsumption
						Case 4
							WeaponENConsumption = 0.7 * WeaponENConsumption
						Case 5
							WeaponENConsumption = 0.6 * WeaponENConsumption
						Case 6
							WeaponENConsumption = 0.5 * WeaponENConsumption
						Case 7
							WeaponENConsumption = 0.45 * WeaponENConsumption
						Case 8
							WeaponENConsumption = 0.4 * WeaponENConsumption
						Case 9
							WeaponENConsumption = 0.35 * WeaponENConsumption
						Case Is >= 10
							WeaponENConsumption = 0.3 * WeaponENConsumption
					End Select
					WeaponENConsumption = MinLng(MaxLng(WeaponENConsumption, 5), .ENConsumption)
				End If
				
				'技に該当するか？
				If IsFeatWeapon(w) Then
					'技に該当する場合は技技能によってＥＮ消費量を変える
					Select Case MainPilot.SkillLevel("技")
						Case 1
						Case 2
							WeaponENConsumption = 0.9 * WeaponENConsumption
						Case 3
							WeaponENConsumption = 0.8 * WeaponENConsumption
						Case 4
							WeaponENConsumption = 0.7 * WeaponENConsumption
						Case 5
							WeaponENConsumption = 0.6 * WeaponENConsumption
						Case 6
							WeaponENConsumption = 0.5 * WeaponENConsumption
						Case 7
							WeaponENConsumption = 0.45 * WeaponENConsumption
						Case 8
							WeaponENConsumption = 0.4 * WeaponENConsumption
						Case 9
							WeaponENConsumption = 0.35 * WeaponENConsumption
						Case Is >= 10
							WeaponENConsumption = 0.3 * WeaponENConsumption
					End Select
					WeaponENConsumption = MinLng(MaxLng(WeaponENConsumption, 5), .ENConsumption)
				End If
			End If
			
			'ＥＮ消費減少能力による修正
			rate_Renamed = 1
			If IsFeatureAvailable("ＥＮ消費減少") Then
				For i = 1 To CountFeature
					If Feature(i) = "ＥＮ消費減少" Then
						rate_Renamed = rate_Renamed - 0.1 * FeatureLevel(i)
					End If
				Next 
			End If
			
			If rate_Renamed < 0.1 Then
				rate_Renamed = 0.1
			End If
			WeaponENConsumption = rate_Renamed * WeaponENConsumption
		End With
	End Function
	
	'武器 w の命中率
	Public Function WeaponPrecision(ByVal w As Short) As Short
		WeaponPrecision = intWeaponPrecision(w)
	End Function
	
	'武器 w のＣＴ率
	Public Function WeaponCritical(ByVal w As Short) As Short
		WeaponCritical = intWeaponCritical(w)
	End Function
	
	'武器 w の属性
	Public Function WeaponClass(ByVal w As Short) As String
		WeaponClass = strWeaponClass(w)
	End Function
	
	'武器 w が武器属性 attr を持っているかどうか
	Public Function IsWeaponClassifiedAs(ByVal w As Short, ByRef attr As String) As Boolean
		Dim wclass As String
		
		wclass = strWeaponClass(w)
		
		'属性が２文字以下ならそのまま判定
		If Len(attr) <= 2 Then
			If InStrNotNest(wclass, attr) > 0 Then
				IsWeaponClassifiedAs = True
			Else
				IsWeaponClassifiedAs = False
			End If
			Exit Function
		End If
		
		'属性の頭文字が弱攻剋ならそのまま判定
		If InStr("弱効剋", Left(attr, 1)) > 0 Then
			If InStrNotNest(wclass, attr) > 0 Then
				IsWeaponClassifiedAs = True
			Else
				IsWeaponClassifiedAs = False
			End If
			Exit Function
		End If
		
		'条件が複雑な場合
		Select Case attr
			Case "格闘系"
				If InStrNotNest(wclass, "格") > 0 Then
					IsWeaponClassifiedAs = True
				ElseIf InStrNotNest(wclass, "射") > 0 Then 
					IsWeaponClassifiedAs = False
				ElseIf Weapon(w).MaxRange = 1 Then 
					IsWeaponClassifiedAs = True
				Else
					IsWeaponClassifiedAs = False
				End If
				Exit Function
			Case "射撃系"
				If InStrNotNest(wclass, "格") > 0 Then
					IsWeaponClassifiedAs = False
				ElseIf InStrNotNest(wclass, "射") > 0 Then 
					IsWeaponClassifiedAs = True
				ElseIf Weapon(w).MaxRange = 1 Then 
					IsWeaponClassifiedAs = False
				Else
					IsWeaponClassifiedAs = True
				End If
			Case "移動後攻撃可"
				If IsUnderSpecialPowerEffect("全武器移動後使用可能") And InStrNotNest(wclass, "Ｍ") = 0 And InStrNotNest(wclass, "Ｑ") = 0 Then
					IsWeaponClassifiedAs = True
				ElseIf Weapon(w).MaxRange = 1 Then 
					If InStrNotNest(wclass, "Ｑ") = 0 Then
						IsWeaponClassifiedAs = True
					Else
						IsWeaponClassifiedAs = False
					End If
				ElseIf InStrNotNest(wclass, "Ｐ") > 0 Then 
					IsWeaponClassifiedAs = True
				End If
		End Select
	End Function
	
	'武器 w の属性 attr におけるレベル
	Public Function WeaponLevel(ByVal w As Short, ByRef attr As String) As Double
		Dim attrlv, wclass As String
		Dim start_idx, i As Short
		Dim c As String
		
		On Error GoTo ErrorHandler
		
		attrlv = attr & "L"
		
		'武器属性を調べてみる
		wclass = strWeaponClass(w)
		
		'レベル指定があるか？
		start_idx = InStrNotNest(wclass, attrlv)
		If start_idx = 0 Then
			Exit Function
		End If
		
		'レベル指定部分の切り出し
		start_idx = start_idx + Len(attrlv)
		i = start_idx
		Do While True
			c = Mid(wclass, i, 1)
			
			If c = "" Then
				Exit Do
			End If
			
			Select Case Asc(c)
				Case 45 To 46, 48 To 57 '"-", ".", 0-9
				Case Else
					Exit Do
			End Select
			
			i = i + 1
		Loop 
		
		WeaponLevel = CDbl(Mid(wclass, start_idx, i - start_idx))
		Exit Function
		
ErrorHandler: 
		ErrorMessage(Name & "の" & "武装「" & Weapon(w).Name & "」の" & "属性「" & attr & "」のレベル指定が不正です")
	End Function
	
	'武器 w の属性 attr にレベル指定がなされているか
	Public Function IsWeaponLevelSpecified(ByVal w As Short, ByRef attr As String) As Boolean
		If InStr(strWeaponClass(w), attr & "L") > 0 Then
			IsWeaponLevelSpecified = True
			Exit Function
		End If
	End Function
	
	'武器 w が通常武器かどうか
	Public Function IsNormalWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim wclass As String
		Dim ret As Short
		
		'特殊効果属性を持つ？
		wclass = strWeaponClass(w)
		For i = 1 To Len(wclass)
			ret = InStr("Ｓ縛劣中石凍痺眠乱魅恐踊狂ゾ害憑盲毒撹不止黙除即告脱Ｄ低吹Ｋ引転衰滅盗習写化弱効剋", Mid(wclass, i, 1))
			If ret > 0 Then
				Exit Function
			End If
		Next 
		
		IsNormalWeapon = True
	End Function
	
	'武器が持つ特殊効果の数を返す
	Public Function CountWeaponEffect(ByVal w As Short) As Short
		Dim wclass, wattr As String
		Dim i, ret As Short
		
		wclass = strWeaponClass(w)
		For i = 1 To Len(wclass)
			'弱Ｓのような入れ子があれば、入れ子の分カウントを進める
			wattr = GetClassBundle(wclass, i, 1)
			
			'非表示部分は無視
			If wattr = "|" Then
				Exit For
			End If
			
			'ＣＴ時発動系
			ret = InStr("Ｓ縛劣中石凍痺眠乱魅恐踊狂ゾ害憑盲毒撹不止黙除即告脱Ｄ低吹Ｋ引転衰滅盗習写化弱効剋", wattr)
			If ret > 0 Then
				CountWeaponEffect = CountWeaponEffect + 1
			End If
			
			'それ以外
			ret = InStr("先再忍貫固殺無浸破間浄吸減奪", wattr)
			If ret > 0 Then
				CountWeaponEffect = CountWeaponEffect + 1
			End If
		Next 
	End Function
	
	'武器 w が術かどうか
	Public Function IsSpellWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsWeaponClassifiedAs(w, "術") Then
			IsSpellWeapon = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Weapon(w).NecessarySkill))
				nskill = LIndex((Weapon(w).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "術" Then
					IsSpellWeapon = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'武器 w が技かどうか
	Public Function IsFeatWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsWeaponClassifiedAs(w, "技") Then
			IsFeatWeapon = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Weapon(w).NecessarySkill))
				nskill = LIndex((Weapon(w).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "技" Then
					IsFeatWeapon = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'武器 w が使用可能かどうか
	'ref_mode はユニットの状態（移動前、移動後）を示す
	Public Function IsWeaponAvailable(ByVal w As Short, ByRef ref_mode As String) As Boolean
		Dim i As Short
		Dim wd As WeaponData
		Dim wclass As String
		
		IsWeaponAvailable = False
		
		' ADD START MARGE
		'武器が取得できない場合はFalse（防御や無抵抗の場合、wが0や-1になる）
		If Not (w > 0) Then
			Exit Function
		End If
		' ADD END MARGE
		
		wd = Weapon(w)
		wclass = WeaponClass(w)
		
		'イベントコマンド「Disable」で封印されている？
		If IsDisabled((wd.Name)) Then
			Exit Function
		End If
		
		'パイロットが乗っていなければ常に使用可能と判定
		If CountPilot = 0 Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'必要技能＆必要条件
		If ref_mode <> "必要技能無視" Then
			If Not IsWeaponMastered(w) Then
				Exit Function
			End If
			
			If Not IsWeaponEnabled(w) Then
				Exit Function
			End If
		End If
		
		'ステータス表示では必要技能だけ満たしていればＯＫ
		If ref_mode = "インターミッション" Or ref_mode = "" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		With MainPilot
			'必要気力
			If wd.NecessaryMorale > 0 Then
				If .Morale < wd.NecessaryMorale Then
					Exit Function
				End If
			End If
			
			'霊力消費攻撃
			If InStrNotNest(wclass, "霊") > 0 Then
				If .Plana < WeaponLevel(w, "霊") * 5 Then
					Exit Function
				End If
			ElseIf InStrNotNest(wclass, "プ") > 0 Then 
				If .Plana < WeaponLevel(w, "プ") * 5 Then
					Exit Function
				End If
			End If
		End With
		
		'属性使用不能状態
		If ConditionLifetime("オーラ使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "オ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("超能力使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "超") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("同調率使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "シ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("超感覚使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "サ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("知覚強化使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "サ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("霊力使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "霊") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("術使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "術") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("技使用不能") > 0 Then
			If IsWeaponClassifiedAs(w, "技") Then
				Exit Function
			End If
		End If
		For i = 1 To CountCondition
			If Len(Condition(i)) > 6 Then
				If Right(Condition(i), 6) = "属性使用不能" Then
					If InStrNotNest(WeaponClass(w), Left(Condition(i), Len(Condition(i)) - 6)) > 0 Then
						Exit Function
					End If
				End If
			End If
		Next 
		
		'弾数が足りるか
		If wd.Bullet > 0 Then
			If Bullet(w) < 1 Then
				Exit Function
			End If
		End If
		
		'ＥＮが足りるか
		If wd.ENConsumption > 0 Then
			If EN < WeaponENConsumption(w) Then
				Exit Function
			End If
		End If
		
		'お金が足りるか……
		If Party = "味方" Then
			If InStrNotNest(wclass, "銭") > 0 Then
				If Money < MaxLng(WeaponLevel(w, "銭"), 1) * Value \ 10 Then
					Exit Function
				End If
			End If
		End If
		
		'攻撃不能？
		If ref_mode <> "ステータス" Then
			If IsConditionSatisfied("攻撃不能") Then
				Exit Function
			End If
		End If
		If Area = "地中" Then
			Exit Function
		End If
		
		'移動不能時には移動型マップ攻撃は使用不能
		If IsConditionSatisfied("移動不能") Then
			If InStrNotNest(wclass, "Ｍ移") > 0 Then
				Exit Function
			End If
		End If
		
		'術および音は沈黙状態では使用不能
		If IsConditionSatisfied("沈黙") Then
			If IsSpellWeapon(w) Or InStrNotNest(wclass, "音") > 0 Then
				Exit Function
			End If
		End If
		
		'合体技の処理
		If InStrNotNest(wclass, "合") > 0 Then
			If Not IsCombinationAttackAvailable(w) Then
				Exit Function
			End If
		End If
		
		'変形技の場合は今いる地形で変形できる必要あり
		If InStrNotNest(wclass, "変") > 0 Then
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = wd.Name Then
						If Not OtherForm(LIndex(FeatureData(i), 2)).IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End If
				Next 
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				If Not OtherForm(LIndex(FeatureData("ノーマルモード"), 1)).IsAbleToEnter(x, y) Then
					Exit Function
				End If
			End If
			If IsConditionSatisfied("形態固定") Then
				Exit Function
			End If
			If IsConditionSatisfied("機体固定") Then
				Exit Function
			End If
		End If
		
		'瀕死時限定
		If InStrNotNest(wclass, "瀕") > 0 Then
			If HP > MaxHP \ 4 Then
				Exit Function
			End If
		End If
		
		'自動チャージ攻撃を再充填中
		If IsConditionSatisfied(WeaponNickname(w) & "充填中") Then
			Exit Function
		End If
		'共有武器＆アビリティが充填中の場合も使用不可
		Dim lv As Short
		If InStrNotNest(wclass, "共") > 0 Then
			lv = WeaponLevel(w, "共")
			For i = 1 To CountWeapon
				If IsWeaponClassifiedAs(i, "共") Then
					If lv = WeaponLevel(i, "共") Then
						If IsConditionSatisfied(WeaponNickname(i) & "充填中") Then
							Exit Function
						End If
					End If
				End If
			Next 
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "共") Then
					If lv = AbilityLevel(i, "共") Then
						If IsConditionSatisfied(AbilityNickname(i) & "充填中") Then
							Exit Function
						End If
					End If
				End If
			Next 
		End If
		
		'能力コピー
		If InStrNotNest(wclass, "写") > 0 Or InStrNotNest(wclass, "化") > 0 Then
			If IsFeatureAvailable("ノーマルモード") Then
				'既に変身済みの場合はコピー出来ない
				Exit Function
			End If
		End If
		
		'使用禁止
		If InStrNotNest(wclass, "禁") > 0 Then
			Exit Function
		End If
		
		'チャージ判定であればここまででＯＫ
		If ref_mode = "チャージ" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'チャージ式攻撃
		If InStrNotNest(wclass, "Ｃ") > 0 Then
			If Not IsConditionSatisfied("チャージ完了") Then
				Exit Function
			End If
		End If
		
		If ref_mode = "ステータス" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'反撃かどうかの判定
		'自軍のフェイズでなければ反撃時である
		If Party <> Stage Then
			'反撃ではマップ攻撃、合体技は使用できない
			If InStrNotNest(wclass, "Ｍ") > 0 Or InStrNotNest(wclass, "合") > 0 Then
				Exit Function
			End If
			
			'攻撃専用武器
			If InStrNotNest(wclass, "攻") > 0 Then
				For i = 1 To Len(wclass)
					If Mid(wclass, i, 1) = "攻" Then
						If i = 1 Then
							Exit Function
						End If
						If Mid(wclass, i - 1, 1) <> "低" Then
							Exit Function
						End If
					End If
				Next 
			End If
		Else
			'反撃専用攻撃
			If InStrNotNest(wclass, "反") > 0 Then
				Exit Function
			End If
		End If
		
		'移動前か後か……
		If ref_mode = "移動前" Or ref_mode = "必要技能無視" Or Not SelectedUnit Is Me Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'移動後の場合
		If IsUnderSpecialPowerEffect("全武器移動後使用可能") And Not InStrNotNest(wclass, "Ｍ") > 0 Then
			IsWeaponAvailable = True
		ElseIf WeaponMaxRange(w) > 1 Then 
			If InStrNotNest(wclass, "Ｐ") > 0 Then
				IsWeaponAvailable = True
			Else
				IsWeaponAvailable = False
			End If
		Else
			If InStrNotNest(wclass, "Ｑ") > 0 Then
				IsWeaponAvailable = False
			Else
				IsWeaponAvailable = True
			End If
		End If
	End Function
	
	'武器 w の使用技能を満たしているか。
	Public Function IsWeaponMastered(ByVal w As Short) As Boolean
		IsWeaponMastered = IsNecessarySkillSatisfied((Weapon(w).NecessarySkill))
	End Function
	
	'武器 w の使用条件を満たしているか。
	Public Function IsWeaponEnabled(ByVal w As Short) As Boolean
		IsWeaponEnabled = IsNecessarySkillSatisfied((Weapon(w).NecessaryCondition))
	End Function
	
	'武器が使用可能であり、かつ射程内に敵がいるかどうか
	Public Function IsWeaponUseful(ByVal w As Short, ByRef ref_mode As String) As Boolean
		Dim i, j As Short
		Dim u As Unit
		Dim max_range As Short
		
		'武器が使用可能か？
		If Not IsWeaponAvailable(w, ref_mode) Then
			IsWeaponUseful = False
			Exit Function
		End If
		
		'扇型マップ攻撃は特殊なので判定ができない
		'移動型マップ攻撃は移動手段として使うことを考慮
		If IsWeaponClassifiedAs(w, "Ｍ扇") Or IsWeaponClassifiedAs(w, "Ｍ移") Then
			IsWeaponUseful = True
			Exit Function
		End If
		
		max_range = WeaponMaxRange(w)
		
		'投下型マップ攻撃は効果範囲が広い
		max_range = max_range + WeaponLevel(w, "Ｍ投")
		
		'敵の存在判定
		For i = MaxLng(x - max_range, 1) To MinLng(x + max_range, MapWidth)
			For j = MaxLng(y - max_range, 1) To MinLng(y + max_range, MapHeight)
				u = MapDataForUnit(i, j)
				If u Is Nothing Then
					GoTo NextUnit
				End If
				With u
					Select Case Party
						Case "味方", "ＮＰＣ"
							Select Case .Party
								Case "味方", "ＮＰＣ"
									'ステータス異常の場合は味方ユニットでも排除可能
									If Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("憑依") And Not .IsConditionSatisfied("睡眠") Then
										GoTo NextUnit
									End If
							End Select
						Case Else
							If Party = .Party Then
								'ステータス異常の場合は味方ユニットでも排除可能
								If Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("憑依") Then
									GoTo NextUnit
								End If
							End If
					End Select
				End With
				
				If IsTargetWithinRange(w, u) Then
					If Weapon(w).Power > 0 Then
						If Damage(w, u, True) <> 0 Then
							IsWeaponUseful = True
							Exit Function
						End If
					Else
						If CriticalProbability(w, u) > 0 Then
							IsWeaponUseful = True
							Exit Function
						End If
					End If
				End If
NextUnit: 
			Next 
		Next 
		
		'敵は見つからなかった
		IsWeaponUseful = False
	End Function
	
	
	'ユニット t が武器 w の射程範囲内にいるかをチェック
	Public Function IsTargetWithinRange(ByVal w As Short, ByRef t As Unit) As Boolean
		Dim max_range, distance, range_mod As Short
		Dim lv As Short
		
		IsTargetWithinRange = True
		
		Dim partners() As Unit
		With t
			'距離を算出
			distance = System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
			
			'Ｍ投武器は目標地点からさらに効果範囲が伸びるので射程修正を行う
			range_mod = WeaponLevel(w, "Ｍ投")
			
			'最大射程チェック
			max_range = WeaponMaxRange(w)
			If distance > max_range + range_mod Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'最小射程チェック
			If distance < Weapon(w).MinRange - range_mod Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'敵がステルスの場合
			If .IsFeatureAvailable("ステルス") Then
				If .IsFeatureLevelSpecified("ステルス") Then
					lv = .FeatureLevel("ステルス")
				Else
					lv = 3
				End If
				If Not .IsConditionSatisfied("ステルス無効") And Not IsFeatureAvailable("ステルス無効化") And distance > lv Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
			
			'隠れ身中？
			If .IsUnderSpecialPowerEffect("隠れ身") Then
				If Not .IsUnderSpecialPowerEffect("無防備") Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
			
			'攻撃できない地形にいる場合は射程外とみなす
			If WeaponAdaption(w, .Area) = 0 Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'合体技で射程が１の場合は相手を囲んでいる必要がある
			If IsWeaponClassifiedAs(w, "合") And Not IsWeaponClassifiedAs(w, "Ｍ") And max_range = 1 Then
				CombinationPartner("武装", w, partners, .x, .y)
				If UBound(partners) = 0 Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
		End With
	End Function
	
	'移動を併用した場合にユニット t が武器 w の射程範囲内にいるかをチェック
	Public Function IsTargetReachable(ByVal w As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim max_range, min_range As Short
		
		Dim partners() As Unit
		With t
			'地形適応をチェック
			If WeaponAdaption(w, .Area) = 0 Then
				IsTargetReachable = False
				Exit Function
			End If
			
			'隠れ身使用中？
			If .IsUnderSpecialPowerEffect("隠れ身") Then
				If Not .IsUnderSpecialPowerEffect("無防備") Then
					IsTargetReachable = False
					Exit Function
				End If
			End If
			
			'射程計算
			min_range = Weapon(w).MinRange
			max_range = WeaponMaxRange(w)
			'敵がステルスの場合
			If .IsFeatureAvailable("ステルス") And Not .IsConditionSatisfied("ステルス無効") And Not IsFeatureAvailable("ステルス無効化") Then
				If .IsFeatureLevelSpecified("ステルス") Then
					max_range = MinLng(max_range, .FeatureLevel("ステルス") + 1)
				Else
					max_range = MinLng(max_range, 4)
				End If
			End If
			
			'隣接していれば必ず届く
			If min_range = 1 And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
				'ただし合体技の場合は例外……
				'合体技で射程が１の場合は相手を囲んでいる必要がある
				If IsWeaponClassifiedAs(w, "合") And Not IsWeaponClassifiedAs(w, "Ｍ") And WeaponMaxRange(w) = 1 Then
					CombinationPartner("武装", w, partners, t.x, t.y)
					If UBound(partners) = 0 Then
						IsTargetReachable = False
						Exit Function
					End If
				End If
				IsTargetReachable = True
				Exit Function
			End If
			
			'移動範囲から敵に攻撃が届くかをチェック
			For i = MaxLng(.x - max_range, 1) To MinLng(.x + max_range, MapWidth)
				For j = MaxLng(.y - (max_range - System.Math.Abs(.x - i)), 1) To MinLng(.y + (max_range - System.Math.Abs(.x - i)), MapHeight)
					If min_range <= System.Math.Abs(.x - i) + System.Math.Abs(.y - i) Then
						If Not MaskData(i, j) Then
							IsTargetReachable = True
							Exit Function
						End If
					End If
				Next 
			Next 
		End With
		
		IsTargetReachable = False
	End Function
	
	'武器 w のユニット t に対する命中率
	'敵ユニットはスペシャルパワー等による補正を考慮しないので
	' is_true_value によって補正を省くかどうかを指定できるようにしている
	Public Function HitProbability(ByVal w As Short, ByRef t As Unit, ByVal is_true_value As Boolean) As Short
		Dim prob As Integer
		Dim mpskill As Short
		Dim i, j As Short
		Dim u As Unit
		Dim wclass As String
		Dim ecm_lv, eccm_lv As Double
		Dim buf As String
		Dim fdata As String
		Dim flevel, prob_mod As Double
		Dim nmorale As Short
		'命中、回避、地形補正、サイズ補正の数値を定義
		Dim ed_hit, ed_avd As Integer
		Dim ed_aradap, ed_size As Double
		
		'初期値
		ed_aradap = 1
		
		'スペシャルパワーによる捨て身状態
		If t.IsUnderSpecialPowerEffect("無防備") Then
			HitProbability = 100
			Exit Function
		End If
		
		'パイロットの技量によって命中率を正確に予測できるか左右される
		mpskill = MainPilot.TacticalTechnique
		
		'スペシャルパワーによる影響
		If is_true_value Or mpskill >= 160 Then
			If t.IsUnderSpecialPowerEffect("絶対回避") Then
				HitProbability = 0
				Exit Function
			End If
			If IsUnderSpecialPowerEffect("絶対命中") Then
				HitProbability = 1000
				Exit Function
			End If
		End If
		
		'自ユニットによる修正
		With MainPilot
			If BCList.IsDefined("命中補正") Then
				'命中を一時保存
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.AttackExp = WeaponPrecision(w)
				ed_hit = BCList.Item("命中補正").Calculate()
			Else
				'命中を一時保存
				ed_hit = 100 + .Hit + .Intuition + Mobility() + WeaponPrecision(w)
			End If
		End With
		
		'敵ユニットによる修正
		With t.MainPilot
			If BCList.IsDefined("回避補正") Then
				'回避を一時保存
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = t
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				ed_avd = BCList.Item("回避補正").Calculate()
			Else
				'回避を一時保存
				ed_avd = .Dodge + .Intuition + t.Mobility()
			End If
		End With
		
		'地形適応、サイズ修正の位置を変更
		Dim uadaption As Double
		Dim tarea As String
		Dim tx, ty As Short
		With t
			'地形修正
			If .Area <> "空中" And (.Area <> "宇宙" Or TerrainClass(.x, .y) <> "月面") Then
				'地形修正を一時保存
				ed_aradap = ed_aradap * (100 - TerrainEffectForHit(.x, .y)) / 100
			End If
			
			'地形適応修正
			If IsOptionDefined("地形適応命中率修正") Then
				
				'接近戦攻撃の場合はターゲット側の地形を参照
				If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
					tarea = .Area
					tx = .x
					ty = .y
				Else
					tarea = Area
					tx = x
					ty = y
				End If
				
				Select Case tarea
					Case "空中"
						uadaption = AdaptionMod(1)
						'ジャンプ攻撃の場合はＪ属性による修正を加える
						If (.Area = "空中" Or .Area = "宇宙") And Area <> "空中" And Area <> "宇宙" And Not IsTransAvailable("空") Then
							If InStrNotNest(wclass, "武") Or InStrNotNest(wclass, "突") Or InStrNotNest(wclass, "接") Then
								uadaption = AdaptionMod(1, WeaponLevel(w, "Ｊ"))
							End If
						End If
					Case "地上"
						If TerrainClass(tx, ty) = "月面" Then
							uadaption = AdaptionMod(4)
						Else
							uadaption = AdaptionMod(2)
						End If
					Case "水上"
						uadaption = AdaptionMod(2)
					Case "水中"
						uadaption = AdaptionMod(3)
					Case "宇宙"
						uadaption = AdaptionMod(4)
					Case "地中"
						HitProbability = 0
						Exit Function
				End Select
				
				'地形修正を一時保存
				ed_aradap = ed_aradap * uadaption
			End If
			
			'サイズ補正
			Select Case .Size
				Case "M"
					ed_size = 1
				Case "L"
					ed_size = 1.2
				Case "S"
					ed_size = 0.8
				Case "LL"
					ed_size = 1.4
				Case "SS"
					ed_size = 0.5
				Case "XL"
					ed_size = 2
			End Select
		End With
		
		'命中率計算実行
		If BCList.IsDefined("命中率") Then
			'事前にデータを登録
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			BCVariable.DefUnit = t
			BCVariable.WeaponNumber = w
			BCVariable.AttackVariable = ed_hit
			BCVariable.DffenceVariable = ed_avd
			BCVariable.TerrainAdaption = ed_aradap
			BCVariable.SizeMod = ed_size
			prob = BCList.Item("命中率").Calculate()
		Else
			prob = (ed_hit - ed_avd) * ed_aradap * ed_size
		End If
		
		'不意打ち
		If IsFeatureAvailable("ステルス") And Not IsConditionSatisfied("ステルス無効") And Not t.IsFeatureAvailable("ステルス無効化") Then
			prob = prob + 20
		End If
		
		wclass = WeaponClass(w)
		
		Dim uad As Short
		With t
			'散属性武器は指定したレベル以上離れるほど命中がアップ
			If InStrNotNest(wclass, "散") > 0 Then
				Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
					Case 1
						'修正なし
					Case 2
						prob = prob + 5
					Case 3
						prob = prob + 10
					Case 4
						prob = prob + 15
					Case Else
						prob = prob + 20
				End Select
			End If
			
			If InStrNotNest(wclass, "サ") = 0 And InStrNotNest(wclass, "有") = 0 And InStrNotNest(wclass, "誘") = 0 And InStrNotNest(wclass, "追") = 0 And InStrNotNest(wclass, "武") = 0 And InStrNotNest(wclass, "突") = 0 And InStrNotNest(wclass, "接") = 0 Then
				'距離修正
				If IsOptionDefined("距離修正") Then
					If InStrNotNest(wclass, "Ｈ") = 0 And InStrNotNest(wclass, "Ｍ") = 0 Then
						If IsOptionDefined("大型マップ") Then
							Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
								Case 1 To 4
									'修正なし
								Case 5, 6
									prob = 0.9 * prob
								Case 7, 8
									prob = 0.8 * prob
								Case 9, 10
									prob = 0.7 * prob
								Case Else
									prob = 0.6 * prob
							End Select
						ElseIf IsOptionDefined("小型マップ") Then 
							Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
								Case 1
									'修正なし
								Case 2
									prob = 0.9 * prob
								Case 3
									prob = 0.8 * prob
								Case 4
									prob = 0.75 * prob
								Case 5
									prob = 0.7 * prob
								Case 6
									prob = 0.65 * prob
								Case Else
									prob = 0.6 * prob
							End Select
						Else
							Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
								Case 1 To 3
									'修正なし
								Case 4
									prob = 0.9 * prob
								Case 5
									prob = 0.8 * prob
								Case 6
									prob = 0.7 * prob
								Case Else
									prob = 0.6 * prob
							End Select
						End If
					End If
				End If
				
				'ＥＣＭ
				For i = MaxLng(.x - 2, 1) To MinLng(.x + 2, MapWidth)
					For j = MaxLng(.y - 2, 1) To MinLng(.y + 2, MapHeight)
						If System.Math.Abs(.x - i) + System.Math.Abs(.y - j) <= 3 Then
							u = MapDataForUnit(i, j)
							If Not u Is Nothing Then
								With u
									If .IsAlly(t) Then
										ecm_lv = MaxDbl(ecm_lv, .FeatureLevel("ＥＣＭ"))
									ElseIf .IsAlly(Me) Then 
										eccm_lv = MaxDbl(eccm_lv, .FeatureLevel("ＥＣＭ"))
									End If
								End With
							End If
						End If
					Next 
				Next 
				'ホーミング攻撃はＥＣＭの影響を強く受ける
				If InStrNotNest(wclass, "Ｈ") > 0 Then
					prob = prob * (100 - 10 * MaxDbl(ecm_lv - eccm_lv, 0)) \ 100
				Else
					prob = prob * (100 - 5 * MaxDbl(ecm_lv - eccm_lv, 0)) \ 100
				End If
			End If
			
			'ステルスによる補正
			If .IsFeatureAvailable("ステルス") And Not IsFeatureAvailable("ステルス無効化") Then
				If .IsFeatureLevelSpecified("ステルス") Then
					If System.Math.Abs(x - .x) + System.Math.Abs(y - .y) > .FeatureLevel("ステルス") Then
						prob = prob * 0.8
					End If
				Else
					If System.Math.Abs(x - .x) + System.Math.Abs(y - .y) > 3 Then
						prob = prob * 0.8
					End If
				End If
			End If
			
			'地上から空中の敵に攻撃する
			If (.Area = "空中" Or .Area = "宇宙") And Area <> "空中" And Area <> "宇宙" Then
				If InStrNotNest(wclass, "武") Or InStrNotNest(wclass, "突") Or InStrNotNest(wclass, "接") Then
					'ジャンプ攻撃
					If Not IsOptionDefined("地形適応命中率修正") Then
						If Not IsTransAvailable("空") Then
							uad = Adaption(1)
							If InStrNotNest(wclass, "Ｊ") > 0 Then
								uad = MinLng(uad + WeaponLevel(w, "Ｊ"), 4)
							End If
							uad = MinLng(uad, 4)
							prob = (uad + 6) * prob \ 10
						End If
					End If
				Else
					'通常攻撃
					If IsOptionDefined("高度修正") Then
						If InStrNotNest(wclass, "空") = 0 Then
							prob = 0.7 * prob
						End If
					End If
				End If
			End If
			
			'局地戦能力
			If .IsFeatureAvailable("地形適応") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "地形適応" Then
						buf = .FeatureData(i)
						For j = 2 To LLength(buf)
							If TerrainName(.x, .y) = LIndex(buf, j) Then
								prob = prob - 10
								Exit For
							End If
						Next 
					End If
				Next 
			End If
			
			'攻撃回避
			If .IsFeatureAvailable("攻撃回避") Then
				prob_mod = 0
				For i = 1 To .CountFeature
					If .Feature(i) = "攻撃回避" Then
						fdata = .FeatureData(i)
						flevel = .FeatureLevel(i)
						
						'必要条件
						If IsNumeric(LIndex(fdata, 3)) Then
							nmorale = CShort(LIndex(fdata, 3))
						Else
							nmorale = 0
						End If
						
						'発動可能？
						If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) Then
							'攻撃回避発動
							prob_mod = prob_mod + flevel
						End If
					End If
				Next 
				prob = prob * (10 - prob_mod) \ 10
			End If
			
			'動けなければ絶対に命中
			If .IsConditionSatisfied("行動不能") Or .IsConditionSatisfied("麻痺") Or .IsConditionSatisfied("睡眠") Or .IsConditionSatisfied("石化") Or .IsConditionSatisfied("凍結") Or .IsUnderSpecialPowerEffect("行動不能") Then
				HitProbability = 1000
				Exit Function
			End If
			
			'ステータス異常による修正
			If InStrNotNest(wclass, "Ｈ") = 0 And InStrNotNest(wclass, "追") = 0 Then
				If IsConditionSatisfied("撹乱") Then
					prob = prob \ 2
				End If
				If IsConditionSatisfied("恐怖") Then
					prob = prob \ 2
				End If
				If IsConditionSatisfied("盲目") Then
					prob = prob \ 2
				End If
			End If
			
			'ターゲットのステータス異常による修正
			If .IsConditionSatisfied("盲目") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("チャージ") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("消耗") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("狂戦士") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("移動不能") Then
				prob = 1.5 * prob
			End If
			
			'底力
			If HP <= MaxHP \ 4 Then
				With MainPilot
					If .IsSkillAvailable("超底力") Then
						prob = prob + 50
					ElseIf .IsSkillAvailable("底力") Then 
						prob = prob + 30
					End If
				End With
			End If
			If .HP <= .MaxHP \ 4 Then
				With .MainPilot
					If .IsSkillAvailable("超底力") Then
						prob = prob - 50
					ElseIf .IsSkillAvailable("底力") Then 
						prob = prob - 30
					End If
				End With
			End If
			
			'スペシャルパワー及び特殊状態による補正
			If is_true_value Or mpskill >= 160 Then
				If IsUnderSpecialPowerEffect("命中強化") Then
					prob = prob + 10 * SpecialPowerEffectLevel("命中強化")
				ElseIf IsConditionSatisfied("運動性ＵＰ") Then 
					prob = prob + 15
				End If
				If .IsUnderSpecialPowerEffect("回避強化") Then
					prob = prob - 10 * .SpecialPowerEffectLevel("回避強化")
				ElseIf .IsConditionSatisfied("運動性ＵＰ") Then 
					prob = prob - 15
				End If
				
				If IsConditionSatisfied("運動性ＤＯＷＮ") Then
					prob = prob - 15
				End If
				If .IsConditionSatisfied("運動性ＤＯＷＮ") Then
					prob = prob + 15
				End If
				
				If IsUnderSpecialPowerEffect("命中低下") Then
					prob = prob - 10 * SpecialPowerEffectLevel("命中低下")
				End If
				If .IsUnderSpecialPowerEffect("回避低下") Then
					prob = prob + 10 * .SpecialPowerEffectLevel("回避低下")
				End If
				
				If IsUnderSpecialPowerEffect("命中率低下") Then
					prob = prob * (10 - SpecialPowerEffectLevel("命中率低下")) \ 10
				End If
			End If
		End With
		
		' 最終命中率を定義する。これがないときは何もしない
		If BCList.IsDefined("最終命中率") Then
			'事前にデータを登録
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			BCVariable.DefUnit = t
			BCVariable.WeaponNumber = w
			BCVariable.LastVariable = prob
			prob = BCList.Item("最終命中率").Calculate()
		End If
		
		If prob < 0 Then
			HitProbability = 0
		Else
			HitProbability = prob
		End If
	End Function
	
	'武器 w のユニット t に対するダメージ
	'敵ユニットはスペシャルパワー等による補正を考慮しないので
	' is_true_value によって補正を省くかどうかを指定できるようにしている
	Public Function Damage(ByVal w As Short, ByRef t As Unit, ByVal is_true_value As Boolean, Optional ByVal is_support_attack As Boolean = False) As Integer
		Dim arm, arm_mod As Integer
		Dim j, i, idx As Short
		Dim ch, wclass, buf As String
		Dim mpskill As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim slevel As Double
		Dim sdata As String
		Dim nmorale As Short
		Dim neautralize As Boolean
		Dim lv_mod As Double
		Dim opt As String
		Dim tname As String
		Dim dmg_mod, uadaption As Double
		'装甲、装甲補正一時保存
		Dim ed_amr As Double
		Dim ed_amr_fix As Double
		
		wclass = WeaponClass(w)
		
		'パイロットの技量によってダメージを正確に予測できるか左右される
		mpskill = MainPilot.TacticalTechnique
		
		With t
			'武器攻撃力
			Damage = WeaponPower(w, .Area)
			'攻撃力が0の場合は常にダメージ0
			If Damage = 0 Then
				Exit Function
			End If
			
			'基本装甲値
			arm = .Armor
			
			'アーマー能力
			If Not .IsFeatureAvailable("アーマー") Then
				GoTo SkipArmor
			End If
			'ザコはアーマーを考慮しない
			If Not is_true_value And mpskill < 150 Then
				GoTo SkipArmor
			End If
			arm_mod = 0
			For i = 1 To .CountFeature
				If .Feature(i) = "アーマー" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'必要条件
					If IsNumeric(LIndex(fdata, 3)) Then
						nmorale = CShort(LIndex(fdata, 3))
					Else
						nmorale = 0
					End If
					
					'オプション
					neautralize = False
					slevel = 0
					For j = 4 To LLength(fdata)
						opt = LIndex(fdata, j)
						idx = InStr(opt, "*")
						If idx > 0 Then
							lv_mod = StrToDbl(Mid(opt, idx + 1))
							opt = Left(opt, idx - 1)
						Else
							lv_mod = -1
						End If
						
						Select Case opt
							Case "能力必要"
								'スキップ
							Case "同調率"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * (.MainPilot.SynchroRate - 30)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "霊力"
								If lv_mod = -1 Then
									lv_mod = 2
								End If
								slevel = lv_mod * .MainPilot.Plana
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "オーラ"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超能力"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超感覚"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * (.MainPilot.SkillLevel("超感覚") + .MainPilot.SkillLevel("知覚強化"))
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .MainPilot.SkillLevel(opt)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'発動可能？
					If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'アーマー発動
						arm_mod = arm_mod + 100 * flevel + slevel
					End If
				End If
			Next 
			
			'装甲が劣化している場合はアーマーによる装甲追加も半減
			If .IsConditionSatisfied("装甲劣化") Then
				arm_mod = arm_mod \ 2
			End If
			
			arm = arm + arm_mod
			
SkipArmor: 
			
			'地形適応による装甲修正
			If Not IsOptionDefined("地形適応命中率修正") Then
				Select Case .Area
					Case "空中"
						uadaption = .AdaptionMod(1)
					Case "地上"
						If TerrainClass(.x, .y) = "月面" Then
							uadaption = .AdaptionMod(4)
						Else
							uadaption = .AdaptionMod(2)
						End If
					Case "水上"
						uadaption = .AdaptionMod(2)
					Case "水中"
						uadaption = .AdaptionMod(3)
					Case "宇宙"
						uadaption = .AdaptionMod(4)
					Case "地中"
						Damage = 0
						Exit Function
				End Select
				If uadaption = 0 Then
					uadaption = 0.6
				End If
			Else
				If .Area = "地中" Then
					Damage = 0
					Exit Function
				Else
					uadaption = 1
				End If
			End If
			
			'不屈による装甲修正
			If .MainPilot.IsSkillAvailable("不屈") Then
				If IsOptionDefined("防御力倍率低下") Then
					If .HP <= .MaxHP \ 8 Then
						arm = 1.15 * arm
					ElseIf .HP <= .MaxHP \ 4 Then 
						arm = 1.1 * arm
					ElseIf .HP <= .MaxHP \ 2 Then 
						arm = 1.05 * arm
					End If
				Else
					If .HP <= .MaxHP \ 8 Then
						arm = 1.3 * arm
					ElseIf .HP <= .MaxHP \ 4 Then 
						arm = 1.2 * arm
					ElseIf .HP <= .MaxHP \ 2 Then 
						arm = 1.1 * arm
					End If
				End If
			End If
			
			'スペシャルパワーによる無防備状態
			If .IsUnderSpecialPowerEffect("無防備") Then
				arm = 0
			End If
			
			If is_true_value Or mpskill >= 160 Then
				'スペシャルパワーによる修正
				If .IsUnderSpecialPowerEffect("装甲強化") Then
					arm = arm * (1# + 0.1 * .SpecialPowerEffectLevel("装甲強化"))
					'装甲強化
				ElseIf .IsConditionSatisfied("防御力ＵＰ") Then 
					If IsOptionDefined("防御力倍率低下") Then
						arm = 1.25 * arm
					Else
						arm = 1.5 * arm
					End If
				End If
				If .IsUnderSpecialPowerEffect("装甲低下") Then
					arm = arm * (1# + 0.1 * .SpecialPowerEffectLevel("装甲低下"))
				ElseIf .IsConditionSatisfied("防御力ＤＯＷＮ") Then 
					arm = 0.75 * arm
				End If
			End If
			
			'貫通型攻撃
			If IsUnderSpecialPowerEffect("貫通攻撃") Then
				arm = arm \ 2
			ElseIf IsWeaponClassifiedAs(w, "貫") Then 
				If IsWeaponLevelSpecified(w, "貫") Then
					arm = arm * (10 - WeaponLevel(w, "貫")) \ 10
				Else
					arm = arm \ 2
				End If
			End If
			If is_true_value Or mpskill >= 140 Then
				'弱点
				If .Weakness(wclass) Then
					arm = arm \ 2
					'吸収する場合は装甲を無視して判定
				ElseIf Not .Effective(wclass) And .Absorb(wclass) Then 
					arm = 0
				End If
			End If
			
			If BCList.IsDefined("防御補正") Then
				'バトルコンフィグデータによる計算実行
				BCVariable.DataReset()
				BCVariable.MeUnit = t
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.Armor = arm
				BCVariable.TerrainAdaption = uadaption
				arm = BCList.Item("防御補正").Calculate()
			Else
				With .MainPilot
					'気力による装甲修正
					If IsOptionDefined("気力効果小") Then
						arm = arm * (50 + (.Morale + .MoraleMod) \ 2) \ 100
					Else
						arm = arm * (.Morale + .MoraleMod) \ 100
					End If
					
					'レベルアップによる装甲修正＋耐久能力
					arm = arm * .Defense \ 100
				End With
				
				' 地形適応による装甲修正
				arm = arm * uadaption
			End If
			
			'ダメージ固定武器の場合は装甲と地形＆距離修正を無視
			If InStrNotNest(wclass, "固") > 0 Then
				GoTo SkipDamageMod
			End If
			
			If BCList.IsDefined("ダメージ") Then
				'バトルコンフィグデータによる計算実行
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.AttackVariable = Damage
				BCVariable.DffenceVariable = arm
				If TerrainClass(.x, .y) = "月面" Then
					If .Area = "地上" Then
						BCVariable.TerrainAdaption = (100 - TerrainEffectForDamage(.x, .y)) / 100
					Else
						BCVariable.TerrainAdaption = 1
					End If
				ElseIf .Area <> "空中" Then 
					BCVariable.TerrainAdaption = (100 - TerrainEffectForDamage(.x, .y)) / 100
				Else
					BCVariable.TerrainAdaption = 1
				End If
				Damage = BCList.Item("ダメージ").Calculate()
			Else
				'装甲値によってダメージを軽減
				Damage = Damage - arm
				
				'地形補正
				If TerrainClass(.x, .y) = "月面" Then
					If .Area = "地上" Then
						Damage = Damage * ((100 - TerrainEffectForDamage(.x, .y)) / 100)
					End If
				ElseIf .Area <> "空中" Then 
					Damage = Damage * ((100 - TerrainEffectForDamage(.x, .y)) / 100)
				End If
			End If
			
			'散属性武器は離れるほどダメージダウン
			If InStrNotNest(wclass, "散") > 0 Then
				Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
					Case 1
						'修正なし
					Case 2
						Damage = 0.95 * Damage
					Case 3
						Damage = 0.9 * Damage
					Case 4
						Damage = 0.85 * Damage
					Case Else
						Damage = 0.8 * Damage
				End Select
			End If
			
			'距離修正
			If IsOptionDefined("距離修正") Then
				If InStrNotNest(wclass, "実") = 0 And InStrNotNest(wclass, "武") = 0 And InStrNotNest(wclass, "突") = 0 And InStrNotNest(wclass, "接") = 0 And InStrNotNest(wclass, "爆") = 0 Then
					If IsOptionDefined("大型マップ") Then
						Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
							Case 1 To 4
								'修正なし
							Case 5, 6
								Damage = 0.95 * Damage
							Case 7, 8
								Damage = 0.9 * Damage
							Case 9, 10
								Damage = 0.85 * Damage
							Case Else
								Damage = 0.8 * Damage
						End Select
					ElseIf IsOptionDefined("小型マップ") Then 
						Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
							Case 1
								'修正なし
							Case 2
								Damage = 0.95 * Damage
							Case 3
								Damage = 0.9 * Damage
							Case 4
								Damage = 0.85 * Damage
							Case 5
								Damage = 0.8 * Damage
							Case 6
								Damage = 0.75 * Damage
							Case Else
								Damage = 0.7 * Damage
						End Select
					Else
						Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
							Case 1 To 3
								'修正なし
							Case 4
								Damage = 0.95 * Damage
							Case 5
								Damage = 0.9 * Damage
							Case 6
								Damage = 0.85 * Damage
							Case Else
								Damage = 0.8 * Damage
						End Select
					End If
				End If
			End If
			
SkipDamageMod: 
			
			'封印攻撃は弱点もしくは有効を持つユニット以外には効かない
			If InStrNotNest(wclass, "封") > 0 Then
				buf = .strWeakness & .strEffective
				For i = 1 To Len(buf)
					'属性をひとまとめずつ取得
					ch = GetClassBundle(buf, i)
					If ch <> "物" And ch <> "魔" Then
						If InStrNotNest(wclass, ch) > 0 Then
							Exit For
						End If
					End If
				Next 
				If i > Len(buf) Then
					Damage = 0
					Exit Function
				End If
			End If
			
			'限定攻撃は指定属性に対して弱点もしくは有効を持つユニット以外には効かない
			idx = InStrNotNest(wclass, "限")
			If idx > 0 Then
				buf = .strWeakness & .strEffective
				For i = 1 To Len(buf)
					'属性をひとまとめずつ取得
					ch = GetClassBundle(buf, i)
					If ch <> "物" And ch <> "魔" Then
						If InStrNotNest(wclass, ch) > idx Then
							Exit For
						End If
					End If
				Next 
				If i > Len(buf) Then
					Damage = 0
					Exit Function
				End If
			End If
			
			'特定レベル限定攻撃
			If WeaponLevel(w, "対") > 0 Then
				'UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If t.MainPilot.Level Mod WeaponLevel(w, "対") <> 0 Then
					Damage = 0
					Exit Function
				End If
			End If
			
			If is_true_value Or mpskill >= 140 Then
				'弱点、有効、吸収を優先
				If Not .Weakness(wclass) And Not .Effective(wclass) And Not .Absorb(wclass) Then
					'無効化
					If .Immune(wclass) Then
						Damage = 0
						Exit Function
						'耐性
					ElseIf .Resist(wclass) Then 
						Damage = Damage \ 2
					End If
				End If
			End If
			
			'盲目状態には視覚攻撃は効かない
			If is_true_value Or mpskill >= 140 Then
				If InStrNotNest(wclass, "視") > 0 Then
					If .IsConditionSatisfied("盲目") Then
						Damage = 0
						Exit Function
					End If
				End If
			End If
			
			'機械には精神攻撃は効かない
			If is_true_value Or mpskill >= 140 Then
				If InStrNotNest(wclass, "精") > 0 Then
					If .MainPilot.Personality = "機械" Then
						Damage = 0
						Exit Function
					End If
				End If
			End If
			
			'性別限定武器
			If InStrNotNest(wclass, "♂") > 0 Then
				If .MainPilot.Sex <> "男性" Then
					Damage = 0
					Exit Function
				End If
			End If
			If InStrNotNest(wclass, "♀") > 0 Then
				If .MainPilot.Sex <> "女性" Then
					Damage = 0
					Exit Function
				End If
			End If
			
			'寝こみを襲うとダメージ1.5倍
			If .IsConditionSatisfied("睡眠") Then
				Damage = 1.5 * Damage
			End If
			
			With MainPilot
				'高気力時のダメージ増加能力
				If .Morale >= 130 Then
					If IsOptionDefined("ダメージ倍率低下") Then
						If .IsSkillAvailable("潜在力開放") Then
							Damage = 1.2 * Damage
						End If
						If IsFeatureAvailable("ブースト") Then
							Damage = 1.2 * Damage
						End If
					Else
						If .IsSkillAvailable("潜在力開放") Then
							Damage = 1.25 * Damage
						End If
						If IsFeatureAvailable("ブースト") Then
							Damage = 1.25 * Damage
						End If
					End If
				End If
				
				'得意技
				If .IsSkillAvailable("得意技") Then
					sdata = .SkillData("得意技")
					For i = 1 To Len(sdata)
						If InStrNotNest(wclass, Mid(sdata, i, 1)) > 0 Then
							Damage = 1.2 * Damage
							Exit For
						End If
					Next 
				End If
				
				'不得手
				If .IsSkillAvailable("不得手") Then
					sdata = .SkillData("不得手")
					For i = 1 To Len(sdata)
						If InStrNotNest(wclass, Mid(sdata, i, 1)) > 0 Then
							Damage = 0.8 * Damage
							Exit For
						End If
					Next 
				End If
			End With
			
			'ハンター能力
			'(ターゲットのMainPilotを参照するため、「With .MainPilot」は使えない)
			If MainPilot.IsSkillAvailable("ハンター") Then
				For i = 1 To MainPilot.CountSkill
					If MainPilot.Skill(i) = "ハンター" Then
						sdata = MainPilot.SkillData(i)
						For j = 2 To LLength(sdata)
							tname = LIndex(sdata, j)
							If .Name = tname Or .Class0 = tname Or .Size & "サイズ" = tname Or .MainPilot.Name = tname Or .MainPilot.Sex = tname Then
								Exit For
							End If
						Next 
						If j <= LLength(sdata) Then
							Damage = (10 + MainPilot.SkillLevel(i)) * Damage \ 10
							Exit For
						End If
					End If
				Next 
				If IsConditionSatisfied("ハンター付加") Or IsConditionSatisfied("ハンター付加２") Then
					sdata = MainPilot.SkillData("ハンター")
					For i = 2 To LLength(sdata)
						tname = LIndex(sdata, i)
						If .Name = tname Or .Class0 = tname Or .Size & "サイズ" = tname Or .MainPilot.Name = tname Or .MainPilot.Sex = tname Then
							Exit For
						End If
					Next 
					If i <= LLength(sdata) Then
						Damage = (10 + MainPilot.SkillLevel("ハンター")) * Damage \ 10
					End If
				End If
			End If
			
			'スペシャルパワー、特殊状態によるダメージ増加
			dmg_mod = 1
			If IsConditionSatisfied("攻撃力ＵＰ") Or IsConditionSatisfied("狂戦士") Then
				If IsOptionDefined("ダメージ倍率低下") Then
					dmg_mod = 1.2
				Else
					dmg_mod = 1.25
				End If
			End If
			'サポートアタックの場合はスペシャルパワーによる修正が無い
			If Not is_support_attack Then
				If is_true_value Or mpskill >= 160 Then
					'スペシャルパワーによるダメージ増加は特殊状態による増加と重複しない
					dmg_mod = MaxDbl(dmg_mod, 1 + 0.1 * SpecialPowerEffectLevel("ダメージ増加"))
					dmg_mod = dmg_mod + 0.1 * .SpecialPowerEffectLevel("被ダメージ増加")
				End If
			End If
			Damage = dmg_mod * Damage
			
			'スペシャルパワー、特殊状態、サポートアタックによるダメージ低下
			If is_true_value Or mpskill >= 160 Then
				dmg_mod = 1
				dmg_mod = dmg_mod - 0.1 * SpecialPowerEffectLevel("ダメージ低下")
				dmg_mod = dmg_mod - 0.1 * .SpecialPowerEffectLevel("被ダメージ低下")
				Damage = dmg_mod * Damage
			End If
			If IsConditionSatisfied("攻撃力ＤＯＷＮ") Then
				Damage = 0.75 * Damage
			End If
			If IsConditionSatisfied("恐怖") Then
				Damage = 0.8 * Damage
			End If
			If is_support_attack Then
				'サポートアタックダメージ低下
				If IsOptionDefined("サポートアタックダメージ低下") Then
					Damage = 0.7 * Damage
				End If
			End If
			
			'レジスト能力
			dmg_mod = 0
			If Not .IsFeatureAvailable("レジスト") Then
				GoTo SkipResist
			End If
			'ザコはレジストを考慮しない
			If Not is_true_value And mpskill < 150 Then
				GoTo SkipResist
			End If
			For i = 1 To .CountFeature
				If .Feature(i) = "レジスト" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'必要条件
					If IsNumeric(LIndex(fdata, 3)) Then
						nmorale = CShort(LIndex(fdata, 3))
					Else
						nmorale = 0
					End If
					
					'オプション
					neautralize = False
					slevel = 0
					For j = 4 To LLength(fdata)
						opt = LIndex(fdata, j)
						idx = InStr(opt, "*")
						If idx > 0 Then
							lv_mod = StrToDbl(Mid(opt, idx + 1))
							opt = Left(opt, idx - 1)
						Else
							lv_mod = -1
						End If
						
						Select Case opt
							Case "能力必要"
								'スキップ
							Case "同調率"
								If lv_mod = -1 Then
									lv_mod = 0.5
								End If
								slevel = lv_mod * (.MainPilot.SynchroRate - 30)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "霊力"
								If lv_mod = -1 Then
									lv_mod = 0.2
								End If
								slevel = lv_mod * .MainPilot.Plana
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "オーラ"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超能力"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超感覚"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * (.MainPilot.SkillLevel("超感覚") + .MainPilot.SkillLevel("知覚強化"))
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .MainPilot.SkillLevel(opt)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'発動可能？
					If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'レジスト発動
						dmg_mod = dmg_mod + 10 * flevel + slevel
					End If
				End If
			Next 
			
			Damage = Damage * (100 - dmg_mod) \ 100
			
SkipResist: 
			
			If BCList.IsDefined("最終ダメージ") Then
				'バトルコンフィグデータによる計算実行
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = Damage
				Damage = BCList.Item("最終ダメージ").Calculate()
			End If
			
			'最低ダメージは10
			If dmg_mod < 100 Then
				If Damage < 10 Then
					' MOD START MARGE
					'                Damage = 10
					If IsOptionDefined("ダメージ下限解除") Then
						Damage = MaxLng(Damage, 0)
					ElseIf IsOptionDefined("ダメージ下限１") Then 
						Damage = MaxLng(Damage, 1)
					Else
						Damage = 10
					End If
					' MOD END MARGE
				End If
			End If
			
			'ダメージを吸収する場合は最後に反転
			If is_true_value Or mpskill >= 140 Then
				'弱点、有効を優先
				If Not .Weakness(wclass) And Not .Effective(wclass) Then
					'吸収
					If Damage > 0 And .Absorb(wclass) Then
						Damage = -Damage \ 2
					End If
				End If
			End If
		End With
	End Function
	
	'クリティカルの発生率
	Public Function CriticalProbability(ByVal w As Short, ByRef t As Unit, Optional ByVal def_mode As String = "") As Short
		Dim i, prob, idx As Short
		Dim wclass As String
		Dim buf, c As String
		Dim is_special As Boolean
		'クリティカル攻撃、防御の一時保存変数
		Dim ed_crtatk, ed_crtdfe As Short
		
		With t
			If IsNormalWeapon(w) Then
				'通常攻撃
				
				'スペシャルパワーとの効果の重ね合わせが禁止されている場合
				If IsOptionDefined("スペシャルパワー使用時クリティカル無効") Or IsOptionDefined("精神コマンド使用時クリティカル無効") Then
					If IsUnderSpecialPowerEffect("ダメージ増加") Then
						Exit Function
					End If
				End If
				
				'攻撃側による補正
				If BCList.IsDefined("クリティカル攻撃補正") Then
					'バトルコンフィグデータの設定による修正
					' 一時保存変数に一時保存
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackExp = WeaponCritical(w)
					ed_crtatk = BCList.Item("クリティカル攻撃補正").Calculate()
				Else
					' 一時保存変数に一時保存
					ed_crtatk = WeaponCritical(w) + MainPilot.Technique
				End If
				
				'防御側による補正
				If BCList.IsDefined("クリティカル防御補正") Then
					'バトルコンフィグデータの設定による修正
					' 一時保存変数に一時保存
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = t
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					ed_crtdfe = BCList.Item("クリティカル防御補正").Calculate()
				Else
					' 一時保存変数に一時保存
					ed_crtdfe = .MainPilot.Technique
				End If
				
				' クリティカル発生率計算
				If BCList.IsDefined("クリティカル発生率") Then
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackVariable = ed_crtatk
					BCVariable.DffenceVariable = ed_crtdfe
					prob = BCList.Item("クリティカル発生率").Calculate()
				Else
					prob = ed_crtatk - ed_crtdfe
				End If
				
				'超反応による修正
				prob = prob + 2 * MainPilot.SkillLevel("超反応") - 2 * .MainPilot.SkillLevel("超反応")
				
				'超能力による修正
				If MainPilot.IsSkillAvailable("超能力") Then
					prob = prob + 5
				End If
				
				'底力、超底力、覚悟による修正
				If HP <= MaxHP \ 4 Then
					If MainPilot.IsSkillAvailable("底力") Or MainPilot.IsSkillAvailable("超底力") Or MainPilot.IsSkillAvailable("覚悟") Then
						prob = prob + 50
					End If
				End If
				
				'スペシャルパワーにる修正
				prob = prob + 10 * SpecialPowerEffectLevel("クリティカル率増加")
			Else
				'特殊効果を伴う攻撃
				is_special = True
				
				'攻撃側による補正
				If BCList.IsDefined("特殊効果攻撃補正") Then
					'バトルコンフィグデータの設定による修正
					' 一時保存変数に一時保存
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackExp = WeaponCritical(w)
					ed_crtatk = BCList.Item("特殊効果攻撃補正").Calculate()
				Else
					' 一時保存変数に一時保存
					ed_crtatk = WeaponCritical(w) + MainPilot.Technique \ 2
				End If
				
				'防御側による補正
				If BCList.IsDefined("特殊効果防御補正") Then
					'バトルコンフィグデータの設定による修正
					' 一時保存変数に一時保存
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = t
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					'特殊効果の場合は相手がザコの時に確率が増加
					If InStr(.MainPilot.Name, "(ザコ)") > 0 Then
						BCVariable.CommonEnemy = 30
					End If
					ed_crtdfe = BCList.Item("特殊効果防御補正").Calculate()
				Else
					' 一時保存変数に一時保存
					ed_crtdfe = .MainPilot.Technique \ 2
					
					'特殊効果の場合は相手がザコの時に確率が増加
					If InStr(.MainPilot.Name, "(ザコ)") > 0 Then
						' 一時保存変数に一時保存
						ed_crtdfe = ed_crtdfe - 30
					End If
				End If
				
				' 特殊効果発生率計算
				If BCList.IsDefined("特殊効果発生率") Then
					'事前にデータを登録
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackVariable = ed_crtatk
					BCVariable.DffenceVariable = ed_crtdfe
					prob = BCList.Item("特殊効果発生率").Calculate()
				Else
					prob = ed_crtatk - ed_crtdfe
				End If
				
				'抵抗力による修正
				prob = prob - 10 * .FeatureLevel("抵抗力")
			End If
			
			'不意打ち
			If IsFeatureAvailable("ステルス") And Not IsConditionSatisfied("ステルス無効") And Not .IsFeatureAvailable("ステルス無効化") And IsWeaponClassifiedAs(w, "忍") Then
				prob = prob + 10
			End If
			
			'相手が動けなければ確率アップ
			If .IsConditionSatisfied("行動不能") Or .IsConditionSatisfied("石化") Or .IsConditionSatisfied("凍結") Or .IsConditionSatisfied("麻痺") Or .IsConditionSatisfied("睡眠") Or .IsUnderSpecialPowerEffect("行動不能") Then
				prob = prob + 10
			End If
			
			'以下の修正は特殊効果発動確率にのみ影響
			If is_special Then
				wclass = WeaponClass(w)
				
				'封印攻撃は弱点、有効を持つユニット以外には効かない
				If InStrNotNest(wclass, "封") > 0 Then
					buf = .strWeakness & .strEffective
					For i = 1 To Len(buf)
						'属性をひとまとめずつ取得
						c = GetClassBundle(buf, i)
						If c <> "物" And c <> "魔" Then
							If InStrNotNest(wclass, c) > 0 Then
								Exit For
							End If
						End If
					Next 
					If i > Len(buf) Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'限定攻撃は弱点、有効を持つユニット以外には効かない
				idx = InStrNotNest(wclass, "限")
				If idx > 0 Then
					buf = .strWeakness & .strEffective
					For i = 1 To Len(buf)
						'属性をひとまとめずつ取得
						c = GetClassBundle(buf, i)
						If c <> "物" And c <> "魔" Then
							If InStrNotNest(wclass, c) > idx Then
								Exit For
							End If
						End If
					Next 
					If i > Len(buf) Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'特定レベル限定攻撃
				If InStrNotNest(wclass, "対") > 0 Then
					'UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
					If t.MainPilot.Level Mod WeaponLevel(w, "対") <> 0 Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'クリティカル率については、
				'弱、効属性の指定属性に対しての防御特性を考慮する。
				buf = ""
				i = InStrNotNest(wclass, "弱")
				Do While i > 0
					buf = buf & Mid(GetClassBundle(wclass, i), 2)
					i = InStrNotNest(wclass, "弱", i + 1)
				Loop 
				i = InStrNotNest(wclass, "効")
				Do While i > 0
					buf = buf & Mid(GetClassBundle(wclass, i), 2)
					i = InStrNotNest(wclass, "効", i + 1)
				Loop 
				buf = buf & wclass
				
				'弱点
				If .Weakness(buf) Then
					prob = prob + 10
					'有効
				ElseIf .Effective(buf) Then 
					'変化なし
					'封印技
				ElseIf InStrNotNest(wclass, "封") > 0 Then 
					CriticalProbability = 0
					Exit Function
					'限定技
				ElseIf InStrNotNest(wclass, "限") > 0 Then 
					CriticalProbability = 0
					Exit Function
					'吸収
				ElseIf .Absorb(buf) Then 
					CriticalProbability = 0
					Exit Function
					'無効化
				ElseIf .Immune(buf) Then 
					CriticalProbability = 0
					Exit Function
					'耐性
				ElseIf .Resist(buf) Then 
					prob = prob \ 2
				End If
				
				'盲目状態には視覚攻撃は効かない
				If InStrNotNest(wclass, "視") > 0 Then
					If .IsConditionSatisfied("盲目") Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'機械には精神攻撃は効かない
				If InStrNotNest(wclass, "精") > 0 Then
					If .MainPilot.Personality = "機械" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'性別限定武器
				If InStrNotNest(wclass, "♂") > 0 Then
					If .MainPilot.Sex <> "男性" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				If InStrNotNest(wclass, "♀") > 0 Then
					If .MainPilot.Sex <> "女性" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
			End If
		End With
		
		'防御時はクリティカル発生確率が半減
		If def_mode = "防御" Then
			prob = prob \ 2
		End If
		
		' 最終クリティカル/特殊効果を定義する。これがないときは何もしない
		If IsNormalWeapon(w) Then
			'クリティカル
			If BCList.IsDefined("最終クリティカル発生率") Then
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = prob
				prob = BCList.Item("最終クリティカル発生率").Calculate()
			End If
		Else
			'特殊効果
			If BCList.IsDefined("最終特殊効果発生率") Then
				'事前にデータを登録
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = prob
				prob = BCList.Item("最終特殊効果発生率").Calculate()
			End If
		End If
		
		If prob > 100 Then
			CriticalProbability = 100
		ElseIf prob < 1 Then 
			CriticalProbability = 1
		Else
			CriticalProbability = prob
		End If
	End Function
	
	'武器wでユニットtに攻撃をかけた時のダメージの期待値
	Public Function ExpDamage(ByVal w As Short, ByRef t As Unit, ByVal is_true_value As Boolean, Optional ByVal dmg_mod As Double = 0) As Integer
		Dim dmg As Integer
		Dim j, i, idx As Short
		Dim slevel As Double
		Dim wclass As String
		Dim fname, fdata As String
		Dim flevel As Double
		Dim ecost, nmorale As Integer
		Dim neautralize As Boolean
		Dim lv_mod As Double
		Dim opt As String
		
		wclass = WeaponClass(w)
		
		'攻撃力が0であれば常にダメージ0
		If WeaponPower(w, "") <= 0 Then
			Exit Function
		End If
		
		'ダメージ
		dmg = Damage(w, t, is_true_value)
		
		'ダメージに修正を加える場合
		If dmg_mod > 0 Then
			If InStrNotNest(wclass, "殺") = 0 Then
				dmg = dmg * dmg_mod
			End If
		End If
		
		'抹殺攻撃は一撃で相手を倒せない限り効果がない
		If InStrNotNest(wclass, "殺") > 0 Then
			If t.HP > dmg Then
				Exit Function
			End If
		End If
		
		'ダメージが与えられない場合
		If dmg <= 0 Then
			'地形適応や封印武器、限定武器、性別限定武器、無効化、吸収が原因であれば期待値は0
			If WeaponAdaption(w, (t.Area)) = 0 Or InStrNotNest(wclass, "封") > 0 Or InStrNotNest(wclass, "限") > 0 Or InStrNotNest(wclass, "♂") > 0 Or InStrNotNest(wclass, "♀") > 0 Or t.Immune(wclass) Or t.Absorb(wclass) Then
				Exit Function
			End If
			
			'それ以外の要因であればダミーでダメージwとする。
			'こうしておかないと敵が攻撃が無駄の場合はまったく自分から
			'攻撃しなくなってしまうので。
			'単純にExpDamage=1などとしないのは攻撃力の高い武器を優先させて使わせるため
			ExpDamage = w
			Exit Function
		End If
		
		'バリア無効化
		If InStrNotNest(wclass, "無") > 0 Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			'抹殺攻撃は一撃で相手を倒せない限り効果がない
			If InStrNotNest(wclass, "殺") > 0 Then
				If t.HP > dmg Then
					Exit Function
				End If
			End If
			ExpDamage = dmg
			Exit Function
		End If
		
		'技量の低い敵はバリアを考慮せず攻撃をかける
		With MainPilot
			If Not is_true_value And .TacticalTechnique < 150 Then
				'抹殺攻撃は一撃で相手を倒せない限り効果がない
				If InStrNotNest(wclass, "殺") > 0 Then
					If t.HP > dmg Then
						Exit Function
					End If
				End If
				ExpDamage = dmg
				Exit Function
			End If
		End With
		
		With t
			'バリア能力
			For i = 1 To .CountFeature
				If .Feature(i) = "バリア" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'必要条件
					If IsNumeric(LIndex(fdata, 3)) Then
						ecost = CShort(LIndex(fdata, 3))
					Else
						ecost = 10
					End If
					If IsNumeric(LIndex(fdata, 4)) Then
						nmorale = CShort(LIndex(fdata, 4))
					Else
						nmorale = 0
					End If
					
					'オプション
					neautralize = False
					slevel = 0
					For j = 5 To LLength(fdata)
						opt = LIndex(fdata, j)
						idx = InStr(opt, "*")
						If idx > 0 Then
							lv_mod = StrToDbl(Mid(opt, idx + 1))
							opt = Left(opt, idx - 1)
						Else
							lv_mod = -1
						End If
						Select Case opt
							Case "相殺"
								If IsSameCategory(fdata, FeatureData("バリア")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "中和"
								If IsSameCategory(fdata, FeatureData("バリア")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("バリア")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "近接無効"
								If InStrNotNest(wclass, "武") > 0 Or InStrNotNest(wclass, "突") > 0 Or InStrNotNest(wclass, "接") > 0 Then
									neautralize = True
								End If
							Case "手動"
								neautralize = True
							Case "能力必要"
								'スキップ
							Case "同調率"
								If lv_mod = -1 Then
									lv_mod = 20
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "霊力"
								If lv_mod = -1 Then
									lv_mod = 10
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "オーラ"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超能力"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'バリア無効化で無効化されている？
					If t.IsConditionSatisfied("バリア無効化") Then
						If InStr(fdata, "バリア無効化無効") = 0 Then
							neautralize = True
						End If
					End If
					
					'発動可能？
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'バリア発動
						If dmg <= 1000 * flevel + slevel Then
							ExpDamage = w
							Exit Function
						End If
					End If
				End If
			Next 
			
			'フィールド能力
			For i = 1 To .CountFeature
				If .Feature(i) = "フィールド" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'必要条件
					If IsNumeric(LIndex(fdata, 3)) Then
						ecost = CShort(LIndex(fdata, 3))
					Else
						ecost = 0
					End If
					If IsNumeric(LIndex(fdata, 4)) Then
						nmorale = CShort(LIndex(fdata, 4))
					Else
						nmorale = 0
					End If
					
					'オプション
					neautralize = False
					slevel = 0
					For j = 5 To LLength(fdata)
						opt = LIndex(fdata, j)
						idx = InStr(opt, "*")
						If idx > 0 Then
							lv_mod = StrToDbl(Mid(opt, idx + 1))
							opt = Left(opt, idx - 1)
						Else
							lv_mod = -1
						End If
						Select Case opt
							Case "相殺"
								If IsSameCategory(fdata, FeatureData("フィールド")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "中和"
								If IsSameCategory(fdata, FeatureData("フィールド")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("フィールド")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "近接無効"
								If InStrNotNest(wclass, "武") > 0 Or InStrNotNest(wclass, "突") > 0 Or InStrNotNest(wclass, "接") > 0 Then
									neautralize = True
								End If
							Case "手動"
								neautralize = True
							Case "能力必要"
								'スキップ
							Case "同調率"
								If lv_mod = -1 Then
									lv_mod = 20
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "霊力"
								If lv_mod = -1 Then
									lv_mod = 10
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "オーラ"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超能力"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'バリア無効化で無効化されている？
					If t.IsConditionSatisfied("バリア無効化") Then
						If InStr(fdata, "バリア無効化無効") = 0 Then
							neautralize = True
						End If
					End If
					
					'発動可能？
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'フィールド発動
						If dmg <= 500 * flevel + slevel Then
							ExpDamage = w
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							dmg = dmg - 500 * flevel - slevel
						End If
					End If
				End If
			Next 
			
			'プロテクション能力
			For i = 1 To .CountFeature
				If .Feature(i) = "プロテクション" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'必要条件
					If IsNumeric(LIndex(fdata, 3)) Then
						ecost = CShort(LIndex(fdata, 3))
					Else
						ecost = 10
					End If
					If IsNumeric(LIndex(fdata, 4)) Then
						nmorale = CShort(LIndex(fdata, 4))
					Else
						nmorale = 0
					End If
					
					'オプション
					neautralize = False
					slevel = 0
					For j = 5 To LLength(fdata)
						opt = LIndex(fdata, j)
						idx = InStr(opt, "*")
						If idx > 0 Then
							lv_mod = StrToDbl(Mid(opt, idx + 1))
							opt = Left(opt, idx - 1)
						Else
							lv_mod = -1
						End If
						Select Case opt
							Case "相殺"
								If IsSameCategory(fdata, FeatureData("プロテクション")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "中和"
								If IsSameCategory(fdata, FeatureData("プロテクション")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("プロテクション")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "近接無効"
								If InStrNotNest(wclass, "武") > 0 Or InStrNotNest(wclass, "突") > 0 Or InStrNotNest(wclass, "接") > 0 Then
									neautralize = True
								End If
							Case "手動"
								neautralize = True
							Case "能力必要"
								'スキップ
							Case "同調率"
								If lv_mod = -1 Then
									lv_mod = 0.5
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "霊力"
								If lv_mod = -1 Then
									lv_mod = 0.2
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "オーラ"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "超能力"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "能力必要") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'バリア無効化で無効化されている？
					If t.IsConditionSatisfied("バリア無効化") Then
						If InStr(fdata, "バリア無効化無効") = 0 Then
							neautralize = True
						End If
					End If
					
					'発動可能？
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'プロテクション発動
						dmg = dmg * (100 - 10 * flevel - slevel) \ 100
						If dmg <= 0 Then
							ExpDamage = w
							Exit Function
						End If
					End If
				End If
			Next 
			
			'対ビーム用防御能力
			If InStrNotNest(wclass, "Ｂ") > 0 Then
				'ビーム吸収
				If .IsFeatureAvailable("ビーム吸収") Then
					ExpDamage = w
					Exit Function
				End If
			End If
			
			'抹殺攻撃は一撃で相手を倒せる場合にのみ有効
			If InStrNotNest(wclass, "殺") > 0 Then
				If dmg < .HP Then
					dmg = 0
				End If
			End If
			
			'盾防御
			If .IsFeatureAvailable("盾") And .MainPilot.IsSkillAvailable("Ｓ防御") And .MaxAction > 0 And Not IsWeaponClassifiedAs(w, "精") And Not IsWeaponClassifiedAs(w, "浸") And Not IsWeaponClassifiedAs(w, "殺") And (.IsConditionSatisfied("盾付加") Or .FeatureLevel("盾") > .ConditionLevel("盾ダメージ")) Then
				If IsWeaponClassifiedAs(w, "破") Then
					dmg = dmg - 50 * (.MainPilot.SkillLevel("Ｓ防御") + 4)
				Else
					dmg = dmg - 100 * (.MainPilot.SkillLevel("Ｓ防御") + 4)
				End If
			End If
		End With
		
		'ダメージが減少されて0以下になった場合もダミーで1ダメージ
		If dmg <= 0 Then
			dmg = 1
		End If
		
		'抹殺攻撃は一撃で相手を倒せない限り効果がない
		If InStr(CStr(w), "殺") > 0 Then
			If t.HP > dmg Then
				Exit Function
			End If
		End If
		
		ExpDamage = dmg
	End Function
	
	
	' === 防御属性判定処理 ===
	
	'属性 aname に対して吸収属性を持つか？
	Public Function Absorb(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'全属性に有効な場合
		If InStrNotNest(strAbsorb, "全") > 0 Then
			Absorb = True
			Exit Function
		End If
		
		'無属性は物理攻撃に分類される
		If Len(aname) = 0 Then
			If InStrNotNest(strAbsorb, "物") > 0 Then
				Absorb = True
			End If
			Exit Function
		End If
		
		'属性に該当するかを判定
		i = 1
		slen = Len(strAbsorb)
		Do While i <= slen
			'属性をひとまとめずつ取得
			c = GetClassBundle(strAbsorb, i)
			Select Case c
				Case "物"
					If InStrNotNest(aname, "魔") = 0 And InStrNotNest(aname, "精") = 0 Then
						Absorb = True
						Exit Do
					End If
				Case "魔"
					'魔法武器以外の魔属性なら特性が有効
					If InStrNotNest(aname, "魔") > 0 Then
						If InStrNotNest(aname, "魔武") = 0 And InStrNotNest(aname, "魔突") = 0 And InStrNotNest(aname, "魔接") = 0 And InStrNotNest(aname, "魔銃") = 0 And InStrNotNest(aname, "魔実") = 0 Then
							Absorb = True
							Exit Do
						End If
					End If
				Case Else
					If InStrNotNest(aname, c) > 0 Then
						Absorb = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
	End Function
	
	'属性 aname に対して無効化属性を持つか？
	Public Function Immune(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'全属性に有効な場合
		If InStrNotNest(strImmune, "全") > 0 Then
			Immune = True
			Exit Function
		End If
		
		'無属性は物理攻撃に分類される
		If Len(aname) = 0 Then
			If InStrNotNest(strImmune, "物") > 0 Then
				Immune = True
			End If
			Exit Function
		End If
		
		'属性に該当するかを判定
		i = 1
		slen = Len(strImmune)
		Do While i <= slen
			'属性をひとまとめずつ取得
			c = GetClassBundle(strImmune, i)
			Select Case c
				Case "物"
					If InStrNotNest(aname, "魔") = 0 And InStrNotNest(aname, "精") = 0 Then
						Immune = True
						Exit Do
					End If
				Case "魔"
					'魔法武器以外の魔属性なら特性が有効
					If InStrNotNest(aname, "魔") > 0 And InStrNotNest(aname, "魔武") = 0 And InStrNotNest(aname, "魔突") = 0 And InStrNotNest(aname, "魔接") = 0 And InStrNotNest(aname, "魔銃") = 0 And InStrNotNest(aname, "魔実") = 0 Then
						Immune = True
						Exit Do
					End If
				Case Else
					If InStrNotNest(aname, c) > 0 Then
						Immune = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
	End Function
	
	'属性 aname に対して耐性属性を持つか？
	Public Function Resist(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'全属性に有効な場合
		If InStrNotNest(strResist, "全") > 0 Then
			Resist = True
			Exit Function
		End If
		
		'無属性は物理攻撃に分類される
		If Len(aname) = 0 Then
			If InStrNotNest(strResist, "物") > 0 Then
				Resist = True
			End If
			Exit Function
		End If
		
		'属性に該当するかを判定
		i = 1
		slen = Len(strResist)
		Do While i <= slen
			'属性をひとまとめずつ取得
			c = GetClassBundle(strResist, i)
			Select Case c
				Case "物"
					If InStrNotNest(aname, "魔") = 0 And InStrNotNest(aname, "精") = 0 Then
						Resist = True
						Exit Do
					End If
				Case "魔"
					'魔法武器以外の魔属性なら特性が有効
					If InStrNotNest(aname, "魔") > 0 And InStrNotNest(aname, "魔武") = 0 And InStrNotNest(aname, "魔突") = 0 And InStrNotNest(aname, "魔接") = 0 And InStrNotNest(aname, "魔銃") = 0 And InStrNotNest(aname, "魔実") = 0 Then
						Resist = True
						Exit Do
					End If
				Case Else
					If InStrNotNest(aname, c) > 0 Then
						Resist = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
	End Function
	
	'属性 aname に対して弱点属性を持つか？
	Public Function Weakness(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'全属性に有効な場合
		If InStrNotNest(strWeakness, "全") > 0 Then
			Weakness = True
			Exit Function
		End If
		
		If Len(aname) = 0 Then
			If InStrNotNest(strWeakness, "物") > 0 Then
				Weakness = True
			End If
			Exit Function
		End If
		
		i = 1
		slen = Len(strWeakness)
		Do While i <= slen
			'属性をひとまとめずつ取得
			c = GetClassBundle(strWeakness, i)
			Select Case c
				Case "物"
					If InStrNotNest(aname, "魔") = 0 And InStrNotNest(aname, "精") = 0 Then
						Weakness = True
						Exit Do
					End If
				Case Else
					If InStrNotNest(aname, c) > 0 Then
						Weakness = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
	End Function
	
	'属性 aname に対して有効属性を持つか？
	Public Function Effective(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'全属性に有効な場合
		If InStrNotNest(strEffective, "全") > 0 Then
			Effective = True
			Exit Function
		End If
		
		If Len(aname) = 0 Then
			If InStrNotNest(strEffective, "物") > 0 Then
				Effective = True
			End If
			Exit Function
		End If
		
		i = 1
		slen = Len(strEffective)
		Do While i <= slen
			'属性をひとまとめずつ取得
			c = GetClassBundle(strEffective, i)
			Select Case c
				Case "物"
					If InStrNotNest(aname, "魔") = 0 And InStrNotNest(aname, "精") = 0 Then
						Effective = True
						Exit Do
					End If
				Case Else
					If InStrNotNest(aname, c) > 0 Then
						Effective = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
	End Function
	
	'属性 aname に対して特殊効果無効化属性を持つか？
	Public Function SpecialEffectImmune(ByRef aname As String) As Boolean
		'全属性に有効な場合
		If InStrNotNest(strSpecialEffectImmune, "全") > 0 Then
			SpecialEffectImmune = True
			Exit Function
		End If
		
		If Len(aname) = 0 Then
			Exit Function
		End If
		
		If InStrNotNest(strSpecialEffectImmune, aname) > 0 Then
			SpecialEffectImmune = True
			Exit Function
		End If
		
		'無効化や弱点と違い、クリティカル率のみなので
		'「火」に対する防御特性が「弱火」のクリティカル率に影響する点について
		'直接関数内に記述できる。
		If Left(aname, 1) = "弱" Or Left(aname, 1) = "効" Then
			If InStrNotNest(strSpecialEffectImmune, aname) > 0 Then
				SpecialEffectImmune = True
				Exit Function
			End If
		End If
	End Function
	
	'属性の該当判定
	' aclass1 が防御属性、aclass2 が武器属性
	Public Function IsAttributeClassified(ByRef aclass1 As String, ByRef aclass2 As String) As Boolean
		Dim attr As String
		Dim alen, i As Short
		Dim with_not As Boolean
		
		If Len(aclass1) = 0 Then
			IsAttributeClassified = True
			Exit Function
		End If
		If aclass1 = "全" Then
			IsAttributeClassified = True
			Exit Function
		End If
		
		'無属性の攻撃は物理攻撃に分類される
		If Len(aclass2) = 0 Then
			If InStrNotNest(aclass1, "物") > 0 Then
				IsAttributeClassified = True
			End If
			If InStrNotNest(aclass1, "!") > 0 Then
				IsAttributeClassified = Not IsAttributeClassified
			End If
			GoTo EndOfFunction
		End If
		
		i = 1
		alen = Len(aclass1)
		Do While i <= alen
			attr = GetClassBundle(aclass1, i)
			Select Case attr
				Case "物"
					If InStrNotNest(aclass2, "魔") = 0 And InStrNotNest(aclass2, "精") = 0 Then
						IsAttributeClassified = True
						Exit Do
					End If
				Case "魔"
					'魔法武器以外の魔属性なら特性が有効
					If InStrNotNest(aclass2, "魔") > 0 Then
						If InStrNotNest(aclass2, "魔武") = 0 And InStrNotNest(aclass2, "魔突") = 0 And InStrNotNest(aclass2, "魔接") = 0 And InStrNotNest(aclass2, "魔銃") = 0 And InStrNotNest(aclass2, "魔実") = 0 Then
							IsAttributeClassified = True
						ElseIf with_not Then 
							IsAttributeClassified = True
						End If
						Exit Do
					End If
				Case "!"
					with_not = True
				Case Else
					If InStrNotNest(aclass2, attr) > 0 Then
						IsAttributeClassified = True
						Exit Do
					End If
			End Select
			i = i + 1
		Loop 
		
EndOfFunction: 
		If with_not Then
			IsAttributeClassified = Not IsAttributeClassified
		End If
	End Function
	
	
	
	' === 攻撃関連処理 ===
	
	'武器 w でユニット t に攻撃
	'attack_mode は攻撃の種類
	'def_mode はユニット t の防御態勢
	'is_event はイベント(Attackコマンド)による攻撃かどうかを現す
	Public Sub Attack(ByVal w As Short, ByVal t As Unit, ByVal attack_mode As String, ByVal def_mode As String, Optional ByVal is_event As Boolean = False)
		Dim prob As Short
		Dim dmg, prev_hp As Integer
		Dim is_hit, is_critical As Boolean
		Dim critical_type As String
		Dim use_shield, use_shield_msg As Boolean
		Dim is_penetrated As Boolean
		Dim use_protect_msg As Boolean
		Dim use_support_guard As Boolean
		Dim wname, wnickname As String
		Dim fname, uname As String
		Dim msg, buf As String
		Dim k, i, j, num As Short
		Dim p As Pilot
		Dim su, orig_t As Unit
		Dim partners() As Unit
		Dim tx, ty As Short
		Dim tarea As String
		Dim prev_x, prev_y As Short
		Dim prev_area As String
		Dim second_attack As Boolean
		Dim be_quiet As Boolean
		Dim attack_num, hit_count As Integer
		Dim slevel As Short
		Dim saved_selected_unit As Unit
		Dim hp_ratio, en_ratio As Double
		Dim separate_parts As Boolean
		Dim orig_w As Short
		Dim itm As Item
		' ADD START MARGE
		Dim is_ext_anime_defined As Boolean
		' ADD END MARGE
		
		wname = Weapon(w).Name
		wnickname = WeaponNickname(w)
		
		'メッセージ表示用に選択状況を切り替え
		SaveSelections()
		saved_selected_unit = SelectedUnit
		If attack_mode = "反射" Then
			SelectedUnit = SelectedTarget
			SelectedTarget = Me
			SelectedUnitForEvent = SelectedTargetForEvent
			SelectedTargetForEvent = Me
			SelectedWeapon = w
			SelectedWeaponName = wname
		Else
			If SelectedUnit Is t Then
				SelectedTWeapon = SelectedWeapon
				SelectedTWeaponName = SelectedWeaponName
			End If
			SelectedWeapon = w
			SelectedWeaponName = wname
			SelectedUnit = Me
			SelectedTarget = t
			SelectedUnitForEvent = Me
			SelectedTargetForEvent = t
		End If
		
		'サポートガードを行ったユニットに関する情報をクリア
		If Not IsDefense() Then
			'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SupportGuardUnit = Nothing
			'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SupportGuardUnit2 = Nothing
		End If
		
		'パイロットのセリフを表示するかどうかを判定
		If attack_mode = "マップ攻撃" Or attack_mode = "反射" Or attack_mode = "当て身技" Or attack_mode = "自動反撃" Then
			be_quiet = True
		Else
			be_quiet = False
		End If
		
		'戦闘アニメを表示する場合はマップウィンドウをクリアする
		If BattleAnimation Then
			If MainWidth <> 15 Then
				ClearUnitStatus()
			End If
			If Not IsOptionDefined("戦闘中画面初期化無効") Then
				RedrawScreen()
			End If
		End If
		
		orig_t = t
		
		'かばった時にターゲットの位置を元のターゲットの位置と一致させるため記録
		tx = t.x
		ty = t.y
		tarea = t.Area
		
begin: 
		
		'情報を更新
		Update()
		MainPilot.UpdateSupportMod()
		t.Update()
		t.MainPilot.UpdateSupportMod()
		
		'ダメージ表示のため、ターゲットのＨＰを記録しておく
		prev_hp = t.HP
		
		'各種設定をリセット
		msg = ""
		is_critical = False
		critical_type = ""
		use_shield = False
		use_shield_msg = False
		use_protect_msg = False
		use_support_guard = False
		is_penetrated = False
		
		'命中率を算出
		prob = HitProbability(w, t, True)
		
		'ダメージを算出
		dmg = Damage(w, t, True, InStr(attack_mode, "援護攻撃") > 0)
		
		'特殊効果を持たない武器ならクリティカルの可能性あり
		If IsNormalWeapon(w) And dmg > 0 Then
			If CriticalProbability(w, t, def_mode) >= Dice(100) Or attack_mode = "統率" Or attack_mode = "同時援護攻撃" Then
				is_critical = True
			End If
		End If
		
		ReDim partners(0)
		ReDim SelectedPartners(0)
		If attack_mode <> "マップ攻撃" And attack_mode <> "反射" And Not second_attack Then
			If IsWeaponClassifiedAs(w, "合") Then
				'合体技の場合にパートナーをハイライト表示
				If WeaponMaxRange(w) = 1 Then
					CombinationPartner("武装", w, partners, tx, ty)
				Else
					CombinationPartner("武装", w, partners)
				End If
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.x, .y) = False
					End With
				Next 
				If Not BattleAnimation Then
					MaskScreen()
				End If
			ElseIf Not is_critical And dmg > 0 And InStr(attack_mode, "援護攻撃") = 0 Then 
				'連携攻撃が発動するかを判定
				'（連携攻撃は合体技では発動しない）
				If Weapon(w).MaxRange > 1 Then
					su = LookForAttackHelp(x, y)
				Else
					su = LookForAttackHelp(tx, ty)
				End If
				If Not su Is Nothing Then
					'連携攻撃発動
					MaskData(su.x, su.y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					If IsMessageDefined("連携攻撃(" & su.MainPilot.Name & ")", True) Then
						PilotMessage("連携攻撃(" & su.MainPilot.Name & ")")
					Else
						PilotMessage("連携攻撃(" & su.MainPilot.Nickname & ")")
					End If
					is_critical = True
					'UPGRADE_NOTE: オブジェクト su をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					su = Nothing
				End If
			End If
		End If
		
		'クリティカルならダメージ1.5倍
		If is_critical Then
			If IsOptionDefined("ダメージ倍率低下") Then
				If IsWeaponClassifiedAs(w, "痛") Then
					dmg = (1 + 0.1 * (WeaponLevel(w, "痛") + 2)) * dmg
				Else
					dmg = 1.2 * dmg
				End If
			Else
				If IsWeaponClassifiedAs(w, "痛") Then
					dmg = (1 + 0.25 * (WeaponLevel(w, "痛") + 2)) * dmg
				Else
					dmg = 1.5 * dmg
				End If
			End If
		End If
		
		'攻撃種類のアニメ表示
		If BattleAnimation Then
			Select Case attack_mode
				Case "援護攻撃", "同時援護攻撃"
					ShowAnimation("援護攻撃発動")
				Case "カウンター"
					ShowAnimation("カウンター発動")
			End Select
		End If
		
		'攻撃側のメッセージ表示
		If Not be_quiet Then
			'攻撃準備の効果音
			If IsAnimationDefined(wname & "(準備)") Then
				PlayAnimation(wname & "(準備)")
			ElseIf IsAnimationDefined(wname) And Not IsOptionDefined("武器準備アニメ非表示") And WeaponAnimation Then 
				PlayAnimation(wname & "(準備)")
			ElseIf IsSpecialEffectDefined(wname & "(準備)") Then 
				SpecialEffect(wname & "(準備)")
			Else
				PrepareWeaponEffect(Me, w)
			End If
			
			'攻撃メッセージの前に出力されるメッセージ
			If second_attack Then
				PilotMessage("再攻撃")
			ElseIf InStr(attack_mode, "援護攻撃") > 0 Then 
				With AttackUnit.CurrentForm.MainPilot
					If IsMessageDefined("サポートアタック(" & .Name & ")") Then
						PilotMessage("サポートアタック(" & .Name & ")")
					ElseIf IsMessageDefined("サポートアタック(" & .Nickname & ")") Then 
						PilotMessage("サポートアタック(" & .Nickname & ")")
					ElseIf IsMessageDefined("サポートアタック") Then 
						PilotMessage("サポートアタック")
					End If
				End With
			ElseIf attack_mode = "カウンター" Then 
				PilotMessage("カウンター")
			ElseIf IsMessageDefined(wname) And wname <> "格闘" And wname <> "射撃" And wname <> "攻撃" And Not IsWeaponClassifiedAs(w, "合") Then 
				If IsMessageDefined("かけ声(" & wname & ")") Then
					PilotMessage("かけ声(" & wname & ")")
				ElseIf IsDefense() Then 
					PilotMessage("かけ声(反撃)")
				Else
					PilotMessage("かけ声")
				End If
			End If
			
			'攻撃メッセージ
			IsWavePlayed = False
			If Not second_attack Then
				If attack_mode = "カウンター" Then
					PilotMessage(wname, "カウンター")
				Else
					PilotMessage(wname, "攻撃")
				End If
			End If
			
			'攻撃アニメ
			If IsDefense() And IsAnimationDefined(wname & "(反撃)") Then
				PlayAnimation(wname & "(反撃)")
			ElseIf IsAnimationDefined(wname & "(攻撃)") Or IsAnimationDefined(wname) Then 
				PlayAnimation(wname & "(攻撃)")
			ElseIf IsSpecialEffectDefined(wname) Then 
				SpecialEffect(wname)
			ElseIf Not IsWavePlayed Then 
				AttackEffect(Me, w)
			End If
		ElseIf attack_mode = "自動反撃" Then 
			'攻撃アニメ
			If IsDefense() And IsAnimationDefined(wname & "(反撃)") Then
				PlayAnimation(wname & "(反撃)")
			ElseIf IsAnimationDefined(wname & "(攻撃)") Or IsAnimationDefined(wname) Then 
				PlayAnimation(wname & "(攻撃)")
			ElseIf IsSpecialEffectDefined(wname) Then 
				SpecialEffect(wname)
			ElseIf Not IsWavePlayed Then 
				AttackEffect(Me, w)
			End If
		End If
		
		If attack_mode <> "マップ攻撃" And attack_mode <> "反射" Then
			'武器使用による弾数＆ＥＮの消費
			UseWeapon(w)
			'武器使用によるＥＮ消費の表示
			UpdateMessageForm(Me, t)
		End If
		
		'防御手段による命中率低下
		If def_mode = "回避" Then
			If Not IsUnderSpecialPowerEffect("絶対命中") And Not t.IsUnderSpecialPowerEffect("無防備") And Not t.IsFeatureAvailable("回避不可") And Not t.IsConditionSatisfied("移動不能") Then
				prob = prob \ 2
			End If
		End If
		
		'反射攻撃の場合は命中率が低下
		If attack_mode = "反射" Then
			prob = prob \ 2
		End If
		
		'攻撃を行ったことについてのシステムメッセージ
		If Not be_quiet Then
			Select Case UBound(partners)
				Case 0
					'通常攻撃
					msg = Nickname & "は"
				Case 1
					'２体合体攻撃
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "と[" & partners(1).MainPilot.Nickname & "]は"
					Else
						msg = Nickname & "達は"
					End If
				Case 2
					'３体合体攻撃
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]、[" & partners(2).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "は[" & partners(1).MainPilot.Nickname & "]、[" & partners(2).MainPilot.Nickname & "]と共に"
					Else
						msg = Nickname & "達は"
					End If
				Case Else
					'３体以上による合体攻撃
					msg = Nickname & "達は"
			End Select
			
			'ジャンプ攻撃
			If t.Area = "空中" And (IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接")) And Not IsTransAvailable("空") Then
				msg = msg & "ジャンプし、"
			End If
			
			If second_attack Then
				msg = msg & "再度"
			ElseIf attack_mode = "カウンター" Or attack_mode = "先制攻撃" Then 
				msg = "先制攻撃！;" & msg & "先手を取り"
			End If
			
			'攻撃の種類によってメッセージを切り替え
			If Right(wnickname, 2) = "攻撃" Or Right(wnickname, 4) = "アタック" Or wnickname = "突撃" Then
				msg = msg & "[" & wnickname & "]をかけた。;"
			ElseIf IsSpellWeapon(w) Then 
				If Right(wnickname, 2) = "呪文" Then
					msg = msg & "[" & wnickname & "]を唱えた。;"
				ElseIf Right(wnickname, 2) = "の杖" Then 
					msg = msg & "[" & Left(wnickname, Len(wnickname) - 2) & "]の呪文を唱えた。;"
				Else
					msg = msg & "[" & wnickname & "]の呪文を唱えた。;"
				End If
			ElseIf IsWeaponClassifiedAs(w, "盗") Then 
				msg = msg & "[" & t.Nickname & "]の持ち物を盗もうとした。;"
			ElseIf IsWeaponClassifiedAs(w, "習") Then 
				msg = msg & "[" & t.Nickname & "]の技を習得しようと試みた。;"
			ElseIf IsWeaponClassifiedAs(w, "実") And (InStr(wnickname, "ミサイル") > 0 Or InStr(wnickname, "ロケット") > 0) Then 
				msg = msg & "[" & wnickname & "]を発射した。;"
			ElseIf Right(wnickname, 1) = "息" Or Right(wnickname, 3) = "ブレス" Or Right(wnickname, 2) = "光線" Or Right(wnickname, 1) = "光" Or Right(wnickname, 3) = "ビーム" Or Right(wnickname, 4) = "レーザー" Then 
				msg = msg & "[" & wnickname & "]を放った。;"
			ElseIf Right(wnickname, 1) = "歌" Then 
				msg = msg & "[" & wnickname & "]を歌った。;"
			ElseIf Right(wnickname, 2) = "踊り" Then 
				msg = msg & "[" & wnickname & "]を踊った。;"
			Else
				msg = msg & "[" & wnickname & "]で攻撃をかけた。;"
			End If
			
			'命中率＆ＣＴ率表示
			If is_event Then
				'イベントによる攻撃の場合は命中率をスペシャルパワーの影響を含めずに表示
				If def_mode = "回避" Then
					buf = "命中率 = " & MinLng(HitProbability(w, t, False) \ 2, 100) & "％" & "（" & VB6.Format(CriticalProbability(w, t, def_mode)) & "％）"
				Else
					buf = "命中率 = " & MinLng(HitProbability(w, t, False), 100) & "％" & "（" & VB6.Format(CriticalProbability(w, t, def_mode)) & "％）"
				End If
			Else
				buf = "命中率 = " & MinLng(prob, 100) & "％" & "（" & VB6.Format(CriticalProbability(w, t, def_mode)) & "％）"
			End If
			
			'攻撃解説表示
			If IsSysMessageDefined(wname) Then
				'「武器名(解説)」のメッセージを使用
				SysMessage(wname, "", buf)
			ElseIf IsSysMessageDefined("攻撃") Then 
				'「攻撃(解説)」のメッセージを使用
				SysMessage("攻撃", "", buf)
			Else
				DisplaySysMessage(msg & buf, BattleAnimation)
			End If
		End If
		
		msg = ""
		
		'防御方法を表示
		Select Case def_mode
			Case "回避"
				If t.IsConditionSatisfied("踊り") Then
					msg = t.Nickname & "は踊っている。;"
				Else
					msg = t.Nickname & "は回避運動をとった。;"
				End If
			Case "防御"
				msg = t.Nickname & "は防御行動をとった。;"
		End Select
		
		'スペシャルパワー「必殺」「瀕死」
		If IsUnderSpecialPowerEffect("絶対破壊") Or IsUnderSpecialPowerEffect("絶対瀕死") Then
			If Not be_quiet Then
				PilotMessage(wname & "(命中)")
			End If
			
			If IsAnimationDefined(wname & "(命中)") Or IsAnimationDefined(wname) Then
				PlayAnimation(wname & "(命中)")
			ElseIf IsSpecialEffectDefined(wname & "(命中)") Then 
				SpecialEffect(wname & "(命中)")
			ElseIf Not IsWavePlayed Then 
				HitEffect(Me, w, t)
			End If
			
			If IsUnderSpecialPowerEffect("絶対瀕死") Then
				' MOD START MARGE
				'            If t.HP > 10 Then
				'                dmg = t.HP - 10
				'            Else
				'                dmg = 0
				'            End If
				If IsOptionDefined("ダメージ下限解除") Or IsOptionDefined("ダメージ下限１") Then
					If t.HP > 1 Then
						dmg = t.HP - 1
					Else
						dmg = 0
					End If
				Else
					If t.HP > 10 Then
						dmg = t.HP - 10
					Else
						dmg = 0
					End If
				End If
				' MOD END MARGE
				
			Else
				dmg = t.HP
			End If
			GoTo ApplyDamage
		End If
		
		'回避能力の処理
		If prob > 0 Then
			If CheckDodgeFeature(w, t, tx, ty, attack_mode, def_mode, dmg, be_quiet) Then
				dmg = 0
				GoTo EndAttack
			End If
		End If
		
		'攻撃回数を求める
		If IsWeaponClassifiedAs(w, "連") Then
			attack_num = WeaponLevel(w, "連")
		Else
			attack_num = 1
		End If
		
		'命中回数を求める
		hit_count = 0
		For i = 1 To attack_num
			If Dice(100) <= prob Then
				hit_count = hit_count + 1
			End If
		Next 
		'命中回数に基いてダメージを修正
		dmg = dmg * hit_count \ attack_num
		
		'攻撃回避時の処理
		If hit_count = 0 Then
			If IsAnimationDefined(wname & "(回避)") Then
				PlayAnimation(wname & "(回避)")
			ElseIf IsSpecialEffectDefined(wname & "(回避)") Then 
				SpecialEffect(wname & "(回避)")
			ElseIf t.IsAnimationDefined("回避") Then 
				t.PlayAnimation("回避")
			ElseIf t.IsSpecialEffectDefined("回避") Then 
				t.SpecialEffect("回避")
			Else
				DodgeEffect(Me, w)
			End If
			
			If Not be_quiet Then
				t.PilotMessage("回避")
				PilotMessage(wname & "(回避)")
			End If
			
			If t.IsSysMessageDefined("回避") Then
				t.SysMessage("回避")
			Else
				Select Case def_mode
					Case "回避"
						If t.IsConditionSatisfied("踊り") Then
							DisplaySysMessage(t.Nickname & "は激しく踊りながら攻撃をかわした。")
						Else
							DisplaySysMessage(t.Nickname & "は回避運動をとり、攻撃をかわした。")
						End If
					Case "防御"
						DisplaySysMessage(t.Nickname & "は防御行動をとったが、攻撃は外れた。")
					Case Else
						DisplaySysMessage(t.Nickname & "は攻撃をかわした。")
				End Select
			End If
			GoTo EndAttack
		End If
		
		'敵ユニットがかばわれた場合の処理
		If su Is Nothing Then
			use_support_guard = False
			If t.IsUnderSpecialPowerEffect("みがわり") Then
				'スペシャルパワー「みがわり」
				i = 1
				Do While i <= t.CountSpecialPower
					'UPGRADE_WARNING: オブジェクト t.SpecialPower(i).IsEffectAvailable(みがわり) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If t.SpecialPower(i).IsEffectAvailable("みがわり") Then
						If PList.IsDefined(t.SpecialPowerData(i)) Then
							su = PList.Item(t.SpecialPowerData(i)).Unit_Renamed
							t.RemoveSpecialPowerInEffect("みがわり")
							i = i - 1
							If Not su Is Nothing Then
								su = su.CurrentForm
								If su.Status_Renamed <> "出撃" Then
									'UPGRADE_NOTE: オブジェクト su をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
									su = Nothing
								End If
							End If
						End If
					End If
					i = i + 1
				Loop 
			ElseIf Not is_event And def_mode <> "マップ攻撃" And def_mode <> "援護防御" Then 
				If t.IsDefense() Then
					'サポートガード
					If UseSupportGuard Then
						su = t.LookForSupportGuard(Me, w)
						If Not su Is Nothing Then
							use_support_guard = True
							'サポートガードの残り回数を減らす
							su.UsedSupportGuard = su.UsedSupportGuard + 1
						End If
					End If
				End If
				If su Is Nothing Then
					'かばう
					su = t.LookForGuardHelp(Me, w, is_critical)
				End If
			End If
			
			If Not su Is Nothing Then
				su.Update()
				
				'メッセージウィンドウの表示を入れ替え
				If Party = "味方" Or Party = "ＮＰＣ" Then
					UpdateMessageForm(su, Me)
				Else
					UpdateMessageForm(Me, su)
				End If
				
				If Not BattleAnimation Then
					'身代わりになるユニットをハイライト表示
					If MaskData(su.x, su.y) Then
						MaskData(su.x, su.y) = False
						MaskScreen()
						MaskData(su.x, su.y) = True
					End If
				End If
				
				'かばう際のメッセージ
				If use_support_guard Then
					If su.IsMessageDefined("サポートガード(" & t.MainPilot.Name & ")") Then
						su.PilotMessage("サポートガード(" & t.MainPilot.Name & ")")
					ElseIf su.IsMessageDefined("サポートガード(" & t.MainPilot.Nickname & ")") Then 
						su.PilotMessage("サポートガード(" & t.MainPilot.Nickname & ")")
					ElseIf su.IsMessageDefined("サポートガード") Then 
						su.PilotMessage("サポートガード")
					End If
				ElseIf su.IsMessageDefined("かばう(" & t.MainPilot.Name & ")") Then 
					su.PilotMessage("かばう(" & t.MainPilot.Name & ")")
					use_protect_msg = True
				ElseIf su.IsMessageDefined("かばう(" & t.MainPilot.Nickname & ")") Then 
					su.PilotMessage("かばう(" & t.MainPilot.Nickname & ")")
					use_protect_msg = True
				End If
				msg = su.MainPilot.Nickname & "は[" & t.MainPilot.Nickname & "]をかばった。;"
				
				'身代わりになるユニットをターゲットの位置まで移動
				With su
					'アニメ表示
					If BattleAnimation Then
						If su.IsAnimationDefined("サポートガード開始") Then
							su.PlayAnimation("サポートガード開始")
						Else
							If Not IsRButtonPressed() Then
								If use_support_guard Then
									MoveUnitBitmap(su, .x, .y, tx, ty, 80, 4)
								Else
									MoveUnitBitmap(su, .x, .y, tx, ty, 50)
								End If
							End If
						End If
					End If
					
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(.x, .y) = Nothing
					prev_x = .x
					prev_y = .y
					prev_area = .Area
					.x = tx
					.y = ty
					.Area = tarea
					MapDataForUnit(.x, .y) = su
				End With
				
				'ターゲットを再設定
				t = su
				SelectedTarget = t
				SelectedTargetForEvent = t
			End If
		End If
		If Not su Is Nothing Then
			'ダメージを再計算
			With t
				prev_hp = .HP
				dmg = Damage(w, t, True)
				If is_critical Then
					If IsOptionDefined("ダメージ倍率低下") Then
						If IsWeaponClassifiedAs(w, "痛") Then
							dmg = (1 + 0.1 * (WeaponLevel(w, "痛") + 2)) * dmg
						Else
							dmg = 1.2 * dmg
						End If
					Else
						If IsWeaponClassifiedAs(w, "痛") Then
							dmg = (1 + 0.25 * (WeaponLevel(w, "痛") + 2)) * dmg
						Else
							dmg = 1.5 * dmg
						End If
					End If
				End If
			End With
			
			'かばう場合は常に全弾命中
			hit_count = attack_num
			
			'常に防御モードに設定
			def_mode = "防御"
			
			'サポートガードを行うユニットに関する情報を記録
			If IsDefense() Then
				SupportGuardUnit2 = su
				SupportGuardUnitHPRatio2 = su.HP / su.MaxHP
			Else
				SupportGuardUnit = su
				SupportGuardUnitHPRatio = su.HP / su.MaxHP
			End If
		End If
		
		'受けの処理
		If CheckParryFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet Or use_protect_msg) Then
			dmg = 0
			GoTo EndAttack
		End If
		
		'防御＆かばう時はダメージを半減
		If Not IsWeaponClassifiedAs(w, "殺") Then
			If def_mode = "防御" And Not t.IsUnderSpecialPowerEffect("無防備") And Not t.IsFeatureAvailable("防御不可") Then
				dmg = dmg \ 2
			End If
		End If
		
		'ダミー
		If CheckDummyFeature(w, t, be_quiet) Then
			dmg = 0
			GoTo EndAttack
		End If
		
		'これ以降は命中時の処理
		
		is_hit = True
		
		'シールド防御判定
		CheckShieldFeature(w, t, dmg, be_quiet, use_shield, use_shield_msg)
		
		'防御能力の処理
		If CheckDefenseFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet Or use_protect_msg, is_penetrated) Then
			If Not be_quiet Then
				PilotMessage(wname & "(攻撃無効化)")
			End If
			dmg = 0
			GoTo EndAttack
		End If
		
		'命中時の特殊効果を表示。
		'防御能力の処理を先に行うのは攻撃無効化の特殊効果を優先させるため。
		IsWavePlayed = False
		If Not be_quiet Then
			PilotMessage(wname & "(命中)")
		End If
		If IsAnimationDefined(wname & "(命中)") Or IsAnimationDefined(wname) Then
			PlayAnimation(wname & "(命中)")
		ElseIf IsSpecialEffectDefined(wname & "(命中)") Then 
			SpecialEffect(wname & "(命中)")
		ElseIf Not IsWavePlayed Then 
			HitEffect(Me, w, t, hit_count)
		End If
		SysMessage(wname & "(命中)")
		
		'無敵の場合
		If t.IsConditionSatisfied("無敵") Then
			If Not be_quiet Then
				t.PilotMessage("攻撃無効化")
				PilotMessage(wname & "(攻撃無効化)")
			End If
			DisplaySysMessage(msg & t.Nickname & "は[" & wnickname & "]を無効化した！")
			dmg = 0
			GoTo EndAttack
		End If
		
		'抹殺攻撃は一撃で倒せる場合にしか効かない
		If IsWeaponClassifiedAs(w, "殺") Then
			If t.HP > dmg Then
				DisplaySysMessage(msg & t.Nickname & "は攻撃による影響を受けなかった。")
				GoTo EndAttack
			End If
		End If
		
		'ターゲット位置を変更する攻撃はサポートガードの場合は無効
		If su Is Nothing And def_mode <> "援護防御" Then
			'吹き飛ばし
			If IsWeaponClassifiedAs(w, "吹") Or IsWeaponClassifiedAs(w, "Ｋ") Then
				CheckBlowAttack(w, t, dmg, msg, attack_mode, def_mode, critical_type)
			End If
			
			'引き寄せ
			If IsWeaponClassifiedAs(w, "引") Then
				CheckDrawAttack(w, t, msg, def_mode, critical_type)
			End If
			
			'強制転移
			If IsWeaponClassifiedAs(w, "転") Then
				CheckTeleportAwayAttack(w, t, msg, def_mode, critical_type)
			End If
		End If
		
		'クリティカルメッセージはこの時点で追加
		If is_critical Then
			msg = msg & "クリティカル！;"
		End If
		
		'シールド防御の効果適用
		Dim spower As Integer
		If use_shield Then
			If IsWeaponClassifiedAs(w, "破") Then
				If t.IsFeatureAvailable("小型シールド") Then
					dmg = (5 * dmg) \ 6
				Else
					dmg = 3 * dmg \ 4
				End If
			Else
				If t.IsFeatureAvailable("小型シールド") Then
					dmg = (2 * dmg) \ 3
				Else
					dmg = dmg \ 2
				End If
			End If
			
			If t.IsFeatureAvailable("エネルギーシールド") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "無") And Not IsUnderSpecialPowerEffect("防御能力無効化") Then
				
				t.EN = t.EN - 5
				
				If IsWeaponClassifiedAs(w, "破") Then
					spower = 50 * t.FeatureLevel("エネルギーシールド")
				Else
					spower = 100 * t.FeatureLevel("エネルギーシールド")
				End If
				
				If dmg <= spower Then
					If attack_mode <> "反射" Then
						UpdateMessageForm(Me, t)
					Else
						UpdateMessageForm(Me, Nothing)
					End If
					
					fname = t.FeatureName0("エネルギーシールド")
					If Not be_quiet Then
						If t.IsMessageDefined("攻撃無効化(" & fname & ")") Then
							t.PilotMessage("攻撃無効化(" & fname & ")")
						Else
							t.PilotMessage("攻撃無効化")
						End If
					End If
					If t.IsAnimationDefined("攻撃無効化", fname) Then
						t.PlayAnimation("攻撃無効化", fname)
					Else
						t.SpecialEffect("攻撃無効化", fname)
					End If
					
					DisplaySysMessage(msg & fname & "が攻撃を防いだ。")
					GoTo EndAttack
				End If
				
				dmg = dmg - spower
			End If
		End If
		
		'最低ダメージは10
		If dmg > 0 And dmg < 10 Then
			dmg = 10
		End If
		
		'都合により破壊させない場合
		If (IsUnderSpecialPowerEffect("てかげん") And MainPilot.Technique > t.MainPilot.Technique And InStr(attack_mode, "援護攻撃") = 0) Or t.IsConditionSatisfied("不死身") Then
			If t.HP <= 10 Then
				dmg = 0
			ElseIf t.HP - dmg < 10 Then 
				dmg = t.HP - 10
			End If
		End If
		
		'特殊効果
		CauseEffect(w, t, msg, critical_type, def_mode, dmg >= t.HP)
		
		If InStr(critical_type, "即死") > 0 And Not use_support_guard And Not use_protect_msg Then
			If t.IsHero() Then
				msg = msg & WeaponNickname(w) & "が" & t.Nickname & "の命を奪った。;"
			Else
				msg = msg & WeaponNickname(w) & "が" & t.Nickname & "を一撃で破壊した。;"
			End If
			dmg = t.HP
		Else
			If t.HP - dmg < 0 Then
				dmg = t.HP
			End If
		End If
		
		
		'確実に発生する特殊効果
		Dim prev_en As Short
		If IsWeaponClassifiedAs(w, "減") And Not t.SpecialEffectImmune("減") Then
			msg = msg & wnickname & "が[" & t.Nickname & "]の" & Term("ＥＮ", t) & "を低下させた。;"
			t.EN = MaxLng(t.EN - t.MaxEN * (dmg / t.MaxHP), 0)
		ElseIf IsWeaponClassifiedAs(w, "奪") And Not t.SpecialEffectImmune("奪") Then 
			msg = msg & Nickname & "は[" & t.Nickname & "]の" & Term("ＥＮ", t) & "を吸収した。;"
			prev_en = t.EN
			t.EN = MaxLng(t.EN - t.MaxEN * (dmg / t.MaxHP), 0)
			EN = EN + (prev_en - t.EN) \ 2
		End If
		
		'クリティカル時メッセージ
		If is_critical Or Len(critical_type) > 0 Then
			If Not be_quiet Then
				PilotMessage(wname & "(クリティカル)")
			End If
			If IsAnimationDefined(wname & "(クリティカル)") Then
				PlayAnimation(wname & "(クリティカル)")
			ElseIf IsSpecialEffectDefined(wname & "(クリティカル)") Then 
				SpecialEffect(wname & "(クリティカル)")
			Else
				CriticalEffect(critical_type, w, use_support_guard Or use_protect_msg)
			End If
		End If
		
ApplyDamage: 
		'ダメージの適用
		t.HP = t.HP - dmg
		
		'ＨＰ吸収
		If IsWeaponClassifiedAs(w, "吸") And Not t.SpecialEffectImmune("吸") Then
			If HP < MaxHP Then
				msg = msg & Nickname & "は[" & t.Nickname & "]の" & Term("ＨＰ", t) & "を吸収した。;"
				HP = HP + (prev_hp - t.HP) \ 4
			End If
		End If
		
		'マップ攻撃の場合はメッセージが表示されないので
		'その代わりに少しディレイを入れる
		If def_mode = "マップ攻撃" Then
			Sleep(150)
		End If
		
		'ダメージによるＨＰゲージ減少を表示
		If attack_mode <> "反射" Then
			UpdateMessageForm(Me, t)
		Else
			UpdateMessageForm(Me, Nothing)
		End If
		
		'ダメージ量表示前にカットインは一旦消去しておく
		If Not IsOptionDefined("戦闘中画面初期化無効") Or attack_mode = "マップ攻撃" Then
			If IsPictureVisible Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
			End If
		End If
		
		'ダメージ量をマップウィンドウに表示
		If Not IsOptionDefined("ダメージ表示無効") Or attack_mode = "マップ攻撃" Then
			If IsAnimationDefined(wname & "(ダメージ表示)") Then
				PlayAnimation(wname & "(ダメージ表示)")
			ElseIf IsAnimationDefined("ダメージ表示") Then 
				PlayAnimation("ダメージ表示")
			Else
				If Not BattleAnimation Or WeaponPower(w, "") > 0 Or dmg > 0 Then
					If Not BattleAnimation And Not su Is Nothing Then
						DrawSysString(prev_x, prev_y, VB6.Format(dmg))
					Else
						DrawSysString(t.x, t.y, VB6.Format(dmg))
					End If
				End If
			End If
		End If
		
		'自動反撃発動
		If t.HP > 0 Then
			CheckAutoAttack(w, t, attack_mode, def_mode, dmg, be_quiet Or use_protect_msg)
			If Status_Renamed <> "出撃" Then
				GoTo EndAttack
			End If
		End If
		
		'破壊アニメ
		If t.HP = 0 Then
			If t.IsAnimationDefined("破壊") Then
				t.PlayAnimation("破壊")
			Else
				t.SpecialEffect("破壊")
			End If
		End If
		
		'パーツ分離が発動可能かチェック
		separate_parts = False
		If t.HP = 0 Then
			If t.IsFeatureAvailable("パーツ分離") Then
				If t.OtherForm(LIndex(t.FeatureData("パーツ分離"), 2)).IsAbleToEnter(t.x, t.y) Then
					If t.IsFeatureLevelSpecified("パーツ分離") Then
						If Dice(100) <= 10 * t.FeatureLevel("パーツ分離") Then
							separate_parts = True
						End If
					Else
						separate_parts = True
					End If
				End If
			End If
		End If
		
		'破壊メッセージ
		If attack_mode <> "マップ攻撃" And Not use_protect_msg And Not use_shield_msg Then
			If t.HP = 0 Then
				If separate_parts Then
					fname = t.FeatureName("パーツ分離")
					If t.IsMessageDefined("破壊時分離(" & t.Name & ")") Then
						t.PilotMessage("破壊時分離(" & t.Name & ")")
					ElseIf t.IsMessageDefined("破壊時分離(" & fname & ")") Then 
						t.PilotMessage("破壊時分離(" & fname & ")")
					ElseIf t.IsMessageDefined("破壊時") Then 
						t.PilotMessage("破壊時分離")
					ElseIf t.IsMessageDefined("分離(" & t.Name & ")") Then 
						t.PilotMessage("分離(" & t.Name & ")")
					ElseIf t.IsMessageDefined("分離(" & fname & ")") Then 
						t.PilotMessage("分離(" & fname & ")")
					ElseIf t.IsMessageDefined("分離") Then 
						t.PilotMessage("分離")
					Else
						t.PilotMessage("ダメージ大")
					End If
				Else
					t.PilotMessage("破壊")
				End If
			End If
		End If
		
		If Not be_quiet Then
			If t.HP = 0 Then
				'とどめメッセージ
				PilotMessage(wname & "(とどめ)")
			Else
				'ダメージメッセージ
				PilotMessage(wname & "(ダメージ)")
			End If
		End If
		
		'ダメージアニメ
		If t.HP = 0 Then
			'どどめアニメ
			If attack_mode <> "マップ攻撃" And attack_mode <> "反射" Then
				If IsAnimationDefined(wname & "(とどめ)") Then
					PlayAnimation(wname & "(とどめ)")
				Else
					SpecialEffect(wname & "(とどめ)")
				End If
			End If
		Else
			If ((dmg <= 0.05 * t.MaxHP And t.HP >= 0.25 * t.MaxHP) Or dmg <= 10) And Len(critical_type) = 0 Then
				'ダメージが非常に小さい
				If t.IsAnimationDefined("ダメージ小") Then
					t.PlayAnimation("ダメージ小")
				Else
					t.SpecialEffect("ダメージ小")
				End If
			ElseIf t.HP < 0.25 * t.MaxHP Then 
				'ダメージ大
				If t.IsAnimationDefined("ダメージ大") Then
					t.PlayAnimation("ダメージ大")
				Else
					t.SpecialEffect("ダメージ大")
				End If
			ElseIf t.HP > 0.8 * t.MaxHP And Len(critical_type) = 0 Then 
				'ダメージ小
				If t.IsAnimationDefined("ダメージ小") Then
					t.PlayAnimation("ダメージ小")
				Else
					t.SpecialEffect("ダメージ小")
				End If
			Else
				'ダメージ中
				If t.IsAnimationDefined("ダメージ中") Then
					t.PlayAnimation("ダメージ中")
				Else
					t.SpecialEffect("ダメージ中")
				End If
			End If
		End If
		
		'ダメージメッセージ
		If attack_mode <> "マップ攻撃" And Not use_protect_msg And Not use_shield_msg Then
			If t.HP = 0 Then
				'破壊時メッセージは既に表示している
			ElseIf ((dmg <= 0.05 * t.MaxHP And t.HP >= 0.25 * t.MaxHP) Or dmg <= 10) And Len(critical_type) = 0 Then 
				'ダメージが非常に小さい
				t.PilotMessage("ダメージ小")
			ElseIf t.HP < 0.25 * t.MaxHP Then 
				'ダメージ大
				t.PilotMessage("ダメージ大")
			ElseIf is_penetrated And t.IsMessageDefined("バリア貫通") Then 
				t.PilotMessage("バリア貫通")
			ElseIf t.HP >= 0.8 * t.MaxHP And Len(critical_type) = 0 Then 
				'ステータス異常が起こった場合は最低でもダメージ中のメッセージ
				t.PilotMessage("ダメージ小")
			Else
				t.PilotMessage("ダメージ中")
			End If
		End If
		
		'シールド防御
		If use_shield And t.HP > 0 Then
			If t.IsFeatureAvailable("シールド") Then
				fname = t.FeatureName("シールド")
				If t.IsSysMessageDefined("シールド防御", fname) Then
					t.SysMessage("シールド防御", fname)
				Else
					msg = msg & t.Nickname & "は[" & fname & "]で防御した。;"
				End If
			ElseIf t.IsFeatureAvailable("エネルギーシールド") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "無") And Not IsUnderSpecialPowerEffect("防御能力無効化") Then 
				t.EN = t.EN - 5
				fname = t.FeatureName("エネルギーシールド")
				If t.IsSysMessageDefined("シールド防御", fname) Then
					t.SysMessage("シールド防御", fname)
				Else
					msg = msg & t.Nickname & "は[" & fname & "]を展開した。;"
				End If
			ElseIf t.IsFeatureAvailable("小型シールド") Then 
				fname = t.FeatureName("小型シールド")
				If t.IsSysMessageDefined("シールド防御", fname) Then
					t.SysMessage("シールド防御", fname)
				Else
					msg = msg & t.Nickname & "は[" & fname & "]で防御した。;"
				End If
			ElseIf t.IsFeatureAvailable("大型シールド") Then 
				fname = t.FeatureName("大型シールド")
				If t.IsSysMessageDefined("シールド防御", fname) Then
					t.SysMessage("シールド防御", fname)
				Else
					msg = msg & t.Nickname & "は[" & fname & "]で防御した。;"
				End If
			ElseIf t.IsFeatureAvailable("アクティブシールド") Then 
				fname = t.FeatureName("アクティブシールド")
				If t.IsSysMessageDefined("シールド防御", fname) Then
					t.SysMessage("シールド防御", fname)
				ElseIf Not t.IsHero Then 
					msg = msg & t.Nickname & "の[" & fname & "]が機体を守った。;"
				Else
					msg = msg & fname & "が[" & t.Nickname & "]を守った。;"
				End If
			End If
		End If
		
		'ターゲットが生き残った場合の処理
		If t.HP > 0 Then
			If dmg = 0 Then
				If Len(critical_type) > 0 Then
					DisplaySysMessage(msg)
				ElseIf IsWeaponClassifiedAs(w, "盗") Then 
					'盗み失敗
					If t.IsConditionSatisfied("すかんぴん") Then
						DisplaySysMessage(msg & t.Nickname & "は盗める物を持っていなかった。")
					Else
						DisplaySysMessage(msg & t.Nickname & "は素早く持ち物を守った。")
					End If
				ElseIf IsWeaponClassifiedAs(w, "習") Then 
					'ラーニング失敗
					If t.IsFeatureAvailable("ラーニング可能技") Then
						buf = t.FeatureData("ラーニング可能技")
						Select Case LIndex(buf, 2)
							Case "表示", ""
								fname = LIndex(buf, 1)
							Case Else
								fname = LIndex(buf, 2)
						End Select
						If MainPilot.IsSkillAvailable(LIndex(buf, 1)) Then
							DisplaySysMessage(msg & MainPilot.Nickname & "は「" & fname & "」を既に習得していた。")
						Else
							DisplaySysMessage(msg & MainPilot.Nickname & "は「" & fname & "」を習得出来なかった。")
						End If
					Else
						DisplaySysMessage(msg & t.Nickname & "は習得可能な技を持っていなかった。")
					End If
				ElseIf IsWeaponClassifiedAs(w, "写") Or IsWeaponClassifiedAs(w, "化") Then 
					'能力コピーの判定はこれから
				Else
					DisplaySysMessage(msg & t.Nickname & "は攻撃による影響を受けなかった。")
				End If
			ElseIf t.IsConditionSatisfied("データ不明") Then 
				If attack_num > 1 Then
					msg = msg & VB6.Format(hit_count) & "回命中し、"
				End If
				DisplaySysMessage(msg & t.Nickname & "は[" & VB6.Format(dmg) & "]のダメージを受けた。")
			Else
				If attack_num > 1 Then
					msg = msg & VB6.Format(hit_count) & "回命中し、"
				End If
				DisplaySysMessage(msg & t.Nickname & "は[" & VB6.Format(dmg) & "]のダメージを受けた。;" & "残りＨＰは" & VB6.Format(t.HP) & "（損傷率 = " & VB6.Format(100 * (t.MaxHP - t.HP) \ t.MaxHP) & "％）")
			End If
			
			'特殊能力「不安定」による暴走チェック
			If t.IsFeatureAvailable("不安定") Then
				If t.HP <= t.MaxHP \ 4 And Not t.IsConditionSatisfied("暴走") Then
					t.AddCondition("暴走", -1)
					t.Update()
					If t.IsHero Then
						DisplaySysMessage(t.Nickname & "は暴走した。")
					Else
						If Len(t.FeatureName("不安定")) > 0 Then
							DisplaySysMessage(t.Nickname & "は[" & t.FeatureName("不安定") & "]の暴走のために制御不能に陥った。")
						Else
							DisplaySysMessage(t.Nickname & "は制御不能に陥った。")
						End If
					End If
				End If
			End If
			
			'ダメージを受ければ眠りからさめる
			If t.IsConditionSatisfied("睡眠") And Not IsWeaponClassifiedAs(w, "眠") Then
				t.DeleteCondition("睡眠")
				DisplaySysMessage(t.Nickname & "は眠りから覚めた。")
			End If
			
			'ダメージを受けると凍結解除
			If t.IsConditionSatisfied("凍結") And Not IsWeaponClassifiedAs(w, "凍") Then
				t.DeleteCondition("凍結")
				DisplaySysMessage(t.Nickname & "は凍結状態から開放された。")
			End If
		End If
		
		'破壊された場合の処理
		Dim morale_mod As Short
		If t.HP = 0 Then
			If attack_num > 1 Then
				msg = msg & VB6.Format(hit_count) & "回命中し、"
			End If
			If t.IsSysMessageDefined("破壊") Then
				t.SysMessage("破壊")
			ElseIf t.IsHero Then 
				DisplaySysMessage(msg & t.Nickname & "は[" & VB6.Format(dmg) & "]のダメージを受け倒された。")
			Else
				DisplaySysMessage(msg & t.Nickname & "は[" & VB6.Format(dmg) & "]のダメージを受け破壊された。")
			End If
			
			'復活するかどうかのチェックを行う
			
			'スペシャルパワー「復活」
			If t.IsUnderSpecialPowerEffect("復活") Then
				t.RemoveSpecialPowerInEffect("破壊")
				GoTo Resurrect
			End If
			
			'パイロット用特殊能力「英雄」＆「再生」
			If Not is_event And Not IsUnderSpecialPowerEffect("絶対破壊") Then
				If Dice(16) <= t.MainPilot.SkillLevel("英雄") Then
					t.HP = t.MaxHP \ 2
					t.IncreaseMorale(10)
					If t.IsMessageDefined("復活") Then
						t.PilotMessage("復活")
					End If
					If t.IsAnimationDefined("復活") Then
						t.PlayAnimation("復活")
					Else
						t.SpecialEffect("復活")
					End If
					
					buf = t.MainPilot.SkillName0("英雄")
					If buf = "非表示" Then
						buf = "英雄"
					End If
					If t.IsSysMessageDefined("復活", buf) Then
						t.SysMessage("復活", buf)
					Else
						DisplaySysMessage(t.MainPilot.Nickname & "の熱き" & buf & "の心が[" & t.Nickname & "]を復活させた！")
					End If
					GoTo Resurrect
				End If
				
				'浄化の適用
				If t.MainPilot.IsSkillAvailable("再生") Then
					If IsWeaponClassifiedAs(w, "浄") Then
						If MainPilot.IsSkillAvailable("浄化") Then
							If IsMessageDefined("浄化(" & wname & ")") Then
								PilotMessage("浄化(" & wname & ")")
								If IsAnimationDefined("浄化", wname) Then
									PlayAnimation("浄化", wname)
								Else
									SpecialEffect("浄化", wname)
								End If
							ElseIf IsMessageDefined("浄化") Then 
								PilotMessage("浄化")
								If IsAnimationDefined("浄化", wname) Then
									PlayAnimation("浄化", wname)
								Else
									SpecialEffect("浄化", wname)
								End If
							ElseIf IsMessageDefined("浄解(" & wname & ")") Then 
								PilotMessage("浄解(" & wname & ")")
								If IsAnimationDefined("浄解", wname) Then
									PlayAnimation("浄解", wname)
								Else
									SpecialEffect("浄解", wname)
								End If
							ElseIf IsMessageDefined("浄解") Then 
								PilotMessage("浄解")
								If IsAnimationDefined("浄解", wname) Then
									PlayAnimation("浄解", wname)
								Else
									SpecialEffect("浄解", wname)
								End If
							End If
							If IsSysMessageDefined("浄化") Then
								SysMessage("浄化")
							Else
								DisplaySysMessage(MainPilot.Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
							End If
							GoTo Cure
						End If
						For i = 2 To CountPilot
							If Pilot(i).IsSkillAvailable("浄化") Then
								If IsMessageDefined("浄化(" & wname & ")") Then
									PilotMessage("浄化(" & wname & ")")
									If IsAnimationDefined("浄化", wname) Then
										PlayAnimation("浄化", wname)
									Else
										SpecialEffect("浄化", wname)
									End If
								ElseIf IsMessageDefined("浄化") Then 
									PilotMessage("浄化")
									If IsAnimationDefined("浄化", wname) Then
										PlayAnimation("浄化", wname)
									Else
										SpecialEffect("浄化", wname)
									End If
								ElseIf IsMessageDefined("浄解(" & wname & ")") Then 
									PilotMessage("浄解(" & wname & ")")
									If IsAnimationDefined("浄解", wname) Then
										PlayAnimation("浄解", wname)
									Else
										SpecialEffect("浄解", wname)
									End If
								ElseIf IsMessageDefined("浄解") Then 
									PilotMessage("浄解")
									If IsAnimationDefined("浄解", wname) Then
										PlayAnimation("浄解", wname)
									Else
										SpecialEffect("浄解", wname)
									End If
								End If
								If IsSysMessageDefined("浄化") Then
									SysMessage("浄化")
								Else
									DisplaySysMessage(Pilot(i).Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
								End If
								GoTo Cure
							End If
						Next 
						For i = 1 To CountSupport
							If Support(i).IsSkillAvailable("浄化") Then
								If IsMessageDefined("浄化(" & wname & ")") Then
									PilotMessage("浄化(" & wname & ")")
									If IsAnimationDefined("浄化", wname) Then
										PlayAnimation("浄化", wname)
									Else
										SpecialEffect("浄化", wname)
									End If
								ElseIf IsMessageDefined("浄化") Then 
									PilotMessage("浄化")
									If IsAnimationDefined("浄化", wname) Then
										PlayAnimation("浄化", wname)
									Else
										SpecialEffect("浄化", wname)
									End If
								ElseIf IsMessageDefined("浄解(" & wname & ")") Then 
									PilotMessage("浄解(" & wname & ")")
									If IsAnimationDefined("浄解", wname) Then
										PlayAnimation("浄解", wname)
									Else
										SpecialEffect("浄解", wname)
									End If
								ElseIf IsMessageDefined("浄解") Then 
									PilotMessage("浄解")
									If IsAnimationDefined("浄解", wname) Then
										PlayAnimation("浄解", wname)
									Else
										SpecialEffect("浄解", wname)
									End If
								End If
								If IsSysMessageDefined("浄化") Then
									SysMessage("浄化")
								Else
									DisplaySysMessage(Support(i).Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
								End If
								GoTo Cure
							End If
						Next 
						If IsHero Then
							If IsMessageDefined("浄化(" & wname & ")") Then
								PilotMessage("浄化(" & wname & ")")
								If IsAnimationDefined("浄化", wname) Then
									PlayAnimation("浄化", wname)
								Else
									SpecialEffect("浄化", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
							ElseIf IsMessageDefined("浄化") Then 
								PilotMessage("浄化")
								If IsAnimationDefined("浄化", wname) Then
									PlayAnimation("浄化", wname)
								Else
									SpecialEffect("浄化", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
							ElseIf IsMessageDefined("浄解(" & wname & ")") Then 
								PilotMessage("浄解(" & wname & ")")
								If IsAnimationDefined("浄解", wname) Then
									PlayAnimation("浄解", wname)
								Else
									SpecialEffect("浄解", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
							ElseIf IsMessageDefined("浄解") Then 
								PilotMessage("浄解")
								If IsAnimationDefined("浄解", wname) Then
									PlayAnimation("浄解", wname)
								Else
									SpecialEffect("浄解", wname)
								End If
								If IsSysMessageDefined("浄化") Then
									SysMessage("浄化")
								Else
									DisplaySysMessage(MainPilot.Nickname & "は浄化を行って[" & t.Nickname & "]の復活を防いだ。")
								End If
							End If
							GoTo Cure
						End If
					End If
					
					If Dice(16) <= t.MainPilot.SkillLevel("再生") Then
						t.HP = t.MaxHP \ 2
						If t.IsMessageDefined("復活") Then
							t.PilotMessage("復活")
						End If
						If t.IsAnimationDefined("復活") Then
							t.PlayAnimation("復活")
						Else
							t.SpecialEffect("復活")
						End If
						
						buf = t.MainPilot.SkillName0("再生")
						If buf = "非表示" Then
							buf = "再生"
						End If
						If t.IsSysMessageDefined("再生", buf) Then
							t.SysMessage("再生", buf)
						Else
							DisplaySysMessage(t.Nickname & "は" & buf & "の力で一瞬にして復活した！")
						End If
						GoTo Resurrect
					End If
				End If
			End If
			
Cure: 
			
			'ユニット破壊によるパーツ分離
			If separate_parts Then
				uname = LIndex(t.FeatureData("パーツ分離"), 2)
				
				If Not t.IsHero Then
					If BattleAnimation Then
						ExplodeAnimation((t.Size), t.x, t.y)
					Else
						PlayWave("Explode.wav")
					End If
				End If
				
				fname = t.FeatureName("パーツ分離")
				If t.IsAnimationDefined("破壊時分離(" & t.Name & ")") Then
					t.PlayAnimation("破壊時分離(" & t.Name & ")")
				ElseIf t.IsAnimationDefined("破壊時分離(" & fname & ")") Then 
					t.PlayAnimation("破壊時分離(" & fname & ")")
				ElseIf t.IsAnimationDefined("破壊時分離") Then 
					t.PlayAnimation("破壊時分離")
				ElseIf t.IsSpecialEffectDefined("破壊時分離(" & t.Name & ")") Then 
					t.SpecialEffect("破壊時分離(" & t.Name & ")")
				ElseIf t.IsSpecialEffectDefined("破壊時分離(" & fname & ")") Then 
					t.SpecialEffect("破壊時分離(" & fname & ")")
				ElseIf t.IsSpecialEffectDefined("破壊時分離") Then 
					t.SpecialEffect("破壊時分離")
				ElseIf t.IsAnimationDefined("分離(" & t.Name & ")") Then 
					t.PlayAnimation("分離(" & t.Name & ")")
				ElseIf t.IsAnimationDefined("分離(" & fname & ")") Then 
					t.PlayAnimation("分離(" & fname & ")")
				ElseIf t.IsAnimationDefined("分離") Then 
					t.PlayAnimation("分離")
				ElseIf t.IsSpecialEffectDefined("分離(" & t.Name & ")") Then 
					t.SpecialEffect("分離(" & t.Name & ")")
				ElseIf t.IsSpecialEffectDefined("分離(" & fname & ")") Then 
					t.SpecialEffect("分離(" & fname & ")")
				Else
					t.SpecialEffect("分離")
				End If
				
				t.Transform(uname)
				With t.CurrentForm
					.HP = .MaxHP
					'自分から攻撃して破壊された時には行動数を0に
					If .Party = Stage Then
						.UsedAction = .MaxAction
					End If
				End With
				
				If t.IsSysMessageDefined("破壊時分離(" & t.Name & ")") Then
					t.SysMessage("破壊時分離(" & t.Name & ")")
				ElseIf t.IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
					t.SysMessage("破壊時分離(" & fname & ")")
				ElseIf t.IsSysMessageDefined("破壊時分離") Then 
					t.SysMessage("破壊時分離")
				ElseIf t.IsSysMessageDefined("分離(" & t.Name & ")") Then 
					t.SysMessage("分離(" & t.Name & ")")
				ElseIf t.IsSysMessageDefined("分離(" & fname & ")") Then 
					t.SysMessage("分離(" & fname & ")")
				ElseIf t.IsSysMessageDefined("分離") Then 
					t.SysMessage("分離")
				ElseIf t.IsHero() Then 
					If t.Nickname <> t.OtherForm(uname).Nickname Then
						DisplaySysMessage(t.Nickname & "は" & t.OtherForm(uname).Nickname & "に変化した。")
					Else
						DisplaySysMessage(t.Nickname & "は変化し、蘇った。")
					End If
				Else
					DisplaySysMessage(t.Nickname & "は破壊されたパーツを分離させた。")
				End If
				
				t = t.CurrentForm
				SelectedTarget = t
				SelectedTargetForEvent = t
				GoTo Resurrect
			End If
			
			'ユニット破壊による気力の変動
			If attack_mode <> "マップ攻撃" Then
				'敵を破壊したユニットのパイロットはトータルで気力+4
				If InStr(attack_mode, "援護攻撃") > 0 Then
					AttackUnit.CurrentForm.IncreaseMorale(3)
				Else
					IncreaseMorale(3)
				End If
				
				'それ以外のパイロット
				For	Each p In PList
					With p
						'出撃中のパイロットのみが対象
						If .Unit_Renamed Is Nothing Then
							GoTo NextPilot
						End If
						If .Unit_Renamed.Status_Renamed <> "出撃" Then
							GoTo NextPilot
						End If
						
						If .Party = Party Then
							'敵を破壊したユニットの陣営のパイロットは気力+1
							If .Personality <> "機械" Then
								.Morale = .Morale + 1
							End If
						ElseIf .Party = t.Party Then 
							'破壊されたユニットの陣営のパイロットは性格に応じて気力を変化
							Select Case .Personality
								Case "超強気"
									morale_mod = 2
								Case "強気"
									morale_mod = 1
								Case "弱気"
									morale_mod = -1
								Case Else
									morale_mod = 0
							End Select
							
							'味方の場合の気力変化量はオプションで変化する
							If .Party = "味方" And IsOptionDefined("破壊時味方気力変化５倍") Then
								.Morale = .Morale + 5 * morale_mod
							Else
								.Morale = .Morale + morale_mod
							End If
						End If
					End With
NextPilot: 
				Next p
			End If
			
			'脱出メッセージの表示
			If t.IsMessageDefined("脱出") And Not is_event And Not IsEventDefined("破壊 " & t.MainPilot.ID, True) Then
				t.PilotMessage("脱出")
			End If
			
			'戦闘アニメ表示を使わない場合はかばったユニットを元の位置に戻しておく
			If Not BattleAnimation Then
				If Not su Is Nothing Then
					With su
						.x = prev_x
						.y = prev_y
						.Area = prev_area
					End With
				End If
			End If
			
			'ユニットを破壊
			t.Die()
		End If
		
Resurrect: '復活した場合は破壊関連の処理を行わない
		
EndAttack: 
		
		If Status_Renamed = "出撃" And t.Status_Renamed = "出撃" And InStr(attack_mode, "援護攻撃") = 0 And attack_mode <> "マップ攻撃" And attack_mode <> "反射" And Not IsWeaponClassifiedAs(w, "合") And HP > 0 And t.HP > 0 Then
			'再攻撃
			If Not second_attack And IsWeaponAvailable(w, "ステータス") And IsTargetWithinRange(w, t) Then
				'スペシャルパワー効果「再攻撃」
				If IsUnderSpecialPowerEffect("再攻撃") Then
					second_attack = True
					RemoveSpecialPowerInEffect("攻撃")
					GoTo begin
				End If
				
				'再攻撃能力
				If MainPilot.IsSkillAvailable("再攻撃") Then
					If MainPilot.Intuition >= t.MainPilot.Intuition Then
						slevel = 2 * MainPilot.SkillLevel("再攻撃")
					Else
						slevel = MainPilot.SkillLevel("再攻撃")
					End If
					If slevel >= Dice(32) Then
						second_attack = True
						RemoveSpecialPowerInEffect("攻撃")
						GoTo begin
					End If
				End If
				
				'再属性
				If WeaponLevel(w, "再") >= Dice(16) Then
					second_attack = True
					RemoveSpecialPowerInEffect("攻撃")
					GoTo begin
				End If
			End If
			
			'追加攻撃
			If su Is t Then
				CheckAdditionalAttack(w, t, be_quiet, attack_mode, "援護防御", dmg)
			Else
				CheckAdditionalAttack(w, t, be_quiet, attack_mode, "", dmg)
			End If
		End If
		
		'サポートガードを行ったユニットは破壊処理の前に以前の位置に復帰させる
		Dim sx, sy As Short
		If Not su Is Nothing Then
			su = su.CurrentForm
			With su
				
				sx = .x
				sy = .y
				
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(sx, sy) = Nothing
				
				.x = prev_x
				.y = prev_y
				.Area = prev_area
				
				If .Status_Renamed = "出撃" Then
					MapDataForUnit(.x, .y) = su
					MapDataForUnit(tx, ty) = orig_t
					If BattleAnimation Then
						If su.IsAnimationDefined("サポートガード終了") Then
							If Not IsRButtonPressed() Then
								su.PlayAnimation("サポートガード終了")
							End If
						Else
							If Not IsRButtonPressed() Then
								PaintUnitBitmap(orig_t, "リフレッシュ無し")
								If use_support_guard Then
									MoveUnitBitmap(su, sx, sy, .x, .y, 80, 4)
								Else
									MoveUnitBitmap(su, sx, sy, .x, .y, 50)
								End If
							Else
								PaintUnitBitmap(su, "リフレッシュ無し")
								PaintUnitBitmap(orig_t, "リフレッシュ無し")
							End If
						End If
					End If
				Else
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(.x, .y) = Nothing
					MapDataForUnit(tx, ty) = orig_t
					PaintUnitBitmap(orig_t, "リフレッシュ無し")
				End If
			End With
		End If
		
		If is_hit Then
			'攻撃を命中させたことによる気力増加
			If attack_mode <> "マップ攻撃" And attack_mode <> "反射" Then
				With CurrentForm
					If .MainPilot.IsSkillAvailable("命中時気力増加") Then
						.Pilot(1).Morale = .Pilot(1).Morale + .MainPilot.SkillLevel("命中時気力増加")
					End If
				End With
			End If
			
			'攻撃を受けたことによる気力増加
			t.IncreaseMorale(1)
			If t.MainPilot.IsSkillAvailable("損傷時気力増加") Then
				t.Pilot(1).Morale = t.Pilot(1).Morale + t.MainPilot.SkillLevel("損傷時気力増加")
			End If
		Else
			'攻撃を外したことによる気力増加
			If attack_mode <> "マップ攻撃" And attack_mode <> "反射" Then
				With CurrentForm
					If .MainPilot.IsSkillAvailable("失敗時気力増加") Then
						.Pilot(1).Morale = .Pilot(1).Morale + .MainPilot.SkillLevel("失敗時気力増加")
					End If
				End With
			End If
			
			'攻撃を回避したことによる気力増加
			If t.MainPilot.IsSkillAvailable("回避時気力増加") Then
				t.Pilot(1).Morale = t.Pilot(1).Morale + t.MainPilot.SkillLevel("回避時気力増加")
			End If
		End If
		
		'スペシャルパワー効果の解除
		If InStr(msg, "かばった") = 0 Then
			t.RemoveSpecialPowerInEffect("防御")
		End If
		If is_hit Then
			t.RemoveSpecialPowerInEffect("被弾")
		End If
		
		'戦闘アニメで変更されたユニット画像を元に戻す
		If t.IsConditionSatisfied("ユニット画像") Then
			t.DeleteCondition("ユニット画像")
			t.BitmapID = MakeUnitBitmap(t)
			If t.Status_Renamed = "出撃" Then
				PaintUnitBitmap(t, "リフレッシュ無し")
			End If
		End If
		If t.IsConditionSatisfied("非表示付加") Then
			t.DeleteCondition("非表示付加")
			t.BitmapID = MakeUnitBitmap(t)
			If t.Status_Renamed = "出撃" Then
				PaintUnitBitmap(t, "リフレッシュ無し")
			End If
		End If
		
		'戦闘に参加したユニットを識別
		With CurrentForm
			If IsOptionDefined("ユニット情報隠蔽") Then
				If .Party0 = "敵" Or .Party0 = "中立" Then
					.AddCondition("識別済み", -1, 0, "非表示")
				End If
				If t.Party0 = "敵" Or t.Party0 = "中立" Then
					t.AddCondition("識別済み", -1, 0, "非表示")
				End If
			Else
				If .IsConditionSatisfied("ユニット情報隠蔽") Then
					.DeleteCondition("ユニット情報隠蔽")
				End If
				If t.IsConditionSatisfied("ユニット情報隠蔽") Then
					t.DeleteCondition("ユニット情報隠蔽")
				End If
			End If
		End With
		
		'情報を更新
		CurrentForm.Update()
		t.Update()
		
		'マップ攻撃や反射による攻撃の場合はここまで
		Select Case attack_mode
			Case "マップ攻撃", "反射"
				RestoreSelections()
				Exit Sub
		End Select
		
		'ステルスが解ける？
		If IsFeatureAvailable("ステルス") Then
			If IsWeaponClassifiedAs(w, "忍") Then
				'暗殺武器の場合、相手を倒すか行動不能にすればステルス継続
				If t.CurrentForm.Status_Renamed = "出撃" And t.CurrentForm.MaxAction > 0 Then
					AddCondition("ステルス無効", 1)
				End If
			Else
				AddCondition("ステルス無効", 1)
			End If
		End If
		
		'合体技のパートナーの弾数＆ＥＮの消費
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountWeapon
					If .Weapon(j).Name = wname Then
						.UseWeapon(j)
						If .IsWeaponClassifiedAs(j, "自") Then
							If .IsFeatureAvailable("パーツ分離") Then
								uname = LIndex(.FeatureData("パーツ分離"), 2)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
									With .CurrentForm
										.HP = .MaxHP
										.UsedAction = .MaxAction
									End With
								Else
									.Die()
								End If
							Else
								.Die()
							End If
						ElseIf .IsWeaponClassifiedAs(j, "失") And .HP = 0 Then 
							.Die()
						ElseIf .IsWeaponClassifiedAs(j, "変") Then 
							If .IsFeatureAvailable("変形技") Then
								uname = ""
								For k = 1 To .CountFeature
									If .Feature(k) = "変形技" And LIndex(.FeatureData(k), 1) = wname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("ノーマルモード") Then
										uname = LIndex(.FeatureData("ノーマルモード"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("ノーマルモード") Then 
								uname = LIndex(.FeatureData("ノーマルモード"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'同名の武器がなかった場合は自分のデータを使って処理
				If j > .CountWeapon Then
					If Weapon(w).ENConsumption > 0 Then
						.EN = .EN - WeaponENConsumption(w)
					End If
					If IsWeaponClassifiedAs(w, "消") Then
						.AddCondition("消耗", 1)
					End If
					If IsWeaponClassifiedAs(w, "Ｃ") And .IsConditionSatisfied("チャージ完了") Then
						.DeleteCondition("チャージ完了")
					End If
					If IsWeaponClassifiedAs(w, "気") Then
						.IncreaseMorale(-5 * WeaponLevel(w, "気"))
					End If
					If IsWeaponClassifiedAs(w, "霊") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "霊")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsWeaponClassifiedAs(w, "プ") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "プ")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsWeaponClassifiedAs(w, "失") Then
						.HP = MaxLng(.HP - .MaxHP * WeaponLevel(w, "失") \ 10, 0)
					End If
					If IsWeaponClassifiedAs(w, "自") Then
						If .IsFeatureAvailable("パーツ分離") Then
							uname = LIndex(.FeatureData("パーツ分離"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
							Else
								.Die()
							End If
						Else
							.Die()
						End If
					ElseIf IsWeaponClassifiedAs(w, "失") And .HP = 0 Then 
						.Die()
					ElseIf IsWeaponClassifiedAs(w, "変") Then 
						If .IsFeatureAvailable("ノーマルモード") Then
							uname = LIndex(.FeatureData("ノーマルモード"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'以下の特殊効果はは武器データの変化があるため、同時には適応されない
		
		If CurrentForm.Status_Renamed = "破壊" Then
			'反射等により破壊された場合はなにも出来ない
			
			'自爆攻撃
		ElseIf IsWeaponClassifiedAs(w, "自") Then 
			If IsFeatureAvailable("パーツ分離") Then
				uname = LIndex(FeatureData("パーツ分離"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("パーツ分離")
					If IsSysMessageDefined("破壊時分離(" & Name & ")") Then
						SysMessage("破壊時分離(" & Name & ")")
					ElseIf IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
						SysMessage("破壊時分離(" & fname & ")")
					ElseIf IsSysMessageDefined("破壊時分離") Then 
						SysMessage("破壊時分離")
					ElseIf IsSysMessageDefined("分離(" & Name & ")") Then 
						SysMessage("分離(" & Name & ")")
					ElseIf IsSysMessageDefined("分離(" & fname & ")") Then 
						SysMessage("分離(" & fname & ")")
					ElseIf IsSysMessageDefined("分離") Then 
						SysMessage("分離")
					Else
						DisplaySysMessage(Nickname & "は破壊されたパーツを分離させた。")
					End If
				Else
					Die()
				End If
			Else
				Die()
			End If
			
			'ＨＰ消費攻撃による自殺
		ElseIf IsWeaponClassifiedAs(w, "失") And HP = 0 Then 
			Die()
			
			'変形技
		ElseIf IsWeaponClassifiedAs(w, "変") Then 
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = wname Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("ノーマルモード") Then
						uname = LIndex(FeatureData("ノーマルモード"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				uname = LIndex(FeatureData("ノーマルモード"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'アイテムを消費
		ElseIf Weapon(w).IsItem And Bullet(w) = 0 And MaxBullet(w) > 0 Then 
			'アイテムを削除
			num = Data.CountWeapon
			num = num + MainPilot.Data.CountWeapon
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountWeapon
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountWeapon
			Next 
			If IsFeatureAvailable("追加サポート") Then
				num = num + AdditionalSupport.Data.CountWeapon
			End If
			For	Each itm In colItem
				num = num + itm.CountWeapon
				If w <= num Then
					itm.Exist = False
					DeleteItem((itm.ID))
					Exit For
				End If
			Next itm
			
			'能力コピー
		ElseIf is_hit And (IsWeaponClassifiedAs(w, "写") Or IsWeaponClassifiedAs(w, "化")) And (dmg > 0 Or Not IsWeaponClassifiedAs(w, "殺")) Then 
			CheckMetamorphAttack(w, t, def_mode)
		End If
		
		With CurrentForm
			'スペシャルパワーの効果を削除
			If InStr(attack_mode, "援護攻撃") = 0 Then
				If .IsUnderSpecialPowerEffect("攻撃後消耗") Then
					.AddCondition("消耗", 1)
				End If
				.RemoveSpecialPowerInEffect("攻撃")
				If is_hit Then
					.RemoveSpecialPowerInEffect("命中")
				End If
			End If
			
			'戦闘アニメで変更されたユニット画像を元に戻す
			If .IsConditionSatisfied("ユニット画像") Then
				.DeleteCondition("ユニット画像")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("非表示付加") Then
				.DeleteCondition("非表示付加")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			For i = 1 To UBound(partners)
				With partners(i).CurrentForm
					If .IsConditionSatisfied("ユニット画像") Then
						.DeleteCondition("ユニット画像")
						.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
						PaintUnitBitmap(partners(i).CurrentForm)
					End If
					If .IsConditionSatisfied("非表示付加") Then
						.DeleteCondition("非表示付加")
						.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
						PaintUnitBitmap(partners(i).CurrentForm)
					End If
				End With
			Next 
		End With
		
		'カットインは消去しておく
		If Not IsOptionDefined("戦闘中画面初期化無効") Or attack_mode = "マップ攻撃" Then
			If IsPictureVisible Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
			End If
		End If
		
		' ADD START MARGE
		'戦闘アニメ終了処理
		If IsAnimationDefined(wname & "(終了)") Then
			PlayAnimation(wname & "(終了)")
		ElseIf IsAnimationDefined("終了") Then 
			PlayAnimation("終了")
		End If
		' ADD END MARGE
		
		'ユニット選択を解除
		RestoreSelections()
	End Sub
	
	'回避用特殊能力の判定
	Public Function CheckDodgeFeature(ByVal w As Short, ByRef t As Unit, ByRef tx As Short, ByRef ty As Short, ByRef attack_mode As String, ByRef def_mode As String, ByRef dmg As Integer, ByRef be_quiet As Boolean) As Boolean
		Dim wname As String
		Dim ecost, nmorale As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim fid, frange As Short
		Dim u As Unit
		Dim j, i, k As Short
		Dim prob As Integer
		Dim buf As String
		Dim team, uteam As String
		
		'スペシャルパワーで回避能力が無効化されている？
		If (IsUnderSpecialPowerEffect("絶対命中") Or IsUnderSpecialPowerEffect("回避能力無効化")) And Not t.IsUnderSpecialPowerEffect("特殊防御発動") Then
			Exit Function
		End If
		
		'能動防御は行動できなければ発動しない
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("無防備") Then
			Exit Function
		End If
		
		wname = WeaponNickname(w)
		team = MainPilot.SkillData("チーム")
		
		'阻止無効化
		If IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			GoTo SkipBlock
		End If
		
		'広域阻止
		'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u = Nothing
		flevel = 0
		fid = 0
		'阻止してくれるユニットを探す
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint
				End If
				
				With MapDataForUnit(i, j)
					If .IsEnemy(t) Then
						GoTo NextPoint
					End If
					
					If .Area = "地中" Then
						GoTo NextPoint
					End If
					
					If Not .IsFeatureAvailable("広域阻止") Then
						GoTo NextPoint
					End If
					
					'同じチームに属している？
					uteam = .MainPilot.SkillData("チーム")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "広域阻止" Then
							fdata = .FeatureData(k)
							
							'有効範囲
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'使用条件
							If IsNumeric(LIndex(fdata, 5)) Then
								ecost = CShort(LIndex(fdata, 5))
							Else
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 6)) Then
								nmorale = CShort(LIndex(fdata, 6))
							Else
								nmorale = 0
							End If
							
							'発動条件を満たしている？
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("阻止")) Then
								If .FeatureLevel(k) > flevel Then
									u = MapDataForUnit(i, j)
									flevel = .FeatureLevel(k)
									fid = k
								End If
							End If
						End If
					Next 
				End With
NextPoint: 
			Next 
		Next 
		If Not u Is Nothing Then
			'阻止してくれるユニットがいる場合
			If fid = 0 Then
				fname = u.FeatureName("広域阻止")
				fdata = u.FeatureData("広域阻止")
				flevel = u.FeatureLevel("広域阻止")
			Else
				fname = u.FeatureName(fid)
				fdata = u.FeatureData(fid)
				flevel = u.FeatureLevel(fid)
			End If
			If flevel = 1 Then
				flevel = 10000
			End If
			
			'阻止確率の設定
			buf = LIndex(fdata, 4)
			If IsNumeric(buf) Then
				prob = CShort(buf)
			ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
				i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
				prob = 100 * (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) / 16
			Else
				prob = u.SkillLevel(buf) * 100 / 16
			End If
			
			'見切り
			If u.IsUnderSpecialPowerEffect("特殊防御発動") Then
				prob = 100
			End If
			
			'必中がかかっていれば阻止は無効
			If IsUnderSpecialPowerEffect("絶対命中") And Not u.IsUnderSpecialPowerEffect("特殊防御発動") Then
				prob = 0
			End If
			
			'ダメージが許容範囲外であれば阻止できない
			If dmg > 500 * flevel Then
				prob = 0
			End If
			
			'ＥＮ消費量
			If IsNumeric(LIndex(fdata, 5)) Then
				ecost = CShort(LIndex(fdata, 5))
			Else
				ecost = 0
			End If
			
			'攻撃を阻止
			If prob >= Dice(100) Then
				u.EN = u.EN - ecost
				If Not be_quiet Then
					If u.IsMessageDefined("阻止(" & fname & ")") Then
						u.PilotMessage("阻止(" & fname & ")")
					Else
						u.PilotMessage("阻止")
					End If
				End If
				If u.IsAnimationDefined("阻止", fname) Then
					u.PlayAnimation("阻止", fname)
				Else
					u.SpecialEffect("阻止", fname)
				End If
				
				If u.IsSysMessageDefined("阻止", fname) Then
					u.SysMessage("阻止", fname)
				Else
					DisplaySysMessage(u.Nickname & "は[" & fname & "]で[" & wname & "]を防いだ。")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
		
SkipBlock: 
		
		'分身(ユニット用特殊能力)
		If t.IsFeatureAvailable("分身") And t.MainPilot.Morale >= 130 And Not t.IsFeatureLevelSpecified("分身") And (Dice(2) = 1 Or t.IsUnderSpecialPowerEffect("特殊防御発動")) Then
			fname = t.FeatureName("分身")
			
			'特殊効果
			If t.IsAnimationDefined("分身", fname) Then
				t.PlayAnimation("分身", fname)
			ElseIf t.IsSpecialEffectDefined("分身", fname) Then 
				t.SpecialEffect("分身", fname)
			ElseIf BattleAnimation Then 
				If fname = "分身" Then
					ShowAnimation("分身発動")
				Else
					ShowAnimation("分身発動 - " & fname)
				End If
			End If
			
			'回避音
			DodgeEffect(Me, w)
			
			'メッセージ
			If Not be_quiet Then
				If t.IsMessageDefined("分身(" & fname & ")") Then
					t.PilotMessage("分身(" & fname & ")")
				Else
					t.PilotMessage("分身")
				End If
			End If
			
			If t.IsSysMessageDefined("分身", fname) Then
				t.SysMessage("分身", fname)
			Else
				If fname <> "分身" Then
					DisplaySysMessage(t.Nickname & "は[" & fname & "]を使って攻撃をかわした")
				Else
					DisplaySysMessage(t.Nickname & "は分身して攻撃をかわした。")
				End If
			End If
			
			CheckDodgeFeature = True
			Exit Function
		End If
		
		'超回避
		If t.IsFeatureAvailable("超回避") Then
			fname = t.FeatureName("超回避")
			fdata = t.FeatureData("超回避")
			flevel = t.FeatureLevel("超回避")
			
			'発動率
			prob = flevel
			If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
				prob = 10
			End If
			
			'必要条件
			If IsNumeric(LIndex(fdata, 2)) Then
				ecost = CShort(LIndex(fdata, 2))
			Else
				ecost = 0
			End If
			If IsNumeric(LIndex(fdata, 3)) Then
				nmorale = CShort(LIndex(fdata, 3))
			Else
				nmorale = 0
			End If
			If LIndex(fdata, 4) = "手動" Then
				If def_mode <> "回避" Then
					prob = 0
				End If
			End If
			
			'発動条件を満たしている？
			If t.EN >= ecost And t.MainPilot.Morale >= nmorale And prob >= Dice(10) Then
				'ＥＮ消費
				If ecost <> 0 Then
					t.EN = t.EN - ecost
					If attack_mode <> "反射" Then
						UpdateMessageForm(Me, t)
					Else
						UpdateMessageForm(Me, Nothing)
					End If
				End If
				
				'特殊効果
				If t.IsAnimationDefined("分身", fname) Then
					t.PlayAnimation("分身", fname)
				ElseIf t.IsSpecialEffectDefined("分身", fname) Then 
					t.SpecialEffect("分身", fname)
				ElseIf BattleAnimation Then 
					ShowAnimation("回避発動")
				Else
					'回避音
					DodgeEffect(Me, w)
				End If
				
				'メッセージ
				If Not be_quiet Then
					If t.IsMessageDefined("分身(" & fname & ")") Then
						t.PilotMessage("分身(" & fname & ")")
					Else
						t.PilotMessage("分身")
					End If
				End If
				
				If t.IsSysMessageDefined("分身", fname) Then
					t.SysMessage("分身", fname)
				Else
					DisplaySysMessage(t.Nickname & "は[" & fname & "]を使って攻撃をかわした。")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
		
		'緊急テレポート
		Dim new_x, new_y As Short
		If t.IsFeatureAvailable("緊急テレポート") Then
			fname = t.FeatureName("緊急テレポート")
			fdata = t.FeatureData("緊急テレポート")
			flevel = t.FeatureLevel("緊急テレポート")
			
			'発動率
			prob = flevel
			If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
				prob = 10
			End If
			
			'必要条件
			If IsNumeric(LIndex(fdata, 3)) Then
				ecost = CShort(LIndex(fdata, 3))
			Else
				ecost = 0
			End If
			If IsNumeric(LIndex(fdata, 4)) Then
				nmorale = CShort(LIndex(fdata, 4))
			Else
				nmorale = 0
			End If
			If LIndex(fdata, 5) = "手動" Then
				If def_mode <> "回避" Then
					prob = 0
				End If
			End If
			
			'発動条件を満たしている？
			If t.EN >= ecost And t.MainPilot.Morale >= nmorale And prob >= Dice(10) Then
				
				'逃げ場所がある？
				AreaInTeleport(t, StrToLng(LIndex(fdata, 2)))
				SafetyPoint(t, new_x, new_y)
				
				If (t.x <> new_x Or t.y <> new_y) And new_x <> 0 And new_y <> 0 Then
					'ＥＮ消費
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "反射" Then
							UpdateMessageForm(Me, t)
						Else
							UpdateMessageForm(Me, Nothing)
						End If
					End If
					
					'特殊効果
					If t.IsAnimationDefined("緊急テレポート", fname) Then
						t.PlayAnimation("緊急テレポート", fname)
					ElseIf t.IsSpecialEffectDefined("緊急テレポート", fname) Then 
						t.SpecialEffect("緊急テレポート", fname)
					ElseIf BattleAnimation Then 
						If fname = "緊急テレポート" Then
							ShowAnimation("緊急テレポート発動")
						Else
							ShowAnimation("緊急テレポート発動 - " & fname)
						End If
					End If
					
					'回避音
					DodgeEffect(Me, w)
					
					'緊急テレポート発動！
					t.Jump(new_x, new_y)
					
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("緊急テレポート(" & fname & ")") Then
							t.PilotMessage("緊急テレポート(" & fname & ")")
						Else
							t.PilotMessage("緊急テレポート")
						End If
					End If
					
					If t.IsSysMessageDefined("緊急テレポート", fname) Then
						t.SysMessage("緊急テレポート", fname)
					Else
						DisplaySysMessage(t.Nickname & "は[" & fname & "]を使って攻撃をかわした。")
					End If
					
					CheckDodgeFeature = True
					Exit Function
				End If
			End If
		End If
		
		'分身(パイロット用特殊能力)
		If t.MainPilot.IsSkillAvailable("分身") Then
			prob = 2 * t.MainPilot.SkillLevel("分身") - MainPilot.SkillLevel("分身")
			If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
				prob = 32
			End If
			
			If prob >= Dice(32) Then
				fname = t.MainPilot.SkillName0("分身")
				
				'特殊効果
				If t.IsAnimationDefined("分身", fname) Then
					t.PlayAnimation("分身", fname)
				ElseIf t.IsSpecialEffectDefined("分身", fname) Then 
					t.SpecialEffect("分身", fname)
				ElseIf BattleAnimation Then 
					ShowAnimation("分身発動")
				Else
					'回避音
					DodgeEffect(Me, w)
				End If
				
				'メッセージ
				If Not be_quiet Then
					If t.IsMessageDefined("分身(" & fname & ")") Then
						t.PilotMessage("分身(" & fname & ")")
					Else
						t.PilotMessage("分身")
					End If
				End If
				
				If t.IsSysMessageDefined("分身", fname) Then
					t.SysMessage("分身", fname)
				Else
					DisplaySysMessage(t.Nickname & "は分身して攻撃をかわした。")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
	End Function
	
	'切り払い＆反射のチェック
	'(命中時に発動し、発動すれば必ずダメージが0になる能力)
	Public Function CheckParryFeature(ByVal w As Short, ByRef t As Unit, ByRef tx As Short, ByRef ty As Short, ByRef attack_mode As String, ByRef def_mode As String, ByRef dmg As Integer, ByRef msg As String, ByRef be_quiet As Boolean) As Boolean
		Dim wname, wname2 As String
		Dim w2 As Short
		Dim ecost, nmorale As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim slevel, lv_mod As Double
		Dim opt As String
		Dim j, i, idx As Short
		Dim prob As Integer
		Dim buf As String
		
		'スペシャルパワーで回避能力が無効化されている？
		If (IsUnderSpecialPowerEffect("絶対命中") Or IsUnderSpecialPowerEffect("回避能力無効化")) And Not t.IsUnderSpecialPowerEffect("特殊防御発動") Then
			Exit Function
		End If
		
		'能動防御は行動できなければ発動しない
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("無防備") Then
			Exit Function
		End If
		
		wname = WeaponNickname(w)
		
		'ターゲットの迎撃レベルをチェック
		slevel = t.SkillLevel("迎撃")
		
		'切り払いに使用する武器を持っている？
		'(持っていれば切り払いの方を優先)
		wname2 = ""
		If t.IsFeatureAvailable("格闘武器") Then
			wname2 = t.FeatureData("格闘武器")
		Else
			For i = 1 To t.CountWeapon
				If t.IsWeaponClassifiedAs(i, "武") And t.IsWeaponAvailable(i, "移動前") Then
					wname2 = t.WeaponNickname(i)
					Exit For
				End If
			Next 
		End If
		'発動条件を満たしている？
		If IsWeaponClassifiedAs(w, "実") And (slevel > t.MainPilot.SkillLevel("切り払い") Or (slevel > 0 And Len(wname2) = 0)) Then
			'迎撃武器を検索
			i = 0
			If t.IsFeatureAvailable("迎撃武器") Then
				For i = 1 To t.CountWeapon
					If t.Weapon(i).Name = t.FeatureData("迎撃武器") Then
						If Not t.IsWeaponAvailable(i, "移動前") Then
							i = 0
						End If
						Exit For
					End If
				Next 
			End If
			If i = 0 Then
				'迎撃武器がない場合は迎撃用の武器としての条件を満たす武器を検索
				For i = 1 To t.CountWeapon
					If t.IsWeaponAvailable(i, "移動後") And t.IsWeaponClassifiedAs(i, "移動後攻撃可") And t.IsWeaponClassifiedAs(i, "射撃系") And (t.Weapon(i).Bullet >= 10 Or (t.Weapon(i).Bullet = 0 And t.Weapon(i).ENConsumption <= 5)) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale Then
						Exit For
					End If
				Next 
			End If
			
			'迎撃用武器が弾切れ、ＥＮ不足の場合は迎撃不可
			If 0 < i And i <= t.CountWeapon Then
				If Not t.IsWeaponAvailable(i, "ステータス") Then
					i = 0
				End If
			End If
			
			'迎撃を実行
			If 0 < i And i <= t.CountWeapon And (slevel >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動")) Then
				'メッセージ
				If Not be_quiet Then
					If t.IsMessageDefined("迎撃(" & t.Weapon(i).Name & ")") Then
						t.PilotMessage("迎撃(" & t.Weapon(i).Name & ")")
					Else
						t.PilotMessage("迎撃")
					End If
				Else
					IsWavePlayed = False
				End If
				
				'効果音
				If Not IsWavePlayed Then
					If IsAnimationDefined(wname & "(迎撃)") Then
						PlayAnimation(wname & "(迎撃)")
					ElseIf IsSpecialEffectDefined(wname & "(迎撃)") Then 
						SpecialEffect(wname & "(迎撃)")
					ElseIf t.IsAnimationDefined("迎撃", fname) Then 
						t.PlayAnimation("迎撃", fname)
					ElseIf t.IsSpecialEffectDefined("迎撃", fname) Then 
						t.SpecialEffect("迎撃", fname)
					ElseIf t.IsSpecialEffectDefined((t.Weapon(i).Name)) Then 
						t.SpecialEffect((t.Weapon(i).Name))
					Else
						AttackEffect(t, i)
					End If
				End If
				
				DisplaySysMessage(t.Nickname & "は[" & t.WeaponNickname(i) & "]で[" & wname & "]を阻止した。")
				
				'迎撃された永続武器は使用回数を減らす
				If IsWeaponClassifiedAs(w, "永") And Weapon(w).Bullet > 0 Then
					SetBullet(w, Bullet(w) - 1)
					SyncBullet()
					IsMapAttackCanceled = True
				End If
				
				'迎撃武器の弾数を消費
				t.UseWeapon(i)
				
				CheckParryFeature = True
				Exit Function
			End If
		End If
		
		'無属性武器には阻止が効かない
		If IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			GoTo SkipBlock
		End If
		
		'阻止
		For i = 1 To t.CountFeature
			If t.Feature(i) = "阻止" Then
				fname = t.FeatureName0(i)
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'阻止確率の設定
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'見切り
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 100
				End If
				
				'必中がかかっていれば阻止は無効
				If IsUnderSpecialPowerEffect("絶対命中") And Not t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 0
				End If
				
				'対象属性の判定
				If Not t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					prob = 0
				End If
				
				'使用条件
				If IsNumeric(LIndex(fdata, 4)) Then
					ecost = CShort(LIndex(fdata, 4))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 5)) Then
					nmorale = CShort(LIndex(fdata, 5))
				Else
					nmorale = 0
				End If
				If t.EN < ecost Or t.MainPilot.Morale < nmorale Then
					prob = 0
				End If
				
				'オプション
				slevel = 0
				For j = 6 To LLength(fdata)
					If prob = 0 Then
						Exit For
					End If
					
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If LIndex(fdata, 1) = LIndex(FeatureData("阻止"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "は[" & fname & "]を中和した。;"
								prob = 0
							End If
						Case "中和"
							If LIndex(fdata, 1) = LIndex(FeatureData("阻止"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("阻止")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									prob = 0
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								prob = 0
							End If
						Case "手動"
							If def_mode <> "防御" Then
								prob = 0
							End If
						Case "能力必要"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'ダメージが許容範囲外であれば阻止できない
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'攻撃を阻止
				If prob >= Dice(100) Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "反射" Then
							UpdateMessageForm(Me, t)
						Else
							UpdateMessageForm(Me, Nothing)
						End If
					End If
					If Not be_quiet Then
						If t.IsMessageDefined("阻止(" & fname & ")") Then
							t.PilotMessage("阻止(" & fname & ")")
						Else
							t.PilotMessage("阻止")
						End If
					End If
					If IsAnimationDefined(wname & "(阻止)") Then
						PlayAnimation(wname & "(阻止)")
					ElseIf IsSpecialEffectDefined(wname & "(阻止)") Then 
						SpecialEffect(wname & "(阻止)")
					ElseIf t.IsAnimationDefined("阻止", fname) Then 
						t.PlayAnimation("阻止", fname)
					Else
						t.SpecialEffect("阻止", fname)
					End If
					
					If t.IsSysMessageDefined("阻止", fname) Then
						t.SysMessage("阻止", fname)
					Else
						DisplaySysMessage(t.Nickname & "は[" & fname & "]で[" & wname & "]を防いだ。")
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
		
SkipBlock: 
		
		'マップ攻撃や無属性武器には当て身技は効かない
		If IsWeaponClassifiedAs(w, "Ｍ") Or IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			GoTo SkipParryAttack
		End If
		
		'当て身技
		For i = 1 To t.CountFeature
			'封印されている？
			If t.Feature(i) = "当て身技" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					fname = "当て身技"
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'当て身確率の設定
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'見切り
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 100
				End If
				
				'必中がかかっていれば当て身技は無効
				If IsUnderSpecialPowerEffect("絶対命中") And Not t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					Exit For
				End If
				
				'自分の反射や当て身技に対して当て身技は出来ない
				If attack_mode = "反射" Or attack_mode = "当て身技" Then
					Exit For
				End If
				
				'対象属性の判定
				If Not t.IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) Then
					prob = 0
				End If
				
				'使用条件
				If IsNumeric(LIndex(fdata, 5)) Then
					ecost = CShort(LIndex(fdata, 5))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 6)) Then
					nmorale = CShort(LIndex(fdata, 6))
				Else
					nmorale = 0
				End If
				If t.EN < ecost Or t.MainPilot.Morale < nmorale Then
					prob = 0
				End If
				
				'オプション
				slevel = 0
				For j = 7 To LLength(fdata)
					If prob = 0 Then
						Exit For
					End If
					
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("当て身技")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								prob = 0
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("当て身技")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("当て身技")
								If flevel <= 0 Then
									prob = 0
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								prob = 0
							End If
						Case "手動"
							If def_mode <> "防御" Then
								prob = 0
							End If
						Case "能力必要"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'ダメージが許容範囲外であれば当て身技を使えない
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'使用する当て身技を検索
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To t.CountWeapon
					If t.Weapon(j).Name = wname2 Then
						If t.IsWeaponAvailable(j, "必要技能無視") Then
							w2 = j
						End If
						Exit For
					End If
				Next 
				
				'当て身技発動
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("当て身技(" & fname & ")") Then
							t.PilotMessage("当て身技(" & fname & ")")
						Else
							t.PilotMessage("当て身技")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(当て身技)") Then
							PlayAnimation(wname & "(当て身技)")
						ElseIf IsSpecialEffectDefined(wname & "(当て身技)") Then 
							SpecialEffect(wname & "(当て身技)")
						ElseIf t.IsAnimationDefined("当て身技", fname) Then 
							t.PlayAnimation("当て身技", fname)
						ElseIf t.IsSpecialEffectDefined("当て身技", fname) Then 
							t.SpecialEffect("当て身技", fname)
						ElseIf BattleAnimation Then 
							ShowAnimation("打突命中")
						ElseIf IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Or IsWeaponClassifiedAs(w, "実") Then 
							PlayWave("Sword.wav")
						Else
							PlayWave("BeamCoat.wav")
						End If
					End If
					
					If t.IsSysMessageDefined("当て身技", fname) Then
						t.SysMessage("当て身技", fname)
					Else
						DisplaySysMessage(t.Nickname & "は[" & fname & "]で[" & wname & "]を受け止めた。")
					End If
					
					'当て身技で攻撃をかける
					t.Attack(w2, Me, "当て身技", "")
					t = t.CurrentForm
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
		
SkipParryAttack: 
		
		'切り払いに使用する武器を調べる
		wname2 = ""
		If t.IsFeatureAvailable("格闘武器") Then
			wname2 = t.FeatureData("格闘武器")
		Else
			For i = 1 To t.CountWeapon
				If t.IsWeaponClassifiedAs(i, "武") And Not t.IsWeaponClassifiedAs(i, "合") And t.IsWeaponMastered(i) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale And Not t.IsDisabled((t.Weapon(i).Name)) Then
					wname2 = t.WeaponNickname(i)
					Exit For
				End If
			Next 
		End If
		
		'切り払い出来る？
		If t.MainPilot.SkillLevel("切り払い") > 0 And Len(wname2) > 0 Then
			If IsWeaponClassifiedAs(w, "実") Then
				prob = 0
				
				'思念誘導はＮＴレベルに応じて切り払いにくくなる
				If IsWeaponClassifiedAs(w, "サ") Then
					prob = t.MainPilot.SkillLevel("超感覚") + t.MainPilot.SkillLevel("知覚強化")
					prob = prob - MainPilot.SkillLevel("超感覚") - MainPilot.SkillLevel("知覚強化")
					If prob > 0 Then
						prob = 0
					End If
				Else
					prob = 0
				End If
				
				prob = prob + 2 * t.MainPilot.SkillLevel("切り払い")
				
				'見切りがあれば必ず発動
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("切り払い(" & wname2 & ")") Then
							t.PilotMessage("切り払い(" & wname2 & ")")
						Else
							t.PilotMessage("切り払い")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(切り払い)") Then
							PlayAnimation(wname & "(切り払い)")
						ElseIf IsSpecialEffectDefined(wname & "(切り払い)") Then 
							SpecialEffect(wname & "(切り払い)")
						ElseIf t.IsAnimationDefined("切り払い", wname2) Then 
							t.PlayAnimation("切り払い", wname2)
						ElseIf t.IsSpecialEffectDefined("切り払い", wname2) Then 
							t.SpecialEffect("切り払い", wname2)
						Else
							ParryEffect(Me, w, t)
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "は[" & wname2 & "]で[" & wname & "]を叩き落とした。")
					
					'切り払われた永続武器は使用回数を減らす
					If IsWeaponClassifiedAs(w, "永") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			ElseIf IsWeaponClassifiedAs(w, "接") Then 
				'武属性や突属性を持っていても切り払いの対象外になります
			ElseIf IsWeaponClassifiedAs(w, "突") Then 
				'相手も切り払い出来れば切り払い確率は下がる
				prob = 2 * t.MainPilot.SkillLevel("切り払い") - MainPilot.SkillLevel("切り払い")
				
				'見切りがあれば必ず発動
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("切り払い(" & wname2 & ")") Then
							t.PilotMessage("切り払い(" & wname2 & ")")
						Else
							t.PilotMessage("切り払い")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(切り払い)") Then
							PlayAnimation(wname & "(切り払い)")
						ElseIf IsSpecialEffectDefined(wname & "(切り払い)") Then 
							SpecialEffect(wname & "(切り払い)")
						ElseIf t.IsAnimationDefined("切り払い", wname2) Then 
							t.PlayAnimation("切り払い", wname2)
						ElseIf t.IsSpecialEffectDefined("切り払い", wname2) Then 
							t.SpecialEffect("切り払い", wname2)
						Else
							DodgeEffect(Me, w)
							Sleep(190)
							PlayWave("Sword.wav")
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "は[" & wname2 & "]で[" & wname & "]を受け流した。")
					
					'切り払われた永続武器は使用回数を減らす
					If IsWeaponClassifiedAs(w, "永") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			ElseIf IsWeaponClassifiedAs(w, "武") Then 
				'相手も切り払い出来れば切り払い確率は下がる
				prob = 2 * t.MainPilot.SkillLevel("切り払い") - MainPilot.SkillLevel("切り払い")
				
				'見切りがあれば必ず発動
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("切り払い(" & wname2 & ")") Then
							t.PilotMessage("切り払い(" & wname2 & ")")
						Else
							t.PilotMessage("切り払い")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(切り払い)") Then
							PlayAnimation(wname & "(切り払い)")
						ElseIf IsSpecialEffectDefined(wname & "(切り払い)") Then 
							SpecialEffect(wname & "(切り払い)")
						ElseIf t.IsAnimationDefined("切り払い", wname2) Then 
							t.PlayAnimation("切り払い", wname2)
						ElseIf t.IsSpecialEffectDefined("切り払い", wname2) Then 
							t.SpecialEffect("切り払い", wname2)
						Else
							DodgeEffect(Me, w)
							Sleep(190)
							PlayWave("Sword.wav")
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "は[" & wname2 & "]で[" & wname & "]を受けとめた。")
					
					'切り払われた永続武器は使用回数を減らす
					If IsWeaponClassifiedAs(w, "永") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		End If
		
		'反射無効化
		If IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			Exit Function
		End If
		
		'攻撃反射の処理
		For i = 1 To t.CountFeature
			If t.Feature(i) = "反射" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("バリアシールド") Then
						fname = t.FeatureName0("バリアシールド")
					Else
						fname = "反射"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'反射確率の設定
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'反射された攻撃を反射する場合は確率を下げる
				If attack_mode = "反射" Then
					prob = prob \ 2
				End If
				
				'見切り
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 100
				End If
				
				'当て身技は反射出来ない
				If attack_mode = "当て身技" Then
					Exit For
				End If
				
				'必中がかかっていれば反射は無効
				If IsUnderSpecialPowerEffect("絶対命中") And Not t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					Exit For
				End If
				
				'対象属性の判定
				If Not t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					prob = 0
				End If
				
				'使用条件
				If IsNumeric(LIndex(fdata, 4)) Then
					ecost = CShort(LIndex(fdata, 4))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 5)) Then
					nmorale = CShort(LIndex(fdata, 5))
				Else
					nmorale = 0
				End If
				If t.EN < ecost Or t.MainPilot.Morale < nmorale Then
					prob = 0
				End If
				
				'オプション
				slevel = 0
				For j = 6 To LLength(fdata)
					If prob = 0 Then
						Exit For
					End If
					
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If LIndex(fdata, 1) = LIndex(FeatureData("阻止"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "は[" & fname & "]を中和した。;"
								prob = 0
							End If
						Case "中和"
							If LIndex(fdata, 1) = LIndex(FeatureData("阻止"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("阻止")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									prob = 0
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								prob = 0
							End If
						Case "手動"
							If def_mode <> "防御" Then
								prob = 0
							End If
						Case "能力必要"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'ダメージが許容範囲外であれば反射できない
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'攻撃を反射
				If prob >= Dice(100) Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("反射(" & fname & ")") Then
							t.PilotMessage("反射(" & fname & ")")
						Else
							t.PilotMessage("反射")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(反射)") Then
							PlayAnimation(wname & "(反射)")
						ElseIf IsSpecialEffectDefined(wname & "(反射)") Then 
							SpecialEffect(wname & "(反射)")
						ElseIf t.IsAnimationDefined("反射", fname) Then 
							t.PlayAnimation("反射", fname)
						ElseIf t.IsSpecialEffectDefined("反射", fname) Then 
							t.SpecialEffect("反射", fname)
						ElseIf BattleAnimation Then 
							If fname = "反射" Then
								ShowAnimation("反射発動")
							Else
								ShowAnimation("反射発動 - " & fname)
							End If
						ElseIf IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Or IsWeaponClassifiedAs(w, "実") Then 
							PlayWave("Sword.wav")
						Else
							PlayWave("BeamCoat.wav")
						End If
					End If
					
					If t.IsSysMessageDefined("反射", fname) Then
						t.SysMessage("反射", fname)
					ElseIf fname <> "反射" Then 
						DisplaySysMessage(t.Nickname & "は[" & fname & "]で[" & wname & "]を弾き返した。")
					Else
						DisplaySysMessage(t.Nickname & "は[" & wname & "]を弾き返した。")
					End If
					
					'攻撃を反射
					If Not IsWeaponClassifiedAs(w, "Ｍ") And attack_mode <> "反射" Then
						Attack(w, Me, "反射", "")
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
	End Function
	
	'ダミー能力のチェック
	Private Function CheckDummyFeature(ByVal w As Short, ByRef t As Unit, ByVal be_quiet As Boolean) As Boolean
		Dim wname As String
		Dim fname As String
		
		wname = WeaponNickname(w)
		
		If t.IsConditionSatisfied("ダミー付加") Then
			'命中時の特殊効果
			IsWavePlayed = False
			If Not be_quiet Then
				PilotMessage(wname & "(命中)")
			End If
			If IsAnimationDefined(wname & "(命中)") Or IsAnimationDefined(wname) Then
				PlayAnimation(wname & "(命中)")
			ElseIf IsSpecialEffectDefined(wname & "(命中)") Then 
				SpecialEffect(wname & "(命中)")
			ElseIf Not IsWavePlayed Then 
				HitEffect(Me, w, t)
			End If
			
			fname = t.FeatureName("ダミー")
			If Len(fname) > 0 Then
				If InStr(fname, "Lv") > 0 Then
					fname = Left(fname, InStr(fname, "Lv") - 1)
				End If
			Else
				fname = "ダミー"
			End If
			
			If Not be_quiet Then
				If t.IsMessageDefined("ダミー(" & fname & ")") Then
					t.PilotMessage("ダミー(" & fname & ")")
				Else
					t.PilotMessage("ダミー")
				End If
			End If
			If t.IsAnimationDefined("ダミー", fname) Then
				t.PlayAnimation("ダミー", fname)
			Else
				t.SpecialEffect("ダミー", fname)
			End If
			
			If t.IsSysMessageDefined("ダミー", fname) Then
				t.SysMessage("ダミー", fname)
			Else
				DisplaySysMessage(t.Nickname & "は[" & fname & "]を身代わりにして攻撃をかわした。")
			End If
			
			t.SetConditionLevel("ダミー付加", t.ConditionLevel("ダミー付加") - 1)
			If t.ConditionLevel("ダミー付加") = 0 Then
				t.DeleteCondition("ダミー付加")
			End If
			
			CheckDummyFeature = True
		ElseIf t.IsFeatureAvailable("ダミー") Then 
			If t.ConditionLevel("ダミー破壊") < t.FeatureLevel("ダミー") Then
				'命中時の特殊効果
				IsWavePlayed = False
				If Not be_quiet Then
					PilotMessage(wname & "(命中)")
				End If
				If IsAnimationDefined(wname & "(命中)") Or IsAnimationDefined(wname) Then
					PlayAnimation(wname & "(命中)")
				ElseIf IsSpecialEffectDefined(wname & "(命中)") Then 
					SpecialEffect(wname & "(命中)")
				ElseIf Not IsWavePlayed Then 
					HitEffect(Me, w, t)
				End If
				
				fname = t.FeatureName("ダミー")
				If Len(fname) > 0 Then
					If InStr(fname, "Lv") > 0 Then
						fname = Left(fname, InStr(fname, "Lv") - 1)
					End If
				Else
					fname = "ダミー"
				End If
				
				If Not be_quiet Then
					If t.IsMessageDefined("ダミー(" & fname & ")") Then
						t.PilotMessage("ダミー(" & fname & ")")
					Else
						t.PilotMessage("ダミー")
					End If
				End If
				If t.IsAnimationDefined("ダミー", fname) Then
					t.PlayAnimation("ダミー", fname)
				Else
					t.SpecialEffect("ダミー", fname)
				End If
				
				If IsSysMessageDefined("ダミー", fname) Then
					SysMessage("ダミー", fname)
				Else
					DisplaySysMessage(t.Nickname & "は[" & fname & "]を身代わりにして攻撃をかわした。")
				End If
				
				If t.IsConditionSatisfied("ダミー破壊") Then
					t.SetConditionLevel("ダミー破壊", t.ConditionLevel("ダミー破壊") + 1)
				Else
					t.AddCondition("ダミー破壊", -1, 1)
				End If
				
				CheckDummyFeature = True
			End If
		End If
	End Function
	
	'シールド防御能力のチェック
	Private Function CheckShieldFeature(ByVal w As Short, ByRef t As Unit, ByVal dmg As Integer, ByVal be_quiet As Boolean, ByRef use_shield As Boolean, ByRef use_shield_msg As Boolean) As Boolean
		Dim prob As Short
		Dim fname As String
		
		'ダメージが0以下ならシールド防御しても意味がない
		If dmg <= 0 Then
			Exit Function
		End If
		
		'Ｓ防御技能を持っている？
		If t.MainPilot.SkillLevel("Ｓ防御") <= 0 Then
			Exit Function
		End If
		
		'行動可能？
		If t.IsConditionSatisfied("行動不能") Or t.IsConditionSatisfied("麻痺") Or t.IsConditionSatisfied("石化") Or t.IsConditionSatisfied("凍結") Or t.IsConditionSatisfied("睡眠") Or t.IsUnderSpecialPowerEffect("行動不能") Or t.IsUnderSpecialPowerEffect("無防備") Then
			Exit Function
		End If
		
		'シールド防御出来ない武器？
		If IsWeaponClassifiedAs(w, "精") Or IsWeaponClassifiedAs(w, "殺") Or IsWeaponClassifiedAs(w, "浸") Then
			Exit Function
		End If
		
		'スペシャルパワーで無効化される？
		If IsUnderSpecialPowerEffect("シールド防御無効化") Then
			Exit Function
		End If
		
		'シールド系防御能力を検索
		If t.IsFeatureAvailable("シールド") Then
			prob = t.MainPilot.SkillLevel("Ｓ防御")
			fname = t.FeatureName("シールド")
		ElseIf t.IsFeatureAvailable("小型シールド") Then 
			prob = t.MainPilot.SkillLevel("Ｓ防御")
			fname = t.FeatureName("小型シールド")
		ElseIf t.IsFeatureAvailable("エネルギーシールド") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "無") And Not IsUnderSpecialPowerEffect("防御能力無効化") Then 
			prob = t.MainPilot.SkillLevel("Ｓ防御")
			fname = t.FeatureName("エネルギーシールド")
		ElseIf t.IsFeatureAvailable("大型シールド") Then 
			prob = t.MainPilot.SkillLevel("Ｓ防御") + 1
			fname = t.FeatureName("大型シールド")
		ElseIf t.IsFeatureAvailable("アクティブシールド") Then 
			prob = t.MainPilot.SkillLevel("Ｓ防御") + 2
			fname = t.FeatureName("アクティブシールド")
		Else
			'使用可能なシールド系防御能力が無かった
			Exit Function
		End If
		
		'シールド発動確率を満たしている？
		If prob >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動") Then
			use_shield = True
			
			If IsWeaponClassifiedAs(w, "破") Then
				If t.IsFeatureAvailable("小型シールド") Then
					dmg = (5 * dmg) \ 6
				Else
					dmg = 3 * dmg \ 4
				End If
			Else
				If t.IsFeatureAvailable("小型シールド") Then
					dmg = (2 * dmg) \ 3
				Else
					dmg = dmg \ 2
				End If
			End If
			
			If dmg > 0 And dmg < 10 Then
				dmg = 10
			End If
			
			If dmg < t.HP And Not be_quiet Then
				If t.IsMessageDefined("シールド防御(" & fname & ")") Then
					t.PilotMessage("シールド防御(" & fname & ")")
					use_shield_msg = True
				ElseIf t.IsMessageDefined("シールド防御") Then 
					t.PilotMessage("シールド防御")
					use_shield_msg = True
				End If
			End If
			
			If t.IsAnimationDefined("シールド防御", fname) Then
				t.PlayAnimation("シールド防御", fname)
			ElseIf t.IsSpecialEffectDefined("シールド防御", fname) Then 
				t.SpecialEffect("シールド防御", fname)
			Else
				ShieldEffect(t)
			End If
		End If
	End Function
	
	'バリアなどの防御能力のチェック
	Private Function CheckDefenseFeature(ByVal w As Short, ByRef t As Unit, ByRef tx As Short, ByRef ty As Short, ByRef attack_mode As String, ByRef def_mode As String, ByRef dmg As Integer, ByRef msg As String, ByRef be_quiet As Boolean, ByRef is_penetrated As Boolean) As Boolean
		Dim wname As String
		Dim ecost, nmorale As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim fid, frange As Short
		Dim opt As String
		Dim lv_mod As Double
		Dim u As Unit
		Dim slevel As Double
		Dim k, i, j, idx As Short
		Dim neautralize As Boolean
		Dim team, uteam As String
		Dim dmg_mod As Double
		Dim defined As Boolean
		
		wname = WeaponNickname(w)
		team = MainPilot.SkillData("チーム")
		
		'攻撃吸収
		If dmg < 0 Then
			t.HP = t.HP - dmg
			
			If attack_mode <> "反射" Then
				UpdateMessageForm(Me, t)
			Else
				UpdateMessageForm(Me, Nothing)
			End If
			
			NegateEffect(Me, t, w, wname, dmg, "吸収", "", 0, msg, be_quiet)
			
			CheckDefenseFeature = True
			Exit Function
		End If
		
		'攻撃無効化
		If dmg = 0 And Weapon(w).Power > 0 Then
			If IsWeaponClassifiedAs(w, "封") Or IsWeaponClassifiedAs(w, "限") Then
				DisplaySysMessage(msg & t.Nickname & "には[" & wname & "]は通用しない。")
			Else
				NegateEffect(Me, t, w, wname, dmg, "", "", 0, msg, be_quiet)
			End If
			CheckDefenseFeature = True
			Exit Function
		End If
		
		'特殊効果がない場合にはクリティカル発生の可能性がある
		If Not IsNormalWeapon(w) Then
			'特殊効果を伴う武器
			If CriticalProbability(w, t, def_mode) = 0 And Weapon(w).Power = 0 Then
				'攻撃力が0の攻撃は、クリティカル発生率が0の場合も無効化されていると見なす
				NegateEffect(Me, t, w, wname, dmg, "", "", 0, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
		
		'バリア無効化
		If IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			GoTo SkipBarrier
		End If
		
		'広域バリア
		'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u = Nothing
		flevel = 0
		fid = 0
		'バリアをはってくれるユニットを探す
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint
				End If
				
				With MapDataForUnit(i, j)
					'敵？
					If .IsEnemy(t) Then
						GoTo NextPoint
					End If
					
					'行動不能？
					If .MaxAction = 0 Then
						GoTo NextPoint
					End If
					
					'地中にいる？
					If .Area = "地中" Then
						GoTo NextPoint
					End If
					
					'広域バリアを持っている？
					If Not .IsFeatureAvailable("広域バリア") Then
						GoTo NextPoint
					End If
					
					'同じチームに属している？
					uteam = .MainPilot.SkillData("チーム")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "広域バリア" Then
							fdata = .FeatureData(k)
							
							'効果範囲
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'使用条件
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("バリア発動") Then
								'すでに発動済み
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'発動可能かチェック
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("バリア")) And Not .IsConditionSatisfied("バリア無効化") Then
								If .FeatureLevel(k) > flevel Then
									u = MapDataForUnit(i, j)
									flevel = .FeatureLevel(k)
									fid = k
								End If
							End If
						End If
					Next 
				End With
NextPoint: 
			Next 
		Next 
		If Not u Is Nothing Then
			'バリアをはってくれるユニットがいる場合
			If fid = 0 Then
				fname = u.FeatureName0("広域バリア")
				fdata = u.FeatureData("広域バリア")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("バリア") Then
					fname = u.FeatureName0("バリア")
				Else
					fname = "広域バリア"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("バリア発動") Then
				'バリア発動はターン中に一度のみ
				u.EN = u.EN - ecost
				If u.IsMessageDefined("バリア発動(" & fname & ")") Then
					u.PilotMessage("バリア発動(" & fname & ")")
				Else
					u.PilotMessage("バリア発動")
				End If
				If u.IsAnimationDefined("バリア発動", fname) Then
					u.PlayAnimation("バリア発動", fname)
				Else
					u.SpecialEffect("バリア発動", fname)
				End If
				
				If u.IsSysMessageDefined("バリア発動", fname) Then
					u.SysMessage("バリア発動", fname)
				Else
					DisplaySysMessage(u.Nickname & "は[" & fname & "]を発動させた。")
				End If
				
				If fname = "広域バリア" Or fname = "バリア" Then
					u.AddCondition("バリア発動", 1)
				Else
					u.AddCondition("バリア発動", 1, 0, fname & "発動")
				End If
			End If
			
			If 1000 * flevel >= dmg Then
				NegateEffect(Me, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				msg = msg & wname & "が[" & fname & "]を貫いた。;"
			End If
		End If
		
		'広域フィールド
		'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u = Nothing
		flevel = 0
		fid = 0
		'フィールドをはってくれるユニットを探す
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint2
				End If
				
				With MapDataForUnit(i, j)
					'敵？
					If .IsEnemy(t) Then
						GoTo NextPoint2
					End If
					
					'行動不能？
					If .MaxAction = 0 Then
						GoTo NextPoint2
					End If
					
					'地中にいる？
					If .Area = "地中" Then
						GoTo NextPoint2
					End If
					
					'広域フィールドを持っている？
					If Not .IsFeatureAvailable("広域フィールド") Then
						GoTo NextPoint2
					End If
					
					'同じチームに属している？
					uteam = .MainPilot.SkillData("チーム")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint2
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "広域フィールド" Then
							fdata = .FeatureData(k)
							
							'効果範囲
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'使用条件
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("フィールド発動") Then
								'すでに発動済み
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'発動可能かチェック
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("フィールド")) And Not .IsConditionSatisfied("バリア無効化") Then
								If .FeatureLevel(k) > flevel Then
									u = MapDataForUnit(i, j)
									flevel = .FeatureLevel(k)
									fid = k
								End If
							End If
						End If
					Next 
				End With
NextPoint2: 
			Next 
		Next 
		If Not u Is Nothing Then
			'フィールドをはってくれるユニットがいる場合
			If fid = 0 Then
				fname = u.FeatureName0("広域フィールド")
				fdata = u.FeatureData("広域フィールド")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("フィールド") Then
					fname = u.FeatureName0("フィールド")
				Else
					fname = "広域フィールド"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("フィールド発動") Then
				'フィールド発動はターン中に一度のみ
				u.EN = u.EN - ecost
				If u.IsMessageDefined("フィールド発動(" & fname & ")") Then
					u.PilotMessage("フィールド発動(" & fname & ")")
				Else
					u.PilotMessage("フィールド発動")
				End If
				If u.IsAnimationDefined("フィールド発動", fname) Then
					u.PlayAnimation("フィールド発動", fname)
				Else
					u.SpecialEffect("フィールド発動", fname)
				End If
				
				If u.IsSysMessageDefined("フィールド発動", fname) Then
					u.SysMessage("フィールド発動", fname)
				Else
					DisplaySysMessage(u.Nickname & "は[" & fname & "]を発動させた。")
				End If
				
				If fname = "広域フィールド" Or fname = "フィールド" Then
					u.AddCondition("フィールド発動", 1)
				Else
					u.AddCondition("フィールド発動", 1, 0, fname & "発動")
				End If
			End If
			
			If 500 * flevel >= dmg Then
				NegateEffect(Me, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				dmg = dmg - 500 * flevel
				msg = msg & wname & "が[" & fname & "]を貫いた。;"
			End If
		End If
		
		'広域プロテクション
		'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u = Nothing
		flevel = 0
		fid = 0
		'プロテクションをはってくれるユニットを探す
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint3
				End If
				
				With MapDataForUnit(i, j)
					'敵？
					If .IsEnemy(t) Then
						GoTo NextPoint3
					End If
					
					'行動不能？
					If .MaxAction = 0 Then
						GoTo NextPoint3
					End If
					
					'地中にいる？
					If .Area = "地中" Then
						GoTo NextPoint3
					End If
					
					'広域プロテクションを持っている？
					If Not .IsFeatureAvailable("広域プロテクション") Then
						GoTo NextPoint3
					End If
					
					'同じチームに属している？
					uteam = .MainPilot.SkillData("チーム")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint3
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "広域プロテクション" Then
							fdata = .FeatureData(k)
							
							'効果範囲
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'使用条件
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("プロテクション発動") Then
								'すでに発動済み
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'発動可能かチェック
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("プロテクション")) And Not .IsConditionSatisfied("バリア無効化") Then
								If .FeatureLevel(k) > flevel Then
									u = MapDataForUnit(i, j)
									flevel = .FeatureLevel(k)
									fid = k
								End If
							End If
						End If
					Next 
				End With
NextPoint3: 
			Next 
		Next 
		If Not u Is Nothing Then
			'プロテクションをはってくれるユニットがいる場合
			If fid = 0 Then
				fname = u.FeatureName0("広域プロテクション")
				fdata = u.FeatureData("広域プロテクション")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("プロテクション") Then
					fname = u.FeatureName0("プロテクション")
				Else
					fname = "広域プロテクション"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("プロテクション発動") Then
				'プロテクション発動はターン中に一度のみ
				u.EN = u.EN - ecost
				If u.IsMessageDefined("プロテクション発動(" & fname & ")") Then
					u.PilotMessage("プロテクション発動(" & fname & ")")
				Else
					u.PilotMessage("プロテクション発動")
				End If
				If u.IsAnimationDefined("プロテクション発動", fname) Then
					u.PlayAnimation("プロテクション発動", fname)
				Else
					u.SpecialEffect("プロテクション発動", fname)
				End If
				
				If u.IsSysMessageDefined("プロテクション発動", fname) Then
					u.SysMessage("プロテクション発動", fname)
				Else
					DisplaySysMessage(u.Nickname & "は[" & fname & "]を発動させた。")
				End If
				
				If fname = "広域プロテクション" Or fname = "プロテクション" Then
					u.AddCondition("プロテクション発動", 1)
				Else
					u.AddCondition("プロテクション発動", 1, 0, fname & "発動")
				End If
			End If
			
			dmg = dmg * (10 - flevel) \ 10
			If dmg < 0 Then
				msg = msg & u.Nickname & "がダメージを吸収した。;"
				u.HP = u.HP - dmg
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				msg = msg & u.Nickname & "の[" & fname & "]がダメージを減少させた。;"
			End If
		End If
		
		'バリア能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "バリア" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("広域バリア") Then
						fname = t.FeatureName0("広域バリア")
					Else
						fname = "バリア"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'必要条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 10
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("バリア")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "は[" & t.FeatureName(i) & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("バリア")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("バリア")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize Then
					'バリア発動
					t.EN = t.EN - ecost
					If dmg <= 1000 * flevel + slevel Then
						If ecost <> 0 Then
							If attack_mode <> "反射" Then
								UpdateMessageForm(Me, t)
							Else
								UpdateMessageForm(Me, Nothing)
							End If
						End If
						NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
						CheckDefenseFeature = True
						Exit Function
					ElseIf flevel > 0 Or slevel > 0 Then 
						If InStr(msg, "[" & fname & "]を貫いた") = 0 Then
							is_penetrated = True
							msg = msg & wname & "が[" & fname & "]を貫いた。;"
							If t.IsAnimationDefined("バリア貫通", fname) Then
								t.PlayAnimation("バリア貫通", fname)
							Else
								t.SpecialEffect("バリア貫通", fname)
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'フィールド能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "フィールド" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("バリア") Then
						fname = t.FeatureName("バリア")
					Else
						fname = "フィールド"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'必要条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("フィールド")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "は[" & fname & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("フィールド")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("フィールド")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize Then
					'フィールド発動
					t.EN = t.EN - ecost
					If dmg <= 500 * flevel + slevel Then
						If ecost <> 0 Then
							If attack_mode <> "反射" Then
								UpdateMessageForm(Me, t)
							Else
								UpdateMessageForm(Me, Nothing)
							End If
						End If
						NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
						CheckDefenseFeature = True
						Exit Function
					ElseIf flevel > 0 Or slevel > 0 Then 
						dmg = dmg - 500 * flevel - slevel
						If InStr(msg, "[" & fname & "]を貫いた") = 0 Then
							msg = msg & wname & "が[" & fname & "]を貫いた。;"
						End If
					End If
				End If
			End If
		Next 
		
		'プロテクション能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "プロテクション" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("バリア") Then
						fname = t.FeatureName("バリア")
					Else
						fname = "プロテクション"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'必要条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 10
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("プロテクション")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "は[" & fname & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("プロテクション")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("プロテクション")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize And dmg > 0 Then
					'プロテクション発動
					dmg = dmg * (100 - 10 * flevel - slevel) \ 100
					
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "反射" Then
							UpdateMessageForm(Me, t)
						Else
							UpdateMessageForm(Me, Nothing)
						End If
					End If
					
					If dmg <= 0 Then
						NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
						t.HP = t.HP - dmg
						UpdateMessageForm(Me, t)
						CheckDefenseFeature = True
						Exit Function
					ElseIf flevel > 0 Or slevel > 0 Then 
						If InStr(msg, "[" & fname & "]") = 0 Then
							msg = msg & "[" & fname & "]がダメージを減少させた。;"
						End If
					End If
				End If
			End If
		Next 
		
		'バリアシールド、アクティブフィールド、アクティブプロテクションは能動防御
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("無防備") Then
			GoTo SkipActiveBarrier
		End If
		
		'バリアシールド能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "バリアシールド" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("反射") Then
						fname = t.FeatureName0("反射")
					Else
						fname = "バリアシールド"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'使用条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 10
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("バリアシールド")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "は[" & t.FeatureName(i) & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("バリアシールド")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("バリアシールド")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("Ｓ防御") > 0 And Not neautralize Then
					'バリアシールド発動
					If t.MainPilot.SkillLevel("Ｓ防御") >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動") Then
						t.EN = t.EN - ecost
						If dmg <= 1000 * flevel + slevel Then
							If ecost <> 0 Then
								If attack_mode <> "反射" Then
									UpdateMessageForm(Me, t)
								Else
									UpdateMessageForm(Me, Nothing)
								End If
							End If
							NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
							CheckDefenseFeature = True
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							If InStr(msg, "[" & fname & "]を貫いた") = 0 Then
								is_penetrated = True
								msg = msg & wname & "が[" & fname & "]を貫いた。;"
								If t.IsAnimationDefined("バリア貫通", fname) Then
									t.PlayAnimation("バリア貫通", fname)
								Else
									t.SpecialEffect("バリア貫通", fname)
								End If
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'アクティブフィールド能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "アクティブフィールド" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("反射") Then
						fname = t.FeatureName0("反射")
					Else
						fname = "アクティブフィールド"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'使用条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("アクティブフィールド")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "は[" & t.FeatureName(i) & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("アクティブフィールド")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("アクティブフィールド")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("Ｓ防御") > 0 And Not neautralize Then
					'アクティブフィールド発動
					If t.MainPilot.SkillLevel("Ｓ防御") >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動") Then
						t.EN = t.EN - ecost
						If dmg <= 500 * flevel + slevel Then
							If ecost <> 0 Then
								If attack_mode <> "反射" Then
									UpdateMessageForm(Me, t)
								Else
									UpdateMessageForm(Me, Nothing)
								End If
							End If
							NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
							CheckDefenseFeature = True
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							dmg = dmg - 500 * flevel - slevel
							If InStr(msg, "[" & fname & "]を貫いた") = 0 Then
								msg = msg & wname & "が[" & fname & "]を貫いた。;"
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'アクティブプロテクション能力
		For i = 1 To t.CountFeature
			If t.Feature(i) = "アクティブプロテクション" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("反射") Then
						fname = t.FeatureName0("反射")
					Else
						fname = "アクティブプロテクション"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'使用条件
				If IsNumeric(LIndex(fdata, 3)) Then
					ecost = CShort(LIndex(fdata, 3))
				Else
					ecost = 10
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					nmorale = CShort(LIndex(fdata, 4))
				Else
					nmorale = 0
				End If
				
				'オプション
				neautralize = False
				slevel = 0
				For j = 5 To LLength(fdata)
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("アクティブプロテクション")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "は[" & t.FeatureName(i) & "]を中和した。;"
								neautralize = True
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("アクティブプロテクション")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("アクティブプロテクション")
								If flevel <= 0 Then
									msg = msg & Nickname & "は[" & fname & "]を中和した。;"
									neautralize = True
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								neautralize = True
							End If
						Case "手動"
							If def_mode <> "防御" Then
								neautralize = True
							End If
						Case "能力必要", "バリア無効化無効"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'バリア無効化で無効化されている？
				If t.IsConditionSatisfied("バリア無効化") Then
					If InStr(fdata, "バリア無効化無効") = 0 Then
						neautralize = True
					End If
				End If
				
				'発動可能？
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("Ｓ防御") > 0 And Not neautralize And dmg > 0 Then
					'アクティブプロテクション発動
					If t.MainPilot.SkillLevel("Ｓ防御") >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動") Then
						dmg = dmg * (100 - 10 * flevel - slevel) \ 100
						
						If ecost <> 0 Then
							t.EN = t.EN - ecost
							If attack_mode <> "反射" Then
								UpdateMessageForm(Me, t)
							Else
								UpdateMessageForm(Me, Nothing)
							End If
						End If
						
						If dmg <= 0 Then
							NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
							t.HP = t.HP - dmg
							UpdateMessageForm(Me, t)
							CheckDefenseFeature = True
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							If InStr(msg, "[" & fname & "]") = 0 Then
								msg = msg & "[" & fname & "]がダメージを減少させた。;"
							End If
						End If
					End If
				End If
			End If
		Next 
		
SkipActiveBarrier: 
		
		'相手の攻撃をＥＮに変換
		For i = 1 To t.CountFeature
			If t.Feature(i) = "変換" Then
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'必要気力
				If IsNumeric(LIndex(fdata, 3)) Then
					nmorale = CShort(LIndex(fdata, 3))
				Else
					nmorale = 0
				End If
				
				'発動可能？
				If t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					t.EN = t.EN + 0.01 * flevel * dmg
				End If
			End If
		Next 
		
		'対ビーム用防御能力
		If IsWeaponClassifiedAs(w, "Ｂ") Then
			'ビーム吸収
			If t.IsFeatureAvailable("ビーム吸収") Then
				fname = t.FeatureName("ビーム吸収")
				t.HP = t.HP + dmg
				NegateEffect(Me, t, w, wname, dmg, fname, "Ｂ", 0, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
		
SkipBarrier: 
		
		'攻撃力が0の場合は盾や融合を無視
		If Weapon(w).Power = 0 Then
			Exit Function
		End If
		
		'盾防御
		If t.IsFeatureAvailable("盾") And t.MainPilot.IsSkillAvailable("Ｓ防御") And t.MaxAction > 0 And Not IsWeaponClassifiedAs(w, "精") And Not IsWeaponClassifiedAs(w, "浸") And Not IsWeaponClassifiedAs(w, "殺") And Not IsUnderSpecialPowerEffect("シールド防御無効化") And Not t.IsUnderSpecialPowerEffect("無防備") And (t.IsConditionSatisfied("盾付加") Or t.FeatureLevel("盾") > t.ConditionLevel("盾ダメージ")) Then
			fname = t.FeatureName0("盾")
			
			If Not be_quiet Then
				t.PilotMessage("シールド防御", fname)
			End If
			
			If t.IsAnimationDefined("シールド防御", fname) Then
				t.PlayAnimation("シールド防御", fname)
			ElseIf t.IsSpecialEffectDefined("シールド防御", fname) Then 
				t.SpecialEffect("シールド防御", fname)
			Else
				ShowAnimation("ミドルシールド発動")
			End If
			
			If IsWeaponClassifiedAs(w, "破") Then
				dmg = MaxLng(dmg - 50 * (t.MainPilot.SkillLevel("Ｓ防御") + 4), 0)
			Else
				dmg = MaxLng(dmg - 100 * (t.MainPilot.SkillLevel("Ｓ防御") + 4), 0)
			End If
			
			If t.IsSysMessageDefined("シールド防御", fname) Then
				t.SysMessage("シールド防御", fname)
			ElseIf dmg = 0 Then 
				DisplaySysMessage(t.Nickname & "は[" & fname & "]を使って攻撃を防いだ。")
			Else
				DisplaySysMessage(t.Nickname & "は[" & fname & "]を使ってダメージを軽減させた。")
			End If
			
			If dmg = 0 Then
				'攻撃を盾で完全に防いだ場合
				
				'命中時の特殊効果
				IsWavePlayed = False
				If Not be_quiet Then
					PilotMessage(wname & "(命中)")
				End If
				If IsAnimationDefined(wname & "(命中)") Or IsAnimationDefined(wname) Then
					PlayAnimation(wname & "(命中)")
				ElseIf IsSpecialEffectDefined(wname & "(命中)") Then 
					SpecialEffect(wname & "(命中)")
				ElseIf Not IsWavePlayed Then 
					HitEffect(Me, w, t)
				End If
				
				CheckDefenseFeature = True
				Exit Function
			Else
				'攻撃が盾を貫通した場合
				If t.IsConditionSatisfied("盾付加") Then
					If IsWeaponClassifiedAs(w, "破") Then
						t.SetConditionLevel("盾付加", t.ConditionLevel("盾付加") - 2)
					Else
						t.SetConditionLevel("盾付加", t.ConditionLevel("盾付加") - 1)
					End If
					If t.ConditionLevel("盾付加") <= 0 Then
						t.DeleteCondition("盾付加")
					End If
				Else
					If IsWeaponClassifiedAs(w, "破") Then
						If t.IsConditionSatisfied("盾ダメージ") Then
							t.SetConditionLevel("盾ダメージ", t.ConditionLevel("盾ダメージ") + 2)
						Else
							t.AddCondition("盾ダメージ", -1, 2)
						End If
					Else
						If t.IsConditionSatisfied("盾ダメージ") Then
							t.SetConditionLevel("盾ダメージ", t.ConditionLevel("盾ダメージ") + 1)
						Else
							t.AddCondition("盾ダメージ", -1, 1)
						End If
					End If
				End If
			End If
		End If
		
		'融合能力
		If t.IsFeatureAvailable("融合") Then
			'融合可能？
			If Not IsWeaponClassifiedAs(w, "武") And Not IsWeaponClassifiedAs(w, "突") And Not IsWeaponClassifiedAs(w, "接") And (t.FeatureLevel("融合") >= Dice(16) Or t.IsUnderSpecialPowerEffect("特殊防御発動")) Then
				'融合発動
				t.HP = t.HP + dmg
				
				If attack_mode <> "反射" Then
					UpdateMessageForm(Me, t)
				Else
					UpdateMessageForm(Me, Nothing)
				End If
				
				fname = t.FeatureName("融合")
				If Not be_quiet Then
					If t.IsMessageDefined("攻撃無効化(" & fname & ")") Then
						t.PilotMessage("攻撃無効化(" & fname & ")")
					Else
						t.PilotMessage("攻撃無効化")
					End If
				End If
				
				If t.IsAnimationDefined("攻撃無効化", fname) Then
					t.PlayAnimation("攻撃無効化", fname)
				ElseIf t.IsSpecialEffectDefined("攻撃無効化", fname) Then 
					t.SpecialEffect("攻撃無効化", fname)
				Else
					AbsorbEffect(Me, w, t)
				End If
				If IsAnimationDefined(wname & "(攻撃無効化)") Then
					PlayAnimation(wname & "(攻撃無効化)")
				ElseIf IsSpecialEffectDefined(wname & "(攻撃無効化)") Then 
					SpecialEffect(wname & "(攻撃無効化)")
				End If
				
				If t.IsSysMessageDefined("攻撃無効化", fname) Then
					t.SysMessage("攻撃無効化", fname)
				Else
					If IsWeaponClassifiedAs(w, "実") Then
						DisplaySysMessage(msg & t.Nickname & "は[" & wname & "]を取り込んだ。")
					Else
						DisplaySysMessage(msg & t.Nickname & "は[" & wname & "]の攻撃を吸収した。")
					End If
				End If
				
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
	End Function
	
	'自動反撃のチェック
	Public Sub CheckAutoAttack(ByVal w As Short, ByRef t As Unit, ByRef attack_mode As String, ByRef def_mode As String, ByRef dmg As Integer, ByRef be_quiet As Boolean)
		Dim wname2 As String
		Dim w2 As Short
		Dim ecost, nmorale As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim slevel, lv_mod As Double
		Dim opt As String
		Dim j, i, idx As Short
		Dim prob As Integer
		Dim buf As String
		
		'反撃系の攻撃に対しては自動反撃を行わない
		If attack_mode = "自動反撃" Or attack_mode = "反射" Or attack_mode = "当て身技" Then
			Exit Sub
		End If
		
		'マップ攻撃、間接攻撃、無属性武器には自動反撃出来ない
		If IsWeaponClassifiedAs(w, "Ｍ") Or IsWeaponClassifiedAs(w, "間") Or IsWeaponClassifiedAs(w, "無") Or IsUnderSpecialPowerEffect("防御能力無効化") Then
			Exit Sub
		End If
		
		'自動反撃の結果形態が変化して特殊能力数が変わることがあるのでFor文は使わない
		i = 1
		Do While i <= t.CountFeature
			If t.Feature(i) = "自動反撃" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					fname = "自動反撃"
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'自動反撃確率の設定
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'見切り
				If t.IsUnderSpecialPowerEffect("特殊防御発動") Then
					prob = 100
				End If
				
				'対象属性の判定
				If Not t.IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) Then
					prob = 0
				End If
				
				'使用条件
				If IsNumeric(LIndex(fdata, 5)) Then
					ecost = CShort(LIndex(fdata, 5))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 6)) Then
					nmorale = CShort(LIndex(fdata, 6))
				Else
					nmorale = 0
				End If
				If t.EN < ecost Or t.MainPilot.Morale < nmorale Then
					prob = 0
				End If
				
				'能動防御は行動できなければ発動しない
				If t.MaxAction = 0 Then
					If InStr(fdata, "完全自動") = 0 Then
						prob = 0
					End If
				End If
				
				'オプション
				slevel = 0
				For j = 7 To LLength(fdata)
					If prob = 0 Then
						Exit For
					End If
					
					opt = LIndex(fdata, j)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					
					Select Case t.MainPilot.SkillType(opt)
						Case "相殺"
							If IsSameCategory(fdata, FeatureData("自動反撃")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								prob = 0
							End If
						Case "中和"
							If IsSameCategory(fdata, FeatureData("自動反撃")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								flevel = flevel - FeatureLevel("自動反撃")
								If flevel <= 0 Then
									prob = 0
								End If
							End If
						Case "近接無効"
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								prob = 0
							End If
						Case "手動"
							If def_mode <> "防御" Then
								prob = 0
							End If
						Case "能力必要"
							'スキップ
						Case "同調率"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "霊力"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "オーラ"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "超能力"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "能力必要") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'ダメージが許容範囲外であれば自動反撃を使えない
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'使用する武器を検索
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To t.CountWeapon
					If t.Weapon(j).Name = wname2 Then
						If t.IsWeaponAvailable(j, "必要技能無視") Then
							If IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "突") Or IsWeaponClassifiedAs(w, "接") Then
								w2 = j
							Else
								If t.IsTargetWithinRange(j, Me) Then
									w2 = j
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'自動反撃発動
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'メッセージ
					If Not be_quiet Then
						If t.IsMessageDefined("自動反撃(" & fname & ")") Then
							t.PilotMessage("自動反撃(" & fname & ")")
						Else
							t.PilotMessage("自動反撃")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'効果音
					If Not IsWavePlayed Then
						If t.IsAnimationDefined("自動反撃", fname) Then
							t.PlayAnimation("自動反撃", fname)
						ElseIf t.IsSpecialEffectDefined("自動反撃", fname) Then 
							t.SpecialEffect("自動反撃", fname)
						End If
					End If
					
					If t.IsSysMessageDefined("自動反撃", fname) Then
						t.SysMessage("自動反撃", fname)
					Else
						DisplaySysMessage(t.Nickname & "は" & t.WeaponNickname(w2) & "で反撃した。")
					End If
					
					'自動反撃で攻撃をかける
					t.Attack(w2, Me, "自動反撃", "")
					t = t.CurrentForm
					If Status_Renamed <> "出撃" Or t.Status_Renamed <> "出撃" Then
						Exit Sub
					End If
				End If
			End If
			i = i + 1
		Loop 
	End Sub
	
	
	'追加攻撃のチェック
	Public Sub CheckAdditionalAttack(ByVal w As Short, ByRef t As Unit, ByVal be_quiet As Boolean, ByRef attack_mode As String, ByRef def_mode As String, ByVal dmg As Integer)
		Dim wnskill, wname, wnickname, wclass As String
		Dim wtype, sname As String
		Dim wname2 As String
		Dim w2 As Short
		Dim ecost, nmorale As Short
		Dim fname, fdata As String
		Dim flevel As Double
		Dim i, j As Short
		Dim prob As Integer
		Dim buf As String
		Dim found As Boolean
		Dim attack_count As Short
		
		wname = Weapon(w).Name
		wnickname = WeaponNickname(w)
		wclass = WeaponClass(w)
		wnskill = Weapon(w).NecessarySkill
		
		'追加攻撃の結果形態が変化して特殊能力数が変わることがあるのでFor文は使わない
		i = 1
		Do While i <= CountFeature
			If Feature(i) = "追加攻撃" Then
				fname = FeatureName0(i)
				If fname = "" Then
					fname = "追加攻撃"
				End If
				fdata = FeatureData(i)
				flevel = FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'追加攻撃確率の設定
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = SkillLevel(buf) * 100 / 16
				End If
				
				'対象武器の判定
				wtype = LIndex(fdata, 3)
				found = False
				If Left(wtype, 1) = "@" Then
					'武器名または必要技能による指定
					wtype = Mid(wtype, 2)
					If wname = wtype Or wnickname = wtype Then
						found = True
					Else
						For j = 1 To LLength(wnskill)
							sname = LIndex(wnskill, j)
							If InStr(sname, "Lv") > 0 Then
								sname = Left(sname, InStr(sname, "Lv") - 1)
							End If
							If sname = wtype Then
								found = True
								Exit For
							End If
						Next 
					End If
				Else
					'属性による指定
					Select Case wtype
						Case "全"
							found = True
						Case "物"
							If InStrNotNest(wclass, "魔") = 0 Or InStrNotNest(wclass, "魔武") > 0 Or InStrNotNest(wclass, "魔突") > 0 Or InStrNotNest(wclass, "魔接") > 0 Or InStrNotNest(wclass, "魔銃") > 0 Or InStrNotNest(wclass, "魔実") > 0 Then
								found = True
							End If
						Case Else
							If IsAttributeClassified(wtype, wclass) Then
								found = True
							End If
					End Select
				End If
				If Not found Then
					prob = 0
				End If
				
				'使用条件
				If IsNumeric(LIndex(fdata, 5)) Then
					ecost = CShort(LIndex(fdata, 5))
				Else
					ecost = 0
				End If
				If IsNumeric(LIndex(fdata, 6)) Then
					nmorale = CShort(LIndex(fdata, 6))
				Else
					nmorale = 0
				End If
				If EN < ecost Or MainPilot.Morale < nmorale Then
					prob = 0
				End If
				
				'連鎖不可
				If InStr(fdata, "連鎖不可") > 0 Then
					If attack_count > 0 Or attack_mode = "追加攻撃" Then
						prob = 0
					End If
				End If
				
				'命中時限定
				If InStr(fdata, "命中時限定") > 0 Then
					If dmg <= 0 Then
						prob = 0
					End If
				End If
				
				'使用する武器を検索
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To CountWeapon
					If Weapon(j).Name = wname2 Then
						If IsWeaponAvailable(j, "必要技能無視") Then
							If IsTargetWithinRange(j, t) Then
								w2 = j
								Exit For
							End If
						End If
					End If
				Next 
				
				'追加攻撃反撃発動
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						EN = EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'メッセージ
					If Not be_quiet Then
						If IsMessageDefined("追加攻撃(" & fname & ")") Then
							PilotMessage("追加攻撃(" & fname & ")")
						Else
							PilotMessage("追加攻撃")
						End If
					End If
					
					'効果音
					If IsAnimationDefined("追加攻撃", fname) Then
						PlayAnimation("追加攻撃", fname)
					ElseIf IsSpecialEffectDefined("追加攻撃", fname) Then 
						SpecialEffect("追加攻撃", fname)
					End If
					
					If IsSysMessageDefined("追加攻撃", fname) Then
						SysMessage("追加攻撃", fname)
					Else
						DisplaySysMessage(Nickname & "はさらに[" & WeaponNickname(w2) & "]で攻撃を加えた。")
					End If
					
					'追加攻撃をかける
					Attack(w2, t, "追加攻撃", def_mode)
					t = t.CurrentForm
					If Status_Renamed <> "出撃" Or t.Status_Renamed <> "出撃" Then
						Exit Sub
					End If
					
					'追加攻撃を実施したことを記録
					attack_count = attack_count + 1
				End If
			End If
			i = i + 1
		Loop 
	End Sub
	
	'特殊能力 fdata1 と fdata2 が同じ名称か判定
	'「中和」「相殺」用
	Private Function IsSameCategory(ByRef fdata1 As String, ByRef fdata2 As String) As Boolean
		Dim fc1, fc2 As String
		
		fc1 = LIndex(fdata1, 1)
		'レベル指定を除く
		If InStr(fc1, "Lv") > 0 Then
			fc1 = Left(fc1, InStr(fc1, "Lv") - 1)
		End If
		
		fc2 = LIndex(fdata2, 1)
		'レベル指定を除く
		If InStr(fc2, "Lv") > 0 Then
			fc2 = Left(fc2, InStr(fc2, "Lv") - 1)
		End If
		
		If fc1 = fc2 Then
			IsSameCategory = True
		End If
	End Function
	
	'クリティカルによる特殊効果
	Public Function CauseEffect(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef critical_type As String, ByRef def_mode As String, ByVal will_die As Boolean) As Boolean
		Dim i, prob, j As Short
		Dim fname, wname, ch As String
		Dim Skill() As String
		
		wname = WeaponNickname(w)
		
		'特殊効果発生確率
		If IsUnderSpecialPowerEffect("特殊効果発動") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		If will_die Then
			'メッセージ等がうっとうしいので破壊が確定している場合は
			'通常の特殊効果をスキップ
			GoTo SkipNormalEffect
		End If
		
		'各種効果の発動チェック
		
		'捕縛攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "縛") And Not t.SpecialEffectImmune("縛") Then
				msg = msg & "[" & t.Nickname & "]の自由を奪った。;"
				If IsWeaponLevelSpecified(w, "縛") Then
					t.AddCondition("行動不能", WeaponLevel(w, "縛"))
				Else
					t.AddCondition("行動不能", 2)
				End If
				critical_type = critical_type & " 捕縛"
				CauseEffect = True
			End If
		End If
		
		'ショック攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "Ｓ") And Not t.SpecialEffectImmune("Ｓ") Then
				msg = msg & "[" & t.Nickname & "]を一時的に行動不能にした。;"
				If IsWeaponLevelSpecified(w, "Ｓ") Then
					t.AddCondition("行動不能", WeaponLevel(w, "Ｓ"))
				Else
					t.AddCondition("行動不能", 1)
				End If
				critical_type = critical_type & " ショック"
				CauseEffect = True
			End If
		End If
		
		'装甲劣化攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "劣") And Not t.SpecialEffectImmune("劣") Then
				msg = msg & "[" & t.Nickname & "]の" & Term("装甲", t) & "を劣化させた。;"
				If IsWeaponLevelSpecified(w, "劣") Then
					t.AddCondition("装甲劣化", WeaponLevel(w, "劣"), DEFAULT_LEVEL, Term("装甲", t) & "劣化")
				Else
					t.AddCondition("装甲劣化", 10000, DEFAULT_LEVEL, Term("装甲", t) & "劣化")
				End If
				critical_type = critical_type & " 劣化"
				CauseEffect = True
			End If
		End If
		
		'バリア中和攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "中") And Not t.SpecialEffectImmune("中") And (t.IsFeatureAvailable("バリア") Or t.IsFeatureAvailable("バリアシールド") Or t.IsFeatureAvailable("広域バリア") Or t.IsFeatureAvailable("フィールド") Or t.IsFeatureAvailable("アクティブフィールド") Or t.IsFeatureAvailable("広域フィールド") Or t.IsFeatureAvailable("プロテクション") Or t.IsFeatureAvailable("アクティプロテクション") Or t.IsFeatureAvailable("広域プロテクション")) Then
				fname = "バリア"
				If t.IsFeatureAvailable("バリア") And InStr(t.FeatureData("バリア"), "バリア無効化無効") = 0 Then
					fname = t.FeatureName0("バリア")
					If Len(fname) = 0 Then
						fname = "バリア"
					End If
				ElseIf t.IsFeatureAvailable("バリアシールド") And InStr(t.FeatureData("バリアシールド"), "バリア無効化無効") = 0 Then 
					fname = t.FeatureName0("バリアシールド")
					If Len(fname) = 0 Then
						fname = "バリアシールド"
					End If
				ElseIf t.IsFeatureAvailable("広域バリア") Then 
					fname = t.FeatureName0("広域バリア")
					If Len(fname) = 0 Then
						fname = "広域バリア"
					End If
				ElseIf t.IsFeatureAvailable("フィールド") And InStr(t.FeatureData("フィールド"), "バリア無効化無効") = 0 Then 
					fname = t.FeatureName0("フィールド")
					If Len(fname) = 0 Then
						fname = "フィールド"
					End If
				ElseIf t.IsFeatureAvailable("アクティブフィールド") And InStr(t.FeatureData("アクティブフィールド"), "バリア無効化無効") = 0 Then 
					fname = t.FeatureName0("アクティブフィールド")
					If Len(fname) = 0 Then
						fname = "アクティブフィールド"
					End If
				ElseIf t.IsFeatureAvailable("広域フィールド") Then 
					fname = t.FeatureName0("広域フィールド")
					If Len(fname) = 0 Then
						fname = "広域フィールド"
					End If
				ElseIf t.IsFeatureAvailable("プロテクション") And InStr(t.FeatureData("プロテクション"), "バリア無効化無効") = 0 Then 
					fname = t.FeatureName0("プロテクション")
					If Len(fname) = 0 Then
						fname = "プロテクション"
					End If
				ElseIf t.IsFeatureAvailable("アクティブプロテクション") And InStr(t.FeatureData("アクティブプロテクション"), "バリア無効化無効") = 0 Then 
					fname = t.FeatureName0("アクティブプロテクション")
					If Len(fname) = 0 Then
						fname = "アクティブプロテクション"
					End If
				ElseIf t.IsFeatureAvailable("広域プロテクション") Then 
					fname = t.FeatureName0("広域プロテクション")
					If Len(fname) = 0 Then
						fname = "広域プロテクション"
					End If
				End If
				msg = msg & "[" & t.Nickname & "]の" & fname & "を無効化した。;"
				
				If IsWeaponLevelSpecified(w, "中") Then
					t.AddCondition("バリア無効化", WeaponLevel(w, "中"))
				Else
					t.AddCondition("バリア無効化", 1)
				End If
				critical_type = critical_type & " バリア中和"
				CauseEffect = True
			End If
		End If
		
		'石化攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "石") And Not t.SpecialEffectImmune("石") And t.BossRank < 0 Then
				msg = msg & "[" & t.Nickname & "]を石化させた。;"
				If IsWeaponLevelSpecified(w, "石") Then
					t.AddCondition("石化", WeaponLevel(w, "石"))
				Else
					t.AddCondition("石化", 10000)
				End If
				critical_type = critical_type & " 石化"
				CauseEffect = True
			End If
		End If
		
		'凍結攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "凍") And Not t.SpecialEffectImmune("凍") Then
				msg = msg & "[" & t.Nickname & "]を凍らせた。;"
				If IsWeaponLevelSpecified(w, "凍") Then
					t.AddCondition("凍結", WeaponLevel(w, "凍"))
				Else
					t.AddCondition("凍結", 3)
				End If
				critical_type = critical_type & " 凍結"
				CauseEffect = True
			End If
		End If
		
		'麻痺攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "痺") And Not t.SpecialEffectImmune("痺") Then
				msg = msg & "[" & t.Nickname & "]を麻痺させた。;"
				If IsWeaponLevelSpecified(w, "痺") Then
					t.AddCondition("麻痺", WeaponLevel(w, "痺"))
				Else
					t.AddCondition("麻痺", 3)
				End If
				critical_type = critical_type & " 麻痺"
				CauseEffect = True
			End If
		End If
		
		'催眠攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "眠") And Not t.SpecialEffectImmune("眠") And Not t.MainPilot.Personality = "機械" Then
				msg = msg & "[" & t.MainPilot.Nickname & "]を眠らせた。;"
				If IsWeaponLevelSpecified(w, "眠") Then
					t.AddCondition("睡眠", WeaponLevel(w, "眠"))
				Else
					t.AddCondition("睡眠", 3)
				End If
				critical_type = critical_type & " 睡眠"
				CauseEffect = True
			End If
		End If
		
		'混乱攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "乱") And Not t.SpecialEffectImmune("乱") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]を混乱させた。;"
				If IsWeaponLevelSpecified(w, "乱") Then
					t.AddCondition("混乱", WeaponLevel(w, "乱"))
				Else
					t.AddCondition("混乱", 3)
				End If
				critical_type = critical_type & " 混乱"
				CauseEffect = True
			End If
		End If
		
		If Not t Is Me Then
			'魅了攻撃
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "魅") And Not t.SpecialEffectImmune("魅") And Not t.IsConditionSatisfied("魅了") And Not t.IsConditionSatisfied("憑依") Then
					msg = msg & MainPilot.Nickname & "が[" & t.MainPilot.Nickname & "]を魅了した。;"
					If IsWeaponLevelSpecified(w, "魅") Then
						t.AddCondition("魅了", WeaponLevel(w, "魅"))
					Else
						t.AddCondition("魅了", 3)
					End If
					If Not t.Master Is Nothing Then
						t.Master.DeleteSlave((t.ID))
					End If
					AddSlave(t)
					t.Master = Me
					t.Mode = MainPilot.ID
					PList.UpdateSupportMod(t)
					critical_type = critical_type & " 魅了"
					CauseEffect = True
				End If
			End If
			
			'憑依攻撃
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "憑") And Not t.SpecialEffectImmune("憑") And Not t.IsConditionSatisfied("憑依") And t.BossRank < 0 Then
					msg = msg & MainPilot.Nickname & "が[" & t.Nickname & "]を乗っ取った。;"
					If t.IsConditionSatisfied("魅了") Then
						'憑依の方の効果を優先する
						t.DeleteCondition("魅了")
					End If
					If IsWeaponLevelSpecified(w, "憑") Then
						t.AddCondition("憑依", WeaponLevel(w, "憑"))
					Else
						t.AddCondition("憑依", 10000)
					End If
					If Not t.Master Is Nothing Then
						t.Master.DeleteSlave((t.ID))
					End If
					AddSlave(t)
					t.Master = Me
					PList.UpdateSupportMod(t)
					critical_type = critical_type & " 憑依"
					CauseEffect = True
				End If
			End If
		End If
		
		'撹乱攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "撹") And Not t.SpecialEffectImmune("撹") Then
				msg = msg & "[" & t.Nickname & "]を撹乱した。;"
				If IsWeaponLevelSpecified(w, "撹") Then
					t.AddCondition("撹乱", WeaponLevel(w, "撹"))
				Else
					t.AddCondition("撹乱", 2)
				End If
				critical_type = critical_type & " 撹乱"
				CauseEffect = True
			End If
		End If
		
		'恐怖攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "恐") And Not t.SpecialEffectImmune("恐") Then
				msg = msg & t.MainPilot.Nickname & "は恐怖に陥った。;"
				If IsWeaponLevelSpecified(w, "恐") Then
					t.AddCondition("恐怖", WeaponLevel(w, "恐"))
				Else
					t.AddCondition("恐怖", 3)
				End If
				critical_type = critical_type & " 恐怖"
				CauseEffect = True
			End If
		End If
		
		'目潰し攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "盲") And Not t.SpecialEffectImmune("盲") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]の視力を奪った。;"
				If IsWeaponLevelSpecified(w, "盲") Then
					t.AddCondition("盲目", WeaponLevel(w, "盲"))
				Else
					t.AddCondition("盲目", 3)
				End If
				critical_type = critical_type & " 盲目"
				CauseEffect = True
			End If
		End If
		
		'毒攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "毒") And Not t.SpecialEffectImmune("毒") Then
				msg = msg & t.Nickname & "は毒を受けた。;"
				If IsWeaponLevelSpecified(w, "毒") Then
					t.AddCondition("毒", WeaponLevel(w, "毒"))
				Else
					t.AddCondition("毒", 3)
				End If
				critical_type = critical_type & " 毒"
				CauseEffect = True
			End If
		End If
		
		'攻撃封印攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "不") And Not t.SpecialEffectImmune("不") Then
				msg = msg & "[" & t.Nickname & "]の攻撃能力を奪った。;"
				If IsWeaponLevelSpecified(w, "不") Then
					t.AddCondition("攻撃不能", WeaponLevel(w, "不"))
				Else
					t.AddCondition("攻撃不能", 1)
				End If
				critical_type = critical_type & " 攻撃不能"
				CauseEffect = True
			End If
		End If
		
		'足止め攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "止") And Not t.SpecialEffectImmune("止") Then
				msg = msg & "[" & t.Nickname & "]の動きを止めた。;"
				If t.Party <> Stage Then
					If IsWeaponLevelSpecified(w, "止") Then
						t.AddCondition("移動不能", WeaponLevel(w, "止") + 1)
					Else
						t.AddCondition("移動不能", 2)
					End If
				Else
					If IsWeaponLevelSpecified(w, "止") Then
						t.AddCondition("移動不能", WeaponLevel(w, "止"))
					Else
						t.AddCondition("移動不能", 1)
					End If
				End If
				critical_type = critical_type & " 移動不能"
				CauseEffect = True
			End If
		End If
		
		'沈黙攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "黙") And Not t.SpecialEffectImmune("黙") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]を沈黙させた。;"
				If IsWeaponLevelSpecified(w, "黙") Then
					t.AddCondition("沈黙", WeaponLevel(w, "黙"))
				Else
					t.AddCondition("沈黙", 3)
				End If
				critical_type = critical_type & " 沈黙"
				CauseEffect = True
			End If
		End If
		
		'踊らせ攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "踊") And Not t.SpecialEffectImmune("踊") Then
				msg = msg & "[" & t.Nickname & "]は突然踊りだした。;"
				If IsWeaponLevelSpecified(w, "踊") Then
					t.AddCondition("踊り", WeaponLevel(w, "踊"))
				Else
					t.AddCondition("踊り", 3)
				End If
				critical_type = critical_type & " 踊り"
				CauseEffect = True
			End If
		End If
		
		'狂戦士化攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "狂") And Not t.SpecialEffectImmune("狂") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]は狂戦士と化した。;"
				If IsWeaponLevelSpecified(w, "狂") Then
					t.AddCondition("狂戦士", WeaponLevel(w, "狂"))
				Else
					t.AddCondition("狂戦士", 3)
				End If
				critical_type = critical_type & " 狂戦士"
				CauseEffect = True
			End If
		End If
		
		'ゾンビ化攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "ゾ") And Not t.SpecialEffectImmune("ゾ") Then
				msg = msg & "[" & t.Nickname & "]はゾンビと化した。;"
				If IsWeaponLevelSpecified(w, "ゾ") Then
					t.AddCondition("ゾンビ", WeaponLevel(w, "ゾ"))
				Else
					t.AddCondition("ゾンビ", 10000)
				End If
				critical_type = critical_type & " ゾンビ"
				CauseEffect = True
			End If
		End If
		
		'回復能力阻害攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "害") And Not t.SpecialEffectImmune("害") Then
				msg = msg & "[" & t.Nickname & "]の自己回復能力は封じられた。;"
				If IsWeaponLevelSpecified(w, "害") Then
					t.AddCondition("回復不能", WeaponLevel(w, "害"))
				Else
					t.AddCondition("回復不能", 10000)
				End If
				critical_type = critical_type & " 回復不能"
				CauseEffect = True
			End If
		End If
		
		'特殊効果除去攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "除") And Not t.SpecialEffectImmune("除") Then
				For i = 1 To t.CountCondition
					If (InStr(t.Condition(i), "付加") > 0 Or InStr(t.Condition(i), "強化") > 0 Or InStr(t.Condition(i), "ＵＰ") > 0) And t.Condition(i) <> "ノーマルモード付加" And t.ConditionLifetime(i) > 0 Then
						Exit For
					End If
				Next 
				If i <= t.CountCondition Then
					msg = msg & "[" & t.Nickname & "]にかけられた特殊効果を打ち消した。;"
					Do 
						If (InStr(t.Condition(i), "付加") > 0 Or InStr(t.Condition(i), "強化") > 0 Or InStr(t.Condition(i), "ＵＰ") > 0) And t.Condition(i) <> "ノーマルモード付加" And t.ConditionLifetime(i) > 0 Then
							t.DeleteCondition(i)
						Else
							i = i + 1
						End If
					Loop While i <= t.CountCondition
					critical_type = critical_type & " 解除"
					CauseEffect = True
				End If
			End If
		End If
		
		'即死攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "即") And (Not t.SpecialEffectImmune("即") Or t.Weakness(WeaponClass(w)) Or t.Effective(WeaponClass(w))) And t.BossRank < 0 And (Not IsUnderSpecialPowerEffect("てかげん") Or MainPilot.Technique <= t.MainPilot.Technique) And Not t.IsConditionSatisfied("不死身") Then
				critical_type = critical_type & " 即死"
				CauseEffect = True
				Exit Function
			End If
		End If
		
		'死の宣告
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "告") And Not t.SpecialEffectImmune("告") And t.BossRank < 0 Then
				msg = msg & "[" & t.MainPilot.Nickname & "]に死の宣告が下された。;"
				If InStr(WeaponClass(w), "告L") > 0 Then
					If WeaponLevel(w, "告") > 0 Then
						t.AddCondition("死の宣告", WeaponLevel(w, "告"))
					Else
						t.HP = 1
					End If
				Else
					t.AddCondition("死の宣告", 1)
				End If
				critical_type = critical_type & " 死の宣告"
				CauseEffect = True
			End If
		End If
		
		If t.MainPilot.Personality <> "機械" Then
			'気力減少攻撃
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "脱") And Not t.SpecialEffectImmune("脱") Then
					msg = msg & "[" & t.MainPilot.Nickname & "]の" & Term("気力", t) & "を低下させた。;"
					If IsWeaponLevelSpecified(w, "脱") Then
						t.IncreaseMorale(-5 * WeaponLevel(w, "脱"))
					Else
						t.IncreaseMorale(-10)
					End If
					critical_type = critical_type & " 脱力"
					CauseEffect = True
				End If
			End If
			
			'気力吸収攻撃
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "Ｄ") And Not t.SpecialEffectImmune("Ｄ") Then
					msg = msg & MainPilot.Nickname & "は[" & t.MainPilot.Nickname & "]の" & Term("気力", t) & "を吸い取った。;"
					If IsWeaponLevelSpecified(w, "Ｄ") Then
						t.IncreaseMorale(-5 * WeaponLevel(w, "Ｄ"))
						IncreaseMorale(2.5 * WeaponLevel(w, "Ｄ"))
					Else
						t.IncreaseMorale(-10)
						IncreaseMorale(5)
					End If
					critical_type = critical_type & " 気力吸収"
					CauseEffect = True
				End If
			End If
		End If
		
		'攻撃力低下攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "低攻") And Not t.SpecialEffectImmune("低攻") Then
				msg = msg & "[" & t.Nickname & "]の攻撃力を低下させた。;"
				If t.IsConditionSatisfied("攻撃力ＵＰ") Then
					t.DeleteCondition("攻撃力ＵＰ")
				Else
					If IsWeaponLevelSpecified(w, "低攻") Then
						t.AddCondition("攻撃力ＤＯＷＮ", WeaponLevel(w, "低攻"))
					Else
						t.AddCondition("攻撃力ＤＯＷＮ", 3)
					End If
				End If
				critical_type = critical_type & " 攻撃力ＤＯＷＮ"
				CauseEffect = True
			End If
		End If
		
		'防御力低下攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "低防") And Not t.SpecialEffectImmune("低防") Then
				msg = msg & "[" & t.Nickname & "]の防御力を低下させた。;"
				If t.IsConditionSatisfied("防御力ＵＰ") Then
					t.DeleteCondition("防御力ＵＰ")
				Else
					If IsWeaponLevelSpecified(w, "低防") Then
						t.AddCondition("防御力ＤＯＷＮ", WeaponLevel(w, "低防"))
					Else
						t.AddCondition("防御力ＤＯＷＮ", 3)
					End If
				End If
				critical_type = critical_type & " 防御力ＤＯＷＮ"
				CauseEffect = True
			End If
		End If
		
		'運動性低下攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "低運") And Not t.SpecialEffectImmune("低運") Then
				msg = msg & "[" & t.Nickname & "]の" & Term("運動性", t) & "を低下させた。;"
				If t.IsConditionSatisfied("運動性ＵＰ") Then
					t.DeleteCondition("運動性ＵＰ")
				Else
					If IsWeaponLevelSpecified(w, "低運") Then
						t.AddCondition("運動性ＤＯＷＮ", WeaponLevel(w, "低運"), DEFAULT_LEVEL, Term("運動性", t) & "ＤＯＷＮ")
					Else
						t.AddCondition("運動性ＤＯＷＮ", 3, DEFAULT_LEVEL, Term("運動性", t) & "ＤＯＷＮ")
					End If
				End If
				critical_type = critical_type & " 運動性ＤＯＷＮ"
				CauseEffect = True
			End If
		End If
		
		'移動力低下攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "低移") And Not t.SpecialEffectImmune("低移") Then
				msg = msg & "[" & t.Nickname & "]の" & Term("移動力", t) & "を低下させた。;"
				If t.IsConditionSatisfied("移動力ＵＰ") Then
					t.DeleteCondition("移動力ＵＰ")
				Else
					If IsWeaponLevelSpecified(w, "低移") Then
						t.AddCondition("移動力ＤＯＷＮ", WeaponLevel(w, "低移"), DEFAULT_LEVEL, Term("移動力", t) & "ＤＯＷＮ")
					Else
						t.AddCondition("移動力ＤＯＷＮ", 3, DEFAULT_LEVEL, Term("移動力", t) & "ＤＯＷＮ")
					End If
				End If
				critical_type = critical_type & " 移動力ＤＯＷＮ"
				CauseEffect = True
			End If
		End If
		
		'ＨＰ減衰攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "衰") And Not t.SpecialEffectImmune("衰") Then
				msg = msg & "[" & t.Nickname & "]の" & Term("ＨＰ", t) & "を"
				If t.BossRank >= 0 Then
					Select Case CShort(WeaponLevel(w, "衰"))
						Case 1
							t.HP = t.HP * 7 \ 8
							msg = msg & "12.5%"
						Case 2
							t.HP = t.HP * 3 \ 4
							msg = msg & "25%"
						Case 3
							t.HP = t.HP \ 2
							msg = msg & "50%"
					End Select
				Else
					Select Case CShort(WeaponLevel(w, "衰"))
						Case 1
							t.HP = t.HP * 3 \ 4
							msg = msg & "25%"
						Case 2
							t.HP = t.HP \ 2
							msg = msg & "50%"
						Case 3
							t.HP = t.HP \ 4
							msg = msg & "75%"
					End Select
				End If
				msg = msg & "減少させた。;"
				critical_type = critical_type & " 減衰"
				CauseEffect = True
			End If
		End If
		
		'ＥＮ減衰攻撃
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "滅") And Not t.SpecialEffectImmune("滅") Then
				msg = msg & "[" & t.Nickname & "]の" & Term("ＥＮ", t) & "を"
				If t.BossRank >= 0 Then
					Select Case CShort(WeaponLevel(w, "滅"))
						Case 1
							t.EN = t.EN * 7 \ 8
							msg = msg & "12.5%"
						Case 2
							t.EN = t.EN * 3 \ 4
							msg = msg & "25%"
						Case 3
							t.EN = t.EN \ 2
							msg = msg & "50%"
					End Select
				Else
					Select Case CShort(WeaponLevel(w, "滅"))
						Case 1
							t.EN = t.EN * 3 \ 4
							msg = msg & "25%"
						Case 2
							t.EN = t.EN \ 2
							msg = msg & "50%"
						Case 3
							t.EN = t.EN \ 4
							msg = msg & "75%"
					End Select
				End If
				msg = msg & "減少させた。;"
				critical_type = critical_type & " 減衰"
				CauseEffect = True
			End If
		End If
		
		'弱点付加属性（弱が存在するだけループ）
		i = InStrNotNest(strWeaponClass(w), "弱")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				If Not t.SpecialEffectImmune(ch) Then
					msg = msg & "[" & t.Nickname & "]は[" & ch & "]属性に弱くなった。;"
					If IsWeaponLevelSpecified(w, "弱" & ch) Then
						t.AddCondition(ch & "属性弱点付加", WeaponLevel(w, "弱" & ch))
					Else
						t.AddCondition(ch & "属性弱点付加", 3)
					End If
					critical_type = critical_type & " " & ch & "属性弱点付加"
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "弱", i + 1)
		Loop 
		
		'有効付加属性
		i = InStrNotNest(strWeaponClass(w), "効")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				'既に相手が指定属性を弱点として持っている場合無効
				If Not t.Weakness(ch) And Not t.SpecialEffectImmune(ch) Then
					msg = msg & "[" & t.Nickname & "]に[" & ch & "]属性が有効になった。;"
					If IsWeaponLevelSpecified(w, "効" & ch) Then
						t.AddCondition(ch & "属性有効付加", WeaponLevel(w, "効" & ch))
					Else
						t.AddCondition(ch & "属性有効付加", 3)
					End If
					critical_type = critical_type & " " & ch & "属性有効付加"
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "効", i + 1)
		Loop 
		
		'属性使用禁止攻撃
		i = InStrNotNest(strWeaponClass(w), "剋")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				If Not t.SpecialEffectImmune(ch) Then
					ReDim Skill(0)
					Select Case ch
						Case "オ"
							Skill(0) = "オーラ"
						Case "超"
							Skill(0) = "超能力"
						Case "シ"
							Skill(0) = "同調率"
						Case "サ"
							If t.MainPilot.IsSkillAvailable("超感覚") And t.MainPilot(0).IsSkillAvailable("知覚強化") Then
								ReDim Skill(1)
								Skill(0) = "超感覚"
								Skill(1) = "知覚強化"
							ElseIf t.MainPilot.IsSkillAvailable("超感覚") Then 
								Skill(0) = "超感覚"
							ElseIf t.MainPilot.IsSkillAvailable("知覚強化") Then 
								Skill(0) = "知覚強化"
							Else
								ReDim Skill(1)
								Skill(0) = "超感覚"
								Skill(1) = "知覚強化"
							End If
						Case "霊"
							Skill(0) = "霊力"
						Case "術"
							Skill(0) = "術"
						Case "技"
							Skill(0) = "技"
						Case Else
							Skill(0) = ""
					End Select
					For j = 0 To UBound(Skill)
						If Len(Skill(j)) > 0 Then
							fname = t.MainPilot.SkillName0(Skill(j))
							If fname = "非表示" Then
								fname = Skill(j)
							End If
						Else
							Skill(0) = ch & "属性"
							fname = ch & "属性"
						End If
						msg = msg & "[" & t.Nickname & "]は" & fname & "が使用出来なくなった。;"
						If IsWeaponLevelSpecified(w, "剋" & ch) Then
							t.AddCondition(Skill(j) & "使用不能", WeaponLevel(w, "剋" & ch))
						Else
							t.AddCondition(Skill(j) & "使用不能", 3)
						End If
						critical_type = critical_type & " " & Skill(j) & "使用不能"
					Next 
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "剋", i + 1)
		Loop 
		
SkipNormalEffect: 
		
		'これ以降の効果は敵が破壊される場合も発動する
		
		'盗み
		Dim prev_money As Integer
		Dim iname As String
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "盗") And Not t.SpecialEffectImmune("盗") And Not t.IsConditionSatisfied("すかんぴん") And ((Party = "味方" And t.Party0 <> "味方") Or (Party <> "味方" And t.Party0 = "味方")) Then
				If t.Party0 = "味方" Then
					'味方の場合は必ず資金が減少する
					prev_money = Money
					IncrMoney(-t.Value \ 2)
					If Weapon(w).Power > 0 Then
						msg = msg & "[" & t.Nickname & "]は" & Term("資金", t) & VB6.Format(prev_money - Money) & "を奪い取られた。;"
					Else
						msg = msg & "[" & t.Nickname & "]は" & Term("資金", t) & VB6.Format(prev_money - Money) & "を盗まれた。;"
					End If
					critical_type = critical_type & " 盗み"
					CauseEffect = True
				ElseIf Dice(8) = 1 And t.IsFeatureAvailable("レアアイテム所有") Then 
					'レアアイテムを盗んだ場合
					iname = t.FeatureData("レアアイテム所有")
					If IDList.IsDefined(iname) Then
						IList.Add(iname)
						If Weapon(w).Power > 0 Then
							msg = msg & "[" & t.Nickname & "]から" & IDList.Item(iname).Nickname & "を奪い取った。;"
						Else
							msg = msg & "[" & t.Nickname & "]から" & IDList.Item(iname).Nickname & "を盗んだ。;"
						End If
					Else
						ErrorMessage(t.Name & "の所有アイテム「" & iname & "」のデータが見つかりません")
					End If
					critical_type = critical_type & " 盗み"
					CauseEffect = True
				ElseIf t.IsFeatureAvailable("アイテム所有") Then 
					'アイテムを盗んだ場合
					iname = t.FeatureData("アイテム所有")
					If IDList.IsDefined(iname) Then
						IList.Add(iname)
						If Weapon(w).Power > 0 Then
							msg = msg & "[" & t.Nickname & "]から" & IDList.Item(iname).Nickname & "を奪い取った。;"
						Else
							msg = msg & "[" & t.Nickname & "]から" & IDList.Item(iname).Nickname & "を盗んだ。;"
						End If
					Else
						ErrorMessage(t.Name & "の所有アイテム「" & iname & "」のデータが見つかりません")
					End If
					critical_type = critical_type & " 盗み"
					CauseEffect = True
				ElseIf t.Value > 0 Then 
					'資金を盗んだ場合
					IncrMoney(t.Value \ 4)
					If Weapon(w).Power > 0 Then
						msg = msg & "[" & t.Nickname & "]から" & Term("資金", t) & VB6.Format(t.Value \ 4) & "を奪い取った。;"
					Else
						msg = msg & "[" & t.Nickname & "]から" & Term("資金", t) & VB6.Format(t.Value \ 4) & "を盗んだ。;"
					End If
					critical_type = critical_type & " 盗み"
					CauseEffect = True
				End If
				
				'一度盗んだユニットからは再度盗むことは出来ない
				If t.Party0 <> "味方" Then
					t.AddCondition("すかんぴん", -1, 0, "非表示")
				End If
			End If
		End If
		
		'ラーニング
		Dim sname, stype, vname As String
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "習") And t.IsFeatureAvailable("ラーニング可能技") And Party0 = "味方" Then
				stype = LIndex(t.FeatureData("ラーニング可能技"), 1)
				Select Case LIndex(t.FeatureData("ラーニング可能技"), 2)
					Case "表示", ""
						sname = stype
					Case Else
						sname = LIndex(t.FeatureData("ラーニング可能技"), 2)
				End Select
				If Not MainPilot.IsSkillAvailable(stype) Then
					msg = msg & "[" & MainPilot.Nickname & "]は「" & sname & "」を習得した。;"
					
					vname = "Ability(" & MainPilot.ID & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						GlobalVariableList.Item(vname).StringValue = stype
					Else
						With GlobalVariableList.Item(vname)
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							.StringValue = .StringValue & " " & stype
						End With
					End If
					
					vname = "Ability(" & MainPilot.ID & "," & stype & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
					End If
					
					If LLength(t.FeatureData("ラーニング可能技")) = 1 Then
						SetVariableAsString(vname, "-1 非表示")
					ElseIf stype <> sname Then 
						SetVariableAsString(vname, "-1 " & sname)
					Else
						SetVariableAsString(vname, "-1")
					End If
					
					critical_type = critical_type & " ラーニング"
					CauseEffect = True
				End If
			End If
		End If
	End Function
	
	'吹き飛ばしチェック
	Public Function CheckBlowAttack(ByVal w As Short, ByRef t As Unit, ByRef dmg As Integer, ByRef msg As String, ByRef attack_mode As String, ByRef def_mode As String, ByRef critical_type As String) As Boolean
		Dim tx, ty As Short
		Dim sx, sy As Short
		Dim nx, ny As Short
		Dim dx, dy As Short
		Dim is_crashed As Boolean
		Dim t2 As Unit
		Dim dmg2, orig_dmg As Integer
		Dim wlevel As Short
		Dim i, prob As Short
		Dim is_critical As Boolean
		Dim td As TerrainData
		
		'特殊効果無効？
		If IsWeaponClassifiedAs(w, "吹") And t.SpecialEffectImmune("吹") Then
			Exit Function
		End If
		If IsWeaponClassifiedAs(w, "Ｋ") And t.SpecialEffectImmune("Ｋ") Then
			Exit Function
		End If
		
		wlevel = MaxLng(WeaponLevel(w, "吹"), WeaponLevel(w, "Ｋ"))
		
		'特殊効果発生確率
		If IsUnderSpecialPowerEffect("特殊効果発動") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'吹き飛ばし距離の算出
		If prob >= Dice(100) Then
			wlevel = wlevel + 1
			is_critical = True
		End If
		
		'吹き飛ばし距離が０であればここで終わり
		If wlevel = 0 Then
			Exit Function
		End If
		
		'サイズによる制限
		If t.Size = "XL" Then
			Exit Function
		End If
		If IsWeaponClassifiedAs(w, "Ｋ") Then
			Select Case Size
				Case "SS"
					If t.Size <> "SS" And t.Size <> "S" Then
						Exit Function
					End If
				Case "S"
					If t.Size = "L" Or t.Size = "LL" Then
						Exit Function
					End If
				Case "M"
					If t.Size = "LL" Then
						Exit Function
					End If
			End Select
		End If
		
		'固定物は動かせない
		If t.IsFeatureAvailable("地形ユニット") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'自分自身は吹き飛ばせない
		If t Is Me Then
			Exit Function
		End If
		
		'吹き飛ばしの中心座標を設定
		If WeaponLevel(w, "Ｍ投") > 0 Then
			sx = SelectedX
			sy = SelectedY
		Else
			sx = x
			sy = y
		End If
		
		'吹き飛ばされる場所を設定
		tx = t.x
		ty = t.y
		If Not IsWeaponClassifiedAs(w, "Ｍ移") Then
			If System.Math.Abs(sx - tx) > System.Math.Abs(sy - ty) Then
				If sx > tx Then
					dx = -1
				Else
					dx = 1
				End If
			ElseIf System.Math.Abs(sx - tx) < System.Math.Abs(sy - ty) Then 
				If sy > ty Then
					dy = -1
				Else
					dy = 1
				End If
			Else
				If Dice(2) = 1 Then
					If sx > tx Then
						dx = -1
					Else
						dx = 1
					End If
				Else
					If sy > ty Then
						dy = -1
					Else
						dy = 1
					End If
				End If
			End If
		Else
			'Ｍ移の場合は横に弾き飛ばす形になる
			If System.Math.Abs(sx - tx) > System.Math.Abs(sy - ty) Then
				If Dice(2) = 1 Then
					dy = 1
				Else
					dy = -1
				End If
			ElseIf System.Math.Abs(sx - tx) < System.Math.Abs(sy - ty) Then 
				If Dice(2) = 1 Then
					dx = 1
				Else
					dx = -1
				End If
			ElseIf sx = tx And sx = ty Then 
				Select Case Dice(4)
					Case 1
						dx = -1
					Case 2
						dx = 1
					Case 3
						dy = -1
					Case 4
						dy = 1
				End Select
			Else
				If Dice(2) = 1 Then
					If sx > tx Then
						dx = 1
					Else
						dx = -1
					End If
				Else
					If sy > ty Then
						dy = 1
					Else
						dy = -1
					End If
				End If
			End If
		End If
		
		'吹き飛ばし後の位置の計算と、衝突の判定
		nx = tx
		ny = ty
		i = 1
		Do While i <= wlevel
			nx = nx + dx
			ny = ny + dy
			
			'吹き飛ばしコストに地形効果【摩擦】の補正を加える
			'MOD START 240a
			'        Set td = TDList.Item(MapData(X, Y, 0))
			Select Case MapData(x, y, Map.MapDataIndex.BoxType)
				Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.TerrainType))
				Case Else
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.LayerType))
			End Select
			'MOD START 240a
			With td
				If (t.Area = "地上" And (.Class_Renamed = "陸" Or .Class_Renamed = "屋内" Or .Class_Renamed = "月面")) Or (t.Area = "水中" And (.Class_Renamed = "水" Or .Class_Renamed = "深水")) Or t.Area = Class_Renamed Then
					If .IsFeatureAvailable("摩擦") Then
						i = i + .FeatureLevel("摩擦")
					End If
				End If
			End With
			
			'マップ端
			If nx < 1 Or MapWidth < nx Or ny < 1 Or MapHeight < ny Then
				nx = nx - dx
				ny = ny - dy
				Exit Do
			End If
			
			'進入不能？
			If Not t.IsAbleToEnter(nx, ny) Or Not MapDataForUnit(nx, ny) Is Nothing Then
				is_crashed = True
				
				If Not MapDataForUnit(nx, ny) Is Nothing Then
					t2 = MapDataForUnit(nx, ny)
				End If
				
				nx = nx - dx
				ny = ny - dy
				Exit Do
			End If
			
			'障害物あり？
			If t.Area <> "空中" Then
				If TerrainHasObstacle(nx, ny) Then
					is_crashed = True
				End If
			End If
			
			i = i + 1
		Loop 
		
		'ユニットを強制移動
		If tx <> nx Or ty <> ny Then
			EraseUnitBitmap(tx, ty)
			If IsAnimationDefined("吹き飛ばし") Then
				PlayAnimation("吹き飛ばし")
			Else
				MoveUnitBitmap(t, tx, ty, nx, ny, 20)
			End If
			t.Jump(nx, ny, False)
		End If
		
		'激突
		orig_dmg = dmg
		If is_crashed Then
			dmg = orig_dmg + MaxLng((orig_dmg - t.Armor * t.MainPilot.Morale \ 100) \ 2, 0)
			
			'最低ダメージ
			If def_mode = "防御" Then
				dmg = MaxLng(dmg, 5)
			Else
				dmg = MaxLng(dmg, 10)
			End If
			
			PlayWave("Crash.wav")
		End If
		
		'巻き添え
		If Not t2 Is Nothing And Not t2 Is t Then
			With t2
				dmg2 = (orig_dmg - .Armor * .MainPilot.Morale \ 100) \ 2
				
				'最低ダメージ
				If dmg2 < 10 Then
					dmg2 = 10
				End If
				
				'無敵の場合はダメージを受けない
				If .IsConditionSatisfied("無敵") Then
					dmg2 = 0
				End If
				
				'各種処理がややこしくなるので巻き添えではユニットを破壊しない
				If .HP - dmg2 < 10 Then
					dmg2 = .HP - 10
				End If
				
				'ダメージ適用
				If dmg2 > 0 Then
					.HP = .HP - dmg2
				Else
					dmg2 = 0
				End If
				
				'ダメージ量表示
				If Not IsOptionDefined("ダメージ表示無効") Or attack_mode = "マップ攻撃" Then
					DrawSysString(.x, .y, VB6.Format(dmg2), True)
				End If
				
				'特殊能力「不安定」による暴走チェック
				If .IsFeatureAvailable("不安定") Then
					If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("暴走") Then
						.AddCondition("暴走", -1)
						.Update()
					End If
				End If
				
				'ダメージを受ければ眠りからさめる
				If .IsConditionSatisfied("睡眠") And Not IsWeaponClassifiedAs(w, "眠") Then
					.DeleteCondition("睡眠")
				End If
			End With
		End If
		
		msg = t.Nickname & "を吹き飛ばした。;" & msg
		If is_critical Then
			msg = "クリティカル！ " & msg
		End If
		
		'吹き飛ばしが発生したことを伝える
		critical_type = critical_type & " 吹き飛ばし"
		CheckBlowAttack = True
	End Function
	
	'引き寄せチェック
	Public Function CheckDrawAttack(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef def_mode As String, ByRef critical_type As String) As Boolean
		Dim tx, ty As Short
		Dim sx, sy As Short
		Dim nx, ny As Short
		Dim prob As Short
		
		'特殊効果無効？
		If t.SpecialEffectImmune("引") Then
			Exit Function
		End If
		
		'既に隣接している？
		If System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
			Exit Function
		End If
		
		'サイズによる制限
		If t.Size = "XL" Then
			Exit Function
		End If
		
		'固定物は動かせない
		If t.IsFeatureAvailable("地形ユニット") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'自分自身は引き寄せない
		If t Is Me Then
			Exit Function
		End If
		
		'特殊効果発生確率
		If IsUnderSpecialPowerEffect("特殊効果発動") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'引き寄せ発生？
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'引き寄せの中心座標を設定
		If WeaponLevel(w, "Ｍ投") > 0 Then
			sx = SelectedX
			sy = SelectedY
		Else
			sx = x
			sy = y
		End If
		
		'ターゲットの座標
		tx = t.x
		ty = t.y
		
		'既に引き寄せの中心位置にいる？
		If sx = tx And sy = ty Then
			Exit Function
		End If
		
		'引き寄せられる場所を設定
		If MapDataForUnit(sx, sy) Is Nothing Then
			nx = sx
			ny = sy
		ElseIf System.Math.Abs(sx - tx) > System.Math.Abs(sy - ty) Then 
			If sx > tx Then
				nx = sx - 1
			Else
				nx = sx + 1
			End If
			ny = y
			
			If Not MapDataForUnit(nx, ny) Is Nothing Then
				If sy <> ty Then
					If sy > ty Then
						If MapDataForUnit(sx, sy - 1) Is Nothing Then
							nx = sx
							ny = sy - 1
						ElseIf MapDataForUnit(nx, sy - 1) Is Nothing Then 
							ny = sy - 1
						End If
					Else
						If MapDataForUnit(sx, sy + 1) Is Nothing Then
							nx = sx
							ny = sy + 1
						ElseIf MapDataForUnit(nx, sy + 1) Is Nothing Then 
							ny = sy + 1
						End If
					End If
				End If
			End If
		ElseIf System.Math.Abs(sx - tx) < System.Math.Abs(sy - ty) Then 
			nx = sx
			If sy > ty Then
				ny = sy - 1
			Else
				ny = sy + 1
			End If
			
			If Not MapDataForUnit(nx, ny) Is Nothing Then
				If sx <> tx Then
					If sx > tx Then
						If MapDataForUnit(sx - 1, sy) Is Nothing Then
							nx = sx - 1
							ny = sy
						ElseIf MapDataForUnit(sx - 1, ny) Is Nothing Then 
							nx = sx - 1
						End If
					Else
						If MapDataForUnit(sx + 1, sy) Is Nothing Then
							nx = sx + 1
							ny = sy
						ElseIf MapDataForUnit(sx + 1, ny) Is Nothing Then 
							nx = sx + 1
						End If
					End If
				End If
			End If
		Else
			If Dice(2) = 1 Then
				If sx > tx Then
					nx = sx - 1
				Else
					nx = sx + 1
				End If
				ny = sy
				
				If Not MapDataForUnit(nx, ny) Is Nothing Then
					nx = sx
					If sy > ty Then
						ny = sy - 1
					Else
						ny = sy + 1
					End If
				End If
			Else
				nx = sx
				If sy > ty Then
					ny = sy - 1
				Else
					ny = sy + 1
				End If
				
				If Not MapDataForUnit(nx, ny) Is Nothing Then
					If sx > tx Then
						nx = sx - 1
					Else
						nx = sx + 1
					End If
					ny = sy
				End If
			End If
			
			If Not MapDataForUnit(nx, ny) Is Nothing Then
				If sx > tx Then
					nx = sx - 1
				Else
					nx = sx + 1
				End If
				If sy > ty Then
					ny = sy - 1
				Else
					ny = sy + 1
				End If
			End If
		End If
		
		'結局動いてない？
		If nx = tx And ny = ty Then
			Exit Function
		End If
		
		'ユニットを強制移動
		t.Jump(nx, ny)
		
		'本当に動いた？
		If t.x = tx And t.y = ty Then
			Exit Function
		End If
		
		msg = t.Nickname & "を引き寄せた。;" & msg
		
		'引き寄せが発生したことを伝える
		critical_type = critical_type & " 引き寄せ"
		CheckDrawAttack = True
	End Function
	
	'強制転移チェック
	Public Function CheckTeleportAwayAttack(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef def_mode As String, ByRef critical_type As String) As Boolean
		Dim tx, ty As Short
		Dim nx, ny As Short
		Dim d, prob, i As Short
		
		'特殊効果無効？
		If t.SpecialEffectImmune("転") Then
			Exit Function
		End If
		
		'サイズによる制限
		If t.Size = "XL" Then
			Exit Function
		End If
		
		'固定物は動かせない
		If t.IsFeatureAvailable("地形ユニット") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'自分自身は強制転移出来ない
		If t Is Me Then
			Exit Function
		End If
		
		'特殊効果発生確率
		If IsUnderSpecialPowerEffect("特殊効果発動") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'強制転移発生？
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'強制転移先を設定
		tx = t.x
		ty = t.y
		For i = 1 To 10
			d = Dice(WeaponLevel(w, "転"))
			If Dice(2) = 1 Then
				nx = tx + d
			Else
				nx = tx - d
			End If
			
			d = WeaponLevel(w, "転") - d
			If Dice(2) = 1 Then
				ny = ty + d
			Else
				ny = ty - d
			End If
			
			If 1 <= nx And nx <= MapWidth And 1 <= ny And ny <= MapHeight Then
				Exit For
			End If
		Next 
		
		'転院先がない？
		If i > 10 Then
			Exit Function
		End If
		
		'ユニットを強制移動
		t.Jump(nx, ny)
		
		'本当に動いた？
		If t.x = tx And t.y = ty Then
			Exit Function
		End If
		
		msg = t.Nickname & "をテレポートさせた。;" & msg
		
		'強制転移が発生したことを伝える
		critical_type = critical_type & " 強制転移"
		CheckTeleportAwayAttack = True
	End Function
	
	'能力コピーチェック
	Public Function CheckMetamorphAttack(ByVal w As Short, ByRef t As Unit, ByRef def_mode As String) As Boolean
		Dim prob, wlv As Short
		Dim uname As String
		
		'既にコピー済み？
		If IsFeatureAvailable("ノーマルモード") Then
			Exit Function
		End If
		
		'特殊効果無効？
		If t.SpecialEffectImmune("写") Then
			Exit Function
		End If
		
		'ボスユニットはコピー出来ない
		If t.BossRank >= 0 Then
			Exit Function
		End If
		
		'自分自身はコピー出来ない
		If t Is Me Then
			Exit Function
		End If
		
		'サイズ制限
		If IsWeaponClassifiedAs(w, "写") Then
			Select Case Size
				Case "SS"
					Select Case t.Size
						Case "M", "L", "LL", "XL"
							Exit Function
					End Select
				Case "S"
					Select Case t.Size
						Case "L", "LL", "XL"
							Exit Function
					End Select
				Case "M"
					Select Case t.Size
						Case "SS", "LL", "XL"
							Exit Function
					End Select
				Case "L"
					Select Case t.Size
						Case "SS", "S", "XL"
							Exit Function
					End Select
				Case "LL"
					Select Case t.Size
						Case "SS", "S", "M"
							Exit Function
					End Select
				Case "XL"
					Select Case t.Size
						Case "SS", "S", "M", "L"
							Exit Function
					End Select
			End Select
		End If
		
		'特殊効果発生確率
		If IsUnderSpecialPowerEffect("特殊効果発動") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'コピー成功？
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'コピーしてしまうとその場にいれなくなってしまう？
		If Not OtherForm((t.Name)).IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'変身前に情報を記録しておく
		uname = Nickname
		wlv = MaxLng(WeaponLevel(w, "写"), WeaponLevel(w, "化"))
		
		'変身
		Transform((t.Name))
		
		With CurrentForm
			'元に戻れるように設定
			If wlv > 0 Then
				.AddCondition("残り時間", wlv)
			End If
			.AddCondition("ノーマルモード付加", -1, 1, Name & " 手動解除可")
			.AddCondition("能力コピー", -1)
			
			'コピー元のパイロット画像とメッセージを使うように設定
			.AddCondition("パイロット画像", -1, 0, "非表示 " & t.MainPilot.Bitmap)
			.AddCondition("メッセージ", -1, 0, "非表示 " & t.MainPilot.MessageType)
		End With
		
		DisplaySysMessage(uname & "は" & t.Nickname & "に変身した。")
		
		'能力コピーが発生したことを伝える
		CheckMetamorphAttack = True
	End Function
	
	
	'マップ攻撃 w で (tx,ty) を攻撃
	Public Sub MapAttack(ByVal w As Short, ByVal tx As Short, ByVal ty As Short, Optional ByVal is_event As Boolean = False)
		Dim k, i, j, num As Short
		Dim t, u As Unit
		Dim prev_level As Short
		Dim earned_exp As Integer
		Dim prev_money, earnings As Integer
		Dim prev_stype() As String
		Dim prev_sname() As String
		Dim prev_slevel() As Double
		Dim sname As String
		Dim prev_special_power() As String
		Dim msg As String
		Dim partners() As Unit
		Dim wname, wnickname As String
		Dim targets() As Unit
		Dim targets_hp_ratio() As Double
		Dim targets_x() As Short
		Dim targets_y() As Short
		Dim rx, ry As Short
		Dim min_range, max_range As Short
		Dim uname, fname As String
		Dim hp_ratio, en_ratio As Double
		Dim itm As Item
		
		wname = Weapon(w).Name
		SelectedWeaponName = wname
		wnickname = WeaponNickname(w)
		
		'効果範囲を設定
		min_range = Weapon(w).MinRange
		max_range = WeaponMaxRange(w)
		If IsWeaponClassifiedAs(w, "Ｍ直") Then
			If ty < y Then
				AreaInLine(x, y, min_range, max_range, "N")
			ElseIf ty > y Then 
				AreaInLine(x, y, min_range, max_range, "S")
			ElseIf tx < x Then 
				AreaInLine(x, y, min_range, max_range, "W")
			Else
				AreaInLine(x, y, min_range, max_range, "E")
			End If
		ElseIf IsWeaponClassifiedAs(w, "Ｍ拡") Then 
			If ty < y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then
				AreaInCone(x, y, min_range, max_range, "N")
			ElseIf ty > y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then 
				AreaInCone(x, y, min_range, max_range, "S")
			ElseIf tx < x And System.Math.Abs(x - tx) > System.Math.Abs(y - ty) Then 
				AreaInCone(x, y, min_range, max_range, "W")
			Else
				AreaInCone(x, y, min_range, max_range, "E")
			End If
		ElseIf IsWeaponClassifiedAs(w, "Ｍ扇") Then 
			If ty < y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then
				AreaInSector(x, y, min_range, max_range, "N", WeaponLevel(w, "Ｍ扇"))
			ElseIf ty > y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then 
				AreaInSector(x, y, min_range, max_range, "S", WeaponLevel(w, "Ｍ扇"))
			ElseIf tx < x And System.Math.Abs(x - tx) >= System.Math.Abs(y - ty) Then 
				AreaInSector(x, y, min_range, max_range, "W", WeaponLevel(w, "Ｍ扇"))
			Else
				AreaInSector(x, y, min_range, max_range, "E", WeaponLevel(w, "Ｍ扇"))
			End If
		ElseIf IsWeaponClassifiedAs(w, "Ｍ投") Then 
			AreaInRange(tx, ty, WeaponLevel(w, "Ｍ投"), 1, "すべて")
		ElseIf IsWeaponClassifiedAs(w, "Ｍ全") Then 
			AreaInRange(x, y, max_range, min_range, "すべて")
		ElseIf IsWeaponClassifiedAs(w, "Ｍ移") Or IsWeaponClassifiedAs(w, "Ｍ線") Then 
			AreaInPointToPoint(x, y, tx, ty)
		End If
		MaskData(x, y) = False
		
		'識別型マップ攻撃
		If IsWeaponClassifiedAs(w, "識") Or IsUnderSpecialPowerEffect("識別攻撃") Then
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Then
						If IsAlly(u) Or WeaponAdaption(w, .Area) = 0 Then
							MaskData(.x, .y) = True
						End If
					End If
				End With
			Next u
			MaskData(x, y) = False
		End If
		
		'合体技の処理
		Dim TmpMaskData() As Boolean
		If IsWeaponClassifiedAs(w, "合") Then
			
			'合体技のパートナーのハイライト表示
			'MaskDataを保存して使用している
			ReDim TmpMaskData(MapWidth, MapHeight)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					TmpMaskData(i, j) = MaskData(i, j)
				Next 
			Next 
			
			CombinationPartner("武装", w, partners)
			
			For i = 1 To UBound(partners)
				With partners(i)
					MaskData(.x, .y) = False
					TmpMaskData(.x, .y) = True
				End With
			Next 
			
			MaskScreen()
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = TmpMaskData(i, j)
				Next 
			Next 
		Else
			ReDim partners(0)
			ReDim SelectedPartners(0)
			MaskScreen()
		End If
		
		'自分自身には攻撃しない
		MaskData(x, y) = True
		
		'マップ攻撃の影響を受けるユニットのリストを作成
		ReDim targets(0)
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'マップ攻撃の影響をうけるかチェック
				If MaskData(i, j) Then
					GoTo NextLoop
				End If
				If MapDataForUnit(i, j) Is Nothing Then
					GoTo NextLoop
				End If
				
				t = MapDataForUnit(i, j)
				With t
					If WeaponAdaption(w, .Area) = 0 Then
						GoTo NextLoop
					End If
					If IsWeaponClassifiedAs(w, "識") Or IsUnderSpecialPowerEffect("識別攻撃") Then
						If IsAlly(t) Then
							GoTo NextLoop
						End If
					End If
					If .IsUnderSpecialPowerEffect("隠れ身") Then
						GoTo NextLoop
					End If
					
					ReDim Preserve targets(UBound(targets) + 1)
					targets(UBound(targets)) = t
				End With
NextLoop: 
			Next 
		Next 
		
		'攻撃の起点を設定
		If IsWeaponClassifiedAs(w, "Ｍ投") Then
			rx = tx
			ry = ty
		Else
			rx = x
			ry = y
		End If
		
		'起点からの距離に応じて並べ替え
		Dim min_item, min_value As Short
		For i = 1 To UBound(targets) - 1
			
			min_item = i
			With targets(i)
				min_value = System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry)
			End With
			For j = i + 1 To UBound(targets)
				With targets(j)
					If System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry) < min_value Then
						min_item = j
						min_value = System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry)
					End If
				End With
			Next 
			If min_item <> i Then
				u = targets(i)
				targets(i) = targets(min_item)
				targets(min_item) = u
			End If
		Next 
		
		'戦闘前に一旦クリア
		'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit2 = Nothing
		
		'イベントの処理
		If Not is_event Then
			'使用イベント
			HandleEvent("使用", MainPilot.ID, wname)
			If IsScenarioFinished Then
				IsScenarioFinished = False
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				Exit Sub
			End If
			
			'マップ攻撃開始前にあらかじめ攻撃イベントを発生させる
			For i = 1 To UBound(targets)
				t = targets(i)
				SaveSelections()
				SelectedTarget = t
				HandleEvent("攻撃", MainPilot.ID, t.MainPilot.ID)
				RestoreSelections()
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
		End If
		
		'まだ攻撃可能？
		If Not is_event Then
			If Status_Renamed <> "出撃" Or MaxAction(True) = 0 Or IsConditionSatisfied("攻撃不能") Then
				Exit Sub
			End If
		End If
		
		'ターゲットに関する情報を記録
		ReDim targets_hp_ratio(UBound(targets))
		ReDim targets_x(UBound(targets))
		ReDim targets_y(UBound(targets))
		For i = 1 To UBound(targets)
			t = targets(i).CurrentForm
			targets(i) = t
			With t
				targets_hp_ratio(i) = .HP / .MaxHP
				targets_x(i) = .x
				targets_y(i) = .y
			End With
		Next 
		
		OpenMessageForm(Me)
		
		'現在の選択状況をセーブ
		SaveSelections()
		
		'選択内容を切り替え
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedWeapon = w
		SelectedX = tx
		SelectedY = ty
		
		'変な「対〜」メッセージが表示されないようにターゲットをオフ
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTargetForEvent = Nothing
		
		'攻撃準備の効果音
		If IsAnimationDefined(wname & "(準備)") Then
			PlayAnimation(wname & "(準備)")
		ElseIf IsAnimationDefined(wname) And Not IsOptionDefined("武器準備アニメ非表示") And WeaponAnimation Then 
			PlayAnimation(wname & "(準備)")
		ElseIf IsSpecialEffectDefined(wname & "(準備)") Then 
			SpecialEffect(wname & "(準備)")
		End If
		
		'マップ攻撃攻撃開始のメッセージ
		If IsMessageDefined(wname) Then
			If IsMessageDefined("かけ声(" & wname & ")") Then
				PilotMessage("かけ声(" & wname & ")")
			Else
				PilotMessage("かけ声")
			End If
		End If
		
		'攻撃メッセージ
		PilotMessage(wname, "攻撃")
		
		'戦闘アニメ or 効果音
		If IsAnimationDefined(wname & "(攻撃)") Or IsAnimationDefined(wname) Then
			PlayAnimation(wname & "(攻撃)")
		ElseIf IsSpecialEffectDefined(wname) Then 
			SpecialEffect(wname)
		Else
			AttackEffect(Me, w)
		End If
		
		'攻撃中の攻撃力変動を避けるため、あらかじめ攻撃力を保存しておく
		SelectedMapAttackPower = 0
		SelectedMapAttackPower = WeaponPower(w, "初期値")
		
		'「永」属性武器が破壊されることによるマップ攻撃キャンセル処理の初期化
		IsMapAttackCanceled = False
		
		'武器使用による弾数＆ＥＮ消費
		UseWeapon(w)
		UpdateMessageForm(Me)
		
		'攻撃時のシステムメッセージ
		If IsSysMessageDefined(wname) Then
			'「武器名(解説)」のメッセージを使用
			SysMessage(wname)
		ElseIf IsSysMessageDefined("攻撃") Then 
			'「攻撃(解説)」のメッセージを使用
			SysMessage(wname)
		Else
			Select Case UBound(partners)
				Case 0
					'通常攻撃
					msg = Nickname & "は"
				Case 1
					'２体合体攻撃
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "と[" & partners(1).MainPilot.Nickname & "]の[" & Nickname & "]は"
					Else
						msg = Nickname & "達は"
					End If
				Case 2
					'３体合体攻撃
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]、[" & partners(2).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "、[" & partners(1).MainPilot.Nickname & "]、[" & partners(2).MainPilot.Nickname & "]の[" & Nickname & "]は"
					Else
						msg = Nickname & "達は"
					End If
				Case Else
					'３体以上による合体攻撃
					msg = Nickname & "達は"
			End Select
			
			'攻撃の種類によってメッセージを切り替え
			If Right(wnickname, 2) = "攻撃" Or Right(wnickname, 4) = "アタック" Or wnickname = "突撃" Then
				msg = msg & "[" & wnickname & "]をかけた。"
			ElseIf IsSpellWeapon(w) Then 
				If Right(wnickname, 2) = "呪文" Then
					msg = msg & "[" & wnickname & "]を唱えた。"
				ElseIf Right(wnickname, 2) = "の杖" Then 
					msg = msg & "[" & Left(wnickname, Len(wnickname) - 2) & "]の呪文を唱えた。"
				Else
					msg = msg & "[" & wnickname & "]の呪文を唱えた。"
				End If
			ElseIf IsWeaponClassifiedAs(w, "実") And (InStr(wnickname, "ミサイル") > 0 Or InStr(wnickname, "ロケット") > 0) Then 
				msg = msg & "[" & wnickname & "]を発射した。"
			ElseIf Right(wnickname, 1) = "息" Or Right(wnickname, 3) = "ブレス" Or Right(wnickname, 2) = "光線" Or Right(wnickname, 1) = "光" Or Right(wnickname, 3) = "ビーム" Or Right(wnickname, 4) = "レーザー" Then 
				msg = msg & "[" & wnickname & "]を放った。"
			ElseIf Right(wnickname, 1) = "歌" Then 
				msg = msg & "[" & wnickname & "]を歌った。"
			ElseIf Right(wnickname, 2) = "踊り" Then 
				msg = msg & "[" & wnickname & "]を踊った。"
			Else
				msg = msg & "[" & wnickname & "]で攻撃をかけた。"
			End If
			
			'メッセージを表示
			DisplaySysMessage(msg)
		End If
		
		'命中後メッセージ
		PilotMessage(wname & "(命中)")
		
		'選択状況を復元
		RestoreSelections()
		
		'経験値＆資金獲得のメッセージ表示用に各種データを保存
		With MainPilot
			prev_level = .Level
			ReDim prev_special_power(.CountSpecialPower)
			For i = 1 To .CountSpecialPower
				prev_special_power(i) = .SpecialPower(i)
			Next 
			ReDim prev_stype(.CountSkill)
			ReDim prev_slevel(.CountSkill)
			ReDim prev_sname(.CountSkill)
			For i = 1 To .CountSkill
				prev_stype(i) = .Skill(i)
				prev_slevel(i) = .SkillLevel(i, "基本値")
				prev_sname(i) = .SkillName(i)
			Next 
		End With
		prev_money = Money
		
		'攻撃元ユニットは SelectedTarget に設定していないといけない
		SelectedTarget = Me
		
		'移動型マップ攻撃による移動を行う
		If IsWeaponClassifiedAs(w, "Ｍ移") Then
			Jump(tx, ty)
		End If
		
		'個々のユニットに対して攻撃
		For i = 1 To UBound(targets)
			t = targets(i).CurrentForm
			If t.Status_Renamed = "出撃" Then
				If Party = "味方" Or Party = "ＮＰＣ" Then
					UpdateMessageForm(t, Me)
				Else
					UpdateMessageForm(Me, t)
				End If
				
				'攻撃を行う
				Attack(w, t, "マップ攻撃", "", is_event)
				
				'かばうによりターゲットが変化している？
				If Not SupportGuardUnit Is Nothing Then
					targets(i) = SupportGuardUnit.CurrentForm
					targets_hp_ratio(i) = SupportGuardUnitHPRatio
					targets_x(i) = targets(i).x
					targets_y(i) = targets(i).y
				End If
				
				'これ以上攻撃を続けられない場合
				If Status_Renamed <> "出撃" Or CountPilot = 0 Or IsMapAttackCanceled Then
					CloseMessageForm()
					SelectedMapAttackPower = 0
					GoTo DoEvent
				End If
				
				ClearMessageForm()
			End If
		Next 
		
		'とどめメッセージ
		If IsMessageDefined(wname & "(とどめ)") Then
			PilotMessage(wname & "(とどめ)")
		End If
		If IsAnimationDefined(wname & "(とどめ)") Then
			PlayAnimation(wname & "(とどめ)")
		ElseIf IsSpecialEffectDefined(wname & "(とどめ)") Then 
			SpecialEffect(wname & "(とどめ)")
		End If
		
		'カットインは消去しておく
		If IsPictureVisible Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
		End If
		
		'保存した攻撃力の使用を止める
		SelectedMapAttackPower = 0
		
		' ADD START MARGE
		'戦闘アニメ終了処理
		If IsAnimationDefined(wname & "(終了)") Then
			PlayAnimation(wname & "(終了)")
		ElseIf IsAnimationDefined("終了") Then 
			PlayAnimation("終了")
		End If
		' ADD END MARGE
		
		'戦闘アニメで変更されたユニット画像を元に戻す
		If IsConditionSatisfied("ユニット画像") Then
			DeleteCondition("ユニット画像")
			BitmapID = MakeUnitBitmap(Me)
			If IsPictureVisible Then
				PaintUnitBitmap(Me, "リフレッシュ無し")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		If IsConditionSatisfied("非表示付加") Then
			DeleteCondition("非表示付加")
			BitmapID = MakeUnitBitmap(Me)
			If IsPictureVisible Then
				PaintUnitBitmap(Me, "リフレッシュ無し")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("ユニット画像") Then
					.DeleteCondition("ユニット画像")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("非表示付加") Then
					.DeleteCondition("非表示付加")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
		
		If Party = "味方" And Not is_event Then
			'経験値＆資金の獲得
			For i = 1 To UBound(targets)
				t = targets(i).CurrentForm
				If Not IsEnemy(t) Then
					'味方からは経験値＆資金は得られない
				ElseIf t.Status_Renamed = "破壊" Then 
					'経験値の獲得
					earned_exp = earned_exp + GetExp(t, "破壊", "マップ")
					
					'合体技のパートナーへの経験値
					If Not IsOptionDefined("合体技パートナー経験値無効") Then
						For j = 1 To UBound(partners)
							partners(j).CurrentForm.GetExp(t, "破壊", "パートナー")
						Next 
					End If
					
					'獲得する資金を算出
					earnings = t.Value \ 2
					
					'スペシャルパワーによる獲得資金増加
					If IsUnderSpecialPowerEffect("獲得資金増加") Then
						earnings = MinDbl(earnings * (1 + 0.1 * SpecialPowerEffectLevel("獲得資金増加")), 999999999)
					End If
					
					'パイロット能力による獲得資金増加
					If IsSkillAvailable("資金獲得") Then
						If Not IsUnderSpecialPowerEffect("獲得資金増加") Or IsOptionDefined("収得効果重複") Then
							earnings = MinDbl(earnings * ((10 + SkillLevel("資金獲得", 5)) / 10), 999999999)
						End If
					End If
					
					'資金を獲得
					IncrMoney(earnings)
				Else
					'経験値の獲得
					earned_exp = earned_exp + GetExp(t, "攻撃", "マップ")
					
					'合体技のパートナーへの経験値
					If Not IsOptionDefined("合体技パートナー経験値無効") Then
						For j = 1 To UBound(partners)
							partners(j).CurrentForm.GetExp(t, "攻撃", "パートナー")
						Next 
					End If
				End If
			Next 
			
			'獲得した経験値＆資金の表示
			If Money > prev_money Then
				DisplaySysMessage(VB6.Format(Money - prev_money) & "の" & Term("資金", t) & "を得た。")
			End If
			With MainPilot
				'レベルアップの処理
				If .Level > prev_level Then
					If IsAnimationDefined("レベルアップ") Then
						PlayAnimation("レベルアップ")
					ElseIf IsSpecialEffectDefined("レベルアップ") Then 
						SpecialEffect("レベルアップ")
					End If
					If IsMessageDefined("レベルアップ") Then
						PilotMessage("レベルアップ")
					End If
					
					msg = .Nickname & "は" & VB6.Format(earned_exp) & "の経験値を獲得し、" & "レベル[" & VB6.Format(.Level) & "]にレベルアップ。"
					
					'特殊能力の習得
					For i = 1 To .CountSkill
						sname = .Skill(i)
						If InStr(.SkillName(i), "非表示") = 0 Then
							Select Case sname
								Case "同調率", "霊力", "追加レベル", "魔力所有"
								Case Else
									For j = 1 To UBound(prev_stype)
										If sname = prev_stype(j) Then
											Exit For
										End If
									Next 
									If j > UBound(prev_stype) Then
										msg = msg & ";" & .SkillName(i) & "を習得。"
									ElseIf .SkillLevel(sname, "基本値") > prev_slevel(j) Then 
										msg = msg & ";" & prev_sname(j) & " => " & .SkillName(i) & "。"
									End If
							End Select
						End If
					Next 
					
					'スペシャルパワーの習得
					If .CountSpecialPower > UBound(prev_special_power) Then
						msg = msg & ";スペシャルパワー"
						For i = 1 To .CountSpecialPower
							sname = .SpecialPower(i)
							For j = 1 To UBound(prev_special_power)
								If sname = prev_special_power(j) Then
									Exit For
								End If
							Next 
							If j > UBound(prev_special_power) Then
								msg = msg & "「" & sname & "」"
							End If
						Next 
						msg = msg & "を習得。"
					End If
					
					DisplaySysMessage(msg)
					
					HandleEvent("レベルアップ", .ID)
					
					PList.UpdateSupportMod(Me)
				Else
					If earned_exp > 0 Then
						DisplaySysMessage(.Nickname & "は" & VB6.Format(earned_exp) & "の経験値を得た。")
					End If
				End If
			End With
		End If
		
		'スペシャルパワー効果の解除
		If IsUnderSpecialPowerEffect("攻撃後消耗") Then
			AddCondition("消耗", 1)
		End If
		RemoveSpecialPowerInEffect("攻撃")
		RemoveSpecialPowerInEffect("戦闘終了")
		If earnings > 0 Then
			RemoveSpecialPowerInEffect("敵破壊")
		End If
		For i = 1 To UBound(targets)
			targets(i).CurrentForm.RemoveSpecialPowerInEffect("戦闘終了")
		Next 
		
		'状態の解除
		For i = 1 To UBound(targets)
			targets(i).CurrentForm.UpdateCondition()
		Next 
		
		'ステルスが解ける？
		If IsFeatureAvailable("ステルス") Then
			If IsWeaponClassifiedAs(w, "忍") Then
				For i = 1 To UBound(targets)
					With targets(i).CurrentForm
						If .Status_Renamed = "出撃" And .MaxAction > 0 Then
							AddCondition("ステルス無効", 1)
							Exit For
						End If
					End With
				Next 
			Else
				AddCondition("ステルス無効", 1)
			End If
		End If
		
		'合体技のパートナーの弾数＆ＥＮを消費
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountWeapon
					If .Weapon(j).Name = wname Then
						.UseWeapon(j)
						If .IsWeaponClassifiedAs(j, "自") Then
							If .IsFeatureAvailable("パーツ分離") Then
								uname = LIndex(.FeatureData("パーツ分離"), 2)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
									With .CurrentForm
										.HP = .MaxHP
										.UsedAction = .MaxAction
									End With
								Else
									.Die()
								End If
							Else
								.Die()
							End If
						ElseIf .IsWeaponClassifiedAs(j, "失") And .HP = 0 Then 
							.Die()
						ElseIf .IsWeaponClassifiedAs(j, "変") Then 
							If .IsFeatureAvailable("変形技") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "変形技" And LIndex(.FeatureData(k), 1) = wname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("ノーマルモード") Then
										uname = LIndex(.FeatureData("ノーマルモード"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("ノーマルモード") Then 
								uname = LIndex(.FeatureData("ノーマルモード"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'同名の武器がなかった場合は自分のデータを使って処理
				If j > .CountWeapon Then
					If Weapon(w).ENConsumption > 0 Then
						.EN = .EN - WeaponENConsumption(w)
					End If
					If IsWeaponClassifiedAs(w, "消") Then
						.AddCondition("消耗", 1)
					End If
					If IsWeaponClassifiedAs(w, "Ｃ") And .IsConditionSatisfied("チャージ完了") Then
						.DeleteCondition("チャージ完了")
					End If
					If IsWeaponClassifiedAs(w, "気") Then
						.IncreaseMorale(-5 * WeaponLevel(w, "気"))
					End If
					If IsWeaponClassifiedAs(w, "霊") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "霊")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsWeaponClassifiedAs(w, "プ") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "プ")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsWeaponClassifiedAs(w, "失") Then
						.HP = MaxLng(.HP - .MaxHP * WeaponLevel(w, "失") \ 10, 0)
					End If
					If IsWeaponClassifiedAs(w, "自") Then
						If .IsFeatureAvailable("パーツ分離") Then
							uname = LIndex(.FeatureData("パーツ分離"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
							Else
								.Die()
							End If
						Else
							.Die()
						End If
					ElseIf IsWeaponClassifiedAs(w, "失") And .HP = 0 Then 
						.Die()
					ElseIf IsWeaponClassifiedAs(w, "変") Then 
						If .IsFeatureAvailable("ノーマルモード") Then
							uname = LIndex(.FeatureData("ノーマルモード"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'以下の特殊効果は武器データが変化する可能性があるため、同時には適用されない
		
		'自爆の処理
		If IsWeaponClassifiedAs(w, "自") Then
			If IsFeatureAvailable("パーツ分離") Then
				uname = LIndex(FeatureData("パーツ分離"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("パーツ分離")
					If IsSysMessageDefined("破壊時分離(" & Name & ")") Then
						SysMessage("破壊時分離(" & Name & ")")
					ElseIf IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
						SysMessage("破壊時分離(" & fname & ")")
					ElseIf IsSysMessageDefined("破壊時分離") Then 
						SysMessage("破壊時分離")
					ElseIf IsSysMessageDefined("分離(" & Name & ")") Then 
						SysMessage("分離(" & Name & ")")
					ElseIf IsSysMessageDefined("分離(" & fname & ")") Then 
						SysMessage("分離(" & fname & ")")
					ElseIf IsSysMessageDefined("分離") Then 
						SysMessage("分離")
					Else
						DisplaySysMessage(Nickname & "は破壊されたパーツを分離させた。")
					End If
				Else
					Die()
					CloseMessageForm()
					If Not is_event Then
						HandleEvent("破壊", MainPilot.ID)
						If IsScenarioFinished Then
							Exit Sub
						End If
					End If
				End If
			Else
				Die()
				CloseMessageForm()
				If Not is_event Then
					HandleEvent("破壊", MainPilot.ID)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
			End If
			
			'ＨＰ消費攻撃による自殺
		ElseIf IsWeaponClassifiedAs(w, "失") And HP = 0 Then 
			Die()
			CloseMessageForm()
			If Not is_event Then
				HandleEvent("破壊", MainPilot.ID)
				If IsScenarioFinished Then
					Exit Sub
				End If
			End If
			
			'変形技
		ElseIf IsWeaponClassifiedAs(w, "変") Then 
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = wname Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("ノーマルモード") Then
						uname = LIndex(FeatureData("ノーマルモード"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				uname = LIndex(FeatureData("ノーマルモード"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'アイテムを消費
		ElseIf Weapon(w).IsItem And Bullet(w) = 0 And MaxBullet(w) > 0 Then 
			'アイテムを削除
			num = Data.CountWeapon
			num = num + MainPilot.Data.CountWeapon
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountWeapon
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountWeapon
			Next 
			If IsFeatureAvailable("追加サポート") Then
				num = num + AdditionalSupport.Data.CountWeapon
			End If
			For	Each itm In colItem
				num = num + itm.CountWeapon
				If w <= num Then
					itm.Exist = False
					DeleteItem((itm.ID))
					Exit For
				End If
			Next itm
		End If
		
		CloseMessageForm()
		
DoEvent: 
		
		'イベント処理
		Dim uparty As String
		Dim found As Boolean
		If Not is_event Then
			For i = 1 To UBound(targets)
				t = targets(i).CurrentForm
				
				If t.Status_Renamed = "破壊" Then
					'破壊イベントを発生
					SaveSelections()
					SwapSelections()
					
					HandleEvent("マップ攻撃破壊", t.MainPilot.ID)
					
					RestoreSelections()
					If IsScenarioFinished Or IsCanceled Then
						Exit Sub
					End If
				ElseIf t.Status_Renamed = "出撃" Then 
					If t.HP / t.MaxHP < targets_hp_ratio(i) Then
						'損傷率イベント
						SaveSelections()
						SwapSelections()
						
						HandleEvent("損傷率", t.MainPilot.ID, 100 * (t.MaxHP - t.HP) \ t.MaxHP)
						
						RestoreSelections()
						If IsScenarioFinished Or IsCanceled Then
							Exit Sub
						End If
					End If
					
					'ターゲットが動いていたら進入イベントを発生
					With t.CurrentForm
						If .Status_Renamed = "出撃" And (.x <> targets_x(i) Or .y <> targets_y(i)) Then
							HandleEvent("進入", .MainPilot.ID, .x, .y)
							If IsScenarioFinished Or IsCanceled Then
								Exit Sub
							End If
						End If
					End With
				End If
			Next 
			
			'全滅イベント
			For i = 1 To 4
				
				Select Case i
					Case 1
						uparty = "味方"
					Case 2
						uparty = "ＮＰＣ"
					Case 3
						uparty = "敵"
					Case 4
						uparty = "中立"
				End Select
				
				found = False
				For j = 1 To UBound(targets)
					With targets(j).CurrentForm
						If .Party0 = uparty And .Status_Renamed <> "出撃" Then
							found = True
							Exit For
						End If
					End With
				Next 
				
				If found Then
					found = False
					For	Each u In UList
						With u
							If .Party0 = uparty And .Status_Renamed = "出撃" And Not .IsConditionSatisfied("憑依") Then
								found = True
								Exit For
							End If
						End With
					Next u
					If Not found Then
						HandleEvent("全滅", uparty)
						If IsScenarioFinished Or IsCanceled Then
							Exit Sub
						End If
					End If
				End If
			Next 
			
			'使用後イベント
			If CurrentForm.Status_Renamed = "出撃" Then
				HandleEvent("使用後", CurrentForm.MainPilot.ID, wname)
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
			
			'攻撃後イベント
			If CurrentForm.Status_Renamed = "出撃" Then
				SaveSelections()
				For i = 1 To UBound(targets)
					SelectedTarget = targets(i).CurrentForm
					With SelectedTarget
						If .Status_Renamed = "出撃" Then
							HandleEvent("攻撃後", CurrentForm.MainPilot.ID, .MainPilot.ID)
							If IsScenarioFinished Then
								RestoreSelections()
								Exit Sub
							End If
						End If
					End With
				Next 
				RestoreSelections()
			End If
		End If
		
		'ハイパーモード＆ノーマルモードの自動発動をチェック
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
	End Sub
	
	'武器の使用によるＥＮ、弾薬の消費等を行う
	Public Sub UseWeapon(ByVal w As Short)
		Dim i, lv As Short
		Dim hp_ratio, en_ratio As Double
		
		'ＥＮ消費
		If Weapon(w).ENConsumption > 0 Then
			EN = EN - WeaponENConsumption(w)
		End If
		
		'弾数消費
		If Weapon(w).Bullet > 0 And Not IsWeaponClassifiedAs(w, "永") Then
			SetBullet(w, Bullet(w) - 1)
			
			'全弾一斉発射
			If IsWeaponClassifiedAs(w, "斉") Then
				For i = 1 To UBound(dblBullet)
					SetBullet(i, MinLng(MaxBullet(i) * dblBullet(w), Bullet(i)))
				Next 
			Else
				For i = 1 To UBound(dblBullet)
					If IsWeaponClassifiedAs(i, "斉") Then
						SetBullet(i, MinLng(CShort(MaxBullet(i) * dblBullet(w) + 0.49999), Bullet(i)))
					End If
				Next 
			End If
			
			'弾数・使用回数共有の処理
			SyncBullet()
		End If
		
		If IsWeaponClassifiedAs(w, "消") Then
			AddCondition("消耗", 1)
		End If
		
		If IsWeaponClassifiedAs(w, "尽") Then
			EN = 0
		End If
		
		If IsWeaponClassifiedAs(w, "Ｃ") And IsConditionSatisfied("チャージ完了") Then
			DeleteCondition("チャージ完了")
		End If
		
		If WeaponLevel(w, "Ａ") > 0 Then
			AddCondition(WeaponNickname(w) & "充填中", WeaponLevel(w, "Ａ"))
		End If
		
		If IsWeaponClassifiedAs(w, "気") Then
			IncreaseMorale(-5 * WeaponLevel(w, "気"))
		End If
		
		If IsWeaponClassifiedAs(w, "霊") Then
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * WeaponLevel(w, "霊")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		ElseIf IsWeaponClassifiedAs(w, "プ") Then 
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * WeaponLevel(w, "プ")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		End If
		
		If Party = "味方" Then
			If IsWeaponClassifiedAs(w, "銭") Then
				IncrMoney(-MaxLng(WeaponLevel(w, "銭"), 1) * Value \ 10)
			End If
		End If
		
		If IsWeaponClassifiedAs(w, "失") Then
			HP = MaxLng(HP - MaxHP * WeaponLevel(w, "失") \ 10, 0)
		End If
		
		'    '合体技は１ターンに１回だけ使用可能
		'    If IsWeaponClassifiedAs(w, "合") Then
		'        AddCondition "合体技使用不可", 1, 0, "非表示"
		'    End If
	End Sub
	
	'弾数
	Public Function Bullet(ByVal w As Short) As Short
		Bullet = dblBullet(w) * intMaxBullet(w)
	End Function
	
	'最大弾数
	Public Function MaxBullet(ByVal w As Short) As Short
		MaxBullet = intMaxBullet(w)
	End Function
	
	'弾数を設定
	Public Sub SetBullet(ByVal w As Short, ByVal new_bullet As Short)
		If new_bullet < 0 Then
			dblBullet(w) = 0
		ElseIf intMaxBullet(w) > 0 Then 
			dblBullet(w) = new_bullet / intMaxBullet(w)
		Else
			dblBullet(w) = 1
		End If
	End Sub
	
	'弾数・使用回数共有の処理
	Public Sub SyncBullet()
		Dim j, a, w, i, k As Short
		Dim lv, idx As Short
		
		'共属性武器の処理
		For w = 1 To CountWeapon
			If IsWeaponClassifiedAs(w, "共") Then
				lv = WeaponLevel(w, "共")
				'弾数を合わせる
				For i = 1 To CountWeapon
					If w <> i And IsWeaponClassifiedAs(i, "共") And lv = WeaponLevel(i, "共") And MaxBullet(w) > 0 Then
						If MaxBullet(i) > MaxBullet(w) Then
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Bullet(w) \ MaxBullet(w)))
						Else
							SetBullet(i, MinLng(Bullet(i), CShort(MaxBullet(i) * Bullet(w) / MaxBullet(w) + 0.49999)))
						End If
					End If
				Next 
				'アビリティの使用回数を合わせる
				For i = 1 To CountAbility
					If IsAbilityClassifiedAs(i, "共") And lv = AbilityLevel(i, "共") And MaxBullet(w) > 0 Then
						If MaxStock(i) > MaxBullet(w) Then
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Bullet(w) \ MaxBullet(w)))
						Else
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Bullet(w) / MaxBullet(w) + 0.49999))
						End If
					End If
				Next 
			End If
		Next 
		
		'共属性アビリティの処理
		For a = 1 To CountAbility
			If IsAbilityClassifiedAs(a, "共") Then
				lv = AbilityLevel(a, "共")
				'使用回数を合わせる
				For i = 1 To CountAbility
					If a <> i And IsAbilityClassifiedAs(i, "共") And lv = AbilityLevel(i, "共") And MaxStock(a) > 0 Then
						If MaxStock(i) > MaxStock(a) Then
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Stock(a) \ MaxStock(a)))
						Else
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Stock(a) / MaxStock(a) + 0.49999))
						End If
					End If
				Next 
				'弾数を合わせる
				For i = 1 To CountWeapon
					If IsWeaponClassifiedAs(i, "共") And lv = WeaponLevel(i, "共") And MaxStock(a) > 0 Then
						If MaxBullet(i) > MaxStock(a) Then
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Stock(a) \ MaxStock(a)))
						Else
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Stock(a) / MaxStock(a) + 0.49999))
						End If
					End If
				Next 
			End If
		Next 
		
		'斉属性武器の処理
		For w = 1 To CountWeapon
			If IsWeaponClassifiedAs(w, "斉") Then
				'弾数を合わせる
				For i = 1 To CountWeapon
					If w <> i And MaxBullet(i) > 0 Then
						SetBullet(w, MinLng(Bullet(w), CShort(MaxBullet(w) * Bullet(i) / MaxBullet(i) + 0.49999)))
					End If
				Next 
			End If
		Next 
		
		'他の形態の弾数も変更
		Dim counter As Short
		For i = 1 To CountOtherForm
			With OtherForm(i)
				idx = 1
				For j = 1 To CountWeapon
					counter = idx
					For k = counter To .CountWeapon
						If Weapon(j).Name = .Weapon(k).Name And MaxBullet(j) > 0 And .MaxBullet(k) > 0 Then
							.SetBullet(k, .MaxBullet(k) * Bullet(j) \ MaxBullet(j))
							idx = k + 1
							Exit For
						End If
					Next 
				Next 
				
				idx = 1
				For j = 1 To CountAbility
					counter = idx
					For k = counter To .CountAbility
						If Ability(j).Name = .Ability(k).Name And MaxStock(j) > 0 And .MaxStock(k) > 0 Then
							.SetStock(k, .MaxStock(k) * Stock(j) \ MaxStock(j))
							idx = k + 1
							Exit For
						End If
					Next 
				Next 
			End With
		Next 
	End Sub
	
	
	
	' === アビリティ関連処理 ===
	
	'アビリティ
	Public Function Ability(ByVal a As Short) As AbilityData
		Ability = adata(a)
	End Function
	
	'アビリティ総数
	Public Function CountAbility() As Short
		CountAbility = UBound(adata)
	End Function
	
	'アビリティの愛称
	Public Function AbilityNickname(ByVal a As Short) As String
		Dim u As Unit
		
		'愛称内の式置換のため、デフォルトユニットを一時的に変更する
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Me
		AbilityNickname = adata(a).Nickname
		SelectedUnitForEvent = u
	End Function
	
	'アビリティ a の最小射程
	Public Function AbilityMinRange(ByVal a As Short) As Short
		AbilityMinRange = Ability(a).MinRange
		If IsAbilityClassifiedAs(a, "小") Then
			AbilityMinRange = MinLng(AbilityMinRange + AbilityLevel(a, "小"), Ability(a).MaxRange)
		End If
	End Function
	
	'アビリティ a の最大射程
	Public Function AbilityMaxRange(ByVal a As Short) As Short
		AbilityMaxRange = Ability(a).MaxRange
	End Function
	
	'アビリティ a の消費ＥＮ
	Public Function AbilityENConsumption(ByVal a As Short) As Short
		'UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim rate_Renamed As Double
		Dim p As Pilot
		Dim i As Short
		
		With Ability(a)
			AbilityENConsumption = .ENConsumption
			
			'パイロットの能力によって術及び技の消費ＥＮは減少する
			If CountPilot > 0 Then
				p = MainPilot
				
				'術に該当するか？
				If IsSpellAbility(a) Then
					'術に該当する場合は術技能によってＥＮ消費量を変える
					Select Case p.SkillLevel("術")
						Case 1
						Case 2
							AbilityENConsumption = 0.9 * AbilityENConsumption
						Case 3
							AbilityENConsumption = 0.8 * AbilityENConsumption
						Case 4
							AbilityENConsumption = 0.7 * AbilityENConsumption
						Case 5
							AbilityENConsumption = 0.6 * AbilityENConsumption
						Case 6
							AbilityENConsumption = 0.5 * AbilityENConsumption
						Case 7
							AbilityENConsumption = 0.45 * AbilityENConsumption
						Case 8
							AbilityENConsumption = 0.4 * AbilityENConsumption
						Case 9
							AbilityENConsumption = 0.35 * AbilityENConsumption
						Case Is >= 10
							AbilityENConsumption = 0.3 * AbilityENConsumption
					End Select
					AbilityENConsumption = MinLng(MaxLng(AbilityENConsumption, 5), .ENConsumption)
				End If
				
				'技に該当するか？
				If IsFeatAbility(a) Then
					'技に該当する場合は技技能によってＥＮ消費量を変える
					Select Case p.SkillLevel("技")
						Case 1
						Case 2
							AbilityENConsumption = 0.9 * AbilityENConsumption
						Case 3
							AbilityENConsumption = 0.8 * AbilityENConsumption
						Case 4
							AbilityENConsumption = 0.7 * AbilityENConsumption
						Case 5
							AbilityENConsumption = 0.6 * AbilityENConsumption
						Case 6
							AbilityENConsumption = 0.5 * AbilityENConsumption
						Case 7
							AbilityENConsumption = 0.45 * AbilityENConsumption
						Case 8
							AbilityENConsumption = 0.4 * AbilityENConsumption
						Case 9
							AbilityENConsumption = 0.35 * AbilityENConsumption
						Case Is >= 10
							AbilityENConsumption = 0.3 * AbilityENConsumption
					End Select
					AbilityENConsumption = MinLng(MaxLng(AbilityENConsumption, 5), .ENConsumption)
				End If
			End If
			
			'ＥＮ消費減少能力による修正
			rate_Renamed = 1
			If IsFeatureAvailable("ＥＮ消費減少") Then
				For i = 1 To CountFeature
					If Feature(i) = "ＥＮ消費減少" Then
						rate_Renamed = rate_Renamed - 0.1 * FeatureLevel(i)
					End If
				Next 
			End If
			
			If rate_Renamed < 0.1 Then
				rate_Renamed = 0.1
			End If
			AbilityENConsumption = rate_Renamed * AbilityENConsumption
		End With
	End Function
	
	'アビリティ a が属性 attr を持つかどうか
	Public Function IsAbilityClassifiedAs(ByVal a As Short, ByRef attr As String) As Boolean
		If InStrNotNest(Ability(a).Class_Renamed, attr) > 0 Then
			IsAbilityClassifiedAs = True
		Else
			IsAbilityClassifiedAs = False
		End If
	End Function
	
	'アビリティ a の属性 atrr のレベル
	Public Function AbilityLevel(ByVal a As Short, ByRef attr As String) As Double
		Dim attrlv, aclass As String
		Dim start_idx, i As Short
		Dim c As String
		
		On Error GoTo ErrorHandler
		
		attrlv = attr & "L"
		
		'アビリティ属性を調べてみる
		aclass = Ability(a).Class_Renamed
		
		'レベル指定があるか？
		start_idx = InStr(aclass, attrlv)
		If start_idx = 0 Then
			Exit Function
		End If
		
		'レベル指定部分の切り出し
		start_idx = start_idx + Len(attrlv)
		i = start_idx
		Do While True
			c = Mid(aclass, i, 1)
			
			If c = "" Then
				Exit Do
			End If
			
			Select Case Asc(c)
				Case 45 To 46, 48 To 57 '"-", ".", 0-9
				Case Else
					Exit Do
			End Select
			
			i = i + 1
		Loop 
		
		AbilityLevel = CDbl(Mid(aclass, start_idx, i - start_idx))
		Exit Function
		
ErrorHandler: 
		ErrorMessage(Name & "の" & "アビリティ「" & Ability(a).Name & "」の" & "属性「" & attr & "」のレベル指定が不正です")
	End Function
	
	'アビリティ a が術かどうか
	Public Function IsSpellAbility(ByVal a As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsAbilityClassifiedAs(a, "術") Then
			IsSpellAbility = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Ability(a).NecessarySkill))
				nskill = LIndex((Ability(a).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "術" Then
					IsSpellAbility = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'アビリティ a が技かどうか
	Public Function IsFeatAbility(ByVal a As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsAbilityClassifiedAs(a, "技") Then
			IsFeatAbility = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Ability(a).NecessarySkill))
				nskill = LIndex((Ability(a).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "技" Then
					IsFeatAbility = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'アビリティ a が使用可能かどうか
	'ref_mode はユニットの状態（移動前、移動後）を示す
	Public Function IsAbilityAvailable(ByVal a As Short, ByRef ref_mode As String) As Boolean
		Dim j, i, k As Short
		Dim ad As AbilityData
		Dim uname, pname As String
		Dim u As Unit
		
		IsAbilityAvailable = False
		
		ad = Ability(a)
		
		'イベントコマンド「Disable」
		If IsDisabled((ad.Name)) Then
			Exit Function
		End If
		
		'パイロットが乗っていなければ常に使用可能と判定
		If CountPilot = 0 Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		'必要技能
		If Not IsAbilityMastered(a) Then
			Exit Function
		End If
		
		'必要条件
		If Not IsAbilityEnabled(a) Then
			Exit Function
		End If
		
		'ステータス表示では必要技能だけ満たしていればＯＫ
		If ref_mode = "インターミッション" Or ref_mode = "" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		With MainPilot
			'必要気力
			If ad.NecessaryMorale > 0 Then
				If .Morale < ad.NecessaryMorale Then
					Exit Function
				End If
			End If
			
			'霊力消費アビリティ
			If IsAbilityClassifiedAs(a, "霊") Then
				If .Plana < AbilityLevel(a, "霊") * 5 Then
					Exit Function
				End If
			ElseIf IsAbilityClassifiedAs(a, "プ") Then 
				If .Plana < AbilityLevel(a, "プ") * 5 Then
					Exit Function
				End If
			End If
		End With
		
		'属性使用不能状態
		If ConditionLifetime("オーラ使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "オ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("超能力使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "超") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("同調率使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "シ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("超感覚使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "サ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("知覚強化使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "サ") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("霊力使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "霊") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("術使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "術") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("技使用不能") > 0 Then
			If IsAbilityClassifiedAs(a, "技") Then
				Exit Function
			End If
		End If
		For i = 1 To CountCondition
			If Len(Condition(i)) > 6 Then
				If Right(Condition(i), 6) = "属性使用不能" Then
					If InStrNotNest(Ability(a).Class_Renamed, Left(Condition(i), Len(Condition(i)) - 6)) > 0 Then
						Exit Function
					End If
				End If
			End If
		Next 
		
		'弾数が足りるか
		If MaxStock(a) > 0 Then
			If Stock(a) < 1 Then
				Exit Function
			End If
		End If
		
		'ＥＮが足りるか
		If ad.ENConsumption > 0 Then
			If EN < AbilityENConsumption(a) Then
				Exit Function
			End If
		End If
		
		'お金が足りるか……
		If Party = "味方" Then
			If IsAbilityClassifiedAs(a, "銭") Then
				If Money < MaxLng(AbilityLevel(a, "銭"), 1) * Value \ 10 Then
					Exit Function
				End If
			End If
		End If
		
		'移動不能時には移動型マップアビリティは使用不能
		If IsConditionSatisfied("移動不能") Then
			If IsAbilityClassifiedAs(a, "Ｍ移") Then
				Exit Function
			End If
		End If
		
		'術及び音声技は沈黙状態では使用不能
		If IsConditionSatisfied("沈黙") Then
			With MainPilot
				If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "音") Then
					Exit Function
				End If
			End With
		End If
		
		'術は狂戦士状態では使用不能
		If IsConditionSatisfied("狂戦士") Then
			With MainPilot
				If IsSpellAbility(a) Then
					Exit Function
				End If
			End With
		End If
		
		'合体技の処理
		If IsAbilityClassifiedAs(a, "合") Then
			If Not IsCombinationAbilityAvailable(a) Then
				Exit Function
			End If
		End If
		
		'この地形で変形できるか？
		If IsAbilityClassifiedAs(a, "変") Then
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = ad.Name Then
						If Not OtherForm(LIndex(FeatureData(i), 2)).IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End If
				Next 
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				If Not OtherForm(LIndex(FeatureData("ノーマルモード"), 1)).IsAbleToEnter(x, y) Then
					Exit Function
				End If
			End If
			If IsConditionSatisfied("形態固定") Then
				Exit Function
			End If
			If IsConditionSatisfied("機体固定") Then
				Exit Function
			End If
		End If
		
		'瀕死時限定
		If IsAbilityClassifiedAs(a, "瀕") Then
			If HP > MaxHP \ 4 Then
				Exit Function
			End If
		End If
		
		'自動チャージアビリティを充填中
		If IsConditionSatisfied(AbilityNickname(a) & "充填中") Then
			Exit Function
		End If
		'共有武器＆アビリティが充填中の場合も使用不可
		Dim lv As Short
		If IsAbilityClassifiedAs(a, "共") Then
			lv = AbilityLevel(a, "共")
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "共") Then
					If lv = AbilityLevel(i, "共") Then
						If IsConditionSatisfied(AbilityNickname(i) & "充填中") Then
							Exit Function
						End If
					End If
				End If
			Next 
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "共") Then
					If lv = AbilityLevel(i, "共") Then
						If IsConditionSatisfied(AbilityNickname(i) & "充填中") Then
							Exit Function
						End If
					End If
				End If
			Next 
		End If
		
		'使用禁止
		If IsAbilityClassifiedAs(a, "禁") > 0 Then
			Exit Function
		End If
		
		'チャージ判定であればここまででＯＫ
		If ref_mode = "チャージ" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		'チャージ式アビリティ
		If IsAbilityClassifiedAs(a, "Ｃ") Then
			If Not IsConditionSatisfied("チャージ完了") Then
				Exit Function
			End If
		End If
		
		For i = 1 To ad.CountEffect
			If ad.EffectType(i) = "召喚" Then
				'召喚は既に召喚を行っている場合には不可能
				For j = 1 To CountServant
					With Servant(j).CurrentForm
						Select Case .Status_Renamed
							Case "出撃", "格納"
								'使用不可
								Exit Function
							Case "旧主形態", "旧形態"
								'合体後の形態が出撃中なら使用不可
								For k = 1 To .CountFeature
									If .Feature(k) = "合体" Then
										uname = LIndex(.FeatureData(k), 2)
										If UList.IsDefined(uname) Then
											With UList.Item(uname).CurrentForm
												If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
													Exit Function
												End If
											End With
										End If
									End If
								Next 
						End Select
					End With
				Next 
				
				'召喚ユニットのデータがちゃんと定義されているかチェック
				If Not UDList.IsDefined(ad.EffectData(i)) Then
					Exit Function
				End If
				pname = UDList.Item(ad.EffectData(i)).FeatureData("追加パイロット")
				If Not PDList.IsDefined(pname) Then
					Exit Function
				End If
				
				'召喚するユニットに乗るパイロットが汎用パイロットでもザコパイロットでも
				'ない場合、そのユニットが既に出撃中であれば使用不可
				If InStr(pname, "(汎用)") = 0 And InStr(pname, "(ザコ)") = 0 Then
					If PList.IsDefined(pname) Then
						u = PList.Item(pname).Unit_Renamed
						If Not u Is Nothing Then
							If u.Status_Renamed = "出撃" Or u.Status_Renamed = "格納" Then
								Exit Function
							End If
						End If
					End If
				End If
			End If
		Next 
		
		If ref_mode = "ステータス" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		For i = 1 To ad.CountEffect
			If ad.EffectType(i) = "変身" Then
				'自分を変身させる場合
				If Ability(a).MaxRange = 0 Then
					'ノーマルモードを持つユニットは変身できない
					'(変身からの復帰が出来ないため)
					If IsFeatureAvailable("ノーマルモード") Then
						Exit Function
					End If
					
					'その場所で変身可能か？
					With OtherForm(LIndex(Ability(a).EffectData(i), 1))
						If Not .IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End With
				End If
			End If
		Next 
		
		If ref_mode = "移動前" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		If AbilityMaxRange(a) > 1 Or AbilityMaxRange(a) = 0 Then
			If IsAbilityClassifiedAs(a, "Ｐ") Then
				IsAbilityAvailable = True
			Else
				IsAbilityAvailable = False
			End If
		Else
			If IsAbilityClassifiedAs(a, "Ｑ") Then
				IsAbilityAvailable = False
			Else
				IsAbilityAvailable = True
			End If
		End If
	End Function
	
	'アビリティ a の必要技能を満たしているか。
	Public Function IsAbilityMastered(ByVal a As Short) As Boolean
		IsAbilityMastered = IsNecessarySkillSatisfied((Ability(a).NecessarySkill))
	End Function
	
	'アビリティ a の必要条件を満たしているか。
	Public Function IsAbilityEnabled(ByVal a As Short) As Boolean
		IsAbilityEnabled = IsNecessarySkillSatisfied((Ability(a).NecessaryCondition))
	End Function
	
	'アビリティが使用可能であり、かつ射程内に有効なターゲットがいるかどうか
	Public Function IsAbilityUseful(ByVal a As Short, ByRef ref_mode As String) As Boolean
		Dim i, j As Short
		Dim max_range, min_range As Short
		
		'アビリティが使用可能か？
		If Not IsAbilityAvailable(a, ref_mode) Then
			IsAbilityUseful = False
			Exit Function
		End If
		
		'投下型マップアビリティと扇型マップアビリティは特殊なので判定ができない
		'移動型マップアビリティは移動手段として使うことを考慮
		If IsAbilityClassifiedAs(a, "Ｍ投") Or IsAbilityClassifiedAs(a, "Ｍ扇") Or IsAbilityClassifiedAs(a, "Ｍ移") Then
			IsAbilityUseful = True
			Exit Function
		End If
		
		'召喚は常に有用
		For i = 1 To Ability(a).CountEffect
			If Ability(a).EffectType(i) = "召喚" Then
				IsAbilityUseful = True
				Exit Function
			End If
		Next 
		
		min_range = AbilityMinRange(a)
		max_range = AbilityMaxRange(a)
		
		'使用する相手がいるか検索
		For i = MaxLng(x - max_range, 1) To MinLng(x + max_range, MapWidth)
			For j = MaxLng(y - max_range, 1) To MinLng(y + max_range, MapHeight)
				If System.Math.Abs(x - i) + System.Math.Abs(y - j) > max_range Then
					GoTo NextLoop
				End If
				
				If MapDataForUnit(i, j) Is Nothing Then
					GoTo NextLoop
				End If
				
				If IsAbilityEffective(a, MapDataForUnit(i, j)) Then
					IsAbilityUseful = True
					Exit Function
				End If
NextLoop: 
			Next 
		Next 
		
		IsAbilityUseful = False
	End Function
	
	'アビリティがターゲットtに対して有効(役に立つ)かどうか
	Public Function IsAbilityEffective(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim edata As String
		Dim elevel As Double
		Dim flag As Boolean
		
		With t
			'敵には使用できない。
			'IsEnemyでは魅了等がかかった味方ユニットを敵と認識してしまうので
			'ここでは独自の判定基準を使う
			Select Case Party
				Case "味方", "ＮＰＣ"
					If .Party <> "味方" And .Party0 <> "味方" And .Party <> "ＮＰＣ" And .Party0 <> "ＮＰＣ" Then
						Exit Function
					End If
				Case Else
					If .Party <> Party And .Party0 <> Party Then
						Exit Function
					End If
			End Select
			
			'アビリティがそのユニットに対して適用可能か？
			If Not IsAbilityApplicable(a, t) Then
				Exit Function
			End If
			
			IsAbilityEffective = True
			For i = 1 To Ability(a).CountEffect
				edata = Ability(a).EffectData(i)
				elevel = Ability(a).EffectLevel(i)
				Select Case Ability(a).EffectType(i)
					Case "回復"
						If elevel > 0 Then
							If .HP < .MaxHP Then
								If Not .IsConditionSatisfied("ゾンビ") Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							IsAbilityEffective = False
						Else
							'ＨＰを減少させるためのアビリティというのは有り得るので
							IsAbilityEffective = True
							Exit Function
						End If
						
					Case "治癒"
						If edata = "" Then
							If .ConditionLifetime("攻撃不能") > 0 Or .ConditionLifetime("移動不能") > 0 Or .ConditionLifetime("装甲劣化") > 0 Or .ConditionLifetime("混乱") > 0 Or .ConditionLifetime("恐怖") > 0 Or .ConditionLifetime("踊り") > 0 Or .ConditionLifetime("狂戦士") > 0 Or .ConditionLifetime("ゾンビ") > 0 Or .ConditionLifetime("回復不能") > 0 Or .ConditionLifetime("石化") > 0 Or .ConditionLifetime("凍結") > 0 Or .ConditionLifetime("麻痺") > 0 Or .ConditionLifetime("睡眠") > 0 Or .ConditionLifetime("毒") > 0 Or .ConditionLifetime("盲目") > 0 Or .ConditionLifetime("沈黙") > 0 Or .ConditionLifetime("魅了") > 0 Or .ConditionLifetime("憑依") > 0 Or .ConditionLifetime("オーラ使用不能") > 0 Or .ConditionLifetime("超能力使用不能") > 0 Or .ConditionLifetime("同調率使用不能") > 0 Or .ConditionLifetime("超感覚使用不能") > 0 Or .ConditionLifetime("知覚強化使用不能") > 0 Or .ConditionLifetime("霊力使用不能") > 0 Or .ConditionLifetime("術使用不能") > 0 Or .ConditionLifetime("技使用不能") > 0 Then
								IsAbilityEffective = True
								Exit Function
							End If
							For j = 1 To .CountCondition
								If Len(.Condition(j)) > 6 Then
									'前回書き忘れたのですが、
									'弱点はともかく有効は一概にデメリットのみでもないので
									'状態回復から除外してみました。
									If Right(.Condition(j), 6) = "属性使用不能" Then
										If .ConditionLifetime(.Condition(j)) > 0 Then
											IsAbilityEffective = True
											Exit Function
										End If
									End If
								End If
							Next 
						Else
							For j = 1 To LLength(edata)
								If .ConditionLifetime(LIndex(edata, j)) > 0 Then
									IsAbilityEffective = True
									Exit Function
								End If
							Next 
						End If
						IsAbilityEffective = False
						
					Case "補給"
						If elevel > 0 Then
							If .EN < .MaxEN Then
								If Not .IsConditionSatisfied("ゾンビ") Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							IsAbilityEffective = False
						End If
						
					Case "霊力回復", "プラーナ回復"
						If elevel > 0 Then
							If .MainPilot.Plana < .MainPilot.MaxPlana Then
								IsAbilityEffective = True
								Exit Function
							End If
							IsAbilityEffective = False
						End If
						
					Case "ＳＰ回復"
						If elevel > 0 Then
							If .MainPilot.SP < .MainPilot.MaxSP Then
								IsAbilityEffective = True
								Exit Function
							End If
							
							For j = 2 To .CountPilot
								If .Pilot(j).SP < .Pilot(j).MaxSP Then
									IsAbilityEffective = True
									Exit Function
								End If
							Next 
							
							For j = 1 To .CountSupport
								If .Support(j).SP < .Support(j).MaxSP Then
									IsAbilityEffective = True
									Exit Function
								End If
							Next 
							
							If .IsFeatureAvailable("追加サポート") Then
								If .AdditionalSupport.SP < .AdditionalSupport.MaxSP Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							
							IsAbilityEffective = False
						End If
						
					Case "気力増加"
						If elevel > 0 Then
							With .MainPilot
								If .Morale < .MaxMorale And .Personality <> "機械" Then
									IsAbilityEffective = True
									Exit Function
								End If
							End With
							
							For j = 2 To .CountPilot
								With .Pilot(j)
									If .Morale < .MaxMorale And .Personality <> "機械" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							Next 
							
							For j = 1 To .CountSupport
								With .Support(j)
									If .Morale < .MaxMorale And .Personality <> "機械" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							Next 
							
							If .IsFeatureAvailable("追加サポート") Then
								With .AdditionalSupport
									If .Morale < .MaxMorale And .Personality <> "機械" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							End If
							
							IsAbilityEffective = False
						End If
						
					Case "装填"
						If edata = "" Then
							For j = 1 To .CountWeapon
								If .Bullet(j) < .MaxBullet(j) Then
									IsAbilityEffective = True
									Exit Function
								End If
							Next 
						Else
							For j = 1 To .CountWeapon
								If .Bullet(j) < .MaxBullet(j) Then
									If .WeaponNickname(j) = edata Or InStrNotNest(.Weapon(j).Class_Renamed, edata) > 0 Then
										IsAbilityEffective = True
										Exit Function
									End If
								End If
							Next 
						End If
						IsAbilityEffective = False
						
					Case "付加"
						If Not .IsConditionSatisfied(LIndex(edata, 1) & "付加") Or IsAbilityClassifiedAs(a, "除") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "強化"
						If Not .IsConditionSatisfied(LIndex(edata, 1) & "強化") Or IsAbilityClassifiedAs(a, "除") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "状態"
						If Not .IsConditionSatisfied(edata) Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "再行動"
						If Ability(a).MaxRange = 0 Then
							GoTo NextEffect
						End If
						
						If .Action = 0 And .MaxAction > 0 Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "変身"
						If Not .IsFeatureAvailable("ノーマルモード") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "能力コピー"
						If t Is Me Or IsFeatureAvailable("ノーマルモード") Or .IsConditionSatisfied("混乱") > 0 Or .IsEnemy(Me) Or IsEnemy(t) Then
							IsAbilityEffective = False
							GoTo NextEffect
						End If
						
						If InStr(edata, "サイズ制限強") > 0 Then
							If Size <> .Size Then
								IsAbilityEffective = False
								GoTo NextEffect
							End If
						ElseIf InStr(edata, "サイズ制限無し") = 0 Then 
							Select Case Size
								Case "SS"
									Select Case .Size
										Case "M", "L", "LL", "XL"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
								Case "S"
									Select Case .Size
										Case "L", "LL", "XL"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
								Case "M"
									Select Case .Size
										Case "SS", "LL", "XL"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
								Case "L"
									Select Case .Size
										Case "SS", "S", "XL"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
								Case "LL"
									Select Case .Size
										Case "SS", "S", "M"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
								Case "XL"
									Select Case .Size
										Case "SS", "S", "M", "L"
											IsAbilityEffective = False
											GoTo NextEffect
									End Select
							End Select
						End If
						
						IsAbilityEffective = True
						Exit Function
						
				End Select
NextEffect: 
			Next 
			
			'そもそも効果がないものは常に使用可能とみなす
			'(include等で特殊効果を定義していると仮定)
			If IsAbilityEffective Then
				Exit Function
			End If
		End With
	End Function
	
	'アビリティがターゲットtに対して適用可能かどうか
	Public Function IsAbilityApplicable(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i As Short
		Dim fname As String
		
		If IsAbilityClassifiedAs(a, "封") Then
			If Not t.Weakness(Ability(a).Class_Renamed) And Not t.Effective(Ability(a).Class_Renamed) Then
				Exit Function
			End If
		End If
		
		If IsAbilityClassifiedAs(a, "限") Then
			If Not t.Weakness(Mid(Ability(a).Class_Renamed, InStrNotNest(Ability(a).Class_Renamed, "限") + 1)) And Not t.Effective(Mid(Ability(a).Class_Renamed, InStrNotNest(Ability(a).Class_Renamed, "限") + 1)) Then
				Exit Function
			End If
		End If
		
		If Me Is t Then
			'支援専用アビリティは自分には使用できない
			If Not IsAbilityClassifiedAs(a, "援") Then
				IsAbilityApplicable = True
			End If
			Exit Function
		End If
		
		'無効化の対象になる場合は使用出来ない
		If t.Immune(Ability(a).Class_Renamed) Then
			If Not t.Weakness(Ability(a).Class_Renamed) And Not t.Effective(Ability(a).Class_Renamed) Then
				Exit Function
			End If
		End If
		
		If IsAbilityClassifiedAs(a, "視") Then
			If t.IsConditionSatisfied("盲目") Then
				Exit Function
			End If
		End If
		
		With t.MainPilot
			If IsAbilityClassifiedAs(a, "対") Then
				'UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If .Level Mod AbilityLevel(a, "対") <> 0 Then
					Exit Function
				End If
			End If
			
			If IsAbilityClassifiedAs(a, "精") Then
				If .Personality = "機械" Then
					Exit Function
				End If
			End If
			
			If IsAbilityClassifiedAs(a, "♂") Then
				If .Sex <> "男性" Then
					Exit Function
				End If
			End If
			If IsAbilityClassifiedAs(a, "♀") Then
				If .Sex <> "女性" Then
					Exit Function
				End If
			End If
		End With
		
		'修理不可
		If t.IsFeatureAvailable("修理不可") Then
			For i = 1 To Ability(a).CountEffect
				If Ability(a).EffectType(i) = "回復" Then
					Exit For
				End If
			Next 
			If i <= Ability(a).CountEffect Then
				For i = 2 To CInt(t.FeatureData("修理不可"))
					fname = LIndex(t.FeatureData("修理不可"), i)
					If Left(fname, 1) = "!" Then
						fname = Mid(fname, 2)
						If fname <> AbilityNickname(a) Then
							Exit Function
						End If
					Else
						If fname = AbilityNickname(a) Then
							Exit Function
						End If
					End If
				Next 
			End If
		End If
		
		IsAbilityApplicable = True
	End Function
	
	'ユニット t がアビリティ a の射程範囲内にいるかをチェック
	Public Function IsTargetWithinAbilityRange(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim distance As Short
		
		IsTargetWithinAbilityRange = True
		
		distance = System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y)
		
		'最小射程チェック
		If distance < AbilityMinRange(a) Then
			IsTargetWithinAbilityRange = False
			Exit Function
		End If
		
		'最大射程チェック
		If distance > AbilityMaxRange(a) Then
			IsTargetWithinAbilityRange = False
			Exit Function
		End If
		
		'合体技で射程が１の場合は相手を囲んでいる必要がある
		Dim partners() As Unit
		If IsAbilityClassifiedAs(a, "合") And Not IsAbilityClassifiedAs(a, "Ｍ") And AbilityMaxRange(a) = 1 Then
			CombinationPartner("アビリティ", a, partners, t.x, t.y)
			If UBound(partners) = 0 Then
				IsTargetWithinAbilityRange = False
				Exit Function
			End If
		End If
	End Function
	
	'移動を併用した場合にユニット t がアビリティ a の射程範囲内にいるかをチェック
	Public Function IsTargetReachableForAbility(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim max_range As Short
		
		IsTargetReachableForAbility = True
		
		With t
			'移動範囲から敵に攻撃が届くかをチェック
			max_range = AbilityMaxRange(a)
			For i = MaxLng(.x - max_range, 1) To MinLng(.x + max_range, MapWidth)
				For j = MaxLng(.y - (max_range - System.Math.Abs(.x - i)), 1) To MinLng(.y + (max_range - System.Math.Abs(.x - i)), MapHeight)
					If Not MaskData(i, j) Then
						Exit Function
					End If
				Next 
			Next 
		End With
		
		IsTargetReachableForAbility = False
	End Function
	
	'アビリティの残り使用回数
	Public Function Stock(ByVal a As Short) As Short
		Stock = dblStock(a) * MaxStock(a)
	End Function
	
	'アビリティの最大使用回数
	Public Function MaxStock(ByVal a As Short) As Short
		If BossRank > 0 Then
			MaxStock = Ability(a).Stock * (5 + BossRank) / 5
		Else
			MaxStock = Ability(a).Stock
		End If
	End Function
	
	'アビリティの残り使用回数を設定
	Public Sub SetStock(ByVal a As Short, ByVal new_stock As Short)
		If new_stock < 0 Then
			dblStock(a) = 0
		ElseIf MaxStock(a) > 0 Then 
			dblStock(a) = new_stock / MaxStock(a)
		Else
			dblStock(a) = 1
		End If
	End Sub
	
	
	
	' === アビリティ発動関連処理 ===
	
	'アビリティを使用
	Public Function ExecuteAbility(ByVal a As Short, ByRef t As Unit, Optional ByVal is_map_ability As Boolean = False, Optional ByVal is_event As Boolean = False) As Boolean
		Dim partners() As Unit
		Dim num, j, i, k, w As Short
		Dim aclass, aname, anickname, atype As String
		Dim edata As String
		Dim elevel, elevel2 As Double
		Dim elv_mod, elv_mod2 As Double
		Dim epower As Integer
		Dim prev_value As Integer
		Dim is_useful, flag As Boolean
		Dim u As Unit
		Dim p As Pilot
		Dim buf, msg As String
		Dim uname, pname, fname As String
		Dim ftype, fdata As String
		Dim flevel As Double
		Dim ftype2, fdata2 As String
		Dim flevel2 As Double
		Dim is_anime_played As Boolean
		Dim hp_ratio, en_ratio As Double
		Dim tx, ty As Short
		Dim tx2, ty2 As Short
		Dim itm As Item
		Dim cname As String
		
		aname = Ability(a).Name
		anickname = AbilityNickname(a)
		aclass = Ability(a).Class_Renamed
		
		'現在の選択状況をセーブ
		SaveSelections()
		
		'選択内容を切り替え
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedTarget = t
		SelectedTargetForEvent = t
		SelectedAbility = a
		SelectedAbilityName = aname
		
		If Not is_map_ability Then
			'通常アビリティの場合
			If BattleAnimation Then
				RedrawScreen()
			End If
			
			If IsAbilityClassifiedAs(a, "合") Then
				'射程が0の場合はマスクをクリアしておく
				If AbilityMaxRange(a) = 0 Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							MaskData(i, j) = True
						Next 
					Next 
					MaskData(x, y) = False
				End If
				
				'合体技の場合にパートナーをハイライト表示
				If AbilityMaxRange(a) = 1 Then
					CombinationPartner("アビリティ", a, partners, t.x, t.y)
				Else
					CombinationPartner("アビリティ", a, partners)
				End If
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.x, .y) = False
					End With
				Next 
				
				If Not BattleAnimation Then
					MaskScreen()
				End If
			Else
				ReDim partners(0)
				ReDim SelectedPartners(0)
			End If
			
			'ダイアログ用にあらかじめ追加パイロットを作成しておく
			For i = 1 To Ability(a).CountEffect
				edata = Ability(a).EffectData(i)
				Select Case Ability(a).EffectType(i)
					Case "変身"
						If Not UDList.IsDefined(LIndex(edata, 1)) Then
							ErrorMessage(LIndex(edata, 1) & "のデータが定義されていません")
							Exit Function
						End If
						
						With UDList.Item(LIndex(edata, 1))
							If .IsFeatureAvailable("追加パイロット") Then
								If Not PList.IsDefined(.FeatureData("追加パイロット")) Then
									PList.Add(.FeatureData("追加パイロット"), MainPilot.Level, Party0)
								End If
							End If
						End With
				End Select
			Next 
			
			'アビリティ使用時のメッセージ＆特殊効果
			If IsAnimationDefined(aname & "(準備)") Then
				PlayAnimation(aname & "(準備)")
			End If
			If IsMessageDefined("かけ声(" & aname & ")") Then
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				PilotMessage("かけ声(" & aname & ")")
			End If
			If IsMessageDefined(aname) Or IsMessageDefined("アビリティ") Then
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				PilotMessage(aname, "アビリティ")
			End If
			If IsAnimationDefined(aname & "(使用)") Then
				PlayAnimation(aname & "(使用)", "", True)
			End If
			If IsAnimationDefined(aname & "(発動)") Or IsAnimationDefined(aname) Then
				PlayAnimation(aname & "(発動)", "", True)
				is_anime_played = True
			Else
				SpecialEffect(aname, "", True)
			End If
			
			'アビリティの種類は？
			For i = 1 To Ability(a).CountEffect
				Select Case Ability(a).EffectType(i)
					Case "召喚"
						aname = ""
						Exit For
					Case "再行動"
						If Ability(a).MaxRange > 0 Then
							atype = Ability(a).EffectType(i)
						End If
					Case "解説"
					Case Else
						atype = Ability(a).EffectType(i)
				End Select
			Next 
			Select Case UBound(partners)
				Case 0
					'通常
					msg = Nickname & "は"
				Case 1
					'２体合体
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "と[" & partners(1).MainPilot.Nickname & "]の[" & Nickname & "]は"
					Else
						msg = Nickname & "達は"
					End If
				Case 2
					'３体合体
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "は[" & partners(1).Nickname & "]、[" & partners(2).Nickname & "]と共に"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "、[" & partners(1).MainPilot.Nickname & "]、[" & partners(2).MainPilot.Nickname & "]の[" & Nickname & "]は"
					Else
						msg = Nickname & "達は"
					End If
				Case Else
					'３体以上
					msg = Nickname & "達は"
			End Select
			
			If IsSpellAbility(a) Then
				If Not t Is Nothing And Ability(a).MaxRange <> 0 Then
					If Me Is t Then
						msg = msg & "自分に"
					Else
						msg = msg & "[" & t.Nickname & "]に"
					End If
				End If
				If Right(anickname, 2) = "呪文" Then
					msg = msg & "[" & anickname & "]を唱えた。"
				ElseIf Right(anickname, 2) = "の杖" Then 
					msg = msg & "[" & Left(anickname, Len(anickname) - 2) & "]の呪文を唱えた。"
				Else
					msg = msg & "[" & anickname & "]の呪文を唱えた。"
				End If
			ElseIf Right(anickname, 1) = "歌" Then 
				msg = msg & "[" & anickname & "]を歌った。"
			ElseIf Right(anickname, 2) = "踊り" Then 
				msg = msg & "[" & anickname & "]を踊った。"
			Else
				If Not t Is Nothing And Ability(a).MaxRange <> 0 Then
					If Me Is t Then
						msg = msg & "自分に"
					Else
						msg = msg & "[" & t.Nickname & "]に"
					End If
				End If
				msg = msg & "[" & anickname & "]を使った。"
			End If
			
			If IsSysMessageDefined(aname) Then
				'「アビリティ名(解説)」のメッセージを使用
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				SysMessage(aname)
			ElseIf IsSysMessageDefined("アビリティ") Then 
				'「アビリティ(解説)」のメッセージを使用
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				SysMessage("アビリティ")
			ElseIf atype = "変身" And Ability(a).MaxRange = 0 Then 
				'変身の場合はメッセージなし
			ElseIf atype <> "" Then 
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				DisplaySysMessage(msg)
			End If
			
			'ＥＮ消費＆使用回数減少
			UseAbility(a)
			
			'アビリティの使用に失敗？
			If Dice(10) <= AbilityLevel(a, "難") Then
				DisplaySysMessage("しかし何もおきなかった…")
				GoTo Finish
			End If
		Else
			'マップアビリティの場合
			If IsAnimationDefined(aname & "(発動)") Or IsAnimationDefined(aname) Then
				PlayAnimation(aname & "(発動)")
				is_anime_played = True
			End If
		End If
		
		'相手がアビリティの属性に対して無効化属性を持っているならアビリティは
		'効果なし
		If Not Me Is t Then
			If t.Immune(aclass) Then
				GoTo Finish
			End If
		End If
		
		'気力低下アビリティ
		If IsAbilityClassifiedAs(a, "脱") Then
			t.IncreaseMorale(-10)
		End If
		
		'特殊効果除去アビリティ
		If IsAbilityClassifiedAs(a, "除") Then
			i = 1
			Do While i <= t.CountCondition
				If (InStr(t.Condition(i), "付加") > 0 Or InStr(t.Condition(i), "強化") > 0 Or InStr(t.Condition(i), "ＵＰ") > 0) And t.Condition(i) <> "ノーマルモード付加" And t.ConditionLifetime(i) <> 0 Then
					t.DeleteCondition(i)
				Else
					i = i + 1
				End If
			Loop 
		End If
		
		'得意技・不得手によるアビリティ効果への修正値を計算
		elv_mod = 1#
		elv_mod2 = 1#
		With MainPilot
			'得意技
			If .IsSkillAvailable("得意技") Then
				buf = .SkillData("得意技")
				For i = 1 To Len(buf)
					If InStr(aclass, GetClassBundle(buf, i)) > 0 Then
						elv_mod = 1.2 * elv_mod
						elv_mod2 = 1.4 * elv_mod2
						Exit For
					End If
				Next 
			End If
			
			'不得手
			If .IsSkillAvailable("不得手") Then
				buf = .SkillData("不得手")
				For i = 1 To Len(buf)
					If InStr(aclass, GetClassBundle(buf, i)) > 0 Then
						elv_mod = 0.8 * elv_mod
						elv_mod2 = 0.6 * elv_mod2
						Exit For
					End If
				Next 
			End If
		End With
		
		'アビリティの効果を適用
		For i = 1 To Ability(a).CountEffect
			With Ability(a)
				edata = .EffectData(i)
				elevel = .EffectLevel(i) * elv_mod
				elevel2 = .EffectLevel(i) * elv_mod2
			End With
			Select Case Ability(a).EffectType(i)
				Case "回復"
					With t
						If elevel > 0 Then
							'ＨＰは既に最大値？
							If .HP = .MaxHP Then
								GoTo NextLoop
							End If
							
							'ゾンビ？
							If .IsConditionSatisfied("ゾンビ") Then
								GoTo NextLoop
							End If
							
							If Not is_anime_played Then
								If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "魔") Then
									ShowAnimation("回復魔法発動")
								Else
									ShowAnimation("修理装置発動")
								End If
							End If
							
							prev_value = .HP
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(5 * elevel * .Shooting)
								Else
									epower = 500 * elevel
								End If
								epower = epower * (10 + .SkillLevel("修理")) \ 10
							End With
							t.HP = t.HP + epower
							DrawSysString(.x, .y, "+" & VB6.Format(.HP - prev_value))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の" & Term("ＨＰ", t) & "が[" & VB6.Format(.HP - prev_value) & "]回復した;" & "残り" & Term("ＨＰ", t) & "は" & VB6.Format(.HP) & "（損傷率 = " & VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP) & "％）")
							is_useful = True
						ElseIf elevel < 0 Then 
							prev_value = .HP
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(5 * elevel * .Shooting)
								Else
									epower = 500 * elevel
								End If
							End With
							t.HP = t.HP + epower
							DrawSysString(.x, .y, "-" & VB6.Format(prev_value - .HP))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の" & Term("ＨＰ", t) & "が[" & VB6.Format(prev_value - .HP) & "]減少した;" & "残り" & Term("ＨＰ", t) & "は" & VB6.Format(.HP) & "（損傷率 = " & VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP) & "％）")
						End If
					End With
					
				Case "補給"
					With t
						If elevel > 0 Then
							'ＥＮは既に最大値？
							If .EN = .MaxEN Then
								GoTo NextLoop
							End If
							
							'ゾンビ？
							If .IsConditionSatisfied("ゾンビ") Then
								GoTo NextLoop
							End If
							
							If Not is_anime_played Then
								If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "魔") Then
									ShowAnimation("回復魔法発動")
								Else
									ShowAnimation("補給装置発動")
								End If
							End If
							
							prev_value = .EN
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(elevel * .Shooting \ 2)
								Else
									epower = 50 * elevel
								End If
								epower = epower * (10 + .SkillLevel("補給")) \ 10
							End With
							t.EN = t.EN + epower
							DrawSysString(.x, .y, "+" & VB6.Format(.EN - prev_value))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の" & Term("ＥＮ", t) & "が[" & VB6.Format(.EN - prev_value) & "]回復した;" & "残り" & Term("ＥＮ", t) & "は" & VB6.Format(.EN))
							is_useful = True
						ElseIf elevel < 0 Then 
							'ＥＮは既に0？
							If .EN = 0 Then
								GoTo NextLoop
							End If
							
							prev_value = .EN
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(elevel * .Shooting \ 2)
								Else
									epower = 50 * elevel
								End If
							End With
							t.EN = t.EN + epower
							DrawSysString(.x, .y, "-" & VB6.Format(prev_value - .EN))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の" & Term("ＥＮ", t) & "が[" & VB6.Format(prev_value - .EN) & "]減少した;" & "残り" & Term("ＥＮ", t) & "は" & VB6.Format(.EN))
						End If
					End With
					
				Case "霊力回復", "プラーナ回復"
					With t.MainPilot
						If elevel > 0 Then
							'霊力は既に最大値？
							If .Plana = .MaxPlana Then
								GoTo NextLoop
							End If
							
							prev_value = .Plana
							If IsSpellAbility(a) Then
								.Plana = .Plana + CInt(elevel * MainPilot.Shooting \ 10)
							Else
								.Plana = .Plana + 10 * elevel
							End If
							DrawSysString(t.x, t.y, "+" & VB6.Format(.Plana - prev_value))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の[" & .SkillName0("霊力") & "]が[" & VB6.Format(.Plana - prev_value) & "]回復した。")
							is_useful = True
						ElseIf elevel < 0 Then 
							'霊力は既に0？
							If .Plana = 0 Then
								GoTo NextLoop
							End If
							
							prev_value = .Plana
							If IsSpellAbility(a) Then
								.Plana = .Plana + CInt(elevel * MainPilot.Shooting \ 10)
							Else
								.Plana = .Plana + 10 * elevel
							End If
							DrawSysString(t.x, t.y, "-" & VB6.Format(prev_value - .Plana))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "の[" & .SkillName0("霊力") & "]が[" & VB6.Format(prev_value - .Plana) & "]減少した。")
						End If
					End With
					
				Case "ＳＰ回復"
					If IsSpellAbility(a) Then
						epower = CInt(elevel * MainPilot.Shooting \ 10)
					Else
						epower = 10 * elevel
					End If
					
					With t
						'パイロット数を計算
						num = .CountPilot + .CountSupport
						If .IsFeatureAvailable("追加サポート") Then
							num = num + 1
						End If
						
						If elevel > 0 Then
							If num = 1 Then
								'パイロットが１名のみ
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower
									DrawSysString(t.x, t.y, "+" & VB6.Format(.SP - prev_value))
									DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(.SP - prev_value) & "回復した。")
									If .SP > prev_value Then
										is_useful = True
									End If
								End With
							Else
								'複数のパイロットが対象
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower \ 5 + epower \ num
									DrawSysString(t.x, t.y, "+" & VB6.Format(.SP - prev_value))
									DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(.SP - prev_value) & "回復した。")
									If .SP > prev_value Then
										is_useful = True
									End If
								End With
								
								For j = 2 To .CountPilot
									With .Pilot(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(.SP - prev_value) & "回復した。")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								Next 
								
								For j = 1 To .CountSupport
									With .Support(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(.SP - prev_value) & "回復した。")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								Next 
								
								If .IsFeatureAvailable("追加サポート") Then
									With .AdditionalSupport
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(.SP - prev_value) & "回復した。")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								End If
							End If
						ElseIf elevel < 0 Then 
							If num = 1 Then
								'パイロットが１名のみ
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower
									DrawSysString(t.x, t.y, "-" & VB6.Format(prev_value - .SP))
									DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(prev_value - .SP) & "減少した。")
								End With
							Else
								'複数のパイロットが対象
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower \ 5 + epower \ num
									DrawSysString(t.x, t.y, "+" & VB6.Format(prev_value - .SP))
									DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(prev_value - .SP) & "減少した。")
								End With
								
								For j = 2 To .CountPilot
									With .Pilot(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(prev_value - .SP) & "減少した。")
									End With
								Next 
								
								For j = 1 To .CountSupport
									With .Support(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(prev_value - .SP) & "減少した。")
									End With
								Next 
								
								If .IsFeatureAvailable("追加サポート") Then
									With .AdditionalSupport
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "の" & Term("ＳＰ", t) & "が" & VB6.Format(prev_value - .SP) & "減少した。")
									End With
								End If
							End If
						End If
					End With
					
				Case "気力増加"
					If IsSpellAbility(a) Then
						epower = CInt(elevel * MainPilot.Shooting \ 10)
					Else
						epower = 10 * elevel
					End If
					With t
						prev_value = .MainPilot.Morale
						.IncreaseMorale(epower)
						
						If elevel > 0 Then
							With .MainPilot
								DrawSysString(t.x, t.y, "+" & VB6.Format(.Morale - prev_value))
								DisplaySysMessage(.Nickname & "の" & Term("気力", t) & "が" & VB6.Format(.Morale - prev_value) & "増加した。")
							End With
						ElseIf elevel < 0 Then 
							With .MainPilot
								DrawSysString(t.x, t.y, "-" & VB6.Format(prev_value - .Morale))
								DisplaySysMessage(.Nickname & "の" & Term("気力", t) & "が" & VB6.Format(prev_value - .Morale) & "減少した。")
							End With
						End If
						
						If .MainPilot.Morale > prev_value Then
							is_useful = True
						End If
					End With
					
				Case "装填"
					With t
						flag = False
						If edata = "" Then
							'全ての武器の弾数を回復
							For j = 1 To .CountWeapon
								If .Bullet(j) < .MaxBullet(j) Then
									.BulletSupply()
									flag = True
									Exit For
								End If
							Next 
							
							'弾数とアビリティ使用回数の同期を取る
							If flag Then
								For j = 1 To .CountAbility
									If .IsAbilityClassifiedAs(j, "共") Then
										For k = 1 To .CountWeapon
											If .IsWeaponClassifiedAs(k, "共") And .AbilityLevel(j, "共") = .WeaponLevel(k, "共") Then
												.SetStock(j, .MaxStock(j))
											End If
										Next 
									End If
								Next 
								
								'弾数・使用回数の共有化処理
								.SyncBullet()
							End If
						Else
							'特定の武器の弾数のみを回復
							For j = 1 To .CountWeapon
								If .Bullet(j) < .MaxBullet(j) Then
									If .WeaponNickname(j) = edata Or InStrNotNest(.Weapon(j).Class_Renamed, edata) > 0 Then
										.SetBullet(j, .MaxBullet(j))
										flag = True
										w = j
									End If
								End If
							Next 
							For j = 1 To .CountOtherForm
								With .OtherForm(j)
									For k = 1 To .CountWeapon
										If .Bullet(k) < .MaxBullet(k) Then
											If .WeaponNickname(k) = edata Or InStrNotNest(.Weapon(k).Class_Renamed, edata) > 0 Then
												.SetBullet(k, .MaxBullet(k))
											End If
										End If
									Next 
								End With
							Next 
							
							'弾数の同期を取る
							If flag Then
								If .IsWeaponClassifiedAs(w, "共") Then
									For j = 1 To .CountWeapon
										If .IsWeaponClassifiedAs(j, "共") And .WeaponLevel(j, "共") = .WeaponLevel(w, "共") Then
											.SetBullet(j, .MaxBullet(j))
										End If
									Next 
									For j = 1 To .CountAbility
										If .IsAbilityClassifiedAs(j, "共") And .AbilityLevel(j, "共") = .WeaponLevel(w, "共") Then
											.SetStock(j, .MaxStock(j))
										End If
									Next 
								End If
								
								'弾数・使用回数の共有化処理
								.SyncBullet()
							End If
						End If
						
						If flag Then
							DisplaySysMessage(.Nickname & "の武装の使用回数が回復した。")
							If AbilityMaxRange(a) > 0 Then
								is_useful = True
							End If
						End If
					End With
					
				Case "治癒"
					With t
						If Not is_anime_played Then
							If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "魔") Then
								ShowAnimation("回復魔法発動")
							End If
						End If
						If edata = "" Then
							'全てのステータス異常を回復
							If .ConditionLifetime("攻撃不能") > 0 Then
								.DeleteCondition("攻撃不能")
								is_useful = True
							End If
							If .ConditionLifetime("移動不能") > 0 Then
								.DeleteCondition("移動不能")
								is_useful = True
							End If
							If .ConditionLifetime("装甲劣化") > 0 Then
								.DeleteCondition("装甲劣化")
								is_useful = True
							End If
							If .ConditionLifetime("混乱") > 0 Then
								.DeleteCondition("混乱")
								is_useful = True
							End If
							If .ConditionLifetime("恐怖") > 0 Then
								.DeleteCondition("恐怖")
								is_useful = True
							End If
							If .ConditionLifetime("踊り") > 0 Then
								.DeleteCondition("踊り")
								is_useful = True
							End If
							If .ConditionLifetime("狂戦士") > 0 Then
								.DeleteCondition("狂戦士")
								is_useful = True
							End If
							If .ConditionLifetime("ゾンビ") > 0 Then
								.DeleteCondition("ゾンビ")
								is_useful = True
							End If
							If .ConditionLifetime("回復不能") > 0 Then
								.DeleteCondition("回復不能")
								is_useful = True
							End If
							If .ConditionLifetime("石化") > 0 Then
								.DeleteCondition("石化")
								is_useful = True
							End If
							If .ConditionLifetime("凍結") > 0 Then
								.DeleteCondition("凍結")
								is_useful = True
							End If
							If .ConditionLifetime("麻痺") > 0 Then
								.DeleteCondition("麻痺")
								is_useful = True
							End If
							If .ConditionLifetime("睡眠") > 0 Then
								.DeleteCondition("睡眠")
								is_useful = True
							End If
							If .ConditionLifetime("毒") > 0 Then
								.DeleteCondition("毒")
								is_useful = True
							End If
							If .ConditionLifetime("盲目") > 0 Then
								.DeleteCondition("盲目")
								is_useful = True
							End If
							If .ConditionLifetime("沈黙") > 0 Then
								.DeleteCondition("沈黙")
								is_useful = True
							End If
							If .ConditionLifetime("魅了") > 0 Then
								.DeleteCondition("魅了")
								is_useful = True
							End If
							If .ConditionLifetime("憑依") > 0 Then
								.DeleteCondition("憑依")
								is_useful = True
							End If
							'剋属性
							If .ConditionLifetime("オーラ使用不能") > 0 Then
								.DeleteCondition("オーラ使用不能")
							End If
							If .ConditionLifetime("超能力使用不能") > 0 Then
								.DeleteCondition("超能力使用不能")
							End If
							If .ConditionLifetime("同調率使用不能") > 0 Then
								.DeleteCondition("同調率使用不能")
							End If
							If .ConditionLifetime("超感覚使用不能") > 0 Then
								.DeleteCondition("超感覚使用不能")
							End If
							If .ConditionLifetime("知覚強化使用不能") > 0 Then
								.DeleteCondition("知覚強化使用不能")
							End If
							If .ConditionLifetime("霊力使用不能") > 0 Then
								.DeleteCondition("霊力使用不能")
							End If
							If .ConditionLifetime("術使用不能") > 0 Then
								.DeleteCondition("術使用不能")
							End If
							If .ConditionLifetime("技使用不能") > 0 Then
								.DeleteCondition("技使用不能")
							End If
							j = 1
							Do While j <= .CountCondition
								'弱点、有効付加はあえて外してあります。
								If Len(.Condition(j)) > 6 And Right(.Condition(j), 6) = "属性使用不能" And .ConditionLifetime(.Condition(j)) > 0 Then
									.DeleteCondition(.Condition(j))
									is_useful = True
								Else
									j = j + 1
								End If
							Loop 
							If is_useful Then
								If t Is CurrentForm Then
									UpdateMessageForm(t)
								Else
									UpdateMessageForm(t, CurrentForm)
								End If
								DisplaySysMessage(.Nickname & "の状態が回復した。")
							End If
						Else
							'指定されたステータス異常のみを回復
							j = 1
							Do While j <= LLength(edata)
								cname = LIndex(edata, j)
								If .ConditionLifetime(cname) > 0 Then
									.DeleteCondition(cname)
									If t Is CurrentForm Then
										UpdateMessageForm(t)
									Else
										UpdateMessageForm(t, CurrentForm)
									End If
									If cname = "装甲劣化" Then
										cname = Term("装甲", t) & "劣化"
									End If
									DisplaySysMessage(.Nickname & "の[" & cname & "]が回復した。")
									is_useful = True
								Else
									j = j + 1
								End If
							Loop 
						End If
					End With
					
				Case "付加"
					With t
						If elevel2 = 0 Then
							'レベル指定がない場合は付加が半永久的に持続
							elevel2 = 10000
						Else
							'そうでなければ最低１ターンは効果が持続
							elevel2 = MaxLng(CShort(elevel2), 1)
						End If
						
						'効果時間が継続中？
						If .IsConditionSatisfied(LIndex(edata, 1) & "付加") Then
							GoTo NextLoop
						End If
						
						ftype = LIndex(edata, 1)
						flevel = CDbl(LIndex(edata, 2))
						fdata = ""
						For j = 3 To LLength(edata)
							fdata = fdata & LIndex(edata, j) & " "
						Next 
						fdata = Trim(fdata)
						If Left(fdata, 1) = """" And Right(fdata, 1) = """" Then
							fdata = Trim(Mid(fdata, 2, Len(fdata) - 2))
						End If
						
						'エリアスが定義されている？
						If ALDList.IsDefined(ftype) Then
							With ALDList.Item(ftype)
								For j = 1 To .Count
									'エリアスの定義に従って特殊能力定義を置き換える
									ftype2 = .AliasType(j)
									
									If LIndex(.AliasData(j), 1) = "解説" Then
										'特殊能力の解説
										If fdata <> "" Then
											ftype2 = LIndex(fdata, 1)
										End If
										flevel2 = DEFAULT_LEVEL
										fdata2 = .AliasData(j)
									Else
										'通常の特殊能力
										If .AliasLevelIsPlusMod(j) Then
											If flevel = DEFAULT_LEVEL Then
												flevel = 1
											End If
											flevel2 = flevel + .AliasLevel(j)
										ElseIf .AliasLevelIsMultMod(j) Then 
											If flevel = DEFAULT_LEVEL Then
												flevel = 1
											End If
											flevel2 = flevel * .AliasLevel(j)
										ElseIf flevel <> DEFAULT_LEVEL Then 
											flevel2 = flevel
										Else
											flevel2 = .AliasLevel(j)
										End If
										
										fdata2 = .AliasData(j)
										If fdata <> "" Then
											If InStr(fdata2, "非表示") <> 1 Then
												fdata2 = fdata & " " & ListTail(fdata2, LLength(fdata) + 1)
											End If
										End If
									End If
									
									t.AddCondition(ftype2 & "付加", elevel2, flevel2, fdata2)
								Next 
							End With
						Else
							.AddCondition(ftype & "付加", elevel2, flevel, fdata)
						End If
						
						.Update()
						
						If t Is CurrentForm Then
							UpdateMessageForm(t)
						Else
							UpdateMessageForm(t, CurrentForm)
						End If
						Select Case LIndex(edata, 1)
							Case "耐性", "無効化", "吸収"
								DisplaySysMessage(.Nickname & "は[" & LIndex(edata, 3) & "]属性に対する[" & LIndex(edata, 1) & "]能力を得た。")
							Case "特殊効果無効化"
								DisplaySysMessage(.Nickname & "は[" & LIndex(edata, 3) & "]属性に対する無効化能力を得た。")
							Case "攻撃属性"
								DisplaySysMessage(.Nickname & "は[" & LIndex(edata, 3) & "]の攻撃属性を得た。")
							Case "武器強化"
								DisplaySysMessage(.Nickname & "の" & "武器の攻撃力が上がった。")
							Case "命中率強化"
								DisplaySysMessage(.Nickname & "の" & "武器の命中率が上がった。")
							Case "ＣＴ率強化"
								DisplaySysMessage(.Nickname & "の" & "武器のＣＴ率が上がった。")
							Case "特殊効果発動率強化"
								DisplaySysMessage(.Nickname & "の" & "武器の特殊効果発動率が上がった。")
							Case "射程延長"
								DisplaySysMessage(.Nickname & "の" & "武器の射程が伸びた。")
							Case "サイズ変更"
								DisplaySysMessage(.Nickname & "の" & "サイズが" & StrConv(LIndex(edata, 3), VbStrConv.Wide) & "サイズに変化した。")
							Case "パイロット愛称", "パイロット画像", "愛称変更", "ユニット画像", "ＢＧＭ"
								'メッセージを表示しない。
							Case Else
								'付加する能力名
								fname = ListIndex(fdata, 1)
								If fname = "" Or fname = "非表示" Then
									If LIndex(edata, 2) <> VB6.Format(DEFAULT_LEVEL) Then
										fname = LIndex(edata, 1) & "Lv" & LIndex(edata, 2)
									Else
										fname = LIndex(edata, 1)
									End If
								End If
								DisplaySysMessage(.Nickname & "は[" & fname & "]の能力を得た。")
						End Select
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "強化"
					With t
						If elevel2 = 0 Then
							'レベル指定がない場合は付加が半永久的に持続
							elevel2 = 10000
						Else
							'そうでなければ最低１ターンは効果が持続
							elevel2 = MaxLng(CShort(elevel2), 1)
						End If
						
						'効果時間が継続中？
						If .IsConditionSatisfied(LIndex(edata, 1) & "強化") Then
							GoTo NextLoop
						End If
						
						ftype = LIndex(edata, 1)
						flevel = CDbl(LIndex(edata, 2))
						fdata = ""
						For j = 3 To LLength(edata)
							fdata = fdata & LIndex(edata, j) & " "
						Next 
						fdata = Trim(fdata)
						
						'エリアスが定義されている？
						If ALDList.IsDefined(ftype) Then
							With ALDList.Item(ftype)
								For j = 1 To .Count
									'エリアスの定義に従って特殊能力定義を置き換える
									ftype2 = .AliasType(i)
									
									If LIndex(.AliasData(j), 1) = "解説" Then
										'特殊能力の解説
										If fdata <> "" Then
											ftype2 = LIndex(fdata, 1)
										End If
										flevel2 = DEFAULT_LEVEL
										fdata2 = .AliasData(j)
										t.AddCondition(ftype2 & "付加", elevel2, flevel2, fdata2)
									Else
										'通常の特殊能力
										If .AliasLevelIsMultMod(j) Then
											If flevel = DEFAULT_LEVEL Then
												flevel = 1
											End If
											flevel2 = flevel * .AliasLevel(j)
										ElseIf flevel <> DEFAULT_LEVEL Then 
											flevel2 = flevel
										Else
											flevel2 = .AliasLevel(j)
										End If
										
										fdata2 = .AliasData(j)
										If fdata <> "" Then
											If InStr(fdata2, "非表示") <> 1 Then
												fdata2 = fdata & " " & ListTail(fdata2, LLength(fdata) + 1)
											End If
										End If
										
										t.AddCondition(ftype2 & "強化", elevel2, flevel2, fdata2)
									End If
								Next 
							End With
						Else
							.AddCondition(ftype & "強化", elevel2, flevel, fdata)
						End If
						
						.Update()
						
						If t Is CurrentForm Then
							UpdateMessageForm(t)
						Else
							UpdateMessageForm(t, CurrentForm)
						End If
						
						'強化する能力名
						fname = LIndex(edata, 3)
						If fname = "" Or fname = "非表示" Then
							fname = LIndex(edata, 1)
						End If
						If t.SkillName0(fname) <> "非表示" Then
							fname = t.SkillName0(fname)
						End If
						DisplaySysMessage(.Nickname & "の[" & fname & "]レベルが" & LIndex(edata, 2) & "上がった。")
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "状態"
					With t
						If elevel2 = 0 Then
							'レベル指定がない場合は付加が半永久的に持続
							elevel2 = 10000
						Else
							'そうでなければ最低１ターンは状態が持続
							elevel = MaxLng(CShort(elevel2), 1)
						End If
						
						'効果時間が継続中？
						If .IsConditionSatisfied(edata) Then
							GoTo NextLoop
						End If
						
						.AddCondition(edata, elevel2)
						
						'状態発動アニメーション表示
						If Not IsAnimationDefined(aname & "(発動)") And Not IsAnimationDefined(aname) Then
							Select Case edata
								Case "攻撃力ＵＰ", "防御力ＵＰ", "運動性ＵＰ", "移動力ＵＰ", "狂戦士"
									ShowAnimation(edata & "発動")
							End Select
						End If
						
						Select Case edata
							Case "装甲劣化"
								cname = Term("装甲", t) & "劣化"
							Case "運動性ＵＰ"
								cname = Term("運動性", t) & "ＵＰ"
							Case "運動性ＤＯＷＮ"
								cname = Term("運動性", t) & "ＤＯＷＮ"
							Case "移動力ＵＰ"
								cname = Term("移動力", t) & "ＵＰ"
							Case "移動力ＤＯＷＮ"
								cname = Term("移動力", t) & "ＤＯＷＮ"
							Case Else
								cname = edata
						End Select
						
						DisplaySysMessage(.Nickname & "は" & cname & "の状態になった。")
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "召喚"
					UpdateMessageForm(CurrentForm)
					If Not UDList.IsDefined(edata) Then
						ErrorMessage(edata & "のデータが定義されていません")
						Exit Function
					End If
					
					pname = UDList.Item(edata).FeatureData("追加パイロット")
					If Not PDList.IsDefined(pname) Then
						ErrorMessage("追加パイロット「" & pname & "」のデータがありません")
						Exit Function
					End If
					
					'召喚したユニットを配置する座標を決定する。
					'最も近い敵ユニットの方向にユニットを配置する。
					u = SearchNearestEnemy(Me)
					If Not u Is Nothing Then
						If System.Math.Abs(x - u.x) > System.Math.Abs(y - u.y) Then
							If x < u.x Then
								tx = x + 1
							ElseIf x > u.x Then 
								tx = x - 1
							Else
								tx = x
							End If
							ty = y
							
							tx2 = x
							If y < u.y Then
								ty2 = y + 1
							ElseIf y > u.y Then 
								ty2 = y - 1
							Else
								If y = 1 Then
									If MapDataForUnit(x, 2) Is Nothing Then
										ty2 = 2
									Else
										ty2 = 1
									End If
								ElseIf y = MapHeight Then 
									If MapDataForUnit(x, MapHeight - 1) Is Nothing Then
										ty2 = MapHeight - 1
									Else
										ty2 = MapHeight
									End If
								Else
									If MapDataForUnit(x, y - 1) Is Nothing Then
										ty2 = y - 1
									ElseIf MapDataForUnit(x, y + 1) Is Nothing Then 
										ty2 = y - 1
									Else
										ty2 = y
									End If
								End If
							End If
						Else
							tx = x
							If y < u.y Then
								ty = y + 1
							ElseIf y > u.y Then 
								ty = y - 1
							Else
								ty = y
							End If
							
							If x < u.x Then
								tx2 = x + 1
							ElseIf x > u.x Then 
								tx2 = x - 1
							Else
								If x = 1 Then
									If MapDataForUnit(2, y) Is Nothing Then
										tx2 = 2
									Else
										tx2 = 1
									End If
								ElseIf x = MapWidth Then 
									If MapDataForUnit(MapWidth - 1, y) Is Nothing Then
										tx2 = MapWidth - 1
									Else
										tx2 = MapWidth
									End If
								Else
									If MapDataForUnit(x - 1, y) Is Nothing Then
										tx2 = x - 1
									ElseIf MapDataForUnit(x + 1, y) Is Nothing Then 
										tx2 = x + 1
									Else
										tx2 = x
									End If
								End If
							End If
							ty2 = y
						End If
					Else
						tx = x
						ty = y
						tx2 = x
						ty2 = y
					End If
					
					For j = 1 To MaxLng(elevel, 1)
						If InStr(PDList.Item(pname).Name, "(ザコ)") > 0 Or InStr(PDList.Item(pname).Name, "(汎用)") > 0 Then
							p = PList.Add(pname, MainPilot.Level, Party)
							p.FullRecover()
							u = UList.Add(edata, Rank, Party)
						Else
							If Not PList.IsDefined(pname) Then
								p = PList.Add(pname, MainPilot.Level, Party)
								p.FullRecover()
								u = UList.Add(edata, Rank, Party)
							Else
								p = PList.Item(pname)
								u = p.Unit_Renamed
								If u Is Nothing Then
									If UList.IsDefined(edata) Then
										u = UList.Item(edata)
									Else
										u = UList.Add(edata, Rank, Party)
									End If
								End If
							End If
						End If
						p.Ride(u)
						AddServant(u)
						
						If Party = "味方" Then
							If LIndex(u.FeatureData("召喚ユニット"), 2) = "ＮＰＣ" Then
								u.ChangeParty("ＮＰＣ")
							End If
						End If
						
						With u
							.Summoner = CurrentForm
							.FullRecover()
							.Mode = MainPilot.ID
							.UsedAction = 0
							If .IsFeatureAvailable("制限時間") Then
								.AddCondition("残り時間", CShort(.FeatureData("制限時間")))
							End If
							
							If .IsMessageDefined("発進") Then
								If Not frmMessage.Visible Then
									OpenMessageForm(Me)
								End If
								.PilotMessage("発進")
							End If
							
							'ユニットを配置
							If MapDataForUnit(tx, ty) Is Nothing And .IsAbleToEnter(tx, ty) Then
								.StandBy(tx, ty, "出撃")
							ElseIf MapDataForUnit(tx2, ty2) Is Nothing And .IsAbleToEnter(tx2, ty2) Then 
								.StandBy(tx2, ty2, "出撃")
							Else
								.StandBy(x, y, "出撃")
							End If
							
							'ちゃんと配置できた？
							If .Status_Renamed = "待機" Then
								'空いた場所がなく出撃出来なかった場合
								DisplaySysMessage(Nickname & "は" & .Nickname & "の召喚に失敗した。")
								DeleteServant(.ID)
								.Status_Renamed = "破棄"
							End If
						End With
					Next 
					
				Case "変身"
					'既に変身している場合は変身出来ない
					If t.IsFeatureAvailable("ノーマルモード") Then
						GoTo NextLoop
					End If
					
					buf = t.Name
					
					t.Transform(LIndex(edata, 1))
					t = t.CurrentForm
					If elevel2 > 0 Then
						t.AddCondition("残り時間", MaxLng(CShort(elevel2), 1))
					End If
					
					For j = 2 To LLength(edata)
						buf = buf & " " & LIndex(edata, j)
					Next 
					t.AddCondition("ノーマルモード付加", -1, 1, buf)
					
					'変身した場合はそこで終わり
					Exit For
					
				Case "能力コピー"
					'既に変身している場合は能力コピー出来ない
					If IsFeatureAvailable("ノーマルモード") Then
						GoTo NextLoop
					End If
					
					Transform((t.Name))
					
					With CurrentForm
						If elevel2 > 0 Then
							.AddCondition("残り時間", MaxLng(CShort(elevel2), 1))
						End If
						
						'元の形態に戻れるように設定
						buf = Name
						For j = 1 To LLength(edata)
							buf = buf & " " & LIndex(edata, j)
						Next 
						.AddCondition("ノーマルモード付加", -1, 1, buf)
						.AddCondition("能力コピー", -1)
						
						'コピー元のパイロット画像とメッセージを使うように設定
						.AddCondition("パイロット画像", -1, 0, "非表示 " & t.MainPilot.Bitmap)
						.AddCondition("メッセージ", -1, 0, "非表示 " & t.MainPilot.MessageType)
					End With
					
					'能力コピーした場合はそこで終わり
					ExecuteAbility = True
					RestoreSelections()
					Exit Function
					
				Case "再行動"
					If Not t Is CurrentForm Then
						If t.Action = 0 And t.MaxAction > 0 Then
							If t.UsedAction > t.MaxAction Then
								t.UsedAction = t.MaxAction
							End If
							t.UsedAction = t.UsedAction - 1
							DisplaySysMessage(t.Nickname & "を行動可能にした。")
							is_useful = True
						End If
					Else
						t.UsedAction = t.UsedAction - 1
					End If
			End Select
NextLoop: 
		Next 
		
		t.CurrentForm.Update()
		t.CurrentForm.CheckAutoHyperMode()
		t.CurrentForm.CheckAutoNormalMode()
		
		ExecuteAbility = is_useful
		
Finish: 
		
		'選択状況を復元
		RestoreSelections()
		
		'マップアビリティの場合、これ以降の処理は必要なし
		If is_map_ability Then
			Exit Function
		End If
		
		'合体技のパートナーの弾数＆ＥＮの消費
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountAbility
					'パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
					If .Ability(j).Name = aname Then
						.UseAbility(j)
						If .IsAbilityClassifiedAs(j, "自") Then
							If .IsFeatureAvailable("パーツ分離") Then
								uname = LIndex(.FeatureData("パーツ分離"), 2)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
									With .CurrentForm
										.HP = .MaxHP
										.UsedAction = .MaxAction
									End With
								Else
									.Die()
								End If
							Else
								.Die()
							End If
						ElseIf .IsAbilityClassifiedAs(j, "失") And .HP = 0 Then 
							.Die()
						ElseIf .IsAbilityClassifiedAs(j, "変") Then 
							If .IsFeatureAvailable("変形技") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "変形技" And LIndex(.FeatureData(k), 1) = aname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("ノーマルモード") Then
										uname = LIndex(.FeatureData("ノーマルモード"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("ノーマルモード") Then 
								uname = LIndex(.FeatureData("ノーマルモード"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'同名のアビリティがなかった場合は自分のデータを使って処理
				If j > .CountAbility Then
					If Ability(a).ENConsumption > 0 Then
						.EN = .EN - AbilityENConsumption(a)
					End If
					If IsAbilityClassifiedAs(a, "消") Then
						.AddCondition("消耗", 1)
					End If
					If IsAbilityClassifiedAs(a, "Ｃ") And .IsConditionSatisfied("チャージ完了") Then
						.DeleteCondition("チャージ完了")
					End If
					If IsAbilityClassifiedAs(a, "気") Then
						.IncreaseMorale(-5 * AbilityLevel(a, "気"))
					End If
					If IsAbilityClassifiedAs(a, "霊") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "霊")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsAbilityClassifiedAs(a, "プ") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "プ")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsAbilityClassifiedAs(a, "失") Then
						.HP = MaxLng(.HP - .MaxHP * AbilityLevel(a, "失") \ 10, 0)
					End If
					If IsAbilityClassifiedAs(a, "自") Then
						If .IsFeatureAvailable("パーツ分離") Then
							uname = LIndex(.FeatureData("パーツ分離"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
							Else
								.Die()
							End If
						Else
							.Die()
						End If
					ElseIf IsAbilityClassifiedAs(a, "失") And .HP = 0 Then 
						.Die()
					ElseIf IsAbilityClassifiedAs(a, "変") Then 
						If .IsFeatureAvailable("ノーマルモード") Then
							uname = LIndex(.FeatureData("ノーマルモード"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'変身した場合
		If Status_Renamed = "他形態" Then
			With CurrentForm
				'使い捨てアイテムによる変身の処理
				For i = 1 To .CountAbility
					If .Ability(i).Name = aname Then
						'アイテムを消費
						If .Ability(i).IsItem And .Stock(i) = 0 And .MaxStock(i) > 0 Then
							For j = 1 To .CountItem
								For k = 1 To .Item(j).CountAbility
									If .Item(j).Ability(k).Name = aname Then
										.Item(j).Exist = False
										.DeleteItem(j)
										.Update()
										GoTo ExitLoop
									End If
								Next 
							Next 
						End If
					End If
				Next 
ExitLoop: 
				
				'自殺？
				If .HP = 0 Then
					.Die()
				End If
			End With
			
			'WaitCommandによる画面クリアが行われないので
			RedrawScreen()
			Exit Function
		End If
		
		'経験値の獲得
		If is_useful And Not is_event And Not IsOptionDefined("アビリティ経験値無効") Then
			GetExp(t, "アビリティ")
			If Not IsOptionDefined("合体技パートナー経験値無効") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(t, "アビリティ", "パートナー")
				Next 
			End If
		End If
		
		'以下の効果はアビリティデータが変化する場合があるため同時には適応されない
		
		'自爆技
		If IsAbilityClassifiedAs(a, "自") Then
			If IsFeatureAvailable("パーツ分離") Then
				uname = LIndex(FeatureData("パーツ分離"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("パーツ分離")
					If IsSysMessageDefined("破壊時分離(" & Name & ")") Then
						SysMessage("破壊時分離(" & Name & ")")
					ElseIf IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
						SysMessage("破壊時分離(" & fname & ")")
					ElseIf IsSysMessageDefined("破壊時分離") Then 
						SysMessage("破壊時分離")
					ElseIf IsSysMessageDefined("分離(" & Name & ")") Then 
						SysMessage("分離(" & Name & ")")
					ElseIf IsSysMessageDefined("分離(" & fname & ")") Then 
						SysMessage("分離(" & fname & ")")
					ElseIf IsSysMessageDefined("分離") Then 
						SysMessage("分離")
					Else
						DisplaySysMessage(Nickname & "は破壊されたパーツを分離させた。")
					End If
				Else
					Die()
				End If
			Else
				Die()
			End If
			
			'ＨＰ消費アビリティで自殺
		ElseIf IsAbilityClassifiedAs(a, "失") And HP = 0 Then 
			Die()
			
			'変形技
		ElseIf IsAbilityClassifiedAs(a, "変") Then 
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = Ability(a).Name Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("ノーマルモード") Then
						uname = LIndex(FeatureData("ノーマルモード"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				uname = LIndex(FeatureData("ノーマルモード"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'アイテムを消費
		ElseIf Ability(a).IsItem And Stock(a) = 0 And MaxStock(a) > 0 Then 
			'アイテムを削除
			num = Data.CountAbility
			num = num + MainPilot.Data.CountAbility
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountAbility
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountAbility
			Next 
			If IsFeatureAvailable("追加サポート") Then
				num = num + AdditionalSupport.Data.CountAbility
			End If
			For	Each itm In colItem
				num = num + itm.CountAbility
				If a <= num Then
					itm.Exist = False
					DeleteItem((itm.ID))
					Exit For
				End If
			Next itm
		End If
		
		' ADD START MARGE
		'戦闘アニメ終了処理
		If IsAnimationDefined(aname & "(終了)") Then
			PlayAnimation(aname & "(終了)")
		ElseIf IsAnimationDefined("終了") Then 
			PlayAnimation("終了")
		End If
		' ADD END MARGE
		
		With CurrentForm
			'戦闘アニメで変更されたユニット画像を元に戻す
			If .IsConditionSatisfied("ユニット画像") Then
				.DeleteCondition("ユニット画像")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("非表示付加") Then
				.DeleteCondition("非表示付加")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
		End With
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("ユニット画像") Then
					.DeleteCondition("ユニット画像")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("非表示付加") Then
					.DeleteCondition("非表示付加")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
	End Function
	
	'マップアビリティ a を (tx,ty) に使用
	Public Sub ExecuteMapAbility(ByVal a As Short, ByVal tx As Short, ByVal ty As Short, Optional ByVal is_event As Boolean = False)
		Dim k, i, j, num As Short
		Dim t, max_lv_t As Unit
		Dim targets() As Unit
		Dim partners() As Unit
		Dim is_useful As Boolean
		Dim anickname, aname, msg As String
		Dim min_range, max_range As Short
		Dim rx, ry As Short
		Dim uname, fname As String
		Dim hp_ratio, en_ratio As Double
		Dim itm As Item
		
		aname = Ability(a).Name
		anickname = AbilityNickname(a)
		
		If Not is_event Then
			'マップ攻撃の使用イベント
			HandleEvent("使用", MainPilot.ID, aname)
			If IsScenarioFinished Then
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				Exit Sub
			End If
		End If
		
		'効果範囲を設定
		min_range = AbilityMinRange(a)
		max_range = AbilityMaxRange(a)
		If IsAbilityClassifiedAs(a, "Ｍ直") Then
			If ty < y Then
				AreaInLine(x, y, min_range, max_range, "N")
			ElseIf ty > y Then 
				AreaInLine(x, y, min_range, max_range, "S")
			ElseIf tx < x Then 
				AreaInLine(x, y, min_range, max_range, "W")
			Else
				AreaInLine(x, y, min_range, max_range, "E")
			End If
		ElseIf IsAbilityClassifiedAs(a, "Ｍ拡") Then 
			If ty < y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then
				AreaInCone(x, y, min_range, max_range, "N")
			ElseIf ty > y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then 
				AreaInCone(x, y, min_range, max_range, "S")
			ElseIf tx < x And System.Math.Abs(x - tx) > System.Math.Abs(y - ty) Then 
				AreaInCone(x, y, min_range, max_range, "W")
			Else
				AreaInCone(x, y, min_range, max_range, "E")
			End If
		ElseIf IsAbilityClassifiedAs(a, "Ｍ扇") Then 
			If ty < y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then
				AreaInSector(x, y, min_range, max_range, "N", AbilityLevel(a, "Ｍ扇"))
			ElseIf ty > y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then 
				AreaInSector(x, y, min_range, max_range, "S", AbilityLevel(a, "Ｍ扇"))
			ElseIf tx < x And System.Math.Abs(x - tx) >= System.Math.Abs(y - ty) Then 
				AreaInSector(x, y, min_range, max_range, "W", AbilityLevel(a, "Ｍ扇"))
			Else
				AreaInSector(x, y, min_range, max_range, "E", AbilityLevel(a, "Ｍ扇"))
			End If
		ElseIf IsAbilityClassifiedAs(a, "Ｍ投") Then 
			AreaInRange(tx, ty, AbilityLevel(a, "Ｍ投"), 1, "すべて")
		ElseIf IsAbilityClassifiedAs(a, "Ｍ全") Then 
			AreaInRange(x, y, max_range, min_range, "すべて")
		ElseIf IsAbilityClassifiedAs(a, "Ｍ移") Or IsAbilityClassifiedAs(a, "Ｍ線") Then 
			AreaInPointToPoint(x, y, tx, ty)
		End If
		
		'ユニットがいるマスの処理
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				If Not MaskData(i, j) Then
					t = MapDataForUnit(i, j)
					If Not t Is Nothing Then
						'有効？
						If IsAbilityEffective(a, t) Then
							MaskData(i, j) = False
						Else
							MaskData(i, j) = True
						End If
					End If
				End If
			Next 
		Next 
		
		'支援専用アビリティは自分には使用できない
		If IsAbilityClassifiedAs(a, "援") Then
			MaskData(x, y) = True
		End If
		
		'マップアビリティの影響を受けるユニットのリストを作成
		ReDim targets(0)
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'マップアビリティの影響をうけるかチェック
				If MaskData(i, j) Then
					GoTo NextLoop
				End If
				t = MapDataForUnit(i, j)
				If t Is Nothing Then
					GoTo NextLoop
				End If
				If Not IsAbilityApplicable(a, t) Then
					MaskData(i, j) = True
					GoTo NextLoop
				End If
				
				ReDim Preserve targets(UBound(targets) + 1)
				targets(UBound(targets)) = t
NextLoop: 
			Next 
		Next 
		
		'アビリティ実行の起点を設定
		If IsAbilityClassifiedAs(a, "Ｍ投") Then
			rx = tx
			ry = ty
		Else
			rx = x
			ry = y
		End If
		
		'起点からの距離に応じて並べ替え
		Dim min_item, min_value As Short
		For i = 1 To UBound(targets) - 1
			
			min_item = i
			With targets(i)
				min_value = System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry)
			End With
			For j = i + 1 To UBound(targets)
				With targets(j)
					If System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry) < min_value Then
						min_item = j
						min_value = System.Math.Abs(.x - rx) + System.Math.Abs(.y - ry)
					End If
				End With
			Next 
			If min_item <> i Then
				t = targets(i)
				targets(i) = targets(min_item)
				targets(min_item) = t
			End If
		Next 
		
		'合体技
		Dim TmpMaskData() As Boolean
		If IsAbilityClassifiedAs(a, "合") Then
			
			'合体技のパートナーのハイライト表示
			'MaskDataを保存して使用している
			ReDim TmpMaskData(MapWidth, MapHeight)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					TmpMaskData(i, j) = MaskData(i, j)
				Next 
			Next 
			
			CombinationPartner("アビリティ", a, partners)
			
			'パートナーユニットはマスクを解除
			For i = 1 To UBound(partners)
				With partners(i)
					MaskData(.x, .y) = False
					TmpMaskData(.x, .y) = True
				End With
			Next 
			
			MaskScreen()
			
			'マスクを復元
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = TmpMaskData(i, j)
				Next 
			Next 
		Else
			ReDim partners(0)
			ReDim SelectedPartners(0)
			MaskScreen()
		End If
		
		OpenMessageForm(Me)
		
		'現在の選択状況をセーブ
		SaveSelections()
		
		'選択内容を切り替え
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedAbility = a
		SelectedAbilityName = Ability(a).Name
		SelectedX = tx
		SelectedY = ty
		
		'変な「対〜」メッセージが表示されないようにターゲットをオフ
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTargetForEvent = Nothing
		
		'マップアビリティ開始のメッセージ＆特殊効果
		If IsAnimationDefined(aname & "(準備)") Then
			PlayAnimation(aname & "(準備)")
		End If
		If IsMessageDefined("かけ声(" & aname & ")") Then
			PilotMessage("かけ声(" & aname & ")")
		End If
		PilotMessage(aname, "アビリティ")
		If IsAnimationDefined(aname & "(使用)") Then
			PlayAnimation(aname & "(使用)", "", True)
		Else
			SpecialEffect(aname, "", True)
		End If
		
		'ＥＮ消費＆使用回数減少
		UseAbility(a)
		
		UpdateMessageForm(Me)
		
		Select Case UBound(partners)
			Case 0
				'通常
				msg = Nickname & "は"
			Case 1
				'２体合体
				If Nickname <> partners(1).Nickname Then
					msg = Nickname & "は[" & partners(1).Nickname & "]と共に"
				ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
					msg = MainPilot.Nickname & "と[" & partners(1).MainPilot.Nickname & "]の[" & Nickname & "]は"
				Else
					msg = Nickname & "達は"
				End If
			Case 2
				'３体合体
				If Nickname <> partners(1).Nickname Then
					msg = Nickname & "は[" & partners(1).Nickname & "]、[" & partners(2).Nickname & "]と共に"
				ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
					msg = MainPilot.Nickname & "、[" & partners(1).MainPilot.Nickname & "]、[" & partners(2).MainPilot.Nickname & "]の[" & Nickname & "]は"
				Else
					msg = Nickname & "達は"
				End If
			Case Else
				'３体以上
				msg = Nickname & "達は"
		End Select
		
		If IsSpellAbility(a) Then
			If Right(anickname, 2) = "呪文" Then
				msg = msg & "[" & anickname & "]を唱えた。"
			ElseIf Right(anickname, 2) = "の杖" Then 
				msg = msg & "[" & Left(anickname, Len(anickname) - 2) & "]の呪文を唱えた。"
			Else
				msg = msg & "[" & anickname & "]の呪文を唱えた。"
			End If
		ElseIf Right(anickname, 1) = "歌" Then 
			msg = msg & "[" & anickname & "]を歌った。"
		ElseIf Right(anickname, 2) = "踊り" Then 
			msg = msg & "[" & anickname & "]を踊った。"
		Else
			msg = msg & "[" & anickname & "]を使った。"
		End If
		
		If IsSysMessageDefined(aname) Then
			'「アビリティ名(解説)」のメッセージを使用
			SysMessage(aname)
		ElseIf IsSysMessageDefined("アビリティ") Then 
			'「アビリティ(解説)」のメッセージを使用
			SysMessage("アビリティ")
		Else
			DisplaySysMessage(msg)
		End If
		
		'選択状況を復元
		RestoreSelections()
		
		'アビリティの使用に失敗？
		If Dice(10) <= AbilityLevel(a, "難") Then
			DisplaySysMessage("しかし何もおきなかった…")
			GoTo Finish
		End If
		
		'使用元ユニットは SelectedTarget に設定していないといけない
		SelectedTarget = Me
		
		'各ユニットにアビリティを使用
		'UPGRADE_NOTE: オブジェクト max_lv_t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		max_lv_t = Nothing
		For i = 1 To UBound(targets)
			t = targets(i).CurrentForm
			If t.Status_Renamed = "出撃" Then
				If t Is Me Then
					UpdateMessageForm(Me)
				Else
					UpdateMessageForm(t, Me)
				End If
				
				If ExecuteAbility(a, t, True) Then
					t = t.CurrentForm
					
					is_useful = True
					
					'獲得経験値算出用にメインパイロットのレベルが最も高い
					'ユニットを求めておく
					If max_lv_t Is Nothing Then
						max_lv_t = t
					Else
						If t.MainPilot.Level > max_lv_t.MainPilot.Level Then
							max_lv_t = t
						End If
					End If
				End If
			End If
		Next 
		
		' ADD START MARGE
		'戦闘アニメ終了処理
		If IsAnimationDefined(aname & "(終了)") Then
			PlayAnimation(aname & "(終了)")
		ElseIf IsAnimationDefined("終了") Then 
			PlayAnimation("終了")
		End If
		' ADD END MARGE
		
		With CurrentForm
			'戦闘アニメで変更されたユニット画像を元に戻す
			If .IsConditionSatisfied("ユニット画像") Then
				.DeleteCondition("ユニット画像")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("非表示付加") Then
				.DeleteCondition("非表示付加")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
		End With
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("ユニット画像") Then
					.DeleteCondition("ユニット画像")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("非表示付加") Then
					.DeleteCondition("非表示付加")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
		
		'獲得した経験値の表示
		If is_useful And Not is_event And Not IsOptionDefined("アビリティ経験値無効") Then
			GetExp(max_lv_t, "アビリティ")
			If Not IsOptionDefined("合体技パートナー経験値無効") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(Nothing, "アビリティ", "パートナー")
				Next 
			End If
		End If
		
		'合体技のパートナーの弾数＆ＥＮの消費
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountAbility
					'パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
					If .Ability(j).Name = aname Then
						.UseAbility(j)
						If .IsAbilityClassifiedAs(j, "自") Then
							If .IsFeatureAvailable("パーツ分離") Then
								uname = LIndex(.FeatureData("パーツ分離"), 2)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
									With .CurrentForm
										.HP = .MaxHP
										.UsedAction = .MaxAction
									End With
								Else
									.Die()
								End If
							Else
								.Die()
							End If
						ElseIf .IsAbilityClassifiedAs(j, "失") And .HP = 0 Then 
							.Die()
						ElseIf .IsAbilityClassifiedAs(j, "変") Then 
							If .IsFeatureAvailable("変形技") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "変形技" And LIndex(.FeatureData(k), 1) = aname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("ノーマルモード") Then
										uname = LIndex(.FeatureData("ノーマルモード"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("ノーマルモード") Then 
								uname = LIndex(.FeatureData("ノーマルモード"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'同名のアビリティがなかった場合は自分のデータを使って処理
				If j > .CountAbility Then
					If Ability(a).ENConsumption > 0 Then
						.EN = .EN - AbilityENConsumption(a)
					End If
					If IsAbilityClassifiedAs(a, "消") Then
						.AddCondition("消耗", 1)
					End If
					If IsAbilityClassifiedAs(a, "Ｃ") And .IsConditionSatisfied("チャージ完了") Then
						.DeleteCondition("チャージ完了")
					End If
					If IsAbilityClassifiedAs(a, "気") Then
						.IncreaseMorale(-5 * AbilityLevel(a, "気"))
					End If
					If IsAbilityClassifiedAs(a, "霊") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "霊")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsAbilityClassifiedAs(a, "プ") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "プ")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsAbilityClassifiedAs(a, "失") Then
						.HP = MaxLng(.HP - .MaxHP * AbilityLevel(a, "失") \ 10, 0)
					End If
					If IsAbilityClassifiedAs(a, "自") Then
						If .IsFeatureAvailable("パーツ分離") Then
							uname = LIndex(.FeatureData("パーツ分離"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
							Else
								.Die()
							End If
						Else
							.Die()
						End If
					ElseIf IsAbilityClassifiedAs(a, "失") And .HP = 0 Then 
						.Die()
					ElseIf IsAbilityClassifiedAs(a, "変") Then 
						If .IsFeatureAvailable("ノーマルモード") Then
							uname = LIndex(.FeatureData("ノーマルモード"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'移動型マップアビリティによる移動
		If IsAbilityClassifiedAs(a, "Ｍ移") Then
			Jump(tx, ty)
		End If
		
Finish: 
		
		'以下の効果はアビリティデータが変化する可能性があるため、同時には適用されない
		
		'自爆の処理
		If IsAbilityClassifiedAs(a, "自") Then
			If IsFeatureAvailable("パーツ分離") Then
				'パーツ合体したユニットが自爆する時はパーツを分離するだけ
				uname = LIndex(FeatureData("パーツ分離"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("パーツ分離")
					If IsSysMessageDefined("破壊時分離(" & Name & ")") Then
						SysMessage("破壊時分離(" & Name & ")")
					ElseIf IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
						SysMessage("破壊時分離(" & fname & ")")
					ElseIf IsSysMessageDefined("破壊時分離") Then 
						SysMessage("破壊時分離")
					ElseIf IsSysMessageDefined("分離(" & Name & ")") Then 
						SysMessage("分離(" & Name & ")")
					ElseIf IsSysMessageDefined("分離(" & fname & ")") Then 
						SysMessage("分離(" & fname & ")")
					ElseIf IsSysMessageDefined("分離") Then 
						SysMessage("分離")
					Else
						DisplaySysMessage(Nickname & "は破壊されたパーツを分離させた。")
					End If
				Else
					'しかし、パーツ分離できない地形ではそのまま自爆
					Die()
					If Not is_event Then
						HandleEvent("破壊", MainPilot.ID)
						If IsScenarioFinished Then
							Exit Sub
						End If
					End If
				End If
			Else
				Die()
				If Not is_event Then
					HandleEvent("破壊", MainPilot.ID)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
			End If
			
			'ＨＰ消費アビリティで自殺した場合
		ElseIf IsAbilityClassifiedAs(a, "失") And HP = 0 Then 
			Die()
			If Not is_event Then
				HandleEvent("破壊", MainPilot.ID)
				If IsScenarioFinished Then
					Exit Sub
				End If
			End If
			
			'変形技
		ElseIf IsAbilityClassifiedAs(a, "変") Then 
			If IsFeatureAvailable("変形技") Then
				For i = 1 To CountFeature
					If Feature(i) = "変形技" And LIndex(FeatureData(i), 1) = Ability(a).Name Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("ノーマルモード") Then
						uname = LIndex(FeatureData("ノーマルモード"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("ノーマルモード") Then 
				uname = LIndex(FeatureData("ノーマルモード"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'アイテムを消費
		ElseIf Ability(a).IsItem And Stock(a) = 0 And MaxStock(a) > 0 Then 
			'アイテムを削除
			num = Data.CountAbility
			num = num + MainPilot.Data.CountAbility
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountAbility
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountAbility
			Next 
			If IsFeatureAvailable("追加サポート") Then
				num = num + AdditionalSupport.Data.CountAbility
			End If
			For	Each itm In colItem
				num = num + itm.CountAbility
				If a <= num Then
					itm.Exist = False
					DeleteItem((itm.ID))
					Exit For
				End If
			Next itm
		End If
		
		'使用後イベント
		If Not is_event Then
			HandleEvent("使用後", CurrentForm.MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		
		CloseMessageForm()
		
		'ハイパーモード＆ノーマルモードの自動発動をチェック
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
	End Sub
	
	'アビリティの使用によるＥＮ、使用回数の消費等を行う
	Public Sub UseAbility(ByVal a As Short)
		Dim i, lv As Short
		Dim hp_ratio, en_ratio As Double
		
		If Ability(a).ENConsumption > 0 Then
			EN = EN - AbilityENConsumption(a)
		End If
		
		If Ability(a).Stock > 0 Then
			SetStock(a, Stock(a) - 1)
			
			'一斉使用
			If IsAbilityClassifiedAs(a, "斉") Then
				For i = 1 To UBound(dblStock)
					SetStock(i, MinLng(MaxStock(i) * Stock(a) \ MaxStock(a), Stock(i)))
				Next 
			Else
				For i = 1 To UBound(dblStock)
					If IsAbilityClassifiedAs(i, "斉") Then
						SetStock(i, MinLng(CShort(MaxStock(i) * Stock(a) \ MaxStock(a) + 0.49999), Stock(i)))
					End If
				Next 
			End If
			
			'弾数・使用回数共有の処理
			SyncBullet()
		End If
		
		'消耗技
		If IsAbilityClassifiedAs(a, "消") Then
			AddCondition("消耗", 1)
		End If
		
		'全ＥＮ消費アビリティ
		If IsAbilityClassifiedAs(a, "尽") Then
			EN = 0
		End If
		
		'チャージ式アビリティ
		If IsAbilityClassifiedAs(a, "Ｃ") And IsConditionSatisfied("チャージ完了") Then
			DeleteCondition("チャージ完了")
		End If
		
		'自動充填式アビリティ
		If AbilityLevel(a, "Ａ") > 0 Then
			AddCondition(AbilityNickname(a) & "充填中", AbilityLevel(a, "Ａ"))
		End If
		
		'気力を消費
		If IsAbilityClassifiedAs(a, "気") Then
			IncreaseMorale(-5 * AbilityLevel(a, "気"))
		End If
		
		'霊力の消費
		If IsAbilityClassifiedAs(a, "霊") Then
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * AbilityLevel(a, "霊")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		ElseIf IsAbilityClassifiedAs(a, "プ") Then 
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * AbilityLevel(a, "プ")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		End If
		
		'資金消費アビリティ
		If Party = "味方" Then
			If IsAbilityClassifiedAs(a, "銭") Then
				IncrMoney(-MaxLng(AbilityLevel(a, "銭"), 1) * Value \ 10)
			End If
		End If
		
		'ＨＰ消費アビリティ
		If IsAbilityClassifiedAs(a, "失") Then
			HP = MaxLng(HP - MaxHP * AbilityLevel(a, "失") \ 10, 0)
		End If
	End Sub
	
	
	' === アイテム関連処理 ===
	
	'アイテム装備可能数
	Public Function MaxItemNum() As Short
		Dim i As Short
		
		MaxItemNum = Data.ItemNum
		If IsFeatureAvailable("ハードポイント") Then
			For i = 1 To CountFeature
				If Feature(i) = "ハードポイント" And (FeatureData(i) = "強化パーツ" Or FeatureData(i) = "アイテム") Then
					MaxItemNum = MaxItemNum + FeatureLevel(i)
					Exit For
				End If
			Next 
		End If
	End Function
	
	'装備しているアイテムの総数
	Public Function CountItem() As Short
		CountItem = colItem.Count()
	End Function
	
	'アイテム
	Public Function Item(ByRef Index As Object) As Item
		Dim itm As Item
		
		On Error GoTo ErrorHandler
		
		Item = colItem.Item(Index)
		
		Exit Function
		
ErrorHandler: 
		'見つからなければアイテム名で検索
		For	Each itm In colItem
			'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If itm.Name = Index Then
				Item = itm
				Exit Function
			End If
		Next itm
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'アイテムを装備
	Public Sub AddItem(ByRef itm As Item, Optional ByVal without_refresh As Boolean = False)
		Dim i, num As Short
		Dim itm2 As Item
		Dim empty_slot As Short
		Dim found_item As Boolean
		
		'既に装備していたらそのまま終了
		If itm.Unit Is Me Then
			Exit Sub
		End If
		
		'イベント専用アイテムは装備個所を消費しない
		If itm.Class_Renamed() = "固定" Then
			If itm.IsFeatureAvailable("非表示") Then
				GoTo EquipItem
			End If
		End If
		
		'装備個所が足りない場合に元のアイテムを外す
		Select Case itm.Part
			Case "強化パーツ", "アイテム"
				If itm.FeatureData("ハードポイント") <> "強化パーツ" And itm.FeatureData("ハードポイント") <> "アイテム" Then
					'装備している強化パーツ数をカウント
					num = 0
					For	Each itm2 In colItem
						With itm2
							If .Part = "強化パーツ" Or .Part = "アイテム" Then
								num = num + .Size
							End If
						End With
					Next itm2
					
					'大型アイテムの場合は余分に外す
					num = num + itm.FeatureLevel("大型アイテム")
					
					'何れかを外さなければならない場合
					Do While num >= MaxItemNum And num > 0
						found_item = False
						
						'まずはハードポイントを持たないものから
						For	Each itm2 In colItem
							With itm2
								If .Part = "強化パーツ" Or .Part = "アイテム" Then
									If Not .IsFeatureAvailable("ハードポイント") Then
										num = num - .Size
										If Party0 <> "味方" Then
											.Exist = False
										End If
										DeleteItem(.ID)
										'UPGRADE_NOTE: オブジェクト itm2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
										itm2 = Nothing
										found_item = True
										Exit For
									End If
								End If
							End With
						Next itm2
						
						'ハードポイント付きのものしかない場合
						If Not itm2 Is Nothing Then
							num = num - Item(1).Size
							If Party0 <> "味方" Then
								Item(1).Exist = False
							End If
							DeleteItem(1)
							found_item = True
						End If
						
						If Not found_item Then
							'外せるアイテムがない
							Exit Do
						End If
					Loop 
					
					If MaxItemNum = 0 Then
						'装備出来ません…
						Exit Sub
					End If
				End If
				
			Case "両手"
				For	Each itm2 In colItem
					With itm2
						If .Part = "両手" Or .Part = "片手" Or .Part = "盾" Then
							If Party0 <> "味方" Then
								.Exist = False
							End If
							DeleteItem(.ID)
							Exit For
						End If
					End With
				Next itm2
				
			Case "片手"
				If IsFeatureAvailable("両手利き") Then
					num = 0
					For	Each itm2 In colItem
						With itm2
							Select Case .Part
								Case "両手"
									If Party0 <> "味方" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								Case "片手", "盾"
									num = num + 1
									If num > 1 Then
										If Party0 <> "味方" Then
											.Exist = False
										End If
										DeleteItem(.ID)
										Exit For
									End If
							End Select
						End With
					Next itm2
				Else
					For	Each itm2 In colItem
						With itm2
							Select Case .Part
								Case "両手", "片手"
									If Party0 <> "味方" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
							End Select
						End With
					Next itm2
				End If
				
			Case "盾"
				For	Each itm2 In colItem
					With itm2
						Select Case .Part
							Case "両手", "盾"
								If Party0 <> "味方" Then
									.Exist = False
								End If
								DeleteItem(.ID)
								Exit For
							Case "片手"
								i = i + 1
								If i > 1 Then
									If Party0 <> "味方" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								End If
						End Select
					End With
				Next itm2
				
			Case "両肩"
				For	Each itm2 In colItem
					With itm2
						If .Part = "両肩" Or .Part = "肩" Then
							If Party0 <> "味方" Then
								.Exist = False
							End If
							DeleteItem(.ID)
						End If
					End With
				Next itm2
				
			Case "肩"
				num = 0
				For	Each itm2 In colItem
					With itm2
						Select Case .Part
							Case "両肩"
								If Party0 <> "味方" Then
									.Exist = False
								End If
								DeleteItem(.ID)
								Exit For
							Case "肩"
								num = num + 1
								If num > 1 Then
									If Party0 <> "味方" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								End If
						End Select
					End With
				Next itm2
				
			Case "非表示"
				'装備個所が「非表示」のアイテムは装備数に制限なし
				
			Case Else
				'ハードポイントに装備する場合
				For i = 1 To CountFeature
					If Feature(i) = "ハードポイント" And FeatureData(i) = itm.Part Then
						'まず空きスロット数を計算
						empty_slot = ItemSlotSize(itm.Part)
						For	Each itm2 In colItem
							With itm2
								If .Part = itm.Part Then
									empty_slot = empty_slot - .Size
								End If
							End With
						Next itm2
						'足らないスロット数分、アイテムを外す
						If empty_slot < itm.Size Then
							For	Each itm2 In colItem
								With itm2
									If .Part = itm.Part Then
										If Party0 <> "味方" Then
											.Exist = False
										End If
										DeleteItem(.ID)
										empty_slot = empty_slot + .Size
										If empty_slot >= itm.Size Then
											Exit For
										End If
									End If
								End With
							Next itm2
						End If
						i = 0
						Exit For
					End If
				Next 
				'そうでない場合
				If i > 0 Then
					For	Each itm2 In colItem
						With itm2
							If itm.Part = .Part Then
								If Party0 <> "味方" Then
									.Exist = False
								End If
								DeleteItem(.ID)
								Exit For
							End If
						End With
					Next itm2
				End If
		End Select
		
EquipItem: 
		
		'装備されたアイテムは常に存在するとみなす
		If Status_Renamed <> "破棄" Then
			itm.Exist = True
		End If
		
		colItem.Add(itm, itm.ID)
		itm.Unit = Me
		
		'アイテムを装備したことによるステータスの変化
		Update(without_refresh)
	End Sub
	
	Public Sub AddItem0(ByRef itm As Item)
		colItem.Add(itm, itm.ID)
		itm.Unit = Me
	End Sub
	
	'アイテムをはずす
	Public Sub DeleteItem(ByRef Index As Object, Optional ByVal without_refresh As Boolean = False)
		Dim itm, itm2 As Item
		Dim num, i, j, num2 As Short
		Dim prev_max_item_num As Short
		Dim prev_hard_point() As String
		Dim prev_hard_point_size() As Short
		Dim cur_hard_point() As String
		Dim cur_hard_point_size() As Short
		Dim is_changed As Boolean
		Dim is_ambidextrous As Boolean
		
		itm = Item(Index)
		
		'存在しないアイテム？
		If itm Is Nothing Then
			Exit Sub
		End If
		
		'削除するアイテムの武器・アビリティの残弾数が引き継がれるのを防ぐため、
		'削除するアイテムによって付加された武器・アビリティのデータを削除する。
		num = Data.CountWeapon
		num2 = Data.CountAbility
		If CountPilot > 0 Then
			num = num + MainPilot.Data.CountWeapon
			num2 = num2 + MainPilot.Data.CountAbility
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountWeapon
				num2 = num2 + Pilot(i).Data.CountAbility
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountWeapon
				num2 = num2 + Support(i).Data.CountAbility
			Next 
			If IsFeatureAvailable("追加サポート") Then
				num = num + AdditionalSupport.Data.CountWeapon
				num2 = num2 + AdditionalSupport.Data.CountAbility
			End If
		End If
		For	Each itm2 In colItem
			With itm2
				If itm Is itm2 Then
					For i = num + 1 To num + .CountWeapon
						If i <= CountWeapon Then
							'UPGRADE_NOTE: オブジェクト WData() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							WData(i) = Nothing
						End If
					Next 
					For i = num2 + 1 To num2 + .CountAbility
						If i <= CountAbility Then
							'UPGRADE_NOTE: オブジェクト adata() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							adata(i) = Nothing
						End If
					Next 
					Exit For
				Else
					num = num + .CountWeapon
					num2 = num2 + .CountAbility
				End If
			End With
		Next itm2
		
		With itm
			colItem.Remove(.ID)
			If Not .Unit Is Nothing Then
				If .Unit.ID = ID Then
					'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					.Unit = Nothing
				End If
				'追加パイロットを持つアイテムを削除する場合
				If itm.IsFeatureAvailable("追加パイロット") Then
					If PList.IsDefined(.FeatureData("追加パイロット")) Then
						With PList.Item(.FeatureData("追加パイロット"))
							.Alive = False
							'UPGRADE_NOTE: オブジェクト PList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							.Unit_Renamed = Nothing
						End With
					End If
				End If
			End If
		End With
		
		'ハードポイントを持つアイテムをはずした場合は他のアイテムを連続してはずす必要がある
		Do 
			is_changed = False
			
			'現在のアイテム装備可能回数を記録
			prev_max_item_num = MaxItemNum
			ReDim prev_hard_point(0)
			ReDim prev_hard_point_size(0)
			For i = 1 To CountFeature
				If Feature(i) = "ハードポイント" Then
					If FeatureData(i) <> "強化パーツ" And FeatureData(i) <> "アイテム" Then
						For j = 1 To UBound(prev_hard_point)
							If prev_hard_point(j) = FeatureData(i) Then
								prev_hard_point_size(j) = prev_hard_point_size(j) + FeatureLevel(i)
								Exit For
							End If
						Next 
						If j > UBound(prev_hard_point) Then
							ReDim Preserve prev_hard_point(UBound(prev_hard_point) + 1)
							ReDim Preserve prev_hard_point_size(UBound(prev_hard_point))
							prev_hard_point(UBound(prev_hard_point)) = FeatureData(i)
							prev_hard_point_size(UBound(prev_hard_point)) = FeatureLevel(i)
						End If
					End If
				End If
			Next 
			is_ambidextrous = IsFeatureAvailable("両手利き")
			
			'アイテムを外したことによるステータスの変化
			Update(without_refresh)
			
			'アイテム装備可能数が減少？
			If prev_max_item_num > MaxItemNum Then
				is_changed = True
				num = MaxItemNum
				
				i = 0
				For	Each itm In colItem
					With itm
						If .Part = "強化パーツ" Or .Part = "アイテム" Then
							i = i + 1
							'ハードポイントを持たないアイテムから選んで削除
							If i > num And Not .IsFeatureAvailable("ハードポイント") Then
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
										.Unit = Nothing
									End If
								End If
								'追加パイロットを持つアイテムを削除する場合
								If .IsFeatureAvailable("追加パイロット") Then
									If PList.IsDefined(.FeatureData("追加パイロット")) Then
										With PList.Item(.FeatureData("追加パイロット"))
											.Alive = False
											'UPGRADE_NOTE: オブジェクト PList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
											.Unit_Renamed = Nothing
										End With
									End If
								End If
							End If
						End If
					End With
				Next itm
				
				i = 0
				For	Each itm In colItem
					With itm
						If .Part = "強化パーツ" Or .Part = "アイテム" Then
							i = i + 1
							If i > num Then
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
										.Unit = Nothing
									End If
								End If
								'追加パイロットを持つアイテムを削除する場合
								If .IsFeatureAvailable("追加パイロット") Then
									If PList.IsDefined(.FeatureData("追加パイロット")) Then
										With PList.Item(.FeatureData("追加パイロット"))
											.Alive = False
											'UPGRADE_NOTE: オブジェクト PList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
											.Unit_Renamed = Nothing
										End With
									End If
								End If
							End If
						End If
					End With
				Next itm
			End If
			
			'現在のアイテム装備可能回数を記録
			ReDim cur_hard_point(0)
			ReDim cur_hard_point_size(0)
			For i = 1 To CountFeature
				If Feature(i) = "ハードポイント" Then
					For j = 1 To UBound(cur_hard_point)
						If cur_hard_point(j) = FeatureData(i) Then
							cur_hard_point_size(j) = cur_hard_point_size(j) + FeatureLevel(i)
							Exit For
						End If
					Next 
					If j > UBound(cur_hard_point) Then
						ReDim Preserve cur_hard_point(UBound(cur_hard_point) + 1)
						ReDim Preserve cur_hard_point_size(UBound(cur_hard_point))
						cur_hard_point(UBound(cur_hard_point)) = FeatureData(i)
						cur_hard_point_size(UBound(cur_hard_point)) = FeatureLevel(i)
					End If
				End If
			Next 
			
			'ハードポイントが減少？
			For i = 1 To UBound(prev_hard_point)
				num = 0
				For j = 1 To UBound(cur_hard_point)
					If prev_hard_point(i) = cur_hard_point(j) Then
						num = cur_hard_point_size(j)
					End If
				Next 
				
				If num < prev_hard_point_size(i) Then
					is_changed = True
					For	Each itm In colItem
						With itm
							If .Part = prev_hard_point(i) Then
								num = num - .Size
								If num < 0 Then
									colItem.Remove(.ID)
									If Not .Unit Is Nothing Then
										If .Unit.ID = ID Then
											'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
											.Unit = Nothing
										End If
									End If
									'追加パイロットを持つアイテムを削除する場合
									If .IsFeatureAvailable("追加パイロット") Then
										If PList.IsDefined(.FeatureData("追加パイロット")) Then
											With PList.Item(.FeatureData("追加パイロット"))
												.Alive = False
												'UPGRADE_NOTE: オブジェクト PList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
												.Unit_Renamed = Nothing
											End With
										End If
									End If
								End If
							End If
						End With
					Next itm
				End If
			Next 
			
			'両手利きで無くなってしまった場合は二個目の片手アイテムを外す
			If is_ambidextrous And Not IsFeatureAvailable("両手利き") Then
				num = 0
				For	Each itm In colItem
					With itm
						If .Part = "片手" Then
							num = num + 1
							If num > 1 Then
								is_changed = True
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
										.Unit = Nothing
									End If
								End If
								'追加パイロットを持つアイテムを削除する場合
								If .IsFeatureAvailable("追加パイロット") Then
									If PList.IsDefined(.FeatureData("追加パイロット")) Then
										With PList.Item(.FeatureData("追加パイロット"))
											.Alive = False
											'UPGRADE_NOTE: オブジェクト PList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
											.Unit_Renamed = Nothing
										End With
									End If
								End If
							End If
						End If
					End With
				Next itm
			End If
		Loop While is_changed
	End Sub
	
	'装備個所が ipart のアイテムの装備可能数
	Public Function ItemSlotSize(ByRef ipart As String) As Short
		Dim i As Short
		
		Select Case ipart
			Case "強化パーツ", "アイテム"
				ItemSlotSize = Data.ItemNum
				If Not IsFeatureAvailable("ハードポイント") Then
					Exit Function
				End If
				For i = 1 To CountFeature
					If Feature(i) = "ハードポイント" Then
						Select Case FeatureData(i)
							Case "強化パーツ", "アイテム"
								ItemSlotSize = ItemSlotSize + FeatureLevel(i)
						End Select
					End If
				Next 
			Case Else
				If Not IsFeatureAvailable("ハードポイント") Then
					ItemSlotSize = 1
					Exit Function
				End If
				For i = 1 To CountFeature
					If Feature(i) = "ハードポイント" Then
						If FeatureData(i) = ipart Then
							ItemSlotSize = ItemSlotSize + FeatureLevel(i)
						End If
					End If
				Next 
		End Select
	End Function
	
	'アイテム iname を装備しているか？
	Public Function IsEquiped(ByRef iname As String) As Boolean
		Dim i As Short
		
		IsEquiped = False
		For i = 1 To CountItem
			If Item(i).Name = iname Then
				IsEquiped = True
				Exit Function
			End If
		Next 
	End Function
	
	'装備可能な武器クラス
	Public Function WeaponProficiency() As String
		Dim fd As FeatureData
		
		For	Each fd In colFeature
			With fd
				If .Name = "武器クラス" Then
					WeaponProficiency = WeaponProficiency & " " & .StrData
				End If
			End With
		Next fd
	End Function
	
	'装備可能な防具クラス
	Public Function ArmorProficiency() As String
		Dim fd As FeatureData
		
		For	Each fd In colFeature
			With fd
				If .Name = "防具クラス" Then
					ArmorProficiency = ArmorProficiency & " " & .StrData
				End If
			End With
		Next fd
	End Function
	
	'アイテムitを装備できるかどうかを判定
	Public Function IsAbleToEquip(ByRef it As Item) As Boolean
		Dim iclass As String
		Dim eclass0, uclass, eclass As String
		Dim i, j As Short
		
		With it
			'既に装備済みのアイテムは装備できない
			If Not .Unit Is Nothing Then
				If .Unit.ID = ID Then
					IsAbleToEquip = False
					Exit Function
				End If
			End If
			
			'Fixコマンドで固定されたアイテムは装備不可能
			If IsGlobalVariableDefined("Fix(" & .Name & ")") Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'必要技能は満たしているか？
			If Not .IsAvailable(Me) Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'アイテムのクラスを記録
			iclass = .Class_Renamed()
			
			'汎用ならばユニットの種類に関わらず装備可能
			If iclass = "汎用" Then
				'ただし強化パーツのチェックは必要
				If .Part = "強化パーツ" And IsHero Then
					IsAbleToEquip = False
					Exit Function
				End If
				IsAbleToEquip = True
				Exit Function
			End If
			
			'固定アイテムは装備不能とみなす
			If iclass = "固定" Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'ユニットクラスから余分な指定を取り除く
			uclass = Class0
			
			'装備個所
			Select Case .Part
				Case "武器", "片手", "両手"
					eclass = WeaponProficiency()
					For i = 1 To LLength(eclass)
						eclass0 = LIndex(eclass, i)
						If iclass = eclass0 Then
							IsAbleToEquip = True
							Exit Function
						ElseIf InStr(iclass, "専用)") > 0 Then 
							'ユニットクラス、ユニット名による専用指定？
							If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & Name & "専用)") > 0 Or InStr(iclass, "(" & Nickname & "専用)") > 0) Then
								IsAbleToEquip = True
								Exit Function
							End If
							
							'性別による専用指定？
							If CountPilot > 0 Then
								If iclass = eclass0 & "(" & MainPilot.Sex & "専用)" Then
									IsAbleToEquip = True
									Exit Function
								End If
							End If
						End If
					Next 
					
					'一部の形態でのみ利用可能な武器の判定
					For i = 1 To CountOtherForm
						With OtherForm(i)
							uclass = .Class0
							eclass = .WeaponProficiency()
							For j = 1 To LLength(eclass)
								eclass0 = LIndex(eclass, j)
								If iclass = eclass0 Then
									IsAbleToEquip = True
									Exit Function
								ElseIf InStr(iclass, "専用)") > 0 Then 
									'ユニットクラス、ユニット名による専用指定？
									If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & .Name & "専用)") > 0 Or InStr(iclass, "(" & .Nickname & "専用)") > 0) Then
										IsAbleToEquip = True
										Exit Function
									End If
									
									'性別による専用指定？
									If CountPilot > 0 Then
										If iclass = eclass0 & "(" & MainPilot.Sex & "専用)" Then
											IsAbleToEquip = True
											Exit Function
										End If
									End If
								End If
							Next 
						End With
					Next 
					
				Case "体", "頭", "盾"
					eclass = ArmorProficiency()
					For i = 1 To LLength(eclass)
						eclass0 = LIndex(eclass, i)
						If iclass = eclass0 Then
							IsAbleToEquip = True
							Exit Function
						ElseIf InStr(iclass, "専用)") > 0 Then 
							'ユニットクラス、ユニット名による専用指定？
							If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & Name & "専用)") > 0 Or InStr(iclass, "(" & Nickname & "専用)") > 0) Then
								IsAbleToEquip = True
								Exit Function
							End If
							
							'性別による専用指定？
							If CountPilot > 0 Then
								If iclass = eclass0 & "(" & MainPilot.Sex & "専用)" Then
									IsAbleToEquip = True
									Exit Function
								End If
							End If
						End If
					Next 
					
					'一部の形態でのみ利用可能な防具の判定
					For i = 1 To CountOtherForm
						With OtherForm(i)
							uclass = .Class0
							eclass = .ArmorProficiency()
							For j = 1 To LLength(eclass)
								eclass0 = LIndex(eclass, j)
								If iclass = eclass0 Then
									IsAbleToEquip = True
									Exit Function
								ElseIf InStr(iclass, "専用)") > 0 Then 
									'ユニットクラス、ユニット名による専用指定？
									If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & .Name & "専用)") > 0 Or InStr(iclass, "(" & .Nickname & "専用)") > 0) Then
										IsAbleToEquip = True
										Exit Function
									End If
									
									'性別による専用指定？
									If CountPilot > 0 Then
										If iclass = eclass0 & "(" & MainPilot.Sex & "専用)" Then
											IsAbleToEquip = True
											Exit Function
										End If
									End If
								End If
							Next 
						End With
					Next 
					
				Case "アイテム", "強化パーツ"
					'強化パーツは人間ユニットには装備できない
					If InStr(.Part, "強化パーツ") = 1 Then
						If IsHero Then
							IsAbleToEquip = False
							Exit Function
						End If
					End If
					
					'これらのアイテムは専用アイテムでない限り必ず装備可能
					If InStr(iclass, "専用)") = 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'ユニットクラス、ユニット名による専用指定？
					If InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & Name & "専用)") > 0 Or InStr(iclass, "(" & Nickname & "専用)") > 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'性別による専用指定？
					If CountPilot > 0 Then
						If InStr(iclass, "(" & MainPilot.Sex & "専用)") > 0 Then
							IsAbleToEquip = True
							Exit Function
						End If
					End If
					
					'他の形態の名前で専用指定されている？
					For i = 1 To CountOtherForm
						With OtherForm(i)
							If InStr(iclass, "(" & .Class0 & "専用)") > 0 Or InStr(iclass, "(" & .Name & "専用)") > 0 Or InStr(iclass, "(" & .Nickname & "専用)") > 0 Then
								IsAbleToEquip = True
								Exit Function
							End If
						End With
					Next 
					
				Case Else
					'創作された装備個所のアイテムは専用アイテムでない限り必ず装備可能
					If InStr(iclass, "専用)") = 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'ユニットクラス、ユニット名による専用指定？
					If InStr(iclass, "(" & uclass & "専用)") > 0 Or InStr(iclass, "(" & Name & "専用)") > 0 Or InStr(iclass, "(" & Nickname & "専用)") > 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'性別による専用指定？
					If CountPilot > 0 Then
						If InStr(iclass, "(" & MainPilot.Sex & "専用)") > 0 Then
							IsAbleToEquip = True
							Exit Function
						End If
					End If
					
					'他の形態の名前で専用指定されている？
					For i = 1 To CountOtherForm
						With OtherForm(i)
							If InStr(iclass, "(" & .Class0 & "専用)") > 0 Or InStr(iclass, "(" & .Name & "専用)") > 0 Or InStr(iclass, "(" & .Nickname & "専用)") > 0 Then
								IsAbleToEquip = True
								Exit Function
							End If
						End With
					Next 
			End Select
		End With
		
		IsAbleToEquip = False
	End Function
	
	
	' === メッセージ関連処理 ===
	
	'状況 Situation に応じたパイロットメッセージを表示
	Public Sub PilotMessage(ByRef Situation As String, Optional ByRef msg_mode As String = "")
		Dim k, i, j, w As Short
		Dim p As Pilot
		Dim md As MessageData
		Dim dd As Dialog
		Dim msg As String
		Dim situations() As String
		Dim wname As String
		Dim pnames() As String
		Dim buf As String
		Dim selected_pilot As String
		Dim selected_situation As String
		Dim selected_msg As String
		
		'WAVEを演奏したかチェックするため、あらかじめクリア
		IsWavePlayed = False
		
		'対応メッセージが定義されていなかった場合に使用するシチュエーションを設定
		ReDim situations(1)
		situations(1) = Situation
		Select Case Situation
			Case "分身", "切り払い", "迎撃", "反射", "当て身技", "阻止", "ダミー", "緊急テレポート"
				ReDim Preserve situations(2)
				situations(2) = "回避"
			Case "ビーム無効化", "攻撃無効化", "シールド防御"
				ReDim Preserve situations(2)
				situations(2) = "ダメージ小"
			Case "回避", "破壊", "ダメージ大", "ダメージ中", "ダメージ小", "かけ声"
			Case Else
				If msg_mode = "攻撃" Or msg_mode = "カウンター" Then
					'攻撃メッセージ
					wname = situations(1)
					
					'武器番号を検索
					For w = 1 To CountWeapon
						With Weapon(w)
							If Situation = .Name Or Situation = .Nickname Then
								Exit For
							End If
						End With
					Next 
					
					If Not IsDefense() Then
						ReDim Preserve situations(2)
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(2) = "格闘"
						Else
							situations(2) = "射撃"
						End If
					ElseIf msg_mode = "カウンター" Then 
						ReDim Preserve situations(3)
						situations(1) = Situation & "(反撃)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(3) = "格闘"
						Else
							situations(3) = "射撃"
						End If
					Else
						ReDim Preserve situations(4)
						situations(1) = Situation & "(反撃)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(3) = "格闘(反撃)"
							situations(4) = "格闘"
						Else
							situations(3) = "射撃(反撃)"
							situations(4) = "射撃"
						End If
					End If
				ElseIf msg_mode = "アビリティ" Then 
					ReDim Preserve situations(2)
					situations(2) = "アビリティ"
				ElseIf InStr(Situation, "(命中)") > 0 Or InStr(Situation, "(回避)") > 0 Or InStr(Situation, "(とどめ)") > 0 Or InStr(Situation, "(クリティカル)") > 0 Then 
					'サブシチュエーション付きの攻撃メッセージ
					
					'武器番号を検索
					wname = Left(Situation, InStr2(Situation, "(") - 1)
					For w = 1 To CountWeapon
						With Weapon(w)
							If wname = .Name Or wname = .Nickname Then
								Exit For
							End If
						End With
					Next 
					
					If Not IsDefense() Then
						ReDim Preserve situations(2)
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(2) = "格闘" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(2) = "射撃" & Mid(Situation, InStr2(Situation, "("))
						End If
					ElseIf msg_mode = "カウンター" Then 
						ReDim Preserve situations(3)
						situations(1) = Situation & "(反撃)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(3) = "格闘" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(3) = "射撃" & Mid(Situation, InStr2(Situation, "("))
						End If
					Else
						ReDim Preserve situations(4)
						situations(1) = Situation & "(反撃)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "格闘系") Then
							situations(3) = "格闘" & Mid(Situation, InStr2(Situation, "(")) & "(反撃)"
							situations(4) = "格闘" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(3) = "射撃" & Mid(Situation, InStr2(Situation, "(")) & "(反撃)"
							situations(4) = "射撃" & Mid(Situation, InStr2(Situation, "("))
						End If
					End If
				Else
					'攻撃メッセージでなくても一応攻撃武器名を設定
					If SelectedUnit Is Me Then
						If 0 < SelectedWeapon And SelectedWeapon <= CountWeapon Then
							wname = Weapon(SelectedWeapon).Name
						End If
					ElseIf SelectedTarget Is Me Then 
						If 0 < SelectedTWeapon And SelectedTWeapon <= CountWeapon Then
							wname = Weapon(SelectedTWeapon).Name
						End If
					End If
				End If
		End Select
		
		'SelectMessageコマンド
		For i = 1 To UBound(situations)
			buf = "Message(" & MainPilot.ID & "," & situations(i) & ")"
			If IsLocalVariableDefined(buf) Then
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				selected_msg = LocalVariableList.Item(buf).StringValue
				selected_situation = situations(i)
				UndefineVariable(buf)
				Exit For
			End If
			
			If situations(i) = "格闘" Or situations(i) = "射撃" Then
				buf = "Message(" & MainPilot.ID & ",攻撃)"
				If IsLocalVariableDefined(buf) Then
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					selected_msg = LocalVariableList.Item(buf).StringValue
					selected_situation = "攻撃"
					UndefineVariable(buf)
					Exit For
				End If
			End If
			
			If situations(i) = "格闘(反撃)" Or situations(i) = "射撃(反撃)" Then
				buf = "Message(" & MainPilot.ID & ",攻撃(反撃))"
				If IsLocalVariableDefined(buf) Then
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					selected_msg = LocalVariableList.Item(buf).StringValue
					selected_situation = "攻撃(反撃)"
					UndefineVariable(buf)
					Exit For
				End If
			End If
		Next 
		If InStr(selected_msg, "::") > 0 Then
			selected_pilot = Left(selected_msg, InStr(selected_msg, "::") - 1)
			selected_msg = Mid(selected_msg, InStr(selected_msg, "::") + 2)
		End If
		
		'かけ声は３分の２の確率で表示
		If selected_msg = "" Then
			If InStr(Situation, "かけ声") = 1 Then
				If Dice(3) = 1 Then
					Exit Sub
				End If
			End If
		End If
		
		'しゃべれない場合
		'ただしSetMessageコマンドでメッセージが設定されている場合は
		'そちらを使用。
		If selected_msg = "" Then
			If IsConditionSatisfied("石化") Or IsConditionSatisfied("凍結") Or IsConditionSatisfied("麻痺") Then
				'意識不明
				Exit Sub
			End If
			If IsConditionSatisfied("沈黙") Or IsConditionSatisfied("憑依") Then
				'無言
				If InStr(Situation, "(") = 0 Then
					Select Case Situation
						Case "ダメージ中", "ダメージ大", "破壊"
							If NPDList.IsDefined(MainPilot.Name & "(ダメージ)") Then
								DisplayBattleMessage(MainPilot.Name & "(ダメージ)", "…………！")
								Exit Sub
							End If
						Case "かけ声"
							Exit Sub
					End Select
					If wname <> "" Then
						If NPDList.IsDefined(MainPilot.Name & "(攻撃)") Then
							DisplayBattleMessage(MainPilot.Name & "(攻撃)", "…………！")
							Exit Sub
						End If
					End If
					DisplayBattleMessage(MainPilot.ID, "…………")
				End If
				Exit Sub
			End If
			If IsConditionSatisfied("睡眠") Then
				'寝言
				If InStr(Situation, "(") = 0 Then
					DisplayBattleMessage(MainPilot.ID, "ＺＺＺ……")
				End If
				Exit Sub
			End If
			If IsConditionSatisfied("恐怖") Then
				If IsMessageDefined("恐怖") Then
					'恐怖状態用メッセージが定義されていればそちらを使う
					ReDim situations(1)
					situations(1) = "恐怖"
				Else
					'パニック時のメッセージを作成して表示
					If InStr(Situation, "(") = 0 Then
						msg = ""
						Select Case MainPilot.Sex
							Case "男性"
								Select Case Dice(4)
									Case 1
										msg = "う、うわああああっ！"
									Case 2
										msg = "うわあああっ！"
									Case 3
										msg = "ひ、ひいいいっ！"
									Case 4
										msg = "ひいいいっ！"
								End Select
							Case "女性"
								Select Case Dice(4)
									Case 1
										msg = "きゃあああああっ！"
									Case 2
										msg = "きゃああっ！"
									Case 3
										msg = "うわあああっ！"
									Case 4
										msg = "た、助けてええっ！"
								End Select
						End Select
						
						If msg <> "" Then
							If NPDList.IsDefined(MainPilot.Name & "(泣き)") Then
								DisplayBattleMessage(MainPilot.Name & "(泣き)", msg)
							ElseIf NPDList.IsDefined(MainPilot.Name & "(ダメージ)") Then 
								DisplayBattleMessage(MainPilot.Name & "(ダメージ)", msg)
							Else
								DisplayBattleMessage(MainPilot.ID, msg)
							End If
						End If
					End If
					Exit Sub
				End If
			End If
			If IsConditionSatisfied("混乱") Then
				If IsMessageDefined("混乱") Then
					'混乱状態用メッセージが定義されていればそちらを使う
					ReDim situations(1)
					situations(1) = "混乱"
				End If
			End If
		End If
		
		'ダイアログデータを使って判定
		ReDim pnames(3)
		pnames(1) = MainPilot.MessageType
		pnames(2) = MainPilot.MessageType
		pnames(3) = MainPilot.MessageType
		If IsFeatureAvailable("追加パイロット") Then
			ReDim Preserve pnames(4)
			pnames(4) = Pilot(1).MessageType
		End If
		For i = 2 To CountPilot
			pnames(1) = pnames(1) & " " & Pilot(i).MessageType
			pnames(2) = pnames(2) & " " & Pilot(i).MessageType
		Next 
		For i = 1 To CountSupport
			pnames(1) = pnames(1) & " " & Support(i).MessageType
		Next 
		If IsFeatureAvailable("追加サポート") Then
			pnames(1) = pnames(1) & " " & AdditionalSupport.MessageType
		End If
		If Situation = SelectedSpecialPower Then
			pnames(3) = SelectedPilot.MessageType
		End If
		For i = 1 To UBound(pnames)
			'追加パイロットにメッセージデータがあればそちらを優先
			If i = 4 Then
				If MDList.IsDefined(MainPilot.MessageType) Then
					Exit For
				End If
			End If
			
			If DDList.IsDefined(pnames(i)) Then
				With DDList.Item(pnames(i))
					If selected_msg <> "" Then
						'SelectMessageで選択されたメッセージを検索
						k = 0
						For j = 1 To .CountDialog
							If .Situation(j) = selected_situation Then
								k = k + 1
								If VB6.Format(k) = selected_msg Then
									PlayDialog(.Dialog(j), wname)
									Exit Sub
								End If
							ElseIf .Situation(j) = selected_msg Then 
								PlayDialog(.Dialog(j), wname)
								Exit Sub
							End If
						Next 
					Else
						For j = 1 To UBound(situations)
							dd = .SelectDialog(situations(j), Me)
							If Not dd Is Nothing Then
								PlayDialog(dd, wname)
								Exit Sub
							End If
						Next 
					End If
				End With
			End If
		Next 
		
		'ゲッターのようなユニットは必ずメインパイロットを使ってメッセージを表示
		If Data.PilotNum > 0 And MainPilot Is Pilot(1) And Situation <> SelectedSpecialPower Then
			i = Dice(CountPilot + CountSupport)
		Else
			i = 1
		End If
		
TryAgain: 
		'選んだパイロットによるメッセージデータで判定
		If Situation = SelectedSpecialPower Then
			If Not MDList.IsDefined((SelectedPilot.MessageType)) Then
				GoTo TrySelectedMessage
			End If
		ElseIf i = 1 Then 
			If Not MDList.IsDefined(MainPilot.MessageType) And Not MDList.IsDefined((Pilot(1).MessageType)) Then
				GoTo TrySelectedMessage
			End If
		ElseIf i <= CountPilot Then 
			If Not MDList.IsDefined((Pilot(i).MessageType)) Then
				i = 1
				GoTo TryAgain
			End If
		Else
			If Not MDList.IsDefined((Support(i - CountPilot).MessageType)) Then
				If i > 1 Then
					i = 1
					GoTo TryAgain
				Else
					GoTo TrySelectedMessage
				End If
			End If
		End If
		
		'メッセージを表示
		If Situation = SelectedSpecialPower Then
			md = MDList.Item((SelectedPilot.MessageType))
			p = SelectedPilot
		ElseIf i = 1 Then 
			md = MDList.Item(MainPilot.MessageType)
			p = MainPilot
			If Not md Is Nothing Then
				For j = 1 To UBound(situations)
					If Len(md.SelectMessage(situations(j), Me)) > 0 Then
						Exit For
					End If
				Next 
			Else
				md = MDList.Item((Pilot(1).MessageType))
				p = Pilot(1)
			End If
		ElseIf i <= CountPilot Then 
			md = MDList.Item((Pilot(i).MessageType))
			p = Pilot(i)
		Else
			md = MDList.Item((Support(i - CountPilot).MessageType))
			p = Support(i - CountPilot)
		End If
		
		'メッセージデータが見つからない場合は他のパイロットで探しなおす
		If md Is Nothing Then
			If i <> 1 Then
				i = 1
				GoTo TryAgain
			Else
				GoTo TrySelectedMessage
			End If
		End If
		
		If selected_msg <> "" Then
			'SelectMessageで選択されたメッセージを検索
			k = 0
			With md
				For j = 1 To .CountMessage
					If .Situation(j) = selected_situation Then
						k = k + 1
						If VB6.Format(k) = selected_msg Then
							PlayMessage(p, .Message(j), wname)
							Exit Sub
						End If
					ElseIf .Situation(j) = selected_msg Then 
						PlayMessage(p, .Message(j), wname)
						Exit Sub
					End If
				Next 
			End With
		Else
			'メッセージデータからメッセージを検索
			For j = 1 To UBound(situations)
				msg = md.SelectMessage(situations(j), Me)
				If msg <> "" Then
					PlayMessage(p, msg, wname)
					Exit Sub
				End If
			Next 
		End If
		
		If i <> 1 Then
			i = 1
			GoTo TryAgain
		End If
		
TrySelectedMessage: 
		'メッセージを表示
		If selected_msg <> "" And selected_msg <> "-" Then
			If selected_pilot <> "" Then
				DisplayBattleMessage(selected_pilot, selected_msg)
			Else
				DisplayBattleMessage(MainPilot.ID, selected_msg)
			End If
		End If
	End Sub
	
	'ダイアログの再生
	Public Sub PlayDialog(ByRef dd As Dialog, ByRef wname As String)
		Dim msg, buf As String
		Dim i, idx As Short
		Dim t As Unit
		Dim tw As Short
		
		'画像描画が行われたかどうかの判定のためにフラグを初期化
		IsPictureDrawn = False
		
		With dd
			'ダイアログの個々のメッセージを表示
			For i = 1 To .Count
				msg = .Message(i)
				
				'メッセージ表示のキャンセル
				If msg = "-" Then
					Exit Sub
				End If
				
				'ユニット名
				Do While InStr(msg, "$(ユニット)") > 0
					idx = InStr(msg, "$(ユニット)")
					buf = Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "専用") > 0 Then
						buf = Mid(buf, InStr(buf, "専用") + 2)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
				Loop 
				Do While InStr(msg, "$(機体)") > 0
					idx = InStr(msg, "$(機体)")
					buf = Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "専用") > 0 Then
						buf = Mid(buf, InStr(buf, "専用") + 2)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
				Loop 
				
				'パイロット名
				Do While InStr(msg, "$(パイロット)") > 0
					buf = MainPilot.Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					idx = InStr(msg, "$(パイロット)")
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
				Loop 
				
				'武器名
				Do While InStr(msg, "$(武器)") > 0
					idx = InStr(msg, "$(武器)")
					buf = wname
					ReplaceSubExpression(buf)
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "<") > 0 Then
						buf = Left(buf, InStr(buf, "<") - 1)
					End If
					If InStr(buf, "＜") > 0 Then
						buf = Left(buf, InStr(buf, "＜") - 1)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
				Loop 
				
				'損傷率
				Do While InStr(msg, "$(損傷率)") > 0
					idx = InStr(msg, "$(損傷率)")
					msg = Left(msg, idx - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, idx + 6)
				Loop 
				
				'相手ユニットを設定
				If SelectedUnit Is Me Then
					t = SelectedTarget
					tw = SelectedTWeapon
				Else
					t = SelectedUnit
					tw = SelectedWeapon
				End If
				
				If Not t Is Nothing Then
					'相手ユニット名
					Do While InStr(msg, "$(相手ユニット)") > 0
						buf = t.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						If InStr(buf, "専用") > 0 Then
							buf = Mid(buf, InStr(buf, "専用") + 2)
						End If
						idx = InStr(msg, "$(相手ユニット)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 9)
					Loop 
					Do While InStr(msg, "$(相手機体)") > 0
						buf = t.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						If InStr(buf, "専用") > 0 Then
							buf = Mid(buf, InStr(buf, "専用") + 2)
						End If
						idx = InStr(msg, "$(相手機体)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
					Loop 
					
					'相手パイロット名
					Do While InStr(msg, "$(相手パイロット)") > 0
						buf = t.MainPilot.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						idx = InStr(msg, "$(相手パイロット)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 10)
					Loop 
					
					'相手武器名
					Do While InStr(msg, "$(相手武器)") > 0
						If 1 <= tw And tw <= t.CountWeapon Then
							buf = t.WeaponNickname(tw)
						Else
							buf = ""
						End If
						If InStr(buf, "<") > 0 Then
							buf = Left(buf, InStr(buf, "<") - 1)
						End If
						If InStr(buf, "＜") > 0 Then
							buf = Left(buf, InStr(buf, "＜") - 1)
						End If
						idx = InStr(msg, "$(相手武器)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
					Loop 
					
					'相手損傷率
					Do While InStr(msg, "$(相手損傷率)") > 0
						With t
							buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
						End With
						idx = InStr(msg, "$(相手損傷率)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
					Loop 
				End If
				
				'メッセージを表示
				If .Name(i) = MainPilot.Name Then
					DisplayBattleMessage(MainPilot.ID, msg)
				ElseIf Left(.Name(i), 1) = "@" Then 
					DisplayBattleMessage(Mid(.Name(i), 2), msg)
				Else
					DisplayBattleMessage(.Name(i), msg)
				End If
			Next 
		End With
		
		'カットインは消去しておく
		If Not IsOptionDefined("戦闘中画面初期化無効") Then
			If IsPictureDrawn Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
			End If
		End If
	End Sub
	
	'メッセージを再生
	Public Sub PlayMessage(ByRef p As Pilot, ByRef msg As String, ByRef wname As String)
		Dim buf As String
		Dim idx As Short
		Dim t As Unit
		Dim tw As Short
		
		'メッセージ表示をキャンセル
		If msg = "-" Then
			Exit Sub
		End If
		
		'画像描画が行われたかどうかの判定のためにフラグを初期化
		IsPictureDrawn = False
		
		'ユニット名
		Do While InStr(msg, "$(ユニット)") > 0
			idx = InStr(msg, "$(ユニット)")
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Mid(buf, InStr(buf, "専用") + 2)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
		Loop 
		Do While InStr(msg, "$(機体)") > 0
			idx = InStr(msg, "$(機体)")
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Mid(buf, InStr(buf, "専用") + 2)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
		Loop 
		
		'パイロット名
		Do While InStr(msg, "$(パイロット)") > 0
			buf = MainPilot.Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			idx = InStr(msg, "$(パイロット)")
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
		Loop 
		
		'武器名
		Do While InStr(msg, "$(武器)") > 0
			idx = InStr(msg, "$(武器)")
			buf = wname
			ReplaceSubExpression(buf)
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "<") > 0 Then
				buf = Left(buf, InStr(buf, "<") - 1)
			End If
			If InStr(buf, "＜") > 0 Then
				buf = Left(buf, InStr(buf, "＜") - 1)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
		Loop 
		
		'損傷率
		Do While InStr(msg, "$(損傷率)") > 0
			idx = InStr(msg, "$(損傷率)")
			msg = Left(msg, idx - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, idx + 6)
		Loop 
		
		'相手ユニットを設定
		If SelectedUnit Is Me Then
			t = SelectedTarget
			tw = SelectedTWeapon
		Else
			t = SelectedUnit
			tw = SelectedWeapon
		End If
		
		If Not t Is Nothing Then
			'相手ユニット名
			Do While InStr(msg, "$(相手ユニット)") > 0
				buf = t.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				If InStr(buf, "専用") > 0 Then
					buf = Mid(buf, InStr(buf, "専用") + 2)
				End If
				idx = InStr(msg, "$(相手ユニット)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 9)
			Loop 
			Do While InStr(msg, "$(相手機体)") > 0
				buf = t.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				If InStr(buf, "専用") > 0 Then
					buf = Mid(buf, InStr(buf, "専用") + 2)
				End If
				idx = InStr(msg, "$(相手機体)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
			Loop 
			
			'相手パイロット名
			Do While InStr(msg, "$(相手パイロット)") > 0
				buf = t.MainPilot.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				idx = InStr(msg, "$(相手パイロット)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 10)
			Loop 
			
			'相手武器名
			Do While InStr(msg, "$(相手武器)") > 0
				If 1 <= tw And tw <= t.CountWeapon Then
					buf = t.WeaponNickname(tw)
				Else
					buf = ""
				End If
				If InStr(buf, "<") > 0 Then
					buf = Left(buf, InStr(buf, "<") - 1)
				End If
				If InStr(buf, "＜") > 0 Then
					buf = Left(buf, InStr(buf, "＜") - 1)
				End If
				idx = InStr(msg, "$(相手武器)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
			Loop 
			
			'相手損傷率
			Do While InStr(msg, "$(相手損傷率)") > 0
				With t
					buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
				End With
				idx = InStr(msg, "$(相手損傷率)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
			Loop 
		End If
		
		'メッセージを表示
		DisplayBattleMessage((p.ID), msg)
		
		'カットインは消去しておく
		If Not IsOptionDefined("戦闘中画面初期化無効") Then
			If IsPictureDrawn Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
			End If
		End If
	End Sub
	
	'シチュエーション main_situation に対応するメッセージが定義されているか
	Public Function IsMessageDefined(ByRef main_situation As String, Optional ByVal ignore_condition As Boolean = False) As Boolean
		Dim pnames(4) As String
		Dim msg As String
		Dim i As Short
		
		'しゃべれない場合
		If Not ignore_condition Then
			If IsConditionSatisfied("沈黙") Or IsConditionSatisfied("憑依") Or IsConditionSatisfied("石化") Or IsConditionSatisfied("凍結") Or IsConditionSatisfied("麻痺") Or IsConditionSatisfied("睡眠") Then
				IsMessageDefined = False
				Exit Function
			End If
			
			'特殊状態用メッセージが定義されているか確認する場合を考慮
			If IsConditionSatisfied("恐怖") And main_situation <> "恐怖" Then
				IsMessageDefined = False
				Exit Function
			End If
			If IsConditionSatisfied("混乱") And main_situation <> "混乱" Then
				IsMessageDefined = False
				Exit Function
			End If
		End If
		
		'SetMessageコマンドでメッセージが設定されているか判定
		If IsLocalVariableDefined("Message(" & MainPilot.ID & "," & main_situation & ")") Then
			IsMessageDefined = True
			Exit Function
		End If
		
		'ダイアログデータを使って判定
		With MainPilot
			pnames(1) = .MessageType
			pnames(2) = .MessageType
			pnames(3) = .MessageType
		End With
		pnames(4) = Pilot(1).MessageType
		For i = 2 To CountPilot
			With Pilot(i)
				pnames(1) = pnames(1) & " " & .MessageType
				pnames(2) = pnames(2) & " " & .MessageType
			End With
		Next 
		For i = 1 To CountSupport
			pnames(1) = pnames(1) & " " & Support(i).MessageType
		Next 
		If IsFeatureAvailable("追加サポート") Then
			pnames(1) = pnames(1) & " " & AdditionalSupport.MessageType
		End If
		If main_situation <> "" Then
			If main_situation = SelectedSpecialPower Then
				pnames(3) = SelectedPilot.MessageType
			End If
		End If
		For i = 1 To 4
			If DDList.IsDefined(pnames(i)) Then
				With DDList.Item(pnames(i))
					If Not .SelectDialog(main_situation, Me, ignore_condition) Is Nothing Then
						IsMessageDefined = True
						Exit Function
					End If
				End With
			End If
		Next 
		
		'メッセージデータを使って判定
		If main_situation = SelectedSpecialPower Then
			With SelectedPilot
				If MDList.IsDefined(.MessageType) Then
					msg = MDList.Item(.MessageType).SelectMessage(main_situation, Me)
				End If
			End With
		Else
			With MainPilot
				If MDList.IsDefined(.MessageType) Then
					msg = MDList.Item(.MessageType).SelectMessage(main_situation, Me)
				End If
			End With
			If Len(msg) = 0 Then
				With Pilot(1)
					If MDList.IsDefined(.MessageType) Then
						msg = MDList.Item(.MessageType).SelectMessage(main_situation, Me)
					End If
				End With
			End If
		End If
		
		If Len(msg) > 0 Then
			IsMessageDefined = True
		End If
	End Function
	
	'解説メッセージを表示
	Public Sub SysMessage(ByRef main_situation As String, Optional ByRef sub_situation As String = "", Optional ByRef add_msg As String = "")
		Dim uname, msg, uclass As String
		Dim situations() As String
		Dim idx, buf As String
		Dim i, ret As Short
		Dim wname As String
		
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation & "(解説)"
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")(解説)"
			situations(2) = main_situation & "(解説)"
		End If
		
		' ADD START MARGE
		'拡張戦闘アニメデータで検索
		If ExtendedAnimation Then
			With EADList
				For i = 1 To UBound(situations)
					'戦闘アニメ能力で指定された名称で検索
					If IsFeatureAvailable("戦闘アニメ") Then
						uname = FeatureData("戦闘アニメ")
						If .IsDefined(uname) Then
							msg = .Item(uname).SelectMessage(situations(i), Me)
							If Len(msg) > 0 Then
								GoTo FoundMessage
							End If
						End If
					End If
					
					'ユニット名称で検索
					If .IsDefined(Name) Then
						msg = .Item(Name).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'ユニット愛称を修正したもので検索
					uname = Nickname0
					ret = InStr(uname, "(")
					If ret > 1 Then
						uname = Left(uname, ret - 1)
					End If
					ret = InStr(uname, "用")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					ret = InStr(uname, "型")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					If Right(uname, 4) = "カスタム" Then
						uname = Left(uname, Len(uname) - 4)
					End If
					If Right(uname, 1) = "改" Then
						uname = Left(uname, Len(uname) - 1)
					End If
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'ユニットクラスで検索
					uclass = Class0
					If .IsDefined(uclass) Then
						msg = .Item(uclass).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'汎用
					If .IsDefined("汎用") Then
						msg = .Item("汎用").SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				Next 
			End With
		End If
		' ADD END MARGE
		
		'戦闘アニメデータで検索
		With ADList
			For i = 1 To UBound(situations)
				'戦闘アニメ能力で指定された名称で検索
				If IsFeatureAvailable("戦闘アニメ") Then
					uname = FeatureData("戦闘アニメ")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				End If
				
				'ユニット名称で検索
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'ユニット愛称を修正したもので検索
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "用")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "型")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "カスタム" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "改" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'ユニットクラスで検索
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'汎用
				If .IsDefined("汎用") Then
					msg = .Item("汎用").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
			Next 
		End With
		
		'特殊効果データで検索
		With EDList
			For i = 1 To UBound(situations)
				'特殊効果能力で指定された名称で検索
				If IsFeatureAvailable("特殊効果") Then
					uname = FeatureData("特殊効果")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				End If
				
				'ユニット名称で検索
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'ユニット愛称を修正したもので検索
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "用")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "型")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "カスタム" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "改" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'ユニットクラスで検索
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'汎用
				If .IsDefined("汎用") Then
					msg = .Item("汎用").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
			Next 
		End With
		
		'対応するメッセージが見つからなかった
		Exit Sub
		
FoundMessage: 
		
		'メッセージ表示のキャンセル
		If msg = "-" Then
			Exit Sub
		End If
		
		'ユニット名
		Do While InStr(msg, "$(ユニット)") > 0
			idx = CStr(InStr(msg, "$(ユニット)"))
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Left(buf, InStr(buf, "専用") + 2)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		Do While InStr(msg, "$(機体)") > 0
			idx = CStr(InStr(msg, "$(機体)"))
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Left(buf, InStr(buf, "専用") + 2)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 5)
		Loop 
		
		'パイロット名
		Do While InStr(msg, "$(パイロット)") > 0
			idx = CStr(InStr(msg, "$(パイロット)"))
			buf = MainPilot.Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 8)
		Loop 
		
		'武器名
		If InStr(msg, "$(武器)") > 0 Then
			If SelectedUnit Is Me Then
				wname = Weapon(SelectedWeapon).Name
			Else
				wname = Weapon(SelectedTWeapon).Name
			End If
			If InStr(wname, "(") > 0 Then
				wname = Left(wname, InStr(wname, "(") - 1)
			End If
			If InStr(wname, "<") > 0 Then
				wname = Left(wname, InStr(wname, "<") - 1)
			End If
			Do While InStr(msg, "$(武器)") > 0
				idx = CStr(InStr(msg, "$(武器)"))
				msg = Left(msg, CDbl(idx) - 1) & wname & Mid(msg, CDbl(idx) + 5)
			Loop 
		End If
		
		'損傷率
		Do While InStr(msg, "$(損傷率)") > 0
			idx = CStr(InStr(msg, "$(損傷率)"))
			msg = Left(msg, CDbl(idx) - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, CDbl(idx) + 6)
		Loop 
		
		'相手ユニット名
		Do While InStr(msg, "$(相手ユニット)") > 0
			If SelectedUnit Is Me Then
				If Not SelectedTarget Is Nothing Then
					buf = SelectedTarget.Nickname
				Else
					buf = ""
				End If
			Else
				buf = SelectedUnit.Nickname
			End If
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Left(buf, InStr(buf, "専用") + 2)
			End If
			idx = CStr(InStr(msg, "$(相手ユニット)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 9)
		Loop 
		Do While InStr(msg, "$(相手機体)") > 0
			If SelectedUnit Is Me Then
				If Not SelectedTarget Is Nothing Then
					buf = SelectedTarget.Nickname
				Else
					buf = ""
				End If
			Else
				buf = SelectedUnit.Nickname
			End If
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "専用") > 0 Then
				buf = Left(buf, InStr(buf, "専用") + 2)
			End If
			idx = CStr(InStr(msg, "$(相手機体)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		
		'相手パイロット名
		Do While InStr(msg, "$(相手パイロット)") > 0
			If SelectedUnit Is Me Then
				If Not SelectedTarget Is Nothing Then
					buf = SelectedTarget.MainPilot.Nickname
				Else
					buf = ""
				End If
			Else
				buf = SelectedUnit.MainPilot.Nickname
			End If
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			idx = CStr(InStr(msg, "$(相手パイロット)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 10)
		Loop 
		
		'相手武器名
		Do While InStr(msg, "$(相手武器)") > 0
			If SelectedUnit Is Me Then
				If Not SelectedTarget Is Nothing Then
					buf = SelectedTarget.Weapon(SelectedTWeapon).Name
				Else
					buf = ""
				End If
			Else
				buf = SelectedUnit.Weapon(SelectedWeapon).Name
			End If
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "<") > 0 Then
				buf = Left(buf, InStr(buf, "<") - 1)
			End If
			If InStr(buf, "＜") > 0 Then
				buf = Left(buf, InStr(buf, "＜") - 1)
			End If
			idx = CStr(InStr(msg, "$(相手武器)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		
		'相手損傷率
		Do While InStr(msg, "$(相手損傷率)") > 0
			idx = CStr(InStr(msg, "$(相手損傷率)"))
			If SelectedUnit Is Me Then
				If Not SelectedTarget Is Nothing Then
					With SelectedTarget
						buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
					End With
				Else
					buf = ""
				End If
			Else
				With SelectedUnit
					buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
				End With
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 8)
		Loop 
		
		If Not frmMessage.Visible Then
			OpenMessageForm()
		End If
		If add_msg <> "" Then
			DisplayBattleMessage("-", msg & "." & add_msg)
		Else
			DisplayBattleMessage("-", msg)
		End If
	End Sub
	
	'解説メッセージが定義されているか？
	Public Function IsSysMessageDefined(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As Boolean
		Dim uclass, uname, msg As String
		Dim situations() As String
		Dim i, ret As Short
		
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation & "(解説)"
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")(解説)"
			situations(2) = main_situation & "(解説)"
		End If
		
		' ADD START MARGE
		'拡張戦闘アニメデータで検索
		If ExtendedAnimation Then
			With EADList
				For i = 1 To UBound(situations)
					'戦闘アニメ能力で指定された名称で検索
					If IsFeatureAvailable("戦闘アニメ") Then
						uname = FeatureData("戦闘アニメ")
						If .IsDefined(uname) Then
							msg = .Item(uname).SelectMessage(situations(i), Me)
							If Len(msg) > 0 Then
								IsSysMessageDefined = True
								Exit Function
							End If
						End If
					End If
					
					'ユニット名称で検索
					If .IsDefined(Name) Then
						msg = .Item(Name).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'ユニット愛称を修正したもので検索
					uname = Nickname0
					ret = InStr(uname, "(")
					If ret > 1 Then
						uname = Left(uname, ret - 1)
					End If
					ret = InStr(uname, "用")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					ret = InStr(uname, "型")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					If Right(uname, 4) = "カスタム" Then
						uname = Left(uname, Len(uname) - 4)
					End If
					If Right(uname, 1) = "改" Then
						uname = Left(uname, Len(uname) - 1)
					End If
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'ユニットクラスで検索
					uclass = Class0
					If .IsDefined(uclass) Then
						msg = .Item(uclass).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'汎用
					If .IsDefined("汎用") Then
						msg = .Item("汎用").SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				Next 
			End With
		End If
		' ADD END MARGE
		
		'戦闘アニメデータで検索
		With ADList
			For i = 1 To UBound(situations)
				'戦闘アニメ能力で指定された名称で検索
				If IsFeatureAvailable("戦闘アニメ") Then
					uname = FeatureData("戦闘アニメ")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				End If
				
				'ユニット名称で検索
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'ユニット愛称を修正したもので検索
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "用")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "型")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "カスタム" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "改" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'ユニットクラスで検索
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'汎用
				If .IsDefined("汎用") Then
					msg = .Item("汎用").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
			Next 
		End With
		
		'特殊効果データで検索
		With EDList
			For i = 1 To UBound(situations)
				'特殊効果能力で指定された名称で検索
				If IsFeatureAvailable("特殊効果") Then
					uname = FeatureData("特殊効果")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				End If
				
				'ユニット名称で検索
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'ユニット愛称を修正したもので検索
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "用")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "型")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "カスタム" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "改" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'ユニットクラスで検索
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'汎用
				If .IsDefined("汎用") Then
					msg = .Item("汎用").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
			Next 
		End With
		
		IsSysMessageDefined = False
	End Function
	
	
	' === 特殊効果関連処理 ===
	
	'特殊効果データを検索
	Public Function SpecialEffectData(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As String
		Dim uname, uclass As String
		Dim situations() As String
		Dim i, ret As Short
		
		'シチュエーションのリストを構築
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")"
			situations(2) = main_situation
		End If
		
		For i = 1 To UBound(situations)
			'特殊効果能力で指定された名称で検索
			If IsFeatureAvailable("特殊効果") Then
				uname = FeatureData("特殊効果")
				If EDList.IsDefined(uname) Then
					SpecialEffectData = EDList.Item(uname).SelectMessage(situations(i), Me)
					If Len(SpecialEffectData) > 0 Then
						Exit Function
					End If
				End If
			End If
			
			'ユニット名称で検索
			If EDList.IsDefined(Name) Then
				SpecialEffectData = EDList.Item(Name).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'ユニット愛称を修正したもので検索
			uname = Nickname
			ret = InStr(uname, "(")
			If ret > 1 Then
				uname = Left(uname, ret - 1)
			End If
			ret = InStr(uname, "用")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			ret = InStr(uname, "型")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			If Right(uname, 4) = "カスタム" Then
				uname = Left(uname, Len(uname) - 4)
			End If
			If Right(uname, 1) = "改" Then
				uname = Left(uname, Len(uname) - 1)
			End If
			If EDList.IsDefined(uname) Then
				SpecialEffectData = EDList.Item(uname).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'ユニットクラスで検索
			uclass = Class0
			If EDList.IsDefined(uclass) Then
				SpecialEffectData = EDList.Item(uclass).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'汎用
			If EDList.IsDefined("汎用") Then
				SpecialEffectData = EDList.Item("汎用").SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
		Next 
	End Function
	
	'特殊効果データを再生
	Public Sub SpecialEffect(ByRef main_situation As String, Optional ByRef sub_situation As String = "", Optional ByVal keep_message_form As Boolean = False)
		Dim anime, sname As String
		Dim animes() As String
		Dim idx, i, j, w As Short
		Dim ret As Double
		Dim buf As String
		Dim anime_head As Short
		Dim is_message_form_opened As Boolean
		Dim is_weapon As Boolean
		Dim is_ability As Boolean
		Dim in_bulk As Boolean
		Dim wait_time As Integer
		Dim need_refresh As Boolean
		Dim prev_obj_color As Integer
		Dim prev_obj_fill_color As Integer
		Dim prev_obj_fill_style As Integer
		Dim prev_obj_draw_width As Integer
		Dim prev_obj_draw_option As String
		Dim prev_selected_target As Unit
		
		'特殊効果データを検索
		anime = SpecialEffectData(main_situation, sub_situation)
		
		TrimString(anime)
		
		'表示キャンセル
		If anime = "" Or anime = "-" Then
			Exit Sub
		End If
		
		'マウスの右ボタンでキャンセル
		If IsRButtonPressed() Then
			'式評価のみ行う
			FormatMessage(anime)
			Exit Sub
		End If
		
		If BattleAnimation And Not IsOptionDefined("戦闘アニメ非自動選択") Then
			For i = 1 To CountWeapon
				If Weapon(i).Name = main_situation Then
					w = i
					Exit For
				End If
			Next 
			If w > 0 Then
				Select Case LCase(anime)
					Case "swing.wav"
						If InStr(main_situation, "槍") > 0 Or InStr(main_situation, "スピア") > 0 Or InStr(main_situation, "ランス") > 0 Or InStr(main_situation, "ジャベリン") > 0 Then
							ShowAnimation("刺突攻撃")
							Exit Sub
						ElseIf IsWeaponClassifiedAs(w, "武") Or IsWeaponClassifiedAs(w, "実") Then 
							ShowAnimation("白兵武器攻撃")
							Exit Sub
						End If
				End Select
			ElseIf InStr(main_situation, "(命中)") > 0 Then 
				Select Case LCase(anime)
					Case "break.wav"
						ShowAnimation("打撃命中")
						Exit Sub
					Case "combo.wav"
						ShowAnimation("乱打命中")
						Exit Sub
					Case "crash.wav"
						ShowAnimation("強打命中 Crash.wav")
						Exit Sub
					Case "explode.wav"
						ShowAnimation("爆発命中")
						Exit Sub
					Case "explode(far).wav"
						ShowAnimation("超爆発命中 Explode(Far).wav")
						Exit Sub
					Case "explode(nuclear).wav"
						ShowAnimation("超爆発命中 Explode(Nuclear).wav")
						Exit Sub
					Case "fire.wav"
						ShowAnimation("炎命中")
						Exit Sub
					Case "glass.wav"
						If IsWeaponClassifiedAs(w, "冷") Then
							ShowAnimation("凍結命中 Glass.wav")
						End If
						Exit Sub
					Case "punch.wav"
						ShowAnimation("打撃命中")
						Exit Sub
					Case "punch(2).wav", "punch(3).wav", "punch(4).wav"
						ShowAnimation("連打命中")
						Exit Sub
					Case "saber.wav", "slash.wav"
						ShowAnimation("斬撃命中 " & anime)
						Exit Sub
					Case "shock(low).wav"
						ShowAnimation("強打命中 Shock(Low).wav")
						Exit Sub
					Case "stab.wav"
						ShowAnimation("刺突命中")
						Exit Sub
					Case "thunder.wav"
						ShowAnimation("放電命中 Thunder.wav")
						Exit Sub
					Case "whip.wav"
						ShowAnimation("打撃命中 Whip.wav")
						Exit Sub
				End Select
			End If
		End If
		
		'メッセージウィンドウは表示されている？
		is_message_form_opened = frmMessage.Visible
		
		'オブジェクト色等を記録しておく
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'オブジェクト色等をデフォルトに戻す
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'検索するシチュエーションが武器名かどうか調べる
		For i = 1 To CountWeapon
			If main_situation = Weapon(i).Name Then
				is_weapon = True
				Exit For
			End If
		Next 
		
		'検索するシチュエーションがアビリティかどうか調べる
		For i = 1 To CountAbility
			If main_situation = Ability(i).Name Then
				is_ability = True
				Exit For
			End If
		Next 
		
		'イベント用ターゲットを記録しておく
		prev_selected_target = SelectedTargetForEvent
		
		'攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
		'自分自身をターゲットに設定する
		'(発動アニメではアニメ表示にSelectedTargetForEventが使われるため)
		If Not is_weapon And Not is_ability Then
			If SelectedTargetForEvent Is Nothing Then
				SelectedTargetForEvent = Me
			End If
		End If
		
		'特殊効果指定を分割
		ReDim animes(1)
		anime_head = 1
		For i = 1 To Len(anime)
			If Mid(anime, i, 1) = ";" Then
				animes(UBound(animes)) = Mid(anime, anime_head, i - anime_head)
				ReDim Preserve animes(UBound(animes) + 1)
				anime_head = i + 1
			End If
		Next 
		animes(UBound(animes)) = Mid(anime, anime_head)
		
		On Error GoTo ErrorHandler
		
		For i = 1 To UBound(animes)
			anime = animes(i)
			
			'式評価
			FormatMessage(anime)
			
			'画面クリア？
			If LCase(anime) = "clear" Then
				ClearPicture()
				need_refresh = True
				GoTo NextAnime
			End If
			
			'特殊効果
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'効果音
					PlayWave(anime)
					If wait_time > 0 Then
						If need_refresh Then
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).Refresh()
							need_refresh = False
						End If
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'カットインの表示
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
						need_refresh = False
					ElseIf Left(anime, 1) = "@" Then 
						need_refresh = False
					Else
						need_refresh = True
					End If
					DisplayBattleMessage("-", anime)
					GoTo NextAnime
			End Select
			
			Select Case LCase(LIndex(anime, 1))
				Case "line", "circle", "arc", "oval", "color", "fillcolor", "fillstyle", "drawwidth"
					'画面処理コマンド
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
						need_refresh = False
					Else
						need_refresh = True
					End If
					DisplayBattleMessage("-", anime)
					GoTo NextAnime
				Case "center"
					'指定したユニットを中央表示
					buf = GetValueAsString(ListIndex(anime, 2))
					If UList.IsDefined(buf) Then
						With UList.Item(buf)
							Center(.x, .y)
							RedrawScreen()
							need_refresh = False
						End With
					End If
					GoTo NextAnime
				Case "keep"
					'そのまま終了
					Exit For
			End Select
			
			'ウェイト？
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'メッセージ表示として処理
			If wait_time > 0 Then
				anime = VB6.Format(wait_time / 100) & ";" & anime
				wait_time = 0
			End If
			If Not frmMessage.Visible Then
				If SelectedTarget Is Me Then
					OpenMessageForm(Me)
				Else
					OpenMessageForm(SelectedTarget, Me)
				End If
			End If
			DisplayBattleMessage("-", anime)
			GoTo NextAnime
			
NextAnime: 
		Next 
		
		If BattleAnimation And Not IsPictureDrawn And Not IsOptionDefined("戦闘アニメ非自動選択") Then
			If w > 0 Then
				ShowAnimation("デフォルト攻撃")
			ElseIf InStr(main_situation, "(命中)") > 0 Then 
				ShowAnimation("ダメージ命中 -.wav")
			End If
		End If
		
		'特殊効果再生後にウェイトを入れる？
		If need_refresh Then
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
			need_refresh = False
		End If
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'画像を消去しておく
		If IsPictureDrawn And InStr(main_situation, "(準備)") = 0 And LCase(anime) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
		End If
		
		'最初から表示されていたのでなければメッセージウィンドウを閉じる
		If Not is_message_form_opened And Not keep_message_form Then
			CloseMessageForm()
		End If
		
		'オブジェクト色等を元に戻す
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		'イベント用ターゲットを元に戻す
		SelectedTargetForEvent = prev_selected_target
		
		Exit Sub
		
ErrorHandler: 
		
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Sub
	
	'特殊効果データが定義されているか？
	Public Function IsSpecialEffectDefined(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As Boolean
		Dim msg As String
		
		msg = SpecialEffectData(main_situation, sub_situation)
		
		If Len(msg) > 0 Then
			IsSpecialEffectDefined = True
		End If
	End Function
	
	
	' === 戦闘アニメ関連処理 ===
	
	'戦闘アニメデータを検索
	' MOD START MARGE
	'Public Function AnimationData(main_situation As String, sub_situation As String) As String
	Public Function AnimationData(ByRef main_situation As String, ByRef sub_situation As String, Optional ByVal ext_anime_only As Boolean = False) As String
		' MOD END MARGE
		Dim uname, uclass As String
		Dim situations() As String
		Dim i, ret As Short
		
		If Not BattleAnimation Then
			Exit Function
		End If
		
		'シチュエーションのリストを構築
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")"
			situations(2) = main_situation
		End If
		
		For i = 1 To UBound(situations)
			'戦闘アニメ能力で指定された名称で検索
			If IsFeatureAvailable("戦闘アニメ") Then
				uname = FeatureData("戦闘アニメ")
				' MOD START MARGE
				'            If ADList.IsDefined(uname) Then
				'                AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
				'                If Len(AnimationData) > 0 Then
				'                    Exit Function
				'                End If
				'            End If
				If ExtendedAnimation Then
					If EADList.IsDefined(uname) Then
						AnimationData = EADList.Item(uname).SelectMessage(situations(i), Me)
						If Len(AnimationData) > 0 Then
							Exit Function
						End If
					End If
				End If
				If Not ext_anime_only Then
					If ADList.IsDefined(uname) Then
						AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
						If Len(AnimationData) > 0 Then
							Exit Function
						End If
					End If
				End If
				' MOD END MARGE
			End If
			
			'ユニット名称で検索
			' MOD START MARGE
			'        If ADList.IsDefined(Name) Then
			'            AnimationData = ADList.Item(Name).SelectMessage(situations(i), Me)
			'            If Len(AnimationData) > 0 Then
			'                Exit Function
			'            End If
			'        End If
			If ExtendedAnimation Then
				If EADList.IsDefined(Name) Then
					AnimationData = EADList.Item(Name).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			If Not ext_anime_only Then
				If ADList.IsDefined(Name) Then
					AnimationData = ADList.Item(Name).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			' MOD END MARGE
			
			'ユニット愛称を修正したもので検索
			uname = Nickname0
			ret = InStr(uname, "(")
			If ret > 1 Then
				uname = Left(uname, ret - 1)
			End If
			ret = InStr(uname, "用")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			ret = InStr(uname, "型")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			If Right(uname, 4) = "カスタム" Then
				uname = Left(uname, Len(uname) - 4)
			End If
			If Right(uname, 1) = "改" Then
				uname = Left(uname, Len(uname) - 1)
			End If
			' MOD START MARGE
			'        If ADList.IsDefined(uname) Then
			'            AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
			'            If Len(AnimationData) > 0 Then
			'                Exit Function
			'            End If
			'        End If
			If ExtendedAnimation Then
				If EADList.IsDefined(uname) Then
					AnimationData = EADList.Item(uname).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			If Not ext_anime_only Then
				If ADList.IsDefined(uname) Then
					AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			' MOD END MARGE
			
			'ユニットクラスで検索
			uclass = Class0
			' MOD START MARGE
			'        If ADList.IsDefined(uclass) Then
			'            AnimationData = ADList.Item(uclass).SelectMessage(situations(i), Me)
			'            If Len(AnimationData) > 0 Then
			'                Exit Function
			'            End If
			'        End If
			If ExtendedAnimation Then
				If EADList.IsDefined(uclass) Then
					AnimationData = EADList.Item(uclass).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			If Not ext_anime_only Then
				If ADList.IsDefined(uclass) Then
					AnimationData = ADList.Item(uclass).SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			' MOD END MARGE
			
			'汎用
			' MOD START MARGE
			'        If ADList.IsDefined("汎用") Then
			'            AnimationData = ADList.Item("汎用").SelectMessage(situations(i), Me)
			'            If Len(AnimationData) > 0 Then
			'                Exit Function
			'            End If
			'        End If
			If ExtendedAnimation Then
				If EADList.IsDefined("汎用") Then
					AnimationData = EADList.Item("汎用").SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			If Not ext_anime_only Then
				If ADList.IsDefined("汎用") Then
					AnimationData = ADList.Item("汎用").SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			' MOD END MARGE
		Next 
	End Function
	
	'戦闘アニメを再生
	Public Sub PlayAnimation(ByRef main_situation As String, Optional ByRef sub_situation As String = "", Optional ByVal keep_message_form As Boolean = False)
		Dim anime, sname As String
		Dim animes() As String
		Dim j, i, idx As Short
		Dim ret As Double
		Dim buf As String
		Dim anime_head As Short
		Dim is_message_form_opened As Boolean
		Dim is_weapon As Boolean
		Dim is_ability As Boolean
		Dim in_bulk As Boolean
		Dim wait_time As Integer
		Dim need_refresh As Boolean
		Dim prev_obj_color As Integer
		Dim prev_obj_fill_color As Integer
		Dim prev_obj_fill_style As Integer
		Dim prev_obj_draw_width As Integer
		Dim prev_obj_draw_option As String
		Dim prev_selected_target As Unit
		
		'戦闘アニメデータを検索
		anime = AnimationData(main_situation, sub_situation)
		
		'見つからなかった場合は一括指定を試してみる
		If anime = "" Then
			Select Case Right(main_situation, 4)
				Case "(準備)", "(攻撃)", "(命中)"
					anime = AnimationData(Left(main_situation, Len(main_situation) - 4), sub_situation)
					in_bulk = True
				Case "(発動)"
					anime = AnimationData(Left(main_situation, Len(main_situation) - 4), sub_situation)
			End Select
		End If
		
		TrimString(anime)
		
		'表示キャンセル
		If anime = "" Or anime = "-" Then
			Exit Sub
		End If
		
		'マウスの右ボタンでキャンセル
		If IsRButtonPressed() Then
			' MOD START MARGE
			'        '式評価のみ行う
			'        FormatMessage anime
			'        Exit Sub
			'アニメの終了処理はキャンセルしない
			If main_situation <> "終了" And Right(main_situation, 4) <> "(終了)" Then
				'式評価のみ行う
				FormatMessage(anime)
				Exit Sub
			End If
			' MOD END MARGE
		End If
		
		'メッセージウィンドウは表示されている？
		is_message_form_opened = frmMessage.Visible
		
		'オブジェクト色等を記録しておく
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'オブジェクト色等をデフォルトに戻す
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'検索するシチュエーションが武器名かどうか調べる
		For i = 1 To CountWeapon
			If main_situation = Weapon(i).Name & "(攻撃)" Then
				is_weapon = True
				Exit For
			End If
		Next 
		
		'検索するシチュエーションがアビリティかどうか調べる
		For i = 1 To CountAbility
			If main_situation = Ability(i).Name & "(発動)" Then
				is_ability = True
				Exit For
			End If
		Next 
		
		'イベント用ターゲットを記録しておく
		prev_selected_target = SelectedTargetForEvent
		
		'攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
		'自分自身をターゲットに設定する
		'(発動アニメではアニメ表示にSelectedTargetForEventが使われるため)
		If Not is_weapon And Not is_ability Then
			If SelectedTargetForEvent Is Nothing Then
				SelectedTargetForEvent = Me
			End If
		End If
		
		'アニメ指定を分割
		ReDim animes(1)
		anime_head = 1
		For i = 1 To Len(anime)
			If Mid(anime, i, 1) = ";" Then
				animes(UBound(animes)) = Mid(anime, anime_head, i - anime_head)
				ReDim Preserve animes(UBound(animes) + 1)
				anime_head = i + 1
			End If
		Next 
		animes(UBound(animes)) = Mid(anime, anime_head)
		
		On Error GoTo ErrorHandler
		
		For i = 1 To UBound(animes)
			anime = animes(i)
			
			'最後に実行されたのがサブルーチン呼び出しかどうかを判定するため
			'サブルーチン名をあらかじめクリアしておく
			sname = ""
			
			'式評価
			FormatMessage(anime)
			
			'画面クリア？
			If LCase(anime) = "clear" Then
				ClearPicture()
				need_refresh = True
				GoTo NextAnime
			End If
			
			'戦闘アニメ以外の特殊効果
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'効果音
					PlayWave(anime)
					If wait_time > 0 Then
						If need_refresh Then
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).Refresh()
							need_refresh = False
						End If
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'カットインの表示
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
						need_refresh = False
					ElseIf Left(anime, 1) = "@" Then 
						need_refresh = False
					Else
						need_refresh = True
					End If
					DisplayBattleMessage("-", anime)
					GoTo NextAnime
			End Select
			
			Select Case LCase(LIndex(anime, 1))
				Case "line", "circle", "arc", "oval", "color", "fillcolor", "fillstyle", "drawwidth"
					'画面処理コマンド
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
						need_refresh = False
					Else
						need_refresh = True
					End If
					DisplayBattleMessage("-", anime)
					GoTo NextAnime
				Case "center"
					'指定したユニットを中央表示
					buf = GetValueAsString(ListIndex(anime, 2))
					If UList.IsDefined(buf) Then
						With UList.Item(buf)
							Center(.x, .y)
							RedrawScreen()
							need_refresh = False
						End With
					End If
					GoTo NextAnime
				Case "keep"
					'そのまま終了
					Exit For
			End Select
			
			'ウェイト？
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'サブルーチンの呼び出しが確定
			
			'戦闘アニメ再生用のサブルーチン名を作成
			sname = LIndex(anime, 1)
			If Left(sname, 1) = "@" Then
				sname = Mid(sname, 2)
			ElseIf is_weapon Then 
				'武器名の場合
				sname = "戦闘アニメ_" & sname & "攻撃"
			Else
				'その他の場合
				'括弧を含んだ武器名に対応するため、"("は後ろから検索
				idx = InStr2(main_situation, "(")
				
				'変形系のシチュエーションではサフィックスを無視
				If idx > 0 Then
					Select Case Left(main_situation, idx - 1)
						Case "変形", "ハイパーモード", "ノーマルモード", "パーツ分離", "合体", "分離"
							idx = 0
					End Select
				End If
				
				'武器名(攻撃無効化)の場合もサフィックスを無視
				If idx > 0 Then
					If Mid(main_situation, idx) = "(攻撃無効化)" Then
						idx = 0
					End If
				End If
				
				If idx > 0 Then
					'サフィックスあり
					sname = "戦闘アニメ_" & sname & Mid(main_situation, idx + 1, Len(main_situation) - idx - 1)
				Else
					sname = "戦闘アニメ_" & sname & "発動"
				End If
			End If
			
			'サブルーチンが見つからなかった
			If FindNormalLabel(sname) = 0 Then
				If in_bulk Then
					'一括指定を利用している場合
					Select Case Right(main_situation, 4)
						Case "(準備)"
							'表示をキャンセル
							GoTo NextAnime
						Case "(攻撃)"
							'複数のアニメ指定がある場合は諦めて他のものを使う
							If UBound(animes) > 1 Then
								GoTo NextAnime
							End If
							'そうでなければ「デフォルト」を使用
							sname = "戦闘アニメ_デフォルト攻撃"
						Case "(命中)"
							'複数のアニメ指定がある場合は諦めて他のものを使う
							If UBound(animes) > 1 Then
								GoTo NextAnime
							End If
							'そうでなければ「ダメージ」を使用
							sname = "戦闘アニメ_ダメージ命中"
					End Select
				Else
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
					End If
					If Not frmMessage.Visible Then
						If SelectedTarget Is Me Then
							OpenMessageForm(Me)
						Else
							OpenMessageForm(SelectedTarget, Me)
						End If
					End If
					DisplayBattleMessage("-", anime)
					GoTo NextAnime
				End If
			End If
			
			sname = "`" & sname & "`"
			
			'引数の構築
			For j = 2 To ListLength(anime)
				sname = sname & "," & ListIndex(anime, j)
			Next 
			If in_bulk Then
				sname = sname & ",`一括指定`"
			End If
			
			'戦闘アニメ再生前にウェイトを入れる
			If need_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
				need_refresh = False
			End If
			If wait_time > 0 Then
				Sleep(wait_time)
				wait_time = 0
			End If
			
			'画像描画が行われたかどうかの判定のためにフラグを初期化
			IsPictureDrawn = False
			
			'戦闘アニメ再生
			SaveBasePoint()
			CallFunction("Call(" & sname & ")", Expression.ValueType.StringType, buf, ret)
			RestoreBasePoint()
			
			'画像を消去しておく
			If IsPictureDrawn And LCase(buf) <> "keep" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
			End If
NextAnime: 
		Next 
		
		'戦闘アニメ再生後にウェイトを入れる？
		If need_refresh Then
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
			need_refresh = False
		End If
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'画像を消去しておく
		If IsPictureDrawn And sname = "" And InStr(main_situation, "(準備)") = 0 And LCase(anime) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
		End If
		
		'最初から表示されていたのでなければメッセージウィンドウを閉じる
		If Not is_message_form_opened And Not keep_message_form Then
			CloseMessageForm()
		End If
		
		'オブジェクト色等を元に戻す
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		'イベント用ターゲットを元に戻す
		SelectedTargetForEvent = prev_selected_target
		
		Exit Sub
		
ErrorHandler: 
		
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Sub
	
	'戦闘アニメが定義されているか？
	' MOD START MARGE
	'Public Function IsAnimationDefined(main_situation As String, _
	''    Optional sub_situation As String) As Boolean
	Public Function IsAnimationDefined(ByRef main_situation As String, Optional ByRef sub_situation As String = "", Optional ByVal ext_anime_only As Boolean = False) As Boolean
		' MOD END MARGE
		Dim anime As String
		
		' MOD START MARGE
		'    anime = AnimationData(main_situation, sub_situation)
		anime = AnimationData(main_situation, sub_situation, ext_anime_only)
		' MOD END MARGE
		
		If Len(anime) > 0 Then
			IsAnimationDefined = True
		Else
			IsAnimationDefined = False
		End If
	End Function
	
	
	
	'ユニットを(new_x,new_y)に配置
	Public Sub StandBy(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal smode As String = "")
		Dim j, i, k As Short
		Dim u As Unit
		
		'とりあえず地形を考慮せずにデフォルトのポジションを決めておく
		'(Createコマンドの後で空中移動用アイテムを付けるときのため)
		For i = 0 To 20
			For j = MaxLng(new_x - i, 1) To MinLng(new_x + i, MapWidth)
				For k = MaxLng(new_y - i, 1) To MinLng(new_y + i, MapHeight)
					If System.Math.Abs(new_x - j) + System.Math.Abs(new_y - k) = i Then
						If MapDataForUnit(j, k) Is Nothing Then
							x = j
							y = k
							GoTo DefaultPositionDefined
						End If
					End If
				Next 
			Next 
		Next 
DefaultPositionDefined: 
		
		'空いた場所を検索
		For i = 0 To 20
			'ユニット同士を隣接させずに配置する？
			' MOD START MARGE
			'        If smode = "部隊配置" Then
			If InStr(smode, "部隊配置") > 0 Then
				' MOD END MARGE
				If i Mod 2 <> 0 Then
					GoTo NextDistance
				End If
			End If
			'指定した場所の周りを調べる
			For j = MaxLng(new_x - i, 1) To MinLng(new_x + i, MapWidth)
				For k = MaxLng(new_y - i, 1) To MinLng(new_y + i, MapHeight)
					If System.Math.Abs(new_x - j) + System.Math.Abs(new_y - k) <> i Then
						GoTo NextLoop
					End If
					
					'既に他のユニットがいる？
					If Not MapDataForUnit(j, k) Is Nothing Then
						GoTo NextLoop
					End If
					
					'進入不能の地形？
					If TerrainMoveCost(j, k) > 100 Then
						GoTo NextLoop
					End If
					Select Case TerrainClass(j, k)
						Case "空"
							If Not IsTransAvailable("空") Then
								GoTo NextLoop
							End If
						Case "水"
							If Not IsTransAvailable("水上") And Not IsTransAvailable("空") And Adaption(3) = 0 Then
								GoTo NextLoop
							End If
						Case "深水"
							If Not IsTransAvailable("水上") And Not IsTransAvailable("空") And Not IsTransAvailable("水") Then
								GoTo NextLoop
							End If
					End Select
					
					'空き位置が見つかった
					x = j
					y = k
					GoTo ExitFor
NextLoop: 
				Next 
			Next 
NextDistance: 
		Next 
ExitFor: 
		
		'空いた場所がなかった？
		If x = 0 And y = 0 Then
			Status_Renamed = "待機"
			Exit Sub
		End If
		
		'他の形態と格納したユニットの座標も合わせておく
		For i = 1 To CountOtherForm
			With OtherForm(i)
				.x = x
				.y = y
			End With
		Next 
		For i = 1 To CountUnitOnBoard
			With UnitOnBoard(i)
				.x = x
				.y = y
			End With
		Next 
		
		'格納されていた場合はあらかじめ降ろしておく
		If Status_Renamed = "格納" Then
			For	Each u In UList
				With u
					For i = 1 To .CountUnitOnBoard
						If ID = .UnitOnBoard(i).ID Then
							.UnloadUnit(ID)
							GoTo EndLoop
						End If
					Next 
				End With
			Next u
EndLoop: 
		End If
		
		'Statusを更新
		Status_Renamed = "出撃"
		For i = 1 To CountOtherForm
			OtherForm(i).Status_Renamed = "他形態"
		Next 
		
		'ユニットのいる地形は？
		Select Case TerrainClass(x, y)
			Case "空"
				Area = "空中"
			Case "陸"
				If IsTransAvailable("地中") And Area = "地中" Then
					Area = "地中"
				ElseIf IsTransAvailable("空") And Adaption(1) >= Adaption(2) Then 
					Area = "空中"
				ElseIf IsTransAvailable("陸") Then 
					Area = "地上"
				Else
					Area = "空中"
				End If
			Case "屋内"
				If IsTransAvailable("空") And Adaption(1) >= Adaption(2) Then
					Area = "空中"
				ElseIf IsTransAvailable("陸") Then 
					Area = "地上"
				Else
					Area = "空中"
				End If
			Case "月面"
				If IsTransAvailable("空") Or IsTransAvailable("宇宙") Then
					Area = "宇宙"
				ElseIf IsTransAvailable("陸") Then 
					Area = "地上"
				Else
					Area = "宇宙"
				End If
			Case "水", "深水"
				If IsTransAvailable("空") And Adaption(1) >= Adaption(2) Then
					Area = "空中"
				ElseIf IsTransAvailable("水上") Then 
					Area = "水上"
				Else
					Area = "水中"
				End If
			Case "宇宙"
				Area = "宇宙"
			Case Else
				Area = "地上"
		End Select
		
		'マップに登録
		MapDataForUnit(x, y) = Me
		
		'ビットマップを作成
		If BitmapID = 0 Then
			BitmapID = MakeUnitBitmap(Me)
		End If
		
		'登場時アニメを表示
		' MOD START MARGE
		'    If (smode = "出撃" Or smode = "部隊配置") _
		''        And MainForm.Visible _
		''        And Not IsPictureVisible _
		''        And Not IsRButtonPressed() _
		''        And BitmapID > 0 _
		''    Then
		Dim fname As String
		Dim start_time, current_time As Integer
		If (InStr(smode, "出撃") > 0 Or InStr(smode, "部隊配置") > 0) And MainForm.Visible And Not IsPictureVisible And Not IsRButtonPressed() And BitmapID > 0 Then
			' MOD END MARGE
			
			'ユニット出現音
			PlayWave("UnitOn.wav")
			
			'表示させる画像
			Select Case Party0
				Case "味方", "ＮＰＣ"
					fname = "Bitmap\Event\AUnitOn0"
				Case "敵"
					fname = "Bitmap\Event\EUnitOn0"
				Case "中立"
					fname = "Bitmap\Event\NUnitOn0"
			End Select
			
			If FileExists(AppPath & fname & "1.bmp") Then
				'アニメ表示開始時刻を記録
				start_time = timeGetTime()
				
				For i = 1 To 4
					'画像を透過表示
					If DrawPicture(fname & VB6.Format(i) & ".bmp", MapToPixelX(x), MapToPixelY(y), 32, 32, 0, 0, 0, 0, "透過") = False Then
						Exit For
					End If
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMain(0).Refresh()
					
					'ウェイト
					Do 
						System.Windows.Forms.Application.DoEvents()
						current_time = timeGetTime()
					Loop While current_time < start_time + 15
					start_time = current_time
					
					'画像を消去
					ClearPicture()
				Next 
				
				'アニメ画像は上書きして消してしまうので……
				IsPictureVisible = False
			End If
		End If
		
		'ユニット画像をマップに描画
		If Not IsPictureVisible And MapFileName <> "" Then
			' MOD START MARGE
			'        If smode = "非同期" Then
			If InStr(smode, "非同期") > 0 Then
				' MOD END MARGE
				PaintUnitBitmap(Me, "リフレッシュ無し")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		
		'制御不能？
		If IsFeatureAvailable("制御不可") Then
			AddCondition("暴走", -1)
		End If
		
		Update()
		
		PList.UpdateSupportMod(Me)
	End Sub
	
	'ユニットを(new_x,new_y)に移動
	Public Sub Move(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal without_en_consumption As Boolean = False, Optional ByVal by_cancel As Boolean = False, Optional ByVal by_teleport_or_jump As Boolean = False)
		Dim prev_x, prev_y As Short
		Dim i As Short
		
		'ユニットをマップからいったん削除
		If MapDataForUnit(x, y) Is Me Then
			'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			MapDataForUnit(x, y) = Nothing
		End If
		If IsPictureVisible Then
			EraseUnitBitmap(x, y, False)
		Else
			EraseUnitBitmap(x, y, False)
		End If
		PList.UpdateSupportMod(Me)
		
		'ユニット位置を指定された座標に
		prev_x = x
		prev_y = y
		x = new_x
		y = new_y
		For i = 1 To CountOtherForm
			With OtherForm(i)
				.x = x
				.y = y
			End With
		Next 
		For i = 1 To CountUnitOnBoard
			With UnitOnBoard(i)
				.x = x
				.y = y
			End With
		Next 
		
		'指定された場所に既にユニットが存在？
		If Not MapDataForUnit(x, y) Is Nothing Then
			With MapDataForUnit(x, y)
				'合体？
				For i = 1 To .CountFeature
					If .Feature(i) = "合体" And LLength(.FeatureData(i)) = 3 Then
						If UList.IsDefined(LIndex(.FeatureData(i), 3)) Then
							If UList.Item(LIndex(.FeatureData(i), 3)).CurrentForm Is Me Then
								Combine()
								Exit Sub
							End If
						End If
					End If
				Next 
				
				'着艦？
				If Not .IsFeatureAvailable("母艦") Then
					ErrorMessage("合体元ユニット「" & Name & "」が複数あるため合体処理が出来ません")
					Exit Sub
				End If
			End With
			
			'着艦処理
			Land(MapDataForUnit(x, y), by_cancel)
			Exit Sub
		End If
		
		'移動先によるユニット位置変更
		Select Case TerrainClass(x, y)
			Case "空"
				Area = "空中"
			Case "陸", "屋内"
				Select Case Area
					Case "水中", "水上"
						Area = "地上"
					Case "宇宙"
						If IsTransAvailable("空") And Adaption(1) >= Adaption(2) Then
							Area = "空中"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "空中"
						End If
				End Select
			Case "月面"
				Select Case Area
					Case "地上", "地中"
						'変更なし
					Case Else
						If (IsTransAvailable("空") Or IsTransAvailable("宇宙")) And Adaption(4) >= Adaption(2) Then
							Area = "宇宙"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "宇宙"
						End If
				End Select
			Case "水", "深水"
				Select Case Area
					Case "地上"
						If IsTransAvailable("水上") Then
							Area = "水上"
						Else
							Area = "水中"
						End If
					Case "宇宙"
						Area = "水中"
				End Select
			Case "宇宙"
				Area = "宇宙"
		End Select
		
		'マップにユニットを登録
		MapDataForUnit(x, y) = Me
		
		'ユニット描画
		If Not IsPictureVisible Then
			If MoveAnimation And Not by_cancel And Not by_teleport_or_jump Then
				MoveUnitBitmap2(Me, 20)
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		
		'移動によるＥＮ消費
		If Not without_en_consumption Then
			Select Case Area
				Case "地上", "水上"
					If IsFeatureAvailable("ホバー移動") Then
						EN = EN - 5
					End If
				Case "空中", "宇宙"
					EN = EN - 5
				Case "地中"
					EN = EN - 10
			End Select
		End If
		
		'情報更新
		Update()
		PList.UpdateSupportMod(Me)
	End Sub
	
	'ユニットを(new_x,new_y)にジャンプ
	Public Sub Jump(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal do_refresh As Boolean = True)
		Dim j, i, k As Short
		
		'ユニットを一旦マップから削除
		'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		MapDataForUnit(x, y) = Nothing
		EraseUnitBitmap(x, y, do_refresh)
		PList.UpdateSupportMod(Me)
		
		'空き位置を検索
		For i = 0 To 10
			For j = MaxLng(new_x - i, 1) To MinLng(new_x + i, MapWidth)
				For k = MaxLng(new_y - i, 1) To MinLng(new_y + i, MapHeight)
					If System.Math.Abs(new_x - j) + System.Math.Abs(new_y - k) <> i Then
						GoTo NextLoop
					End If
					If Not MapDataForUnit(j, k) Is Nothing Then
						GoTo NextLoop
					End If
					If TerrainMoveCost(j, k) > 100 Then
						GoTo NextLoop
					End If
					Select Case TerrainClass(j, k)
						Case "空"
							If Not IsTransAvailable("空") Then
								GoTo NextLoop
							End If
						Case "水", "深水"
							If Not IsTransAvailable("水上") And Not IsTransAvailable("空") And Adaption(3) = 0 Then
								GoTo NextLoop
							End If
					End Select
					
					x = j
					y = k
					
					GoTo ExitFor
NextLoop: 
				Next 
			Next 
		Next 
ExitFor: 
		
		'他の形態と格納したユニットの座標を更新
		For i = 1 To CountOtherForm
			With OtherForm(i)
				.x = x
				.y = y
			End With
		Next 
		For i = 1 To CountUnitOnBoard
			With UnitOnBoard(i)
				.x = x
				.y = y
			End With
		Next 
		
		'移動先によるユニット位置変更
		Select Case TerrainClass(x, y)
			Case "空"
				Area = "空中"
			Case "陸", "屋内"
				Select Case Area
					Case "水中", "水上"
						Area = "地上"
					Case "宇宙"
						If IsTransAvailable("空") And Adaption(1) >= Adaption(2) Then
							Area = "空中"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "空中"
						End If
				End Select
			Case "月面"
				Select Case Area
					Case "地上", "地中"
						'変更なし
					Case Else
						If (IsTransAvailable("空") Or IsTransAvailable("宇宙")) And Adaption(4) >= Adaption(2) Then
							Area = "宇宙"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "宇宙"
						End If
				End Select
			Case "水", "深水"
				Select Case Area
					Case "地上"
						If IsTransAvailable("水上") Then
							Area = "水上"
						Else
							Area = "水中"
						End If
					Case "宇宙"
						Area = "水中"
				End Select
			Case "宇宙"
				Area = "宇宙"
		End Select
		
		'マップにユニットを登録
		MapDataForUnit(x, y) = Me
		
		'情報更新
		Update()
		PList.UpdateSupportMod(Me)
		
		'ユニット描画
		If do_refresh Then
			PaintUnitBitmap(Me)
		End If
	End Sub
	
	'マップ上から脱出
	Public Sub Escape(Optional ByVal smode As String = "")
		Dim u As Unit
		Dim i, j As Short
		
		'母艦に乗っていた場合は降りておく
		If Status_Renamed = "格納" Then
			For	Each u In UList
				With u
					For i = 1 To .CountUnitOnBoard
						If ID = .UnitOnBoard(i).ID Then
							.UnloadUnit(ID)
							GoTo EndLoop
						End If
					Next 
				End With
			Next u
EndLoop: 
		End If
		
		'出撃している場合は画面上からユニットを消去
		If Status_Renamed = "出撃" Or Status_Renamed = "破壊" Then
			If MapDataForUnit(x, y) Is Me Then
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(x, y) = Nothing
				If smode = "非同期" Or IsPictureVisible Or MapFileName = "" Then
					EraseUnitBitmap(x, y, False)
				Else
					EraseUnitBitmap(x, y, True)
				End If
				PList.UpdateSupportMod(Me)
			End If
		End If
		
		If Status_Renamed = "出撃" Or Status_Renamed = "格納" Then
			Status_Renamed = "待機"
		End If
		
		'破壊をキャンセル状態は解除
		If IsConditionSatisfied("破壊キャンセル") Then
			DeleteCondition("破壊キャンセル")
		End If
		
		'ユニットを格納していたら降ろす
		For	Each u In colUnitOnBoard
			With u
				.Status_Renamed = "待機"
				colUnitOnBoard.Remove(.ID)
			End With
		Next u
		
		'召喚したユニットを解放
		DismissServant()
		
		'魅了・憑依したユニットを解放
		DismissSlave()
		
		'ステータス表示中の場合は表示を解除
		If Me Is DisplayedUnit Then
			ClearUnitStatus()
		End If
	End Sub
	
	'母艦 u に着艦
	Public Sub Land(ByRef u As Unit, Optional ByVal by_cancel As Boolean = False, Optional ByVal is_event As Boolean = False)
		Dim tclass As String
		Dim i As Short
		
		'Landコマンドで着艦した場合
		If is_event Then
			If Status_Renamed = "出撃" Or Status_Renamed = "格納" Then
				Escape()
			Else
				'出撃のための前準備
				
				'ユニットが存在する位置を決定
				If u.Status_Renamed = "出撃" Then
					tclass = TerrainClass(u.x, u.y)
				Else
					tclass = TerrainClass(MapWidth \ 2, MapHeight \ 2)
				End If
				Select Case tclass
					Case "空"
						Area = "空中"
					Case "陸", "屋内"
						If IsTransAvailable("空") And Mid(strAdaption, 1, 1) = "A" Then
							Area = "空中"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "空中"
						End If
					Case "月面"
						If (IsTransAvailable("空") Or IsTransAvailable("宇宙")) And Mid(strAdaption, 4, 1) = "A" Then
							Area = "宇宙"
						ElseIf IsTransAvailable("陸") Then 
							Area = "地上"
						Else
							Area = "宇宙"
						End If
					Case "水", "深水"
						If IsTransAvailable("空") Then
							Area = "空中"
						ElseIf IsTransAvailable("水上") Then 
							Area = "水上"
						Else
							Area = "水中"
						End If
					Case "宇宙"
						Area = "宇宙"
				End Select
				
				'行動回数等を回復
				UsedAction = 0
				UsedSupportAttack = 0
				UsedSupportGuard = 0
				UsedSyncAttack = 0
				UsedCounterAttack = 0
				
				If BitmapID = 0 Then
					With UList.Item(Name)
						If .Party0 = Party0 And .BitmapID <> 0 And .Bitmap = Bitmap Then
							BitmapID = .BitmapID
						Else
							BitmapID = MakeUnitBitmap(Me)
						End If
					End With
				End If
				
				If IsFeatureAvailable("制御不可") Then
					AddCondition("暴走", -1)
				End If
			End If
		End If
		
		'母艦に自分自身を格納
		u.LoadUnit(Me)
		
		'座標を母艦に合わせる
		x = u.x
		y = u.y
		
		Status_Renamed = "格納"
		If Area <> "宇宙" And Area <> "空中" Then
			Area = "地上"
		End If
		
		'気力減少
		If Not by_cancel Then
			With MainPilot
				If .Personality <> "機械" Then
					If IsOptionDefined("母艦収納時気力低下小") Then
						.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
					Else
						.Morale = .Morale - 5
					End If
				End If
			End With
			For i = 1 To CountPilot
				With Pilot(i)
					If MainPilot.ID <> .ID And .Personality <> "機械" Then
						If IsOptionDefined("母艦収納時気力低下小") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			Next 
			For i = 1 To CountSupport
				With Support(i)
					If .Personality <> "機械" Then
						If IsOptionDefined("母艦収納時気力低下小") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			Next 
			If IsFeatureAvailable("追加サポート") Then
				With AdditionalSupport
					If .Personality <> "機械" Then
						If IsOptionDefined("母艦収納時気力低下小") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			End If
		End If
	End Sub
	
	' new_form へ変形（換装、ハイパーモード、パーツ分離＆合体を含む）
	Public Sub Transform(ByRef new_form As String)
		Dim list As String
		Dim i, idx, idx2, j As Short
		Dim u As Unit
		Dim wname() As String
		Dim wbullet() As Short
		Dim wmaxbullet() As Short
		Dim aname() As String
		Dim astock() As Short
		Dim amaxstock() As Short
		Dim hp_ratio, en_ratio As Double
		Dim prev_x, prev_y As Short
		Dim buf As String
		
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		
		u = OtherForm(new_form)
		u.Status_Renamed = Status_Renamed
		If Status_Renamed <> "破棄" Then
			Status_Renamed = "他形態"
		End If
		
		'制御不可能な形態から元に戻る場合は暴走を解除
		If IsFeatureAvailable("制御不可") Then
			If IsConditionSatisfied("暴走") Then
				DeleteCondition("暴走")
			End If
		End If
		
		'元の形態に戻る？
		If LIndex(FeatureData("ノーマルモード"), 1) = new_form Then
			If IsConditionSatisfied("ノーマルモード付加") Then
				'変身が解ける場合
				If MapFileName <> "" Then
					For i = 2 To LLength(FeatureData("ノーマルモード"))
						Select Case LIndex(FeatureData("ノーマルモード"), i)
							Case "消耗あり"
								AddCondition("消耗", 1)
							Case "気力低下"
								IncreaseMorale(-10)
						End Select
					Next 
				End If
				
				DeleteCondition("ノーマルモード付加")
				
				If IsConditionSatisfied("能力コピー") Then
					DeleteCondition("能力コピー")
					DeleteCondition("パイロット画像")
					DeleteCondition("メッセージ")
				End If
			Else
				'ハイパーモードが解ける場合
				If MapFileName <> "" Then
					AddCondition("消耗", 1)
					For i = 2 To LLength(FeatureData("ノーマルモード"))
						Select Case LIndex(FeatureData("ノーマルモード"), i)
							Case "消耗なし"
								DeleteCondition("消耗")
							Case "気力低下"
								IncreaseMorale(-10)
						End Select
					Next 
				End If
			End If
			
			If IsConditionSatisfied("残り時間") Then
				DeleteCondition("残り時間")
			End If
		End If
		
		'戦闘アニメで変更されたユニット画像を元に戻す
		If IsConditionSatisfied("ユニット画像") Then
			DeleteCondition("ユニット画像")
			BitmapID = MakeUnitBitmap(Me)
		End If
		If IsConditionSatisfied("非表示付加") Then
			DeleteCondition("非表示付加")
			BitmapID = MakeUnitBitmap(Me)
		End If
		
		Dim eu As Unit
		Dim counter As Short
		With u
			'パラメータ受け継ぎ
			.BossRank = BossRank
			.Rank = Rank
			.Mode = Mode
			.Area = Area
			.UsedSupportAttack = UsedSupportAttack
			.UsedSupportGuard = UsedSupportGuard
			.UsedSyncAttack = UsedSyncAttack
			.UsedCounterAttack = UsedCounterAttack
			
			.Master = Master
			'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Master = Nothing
			.Summoner = Summoner
			'UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Summoner = Nothing
			
			'アイテム受け継ぎ
			For i = 1 To CountItem
				.AddItem0(Item(i))
			Next 
			
			'スペシャルパワー効果のコピー
			CopySpecialPowerInEffect(u)
			RemoveAllSpecialPowerInEffect()
			
			'特殊ステータスのコピー
			For i = 1 To .CountCondition
				.DeleteCondition0(1)
			Next 
			For i = 1 To CountCondition
				If ConditionLifetime(i) <> 0 And InStr(ConditionData(i), "パイロット能力付加") = 0 And InStr(ConditionData(i), "パイロット能力強化") = 0 Then
					.AddCondition(Condition(i), ConditionLifetime(i), ConditionLevel(i), ConditionData(i))
				End If
			Next 
			For i = 1 To CountCondition
				DeleteCondition0(1)
			Next 
			
			'パイロットの乗せ換え
			list = FeatureData("変形")
			If LLength(list) > 0 And Data.PilotNum = -LLength(list) And CountPilot = LLength(list) Then
				'変形によりパイロットの順番が変化する場合
				For idx = 2 To LLength(list)
					If .Name = LIndex(list, idx) Then
						Exit For
					End If
				Next 
				If idx <= LLength(list) Then
					list = .FeatureData("変形")
					For idx2 = 2 To LLength(list)
						buf = LIndex(list, idx2)
						If Name = buf Then
							Exit For
						End If
					Next 
					j = 2
					For i = 1 To CountPilot
						Select Case i
							Case 1
								.AddPilot(Pilot(idx))
							Case idx2
								.AddPilot(Pilot(1))
							Case Else
								If idx = j Then
									j = j + 1
								End If
								.AddPilot(Pilot(j))
								j = j + 1
						End Select
					Next 
				Else
					For i = 1 To CountPilot
						.AddPilot(Pilot(i))
					Next 
				End If
			Else
				For i = 1 To CountPilot
					.AddPilot(Pilot(i))
				Next 
			End If
			For i = 1 To CountSupport
				.AddSupport(Support(i))
			Next 
			For i = 1 To CountUnitOnBoard
				.LoadUnit(UnitOnBoard(i))
			Next 
			For i = 1 To CountServant
				.AddServant(Servant(i))
			Next 
			For i = 1 To CountSlave
				.AddSlave(Slave(i))
			Next 
			
			For i = 1 To CountPilot
				DeletePilot(1)
			Next 
			For i = 1 To CountSupport
				DeleteSupport(1)
			Next 
			For i = 1 To CountUnitOnBoard
				UnloadUnit(1)
			Next 
			For i = 1 To CountServant
				DeleteServant(1)
			Next 
			For i = 1 To CountSlave
				DeleteSlave(1)
			Next 
			
			For i = 1 To .CountPilot
				.Pilot(i).Unit_Renamed = u
			Next 
			For i = 1 To .CountSupport
				.Support(i).Unit_Renamed = u
				If .Support(i).SupportIndex > 0 Then
					If IsFeatureAvailable("分離") And .IsFeatureAvailable("分離") Then
						For j = 2 To LLength(.FeatureData("分離"))
							If LIndex(FeatureData("分離"), .Support(i).SupportIndex + 1) = LIndex(.FeatureData("分離"), j) Then
								.Support(i).SupportIndex = j - 1
								Exit For
							End If
						Next 
					End If
				End If
			Next 
			
			.Update()
			
			'弾数データを記録
			ReDim wname(CountWeapon)
			ReDim wbullet(CountWeapon)
			ReDim wmaxbullet(CountWeapon)
			For i = 1 To CountWeapon
				wname(i) = Weapon(i).Name
				wbullet(i) = Bullet(i)
				wmaxbullet(i) = MaxBullet(i)
			Next 
			
			ReDim aname(CountAbility)
			ReDim astock(CountAbility)
			ReDim amaxstock(CountAbility)
			For i = 1 To CountAbility
				aname(i) = Ability(i).Name
				astock(i) = Stock(i)
				amaxstock(i) = MaxStock(i)
			Next 
			
			'弾数の受け継ぎ
			idx = 1
			For i = 1 To .CountWeapon
				counter = idx
				For j = counter To UBound(wname)
					If .Weapon(i).Name = wname(j) And .MaxBullet(i) > 0 And wmaxbullet(j) > 0 Then
						.SetBullet(i, wbullet(j) * .MaxBullet(i) \ wmaxbullet(j))
						idx = j + 1
						Exit For
					End If
				Next 
			Next 
			
			idx = 1
			For i = 1 To .CountAbility
				counter = idx
				For j = counter To UBound(aname)
					If .Ability(i).Name = aname(j) And .MaxStock(i) > 0 And amaxstock(j) > 0 Then
						.SetStock(i, astock(j) * .MaxStock(i) \ amaxstock(j))
						idx = j + 1
						Exit For
					End If
				Next 
			Next 
			
			'弾数・使用回数共有の実現
			.SyncBullet()
			
			'アイテムを削除
			For i = 1 To CountItem
				DeleteItem(1)
			Next 
			
			.Update()
			
			'ＨＰ＆ＥＮの受け継ぎ
			If new_form = LIndex(FeatureData("パーツ分離"), 2) Then
				.HP = .MaxHP
			Else
				.HP = .MaxHP * hp_ratio / 100
			End If
			.EN = .MaxEN * en_ratio / 100
			
			'ノーマルモードや制限時間つきの形態の場合は残り時間を付加
			If Not .IsConditionSatisfied("残り時間") Then
				If .IsFeatureAvailable("ノーマルモード") Then
					If IsNumeric(LIndex(.FeatureData("ノーマルモード"), 2)) Then
						If .IsConditionSatisfied("残り時間") Then
							.DeleteCondition("残り時間")
						End If
						.AddCondition("残り時間", CShort(LIndex(.FeatureData("ノーマルモード"), 2)))
					End If
				ElseIf .IsFeatureAvailable("制限時間") Then 
					.AddCondition("残り時間", CShort(.FeatureData("制限時間")))
				End If
			ElseIf Not .IsFeatureAvailable("ノーマルモード") And Not .IsFeatureAvailable("制限時間") Then 
				'残り時間が必要ない形態にTransformコマンドで強制変形された？
				.DeleteCondition("残り時間")
			End If
			
			Select Case .Status_Renamed
				Case "出撃"
					'変形後のユニットを出撃させる
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(x, y) = Nothing
					prev_x = x
					prev_y = y
					.UsedAction = UsedAction
					.StandBy(x, y)
					If .x <> prev_x Or .y <> prev_y Then
						EraseUnitBitmap(prev_x, prev_y, False)
					End If
				Case "格納"
					'変形後のユニットを格納する
					For	Each eu In UList
						With eu
							For j = 1 To .CountUnitOnBoard
								If ID = .UnitOnBoard(j).ID Then
									.UnloadUnit(ID)
									.LoadUnit(u)
									GoTo EndLoop
								End If
							Next 
						End With
					Next eu
EndLoop: 
			End Select
		End With
		
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'ハイパーモードが解ける場合
		buf = FeatureData("ノーマルモード")
		If LIndex(buf, 1) = new_form Then
			For i = 2 To LLength(buf)
				Select Case LIndex(buf, i)
					Case "回数制限"
						AddCondition("行動不能", -1)
				End Select
			Next 
		End If
	End Sub
	
	'合体
	Public Sub Combine(Optional ByRef uname As String = "", Optional ByVal is_event As Boolean = False)
		Dim k, i, j, l As Short
		Dim u As Unit
		Dim rarray() As Unit
		Dim prev_status As String
		Dim hp_ratio, en_ratio As Double
		Dim fdata As String
		
		prev_status = Status_Renamed
		
		If uname = "" Then
			'合体形態が指定されてなければその場所にいるユニットと２体合体
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			For i = 1 To CountFeature
				If Feature(i) = "合体" Then
					fdata = FeatureData(i)
					If LLength(fdata) = 3 And MapDataForUnit(x, y).Name = LIndex(fdata, 3) And UList.IsDefined(LIndex(fdata, 2)) Then
						u = UList.Item(LIndex(FeatureData(i), 2)).CurrentForm
						Exit For
					End If
				End If
			Next 
			If u Is Nothing Then
				For i = 1 To CountFeature
					If Feature(i) = "合体" Then
						fdata = FeatureData(i)
						If LLength(fdata) = 3 And MapDataForUnit(x, y).IsEqual(LIndex(fdata, 3)) And UList.IsDefined(LIndex(fdata, 2)) Then
							u = UList.Item(LIndex(fdata, 2)).CurrentForm
							Exit For
						End If
					End If
				Next 
			End If
			
			'合体のパートナーを調べる
			For i = 1 To u.CountFeature
				If u.Feature(i) = "分離" And LLength(u.FeatureData(i)) = 3 And ((IsEqual(LIndex(u.FeatureData(i), 2)) And MapDataForUnit(x, y).IsEqual(LIndex(u.FeatureData(i), 3))) Or (IsEqual(LIndex(u.FeatureData(i), 3)) And MapDataForUnit(x, y).IsEqual(LIndex(u.FeatureData(i), 2)))) Then
					Exit For
				End If
			Next 
		Else
			'合体ユニットが作成されていない
			If Not UList.IsDefined(uname) Then
				ErrorMessage(uname & "が作成されていません")
				ExitGame()
			End If
			
			u = UList.Item(uname).CurrentForm
			
			'合体のパートナーを調べる
			For i = 1 To u.CountFeature
				If u.Feature(i) = "分離" And LLength(u.FeatureData(i)) > 2 Then
					Exit For
				End If
			Next 
		End If
		
		'合体するユニットの配列を作成
		If i > u.CountFeature Then
			ErrorMessage(u.Name & "のデータに" & Name & "に対する分離指定がみつかりません。" & "書式を確認してください。")
			Exit Sub
		End If
		ReDim rarray(LLength(u.FeatureData(i)) - 1)
		For j = 1 To UBound(rarray)
			If Not UList.IsDefined(LIndex(u.FeatureData(i), j + 1)) Then
				ErrorMessage(LIndex(u.FeatureData(i), j + 1) & "が作成されていません")
				Exit Sub
			End If
			rarray(j) = UList.Item(LIndex(u.FeatureData(i), j + 1))
		Next 
		
		Dim BGM As String
		If Not is_event Then
			If Status_Renamed = "出撃" Then
				'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
				If u.IsFeatureAvailable("追加パイロット") Then
					If Not PList.IsDefined(u.FeatureData("追加パイロット")) Then
						If Not PDList.IsDefined(u.FeatureData("追加パイロット")) Then
							ErrorMessage(u.Name & "の追加パイロット「" & u.FeatureData("追加パイロット") & "」のデータが見つかりません")
							TerminateSRC()
						End If
						PList.Add(u.FeatureData("追加パイロット"), MainPilot.Level, Party0)
					End If
				End If
				
				If IsMessageDefined("合体(" & u.Name & ")") Or IsMessageDefined("合体(" & FeatureName("合体") & ")") Or IsMessageDefined("合体") Then
					If IsFeatureAvailable("合体ＢＧＭ") Then
						For i = 1 To CountFeature
							If Feature(i) = "合体ＢＧＭ" And LIndex(FeatureData(i), 1) = u.Name Then
								BGM = SearchMidiFile(Mid(FeatureData(i), InStr(FeatureData(i), " ") + 1))
								If Len(BGM) > 0 Then
									ChangeBGM(BGM)
									Sleep(500)
								End If
								Exit For
							End If
						Next 
					End If
					
					OpenMessageForm()
					If IsMessageDefined("合体(" & u.Name & ")") Then
						PilotMessage("合体(" & u.Name & ")")
					ElseIf IsMessageDefined("合体(" & FeatureName("合体") & ")") Then 
						PilotMessage("合体(" & FeatureName("合体") & ")")
					Else
						PilotMessage("合体")
					End If
					CloseMessageForm()
				End If
			End If
		End If
		
		'分離ユニットと合体ユニットが同名の武器を持つ場合は弾数を累積するため
		'このような武器の弾数を0にする
		For i = 1 To u.CountWeapon
			For j = 1 To UBound(rarray)
				With rarray(j).CurrentForm
					For k = 1 To .CountWeapon
						If u.Weapon(i).Name = .Weapon(k).Name Then
							u.SetBullet(i, 0)
							Exit For
						End If
					Next 
				End With
			Next 
		Next 
		'使用回数を合わせる
		For i = 1 To u.CountAbility
			For j = 1 To UBound(rarray)
				With rarray(j).CurrentForm
					For k = 1 To .CountAbility
						If u.Ability(i).Name = .Ability(k).Name Then
							u.SetStock(i, 0)
							Exit For
						End If
					Next 
				End With
			Next 
		Next 
		
		'１番目のユニットのステータスを合体後のユニットに継承
		With rarray(1).CurrentForm
			.CopySpecialPowerInEffect(u)
			.RemoveAllSpecialPowerInEffect()
			
			For i = 1 To .CountItem
				u.AddItem(.Item(i))
			Next 
			
			u.Master = .Master
			'UPGRADE_NOTE: オブジェクト rarray().CurrentForm.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			.Master = Nothing
			u.Summoner = .Summoner
			'UPGRADE_NOTE: オブジェクト rarray().CurrentForm.Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			.Summoner = Nothing
			
			u.UsedSupportAttack = .UsedSupportAttack
			u.UsedSupportGuard = .UsedSupportGuard
			u.UsedSyncAttack = .UsedSyncAttack
			u.UsedCounterAttack = .UsedCounterAttack
			
			For i = 1 To .CountServant
				u.AddServant(.Servant(i))
			Next 
			For i = 1 To .CountServant
				.DeleteServant(1)
			Next 
			For i = 1 To .CountSlave
				u.AddSlave(.Slave(i))
			Next 
			For i = 1 To .CountSlave
				.DeleteSlave(1)
			Next 
		End With
		
		'合体する各ユニットに対しての処理を行う
		Dim eu As Unit
		For i = 1 To UBound(rarray)
			'マップ上から撤退させる
			With rarray(i).CurrentForm
				Select Case .Status_Renamed
					Case "出撃"
						.Status_Renamed = "待機"
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(.x, .y) = Nothing
						EraseUnitBitmap(.x, .y)
					Case "格納"
						.Status_Renamed = "待機"
						For	Each eu In UList
							For j = 1 To eu.CountUnitOnBoard
								If .ID = eu.UnitOnBoard(j).ID Then
									eu.UnloadUnit(.ID)
									GoTo EndLoop
								End If
							Next 
						Next eu
EndLoop: 
				End Select
			End With
			
			'デフォルトの形態に変形させておく
			If Not rarray(i).CurrentForm Is rarray(i) Then
				rarray(i).CurrentForm.Transform((rarray(i).Name))
			End If
			
			With rarray(i)
				If i = 1 Then
					.Status_Renamed = "旧主形態"
				Else
					.Status_Renamed = "旧形態"
				End If
				
				hp_ratio = hp_ratio + 100 * .HP / .MaxHP
				en_ratio = en_ratio + 100 * .EN / .MaxEN
				
				If .Rank > u.Rank Then
					u.Rank = .Rank
				End If
				If .BossRank > u.BossRank Then
					u.BossRank = .BossRank
					u.FullRecover()
				End If
				
				If .IsFeatureAvailable("召喚ユニット") Then
					'召喚ユニットの場合はパイロットの乗せ換えは行わない
					If InStr(.MainPilot.Name, "(ザコ)") > 0 Or InStr(.MainPilot.Name, "(汎用)") > 0 Then
						'汎用パイロットの場合は削除
						.MainPilot.Alive = False
					End If
				Else
					'パイロットの乗せ換え
					For j = 1 To .CountPilot
						.Pilot(j).Ride(u)
					Next 
					For j = 1 To .CountPilot
						.DeletePilot(1)
					Next 
					
					'サポートの乗せ換え
					For j = 1 To .CountSupport
						.Support(j).Ride(u, True)
						.Support(j).SupportIndex = i
					Next 
					For j = 1 To .CountSupport
						.DeleteSupport(1)
					Next 
				End If
				
				'搭載ユニットの乗せ換え
				For j = 1 To .CountUnitOnBoard
					u.LoadUnit(.UnitOnBoard(j))
				Next 
				For j = 1 To u.CountUnitOnBoard
					.UnloadUnit(1)
				Next 
				
				'分離ユニットと共通する武装の弾数は一旦0にクリア
				For j = 1 To u.CountWeapon
					For k = 1 To .CountWeapon
						If u.Weapon(j).Name = .Weapon(k).Name Then
							u.SetBullet(j, 0)
							Exit For
						End If
					Next 
					For k = 1 To .CountOtherForm
						With .OtherForm(k)
							For l = 1 To .CountWeapon
								If u.Weapon(j).Name = .Weapon(l).Name Then
									u.SetBullet(j, 0)
									Exit For
								End If
							Next 
						End With
					Next 
				Next 
				
				'アビリティの使用回数も同様の処理を行う
				For j = 1 To u.CountAbility
					For k = 1 To .CountAbility
						If u.Ability(j).Name = .Ability(k).Name Then
							u.SetStock(j, 0)
							Exit For
						End If
					Next 
					For k = 1 To .CountOtherForm
						With .OtherForm(k)
							For l = 1 To .CountAbility
								If u.Ability(j).Name = .Ability(l).Name Then
									u.SetStock(j, 0)
									Exit For
								End If
							Next 
						End With
					Next 
				Next 
				
				'スペシャルパワーの効果を消去
				.RemoveAllSpecialPowerInEffect()
			End With
		Next 
		
		'合体後のユニットの武装の弾数及びアビリティの使用回数は分離ユニットの
		'弾数及び使用回数の合計に設定する
		For i = 1 To UBound(rarray)
			With rarray(i)
				'武装の弾数の処理
				For j = 1 To u.CountWeapon
					For k = 1 To .CountWeapon
						If u.Weapon(j).Name = .Weapon(k).Name Then
							u.SetBullet(j, u.Bullet(j) + .Bullet(k))
							GoTo NextWeapon
						End If
					Next 
					For k = 1 To .CountOtherForm
						With .OtherForm(k)
							For l = 1 To .CountWeapon
								If u.Weapon(j).Name = .Weapon(l).Name Then
									u.SetBullet(j, u.Bullet(j) + .Bullet(l))
									GoTo NextWeapon
								End If
							Next 
						End With
					Next 
NextWeapon: 
				Next 
				
				'アビリティの使用回数の処理
				For j = 1 To u.CountAbility
					For k = 1 To .CountAbility
						If u.Ability(j).Name = .Ability(k).Name Then
							u.SetStock(j, u.Stock(j) + .Stock(k))
							GoTo NextAbility
						End If
					Next 
					For k = 1 To .CountOtherForm
						With .OtherForm(k)
							For l = 1 To .CountAbility
								If u.Ability(j).Name = .Ability(l).Name Then
									u.SetStock(j, u.Stock(j) + .Stock(l))
									GoTo NextAbility
								End If
							Next 
						End With
					Next 
NextAbility: 
				Next 
			End With
		Next 
		
		'１番目のユニットのアイテムを外す
		With rarray(1)
			For i = 1 To .CountItem
				.DeleteItem(1)
			Next 
		End With
		
		'合体後のユニットに関する処理
		With u
			.Update()
			
			.Party = Party0
			For i = 1 To .CountOtherForm
				.OtherForm(i).Party = Party0
			Next 
			For i = 1 To .CountPilot
				.Pilot(i).Party = Party0
			Next 
			For i = 1 To .CountSupport
				.Support(i).Party = Party0
			Next 
			
			.HP = .MaxHP * hp_ratio / 100 / UBound(rarray)
			.EN = 1 * .MaxEN * en_ratio / 100 / UBound(rarray)
			
			'弾数・使用回数共有の実現
			.SyncBullet()
			
			If prev_status = "出撃" Then
				.StandBy(x, y)
				
				'ノーマルモードや制限時間つきの形態の場合は残り時間を付加
				If .IsFeatureAvailable("ノーマルモード") Then
					If IsNumeric(LIndex(.FeatureData("ノーマルモード"), 2)) Then
						If .IsConditionSatisfied("残り時間") Then
							.DeleteCondition("残り時間")
						End If
						.AddCondition("残り時間", CShort(LIndex(.FeatureData("ノーマルモード"), 2)))
					End If
				ElseIf .IsFeatureAvailable("制限時間") Then 
					.AddCondition("残り時間", CShort(.FeatureData("制限時間")))
				End If
			Else
				.Status_Renamed = prev_status
			End If
		End With
		
		'分離ユニットの座標を合体後のユニットの座標に合わせる
		For i = 1 To UBound(rarray)
			With rarray(i).CurrentForm
				.x = u.x
				.y = u.y
			End With
		Next 
	End Sub
	
	'分離
	'UPGRADE_NOTE: Split は Split_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Sub Split_Renamed()
		Dim k, i, j, l As Short
		Dim idx, n As Short
		Dim buf As String
		Dim u As Unit
		Dim uarray() As Unit
		Dim hp_ratio, en_ratio As Double
		Dim pname As String
		Dim p As Pilot
		
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		
		'まずは撤退
		If Status_Renamed = "出撃" Then
			'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			MapDataForUnit(x, y) = Nothing
			EraseUnitBitmap(x, y)
		End If
		
		'分離先のユニットを調べる
		buf = FeatureData("分離")
		ReDim uarray(LLength(buf) - 1)
		For i = 2 To LLength(buf)
			uarray(i - 1) = UList.Item(LIndex(buf, i))
			If uarray(i - 1) Is Nothing Then
				ErrorMessage(LIndex(buf, i - 1) & "が存在しません")
				Exit Sub
			End If
		Next 
		
		'分離後の１番機を検索
		For i = 1 To UBound(uarray)
			If uarray(i).Status_Renamed = "旧主形態" Then
				Exit For
			End If
		Next 
		If i > UBound(uarray) Then
			i = 1
		End If
		
		'１番機に現在のステータスを継承
		CopySpecialPowerInEffect(uarray(i))
		RemoveAllSpecialPowerInEffect()
		With uarray(i)
			For j = 1 To CountItem
				.AddItem(Item(j))
			Next 
			For j = 1 To CountItem
				DeleteItem(1)
			Next 
			
			.Master = Master
			'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Master = Nothing
			.Summoner = Summoner
			'UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Summoner = Nothing
			
			.UsedSupportAttack = UsedSupportAttack
			.UsedSupportGuard = UsedSupportGuard
			.UsedSyncAttack = UsedSyncAttack
			.UsedCounterAttack = UsedCounterAttack
			
			For j = 1 To CountServant
				.AddServant(Servant(j))
			Next 
			For j = 1 To CountServant
				DeleteServant(1)
			Next 
			For j = 1 To CountSlave
				.AddSlave(Slave(j))
			Next 
			For j = 1 To CountSlave
				DeleteSlave(1)
			Next 
		End With
		
		'各分離ユニットに対する処理
		n = 1
		Dim eu As Unit
		Dim counter As Short
		For i = 1 To UBound(uarray)
			With uarray(i)
				'召喚ユニットでない場合は陣営を合わせる
				If Not .IsFeatureAvailable("召喚ユニット") Then
					.Party = Party0
				End If
				
				'パイロットの搭乗
				If CountPilot > 0 Then
					For j = 1 To System.Math.Abs(.Data.PilotNum)
						If .IsFeatureAvailable("召喚ユニット") Then
							If Status_Renamed = "出撃" Or Status_Renamed = "格納" Then
								pname = .FeatureData("追加パイロット")
								If InStr(PDList.Item(pname).Name, "(ザコ)") > 0 Or InStr(PDList.Item(pname).Name, "(汎用)") > 0 Then
									p = PList.Add(pname, MainPilot.Level, Party)
									p.FullRecover()
								Else
									If Not PList.IsDefined(pname) Then
										p = PList.Add(pname, MainPilot.Level, Party)
										p.FullRecover()
									Else
										p = PList.Item(pname)
									End If
								End If
								p.Ride(uarray(i))
							End If
						Else
							If n <= CountPilot Then
								Pilot(n).Ride(uarray(i))
								n = n + 1
							ElseIf Not .IsFeatureAvailable("追加パイロット") Then 
								If CountSupport > 0 Then
									Support(1).Ride(uarray(i))
									DeleteSupport(1)
								Else
									ErrorMessage(Name & "分離後のユニットに載せる" & "パイロットが存在しません。" & "データのパイロット数を確認して下さい。")
									TerminateSRC()
								End If
							End If
						End If
					Next 
				End If
				
				.Update()
				
				'母艦の場合は格納したユニットを受け渡し
				If .IsFeatureAvailable("母艦") Then
					For j = 1 To CountUnitOnBoard
						.LoadUnit(UnitOnBoard(j))
					Next 
					For j = 1 To CountUnitOnBoard
						UnloadUnit(1)
					Next 
				End If
				
				'ＨＰ＆ＥＮの同期
				.HP = .MaxHP * hp_ratio / 100
				.EN = 1 * .MaxEN * en_ratio / 100
				
				'弾数を合わせる
				idx = 1
				For j = 1 To CountWeapon
					counter = idx
					For k = counter To .CountWeapon
						If Weapon(j).Name = .Weapon(k).Name And Weapon(j).Bullet > 0 And .Weapon(k).Bullet > 0 Then
							.SetBullet(k, .MaxBullet(k) * Bullet(j) \ MaxBullet(j))
							idx = k + 1
							Exit For
						End If
					Next 
				Next 
				For j = 1 To .CountOtherForm
					With .OtherForm(j)
						idx = 1
						For k = 1 To CountWeapon
							counter = idx
							For l = counter To .CountWeapon
								If Weapon(k).Name = .Weapon(l).Name And Weapon(k).Bullet > 0 And .Weapon(l).Bullet > 0 Then
									.SetBullet(l, .MaxBullet(l) * Bullet(k) \ MaxBullet(k))
									idx = l + 1
									Exit For
								End If
							Next 
						Next 
					End With
				Next 
				
				'使用回数を合わせる
				idx = 1
				For j = 1 To CountAbility
					counter = idx
					For k = counter To .CountAbility
						If Ability(j).Name = .Ability(k).Name And Ability(j).Stock > 0 And .Ability(k).Stock > 0 Then
							.SetStock(k, .Ability(k).Stock * Stock(j) \ MaxStock(j))
							idx = k + 1
							Exit For
						End If
					Next 
				Next 
				For j = 1 To .CountOtherForm
					With .OtherForm(j)
						idx = 1
						For k = 1 To CountAbility
							counter = idx
							For l = counter To .CountAbility
								If Ability(k).Name = .Ability(l).Name And Ability(k).Stock > 0 And .Ability(l).Stock > 0 Then
									.SetStock(l, .Ability(l).Stock * Stock(k) \ MaxStock(k))
									idx = l + 1
									Exit For
								End If
							Next 
						Next 
					End With
				Next 
				
				'弾数・使用回数共有の実現
				.SyncBullet()
				
				'出撃 or 格納？
				.Status_Renamed = Status_Renamed
				Select Case Status_Renamed
					Case "出撃"
						If i = 1 Then
							.UsedAction = UsedAction
						Else
							.UsedAction = MaxLng(UsedAction, .UsedAction)
							.UsedSupportAttack = 0
							.UsedSupportGuard = 0
							.UsedSyncAttack = 0
							.UsedCounterAttack = 0
						End If
						.StandBy(x, y)
					Case "格納"
						For	Each eu In UList
							With eu
								For j = 1 To .CountOtherForm
									If ID = .UnitOnBoard(j).ID Then
										.LoadUnit(uarray(i))
										GoTo EndLoop
									End If
								Next 
							End With
						Next eu
EndLoop: 
				End Select
				
				'ノーマルモードや制限時間つきの形態の場合は残り時間を付加
				If .IsFeatureAvailable("ノーマルモード") Then
					If IsNumeric(LIndex(.FeatureData("ノーマルモード"), 2)) Then
						If .IsConditionSatisfied("残り時間") Then
							.DeleteCondition("残り時間")
						End If
						.AddCondition("残り時間", CShort(LIndex(.FeatureData("ノーマルモード"), 2)))
					End If
				ElseIf .IsFeatureAvailable("制限時間") Then 
					.AddCondition("残り時間", CShort(.FeatureData("制限時間")))
				End If
			End With
		Next 
		
		'パイロットを合体ユニットから削除
		For i = 1 To CountPilot
			DeletePilot(1)
		Next 
		
		'サポートパイロットの乗り換え
		For i = 1 To CountSupport
			With Support(i)
				If .SupportIndex = 0 Then
					.Ride(UList.Item(LIndex(buf, 2)))
				Else
					.Ride(uarray(.SupportIndex))
				End If
			End With
		Next 
		For i = 1 To CountSupport
			DeleteSupport(1)
		Next 
		
		'格納されている場合は母艦から自分のエントリーを外しておく
		If Status_Renamed = "格納" Then
			For	Each u In UList
				With u
					For j = 1 To .CountUnitOnBoard
						If ID = .UnitOnBoard(j).ID Then
							.UnloadUnit(ID)
							GoTo EndLoop2
						End If
					Next 
				End With
			Next u
EndLoop2: 
		End If
		
		Status_Renamed = "他形態"
		
		'ユニットステータスコマンドの場合以外は制限時間付き合体ユニットは
		'２度とその形態を利用できない
		If MapFileName = "" Then
			Exit Sub
		End If
		If IsFeatureAvailable("制限時間") Then
			AddCondition("行動不能", -1)
		End If
	End Sub
	
	'経験値を入手
	't:ターゲット
	'exp_situation:経験値入手の理由
	'exp_mode:マップ攻撃による入手？
	Public Function GetExp(ByRef t As Unit, ByRef exp_situation As String, Optional ByRef exp_mode As String = "") As Integer
		Dim xp As Integer
		Dim j, i, n As Short
		Dim prev_level As Short
		Dim prev_stype() As String
		Dim prev_sname() As String
		Dim prev_slevel() As Double
		Dim prev_special_power() As String
		Dim stype, sname As String
		Dim p As Pilot
		Dim msg As String
		
		'経験値を入手するのは味方ユニット及びＮＰＣの召喚ユニットのみ
		If (Party <> "味方" Or Party0 <> "味方") And (Party <> "ＮＰＣ" Or Party0 <> "ＮＰＣ" Or Not IsFeatureAvailable("召喚ユニット")) Then
			Exit Function
		End If
		
		'メインパイロットの現在の能力を記録
		With MainPilot
			prev_level = .Level
			ReDim prev_special_power(.CountSpecialPower)
			For i = 1 To .CountSpecialPower
				prev_special_power(i) = .SpecialPower(i)
			Next 
			ReDim prev_stype(.CountSkill)
			ReDim prev_sname(.CountSkill)
			ReDim prev_slevel(.CountSkill)
			For i = 1 To .CountSkill
				prev_stype(i) = .Skill(i)
				prev_sname(i) = .SkillName(i)
				prev_slevel(i) = .SkillLevel(i, "基本値")
			Next 
		End With
		
		'ターゲットが指定されていない場合は自分がターゲット
		If t Is Nothing Then
			t = Me
		End If
		
		'ターゲットにパイロットが乗っていない場合は経験値なし
		If t.CountPilot = 0 Then
			Exit Function
		End If
		
		'ユニットに乗っているパイロット総数を計算
		n = CountPilot + CountSupport
		If IsFeatureAvailable("追加サポート") Then
			n = n + 1
		End If
		
		'各パイロットが経験値を入手
		For i = 1 To n
			If i <= CountPilot Then
				p = Pilot(i)
			ElseIf i <= CountPilot + CountSupport Then 
				p = Support(i - CountPilot)
			Else
				p = AdditionalSupport
			End If
			
			Select Case exp_situation
				Case "破壊"
					xp = t.ExpValue + t.MainPilot.ExpValue
					If IsUnderSpecialPowerEffect("獲得経験値増加") And exp_mode <> "パートナー" Then
						xp = xp * (1 + 0.1 * SpecialPowerEffectLevel("獲得経験値増加"))
					End If
				Case "攻撃"
					xp = (t.ExpValue + t.MainPilot.ExpValue) \ 10
					If IsUnderSpecialPowerEffect("獲得経験値増加") And exp_mode <> "パートナー" Then
						xp = xp * (1 + 0.1 * SpecialPowerEffectLevel("獲得経験値増加"))
					End If
				Case "アビリティ"
					If t Is Me Then
						xp = 50
					Else
						xp = 100
					End If
				Case "修理"
					xp = 100
				Case "補給"
					xp = 150
			End Select
			If Not IsUnderSpecialPowerEffect("獲得経験値増加") Or IsOptionDefined("収得効果重複") Then
				If p.IsSkillAvailable("素質") Then
					If p.IsSkillLevelSpecified("素質") Then
						xp = xp * (10 + p.SkillLevel("素質")) \ 10
					Else
						xp = 1.5 * xp
					End If
				End If
			End If
			If p.IsSkillAvailable("遅成長") Then
				xp = xp \ 2
			End If
			
			'対象のパイロットのレベル差による修正
			Select Case t.MainPilot.Level - p.Level
				Case Is > 7
					xp = 5 * xp
				Case 7
					xp = 4.5 * xp
				Case 6
					xp = 4 * xp
				Case 5
					xp = 3.5 * xp
				Case 4
					xp = 3 * xp
				Case 3
					xp = 2.5 * xp
				Case 2
					xp = 2 * xp
				Case 1
					xp = 1.5 * xp
				Case 0
				Case -1
					xp = xp \ 2
				Case -2
					xp = xp \ 4
				Case -3
					xp = xp \ 6
				Case -4
					xp = xp \ 8
				Case -5
					xp = xp \ 10
				Case Is < -5
					xp = xp \ 12
			End Select
			p.Exp = p.Exp + xp
			
			'一番目のパイロットが獲得した経験値を返す
			If i = 1 Then
				GetExp = xp
			End If
		Next 
		
		'追加パイロットの場合、一番目のパイロットにレベル、経験値を合わせる
		If Not MainPilot Is Pilot(1) Then
			MainPilot.Level = Pilot(1).Level
			MainPilot.Exp = Pilot(1).Exp
		End If
		
		'召喚主も経験値を入手
		If Not Summoner Is Nothing Then
			Summoner.CurrentForm.GetExp(t, exp_situation, "パートナー")
		End If
		
		'マップ攻撃による経験値収得の場合はメッセージ表示を省略
		If exp_mode = "マップ" Then
			Exit Function
		End If
		
		'経験値入手時のメッセージ
		With MainPilot
			If .Level > prev_level Then
				'レベルアップ
				
				If IsAnimationDefined("レベルアップ") Then
					PlayAnimation("レベルアップ")
				ElseIf IsSpecialEffectDefined("レベルアップ") Then 
					SpecialEffect("レベルアップ")
				End If
				If IsMessageDefined("レベルアップ") Then
					PilotMessage("レベルアップ")
				End If
				
				msg = .Nickname & "は経験値[" & VB6.Format(GetExp) & "]を獲得、" & "レベル[" & VB6.Format(.Level) & "]にレベルアップ。"
				
				'特殊能力の習得
				For i = 1 To .CountSkill
					stype = .Skill(i)
					sname = .SkillName(i)
					If InStr(sname, "非表示") = 0 Then
						Select Case stype
							Case "同調率", "霊力", "追加レベル", "魔力所有"
							Case "ＳＰ消費減少", "スペシャルパワー自動発動", "ハンター"
								For j = 1 To UBound(prev_stype)
									If stype = prev_stype(j) Then
										If sname = prev_sname(j) Then
											Exit For
										End If
									End If
								Next 
								If j > UBound(prev_stype) Then
									msg = msg & ";" & sname & "を習得した。"
								End If
							Case Else
								For j = 1 To UBound(prev_stype)
									If stype = prev_stype(j) Then
										Exit For
									End If
								Next 
								If j > UBound(prev_stype) Then
									msg = msg & ";" & sname & "を習得した。"
								ElseIf .SkillLevel(i, "基本値") > prev_slevel(j) Then 
									msg = msg & ";" & prev_sname(j) & " => " & sname & "。"
								End If
						End Select
					End If
				Next 
				
				'スペシャルパワーの習得
				If .CountSpecialPower > UBound(prev_special_power) Then
					msg = msg & ";" & Term("スペシャルパワー", Me)
					For i = 1 To .CountSpecialPower
						sname = .SpecialPower(i)
						For j = 1 To UBound(prev_special_power)
							If sname = prev_special_power(j) Then
								Exit For
							End If
						Next 
						If j > UBound(prev_special_power) Then
							msg = msg & "「" & sname & "」"
						End If
					Next 
					msg = msg & "を習得した。"
				End If
				
				DisplaySysMessage(msg)
				If MessageWait < 10000 Then
					Sleep(MessageWait)
				End If
				
				HandleEvent("レベルアップ", .ID)
				
				PList.UpdateSupportMod(Me)
			ElseIf GetExp > 0 Then 
				DisplaySysMessage(.Nickname & "は" & VB6.Format(GetExp) & "の経験値を得た。")
			End If
		End With
	End Function
	
	'ユニットの陣営を変更
	Public Sub ChangeParty(ByRef new_party As String)
		Dim i As Short
		
		'陣営を変更
		Party = new_party
		
		'ビットマップを作り直す
		BitmapID = MakeUnitBitmap(Me)
		
		'パイロットの陣営を変更
		For i = 1 To CountPilot
			Pilot(i).Party = new_party
		Next 
		For i = 1 To CountSupport
			Support(i).Party = new_party
		Next 
		If IsFeatureAvailable("追加サポート") Then
			AdditionalSupport.Party = new_party
		End If
		
		'他形態の陣営を変更
		For i = 1 To CountOtherForm
			OtherForm(i).Party = new_party
			OtherForm(i).BitmapID = 0
		Next 
		
		'出撃中？
		If Status_Renamed = "出撃" Then
			'自分の陣営のステージなら行動可能に
			If Party = Stage Then
				Rest()
			End If
			'マップ上のユニット画像を更新
			PaintUnitBitmap(Me)
		End If
		
		PList.UpdateSupportMod(Me)
		
		'思考モードを通常に
		Mode = "通常"
	End Sub
	
	'ユニットに乗っているパイロットの気力をnumだけ増減
	'is_event:イベントによる気力増減(性格を無視して気力操作)
	Public Sub IncreaseMorale(ByVal num As Short, Optional ByVal is_event As Boolean = False)
		Dim p As Pilot
		
		If CountPilot = 0 Then
			Exit Sub
		End If
		
		'メインパイロット
		With MainPilot
			If .Personality <> "機械" Or is_event Then
				.Morale = .Morale + num
			End If
		End With
		
		'サブパイロット
		For	Each p In colPilot
			With p
				If MainPilot.ID <> .ID And (.Personality <> "機械" Or is_event) Then
					.Morale = .Morale + num
				End If
			End With
		Next p
		
		'サポート
		For	Each p In colSupport
			With p
				If .Personality <> "機械" Or is_event Then
					.Morale = .Morale + num
				End If
			End With
		Next p
		
		'追加サポート
		If IsFeatureAvailable("追加サポート") Then
			With AdditionalSupport
				If .Personality <> "機械" Or is_event Then
					.Morale = .Morale + num
				End If
			End With
		End If
	End Sub
	
	'ユニットが破壊された時の処理
	Public Sub Die(Optional ByVal without_update As Boolean = False)
		Dim i, j As Short
		Dim pname As String
		Dim p As Pilot
		
		HP = 0
		Status_Renamed = "破壊"
		
		'破壊をキャンセルし、破壊イベント内で処理をしたい場合
		If IsConditionSatisfied("破壊キャンセル") Then
			DeleteCondition("破壊キャンセル")
			GoTo SkipExplode
		End If
		
		'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		MapDataForUnit(x, y) = Nothing
		
		'爆発表示
		ClearPicture()
		If IsAnimationDefined("脱出") Then
			EraseUnitBitmap(x, y, False)
			PlayAnimation("脱出")
		ElseIf IsSpecialEffectDefined("脱出") Then 
			EraseUnitBitmap(x, y, False)
			SpecialEffect("脱出")
		Else
			DieAnimation(Me)
		End If
		
SkipExplode: 
		
		'召喚したユニットを解放
		DismissServant()
		
		'魅了・憑依したユニットを解放
		DismissSlave()
		
		If Not Master Is Nothing Then
			Master.CurrentForm.DeleteSlave(ID)
			'UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Master = Nothing
		End If
		
		If Not Summoner Is Nothing Then
			Summoner.CurrentForm.DeleteServant(ID)
			'UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Summoner = Nothing
		End If
		
		'支配しているユニットを強制退却
		If IsFeatureAvailable("支配") Then
			For i = 2 To LLength(FeatureData("支配"))
				pname = LIndex(FeatureData("支配"), i)
				For	Each p In PList
					With p
						If .Name = pname Or .Nickname = pname Then
							If Not .Unit_Renamed Is Nothing Then
								If .Unit_Renamed.Status_Renamed = "出撃" Or .Unit_Renamed.Status_Renamed = "格納" Then
									.Unit_Renamed.Die(True)
								End If
							End If
						End If
					End With
				Next p
			Next 
		End If
		
		'情報更新
		If Not without_update Then
			PList.UpdateSupportMod(Me)
		End If
	End Sub
	
	'スペシャルパワー自爆による自爆
	Public Sub SuicidalExplosion(Optional ByVal is_event As Boolean = False)
		Dim i, j As Short
		Dim prev_hp As Integer
		Dim u, t As Unit
		Dim dmg, tdmg As Integer
		Dim uname, fname As String
		
		PilotMessage("自爆")
		DisplaySysMessage(Nickname & "は自爆した。")
		
		'ダメージ量設定
		dmg = HP
		
		'効果範囲の設定
		AreaInRange(x, y, 1, 1, "")
		MaskData(x, y) = True
		
		'爆発
		EraseUnitBitmap(x, y)
		ExplodeAnimation(Size, x, y)
		
		'パーツ分離できれば自爆後にパーツ分離
		If IsFeatureAvailable("パーツ分離") Then
			uname = LIndex(FeatureData("パーツ分離"), 2)
			If OtherForm(uname).IsAbleToEnter(x, y) Then
				Transform(uname)
				MapDataForUnit(x, y).HP = MapDataForUnit(x, y).MaxHP
				fname = FeatureName("パーツ分離")
				If IsSysMessageDefined("破壊時分離(" & Name & ")") Then
					SysMessage("破壊時分離(" & Name & ")")
				ElseIf IsSysMessageDefined("破壊時分離(" & fname & ")") Then 
					SysMessage("破壊時分離(" & fname & ")")
				ElseIf IsSysMessageDefined("破壊時分離") Then 
					SysMessage("破壊時分離")
				ElseIf IsSysMessageDefined("分離(" & Name & ")") Then 
					SysMessage("分離(" & Name & ")")
				ElseIf IsSysMessageDefined("分離(" & fname & ")") Then 
					SysMessage("分離(" & fname & ")")
				ElseIf IsSysMessageDefined("分離") Then 
					SysMessage("分離")
				Else
					DisplaySysMessage(Nickname & "は破壊されたパーツを分離させた。")
				End If
				GoTo SkipSuicide
			End If
		End If
		
		'自分を破壊
		HP = 0
		UpdateMessageForm(Me)
		'既に爆発アニメーションを表示しているので
		AddCondition("破壊キャンセル", 1)
		'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		MapDataForUnit(x, y) = Nothing
		Die()
		If Not is_event Then
			u = SelectedUnit
			SelectedUnit = Me
			HandleEvent("破壊", MainPilot.ID)
			SelectedUnit = u
			If IsScenarioFinished Then
				IsScenarioFinished = False
				Exit Sub
			End If
		End If
		
SkipSuicide: 
		
		'周りのエリアに爆発効果を適用
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				If MaskData(i, j) Then
					GoTo NextLoop
				End If
				
				t = MapDataForUnit(i, j)
				
				If t Is Nothing Then
					GoTo NextLoop
				End If
				
				With t
					ClearMessageForm()
					If CurrentForm.Party = "味方" Or CurrentForm.Party = "ＮＰＣ" Then
						UpdateMessageForm(t, CurrentForm)
					Else
						UpdateMessageForm(CurrentForm, t)
					End If
					
					'ダメージの適用
					prev_hp = .HP
					If .IsConditionSatisfied("無敵") Then
						tdmg = 0
					ElseIf .IsConditionSatisfied("不死身") Then 
						.HP = MaxLng(.HP - dmg, 10)
						tdmg = prev_hp - .HP
					Else
						.HP = .HP - dmg
						tdmg = prev_hp - .HP
					End If
					
					'特殊能力「不安定」による暴走チェック
					If .IsFeatureAvailable("不安定") Then
						If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("暴走") Then
							.AddCondition("暴走", -1)
							.Update()
						End If
					End If
					
					'ダメージを受ければ眠りからさめる
					If .IsConditionSatisfied("睡眠") Then
						.DeleteCondition("睡眠")
					End If
					
					If CurrentForm.Party = "味方" Or CurrentForm.Party = "ＮＰＣ" Then
						UpdateMessageForm(t, CurrentForm)
					Else
						UpdateMessageForm(CurrentForm, t)
					End If
					If .HP > 0 Then
						DrawSysString(.x, .y, VB6.Format(tdmg))
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.picMain(0).Refresh()
					End If
					
					If .HP = 0 Then
						If .IsUnderSpecialPowerEffect("復活") Then
							.HP = .MaxHP
							.RemoveSpecialPowerInEffect("破壊")
							DisplaySysMessage(.Nickname & "は復活した！")
							GoTo NextLoop
						End If
						
						If .IsFeatureAvailable("パーツ分離") Then
							uname = LIndex(.FeatureData("パーツ分離"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
								DisplaySysMessage(.Nickname & "は破壊されたパーツを分離させた。")
								GoTo NextLoop
							End If
						End If
						
						.Die()
					End If
					
					If Not is_event Then
						u = SelectedUnit
						SelectedUnit = .CurrentForm
						SelectedTarget = Me
						If .Status_Renamed = "破壊" Then
							DisplaySysMessage(.Nickname & "は破壊された")
							HandleEvent("破壊", .MainPilot.ID)
						Else
							DisplaySysMessage(.Nickname & "は" & tdmg & "のダメージを受けた。;" & "残りＨＰは" & VB6.Format(.HP) & "（損傷率 = " & 100 * (.MaxHP - .HP) \ .MaxHP & "％）")
							HandleEvent("損傷率", .MainPilot.ID, 100 - .HP * 100 \ .MaxHP)
						End If
						SelectedUnit = u
						If IsScenarioFinished Then
							IsScenarioFinished = False
							Exit Sub
						End If
					End If
				End With
NextLoop: 
			Next 
		Next 
	End Sub
	
	
	' === ステータス回復関連処理 ===
	
	'ステータスを全回復
	Public Sub FullRecover()
		Dim i, j As Short
		
		'パイロットのステータスを全回復
		For i = 1 To CountPilot
			Pilot(i).FullRecover()
		Next 
		For i = 1 To CountSupport
			Support(i).FullRecover()
		Next 
		If IsFeatureAvailable("追加パイロット") Then
			If PList.IsDefined(FeatureData("追加パイロット")) Then
				PList.Item(FeatureData("追加パイロット")).FullRecover()
			End If
		End If
		
		With CurrentForm
			'ＨＰを回復
			.HP = .MaxHP
			
			'ＥＮ、弾数を回復
			.FullSupply()
			
			'ステータス異常のみを消去
			i = 1
			Do While i <= .CountCondition
				If .Condition(i) = "残り時間" Or .Condition(i) = "非操作" Or Right(.Condition(i), 2) = "付加" Or Right(.Condition(i), 2) = "強化" Or Right(.Condition(i), 3) = "付加２" Or Right(.Condition(i), 3) = "強化２" Or Right(.Condition(i), 2) = "ＵＰ" Then
					i = i + 1
				Else
					.DeleteCondition(i)
				End If
			Loop 
			
			'サポートアタック＆ガード、同時援護攻撃、カウンター攻撃回数回復
			.UsedSupportAttack = 0
			.UsedSupportGuard = 0
			.UsedSyncAttack = 0
			.UsedCounterAttack = 0
			
			.Mode = "通常"
			
			'他形態も回復
			For i = 1 To .CountOtherForm
				With .OtherForm(i)
					.HP = .MaxHP
					.EN = .MaxEN
					For j = 1 To .CountWeapon
						.SetBullet(j, .MaxBullet(j))
					Next 
					For j = 1 To .CountAbility
						.SetStock(j, .MaxStock(j))
					Next 
				End With
			Next 
		End With
	End Sub
	
	'ＥＮ＆弾数を回復
	Public Sub FullSupply()
		Dim i, j As Short
		
		'ＥＮ回復
		EN = MaxEN
		
		'弾数回復
		For i = 1 To CountWeapon
			dblBullet(i) = 1
		Next 
		For i = 1 To CountAbility
			dblStock(i) = 1
		Next 
		
		'他形態も回復
		For i = 1 To CountOtherForm
			With OtherForm(i)
				.EN = .MaxEN
				For j = 1 To .CountWeapon
					.SetBullet(j, .MaxBullet(j))
				Next 
				For j = 1 To .CountAbility
					.SetStock(j, .MaxStock(j))
				Next 
			End With
		Next 
	End Sub
	
	'弾数のみを回復
	Public Sub BulletSupply()
		Dim i, j As Short
		
		For i = 1 To CountWeapon
			dblBullet(i) = 1
		Next 
		
		'他形態も回復
		For i = 1 To CountOtherForm
			With OtherForm(i)
				For j = 1 To .CountWeapon
					.SetBullet(j, .MaxBullet(j))
				Next 
			End With
		Next 
	End Sub
	
	'ＨＰを percent ％回復
	Public Sub RecoverHP(ByVal percent As Double)
		HP = HP + MaxHP * percent / 100
		If HP <= 0 Then
			HP = 1
		End If
		
		'特殊能力「不安定」による暴走チェック
		If IsFeatureAvailable("不安定") Then
			If HP <= MaxHP \ 4 And Not IsConditionSatisfied("暴走") Then
				AddCondition("暴走", -1)
			End If
		End If
	End Sub
	
	'ＥＮを percent ％回復
	Public Sub RecoverEN(ByVal percent As Double)
		EN = EN + MaxEN * percent / 100
		If EN <= 0 Then
			EN = 0
		End If
	End Sub
	
	'ターン経過によるステータス回復
	Public Sub Rest()
		Dim hp_recovery, en_recovery As Integer
		Dim hp_up, en_up As Integer
		Dim hp_ratio, en_ratio As Double
		Dim spname, buf As String
		Dim i, j As Short
		Dim u As Unit
		Dim td As TerrainData
		Dim cname As String
		Dim is_time_limit As Boolean
		Dim next_form As String
		' ADD START MARGE
		Dim is_terrain_effective As Boolean
		Dim is_immune_to_terrain_effect As Boolean
		' ADD END MARGE
		
		'味方ステージの1ターン目(スタートイベント直後)は回復を行わない
		If Stage = "味方" And Turn = 1 Then
			Exit Sub
		End If
		
		'データ更新
		Update()
		
		'変形に対応して自分を登録
		u = Me
		
		With MainPilot
			'霊力回復
			If .MaxPlana > 0 Then
				hp_ratio = 100 * HP / MaxHP
				en_ratio = 100 * EN / MaxEN
				
				.Plana = .Plana + .MaxPlana \ 16 + .MaxPlana * FeatureLevel("霊力回復") \ 10 - .MaxPlana * FeatureLevel("霊力消費") \ 10
				
				HP = MaxHP * hp_ratio \ 100
				EN = MaxEN * en_ratio \ 100
			End If
			
			'ＳＰ回復
			If .IsSkillAvailable("ＳＰ回復") Then
				.SP = .SP + .Level \ 8 + 5
			End If
			If .IsSkillAvailable("精神統一") Then
				If .SP < .MaxSP \ 5 Then
					.SP = .SP + .MaxSP \ 10
				End If
			End If
		End With
		
		'ＳＰ回復
		For i = 2 To CountPilot
			With Pilot(i)
				If .IsSkillAvailable("ＳＰ回復") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("精神統一") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		Next 
		For i = 1 To CountSupport
			With Support(i)
				If .IsSkillAvailable("ＳＰ回復") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("精神統一") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		Next 
		If IsFeatureAvailable("追加サポート") Then
			With AdditionalSupport()
				If .IsSkillAvailable("ＳＰ回復") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("精神統一") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		End If
		
		'行動回数
		UsedAction = 0
		
		'スペシャルパワー効果を解除
		RemoveSpecialPowerInEffect("ターン")
		
		'スペシャルパワー自動発動
		With MainPilot
			For i = 1 To .CountSkill
				If .Skill(i) = "スペシャルパワー自動発動" Then
					spname = LIndex(.SkillData(i), 2)
					If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) And Not IsSpecialPowerInEffect(spname) Then
						Center(x, y)
						.UseSpecialPower(spname, 0)
						If Status_Renamed = "他形態" Then
							Exit Sub
						End If
					End If
				End If
			Next 
			If IsConditionSatisfied("スペシャルパワー自動発動付加") Or IsConditionSatisfied("スペシャルパワー自動発動付加２") Then
				spname = LIndex(.SkillData("スペシャルパワー自動発動"), 2)
				If .Morale >= StrToLng(LIndex(.SkillData("スペシャルパワー自動発動"), 3)) And Not IsSpecialPowerInEffect(spname) Then
					Center(x, y)
					.UseSpecialPower(spname, 0)
					If Status_Renamed = "他形態" Then
						Exit Sub
					End If
				End If
			End If
		End With
		For i = 2 To CountPilot
			With Pilot(i)
				For j = 1 To .CountSkill
					If .Skill(j) = "スペシャルパワー自動発動" Then
						spname = LIndex(.SkillData(j), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(j), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "他形態" Then
								Exit Sub
							End If
						End If
					End If
				Next 
			End With
		Next 
		For i = 1 To CountSupport
			With Support(i)
				For j = 1 To .CountSkill
					If .Skill(j) = "スペシャルパワー自動発動" Then
						spname = LIndex(.SkillData(j), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(j), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "他形態" Then
								Exit Sub
							End If
						End If
					End If
				Next 
			End With
		Next 
		If IsFeatureAvailable("追加サポート") Then
			With AdditionalSupport
				For i = 1 To .CountSkill
					If .Skill(i) = "スペシャルパワー自動発動" Then
						spname = LIndex(.SkillData(i), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "他形態" Then
								Exit Sub
							End If
						End If
					End If
				Next 
			End With
		End If
		
		'起死回生
		With MainPilot
			If .IsSkillAvailable("起死回生") And .SP <= .MaxSP \ 5 And HP <= MaxHP \ 5 And EN <= MaxEN \ 5 Then
				.SP = .MaxSP
				HP = MaxHP
				EN = MaxEN
				If SpecialPowerAnimation Then
					Center(x, y)
					If SPDList.IsDefined("ド根性") Then
						SPDList.Item("ド根性").PlayAnimation()
					End If
				End If
			End If
		End With
		
		'ＨＰとＥＮ回復＆消費
		' MOD START MARGE
		'    If Not IsConditionSatisfied("回復不能") Then
		If Not IsConditionSatisfied("回復不能") And Not IsSpecialPowerInEffect("回復不能") Then
			' MOD END MARGE
			If IsFeatureAvailable("ＨＰ回復") Then
				hp_recovery = 10 * FeatureLevel("ＨＰ回復")
			End If
			If IsFeatureAvailable("ＥＮ回復") Then
				en_recovery = 10 * FeatureLevel("ＥＮ回復")
			End If
		End If
		If IsFeatureAvailable("ＨＰ消費") Then
			hp_recovery = hp_recovery - 10 * FeatureLevel("ＨＰ消費")
		End If
		If IsFeatureAvailable("ＥＮ消費") Then
			en_recovery = en_recovery - 10 * FeatureLevel("ＥＮ消費")
		End If
		
		'毒によるＨＰ減少
		Dim plv As Short
		If IsConditionSatisfied("毒") Then
			
			If IsOptionDefined("毒効果大") And BossRank < 0 Then
				plv = 25
			Else
				plv = 10
			End If
			
			If Weakness("毒") Then
				plv = 2 * plv
			ElseIf Effective("毒") Then 
				'変化なし
			ElseIf Immune("毒") Or Absorb("毒") Then 
				plv = 0
			ElseIf Resist("毒") Then 
				plv = plv \ 2
			End If
			
			hp_recovery = hp_recovery - plv
		End If
		
		'活動限界時間切れ？
		If ConditionLifetime("活動限界") = 1 Then
			Center(x, y)
			Escape()
			OpenMessageForm()
			DisplaySysMessage(Nickname & "は強制的に退却させられた。")
			CloseMessageForm()
			HandleEvent("破壊", MainPilot.ID)
		End If
		
		'死の宣告
		If ConditionLifetime("死の宣告") = 1 Then
			hp_recovery = hp_recovery - 1000
		End If
		
		'残り時間
		If ConditionLifetime("残り時間") = 1 Then
			is_time_limit = True
			If IsFeatureAvailable("ノーマルモード") Then
				'ハイパーモード＆変身の時間切れの場合は戻り先の形態を記録しておく
				next_form = LIndex(FeatureData("ノーマルモード"), 1)
			End If
		End If
		
		'ＨＰ回復などを付加した場合のことを考えて状態のアップデートは
		'ＨＰ＆ＥＮ回復量を計算した後に行う
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		UpdateCondition(True)
		HP = MaxHP * hp_ratio \ 100
		EN = MaxEN * en_ratio \ 100
		
		'サポートアタック＆ガード
		UsedSupportAttack = 0
		UsedSupportGuard = 0
		
		'同時援護攻撃
		UsedSyncAttack = 0
		
		'カウンター攻撃
		UsedCounterAttack = 0
		
		'チャージ完了？
		If ConditionLifetime("チャージ") = 0 Then
			AddCondition("チャージ完了", 1)
		End If
		
		'付加された移動能力が切れた場合の処理
		If Status_Renamed = "出撃" And MapFileName <> "" Then
			Select Case Area
				Case "空中"
					If Not IsTransAvailable("空") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "は強制的に退却させられた。")
							CloseMessageForm()
							HandleEvent("破壊", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "地上"
					If Not IsTransAvailable("陸") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "は強制的に退却させられた。")
							CloseMessageForm()
							HandleEvent("破壊", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "水上"
					If Not IsFeatureAvailable("水上移動") And Not IsFeatureAvailable("ホバー移動") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "は強制的に退却させられた。")
							CloseMessageForm()
							HandleEvent("破壊", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "水中"
					If Adaption(3) = 0 Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "は強制的に退却させられた。")
							CloseMessageForm()
							HandleEvent("破壊", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
			End Select
		End If
		
		If Status_Renamed = "格納" Then
			'格納時は回復率ＵＰ
			' MOD START MARGE
			'        If Not IsConditionSatisfied("回復不能") Then
			If Not IsConditionSatisfied("回復不能") And Not IsSpecialPowerInEffect("回復不能") Then
				' MOD END MARGE
				hp_recovery = hp_recovery + 50
				en_recovery = en_recovery + 50
			End If
			
			'弾数回復
			For i = 1 To CountWeapon
				dblBullet(i) = 1
			Next 
			For i = 1 To CountAbility
				dblStock(i) = 1
			Next 
		Else
			' MOD START MARGE
			'        '格納されてない場合は地形による回復修正
			'        If Not IsConditionSatisfied("回復不能") Then
			'            hp_recovery = hp_recovery + TerrainEffectForHPRecover(X, Y)
			'            en_recovery = en_recovery + TerrainEffectForENRecover(X, Y)
			'        End If
			'
			'        '地形による減少修正＆状態付加
			'        Set td = TDList.Item(MapData(X, Y, 0))
			'        With td
			'            For i = 1 To .CountFeature
			'                Select Case .Feature(i)
			'                    Case "ＨＰ減少"
			'                        If Weakness(.FeatureData(i)) Then
			'                            hp_recovery = hp_recovery - 20 * .FeatureLevel(i)
			'                        ElseIf Effective(.FeatureData(i)) Then
			'                            hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
			'                        ElseIf Not Immune(.FeatureData(i)) Then
			'                            If Absorb(.FeatureData(i)) Then
			'                                hp_recovery = hp_recovery + 10 * .FeatureLevel(i)
			'                            ElseIf Resist(.FeatureData(i)) Then
			'                                hp_recovery = hp_recovery - 5 * .FeatureLevel(i)
			'                            Else
			'                                hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
			'                            End If
			'                        End If
			'
			'                    Case "ＥＮ減少"
			'                        If Weakness(.FeatureData(i)) Then
			'                            en_recovery = en_recovery - 20 * .FeatureLevel(i)
			'                        ElseIf Effective(.FeatureData(i)) Then
			'                            en_recovery = en_recovery - 10 * .FeatureLevel(i)
			'                        ElseIf Not Immune(.FeatureData(i)) Then
			'                            If Absorb(.FeatureData(i)) Then
			'                                en_recovery = en_recovery + 10 * .FeatureLevel(i)
			'                            ElseIf Resist(.FeatureData(i)) Then
			'                                en_recovery = en_recovery - 5 * .FeatureLevel(i)
			'                            Else
			'                                en_recovery = en_recovery - 10 * .FeatureLevel(i)
			'                            End If
			'                        End If
			'
			'                    Case "ＨＰ増加"
			'                        If Not IsConditionSatisfied("回復不能") Then
			'                            hp_up = hp_up + 1000 * .FeatureLevel(i)
			'                        End If
			'
			'                    Case "ＥＮ増加"
			'                        If Not IsConditionSatisfied("回復不能") Then
			'                            en_up = en_up + 10 * .FeatureLevel(i)
			'                        End If
			'
			'                    Case "ＨＰ低下"
			'                        If Weakness(.FeatureData(i)) Then
			'                            hp_up = hp_up - 2000 * .FeatureLevel(i)
			'                        ElseIf Effective(.FeatureData(i)) Then
			'                            hp_up = hp_up - 1000 * .FeatureLevel(i)
			'                        ElseIf Not Immune(.FeatureData(i)) Then
			'                            If Absorb(.FeatureData(i)) Then
			'                                hp_up = hp_up + 1000 * .FeatureLevel(i)
			'                            ElseIf Resist(.FeatureData(i)) Then
			'                                hp_up = hp_up - 500 * .FeatureLevel(i)
			'                            Else
			'                                hp_up = hp_up - 1000 * .FeatureLevel(i)
			'                            End If
			'                        End If
			'
			'                    Case "ＥＮ低下"
			'                        If Weakness(.FeatureData(i)) Then
			'                            en_up = en_up - 20 * .FeatureLevel(i)
			'                        ElseIf Effective(.FeatureData(i)) Then
			'                            en_up = en_up - 10 * .FeatureLevel(i)
			'                        ElseIf Not Immune(.FeatureData(i)) Then
			'                            If Absorb(.FeatureData(i)) Then
			'                                en_up = en_up + 10 * .FeatureLevel(i)
			'                            ElseIf Resist(.FeatureData(i)) Then
			'                                en_up = en_up - 5 * .FeatureLevel(i)
			'                            Else
			'                                en_up = en_up - 10 * .FeatureLevel(i)
			'                            End If
			'                        End If
			'
			'                    Case "状態付加"
			'                        cname = .FeatureData(i)
			'
			'                        '状態が無効化されるかチェック
			'                        Select Case cname
			'                            Case "装甲劣化"
			'                                If SpecialEffectImmune("劣") Then
			'                                    cname = ""
			'                                End If
			'                            Case "混乱"
			'                                If SpecialEffectImmune("乱") Then
			'                                    cname = ""
			'                                End If
			'                            Case "恐怖"
			'                                If SpecialEffectImmune("恐") Then
			'                                    cname = ""
			'                                End If
			'                            Case "踊り"
			'                                If SpecialEffectImmune("踊") Then
			'                                    cname = ""
			'                                End If
			'                            Case "狂戦士"
			'                                If SpecialEffectImmune("狂") Then
			'                                    cname = ""
			'                                End If
			'                            Case "ゾンビ"
			'                                If SpecialEffectImmune("ゾ") Then
			'                                    cname = ""
			'                                End If
			'                            Case "回復不能"
			'                                If SpecialEffectImmune("害") Then
			'                                    cname = ""
			'                                End If
			'                            Case "石化"
			'                                If SpecialEffectImmune("石") Then
			'                                    cname = ""
			'                                End If
			'                            Case "凍結"
			'                                If SpecialEffectImmune("凍") Then
			'                                    cname = ""
			'                                End If
			'                            Case "麻痺"
			'                                If SpecialEffectImmune("痺") Then
			'                                    cname = ""
			'                                End If
			'                            Case "睡眠"
			'                                If SpecialEffectImmune("眠") Then
			'                                    cname = ""
			'                                End If
			'                            Case "毒"
			'                                If SpecialEffectImmune("毒") Then
			'                                    cname = ""
			'                                End If
			'                            Case "盲目"
			'                                If SpecialEffectImmune("盲") Then
			'                                    cname = ""
			'                                End If
			'                            Case "沈黙"
			'                                If SpecialEffectImmune("黙") Then
			'                                    cname = ""
			'                                End If
			'                            '属性使用不能状態
			'                            Case "オーラ使用不能"
			'                                If SpecialEffectImmune("剋オ") Then
			'                                    cname = ""
			'                                End If
			'                            Case "超能力使用不能"
			'                                If SpecialEffectImmune("剋超") Then
			'                                    cname = ""
			'                                End If
			'                            Case "同調率使用不能"
			'                                If SpecialEffectImmune("剋シ") Then
			'                                    cname = ""
			'                                End If
			'                            Case "超感覚使用不能"
			'                                If SpecialEffectImmune("剋サ") Then
			'                                    cname = ""
			'                                End If
			'                            Case "知覚強化使用不能"
			'                                If SpecialEffectImmune("剋サ") Then
			'                                    cname = ""
			'                                End If
			'                            Case "霊力使用不能"
			'                                If SpecialEffectImmune("剋霊") Then
			'                                    cname = ""
			'                                End If
			'                            Case "術使用不能"
			'                                If SpecialEffectImmune("剋術") Then
			'                                    cname = ""
			'                                End If
			'                            Case "技使用不能"
			'                                If SpecialEffectImmune("剋技") Then
			'                                    cname = ""
			'                                End If
			'                            Case Else
			'                                If Len(cname) > 6 Then
			'                                    If Right$(cname, 6) = "属性弱点付加" Then
			'                                        If SpecialEffectImmune("弱" & Left$(cname, Len(cname) - 6)) _
			''                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
			''                                            Or Immune(Left$(cname, Len(cname) - 6)) _
			''                                        Then
			'                                            cname = ""
			'                                        End If
			'                                    ElseIf Right$(cname, 6) = "属性有効付加" Then
			'                                        If SpecialEffectImmune("有" & Left$(cname, Len(cname) - 6)) _
			''                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
			''                                            Or Immune(Left$(cname, Len(cname) - 6)) _
			''                                        Then
			'                                            cname = ""
			'                                        End If
			'                                    ElseIf Right$(cname, 6) = "属性使用不能" Then
			'                                        If SpecialEffectImmune("剋" & Left$(cname, Len(cname) - 6)) Then
			'                                            cname = ""
			'                                        End If
			'                                    End If
			'                                End If
			'                        End Select
			'
			'                        If cname <> "" Then
			'                            If .IsFeatureLevelSpecified(i) Then
			'                                AddCondition cname, .FeatureLevel(i)
			'                            Else
			'                                AddCondition cname, 10000
			'                            End If
			'                        End If
			'                End Select
			'            Next
			'        End With
			'格納されてない場合は地形による各種修正＆状態付加
			'MOD START 240a
			'        Set td = TDList.Item(MapData(X, Y, 0))
			'レイヤーの状態に応じて上層下層どちらを取得するか判別
			Select Case MapData(x, y, Map.MapDataIndex.BoxType)
				Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.TerrainType))
				Case Else
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.LayerType))
			End Select
			'MOD START 240a
			With td
				'地形効果が適用される位置にいるかを判定
				If .IsFeatureAvailable("効果範囲") Then
					For i = 1 To LLength(.FeatureData("効果範囲"))
						If Area = LIndex(.FeatureData("効果範囲"), i) Then
							is_terrain_effective = True
							Exit For
						End If
					Next 
				Else
					is_terrain_effective = True
				End If
				
				'地形効果に対する無効化能力を持っているか
				If IsFeatureAvailable("地形効果無効化") Then
					If LLength(FeatureData("地形効果無効化")) > 1 Then
						For i = 2 To LLength(FeatureData("地形効果無効化"))
							If .Name = LIndex(FeatureData("地形効果無効化"), i) Then
								is_immune_to_terrain_effect = True
								Exit For
							End If
						Next 
					Else
						is_immune_to_terrain_effect = True
					End If
				ElseIf IsSpecialPowerInEffect("地形効果無効化") Then 
					is_immune_to_terrain_effect = True
				End If
				
				'地形効果を適用
				If is_terrain_effective Then
					For i = 1 To .CountFeature
						If Not IsConditionSatisfied("回復不能") And Not IsSpecialPowerInEffect("回復不能") Then
							Select Case .Feature(i)
								Case "ＨＰ回復"
									hp_recovery = hp_recovery + 10 * .FeatureLevel(i)
								Case "ＥＮ回復"
									en_recovery = en_recovery + 10 * .FeatureLevel(i)
								Case "ＨＰ増加"
									hp_up = hp_up + 1000 * .FeatureLevel(i)
								Case "ＥＮ増加"
									en_up = en_up + 10 * .FeatureLevel(i)
							End Select
						End If
						
						If Not is_immune_to_terrain_effect Then
							Select Case .Feature(i)
								Case "ＨＰ減少"
									If Weakness(.FeatureData(i)) Then
										hp_recovery = hp_recovery - 20 * .FeatureLevel(i)
									ElseIf Effective(.FeatureData(i)) Then 
										hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
									ElseIf Absorb(.FeatureData(i)) Then 
										hp_recovery = hp_recovery + 10 * .FeatureLevel(i)
									ElseIf Resist(.FeatureData(i)) Then 
										hp_recovery = hp_recovery - 5 * .FeatureLevel(i)
									ElseIf Not Immune(.FeatureData(i)) Then 
										hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
									End If
									
								Case "ＥＮ減少"
									If Weakness(.FeatureData(i)) Then
										en_recovery = en_recovery - 20 * .FeatureLevel(i)
									ElseIf Effective(.FeatureData(i)) Then 
										en_recovery = en_recovery - 10 * .FeatureLevel(i)
									ElseIf Absorb(.FeatureData(i)) Then 
										en_recovery = en_recovery + 10 * .FeatureLevel(i)
									ElseIf Resist(.FeatureData(i)) Then 
										en_recovery = en_recovery - 5 * .FeatureLevel(i)
									ElseIf Not Immune(.FeatureData(i)) Then 
										en_recovery = en_recovery - 10 * .FeatureLevel(i)
									End If
									
								Case "ＨＰ低下"
									If Weakness(.FeatureData(i)) Then
										hp_up = hp_up - 2000 * .FeatureLevel(i)
									ElseIf Effective(.FeatureData(i)) Then 
										hp_up = hp_up - 1000 * .FeatureLevel(i)
									ElseIf Absorb(.FeatureData(i)) Then 
										hp_up = hp_up + 1000 * .FeatureLevel(i)
									ElseIf Resist(.FeatureData(i)) Then 
										hp_up = hp_up - 500 * .FeatureLevel(i)
									ElseIf Not Immune(.FeatureData(i)) Then 
										hp_up = hp_up - 1000 * .FeatureLevel(i)
									End If
									
								Case "ＥＮ低下"
									If Weakness(.FeatureData(i)) Then
										en_up = en_up - 20 * .FeatureLevel(i)
									ElseIf Effective(.FeatureData(i)) Then 
										en_up = en_up - 10 * .FeatureLevel(i)
									ElseIf Absorb(.FeatureData(i)) Then 
										en_up = en_up + 10 * .FeatureLevel(i)
									ElseIf Resist(.FeatureData(i)) Then 
										en_up = en_up - 5 * .FeatureLevel(i)
									ElseIf Not Immune(.FeatureData(i)) Then 
										en_up = en_up - 10 * .FeatureLevel(i)
									End If
									
								Case "状態付加"
									cname = .FeatureData(i)
									
									'状態が無効化されるかチェック
									Select Case cname
										Case "装甲劣化"
											If SpecialEffectImmune("劣") Then
												cname = ""
											End If
										Case "混乱"
											If SpecialEffectImmune("乱") Then
												cname = ""
											End If
										Case "恐怖"
											If SpecialEffectImmune("恐") Then
												cname = ""
											End If
										Case "踊り"
											If SpecialEffectImmune("踊") Then
												cname = ""
											End If
										Case "狂戦士"
											If SpecialEffectImmune("狂") Then
												cname = ""
											End If
										Case "ゾンビ"
											If SpecialEffectImmune("ゾ") Then
												cname = ""
											End If
										Case "回復不能"
											If SpecialEffectImmune("害") Then
												cname = ""
											End If
										Case "石化"
											If SpecialEffectImmune("石") Then
												cname = ""
											End If
										Case "凍結"
											If SpecialEffectImmune("凍") Then
												cname = ""
											End If
										Case "麻痺"
											If SpecialEffectImmune("痺") Then
												cname = ""
											End If
										Case "睡眠"
											If SpecialEffectImmune("眠") Then
												cname = ""
											End If
										Case "毒"
											If SpecialEffectImmune("毒") Then
												cname = ""
											End If
										Case "盲目"
											If SpecialEffectImmune("盲") Then
												cname = ""
											End If
										Case "沈黙"
											If SpecialEffectImmune("黙") Then
												cname = ""
											End If
											'属性使用不能状態
										Case "オーラ使用不能"
											If SpecialEffectImmune("剋オ") Then
												cname = ""
											End If
										Case "超能力使用不能"
											If SpecialEffectImmune("剋超") Then
												cname = ""
											End If
										Case "同調率使用不能"
											If SpecialEffectImmune("剋シ") Then
												cname = ""
											End If
										Case "超感覚使用不能"
											If SpecialEffectImmune("剋サ") Then
												cname = ""
											End If
										Case "知覚強化使用不能"
											If SpecialEffectImmune("剋サ") Then
												cname = ""
											End If
										Case "霊力使用不能"
											If SpecialEffectImmune("剋霊") Then
												cname = ""
											End If
										Case "術使用不能"
											If SpecialEffectImmune("剋術") Then
												cname = ""
											End If
										Case "技使用不能"
											If SpecialEffectImmune("剋技") Then
												cname = ""
											End If
										Case Else
											If Len(cname) > 6 Then
												If Right(cname, 6) = "属性弱点付加" Then
													If SpecialEffectImmune("弱" & Left(cname, Len(cname) - 6)) Or Absorb(Left(cname, Len(cname) - 6)) Or Immune(Left(cname, Len(cname) - 6)) Then
														cname = ""
													End If
												ElseIf Right(cname, 6) = "属性有効付加" Then 
													If SpecialEffectImmune("有" & Left(cname, Len(cname) - 6)) Or Absorb(Left(cname, Len(cname) - 6)) Or Immune(Left(cname, Len(cname) - 6)) Then
														cname = ""
													End If
												ElseIf Right(cname, 6) = "属性使用不能" Then 
													If SpecialEffectImmune("剋" & Left(cname, Len(cname) - 6)) Then
														cname = ""
													End If
												End If
											End If
									End Select
									
									If cname <> "" Then
										If .IsFeatureLevelSpecified(i) Then
											AddCondition(cname, .FeatureLevel(i))
										Else
											AddCondition(cname, 10000)
										End If
									End If
							End Select
						End If
					Next 
				End If
			End With
			' MOD END MARGE
		End If
		
		'ＥＮは毎ターン5回復
		' MOD START MARGE
		'    If Not IsConditionSatisfied("回復不能") _
		''        And Not IsOptionDefined("ＥＮ自然回復無効") _
		''    Then
		If Not IsConditionSatisfied("回復不能") And Not IsSpecialPowerInEffect("回復不能") And Not IsOptionDefined("ＥＮ自然回復無効") Then
			' MOD END MARGE
			EN = EN + 5
		End If
		
		'算出した回復率を使ってＨＰを回復
		HP = HP + MaxHP * hp_recovery \ 100 + hp_up
		If HP <= 0 Then
			HP = 1
		End If
		
		'特殊能力「不安定」による暴走チェック
		If IsFeatureAvailable("不安定") Then
			If HP <= MaxHP \ 4 And Not IsConditionSatisfied("暴走") Then
				AddCondition("暴走", -1)
			End If
		End If
		
		'算出した回復率を使ってＥＮを回復
		If EN + MaxEN * en_recovery \ 100 + en_up > 0 Then
			EN = EN + MaxEN * en_recovery \ 100 + en_up
		Else
			'ＥＮが減少して０になる場合はハイパーモード解除もしくは行動不能
			If IsFeatureAvailable("ノーマルモード") Then
				'ただしノーマルモードに戻れない地形だとそのまま退却……
				If OtherForm(LIndex(FeatureData("ノーマルモード"), 1)).IsAbleToEnter(x, y) Then
					Transform(LIndex(FeatureData("ノーマルモード"), 1))
				Else
					Center(x, y)
					Escape()
					OpenMessageForm()
					DisplaySysMessage(Nickname & "は強制的に退却させられた。")
					CloseMessageForm()
					HandleEvent("破壊", MainPilot.ID)
					Exit Sub
				End If
			ElseIf IsFeatureAvailable("変形") Then 
				'変形できれば変形
				buf = FeatureData("変形")
				For i = 2 To LLength(buf)
					With OtherForm(LIndex(buf, i))
						If .IsAbleToEnter(x, y) And Not .IsFeatureAvailable("ＥＮ消費") Then
							Transform(LIndex(buf, i))
							Exit For
						End If
					End With
				Next 
				If i > LLength(buf) Then
					EN = 0
				End If
			Else
				EN = 0
			End If
		End If
		
		'データ更新
		Update()
		
		'時間切れ？
		If is_time_limit Then
			If next_form <> "" Then
				'ハイパーモード＆変身の時間切れ
				u = OtherForm(next_form)
				If u.IsAbleToEnter(x, y) Then
					'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
					If u.IsFeatureAvailable("追加パイロット") Then
						If Not PList.IsDefined(u.FeatureData("追加パイロット")) Then
							PList.Add(u.FeatureData("追加パイロット"), MainPilot.Level, Party0)
						End If
					End If
					
					'ノーマルモードメッセージ
					If IsMessageDefined("ノーマルモード(" & Name & "=>" & u.Name & ")") Then
						OpenMessageForm()
						PilotMessage("ノーマルモード(" & Name & "=>" & u.Name & ")")
						CloseMessageForm()
					ElseIf IsMessageDefined("ノーマルモード(" & u.Name & ")") Then 
						OpenMessageForm()
						PilotMessage("ノーマルモード(" & u.Name & ")")
						CloseMessageForm()
					ElseIf IsMessageDefined("ノーマルモード") Then 
						OpenMessageForm()
						PilotMessage("ノーマルモード")
						CloseMessageForm()
					End If
					'特殊効果
					If IsSpecialEffectDefined("ノーマルモード(" & Name & "=>" & u.Name & ")") Then
						SpecialEffect("ノーマルモード(" & Name & "=>" & u.Name & ")")
					ElseIf IsSpecialEffectDefined("ノーマルモード(" & u.Name & ")") Then 
						SpecialEffect("ノーマルモード(" & u.Name & ")")
					ElseIf IsSpecialEffectDefined("ノーマルモード") Then 
						SpecialEffect("ノーマルモード")
					End If
					
					'変形
					Transform(LIndex(FeatureData("ノーマルモード"), 1))
				Else
					'変形するとその地形にいれなくなる場合は退却
					Center(x, y)
					Escape()
					OpenMessageForm()
					DisplaySysMessage(Nickname & "は強制的に退却させられた。")
					CloseMessageForm()
					HandleEvent("破壊", MainPilot.ID)
					Exit Sub
				End If
			ElseIf IsFeatureAvailable("分離") Then 
				'合体時間切れ
				
				'メッセージ表示
				If IsMessageDefined("分離(" & Name & ")") Or IsMessageDefined("分離(" & FeatureName("分離") & ")") Or IsMessageDefined("分離") Then
					If IsFeatureAvailable("分離ＢＧＭ") Then
						StartBGM(FeatureData("分離ＢＧＭ"))
						Sleep(500)
					ElseIf MainPilot.BGM <> "-" Then 
						StartBGM(MainPilot.BGM)
						Sleep(500)
					End If
					Center(x, y)
					RefreshScreen()
					
					OpenMessageForm()
					If IsMessageDefined("分離(" & Name & ")") Then
						PilotMessage("分離(" & Name & ")")
					ElseIf IsMessageDefined("分離(" & FeatureName("分離") & ")") Then 
						PilotMessage("分離(" & FeatureName("分離") & ")")
					Else
						PilotMessage("分離")
					End If
					CloseMessageForm()
				End If
				'特殊効果
				If IsSpecialEffectDefined("分離(" & Name & ")") Then
					SpecialEffect("分離(" & Name & ")")
				ElseIf IsSpecialEffectDefined("分離(" & FeatureName("分離") & ")") Then 
					SpecialEffect("分離(" & FeatureName("分離") & ")")
				Else
					SpecialEffect("分離")
				End If
				
				'分離
				Split_Renamed()
			Else
				'制限時間切れ
				Center(x, y)
				RefreshScreen()
				OpenMessageForm()
				DisplaySysMessage(Nickname & "は制限時間切れのため退却します。")
				CloseMessageForm()
				Escape()
				HandleEvent("破壊", MainPilot.ID)
				Exit Sub
			End If
		End If
		
		'ハイパーモード＆ノーマルモードの自動発動をチェック
		CurrentForm.CheckAutoHyperMode()
		CurrentForm.CheckAutoNormalMode()
	End Sub
	
	'ハイパーモードの自動発動チェック
	Public Sub CheckAutoHyperMode()
		Dim is_available, message_window_visible As Boolean
		Dim fname, fdata As String
		Dim flevel As Double
		Dim uname As String
		Dim i As Short
		
		'ハイパーモードが自動発動するか判定
		
		If Status_Renamed <> "出撃" Then
			Exit Sub
		End If
		
		If Not IsFeatureAvailable("ハイパーモード") Then
			Exit Sub
		End If
		
		fname = FeatureName("ハイパーモード")
		flevel = FeatureLevel("ハイパーモード")
		fdata = FeatureData("ハイパーモード")
		
		If InStr(fdata, "自動発動") = 0 Then
			Exit Sub
		End If
		
		'発動条件を満たす？
		If MainPilot.Morale < CShort(10# * flevel) + 100 And (HP > MaxHP \ 4 Or InStr(fdata, "気力発動") > 0) Then
			Exit Sub
		End If
		
		'変身中・能力コピー中はハイパーモードを使用できない
		If IsConditionSatisfied("ノーマルモード付加") Then
			Exit Sub
		End If
		
		'ハイパーモード先の形態が利用可能？
		uname = LIndex(fdata, 2)
		is_available = False
		With OtherForm(uname)
			Select Case TerrainClass(x, y)
				Case "空"
					If .IsTransAvailable("空") Then
						is_available = True
					End If
				Case "深水"
					If .IsTransAvailable("空") Or .IsTransAvailable("水") Or .IsTransAvailable("水上") Then
						is_available = True
					End If
				Case Else
					is_available = True
			End Select
			
			If Not .IsAbleToEnter(x, y) Then
				is_available = False
			End If
		End With
		
		'自動発動する？
		If Not is_available Then
			Exit Sub
		End If
		
		'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
		If UDList.IsDefined(uname) Then
			With UDList.Item(uname)
				If IsFeatureAvailable("追加パイロット") Then
					If Not PList.IsDefined(FeatureData("追加パイロット")) Then
						PList.Add(FeatureData("追加パイロット"), MainPilot.Level, Party0)
					End If
				End If
			End With
		End If
		
		'ＢＧＭを切り替え
		If IsFeatureAvailable("ハイパーモードＢＧＭ") Then
			For i = 1 To CountFeature
				If Feature(i) = "ハイパーモードＢＧＭ" And LIndex(FeatureData(i), 1) = uname Then
					StartBGM(Mid(FeatureData(i), InStr(FeatureData(i), " ") + 1))
					Sleep(500)
					Exit For
				End If
			Next 
		End If
		
		'メッセージを表示
		If IsMessageDefined("ハイパーモード(" & Name & "=>" & uname & ")") Or IsMessageDefined("ハイパーモード(" & uname & ")") Or IsMessageDefined("ハイパーモード(" & fname & ")") Or IsMessageDefined("ハイパーモード") Then
			Center(x, y)
			RefreshScreen()
			
			If Not message_window_visible Then
				OpenMessageForm()
			Else
				message_window_visible = True
			End If
			
			'メッセージを表示
			If IsMessageDefined("ハイパーモード(" & Name & "=>" & uname & ")") Then
				PilotMessage("ハイパーモード(" & Name & "=>" & uname & ")")
			ElseIf IsMessageDefined("ハイパーモード(" & uname & ")") Then 
				PilotMessage("ハイパーモード(" & uname & ")")
			ElseIf IsMessageDefined("ハイパーモード(" & fname & ")") Then 
				PilotMessage("ハイパーモード(" & fname & ")")
			Else
				PilotMessage("ハイパーモード")
			End If
			
			If Not message_window_visible Then
				CloseMessageForm()
			End If
		End If
		
		'特殊効果
		SaveSelections()
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTargetForEvent = Nothing
		If IsAnimationDefined("ハイパーモード(" & Name & "=>" & uname & ")") Then
			PlayAnimation("ハイパーモード(" & Name & "=>" & uname & ")")
		ElseIf IsAnimationDefined("ハイパーモード(" & uname & ")") Then 
			PlayAnimation("ハイパーモード(" & uname & ")")
		ElseIf IsAnimationDefined("ハイパーモード(" & FeatureName("ハイパーモード") & ")") Then 
			PlayAnimation("ハイパーモード(" & FeatureName("ハイパーモード") & ")")
		ElseIf IsAnimationDefined("ハイパーモード") Then 
			PlayAnimation("ハイパーモード")
		ElseIf IsSpecialEffectDefined("ハイパーモード(" & Name & "=>" & uname & ")") Then 
			SpecialEffect("ハイパーモード(" & Name & "=>" & uname & ")")
		ElseIf IsSpecialEffectDefined("ハイパーモード(" & uname & ")") Then 
			SpecialEffect("ハイパーモード(" & uname & ")")
		ElseIf IsSpecialEffectDefined("ハイパーモード(" & fname & ")") Then 
			SpecialEffect("ハイパーモード(" & fname & ")")
		Else
			SpecialEffect("ハイパーモード")
		End If
		RestoreSelections()
		
		'ハイパーモードに変形
		Transform(uname)
		
		'ユニット変数を置き換え
		If Not SelectedUnit Is Nothing Then
			If ID = SelectedUnit.ID Then
				SelectedUnit = CurrentForm
			End If
		End If
		If Not SelectedUnitForEvent Is Nothing Then
			If ID = SelectedUnitForEvent.ID Then
				SelectedUnitForEvent = CurrentForm
			End If
		End If
		If Not SelectedTarget Is Nothing Then
			If ID = SelectedTarget.ID Then
				SelectedTarget = CurrentForm
			End If
		End If
		If Not SelectedTargetForEvent Is Nothing Then
			If ID = SelectedTargetForEvent.ID Then
				SelectedTargetForEvent = CurrentForm
			End If
		End If
		
		'変形イベント
		With CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
	End Sub
	
	'ノーマルモードの自動発動チェック
	Public Function CheckAutoNormalMode(Optional ByVal without_redraw As Boolean = False) As Boolean
		Dim message_window_visible As Boolean
		Dim uname As String
		Dim i As Short
		
		'ノーマルモードが自動発動するか判定
		
		If Status_Renamed <> "出撃" Then
			Exit Function
		End If
		
		If Not IsFeatureAvailable("ノーマルモード") Then
			Exit Function
		End If
		
		'まだ元の形態でもＯＫ？
		If IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'ノーマルモード先が利用可能？
		uname = LIndex(FeatureData("ノーマルモード"), 1)
		If Not OtherForm(uname).IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
		If UDList.IsDefined(uname) Then
			With UDList.Item(uname)
				If IsFeatureAvailable("追加パイロット") Then
					If Not PList.IsDefined(FeatureData("追加パイロット")) Then
						PList.Add(FeatureData("追加パイロット"), MainPilot.Level, Party0)
					End If
				End If
			End With
		End If
		
		'メッセージを表示
		If IsMessageDefined("ノーマルモード(" & Name & "=>" & uname & ")") Or IsMessageDefined("ノーマルモード(" & uname & ")") Or IsMessageDefined("ノーマルモード") Then
			'ＢＧＭを切り替え
			If IsFeatureAvailable("ノーマルモードＢＧＭ") Then
				For i = 1 To CountFeature
					If Feature(i) = "ノーマルモードＢＧＭ" And LIndex(FeatureData(i), 1) = uname Then
						StartBGM(Mid(FeatureData(i), InStr(FeatureData(i), " ") + 1))
						Sleep(500)
						Exit For
					End If
				Next 
			End If
			
			Center(x, y)
			RefreshScreen()
			
			If Not message_window_visible Then
				OpenMessageForm()
			Else
				message_window_visible = True
			End If
			
			'メッセージを表示
			If IsMessageDefined("ノーマルモード(" & Name & "=>" & uname & ")") Then
				PilotMessage("ノーマルモード(" & Name & "=>" & uname & ")")
			ElseIf IsMessageDefined("ノーマルモード(" & uname & ")") Then 
				PilotMessage("ノーマルモード(" & uname & ")")
			Else
				PilotMessage("ノーマルモード")
			End If
			
			If Not message_window_visible Then
				CloseMessageForm()
			End If
		End If
		
		'特殊効果
		SaveSelections()
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTargetForEvent = Nothing
		If IsAnimationDefined("ノーマルモード(" & Name & "=>" & uname & ")") Then
			PlayAnimation("ノーマルモード(" & Name & "=>" & uname & ")")
		ElseIf IsAnimationDefined("ノーマルモード(" & uname & ")") Then 
			PlayAnimation("ノーマルモード(" & uname & ")")
		ElseIf IsAnimationDefined("ノーマルモード") Then 
			PlayAnimation("ノーマルモード")
		ElseIf IsSpecialEffectDefined("ノーマルモード(" & Name & "=>" & uname & ")") Then 
			SpecialEffect("ノーマルモード(" & Name & "=>" & uname & ")")
		ElseIf IsSpecialEffectDefined("ノーマルモード(" & uname & ")") Then 
			SpecialEffect("ノーマルモード(" & uname & ")")
		Else
			SpecialEffect("ノーマルモード")
		End If
		RestoreSelections()
		
		'ノーマルモードに変形
		Transform(uname)
		
		'ユニット変数を置き換え
		If Not SelectedUnit Is Nothing Then
			If ID = SelectedUnit.ID Then
				SelectedUnit = CurrentForm
			End If
		End If
		If Not SelectedUnitForEvent Is Nothing Then
			If ID = SelectedUnitForEvent.ID Then
				SelectedUnitForEvent = CurrentForm
			End If
		End If
		If Not SelectedTarget Is Nothing Then
			If ID = SelectedTarget.ID Then
				SelectedTarget = CurrentForm
			End If
		End If
		If Not SelectedTargetForEvent Is Nothing Then
			If ID = SelectedTargetForEvent.ID Then
				SelectedTargetForEvent = CurrentForm
			End If
		End If
		
		'画面の再描画が必要？
		If CurrentForm.IsConditionSatisfied("消耗") Then
			CheckAutoNormalMode = True
			If Not without_redraw Then
				RedrawScreen()
			End If
		End If
		
		'変形イベント
		With CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
	End Function
	
	
	'データをリセット
	'UPGRADE_NOTE: Reset は Reset_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Sub Reset_Renamed()
		Dim i As Short
		Dim pname As String
		
		For i = 1 To CountCondition
			DeleteCondition0(1)
		Next 
		RemoveAllSpecialPowerInEffect()
		
		Update()
		
		For i = 1 To CountPilot
			Pilot(i).FullRecover()
		Next 
		For i = 1 To CountSupport
			Support(i).FullRecover()
		Next 
		If IsFeatureAvailable("追加パイロット") Then
			pname = FeatureData("追加パイロット")
			If PList.IsDefined(pname) Then
				PList.Item(pname).FullRecover()
			End If
		End If
		If IsFeatureAvailable("追加サポート") Then
			pname = FeatureData("追加サポート")
			If PList.IsDefined(pname) Then
				PList.Item(pname).FullRecover()
			End If
		End If
		
		HP = MaxHP
		FullSupply()
		Mode = "通常"
	End Sub
	
	'相手ユニットが敵かどうかを判定
	Public Function IsEnemy(ByRef t As Unit, Optional ByVal for_move As Boolean = False) As Boolean
		Dim myparty, tparty As String
		
		'自分自身は常に味方
		If t Is Me Then
			IsEnemy = False
			Exit Function
		End If
		
		'暴走したユニットにとってはすべてが敵
		If IsConditionSatisfied("暴走") Then
			IsEnemy = True
			Exit Function
		End If
		
		'混乱した場合はランダムで判定
		If IsConditionSatisfied("混乱") Then
			If for_move Then
				IsEnemy = True
			ElseIf Dice(2) = 1 Then 
				IsEnemy = True
			Else
				IsEnemy = False
			End If
			Exit Function
		End If
		
		With t
			myparty = Party
			tparty = .Party
			
			'味方ユニットは暴走、憑依、魅了したユニットを排除可能
			'(暴走した味方ユニットのPartyはＮＰＣとみなされる)
			If myparty = "味方" And tparty = "ＮＰＣ" Then
				If .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("憑依") Or .IsConditionSatisfied("魅了") Then
					IsEnemy = True
					Exit Function
				End If
			End If
			
			If myparty <> "味方" Then
				'ターゲットの陣営が限定されている場合、敵対関係にない陣営の
				'ユニットは味方と見なされる。
				'ただし、プレイヤーがコントロールするユニットはこのような自
				'分を攻撃してこないユニットも排除可能。
				
				'特定の陣営のみを狙う場合
				Select Case Mode
					Case "味方", "ＮＰＣ"
						Select Case tparty
							Case "味方", "ＮＰＣ"
								IsEnemy = True
							Case Else
								IsEnemy = False
						End Select
						Exit Function
					Case "敵", "中立"
						If tparty = Mode Then
							IsEnemy = True
						Else
							IsEnemy = False
						End If
						Exit Function
				End Select
				
				'相手が特定の陣営のみを狙う場合
				Select Case .Mode
					Case "味方", "ＮＰＣ"
						Select Case myparty
							Case "味方", "ＮＰＣ"
								IsEnemy = True
							Case Else
								IsEnemy = False
						End Select
						Exit Function
					Case "敵", "中立"
						If myparty = .Mode Then
							IsEnemy = True
						Else
							IsEnemy = False
						End If
						Exit Function
				End Select
			End If
			
			'敵味方を判定
			Select Case myparty
				Case "味方", "ＮＰＣ"
					Select Case tparty
						Case "味方", "ＮＰＣ"
							IsEnemy = False
						Case Else
							IsEnemy = True
					End Select
				Case Else
					If myparty = tparty Then
						IsEnemy = False
					Else
						IsEnemy = True
					End If
			End Select
		End With
	End Function
	
	'相手ユニットが味方かどうかを判定
	Public Function IsAlly(ByRef t As Unit) As Boolean
		'自分自身は常に味方
		If t Is Me Then
			IsAlly = True
			Exit Function
		End If
		
		'暴走したユニットにとってはすべてが敵
		If IsConditionSatisfied("暴走") Then
			IsAlly = False
			Exit Function
		End If
		
		'混乱した場合はランダムで判定
		If IsConditionSatisfied("混乱") Then
			If Dice(2) = 1 Then
				IsAlly = True
			Else
				IsAlly = False
			End If
			Exit Function
		End If
		
		'敵味方を判定
		Select Case Party
			Case "味方", "ＮＰＣ"
				If t.Party = "味方" Or t.Party = "ＮＰＣ" Then
					IsAlly = True
				Else
					IsAlly = False
				End If
			Case Else
				If Party = t.Party Then
					IsAlly = True
				Else
					IsAlly = False
				End If
		End Select
	End Function
	
	
	' === ユニット搭載関連処理 ===
	
	'ユニットを搭載
	Public Sub LoadUnit(ByRef u As Unit)
		colUnitOnBoard.Add(u, u.ID)
	End Sub
	
	'搭載したユニットを削除
	Public Sub UnloadUnit(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colUnitOnBoard.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colUnitOnBoard.Count()
			'UPGRADE_WARNING: オブジェクト colUnitOnBoard(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If colUnitOnBoard.Item(i).Name = Index Then
				colUnitOnBoard.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'搭載したユニットの総数
	Public Function CountUnitOnBoard() As Short
		CountUnitOnBoard = colUnitOnBoard.Count()
	End Function
	
	'搭載したユニット
	Public Function UnitOnBoard(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		UnitOnBoard = colUnitOnBoard.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colUnitOnBoard
			'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If u.Name = Index Then
				UnitOnBoard = u
				Exit Function
			End If
		Next u
	End Function
	
	
	' === 召喚ユニット関連処理 ===
	
	'召喚ユニットを追加
	Public Sub AddServant(ByRef u As Unit)
		'既に登録している？
		If Not Servant((u.ID)) Is Nothing Then
			Exit Sub
		End If
		
		colServant.Add(u, u.ID)
	End Sub
	
	'召喚ユニットを削除
	Public Sub DeleteServant(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colServant.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colServant.Count()
			'UPGRADE_WARNING: オブジェクト colServant(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If colServant.Item(i).Name = Index Then
				colServant.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'召喚ユニット総数
	Public Function CountServant() As Short
		CountServant = colServant.Count()
	End Function
	
	'召喚ユニット
	Public Function Servant(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		Servant = colServant.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colServant
			'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If u.Name = Index Then
				Servant = u
				Exit Function
			End If
		Next u
	End Function
	
	'召喚ユニットを解放する
	Public Sub DismissServant()
		Dim i, j As Short
		Dim uname As String
		
		For i = 1 To CountServant
			With Servant(1).CurrentForm
				Select Case .Status_Renamed
					Case "出撃", "格納"
						.Escape()
						.Status_Renamed = "破棄"
					Case "旧主形態", "旧形態"
						For j = 1 To .CountFeature
							If .Feature(j) = "合体" Then
								uname = LIndex(.FeatureData(j), 2)
								If UList.IsDefined(uname) Then
									UList.Item(uname).CurrentForm.Split_Renamed()
								End If
							End If
						Next 
						With .CurrentForm
							If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
								.Escape()
								.Status_Renamed = "破棄"
							End If
						End With
				End Select
			End With
			DeleteServant(1)
		Next 
	End Sub
	
	
	' === 隷属ユニット関連処理 ===
	
	'隷属ユニットを追加
	Public Sub AddSlave(ByRef u As Unit)
		colSlave.Add(u, u.ID)
	End Sub
	
	'隷属ユニットを削除
	Public Sub DeleteSlave(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colSlave.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colSlave.Count()
			'UPGRADE_WARNING: オブジェクト colSlave(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If colSlave.Item(i).Name = Index Then
				colSlave.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'隷属ユニット総数
	Public Function CountSlave() As Short
		CountSlave = colSlave.Count()
	End Function
	
	'隷属ユニット
	Public Function Slave(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		Slave = colSlave.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colSlave
			'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If u.Name = Index Then
				Slave = u
				Exit Function
			End If
		Next u
	End Function
	
	'隷属ユニットを解放する
	Public Sub DismissSlave()
		Dim i As Short
		
		For i = 1 To CountSlave
			With Slave(1).CurrentForm
				If .IsConditionSatisfied("魅了") And Not .Master Is Nothing Then
					If .Master.CurrentForm Is Me Then
						.DeleteCondition("魅了")
						'UPGRADE_NOTE: オブジェクト Slave().CurrentForm.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						.Master = Nothing
					End If
				End If
				
				If .IsConditionSatisfied("憑依") And Not .Master Is Nothing Then
					If .Master.CurrentForm Is Me Then
						.DeleteCondition("憑依")
						'UPGRADE_NOTE: オブジェクト Slave().CurrentForm.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						.Master = Nothing
					End If
				End If
			End With
			DeleteSlave(1)
		Next 
	End Sub
	
	' === 一時中断用データ関連処理 ===
	
	'一時中断用データをファイルにセーブする
	Public Sub Dump()
		Dim cnd As Condition
		Dim i As Short
		
		WriteLine(SaveDataFileNumber, Name, ID, Party0)
		WriteLine(SaveDataFileNumber, Rank, BossRank, x, y)
		WriteLine(SaveDataFileNumber, Area, UsedAction, Mode, Status_Renamed)
		
		If Not Master Is Nothing Then
			WriteLine(SaveDataFileNumber, Master.ID)
		Else
			WriteLine(SaveDataFileNumber, "-")
		End If
		If Not Summoner Is Nothing Then
			WriteLine(SaveDataFileNumber, Summoner.ID)
		Else
			WriteLine(SaveDataFileNumber, "-")
		End If
		
		WriteLine(SaveDataFileNumber, UsedSupportAttack, UsedSupportGuard)
		WriteLine(SaveDataFileNumber, UsedSyncAttack, UsedCounterAttack)
		
		WriteLine(SaveDataFileNumber, CountOtherForm)
		For i = 1 To CountOtherForm
			WriteLine(SaveDataFileNumber, OtherForm(i).ID)
		Next 
		
		WriteLine(SaveDataFileNumber, CountUnitOnBoard)
		For i = 1 To CountUnitOnBoard
			WriteLine(SaveDataFileNumber, UnitOnBoard(i).ID)
		Next 
		
		WriteLine(SaveDataFileNumber, CountPilot)
		For i = 1 To CountPilot
			WriteLine(SaveDataFileNumber, Pilot(i).ID)
		Next 
		
		WriteLine(SaveDataFileNumber, CountSupport)
		For i = 1 To CountSupport
			WriteLine(SaveDataFileNumber, Support(i).ID)
		Next 
		
		If AdditionalSupport Is Nothing Then
			WriteLine(SaveDataFileNumber, "")
		Else
			WriteLine(SaveDataFileNumber, AdditionalSupport.ID)
		End If
		
		WriteLine(SaveDataFileNumber, CountServant)
		For i = 1 To CountServant
			WriteLine(SaveDataFileNumber, Servant(i).ID)
		Next 
		
		WriteLine(SaveDataFileNumber, CountItem)
		For i = 1 To CountItem
			WriteLine(SaveDataFileNumber, Item(i).ID)
		Next 
		
		WriteLine(SaveDataFileNumber, colSpecialPowerInEffect.Count())
		For	Each cnd In colSpecialPowerInEffect
			With cnd
				WriteLine(SaveDataFileNumber, .Name, .StrData)
			End With
		Next cnd
		
		WriteLine(SaveDataFileNumber, colCondition.Count())
		For	Each cnd In colCondition
			With cnd
				WriteLine(SaveDataFileNumber, .Name, .Lifetime, .Level, .StrData)
			End With
		Next cnd
		
		WriteLine(SaveDataFileNumber, CountWeapon)
		For i = 1 To CountWeapon
			WriteLine(SaveDataFileNumber, Bullet(i))
		Next 
		
		WriteLine(SaveDataFileNumber, CountAbility)
		For i = 1 To CountAbility
			WriteLine(SaveDataFileNumber, Stock(i))
		Next 
		
		WriteLine(SaveDataFileNumber, HP, EN)
	End Sub
	
	'一時中断用データをファイルからロードする
	Public Sub Restore()
		Dim sbuf As String
		Dim ibuf As Short
		Dim lbuf As Integer
		'UPGRADE_NOTE: ctype は ctype_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim ctype_Renamed, cdata As String
		Dim cltime As Short
		Dim clevel As Double
		Dim num, i, ret As Short
		
		'Name, ID, Party
		Input(SaveDataFileNumber, sbuf)
		strName = sbuf
		Data = UDList.Item(sbuf)
		Input(SaveDataFileNumber, sbuf)
		ID = sbuf
		Input(SaveDataFileNumber, sbuf)
		strParty = sbuf
		
		'Rank, BossRank, X, Y
		Input(SaveDataFileNumber, ibuf)
		intRank = ibuf
		Input(SaveDataFileNumber, ibuf)
		intBossRank = ibuf
		Input(SaveDataFileNumber, ibuf)
		x = ibuf
		Input(SaveDataFileNumber, ibuf)
		y = ibuf
		
		'Area, Action, Mode, Status
		Input(SaveDataFileNumber, sbuf)
		Area = sbuf
		Input(SaveDataFileNumber, ibuf)
		UsedAction = ibuf
		Input(SaveDataFileNumber, sbuf)
		strMode = sbuf
		Input(SaveDataFileNumber, sbuf)
		Status_Renamed = sbuf
		
		'Master, Summoner
		Input(SaveDataFileNumber, sbuf)
		Input(SaveDataFileNumber, sbuf)
		
		'UsedSupportAttack, UsedSupportGuard
		Input(SaveDataFileNumber, ibuf)
		UsedSupportAttack = ibuf
		Input(SaveDataFileNumber, ibuf)
		UsedSupportGuard = ibuf
		
		'UsedSyncAttack, UsedCounterAttack
		Input(SaveDataFileNumber, ibuf)
		UsedSyncAttack = ibuf
		Input(SaveDataFileNumber, ibuf)
		UsedCounterAttack = ibuf
		
		'OtherForm
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'UnitOnBoard
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Pilot
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			AddPilot(PList.Item(sbuf))
		Next 
		
		'Support
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			AddSupport(PList.Item(sbuf))
		Next 
		
		'AdditionalSupport
		If SaveDataVersion >= 20210 Then
			Input(SaveDataFileNumber, sbuf)
			If sbuf <> "" Then
				pltAdditionalSupport = PList.Item(sbuf)
				pltAdditionalSupport.Unit_Renamed = Me
				pltAdditionalSupport.IsAdditionalSupport = True
			End If
		End If
		
		'Servant
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Item
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'SpecialPowerInEffect
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, ctype_Renamed)
			Input(SaveDataFileNumber, cdata)
			MakeSpecialPowerInEffect(ctype_Renamed, cdata)
		Next 
		
		'Condition
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
			
			'付加した特殊能力のデータに「"」や「,」が含まれているとデータの処理が
			'正しく行えないので手動でパーシング
			
			ret = InStr(sbuf, ",")
			ctype_Renamed = Left(sbuf, ret - 1)
			If Left(ctype_Renamed, 1) = """" Then
				ctype_Renamed = Mid(ctype_Renamed, 2, Len(ctype_Renamed) - 2)
			End If
			sbuf = Mid(sbuf, ret + 1)
			
			ret = InStr(sbuf, ",")
			cltime = StrToLng(Left(sbuf, ret - 1))
			sbuf = Mid(sbuf, ret + 1)
			
			ret = InStr(sbuf, ",")
			clevel = StrToLng(Left(sbuf, ret - 1))
			sbuf = Mid(sbuf, ret + 1)
			
			cdata = sbuf
			If Left(cdata, 1) = """" Then
				cdata = Mid(cdata, 2, Len(cdata) - 2)
			End If
			
			If SaveDataVersion < 10741 Then
				If InStr(cdata, " パイロット能力付加 ") > 0 Then
					GoTo NextCondition
				End If
				If InStr(cdata, " パイロット能力強化 ") > 0 Then
					GoTo NextCondition
				End If
			End If
			
			AddCondition(ctype_Renamed, cltime, clevel, cdata)
NextCondition: 
		Next 
		
		'Weapon
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Ability
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'HP, EN
		Input(SaveDataFileNumber, lbuf)
		Input(SaveDataFileNumber, ibuf)
		HP = lbuf
		EN = ibuf
	End Sub
	
	'一時中断用データのリンク情報をファイルからロードする
	Public Sub RestoreLinkInfo()
		Dim sbuf As String
		Dim ibuf As Short
		Dim lbuf As Integer
		Dim dbuf As Double
		Dim i, num As Short
		Dim itm As Item
		
		'Name, ID, Party
		sbuf = LineInput(SaveDataFileNumber)
		
		'Rank, BossRank, X, Y
		sbuf = LineInput(SaveDataFileNumber)
		
		'Area, Action, Mode, Status
		sbuf = LineInput(SaveDataFileNumber)
		
		'Master, Summoner
		Input(SaveDataFileNumber, sbuf)
		Master = UList.Item(sbuf)
		Input(SaveDataFileNumber, sbuf)
		Summoner = UList.Item(sbuf)
		
		'SupportAttackStock, SupportGuardStock
		sbuf = LineInput(SaveDataFileNumber)
		'UsedSyncAttack, UsedCounterAttack
		sbuf = LineInput(SaveDataFileNumber)
		
		'OtherForm
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			AddOtherForm(UList.Item(sbuf))
		Next 
		
		'UnitOnBoard
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			LoadUnit(UList.Item(sbuf))
		Next 
		
		'Pilot
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Support
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'AdditionalSupport
		If SaveDataVersion >= 20210 Then
			sbuf = LineInput(SaveDataFileNumber)
		End If
		
		'Servant
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			AddServant(UList.Item(sbuf))
		Next 
		
		'Item
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, sbuf)
			AddItem0(IList.Item(sbuf))
		Next 
		
		'SpecialPowerInEffect
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Condition
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		Update(True)
		
		'Weapon
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, ibuf)
			If i <= CountWeapon() Then
				SetBullet(i, ibuf)
			End If
		Next 
		
		'Ability
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			Input(SaveDataFileNumber, ibuf)
			If i <= CountAbility() Then
				SetStock(i, ibuf)
			End If
		Next 
		
		'HP, EN
		Input(SaveDataFileNumber, lbuf)
		Input(SaveDataFileNumber, ibuf)
		HP = lbuf
		EN = ibuf
	End Sub
	
	'一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim sbuf As String
		Dim ibuf As Short
		Dim lbuf As Integer
		Dim dbuf As Double
		Dim i, num As Short
		
		'Name, ID, Party
		sbuf = LineInput(SaveDataFileNumber)
		
		'Rank, BossRank, X, Y
		sbuf = LineInput(SaveDataFileNumber)
		
		'Area, Action, Mode, Status
		sbuf = LineInput(SaveDataFileNumber)
		
		'Master
		sbuf = LineInput(SaveDataFileNumber)
		'Summoner
		sbuf = LineInput(SaveDataFileNumber)
		
		'SupportAttackStock, SupportGuardStock
		sbuf = LineInput(SaveDataFileNumber)
		'UsedSyncAttack, UsedCounterAttack
		sbuf = LineInput(SaveDataFileNumber)
		
		'OtherForm
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'UnitOnBoard
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Pilot
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Support
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'AdditionalSupport
		If SaveDataVersion >= 20210 Then
			sbuf = LineInput(SaveDataFileNumber)
		End If
		
		'Servant
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Item
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'SpecialPowerInEffect
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Condition
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Weapon
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'Ability
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			sbuf = LineInput(SaveDataFileNumber)
		Next 
		
		'HP, EN
		Input(SaveDataFileNumber, lbuf)
		Input(SaveDataFileNumber, ibuf)
		HP = lbuf
		EN = ibuf
		
		Update()
	End Sub
	
	
	' === 各種処理を行うための関数＆サブルーチン ===
	
	'出撃中？
	Public Function IsOperational() As Boolean
		Dim i As Short
		
		If Status_Renamed = "出撃" Then
			IsOperational = True
			Exit Function
		End If
		
		For i = 1 To CountOtherForm
			If OtherForm(i).Status_Renamed = "出撃" Then
				IsOperational = True
				Exit Function
			End If
		Next 
		
		IsOperational = False
	End Function
	
	'ユニットがユニット nm と同一？
	Public Function IsEqual(ByVal nm As String) As Boolean
		Dim i As Short
		
		If Name = nm Then
			IsEqual = True
			Exit Function
		End If
		
		For i = 1 To CountOtherForm
			If OtherForm(i).Name = nm Then
				IsEqual = True
				Exit Function
			End If
		Next 
		
		IsEqual = False
	End Function
	
	'ユニットが現在とっている形態
	Public Function CurrentForm() As Unit
		Dim i As Short
		
		If Status_Renamed = "他形態" Then
			For i = 1 To CountOtherForm
				If OtherForm(i).Status_Renamed <> "他形態" Then
					CurrentForm = OtherForm(i)
					Exit Function
				End If
			Next 
		End If
		
		CurrentForm = Me
	End Function
	
	'人間ユニットかどうか判定
	Public Function IsHero() As Boolean
		With Data
			If Left(.Class_Renamed, 1) = "(" Then
				IsHero = True
			Else
				IsHero = False
			End If
		End With
	End Function
	
	'(tx,ty)の地点の周囲に「連携攻撃」を行ってくれるユニットがいるかどうかを判定
	Public Function LookForAttackHelp(ByVal tx As Short, ByVal ty As Short) As Unit
		Dim u As Unit
		Dim i As Short
		
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			Select Case i
				Case 1
					If tx > 1 Then
						u = MapDataForUnit(tx - 1, ty)
					End If
				Case 2
					If tx < MapWidth Then
						u = MapDataForUnit(tx + 1, ty)
					End If
				Case 3
					If ty > 1 Then
						u = MapDataForUnit(tx, ty - 1)
					End If
				Case 4
					If ty < MapHeight Then
						u = MapDataForUnit(tx, ty + 1)
					End If
			End Select
			
			'ユニットがいる？
			If u Is Nothing Then
				GoTo NextLoop
			End If
			
			'ユニットが敵でない？
			If IsEnemy(u) Then
				GoTo NextLoop
			End If
			
			With u
				'信頼度を満たしている？
				If Dice(10) > .MainPilot.Relation(MainPilot) Then
					GoTo NextLoop
				End If
				
				'行動可能？
				If .MaxAction = 0 Then
					GoTo NextLoop
				End If
				
				'正常な判断力がある？
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("魅了") Or .IsConditionSatisfied("憑依") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Then
					GoTo NextLoop
				End If
				
				'メッセージが登録されている？
				If Not IsMessageDefined("連携攻撃(" & .MainPilot.Name & ")", True) And Not IsMessageDefined("連携攻撃(" & .MainPilot.Nickname & ")", True) Then
					GoTo NextLoop
				End If
			End With
			
			'見つかった
			LookForAttackHelp = u
			Exit Function
NextLoop: 
		Next 
		
		'見つからなかった
		'UPGRADE_NOTE: オブジェクト LookForAttackHelp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		LookForAttackHelp = Nothing
	End Function
	
	'tからの攻撃に対して「かばう」を行ってくれるユニットがいるかどうか判定
	Public Function LookForGuardHelp(ByRef t As Unit, ByVal tw As Short, ByVal is_critical As Boolean) As Unit
		Dim u As Unit
		Dim i As Short
		Dim dmg As Integer
		Dim ratio As Double
		Dim ux, uy As Short
		Dim uarea As String
		
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			Select Case i
				Case 1
					If x > 1 Then
						u = MapDataForUnit(x - 1, y)
					End If
				Case 2
					If x < MapWidth Then
						u = MapDataForUnit(x + 1, y)
					End If
				Case 3
					If y > 1 Then
						u = MapDataForUnit(x, y - 1)
					End If
				Case 4
					If y < MapHeight Then
						u = MapDataForUnit(x, y + 1)
					End If
			End Select
			
			'ユニットがいる？
			If u Is Nothing Then
				GoTo NextLoop
			End If
			
			'ユニットが敵でない？
			If IsEnemy(u) Then
				GoTo NextLoop
			End If
			
			With u
				'信頼度を満たしている？
				If Dice(10) > .MainPilot.Relation(MainPilot) Then
					GoTo NextLoop
				End If
				
				'行動可能？
				If .MaxAction = 0 Then
					GoTo NextLoop
				End If
				
				'正常な判断力がある？
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("魅了") Or .IsConditionSatisfied("憑依") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Then
					GoTo NextLoop
				End If
				
				'メッセージが登録されている？
				If Not .IsMessageDefined("かばう(" & MainPilot.Name & ")", True) And Not .IsMessageDefined("かばう(" & MainPilot.Nickname & ")", True) Then
					GoTo NextLoop
				End If
				
				'援護相手のユニットのいる地形に進入可能？
				If Area <> .Area Then
					Select Case Area
						Case "空中"
							If .Adaption(1) = 0 Then
								GoTo NextLoop
							End If
						Case "地上"
							If .Adaption(2) = 0 Then
								GoTo NextLoop
							End If
						Case "水中", "水上"
							If .Adaption(3) = 0 Then
								GoTo NextLoop
							End If
						Case "宇宙"
							If TerrainClass(x, y) = "月面" Then
								If Not .IsTransAvailable("空") And Not .IsTransAvailable("宇宙") Then
									GoTo NextLoop
								End If
							ElseIf .Adaption(4) = 0 Then 
								GoTo NextLoop
							End If
					End Select
				End If
				
				'ダメージを算出
				If .IsFeatureAvailable("防御不可") Or t.IsWeaponClassifiedAs(tw, "殺") Then
					ratio = 1#
				Else
					ratio = 0.5
				End If
				If is_critical Then
					If IsOptionDefined("ダメージ倍率低下") Then
						ratio = 1.2 * ratio
					Else
						ratio = 1.5 * ratio
					End If
				End If
				ux = .x
				uy = .y
				uarea = .Area
				.x = x
				.y = y
				.Area = Area
				dmg = t.ExpDamage(tw, u, True, ratio)
				.x = ux
				.y = uy
				.Area = uarea
				
				'自分が倒されてしまうような場合はかばわない
				If dmg >= .HP Then
					GoTo NextLoop
				End If
			End With
			
			'見つかった
			LookForGuardHelp = u
			Exit Function
NextLoop: 
		Next 
		
		'見つからなかった
		'UPGRADE_NOTE: オブジェクト LookForGuardHelp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		LookForGuardHelp = Nothing
	End Function
	
	'最大サポートアタック回数
	Public Function MaxSupportAttack() As Short
		With MainPilot
			MaxSupportAttack = MaxLng(.SkillLevel("援護攻撃"), .SkillLevel("援護"))
		End With
	End Function
	
	'最大サポートガード回数
	Public Function MaxSupportGuard() As Short
		With MainPilot
			MaxSupportGuard = MaxLng(.SkillLevel("援護防御"), .SkillLevel("援護"))
		End With
	End Function
	
	'最大同時援護攻撃回数
	Public Function MaxSyncAttack() As Short
		MaxSyncAttack = MainPilot.SkillLevel("統率")
	End Function
	
	'最大カウンター攻撃回数
	Public Function MaxCounterAttack() As Short
		With MainPilot
			MaxCounterAttack = .SkillLevel("カウンター")
			
			If .IsSkillAvailable("先手必勝") Then
				If LLength(.SkillData("先手必勝")) = 2 Then
					If .Morale >= StrToLng(LIndex(.SkillData("先手必勝"), 2)) Then
						MaxCounterAttack = 1000
					End If
				Else
					If .Morale >= 120 Then
						MaxCounterAttack = 1000
					End If
				End If
			End If
		End With
	End Function
	
	'ユニット t に対して周囲にサポートアタックを行ってくれるユニットがいるかどうかを判定
	Public Function LookForSupportAttack(ByRef t As Unit) As Unit
		Dim u As Unit
		Dim i, w As Short
		Dim max_wpower As Integer
		Dim team, uteam As String
		
		'正常な判断が可能？
		If IsConditionSatisfied("混乱") Then
			Exit Function
		End If
		
		'同士討ちの場合はどちらに荷担すべきか分からないので……
		If Not t Is Nothing Then
			If Party = t.Party Then
				Exit Function
			End If
		End If
		
		team = MainPilot.SkillData("チーム")
		
		max_wpower = -1
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			Select Case i
				Case 1
					If x > 1 Then
						u = MapDataForUnit(x - 1, y)
					End If
				Case 2
					If x < MapWidth Then
						u = MapDataForUnit(x + 1, y)
					End If
				Case 3
					If y > 1 Then
						u = MapDataForUnit(x, y - 1)
					End If
				Case 4
					If y < MapHeight Then
						u = MapDataForUnit(x, y + 1)
					End If
			End Select
			
			If u Is Nothing Then
				GoTo NextUnit
			End If
			
			With u
				'サポートアタック数が残っている？
				If .MaxSupportAttack <= .UsedSupportAttack Then
					GoTo NextUnit
				End If
				
				'行動数が残っている？
				If .Action = 0 Then
					GoTo NextUnit
				End If
				
				'正常な判断が可能？
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Or .IsConditionSatisfied("踊り") Then
					GoTo NextUnit
				End If
				
				'味方？
				Select Case .Party
					Case "ＮＰＣ"
						Select Case Party
							Case "敵", "中立"
								GoTo NextUnit
						End Select
					Case Else
						If .Party <> Party Then
							GoTo NextUnit
						End If
				End Select
				
				'同じチームに属している？
				uteam = .MainPilot.SkillData("チーム")
				If team <> uteam And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'まだターゲットが特定されていない？
				If t Is Nothing Then
					LookForSupportAttack = u
					Exit Function
				End If
				
				'攻撃可能？
				'高い威力の武器はリストの最後の方にあることが多いので後ろから判定
				w = .CountWeapon
				Do While w > 0
					'攻撃力が今まで見つかった武器以下の場合は選考外
					If .WeaponPower(w, (t.Area)) <= max_wpower Then
						GoTo NextWeapon
					End If
					
					'サポートアタックに利用可能？
					If .IsWeaponClassifiedAs(w, "Ｍ") Then
						GoTo NextWeapon
					End If
					If .IsWeaponClassifiedAs(w, "合") Then
						GoTo NextWeapon
					End If
					If Not .IsWeaponAvailable(w, "移動前") Then
						GoTo NextWeapon
					End If
					If Not .IsTargetWithinRange(w, t) Then
						GoTo NextWeapon
					End If
					
					If .Party = "味方" And .Party0 = "味方" Then
						'味方ユニットは自爆攻撃をサポートアタックには使用しない
						If .IsWeaponClassifiedAs(w, "自") Then
							GoTo NextWeapon
						End If
						
						'自動反撃の場合、味方ユニットは残弾数が少ない武器を使用しない
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
							If Not .IsWeaponClassifiedAs(w, "永") Then
								If .Bullet(w) = 1 Or .MaxBullet(w) = 2 Or .MaxBullet(w) = 3 Then
									GoTo NextWeapon
								End If
							End If
							If .WeaponENConsumption(w) > 0 Then
								If .WeaponENConsumption(w) >= .EN \ 2 Or .WeaponENConsumption(w) >= .MaxEN \ 4 Then
									GoTo NextWeapon
								End If
							End If
							If .IsWeaponClassifiedAs(w, "尽") Then
								GoTo NextWeapon
							End If
						End If
					End If
					
					'援護攻撃用の武器が見つかった
					max_wpower = .WeaponPower(w, (t.Area))
					LookForSupportAttack = u
					
NextWeapon: 
					w = w - 1
				Loop 
			End With
NextUnit: 
		Next 
	End Function
	
	'ユニット t からの攻撃に対して周囲にサポートガードを行ってくれるユニットが
	'いるかどうかを判定
	Public Function LookForSupportGuard(ByRef t As Unit, ByVal tw As Short) As Unit
		Dim u As Unit
		Dim i As Short
		Dim my_dmg, dmg As Integer
		Dim ratio As Double
		Dim ux, uy As Short
		Dim uarea As String
		Dim team, uteam As String
		
		'マップ攻撃はサポートガード不能
		If t.IsWeaponClassifiedAs(tw, "Ｍ") Then
			Exit Function
		End If
		
		'スペシャルパワーでサポートガードが無効化されている？
		If t.IsUnderSpecialPowerEffect("サポートガード無効化") Then
			Exit Function
		End If
		
		'同士討ちの場合は本来の陣営に属するユニットのみを守る
		If Party = t.Party Or (Party = "ＮＰＣ" And t.Party = "味方") Then
			If Party <> Party0 Then
				Exit Function
			End If
			If IsConditionSatisfied("暴走") Then
				Exit Function
			End If
		End If
		
		'自分が受けるダメージを求めておく
		my_dmg = t.ExpDamage(tw, Me, True)
		
		'かばう必要がない？
		'手動反撃で味方の場合はダメージにかかわらず常にかばう
		'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		If Party <> "味方" Or MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
			If t.IsNormalWeapon(tw) Then
				If my_dmg < MaxHP \ 20 And my_dmg < HP \ 5 Then
					Exit Function
				End If
			Else
				If t.CriticalProbability(tw, Me) > 0 Then
					Exit Function
				End If
			End If
		End If
		
		team = MainPilot.SkillData("チーム")
		
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			Select Case i
				Case 1
					If x > 1 Then
						u = MapDataForUnit(x - 1, y)
					End If
				Case 2
					If x < MapWidth Then
						u = MapDataForUnit(x + 1, y)
					End If
				Case 3
					If y > 1 Then
						u = MapDataForUnit(x, y - 1)
					End If
				Case 4
					If y < MapHeight Then
						u = MapDataForUnit(x, y + 1)
					End If
			End Select
			
			If u Is Nothing Then
				GoTo NextUnit
			End If
			If u Is t Then
				GoTo NextUnit
			End If
			
			With u
				'サポートガード数が残っている？
				If .MaxSupportGuard <= .UsedSupportGuard Then
					GoTo NextUnit
				End If
				
				'行動可能？
				If .MaxAction = 0 Then
					GoTo NextUnit
				End If
				
				'スペシャルパワーでサポートガードが封印されている？
				If .IsUnderSpecialPowerEffect("サポートガード不能") Then
					GoTo NextUnit
				End If
				
				'正常な判断が可能？
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Then
					GoTo NextUnit
				End If
				
				'ＨＰが高いほうを優先
				If Not LookForSupportGuard Is Nothing Then
					If LookForSupportGuard.HP >= .HP Then
						GoTo NextUnit
					End If
				End If
				
				'味方？
				Select Case .Party
					Case "味方"
						If IsOptionDefined("対ＮＰＣサポートガード無効") Then
							If .Party <> Party Then
								GoTo NextUnit
							End If
						Else
							Select Case Party
								Case "敵", "中立"
									GoTo NextUnit
							End Select
						End If
					Case "ＮＰＣ"
						Select Case Party
							Case "敵", "中立"
								GoTo NextUnit
						End Select
					Case Else
						If .Party <> Party Then
							GoTo NextUnit
						End If
				End Select
				
				'同じチームに属している？
				uteam = .MainPilot.SkillData("チーム")
				If team <> uteam And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'援護相手のユニットのいる地形に進入可能？
				If Area <> .Area Then
					Select Case Area
						Case "空中"
							If Not .IsTransAvailable("空") Then
								GoTo NextUnit
							End If
						Case "地上"
							If .Adaption(2) = 0 Then
								GoTo NextUnit
							End If
						Case "水中", "水上"
							If .Adaption(3) = 0 Then
								GoTo NextUnit
							End If
						Case "宇宙"
							If .Adaption(4) = 0 Then
								GoTo NextUnit
							End If
					End Select
				End If
				
				'機械をかばうのは機械のみ
				If MainPilot.Personality = "機械" Then
					If .MainPilot.Personality <> "機械" Then
						GoTo NextUnit
					End If
				End If
				
				'ダメージを算出
				If .IsFeatureAvailable("防御不可") Or t.IsWeaponClassifiedAs(tw, "殺") Then
					ratio = 1#
				Else
					ratio = 0.5
				End If
				If t.IsNormalWeapon(tw) Then
					'ダメージは常に最悪の状況を考えてクリティカル時の値に
					If IsOptionDefined("ダメージ倍率低下") Then
						ratio = 1.2 * ratio
					Else
						ratio = 1.5 * ratio
					End If
				End If
				ux = .x
				uy = .y
				uarea = .Area
				.x = x
				.y = y
				.Area = Area
				dmg = t.ExpDamage(tw, u, True, ratio)
				.x = ux
				.y = uy
				.Area = uarea
				
				'ボスはザコを見殺しにする！
				If .BossRank > BossRank Then
					'被るダメージが少ない場合は別だけど……
					If dmg >= .MaxHP \ 20 Or dmg >= .HP \ 5 Then
						GoTo NextUnit
					End If
				End If
				
				'自分が倒されてしまうような場合はかばわない(クリティカルを含む)
				If dmg >= .HP Then
					'ボスは例外……
					If .BossRank >= BossRank Then
						GoTo NextUnit
					End If
				End If
			End With
			
			LookForSupportGuard = u
NextUnit: 
		Next 
	End Function
	
	'(tx,ty)の地点の周囲にサポートを行ってくれるユニットがいるかどうかを判定。
	Public Function LookForSupport(ByVal tx As Short, ByVal ty As Short, Optional ByVal for_attack As Boolean = False) As Short
		Dim u As Unit
		Dim i As Short
		Dim do_support As Boolean
		Dim team, uteam As String
		
		With MainPilot
			'自分自身がサポートを行うことが出来るか？
			If .IsSkillAvailable("援護") Or .IsSkillAvailable("援護攻撃") Or .IsSkillAvailable("援護防御") Or .IsSkillAvailable("指揮") Or .IsSkillAvailable("広域サポート") Then
				do_support = True
			End If
			
			team = .SkillData("チーム")
		End With
		
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			u = Nothing
			Select Case i
				Case 1
					If tx > 1 Then
						u = MapDataForUnit(tx - 1, ty)
					End If
				Case 2
					If tx < MapWidth Then
						u = MapDataForUnit(tx + 1, ty)
					End If
				Case 3
					If ty > 1 Then
						u = MapDataForUnit(tx, ty - 1)
					End If
				Case 4
					If ty < MapHeight Then
						u = MapDataForUnit(tx, ty + 1)
					End If
			End Select
			
			If u Is Nothing Then
				GoTo NextUnit
			End If
			If u Is Me Then
				GoTo NextUnit
			End If
			
			With u
				'正常な判断が可能？
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Then
					GoTo NextUnit
				End If
				
				'味方？
				If IsEnemy(u) Or .IsEnemy(Me) Then
					GoTo NextUnit
				End If
				
				'同じチームに属している？
				uteam = .MainPilot.SkillData("チーム")
				If team <> uteam And team <> "" And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'移動のみの場合、相手がこれから移動してしまっては意味がない
				If Not for_attack Then
					If .Action > 0 Then
						GoTo NextUnit
					End If
				End If
				
				'自分自身がサポート可能であれば、相手が誰でも役に立つ
				If do_support Then
					LookForSupport = LookForSupport + 1
				End If
				
				'サポート能力を持っている？
				With .MainPilot
					If .IsSkillAvailable("援護") Or .IsSkillAvailable("援護攻撃") Then
						LookForSupport = LookForSupport + 1
						'これから攻撃する場合、相手が行動出来ればサポートアタックが可能
						If for_attack Then
							If u.Action > 0 Then
								LookForSupport = LookForSupport + 1
								'同時援護攻撃が可能であればさらにボーナス
								If MainPilot.IsSkillAvailable("統率") Then
									LookForSupport = LookForSupport + 1
								End If
							End If
						End If
					ElseIf .IsSkillAvailable("援護防御") Or .IsSkillAvailable("指揮") Or .IsSkillAvailable("広域サポート") Then 
						LookForSupport = LookForSupport + 1
					End If
				End With
			End With
NextUnit: 
		Next 
	End Function
	
	'合体技のパートナーを探す
	'UPGRADE_NOTE: ctype は ctype_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Sub CombinationPartner(ByRef ctype_Renamed As String, ByVal w As Short, ByRef partners() As Unit, Optional ByVal tx As Short = 0, Optional ByVal ty As Short = 0, Optional ByVal check_formation As Boolean = False)
		Dim u As Unit
		Dim uname As String
		Dim j, i, k As Short
		Dim clevel, cnum As Short
		Dim clist As String
		Dim cname As String
		Dim cmorale, cen, cplana As Short
		Dim crange, loop_limit As Short
		
		'正常な判断が可能？
		If IsConditionSatisfied("混乱") Then
			ReDim partners(0)
			Exit Sub
		End If
		
		'合体技のデータを調べておく
		If ctype_Renamed = "武装" Then
			cname = Weapon(w).Name
			cen = WeaponENConsumption(w)
			cmorale = Weapon(w).NecessaryMorale
			If IsWeaponClassifiedAs(w, "霊") Then
				cplana = 5 * WeaponLevel(w, "霊")
			ElseIf IsWeaponClassifiedAs(w, "プ") Then 
				cplana = 5 * WeaponLevel(w, "プ")
			End If
			crange = WeaponMaxRange(w)
		Else
			cname = Ability(w).Name
			cen = AbilityENConsumption(w)
			cmorale = Ability(w).NecessaryMorale
			If IsAbilityClassifiedAs(w, "霊") Then
				cplana = 5 * AbilityLevel(w, "霊")
			ElseIf IsAbilityClassifiedAs(w, "プ") Then 
				cplana = 5 * AbilityLevel(w, "プ")
			End If
			crange = AbilityMaxRange(w)
		End If
		
		'ユニットの特殊能力「合体技」の検索
		For i = 1 To CountFeature
			If Feature(i) = "合体技" Then
				If LIndex(FeatureData(i), 1) = cname Then
					If IsFeatureLevelSpecified(i) Then
						clevel = FeatureLevel(i)
					Else
						clevel = 0
					End If
					clist = ListTail(FeatureData(i), 2)
					cnum = LLength(clist)
					Exit For
				End If
			End If
		Next 
		If i > CountFeature Then
			ReDim partners(0)
			Exit Sub
		End If
		
		'出撃していない場合
		If Status_Renamed <> "出撃" Or MapFileName = "" Then
			'パートナーが仲間にいるだけでよい
			For i = 1 To cnum
				uname = LIndex(clist, i)
				
				'パートナーがユニット名で指定されている場合
				If UList.IsDefined(uname) Then
					With UList.Item(uname)
						If .Status_Renamed = "出撃" Or .Status_Renamed = "待機" Then
							GoTo NextPartner
						End If
					End With
				End If
				
				'パートナーがパイロット名で指定されている場合
				If PList.IsDefined(uname) Then
					If Not PList.Item(uname).Unit_Renamed Is Nothing Then
						With PList.Item(uname).Unit_Renamed
							If .Status_Renamed = "出撃" Or .Status_Renamed = "待機" Then
								GoTo NextPartner
							End If
						End With
					End If
				End If
				
				'パートナーが見つからなかった
				ReDim partners(0)
				Exit Sub
NextPartner: 
			Next 
			'パートナーが全員仲間にいる
			ReDim partners(cnum)
			Exit Sub
		End If
		
		'合体技の基点の設定
		If tx = 0 Then
			tx = x
		End If
		If ty = 0 Then
			ty = y
		End If
		
		'パートナーの検索範囲を設定
		
		If crange = 1 Then
			If cnum >= 8 Then
				'射程１で８体合体以上の場合は２マス以内
				loop_limit = 12
			ElseIf cnum >= 4 Then 
				'射程１で４体合体以上の場合は斜め隣接可
				loop_limit = 8
			Else
				'どれにも該当していなければ隣接のみ
				loop_limit = 4
			End If
		Else
			If cnum >= 9 Then
				'射程２以上で９体合体以上の場合は２マス以内
				loop_limit = 12
			ElseIf cnum >= 5 Then 
				'射程２以上で５体合体以上の場合は斜め隣接可
				loop_limit = 8
			Else
				'どれにも該当していなければ隣接のみ
				loop_limit = 4
			End If
		End If
		
		'合体技斜め隣接可オプション
		If IsOptionDefined("合体技斜め隣接可") Then
			If loop_limit = 4 Then
				loop_limit = 8
			End If
		End If
		
		ReDim partners(0)
		For i = 1 To cnum
			'パートナーの名称
			uname = LIndex(clist, i)
			
			For j = 1 To loop_limit
				'パートナーの検索位置設定
				'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				u = Nothing
				Select Case j
					Case 1
						If tx > 1 Then
							u = MapDataForUnit(tx - 1, ty)
						End If
					Case 2
						If tx < MapWidth Then
							u = MapDataForUnit(tx + 1, ty)
						End If
					Case 3
						If ty > 1 Then
							u = MapDataForUnit(tx, ty - 1)
						End If
					Case 4
						If ty < MapHeight Then
							u = MapDataForUnit(tx, ty + 1)
						End If
					Case 5
						If tx > 1 And ty > 1 Then
							u = MapDataForUnit(tx - 1, ty - 1)
						End If
					Case 6
						If tx < MapWidth And ty < MapHeight Then
							u = MapDataForUnit(tx + 1, ty + 1)
						End If
					Case 7
						If tx > 1 And ty < MapHeight Then
							u = MapDataForUnit(tx - 1, ty + 1)
						End If
					Case 8
						If tx < MapWidth And ty > 1 Then
							u = MapDataForUnit(tx + 1, ty - 1)
						End If
					Case 9
						If tx > 2 Then
							u = MapDataForUnit(tx - 2, ty)
						End If
					Case 10
						If tx < MapWidth - 1 Then
							u = MapDataForUnit(tx + 2, ty)
						End If
					Case 11
						If ty > 2 Then
							u = MapDataForUnit(tx, ty - 2)
						End If
					Case 12
						If ty < MapHeight - 1 Then
							u = MapDataForUnit(tx, ty + 2)
						End If
				End Select
				
				'ユニットが存在する？
				If u Is Nothing Then
					GoTo NextNeighbor
				End If
				
				With u
					'合体技のパートナーに該当する？
					If .Name <> uname Then
						'パイロット名でも確認
						If .MainPilot.Name <> uname Then
							GoTo NextNeighbor
						End If
					End If
					
					'ユニットが自分？
					If u Is Me Then
						GoTo NextNeighbor
					End If
					
					'既に選択済み？
					For k = 1 To UBound(partners)
						If u Is partners(k) Then
							GoTo NextNeighbor
						End If
					Next 
					
					'ユニットが敵？
					If IsEnemy(u) Then
						GoTo NextNeighbor
					End If
					
					'行動出来なければだめ
					If .MaxAction = 0 Or .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("憑依") Then
						GoTo NextNeighbor
					End If
					
					'合体技にレベルが設定されていればパイロット間の信頼度をチェック
					If clevel > 0 Then
						If MainPilot.Relation(.MainPilot) < clevel Or .MainPilot.Relation(MainPilot) < clevel Then
							GoTo NextNeighbor
						End If
					End If
					
					'パートナーが武器を使うための条件を満たしているかを判定
					If Not check_formation Then
						If ctype_Renamed = "武装" Then
							'合体技と同名の武器を検索
							For k = 1 To .CountWeapon
								If .Weapon(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountWeapon Then
								'武器が使える？
								If Not .IsWeaponMastered(k) Then
									GoTo NextNeighbor
								End If
								If .Weapon(k).NecessaryMorale > 0 Then
									If .MainPilot.Morale < .Weapon(k).NecessaryMorale Then
										GoTo NextNeighbor
									End If
								End If
								If .WeaponENConsumption(k) > 0 Then
									If .EN < .WeaponENConsumption(k) Then
										GoTo NextNeighbor
									End If
								End If
								If .Weapon(k).Bullet > 0 Then
									If .Bullet(k) = 0 Then
										GoTo NextNeighbor
									End If
								End If
								If .WeaponLevel(k, "霊") > 0 Then
									If .MainPilot.Plana < 5 * .WeaponLevel(k, "霊") Then
										GoTo NextNeighbor
									End If
								ElseIf .WeaponLevel(k, "プ") > 0 Then 
									If .MainPilot.Plana < 5 * .WeaponLevel(k, "プ") Then
										GoTo NextNeighbor
									End If
								End If
							Else
								'同名の武器を持っていなかった場合はチェック項目を限定
								If cmorale > 0 Then
									If .MainPilot.Morale < cmorale Then
										GoTo NextNeighbor
									End If
								End If
								If cen > 0 Then
									If .EN < cen Then
										GoTo NextNeighbor
									End If
								End If
								If cplana > 0 Then
									If .MainPilot.Plana < cplana Then
										GoTo NextNeighbor
									End If
								End If
							End If
						Else
							'合体技と同名のアビリティを検索
							For k = 1 To .CountAbility
								If .Ability(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountAbility Then
								'アビリティが使える？
								If Not .IsAbilityMastered(k) Then
									GoTo NextNeighbor
								End If
								If .Ability(k).NecessaryMorale > 0 Then
									If .MainPilot.Morale < .Ability(k).NecessaryMorale Then
										GoTo NextNeighbor
									End If
								End If
								If .AbilityENConsumption(k) > 0 Then
									If .EN < .AbilityENConsumption(k) Then
										GoTo NextNeighbor
									End If
								End If
								If .Ability(k).Stock > 0 Then
									If .Stock(k) = 0 Then
										GoTo NextNeighbor
									End If
								End If
								If .AbilityLevel(k, "霊") > 0 Then
									If .MainPilot.Plana < 5 * .AbilityLevel(k, "霊") Then
										GoTo NextNeighbor
									End If
								ElseIf .AbilityLevel(k, "プ") > 0 Then 
									If .MainPilot.Plana < 5 * .AbilityLevel(k, "プ") Then
										GoTo NextNeighbor
									End If
								End If
							Else
								'同名のアビリティを持っていなかった場合はチェック項目を限定
								If cmorale > 0 Then
									If .MainPilot.Morale < cmorale Then
										GoTo NextNeighbor
									End If
								End If
								If cen > 0 Then
									If .EN < cen Then
										GoTo NextNeighbor
									End If
								End If
								If cplana > 0 Then
									If .MainPilot.Plana < cplana Then
										GoTo NextNeighbor
									End If
								End If
							End If
						End If
					Else
						'フォーメーションのチェックだけの時も必要技能は調べておく
						If ctype_Renamed = "武装" Then
							For k = 1 To .CountWeapon
								If .Weapon(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountWeapon Then
								If Not .IsWeaponMastered(k) Then
									GoTo NextNeighbor
								End If
							End If
						Else
							For k = 1 To .CountAbility
								If .Ability(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountAbility Then
								If Not .IsAbilityMastered(k) Then
									GoTo NextNeighbor
								End If
							End If
						End If
					End If
				End With
				
				'見つかったパートナーを記録
				ReDim Preserve partners(i)
				partners(i) = u
				Exit For
NextNeighbor: 
			Next 
			
			'パートナーが見つからなかった？
			If j > loop_limit Then
				ReDim partners(0)
				Exit Sub
			End If
		Next 
		
		'合体技メッセージ判定用にパートナー一覧を記録
		ReDim SelectedPartners(UBound(partners))
		For i = 1 To UBound(partners)
			SelectedPartners(i) = partners(i)
		Next 
	End Sub
	
	'合体技攻撃に必要なパートナーが見つかるか？
	Public Function IsCombinationAttackAvailable(ByVal w As Short, Optional ByVal check_formation As Boolean = False) As Boolean
		Dim partners() As Unit
		
		ReDim partners(0)
		If Status_Renamed = "待機" Or MapFileName = "" Then
			'出撃時以外は相手が仲間にいるだけでＯＫ
			CombinationPartner("武装", w, partners, x, y)
		ElseIf WeaponMaxRange(w) = 1 And Not IsWeaponClassifiedAs(w, "Ｍ") Then 
			'射程１の場合は自分の周りのいずれかの敵ユニットに対して合体技が使えればＯＫ
			If x > 1 Then
				If Not MapDataForUnit(x - 1, y) Is Nothing Then
					If IsEnemy(MapDataForUnit(x - 1, y)) Then
						CombinationPartner("武装", w, partners, x - 1, y, check_formation)
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If x < MapWidth Then
					If Not MapDataForUnit(x + 1, y) Is Nothing Then
						If IsEnemy(MapDataForUnit(x + 1, y)) Then
							CombinationPartner("武装", w, partners, x + 1, y, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > 1 Then
					If Not MapDataForUnit(x, y - 1) Is Nothing Then
						If IsEnemy(MapDataForUnit(x, y - 1)) Then
							CombinationPartner("武装", w, partners, x, y - 1, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y < MapHeight Then
					If Not MapDataForUnit(x, y + 1) Is Nothing Then
						If IsEnemy(MapDataForUnit(x, y + 1)) Then
							CombinationPartner("武装", w, partners, x, y + 1, check_formation)
						End If
					End If
				End If
			End If
		Else
			'射程２以上の場合は自分の周りにパートナーがいればＯＫ
			CombinationPartner("武装", w, partners, x, y, check_formation)
		End If
		
		'条件を満たすパートナーの組が見つかったか判定
		If UBound(partners) > 0 Then
			IsCombinationAttackAvailable = True
		Else
			IsCombinationAttackAvailable = False
		End If
	End Function
	
	'合体技アビリティに必要なパートナーが見つかるか？
	Public Function IsCombinationAbilityAvailable(ByVal a As Short, Optional ByVal check_formation As Boolean = False) As Boolean
		Dim partners() As Unit
		
		ReDim partners(0)
		If Status_Renamed = "待機" Or MapFileName = "" Then
			'出撃時以外は相手が仲間にいるだけでＯＫ
			CombinationPartner("アビリティ", a, partners, x, y)
		ElseIf AbilityMaxRange(a) = 1 And Not IsAbilityClassifiedAs(a, "Ｍ") Then 
			'射程１の場合は自分の周りのいずれかの味方ユニットに対して合体技が使えればＯＫ
			If x > 1 Then
				If Not MapDataForUnit(x - 1, y) Is Nothing Then
					If IsAlly(MapDataForUnit(x - 1, y)) Then
						CombinationPartner("アビリティ", a, partners, x - 1, y, check_formation)
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If x < MapWidth Then
					If Not MapDataForUnit(x + 1, y) Is Nothing Then
						If IsAlly(MapDataForUnit(x + 1, y)) Then
							CombinationPartner("アビリティ", a, partners, x + 1, y, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > 1 Then
					If Not MapDataForUnit(x, y - 1) Is Nothing Then
						If IsAlly(MapDataForUnit(x, y - 1)) Then
							CombinationPartner("アビリティ", a, partners, x, y - 1, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > MapHeight Then
					If Not MapDataForUnit(x, y + 1) Is Nothing Then
						If IsAlly(MapDataForUnit(x, y + 1)) Then
							CombinationPartner("アビリティ", a, partners, x, y + 1, check_formation)
						End If
					End If
				End If
			End If
		Else
			'射程２以上の場合は自分の周りにパートナーがいればＯＫ
			CombinationPartner("アビリティ", a, partners, x, y, check_formation)
		End If
		
		'条件を満たすパートナーの組が見つかったか判定
		If UBound(partners) > 0 Then
			IsCombinationAbilityAvailable = True
		Else
			IsCombinationAbilityAvailable = False
		End If
	End Function
	
	'(tx,ty)にユニットが進入可能か？
	Public Function IsAbleToEnter(ByVal tx As Short, ByVal ty As Short) As Boolean
		Dim ignore_move_cost As Boolean
		
		'使用不能の形態はどの地形に対しても進入不可能とみなす
		If Not IsAvailable() Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		'単に必要技能をチェックしている場合？
		If MapFileName = "" Then
			IsAbleToEnter = True
			Exit Function
		End If
		
		'マップ外？
		If tx < 1 Or MapWidth < tx Or ty < 1 Or MapHeight < ty Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		'地形適応チェック
		Select Case TerrainClass(tx, ty)
			Case "空"
				If Not IsTransAvailable("空") And Not CurrentForm.IsFeatureAvailable("空中移動") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "水"
				If IsTransAvailable("空") Or CurrentForm.IsFeatureAvailable("空中移動") Or IsTransAvailable("水上") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Adaption(3) = 0 And Not CurrentForm.IsFeatureAvailable("水中移動") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "深水"
				If IsTransAvailable("空") Or CurrentForm.IsFeatureAvailable("空中移動") Or IsTransAvailable("水上") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Not IsTransAvailable("水") And Not CurrentForm.IsFeatureAvailable("水中移動") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "宇宙"
				If Adaption(4) = 0 And Not CurrentForm.IsFeatureAvailable("宇宙移動") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "月面"
				If IsTransAvailable("空") Or CurrentForm.IsFeatureAvailable("空中移動") Or IsTransAvailable("宇") Or CurrentForm.IsFeatureAvailable("宇宙移動") Then
					IsAbleToEnter = True
					Exit Function
				End If
			Case Else
				If IsTransAvailable("空") Or CurrentForm.IsFeatureAvailable("空中移動") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Not IsTransAvailable("陸") And Not CurrentForm.IsFeatureAvailable("陸上移動") Then
					IsAbleToEnter = False
					Exit Function
				End If
		End Select
		
		'進入不能？
		If TerrainMoveCost(tx, ty) >= 1000 Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		IsAbleToEnter = True
	End Function
	
	'この形態が使用可能か？ (Disable＆必要技能のチェック)
	Public Function IsAvailable() As Boolean
		Dim i As Short
		
		IsAvailable = True
		
		'イベントコマンド「Disable」
		If IsDisabled(Name) Then
			IsAvailable = False
			Exit Function
		End If
		
		'制限時間の切れた形態？
		If Status_Renamed = "他形態" Then
			If IsConditionSatisfied("行動不能") Then
				IsAvailable = False
				Exit Function
			End If
		End If
		
		With CurrentForm
			'技能チェックが必要？
			If .CountPilot = 0 Or (Not IsFeatureAvailable("必要技能") And Not IsFeatureAvailable("不必要技能")) Then
				Exit Function
			End If
			
			'必要技能をチェック
			For i = 1 To CountFeature
				Select Case Feature(i)
					Case "必要技能"
						If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
							IsAvailable = False
							Exit Function
						End If
					Case "不必要技能"
						If .IsNecessarySkillSatisfied(FeatureData(i)) Then
							IsAvailable = False
							Exit Function
						End If
				End Select
			Next 
		End With
	End Function
	
	'必要技能を満たしているか？
	Public Function IsNecessarySkillSatisfied(ByRef nabilities As String, Optional ByRef p As Pilot = Nothing) As Boolean
		Dim i, num As Short
		Dim nskill_list(100) As String
		
		If Len(nabilities) = 0 Then
			IsNecessarySkillSatisfied = True
			Exit Function
		End If
		
		num = LLength(nabilities)
		For i = 1 To MinLng(num, 100)
			nskill_list(i) = LIndex(nabilities, i)
		Next 
		
		'個々の必要条件をチェック
		i = 1
		Do While i <= MinLng(num, 100)
			If IsNecessarySkillSatisfied2(nskill_list(i), p) Then
				'必要条件が満たされた場合、その後の「or」をスキップ
				If i <= num - 2 Then
					Do While LCase(nskill_list(i + 1)) = "or"
						i = i + 2
						'検査する必要条件が無くなったので必要技能が満たされたと判定
						If i = num Then
							IsNecessarySkillSatisfied = True
							Exit Function
						ElseIf i > num Then 
							'orの後ろに必要条件がない
							ErrorMessage(Name & "に対する必要技能「" & nabilities & "」が不正です")
							TerminateSRC()
						End If
					Loop 
				End If
			Else
				'必要条件が満たされなかった場合、その後に「or」がなければ
				'必要技能が満たされなかったと判定
				If i > num - 2 Then
					Exit Function
				End If
				i = i + 1
				If LCase(nskill_list(i)) <> "or" Then
					Exit Function
				End If
			End If
			i = i + 1
		Loop 
		
		IsNecessarySkillSatisfied = True
	End Function
	
	Public Function IsNecessarySkillSatisfied2(ByRef ndata As String, ByRef p As Pilot) As Boolean
		Dim stype2, stype, sname As String
		Dim slevel As Double
		Dim nlevel As Double
		Dim mp As Pilot
		Dim i, j As Short
		
		'ステータスコマンド実行時は条件が満たされていると見なす？
		If Left(ndata, 1) = "+" Then
			If Status_Renamed = "出撃" And InStatusCommand() Then
				IsNecessarySkillSatisfied2 = True
				Exit Function
			End If
			ndata = Mid(ndata, 2)
		End If
		
		'召喚者技能を参照？
		If Left(ndata, 1) = "*" Then
			If Summoner Is Nothing Then
				Exit Function
			End If
			IsNecessarySkillSatisfied2 = Summoner.IsNecessarySkillSatisfied2(Mid(ndata, 2), Nothing)
			Exit Function
		End If
		
		i = InStr(ndata, "Lv")
		If i > 0 Then
			sname = Left(ndata, i - 1)
			nlevel = StrToDbl(Mid(ndata, i + 2))
		Else
			sname = ndata
			nlevel = 1
		End If
		
		'不必要技能？
		If Left(sname, 1) = "!" Then
			IsNecessarySkillSatisfied2 = Not IsNecessarySkillSatisfied2(Mid(ndata, 2), p)
			Exit Function
		End If
		
		'必要技能の判定に使用するパイロットを設定
		If p Is Nothing Then
			If CountPilot > 0 Then
				mp = MainPilot
			Else
				With CurrentForm
					If .CountPilot > 0 Then
						mp = .MainPilot
					End If
				End With
			End If
		Else
			mp = p
		End If
		
		'ダミーパイロットの場合は無視
		If Not mp Is Nothing Then
			If mp.Nickname0 = "パイロット不在" Then
				'UPGRADE_NOTE: オブジェクト mp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				mp = Nothing
			End If
		End If
		
		slevel = -10000
		
		'まず名称が変わらない必要技能を判定
		Select Case sname
			Case "レベル"
				If Not mp Is Nothing Then
					slevel = mp.Level
				Else
					slevel = 0
				End If
			Case "格闘"
				If Not mp Is Nothing Then
					slevel = mp.InfightBase
				Else
					slevel = 0
				End If
			Case "射撃"
				slevel = 0
				If Not mp Is Nothing Then
					If Not mp.HasMana() Then
						slevel = mp.ShootingBase
					End If
				End If
			Case "魔力"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.HasMana() Then
						slevel = mp.ShootingBase
					End If
				End If
			Case "命中"
				If Not mp Is Nothing Then
					slevel = mp.HitBase
				Else
					slevel = 0
				End If
			Case "回避"
				If Not mp Is Nothing Then
					slevel = mp.DodgeBase
				Else
					slevel = 0
				End If
			Case "技量"
				If Not mp Is Nothing Then
					slevel = mp.TechniqueBase
				Else
					slevel = 0
				End If
			Case "反応"
				If Not mp Is Nothing Then
					slevel = mp.IntuitionBase
				Else
					slevel = 0
				End If
			Case "格闘初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If IsOptionDefined("攻撃力低成長") Then
							slevel = .InfightBase - .Level \ 2
						Else
							slevel = .InfightBase - .Level
						End If
					End With
				End If
			Case "射撃初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If Not .HasMana() Then
							If IsOptionDefined("攻撃力低成長") Then
								slevel = .ShootingBase - .Level \ 2
							Else
								slevel = .ShootingBase - .Level
							End If
						End If
					End With
				End If
			Case "魔力初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If .HasMana() Then
							If IsOptionDefined("攻撃力低成長") Then
								slevel = .ShootingBase - .Level \ 2
							Else
								slevel = .ShootingBase - .Level
							End If
						End If
					End With
				End If
			Case "命中初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .HitBase - .Level
					End With
				End If
			Case "回避初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .DodgeBase - .Level
					End With
				End If
			Case "技量初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .TechniqueBase - .Level
					End With
				End If
			Case "反応初期値"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .IntuitionBase - .Level
					End With
				End If
			Case "男性"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.Sex = "男性" Then
						slevel = 1
					End If
					If Data.PilotNum > 1 Then
						For i = 1 To CountPilot
							If Pilot(i).Sex = "男性" Then
								slevel = 1
							End If
						Next 
					End If
					For i = 1 To CountSupport
						If Support(i).Sex = "男性" Then
							slevel = 1
						End If
					Next 
					If IsFeatureAvailable("追加サポート") Then
						If AdditionalSupport.Sex = "男性" Then
							slevel = 1
						End If
					End If
				End If
			Case "女性"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.Sex = "女性" Then
						slevel = 1
					End If
					If Data.PilotNum > 1 Then
						For i = 1 To CountPilot
							If Pilot(i).Sex = "女性" Then
								slevel = 1
							End If
						Next 
					End If
					For i = 1 To CountSupport
						If Support(i).Sex = "女性" Then
							slevel = 1
						End If
					Next 
					If IsFeatureAvailable("追加サポート") Then
						If AdditionalSupport.Sex = "女性" Then
							slevel = 1
						End If
					End If
				End If
			Case "生身"
				If IsHero Then
					slevel = 1
				Else
					slevel = 0
				End If
			Case "瀕死"
				If HP <= MaxHP \ 4 Then
					slevel = 1
				Else
					slevel = 0
				End If
			Case "ＨＰ"
				slevel = 10# * HP / MaxHP
			Case "ＥＮ"
				slevel = 10# * EN / MaxEN
			Case "気力"
				If Not mp Is Nothing Then
					slevel = CDbl(mp.Morale - 100#)
					slevel = CDbl(slevel / 10#)
				Else
					slevel = 0
				End If
			Case "ランク"
				slevel = Rank
			Case "地上", "空中", "水中", "水上", "宇宙", "地中"
				slevel = 0
				If Status_Renamed = "出撃" Then
					If sname = Area Then
						slevel = 1
					End If
				End If
			Case "アイテム"
				'使い捨てアイテム表記用
				slevel = 1
			Case "当て身技", "自動反撃"
				'アビリティで付加された当て身技及び自動反撃専用の武器が表示されるのを
				'防ぐため、これらの必要技能は常に満たされないとみなす
				Exit Function
		End Select
		
		'上の条件のいずれかに該当？
		If slevel <> -10000 Then
			'指定された技能のレベルが必要なレベル以上の場合に必要技能が満たされたと判定
			If slevel >= nlevel Then
				IsNecessarySkillSatisfied2 = True
			End If
			Exit Function
		End If
		
		'必要技能の種類を判別
		If Not mp Is Nothing Then
			stype = mp.SkillType(sname)
		Else
			stype = sname
		End If
		
		'名称が変わる可能性がある必要技能を判定
		Dim iname As String
		Dim uname As String
		Dim u As Unit
		Dim max_range As Short
		Select Case stype
			Case "超感覚"
				If Not p Is Nothing Then
					slevel = p.SkillLevel("超感覚")
					If stype <> sname Then
						If p.SkillNameForNS(stype) <> sname Then
							slevel = 0
						End If
					End If
					slevel = slevel + p.SkillLevel("知覚強化")
				ElseIf Not mp Is Nothing Then 
					slevel = mp.SkillLevel("超感覚")
					If Data.PilotNum > 1 Then
						For i = 2 To CountPilot
							With Pilot(i)
								slevel = MaxDbl(slevel, .SkillLevel("超感覚"))
								slevel = MaxDbl(slevel, .SkillLevel(sname))
							End With
						Next 
					End If
					For i = 1 To CountSupport
						With Support(i)
							slevel = MaxDbl(slevel, .SkillLevel("超感覚"))
							slevel = MaxDbl(slevel, .SkillLevel(sname))
						End With
					Next 
					If IsFeatureAvailable("追加サポート") Then
						With AdditionalSupport
							slevel = MaxDbl(slevel, .SkillLevel("超感覚"))
							slevel = MaxDbl(slevel, .SkillLevel(sname))
						End With
					End If
					
					If stype <> sname Then
						If mp.SkillNameForNS(stype) <> sname Then
							slevel = 0
						End If
					End If
					
					slevel = slevel + mp.SkillLevel("知覚強化")
					If Data.PilotNum > 1 Then
						For i = 2 To CountPilot
							slevel = MaxDbl(slevel, Pilot(i).SkillLevel("知覚強化"))
						Next 
					End If
					For i = 1 To CountSupport
						slevel = MaxDbl(slevel, Support(i).SkillLevel("知覚強化"))
					Next 
					If IsFeatureAvailable("追加サポート") Then
						slevel = MaxDbl(slevel, AdditionalSupport.SkillLevel("知覚強化"))
					End If
				End If
			Case "同調率"
				If Not p Is Nothing Then
					slevel = p.SynchroRate
				ElseIf Not mp Is Nothing Then 
					slevel = mp.SynchroRate
					If Data.PilotNum > 1 Then
						For i = 2 To CountPilot
							slevel = MaxDbl(slevel, Pilot(i).SynchroRate)
						Next 
					End If
					For i = 1 To CountSupport
						slevel = MaxDbl(slevel, Support(i).SynchroRate)
					Next 
					If IsFeatureAvailable("追加サポート") Then
						slevel = MaxDbl(slevel, AdditionalSupport.SynchroRate)
					End If
				End If
				If stype <> sname Then
					If mp.SkillNameForNS(stype) <> sname Then
						slevel = 0
					End If
				End If
			Case "オーラ"
				If Not p Is Nothing Then
					slevel = p.SkillLevel("オーラ")
				ElseIf Not mp Is Nothing Then 
					slevel = AuraLevel()
				End If
				If stype <> sname Then
					If mp.SkillNameForNS(stype) <> sname Then
						slevel = 0
					End If
				End If
			Case "霊力"
				If Not p Is Nothing Then
					slevel = p.Plana
				ElseIf Not mp Is Nothing Then 
					slevel = mp.Plana
				End If
				If stype <> sname Then
					If mp.SkillNameForNS(stype) <> sname Then
						slevel = 0
					End If
				End If
			Case Else
				'上記以外のパイロット用特殊能力
				
				If Not mp Is Nothing Then
					With mp
						'特定パイロット専用？
						If sname = .Name Or sname = .Nickname Then
							slevel = 1
						ElseIf stype = sname Then 
							slevel = .SkillLevel(stype)
						ElseIf .SkillNameForNS(stype) = sname Then 
							slevel = .SkillLevel(stype)
						End If
					End With
					
					'パイロット数が括弧つきでない場合のみ
					If Data.PilotNum > 1 Then
						'サブパイロットの技能を検索
						For i = 2 To CountPilot
							With Pilot(i)
								If sname = .Name Or sname = .Nickname Then
									slevel = 1
									Exit For
								End If
								
								stype2 = .SkillType(sname)
								If stype2 = sname Then
									slevel = MaxDbl(slevel, .SkillLevel(stype2))
								ElseIf .SkillNameForNS(stype2) = sname Then 
									slevel = MaxDbl(slevel, .SkillLevel(stype2))
								End If
							End With
						Next 
					End If
					
					'サポートパイロットの技能を検索
					For i = 1 To CountSupport
						With Support(i)
							If sname = .Name Or sname = .Nickname Then
								slevel = 1
								Exit For
							End If
							stype2 = .SkillType(sname)
							If stype2 = sname Then
								slevel = MaxDbl(slevel, .SkillLevel(stype2))
							ElseIf .SkillNameForNS(stype2) = sname Then 
								slevel = MaxDbl(slevel, .SkillLevel(stype2))
							End If
						End With
					Next 
					
					'追加サポートの技能を検索
					If IsFeatureAvailable("追加サポート") And CountPilot > 0 Then
						With AdditionalSupport
							If sname = .Name Or sname = .Nickname Then
								slevel = 1
							Else
								stype2 = .SkillType(sname)
								If stype2 = sname Then
									slevel = MaxDbl(slevel, .SkillLevel(stype2))
								ElseIf .SkillNameForNS(stype2) = sname Then 
									slevel = MaxDbl(slevel, .SkillLevel(stype2))
								End If
							End If
						End With
					End If
				End If
				
				If slevel = 0 Then
					'ユニット名またはクラスに該当？
					If sname = Name Or sname = Nickname0 Or sname = Class0 Then
						slevel = 1
					End If
				End If
				
				If slevel = 0 Then
					If Left(sname, 1) = "@" Then
						'地形を指定した必要技能
						If Status_Renamed = "出撃" And 1 <= x And x <= MapWidth And 1 <= y And y <= MapHeight Then
							If Mid(sname, 2) = TerrainName(x, y) Then
								slevel = 1
							End If
						End If
					ElseIf Right(sname, 2) = "装備" Then 
						'アイテムを指定した必要技能
						iname = Left(sname, Len(sname) - 2)
						For i = 1 To CountItem
							With Item(i)
								If .Activated Then
									If iname = .Name Or iname = .Class0 Then
										slevel = 1
										Exit For
									End If
								End If
							End With
						Next 
					ElseIf Right(sname, 2) = "隣接" Or Right(sname, 4) = "マス以内" Then 
						'特定のユニットが近くにいることを指定した必要技能
						If Status_Renamed = "出撃" Then
							If Right(sname, 2) = "隣接" Then
								uname = Left(sname, Len(sname) - 2)
								max_range = 1
							Else
								uname = Left(sname, Len(sname) - 5)
								max_range = StrToLng(Mid(sname, Len(sname) - 4, 1))
							End If
							
							For i = MaxLng(x - max_range, 1) To MinLng(x + max_range, MapWidth)
								For j = MaxLng(y - max_range, 1) To MinLng(y + max_range, MapHeight)
									u = MapDataForUnit(i, j)
									
									'距離が範囲外？
									If System.Math.Abs(x - i) + System.Math.Abs(y - j) > max_range Then
										GoTo NextNeighbor
									End If
									
									'ユニットがいない？
									If u Is Nothing Then
										GoTo NextNeighbor
									End If
									
									'ユニットが自分？
									If u Is Me Or (x = i And y = j) Then
										GoTo NextNeighbor
									End If
									
									'ユニットが敵？
									If IsEnemy(u) Then
										GoTo NextNeighbor
									End If
									
									With u
										'合体技のパートナーに該当するか
										If uname = "母艦" Then
											If Not .IsFeatureAvailable("母艦") Then
												GoTo NextNeighbor
											End If
										Else
											If .Name <> uname And .MainPilot.Name <> uname Then
												GoTo NextNeighbor
											End If
										End If
										
										'行動出来なければだめ
										If .MaxAction = 0 Or .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("憑依") Then
											GoTo NextNeighbor
										End If
									End With
									
									'パートナーが見つかった
									IsNecessarySkillSatisfied2 = True
									Exit Function
NextNeighbor: 
								Next 
							Next 
						End If
					ElseIf Right(sname, 2) = "状態" Then 
						'特殊状態を指定した必要技能
						If IsConditionSatisfied(Left(sname, Len(sname) - 2)) Then
							slevel = 1
						End If
					End If
				End If
		End Select
		
		'指定された技能のレベルが必要なレベル以上の場合に必要技能が満たされたと判定
		If slevel >= nlevel Then
			IsNecessarySkillSatisfied2 = True
		End If
	End Function
	
	'能力 fname を封印されているか？
	Public Function IsDisabled(ByRef fname As String) As Boolean
		If Len(fname) = 0 Then
			IsDisabled = False
			Exit Function
		End If
		
		If IsGlobalVariableDefined("Disable(" & fname & ")") Then
			IsDisabled = True
			Exit Function
		End If
		
		If IsGlobalVariableDefined("Disable(" & Name & "," & fname & ")") Then
			IsDisabled = True
			Exit Function
		End If
		
		IsDisabled = False
	End Function
	
	
	'現在、自分が攻撃を受けている側かどうか判定
	Public Function IsDefense() As Boolean
		If Party = Stage Then
			IsDefense = False
		Else
			IsDefense = True
		End If
	End Function
End Class