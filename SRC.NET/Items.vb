Option Strict Off
Option Explicit On
Friend Class Items
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�A�C�e���h�c�쐬�p�J�E���^
	Private IDCount As Integer
	
	'�A�C�e���ꗗ
	Private colItems As New Collection
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colItems
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colItems ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colItems = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach�p�֐�
	'UPGRADE_NOTE: NewEnum �v���p�e�B���R�����g �A�E�g����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ���N���b�N���Ă��������B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colItems.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: �R���N�V�����񋓎q��Ԃ��ɂ́A�R�����g���������Ĉȉ��̍s��ύX���Ă��������B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ���N���b�N���Ă��������B
		'GetEnumerator = colItems.GetEnumerator
	End Function
	
	
	'���X�g�ɃA�C�e����ǉ�
	Public Function Add(ByRef Name As String) As Item
		Dim new_item As Item
		
		If Not IDList.IsDefined(Name) Then
			'UPGRADE_NOTE: �I�u�W�F�N�g Add ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Add = Nothing
			Exit Function
		End If
		
		new_item = New Item
		Add = new_item
		With new_item
			.Name = Name
			.ID = CreateID(Name)
		End With
		colItems.Add(new_item, new_item.ID)
	End Function
	
	'�V�����A�C�e���h�c���쐬
	Private Function CreateID(ByRef iname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined2(iname & "_" & VB6.Format(IDCount))
		CreateID = iname & "_" & VB6.Format(IDCount)
	End Function
	
	'���X�g�ɓo�^����Ă���A�C�e���̑���
	Public Function Count() As Short
		Count = colItems.Count()
	End Function
	
	'���X�g����A�C�e�����폜
	Public Sub Delete(ByRef Index As Object)
		colItems.Remove(Index)
	End Sub
	
	'�w�肳�ꂽ�A�C�e��������
	Public Function Item(ByRef Index As Object) As Item
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		Item = colItems.Item(Index)
		
		'�j������Ă��Ȃ��H
		If Item.Exist Then
			Exit Function
		End If
		
ErrorHandler: 
		'������Ȃ���΃A�C�e�����Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		iname = CStr(Index)
		For	Each it In colItems
			With it
				If .Name = iname And .Exist Then
					Item = it
					Exit Function
				End If
			End With
		Next it
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'�w�肳�ꂽ�A�C�e�����o�^����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		
		'�j�����ꂽ�A�C�e���͓o�^����Ă��Ȃ��Ƃ݂Ȃ�
		If it.Exist Then
			IsDefined = True
			Exit Function
		End If
		
ErrorHandler: 
		'������Ȃ���΃A�C�e�����Ō���
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		iname = CStr(Index)
		For	Each it In colItems
			With it
				If .Name = iname And .Exist Then
					IsDefined = True
					Exit Function
				End If
			End With
		Next it
		IsDefined = False
	End Function
	
	'�A�C�e������Exit�t���O�𖳎����ăA�C�e��������
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim it As Item
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'���X�g�ɓo�^���ꂽ�A�C�e�����A�b�v�f�[�g
	Public Sub Update()
		Dim it As Item
		
		'�j�����ꂽ�A�C�e�����폜
		For	Each it In colItems
			With it
				If Not .Exist Then
					colItems.Remove(.ID)
				End If
			End With
		Next it
		
		'�����N�f�[�^�̐����������
		For	Each it In colItems
			With it
				If Not .Unit_Renamed Is Nothing Then
					.Unit_Renamed = UList.Item((.Unit_Renamed.ID))
				End If
			End With
		Next it
	End Sub
	
	
	'�f�[�^���t�@�C���ɃZ�[�u
	Public Sub Save()
		Dim i As Short
		
		WriteLine(SaveDataFileNumber, IDCount)
		WriteLine(SaveDataFileNumber, Count)
		For i = 1 To Count
			With Item(i)
				WriteLine(SaveDataFileNumber, .Name)
				If .Unit_Renamed Is Nothing Then
					WriteLine(SaveDataFileNumber, .ID, "-")
				Else
					WriteLine(SaveDataFileNumber, .ID, .Unit_Renamed.ID)
				End If
			End With
		Next 
	End Sub
	
	'�f�[�^���t�@�C�����烍�[�h
	Public Sub Load()
		Dim num As Short
		Dim new_item As Item
		Dim iname As String
		Dim iid As String
		Dim i As Short
		Dim dummy As String
		
		If EOF(SaveDataFileNumber) Then
			Exit Sub
		End If
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			new_item = New Item
			With new_item
				'Name
				Input(SaveDataFileNumber, iname)
				'ID, Unit
				Input(SaveDataFileNumber, iid)
				Input(SaveDataFileNumber, dummy)
				
				If Not IDList.IsDefined(iname) Then
					ErrorMessage(iname & "�̃f�[�^����`����Ă��܂���")
					StopBGM()
					End
				End If
				
				.Name = iname
				.ID = iid
			End With
			colItems.Add(new_item, iid)
		Next 
	End Sub
	
	'�����N�����t�@�C�����烍�[�h
	Public Sub LoadLinkInfo()
		Dim num, i As Short
		Dim dummy As String
		
		If EOF(SaveDataFileNumber) Then
			Exit Sub
		End If
		
		'IDCount
		dummy = LineInput(SaveDataFileNumber)
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			dummy = LineInput(SaveDataFileNumber)
			'ID, Unit
			dummy = LineInput(SaveDataFileNumber)
		Next 
	End Sub
	
	
	'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
	Public Sub Dump()
		Dim it As Item
		
		WriteLine(SaveDataFileNumber, Count)
		For	Each it In colItems
			it.Dump()
		Next it
	End Sub
	
	'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
	Public Sub Restore()
		Dim i, num As Short
		Dim it As Item
		
		With colItems
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			it = New Item
			With it
				.Restore()
				colItems.Add(it, .ID)
			End With
		Next 
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃����N�����t�@�C�����烍�[�h����
	Public Sub RestoreLinkInfo()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreLinkInfo()
		Next it
	End Sub
	
	'�ꎞ���f�p�f�[�^�̃p�����[�^�����t�@�C�����烍�[�h����
	Public Sub RestoreParameter()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreParameter()
		Next it
	End Sub
	
	
	'���X�g���N���A
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
End Class