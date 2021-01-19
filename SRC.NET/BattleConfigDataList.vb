Option Strict Off
Option Explicit On
Friend Class BattleConfigDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�o�g���R���t�B�O�f�[�^���Ǘ�����N���X
	' --- �_���[�W�v�Z�A�������Z�o�ȂǁA�o�g���Ɋ֘A����G���A�X�̒�`��ݒ肵�܂��B
	
	'�o�g���R���t�B�O�f�[�^�̃R���N�V����
	Private colBattleConfigData As New Collection
	
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colBattleConfigData
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colBattleConfigData ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colBattleConfigData = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�o�g���R���t�B�O�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef cname As String) As BattleConfigData
		Dim cd As New BattleConfigData
		
		cd.Name = cname
		colBattleConfigData.Add(cd, cname)
		Add = cd
	End Function
	
	'�o�g���R���t�B�O�f�[�^���X�g�ɓo�^����Ă���f�[�^�̑���
	Public Function Count() As Short
		Count = colBattleConfigData.Count()
	End Function
	
	'�o�g���R���t�B�O�f�[�^���X�g����f�[�^���폜
	Public Sub Delete(ByRef Index As Object)
		colBattleConfigData.Remove(Index)
	End Sub
	
	'�o�g���R���t�B�O�f�[�^���X�g����f�[�^�����o��
	Public Function Item(ByRef Index As String) As BattleConfigData
		Item = colBattleConfigData.Item(Index)
	End Function
	
	'�o�g���R���t�B�O�f�[�^���X�g�Ɏw�肵���f�[�^����`����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim cd As BattleConfigData
		
		On Error GoTo ErrorHandler
		cd = colBattleConfigData.Item(Index)
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
		Dim cd As BattleConfigData
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
			
			data_name = line_buf
			
			If IsDefined(data_name) Then
				'���łɒ�`����Ă���G���A�X�̃f�[�^�ł���Βu��������
				Delete(data_name)
			End If
			cd = Add(data_name)
			
			With cd
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					If Len(line_buf) = 0 Then
						Exit Do
					End If
					
					.ConfigCalc = line_buf
				Loop 
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