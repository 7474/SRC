Option Strict Off
Option Explicit On
Friend Class MessageDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S���b�Z�[�W�f�[�^(�܂��͓�����ʃf�[�^)���Ǘ�����R���N�V�����N���X
	
	'���b�Z�[�W�f�[�^(�܂��͓�����ʃf�[�^)�ꗗ
	Private colMessageDataList As New Collection
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colMessageDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colMessageDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colMessageDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'���b�Z�[�W�f�[�^�̒ǉ�
	Public Function Add(ByRef mname As String) As MessageData
		Dim new_md As New MessageData
		
		new_md.Name = mname
		colMessageDataList.Add(new_md, mname)
		Add = new_md
	End Function
	
	'���b�Z�[�W�f�[�^�̑���
	Public Function Count() As Short
		Count = colMessageDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colMessageDataList.Remove(Index)
	End Sub
	
	'���b�Z�[�W�f�[�^�̌���
	Public Function Item(ByRef Index As Object) As MessageData
		On Error GoTo ErrorHandler
		
		Item = colMessageDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'���b�Z�[�W�f�[�^���o�^����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim md As MessageData
		
		On Error GoTo ErrorHandler
		md = colMessageDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'���b�Z�[�W�f�[�^���t�@�C�����烍�[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim md As MessageData
		Dim is_effect As Boolean
		Dim sname, msg As String
		Dim data_name As String
		Dim err_msg As String
		
		'������ʃf�[�^or�퓬�A�j���f�[�^���H
		If InStr(LCase(fname), "effect.txt") > 0 Or InStr(LCase(fname), "animation.txt") > 0 Then
			is_effect = True
		End If
		
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
			
			'�d�����Ē�`���ꂽ�f�[�^�̏ꍇ
			If IsDefined(data_name) Then
				If Not is_effect Then
					'�p�C���b�g���b�Z�[�W�̏ꍇ�͌ォ���`���ꂽ���̂�D��
					Delete(data_name)
					md = Add(data_name)
				Else
					'������ʃf�[�^�̏ꍇ�͊����̂��̂ɒǉ�
					md = Item(data_name)
				End If
			Else
				md = Add(data_name)
			End If
			
			With md
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					ret = InStr(line_buf, ",")
					
					If ret < 2 Then
						Error(0)
					End If
					
					sname = Left(line_buf, ret - 1)
					msg = Trim(Mid(line_buf, ret + 1))
					
					If Len(sname) = 0 Then
						err_msg = "�V�`���G�[�V�����̎w�肪�����Ă��܂��B"
						Error(0)
					End If
					
					.AddMessage(sname, msg)
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
		Loop 
		
ErrorHandler: 
		'�G���[����
		If line_num = 0 Then
			ErrorMessage(fname & "���J���܂���B")
		Else
			FileClose(FileNumber)
			DataErrorMessage("", fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
	
	'�f�t�H���g�̐퓬�A�j���f�[�^���`
	Public Sub AddDefaultAnimation()
		Dim md As MessageData
		
		'�A�j���f�[�^���p�ӂ���Ă��Ȃ��ꍇ�� Data\System\animation.txt ��ǂݍ���
		If Count() = 0 Then
			If FileExists(AppPath & "Data\System\animation.txt") Then
				Load(AppPath & "Data\System\animation.txt")
			End If
		End If
		
		If IsDefined("�ėp") Then
			md = Item("�ėp")
		Else
			md = Add("�ėp")
		End If
		
		With md
			If FindNormalLabel("�퓬�A�j��_��𔭓�") > 0 Then
				If .SelectMessage("���") = "" Then
					.AddMessage("���", "���")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�؂蕥������") > 0 Then
				If .SelectMessage("�؂蕥��") = "" Then
					.AddMessage("�؂蕥��", "�؂蕥��")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�}������") > 0 Then
				If .SelectMessage("�}��") = "" Then
					.AddMessage("�}��", "�}��")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�_�~�[����") > 0 Then
				If .SelectMessage("�_�~�[") = "" Then
					.AddMessage("�_�~�[", "�_�~�[")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�C�����u����") > 0 Then
				If .SelectMessage("�C��") = "" Then
					.AddMessage("�C��", "�C�����u")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�⋋���u����") > 0 Then
				If .SelectMessage("�⋋") = "" Then
					.AddMessage("�⋋", "�⋋���u")
				End If
			End If
			If FindNormalLabel("�퓬�A�j��_�I������") > 0 Then
				If .SelectMessage("�I��") = "" Then
					.AddMessage("�I��", "�I��")
				End If
			End If
		End With
	End Sub
End Class