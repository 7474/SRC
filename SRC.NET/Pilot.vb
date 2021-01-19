Option Strict Off
Option Explicit On
Friend Class Pilot
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�쐬���ꂽ�p�C���b�g�̃N���X
	
	'�p�C���b�g�f�[�^�ւ̃|�C���^
	Public Data As PilotData
	
	'���ʗp�h�c
	Public ID As String
	'�����w�c
	Public Party As String
	'���悵�Ă��郆�j�b�g
	'�����掞�� Nothing
	'UPGRADE_NOTE: Unit �� Unit_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Unit_Renamed As Unit
	
	'�����Ă��邩�ǂ���
	Public Alive As Boolean
	
	'Leave���Ă��邩�ǂ���
	Public Away As Boolean
	
	'�ǉ��p�C���b�g���ǂ���
	Public IsAdditionalPilot As Boolean
	
	'�ǉ��T�|�[�g���ǂ���
	Public IsAdditionalSupport As Boolean
	
	'�T�|�[�g�p�C���b�g�Ƃ��ď�荞�񂾎��̏���
	Public SupportIndex As Short
	
	'���x��
	Private proLevel As Short
	
	'�o���l
	Private proEXP As Short
	'�r�o
	Private proSP As Short
	'�C��
	Private proMorale As Short
	'���
	Private proPlana As Short
	
	'�\�͒l
	Public Infight As Short
	Public Shooting As Short
	Public Hit As Short
	Public Dodge As Short
	Public Technique As Short
	Public Intuition As Short
	Public Adaption As String
	
	'�\�͒l�̊�{�l
	Public InfightBase As Short
	Public ShootingBase As Short
	Public HitBase As Short
	Public DodgeBase As Short
	Public TechniqueBase As Short
	Public IntuitionBase As Short
	
	'�\�͒l�̏C���l
	
	'����\�́������j�b�g�ɂ��C��
	Public InfightMod As Short
	Public ShootingMod As Short
	Public HitMod As Short
	Public DodgeMod As Short
	Public TechniqueMod As Short
	Public IntuitionMod As Short
	
	'�����j�b�g�ɂ��x���C��
	Public InfightMod2 As Short
	Public ShootingMod2 As Short
	Public HitMod2 As Short
	Public DodgeMod2 As Short
	Public TechniqueMod2 As Short
	Public IntuitionMod2 As Short
	
	'�C�͏C���l
	Public MoraleMod As Short
	
	'����\��
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
	
	'����
	
	Public Property Name() As String
		Get
			Name = Data.Name
		End Get
		Set(ByVal Value As String)
			Data = PDList.Item(Value)
			Update()
		End Set
	End Property
	
	'����
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
			
			'���̕ύX
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
				
				'�p�C���b�g�X�e�[�^�X�R�}���h���̏ꍇ�̓��j�b�g����������K�v������
				If .Name = "�X�e�[�^�X�\���p�_�~�[���j�b�g" Then
					'���C���p�C���b�g���ǂ����`�F�b�N
					vname = "���揇��[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "���惆�j�b�g[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsFeatureAvailable("�p�C���b�g����") Then
						Nickname = .FeatureData("�p�C���b�g����")
						idx = InStr(Nickname, "$(����)")
						If idx > 0 Then
							Nickname = Left(Nickname, idx - 1) & Data.Nickname & Mid(Nickname, idx + 5)
						End If
					End If
				End With
			End With
			
			'Pilot��Nickname()��Unit��Nickname()�̌Ăяo���������ɑ����Ȃ��悤��
			'Nickname()�ւ̌Ăяo���͖�����
			If dont_call_unit_nickname Then
				ReplaceString(Nickname, "Nickname()", "")
				ReplaceString(Nickname, "nickname()", "")
			End If
			
			'���̓��̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Unit_Renamed
			ReplaceSubExpression(Nickname)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'�ǂ݉���
	Public ReadOnly Property KanaName() As String
		Get
			Dim idx As Short
			Dim u As Unit
			Dim uname, vname As String
			
			KanaName = Data.KanaName
			
			'���̕ύX
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
				
				'�p�C���b�g�X�e�[�^�X�R�}���h���̏ꍇ�̓��j�b�g����������K�v������
				If .Name = "�X�e�[�^�X�\���p�_�~�[���j�b�g" Then
					'���C���p�C���b�g���ǂ����`�F�b�N
					vname = "���揇��[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "���惆�j�b�g[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsFeatureAvailable("�p�C���b�g�ǂ݉���") Then
						KanaName = .FeatureData("�p�C���b�g�ǂ݉���")
						idx = InStr(KanaName, "$(�ǂ݉���)")
						If idx > 0 Then
							KanaName = Left(KanaName, idx - 1) & Data.KanaName & Mid(KanaName, idx + 5)
						End If
					ElseIf .IsFeatureAvailable("�p�C���b�g����") Then 
						KanaName = .FeatureData("�p�C���b�g����")
						idx = InStr(KanaName, "$(����)")
						If idx > 0 Then
							KanaName = Left(KanaName, idx - 1) & Data.Nickname & Mid(KanaName, idx + 5)
						End If
						KanaName = StrToHiragana(KanaName)
					End If
				End With
			End With
			
			'�ǂ݉������̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Unit_Renamed
			ReplaceSubExpression(KanaName)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'����
	Public ReadOnly Property Sex() As String
		Get
			Sex = Data.Sex
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .IsFeatureAvailable("����") Then
						Sex = .FeatureData("����")
					End If
				End With
			End If
		End Get
	End Property
	
	'���悷�郆�j�b�g�̃N���X
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public ReadOnly Property Class_Renamed() As String
		Get
			Class_Renamed = Data.Class_Renamed
		End Get
	End Property
	
	'�|�����Ƃ��ɓ�����o���l
	Public ReadOnly Property ExpValue() As Short
		Get
			ExpValue = Data.ExpValue
		End Get
	End Property
	
	'���i
	Public ReadOnly Property Personality() As String
		Get
			Personality = Data.Personality
			
			'���j�b�g�ɏ���Ă���H
			If Unit_Renamed Is Nothing Then
				Exit Property
			End If
			
			With Unit_Renamed
				'�A�C�e���p����\�́u���i�ύX�v
				If .IsFeatureAvailable("���i�ύX") Then
					Personality = .FeatureData("���i�ύX")
					Exit Property
				End If
				
				'�ǉ��p�C���b�g�̐��i��D�悳����
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
	
	'�r�b�g�}�b�v
	Public ReadOnly Property Bitmap(Optional ByVal use_orig As Boolean = False) As String
		Get
			Dim u As Unit
			Dim uname, vname As String
			
			If use_orig Then
				Bitmap = Data.Bitmap0
			Else
				Bitmap = Data.Bitmap
			End If
			
			'�p�C���b�g�摜�ύX
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
				
				'�p�C���b�g�X�e�[�^�X�R�}���h���̏ꍇ�̓��j�b�g����������K�v������
				If .Name = "�X�e�[�^�X�\���p�_�~�[���j�b�g" Then
					'���C���p�C���b�g���ǂ����`�F�b�N
					vname = "���揇��[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item(vname).NumericValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						If LocalVariableList.Item(vname).NumericValue <> 1 Then
							Exit Property
						End If
					Else
						Exit Property
					End If
					
					vname = "���惆�j�b�g[" & ID & "]"
					If IsLocalVariableDefined(vname) Then
						'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						uname = LocalVariableList.Item(vname).StringValue
					End If
					If uname = "" Then
						Exit Property
					End If
					
					u = UList.Item(uname)
				End If
				
				With u
					If .IsConditionSatisfied("�p�C���b�g�摜") Then
						Bitmap = LIndex(.ConditionData("�p�C���b�g�摜"), 2)
					End If
					If .IsFeatureAvailable("�p�C���b�g�摜") Then
						Bitmap = .FeatureData("�p�C���b�g�摜")
					End If
				End With
			End With
		End Get
	End Property
	
	'�a�f�l
	Public ReadOnly Property BGM() As String
		Get
			BGM = Data.BGM
		End Get
	End Property
	
	'���b�Z�[�W�^�C�v
	Public ReadOnly Property MessageType() As String
		Get
			MessageType = Name
			
			'�p�C���b�g�\�́u���b�Z�[�W�v
			If IsSkillAvailable("���b�Z�[�W") Then
				MessageType = SkillData("���b�Z�[�W")
			End If
			
			'�\�̓R�s�[�ŕϐg�����ꍇ�̓��b�Z�[�W���R�s�[���p�C���b�g�̂��̂��g��
			If Not Unit_Renamed Is Nothing Then
				With Unit_Renamed
					If .IsConditionSatisfied("���b�Z�[�W") Then
						MessageType = LIndex(.ConditionData("���b�Z�[�W"), 2)
					End If
				End With
			End If
		End Get
	End Property
	
	
	'�h���
	Public ReadOnly Property Defense() As Short
		Get
			If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
				Defense = 100 + 5 * SkillLevel("�ϋv")
				If IsOptionDefined("�h��͒ᐬ��") Then
					Defense = Defense + (Level * (1 + 2 * SkillLevel("�h�䐬��"))) \ 2
				Else
					Defense = Defense + Int(Level * (1 + SkillLevel("�h�䐬��")))
				End If
			Else
				Defense = 100 + 5 * SkillLevel("�ϋv")
			End If
		End Get
	End Property
	
	
	' === ���x�����o���l�֘A���� ===
	
	'���x��
	
	Public Property Level() As Short
		Get
			Level = proLevel
		End Get
		Set(ByVal Value As Short)
			If proLevel = Value Then
				'�ω��Ȃ�
				Exit Property
			End If
			
			proLevel = Value
			Update()
		End Set
	End Property
	
	'�o���l
	
	Public Property Exp() As Integer
		Get
			Exp = proEXP
		End Get
		Set(ByVal Value As Integer)
			Dim prev_level As Short
			
			prev_level = proLevel
			
			'500���ƂɃ��x���A�b�v
			proEXP = Value Mod 500
			proLevel = proLevel + Value \ 500
			
			'�o���l��������ꍇ�̓��x����������
			If proEXP < 0 Then
				If proLevel > 1 Then
					proEXP = proEXP + 500
					proLevel = proLevel - 1
				Else
					'����ȏ�̓��x�����������Ȃ��̂�
					proEXP = 0
				End If
			End If
			
			'���x������`�F�b�N
			If Value \ 500 > 0 Then
				If IsOptionDefined("���x�����E�˔j") Then
					If proLevel > 999 Then '���x��999�őł��~��
						proLevel = 999
						proEXP = 500
					End If
				Else
					If proLevel > 99 Then '���x��99�őł��~��
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
	
	
	'�C��
	
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
			If IsSkillAvailable("�C�͏��") Then
				If IsSkillLevelSpecified("�C�͏��") Then
					MaxMorale = MaxLng(SkillLevel("�C�͏��"), 0)
				End If
			End If
		End Get
	End Property
	
	Public ReadOnly Property MinMorale() As Short
		Get
			MinMorale = 50
			If IsSkillAvailable("�C�͉���") Then
				If IsSkillLevelSpecified("�C�͉���") Then
					MinMorale = MaxLng(SkillLevel("�C�͉���"), 0)
				End If
			End If
		End Get
	End Property
	
	
	' === �r�o�l�֘A���� ===
	
	'�ő�r�o
	Public ReadOnly Property MaxSP() As Integer
		Get
			Dim lv As Short
			
			'�r�o�Ȃ��̏ꍇ�̓��x���Ɋւ�炸0
			If Data.SP <= 0 Then
				MaxSP = 0
				'�������ǉ��p�C���b�g�̏ꍇ�͑�P�p�C���b�g�̍ő�r�o���g�p
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
			
			'���x���ɂ��㏸�l
			lv = Level
			If lv > 99 Then
				lv = 100
			End If
			lv = lv + SkillLevel("�ǉ����x��")
			If lv > 40 Then
				MaxSP = lv + 40
			Else
				MaxSP = 2 * lv
			End If
			If IsSkillAvailable("�r�o�ᐬ��") Then
				MaxSP = MaxSP \ 2
			ElseIf IsSkillAvailable("�r�o������") Then 
				MaxSP = 1.5 * MaxSP
			End If
			If IsOptionDefined("�r�o�ᐬ��") Then
				MaxSP = MaxSP \ 2
			End If
			
			'��{�l��ǉ�
			MaxSP = MaxSP + Data.SP
			
			'�\�͂t�o
			MaxSP = MaxSP + SkillLevel("�r�o�t�o")
			
			'�\�͂c�n�v�m
			MaxSP = MaxSP - SkillLevel("�r�o�c�n�v�m")
			
			'����𒴂��Ȃ��悤��
			If MaxSP > 9999 Then
				MaxSP = 9999
			End If
		End Get
	End Property
	
	'�r�o�l
	
	Public Property SP() As Integer
		Get
			SP = proSP
			
			'�ǉ��p�C���b�g���ǂ�������
			
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
				
				'�ǉ��p�C���b�g�������̂ő�P�p�C���b�g�̂r�o�l�����Ɏg��
				If Data.SP > 0 Then
					'�r�o�����ꍇ�͏���ʂ���v������
					With .Pilot(1)
						If .MaxSP > 0 Then
							proSP = MaxSP * .SP0 \ .MaxSP
							SP = proSP
						End If
					End With
				Else
					'�r�o�������Ȃ��ꍇ�͂��̂܂܎g��
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
			
			'�ǉ��p�C���b�g���ǂ�������
			
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
				
				'�ǉ��p�C���b�g�������̂ő�P�p�C���b�g�̂r�o�l�����Ɏg��
				With .Pilot(1)
					If Data.SP > 0 Then
						'�ǉ��p�C���b�g���r�o�����ꍇ�͑�P�p�C���b�g�Ə������v������
						If .MaxSP > 0 Then
							.SP0 = .MaxSP * proSP \ MaxSP
							proSP = MaxSP * .SP0 \ .MaxSP
						End If
					Else
						'�ǉ��p�C���b�g���r�o�������Ȃ��ꍇ�͑�P�p�C���b�g�̂r�o�l�����̂܂܎g��
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
	
	'���
	
	Public Property Plana() As Integer
		Get
			If IsSkillAvailable("���") Then
				Plana = proPlana
			End If
			
			'�ǉ��p�C���b�g���ǂ�������
			
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
				
				'�ǉ��p�C���b�g�������̂ő�P�p�C���b�g�̗�͂����Ɏg��
				If IsSkillAvailable("���") Then
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
			
			'�ǉ��p�C���b�g���ǂ�������
			
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
				
				'�ǉ��p�C���b�g�������̂ő�P�p�C���b�g�̗�͒l�����Ɏg��
				With .Pilot(1)
					If IsSkillAvailable("���") Then
						'�ǉ��p�C���b�g����͂����ꍇ�͑�P�p�C���b�g�Ə������v������
						If .MaxSP > 0 Then
							.Plana0 = .MaxPlana * proPlana \ MaxPlana
							proPlana = MaxPlana * .Plana0 \ .MaxPlana
						End If
					Else
						'�ǉ��p�C���b�g����͂������Ȃ��ꍇ�͑�P�p�C���b�g�̗�͒l�����̂܂܎g��
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
	
	
	' === �X�y�V�����p���[�֘A���� ===
	
	'�X�y�V�����p���[�̌�
	Public ReadOnly Property CountSpecialPower() As Short
		Get
			If Data.SP <= 0 Then
				'�r�o�������Ȃ��ǉ��p�C���b�g�̏ꍇ�͂P�Ԗڂ̃p�C���b�g�̃f�[�^���g��
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
	
	'idx�Ԗڂ̃X�y�V�����p���[
	Public ReadOnly Property SpecialPower(ByVal idx As Short) As String
		Get
			If Data.SP <= 0 Then
				'�r�o�������Ȃ��ǉ��p�C���b�g�̏ꍇ�͂P�Ԗڂ̃p�C���b�g�̃f�[�^���g��
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
	
	
	'�\�͒l���X�V
	Public Sub Update()
		Dim skill_num As Short
		Dim skill_name(64) As String
		Dim skill_data(64) As SkillData
		Dim i, j As Short
		Dim lv As Double
		Dim sd As SkillData
		
		'���݂̃��x���Ŏg�p�\�ȓ���\�͂̈ꗗ���쐬
		
		'�ȑO�̈ꗗ���폜
		With colSkill
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		'�p�C���b�g�f�[�^���Q�Ƃ��Ȃ���g�p�\�ȓ���\�͂�����
		skill_num = 0
		For	Each sd In Data.colSkill
			With sd
				If Level >= .NecessaryLevel Then
					'���ɓo�^�ς݁H
					If .Name = "�r�o�����" Or .Name = "�X�y�V�����p���[��������" Or .Name = "�n���^�[" Then
						'�����̓���\�͓͂���̔\�͂𕡐������Ƃ��o����
						For i = 1 To skill_num
							If .Name = skill_name(i) Then
								If .StrData = skill_data(i).StrData Then
									'�������f�[�^�w��܂œ���ł���Γ����\�͂ƌ��Ȃ�
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
						'���o�^
						skill_num = skill_num + 1
						skill_name(skill_num) = .Name
						skill_data(skill_num) = sd
					ElseIf .NecessaryLevel > skill_data(i).NecessaryLevel Then 
						'�o�^�ς݂ł���ꍇ�͏K�����x�����������̂�D��
						skill_data(i) = sd
					End If
				End If
			End With
		Next sd
		
		'SetSkill�R�}���h�ŕt�����ꂽ����\�͂�����
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
				
				'���ɓo�^�ς݁H
				If sname = "�r�o�����" Or sname = "�X�y�V�����p���[��������" Or sname = "�n���^�[" Then
					'�����̓���\�͓͂���̔\�͂𕡐������Ƃ��o����
					For j = 1 To skill_num
						If sname = skill_name(j) Then
							If sdata = skill_data(j).StrData Then
								'�������f�[�^�w��܂œ���ł���Γ����\�͂ƌ��Ȃ�
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
					'���o�^
					skill_num = j
					skill_name(j) = sname
				End If
				
				If StrToDbl(LIndex(buf, 1)) = 0 Then
					'���x��0�̏ꍇ�͔\�͂𕕈�
					'UPGRADE_NOTE: �I�u�W�F�N�g skill_data() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					skill_data(j) = Nothing
				Else
					'PDList�̃f�[�^������������킯�ɂ͂����Ȃ��̂�
					'�A�r���e�B�f�[�^��V�K�ɍ쐬
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
		
		'�����g�p�s�\��Ԃ̍ہA�Ή�����Z�\�𕕈󂷂�B
		If Not Unit_Renamed Is Nothing Then
			For j = 1 To skill_num
				If Not skill_data(j) Is Nothing Then
					If Unit_Renamed.ConditionLifetime(skill_data(j).Name & "�g�p�s�\") > 0 Then
						'UPGRADE_NOTE: �I�u�W�F�N�g skill_data() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						skill_data(j) = Nothing
					End If
				End If
			Next 
		End If
		
		'�g�p�\�ȓ���\�͂�o�^
		With colSkill
			For i = 1 To skill_num
				If Not skill_data(i) Is Nothing Then
					Select Case skill_name(i)
						Case "�r�o�����", "�X�y�V�����p���[��������", "�n���^�["
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
		
		'���ꂩ�牺�͔\�͒l�̌v�Z
		
		'��{�l
		With Data
			InfightBase = .Infight
			ShootingBase = .Shooting
			HitBase = .Hit
			DodgeBase = .Dodge
			TechniqueBase = .Technique
			IntuitionBase = .Intuition
			Adaption = .Adaption
		End With
		
		'���x���ɂ��ǉ���
		lv = Level + SkillLevel("�ǉ����x��")
		If IsOptionDefined("�U���͒ᐬ��") Then
			InfightBase = InfightBase + (lv * (1 + 2 * SkillLevel("�i������"))) \ 2
			ShootingBase = ShootingBase + (lv * (1 + 2 * SkillLevel("�ˌ�����"))) \ 2
		Else
			InfightBase = InfightBase + Int(lv * (1 + SkillLevel("�i������")))
			ShootingBase = ShootingBase + Int(lv * (1 + SkillLevel("�ˌ�����")))
		End If
		HitBase = HitBase + Int(lv * (2 + SkillLevel("��������")))
		DodgeBase = DodgeBase + Int(lv * (2 + SkillLevel("��𐬒�")))
		TechniqueBase = TechniqueBase + Int(lv * (1 + SkillLevel("�Z�ʐ���")))
		IntuitionBase = IntuitionBase + Int(lv * (1 + SkillLevel("��������")))
		
		'�\�͂t�o
		InfightBase = InfightBase + SkillLevel("�i���t�o")
		ShootingBase = ShootingBase + SkillLevel("�ˌ��t�o")
		HitBase = HitBase + SkillLevel("�����t�o")
		DodgeBase = DodgeBase + SkillLevel("����t�o")
		TechniqueBase = TechniqueBase + SkillLevel("�Z�ʂt�o")
		IntuitionBase = IntuitionBase + SkillLevel("�����t�o")
		
		'�\�͂c�n�v�m
		InfightBase = InfightBase - SkillLevel("�i���c�n�v�m")
		ShootingBase = ShootingBase - SkillLevel("�ˌ��c�n�v�m")
		HitBase = HitBase - SkillLevel("�����c�n�v�m")
		DodgeBase = DodgeBase - SkillLevel("����c�n�v�m")
		TechniqueBase = TechniqueBase - SkillLevel("�Z�ʂc�n�v�m")
		IntuitionBase = IntuitionBase - SkillLevel("�����c�n�v�m")
		
		'����𒴂��Ȃ��悤��
		InfightBase = MinLng(InfightBase, 9999)
		ShootingBase = MinLng(ShootingBase, 9999)
		HitBase = MinLng(HitBase, 9999)
		DodgeBase = MinLng(DodgeBase, 9999)
		TechniqueBase = MinLng(TechniqueBase, 9999)
		IntuitionBase = MinLng(IntuitionBase, 9999)
		
		'���ꂩ�牺�͓���\�͂ɂ��C���l�̌v�Z
		
		'�܂��͏C���l��������
		InfightMod = 0
		ShootingMod = 0
		HitMod = 0
		DodgeMod = 0
		TechniqueMod = 0
		IntuitionMod = 0
		
		'�p�C���b�g�p����\�͂ɂ��C��
		
		lv = SkillLevel("�����o")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("�m�o����")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("�O��")
		If lv > 0 Then
			HitMod = HitMod + 2 * lv + 3
			DodgeMod = DodgeMod + 2 * lv + 3
		End If
		
		lv = SkillLevel("������")
		HitMod = HitMod + 2 * lv
		DodgeMod = DodgeMod + 2 * lv
		
		If IsSkillAvailable("�T�C�{�[�O") Then
			HitMod = HitMod + 5
			DodgeMod = DodgeMod + 5
		End If
		If IsSkillAvailable("���") Then
			HitMod = HitMod + 10
			DodgeMod = DodgeMod + 10
		End If
		If IsSkillAvailable("���\��") Then
			HitMod = HitMod + 5
			DodgeMod = DodgeMod + 5
		End If
		
		'���ꂩ�牺�̓��j�b�g�ɂ��C���l�̌v�Z
		
		'���j�b�g�ɏ���Ă��Ȃ��H
		If Unit_Renamed Is Nothing Then
			GoTo SkipUnitMod
		End If
		
		Dim padaption(4) As Short
		With Unit_Renamed
			'�N�C�b�N�Z�[�u�����ȂǂŎ��ۂɂ͏���Ă��Ȃ��ꍇ
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			'�T�u�p�C���b�g���T�|�[�g�p�C���b�g�ɂ��T�|�[�g
			If Me Is .MainPilot And .Status = "�o��" Then
				For i = 2 To .CountPilot
					With .Pilot(i)
						InfightMod = InfightMod + 2 * .SkillLevel("�i���T�|�[�g")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("���̓T�|�[�g")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("�ˌ��T�|�[�g")
						End If
						HitMod = HitMod + 3 * .SkillLevel("�T�|�[�g")
						HitMod = HitMod + 2 * .SkillLevel("�����T�|�[�g")
						DodgeMod = DodgeMod + 3 * .SkillLevel("�T�|�[�g")
						DodgeMod = DodgeMod + 2 * .SkillLevel("����T�|�[�g")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("�Z�ʃT�|�[�g")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("�����T�|�[�g")
					End With
				Next 
				For i = 1 To .CountSupport
					With .Support(i)
						InfightMod = InfightMod + 2 * .SkillLevel("�i���T�|�[�g")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("���̓T�|�[�g")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("�ˌ��T�|�[�g")
						End If
						HitMod = HitMod + 3 * .SkillLevel("�T�|�[�g")
						HitMod = HitMod + 2 * .SkillLevel("�����T�|�[�g")
						DodgeMod = DodgeMod + 3 * .SkillLevel("�T�|�[�g")
						DodgeMod = DodgeMod + 2 * .SkillLevel("����T�|�[�g")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("�Z�ʃT�|�[�g")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("�����T�|�[�g")
					End With
				Next 
				If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
					With .AdditionalSupport
						InfightMod = InfightMod + 2 * .SkillLevel("�i���T�|�[�g")
						If HasMana() Then
							ShootingMod = ShootingMod + 2 * .SkillLevel("���̓T�|�[�g")
						Else
							ShootingMod = ShootingMod + 2 * .SkillLevel("�ˌ��T�|�[�g")
						End If
						HitMod = HitMod + 3 * .SkillLevel("�T�|�[�g")
						HitMod = HitMod + 2 * .SkillLevel("�����T�|�[�g")
						DodgeMod = DodgeMod + 3 * .SkillLevel("�T�|�[�g")
						DodgeMod = DodgeMod + 2 * .SkillLevel("����T�|�[�g")
						TechniqueMod = TechniqueMod + 2 * .SkillLevel("�Z�ʃT�|�[�g")
						IntuitionMod = IntuitionMod + 2 * .SkillLevel("�����T�|�[�g")
					End With
				End If
			End If
			
			'���j�b�g���A�C�e���ɂ�鋭��
			For i = 1 To .CountFeature
				Select Case .Feature(i)
					Case "�i������"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							InfightMod = InfightMod + 5 * .FeatureLevel(i)
						End If
					Case "�ˌ�����"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							ShootingMod = ShootingMod + 5 * .FeatureLevel(i)
						End If
					Case "��������"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							HitMod = HitMod + 5 * .FeatureLevel(i)
						End If
					Case "�������"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							DodgeMod = DodgeMod + 5 * .FeatureLevel(i)
						End If
					Case "�Z�ʋ���"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							TechniqueMod = TechniqueMod + 5 * .FeatureLevel(i)
						End If
					Case "��������"
						If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
							IntuitionMod = IntuitionMod + 5 * .FeatureLevel(i)
						End If
				End Select
			Next 
			
			'�n�`�K���ύX
			If .IsFeatureAvailable("�p�C���b�g�n�`�K���ύX") Then
				
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
				
				'�n�`�K���ύX�\�͂ɂ��C��
				For i = 1 To .CountFeature
					If .Feature(i) = "�p�C���b�g�n�`�K���ύX" Then
						For j = 1 To 4
							If StrToLng(LIndex(.FeatureData(i), j)) >= 0 Then
								'�C���l���v���X�̂Ƃ�
								If padaption(j) < 4 Then
									padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
									'�n�`�K����A��荂���͂Ȃ�Ȃ�
									If padaption(j) > 4 Then
										padaption(j) = 4
									End If
								End If
							Else
								'�C���l���}�C�i�X�̂Ƃ��͖{���̒n�`�K����"A"�ȏ�ł��������s�Ȃ�
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
		
		'�C�͂̒l���C�͏���E�C�͉����͈̔͂ɂ���
		SetMorale(Morale)
		
SkipUnitMod: 
		
		'��{�l�ƏC���l�̍��v������ۂ̔\�͒l���Z�o
		Infight = MinLng(InfightBase + InfightMod + InfightMod2, 9999)
		Shooting = MinLng(ShootingBase + ShootingMod + ShootingMod2, 9999)
		Hit = MinLng(HitBase + HitMod + HitMod2, 9999)
		Dodge = MinLng(DodgeBase + DodgeMod + DodgeMod2, 9999)
		Technique = MinLng(TechniqueBase + TechniqueMod + TechniqueMod2, 9999)
		Intuition = MinLng(IntuitionBase + IntuitionMod + IntuitionMod2, 9999)
	End Sub
	
	'����̃��j�b�g�ɂ��x�����ʂ��X�V
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
		
		'�x�����ʂɂ��C���l��������
		
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
		
		'�X�e�[�^�X�\�����ɂ͎x�����ʂ𖳎�
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'���j�b�g�ɏ���Ă��Ȃ���΂����ŏI��
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		'��U����Ă��郆�j�b�g���L�^���Ă���
		my_unit = Unit_Renamed
		
		With Unit_Renamed
			'���j�b�g���o�����Ă��Ȃ���΂����ŏI��
			If .Status <> "�o��" Then
				Exit Sub
			End If
			If Not Unit_Renamed Is MapDataForUnit(.X, .Y) Then
				Exit Sub
			End If
			
			'���C���p�C���b�g�łȂ���΂����ŏI��
			If .CountPilot = 0 Then
				Exit Sub
			End If
			If Not Me Is .MainPilot Then
				Exit Sub
			End If
			
			'����Ȕ��f���o���Ȃ����j�b�g�͎x�����󂯂��Ȃ�
			If .IsConditionSatisfied("�\��") Then
				Exit Sub
			End If
			If .IsConditionSatisfied("����") Then
				Exit Sub
			End If
			
			'�x�����󂯂��邩�ǂ����̔���p�ɐw�c���Q�Ƃ��Ă���
			my_party = .Party
			
			'�w�����ʔ���p�Ɏ����̊K�����x�����Z�o
			If IsSkillAvailable("�K��") Then
				my_cmd_rank = SkillLevel("�K��")
				cmd_rank = my_cmd_rank
			Else
				If InStr(Name, "(�U�R)") = 0 And InStr(Name, "(�ėp)") = 0 Then
					my_cmd_rank = DEFAULT_LEVEL
				Else
					my_cmd_rank = 0
				End If
				cmd_rank = 0
			End If
			
			'�������������Ă���`�[����
			team = SkillData("�`�[��")
			
			'����̃��j�b�g�𒲂ׂ�
			cs_level = DEFAULT_LEVEL
			max_range = 5
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'���j�b�g�Ԃ̋������͈͓��H
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
						'���j�b�g�Ƀp�C���b�g������Ă��Ȃ���Ζ���
						If .CountPilot = 0 Then
							GoTo NextUnit
						End If
						
						'�w�c����v���Ă��Ȃ��Ǝx���͎󂯂��Ȃ�
						Select Case my_party
							Case "����", "�m�o�b"
								Select Case .Party
									Case "�G", "����"
										GoTo NextUnit
								End Select
							Case Else
								If my_party <> .Party Then
									GoTo NextUnit
								End If
						End Select
						
						'���肪����Ȕ��f�\�͂������Ă��Ȃ��ꍇ���x���͎󂯂��Ȃ�
						If .IsConditionSatisfied("�\��") Then
							GoTo NextUnit
						End If
						If .IsConditionSatisfied("����") Then
							GoTo NextUnit
						End If
					End With
					
					With u.MainPilot(True)
						'�����`�[���ɏ������Ă���H
						uteam = .SkillData("�`�[��")
						If team <> uteam And uteam <> "" Then
							GoTo NextUnit
						End If
						
						'�L��T�|�[�g
						If range <= 2 Then
							cs_level = MaxDbl(cs_level, .SkillLevel("�L��T�|�[�g"))
						End If
						
						'�w������
						If my_cmd_rank >= 0 Then
							If range > .CommandRange Then
								GoTo NextUnit
							End If
							
							cmd_rank2 = .SkillLevel("�K��")
							If cmd_rank2 > cmd_rank Then
								cmd_rank = cmd_rank2
								cmd_level = .SkillLevel("�w��")
							ElseIf cmd_rank2 = cmd_rank Then 
								cmd_level = MaxDbl(cmd_level, .SkillLevel("�w��"))
							End If
						End If
					End With
NextUnit: 
				Next 
			Next 
			
			'�ǉ��p�C���b�g�̏ꍇ�͏���Ă��郆�j�b�g���ω����Ă��܂����Ƃ�����̂�
			'�ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'�L��T�|�[�g�ɂ��C��
			If cs_level <> DEFAULT_LEVEL Then
				HitMod2 = HitMod2 + 5 * cs_level
				DodgeMod2 = DodgeMod2 + 5 * cs_level
			End If
			
			'�w���\�͂ɂ��C��
			Select Case my_cmd_rank
				Case DEFAULT_LEVEL
					'�C���Ȃ�
				Case 0
					HitMod2 = HitMod2 + 5 * cmd_level
					DodgeMod2 = DodgeMod2 + 5 * cmd_level
				Case Else
					'�������K�����x���������Ă���ꍇ�͂�荂���K�����x����
					'���p�C���b�g�̎w�����ʂ݂̂��󂯂�
					If cmd_rank > my_cmd_rank Then
						HitMod2 = HitMod2 + 5 * cmd_level
						DodgeMod2 = DodgeMod2 + 5 * cmd_level
					End If
			End Select
			
			'�x�����ʂɂ��C����\�͒l�ɉ��Z
			Infight = Infight + InfightMod2
			Shooting = Shooting + ShootingMod2
			Hit = Hit + HitMod2
			Dodge = Dodge + DodgeMod2
			Technique = Technique + TechniqueMod2
			Intuition = Intuition + IntuitionMod2
			
			'�M���␳
			If Not IsOptionDefined("�M���␳") Then
				Exit Sub
			End If
			If InStr(Name, "(�U�R)") > 0 Then
				Exit Sub
			End If
			
			'�M���␳���d������H
			mod_stack = IsOptionDefined("�M���␳�d��")
			
			'�������j�b�g�ɏ���Ă���T�|�[�g�p�C���b�g����̕␳
			If mod_stack Then
				For i = 1 To .CountSupport
					rel_lv = rel_lv + Relation(.Support(i))
				Next 
				If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
					rel_lv = rel_lv + Relation(.AdditionalSupport)
				End If
			Else
				For i = 1 To .CountSupport
					rel_lv = MaxLng(Relation(.Support(i)), rel_lv)
				Next 
				If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
					rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
				End If
			End If
			
			'���͂̃��j�b�g����̕␳
			If IsOptionDefined("�M���␳�͈͊g��") Then
				max_range = 2
			Else
				max_range = 1
			End If
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					'���j�b�g�Ԃ̋������͈͓��H
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
						'���j�b�g�Ƀp�C���b�g������Ă��Ȃ���Ζ���
						If .CountPilot = 0 Then
							GoTo NextUnit2
						End If
						
						'�������ǂ�������
						Select Case my_party
							Case "����", "�m�o�b"
								Select Case .Party
									Case "�G", "����"
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
							If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
							If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
								rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
							End If
						End If
					End With
NextUnit2: 
				Next 
			Next 
			
			'�ǉ��p�C���b�g�̏ꍇ�͏���Ă��郆�j�b�g���ω����Ă��܂����Ƃ�����̂�
			'�ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
			If Not my_unit Is Unit_Renamed Then
				my_unit.MainPilot()
			End If
			
			'�M���␳��ݒ�
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
	
	
	' === ��͊֘A���� ===
	
	'��͍ő�l
	Public Function MaxPlana() As Integer
		Dim lv As Short
		
		If Not IsSkillAvailable("���") Then
			'��͔\�͂������Ȃ��ꍇ
			MaxPlana = 0
			
			'�ǉ��p�C���b�g�̏ꍇ�͑�P�p�C���b�g�̗�͂����Ɏg��
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
		
		'��͊�{�l
		MaxPlana = SkillLevel("���")
		
		'���x���ɂ�鑝����
		lv = MinLng(Level, 100)
		If IsSkillAvailable("��͐���") Then
			MaxPlana = MaxPlana + 1.5 * lv * (10 + SkillLevel("��͐���")) \ 10
		Else
			MaxPlana = MaxPlana + 1.5 * lv
		End If
	End Function
	
	
	' === ����\�͊֘A���� ===
	
	'����\�͂̑���
	Public Function CountSkill() As Short
		CountSkill = colSkill.Count()
	End Function
	
	'����\��
	Public Function Skill(ByRef Index As Object) As String
		Dim sd As SkillData
		
		sd = colSkill.Item(Index)
		Skill = sd.Name
	End Function
	
	'���݂̃��x���ɂ����ē���\�� sname ���g�p�\��
	Public Function IsSkillAvailable(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable = True
		Exit Function
		
ErrorHandler: 
		
		'����\�͕t���������ɂ��C��
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
				
				If .IsConditionSatisfied(sname & "�t��") Then
					IsSkillAvailable = True
					Exit Function
				ElseIf .IsConditionSatisfied(sname & "�t���Q") Then 
					IsSkillAvailable = True
					Exit Function
				End If
				
				If .IsConditionSatisfied(sname & "����") Then
					If .ConditionLevel(sname & "����") > 0 Then
						IsSkillAvailable = True
						Exit Function
					End If
				End If
				If .IsConditionSatisfied(sname & "�����Q") Then
					If .ConditionLevel(sname & "�����Q") > 0 Then
						IsSkillAvailable = True
						Exit Function
					End If
				End If
			End With
		End If
		
		IsSkillAvailable = False
	End Function
	
	'���݂̃��x���ɂ����ē���\�� sname ���g�p�\��
	'(�t���ɂ��e���𖳎������ꍇ)
	Public Function IsSkillAvailable2(ByRef sname As String) As Boolean
		Dim sd As SkillData
		
		On Error GoTo ErrorHandler
		sd = colSkill.Item(sname)
		IsSkillAvailable2 = True
		Exit Function
		
ErrorHandler: 
		IsSkillAvailable2 = False
	End Function
	
	'���݂̃��x���ɂ��������\�� Index �̃��x��
	'�f�[�^�Ń��x���w�肪�Ȃ��ꍇ�̓��x�� 1
	'����\�͂��g�p�s�\�̏ꍇ�̓��x�� 0
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
		
		If ref_mode = "�C���l" Then
			SkillLevel = 0
		ElseIf ref_mode = "��{�l" Then 
			Exit Function
		End If
		
		'�d���\�Ȕ\�͓͂���\�͕t���Œu��������ꂱ�Ƃ͂Ȃ�
		Select Case sname
			Case "�n���^�[", "�r�o�����", "�X�y�V�����p���[��������"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'����\�͕t���������ɂ��C��
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
			
			If .IsConditionSatisfied(sname & "�t��") Then
				SkillLevel = .ConditionLevel(sname & "�t��")
				If SkillLevel = DEFAULT_LEVEL Then
					SkillLevel = 1
				End If
			ElseIf .IsConditionSatisfied(sname & "�t���Q") Then 
				SkillLevel = .ConditionLevel(sname & "�t���Q")
				If SkillLevel = DEFAULT_LEVEL Then
					SkillLevel = 1
				End If
			End If
			
			If .IsConditionSatisfied(sname & "����") Then
				SkillLevel = SkillLevel + .ConditionLevel(sname & "����")
			End If
			If .IsConditionSatisfied(sname & "�����Q") Then
				SkillLevel = SkillLevel + .ConditionLevel(sname & "�����Q")
			End If
		End With
	End Function
	
	'����\�� Index �Ƀ��x���w�肪�Ȃ���Ă��邩����
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
		
		'����\�͕t���������ɂ��C��
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
			
			If .IsConditionSatisfied(sname & "�t��") Then
				If .ConditionLevel(sname & "�t��") <> DEFAULT_LEVEL Then
					IsSkillLevelSpecified = True
				End If
			ElseIf .IsConditionSatisfied(sname & "�t���Q") Then 
				If .ConditionLevel(sname & "�t���Q") <> DEFAULT_LEVEL Then
					IsSkillLevelSpecified = True
				End If
			End If
			If .IsConditionSatisfied(sname & "����") Then
				IsSkillLevelSpecified = True
			ElseIf .IsConditionSatisfied(sname & "�����Q") Then 
				IsSkillLevelSpecified = True
			End If
		End With
	End Function
	
	'����\�͂̃f�[�^
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
		
		'�d���\�Ȕ\�͓͂���\�͕t���Œu��������ꂱ�Ƃ͂Ȃ�
		Select Case sname
			Case "�n���^�[", "�r�o�����", "�X�y�V�����p���[��������"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'����\�͕t���������ɂ��C��
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
			
			If .IsConditionSatisfied(sname & "�t��") Then
				SkillData = .ConditionData(sname & "�t��")
			ElseIf .IsConditionSatisfied(sname & "�t���Q") Then 
				SkillData = .ConditionData(sname & "�t���Q")
			End If
			
			If .IsConditionSatisfied(sname & "����") Then
				If Len(.ConditionData(sname & "����")) > 0 Then
					SkillData = .ConditionData(sname & "����")
				End If
			End If
			If .IsConditionSatisfied(sname & "�����Q") Then
				If Len(.ConditionData(sname & "�����Q")) > 0 Then
					SkillData = .ConditionData(sname & "�����Q")
				End If
			End If
		End With
	End Function
	
	'����\�̖͂���
	Public Function SkillName(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'�p�C���b�g�����L���Ă������\�͂̒����猟��
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'�\�͋����n�͔�\��
			If Right(sname, 2) = "�t�o" Or Right(sname, 4) = "�c�n�v�m" Then
				SkillName = "��\��"
				Exit Function
			End If
			
			Select Case sname
				Case "�ǉ����x��", "���b�Z�[�W", "���͏��L"
					'��\���̔\��
					SkillName = "��\��"
					Exit Function
				Case "���ӋZ", "�s����"
					'�ʖ��w�肪���݂��Ȃ��\��
					SkillName = sname
					Exit Function
			End Select
			
			If Len(.StrData) > 0 Then
				SkillName = LIndex(.StrData, 1)
				Select Case SkillName
					Case "��\��"
						Exit Function
					Case "���"
						SkillName = "��\��"
						Exit Function
				End Select
			Else
				SkillName = sname
			End If
			
			'���x���w��
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
		
		If sname = "�ϋv" Then
			If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
				'�h��͐����I�v�V�����g�p���ɂ͑ϋv�\�͂��\��
				SkillName = "��\��"
				Exit Function
			End If
		End If
		
		'���ӋZ���s����͖��̕ύX����Ȃ�
		Select Case sname
			Case "���ӋZ", "�s����"
				SkillName = sname
				Exit Function
		End Select
		
		'SetSkill�R�}���h�ŕ��󂳂�Ă���ꍇ
		If SkillName = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
				'�I���W�i���̖��̂��g�p
				SkillName = Data.SkillName(Level, sname)
				
				If InStr(SkillName, "��\��") > 0 Then
					SkillName = "��\��"
					Exit Function
				End If
			End If
		End If
		
		'�d���\�Ȕ\�͓͂���\�͕t���Ŗ��̂��u��������ꂱ�Ƃ͂Ȃ�
		Select Case sname
			Case "�n���^�[", "�X�y�V�����p���[��������"
				If IsNumeric(Index) Then
					If Left(SkillName, 1) = "(" Then
						SkillName = Mid(SkillName, 2)
						SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
					End If
					Exit Function
				End If
			Case "�r�o�����"
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
		
		'����\�͕t���������ɂ��C��
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'���j�b�g�p����\�͂ɂ��t��
						If .IsConditionSatisfied(sname & "�t���Q") Then
							buf = LIndex(.ConditionData(sname & "�t���Q"), 1)
							
							If buf <> "" Then
								SkillName = buf
							ElseIf SkillName = "" Then 
								SkillName = sname
							End If
							
							If InStr(SkillName, "��\��") > 0 Then
								SkillName = "��\��"
								Exit Function
							End If
							
							'���x���w��
							If .ConditionLevel(sname & "�t���Q") <> DEFAULT_LEVEL Then
								If InStr(SkillName, "Lv") > 0 Then
									SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
								End If
								SkillName = SkillName & "Lv" & VB6.Format(.ConditionLevel(sname & "�t���Q"))
							End If
						End If
						
						'�A�r���e�B�ɂ��t��
						If .IsConditionSatisfied(sname & "�t��") Then
							buf = LIndex(.ConditionData(sname & "�t��"), 1)
							
							If buf <> "" Then
								SkillName = buf
							ElseIf SkillName = "" Then 
								SkillName = sname
							End If
							
							If InStr(SkillName, "��\��") > 0 Then
								SkillName = "��\��"
								Exit Function
							End If
							
							'���x���w��
							If .ConditionLevel(sname & "�t��") <> DEFAULT_LEVEL Then
								If InStr(SkillName, "Lv") > 0 Then
									SkillName = Left(SkillName, InStr(SkillName, "Lv") - 1)
								End If
								SkillName = SkillName & "Lv" & VB6.Format(.ConditionLevel(sname & "�t��"))
							End If
						End If
						
						'���j�b�g�p����\�͂ɂ�鋭��
						If .IsConditionSatisfied(sname & "�����Q") Then
							If SkillName = "" Then
								'���������\�͂��p�C���b�g�������Ă��Ȃ������ꍇ
								SkillName = LIndex(.ConditionData(sname & "�����Q"), 1)
								
								If SkillName = "" Then
									SkillName = sname
								End If
								
								If InStr(SkillName, "��\��") > 0 Then
									SkillName = "��\��"
									Exit Function
								End If
								
								SkillName = SkillName & "Lv0"
							End If
							
							If sname <> "������" And sname <> "���" Then
								If .ConditionLevel(sname & "�����Q") >= 0 Then
									SkillName = SkillName & "+" & VB6.Format(.ConditionLevel(sname & "�����Q"))
								Else
									SkillName = SkillName & VB6.Format(.ConditionLevel(sname & "�����Q"))
								End If
							End If
						End If
						
						'�A�r���e�B�ɂ�鋭��
						If .IsConditionSatisfied(sname & "����") Then
							If SkillName = "" Then
								'���������\�͂��p�C���b�g�������Ă��Ȃ������ꍇ
								SkillName = LIndex(.ConditionData(sname & "����"), 1)
								
								If SkillName = "" Then
									SkillName = sname
								End If
								
								If InStr(SkillName, "��\��") > 0 Then
									SkillName = "��\��"
									Exit Function
								End If
								
								SkillName = SkillName & "Lv0"
							End If
							
							If sname <> "������" And sname <> "���" Then
								If .ConditionLevel(sname & "����") >= 0 Then
									SkillName = SkillName & "+" & VB6.Format(.ConditionLevel(sname & "����"))
								Else
									SkillName = SkillName & VB6.Format(.ConditionLevel(sname & "����"))
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'�\�͋����n�͔�\��
		If Right(sname, 2) = "�t�o" Or Right(sname, 4) = "�c�n�v�m" Then
			SkillName = "��\��"
			Exit Function
		End If
		
		Select Case sname
			Case "�ǉ����x��", "���b�Z�[�W", "���͏��L"
				'��\���̔\��
				SkillName = "��\��"
				Exit Function
			Case "�ϋv"
				If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
					'�h��͐����I�v�V�����g�p���ɂ͑ϋv�\�͂��\��
					SkillName = "��\��"
					Exit Function
				End If
		End Select
		
		'�����̔\�͂���̓��x���w�������
		Select Case sname
			Case "�K��", "������", "���", "�r�o�����"
				i = InStr(SkillName, "Lv")
				If i > 0 Then
					SkillName = Left(SkillName, i - 1)
				End If
		End Select
		
		'���x����\���p�̊��ʂ��폜
		If Left(SkillName, 1) = "(" Then
			SkillName = Mid(SkillName, 2)
			SkillName = Left(SkillName, InStr2(SkillName, ")") - 1)
		End If
		
		If SkillName = "" Then
			SkillName = sname
		End If
	End Function
	
	'����\�͖��́i���x���\�������j
	Public Function SkillName0(ByRef Index As Object) As String
		Dim sd As SkillData
		Dim sname As String
		Dim buf As String
		Dim i As Short
		
		'�p�C���b�g�����L���Ă������\�͂̒����猟��
		On Error GoTo ErrorHandler
		sd = colSkill.Item(Index)
		With sd
			sname = .Name
			
			'�\�͋����n�͔�\��
			If Right(sname, 2) = "�t�o" Or Right(sname, 4) = "�c�n�v�m" Then
				SkillName0 = "��\��"
				Exit Function
			End If
			
			Select Case sname
				Case "�ǉ����x��", "���b�Z�[�W", "���͏��L"
					'��\���̔\��
					SkillName0 = "��\��"
					Exit Function
				Case "���ӋZ", "�s����"
					'�ʖ��w�肪���݂��Ȃ��\��
					SkillName0 = sname
					Exit Function
			End Select
			
			If Len(.StrData) > 0 Then
				SkillName0 = LIndex(.StrData, 1)
				
				If SkillName0 = "��\��" Then
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
		
		If sname = "�ϋv" Then
			If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
				'�h��͐����I�v�V�����g�p���ɂ͑ϋv�\�͂��\��
				SkillName0 = "��\��"
				Exit Function
			End If
		End If
		
		'���ӋZ���s����͖��̕ύX����Ȃ�
		Select Case sname
			Case "���ӋZ", "�s����"
				SkillName0 = sname
				Exit Function
		End Select
		
		'SetSkill�R�}���h�ŕ��󂳂�Ă���ꍇ
		If SkillName0 = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
				'�I���W�i���̖��̂��g�p
				SkillName0 = Data.SkillName(Level, sname)
				
				If InStr(SkillName0, "��\��") > 0 Then
					SkillName0 = "��\��"
					Exit Function
				End If
			End If
		End If
		
		'�d���\�Ȕ\�͓͂���\�͕t���Ŗ��̂��u��������ꂱ�Ƃ͂Ȃ�
		Select Case sname
			Case "�n���^�[", "�r�o�����", "�X�y�V�����p���[��������"
				If IsNumeric(Index) Then
					Exit Function
				End If
		End Select
		
		'����\�͕t���������ɂ��C��
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If .MainPilot Is Me Or .Pilot(1) Is Me Then
						'���j�b�g�p����\�͂ɂ��t��
						If .IsConditionSatisfied(sname & "�t���Q") Then
							buf = LIndex(.ConditionData(sname & "�t���Q"), 1)
							
							If buf <> "" Then
								SkillName0 = buf
							ElseIf SkillName0 = "" Then 
								SkillName0 = sname
							End If
							
							If InStr(SkillName0, "��\��") > 0 Then
								SkillName0 = "��\��"
								Exit Function
							End If
						End If
						
						'�A�r���e�B�ɂ��t��
						If .IsConditionSatisfied(sname & "�t��") Then
							buf = LIndex(.ConditionData(sname & "�t��"), 1)
							
							If buf <> "" Then
								SkillName0 = buf
							ElseIf SkillName0 = "" Then 
								SkillName0 = sname
							End If
							
							If InStr(SkillName0, "��\��") > 0 Then
								SkillName0 = "��\��"
								Exit Function
							End If
						End If
						
						'���j�b�g�p����\�͂ɂ�鋭��
						If SkillName0 = "" Then
							If .IsConditionSatisfied(sname & "�����Q") Then
								SkillName0 = LIndex(.ConditionData(sname & "�����Q"), 1)
								
								If SkillName0 = "" Then
									SkillName0 = sname
								End If
								
								If InStr(SkillName0, "��\��") > 0 Then
									SkillName0 = "��\��"
									Exit Function
								End If
							End If
						End If
						
						'�A�r���e�B�ɂ�鋭��
						If SkillName0 = "" Then
							If .IsConditionSatisfied(sname & "����") Then
								SkillName0 = LIndex(.ConditionData(sname & "����"), 1)
								
								If SkillName0 = "" Then
									SkillName0 = sname
								End If
								
								If InStr(SkillName0, "��\��") > 0 Then
									SkillName0 = "��\��"
									Exit Function
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'�Y��������̂�������΃G���A�X���猟��
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
		
		'�\�͋����n�͔�\��
		If Right(sname, 2) = "�t�o" Or Right(sname, 4) = "�c�n�v�m" Then
			SkillName0 = "��\��"
			Exit Function
		End If
		
		Select Case sname
			Case "�ǉ����x��", "���b�Z�[�W", "���͏��L"
				'��\���̔\��
				SkillName0 = "��\��"
				Exit Function
			Case "�ϋv"
				If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
					'�h��͐����I�v�V�����g�p���ɂ͑ϋv�\�͂��\��
					SkillName0 = "��\��"
					Exit Function
				End If
		End Select
		
		'���x����\���p�̊��ʂ��폜
		If Left(SkillName0, 1) = "(" Then
			SkillName0 = Mid(SkillName0, 2)
			SkillName0 = Left(SkillName0, InStr2(SkillName0, ")") - 1)
		End If
		
		'���x���w����폜
		i = InStr(SkillName0, "Lv")
		If i > 0 Then
			SkillName0 = Left(SkillName0, i - 1)
		End If
	End Function
	
	'����\�͖��́i�K�v�Z�\����p�j
	'���̂��烌�x���w����폜���A���̂���\���ɂ���Ă���ꍇ�͌��̓���\�͖�
	'�������̓G���A�X�����g�p����B
	Public Function SkillNameForNS(ByRef stype As String) As String
		Dim sd As SkillData
		Dim buf As String
		Dim i As Short
		
		'��\���̓���\��
		If Right(stype, 2) = "�t�o" Or Right(stype, 4) = "�c�n�v�m" Then
			SkillNameForNS = stype
			Exit Function
		End If
		If stype = "���b�Z�[�W" Then
			SkillNameForNS = stype
			Exit Function
		End If
		
		'�p�C���b�g�����L���Ă������\�͂̒����猟��
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
		
		'SetSkill�R�}���h�ŕ��󂳂�Ă���ꍇ
		If SkillNameForNS = "" Then
			If IsGlobalVariableDefined("Ability(" & ID & "," & stype & ")") Then
				'�I���W�i���̖��̂��g�p
				SkillNameForNS = Data.SkillName(Level, stype)
				
				If InStr(SkillNameForNS, "��\��") > 0 Then
					SkillNameForNS = "��\��"
				End If
			End If
		End If
		
		'����\�͕t���������ɂ��C��
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition > 0 And .CountPilot > 0 Then
					If Me Is .MainPilot Or Me Is .Pilot(1) Then
						'���j�b�g�p����\�͂ɂ��t��
						If .IsConditionSatisfied(stype & "�t���Q") Then
							buf = LIndex(.ConditionData(stype & "�t���Q"), 1)
							
							If buf <> "" Then
								SkillNameForNS = buf
							ElseIf SkillNameForNS = "" Then 
								SkillNameForNS = stype
							End If
							
							If InStr(SkillNameForNS, "��\��") > 0 Then
								SkillNameForNS = "��\��"
							End If
						End If
						
						'�A�r���e�B�ɂ��t��
						If .IsConditionSatisfied(stype & "�t��") Then
							buf = LIndex(.ConditionData(stype & "�t��"), 1)
							
							If buf <> "" Then
								SkillNameForNS = buf
							ElseIf SkillNameForNS = "" Then 
								SkillNameForNS = stype
							End If
							
							If InStr(SkillNameForNS, "��\��") > 0 Then
								SkillNameForNS = "��\��"
							End If
						End If
						
						'���j�b�g�p����\�͂ɂ�鋭��
						If SkillNameForNS = "" Then
							If .IsConditionSatisfied(stype & "�����Q") Then
								SkillNameForNS = LIndex(.ConditionData(stype & "�����Q"), 1)
								
								If SkillNameForNS = "" Then
									SkillNameForNS = stype
								End If
								
								If InStr(SkillNameForNS, "��\��") > 0 Then
									SkillNameForNS = "��\��"
								End If
							End If
						End If
						
						'�A�r���e�B�ɂ�鋭��
						If SkillNameForNS = "" Then
							If .IsConditionSatisfied(stype & "����") Then
								SkillNameForNS = LIndex(.ConditionData(stype & "����"), 1)
								
								If SkillNameForNS = "" Then
									SkillNameForNS = stype
								End If
								
								If InStr(SkillNameForNS, "��\��") > 0 Then
									SkillNameForNS = "��\��"
								End If
							End If
						End If
					End If
				End If
			End With
		End If
		
		'�Y��������̂�������΃G���A�X���猟��
		If SkillNameForNS = "" Or SkillNameForNS = "��\��" Then
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
		
		'���x����\���p�̊��ʂ��폜
		If Left(SkillNameForNS, 1) = "(" Then
			SkillNameForNS = Mid(SkillNameForNS, 2)
			SkillNameForNS = Left(SkillNameForNS, InStr2(SkillNameForNS, ")") - 1)
		End If
		
		'���x���\�����폜
		i = InStr(SkillNameForNS, "Lv")
		If i > 0 Then
			SkillNameForNS = Left(SkillNameForNS, i - 1)
		End If
	End Function
	
	'����\�͂̎��
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
		
		'�G���A�X�f�[�^����`����Ă���H
		If ALDList.IsDefined(sname0) Then
			With ALDList.Item(sname0)
				SkillType = .AliasType(1)
				Exit Function
			End With
		End If
		
		'����\�͈ꗗ���猟��
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
		
		'���̔\�͂��C�����Ă��Ȃ�
		SkillType = sname0
		
		'����\�͕t���ɂ��C��
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountCondition And .CountPilot > 0 Then
					If Me Is .MainPilot Or Me Is .Pilot(1) Then
						For i = 1 To .CountCondition
							If Right(.Condition(i), 2) = "�t��" Then
								If LIndex(.ConditionData(i), 1) = sname0 Then
									SkillType = .Condition(i)
									SkillType = Left(SkillType, Len(SkillType) - 2)
									Exit For
								End If
							ElseIf Right(.Condition(i), 3) = "�t���Q" Then 
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
	
	'�X�y�V�����p���[ sname ���C�����Ă��邩�H
	Public Function IsSpecialPowerAvailable(ByRef sname As String) As Boolean
		If Data.SP <= 0 Then
			'�r�o�������Ȃ��ǉ��p�C���b�g�̏ꍇ�͂P�Ԗڂ̃p�C���b�g�̃f�[�^���g��
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
	
	'�X�y�V�����p���[ sname ���L�p���H
	Public Function IsSpecialPowerUseful(ByRef sname As String) As Boolean
		IsSpecialPowerUseful = SPDList.Item(sname).Useful(Me)
	End Function
	
	'�X�y�V�����p���[ sname �ɕK�v�Ȃr�o�l
	Public Function SpecialPowerCost(ByRef sname As String) As Short
		Dim i, j As Short
		Dim adata As String
		
		If Data.SP <= 0 Then
			'�r�o�������Ȃ��ǉ��p�C���b�g�̏ꍇ�͂P�Ԗڂ̃p�C���b�g�̃f�[�^���g��
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
		
		'��{����r�o�l
		SpecialPowerCost = Data.SpecialPowerCost(sname)
		
		'����\�͂ɂ�����r�o�l�C��
		If IsSkillAvailable("���\��") Or IsSkillAvailable("�W����") Then
			SpecialPowerCost = 0.8 * SpecialPowerCost
		End If
		If IsSkillAvailable("�m�o����") Then
			SpecialPowerCost = 1.2 * SpecialPowerCost
		End If
		
		'�r�o������\��
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .CountPilot > 0 Then
					If .MainPilot Is Me Then
						If .IsConditionSatisfied("�r�o������t��") Or .IsConditionSatisfied("�r�o������t���Q") Then
							adata = SkillData("�r�o�����")
							For i = 2 To LLength(adata)
								If sname = LIndex(adata, i) Then
									SpecialPowerCost = (10 - SkillLevel("�r�o�����")) * SpecialPowerCost \ 10
									Exit Function
								End If
							Next 
						End If
					End If
				End If
			End With
		End If
		For i = 1 To CountSkill
			If Skill(i) = "�r�o�����" Then
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
	
	'�X�y�V�����p���[ sname �����s����
	Public Sub UseSpecialPower(ByRef sname As String, Optional ByVal sp_mod As Double = 1)
		Dim my_unit As Unit
		
		If Not SPDList.IsDefined(sname) Then
			Exit Sub
		End If
		
		ClearUnitStatus()
		
		SelectedPilot = Me
		
		'�X�y�V�����p���[�g�p���b�Z�[�W
		If sp_mod <> 2 And Not SPDList.Item(sname).IsEffectAvailable("����") And Not SPDList.Item(sname).IsEffectAvailable("����") Then
			If Unit_Renamed.IsMessageDefined(sname) Then
				OpenMessageForm()
				Unit_Renamed.PilotMessage(sname)
				CloseMessageForm()
			End If
		End If
		
		'�����ǉ��p�C���b�g�������j�b�g����������ꍇ�A�p�C���b�g��Unit��
		'�ω����Ă��܂����Ƃ����邽�߁A����Unit���L�^���Ă���
		my_unit = Unit_Renamed
		
		'�X�y�V�����p���[�A�j����\��
		If Not SPDList.Item(sname).PlayAnimation Then
			'���b�Z�[�W�\���̂�
			OpenMessageForm(Unit_Renamed)
			DisplaySysMessage(Nickname & "��" & sname & "���g�����B")
		End If
		
		'Unit���ω������ꍇ�Ɍ��ɖ߂�
		If Not my_unit Is Unit_Renamed Then
			my_unit.MainPilot()
		End If
		
		'�X�y�V�����p���[�����s
		SPDList.Item(sname).Execute(Me)
		
		'Unit���ω������ꍇ�Ɍ��ɖ߂�
		If Not my_unit Is Unit_Renamed Then
			my_unit.CurrentForm.MainPilot()
		End If
		
		SP = SP - sp_mod * SpecialPowerCost(sname)
		
		CloseMessageForm()
	End Sub
	
	
	' === ���j�b�g���恕����֘A���� ===
	
	'���j�b�g u �ɓ���
	Public Sub Ride(ByRef u As Unit, Optional ByVal is_support As Boolean = False)
		Dim hp_ratio, en_ratio As Double
		Dim plana_ratio As Double
		
		'���ɏ���Ă���΂Ȃɂ����Ȃ�
		If Unit_Renamed Is u Then
			Exit Sub
		End If
		
		With u
			hp_ratio = 100 * .HP / .MaxHP
			en_ratio = 100 * .EN / .MaxEN
			
			'���݂̗�͒l���L�^
			If MaxPlana > 0 Then
				plana_ratio = 100 * Plana / MaxPlana
			Else
				plana_ratio = -1
			End If
			
			Unit_Renamed = u
			
			If InStrNotNest(Class_Renamed, "�T�|�[�g)") > 0 And LLength(Class_Renamed) = 1 And Not .IsFeatureAvailable("�_�~�[���j�b�g") Then
				'�T�|�[�g�ɂ����Ȃ�Ȃ��p�C���b�g�̏ꍇ
				.AddSupport(Me)
			ElseIf IsSupport(u) Then 
				'�������j�b�g�N���X�ɑ΂��Ēʏ�p�C���b�g�ƃT�|�[�g�̗����̃p�^�[��
				'��������ꍇ�͒ʏ�p�C���b�g��D��
				If .CountPilot < System.Math.Abs(.Data.PilotNum) And InStrNotNest(Class_Renamed, u.Class0 & " ") > 0 And Not is_support Then
					.AddPilot(Me)
				Else
					.AddSupport(Me)
				End If
			Else
				'�p�C���b�g�����ɋK�萔�̏ꍇ�͑S�p�C���b�g���~�낷
				If .CountPilot = System.Math.Abs(.Data.PilotNum) Then
					.Pilot(1).GetOff()
				End If
				.AddPilot(Me)
			End If
			
			'Pilot�R�}���h�ō쐬���ꂽ�p�C���b�g�͑S�Ė����Ȃ̂œ��掞�ɕύX���K�v
			Party = .Party0
			
			'���j�b�g�̃X�e�[�^�X���A�b�v�f�[�g
			.Update()
			
			'��͒l�̃A�b�v�f�[�g
			If plana_ratio >= 0 Then
				Plana = MaxPlana * plana_ratio \ 100
			Else
				Plana = MaxPlana
			End If
			
			'�p�C���b�g����荞�ނ��Ƃɂ��g�o���d�m�̑����ɑΉ�
			.HP = .MaxHP * hp_ratio \ 100
			.EN = .MaxEN * en_ratio \ 100
		End With
	End Sub
	
	'�p�C���b�g�����j�b�g����~�낷
	Public Sub GetOff(Optional ByVal without_leave As Boolean = False)
		Dim i As Short
		
		'���ɍ~��Ă���H
		If Unit_Renamed Is Nothing Then
			Exit Sub
		End If
		
		With Unit_Renamed
			For i = 1 To .CountSupport
				If .Support(i) Is Me Then
					'�T�|�[�g�p�C���b�g�Ƃ��ď�荞��ł���ꍇ
					.DeleteSupport(i)
					.Update()
					'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					Unit_Renamed = Nothing
					Update()
					Exit Sub
				End If
			Next 
			
			'�o�����Ă����ꍇ�͑ދp
			If Not without_leave Then
				If .Status = "�o��" Then
					.Status = "�ҋ@"
					'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					MapDataForUnit(.X, .Y) = Nothing
					EraseUnitBitmap(.X, .Y, False)
				End If
			End If
			
			'�ʏ�̃p�C���b�g�̏ꍇ�́A���̃��j�b�g�ɏ���Ă������̃p�C���b�g���~�낳���
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
	
	'�p�C���b�g�����j�b�g u �̃T�|�[�g���ǂ���
	Public Function IsSupport(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i, j As Short
		
		With u
			If .IsFeatureAvailable("�_�~�[���j�b�g") Then
				'�_�~�[���j�b�g�̏ꍇ�̓T�|�[�g�p�C���b�g���ʏ�̃p�C���b�g�Ƃ��Ĉ���
				IsSupport = False
				Exit Function
			End If
			
			'�T�|�[�g�w�肪���݂���H
			If InStrNotNest(Class_Renamed, "�T�|�[�g)") = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			If .CountPilot = 0 Then
				'�p�C���b�g������Ă��Ȃ����j�b�g�̏ꍇ�͒ʏ�p�C���b�g��D��
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If .Class_Renamed = pclass Or .Class_Renamed = pclass & "(" & Name & "��p)" Or .Class_Renamed = pclass & "(" & Nickname & "��p)" Or .Class_Renamed = pclass & "(" & Sex & "��p)" Then
						'�ʏ�̃p�C���b�g�Ƃ��ē���\�ł���΃T�|�[�g�łȂ��Ƃ݂Ȃ�
						IsSupport = False
						Exit Function
					End If
				Next 
			Else
				'�ʏ�̃p�C���b�g�Ƃ��ē��悵�Ă���H
				For i = 1 To .CountPilot
					If .Pilot(i) Is Me Then
						IsSupport = False
						Exit Function
					End If
				Next 
			End If
			
			uclass = .Class0
			
			'�ʏ�̃T�|�[�g�H
			For i = 1 To LLength(Class_Renamed)
				If uclass & "(�T�|�[�g)" = LIndex(Class_Renamed, i) Then
					IsSupport = True
					Exit Function
				End If
			Next 
			
			'�p�C���b�g������Ă��Ȃ����j�b�g�̏ꍇ�͂����ŏI��
			If .CountPilot = 0 Then
				IsSupport = False
				Exit Function
			End If
			
			'�ꑮ�T�|�[�g�H
			With .MainPilot
				For i = 1 To LLength(Class_Renamed)
					pclass = LIndex(Class_Renamed, i)
					If pclass = uclass & "(" & .Name & "�ꑮ�T�|�[�g)" Or pclass = uclass & "(" & .Nickname & "�ꑮ�T�|�[�g)" Or pclass = uclass & "(" & .Sex & "�ꑮ�T�|�[�g)" Then
						IsSupport = True
						Exit Function
					End If
					
					For j = 1 To .CountSkill
						If pclass = uclass & "(" & .Skill(j) & "�ꑮ�T�|�[�g)" Then
							IsSupport = True
							Exit Function
						End If
					Next 
				Next 
			End With
		End With
		
		IsSupport = False
	End Function
	
	'���j�b�g u �ɏ�邱�Ƃ��ł��邩�ǂ���
	Public Function IsAbleToRide(ByRef u As Unit) As Boolean
		Dim uclass, pclass As String
		Dim i As Short
		
		With u
			'�ėp���j�b�g�͕K�v�Z�\�𖞂����΂n�j
			If .Class_Renamed = "�ėp" Then
				IsAbleToRide = True
				GoTo CheckNecessarySkill
			End If
			
			'�l�ԃ��j�b�g�w��������Ĕ���
			If Left(.Class_Renamed, 1) = "(" And Right(.Class_Renamed, 1) = ")" Then
				uclass = Mid(.Class_Renamed, 2, Len(.Class_Renamed) - 2)
			Else
				uclass = .Class_Renamed
			End If
			
			'�T�|�[�g���ǂ������܂����肵�Ă���
			If IsSupport(u) Then
				IsAbleToRide = True
				'�K�v�Z�\���`�F�b�N����
				GoTo CheckNecessarySkill
			End If
			
			For i = 1 To LLength(Class_Renamed) '���j�b�g�N���X�͕����ݒ�\
				pclass = LIndex(Class_Renamed, i)
				If uclass = pclass Or uclass = pclass & "(" & Nickname & "��p)" Or uclass = pclass & "(" & Name & "��p)" Or uclass = pclass & "(" & Sex & "��p)" Then
					IsAbleToRide = True
					'�K�v�Z�\���`�F�b�N����
					GoTo CheckNecessarySkill
				End If
			Next 
			
			'�N���X������Ȃ�
			IsAbleToRide = False
			Exit Function
			
CheckNecessarySkill: 
			
			'�K�v�Z�\���s�K�v�Z�\���`�F�b�N
			
			'���\�͂������Ă��Ȃ��ꍇ�̓`�F�b�N�s�v
			If Not .IsFeatureAvailable("�K�v�Z�\") And Not .IsFeatureAvailable("�s�K�v�Z�\") Then
				Exit Function
			End If
			
			For i = 1 To .CountFeature
				If .Feature(i) = "�K�v�Z�\" Then
					If Not .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				ElseIf .Feature(i) = "�s�K�v�Z�\" Then 
					If .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
						IsAbleToRide = False
						Exit Function
					End If
				End If
			Next 
		End With
	End Function
	
	
	' === �ꎞ���f�֘A���� ===
	
	'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
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
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
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
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
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
	
	'�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
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
	
	
	' === ���̑� ===
	
	'�S��
	Public Sub FullRecover()
		'�����{�\�ɂ���ď����C�͕͂ω�����
		If IsSkillAvailable("�����{�\") Then
			If MinMorale > 100 Then
				SetMorale(MinMorale + 5 * SkillLevel("�����{�\"))
			Else
				SetMorale(100 + 5 * SkillLevel("�����{�\"))
			End If
		Else
			SetMorale(100)
		End If
		
		SP = MaxSP
		Plana = MaxPlana
	End Sub
	
	'������
	Public Function SynchroRate() As Short
		Dim lv As Short
		
		If Not IsSkillAvailable("������") Then
			Exit Function
		End If
		
		'��������{�l
		SynchroRate = SkillLevel("������")
		
		'���x���ɂ�鑝����
		lv = MinLng(Level, 100)
		If IsSkillAvailable("����������") Then
			SynchroRate = SynchroRate + lv * (10 + SkillLevel("����������")) \ 10
		Else
			SynchroRate = SynchroRate + lv
		End If
	End Function
	
	'�w���͈�
	Public Function CommandRange() As Short
		'�w���\�͂������Ă��Ȃ���Δ͈͂�0
		If Not IsSkillAvailable("�w��") Then
			CommandRange = 0
			Exit Function
		End If
		
		'�w���\�͂������Ă���ꍇ�͊K�����x���Ɉˑ�
		Select Case SkillLevel("�K��")
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
	
	'�s������ɗp������퓬���f��
	Public Function TacticalTechnique0() As Short
		TacticalTechnique0 = TechniqueBase - Level + 10 * SkillLevel("��p")
	End Function
	
	Public Function TacticalTechnique() As Short
		'����Ȕ��f�\�͂�����H
		If Not Unit_Renamed Is Nothing Then
			With Unit_Renamed
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("����m") Then
					Exit Function
				End If
			End With
		End If
		
		TacticalTechnique = TacticalTechnique0
	End Function
	
	'�C�x���g�R�}���h SetRelation �Őݒ肵���l��Ԃ�
	Public Function Relation(ByRef t As Pilot) As Short
		Relation = GetValueAsLong("�֌W:" & Name & ":" & t.Name)
	End Function
	
	'�ˌ��\�͂��u���́v�ƕ\������邩�ǂ���
	Public Function HasMana() As Boolean
		If IsSkillAvailable("�p") Or IsSkillAvailable("���͏��L") Then
			HasMana = True
			Exit Function
		End If
		
		If IsOptionDefined("���͎g�p") Then
			HasMana = True
			Exit Function
		End If
		
		HasMana = False
	End Function
End Class