Option Strict Off
Option Explicit On
Friend Class Pilot
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'作成されたパイロットのクラス
	
	'パイロットデータへのポインタ
	Public Data As PilotData
	
	'識別用ＩＤ
	Public ID As String
	'所属陣営
	Public Party As String
	'搭乗しているユニット
	'未搭乗時は Nothing
	'UPGRADE_NOTE: Unit は Unit_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Unit_Renamed As Unit
	
	'生きているかどうか
	Public Alive As Boolean
	
	'Leaveしているかどうか
	Public Away As Boolean
	
	'追加パイロットかどうか
	Public IsAdditionalPilot As Boolean
	
	'追加サポートかどうか
	Public IsAdditionalSupport As Boolean
	
	'サポートパイロットとして乗り込んだ時の順番
	Public SupportIndex As Short
	
	'レベル
	Private proLevel As Short
	
	'経験値
	Private proEXP As Short
	'ＳＰ
	Private proSP As Short
	'気力
	Private proMorale As Short
	'霊力
	Private proPlana As Short
	
	'能力値
	Public Infight As Short
	Public Shooting As Short
	Public Hit As Short
	Public Dodge As Short
	Public Technique As Short
	Public Intuition As Short
	Public Adaption As String
	
	'能力値の基本値
	Public InfightBase As Short
	Public ShootingBase As Short
	Public HitBase As Short
	Public DodgeBase As Short
	Public TechniqueBase As Short
	Public IntuitionBase As Short
	
	'能力値の修正値
	
	'特殊能力＆自ユニットによる修正
	Public InfightMod As Short
	Public ShootingMod As Short
	Public HitMod As Short
	Public DodgeMod As Short
	Public TechniqueMod As Short
	Public IntuitionMod As Short
	
	'他ユニットによる支援修正
	Public InfightMod2 As Short
	Public ShootingMod2 As Short
	Public HitMod2 As Short
	Public DodgeMod2 As Short
	Public TechniqueMod2 As Short
	Public IntuitionMod2 As Short
	
	'気力修正値
	Public MoraleMod As Short
	
	'特殊能力
	Private colSkill As New Collection
	
	
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		'UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Data = Nothing
		'UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Unit_Renamed = Nothing
		
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colSkill をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colSkill = Nothing
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
			Data = PDList.Item(Value)
			Update()
		End Set
	End Property
	
	'愛称
	Public ReadOnly Property Nickname0() As String
		Get
			Nickname0 = Data.Nickname
		End Get
	End Property
	
	Public ReadOnly Property Nickname(Optional ByVal dont_call_unit_nickname As Boolean = False) As String
		Get
			Dim idx As Short
			Dim u As Unit
			Dim uname, vname As String
			
			Nickname = Nickname0
			
			'愛称変更
			If Unit_Renamed Is Nothing Then
				ReplaceSubExpression(Nickname)
				Exit Property
			End If
			With Unit_Renamed
				If .CountPilot > 0 Then
					If Not .MainPilot Is Me Then
						ReplaceSubExpression(Nickname)
						Exit Property
					End If
				End If
				
				u = Unit_Renamed
				
				'パイロットステータスコマンド中の場合はユニットを検索する必要がある
				If .Name = "ステータス表示用ダミーユニット" Then
					'メインパイロットかどうかチェック
					vname = "搭乗順番[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "搭乗ユニット[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsFeatureAvailable("パイロット愛称") Then
						Nickname = .FeatureData("パイロット愛称")
						idx = InStr(Nickname, "$(愛称)")
						If idx > 0 Then
							Nickname = Left(Nickname, idx - 1) & Data.Nickname & Mid(Nickname, idx + 5)
						End If
					End If
				End With
			End With
			
			'PilotのNickname()とUnitのNickname()の呼び出しが無限に続かないように
			'Nickname()への呼び出しは無効化
			If dont_call_unit_nickname Then
				ReplaceString(Nickname, "Nickname()", "")
				ReplaceString(Nickname, "nickname()", "")
			End If
			
			'愛称内の式置換のため、デフォルトユニットを一時的に変更する
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Unit_Renamed
			ReplaceSubExpression(Nickname)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'読み仮名
	Public ReadOnly Property KanaName() As String
		Get
			Dim idx As Short
			Dim u As Unit
			Dim uname, vname As String
			
			KanaName = Data.KanaName
			
			'愛称変更
			If Unit_Renamed Is Nothing Then
				ReplaceSubExpression(KanaName)
				Exit Property
			End If
			With Unit_Renamed
				If .CountPilot > 0 Then
					If Not .MainPilot Is Me Then
						ReplaceSubExpression(KanaName)
						Exit Property
					End If
				End If
				
				u = Unit_Renamed
				
				'パイロットステータスコマンド中の場合はユニットを検索する必要がある
				If .Name = "ステータス表示用ダミーユニット" Then
					'メインパイロットかどうかチェック
					vname = "搭乗順番[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "搭乗ユニット[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsFeatureAvailable("パイロット読み仮名") Then
						KanaName = .FeatureData("パイロット読み仮名")
						idx = InStr(KanaName, "$(読み仮名)")
						If idx > 0 Then
							KanaName = Left(KanaName, idx - 1) & Data.KanaName & Mid(KanaName, idx + 5)
						End If
					ElseIf .IsFeatureAvailable("パイロット愛称") Then 
						KanaName = .FeatureData("パイロット愛称")
						idx = InStr(KanaName, "$(愛称)")
						If idx > 0 Then
							KanaName = Left(KanaName, idx - 1) & Data.Nickname & Mid(KanaName, idx + 5)
						End If
						KanaName = StrToHiragana(KanaName)
					End If
				End With
			End With
			
			'読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Unit_Renamed
			ReplaceSubExpression(KanaName)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'性別
	Public ReadOnly Property Sex() As String
		Get
			Sex = Data.Sex
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .IsFeatureAvailable("性別") Then
						Sex = .FeatureData("性別")
					End If
				End With
			End If
		End Get
	End Property
	
	'搭乗するユニットのクラス
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public ReadOnly Property Class_Renamed() As String
		Get
			Class_Renamed = Data.Class_Renamed
		End Get
	End Property
	
	'倒したときに得られる経験値
	Public ReadOnly Property ExpValue() As Short
		Get
			ExpValue = Data.ExpValue
		End Get
	End Property
	
	'性格
	Public ReadOnly Property Personality() As String
		Get
			Personality = Data.Personality
			
			'ユニットに乗っている？
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				'アイテム用特殊能力「性格変更」
				If .IsFeatureAvailable("性格変更") Then
					Personality = .FeatureData("性格変更")
					Exit Property
				End If
				
				'追加パイロットの性格を優先させる
				If Not IsAdditionalPilot Then
					If .CountPilot > 0 Then
						If .Pilot(1) Is Me Then
							Personality = .MainPilot.Data.Personality
						End If
					End If
				End If
			End With
		End Get
	End Property
	
	'ビットマップ
	Public ReadOnly Property Bitmap(Optional ByVal use_orig As Boolean = False) As String
		Get
			Dim u As Unit
			Dim uname, vname As String
			
			If use_orig Then
				Bitmap = Data.Bitmap0
			Else
				Bitmap = Data.Bitmap
			End If
			
			'パイロット画像変更
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			With Unit_Renamed
				If .CountPilot > 0 Then
					If Not .MainPilot Is Me Then
						Exit Property
					End If
				End If
				
				u = Unit_Renamed
				
				'パイロットステータスコマンド中の場合はユニットを検索する必要がある
				If .Name = "ステータス表示用ダミーユニット" Then
					'メインパイロットかどうかチェック
					vname = "搭乗順番[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "搭乗ユニット[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsConditionSatisfied("パイロット画像") Then
						Bitmap = LIndex(.ConditionData("パイロット画像"), 2)
					End If
					If .IsFeatureAvailable("パイロット画像") Then
						Bitmap = .FeatureData("パイロット画像")
					End If
				End With
			End With
		End Get
	End Property
	
	'ＢＧＭ
	Public ReadOnly Property BGM() As String
		Get
			BGM = Data.BGM
		End Get
	End Property
	
	'メッセージタイプ
	Public ReadOnly Property MessageType() As String
		Get
			MessageType = Name
			
			'パイロット能力「メッセージ」
			If IsSkillAvailable("メッセージ") Then
				MessageType = SkillData("メッセージ")
			End If
			
			'能力コピーで変身した場合はメッセージもコピー元パイロットのものを使う
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .IsConditionSatisfied("メッセージ") Then
						MessageType = LIndex(.ConditionData("メッセージ"), 2)
					End If
				End With
			End If
		End Get
	End Property
	
	
	'防御力
	Public ReadOnly Property Defense() As Short
		Get
			If IsOptionDefined("防御力成長") Or IsOptionDefined("防御力レベルアップ") Then
				Defense = 100 + 5 * SkillLevel("耐久")
				If IsOptionDefined("防御力低成長") Then
					Defense = Defense + (Level * (1 + 2 * SkillLevel("防御成長"))) \ 2
				Else
					Defense = Defense + Int(Level * (1 + SkillLevel("防御成長")))
				End If
			Else
				Defense = 100 + 5 * SkillLevel("耐久")
			End If
		End Get
	End Property
	
	
	' === レベル＆経験値関連処理 ===
	
	'レベル
	
	Public Property Level() As Short
		Get
			Level = proLevel
		End Get
		Set(ByVal Value As Short)
			If proLevel = Value Then
				'変化なし
				Exit Property
			End If
			
			proLevel = Value
			Update()
		End Set
	End Property
	
	'経験値
	
	Public Property Exp() As Integer
		Get
			Exp = proEXP
		End Get
		Set(ByVal Value As Integer)
			Dim prev_level As Short
			
			prev_level = proLevel
			
			'500ごとにレベルアップ
			proEXP = Value Mod 500
			proLevel = proLevel + Value \ 500
			
			'経験値が下がる場合はレベルを下げる
			If proEXP < 0 Then
				If proLevel > 1 Then
					proEXP = proEXP + 500
					proLevel = proLevel - 1
				Else
					'これ以上はレベルを下げられないので
					proEXP = 0
				End If
			End If
			
			'レベル上限チェック
			If Value \ 500 > 0 Then
				If IsOptionDefined("レベル限界突破") Then
					If proLevel > 999 Then 'レベル999で打ち止め
						proLevel = 999
						proEXP = 500
					End If
				Else
					If proLevel > 99 Then 'レベル99で打ち止め
						proLevel = 99
						proEXP = 500
					End If
				End If
			End If
			
			If prev_level <> proLevel Then
				Update()
			End If
		End Set
	End Property
	
	
	'気力
	
	Public Property Morale() As Short
		Get
			Morale = proMorale
		End Get
		Set(ByVal Value As Short)
			SetMorale(Value)
		End Set
	End Property
	
	Public ReadOnly Property MaxMorale() As Short
		Get
			MaxMorale = 150
			If IsSkillAvailable("気力上限") Then
				If IsSkillLevelSpecified("気力上限") Then
					MaxMorale = MaxLng(SkillLevel("気力上限"), 0)
				End If
			End If
		End Get
	End Property
	
	Public ReadOnly Property MinMorale() As Short
		Get
			MinMorale = 50
			If IsSkillAvailable("気力下限") Then
				If IsSkillLevelSpecified("気力下限") Then
					MinMorale = MaxLng(SkillLevel("気力下限"), 0)
				End If
			End If
		End Get
	End Property
	
	
	' === ＳＰ値関連処理 ===
	
	'最大ＳＰ
	Public ReadOnly Property MaxSP() As Integer
		Get
			Dim lv As Short
			
			'ＳＰなしの場合はレベルに関わらず0
			If Data.SP <= 0 Then
				MaxSP = 0
				'ただし追加パイロットの場合は第１パイロットの最大ＳＰを使用
				If Not Unit_Renamed Is Nothing Then
					With Unit_Renamed
						If .CountPilot > 0 Then
							If Not .Pilot(1) Is Me Then
								If .MainPilot Is Me Then
									MaxSP = .Pilot(1).MaxSP
								End If
							End If
						End If
					End With
				End If
				Exit Property
			End If
			
			'レベルによる上昇値
			lv = Level
			If lv > 99 Then
				lv = 100
			End If
			lv = lv + SkillLevel("追加レベル")
			If lv > 40 Then
				MaxSP = lv + 40
			Else
				MaxSP = 2 * lv
			End If
			If IsSkillAvailable("ＳＰ低成長") Then
				MaxSP = MaxSP \ 2
			ElseIf IsSkillAvailable("ＳＰ高成長") Then 
				MaxSP = 1.5 * MaxSP
			End If
			If IsOptionDefined("ＳＰ低成長") Then
				MaxSP = MaxSP \ 2
			End If
			
			'基本値を追加
			MaxSP = MaxSP + Data.SP
			
			'能力ＵＰ
			MaxSP = MaxSP + SkillLevel("ＳＰＵＰ")
			
			'能力ＤＯＷＮ
			MaxSP = MaxSP - SkillLevel("ＳＰＤＯＷＮ")
			
			'上限を超えないように
			If MaxSP > 9999 Then
				MaxSP = 9999
			End If
		End Get
	End Property
	
	'ＳＰ値
	
	Public Property SP() As Integer
		Get
			SP = proSP
			
			'追加パイロットかどうか判定
			
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				If .CountPilot = 0 Then
					Exit Property
				End If
				
				If .Pilot(1) Is Me Then
					Exit Property
				End If
				If Not .MainPilot Is Me Then
					Exit Property
				End If
				
				'追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
				If Data.SP > 0 Then
					'ＳＰを持つ場合は消費量を一致させる
					With .Pilot(1)
						If .MaxSP > 0 Then
							proSP = MaxSP * .SP0 \ .MaxSP
							SP = proSP
						End If
					End With
				Else
					'ＳＰを持たない場合はそのまま使う
					SP = .Pilot(1).SP0
				End If
			End With
		End Get
		Set(ByVal Value As Integer)
			Dim prev_sp As Integer
			
			prev_sp = proSP
			If Value > MaxSP Then
				proSP = MaxSP
			ElseIf Value < 0 Then 
				proSP = 0
			Else
				proSP = Value
			End If
			
			'追加パイロットかどうか判定
			
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				If .CountPilot = 0 Then
					Exit Property
				End If
				
				If .Pilot(1) Is Me Then
					Exit Property
				End If
				If Not .MainPilot Is Me Then
					Exit Property
				End If
				
				'追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
				With .Pilot(1)
					If Data.SP > 0 Then
						'追加パイロットがＳＰを持つ場合は第１パイロットと消費率を一致させる
						If .MaxSP > 0 Then
							.SP0 = .MaxSP * proSP \ MaxSP
							proSP = MaxSP * .SP0 \ .MaxSP
						End If
					Else
						'追加パイロットがＳＰを持たない場合は第１パイロットのＳＰ値をそのまま使う
						If Value > .MaxSP Then
							.SP0 = .MaxSP
						ElseIf Value < 0 Then 
							.SP0 = 0
						Else
							.SP0 = Value
						End If
					End If
				End With
			End With
		End Set
	End Property
	
	
	Public Property SP0() As Integer
		Get
			SP0 = proSP
		End Get
		Set(ByVal Value As Integer)
			proSP = Value
		End Set
	End Property
	
	'霊力
	
	Public Property Plana() As Integer
		Get
			If IsSkillAvailable("霊力") Then
				Plana = proPlana
			End If
			
			'追加パイロットかどうか判定
			
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				If .CountPilot = 0 Then
					Exit Property
				End If
				
				If .Pilot(1) Is Me Then
					Exit Property
				End If
				If Not .MainPilot Is Me Then
					Exit Property
				End If
				
				'追加パイロットだったので第１パイロットの霊力を代わりに使う
				If IsSkillAvailable("霊力") Then
					With .Pilot(1)
						If .MaxPlana > 0 Then
							proPlana = MaxPlana * .Plana0 \ .MaxPlana
							Plana = proPlana
						End If
					End With
				Else
					Plana = .Pilot(1).Plana0
				End If
			End With
		End Get
		Set(ByVal Value As Integer)
			Dim prev_plana As Integer
			
			prev_plana = proPlana
			If Value > MaxPlana Then
				proPlana = MaxPlana
			ElseIf Value < 0 Then 
				proPlana = 0
			Else
				proPlana = Value
			End If
			
			'追加パイロットかどうか判定
			
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				If .CountPilot = 0 Then
					Exit Property
				End If
				
				If .Pilot(1) Is Me Then
					Exit Property
				End If
				If Not .MainPilot Is Me Then
					Exit Property
				End If
				
				'追加パイロットだったので第１パイロットの霊力値を代わりに使う
				With .Pilot(1)
					If IsSkillAvailable("霊力") Then
						'追加パイロットが霊力を持つ場合は第１パイロットと消費率を一致させる
						If .MaxSP > 0 Then
							.Plana0 = .MaxPlana * proPlana \ MaxPlana
							proPlana = MaxPlana * .Plana0 \ .MaxPlana
						End If
					Else
						'追加パイロットが霊力を持たない場合は第１パイロットの霊力値をそのまま使う
						If Value > .MaxPlana Then
							.Plana0 = .MaxPlana
						ElseIf Value < 0 Then 
							.Plana0 = 0
						Else
							.Plana0 = Value
						End If
					End If
				End With
			End With
		End Set
	End Property
	
	
	Public Property Plana0() As Integer
		Get
			Plana0 = proPlana
		End Get
		Set(ByVal Value As Integer)
			proPlana = Value
		End Set
	End Property
	
	
	' === スペシャルパワー関連処理 ===
	
	'スペシャルパワーの個数
	Public ReadOnly Property CountSpecialPower() As Short
		Get
			If Data.SP <= 0 Then
				'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
				If Not Unit_Renamed Is Nothing Then
					With Unit_Renamed
						If .CountPilot > 0 Then
							If Not .Pilot(1) Is Me Then
								If .MainPilot Is Me Then
									CountSpecialPower = .Pilot(1).Data.CountSpecialPower(Level)
									Exit Property
								End If
							End If
						End If
					End With
				End If
			End If
			
			CountSpecialPower = Data.CountSpecialPower(Level)
		End Get
	End Property
	
	'idx番目のスペシャルパワー
	Public ReadOnly Property SpecialPower(ByVal idx As Short) As String
		Get
			If Data.SP <= 0 Then
				'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
				If Not Unit_Renamed Is Nothing Then
					With Unit_Renamed
						If .CountPilot > 0 Then
							If Not .Pilot(1) Is Me Then
								If .MainPilot Is Me Then
									SpecialPower = .Pilot(1).Data.SpecialPower(Level, idx)
									Exit Property
								End If
							End If
						End If
					End With
				End If
			End If
			
			SpecialPower = Data.SpecialPower(Level, idx)
		End Get
	End Property
	
	
	'能力値を更新
	Public Sub Update()
		Dim skill_num As Short
		Dim skill_name(64) As String
		Dim skill_data(64) As SkillData
		Dim i, j As Short
		Dim lv As Double
		Dim sd As SkillData
		
		'現在のレベルで使用可能な特殊能力の一覧を作成
		
		'以前の一覧を削除
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'パイロットデータを参照しながら使用可能な特殊能力を検索
		skill_num = 0
		For	Each sd In Data.colSkill
			With sd
				If Level >= .NecessaryLevel Then
					'既に登録済み？
					If .Name = "ＳＰ消費減少" Or .Name = "スペシャルパワー自動発動" Or .Name = "ハンター" Then
						'これらの特殊能力は同種の能力を複数持つことが出来る
						For i = 1 To skill_num
							If .Name = skill_name(i) Then
								If .StrData = skill_data(i).StrData Then
									'ただしデータ指定まで同一であれば同じ能力と見なす
									Exit For
								End If
							End If
						Next 
					Else
						For i = 1 To skill_num
							If .Name = skill_name(i) Then
								Exit For
							End If
						Next 
					End If
					
					If i > skill_num Then
						'未登録
						skill_num = skill_num + 1
						skill_name(skill_num) = .Name
						skill_data(skill_num) = sd
					ElseIf .NecessaryLevel > skill_data(i).NecessaryLevel Then 
						'登録済みである場合は習得レベルが高いものを優先
						skill_data(i) = sd
					End If
				End If
			End With
		Next sd
		
		'SetSkillコマンドで付加された特殊能力を検索
		Dim sname, alist, sdata As String
		Dim buf As String
		If IsGlobalVariableDefined("Ability(" & ID & ")") Then
			
			'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			alist = GlobalVariableList.Item("Ability(" & ID & ")").StringValue
			For i = 1 To LLength(alist)
				sname = LIndex(alist, i)
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				buf = GlobalVariableList.Item("Ability(" & ID & "," & sname & ")").StringValue
				sdata = ListTail(buf, 2)
				
				'既に登録済み？
				If sname = "ＳＰ消費減少" Or sname = "スペシャルパワー自動発動" Or sname = "ハンター" Then
					'これらの特殊能力は同種の能力を複数持つことが出来る
					For j = 1 To skill_num
						If sname = skill_name(j) Then
							If sdata = skill_data(j).StrData Then
								'ただしデータ指定まで同一であれば同じ能力と見なす
								Exit For
							End If
						End If
					Next 
				Else
					For j = 1 To skill_num
						If sname = skill_name(j) Then
							Exit For
						End If
					Next 
				End If
				
				If j > skill_num Then
					'未登録
					skill_num = j
					skill_name(j) = sname
				End If
				
				If StrToDbl(LIndex(buf, 1)) = 0 Then
					'レベル0の場合は能力を封印
					'UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					skill_data(j) = Nothing
				Else
					'PDListのデータを書き換えるわけにはいかないので
					'アビリティデータを新規に作成
					sd = New SkillData
					With sd
						.Name = sname
						.Level = StrToDbl(LIndex(buf, 1))
						If .Level = -1 Then
							.Level = DEFAULT_LEVEL
						End If
						.StrData = ListTail(buf, 2)
					End With
					skill_data(j) = sd
				End If
			Next 
		End If
		
		'属性使用不能状態の際、対応する技能を封印する。
		If Not Unit_Renamed Is Nothing Then
			For j = 1 To skill_num
				If Not skill_data(j) Is Nothing Then
					If Unit_Renamed.ConditionLifetime(skill_data(j).Name & "使用不能") > 0 Then
						'UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						skill_data(j) = Nothing
					End If
				End If
			Next 
		End If
		
		'使用可能な特殊能力を登録
		With colSkill
			For i = 1 To skill_num
				If Not skill_data(i) Is Nothing Then
					Select Case skill_name(i)
						Case "ＳＰ消費減少", "スペシャルパワー自動発動", "ハンター"
							For j = 1 To i - 1
								If skill_name(i) = skill_name(j) Then
									Exit For
								End If
							Next 
							If j >= i Then
								.Add(skill_data(i), skill_name(i))
							Else
								.Add(skill_data(i), skill_name(i) & ":" & VB6.Format(i))
							End If
						Case Else
							.Add(skill_data(i), skill_name(i))
					End Select
				End If
			Next 
		End With
		
		'これから下は能力値の計算
		
		'基本値
		With Data
			InfightBase = .Infight
			ShootingBase = .Shooting
			HitBase = .Hit
			DodgeBase = .Dodge
			TechniqueBase = .Technique
			IntuitionBase = .Intuition
			Adaption = .Adaption
		End With
		
		'レベルによる追加分
		lv = Level + SkillLevel("追加レベル")
		If IsOptionDefined("攻撃力低成長") Then
			InfightBase = InfightBase + (lv * (1 + 2 * SkillLevel("格闘成長"))) \ 2
			ShootingBase = ShootingBase + (lv * (1 + 2 * SkillLevel("射撃成長"))) \ 2
		Else
			InfightBase = InfightBase + Int(lv * (1 + SkillLevel("格闘成長")))
			ShootingBase = ShootingBase + Int(lv * (1 + SkillLevel("射撃成長")))
		End If
		HitBase = HitBase + Int(lv * (2 + SkillLevel("命中成長")))
		DodgeBase = DodgeBase + Int(lv * (2 + SkillLevel("回避成長")))
		TechniqueBase = TechniqueBase + Int(lv * (1 + SkillLevel("技量成長")))
		IntuitionBase = IntuitionBase + Int(lv * (1 + SkillLevel("反応成長")))
		
		'能力ＵＰ
		InfightBase = InfightBase + SkillLevel("格闘ＵＰ")
		ShootingBase = ShootingBase + SkillLevel("射撃ＵＰ")
		HitBase = HitBase + SkillLevel("命中ＵＰ")
		DodgeBase = DodgeBase + SkillLevel("回避ＵＰ")
		TechniqueBase = TechniqueBase + SkillLevel("技量ＵＰ")
		IntuitionBase = IntuitionBase + SkillLevel("反応ＵＰ")
		
		'能力ＤＯＷＮ
		InfightBase = InfightBase - SkillLevel("格闘ＤＯＷＮ")
		ShootingBase = ShootingBase - SkillLevel("射撃ＤＯＷＮ")
		HitBase = HitBase - SkillLevel("命中ＤＯＷＮ")
		DodgeBase = DodgeBase - SkillLevel("回避ＤＯＷＮ")
		TechniqueBase = TechniqueBase - SkillLevel("技量ＤＯＷＮ")
		IntuitionBase = IntuitionBase - SkillLevel("反応ＤＯＷＮ")
		
		'上限を超えないように
		InfightBase = MinLng(InfightBase, 9999)
		ShootingBase = MinLng(ShootingBase, 9999)
		HitBase = MinLng(HitBase, 9999)
		DodgeBase = MinLng(DodgeBase, 9999)
		TechniqueBase = MinLng(TechniqueBase, 9999)
		IntuitionBase = MinLng(IntuitionBase, 9999)
		
		'これから下は特殊能力による修正値の計算
		
		'まずは修正値を初期化
		InfightMod = 0
		ShootingMod = 0
		HitMod = 0
		DodgeMod = 0
		TechniqueMod = 0
		IntuitionMod = 0
		
		'パイロット用特殊能力による修正
		
		lv = SkillLevel("超感覚")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("知覚強化")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("念力")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("超反応")
		HitMod = HitMod + 2 * lv
		DodgeMod = DodgeMod + 2 * lv
		
		If IsSkillAvailable("サイボーグ") Then
			HitMod = HitMod + 5
			DodgeMod = DodgeMod + 5
		End If
		If IsSkillAvailable("悟り") Then
			HitMod = HitMod + 10
			DodgeMod = DodgeMod + 10
		End If
		If IsSkillAvailable("超能力") Then
			HitMod = HitMod + 5
			DodgeMod = DodgeMod + 5
		End If
		
		'これから下はユニットによる修正値の計算
		
		'ユニットに乗っていない？
		If Unit_Renamed Is Nothing Then
			GoTo SkipUnitMod
		End If
		
		Dim padaption(4) As Short
		With Unit_Renamed
			'クイックセーブ処理などで実際には乗っていない場合
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			'サブパイロット＆サポートパイロットによるサポート
			If Me Is .MainPilot And .Status = "出撃" Then
				For i = 2 To .CountPilot
					With .Pilot(i)
						InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
						End If
						HitMod = HitMod + 3 * .SkillLevel("サポート")
						HitMod = HitMod + 2 * .SkillLevel("命中サポート")
						DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
						DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
					End With
				Next 
				For i = 1 To .CountSupport
					With .Support(i)
						InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
						End If
						HitMod = HitMod + 3 * .SkillLevel("サポート")
						HitMod = HitMod + 2 * .SkillLevel("命中サポート")
						DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
						DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
					End With
				Next 
				If .IsFeatureAvailable("追加サポート") Then
					With .AdditionalSupport
						InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
						End If
						HitMod = HitMod + 3 * .SkillLevel("サポート")
						HitMod = HitMod + 2 * .SkillLevel("命中サポート")
						DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
						DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
					End With
				End If
			End If
			
			'ユニット＆アイテムによる強化
			For i = 1 To .CountFeature
				Select Case .Feature(i)
					Case "格闘強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							InfightMod = InfightMod + 5 * .FeatureLevel(i)
						End If
					Case "射撃強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							ShootingMod = ShootingMod + 5 * .FeatureLevel(i)
						End If
					Case "命中強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							HitMod = HitMod + 5 * .FeatureLevel(i)
						End If
					Case "回避強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							DodgeMod = DodgeMod + 5 * .FeatureLevel(i)
						End If
					Case "技量強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							TechniqueMod = TechniqueMod + 5 * .FeatureLevel(i)
						End If
					Case "反応強化"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							IntuitionMod = IntuitionMod + 5 * .FeatureLevel(i)
						End If
				End Select
			Next 
			
			'地形適応変更
			If .IsFeatureAvailable("パイロット地形適応変更") Then
				
				For i = 1 To 4
					Select Case Mid(Adaption, i, 1)
						Case "S"
							padaption(i) = 5
						Case "A"
							padaption(i) = 4
						Case "B"
							padaption(i) = 3
						Case "C"
							padaption(i) = 2
						Case "D"
							padaption(i) = 1
						Case "E", "-"
							padaption(i) = 0
					End Select
				Next 
				
				'地形適応変更能力による修正
				For i = 1 To .CountFeature
					If .Feature(i) = "パイロット地形適応変更" Then
						For j = 1 To 4
							If StrToLng(LIndex(.FeatureData(i), j)) >= 0 Then
								'修正値がプラスのとき
								If padaption(j) < 4 Then
									padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
									'地形適応はAより高くはならない
									If padaption(j) > 4 Then
										padaption(j) = 4
									End If
								End If
							Else
								'修正値がマイナスのときは本来の地形適応が"A"以上でも処理を行なう
								padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
							End If
						Next 
					End If
				Next 
				
				Adaption = ""
				For i = 1 To 4
					Select Case padaption(i)
						Case Is >= 5
							Adaption = Adaption & "S"
						Case 4
							Adaption = Adaption & "A"
						Case 3
							Adaption = Adaption & "B"
						Case 2
							Adaption = Adaption & "C"
						Case 1
							Adaption = Adaption & "D"
						Case Is <= 0
							Adaption = Adaption & "-"
					End Select
				Next 
			End If
		End With
		
		'気力の値を気力上限・気力下限の範囲にする
		SetMorale(Morale)
		
SkipUnitMod: 
		
		'基本値と修正値の合計から実際の能力値を算出
		Infight = MinLng(InfightBase + InfightMod + InfightMod2, 9999)
		Shooting = MinLng(ShootingBase + ShootingMod + ShootingMod2, 9999)
		Hit = MinLng(HitBase + HitMod + HitMod2, 9999)
		Dodge = MinLng(DodgeBase + DodgeMod + DodgeMod2, 9999)
		Technique = MinLng(TechniqueBase + TechniqueMod + TechniqueMod2, 9999)
		Intuition = MinLng(IntuitionBase + IntuitionMod + IntuitionMod2, 9999)
	End Sub
	
	'周りのユニットによる支援効果を更新
	Public Sub UpdateSupportMod()
		Dim u, my_unit As Unit
		Dim my_party As String
		Dim my_cmd_rank As Short
		Dim cmd_rank, cmd_rank2 As Short
		Dim cmd_level As Double
		Dim cs_level As Double
		Dim range, max_range As Short
		Dim mod_stack As Boolean
		Dim rel_lv As Short
		Dim team, uteam As String
		Dim j, i, k As Short
		
		'支援効果による修正値を初期化
		
		Infight = InfightBase + InfightMod
		Shooting = ShootingBase + ShootingMod
		Hit = HitBase + HitMod
		Dodge = DodgeBase + DodgeMod
		Technique = TechniqueBase + TechniqueMod
		Intuition = IntuitionBase + IntuitionMod
		
		InfightMod2 = 0
		ShootingMod2 = 0
		HitMod2 = 0
		DodgeMod2 = 0
		TechniqueMod2 = 0
		IntuitionMod2 = 0
		
		MoraleMod = 0
		
		'ステータス表示時には支援効果を無視
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'ユニットに乗っていなければここで終了
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		'一旦乗っているユニットを記録しておく
		my_unit = Unit_Renamed
		
		With Unit_Renamed
			'ユニットが出撃していなければここで終了
			If .Status <> "出撃" Then
				Exit Sub
			End If
			If Not Unit_Renamed Is MapDataForUnit(.X, .Y) Then
				Exit Sub
			End If
			
			'メインパイロットでなければここで終了
			If .CountPilot = 0 Then
				Exit Sub
			End If
			If Not Me Is .MainPilot Then
				Exit Sub
			End If
			
			'正常な判断が出来ないユニットは支援を受けられない
			If .IsConditionSatisfied("暴走") Then
				Exit Sub
			End If
			If .IsConditionSatisfied("混乱") Then
				Exit Sub
			End If
			
			'支援を受けられるかどうかの判定用に陣営を参照しておく
			my_party = .Party
			
			'指揮効果判定用に自分の階級レベルを算出
			If IsSkillAvailable("階級") Then
				my_cmd_rank = SkillLevel("階級")
				cmd_rank = my_cmd_rank
			Else
				If InStr(Name, "(ザコ)") = 0 And InStr(Name, "(汎用)") = 0 Then
					my_cmd_rank = DEFAULT_LEVEL
				Else
					my_cmd_rank = 0
				End If
				cmd_rank = 0
			End If
			
			'自分が所属しているチーム名
			team = SkillData("チーム")
			
			'周りのユニットを調べる
			cs_level = DEFAULT_LEVEL
			max_range = 5
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'ユニット間の距離が範囲内？
					range = System.Math.Abs(.X - i) + System.Math.Abs(.Y - j)
					If range > max_range Then
						GoTo NextUnit
					End If
					
					u = MapDataForUnit(i, j)
					
					If u Is Nothing Then
						GoTo NextUnit
					End If
					If u Is Unit_Renamed Then
						GoTo NextUnit
					End If
					
					With u
						'ユニットにパイロットが乗っていなければ無視
						If .CountPilot = 0 Then
							GoTo NextUnit
						End If
						
						'陣営が一致していないと支援は受けられない
						Select Case my_party
							Case "味方", "ＮＰＣ"
								Select Case .Party
									Case "敵", "中立"
										GoTo NextUnit
								End Select
							Case Else
								If my_party <> .Party Then
									GoTo NextUnit
								End If
						End Select
						
						'相手が正常な判断能力を持っていない場合も支援は受けられない
						If .IsConditionSatisfied("暴走") Then
							GoTo NextUnit
						End If
						If .IsConditionSatisfied("混乱") Then
							GoTo NextUnit
						End If
					End With
					
					With u.MainPilot(True)
						'同じチームに所属している？
						uteam = .SkillData("チーム")
						If team <> uteam And uteam <> "" Then
							GoTo NextUnit
						End If
						
						'広域サポート
						If range <= 2 Then
							cs_level = MaxDbl(cs_level, .SkillLevel("広域サポート"))
						End If
						
						'指揮効果
						If my_cmd_rank >= 0 Then
							If range > .CommandRange Then
								GoTo NextUnit
							End If
							
							cmd_rank2 = .SkillLevel("階級")
							If cmd_rank2 > cmd_rank Then
								cmd_rank = cmd_rank2
								cmd_level = .SkillLevel("指揮")
							ElseIf cmd_rank2 = cmd_rank Then 
								cmd_level = MaxDbl(cmd_level, .SkillLevel("指揮"))
							End If
						End If
					End With
NextUnit: 
				Next 
			Next 
			
			'追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
			'変化してしまった場合は元に戻しておく
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'広域サポートによる修正
			If cs_level <> DEFAULT_LEVEL Then
				HitMod2 = HitMod2 + 5 * cs_level
				DodgeMod2 = DodgeMod2 + 5 * cs_level
			End If
			
			'指揮能力による修正
			Select Case my_cmd_rank
				Case DEFAULT_LEVEL
					'修正なし
				Case 0
					HitMod2 = HitMod2 + 5 * cmd_level
					DodgeMod2 = DodgeMod2 + 5 * cmd_level
				Case Else
					'自分が階級レベルを持っている場合はより高い階級レベルを
					'持つパイロットの指揮効果のみを受ける
					If cmd_rank > my_cmd_rank Then
						HitMod2 = HitMod2 + 5 * cmd_level
						DodgeMod2 = DodgeMod2 + 5 * cmd_level
					End If
			End Select
			
			'支援効果による修正を能力値に加算
			Infight = Infight + InfightMod2
			Shooting = Shooting + ShootingMod2
			Hit = Hit + HitMod2
			Dodge = Dodge + DodgeMod2
			Technique = Technique + TechniqueMod2
			Intuition = Intuition + IntuitionMod2
			
			'信頼補正
			If Not IsOptionDefined("信頼補正") Then
				Exit Sub
			End If
			If InStr(Name, "(ザコ)") > 0 Then
				Exit Sub
			End If
			
			'信頼補正が重複する？
			mod_stack = IsOptionDefined("信頼補正重複")
			
			'同じユニットに乗っているサポートパイロットからの補正
			If mod_stack Then
				For i = 1 To .CountSupport
					rel_lv = rel_lv + Relation(.Support(i))
				Next 
				If .IsFeatureAvailable("追加サポート") Then
					rel_lv = rel_lv + Relation(.AdditionalSupport)
				End If
			Else
				For i = 1 To .CountSupport
					rel_lv = MaxLng(Relation(.Support(i)), rel_lv)
				Next 
				If .IsFeatureAvailable("追加サポート") Then
					rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
				End If
			End If
			
			'周囲のユニットからの補正
			If IsOptionDefined("信頼補正範囲拡大") Then
				max_range = 2
			Else
				max_range = 1
			End If
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'ユニット間の距離が範囲内？
					range = System.Math.Abs(.X - i) + System.Math.Abs(.Y - j)
					If range > max_range Then
						GoTo NextUnit2
					End If
					
					u = MapDataForUnit(i, j)
					
					If u Is Nothing Then
						GoTo NextUnit2
					End If
					If u Is Unit_Renamed Then
						GoTo NextUnit2
					End If
					
					With u
						'ユニットにパイロットが乗っていなければ無視
						If .CountPilot = 0 Then
							GoTo NextUnit2
						End If
						
						'味方かどうか判定
						Select Case my_party
							Case "味方", "ＮＰＣ"
								Select Case .Party
									Case "敵", "中立"
										GoTo NextUnit2
								End Select
							Case Else
								If my_party <> .Party Then
									GoTo NextUnit2
								End If
						End Select
						
						If mod_stack Then
							rel_lv = rel_lv + Relation(.MainPilot(True))
							For k = 2 To .CountPilot
								rel_lv = rel_lv + Relation(.Pilot(k))
							Next 
							For k = 1 To .CountSupport
								rel_lv = rel_lv + Relation(.Support(k))
							Next 
							If .IsFeatureAvailable("追加サポート") Then
								rel_lv = rel_lv + Relation(.AdditionalSupport)
							End If
						Else
							rel_lv = MaxLng(Relation(.MainPilot(True)), rel_lv)
							For k = 2 To .CountPilot
								rel_lv = MaxLng(Relation(.Pilot(k)), rel_lv)
							Next 
							For k = 1 To .CountSupport
								rel_lv = MaxLng(Relation(.Support(k)), rel_lv)
							Next 
							If .IsFeatureAvailable("追加サポート") Then
								rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
							End If
						End If
					End With
NextUnit2: 
				Next 
			Next 
			
			'追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
			'変化してしまった場合は元に戻しておく
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'信頼補正を設定
			Select Case rel_lv
				Case 1
					MoraleMod = MoraleMod + 5
				Case 2
					MoraleMod = MoraleMod + 8
				Case Is > 2
					MoraleMod = MoraleMod + 2 * rel_lv + 4
			End Select
		End With
	End Sub
	
	Private Sub SetMorale(ByVal new_morale As Short)
		Dim maxm As Short
		Dim minm As Short
		
		maxm = MaxMorale
		minm = MinMorale
		
		If new_morale > maxm Then
			proMorale = maxm
		ElseIf new_morale < minm Then 
			proMorale = minm
		Else
			proMorale = new_morale
		End If
	End Sub
	
	
	' === 霊力関連処理 ===
	
	'霊力最大値
	Public Function MaxPlana() As Integer
		Dim lv As Short
		
		If Not IsSkillAvailable("霊力") Then
			'霊力能力を持たない場合
			MaxPlana = 0
			
			'追加パイロットの場合は第１パイロットの霊力を代わりに使う
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .CountPilot > 0 Then
						If Not .Pilot(1) Is Me Then
							If .MainPilot Is Me Then
								MaxPlana = .Pilot(1).MaxPlana
							End If
						End If
					End If
				End With
			End If
			
			Exit Function
		End If
		
		'霊力基本値
		MaxPlana = SkillLevel("霊力")
		
		'レベルによる増加分
		lv = MinLng(Level, 100)
		If IsSkillAvailable("霊力成長") Then
			MaxPlana = MaxPlana + 1.5 * lv * (10 + SkillLevel("霊力成長")) \ 10
		Else
			MaxPlana = MaxPlana + 1.5 * lv
		End If
	End Function
	
	
	' === 特殊能力関連処理 ===
	
	'特殊能力の総数
	Public Function CountSkill() As Short
		CountSkill = colSkill.Count()
	End Function
	
	'特殊能力
	Public Function Skill(ByRef Index As Object) As String
		Dim sd As SkillData
		
		sd = colSkill.Item(Index)
		Skill = sd.Name
	End Function
	
	'現在のレベルにおいて特殊能力 sname が使用可能か
	Public Function IsSkillAvailable(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable = True
		Exit Function
		
ErrorHandler: 
		
		'特殊能力付加＆強化による修正
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition = 0 Then
					Exit Function
				End If
				
				If .CountPilot = 0 Then
					Exit Function
				End If
				If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
					Exit Function
				End If
				
				If .IsConditionSatisfied(sname & "付加") Then
					IsSkillAvailable = True
					Exit Function
				ElseIf .IsConditionSatisfied(sname & "付加２") Then 
					IsSkillAvailable = True
					Exit Function
				End If
				
				If .IsConditionSatisfied(sname & "強化") Then
					If .ConditionLevel(sname & "強化") > 0 Then
						IsSkillAvailable = True
						Exit Function
					End If
				End If
				If .IsConditionSatisfied(sname & "強化２") Then
					If .ConditionLevel(sname & "強化２") > 0 Then
						IsSkillAvailable = True
						Exit Function
					End If
				End If
			End With
		End If
		
		IsSkillAvailable = False
	End Function
	
	'現在のレベルにおいて特殊能力 sname が使用可能か
	'(付加による影響を無視した場合)
	Public Function IsSkillAvailable2(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable2 = True
		Exit Function
		
ErrorHandler: 
		IsSkillAvailable2 = False
	End Function
	
	'現在のレベルにおける特殊能力 Index のレベル
	'データでレベル指定がない場合はレベル 1
	'特殊能力が使用不能の場合はレベル 0
	Public Function SkillLevel(ByRef Index As Object, Optional ByRef ref_mode As String = "") As Double
		Dim sname As String
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			SkillLevel = .Level
		End With
		If SkillLevel = DEFAULT_LEVEL Then
			SkillLevel = 1
		End If
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				sname = CStr(Index)
			End If
		End If
		
		If ref_mode = "修正値" Then
			SkillLevel = 0
		ElseIf ref_mode = "基本値" Then 
			Exit Function
		End If
		
		'重複可能な能力は特殊能力付加で置き換えられことはない
		Select Case sname
			Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'特殊能力付加＆強化による修正
		If Unit_Renamed Is Nothing Then
			Exit Function
		End If
		With Unit_Renamed
			If .CountCondition = 0 Then
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				Exit Function
			End If
			If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
				Exit Function
			End If
			
			If .IsConditionSatisfied(sname & "付加") Then
				SkillLevel = .ConditionLevel(sname & "付加")
				If SkillLevel = DEFAULT_LEVEL Then
					SkillLevel = 1
				End If
			ElseIf .IsConditionSatisfied(sname & "付加２") Then 
				SkillLevel = .ConditionLevel(sname & "付加２")
				If SkillLevel = DEFAULT_LEVEL Then
					SkillLevel = 1
				End If
			End If
			
			If .IsConditionSatisfied(sname & "強化") Then
				SkillLevel = SkillLevel + .ConditionLevel(sname & "強化")
			End If
			If .IsConditionSatisfied(sname & "強化２") Then
				SkillLevel = SkillLevel + .ConditionLevel(sname & "強化２")
			End If
		End With
	End Function
	
	'特殊能力 Index にレベル指定がなされているか判定
	Public Function IsSkillLevelSpecified(ByRef Index As Object) As Boolean
		Dim sname As String
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			If .Level <> DEFAULT_LEVEL Then
				IsSkillLevelSpecified = True
				sname = .Name
			End If
		End With
		
		Exit Function
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				sname = CStr(Index)
			End If
		End If
		
		'特殊能力付加＆強化による修正
		If Unit_Renamed Is Nothing Then
			Exit Function
		End If
		With Unit_Renamed
			If .CountCondition = 0 Then
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				Exit Function
			End If
			If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
				Exit Function
			End If
			
			If .IsConditionSatisfied(sname & "付加") Then
				If .ConditionLevel(sname & "付加") <> DEFAULT_LEVEL Then
					IsSkillLevelSpecified = True
				End If
			ElseIf .IsConditionSatisfied(sname & "付加２") Then 
				If .ConditionLevel(sname & "付加２") <> DEFAULT_LEVEL Then
					IsSkillLevelSpecified = True
				End If
			End If
			If .IsConditionSatisfied(sname & "強化") Then
				IsSkillLevelSpecified = True
			ElseIf .IsConditionSatisfied(sname & "強化２") Then 
				IsSkillLevelSpecified = True
			End If
		End With
	End Function
	
	'特殊能力のデータ
	Public Function SkillData(ByRef Index As Object) As String
		Dim sname As String
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			SkillData = .StrData
		End With
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				sname = CStr(Index)
			End If
		End If
		
		'重複可能な能力は特殊能力付加で置き換えられことはない
		Select Case sname
			Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'特殊能力付加＆強化による修正
		If Unit_Renamed Is Nothing Then
			Exit Function
		End If
		With Unit_Renamed
			If .CountCondition = 0 Then
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				Exit Function
			End If
			If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
				Exit Function
			End If
			
			If .IsConditionSatisfied(sname & "付加") Then
				SkillData = .ConditionData(sname & "付加")
			ElseIf .IsConditionSatisfied(sname & "付加２") Then 
				SkillData = .ConditionData(sname & "付加２")
			End If
			
			If .IsConditionSatisfied(sname & "強化") Then
				If Len(.ConditionData(sname & "強化")) > 0 Then
					SkillData = .ConditionData(sname & "強化")
				End If
			End If
			If .IsConditionSatisfied(sname & "強化２") Then
				If Len(.ConditionData(sname & "強化２")) > 0 Then
					SkillData = .ConditionData(sname & "強化２")
				End If
			End If
		End With
	End Function
	
	'特殊能力の名称
	Public Function SkillName(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'パイロットが所有している特殊能力の中から検索
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'能力強化系は非表示
			If Right(sname, 2) = "ＵＰ" Or Right(sname, 4) = "ＤＯＷＮ" Then
				SkillName = "非表示"
				Exit Function
			End If
			
			Select Case sname
				Case "追加レベル", "メッセージ", "魔力所有"
					'非表示の能力
					SkillName = "非表示"
					Exit Function
				Case "得意技", "不得手"
					'別名指定が存在しない能力
					SkillName = sname
					Exit Function
			End Select
			
			If Len(.StrData) > 0 Then
				SkillName = LIndex(.StrData, 1)
				Select Case SkillName
					Case "非表示"
						Exit Function
					Case "解説"
						SkillName = "非表示"
						Exit Function
				End Select
			Else
				SkillName = sname
			End If
			
			'レベル指定
			If .Level <> DEFAULT_LEVEL And InStr(SkillName, "Lv") = 0 And Left(SkillName, 1) <> "(" Then
				SkillName = SkillName & "Lv" & VB6.Format(.Level)
			End If
		End With
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				sname = CStr(Index)
			End If
		End If
		
		If sname = "耐久" Then
			If IsOptionDefined("防御力成長") Or IsOptionDefined("防御力レベルアップ") Then
				'防御力成長オプション使用時には耐久能力を非表示
				SkillName = "非表示"
				Exit Function
			End If
		End If
		
		'得意技＆不得手は名称変更されない
		Select Case sname
			Case "得意技", "不得手"
				SkillName = sname
				Exit Function
		End Select
		
		'SetSkillコマンドで封印されている場合
		If SkillName = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
				'オリジナルの名称を使用
				SkillName = Data.SkillName(Level, sname)
				
				If InStr(SkillName, "非表示") > 0 Then
					SkillName = "非表示"
					Exit Function
				End If
			End If
		End If
		
		'重複可能な能力は特殊能力付加で名称が置き換えられことはない
		Select Case sname
			Case "ハンター", "スペシャルパワー自動発動"
				If IsNumeric(Index) Then
					If Left(SkillName, 1) = "(" Then
						SkillName = Mid(SkillName, 2)
						SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
					End If
					Exit Function
				End If
			Case "ＳＰ消費減少"
				If IsNumeric(Index) Then
					If Left(SkillName, 1) = "(" Then
						SkillName = Mid(SkillName, 2)
						SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
					End If
					i = InStr(SkillName, "Lv")
					If i > 0 Then
						SkillName = Left(SkillName, i - 1)
					End If
					Exit Function
				End If
		End Select
		
		'特殊能力付加＆強化による修正
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'ユニット用特殊能力による付加
						If .IsConditionSatisfied(sname & "付加２") Then
							buf = LIndex(.ConditionData(sname & "付加２"), 1)
							
							If buf <> "" Then
								SkillName = buf
							ElseIf SkillName = "" Then 
								SkillName = sname
							End If
							
							If InStr(SkillName, "非表示") > 0 Then
								SkillName = "非表示"
								Exit Function
							End If
							
							'レベル指定
							If .ConditionLevel(sname & "付加２") <> DEFAULT_LEVEL Then
								If InStr(SkillName, "Lv") > 0 Then
									SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
								End If
								SkillName = SkillName & "Lv" & VB6.Format(.ConditionLevel(sname & "付加２"))
							End If
						End If
						
						'アビリティによる付加
						If .IsConditionSatisfied(sname & "付加") Then
							buf = LIndex(.ConditionData(sname & "付加"), 1)
							
							If buf <> "" Then
								SkillName = buf
							ElseIf SkillName = "" Then 
								SkillName = sname
							End If
							
							If InStr(SkillName, "非表示") > 0 Then
								SkillName = "非表示"
								Exit Function
							End If
							
							'レベル指定
							If .ConditionLevel(sname & "付加") <> DEFAULT_LEVEL Then
								If InStr(SkillName, "Lv") > 0 Then
									SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
								End If
								SkillName = SkillName & "Lv" & VB6.Format(.ConditionLevel(sname & "付加"))
							End If
						End If
						
						'ユニット用特殊能力による強化
						If .IsConditionSatisfied(sname & "強化２") Then
							If SkillName = "" Then
								'強化される能力をパイロットが持っていなかった場合
								SkillName = LIndex(.ConditionData(sname & "強化２"), 1)
								
								If SkillName = "" Then
									SkillName = sname
								End If
								
								If InStr(SkillName, "非表示") > 0 Then
									SkillName = "非表示"
									Exit Function
								End If
								
								SkillName = SkillName & "Lv0"
							End If
							
							If sname <> "同調率" And sname <> "霊力" Then
								If .ConditionLevel(sname & "強化２") >= 0 Then
									SkillName = SkillName & "+" & VB6.Format(.ConditionLevel(sname & "強化２"))
								Else
									SkillName = SkillName & VB6.Format(.ConditionLevel(sname & "強化２"))
								End If
							End If
						End If
						
						'アビリティによる強化
						If .IsConditionSatisfied(sname & "強化") Then
							If SkillName = "" Then
								'強化される能力をパイロットが持っていなかった場合
								SkillName = LIndex(.ConditionData(sname & "強化"), 1)
								
								If SkillName = "" Then
									SkillName = sname
								End If
								
								If InStr(SkillName, "非表示") > 0 Then
									SkillName = "非表示"
									Exit Function
								End If
								
								SkillName = SkillName & "Lv0"
							End If
							
							If sname <> "同調率" And sname <> "霊力" Then
								If .ConditionLevel(sname & "強化") >= 0 Then
									SkillName = SkillName & "+" & VB6.Format(.ConditionLevel(sname & "強化"))
								Else
									SkillName = SkillName & VB6.Format(.ConditionLevel(sname & "強化"))
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'能力強化系は非表示
		If Right(sname, 2) = "ＵＰ" Or Right(sname, 4) = "ＤＯＷＮ" Then
			SkillName = "非表示"
			Exit Function
		End If
		
		Select Case sname
			Case "追加レベル", "メッセージ", "魔力所有"
				'非表示の能力
				SkillName = "非表示"
				Exit Function
			Case "耐久"
				If IsOptionDefined("防御力成長") Or IsOptionDefined("防御力レベルアップ") Then
					'防御力成長オプション使用時には耐久能力を非表示
					SkillName = "非表示"
					Exit Function
				End If
		End Select
		
		'これらの能力からはレベル指定を除く
		Select Case sname
			Case "階級", "同調率", "霊力", "ＳＰ消費減少"
				i = InStr(SkillName, "Lv")
				If i > 0 Then
					SkillName = Left(SkillName, i - 1)
				End If
		End Select
		
		'レベル非表示用の括弧を削除
		If Left(SkillName, 1) = "(" Then
			SkillName = Mid(SkillName, 2)
			SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
		End If
		
		If SkillName = "" Then
			SkillName = sname
		End If
	End Function
	
	'特殊能力名称（レベル表示抜き）
	Public Function SkillName0(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'パイロットが所有している特殊能力の中から検索
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'能力強化系は非表示
			If Right(sname, 2) = "ＵＰ" Or Right(sname, 4) = "ＤＯＷＮ" Then
				SkillName0 = "非表示"
				Exit Function
			End If
			
			Select Case sname
				Case "追加レベル", "メッセージ", "魔力所有"
					'非表示の能力
					SkillName0 = "非表示"
					Exit Function
				Case "得意技", "不得手"
					'別名指定が存在しない能力
					SkillName0 = sname
					Exit Function
			End Select
			
			If Len(.StrData) > 0 Then
				SkillName0 = LIndex(.StrData, 1)
				
				If SkillName0 = "非表示" Then
					Exit Function
				End If
			Else
				SkillName0 = sname
			End If
		End With
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				sname = CStr(Index)
			End If
		End If
		
		If sname = "耐久" Then
			If IsOptionDefined("防御力成長") Or IsOptionDefined("防御力レベルアップ") Then
				'防御力成長オプション使用時には耐久能力を非表示
				SkillName0 = "非表示"
				Exit Function
			End If
		End If
		
		'得意技＆不得手は名称変更されない
		Select Case sname
			Case "得意技", "不得手"
				SkillName0 = sname
				Exit Function
		End Select
		
		'SetSkillコマンドで封印されている場合
		If SkillName0 = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
				'オリジナルの名称を使用
				SkillName0 = Data.SkillName(Level, sname)
				
				If InStr(SkillName0, "非表示") > 0 Then
					SkillName0 = "非表示"
					Exit Function
				End If
			End If
		End If
		
		'重複可能な能力は特殊能力付加で名称が置き換えられことはない
		Select Case sname
			Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'特殊能力付加＆強化による修正
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'ユニット用特殊能力による付加
						If .IsConditionSatisfied(sname & "付加２") Then
							buf = LIndex(.ConditionData(sname & "付加２"), 1)
							
							If buf <> "" Then
								SkillName0 = buf
							ElseIf SkillName0 = "" Then 
								SkillName0 = sname
							End If
							
							If InStr(SkillName0, "非表示") > 0 Then
								SkillName0 = "非表示"
								Exit Function
							End If
						End If
						
						'アビリティによる付加
						If .IsConditionSatisfied(sname & "付加") Then
							buf = LIndex(.ConditionData(sname & "付加"), 1)
							
							If buf <> "" Then
								SkillName0 = buf
							ElseIf SkillName0 = "" Then 
								SkillName0 = sname
							End If
							
							If InStr(SkillName0, "非表示") > 0 Then
								SkillName0 = "非表示"
								Exit Function
							End If
						End If
						
						'ユニット用特殊能力による強化
						If SkillName0 = "" Then
							If .IsConditionSatisfied(sname & "強化２") Then
								SkillName0 = LIndex(.ConditionData(sname & "強化２"), 1)
								
								If SkillName0 = "" Then
									SkillName0 = sname
								End If
								
								If InStr(SkillName0, "非表示") > 0 Then
									SkillName0 = "非表示"
									Exit Function
								End If
							End If
						End If
						
						'アビリティによる強化
						If SkillName0 = "" Then
							If .IsConditionSatisfied(sname & "強化") Then
								SkillName0 = LIndex(.ConditionData(sname & "強化"), 1)
								
								If SkillName0 = "" Then
									SkillName0 = sname
								End If
								
								If InStr(SkillName0, "非表示") > 0 Then
									SkillName0 = "非表示"
									Exit Function
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'該当するものが無ければエリアスから検索
		If SkillName0 = "" Then
			With ALDList
				For i = 1 To .Count
					With .Item(i)
						If .AliasType(1) = sname Then
							SkillName0 = .Name
							Exit Function
						End If
					End With
				Next 
			End With
			SkillName0 = sname
		End If
		
		'能力強化系は非表示
		If Right(sname, 2) = "ＵＰ" Or Right(sname, 4) = "ＤＯＷＮ" Then
			SkillName0 = "非表示"
			Exit Function
		End If
		
		Select Case sname
			Case "追加レベル", "メッセージ", "魔力所有"
				'非表示の能力
				SkillName0 = "非表示"
				Exit Function
			Case "耐久"
				If IsOptionDefined("防御力成長") Or IsOptionDefined("防御力レベルアップ") Then
					'防御力成長オプション使用時には耐久能力を非表示
					SkillName0 = "非表示"
					Exit Function
				End If
		End Select
		
		'レベル非表示用の括弧を削除
		If Left(SkillName0, 1) = "(" Then
			SkillName0 = Mid(SkillName0, 2)
			SkillName0 = Left(SkillName0, InStr2(SkillName0, ")") - 1)
		End If
		
		'レベル指定を削除
		i = InStr(SkillName0, "Lv")
		If i > 0 Then
			SkillName0 = Left(SkillName0, i - 1)
		End If
	End Function
	
	'特殊能力名称（必要技能判定用）
	'名称からレベル指定を削除し、名称が非表示にされている場合は元の特殊能力名
	'もしくはエリアス名を使用する。
	Public Function SkillNameForNS(ByRef stype As String) As String
		Dim sd As SkillData
		Dim buf As String
		Dim i As Short
		
		'非表示の特殊能力
		If Right(stype, 2) = "ＵＰ" Or Right(stype, 4) = "ＤＯＷＮ" Then
			SkillNameForNS = stype
			Exit Function
		End If
		If stype = "メッセージ" Then
			SkillNameForNS = stype
			Exit Function
		End If
		
		'パイロットが所有している特殊能力の中から検索
		On Error GoTo ErrorHandler
		sd = colSkill.Item(stype)
		With sd
			If Len(.StrData) > 0 Then
				SkillNameForNS = LIndex(.StrData, 1)
			Else
				SkillNameForNS = stype
			End If
		End With
		
ErrorHandler: 
		
		'SetSkillコマンドで封印されている場合
		If SkillNameForNS = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & stype & ")") Then
				'オリジナルの名称を使用
				SkillNameForNS = Data.SkillName(Level, stype)
				
				If InStr(SkillNameForNS, "非表示") > 0 Then
					SkillNameForNS = "非表示"
				End If
			End If
		End If
		
		'特殊能力付加＆強化による修正
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If Me Is .MainPilot Or Me Is .Pilot(1) Then
						'ユニット用特殊能力による付加
						If .IsConditionSatisfied(stype & "付加２") Then
							buf = LIndex(.ConditionData(stype & "付加２"), 1)
							
							If buf <> "" Then
								SkillNameForNS = buf
							ElseIf SkillNameForNS = "" Then 
								SkillNameForNS = stype
							End If
							
							If InStr(SkillNameForNS, "非表示") > 0 Then
								SkillNameForNS = "非表示"
							End If
						End If
						
						'アビリティによる付加
						If .IsConditionSatisfied(stype & "付加") Then
							buf = LIndex(.ConditionData(stype & "付加"), 1)
							
							If buf <> "" Then
								SkillNameForNS = buf
							ElseIf SkillNameForNS = "" Then 
								SkillNameForNS = stype
							End If
							
							If InStr(SkillNameForNS, "非表示") > 0 Then
								SkillNameForNS = "非表示"
							End If
						End If
						
						'ユニット用特殊能力による強化
						If SkillNameForNS = "" Then
							If .IsConditionSatisfied(stype & "強化２") Then
								SkillNameForNS = LIndex(.ConditionData(stype & "強化２"), 1)
								
								If SkillNameForNS = "" Then
									SkillNameForNS = stype
								End If
								
								If InStr(SkillNameForNS, "非表示") > 0 Then
									SkillNameForNS = "非表示"
								End If
							End If
						End If
						
						'アビリティによる強化
						If SkillNameForNS = "" Then
							If .IsConditionSatisfied(stype & "強化") Then
								SkillNameForNS = LIndex(.ConditionData(stype & "強化"), 1)
								
								If SkillNameForNS = "" Then
									SkillNameForNS = stype
								End If
								
								If InStr(SkillNameForNS, "非表示") > 0 Then
									SkillNameForNS = "非表示"
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'該当するものが無ければエリアスから検索
		If SkillNameForNS = "" Or SkillNameForNS = "非表示" Then
			With ALDList
				For i = 1 To .Count
					With .Item(i)
						If .AliasType(1) = stype Then
							SkillNameForNS = .Name
							Exit Function
						End If
					End With
				Next 
			End With
			SkillNameForNS = stype
		End If
		
		'レベル非表示用の括弧を削除
		If Left(SkillNameForNS, 1) = "(" Then
			SkillNameForNS = Mid(SkillNameForNS, 2)
			SkillNameForNS = Left(SkillNameForNS, InStr2(SkillNameForNS, ")") - 1)
		End If
		
		'レベル表示を削除
		i = InStr(SkillNameForNS, "Lv")
		If i > 0 Then
			SkillNameForNS = Left(SkillNameForNS, i - 1)
		End If
	End Function
	
	'特殊能力の種類
	Public Function SkillType(ByRef sname As String) As String
		Dim i As Short
		Dim sd As SkillData
		Dim sname0, sname2 As String
		
		If sname = "" Then
			Exit Function
		End If
		
		i = InStr(sname, "Lv")
		If i > 0 Then
			sname0 = Left(sname, i - 1)
		Else
			sname0 = sname
		End If
		
		'エリアスデータが定義されている？
		If ALDList.IsDefined(sname0) Then
			With ALDList.Item(sname0)
				SkillType = .AliasType(1)
				Exit Function
			End With
		End If
		
		'特殊能力一覧から検索
		For	Each sd In colSkill
			With sd
				If sname0 = .Name Then
					SkillType = .Name
					Exit Function
				End If
				
				sname2 = LIndex(.StrData, 1)
				If sname0 = sname2 Then
					SkillType = .Name
					Exit Function
				End If
				
				If Left(sname2, 1) = "(" Then
					If Right(sname2, 1) = ")" Then
						sname2 = Mid(sname2, 2, Len(sname2) - 2)
						If sname = sname2 Then
							SkillType = .Name
							Exit Function
						End If
					End If
				End If
			End With
		Next sd
		
		'その能力を修得していない
		SkillType = sname0
		
		'特殊能力付加による修正
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition And .CountPilot > 0 Then
					If Me Is .MainPilot Or Me Is .Pilot(1) Then
						For i = 1 To .CountCondition
							If Right(.Condition(i), 2) = "付加" Then
								If LIndex(.ConditionData(i), 1) = sname0 Then
									SkillType = .Condition(i)
									SkillType = Left(SkillType, Len(SkillType) - 2)
									Exit For
								End If
							ElseIf Right(.Condition(i), 3) = "付加２" Then 
								If LIndex(.ConditionData(i), 1) = sname0 Then
									SkillType = .Condition(i)
									SkillType = Left(SkillType, Len(SkillType) - 3)
									Exit For
								End If
							End If
						Next 
					End If
				End If
			End With
		End If
	End Function
	
	'スペシャルパワー sname を修得しているか？
	Public Function IsSpecialPowerAvailable(ByRef sname As String) As Boolean
		If Data.SP <= 0 Then
			'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .CountPilot > 0 Then
						If Not .Pilot(1) Is Me Then
							If .MainPilot Is Me Then
								IsSpecialPowerAvailable = Unit_Renamed.Pilot(1).Data.IsSpecialPowerAvailable(Level, sname)
								Exit Function
							End If
						End If
					End If
				End With
			End If
		End If
		
		IsSpecialPowerAvailable = Data.IsSpecialPowerAvailable(Level, sname)
	End Function
	
	'スペシャルパワー sname が有用か？
	Public Function IsSpecialPowerUseful(ByRef sname As String) As Boolean
		IsSpecialPowerUseful = SPDList.Item(sname).Useful(Me)
	End Function
	
	'スペシャルパワー sname に必要なＳＰ値
	Public Function SpecialPowerCost(ByRef sname As String) As Short
		Dim i, j As Short
		Dim adata As String
		
		If Data.SP <= 0 Then
			'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .CountPilot > 0 Then
						If Not .Pilot(1) Is Me Then
							If .MainPilot Is Me Then
								SpecialPowerCost = .Pilot(1).SpecialPowerCost(sname)
								Exit Function
							End If
						End If
					End If
				End With
			End If
		End If
		
		'基本消費ＳＰ値
		SpecialPowerCost = Data.SpecialPowerCost(sname)
		
		'特殊能力による消費ＳＰ値修正
		If IsSkillAvailable("超能力") Or IsSkillAvailable("集中力") Then
			SpecialPowerCost = 0.8 * SpecialPowerCost
		End If
		If IsSkillAvailable("知覚強化") Then
			SpecialPowerCost = 1.2 * SpecialPowerCost
		End If
		
		'ＳＰ消費減少能力
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountPilot > 0 Then
					If .MainPilot Is Me Then
						If .IsConditionSatisfied("ＳＰ消費減少付加") Or .IsConditionSatisfied("ＳＰ消費減少付加２") Then
							adata = SkillData("ＳＰ消費減少")
							For i = 2 To LLength(adata)
								If sname = LIndex(adata, i) Then
									SpecialPowerCost = (10 - SkillLevel("ＳＰ消費減少")) * SpecialPowerCost \ 10
									Exit Function
								End If
							Next 
						End If
					End If
				End If
			End With
		End If
		For i = 1 To CountSkill
			If Skill(i) = "ＳＰ消費減少" Then
				adata = SkillData(i)
				For j = 2 To LLength(adata)
					If sname = LIndex(adata, j) Then
						SpecialPowerCost = (10 - SkillLevel(i)) * SpecialPowerCost \ 10
						Exit Function
					End If
				Next 
			End If
		Next 
	End Function
	
	'スペシャルパワー sname を実行する
	Public Sub UseSpecialPower(ByRef sname As String, Optional ByVal sp_mod As Double = 1)
		Dim my_unit As Unit
		
		If Not SPDList.IsDefined(sname) Then
			Exit Sub
		End If
		
		ClearUnitStatus()
		
		SelectedPilot = Me
		
		'スペシャルパワー使用メッセージ
		If sp_mod <> 2 And Not SPDList.Item(sname).IsEffectAvailable("復活") And Not SPDList.Item(sname).IsEffectAvailable("自爆") Then
			If Unit_Renamed.IsMessageDefined(sname) Then
				OpenMessageForm()
				Unit_Renamed.PilotMessage(sname)
				CloseMessageForm()
			End If
		End If
		
		'同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
		'変化してしまうことがあるため、元のUnitを記録しておく
		my_unit = Unit_Renamed
		
		'スペシャルパワーアニメを表示
		If Not SPDList.Item(sname).PlayAnimation Then
			'メッセージ表示のみ
			OpenMessageForm(Unit_Renamed)
			DisplaySysMessage(Nickname & "は" & sname & "を使った。")
		End If
		
		'Unitが変化した場合に元に戻す
		If Not my_unit Is Unit_Renamed Then
			my_unit.MainPilot()
		End If
		
		'スペシャルパワーを実行
		SPDList.Item(sname).Execute(Me)
		
		'Unitが変化した場合に元に戻す
		If Not my_unit Is Unit_Renamed Then
			my_unit.CurrentForm.MainPilot()
		End If
		
		SP = SP - sp_mod * SpecialPowerCost(sname)
		
		CloseMessageForm()
	End Sub
	
	
	' === ユニット搭乗＆下乗関連処理 ===
	
	'ユニット u に搭乗
	Public Sub Ride(ByRef u As Unit, Optional ByVal is_support As Boolean = False)
		Dim hp_ratio, en_ratio As Double
		Dim plana_ratio As Double
		
		'既に乗っていればなにもしない
		If Unit_Renamed Is u Then
			Exit Sub
		End If
		
		With u
			hp_ratio = 100 * .HP / .MaxHP
			en_ratio = 100 * .EN / .MaxEN
			
			'現在の霊力値を記録
			If MaxPlana > 0 Then
				plana_ratio = 100 * Plana / MaxPlana
			Else
				plana_ratio = -1
			End If
			
			Unit_Renamed = u
			
			If InStrNotNest(Class_Renamed, "サポート)") > 0 And LLength(Class_Renamed) = 1 And Not .IsFeatureAvailable("ダミーユニット") Then
				'サポートにしかなれないパイロットの場合
				.AddSupport(Me)
			ElseIf IsSupport(u) Then 
				'同じユニットクラスに対して通常パイロットとサポートの両方のパターン
				'がいける場合は通常パイロットを優先
				If .CountPilot < System.Math.Abs(.Data.PilotNum) And InStrNotNest(Class_Renamed, u.Class0 & " ") > 0 And Not is_support Then
					.AddPilot(Me)
				Else
					.AddSupport(Me)
				End If
			Else
				'パイロットが既に規定数の場合は全パイロットを降ろす
				If .CountPilot = System.Math.Abs(.Data.PilotNum) Then
					.Pilot(1).GetOff()
				End If
				.AddPilot(Me)
			End If
			
			'Pilotコマンドで作成されたパイロットは全て味方なので搭乗時に変更が必要
			Party = .Party0
			
			'ユニットのステータスをアップデート
			.Update()
			
			'霊力値のアップデート
			If plana_ratio >= 0 Then
				Plana = MaxPlana * plana_ratio \ 100
			Else
				Plana = MaxPlana
			End If
			
			'パイロットが乗り込むことによるＨＰ＆ＥＮの増減に対応
			.HP = .MaxHP * hp_ratio \ 100
			.EN = .MaxEN * en_ratio \ 100
		End With
	End Sub
	
	'パイロットをユニットから降ろす
	Public Sub GetOff(Optional ByVal without_leave As Boolean = False)
		Dim i As Short
		
		'既に降りている？
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		With Unit_Renamed
			For i = 1 To .CountSupport
				If .Support(i) Is Me Then
					'サポートパイロットとして乗り込んでいる場合
					.DeleteSupport(i)
					.Update()
					'UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					Unit_Renamed = Nothing
					Update()
					Exit Sub
				End If
			Next 
			
			'出撃していた場合は退却
			If Not without_leave Then
				If .Status = "出撃" Then
					.Status = "待機"
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(.X, .Y) = Nothing
					EraseUnitBitmap(.X, .Y, False)
				End If
			End If
			
			'通常のパイロットの場合は、そのユニットに乗っていた他のパイロットも降ろされる
			For i = 1 To .CountPilot
				'UPGRADE_NOTE: オブジェクト Unit_Renamed.Pilot().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				.Pilot(1).Unit_Renamed = Nothing
				.DeletePilot(1)
			Next 
			For i = 1 To .CountSupport
				'UPGRADE_NOTE: オブジェクト Unit_Renamed.Support().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				.Support(1).Unit_Renamed = Nothing
				.DeleteSupport(1)
			Next 
			
			.Update()
		End With
		
		'UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Unit_Renamed = Nothing
		
		Update()
	End Sub
	
	'パイロットがユニット u のサポートかどうか
	Public Function IsSupport(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i, j As Short
		
		With u
			If .IsFeatureAvailable("ダミーユニット") Then
				'ダミーユニットの場合はサポートパイロットも通常のパイロットとして扱う
				IsSupport = False
				Exit Function
			End If
			
			'サポート指定が存在する？
			If InStrNotNest(Class_Renamed, "サポート)") = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				'パイロットが乗っていないユニットの場合は通常パイロットを優先
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If .Class_Renamed = pclass Or .Class_Renamed = pclass & "(" & Name & "専用)" Or .Class_Renamed = pclass & "(" & Nickname & "専用)" Or .Class_Renamed = pclass & "(" & Sex & "専用)" Then
						'通常のパイロットとして搭乗可能であればサポートでないとみなす
						IsSupport = False
						Exit Function
					End If
				Next 
			Else
				'通常のパイロットとして搭乗している？
				For i = 1 To .CountPilot
					If .Pilot(i) Is Me Then
						IsSupport = False
						Exit Function
					End If
				Next 
			End If
			
			uclass = .Class0
			
			'通常のサポート？
			For i = 1 To LLength(Class_Renamed)
				If uclass & "(サポート)" = LIndex(Class_Renamed, i) Then
					IsSupport = True
					Exit Function
				End If
			Next 
			
			'パイロットが乗っていないユニットの場合はここで終了
			If .CountPilot = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			'専属サポート？
			With .MainPilot
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If pclass = uclass & "(" & .Name & "専属サポート)" Or pclass = uclass & "(" & .Nickname & "専属サポート)" Or pclass = uclass & "(" & .Sex & "専属サポート)" Then
						IsSupport = True
						Exit Function
					End If
					
					For j = 1 To .CountSkill
						If pclass = uclass & "(" & .Skill(j) & "専属サポート)" Then
							IsSupport = True
							Exit Function
						End If
					Next 
				Next 
			End With
		End With
		
		IsSupport = False
	End Function
	
	'ユニット u に乗ることができるかどうか
	Public Function IsAbleToRide(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i As Short
		
		With u
			'汎用ユニットは必要技能を満たせばＯＫ
			If .Class_Renamed = "汎用" Then
				IsAbleToRide = True
				GoTo CheckNecessarySkill
			End If
			
			'人間ユニット指定を除いて判定
			If Left(.Class_Renamed, 1) = "(" And Right(.Class_Renamed, 1) = ")" Then
				uclass = Mid(.Class_Renamed, 2, Len(.Class_Renamed) - 2)
			Else
				uclass = .Class_Renamed
			End If
			
			'サポートかどうかをまず判定しておく
			If IsSupport(u) Then
				IsAbleToRide = True
				'必要技能をチェックする
				GoTo CheckNecessarySkill
			End If
			
			For i = 1 To LLength(Class_Renamed) 'ユニットクラスは複数設定可能
				pclass = LIndex(Class_Renamed, i)
				If uclass = pclass Or uclass = pclass & "(" & Nickname & "専用)" Or uclass = pclass & "(" & Name & "専用)" Or uclass = pclass & "(" & Sex & "専用)" Then
					IsAbleToRide = True
					'必要技能をチェックする
					GoTo CheckNecessarySkill
				End If
			Next 
			
			'クラスが合わない
			IsAbleToRide = False
			Exit Function
			
CheckNecessarySkill: 
			
			'必要技能＆不必要技能をチェック
			
			'両能力を持っていない場合はチェック不要
			If Not .IsFeatureAvailable("必要技能") And Not .IsFeatureAvailable("不必要技能") Then
				Exit Function
			End If
			
			For i = 1 To .CountFeature
				If .Feature(i) = "必要技能" Then
					If Not .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				ElseIf .Feature(i) = "不必要技能" Then 
					If .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				End If
			Next 
		End With
	End Function
	
	
	' === 一時中断関連処理 ===
	
	'一時中断用データをファイルにセーブする
	Public Sub Dump()
		WriteLine(SaveDataFileNumber, Name, ID, Party)
		WriteLine(SaveDataFileNumber, Level, Exp)
		WriteLine(SaveDataFileNumber, SP, Morale, Plana)
		WriteLine(SaveDataFileNumber, Alive, Away, SupportIndex)
		If Unit_Renamed Is Nothing Then
			WriteLine(SaveDataFileNumber, "-")
		Else
			WriteLine(SaveDataFileNumber, Unit_Renamed.ID)
		End If
	End Sub
	
	'一時中断用データをファイルからロードする
	Public Sub Restore()
		Dim sbuf As String
		Dim ibuf As Short
		Dim bbuf As Boolean
		
		'Name, ID, Party
		Input(SaveDataFileNumber, sbuf)
		Name = sbuf
		Input(SaveDataFileNumber, sbuf)
		ID = sbuf
		Input(SaveDataFileNumber, sbuf)
		Party = sbuf
		
		'Leve, Exp
		Input(SaveDataFileNumber, ibuf)
		Level = ibuf
		Input(SaveDataFileNumber, ibuf)
		Exp = ibuf
		
		'SP, Morale, Plana
		Input(SaveDataFileNumber, ibuf)
		SP = ibuf
		Input(SaveDataFileNumber, ibuf)
		Morale = ibuf
		Input(SaveDataFileNumber, ibuf)
		Plana = ibuf
		
		'Alive, Away, SupportIndex
		Input(SaveDataFileNumber, bbuf)
		Alive = bbuf
		Input(SaveDataFileNumber, bbuf)
		Away = bbuf
		Input(SaveDataFileNumber, ibuf)
		SupportIndex = ibuf
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
	
	'一時中断用データのリンク情報をファイルからロードする
	Public Sub RestoreLinkInfo()
		Dim sbuf As String
		Dim ibuf As Short
		
		'Name, ID, Party
		sbuf = LineInput(SaveDataFileNumber)
		
		'Leve, Exp
		sbuf = LineInput(SaveDataFileNumber)
		
		'SP, Morale, Plana
		sbuf = LineInput(SaveDataFileNumber)
		
		'Alive, Away, SupportIndex
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		Input(SaveDataFileNumber, sbuf)
		Unit_Renamed = UList.Item(sbuf)
	End Sub
	
	'一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim sbuf As String
		Dim ibuf As Short
		
		'Name, ID, Party
		sbuf = LineInput(SaveDataFileNumber)
		
		'Leve, Exp
		sbuf = LineInput(SaveDataFileNumber)
		
		'SP, Morale, Plana
		Input(SaveDataFileNumber, ibuf)
		SP = ibuf
		Input(SaveDataFileNumber, ibuf)
		Morale = ibuf
		Input(SaveDataFileNumber, ibuf)
		Plana = ibuf
		
		'Alive, Away, SupportIndex
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
	
	
	' === その他 ===
	
	'全回復
	Public Sub FullRecover()
		'闘争本能によって初期気力は変化する
		If IsSkillAvailable("闘争本能") Then
			If MinMorale > 100 Then
				SetMorale(MinMorale + 5 * SkillLevel("闘争本能"))
			Else
				SetMorale(100 + 5 * SkillLevel("闘争本能"))
			End If
		Else
			SetMorale(100)
		End If
		
		SP = MaxSP
		Plana = MaxPlana
	End Sub
	
	'同調率
	Public Function SynchroRate() As Short
		Dim lv As Short
		
		If Not IsSkillAvailable("同調率") Then
			Exit Function
		End If
		
		'同調率基本値
		SynchroRate = SkillLevel("同調率")
		
		'レベルによる増加分
		lv = MinLng(Level, 100)
		If IsSkillAvailable("同調率成長") Then
			SynchroRate = SynchroRate + lv * (10 + SkillLevel("同調率成長")) \ 10
		Else
			SynchroRate = SynchroRate + lv
		End If
	End Function
	
	'指揮範囲
	Public Function CommandRange() As Short
		'指揮能力を持っていなければ範囲は0
		If Not IsSkillAvailable("指揮") Then
			CommandRange = 0
			Exit Function
		End If
		
		'指揮能力を持っている場合は階級レベルに依存
		Select Case SkillLevel("階級")
			Case 0 To 6
				CommandRange = 2
			Case 7 To 9
				CommandRange = 3
			Case 10 To 12
				CommandRange = 4
			Case Else
				CommandRange = 5
		End Select
	End Function
	
	'行動決定に用いられる戦闘判断力
	Public Function TacticalTechnique0() As Short
		TacticalTechnique0 = TechniqueBase - Level + 10 * SkillLevel("戦術")
	End Function
	
	Public Function TacticalTechnique() As Short
		'正常な判断能力がある？
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("狂戦士") Then
					Exit Function
				End If
			End With
		End If
		
		TacticalTechnique = TacticalTechnique0
	End Function
	
	'イベントコマンド SetRelation で設定した値を返す
	Public Function Relation(ByRef t As Pilot) As Short
		Relation = GetValueAsLong("関係:" & Name & ":" & t.Name)
	End Function
	
	'射撃能力が「魔力」と表示されるかどうか
	Public Function HasMana() As Boolean
		If IsSkillAvailable("術") Or IsSkillAvailable("魔力所有") Then
			HasMana = True
			Exit Function
		End If
		
		If IsOptionDefined("魔力使用") Then
			HasMana = True
			Exit Function
		End If
		
		HasMana = False
	End Function
End Class