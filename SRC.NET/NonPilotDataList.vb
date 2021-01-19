Option Strict Off
Option Explicit On
Friend Class NonPilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�m���p�C���b�g�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'�m���p�C���b�g�f�[�^�̃R���N�V����
	Private colNonPilotDataList As New Collection
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		Dim npd As New NonPilotData
		
		'Talk�R�}���h�p
		With npd
			.Name = "�i���[�^�["
			.Nickname = "�i���[�^�["
			.Bitmap = ".bmp"
		End With
		colNonPilotDataList.Add(npd, npd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colNonPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colNonPilotDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colNonPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�m���p�C���b�g�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef pname As String) As NonPilotData
		Dim new_pilot_data As New NonPilotData
		
		new_pilot_data.Name = pname
		colNonPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'�m���p�C���b�g�f�[�^���X�g�ɓo�^����Ă���f�[�^�̑���
	Public Function Count() As Short
		Count = colNonPilotDataList.Count()
	End Function
	
	'�m���p�C���b�g�f�[�^���X�g����f�[�^���폜
	Public Sub Delete(ByRef Index As Object)
		colNonPilotDataList.Remove(Index)
	End Sub
	
	'�m���p�C���b�g�f�[�^���X�g����f�[�^�����o��
	Public Function Item(ByRef Index As Object) As NonPilotData
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colNonPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'�m���p�C���b�g�f�[�^���X�g�Ɏw�肵���f�[�^����`����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'�m���p�C���b�g�f�[�^���X�g�Ɏw�肵���f�[�^����`����Ă��邩�H (���̂͌��Ȃ�)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim ret As Short
		Dim pd As New NonPilotData
		Dim data_name As String
		Dim err_msg As String
		
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
			
			If InStr(line_buf, ",") > 0 Then
				err_msg = "���̂̐ݒ肪�����Ă��܂��B"
				Error(0)
			End If
			
			'����
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
				'���łɒ�`����Ă���m���p�C���b�g�̃f�[�^�ł���Βu��������
				If Item(data_name).Name = data_name Then
					pd = Item(data_name)
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'���́A�r�b�g�}�b�v
				GetLine(FileNumber, line_buf, line_num)
				
				'����
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�r�b�g�}�b�v�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If buf2 <> "" Then
					.Nickname = buf2
				Else
					err_msg = "���̂̐ݒ肪�Ԉ���Ă��܂��B"
					Error(0)
				End If
				
				'�r�b�g�}�b�v
				buf2 = Trim(buf)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("�r�b�g�}�b�v�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
End Class