Option Strict Off
Option Explicit On
Friend Class UnitData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'���j�b�g�f�[�^�̃N���X
	
	'����
	Public Name As String
	'���ʎq
	Public ID As Integer
	'�N���X
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Class_Renamed As String
	'�p�C���b�g�� (�}�C�i�X�̏ꍇ�͊��ʂ��̎w��)
	Public PilotNum As Short
	'�A�C�e����
	Public ItemNum As Short
	'�n�`�K��
	Public Adaption As String
	'�g�o
	Public HP As Integer
	'�d�m
	Public EN As Short
	'�ړ��^�C�v
	Public Transportation As String
	'�ړ���
	Public Speed As Short
	'�T�C�Y
	Public Size As String
	'���b
	Public Armor As Integer
	'�^����
	Public Mobility As Short
	'�C����
	Public Value As Integer
	'�o���l
	Public ExpValue As Short
	
	'����
	Private proNickname As String
	'�ǂ݉���
	Private proKanaName As String
	
	'�r�b�g�}�b�v��
	Private proBitmap As String
	'�r�b�g�}�b�v�����݂��邩
	Public IsBitmapMissing As Boolean
	
	'����\��
	Public colFeature As Collection
	'����f�[�^
	Private colWeaponData As Collection
	'�A�r���e�B�f�[�^
	Private colAbilityData As Collection
	
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		If Not colFeature Is Nothing Then
			With colFeature
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: �I�u�W�F�N�g colFeature ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			colFeature = Nothing
		End If
		
		If Not colWeaponData Is Nothing Then
			With colWeaponData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: �I�u�W�F�N�g colWeaponData ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			colWeaponData = Nothing
		End If
		
		If Not colAbilityData Is Nothing Then
			With colAbilityData
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: �I�u�W�F�N�g colAbilityData ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			colAbilityData = Nothing
		End If
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'����
	
	Public Property Nickname() As String
		Get
			Nickname = proNickname
			If InStr(Nickname, "��l��") = 1 Or InStr(Nickname, "�q���C��") = 1 Then
				Nickname = GetValueAsString(Nickname & "����")
			End If
			ReplaceSubExpression(Nickname)
		End Get
		Set(ByVal Value As String)
			proNickname = Value
		End Set
	End Property
	
	'�ǂ݉���
	
	Public Property KanaName() As String
		Get
			KanaName = proKanaName
			If InStr(KanaName, "��l��") = 1 Or InStr(KanaName, "�q���C��") = 1 Or InStr(KanaName, "�Ђ낢��") = 1 Then
				If IsVariableDefined(KanaName & "�ǂ݉���") Then
					KanaName = GetValueAsString(KanaName & "�ǂ݉���")
				Else
					KanaName = StrToHiragana(GetValueAsString(KanaName & "����"))
				End If
			End If
			ReplaceSubExpression(KanaName)
		End Get
		Set(ByVal Value As String)
			proKanaName = Value
		End Set
	End Property
	
	'�r�b�g�}�b�v
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
	
	
	'����\�͂�ǉ�
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
		
		'�K�v�Z�\�̐؂�o��
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
		
		'�K�v�����̐؂�o��
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
		
		'����\�͂̎�ށA���x���A�f�[�^��؂�o��
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
		
		'�f�[�^���u"�v�ň͂܂�Ă���ꍇ�A�u"�v���폜
		If Left(fdata, 1) = """" Then
			If Right(fdata, 1) = """" Then
				fdata = Mid(fdata, 2, Len(fdata) - 2)
			End If
		End If
		
		'�G���A�X����`����Ă���H
		If ALDList.IsDefined(ftype) Then
			If LIndex(fdata, 1) <> "���" Then
				With ALDList.Item(ftype)
					For i = 1 To .Count
						fd = New FeatureData
						
						'�G���A�X�̒�`�ɏ]���ē���\�͒�`��u��������
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
							If fdata <> "" And InStr(.AliasData(i), "��\��") <> 1 Then
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
							'����\�͉���̒�`
							If fdata <> "" And LIndex(fdata, 1) <> "��\��" Then
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
						
						'����\�͂�o�^
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
		
		'����\�͂�o�^
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
	
	'����\�͂̑���
	Public Function CountFeature() As Short
		If colFeature Is Nothing Then
			Exit Function
		End If
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
	
	'����\�͂̃��x��
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
	
	'�w�肵������\�͂������Ă��邩�H
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(fname)
		IsFeatureAvailable = True
		Exit Function
		
ErrorHandler: 
		IsFeatureAvailable = False
	End Function
	
	'�w�肵������\�͂����x���w�肳��Ă��邩�H
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
	
	'�����ǉ�
	Public Function AddWeapon(ByRef wname As String) As WeaponData
		Dim new_wdata As New WeaponData
		
		If colWeaponData Is Nothing Then
			colWeaponData = New Collection
		End If
		new_wdata.Name = wname
		colWeaponData.Add(new_wdata, wname & VB6.Format(CountWeapon))
		AddWeapon = new_wdata
	End Function
	
	'����̑���
	Public Function CountWeapon() As Short
		If colWeaponData Is Nothing Then
			Exit Function
		End If
		CountWeapon = colWeaponData.Count()
	End Function
	
	'����f�[�^
	Public Function Weapon(ByRef Index As Object) As WeaponData
		Weapon = colWeaponData.Item(Index)
	End Function
	
	'�A�r���e�B��ǉ�
	Public Function AddAbility(ByRef aname As String) As AbilityData
		Dim new_sadata As New AbilityData
		
		If colAbilityData Is Nothing Then
			colAbilityData = New Collection
		End If
		new_sadata.Name = aname
		colAbilityData.Add(new_sadata, aname & VB6.Format(CountAbility))
		AddAbility = new_sadata
	End Function
	
	'�A�r���e�B�̑���
	Public Function CountAbility() As Short
		If colAbilityData Is Nothing Then
			Exit Function
		End If
		CountAbility = colAbilityData.Count()
	End Function
	
	'�A�r���e�B�f�[�^
	Public Function Ability(ByRef Index As Object) As AbilityData
		Ability = colAbilityData.Item(Index)
	End Function
	
	'����\�́A����f�[�^�A�A�r���e�B�f�[�^���폜����
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