Option Strict Off
Option Explicit On
Friend Class Units
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S���j�b�g�̃f�[�^���Ǘ����郊�X�g�̃N���X
	
	'���j�b�g�h�c�쐬�p�J�E���^
	Public IDCount As Integer
	
	'���j�b�g�ꗗ
	Private colUnits As New Collection
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		IDCount = 0
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colUnits ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colUnits = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach�p�֐�
	'UPGRADE_NOTE: NewEnum �v���p�e�B���R�����g �A�E�g����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ���N���b�N���Ă��������B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colUnits.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: �R���N�V�����񋓎q��Ԃ��ɂ́A�R�����g���������Ĉȉ��̍s��ύX���Ă��������B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ���N���b�N���Ă��������B
		'GetEnumerator = colUnits.GetEnumerator
	End Function
	
	
	'���j�b�g���X�g�ɐV�������j�b�g��ǉ�
	Public Function Add(ByRef uname As String, ByVal urank As Short, ByRef uparty As String) As Unit
		Dim new_unit As Unit
		Dim new_form As Unit
		Dim ud As UnitData
		Dim uname2 As String
		Dim other_forms() As String
		Dim i, j As Short
		Dim list As String
		
		'���j�b�g�f�[�^����`����Ă���H
		If Not UDList.IsDefined(uname) Then
			'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Add = Nothing
			Exit Function
		End If
		
		ud = UDList.Item(uname)
		
		new_unit = New Unit
		Add = new_unit
		With new_unit
			.Name = ud.Name
			.Rank = urank
			.Party = uparty
			.ID = CreateID((ud.Name))
			.FullRecover()
		End With
		colUnits.Add(new_unit, new_unit.ID)
		
		'����ȍ~�͖{�̈ȊO�̌`�Ԃ̒ǉ�
		ReDim other_forms(0)
		
		'�ό`��̌`��
		list = ud.FeatureData("�ό`")
		For i = 2 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̕ό`��`�ԁu" & uname2 & "�v��������܂���")
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'�n�C�p�[���[�h��̌`��
		If ud.IsFeatureAvailable("�n�C�p�[���[�h") Then
			list = ud.FeatureData("�n�C�p�[���[�h")
			uname2 = LIndex(list, 2)
			If Not UDList.IsDefined(uname2) Then
				If uname = "" Then
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃n�C�p�[���[�h��`�Ԃ��w�肳��Ă��܂���")
				Else
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃n�C�p�[���[�h��`�ԁu" & uname2 & "�v��������܂���")
				End If
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'�m�[�}�����[�h��̌`��
		If ud.IsFeatureAvailable("�m�[�}�����[�h") Then
			list = ud.FeatureData("�m�[�}�����[�h")
			uname2 = LIndex(list, 1)
			If Not UDList.IsDefined(uname2) Then
				If uname2 = "" Then
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃m�[�}�����[�h��`�Ԃ��w�肳��Ă��܂���")
				Else
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃m�[�}�����[�h��`�ԁu" & uname2 & "�v��������܂���")
				End If
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'�p�[�c������̌`��
		If ud.IsFeatureAvailable("�p�[�c����") Then
			uname2 = LIndex(ud.FeatureData("�p�[�c����"), 2)
			If Not UDList.IsDefined(uname2) Then
				If uname2 = "" Then
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃p�[�c������`�Ԃ��w�肳��Ă��܂���")
				Else
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃p�[�c������`�ԁu" & uname2 & "�v��������܂���")
				End If
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'�p�[�c���̐�̌`��
		If ud.IsFeatureAvailable("�p�[�c����") Then
			uname2 = ud.FeatureData("�p�[�c����")
			If Not UDList.IsDefined(uname2) Then
				If uname = "" Then
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃p�[�c���̐�`�Ԃ��w�肳��Ă��܂���")
				Else
					ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̃p�[�c���̐�`�ԁu" & uname2 & "�v��������܂���")
				End If
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'�ό`�Z�̕ό`��̌`��
		With ud
			If .IsFeatureAvailable("�ό`�Z") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "�ό`�Z" Then
						uname2 = LIndex(.FeatureData(i), 2)
						If Not UDList.IsDefined(uname2) Then
							If uname2 = "" Then
								ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̕ό`�Z�g�p��`�Ԃ��w�肳��Ă��܂���")
							Else
								ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̕ό`�Z�g�p��`�ԁu" & uname2 & "�v��������܂���")
							End If
							'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							Add = Nothing
							Exit Function
						End If
						ReDim Preserve other_forms(UBound(other_forms) + 1)
						other_forms(UBound(other_forms)) = uname2
					End If
				Next 
			End If
		End With
		
		'������̌`��
		list = ud.FeatureData("����")
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̊�����`�ԁu" & uname2 & "�v��������܂���")
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'���`�ԂŎw�肳�ꂽ�`��
		list = ud.FeatureData("���`��")
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("���j�b�g�f�[�^�u" & uname & "�v�̑��`�ԁu" & uname2 & "�v��������܂���")
				'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'�`�Ԃ�ǉ�
		For i = 1 To UBound(other_forms)
			If Not new_unit.IsOtherFormDefined(other_forms(i)) Then
				new_form = New Unit
				With new_form
					.Name = other_forms(i)
					.Rank = urank
					.Party = uparty
					.ID = CreateID((ud.Name))
					.FullRecover()
					.Status_Renamed = "���`��"
				End With
				colUnits.Add(new_form, new_form.ID)
				new_unit.AddOtherForm(new_form)
			End If
		Next 
		
		'�ǉ������`�Ԃɑ΂��Ď������g��ǉ����Ă���
		For i = 1 To new_unit.CountOtherForm
			new_unit.OtherForm(i).AddOtherForm(new_unit)
			For j = 1 To new_unit.CountOtherForm
				If Not i = j Then
					new_unit.OtherForm(i).AddOtherForm(new_unit.OtherForm(j))
				End If
			Next 
		Next 
		
		'���ɍ��̐� or ������̃��j�b�g���쐬����Ă���Ύ����͑��`��
		With ud
			For i = 1 To .CountFeature
				If .Feature(i) = "����" Then
					If UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
						new_unit.Status_Renamed = "���`��"
						Exit Function
					End If
				End If
				If .Feature(i) = "����" Then
					For j = 2 To LLength(.FeatureData(i))
						If UList.IsDefined(LIndex(.FeatureData(i), j)) Then
							new_unit.Status_Renamed = "���`��"
							Exit Function
						End If
					Next 
				End If
			Next 
		End With
	End Function
	
	'���j�b�g���X�g�Ƀ��j�b�g u ��ǉ�
	Public Sub Add2(ByRef u As Unit)
		colUnits.Add(u, u.ID)
	End Sub
	
	'�V�K���j�b�gID���쐬
	Public Function CreateID(ByRef uname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined(uname & ":" & VB6.Format(IDCount))
		CreateID = uname & ":" & VB6.Format(IDCount)
	End Function
	
	'���j�b�g���X�g�ɓo�^����Ă��郆�j�b�g����Ԃ�
	Public Function Count() As Short
		Count = colUnits.Count()
	End Function
	
	'���j�b�g���X�g���烆�j�b�g���폜
	Public Sub Delete(ByRef Index As Object)
		colUnits.Remove(Index)
	End Sub
	
	'���j�b�g���X�g����w�肳�ꂽ���j�b�g��Ԃ�
	Public Function Item(ByRef Index As Object) As Unit
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		Item = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'ID�Ō�����Ȃ���΃��j�b�g���Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					If .Status_Renamed <> "�j��" Then
						Item = u
						Exit Function
					End If
				End If
			End With
		Next u
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'���j�b�g���X�g���烆�j�b�g������ (ID�̂�)
	Public Function Item2(ByRef Index As Object) As Unit
		On Error GoTo ErrorHandler
		Item2 = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item2 = Nothing
	End Function
	
	'���j�b�g���X�g�Ɏw�肳�ꂽ���j�b�g����`����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'ID�Ō�����Ȃ���΃��j�b�g���Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					If .Status_Renamed <> "�j��" Then
						IsDefined = True
						Exit Function
					End If
				End If
			End With
		Next u
		IsDefined = False
	End Function
	
	'���j�b�g���X�g�Ɏw�肳�ꂽ���j�b�g����`����Ă��邩�H (ID�̂�)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'���j�b�g���X�g���A�b�v�f�[�g
	Public Sub Update()
		Dim u As Unit
		Dim k, i, j, n As Short
		Dim prev_money As Integer
		Dim flag As Boolean
		Dim pname, uname, uname2, buf As String
		
		'��͂Ɋi�[���ꂽ���j�b�g���~�낷
		For	Each u In colUnits
			With u
				For i = 1 To .CountUnitOnBoard
					.UnloadUnit(1)
				Next 
			End With
		Next u
		
		'�j�󂳂ꂽ�������j�b�g�����邩����
		For	Each u In colUnits
			With u
				If .Party0 = "����" Then
					If .Status_Renamed = "�j��" Then
						flag = True
						Exit For
					End If
				ElseIf .Party0 = "�m�o�b" Then 
					If .Status_Renamed = "�j��" Then
						If Not .Summoner Is Nothing Then
							If .Summoner.Party0 = "����" Then
								flag = True
								Exit For
							End If
						End If
					End If
				End If
			End With
		Next u
		
		'�j�󂳂ꂽ�������j�b�g������ΏC��
		If flag Then
			OpenMessageForm()
			prev_money = Money
			For	Each u In colUnits
				With u
					If .Status_Renamed <> "�j��" Then
						GoTo NextDestroyedUnit
					End If
					If .IsFeatureAvailable("�������j�b�g") Then
						GoTo NextDestroyedUnit
					End If
					Select Case .Party0
						Case "����"
						Case "�m�o�b"
							If .Summoner Is Nothing Then
								GoTo NextDestroyedUnit
							ElseIf .Summoner.Party0 <> "����" Then 
								GoTo NextDestroyedUnit
							End If
						Case Else
							GoTo NextDestroyedUnit
					End Select
					
					IncrMoney(-.Value)
					.Status_Renamed = "�ҋ@"
					If Not .IsHero Then
						DisplayMessage("�V�X�e��", .Nickname & "���C������;�C���� = " & VB6.Format(.Value))
					Else
						DisplayMessage("�V�X�e��", .Nickname & "�����Â���;���Ô� = " & VB6.Format(.Value))
					End If
				End With
NextDestroyedUnit: 
			Next u
			DisplayMessage("�V�X�e��", "���v = " & VB6.Format(prev_money - Money))
			CloseMessageForm()
		End If
		
		'�S���j�b�g��ҋ@��ԂɕύX
		For	Each u In colUnits
			With u
				Select Case .Status_Renamed
					Case "�o��", "�i�["
						.Status_Renamed = "�ҋ@"
				End Select
			End With
		Next u
		
		'�R�i�K�܂ł̕ό`�E���̂ɑΉ�
		For i = 1 To 3
			'�m�[�}�����[�h�E�p�[�c���̂��s��
			For	Each u In colUnits
				With u
					If .Party0 = "����" And .Status_Renamed <> "���`��" And .Status_Renamed <> "����`��" And .Status_Renamed <> "���`��" Then
						If .IsFeatureAvailable("�m�[�}�����[�h") Then
							.Transform(LIndex(.FeatureData("�m�[�}�����[�h"), 1))
						ElseIf .IsFeatureAvailable("�p�[�c����") Then 
							If LLength(.FeatureData("�p�[�c����")) = 2 Then
								.Transform(LIndex(.FeatureData("�p�[�c����"), 2))
							Else
								.Transform(LIndex(.FeatureData("�p�[�c����"), 1))
							End If
						End If
					End If
				End With
			Next u
			
			'�������s��
			For	Each u In colUnits
				With u
					If Not .IsFeatureAvailable("����") Then
						GoTo NextLoop1
					End If
					If .Party0 <> "����" Or .Status_Renamed = "���`��" Or .Status_Renamed = "����`��" Or .Status_Renamed = "���`��" Then
						GoTo NextLoop1
					End If
					
					If .Status_Renamed = "�j��" Then
						If .CountPilot = 0 Then
							GoTo NextLoop1
						End If
					End If
					
					'���̌`�Ԃ���`�ԂȂ番�����s��Ȃ�
					
					If LLength(.FeatureData("����")) > 3 And Not .IsFeatureAvailable("��������") Then
						GoTo NextLoop1
					End If
					
					If .IsFeatureAvailable("��`��") Then
						GoTo NextLoop1
					End If
					
					'�p�C���b�g������Ȃ��ꍇ�͕������s��Ȃ�
					n = 0
					For j = 2 To LLength(.FeatureData("����"))
						uname = LIndex(.FeatureData("����"), j)
						If UDList.IsDefined(uname) Then
							With UDList.Item(uname)
								If Not .IsFeatureAvailable("�������j�b�g") Then
									n = n + .PilotNum
								End If
							End With
						End If
					Next 
					If .CountPilot < n Then
						GoTo NextLoop1
					End If
					
					'������̌`�Ԃ����p�\�H
					For j = 2 To LLength(.FeatureData("����"))
						uname = LIndex(.FeatureData("����"), j)
						If Not UList.IsDefined(uname) Then
							GoTo NextLoop1
						End If
						If UList.Item(uname).CurrentForm.Status_Renamed = "�ҋ@" Then
							GoTo NextLoop1
						End If
					Next 
					
					'���������{
					.Split_Renamed()
				End With
NextLoop1: 
			Next u
			
			'���̂��s��
			For	Each u In colUnits
				With u
					If .Party0 = "����" And .Status_Renamed <> "���`��" And .Status_Renamed <> "����`��" And .Status_Renamed <> "���`��" Then
						If .IsFeatureAvailable("����") Then
							For j = 1 To .CountFeature
								If .Feature(j) <> "����" Then
									GoTo NextLoop2
								End If
								
								'���̌�̌`�Ԃ����p�\�H
								uname = LIndex(.FeatureData(j), 2)
								If Not UList.IsDefined(uname) Then
									GoTo NextLoop2
								End If
								With UList.Item(uname)
									If u.Status_Renamed = "�ҋ@" And .CurrentForm.Status_Renamed = "���E" Then
										GoTo NextLoop2
									End If
									If .IsFeatureAvailable("��������") Then
										GoTo NextLoop2
									End If
									If Not .IsFeatureAvailable("��`��") And LLength(u.FeatureData(j)) = 3 Then
										GoTo NextLoop2
									End If
								End With
								
								'���̂̃p�[�g�i�[�����p�\�H
								For k = 3 To LLength(.FeatureData(j))
									uname = LIndex(.FeatureData(j), k)
									If Not UList.IsDefined(uname) Then
										GoTo NextLoop2
									End If
									With UList.Item(uname)
										If u.Status_Renamed = "�ҋ@" Then
											If .CurrentForm.Status_Renamed <> "�ҋ@" Then
												GoTo NextLoop2
											End If
										Else
											If .CurrentForm.Status_Renamed <> "���E" Then
												GoTo NextLoop2
											End If
										End If
									End With
								Next 
								
								'���̂����{
								.Combine(LIndex(.FeatureData(j), 2))
								Exit For
NextLoop2: 
							Next 
						End If
					End If
				End With
			Next u
			
			'�W���`�Ԃɕό`
			For	Each u In colUnits
				With u
					If .Party0 = "����" And .Status_Renamed <> "���`��" And .Status_Renamed <> "����`��" And .Status_Renamed <> "���`��" Then
						If .IsFeatureAvailable("�ό`") Then
							uname = .Name
							buf = .FeatureData("�ό`")
							For j = 2 To LLength(buf)
								uname2 = LIndex(buf, j)
								If UDList.IsDefined(uname2) Then
									If UDList.Item(uname2).ID < UDList.Item(uname).ID Then
										uname = uname2
									End If
								Else
									ErrorMessage(uname & "�̕ό`�惆�j�b�g�u" & uname2 & "�v�̃f�[�^����`����Ă��܂���B")
								End If
							Next 
							
							If uname <> .Name Then
								.Transform(uname)
							End If
						End If
					End If
				End With
			Next u
		Next 
		
		'�\�����p�C���b�g���폜
		For	Each u In colUnits
			With u
				If .IsFeatureAvailable("�\�����p�C���b�g") Then
					If PList.IsDefined(.FeatureData("�\�����p�C���b�g")) Then
						PList.Delete(.FeatureData("�\�����p�C���b�g"))
					End If
				End If
			End With
		Next u
		
		'�_�~�[�p�C���b�g���폜
		For	Each u In colUnits
			With u
				If .CountPilot > 0 Then
					If .Pilot(1).Nickname0 = "�p�C���b�g�s��" Then
						.DeletePilot(1)
					End If
				End If
			End With
		Next u
		
		'�ϐg��̌`�ԓ��A�ꎞ�I�Ȍ`�Ԃ��폜
		For	Each u In colUnits
			With u
				If .Status_Renamed = "�ҋ@" Then
					.DeleteTemporaryOtherForm()
				End If
			End With
		Next u
		
		'�j�����ꂽ���j�b�g���폜
		For	Each u In colUnits
			With u
				'�������j�b�g�͕K���j��
				If .IsFeatureAvailable("�������j�b�g") Then
					.Status_Renamed = "�j��"
				End If
				'�_�~�[���j�b�g��j��
				If .IsFeatureAvailable("�_�~�[���j�b�g") Then
					.Status_Renamed = "�j��"
				End If
				
				'�������j�b�g�ȊO�̃��j�b�g�Ɣj�����ꂽ���j�b�g���폜
				If .Party0 <> "����" Or .Status_Renamed = "�j��" Then
					'���j�b�g���������Ă���A�C�e�����j��
					For i = 1 To .CountItem
						.Item(i).Exist = False
					Next 
					Delete(.ID)
				End If
			End With
		Next u
		
		'���j�b�g�̏�Ԃ���
		For	Each u In colUnits
			u.Reset_Renamed()
		Next u
		
		'�X�e�[�^�X���A�b�v�f�[�g
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'���j�b�g���X�g�ɓo�^���ꂽ���j�b�g�̏����Z�[�u
	Public Sub Save()
		Dim i As Short
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, IDCount)
		WriteLine(SaveDataFileNumber, Count)
		For	Each u In colUnits
			With u
				WriteLine(SaveDataFileNumber, .Name)
				WriteLine(SaveDataFileNumber, .ID, .Rank, .Status_Renamed)
				
				WriteLine(SaveDataFileNumber, .CountOtherForm)
				For i = 1 To .CountOtherForm
					WriteLine(SaveDataFileNumber, .OtherForm(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountPilot)
				For i = 1 To .CountPilot
					WriteLine(SaveDataFileNumber, .Pilot(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountSupport)
				For i = 1 To .CountSupport
					WriteLine(SaveDataFileNumber, .Support(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountItem)
				For i = 1 To .CountItem
					WriteLine(SaveDataFileNumber, .Item(i).ID)
				Next 
			End With
		Next u
	End Sub
	
	'���j�b�g���X�g�Ƀ��j�b�g�̏������[�h
	'(�����N�͌�ōs��)
	Public Sub Load()
		Dim num, num2 As Short
		Dim new_unit As Unit
		Dim Name As String
		'UPGRADE_NOTE: Status �� Status_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
		Dim ID, Status_Renamed As String
		Dim Rank As Short
		Dim i, j As Short
		Dim dummy As String
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			new_unit = New Unit
			With new_unit
				'Name
				Input(SaveDataFileNumber, Name)
				
				If Not UDList.IsDefined(Name) Then
					ErrorMessage(Name & "�̃f�[�^����`����Ă��܂���")
					TerminateSRC()
					End
				End If
				
				'ID, Rank, Status
				Input(SaveDataFileNumber, ID)
				Input(SaveDataFileNumber, Rank)
				Input(SaveDataFileNumber, Status_Renamed)
				
				'���`���̃��j�b�g�h�c��V�`���ɕϊ�
				If SaveDataVersion < 10700 Then
					ConvertUnitID(ID)
				End If
				
				.Name = Name
				.ID = ID
				.Rank = Rank
				.Party = "����"
				.Status_Renamed = Status_Renamed
				.FullRecover()
			End With
			colUnits.Add(new_unit, new_unit.ID)
			
			'OtherForm
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Pilot
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Support
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Item
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
		Next 
	End Sub
	
	'���j�b�g���X�g�Ƀ��j�b�g�̏������[�h���A�����N���s��
	Public Sub LoadLinkInfo()
		Dim num, num2 As Short
		Dim ID, ID2 As String
		Dim i, j As Short
		Dim int_dummy As Short
		Dim str_dummy As String
		Dim u As Unit
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'Name
			str_dummy = LineInput(SaveDataFileNumber)
			'ID, Rank, Status
			Input(SaveDataFileNumber, ID)
			Input(SaveDataFileNumber, int_dummy)
			Input(SaveDataFileNumber, str_dummy)
			
			'���`���̃��j�b�g�h�c��V�`���ɕϊ�
			If SaveDataVersion < 10700 Then
				ConvertUnitID(ID)
			End If
			
			With Item(ID)
				'OtherForm
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					ConvertUnitID(ID2)
					If IsDefined(ID2) Then
						.AddOtherForm(Item(ID2))
					End If
				Next 
				
				'Pilot
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddPilot(PList.Item(ID2))
						If .Status_Renamed = "���E" Then
							PList.Item(ID2).Away = True
						End If
					Else
						ID2 = Left(ID2, InStr(ID2, "(") - 1)
						If PList.IsDefined(ID2) Then
							.AddPilot(PList.Item(ID2))
							If .Status_Renamed = "���E" Then
								PList.Item(ID2).Away = True
							End If
						End If
					End If
				Next 
				
				'Support
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddSupport(PList.Item(ID2))
						If .Status_Renamed = "���E" Then
							PList.Item(ID2).Away = True
						End If
					End If
				Next 
				
				'Unit
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If IList.IsDefined(ID2) Then
						If IList.Item(ID2).Unit Is Nothing Then
							.CurrentForm.AddItem0(IList.Item(ID2))
						End If
					ElseIf IDList.IsDefined(ID2) Then 
						.CurrentForm.AddItem0(IList.Add(ID2))
					End If
				Next 
			End With
		Next 
		
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
	Public Sub Dump()
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, Count)
		
		For	Each u In colUnits
			u.Dump()
		Next u
	End Sub
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
	Public Sub Restore()
		Dim i, num As Short
		Dim u As Unit
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			u = New Unit
			With u
				.Restore()
				colUnits.Add(u, .ID)
			End With
		Next 
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
	Public Sub RestoreLinkInfo()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreLinkInfo()
		Next u
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
	Public Sub RestoreParameter()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreParameter()
		Next u
	End Sub
	
	
	'���j�b�g���X�g���N���A
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
	
	'���j�b�g���X�g�ɓo�^���ꂽ���j�b�g�̃r�b�g�}�b�vID���N���A
	Public Sub ClearUnitBitmap()
		Dim u As Unit
		
		'UPGRADE_ISSUE: Control picUnitBitmap �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picUnitBitmap
			'UPGRADE_ISSUE: Control picUnitBitmap �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			If .Width = 32 Then
				'���ɃN���A����Ă���΂��̂܂܏I��
				Exit Sub
			End If
			
			'�摜���N���A
			'UPGRADE_ISSUE: Control picUnitBitmap �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picUnitBitmap �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Move(0, 0, 32, 96)
		End With
		
		'BitmapID���N���A
		For	Each u In colUnits
			u.BitmapID = 0
		Next u
	End Sub
	
	
	'�n�C�p�[���[�h�̎��������`�F�b�N
	Public Sub CheckAutoHyperMode()
		Dim u As Unit
		
		For	Each u In colUnits
			u.CheckAutoHyperMode()
		Next u
	End Sub
	
	'�m�[�}�����[�h�̎��������`�F�b�N
	Public Sub CheckAutoNormalMode()
		Dim u As Unit
		Dim is_redraw_necessary As Boolean
		
		For	Each u In colUnits
			If u.CheckAutoNormalMode(True) Then
				is_redraw_necessary = True
			End If
		Next u
		
		'��ʂ̍ĕ`�悪�K�v�H
		If is_redraw_necessary Then
			RedrawScreen()
		End If
	End Sub
	
	'�j�����ꂽ���j�b�g���폜
	Public Sub Clean()
		Dim u As Unit
		Dim i As Short
		
		For	Each u In colUnits
			With u
				'�o�����Ă��Ȃ����j�b�g�͖������j�b�g�ȊO�S�č폜
				If .Party0 <> "����" Then
					If .Status_Renamed = "�ҋ@" Or .Status_Renamed = "�j��" Then
						.Status_Renamed = "�j��"
						For i = 1 To .CountOtherForm
							.OtherForm(i).Status_Renamed = "�j��"
						Next 
					End If
				End If
			End With
		Next u
		
		For	Each u In colUnits
			With u
				'�j�����ꂽ���j�b�g���폜
				If .Status_Renamed = "�j��" Then
					'���j�b�g�ɏ���Ă���p�C���b�g���j��
					For i = 1 To .CountPilot
						.Pilot(i).Alive = False
					Next 
					For i = 1 To .CountSupport
						.Support(i).Alive = False
					Next 
					
					'���j�b�g���������Ă���A�C�e�����j��
					For i = 1 To .CountItem
						.Item(i).Exist = False
					Next 
					
					Delete(.ID)
				End If
			End With
		Next u
	End Sub
End Class