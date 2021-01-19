Option Strict Off
Option Explicit On
Friend Class PilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�p�C���b�g�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'�p�C���b�g�f�[�^�̃R���N�V����
	Private colPilotDataList As New Collection
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		Dim pd As New PilotData
		
		'���j�b�g�X�e�[�^�X�R�}���h�̖��l���j�b�g�p
		With pd
			.Name = "�X�e�[�^�X�\���p�_�~�[�p�C���b�g(�U�R)"
			.Nickname = "�p�C���b�g�s��"
			.Adaption = "AAAA"
			.Bitmap = ".bmp"
		End With
		colPilotDataList.Add(pd, pd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colPilotDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�p�C���b�g�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef pname As String) As PilotData
		Dim new_pilot_data As New PilotData
		
		new_pilot_data.Name = pname
		colPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'�p�C���b�g�f�[�^���X�g�ɓo�^����Ă���f�[�^�̑���
	Public Function Count() As Short
		Count = colPilotDataList.Count()
	End Function
	
	'�p�C���b�g�f�[�^���X�g����w�肵���f�[�^������
	Public Sub Delete(ByRef Index As Object)
		colPilotDataList.Remove(Index)
	End Sub
	
	'�p�C���b�g�f�[�^���X�g����w�肵���f�[�^�����o��
	Public Function Item(ByRef Index As Object) As PilotData
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'�p�C���b�g�f�[�^���X�g�Ɏw�肵���f�[�^���o�^����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'�p�C���b�g�f�[�^���X�g�Ɏw�肵���f�[�^���o�^����Ă��邩�H (���̂͌��Ȃ�)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim i, j As Short
		Dim ret, n, ret2 As Integer
		Dim buf, line_buf, buf2 As String
		Dim pd As PilotData
		Dim data_name As String
		Dim err_msg As String
		Dim aname, adata As String
		Dim alevel As Double
		Dim wd As WeaponData
		Dim sd As AbilityData
		Dim wname, sname As String
		Dim sp_cost As Short
		Dim in_quote As Boolean
		Dim comma_num As Short
		
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
			
			'����
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "���̂ɔ��p�X�y�[�X�͎g�p�o���܂���B"
				Error(0)
			End If
			If InStr(data_name, "�i") > 0 Or InStr(data_name, "�j") > 0 Then
				err_msg = "���̂ɑS�p���ʂ͎g�p�o���܂���"
				Error(0)
			End If
			If InStr(data_name, """") > 0 Then
				err_msg = "���̂�""�͎g�p�o���܂���B"
				Error(0)
			End If
			
			If IsDefined(data_name) Then
				'���łɒ�`�ς݂̃p�C���b�g�̏ꍇ�̓f�[�^��u������
				If Item(data_name).Name = data_name Then
					pd = Item(data_name)
					pd.Clear()
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'����, �ǂ݉���, ����, �N���X, �n�`�K��, �o���l
				GetLine(FileNumber, line_buf, line_num)
				
				'�����`�F�b�N�̂��߁A�R���}�̐��𐔂��Ă���
				comma_num = 0
				For i = 1 To Len(line_buf)
					If Mid(line_buf, i, 1) = "," Then
						comma_num = comma_num + 1
					End If
				Next 
				
				If comma_num < 3 Then
					err_msg = "�ݒ�ɔ���������܂��B"
					Error(0)
				ElseIf comma_num > 5 Then 
					err_msg = "�]���ȁu,�v������܂��B"
					Error(0)
				End If
				
				'����
				ret = InStr(line_buf, ",")
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Len(buf2) = 0 Then
					err_msg = "���̂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				.Nickname = buf2
				
				Select Case comma_num
					Case 4
						'�ǂ݉��� or ����
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "�j��", "����", "-"
								.KanaName = StrToHiragana(.Nickname)
								.Sex = buf2
							Case Else
								.KanaName = buf2
						End Select
					Case 5
						'�ǂ݉���
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "�j��", "����", "-"
								DataErrorMessage("�ǂ݉����̐ݒ肪�����Ă��܂��B", fname, line_num, line_buf, data_name)
								.KanaName = StrToHiragana(.Nickname)
							Case Else
								.KanaName = buf2
						End Select
						
						'����
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "�j��", "����", "-"
								.Sex = buf2
							Case Else
								DataErrorMessage("���ʂ̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						End Select
					Case Else
						.KanaName = StrToHiragana(.Nickname)
				End Select
				
				'�N���X
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Class_Renamed = buf2
				Else
					DataErrorMessage("�N���X�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�n�`�K��
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Len(buf2) = 4 Then
					.Adaption = buf2
				Else
					DataErrorMessage("�n�`�K���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					.Adaption = "AAAA"
				End If
				
				'�o���l
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.ExpValue = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�o���l�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'����\�̓f�[�^
				GetLine(FileNumber, line_buf, line_num)
				If line_buf = "����\�͂Ȃ�" Then
					GetLine(FileNumber, line_buf, line_num)
				ElseIf line_buf = "����\��" Then 
					'�V�`���ɂ�����\�͕\�L
					GetLine(FileNumber, line_buf, line_num)
					buf = line_buf
					
					i = 0
					aname = ""
					Do While True
						i = i + 1
						
						'�R���}�̈ʒu������
						ret = InStr(buf, ",")
						'�u"�v���g���Ă��邩����
						ret2 = InStr(buf, """")
						
						If ret2 < ret And ret2 > 0 Then
							'�u"�v�����������ꍇ�A���́u"�v��̃R���}������
							in_quote = True
							j = ret2 + 1
							Do While j <= Len(buf)
								Select Case Mid(buf, j, 1)
									Case """"
										in_quote = Not in_quote
									Case ","
										If Not in_quote Then
											buf2 = Left(buf, j - 1)
											buf = Mid(buf, j + 1)
										End If
								End Select
								j = j + 1
							Loop 
							If j = Len(buf) Then
								buf2 = buf
								buf = ""
							End If
							in_quote = False
						ElseIf ret > 0 Then 
							'�R���}������������R���}�܂ł̕������؂�o��
							buf2 = Trim(Left(buf, ret - 1))
							buf = Trim(Mid(buf, ret + 1))
							
							'�R���}�̌��̕����񂪋󔒂̏ꍇ
							If buf = "" Then
								If i Mod 2 = 1 Then
									err_msg = "�s���́u,�v�̌�ɓ���\�͎w�肪�����Ă��܂��B"
								Else
									err_msg = "�s���́u,�v�̌�ɓ���\�̓��x���w�肪�����Ă��܂��B"
								End If
								Error(0)
							End If
						Else
							'�s���̕�����
							buf2 = buf
							buf = ""
						End If
						
						If i Mod 2 = 1 Then
							'����\�͖������x��
							
							If IsNumeric(buf2) Then
								If i = 1 Then
									'����\�͂̎w��͏I��B�\�͒l�̎w���
									buf = buf2 & ", " & buf
									Exit Do
								Else
									DataErrorMessage("�s������" & VB6.Format((i + 1) \ 2) & "�Ԗڂ̓���\�͖��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								End If
							End If
							
							If InStr(buf2, " ") > 0 Then
								If Left(buf2, 4) <> "���K��" And Left(buf2, 6) <> "�r�o�����" And Left(buf2, 12) <> "�X�y�V�����p���[��������" And Left(buf2, 4) <> "�n���^�[" And InStr(buf2, "=��� ") = 0 Then
									If aname = "" Then
										err_msg = "�s������" & VB6.Format((i + 1) \ 2) & "�Ԗڂ̓���\�́u" & Trim(Left(buf2, InStr(buf2, " "))) & "�v�̎w��̌�Ɂu,�v�������Ă��܂��B"
									Else
										err_msg = "����\�́u" & aname & "�v�̃��x���w��̌�Ɂu,�v�������Ă��܂��B"
									End If
									Error(0)
								End If
							End If
							
							'����\�͂̕ʖ��w�肪����H
							j = InStr(buf2, "=")
							If j > 0 Then
								adata = Mid(buf2, j + 1)
								buf2 = Left(buf2, j - 1)
							Else
								adata = ""
							End If
							
							'����\�͂̃��x���w���؂�o��
							j = InStr(buf2, "Lv")
							Select Case j
								Case 0
									'�w��Ȃ�
									aname = buf2
									alevel = DEFAULT_LEVEL
								Case 1
									'���x���w��݂̂���
									If Not IsNumeric(Mid(buf2, j + 2)) Then
										DataErrorMessage("����\�́u" & aname & "�v�̃��x���w�肪�s���ł��B", fname, line_num, line_buf, data_name)
									End If
									
									alevel = CShort(Mid(buf2, j + 2))
									If aname = "" Then
										DataErrorMessage("�s������" & VB6.Format((i + 1) \ 2) & "�Ԗڂ̓���\�͖��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
									End If
								Case Else
									'����\�͖��ƃ��x���̗������w�肳��Ă���
									aname = Left(buf2, j - 1)
									alevel = CDbl(Mid(buf2, j + 2))
							End Select
						Else
							'����\�͏C�����x��
							If IsNumeric(buf2) Then
								.AddSkill(aname, alevel, adata, CShort(buf2))
							Else
								If alevel > 0 Then
									DataErrorMessage("����\�́u" & aname & "Lv" & VB6.Format(alevel) & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("����\�́u" & aname & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								End If
								.AddSkill(aname, alevel, adata, 1)
							End If
						End If
						
						If buf = "" Then
							'�����ł��̍s�͏I��
							
							'i����̏ꍇ�͏C�����x���������Ă���
							If i Mod 2 = 1 Then
								If alevel > 0 Then
									DataErrorMessage("����\�́u" & aname & "Lv" & VB6.Format(alevel) & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("����\�́u" & aname & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								End If
							End If
							
							GetLine(FileNumber, line_buf, line_num)
							buf = line_buf
							
							i = 0
							aname = ""
						End If
					Loop 
				ElseIf InStr(line_buf, "����\��,") = 1 Then 
					'���`���ɂ�����\�͕\�L
					buf = Mid(line_buf, 6)
					
					i = 0
					aname = ""
					Do 
						i = i + 1
						
						'�R���}�̈ʒu������
						ret = InStr(buf, ",")
						'�u"�v���g���Ă��邩����
						ret2 = InStr(buf, """")
						
						If ret2 < ret And ret2 > 0 Then
							'�u"�v�����������ꍇ�A���́u"�v��̃R���}������
							in_quote = True
							j = ret2 + 1
							Do While j <= Len(buf)
								Select Case Mid(buf, j, 1)
									Case """"
										in_quote = Not in_quote
									Case ","
										If Not in_quote Then
											buf2 = Left(buf, j - 1)
											buf = Mid(buf, j + 1)
										End If
								End Select
								j = j + 1
							Loop 
							If j = Len(buf) Then
								buf2 = buf
								buf = ""
							End If
							in_quote = False
						ElseIf ret > 0 Then 
							'�R���}������������R���}�܂ł̕������؂�o��
							buf2 = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
							
							'�R���}�̌��̕����񂪋󔒂̏ꍇ
							If buf = "" Then
								If i Mod 2 = 1 Then
									err_msg = "�s���́u,�v�̌�ɓ���\�͎w�肪�����Ă��܂��B"
								Else
									err_msg = "�s���́u,�v�̌�ɓ���\�̓��x���w�肪�����Ă��܂��B"
								End If
								Error(0)
							End If
						Else
							'�s���̕�����
							buf2 = buf
							buf = ""
						End If
						
						If i Mod 2 = 1 Then
							'����\�͖������x��
							
							If InStr(buf2, " ") > 0 Then
								If aname = "" Then
									err_msg = "�s������" & VB6.Format((i + 1) \ 2) & "�Ԗڂ̓���\�͂̎w��̌�Ɂu,�v�������Ă��܂��B"
								Else
									err_msg = "����\�́u" & aname & "�v�̃��x���w��̌�Ɂu,�v�������Ă��܂��B"
								End If
								Error(0)
							End If
							
							'����\�͂̕ʖ��w�肪����H
							j = InStr(buf2, "=")
							If j > 0 Then
								adata = Mid(buf2, j + 1)
								buf2 = Left(buf2, j - 1)
							Else
								adata = ""
							End If
							
							'����\�͂̃��x���w���؂�o��
							j = InStr(buf2, "Lv")
							Select Case j
								Case 0
									'�w��Ȃ�
									aname = buf2
									alevel = DEFAULT_LEVEL
								Case 1
									'���x���w��݂̂���
									If Not IsNumeric(Mid(buf2, j + 2)) Then
										err_msg = "����\�́u" & aname & "�v�̃��x���w�肪�s���ł�"
										Error(0)
									End If
									
									alevel = CDbl(Mid(buf2, j + 2))
									If aname = "" Then
										err_msg = "�s������" & VB6.Format((i + 1) \ 2) & "�Ԗڂ̓���\�̖͂��O�u" & buf2 & "�v���s���ł�"
										Error(0)
									End If
								Case Else
									'����\�͖��ƃ��x���̗������w�肳��Ă���
									aname = Left(buf2, j - 1)
									alevel = CDbl(Mid(buf2, j + 2))
							End Select
						Else
							'����\�͏C�����x��
							If IsNumeric(buf2) Then
								.AddSkill(aname, alevel, adata, CShort(buf2))
							Else
								If alevel > 0 Then
									DataErrorMessage("����\�́u" & aname & "Lv" & VB6.Format(alevel) & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("����\�́u" & aname & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								End If
								.AddSkill(aname, alevel, adata, 1)
							End If
						End If
					Loop While ret > 0
					
					'i����̏ꍇ�͏C�����x���������Ă���
					If i Mod 2 = 1 Then
						If alevel > 0 Then
							DataErrorMessage("����\�́u" & aname & "Lv" & VB6.Format(alevel) & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						Else
							DataErrorMessage("����\�́u" & aname & "�v�̏C�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						End If
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Else
					err_msg = "����\�͂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				
				'�i��
				If Len(line_buf) = 0 Then
					err_msg = "�i���U���͂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�ˌ��U���͂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If IsNumeric(buf2) Then
					.Infight = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�i���U���͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�ˌ�
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Shooting = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�ˌ��U���͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'����
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Hit = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'���
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�Z�ʂ̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Dodge = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�Z��
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Technique = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�Z�ʂ̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'����
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "���i�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Intuition = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'���i
				buf2 = Trim(buf)
				If Len(buf2) = 0 Then
					err_msg = "���i�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				If InStr(buf2, ",") > 0 Then
					DataErrorMessage("�s���ɗ]���ȃR���}���t�����Ă��܂��B", fname, line_num, line_buf, data_name)
					buf2 = Trim(Left(buf2, InStr(buf2, ",") - 1))
				End If
				If Not IsNumeric(buf2) Then
					.Personality = buf2
				Else
					DataErrorMessage("���i�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�X�y�V�����p���[
				GetLine(FileNumber, line_buf, line_num)
				Select Case line_buf
					Case "�r�o�Ȃ�", "���_�Ȃ�"
						'�X�y�V�����p���[�������Ă��Ȃ�
					Case ""
						err_msg = "�X�y�V�����p���[�̐ݒ肪�����Ă��܂��B"
						Error(0)
					Case Else
						ret = InStr(line_buf, ",")
						If ret = 0 Then
							err_msg = "�r�o�l�̐ݒ肪�����Ă��܂��B"
							Error(0)
						End If
						buf2 = Trim(Left(line_buf, ret - 1))
						buf = Mid(line_buf, ret + 1)
						If buf2 <> "�r�o" And buf2 <> "���_" Then
							err_msg = "�X�y�V�����p���[�̐ݒ肪�����Ă��܂��B"
							Error(0)
						End If
						
						'�r�o�l
						ret = InStr(buf, ",")
						If ret > 0 Then
							buf2 = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
						Else
							buf2 = Trim(buf)
							buf = ""
						End If
						If IsNumeric(buf2) Then
							.SP = MinLng(CInt(buf2), 9999)
						Else
							DataErrorMessage("�r�o�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							.SP = 1
						End If
						
						'�X�y�V�����p���[�Ɗl�����x��
						ret = InStr(buf, ",")
						Do While ret > 0
							sname = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
							
							'�r�o�����
							If InStr(sname, "=") > 0 Then
								sp_cost = StrToLng(Mid(sname, InStr(sname, "=") + 1))
								sname = Left(sname, InStr(sname, "=") - 1)
							Else
								sp_cost = 0
							End If
							
							ret = InStr(buf, ",")
							If ret = 0 Then
								buf2 = Trim(buf)
								buf = ""
							Else
								buf2 = Trim(Left(buf, ret - 1))
								buf = Mid(buf, ret + 1)
							End If
							
							If sname = "" Then
								DataErrorMessage("�X�y�V�����p���[�̎w�肪�����Ă��܂��B", fname, line_num, line_buf, data_name)
							ElseIf Not SPDList.IsDefined(sname) Then 
								DataErrorMessage(sname & "�Ƃ����X�y�V�����p���[�͑��݂��܂���B", fname, line_num, line_buf, data_name)
							ElseIf Not IsNumeric(buf2) Then 
								DataErrorMessage("�X�y�V�����p���[�u" & sname & "�v�̊l�����x�����Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
								.AddSpecialPower(sname, 1, sp_cost)
							Else
								.AddSpecialPower(sname, CShort(buf2), sp_cost)
							End If
							
							ret = InStr(buf, ",")
						Loop 
						
						If buf <> "" Then
							DataErrorMessage("�X�y�V�����p���[�u" & Trim(sname) & "�v�̊l�����x���w�肪�����Ă��܂��B", fname, line_num, line_buf, data_name)
						End If
				End Select
				
				'�r�b�g�}�b�v, �l�h�c�h
				GetLine(FileNumber, line_buf, line_num)
				
				'�r�b�g�}�b�v
				If Len(line_buf) = 0 Then
					err_msg = "�r�b�g�}�b�v�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�l�h�c�h�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("�r�b�g�}�b�v�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					.IsBitmapMissing = True
				End If
				
				'�l�h�c�h
				buf = Trim(buf)
				buf2 = buf
				Do While Right(buf2, 1) = ")"
					buf2 = Left(buf2, Len(buf2) - 1)
				Loop 
				Select Case LCase(Right(buf2, 4))
					Case ".mid", ".mp3", ".wav", "-"
						.BGM = buf
					Case ""
						DataErrorMessage("�l�h�c�h�̐ݒ肪�����Ă��܂��B", fname, line_num, line_buf, data_name)
						.Bitmap = "-.mid"
					Case Else
						DataErrorMessage("�l�h�c�h�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						.Bitmap = "-.mid"
				End Select
				
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				
				GetLine(FileNumber, line_buf, line_num)
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'����\�̓f�[�^
				GetLine(FileNumber, line_buf, line_num)
				
				buf = line_buf
				i = 0
				Do While line_buf <> "==="
					i = i + 1
					
					ret = 0
					in_quote = False
					For j = 1 To Len(buf)
						Select Case Mid(buf, j, 1)
							Case ","
								If Not in_quote Then
									ret = j
									Exit For
								End If
							Case """"
								in_quote = Not in_quote
						End Select
					Next 
					
					If ret > 0 Then
						buf2 = Trim(Left(buf, ret - 1))
						buf = Trim(Mid(buf, ret + 1))
					Else
						buf2 = buf
						buf = ""
					End If
					
					If buf2 = "" Or IsNumeric(buf2) Then
						DataErrorMessage("�s������" & VB6.Format(i) & "�Ԗڂ̓���\�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					Else
						.AddFeature(buf2)
					End If
					
					If buf = "" Then
						If EOF(FileNumber) Then
							FileClose(FileNumber)
							Exit Sub
						End If
						
						GetLine(FileNumber, line_buf, line_num)
						buf = line_buf
						i = 0
						
						If line_buf = "" Or line_buf = "===" Then
							Exit Do
						End If
					End If
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'����f�[�^
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0 And line_buf <> "==="
					'���햼
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "����f�[�^�̏I��ɂ͋�s��u���Ă��������B"
						Error(0)
					End If
					wname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If wname = "" Then
						err_msg = "���햼�̐ݒ肪�Ԉ���Ă��܂��B"
						Error(0)
					End If
					
					'�����o�^
					wd = .AddWeapon(wname)
					
					'�U����
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̍ŏ��˒��������Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.Power = MinLng(CInt(buf2), 99999)
					ElseIf buf = "-" Then 
						wd.Power = 0
					Else
						DataErrorMessage(wname & "�̍U���͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Power = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'�ŏ��˒�
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̍ő�˒��̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MinRange = CShort(buf2)
					Else
						DataErrorMessage(wname & "�̍ŏ��˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						wd.MinRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MinRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'�ő�˒�
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̖������̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MaxRange = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage(wname & "�̍ő�˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						wd.MaxRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'������
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̒e���̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Precision = n
					Else
						DataErrorMessage(wname & "�̖������̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Precision = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'�e��
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̏���d�m�̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.Bullet = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(wname & "�̒e���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.Bullet = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'����d�m
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̕K�v�C�͂������Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(wname & "�̏���d�m�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'�K�v�C��
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̒n�`�K���������Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							wd.NecessaryMorale = n
						Else
							DataErrorMessage(wname & "�̕K�v�C�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'�n�`�K��
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̃N���e�B�J�����������Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If Len(buf2) = 4 Then
						wd.Adaption = buf2
					Else
						DataErrorMessage(wname & "�̒n�`�K���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						wd.Adaption = "----"
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Adaption = LIndex(buf2, 1)
						End If
					End If
					
					'�N���e�B�J����
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "�̕��푮���������Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Critical = n
					Else
						DataErrorMessage(wname & "�̃N���e�B�J�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Critical = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'���푮��
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(wname & "�̕��푮���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'�K�v�Z�\
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								wd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(wd.NecessarySkill, "(")
								wd.NecessarySkill = Mid(wd.NecessarySkill, ret + 1, Len(wd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								wd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'�K�v����
						ret = InStr(buf, "<")
						If ret > 0 Then
							wd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					wd.Class_Renamed = buf
					If wd.Class_Renamed = "-" Then
						wd.Class_Renamed = ""
					End If
					If InStr(wd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(wname & "�̑����̃��x���w�肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'�A�r���e�B�f�[�^
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					'�A�r���e�B��
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "�A�r���e�B�f�[�^�̏I��ɂ͋�s��u���Ă��������B"
						Error(0)
					End If
					sname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If sname = "" Then
						err_msg = "�A�r���e�B���̐ݒ肪�Ԉ���Ă��܂��B"
						Error(0)
					End If
					
					'�A�r���e�B��o�^
					sd = .AddAbility(sname)
					
					'����
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "�̎˒��̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					sd.SetEffect(buf2)
					
					'�˒�
					sd.MinRange = 0
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "�̉񐔂̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						sd.MaxRange = MinLng(CInt(buf2), 99)
					ElseIf buf2 = "-" Then 
						sd.MaxRange = 0
					Else
						DataErrorMessage(sname & "�̎˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							sd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'��
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "�̏���d�m�̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.Stock = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(sname & "�̉񐔂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.Stock = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'����d�m
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "�̕K�v�C�͂̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(sname & "�̏���d�m�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'�K�v�C��
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "�̃A�r���e�B�����̐ݒ肪�����Ă��܂��B"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							sd.NecessaryMorale = n
						Else
							DataErrorMessage(sname & "�̕K�v�C�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'�A�r���e�B����
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(sname & "�̃A�r���e�B�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'�K�v�Z�\
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								sd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(sd.NecessarySkill, "(")
								sd.NecessarySkill = Mid(sd.NecessarySkill, ret + 1, Len(sd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								sd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'�K�v����
						ret = InStr(buf, "<")
						If ret > 0 Then
							sd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					sd.Class_Renamed = buf
					If sd.Class_Renamed = "-" Then
						sd.Class_Renamed = ""
					End If
					If InStr(sd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(sname & "�̑����̃��x���w�肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
SkipRest: 
		Loop 
		
ErrorHandler: 
		'�G���[����
		If line_num = 0 Then
			ErrorMessage(fname & "���J���܂���")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class