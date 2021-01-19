Option Strict Off
Option Explicit On
Friend Class Item
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�쐬���ꂽ�A�C�e���̃N���X
	
	'���ʎq
	Public ID As String
	'�A�C�e���f�[�^�ւ̃|�C���^
	Public Data As ItemData
	'�A�C�e���𑕔����Ă��郆�j�b�g
	'UPGRADE_NOTE: Unit �� Unit_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Unit_Renamed As Unit
	'�A�C�e�������݂��Ă��邩�H (RemoveItem����Ă��Ȃ����H)
	Public Exist As Boolean
	'�A�C�e�������͂𔭊��ł��Ă��邩�H (�K�v�Z�\�═��N���X���h��N���X�𖞂����Ă��邩�H)
	Public Activated As Boolean
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		Exist = True
		Activated = True
		'UPGRADE_NOTE: �I�u�W�F�N�g Data ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Data = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Unit_Renamed = Nothing
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		'UPGRADE_NOTE: �I�u�W�F�N�g Data ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Data = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g Unit_Renamed ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Unit_Renamed = Nothing
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
			Data = IDList.Item(Value)
		End Set
	End Property
	
	'����
	Public Function Nickname() As String
		Dim u As Unit
		
		Nickname = Data.Nickname
		
		'���̓��̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(Nickname)
		SelectedUnitForEvent = u
	End Function
	
	'�ǂ݉���
	Public Function KanaName() As String
		Dim u As Unit
		
		KanaName = Data.KanaName
		
		'�ǂ݉������̎��u���̂��߁A�f�t�H���g���j�b�g���ꎞ�I�ɕύX����
		u = SelectedUnitForEvent
		SelectedUnitForEvent = Unit_Renamed
		ReplaceSubExpression(KanaName)
		SelectedUnitForEvent = u
	End Function
	
	'�N���X
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Function Class_Renamed() As String
		Class_Renamed = Data.Class_Renamed
	End Function
	
	Public Function Class0() As String
		Dim i, n As Short
		
		Class0 = Data.Class_Renamed
		
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
	End Function
	
	'������
	Public Function Part() As String
		Part = Data.Part
	End Function
	
	'�g�o�C���l
	Public Function HP() As Integer
		HP = Data.HP
	End Function
	
	'�d�m�C���l
	Public Function EN() As Short
		EN = Data.EN
	End Function
	
	'���b�C���l
	Public Function Armor() As Integer
		Armor = Data.Armor
	End Function
	
	'�^�����C���l
	Public Function Mobility() As Short
		Mobility = Data.Mobility
	End Function
	
	'�ړ��͏C���l
	Public Function Speed() As Short
		Speed = Data.Speed
	End Function
	
	'����\�͑���
	Public Function CountFeature() As Short
		CountFeature = Data.CountFeature
	End Function
	
	'����\��
	Public Function Feature(ByRef Index As Object) As String
		Feature = Data.Feature(Index)
	End Function
	
	'����\�̖͂���
	Public Function FeatureName(ByRef Index As Object) As String
		FeatureName = Data.FeatureName(Index)
	End Function
	
	'����\�͂̃��x��
	Public Function FeatureLevel(ByRef Index As Object) As Double
		FeatureLevel = Data.FeatureLevel(Index)
	End Function
	
	'����\�͂̃f�[�^
	Public Function FeatureData(ByRef Index As Object) As String
		FeatureData = Data.FeatureData(Index)
	End Function
	
	'����\�͂̕K�v�Z�\
	Public Function FeatureNecessarySkill(ByRef Index As Object) As String
		FeatureNecessarySkill = Data.FeatureNecessarySkill(Index)
	End Function
	
	'�w�肵������\�͂������Ă��邩�H
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		IsFeatureAvailable = Data.IsFeatureAvailable(fname)
	End Function
	
	'����f�[�^
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = Data.Weapon(Index)
	End Function
	
	'����̑���
	Public Function CountWeapon() As Short
		CountWeapon = Data.CountWeapon
	End Function
	
	'�A�r���e�B�f�[�^
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = Data.Ability(Index)
	End Function
	
	'�A�r���e�B�̑���
	Public Function CountAbility() As Short
		CountAbility = Data.CountAbility
	End Function
	
	'�T�C�Y(�A�C�e���������A�C�e���X���b�g��)
	Public Function Size() As Short
		Size = Data.Size
	End Function
	
	
	'�A�C�e�����g�p�\���H
	Public Function IsAvailable(ByRef u As Unit) As Boolean
		Dim j, i, k As Short
		Dim iclass As String
		Dim sname, fdata As String
		
		IsAvailable = False
		
		'�C�x���g�R�}���h�uDisable�v
		If IsGlobalVariableDefined("Disable(" & Name & ")") Then
			Exit Function
		End If
		
		'�������ɓK�����Ă��邩
		Select Case Part
			Case "�Ў�", "����", "��"
				If InStr(u.FeatureData("������"), "�r") = 0 Then
					Exit Function
				End If
			Case "��", "����"
				If InStr(u.FeatureData("������"), "��") = 0 Then
					Exit Function
				End If
			Case "��"
				If InStr(u.FeatureData("������"), "��") = 0 Then
					Exit Function
				End If
			Case "��"
				If InStr(u.FeatureData("������"), "��") = 0 Then
					Exit Function
				End If
		End Select
		
		'����N���X or �h��N���X�ɑ����Ă��邩�H
		Select Case Part
			Case "����", "�Ў�", "����"
				iclass = u.WeaponProficiency() & " �Œ� �ėp"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case "��", "��", "��"
				iclass = u.ArmorProficiency() & " �Œ� �ėp"
				For i = 1 To LLength(iclass)
					If Class0 = LIndex(iclass, i) Then
						IsAvailable = True
						Exit For
					End If
				Next 
			Case Else
				'���̑��̃A�C�e���͏�ɗ��p�\
				IsAvailable = True
		End Select
		
		If Not IsAvailable Then
			Exit Function
		End If
		
		'�Z�\�`�F�b�N���K�v�H
		If Not IsFeatureAvailable("�K�v�Z�\") And Not IsFeatureAvailable("�s�K�v�Z�\") Then
			Exit Function
		End If
		
		With u
			'�K�v�Z�\���`�F�b�N
			For i = 1 To CountFeature
				Select Case Feature(i)
					Case "�K�v�Z�\"
						If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'�A�C�e�����g�ɂ��K�v�Z�\�Ɏw�肳�ꂽ�\�͂����󂳂�Ă����ꍇ��
							'�K�v�Z�\�𖞂����Ă���Ɣ��肳���邽�߁A�`�F�b�N����K�v������B
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'���ɑ������Ă���̂łȂ���Α������Ȃ�
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'�K�v�Z�\���u�`�����v�H
							If Right(sname, 2) = "����" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'���󂷂�\�͂��K�v�Z�\�ɂȂ��Ă���H
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "�p�C���b�g�\�͕t��", "�p�C���b�g�\�͋���"
									Case Else
										GoTo NextLoop1
								End Select
								
								'���󂷂�\�͖�
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'�K�v�Z�\�ƕ��󂷂�\�͂���v���Ă���H
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
							
							'�K�v�Z�\����������Ă��Ȃ�����
							IsAvailable = False
							Exit Function
						End If
					Case "�s�K�v�Z�\"
						If .IsNecessarySkillSatisfied(FeatureData(i)) Then
							'�A�C�e�����g�ɂ��s�K�v�Z�\����������Ă���ꍇ�͕s�K�v�Z�\��
							'���������邽�߁A�`�F�b�N����K�v������B
							
							For j = 1 To .CountItem
								If Me Is .Item(j) Then
									Exit For
								End If
							Next 
							If j > .CountItem Then
								'���ɑ������Ă���̂łȂ���Α������Ȃ�
								IsAvailable = False
								Exit Function
							End If
							
							If .CountPilot > 0 Then
								sname = .MainPilot.SkillType(FeatureData(i))
							Else
								sname = FeatureData(i)
							End If
							
							'�s�K�v�Z�\���u�`�����v�H
							If Right(sname, 2) = "����" Then
								If Left(sname, Len(sname) - 2) = Name Or Left(sname, Len(sname) - 2) = Class0 Then
									GoTo NextLoop
								End If
							End If
							
							'�t������\�͂��s�K�v�Z�\�ɂȂ��Ă���H
							For j = 1 To CountFeature
								Select Case Feature(j)
									Case "�p�C���b�g�\�͕t��", "�p�C���b�g�\�͋���"
									Case Else
										GoTo NextLoop2
								End Select
								
								'�t������\�͖�
								fdata = FeatureData(j)
								If Left(fdata, 1) = """" Then
									fdata = Mid(fdata, 2, Len(fdata) - 2)
								End If
								If InStr(fdata, "=") > 0 Then
									fdata = Left(fdata, InStr(fdata, "=") - 1)
								End If
								
								'�K�v�Z�\�ƕt������\�͂���v���Ă���H
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
							
							'�s�K�v�Z�\����������Ă���
							IsAvailable = False
							Exit Function
						End If
				End Select
NextLoop: 
			Next 
		End With
	End Function
	
	
	'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
	Public Sub Dump()
		WriteLine(SaveDataFileNumber, Name, ID, Exist)
		If Unit_Renamed Is Nothing Then
			WriteLine(SaveDataFileNumber, "-")
		Else
			WriteLine(SaveDataFileNumber, Unit_Renamed.ID)
		End If
	End Sub
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
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
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
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
	
	''�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
	Public Sub RestoreParameter()
		Dim sbuf As String
		
		'Name, ID, Exist
		sbuf = LineInput(SaveDataFileNumber)
		
		'Unit
		sbuf = LineInput(SaveDataFileNumber)
	End Sub
End Class