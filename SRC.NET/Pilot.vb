Option Strict Off
Option Explicit On
Friend Class Pilot
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Data As PilotData
	
	'Invalid_string_refer_to_original_code
	Public ID As String
	'所属陣営
	Public Party As String
	'Invalid_string_refer_to_original_code
	'未搭乗時は Nothing
	'UPGRADE_NOTE: Unit �� Unit_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Unit_Renamed As Unit
	
	'Invalid_string_refer_to_original_code
	Public Alive As Boolean
	
	'Invalid_string_refer_to_original_code
	Public Away As Boolean
	
	'Invalid_string_refer_to_original_code
	Public IsAdditionalPilot As Boolean
	
	'Invalid_string_refer_to_original_code
	Public IsAdditionalSupport As Boolean
	
	'Invalid_string_refer_to_original_code
	Public SupportIndex As Short
	
	'レベル
	Private proLevel As Short
	
	'経験値
	Private proEXP As Short
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Private colSkill As New Collection
	
	
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		'UPGRADE_NOTE: �I�u�W�F�N�g Data ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Data = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Unit_Renamed = Nothing
		
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colSkill ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If LocalVariableList.Item(vname).NumericValue <> 1 Then
						Exit Property
					End If
				Else
					Exit Property
				End If
				
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					uname = LocalVariableList.Item(vname).StringValue
				End If
				If uname = "" Then
					Exit Property
				End If
				
				u = UList.Item(uname)
				'End If
				
				With u
					If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						Nickname = .FeatureData("Invalid_string_refer_to_original_code")
						idx = InStr(Nickname, "$(愛称)")
						If idx > 0 Then
							Nickname = Left(Nickname, idx - 1) & Data.Nickname & Mid(Nickname, idx + 5)
						End If
					End If
				End With
			End With
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If dont_call_unit_nickname Then
				ReplaceString(Nickname, "Nickname()", "")
				ReplaceString(Nickname, "nickname()", "")
			End If
			
			'Invalid_string_refer_to_original_code
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Unit_Renamed
			ReplaceSubExpression(Nickname)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If LocalVariableList.Item(vname).NumericValue <> 1 Then
						Exit Property
					End If
				Else
					Exit Property
				End If
				
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					uname = LocalVariableList.Item(vname).StringValue
				End If
				If uname = "" Then
					Exit Property
				End If
				
				u = UList.Item(uname)
				'End If
				
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					idx = InStr(KanaName, "Invalid_string_refer_to_original_code")
					If idx > 0 Then
						KanaName = Left(KanaName, idx - 1) & Data.KanaName & Mid(KanaName, idx + 5)
					End If
					.IsFeatureAvailable("Invalid_string_refer_to_original_code")
					KanaName = .FeatureData("Invalid_string_refer_to_original_code")
					idx = InStr(KanaName, "$(愛称)")
					If idx > 0 Then
						KanaName = Left(KanaName, idx - 1) & Data.Nickname & Mid(KanaName, idx + 5)
					End If
					KanaName = StrToHiragana(KanaName)
					'End If
				End With
			End With
			
			'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
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
			
			'Invalid_string_refer_to_original_code
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				'Invalid_string_refer_to_original_code
				If .IsFeatureAvailable("性格変更") Then
					Personality = .FeatureData("性格変更")
					Exit Property
				End If
				
				'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property Bitmap(Optional ByVal use_orig As Boolean = False) As String
		Get
			Dim u As Unit
			Dim uname, vname As String
			
			If use_orig Then
				Bitmap = Data.Bitmap0
			Else
				Bitmap = Data.Bitmap
			End If
			
			'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If LocalVariableList.Item(vname).NumericValue <> 1 Then
						Exit Property
					End If
				Else
					Exit Property
				End If
				
				vname = "Invalid_string_refer_to_original_code" & ID & "]"
				If IsLocalVariableDefined(vname) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					uname = LocalVariableList.Item(vname).StringValue
				End If
				If uname = "" Then
					Exit Property
				End If
				
				u = UList.Item(uname)
				'End If
				
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'End If
				End With
			End With
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property BGM() As String
		Get
			BGM = Data.BGM
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property MessageType() As String
		Get
			MessageType = Name
			
			'Invalid_string_refer_to_original_code
			If IsSkillAvailable("Invalid_string_refer_to_original_code") Then
				MessageType = SkillData("Invalid_string_refer_to_original_code")
			End If
			
			'Invalid_string_refer_to_original_code
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
						MessageType = LIndex(.ConditionData("Invalid_string_refer_to_original_code"), 2)
					End If
				End With
			End If
		End Get
	End Property
	
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property Defense() As Short
		Get
			If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					Defense = Defense + (Level * (1 + 2 * SkillLevel("防御成長"))) \ 2
				Else
					Defense = Defense + Int(Level * (1 + SkillLevel("防御成長")))
				End If
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End If
		End Get
	End Property
	
	
	'Invalid_string_refer_to_original_code
	
	'レベル
	
	Public Property Level() As Short
		Get
			Level = proLevel
		End Get
		Set(ByVal Value As Short)
			If proLevel = Value Then
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			proEXP = Value Mod 500
			proLevel = proLevel + Value \ 500
			
			'Invalid_string_refer_to_original_code
			If proEXP < 0 Then
				If proLevel > 1 Then
					proEXP = proEXP + 500
					proLevel = proLevel - 1
				Else
					'Invalid_string_refer_to_original_code
					proEXP = 0
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If Value \ 500 > 0 Then
				If IsOptionDefined("レベル限界突破") Then
					If proLevel > 999 Then 'Invalid_string_refer_to_original_code
						proLevel = 999
						proEXP = 500
					End If
				Else
					If proLevel > 99 Then 'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property MaxSP() As Integer
		Get
			Dim lv As Short
			
			'Invalid_string_refer_to_original_code
			If Data.SP <= 0 Then
				MaxSP = 0
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
			If IsSkillAvailable("Invalid_string_refer_to_original_code") Then
				MaxSP = MaxSP \ 2
			ElseIf IsSkillAvailable("Invalid_string_refer_to_original_code") Then 
				MaxSP = 1.5 * MaxSP
			End If
			If IsOptionDefined("Invalid_string_refer_to_original_code") Then
				MaxSP = MaxSP \ 2
			End If
			
			'基本値を追加
			MaxSP = MaxSP + Data.SP
			
			'Invalid_string_refer_to_original_code
			MaxSP = MaxSP + SkillLevel("Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			MaxSP = MaxSP - SkillLevel("Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			If MaxSP > 9999 Then
				MaxSP = 9999
			End If
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	
	Public Property SP() As Integer
		Get
			SP = proSP
			
			'Invalid_string_refer_to_original_code
			
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
				
				'Invalid_string_refer_to_original_code
				If Data.SP > 0 Then
					'Invalid_string_refer_to_original_code
					With .Pilot(1)
						If .MaxSP > 0 Then
							proSP = MaxSP * .SP0 \ .MaxSP
							SP = proSP
						End If
					End With
				Else
					'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			
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
				
				'Invalid_string_refer_to_original_code
				With .Pilot(1)
					If Data.SP > 0 Then
						'Invalid_string_refer_to_original_code
						If .MaxSP > 0 Then
							.SP0 = .MaxSP * proSP \ MaxSP
							proSP = MaxSP * .SP0 \ .MaxSP
						End If
					Else
						'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			
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
				
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			
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
				
				'Invalid_string_refer_to_original_code
				With .Pilot(1)
					If IsSkillAvailable("霊力") Then
						'Invalid_string_refer_to_original_code
						If .MaxSP > 0 Then
							.Plana0 = .MaxPlana * proPlana \ MaxPlana
							proPlana = MaxPlana * .Plana0 \ .MaxPlana
						End If
					Else
						'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
	'スペシャルパワーの個数
	Public ReadOnly Property CountSpecialPower() As Short
		Get
			If Data.SP <= 0 Then
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		skill_num = 0
		For	Each sd In Data.colSkill
			With sd
				If Level >= .NecessaryLevel Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Name = "ハンター" _
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					For i = 1 To skill_num
						If .Name = skill_name(i) Then
							If .StrData = skill_data(i).StrData Then
								'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					skill_data(i) = sd
				End If
				'End If
			End With
		Next sd
		
		'Invalid_string_refer_to_original_code
		Dim sname, alist, sdata As String
		Dim buf As String
		If IsGlobalVariableDefined("Ability(" & ID & ")") Then
			
			'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			alist = GlobalVariableList.Item("Ability(" & ID & ")").StringValue
			For i = 1 To LLength(alist)
				sname = LIndex(alist, i)
				'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				buf = GlobalVariableList.Item("Ability(" & ID & "," & sname & ")").StringValue
				sdata = ListTail(buf, 2)
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Or sname = "ハンター" _
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				For j = 1 To skill_num
					If sname = skill_name(j) Then
						If sdata = skill_data(j).StrData Then
							'Invalid_string_refer_to_original_code
							Exit For
						End If
					End If
				Next 
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
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: �I�u�W�F�N�g skill_data() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			skill_data(j) = Nothing
		Else
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
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
		'Next
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			For j = 1 To skill_num
				If Not skill_data(j) Is Nothing Then
					If Unit_Renamed.ConditionLifetime(skill_data(j).Name & "Invalid_string_refer_to_original_code") > 0 Then
						'UPGRADE_NOTE: �I�u�W�F�N�g skill_data() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						skill_data(j) = Nothing
					End If
				End If
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		With colSkill
			For i = 1 To skill_num
				If Not skill_data(i) Is Nothing Then
					Select Case skill_name(i)
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
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
		
		'Invalid_string_refer_to_original_code
		
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
		
		'Invalid_string_refer_to_original_code
		lv = Level + SkillLevel("追加レベル")
		If IsOptionDefined("Invalid_string_refer_to_original_code") Then
			InfightBase = InfightBase + (lv * (1 + 2 * SkillLevel("Invalid_string_refer_to_original_code"))) \ 2
			ShootingBase = ShootingBase + (lv * (1 + 2 * SkillLevel("Invalid_string_refer_to_original_code"))) \ 2
		Else
			InfightBase = InfightBase + Int(lv * (1 + SkillLevel("Invalid_string_refer_to_original_code")))
			ShootingBase = ShootingBase + Int(lv * (1 + SkillLevel("Invalid_string_refer_to_original_code")))
		End If
		HitBase = HitBase + Int(lv * (2 + SkillLevel("命中成長")))
		DodgeBase = DodgeBase + Int(lv * (2 + SkillLevel("回避成長")))
		TechniqueBase = TechniqueBase + Int(lv * (1 + SkillLevel("Invalid_string_refer_to_original_code")))
		IntuitionBase = IntuitionBase + Int(lv * (1 + SkillLevel("Invalid_string_refer_to_original_code")))
		
		'Invalid_string_refer_to_original_code
		InfightBase = InfightBase + SkillLevel("Invalid_string_refer_to_original_code")
		ShootingBase = ShootingBase + SkillLevel("Invalid_string_refer_to_original_code")
		HitBase = HitBase + SkillLevel("Invalid_string_refer_to_original_code")
		DodgeBase = DodgeBase + SkillLevel("Invalid_string_refer_to_original_code")
		TechniqueBase = TechniqueBase + SkillLevel("Invalid_string_refer_to_original_code")
		IntuitionBase = IntuitionBase + SkillLevel("Invalid_string_refer_to_original_code")
		
		'Invalid_string_refer_to_original_code
		InfightBase = InfightBase - SkillLevel("Invalid_string_refer_to_original_code")
		ShootingBase = ShootingBase - SkillLevel("Invalid_string_refer_to_original_code")
		HitBase = HitBase - SkillLevel("Invalid_string_refer_to_original_code")
		DodgeBase = DodgeBase - SkillLevel("Invalid_string_refer_to_original_code")
		TechniqueBase = TechniqueBase - SkillLevel("Invalid_string_refer_to_original_code")
		IntuitionBase = IntuitionBase - SkillLevel("Invalid_string_refer_to_original_code")
		
		'Invalid_string_refer_to_original_code
		InfightBase = MinLng(InfightBase, 9999)
		ShootingBase = MinLng(ShootingBase, 9999)
		HitBase = MinLng(HitBase, 9999)
		DodgeBase = MinLng(DodgeBase, 9999)
		TechniqueBase = MinLng(TechniqueBase, 9999)
		IntuitionBase = MinLng(IntuitionBase, 9999)
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		InfightMod = 0
		ShootingMod = 0
		HitMod = 0
		DodgeMod = 0
		TechniqueMod = 0
		IntuitionMod = 0
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		HitMod = HitMod + 2 * lv
		DodgeMod = DodgeMod + 2 * lv
		
		If IsSkillAvailable("Invalid_string_refer_to_original_code") Then
			HitMod = HitMod + 5
			DodgeMod = DodgeMod + 5
		End If
		If IsSkillAvailable("悟り") Then
			HitMod = HitMod + 10
			DodgeMod = DodgeMod + 10
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		HitMod = HitMod + 5
		DodgeMod = DodgeMod + 5
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		If Unit_Renamed Is Nothing Then
			GoTo SkipUnitMod
		End If
		
		Dim padaption(4) As Short
		With Unit_Renamed
			'Invalid_string_refer_to_original_code
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			For i = 2 To .CountPilot
				With .Pilot(i)
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					If HasMana() Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End With
			Next 
			For i = 1 To .CountSupport
				With .Support(i)
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					If HasMana() Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End With
			Next 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			With .AdditionalSupport
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If HasMana() Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End With
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountFeature
				Select Case .Feature(i)
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							InfightMod = InfightMod + 5 * .FeatureLevel(i)
						End If
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							ShootingMod = ShootingMod + 5 * .FeatureLevel(i)
						End If
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							HitMod = HitMod + 5 * .FeatureLevel(i)
						End If
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							DodgeMod = DodgeMod + 5 * .FeatureLevel(i)
						End If
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							TechniqueMod = TechniqueMod + 5 * .FeatureLevel(i)
						End If
					Case "Invalid_string_refer_to_original_code"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							IntuitionMod = IntuitionMod + 5 * .FeatureLevel(i)
						End If
				End Select
			Next 
			
			'地形適応変更
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				
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
					If .Feature(i) = "Invalid_string_refer_to_original_code" Then
						For j = 1 To 4
							If StrToLng(LIndex(.FeatureData(i), j)) >= 0 Then
								'Invalid_string_refer_to_original_code
								If padaption(j) < 4 Then
									padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
									'Invalid_string_refer_to_original_code
									If padaption(j) > 4 Then
										padaption(j) = 4
									End If
								End If
							Else
								'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		SetMorale(Morale)
		
SkipUnitMod: 
		
		'Invalid_string_refer_to_original_code
		Infight = MinLng(InfightBase + InfightMod + InfightMod2, 9999)
		Shooting = MinLng(ShootingBase + ShootingMod + ShootingMod2, 9999)
		Hit = MinLng(HitBase + HitMod + HitMod2, 9999)
		Dodge = MinLng(DodgeBase + DodgeMod + DodgeMod2, 9999)
		Technique = MinLng(TechniqueBase + TechniqueMod + TechniqueMod2, 9999)
		Intuition = MinLng(IntuitionBase + IntuitionMod + IntuitionMod2, 9999)
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		
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
		
		'Invalid_string_refer_to_original_code
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		my_unit = Unit_Renamed
		
		With Unit_Renamed
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Exit Sub
			'End If
			If Not Unit_Renamed Is MapDataForUnit(.X, .Y) Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			If .CountPilot = 0 Then
				Exit Sub
			End If
			If Not Me Is .MainPilot Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("暴走") Then
				Exit Sub
			End If
			If .IsConditionSatisfied("混乱") Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			my_party = .Party
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cmd_rank = my_cmd_rank
			If InStr(Name, "(ザコ)") = 0 And InStr(Name, "(汎用)") = 0 Then
				my_cmd_rank = DEFAULT_LEVEL
			Else
				my_cmd_rank = 0
			End If
			cmd_rank = 0
			'End If
			
			'Invalid_string_refer_to_original_code
			team = SkillData("Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			cs_level = DEFAULT_LEVEL
			max_range = 5
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						If .CountPilot = 0 Then
							GoTo NextUnit
						End If
						
						'Invalid_string_refer_to_original_code
						Select Case my_party
							Case "味方", "Invalid_string_refer_to_original_code"
								Select Case .Party
									Case "敵", "Invalid_string_refer_to_original_code"
										GoTo NextUnit
								End Select
							Case Else
								If my_party <> .Party Then
									GoTo NextUnit
								End If
						End Select
						
						'Invalid_string_refer_to_original_code
						If .IsConditionSatisfied("暴走") Then
							GoTo NextUnit
						End If
						If .IsConditionSatisfied("混乱") Then
							GoTo NextUnit
						End If
					End With
					
					With u.MainPilot(True)
						'Invalid_string_refer_to_original_code
						uteam = .SkillData("Invalid_string_refer_to_original_code")
						If team <> uteam And uteam <> "" Then
							GoTo NextUnit
						End If
						
						'Invalid_string_refer_to_original_code
						If range <= 2 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						End If
						
						'Invalid_string_refer_to_original_code
						If my_cmd_rank >= 0 Then
							If range > .CommandRange Then
								GoTo NextUnit
							End If
							
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							If cmd_rank2 > cmd_rank Then
								cmd_rank = cmd_rank2
								cmd_level = .SkillLevel("Invalid_string_refer_to_original_code")
							ElseIf cmd_rank2 = cmd_rank Then 
								cmd_level = MaxDbl(cmd_level, .SkillLevel("Invalid_string_refer_to_original_code"))
							End If
						End If
					End With
NextUnit: 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'Invalid_string_refer_to_original_code
			If cs_level <> DEFAULT_LEVEL Then
				HitMod2 = HitMod2 + 5 * cs_level
				DodgeMod2 = DodgeMod2 + 5 * cs_level
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case my_cmd_rank
				Case DEFAULT_LEVEL
					'Invalid_string_refer_to_original_code
				Case 0
					HitMod2 = HitMod2 + 5 * cmd_level
					DodgeMod2 = DodgeMod2 + 5 * cmd_level
				Case Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If cmd_rank > my_cmd_rank Then
						HitMod2 = HitMod2 + 5 * cmd_level
						DodgeMod2 = DodgeMod2 + 5 * cmd_level
					End If
			End Select
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			
			'Invalid_string_refer_to_original_code
			If mod_stack Then
				For i = 1 To .CountSupport
					rel_lv = rel_lv + Relation(.Support(i))
				Next 
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				rel_lv = rel_lv + Relation(.AdditionalSupport)
			End If
			For i = 1 To .CountSupport
				rel_lv = MaxLng(Relation(.Support(i)), rel_lv)
			Next 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If IsOptionDefined("Invalid_string_refer_to_original_code") Then
				max_range = 2
			Else
				max_range = 1
			End If
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						If .CountPilot = 0 Then
							GoTo NextUnit2
						End If
						
						'Invalid_string_refer_to_original_code
						Select Case my_party
							Case "味方", "Invalid_string_refer_to_original_code"
								Select Case .Party
									Case "敵", "Invalid_string_refer_to_original_code"
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
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							rel_lv = rel_lv + Relation(.AdditionalSupport)
						End If
						rel_lv = MaxLng(Relation(.MainPilot(True)), rel_lv)
						For k = 2 To .CountPilot
							rel_lv = MaxLng(Relation(.Pilot(k)), rel_lv)
						Next 
						For k = 1 To .CountSupport
							rel_lv = MaxLng(Relation(.Support(k)), rel_lv)
						Next 
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
						'End If
						'End If
					End With
NextUnit2: 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
	'霊力最大値
	Public Function MaxPlana() As Integer
		Dim lv As Short
		
		If Not IsSkillAvailable("霊力") Then
			'Invalid_string_refer_to_original_code
			MaxPlana = 0
			
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		lv = MinLng(Level, 100)
		If IsSkillAvailable("霊力成長") Then
			MaxPlana = MaxPlana + 1.5 * lv * (10 + SkillLevel("霊力成長")) \ 10
		Else
			MaxPlana = MaxPlana + 1.5 * lv
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Function CountSkill() As Short
		CountSkill = colSkill.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Skill(ByRef Index As Object) As String
		Dim sd As SkillData
		
		sd = colSkill.Item(Index)
		Skill = sd.Name
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsSkillAvailable(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable = True
		Exit Function
		
ErrorHandler: 
		
		'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					IsSkillAvailable = True
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				IsSkillAvailable = True
				Exit Function
			End With
		End If
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsSkillAvailable = True
		Exit Function
		'End If
		'End If
		'End With
		'End If
		
		IsSkillAvailable = False
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function IsSkillAvailable2(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable2 = True
		Exit Function
		
ErrorHandler: 
		IsSkillAvailable2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
				'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				sname = CStr(Index)
			End If
		End If
		
		If ref_mode = "修正値" Then
			SkillLevel = 0
		ElseIf ref_mode = "基本値" Then 
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "ハンター", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If SkillLevel = DEFAULT_LEVEL Then
					SkillLevel = 1
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
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
				'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				sname = CStr(Index)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				IsSkillLevelSpecified = True
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			IsSkillLevelSpecified = True
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			IsSkillLevelSpecified = True
			'End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
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
				'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				sname = CStr(Index)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "ハンター", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'End If
			'End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SkillName(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'能力強化系は非表示
			If Right(sname, 2) = "Invalid_string_refer_to_original_code" Or Right(sname, 4) = "Invalid_string_refer_to_original_code" Then
				SkillName = "非表示"
				Exit Function
			End If
			
			Select Case sname
				Case "追加レベル", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					SkillName = "非表示"
					Exit Function
				Case "得意技", "不得手"
					'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If .Level <> DEFAULT_LEVEL And InStr(SkillName, "Lv") = 0 And Left(SkillName, 1) <> "(" Then
				SkillName = SkillName & "Lv" & VB6.Format(.Level)
			End If
		End With
		
ErrorHandler: 
		
		If sname = "" Then
			If IsNumeric(Index) Then
				Exit Function
			Else
				'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				sname = CStr(Index)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
			'Invalid_string_refer_to_original_code
			SkillName = "非表示"
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "得意技", "不得手"
				SkillName = sname
				Exit Function
		End Select
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "ハンター", "Invalid_string_refer_to_original_code"
				If IsNumeric(Index) Then
					If Left(SkillName, 1) = "(" Then
						SkillName = Mid(SkillName, 2)
						SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
					End If
					Exit Function
				End If
			Case "Invalid_string_refer_to_original_code"
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
		
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						
						If buf <> "" Then
							SkillName = buf
						ElseIf SkillName = "" Then 
							SkillName = sname
						End If
						
						If InStr(SkillName, "非表示") > 0 Then
							SkillName = "非表示"
							Exit Function
						End If
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If InStr(SkillName, "Lv") > 0 Then
							SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				
				'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
					If .ConditionLevel(sname & "付加") <> DEFAULT_LEVEL Then
						If InStr(SkillName, "Lv") > 0 Then
							SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
						End If
						SkillName = SkillName & "Lv" & VB6.Format(.ConditionLevel(sname & "付加"))
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If SkillName = "" Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					
					If SkillName = "" Then
						SkillName = sname
					End If
					
					If InStr(SkillName, "非表示") > 0 Then
						SkillName = "非表示"
						Exit Function
					End If
					
					SkillName = SkillName & "Lv0"
				End If
				
				'Invalid_string_refer_to_original_code_
				'And sname <> "霊力" _
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			End With
		Else
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		End If
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If SkillName = "" Then
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			
			If SkillName = "" Then
				SkillName = sname
			End If
			
			If InStr(SkillName, "非表示") > 0 Then
				SkillName = "非表示"
				Exit Function
			End If
			
			SkillName = SkillName & "Lv0"
		End If
		
		'Invalid_string_refer_to_original_code_
		'And sname <> "霊力" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'End If
		'End If
		'End If
		'End If
		'End If
		'End With
		'End If
		
		'能力強化系は非表示
		If Right(sname, 2) = "Invalid_string_refer_to_original_code" Or Right(sname, 4) = "Invalid_string_refer_to_original_code" Then
			SkillName = "非表示"
			Exit Function
		End If
		
		Select Case sname
			Case "追加レベル", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				SkillName = "非表示"
				Exit Function
			Case "Invalid_string_refer_to_original_code"
				If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					SkillName = "非表示"
					Exit Function
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
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
	
	'Invalid_string_refer_to_original_code
	Public Function SkillName0(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'能力強化系は非表示
			If Right(sname, 2) = "Invalid_string_refer_to_original_code" Or Right(sname, 4) = "Invalid_string_refer_to_original_code" Then
				SkillName0 = "非表示"
				Exit Function
			End If
			
			Select Case sname
				Case "追加レベル", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					SkillName0 = "非表示"
					Exit Function
				Case "得意技", "不得手"
					'Invalid_string_refer_to_original_code
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
				'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				sname = CStr(Index)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
			'Invalid_string_refer_to_original_code
			SkillName0 = "非表示"
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "得意技", "不得手"
				SkillName0 = sname
				Exit Function
		End Select
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case sname
			Case "ハンター", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						
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
					
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
					If SkillName0 = "" Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						
						If SkillName0 = "" Then
							SkillName0 = sname
						End If
						
						If InStr(SkillName0, "非表示") > 0 Then
							SkillName0 = "非表示"
							Exit Function
						End If
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				If SkillName0 = "" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					
					If SkillName0 = "" Then
						SkillName0 = sname
					End If
					
					If InStr(SkillName0, "非表示") > 0 Then
						SkillName0 = "非表示"
						Exit Function
					End If
				End If
			End With
		End If
		'End If
		'End If
		'End With
		'End If
		
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
		If Right(sname, 2) = "Invalid_string_refer_to_original_code" Or Right(sname, 4) = "Invalid_string_refer_to_original_code" Then
			SkillName0 = "非表示"
			Exit Function
		End If
		
		Select Case sname
			Case "追加レベル", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				SkillName0 = "非表示"
				Exit Function
			Case "Invalid_string_refer_to_original_code"
				If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
					'Invalid_string_refer_to_original_code
					SkillName0 = "非表示"
					Exit Function
				End If
		End Select
		
		'レベル非表示用の括弧を削除
		If Left(SkillName0, 1) = "(" Then
			SkillName0 = Mid(SkillName0, 2)
			SkillName0 = Left(SkillName0, InStr2(SkillName0, ")") - 1)
		End If
		
		'Invalid_string_refer_to_original_code
		i = InStr(SkillName0, "Lv")
		If i > 0 Then
			SkillName0 = Left(SkillName0, i - 1)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function SkillNameForNS(ByRef stype As String) As String
		Dim sd As SkillData
		Dim buf As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If Right(stype, 2) = "Invalid_string_refer_to_original_code" Or Right(stype, 4) = "Invalid_string_refer_to_original_code" Then
			SkillNameForNS = stype
			Exit Function
		End If
		If stype = "Invalid_string_refer_to_original_code" Then
			SkillNameForNS = stype
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If SkillNameForNS = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & stype & ")") Then
				'オリジナルの名称を使用
				SkillNameForNS = Data.SkillName(Level, stype)
				
				If InStr(SkillNameForNS, "非表示") > 0 Then
					SkillNameForNS = "非表示"
				End If
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If Me Is .MainPilot Or Me Is .Pilot(1) Then
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						
						If buf <> "" Then
							SkillNameForNS = buf
						ElseIf SkillNameForNS = "" Then 
							SkillNameForNS = stype
						End If
						
						If InStr(SkillNameForNS, "非表示") > 0 Then
							SkillNameForNS = "非表示"
						End If
					End If
					
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
					If SkillNameForNS = "" Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						
						If SkillNameForNS = "" Then
							SkillNameForNS = stype
						End If
						
						If InStr(SkillNameForNS, "非表示") > 0 Then
							SkillNameForNS = "非表示"
						End If
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				If SkillNameForNS = "" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					
					If SkillNameForNS = "" Then
						SkillNameForNS = stype
					End If
					
					If InStr(SkillNameForNS, "非表示") > 0 Then
						SkillNameForNS = "非表示"
					End If
				End If
			End With
		End If
		'End If
		'End If
		'End With
		'End If
		
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
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If ALDList.IsDefined(sname0) Then
			With ALDList.Item(sname0)
				SkillType = .AliasType(1)
				Exit Function
			End With
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		SkillType = sname0
		
		'Invalid_string_refer_to_original_code
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
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
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
	
	'Invalid_string_refer_to_original_code
	Public Function IsSpecialPowerAvailable(ByRef sname As String) As Boolean
		If Data.SP <= 0 Then
			'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Function IsSpecialPowerUseful(ByRef sname As String) As Boolean
		IsSpecialPowerUseful = SPDList.Item(sname).Useful(Me)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function SpecialPowerCost(ByRef sname As String) As Short
		Dim i, j As Short
		Dim adata As String
		
		If Data.SP <= 0 Then
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		SpecialPowerCost = Data.SpecialPowerCost(sname)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpecialPowerCost = 0.8 * SpecialPowerCost
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpecialPowerCost = 1.2 * SpecialPowerCost
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountPilot > 0 Then
					If .MainPilot Is Me Then
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						For i = 2 To LLength(adata)
							If sname = LIndex(adata, i) Then
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								Exit Function
							End If
						Next 
					End If
				End If
			End With
		End If
		'End With
		'End If
		For i = 1 To CountSkill
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			adata = SkillData(i)
			For j = 2 To LLength(adata)
				If sname = LIndex(adata, j) Then
					SpecialPowerCost = (10 - SkillLevel(i)) * SpecialPowerCost \ 10
					Exit Function
				End If
			Next 
			'End If
		Next 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub UseSpecialPower(ByRef sname As String, Optional ByVal sp_mod As Double = 1)
		Dim my_unit As Unit
		
		If Not SPDList.IsDefined(sname) Then
			Exit Sub
		End If
		
		ClearUnitStatus()
		
		SelectedPilot = Me
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If Unit_Renamed.IsMessageDefined(sname) Then
			OpenMessageForm()
			Unit_Renamed.PilotMessage(sname)
			CloseMessageForm()
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		my_unit = Unit_Renamed
		
		'スペシャルパワーアニメを表示
		If Not SPDList.Item(sname).PlayAnimation Then
			'Invalid_string_refer_to_original_code
			OpenMessageForm(Unit_Renamed)
			DisplaySysMessage(Nickname & "は" & sname & "Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		If Not my_unit Is Unit_Renamed Then
			my_unit.MainPilot()
		End If
		
		'Invalid_string_refer_to_original_code
		SPDList.Item(sname).Execute(Me)
		
		'Invalid_string_refer_to_original_code
		If Not my_unit Is Unit_Renamed Then
			my_unit.CurrentForm.MainPilot()
		End If
		
		SP = SP - sp_mod * SpecialPowerCost(sname)
		
		CloseMessageForm()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Sub Ride(ByRef u As Unit, Optional ByVal is_support As Boolean = False)
		Dim hp_ratio, en_ratio As Double
		Dim plana_ratio As Double
		
		'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			.AddSupport(Me)
			IsSupport(u)
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If .CountPilot < System.Math.Abs(.Data.PilotNum) And InStrNotNest(Class_Renamed, u.Class0 & " ") > 0 And Not is_support Then
				.AddPilot(Me)
			Else
				.AddSupport(Me)
			End If
			'Invalid_string_refer_to_original_code
			If .CountPilot = System.Math.Abs(.Data.PilotNum) Then
				.Pilot(1).GetOff()
			End If
			.AddPilot(Me)
			'End If
			
			'Invalid_string_refer_to_original_code
			Party = .Party0
			
			'Invalid_string_refer_to_original_code
			.Update()
			
			'Invalid_string_refer_to_original_code
			If plana_ratio >= 0 Then
				Plana = MaxPlana * plana_ratio \ 100
			Else
				Plana = MaxPlana
			End If
			
			'Invalid_string_refer_to_original_code
			.HP = .MaxHP * hp_ratio \ 100
			.EN = .MaxEN * en_ratio \ 100
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub GetOff(Optional ByVal without_leave As Boolean = False)
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		With Unit_Renamed
			For i = 1 To .CountSupport
				If .Support(i) Is Me Then
					'Invalid_string_refer_to_original_code
					.DeleteSupport(i)
					.Update()
					'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					Unit_Renamed = Nothing
					Update()
					Exit Sub
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			If Not without_leave Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				.Status = "Invalid_string_refer_to_original_code"
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(.X, .Y) = Nothing
				EraseUnitBitmap(.X, .Y, False)
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountPilot
				'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed.Pilot().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				.Pilot(1).Unit_Renamed = Nothing
				.DeletePilot(1)
			Next 
			For i = 1 To .CountSupport
				'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed.Support().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				.Support(1).Unit_Renamed = Nothing
				.DeleteSupport(1)
			Next 
			
			.Update()
		End With
		
		'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Unit_Renamed = Nothing
		
		Update()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function IsSupport(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i, j As Short
		
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			IsSupport = False
			Exit Function
			'End If
			
			'Invalid_string_refer_to_original_code
			If InStrNotNest(Class_Renamed, "Invalid_string_refer_to_original_code") = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				'Invalid_string_refer_to_original_code
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If .Class_Renamed = pclass Or .Class_Renamed = pclass & "(" & Name & "専用)" Or .Class_Renamed = pclass & "(" & Nickname & "専用)" Or .Class_Renamed = pclass & "(" & Sex & "専用)" Then
						'Invalid_string_refer_to_original_code
						IsSupport = False
						Exit Function
					End If
				Next 
			Else
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountPilot
					If .Pilot(i) Is Me Then
						IsSupport = False
						Exit Function
					End If
				Next 
			End If
			
			uclass = .Class0
			
			'Invalid_string_refer_to_original_code
			For i = 1 To LLength(Class_Renamed)
				If uclass & "Invalid_string_refer_to_original_code" = LIndex(Class_Renamed, i) Then
					IsSupport = True
					Exit Function
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			If .CountPilot = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			With .MainPilot
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If pclass = uclass & "(" & .Name & "Invalid_string_refer_to_original_code" Or pclass = uclass & "(" & .Nickname & "Invalid_string_refer_to_original_code" Or pclass = uclass & "(" & .Sex & "Invalid_string_refer_to_original_code" Then
						IsSupport = True
						Exit Function
					End If
					
					For j = 1 To .CountSkill
						If pclass = uclass & "(" & .Skill(j) & "Invalid_string_refer_to_original_code" Then
							IsSupport = True
							Exit Function
						End If
					Next 
				Next 
			End With
		End With
		
		IsSupport = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsAbleToRide(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i As Short
		
		With u
			'Invalid_string_refer_to_original_code
			If .Class_Renamed = "汎用" Then
				IsAbleToRide = True
				GoTo CheckNecessarySkill
			End If
			
			'Invalid_string_refer_to_original_code
			If Left(.Class_Renamed, 1) = "(" And Right(.Class_Renamed, 1) = ")" Then
				uclass = Mid(.Class_Renamed, 2, Len(.Class_Renamed) - 2)
			Else
				uclass = .Class_Renamed
			End If
			
			'Invalid_string_refer_to_original_code
			If IsSupport(u) Then
				IsAbleToRide = True
				'Invalid_string_refer_to_original_code
				GoTo CheckNecessarySkill
			End If
			
			For i = 1 To LLength(Class_Renamed) 'Invalid_string_refer_to_original_code
				pclass = LIndex(Class_Renamed, i)
				If uclass = pclass Or uclass = pclass & "(" & Nickname & "専用)" Or uclass = pclass & "(" & Name & "専用)" Or uclass = pclass & "(" & Sex & "専用)" Then
					IsAbleToRide = True
					'Invalid_string_refer_to_original_code
					GoTo CheckNecessarySkill
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			IsAbleToRide = False
			Exit Function
			
CheckNecessarySkill: 
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If Not .IsFeatureAvailable("Invalid_string_refer_to_original_code") And Not .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				Exit Function
			End If
			
			For i = 1 To .CountFeature
				If .Feature(i) = "Invalid_string_refer_to_original_code" Then
					If Not .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				ElseIf .Feature(i) = "Invalid_string_refer_to_original_code" Then 
					If .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				End If
			Next 
		End With
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
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
	
	'Invalid_string_refer_to_original_code
	Public Function SynchroRate() As Short
		Dim lv As Short
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		'Invalid_string_refer_to_original_code
		lv = MinLng(Level, 100)
		If IsSkillAvailable("Invalid_string_refer_to_original_code") Then
			SynchroRate = SynchroRate + lv * (10 + SkillLevel("Invalid_string_refer_to_original_code")) \ 10
		Else
			SynchroRate = SynchroRate + lv
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function CommandRange() As Short
		'Invalid_string_refer_to_original_code
		If Not IsSkillAvailable("Invalid_string_refer_to_original_code") Then
			CommandRange = 0
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'UPGRADE_WARNING: CommandRange �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'Case CStr(0 To 6)
			'CommandRange = 2
			''UPGRADE_WARNING: CommandRange �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
			'Case CStr(7 To 9)
				'CommandRange = 3
				''UPGRADE_WARNING: CommandRange �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
				'Case CStr(10 To 12)
					'CommandRange = 4
					''UPGRADE_WARNING: CommandRange �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TacticalTechnique0() As Short
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
	End Function
	
	Public Function TacticalTechnique() As Short
		'Invalid_string_refer_to_original_code
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("暴走") Or .IsConditionSatisfied("狂戦士") Then
					Exit Function
				End If
			End With
		End If
		
		TacticalTechnique = TacticalTechnique0
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Relation(ByRef t As Pilot) As Short
		Relation = GetValueAsLong("Invalid_string_refer_to_original_code" & Name & ":" & t.Name)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function HasMana() As Boolean
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		HasMana = True
		Exit Function
		'End If
		
		If IsOptionDefined("魔力使用") Then
			HasMana = True
			Exit Function
		End If
		
		HasMana = False
	End Function
End Class