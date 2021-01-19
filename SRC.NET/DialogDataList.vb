Option Strict Off
Option Explicit On
Friend Class DialogDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�_�C�A���O�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'�_�C�A���O�f�[�^�̃R���N�V����
	Private colDialogDataList As New Collection
	
	'�N���X�����
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colDialogDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colDialogDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colDialogDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�_�C�A���O�f�[�^��ǉ�
	Public Function Add(ByRef dname As String) As DialogData
		Dim new_dd As New DialogData
		
		new_dd.Name = dname
		colDialogDataList.Add(new_dd, dname)
		Add = new_dd
	End Function
	
	'�_�C�A���O�f�[�^�̑���
	Public Function Count() As Short
		Count = colDialogDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colDialogDataList.Remove(Index)
	End Sub
	
	'�_�C�A���O�f�[�^��Ԃ�
	Public Function Item(ByRef Index As Object) As DialogData
		On Error GoTo ErrorHandler
		
		Item = colDialogDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'�w�肵���_�C�A���O�f�[�^����`����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As DialogData
		
		On Error GoTo ErrorHandler
		dummy = colDialogDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim i, ret As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim pilot_list As String
		Dim d As Dialog
		Dim dd As DialogData
		Dim err_msg As String
		Dim pname, msg As String
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			Do 
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				GetLine(FileNumber, line_buf, line_num)
			Loop While Len(line_buf) = 0
			
			'UPGRADE_NOTE: �I�u�W�F�N�g dd ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			dd = Nothing
			
			'�p�C���b�g���ꗗ
			If LLength(line_buf) = 0 Then
				Error(0)
			End If
			pilot_list = ""
			For i = 1 To LLength(line_buf)
				pilot_list = pilot_list & " " & LIndex(line_buf, i)
			Next 
			pilot_list = Trim(pilot_list)
			
			If IsDefined(pilot_list) Then
				Delete(pilot_list)
			End If
			dd = Add(pilot_list)
			
			GetLine(FileNumber, line_buf, line_num)
			Do While Len(line_buf) > 0
				'�V�`���[�V����
				d = dd.AddDialog(line_buf)
				
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					'�b��
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						Exit Do
					End If
					pname = Left(line_buf, ret - 1)
					
					'�w�肵���b�҂̃f�[�^�����݂��邩�`�F�b�N�B
					'���������̋Z�̃p�[�g�i�[�͏ꍇ�͑��̍�i�̃p�C���b�g�ł��邱�Ƃ�
					'����̂Řb�҃`�F�b�N���s��Ȃ��B
					If Left(pname, 1) <> "@" Then
						If Not PDList.IsDefined(pname) And Not NPDList.IsDefined(pname) And pname <> "�V�X�e��" Then
							err_msg = "�p�C���b�g�u" & pname & "�v����`����Ă��܂���B"
							Error(0)
						End If
					End If
					
					'���b�Z�[�W
					If Len(line_buf) = ret Then
						err_msg = "���b�Z�[�W����`����Ă��܂���B"
						Error(0)
					End If
					msg = Trim(Mid(line_buf, ret + 1))
					
					d.AddMessage(pname, msg)
				Loop 
			Loop 
		Loop 
		
ErrorHandler: 
		If line_num = 0 Then
			ErrorMessage(fname & "���J���܂���B")
		Else
			FileClose(FileNumber)
			If dd Is Nothing Then
				DataErrorMessage(err_msg, fname, line_num, line_buf, "")
			Else
				DataErrorMessage(err_msg, fname, line_num, line_buf, (dd.Name))
			End If
		End If
		TerminateSRC()
	End Sub
End Class