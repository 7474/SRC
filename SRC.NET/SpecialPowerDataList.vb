Option Strict Off
Option Explicit On
Friend Class SpecialPowerDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�X�y�V�����p���[�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'�X�y�V�����p���[�f�[�^�̃R���N�V����
	Private colSpecialPowerDataList As New Collection
	
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colSpecialPowerDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colSpecialPowerDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colSpecialPowerDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�X�y�V�����p���[�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef sname As String) As SpecialPowerData
		Dim new_data As New SpecialPowerData
		
		new_data.Name = sname
		colSpecialPowerDataList.Add(new_data, sname)
		Add = new_data
	End Function
	
	'�X�y�V�����p���[�f�[�^���X�g�ɓo�^����Ă���f�[�^�̑���
	Public Function Count() As Short
		Count = colSpecialPowerDataList.Count()
	End Function
	
	'�X�y�V�����p���[�f�[�^���X�g����w�肵���f�[�^���폜
	Public Sub Delete(ByRef Index As Object)
		colSpecialPowerDataList.Remove(Index)
	End Sub
	
	'�X�y�V�����p���[�f�[�^���X�g����w�肵���f�[�^�����o��
	Public Function Item(ByRef Index As Object) As SpecialPowerData
		On Error GoTo ErrorHandler
		
		Item = colSpecialPowerDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'�X�y�V�����p���[�f�[�^���X�g�Ɏw�肵���f�[�^���o�^����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As SpecialPowerData
		
		On Error GoTo ErrorHandler
		dummy = colSpecialPowerDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim sd As SpecialPowerData
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
			
			'����
			ret = InStr(line_buf, ",")
			If ret > 0 Then
				data_name = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
			Else
				data_name = line_buf
				buf = ""
			End If
			
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
				Delete(data_name)
			End If
			sd = Add(data_name)
			
			With sd
				'�ǂ݉���
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "�ǂ݉����̌�ɗ]���ȃf�[�^���w�肳��Ă��܂��B"
					Error(0)
				End If
				.KanaName = Trim(buf)
				If .KanaName = "" Then
					.KanaName = StrToHiragana(data_name)
				End If
				
				'�Z�k�`, ����r�o, �Ώ�, ���ʎ���, �K�p����, �g�p����, �A�j��
				GetLine(FileNumber, line_buf, line_num)
				
				'�Z�k�`
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "����r�o�������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.ShortName = buf2
				
				'����r�o
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�Ώۂ������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.SPConsumption = CShort(buf2)
				Else
					DataErrorMessage("����r�o�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�Ώ�
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "���ʎ��Ԃ������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "�Ώۂ��Ԉ���Ă��܂��B"
					Error(0)
				End If
				.TargetType = buf2
				
				'���ʎ���
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "���ʎ��Ԃ������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "���ʎ��Ԃ��Ԉ���Ă��܂��B"
					Error(0)
				End If
				.Duration = buf2
				
				'�K�p����
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�g�p�����������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "�K�p�������Ԉ���Ă��܂��B"
					Error(0)
				End If
				.NecessaryCondition = buf2
				
				'�g�p����, �A�j��
				ret = InStr(buf, ",")
				If ret > 0 Then
					.Animation = Trim(Mid(buf, InStr(buf, ",") + 1))
				Else
					.Animation = .Name
				End If
				
				'����
				GetLine(FileNumber, line_buf, line_num)
				If Len(line_buf) = 0 Then
					err_msg = "���ʂ��w�肳��Ă��܂���B"
					Error(0)
				End If
				.SetEffect(line_buf)
				
				'���
				GetLine(FileNumber, line_buf, line_num)
				If Len(line_buf) = 0 Then
					err_msg = "������w�肳��Ă��܂���B"
					Error(0)
				End If
				.Comment = line_buf
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