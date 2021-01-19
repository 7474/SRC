Option Strict Off
Option Explicit On
Friend Class AliasDataList
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�G���A�X�f�[�^���Ǘ����郊�X�g�̃N���X
	
	
	'�G���A�X�f�[�^�̃R���N�V����
	Private colAliasDataList As New Collection
	
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colAliasDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colAliasDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colAliasDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�G���A�X�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef aname As String) As AliasData
		Dim ad As New AliasData
		
		ad.Name = aname
		colAliasDataList.Add(ad, aname)
		Add = ad
	End Function
	
	'�G���A�X�f�[�^���X�g�ɓo�^����Ă���f�[�^�̑���
	Public Function Count() As Short
		Count = colAliasDataList.Count()
	End Function
	
	'�G���A�X�f�[�^���X�g����f�[�^���폜
	Public Sub Delete(ByRef Index As Object)
		colAliasDataList.Remove(Index)
	End Sub
	
	'�G���A�X�f�[�^���X�g����f�[�^�����o��
	Public Function Item(ByRef Index As Object) As AliasData
		Item = colAliasDataList.Item(Index)
	End Function
	
	'�G���A�X�f�[�^���X�g�Ɏw�肵���f�[�^����`����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim ad As AliasData
		
		On Error GoTo ErrorHandler
		ad = colAliasDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim ad As AliasData
		Dim data_name As String
		Dim err_msg As String
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			data_name = ""
			
			Do 
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				GetLine(FileNumber, line_buf, line_num)
			Loop While Len(line_buf) = 0
			
			If InStr(line_buf, ",") > 0 Then
				err_msg = "���̂̐ݒ肪�����Ă��܂��B"
				Error(0)
			End If
			
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "���̂ɔ��p�X�y�[�X�͎g�p�o���܂���B"
				Error(0)
			End If
			If InStr(data_name, "�i") > 0 Or InStr(data_name, "�j") > 0 Then
				err_msg = "���̂ɑS�p���ʂ͎g�p�o���܂���B"
				Error(0)
			End If
			If InStr(data_name, """") > 0 Then
				err_msg = "���̂�""�͎g�p�o���܂���B"
				Error(0)
			End If
			
			If IsDefined(data_name) Then
				'���łɒ�`����Ă���G���A�X�̃f�[�^�ł���Βu��������
				Delete(data_name)
			End If
			ad = Add(data_name)
			
			With ad
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					If Len(line_buf) = 0 Then
						Exit Do
					End If
					
					.AddAlias(line_buf)
				Loop 
				If .Count = 0 Then
					err_msg = "�G���A�X�Ώۂ̃f�[�^����`����Ă��܂���B"
					Error(0)
				End If
			End With
		Loop 
		
ErrorHandler: 
		'�G���[����
		If line_num = 0 Then
			ErrorMessage(fname & "���J���܂���B")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
	
	'ForEach�p�֐�
	'UPGRADE_NOTE: NewEnum �v���p�e�B���R�����g �A�E�g����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ���N���b�N���Ă��������B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colAliasDataList.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: �R���N�V�����񋓎q��Ԃ��ɂ́A�R�����g���������Ĉȉ��̍s��ύX���Ă��������B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ���N���b�N���Ă��������B
		'GetEnumerator = colAliasDataList.GetEnumerator
	End Function
End Class