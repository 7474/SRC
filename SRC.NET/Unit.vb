Option Strict Off
Option Explicit On
Friend Class Unit
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�f�[�^
	Public Data As UnitData
	
	'���ʗp�h�c
	Public ID As String
	
	'�r�b�g�}�b�vID
	'����̃��j�b�g�͓���ID�����L
	Public BitmapID As Short
	
	'�w���W
	Public x As Short
	'�x���W
	Public y As Short
	
	'���j�b�g�̏ꏊ�i�n��A����A�����A�󒆁A�n���A�F���j
	Public Area As String
	
	'�g�p�ςݍs����
	Public UsedAction As Short
	
	'�v�l���[�h
	Private strMode As String
	
	'�X�e�[�^�X
	'�o���F�}�b�v��ɏo��
	'���`�ԁF���̌`�Ԃɕό`(�n�C�p�[���[�h)��
	'�j��F�j�󂳂�Ă���
	'�j���F�C�x���g�R�}���h RemoveUnit �Ȃǂɂ��C�x���g�Ŕj�󂳂�Ă���
	'�i�[�F��͓��Ɋi�[����Ă���
	'�ҋ@�F�ҋ@��
	'���`�ԁF�������j�b�g�����̑O�Ɏ���Ă����`��
	'���E�FLeave�R�}���h�ɂ�����𗣒E�BOrganize�R�}���h�ł��\������Ȃ�
	'UPGRADE_NOTE: Status �� Status_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Status_Renamed As String
	
	'���j�b�g�ɑ΂��Ďg�p����Ă���X�y�V�����p���[
	Private colSpecialPowerInEffect As New Collection
	
	'�T�|�[�g�A�^�b�N���K�[�h�̎g�p��
	Public UsedSupportAttack As Short
	Public UsedSupportGuard As Short
	
	'��������U���̎g�p��
	Public UsedSyncAttack As Short
	
	'�J�E���^�[�U���̎g�p��
	Public UsedCounterAttack As Short
	
	'���j�b�g����
	Private strName As String
	'�w�c
	Private strParty As String
	'���j�b�g�����N
	Private intRank As Short
	'�{�X�����N
	Private intBossRank As Short
	'�g�o
	Private lngMaxHP As Integer
	Private lngHP As Integer
	'�d�m
	Private intMaxEN As Short
	Private intEN As Short
	'���b
	Private lngArmor As Integer
	'�^����
	Private intMobility As Short
	'�ړ���
	Private intSpeed As Short
	
	'���悵�Ă���p�C���b�g
	Private colPilot As New Collection
	
	'���悵�Ă���T�|�[�g�p�C���b�g
	Private colSupport As New Collection
	
	'�֘A���郆�j�b�g
	'�ό`���j�b�g�ɂ����鑼�`�ԓ�
	Private colOtherForm As New Collection
	
	'�i�[�������j�b�g
	Private colUnitOnBoard As New Collection
	
	'�������Ă���A�C�e��
	Private colItem As New Collection
	
	'���݂̓���X�e�[�^�X
	Private colCondition As New Collection
	
	'�e����̎c�e��
	Private dblBullet() As Double
	
	'�A�r���e�B�̎c��g�p��
	Private dblStock() As Double
	
	'����\��
	Private colFeature As New Collection
	
	'����\��(�K�v�����𖞂����Ȃ����̂��܂�)
	Private colAllFeature As New Collection
	
	'�t�����ꂽ����\�͐�
	Public AdditionalFeaturesNum As Short
	
	'�n�`�K��
	Private strAdaption As String
	
	'�U���ւ̑ϐ�
	Public strAbsorb As String
	Public strImmune As String
	Public strResist As String
	Public strWeakness As String
	Public strEffective As String
	Public strSpecialEffectImmune As String
	
	'����f�[�^
	Private WData() As WeaponData
	Private lngWeaponPower() As Integer
	Private intWeaponMaxRange() As Short
	Private intWeaponPrecision() As Short
	Private intWeaponCritical() As Short
	Private strWeaponClass() As String
	Private intMaxBullet() As Short
	
	'�A�r���e�B�f�[�^
	Private adata() As AbilityData
	
	'�I�������}�b�v�U���̍U����
	Private SelectedMapAttackPower As Integer
	
	'�I�������}�b�v�U���̍U����
	Private IsMapAttackCanceled As Boolean
	
	'�����������j�b�g
	Private colServant As New Collection
	
	'�����E�߈˂������j�b�g
	Private colSlave As New Collection
	
	'������
	Public Summoner As Unit
	
	'����l�l
	Public Master As Unit
	
	'�ǉ��p�C���b�g
	Public pltAdditionalPilot As Pilot
	
	'�ǉ��T�|�[�g
	Public pltAdditionalSupport As Pilot
	
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		Status_Renamed = "�ҋ@"
		intBossRank = -1
		'UPGRADE_NOTE: �I�u�W�F�N�g Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Summoner = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Master = Nothing
		ReDim WData(0)
		ReDim adata(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		'UPGRADE_NOTE: �I�u�W�F�N�g Data ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Data = Nothing
		
		With colPilot
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colPilot = Nothing
		
		With colSupport
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colSupport = Nothing
		
		With colOtherForm
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colOtherForm ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colOtherForm = Nothing
		
		With colItem
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colItem ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colItem = Nothing
		
		With colCondition
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colCondition ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colCondition = Nothing
		
		With colFeature
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colFeature ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colFeature = Nothing
		
		For i = 1 To UBound(WData)
			'UPGRADE_NOTE: �I�u�W�F�N�g WData() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			WData(i) = Nothing
		Next 
		
		For i = 1 To UBound(adata)
			'UPGRADE_NOTE: �I�u�W�F�N�g adata() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			adata(i) = Nothing
		Next 
		
		With colUnitOnBoard
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colUnitOnBoard ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colUnitOnBoard = Nothing
		
		With colServant
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colServant ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colServant = Nothing
		
		With colSlave
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colSlave ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colSlave = Nothing
		
		'UPGRADE_NOTE: �I�u�W�F�N�g Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Summoner = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Master = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g pltAdditionalPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		pltAdditionalPilot = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g pltAdditionalSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		pltAdditionalSupport = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	' === �e���{�X�e�[�^�X ===
	
	'���j�b�g����
	
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
	
	'����
	Public ReadOnly Property Nickname0() As String
		Get
			Dim idx As Short
			Dim u As Unit
			
			Nickname0 = Data.Nickname
			
			'���̕ύX�\�͂ɂ��ύX
			If IsFeatureAvailable("���̕ύX") Then
				Nickname0 = FeatureData("���̕ύX")
				idx = InStr(Nickname0, "$(����)")
				If idx > 0 Then
					Nickname0 = Left(Nickname0, idx - 1) & Data.Nickname & Mid(Nickname0, idx + 5)
				End If
				idx = InStr(Nickname0, "$(�p�C���b�g����)")
				If idx > 0 Then
					If CountPilot > 0 Then
						Nickname0 = Left(Nickname0, idx - 1) & MainPilot.Nickname(True) & Mid(Nickname0, idx + 10)
					Else
						Nickname0 = Left(Nickname0, idx - 1) & "����" & Mid(Nickname0, idx + 10)
					End If
				End If
			End If
			
			u = SelectedUnitForEvent
			SelectedUnitForEvent = Me
			ReplaceSubExpression(Nickname0)
			SelectedUnitForEvent = u
		End Get
	End Property
	
	'���b�Z�[�W���ŕ\������ۂ̈��͓̂��g���ł̓p�C���b�g�����g��
	Public ReadOnly Property Nickname() As String
		Get
			If IsOptionDefined("���g��") Then
				If CountPilot() > 0 Then
					With MainPilot
						If InStr(.Name, "(�U�R)") = 0 And InStr(.Name, "(�ėp)") = 0 Then
							Nickname = MainPilot.Nickname
							Exit Property
						End If
					End With
				End If
			End If
			Nickname = Nickname0
		End Get
	End Property
	
	'�ǂ݉���
	Public ReadOnly Property KanaName() As String
		Get
			Dim idx As Short
			Dim u As Unit
			
			KanaName = Data.KanaName
			
			'�ǂ݉����ύX�\�͂ɂ��ύX
			If IsFeatureAvailable("�ǂ݉����ύX") Then
				KanaName = FeatureData("�ǂ݉����ύX")
				idx = InStr(KanaName, "$(�ǂ݉���)")
				If idx > 0 Then
					KanaName = Left(KanaName, idx - 1) & Data.KanaName & Mid(KanaName, idx + 5)
				End If
				idx = InStr(KanaName, "$(�p�C���b�g�ǂ݉���)")
				If idx > 0 Then
					If CountPilot > 0 Then
						KanaName = Left(KanaName, idx - 1) & MainPilot.KanaName & Mid(KanaName, idx + 10)
					Else
						KanaName = Left(KanaName, idx - 1) & "����" & Mid(KanaName, idx + 10)
					End If
				End If
			ElseIf IsFeatureAvailable("���̕ύX") Then 
				KanaName = FeatureData("���̕ύX")
				idx = InStr(KanaName, "$(����)")
				If idx > 0 Then
					KanaName = Left(KanaName, idx - 1) & Data.Nickname & Mid(KanaName, idx + 5)
				End If
				idx = InStr(KanaName, "$(�p�C���b�g����)")
				If idx > 0 Then
					If CountPilot > 0 Then
						KanaName = Left(KanaName, idx - 1) & MainPilot.Nickname & Mid(KanaName, idx + 10)
					Else
						KanaName = Left(KanaName, idx - 1) & "����" & Mid(KanaName, idx + 10)
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
	
	'���j�b�g�����N
	
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
			
			'�p�����[�^���X�V
			Update()
		End Set
	End Property
	
	'�{�X�����N
	
	Public Property BossRank() As Short
		Get
			BossRank = intBossRank
		End Get
		Set(ByVal Value As Short)
			If intBossRank = Value Then
				Exit Property
			End If
			
			intBossRank = Value
			
			'�p�����[�^���X�V
			Update()
		End Set
	End Property
	
	'���j�b�g�N���X
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public ReadOnly Property Class_Renamed() As String
		Get
			Class_Renamed = Data.Class_Renamed
		End Get
	End Property
	
	'���j�b�g�N���X����]���Ȏw�������������
	Public ReadOnly Property Class0() As String
		Get
			Dim i, n As Short
			
			Class0 = Data.Class_Renamed
			
			'�l�ԃ��j�b�g�w����폜
			If Left(Class0, 1) = "(" Then
				Class0 = Mid(Class0, 2, Len(Class0) - 2)
			End If
			
			'��p�w����폜
			If Right(Class0, 3) = "��p)" Then
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
	
	
	'���j�b�g�T�C�Y
	Public ReadOnly Property Size() As String
		Get
			If IsFeatureAvailable("�T�C�Y�ύX") Then
				Size = FeatureData("�T�C�Y�ύX")
				Exit Property
			End If
			
			Size = Data.Size
		End Get
	End Property
	
	
	'�ő�g�o�l
	Public ReadOnly Property MaxHP() As Integer
		Get
			MaxHP = lngMaxHP
			
			'�p�C���b�g�ɂ��C��
			If CountPilot > 0 Then
				'��͕ϊ��푕�����j�b�g�͗�͂ɉ����čő�g�o���ω�
				If IsFeatureAvailable("��͕ϊ���") Then
					MaxHP = MaxHP + 10 * PlanaLevel()
				End If
				
				'�I�[���ϊ��푕�����j�b�g�̓I�[�����x���ɉ����čő�g�o���ω�
				If IsFeatureAvailable("�I�[���ϊ���") Then
					MaxHP = MaxHP + 100 * AuraLevel()
				End If
			End If
			
			'�ő�g�o�͍Œ�ł�1
			If MaxHP < 1 Then
				MaxHP = 1
			End If
		End Get
	End Property
	
	'�ő�d�m�l
	Public ReadOnly Property MaxEN() As Short
		Get
			MaxEN = intMaxEN
			
			'�p�C���b�g�ɂ��C��
			If CountPilot > 0 Then
				'��͕ϊ��푕�����j�b�g�͗�͂ɉ����čő�d�m���ω�
				If IsFeatureAvailable("��͕ϊ���") Then
					MaxEN = MaxEN + 0.5 * PlanaLevel()
				End If
				
				'�I�[���ϊ��푕�����j�b�g�̓I�[�����x���ɉ����čő�d�m���ω�
				If IsFeatureAvailable("�I�[���ϊ���") Then
					MaxEN = MaxEN + 10 * AuraLevel()
				End If
			End If
			
			'�ő�d�m�͍Œ�ł�5
			If MaxEN < 5 Then
				MaxEN = 5
			End If
		End Get
	End Property
	
	'�g�o
	
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
	
	'�d�m
	
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
	
	
	'�ړ���
	Public ReadOnly Property Speed0() As Short
		Get
			Speed0 = intSpeed
		End Get
	End Property
	
	Public ReadOnly Property Speed() As Short
		Get
			Speed = Speed0
			
			'���X�ړ��͂�0�̏ꍇ��0�̂܂�
			If Speed = 0 And Not IsFeatureAvailable("�e���|�[�g") And Not IsFeatureAvailable("�W�����v") Then
				Exit Property
			End If
			
			'�����Ԃɂ��ړ��͏C��
			If IsUnderSpecialPowerEffect("�ړ��͋���") Then
				Speed = Speed + SpecialPowerEffectLevel("�ړ��͋���")
			ElseIf IsConditionSatisfied("�ړ��͂t�o") Then 
				If IsOptionDefined("��^�}�b�v") Then
					Speed = Speed + 2
				Else
					Speed = Speed + 1
				End If
			End If
			If IsConditionSatisfied("�ړ��͂c�n�v�m") Then
				Speed = MaxLng(Speed \ 2, 1)
			End If
			
			'��͂ɂ��ړ��͂t�o
			If IsFeatureAvailable("��͕ϊ���") Then
				If CountPilot > 0 Then
					Speed = Speed + Int(0.01 * PlanaLevel())
				End If
			End If
			
			'�X�y�V�����p���[�ɂ��ړ��͒ቺ
			If IsUnderSpecialPowerEffect("�ړ��͒ቺ") Then
				Speed = MaxLng(Speed \ 2, 1)
			End If
			
			'�ړ��s�\�̏ꍇ�͈ړ��͂O
			If IsConditionSatisfied("�ړ��s�\") Then
				Speed = 0
			End If
			
			'�d�m�؂�ɂ��ړ��ł��Ȃ��ꍇ
			If Status_Renamed = "�o��" Then
				Select Case Area
					Case "��", "�F��"
						If EN < 5 Then
							Speed = 0
						End If
					Case "�n��"
						If EN < 10 Then
							Speed = 0
						End If
				End Select
			End If
		End Get
	End Property
	
	
	'�ړ��`��
	Public ReadOnly Property Transportation() As String
		Get
			Transportation = Data.Transportation
			
			'����\�͂ɂ��ړ��\�n�`�ǉ�
			If IsFeatureAvailable("�󒆈ړ�") Then
				If InStr(Transportation, "��") = 0 Then
					Transportation = "��" & Transportation
				End If
			End If
			If IsFeatureAvailable("����ړ�") Then
				If InStr(Transportation, "��") = 0 Then
					Transportation = "��" & Transportation
				End If
			End If
			If IsFeatureAvailable("�����ړ�") Then
				If InStr(Transportation, "��") = 0 Then
					Transportation = "��" & Transportation
				End If
			End If
			If IsFeatureAvailable("�n���ړ�") Then
				If InStr(Transportation, "�n��") = 0 Then
					Transportation = Transportation & "�n��"
				End If
			End If
			If IsFeatureAvailable("�F���ړ�") Then
				If Transportation = "" Then
					Transportation = "�F��"
				End If
			End If
		End Get
	End Property
	
	
	'�n�`�K��
	Public ReadOnly Property Adaption(ByVal idx As Short) As Short
		Get
			Dim uad, pad As Short
			
			'���j�b�g���̒n�`�K�����Z�o
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
			
			'�p�C���b�g���̒n�`�K�����Z�o
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
			
			If IsOptionDefined("�n�`�K�����a�v�Z") Then
				'���j�b�g�ƃp�C���b�g�̒n�`�K���̑��a����n�`�K��������
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
				'���j�b�g�ƃp�C���b�g�̒n�`�K���̒Ⴂ�����g�p
				If uad > pad Then
					Adaption = pad
				Else
					Adaption = uad
				End If
			End If
		End Get
	End Property
	
	'�n�`�K���ɂ��C���l
	Public ReadOnly Property AdaptionMod(ByVal idx As Short, Optional ByVal ad_mod As Short = 0) As Double
		Get
			Dim uad As Short
			
			uad = Adaption(idx)
			
			'���X������S�łȂ�����AS�ɂ͂��Ȃ�
			If uad = 5 Then
				uad = MinLng(uad + ad_mod, 5)
			Else
				uad = MinLng(uad + ad_mod, 4)
			End If
			
			'Option�R�}���h�̐ݒ�ɉ����ēK���C���l��ݒ�
			If IsOptionDefined("�n�`�K���C���ɘa") Then
				If IsOptionDefined("�n�`�K���C���J�艺��") Then
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
				If IsOptionDefined("�n�`�K���C���J�艺��") Then
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
	
	
	'���b
	Public ReadOnly Property Armor(Optional ByVal ref_mode As String = "") As Integer
		Get
			Armor = lngArmor
			
			'�X�e�[�^�X�\���p
			Select Case ref_mode
				Case "��{�l"
					If IsConditionSatisfied("���b��") Then
						Armor = Armor \ 2
					End If
					If IsConditionSatisfied("�Ή�") Then
						Armor = 2 * Armor
					End If
					If IsConditionSatisfied("����") Then
						Armor = Armor \ 2
					End If
					Exit Property
				Case "�C���l"
					Armor = 0
			End Select
			
			'�p�C���b�g�ɂ��C��
			If CountPilot > 0 Then
				'��͂ɂ�鑕�b�C��
				If IsFeatureAvailable("��͕ϊ���") Then
					Armor = Armor + 5 * PlanaLevel()
				End If
				
				'�T�C�L�b�N�h���C�u�������j�b�g�͒��\�̓��x���ɉ����đ��b���ω�
				If IsFeatureAvailable("�T�C�L�b�N�h���C�u") Then
					Armor = Armor + 100 * PsychicLevel()
				End If
				
				'�I�[���ϊ��푕�����j�b�g�̓I�[�����x���ɉ����đ��b���ω�
				If IsFeatureAvailable("�I�[���ϊ���") Then
					Armor = Armor + 50 * AuraLevel()
				End If
			End If
			
			'���b���򉻂��Ă���ꍇ�͑��b�l�͔���
			If IsConditionSatisfied("���b��") Then
				Armor = Armor \ 2
			End If
			
			'�Ή����Ă��郆�j�b�g�͂ƂĂ��ł��c�c
			If IsConditionSatisfied("�Ή�") Then
				Armor = 2 * Armor
			End If
			
			'�����Ă��郆�j�b�g�͐Ƃ��Ȃ�
			If IsConditionSatisfied("����") Then
				Armor = Armor \ 2
			End If
		End Get
	End Property
	
	'�^����
	Public ReadOnly Property Mobility(Optional ByVal ref_mode As String = "") As Short
		Get
			Mobility = intMobility
			
			Select Case ref_mode
				Case "��{�l"
					Exit Property
				Case "�C���l"
					Mobility = 0
			End Select
			
			'�p�C���b�g�ɂ��C��
			If CountPilot > 0 Then
				'�T�C�L�b�N�h���C�u�������j�b�g�͒��\�̓��x���ɉ����ĉ^�������ω�
				If IsFeatureAvailable("�T�C�L�b�N�h���C�u") Then
					Mobility = Mobility + 5 * PsychicLevel()
				End If
				
				'�I�[���ϊ��푕�����j�b�g�̓I�[�����x���ɉ����ĉ^�������ω�
				If IsFeatureAvailable("�I�[���ϊ���") Then
					Mobility = Mobility + 2 * AuraLevel()
				End If
				
				'�V���N���h���C�u�������j�b�g�͓��������x���ɉ����ĉ^�������ω�
				If IsFeatureAvailable("�V���N���h���C�u") Then
					If MainPilot.SynchroRate > 0 Then
						Mobility = Mobility + (SyncLevel() - 50) \ 2
					End If
				End If
			End If
		End Get
	End Property
	
	'�r�b�g�}�b�v
	Public ReadOnly Property Bitmap(Optional ByVal use_orig As Boolean = False) As String
		Get
			If IsConditionSatisfied("���j�b�g�摜") Then
				Bitmap = LIndex(ConditionData("���j�b�g�摜"), 2)
				Exit Property
			End If
			
			If IsFeatureAvailable("���j�b�g�摜") Then
				Bitmap = FeatureData("���j�b�g�摜")
				Exit Property
			End If
			
			If use_orig Then
				Bitmap = Data.Bitmap0
			Else
				Bitmap = Data.Bitmap
			End If
		End Get
	End Property
	
	'�C����(�l������)
	Public ReadOnly Property Value() As Integer
		Get
			Value = Data.Value
			
			If IsFeatureAvailable("�C����C��") Then
				Value = MaxLng(Value + 1000 * FeatureLevel("�C����C��"), 0)
			End If
			
			If BossRank > 0 Then
				Value = Value * (1 + 0.5 * BossRank + 0.05 * Rank)
			Else
				Value = Value * (1 + 0.05 * Rank)
			End If
		End Get
	End Property
	
	'�o���l
	Public ReadOnly Property ExpValue() As Integer
		Get
			ExpValue = Data.ExpValue
			
			If IsFeatureAvailable("�o���l�C��") Then
				ExpValue = MaxLng(ExpValue + 10 * FeatureLevel("�o���l�C��"), 0)
			End If
			
			If BossRank > 0 Then
				ExpValue = ExpValue + 20 * BossRank
			End If
		End Get
	End Property
	
	'�c��s����
	Public ReadOnly Property Action() As Short
		Get
			If MaxAction > 0 Then
				Action = MaxLng(MaxAction - UsedAction, 0)
			Else
				Action = 0
			End If
		End Get
	End Property
	
	
	' === �s���p�^�[�����K�肷��p�����[�^�֘A���� ===
	
	'�w�c
	Public ReadOnly Property Party0() As String
		Get
			Party0 = strParty
		End Get
	End Property
	
	
	Public Property Party() As String
		Get
			Party = strParty
			
			'��������Ă���ꍇ
			If IsConditionSatisfied("����") And Not Master Is Nothing Then
				Party = Master.Party
				If Party = "����" Then
					'�����ɂȂ��Ă������ł͑���ł��Ȃ�
					Party = "�m�o�b"
				End If
			End If
			
			'�߈˂���Ă���ꍇ
			If IsConditionSatisfied("�߈�") And Not Master Is Nothing Then
				Party = Master.Party
			End If
			
			'�R���g���[���s�\�̖������j�b�g�͂m�o�b�Ƃ��Ĉ���
			If Party = "����" Then
				If IsConditionSatisfied("�\��") Or IsConditionSatisfied("����") Or IsConditionSatisfied("���|") Or IsConditionSatisfied("�x��") Or IsConditionSatisfied("����m") Then
					Party = "�m�o�b"
				End If
			End If
		End Get
		Set(ByVal Value As String)
			strParty = Value
		End Set
	End Property
	
	'�v�l���[�h
	
	Public Property Mode() As String
		Get
			Dim i As Short
			
			If IsUnderSpecialPowerEffect("����") Then
				'�������ŗD��
				For i = 1 To CountSpecialPower
					'UPGRADE_WARNING: �I�u�W�F�N�g SpecialPower(i).IsEffectAvailable(����) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If SpecialPower(i).IsEffectAvailable("����") Then
						Mode = SpecialPowerData(i)
						Exit Property
					End If
				Next 
			End If
			
			If IsConditionSatisfied("�\��") Or IsConditionSatisfied("����") Or IsConditionSatisfied("�߈�") Or IsConditionSatisfied("����m") Then
				'����Ȕ��f���o���Ȃ��ꍇ�͓����̖ړI��Y��Ă��܂�����
				'��ɒʏ탂�[�h�Ƃ��Ĉ���
				Mode = "�ʏ�"
				Exit Property
			End If
			
			If IsConditionSatisfied("���|") Then
				'���|�ɂ���ꂽ�ꍇ�͓��S
				Mode = "���S"
				Exit Property
			End If
			
			If IsConditionSatisfied("�x��") Then
				'�x��̂ɖZ�����c�c
				Mode = "�Œ�"
				Exit Property
			End If
			
			Mode = strMode
		End Get
		Set(ByVal Value As String)
			strMode = Value
		End Set
	End Property
	
	'�n�` area_name �ł̈ړ����\���ǂ���
	Public Function IsTransAvailable(ByRef area_name As String) As Boolean
		'�ړ��\�n�`�Ɋ܂܂�Ă��邩�H
		If InStr(Transportation, area_name) > 0 Then
			IsTransAvailable = True
		Else
			IsTransAvailable = False
		End If
		
		'����ړ��̏ꍇ
		If area_name = "����" Then
			If IsFeatureAvailable("����ړ�") Or IsFeatureAvailable("�z�o�[�ړ�") Then
				IsTransAvailable = True
			End If
		End If
	End Function
	
	
	' === ���j�b�g�p����\�͊֘A���� ===
	
	'����\�͂̑���
	Public Function CountFeature() As Short
		CountFeature = colFeature.Count()
	End Function
	
	'����\��
	Public Function Feature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		Feature = fd.Name
	End Function
	
	'����\�̖͂���
	Public Function FeatureName(ByRef Index As Object) As String
		FeatureName = FeatureNameInt(Index, colFeature)
	End Function
	
	Private Function FeatureNameInt(ByRef Index As Object, ByRef feature_list As Collection) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = feature_list.Item(Index)
		With fd
			'��\���̔\��
			Select Case .Name
				Case "�m�[�}�����[�h", "�p�[�c����", "����", "��������", "����s��", "��`��", "���`��", "���̐���", "�i������", "�}������", "���̋Z", "�ό`�Z", "�����N�A�b�v", "�ǉ��p�C���b�g", "�\�����p�C���b�g", "�ǉ��T�|�[�g", "������", "�n�[�h�|�C���g", "����N���X", "�h��N���X", "�a�f�l", "����a�f�l", "�A�r���e�B�a�f�l", "���̂a�f�l", "�����a�f�l", "�ό`�a�f�l", "�n�C�p�[���[�h�a�f�l", "���j�b�g�摜", "�p�C���b�g�摜", "�p�C���b�g����", "�p�C���b�g�ǂ݉���", "����", "���i�ύX", "�z��", "������", "�ϐ�", "��_", "�L��", "������ʖ�����", "�A�C�e�����L", "���A�A�C�e�����L", "���[�j���O�\�Z", "������C��", "�ő������", "�p�C���b�g�\�͕t��", "�p�C���b�g�\�͋���", "��\��", "�U������", "�˒�����", "���틭��", "����������", "�b�s������", "������ʔ���������", "�K�v�Z�\", "�s�K�v�Z�\", "�_�~�[���j�b�g", "�n�`���j�b�g", "���������R�}���h��", "�ϐg�����R�}���h��", "�P�l���\", "�������", "�퓬�A�j��", "�p�C���b�g�n�`�K���ύX", "���b�Z�[�W�N���X", "�p�ꖼ", "����"
					'���j�b�g�p����\��
					FeatureNameInt = ""
					Exit Function
				Case "���̕ύX", "�ǂ݉����ύX", "�T�C�Y�ύX", "�n�`�K���ύX", "�n�`�K���Œ�ύX", "�󒆈ړ�", "����ړ�", "�����ړ�", "�F���ړ�", "�n���ړ�", "�C����C��", "�o���l�C��", "�ő�e������", "�d�m�����", "�u�|�t�o", "��^�A�C�e��", "��"
					'�A�C�e���p����\��
					FeatureNameInt = ""
					Exit Function
			End Select
			
			' ADD START MARGE
			'�g��摜�\�͂́u�g��摜(������)�v�Ƃ������w�������̂ő��̔�\���\�͂ƈقȂ�
			'������@���g��
			If InStr(.Name, "�g��摜") = 1 Then
				FeatureNameInt = ""
				Exit Function
			End If
			' ADD END MARGE
			
			If Len(.StrData) > 0 Then
				'�ʖ��̎w�肠��
				FeatureNameInt = ListIndex(.StrData, 1)
				If FeatureNameInt = "��\��" Or FeatureNameInt = "���" Then
					FeatureNameInt = ""
				End If
			ElseIf .Level = DEFAULT_LEVEL Then 
				'���x���w��Ȃ�
				FeatureNameInt = .Name
			ElseIf .Level >= 0 Then 
				'���x���w�肠��
				FeatureNameInt = .Name & "Lv" & VB6.Format(.Level)
				If .Name = "�ˌ�����" Then
					If CountPilot > 0 Then
						If MainPilot.HasMana() Then
							FeatureNameInt = "���͋���Lv" & VB6.Format(.Level)
						End If
					End If
				End If
			Else
				'�}�C�i�X�̃��x���w��
				Select Case .Name
					Case "�i������"
						FeatureNameInt = "�i���ቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "�ˌ�����"
						FeatureNameInt = "�ˌ��ቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
						If CountPilot > 0 Then
							If MainPilot.HasMana() Then
								FeatureNameInt = "���͒ቺLv" & VB6.Format(System.Math.Abs(.Level))
							End If
						End If
					Case "��������"
						FeatureNameInt = "�����ቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "�������"
						FeatureNameInt = "���ቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "�Z�ʋ���"
						FeatureNameInt = "�Z�ʒቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case "��������"
						FeatureNameInt = "�����ቺ" & "Lv" & VB6.Format(System.Math.Abs(.Level))
					Case Else
						FeatureNameInt = .Name & "Lv" & VB6.Format(.Level)
				End Select
			End If
		End With
		Exit Function
		
ErrorHandler: 
		'������Ȃ������ꍇ
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		FeatureNameInt = CStr(Index)
	End Function
	
	Public Function FeatureName0(ByRef Index As Object) As String
		FeatureName0 = FeatureName(Index)
		If InStr(FeatureName0, "Lv") > 0 Then
			FeatureName0 = Left(FeatureName0, InStr(FeatureName0, "Lv") - 1)
		End If
	End Function
	
	'����\�͂̃��x��
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
	
	'����\�͂̃f�[�^
	Public Function FeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		FeatureData = ""
	End Function
	
	'����\�͂̕K�v�Z�\
	Public Function FeatureNecessarySkill(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureNecessarySkill = fd.NecessarySkill
		Exit Function
		
ErrorHandler: 
		FeatureNecessarySkill = ""
	End Function
	
	'�w�肵������\�͂����L���Ă��邩�H
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(fname)
		IsFeatureAvailable = True
		Exit Function
		
ErrorHandler: 
		IsFeatureAvailable = False
	End Function
	
	'����\�͂Ƀ��x���w�肪����Ă���H
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
	
	'����\�͂̑���(�K�v�����𖞂����Ȃ����̂��܂�)
	Public Function CountAllFeature() As Short
		CountAllFeature = colAllFeature.Count()
	End Function
	
	'����\��(�K�v�����𖞂����Ȃ����̂��܂�)
	Public Function AllFeature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colAllFeature.Item(Index)
		AllFeature = fd.Name
	End Function
	
	'����\�̖͂���(�K�v�����𖞂����Ȃ����̂��܂�)
	Public Function AllFeatureName(ByRef Index As Object) As String
		AllFeatureName = FeatureNameInt(Index, colAllFeature)
	End Function
	
	'����\�͂̃��x��(�K�v�����𖞂����Ȃ����̂��܂�)
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
	
	'����\�͂̃��x�����w�肳��Ă��邩(�K�v�����𖞂����Ȃ����̂��܂�)
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
	
	'����\�͂̃f�[�^(�K�v�����𖞂����Ȃ����̂��܂�)
	Public Function AllFeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colAllFeature.Item(Index)
		AllFeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		AllFeatureData = ""
	End Function
	
	'����\�͂Ƀ��x���w�肪����Ă���H(�K�v�����𖞂����Ȃ����̂��܂�)
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
	
	'����\�͂��K�v�����𖞂����Ă��邩
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
	
	
	' === ���j�b�g�X�e�[�^�X�̍X�V���� ===
	
	'���j�b�g�̊e��p�����[�^���X�V����T�u���[�`��
	'�p�����[�^�═��A�A�r���e�B�f�[�^�����ω�����ۂɂ͕K���Ăяo���K�v������B
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
		
		'�g�o�Ƃd�m�̒l���L�^
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		
		'���j�b�g�p�摜�t�@�C�������L�^���Ă���
		ubitmap = Bitmap
		
		'��\�����ǂ����L�^���Ă���
		is_invisible = IsFeatureAvailable("��\��")
		
		'����s���ǂ������L�^���Ă���
		is_uncontrollable = IsFeatureAvailable("����s��")
		
		'�s���肪�ǂ������L�^���Ă���
		is_stable = IsFeatureAvailable("�s����")
		
TryAgain: 
		
		'�A�C�e�������݂̌`�ԂŌ��͂𔭊����Ă���邩����
		For	Each itm In colItem
			With itm
				.Activated = .IsAvailable(Me)
			End With
		Next itm
		
		'�����N�A�b�v�ɂ��f�[�^�ύX
		Do While Data.IsFeatureAvailable("�����N�A�b�v")
			With Data
				If Rank < .FeatureLevel("�����N�A�b�v") Then
					Exit Do
				End If
				If Not IsNecessarySkillSatisfied(.FeatureNecessarySkill("�����N�A�b�v")) Then
					Exit Do
				End If
				fdata = .FeatureData("�����N�A�b�v")
			End With
			
			With UDList
				If Not .IsDefined(fdata) Then
					ErrorMessage(Name & "�̃����N�A�b�v�惆�j�b�g�u" & fdata & "�v�̃f�[�^����`����Ă��܂���")
					TerminateSRC()
				End If
				
				Data = .Item(fdata)
			End With
		Loop 
		
		'����\�͂��X�V
		
		'�܂�����\�̓��X�g���N���A
		With colFeature
			For	Each fd In colFeature
				.Remove(1)
			Next fd
		End With
		
		'�t�����ꂽ����\��
		For	Each cnd In colCondition
			With cnd
				If .Lifetime <> 0 Then
					If Right(.Name, 2) = "�t��" Then
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
		
		'���j�b�g�f�[�^�Œ�`����Ă������\��
		AddFeatures((Data.colFeature))
		
		'�A�C�e���œ���ꂽ����\��
		For i = 1 To CountItem
			With Item(i)
				If .Activated Then
					AddFeatures((.Data.colFeature), True)
				End If
			End With
		Next 
		
		'�p�C���b�g�f�[�^�Œ�`����Ă������\��
		If CountPilot > 0 Then
			If IsFeatureAvailable("�ǉ��p�C���b�g") Then
				'����\�͂�t������O�ɕK�v�Z�\����������Ă��邩�ǂ�������
				UpdateFeatures("�ǉ��p�C���b�g")
			End If
			AddFeatures((MainPilot.Data.colFeature))
			For i = 2 To CountPilot
				AddFeatures((Pilot(i).Data.colFeature))
			Next 
			For i = 1 To CountSupport
				AddFeatures((Support(i).Data.colFeature))
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
				'����\�͂�t������O�ɕK�v�Z�\����������Ă��邩�ǂ�������
				UpdateFeatures("�ǉ��T�|�[�g")
				If IsFeatureAvailable("�ǉ��T�|�[�g") Then
					AddFeatures((AdditionalSupport.Data.colFeature))
				End If
			End If
		End If
		
		'�p�C���b�g�\�͕t���������̌��ʂ��N���A
		i = 1
		Do While i <= CountCondition
			Select Case Right(Condition(i), 3)
				Case "�t���Q", "�����Q"
					DeleteCondition(i)
				Case Else
					i = i + 1
			End Select
		Loop 
		
		'�p�C���b�g�\�͕t��
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
					Case "�p�C���b�g�\�͕t��"
						'�K�v�Z�\�𖞂����Ă���H
						If Not IsNecessarySkillSatisfied(.NecessarySkill) Then
							found = True
							GoTo NextFeature
						End If
						'�K�v�����𖞂����Ă���H
						If Not IsNecessarySkillSatisfied(.NecessaryCondition) Then
							found = True
							GoTo NextFeature
						End If
						flags(i) = True
						
						'�\�͎w�肪�u"�v�ň͂܂�Ă���ꍇ�́u"�v���폜
						If Asc(.StrData) = 34 Then '"
							buf = Mid(.StrData, 2, Len(.StrData) - 2)
						Else
							buf = .StrData
						End If
						
						'�t���������\�͂̎�ށA���x���A�f�[�^�����
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
						
						'�G���A�X����`����Ă���H
						If ALDList.IsDefined(stype) Then
							With ALDList.Item(stype)
								For j = 1 To .Count
									'�G���A�X�̒�`�ɏ]���ē���\�͒�`��u��������
									stype2 = .AliasType(j)
									
									If LIndex(.AliasData(j), 1) = "���" Then
										'����\�͂̉��
										If sdata <> "" Then
											stype2 = LIndex(sdata, 1)
										End If
										slevel2 = DEFAULT_LEVEL
										sdata2 = .AliasData(j)
									Else
										'�ʏ�̔\��
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
											If InStr(sdata2, "��\��") <> 1 Then
												sdata2 = sdata & " " & ListTail(sdata2, LLength(sdata) + 1)
											End If
										End If
										
										If .AliasLevelIsPlusMod(j) Or .AliasLevelIsMultMod(j) Then
											sdata2 = LIndex(sdata2, 1) & "Lv" & VB6.Format(slevel) & " " & ListTail(sdata2, 2)
											sdata2 = Trim(sdata2)
										End If
									End If
									
									'�����g�p�s�\�U���ɂ��g�p�s�\�ɂȂ����Z�\�𕕈󂷂�B
									If ConditionLifetime(stype2 & "�g�p�s�\") > 0 Then
										GoTo NextFeature
									End If
									AddCondition(stype2 & "�t���Q", -1, slevel2, sdata2)
								Next 
							End With
						Else
							'�����g�p�s�\�U���ɂ��g�p�s�\�ɂȂ����Z�\�𕕈󂷂�B
							If ConditionLifetime(stype & "�g�p�s�\") > 0 Then
								GoTo NextFeature
							End If
							AddCondition(stype & "�t���Q", -1, slevel, sdata)
						End If
						
					Case "�p�C���b�g�\�͋���"
						'�K�v�Z�\�𖞂����Ă���H
						If Not IsNecessarySkillSatisfied(.NecessarySkill) Then
							found = True
							GoTo NextFeature
						End If
						'�K�v�����𖞂����Ă���H
						If Not IsNecessarySkillSatisfied(.NecessaryCondition) Then
							found = True
							GoTo NextFeature
						End If
						flags(i) = True
						
						'�\�͎w�肪�u"�v�ň͂܂�Ă���ꍇ�́u"�v���폜
						If Asc(.StrData) = 34 Then '"
							buf = Mid(.StrData, 2, Len(.StrData) - 2)
						Else
							buf = .StrData
						End If
						
						'�����������\�͂̎�ށA���x���A�f�[�^�����
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
						
						'�G���A�X����`����Ă���H
						If ALDList.IsDefined(stype) Then
							With ALDList.Item(stype)
								For j = 1 To .Count
									'�G���A�X�̒�`�ɏ]���ē���\�͒�`��u��������
									stype2 = .AliasType(j)
									
									'�����g�p�s�\�U���ɂ��g�p�s�\�ɂȂ����Z�\�𕕈󂷂�B
									If ConditionLifetime(stype2 & "�g�p�s�\") > 0 Then
										GoTo NextFeature
									End If
									If LIndex(.AliasData(j), 1) = "���" Then
										'����\�͂̉��
										If sdata <> "" Then
											stype2 = LIndex(sdata, 1)
										End If
										slevel2 = DEFAULT_LEVEL
										sdata2 = .AliasData(j)
										'�����g�p�s�\�U���ɂ��g�p�s�\�ɂȂ����Z�\�𕕈󂷂�B
										If ConditionLifetime(stype2 & "�g�p�s�\") > 0 Then
											GoTo NextFeature
										End If
										AddCondition(stype2 & "�t���Q", -1, slevel2, sdata2)
									Else
										'�ʏ�̔\��
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
											If InStr(sdata2, "��\��") <> 1 Then
												sdata2 = sdata & " " & ListTail(sdata2, LLength(sdata) + 1)
											End If
										End If
										
										'�������郌�x���͗ݐς���
										If IsConditionSatisfied(stype2 & "�����Q") Then
											slevel2 = slevel2 + ConditionLevel(stype2 & "�����Q")
											DeleteCondition(stype2 & "�����Q")
										End If
										
										AddCondition(stype2 & "�����Q", -1, slevel2, sdata2)
									End If
								Next 
							End With
						Else
							'�������郌�x���͗ݐς���
							If IsConditionSatisfied(stype & "�����Q") Then
								slevel = slevel + ConditionLevel(stype & "�����Q")
								DeleteCondition(stype & "�����Q")
							End If
							
							AddCondition(stype & "�����Q", -1, slevel, sdata)
						End If
				End Select
			End With
NextFeature: 
			i = i + 1
		Next fd
		'�K�v�Z�\���K�v�����t���̃p�C���b�g�\�͕t��������������ꍇ�͕t���⋭���̌��ʁA
		'�K�v�Z�\���K�v��������������邱�Ƃ�����̂ň�x������蒼��
		If Not flag And found Then
			flag = True
			GoTo AddSkills
		End If
		
		'�p�C���b�g�p����\�͂̕t�������������������̂ŕK�v�Z�\�̔��肪�\�ɂȂ����B
		UpdateFeatures()
		
		'�A�C�e�����K�v�Z�\�𖞂������ēx�`�F�b�N�B
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
			'�A�C�e���̎g�p�ۂ��ω������̂ōŏ������蒼��
			GoTo TryAgain
		End If
		
		'�����N�A�b�v���邩�ēx�`�F�b�N�B
		With Data
			If .IsFeatureAvailable("�����N�A�b�v") Then
				If Rank >= .FeatureLevel("�����N�A�b�v") Then
					If IsNecessarySkillSatisfied(.FeatureNecessarySkill("�����N�A�b�v")) Then
						'�����N�A�b�v���\�ɂȂ����̂ōŏ������蒼��
						GoTo TryAgain
					End If
				End If
			End If
		End With
		
		If CountPilot > 0 Then
			'�p�C���b�g�\�͂��A�b�v�f�[�g
			For i = 2 To CountPilot
				Pilot(i).Update()
			Next 
			For i = 1 To CountSupport
				Support(i).Update()
			Next 
			
			'���C���p�C���b�g�͑��̃p�C���b�g�̃T�|�[�g���󂯂�֌W��
			'�Ō�ɃA�b�v�f�[�g����
			Pilot(1).Update()
			If Not MainPilot Is Pilot(1) Then
				MainPilot.Update()
			End If
		End If
		
		'���j�b�g�摜�p�t�@�C�����ɕω�������ꍇ�̓��j�b�g�摜���X�V
		If BitmapID <> 0 Then
			If ubitmap <> Bitmap Then
				BitmapID = MakeUnitBitmap(Me)
				For i = 1 To CountOtherForm
					OtherForm(i).BitmapID = 0
				Next 
				If Not without_refresh Then
					If Status_Renamed = "�o��" Then
						If Not IsPictureVisible And MapFileName <> "" Then
							PaintUnitBitmap(Me)
						End If
					End If
				End If
			End If
		End If
		
		'���j�b�g�̕\���A��\�����؂�ւ�����ꍇ
		If is_invisible <> IsFeatureAvailable("��\��") Then
			If Status_Renamed = "�o��" Then
				If Not IsPictureVisible And MapFileName <> "" Then
					BitmapID = MakeUnitBitmap(Me)
					If IsFeatureAvailable("��\��") Then
						EraseUnitBitmap(x, y, Not without_refresh)
					Else
						If Not without_refresh Then
							PaintUnitBitmap(Me)
						End If
					End If
				End If
			End If
		End If
		
		'�e��p�����[�^
		With Data
			lngMaxHP = .HP + 200 * CInt(Rank)
			intMaxEN = .EN + 10 * Rank
			lngArmor = .Armor + 100 * CInt(Rank)
			intMobility = .Mobility + 5 * Rank
			intSpeed = .Speed
		End With
		
		'�{�X�����N�ɂ��C��
		If IsHero Or IsOptionDefined("���g��") Then
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
			
			If IsOptionDefined("BossRank���b�C���ቺ") Then
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
		
		'�g�o�����I�v�V����
		If IsOptionDefined("�g�o����") Then
			If CountPilot > 0 Then
				lngMaxHP = MinLng((lngMaxHP / 100) * (100 + MainPilot.Level), 9999999)
			End If
		End If
		
		'�d�m�����I�v�V����
		If IsOptionDefined("�d�m����") Then
			If CountPilot > 0 Then
				intMaxEN = MinLng((intMaxEN / 100) * (100 + MainPilot.Level), 9999)
			End If
		End If
		
		'����\�͂ɂ��C��
		If CountPilot > 0 Then
			pmorale = MainPilot.Morale
		Else
			pmorale = 100
		End If
		For	Each fd In colFeature
			With fd
				Select Case .Name
					'�Œ�l�ɂ�鋭��
					Case "�g�o����"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngMaxHP = lngMaxHP + 200 * .Level
						End If
					Case "�d�m����"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMaxEN = intMaxEN + 10 * .Level
						End If
					Case "���b����"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngArmor = lngArmor + 100 * .Level
						End If
					Case "�^��������"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMobility = intMobility + 5 * .Level
						End If
					Case "�ړ��͋���"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intSpeed = intSpeed + .Level
						End If
						'�����ɂ�鋭��
					Case "�g�o��������"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngMaxHP = lngMaxHP + Data.HP * .Level \ 20
						End If
					Case "�d�m��������"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMaxEN = intMaxEN + Data.EN * .Level \ 20
						End If
					Case "���b��������"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							lngArmor = lngArmor + Data.Armor * .Level \ 20
						End If
					Case "�^������������"
						If pmorale >= StrToLng(LIndex(.StrData, 2)) Then
							intMobility = intMobility + Data.Mobility * .Level \ 20
						End If
				End Select
			End With
		Next fd
		
		'�A�C�e���ɂ��C��
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
		
		'�������Ă���u�u�|�t�o=���j�b�g�v�A�C�e���ɂ��C��
		num = 0
		If IsConditionSatisfied("�u�|�t�o") Then
			Select Case FeatureData("�u�|�t�o")
				Case "�S", "���j�b�g"
					num = num + 1
			End Select
		End If
		For	Each itm In colItem
			With itm
				If .IsFeatureAvailable("�u�|�t�o") Then
					Select Case .FeatureData("�u�|�t�o")
						Case "�S", "���j�b�g"
							num = num + 1
					End Select
				End If
			End With
		Next itm
		If CountPilot > 0 Then
			With MainPilot.Data
				If .IsFeatureAvailable("�u�|�t�o") Then
					Select Case .FeatureData("�u�|�t�o")
						Case "�S", "���j�b�g"
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
		
		'�ǉ��ړ���
		If IsFeatureAvailable("�ǉ��ړ���") Then
			For	Each fd In colFeature
				With fd
					If .Name = "�ǉ��ړ���" Then
						If Area = LIndex(.StrData, 2) Then
							intSpeed = intSpeed + .Level
						End If
					End If
				End With
			Next fd
			intSpeed = MaxLng(intSpeed, 0)
		End If
		
		'����l�𒴂��Ȃ��悤��
		lngMaxHP = MinLng(lngMaxHP, 9999999)
		intMaxEN = MinLng(intMaxEN, 9999)
		lngArmor = MinLng(lngArmor, 99999)
		intMobility = MinLng(intMobility, 9999)
		intSpeed = MinLng(intSpeed, 99)
		
		'�g�o�A�d�m�̍ő�l�̕ϓ��ɑΉ�
		HP = MaxHP * hp_ratio / 100
		EN = MaxEN * en_ratio / 100
		
		'�؂艺���̌��ʂg�o��0�ɂȂ邱�Ƃ�h��
		If hp_ratio > 0 Then
			If HP = 0 Then
				HP = 1
			End If
		End If
		
		'�n�`�K��
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
		
		'�ړ��^�C�v�ǉ��ɂ��n�`�K���C��
		If IsFeatureAvailable("�󒆈ړ�") Then
			uadaption(1) = MaxLng(uadaption(1), 4)
		End If
		If IsFeatureAvailable("����ړ�") Then
			uadaption(2) = MaxLng(uadaption(2), 4)
		End If
		If IsFeatureAvailable("�����ړ�") Then
			uadaption(3) = MaxLng(uadaption(3), 4)
		End If
		If IsFeatureAvailable("�F���ړ�") Then
			uadaption(4) = MaxLng(uadaption(4), 4)
		End If
		
		'�n�`�K���ύX�\�͂ɂ��C��
		For	Each fd In colFeature
			With fd
				Select Case .Name
					Case "�n�`�K���ύX"
						For i = 1 To 4
							num = StrToLng(LIndex(.StrData, i))
							If num > 0 Then
								If uadaption(i) < 4 Then
									uadaption(i) = uadaption(i) + num
									'�n�`�K����A��荂���͂Ȃ�Ȃ�
									If uadaption(i) > 4 Then
										uadaption(i) = 4
									End If
								End If
							Else
								uadaption(i) = uadaption(i) + num
							End If
						Next 
					Case "�n�`�K���Œ�ύX"
						For i = 1 To 4
							num = StrToLng(LIndex(.StrData, i))
							If LIndex(.StrData, 5) = "����" Then
								' �����ύX�̏ꍇ
								If num >= 0 And num <= 5 Then
									uadaption(i) = num
								End If
							Else
								' �����ق���D�悷��ꍇ
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
		
		'�󒆂ɗ��܂邱�Ƃ��o���邩�`�F�b�N
		If Status_Renamed = "�o��" And Area = "��" And Not IsTransAvailable("��") Then
			'�n��(����)�ɖ߂�
			Select Case TerrainClass(x, y)
				Case "��", "����"
					Area = "�n��"
				Case "��", "�[��"
					If IsTransAvailable("����") Then
						Area = "����"
					Else
						Area = "����"
					End If
			End Select
			If Not without_refresh Then
				If Not IsPictureVisible And MapFileName <> "" Then
					PaintUnitBitmap(Me)
				End If
			End If
		End If
		
		'�U���ւ̑ϐ����X�V
		strAbsorb = ""
		strImmune = ""
		strResist = ""
		strWeakness = ""
		strEffective = ""
		strSpecialEffectImmune = ""
		'����\�͂ɂ���ē���ꂽ�ϐ�
		For	Each fd In colFeature
			With fd
				Select Case .Name
					Case "�z��"
						strAbsorb = strAbsorb & .StrData
					Case "������"
						strImmune = strImmune & .StrData
					Case "�ϐ�"
						strResist = strResist & .StrData
					Case "��_"
						strWeakness = strWeakness & .StrData
					Case "�L��"
						strEffective = strEffective & .StrData
					Case "������ʖ�����"
						strSpecialEffectImmune = strSpecialEffectImmune & .StrData
				End Select
			End With
		Next fd
		'��_�A�L���t�������U���ɂ���_�A�L���̕t��
		For i = 1 To CountCondition
			If ConditionLifetime(i) <> 0 Then
				ch = Condition(i)
				Select Case Right(ch, 6)
					Case "������_�t��"
						strWeakness = strWeakness & Left(ch, Len(ch) - 6)
					Case "�����L���t��"
						strEffective = strEffective & Left(ch, Len(ch) - 6)
				End Select
			End If
		Next 
		'�����̃_�u����Ȃ���
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
		
		'����f�[�^���X�V
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
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		
		'���푮�����X�V
		ReDim strWeaponClass(CountWeapon)
		For i = 1 To CountWeapon
			strWeaponClass(i) = Weapon(i).Class_Renamed
		Next 
		Dim hidden_attr As String
		Dim skipped As Boolean
		If IsFeatureAvailable("�U������") Then
			For i = 1 To CountWeapon
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				'��\���̑���������ꍇ�͈�U�����o��
				If InStrNotNest(wclass, "|") > 0 Then
					strWeaponClass(i) = Left(wclass, InStrNotNest(wclass, "|") - 1)
					hidden_attr = Mid(wclass, InStrNotNest(wclass, "|") + 1)
				Else
					hidden_attr = ""
				End If
				
				For j = 1 To CountFeature
					If Feature(j) = "�U������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						
						If flen = 1 Then
							'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕���ɑ�����t��
							flag = True
							k = 2
						ElseIf LIndex(fdata, 1) = "��\��" Then 
							'��\���w�肪����ꍇ (����w�肪����ꍇ���܂�)
							If flen = 2 Then
								'����w�薳��
								flag = True
							Else
								'����w�肠��
								flag = False
							End If
							k = 3
						Else
							'����w�肪����ꍇ
							flag = False
							k = 2
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
								false_count = false_count + 1
							End If
							
							k = k + 1
						Loop 
						
						'������ǉ�
						If flag Or false_count = 0 Then
							buf = LIndex(fdata, 1)
							If buf = "��\��" Then
								'��\���̑����̏ꍇ
								hidden_attr = hidden_attr & LIndex(fdata, 2)
							Else
								'�������d�����Ȃ��悤�ɕt��
								skipped = False
								For k = 1 To Len(buf)
									ch = GetClassBundle(buf, k)
									
									If Not IsNumeric(ch) And ch <> "L" And ch <> "." Then
										skipped = False
									End If
									
									If (InStrNotNest(strWeaponClass(i), ch) = 0 Or IsNumeric(ch) Or ch = "L" Or ch = ".") And Not skipped Then
										If ch = "��" Then
											'��������t������ꍇ�͕���𖂖@���퉻����
											l = InStrNotNest(strWeaponClass(i), "��")
											l = MaxLng(InStrNotNest(strWeaponClass(i), "��"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "��"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "�e"), l)
											l = MaxLng(InStrNotNest(strWeaponClass(i), "��"), l)
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
				
				'��\���̑�����ǉ�
				If Len(hidden_attr) > 0 Then
					strWeaponClass(i) = strWeaponClass(i) & "|" & hidden_attr
				End If
			Next 
		End If
		
		'����U���͂��X�V
		ReDim lngWeaponPower(CountWeapon)
		
		'�������Ă���u�u�|�t�o=����v�A�C�e���̌����J�E���g���Ă���
		num = 0
		If IsConditionSatisfied("�u�|�t�o") Then
			Select Case FeatureData("�u�|�t�o")
				Case "�S", "����"
					num = num + 1
			End Select
		End If
		For	Each itm In colItem
			With itm
				If .Activated Then
					If .IsFeatureAvailable("�u�|�t�o") Then
						Select Case .FeatureData("�u�|�t�o")
							Case "�S", "����"
								num = num + 1
						End Select
					End If
				End If
			End With
		Next itm
		If CountPilot > 0 Then
			With MainPilot.Data
				If .IsFeatureAvailable("�u�|�t�o") Then
					Select Case .FeatureData("�u�|�t�o")
						Case "�S", "����"
							num = num + 1
					End Select
				End If
			End With
		End If
		num = num * Data.ItemNum
		
		For i = 1 To CountWeapon
			lngWeaponPower(i) = Weapon(i).Power
			
			'���Ƃ��ƍU���͂�0�̕����0�ɌŒ�
			If lngWeaponPower(i) = 0 Then
				GoTo NextWeapon
			End If
			
			'���틭���ɂ��C��
			If IsFeatureAvailable("���틭��") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "���틭��" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
							If IsWeaponClassifiedAs(i, "��") Then
								'�_���[�W�Œ蕐��͕���w�肪���햼�A����\�����A�u�Łv��
								'�����ꂩ�ōs��ꂽ�ꍇ�ɂ̂݋���
								If wtype = "��" Or wname = wtype Or wnickname = wtype Then
									found = True
								End If
							Else
								Select Case wtype
									Case "�S"
										found = True
									Case "��"
										If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
											found = True
										End If
									Case Else
										If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
											found = True
										Else
											'�K�v�Z�\�ɂ��w��
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
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
			'���튄�������ɂ��C��
			If IsFeatureAvailable("���튄������") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "���튄������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
							If IsWeaponClassifiedAs(i, "��") Then
								'�_���[�W�Œ蕐��͕���w�肪���햼�A����\�����A�u�Łv��
								'�����ꂩ�ōs��ꂽ�ꍇ�ɂ̂݋���
								If wtype = "��" Or wname = wtype Or wnickname = wtype Then
									found = True
								End If
							Else
								Select Case wtype
									Case "�S"
										found = True
									Case "��"
										If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
											found = True
										End If
									Case Else
										If InStrNotNest(wclass, wtype) > 0 Or wname = wtype Or wnickname = wtype Then
											found = True
										Else
											'�K�v�Z�\�ɂ��w��
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
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
			
			'�_���[�W�Œ蕐��
			If IsWeaponClassifiedAs(i, "��") Then
				GoTo NextWeapon
			End If
			
			If IsWeaponClassifiedAs(i, "�q") Then
				'�ᐬ���^�̍U��
				If IsWeaponLevelSpecified(i, "�q") Then
					'���x���ݒ肳��Ă���ꍇ�A�����ʂ����x���~�P�O�~�����N�ɂ���
					lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "�q") * CInt(Rank + num)
					'�I�E�V�E���ƕ��p�����ꍇ
					If IsWeaponClassifiedAs(i, "�I") Or IsWeaponClassifiedAs(i, "��") Or IsWeaponClassifiedAs(i, "�V") Then
						lngWeaponPower(i) = lngWeaponPower(i) + 10 * (10 - WeaponLevel(i, "�q")) * CInt(Rank + num)
						
						'�I�[���Z
						If IsWeaponClassifiedAs(i, "�I") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "�q") * AuraLevel()
						End If
						
						'�T�C�L�b�N�U��
						If IsWeaponClassifiedAs(i, "��") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "�q") * PsychicLevel()
						End If
						
						'�������ΏۍU��
						If IsWeaponClassifiedAs(i, "�V") Then
							If CountPilot() > 0 Then
								If MainPilot.SynchroRate > 0 Then
									lngWeaponPower(i) = lngWeaponPower(i) + 15 * WeaponLevel(i, "�q") * (SyncLevel() - 50) \ 10
								End If
							End If
						End If
					End If
				Else
					'���x���w�肳��Ă��Ȃ��ꍇ�͍��܂łǂ��胉���N�~�T�O
					lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
					
					'�I�E�V�E���ƕ��p�����ꍇ
					If IsWeaponClassifiedAs(i, "�I") Or IsWeaponClassifiedAs(i, "��") Or IsWeaponClassifiedAs(i, "�V") Then
						lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
						
						'�I�[���Z
						If IsWeaponClassifiedAs(i, "�I") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 50 * AuraLevel()
						End If
						
						'�T�C�L�b�N�U��
						If IsWeaponClassifiedAs(i, "��") Then
							lngWeaponPower(i) = lngWeaponPower(i) + 50 * PsychicLevel()
						End If
						
						'�������ΏۍU��
						If IsWeaponClassifiedAs(i, "�V") Then
							If CountPilot() > 0 Then
								If MainPilot.SynchroRate > 0 Then
									lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50) \ 2
								End If
							End If
						End If
					End If
				End If
			ElseIf IsWeaponClassifiedAs(i, "��") Then 
				'���������I�E���E�V�����𖳎������q����
				If IsWeaponLevelSpecified(i, "��") Then
					'���x���ݒ肳��Ă���ꍇ�A�����ʂ����x���~�P�O�~�����N�ɂ���
					lngWeaponPower(i) = lngWeaponPower(i) + 10 * WeaponLevel(i, "��") * (Rank + num)
				Else
					'���x���w�肪�Ȃ��ꍇ�A�����ʂ͂T�O�~�����N
					lngWeaponPower(i) = lngWeaponPower(i) + 50 * CInt(Rank + num)
				End If
				
				'�I�[���Z
				If IsWeaponClassifiedAs(i, "�I") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * AuraLevel()
				End If
				
				'�T�C�L�b�N�U��
				If IsWeaponClassifiedAs(i, "��") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * PsychicLevel()
				End If
				
				'�������ΏۍU��
				If IsWeaponClassifiedAs(i, "�V") Then
					If CountPilot() > 0 Then
						If MainPilot.SynchroRate > 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50)
						End If
					End If
				End If
			Else
				'�q�A�������������Ƃ��Ȃ��ꍇ
				lngWeaponPower(i) = lngWeaponPower(i) + 100 * CInt(Rank + num)
				
				'�I�[���Z
				If IsWeaponClassifiedAs(i, "�I") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * AuraLevel()
				End If
				
				'�T�C�L�b�N�U��
				If IsWeaponClassifiedAs(i, "��") Then
					lngWeaponPower(i) = lngWeaponPower(i) + 100 * PsychicLevel()
				End If
				
				'�������ΏۍU��
				If IsWeaponClassifiedAs(i, "�V") Then
					If CountPilot() > 0 Then
						If MainPilot.SynchroRate > 0 Then
							lngWeaponPower(i) = lngWeaponPower(i) + 15 * (SyncLevel() - 50)
						End If
					End If
				End If
			End If
			
			'�{�X�����N�ɂ��C��
			If BossRank > 0 Then
				lngWeaponPower(i) = lngWeaponPower(i) + MinLng(100 * BossRank, 300)
			End If
			
			'�U���͂̍ō��l��99999
			If lngWeaponPower(i) > 99999 Then
				lngWeaponPower(i) = 99999
			End If
			
			'�Œ�l��1
			If lngWeaponPower(i) <= 0 Then
				lngWeaponPower(i) = 1
			End If
NextWeapon: 
		Next 
		
		'����˒����X�V
		ReDim intWeaponMaxRange(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponMaxRange(i) = Weapon(i).MaxRange
			
			'�ő�˒������Ƃ��ƂP�Ȃ炻��ȏ�ω����Ȃ�
			If intWeaponMaxRange(i) = 1 Then
				GoTo NextWeapon2
			End If
			
			'�v�O�U���U���̂m�s�\�͂ɂ��˒�����
			If InStrNotNest(strWeaponClass(i), "�T") > 0 Then
				If CountPilot() > 0 Then
					With MainPilot
						intWeaponMaxRange(i) = intWeaponMaxRange(i) + .SkillLevel("�����o") \ 4 + .SkillLevel("�m�o����") \ 4
					End With
				End If
			End If
			
			'�}�b�v�U���ɂ͓K�p����Ȃ�
			If InStrNotNest(strWeaponClass(i), "�l") > 0 Then
				GoTo NextWeapon2
			End If
			
			'�ڋߐ핐��ɂ͓K�p����Ȃ�
			If InStrNotNest(strWeaponClass(i), "��") > 0 Or InStrNotNest(strWeaponClass(i), "��") > 0 Or InStrNotNest(strWeaponClass(i), "��") > 0 Then
				GoTo NextWeapon2
			End If
			
			'�L�����U���U���ɂ͓K�p����Ȃ�
			If InStrNotNest(strWeaponClass(i), "�L") > 0 Then
				GoTo NextWeapon2
			End If
			
			'�˒������ɂ��C��
			If IsFeatureAvailable("�˒�����") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "�˒�����" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponMaxRange(i) = intWeaponMaxRange(i) + FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'�Œ�l��1
			If intWeaponMaxRange(i) <= 0 Then
				intWeaponMaxRange(i) = 1
			End If
NextWeapon2: 
		Next 
		
		'���햽�������X�V
		ReDim intWeaponPrecision(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponPrecision(i) = Weapon(i).Precision
			
			'���틭���ɂ��C��
			If IsFeatureAvailable("����������") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "����������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
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
		
		'����̂b�s�����X�V
		ReDim intWeaponCritical(CountWeapon)
		For i = 1 To CountWeapon
			intWeaponCritical(i) = Weapon(i).Critical
			
			'�b�s�������ɂ��C��
			If IsFeatureAvailable("�b�s������") And IsNormalWeapon(i) Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "�b�s������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							intWeaponCritical(i) = intWeaponCritical(i) + 5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'������ʔ����������ɂ��C��
			If IsFeatureAvailable("������ʔ���������") And Not IsNormalWeapon(i) Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "������ʔ���������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕��������
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
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
		
		'�ő�e�����X�V
		ReDim intMaxBullet(CountWeapon)
		'UPGRADE_NOTE: rate �� rate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
		Dim rate_Renamed As Double
		For i = 1 To CountWeapon
			
			intMaxBullet(i) = Weapon(i).Bullet
			
			'�ő�e���̑�����
			rate_Renamed = 0
			
			'�{�X�����N�ɂ��C��
			If intBossRank > 0 Then
				rate_Renamed = 0.2 * BossRank
			End If
			
			'�ő�e�������ɂ��C��
			If IsFeatureAvailable("�ő�e������") Then
				With Weapon(i)
					wname = .Name
					wnickname = WeaponNickname(i)
					wnskill = .NecessarySkill
				End With
				wclass = strWeaponClass(i)
				
				For j = 1 To CountFeature
					If Feature(j) = "�ő�e������" Then
						fdata = FeatureData(j)
						
						'�u"�v������
						If Left(fdata, 1) = """" Then
							fdata = Mid(fdata, 2, Len(fdata) - 2)
						End If
						
						flen = LLength(fdata)
						flag = False
						
						'����w�肪�Ȃ��ꍇ�͂��ׂĂ̕���̒e���𑝉�
						If flen = 0 Then
							flag = True
						End If
						
						'����w�肪����ꍇ�͂��ꂼ��̎w����`�F�b�N
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
								Case "�S"
									found = True
								Case "��"
									If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
								'!�w�肠��
								If found Then
									'�����𖞂������ꍇ�͓K�p���Ȃ�
									flag = False
									false_count = false_count + 1
								End If
							ElseIf found Then 
								'!�w�薳���̏����𖞂�����
								flag = True
							Else
								'!�w�薳���̏����𖞂�����
								false_count = false_count + 1
							End If
						Next 
						
						If flag Or false_count = 0 Then
							rate_Renamed = rate_Renamed + 0.5 * FeatureLevel(j)
						End If
					End If
				Next 
			End If
			
			'�������ɍ��킹�Ēe�����C��
			intMaxBullet(i) = (1 + rate_Renamed) * intMaxBullet(i)
			
			'�ő�l��99
			If intMaxBullet(i) > 99 Then
				intMaxBullet(i) = 99
			End If
			'�Œ�l��0
			If intMaxBullet(i) < 0 Then
				intMaxBullet(i) = 0
			End If
		Next 
		
		'�e�����X�V
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
		
		'�A�r���e�B�f�[�^���X�V
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
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		
		'�g�p�񐔂��X�V
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
		
		If Status_Renamed <> "�o��" Then
			Exit Sub
		End If
		
		'����s�\�H
		If IsFeatureAvailable("����s��") Then
			If Not is_uncontrollable Then
				AddCondition("�\��", -1)
			End If
		Else
			If is_uncontrollable Then
				If IsConditionSatisfied("�\��") Then
					DeleteCondition("�\��")
				End If
			End If
		End If
		
		'�s����H
		If IsFeatureAvailable("�s����") Then
			If Not is_stable Then
				If HP <= MaxHP \ 4 Then
					AddCondition("�\��", -1)
				End If
			End If
		Else
			If is_stable Then
				If IsConditionSatisfied("�\��") Then
					DeleteCondition("�\��")
				End If
			End If
		End If
	End Sub
	
	'����\�͂�o�^
	Private Sub AddFeatures(ByRef fdc As Collection, Optional ByVal is_item As Boolean = False)
		Dim fd As FeatureData
		
		If fdc Is Nothing Then
			Exit Sub
		End If
		
		For	Each fd In fdc
			With fd
				'�A�C�e���Ŏw�肳�ꂽ���L�̔\�͂̓A�C�e�����̂��̂̑����Ȃ̂�
				'���j�b�g���ɂ͒ǉ����Ȃ�
				If is_item Then
					Select Case .Name
						Case "�K�v�Z�\", "�s�K�v�Z�\", "�\��", "��\��", "��"
							GoTo NextFeature
					End Select
				End If
				
				'���󂳂�Ă���H
				If IsDisabled(.Name) Or IsDisabled(LIndex(.StrData, 1)) Then
					GoTo NextFeature
				End If
				
				'���ɂ��̔\�͂��o�^����Ă���H
				If Not IsFeatureRegistered(.Name) Then
					colFeature.Add(fd, .Name)
				Else
					colFeature.Add(fd, .Name & ":" & VB6.Format(colFeature.Count()))
				End If
			End With
NextFeature: 
		Next fd
	End Sub
	
	'����\�͂�o�^�ς݁H
	Private Function IsFeatureRegistered(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = colFeature.Item(fname)
		IsFeatureRegistered = True
		Exit Function
		
ErrorHandler: 
		IsFeatureRegistered = False
	End Function
	
	'����\�͂�o�^�ς݁H(�K�v�����𖞂����Ȃ�����\�͂��܂�)
	Private Function IsAllFeatureRegistered(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		
		fd = colAllFeature.Item(fname)
		IsAllFeatureRegistered = True
		Exit Function
		
ErrorHandler: 
		IsAllFeatureRegistered = False
	End Function
	
	'����\�͂��K�v�����𖞂����Ă��邩�ǂ������肵�A�������Ă��Ȃ��\�͂��폜����
	'fname���w�肳�ꂽ�ꍇ�A�w�肳�ꂽ����\�͂ɑ΂��Ă̂ݕK�v�Z�\�𔻒�
	Private Sub UpdateFeatures(Optional ByVal fname As String = "")
		Dim fd As FeatureData
		Dim farray() As FeatureData
		Dim i As Short
		Dim found As Boolean
		
		If fname <> "" Then
			'�K�v�Z�\�������𖞂����ĂȂ�����\�͂��폜�B
			found = False
			i = 1
			With colFeature
				Do While i <= .Count()
					'�K�v�Z�\�𖞂����Ă���H
					'UPGRADE_WARNING: �I�u�W�F�N�g colFeature.Item(i).Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If fname = .Item(i).Name Then
						'UPGRADE_WARNING: �I�u�W�F�N�g colFeature.Item().NecessaryCondition �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						'UPGRADE_WARNING: �I�u�W�F�N�g colFeature.Item().NecessarySkill �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						If Not IsNecessarySkillSatisfied(.Item(i).NecessarySkill) Or Not IsNecessarySkillSatisfied(.Item(i).NecessaryCondition) Then
							'�K�v�Z�\�������𖞂����Ă��Ȃ��̂ō폜
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
			'�K�v�Z�\�𖞂����ĂȂ�����\�͂��폜�B
			found = False
			i = 1
			With colFeature
				Do While i <= .Count()
					'�K�v�Z�\�𖞂����Ă���H
					'UPGRADE_WARNING: �I�u�W�F�N�g colFeature.Item().NecessarySkill �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If Not IsNecessarySkillSatisfied(.Item(i).NecessarySkill) Then
						'�K�v�Z�\�𖞂����Ă��Ȃ��̂ō폜
						.Remove(i)
						found = True
					Else
						i = i + 1
					End If
				Loop 
			End With
			
			'�K�v������K�p����O�̓���\�͂�ۑ�
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
			
			'�K�v�����𖞂����ĂȂ�����\�͂��폜�B
			i = 1
			With colFeature
				Do While i <= .Count()
					'�K�v�����𖞂����Ă���H
					'UPGRADE_WARNING: �I�u�W�F�N�g colFeature.Item().NecessaryCondition �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If Not IsNecessarySkillSatisfied(.Item(i).NecessaryCondition) Then
						'�K�v�����𖞂����Ă��Ȃ��̂ō폜
						.Remove(i)
						found = True
					Else
						i = i + 1
					End If
				Loop 
			End With
		End If
		
		'����\�͂��폜���ꂽ�ꍇ�A����\�͂̕ێ����肪�������s����悤�ɓ���\�͂�
		'�o�^���Ȃ����K�v������B
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
	
	
	' === ���`�Ԋ֘A���� ===
	
	'���`�Ԃ�o�^
	Public Sub AddOtherForm(ByRef u As Unit)
		colOtherForm.Add(u, u.ID)
	End Sub
	
	'���`�Ԃ��폜
	Public Sub DeleteOtherForm(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colOtherForm.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		'������Ȃ���΃��j�b�g���̂Ō���
		For i = 1 To colOtherForm.Count()
			'UPGRADE_WARNING: �I�u�W�F�N�g colOtherForm(i).Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If colOtherForm.Item(i).Name = Index Then
				colOtherForm.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'���`�Ԃ̑���
	Public Function CountOtherForm() As Short
		CountOtherForm = colOtherForm.Count()
	End Function
	
	'���`��
	Public Function OtherForm(ByRef Index As Object) As Unit
		Dim u As Unit
		Dim uname As String
		Dim i As Short
		
		On Error GoTo ErrorHandler
		OtherForm = colOtherForm.Item(Index)
		Exit Function
		
ErrorHandler: 
		'������Ȃ���΃��j�b�g���̂Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		uname = CStr(Index)
		
		For	Each u In colOtherForm
			If u.Name = uname Then
				OtherForm = u
				Exit Function
			End If
		Next u
		
		'�Y�����郆�j�b�g���Ȃ���΍쐬���Ēǉ�
		If UDList.IsDefined(uname) Then
			u = New Unit
			With u
				.Name = UDList.Item(uname).Name
				.Rank = Rank
				.BossRank = BossRank
				.Party = Party0
				.ID = UList.CreateID(uname)
				.Status_Renamed = "���`��"
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
			ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v��������܂���")
		End If
	End Function
	
	'�w�肵�����`�Ԃ��o�^����Ă��邩�H
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
	
	'�s�v�Ȍ`�Ԃ��폜
	Public Sub DeleteTemporaryOtherForm()
		Dim uarray() As String
		Dim fname, fdata As String
		Dim k, i, j, n As Short
		
		'�K�v�Ȍ`�Ԃ̈ꗗ���쐬
		n = 1
		ReDim uarray(1)
		uarray(1) = Name
		For i = 1 To CountFeature
			fname = Feature(i)
			Select Case fname
				Case "�ό`"
					fdata = FeatureData(fname)
					n = n + LLength(fdata) - 1
					ReDim Preserve uarray(n)
					For j = 1 To LLength(fdata) - 1
						uarray(n - j + 1) = LIndex(fdata, j + 1)
					Next 
				Case "����", "���`��"
					fdata = FeatureData(fname)
					n = n + LLength(fdata)
					ReDim Preserve uarray(n)
					For j = 1 To LLength(fdata)
						uarray(n - j + 1) = LIndex(fdata, j)
					Next 
				Case "�n�C�p�[���[�h", "�p�[�c����", "�ό`�Z"
					fdata = FeatureData(fname)
					n = n + 1
					ReDim Preserve uarray(n)
					uarray(n) = LIndex(fdata, 2)
				Case "�m�[�}�����[�h", "�p�[�c����"
					fdata = FeatureData(fname)
					n = n + 1
					ReDim Preserve uarray(n)
					uarray(n) = LIndex(fdata, 1)
			End Select
		Next 
		
		'���`�Ԃ���K�v�Ȃ��`�Ԃւ̃����N���폜
		For i = 1 To CountOtherForm
			With OtherForm(i)
				If .Status_Renamed = "���`��" Then
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
		
		'�K�v�Ȃ��`�Ԃ�j�����A�����N���폜
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
				OtherForm(i).Status_Renamed = "�j��"
				DeleteOtherForm(i)
			Else
				i = i + 1
			End If
		Loop 
	End Sub
	
	
	' === �p�C���b�g�֘A���� ===
	
	'�p�C���b�g��ǉ�
	Public Sub AddPilot(ByRef p As Pilot)
		colPilot.Add(p, p.ID)
	End Sub
	
	'�p�C���b�g���폜
	Public Sub DeletePilot(ByRef Index As Object)
		colPilot.Remove(Index)
	End Sub
	
	'�p�C���b�g�̓���ւ�
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
		'UPGRADE_NOTE: �I�u�W�F�N�g prev_p.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		prev_p.Unit_Renamed = Nothing
		prev_p.Alive = False
	End Sub
	
	'�������
	Public Function CountPilot() As Short
		CountPilot = colPilot.Count()
	End Function
	
	'�p�C���b�g
	Public Function Pilot(ByRef Index As Object) As Pilot
		Pilot = colPilot.Item(Index)
	End Function
	
	'���C���p�C���b�g
	'�e����ɂ͂��̃p�C���b�g�̔\�͂�p����
	Public Function MainPilot(Optional ByVal without_update As Boolean = False) As Pilot
		Dim pname As String
		Dim p As Pilot
		Dim i As Short
		Dim need_update As Boolean
		
		'�p�C���b�g������Ă��Ȃ��H
		If CountPilot = 0 Then
			If Not IsFeatureAvailable("�ǉ��p�C���b�g") Then
				ErrorMessage("���j�b�g�u" & Name & "�v�Ƀp�C���b�g������Ă��܂���")
				TerminateSRC()
			End If
		End If
		
		'�j�����ꂽ�ꍇ�̓��C���p�C���b�g�̕ύX���s��Ȃ�
		If Status_Renamed = "�j��" Then
			MainPilot = colPilot.Item(1)
			Exit Function
		End If
		
		'�\�̓R�s�[���͓����p�C���b�g�������̃��j�b�g�̃��C���p�C���b�g�Ɏg�p�����̂�h������
		'�ǉ��p�C���b�g�Ɩ\�����p�C���b�g���g�p���Ȃ�
		If IsConditionSatisfied("�\�̓R�s�[") Then
			MainPilot = colPilot.Item(1)
			Exit Function
		End If
		
		'�\�����̓���p�C���b�g
		If IsConditionSatisfied("�\��") Then
			If IsFeatureAvailable("�\�����p�C���b�g") Then
				pname = FeatureData("�\�����p�C���b�g")
				
				If PDList.IsDefined(pname) Then
					pname = PDList.Item(pname).Name
				Else
					ErrorMessage("�\�����p�C���b�g�u" & pname & "�v�̃f�[�^����`����Ă��܂���")
				End If
				
				If PList.IsDefined(pname) Then
					'���ɖ\�����p�C���b�g���쐬�ς�
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
					'�\�����p�C���b�g���쐬����Ă��Ȃ��̂ō쐬����
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
		
		'�ǉ��p�C���b�g������΁A������g�p
		If IsFeatureAvailable("�ǉ��p�C���b�g") Then
			pname = FeatureData("�ǉ��p�C���b�g")
			
			If PDList.IsDefined(pname) Then
				pname = PDList.Item(pname).Name
			Else
				ErrorMessage("�ǉ��p�C���b�g�u" & pname & "�v�̃f�[�^����`����Ă��܂���")
			End If
			
			'�o�^�ς݂̃p�C���b�g���܂��`�F�b�N
			If Not pltAdditionalPilot Is Nothing Then
				If pltAdditionalPilot.Name = pname Then
					MainPilot = pltAdditionalPilot
					With pltAdditionalPilot
						If .IsAdditionalPilot And Not .Unit_Renamed Is Me Then
							.Unit_Renamed = Me
							.Party = Party0
							.Exp = Pilot(1).Exp
							If .Personality <> "�@�B" Then
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
								If .Personality <> "�@�B" Then
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
			
			'���ɓ��悵�Ă���p�C���b�g���猟��
			If CountPilot > 0 Then
				'�P�Ȃ郁�C���p�C���b�g�̌��Ƃ��Ĉ������߁AIsAdditionalPilot�̃t���O�͗��ĂȂ�
				For i = 1 To CountPilot
					If Pilot(i).Name = pname Then
						pltAdditionalPilot = Pilot(i)
						MainPilot = pltAdditionalPilot
						Exit Function
					End If
				Next 
			End If
			
			'���ɍ쐬����Ă���΂�����g��
			'(�����������쐬�\�ȃp�C���b�g�ŁA���̃��j�b�g�̒ǉ��p�C���b�g�Ƃ��ēo�^�ς݂̏ꍇ�͏���)
			If PList.IsDefined(pname) Then
				p = PList.Item(pname)
				If Not p.IsAdditionalPilot Or (InStr(pname, "(�U�R)") = 0 And InStr(pname, "(�ėp)") = 0) Then
					pltAdditionalPilot = p
					With pltAdditionalPilot
						.IsAdditionalPilot = True
						.Party = Party0
						.Level = Pilot(1).Level
						.Exp = Pilot(1).Exp
						If .Personality <> "�@�B" Then
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
			
			'�܂��쐬����Ă��Ȃ��̂ō쐬����
			If CountPilot > 0 Then
				pltAdditionalPilot = PList.Add(pname, Pilot(1).Level, Party0)
				With pltAdditionalPilot
					.IsAdditionalPilot = True
					.Exp = Pilot(1).Exp
					If .Personality <> "�@�B" Then
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
		
		'�����łȂ���Α�P�p�C���b�g���g�p
		MainPilot = colPilot.Item(1)
	End Function
	
	
	'�T�|�[�g�p�C���b�g��ǉ�
	Public Sub AddSupport(ByRef p As Pilot)
		colSupport.Add(p, p.Name)
	End Sub
	
	'�T�|�[�g�p�C���b�g���폜
	Public Sub DeleteSupport(ByRef Index As Object)
		colSupport.Remove(Index)
	End Sub
	
	'�T�|�[�g�p�C���b�g�̓���ւ�
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
		'UPGRADE_NOTE: �I�u�W�F�N�g prev_p.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		prev_p.Unit_Renamed = Nothing
		prev_p.Alive = False
	End Sub
	
	'���T�|�[�g�p�C���b�g��
	Public Function CountSupport() As Short
		CountSupport = colSupport.Count()
	End Function
	
	'�T�|�[�g
	Public Function Support(ByRef Index As Object) As Pilot
		Support = colSupport.Item(Index)
	End Function
	
	'�ǉ��T�|�[�g
	Public Function AdditionalSupport() As Pilot
		Dim pname As String
		Dim p As Pilot
		Dim i As Short
		
		'�ǉ��T�|�[�g�p�C���b�g�̖���
		pname = FeatureData("�ǉ��T�|�[�g")
		
		'�ǉ��T�|�[�g�����݂��Ȃ��H
		If pname = "" Then
			Exit Function
		End If
		
		'���Ƀp�C���b�g������Ă��Ȃ��ꍇ�͖���
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'���ɓo�^�ς݂ł��邩�`�F�b�N
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
		
		'���ɍ쐬����Ă���΂�����g��
		'(���������̃��j�b�g�̒ǉ��T�|�[�g�Ƃ��ēo�^�ς݂̏ꍇ�͏���)
		If PList.IsDefined(pname) Then
			p = PList.Item(pname)
			If Not p.IsAdditionalSupport Or (InStr(pname, "(�U�R)") = 0 And InStr(pname, "(�ėp)") = 0) Then
				pltAdditionalSupport = p
				With pltAdditionalSupport
					.IsAdditionalSupport = True
					.Party = Party0
					.Unit_Renamed = Me
					.Level = Pilot(1).Level
					.Exp = Pilot(1).Exp
					If .Personality <> "�@�B" Then
						.Morale = Pilot(1).Morale
					End If
				End With
				AdditionalSupport = pltAdditionalSupport
				Exit Function
			End If
		End If
		
		'�܂��쐬����Ă��Ȃ��̂ō쐬����
		If Not PDList.IsDefined(pname) Then
			ErrorMessage("�ǉ��T�|�[�g�u" & pname & "�v�̃f�[�^����`����Ă��܂���")
			Exit Function
		End If
		pltAdditionalSupport = PList.Add(pname, Pilot(1).Level, Party0)
		With pltAdditionalSupport
			.IsAdditionalSupport = True
			.Unit_Renamed = Me
			.Exp = Pilot(1).Exp
			If .Personality <> "�@�B" Then
				.Morale = Pilot(1).Morale
			End If
		End With
		AdditionalSupport = pltAdditionalSupport
	End Function
	
	'�����ꂩ�̃p�C���b�g������\�� sname �������Ă��邩����
	Public Function IsSkillAvailable(ByRef sname As String) As Boolean
		Dim i As Short
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'���C���p�C���b�g
		If MainPilot.IsSkillAvailable(sname) Then
			IsSkillAvailable = True
			Exit Function
		End If
		
		'�p�C���b�g�������̏ꍇ�̓��C���p�C���b�g�̔\�݂͂̂��L��
		If Data.PilotNum > 0 Then
			For i = 2 To CountPilot
				If Pilot(i).IsSkillAvailable(sname) Then
					IsSkillAvailable = True
					Exit Function
				End If
			Next 
		End If
		
		'�T�|�[�g
		For i = 1 To CountSupport
			If Support(i).IsSkillAvailable(sname) Then
				IsSkillAvailable = True
				Exit Function
			End If
		Next 
		
		'�ǉ��T�|�[�g
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			If AdditionalSupport.IsSkillAvailable(sname) Then
				IsSkillAvailable = True
				Exit Function
			End If
		End If
		
		IsSkillAvailable = False
	End Function
	
	'�p�C���b�g�S���ɂ��p�C���b�g�\�̓��x��
	Public Function SkillLevel(ByVal sname As String, Optional ByVal default_slevel As Double = 1) As Double
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'�G���A�X���ݒ肳��Ă邩�`�F�b�N
		If ALDList.IsDefined(sname) Then
			sname = ALDList.Item(sname).AliasType(1)
		End If
		
		Select Case sname
			Case "������"
				SkillLevel = SyncLevel
			Case "���"
				SkillLevel = PlanaLevel
			Case "�I�[��"
				SkillLevel = AuraLevel
			Case "���\��"
				SkillLevel = PsychicLevel
			Case "�r�h��", "�؂蕥��"
				SkillLevel = MainPilot.SkillLevel(sname, CStr(1))
			Case "�����o"
				If MaxSkillLevel("�����o", 1) > MaxSkillLevel("�m�o����", 1) Then
					SkillLevel = MaxSkillLevel("�����o", 1)
				Else
					SkillLevel = MaxSkillLevel("�m�o����", 1)
				End If
			Case Else
				SkillLevel = MaxSkillLevel(sname, default_slevel)
		End Select
	End Function
	
	'�p�C���b�g���ł̍ł������p�C���b�g�\�̓��x����Ԃ�
	Private Function MaxSkillLevel(ByRef sname As String, ByVal default_slevel As Double) As Double
		Dim slevel As Double
		Dim i As Short
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'���C���p�C���b�g
		With MainPilot
			If .IsSkillLevelSpecified(sname) Then
				MaxSkillLevel = .SkillLevel(sname)
			ElseIf .IsSkillAvailable(sname) Then 
				MaxSkillLevel = default_slevel
			Else
				MaxSkillLevel = 0
			End If
		End With
		
		'�p�C���b�g�������̏ꍇ�̓��C���p�C���b�g�̔\�݂͂̂��L��
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
		
		'�T�|�[�g
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
		
		'�ǉ��T�|�[�g
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
	
	'���j�b�g�̃I�[���̓��x��
	Public Function AuraLevel(Optional ByVal no_limit As Boolean = False) As Double
		Select Case CountPilot
			Case 0
				Exit Function
			Case 1
				AuraLevel = MainPilot.SkillLevel("�I�[��")
			Case Else
				'�p�C���b�g���Q���ȏ�̏ꍇ�͂Q�l�ڂ̃I�[���͂����Z
				AuraLevel = MainPilot.SkillLevel("�I�[��") + Pilot(2).SkillLevel("�I�[��") / 2
		End Select
		
		'�T�|�[�g�̃I�[���͂����Z
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			AuraLevel = AuraLevel + AdditionalSupport.SkillLevel("�I�[��") / 2
		ElseIf CountSupport > 0 Then 
			AuraLevel = AuraLevel + Support(1).SkillLevel("�I�[��") / 2
		End If
		
		'�I�[���ϊ��탌�x���ɂ�鐧��
		If IsFeatureAvailable("�I�[���ϊ���") And Not no_limit Then
			If IsFeatureLevelSpecified("�I�[���ϊ���") Then
				AuraLevel = MinDbl(AuraLevel, FeatureLevel("�I�[���ϊ���"))
			End If
		End If
	End Function
	
	'���j�b�g�̒��\�̓��x��
	Public Function PsychicLevel(Optional ByVal no_limit As Boolean = False) As Double
		Select Case CountPilot
			Case 0
				Exit Function
			Case 1
				PsychicLevel = MainPilot.SkillLevel("���\��")
			Case Else
				'�p�C���b�g���Q���ȏ�̏ꍇ�͂Q�l�ڂ̒��\�͂����Z
				PsychicLevel = MainPilot.SkillLevel("���\��") + Pilot(2).SkillLevel("���\��") / 2
		End Select
		
		'�T�|�[�g�̃I�[���͂����Z
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			PsychicLevel = PsychicLevel + AdditionalSupport.SkillLevel("���\��") / 2
		ElseIf CountSupport > 0 Then 
			'�T�|�[�g�̒��\�͂����Z
			PsychicLevel = PsychicLevel + Support(1).SkillLevel("���\��") / 2
		End If
		
		'�T�C�L�b�N�h���C�u�ɂ�鐧��
		If IsFeatureAvailable("�T�C�L�b�N�h���C�u") And Not no_limit Then
			If IsFeatureLevelSpecified("�T�C�L�b�N�h���C�u") Then
				PsychicLevel = MinDbl(PsychicLevel, FeatureLevel("�T�C�L�b�N�h���C�u"))
			End If
		End If
	End Function
	
	'���j�b�g�̓�����
	Public Function SyncLevel(Optional ByVal no_limit As Boolean = False) As Double
		If CountPilot = 0 Then
			Exit Function
		End If
		
		SyncLevel = MainPilot.SynchroRate
		
		'�V���N���h���C�u���x���ɂ�鐧��
		If IsFeatureAvailable("�V���N���h���C�u") And Not no_limit Then
			If IsFeatureLevelSpecified("�V���N���h���C�u") Then
				SyncLevel = MinDbl(SyncLevel, FeatureLevel("�V���N���h���C�u"))
			End If
		End If
	End Function
	
	'���j�b�g�̗�̓��x��
	Public Function PlanaLevel(Optional ByVal no_limit As Boolean = False) As Double
		If CountPilot = 0 Then
			Exit Function
		End If
		
		PlanaLevel = MainPilot.Plana
		
		'��͕ϊ��탌�x���ɂ�鐧��
		If IsFeatureAvailable("��͕ϊ���") And Not no_limit Then
			If IsFeatureLevelSpecified("��͕ϊ���") Then
				PlanaLevel = MinDbl(PlanaLevel, FeatureLevel("��͕ϊ���"))
			End If
		End If
	End Function
	
	'�p�C���b�g�S������p�C���b�g�\�͖�������
	Public Function SkillName0(ByVal sname As String) As String
		Dim i As Short
		
		If ALDList.IsDefined(sname) Then
			sname = ALDList.Item(sname).AliasType(1)
		End If
		
		If CountPilot = 0 Then
			SkillName0 = sname
			Exit Function
		End If
		
		'���C���p�C���b�g
		SkillName0 = MainPilot.SkillName0(sname)
		If SkillName0 <> sname Then
			Exit Function
		End If
		
		'�p�C���b�g�������̏ꍇ�̓��C���p�C���b�g�̔\�݂͂̂��L��
		If Data.PilotNum > 0 Then
			For i = 2 To CountPilot
				SkillName0 = Pilot(i).SkillName0(sname)
				If SkillName0 <> sname Then
					Exit Function
				End If
			Next 
		End If
		
		'�T�|�[�g
		For i = 1 To CountSupport
			SkillName0 = Support(i).SkillName0(sname)
			If SkillName0 <> sname Then
				Exit Function
			End If
		Next 
		
		'�ǉ��T�|�[�g
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			SkillName0 = AdditionalSupport.SkillName0(sname)
		End If
	End Function
	
	
	' === �s�����֘A���� ===
	
	'�P�^�[���ɉ\�ȍs����
	Public Function MaxAction(Optional ByVal ignore_en As Boolean = False) As Short
		'�X�e�[�^�X�ُ�H
		If IsConditionSatisfied("�s���s�\") Or IsConditionSatisfied("���") Or IsConditionSatisfied("�Ή�") Or IsConditionSatisfied("����") Or IsConditionSatisfied("����") Or IsConditionSatisfied("�`���[�W") Or IsConditionSatisfied("����") Or IsUnderSpecialPowerEffect("�s���s�\") Then
			Exit Function
		End If
		
		'�d�m�؂�H
		If Not ignore_en Then
			If EN = 0 Then
				If Not IsOptionDefined("�d�m�O���s����") Then
					Exit Function
				End If
			End If
		End If
		
		If CountPilot = 0 Then
			Exit Function
		End If
		
		'�Q��s���\�H
		If IsOptionDefined("�Q��s���\�͎g�p") Then
			If MainPilot.IsSkillAvailable("�Q��s��") Then
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
	
	'�s����������
	Public Sub UseAction()
		Dim max_action As Short
		
		'�Q��s���\�H
		If CountPilot = 0 Then
			max_action = 1
		ElseIf IsOptionDefined("�Q��s���\�͎g�p") Then 
			If MainPilot.IsSkillAvailable("�Q��s��") Then
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
		
		'�ő�s�����܂ōs������ʂ��J�E���g
		UsedAction = MinLng(UsedAction + 1, max_action)
	End Sub
	
	
	' === �X�y�V�����p���[�֘A���� ===
	
	'�e�����ɂ���X�y�V�����p���[�ꗗ
	Public Function SpecialPowerInEffect() As String
		Dim cnd As Condition
		
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				If .ShortName = "��\��" Then
					'�C�x���g��p
					GoTo NextSpecialPower
				End If
				
				If .Duration = "�݂����" Then
					'�݂����͕ʕ\��
					GoTo NextSpecialPower
				End If
				
				SpecialPowerInEffect = SpecialPowerInEffect & .ShortName
			End With
NextSpecialPower: 
		Next cnd
		
		'�݂����͂��΂��Ă���郆�j�b�g��\������
		For	Each cnd In colSpecialPowerInEffect
			With SPDList.Item((cnd.Name))
				If .Duration = "�݂����" Then
					If PList.IsDefined((cnd.StrData)) Then
						SpecialPowerInEffect = SpecialPowerInEffect & .ShortName & "(" & PList.Item((cnd.StrData)).Nickname & ")"
					End If
					Exit Function
				End If
			End With
		Next cnd
	End Function
	
	'���j�b�g���X�y�V�����p���[ sname �̉e�����ɂ��邩�ǂ���
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
	
	'���j�b�g���X�y�V�����p���[���� sptype �̉e�����ɂ��邩�ǂ���
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
	
	'�e�����ɂ���X�y�V�����p���[�̑���
	Public Function CountSpecialPower() As Short
		CountSpecialPower = colSpecialPowerInEffect.Count()
	End Function
	
	'�e�����ɂ���X�y�V�����p���[
	Public Function SpecialPower(ByRef Index As Object) As SpecialPowerData
		Dim cnd As Condition
		
		On Error GoTo ErrorHandler
		
		cnd = colSpecialPowerInEffect.Item(Index)
		SpecialPower = SPDList.Item((cnd.Name))
		
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g SpecialPower ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SpecialPower = Nothing
	End Function
	
	'�X�y�V�����p���[ mname �̌��ʃ��x��
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
	
	'�X�y�V�����p���[�̃f�[�^
	Public Function SpecialPowerData(ByRef Index As Object) As String
		Dim cnd As Condition
		
		On Error GoTo ErrorHandler
		cnd = colSpecialPowerInEffect.Item(Index)
		SpecialPowerData = cnd.StrData
		Exit Function
		
ErrorHandler: 
	End Function
	
	'�X�y�V�����p���[ sname �̌��ʂ�K�p
	Public Sub MakeSpecialPowerInEffect(ByRef sname As String, Optional ByRef sdata As String = "")
		Dim cnd As New Condition
		
		'���łɎg�p����Ă���΂Ȃɂ����Ȃ�
		If IsSpecialPowerInEffect(sname) Then
			Exit Sub
		End If
		
		With cnd
			.Name = sname
			.StrData = sdata
		End With
		
		colSpecialPowerInEffect.Add(cnd, sname)
	End Sub
	
	'�������Ԃ� stype �ł���X�y�V�����p���[�̌��ʂ𔭓���A��菜��
	Public Sub RemoveSpecialPowerInEffect(ByRef stype As String)
		Dim sd As SpecialPowerData
		Dim i As Short
		Dim is_message_form_visible As Boolean
		Dim pid As String
		
		'���b�Z�[�W�E�B���h�E���\������Ă��邩�L�^
		is_message_form_visible = frmMessage.Visible
		
		i = 1
		Do While i <= CurrentForm.CountSpecialPower
			sd = SpecialPower(i)
			
			'�X�y�V�����p���[�̎������Ԃ��w�肵�����̂ƈ�v���Ă��邩�`�F�b�N
			If stype <> sd.Duration Then
				i = i + 1
				GoTo NextSP
			End If
			
			'�������Ԃ��G�^�[���̏ꍇ�A�X�y�V�����p���[�������Ă����G�̃t�F�C�Y
			'������܂Ō��ʂ��폜���Ȃ�
			If stype = "�G�^�[��" Then
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
			
			'��������X�y�V�����p���[�̌��ʂ𔭓�
			If CurrentForm.Status_Renamed = "�o��" Then
				sd.Apply(CurrentForm.MainPilot, CurrentForm, False, True)
			End If
			
			'�X�y�V�����p���[�̌��ʂ��폜
			CurrentForm.RemoveSpecialPowerInEffect2(i)
NextSP: 
		Loop 
		
		'���b�Z�[�W�E�B���h�E��������\������Ă��Ȃ���Ε��Ă���
		If Not is_message_form_visible And frmMessage.Visible Then
			CloseMessageForm()
		End If
	End Sub
	
	'�X�y�V�����p���[ sname �̌��ʂ���菜��
	Public Sub RemoveSpecialPowerInEffect2(ByRef Index As Object)
		On Error GoTo ErrorHandler
		colSpecialPowerInEffect.Remove(Index)
		Exit Sub
ErrorHandler: 
	End Sub
	
	'�S�ẴX�y�V�����p���[�̌��ʂ���菜��
	Public Sub RemoveAllSpecialPowerInEffect()
		Dim i As Short
		
		With colSpecialPowerInEffect
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
	End Sub
	
	'�X�y�V�����p���[�̌��ʂ����j�b�g u �ɃR�s�[����
	Public Sub CopySpecialPowerInEffect(ByRef u As Unit)
		Dim cnd As Condition
		
		For	Each cnd In colSpecialPowerInEffect
			With cnd
				u.MakeSpecialPowerInEffect(.Name, .StrData)
			End With
		Next cnd
	End Sub
	
	
	' === �����Ԋ֘A���� ===
	
	'�����Ԃ�t��
	Public Sub AddCondition(ByRef cname As String, ByVal ltime As Short, Optional ByVal clevel As Double = DEFAULT_LEVEL, Optional ByRef cdata As String = "")
		Dim cnd As Condition
		Dim new_condition As New Condition
		
		'���������Ԃ����ɕt������Ă���H
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
		
		'�����Ԃ�t��
		With new_condition
			.Name = cname
			.Lifetime = ltime
			.Level = clevel
			.StrData = cdata
		End With
		
		colCondition.Add(new_condition, cname)
	End Sub
	
	'�����Ԃ��폜
	Public Sub DeleteCondition(ByRef Index As Object)
		With colCondition.Item(Index)
			colCondition.Remove(Index)
			
			'����\�͕t���̏ꍇ�̓��j�b�g�̃X�e�[�^�X���A�b�v�f�[�g
			'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().StrData �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If Right(.Name, 2) = "�t��" And InStr(.StrData, "�p�C���b�g�\�͕t��") = 0 Then
				Update()
			End If
		End With
	End Sub
	
	Public Sub DeleteCondition0(ByRef Index As Object)
		colCondition.Remove(Index)
	End Sub
	
	'�t�����ꂽ�����Ԃ̑���
	Public Function CountCondition() As Short
		CountCondition = colCondition.Count()
	End Function
	
	'������
	Public Function Condition(ByRef Index As Object) As String
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		Condition = colCondition.Item(Index).Name
	End Function
	
	'�����Ԃ̎c��^�[����
	Public Function ConditionLifetime(ByRef Index As Object) As Short
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Lifetime �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		ConditionLifetime = colCondition.Item(Index).Lifetime
		Exit Function
		
ErrorHandler: 
		ConditionLifetime = -1
	End Function
	
	'�w�肵������\�͂��t������Ă��邩�H
	Public Function IsConditionSatisfied(ByRef Index As Object) As Boolean
		Dim ltime As Short
		
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Lifetime �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		ltime = colCondition.Item(Index).Lifetime
		IsConditionSatisfied = True
		Exit Function
		
ErrorHandler: 
		IsConditionSatisfied = False
	End Function
	
	'�����Ԃ̃��x��
	Public Function ConditionLevel(ByRef Index As Object) As Double
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Level �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		ConditionLevel = colCondition.Item(Index).Level
		Exit Function
		
ErrorHandler: 
		ConditionLevel = 0
	End Function
	
	'�����Ԃ̃��x���̕ύX
	Public Sub SetConditionLevel(ByRef Index As Object, ByVal lv As Double)
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().Level �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		colCondition.Item(Index).Level = lv
ErrorHandler: 
	End Sub
	
	'����\�͂̃f�[�^
	Public Function ConditionData(ByRef Index As Object) As String
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: �I�u�W�F�N�g colCondition.Item().StrData �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		ConditionData = colCondition.Item(Index).StrData
		Exit Function
		
ErrorHandler: 
		ConditionData = ""
	End Function
	
	'����\�͂̎c��^�[�������X�V
	Public Sub UpdateCondition(Optional ByVal decrement_lifetime As Boolean = False)
		Dim cnd As Condition
		Dim update_is_necessary As Boolean
		Dim charge_complete As Boolean
		
		For	Each cnd In colCondition
			With cnd
				If decrement_lifetime Then
					'�c��^�[������1���炷
					If .Lifetime > 0 Then
						.Lifetime = .Lifetime - 1
					End If
				End If
				
				If .Lifetime = 0 Then
					'�c��^�[������0�Ȃ�폜
					colCondition.Remove(.Name)
					
					Select Case .Name
						Case "����"
							'����������
							If Not Master Is Nothing Then
								Master.CurrentForm.DeleteSlave(ID)
								'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
								Master = Nothing
							End If
							Mode = "�ʏ�"
						Case "�`���[�W"
							'�`���[�W����
							charge_complete = True
						Case "�������E"
							'�������E���Ԑ؂�
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
							CloseMessageForm()
							HandleEvent("�j��", MainPilot.ID)
						Case Else
							'����\�͕t��������
							If Right(.Name, 2) = "�t��" Or Right(.Name, 2) = "����" Then
								update_is_necessary = True
							End If
					End Select
				End If
			End With
		Next cnd
		
		'�`���[�W��Ԃ��I��������`���[�W������Ԃɂ���
		If charge_complete Then
			AddCondition("�`���[�W����", 1)
		End If
		
		'���j�b�g�̃X�e�[�^�X�ω�����H
		If update_is_necessary Then
			Update()
		End If
	End Sub
	
	
	' === ����֘A���� ===
	
	'����
	Public Function Weapon(ByVal w As Short) As WeaponData
		Weapon = WData(w)
	End Function
	
	'����̑���
	Public Function CountWeapon() As Short
		CountWeapon = UBound(WData)
	End Function
	
	'����̈���
	Public Function WeaponNickname(ByVal w As Short) As String
		Dim u As Unit
		
		'���̓��̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Me
		WeaponNickname = WData(w).Nickname
		SelectedUnitForEvent = u
	End Function
	
	'����̍U����
	'tarea �͓G�̂���n�`
	Public Function WeaponPower(ByVal w As Short, ByRef tarea As String) As Integer
		Dim pat As Integer
		'�U���␳�ꎞ�ۑ�
		Dim ed_atk As Double
		
		WeaponPower = lngWeaponPower(w)
		
		'�u�́v������������͎c��g�o�ɉ����čU���͂�������
		If IsWeaponClassifiedAs(w, "��") Then
			WeaponPower = WeaponPower + (HP / MaxHP) * 100 * WeaponLevel(w, "��")
		End If
		
		'�u�s�v������������͎c��d�m�ɉ����čU���͂�������
		If IsWeaponClassifiedAs(w, "�s") Then
			If EN >= WeaponENConsumption(w) Then
				WeaponPower = WeaponPower + (EN - WeaponENConsumption(w)) * WeaponLevel(w, "�s")
			End If
		End If
		
		'�_���[�W�Œ蕐��
		Dim wad As Double
		If IsWeaponClassifiedAs(w, "��") Then
			
			'����ꗗ�̏ꍇ�͍U���͂����̂܂ܕ\��
			If tarea = "" Then
				Exit Function
			End If
			
			'�}�b�v�U���͍U���J�n���ɕۑ������U���͂����̂܂܎g��
			If IsWeaponClassifiedAs(w, "�l") Then
				If SelectedMapAttackPower > 0 Then
					WeaponPower = SelectedMapAttackPower
				End If
			End If
			
			'�n�`�K���ɂ��C���݂̂�K�p
			wad = WeaponAdaption(w, tarea)
			
			'�n�`�K���C���J�艺���I�v�V�����̌��ʂ͓K�p���Ȃ�
			If IsOptionDefined("�n�`�K���C���J�艺��") Then
				If IsOptionDefined("�n�`�K���C���ɘa") Then
					wad = wad + 0.1
				Else
					wad = wad + 0.2
				End If
			End If
			
			'�n�`�K�����`�̏ꍇ�ɍU���͂Ɠ����_���[�W��^����悤�ɂ���
			If IsOptionDefined("�n�`�K���C���ɘa") Then
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
		
		'�������j�b�g�̓_���[�W���󂯂�ƍU���͂��ቺ
		If IsFeatureAvailable("�������j�b�g") Then
			WeaponPower = WeaponPower * (50 + 50 * HP / MaxHP) \ 100
		End If
		
		'�W�I�̂���n�`���ݒ肳��Ă��Ȃ��Ƃ��͕���̈ꗗ�\���p�Ȃ̂Ŋe��␳���Ȃ�
		If tarea = "" Then
			Exit Function
		End If
		
		With MainPilot
			If BCList.IsDefined("�U���␳") Then
				'�o�g���R���t�B�O�f�[�^�̐ݒ�ɂ��C��
				If IsWeaponClassifiedAs(w, "��") Then
					pat = (.Infight + .Shooting) \ 2
				ElseIf IsWeaponClassifiedAs(w, "�i���n") Then 
					pat = .Infight
				Else
					pat = .Shooting
				End If
				
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				'UPGRADE_NOTE: �I�u�W�F�N�g BCVariable.DefUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				BCVariable.DefUnit = Nothing
				BCVariable.WeaponNumber = w
				BCVariable.AttackExp = pat
				BCVariable.WeaponPower = WeaponPower
				WeaponPower = BCList.Item("�U���␳").Calculate()
			Else
				'�p�C���b�g�̍U���͂ɂ��C��
				
				If IsWeaponClassifiedAs(w, "��") Then
					WeaponPower = WeaponPower * (.Infight + .Shooting) \ 200
				ElseIf IsWeaponClassifiedAs(w, "�i���n") Then 
					WeaponPower = WeaponPower * .Infight \ 100
				Else
					WeaponPower = WeaponPower * .Shooting \ 100
				End If
				
				'�C�͂ɂ��C��
				If IsOptionDefined("�C�͌��ʏ�") Then
					WeaponPower = WeaponPower * (50 + (.Morale + .MoraleMod) \ 2) \ 100
				Else
					WeaponPower = WeaponPower * (.Morale + .MoraleMod) \ 100
				End If
				
			End If
			
			'�o��
			If HP <= MaxHP \ 4 Then
				If .IsSkillAvailable("�o��") Then
					If IsOptionDefined("�_���[�W�{���ቺ") Then
						WeaponPower = 1.1 * WeaponPower
					Else
						WeaponPower = 1.2 * WeaponPower
					End If
				End If
			End If
		End With
		
		'�}�b�v�U���p�ɍU���͎Z�o
		If tarea = "�����l" Then
			Exit Function
		End If
		
		'�}�b�v�U���͍U���J�n���ɕۑ������U���͂����̂܂܎g��
		If IsWeaponClassifiedAs(w, "�l") Then
			If SelectedMapAttackPower > 0 Then
				WeaponPower = SelectedMapAttackPower
			End If
		End If
		
		'�n�`�␳
		If BCList.IsDefined("�U���n�`�␳") Then
			'���O�Ƀf�[�^��o�^
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			'UPGRADE_NOTE: �I�u�W�F�N�g BCVariable.DefUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			BCVariable.DefUnit = Nothing
			BCVariable.WeaponNumber = w
			BCVariable.AttackExp = WeaponPower
			BCVariable.TerrainAdaption = WeaponAdaption(w, tarea)
			WeaponPower = BCList.Item("�U���n�`�␳").Calculate()
		Else
			WeaponPower = WeaponPower * WeaponAdaption(w, tarea)
		End If
	End Function
	
	'���� w �̒n�` tarea �ɂ�����_���[�W�C���l
	Public Function WeaponAdaption(ByVal w As Short, ByRef tarea As String) As Double
		Dim wad, uad, xad As Short
		Dim ind As Short
		
		'����̒n�`�K���l�̌v�Z�Ɏg�p����K���l������
		Select Case tarea
			Case "��"
				ind = 1
			Case "�n��"
				If TerrainClass(x, y) = "����" Then
					ind = 4
				Else
					ind = 2
				End If
			Case "����"
				If Mid(Weapon(w).Adaption, 3, 1) = "A" Then
					ind = 3
				Else
					ind = 2
				End If
			Case "����"
				ind = 3
			Case "�F��"
				ind = 4
			Case "�n��"
				WeaponAdaption = 0
				Exit Function
			Case Else
				xad = 4
				GoTo CalcAdaption
		End Select
		
		'����̒n�`�K���l
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
		
		'���j�b�g�̒n�`�K���l�̌v�Z�Ɏg�p����K���l������
		If Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "��") Then
			'�i����ȊO�̏ꍇ�̓��j�b�g������n�`���Q��
			Select Case Area
				Case "��"
					ind = 1
				Case "�n��"
					If TerrainClass(x, y) = "����" Then
						ind = 4
					Else
						ind = 2
					End If
				Case "����"
					ind = 2
				Case "����"
					ind = 3
				Case "�F��"
					ind = 4
				Case "�n��"
					WeaponAdaption = 0
					Exit Function
			End Select
			'���j�b�g�̒n�`�K���l
			uad = Adaption(ind)
		Else
			'�i����̏ꍇ�̓^�[�Q�b�g������n�`���Q��
			Select Case tarea
				Case "��"
					uad = Adaption(1)
					'�W�����v�U��
					If IsWeaponClassifiedAs(w, "�i") Then
						uad = uad + WeaponLevel(w, "�i")
					End If
				Case "�n��"
					If Adaption(2) > 0 Then
						uad = Adaption(2)
					Else
						'�󒆐�p���j�b�g���n��̃��j�b�g�Ɋi���������������悤�ɂ���
						uad = MaxLng(Adaption(1) - 1, 0)
					End If
				Case "����"
					'������p���j�b�g������̃��j�b�g�Ɋi���������������悤�ɂ���
					uad = MaxDbl(Adaption(2), Adaption(3))
					If uad <= 0 Then
						'�󒆐�p���j�b�g���n��̃��j�b�g�Ɋi���������������悤�ɂ���
						uad = MaxLng(Adaption(1) - 1, 0)
					End If
				Case "����"
					uad = Adaption(3)
				Case "�F��"
					uad = Adaption(4)
					If Area = "�n��" And TerrainClass(x, y) = "����" Then
						'���ʂ���̃W�����v�U��
						If IsWeaponClassifiedAs(w, "�i") Then
							uad = uad + WeaponLevel(w, "�i")
						End If
					End If
				Case Else
					uad = Adaption(ind)
			End Select
		End If
		
		'�n�`�K�����������ɓK�������ꍇ�A���j�b�g�̒n�`�K���͍U���ۂ̔���ɂ̂ݗp����
		If IsOptionDefined("�n�`�K���������C��") Then
			If uad > 0 Then
				xad = wad
				GoTo CalcAdaption
			Else
				WeaponAdaption = 0
				Exit Function
			End If
		End If
		
		'���푤�ƃ��j�b�g���̒n�`�K���̒Ⴂ����D��
		If uad > wad Then
			xad = wad
		Else
			xad = uad
		End If
		
CalcAdaption: 
		
		'Option�R�}���h�̐ݒ�ɏ]���Ēn�`�K���l���Z�o
		If IsOptionDefined("�n�`�K���C���ɘa") Then
			If IsOptionDefined("�n�`�K���C���J�艺��") Then
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
			If IsOptionDefined("�n�`�K���C���J�艺��") Then
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
	
	'���� w �̍ő�˒�
	Public Function WeaponMaxRange(ByVal w As Short) As Short
		WeaponMaxRange = intWeaponMaxRange(w)
		
		'�ő�˒������Ƃ��ƂP�Ȃ炻��ȏ�ω����Ȃ�
		If WeaponMaxRange = 1 Then
			Exit Function
		End If
		
		'�}�b�v�U���ɂ͓K�p����Ȃ�
		If IsWeaponClassifiedAs(w, "�l") Then
			Exit Function
		End If
		
		'�ڋߐ핐��ɂ͓K�p����Ȃ�
		If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
			Exit Function
		End If
		
		'�L�����U���U���ɂ͓K�p����Ȃ�
		If IsWeaponClassifiedAs(w, "�L") Then
			Exit Function
		End If
		
		'�X�y�V�����p���[�ɂ��˒�����
		If IsUnderSpecialPowerEffect("�˒�����") Then
			WeaponMaxRange = WeaponMaxRange + SpecialPowerEffectLevel("�˒�����")
		End If
	End Function
	
	'���� w �̏���d�m
	Public Function WeaponENConsumption(ByVal w As Short) As Short
		'UPGRADE_NOTE: rate �� rate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
		Dim rate_Renamed As Double
		Dim i As Short
		
		With Weapon(w)
			WeaponENConsumption = .ENConsumption
			
			'�p�C���b�g�̔\�͂ɂ���ďp�y�ыZ�̏���d�m�͌�������
			If CountPilot > 0 Then
				'�p�ɊY�����邩�H
				If IsSpellWeapon(w) Then
					'�p�ɊY������ꍇ�͏p�Z�\�ɂ���Ăd�m����ʂ�ς���
					Select Case MainPilot.SkillLevel("�p")
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
				
				'�Z�ɊY�����邩�H
				If IsFeatWeapon(w) Then
					'�Z�ɊY������ꍇ�͋Z�Z�\�ɂ���Ăd�m����ʂ�ς���
					Select Case MainPilot.SkillLevel("�Z")
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
			
			'�d�m������\�͂ɂ��C��
			rate_Renamed = 1
			If IsFeatureAvailable("�d�m�����") Then
				For i = 1 To CountFeature
					If Feature(i) = "�d�m�����" Then
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
	
	'���� w �̖�����
	Public Function WeaponPrecision(ByVal w As Short) As Short
		WeaponPrecision = intWeaponPrecision(w)
	End Function
	
	'���� w �̂b�s��
	Public Function WeaponCritical(ByVal w As Short) As Short
		WeaponCritical = intWeaponCritical(w)
	End Function
	
	'���� w �̑���
	Public Function WeaponClass(ByVal w As Short) As String
		WeaponClass = strWeaponClass(w)
	End Function
	
	'���� w �����푮�� attr �������Ă��邩�ǂ���
	Public Function IsWeaponClassifiedAs(ByVal w As Short, ByRef attr As String) As Boolean
		Dim wclass As String
		
		wclass = strWeaponClass(w)
		
		'�������Q�����ȉ��Ȃ炻�̂܂ܔ���
		If Len(attr) <= 2 Then
			If InStrNotNest(wclass, attr) > 0 Then
				IsWeaponClassifiedAs = True
			Else
				IsWeaponClassifiedAs = False
			End If
			Exit Function
		End If
		
		'�����̓���������U���Ȃ炻�̂܂ܔ���
		If InStr("�����", Left(attr, 1)) > 0 Then
			If InStrNotNest(wclass, attr) > 0 Then
				IsWeaponClassifiedAs = True
			Else
				IsWeaponClassifiedAs = False
			End If
			Exit Function
		End If
		
		'���������G�ȏꍇ
		Select Case attr
			Case "�i���n"
				If InStrNotNest(wclass, "�i") > 0 Then
					IsWeaponClassifiedAs = True
				ElseIf InStrNotNest(wclass, "��") > 0 Then 
					IsWeaponClassifiedAs = False
				ElseIf Weapon(w).MaxRange = 1 Then 
					IsWeaponClassifiedAs = True
				Else
					IsWeaponClassifiedAs = False
				End If
				Exit Function
			Case "�ˌ��n"
				If InStrNotNest(wclass, "�i") > 0 Then
					IsWeaponClassifiedAs = False
				ElseIf InStrNotNest(wclass, "��") > 0 Then 
					IsWeaponClassifiedAs = True
				ElseIf Weapon(w).MaxRange = 1 Then 
					IsWeaponClassifiedAs = False
				Else
					IsWeaponClassifiedAs = True
				End If
			Case "�ړ���U����"
				If IsUnderSpecialPowerEffect("�S����ړ���g�p�\") And InStrNotNest(wclass, "�l") = 0 And InStrNotNest(wclass, "�p") = 0 Then
					IsWeaponClassifiedAs = True
				ElseIf Weapon(w).MaxRange = 1 Then 
					If InStrNotNest(wclass, "�p") = 0 Then
						IsWeaponClassifiedAs = True
					Else
						IsWeaponClassifiedAs = False
					End If
				ElseIf InStrNotNest(wclass, "�o") > 0 Then 
					IsWeaponClassifiedAs = True
				End If
		End Select
	End Function
	
	'���� w �̑��� attr �ɂ����郌�x��
	Public Function WeaponLevel(ByVal w As Short, ByRef attr As String) As Double
		Dim attrlv, wclass As String
		Dim start_idx, i As Short
		Dim c As String
		
		On Error GoTo ErrorHandler
		
		attrlv = attr & "L"
		
		'���푮���𒲂ׂĂ݂�
		wclass = strWeaponClass(w)
		
		'���x���w�肪���邩�H
		start_idx = InStrNotNest(wclass, attrlv)
		If start_idx = 0 Then
			Exit Function
		End If
		
		'���x���w�蕔���̐؂�o��
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
		ErrorMessage(Name & "��" & "�����u" & Weapon(w).Name & "�v��" & "�����u" & attr & "�v�̃��x���w�肪�s���ł�")
	End Function
	
	'���� w �̑��� attr �Ƀ��x���w�肪�Ȃ���Ă��邩
	Public Function IsWeaponLevelSpecified(ByVal w As Short, ByRef attr As String) As Boolean
		If InStr(strWeaponClass(w), attr & "L") > 0 Then
			IsWeaponLevelSpecified = True
			Exit Function
		End If
	End Function
	
	'���� w ���ʏ핐�킩�ǂ���
	Public Function IsNormalWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim wclass As String
		Dim ret As Short
		
		'������ʑ��������H
		wclass = strWeaponClass(w)
		For i = 1 To Len(wclass)
			ret = InStr("�r���򒆐Γ�ზ��������x���]�Q�ߖӓŊh�s�~�ُ������E�c�ᐁ�j���]���œ��K�ʉ������", Mid(wclass, i, 1))
			If ret > 0 Then
				Exit Function
			End If
		Next 
		
		IsNormalWeapon = True
	End Function
	
	'���킪��������ʂ̐���Ԃ�
	Public Function CountWeaponEffect(ByVal w As Short) As Short
		Dim wclass, wattr As String
		Dim i, ret As Short
		
		wclass = strWeaponClass(w)
		For i = 1 To Len(wclass)
			'��r�̂悤�ȓ���q������΁A����q�̕��J�E���g��i�߂�
			wattr = GetClassBundle(wclass, i, 1)
			
			'��\�������͖���
			If wattr = "|" Then
				Exit For
			End If
			
			'�b�s�������n
			ret = InStr("�r���򒆐Γ�ზ��������x���]�Q�ߖӓŊh�s�~�ُ������E�c�ᐁ�j���]���œ��K�ʉ������", wattr)
			If ret > 0 Then
				CountWeaponEffect = CountWeaponEffect + 1
			End If
			
			'����ȊO
			ret = InStr("��ĔE�ьŎE���Z�j�ԏ�z���D", wattr)
			If ret > 0 Then
				CountWeaponEffect = CountWeaponEffect + 1
			End If
		Next 
	End Function
	
	'���� w ���p���ǂ���
	Public Function IsSpellWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsWeaponClassifiedAs(w, "�p") Then
			IsSpellWeapon = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Weapon(w).NecessarySkill))
				nskill = LIndex((Weapon(w).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "�p" Then
					IsSpellWeapon = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'���� w ���Z���ǂ���
	Public Function IsFeatWeapon(ByVal w As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsWeaponClassifiedAs(w, "�Z") Then
			IsFeatWeapon = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Weapon(w).NecessarySkill))
				nskill = LIndex((Weapon(w).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "�Z" Then
					IsFeatWeapon = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'���� w ���g�p�\���ǂ���
	'ref_mode �̓��j�b�g�̏�ԁi�ړ��O�A�ړ���j������
	Public Function IsWeaponAvailable(ByVal w As Short, ByRef ref_mode As String) As Boolean
		Dim i As Short
		Dim wd As WeaponData
		Dim wclass As String
		
		IsWeaponAvailable = False
		
		' ADD START MARGE
		'���킪�擾�ł��Ȃ��ꍇ��False�i�h��△��R�̏ꍇ�Aw��0��-1�ɂȂ�j
		If Not (w > 0) Then
			Exit Function
		End If
		' ADD END MARGE
		
		wd = Weapon(w)
		wclass = WeaponClass(w)
		
		'�C�x���g�R�}���h�uDisable�v�ŕ��󂳂�Ă���H
		If IsDisabled((wd.Name)) Then
			Exit Function
		End If
		
		'�p�C���b�g������Ă��Ȃ���Ώ�Ɏg�p�\�Ɣ���
		If CountPilot = 0 Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'�K�v�Z�\���K�v����
		If ref_mode <> "�K�v�Z�\����" Then
			If Not IsWeaponMastered(w) Then
				Exit Function
			End If
			
			If Not IsWeaponEnabled(w) Then
				Exit Function
			End If
		End If
		
		'�X�e�[�^�X�\���ł͕K�v�Z�\�����������Ă���΂n�j
		If ref_mode = "�C���^�[�~�b�V����" Or ref_mode = "" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		With MainPilot
			'�K�v�C��
			If wd.NecessaryMorale > 0 Then
				If .Morale < wd.NecessaryMorale Then
					Exit Function
				End If
			End If
			
			'��͏���U��
			If InStrNotNest(wclass, "��") > 0 Then
				If .Plana < WeaponLevel(w, "��") * 5 Then
					Exit Function
				End If
			ElseIf InStrNotNest(wclass, "�v") > 0 Then 
				If .Plana < WeaponLevel(w, "�v") * 5 Then
					Exit Function
				End If
			End If
		End With
		
		'�����g�p�s�\���
		If ConditionLifetime("�I�[���g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�I") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("���\�͎g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "��") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�������g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�V") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�����o�g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�T") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�m�o�����g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�T") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("��͎g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "��") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�p�g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�p") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�Z�g�p�s�\") > 0 Then
			If IsWeaponClassifiedAs(w, "�Z") Then
				Exit Function
			End If
		End If
		For i = 1 To CountCondition
			If Len(Condition(i)) > 6 Then
				If Right(Condition(i), 6) = "�����g�p�s�\" Then
					If InStrNotNest(WeaponClass(w), Left(Condition(i), Len(Condition(i)) - 6)) > 0 Then
						Exit Function
					End If
				End If
			End If
		Next 
		
		'�e��������邩
		If wd.Bullet > 0 Then
			If Bullet(w) < 1 Then
				Exit Function
			End If
		End If
		
		'�d�m������邩
		If wd.ENConsumption > 0 Then
			If EN < WeaponENConsumption(w) Then
				Exit Function
			End If
		End If
		
		'����������邩�c�c
		If Party = "����" Then
			If InStrNotNest(wclass, "�K") > 0 Then
				If Money < MaxLng(WeaponLevel(w, "�K"), 1) * Value \ 10 Then
					Exit Function
				End If
			End If
		End If
		
		'�U���s�\�H
		If ref_mode <> "�X�e�[�^�X" Then
			If IsConditionSatisfied("�U���s�\") Then
				Exit Function
			End If
		End If
		If Area = "�n��" Then
			Exit Function
		End If
		
		'�ړ��s�\���ɂ͈ړ��^�}�b�v�U���͎g�p�s�\
		If IsConditionSatisfied("�ړ��s�\") Then
			If InStrNotNest(wclass, "�l��") > 0 Then
				Exit Function
			End If
		End If
		
		'�p����щ��͒��ُ�Ԃł͎g�p�s�\
		If IsConditionSatisfied("����") Then
			If IsSpellWeapon(w) Or InStrNotNest(wclass, "��") > 0 Then
				Exit Function
			End If
		End If
		
		'���̋Z�̏���
		If InStrNotNest(wclass, "��") > 0 Then
			If Not IsCombinationAttackAvailable(w) Then
				Exit Function
			End If
		End If
		
		'�ό`�Z�̏ꍇ�͍�����n�`�ŕό`�ł���K�v����
		If InStrNotNest(wclass, "��") > 0 Then
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = wd.Name Then
						If Not OtherForm(LIndex(FeatureData(i), 2)).IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End If
				Next 
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				If Not OtherForm(LIndex(FeatureData("�m�[�}�����[�h"), 1)).IsAbleToEnter(x, y) Then
					Exit Function
				End If
			End If
			If IsConditionSatisfied("�`�ԌŒ�") Then
				Exit Function
			End If
			If IsConditionSatisfied("�@�̌Œ�") Then
				Exit Function
			End If
		End If
		
		'�m��������
		If InStrNotNest(wclass, "�m") > 0 Then
			If HP > MaxHP \ 4 Then
				Exit Function
			End If
		End If
		
		'�����`���[�W�U�����ď[�U��
		If IsConditionSatisfied(WeaponNickname(w) & "�[�U��") Then
			Exit Function
		End If
		'���L���큕�A�r���e�B���[�U���̏ꍇ���g�p�s��
		Dim lv As Short
		If InStrNotNest(wclass, "��") > 0 Then
			lv = WeaponLevel(w, "��")
			For i = 1 To CountWeapon
				If IsWeaponClassifiedAs(i, "��") Then
					If lv = WeaponLevel(i, "��") Then
						If IsConditionSatisfied(WeaponNickname(i) & "�[�U��") Then
							Exit Function
						End If
					End If
				End If
			Next 
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "��") Then
					If lv = AbilityLevel(i, "��") Then
						If IsConditionSatisfied(AbilityNickname(i) & "�[�U��") Then
							Exit Function
						End If
					End If
				End If
			Next 
		End If
		
		'�\�̓R�s�[
		If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
			If IsFeatureAvailable("�m�[�}�����[�h") Then
				'���ɕϐg�ς݂̏ꍇ�̓R�s�[�o���Ȃ�
				Exit Function
			End If
		End If
		
		'�g�p�֎~
		If InStrNotNest(wclass, "��") > 0 Then
			Exit Function
		End If
		
		'�`���[�W����ł���΂����܂łłn�j
		If ref_mode = "�`���[�W" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'�`���[�W���U��
		If InStrNotNest(wclass, "�b") > 0 Then
			If Not IsConditionSatisfied("�`���[�W����") Then
				Exit Function
			End If
		End If
		
		If ref_mode = "�X�e�[�^�X" Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'�������ǂ����̔���
		'���R�̃t�F�C�Y�łȂ���Δ������ł���
		If Party <> Stage Then
			'�����ł̓}�b�v�U���A���̋Z�͎g�p�ł��Ȃ�
			If InStrNotNest(wclass, "�l") > 0 Or InStrNotNest(wclass, "��") > 0 Then
				Exit Function
			End If
			
			'�U����p����
			If InStrNotNest(wclass, "�U") > 0 Then
				For i = 1 To Len(wclass)
					If Mid(wclass, i, 1) = "�U" Then
						If i = 1 Then
							Exit Function
						End If
						If Mid(wclass, i - 1, 1) <> "��" Then
							Exit Function
						End If
					End If
				Next 
			End If
		Else
			'������p�U��
			If InStrNotNest(wclass, "��") > 0 Then
				Exit Function
			End If
		End If
		
		'�ړ��O���ォ�c�c
		If ref_mode = "�ړ��O" Or ref_mode = "�K�v�Z�\����" Or Not SelectedUnit Is Me Then
			IsWeaponAvailable = True
			Exit Function
		End If
		
		'�ړ���̏ꍇ
		If IsUnderSpecialPowerEffect("�S����ړ���g�p�\") And Not InStrNotNest(wclass, "�l") > 0 Then
			IsWeaponAvailable = True
		ElseIf WeaponMaxRange(w) > 1 Then 
			If InStrNotNest(wclass, "�o") > 0 Then
				IsWeaponAvailable = True
			Else
				IsWeaponAvailable = False
			End If
		Else
			If InStrNotNest(wclass, "�p") > 0 Then
				IsWeaponAvailable = False
			Else
				IsWeaponAvailable = True
			End If
		End If
	End Function
	
	'���� w �̎g�p�Z�\�𖞂����Ă��邩�B
	Public Function IsWeaponMastered(ByVal w As Short) As Boolean
		IsWeaponMastered = IsNecessarySkillSatisfied((Weapon(w).NecessarySkill))
	End Function
	
	'���� w �̎g�p�����𖞂����Ă��邩�B
	Public Function IsWeaponEnabled(ByVal w As Short) As Boolean
		IsWeaponEnabled = IsNecessarySkillSatisfied((Weapon(w).NecessaryCondition))
	End Function
	
	'���킪�g�p�\�ł���A���˒����ɓG�����邩�ǂ���
	Public Function IsWeaponUseful(ByVal w As Short, ByRef ref_mode As String) As Boolean
		Dim i, j As Short
		Dim u As Unit
		Dim max_range As Short
		
		'���킪�g�p�\���H
		If Not IsWeaponAvailable(w, ref_mode) Then
			IsWeaponUseful = False
			Exit Function
		End If
		
		'��^�}�b�v�U���͓���Ȃ̂Ŕ��肪�ł��Ȃ�
		'�ړ��^�}�b�v�U���͈ړ���i�Ƃ��Ďg�����Ƃ��l��
		If IsWeaponClassifiedAs(w, "�l��") Or IsWeaponClassifiedAs(w, "�l��") Then
			IsWeaponUseful = True
			Exit Function
		End If
		
		max_range = WeaponMaxRange(w)
		
		'�����^�}�b�v�U���͌��ʔ͈͂��L��
		max_range = max_range + WeaponLevel(w, "�l��")
		
		'�G�̑��ݔ���
		For i = MaxLng(x - max_range, 1) To MinLng(x + max_range, MapWidth)
			For j = MaxLng(y - max_range, 1) To MinLng(y + max_range, MapHeight)
				u = MapDataForUnit(i, j)
				If u Is Nothing Then
					GoTo NextUnit
				End If
				With u
					Select Case Party
						Case "����", "�m�o�b"
							Select Case .Party
								Case "����", "�m�o�b"
									'�X�e�[�^�X�ُ�̏ꍇ�͖������j�b�g�ł��r���\
									If Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�߈�") And Not .IsConditionSatisfied("����") Then
										GoTo NextUnit
									End If
							End Select
						Case Else
							If Party = .Party Then
								'�X�e�[�^�X�ُ�̏ꍇ�͖������j�b�g�ł��r���\
								If Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�߈�") Then
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
		
		'�G�͌�����Ȃ�����
		IsWeaponUseful = False
	End Function
	
	
	'���j�b�g t ������ w �̎˒��͈͓��ɂ��邩���`�F�b�N
	Public Function IsTargetWithinRange(ByVal w As Short, ByRef t As Unit) As Boolean
		Dim max_range, distance, range_mod As Short
		Dim lv As Short
		
		IsTargetWithinRange = True
		
		Dim partners() As Unit
		With t
			'�������Z�o
			distance = System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
			
			'�l������͖ڕW�n�_���炳��Ɍ��ʔ͈͂��L�т�̂Ŏ˒��C�����s��
			range_mod = WeaponLevel(w, "�l��")
			
			'�ő�˒��`�F�b�N
			max_range = WeaponMaxRange(w)
			If distance > max_range + range_mod Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'�ŏ��˒��`�F�b�N
			If distance < Weapon(w).MinRange - range_mod Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'�G���X�e���X�̏ꍇ
			If .IsFeatureAvailable("�X�e���X") Then
				If .IsFeatureLevelSpecified("�X�e���X") Then
					lv = .FeatureLevel("�X�e���X")
				Else
					lv = 3
				End If
				If Not .IsConditionSatisfied("�X�e���X����") And Not IsFeatureAvailable("�X�e���X������") And distance > lv Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
			
			'�B��g���H
			If .IsUnderSpecialPowerEffect("�B��g") Then
				If Not .IsUnderSpecialPowerEffect("���h��") Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
			
			'�U���ł��Ȃ��n�`�ɂ���ꍇ�͎˒��O�Ƃ݂Ȃ�
			If WeaponAdaption(w, .Area) = 0 Then
				IsTargetWithinRange = False
				Exit Function
			End If
			
			'���̋Z�Ŏ˒����P�̏ꍇ�͑�����͂�ł���K�v������
			If IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "�l") And max_range = 1 Then
				CombinationPartner("����", w, partners, .x, .y)
				If UBound(partners) = 0 Then
					IsTargetWithinRange = False
					Exit Function
				End If
			End If
		End With
	End Function
	
	'�ړ��𕹗p�����ꍇ�Ƀ��j�b�g t ������ w �̎˒��͈͓��ɂ��邩���`�F�b�N
	Public Function IsTargetReachable(ByVal w As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim max_range, min_range As Short
		
		Dim partners() As Unit
		With t
			'�n�`�K�����`�F�b�N
			If WeaponAdaption(w, .Area) = 0 Then
				IsTargetReachable = False
				Exit Function
			End If
			
			'�B��g�g�p���H
			If .IsUnderSpecialPowerEffect("�B��g") Then
				If Not .IsUnderSpecialPowerEffect("���h��") Then
					IsTargetReachable = False
					Exit Function
				End If
			End If
			
			'�˒��v�Z
			min_range = Weapon(w).MinRange
			max_range = WeaponMaxRange(w)
			'�G���X�e���X�̏ꍇ
			If .IsFeatureAvailable("�X�e���X") And Not .IsConditionSatisfied("�X�e���X����") And Not IsFeatureAvailable("�X�e���X������") Then
				If .IsFeatureLevelSpecified("�X�e���X") Then
					max_range = MinLng(max_range, .FeatureLevel("�X�e���X") + 1)
				Else
					max_range = MinLng(max_range, 4)
				End If
			End If
			
			'�אڂ��Ă���ΕK���͂�
			If min_range = 1 And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
				'���������̋Z�̏ꍇ�͗�O�c�c
				'���̋Z�Ŏ˒����P�̏ꍇ�͑�����͂�ł���K�v������
				If IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "�l") And WeaponMaxRange(w) = 1 Then
					CombinationPartner("����", w, partners, t.x, t.y)
					If UBound(partners) = 0 Then
						IsTargetReachable = False
						Exit Function
					End If
				End If
				IsTargetReachable = True
				Exit Function
			End If
			
			'�ړ��͈͂���G�ɍU�����͂������`�F�b�N
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
	
	'���� w �̃��j�b�g t �ɑ΂��閽����
	'�G���j�b�g�̓X�y�V�����p���[���ɂ��␳���l�����Ȃ��̂�
	' is_true_value �ɂ���ĕ␳���Ȃ����ǂ������w��ł���悤�ɂ��Ă���
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
		'�����A����A�n�`�␳�A�T�C�Y�␳�̐��l���`
		Dim ed_hit, ed_avd As Integer
		Dim ed_aradap, ed_size As Double
		
		'�����l
		ed_aradap = 1
		
		'�X�y�V�����p���[�ɂ��̂Đg���
		If t.IsUnderSpecialPowerEffect("���h��") Then
			HitProbability = 100
			Exit Function
		End If
		
		'�p�C���b�g�̋Z�ʂɂ���Ė������𐳊m�ɗ\���ł��邩���E�����
		mpskill = MainPilot.TacticalTechnique
		
		'�X�y�V�����p���[�ɂ��e��
		If is_true_value Or mpskill >= 160 Then
			If t.IsUnderSpecialPowerEffect("��Ή��") Then
				HitProbability = 0
				Exit Function
			End If
			If IsUnderSpecialPowerEffect("��Ζ���") Then
				HitProbability = 1000
				Exit Function
			End If
		End If
		
		'�����j�b�g�ɂ��C��
		With MainPilot
			If BCList.IsDefined("�����␳") Then
				'�������ꎞ�ۑ�
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.AttackExp = WeaponPrecision(w)
				ed_hit = BCList.Item("�����␳").Calculate()
			Else
				'�������ꎞ�ۑ�
				ed_hit = 100 + .Hit + .Intuition + Mobility() + WeaponPrecision(w)
			End If
		End With
		
		'�G���j�b�g�ɂ��C��
		With t.MainPilot
			If BCList.IsDefined("���␳") Then
				'������ꎞ�ۑ�
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = t
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				ed_avd = BCList.Item("���␳").Calculate()
			Else
				'������ꎞ�ۑ�
				ed_avd = .Dodge + .Intuition + t.Mobility()
			End If
		End With
		
		'�n�`�K���A�T�C�Y�C���̈ʒu��ύX
		Dim uadaption As Double
		Dim tarea As String
		Dim tx, ty As Short
		With t
			'�n�`�C��
			If .Area <> "��" And (.Area <> "�F��" Or TerrainClass(.x, .y) <> "����") Then
				'�n�`�C�����ꎞ�ۑ�
				ed_aradap = ed_aradap * (100 - TerrainEffectForHit(.x, .y)) / 100
			End If
			
			'�n�`�K���C��
			If IsOptionDefined("�n�`�K���������C��") Then
				
				'�ڋߐ�U���̏ꍇ�̓^�[�Q�b�g���̒n�`���Q��
				If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
					tarea = .Area
					tx = .x
					ty = .y
				Else
					tarea = Area
					tx = x
					ty = y
				End If
				
				Select Case tarea
					Case "��"
						uadaption = AdaptionMod(1)
						'�W�����v�U���̏ꍇ�͂i�����ɂ��C����������
						If (.Area = "��" Or .Area = "�F��") And Area <> "��" And Area <> "�F��" And Not IsTransAvailable("��") Then
							If InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Then
								uadaption = AdaptionMod(1, WeaponLevel(w, "�i"))
							End If
						End If
					Case "�n��"
						If TerrainClass(tx, ty) = "����" Then
							uadaption = AdaptionMod(4)
						Else
							uadaption = AdaptionMod(2)
						End If
					Case "����"
						uadaption = AdaptionMod(2)
					Case "����"
						uadaption = AdaptionMod(3)
					Case "�F��"
						uadaption = AdaptionMod(4)
					Case "�n��"
						HitProbability = 0
						Exit Function
				End Select
				
				'�n�`�C�����ꎞ�ۑ�
				ed_aradap = ed_aradap * uadaption
			End If
			
			'�T�C�Y�␳
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
		
		'�������v�Z���s
		If BCList.IsDefined("������") Then
			'���O�Ƀf�[�^��o�^
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			BCVariable.DefUnit = t
			BCVariable.WeaponNumber = w
			BCVariable.AttackVariable = ed_hit
			BCVariable.DffenceVariable = ed_avd
			BCVariable.TerrainAdaption = ed_aradap
			BCVariable.SizeMod = ed_size
			prob = BCList.Item("������").Calculate()
		Else
			prob = (ed_hit - ed_avd) * ed_aradap * ed_size
		End If
		
		'�s�ӑł�
		If IsFeatureAvailable("�X�e���X") And Not IsConditionSatisfied("�X�e���X����") And Not t.IsFeatureAvailable("�X�e���X������") Then
			prob = prob + 20
		End If
		
		wclass = WeaponClass(w)
		
		Dim uad As Short
		With t
			'�U��������͎w�肵�����x���ȏ㗣���قǖ������A�b�v
			If InStrNotNest(wclass, "�U") > 0 Then
				Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
					Case 1
						'�C���Ȃ�
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
			
			If InStrNotNest(wclass, "�T") = 0 And InStrNotNest(wclass, "�L") = 0 And InStrNotNest(wclass, "�U") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 Then
				'�����C��
				If IsOptionDefined("�����C��") Then
					If InStrNotNest(wclass, "�g") = 0 And InStrNotNest(wclass, "�l") = 0 Then
						If IsOptionDefined("��^�}�b�v") Then
							Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
								Case 1 To 4
									'�C���Ȃ�
								Case 5, 6
									prob = 0.9 * prob
								Case 7, 8
									prob = 0.8 * prob
								Case 9, 10
									prob = 0.7 * prob
								Case Else
									prob = 0.6 * prob
							End Select
						ElseIf IsOptionDefined("���^�}�b�v") Then 
							Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
								Case 1
									'�C���Ȃ�
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
									'�C���Ȃ�
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
				
				'�d�b�l
				For i = MaxLng(.x - 2, 1) To MinLng(.x + 2, MapWidth)
					For j = MaxLng(.y - 2, 1) To MinLng(.y + 2, MapHeight)
						If System.Math.Abs(.x - i) + System.Math.Abs(.y - j) <= 3 Then
							u = MapDataForUnit(i, j)
							If Not u Is Nothing Then
								With u
									If .IsAlly(t) Then
										ecm_lv = MaxDbl(ecm_lv, .FeatureLevel("�d�b�l"))
									ElseIf .IsAlly(Me) Then 
										eccm_lv = MaxDbl(eccm_lv, .FeatureLevel("�d�b�l"))
									End If
								End With
							End If
						End If
					Next 
				Next 
				'�z�[�~���O�U���͂d�b�l�̉e���������󂯂�
				If InStrNotNest(wclass, "�g") > 0 Then
					prob = prob * (100 - 10 * MaxDbl(ecm_lv - eccm_lv, 0)) \ 100
				Else
					prob = prob * (100 - 5 * MaxDbl(ecm_lv - eccm_lv, 0)) \ 100
				End If
			End If
			
			'�X�e���X�ɂ��␳
			If .IsFeatureAvailable("�X�e���X") And Not IsFeatureAvailable("�X�e���X������") Then
				If .IsFeatureLevelSpecified("�X�e���X") Then
					If System.Math.Abs(x - .x) + System.Math.Abs(y - .y) > .FeatureLevel("�X�e���X") Then
						prob = prob * 0.8
					End If
				Else
					If System.Math.Abs(x - .x) + System.Math.Abs(y - .y) > 3 Then
						prob = prob * 0.8
					End If
				End If
			End If
			
			'�n�ォ��󒆂̓G�ɍU������
			If (.Area = "��" Or .Area = "�F��") And Area <> "��" And Area <> "�F��" Then
				If InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Then
					'�W�����v�U��
					If Not IsOptionDefined("�n�`�K���������C��") Then
						If Not IsTransAvailable("��") Then
							uad = Adaption(1)
							If InStrNotNest(wclass, "�i") > 0 Then
								uad = MinLng(uad + WeaponLevel(w, "�i"), 4)
							End If
							uad = MinLng(uad, 4)
							prob = (uad + 6) * prob \ 10
						End If
					End If
				Else
					'�ʏ�U��
					If IsOptionDefined("���x�C��") Then
						If InStrNotNest(wclass, "��") = 0 Then
							prob = 0.7 * prob
						End If
					End If
				End If
			End If
			
			'�ǒn��\��
			If .IsFeatureAvailable("�n�`�K��") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "�n�`�K��" Then
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
			
			'�U�����
			If .IsFeatureAvailable("�U�����") Then
				prob_mod = 0
				For i = 1 To .CountFeature
					If .Feature(i) = "�U�����" Then
						fdata = .FeatureData(i)
						flevel = .FeatureLevel(i)
						
						'�K�v����
						If IsNumeric(LIndex(fdata, 3)) Then
							nmorale = CShort(LIndex(fdata, 3))
						Else
							nmorale = 0
						End If
						
						'�����\�H
						If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) Then
							'�U����𔭓�
							prob_mod = prob_mod + flevel
						End If
					End If
				Next 
				prob = prob * (10 - prob_mod) \ 10
			End If
			
			'�����Ȃ���ΐ�΂ɖ���
			If .IsConditionSatisfied("�s���s�\") Or .IsConditionSatisfied("���") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("�Ή�") Or .IsConditionSatisfied("����") Or .IsUnderSpecialPowerEffect("�s���s�\") Then
				HitProbability = 1000
				Exit Function
			End If
			
			'�X�e�[�^�X�ُ�ɂ��C��
			If InStrNotNest(wclass, "�g") = 0 And InStrNotNest(wclass, "��") = 0 Then
				If IsConditionSatisfied("�h��") Then
					prob = prob \ 2
				End If
				If IsConditionSatisfied("���|") Then
					prob = prob \ 2
				End If
				If IsConditionSatisfied("�Ӗ�") Then
					prob = prob \ 2
				End If
			End If
			
			'�^�[�Q�b�g�̃X�e�[�^�X�ُ�ɂ��C��
			If .IsConditionSatisfied("�Ӗ�") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("�`���[�W") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("����") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("����m") Then
				prob = 1.5 * prob
			End If
			If .IsConditionSatisfied("�ړ��s�\") Then
				prob = 1.5 * prob
			End If
			
			'���
			If HP <= MaxHP \ 4 Then
				With MainPilot
					If .IsSkillAvailable("�����") Then
						prob = prob + 50
					ElseIf .IsSkillAvailable("���") Then 
						prob = prob + 30
					End If
				End With
			End If
			If .HP <= .MaxHP \ 4 Then
				With .MainPilot
					If .IsSkillAvailable("�����") Then
						prob = prob - 50
					ElseIf .IsSkillAvailable("���") Then 
						prob = prob - 30
					End If
				End With
			End If
			
			'�X�y�V�����p���[�y�ѓ����Ԃɂ��␳
			If is_true_value Or mpskill >= 160 Then
				If IsUnderSpecialPowerEffect("��������") Then
					prob = prob + 10 * SpecialPowerEffectLevel("��������")
				ElseIf IsConditionSatisfied("�^�����t�o") Then 
					prob = prob + 15
				End If
				If .IsUnderSpecialPowerEffect("�������") Then
					prob = prob - 10 * .SpecialPowerEffectLevel("�������")
				ElseIf .IsConditionSatisfied("�^�����t�o") Then 
					prob = prob - 15
				End If
				
				If IsConditionSatisfied("�^�����c�n�v�m") Then
					prob = prob - 15
				End If
				If .IsConditionSatisfied("�^�����c�n�v�m") Then
					prob = prob + 15
				End If
				
				If IsUnderSpecialPowerEffect("�����ቺ") Then
					prob = prob - 10 * SpecialPowerEffectLevel("�����ቺ")
				End If
				If .IsUnderSpecialPowerEffect("���ቺ") Then
					prob = prob + 10 * .SpecialPowerEffectLevel("���ቺ")
				End If
				
				If IsUnderSpecialPowerEffect("�������ቺ") Then
					prob = prob * (10 - SpecialPowerEffectLevel("�������ቺ")) \ 10
				End If
			End If
		End With
		
		' �ŏI���������`����B���ꂪ�Ȃ��Ƃ��͉������Ȃ�
		If BCList.IsDefined("�ŏI������") Then
			'���O�Ƀf�[�^��o�^
			BCVariable.DataReset()
			BCVariable.MeUnit = Me
			BCVariable.AtkUnit = Me
			BCVariable.DefUnit = t
			BCVariable.WeaponNumber = w
			BCVariable.LastVariable = prob
			prob = BCList.Item("�ŏI������").Calculate()
		End If
		
		If prob < 0 Then
			HitProbability = 0
		Else
			HitProbability = prob
		End If
	End Function
	
	'���� w �̃��j�b�g t �ɑ΂���_���[�W
	'�G���j�b�g�̓X�y�V�����p���[���ɂ��␳���l�����Ȃ��̂�
	' is_true_value �ɂ���ĕ␳���Ȃ����ǂ������w��ł���悤�ɂ��Ă���
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
		'���b�A���b�␳�ꎞ�ۑ�
		Dim ed_amr As Double
		Dim ed_amr_fix As Double
		
		wclass = WeaponClass(w)
		
		'�p�C���b�g�̋Z�ʂɂ���ă_���[�W�𐳊m�ɗ\���ł��邩���E�����
		mpskill = MainPilot.TacticalTechnique
		
		With t
			'����U����
			Damage = WeaponPower(w, .Area)
			'�U���͂�0�̏ꍇ�͏�Ƀ_���[�W0
			If Damage = 0 Then
				Exit Function
			End If
			
			'��{���b�l
			arm = .Armor
			
			'�A�[�}�[�\��
			If Not .IsFeatureAvailable("�A�[�}�[") Then
				GoTo SkipArmor
			End If
			'�U�R�̓A�[�}�[���l�����Ȃ�
			If Not is_true_value And mpskill < 150 Then
				GoTo SkipArmor
			End If
			arm_mod = 0
			For i = 1 To .CountFeature
				If .Feature(i) = "�A�[�}�[" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'�K�v����
					If IsNumeric(LIndex(fdata, 3)) Then
						nmorale = CShort(LIndex(fdata, 3))
					Else
						nmorale = 0
					End If
					
					'�I�v�V����
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
							Case "�\�͕K�v"
								'�X�L�b�v
							Case "������"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * (.MainPilot.SynchroRate - 30)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "���"
								If lv_mod = -1 Then
									lv_mod = 2
								End If
								slevel = lv_mod * .MainPilot.Plana
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�I�[��"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "���\��"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�����o"
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * (.MainPilot.SkillLevel("�����o") + .MainPilot.SkillLevel("�m�o����"))
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 50
								End If
								slevel = lv_mod * .MainPilot.SkillLevel(opt)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'�����\�H
					If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'�A�[�}�[����
						arm_mod = arm_mod + 100 * flevel + slevel
					End If
				End If
			Next 
			
			'���b���򉻂��Ă���ꍇ�̓A�[�}�[�ɂ�鑕�b�ǉ�������
			If .IsConditionSatisfied("���b��") Then
				arm_mod = arm_mod \ 2
			End If
			
			arm = arm + arm_mod
			
SkipArmor: 
			
			'�n�`�K���ɂ�鑕�b�C��
			If Not IsOptionDefined("�n�`�K���������C��") Then
				Select Case .Area
					Case "��"
						uadaption = .AdaptionMod(1)
					Case "�n��"
						If TerrainClass(.x, .y) = "����" Then
							uadaption = .AdaptionMod(4)
						Else
							uadaption = .AdaptionMod(2)
						End If
					Case "����"
						uadaption = .AdaptionMod(2)
					Case "����"
						uadaption = .AdaptionMod(3)
					Case "�F��"
						uadaption = .AdaptionMod(4)
					Case "�n��"
						Damage = 0
						Exit Function
				End Select
				If uadaption = 0 Then
					uadaption = 0.6
				End If
			Else
				If .Area = "�n��" Then
					Damage = 0
					Exit Function
				Else
					uadaption = 1
				End If
			End If
			
			'�s���ɂ�鑕�b�C��
			If .MainPilot.IsSkillAvailable("�s��") Then
				If IsOptionDefined("�h��͔{���ቺ") Then
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
			
			'�X�y�V�����p���[�ɂ�閳�h�����
			If .IsUnderSpecialPowerEffect("���h��") Then
				arm = 0
			End If
			
			If is_true_value Or mpskill >= 160 Then
				'�X�y�V�����p���[�ɂ��C��
				If .IsUnderSpecialPowerEffect("���b����") Then
					arm = arm * (1# + 0.1 * .SpecialPowerEffectLevel("���b����"))
					'���b����
				ElseIf .IsConditionSatisfied("�h��͂t�o") Then 
					If IsOptionDefined("�h��͔{���ቺ") Then
						arm = 1.25 * arm
					Else
						arm = 1.5 * arm
					End If
				End If
				If .IsUnderSpecialPowerEffect("���b�ቺ") Then
					arm = arm * (1# + 0.1 * .SpecialPowerEffectLevel("���b�ቺ"))
				ElseIf .IsConditionSatisfied("�h��͂c�n�v�m") Then 
					arm = 0.75 * arm
				End If
			End If
			
			'�ђʌ^�U��
			If IsUnderSpecialPowerEffect("�ђʍU��") Then
				arm = arm \ 2
			ElseIf IsWeaponClassifiedAs(w, "��") Then 
				If IsWeaponLevelSpecified(w, "��") Then
					arm = arm * (10 - WeaponLevel(w, "��")) \ 10
				Else
					arm = arm \ 2
				End If
			End If
			If is_true_value Or mpskill >= 140 Then
				'��_
				If .Weakness(wclass) Then
					arm = arm \ 2
					'�z������ꍇ�͑��b�𖳎����Ĕ���
				ElseIf Not .Effective(wclass) And .Absorb(wclass) Then 
					arm = 0
				End If
			End If
			
			If BCList.IsDefined("�h��␳") Then
				'�o�g���R���t�B�O�f�[�^�ɂ��v�Z���s
				BCVariable.DataReset()
				BCVariable.MeUnit = t
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.Armor = arm
				BCVariable.TerrainAdaption = uadaption
				arm = BCList.Item("�h��␳").Calculate()
			Else
				With .MainPilot
					'�C�͂ɂ�鑕�b�C��
					If IsOptionDefined("�C�͌��ʏ�") Then
						arm = arm * (50 + (.Morale + .MoraleMod) \ 2) \ 100
					Else
						arm = arm * (.Morale + .MoraleMod) \ 100
					End If
					
					'���x���A�b�v�ɂ�鑕�b�C���{�ϋv�\��
					arm = arm * .Defense \ 100
				End With
				
				' �n�`�K���ɂ�鑕�b�C��
				arm = arm * uadaption
			End If
			
			'�_���[�W�Œ蕐��̏ꍇ�͑��b�ƒn�`�������C���𖳎�
			If InStrNotNest(wclass, "��") > 0 Then
				GoTo SkipDamageMod
			End If
			
			If BCList.IsDefined("�_���[�W") Then
				'�o�g���R���t�B�O�f�[�^�ɂ��v�Z���s
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.AttackVariable = Damage
				BCVariable.DffenceVariable = arm
				If TerrainClass(.x, .y) = "����" Then
					If .Area = "�n��" Then
						BCVariable.TerrainAdaption = (100 - TerrainEffectForDamage(.x, .y)) / 100
					Else
						BCVariable.TerrainAdaption = 1
					End If
				ElseIf .Area <> "��" Then 
					BCVariable.TerrainAdaption = (100 - TerrainEffectForDamage(.x, .y)) / 100
				Else
					BCVariable.TerrainAdaption = 1
				End If
				Damage = BCList.Item("�_���[�W").Calculate()
			Else
				'���b�l�ɂ���ă_���[�W���y��
				Damage = Damage - arm
				
				'�n�`�␳
				If TerrainClass(.x, .y) = "����" Then
					If .Area = "�n��" Then
						Damage = Damage * ((100 - TerrainEffectForDamage(.x, .y)) / 100)
					End If
				ElseIf .Area <> "��" Then 
					Damage = Damage * ((100 - TerrainEffectForDamage(.x, .y)) / 100)
				End If
			End If
			
			'�U��������͗����قǃ_���[�W�_�E��
			If InStrNotNest(wclass, "�U") > 0 Then
				Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
					Case 1
						'�C���Ȃ�
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
			
			'�����C��
			If IsOptionDefined("�����C��") Then
				If InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 Then
					If IsOptionDefined("��^�}�b�v") Then
						Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
							Case 1 To 4
								'�C���Ȃ�
							Case 5, 6
								Damage = 0.95 * Damage
							Case 7, 8
								Damage = 0.9 * Damage
							Case 9, 10
								Damage = 0.85 * Damage
							Case Else
								Damage = 0.8 * Damage
						End Select
					ElseIf IsOptionDefined("���^�}�b�v") Then 
						Select Case System.Math.Abs(x - .x) + System.Math.Abs(y - .y)
							Case 1
								'�C���Ȃ�
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
								'�C���Ȃ�
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
			
			'����U���͎�_�������͗L���������j�b�g�ȊO�ɂ͌����Ȃ�
			If InStrNotNest(wclass, "��") > 0 Then
				buf = .strWeakness & .strEffective
				For i = 1 To Len(buf)
					'�������ЂƂ܂Ƃ߂��擾
					ch = GetClassBundle(buf, i)
					If ch <> "��" And ch <> "��" Then
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
			
			'����U���͎w�葮���ɑ΂��Ď�_�������͗L���������j�b�g�ȊO�ɂ͌����Ȃ�
			idx = InStrNotNest(wclass, "��")
			If idx > 0 Then
				buf = .strWeakness & .strEffective
				For i = 1 To Len(buf)
					'�������ЂƂ܂Ƃ߂��擾
					ch = GetClassBundle(buf, i)
					If ch <> "��" And ch <> "��" Then
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
			
			'���背�x������U��
			If WeaponLevel(w, "��") > 0 Then
				'UPGRADE_WARNING: Mod �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If t.MainPilot.Level Mod WeaponLevel(w, "��") <> 0 Then
					Damage = 0
					Exit Function
				End If
			End If
			
			If is_true_value Or mpskill >= 140 Then
				'��_�A�L���A�z����D��
				If Not .Weakness(wclass) And Not .Effective(wclass) And Not .Absorb(wclass) Then
					'������
					If .Immune(wclass) Then
						Damage = 0
						Exit Function
						'�ϐ�
					ElseIf .Resist(wclass) Then 
						Damage = Damage \ 2
					End If
				End If
			End If
			
			'�Ӗڏ�Ԃɂ͎��o�U���͌����Ȃ�
			If is_true_value Or mpskill >= 140 Then
				If InStrNotNest(wclass, "��") > 0 Then
					If .IsConditionSatisfied("�Ӗ�") Then
						Damage = 0
						Exit Function
					End If
				End If
			End If
			
			'�@�B�ɂ͐��_�U���͌����Ȃ�
			If is_true_value Or mpskill >= 140 Then
				If InStrNotNest(wclass, "��") > 0 Then
					If .MainPilot.Personality = "�@�B" Then
						Damage = 0
						Exit Function
					End If
				End If
			End If
			
			'���ʌ��蕐��
			If InStrNotNest(wclass, "��") > 0 Then
				If .MainPilot.Sex <> "�j��" Then
					Damage = 0
					Exit Function
				End If
			End If
			If InStrNotNest(wclass, "��") > 0 Then
				If .MainPilot.Sex <> "����" Then
					Damage = 0
					Exit Function
				End If
			End If
			
			'�Q���݂��P���ƃ_���[�W1.5�{
			If .IsConditionSatisfied("����") Then
				Damage = 1.5 * Damage
			End If
			
			With MainPilot
				'���C�͎��̃_���[�W�����\��
				If .Morale >= 130 Then
					If IsOptionDefined("�_���[�W�{���ቺ") Then
						If .IsSkillAvailable("���ݗ͊J��") Then
							Damage = 1.2 * Damage
						End If
						If IsFeatureAvailable("�u�[�X�g") Then
							Damage = 1.2 * Damage
						End If
					Else
						If .IsSkillAvailable("���ݗ͊J��") Then
							Damage = 1.25 * Damage
						End If
						If IsFeatureAvailable("�u�[�X�g") Then
							Damage = 1.25 * Damage
						End If
					End If
				End If
				
				'���ӋZ
				If .IsSkillAvailable("���ӋZ") Then
					sdata = .SkillData("���ӋZ")
					For i = 1 To Len(sdata)
						If InStrNotNest(wclass, Mid(sdata, i, 1)) > 0 Then
							Damage = 1.2 * Damage
							Exit For
						End If
					Next 
				End If
				
				'�s����
				If .IsSkillAvailable("�s����") Then
					sdata = .SkillData("�s����")
					For i = 1 To Len(sdata)
						If InStrNotNest(wclass, Mid(sdata, i, 1)) > 0 Then
							Damage = 0.8 * Damage
							Exit For
						End If
					Next 
				End If
			End With
			
			'�n���^�[�\��
			'(�^�[�Q�b�g��MainPilot���Q�Ƃ��邽�߁A�uWith .MainPilot�v�͎g���Ȃ�)
			If MainPilot.IsSkillAvailable("�n���^�[") Then
				For i = 1 To MainPilot.CountSkill
					If MainPilot.Skill(i) = "�n���^�[" Then
						sdata = MainPilot.SkillData(i)
						For j = 2 To LLength(sdata)
							tname = LIndex(sdata, j)
							If .Name = tname Or .Class0 = tname Or .Size & "�T�C�Y" = tname Or .MainPilot.Name = tname Or .MainPilot.Sex = tname Then
								Exit For
							End If
						Next 
						If j <= LLength(sdata) Then
							Damage = (10 + MainPilot.SkillLevel(i)) * Damage \ 10
							Exit For
						End If
					End If
				Next 
				If IsConditionSatisfied("�n���^�[�t��") Or IsConditionSatisfied("�n���^�[�t���Q") Then
					sdata = MainPilot.SkillData("�n���^�[")
					For i = 2 To LLength(sdata)
						tname = LIndex(sdata, i)
						If .Name = tname Or .Class0 = tname Or .Size & "�T�C�Y" = tname Or .MainPilot.Name = tname Or .MainPilot.Sex = tname Then
							Exit For
						End If
					Next 
					If i <= LLength(sdata) Then
						Damage = (10 + MainPilot.SkillLevel("�n���^�[")) * Damage \ 10
					End If
				End If
			End If
			
			'�X�y�V�����p���[�A�����Ԃɂ��_���[�W����
			dmg_mod = 1
			If IsConditionSatisfied("�U���͂t�o") Or IsConditionSatisfied("����m") Then
				If IsOptionDefined("�_���[�W�{���ቺ") Then
					dmg_mod = 1.2
				Else
					dmg_mod = 1.25
				End If
			End If
			'�T�|�[�g�A�^�b�N�̏ꍇ�̓X�y�V�����p���[�ɂ��C��������
			If Not is_support_attack Then
				If is_true_value Or mpskill >= 160 Then
					'�X�y�V�����p���[�ɂ��_���[�W�����͓����Ԃɂ�鑝���Əd�����Ȃ�
					dmg_mod = MaxDbl(dmg_mod, 1 + 0.1 * SpecialPowerEffectLevel("�_���[�W����"))
					dmg_mod = dmg_mod + 0.1 * .SpecialPowerEffectLevel("��_���[�W����")
				End If
			End If
			Damage = dmg_mod * Damage
			
			'�X�y�V�����p���[�A�����ԁA�T�|�[�g�A�^�b�N�ɂ��_���[�W�ቺ
			If is_true_value Or mpskill >= 160 Then
				dmg_mod = 1
				dmg_mod = dmg_mod - 0.1 * SpecialPowerEffectLevel("�_���[�W�ቺ")
				dmg_mod = dmg_mod - 0.1 * .SpecialPowerEffectLevel("��_���[�W�ቺ")
				Damage = dmg_mod * Damage
			End If
			If IsConditionSatisfied("�U���͂c�n�v�m") Then
				Damage = 0.75 * Damage
			End If
			If IsConditionSatisfied("���|") Then
				Damage = 0.8 * Damage
			End If
			If is_support_attack Then
				'�T�|�[�g�A�^�b�N�_���[�W�ቺ
				If IsOptionDefined("�T�|�[�g�A�^�b�N�_���[�W�ቺ") Then
					Damage = 0.7 * Damage
				End If
			End If
			
			'���W�X�g�\��
			dmg_mod = 0
			If Not .IsFeatureAvailable("���W�X�g") Then
				GoTo SkipResist
			End If
			'�U�R�̓��W�X�g���l�����Ȃ�
			If Not is_true_value And mpskill < 150 Then
				GoTo SkipResist
			End If
			For i = 1 To .CountFeature
				If .Feature(i) = "���W�X�g" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'�K�v����
					If IsNumeric(LIndex(fdata, 3)) Then
						nmorale = CShort(LIndex(fdata, 3))
					Else
						nmorale = 0
					End If
					
					'�I�v�V����
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
							Case "�\�͕K�v"
								'�X�L�b�v
							Case "������"
								If lv_mod = -1 Then
									lv_mod = 0.5
								End If
								slevel = lv_mod * (.MainPilot.SynchroRate - 30)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "���"
								If lv_mod = -1 Then
									lv_mod = 0.2
								End If
								slevel = lv_mod * .MainPilot.Plana
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�I�[��"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "���\��"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�����o"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * (.MainPilot.SkillLevel("�����o") + .MainPilot.SkillLevel("�m�o����"))
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .MainPilot.SkillLevel(opt)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'�����\�H
					If .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'���W�X�g����
						dmg_mod = dmg_mod + 10 * flevel + slevel
					End If
				End If
			Next 
			
			Damage = Damage * (100 - dmg_mod) \ 100
			
SkipResist: 
			
			If BCList.IsDefined("�ŏI�_���[�W") Then
				'�o�g���R���t�B�O�f�[�^�ɂ��v�Z���s
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = Damage
				Damage = BCList.Item("�ŏI�_���[�W").Calculate()
			End If
			
			'�Œ�_���[�W��10
			If dmg_mod < 100 Then
				If Damage < 10 Then
					' MOD START MARGE
					'                Damage = 10
					If IsOptionDefined("�_���[�W��������") Then
						Damage = MaxLng(Damage, 0)
					ElseIf IsOptionDefined("�_���[�W�����P") Then 
						Damage = MaxLng(Damage, 1)
					Else
						Damage = 10
					End If
					' MOD END MARGE
				End If
			End If
			
			'�_���[�W���z������ꍇ�͍Ō�ɔ��]
			If is_true_value Or mpskill >= 140 Then
				'��_�A�L����D��
				If Not .Weakness(wclass) And Not .Effective(wclass) Then
					'�z��
					If Damage > 0 And .Absorb(wclass) Then
						Damage = -Damage \ 2
					End If
				End If
			End If
		End With
	End Function
	
	'�N���e�B�J���̔�����
	Public Function CriticalProbability(ByVal w As Short, ByRef t As Unit, Optional ByVal def_mode As String = "") As Short
		Dim i, prob, idx As Short
		Dim wclass As String
		Dim buf, c As String
		Dim is_special As Boolean
		'�N���e�B�J���U���A�h��̈ꎞ�ۑ��ϐ�
		Dim ed_crtatk, ed_crtdfe As Short
		
		With t
			If IsNormalWeapon(w) Then
				'�ʏ�U��
				
				'�X�y�V�����p���[�Ƃ̌��ʂ̏d�ˍ��킹���֎~����Ă���ꍇ
				If IsOptionDefined("�X�y�V�����p���[�g�p���N���e�B�J������") Or IsOptionDefined("���_�R�}���h�g�p���N���e�B�J������") Then
					If IsUnderSpecialPowerEffect("�_���[�W����") Then
						Exit Function
					End If
				End If
				
				'�U�����ɂ��␳
				If BCList.IsDefined("�N���e�B�J���U���␳") Then
					'�o�g���R���t�B�O�f�[�^�̐ݒ�ɂ��C��
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackExp = WeaponCritical(w)
					ed_crtatk = BCList.Item("�N���e�B�J���U���␳").Calculate()
				Else
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					ed_crtatk = WeaponCritical(w) + MainPilot.Technique
				End If
				
				'�h�䑤�ɂ��␳
				If BCList.IsDefined("�N���e�B�J���h��␳") Then
					'�o�g���R���t�B�O�f�[�^�̐ݒ�ɂ��C��
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = t
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					ed_crtdfe = BCList.Item("�N���e�B�J���h��␳").Calculate()
				Else
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					ed_crtdfe = .MainPilot.Technique
				End If
				
				' �N���e�B�J���������v�Z
				If BCList.IsDefined("�N���e�B�J��������") Then
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackVariable = ed_crtatk
					BCVariable.DffenceVariable = ed_crtdfe
					prob = BCList.Item("�N���e�B�J��������").Calculate()
				Else
					prob = ed_crtatk - ed_crtdfe
				End If
				
				'�������ɂ��C��
				prob = prob + 2 * MainPilot.SkillLevel("������") - 2 * .MainPilot.SkillLevel("������")
				
				'���\�͂ɂ��C��
				If MainPilot.IsSkillAvailable("���\��") Then
					prob = prob + 5
				End If
				
				'��́A����́A�o��ɂ��C��
				If HP <= MaxHP \ 4 Then
					If MainPilot.IsSkillAvailable("���") Or MainPilot.IsSkillAvailable("�����") Or MainPilot.IsSkillAvailable("�o��") Then
						prob = prob + 50
					End If
				End If
				
				'�X�y�V�����p���[�ɂ�C��
				prob = prob + 10 * SpecialPowerEffectLevel("�N���e�B�J��������")
			Else
				'������ʂ𔺂��U��
				is_special = True
				
				'�U�����ɂ��␳
				If BCList.IsDefined("������ʍU���␳") Then
					'�o�g���R���t�B�O�f�[�^�̐ݒ�ɂ��C��
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackExp = WeaponCritical(w)
					ed_crtatk = BCList.Item("������ʍU���␳").Calculate()
				Else
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					ed_crtatk = WeaponCritical(w) + MainPilot.Technique \ 2
				End If
				
				'�h�䑤�ɂ��␳
				If BCList.IsDefined("������ʖh��␳") Then
					'�o�g���R���t�B�O�f�[�^�̐ݒ�ɂ��C��
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = t
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					'������ʂ̏ꍇ�͑��肪�U�R�̎��Ɋm��������
					If InStr(.MainPilot.Name, "(�U�R)") > 0 Then
						BCVariable.CommonEnemy = 30
					End If
					ed_crtdfe = BCList.Item("������ʖh��␳").Calculate()
				Else
					' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
					ed_crtdfe = .MainPilot.Technique \ 2
					
					'������ʂ̏ꍇ�͑��肪�U�R�̎��Ɋm��������
					If InStr(.MainPilot.Name, "(�U�R)") > 0 Then
						' �ꎞ�ۑ��ϐ��Ɉꎞ�ۑ�
						ed_crtdfe = ed_crtdfe - 30
					End If
				End If
				
				' ������ʔ������v�Z
				If BCList.IsDefined("������ʔ�����") Then
					'���O�Ƀf�[�^��o�^
					BCVariable.DataReset()
					BCVariable.MeUnit = Me
					BCVariable.AtkUnit = Me
					BCVariable.DefUnit = t
					BCVariable.WeaponNumber = w
					BCVariable.AttackVariable = ed_crtatk
					BCVariable.DffenceVariable = ed_crtdfe
					prob = BCList.Item("������ʔ�����").Calculate()
				Else
					prob = ed_crtatk - ed_crtdfe
				End If
				
				'��R�͂ɂ��C��
				prob = prob - 10 * .FeatureLevel("��R��")
			End If
			
			'�s�ӑł�
			If IsFeatureAvailable("�X�e���X") And Not IsConditionSatisfied("�X�e���X����") And Not .IsFeatureAvailable("�X�e���X������") And IsWeaponClassifiedAs(w, "�E") Then
				prob = prob + 10
			End If
			
			'���肪�����Ȃ���Ίm���A�b�v
			If .IsConditionSatisfied("�s���s�\") Or .IsConditionSatisfied("�Ή�") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("���") Or .IsConditionSatisfied("����") Or .IsUnderSpecialPowerEffect("�s���s�\") Then
				prob = prob + 10
			End If
			
			'�ȉ��̏C���͓�����ʔ����m���ɂ̂݉e��
			If is_special Then
				wclass = WeaponClass(w)
				
				'����U���͎�_�A�L���������j�b�g�ȊO�ɂ͌����Ȃ�
				If InStrNotNest(wclass, "��") > 0 Then
					buf = .strWeakness & .strEffective
					For i = 1 To Len(buf)
						'�������ЂƂ܂Ƃ߂��擾
						c = GetClassBundle(buf, i)
						If c <> "��" And c <> "��" Then
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
				
				'����U���͎�_�A�L���������j�b�g�ȊO�ɂ͌����Ȃ�
				idx = InStrNotNest(wclass, "��")
				If idx > 0 Then
					buf = .strWeakness & .strEffective
					For i = 1 To Len(buf)
						'�������ЂƂ܂Ƃ߂��擾
						c = GetClassBundle(buf, i)
						If c <> "��" And c <> "��" Then
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
				
				'���背�x������U��
				If InStrNotNest(wclass, "��") > 0 Then
					'UPGRADE_WARNING: Mod �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
					If t.MainPilot.Level Mod WeaponLevel(w, "��") <> 0 Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'�N���e�B�J�����ɂ��ẮA
				'��A�������̎w�葮���ɑ΂��Ă̖h��������l������B
				buf = ""
				i = InStrNotNest(wclass, "��")
				Do While i > 0
					buf = buf & Mid(GetClassBundle(wclass, i), 2)
					i = InStrNotNest(wclass, "��", i + 1)
				Loop 
				i = InStrNotNest(wclass, "��")
				Do While i > 0
					buf = buf & Mid(GetClassBundle(wclass, i), 2)
					i = InStrNotNest(wclass, "��", i + 1)
				Loop 
				buf = buf & wclass
				
				'��_
				If .Weakness(buf) Then
					prob = prob + 10
					'�L��
				ElseIf .Effective(buf) Then 
					'�ω��Ȃ�
					'����Z
				ElseIf InStrNotNest(wclass, "��") > 0 Then 
					CriticalProbability = 0
					Exit Function
					'����Z
				ElseIf InStrNotNest(wclass, "��") > 0 Then 
					CriticalProbability = 0
					Exit Function
					'�z��
				ElseIf .Absorb(buf) Then 
					CriticalProbability = 0
					Exit Function
					'������
				ElseIf .Immune(buf) Then 
					CriticalProbability = 0
					Exit Function
					'�ϐ�
				ElseIf .Resist(buf) Then 
					prob = prob \ 2
				End If
				
				'�Ӗڏ�Ԃɂ͎��o�U���͌����Ȃ�
				If InStrNotNest(wclass, "��") > 0 Then
					If .IsConditionSatisfied("�Ӗ�") Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'�@�B�ɂ͐��_�U���͌����Ȃ�
				If InStrNotNest(wclass, "��") > 0 Then
					If .MainPilot.Personality = "�@�B" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				
				'���ʌ��蕐��
				If InStrNotNest(wclass, "��") > 0 Then
					If .MainPilot.Sex <> "�j��" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
				If InStrNotNest(wclass, "��") > 0 Then
					If .MainPilot.Sex <> "����" Then
						CriticalProbability = 0
						Exit Function
					End If
				End If
			End If
		End With
		
		'�h�䎞�̓N���e�B�J�������m��������
		If def_mode = "�h��" Then
			prob = prob \ 2
		End If
		
		' �ŏI�N���e�B�J��/������ʂ��`����B���ꂪ�Ȃ��Ƃ��͉������Ȃ�
		If IsNormalWeapon(w) Then
			'�N���e�B�J��
			If BCList.IsDefined("�ŏI�N���e�B�J��������") Then
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = prob
				prob = BCList.Item("�ŏI�N���e�B�J��������").Calculate()
			End If
		Else
			'�������
			If BCList.IsDefined("�ŏI������ʔ�����") Then
				'���O�Ƀf�[�^��o�^
				BCVariable.DataReset()
				BCVariable.MeUnit = Me
				BCVariable.AtkUnit = Me
				BCVariable.DefUnit = t
				BCVariable.WeaponNumber = w
				BCVariable.LastVariable = prob
				prob = BCList.Item("�ŏI������ʔ�����").Calculate()
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
	
	'����w�Ń��j�b�gt�ɍU�������������̃_���[�W�̊��Ғl
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
		
		'�U���͂�0�ł���Ώ�Ƀ_���[�W0
		If WeaponPower(w, "") <= 0 Then
			Exit Function
		End If
		
		'�_���[�W
		dmg = Damage(w, t, is_true_value)
		
		'�_���[�W�ɏC����������ꍇ
		If dmg_mod > 0 Then
			If InStrNotNest(wclass, "�E") = 0 Then
				dmg = dmg * dmg_mod
			End If
		End If
		
		'���E�U���͈ꌂ�ő����|���Ȃ�������ʂ��Ȃ�
		If InStrNotNest(wclass, "�E") > 0 Then
			If t.HP > dmg Then
				Exit Function
			End If
		End If
		
		'�_���[�W���^�����Ȃ��ꍇ
		If dmg <= 0 Then
			'�n�`�K���╕�󕐊�A���蕐��A���ʌ��蕐��A�������A�z���������ł���Ί��Ғl��0
			If WeaponAdaption(w, (t.Area)) = 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or t.Immune(wclass) Or t.Absorb(wclass) Then
				Exit Function
			End If
			
			'����ȊO�̗v���ł���΃_�~�[�Ń_���[�Ww�Ƃ���B
			'�������Ă����Ȃ��ƓG���U�������ʂ̏ꍇ�͂܂�������������
			'�U�����Ȃ��Ȃ��Ă��܂��̂ŁB
			'�P����ExpDamage=1�ȂǂƂ��Ȃ��͍̂U���͂̍��������D�悳���Ďg�킹�邽��
			ExpDamage = w
			Exit Function
		End If
		
		'�o���A������
		If InStrNotNest(wclass, "��") > 0 Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			'���E�U���͈ꌂ�ő����|���Ȃ�������ʂ��Ȃ�
			If InStrNotNest(wclass, "�E") > 0 Then
				If t.HP > dmg Then
					Exit Function
				End If
			End If
			ExpDamage = dmg
			Exit Function
		End If
		
		'�Z�ʂ̒Ⴂ�G�̓o���A���l�������U����������
		With MainPilot
			If Not is_true_value And .TacticalTechnique < 150 Then
				'���E�U���͈ꌂ�ő����|���Ȃ�������ʂ��Ȃ�
				If InStrNotNest(wclass, "�E") > 0 Then
					If t.HP > dmg Then
						Exit Function
					End If
				End If
				ExpDamage = dmg
				Exit Function
			End If
		End With
		
		With t
			'�o���A�\��
			For i = 1 To .CountFeature
				If .Feature(i) = "�o���A" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'�K�v����
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
					
					'�I�v�V����
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
							Case "���E"
								If IsSameCategory(fdata, FeatureData("�o���A")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "���a"
								If IsSameCategory(fdata, FeatureData("�o���A")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("�o���A")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "�ߐږ���"
								If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
									neautralize = True
								End If
							Case "�蓮"
								neautralize = True
							Case "�\�͕K�v"
								'�X�L�b�v
							Case "������"
								If lv_mod = -1 Then
									lv_mod = 20
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "���"
								If lv_mod = -1 Then
									lv_mod = 10
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�I�[��"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "���\��"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'�o���A�������Ŗ���������Ă���H
					If t.IsConditionSatisfied("�o���A������") Then
						If InStr(fdata, "�o���A����������") = 0 Then
							neautralize = True
						End If
					End If
					
					'�����\�H
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'�o���A����
						If dmg <= 1000 * flevel + slevel Then
							ExpDamage = w
							Exit Function
						End If
					End If
				End If
			Next 
			
			'�t�B�[���h�\��
			For i = 1 To .CountFeature
				If .Feature(i) = "�t�B�[���h" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'�K�v����
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
					
					'�I�v�V����
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
							Case "���E"
								If IsSameCategory(fdata, FeatureData("�t�B�[���h")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "���a"
								If IsSameCategory(fdata, FeatureData("�t�B�[���h")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("�t�B�[���h")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "�ߐږ���"
								If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
									neautralize = True
								End If
							Case "�蓮"
								neautralize = True
							Case "�\�͕K�v"
								'�X�L�b�v
							Case "������"
								If lv_mod = -1 Then
									lv_mod = 20
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "���"
								If lv_mod = -1 Then
									lv_mod = 10
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�I�[��"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "���\��"
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 200
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'�o���A�������Ŗ���������Ă���H
					If t.IsConditionSatisfied("�o���A������") Then
						If InStr(fdata, "�o���A����������") = 0 Then
							neautralize = True
						End If
					End If
					
					'�����\�H
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'�t�B�[���h����
						If dmg <= 500 * flevel + slevel Then
							ExpDamage = w
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							dmg = dmg - 500 * flevel - slevel
						End If
					End If
				End If
			Next 
			
			'�v���e�N�V�����\��
			For i = 1 To .CountFeature
				If .Feature(i) = "�v���e�N�V����" Then
					fname = .FeatureName0(i)
					fdata = .FeatureData(i)
					flevel = .FeatureLevel(i)
					
					'�K�v����
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
					
					'�I�v�V����
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
							Case "���E"
								If IsSameCategory(fdata, FeatureData("�v���e�N�V����")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									neautralize = True
								End If
							Case "���a"
								If IsSameCategory(fdata, FeatureData("�v���e�N�V����")) And System.Math.Abs(x - .x) + System.Math.Abs(y - .y) = 1 Then
									flevel = flevel - FeatureLevel("�v���e�N�V����")
									If flevel <= 0 Then
										neautralize = True
									End If
								End If
							Case "�ߐږ���"
								If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
									neautralize = True
								End If
							Case "�蓮"
								neautralize = True
							Case "�\�͕K�v"
								'�X�L�b�v
							Case "������"
								If lv_mod = -1 Then
									lv_mod = 0.5
								End If
								slevel = lv_mod * (.SyncLevel - 30)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = -30 * lv_mod Then
										neautralize = True
									End If
								Else
									If slevel = -30 * lv_mod Then
										slevel = 0
									End If
								End If
							Case "���"
								If lv_mod = -1 Then
									lv_mod = 0.2
								End If
								slevel = lv_mod * .PlanaLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "�I�[��"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .AuraLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case "���\��"
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .PsychicLevel
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
							Case Else
								If lv_mod = -1 Then
									lv_mod = 5
								End If
								slevel = lv_mod * .SkillLevel(opt)
								If InStr(fdata, "�\�͕K�v") > 0 Then
									If slevel = 0 Then
										neautralize = True
									End If
								End If
						End Select
					Next 
					
					'�o���A�������Ŗ���������Ă���H
					If t.IsConditionSatisfied("�o���A������") Then
						If InStr(fdata, "�o���A����������") = 0 Then
							neautralize = True
						End If
					End If
					
					'�����\�H
					If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 2), wclass) And Not neautralize Then
						'�v���e�N�V��������
						dmg = dmg * (100 - 10 * flevel - slevel) \ 100
						If dmg <= 0 Then
							ExpDamage = w
							Exit Function
						End If
					End If
				End If
			Next 
			
			'�΃r�[���p�h��\��
			If InStrNotNest(wclass, "�a") > 0 Then
				'�r�[���z��
				If .IsFeatureAvailable("�r�[���z��") Then
					ExpDamage = w
					Exit Function
				End If
			End If
			
			'���E�U���͈ꌂ�ő����|����ꍇ�ɂ̂ݗL��
			If InStrNotNest(wclass, "�E") > 0 Then
				If dmg < .HP Then
					dmg = 0
				End If
			End If
			
			'���h��
			If .IsFeatureAvailable("��") And .MainPilot.IsSkillAvailable("�r�h��") And .MaxAction > 0 And Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "�Z") And Not IsWeaponClassifiedAs(w, "�E") And (.IsConditionSatisfied("���t��") Or .FeatureLevel("��") > .ConditionLevel("���_���[�W")) Then
				If IsWeaponClassifiedAs(w, "�j") Then
					dmg = dmg - 50 * (.MainPilot.SkillLevel("�r�h��") + 4)
				Else
					dmg = dmg - 100 * (.MainPilot.SkillLevel("�r�h��") + 4)
				End If
			End If
		End With
		
		'�_���[�W�����������0�ȉ��ɂȂ����ꍇ���_�~�[��1�_���[�W
		If dmg <= 0 Then
			dmg = 1
		End If
		
		'���E�U���͈ꌂ�ő����|���Ȃ�������ʂ��Ȃ�
		If InStr(CStr(w), "�E") > 0 Then
			If t.HP > dmg Then
				Exit Function
			End If
		End If
		
		ExpDamage = dmg
	End Function
	
	
	' === �h�䑮�����菈�� ===
	
	'���� aname �ɑ΂��ċz�������������H
	Public Function Absorb(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strAbsorb, "�S") > 0 Then
			Absorb = True
			Exit Function
		End If
		
		'�������͕����U���ɕ��ނ����
		If Len(aname) = 0 Then
			If InStrNotNest(strAbsorb, "��") > 0 Then
				Absorb = True
			End If
			Exit Function
		End If
		
		'�����ɊY�����邩�𔻒�
		i = 1
		slen = Len(strAbsorb)
		Do While i <= slen
			'�������ЂƂ܂Ƃ߂��擾
			c = GetClassBundle(strAbsorb, i)
			Select Case c
				Case "��"
					If InStrNotNest(aname, "��") = 0 And InStrNotNest(aname, "��") = 0 Then
						Absorb = True
						Exit Do
					End If
				Case "��"
					'���@����ȊO�̖������Ȃ�������L��
					If InStrNotNest(aname, "��") > 0 Then
						If InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "���e") = 0 And InStrNotNest(aname, "����") = 0 Then
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
	
	'���� aname �ɑ΂��Ė����������������H
	Public Function Immune(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strImmune, "�S") > 0 Then
			Immune = True
			Exit Function
		End If
		
		'�������͕����U���ɕ��ނ����
		If Len(aname) = 0 Then
			If InStrNotNest(strImmune, "��") > 0 Then
				Immune = True
			End If
			Exit Function
		End If
		
		'�����ɊY�����邩�𔻒�
		i = 1
		slen = Len(strImmune)
		Do While i <= slen
			'�������ЂƂ܂Ƃ߂��擾
			c = GetClassBundle(strImmune, i)
			Select Case c
				Case "��"
					If InStrNotNest(aname, "��") = 0 And InStrNotNest(aname, "��") = 0 Then
						Immune = True
						Exit Do
					End If
				Case "��"
					'���@����ȊO�̖������Ȃ�������L��
					If InStrNotNest(aname, "��") > 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "���e") = 0 And InStrNotNest(aname, "����") = 0 Then
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
	
	'���� aname �ɑ΂��đϐ������������H
	Public Function Resist(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strResist, "�S") > 0 Then
			Resist = True
			Exit Function
		End If
		
		'�������͕����U���ɕ��ނ����
		If Len(aname) = 0 Then
			If InStrNotNest(strResist, "��") > 0 Then
				Resist = True
			End If
			Exit Function
		End If
		
		'�����ɊY�����邩�𔻒�
		i = 1
		slen = Len(strResist)
		Do While i <= slen
			'�������ЂƂ܂Ƃ߂��擾
			c = GetClassBundle(strResist, i)
			Select Case c
				Case "��"
					If InStrNotNest(aname, "��") = 0 And InStrNotNest(aname, "��") = 0 Then
						Resist = True
						Exit Do
					End If
				Case "��"
					'���@����ȊO�̖������Ȃ�������L��
					If InStrNotNest(aname, "��") > 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "����") = 0 And InStrNotNest(aname, "���e") = 0 And InStrNotNest(aname, "����") = 0 Then
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
	
	'���� aname �ɑ΂��Ď�_�����������H
	Public Function Weakness(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strWeakness, "�S") > 0 Then
			Weakness = True
			Exit Function
		End If
		
		If Len(aname) = 0 Then
			If InStrNotNest(strWeakness, "��") > 0 Then
				Weakness = True
			End If
			Exit Function
		End If
		
		i = 1
		slen = Len(strWeakness)
		Do While i <= slen
			'�������ЂƂ܂Ƃ߂��擾
			c = GetClassBundle(strWeakness, i)
			Select Case c
				Case "��"
					If InStrNotNest(aname, "��") = 0 And InStrNotNest(aname, "��") = 0 Then
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
	
	'���� aname �ɑ΂��ėL�������������H
	Public Function Effective(ByRef aname As String) As Boolean
		Dim c As String
		Dim i As Short
		Dim slen As Short
		
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strEffective, "�S") > 0 Then
			Effective = True
			Exit Function
		End If
		
		If Len(aname) = 0 Then
			If InStrNotNest(strEffective, "��") > 0 Then
				Effective = True
			End If
			Exit Function
		End If
		
		i = 1
		slen = Len(strEffective)
		Do While i <= slen
			'�������ЂƂ܂Ƃ߂��擾
			c = GetClassBundle(strEffective, i)
			Select Case c
				Case "��"
					If InStrNotNest(aname, "��") = 0 And InStrNotNest(aname, "��") = 0 Then
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
	
	'���� aname �ɑ΂��ē�����ʖ����������������H
	Public Function SpecialEffectImmune(ByRef aname As String) As Boolean
		'�S�����ɗL���ȏꍇ
		If InStrNotNest(strSpecialEffectImmune, "�S") > 0 Then
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
		
		'���������_�ƈႢ�A�N���e�B�J�����݂̂Ȃ̂�
		'�u�΁v�ɑ΂���h��������u��΁v�̃N���e�B�J�����ɉe������_�ɂ���
		'���ڊ֐����ɋL�q�ł���B
		If Left(aname, 1) = "��" Or Left(aname, 1) = "��" Then
			If InStrNotNest(strSpecialEffectImmune, aname) > 0 Then
				SpecialEffectImmune = True
				Exit Function
			End If
		End If
	End Function
	
	'�����̊Y������
	' aclass1 ���h�䑮���Aaclass2 �����푮��
	Public Function IsAttributeClassified(ByRef aclass1 As String, ByRef aclass2 As String) As Boolean
		Dim attr As String
		Dim alen, i As Short
		Dim with_not As Boolean
		
		If Len(aclass1) = 0 Then
			IsAttributeClassified = True
			Exit Function
		End If
		If aclass1 = "�S" Then
			IsAttributeClassified = True
			Exit Function
		End If
		
		'�������̍U���͕����U���ɕ��ނ����
		If Len(aclass2) = 0 Then
			If InStrNotNest(aclass1, "��") > 0 Then
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
				Case "��"
					If InStrNotNest(aclass2, "��") = 0 And InStrNotNest(aclass2, "��") = 0 Then
						IsAttributeClassified = True
						Exit Do
					End If
				Case "��"
					'���@����ȊO�̖������Ȃ�������L��
					If InStrNotNest(aclass2, "��") > 0 Then
						If InStrNotNest(aclass2, "����") = 0 And InStrNotNest(aclass2, "����") = 0 And InStrNotNest(aclass2, "����") = 0 And InStrNotNest(aclass2, "���e") = 0 And InStrNotNest(aclass2, "����") = 0 Then
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
	
	
	
	' === �U���֘A���� ===
	
	'���� w �Ń��j�b�g t �ɍU��
	'attack_mode �͍U���̎��
	'def_mode �̓��j�b�g t �̖h��Ԑ�
	'is_event �̓C�x���g(Attack�R�}���h)�ɂ��U�����ǂ���������
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
		
		'���b�Z�[�W�\���p�ɑI���󋵂�؂�ւ�
		SaveSelections()
		saved_selected_unit = SelectedUnit
		If attack_mode = "����" Then
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
		
		'�T�|�[�g�K�[�h���s�������j�b�g�Ɋւ�������N���A
		If Not IsDefense() Then
			'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			SupportGuardUnit = Nothing
			'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			SupportGuardUnit2 = Nothing
		End If
		
		'�p�C���b�g�̃Z���t��\�����邩�ǂ����𔻒�
		If attack_mode = "�}�b�v�U��" Or attack_mode = "����" Or attack_mode = "���Đg�Z" Or attack_mode = "��������" Then
			be_quiet = True
		Else
			be_quiet = False
		End If
		
		'�퓬�A�j����\������ꍇ�̓}�b�v�E�B���h�E���N���A����
		If BattleAnimation Then
			If MainWidth <> 15 Then
				ClearUnitStatus()
			End If
			If Not IsOptionDefined("�퓬����ʏ���������") Then
				RedrawScreen()
			End If
		End If
		
		orig_t = t
		
		'���΂������Ƀ^�[�Q�b�g�̈ʒu�����̃^�[�Q�b�g�̈ʒu�ƈ�v�����邽�ߋL�^
		tx = t.x
		ty = t.y
		tarea = t.Area
		
begin: 
		
		'�����X�V
		Update()
		MainPilot.UpdateSupportMod()
		t.Update()
		t.MainPilot.UpdateSupportMod()
		
		'�_���[�W�\���̂��߁A�^�[�Q�b�g�̂g�o���L�^���Ă���
		prev_hp = t.HP
		
		'�e��ݒ�����Z�b�g
		msg = ""
		is_critical = False
		critical_type = ""
		use_shield = False
		use_shield_msg = False
		use_protect_msg = False
		use_support_guard = False
		is_penetrated = False
		
		'���������Z�o
		prob = HitProbability(w, t, True)
		
		'�_���[�W���Z�o
		dmg = Damage(w, t, True, InStr(attack_mode, "����U��") > 0)
		
		'������ʂ������Ȃ�����Ȃ�N���e�B�J���̉\������
		If IsNormalWeapon(w) And dmg > 0 Then
			If CriticalProbability(w, t, def_mode) >= Dice(100) Or attack_mode = "����" Or attack_mode = "��������U��" Then
				is_critical = True
			End If
		End If
		
		ReDim partners(0)
		ReDim SelectedPartners(0)
		If attack_mode <> "�}�b�v�U��" And attack_mode <> "����" And Not second_attack Then
			If IsWeaponClassifiedAs(w, "��") Then
				'���̋Z�̏ꍇ�Ƀp�[�g�i�[���n�C���C�g�\��
				If WeaponMaxRange(w) = 1 Then
					CombinationPartner("����", w, partners, tx, ty)
				Else
					CombinationPartner("����", w, partners)
				End If
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.x, .y) = False
					End With
				Next 
				If Not BattleAnimation Then
					MaskScreen()
				End If
			ElseIf Not is_critical And dmg > 0 And InStr(attack_mode, "����U��") = 0 Then 
				'�A�g�U�����������邩�𔻒�
				'�i�A�g�U���͍��̋Z�ł͔������Ȃ��j
				If Weapon(w).MaxRange > 1 Then
					su = LookForAttackHelp(x, y)
				Else
					su = LookForAttackHelp(tx, ty)
				End If
				If Not su Is Nothing Then
					'�A�g�U������
					MaskData(su.x, su.y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					If IsMessageDefined("�A�g�U��(" & su.MainPilot.Name & ")", True) Then
						PilotMessage("�A�g�U��(" & su.MainPilot.Name & ")")
					Else
						PilotMessage("�A�g�U��(" & su.MainPilot.Nickname & ")")
					End If
					is_critical = True
					'UPGRADE_NOTE: �I�u�W�F�N�g su ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					su = Nothing
				End If
			End If
		End If
		
		'�N���e�B�J���Ȃ�_���[�W1.5�{
		If is_critical Then
			If IsOptionDefined("�_���[�W�{���ቺ") Then
				If IsWeaponClassifiedAs(w, "��") Then
					dmg = (1 + 0.1 * (WeaponLevel(w, "��") + 2)) * dmg
				Else
					dmg = 1.2 * dmg
				End If
			Else
				If IsWeaponClassifiedAs(w, "��") Then
					dmg = (1 + 0.25 * (WeaponLevel(w, "��") + 2)) * dmg
				Else
					dmg = 1.5 * dmg
				End If
			End If
		End If
		
		'�U����ނ̃A�j���\��
		If BattleAnimation Then
			Select Case attack_mode
				Case "����U��", "��������U��"
					ShowAnimation("����U������")
				Case "�J�E���^�["
					ShowAnimation("�J�E���^�[����")
			End Select
		End If
		
		'�U�����̃��b�Z�[�W�\��
		If Not be_quiet Then
			'�U�������̌��ʉ�
			If IsAnimationDefined(wname & "(����)") Then
				PlayAnimation(wname & "(����)")
			ElseIf IsAnimationDefined(wname) And Not IsOptionDefined("���폀���A�j����\��") And WeaponAnimation Then 
				PlayAnimation(wname & "(����)")
			ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
				SpecialEffect(wname & "(����)")
			Else
				PrepareWeaponEffect(Me, w)
			End If
			
			'�U�����b�Z�[�W�̑O�ɏo�͂���郁�b�Z�[�W
			If second_attack Then
				PilotMessage("�čU��")
			ElseIf InStr(attack_mode, "����U��") > 0 Then 
				With AttackUnit.CurrentForm.MainPilot
					If IsMessageDefined("�T�|�[�g�A�^�b�N(" & .Name & ")") Then
						PilotMessage("�T�|�[�g�A�^�b�N(" & .Name & ")")
					ElseIf IsMessageDefined("�T�|�[�g�A�^�b�N(" & .Nickname & ")") Then 
						PilotMessage("�T�|�[�g�A�^�b�N(" & .Nickname & ")")
					ElseIf IsMessageDefined("�T�|�[�g�A�^�b�N") Then 
						PilotMessage("�T�|�[�g�A�^�b�N")
					End If
				End With
			ElseIf attack_mode = "�J�E���^�[" Then 
				PilotMessage("�J�E���^�[")
			ElseIf IsMessageDefined(wname) And wname <> "�i��" And wname <> "�ˌ�" And wname <> "�U��" And Not IsWeaponClassifiedAs(w, "��") Then 
				If IsMessageDefined("������(" & wname & ")") Then
					PilotMessage("������(" & wname & ")")
				ElseIf IsDefense() Then 
					PilotMessage("������(����)")
				Else
					PilotMessage("������")
				End If
			End If
			
			'�U�����b�Z�[�W
			IsWavePlayed = False
			If Not second_attack Then
				If attack_mode = "�J�E���^�[" Then
					PilotMessage(wname, "�J�E���^�[")
				Else
					PilotMessage(wname, "�U��")
				End If
			End If
			
			'�U���A�j��
			If IsDefense() And IsAnimationDefined(wname & "(����)") Then
				PlayAnimation(wname & "(����)")
			ElseIf IsAnimationDefined(wname & "(�U��)") Or IsAnimationDefined(wname) Then 
				PlayAnimation(wname & "(�U��)")
			ElseIf IsSpecialEffectDefined(wname) Then 
				SpecialEffect(wname)
			ElseIf Not IsWavePlayed Then 
				AttackEffect(Me, w)
			End If
		ElseIf attack_mode = "��������" Then 
			'�U���A�j��
			If IsDefense() And IsAnimationDefined(wname & "(����)") Then
				PlayAnimation(wname & "(����)")
			ElseIf IsAnimationDefined(wname & "(�U��)") Or IsAnimationDefined(wname) Then 
				PlayAnimation(wname & "(�U��)")
			ElseIf IsSpecialEffectDefined(wname) Then 
				SpecialEffect(wname)
			ElseIf Not IsWavePlayed Then 
				AttackEffect(Me, w)
			End If
		End If
		
		If attack_mode <> "�}�b�v�U��" And attack_mode <> "����" Then
			'����g�p�ɂ��e�����d�m�̏���
			UseWeapon(w)
			'����g�p�ɂ��d�m����̕\��
			UpdateMessageForm(Me, t)
		End If
		
		'�h���i�ɂ�閽�����ቺ
		If def_mode = "���" Then
			If Not IsUnderSpecialPowerEffect("��Ζ���") And Not t.IsUnderSpecialPowerEffect("���h��") And Not t.IsFeatureAvailable("���s��") And Not t.IsConditionSatisfied("�ړ��s�\") Then
				prob = prob \ 2
			End If
		End If
		
		'���ˍU���̏ꍇ�͖��������ቺ
		If attack_mode = "����" Then
			prob = prob \ 2
		End If
		
		'�U�����s�������Ƃɂ��ẴV�X�e�����b�Z�[�W
		If Not be_quiet Then
			Select Case UBound(partners)
				Case 0
					'�ʏ�U��
					msg = Nickname & "��"
				Case 1
					'�Q�̍��̍U��
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "��[" & partners(1).MainPilot.Nickname & "]��"
					Else
						msg = Nickname & "�B��"
					End If
				Case 2
					'�R�̍��̍U��
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�A[" & partners(2).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "��[" & partners(1).MainPilot.Nickname & "]�A[" & partners(2).MainPilot.Nickname & "]�Ƌ���"
					Else
						msg = Nickname & "�B��"
					End If
				Case Else
					'�R�̈ȏ�ɂ�鍇�̍U��
					msg = Nickname & "�B��"
			End Select
			
			'�W�����v�U��
			If t.Area = "��" And (IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��")) And Not IsTransAvailable("��") Then
				msg = msg & "�W�����v���A"
			End If
			
			If second_attack Then
				msg = msg & "�ēx"
			ElseIf attack_mode = "�J�E���^�[" Or attack_mode = "�搧�U��" Then 
				msg = "�搧�U���I;" & msg & "�������"
			End If
			
			'�U���̎�ނɂ���ă��b�Z�[�W��؂�ւ�
			If Right(wnickname, 2) = "�U��" Or Right(wnickname, 4) = "�A�^�b�N" Or wnickname = "�ˌ�" Then
				msg = msg & "[" & wnickname & "]���������B;"
			ElseIf IsSpellWeapon(w) Then 
				If Right(wnickname, 2) = "����" Then
					msg = msg & "[" & wnickname & "]���������B;"
				ElseIf Right(wnickname, 2) = "�̏�" Then 
					msg = msg & "[" & Left(wnickname, Len(wnickname) - 2) & "]�̎������������B;"
				Else
					msg = msg & "[" & wnickname & "]�̎������������B;"
				End If
			ElseIf IsWeaponClassifiedAs(w, "��") Then 
				msg = msg & "[" & t.Nickname & "]�̎������𓐂����Ƃ����B;"
			ElseIf IsWeaponClassifiedAs(w, "�K") Then 
				msg = msg & "[" & t.Nickname & "]�̋Z���K�����悤�Ǝ��݂��B;"
			ElseIf IsWeaponClassifiedAs(w, "��") And (InStr(wnickname, "�~�T�C��") > 0 Or InStr(wnickname, "���P�b�g") > 0) Then 
				msg = msg & "[" & wnickname & "]�𔭎˂����B;"
			ElseIf Right(wnickname, 1) = "��" Or Right(wnickname, 3) = "�u���X" Or Right(wnickname, 2) = "����" Or Right(wnickname, 1) = "��" Or Right(wnickname, 3) = "�r�[��" Or Right(wnickname, 4) = "���[�U�[" Then 
				msg = msg & "[" & wnickname & "]��������B;"
			ElseIf Right(wnickname, 1) = "��" Then 
				msg = msg & "[" & wnickname & "]���̂����B;"
			ElseIf Right(wnickname, 2) = "�x��" Then 
				msg = msg & "[" & wnickname & "]��x�����B;"
			Else
				msg = msg & "[" & wnickname & "]�ōU�����������B;"
			End If
			
			'���������b�s���\��
			If is_event Then
				'�C�x���g�ɂ��U���̏ꍇ�͖��������X�y�V�����p���[�̉e�����܂߂��ɕ\��
				If def_mode = "���" Then
					buf = "������ = " & MinLng(HitProbability(w, t, False) \ 2, 100) & "��" & "�i" & VB6.Format(CriticalProbability(w, t, def_mode)) & "���j"
				Else
					buf = "������ = " & MinLng(HitProbability(w, t, False), 100) & "��" & "�i" & VB6.Format(CriticalProbability(w, t, def_mode)) & "���j"
				End If
			Else
				buf = "������ = " & MinLng(prob, 100) & "��" & "�i" & VB6.Format(CriticalProbability(w, t, def_mode)) & "���j"
			End If
			
			'�U������\��
			If IsSysMessageDefined(wname) Then
				'�u���햼(���)�v�̃��b�Z�[�W���g�p
				SysMessage(wname, "", buf)
			ElseIf IsSysMessageDefined("�U��") Then 
				'�u�U��(���)�v�̃��b�Z�[�W���g�p
				SysMessage("�U��", "", buf)
			Else
				DisplaySysMessage(msg & buf, BattleAnimation)
			End If
		End If
		
		msg = ""
		
		'�h����@��\��
		Select Case def_mode
			Case "���"
				If t.IsConditionSatisfied("�x��") Then
					msg = t.Nickname & "�͗x���Ă���B;"
				Else
					msg = t.Nickname & "�͉���^�����Ƃ����B;"
				End If
			Case "�h��"
				msg = t.Nickname & "�͖h��s�����Ƃ����B;"
		End Select
		
		'�X�y�V�����p���[�u�K�E�v�u�m���v
		If IsUnderSpecialPowerEffect("��Δj��") Or IsUnderSpecialPowerEffect("��Εm��") Then
			If Not be_quiet Then
				PilotMessage(wname & "(����)")
			End If
			
			If IsAnimationDefined(wname & "(����)") Or IsAnimationDefined(wname) Then
				PlayAnimation(wname & "(����)")
			ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
				SpecialEffect(wname & "(����)")
			ElseIf Not IsWavePlayed Then 
				HitEffect(Me, w, t)
			End If
			
			If IsUnderSpecialPowerEffect("��Εm��") Then
				' MOD START MARGE
				'            If t.HP > 10 Then
				'                dmg = t.HP - 10
				'            Else
				'                dmg = 0
				'            End If
				If IsOptionDefined("�_���[�W��������") Or IsOptionDefined("�_���[�W�����P") Then
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
		
		'���\�͂̏���
		If prob > 0 Then
			If CheckDodgeFeature(w, t, tx, ty, attack_mode, def_mode, dmg, be_quiet) Then
				dmg = 0
				GoTo EndAttack
			End If
		End If
		
		'�U���񐔂����߂�
		If IsWeaponClassifiedAs(w, "�A") Then
			attack_num = WeaponLevel(w, "�A")
		Else
			attack_num = 1
		End If
		
		'�����񐔂����߂�
		hit_count = 0
		For i = 1 To attack_num
			If Dice(100) <= prob Then
				hit_count = hit_count + 1
			End If
		Next 
		'�����񐔂Ɋ�ă_���[�W���C��
		dmg = dmg * hit_count \ attack_num
		
		'�U��������̏���
		If hit_count = 0 Then
			If IsAnimationDefined(wname & "(���)") Then
				PlayAnimation(wname & "(���)")
			ElseIf IsSpecialEffectDefined(wname & "(���)") Then 
				SpecialEffect(wname & "(���)")
			ElseIf t.IsAnimationDefined("���") Then 
				t.PlayAnimation("���")
			ElseIf t.IsSpecialEffectDefined("���") Then 
				t.SpecialEffect("���")
			Else
				DodgeEffect(Me, w)
			End If
			
			If Not be_quiet Then
				t.PilotMessage("���")
				PilotMessage(wname & "(���)")
			End If
			
			If t.IsSysMessageDefined("���") Then
				t.SysMessage("���")
			Else
				Select Case def_mode
					Case "���"
						If t.IsConditionSatisfied("�x��") Then
							DisplaySysMessage(t.Nickname & "�͌������x��Ȃ���U�������킵���B")
						Else
							DisplaySysMessage(t.Nickname & "�͉���^�����Ƃ�A�U�������킵���B")
						End If
					Case "�h��"
						DisplaySysMessage(t.Nickname & "�͖h��s�����Ƃ������A�U���͊O�ꂽ�B")
					Case Else
						DisplaySysMessage(t.Nickname & "�͍U�������킵���B")
				End Select
			End If
			GoTo EndAttack
		End If
		
		'�G���j�b�g�����΂�ꂽ�ꍇ�̏���
		If su Is Nothing Then
			use_support_guard = False
			If t.IsUnderSpecialPowerEffect("�݂����") Then
				'�X�y�V�����p���[�u�݂����v
				i = 1
				Do While i <= t.CountSpecialPower
					'UPGRADE_WARNING: �I�u�W�F�N�g t.SpecialPower(i).IsEffectAvailable(�݂����) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If t.SpecialPower(i).IsEffectAvailable("�݂����") Then
						If PList.IsDefined(t.SpecialPowerData(i)) Then
							su = PList.Item(t.SpecialPowerData(i)).Unit_Renamed
							t.RemoveSpecialPowerInEffect("�݂����")
							i = i - 1
							If Not su Is Nothing Then
								su = su.CurrentForm
								If su.Status_Renamed <> "�o��" Then
									'UPGRADE_NOTE: �I�u�W�F�N�g su ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
									su = Nothing
								End If
							End If
						End If
					End If
					i = i + 1
				Loop 
			ElseIf Not is_event And def_mode <> "�}�b�v�U��" And def_mode <> "����h��" Then 
				If t.IsDefense() Then
					'�T�|�[�g�K�[�h
					If UseSupportGuard Then
						su = t.LookForSupportGuard(Me, w)
						If Not su Is Nothing Then
							use_support_guard = True
							'�T�|�[�g�K�[�h�̎c��񐔂����炷
							su.UsedSupportGuard = su.UsedSupportGuard + 1
						End If
					End If
				End If
				If su Is Nothing Then
					'���΂�
					su = t.LookForGuardHelp(Me, w, is_critical)
				End If
			End If
			
			If Not su Is Nothing Then
				su.Update()
				
				'���b�Z�[�W�E�B���h�E�̕\�������ւ�
				If Party = "����" Or Party = "�m�o�b" Then
					UpdateMessageForm(su, Me)
				Else
					UpdateMessageForm(Me, su)
				End If
				
				If Not BattleAnimation Then
					'�g����ɂȂ郆�j�b�g���n�C���C�g�\��
					If MaskData(su.x, su.y) Then
						MaskData(su.x, su.y) = False
						MaskScreen()
						MaskData(su.x, su.y) = True
					End If
				End If
				
				'���΂��ۂ̃��b�Z�[�W
				If use_support_guard Then
					If su.IsMessageDefined("�T�|�[�g�K�[�h(" & t.MainPilot.Name & ")") Then
						su.PilotMessage("�T�|�[�g�K�[�h(" & t.MainPilot.Name & ")")
					ElseIf su.IsMessageDefined("�T�|�[�g�K�[�h(" & t.MainPilot.Nickname & ")") Then 
						su.PilotMessage("�T�|�[�g�K�[�h(" & t.MainPilot.Nickname & ")")
					ElseIf su.IsMessageDefined("�T�|�[�g�K�[�h") Then 
						su.PilotMessage("�T�|�[�g�K�[�h")
					End If
				ElseIf su.IsMessageDefined("���΂�(" & t.MainPilot.Name & ")") Then 
					su.PilotMessage("���΂�(" & t.MainPilot.Name & ")")
					use_protect_msg = True
				ElseIf su.IsMessageDefined("���΂�(" & t.MainPilot.Nickname & ")") Then 
					su.PilotMessage("���΂�(" & t.MainPilot.Nickname & ")")
					use_protect_msg = True
				End If
				msg = su.MainPilot.Nickname & "��[" & t.MainPilot.Nickname & "]�����΂����B;"
				
				'�g����ɂȂ郆�j�b�g���^�[�Q�b�g�̈ʒu�܂ňړ�
				With su
					'�A�j���\��
					If BattleAnimation Then
						If su.IsAnimationDefined("�T�|�[�g�K�[�h�J�n") Then
							su.PlayAnimation("�T�|�[�g�K�[�h�J�n")
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
					
					'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					MapDataForUnit(.x, .y) = Nothing
					prev_x = .x
					prev_y = .y
					prev_area = .Area
					.x = tx
					.y = ty
					.Area = tarea
					MapDataForUnit(.x, .y) = su
				End With
				
				'�^�[�Q�b�g���Đݒ�
				t = su
				SelectedTarget = t
				SelectedTargetForEvent = t
			End If
		End If
		If Not su Is Nothing Then
			'�_���[�W���Čv�Z
			With t
				prev_hp = .HP
				dmg = Damage(w, t, True)
				If is_critical Then
					If IsOptionDefined("�_���[�W�{���ቺ") Then
						If IsWeaponClassifiedAs(w, "��") Then
							dmg = (1 + 0.1 * (WeaponLevel(w, "��") + 2)) * dmg
						Else
							dmg = 1.2 * dmg
						End If
					Else
						If IsWeaponClassifiedAs(w, "��") Then
							dmg = (1 + 0.25 * (WeaponLevel(w, "��") + 2)) * dmg
						Else
							dmg = 1.5 * dmg
						End If
					End If
				End If
			End With
			
			'���΂��ꍇ�͏�ɑS�e����
			hit_count = attack_num
			
			'��ɖh�䃂�[�h�ɐݒ�
			def_mode = "�h��"
			
			'�T�|�[�g�K�[�h���s�����j�b�g�Ɋւ�������L�^
			If IsDefense() Then
				SupportGuardUnit2 = su
				SupportGuardUnitHPRatio2 = su.HP / su.MaxHP
			Else
				SupportGuardUnit = su
				SupportGuardUnitHPRatio = su.HP / su.MaxHP
			End If
		End If
		
		'�󂯂̏���
		If CheckParryFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet Or use_protect_msg) Then
			dmg = 0
			GoTo EndAttack
		End If
		
		'�h�䁕���΂����̓_���[�W�𔼌�
		If Not IsWeaponClassifiedAs(w, "�E") Then
			If def_mode = "�h��" And Not t.IsUnderSpecialPowerEffect("���h��") And Not t.IsFeatureAvailable("�h��s��") Then
				dmg = dmg \ 2
			End If
		End If
		
		'�_�~�[
		If CheckDummyFeature(w, t, be_quiet) Then
			dmg = 0
			GoTo EndAttack
		End If
		
		'����ȍ~�͖������̏���
		
		is_hit = True
		
		'�V�[���h�h�䔻��
		CheckShieldFeature(w, t, dmg, be_quiet, use_shield, use_shield_msg)
		
		'�h��\�͂̏���
		If CheckDefenseFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet Or use_protect_msg, is_penetrated) Then
			If Not be_quiet Then
				PilotMessage(wname & "(�U��������)")
			End If
			dmg = 0
			GoTo EndAttack
		End If
		
		'�������̓�����ʂ�\���B
		'�h��\�͂̏������ɍs���͍̂U���������̓�����ʂ�D�悳���邽�߁B
		IsWavePlayed = False
		If Not be_quiet Then
			PilotMessage(wname & "(����)")
		End If
		If IsAnimationDefined(wname & "(����)") Or IsAnimationDefined(wname) Then
			PlayAnimation(wname & "(����)")
		ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
			SpecialEffect(wname & "(����)")
		ElseIf Not IsWavePlayed Then 
			HitEffect(Me, w, t, hit_count)
		End If
		SysMessage(wname & "(����)")
		
		'���G�̏ꍇ
		If t.IsConditionSatisfied("���G") Then
			If Not be_quiet Then
				t.PilotMessage("�U��������")
				PilotMessage(wname & "(�U��������)")
			End If
			DisplaySysMessage(msg & t.Nickname & "��[" & wnickname & "]�𖳌��������I")
			dmg = 0
			GoTo EndAttack
		End If
		
		'���E�U���͈ꌂ�œ|����ꍇ�ɂ��������Ȃ�
		If IsWeaponClassifiedAs(w, "�E") Then
			If t.HP > dmg Then
				DisplaySysMessage(msg & t.Nickname & "�͍U���ɂ��e�����󂯂Ȃ������B")
				GoTo EndAttack
			End If
		End If
		
		'�^�[�Q�b�g�ʒu��ύX����U���̓T�|�[�g�K�[�h�̏ꍇ�͖���
		If su Is Nothing And def_mode <> "����h��" Then
			'������΂�
			If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "�j") Then
				CheckBlowAttack(w, t, dmg, msg, attack_mode, def_mode, critical_type)
			End If
			
			'������
			If IsWeaponClassifiedAs(w, "��") Then
				CheckDrawAttack(w, t, msg, def_mode, critical_type)
			End If
			
			'�����]��
			If IsWeaponClassifiedAs(w, "�]") Then
				CheckTeleportAwayAttack(w, t, msg, def_mode, critical_type)
			End If
		End If
		
		'�N���e�B�J�����b�Z�[�W�͂��̎��_�Œǉ�
		If is_critical Then
			msg = msg & "�N���e�B�J���I;"
		End If
		
		'�V�[���h�h��̌��ʓK�p
		Dim spower As Integer
		If use_shield Then
			If IsWeaponClassifiedAs(w, "�j") Then
				If t.IsFeatureAvailable("���^�V�[���h") Then
					dmg = (5 * dmg) \ 6
				Else
					dmg = 3 * dmg \ 4
				End If
			Else
				If t.IsFeatureAvailable("���^�V�[���h") Then
					dmg = (2 * dmg) \ 3
				Else
					dmg = dmg \ 2
				End If
			End If
			
			If t.IsFeatureAvailable("�G�l���M�[�V�[���h") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "��") And Not IsUnderSpecialPowerEffect("�h��\�͖�����") Then
				
				t.EN = t.EN - 5
				
				If IsWeaponClassifiedAs(w, "�j") Then
					spower = 50 * t.FeatureLevel("�G�l���M�[�V�[���h")
				Else
					spower = 100 * t.FeatureLevel("�G�l���M�[�V�[���h")
				End If
				
				If dmg <= spower Then
					If attack_mode <> "����" Then
						UpdateMessageForm(Me, t)
					Else
						UpdateMessageForm(Me, Nothing)
					End If
					
					fname = t.FeatureName0("�G�l���M�[�V�[���h")
					If Not be_quiet Then
						If t.IsMessageDefined("�U��������(" & fname & ")") Then
							t.PilotMessage("�U��������(" & fname & ")")
						Else
							t.PilotMessage("�U��������")
						End If
					End If
					If t.IsAnimationDefined("�U��������", fname) Then
						t.PlayAnimation("�U��������", fname)
					Else
						t.SpecialEffect("�U��������", fname)
					End If
					
					DisplaySysMessage(msg & fname & "���U����h�����B")
					GoTo EndAttack
				End If
				
				dmg = dmg - spower
			End If
		End If
		
		'�Œ�_���[�W��10
		If dmg > 0 And dmg < 10 Then
			dmg = 10
		End If
		
		'�s���ɂ��j�󂳂��Ȃ��ꍇ
		If (IsUnderSpecialPowerEffect("�Ă�����") And MainPilot.Technique > t.MainPilot.Technique And InStr(attack_mode, "����U��") = 0) Or t.IsConditionSatisfied("�s���g") Then
			If t.HP <= 10 Then
				dmg = 0
			ElseIf t.HP - dmg < 10 Then 
				dmg = t.HP - 10
			End If
		End If
		
		'�������
		CauseEffect(w, t, msg, critical_type, def_mode, dmg >= t.HP)
		
		If InStr(critical_type, "����") > 0 And Not use_support_guard And Not use_protect_msg Then
			If t.IsHero() Then
				msg = msg & WeaponNickname(w) & "��" & t.Nickname & "�̖���D�����B;"
			Else
				msg = msg & WeaponNickname(w) & "��" & t.Nickname & "���ꌂ�Ŕj�󂵂��B;"
			End If
			dmg = t.HP
		Else
			If t.HP - dmg < 0 Then
				dmg = t.HP
			End If
		End If
		
		
		'�m���ɔ�������������
		Dim prev_en As Short
		If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
			msg = msg & wnickname & "��[" & t.Nickname & "]��" & Term("�d�m", t) & "��ቺ�������B;"
			t.EN = MaxLng(t.EN - t.MaxEN * (dmg / t.MaxHP), 0)
		ElseIf IsWeaponClassifiedAs(w, "�D") And Not t.SpecialEffectImmune("�D") Then 
			msg = msg & Nickname & "��[" & t.Nickname & "]��" & Term("�d�m", t) & "���z�������B;"
			prev_en = t.EN
			t.EN = MaxLng(t.EN - t.MaxEN * (dmg / t.MaxHP), 0)
			EN = EN + (prev_en - t.EN) \ 2
		End If
		
		'�N���e�B�J�������b�Z�[�W
		If is_critical Or Len(critical_type) > 0 Then
			If Not be_quiet Then
				PilotMessage(wname & "(�N���e�B�J��)")
			End If
			If IsAnimationDefined(wname & "(�N���e�B�J��)") Then
				PlayAnimation(wname & "(�N���e�B�J��)")
			ElseIf IsSpecialEffectDefined(wname & "(�N���e�B�J��)") Then 
				SpecialEffect(wname & "(�N���e�B�J��)")
			Else
				CriticalEffect(critical_type, w, use_support_guard Or use_protect_msg)
			End If
		End If
		
ApplyDamage: 
		'�_���[�W�̓K�p
		t.HP = t.HP - dmg
		
		'�g�o�z��
		If IsWeaponClassifiedAs(w, "�z") And Not t.SpecialEffectImmune("�z") Then
			If HP < MaxHP Then
				msg = msg & Nickname & "��[" & t.Nickname & "]��" & Term("�g�o", t) & "���z�������B;"
				HP = HP + (prev_hp - t.HP) \ 4
			End If
		End If
		
		'�}�b�v�U���̏ꍇ�̓��b�Z�[�W���\������Ȃ��̂�
		'���̑���ɏ����f�B���C������
		If def_mode = "�}�b�v�U��" Then
			Sleep(150)
		End If
		
		'�_���[�W�ɂ��g�o�Q�[�W������\��
		If attack_mode <> "����" Then
			UpdateMessageForm(Me, t)
		Else
			UpdateMessageForm(Me, Nothing)
		End If
		
		'�_���[�W�ʕ\���O�ɃJ�b�g�C���͈�U�������Ă���
		If Not IsOptionDefined("�퓬����ʏ���������") Or attack_mode = "�}�b�v�U��" Then
			If IsPictureVisible Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
		End If
		
		'�_���[�W�ʂ��}�b�v�E�B���h�E�ɕ\��
		If Not IsOptionDefined("�_���[�W�\������") Or attack_mode = "�}�b�v�U��" Then
			If IsAnimationDefined(wname & "(�_���[�W�\��)") Then
				PlayAnimation(wname & "(�_���[�W�\��)")
			ElseIf IsAnimationDefined("�_���[�W�\��") Then 
				PlayAnimation("�_���[�W�\��")
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
		
		'������������
		If t.HP > 0 Then
			CheckAutoAttack(w, t, attack_mode, def_mode, dmg, be_quiet Or use_protect_msg)
			If Status_Renamed <> "�o��" Then
				GoTo EndAttack
			End If
		End If
		
		'�j��A�j��
		If t.HP = 0 Then
			If t.IsAnimationDefined("�j��") Then
				t.PlayAnimation("�j��")
			Else
				t.SpecialEffect("�j��")
			End If
		End If
		
		'�p�[�c�����������\���`�F�b�N
		separate_parts = False
		If t.HP = 0 Then
			If t.IsFeatureAvailable("�p�[�c����") Then
				If t.OtherForm(LIndex(t.FeatureData("�p�[�c����"), 2)).IsAbleToEnter(t.x, t.y) Then
					If t.IsFeatureLevelSpecified("�p�[�c����") Then
						If Dice(100) <= 10 * t.FeatureLevel("�p�[�c����") Then
							separate_parts = True
						End If
					Else
						separate_parts = True
					End If
				End If
			End If
		End If
		
		'�j�󃁃b�Z�[�W
		If attack_mode <> "�}�b�v�U��" And Not use_protect_msg And Not use_shield_msg Then
			If t.HP = 0 Then
				If separate_parts Then
					fname = t.FeatureName("�p�[�c����")
					If t.IsMessageDefined("�j�󎞕���(" & t.Name & ")") Then
						t.PilotMessage("�j�󎞕���(" & t.Name & ")")
					ElseIf t.IsMessageDefined("�j�󎞕���(" & fname & ")") Then 
						t.PilotMessage("�j�󎞕���(" & fname & ")")
					ElseIf t.IsMessageDefined("�j��") Then 
						t.PilotMessage("�j�󎞕���")
					ElseIf t.IsMessageDefined("����(" & t.Name & ")") Then 
						t.PilotMessage("����(" & t.Name & ")")
					ElseIf t.IsMessageDefined("����(" & fname & ")") Then 
						t.PilotMessage("����(" & fname & ")")
					ElseIf t.IsMessageDefined("����") Then 
						t.PilotMessage("����")
					Else
						t.PilotMessage("�_���[�W��")
					End If
				Else
					t.PilotMessage("�j��")
				End If
			End If
		End If
		
		If Not be_quiet Then
			If t.HP = 0 Then
				'�Ƃǂ߃��b�Z�[�W
				PilotMessage(wname & "(�Ƃǂ�)")
			Else
				'�_���[�W���b�Z�[�W
				PilotMessage(wname & "(�_���[�W)")
			End If
		End If
		
		'�_���[�W�A�j��
		If t.HP = 0 Then
			'�ǂǂ߃A�j��
			If attack_mode <> "�}�b�v�U��" And attack_mode <> "����" Then
				If IsAnimationDefined(wname & "(�Ƃǂ�)") Then
					PlayAnimation(wname & "(�Ƃǂ�)")
				Else
					SpecialEffect(wname & "(�Ƃǂ�)")
				End If
			End If
		Else
			If ((dmg <= 0.05 * t.MaxHP And t.HP >= 0.25 * t.MaxHP) Or dmg <= 10) And Len(critical_type) = 0 Then
				'�_���[�W�����ɏ�����
				If t.IsAnimationDefined("�_���[�W��") Then
					t.PlayAnimation("�_���[�W��")
				Else
					t.SpecialEffect("�_���[�W��")
				End If
			ElseIf t.HP < 0.25 * t.MaxHP Then 
				'�_���[�W��
				If t.IsAnimationDefined("�_���[�W��") Then
					t.PlayAnimation("�_���[�W��")
				Else
					t.SpecialEffect("�_���[�W��")
				End If
			ElseIf t.HP > 0.8 * t.MaxHP And Len(critical_type) = 0 Then 
				'�_���[�W��
				If t.IsAnimationDefined("�_���[�W��") Then
					t.PlayAnimation("�_���[�W��")
				Else
					t.SpecialEffect("�_���[�W��")
				End If
			Else
				'�_���[�W��
				If t.IsAnimationDefined("�_���[�W��") Then
					t.PlayAnimation("�_���[�W��")
				Else
					t.SpecialEffect("�_���[�W��")
				End If
			End If
		End If
		
		'�_���[�W���b�Z�[�W
		If attack_mode <> "�}�b�v�U��" And Not use_protect_msg And Not use_shield_msg Then
			If t.HP = 0 Then
				'�j�󎞃��b�Z�[�W�͊��ɕ\�����Ă���
			ElseIf ((dmg <= 0.05 * t.MaxHP And t.HP >= 0.25 * t.MaxHP) Or dmg <= 10) And Len(critical_type) = 0 Then 
				'�_���[�W�����ɏ�����
				t.PilotMessage("�_���[�W��")
			ElseIf t.HP < 0.25 * t.MaxHP Then 
				'�_���[�W��
				t.PilotMessage("�_���[�W��")
			ElseIf is_penetrated And t.IsMessageDefined("�o���A�ђ�") Then 
				t.PilotMessage("�o���A�ђ�")
			ElseIf t.HP >= 0.8 * t.MaxHP And Len(critical_type) = 0 Then 
				'�X�e�[�^�X�ُ킪�N�������ꍇ�͍Œ�ł��_���[�W���̃��b�Z�[�W
				t.PilotMessage("�_���[�W��")
			Else
				t.PilotMessage("�_���[�W��")
			End If
		End If
		
		'�V�[���h�h��
		If use_shield And t.HP > 0 Then
			If t.IsFeatureAvailable("�V�[���h") Then
				fname = t.FeatureName("�V�[���h")
				If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
					t.SysMessage("�V�[���h�h��", fname)
				Else
					msg = msg & t.Nickname & "��[" & fname & "]�Ŗh�䂵���B;"
				End If
			ElseIf t.IsFeatureAvailable("�G�l���M�[�V�[���h") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "��") And Not IsUnderSpecialPowerEffect("�h��\�͖�����") Then 
				t.EN = t.EN - 5
				fname = t.FeatureName("�G�l���M�[�V�[���h")
				If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
					t.SysMessage("�V�[���h�h��", fname)
				Else
					msg = msg & t.Nickname & "��[" & fname & "]��W�J�����B;"
				End If
			ElseIf t.IsFeatureAvailable("���^�V�[���h") Then 
				fname = t.FeatureName("���^�V�[���h")
				If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
					t.SysMessage("�V�[���h�h��", fname)
				Else
					msg = msg & t.Nickname & "��[" & fname & "]�Ŗh�䂵���B;"
				End If
			ElseIf t.IsFeatureAvailable("��^�V�[���h") Then 
				fname = t.FeatureName("��^�V�[���h")
				If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
					t.SysMessage("�V�[���h�h��", fname)
				Else
					msg = msg & t.Nickname & "��[" & fname & "]�Ŗh�䂵���B;"
				End If
			ElseIf t.IsFeatureAvailable("�A�N�e�B�u�V�[���h") Then 
				fname = t.FeatureName("�A�N�e�B�u�V�[���h")
				If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
					t.SysMessage("�V�[���h�h��", fname)
				ElseIf Not t.IsHero Then 
					msg = msg & t.Nickname & "��[" & fname & "]���@�̂�������B;"
				Else
					msg = msg & fname & "��[" & t.Nickname & "]��������B;"
				End If
			End If
		End If
		
		'�^�[�Q�b�g�������c�����ꍇ�̏���
		If t.HP > 0 Then
			If dmg = 0 Then
				If Len(critical_type) > 0 Then
					DisplaySysMessage(msg)
				ElseIf IsWeaponClassifiedAs(w, "��") Then 
					'���ݎ��s
					If t.IsConditionSatisfied("������҂�") Then
						DisplaySysMessage(msg & t.Nickname & "�͓��߂镨�������Ă��Ȃ������B")
					Else
						DisplaySysMessage(msg & t.Nickname & "�͑f������������������B")
					End If
				ElseIf IsWeaponClassifiedAs(w, "�K") Then 
					'���[�j���O���s
					If t.IsFeatureAvailable("���[�j���O�\�Z") Then
						buf = t.FeatureData("���[�j���O�\�Z")
						Select Case LIndex(buf, 2)
							Case "�\��", ""
								fname = LIndex(buf, 1)
							Case Else
								fname = LIndex(buf, 2)
						End Select
						If MainPilot.IsSkillAvailable(LIndex(buf, 1)) Then
							DisplaySysMessage(msg & MainPilot.Nickname & "�́u" & fname & "�v�����ɏK�����Ă����B")
						Else
							DisplaySysMessage(msg & MainPilot.Nickname & "�́u" & fname & "�v���K���o���Ȃ������B")
						End If
					Else
						DisplaySysMessage(msg & t.Nickname & "�͏K���\�ȋZ�������Ă��Ȃ������B")
					End If
				ElseIf IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then 
					'�\�̓R�s�[�̔���͂��ꂩ��
				Else
					DisplaySysMessage(msg & t.Nickname & "�͍U���ɂ��e�����󂯂Ȃ������B")
				End If
			ElseIf t.IsConditionSatisfied("�f�[�^�s��") Then 
				If attack_num > 1 Then
					msg = msg & VB6.Format(hit_count) & "�񖽒����A"
				End If
				DisplaySysMessage(msg & t.Nickname & "��[" & VB6.Format(dmg) & "]�̃_���[�W���󂯂��B")
			Else
				If attack_num > 1 Then
					msg = msg & VB6.Format(hit_count) & "�񖽒����A"
				End If
				DisplaySysMessage(msg & t.Nickname & "��[" & VB6.Format(dmg) & "]�̃_���[�W���󂯂��B;" & "�c��g�o��" & VB6.Format(t.HP) & "�i������ = " & VB6.Format(100 * (t.MaxHP - t.HP) \ t.MaxHP) & "���j")
			End If
			
			'����\�́u�s����v�ɂ��\���`�F�b�N
			If t.IsFeatureAvailable("�s����") Then
				If t.HP <= t.MaxHP \ 4 And Not t.IsConditionSatisfied("�\��") Then
					t.AddCondition("�\��", -1)
					t.Update()
					If t.IsHero Then
						DisplaySysMessage(t.Nickname & "�͖\�������B")
					Else
						If Len(t.FeatureName("�s����")) > 0 Then
							DisplaySysMessage(t.Nickname & "��[" & t.FeatureName("�s����") & "]�̖\���̂��߂ɐ���s�\�Ɋׂ����B")
						Else
							DisplaySysMessage(t.Nickname & "�͐���s�\�Ɋׂ����B")
						End If
					End If
				End If
			End If
			
			'�_���[�W���󂯂�Ζ��肩�炳�߂�
			If t.IsConditionSatisfied("����") And Not IsWeaponClassifiedAs(w, "��") Then
				t.DeleteCondition("����")
				DisplaySysMessage(t.Nickname & "�͖��肩��o�߂��B")
			End If
			
			'�_���[�W���󂯂�Ɠ�������
			If t.IsConditionSatisfied("����") And Not IsWeaponClassifiedAs(w, "��") Then
				t.DeleteCondition("����")
				DisplaySysMessage(t.Nickname & "�͓�����Ԃ���J�����ꂽ�B")
			End If
		End If
		
		'�j�󂳂ꂽ�ꍇ�̏���
		Dim morale_mod As Short
		If t.HP = 0 Then
			If attack_num > 1 Then
				msg = msg & VB6.Format(hit_count) & "�񖽒����A"
			End If
			If t.IsSysMessageDefined("�j��") Then
				t.SysMessage("�j��")
			ElseIf t.IsHero Then 
				DisplaySysMessage(msg & t.Nickname & "��[" & VB6.Format(dmg) & "]�̃_���[�W���󂯓|���ꂽ�B")
			Else
				DisplaySysMessage(msg & t.Nickname & "��[" & VB6.Format(dmg) & "]�̃_���[�W���󂯔j�󂳂ꂽ�B")
			End If
			
			'�������邩�ǂ����̃`�F�b�N���s��
			
			'�X�y�V�����p���[�u�����v
			If t.IsUnderSpecialPowerEffect("����") Then
				t.RemoveSpecialPowerInEffect("�j��")
				GoTo Resurrect
			End If
			
			'�p�C���b�g�p����\�́u�p�Y�v���u�Đ��v
			If Not is_event And Not IsUnderSpecialPowerEffect("��Δj��") Then
				If Dice(16) <= t.MainPilot.SkillLevel("�p�Y") Then
					t.HP = t.MaxHP \ 2
					t.IncreaseMorale(10)
					If t.IsMessageDefined("����") Then
						t.PilotMessage("����")
					End If
					If t.IsAnimationDefined("����") Then
						t.PlayAnimation("����")
					Else
						t.SpecialEffect("����")
					End If
					
					buf = t.MainPilot.SkillName0("�p�Y")
					If buf = "��\��" Then
						buf = "�p�Y"
					End If
					If t.IsSysMessageDefined("����", buf) Then
						t.SysMessage("����", buf)
					Else
						DisplaySysMessage(t.MainPilot.Nickname & "�̔M��" & buf & "�̐S��[" & t.Nickname & "]�𕜊��������I")
					End If
					GoTo Resurrect
				End If
				
				'�򉻂̓K�p
				If t.MainPilot.IsSkillAvailable("�Đ�") Then
					If IsWeaponClassifiedAs(w, "��") Then
						If MainPilot.IsSkillAvailable("��") Then
							If IsMessageDefined("��(" & wname & ")") Then
								PilotMessage("��(" & wname & ")")
								If IsAnimationDefined("��", wname) Then
									PlayAnimation("��", wname)
								Else
									SpecialEffect("��", wname)
								End If
							ElseIf IsMessageDefined("��") Then 
								PilotMessage("��")
								If IsAnimationDefined("��", wname) Then
									PlayAnimation("��", wname)
								Else
									SpecialEffect("��", wname)
								End If
							ElseIf IsMessageDefined("���(" & wname & ")") Then 
								PilotMessage("���(" & wname & ")")
								If IsAnimationDefined("���", wname) Then
									PlayAnimation("���", wname)
								Else
									SpecialEffect("���", wname)
								End If
							ElseIf IsMessageDefined("���") Then 
								PilotMessage("���")
								If IsAnimationDefined("���", wname) Then
									PlayAnimation("���", wname)
								Else
									SpecialEffect("���", wname)
								End If
							End If
							If IsSysMessageDefined("��") Then
								SysMessage("��")
							Else
								DisplaySysMessage(MainPilot.Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
							End If
							GoTo Cure
						End If
						For i = 2 To CountPilot
							If Pilot(i).IsSkillAvailable("��") Then
								If IsMessageDefined("��(" & wname & ")") Then
									PilotMessage("��(" & wname & ")")
									If IsAnimationDefined("��", wname) Then
										PlayAnimation("��", wname)
									Else
										SpecialEffect("��", wname)
									End If
								ElseIf IsMessageDefined("��") Then 
									PilotMessage("��")
									If IsAnimationDefined("��", wname) Then
										PlayAnimation("��", wname)
									Else
										SpecialEffect("��", wname)
									End If
								ElseIf IsMessageDefined("���(" & wname & ")") Then 
									PilotMessage("���(" & wname & ")")
									If IsAnimationDefined("���", wname) Then
										PlayAnimation("���", wname)
									Else
										SpecialEffect("���", wname)
									End If
								ElseIf IsMessageDefined("���") Then 
									PilotMessage("���")
									If IsAnimationDefined("���", wname) Then
										PlayAnimation("���", wname)
									Else
										SpecialEffect("���", wname)
									End If
								End If
								If IsSysMessageDefined("��") Then
									SysMessage("��")
								Else
									DisplaySysMessage(Pilot(i).Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
								End If
								GoTo Cure
							End If
						Next 
						For i = 1 To CountSupport
							If Support(i).IsSkillAvailable("��") Then
								If IsMessageDefined("��(" & wname & ")") Then
									PilotMessage("��(" & wname & ")")
									If IsAnimationDefined("��", wname) Then
										PlayAnimation("��", wname)
									Else
										SpecialEffect("��", wname)
									End If
								ElseIf IsMessageDefined("��") Then 
									PilotMessage("��")
									If IsAnimationDefined("��", wname) Then
										PlayAnimation("��", wname)
									Else
										SpecialEffect("��", wname)
									End If
								ElseIf IsMessageDefined("���(" & wname & ")") Then 
									PilotMessage("���(" & wname & ")")
									If IsAnimationDefined("���", wname) Then
										PlayAnimation("���", wname)
									Else
										SpecialEffect("���", wname)
									End If
								ElseIf IsMessageDefined("���") Then 
									PilotMessage("���")
									If IsAnimationDefined("���", wname) Then
										PlayAnimation("���", wname)
									Else
										SpecialEffect("���", wname)
									End If
								End If
								If IsSysMessageDefined("��") Then
									SysMessage("��")
								Else
									DisplaySysMessage(Support(i).Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
								End If
								GoTo Cure
							End If
						Next 
						If IsHero Then
							If IsMessageDefined("��(" & wname & ")") Then
								PilotMessage("��(" & wname & ")")
								If IsAnimationDefined("��", wname) Then
									PlayAnimation("��", wname)
								Else
									SpecialEffect("��", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
							ElseIf IsMessageDefined("��") Then 
								PilotMessage("��")
								If IsAnimationDefined("��", wname) Then
									PlayAnimation("��", wname)
								Else
									SpecialEffect("��", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
							ElseIf IsMessageDefined("���(" & wname & ")") Then 
								PilotMessage("���(" & wname & ")")
								If IsAnimationDefined("���", wname) Then
									PlayAnimation("���", wname)
								Else
									SpecialEffect("���", wname)
								End If
								DisplaySysMessage(MainPilot.Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
							ElseIf IsMessageDefined("���") Then 
								PilotMessage("���")
								If IsAnimationDefined("���", wname) Then
									PlayAnimation("���", wname)
								Else
									SpecialEffect("���", wname)
								End If
								If IsSysMessageDefined("��") Then
									SysMessage("��")
								Else
									DisplaySysMessage(MainPilot.Nickname & "�͏򉻂��s����[" & t.Nickname & "]�̕�����h�����B")
								End If
							End If
							GoTo Cure
						End If
					End If
					
					If Dice(16) <= t.MainPilot.SkillLevel("�Đ�") Then
						t.HP = t.MaxHP \ 2
						If t.IsMessageDefined("����") Then
							t.PilotMessage("����")
						End If
						If t.IsAnimationDefined("����") Then
							t.PlayAnimation("����")
						Else
							t.SpecialEffect("����")
						End If
						
						buf = t.MainPilot.SkillName0("�Đ�")
						If buf = "��\��" Then
							buf = "�Đ�"
						End If
						If t.IsSysMessageDefined("�Đ�", buf) Then
							t.SysMessage("�Đ�", buf)
						Else
							DisplaySysMessage(t.Nickname & "��" & buf & "�̗͂ň�u�ɂ��ĕ��������I")
						End If
						GoTo Resurrect
					End If
				End If
			End If
			
Cure: 
			
			'���j�b�g�j��ɂ��p�[�c����
			If separate_parts Then
				uname = LIndex(t.FeatureData("�p�[�c����"), 2)
				
				If Not t.IsHero Then
					If BattleAnimation Then
						ExplodeAnimation((t.Size), t.x, t.y)
					Else
						PlayWave("Explode.wav")
					End If
				End If
				
				fname = t.FeatureName("�p�[�c����")
				If t.IsAnimationDefined("�j�󎞕���(" & t.Name & ")") Then
					t.PlayAnimation("�j�󎞕���(" & t.Name & ")")
				ElseIf t.IsAnimationDefined("�j�󎞕���(" & fname & ")") Then 
					t.PlayAnimation("�j�󎞕���(" & fname & ")")
				ElseIf t.IsAnimationDefined("�j�󎞕���") Then 
					t.PlayAnimation("�j�󎞕���")
				ElseIf t.IsSpecialEffectDefined("�j�󎞕���(" & t.Name & ")") Then 
					t.SpecialEffect("�j�󎞕���(" & t.Name & ")")
				ElseIf t.IsSpecialEffectDefined("�j�󎞕���(" & fname & ")") Then 
					t.SpecialEffect("�j�󎞕���(" & fname & ")")
				ElseIf t.IsSpecialEffectDefined("�j�󎞕���") Then 
					t.SpecialEffect("�j�󎞕���")
				ElseIf t.IsAnimationDefined("����(" & t.Name & ")") Then 
					t.PlayAnimation("����(" & t.Name & ")")
				ElseIf t.IsAnimationDefined("����(" & fname & ")") Then 
					t.PlayAnimation("����(" & fname & ")")
				ElseIf t.IsAnimationDefined("����") Then 
					t.PlayAnimation("����")
				ElseIf t.IsSpecialEffectDefined("����(" & t.Name & ")") Then 
					t.SpecialEffect("����(" & t.Name & ")")
				ElseIf t.IsSpecialEffectDefined("����(" & fname & ")") Then 
					t.SpecialEffect("����(" & fname & ")")
				Else
					t.SpecialEffect("����")
				End If
				
				t.Transform(uname)
				With t.CurrentForm
					.HP = .MaxHP
					'��������U�����Ĕj�󂳂ꂽ���ɂ͍s������0��
					If .Party = Stage Then
						.UsedAction = .MaxAction
					End If
				End With
				
				If t.IsSysMessageDefined("�j�󎞕���(" & t.Name & ")") Then
					t.SysMessage("�j�󎞕���(" & t.Name & ")")
				ElseIf t.IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
					t.SysMessage("�j�󎞕���(" & fname & ")")
				ElseIf t.IsSysMessageDefined("�j�󎞕���") Then 
					t.SysMessage("�j�󎞕���")
				ElseIf t.IsSysMessageDefined("����(" & t.Name & ")") Then 
					t.SysMessage("����(" & t.Name & ")")
				ElseIf t.IsSysMessageDefined("����(" & fname & ")") Then 
					t.SysMessage("����(" & fname & ")")
				ElseIf t.IsSysMessageDefined("����") Then 
					t.SysMessage("����")
				ElseIf t.IsHero() Then 
					If t.Nickname <> t.OtherForm(uname).Nickname Then
						DisplaySysMessage(t.Nickname & "��" & t.OtherForm(uname).Nickname & "�ɕω������B")
					Else
						DisplaySysMessage(t.Nickname & "�͕ω����A�h�����B")
					End If
				Else
					DisplaySysMessage(t.Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
				End If
				
				t = t.CurrentForm
				SelectedTarget = t
				SelectedTargetForEvent = t
				GoTo Resurrect
			End If
			
			'���j�b�g�j��ɂ��C�͂̕ϓ�
			If attack_mode <> "�}�b�v�U��" Then
				'�G��j�󂵂����j�b�g�̃p�C���b�g�̓g�[�^���ŋC��+4
				If InStr(attack_mode, "����U��") > 0 Then
					AttackUnit.CurrentForm.IncreaseMorale(3)
				Else
					IncreaseMorale(3)
				End If
				
				'����ȊO�̃p�C���b�g
				For	Each p In PList
					With p
						'�o�����̃p�C���b�g�݂̂��Ώ�
						If .Unit_Renamed Is Nothing Then
							GoTo NextPilot
						End If
						If .Unit_Renamed.Status_Renamed <> "�o��" Then
							GoTo NextPilot
						End If
						
						If .Party = Party Then
							'�G��j�󂵂����j�b�g�̐w�c�̃p�C���b�g�͋C��+1
							If .Personality <> "�@�B" Then
								.Morale = .Morale + 1
							End If
						ElseIf .Party = t.Party Then 
							'�j�󂳂ꂽ���j�b�g�̐w�c�̃p�C���b�g�͐��i�ɉ����ċC�͂�ω�
							Select Case .Personality
								Case "�����C"
									morale_mod = 2
								Case "���C"
									morale_mod = 1
								Case "��C"
									morale_mod = -1
								Case Else
									morale_mod = 0
							End Select
							
							'�����̏ꍇ�̋C�͕ω��ʂ̓I�v�V�����ŕω�����
							If .Party = "����" And IsOptionDefined("�j�󎞖����C�͕ω��T�{") Then
								.Morale = .Morale + 5 * morale_mod
							Else
								.Morale = .Morale + morale_mod
							End If
						End If
					End With
NextPilot: 
				Next p
			End If
			
			'�E�o���b�Z�[�W�̕\��
			If t.IsMessageDefined("�E�o") And Not is_event And Not IsEventDefined("�j�� " & t.MainPilot.ID, True) Then
				t.PilotMessage("�E�o")
			End If
			
			'�퓬�A�j���\�����g��Ȃ��ꍇ�͂��΂������j�b�g�����̈ʒu�ɖ߂��Ă���
			If Not BattleAnimation Then
				If Not su Is Nothing Then
					With su
						.x = prev_x
						.y = prev_y
						.Area = prev_area
					End With
				End If
			End If
			
			'���j�b�g��j��
			t.Die()
		End If
		
Resurrect: '���������ꍇ�͔j��֘A�̏������s��Ȃ�
		
EndAttack: 
		
		If Status_Renamed = "�o��" And t.Status_Renamed = "�o��" And InStr(attack_mode, "����U��") = 0 And attack_mode <> "�}�b�v�U��" And attack_mode <> "����" And Not IsWeaponClassifiedAs(w, "��") And HP > 0 And t.HP > 0 Then
			'�čU��
			If Not second_attack And IsWeaponAvailable(w, "�X�e�[�^�X") And IsTargetWithinRange(w, t) Then
				'�X�y�V�����p���[���ʁu�čU���v
				If IsUnderSpecialPowerEffect("�čU��") Then
					second_attack = True
					RemoveSpecialPowerInEffect("�U��")
					GoTo begin
				End If
				
				'�čU���\��
				If MainPilot.IsSkillAvailable("�čU��") Then
					If MainPilot.Intuition >= t.MainPilot.Intuition Then
						slevel = 2 * MainPilot.SkillLevel("�čU��")
					Else
						slevel = MainPilot.SkillLevel("�čU��")
					End If
					If slevel >= Dice(32) Then
						second_attack = True
						RemoveSpecialPowerInEffect("�U��")
						GoTo begin
					End If
				End If
				
				'�đ���
				If WeaponLevel(w, "��") >= Dice(16) Then
					second_attack = True
					RemoveSpecialPowerInEffect("�U��")
					GoTo begin
				End If
			End If
			
			'�ǉ��U��
			If su Is t Then
				CheckAdditionalAttack(w, t, be_quiet, attack_mode, "����h��", dmg)
			Else
				CheckAdditionalAttack(w, t, be_quiet, attack_mode, "", dmg)
			End If
		End If
		
		'�T�|�[�g�K�[�h���s�������j�b�g�͔j�󏈗��̑O�ɈȑO�̈ʒu�ɕ��A������
		Dim sx, sy As Short
		If Not su Is Nothing Then
			su = su.CurrentForm
			With su
				
				sx = .x
				sy = .y
				
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(sx, sy) = Nothing
				
				.x = prev_x
				.y = prev_y
				.Area = prev_area
				
				If .Status_Renamed = "�o��" Then
					MapDataForUnit(.x, .y) = su
					MapDataForUnit(tx, ty) = orig_t
					If BattleAnimation Then
						If su.IsAnimationDefined("�T�|�[�g�K�[�h�I��") Then
							If Not IsRButtonPressed() Then
								su.PlayAnimation("�T�|�[�g�K�[�h�I��")
							End If
						Else
							If Not IsRButtonPressed() Then
								PaintUnitBitmap(orig_t, "���t���b�V������")
								If use_support_guard Then
									MoveUnitBitmap(su, sx, sy, .x, .y, 80, 4)
								Else
									MoveUnitBitmap(su, sx, sy, .x, .y, 50)
								End If
							Else
								PaintUnitBitmap(su, "���t���b�V������")
								PaintUnitBitmap(orig_t, "���t���b�V������")
							End If
						End If
					End If
				Else
					'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					MapDataForUnit(.x, .y) = Nothing
					MapDataForUnit(tx, ty) = orig_t
					PaintUnitBitmap(orig_t, "���t���b�V������")
				End If
			End With
		End If
		
		If is_hit Then
			'�U���𖽒����������Ƃɂ��C�͑���
			If attack_mode <> "�}�b�v�U��" And attack_mode <> "����" Then
				With CurrentForm
					If .MainPilot.IsSkillAvailable("�������C�͑���") Then
						.Pilot(1).Morale = .Pilot(1).Morale + .MainPilot.SkillLevel("�������C�͑���")
					End If
				End With
			End If
			
			'�U�����󂯂����Ƃɂ��C�͑���
			t.IncreaseMorale(1)
			If t.MainPilot.IsSkillAvailable("�������C�͑���") Then
				t.Pilot(1).Morale = t.Pilot(1).Morale + t.MainPilot.SkillLevel("�������C�͑���")
			End If
		Else
			'�U�����O�������Ƃɂ��C�͑���
			If attack_mode <> "�}�b�v�U��" And attack_mode <> "����" Then
				With CurrentForm
					If .MainPilot.IsSkillAvailable("���s���C�͑���") Then
						.Pilot(1).Morale = .Pilot(1).Morale + .MainPilot.SkillLevel("���s���C�͑���")
					End If
				End With
			End If
			
			'�U��������������Ƃɂ��C�͑���
			If t.MainPilot.IsSkillAvailable("������C�͑���") Then
				t.Pilot(1).Morale = t.Pilot(1).Morale + t.MainPilot.SkillLevel("������C�͑���")
			End If
		End If
		
		'�X�y�V�����p���[���ʂ̉���
		If InStr(msg, "���΂���") = 0 Then
			t.RemoveSpecialPowerInEffect("�h��")
		End If
		If is_hit Then
			t.RemoveSpecialPowerInEffect("��e")
		End If
		
		'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
		If t.IsConditionSatisfied("���j�b�g�摜") Then
			t.DeleteCondition("���j�b�g�摜")
			t.BitmapID = MakeUnitBitmap(t)
			If t.Status_Renamed = "�o��" Then
				PaintUnitBitmap(t, "���t���b�V������")
			End If
		End If
		If t.IsConditionSatisfied("��\���t��") Then
			t.DeleteCondition("��\���t��")
			t.BitmapID = MakeUnitBitmap(t)
			If t.Status_Renamed = "�o��" Then
				PaintUnitBitmap(t, "���t���b�V������")
			End If
		End If
		
		'�퓬�ɎQ���������j�b�g������
		With CurrentForm
			If IsOptionDefined("���j�b�g���B��") Then
				If .Party0 = "�G" Or .Party0 = "����" Then
					.AddCondition("���ʍς�", -1, 0, "��\��")
				End If
				If t.Party0 = "�G" Or t.Party0 = "����" Then
					t.AddCondition("���ʍς�", -1, 0, "��\��")
				End If
			Else
				If .IsConditionSatisfied("���j�b�g���B��") Then
					.DeleteCondition("���j�b�g���B��")
				End If
				If t.IsConditionSatisfied("���j�b�g���B��") Then
					t.DeleteCondition("���j�b�g���B��")
				End If
			End If
		End With
		
		'�����X�V
		CurrentForm.Update()
		t.Update()
		
		'�}�b�v�U���┽�˂ɂ��U���̏ꍇ�͂����܂�
		Select Case attack_mode
			Case "�}�b�v�U��", "����"
				RestoreSelections()
				Exit Sub
		End Select
		
		'�X�e���X��������H
		If IsFeatureAvailable("�X�e���X") Then
			If IsWeaponClassifiedAs(w, "�E") Then
				'�ÎE����̏ꍇ�A�����|�����s���s�\�ɂ���΃X�e���X�p��
				If t.CurrentForm.Status_Renamed = "�o��" And t.CurrentForm.MaxAction > 0 Then
					AddCondition("�X�e���X����", 1)
				End If
			Else
				AddCondition("�X�e���X����", 1)
			End If
		End If
		
		'���̋Z�̃p�[�g�i�[�̒e�����d�m�̏���
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountWeapon
					If .Weapon(j).Name = wname Then
						.UseWeapon(j)
						If .IsWeaponClassifiedAs(j, "��") Then
							If .IsFeatureAvailable("�p�[�c����") Then
								uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
						ElseIf .IsWeaponClassifiedAs(j, "��") And .HP = 0 Then 
							.Die()
						ElseIf .IsWeaponClassifiedAs(j, "��") Then 
							If .IsFeatureAvailable("�ό`�Z") Then
								uname = ""
								For k = 1 To .CountFeature
									If .Feature(k) = "�ό`�Z" And LIndex(.FeatureData(k), 1) = wname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("�m�[�}�����[�h") Then
										uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("�m�[�}�����[�h") Then 
								uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'�����̕��킪�Ȃ������ꍇ�͎����̃f�[�^���g���ď���
				If j > .CountWeapon Then
					If Weapon(w).ENConsumption > 0 Then
						.EN = .EN - WeaponENConsumption(w)
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						.AddCondition("����", 1)
					End If
					If IsWeaponClassifiedAs(w, "�b") And .IsConditionSatisfied("�`���[�W����") Then
						.DeleteCondition("�`���[�W����")
					End If
					If IsWeaponClassifiedAs(w, "�C") Then
						.IncreaseMorale(-5 * WeaponLevel(w, "�C"))
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "��")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsWeaponClassifiedAs(w, "�v") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "�v")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						.HP = MaxLng(.HP - .MaxHP * WeaponLevel(w, "��") \ 10, 0)
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						If .IsFeatureAvailable("�p�[�c����") Then
							uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
					ElseIf IsWeaponClassifiedAs(w, "��") And .HP = 0 Then 
						.Die()
					ElseIf IsWeaponClassifiedAs(w, "��") Then 
						If .IsFeatureAvailable("�m�[�}�����[�h") Then
							uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'�ȉ��̓�����ʂ͕͂���f�[�^�̕ω������邽�߁A�����ɂ͓K������Ȃ�
		
		If CurrentForm.Status_Renamed = "�j��" Then
			'���˓��ɂ��j�󂳂ꂽ�ꍇ�͂Ȃɂ��o���Ȃ�
			
			'�����U��
		ElseIf IsWeaponClassifiedAs(w, "��") Then 
			If IsFeatureAvailable("�p�[�c����") Then
				uname = LIndex(FeatureData("�p�[�c����"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("�p�[�c����")
					If IsSysMessageDefined("�j�󎞕���(" & Name & ")") Then
						SysMessage("�j�󎞕���(" & Name & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
						SysMessage("�j�󎞕���(" & fname & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���") Then 
						SysMessage("�j�󎞕���")
					ElseIf IsSysMessageDefined("����(" & Name & ")") Then 
						SysMessage("����(" & Name & ")")
					ElseIf IsSysMessageDefined("����(" & fname & ")") Then 
						SysMessage("����(" & fname & ")")
					ElseIf IsSysMessageDefined("����") Then 
						SysMessage("����")
					Else
						DisplaySysMessage(Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
					End If
				Else
					Die()
				End If
			Else
				Die()
			End If
			
			'�g�o����U���ɂ�鎩�E
		ElseIf IsWeaponClassifiedAs(w, "��") And HP = 0 Then 
			Die()
			
			'�ό`�Z
		ElseIf IsWeaponClassifiedAs(w, "��") Then 
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = wname Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'�A�C�e��������
		ElseIf Weapon(w).IsItem And Bullet(w) = 0 And MaxBullet(w) > 0 Then 
			'�A�C�e�����폜
			num = Data.CountWeapon
			num = num + MainPilot.Data.CountWeapon
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountWeapon
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountWeapon
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
			
			'�\�̓R�s�[
		ElseIf is_hit And (IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��")) And (dmg > 0 Or Not IsWeaponClassifiedAs(w, "�E")) Then 
			CheckMetamorphAttack(w, t, def_mode)
		End If
		
		With CurrentForm
			'�X�y�V�����p���[�̌��ʂ��폜
			If InStr(attack_mode, "����U��") = 0 Then
				If .IsUnderSpecialPowerEffect("�U�������") Then
					.AddCondition("����", 1)
				End If
				.RemoveSpecialPowerInEffect("�U��")
				If is_hit Then
					.RemoveSpecialPowerInEffect("����")
				End If
			End If
			
			'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
			If .IsConditionSatisfied("���j�b�g�摜") Then
				.DeleteCondition("���j�b�g�摜")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("��\���t��") Then
				.DeleteCondition("��\���t��")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			For i = 1 To UBound(partners)
				With partners(i).CurrentForm
					If .IsConditionSatisfied("���j�b�g�摜") Then
						.DeleteCondition("���j�b�g�摜")
						.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
						PaintUnitBitmap(partners(i).CurrentForm)
					End If
					If .IsConditionSatisfied("��\���t��") Then
						.DeleteCondition("��\���t��")
						.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
						PaintUnitBitmap(partners(i).CurrentForm)
					End If
				End With
			Next 
		End With
		
		'�J�b�g�C���͏������Ă���
		If Not IsOptionDefined("�퓬����ʏ���������") Or attack_mode = "�}�b�v�U��" Then
			If IsPictureVisible Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
		End If
		
		' ADD START MARGE
		'�퓬�A�j���I������
		If IsAnimationDefined(wname & "(�I��)") Then
			PlayAnimation(wname & "(�I��)")
		ElseIf IsAnimationDefined("�I��") Then 
			PlayAnimation("�I��")
		End If
		' ADD END MARGE
		
		'���j�b�g�I��������
		RestoreSelections()
	End Sub
	
	'���p����\�͂̔���
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
		
		'�X�y�V�����p���[�ŉ��\�͂�����������Ă���H
		If (IsUnderSpecialPowerEffect("��Ζ���") Or IsUnderSpecialPowerEffect("���\�͖�����")) And Not t.IsUnderSpecialPowerEffect("����h�䔭��") Then
			Exit Function
		End If
		
		'�\���h��͍s���ł��Ȃ���Δ������Ȃ�
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("���h��") Then
			Exit Function
		End If
		
		wname = WeaponNickname(w)
		team = MainPilot.SkillData("�`�[��")
		
		'�j�~������
		If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			GoTo SkipBlock
		End If
		
		'�L��j�~
		'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		u = Nothing
		flevel = 0
		fid = 0
		'�j�~���Ă���郆�j�b�g��T��
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint
				End If
				
				With MapDataForUnit(i, j)
					If .IsEnemy(t) Then
						GoTo NextPoint
					End If
					
					If .Area = "�n��" Then
						GoTo NextPoint
					End If
					
					If Not .IsFeatureAvailable("�L��j�~") Then
						GoTo NextPoint
					End If
					
					'�����`�[���ɑ����Ă���H
					uteam = .MainPilot.SkillData("�`�[��")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "�L��j�~" Then
							fdata = .FeatureData(k)
							
							'�L���͈�
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'�g�p����
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
							
							'���������𖞂����Ă���H
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("�j�~")) Then
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
			'�j�~���Ă���郆�j�b�g������ꍇ
			If fid = 0 Then
				fname = u.FeatureName("�L��j�~")
				fdata = u.FeatureData("�L��j�~")
				flevel = u.FeatureLevel("�L��j�~")
			Else
				fname = u.FeatureName(fid)
				fdata = u.FeatureData(fid)
				flevel = u.FeatureLevel(fid)
			End If
			If flevel = 1 Then
				flevel = 10000
			End If
			
			'�j�~�m���̐ݒ�
			buf = LIndex(fdata, 4)
			If IsNumeric(buf) Then
				prob = CShort(buf)
			ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
				i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
				prob = 100 * (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) / 16
			Else
				prob = u.SkillLevel(buf) * 100 / 16
			End If
			
			'���؂�
			If u.IsUnderSpecialPowerEffect("����h�䔭��") Then
				prob = 100
			End If
			
			'�K�����������Ă���Αj�~�͖���
			If IsUnderSpecialPowerEffect("��Ζ���") And Not u.IsUnderSpecialPowerEffect("����h�䔭��") Then
				prob = 0
			End If
			
			'�_���[�W�����e�͈͊O�ł���Αj�~�ł��Ȃ�
			If dmg > 500 * flevel Then
				prob = 0
			End If
			
			'�d�m�����
			If IsNumeric(LIndex(fdata, 5)) Then
				ecost = CShort(LIndex(fdata, 5))
			Else
				ecost = 0
			End If
			
			'�U����j�~
			If prob >= Dice(100) Then
				u.EN = u.EN - ecost
				If Not be_quiet Then
					If u.IsMessageDefined("�j�~(" & fname & ")") Then
						u.PilotMessage("�j�~(" & fname & ")")
					Else
						u.PilotMessage("�j�~")
					End If
				End If
				If u.IsAnimationDefined("�j�~", fname) Then
					u.PlayAnimation("�j�~", fname)
				Else
					u.SpecialEffect("�j�~", fname)
				End If
				
				If u.IsSysMessageDefined("�j�~", fname) Then
					u.SysMessage("�j�~", fname)
				Else
					DisplaySysMessage(u.Nickname & "��[" & fname & "]��[" & wname & "]��h�����B")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
		
SkipBlock: 
		
		'���g(���j�b�g�p����\��)
		If t.IsFeatureAvailable("���g") And t.MainPilot.Morale >= 130 And Not t.IsFeatureLevelSpecified("���g") And (Dice(2) = 1 Or t.IsUnderSpecialPowerEffect("����h�䔭��")) Then
			fname = t.FeatureName("���g")
			
			'�������
			If t.IsAnimationDefined("���g", fname) Then
				t.PlayAnimation("���g", fname)
			ElseIf t.IsSpecialEffectDefined("���g", fname) Then 
				t.SpecialEffect("���g", fname)
			ElseIf BattleAnimation Then 
				If fname = "���g" Then
					ShowAnimation("���g����")
				Else
					ShowAnimation("���g���� - " & fname)
				End If
			End If
			
			'�����
			DodgeEffect(Me, w)
			
			'���b�Z�[�W
			If Not be_quiet Then
				If t.IsMessageDefined("���g(" & fname & ")") Then
					t.PilotMessage("���g(" & fname & ")")
				Else
					t.PilotMessage("���g")
				End If
			End If
			
			If t.IsSysMessageDefined("���g", fname) Then
				t.SysMessage("���g", fname)
			Else
				If fname <> "���g" Then
					DisplaySysMessage(t.Nickname & "��[" & fname & "]���g���čU�������킵��")
				Else
					DisplaySysMessage(t.Nickname & "�͕��g���čU�������킵���B")
				End If
			End If
			
			CheckDodgeFeature = True
			Exit Function
		End If
		
		'�����
		If t.IsFeatureAvailable("�����") Then
			fname = t.FeatureName("�����")
			fdata = t.FeatureData("�����")
			flevel = t.FeatureLevel("�����")
			
			'������
			prob = flevel
			If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
				prob = 10
			End If
			
			'�K�v����
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
			If LIndex(fdata, 4) = "�蓮" Then
				If def_mode <> "���" Then
					prob = 0
				End If
			End If
			
			'���������𖞂����Ă���H
			If t.EN >= ecost And t.MainPilot.Morale >= nmorale And prob >= Dice(10) Then
				'�d�m����
				If ecost <> 0 Then
					t.EN = t.EN - ecost
					If attack_mode <> "����" Then
						UpdateMessageForm(Me, t)
					Else
						UpdateMessageForm(Me, Nothing)
					End If
				End If
				
				'�������
				If t.IsAnimationDefined("���g", fname) Then
					t.PlayAnimation("���g", fname)
				ElseIf t.IsSpecialEffectDefined("���g", fname) Then 
					t.SpecialEffect("���g", fname)
				ElseIf BattleAnimation Then 
					ShowAnimation("��𔭓�")
				Else
					'�����
					DodgeEffect(Me, w)
				End If
				
				'���b�Z�[�W
				If Not be_quiet Then
					If t.IsMessageDefined("���g(" & fname & ")") Then
						t.PilotMessage("���g(" & fname & ")")
					Else
						t.PilotMessage("���g")
					End If
				End If
				
				If t.IsSysMessageDefined("���g", fname) Then
					t.SysMessage("���g", fname)
				Else
					DisplaySysMessage(t.Nickname & "��[" & fname & "]���g���čU�������킵���B")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
		
		'�ً}�e���|�[�g
		Dim new_x, new_y As Short
		If t.IsFeatureAvailable("�ً}�e���|�[�g") Then
			fname = t.FeatureName("�ً}�e���|�[�g")
			fdata = t.FeatureData("�ً}�e���|�[�g")
			flevel = t.FeatureLevel("�ً}�e���|�[�g")
			
			'������
			prob = flevel
			If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
				prob = 10
			End If
			
			'�K�v����
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
			If LIndex(fdata, 5) = "�蓮" Then
				If def_mode <> "���" Then
					prob = 0
				End If
			End If
			
			'���������𖞂����Ă���H
			If t.EN >= ecost And t.MainPilot.Morale >= nmorale And prob >= Dice(10) Then
				
				'�����ꏊ������H
				AreaInTeleport(t, StrToLng(LIndex(fdata, 2)))
				SafetyPoint(t, new_x, new_y)
				
				If (t.x <> new_x Or t.y <> new_y) And new_x <> 0 And new_y <> 0 Then
					'�d�m����
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "����" Then
							UpdateMessageForm(Me, t)
						Else
							UpdateMessageForm(Me, Nothing)
						End If
					End If
					
					'�������
					If t.IsAnimationDefined("�ً}�e���|�[�g", fname) Then
						t.PlayAnimation("�ً}�e���|�[�g", fname)
					ElseIf t.IsSpecialEffectDefined("�ً}�e���|�[�g", fname) Then 
						t.SpecialEffect("�ً}�e���|�[�g", fname)
					ElseIf BattleAnimation Then 
						If fname = "�ً}�e���|�[�g" Then
							ShowAnimation("�ً}�e���|�[�g����")
						Else
							ShowAnimation("�ً}�e���|�[�g���� - " & fname)
						End If
					End If
					
					'�����
					DodgeEffect(Me, w)
					
					'�ً}�e���|�[�g�����I
					t.Jump(new_x, new_y)
					
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("�ً}�e���|�[�g(" & fname & ")") Then
							t.PilotMessage("�ً}�e���|�[�g(" & fname & ")")
						Else
							t.PilotMessage("�ً}�e���|�[�g")
						End If
					End If
					
					If t.IsSysMessageDefined("�ً}�e���|�[�g", fname) Then
						t.SysMessage("�ً}�e���|�[�g", fname)
					Else
						DisplaySysMessage(t.Nickname & "��[" & fname & "]���g���čU�������킵���B")
					End If
					
					CheckDodgeFeature = True
					Exit Function
				End If
			End If
		End If
		
		'���g(�p�C���b�g�p����\��)
		If t.MainPilot.IsSkillAvailable("���g") Then
			prob = 2 * t.MainPilot.SkillLevel("���g") - MainPilot.SkillLevel("���g")
			If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
				prob = 32
			End If
			
			If prob >= Dice(32) Then
				fname = t.MainPilot.SkillName0("���g")
				
				'�������
				If t.IsAnimationDefined("���g", fname) Then
					t.PlayAnimation("���g", fname)
				ElseIf t.IsSpecialEffectDefined("���g", fname) Then 
					t.SpecialEffect("���g", fname)
				ElseIf BattleAnimation Then 
					ShowAnimation("���g����")
				Else
					'�����
					DodgeEffect(Me, w)
				End If
				
				'���b�Z�[�W
				If Not be_quiet Then
					If t.IsMessageDefined("���g(" & fname & ")") Then
						t.PilotMessage("���g(" & fname & ")")
					Else
						t.PilotMessage("���g")
					End If
				End If
				
				If t.IsSysMessageDefined("���g", fname) Then
					t.SysMessage("���g", fname)
				Else
					DisplaySysMessage(t.Nickname & "�͕��g���čU�������킵���B")
				End If
				
				CheckDodgeFeature = True
				Exit Function
			End If
		End If
	End Function
	
	'�؂蕥�������˂̃`�F�b�N
	'(�������ɔ������A��������ΕK���_���[�W��0�ɂȂ�\��)
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
		
		'�X�y�V�����p���[�ŉ��\�͂�����������Ă���H
		If (IsUnderSpecialPowerEffect("��Ζ���") Or IsUnderSpecialPowerEffect("���\�͖�����")) And Not t.IsUnderSpecialPowerEffect("����h�䔭��") Then
			Exit Function
		End If
		
		'�\���h��͍s���ł��Ȃ���Δ������Ȃ�
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("���h��") Then
			Exit Function
		End If
		
		wname = WeaponNickname(w)
		
		'�^�[�Q�b�g�̌}�����x�����`�F�b�N
		slevel = t.SkillLevel("�}��")
		
		'�؂蕥���Ɏg�p���镐��������Ă���H
		'(�����Ă���ΐ؂蕥���̕���D��)
		wname2 = ""
		If t.IsFeatureAvailable("�i������") Then
			wname2 = t.FeatureData("�i������")
		Else
			For i = 1 To t.CountWeapon
				If t.IsWeaponClassifiedAs(i, "��") And t.IsWeaponAvailable(i, "�ړ��O") Then
					wname2 = t.WeaponNickname(i)
					Exit For
				End If
			Next 
		End If
		'���������𖞂����Ă���H
		If IsWeaponClassifiedAs(w, "��") And (slevel > t.MainPilot.SkillLevel("�؂蕥��") Or (slevel > 0 And Len(wname2) = 0)) Then
			'�}�����������
			i = 0
			If t.IsFeatureAvailable("�}������") Then
				For i = 1 To t.CountWeapon
					If t.Weapon(i).Name = t.FeatureData("�}������") Then
						If Not t.IsWeaponAvailable(i, "�ړ��O") Then
							i = 0
						End If
						Exit For
					End If
				Next 
			End If
			If i = 0 Then
				'�}�����킪�Ȃ��ꍇ�͌}���p�̕���Ƃ��Ă̏����𖞂������������
				For i = 1 To t.CountWeapon
					If t.IsWeaponAvailable(i, "�ړ���") And t.IsWeaponClassifiedAs(i, "�ړ���U����") And t.IsWeaponClassifiedAs(i, "�ˌ��n") And (t.Weapon(i).Bullet >= 10 Or (t.Weapon(i).Bullet = 0 And t.Weapon(i).ENConsumption <= 5)) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale Then
						Exit For
					End If
				Next 
			End If
			
			'�}���p���킪�e�؂�A�d�m�s���̏ꍇ�͌}���s��
			If 0 < i And i <= t.CountWeapon Then
				If Not t.IsWeaponAvailable(i, "�X�e�[�^�X") Then
					i = 0
				End If
			End If
			
			'�}�������s
			If 0 < i And i <= t.CountWeapon And (slevel >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��")) Then
				'���b�Z�[�W
				If Not be_quiet Then
					If t.IsMessageDefined("�}��(" & t.Weapon(i).Name & ")") Then
						t.PilotMessage("�}��(" & t.Weapon(i).Name & ")")
					Else
						t.PilotMessage("�}��")
					End If
				Else
					IsWavePlayed = False
				End If
				
				'���ʉ�
				If Not IsWavePlayed Then
					If IsAnimationDefined(wname & "(�}��)") Then
						PlayAnimation(wname & "(�}��)")
					ElseIf IsSpecialEffectDefined(wname & "(�}��)") Then 
						SpecialEffect(wname & "(�}��)")
					ElseIf t.IsAnimationDefined("�}��", fname) Then 
						t.PlayAnimation("�}��", fname)
					ElseIf t.IsSpecialEffectDefined("�}��", fname) Then 
						t.SpecialEffect("�}��", fname)
					ElseIf t.IsSpecialEffectDefined((t.Weapon(i).Name)) Then 
						t.SpecialEffect((t.Weapon(i).Name))
					Else
						AttackEffect(t, i)
					End If
				End If
				
				DisplaySysMessage(t.Nickname & "��[" & t.WeaponNickname(i) & "]��[" & wname & "]��j�~�����B")
				
				'�}�����ꂽ�i������͎g�p�񐔂����炷
				If IsWeaponClassifiedAs(w, "�i") And Weapon(w).Bullet > 0 Then
					SetBullet(w, Bullet(w) - 1)
					SyncBullet()
					IsMapAttackCanceled = True
				End If
				
				'�}������̒e��������
				t.UseWeapon(i)
				
				CheckParryFeature = True
				Exit Function
			End If
		End If
		
		'����������ɂ͑j�~�������Ȃ�
		If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			GoTo SkipBlock
		End If
		
		'�j�~
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�j�~" Then
				fname = t.FeatureName0(i)
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'�j�~�m���̐ݒ�
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'���؂�
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 100
				End If
				
				'�K�����������Ă���Αj�~�͖���
				If IsUnderSpecialPowerEffect("��Ζ���") And Not t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 0
				End If
				
				'�Ώۑ����̔���
				If Not t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					prob = 0
				End If
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If LIndex(fdata, 1) = LIndex(FeatureData("�j�~"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
								prob = 0
							End If
						Case "���a"
							If LIndex(fdata, 1) = LIndex(FeatureData("�j�~"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�j�~")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									prob = 0
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								prob = 0
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								prob = 0
							End If
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'�_���[�W�����e�͈͊O�ł���Αj�~�ł��Ȃ�
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'�U����j�~
				If prob >= Dice(100) Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "����" Then
							UpdateMessageForm(Me, t)
						Else
							UpdateMessageForm(Me, Nothing)
						End If
					End If
					If Not be_quiet Then
						If t.IsMessageDefined("�j�~(" & fname & ")") Then
							t.PilotMessage("�j�~(" & fname & ")")
						Else
							t.PilotMessage("�j�~")
						End If
					End If
					If IsAnimationDefined(wname & "(�j�~)") Then
						PlayAnimation(wname & "(�j�~)")
					ElseIf IsSpecialEffectDefined(wname & "(�j�~)") Then 
						SpecialEffect(wname & "(�j�~)")
					ElseIf t.IsAnimationDefined("�j�~", fname) Then 
						t.PlayAnimation("�j�~", fname)
					Else
						t.SpecialEffect("�j�~", fname)
					End If
					
					If t.IsSysMessageDefined("�j�~", fname) Then
						t.SysMessage("�j�~", fname)
					Else
						DisplaySysMessage(t.Nickname & "��[" & fname & "]��[" & wname & "]��h�����B")
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
		
SkipBlock: 
		
		'�}�b�v�U���△��������ɂ͓��Đg�Z�͌����Ȃ�
		If IsWeaponClassifiedAs(w, "�l") Or IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			GoTo SkipParryAttack
		End If
		
		'���Đg�Z
		For i = 1 To t.CountFeature
			'���󂳂�Ă���H
			If t.Feature(i) = "���Đg�Z" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					fname = "���Đg�Z"
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'���Đg�m���̐ݒ�
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'���؂�
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 100
				End If
				
				'�K�����������Ă���Γ��Đg�Z�͖���
				If IsUnderSpecialPowerEffect("��Ζ���") And Not t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					Exit For
				End If
				
				'�����̔��˂ⓖ�Đg�Z�ɑ΂��ē��Đg�Z�͏o���Ȃ�
				If attack_mode = "����" Or attack_mode = "���Đg�Z" Then
					Exit For
				End If
				
				'�Ώۑ����̔���
				If Not t.IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) Then
					prob = 0
				End If
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("���Đg�Z")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								prob = 0
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("���Đg�Z")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("���Đg�Z")
								If flevel <= 0 Then
									prob = 0
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								prob = 0
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								prob = 0
							End If
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'�_���[�W�����e�͈͊O�ł���Γ��Đg�Z���g���Ȃ�
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'�g�p���铖�Đg�Z������
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To t.CountWeapon
					If t.Weapon(j).Name = wname2 Then
						If t.IsWeaponAvailable(j, "�K�v�Z�\����") Then
							w2 = j
						End If
						Exit For
					End If
				Next 
				
				'���Đg�Z����
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("���Đg�Z(" & fname & ")") Then
							t.PilotMessage("���Đg�Z(" & fname & ")")
						Else
							t.PilotMessage("���Đg�Z")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(���Đg�Z)") Then
							PlayAnimation(wname & "(���Đg�Z)")
						ElseIf IsSpecialEffectDefined(wname & "(���Đg�Z)") Then 
							SpecialEffect(wname & "(���Đg�Z)")
						ElseIf t.IsAnimationDefined("���Đg�Z", fname) Then 
							t.PlayAnimation("���Đg�Z", fname)
						ElseIf t.IsSpecialEffectDefined("���Đg�Z", fname) Then 
							t.SpecialEffect("���Đg�Z", fname)
						ElseIf BattleAnimation Then 
							ShowAnimation("�œ˖���")
						ElseIf IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then 
							PlayWave("Sword.wav")
						Else
							PlayWave("BeamCoat.wav")
						End If
					End If
					
					If t.IsSysMessageDefined("���Đg�Z", fname) Then
						t.SysMessage("���Đg�Z", fname)
					Else
						DisplaySysMessage(t.Nickname & "��[" & fname & "]��[" & wname & "]���󂯎~�߂��B")
					End If
					
					'���Đg�Z�ōU����������
					t.Attack(w2, Me, "���Đg�Z", "")
					t = t.CurrentForm
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
		
SkipParryAttack: 
		
		'�؂蕥���Ɏg�p���镐��𒲂ׂ�
		wname2 = ""
		If t.IsFeatureAvailable("�i������") Then
			wname2 = t.FeatureData("�i������")
		Else
			For i = 1 To t.CountWeapon
				If t.IsWeaponClassifiedAs(i, "��") And Not t.IsWeaponClassifiedAs(i, "��") And t.IsWeaponMastered(i) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale And Not t.IsDisabled((t.Weapon(i).Name)) Then
					wname2 = t.WeaponNickname(i)
					Exit For
				End If
			Next 
		End If
		
		'�؂蕥���o����H
		If t.MainPilot.SkillLevel("�؂蕥��") > 0 And Len(wname2) > 0 Then
			If IsWeaponClassifiedAs(w, "��") Then
				prob = 0
				
				'�v�O�U���͂m�s���x���ɉ����Đ؂蕥���ɂ����Ȃ�
				If IsWeaponClassifiedAs(w, "�T") Then
					prob = t.MainPilot.SkillLevel("�����o") + t.MainPilot.SkillLevel("�m�o����")
					prob = prob - MainPilot.SkillLevel("�����o") - MainPilot.SkillLevel("�m�o����")
					If prob > 0 Then
						prob = 0
					End If
				Else
					prob = 0
				End If
				
				prob = prob + 2 * t.MainPilot.SkillLevel("�؂蕥��")
				
				'���؂肪����ΕK������
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("�؂蕥��(" & wname2 & ")") Then
							t.PilotMessage("�؂蕥��(" & wname2 & ")")
						Else
							t.PilotMessage("�؂蕥��")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(�؂蕥��)") Then
							PlayAnimation(wname & "(�؂蕥��)")
						ElseIf IsSpecialEffectDefined(wname & "(�؂蕥��)") Then 
							SpecialEffect(wname & "(�؂蕥��)")
						ElseIf t.IsAnimationDefined("�؂蕥��", wname2) Then 
							t.PlayAnimation("�؂蕥��", wname2)
						ElseIf t.IsSpecialEffectDefined("�؂蕥��", wname2) Then 
							t.SpecialEffect("�؂蕥��", wname2)
						Else
							ParryEffect(Me, w, t)
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "��[" & wname2 & "]��[" & wname & "]��@�����Ƃ����B")
					
					'�؂蕥��ꂽ�i������͎g�p�񐔂����炷
					If IsWeaponClassifiedAs(w, "�i") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			ElseIf IsWeaponClassifiedAs(w, "��") Then 
				'��������ˑ����������Ă��Ă��؂蕥���̑ΏۊO�ɂȂ�܂�
			ElseIf IsWeaponClassifiedAs(w, "��") Then 
				'������؂蕥���o����ΐ؂蕥���m���͉�����
				prob = 2 * t.MainPilot.SkillLevel("�؂蕥��") - MainPilot.SkillLevel("�؂蕥��")
				
				'���؂肪����ΕK������
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("�؂蕥��(" & wname2 & ")") Then
							t.PilotMessage("�؂蕥��(" & wname2 & ")")
						Else
							t.PilotMessage("�؂蕥��")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(�؂蕥��)") Then
							PlayAnimation(wname & "(�؂蕥��)")
						ElseIf IsSpecialEffectDefined(wname & "(�؂蕥��)") Then 
							SpecialEffect(wname & "(�؂蕥��)")
						ElseIf t.IsAnimationDefined("�؂蕥��", wname2) Then 
							t.PlayAnimation("�؂蕥��", wname2)
						ElseIf t.IsSpecialEffectDefined("�؂蕥��", wname2) Then 
							t.SpecialEffect("�؂蕥��", wname2)
						Else
							DodgeEffect(Me, w)
							Sleep(190)
							PlayWave("Sword.wav")
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "��[" & wname2 & "]��[" & wname & "]���󂯗������B")
					
					'�؂蕥��ꂽ�i������͎g�p�񐔂����炷
					If IsWeaponClassifiedAs(w, "�i") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			ElseIf IsWeaponClassifiedAs(w, "��") Then 
				'������؂蕥���o����ΐ؂蕥���m���͉�����
				prob = 2 * t.MainPilot.SkillLevel("�؂蕥��") - MainPilot.SkillLevel("�؂蕥��")
				
				'���؂肪����ΕK������
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 32
				End If
				
				If prob >= Dice(32) Then
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("�؂蕥��(" & wname2 & ")") Then
							t.PilotMessage("�؂蕥��(" & wname2 & ")")
						Else
							t.PilotMessage("�؂蕥��")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(�؂蕥��)") Then
							PlayAnimation(wname & "(�؂蕥��)")
						ElseIf IsSpecialEffectDefined(wname & "(�؂蕥��)") Then 
							SpecialEffect(wname & "(�؂蕥��)")
						ElseIf t.IsAnimationDefined("�؂蕥��", wname2) Then 
							t.PlayAnimation("�؂蕥��", wname2)
						ElseIf t.IsSpecialEffectDefined("�؂蕥��", wname2) Then 
							t.SpecialEffect("�؂蕥��", wname2)
						Else
							DodgeEffect(Me, w)
							Sleep(190)
							PlayWave("Sword.wav")
						End If
					End If
					
					DisplaySysMessage(t.Nickname & "��[" & wname2 & "]��[" & wname & "]���󂯂Ƃ߂��B")
					
					'�؂蕥��ꂽ�i������͎g�p�񐔂����炷
					If IsWeaponClassifiedAs(w, "�i") And Weapon(w).Bullet > 0 Then
						SetBullet(w, Bullet(w) - 1)
						SyncBullet()
						IsMapAttackCanceled = True
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		End If
		
		'���˖�����
		If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			Exit Function
		End If
		
		'�U�����˂̏���
		For i = 1 To t.CountFeature
			If t.Feature(i) = "����" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("�o���A�V�[���h") Then
						fname = t.FeatureName0("�o���A�V�[���h")
					Else
						fname = "����"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'���ˊm���̐ݒ�
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'���˂��ꂽ�U���𔽎˂���ꍇ�͊m����������
				If attack_mode = "����" Then
					prob = prob \ 2
				End If
				
				'���؂�
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 100
				End If
				
				'���Đg�Z�͔��ˏo���Ȃ�
				If attack_mode = "���Đg�Z" Then
					Exit For
				End If
				
				'�K�����������Ă���Δ��˂͖���
				If IsUnderSpecialPowerEffect("��Ζ���") And Not t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					Exit For
				End If
				
				'�Ώۑ����̔���
				If Not t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					prob = 0
				End If
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If LIndex(fdata, 1) = LIndex(FeatureData("�j�~"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
								prob = 0
							End If
						Case "���a"
							If LIndex(fdata, 1) = LIndex(FeatureData("�j�~"), 1) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�j�~")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									prob = 0
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								prob = 0
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								prob = 0
							End If
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'�_���[�W�����e�͈͊O�ł���Δ��˂ł��Ȃ�
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'�U���𔽎�
				If prob >= Dice(100) Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("����(" & fname & ")") Then
							t.PilotMessage("����(" & fname & ")")
						Else
							t.PilotMessage("����")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If IsAnimationDefined(wname & "(����)") Then
							PlayAnimation(wname & "(����)")
						ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
							SpecialEffect(wname & "(����)")
						ElseIf t.IsAnimationDefined("����", fname) Then 
							t.PlayAnimation("����", fname)
						ElseIf t.IsSpecialEffectDefined("����", fname) Then 
							t.SpecialEffect("����", fname)
						ElseIf BattleAnimation Then 
							If fname = "����" Then
								ShowAnimation("���˔���")
							Else
								ShowAnimation("���˔��� - " & fname)
							End If
						ElseIf IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then 
							PlayWave("Sword.wav")
						Else
							PlayWave("BeamCoat.wav")
						End If
					End If
					
					If t.IsSysMessageDefined("����", fname) Then
						t.SysMessage("����", fname)
					ElseIf fname <> "����" Then 
						DisplaySysMessage(t.Nickname & "��[" & fname & "]��[" & wname & "]��e���Ԃ����B")
					Else
						DisplaySysMessage(t.Nickname & "��[" & wname & "]��e���Ԃ����B")
					End If
					
					'�U���𔽎�
					If Not IsWeaponClassifiedAs(w, "�l") And attack_mode <> "����" Then
						Attack(w, Me, "����", "")
					End If
					
					CheckParryFeature = True
					Exit Function
				End If
			End If
		Next 
	End Function
	
	'�_�~�[�\�͂̃`�F�b�N
	Private Function CheckDummyFeature(ByVal w As Short, ByRef t As Unit, ByVal be_quiet As Boolean) As Boolean
		Dim wname As String
		Dim fname As String
		
		wname = WeaponNickname(w)
		
		If t.IsConditionSatisfied("�_�~�[�t��") Then
			'�������̓������
			IsWavePlayed = False
			If Not be_quiet Then
				PilotMessage(wname & "(����)")
			End If
			If IsAnimationDefined(wname & "(����)") Or IsAnimationDefined(wname) Then
				PlayAnimation(wname & "(����)")
			ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
				SpecialEffect(wname & "(����)")
			ElseIf Not IsWavePlayed Then 
				HitEffect(Me, w, t)
			End If
			
			fname = t.FeatureName("�_�~�[")
			If Len(fname) > 0 Then
				If InStr(fname, "Lv") > 0 Then
					fname = Left(fname, InStr(fname, "Lv") - 1)
				End If
			Else
				fname = "�_�~�["
			End If
			
			If Not be_quiet Then
				If t.IsMessageDefined("�_�~�[(" & fname & ")") Then
					t.PilotMessage("�_�~�[(" & fname & ")")
				Else
					t.PilotMessage("�_�~�[")
				End If
			End If
			If t.IsAnimationDefined("�_�~�[", fname) Then
				t.PlayAnimation("�_�~�[", fname)
			Else
				t.SpecialEffect("�_�~�[", fname)
			End If
			
			If t.IsSysMessageDefined("�_�~�[", fname) Then
				t.SysMessage("�_�~�[", fname)
			Else
				DisplaySysMessage(t.Nickname & "��[" & fname & "]��g����ɂ��čU�������킵���B")
			End If
			
			t.SetConditionLevel("�_�~�[�t��", t.ConditionLevel("�_�~�[�t��") - 1)
			If t.ConditionLevel("�_�~�[�t��") = 0 Then
				t.DeleteCondition("�_�~�[�t��")
			End If
			
			CheckDummyFeature = True
		ElseIf t.IsFeatureAvailable("�_�~�[") Then 
			If t.ConditionLevel("�_�~�[�j��") < t.FeatureLevel("�_�~�[") Then
				'�������̓������
				IsWavePlayed = False
				If Not be_quiet Then
					PilotMessage(wname & "(����)")
				End If
				If IsAnimationDefined(wname & "(����)") Or IsAnimationDefined(wname) Then
					PlayAnimation(wname & "(����)")
				ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
					SpecialEffect(wname & "(����)")
				ElseIf Not IsWavePlayed Then 
					HitEffect(Me, w, t)
				End If
				
				fname = t.FeatureName("�_�~�[")
				If Len(fname) > 0 Then
					If InStr(fname, "Lv") > 0 Then
						fname = Left(fname, InStr(fname, "Lv") - 1)
					End If
				Else
					fname = "�_�~�["
				End If
				
				If Not be_quiet Then
					If t.IsMessageDefined("�_�~�[(" & fname & ")") Then
						t.PilotMessage("�_�~�[(" & fname & ")")
					Else
						t.PilotMessage("�_�~�[")
					End If
				End If
				If t.IsAnimationDefined("�_�~�[", fname) Then
					t.PlayAnimation("�_�~�[", fname)
				Else
					t.SpecialEffect("�_�~�[", fname)
				End If
				
				If IsSysMessageDefined("�_�~�[", fname) Then
					SysMessage("�_�~�[", fname)
				Else
					DisplaySysMessage(t.Nickname & "��[" & fname & "]��g����ɂ��čU�������킵���B")
				End If
				
				If t.IsConditionSatisfied("�_�~�[�j��") Then
					t.SetConditionLevel("�_�~�[�j��", t.ConditionLevel("�_�~�[�j��") + 1)
				Else
					t.AddCondition("�_�~�[�j��", -1, 1)
				End If
				
				CheckDummyFeature = True
			End If
		End If
	End Function
	
	'�V�[���h�h��\�͂̃`�F�b�N
	Private Function CheckShieldFeature(ByVal w As Short, ByRef t As Unit, ByVal dmg As Integer, ByVal be_quiet As Boolean, ByRef use_shield As Boolean, ByRef use_shield_msg As Boolean) As Boolean
		Dim prob As Short
		Dim fname As String
		
		'�_���[�W��0�ȉ��Ȃ�V�[���h�h�䂵�Ă��Ӗ����Ȃ�
		If dmg <= 0 Then
			Exit Function
		End If
		
		'�r�h��Z�\�������Ă���H
		If t.MainPilot.SkillLevel("�r�h��") <= 0 Then
			Exit Function
		End If
		
		'�s���\�H
		If t.IsConditionSatisfied("�s���s�\") Or t.IsConditionSatisfied("���") Or t.IsConditionSatisfied("�Ή�") Or t.IsConditionSatisfied("����") Or t.IsConditionSatisfied("����") Or t.IsUnderSpecialPowerEffect("�s���s�\") Or t.IsUnderSpecialPowerEffect("���h��") Then
			Exit Function
		End If
		
		'�V�[���h�h��o���Ȃ�����H
		If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "�E") Or IsWeaponClassifiedAs(w, "�Z") Then
			Exit Function
		End If
		
		'�X�y�V�����p���[�Ŗ����������H
		If IsUnderSpecialPowerEffect("�V�[���h�h�䖳����") Then
			Exit Function
		End If
		
		'�V�[���h�n�h��\�͂�����
		If t.IsFeatureAvailable("�V�[���h") Then
			prob = t.MainPilot.SkillLevel("�r�h��")
			fname = t.FeatureName("�V�[���h")
		ElseIf t.IsFeatureAvailable("���^�V�[���h") Then 
			prob = t.MainPilot.SkillLevel("�r�h��")
			fname = t.FeatureName("���^�V�[���h")
		ElseIf t.IsFeatureAvailable("�G�l���M�[�V�[���h") And t.EN > 5 And Not IsWeaponClassifiedAs(w, "��") And Not IsUnderSpecialPowerEffect("�h��\�͖�����") Then 
			prob = t.MainPilot.SkillLevel("�r�h��")
			fname = t.FeatureName("�G�l���M�[�V�[���h")
		ElseIf t.IsFeatureAvailable("��^�V�[���h") Then 
			prob = t.MainPilot.SkillLevel("�r�h��") + 1
			fname = t.FeatureName("��^�V�[���h")
		ElseIf t.IsFeatureAvailable("�A�N�e�B�u�V�[���h") Then 
			prob = t.MainPilot.SkillLevel("�r�h��") + 2
			fname = t.FeatureName("�A�N�e�B�u�V�[���h")
		Else
			'�g�p�\�ȃV�[���h�n�h��\�͂���������
			Exit Function
		End If
		
		'�V�[���h�����m���𖞂����Ă���H
		If prob >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��") Then
			use_shield = True
			
			If IsWeaponClassifiedAs(w, "�j") Then
				If t.IsFeatureAvailable("���^�V�[���h") Then
					dmg = (5 * dmg) \ 6
				Else
					dmg = 3 * dmg \ 4
				End If
			Else
				If t.IsFeatureAvailable("���^�V�[���h") Then
					dmg = (2 * dmg) \ 3
				Else
					dmg = dmg \ 2
				End If
			End If
			
			If dmg > 0 And dmg < 10 Then
				dmg = 10
			End If
			
			If dmg < t.HP And Not be_quiet Then
				If t.IsMessageDefined("�V�[���h�h��(" & fname & ")") Then
					t.PilotMessage("�V�[���h�h��(" & fname & ")")
					use_shield_msg = True
				ElseIf t.IsMessageDefined("�V�[���h�h��") Then 
					t.PilotMessage("�V�[���h�h��")
					use_shield_msg = True
				End If
			End If
			
			If t.IsAnimationDefined("�V�[���h�h��", fname) Then
				t.PlayAnimation("�V�[���h�h��", fname)
			ElseIf t.IsSpecialEffectDefined("�V�[���h�h��", fname) Then 
				t.SpecialEffect("�V�[���h�h��", fname)
			Else
				ShieldEffect(t)
			End If
		End If
	End Function
	
	'�o���A�Ȃǂ̖h��\�͂̃`�F�b�N
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
		team = MainPilot.SkillData("�`�[��")
		
		'�U���z��
		If dmg < 0 Then
			t.HP = t.HP - dmg
			
			If attack_mode <> "����" Then
				UpdateMessageForm(Me, t)
			Else
				UpdateMessageForm(Me, Nothing)
			End If
			
			NegateEffect(Me, t, w, wname, dmg, "�z��", "", 0, msg, be_quiet)
			
			CheckDefenseFeature = True
			Exit Function
		End If
		
		'�U��������
		If dmg = 0 And Weapon(w).Power > 0 Then
			If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
				DisplaySysMessage(msg & t.Nickname & "�ɂ�[" & wname & "]�͒ʗp���Ȃ��B")
			Else
				NegateEffect(Me, t, w, wname, dmg, "", "", 0, msg, be_quiet)
			End If
			CheckDefenseFeature = True
			Exit Function
		End If
		
		'������ʂ��Ȃ��ꍇ�ɂ̓N���e�B�J�������̉\��������
		If Not IsNormalWeapon(w) Then
			'������ʂ𔺂�����
			If CriticalProbability(w, t, def_mode) = 0 And Weapon(w).Power = 0 Then
				'�U���͂�0�̍U���́A�N���e�B�J����������0�̏ꍇ������������Ă���ƌ��Ȃ�
				NegateEffect(Me, t, w, wname, dmg, "", "", 0, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
		
		'�o���A������
		If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			GoTo SkipBarrier
		End If
		
		'�L��o���A
		'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		u = Nothing
		flevel = 0
		fid = 0
		'�o���A���͂��Ă���郆�j�b�g��T��
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint
				End If
				
				With MapDataForUnit(i, j)
					'�G�H
					If .IsEnemy(t) Then
						GoTo NextPoint
					End If
					
					'�s���s�\�H
					If .MaxAction = 0 Then
						GoTo NextPoint
					End If
					
					'�n���ɂ���H
					If .Area = "�n��" Then
						GoTo NextPoint
					End If
					
					'�L��o���A�������Ă���H
					If Not .IsFeatureAvailable("�L��o���A") Then
						GoTo NextPoint
					End If
					
					'�����`�[���ɑ����Ă���H
					uteam = .MainPilot.SkillData("�`�[��")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "�L��o���A" Then
							fdata = .FeatureData(k)
							
							'���ʔ͈�
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'�g�p����
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("�o���A����") Then
								'���łɔ����ς�
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'�����\���`�F�b�N
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("�o���A")) And Not .IsConditionSatisfied("�o���A������") Then
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
			'�o���A���͂��Ă���郆�j�b�g������ꍇ
			If fid = 0 Then
				fname = u.FeatureName0("�L��o���A")
				fdata = u.FeatureData("�L��o���A")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("�o���A") Then
					fname = u.FeatureName0("�o���A")
				Else
					fname = "�L��o���A"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("�o���A����") Then
				'�o���A�����̓^�[�����Ɉ�x�̂�
				u.EN = u.EN - ecost
				If u.IsMessageDefined("�o���A����(" & fname & ")") Then
					u.PilotMessage("�o���A����(" & fname & ")")
				Else
					u.PilotMessage("�o���A����")
				End If
				If u.IsAnimationDefined("�o���A����", fname) Then
					u.PlayAnimation("�o���A����", fname)
				Else
					u.SpecialEffect("�o���A����", fname)
				End If
				
				If u.IsSysMessageDefined("�o���A����", fname) Then
					u.SysMessage("�o���A����", fname)
				Else
					DisplaySysMessage(u.Nickname & "��[" & fname & "]�𔭓��������B")
				End If
				
				If fname = "�L��o���A" Or fname = "�o���A" Then
					u.AddCondition("�o���A����", 1)
				Else
					u.AddCondition("�o���A����", 1, 0, fname & "����")
				End If
			End If
			
			If 1000 * flevel >= dmg Then
				NegateEffect(Me, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				msg = msg & wname & "��[" & fname & "]���т����B;"
			End If
		End If
		
		'�L��t�B�[���h
		'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		u = Nothing
		flevel = 0
		fid = 0
		'�t�B�[���h���͂��Ă���郆�j�b�g��T��
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint2
				End If
				
				With MapDataForUnit(i, j)
					'�G�H
					If .IsEnemy(t) Then
						GoTo NextPoint2
					End If
					
					'�s���s�\�H
					If .MaxAction = 0 Then
						GoTo NextPoint2
					End If
					
					'�n���ɂ���H
					If .Area = "�n��" Then
						GoTo NextPoint2
					End If
					
					'�L��t�B�[���h�������Ă���H
					If Not .IsFeatureAvailable("�L��t�B�[���h") Then
						GoTo NextPoint2
					End If
					
					'�����`�[���ɑ����Ă���H
					uteam = .MainPilot.SkillData("�`�[��")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint2
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "�L��t�B�[���h" Then
							fdata = .FeatureData(k)
							
							'���ʔ͈�
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'�g�p����
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("�t�B�[���h����") Then
								'���łɔ����ς�
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'�����\���`�F�b�N
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("�t�B�[���h")) And Not .IsConditionSatisfied("�o���A������") Then
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
			'�t�B�[���h���͂��Ă���郆�j�b�g������ꍇ
			If fid = 0 Then
				fname = u.FeatureName0("�L��t�B�[���h")
				fdata = u.FeatureData("�L��t�B�[���h")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("�t�B�[���h") Then
					fname = u.FeatureName0("�t�B�[���h")
				Else
					fname = "�L��t�B�[���h"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("�t�B�[���h����") Then
				'�t�B�[���h�����̓^�[�����Ɉ�x�̂�
				u.EN = u.EN - ecost
				If u.IsMessageDefined("�t�B�[���h����(" & fname & ")") Then
					u.PilotMessage("�t�B�[���h����(" & fname & ")")
				Else
					u.PilotMessage("�t�B�[���h����")
				End If
				If u.IsAnimationDefined("�t�B�[���h����", fname) Then
					u.PlayAnimation("�t�B�[���h����", fname)
				Else
					u.SpecialEffect("�t�B�[���h����", fname)
				End If
				
				If u.IsSysMessageDefined("�t�B�[���h����", fname) Then
					u.SysMessage("�t�B�[���h����", fname)
				Else
					DisplaySysMessage(u.Nickname & "��[" & fname & "]�𔭓��������B")
				End If
				
				If fname = "�L��t�B�[���h" Or fname = "�t�B�[���h" Then
					u.AddCondition("�t�B�[���h����", 1)
				Else
					u.AddCondition("�t�B�[���h����", 1, 0, fname & "����")
				End If
			End If
			
			If 500 * flevel >= dmg Then
				NegateEffect(Me, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				dmg = dmg - 500 * flevel
				msg = msg & wname & "��[" & fname & "]���т����B;"
			End If
		End If
		
		'�L��v���e�N�V����
		'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		u = Nothing
		flevel = 0
		fid = 0
		'�v���e�N�V�������͂��Ă���郆�j�b�g��T��
		For i = MaxLng(tx - 3, 1) To MinLng(tx + 3, MapWidth)
			For j = MaxLng(ty - 3, 1) To MinLng(ty + 3, MapHeight)
				If MapDataForUnit(i, j) Is Nothing Or System.Math.Abs(tx - i) + System.Math.Abs(ty - j) > 3 Then
					GoTo NextPoint3
				End If
				
				With MapDataForUnit(i, j)
					'�G�H
					If .IsEnemy(t) Then
						GoTo NextPoint3
					End If
					
					'�s���s�\�H
					If .MaxAction = 0 Then
						GoTo NextPoint3
					End If
					
					'�n���ɂ���H
					If .Area = "�n��" Then
						GoTo NextPoint3
					End If
					
					'�L��v���e�N�V�����������Ă���H
					If Not .IsFeatureAvailable("�L��v���e�N�V����") Then
						GoTo NextPoint3
					End If
					
					'�����`�[���ɑ����Ă���H
					uteam = .MainPilot.SkillData("�`�[��")
					If team <> uteam And uteam <> "" Then
						GoTo NextPoint3
					End If
					
					For k = 1 To .CountFeature
						If .Feature(k) = "�L��v���e�N�V����" Then
							fdata = .FeatureData(k)
							
							'���ʔ͈�
							If IsNumeric(LIndex(fdata, 2)) Then
								frange = CShort(LIndex(fdata, 2))
							Else
								frange = 1
							End If
							
							'�g�p����
							If IsNumeric(LIndex(fdata, 4)) Then
								ecost = CShort(LIndex(fdata, 4))
							Else
								ecost = 20 * frange
							End If
							If .IsConditionSatisfied("�v���e�N�V��������") Then
								'���łɔ����ς�
								ecost = 0
							End If
							If IsNumeric(LIndex(fdata, 5)) Then
								nmorale = CShort(LIndex(fdata, 5))
							Else
								nmorale = 0
							End If
							
							'�����\���`�F�b�N
							If .EN >= ecost And .MainPilot.Morale >= nmorale And .IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) <= frange And System.Math.Abs(x - i) + System.Math.Abs(y - j) > frange And (Not MapDataForUnit(i, j) Is t Or Not t.IsFeatureAvailable("�v���e�N�V����")) And Not .IsConditionSatisfied("�o���A������") Then
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
			'�v���e�N�V�������͂��Ă���郆�j�b�g������ꍇ
			If fid = 0 Then
				fname = u.FeatureName0("�L��v���e�N�V����")
				fdata = u.FeatureData("�L��v���e�N�V����")
			Else
				fname = u.FeatureName0(fid)
				fdata = u.FeatureData(fid)
			End If
			If fname = "" Then
				If u.IsFeatureAvailable("�v���e�N�V����") Then
					fname = u.FeatureName0("�v���e�N�V����")
				Else
					fname = "�L��v���e�N�V����"
				End If
			End If
			
			If IsNumeric(LIndex(fdata, 4)) Then
				ecost = CShort(LIndex(fdata, 4))
			Else
				ecost = 20
			End If
			
			If Not u.IsConditionSatisfied("�v���e�N�V��������") Then
				'�v���e�N�V���������̓^�[�����Ɉ�x�̂�
				u.EN = u.EN - ecost
				If u.IsMessageDefined("�v���e�N�V��������(" & fname & ")") Then
					u.PilotMessage("�v���e�N�V��������(" & fname & ")")
				Else
					u.PilotMessage("�v���e�N�V��������")
				End If
				If u.IsAnimationDefined("�v���e�N�V��������", fname) Then
					u.PlayAnimation("�v���e�N�V��������", fname)
				Else
					u.SpecialEffect("�v���e�N�V��������", fname)
				End If
				
				If u.IsSysMessageDefined("�v���e�N�V��������", fname) Then
					u.SysMessage("�v���e�N�V��������", fname)
				Else
					DisplaySysMessage(u.Nickname & "��[" & fname & "]�𔭓��������B")
				End If
				
				If fname = "�L��v���e�N�V����" Or fname = "�v���e�N�V����" Then
					u.AddCondition("�v���e�N�V��������", 1)
				Else
					u.AddCondition("�v���e�N�V��������", 1, 0, fname & "����")
				End If
			End If
			
			dmg = dmg * (10 - flevel) \ 10
			If dmg < 0 Then
				msg = msg & u.Nickname & "���_���[�W���z�������B;"
				u.HP = u.HP - dmg
				CheckDefenseFeature = True
				Exit Function
			ElseIf flevel > 0 Then 
				msg = msg & u.Nickname & "��[" & fname & "]���_���[�W�������������B;"
			End If
		End If
		
		'�o���A�\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�o���A" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("�L��o���A") Then
						fname = t.FeatureName0("�L��o���A")
					Else
						fname = "�o���A"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�K�v����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�o���A")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "��[" & t.FeatureName(i) & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�o���A")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�o���A")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize Then
					'�o���A����
					t.EN = t.EN - ecost
					If dmg <= 1000 * flevel + slevel Then
						If ecost <> 0 Then
							If attack_mode <> "����" Then
								UpdateMessageForm(Me, t)
							Else
								UpdateMessageForm(Me, Nothing)
							End If
						End If
						NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
						CheckDefenseFeature = True
						Exit Function
					ElseIf flevel > 0 Or slevel > 0 Then 
						If InStr(msg, "[" & fname & "]���т���") = 0 Then
							is_penetrated = True
							msg = msg & wname & "��[" & fname & "]���т����B;"
							If t.IsAnimationDefined("�o���A�ђ�", fname) Then
								t.PlayAnimation("�o���A�ђ�", fname)
							Else
								t.SpecialEffect("�o���A�ђ�", fname)
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'�t�B�[���h�\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�t�B�[���h" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("�o���A") Then
						fname = t.FeatureName("�o���A")
					Else
						fname = "�t�B�[���h"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�K�v����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�t�B�[���h")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�t�B�[���h")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�t�B�[���h")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize Then
					'�t�B�[���h����
					t.EN = t.EN - ecost
					If dmg <= 500 * flevel + slevel Then
						If ecost <> 0 Then
							If attack_mode <> "����" Then
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
						If InStr(msg, "[" & fname & "]���т���") = 0 Then
							msg = msg & wname & "��[" & fname & "]���т����B;"
						End If
					End If
				End If
			End If
		Next 
		
		'�v���e�N�V�����\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�v���e�N�V����" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("�o���A") Then
						fname = t.FeatureName("�o���A")
					Else
						fname = "�v���e�N�V����"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�K�v����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�v���e�N�V����")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�v���e�N�V����")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�v���e�N�V����")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And Not neautralize And dmg > 0 Then
					'�v���e�N�V��������
					dmg = dmg * (100 - 10 * flevel - slevel) \ 100
					
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						If attack_mode <> "����" Then
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
							msg = msg & "[" & fname & "]���_���[�W�������������B;"
						End If
					End If
				End If
			End If
		Next 
		
		'�o���A�V�[���h�A�A�N�e�B�u�t�B�[���h�A�A�N�e�B�u�v���e�N�V�����͔\���h��
		If t.MaxAction = 0 Or t.IsUnderSpecialPowerEffect("���h��") Then
			GoTo SkipActiveBarrier
		End If
		
		'�o���A�V�[���h�\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�o���A�V�[���h" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("����") Then
						fname = t.FeatureName0("����")
					Else
						fname = "�o���A�V�[���h"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�o���A�V�[���h")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "��[" & t.FeatureName(i) & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�o���A�V�[���h")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�o���A�V�[���h")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("�r�h��") > 0 And Not neautralize Then
					'�o���A�V�[���h����
					If t.MainPilot.SkillLevel("�r�h��") >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��") Then
						t.EN = t.EN - ecost
						If dmg <= 1000 * flevel + slevel Then
							If ecost <> 0 Then
								If attack_mode <> "����" Then
									UpdateMessageForm(Me, t)
								Else
									UpdateMessageForm(Me, Nothing)
								End If
							End If
							NegateEffect(Me, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet)
							CheckDefenseFeature = True
							Exit Function
						ElseIf flevel > 0 Or slevel > 0 Then 
							If InStr(msg, "[" & fname & "]���т���") = 0 Then
								is_penetrated = True
								msg = msg & wname & "��[" & fname & "]���т����B;"
								If t.IsAnimationDefined("�o���A�ђ�", fname) Then
									t.PlayAnimation("�o���A�ђ�", fname)
								Else
									t.SpecialEffect("�o���A�ђ�", fname)
								End If
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'�A�N�e�B�u�t�B�[���h�\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�A�N�e�B�u�t�B�[���h" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("����") Then
						fname = t.FeatureName0("����")
					Else
						fname = "�A�N�e�B�u�t�B�[���h"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�A�N�e�B�u�t�B�[���h")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "��[" & t.FeatureName(i) & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�A�N�e�B�u�t�B�[���h")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�A�N�e�B�u�t�B�[���h")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("�r�h��") > 0 And Not neautralize Then
					'�A�N�e�B�u�t�B�[���h����
					If t.MainPilot.SkillLevel("�r�h��") >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��") Then
						t.EN = t.EN - ecost
						If dmg <= 500 * flevel + slevel Then
							If ecost <> 0 Then
								If attack_mode <> "����" Then
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
							If InStr(msg, "[" & fname & "]���т���") = 0 Then
								msg = msg & wname & "��[" & fname & "]���т����B;"
							End If
						End If
					End If
				End If
			End If
		Next 
		
		'�A�N�e�B�u�v���e�N�V�����\��
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�A�N�e�B�u�v���e�N�V����" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					If t.IsFeatureAvailable("����") Then
						fname = t.FeatureName0("����")
					Else
						fname = "�A�N�e�B�u�v���e�N�V����"
					End If
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�g�p����
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
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("�A�N�e�B�u�v���e�N�V����")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								msg = msg & Nickname & "��[" & t.FeatureName(i) & "]�𒆘a�����B;"
								neautralize = True
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("�A�N�e�B�u�v���e�N�V����")) And System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
								flevel = flevel - FeatureLevel("�A�N�e�B�u�v���e�N�V����")
								If flevel <= 0 Then
									msg = msg & Nickname & "��[" & fname & "]�𒆘a�����B;"
									neautralize = True
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								neautralize = True
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								neautralize = True
							End If
						Case "�\�͕K�v", "�o���A����������"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									neautralize = True
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									neautralize = True
								End If
							End If
					End Select
				Next 
				
				'�o���A�������Ŗ���������Ă���H
				If t.IsConditionSatisfied("�o���A������") Then
					If InStr(fdata, "�o���A����������") = 0 Then
						neautralize = True
					End If
				End If
				
				'�����\�H
				If t.EN >= ecost And t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) And t.MainPilot.SkillLevel("�r�h��") > 0 And Not neautralize And dmg > 0 Then
					'�A�N�e�B�u�v���e�N�V��������
					If t.MainPilot.SkillLevel("�r�h��") >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��") Then
						dmg = dmg * (100 - 10 * flevel - slevel) \ 100
						
						If ecost <> 0 Then
							t.EN = t.EN - ecost
							If attack_mode <> "����" Then
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
								msg = msg & "[" & fname & "]���_���[�W�������������B;"
							End If
						End If
					End If
				End If
			End If
		Next 
		
SkipActiveBarrier: 
		
		'����̍U�����d�m�ɕϊ�
		For i = 1 To t.CountFeature
			If t.Feature(i) = "�ϊ�" Then
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				
				'�K�v�C��
				If IsNumeric(LIndex(fdata, 3)) Then
					nmorale = CShort(LIndex(fdata, 3))
				Else
					nmorale = 0
				End If
				
				'�����\�H
				If t.MainPilot.Morale >= nmorale And t.IsAttributeClassified(LIndex(fdata, 2), WeaponClass(w)) Then
					t.EN = t.EN + 0.01 * flevel * dmg
				End If
			End If
		Next 
		
		'�΃r�[���p�h��\��
		If IsWeaponClassifiedAs(w, "�a") Then
			'�r�[���z��
			If t.IsFeatureAvailable("�r�[���z��") Then
				fname = t.FeatureName("�r�[���z��")
				t.HP = t.HP + dmg
				NegateEffect(Me, t, w, wname, dmg, fname, "�a", 0, msg, be_quiet)
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
		
SkipBarrier: 
		
		'�U���͂�0�̏ꍇ�͏���Z���𖳎�
		If Weapon(w).Power = 0 Then
			Exit Function
		End If
		
		'���h��
		If t.IsFeatureAvailable("��") And t.MainPilot.IsSkillAvailable("�r�h��") And t.MaxAction > 0 And Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "�Z") And Not IsWeaponClassifiedAs(w, "�E") And Not IsUnderSpecialPowerEffect("�V�[���h�h�䖳����") And Not t.IsUnderSpecialPowerEffect("���h��") And (t.IsConditionSatisfied("���t��") Or t.FeatureLevel("��") > t.ConditionLevel("���_���[�W")) Then
			fname = t.FeatureName0("��")
			
			If Not be_quiet Then
				t.PilotMessage("�V�[���h�h��", fname)
			End If
			
			If t.IsAnimationDefined("�V�[���h�h��", fname) Then
				t.PlayAnimation("�V�[���h�h��", fname)
			ElseIf t.IsSpecialEffectDefined("�V�[���h�h��", fname) Then 
				t.SpecialEffect("�V�[���h�h��", fname)
			Else
				ShowAnimation("�~�h���V�[���h����")
			End If
			
			If IsWeaponClassifiedAs(w, "�j") Then
				dmg = MaxLng(dmg - 50 * (t.MainPilot.SkillLevel("�r�h��") + 4), 0)
			Else
				dmg = MaxLng(dmg - 100 * (t.MainPilot.SkillLevel("�r�h��") + 4), 0)
			End If
			
			If t.IsSysMessageDefined("�V�[���h�h��", fname) Then
				t.SysMessage("�V�[���h�h��", fname)
			ElseIf dmg = 0 Then 
				DisplaySysMessage(t.Nickname & "��[" & fname & "]���g���čU����h�����B")
			Else
				DisplaySysMessage(t.Nickname & "��[" & fname & "]���g���ă_���[�W���y���������B")
			End If
			
			If dmg = 0 Then
				'�U�������Ŋ��S�ɖh�����ꍇ
				
				'�������̓������
				IsWavePlayed = False
				If Not be_quiet Then
					PilotMessage(wname & "(����)")
				End If
				If IsAnimationDefined(wname & "(����)") Or IsAnimationDefined(wname) Then
					PlayAnimation(wname & "(����)")
				ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
					SpecialEffect(wname & "(����)")
				ElseIf Not IsWavePlayed Then 
					HitEffect(Me, w, t)
				End If
				
				CheckDefenseFeature = True
				Exit Function
			Else
				'�U���������ђʂ����ꍇ
				If t.IsConditionSatisfied("���t��") Then
					If IsWeaponClassifiedAs(w, "�j") Then
						t.SetConditionLevel("���t��", t.ConditionLevel("���t��") - 2)
					Else
						t.SetConditionLevel("���t��", t.ConditionLevel("���t��") - 1)
					End If
					If t.ConditionLevel("���t��") <= 0 Then
						t.DeleteCondition("���t��")
					End If
				Else
					If IsWeaponClassifiedAs(w, "�j") Then
						If t.IsConditionSatisfied("���_���[�W") Then
							t.SetConditionLevel("���_���[�W", t.ConditionLevel("���_���[�W") + 2)
						Else
							t.AddCondition("���_���[�W", -1, 2)
						End If
					Else
						If t.IsConditionSatisfied("���_���[�W") Then
							t.SetConditionLevel("���_���[�W", t.ConditionLevel("���_���[�W") + 1)
						Else
							t.AddCondition("���_���[�W", -1, 1)
						End If
					End If
				End If
			End If
		End If
		
		'�Z���\��
		If t.IsFeatureAvailable("�Z��") Then
			'�Z���\�H
			If Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "��") And Not IsWeaponClassifiedAs(w, "��") And (t.FeatureLevel("�Z��") >= Dice(16) Or t.IsUnderSpecialPowerEffect("����h�䔭��")) Then
				'�Z������
				t.HP = t.HP + dmg
				
				If attack_mode <> "����" Then
					UpdateMessageForm(Me, t)
				Else
					UpdateMessageForm(Me, Nothing)
				End If
				
				fname = t.FeatureName("�Z��")
				If Not be_quiet Then
					If t.IsMessageDefined("�U��������(" & fname & ")") Then
						t.PilotMessage("�U��������(" & fname & ")")
					Else
						t.PilotMessage("�U��������")
					End If
				End If
				
				If t.IsAnimationDefined("�U��������", fname) Then
					t.PlayAnimation("�U��������", fname)
				ElseIf t.IsSpecialEffectDefined("�U��������", fname) Then 
					t.SpecialEffect("�U��������", fname)
				Else
					AbsorbEffect(Me, w, t)
				End If
				If IsAnimationDefined(wname & "(�U��������)") Then
					PlayAnimation(wname & "(�U��������)")
				ElseIf IsSpecialEffectDefined(wname & "(�U��������)") Then 
					SpecialEffect(wname & "(�U��������)")
				End If
				
				If t.IsSysMessageDefined("�U��������", fname) Then
					t.SysMessage("�U��������", fname)
				Else
					If IsWeaponClassifiedAs(w, "��") Then
						DisplaySysMessage(msg & t.Nickname & "��[" & wname & "]����荞�񂾁B")
					Else
						DisplaySysMessage(msg & t.Nickname & "��[" & wname & "]�̍U�����z�������B")
					End If
				End If
				
				CheckDefenseFeature = True
				Exit Function
			End If
		End If
	End Function
	
	'���������̃`�F�b�N
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
		
		'�����n�̍U���ɑ΂��Ă͎����������s��Ȃ�
		If attack_mode = "��������" Or attack_mode = "����" Or attack_mode = "���Đg�Z" Then
			Exit Sub
		End If
		
		'�}�b�v�U���A�ԐڍU���A����������ɂ͎��������o���Ȃ�
		If IsWeaponClassifiedAs(w, "�l") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("�h��\�͖�����") Then
			Exit Sub
		End If
		
		'���������̌��ʌ`�Ԃ��ω����ē���\�͐����ς�邱�Ƃ�����̂�For���͎g��Ȃ�
		i = 1
		Do While i <= t.CountFeature
			If t.Feature(i) = "��������" Then
				fname = t.FeatureName0(i)
				If fname = "" Then
					fname = "��������"
				End If
				fdata = t.FeatureData(i)
				flevel = t.FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'���������m���̐ݒ�
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (t.SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = 100 * t.SkillLevel(buf) / 16
				End If
				
				'���؂�
				If t.IsUnderSpecialPowerEffect("����h�䔭��") Then
					prob = 100
				End If
				
				'�Ώۑ����̔���
				If Not t.IsAttributeClassified(LIndex(fdata, 3), WeaponClass(w)) Then
					prob = 0
				End If
				
				'�g�p����
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
				
				'�\���h��͍s���ł��Ȃ���Δ������Ȃ�
				If t.MaxAction = 0 Then
					If InStr(fdata, "���S����") = 0 Then
						prob = 0
					End If
				End If
				
				'�I�v�V����
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
						Case "���E"
							If IsSameCategory(fdata, FeatureData("��������")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								prob = 0
							End If
						Case "���a"
							If IsSameCategory(fdata, FeatureData("��������")) And System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y) = 1 Then
								flevel = flevel - FeatureLevel("��������")
								If flevel <= 0 Then
									prob = 0
								End If
							End If
						Case "�ߐږ���"
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
								prob = 0
							End If
						Case "�蓮"
							If def_mode <> "�h��" Then
								prob = 0
							End If
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							slevel = lv_mod * (t.SyncLevel - 30)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = -30 * lv_mod Then
									prob = 0
								End If
							Else
								If slevel = -30 * lv_mod Then
									slevel = 0
								End If
							End If
						Case "���"
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							slevel = lv_mod * t.PlanaLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "�I�[��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.AuraLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case "���\��"
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.PsychicLevel
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
						Case Else
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							slevel = lv_mod * t.SkillLevel(opt)
							If InStr(fdata, "�\�͕K�v") > 0 Then
								If slevel = 0 Then
									prob = 0
								End If
							End If
					End Select
				Next 
				
				'�_���[�W�����e�͈͊O�ł���Ύ����������g���Ȃ�
				If dmg > 500 * flevel + slevel Then
					prob = 0
				End If
				
				'�g�p���镐�������
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To t.CountWeapon
					If t.Weapon(j).Name = wname2 Then
						If t.IsWeaponAvailable(j, "�K�v�Z�\����") Then
							If IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then
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
				
				'������������
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						t.EN = t.EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'���b�Z�[�W
					If Not be_quiet Then
						If t.IsMessageDefined("��������(" & fname & ")") Then
							t.PilotMessage("��������(" & fname & ")")
						Else
							t.PilotMessage("��������")
						End If
					Else
						IsWavePlayed = False
					End If
					
					'���ʉ�
					If Not IsWavePlayed Then
						If t.IsAnimationDefined("��������", fname) Then
							t.PlayAnimation("��������", fname)
						ElseIf t.IsSpecialEffectDefined("��������", fname) Then 
							t.SpecialEffect("��������", fname)
						End If
					End If
					
					If t.IsSysMessageDefined("��������", fname) Then
						t.SysMessage("��������", fname)
					Else
						DisplaySysMessage(t.Nickname & "��" & t.WeaponNickname(w2) & "�Ŕ��������B")
					End If
					
					'���������ōU����������
					t.Attack(w2, Me, "��������", "")
					t = t.CurrentForm
					If Status_Renamed <> "�o��" Or t.Status_Renamed <> "�o��" Then
						Exit Sub
					End If
				End If
			End If
			i = i + 1
		Loop 
	End Sub
	
	
	'�ǉ��U���̃`�F�b�N
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
		
		'�ǉ��U���̌��ʌ`�Ԃ��ω����ē���\�͐����ς�邱�Ƃ�����̂�For���͎g��Ȃ�
		i = 1
		Do While i <= CountFeature
			If Feature(i) = "�ǉ��U��" Then
				fname = FeatureName0(i)
				If fname = "" Then
					fname = "�ǉ��U��"
				End If
				fdata = FeatureData(i)
				flevel = FeatureLevel(i)
				If flevel = 1 Then
					flevel = 10000
				End If
				
				'�ǉ��U���m���̐ݒ�
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					prob = CShort(buf)
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					j = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					prob = 100 * (SkillLevel(Left(buf, j - 1)) + CShort(Mid(buf, j))) / 16
				Else
					prob = SkillLevel(buf) * 100 / 16
				End If
				
				'�Ώە���̔���
				wtype = LIndex(fdata, 3)
				found = False
				If Left(wtype, 1) = "@" Then
					'���햼�܂��͕K�v�Z�\�ɂ��w��
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
					'�����ɂ��w��
					Select Case wtype
						Case "�S"
							found = True
						Case "��"
							If InStrNotNest(wclass, "��") = 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "����") > 0 Or InStrNotNest(wclass, "���e") > 0 Or InStrNotNest(wclass, "����") > 0 Then
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
				
				'�g�p����
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
				
				'�A���s��
				If InStr(fdata, "�A���s��") > 0 Then
					If attack_count > 0 Or attack_mode = "�ǉ��U��" Then
						prob = 0
					End If
				End If
				
				'����������
				If InStr(fdata, "����������") > 0 Then
					If dmg <= 0 Then
						prob = 0
					End If
				End If
				
				'�g�p���镐�������
				wname2 = LIndex(fdata, 2)
				w2 = 0
				For j = 1 To CountWeapon
					If Weapon(j).Name = wname2 Then
						If IsWeaponAvailable(j, "�K�v�Z�\����") Then
							If IsTargetWithinRange(j, t) Then
								w2 = j
								Exit For
							End If
						End If
					End If
				Next 
				
				'�ǉ��U����������
				If prob >= Dice(100) And w2 > 0 Then
					If ecost <> 0 Then
						EN = EN - ecost
						UpdateMessageForm(Me, t)
					End If
					
					'���b�Z�[�W
					If Not be_quiet Then
						If IsMessageDefined("�ǉ��U��(" & fname & ")") Then
							PilotMessage("�ǉ��U��(" & fname & ")")
						Else
							PilotMessage("�ǉ��U��")
						End If
					End If
					
					'���ʉ�
					If IsAnimationDefined("�ǉ��U��", fname) Then
						PlayAnimation("�ǉ��U��", fname)
					ElseIf IsSpecialEffectDefined("�ǉ��U��", fname) Then 
						SpecialEffect("�ǉ��U��", fname)
					End If
					
					If IsSysMessageDefined("�ǉ��U��", fname) Then
						SysMessage("�ǉ��U��", fname)
					Else
						DisplaySysMessage(Nickname & "�͂����[" & WeaponNickname(w2) & "]�ōU�����������B")
					End If
					
					'�ǉ��U����������
					Attack(w2, t, "�ǉ��U��", def_mode)
					t = t.CurrentForm
					If Status_Renamed <> "�o��" Or t.Status_Renamed <> "�o��" Then
						Exit Sub
					End If
					
					'�ǉ��U�������{�������Ƃ��L�^
					attack_count = attack_count + 1
				End If
			End If
			i = i + 1
		Loop 
	End Sub
	
	'����\�� fdata1 �� fdata2 ���������̂�����
	'�u���a�v�u���E�v�p
	Private Function IsSameCategory(ByRef fdata1 As String, ByRef fdata2 As String) As Boolean
		Dim fc1, fc2 As String
		
		fc1 = LIndex(fdata1, 1)
		'���x���w�������
		If InStr(fc1, "Lv") > 0 Then
			fc1 = Left(fc1, InStr(fc1, "Lv") - 1)
		End If
		
		fc2 = LIndex(fdata2, 1)
		'���x���w�������
		If InStr(fc2, "Lv") > 0 Then
			fc2 = Left(fc2, InStr(fc2, "Lv") - 1)
		End If
		
		If fc1 = fc2 Then
			IsSameCategory = True
		End If
	End Function
	
	'�N���e�B�J���ɂ��������
	Public Function CauseEffect(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef critical_type As String, ByRef def_mode As String, ByVal will_die As Boolean) As Boolean
		Dim i, prob, j As Short
		Dim fname, wname, ch As String
		Dim Skill() As String
		
		wname = WeaponNickname(w)
		
		'������ʔ����m��
		If IsUnderSpecialPowerEffect("������ʔ���") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		If will_die Then
			'���b�Z�[�W���������Ƃ������̂Ŕj�󂪊m�肵�Ă���ꍇ��
			'�ʏ�̓�����ʂ��X�L�b�v
			GoTo SkipNormalEffect
		End If
		
		'�e����ʂ̔����`�F�b�N
		
		'�ߔ��U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.Nickname & "]�̎��R��D�����B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("�s���s�\", WeaponLevel(w, "��"))
				Else
					t.AddCondition("�s���s�\", 2)
				End If
				critical_type = critical_type & " �ߔ�"
				CauseEffect = True
			End If
		End If
		
		'�V���b�N�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�r") And Not t.SpecialEffectImmune("�r") Then
				msg = msg & "[" & t.Nickname & "]���ꎞ�I�ɍs���s�\�ɂ����B;"
				If IsWeaponLevelSpecified(w, "�r") Then
					t.AddCondition("�s���s�\", WeaponLevel(w, "�r"))
				Else
					t.AddCondition("�s���s�\", 1)
				End If
				critical_type = critical_type & " �V���b�N"
				CauseEffect = True
			End If
		End If
		
		'���b�򉻍U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.Nickname & "]��" & Term("���b", t) & "��򉻂������B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("���b��", WeaponLevel(w, "��"), DEFAULT_LEVEL, Term("���b", t) & "��")
				Else
					t.AddCondition("���b��", 10000, DEFAULT_LEVEL, Term("���b", t) & "��")
				End If
				critical_type = critical_type & " ��"
				CauseEffect = True
			End If
		End If
		
		'�o���A���a�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And (t.IsFeatureAvailable("�o���A") Or t.IsFeatureAvailable("�o���A�V�[���h") Or t.IsFeatureAvailable("�L��o���A") Or t.IsFeatureAvailable("�t�B�[���h") Or t.IsFeatureAvailable("�A�N�e�B�u�t�B�[���h") Or t.IsFeatureAvailable("�L��t�B�[���h") Or t.IsFeatureAvailable("�v���e�N�V����") Or t.IsFeatureAvailable("�A�N�e�B�v���e�N�V����") Or t.IsFeatureAvailable("�L��v���e�N�V����")) Then
				fname = "�o���A"
				If t.IsFeatureAvailable("�o���A") And InStr(t.FeatureData("�o���A"), "�o���A����������") = 0 Then
					fname = t.FeatureName0("�o���A")
					If Len(fname) = 0 Then
						fname = "�o���A"
					End If
				ElseIf t.IsFeatureAvailable("�o���A�V�[���h") And InStr(t.FeatureData("�o���A�V�[���h"), "�o���A����������") = 0 Then 
					fname = t.FeatureName0("�o���A�V�[���h")
					If Len(fname) = 0 Then
						fname = "�o���A�V�[���h"
					End If
				ElseIf t.IsFeatureAvailable("�L��o���A") Then 
					fname = t.FeatureName0("�L��o���A")
					If Len(fname) = 0 Then
						fname = "�L��o���A"
					End If
				ElseIf t.IsFeatureAvailable("�t�B�[���h") And InStr(t.FeatureData("�t�B�[���h"), "�o���A����������") = 0 Then 
					fname = t.FeatureName0("�t�B�[���h")
					If Len(fname) = 0 Then
						fname = "�t�B�[���h"
					End If
				ElseIf t.IsFeatureAvailable("�A�N�e�B�u�t�B�[���h") And InStr(t.FeatureData("�A�N�e�B�u�t�B�[���h"), "�o���A����������") = 0 Then 
					fname = t.FeatureName0("�A�N�e�B�u�t�B�[���h")
					If Len(fname) = 0 Then
						fname = "�A�N�e�B�u�t�B�[���h"
					End If
				ElseIf t.IsFeatureAvailable("�L��t�B�[���h") Then 
					fname = t.FeatureName0("�L��t�B�[���h")
					If Len(fname) = 0 Then
						fname = "�L��t�B�[���h"
					End If
				ElseIf t.IsFeatureAvailable("�v���e�N�V����") And InStr(t.FeatureData("�v���e�N�V����"), "�o���A����������") = 0 Then 
					fname = t.FeatureName0("�v���e�N�V����")
					If Len(fname) = 0 Then
						fname = "�v���e�N�V����"
					End If
				ElseIf t.IsFeatureAvailable("�A�N�e�B�u�v���e�N�V����") And InStr(t.FeatureData("�A�N�e�B�u�v���e�N�V����"), "�o���A����������") = 0 Then 
					fname = t.FeatureName0("�A�N�e�B�u�v���e�N�V����")
					If Len(fname) = 0 Then
						fname = "�A�N�e�B�u�v���e�N�V����"
					End If
				ElseIf t.IsFeatureAvailable("�L��v���e�N�V����") Then 
					fname = t.FeatureName0("�L��v���e�N�V����")
					If Len(fname) = 0 Then
						fname = "�L��v���e�N�V����"
					End If
				End If
				msg = msg & "[" & t.Nickname & "]��" & fname & "�𖳌��������B;"
				
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("�o���A������", WeaponLevel(w, "��"))
				Else
					t.AddCondition("�o���A������", 1)
				End If
				critical_type = critical_type & " �o���A���a"
				CauseEffect = True
			End If
		End If
		
		'�Ή��U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And t.BossRank < 0 Then
				msg = msg & "[" & t.Nickname & "]��Ή��������B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("�Ή�", WeaponLevel(w, "��"))
				Else
					t.AddCondition("�Ή�", 10000)
				End If
				critical_type = critical_type & " �Ή�"
				CauseEffect = True
			End If
		End If
		
		'�����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.Nickname & "]�𓀂点���B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("����", WeaponLevel(w, "��"))
				Else
					t.AddCondition("����", 3)
				End If
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		'��ჍU��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�") And Not t.SpecialEffectImmune("�") Then
				msg = msg & "[" & t.Nickname & "]��Ⴢ������B;"
				If IsWeaponLevelSpecified(w, "�") Then
					t.AddCondition("���", WeaponLevel(w, "�"))
				Else
					t.AddCondition("���", 3)
				End If
				critical_type = critical_type & " ���"
				CauseEffect = True
			End If
		End If
		
		'�Ö��U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And Not t.MainPilot.Personality = "�@�B" Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�𖰂点���B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("����", WeaponLevel(w, "��"))
				Else
					t.AddCondition("����", 3)
				End If
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		'�����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�������������B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("����", WeaponLevel(w, "��"))
				Else
					t.AddCondition("����", 3)
				End If
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		If Not t Is Me Then
			'�����U��
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And Not t.IsConditionSatisfied("����") And Not t.IsConditionSatisfied("�߈�") Then
					msg = msg & MainPilot.Nickname & "��[" & t.MainPilot.Nickname & "]�𖣗������B;"
					If IsWeaponLevelSpecified(w, "��") Then
						t.AddCondition("����", WeaponLevel(w, "��"))
					Else
						t.AddCondition("����", 3)
					End If
					If Not t.Master Is Nothing Then
						t.Master.DeleteSlave((t.ID))
					End If
					AddSlave(t)
					t.Master = Me
					t.Mode = MainPilot.ID
					PList.UpdateSupportMod(t)
					critical_type = critical_type & " ����"
					CauseEffect = True
				End If
			End If
			
			'�߈ˍU��
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And Not t.IsConditionSatisfied("�߈�") And t.BossRank < 0 Then
					msg = msg & MainPilot.Nickname & "��[" & t.Nickname & "]�����������B;"
					If t.IsConditionSatisfied("����") Then
						'�߈˂̕��̌��ʂ�D�悷��
						t.DeleteCondition("����")
					End If
					If IsWeaponLevelSpecified(w, "��") Then
						t.AddCondition("�߈�", WeaponLevel(w, "��"))
					Else
						t.AddCondition("�߈�", 10000)
					End If
					If Not t.Master Is Nothing Then
						t.Master.DeleteSlave((t.ID))
					End If
					AddSlave(t)
					t.Master = Me
					PList.UpdateSupportMod(t)
					critical_type = critical_type & " �߈�"
					CauseEffect = True
				End If
			End If
		End If
		
		'�h���U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�h") And Not t.SpecialEffectImmune("�h") Then
				msg = msg & "[" & t.Nickname & "]���h�������B;"
				If IsWeaponLevelSpecified(w, "�h") Then
					t.AddCondition("�h��", WeaponLevel(w, "�h"))
				Else
					t.AddCondition("�h��", 2)
				End If
				critical_type = critical_type & " �h��"
				CauseEffect = True
			End If
		End If
		
		'���|�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & t.MainPilot.Nickname & "�͋��|�Ɋׂ����B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("���|", WeaponLevel(w, "��"))
				Else
					t.AddCondition("���|", 3)
				End If
				critical_type = critical_type & " ���|"
				CauseEffect = True
			End If
		End If
		
		'�ڒׂ��U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�̎��͂�D�����B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("�Ӗ�", WeaponLevel(w, "��"))
				Else
					t.AddCondition("�Ӗ�", 3)
				End If
				critical_type = critical_type & " �Ӗ�"
				CauseEffect = True
			End If
		End If
		
		'�ōU��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & t.Nickname & "�͓ł��󂯂��B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("��", WeaponLevel(w, "��"))
				Else
					t.AddCondition("��", 3)
				End If
				critical_type = critical_type & " ��"
				CauseEffect = True
			End If
		End If
		
		'�U������U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�s") And Not t.SpecialEffectImmune("�s") Then
				msg = msg & "[" & t.Nickname & "]�̍U���\�͂�D�����B;"
				If IsWeaponLevelSpecified(w, "�s") Then
					t.AddCondition("�U���s�\", WeaponLevel(w, "�s"))
				Else
					t.AddCondition("�U���s�\", 1)
				End If
				critical_type = critical_type & " �U���s�\"
				CauseEffect = True
			End If
		End If
		
		'���~�ߍU��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�~") And Not t.SpecialEffectImmune("�~") Then
				msg = msg & "[" & t.Nickname & "]�̓������~�߂��B;"
				If t.Party <> Stage Then
					If IsWeaponLevelSpecified(w, "�~") Then
						t.AddCondition("�ړ��s�\", WeaponLevel(w, "�~") + 1)
					Else
						t.AddCondition("�ړ��s�\", 2)
					End If
				Else
					If IsWeaponLevelSpecified(w, "�~") Then
						t.AddCondition("�ړ��s�\", WeaponLevel(w, "�~"))
					Else
						t.AddCondition("�ړ��s�\", 1)
					End If
				End If
				critical_type = critical_type & " �ړ��s�\"
				CauseEffect = True
			End If
		End If
		
		'���ٍU��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�𒾖ق������B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("����", WeaponLevel(w, "��"))
				Else
					t.AddCondition("����", 3)
				End If
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		'�x�点�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�x") And Not t.SpecialEffectImmune("�x") Then
				msg = msg & "[" & t.Nickname & "]�͓ˑR�x�肾�����B;"
				If IsWeaponLevelSpecified(w, "�x") Then
					t.AddCondition("�x��", WeaponLevel(w, "�x"))
				Else
					t.AddCondition("�x��", 3)
				End If
				critical_type = critical_type & " �x��"
				CauseEffect = True
			End If
		End If
		
		'����m���U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�͋���m�Ɖ������B;"
				If IsWeaponLevelSpecified(w, "��") Then
					t.AddCondition("����m", WeaponLevel(w, "��"))
				Else
					t.AddCondition("����m", 3)
				End If
				critical_type = critical_type & " ����m"
				CauseEffect = True
			End If
		End If
		
		'�]���r���U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�]") And Not t.SpecialEffectImmune("�]") Then
				msg = msg & "[" & t.Nickname & "]�̓]���r�Ɖ������B;"
				If IsWeaponLevelSpecified(w, "�]") Then
					t.AddCondition("�]���r", WeaponLevel(w, "�]"))
				Else
					t.AddCondition("�]���r", 10000)
				End If
				critical_type = critical_type & " �]���r"
				CauseEffect = True
			End If
		End If
		
		'�񕜔\�͑j�Q�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�Q") And Not t.SpecialEffectImmune("�Q") Then
				msg = msg & "[" & t.Nickname & "]�̎��ȉ񕜔\�͕͂�����ꂽ�B;"
				If IsWeaponLevelSpecified(w, "�Q") Then
					t.AddCondition("�񕜕s�\", WeaponLevel(w, "�Q"))
				Else
					t.AddCondition("�񕜕s�\", 10000)
				End If
				critical_type = critical_type & " �񕜕s�\"
				CauseEffect = True
			End If
		End If
		
		'������ʏ����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				For i = 1 To t.CountCondition
					If (InStr(t.Condition(i), "�t��") > 0 Or InStr(t.Condition(i), "����") > 0 Or InStr(t.Condition(i), "�t�o") > 0) And t.Condition(i) <> "�m�[�}�����[�h�t��" And t.ConditionLifetime(i) > 0 Then
						Exit For
					End If
				Next 
				If i <= t.CountCondition Then
					msg = msg & "[" & t.Nickname & "]�ɂ�����ꂽ������ʂ�ł��������B;"
					Do 
						If (InStr(t.Condition(i), "�t��") > 0 Or InStr(t.Condition(i), "����") > 0 Or InStr(t.Condition(i), "�t�o") > 0) And t.Condition(i) <> "�m�[�}�����[�h�t��" And t.ConditionLifetime(i) > 0 Then
							t.DeleteCondition(i)
						Else
							i = i + 1
						End If
					Loop While i <= t.CountCondition
					critical_type = critical_type & " ����"
					CauseEffect = True
				End If
			End If
		End If
		
		'�����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And (Not t.SpecialEffectImmune("��") Or t.Weakness(WeaponClass(w)) Or t.Effective(WeaponClass(w))) And t.BossRank < 0 And (Not IsUnderSpecialPowerEffect("�Ă�����") Or MainPilot.Technique <= t.MainPilot.Technique) And Not t.IsConditionSatisfied("�s���g") Then
				critical_type = critical_type & " ����"
				CauseEffect = True
				Exit Function
			End If
		End If
		
		'���̐鍐
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And t.BossRank < 0 Then
				msg = msg & "[" & t.MainPilot.Nickname & "]�Ɏ��̐鍐�������ꂽ�B;"
				If InStr(WeaponClass(w), "��L") > 0 Then
					If WeaponLevel(w, "��") > 0 Then
						t.AddCondition("���̐鍐", WeaponLevel(w, "��"))
					Else
						t.HP = 1
					End If
				Else
					t.AddCondition("���̐鍐", 1)
				End If
				critical_type = critical_type & " ���̐鍐"
				CauseEffect = True
			End If
		End If
		
		If t.MainPilot.Personality <> "�@�B" Then
			'�C�͌����U��
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "�E") And Not t.SpecialEffectImmune("�E") Then
					msg = msg & "[" & t.MainPilot.Nickname & "]��" & Term("�C��", t) & "��ቺ�������B;"
					If IsWeaponLevelSpecified(w, "�E") Then
						t.IncreaseMorale(-5 * WeaponLevel(w, "�E"))
					Else
						t.IncreaseMorale(-10)
					End If
					critical_type = critical_type & " �E��"
					CauseEffect = True
				End If
			End If
			
			'�C�͋z���U��
			If prob >= Dice(100) Then
				If IsWeaponClassifiedAs(w, "�c") And Not t.SpecialEffectImmune("�c") Then
					msg = msg & MainPilot.Nickname & "��[" & t.MainPilot.Nickname & "]��" & Term("�C��", t) & "���z��������B;"
					If IsWeaponLevelSpecified(w, "�c") Then
						t.IncreaseMorale(-5 * WeaponLevel(w, "�c"))
						IncreaseMorale(2.5 * WeaponLevel(w, "�c"))
					Else
						t.IncreaseMorale(-10)
						IncreaseMorale(5)
					End If
					critical_type = critical_type & " �C�͋z��"
					CauseEffect = True
				End If
			End If
		End If
		
		'�U���͒ቺ�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��U") And Not t.SpecialEffectImmune("��U") Then
				msg = msg & "[" & t.Nickname & "]�̍U���͂�ቺ�������B;"
				If t.IsConditionSatisfied("�U���͂t�o") Then
					t.DeleteCondition("�U���͂t�o")
				Else
					If IsWeaponLevelSpecified(w, "��U") Then
						t.AddCondition("�U���͂c�n�v�m", WeaponLevel(w, "��U"))
					Else
						t.AddCondition("�U���͂c�n�v�m", 3)
					End If
				End If
				critical_type = critical_type & " �U���͂c�n�v�m"
				CauseEffect = True
			End If
		End If
		
		'�h��͒ቺ�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��h") And Not t.SpecialEffectImmune("��h") Then
				msg = msg & "[" & t.Nickname & "]�̖h��͂�ቺ�������B;"
				If t.IsConditionSatisfied("�h��͂t�o") Then
					t.DeleteCondition("�h��͂t�o")
				Else
					If IsWeaponLevelSpecified(w, "��h") Then
						t.AddCondition("�h��͂c�n�v�m", WeaponLevel(w, "��h"))
					Else
						t.AddCondition("�h��͂c�n�v�m", 3)
					End If
				End If
				critical_type = critical_type & " �h��͂c�n�v�m"
				CauseEffect = True
			End If
		End If
		
		'�^�����ቺ�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��^") And Not t.SpecialEffectImmune("��^") Then
				msg = msg & "[" & t.Nickname & "]��" & Term("�^����", t) & "��ቺ�������B;"
				If t.IsConditionSatisfied("�^�����t�o") Then
					t.DeleteCondition("�^�����t�o")
				Else
					If IsWeaponLevelSpecified(w, "��^") Then
						t.AddCondition("�^�����c�n�v�m", WeaponLevel(w, "��^"), DEFAULT_LEVEL, Term("�^����", t) & "�c�n�v�m")
					Else
						t.AddCondition("�^�����c�n�v�m", 3, DEFAULT_LEVEL, Term("�^����", t) & "�c�n�v�m")
					End If
				End If
				critical_type = critical_type & " �^�����c�n�v�m"
				CauseEffect = True
			End If
		End If
		
		'�ړ��͒ቺ�U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "���") And Not t.SpecialEffectImmune("���") Then
				msg = msg & "[" & t.Nickname & "]��" & Term("�ړ���", t) & "��ቺ�������B;"
				If t.IsConditionSatisfied("�ړ��͂t�o") Then
					t.DeleteCondition("�ړ��͂t�o")
				Else
					If IsWeaponLevelSpecified(w, "���") Then
						t.AddCondition("�ړ��͂c�n�v�m", WeaponLevel(w, "���"), DEFAULT_LEVEL, Term("�ړ���", t) & "�c�n�v�m")
					Else
						t.AddCondition("�ړ��͂c�n�v�m", 3, DEFAULT_LEVEL, Term("�ړ���", t) & "�c�n�v�m")
					End If
				End If
				critical_type = critical_type & " �ړ��͂c�n�v�m"
				CauseEffect = True
			End If
		End If
		
		'�g�o�����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.Nickname & "]��" & Term("�g�o", t) & "��"
				If t.BossRank >= 0 Then
					Select Case CShort(WeaponLevel(w, "��"))
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
					Select Case CShort(WeaponLevel(w, "��"))
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
				msg = msg & "�����������B;"
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		'�d�m�����U��
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") Then
				msg = msg & "[" & t.Nickname & "]��" & Term("�d�m", t) & "��"
				If t.BossRank >= 0 Then
					Select Case CShort(WeaponLevel(w, "��"))
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
					Select Case CShort(WeaponLevel(w, "��"))
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
				msg = msg & "�����������B;"
				critical_type = critical_type & " ����"
				CauseEffect = True
			End If
		End If
		
		'��_�t�������i�オ���݂��邾�����[�v�j
		i = InStrNotNest(strWeaponClass(w), "��")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				If Not t.SpecialEffectImmune(ch) Then
					msg = msg & "[" & t.Nickname & "]��[" & ch & "]�����Ɏキ�Ȃ����B;"
					If IsWeaponLevelSpecified(w, "��" & ch) Then
						t.AddCondition(ch & "������_�t��", WeaponLevel(w, "��" & ch))
					Else
						t.AddCondition(ch & "������_�t��", 3)
					End If
					critical_type = critical_type & " " & ch & "������_�t��"
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "��", i + 1)
		Loop 
		
		'�L���t������
		i = InStrNotNest(strWeaponClass(w), "��")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				'���ɑ��肪�w�葮������_�Ƃ��Ď����Ă���ꍇ����
				If Not t.Weakness(ch) And Not t.SpecialEffectImmune(ch) Then
					msg = msg & "[" & t.Nickname & "]��[" & ch & "]�������L���ɂȂ����B;"
					If IsWeaponLevelSpecified(w, "��" & ch) Then
						t.AddCondition(ch & "�����L���t��", WeaponLevel(w, "��" & ch))
					Else
						t.AddCondition(ch & "�����L���t��", 3)
					End If
					critical_type = critical_type & " " & ch & "�����L���t��"
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "��", i + 1)
		Loop 
		
		'�����g�p�֎~�U��
		i = InStrNotNest(strWeaponClass(w), "��")
		Do While i > 0
			ch = Mid(GetClassBundle(strWeaponClass(w), i), 2)
			If prob >= Dice(100) Then
				If Not t.SpecialEffectImmune(ch) Then
					ReDim Skill(0)
					Select Case ch
						Case "�I"
							Skill(0) = "�I�[��"
						Case "��"
							Skill(0) = "���\��"
						Case "�V"
							Skill(0) = "������"
						Case "�T"
							If t.MainPilot.IsSkillAvailable("�����o") And t.MainPilot(0).IsSkillAvailable("�m�o����") Then
								ReDim Skill(1)
								Skill(0) = "�����o"
								Skill(1) = "�m�o����"
							ElseIf t.MainPilot.IsSkillAvailable("�����o") Then 
								Skill(0) = "�����o"
							ElseIf t.MainPilot.IsSkillAvailable("�m�o����") Then 
								Skill(0) = "�m�o����"
							Else
								ReDim Skill(1)
								Skill(0) = "�����o"
								Skill(1) = "�m�o����"
							End If
						Case "��"
							Skill(0) = "���"
						Case "�p"
							Skill(0) = "�p"
						Case "�Z"
							Skill(0) = "�Z"
						Case Else
							Skill(0) = ""
					End Select
					For j = 0 To UBound(Skill)
						If Len(Skill(j)) > 0 Then
							fname = t.MainPilot.SkillName0(Skill(j))
							If fname = "��\��" Then
								fname = Skill(j)
							End If
						Else
							Skill(0) = ch & "����"
							fname = ch & "����"
						End If
						msg = msg & "[" & t.Nickname & "]��" & fname & "���g�p�o���Ȃ��Ȃ����B;"
						If IsWeaponLevelSpecified(w, "��" & ch) Then
							t.AddCondition(Skill(j) & "�g�p�s�\", WeaponLevel(w, "��" & ch))
						Else
							t.AddCondition(Skill(j) & "�g�p�s�\", 3)
						End If
						critical_type = critical_type & " " & Skill(j) & "�g�p�s�\"
					Next 
					CauseEffect = True
				End If
			End If
			i = InStrNotNest(strWeaponClass(w), "��", i + 1)
		Loop 
		
SkipNormalEffect: 
		
		'����ȍ~�̌��ʂ͓G���j�󂳂��ꍇ����������
		
		'����
		Dim prev_money As Integer
		Dim iname As String
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "��") And Not t.SpecialEffectImmune("��") And Not t.IsConditionSatisfied("������҂�") And ((Party = "����" And t.Party0 <> "����") Or (Party <> "����" And t.Party0 = "����")) Then
				If t.Party0 = "����" Then
					'�����̏ꍇ�͕K����������������
					prev_money = Money
					IncrMoney(-t.Value \ 2)
					If Weapon(w).Power > 0 Then
						msg = msg & "[" & t.Nickname & "]��" & Term("����", t) & VB6.Format(prev_money - Money) & "��D�����ꂽ�B;"
					Else
						msg = msg & "[" & t.Nickname & "]��" & Term("����", t) & VB6.Format(prev_money - Money) & "�𓐂܂ꂽ�B;"
					End If
					critical_type = critical_type & " ����"
					CauseEffect = True
				ElseIf Dice(8) = 1 And t.IsFeatureAvailable("���A�A�C�e�����L") Then 
					'���A�A�C�e���𓐂񂾏ꍇ
					iname = t.FeatureData("���A�A�C�e�����L")
					If IDList.IsDefined(iname) Then
						IList.Add(iname)
						If Weapon(w).Power > 0 Then
							msg = msg & "[" & t.Nickname & "]����" & IDList.Item(iname).Nickname & "��D��������B;"
						Else
							msg = msg & "[" & t.Nickname & "]����" & IDList.Item(iname).Nickname & "�𓐂񂾁B;"
						End If
					Else
						ErrorMessage(t.Name & "�̏��L�A�C�e���u" & iname & "�v�̃f�[�^��������܂���")
					End If
					critical_type = critical_type & " ����"
					CauseEffect = True
				ElseIf t.IsFeatureAvailable("�A�C�e�����L") Then 
					'�A�C�e���𓐂񂾏ꍇ
					iname = t.FeatureData("�A�C�e�����L")
					If IDList.IsDefined(iname) Then
						IList.Add(iname)
						If Weapon(w).Power > 0 Then
							msg = msg & "[" & t.Nickname & "]����" & IDList.Item(iname).Nickname & "��D��������B;"
						Else
							msg = msg & "[" & t.Nickname & "]����" & IDList.Item(iname).Nickname & "�𓐂񂾁B;"
						End If
					Else
						ErrorMessage(t.Name & "�̏��L�A�C�e���u" & iname & "�v�̃f�[�^��������܂���")
					End If
					critical_type = critical_type & " ����"
					CauseEffect = True
				ElseIf t.Value > 0 Then 
					'�����𓐂񂾏ꍇ
					IncrMoney(t.Value \ 4)
					If Weapon(w).Power > 0 Then
						msg = msg & "[" & t.Nickname & "]����" & Term("����", t) & VB6.Format(t.Value \ 4) & "��D��������B;"
					Else
						msg = msg & "[" & t.Nickname & "]����" & Term("����", t) & VB6.Format(t.Value \ 4) & "�𓐂񂾁B;"
					End If
					critical_type = critical_type & " ����"
					CauseEffect = True
				End If
				
				'��x���񂾃��j�b�g����͍ēx���ނ��Ƃ͏o���Ȃ�
				If t.Party0 <> "����" Then
					t.AddCondition("������҂�", -1, 0, "��\��")
				End If
			End If
		End If
		
		'���[�j���O
		Dim sname, stype, vname As String
		If prob >= Dice(100) Then
			If IsWeaponClassifiedAs(w, "�K") And t.IsFeatureAvailable("���[�j���O�\�Z") And Party0 = "����" Then
				stype = LIndex(t.FeatureData("���[�j���O�\�Z"), 1)
				Select Case LIndex(t.FeatureData("���[�j���O�\�Z"), 2)
					Case "�\��", ""
						sname = stype
					Case Else
						sname = LIndex(t.FeatureData("���[�j���O�\�Z"), 2)
				End Select
				If Not MainPilot.IsSkillAvailable(stype) Then
					msg = msg & "[" & MainPilot.Nickname & "]�́u" & sname & "�v���K�������B;"
					
					vname = "Ability(" & MainPilot.ID & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
						'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
						GlobalVariableList.Item(vname).StringValue = stype
					Else
						With GlobalVariableList.Item(vname)
							'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item(vname).StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
							'UPGRADE_WARNING: �I�u�W�F�N�g GlobalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
							.StringValue = .StringValue & " " & stype
						End With
					End If
					
					vname = "Ability(" & MainPilot.ID & "," & stype & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
					End If
					
					If LLength(t.FeatureData("���[�j���O�\�Z")) = 1 Then
						SetVariableAsString(vname, "-1 ��\��")
					ElseIf stype <> sname Then 
						SetVariableAsString(vname, "-1 " & sname)
					Else
						SetVariableAsString(vname, "-1")
					End If
					
					critical_type = critical_type & " ���[�j���O"
					CauseEffect = True
				End If
			End If
		End If
	End Function
	
	'������΂��`�F�b�N
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
		
		'������ʖ����H
		If IsWeaponClassifiedAs(w, "��") And t.SpecialEffectImmune("��") Then
			Exit Function
		End If
		If IsWeaponClassifiedAs(w, "�j") And t.SpecialEffectImmune("�j") Then
			Exit Function
		End If
		
		wlevel = MaxLng(WeaponLevel(w, "��"), WeaponLevel(w, "�j"))
		
		'������ʔ����m��
		If IsUnderSpecialPowerEffect("������ʔ���") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'������΂������̎Z�o
		If prob >= Dice(100) Then
			wlevel = wlevel + 1
			is_critical = True
		End If
		
		'������΂��������O�ł���΂����ŏI���
		If wlevel = 0 Then
			Exit Function
		End If
		
		'�T�C�Y�ɂ�鐧��
		If t.Size = "XL" Then
			Exit Function
		End If
		If IsWeaponClassifiedAs(w, "�j") Then
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
		
		'�Œ蕨�͓������Ȃ�
		If t.IsFeatureAvailable("�n�`���j�b�g") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'�������g�͐�����΂��Ȃ�
		If t Is Me Then
			Exit Function
		End If
		
		'������΂��̒��S���W��ݒ�
		If WeaponLevel(w, "�l��") > 0 Then
			sx = SelectedX
			sy = SelectedY
		Else
			sx = x
			sy = y
		End If
		
		'������΂����ꏊ��ݒ�
		tx = t.x
		ty = t.y
		If Not IsWeaponClassifiedAs(w, "�l��") Then
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
			'�l�ڂ̏ꍇ�͉��ɒe����΂��`�ɂȂ�
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
		
		'������΂���̈ʒu�̌v�Z�ƁA�Փ˂̔���
		nx = tx
		ny = ty
		i = 1
		Do While i <= wlevel
			nx = nx + dx
			ny = ny + dy
			
			'������΂��R�X�g�ɒn�`���ʁy���C�z�̕␳��������
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
				If (t.Area = "�n��" And (.Class_Renamed = "��" Or .Class_Renamed = "����" Or .Class_Renamed = "����")) Or (t.Area = "����" And (.Class_Renamed = "��" Or .Class_Renamed = "�[��")) Or t.Area = Class_Renamed Then
					If .IsFeatureAvailable("���C") Then
						i = i + .FeatureLevel("���C")
					End If
				End If
			End With
			
			'�}�b�v�[
			If nx < 1 Or MapWidth < nx Or ny < 1 Or MapHeight < ny Then
				nx = nx - dx
				ny = ny - dy
				Exit Do
			End If
			
			'�i���s�\�H
			If Not t.IsAbleToEnter(nx, ny) Or Not MapDataForUnit(nx, ny) Is Nothing Then
				is_crashed = True
				
				If Not MapDataForUnit(nx, ny) Is Nothing Then
					t2 = MapDataForUnit(nx, ny)
				End If
				
				nx = nx - dx
				ny = ny - dy
				Exit Do
			End If
			
			'��Q������H
			If t.Area <> "��" Then
				If TerrainHasObstacle(nx, ny) Then
					is_crashed = True
				End If
			End If
			
			i = i + 1
		Loop 
		
		'���j�b�g�������ړ�
		If tx <> nx Or ty <> ny Then
			EraseUnitBitmap(tx, ty)
			If IsAnimationDefined("������΂�") Then
				PlayAnimation("������΂�")
			Else
				MoveUnitBitmap(t, tx, ty, nx, ny, 20)
			End If
			t.Jump(nx, ny, False)
		End If
		
		'����
		orig_dmg = dmg
		If is_crashed Then
			dmg = orig_dmg + MaxLng((orig_dmg - t.Armor * t.MainPilot.Morale \ 100) \ 2, 0)
			
			'�Œ�_���[�W
			If def_mode = "�h��" Then
				dmg = MaxLng(dmg, 5)
			Else
				dmg = MaxLng(dmg, 10)
			End If
			
			PlayWave("Crash.wav")
		End If
		
		'�����Y��
		If Not t2 Is Nothing And Not t2 Is t Then
			With t2
				dmg2 = (orig_dmg - .Armor * .MainPilot.Morale \ 100) \ 2
				
				'�Œ�_���[�W
				If dmg2 < 10 Then
					dmg2 = 10
				End If
				
				'���G�̏ꍇ�̓_���[�W���󂯂Ȃ�
				If .IsConditionSatisfied("���G") Then
					dmg2 = 0
				End If
				
				'�e�폈������₱�����Ȃ�̂Ŋ����Y���ł̓��j�b�g��j�󂵂Ȃ�
				If .HP - dmg2 < 10 Then
					dmg2 = .HP - 10
				End If
				
				'�_���[�W�K�p
				If dmg2 > 0 Then
					.HP = .HP - dmg2
				Else
					dmg2 = 0
				End If
				
				'�_���[�W�ʕ\��
				If Not IsOptionDefined("�_���[�W�\������") Or attack_mode = "�}�b�v�U��" Then
					DrawSysString(.x, .y, VB6.Format(dmg2), True)
				End If
				
				'����\�́u�s����v�ɂ��\���`�F�b�N
				If .IsFeatureAvailable("�s����") Then
					If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("�\��") Then
						.AddCondition("�\��", -1)
						.Update()
					End If
				End If
				
				'�_���[�W���󂯂�Ζ��肩�炳�߂�
				If .IsConditionSatisfied("����") And Not IsWeaponClassifiedAs(w, "��") Then
					.DeleteCondition("����")
				End If
			End With
		End If
		
		msg = t.Nickname & "�𐁂���΂����B;" & msg
		If is_critical Then
			msg = "�N���e�B�J���I " & msg
		End If
		
		'������΂��������������Ƃ�`����
		critical_type = critical_type & " ������΂�"
		CheckBlowAttack = True
	End Function
	
	'�����񂹃`�F�b�N
	Public Function CheckDrawAttack(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef def_mode As String, ByRef critical_type As String) As Boolean
		Dim tx, ty As Short
		Dim sx, sy As Short
		Dim nx, ny As Short
		Dim prob As Short
		
		'������ʖ����H
		If t.SpecialEffectImmune("��") Then
			Exit Function
		End If
		
		'���ɗאڂ��Ă���H
		If System.Math.Abs(x - tx) + System.Math.Abs(y - ty) = 1 Then
			Exit Function
		End If
		
		'�T�C�Y�ɂ�鐧��
		If t.Size = "XL" Then
			Exit Function
		End If
		
		'�Œ蕨�͓������Ȃ�
		If t.IsFeatureAvailable("�n�`���j�b�g") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'�������g�͈����񂹂Ȃ�
		If t Is Me Then
			Exit Function
		End If
		
		'������ʔ����m��
		If IsUnderSpecialPowerEffect("������ʔ���") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'�����񂹔����H
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'�����񂹂̒��S���W��ݒ�
		If WeaponLevel(w, "�l��") > 0 Then
			sx = SelectedX
			sy = SelectedY
		Else
			sx = x
			sy = y
		End If
		
		'�^�[�Q�b�g�̍��W
		tx = t.x
		ty = t.y
		
		'���Ɉ����񂹂̒��S�ʒu�ɂ���H
		If sx = tx And sy = ty Then
			Exit Function
		End If
		
		'�����񂹂���ꏊ��ݒ�
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
		
		'���Ǔ����ĂȂ��H
		If nx = tx And ny = ty Then
			Exit Function
		End If
		
		'���j�b�g�������ړ�
		t.Jump(nx, ny)
		
		'�{���ɓ������H
		If t.x = tx And t.y = ty Then
			Exit Function
		End If
		
		msg = t.Nickname & "�������񂹂��B;" & msg
		
		'�����񂹂������������Ƃ�`����
		critical_type = critical_type & " ������"
		CheckDrawAttack = True
	End Function
	
	'�����]�ڃ`�F�b�N
	Public Function CheckTeleportAwayAttack(ByVal w As Short, ByRef t As Unit, ByRef msg As String, ByRef def_mode As String, ByRef critical_type As String) As Boolean
		Dim tx, ty As Short
		Dim nx, ny As Short
		Dim d, prob, i As Short
		
		'������ʖ����H
		If t.SpecialEffectImmune("�]") Then
			Exit Function
		End If
		
		'�T�C�Y�ɂ�鐧��
		If t.Size = "XL" Then
			Exit Function
		End If
		
		'�Œ蕨�͓������Ȃ�
		If t.IsFeatureAvailable("�n�`���j�b�g") Then
			Exit Function
		End If
		If t.Data.Speed = 0 And t.Speed = 0 Then
			Exit Function
		End If
		
		'�������g�͋����]�ڏo���Ȃ�
		If t Is Me Then
			Exit Function
		End If
		
		'������ʔ����m��
		If IsUnderSpecialPowerEffect("������ʔ���") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'�����]�ڔ����H
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'�����]�ڐ��ݒ�
		tx = t.x
		ty = t.y
		For i = 1 To 10
			d = Dice(WeaponLevel(w, "�]"))
			If Dice(2) = 1 Then
				nx = tx + d
			Else
				nx = tx - d
			End If
			
			d = WeaponLevel(w, "�]") - d
			If Dice(2) = 1 Then
				ny = ty + d
			Else
				ny = ty - d
			End If
			
			If 1 <= nx And nx <= MapWidth And 1 <= ny And ny <= MapHeight Then
				Exit For
			End If
		Next 
		
		'�]�@�悪�Ȃ��H
		If i > 10 Then
			Exit Function
		End If
		
		'���j�b�g�������ړ�
		t.Jump(nx, ny)
		
		'�{���ɓ������H
		If t.x = tx And t.y = ty Then
			Exit Function
		End If
		
		msg = t.Nickname & "���e���|�[�g�������B;" & msg
		
		'�����]�ڂ������������Ƃ�`����
		critical_type = critical_type & " �����]��"
		CheckTeleportAwayAttack = True
	End Function
	
	'�\�̓R�s�[�`�F�b�N
	Public Function CheckMetamorphAttack(ByVal w As Short, ByRef t As Unit, ByRef def_mode As String) As Boolean
		Dim prob, wlv As Short
		Dim uname As String
		
		'���ɃR�s�[�ς݁H
		If IsFeatureAvailable("�m�[�}�����[�h") Then
			Exit Function
		End If
		
		'������ʖ����H
		If t.SpecialEffectImmune("��") Then
			Exit Function
		End If
		
		'�{�X���j�b�g�̓R�s�[�o���Ȃ�
		If t.BossRank >= 0 Then
			Exit Function
		End If
		
		'�������g�̓R�s�[�o���Ȃ�
		If t Is Me Then
			Exit Function
		End If
		
		'�T�C�Y����
		If IsWeaponClassifiedAs(w, "��") Then
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
		
		'������ʔ����m��
		If IsUnderSpecialPowerEffect("������ʔ���") Then
			prob = 100
		Else
			prob = CriticalProbability(w, t, def_mode)
		End If
		
		'�R�s�[�����H
		If Dice(100) > prob Then
			Exit Function
		End If
		
		'�R�s�[���Ă��܂��Ƃ��̏�ɂ���Ȃ��Ȃ��Ă��܂��H
		If Not OtherForm((t.Name)).IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'�ϐg�O�ɏ����L�^���Ă���
		uname = Nickname
		wlv = MaxLng(WeaponLevel(w, "��"), WeaponLevel(w, "��"))
		
		'�ϐg
		Transform((t.Name))
		
		With CurrentForm
			'���ɖ߂��悤�ɐݒ�
			If wlv > 0 Then
				.AddCondition("�c�莞��", wlv)
			End If
			.AddCondition("�m�[�}�����[�h�t��", -1, 1, Name & " �蓮������")
			.AddCondition("�\�̓R�s�[", -1)
			
			'�R�s�[���̃p�C���b�g�摜�ƃ��b�Z�[�W���g���悤�ɐݒ�
			.AddCondition("�p�C���b�g�摜", -1, 0, "��\�� " & t.MainPilot.Bitmap)
			.AddCondition("���b�Z�[�W", -1, 0, "��\�� " & t.MainPilot.MessageType)
		End With
		
		DisplaySysMessage(uname & "��" & t.Nickname & "�ɕϐg�����B")
		
		'�\�̓R�s�[�������������Ƃ�`����
		CheckMetamorphAttack = True
	End Function
	
	
	'�}�b�v�U�� w �� (tx,ty) ���U��
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
		
		'���ʔ͈͂�ݒ�
		min_range = Weapon(w).MinRange
		max_range = WeaponMaxRange(w)
		If IsWeaponClassifiedAs(w, "�l��") Then
			If ty < y Then
				AreaInLine(x, y, min_range, max_range, "N")
			ElseIf ty > y Then 
				AreaInLine(x, y, min_range, max_range, "S")
			ElseIf tx < x Then 
				AreaInLine(x, y, min_range, max_range, "W")
			Else
				AreaInLine(x, y, min_range, max_range, "E")
			End If
		ElseIf IsWeaponClassifiedAs(w, "�l�g") Then 
			If ty < y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then
				AreaInCone(x, y, min_range, max_range, "N")
			ElseIf ty > y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then 
				AreaInCone(x, y, min_range, max_range, "S")
			ElseIf tx < x And System.Math.Abs(x - tx) > System.Math.Abs(y - ty) Then 
				AreaInCone(x, y, min_range, max_range, "W")
			Else
				AreaInCone(x, y, min_range, max_range, "E")
			End If
		ElseIf IsWeaponClassifiedAs(w, "�l��") Then 
			If ty < y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then
				AreaInSector(x, y, min_range, max_range, "N", WeaponLevel(w, "�l��"))
			ElseIf ty > y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then 
				AreaInSector(x, y, min_range, max_range, "S", WeaponLevel(w, "�l��"))
			ElseIf tx < x And System.Math.Abs(x - tx) >= System.Math.Abs(y - ty) Then 
				AreaInSector(x, y, min_range, max_range, "W", WeaponLevel(w, "�l��"))
			Else
				AreaInSector(x, y, min_range, max_range, "E", WeaponLevel(w, "�l��"))
			End If
		ElseIf IsWeaponClassifiedAs(w, "�l��") Then 
			AreaInRange(tx, ty, WeaponLevel(w, "�l��"), 1, "���ׂ�")
		ElseIf IsWeaponClassifiedAs(w, "�l�S") Then 
			AreaInRange(x, y, max_range, min_range, "���ׂ�")
		ElseIf IsWeaponClassifiedAs(w, "�l��") Or IsWeaponClassifiedAs(w, "�l��") Then 
			AreaInPointToPoint(x, y, tx, ty)
		End If
		MaskData(x, y) = False
		
		'���ʌ^�}�b�v�U��
		If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("���ʍU��") Then
			For	Each u In UList
				With u
					If .Status_Renamed = "�o��" Then
						If IsAlly(u) Or WeaponAdaption(w, .Area) = 0 Then
							MaskData(.x, .y) = True
						End If
					End If
				End With
			Next u
			MaskData(x, y) = False
		End If
		
		'���̋Z�̏���
		Dim TmpMaskData() As Boolean
		If IsWeaponClassifiedAs(w, "��") Then
			
			'���̋Z�̃p�[�g�i�[�̃n�C���C�g�\��
			'MaskData��ۑ����Ďg�p���Ă���
			ReDim TmpMaskData(MapWidth, MapHeight)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					TmpMaskData(i, j) = MaskData(i, j)
				Next 
			Next 
			
			CombinationPartner("����", w, partners)
			
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
		
		'�������g�ɂ͍U�����Ȃ�
		MaskData(x, y) = True
		
		'�}�b�v�U���̉e�����󂯂郆�j�b�g�̃��X�g���쐬
		ReDim targets(0)
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'�}�b�v�U���̉e���������邩�`�F�b�N
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
					If IsWeaponClassifiedAs(w, "��") Or IsUnderSpecialPowerEffect("���ʍU��") Then
						If IsAlly(t) Then
							GoTo NextLoop
						End If
					End If
					If .IsUnderSpecialPowerEffect("�B��g") Then
						GoTo NextLoop
					End If
					
					ReDim Preserve targets(UBound(targets) + 1)
					targets(UBound(targets)) = t
				End With
NextLoop: 
			Next 
		Next 
		
		'�U���̋N�_��ݒ�
		If IsWeaponClassifiedAs(w, "�l��") Then
			rx = tx
			ry = ty
		Else
			rx = x
			ry = y
		End If
		
		'�N�_����̋����ɉ����ĕ��בւ�
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
		
		'�퓬�O�Ɉ�U�N���A
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportGuardUnit2 = Nothing
		
		'�C�x���g�̏���
		If Not is_event Then
			'�g�p�C�x���g
			HandleEvent("�g�p", MainPilot.ID, wname)
			If IsScenarioFinished Then
				IsScenarioFinished = False
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				Exit Sub
			End If
			
			'�}�b�v�U���J�n�O�ɂ��炩���ߍU���C�x���g�𔭐�������
			For i = 1 To UBound(targets)
				t = targets(i)
				SaveSelections()
				SelectedTarget = t
				HandleEvent("�U��", MainPilot.ID, t.MainPilot.ID)
				RestoreSelections()
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
		End If
		
		'�܂��U���\�H
		If Not is_event Then
			If Status_Renamed <> "�o��" Or MaxAction(True) = 0 Or IsConditionSatisfied("�U���s�\") Then
				Exit Sub
			End If
		End If
		
		'�^�[�Q�b�g�Ɋւ�������L�^
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
		
		'���݂̑I���󋵂��Z�[�u
		SaveSelections()
		
		'�I����e��؂�ւ�
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedWeapon = w
		SelectedX = tx
		SelectedY = ty
		
		'�ςȁu�΁`�v���b�Z�[�W���\������Ȃ��悤�Ƀ^�[�Q�b�g���I�t
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		
		'�U�������̌��ʉ�
		If IsAnimationDefined(wname & "(����)") Then
			PlayAnimation(wname & "(����)")
		ElseIf IsAnimationDefined(wname) And Not IsOptionDefined("���폀���A�j����\��") And WeaponAnimation Then 
			PlayAnimation(wname & "(����)")
		ElseIf IsSpecialEffectDefined(wname & "(����)") Then 
			SpecialEffect(wname & "(����)")
		End If
		
		'�}�b�v�U���U���J�n�̃��b�Z�[�W
		If IsMessageDefined(wname) Then
			If IsMessageDefined("������(" & wname & ")") Then
				PilotMessage("������(" & wname & ")")
			Else
				PilotMessage("������")
			End If
		End If
		
		'�U�����b�Z�[�W
		PilotMessage(wname, "�U��")
		
		'�퓬�A�j�� or ���ʉ�
		If IsAnimationDefined(wname & "(�U��)") Or IsAnimationDefined(wname) Then
			PlayAnimation(wname & "(�U��)")
		ElseIf IsSpecialEffectDefined(wname) Then 
			SpecialEffect(wname)
		Else
			AttackEffect(Me, w)
		End If
		
		'�U�����̍U���͕ϓ�������邽�߁A���炩���ߍU���͂�ۑ����Ă���
		SelectedMapAttackPower = 0
		SelectedMapAttackPower = WeaponPower(w, "�����l")
		
		'�u�i�v�������킪�j�󂳂�邱�Ƃɂ��}�b�v�U���L�����Z�������̏�����
		IsMapAttackCanceled = False
		
		'����g�p�ɂ��e�����d�m����
		UseWeapon(w)
		UpdateMessageForm(Me)
		
		'�U�����̃V�X�e�����b�Z�[�W
		If IsSysMessageDefined(wname) Then
			'�u���햼(���)�v�̃��b�Z�[�W���g�p
			SysMessage(wname)
		ElseIf IsSysMessageDefined("�U��") Then 
			'�u�U��(���)�v�̃��b�Z�[�W���g�p
			SysMessage(wname)
		Else
			Select Case UBound(partners)
				Case 0
					'�ʏ�U��
					msg = Nickname & "��"
				Case 1
					'�Q�̍��̍U��
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "��[" & partners(1).MainPilot.Nickname & "]��[" & Nickname & "]��"
					Else
						msg = Nickname & "�B��"
					End If
				Case 2
					'�R�̍��̍U��
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�A[" & partners(2).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "�A[" & partners(1).MainPilot.Nickname & "]�A[" & partners(2).MainPilot.Nickname & "]��[" & Nickname & "]��"
					Else
						msg = Nickname & "�B��"
					End If
				Case Else
					'�R�̈ȏ�ɂ�鍇�̍U��
					msg = Nickname & "�B��"
			End Select
			
			'�U���̎�ނɂ���ă��b�Z�[�W��؂�ւ�
			If Right(wnickname, 2) = "�U��" Or Right(wnickname, 4) = "�A�^�b�N" Or wnickname = "�ˌ�" Then
				msg = msg & "[" & wnickname & "]���������B"
			ElseIf IsSpellWeapon(w) Then 
				If Right(wnickname, 2) = "����" Then
					msg = msg & "[" & wnickname & "]���������B"
				ElseIf Right(wnickname, 2) = "�̏�" Then 
					msg = msg & "[" & Left(wnickname, Len(wnickname) - 2) & "]�̎������������B"
				Else
					msg = msg & "[" & wnickname & "]�̎������������B"
				End If
			ElseIf IsWeaponClassifiedAs(w, "��") And (InStr(wnickname, "�~�T�C��") > 0 Or InStr(wnickname, "���P�b�g") > 0) Then 
				msg = msg & "[" & wnickname & "]�𔭎˂����B"
			ElseIf Right(wnickname, 1) = "��" Or Right(wnickname, 3) = "�u���X" Or Right(wnickname, 2) = "����" Or Right(wnickname, 1) = "��" Or Right(wnickname, 3) = "�r�[��" Or Right(wnickname, 4) = "���[�U�[" Then 
				msg = msg & "[" & wnickname & "]��������B"
			ElseIf Right(wnickname, 1) = "��" Then 
				msg = msg & "[" & wnickname & "]���̂����B"
			ElseIf Right(wnickname, 2) = "�x��" Then 
				msg = msg & "[" & wnickname & "]��x�����B"
			Else
				msg = msg & "[" & wnickname & "]�ōU�����������B"
			End If
			
			'���b�Z�[�W��\��
			DisplaySysMessage(msg)
		End If
		
		'�����チ�b�Z�[�W
		PilotMessage(wname & "(����)")
		
		'�I���󋵂𕜌�
		RestoreSelections()
		
		'�o���l�������l���̃��b�Z�[�W�\���p�Ɋe��f�[�^��ۑ�
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
				prev_slevel(i) = .SkillLevel(i, "��{�l")
				prev_sname(i) = .SkillName(i)
			Next 
		End With
		prev_money = Money
		
		'�U�������j�b�g�� SelectedTarget �ɐݒ肵�Ă��Ȃ��Ƃ����Ȃ�
		SelectedTarget = Me
		
		'�ړ��^�}�b�v�U���ɂ��ړ����s��
		If IsWeaponClassifiedAs(w, "�l��") Then
			Jump(tx, ty)
		End If
		
		'�X�̃��j�b�g�ɑ΂��čU��
		For i = 1 To UBound(targets)
			t = targets(i).CurrentForm
			If t.Status_Renamed = "�o��" Then
				If Party = "����" Or Party = "�m�o�b" Then
					UpdateMessageForm(t, Me)
				Else
					UpdateMessageForm(Me, t)
				End If
				
				'�U�����s��
				Attack(w, t, "�}�b�v�U��", "", is_event)
				
				'���΂��ɂ��^�[�Q�b�g���ω����Ă���H
				If Not SupportGuardUnit Is Nothing Then
					targets(i) = SupportGuardUnit.CurrentForm
					targets_hp_ratio(i) = SupportGuardUnitHPRatio
					targets_x(i) = targets(i).x
					targets_y(i) = targets(i).y
				End If
				
				'����ȏ�U���𑱂����Ȃ��ꍇ
				If Status_Renamed <> "�o��" Or CountPilot = 0 Or IsMapAttackCanceled Then
					CloseMessageForm()
					SelectedMapAttackPower = 0
					GoTo DoEvent
				End If
				
				ClearMessageForm()
			End If
		Next 
		
		'�Ƃǂ߃��b�Z�[�W
		If IsMessageDefined(wname & "(�Ƃǂ�)") Then
			PilotMessage(wname & "(�Ƃǂ�)")
		End If
		If IsAnimationDefined(wname & "(�Ƃǂ�)") Then
			PlayAnimation(wname & "(�Ƃǂ�)")
		ElseIf IsSpecialEffectDefined(wname & "(�Ƃǂ�)") Then 
			SpecialEffect(wname & "(�Ƃǂ�)")
		End If
		
		'�J�b�g�C���͏������Ă���
		If IsPictureVisible Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
		
		'�ۑ������U���͂̎g�p���~�߂�
		SelectedMapAttackPower = 0
		
		' ADD START MARGE
		'�퓬�A�j���I������
		If IsAnimationDefined(wname & "(�I��)") Then
			PlayAnimation(wname & "(�I��)")
		ElseIf IsAnimationDefined("�I��") Then 
			PlayAnimation("�I��")
		End If
		' ADD END MARGE
		
		'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
		If IsConditionSatisfied("���j�b�g�摜") Then
			DeleteCondition("���j�b�g�摜")
			BitmapID = MakeUnitBitmap(Me)
			If IsPictureVisible Then
				PaintUnitBitmap(Me, "���t���b�V������")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		If IsConditionSatisfied("��\���t��") Then
			DeleteCondition("��\���t��")
			BitmapID = MakeUnitBitmap(Me)
			If IsPictureVisible Then
				PaintUnitBitmap(Me, "���t���b�V������")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("���j�b�g�摜") Then
					.DeleteCondition("���j�b�g�摜")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("��\���t��") Then
					.DeleteCondition("��\���t��")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
		
		If Party = "����" And Not is_event Then
			'�o���l�������̊l��
			For i = 1 To UBound(targets)
				t = targets(i).CurrentForm
				If Not IsEnemy(t) Then
					'��������͌o���l�������͓����Ȃ�
				ElseIf t.Status_Renamed = "�j��" Then 
					'�o���l�̊l��
					earned_exp = earned_exp + GetExp(t, "�j��", "�}�b�v")
					
					'���̋Z�̃p�[�g�i�[�ւ̌o���l
					If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
						For j = 1 To UBound(partners)
							partners(j).CurrentForm.GetExp(t, "�j��", "�p�[�g�i�[")
						Next 
					End If
					
					'�l�����鎑�����Z�o
					earnings = t.Value \ 2
					
					'�X�y�V�����p���[�ɂ��l����������
					If IsUnderSpecialPowerEffect("�l����������") Then
						earnings = MinDbl(earnings * (1 + 0.1 * SpecialPowerEffectLevel("�l����������")), 999999999)
					End If
					
					'�p�C���b�g�\�͂ɂ��l����������
					If IsSkillAvailable("�����l��") Then
						If Not IsUnderSpecialPowerEffect("�l����������") Or IsOptionDefined("�������ʏd��") Then
							earnings = MinDbl(earnings * ((10 + SkillLevel("�����l��", 5)) / 10), 999999999)
						End If
					End If
					
					'�������l��
					IncrMoney(earnings)
				Else
					'�o���l�̊l��
					earned_exp = earned_exp + GetExp(t, "�U��", "�}�b�v")
					
					'���̋Z�̃p�[�g�i�[�ւ̌o���l
					If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
						For j = 1 To UBound(partners)
							partners(j).CurrentForm.GetExp(t, "�U��", "�p�[�g�i�[")
						Next 
					End If
				End If
			Next 
			
			'�l�������o���l�������̕\��
			If Money > prev_money Then
				DisplaySysMessage(VB6.Format(Money - prev_money) & "��" & Term("����", t) & "�𓾂��B")
			End If
			With MainPilot
				'���x���A�b�v�̏���
				If .Level > prev_level Then
					If IsAnimationDefined("���x���A�b�v") Then
						PlayAnimation("���x���A�b�v")
					ElseIf IsSpecialEffectDefined("���x���A�b�v") Then 
						SpecialEffect("���x���A�b�v")
					End If
					If IsMessageDefined("���x���A�b�v") Then
						PilotMessage("���x���A�b�v")
					End If
					
					msg = .Nickname & "��" & VB6.Format(earned_exp) & "�̌o���l���l�����A" & "���x��[" & VB6.Format(.Level) & "]�Ƀ��x���A�b�v�B"
					
					'����\�͂̏K��
					For i = 1 To .CountSkill
						sname = .Skill(i)
						If InStr(.SkillName(i), "��\��") = 0 Then
							Select Case sname
								Case "������", "���", "�ǉ����x��", "���͏��L"
								Case Else
									For j = 1 To UBound(prev_stype)
										If sname = prev_stype(j) Then
											Exit For
										End If
									Next 
									If j > UBound(prev_stype) Then
										msg = msg & ";" & .SkillName(i) & "���K���B"
									ElseIf .SkillLevel(sname, "��{�l") > prev_slevel(j) Then 
										msg = msg & ";" & prev_sname(j) & " => " & .SkillName(i) & "�B"
									End If
							End Select
						End If
					Next 
					
					'�X�y�V�����p���[�̏K��
					If .CountSpecialPower > UBound(prev_special_power) Then
						msg = msg & ";�X�y�V�����p���["
						For i = 1 To .CountSpecialPower
							sname = .SpecialPower(i)
							For j = 1 To UBound(prev_special_power)
								If sname = prev_special_power(j) Then
									Exit For
								End If
							Next 
							If j > UBound(prev_special_power) Then
								msg = msg & "�u" & sname & "�v"
							End If
						Next 
						msg = msg & "���K���B"
					End If
					
					DisplaySysMessage(msg)
					
					HandleEvent("���x���A�b�v", .ID)
					
					PList.UpdateSupportMod(Me)
				Else
					If earned_exp > 0 Then
						DisplaySysMessage(.Nickname & "��" & VB6.Format(earned_exp) & "�̌o���l�𓾂��B")
					End If
				End If
			End With
		End If
		
		'�X�y�V�����p���[���ʂ̉���
		If IsUnderSpecialPowerEffect("�U�������") Then
			AddCondition("����", 1)
		End If
		RemoveSpecialPowerInEffect("�U��")
		RemoveSpecialPowerInEffect("�퓬�I��")
		If earnings > 0 Then
			RemoveSpecialPowerInEffect("�G�j��")
		End If
		For i = 1 To UBound(targets)
			targets(i).CurrentForm.RemoveSpecialPowerInEffect("�퓬�I��")
		Next 
		
		'��Ԃ̉���
		For i = 1 To UBound(targets)
			targets(i).CurrentForm.UpdateCondition()
		Next 
		
		'�X�e���X��������H
		If IsFeatureAvailable("�X�e���X") Then
			If IsWeaponClassifiedAs(w, "�E") Then
				For i = 1 To UBound(targets)
					With targets(i).CurrentForm
						If .Status_Renamed = "�o��" And .MaxAction > 0 Then
							AddCondition("�X�e���X����", 1)
							Exit For
						End If
					End With
				Next 
			Else
				AddCondition("�X�e���X����", 1)
			End If
		End If
		
		'���̋Z�̃p�[�g�i�[�̒e�����d�m������
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountWeapon
					If .Weapon(j).Name = wname Then
						.UseWeapon(j)
						If .IsWeaponClassifiedAs(j, "��") Then
							If .IsFeatureAvailable("�p�[�c����") Then
								uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
						ElseIf .IsWeaponClassifiedAs(j, "��") And .HP = 0 Then 
							.Die()
						ElseIf .IsWeaponClassifiedAs(j, "��") Then 
							If .IsFeatureAvailable("�ό`�Z") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "�ό`�Z" And LIndex(.FeatureData(k), 1) = wname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("�m�[�}�����[�h") Then
										uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("�m�[�}�����[�h") Then 
								uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'�����̕��킪�Ȃ������ꍇ�͎����̃f�[�^���g���ď���
				If j > .CountWeapon Then
					If Weapon(w).ENConsumption > 0 Then
						.EN = .EN - WeaponENConsumption(w)
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						.AddCondition("����", 1)
					End If
					If IsWeaponClassifiedAs(w, "�b") And .IsConditionSatisfied("�`���[�W����") Then
						.DeleteCondition("�`���[�W����")
					End If
					If IsWeaponClassifiedAs(w, "�C") Then
						.IncreaseMorale(-5 * WeaponLevel(w, "�C"))
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "��")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsWeaponClassifiedAs(w, "�v") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * WeaponLevel(w, "�v")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						.HP = MaxLng(.HP - .MaxHP * WeaponLevel(w, "��") \ 10, 0)
					End If
					If IsWeaponClassifiedAs(w, "��") Then
						If .IsFeatureAvailable("�p�[�c����") Then
							uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
					ElseIf IsWeaponClassifiedAs(w, "��") And .HP = 0 Then 
						.Die()
					ElseIf IsWeaponClassifiedAs(w, "��") Then 
						If .IsFeatureAvailable("�m�[�}�����[�h") Then
							uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'�ȉ��̓�����ʂ͕���f�[�^���ω�����\�������邽�߁A�����ɂ͓K�p����Ȃ�
		
		'�����̏���
		If IsWeaponClassifiedAs(w, "��") Then
			If IsFeatureAvailable("�p�[�c����") Then
				uname = LIndex(FeatureData("�p�[�c����"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("�p�[�c����")
					If IsSysMessageDefined("�j�󎞕���(" & Name & ")") Then
						SysMessage("�j�󎞕���(" & Name & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
						SysMessage("�j�󎞕���(" & fname & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���") Then 
						SysMessage("�j�󎞕���")
					ElseIf IsSysMessageDefined("����(" & Name & ")") Then 
						SysMessage("����(" & Name & ")")
					ElseIf IsSysMessageDefined("����(" & fname & ")") Then 
						SysMessage("����(" & fname & ")")
					ElseIf IsSysMessageDefined("����") Then 
						SysMessage("����")
					Else
						DisplaySysMessage(Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
					End If
				Else
					Die()
					CloseMessageForm()
					If Not is_event Then
						HandleEvent("�j��", MainPilot.ID)
						If IsScenarioFinished Then
							Exit Sub
						End If
					End If
				End If
			Else
				Die()
				CloseMessageForm()
				If Not is_event Then
					HandleEvent("�j��", MainPilot.ID)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
			End If
			
			'�g�o����U���ɂ�鎩�E
		ElseIf IsWeaponClassifiedAs(w, "��") And HP = 0 Then 
			Die()
			CloseMessageForm()
			If Not is_event Then
				HandleEvent("�j��", MainPilot.ID)
				If IsScenarioFinished Then
					Exit Sub
				End If
			End If
			
			'�ό`�Z
		ElseIf IsWeaponClassifiedAs(w, "��") Then 
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = wname Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'�A�C�e��������
		ElseIf Weapon(w).IsItem And Bullet(w) = 0 And MaxBullet(w) > 0 Then 
			'�A�C�e�����폜
			num = Data.CountWeapon
			num = num + MainPilot.Data.CountWeapon
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountWeapon
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountWeapon
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		
		'�C�x���g����
		Dim uparty As String
		Dim found As Boolean
		If Not is_event Then
			For i = 1 To UBound(targets)
				t = targets(i).CurrentForm
				
				If t.Status_Renamed = "�j��" Then
					'�j��C�x���g�𔭐�
					SaveSelections()
					SwapSelections()
					
					HandleEvent("�}�b�v�U���j��", t.MainPilot.ID)
					
					RestoreSelections()
					If IsScenarioFinished Or IsCanceled Then
						Exit Sub
					End If
				ElseIf t.Status_Renamed = "�o��" Then 
					If t.HP / t.MaxHP < targets_hp_ratio(i) Then
						'�������C�x���g
						SaveSelections()
						SwapSelections()
						
						HandleEvent("������", t.MainPilot.ID, 100 * (t.MaxHP - t.HP) \ t.MaxHP)
						
						RestoreSelections()
						If IsScenarioFinished Or IsCanceled Then
							Exit Sub
						End If
					End If
					
					'�^�[�Q�b�g�������Ă�����i���C�x���g�𔭐�
					With t.CurrentForm
						If .Status_Renamed = "�o��" And (.x <> targets_x(i) Or .y <> targets_y(i)) Then
							HandleEvent("�i��", .MainPilot.ID, .x, .y)
							If IsScenarioFinished Or IsCanceled Then
								Exit Sub
							End If
						End If
					End With
				End If
			Next 
			
			'�S�ŃC�x���g
			For i = 1 To 4
				
				Select Case i
					Case 1
						uparty = "����"
					Case 2
						uparty = "�m�o�b"
					Case 3
						uparty = "�G"
					Case 4
						uparty = "����"
				End Select
				
				found = False
				For j = 1 To UBound(targets)
					With targets(j).CurrentForm
						If .Party0 = uparty And .Status_Renamed <> "�o��" Then
							found = True
							Exit For
						End If
					End With
				Next 
				
				If found Then
					found = False
					For	Each u In UList
						With u
							If .Party0 = uparty And .Status_Renamed = "�o��" And Not .IsConditionSatisfied("�߈�") Then
								found = True
								Exit For
							End If
						End With
					Next u
					If Not found Then
						HandleEvent("�S��", uparty)
						If IsScenarioFinished Or IsCanceled Then
							Exit Sub
						End If
					End If
				End If
			Next 
			
			'�g�p��C�x���g
			If CurrentForm.Status_Renamed = "�o��" Then
				HandleEvent("�g�p��", CurrentForm.MainPilot.ID, wname)
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
			
			'�U����C�x���g
			If CurrentForm.Status_Renamed = "�o��" Then
				SaveSelections()
				For i = 1 To UBound(targets)
					SelectedTarget = targets(i).CurrentForm
					With SelectedTarget
						If .Status_Renamed = "�o��" Then
							HandleEvent("�U����", CurrentForm.MainPilot.ID, .MainPilot.ID)
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
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎����������`�F�b�N
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
	End Sub
	
	'����̎g�p�ɂ��d�m�A�e��̏�����s��
	Public Sub UseWeapon(ByVal w As Short)
		Dim i, lv As Short
		Dim hp_ratio, en_ratio As Double
		
		'�d�m����
		If Weapon(w).ENConsumption > 0 Then
			EN = EN - WeaponENConsumption(w)
		End If
		
		'�e������
		If Weapon(w).Bullet > 0 And Not IsWeaponClassifiedAs(w, "�i") Then
			SetBullet(w, Bullet(w) - 1)
			
			'�S�e��Ĕ���
			If IsWeaponClassifiedAs(w, "��") Then
				For i = 1 To UBound(dblBullet)
					SetBullet(i, MinLng(MaxBullet(i) * dblBullet(w), Bullet(i)))
				Next 
			Else
				For i = 1 To UBound(dblBullet)
					If IsWeaponClassifiedAs(i, "��") Then
						SetBullet(i, MinLng(CShort(MaxBullet(i) * dblBullet(w) + 0.49999), Bullet(i)))
					End If
				Next 
			End If
			
			'�e���E�g�p�񐔋��L�̏���
			SyncBullet()
		End If
		
		If IsWeaponClassifiedAs(w, "��") Then
			AddCondition("����", 1)
		End If
		
		If IsWeaponClassifiedAs(w, "�s") Then
			EN = 0
		End If
		
		If IsWeaponClassifiedAs(w, "�b") And IsConditionSatisfied("�`���[�W����") Then
			DeleteCondition("�`���[�W����")
		End If
		
		If WeaponLevel(w, "�`") > 0 Then
			AddCondition(WeaponNickname(w) & "�[�U��", WeaponLevel(w, "�`"))
		End If
		
		If IsWeaponClassifiedAs(w, "�C") Then
			IncreaseMorale(-5 * WeaponLevel(w, "�C"))
		End If
		
		If IsWeaponClassifiedAs(w, "��") Then
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * WeaponLevel(w, "��")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		ElseIf IsWeaponClassifiedAs(w, "�v") Then 
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * WeaponLevel(w, "�v")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		End If
		
		If Party = "����" Then
			If IsWeaponClassifiedAs(w, "�K") Then
				IncrMoney(-MaxLng(WeaponLevel(w, "�K"), 1) * Value \ 10)
			End If
		End If
		
		If IsWeaponClassifiedAs(w, "��") Then
			HP = MaxLng(HP - MaxHP * WeaponLevel(w, "��") \ 10, 0)
		End If
		
		'    '���̋Z�͂P�^�[���ɂP�񂾂��g�p�\
		'    If IsWeaponClassifiedAs(w, "��") Then
		'        AddCondition "���̋Z�g�p�s��", 1, 0, "��\��"
		'    End If
	End Sub
	
	'�e��
	Public Function Bullet(ByVal w As Short) As Short
		Bullet = dblBullet(w) * intMaxBullet(w)
	End Function
	
	'�ő�e��
	Public Function MaxBullet(ByVal w As Short) As Short
		MaxBullet = intMaxBullet(w)
	End Function
	
	'�e����ݒ�
	Public Sub SetBullet(ByVal w As Short, ByVal new_bullet As Short)
		If new_bullet < 0 Then
			dblBullet(w) = 0
		ElseIf intMaxBullet(w) > 0 Then 
			dblBullet(w) = new_bullet / intMaxBullet(w)
		Else
			dblBullet(w) = 1
		End If
	End Sub
	
	'�e���E�g�p�񐔋��L�̏���
	Public Sub SyncBullet()
		Dim j, a, w, i, k As Short
		Dim lv, idx As Short
		
		'����������̏���
		For w = 1 To CountWeapon
			If IsWeaponClassifiedAs(w, "��") Then
				lv = WeaponLevel(w, "��")
				'�e�������킹��
				For i = 1 To CountWeapon
					If w <> i And IsWeaponClassifiedAs(i, "��") And lv = WeaponLevel(i, "��") And MaxBullet(w) > 0 Then
						If MaxBullet(i) > MaxBullet(w) Then
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Bullet(w) \ MaxBullet(w)))
						Else
							SetBullet(i, MinLng(Bullet(i), CShort(MaxBullet(i) * Bullet(w) / MaxBullet(w) + 0.49999)))
						End If
					End If
				Next 
				'�A�r���e�B�̎g�p�񐔂����킹��
				For i = 1 To CountAbility
					If IsAbilityClassifiedAs(i, "��") And lv = AbilityLevel(i, "��") And MaxBullet(w) > 0 Then
						If MaxStock(i) > MaxBullet(w) Then
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Bullet(w) \ MaxBullet(w)))
						Else
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Bullet(w) / MaxBullet(w) + 0.49999))
						End If
					End If
				Next 
			End If
		Next 
		
		'�������A�r���e�B�̏���
		For a = 1 To CountAbility
			If IsAbilityClassifiedAs(a, "��") Then
				lv = AbilityLevel(a, "��")
				'�g�p�񐔂����킹��
				For i = 1 To CountAbility
					If a <> i And IsAbilityClassifiedAs(i, "��") And lv = AbilityLevel(i, "��") And MaxStock(a) > 0 Then
						If MaxStock(i) > MaxStock(a) Then
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Stock(a) \ MaxStock(a)))
						Else
							SetStock(i, MinLng(Stock(i), MaxStock(i) * Stock(a) / MaxStock(a) + 0.49999))
						End If
					End If
				Next 
				'�e�������킹��
				For i = 1 To CountWeapon
					If IsWeaponClassifiedAs(i, "��") And lv = WeaponLevel(i, "��") And MaxStock(a) > 0 Then
						If MaxBullet(i) > MaxStock(a) Then
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Stock(a) \ MaxStock(a)))
						Else
							SetBullet(i, MinLng(Bullet(i), MaxBullet(i) * Stock(a) / MaxStock(a) + 0.49999))
						End If
					End If
				Next 
			End If
		Next 
		
		'�đ�������̏���
		For w = 1 To CountWeapon
			If IsWeaponClassifiedAs(w, "��") Then
				'�e�������킹��
				For i = 1 To CountWeapon
					If w <> i And MaxBullet(i) > 0 Then
						SetBullet(w, MinLng(Bullet(w), CShort(MaxBullet(w) * Bullet(i) / MaxBullet(i) + 0.49999)))
					End If
				Next 
			End If
		Next 
		
		'���̌`�Ԃ̒e�����ύX
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
	
	
	
	' === �A�r���e�B�֘A���� ===
	
	'�A�r���e�B
	Public Function Ability(ByVal a As Short) As AbilityData
		Ability = adata(a)
	End Function
	
	'�A�r���e�B����
	Public Function CountAbility() As Short
		CountAbility = UBound(adata)
	End Function
	
	'�A�r���e�B�̈���
	Public Function AbilityNickname(ByVal a As Short) As String
		Dim u As Unit
		
		'���̓��̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Me
		AbilityNickname = adata(a).Nickname
		SelectedUnitForEvent = u
	End Function
	
	'�A�r���e�B a �̍ŏ��˒�
	Public Function AbilityMinRange(ByVal a As Short) As Short
		AbilityMinRange = Ability(a).MinRange
		If IsAbilityClassifiedAs(a, "��") Then
			AbilityMinRange = MinLng(AbilityMinRange + AbilityLevel(a, "��"), Ability(a).MaxRange)
		End If
	End Function
	
	'�A�r���e�B a �̍ő�˒�
	Public Function AbilityMaxRange(ByVal a As Short) As Short
		AbilityMaxRange = Ability(a).MaxRange
	End Function
	
	'�A�r���e�B a �̏���d�m
	Public Function AbilityENConsumption(ByVal a As Short) As Short
		'UPGRADE_NOTE: rate �� rate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
		Dim rate_Renamed As Double
		Dim p As Pilot
		Dim i As Short
		
		With Ability(a)
			AbilityENConsumption = .ENConsumption
			
			'�p�C���b�g�̔\�͂ɂ���ďp�y�ыZ�̏���d�m�͌�������
			If CountPilot > 0 Then
				p = MainPilot
				
				'�p�ɊY�����邩�H
				If IsSpellAbility(a) Then
					'�p�ɊY������ꍇ�͏p�Z�\�ɂ���Ăd�m����ʂ�ς���
					Select Case p.SkillLevel("�p")
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
				
				'�Z�ɊY�����邩�H
				If IsFeatAbility(a) Then
					'�Z�ɊY������ꍇ�͋Z�Z�\�ɂ���Ăd�m����ʂ�ς���
					Select Case p.SkillLevel("�Z")
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
			
			'�d�m������\�͂ɂ��C��
			rate_Renamed = 1
			If IsFeatureAvailable("�d�m�����") Then
				For i = 1 To CountFeature
					If Feature(i) = "�d�m�����" Then
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
	
	'�A�r���e�B a ������ attr �������ǂ���
	Public Function IsAbilityClassifiedAs(ByVal a As Short, ByRef attr As String) As Boolean
		If InStrNotNest(Ability(a).Class_Renamed, attr) > 0 Then
			IsAbilityClassifiedAs = True
		Else
			IsAbilityClassifiedAs = False
		End If
	End Function
	
	'�A�r���e�B a �̑��� atrr �̃��x��
	Public Function AbilityLevel(ByVal a As Short, ByRef attr As String) As Double
		Dim attrlv, aclass As String
		Dim start_idx, i As Short
		Dim c As String
		
		On Error GoTo ErrorHandler
		
		attrlv = attr & "L"
		
		'�A�r���e�B�����𒲂ׂĂ݂�
		aclass = Ability(a).Class_Renamed
		
		'���x���w�肪���邩�H
		start_idx = InStr(aclass, attrlv)
		If start_idx = 0 Then
			Exit Function
		End If
		
		'���x���w�蕔���̐؂�o��
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
		ErrorMessage(Name & "��" & "�A�r���e�B�u" & Ability(a).Name & "�v��" & "�����u" & attr & "�v�̃��x���w�肪�s���ł�")
	End Function
	
	'�A�r���e�B a ���p���ǂ���
	Public Function IsSpellAbility(ByVal a As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsAbilityClassifiedAs(a, "�p") Then
			IsSpellAbility = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Ability(a).NecessarySkill))
				nskill = LIndex((Ability(a).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "�p" Then
					IsSpellAbility = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'�A�r���e�B a ���Z���ǂ���
	Public Function IsFeatAbility(ByVal a As Short) As Boolean
		Dim i As Short
		Dim nskill As String
		
		If IsAbilityClassifiedAs(a, "�Z") Then
			IsFeatAbility = True
			Exit Function
		End If
		
		With MainPilot
			For i = 1 To LLength((Ability(a).NecessarySkill))
				nskill = LIndex((Ability(a).NecessarySkill), i)
				If InStr(nskill, "Lv") > 0 Then
					nskill = Left(nskill, InStr(nskill, "Lv") - 1)
				End If
				If .SkillType(nskill) = "�Z" Then
					IsFeatAbility = True
					Exit Function
				End If
			Next 
		End With
	End Function
	
	'�A�r���e�B a ���g�p�\���ǂ���
	'ref_mode �̓��j�b�g�̏�ԁi�ړ��O�A�ړ���j������
	Public Function IsAbilityAvailable(ByVal a As Short, ByRef ref_mode As String) As Boolean
		Dim j, i, k As Short
		Dim ad As AbilityData
		Dim uname, pname As String
		Dim u As Unit
		
		IsAbilityAvailable = False
		
		ad = Ability(a)
		
		'�C�x���g�R�}���h�uDisable�v
		If IsDisabled((ad.Name)) Then
			Exit Function
		End If
		
		'�p�C���b�g������Ă��Ȃ���Ώ�Ɏg�p�\�Ɣ���
		If CountPilot = 0 Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		'�K�v�Z�\
		If Not IsAbilityMastered(a) Then
			Exit Function
		End If
		
		'�K�v����
		If Not IsAbilityEnabled(a) Then
			Exit Function
		End If
		
		'�X�e�[�^�X�\���ł͕K�v�Z�\�����������Ă���΂n�j
		If ref_mode = "�C���^�[�~�b�V����" Or ref_mode = "" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		With MainPilot
			'�K�v�C��
			If ad.NecessaryMorale > 0 Then
				If .Morale < ad.NecessaryMorale Then
					Exit Function
				End If
			End If
			
			'��͏���A�r���e�B
			If IsAbilityClassifiedAs(a, "��") Then
				If .Plana < AbilityLevel(a, "��") * 5 Then
					Exit Function
				End If
			ElseIf IsAbilityClassifiedAs(a, "�v") Then 
				If .Plana < AbilityLevel(a, "�v") * 5 Then
					Exit Function
				End If
			End If
		End With
		
		'�����g�p�s�\���
		If ConditionLifetime("�I�[���g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�I") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("���\�͎g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "��") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�������g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�V") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�����o�g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�T") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�m�o�����g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�T") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("��͎g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "��") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�p�g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�p") Then
				Exit Function
			End If
		End If
		If ConditionLifetime("�Z�g�p�s�\") > 0 Then
			If IsAbilityClassifiedAs(a, "�Z") Then
				Exit Function
			End If
		End If
		For i = 1 To CountCondition
			If Len(Condition(i)) > 6 Then
				If Right(Condition(i), 6) = "�����g�p�s�\" Then
					If InStrNotNest(Ability(a).Class_Renamed, Left(Condition(i), Len(Condition(i)) - 6)) > 0 Then
						Exit Function
					End If
				End If
			End If
		Next 
		
		'�e��������邩
		If MaxStock(a) > 0 Then
			If Stock(a) < 1 Then
				Exit Function
			End If
		End If
		
		'�d�m������邩
		If ad.ENConsumption > 0 Then
			If EN < AbilityENConsumption(a) Then
				Exit Function
			End If
		End If
		
		'����������邩�c�c
		If Party = "����" Then
			If IsAbilityClassifiedAs(a, "�K") Then
				If Money < MaxLng(AbilityLevel(a, "�K"), 1) * Value \ 10 Then
					Exit Function
				End If
			End If
		End If
		
		'�ړ��s�\���ɂ͈ړ��^�}�b�v�A�r���e�B�͎g�p�s�\
		If IsConditionSatisfied("�ړ��s�\") Then
			If IsAbilityClassifiedAs(a, "�l��") Then
				Exit Function
			End If
		End If
		
		'�p�y�щ����Z�͒��ُ�Ԃł͎g�p�s�\
		If IsConditionSatisfied("����") Then
			With MainPilot
				If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "��") Then
					Exit Function
				End If
			End With
		End If
		
		'�p�͋���m��Ԃł͎g�p�s�\
		If IsConditionSatisfied("����m") Then
			With MainPilot
				If IsSpellAbility(a) Then
					Exit Function
				End If
			End With
		End If
		
		'���̋Z�̏���
		If IsAbilityClassifiedAs(a, "��") Then
			If Not IsCombinationAbilityAvailable(a) Then
				Exit Function
			End If
		End If
		
		'���̒n�`�ŕό`�ł��邩�H
		If IsAbilityClassifiedAs(a, "��") Then
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = ad.Name Then
						If Not OtherForm(LIndex(FeatureData(i), 2)).IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End If
				Next 
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				If Not OtherForm(LIndex(FeatureData("�m�[�}�����[�h"), 1)).IsAbleToEnter(x, y) Then
					Exit Function
				End If
			End If
			If IsConditionSatisfied("�`�ԌŒ�") Then
				Exit Function
			End If
			If IsConditionSatisfied("�@�̌Œ�") Then
				Exit Function
			End If
		End If
		
		'�m��������
		If IsAbilityClassifiedAs(a, "�m") Then
			If HP > MaxHP \ 4 Then
				Exit Function
			End If
		End If
		
		'�����`���[�W�A�r���e�B���[�U��
		If IsConditionSatisfied(AbilityNickname(a) & "�[�U��") Then
			Exit Function
		End If
		'���L���큕�A�r���e�B���[�U���̏ꍇ���g�p�s��
		Dim lv As Short
		If IsAbilityClassifiedAs(a, "��") Then
			lv = AbilityLevel(a, "��")
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "��") Then
					If lv = AbilityLevel(i, "��") Then
						If IsConditionSatisfied(AbilityNickname(i) & "�[�U��") Then
							Exit Function
						End If
					End If
				End If
			Next 
			For i = 1 To CountAbility
				If IsAbilityClassifiedAs(i, "��") Then
					If lv = AbilityLevel(i, "��") Then
						If IsConditionSatisfied(AbilityNickname(i) & "�[�U��") Then
							Exit Function
						End If
					End If
				End If
			Next 
		End If
		
		'�g�p�֎~
		If IsAbilityClassifiedAs(a, "��") > 0 Then
			Exit Function
		End If
		
		'�`���[�W����ł���΂����܂łłn�j
		If ref_mode = "�`���[�W" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		'�`���[�W���A�r���e�B
		If IsAbilityClassifiedAs(a, "�b") Then
			If Not IsConditionSatisfied("�`���[�W����") Then
				Exit Function
			End If
		End If
		
		For i = 1 To ad.CountEffect
			If ad.EffectType(i) = "����" Then
				'�����͊��ɏ������s���Ă���ꍇ�ɂ͕s�\
				For j = 1 To CountServant
					With Servant(j).CurrentForm
						Select Case .Status_Renamed
							Case "�o��", "�i�["
								'�g�p�s��
								Exit Function
							Case "����`��", "���`��"
								'���̌�̌`�Ԃ��o�����Ȃ�g�p�s��
								For k = 1 To .CountFeature
									If .Feature(k) = "����" Then
										uname = LIndex(.FeatureData(k), 2)
										If UList.IsDefined(uname) Then
											With UList.Item(uname).CurrentForm
												If .Status_Renamed = "�o��" Or .Status_Renamed = "�i�[" Then
													Exit Function
												End If
											End With
										End If
									End If
								Next 
						End Select
					End With
				Next 
				
				'�������j�b�g�̃f�[�^�������ƒ�`����Ă��邩�`�F�b�N
				If Not UDList.IsDefined(ad.EffectData(i)) Then
					Exit Function
				End If
				pname = UDList.Item(ad.EffectData(i)).FeatureData("�ǉ��p�C���b�g")
				If Not PDList.IsDefined(pname) Then
					Exit Function
				End If
				
				'�������郆�j�b�g�ɏ��p�C���b�g���ėp�p�C���b�g�ł��U�R�p�C���b�g�ł�
				'�Ȃ��ꍇ�A���̃��j�b�g�����ɏo�����ł���Ύg�p�s��
				If InStr(pname, "(�ėp)") = 0 And InStr(pname, "(�U�R)") = 0 Then
					If PList.IsDefined(pname) Then
						u = PList.Item(pname).Unit_Renamed
						If Not u Is Nothing Then
							If u.Status_Renamed = "�o��" Or u.Status_Renamed = "�i�[" Then
								Exit Function
							End If
						End If
					End If
				End If
			End If
		Next 
		
		If ref_mode = "�X�e�[�^�X" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		For i = 1 To ad.CountEffect
			If ad.EffectType(i) = "�ϐg" Then
				'������ϐg������ꍇ
				If Ability(a).MaxRange = 0 Then
					'�m�[�}�����[�h�������j�b�g�͕ϐg�ł��Ȃ�
					'(�ϐg����̕��A���o���Ȃ�����)
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						Exit Function
					End If
					
					'���̏ꏊ�ŕϐg�\���H
					With OtherForm(LIndex(Ability(a).EffectData(i), 1))
						If Not .IsAbleToEnter(x, y) Then
							Exit Function
						End If
					End With
				End If
			End If
		Next 
		
		If ref_mode = "�ړ��O" Then
			IsAbilityAvailable = True
			Exit Function
		End If
		
		If AbilityMaxRange(a) > 1 Or AbilityMaxRange(a) = 0 Then
			If IsAbilityClassifiedAs(a, "�o") Then
				IsAbilityAvailable = True
			Else
				IsAbilityAvailable = False
			End If
		Else
			If IsAbilityClassifiedAs(a, "�p") Then
				IsAbilityAvailable = False
			Else
				IsAbilityAvailable = True
			End If
		End If
	End Function
	
	'�A�r���e�B a �̕K�v�Z�\�𖞂����Ă��邩�B
	Public Function IsAbilityMastered(ByVal a As Short) As Boolean
		IsAbilityMastered = IsNecessarySkillSatisfied((Ability(a).NecessarySkill))
	End Function
	
	'�A�r���e�B a �̕K�v�����𖞂����Ă��邩�B
	Public Function IsAbilityEnabled(ByVal a As Short) As Boolean
		IsAbilityEnabled = IsNecessarySkillSatisfied((Ability(a).NecessaryCondition))
	End Function
	
	'�A�r���e�B���g�p�\�ł���A���˒����ɗL���ȃ^�[�Q�b�g�����邩�ǂ���
	Public Function IsAbilityUseful(ByVal a As Short, ByRef ref_mode As String) As Boolean
		Dim i, j As Short
		Dim max_range, min_range As Short
		
		'�A�r���e�B���g�p�\���H
		If Not IsAbilityAvailable(a, ref_mode) Then
			IsAbilityUseful = False
			Exit Function
		End If
		
		'�����^�}�b�v�A�r���e�B�Ɛ�^�}�b�v�A�r���e�B�͓���Ȃ̂Ŕ��肪�ł��Ȃ�
		'�ړ��^�}�b�v�A�r���e�B�͈ړ���i�Ƃ��Ďg�����Ƃ��l��
		If IsAbilityClassifiedAs(a, "�l��") Or IsAbilityClassifiedAs(a, "�l��") Or IsAbilityClassifiedAs(a, "�l��") Then
			IsAbilityUseful = True
			Exit Function
		End If
		
		'�����͏�ɗL�p
		For i = 1 To Ability(a).CountEffect
			If Ability(a).EffectType(i) = "����" Then
				IsAbilityUseful = True
				Exit Function
			End If
		Next 
		
		min_range = AbilityMinRange(a)
		max_range = AbilityMaxRange(a)
		
		'�g�p���鑊�肪���邩����
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
	
	'�A�r���e�B���^�[�Q�b�gt�ɑ΂��ėL��(���ɗ���)���ǂ���
	Public Function IsAbilityEffective(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim edata As String
		Dim elevel As Double
		Dim flag As Boolean
		
		With t
			'�G�ɂ͎g�p�ł��Ȃ��B
			'IsEnemy�ł͖����������������������j�b�g��G�ƔF�����Ă��܂��̂�
			'�����ł͓Ǝ��̔������g��
			Select Case Party
				Case "����", "�m�o�b"
					If .Party <> "����" And .Party0 <> "����" And .Party <> "�m�o�b" And .Party0 <> "�m�o�b" Then
						Exit Function
					End If
				Case Else
					If .Party <> Party And .Party0 <> Party Then
						Exit Function
					End If
			End Select
			
			'�A�r���e�B�����̃��j�b�g�ɑ΂��ēK�p�\���H
			If Not IsAbilityApplicable(a, t) Then
				Exit Function
			End If
			
			IsAbilityEffective = True
			For i = 1 To Ability(a).CountEffect
				edata = Ability(a).EffectData(i)
				elevel = Ability(a).EffectLevel(i)
				Select Case Ability(a).EffectType(i)
					Case "��"
						If elevel > 0 Then
							If .HP < .MaxHP Then
								If Not .IsConditionSatisfied("�]���r") Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							IsAbilityEffective = False
						Else
							'�g�o�����������邽�߂̃A�r���e�B�Ƃ����̂͗L�蓾��̂�
							IsAbilityEffective = True
							Exit Function
						End If
						
					Case "����"
						If edata = "" Then
							If .ConditionLifetime("�U���s�\") > 0 Or .ConditionLifetime("�ړ��s�\") > 0 Or .ConditionLifetime("���b��") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("���|") > 0 Or .ConditionLifetime("�x��") > 0 Or .ConditionLifetime("����m") > 0 Or .ConditionLifetime("�]���r") > 0 Or .ConditionLifetime("�񕜕s�\") > 0 Or .ConditionLifetime("�Ή�") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("���") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("��") > 0 Or .ConditionLifetime("�Ӗ�") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("�߈�") > 0 Or .ConditionLifetime("�I�[���g�p�s�\") > 0 Or .ConditionLifetime("���\�͎g�p�s�\") > 0 Or .ConditionLifetime("�������g�p�s�\") > 0 Or .ConditionLifetime("�����o�g�p�s�\") > 0 Or .ConditionLifetime("�m�o�����g�p�s�\") > 0 Or .ConditionLifetime("��͎g�p�s�\") > 0 Or .ConditionLifetime("�p�g�p�s�\") > 0 Or .ConditionLifetime("�Z�g�p�s�\") > 0 Then
								IsAbilityEffective = True
								Exit Function
							End If
							For j = 1 To .CountCondition
								If Len(.Condition(j)) > 6 Then
									'�O�񏑂��Y�ꂽ�̂ł����A
									'��_�͂Ƃ������L���͈�T�Ƀf�����b�g�݂̂ł��Ȃ��̂�
									'��ԉ񕜂��珜�O���Ă݂܂����B
									If Right(.Condition(j), 6) = "�����g�p�s�\" Then
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
						
					Case "�⋋"
						If elevel > 0 Then
							If .EN < .MaxEN Then
								If Not .IsConditionSatisfied("�]���r") Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							IsAbilityEffective = False
						End If
						
					Case "��͉�", "�v���[�i��"
						If elevel > 0 Then
							If .MainPilot.Plana < .MainPilot.MaxPlana Then
								IsAbilityEffective = True
								Exit Function
							End If
							IsAbilityEffective = False
						End If
						
					Case "�r�o��"
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
							
							If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
								If .AdditionalSupport.SP < .AdditionalSupport.MaxSP Then
									IsAbilityEffective = True
									Exit Function
								End If
							End If
							
							IsAbilityEffective = False
						End If
						
					Case "�C�͑���"
						If elevel > 0 Then
							With .MainPilot
								If .Morale < .MaxMorale And .Personality <> "�@�B" Then
									IsAbilityEffective = True
									Exit Function
								End If
							End With
							
							For j = 2 To .CountPilot
								With .Pilot(j)
									If .Morale < .MaxMorale And .Personality <> "�@�B" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							Next 
							
							For j = 1 To .CountSupport
								With .Support(j)
									If .Morale < .MaxMorale And .Personality <> "�@�B" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							Next 
							
							If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
								With .AdditionalSupport
									If .Morale < .MaxMorale And .Personality <> "�@�B" Then
										IsAbilityEffective = True
										Exit Function
									End If
								End With
							End If
							
							IsAbilityEffective = False
						End If
						
					Case "���U"
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
						
					Case "�t��"
						If Not .IsConditionSatisfied(LIndex(edata, 1) & "�t��") Or IsAbilityClassifiedAs(a, "��") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "����"
						If Not .IsConditionSatisfied(LIndex(edata, 1) & "����") Or IsAbilityClassifiedAs(a, "��") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "���"
						If Not .IsConditionSatisfied(edata) Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "�čs��"
						If Ability(a).MaxRange = 0 Then
							GoTo NextEffect
						End If
						
						If .Action = 0 And .MaxAction > 0 Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "�ϐg"
						If Not .IsFeatureAvailable("�m�[�}�����[�h") Then
							IsAbilityEffective = True
							Exit Function
						End If
						IsAbilityEffective = False
						
					Case "�\�̓R�s�["
						If t Is Me Or IsFeatureAvailable("�m�[�}�����[�h") Or .IsConditionSatisfied("����") > 0 Or .IsEnemy(Me) Or IsEnemy(t) Then
							IsAbilityEffective = False
							GoTo NextEffect
						End If
						
						If InStr(edata, "�T�C�Y������") > 0 Then
							If Size <> .Size Then
								IsAbilityEffective = False
								GoTo NextEffect
							End If
						ElseIf InStr(edata, "�T�C�Y��������") = 0 Then 
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
			
			'�����������ʂ��Ȃ����̂͏�Ɏg�p�\�Ƃ݂Ȃ�
			'(include���œ�����ʂ��`���Ă���Ɖ���)
			If IsAbilityEffective Then
				Exit Function
			End If
		End With
	End Function
	
	'�A�r���e�B���^�[�Q�b�gt�ɑ΂��ēK�p�\���ǂ���
	Public Function IsAbilityApplicable(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i As Short
		Dim fname As String
		
		If IsAbilityClassifiedAs(a, "��") Then
			If Not t.Weakness(Ability(a).Class_Renamed) And Not t.Effective(Ability(a).Class_Renamed) Then
				Exit Function
			End If
		End If
		
		If IsAbilityClassifiedAs(a, "��") Then
			If Not t.Weakness(Mid(Ability(a).Class_Renamed, InStrNotNest(Ability(a).Class_Renamed, "��") + 1)) And Not t.Effective(Mid(Ability(a).Class_Renamed, InStrNotNest(Ability(a).Class_Renamed, "��") + 1)) Then
				Exit Function
			End If
		End If
		
		If Me Is t Then
			'�x����p�A�r���e�B�͎����ɂ͎g�p�ł��Ȃ�
			If Not IsAbilityClassifiedAs(a, "��") Then
				IsAbilityApplicable = True
			End If
			Exit Function
		End If
		
		'�������̑ΏۂɂȂ�ꍇ�͎g�p�o���Ȃ�
		If t.Immune(Ability(a).Class_Renamed) Then
			If Not t.Weakness(Ability(a).Class_Renamed) And Not t.Effective(Ability(a).Class_Renamed) Then
				Exit Function
			End If
		End If
		
		If IsAbilityClassifiedAs(a, "��") Then
			If t.IsConditionSatisfied("�Ӗ�") Then
				Exit Function
			End If
		End If
		
		With t.MainPilot
			If IsAbilityClassifiedAs(a, "��") Then
				'UPGRADE_WARNING: Mod �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
				If .Level Mod AbilityLevel(a, "��") <> 0 Then
					Exit Function
				End If
			End If
			
			If IsAbilityClassifiedAs(a, "��") Then
				If .Personality = "�@�B" Then
					Exit Function
				End If
			End If
			
			If IsAbilityClassifiedAs(a, "��") Then
				If .Sex <> "�j��" Then
					Exit Function
				End If
			End If
			If IsAbilityClassifiedAs(a, "��") Then
				If .Sex <> "����" Then
					Exit Function
				End If
			End If
		End With
		
		'�C���s��
		If t.IsFeatureAvailable("�C���s��") Then
			For i = 1 To Ability(a).CountEffect
				If Ability(a).EffectType(i) = "��" Then
					Exit For
				End If
			Next 
			If i <= Ability(a).CountEffect Then
				For i = 2 To CInt(t.FeatureData("�C���s��"))
					fname = LIndex(t.FeatureData("�C���s��"), i)
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
	
	'���j�b�g t ���A�r���e�B a �̎˒��͈͓��ɂ��邩���`�F�b�N
	Public Function IsTargetWithinAbilityRange(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim distance As Short
		
		IsTargetWithinAbilityRange = True
		
		distance = System.Math.Abs(x - t.x) + System.Math.Abs(y - t.y)
		
		'�ŏ��˒��`�F�b�N
		If distance < AbilityMinRange(a) Then
			IsTargetWithinAbilityRange = False
			Exit Function
		End If
		
		'�ő�˒��`�F�b�N
		If distance > AbilityMaxRange(a) Then
			IsTargetWithinAbilityRange = False
			Exit Function
		End If
		
		'���̋Z�Ŏ˒����P�̏ꍇ�͑�����͂�ł���K�v������
		Dim partners() As Unit
		If IsAbilityClassifiedAs(a, "��") And Not IsAbilityClassifiedAs(a, "�l") And AbilityMaxRange(a) = 1 Then
			CombinationPartner("�A�r���e�B", a, partners, t.x, t.y)
			If UBound(partners) = 0 Then
				IsTargetWithinAbilityRange = False
				Exit Function
			End If
		End If
	End Function
	
	'�ړ��𕹗p�����ꍇ�Ƀ��j�b�g t ���A�r���e�B a �̎˒��͈͓��ɂ��邩���`�F�b�N
	Public Function IsTargetReachableForAbility(ByVal a As Short, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim max_range As Short
		
		IsTargetReachableForAbility = True
		
		With t
			'�ړ��͈͂���G�ɍU�����͂������`�F�b�N
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
	
	'�A�r���e�B�̎c��g�p��
	Public Function Stock(ByVal a As Short) As Short
		Stock = dblStock(a) * MaxStock(a)
	End Function
	
	'�A�r���e�B�̍ő�g�p��
	Public Function MaxStock(ByVal a As Short) As Short
		If BossRank > 0 Then
			MaxStock = Ability(a).Stock * (5 + BossRank) / 5
		Else
			MaxStock = Ability(a).Stock
		End If
	End Function
	
	'�A�r���e�B�̎c��g�p�񐔂�ݒ�
	Public Sub SetStock(ByVal a As Short, ByVal new_stock As Short)
		If new_stock < 0 Then
			dblStock(a) = 0
		ElseIf MaxStock(a) > 0 Then 
			dblStock(a) = new_stock / MaxStock(a)
		Else
			dblStock(a) = 1
		End If
	End Sub
	
	
	
	' === �A�r���e�B�����֘A���� ===
	
	'�A�r���e�B���g�p
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
		
		'���݂̑I���󋵂��Z�[�u
		SaveSelections()
		
		'�I����e��؂�ւ�
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedTarget = t
		SelectedTargetForEvent = t
		SelectedAbility = a
		SelectedAbilityName = aname
		
		If Not is_map_ability Then
			'�ʏ�A�r���e�B�̏ꍇ
			If BattleAnimation Then
				RedrawScreen()
			End If
			
			If IsAbilityClassifiedAs(a, "��") Then
				'�˒���0�̏ꍇ�̓}�X�N���N���A���Ă���
				If AbilityMaxRange(a) = 0 Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							MaskData(i, j) = True
						Next 
					Next 
					MaskData(x, y) = False
				End If
				
				'���̋Z�̏ꍇ�Ƀp�[�g�i�[���n�C���C�g�\��
				If AbilityMaxRange(a) = 1 Then
					CombinationPartner("�A�r���e�B", a, partners, t.x, t.y)
				Else
					CombinationPartner("�A�r���e�B", a, partners)
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
			
			'�_�C�A���O�p�ɂ��炩���ߒǉ��p�C���b�g���쐬���Ă���
			For i = 1 To Ability(a).CountEffect
				edata = Ability(a).EffectData(i)
				Select Case Ability(a).EffectType(i)
					Case "�ϐg"
						If Not UDList.IsDefined(LIndex(edata, 1)) Then
							ErrorMessage(LIndex(edata, 1) & "�̃f�[�^����`����Ă��܂���")
							Exit Function
						End If
						
						With UDList.Item(LIndex(edata, 1))
							If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
								If Not PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
									PList.Add(.FeatureData("�ǉ��p�C���b�g"), MainPilot.Level, Party0)
								End If
							End If
						End With
				End Select
			Next 
			
			'�A�r���e�B�g�p���̃��b�Z�[�W���������
			If IsAnimationDefined(aname & "(����)") Then
				PlayAnimation(aname & "(����)")
			End If
			If IsMessageDefined("������(" & aname & ")") Then
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				PilotMessage("������(" & aname & ")")
			End If
			If IsMessageDefined(aname) Or IsMessageDefined("�A�r���e�B") Then
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				PilotMessage(aname, "�A�r���e�B")
			End If
			If IsAnimationDefined(aname & "(�g�p)") Then
				PlayAnimation(aname & "(�g�p)", "", True)
			End If
			If IsAnimationDefined(aname & "(����)") Or IsAnimationDefined(aname) Then
				PlayAnimation(aname & "(����)", "", True)
				is_anime_played = True
			Else
				SpecialEffect(aname, "", True)
			End If
			
			'�A�r���e�B�̎�ނ́H
			For i = 1 To Ability(a).CountEffect
				Select Case Ability(a).EffectType(i)
					Case "����"
						aname = ""
						Exit For
					Case "�čs��"
						If Ability(a).MaxRange > 0 Then
							atype = Ability(a).EffectType(i)
						End If
					Case "���"
					Case Else
						atype = Ability(a).EffectType(i)
				End Select
			Next 
			Select Case UBound(partners)
				Case 0
					'�ʏ�
					msg = Nickname & "��"
				Case 1
					'�Q�̍���
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "��[" & partners(1).MainPilot.Nickname & "]��[" & Nickname & "]��"
					Else
						msg = Nickname & "�B��"
					End If
				Case 2
					'�R�̍���
					If Nickname <> partners(1).Nickname Then
						msg = Nickname & "��[" & partners(1).Nickname & "]�A[" & partners(2).Nickname & "]�Ƌ���"
					ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
						msg = MainPilot.Nickname & "�A[" & partners(1).MainPilot.Nickname & "]�A[" & partners(2).MainPilot.Nickname & "]��[" & Nickname & "]��"
					Else
						msg = Nickname & "�B��"
					End If
				Case Else
					'�R�̈ȏ�
					msg = Nickname & "�B��"
			End Select
			
			If IsSpellAbility(a) Then
				If Not t Is Nothing And Ability(a).MaxRange <> 0 Then
					If Me Is t Then
						msg = msg & "������"
					Else
						msg = msg & "[" & t.Nickname & "]��"
					End If
				End If
				If Right(anickname, 2) = "����" Then
					msg = msg & "[" & anickname & "]���������B"
				ElseIf Right(anickname, 2) = "�̏�" Then 
					msg = msg & "[" & Left(anickname, Len(anickname) - 2) & "]�̎������������B"
				Else
					msg = msg & "[" & anickname & "]�̎������������B"
				End If
			ElseIf Right(anickname, 1) = "��" Then 
				msg = msg & "[" & anickname & "]���̂����B"
			ElseIf Right(anickname, 2) = "�x��" Then 
				msg = msg & "[" & anickname & "]��x�����B"
			Else
				If Not t Is Nothing And Ability(a).MaxRange <> 0 Then
					If Me Is t Then
						msg = msg & "������"
					Else
						msg = msg & "[" & t.Nickname & "]��"
					End If
				End If
				msg = msg & "[" & anickname & "]���g�����B"
			End If
			
			If IsSysMessageDefined(aname) Then
				'�u�A�r���e�B��(���)�v�̃��b�Z�[�W���g�p
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				SysMessage(aname)
			ElseIf IsSysMessageDefined("�A�r���e�B") Then 
				'�u�A�r���e�B(���)�v�̃��b�Z�[�W���g�p
				If Not frmMessage.Visible Then
					If SelectedTarget Is Me Then
						OpenMessageForm(Me)
					Else
						OpenMessageForm(SelectedTarget, Me)
					End If
				End If
				SysMessage("�A�r���e�B")
			ElseIf atype = "�ϐg" And Ability(a).MaxRange = 0 Then 
				'�ϐg�̏ꍇ�̓��b�Z�[�W�Ȃ�
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
			
			'�d�m����g�p�񐔌���
			UseAbility(a)
			
			'�A�r���e�B�̎g�p�Ɏ��s�H
			If Dice(10) <= AbilityLevel(a, "��") Then
				DisplaySysMessage("���������������Ȃ������c")
				GoTo Finish
			End If
		Else
			'�}�b�v�A�r���e�B�̏ꍇ
			If IsAnimationDefined(aname & "(����)") Or IsAnimationDefined(aname) Then
				PlayAnimation(aname & "(����)")
				is_anime_played = True
			End If
		End If
		
		'���肪�A�r���e�B�̑����ɑ΂��Ė����������������Ă���Ȃ�A�r���e�B��
		'���ʂȂ�
		If Not Me Is t Then
			If t.Immune(aclass) Then
				GoTo Finish
			End If
		End If
		
		'�C�͒ቺ�A�r���e�B
		If IsAbilityClassifiedAs(a, "�E") Then
			t.IncreaseMorale(-10)
		End If
		
		'������ʏ����A�r���e�B
		If IsAbilityClassifiedAs(a, "��") Then
			i = 1
			Do While i <= t.CountCondition
				If (InStr(t.Condition(i), "�t��") > 0 Or InStr(t.Condition(i), "����") > 0 Or InStr(t.Condition(i), "�t�o") > 0) And t.Condition(i) <> "�m�[�}�����[�h�t��" And t.ConditionLifetime(i) <> 0 Then
					t.DeleteCondition(i)
				Else
					i = i + 1
				End If
			Loop 
		End If
		
		'���ӋZ�E�s����ɂ��A�r���e�B���ʂւ̏C���l���v�Z
		elv_mod = 1#
		elv_mod2 = 1#
		With MainPilot
			'���ӋZ
			If .IsSkillAvailable("���ӋZ") Then
				buf = .SkillData("���ӋZ")
				For i = 1 To Len(buf)
					If InStr(aclass, GetClassBundle(buf, i)) > 0 Then
						elv_mod = 1.2 * elv_mod
						elv_mod2 = 1.4 * elv_mod2
						Exit For
					End If
				Next 
			End If
			
			'�s����
			If .IsSkillAvailable("�s����") Then
				buf = .SkillData("�s����")
				For i = 1 To Len(buf)
					If InStr(aclass, GetClassBundle(buf, i)) > 0 Then
						elv_mod = 0.8 * elv_mod
						elv_mod2 = 0.6 * elv_mod2
						Exit For
					End If
				Next 
			End If
		End With
		
		'�A�r���e�B�̌��ʂ�K�p
		For i = 1 To Ability(a).CountEffect
			With Ability(a)
				edata = .EffectData(i)
				elevel = .EffectLevel(i) * elv_mod
				elevel2 = .EffectLevel(i) * elv_mod2
			End With
			Select Case Ability(a).EffectType(i)
				Case "��"
					With t
						If elevel > 0 Then
							'�g�o�͊��ɍő�l�H
							If .HP = .MaxHP Then
								GoTo NextLoop
							End If
							
							'�]���r�H
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextLoop
							End If
							
							If Not is_anime_played Then
								If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "��") Then
									ShowAnimation("�񕜖��@����")
								Else
									ShowAnimation("�C�����u����")
								End If
							End If
							
							prev_value = .HP
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(5 * elevel * .Shooting)
								Else
									epower = 500 * elevel
								End If
								epower = epower * (10 + .SkillLevel("�C��")) \ 10
							End With
							t.HP = t.HP + epower
							DrawSysString(.x, .y, "+" & VB6.Format(.HP - prev_value))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��[" & VB6.Format(.HP - prev_value) & "]�񕜂���;" & "�c��" & Term("�g�o", t) & "��" & VB6.Format(.HP) & "�i������ = " & VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP) & "���j")
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
							DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��[" & VB6.Format(prev_value - .HP) & "]��������;" & "�c��" & Term("�g�o", t) & "��" & VB6.Format(.HP) & "�i������ = " & VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP) & "���j")
						End If
					End With
					
				Case "�⋋"
					With t
						If elevel > 0 Then
							'�d�m�͊��ɍő�l�H
							If .EN = .MaxEN Then
								GoTo NextLoop
							End If
							
							'�]���r�H
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextLoop
							End If
							
							If Not is_anime_played Then
								If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "��") Then
									ShowAnimation("�񕜖��@����")
								Else
									ShowAnimation("�⋋���u����")
								End If
							End If
							
							prev_value = .EN
							With MainPilot
								If IsSpellAbility(a) Then
									epower = CInt(elevel * .Shooting \ 2)
								Else
									epower = 50 * elevel
								End If
								epower = epower * (10 + .SkillLevel("�⋋")) \ 10
							End With
							t.EN = t.EN + epower
							DrawSysString(.x, .y, "+" & VB6.Format(.EN - prev_value))
							If t Is Me Then
								UpdateMessageForm(Me)
							Else
								UpdateMessageForm(t, Me)
							End If
							DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��[" & VB6.Format(.EN - prev_value) & "]�񕜂���;" & "�c��" & Term("�d�m", t) & "��" & VB6.Format(.EN))
							is_useful = True
						ElseIf elevel < 0 Then 
							'�d�m�͊���0�H
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
							DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��[" & VB6.Format(prev_value - .EN) & "]��������;" & "�c��" & Term("�d�m", t) & "��" & VB6.Format(.EN))
						End If
					End With
					
				Case "��͉�", "�v���[�i��"
					With t.MainPilot
						If elevel > 0 Then
							'��͂͊��ɍő�l�H
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
							DisplaySysMessage(.Nickname & "��[" & .SkillName0("���") & "]��[" & VB6.Format(.Plana - prev_value) & "]�񕜂����B")
							is_useful = True
						ElseIf elevel < 0 Then 
							'��͂͊���0�H
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
							DisplaySysMessage(.Nickname & "��[" & .SkillName0("���") & "]��[" & VB6.Format(prev_value - .Plana) & "]���������B")
						End If
					End With
					
				Case "�r�o��"
					If IsSpellAbility(a) Then
						epower = CInt(elevel * MainPilot.Shooting \ 10)
					Else
						epower = 10 * elevel
					End If
					
					With t
						'�p�C���b�g�����v�Z
						num = .CountPilot + .CountSupport
						If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
							num = num + 1
						End If
						
						If elevel > 0 Then
							If num = 1 Then
								'�p�C���b�g���P���̂�
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower
									DrawSysString(t.x, t.y, "+" & VB6.Format(.SP - prev_value))
									DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - prev_value) & "�񕜂����B")
									If .SP > prev_value Then
										is_useful = True
									End If
								End With
							Else
								'�����̃p�C���b�g���Ώ�
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower \ 5 + epower \ num
									DrawSysString(t.x, t.y, "+" & VB6.Format(.SP - prev_value))
									DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - prev_value) & "�񕜂����B")
									If .SP > prev_value Then
										is_useful = True
									End If
								End With
								
								For j = 2 To .CountPilot
									With .Pilot(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - prev_value) & "�񕜂����B")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								Next 
								
								For j = 1 To .CountSupport
									With .Support(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - prev_value) & "�񕜂����B")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								Next 
								
								If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
									With .AdditionalSupport
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - prev_value) & "�񕜂����B")
										If .SP > prev_value Then
											is_useful = True
										End If
									End With
								End If
							End If
						ElseIf elevel < 0 Then 
							If num = 1 Then
								'�p�C���b�g���P���̂�
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower
									DrawSysString(t.x, t.y, "-" & VB6.Format(prev_value - .SP))
									DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(prev_value - .SP) & "���������B")
								End With
							Else
								'�����̃p�C���b�g���Ώ�
								With .MainPilot
									prev_value = .SP
									.SP = .SP + epower \ 5 + epower \ num
									DrawSysString(t.x, t.y, "+" & VB6.Format(prev_value - .SP))
									DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(prev_value - .SP) & "���������B")
								End With
								
								For j = 2 To .CountPilot
									With .Pilot(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(prev_value - .SP) & "���������B")
									End With
								Next 
								
								For j = 1 To .CountSupport
									With .Support(j)
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(prev_value - .SP) & "���������B")
									End With
								Next 
								
								If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
									With .AdditionalSupport
										prev_value = .SP
										.SP = .SP + epower \ 5 + epower \ num
										DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(prev_value - .SP) & "���������B")
									End With
								End If
							End If
						End If
					End With
					
				Case "�C�͑���"
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
								DisplaySysMessage(.Nickname & "��" & Term("�C��", t) & "��" & VB6.Format(.Morale - prev_value) & "���������B")
							End With
						ElseIf elevel < 0 Then 
							With .MainPilot
								DrawSysString(t.x, t.y, "-" & VB6.Format(prev_value - .Morale))
								DisplaySysMessage(.Nickname & "��" & Term("�C��", t) & "��" & VB6.Format(prev_value - .Morale) & "���������B")
							End With
						End If
						
						If .MainPilot.Morale > prev_value Then
							is_useful = True
						End If
					End With
					
				Case "���U"
					With t
						flag = False
						If edata = "" Then
							'�S�Ă̕���̒e������
							For j = 1 To .CountWeapon
								If .Bullet(j) < .MaxBullet(j) Then
									.BulletSupply()
									flag = True
									Exit For
								End If
							Next 
							
							'�e���ƃA�r���e�B�g�p�񐔂̓��������
							If flag Then
								For j = 1 To .CountAbility
									If .IsAbilityClassifiedAs(j, "��") Then
										For k = 1 To .CountWeapon
											If .IsWeaponClassifiedAs(k, "��") And .AbilityLevel(j, "��") = .WeaponLevel(k, "��") Then
												.SetStock(j, .MaxStock(j))
											End If
										Next 
									End If
								Next 
								
								'�e���E�g�p�񐔂̋��L������
								.SyncBullet()
							End If
						Else
							'����̕���̒e���݂̂���
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
							
							'�e���̓��������
							If flag Then
								If .IsWeaponClassifiedAs(w, "��") Then
									For j = 1 To .CountWeapon
										If .IsWeaponClassifiedAs(j, "��") And .WeaponLevel(j, "��") = .WeaponLevel(w, "��") Then
											.SetBullet(j, .MaxBullet(j))
										End If
									Next 
									For j = 1 To .CountAbility
										If .IsAbilityClassifiedAs(j, "��") And .AbilityLevel(j, "��") = .WeaponLevel(w, "��") Then
											.SetStock(j, .MaxStock(j))
										End If
									Next 
								End If
								
								'�e���E�g�p�񐔂̋��L������
								.SyncBullet()
							End If
						End If
						
						If flag Then
							DisplaySysMessage(.Nickname & "�̕����̎g�p�񐔂��񕜂����B")
							If AbilityMaxRange(a) > 0 Then
								is_useful = True
							End If
						End If
					End With
					
				Case "����"
					With t
						If Not is_anime_played Then
							If IsSpellAbility(a) Or IsAbilityClassifiedAs(a, "��") Then
								ShowAnimation("�񕜖��@����")
							End If
						End If
						If edata = "" Then
							'�S�ẴX�e�[�^�X�ُ����
							If .ConditionLifetime("�U���s�\") > 0 Then
								.DeleteCondition("�U���s�\")
								is_useful = True
							End If
							If .ConditionLifetime("�ړ��s�\") > 0 Then
								.DeleteCondition("�ړ��s�\")
								is_useful = True
							End If
							If .ConditionLifetime("���b��") > 0 Then
								.DeleteCondition("���b��")
								is_useful = True
							End If
							If .ConditionLifetime("����") > 0 Then
								.DeleteCondition("����")
								is_useful = True
							End If
							If .ConditionLifetime("���|") > 0 Then
								.DeleteCondition("���|")
								is_useful = True
							End If
							If .ConditionLifetime("�x��") > 0 Then
								.DeleteCondition("�x��")
								is_useful = True
							End If
							If .ConditionLifetime("����m") > 0 Then
								.DeleteCondition("����m")
								is_useful = True
							End If
							If .ConditionLifetime("�]���r") > 0 Then
								.DeleteCondition("�]���r")
								is_useful = True
							End If
							If .ConditionLifetime("�񕜕s�\") > 0 Then
								.DeleteCondition("�񕜕s�\")
								is_useful = True
							End If
							If .ConditionLifetime("�Ή�") > 0 Then
								.DeleteCondition("�Ή�")
								is_useful = True
							End If
							If .ConditionLifetime("����") > 0 Then
								.DeleteCondition("����")
								is_useful = True
							End If
							If .ConditionLifetime("���") > 0 Then
								.DeleteCondition("���")
								is_useful = True
							End If
							If .ConditionLifetime("����") > 0 Then
								.DeleteCondition("����")
								is_useful = True
							End If
							If .ConditionLifetime("��") > 0 Then
								.DeleteCondition("��")
								is_useful = True
							End If
							If .ConditionLifetime("�Ӗ�") > 0 Then
								.DeleteCondition("�Ӗ�")
								is_useful = True
							End If
							If .ConditionLifetime("����") > 0 Then
								.DeleteCondition("����")
								is_useful = True
							End If
							If .ConditionLifetime("����") > 0 Then
								.DeleteCondition("����")
								is_useful = True
							End If
							If .ConditionLifetime("�߈�") > 0 Then
								.DeleteCondition("�߈�")
								is_useful = True
							End If
							'������
							If .ConditionLifetime("�I�[���g�p�s�\") > 0 Then
								.DeleteCondition("�I�[���g�p�s�\")
							End If
							If .ConditionLifetime("���\�͎g�p�s�\") > 0 Then
								.DeleteCondition("���\�͎g�p�s�\")
							End If
							If .ConditionLifetime("�������g�p�s�\") > 0 Then
								.DeleteCondition("�������g�p�s�\")
							End If
							If .ConditionLifetime("�����o�g�p�s�\") > 0 Then
								.DeleteCondition("�����o�g�p�s�\")
							End If
							If .ConditionLifetime("�m�o�����g�p�s�\") > 0 Then
								.DeleteCondition("�m�o�����g�p�s�\")
							End If
							If .ConditionLifetime("��͎g�p�s�\") > 0 Then
								.DeleteCondition("��͎g�p�s�\")
							End If
							If .ConditionLifetime("�p�g�p�s�\") > 0 Then
								.DeleteCondition("�p�g�p�s�\")
							End If
							If .ConditionLifetime("�Z�g�p�s�\") > 0 Then
								.DeleteCondition("�Z�g�p�s�\")
							End If
							j = 1
							Do While j <= .CountCondition
								'��_�A�L���t���͂����ĊO���Ă���܂��B
								If Len(.Condition(j)) > 6 And Right(.Condition(j), 6) = "�����g�p�s�\" And .ConditionLifetime(.Condition(j)) > 0 Then
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
								DisplaySysMessage(.Nickname & "�̏�Ԃ��񕜂����B")
							End If
						Else
							'�w�肳�ꂽ�X�e�[�^�X�ُ�݂̂���
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
									If cname = "���b��" Then
										cname = Term("���b", t) & "��"
									End If
									DisplaySysMessage(.Nickname & "��[" & cname & "]���񕜂����B")
									is_useful = True
								Else
									j = j + 1
								End If
							Loop 
						End If
					End With
					
				Case "�t��"
					With t
						If elevel2 = 0 Then
							'���x���w�肪�Ȃ��ꍇ�͕t�������i�v�I�Ɏ���
							elevel2 = 10000
						Else
							'�����łȂ���΍Œ�P�^�[���͌��ʂ�����
							elevel2 = MaxLng(CShort(elevel2), 1)
						End If
						
						'���ʎ��Ԃ��p�����H
						If .IsConditionSatisfied(LIndex(edata, 1) & "�t��") Then
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
						
						'�G���A�X����`����Ă���H
						If ALDList.IsDefined(ftype) Then
							With ALDList.Item(ftype)
								For j = 1 To .Count
									'�G���A�X�̒�`�ɏ]���ē���\�͒�`��u��������
									ftype2 = .AliasType(j)
									
									If LIndex(.AliasData(j), 1) = "���" Then
										'����\�͂̉��
										If fdata <> "" Then
											ftype2 = LIndex(fdata, 1)
										End If
										flevel2 = DEFAULT_LEVEL
										fdata2 = .AliasData(j)
									Else
										'�ʏ�̓���\��
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
											If InStr(fdata2, "��\��") <> 1 Then
												fdata2 = fdata & " " & ListTail(fdata2, LLength(fdata) + 1)
											End If
										End If
									End If
									
									t.AddCondition(ftype2 & "�t��", elevel2, flevel2, fdata2)
								Next 
							End With
						Else
							.AddCondition(ftype & "�t��", elevel2, flevel, fdata)
						End If
						
						.Update()
						
						If t Is CurrentForm Then
							UpdateMessageForm(t)
						Else
							UpdateMessageForm(t, CurrentForm)
						End If
						Select Case LIndex(edata, 1)
							Case "�ϐ�", "������", "�z��"
								DisplaySysMessage(.Nickname & "��[" & LIndex(edata, 3) & "]�����ɑ΂���[" & LIndex(edata, 1) & "]�\�͂𓾂��B")
							Case "������ʖ�����"
								DisplaySysMessage(.Nickname & "��[" & LIndex(edata, 3) & "]�����ɑ΂��閳�����\�͂𓾂��B")
							Case "�U������"
								DisplaySysMessage(.Nickname & "��[" & LIndex(edata, 3) & "]�̍U�������𓾂��B")
							Case "���틭��"
								DisplaySysMessage(.Nickname & "��" & "����̍U���͂��オ�����B")
							Case "����������"
								DisplaySysMessage(.Nickname & "��" & "����̖��������オ�����B")
							Case "�b�s������"
								DisplaySysMessage(.Nickname & "��" & "����̂b�s�����オ�����B")
							Case "������ʔ���������"
								DisplaySysMessage(.Nickname & "��" & "����̓�����ʔ��������オ�����B")
							Case "�˒�����"
								DisplaySysMessage(.Nickname & "��" & "����̎˒����L�т��B")
							Case "�T�C�Y�ύX"
								DisplaySysMessage(.Nickname & "��" & "�T�C�Y��" & StrConv(LIndex(edata, 3), VbStrConv.Wide) & "�T�C�Y�ɕω������B")
							Case "�p�C���b�g����", "�p�C���b�g�摜", "���̕ύX", "���j�b�g�摜", "�a�f�l"
								'���b�Z�[�W��\�����Ȃ��B
							Case Else
								'�t������\�͖�
								fname = ListIndex(fdata, 1)
								If fname = "" Or fname = "��\��" Then
									If LIndex(edata, 2) <> VB6.Format(DEFAULT_LEVEL) Then
										fname = LIndex(edata, 1) & "Lv" & LIndex(edata, 2)
									Else
										fname = LIndex(edata, 1)
									End If
								End If
								DisplaySysMessage(.Nickname & "��[" & fname & "]�̔\�͂𓾂��B")
						End Select
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "����"
					With t
						If elevel2 = 0 Then
							'���x���w�肪�Ȃ��ꍇ�͕t�������i�v�I�Ɏ���
							elevel2 = 10000
						Else
							'�����łȂ���΍Œ�P�^�[���͌��ʂ�����
							elevel2 = MaxLng(CShort(elevel2), 1)
						End If
						
						'���ʎ��Ԃ��p�����H
						If .IsConditionSatisfied(LIndex(edata, 1) & "����") Then
							GoTo NextLoop
						End If
						
						ftype = LIndex(edata, 1)
						flevel = CDbl(LIndex(edata, 2))
						fdata = ""
						For j = 3 To LLength(edata)
							fdata = fdata & LIndex(edata, j) & " "
						Next 
						fdata = Trim(fdata)
						
						'�G���A�X����`����Ă���H
						If ALDList.IsDefined(ftype) Then
							With ALDList.Item(ftype)
								For j = 1 To .Count
									'�G���A�X�̒�`�ɏ]���ē���\�͒�`��u��������
									ftype2 = .AliasType(i)
									
									If LIndex(.AliasData(j), 1) = "���" Then
										'����\�͂̉��
										If fdata <> "" Then
											ftype2 = LIndex(fdata, 1)
										End If
										flevel2 = DEFAULT_LEVEL
										fdata2 = .AliasData(j)
										t.AddCondition(ftype2 & "�t��", elevel2, flevel2, fdata2)
									Else
										'�ʏ�̓���\��
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
											If InStr(fdata2, "��\��") <> 1 Then
												fdata2 = fdata & " " & ListTail(fdata2, LLength(fdata) + 1)
											End If
										End If
										
										t.AddCondition(ftype2 & "����", elevel2, flevel2, fdata2)
									End If
								Next 
							End With
						Else
							.AddCondition(ftype & "����", elevel2, flevel, fdata)
						End If
						
						.Update()
						
						If t Is CurrentForm Then
							UpdateMessageForm(t)
						Else
							UpdateMessageForm(t, CurrentForm)
						End If
						
						'��������\�͖�
						fname = LIndex(edata, 3)
						If fname = "" Or fname = "��\��" Then
							fname = LIndex(edata, 1)
						End If
						If t.SkillName0(fname) <> "��\��" Then
							fname = t.SkillName0(fname)
						End If
						DisplaySysMessage(.Nickname & "��[" & fname & "]���x����" & LIndex(edata, 2) & "�オ�����B")
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "���"
					With t
						If elevel2 = 0 Then
							'���x���w�肪�Ȃ��ꍇ�͕t�������i�v�I�Ɏ���
							elevel2 = 10000
						Else
							'�����łȂ���΍Œ�P�^�[���͏�Ԃ�����
							elevel = MaxLng(CShort(elevel2), 1)
						End If
						
						'���ʎ��Ԃ��p�����H
						If .IsConditionSatisfied(edata) Then
							GoTo NextLoop
						End If
						
						.AddCondition(edata, elevel2)
						
						'��Ԕ����A�j���[�V�����\��
						If Not IsAnimationDefined(aname & "(����)") And Not IsAnimationDefined(aname) Then
							Select Case edata
								Case "�U���͂t�o", "�h��͂t�o", "�^�����t�o", "�ړ��͂t�o", "����m"
									ShowAnimation(edata & "����")
							End Select
						End If
						
						Select Case edata
							Case "���b��"
								cname = Term("���b", t) & "��"
							Case "�^�����t�o"
								cname = Term("�^����", t) & "�t�o"
							Case "�^�����c�n�v�m"
								cname = Term("�^����", t) & "�c�n�v�m"
							Case "�ړ��͂t�o"
								cname = Term("�ړ���", t) & "�t�o"
							Case "�ړ��͂c�n�v�m"
								cname = Term("�ړ���", t) & "�c�n�v�m"
							Case Else
								cname = edata
						End Select
						
						DisplaySysMessage(.Nickname & "��" & cname & "�̏�ԂɂȂ����B")
						
						If AbilityMaxRange(a) > 0 Then
							is_useful = True
						End If
					End With
					
				Case "����"
					UpdateMessageForm(CurrentForm)
					If Not UDList.IsDefined(edata) Then
						ErrorMessage(edata & "�̃f�[�^����`����Ă��܂���")
						Exit Function
					End If
					
					pname = UDList.Item(edata).FeatureData("�ǉ��p�C���b�g")
					If Not PDList.IsDefined(pname) Then
						ErrorMessage("�ǉ��p�C���b�g�u" & pname & "�v�̃f�[�^������܂���")
						Exit Function
					End If
					
					'�����������j�b�g��z�u������W�����肷��B
					'�ł��߂��G���j�b�g�̕����Ƀ��j�b�g��z�u����B
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
						If InStr(PDList.Item(pname).Name, "(�U�R)") > 0 Or InStr(PDList.Item(pname).Name, "(�ėp)") > 0 Then
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
						
						If Party = "����" Then
							If LIndex(u.FeatureData("�������j�b�g"), 2) = "�m�o�b" Then
								u.ChangeParty("�m�o�b")
							End If
						End If
						
						With u
							.Summoner = CurrentForm
							.FullRecover()
							.Mode = MainPilot.ID
							.UsedAction = 0
							If .IsFeatureAvailable("��������") Then
								.AddCondition("�c�莞��", CShort(.FeatureData("��������")))
							End If
							
							If .IsMessageDefined("���i") Then
								If Not frmMessage.Visible Then
									OpenMessageForm(Me)
								End If
								.PilotMessage("���i")
							End If
							
							'���j�b�g��z�u
							If MapDataForUnit(tx, ty) Is Nothing And .IsAbleToEnter(tx, ty) Then
								.StandBy(tx, ty, "�o��")
							ElseIf MapDataForUnit(tx2, ty2) Is Nothing And .IsAbleToEnter(tx2, ty2) Then 
								.StandBy(tx2, ty2, "�o��")
							Else
								.StandBy(x, y, "�o��")
							End If
							
							'�����Ɣz�u�ł����H
							If .Status_Renamed = "�ҋ@" Then
								'�󂢂��ꏊ���Ȃ��o���o���Ȃ������ꍇ
								DisplaySysMessage(Nickname & "��" & .Nickname & "�̏����Ɏ��s�����B")
								DeleteServant(.ID)
								.Status_Renamed = "�j��"
							End If
						End With
					Next 
					
				Case "�ϐg"
					'���ɕϐg���Ă���ꍇ�͕ϐg�o���Ȃ�
					If t.IsFeatureAvailable("�m�[�}�����[�h") Then
						GoTo NextLoop
					End If
					
					buf = t.Name
					
					t.Transform(LIndex(edata, 1))
					t = t.CurrentForm
					If elevel2 > 0 Then
						t.AddCondition("�c�莞��", MaxLng(CShort(elevel2), 1))
					End If
					
					For j = 2 To LLength(edata)
						buf = buf & " " & LIndex(edata, j)
					Next 
					t.AddCondition("�m�[�}�����[�h�t��", -1, 1, buf)
					
					'�ϐg�����ꍇ�͂����ŏI���
					Exit For
					
				Case "�\�̓R�s�["
					'���ɕϐg���Ă���ꍇ�͔\�̓R�s�[�o���Ȃ�
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						GoTo NextLoop
					End If
					
					Transform((t.Name))
					
					With CurrentForm
						If elevel2 > 0 Then
							.AddCondition("�c�莞��", MaxLng(CShort(elevel2), 1))
						End If
						
						'���̌`�Ԃɖ߂��悤�ɐݒ�
						buf = Name
						For j = 1 To LLength(edata)
							buf = buf & " " & LIndex(edata, j)
						Next 
						.AddCondition("�m�[�}�����[�h�t��", -1, 1, buf)
						.AddCondition("�\�̓R�s�[", -1)
						
						'�R�s�[���̃p�C���b�g�摜�ƃ��b�Z�[�W���g���悤�ɐݒ�
						.AddCondition("�p�C���b�g�摜", -1, 0, "��\�� " & t.MainPilot.Bitmap)
						.AddCondition("���b�Z�[�W", -1, 0, "��\�� " & t.MainPilot.MessageType)
					End With
					
					'�\�̓R�s�[�����ꍇ�͂����ŏI���
					ExecuteAbility = True
					RestoreSelections()
					Exit Function
					
				Case "�čs��"
					If Not t Is CurrentForm Then
						If t.Action = 0 And t.MaxAction > 0 Then
							If t.UsedAction > t.MaxAction Then
								t.UsedAction = t.MaxAction
							End If
							t.UsedAction = t.UsedAction - 1
							DisplaySysMessage(t.Nickname & "���s���\�ɂ����B")
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
		
		'�I���󋵂𕜌�
		RestoreSelections()
		
		'�}�b�v�A�r���e�B�̏ꍇ�A����ȍ~�̏����͕K�v�Ȃ�
		If is_map_ability Then
			Exit Function
		End If
		
		'���̋Z�̃p�[�g�i�[�̒e�����d�m�̏���
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountAbility
					'�p�[�g�i�[�������̃A�r���e�B�������Ă���΂��̃A�r���e�B�̃f�[�^���g��
					If .Ability(j).Name = aname Then
						.UseAbility(j)
						If .IsAbilityClassifiedAs(j, "��") Then
							If .IsFeatureAvailable("�p�[�c����") Then
								uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
						ElseIf .IsAbilityClassifiedAs(j, "��") And .HP = 0 Then 
							.Die()
						ElseIf .IsAbilityClassifiedAs(j, "��") Then 
							If .IsFeatureAvailable("�ό`�Z") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "�ό`�Z" And LIndex(.FeatureData(k), 1) = aname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("�m�[�}�����[�h") Then
										uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("�m�[�}�����[�h") Then 
								uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'�����̃A�r���e�B���Ȃ������ꍇ�͎����̃f�[�^���g���ď���
				If j > .CountAbility Then
					If Ability(a).ENConsumption > 0 Then
						.EN = .EN - AbilityENConsumption(a)
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						.AddCondition("����", 1)
					End If
					If IsAbilityClassifiedAs(a, "�b") And .IsConditionSatisfied("�`���[�W����") Then
						.DeleteCondition("�`���[�W����")
					End If
					If IsAbilityClassifiedAs(a, "�C") Then
						.IncreaseMorale(-5 * AbilityLevel(a, "�C"))
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "��")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsAbilityClassifiedAs(a, "�v") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "�v")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						.HP = MaxLng(.HP - .MaxHP * AbilityLevel(a, "��") \ 10, 0)
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						If .IsFeatureAvailable("�p�[�c����") Then
							uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
					ElseIf IsAbilityClassifiedAs(a, "��") And .HP = 0 Then 
						.Die()
					ElseIf IsAbilityClassifiedAs(a, "��") Then 
						If .IsFeatureAvailable("�m�[�}�����[�h") Then
							uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'�ϐg�����ꍇ
		If Status_Renamed = "���`��" Then
			With CurrentForm
				'�g���̂ăA�C�e���ɂ��ϐg�̏���
				For i = 1 To .CountAbility
					If .Ability(i).Name = aname Then
						'�A�C�e��������
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
				
				'���E�H
				If .HP = 0 Then
					.Die()
				End If
			End With
			
			'WaitCommand�ɂ���ʃN���A���s���Ȃ��̂�
			RedrawScreen()
			Exit Function
		End If
		
		'�o���l�̊l��
		If is_useful And Not is_event And Not IsOptionDefined("�A�r���e�B�o���l����") Then
			GetExp(t, "�A�r���e�B")
			If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(t, "�A�r���e�B", "�p�[�g�i�[")
				Next 
			End If
		End If
		
		'�ȉ��̌��ʂ̓A�r���e�B�f�[�^���ω�����ꍇ�����邽�ߓ����ɂ͓K������Ȃ�
		
		'�����Z
		If IsAbilityClassifiedAs(a, "��") Then
			If IsFeatureAvailable("�p�[�c����") Then
				uname = LIndex(FeatureData("�p�[�c����"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("�p�[�c����")
					If IsSysMessageDefined("�j�󎞕���(" & Name & ")") Then
						SysMessage("�j�󎞕���(" & Name & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
						SysMessage("�j�󎞕���(" & fname & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���") Then 
						SysMessage("�j�󎞕���")
					ElseIf IsSysMessageDefined("����(" & Name & ")") Then 
						SysMessage("����(" & Name & ")")
					ElseIf IsSysMessageDefined("����(" & fname & ")") Then 
						SysMessage("����(" & fname & ")")
					ElseIf IsSysMessageDefined("����") Then 
						SysMessage("����")
					Else
						DisplaySysMessage(Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
					End If
				Else
					Die()
				End If
			Else
				Die()
			End If
			
			'�g�o����A�r���e�B�Ŏ��E
		ElseIf IsAbilityClassifiedAs(a, "��") And HP = 0 Then 
			Die()
			
			'�ό`�Z
		ElseIf IsAbilityClassifiedAs(a, "��") Then 
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = Ability(a).Name Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'�A�C�e��������
		ElseIf Ability(a).IsItem And Stock(a) = 0 And MaxStock(a) > 0 Then 
			'�A�C�e�����폜
			num = Data.CountAbility
			num = num + MainPilot.Data.CountAbility
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountAbility
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountAbility
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		'�퓬�A�j���I������
		If IsAnimationDefined(aname & "(�I��)") Then
			PlayAnimation(aname & "(�I��)")
		ElseIf IsAnimationDefined("�I��") Then 
			PlayAnimation("�I��")
		End If
		' ADD END MARGE
		
		With CurrentForm
			'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
			If .IsConditionSatisfied("���j�b�g�摜") Then
				.DeleteCondition("���j�b�g�摜")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("��\���t��") Then
				.DeleteCondition("��\���t��")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
		End With
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("���j�b�g�摜") Then
					.DeleteCondition("���j�b�g�摜")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("��\���t��") Then
					.DeleteCondition("��\���t��")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
	End Function
	
	'�}�b�v�A�r���e�B a �� (tx,ty) �Ɏg�p
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
			'�}�b�v�U���̎g�p�C�x���g
			HandleEvent("�g�p", MainPilot.ID, aname)
			If IsScenarioFinished Then
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				Exit Sub
			End If
		End If
		
		'���ʔ͈͂�ݒ�
		min_range = AbilityMinRange(a)
		max_range = AbilityMaxRange(a)
		If IsAbilityClassifiedAs(a, "�l��") Then
			If ty < y Then
				AreaInLine(x, y, min_range, max_range, "N")
			ElseIf ty > y Then 
				AreaInLine(x, y, min_range, max_range, "S")
			ElseIf tx < x Then 
				AreaInLine(x, y, min_range, max_range, "W")
			Else
				AreaInLine(x, y, min_range, max_range, "E")
			End If
		ElseIf IsAbilityClassifiedAs(a, "�l�g") Then 
			If ty < y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then
				AreaInCone(x, y, min_range, max_range, "N")
			ElseIf ty > y And System.Math.Abs(y - ty) > System.Math.Abs(x - tx) Then 
				AreaInCone(x, y, min_range, max_range, "S")
			ElseIf tx < x And System.Math.Abs(x - tx) > System.Math.Abs(y - ty) Then 
				AreaInCone(x, y, min_range, max_range, "W")
			Else
				AreaInCone(x, y, min_range, max_range, "E")
			End If
		ElseIf IsAbilityClassifiedAs(a, "�l��") Then 
			If ty < y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then
				AreaInSector(x, y, min_range, max_range, "N", AbilityLevel(a, "�l��"))
			ElseIf ty > y And System.Math.Abs(y - ty) >= System.Math.Abs(x - tx) Then 
				AreaInSector(x, y, min_range, max_range, "S", AbilityLevel(a, "�l��"))
			ElseIf tx < x And System.Math.Abs(x - tx) >= System.Math.Abs(y - ty) Then 
				AreaInSector(x, y, min_range, max_range, "W", AbilityLevel(a, "�l��"))
			Else
				AreaInSector(x, y, min_range, max_range, "E", AbilityLevel(a, "�l��"))
			End If
		ElseIf IsAbilityClassifiedAs(a, "�l��") Then 
			AreaInRange(tx, ty, AbilityLevel(a, "�l��"), 1, "���ׂ�")
		ElseIf IsAbilityClassifiedAs(a, "�l�S") Then 
			AreaInRange(x, y, max_range, min_range, "���ׂ�")
		ElseIf IsAbilityClassifiedAs(a, "�l��") Or IsAbilityClassifiedAs(a, "�l��") Then 
			AreaInPointToPoint(x, y, tx, ty)
		End If
		
		'���j�b�g������}�X�̏���
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				If Not MaskData(i, j) Then
					t = MapDataForUnit(i, j)
					If Not t Is Nothing Then
						'�L���H
						If IsAbilityEffective(a, t) Then
							MaskData(i, j) = False
						Else
							MaskData(i, j) = True
						End If
					End If
				End If
			Next 
		Next 
		
		'�x����p�A�r���e�B�͎����ɂ͎g�p�ł��Ȃ�
		If IsAbilityClassifiedAs(a, "��") Then
			MaskData(x, y) = True
		End If
		
		'�}�b�v�A�r���e�B�̉e�����󂯂郆�j�b�g�̃��X�g���쐬
		ReDim targets(0)
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'�}�b�v�A�r���e�B�̉e���������邩�`�F�b�N
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
		
		'�A�r���e�B���s�̋N�_��ݒ�
		If IsAbilityClassifiedAs(a, "�l��") Then
			rx = tx
			ry = ty
		Else
			rx = x
			ry = y
		End If
		
		'�N�_����̋����ɉ����ĕ��בւ�
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
		
		'���̋Z
		Dim TmpMaskData() As Boolean
		If IsAbilityClassifiedAs(a, "��") Then
			
			'���̋Z�̃p�[�g�i�[�̃n�C���C�g�\��
			'MaskData��ۑ����Ďg�p���Ă���
			ReDim TmpMaskData(MapWidth, MapHeight)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					TmpMaskData(i, j) = MaskData(i, j)
				Next 
			Next 
			
			CombinationPartner("�A�r���e�B", a, partners)
			
			'�p�[�g�i�[���j�b�g�̓}�X�N������
			For i = 1 To UBound(partners)
				With partners(i)
					MaskData(.x, .y) = False
					TmpMaskData(.x, .y) = True
				End With
			Next 
			
			MaskScreen()
			
			'�}�X�N�𕜌�
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
		
		'���݂̑I���󋵂��Z�[�u
		SaveSelections()
		
		'�I����e��؂�ւ�
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		SelectedAbility = a
		SelectedAbilityName = Ability(a).Name
		SelectedX = tx
		SelectedY = ty
		
		'�ςȁu�΁`�v���b�Z�[�W���\������Ȃ��悤�Ƀ^�[�Q�b�g���I�t
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		
		'�}�b�v�A�r���e�B�J�n�̃��b�Z�[�W���������
		If IsAnimationDefined(aname & "(����)") Then
			PlayAnimation(aname & "(����)")
		End If
		If IsMessageDefined("������(" & aname & ")") Then
			PilotMessage("������(" & aname & ")")
		End If
		PilotMessage(aname, "�A�r���e�B")
		If IsAnimationDefined(aname & "(�g�p)") Then
			PlayAnimation(aname & "(�g�p)", "", True)
		Else
			SpecialEffect(aname, "", True)
		End If
		
		'�d�m����g�p�񐔌���
		UseAbility(a)
		
		UpdateMessageForm(Me)
		
		Select Case UBound(partners)
			Case 0
				'�ʏ�
				msg = Nickname & "��"
			Case 1
				'�Q�̍���
				If Nickname <> partners(1).Nickname Then
					msg = Nickname & "��[" & partners(1).Nickname & "]�Ƌ���"
				ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
					msg = MainPilot.Nickname & "��[" & partners(1).MainPilot.Nickname & "]��[" & Nickname & "]��"
				Else
					msg = Nickname & "�B��"
				End If
			Case 2
				'�R�̍���
				If Nickname <> partners(1).Nickname Then
					msg = Nickname & "��[" & partners(1).Nickname & "]�A[" & partners(2).Nickname & "]�Ƌ���"
				ElseIf MainPilot.Nickname <> partners(1).MainPilot.Nickname Then 
					msg = MainPilot.Nickname & "�A[" & partners(1).MainPilot.Nickname & "]�A[" & partners(2).MainPilot.Nickname & "]��[" & Nickname & "]��"
				Else
					msg = Nickname & "�B��"
				End If
			Case Else
				'�R�̈ȏ�
				msg = Nickname & "�B��"
		End Select
		
		If IsSpellAbility(a) Then
			If Right(anickname, 2) = "����" Then
				msg = msg & "[" & anickname & "]���������B"
			ElseIf Right(anickname, 2) = "�̏�" Then 
				msg = msg & "[" & Left(anickname, Len(anickname) - 2) & "]�̎������������B"
			Else
				msg = msg & "[" & anickname & "]�̎������������B"
			End If
		ElseIf Right(anickname, 1) = "��" Then 
			msg = msg & "[" & anickname & "]���̂����B"
		ElseIf Right(anickname, 2) = "�x��" Then 
			msg = msg & "[" & anickname & "]��x�����B"
		Else
			msg = msg & "[" & anickname & "]���g�����B"
		End If
		
		If IsSysMessageDefined(aname) Then
			'�u�A�r���e�B��(���)�v�̃��b�Z�[�W���g�p
			SysMessage(aname)
		ElseIf IsSysMessageDefined("�A�r���e�B") Then 
			'�u�A�r���e�B(���)�v�̃��b�Z�[�W���g�p
			SysMessage("�A�r���e�B")
		Else
			DisplaySysMessage(msg)
		End If
		
		'�I���󋵂𕜌�
		RestoreSelections()
		
		'�A�r���e�B�̎g�p�Ɏ��s�H
		If Dice(10) <= AbilityLevel(a, "��") Then
			DisplaySysMessage("���������������Ȃ������c")
			GoTo Finish
		End If
		
		'�g�p�����j�b�g�� SelectedTarget �ɐݒ肵�Ă��Ȃ��Ƃ����Ȃ�
		SelectedTarget = Me
		
		'�e���j�b�g�ɃA�r���e�B���g�p
		'UPGRADE_NOTE: �I�u�W�F�N�g max_lv_t ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		max_lv_t = Nothing
		For i = 1 To UBound(targets)
			t = targets(i).CurrentForm
			If t.Status_Renamed = "�o��" Then
				If t Is Me Then
					UpdateMessageForm(Me)
				Else
					UpdateMessageForm(t, Me)
				End If
				
				If ExecuteAbility(a, t, True) Then
					t = t.CurrentForm
					
					is_useful = True
					
					'�l���o���l�Z�o�p�Ƀ��C���p�C���b�g�̃��x�����ł�����
					'���j�b�g�����߂Ă���
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
		'�퓬�A�j���I������
		If IsAnimationDefined(aname & "(�I��)") Then
			PlayAnimation(aname & "(�I��)")
		ElseIf IsAnimationDefined("�I��") Then 
			PlayAnimation("�I��")
		End If
		' ADD END MARGE
		
		With CurrentForm
			'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
			If .IsConditionSatisfied("���j�b�g�摜") Then
				.DeleteCondition("���j�b�g�摜")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
			If .IsConditionSatisfied("��\���t��") Then
				.DeleteCondition("��\���t��")
				.BitmapID = MakeUnitBitmap(CurrentForm)
				PaintUnitBitmap(CurrentForm)
			End If
		End With
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				If .IsConditionSatisfied("���j�b�g�摜") Then
					.DeleteCondition("���j�b�g�摜")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
				If .IsConditionSatisfied("��\���t��") Then
					.DeleteCondition("��\���t��")
					.BitmapID = MakeUnitBitmap(partners(i).CurrentForm)
					PaintUnitBitmap(partners(i).CurrentForm)
				End If
			End With
		Next 
		
		'�l�������o���l�̕\��
		If is_useful And Not is_event And Not IsOptionDefined("�A�r���e�B�o���l����") Then
			GetExp(max_lv_t, "�A�r���e�B")
			If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(Nothing, "�A�r���e�B", "�p�[�g�i�[")
				Next 
			End If
		End If
		
		'���̋Z�̃p�[�g�i�[�̒e�����d�m�̏���
		For i = 1 To UBound(partners)
			With partners(i).CurrentForm
				For j = 1 To .CountAbility
					'�p�[�g�i�[�������̃A�r���e�B�������Ă���΂��̃A�r���e�B�̃f�[�^���g��
					If .Ability(j).Name = aname Then
						.UseAbility(j)
						If .IsAbilityClassifiedAs(j, "��") Then
							If .IsFeatureAvailable("�p�[�c����") Then
								uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
						ElseIf .IsAbilityClassifiedAs(j, "��") And .HP = 0 Then 
							.Die()
						ElseIf .IsAbilityClassifiedAs(j, "��") Then 
							If .IsFeatureAvailable("�ό`�Z") Then
								For k = 1 To .CountFeature
									If .Feature(k) = "�ό`�Z" And LIndex(.FeatureData(k), 1) = aname Then
										uname = LIndex(.FeatureData(k), 2)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
										Exit For
									End If
								Next 
								If uname <> .CurrentForm.Name Then
									If .IsFeatureAvailable("�m�[�}�����[�h") Then
										uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
										If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
											.Transform(uname)
										End If
									End If
								End If
							ElseIf .IsFeatureAvailable("�m�[�}�����[�h") Then 
								uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
								If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
									.Transform(uname)
								End If
							End If
						End If
						Exit For
					End If
				Next 
				
				'�����̃A�r���e�B���Ȃ������ꍇ�͎����̃f�[�^���g���ď���
				If j > .CountAbility Then
					If Ability(a).ENConsumption > 0 Then
						.EN = .EN - AbilityENConsumption(a)
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						.AddCondition("����", 1)
					End If
					If IsAbilityClassifiedAs(a, "�b") And .IsConditionSatisfied("�`���[�W����") Then
						.DeleteCondition("�`���[�W����")
					End If
					If IsAbilityClassifiedAs(a, "�C") Then
						.IncreaseMorale(-5 * AbilityLevel(a, "�C"))
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "��")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					ElseIf IsAbilityClassifiedAs(a, "�v") Then 
						hp_ratio = 100 * .HP / .MaxHP
						en_ratio = 100 * .EN / .MaxEN
						
						.MainPilot.Plana = .MainPilot.Plana - 5 * AbilityLevel(a, "�v")
						
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						.HP = MaxLng(.HP - .MaxHP * AbilityLevel(a, "��") \ 10, 0)
					End If
					If IsAbilityClassifiedAs(a, "��") Then
						If .IsFeatureAvailable("�p�[�c����") Then
							uname = LIndex(.FeatureData("�p�[�c����"), 2)
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
					ElseIf IsAbilityClassifiedAs(a, "��") And .HP = 0 Then 
						.Die()
					ElseIf IsAbilityClassifiedAs(a, "��") Then 
						If .IsFeatureAvailable("�m�[�}�����[�h") Then
							uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
							End If
						End If
					End If
				End If
			End With
		Next 
		
		'�ړ��^�}�b�v�A�r���e�B�ɂ��ړ�
		If IsAbilityClassifiedAs(a, "�l��") Then
			Jump(tx, ty)
		End If
		
Finish: 
		
		'�ȉ��̌��ʂ̓A�r���e�B�f�[�^���ω�����\�������邽�߁A�����ɂ͓K�p����Ȃ�
		
		'�����̏���
		If IsAbilityClassifiedAs(a, "��") Then
			If IsFeatureAvailable("�p�[�c����") Then
				'�p�[�c���̂������j�b�g���������鎞�̓p�[�c�𕪗����邾��
				uname = LIndex(FeatureData("�p�[�c����"), 2)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
					With CurrentForm
						.HP = .MaxHP
						.UsedAction = .MaxAction
					End With
					fname = FeatureName("�p�[�c����")
					If IsSysMessageDefined("�j�󎞕���(" & Name & ")") Then
						SysMessage("�j�󎞕���(" & Name & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
						SysMessage("�j�󎞕���(" & fname & ")")
					ElseIf IsSysMessageDefined("�j�󎞕���") Then 
						SysMessage("�j�󎞕���")
					ElseIf IsSysMessageDefined("����(" & Name & ")") Then 
						SysMessage("����(" & Name & ")")
					ElseIf IsSysMessageDefined("����(" & fname & ")") Then 
						SysMessage("����(" & fname & ")")
					ElseIf IsSysMessageDefined("����") Then 
						SysMessage("����")
					Else
						DisplaySysMessage(Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
					End If
				Else
					'�������A�p�[�c�����ł��Ȃ��n�`�ł͂��̂܂܎���
					Die()
					If Not is_event Then
						HandleEvent("�j��", MainPilot.ID)
						If IsScenarioFinished Then
							Exit Sub
						End If
					End If
				End If
			Else
				Die()
				If Not is_event Then
					HandleEvent("�j��", MainPilot.ID)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
			End If
			
			'�g�o����A�r���e�B�Ŏ��E�����ꍇ
		ElseIf IsAbilityClassifiedAs(a, "��") And HP = 0 Then 
			Die()
			If Not is_event Then
				HandleEvent("�j��", MainPilot.ID)
				If IsScenarioFinished Then
					Exit Sub
				End If
			End If
			
			'�ό`�Z
		ElseIf IsAbilityClassifiedAs(a, "��") Then 
			If IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To CountFeature
					If Feature(i) = "�ό`�Z" And LIndex(FeatureData(i), 1) = Ability(a).Name Then
						uname = LIndex(FeatureData(i), 2)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
						Exit For
					End If
				Next 
				If uname <> CurrentForm.Name Then
					If IsFeatureAvailable("�m�[�}�����[�h") Then
						uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
						If OtherForm(uname).IsAbleToEnter(x, y) Then
							Transform(uname)
						End If
					End If
				End If
			ElseIf IsFeatureAvailable("�m�[�}�����[�h") Then 
				uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
				If OtherForm(uname).IsAbleToEnter(x, y) Then
					Transform(uname)
				End If
			End If
			
			'�A�C�e��������
		ElseIf Ability(a).IsItem And Stock(a) = 0 And MaxStock(a) > 0 Then 
			'�A�C�e�����폜
			num = Data.CountAbility
			num = num + MainPilot.Data.CountAbility
			For i = 2 To CountPilot
				num = num + Pilot(i).Data.CountAbility
			Next 
			For i = 2 To CountSupport
				num = num + Support(i).Data.CountAbility
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		
		'�g�p��C�x���g
		If Not is_event Then
			HandleEvent("�g�p��", CurrentForm.MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		
		CloseMessageForm()
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎����������`�F�b�N
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
	End Sub
	
	'�A�r���e�B�̎g�p�ɂ��d�m�A�g�p�񐔂̏�����s��
	Public Sub UseAbility(ByVal a As Short)
		Dim i, lv As Short
		Dim hp_ratio, en_ratio As Double
		
		If Ability(a).ENConsumption > 0 Then
			EN = EN - AbilityENConsumption(a)
		End If
		
		If Ability(a).Stock > 0 Then
			SetStock(a, Stock(a) - 1)
			
			'��Ďg�p
			If IsAbilityClassifiedAs(a, "��") Then
				For i = 1 To UBound(dblStock)
					SetStock(i, MinLng(MaxStock(i) * Stock(a) \ MaxStock(a), Stock(i)))
				Next 
			Else
				For i = 1 To UBound(dblStock)
					If IsAbilityClassifiedAs(i, "��") Then
						SetStock(i, MinLng(CShort(MaxStock(i) * Stock(a) \ MaxStock(a) + 0.49999), Stock(i)))
					End If
				Next 
			End If
			
			'�e���E�g�p�񐔋��L�̏���
			SyncBullet()
		End If
		
		'���ՋZ
		If IsAbilityClassifiedAs(a, "��") Then
			AddCondition("����", 1)
		End If
		
		'�S�d�m����A�r���e�B
		If IsAbilityClassifiedAs(a, "�s") Then
			EN = 0
		End If
		
		'�`���[�W���A�r���e�B
		If IsAbilityClassifiedAs(a, "�b") And IsConditionSatisfied("�`���[�W����") Then
			DeleteCondition("�`���[�W����")
		End If
		
		'�����[�U���A�r���e�B
		If AbilityLevel(a, "�`") > 0 Then
			AddCondition(AbilityNickname(a) & "�[�U��", AbilityLevel(a, "�`"))
		End If
		
		'�C�͂�����
		If IsAbilityClassifiedAs(a, "�C") Then
			IncreaseMorale(-5 * AbilityLevel(a, "�C"))
		End If
		
		'��͂̏���
		If IsAbilityClassifiedAs(a, "��") Then
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * AbilityLevel(a, "��")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		ElseIf IsAbilityClassifiedAs(a, "�v") Then 
			hp_ratio = 100 * HP / MaxHP
			en_ratio = 100 * EN / MaxEN
			
			MainPilot.Plana = MainPilot.Plana - 5 * AbilityLevel(a, "�v")
			
			HP = MaxHP * hp_ratio / 100
			EN = MaxEN * en_ratio / 100
		End If
		
		'��������A�r���e�B
		If Party = "����" Then
			If IsAbilityClassifiedAs(a, "�K") Then
				IncrMoney(-MaxLng(AbilityLevel(a, "�K"), 1) * Value \ 10)
			End If
		End If
		
		'�g�o����A�r���e�B
		If IsAbilityClassifiedAs(a, "��") Then
			HP = MaxLng(HP - MaxHP * AbilityLevel(a, "��") \ 10, 0)
		End If
	End Sub
	
	
	' === �A�C�e���֘A���� ===
	
	'�A�C�e�������\��
	Public Function MaxItemNum() As Short
		Dim i As Short
		
		MaxItemNum = Data.ItemNum
		If IsFeatureAvailable("�n�[�h�|�C���g") Then
			For i = 1 To CountFeature
				If Feature(i) = "�n�[�h�|�C���g" And (FeatureData(i) = "�����p�[�c" Or FeatureData(i) = "�A�C�e��") Then
					MaxItemNum = MaxItemNum + FeatureLevel(i)
					Exit For
				End If
			Next 
		End If
	End Function
	
	'�������Ă���A�C�e���̑���
	Public Function CountItem() As Short
		CountItem = colItem.Count()
	End Function
	
	'�A�C�e��
	Public Function Item(ByRef Index As Object) As Item
		Dim itm As Item
		
		On Error GoTo ErrorHandler
		
		Item = colItem.Item(Index)
		
		Exit Function
		
ErrorHandler: 
		'������Ȃ���΃A�C�e�����Ō���
		For	Each itm In colItem
			'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If itm.Name = Index Then
				Item = itm
				Exit Function
			End If
		Next itm
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'�A�C�e���𑕔�
	Public Sub AddItem(ByRef itm As Item, Optional ByVal without_refresh As Boolean = False)
		Dim i, num As Short
		Dim itm2 As Item
		Dim empty_slot As Short
		Dim found_item As Boolean
		
		'���ɑ������Ă����炻�̂܂܏I��
		If itm.Unit Is Me Then
			Exit Sub
		End If
		
		'�C�x���g��p�A�C�e���͑�����������Ȃ�
		If itm.Class_Renamed() = "�Œ�" Then
			If itm.IsFeatureAvailable("��\��") Then
				GoTo EquipItem
			End If
		End If
		
		'������������Ȃ��ꍇ�Ɍ��̃A�C�e�����O��
		Select Case itm.Part
			Case "�����p�[�c", "�A�C�e��"
				If itm.FeatureData("�n�[�h�|�C���g") <> "�����p�[�c" And itm.FeatureData("�n�[�h�|�C���g") <> "�A�C�e��" Then
					'�������Ă��鋭���p�[�c�����J�E���g
					num = 0
					For	Each itm2 In colItem
						With itm2
							If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
								num = num + .Size
							End If
						End With
					Next itm2
					
					'��^�A�C�e���̏ꍇ�͗]���ɊO��
					num = num + itm.FeatureLevel("��^�A�C�e��")
					
					'���ꂩ���O���Ȃ���΂Ȃ�Ȃ��ꍇ
					Do While num >= MaxItemNum And num > 0
						found_item = False
						
						'�܂��̓n�[�h�|�C���g�������Ȃ����̂���
						For	Each itm2 In colItem
							With itm2
								If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
									If Not .IsFeatureAvailable("�n�[�h�|�C���g") Then
										num = num - .Size
										If Party0 <> "����" Then
											.Exist = False
										End If
										DeleteItem(.ID)
										'UPGRADE_NOTE: �I�u�W�F�N�g itm2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
										itm2 = Nothing
										found_item = True
										Exit For
									End If
								End If
							End With
						Next itm2
						
						'�n�[�h�|�C���g�t���̂��̂����Ȃ��ꍇ
						If Not itm2 Is Nothing Then
							num = num - Item(1).Size
							If Party0 <> "����" Then
								Item(1).Exist = False
							End If
							DeleteItem(1)
							found_item = True
						End If
						
						If Not found_item Then
							'�O����A�C�e�����Ȃ�
							Exit Do
						End If
					Loop 
					
					If MaxItemNum = 0 Then
						'�����o���܂���c
						Exit Sub
					End If
				End If
				
			Case "����"
				For	Each itm2 In colItem
					With itm2
						If .Part = "����" Or .Part = "�Ў�" Or .Part = "��" Then
							If Party0 <> "����" Then
								.Exist = False
							End If
							DeleteItem(.ID)
							Exit For
						End If
					End With
				Next itm2
				
			Case "�Ў�"
				If IsFeatureAvailable("���藘��") Then
					num = 0
					For	Each itm2 In colItem
						With itm2
							Select Case .Part
								Case "����"
									If Party0 <> "����" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								Case "�Ў�", "��"
									num = num + 1
									If num > 1 Then
										If Party0 <> "����" Then
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
								Case "����", "�Ў�"
									If Party0 <> "����" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
							End Select
						End With
					Next itm2
				End If
				
			Case "��"
				For	Each itm2 In colItem
					With itm2
						Select Case .Part
							Case "����", "��"
								If Party0 <> "����" Then
									.Exist = False
								End If
								DeleteItem(.ID)
								Exit For
							Case "�Ў�"
								i = i + 1
								If i > 1 Then
									If Party0 <> "����" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								End If
						End Select
					End With
				Next itm2
				
			Case "����"
				For	Each itm2 In colItem
					With itm2
						If .Part = "����" Or .Part = "��" Then
							If Party0 <> "����" Then
								.Exist = False
							End If
							DeleteItem(.ID)
						End If
					End With
				Next itm2
				
			Case "��"
				num = 0
				For	Each itm2 In colItem
					With itm2
						Select Case .Part
							Case "����"
								If Party0 <> "����" Then
									.Exist = False
								End If
								DeleteItem(.ID)
								Exit For
							Case "��"
								num = num + 1
								If num > 1 Then
									If Party0 <> "����" Then
										.Exist = False
									End If
									DeleteItem(.ID)
									Exit For
								End If
						End Select
					End With
				Next itm2
				
			Case "��\��"
				'���������u��\���v�̃A�C�e���͑������ɐ����Ȃ�
				
			Case Else
				'�n�[�h�|�C���g�ɑ�������ꍇ
				For i = 1 To CountFeature
					If Feature(i) = "�n�[�h�|�C���g" And FeatureData(i) = itm.Part Then
						'�܂��󂫃X���b�g�����v�Z
						empty_slot = ItemSlotSize(itm.Part)
						For	Each itm2 In colItem
							With itm2
								If .Part = itm.Part Then
									empty_slot = empty_slot - .Size
								End If
							End With
						Next itm2
						'����Ȃ��X���b�g�����A�A�C�e�����O��
						If empty_slot < itm.Size Then
							For	Each itm2 In colItem
								With itm2
									If .Part = itm.Part Then
										If Party0 <> "����" Then
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
				'�����łȂ��ꍇ
				If i > 0 Then
					For	Each itm2 In colItem
						With itm2
							If itm.Part = .Part Then
								If Party0 <> "����" Then
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
		
		'�������ꂽ�A�C�e���͏�ɑ��݂���Ƃ݂Ȃ�
		If Status_Renamed <> "�j��" Then
			itm.Exist = True
		End If
		
		colItem.Add(itm, itm.ID)
		itm.Unit = Me
		
		'�A�C�e���𑕔��������Ƃɂ��X�e�[�^�X�̕ω�
		Update(without_refresh)
	End Sub
	
	Public Sub AddItem0(ByRef itm As Item)
		colItem.Add(itm, itm.ID)
		itm.Unit = Me
	End Sub
	
	'�A�C�e�����͂���
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
		
		'���݂��Ȃ��A�C�e���H
		If itm Is Nothing Then
			Exit Sub
		End If
		
		'�폜����A�C�e���̕���E�A�r���e�B�̎c�e���������p�����̂�h�����߁A
		'�폜����A�C�e���ɂ���ĕt�����ꂽ����E�A�r���e�B�̃f�[�^���폜����B
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
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
				num = num + AdditionalSupport.Data.CountWeapon
				num2 = num2 + AdditionalSupport.Data.CountAbility
			End If
		End If
		For	Each itm2 In colItem
			With itm2
				If itm Is itm2 Then
					For i = num + 1 To num + .CountWeapon
						If i <= CountWeapon Then
							'UPGRADE_NOTE: �I�u�W�F�N�g WData() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							WData(i) = Nothing
						End If
					Next 
					For i = num2 + 1 To num2 + .CountAbility
						If i <= CountAbility Then
							'UPGRADE_NOTE: �I�u�W�F�N�g adata() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
					'UPGRADE_NOTE: �I�u�W�F�N�g itm.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					.Unit = Nothing
				End If
				'�ǉ��p�C���b�g�����A�C�e�����폜����ꍇ
				If itm.IsFeatureAvailable("�ǉ��p�C���b�g") Then
					If PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
						With PList.Item(.FeatureData("�ǉ��p�C���b�g"))
							.Alive = False
							'UPGRADE_NOTE: �I�u�W�F�N�g PList.Item().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							.Unit_Renamed = Nothing
						End With
					End If
				End If
			End If
		End With
		
		'�n�[�h�|�C���g�����A�C�e�����͂������ꍇ�͑��̃A�C�e����A�����Ă͂����K�v������
		Do 
			is_changed = False
			
			'���݂̃A�C�e�������\�񐔂��L�^
			prev_max_item_num = MaxItemNum
			ReDim prev_hard_point(0)
			ReDim prev_hard_point_size(0)
			For i = 1 To CountFeature
				If Feature(i) = "�n�[�h�|�C���g" Then
					If FeatureData(i) <> "�����p�[�c" And FeatureData(i) <> "�A�C�e��" Then
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
			is_ambidextrous = IsFeatureAvailable("���藘��")
			
			'�A�C�e�����O�������Ƃɂ��X�e�[�^�X�̕ω�
			Update(without_refresh)
			
			'�A�C�e�������\���������H
			If prev_max_item_num > MaxItemNum Then
				is_changed = True
				num = MaxItemNum
				
				i = 0
				For	Each itm In colItem
					With itm
						If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
							i = i + 1
							'�n�[�h�|�C���g�������Ȃ��A�C�e������I��ō폜
							If i > num And Not .IsFeatureAvailable("�n�[�h�|�C���g") Then
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: �I�u�W�F�N�g itm.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
										.Unit = Nothing
									End If
								End If
								'�ǉ��p�C���b�g�����A�C�e�����폜����ꍇ
								If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
									If PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
										With PList.Item(.FeatureData("�ǉ��p�C���b�g"))
											.Alive = False
											'UPGRADE_NOTE: �I�u�W�F�N�g PList.Item().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
						If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
							i = i + 1
							If i > num Then
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: �I�u�W�F�N�g itm.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
										.Unit = Nothing
									End If
								End If
								'�ǉ��p�C���b�g�����A�C�e�����폜����ꍇ
								If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
									If PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
										With PList.Item(.FeatureData("�ǉ��p�C���b�g"))
											.Alive = False
											'UPGRADE_NOTE: �I�u�W�F�N�g PList.Item().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
											.Unit_Renamed = Nothing
										End With
									End If
								End If
							End If
						End If
					End With
				Next itm
			End If
			
			'���݂̃A�C�e�������\�񐔂��L�^
			ReDim cur_hard_point(0)
			ReDim cur_hard_point_size(0)
			For i = 1 To CountFeature
				If Feature(i) = "�n�[�h�|�C���g" Then
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
			
			'�n�[�h�|�C���g�������H
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
											'UPGRADE_NOTE: �I�u�W�F�N�g itm.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
											.Unit = Nothing
										End If
									End If
									'�ǉ��p�C���b�g�����A�C�e�����폜����ꍇ
									If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
										If PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
											With PList.Item(.FeatureData("�ǉ��p�C���b�g"))
												.Alive = False
												'UPGRADE_NOTE: �I�u�W�F�N�g PList.Item().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
			
			'���藘���Ŗ����Ȃ��Ă��܂����ꍇ�͓�ڂ̕Ў�A�C�e�����O��
			If is_ambidextrous And Not IsFeatureAvailable("���藘��") Then
				num = 0
				For	Each itm In colItem
					With itm
						If .Part = "�Ў�" Then
							num = num + 1
							If num > 1 Then
								is_changed = True
								colItem.Remove(.ID)
								If Not .Unit Is Nothing Then
									If .Unit.ID = ID Then
										'UPGRADE_NOTE: �I�u�W�F�N�g itm.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
										.Unit = Nothing
									End If
								End If
								'�ǉ��p�C���b�g�����A�C�e�����폜����ꍇ
								If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
									If PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
										With PList.Item(.FeatureData("�ǉ��p�C���b�g"))
											.Alive = False
											'UPGRADE_NOTE: �I�u�W�F�N�g PList.Item().Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
	
	'�������� ipart �̃A�C�e���̑����\��
	Public Function ItemSlotSize(ByRef ipart As String) As Short
		Dim i As Short
		
		Select Case ipart
			Case "�����p�[�c", "�A�C�e��"
				ItemSlotSize = Data.ItemNum
				If Not IsFeatureAvailable("�n�[�h�|�C���g") Then
					Exit Function
				End If
				For i = 1 To CountFeature
					If Feature(i) = "�n�[�h�|�C���g" Then
						Select Case FeatureData(i)
							Case "�����p�[�c", "�A�C�e��"
								ItemSlotSize = ItemSlotSize + FeatureLevel(i)
						End Select
					End If
				Next 
			Case Else
				If Not IsFeatureAvailable("�n�[�h�|�C���g") Then
					ItemSlotSize = 1
					Exit Function
				End If
				For i = 1 To CountFeature
					If Feature(i) = "�n�[�h�|�C���g" Then
						If FeatureData(i) = ipart Then
							ItemSlotSize = ItemSlotSize + FeatureLevel(i)
						End If
					End If
				Next 
		End Select
	End Function
	
	'�A�C�e�� iname �𑕔����Ă��邩�H
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
	
	'�����\�ȕ���N���X
	Public Function WeaponProficiency() As String
		Dim fd As FeatureData
		
		For	Each fd In colFeature
			With fd
				If .Name = "����N���X" Then
					WeaponProficiency = WeaponProficiency & " " & .StrData
				End If
			End With
		Next fd
	End Function
	
	'�����\�Ȗh��N���X
	Public Function ArmorProficiency() As String
		Dim fd As FeatureData
		
		For	Each fd In colFeature
			With fd
				If .Name = "�h��N���X" Then
					ArmorProficiency = ArmorProficiency & " " & .StrData
				End If
			End With
		Next fd
	End Function
	
	'�A�C�e��it�𑕔��ł��邩�ǂ����𔻒�
	Public Function IsAbleToEquip(ByRef it As Item) As Boolean
		Dim iclass As String
		Dim eclass0, uclass, eclass As String
		Dim i, j As Short
		
		With it
			'���ɑ����ς݂̃A�C�e���͑����ł��Ȃ�
			If Not .Unit Is Nothing Then
				If .Unit.ID = ID Then
					IsAbleToEquip = False
					Exit Function
				End If
			End If
			
			'Fix�R�}���h�ŌŒ肳�ꂽ�A�C�e���͑����s�\
			If IsGlobalVariableDefined("Fix(" & .Name & ")") Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'�K�v�Z�\�͖������Ă��邩�H
			If Not .IsAvailable(Me) Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'�A�C�e���̃N���X���L�^
			iclass = .Class_Renamed()
			
			'�ėp�Ȃ�΃��j�b�g�̎�ނɊւ�炸�����\
			If iclass = "�ėp" Then
				'�����������p�[�c�̃`�F�b�N�͕K�v
				If .Part = "�����p�[�c" And IsHero Then
					IsAbleToEquip = False
					Exit Function
				End If
				IsAbleToEquip = True
				Exit Function
			End If
			
			'�Œ�A�C�e���͑����s�\�Ƃ݂Ȃ�
			If iclass = "�Œ�" Then
				IsAbleToEquip = False
				Exit Function
			End If
			
			'���j�b�g�N���X����]���Ȏw�����菜��
			uclass = Class0
			
			'������
			Select Case .Part
				Case "����", "�Ў�", "����"
					eclass = WeaponProficiency()
					For i = 1 To LLength(eclass)
						eclass0 = LIndex(eclass, i)
						If iclass = eclass0 Then
							IsAbleToEquip = True
							Exit Function
						ElseIf InStr(iclass, "��p)") > 0 Then 
							'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
							If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & Name & "��p)") > 0 Or InStr(iclass, "(" & Nickname & "��p)") > 0) Then
								IsAbleToEquip = True
								Exit Function
							End If
							
							'���ʂɂ���p�w��H
							If CountPilot > 0 Then
								If iclass = eclass0 & "(" & MainPilot.Sex & "��p)" Then
									IsAbleToEquip = True
									Exit Function
								End If
							End If
						End If
					Next 
					
					'�ꕔ�̌`�Ԃł̂ݗ��p�\�ȕ���̔���
					For i = 1 To CountOtherForm
						With OtherForm(i)
							uclass = .Class0
							eclass = .WeaponProficiency()
							For j = 1 To LLength(eclass)
								eclass0 = LIndex(eclass, j)
								If iclass = eclass0 Then
									IsAbleToEquip = True
									Exit Function
								ElseIf InStr(iclass, "��p)") > 0 Then 
									'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
									If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & .Name & "��p)") > 0 Or InStr(iclass, "(" & .Nickname & "��p)") > 0) Then
										IsAbleToEquip = True
										Exit Function
									End If
									
									'���ʂɂ���p�w��H
									If CountPilot > 0 Then
										If iclass = eclass0 & "(" & MainPilot.Sex & "��p)" Then
											IsAbleToEquip = True
											Exit Function
										End If
									End If
								End If
							Next 
						End With
					Next 
					
				Case "��", "��", "��"
					eclass = ArmorProficiency()
					For i = 1 To LLength(eclass)
						eclass0 = LIndex(eclass, i)
						If iclass = eclass0 Then
							IsAbleToEquip = True
							Exit Function
						ElseIf InStr(iclass, "��p)") > 0 Then 
							'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
							If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & Name & "��p)") > 0 Or InStr(iclass, "(" & Nickname & "��p)") > 0) Then
								IsAbleToEquip = True
								Exit Function
							End If
							
							'���ʂɂ���p�w��H
							If CountPilot > 0 Then
								If iclass = eclass0 & "(" & MainPilot.Sex & "��p)" Then
									IsAbleToEquip = True
									Exit Function
								End If
							End If
						End If
					Next 
					
					'�ꕔ�̌`�Ԃł̂ݗ��p�\�Ȗh��̔���
					For i = 1 To CountOtherForm
						With OtherForm(i)
							uclass = .Class0
							eclass = .ArmorProficiency()
							For j = 1 To LLength(eclass)
								eclass0 = LIndex(eclass, j)
								If iclass = eclass0 Then
									IsAbleToEquip = True
									Exit Function
								ElseIf InStr(iclass, "��p)") > 0 Then 
									'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
									If InStr(iclass, eclass0 & "(") = 1 And (InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & .Name & "��p)") > 0 Or InStr(iclass, "(" & .Nickname & "��p)") > 0) Then
										IsAbleToEquip = True
										Exit Function
									End If
									
									'���ʂɂ���p�w��H
									If CountPilot > 0 Then
										If iclass = eclass0 & "(" & MainPilot.Sex & "��p)" Then
											IsAbleToEquip = True
											Exit Function
										End If
									End If
								End If
							Next 
						End With
					Next 
					
				Case "�A�C�e��", "�����p�[�c"
					'�����p�[�c�͐l�ԃ��j�b�g�ɂ͑����ł��Ȃ�
					If InStr(.Part, "�����p�[�c") = 1 Then
						If IsHero Then
							IsAbleToEquip = False
							Exit Function
						End If
					End If
					
					'�����̃A�C�e���͐�p�A�C�e���łȂ�����K�������\
					If InStr(iclass, "��p)") = 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
					If InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & Name & "��p)") > 0 Or InStr(iclass, "(" & Nickname & "��p)") > 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'���ʂɂ���p�w��H
					If CountPilot > 0 Then
						If InStr(iclass, "(" & MainPilot.Sex & "��p)") > 0 Then
							IsAbleToEquip = True
							Exit Function
						End If
					End If
					
					'���̌`�Ԃ̖��O�Ő�p�w�肳��Ă���H
					For i = 1 To CountOtherForm
						With OtherForm(i)
							If InStr(iclass, "(" & .Class0 & "��p)") > 0 Or InStr(iclass, "(" & .Name & "��p)") > 0 Or InStr(iclass, "(" & .Nickname & "��p)") > 0 Then
								IsAbleToEquip = True
								Exit Function
							End If
						End With
					Next 
					
				Case Else
					'�n�삳�ꂽ�������̃A�C�e���͐�p�A�C�e���łȂ�����K�������\
					If InStr(iclass, "��p)") = 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'���j�b�g�N���X�A���j�b�g���ɂ���p�w��H
					If InStr(iclass, "(" & uclass & "��p)") > 0 Or InStr(iclass, "(" & Name & "��p)") > 0 Or InStr(iclass, "(" & Nickname & "��p)") > 0 Then
						IsAbleToEquip = True
						Exit Function
					End If
					
					'���ʂɂ���p�w��H
					If CountPilot > 0 Then
						If InStr(iclass, "(" & MainPilot.Sex & "��p)") > 0 Then
							IsAbleToEquip = True
							Exit Function
						End If
					End If
					
					'���̌`�Ԃ̖��O�Ő�p�w�肳��Ă���H
					For i = 1 To CountOtherForm
						With OtherForm(i)
							If InStr(iclass, "(" & .Class0 & "��p)") > 0 Or InStr(iclass, "(" & .Name & "��p)") > 0 Or InStr(iclass, "(" & .Nickname & "��p)") > 0 Then
								IsAbleToEquip = True
								Exit Function
							End If
						End With
					Next 
			End Select
		End With
		
		IsAbleToEquip = False
	End Function
	
	
	' === ���b�Z�[�W�֘A���� ===
	
	'�� Situation �ɉ������p�C���b�g���b�Z�[�W��\��
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
		
		'WAVE�����t�������`�F�b�N���邽�߁A���炩���߃N���A
		IsWavePlayed = False
		
		'�Ή����b�Z�[�W����`����Ă��Ȃ������ꍇ�Ɏg�p����V�`���G�[�V������ݒ�
		ReDim situations(1)
		situations(1) = Situation
		Select Case Situation
			Case "���g", "�؂蕥��", "�}��", "����", "���Đg�Z", "�j�~", "�_�~�[", "�ً}�e���|�[�g"
				ReDim Preserve situations(2)
				situations(2) = "���"
			Case "�r�[��������", "�U��������", "�V�[���h�h��"
				ReDim Preserve situations(2)
				situations(2) = "�_���[�W��"
			Case "���", "�j��", "�_���[�W��", "�_���[�W��", "�_���[�W��", "������"
			Case Else
				If msg_mode = "�U��" Or msg_mode = "�J�E���^�[" Then
					'�U�����b�Z�[�W
					wname = situations(1)
					
					'����ԍ�������
					For w = 1 To CountWeapon
						With Weapon(w)
							If Situation = .Name Or Situation = .Nickname Then
								Exit For
							End If
						End With
					Next 
					
					If Not IsDefense() Then
						ReDim Preserve situations(2)
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(2) = "�i��"
						Else
							situations(2) = "�ˌ�"
						End If
					ElseIf msg_mode = "�J�E���^�[" Then 
						ReDim Preserve situations(3)
						situations(1) = Situation & "(����)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(3) = "�i��"
						Else
							situations(3) = "�ˌ�"
						End If
					Else
						ReDim Preserve situations(4)
						situations(1) = Situation & "(����)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(3) = "�i��(����)"
							situations(4) = "�i��"
						Else
							situations(3) = "�ˌ�(����)"
							situations(4) = "�ˌ�"
						End If
					End If
				ElseIf msg_mode = "�A�r���e�B" Then 
					ReDim Preserve situations(2)
					situations(2) = "�A�r���e�B"
				ElseIf InStr(Situation, "(����)") > 0 Or InStr(Situation, "(���)") > 0 Or InStr(Situation, "(�Ƃǂ�)") > 0 Or InStr(Situation, "(�N���e�B�J��)") > 0 Then 
					'�T�u�V�`���G�[�V�����t���̍U�����b�Z�[�W
					
					'����ԍ�������
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
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(2) = "�i��" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(2) = "�ˌ�" & Mid(Situation, InStr2(Situation, "("))
						End If
					ElseIf msg_mode = "�J�E���^�[" Then 
						ReDim Preserve situations(3)
						situations(1) = Situation & "(����)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(3) = "�i��" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(3) = "�ˌ�" & Mid(Situation, InStr2(Situation, "("))
						End If
					Else
						ReDim Preserve situations(4)
						situations(1) = Situation & "(����)"
						situations(2) = Situation
						If IsWeaponClassifiedAs(w, "�i���n") Then
							situations(3) = "�i��" & Mid(Situation, InStr2(Situation, "(")) & "(����)"
							situations(4) = "�i��" & Mid(Situation, InStr2(Situation, "("))
						Else
							situations(3) = "�ˌ�" & Mid(Situation, InStr2(Situation, "(")) & "(����)"
							situations(4) = "�ˌ�" & Mid(Situation, InStr2(Situation, "("))
						End If
					End If
				Else
					'�U�����b�Z�[�W�łȂ��Ă��ꉞ�U�����햼��ݒ�
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
		
		'SelectMessage�R�}���h
		For i = 1 To UBound(situations)
			buf = "Message(" & MainPilot.ID & "," & situations(i) & ")"
			If IsLocalVariableDefined(buf) Then
				'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				selected_msg = LocalVariableList.Item(buf).StringValue
				selected_situation = situations(i)
				UndefineVariable(buf)
				Exit For
			End If
			
			If situations(i) = "�i��" Or situations(i) = "�ˌ�" Then
				buf = "Message(" & MainPilot.ID & ",�U��)"
				If IsLocalVariableDefined(buf) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					selected_msg = LocalVariableList.Item(buf).StringValue
					selected_situation = "�U��"
					UndefineVariable(buf)
					Exit For
				End If
			End If
			
			If situations(i) = "�i��(����)" Or situations(i) = "�ˌ�(����)" Then
				buf = "Message(" & MainPilot.ID & ",�U��(����))"
				If IsLocalVariableDefined(buf) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					selected_msg = LocalVariableList.Item(buf).StringValue
					selected_situation = "�U��(����)"
					UndefineVariable(buf)
					Exit For
				End If
			End If
		Next 
		If InStr(selected_msg, "::") > 0 Then
			selected_pilot = Left(selected_msg, InStr(selected_msg, "::") - 1)
			selected_msg = Mid(selected_msg, InStr(selected_msg, "::") + 2)
		End If
		
		'�������͂R���̂Q�̊m���ŕ\��
		If selected_msg = "" Then
			If InStr(Situation, "������") = 1 Then
				If Dice(3) = 1 Then
					Exit Sub
				End If
			End If
		End If
		
		'����ׂ�Ȃ��ꍇ
		'������SetMessage�R�}���h�Ń��b�Z�[�W���ݒ肳��Ă���ꍇ��
		'��������g�p�B
		If selected_msg = "" Then
			If IsConditionSatisfied("�Ή�") Or IsConditionSatisfied("����") Or IsConditionSatisfied("���") Then
				'�ӎ��s��
				Exit Sub
			End If
			If IsConditionSatisfied("����") Or IsConditionSatisfied("�߈�") Then
				'����
				If InStr(Situation, "(") = 0 Then
					Select Case Situation
						Case "�_���[�W��", "�_���[�W��", "�j��"
							If NPDList.IsDefined(MainPilot.Name & "(�_���[�W)") Then
								DisplayBattleMessage(MainPilot.Name & "(�_���[�W)", "�c�c�c�c�I")
								Exit Sub
							End If
						Case "������"
							Exit Sub
					End Select
					If wname <> "" Then
						If NPDList.IsDefined(MainPilot.Name & "(�U��)") Then
							DisplayBattleMessage(MainPilot.Name & "(�U��)", "�c�c�c�c�I")
							Exit Sub
						End If
					End If
					DisplayBattleMessage(MainPilot.ID, "�c�c�c�c")
				End If
				Exit Sub
			End If
			If IsConditionSatisfied("����") Then
				'�Q��
				If InStr(Situation, "(") = 0 Then
					DisplayBattleMessage(MainPilot.ID, "�y�y�y�c�c")
				End If
				Exit Sub
			End If
			If IsConditionSatisfied("���|") Then
				If IsMessageDefined("���|") Then
					'���|��ԗp���b�Z�[�W����`����Ă���΂�������g��
					ReDim situations(1)
					situations(1) = "���|"
				Else
					'�p�j�b�N���̃��b�Z�[�W���쐬���ĕ\��
					If InStr(Situation, "(") = 0 Then
						msg = ""
						Select Case MainPilot.Sex
							Case "�j��"
								Select Case Dice(4)
									Case 1
										msg = "���A���킠���������I"
									Case 2
										msg = "���킠�������I"
									Case 3
										msg = "�ЁA�Ђ��������I"
									Case 4
										msg = "�Ђ��������I"
								End Select
							Case "����"
								Select Case Dice(4)
									Case 1
										msg = "���Ⴀ�����������I"
									Case 2
										msg = "���Ⴀ�����I"
									Case 3
										msg = "���킠�������I"
									Case 4
										msg = "���A�����Ă������I"
								End Select
						End Select
						
						If msg <> "" Then
							If NPDList.IsDefined(MainPilot.Name & "(����)") Then
								DisplayBattleMessage(MainPilot.Name & "(����)", msg)
							ElseIf NPDList.IsDefined(MainPilot.Name & "(�_���[�W)") Then 
								DisplayBattleMessage(MainPilot.Name & "(�_���[�W)", msg)
							Else
								DisplayBattleMessage(MainPilot.ID, msg)
							End If
						End If
					End If
					Exit Sub
				End If
			End If
			If IsConditionSatisfied("����") Then
				If IsMessageDefined("����") Then
					'������ԗp���b�Z�[�W����`����Ă���΂�������g��
					ReDim situations(1)
					situations(1) = "����"
				End If
			End If
		End If
		
		'�_�C�A���O�f�[�^���g���Ĕ���
		ReDim pnames(3)
		pnames(1) = MainPilot.MessageType
		pnames(2) = MainPilot.MessageType
		pnames(3) = MainPilot.MessageType
		If IsFeatureAvailable("�ǉ��p�C���b�g") Then
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
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			pnames(1) = pnames(1) & " " & AdditionalSupport.MessageType
		End If
		If Situation = SelectedSpecialPower Then
			pnames(3) = SelectedPilot.MessageType
		End If
		For i = 1 To UBound(pnames)
			'�ǉ��p�C���b�g�Ƀ��b�Z�[�W�f�[�^������΂������D��
			If i = 4 Then
				If MDList.IsDefined(MainPilot.MessageType) Then
					Exit For
				End If
			End If
			
			If DDList.IsDefined(pnames(i)) Then
				With DDList.Item(pnames(i))
					If selected_msg <> "" Then
						'SelectMessage�őI�����ꂽ���b�Z�[�W������
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
		
		'�Q�b�^�[�̂悤�ȃ��j�b�g�͕K�����C���p�C���b�g���g���ă��b�Z�[�W��\��
		If Data.PilotNum > 0 And MainPilot Is Pilot(1) And Situation <> SelectedSpecialPower Then
			i = Dice(CountPilot + CountSupport)
		Else
			i = 1
		End If
		
TryAgain: 
		'�I�񂾃p�C���b�g�ɂ�郁�b�Z�[�W�f�[�^�Ŕ���
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
		
		'���b�Z�[�W��\��
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
		
		'���b�Z�[�W�f�[�^��������Ȃ��ꍇ�͑��̃p�C���b�g�ŒT���Ȃ���
		If md Is Nothing Then
			If i <> 1 Then
				i = 1
				GoTo TryAgain
			Else
				GoTo TrySelectedMessage
			End If
		End If
		
		If selected_msg <> "" Then
			'SelectMessage�őI�����ꂽ���b�Z�[�W������
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
			'���b�Z�[�W�f�[�^���烁�b�Z�[�W������
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
		'���b�Z�[�W��\��
		If selected_msg <> "" And selected_msg <> "-" Then
			If selected_pilot <> "" Then
				DisplayBattleMessage(selected_pilot, selected_msg)
			Else
				DisplayBattleMessage(MainPilot.ID, selected_msg)
			End If
		End If
	End Sub
	
	'�_�C�A���O�̍Đ�
	Public Sub PlayDialog(ByRef dd As Dialog, ByRef wname As String)
		Dim msg, buf As String
		Dim i, idx As Short
		Dim t As Unit
		Dim tw As Short
		
		'�摜�`�悪�s��ꂽ���ǂ����̔���̂��߂Ƀt���O��������
		IsPictureDrawn = False
		
		With dd
			'�_�C�A���O�̌X�̃��b�Z�[�W��\��
			For i = 1 To .Count
				msg = .Message(i)
				
				'���b�Z�[�W�\���̃L�����Z��
				If msg = "-" Then
					Exit Sub
				End If
				
				'���j�b�g��
				Do While InStr(msg, "$(���j�b�g)") > 0
					idx = InStr(msg, "$(���j�b�g)")
					buf = Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "��p") > 0 Then
						buf = Mid(buf, InStr(buf, "��p") + 2)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
				Loop 
				Do While InStr(msg, "$(�@��)") > 0
					idx = InStr(msg, "$(�@��)")
					buf = Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "��p") > 0 Then
						buf = Mid(buf, InStr(buf, "��p") + 2)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
				Loop 
				
				'�p�C���b�g��
				Do While InStr(msg, "$(�p�C���b�g)") > 0
					buf = MainPilot.Nickname
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					idx = InStr(msg, "$(�p�C���b�g)")
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
				Loop 
				
				'���햼
				Do While InStr(msg, "$(����)") > 0
					idx = InStr(msg, "$(����)")
					buf = wname
					ReplaceSubExpression(buf)
					If InStr(buf, "(") > 0 Then
						buf = Left(buf, InStr(buf, "(") - 1)
					End If
					If InStr(buf, "<") > 0 Then
						buf = Left(buf, InStr(buf, "<") - 1)
					End If
					If InStr(buf, "��") > 0 Then
						buf = Left(buf, InStr(buf, "��") - 1)
					End If
					msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
				Loop 
				
				'������
				Do While InStr(msg, "$(������)") > 0
					idx = InStr(msg, "$(������)")
					msg = Left(msg, idx - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, idx + 6)
				Loop 
				
				'���胆�j�b�g��ݒ�
				If SelectedUnit Is Me Then
					t = SelectedTarget
					tw = SelectedTWeapon
				Else
					t = SelectedUnit
					tw = SelectedWeapon
				End If
				
				If Not t Is Nothing Then
					'���胆�j�b�g��
					Do While InStr(msg, "$(���胆�j�b�g)") > 0
						buf = t.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						If InStr(buf, "��p") > 0 Then
							buf = Mid(buf, InStr(buf, "��p") + 2)
						End If
						idx = InStr(msg, "$(���胆�j�b�g)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 9)
					Loop 
					Do While InStr(msg, "$(����@��)") > 0
						buf = t.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						If InStr(buf, "��p") > 0 Then
							buf = Mid(buf, InStr(buf, "��p") + 2)
						End If
						idx = InStr(msg, "$(����@��)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
					Loop 
					
					'����p�C���b�g��
					Do While InStr(msg, "$(����p�C���b�g)") > 0
						buf = t.MainPilot.Nickname
						If InStr(buf, "(") > 0 Then
							buf = Left(buf, InStr(buf, "(") - 1)
						End If
						idx = InStr(msg, "$(����p�C���b�g)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 10)
					Loop 
					
					'���蕐�햼
					Do While InStr(msg, "$(���蕐��)") > 0
						If 1 <= tw And tw <= t.CountWeapon Then
							buf = t.WeaponNickname(tw)
						Else
							buf = ""
						End If
						If InStr(buf, "<") > 0 Then
							buf = Left(buf, InStr(buf, "<") - 1)
						End If
						If InStr(buf, "��") > 0 Then
							buf = Left(buf, InStr(buf, "��") - 1)
						End If
						idx = InStr(msg, "$(���蕐��)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
					Loop 
					
					'���葹����
					Do While InStr(msg, "$(���葹����)") > 0
						With t
							buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
						End With
						idx = InStr(msg, "$(���葹����)")
						msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
					Loop 
				End If
				
				'���b�Z�[�W��\��
				If .Name(i) = MainPilot.Name Then
					DisplayBattleMessage(MainPilot.ID, msg)
				ElseIf Left(.Name(i), 1) = "@" Then 
					DisplayBattleMessage(Mid(.Name(i), 2), msg)
				Else
					DisplayBattleMessage(.Name(i), msg)
				End If
			Next 
		End With
		
		'�J�b�g�C���͏������Ă���
		If Not IsOptionDefined("�퓬����ʏ���������") Then
			If IsPictureDrawn Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
		End If
	End Sub
	
	'���b�Z�[�W���Đ�
	Public Sub PlayMessage(ByRef p As Pilot, ByRef msg As String, ByRef wname As String)
		Dim buf As String
		Dim idx As Short
		Dim t As Unit
		Dim tw As Short
		
		'���b�Z�[�W�\�����L�����Z��
		If msg = "-" Then
			Exit Sub
		End If
		
		'�摜�`�悪�s��ꂽ���ǂ����̔���̂��߂Ƀt���O��������
		IsPictureDrawn = False
		
		'���j�b�g��
		Do While InStr(msg, "$(���j�b�g)") > 0
			idx = InStr(msg, "$(���j�b�g)")
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "��p") > 0 Then
				buf = Mid(buf, InStr(buf, "��p") + 2)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
		Loop 
		Do While InStr(msg, "$(�@��)") > 0
			idx = InStr(msg, "$(�@��)")
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "��p") > 0 Then
				buf = Mid(buf, InStr(buf, "��p") + 2)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
		Loop 
		
		'�p�C���b�g��
		Do While InStr(msg, "$(�p�C���b�g)") > 0
			buf = MainPilot.Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			idx = InStr(msg, "$(�p�C���b�g)")
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
		Loop 
		
		'���햼
		Do While InStr(msg, "$(����)") > 0
			idx = InStr(msg, "$(����)")
			buf = wname
			ReplaceSubExpression(buf)
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "<") > 0 Then
				buf = Left(buf, InStr(buf, "<") - 1)
			End If
			If InStr(buf, "��") > 0 Then
				buf = Left(buf, InStr(buf, "��") - 1)
			End If
			msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 5)
		Loop 
		
		'������
		Do While InStr(msg, "$(������)") > 0
			idx = InStr(msg, "$(������)")
			msg = Left(msg, idx - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, idx + 6)
		Loop 
		
		'���胆�j�b�g��ݒ�
		If SelectedUnit Is Me Then
			t = SelectedTarget
			tw = SelectedTWeapon
		Else
			t = SelectedUnit
			tw = SelectedWeapon
		End If
		
		If Not t Is Nothing Then
			'���胆�j�b�g��
			Do While InStr(msg, "$(���胆�j�b�g)") > 0
				buf = t.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				If InStr(buf, "��p") > 0 Then
					buf = Mid(buf, InStr(buf, "��p") + 2)
				End If
				idx = InStr(msg, "$(���胆�j�b�g)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 9)
			Loop 
			Do While InStr(msg, "$(����@��)") > 0
				buf = t.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				If InStr(buf, "��p") > 0 Then
					buf = Mid(buf, InStr(buf, "��p") + 2)
				End If
				idx = InStr(msg, "$(����@��)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
			Loop 
			
			'����p�C���b�g��
			Do While InStr(msg, "$(����p�C���b�g)") > 0
				buf = t.MainPilot.Nickname
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				idx = InStr(msg, "$(����p�C���b�g)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 10)
			Loop 
			
			'���蕐�햼
			Do While InStr(msg, "$(���蕐��)") > 0
				If 1 <= tw And tw <= t.CountWeapon Then
					buf = t.WeaponNickname(tw)
				Else
					buf = ""
				End If
				If InStr(buf, "<") > 0 Then
					buf = Left(buf, InStr(buf, "<") - 1)
				End If
				If InStr(buf, "��") > 0 Then
					buf = Left(buf, InStr(buf, "��") - 1)
				End If
				idx = InStr(msg, "$(���蕐��)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 7)
			Loop 
			
			'���葹����
			Do While InStr(msg, "$(���葹����)") > 0
				With t
					buf = VB6.Format(100 * (.MaxHP - .HP) \ .MaxHP)
				End With
				idx = InStr(msg, "$(���葹����)")
				msg = Left(msg, idx - 1) & buf & Mid(msg, idx + 8)
			Loop 
		End If
		
		'���b�Z�[�W��\��
		DisplayBattleMessage((p.ID), msg)
		
		'�J�b�g�C���͏������Ă���
		If Not IsOptionDefined("�퓬����ʏ���������") Then
			If IsPictureDrawn Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
		End If
	End Sub
	
	'�V�`���G�[�V���� main_situation �ɑΉ����郁�b�Z�[�W����`����Ă��邩
	Public Function IsMessageDefined(ByRef main_situation As String, Optional ByVal ignore_condition As Boolean = False) As Boolean
		Dim pnames(4) As String
		Dim msg As String
		Dim i As Short
		
		'����ׂ�Ȃ��ꍇ
		If Not ignore_condition Then
			If IsConditionSatisfied("����") Or IsConditionSatisfied("�߈�") Or IsConditionSatisfied("�Ή�") Or IsConditionSatisfied("����") Or IsConditionSatisfied("���") Or IsConditionSatisfied("����") Then
				IsMessageDefined = False
				Exit Function
			End If
			
			'�����ԗp���b�Z�[�W����`����Ă��邩�m�F����ꍇ���l��
			If IsConditionSatisfied("���|") And main_situation <> "���|" Then
				IsMessageDefined = False
				Exit Function
			End If
			If IsConditionSatisfied("����") And main_situation <> "����" Then
				IsMessageDefined = False
				Exit Function
			End If
		End If
		
		'SetMessage�R�}���h�Ń��b�Z�[�W���ݒ肳��Ă��邩����
		If IsLocalVariableDefined("Message(" & MainPilot.ID & "," & main_situation & ")") Then
			IsMessageDefined = True
			Exit Function
		End If
		
		'�_�C�A���O�f�[�^���g���Ĕ���
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
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
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
		
		'���b�Z�[�W�f�[�^���g���Ĕ���
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
	
	'������b�Z�[�W��\��
	Public Sub SysMessage(ByRef main_situation As String, Optional ByRef sub_situation As String = "", Optional ByRef add_msg As String = "")
		Dim uname, msg, uclass As String
		Dim situations() As String
		Dim idx, buf As String
		Dim i, ret As Short
		Dim wname As String
		
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation & "(���)"
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")(���)"
			situations(2) = main_situation & "(���)"
		End If
		
		' ADD START MARGE
		'�g���퓬�A�j���f�[�^�Ō���
		If ExtendedAnimation Then
			With EADList
				For i = 1 To UBound(situations)
					'�퓬�A�j���\�͂Ŏw�肳�ꂽ���̂Ō���
					If IsFeatureAvailable("�퓬�A�j��") Then
						uname = FeatureData("�퓬�A�j��")
						If .IsDefined(uname) Then
							msg = .Item(uname).SelectMessage(situations(i), Me)
							If Len(msg) > 0 Then
								GoTo FoundMessage
							End If
						End If
					End If
					
					'���j�b�g���̂Ō���
					If .IsDefined(Name) Then
						msg = .Item(Name).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'���j�b�g���̂��C���������̂Ō���
					uname = Nickname0
					ret = InStr(uname, "(")
					If ret > 1 Then
						uname = Left(uname, ret - 1)
					End If
					ret = InStr(uname, "�p")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					ret = InStr(uname, "�^")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					If Right(uname, 4) = "�J�X�^��" Then
						uname = Left(uname, Len(uname) - 4)
					End If
					If Right(uname, 1) = "��" Then
						uname = Left(uname, Len(uname) - 1)
					End If
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'���j�b�g�N���X�Ō���
					uclass = Class0
					If .IsDefined(uclass) Then
						msg = .Item(uclass).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
					
					'�ėp
					If .IsDefined("�ėp") Then
						msg = .Item("�ėp").SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				Next 
			End With
		End If
		' ADD END MARGE
		
		'�퓬�A�j���f�[�^�Ō���
		With ADList
			For i = 1 To UBound(situations)
				'�퓬�A�j���\�͂Ŏw�肳�ꂽ���̂Ō���
				If IsFeatureAvailable("�퓬�A�j��") Then
					uname = FeatureData("�퓬�A�j��")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				End If
				
				'���j�b�g���̂Ō���
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'���j�b�g���̂��C���������̂Ō���
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "�p")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "�^")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "�J�X�^��" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "��" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'���j�b�g�N���X�Ō���
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'�ėp
				If .IsDefined("�ėp") Then
					msg = .Item("�ėp").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
			Next 
		End With
		
		'������ʃf�[�^�Ō���
		With EDList
			For i = 1 To UBound(situations)
				'������ʔ\�͂Ŏw�肳�ꂽ���̂Ō���
				If IsFeatureAvailable("�������") Then
					uname = FeatureData("�������")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							GoTo FoundMessage
						End If
					End If
				End If
				
				'���j�b�g���̂Ō���
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'���j�b�g���̂��C���������̂Ō���
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "�p")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "�^")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "�J�X�^��" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "��" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'���j�b�g�N���X�Ō���
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
				
				'�ėp
				If .IsDefined("�ėp") Then
					msg = .Item("�ėp").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						GoTo FoundMessage
					End If
				End If
			Next 
		End With
		
		'�Ή����郁�b�Z�[�W��������Ȃ�����
		Exit Sub
		
FoundMessage: 
		
		'���b�Z�[�W�\���̃L�����Z��
		If msg = "-" Then
			Exit Sub
		End If
		
		'���j�b�g��
		Do While InStr(msg, "$(���j�b�g)") > 0
			idx = CStr(InStr(msg, "$(���j�b�g)"))
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "��p") > 0 Then
				buf = Left(buf, InStr(buf, "��p") + 2)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		Do While InStr(msg, "$(�@��)") > 0
			idx = CStr(InStr(msg, "$(�@��)"))
			buf = Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			If InStr(buf, "��p") > 0 Then
				buf = Left(buf, InStr(buf, "��p") + 2)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 5)
		Loop 
		
		'�p�C���b�g��
		Do While InStr(msg, "$(�p�C���b�g)") > 0
			idx = CStr(InStr(msg, "$(�p�C���b�g)"))
			buf = MainPilot.Nickname
			If InStr(buf, "(") > 0 Then
				buf = Left(buf, InStr(buf, "(") - 1)
			End If
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 8)
		Loop 
		
		'���햼
		If InStr(msg, "$(����)") > 0 Then
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
			Do While InStr(msg, "$(����)") > 0
				idx = CStr(InStr(msg, "$(����)"))
				msg = Left(msg, CDbl(idx) - 1) & wname & Mid(msg, CDbl(idx) + 5)
			Loop 
		End If
		
		'������
		Do While InStr(msg, "$(������)") > 0
			idx = CStr(InStr(msg, "$(������)"))
			msg = Left(msg, CDbl(idx) - 1) & VB6.Format(100 * (MaxHP - HP) \ MaxHP) & Mid(msg, CDbl(idx) + 6)
		Loop 
		
		'���胆�j�b�g��
		Do While InStr(msg, "$(���胆�j�b�g)") > 0
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
			If InStr(buf, "��p") > 0 Then
				buf = Left(buf, InStr(buf, "��p") + 2)
			End If
			idx = CStr(InStr(msg, "$(���胆�j�b�g)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 9)
		Loop 
		Do While InStr(msg, "$(����@��)") > 0
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
			If InStr(buf, "��p") > 0 Then
				buf = Left(buf, InStr(buf, "��p") + 2)
			End If
			idx = CStr(InStr(msg, "$(����@��)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		
		'����p�C���b�g��
		Do While InStr(msg, "$(����p�C���b�g)") > 0
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
			idx = CStr(InStr(msg, "$(����p�C���b�g)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 10)
		Loop 
		
		'���蕐�햼
		Do While InStr(msg, "$(���蕐��)") > 0
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
			If InStr(buf, "��") > 0 Then
				buf = Left(buf, InStr(buf, "��") - 1)
			End If
			idx = CStr(InStr(msg, "$(���蕐��)"))
			msg = Left(msg, CDbl(idx) - 1) & buf & Mid(msg, CDbl(idx) + 7)
		Loop 
		
		'���葹����
		Do While InStr(msg, "$(���葹����)") > 0
			idx = CStr(InStr(msg, "$(���葹����)"))
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
	
	'������b�Z�[�W����`����Ă��邩�H
	Public Function IsSysMessageDefined(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As Boolean
		Dim uclass, uname, msg As String
		Dim situations() As String
		Dim i, ret As Short
		
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation & "(���)"
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")(���)"
			situations(2) = main_situation & "(���)"
		End If
		
		' ADD START MARGE
		'�g���퓬�A�j���f�[�^�Ō���
		If ExtendedAnimation Then
			With EADList
				For i = 1 To UBound(situations)
					'�퓬�A�j���\�͂Ŏw�肳�ꂽ���̂Ō���
					If IsFeatureAvailable("�퓬�A�j��") Then
						uname = FeatureData("�퓬�A�j��")
						If .IsDefined(uname) Then
							msg = .Item(uname).SelectMessage(situations(i), Me)
							If Len(msg) > 0 Then
								IsSysMessageDefined = True
								Exit Function
							End If
						End If
					End If
					
					'���j�b�g���̂Ō���
					If .IsDefined(Name) Then
						msg = .Item(Name).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'���j�b�g���̂��C���������̂Ō���
					uname = Nickname0
					ret = InStr(uname, "(")
					If ret > 1 Then
						uname = Left(uname, ret - 1)
					End If
					ret = InStr(uname, "�p")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					ret = InStr(uname, "�^")
					If ret > 0 Then
						If ret < Len(uname) Then
							uname = Mid(uname, ret + 1)
						End If
					End If
					If Right(uname, 4) = "�J�X�^��" Then
						uname = Left(uname, Len(uname) - 4)
					End If
					If Right(uname, 1) = "��" Then
						uname = Left(uname, Len(uname) - 1)
					End If
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'���j�b�g�N���X�Ō���
					uclass = Class0
					If .IsDefined(uclass) Then
						msg = .Item(uclass).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
					
					'�ėp
					If .IsDefined("�ėp") Then
						msg = .Item("�ėp").SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				Next 
			End With
		End If
		' ADD END MARGE
		
		'�퓬�A�j���f�[�^�Ō���
		With ADList
			For i = 1 To UBound(situations)
				'�퓬�A�j���\�͂Ŏw�肳�ꂽ���̂Ō���
				If IsFeatureAvailable("�퓬�A�j��") Then
					uname = FeatureData("�퓬�A�j��")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				End If
				
				'���j�b�g���̂Ō���
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'���j�b�g���̂��C���������̂Ō���
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "�p")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "�^")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "�J�X�^��" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "��" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'���j�b�g�N���X�Ō���
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'�ėp
				If .IsDefined("�ėp") Then
					msg = .Item("�ėp").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
			Next 
		End With
		
		'������ʃf�[�^�Ō���
		With EDList
			For i = 1 To UBound(situations)
				'������ʔ\�͂Ŏw�肳�ꂽ���̂Ō���
				If IsFeatureAvailable("�������") Then
					uname = FeatureData("�������")
					If .IsDefined(uname) Then
						msg = .Item(uname).SelectMessage(situations(i), Me)
						If Len(msg) > 0 Then
							IsSysMessageDefined = True
							Exit Function
						End If
					End If
				End If
				
				'���j�b�g���̂Ō���
				If .IsDefined(Name) Then
					msg = .Item(Name).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'���j�b�g���̂��C���������̂Ō���
				uname = Nickname0
				ret = InStr(uname, "(")
				If ret > 1 Then
					uname = Left(uname, ret - 1)
				End If
				ret = InStr(uname, "�p")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				ret = InStr(uname, "�^")
				If ret > 0 Then
					If ret < Len(uname) Then
						uname = Mid(uname, ret + 1)
					End If
				End If
				If Right(uname, 4) = "�J�X�^��" Then
					uname = Left(uname, Len(uname) - 4)
				End If
				If Right(uname, 1) = "��" Then
					uname = Left(uname, Len(uname) - 1)
				End If
				If .IsDefined(uname) Then
					msg = .Item(uname).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'���j�b�g�N���X�Ō���
				uclass = Class0
				If .IsDefined(uclass) Then
					msg = .Item(uclass).SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
				
				'�ėp
				If .IsDefined("�ėp") Then
					msg = .Item("�ėp").SelectMessage(situations(i), Me)
					If Len(msg) > 0 Then
						IsSysMessageDefined = True
						Exit Function
					End If
				End If
			Next 
		End With
		
		IsSysMessageDefined = False
	End Function
	
	
	' === ������ʊ֘A���� ===
	
	'������ʃf�[�^������
	Public Function SpecialEffectData(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As String
		Dim uname, uclass As String
		Dim situations() As String
		Dim i, ret As Short
		
		'�V�`���G�[�V�����̃��X�g���\�z
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")"
			situations(2) = main_situation
		End If
		
		For i = 1 To UBound(situations)
			'������ʔ\�͂Ŏw�肳�ꂽ���̂Ō���
			If IsFeatureAvailable("�������") Then
				uname = FeatureData("�������")
				If EDList.IsDefined(uname) Then
					SpecialEffectData = EDList.Item(uname).SelectMessage(situations(i), Me)
					If Len(SpecialEffectData) > 0 Then
						Exit Function
					End If
				End If
			End If
			
			'���j�b�g���̂Ō���
			If EDList.IsDefined(Name) Then
				SpecialEffectData = EDList.Item(Name).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'���j�b�g���̂��C���������̂Ō���
			uname = Nickname
			ret = InStr(uname, "(")
			If ret > 1 Then
				uname = Left(uname, ret - 1)
			End If
			ret = InStr(uname, "�p")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			ret = InStr(uname, "�^")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			If Right(uname, 4) = "�J�X�^��" Then
				uname = Left(uname, Len(uname) - 4)
			End If
			If Right(uname, 1) = "��" Then
				uname = Left(uname, Len(uname) - 1)
			End If
			If EDList.IsDefined(uname) Then
				SpecialEffectData = EDList.Item(uname).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'���j�b�g�N���X�Ō���
			uclass = Class0
			If EDList.IsDefined(uclass) Then
				SpecialEffectData = EDList.Item(uclass).SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
			
			'�ėp
			If EDList.IsDefined("�ėp") Then
				SpecialEffectData = EDList.Item("�ėp").SelectMessage(situations(i), Me)
				If Len(SpecialEffectData) > 0 Then
					Exit Function
				End If
			End If
		Next 
	End Function
	
	'������ʃf�[�^���Đ�
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
		
		'������ʃf�[�^������
		anime = SpecialEffectData(main_situation, sub_situation)
		
		TrimString(anime)
		
		'�\���L�����Z��
		If anime = "" Or anime = "-" Then
			Exit Sub
		End If
		
		'�}�E�X�̉E�{�^���ŃL�����Z��
		If IsRButtonPressed() Then
			'���]���̂ݍs��
			FormatMessage(anime)
			Exit Sub
		End If
		
		If BattleAnimation And Not IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			For i = 1 To CountWeapon
				If Weapon(i).Name = main_situation Then
					w = i
					Exit For
				End If
			Next 
			If w > 0 Then
				Select Case LCase(anime)
					Case "swing.wav"
						If InStr(main_situation, "��") > 0 Or InStr(main_situation, "�X�s�A") > 0 Or InStr(main_situation, "�����X") > 0 Or InStr(main_situation, "�W���x����") > 0 Then
							ShowAnimation("�h�ˍU��")
							Exit Sub
						ElseIf IsWeaponClassifiedAs(w, "��") Or IsWeaponClassifiedAs(w, "��") Then 
							ShowAnimation("��������U��")
							Exit Sub
						End If
				End Select
			ElseIf InStr(main_situation, "(����)") > 0 Then 
				Select Case LCase(anime)
					Case "break.wav"
						ShowAnimation("�Ō�����")
						Exit Sub
					Case "combo.wav"
						ShowAnimation("���Ŗ���")
						Exit Sub
					Case "crash.wav"
						ShowAnimation("���Ŗ��� Crash.wav")
						Exit Sub
					Case "explode.wav"
						ShowAnimation("��������")
						Exit Sub
					Case "explode(far).wav"
						ShowAnimation("���������� Explode(Far).wav")
						Exit Sub
					Case "explode(nuclear).wav"
						ShowAnimation("���������� Explode(Nuclear).wav")
						Exit Sub
					Case "fire.wav"
						ShowAnimation("������")
						Exit Sub
					Case "glass.wav"
						If IsWeaponClassifiedAs(w, "��") Then
							ShowAnimation("�������� Glass.wav")
						End If
						Exit Sub
					Case "punch.wav"
						ShowAnimation("�Ō�����")
						Exit Sub
					Case "punch(2).wav", "punch(3).wav", "punch(4).wav"
						ShowAnimation("�A�Ŗ���")
						Exit Sub
					Case "saber.wav", "slash.wav"
						ShowAnimation("�a������ " & anime)
						Exit Sub
					Case "shock(low).wav"
						ShowAnimation("���Ŗ��� Shock(Low).wav")
						Exit Sub
					Case "stab.wav"
						ShowAnimation("�h�˖���")
						Exit Sub
					Case "thunder.wav"
						ShowAnimation("���d���� Thunder.wav")
						Exit Sub
					Case "whip.wav"
						ShowAnimation("�Ō����� Whip.wav")
						Exit Sub
				End Select
			End If
		End If
		
		'���b�Z�[�W�E�B���h�E�͕\������Ă���H
		is_message_form_opened = frmMessage.Visible
		
		'�I�u�W�F�N�g�F�����L�^���Ă���
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'�I�u�W�F�N�g�F�����f�t�H���g�ɖ߂�
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'��������V�`���G�[�V���������햼���ǂ������ׂ�
		For i = 1 To CountWeapon
			If main_situation = Weapon(i).Name Then
				is_weapon = True
				Exit For
			End If
		Next 
		
		'��������V�`���G�[�V�������A�r���e�B���ǂ������ׂ�
		For i = 1 To CountAbility
			If main_situation = Ability(i).Name Then
				is_ability = True
				Exit For
			End If
		Next 
		
		'�C�x���g�p�^�[�Q�b�g���L�^���Ă���
		prev_selected_target = SelectedTargetForEvent
		
		'�U���ł��A�r���e�B�ł��Ȃ��ꍇ�A�^�[�Q�b�g���ݒ肳��Ă��Ȃ����
		'�������g���^�[�Q�b�g�ɐݒ肷��
		'(�����A�j���ł̓A�j���\����SelectedTargetForEvent���g���邽��)
		If Not is_weapon And Not is_ability Then
			If SelectedTargetForEvent Is Nothing Then
				SelectedTargetForEvent = Me
			End If
		End If
		
		'������ʎw��𕪊�
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
			
			'���]��
			FormatMessage(anime)
			
			'��ʃN���A�H
			If LCase(anime) = "clear" Then
				ClearPicture()
				need_refresh = True
				GoTo NextAnime
			End If
			
			'�������
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'���ʉ�
					PlayWave(anime)
					If wait_time > 0 Then
						If need_refresh Then
							'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
							MainForm.picMain(0).Refresh()
							need_refresh = False
						End If
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'�J�b�g�C���̕\��
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
					'��ʏ����R�}���h
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
					'�w�肵�����j�b�g�𒆉��\��
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
					'���̂܂܏I��
					Exit For
			End Select
			
			'�E�F�C�g�H
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'���b�Z�[�W�\���Ƃ��ď���
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
		
		If BattleAnimation And Not IsPictureDrawn And Not IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			If w > 0 Then
				ShowAnimation("�f�t�H���g�U��")
			ElseIf InStr(main_situation, "(����)") > 0 Then 
				ShowAnimation("�_���[�W���� -.wav")
			End If
		End If
		
		'������ʍĐ���ɃE�F�C�g������H
		If need_refresh Then
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
			need_refresh = False
		End If
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'�摜���������Ă���
		If IsPictureDrawn And InStr(main_situation, "(����)") = 0 And LCase(anime) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
		
		'�ŏ�����\������Ă����̂łȂ���΃��b�Z�[�W�E�B���h�E�����
		If Not is_message_form_opened And Not keep_message_form Then
			CloseMessageForm()
		End If
		
		'�I�u�W�F�N�g�F�������ɖ߂�
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		'�C�x���g�p�^�[�Q�b�g�����ɖ߂�
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
	
	'������ʃf�[�^����`����Ă��邩�H
	Public Function IsSpecialEffectDefined(ByRef main_situation As String, Optional ByRef sub_situation As String = "") As Boolean
		Dim msg As String
		
		msg = SpecialEffectData(main_situation, sub_situation)
		
		If Len(msg) > 0 Then
			IsSpecialEffectDefined = True
		End If
	End Function
	
	
	' === �퓬�A�j���֘A���� ===
	
	'�퓬�A�j���f�[�^������
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
		
		'�V�`���G�[�V�����̃��X�g���\�z
		If sub_situation = "" Or main_situation = sub_situation Then
			ReDim situations(1)
			situations(1) = main_situation
		Else
			ReDim situations(2)
			situations(1) = main_situation & "(" & sub_situation & ")"
			situations(2) = main_situation
		End If
		
		For i = 1 To UBound(situations)
			'�퓬�A�j���\�͂Ŏw�肳�ꂽ���̂Ō���
			If IsFeatureAvailable("�퓬�A�j��") Then
				uname = FeatureData("�퓬�A�j��")
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
			
			'���j�b�g���̂Ō���
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
			
			'���j�b�g���̂��C���������̂Ō���
			uname = Nickname0
			ret = InStr(uname, "(")
			If ret > 1 Then
				uname = Left(uname, ret - 1)
			End If
			ret = InStr(uname, "�p")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			ret = InStr(uname, "�^")
			If ret > 0 Then
				If ret < Len(uname) Then
					uname = Mid(uname, ret + 1)
				End If
			End If
			If Right(uname, 4) = "�J�X�^��" Then
				uname = Left(uname, Len(uname) - 4)
			End If
			If Right(uname, 1) = "��" Then
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
			
			'���j�b�g�N���X�Ō���
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
			
			'�ėp
			' MOD START MARGE
			'        If ADList.IsDefined("�ėp") Then
			'            AnimationData = ADList.Item("�ėp").SelectMessage(situations(i), Me)
			'            If Len(AnimationData) > 0 Then
			'                Exit Function
			'            End If
			'        End If
			If ExtendedAnimation Then
				If EADList.IsDefined("�ėp") Then
					AnimationData = EADList.Item("�ėp").SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			If Not ext_anime_only Then
				If ADList.IsDefined("�ėp") Then
					AnimationData = ADList.Item("�ėp").SelectMessage(situations(i), Me)
					If Len(AnimationData) > 0 Then
						Exit Function
					End If
				End If
			End If
			' MOD END MARGE
		Next 
	End Function
	
	'�퓬�A�j�����Đ�
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
		
		'�퓬�A�j���f�[�^������
		anime = AnimationData(main_situation, sub_situation)
		
		'������Ȃ������ꍇ�͈ꊇ�w��������Ă݂�
		If anime = "" Then
			Select Case Right(main_situation, 4)
				Case "(����)", "(�U��)", "(����)"
					anime = AnimationData(Left(main_situation, Len(main_situation) - 4), sub_situation)
					in_bulk = True
				Case "(����)"
					anime = AnimationData(Left(main_situation, Len(main_situation) - 4), sub_situation)
			End Select
		End If
		
		TrimString(anime)
		
		'�\���L�����Z��
		If anime = "" Or anime = "-" Then
			Exit Sub
		End If
		
		'�}�E�X�̉E�{�^���ŃL�����Z��
		If IsRButtonPressed() Then
			' MOD START MARGE
			'        '���]���̂ݍs��
			'        FormatMessage anime
			'        Exit Sub
			'�A�j���̏I�������̓L�����Z�����Ȃ�
			If main_situation <> "�I��" And Right(main_situation, 4) <> "(�I��)" Then
				'���]���̂ݍs��
				FormatMessage(anime)
				Exit Sub
			End If
			' MOD END MARGE
		End If
		
		'���b�Z�[�W�E�B���h�E�͕\������Ă���H
		is_message_form_opened = frmMessage.Visible
		
		'�I�u�W�F�N�g�F�����L�^���Ă���
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'�I�u�W�F�N�g�F�����f�t�H���g�ɖ߂�
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'��������V�`���G�[�V���������햼���ǂ������ׂ�
		For i = 1 To CountWeapon
			If main_situation = Weapon(i).Name & "(�U��)" Then
				is_weapon = True
				Exit For
			End If
		Next 
		
		'��������V�`���G�[�V�������A�r���e�B���ǂ������ׂ�
		For i = 1 To CountAbility
			If main_situation = Ability(i).Name & "(����)" Then
				is_ability = True
				Exit For
			End If
		Next 
		
		'�C�x���g�p�^�[�Q�b�g���L�^���Ă���
		prev_selected_target = SelectedTargetForEvent
		
		'�U���ł��A�r���e�B�ł��Ȃ��ꍇ�A�^�[�Q�b�g���ݒ肳��Ă��Ȃ����
		'�������g���^�[�Q�b�g�ɐݒ肷��
		'(�����A�j���ł̓A�j���\����SelectedTargetForEvent���g���邽��)
		If Not is_weapon And Not is_ability Then
			If SelectedTargetForEvent Is Nothing Then
				SelectedTargetForEvent = Me
			End If
		End If
		
		'�A�j���w��𕪊�
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
			
			'�Ō�Ɏ��s���ꂽ�̂��T�u���[�`���Ăяo�����ǂ����𔻒肷�邽��
			'�T�u���[�`���������炩���߃N���A���Ă���
			sname = ""
			
			'���]��
			FormatMessage(anime)
			
			'��ʃN���A�H
			If LCase(anime) = "clear" Then
				ClearPicture()
				need_refresh = True
				GoTo NextAnime
			End If
			
			'�퓬�A�j���ȊO�̓������
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'���ʉ�
					PlayWave(anime)
					If wait_time > 0 Then
						If need_refresh Then
							'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
							MainForm.picMain(0).Refresh()
							need_refresh = False
						End If
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'�J�b�g�C���̕\��
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
					'��ʏ����R�}���h
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
					'�w�肵�����j�b�g�𒆉��\��
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
					'���̂܂܏I��
					Exit For
			End Select
			
			'�E�F�C�g�H
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'�T�u���[�`���̌Ăяo�����m��
			
			'�퓬�A�j���Đ��p�̃T�u���[�`�������쐬
			sname = LIndex(anime, 1)
			If Left(sname, 1) = "@" Then
				sname = Mid(sname, 2)
			ElseIf is_weapon Then 
				'���햼�̏ꍇ
				sname = "�퓬�A�j��_" & sname & "�U��"
			Else
				'���̑��̏ꍇ
				'���ʂ��܂񂾕��햼�ɑΉ����邽�߁A"("�͌�납�猟��
				idx = InStr2(main_situation, "(")
				
				'�ό`�n�̃V�`���G�[�V�����ł̓T�t�B�b�N�X�𖳎�
				If idx > 0 Then
					Select Case Left(main_situation, idx - 1)
						Case "�ό`", "�n�C�p�[���[�h", "�m�[�}�����[�h", "�p�[�c����", "����", "����"
							idx = 0
					End Select
				End If
				
				'���햼(�U��������)�̏ꍇ���T�t�B�b�N�X�𖳎�
				If idx > 0 Then
					If Mid(main_situation, idx) = "(�U��������)" Then
						idx = 0
					End If
				End If
				
				If idx > 0 Then
					'�T�t�B�b�N�X����
					sname = "�퓬�A�j��_" & sname & Mid(main_situation, idx + 1, Len(main_situation) - idx - 1)
				Else
					sname = "�퓬�A�j��_" & sname & "����"
				End If
			End If
			
			'�T�u���[�`����������Ȃ�����
			If FindNormalLabel(sname) = 0 Then
				If in_bulk Then
					'�ꊇ�w��𗘗p���Ă���ꍇ
					Select Case Right(main_situation, 4)
						Case "(����)"
							'�\�����L�����Z��
							GoTo NextAnime
						Case "(�U��)"
							'�����̃A�j���w�肪����ꍇ�͒��߂đ��̂��̂��g��
							If UBound(animes) > 1 Then
								GoTo NextAnime
							End If
							'�����łȂ���΁u�f�t�H���g�v���g�p
							sname = "�퓬�A�j��_�f�t�H���g�U��"
						Case "(����)"
							'�����̃A�j���w�肪����ꍇ�͒��߂đ��̂��̂��g��
							If UBound(animes) > 1 Then
								GoTo NextAnime
							End If
							'�����łȂ���΁u�_���[�W�v���g�p
							sname = "�퓬�A�j��_�_���[�W����"
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
			
			'�����̍\�z
			For j = 2 To ListLength(anime)
				sname = sname & "," & ListIndex(anime, j)
			Next 
			If in_bulk Then
				sname = sname & ",`�ꊇ�w��`"
			End If
			
			'�퓬�A�j���Đ��O�ɃE�F�C�g������
			If need_refresh Then
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
				need_refresh = False
			End If
			If wait_time > 0 Then
				Sleep(wait_time)
				wait_time = 0
			End If
			
			'�摜�`�悪�s��ꂽ���ǂ����̔���̂��߂Ƀt���O��������
			IsPictureDrawn = False
			
			'�퓬�A�j���Đ�
			SaveBasePoint()
			CallFunction("Call(" & sname & ")", Expression.ValueType.StringType, buf, ret)
			RestoreBasePoint()
			
			'�摜���������Ă���
			If IsPictureDrawn And LCase(buf) <> "keep" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
NextAnime: 
		Next 
		
		'�퓬�A�j���Đ���ɃE�F�C�g������H
		If need_refresh Then
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
			need_refresh = False
		End If
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'�摜���������Ă���
		If IsPictureDrawn And sname = "" And InStr(main_situation, "(����)") = 0 And LCase(anime) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
		
		'�ŏ�����\������Ă����̂łȂ���΃��b�Z�[�W�E�B���h�E�����
		If Not is_message_form_opened And Not keep_message_form Then
			CloseMessageForm()
		End If
		
		'�I�u�W�F�N�g�F�������ɖ߂�
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		'�C�x���g�p�^�[�Q�b�g�����ɖ߂�
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
	
	'�퓬�A�j������`����Ă��邩�H
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
	
	
	
	'���j�b�g��(new_x,new_y)�ɔz�u
	Public Sub StandBy(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal smode As String = "")
		Dim j, i, k As Short
		Dim u As Unit
		
		'�Ƃ肠�����n�`���l�������Ƀf�t�H���g�̃|�W�V���������߂Ă���
		'(Create�R�}���h�̌�ŋ󒆈ړ��p�A�C�e����t����Ƃ��̂���)
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
		
		'�󂢂��ꏊ������
		For i = 0 To 20
			'���j�b�g���m��אڂ������ɔz�u����H
			' MOD START MARGE
			'        If smode = "�����z�u" Then
			If InStr(smode, "�����z�u") > 0 Then
				' MOD END MARGE
				If i Mod 2 <> 0 Then
					GoTo NextDistance
				End If
			End If
			'�w�肵���ꏊ�̎���𒲂ׂ�
			For j = MaxLng(new_x - i, 1) To MinLng(new_x + i, MapWidth)
				For k = MaxLng(new_y - i, 1) To MinLng(new_y + i, MapHeight)
					If System.Math.Abs(new_x - j) + System.Math.Abs(new_y - k) <> i Then
						GoTo NextLoop
					End If
					
					'���ɑ��̃��j�b�g������H
					If Not MapDataForUnit(j, k) Is Nothing Then
						GoTo NextLoop
					End If
					
					'�i���s�\�̒n�`�H
					If TerrainMoveCost(j, k) > 100 Then
						GoTo NextLoop
					End If
					Select Case TerrainClass(j, k)
						Case "��"
							If Not IsTransAvailable("��") Then
								GoTo NextLoop
							End If
						Case "��"
							If Not IsTransAvailable("����") And Not IsTransAvailable("��") And Adaption(3) = 0 Then
								GoTo NextLoop
							End If
						Case "�[��"
							If Not IsTransAvailable("����") And Not IsTransAvailable("��") And Not IsTransAvailable("��") Then
								GoTo NextLoop
							End If
					End Select
					
					'�󂫈ʒu����������
					x = j
					y = k
					GoTo ExitFor
NextLoop: 
				Next 
			Next 
NextDistance: 
		Next 
ExitFor: 
		
		'�󂢂��ꏊ���Ȃ������H
		If x = 0 And y = 0 Then
			Status_Renamed = "�ҋ@"
			Exit Sub
		End If
		
		'���̌`�ԂƊi�[�������j�b�g�̍��W�����킹�Ă���
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
		
		'�i�[����Ă����ꍇ�͂��炩���ߍ~�낵�Ă���
		If Status_Renamed = "�i�[" Then
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
		
		'Status���X�V
		Status_Renamed = "�o��"
		For i = 1 To CountOtherForm
			OtherForm(i).Status_Renamed = "���`��"
		Next 
		
		'���j�b�g�̂���n�`�́H
		Select Case TerrainClass(x, y)
			Case "��"
				Area = "��"
			Case "��"
				If IsTransAvailable("�n��") And Area = "�n��" Then
					Area = "�n��"
				ElseIf IsTransAvailable("��") And Adaption(1) >= Adaption(2) Then 
					Area = "��"
				ElseIf IsTransAvailable("��") Then 
					Area = "�n��"
				Else
					Area = "��"
				End If
			Case "����"
				If IsTransAvailable("��") And Adaption(1) >= Adaption(2) Then
					Area = "��"
				ElseIf IsTransAvailable("��") Then 
					Area = "�n��"
				Else
					Area = "��"
				End If
			Case "����"
				If IsTransAvailable("��") Or IsTransAvailable("�F��") Then
					Area = "�F��"
				ElseIf IsTransAvailable("��") Then 
					Area = "�n��"
				Else
					Area = "�F��"
				End If
			Case "��", "�[��"
				If IsTransAvailable("��") And Adaption(1) >= Adaption(2) Then
					Area = "��"
				ElseIf IsTransAvailable("����") Then 
					Area = "����"
				Else
					Area = "����"
				End If
			Case "�F��"
				Area = "�F��"
			Case Else
				Area = "�n��"
		End Select
		
		'�}�b�v�ɓo�^
		MapDataForUnit(x, y) = Me
		
		'�r�b�g�}�b�v���쐬
		If BitmapID = 0 Then
			BitmapID = MakeUnitBitmap(Me)
		End If
		
		'�o�ꎞ�A�j����\��
		' MOD START MARGE
		'    If (smode = "�o��" Or smode = "�����z�u") _
		''        And MainForm.Visible _
		''        And Not IsPictureVisible _
		''        And Not IsRButtonPressed() _
		''        And BitmapID > 0 _
		''    Then
		Dim fname As String
		Dim start_time, current_time As Integer
		If (InStr(smode, "�o��") > 0 Or InStr(smode, "�����z�u") > 0) And MainForm.Visible And Not IsPictureVisible And Not IsRButtonPressed() And BitmapID > 0 Then
			' MOD END MARGE
			
			'���j�b�g�o����
			PlayWave("UnitOn.wav")
			
			'�\��������摜
			Select Case Party0
				Case "����", "�m�o�b"
					fname = "Bitmap\Event\AUnitOn0"
				Case "�G"
					fname = "Bitmap\Event\EUnitOn0"
				Case "����"
					fname = "Bitmap\Event\NUnitOn0"
			End Select
			
			If FileExists(AppPath & fname & "1.bmp") Then
				'�A�j���\���J�n�������L�^
				start_time = timeGetTime()
				
				For i = 1 To 4
					'�摜�𓧉ߕ\��
					If DrawPicture(fname & VB6.Format(i) & ".bmp", MapToPixelX(x), MapToPixelY(y), 32, 32, 0, 0, 0, 0, "����") = False Then
						Exit For
					End If
					'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
					MainForm.picMain(0).Refresh()
					
					'�E�F�C�g
					Do 
						System.Windows.Forms.Application.DoEvents()
						current_time = timeGetTime()
					Loop While current_time < start_time + 15
					start_time = current_time
					
					'�摜������
					ClearPicture()
				Next 
				
				'�A�j���摜�͏㏑�����ď����Ă��܂��̂Łc�c
				IsPictureVisible = False
			End If
		End If
		
		'���j�b�g�摜���}�b�v�ɕ`��
		If Not IsPictureVisible And MapFileName <> "" Then
			' MOD START MARGE
			'        If smode = "�񓯊�" Then
			If InStr(smode, "�񓯊�") > 0 Then
				' MOD END MARGE
				PaintUnitBitmap(Me, "���t���b�V������")
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		
		'����s�\�H
		If IsFeatureAvailable("����s��") Then
			AddCondition("�\��", -1)
		End If
		
		Update()
		
		PList.UpdateSupportMod(Me)
	End Sub
	
	'���j�b�g��(new_x,new_y)�Ɉړ�
	Public Sub Move(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal without_en_consumption As Boolean = False, Optional ByVal by_cancel As Boolean = False, Optional ByVal by_teleport_or_jump As Boolean = False)
		Dim prev_x, prev_y As Short
		Dim i As Short
		
		'���j�b�g���}�b�v���炢������폜
		If MapDataForUnit(x, y) Is Me Then
			'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			MapDataForUnit(x, y) = Nothing
		End If
		If IsPictureVisible Then
			EraseUnitBitmap(x, y, False)
		Else
			EraseUnitBitmap(x, y, False)
		End If
		PList.UpdateSupportMod(Me)
		
		'���j�b�g�ʒu���w�肳�ꂽ���W��
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
		
		'�w�肳�ꂽ�ꏊ�Ɋ��Ƀ��j�b�g�����݁H
		If Not MapDataForUnit(x, y) Is Nothing Then
			With MapDataForUnit(x, y)
				'���́H
				For i = 1 To .CountFeature
					If .Feature(i) = "����" And LLength(.FeatureData(i)) = 3 Then
						If UList.IsDefined(LIndex(.FeatureData(i), 3)) Then
							If UList.Item(LIndex(.FeatureData(i), 3)).CurrentForm Is Me Then
								Combine()
								Exit Sub
							End If
						End If
					End If
				Next 
				
				'���́H
				If Not .IsFeatureAvailable("���") Then
					ErrorMessage("���̌����j�b�g�u" & Name & "�v���������邽�ߍ��̏������o���܂���")
					Exit Sub
				End If
			End With
			
			'���͏���
			Land(MapDataForUnit(x, y), by_cancel)
			Exit Sub
		End If
		
		'�ړ���ɂ�郆�j�b�g�ʒu�ύX
		Select Case TerrainClass(x, y)
			Case "��"
				Area = "��"
			Case "��", "����"
				Select Case Area
					Case "����", "����"
						Area = "�n��"
					Case "�F��"
						If IsTransAvailable("��") And Adaption(1) >= Adaption(2) Then
							Area = "��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "��"
						End If
				End Select
			Case "����"
				Select Case Area
					Case "�n��", "�n��"
						'�ύX�Ȃ�
					Case Else
						If (IsTransAvailable("��") Or IsTransAvailable("�F��")) And Adaption(4) >= Adaption(2) Then
							Area = "�F��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "�F��"
						End If
				End Select
			Case "��", "�[��"
				Select Case Area
					Case "�n��"
						If IsTransAvailable("����") Then
							Area = "����"
						Else
							Area = "����"
						End If
					Case "�F��"
						Area = "����"
				End Select
			Case "�F��"
				Area = "�F��"
		End Select
		
		'�}�b�v�Ƀ��j�b�g��o�^
		MapDataForUnit(x, y) = Me
		
		'���j�b�g�`��
		If Not IsPictureVisible Then
			If MoveAnimation And Not by_cancel And Not by_teleport_or_jump Then
				MoveUnitBitmap2(Me, 20)
			Else
				PaintUnitBitmap(Me)
			End If
		End If
		
		'�ړ��ɂ��d�m����
		If Not without_en_consumption Then
			Select Case Area
				Case "�n��", "����"
					If IsFeatureAvailable("�z�o�[�ړ�") Then
						EN = EN - 5
					End If
				Case "��", "�F��"
					EN = EN - 5
				Case "�n��"
					EN = EN - 10
			End Select
		End If
		
		'���X�V
		Update()
		PList.UpdateSupportMod(Me)
	End Sub
	
	'���j�b�g��(new_x,new_y)�ɃW�����v
	Public Sub Jump(ByVal new_x As Short, ByVal new_y As Short, Optional ByVal do_refresh As Boolean = True)
		Dim j, i, k As Short
		
		'���j�b�g����U�}�b�v����폜
		'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		MapDataForUnit(x, y) = Nothing
		EraseUnitBitmap(x, y, do_refresh)
		PList.UpdateSupportMod(Me)
		
		'�󂫈ʒu������
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
						Case "��"
							If Not IsTransAvailable("��") Then
								GoTo NextLoop
							End If
						Case "��", "�[��"
							If Not IsTransAvailable("����") And Not IsTransAvailable("��") And Adaption(3) = 0 Then
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
		
		'���̌`�ԂƊi�[�������j�b�g�̍��W���X�V
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
		
		'�ړ���ɂ�郆�j�b�g�ʒu�ύX
		Select Case TerrainClass(x, y)
			Case "��"
				Area = "��"
			Case "��", "����"
				Select Case Area
					Case "����", "����"
						Area = "�n��"
					Case "�F��"
						If IsTransAvailable("��") And Adaption(1) >= Adaption(2) Then
							Area = "��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "��"
						End If
				End Select
			Case "����"
				Select Case Area
					Case "�n��", "�n��"
						'�ύX�Ȃ�
					Case Else
						If (IsTransAvailable("��") Or IsTransAvailable("�F��")) And Adaption(4) >= Adaption(2) Then
							Area = "�F��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "�F��"
						End If
				End Select
			Case "��", "�[��"
				Select Case Area
					Case "�n��"
						If IsTransAvailable("����") Then
							Area = "����"
						Else
							Area = "����"
						End If
					Case "�F��"
						Area = "����"
				End Select
			Case "�F��"
				Area = "�F��"
		End Select
		
		'�}�b�v�Ƀ��j�b�g��o�^
		MapDataForUnit(x, y) = Me
		
		'���X�V
		Update()
		PList.UpdateSupportMod(Me)
		
		'���j�b�g�`��
		If do_refresh Then
			PaintUnitBitmap(Me)
		End If
	End Sub
	
	'�}�b�v�ォ��E�o
	Public Sub Escape(Optional ByVal smode As String = "")
		Dim u As Unit
		Dim i, j As Short
		
		'��͂ɏ���Ă����ꍇ�͍~��Ă���
		If Status_Renamed = "�i�[" Then
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
		
		'�o�����Ă���ꍇ�͉�ʏォ�烆�j�b�g������
		If Status_Renamed = "�o��" Or Status_Renamed = "�j��" Then
			If MapDataForUnit(x, y) Is Me Then
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(x, y) = Nothing
				If smode = "�񓯊�" Or IsPictureVisible Or MapFileName = "" Then
					EraseUnitBitmap(x, y, False)
				Else
					EraseUnitBitmap(x, y, True)
				End If
				PList.UpdateSupportMod(Me)
			End If
		End If
		
		If Status_Renamed = "�o��" Or Status_Renamed = "�i�[" Then
			Status_Renamed = "�ҋ@"
		End If
		
		'�j����L�����Z����Ԃ͉���
		If IsConditionSatisfied("�j��L�����Z��") Then
			DeleteCondition("�j��L�����Z��")
		End If
		
		'���j�b�g���i�[���Ă�����~�낷
		For	Each u In colUnitOnBoard
			With u
				.Status_Renamed = "�ҋ@"
				colUnitOnBoard.Remove(.ID)
			End With
		Next u
		
		'�����������j�b�g�����
		DismissServant()
		
		'�����E�߈˂������j�b�g�����
		DismissSlave()
		
		'�X�e�[�^�X�\�����̏ꍇ�͕\��������
		If Me Is DisplayedUnit Then
			ClearUnitStatus()
		End If
	End Sub
	
	'��� u �ɒ���
	Public Sub Land(ByRef u As Unit, Optional ByVal by_cancel As Boolean = False, Optional ByVal is_event As Boolean = False)
		Dim tclass As String
		Dim i As Short
		
		'Land�R�}���h�Œ��͂����ꍇ
		If is_event Then
			If Status_Renamed = "�o��" Or Status_Renamed = "�i�[" Then
				Escape()
			Else
				'�o���̂��߂̑O����
				
				'���j�b�g�����݂���ʒu������
				If u.Status_Renamed = "�o��" Then
					tclass = TerrainClass(u.x, u.y)
				Else
					tclass = TerrainClass(MapWidth \ 2, MapHeight \ 2)
				End If
				Select Case tclass
					Case "��"
						Area = "��"
					Case "��", "����"
						If IsTransAvailable("��") And Mid(strAdaption, 1, 1) = "A" Then
							Area = "��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "��"
						End If
					Case "����"
						If (IsTransAvailable("��") Or IsTransAvailable("�F��")) And Mid(strAdaption, 4, 1) = "A" Then
							Area = "�F��"
						ElseIf IsTransAvailable("��") Then 
							Area = "�n��"
						Else
							Area = "�F��"
						End If
					Case "��", "�[��"
						If IsTransAvailable("��") Then
							Area = "��"
						ElseIf IsTransAvailable("����") Then 
							Area = "����"
						Else
							Area = "����"
						End If
					Case "�F��"
						Area = "�F��"
				End Select
				
				'�s���񐔓�����
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
				
				If IsFeatureAvailable("����s��") Then
					AddCondition("�\��", -1)
				End If
			End If
		End If
		
		'��͂Ɏ������g���i�[
		u.LoadUnit(Me)
		
		'���W���͂ɍ��킹��
		x = u.x
		y = u.y
		
		Status_Renamed = "�i�["
		If Area <> "�F��" And Area <> "��" Then
			Area = "�n��"
		End If
		
		'�C�͌���
		If Not by_cancel Then
			With MainPilot
				If .Personality <> "�@�B" Then
					If IsOptionDefined("��͎��[���C�͒ቺ��") Then
						.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
					Else
						.Morale = .Morale - 5
					End If
				End If
			End With
			For i = 1 To CountPilot
				With Pilot(i)
					If MainPilot.ID <> .ID And .Personality <> "�@�B" Then
						If IsOptionDefined("��͎��[���C�͒ቺ��") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			Next 
			For i = 1 To CountSupport
				With Support(i)
					If .Personality <> "�@�B" Then
						If IsOptionDefined("��͎��[���C�͒ቺ��") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			Next 
			If IsFeatureAvailable("�ǉ��T�|�[�g") Then
				With AdditionalSupport
					If .Personality <> "�@�B" Then
						If IsOptionDefined("��͎��[���C�͒ቺ��") Then
							.Morale = MinLng(.Morale, MaxLng(.Morale - 5, 100))
						Else
							.Morale = .Morale - 5
						End If
					End If
				End With
			End If
		End If
	End Sub
	
	' new_form �֕ό`�i�����A�n�C�p�[���[�h�A�p�[�c���������̂��܂ށj
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
		If Status_Renamed <> "�j��" Then
			Status_Renamed = "���`��"
		End If
		
		'����s�\�Ȍ`�Ԃ��猳�ɖ߂�ꍇ�͖\��������
		If IsFeatureAvailable("����s��") Then
			If IsConditionSatisfied("�\��") Then
				DeleteCondition("�\��")
			End If
		End If
		
		'���̌`�Ԃɖ߂�H
		If LIndex(FeatureData("�m�[�}�����[�h"), 1) = new_form Then
			If IsConditionSatisfied("�m�[�}�����[�h�t��") Then
				'�ϐg��������ꍇ
				If MapFileName <> "" Then
					For i = 2 To LLength(FeatureData("�m�[�}�����[�h"))
						Select Case LIndex(FeatureData("�m�[�}�����[�h"), i)
							Case "���Ղ���"
								AddCondition("����", 1)
							Case "�C�͒ቺ"
								IncreaseMorale(-10)
						End Select
					Next 
				End If
				
				DeleteCondition("�m�[�}�����[�h�t��")
				
				If IsConditionSatisfied("�\�̓R�s�[") Then
					DeleteCondition("�\�̓R�s�[")
					DeleteCondition("�p�C���b�g�摜")
					DeleteCondition("���b�Z�[�W")
				End If
			Else
				'�n�C�p�[���[�h��������ꍇ
				If MapFileName <> "" Then
					AddCondition("����", 1)
					For i = 2 To LLength(FeatureData("�m�[�}�����[�h"))
						Select Case LIndex(FeatureData("�m�[�}�����[�h"), i)
							Case "���ՂȂ�"
								DeleteCondition("����")
							Case "�C�͒ቺ"
								IncreaseMorale(-10)
						End Select
					Next 
				End If
			End If
			
			If IsConditionSatisfied("�c�莞��") Then
				DeleteCondition("�c�莞��")
			End If
		End If
		
		'�퓬�A�j���ŕύX���ꂽ���j�b�g�摜�����ɖ߂�
		If IsConditionSatisfied("���j�b�g�摜") Then
			DeleteCondition("���j�b�g�摜")
			BitmapID = MakeUnitBitmap(Me)
		End If
		If IsConditionSatisfied("��\���t��") Then
			DeleteCondition("��\���t��")
			BitmapID = MakeUnitBitmap(Me)
		End If
		
		Dim eu As Unit
		Dim counter As Short
		With u
			'�p�����[�^�󂯌p��
			.BossRank = BossRank
			.Rank = Rank
			.Mode = Mode
			.Area = Area
			.UsedSupportAttack = UsedSupportAttack
			.UsedSupportGuard = UsedSupportGuard
			.UsedSyncAttack = UsedSyncAttack
			.UsedCounterAttack = UsedCounterAttack
			
			.Master = Master
			'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Master = Nothing
			.Summoner = Summoner
			'UPGRADE_NOTE: �I�u�W�F�N�g Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Summoner = Nothing
			
			'�A�C�e���󂯌p��
			For i = 1 To CountItem
				.AddItem0(Item(i))
			Next 
			
			'�X�y�V�����p���[���ʂ̃R�s�[
			CopySpecialPowerInEffect(u)
			RemoveAllSpecialPowerInEffect()
			
			'����X�e�[�^�X�̃R�s�[
			For i = 1 To .CountCondition
				.DeleteCondition0(1)
			Next 
			For i = 1 To CountCondition
				If ConditionLifetime(i) <> 0 And InStr(ConditionData(i), "�p�C���b�g�\�͕t��") = 0 And InStr(ConditionData(i), "�p�C���b�g�\�͋���") = 0 Then
					.AddCondition(Condition(i), ConditionLifetime(i), ConditionLevel(i), ConditionData(i))
				End If
			Next 
			For i = 1 To CountCondition
				DeleteCondition0(1)
			Next 
			
			'�p�C���b�g�̏悹����
			list = FeatureData("�ό`")
			If LLength(list) > 0 And Data.PilotNum = -LLength(list) And CountPilot = LLength(list) Then
				'�ό`�ɂ��p�C���b�g�̏��Ԃ��ω�����ꍇ
				For idx = 2 To LLength(list)
					If .Name = LIndex(list, idx) Then
						Exit For
					End If
				Next 
				If idx <= LLength(list) Then
					list = .FeatureData("�ό`")
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
					If IsFeatureAvailable("����") And .IsFeatureAvailable("����") Then
						For j = 2 To LLength(.FeatureData("����"))
							If LIndex(FeatureData("����"), .Support(i).SupportIndex + 1) = LIndex(.FeatureData("����"), j) Then
								.Support(i).SupportIndex = j - 1
								Exit For
							End If
						Next 
					End If
				End If
			Next 
			
			.Update()
			
			'�e���f�[�^���L�^
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
			
			'�e���̎󂯌p��
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
			
			'�e���E�g�p�񐔋��L�̎���
			.SyncBullet()
			
			'�A�C�e�����폜
			For i = 1 To CountItem
				DeleteItem(1)
			Next 
			
			.Update()
			
			'�g�o���d�m�̎󂯌p��
			If new_form = LIndex(FeatureData("�p�[�c����"), 2) Then
				.HP = .MaxHP
			Else
				.HP = .MaxHP * hp_ratio / 100
			End If
			.EN = .MaxEN * en_ratio / 100
			
			'�m�[�}�����[�h�␧�����Ԃ��̌`�Ԃ̏ꍇ�͎c�莞�Ԃ�t��
			If Not .IsConditionSatisfied("�c�莞��") Then
				If .IsFeatureAvailable("�m�[�}�����[�h") Then
					If IsNumeric(LIndex(.FeatureData("�m�[�}�����[�h"), 2)) Then
						If .IsConditionSatisfied("�c�莞��") Then
							.DeleteCondition("�c�莞��")
						End If
						.AddCondition("�c�莞��", CShort(LIndex(.FeatureData("�m�[�}�����[�h"), 2)))
					End If
				ElseIf .IsFeatureAvailable("��������") Then 
					.AddCondition("�c�莞��", CShort(.FeatureData("��������")))
				End If
			ElseIf Not .IsFeatureAvailable("�m�[�}�����[�h") And Not .IsFeatureAvailable("��������") Then 
				'�c�莞�Ԃ��K�v�Ȃ��`�Ԃ�Transform�R�}���h�ŋ����ό`���ꂽ�H
				.DeleteCondition("�c�莞��")
			End If
			
			Select Case .Status_Renamed
				Case "�o��"
					'�ό`��̃��j�b�g���o��������
					'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					MapDataForUnit(x, y) = Nothing
					prev_x = x
					prev_y = y
					.UsedAction = UsedAction
					.StandBy(x, y)
					If .x <> prev_x Or .y <> prev_y Then
						EraseUnitBitmap(prev_x, prev_y, False)
					End If
				Case "�i�["
					'�ό`��̃��j�b�g���i�[����
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
		
		'�n�C�p�[���[�h��������ꍇ
		buf = FeatureData("�m�[�}�����[�h")
		If LIndex(buf, 1) = new_form Then
			For i = 2 To LLength(buf)
				Select Case LIndex(buf, i)
					Case "�񐔐���"
						AddCondition("�s���s�\", -1)
				End Select
			Next 
		End If
	End Sub
	
	'����
	Public Sub Combine(Optional ByRef uname As String = "", Optional ByVal is_event As Boolean = False)
		Dim k, i, j, l As Short
		Dim u As Unit
		Dim rarray() As Unit
		Dim prev_status As String
		Dim hp_ratio, en_ratio As Double
		Dim fdata As String
		
		prev_status = Status_Renamed
		
		If uname = "" Then
			'���̌`�Ԃ��w�肳��ĂȂ���΂��̏ꏊ�ɂ��郆�j�b�g�ƂQ�̍���
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			u = Nothing
			For i = 1 To CountFeature
				If Feature(i) = "����" Then
					fdata = FeatureData(i)
					If LLength(fdata) = 3 And MapDataForUnit(x, y).Name = LIndex(fdata, 3) And UList.IsDefined(LIndex(fdata, 2)) Then
						u = UList.Item(LIndex(FeatureData(i), 2)).CurrentForm
						Exit For
					End If
				End If
			Next 
			If u Is Nothing Then
				For i = 1 To CountFeature
					If Feature(i) = "����" Then
						fdata = FeatureData(i)
						If LLength(fdata) = 3 And MapDataForUnit(x, y).IsEqual(LIndex(fdata, 3)) And UList.IsDefined(LIndex(fdata, 2)) Then
							u = UList.Item(LIndex(fdata, 2)).CurrentForm
							Exit For
						End If
					End If
				Next 
			End If
			
			'���̂̃p�[�g�i�[�𒲂ׂ�
			For i = 1 To u.CountFeature
				If u.Feature(i) = "����" And LLength(u.FeatureData(i)) = 3 And ((IsEqual(LIndex(u.FeatureData(i), 2)) And MapDataForUnit(x, y).IsEqual(LIndex(u.FeatureData(i), 3))) Or (IsEqual(LIndex(u.FeatureData(i), 3)) And MapDataForUnit(x, y).IsEqual(LIndex(u.FeatureData(i), 2)))) Then
					Exit For
				End If
			Next 
		Else
			'���̃��j�b�g���쐬����Ă��Ȃ�
			If Not UList.IsDefined(uname) Then
				ErrorMessage(uname & "���쐬����Ă��܂���")
				ExitGame()
			End If
			
			u = UList.Item(uname).CurrentForm
			
			'���̂̃p�[�g�i�[�𒲂ׂ�
			For i = 1 To u.CountFeature
				If u.Feature(i) = "����" And LLength(u.FeatureData(i)) > 2 Then
					Exit For
				End If
			Next 
		End If
		
		'���̂��郆�j�b�g�̔z����쐬
		If i > u.CountFeature Then
			ErrorMessage(u.Name & "�̃f�[�^��" & Name & "�ɑ΂��镪���w�肪�݂���܂���B" & "�������m�F���Ă��������B")
			Exit Sub
		End If
		ReDim rarray(LLength(u.FeatureData(i)) - 1)
		For j = 1 To UBound(rarray)
			If Not UList.IsDefined(LIndex(u.FeatureData(i), j + 1)) Then
				ErrorMessage(LIndex(u.FeatureData(i), j + 1) & "���쐬����Ă��܂���")
				Exit Sub
			End If
			rarray(j) = UList.Item(LIndex(u.FeatureData(i), j + 1))
		Next 
		
		Dim BGM As String
		If Not is_event Then
			If Status_Renamed = "�o��" Then
				'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
				If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
					If Not PList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
						If Not PDList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
							ErrorMessage(u.Name & "�̒ǉ��p�C���b�g�u" & u.FeatureData("�ǉ��p�C���b�g") & "�v�̃f�[�^��������܂���")
							TerminateSRC()
						End If
						PList.Add(u.FeatureData("�ǉ��p�C���b�g"), MainPilot.Level, Party0)
					End If
				End If
				
				If IsMessageDefined("����(" & u.Name & ")") Or IsMessageDefined("����(" & FeatureName("����") & ")") Or IsMessageDefined("����") Then
					If IsFeatureAvailable("���̂a�f�l") Then
						For i = 1 To CountFeature
							If Feature(i) = "���̂a�f�l" And LIndex(FeatureData(i), 1) = u.Name Then
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
					If IsMessageDefined("����(" & u.Name & ")") Then
						PilotMessage("����(" & u.Name & ")")
					ElseIf IsMessageDefined("����(" & FeatureName("����") & ")") Then 
						PilotMessage("����(" & FeatureName("����") & ")")
					Else
						PilotMessage("����")
					End If
					CloseMessageForm()
				End If
			End If
		End If
		
		'�������j�b�g�ƍ��̃��j�b�g�������̕�������ꍇ�͒e����ݐς��邽��
		'���̂悤�ȕ���̒e����0�ɂ���
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
		'�g�p�񐔂����킹��
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
		
		'�P�Ԗڂ̃��j�b�g�̃X�e�[�^�X�����̌�̃��j�b�g�Ɍp��
		With rarray(1).CurrentForm
			.CopySpecialPowerInEffect(u)
			.RemoveAllSpecialPowerInEffect()
			
			For i = 1 To .CountItem
				u.AddItem(.Item(i))
			Next 
			
			u.Master = .Master
			'UPGRADE_NOTE: �I�u�W�F�N�g rarray().CurrentForm.Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			.Master = Nothing
			u.Summoner = .Summoner
			'UPGRADE_NOTE: �I�u�W�F�N�g rarray().CurrentForm.Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
		
		'���̂���e���j�b�g�ɑ΂��Ă̏������s��
		Dim eu As Unit
		For i = 1 To UBound(rarray)
			'�}�b�v�ォ��P�ނ�����
			With rarray(i).CurrentForm
				Select Case .Status_Renamed
					Case "�o��"
						.Status_Renamed = "�ҋ@"
						'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						MapDataForUnit(.x, .y) = Nothing
						EraseUnitBitmap(.x, .y)
					Case "�i�["
						.Status_Renamed = "�ҋ@"
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
			
			'�f�t�H���g�̌`�Ԃɕό`�����Ă���
			If Not rarray(i).CurrentForm Is rarray(i) Then
				rarray(i).CurrentForm.Transform((rarray(i).Name))
			End If
			
			With rarray(i)
				If i = 1 Then
					.Status_Renamed = "����`��"
				Else
					.Status_Renamed = "���`��"
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
				
				If .IsFeatureAvailable("�������j�b�g") Then
					'�������j�b�g�̏ꍇ�̓p�C���b�g�̏悹�����͍s��Ȃ�
					If InStr(.MainPilot.Name, "(�U�R)") > 0 Or InStr(.MainPilot.Name, "(�ėp)") > 0 Then
						'�ėp�p�C���b�g�̏ꍇ�͍폜
						.MainPilot.Alive = False
					End If
				Else
					'�p�C���b�g�̏悹����
					For j = 1 To .CountPilot
						.Pilot(j).Ride(u)
					Next 
					For j = 1 To .CountPilot
						.DeletePilot(1)
					Next 
					
					'�T�|�[�g�̏悹����
					For j = 1 To .CountSupport
						.Support(j).Ride(u, True)
						.Support(j).SupportIndex = i
					Next 
					For j = 1 To .CountSupport
						.DeleteSupport(1)
					Next 
				End If
				
				'���ڃ��j�b�g�̏悹����
				For j = 1 To .CountUnitOnBoard
					u.LoadUnit(.UnitOnBoard(j))
				Next 
				For j = 1 To u.CountUnitOnBoard
					.UnloadUnit(1)
				Next 
				
				'�������j�b�g�Ƌ��ʂ��镐���̒e���͈�U0�ɃN���A
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
				
				'�A�r���e�B�̎g�p�񐔂����l�̏������s��
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
				
				'�X�y�V�����p���[�̌��ʂ�����
				.RemoveAllSpecialPowerInEffect()
			End With
		Next 
		
		'���̌�̃��j�b�g�̕����̒e���y�уA�r���e�B�̎g�p�񐔂͕������j�b�g��
		'�e���y�юg�p�񐔂̍��v�ɐݒ肷��
		For i = 1 To UBound(rarray)
			With rarray(i)
				'�����̒e���̏���
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
				
				'�A�r���e�B�̎g�p�񐔂̏���
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
		
		'�P�Ԗڂ̃��j�b�g�̃A�C�e�����O��
		With rarray(1)
			For i = 1 To .CountItem
				.DeleteItem(1)
			Next 
		End With
		
		'���̌�̃��j�b�g�Ɋւ��鏈��
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
			
			'�e���E�g�p�񐔋��L�̎���
			.SyncBullet()
			
			If prev_status = "�o��" Then
				.StandBy(x, y)
				
				'�m�[�}�����[�h�␧�����Ԃ��̌`�Ԃ̏ꍇ�͎c�莞�Ԃ�t��
				If .IsFeatureAvailable("�m�[�}�����[�h") Then
					If IsNumeric(LIndex(.FeatureData("�m�[�}�����[�h"), 2)) Then
						If .IsConditionSatisfied("�c�莞��") Then
							.DeleteCondition("�c�莞��")
						End If
						.AddCondition("�c�莞��", CShort(LIndex(.FeatureData("�m�[�}�����[�h"), 2)))
					End If
				ElseIf .IsFeatureAvailable("��������") Then 
					.AddCondition("�c�莞��", CShort(.FeatureData("��������")))
				End If
			Else
				.Status_Renamed = prev_status
			End If
		End With
		
		'�������j�b�g�̍��W�����̌�̃��j�b�g�̍��W�ɍ��킹��
		For i = 1 To UBound(rarray)
			With rarray(i).CurrentForm
				.x = u.x
				.y = u.y
			End With
		Next 
	End Sub
	
	'����
	'UPGRADE_NOTE: Split �� Split_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
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
		
		'�܂��͓P��
		If Status_Renamed = "�o��" Then
			'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			MapDataForUnit(x, y) = Nothing
			EraseUnitBitmap(x, y)
		End If
		
		'������̃��j�b�g�𒲂ׂ�
		buf = FeatureData("����")
		ReDim uarray(LLength(buf) - 1)
		For i = 2 To LLength(buf)
			uarray(i - 1) = UList.Item(LIndex(buf, i))
			If uarray(i - 1) Is Nothing Then
				ErrorMessage(LIndex(buf, i - 1) & "�����݂��܂���")
				Exit Sub
			End If
		Next 
		
		'������̂P�ԋ@������
		For i = 1 To UBound(uarray)
			If uarray(i).Status_Renamed = "����`��" Then
				Exit For
			End If
		Next 
		If i > UBound(uarray) Then
			i = 1
		End If
		
		'�P�ԋ@�Ɍ��݂̃X�e�[�^�X���p��
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
			'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Master = Nothing
			.Summoner = Summoner
			'UPGRADE_NOTE: �I�u�W�F�N�g Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
		
		'�e�������j�b�g�ɑ΂��鏈��
		n = 1
		Dim eu As Unit
		Dim counter As Short
		For i = 1 To UBound(uarray)
			With uarray(i)
				'�������j�b�g�łȂ��ꍇ�͐w�c�����킹��
				If Not .IsFeatureAvailable("�������j�b�g") Then
					.Party = Party0
				End If
				
				'�p�C���b�g�̓���
				If CountPilot > 0 Then
					For j = 1 To System.Math.Abs(.Data.PilotNum)
						If .IsFeatureAvailable("�������j�b�g") Then
							If Status_Renamed = "�o��" Or Status_Renamed = "�i�[" Then
								pname = .FeatureData("�ǉ��p�C���b�g")
								If InStr(PDList.Item(pname).Name, "(�U�R)") > 0 Or InStr(PDList.Item(pname).Name, "(�ėp)") > 0 Then
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
							ElseIf Not .IsFeatureAvailable("�ǉ��p�C���b�g") Then 
								If CountSupport > 0 Then
									Support(1).Ride(uarray(i))
									DeleteSupport(1)
								Else
									ErrorMessage(Name & "������̃��j�b�g�ɍڂ���" & "�p�C���b�g�����݂��܂���B" & "�f�[�^�̃p�C���b�g�����m�F���ĉ������B")
									TerminateSRC()
								End If
							End If
						End If
					Next 
				End If
				
				.Update()
				
				'��͂̏ꍇ�͊i�[�������j�b�g���󂯓n��
				If .IsFeatureAvailable("���") Then
					For j = 1 To CountUnitOnBoard
						.LoadUnit(UnitOnBoard(j))
					Next 
					For j = 1 To CountUnitOnBoard
						UnloadUnit(1)
					Next 
				End If
				
				'�g�o���d�m�̓���
				.HP = .MaxHP * hp_ratio / 100
				.EN = 1 * .MaxEN * en_ratio / 100
				
				'�e�������킹��
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
				
				'�g�p�񐔂����킹��
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
				
				'�e���E�g�p�񐔋��L�̎���
				.SyncBullet()
				
				'�o�� or �i�[�H
				.Status_Renamed = Status_Renamed
				Select Case Status_Renamed
					Case "�o��"
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
					Case "�i�["
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
				
				'�m�[�}�����[�h�␧�����Ԃ��̌`�Ԃ̏ꍇ�͎c�莞�Ԃ�t��
				If .IsFeatureAvailable("�m�[�}�����[�h") Then
					If IsNumeric(LIndex(.FeatureData("�m�[�}�����[�h"), 2)) Then
						If .IsConditionSatisfied("�c�莞��") Then
							.DeleteCondition("�c�莞��")
						End If
						.AddCondition("�c�莞��", CShort(LIndex(.FeatureData("�m�[�}�����[�h"), 2)))
					End If
				ElseIf .IsFeatureAvailable("��������") Then 
					.AddCondition("�c�莞��", CShort(.FeatureData("��������")))
				End If
			End With
		Next 
		
		'�p�C���b�g�����̃��j�b�g����폜
		For i = 1 To CountPilot
			DeletePilot(1)
		Next 
		
		'�T�|�[�g�p�C���b�g�̏�芷��
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
		
		'�i�[����Ă���ꍇ�͕�͂��玩���̃G���g���[���O���Ă���
		If Status_Renamed = "�i�[" Then
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
		
		Status_Renamed = "���`��"
		
		'���j�b�g�X�e�[�^�X�R�}���h�̏ꍇ�ȊO�͐������ԕt�����̃��j�b�g��
		'�Q�x�Ƃ��̌`�Ԃ𗘗p�ł��Ȃ�
		If MapFileName = "" Then
			Exit Sub
		End If
		If IsFeatureAvailable("��������") Then
			AddCondition("�s���s�\", -1)
		End If
	End Sub
	
	'�o���l�����
	't:�^�[�Q�b�g
	'exp_situation:�o���l����̗��R
	'exp_mode:�}�b�v�U���ɂ�����H
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
		
		'�o���l����肷��͖̂������j�b�g�y�тm�o�b�̏������j�b�g�̂�
		If (Party <> "����" Or Party0 <> "����") And (Party <> "�m�o�b" Or Party0 <> "�m�o�b" Or Not IsFeatureAvailable("�������j�b�g")) Then
			Exit Function
		End If
		
		'���C���p�C���b�g�̌��݂̔\�͂��L�^
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
				prev_slevel(i) = .SkillLevel(i, "��{�l")
			Next 
		End With
		
		'�^�[�Q�b�g���w�肳��Ă��Ȃ��ꍇ�͎������^�[�Q�b�g
		If t Is Nothing Then
			t = Me
		End If
		
		'�^�[�Q�b�g�Ƀp�C���b�g������Ă��Ȃ��ꍇ�͌o���l�Ȃ�
		If t.CountPilot = 0 Then
			Exit Function
		End If
		
		'���j�b�g�ɏ���Ă���p�C���b�g�������v�Z
		n = CountPilot + CountSupport
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			n = n + 1
		End If
		
		'�e�p�C���b�g���o���l�����
		For i = 1 To n
			If i <= CountPilot Then
				p = Pilot(i)
			ElseIf i <= CountPilot + CountSupport Then 
				p = Support(i - CountPilot)
			Else
				p = AdditionalSupport
			End If
			
			Select Case exp_situation
				Case "�j��"
					xp = t.ExpValue + t.MainPilot.ExpValue
					If IsUnderSpecialPowerEffect("�l���o���l����") And exp_mode <> "�p�[�g�i�[" Then
						xp = xp * (1 + 0.1 * SpecialPowerEffectLevel("�l���o���l����"))
					End If
				Case "�U��"
					xp = (t.ExpValue + t.MainPilot.ExpValue) \ 10
					If IsUnderSpecialPowerEffect("�l���o���l����") And exp_mode <> "�p�[�g�i�[" Then
						xp = xp * (1 + 0.1 * SpecialPowerEffectLevel("�l���o���l����"))
					End If
				Case "�A�r���e�B"
					If t Is Me Then
						xp = 50
					Else
						xp = 100
					End If
				Case "�C��"
					xp = 100
				Case "�⋋"
					xp = 150
			End Select
			If Not IsUnderSpecialPowerEffect("�l���o���l����") Or IsOptionDefined("�������ʏd��") Then
				If p.IsSkillAvailable("�f��") Then
					If p.IsSkillLevelSpecified("�f��") Then
						xp = xp * (10 + p.SkillLevel("�f��")) \ 10
					Else
						xp = 1.5 * xp
					End If
				End If
			End If
			If p.IsSkillAvailable("�x����") Then
				xp = xp \ 2
			End If
			
			'�Ώۂ̃p�C���b�g�̃��x�����ɂ��C��
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
			
			'��Ԗڂ̃p�C���b�g���l�������o���l��Ԃ�
			If i = 1 Then
				GetExp = xp
			End If
		Next 
		
		'�ǉ��p�C���b�g�̏ꍇ�A��Ԗڂ̃p�C���b�g�Ƀ��x���A�o���l�����킹��
		If Not MainPilot Is Pilot(1) Then
			MainPilot.Level = Pilot(1).Level
			MainPilot.Exp = Pilot(1).Exp
		End If
		
		'��������o���l�����
		If Not Summoner Is Nothing Then
			Summoner.CurrentForm.GetExp(t, exp_situation, "�p�[�g�i�[")
		End If
		
		'�}�b�v�U���ɂ��o���l�����̏ꍇ�̓��b�Z�[�W�\�����ȗ�
		If exp_mode = "�}�b�v" Then
			Exit Function
		End If
		
		'�o���l���莞�̃��b�Z�[�W
		With MainPilot
			If .Level > prev_level Then
				'���x���A�b�v
				
				If IsAnimationDefined("���x���A�b�v") Then
					PlayAnimation("���x���A�b�v")
				ElseIf IsSpecialEffectDefined("���x���A�b�v") Then 
					SpecialEffect("���x���A�b�v")
				End If
				If IsMessageDefined("���x���A�b�v") Then
					PilotMessage("���x���A�b�v")
				End If
				
				msg = .Nickname & "�͌o���l[" & VB6.Format(GetExp) & "]���l���A" & "���x��[" & VB6.Format(.Level) & "]�Ƀ��x���A�b�v�B"
				
				'����\�͂̏K��
				For i = 1 To .CountSkill
					stype = .Skill(i)
					sname = .SkillName(i)
					If InStr(sname, "��\��") = 0 Then
						Select Case stype
							Case "������", "���", "�ǉ����x��", "���͏��L"
							Case "�r�o�����", "�X�y�V�����p���[��������", "�n���^�["
								For j = 1 To UBound(prev_stype)
									If stype = prev_stype(j) Then
										If sname = prev_sname(j) Then
											Exit For
										End If
									End If
								Next 
								If j > UBound(prev_stype) Then
									msg = msg & ";" & sname & "���K�������B"
								End If
							Case Else
								For j = 1 To UBound(prev_stype)
									If stype = prev_stype(j) Then
										Exit For
									End If
								Next 
								If j > UBound(prev_stype) Then
									msg = msg & ";" & sname & "���K�������B"
								ElseIf .SkillLevel(i, "��{�l") > prev_slevel(j) Then 
									msg = msg & ";" & prev_sname(j) & " => " & sname & "�B"
								End If
						End Select
					End If
				Next 
				
				'�X�y�V�����p���[�̏K��
				If .CountSpecialPower > UBound(prev_special_power) Then
					msg = msg & ";" & Term("�X�y�V�����p���[", Me)
					For i = 1 To .CountSpecialPower
						sname = .SpecialPower(i)
						For j = 1 To UBound(prev_special_power)
							If sname = prev_special_power(j) Then
								Exit For
							End If
						Next 
						If j > UBound(prev_special_power) Then
							msg = msg & "�u" & sname & "�v"
						End If
					Next 
					msg = msg & "���K�������B"
				End If
				
				DisplaySysMessage(msg)
				If MessageWait < 10000 Then
					Sleep(MessageWait)
				End If
				
				HandleEvent("���x���A�b�v", .ID)
				
				PList.UpdateSupportMod(Me)
			ElseIf GetExp > 0 Then 
				DisplaySysMessage(.Nickname & "��" & VB6.Format(GetExp) & "�̌o���l�𓾂��B")
			End If
		End With
	End Function
	
	'���j�b�g�̐w�c��ύX
	Public Sub ChangeParty(ByRef new_party As String)
		Dim i As Short
		
		'�w�c��ύX
		Party = new_party
		
		'�r�b�g�}�b�v����蒼��
		BitmapID = MakeUnitBitmap(Me)
		
		'�p�C���b�g�̐w�c��ύX
		For i = 1 To CountPilot
			Pilot(i).Party = new_party
		Next 
		For i = 1 To CountSupport
			Support(i).Party = new_party
		Next 
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			AdditionalSupport.Party = new_party
		End If
		
		'���`�Ԃ̐w�c��ύX
		For i = 1 To CountOtherForm
			OtherForm(i).Party = new_party
			OtherForm(i).BitmapID = 0
		Next 
		
		'�o�����H
		If Status_Renamed = "�o��" Then
			'�����̐w�c�̃X�e�[�W�Ȃ�s���\��
			If Party = Stage Then
				Rest()
			End If
			'�}�b�v��̃��j�b�g�摜���X�V
			PaintUnitBitmap(Me)
		End If
		
		PList.UpdateSupportMod(Me)
		
		'�v�l���[�h��ʏ��
		Mode = "�ʏ�"
	End Sub
	
	'���j�b�g�ɏ���Ă���p�C���b�g�̋C�͂�num��������
	'is_event:�C�x���g�ɂ��C�͑���(���i�𖳎����ċC�͑���)
	Public Sub IncreaseMorale(ByVal num As Short, Optional ByVal is_event As Boolean = False)
		Dim p As Pilot
		
		If CountPilot = 0 Then
			Exit Sub
		End If
		
		'���C���p�C���b�g
		With MainPilot
			If .Personality <> "�@�B" Or is_event Then
				.Morale = .Morale + num
			End If
		End With
		
		'�T�u�p�C���b�g
		For	Each p In colPilot
			With p
				If MainPilot.ID <> .ID And (.Personality <> "�@�B" Or is_event) Then
					.Morale = .Morale + num
				End If
			End With
		Next p
		
		'�T�|�[�g
		For	Each p In colSupport
			With p
				If .Personality <> "�@�B" Or is_event Then
					.Morale = .Morale + num
				End If
			End With
		Next p
		
		'�ǉ��T�|�[�g
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			With AdditionalSupport
				If .Personality <> "�@�B" Or is_event Then
					.Morale = .Morale + num
				End If
			End With
		End If
	End Sub
	
	'���j�b�g���j�󂳂ꂽ���̏���
	Public Sub Die(Optional ByVal without_update As Boolean = False)
		Dim i, j As Short
		Dim pname As String
		Dim p As Pilot
		
		HP = 0
		Status_Renamed = "�j��"
		
		'�j����L�����Z�����A�j��C�x���g���ŏ������������ꍇ
		If IsConditionSatisfied("�j��L�����Z��") Then
			DeleteCondition("�j��L�����Z��")
			GoTo SkipExplode
		End If
		
		'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		MapDataForUnit(x, y) = Nothing
		
		'�����\��
		ClearPicture()
		If IsAnimationDefined("�E�o") Then
			EraseUnitBitmap(x, y, False)
			PlayAnimation("�E�o")
		ElseIf IsSpecialEffectDefined("�E�o") Then 
			EraseUnitBitmap(x, y, False)
			SpecialEffect("�E�o")
		Else
			DieAnimation(Me)
		End If
		
SkipExplode: 
		
		'�����������j�b�g�����
		DismissServant()
		
		'�����E�߈˂������j�b�g�����
		DismissSlave()
		
		If Not Master Is Nothing Then
			Master.CurrentForm.DeleteSlave(ID)
			'UPGRADE_NOTE: �I�u�W�F�N�g Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Master = Nothing
		End If
		
		If Not Summoner Is Nothing Then
			Summoner.CurrentForm.DeleteServant(ID)
			'UPGRADE_NOTE: �I�u�W�F�N�g Summoner ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Summoner = Nothing
		End If
		
		'�x�z���Ă��郆�j�b�g�������ދp
		If IsFeatureAvailable("�x�z") Then
			For i = 2 To LLength(FeatureData("�x�z"))
				pname = LIndex(FeatureData("�x�z"), i)
				For	Each p In PList
					With p
						If .Name = pname Or .Nickname = pname Then
							If Not .Unit_Renamed Is Nothing Then
								If .Unit_Renamed.Status_Renamed = "�o��" Or .Unit_Renamed.Status_Renamed = "�i�[" Then
									.Unit_Renamed.Die(True)
								End If
							End If
						End If
					End With
				Next p
			Next 
		End If
		
		'���X�V
		If Not without_update Then
			PList.UpdateSupportMod(Me)
		End If
	End Sub
	
	'�X�y�V�����p���[�����ɂ�鎩��
	Public Sub SuicidalExplosion(Optional ByVal is_event As Boolean = False)
		Dim i, j As Short
		Dim prev_hp As Integer
		Dim u, t As Unit
		Dim dmg, tdmg As Integer
		Dim uname, fname As String
		
		PilotMessage("����")
		DisplaySysMessage(Nickname & "�͎��������B")
		
		'�_���[�W�ʐݒ�
		dmg = HP
		
		'���ʔ͈͂̐ݒ�
		AreaInRange(x, y, 1, 1, "")
		MaskData(x, y) = True
		
		'����
		EraseUnitBitmap(x, y)
		ExplodeAnimation(Size, x, y)
		
		'�p�[�c�����ł���Ύ�����Ƀp�[�c����
		If IsFeatureAvailable("�p�[�c����") Then
			uname = LIndex(FeatureData("�p�[�c����"), 2)
			If OtherForm(uname).IsAbleToEnter(x, y) Then
				Transform(uname)
				MapDataForUnit(x, y).HP = MapDataForUnit(x, y).MaxHP
				fname = FeatureName("�p�[�c����")
				If IsSysMessageDefined("�j�󎞕���(" & Name & ")") Then
					SysMessage("�j�󎞕���(" & Name & ")")
				ElseIf IsSysMessageDefined("�j�󎞕���(" & fname & ")") Then 
					SysMessage("�j�󎞕���(" & fname & ")")
				ElseIf IsSysMessageDefined("�j�󎞕���") Then 
					SysMessage("�j�󎞕���")
				ElseIf IsSysMessageDefined("����(" & Name & ")") Then 
					SysMessage("����(" & Name & ")")
				ElseIf IsSysMessageDefined("����(" & fname & ")") Then 
					SysMessage("����(" & fname & ")")
				ElseIf IsSysMessageDefined("����") Then 
					SysMessage("����")
				Else
					DisplaySysMessage(Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
				End If
				GoTo SkipSuicide
			End If
		End If
		
		'������j��
		HP = 0
		UpdateMessageForm(Me)
		'���ɔ����A�j���[�V������\�����Ă���̂�
		AddCondition("�j��L�����Z��", 1)
		'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		MapDataForUnit(x, y) = Nothing
		Die()
		If Not is_event Then
			u = SelectedUnit
			SelectedUnit = Me
			HandleEvent("�j��", MainPilot.ID)
			SelectedUnit = u
			If IsScenarioFinished Then
				IsScenarioFinished = False
				Exit Sub
			End If
		End If
		
SkipSuicide: 
		
		'����̃G���A�ɔ������ʂ�K�p
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
					If CurrentForm.Party = "����" Or CurrentForm.Party = "�m�o�b" Then
						UpdateMessageForm(t, CurrentForm)
					Else
						UpdateMessageForm(CurrentForm, t)
					End If
					
					'�_���[�W�̓K�p
					prev_hp = .HP
					If .IsConditionSatisfied("���G") Then
						tdmg = 0
					ElseIf .IsConditionSatisfied("�s���g") Then 
						.HP = MaxLng(.HP - dmg, 10)
						tdmg = prev_hp - .HP
					Else
						.HP = .HP - dmg
						tdmg = prev_hp - .HP
					End If
					
					'����\�́u�s����v�ɂ��\���`�F�b�N
					If .IsFeatureAvailable("�s����") Then
						If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("�\��") Then
							.AddCondition("�\��", -1)
							.Update()
						End If
					End If
					
					'�_���[�W���󂯂�Ζ��肩�炳�߂�
					If .IsConditionSatisfied("����") Then
						.DeleteCondition("����")
					End If
					
					If CurrentForm.Party = "����" Or CurrentForm.Party = "�m�o�b" Then
						UpdateMessageForm(t, CurrentForm)
					Else
						UpdateMessageForm(CurrentForm, t)
					End If
					If .HP > 0 Then
						DrawSysString(.x, .y, VB6.Format(tdmg))
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
					End If
					
					If .HP = 0 Then
						If .IsUnderSpecialPowerEffect("����") Then
							.HP = .MaxHP
							.RemoveSpecialPowerInEffect("�j��")
							DisplaySysMessage(.Nickname & "�͕��������I")
							GoTo NextLoop
						End If
						
						If .IsFeatureAvailable("�p�[�c����") Then
							uname = LIndex(.FeatureData("�p�[�c����"), 2)
							If .OtherForm(uname).IsAbleToEnter(.x, .y) Then
								.Transform(uname)
								With .CurrentForm
									.HP = .MaxHP
									.UsedAction = .MaxAction
								End With
								DisplaySysMessage(.Nickname & "�͔j�󂳂ꂽ�p�[�c�𕪗��������B")
								GoTo NextLoop
							End If
						End If
						
						.Die()
					End If
					
					If Not is_event Then
						u = SelectedUnit
						SelectedUnit = .CurrentForm
						SelectedTarget = Me
						If .Status_Renamed = "�j��" Then
							DisplaySysMessage(.Nickname & "�͔j�󂳂ꂽ")
							HandleEvent("�j��", .MainPilot.ID)
						Else
							DisplaySysMessage(.Nickname & "��" & tdmg & "�̃_���[�W���󂯂��B;" & "�c��g�o��" & VB6.Format(.HP) & "�i������ = " & 100 * (.MaxHP - .HP) \ .MaxHP & "���j")
							HandleEvent("������", .MainPilot.ID, 100 - .HP * 100 \ .MaxHP)
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
	
	
	' === �X�e�[�^�X�񕜊֘A���� ===
	
	'�X�e�[�^�X��S��
	Public Sub FullRecover()
		Dim i, j As Short
		
		'�p�C���b�g�̃X�e�[�^�X��S��
		For i = 1 To CountPilot
			Pilot(i).FullRecover()
		Next 
		For i = 1 To CountSupport
			Support(i).FullRecover()
		Next 
		If IsFeatureAvailable("�ǉ��p�C���b�g") Then
			If PList.IsDefined(FeatureData("�ǉ��p�C���b�g")) Then
				PList.Item(FeatureData("�ǉ��p�C���b�g")).FullRecover()
			End If
		End If
		
		With CurrentForm
			'�g�o����
			.HP = .MaxHP
			
			'�d�m�A�e������
			.FullSupply()
			
			'�X�e�[�^�X�ُ�݂̂�����
			i = 1
			Do While i <= .CountCondition
				If .Condition(i) = "�c�莞��" Or .Condition(i) = "�񑀍�" Or Right(.Condition(i), 2) = "�t��" Or Right(.Condition(i), 2) = "����" Or Right(.Condition(i), 3) = "�t���Q" Or Right(.Condition(i), 3) = "�����Q" Or Right(.Condition(i), 2) = "�t�o" Then
					i = i + 1
				Else
					.DeleteCondition(i)
				End If
			Loop 
			
			'�T�|�[�g�A�^�b�N���K�[�h�A��������U���A�J�E���^�[�U���񐔉�
			.UsedSupportAttack = 0
			.UsedSupportGuard = 0
			.UsedSyncAttack = 0
			.UsedCounterAttack = 0
			
			.Mode = "�ʏ�"
			
			'���`�Ԃ���
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
	
	'�d�m���e������
	Public Sub FullSupply()
		Dim i, j As Short
		
		'�d�m��
		EN = MaxEN
		
		'�e����
		For i = 1 To CountWeapon
			dblBullet(i) = 1
		Next 
		For i = 1 To CountAbility
			dblStock(i) = 1
		Next 
		
		'���`�Ԃ���
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
	
	'�e���݂̂���
	Public Sub BulletSupply()
		Dim i, j As Short
		
		For i = 1 To CountWeapon
			dblBullet(i) = 1
		Next 
		
		'���`�Ԃ���
		For i = 1 To CountOtherForm
			With OtherForm(i)
				For j = 1 To .CountWeapon
					.SetBullet(j, .MaxBullet(j))
				Next 
			End With
		Next 
	End Sub
	
	'�g�o�� percent ����
	Public Sub RecoverHP(ByVal percent As Double)
		HP = HP + MaxHP * percent / 100
		If HP <= 0 Then
			HP = 1
		End If
		
		'����\�́u�s����v�ɂ��\���`�F�b�N
		If IsFeatureAvailable("�s����") Then
			If HP <= MaxHP \ 4 And Not IsConditionSatisfied("�\��") Then
				AddCondition("�\��", -1)
			End If
		End If
	End Sub
	
	'�d�m�� percent ����
	Public Sub RecoverEN(ByVal percent As Double)
		EN = EN + MaxEN * percent / 100
		If EN <= 0 Then
			EN = 0
		End If
	End Sub
	
	'�^�[���o�߂ɂ��X�e�[�^�X��
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
		
		'�����X�e�[�W��1�^�[����(�X�^�[�g�C�x���g����)�͉񕜂��s��Ȃ�
		If Stage = "����" And Turn = 1 Then
			Exit Sub
		End If
		
		'�f�[�^�X�V
		Update()
		
		'�ό`�ɑΉ����Ď�����o�^
		u = Me
		
		With MainPilot
			'��͉�
			If .MaxPlana > 0 Then
				hp_ratio = 100 * HP / MaxHP
				en_ratio = 100 * EN / MaxEN
				
				.Plana = .Plana + .MaxPlana \ 16 + .MaxPlana * FeatureLevel("��͉�") \ 10 - .MaxPlana * FeatureLevel("��͏���") \ 10
				
				HP = MaxHP * hp_ratio \ 100
				EN = MaxEN * en_ratio \ 100
			End If
			
			'�r�o��
			If .IsSkillAvailable("�r�o��") Then
				.SP = .SP + .Level \ 8 + 5
			End If
			If .IsSkillAvailable("���_����") Then
				If .SP < .MaxSP \ 5 Then
					.SP = .SP + .MaxSP \ 10
				End If
			End If
		End With
		
		'�r�o��
		For i = 2 To CountPilot
			With Pilot(i)
				If .IsSkillAvailable("�r�o��") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("���_����") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		Next 
		For i = 1 To CountSupport
			With Support(i)
				If .IsSkillAvailable("�r�o��") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("���_����") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		Next 
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			With AdditionalSupport()
				If .IsSkillAvailable("�r�o��") Then
					.SP = .SP + .Level \ 8 + 5
				End If
				If .IsSkillAvailable("���_����") Then
					If .SP < .MaxSP \ 5 Then
						.SP = .SP + .MaxSP \ 10
					End If
				End If
			End With
		End If
		
		'�s����
		UsedAction = 0
		
		'�X�y�V�����p���[���ʂ�����
		RemoveSpecialPowerInEffect("�^�[��")
		
		'�X�y�V�����p���[��������
		With MainPilot
			For i = 1 To .CountSkill
				If .Skill(i) = "�X�y�V�����p���[��������" Then
					spname = LIndex(.SkillData(i), 2)
					If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) And Not IsSpecialPowerInEffect(spname) Then
						Center(x, y)
						.UseSpecialPower(spname, 0)
						If Status_Renamed = "���`��" Then
							Exit Sub
						End If
					End If
				End If
			Next 
			If IsConditionSatisfied("�X�y�V�����p���[���������t��") Or IsConditionSatisfied("�X�y�V�����p���[���������t���Q") Then
				spname = LIndex(.SkillData("�X�y�V�����p���[��������"), 2)
				If .Morale >= StrToLng(LIndex(.SkillData("�X�y�V�����p���[��������"), 3)) And Not IsSpecialPowerInEffect(spname) Then
					Center(x, y)
					.UseSpecialPower(spname, 0)
					If Status_Renamed = "���`��" Then
						Exit Sub
					End If
				End If
			End If
		End With
		For i = 2 To CountPilot
			With Pilot(i)
				For j = 1 To .CountSkill
					If .Skill(j) = "�X�y�V�����p���[��������" Then
						spname = LIndex(.SkillData(j), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(j), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "���`��" Then
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
					If .Skill(j) = "�X�y�V�����p���[��������" Then
						spname = LIndex(.SkillData(j), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(j), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "���`��" Then
								Exit Sub
							End If
						End If
					End If
				Next 
			End With
		Next 
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			With AdditionalSupport
				For i = 1 To .CountSkill
					If .Skill(i) = "�X�y�V�����p���[��������" Then
						spname = LIndex(.SkillData(i), 2)
						If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) And Not IsSpecialPowerInEffect(spname) Then
							Center(x, y)
							.UseSpecialPower(spname, 0)
							If Status_Renamed = "���`��" Then
								Exit Sub
							End If
						End If
					End If
				Next 
			End With
		End If
		
		'�N����
		With MainPilot
			If .IsSkillAvailable("�N����") And .SP <= .MaxSP \ 5 And HP <= MaxHP \ 5 And EN <= MaxEN \ 5 Then
				.SP = .MaxSP
				HP = MaxHP
				EN = MaxEN
				If SpecialPowerAnimation Then
					Center(x, y)
					If SPDList.IsDefined("�h����") Then
						SPDList.Item("�h����").PlayAnimation()
					End If
				End If
			End If
		End With
		
		'�g�o�Ƃd�m�񕜁�����
		' MOD START MARGE
		'    If Not IsConditionSatisfied("�񕜕s�\") Then
		If Not IsConditionSatisfied("�񕜕s�\") And Not IsSpecialPowerInEffect("�񕜕s�\") Then
			' MOD END MARGE
			If IsFeatureAvailable("�g�o��") Then
				hp_recovery = 10 * FeatureLevel("�g�o��")
			End If
			If IsFeatureAvailable("�d�m��") Then
				en_recovery = 10 * FeatureLevel("�d�m��")
			End If
		End If
		If IsFeatureAvailable("�g�o����") Then
			hp_recovery = hp_recovery - 10 * FeatureLevel("�g�o����")
		End If
		If IsFeatureAvailable("�d�m����") Then
			en_recovery = en_recovery - 10 * FeatureLevel("�d�m����")
		End If
		
		'�łɂ��g�o����
		Dim plv As Short
		If IsConditionSatisfied("��") Then
			
			If IsOptionDefined("�Ō��ʑ�") And BossRank < 0 Then
				plv = 25
			Else
				plv = 10
			End If
			
			If Weakness("��") Then
				plv = 2 * plv
			ElseIf Effective("��") Then 
				'�ω��Ȃ�
			ElseIf Immune("��") Or Absorb("��") Then 
				plv = 0
			ElseIf Resist("��") Then 
				plv = plv \ 2
			End If
			
			hp_recovery = hp_recovery - plv
		End If
		
		'�������E���Ԑ؂�H
		If ConditionLifetime("�������E") = 1 Then
			Center(x, y)
			Escape()
			OpenMessageForm()
			DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
			CloseMessageForm()
			HandleEvent("�j��", MainPilot.ID)
		End If
		
		'���̐鍐
		If ConditionLifetime("���̐鍐") = 1 Then
			hp_recovery = hp_recovery - 1000
		End If
		
		'�c�莞��
		If ConditionLifetime("�c�莞��") = 1 Then
			is_time_limit = True
			If IsFeatureAvailable("�m�[�}�����[�h") Then
				'�n�C�p�[���[�h���ϐg�̎��Ԑ؂�̏ꍇ�͖߂��̌`�Ԃ��L�^���Ă���
				next_form = LIndex(FeatureData("�m�[�}�����[�h"), 1)
			End If
		End If
		
		'�g�o�񕜂Ȃǂ�t�������ꍇ�̂��Ƃ��l���ď�Ԃ̃A�b�v�f�[�g��
		'�g�o���d�m�񕜗ʂ��v�Z������ɍs��
		hp_ratio = 100 * HP / MaxHP
		en_ratio = 100 * EN / MaxEN
		UpdateCondition(True)
		HP = MaxHP * hp_ratio \ 100
		EN = MaxEN * en_ratio \ 100
		
		'�T�|�[�g�A�^�b�N���K�[�h
		UsedSupportAttack = 0
		UsedSupportGuard = 0
		
		'��������U��
		UsedSyncAttack = 0
		
		'�J�E���^�[�U��
		UsedCounterAttack = 0
		
		'�`���[�W�����H
		If ConditionLifetime("�`���[�W") = 0 Then
			AddCondition("�`���[�W����", 1)
		End If
		
		'�t�����ꂽ�ړ��\�͂��؂ꂽ�ꍇ�̏���
		If Status_Renamed = "�o��" And MapFileName <> "" Then
			Select Case Area
				Case "��"
					If Not IsTransAvailable("��") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
							CloseMessageForm()
							HandleEvent("�j��", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "�n��"
					If Not IsTransAvailable("��") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
							CloseMessageForm()
							HandleEvent("�j��", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "����"
					If Not IsFeatureAvailable("����ړ�") And Not IsFeatureAvailable("�z�o�[�ړ�") Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
							CloseMessageForm()
							HandleEvent("�j��", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
				Case "����"
					If Adaption(3) = 0 Then
						If Not IsAbleToEnter(x, y) Then
							Center(x, y)
							Escape()
							OpenMessageForm()
							DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
							CloseMessageForm()
							HandleEvent("�j��", MainPilot.ID)
							Exit Sub
						End If
						'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						MapDataForUnit(x, y) = Nothing
						EraseUnitBitmap(x, y)
						StandBy(x, y)
					End If
			End Select
		End If
		
		If Status_Renamed = "�i�[" Then
			'�i�[���͉񕜗��t�o
			' MOD START MARGE
			'        If Not IsConditionSatisfied("�񕜕s�\") Then
			If Not IsConditionSatisfied("�񕜕s�\") And Not IsSpecialPowerInEffect("�񕜕s�\") Then
				' MOD END MARGE
				hp_recovery = hp_recovery + 50
				en_recovery = en_recovery + 50
			End If
			
			'�e����
			For i = 1 To CountWeapon
				dblBullet(i) = 1
			Next 
			For i = 1 To CountAbility
				dblStock(i) = 1
			Next 
		Else
			' MOD START MARGE
			'        '�i�[����ĂȂ��ꍇ�͒n�`�ɂ��񕜏C��
			'        If Not IsConditionSatisfied("�񕜕s�\") Then
			'            hp_recovery = hp_recovery + TerrainEffectForHPRecover(X, Y)
			'            en_recovery = en_recovery + TerrainEffectForENRecover(X, Y)
			'        End If
			'
			'        '�n�`�ɂ�錸���C������ԕt��
			'        Set td = TDList.Item(MapData(X, Y, 0))
			'        With td
			'            For i = 1 To .CountFeature
			'                Select Case .Feature(i)
			'                    Case "�g�o����"
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
			'                    Case "�d�m����"
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
			'                    Case "�g�o����"
			'                        If Not IsConditionSatisfied("�񕜕s�\") Then
			'                            hp_up = hp_up + 1000 * .FeatureLevel(i)
			'                        End If
			'
			'                    Case "�d�m����"
			'                        If Not IsConditionSatisfied("�񕜕s�\") Then
			'                            en_up = en_up + 10 * .FeatureLevel(i)
			'                        End If
			'
			'                    Case "�g�o�ቺ"
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
			'                    Case "�d�m�ቺ"
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
			'                    Case "��ԕt��"
			'                        cname = .FeatureData(i)
			'
			'                        '��Ԃ�����������邩�`�F�b�N
			'                        Select Case cname
			'                            Case "���b��"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "����"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "���|"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�x��"
			'                                If SpecialEffectImmune("�x") Then
			'                                    cname = ""
			'                                End If
			'                            Case "����m"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�]���r"
			'                                If SpecialEffectImmune("�]") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�񕜕s�\"
			'                                If SpecialEffectImmune("�Q") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�Ή�"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "����"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "���"
			'                                If SpecialEffectImmune("�") Then
			'                                    cname = ""
			'                                End If
			'                            Case "����"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "��"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�Ӗ�"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            Case "����"
			'                                If SpecialEffectImmune("��") Then
			'                                    cname = ""
			'                                End If
			'                            '�����g�p�s�\���
			'                            Case "�I�[���g�p�s�\"
			'                                If SpecialEffectImmune("���I") Then
			'                                    cname = ""
			'                                End If
			'                            Case "���\�͎g�p�s�\"
			'                                If SpecialEffectImmune("����") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�������g�p�s�\"
			'                                If SpecialEffectImmune("���V") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�����o�g�p�s�\"
			'                                If SpecialEffectImmune("���T") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�m�o�����g�p�s�\"
			'                                If SpecialEffectImmune("���T") Then
			'                                    cname = ""
			'                                End If
			'                            Case "��͎g�p�s�\"
			'                                If SpecialEffectImmune("����") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�p�g�p�s�\"
			'                                If SpecialEffectImmune("���p") Then
			'                                    cname = ""
			'                                End If
			'                            Case "�Z�g�p�s�\"
			'                                If SpecialEffectImmune("���Z") Then
			'                                    cname = ""
			'                                End If
			'                            Case Else
			'                                If Len(cname) > 6 Then
			'                                    If Right$(cname, 6) = "������_�t��" Then
			'                                        If SpecialEffectImmune("��" & Left$(cname, Len(cname) - 6)) _
			''                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
			''                                            Or Immune(Left$(cname, Len(cname) - 6)) _
			''                                        Then
			'                                            cname = ""
			'                                        End If
			'                                    ElseIf Right$(cname, 6) = "�����L���t��" Then
			'                                        If SpecialEffectImmune("�L" & Left$(cname, Len(cname) - 6)) _
			''                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
			''                                            Or Immune(Left$(cname, Len(cname) - 6)) _
			''                                        Then
			'                                            cname = ""
			'                                        End If
			'                                    ElseIf Right$(cname, 6) = "�����g�p�s�\" Then
			'                                        If SpecialEffectImmune("��" & Left$(cname, Len(cname) - 6)) Then
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
			'�i�[����ĂȂ��ꍇ�͒n�`�ɂ��e��C������ԕt��
			'MOD START 240a
			'        Set td = TDList.Item(MapData(X, Y, 0))
			'���C���[�̏�Ԃɉ����ď�w���w�ǂ�����擾���邩����
			Select Case MapData(x, y, Map.MapDataIndex.BoxType)
				Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.TerrainType))
				Case Else
					td = TDList.Item(MapData(x, y, Map.MapDataIndex.LayerType))
			End Select
			'MOD START 240a
			With td
				'�n�`���ʂ��K�p�����ʒu�ɂ��邩�𔻒�
				If .IsFeatureAvailable("���ʔ͈�") Then
					For i = 1 To LLength(.FeatureData("���ʔ͈�"))
						If Area = LIndex(.FeatureData("���ʔ͈�"), i) Then
							is_terrain_effective = True
							Exit For
						End If
					Next 
				Else
					is_terrain_effective = True
				End If
				
				'�n�`���ʂɑ΂��閳�����\�͂������Ă��邩
				If IsFeatureAvailable("�n�`���ʖ�����") Then
					If LLength(FeatureData("�n�`���ʖ�����")) > 1 Then
						For i = 2 To LLength(FeatureData("�n�`���ʖ�����"))
							If .Name = LIndex(FeatureData("�n�`���ʖ�����"), i) Then
								is_immune_to_terrain_effect = True
								Exit For
							End If
						Next 
					Else
						is_immune_to_terrain_effect = True
					End If
				ElseIf IsSpecialPowerInEffect("�n�`���ʖ�����") Then 
					is_immune_to_terrain_effect = True
				End If
				
				'�n�`���ʂ�K�p
				If is_terrain_effective Then
					For i = 1 To .CountFeature
						If Not IsConditionSatisfied("�񕜕s�\") And Not IsSpecialPowerInEffect("�񕜕s�\") Then
							Select Case .Feature(i)
								Case "�g�o��"
									hp_recovery = hp_recovery + 10 * .FeatureLevel(i)
								Case "�d�m��"
									en_recovery = en_recovery + 10 * .FeatureLevel(i)
								Case "�g�o����"
									hp_up = hp_up + 1000 * .FeatureLevel(i)
								Case "�d�m����"
									en_up = en_up + 10 * .FeatureLevel(i)
							End Select
						End If
						
						If Not is_immune_to_terrain_effect Then
							Select Case .Feature(i)
								Case "�g�o����"
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
									
								Case "�d�m����"
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
									
								Case "�g�o�ቺ"
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
									
								Case "�d�m�ቺ"
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
									
								Case "��ԕt��"
									cname = .FeatureData(i)
									
									'��Ԃ�����������邩�`�F�b�N
									Select Case cname
										Case "���b��"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "����"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "���|"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "�x��"
											If SpecialEffectImmune("�x") Then
												cname = ""
											End If
										Case "����m"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "�]���r"
											If SpecialEffectImmune("�]") Then
												cname = ""
											End If
										Case "�񕜕s�\"
											If SpecialEffectImmune("�Q") Then
												cname = ""
											End If
										Case "�Ή�"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "����"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "���"
											If SpecialEffectImmune("�") Then
												cname = ""
											End If
										Case "����"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "��"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "�Ӗ�"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
										Case "����"
											If SpecialEffectImmune("��") Then
												cname = ""
											End If
											'�����g�p�s�\���
										Case "�I�[���g�p�s�\"
											If SpecialEffectImmune("���I") Then
												cname = ""
											End If
										Case "���\�͎g�p�s�\"
											If SpecialEffectImmune("����") Then
												cname = ""
											End If
										Case "�������g�p�s�\"
											If SpecialEffectImmune("���V") Then
												cname = ""
											End If
										Case "�����o�g�p�s�\"
											If SpecialEffectImmune("���T") Then
												cname = ""
											End If
										Case "�m�o�����g�p�s�\"
											If SpecialEffectImmune("���T") Then
												cname = ""
											End If
										Case "��͎g�p�s�\"
											If SpecialEffectImmune("����") Then
												cname = ""
											End If
										Case "�p�g�p�s�\"
											If SpecialEffectImmune("���p") Then
												cname = ""
											End If
										Case "�Z�g�p�s�\"
											If SpecialEffectImmune("���Z") Then
												cname = ""
											End If
										Case Else
											If Len(cname) > 6 Then
												If Right(cname, 6) = "������_�t��" Then
													If SpecialEffectImmune("��" & Left(cname, Len(cname) - 6)) Or Absorb(Left(cname, Len(cname) - 6)) Or Immune(Left(cname, Len(cname) - 6)) Then
														cname = ""
													End If
												ElseIf Right(cname, 6) = "�����L���t��" Then 
													If SpecialEffectImmune("�L" & Left(cname, Len(cname) - 6)) Or Absorb(Left(cname, Len(cname) - 6)) Or Immune(Left(cname, Len(cname) - 6)) Then
														cname = ""
													End If
												ElseIf Right(cname, 6) = "�����g�p�s�\" Then 
													If SpecialEffectImmune("��" & Left(cname, Len(cname) - 6)) Then
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
		
		'�d�m�͖��^�[��5��
		' MOD START MARGE
		'    If Not IsConditionSatisfied("�񕜕s�\") _
		''        And Not IsOptionDefined("�d�m���R�񕜖���") _
		''    Then
		If Not IsConditionSatisfied("�񕜕s�\") And Not IsSpecialPowerInEffect("�񕜕s�\") And Not IsOptionDefined("�d�m���R�񕜖���") Then
			' MOD END MARGE
			EN = EN + 5
		End If
		
		'�Z�o�����񕜗����g���Ăg�o����
		HP = HP + MaxHP * hp_recovery \ 100 + hp_up
		If HP <= 0 Then
			HP = 1
		End If
		
		'����\�́u�s����v�ɂ��\���`�F�b�N
		If IsFeatureAvailable("�s����") Then
			If HP <= MaxHP \ 4 And Not IsConditionSatisfied("�\��") Then
				AddCondition("�\��", -1)
			End If
		End If
		
		'�Z�o�����񕜗����g���Ăd�m����
		If EN + MaxEN * en_recovery \ 100 + en_up > 0 Then
			EN = EN + MaxEN * en_recovery \ 100 + en_up
		Else
			'�d�m���������ĂO�ɂȂ�ꍇ�̓n�C�p�[���[�h�����������͍s���s�\
			If IsFeatureAvailable("�m�[�}�����[�h") Then
				'�������m�[�}�����[�h�ɖ߂�Ȃ��n�`���Ƃ��̂܂ܑދp�c�c
				If OtherForm(LIndex(FeatureData("�m�[�}�����[�h"), 1)).IsAbleToEnter(x, y) Then
					Transform(LIndex(FeatureData("�m�[�}�����[�h"), 1))
				Else
					Center(x, y)
					Escape()
					OpenMessageForm()
					DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
					CloseMessageForm()
					HandleEvent("�j��", MainPilot.ID)
					Exit Sub
				End If
			ElseIf IsFeatureAvailable("�ό`") Then 
				'�ό`�ł���Εό`
				buf = FeatureData("�ό`")
				For i = 2 To LLength(buf)
					With OtherForm(LIndex(buf, i))
						If .IsAbleToEnter(x, y) And Not .IsFeatureAvailable("�d�m����") Then
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
		
		'�f�[�^�X�V
		Update()
		
		'���Ԑ؂�H
		If is_time_limit Then
			If next_form <> "" Then
				'�n�C�p�[���[�h���ϐg�̎��Ԑ؂�
				u = OtherForm(next_form)
				If u.IsAbleToEnter(x, y) Then
					'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
					If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
						If Not PList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
							PList.Add(u.FeatureData("�ǉ��p�C���b�g"), MainPilot.Level, Party0)
						End If
					End If
					
					'�m�[�}�����[�h���b�Z�[�W
					If IsMessageDefined("�m�[�}�����[�h(" & Name & "=>" & u.Name & ")") Then
						OpenMessageForm()
						PilotMessage("�m�[�}�����[�h(" & Name & "=>" & u.Name & ")")
						CloseMessageForm()
					ElseIf IsMessageDefined("�m�[�}�����[�h(" & u.Name & ")") Then 
						OpenMessageForm()
						PilotMessage("�m�[�}�����[�h(" & u.Name & ")")
						CloseMessageForm()
					ElseIf IsMessageDefined("�m�[�}�����[�h") Then 
						OpenMessageForm()
						PilotMessage("�m�[�}�����[�h")
						CloseMessageForm()
					End If
					'�������
					If IsSpecialEffectDefined("�m�[�}�����[�h(" & Name & "=>" & u.Name & ")") Then
						SpecialEffect("�m�[�}�����[�h(" & Name & "=>" & u.Name & ")")
					ElseIf IsSpecialEffectDefined("�m�[�}�����[�h(" & u.Name & ")") Then 
						SpecialEffect("�m�[�}�����[�h(" & u.Name & ")")
					ElseIf IsSpecialEffectDefined("�m�[�}�����[�h") Then 
						SpecialEffect("�m�[�}�����[�h")
					End If
					
					'�ό`
					Transform(LIndex(FeatureData("�m�[�}�����[�h"), 1))
				Else
					'�ό`����Ƃ��̒n�`�ɂ���Ȃ��Ȃ�ꍇ�͑ދp
					Center(x, y)
					Escape()
					OpenMessageForm()
					DisplaySysMessage(Nickname & "�͋����I�ɑދp������ꂽ�B")
					CloseMessageForm()
					HandleEvent("�j��", MainPilot.ID)
					Exit Sub
				End If
			ElseIf IsFeatureAvailable("����") Then 
				'���̎��Ԑ؂�
				
				'���b�Z�[�W�\��
				If IsMessageDefined("����(" & Name & ")") Or IsMessageDefined("����(" & FeatureName("����") & ")") Or IsMessageDefined("����") Then
					If IsFeatureAvailable("�����a�f�l") Then
						StartBGM(FeatureData("�����a�f�l"))
						Sleep(500)
					ElseIf MainPilot.BGM <> "-" Then 
						StartBGM(MainPilot.BGM)
						Sleep(500)
					End If
					Center(x, y)
					RefreshScreen()
					
					OpenMessageForm()
					If IsMessageDefined("����(" & Name & ")") Then
						PilotMessage("����(" & Name & ")")
					ElseIf IsMessageDefined("����(" & FeatureName("����") & ")") Then 
						PilotMessage("����(" & FeatureName("����") & ")")
					Else
						PilotMessage("����")
					End If
					CloseMessageForm()
				End If
				'�������
				If IsSpecialEffectDefined("����(" & Name & ")") Then
					SpecialEffect("����(" & Name & ")")
				ElseIf IsSpecialEffectDefined("����(" & FeatureName("����") & ")") Then 
					SpecialEffect("����(" & FeatureName("����") & ")")
				Else
					SpecialEffect("����")
				End If
				
				'����
				Split_Renamed()
			Else
				'�������Ԑ؂�
				Center(x, y)
				RefreshScreen()
				OpenMessageForm()
				DisplaySysMessage(Nickname & "�͐������Ԑ؂�̂��ߑދp���܂��B")
				CloseMessageForm()
				Escape()
				HandleEvent("�j��", MainPilot.ID)
				Exit Sub
			End If
		End If
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎����������`�F�b�N
		CurrentForm.CheckAutoHyperMode()
		CurrentForm.CheckAutoNormalMode()
	End Sub
	
	'�n�C�p�[���[�h�̎��������`�F�b�N
	Public Sub CheckAutoHyperMode()
		Dim is_available, message_window_visible As Boolean
		Dim fname, fdata As String
		Dim flevel As Double
		Dim uname As String
		Dim i As Short
		
		'�n�C�p�[���[�h�������������邩����
		
		If Status_Renamed <> "�o��" Then
			Exit Sub
		End If
		
		If Not IsFeatureAvailable("�n�C�p�[���[�h") Then
			Exit Sub
		End If
		
		fname = FeatureName("�n�C�p�[���[�h")
		flevel = FeatureLevel("�n�C�p�[���[�h")
		fdata = FeatureData("�n�C�p�[���[�h")
		
		If InStr(fdata, "��������") = 0 Then
			Exit Sub
		End If
		
		'���������𖞂����H
		If MainPilot.Morale < CShort(10# * flevel) + 100 And (HP > MaxHP \ 4 Or InStr(fdata, "�C�͔���") > 0) Then
			Exit Sub
		End If
		
		'�ϐg���E�\�̓R�s�[���̓n�C�p�[���[�h���g�p�ł��Ȃ�
		If IsConditionSatisfied("�m�[�}�����[�h�t��") Then
			Exit Sub
		End If
		
		'�n�C�p�[���[�h��̌`�Ԃ����p�\�H
		uname = LIndex(fdata, 2)
		is_available = False
		With OtherForm(uname)
			Select Case TerrainClass(x, y)
				Case "��"
					If .IsTransAvailable("��") Then
						is_available = True
					End If
				Case "�[��"
					If .IsTransAvailable("��") Or .IsTransAvailable("��") Or .IsTransAvailable("����") Then
						is_available = True
					End If
				Case Else
					is_available = True
			End Select
			
			If Not .IsAbleToEnter(x, y) Then
				is_available = False
			End If
		End With
		
		'������������H
		If Not is_available Then
			Exit Sub
		End If
		
		'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
		If UDList.IsDefined(uname) Then
			With UDList.Item(uname)
				If IsFeatureAvailable("�ǉ��p�C���b�g") Then
					If Not PList.IsDefined(FeatureData("�ǉ��p�C���b�g")) Then
						PList.Add(FeatureData("�ǉ��p�C���b�g"), MainPilot.Level, Party0)
					End If
				End If
			End With
		End If
		
		'�a�f�l��؂�ւ�
		If IsFeatureAvailable("�n�C�p�[���[�h�a�f�l") Then
			For i = 1 To CountFeature
				If Feature(i) = "�n�C�p�[���[�h�a�f�l" And LIndex(FeatureData(i), 1) = uname Then
					StartBGM(Mid(FeatureData(i), InStr(FeatureData(i), " ") + 1))
					Sleep(500)
					Exit For
				End If
			Next 
		End If
		
		'���b�Z�[�W��\��
		If IsMessageDefined("�n�C�p�[���[�h(" & Name & "=>" & uname & ")") Or IsMessageDefined("�n�C�p�[���[�h(" & uname & ")") Or IsMessageDefined("�n�C�p�[���[�h(" & fname & ")") Or IsMessageDefined("�n�C�p�[���[�h") Then
			Center(x, y)
			RefreshScreen()
			
			If Not message_window_visible Then
				OpenMessageForm()
			Else
				message_window_visible = True
			End If
			
			'���b�Z�[�W��\��
			If IsMessageDefined("�n�C�p�[���[�h(" & Name & "=>" & uname & ")") Then
				PilotMessage("�n�C�p�[���[�h(" & Name & "=>" & uname & ")")
			ElseIf IsMessageDefined("�n�C�p�[���[�h(" & uname & ")") Then 
				PilotMessage("�n�C�p�[���[�h(" & uname & ")")
			ElseIf IsMessageDefined("�n�C�p�[���[�h(" & fname & ")") Then 
				PilotMessage("�n�C�p�[���[�h(" & fname & ")")
			Else
				PilotMessage("�n�C�p�[���[�h")
			End If
			
			If Not message_window_visible Then
				CloseMessageForm()
			End If
		End If
		
		'�������
		SaveSelections()
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		If IsAnimationDefined("�n�C�p�[���[�h(" & Name & "=>" & uname & ")") Then
			PlayAnimation("�n�C�p�[���[�h(" & Name & "=>" & uname & ")")
		ElseIf IsAnimationDefined("�n�C�p�[���[�h(" & uname & ")") Then 
			PlayAnimation("�n�C�p�[���[�h(" & uname & ")")
		ElseIf IsAnimationDefined("�n�C�p�[���[�h(" & FeatureName("�n�C�p�[���[�h") & ")") Then 
			PlayAnimation("�n�C�p�[���[�h(" & FeatureName("�n�C�p�[���[�h") & ")")
		ElseIf IsAnimationDefined("�n�C�p�[���[�h") Then 
			PlayAnimation("�n�C�p�[���[�h")
		ElseIf IsSpecialEffectDefined("�n�C�p�[���[�h(" & Name & "=>" & uname & ")") Then 
			SpecialEffect("�n�C�p�[���[�h(" & Name & "=>" & uname & ")")
		ElseIf IsSpecialEffectDefined("�n�C�p�[���[�h(" & uname & ")") Then 
			SpecialEffect("�n�C�p�[���[�h(" & uname & ")")
		ElseIf IsSpecialEffectDefined("�n�C�p�[���[�h(" & fname & ")") Then 
			SpecialEffect("�n�C�p�[���[�h(" & fname & ")")
		Else
			SpecialEffect("�n�C�p�[���[�h")
		End If
		RestoreSelections()
		
		'�n�C�p�[���[�h�ɕό`
		Transform(uname)
		
		'���j�b�g�ϐ���u������
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
		
		'�ό`�C�x���g
		With CurrentForm
			HandleEvent("�ό`", .MainPilot.ID, .Name)
		End With
	End Sub
	
	'�m�[�}�����[�h�̎��������`�F�b�N
	Public Function CheckAutoNormalMode(Optional ByVal without_redraw As Boolean = False) As Boolean
		Dim message_window_visible As Boolean
		Dim uname As String
		Dim i As Short
		
		'�m�[�}�����[�h�������������邩����
		
		If Status_Renamed <> "�o��" Then
			Exit Function
		End If
		
		If Not IsFeatureAvailable("�m�[�}�����[�h") Then
			Exit Function
		End If
		
		'�܂����̌`�Ԃł��n�j�H
		If IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'�m�[�}�����[�h�悪���p�\�H
		uname = LIndex(FeatureData("�m�[�}�����[�h"), 1)
		If Not OtherForm(uname).IsAbleToEnter(x, y) Then
			Exit Function
		End If
		
		'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
		If UDList.IsDefined(uname) Then
			With UDList.Item(uname)
				If IsFeatureAvailable("�ǉ��p�C���b�g") Then
					If Not PList.IsDefined(FeatureData("�ǉ��p�C���b�g")) Then
						PList.Add(FeatureData("�ǉ��p�C���b�g"), MainPilot.Level, Party0)
					End If
				End If
			End With
		End If
		
		'���b�Z�[�W��\��
		If IsMessageDefined("�m�[�}�����[�h(" & Name & "=>" & uname & ")") Or IsMessageDefined("�m�[�}�����[�h(" & uname & ")") Or IsMessageDefined("�m�[�}�����[�h") Then
			'�a�f�l��؂�ւ�
			If IsFeatureAvailable("�m�[�}�����[�h�a�f�l") Then
				For i = 1 To CountFeature
					If Feature(i) = "�m�[�}�����[�h�a�f�l" And LIndex(FeatureData(i), 1) = uname Then
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
			
			'���b�Z�[�W��\��
			If IsMessageDefined("�m�[�}�����[�h(" & Name & "=>" & uname & ")") Then
				PilotMessage("�m�[�}�����[�h(" & Name & "=>" & uname & ")")
			ElseIf IsMessageDefined("�m�[�}�����[�h(" & uname & ")") Then 
				PilotMessage("�m�[�}�����[�h(" & uname & ")")
			Else
				PilotMessage("�m�[�}�����[�h")
			End If
			
			If Not message_window_visible Then
				CloseMessageForm()
			End If
		End If
		
		'�������
		SaveSelections()
		SelectedUnit = Me
		SelectedUnitForEvent = Me
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		If IsAnimationDefined("�m�[�}�����[�h(" & Name & "=>" & uname & ")") Then
			PlayAnimation("�m�[�}�����[�h(" & Name & "=>" & uname & ")")
		ElseIf IsAnimationDefined("�m�[�}�����[�h(" & uname & ")") Then 
			PlayAnimation("�m�[�}�����[�h(" & uname & ")")
		ElseIf IsAnimationDefined("�m�[�}�����[�h") Then 
			PlayAnimation("�m�[�}�����[�h")
		ElseIf IsSpecialEffectDefined("�m�[�}�����[�h(" & Name & "=>" & uname & ")") Then 
			SpecialEffect("�m�[�}�����[�h(" & Name & "=>" & uname & ")")
		ElseIf IsSpecialEffectDefined("�m�[�}�����[�h(" & uname & ")") Then 
			SpecialEffect("�m�[�}�����[�h(" & uname & ")")
		Else
			SpecialEffect("�m�[�}�����[�h")
		End If
		RestoreSelections()
		
		'�m�[�}�����[�h�ɕό`
		Transform(uname)
		
		'���j�b�g�ϐ���u������
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
		
		'��ʂ̍ĕ`�悪�K�v�H
		If CurrentForm.IsConditionSatisfied("����") Then
			CheckAutoNormalMode = True
			If Not without_redraw Then
				RedrawScreen()
			End If
		End If
		
		'�ό`�C�x���g
		With CurrentForm
			HandleEvent("�ό`", .MainPilot.ID, .Name)
		End With
	End Function
	
	
	'�f�[�^�����Z�b�g
	'UPGRADE_NOTE: Reset �� Reset_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
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
		If IsFeatureAvailable("�ǉ��p�C���b�g") Then
			pname = FeatureData("�ǉ��p�C���b�g")
			If PList.IsDefined(pname) Then
				PList.Item(pname).FullRecover()
			End If
		End If
		If IsFeatureAvailable("�ǉ��T�|�[�g") Then
			pname = FeatureData("�ǉ��T�|�[�g")
			If PList.IsDefined(pname) Then
				PList.Item(pname).FullRecover()
			End If
		End If
		
		HP = MaxHP
		FullSupply()
		Mode = "�ʏ�"
	End Sub
	
	'���胆�j�b�g���G���ǂ����𔻒�
	Public Function IsEnemy(ByRef t As Unit, Optional ByVal for_move As Boolean = False) As Boolean
		Dim myparty, tparty As String
		
		'�������g�͏�ɖ���
		If t Is Me Then
			IsEnemy = False
			Exit Function
		End If
		
		'�\���������j�b�g�ɂƂ��Ă͂��ׂĂ��G
		If IsConditionSatisfied("�\��") Then
			IsEnemy = True
			Exit Function
		End If
		
		'���������ꍇ�̓����_���Ŕ���
		If IsConditionSatisfied("����") Then
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
			
			'�������j�b�g�͖\���A�߈ˁA�����������j�b�g��r���\
			'(�\�������������j�b�g��Party�͂m�o�b�Ƃ݂Ȃ����)
			If myparty = "����" And tparty = "�m�o�b" Then
				If .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("�߈�") Or .IsConditionSatisfied("����") Then
					IsEnemy = True
					Exit Function
				End If
			End If
			
			If myparty <> "����" Then
				'�^�[�Q�b�g�̐w�c�����肳��Ă���ꍇ�A�G�Ί֌W�ɂȂ��w�c��
				'���j�b�g�͖����ƌ��Ȃ����B
				'�������A�v���C���[���R���g���[�����郆�j�b�g�͂��̂悤�Ȏ�
				'�����U�����Ă��Ȃ����j�b�g���r���\�B
				
				'����̐w�c�݂̂�_���ꍇ
				Select Case Mode
					Case "����", "�m�o�b"
						Select Case tparty
							Case "����", "�m�o�b"
								IsEnemy = True
							Case Else
								IsEnemy = False
						End Select
						Exit Function
					Case "�G", "����"
						If tparty = Mode Then
							IsEnemy = True
						Else
							IsEnemy = False
						End If
						Exit Function
				End Select
				
				'���肪����̐w�c�݂̂�_���ꍇ
				Select Case .Mode
					Case "����", "�m�o�b"
						Select Case myparty
							Case "����", "�m�o�b"
								IsEnemy = True
							Case Else
								IsEnemy = False
						End Select
						Exit Function
					Case "�G", "����"
						If myparty = .Mode Then
							IsEnemy = True
						Else
							IsEnemy = False
						End If
						Exit Function
				End Select
			End If
			
			'�G�����𔻒�
			Select Case myparty
				Case "����", "�m�o�b"
					Select Case tparty
						Case "����", "�m�o�b"
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
	
	'���胆�j�b�g���������ǂ����𔻒�
	Public Function IsAlly(ByRef t As Unit) As Boolean
		'�������g�͏�ɖ���
		If t Is Me Then
			IsAlly = True
			Exit Function
		End If
		
		'�\���������j�b�g�ɂƂ��Ă͂��ׂĂ��G
		If IsConditionSatisfied("�\��") Then
			IsAlly = False
			Exit Function
		End If
		
		'���������ꍇ�̓����_���Ŕ���
		If IsConditionSatisfied("����") Then
			If Dice(2) = 1 Then
				IsAlly = True
			Else
				IsAlly = False
			End If
			Exit Function
		End If
		
		'�G�����𔻒�
		Select Case Party
			Case "����", "�m�o�b"
				If t.Party = "����" Or t.Party = "�m�o�b" Then
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
	
	
	' === ���j�b�g���ڊ֘A���� ===
	
	'���j�b�g�𓋍�
	Public Sub LoadUnit(ByRef u As Unit)
		colUnitOnBoard.Add(u, u.ID)
	End Sub
	
	'���ڂ������j�b�g���폜
	Public Sub UnloadUnit(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colUnitOnBoard.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colUnitOnBoard.Count()
			'UPGRADE_WARNING: �I�u�W�F�N�g colUnitOnBoard(i).Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If colUnitOnBoard.Item(i).Name = Index Then
				colUnitOnBoard.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'���ڂ������j�b�g�̑���
	Public Function CountUnitOnBoard() As Short
		CountUnitOnBoard = colUnitOnBoard.Count()
	End Function
	
	'���ڂ������j�b�g
	Public Function UnitOnBoard(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		UnitOnBoard = colUnitOnBoard.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colUnitOnBoard
			'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If u.Name = Index Then
				UnitOnBoard = u
				Exit Function
			End If
		Next u
	End Function
	
	
	' === �������j�b�g�֘A���� ===
	
	'�������j�b�g��ǉ�
	Public Sub AddServant(ByRef u As Unit)
		'���ɓo�^���Ă���H
		If Not Servant((u.ID)) Is Nothing Then
			Exit Sub
		End If
		
		colServant.Add(u, u.ID)
	End Sub
	
	'�������j�b�g���폜
	Public Sub DeleteServant(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colServant.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colServant.Count()
			'UPGRADE_WARNING: �I�u�W�F�N�g colServant(i).Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If colServant.Item(i).Name = Index Then
				colServant.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'�������j�b�g����
	Public Function CountServant() As Short
		CountServant = colServant.Count()
	End Function
	
	'�������j�b�g
	Public Function Servant(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		Servant = colServant.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colServant
			'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If u.Name = Index Then
				Servant = u
				Exit Function
			End If
		Next u
	End Function
	
	'�������j�b�g���������
	Public Sub DismissServant()
		Dim i, j As Short
		Dim uname As String
		
		For i = 1 To CountServant
			With Servant(1).CurrentForm
				Select Case .Status_Renamed
					Case "�o��", "�i�["
						.Escape()
						.Status_Renamed = "�j��"
					Case "����`��", "���`��"
						For j = 1 To .CountFeature
							If .Feature(j) = "����" Then
								uname = LIndex(.FeatureData(j), 2)
								If UList.IsDefined(uname) Then
									UList.Item(uname).CurrentForm.Split_Renamed()
								End If
							End If
						Next 
						With .CurrentForm
							If .Status_Renamed = "�o��" Or .Status_Renamed = "�i�[" Then
								.Escape()
								.Status_Renamed = "�j��"
							End If
						End With
				End Select
			End With
			DeleteServant(1)
		Next 
	End Sub
	
	
	' === �ꑮ���j�b�g�֘A���� ===
	
	'�ꑮ���j�b�g��ǉ�
	Public Sub AddSlave(ByRef u As Unit)
		colSlave.Add(u, u.ID)
	End Sub
	
	'�ꑮ���j�b�g���폜
	Public Sub DeleteSlave(ByRef Index As Object)
		Dim i As Short
		
		On Error GoTo ErrorHandler
		colSlave.Remove(Index)
		Exit Sub
		
ErrorHandler: 
		For i = 1 To colSlave.Count()
			'UPGRADE_WARNING: �I�u�W�F�N�g colSlave(i).Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If colSlave.Item(i).Name = Index Then
				colSlave.Remove(i)
				Exit Sub
			End If
		Next 
	End Sub
	
	'�ꑮ���j�b�g����
	Public Function CountSlave() As Short
		CountSlave = colSlave.Count()
	End Function
	
	'�ꑮ���j�b�g
	Public Function Slave(ByRef Index As Object) As Unit
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		Slave = colSlave.Item(Index)
		Exit Function
		
ErrorHandler: 
		For	Each u In colSlave
			'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If u.Name = Index Then
				Slave = u
				Exit Function
			End If
		Next u
	End Function
	
	'�ꑮ���j�b�g���������
	Public Sub DismissSlave()
		Dim i As Short
		
		For i = 1 To CountSlave
			With Slave(1).CurrentForm
				If .IsConditionSatisfied("����") And Not .Master Is Nothing Then
					If .Master.CurrentForm Is Me Then
						.DeleteCondition("����")
						'UPGRADE_NOTE: �I�u�W�F�N�g Slave().CurrentForm.Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						.Master = Nothing
					End If
				End If
				
				If .IsConditionSatisfied("�߈�") And Not .Master Is Nothing Then
					If .Master.CurrentForm Is Me Then
						.DeleteCondition("�߈�")
						'UPGRADE_NOTE: �I�u�W�F�N�g Slave().CurrentForm.Master ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						.Master = Nothing
					End If
				End If
			End With
			DeleteSlave(1)
		Next 
	End Sub
	
	' === �ꎞ���f�p�f�[�^�֘A���� ===
	
	'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
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
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
	Public Sub Restore()
		Dim sbuf As String
		Dim ibuf As Short
		Dim lbuf As Integer
		'UPGRADE_NOTE: ctype �� ctype_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
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
			
			'�t����������\�͂̃f�[�^�Ɂu"�v��u,�v���܂܂�Ă���ƃf�[�^�̏�����
			'�������s���Ȃ��̂Ŏ蓮�Ńp�[�V���O
			
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
				If InStr(cdata, " �p�C���b�g�\�͕t�� ") > 0 Then
					GoTo NextCondition
				End If
				If InStr(cdata, " �p�C���b�g�\�͋��� ") > 0 Then
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
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
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
	
	'�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
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
	
	
	' === �e�폈�����s�����߂̊֐����T�u���[�`�� ===
	
	'�o�����H
	Public Function IsOperational() As Boolean
		Dim i As Short
		
		If Status_Renamed = "�o��" Then
			IsOperational = True
			Exit Function
		End If
		
		For i = 1 To CountOtherForm
			If OtherForm(i).Status_Renamed = "�o��" Then
				IsOperational = True
				Exit Function
			End If
		Next 
		
		IsOperational = False
	End Function
	
	'���j�b�g�����j�b�g nm �Ɠ���H
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
	
	'���j�b�g�����݂Ƃ��Ă���`��
	Public Function CurrentForm() As Unit
		Dim i As Short
		
		If Status_Renamed = "���`��" Then
			For i = 1 To CountOtherForm
				If OtherForm(i).Status_Renamed <> "���`��" Then
					CurrentForm = OtherForm(i)
					Exit Function
				End If
			Next 
		End If
		
		CurrentForm = Me
	End Function
	
	'�l�ԃ��j�b�g���ǂ�������
	Public Function IsHero() As Boolean
		With Data
			If Left(.Class_Renamed, 1) = "(" Then
				IsHero = True
			Else
				IsHero = False
			End If
		End With
	End Function
	
	'(tx,ty)�̒n�_�̎��͂Ɂu�A�g�U���v���s���Ă���郆�j�b�g�����邩�ǂ����𔻒�
	Public Function LookForAttackHelp(ByVal tx As Short, ByVal ty As Short) As Unit
		Dim u As Unit
		Dim i As Short
		
		For i = 1 To 4
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
			
			'���j�b�g������H
			If u Is Nothing Then
				GoTo NextLoop
			End If
			
			'���j�b�g���G�łȂ��H
			If IsEnemy(u) Then
				GoTo NextLoop
			End If
			
			With u
				'�M���x�𖞂����Ă���H
				If Dice(10) > .MainPilot.Relation(MainPilot) Then
					GoTo NextLoop
				End If
				
				'�s���\�H
				If .MaxAction = 0 Then
					GoTo NextLoop
				End If
				
				'����Ȕ��f�͂�����H
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("�߈�") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Then
					GoTo NextLoop
				End If
				
				'���b�Z�[�W���o�^����Ă���H
				If Not IsMessageDefined("�A�g�U��(" & .MainPilot.Name & ")", True) And Not IsMessageDefined("�A�g�U��(" & .MainPilot.Nickname & ")", True) Then
					GoTo NextLoop
				End If
			End With
			
			'��������
			LookForAttackHelp = u
			Exit Function
NextLoop: 
		Next 
		
		'������Ȃ�����
		'UPGRADE_NOTE: �I�u�W�F�N�g LookForAttackHelp ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		LookForAttackHelp = Nothing
	End Function
	
	't����̍U���ɑ΂��āu���΂��v���s���Ă���郆�j�b�g�����邩�ǂ�������
	Public Function LookForGuardHelp(ByRef t As Unit, ByVal tw As Short, ByVal is_critical As Boolean) As Unit
		Dim u As Unit
		Dim i As Short
		Dim dmg As Integer
		Dim ratio As Double
		Dim ux, uy As Short
		Dim uarea As String
		
		For i = 1 To 4
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
			
			'���j�b�g������H
			If u Is Nothing Then
				GoTo NextLoop
			End If
			
			'���j�b�g���G�łȂ��H
			If IsEnemy(u) Then
				GoTo NextLoop
			End If
			
			With u
				'�M���x�𖞂����Ă���H
				If Dice(10) > .MainPilot.Relation(MainPilot) Then
					GoTo NextLoop
				End If
				
				'�s���\�H
				If .MaxAction = 0 Then
					GoTo NextLoop
				End If
				
				'����Ȕ��f�͂�����H
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("�߈�") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Then
					GoTo NextLoop
				End If
				
				'���b�Z�[�W���o�^����Ă���H
				If Not .IsMessageDefined("���΂�(" & MainPilot.Name & ")", True) And Not .IsMessageDefined("���΂�(" & MainPilot.Nickname & ")", True) Then
					GoTo NextLoop
				End If
				
				'���쑊��̃��j�b�g�̂���n�`�ɐi���\�H
				If Area <> .Area Then
					Select Case Area
						Case "��"
							If .Adaption(1) = 0 Then
								GoTo NextLoop
							End If
						Case "�n��"
							If .Adaption(2) = 0 Then
								GoTo NextLoop
							End If
						Case "����", "����"
							If .Adaption(3) = 0 Then
								GoTo NextLoop
							End If
						Case "�F��"
							If TerrainClass(x, y) = "����" Then
								If Not .IsTransAvailable("��") And Not .IsTransAvailable("�F��") Then
									GoTo NextLoop
								End If
							ElseIf .Adaption(4) = 0 Then 
								GoTo NextLoop
							End If
					End Select
				End If
				
				'�_���[�W���Z�o
				If .IsFeatureAvailable("�h��s��") Or t.IsWeaponClassifiedAs(tw, "�E") Then
					ratio = 1#
				Else
					ratio = 0.5
				End If
				If is_critical Then
					If IsOptionDefined("�_���[�W�{���ቺ") Then
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
				
				'�������|����Ă��܂��悤�ȏꍇ�͂��΂�Ȃ�
				If dmg >= .HP Then
					GoTo NextLoop
				End If
			End With
			
			'��������
			LookForGuardHelp = u
			Exit Function
NextLoop: 
		Next 
		
		'������Ȃ�����
		'UPGRADE_NOTE: �I�u�W�F�N�g LookForGuardHelp ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		LookForGuardHelp = Nothing
	End Function
	
	'�ő�T�|�[�g�A�^�b�N��
	Public Function MaxSupportAttack() As Short
		With MainPilot
			MaxSupportAttack = MaxLng(.SkillLevel("����U��"), .SkillLevel("����"))
		End With
	End Function
	
	'�ő�T�|�[�g�K�[�h��
	Public Function MaxSupportGuard() As Short
		With MainPilot
			MaxSupportGuard = MaxLng(.SkillLevel("����h��"), .SkillLevel("����"))
		End With
	End Function
	
	'�ő哯������U����
	Public Function MaxSyncAttack() As Short
		MaxSyncAttack = MainPilot.SkillLevel("����")
	End Function
	
	'�ő�J�E���^�[�U����
	Public Function MaxCounterAttack() As Short
		With MainPilot
			MaxCounterAttack = .SkillLevel("�J�E���^�[")
			
			If .IsSkillAvailable("���K��") Then
				If LLength(.SkillData("���K��")) = 2 Then
					If .Morale >= StrToLng(LIndex(.SkillData("���K��"), 2)) Then
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
	
	'���j�b�g t �ɑ΂��Ď��͂ɃT�|�[�g�A�^�b�N���s���Ă���郆�j�b�g�����邩�ǂ����𔻒�
	Public Function LookForSupportAttack(ByRef t As Unit) As Unit
		Dim u As Unit
		Dim i, w As Short
		Dim max_wpower As Integer
		Dim team, uteam As String
		
		'����Ȕ��f���\�H
		If IsConditionSatisfied("����") Then
			Exit Function
		End If
		
		'���m�����̏ꍇ�͂ǂ���ɉגS���ׂ���������Ȃ��̂Łc�c
		If Not t Is Nothing Then
			If Party = t.Party Then
				Exit Function
			End If
		End If
		
		team = MainPilot.SkillData("�`�[��")
		
		max_wpower = -1
		For i = 1 To 4
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
				'�T�|�[�g�A�^�b�N�����c���Ă���H
				If .MaxSupportAttack <= .UsedSupportAttack Then
					GoTo NextUnit
				End If
				
				'�s�������c���Ă���H
				If .Action = 0 Then
					GoTo NextUnit
				End If
				
				'����Ȕ��f���\�H
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Or .IsConditionSatisfied("�x��") Then
					GoTo NextUnit
				End If
				
				'�����H
				Select Case .Party
					Case "�m�o�b"
						Select Case Party
							Case "�G", "����"
								GoTo NextUnit
						End Select
					Case Else
						If .Party <> Party Then
							GoTo NextUnit
						End If
				End Select
				
				'�����`�[���ɑ����Ă���H
				uteam = .MainPilot.SkillData("�`�[��")
				If team <> uteam And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'�܂��^�[�Q�b�g�����肳��Ă��Ȃ��H
				If t Is Nothing Then
					LookForSupportAttack = u
					Exit Function
				End If
				
				'�U���\�H
				'�����З͂̕���̓��X�g�̍Ō�̕��ɂ��邱�Ƃ������̂Ō�납�画��
				w = .CountWeapon
				Do While w > 0
					'�U���͂����܂Ō�����������ȉ��̏ꍇ�͑I�l�O
					If .WeaponPower(w, (t.Area)) <= max_wpower Then
						GoTo NextWeapon
					End If
					
					'�T�|�[�g�A�^�b�N�ɗ��p�\�H
					If .IsWeaponClassifiedAs(w, "�l") Then
						GoTo NextWeapon
					End If
					If .IsWeaponClassifiedAs(w, "��") Then
						GoTo NextWeapon
					End If
					If Not .IsWeaponAvailable(w, "�ړ��O") Then
						GoTo NextWeapon
					End If
					If Not .IsTargetWithinRange(w, t) Then
						GoTo NextWeapon
					End If
					
					If .Party = "����" And .Party0 = "����" Then
						'�������j�b�g�͎����U�����T�|�[�g�A�^�b�N�ɂ͎g�p���Ȃ�
						If .IsWeaponClassifiedAs(w, "��") Then
							GoTo NextWeapon
						End If
						
						'���������̏ꍇ�A�������j�b�g�͎c�e�������Ȃ�������g�p���Ȃ�
						'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
							If Not .IsWeaponClassifiedAs(w, "�i") Then
								If .Bullet(w) = 1 Or .MaxBullet(w) = 2 Or .MaxBullet(w) = 3 Then
									GoTo NextWeapon
								End If
							End If
							If .WeaponENConsumption(w) > 0 Then
								If .WeaponENConsumption(w) >= .EN \ 2 Or .WeaponENConsumption(w) >= .MaxEN \ 4 Then
									GoTo NextWeapon
								End If
							End If
							If .IsWeaponClassifiedAs(w, "�s") Then
								GoTo NextWeapon
							End If
						End If
					End If
					
					'����U���p�̕��킪��������
					max_wpower = .WeaponPower(w, (t.Area))
					LookForSupportAttack = u
					
NextWeapon: 
					w = w - 1
				Loop 
			End With
NextUnit: 
		Next 
	End Function
	
	'���j�b�g t ����̍U���ɑ΂��Ď��͂ɃT�|�[�g�K�[�h���s���Ă���郆�j�b�g��
	'���邩�ǂ����𔻒�
	Public Function LookForSupportGuard(ByRef t As Unit, ByVal tw As Short) As Unit
		Dim u As Unit
		Dim i As Short
		Dim my_dmg, dmg As Integer
		Dim ratio As Double
		Dim ux, uy As Short
		Dim uarea As String
		Dim team, uteam As String
		
		'�}�b�v�U���̓T�|�[�g�K�[�h�s�\
		If t.IsWeaponClassifiedAs(tw, "�l") Then
			Exit Function
		End If
		
		'�X�y�V�����p���[�ŃT�|�[�g�K�[�h������������Ă���H
		If t.IsUnderSpecialPowerEffect("�T�|�[�g�K�[�h������") Then
			Exit Function
		End If
		
		'���m�����̏ꍇ�͖{���̐w�c�ɑ����郆�j�b�g�݂̂����
		If Party = t.Party Or (Party = "�m�o�b" And t.Party = "����") Then
			If Party <> Party0 Then
				Exit Function
			End If
			If IsConditionSatisfied("�\��") Then
				Exit Function
			End If
		End If
		
		'�������󂯂�_���[�W�����߂Ă���
		my_dmg = t.ExpDamage(tw, Me, True)
		
		'���΂��K�v���Ȃ��H
		'�蓮�����Ŗ����̏ꍇ�̓_���[�W�ɂ�����炸��ɂ��΂�
		'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		If Party <> "����" Or MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
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
		
		team = MainPilot.SkillData("�`�[��")
		
		For i = 1 To 4
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
				'�T�|�[�g�K�[�h�����c���Ă���H
				If .MaxSupportGuard <= .UsedSupportGuard Then
					GoTo NextUnit
				End If
				
				'�s���\�H
				If .MaxAction = 0 Then
					GoTo NextUnit
				End If
				
				'�X�y�V�����p���[�ŃT�|�[�g�K�[�h�����󂳂�Ă���H
				If .IsUnderSpecialPowerEffect("�T�|�[�g�K�[�h�s�\") Then
					GoTo NextUnit
				End If
				
				'����Ȕ��f���\�H
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Then
					GoTo NextUnit
				End If
				
				'�g�o�������ق���D��
				If Not LookForSupportGuard Is Nothing Then
					If LookForSupportGuard.HP >= .HP Then
						GoTo NextUnit
					End If
				End If
				
				'�����H
				Select Case .Party
					Case "����"
						If IsOptionDefined("�΂m�o�b�T�|�[�g�K�[�h����") Then
							If .Party <> Party Then
								GoTo NextUnit
							End If
						Else
							Select Case Party
								Case "�G", "����"
									GoTo NextUnit
							End Select
						End If
					Case "�m�o�b"
						Select Case Party
							Case "�G", "����"
								GoTo NextUnit
						End Select
					Case Else
						If .Party <> Party Then
							GoTo NextUnit
						End If
				End Select
				
				'�����`�[���ɑ����Ă���H
				uteam = .MainPilot.SkillData("�`�[��")
				If team <> uteam And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'���쑊��̃��j�b�g�̂���n�`�ɐi���\�H
				If Area <> .Area Then
					Select Case Area
						Case "��"
							If Not .IsTransAvailable("��") Then
								GoTo NextUnit
							End If
						Case "�n��"
							If .Adaption(2) = 0 Then
								GoTo NextUnit
							End If
						Case "����", "����"
							If .Adaption(3) = 0 Then
								GoTo NextUnit
							End If
						Case "�F��"
							If .Adaption(4) = 0 Then
								GoTo NextUnit
							End If
					End Select
				End If
				
				'�@�B�����΂��̂͋@�B�̂�
				If MainPilot.Personality = "�@�B" Then
					If .MainPilot.Personality <> "�@�B" Then
						GoTo NextUnit
					End If
				End If
				
				'�_���[�W���Z�o
				If .IsFeatureAvailable("�h��s��") Or t.IsWeaponClassifiedAs(tw, "�E") Then
					ratio = 1#
				Else
					ratio = 0.5
				End If
				If t.IsNormalWeapon(tw) Then
					'�_���[�W�͏�ɍň��̏󋵂��l���ăN���e�B�J�����̒l��
					If IsOptionDefined("�_���[�W�{���ቺ") Then
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
				
				'�{�X�̓U�R�����E���ɂ���I
				If .BossRank > BossRank Then
					'���_���[�W�����Ȃ��ꍇ�͕ʂ����ǁc�c
					If dmg >= .MaxHP \ 20 Or dmg >= .HP \ 5 Then
						GoTo NextUnit
					End If
				End If
				
				'�������|����Ă��܂��悤�ȏꍇ�͂��΂�Ȃ�(�N���e�B�J�����܂�)
				If dmg >= .HP Then
					'�{�X�͗�O�c�c
					If .BossRank >= BossRank Then
						GoTo NextUnit
					End If
				End If
			End With
			
			LookForSupportGuard = u
NextUnit: 
		Next 
	End Function
	
	'(tx,ty)�̒n�_�̎��͂ɃT�|�[�g���s���Ă���郆�j�b�g�����邩�ǂ����𔻒�B
	Public Function LookForSupport(ByVal tx As Short, ByVal ty As Short, Optional ByVal for_attack As Boolean = False) As Short
		Dim u As Unit
		Dim i As Short
		Dim do_support As Boolean
		Dim team, uteam As String
		
		With MainPilot
			'�������g���T�|�[�g���s�����Ƃ��o���邩�H
			If .IsSkillAvailable("����") Or .IsSkillAvailable("����U��") Or .IsSkillAvailable("����h��") Or .IsSkillAvailable("�w��") Or .IsSkillAvailable("�L��T�|�[�g") Then
				do_support = True
			End If
			
			team = .SkillData("�`�[��")
		End With
		
		For i = 1 To 4
			'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
				'����Ȕ��f���\�H
				If .IsConditionSatisfied("����") Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Then
					GoTo NextUnit
				End If
				
				'�����H
				If IsEnemy(u) Or .IsEnemy(Me) Then
					GoTo NextUnit
				End If
				
				'�����`�[���ɑ����Ă���H
				uteam = .MainPilot.SkillData("�`�[��")
				If team <> uteam And team <> "" And uteam <> "" Then
					GoTo NextUnit
				End If
				
				'�ړ��݂̂̏ꍇ�A���肪���ꂩ��ړ����Ă��܂��Ă͈Ӗ����Ȃ�
				If Not for_attack Then
					If .Action > 0 Then
						GoTo NextUnit
					End If
				End If
				
				'�������g���T�|�[�g�\�ł���΁A���肪�N�ł����ɗ���
				If do_support Then
					LookForSupport = LookForSupport + 1
				End If
				
				'�T�|�[�g�\�͂������Ă���H
				With .MainPilot
					If .IsSkillAvailable("����") Or .IsSkillAvailable("����U��") Then
						LookForSupport = LookForSupport + 1
						'���ꂩ��U������ꍇ�A���肪�s���o����΃T�|�[�g�A�^�b�N���\
						If for_attack Then
							If u.Action > 0 Then
								LookForSupport = LookForSupport + 1
								'��������U�����\�ł���΂���Ƀ{�[�i�X
								If MainPilot.IsSkillAvailable("����") Then
									LookForSupport = LookForSupport + 1
								End If
							End If
						End If
					ElseIf .IsSkillAvailable("����h��") Or .IsSkillAvailable("�w��") Or .IsSkillAvailable("�L��T�|�[�g") Then 
						LookForSupport = LookForSupport + 1
					End If
				End With
			End With
NextUnit: 
		Next 
	End Function
	
	'���̋Z�̃p�[�g�i�[��T��
	'UPGRADE_NOTE: ctype �� ctype_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Sub CombinationPartner(ByRef ctype_Renamed As String, ByVal w As Short, ByRef partners() As Unit, Optional ByVal tx As Short = 0, Optional ByVal ty As Short = 0, Optional ByVal check_formation As Boolean = False)
		Dim u As Unit
		Dim uname As String
		Dim j, i, k As Short
		Dim clevel, cnum As Short
		Dim clist As String
		Dim cname As String
		Dim cmorale, cen, cplana As Short
		Dim crange, loop_limit As Short
		
		'����Ȕ��f���\�H
		If IsConditionSatisfied("����") Then
			ReDim partners(0)
			Exit Sub
		End If
		
		'���̋Z�̃f�[�^�𒲂ׂĂ���
		If ctype_Renamed = "����" Then
			cname = Weapon(w).Name
			cen = WeaponENConsumption(w)
			cmorale = Weapon(w).NecessaryMorale
			If IsWeaponClassifiedAs(w, "��") Then
				cplana = 5 * WeaponLevel(w, "��")
			ElseIf IsWeaponClassifiedAs(w, "�v") Then 
				cplana = 5 * WeaponLevel(w, "�v")
			End If
			crange = WeaponMaxRange(w)
		Else
			cname = Ability(w).Name
			cen = AbilityENConsumption(w)
			cmorale = Ability(w).NecessaryMorale
			If IsAbilityClassifiedAs(w, "��") Then
				cplana = 5 * AbilityLevel(w, "��")
			ElseIf IsAbilityClassifiedAs(w, "�v") Then 
				cplana = 5 * AbilityLevel(w, "�v")
			End If
			crange = AbilityMaxRange(w)
		End If
		
		'���j�b�g�̓���\�́u���̋Z�v�̌���
		For i = 1 To CountFeature
			If Feature(i) = "���̋Z" Then
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
		
		'�o�����Ă��Ȃ��ꍇ
		If Status_Renamed <> "�o��" Or MapFileName = "" Then
			'�p�[�g�i�[�����Ԃɂ��邾���ł悢
			For i = 1 To cnum
				uname = LIndex(clist, i)
				
				'�p�[�g�i�[�����j�b�g���Ŏw�肳��Ă���ꍇ
				If UList.IsDefined(uname) Then
					With UList.Item(uname)
						If .Status_Renamed = "�o��" Or .Status_Renamed = "�ҋ@" Then
							GoTo NextPartner
						End If
					End With
				End If
				
				'�p�[�g�i�[���p�C���b�g���Ŏw�肳��Ă���ꍇ
				If PList.IsDefined(uname) Then
					If Not PList.Item(uname).Unit_Renamed Is Nothing Then
						With PList.Item(uname).Unit_Renamed
							If .Status_Renamed = "�o��" Or .Status_Renamed = "�ҋ@" Then
								GoTo NextPartner
							End If
						End With
					End If
				End If
				
				'�p�[�g�i�[��������Ȃ�����
				ReDim partners(0)
				Exit Sub
NextPartner: 
			Next 
			'�p�[�g�i�[���S�����Ԃɂ���
			ReDim partners(cnum)
			Exit Sub
		End If
		
		'���̋Z�̊�_�̐ݒ�
		If tx = 0 Then
			tx = x
		End If
		If ty = 0 Then
			ty = y
		End If
		
		'�p�[�g�i�[�̌����͈͂�ݒ�
		
		If crange = 1 Then
			If cnum >= 8 Then
				'�˒��P�łW�̍��̈ȏ�̏ꍇ�͂Q�}�X�ȓ�
				loop_limit = 12
			ElseIf cnum >= 4 Then 
				'�˒��P�łS�̍��̈ȏ�̏ꍇ�͎΂ߗאډ�
				loop_limit = 8
			Else
				'�ǂ�ɂ��Y�����Ă��Ȃ���Ηאڂ̂�
				loop_limit = 4
			End If
		Else
			If cnum >= 9 Then
				'�˒��Q�ȏ�łX�̍��̈ȏ�̏ꍇ�͂Q�}�X�ȓ�
				loop_limit = 12
			ElseIf cnum >= 5 Then 
				'�˒��Q�ȏ�łT�̍��̈ȏ�̏ꍇ�͎΂ߗאډ�
				loop_limit = 8
			Else
				'�ǂ�ɂ��Y�����Ă��Ȃ���Ηאڂ̂�
				loop_limit = 4
			End If
		End If
		
		'���̋Z�΂ߗאډI�v�V����
		If IsOptionDefined("���̋Z�΂ߗאډ�") Then
			If loop_limit = 4 Then
				loop_limit = 8
			End If
		End If
		
		ReDim partners(0)
		For i = 1 To cnum
			'�p�[�g�i�[�̖���
			uname = LIndex(clist, i)
			
			For j = 1 To loop_limit
				'�p�[�g�i�[�̌����ʒu�ݒ�
				'UPGRADE_NOTE: �I�u�W�F�N�g u ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
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
				
				'���j�b�g�����݂���H
				If u Is Nothing Then
					GoTo NextNeighbor
				End If
				
				With u
					'���̋Z�̃p�[�g�i�[�ɊY������H
					If .Name <> uname Then
						'�p�C���b�g���ł��m�F
						If .MainPilot.Name <> uname Then
							GoTo NextNeighbor
						End If
					End If
					
					'���j�b�g�������H
					If u Is Me Then
						GoTo NextNeighbor
					End If
					
					'���ɑI���ς݁H
					For k = 1 To UBound(partners)
						If u Is partners(k) Then
							GoTo NextNeighbor
						End If
					Next 
					
					'���j�b�g���G�H
					If IsEnemy(u) Then
						GoTo NextNeighbor
					End If
					
					'�s���o���Ȃ���΂���
					If .MaxAction = 0 Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("�߈�") Then
						GoTo NextNeighbor
					End If
					
					'���̋Z�Ƀ��x�����ݒ肳��Ă���΃p�C���b�g�Ԃ̐M���x���`�F�b�N
					If clevel > 0 Then
						If MainPilot.Relation(.MainPilot) < clevel Or .MainPilot.Relation(MainPilot) < clevel Then
							GoTo NextNeighbor
						End If
					End If
					
					'�p�[�g�i�[��������g�����߂̏����𖞂����Ă��邩�𔻒�
					If Not check_formation Then
						If ctype_Renamed = "����" Then
							'���̋Z�Ɠ����̕��������
							For k = 1 To .CountWeapon
								If .Weapon(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountWeapon Then
								'���킪�g����H
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
								If .WeaponLevel(k, "��") > 0 Then
									If .MainPilot.Plana < 5 * .WeaponLevel(k, "��") Then
										GoTo NextNeighbor
									End If
								ElseIf .WeaponLevel(k, "�v") > 0 Then 
									If .MainPilot.Plana < 5 * .WeaponLevel(k, "�v") Then
										GoTo NextNeighbor
									End If
								End If
							Else
								'�����̕���������Ă��Ȃ������ꍇ�̓`�F�b�N���ڂ�����
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
							'���̋Z�Ɠ����̃A�r���e�B������
							For k = 1 To .CountAbility
								If .Ability(k).Name = cname Then
									Exit For
								End If
							Next 
							
							If k <= .CountAbility Then
								'�A�r���e�B���g����H
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
								If .AbilityLevel(k, "��") > 0 Then
									If .MainPilot.Plana < 5 * .AbilityLevel(k, "��") Then
										GoTo NextNeighbor
									End If
								ElseIf .AbilityLevel(k, "�v") > 0 Then 
									If .MainPilot.Plana < 5 * .AbilityLevel(k, "�v") Then
										GoTo NextNeighbor
									End If
								End If
							Else
								'�����̃A�r���e�B�������Ă��Ȃ������ꍇ�̓`�F�b�N���ڂ�����
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
						'�t�H�[���[�V�����̃`�F�b�N�����̎����K�v�Z�\�͒��ׂĂ���
						If ctype_Renamed = "����" Then
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
				
				'���������p�[�g�i�[���L�^
				ReDim Preserve partners(i)
				partners(i) = u
				Exit For
NextNeighbor: 
			Next 
			
			'�p�[�g�i�[��������Ȃ������H
			If j > loop_limit Then
				ReDim partners(0)
				Exit Sub
			End If
		Next 
		
		'���̋Z���b�Z�[�W����p�Ƀp�[�g�i�[�ꗗ���L�^
		ReDim SelectedPartners(UBound(partners))
		For i = 1 To UBound(partners)
			SelectedPartners(i) = partners(i)
		Next 
	End Sub
	
	'���̋Z�U���ɕK�v�ȃp�[�g�i�[�������邩�H
	Public Function IsCombinationAttackAvailable(ByVal w As Short, Optional ByVal check_formation As Boolean = False) As Boolean
		Dim partners() As Unit
		
		ReDim partners(0)
		If Status_Renamed = "�ҋ@" Or MapFileName = "" Then
			'�o�����ȊO�͑��肪���Ԃɂ��邾���łn�j
			CombinationPartner("����", w, partners, x, y)
		ElseIf WeaponMaxRange(w) = 1 And Not IsWeaponClassifiedAs(w, "�l") Then 
			'�˒��P�̏ꍇ�͎����̎���̂����ꂩ�̓G���j�b�g�ɑ΂��č��̋Z���g����΂n�j
			If x > 1 Then
				If Not MapDataForUnit(x - 1, y) Is Nothing Then
					If IsEnemy(MapDataForUnit(x - 1, y)) Then
						CombinationPartner("����", w, partners, x - 1, y, check_formation)
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If x < MapWidth Then
					If Not MapDataForUnit(x + 1, y) Is Nothing Then
						If IsEnemy(MapDataForUnit(x + 1, y)) Then
							CombinationPartner("����", w, partners, x + 1, y, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > 1 Then
					If Not MapDataForUnit(x, y - 1) Is Nothing Then
						If IsEnemy(MapDataForUnit(x, y - 1)) Then
							CombinationPartner("����", w, partners, x, y - 1, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y < MapHeight Then
					If Not MapDataForUnit(x, y + 1) Is Nothing Then
						If IsEnemy(MapDataForUnit(x, y + 1)) Then
							CombinationPartner("����", w, partners, x, y + 1, check_formation)
						End If
					End If
				End If
			End If
		Else
			'�˒��Q�ȏ�̏ꍇ�͎����̎���Ƀp�[�g�i�[������΂n�j
			CombinationPartner("����", w, partners, x, y, check_formation)
		End If
		
		'�����𖞂����p�[�g�i�[�̑g����������������
		If UBound(partners) > 0 Then
			IsCombinationAttackAvailable = True
		Else
			IsCombinationAttackAvailable = False
		End If
	End Function
	
	'���̋Z�A�r���e�B�ɕK�v�ȃp�[�g�i�[�������邩�H
	Public Function IsCombinationAbilityAvailable(ByVal a As Short, Optional ByVal check_formation As Boolean = False) As Boolean
		Dim partners() As Unit
		
		ReDim partners(0)
		If Status_Renamed = "�ҋ@" Or MapFileName = "" Then
			'�o�����ȊO�͑��肪���Ԃɂ��邾���łn�j
			CombinationPartner("�A�r���e�B", a, partners, x, y)
		ElseIf AbilityMaxRange(a) = 1 And Not IsAbilityClassifiedAs(a, "�l") Then 
			'�˒��P�̏ꍇ�͎����̎���̂����ꂩ�̖������j�b�g�ɑ΂��č��̋Z���g����΂n�j
			If x > 1 Then
				If Not MapDataForUnit(x - 1, y) Is Nothing Then
					If IsAlly(MapDataForUnit(x - 1, y)) Then
						CombinationPartner("�A�r���e�B", a, partners, x - 1, y, check_formation)
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If x < MapWidth Then
					If Not MapDataForUnit(x + 1, y) Is Nothing Then
						If IsAlly(MapDataForUnit(x + 1, y)) Then
							CombinationPartner("�A�r���e�B", a, partners, x + 1, y, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > 1 Then
					If Not MapDataForUnit(x, y - 1) Is Nothing Then
						If IsAlly(MapDataForUnit(x, y - 1)) Then
							CombinationPartner("�A�r���e�B", a, partners, x, y - 1, check_formation)
						End If
					End If
				End If
			End If
			If UBound(partners) = 0 Then
				If y > MapHeight Then
					If Not MapDataForUnit(x, y + 1) Is Nothing Then
						If IsAlly(MapDataForUnit(x, y + 1)) Then
							CombinationPartner("�A�r���e�B", a, partners, x, y + 1, check_formation)
						End If
					End If
				End If
			End If
		Else
			'�˒��Q�ȏ�̏ꍇ�͎����̎���Ƀp�[�g�i�[������΂n�j
			CombinationPartner("�A�r���e�B", a, partners, x, y, check_formation)
		End If
		
		'�����𖞂����p�[�g�i�[�̑g����������������
		If UBound(partners) > 0 Then
			IsCombinationAbilityAvailable = True
		Else
			IsCombinationAbilityAvailable = False
		End If
	End Function
	
	'(tx,ty)�Ƀ��j�b�g���i���\���H
	Public Function IsAbleToEnter(ByVal tx As Short, ByVal ty As Short) As Boolean
		Dim ignore_move_cost As Boolean
		
		'�g�p�s�\�̌`�Ԃ͂ǂ̒n�`�ɑ΂��Ă��i���s�\�Ƃ݂Ȃ�
		If Not IsAvailable() Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		'�P�ɕK�v�Z�\���`�F�b�N���Ă���ꍇ�H
		If MapFileName = "" Then
			IsAbleToEnter = True
			Exit Function
		End If
		
		'�}�b�v�O�H
		If tx < 1 Or MapWidth < tx Or ty < 1 Or MapHeight < ty Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		'�n�`�K���`�F�b�N
		Select Case TerrainClass(tx, ty)
			Case "��"
				If Not IsTransAvailable("��") And Not CurrentForm.IsFeatureAvailable("�󒆈ړ�") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "��"
				If IsTransAvailable("��") Or CurrentForm.IsFeatureAvailable("�󒆈ړ�") Or IsTransAvailable("����") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Adaption(3) = 0 And Not CurrentForm.IsFeatureAvailable("�����ړ�") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "�[��"
				If IsTransAvailable("��") Or CurrentForm.IsFeatureAvailable("�󒆈ړ�") Or IsTransAvailable("����") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Not IsTransAvailable("��") And Not CurrentForm.IsFeatureAvailable("�����ړ�") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "�F��"
				If Adaption(4) = 0 And Not CurrentForm.IsFeatureAvailable("�F���ړ�") Then
					IsAbleToEnter = False
					Exit Function
				End If
			Case "����"
				If IsTransAvailable("��") Or CurrentForm.IsFeatureAvailable("�󒆈ړ�") Or IsTransAvailable("�F") Or CurrentForm.IsFeatureAvailable("�F���ړ�") Then
					IsAbleToEnter = True
					Exit Function
				End If
			Case Else
				If IsTransAvailable("��") Or CurrentForm.IsFeatureAvailable("�󒆈ړ�") Then
					IsAbleToEnter = True
					Exit Function
				End If
				If Not IsTransAvailable("��") And Not CurrentForm.IsFeatureAvailable("����ړ�") Then
					IsAbleToEnter = False
					Exit Function
				End If
		End Select
		
		'�i���s�\�H
		If TerrainMoveCost(tx, ty) >= 1000 Then
			IsAbleToEnter = False
			Exit Function
		End If
		
		IsAbleToEnter = True
	End Function
	
	'���̌`�Ԃ��g�p�\���H (Disable���K�v�Z�\�̃`�F�b�N)
	Public Function IsAvailable() As Boolean
		Dim i As Short
		
		IsAvailable = True
		
		'�C�x���g�R�}���h�uDisable�v
		If IsDisabled(Name) Then
			IsAvailable = False
			Exit Function
		End If
		
		'�������Ԃ̐؂ꂽ�`�ԁH
		If Status_Renamed = "���`��" Then
			If IsConditionSatisfied("�s���s�\") Then
				IsAvailable = False
				Exit Function
			End If
		End If
		
		With CurrentForm
			'�Z�\�`�F�b�N���K�v�H
			If .CountPilot = 0 Or (Not IsFeatureAvailable("�K�v�Z�\") And Not IsFeatureAvailable("�s�K�v�Z�\")) Then
				Exit Function
			End If
			
			'�K�v�Z�\���`�F�b�N
			For i = 1 To CountFeature
				Select Case Feature(i)
					Case "�K�v�Z�\"
						If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
							IsAvailable = False
							Exit Function
						End If
					Case "�s�K�v�Z�\"
						If .IsNecessarySkillSatisfied(FeatureData(i)) Then
							IsAvailable = False
							Exit Function
						End If
				End Select
			Next 
		End With
	End Function
	
	'�K�v�Z�\�𖞂����Ă��邩�H
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
		
		'�X�̕K�v�������`�F�b�N
		i = 1
		Do While i <= MinLng(num, 100)
			If IsNecessarySkillSatisfied2(nskill_list(i), p) Then
				'�K�v�������������ꂽ�ꍇ�A���̌�́uor�v���X�L�b�v
				If i <= num - 2 Then
					Do While LCase(nskill_list(i + 1)) = "or"
						i = i + 2
						'��������K�v�����������Ȃ����̂ŕK�v�Z�\���������ꂽ�Ɣ���
						If i = num Then
							IsNecessarySkillSatisfied = True
							Exit Function
						ElseIf i > num Then 
							'or�̌��ɕK�v�������Ȃ�
							ErrorMessage(Name & "�ɑ΂���K�v�Z�\�u" & nabilities & "�v���s���ł�")
							TerminateSRC()
						End If
					Loop 
				End If
			Else
				'�K�v��������������Ȃ������ꍇ�A���̌�Ɂuor�v���Ȃ����
				'�K�v�Z�\����������Ȃ������Ɣ���
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
		
		'�X�e�[�^�X�R�}���h���s���͏�������������Ă���ƌ��Ȃ��H
		If Left(ndata, 1) = "+" Then
			If Status_Renamed = "�o��" And InStatusCommand() Then
				IsNecessarySkillSatisfied2 = True
				Exit Function
			End If
			ndata = Mid(ndata, 2)
		End If
		
		'�����ҋZ�\���Q�ƁH
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
		
		'�s�K�v�Z�\�H
		If Left(sname, 1) = "!" Then
			IsNecessarySkillSatisfied2 = Not IsNecessarySkillSatisfied2(Mid(ndata, 2), p)
			Exit Function
		End If
		
		'�K�v�Z�\�̔���Ɏg�p����p�C���b�g��ݒ�
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
		
		'�_�~�[�p�C���b�g�̏ꍇ�͖���
		If Not mp Is Nothing Then
			If mp.Nickname0 = "�p�C���b�g�s��" Then
				'UPGRADE_NOTE: �I�u�W�F�N�g mp ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				mp = Nothing
			End If
		End If
		
		slevel = -10000
		
		'�܂����̂��ς��Ȃ��K�v�Z�\�𔻒�
		Select Case sname
			Case "���x��"
				If Not mp Is Nothing Then
					slevel = mp.Level
				Else
					slevel = 0
				End If
			Case "�i��"
				If Not mp Is Nothing Then
					slevel = mp.InfightBase
				Else
					slevel = 0
				End If
			Case "�ˌ�"
				slevel = 0
				If Not mp Is Nothing Then
					If Not mp.HasMana() Then
						slevel = mp.ShootingBase
					End If
				End If
			Case "����"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.HasMana() Then
						slevel = mp.ShootingBase
					End If
				End If
			Case "����"
				If Not mp Is Nothing Then
					slevel = mp.HitBase
				Else
					slevel = 0
				End If
			Case "���"
				If Not mp Is Nothing Then
					slevel = mp.DodgeBase
				Else
					slevel = 0
				End If
			Case "�Z��"
				If Not mp Is Nothing Then
					slevel = mp.TechniqueBase
				Else
					slevel = 0
				End If
			Case "����"
				If Not mp Is Nothing Then
					slevel = mp.IntuitionBase
				Else
					slevel = 0
				End If
			Case "�i�������l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If IsOptionDefined("�U���͒ᐬ��") Then
							slevel = .InfightBase - .Level \ 2
						Else
							slevel = .InfightBase - .Level
						End If
					End With
				End If
			Case "�ˌ������l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If Not .HasMana() Then
							If IsOptionDefined("�U���͒ᐬ��") Then
								slevel = .ShootingBase - .Level \ 2
							Else
								slevel = .ShootingBase - .Level
							End If
						End If
					End With
				End If
			Case "���͏����l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						If .HasMana() Then
							If IsOptionDefined("�U���͒ᐬ��") Then
								slevel = .ShootingBase - .Level \ 2
							Else
								slevel = .ShootingBase - .Level
							End If
						End If
					End With
				End If
			Case "���������l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .HitBase - .Level
					End With
				End If
			Case "��������l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .DodgeBase - .Level
					End With
				End If
			Case "�Z�ʏ����l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .TechniqueBase - .Level
					End With
				End If
			Case "���������l"
				slevel = 0
				If Not mp Is Nothing Then
					With mp
						slevel = .IntuitionBase - .Level
					End With
				End If
			Case "�j��"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.Sex = "�j��" Then
						slevel = 1
					End If
					If Data.PilotNum > 1 Then
						For i = 1 To CountPilot
							If Pilot(i).Sex = "�j��" Then
								slevel = 1
							End If
						Next 
					End If
					For i = 1 To CountSupport
						If Support(i).Sex = "�j��" Then
							slevel = 1
						End If
					Next 
					If IsFeatureAvailable("�ǉ��T�|�[�g") Then
						If AdditionalSupport.Sex = "�j��" Then
							slevel = 1
						End If
					End If
				End If
			Case "����"
				slevel = 0
				If Not mp Is Nothing Then
					If mp.Sex = "����" Then
						slevel = 1
					End If
					If Data.PilotNum > 1 Then
						For i = 1 To CountPilot
							If Pilot(i).Sex = "����" Then
								slevel = 1
							End If
						Next 
					End If
					For i = 1 To CountSupport
						If Support(i).Sex = "����" Then
							slevel = 1
						End If
					Next 
					If IsFeatureAvailable("�ǉ��T�|�[�g") Then
						If AdditionalSupport.Sex = "����" Then
							slevel = 1
						End If
					End If
				End If
			Case "���g"
				If IsHero Then
					slevel = 1
				Else
					slevel = 0
				End If
			Case "�m��"
				If HP <= MaxHP \ 4 Then
					slevel = 1
				Else
					slevel = 0
				End If
			Case "�g�o"
				slevel = 10# * HP / MaxHP
			Case "�d�m"
				slevel = 10# * EN / MaxEN
			Case "�C��"
				If Not mp Is Nothing Then
					slevel = CDbl(mp.Morale - 100#)
					slevel = CDbl(slevel / 10#)
				Else
					slevel = 0
				End If
			Case "�����N"
				slevel = Rank
			Case "�n��", "��", "����", "����", "�F��", "�n��"
				slevel = 0
				If Status_Renamed = "�o��" Then
					If sname = Area Then
						slevel = 1
					End If
				End If
			Case "�A�C�e��"
				'�g���̂ăA�C�e���\�L�p
				slevel = 1
			Case "���Đg�Z", "��������"
				'�A�r���e�B�ŕt�����ꂽ���Đg�Z�y�ю���������p�̕��킪�\�������̂�
				'�h�����߁A�����̕K�v�Z�\�͏�ɖ�������Ȃ��Ƃ݂Ȃ�
				Exit Function
		End Select
		
		'��̏����̂����ꂩ�ɊY���H
		If slevel <> -10000 Then
			'�w�肳�ꂽ�Z�\�̃��x�����K�v�ȃ��x���ȏ�̏ꍇ�ɕK�v�Z�\���������ꂽ�Ɣ���
			If slevel >= nlevel Then
				IsNecessarySkillSatisfied2 = True
			End If
			Exit Function
		End If
		
		'�K�v�Z�\�̎�ނ𔻕�
		If Not mp Is Nothing Then
			stype = mp.SkillType(sname)
		Else
			stype = sname
		End If
		
		'���̂��ς��\��������K�v�Z�\�𔻒�
		Dim iname As String
		Dim uname As String
		Dim u As Unit
		Dim max_range As Short
		Select Case stype
			Case "�����o"
				If Not p Is Nothing Then
					slevel = p.SkillLevel("�����o")
					If stype <> sname Then
						If p.SkillNameForNS(stype) <> sname Then
							slevel = 0
						End If
					End If
					slevel = slevel + p.SkillLevel("�m�o����")
				ElseIf Not mp Is Nothing Then 
					slevel = mp.SkillLevel("�����o")
					If Data.PilotNum > 1 Then
						For i = 2 To CountPilot
							With Pilot(i)
								slevel = MaxDbl(slevel, .SkillLevel("�����o"))
								slevel = MaxDbl(slevel, .SkillLevel(sname))
							End With
						Next 
					End If
					For i = 1 To CountSupport
						With Support(i)
							slevel = MaxDbl(slevel, .SkillLevel("�����o"))
							slevel = MaxDbl(slevel, .SkillLevel(sname))
						End With
					Next 
					If IsFeatureAvailable("�ǉ��T�|�[�g") Then
						With AdditionalSupport
							slevel = MaxDbl(slevel, .SkillLevel("�����o"))
							slevel = MaxDbl(slevel, .SkillLevel(sname))
						End With
					End If
					
					If stype <> sname Then
						If mp.SkillNameForNS(stype) <> sname Then
							slevel = 0
						End If
					End If
					
					slevel = slevel + mp.SkillLevel("�m�o����")
					If Data.PilotNum > 1 Then
						For i = 2 To CountPilot
							slevel = MaxDbl(slevel, Pilot(i).SkillLevel("�m�o����"))
						Next 
					End If
					For i = 1 To CountSupport
						slevel = MaxDbl(slevel, Support(i).SkillLevel("�m�o����"))
					Next 
					If IsFeatureAvailable("�ǉ��T�|�[�g") Then
						slevel = MaxDbl(slevel, AdditionalSupport.SkillLevel("�m�o����"))
					End If
				End If
			Case "������"
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
					If IsFeatureAvailable("�ǉ��T�|�[�g") Then
						slevel = MaxDbl(slevel, AdditionalSupport.SynchroRate)
					End If
				End If
				If stype <> sname Then
					If mp.SkillNameForNS(stype) <> sname Then
						slevel = 0
					End If
				End If
			Case "�I�[��"
				If Not p Is Nothing Then
					slevel = p.SkillLevel("�I�[��")
				ElseIf Not mp Is Nothing Then 
					slevel = AuraLevel()
				End If
				If stype <> sname Then
					If mp.SkillNameForNS(stype) <> sname Then
						slevel = 0
					End If
				End If
			Case "���"
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
				'��L�ȊO�̃p�C���b�g�p����\��
				
				If Not mp Is Nothing Then
					With mp
						'����p�C���b�g��p�H
						If sname = .Name Or sname = .Nickname Then
							slevel = 1
						ElseIf stype = sname Then 
							slevel = .SkillLevel(stype)
						ElseIf .SkillNameForNS(stype) = sname Then 
							slevel = .SkillLevel(stype)
						End If
					End With
					
					'�p�C���b�g�������ʂ��łȂ��ꍇ�̂�
					If Data.PilotNum > 1 Then
						'�T�u�p�C���b�g�̋Z�\������
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
					
					'�T�|�[�g�p�C���b�g�̋Z�\������
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
					
					'�ǉ��T�|�[�g�̋Z�\������
					If IsFeatureAvailable("�ǉ��T�|�[�g") And CountPilot > 0 Then
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
					'���j�b�g���܂��̓N���X�ɊY���H
					If sname = Name Or sname = Nickname0 Or sname = Class0 Then
						slevel = 1
					End If
				End If
				
				If slevel = 0 Then
					If Left(sname, 1) = "@" Then
						'�n�`���w�肵���K�v�Z�\
						If Status_Renamed = "�o��" And 1 <= x And x <= MapWidth And 1 <= y And y <= MapHeight Then
							If Mid(sname, 2) = TerrainName(x, y) Then
								slevel = 1
							End If
						End If
					ElseIf Right(sname, 2) = "����" Then 
						'�A�C�e�����w�肵���K�v�Z�\
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
					ElseIf Right(sname, 2) = "�א�" Or Right(sname, 4) = "�}�X�ȓ�" Then 
						'����̃��j�b�g���߂��ɂ��邱�Ƃ��w�肵���K�v�Z�\
						If Status_Renamed = "�o��" Then
							If Right(sname, 2) = "�א�" Then
								uname = Left(sname, Len(sname) - 2)
								max_range = 1
							Else
								uname = Left(sname, Len(sname) - 5)
								max_range = StrToLng(Mid(sname, Len(sname) - 4, 1))
							End If
							
							For i = MaxLng(x - max_range, 1) To MinLng(x + max_range, MapWidth)
								For j = MaxLng(y - max_range, 1) To MinLng(y + max_range, MapHeight)
									u = MapDataForUnit(i, j)
									
									'�������͈͊O�H
									If System.Math.Abs(x - i) + System.Math.Abs(y - j) > max_range Then
										GoTo NextNeighbor
									End If
									
									'���j�b�g�����Ȃ��H
									If u Is Nothing Then
										GoTo NextNeighbor
									End If
									
									'���j�b�g�������H
									If u Is Me Or (x = i And y = j) Then
										GoTo NextNeighbor
									End If
									
									'���j�b�g���G�H
									If IsEnemy(u) Then
										GoTo NextNeighbor
									End If
									
									With u
										'���̋Z�̃p�[�g�i�[�ɊY�����邩
										If uname = "���" Then
											If Not .IsFeatureAvailable("���") Then
												GoTo NextNeighbor
											End If
										Else
											If .Name <> uname And .MainPilot.Name <> uname Then
												GoTo NextNeighbor
											End If
										End If
										
										'�s���o���Ȃ���΂���
										If .MaxAction = 0 Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("�߈�") Then
											GoTo NextNeighbor
										End If
									End With
									
									'�p�[�g�i�[����������
									IsNecessarySkillSatisfied2 = True
									Exit Function
NextNeighbor: 
								Next 
							Next 
						End If
					ElseIf Right(sname, 2) = "���" Then 
						'�����Ԃ��w�肵���K�v�Z�\
						If IsConditionSatisfied(Left(sname, Len(sname) - 2)) Then
							slevel = 1
						End If
					End If
				End If
		End Select
		
		'�w�肳�ꂽ�Z�\�̃��x�����K�v�ȃ��x���ȏ�̏ꍇ�ɕK�v�Z�\���������ꂽ�Ɣ���
		If slevel >= nlevel Then
			IsNecessarySkillSatisfied2 = True
		End If
	End Function
	
	'�\�� fname �𕕈󂳂�Ă��邩�H
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
	
	
	'���݁A�������U�����󂯂Ă��鑤���ǂ�������
	Public Function IsDefense() As Boolean
		If Party = Stage Then
			IsDefense = False
		Else
			IsDefense = True
		End If
	End Function
End Class