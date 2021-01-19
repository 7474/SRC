Option Strict Off
Option Explicit On
Friend Class Pilots
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�쐬���ꂽ�p�C���b�g�̃f�[�^���Ǘ����郊�X�g�N���X
	
	'�p�C���b�g�ꗗ
	Private colPilots As New Collection
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilots
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colPilots ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colPilots = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach�p�֐�
	'UPGRADE_NOTE: NewEnum �v���p�e�B���R�����g �A�E�g����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ���N���b�N���Ă��������B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colPilots.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: �R���N�V�����񋓎q��Ԃ��ɂ́A�R�����g���������Ĉȉ��̍s��ύX���Ă��������B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ���N���b�N���Ă��������B
		'GetEnumerator = colPilots.GetEnumerator
	End Function
	
	
	'�p�C���b�g��ǉ�
	Public Function Add(ByRef pname As String, ByVal plevel As Short, ByRef pparty As String, Optional ByRef gid As String = "") As Pilot
		Dim new_pilot As New Pilot
		Dim key As String
		Dim i As Short
		
		With new_pilot
			.Name = PDList.Item(pname).Name
			.Level = plevel
			.Party = pparty
			.FullRecover()
			.Alive = True
			'UPGRADE_NOTE: �I�u�W�F�N�g new_pilot.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			.Unit_Renamed = Nothing
			
			If gid = "" Then
				'�O���[�v�h�c���w�肳��Ă��Ȃ��ꍇ
				If InStr(.Name, "(�U�R)") = 0 And InStr(.Name, "(�ėp)") = 0 Then
					key = .Name
					If PList.IsDefined2(key) Then
						If PList.Item2(key).ID = key Then
							'��x�쐬���ꂽ�p�C���b�g�𕜊�������ꍇ
							If Not PList.Item2(key).Alive Then
								With PList.Item2(key)
									.Level = plevel
									.Party = pparty
									.FullRecover()
									.Alive = True
								End With
								Add = PList.Item2(key)
								Exit Function
							End If
							
							ErrorMessage(key & "�Ƃ����p�C���b�g�͊��ɓo�^����Ă��܂�")
							Exit Function
						End If
					End If
				Else
					i = colPilots.Count()
					Do 
						i = i + 1
						key = .Name & "_" & VB6.Format(i)
					Loop While PList.IsDefined2(key)
				End If
			Else
				'�O���[�v�h�c���w�肳��Ă���ꍇ
				key = gid
				i = 1
				Do While PList.IsDefined2(key)
					i = i + 1
					key = gid & ":" & VB6.Format(i)
				Loop 
			End If
			
			.ID = key
			colPilots.Add(new_pilot, key)
			Add = new_pilot
		End With
	End Function
	
	'�o�^����Ă���p�C���b�g�̑���
	Public Function Count() As Short
		Count = colPilots.Count()
	End Function
	
	'���X�g����p�C���b�g���폜
	Public Sub Delete(ByRef Index As Object)
		colPilots.Remove(Index)
	End Sub
	
	'���X�g����p�C���b�g������
	Public Function Item(ByRef Index As Object) As Pilot
		Dim p As Pilot
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilots.Item(Index)
		If Item.Alive Then
			Exit Function
		End If
		
ErrorHandler: 
		'�h�c�Ō�����Ȃ���Ζ��̂Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each p In colPilots
			If p.Data.Name = pname Then
				If p.Alive Then
					Item = p
					Exit Function
				End If
			End If
		Next p
		'����ł�������Ȃ���Έ��̂Ō���
		For	Each p In colPilots
			If p.Data.Nickname = pname Then
				If p.Alive Then
					Item = p
					Exit Function
				End If
			End If
		Next p
	End Function
	
	'�p�C���b�g����`����Ă��邩
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim p As Pilot
		Dim pname As String
		
		On Error GoTo ErrorHandler
		p = colPilots.Item(Index)
		If p.Alive Then
			IsDefined = True
			Exit Function
		End If
		
ErrorHandler: 
		'�h�c�Ō�����Ȃ���Ζ��̂Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each p In colPilots
			If p.Data.Name = pname Then
				If p.Alive Then
					IsDefined = True
					Exit Function
				End If
			End If
		Next p
		'����ł�������Ȃ���Έ��̂Ō���
		For	Each p In colPilots
			If p.Data.Nickname = pname Then
				If p.Alive Then
					IsDefined = True
					Exit Function
				End If
			End If
		Next p
	End Function
	
	'�p�C���b�g������ (ID�݂̂Ō���)
	Public Function Item2(ByRef Index As Object) As Pilot
		On Error GoTo ErrorHandler
		Item2 = colPilots.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item2 = Nothing
	End Function
	
	'�p�C���b�g����`����Ă��邩 (ID�݂̂Ō���)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim p As Pilot
		
		On Error GoTo ErrorHandler
		p = colPilots.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'���X�g�̃A�b�v�f�[�g
	Public Sub Update()
		Dim p As Pilot
		Dim i As Short
		
		For	Each p In colPilots
			With p
				If .Party <> "����" Or Not .Alive Then
					'�����łȂ��p�C���b�g��j�����ꂽ�p�C���b�g�͍폜
					Delete(.ID)
				ElseIf .IsAdditionalPilot Then 
					'�ǉ��p�C���b�g�͍폜
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.pltAdditionalPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							.pltAdditionalPilot = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.OtherForm().pltAdditionalPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
								.OtherForm(i).pltAdditionalPilot = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .IsAdditionalSupport Then 
					'�ǉ��T�|�[�g�͍폜
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.pltAdditionalSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							.pltAdditionalSupport = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.OtherForm().pltAdditionalSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
								.OtherForm(i).pltAdditionalSupport = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .Nickname0 = "�p�C���b�g�s��" Then 
					'�_�~�[�p�C���b�g�͍폜
					Delete(.ID)
				ElseIf Not .Unit_Renamed Is Nothing Then 
					If .Unit_Renamed.IsFeatureAvailable("�������j�b�g") Then
						'�������j�b�g�̒ǉ��p�C���b�g���폜
						Delete(.ID)
					End If
				End If
			End With
		Next p
		
		'�c�����p�C���b�g��S��
		For	Each p In colPilots
			p.FullRecover()
		Next p
	End Sub
	
	
	'�t�@�C���Ƀf�[�^���Z�[�u
	Public Sub Save()
		Dim num As Short
		Dim p As Pilot
		
		'�ǉ��p�C���b�g��ǉ��T�|�[�g���������p�C���b�g�����Z�o
		For	Each p In colPilots
			With p
				If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
					num = num + 1
				End If
			End With
		Next p
		WriteLine(SaveDataFileNumber, num)
		
		For	Each p In colPilots
			With p
				'�ǉ��p�C���b�g��ǉ��T�|�[�g�̓Z�[�u���Ȃ�
				If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
					If .Name = .ID Then
						WriteLine(SaveDataFileNumber, .Name)
					Else
						WriteLine(SaveDataFileNumber, .Name & " " & .ID)
					End If
					WriteLine(SaveDataFileNumber, .Level, .Exp)
					If .Unit_Renamed Is Nothing Then
						If .Away Then
							WriteLine(SaveDataFileNumber, "���E")
						Else
							WriteLine(SaveDataFileNumber, "-")
						End If
					Else
						WriteLine(SaveDataFileNumber, .Unit_Renamed.ID)
					End If
				End If
			End With
		Next p
	End Sub
	
	'�t�@�C������f�[�^�����[�h (�����N�͍s��Ȃ�)
	Public Sub Load()
		Dim i, num As Short
		Dim pname As String
		Dim plevel, pexp As Short
		Dim dummy As String
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			Input(SaveDataFileNumber, pname)
			'Level, Exp
			Input(SaveDataFileNumber, plevel)
			Input(SaveDataFileNumber, pexp)
			'Unit
			Input(SaveDataFileNumber, dummy)
			
			If LLength(pname) = 1 Then
				If Not PDList.IsDefined(pname) Then
					If InStr(pname, "(") > 0 Then
						pname = Left(pname, InStr(pname, "(") - 1)
					End If
					If Not PDList.IsDefined(pname) Then
						ErrorMessage(pname & "�̃f�[�^����`����Ă��܂���")
						TerminateSRC()
						End
					End If
				End If
				With Add(pname, plevel, "����")
					.Exp = pexp
				End With
			Else
				If Not PDList.IsDefined(LIndex(pname, 1)) Then
					ErrorMessage(LIndex(pname, 1) & "�̃f�[�^����`����Ă��܂���")
					TerminateSRC()
					End
				End If
				With Add(LIndex(pname, 1), plevel, "����", LIndex(pname, 2))
					.Exp = pexp
				End With
			End If
		Next 
	End Sub
	
	'�t�@�C������f�[�^��ǂݍ��݃����N
	Public Sub LoadLinkInfo()
		Dim ret, i, num As Short
		Dim pname, uid As String
		Dim dummy As String
		Dim u As Unit
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			Input(SaveDataFileNumber, pname)
			'Level, Exp
			dummy = LineInput(SaveDataFileNumber)
			'Unit
			Input(SaveDataFileNumber, uid)
			
			If LLength(pname) = 1 Then
				If Not IsDefined(pname) Then
					pname = Left(pname, InStr(pname, "(") - 1)
				End If
			End If
			
			Select Case uid
				Case "���E"
					'Leave���ꂽ�p�C���b�g
					If LLength(pname) = 1 Then
						Item(pname).Away = True
					Else
						Item(LIndex(pname, 2)).Away = True
					End If
					GoTo NextPilot
				Case "-", "Dummy"
					'���j�b�g�ɏ���Ă��Ȃ��p�C���b�g
					GoTo NextPilot
			End Select
			
			'���`���̃��j�b�g�h�c��V�`���ɕϊ�
			If SaveDataVersion < 10700 Then
				ConvertUnitID(uid)
			End If
			
			If UList.IsDefined(uid) Then
				'�p�C���b�g�����j�b�g�ɏ悹��
				u = UList.Item(uid)
				If LLength(pname) = 1 Then
					Item(pname).Unit_Renamed = u
				Else
					Item(LIndex(pname, 2)).Unit_Renamed = u
				End If
			Else
				'�悹��ׂ����j�b�g��������Ȃ������ꍇ�͋����I�Ƀ��j�b�g��
				'����ď悹�� (�o�O�΍􂾂������ǁc�c�s�v�H)
				ret = InStr(uid, ":")
				uid = Left(uid, ret - 1)
				If UDList.IsDefined(uid) Then
					u = UList.Add(uid, 0, "����")
					If LLength(pname) = 1 Then
						Item(pname).Ride(u)
					Else
						Item(LIndex(pname, 2)).Ride(u)
					End If
				End If
			End If
NextPilot: 
		Next 
	End Sub
	
	
	'�ꎞ���f�p�f�[�^���Z�[�u����
	Public Sub Dump()
		Dim p As Pilot
		Dim num As Short
		
		'�ǉ��p�C���b�g���������p�C���b�g�����Z�o
		For	Each p In colPilots
			With p
				If Not .IsAdditionalPilot Then
					num = num + 1
				End If
			End With
		Next p
		WriteLine(SaveDataFileNumber, num)
		
		For	Each p In colPilots
			With p
				'�ǉ��p�C���b�g�̓Z�[�u���Ȃ�
				If Not .IsAdditionalPilot Then
					.Dump()
				End If
			End With
		Next p
	End Sub
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
	Public Sub Restore()
		Dim i, num As Short
		Dim buf As String
		Dim p As Pilot
		
		With colPilots
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			p = New Pilot
			With p
				.Restore()
				colPilots.Add(p, .ID)
			End With
		Next 
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
	Public Sub RestoreLinkInfo()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: �I�u�W�F�N�g colPilots().RestoreLinkInfo �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			colPilots.Item(i).RestoreLinkInfo()
		Next 
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
	Public Sub RestoreParameter()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: �I�u�W�F�N�g colPilots().RestoreParameter �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			colPilots.Item(i).RestoreParameter()
		Next 
	End Sub
	
	
	'���X�g���N���A
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
	
	
	'�p�C���b�g�̎x���C�����X�V
	Public Sub UpdateSupportMod(Optional ByVal u As Unit = Nothing)
		Dim p As Pilot
		Dim xx, i, yy As Short
		Dim max_range, range As Short
		
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'���j�b�g�w�肪�Ȃ���ΑS�p�C���b�g���X�V
		If u Is Nothing Then
			For	Each p In colPilots
				p.UpdateSupportMod()
			Next p
			Exit Sub
		End If
		
		With u
			'���j�b�g�Ƀp�C���b�g������Ă��Ȃ���΂��̂܂܏I��
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			With .MainPilot
				'���C���p�C���b�g���X�V
				.UpdateSupportMod()
				
				'�x���͈͂��Z�o
				max_range = .CommandRange
				
				If .IsSkillAvailable("�L��T�|�[�g") Then
					max_range = MaxLng(max_range, 2)
				End If
				
				If IsOptionDefined("�M���␳") And InStr(.Name, "(�U�R)") = 0 Then
					If IsOptionDefined("�M���␳�͈͊g��") Then
						max_range = MaxLng(max_range, 2)
					Else
						max_range = MaxLng(max_range, 1)
					End If
				End If
			End With
			
			'���̃p�C���b�g���X�V
			For i = 2 To .CountPilot
				.Pilot(i).UpdateSupportMod()
			Next 
			For i = 1 To .CountSupport
				.Support(i).UpdateSupportMod()
			Next 
			
			'�x���͈͂������Ȃ瑼�̃��j�b�g�ɏ���Ă���p�C���b�g�ɂ͉e������
			If max_range = 0 Then
				Exit Sub
			End If
			
			'����̃��j�b�g�ɏ���Ă���p�C���b�g�̎x���C�����X�V
			For xx = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For yy = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					If MapDataForUnit(xx, yy) Is Nothing Then
						GoTo NextPoint
					End If
					
					'�x���͈͓��ɂ��邩�`�F�b�N
					range = System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy)
					If range > max_range Then
						GoTo NextPoint
					End If
					If range = 0 Then
						GoTo NextPoint
					End If
					
					'����Ă���p�C���b�g�S���̎x���C�����X�V
					With MapDataForUnit(xx, yy)
						If .CountPilot = 0 Then
							GoTo NextPoint
						End If
						
						.MainPilot.UpdateSupportMod()
						For i = 2 To .CountPilot
							.Pilot(i).UpdateSupportMod()
						Next 
						For i = 1 To .CountSupport
							.Support(i).UpdateSupportMod()
						Next 
					End With
NextPoint: 
				Next 
			Next 
		End With
	End Sub
	
	'�j�����ꂽ�p�C���b�g���폜����
	Public Sub Clean()
		Dim p As Pilot
		
		For	Each p In colPilots
			With p
				If Not .Alive Then
					'�j�����ꂽ�p�C���b�g���폜
					Delete(.ID)
				End If
			End With
		Next p
	End Sub
End Class