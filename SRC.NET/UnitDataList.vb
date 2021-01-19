Option Strict Off
Option Explicit On
Friend Class UnitDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S���j�b�g�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'���j�b�g�f�[�^�p�R���N�V����
	Private colUnitDataList As New Collection
	
	'ID�쐬�p�ϐ�
	Private IDNum As Integer
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		Dim ud As New UnitData
		
		With ud
			.Name = "�X�e�[�^�X�\���p�_�~�[���j�b�g"
			.Nickname = "���j�b�g����"
			.PilotNum = 1
			.Transportation = "��"
			.Adaption = "AAAA"
			.Bitmap = ".bmp"
			.AddFeature("�_�~�[���j�b�g=�V�X�e���p��\���\��")
		End With
		colUnitDataList.Add(ud, ud.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colUnitDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colUnitDataList ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colUnitDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'���j�b�g�f�[�^���X�g�Ƀf�[�^��ǉ�
	Public Function Add(ByRef uname As String) As UnitData
		Dim ud As New UnitData
		
		With ud
			.Name = uname
			colUnitDataList.Add(ud, uname)
			IDNum = IDNum + 1
			.ID = IDNum
		End With
		
		Add = ud
	End Function
	
	'���j�b�g�f�[�^���X�g�ɓo�^����Ă���f�[�^��
	Public Function Count() As Short
		Count = colUnitDataList.Count()
	End Function
	
	'���j�b�g�f�[�^���X�g����f�[�^���폜
	Public Sub Delete(ByRef Index As Object)
		colUnitDataList.Remove(Index)
	End Sub
	
	'���j�b�g�f�[�^���X�g����f�[�^�����o��
	Public Function Item(ByRef Index As Object) As UnitData
		On Error GoTo ErrorHandler
		
		Item = colUnitDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item = Nothing
	End Function
	
	'���j�b�g�f�[�^���X�g�ɓo�^����Ă��邩�H
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As UnitData
		
		On Error GoTo ErrorHandler
		dummy = colUnitDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim j, i, k As Short
		Dim n, line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim ret As Short
		Dim ud As UnitData
		Dim wd As WeaponData
		Dim sd As AbilityData
		Dim wname, sname As String
		Dim data_name As String
		Dim err_msg As String
		Dim in_quote As Boolean
		Dim comma_num As Short
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			data_name = ""
			
			'��s���X�L�b�v
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
				ud = Item(data_name)
				ud.Clear()
			Else
				ud = Add(data_name)
			End If
			
			With ud
				'�ǂ݉���
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "�ǂ݉����̌�ɗ]���ȃf�[�^���w�肳��Ă��܂��B"
					Error(0)
				End If
				.KanaName = buf
				
				'����, �ǂ݉���, ���j�b�g�N���X, �p�C���b�g��, �A�C�e����
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
				ElseIf comma_num > 4 Then 
					err_msg = "�]���ȁu,�v������܂��B"
					Error(0)
				End If
				
				'����
				If Len(line_buf) = 0 Then
					err_msg = "���̂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.Nickname = buf2
				
				'�ǂ݉���
				If comma_num = 4 Then
					ret = InStr(buf, ",")
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					.KanaName = buf2
				Else
					.KanaName = StrToHiragana(.Nickname)
				End If
				
				'���j�b�g�N���X
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Class_Renamed = buf2
				Else
					DataErrorMessage("���j�b�g�N���X�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.Class_Renamed = "�ėp"
				End If
				
				'�p�C���b�g��
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Left(buf2, 1) <> "(" Then
					If IsNumeric(buf2) Then
						.PilotNum = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage("�p�C���b�g���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					If .PilotNum < 1 Then
						DataErrorMessage("�p�C���b�g���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						.PilotNum = 1
					End If
				Else
					If Right(buf2, 1) <> ")" Then
						Error(0)
					End If
					buf2 = Mid(buf2, 2, Len(buf2) - 2)
					If IsNumeric(buf2) Then
						.PilotNum = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage("�p�C���b�g���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					If .PilotNum < 1 Then
						DataErrorMessage("�p�C���b�g���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					.PilotNum = -.PilotNum
				End If
				
				'�A�C�e����
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "�A�C�e�����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				If IsNumeric(buf) Then
					.ItemNum = MinLng(CInt(buf), 99)
				Else
					DataErrorMessage("�A�C�e�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.ItemNum = 4
				End If
				
				'�ړ��\�n�`, �ړ���, �T�C�Y, �C����, �o���l
				GetLine(FileNumber, line_buf, line_num)
				
				'�ړ��\�n�`
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�ړ��͂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Transportation = buf2
				Else
					DataErrorMessage("�ړ��\�n�`�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.Transportation = "��"
				End If
				
				'�ړ���
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�T�C�Y�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Speed = MinLng(CInt(buf2), 99)
				Else
					DataErrorMessage("�ړ��͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
				End If
				
				'�T�C�Y
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�o���l�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				Select Case buf2
					Case "XL", "LL", "L", "M", "S", "SS"
						.Size = buf2
					Case Else
						DataErrorMessage("�T�C�Y�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						.Size = "M"
				End Select
				
				'�C����
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�o���l�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Value = MinLng(CInt(buf2), 9999999)
				Else
					DataErrorMessage("�C����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
				End If
				
				'�o���l
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "�o���l�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				If IsNumeric(buf) Then
					.ExpValue = MinLng(CInt(buf), 9999)
				Else
					DataErrorMessage("�o���l�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
					Do While True
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
							
							If j = 1 Then
								If IsNumeric(buf2) Then
									Exit Do
								End If
							End If
							
							buf = Trim(Mid(buf, ret + 1))
						Else
							buf2 = buf
							buf = ""
						End If
						
						If IsNumeric(buf2) Then
							Exit Do
						ElseIf buf2 = "" Or IsNumeric(buf2) Then 
							DataErrorMessage("�s������" & VB6.Format(i) & "�Ԗڂ̓���\�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						Else
							.AddFeature(buf2)
						End If
						
						If buf = "" Then
							GetLine(FileNumber, line_buf, line_num)
							buf = line_buf
							i = 0
						End If
					Loop 
				ElseIf InStr(line_buf, "����\��,") = 1 Then 
					'���`���ɂ�����\�͕\�L
					buf = Mid(line_buf, 6)
					
					ret = 0
					in_quote = False
					For k = 1 To Len(buf)
						Select Case Mid(buf, k, 1)
							Case ","
								If Not in_quote Then
									ret = k
									Exit For
								End If
							Case """"
								in_quote = Not in_quote
						End Select
					Next 
					
					i = 0
					Do While ret > 0
						i = i + 1
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						ret = InStr(buf, ",")
						If buf2 <> "" Then
							.AddFeature(buf2)
						Else
							DataErrorMessage(VB6.Format(i) & "�Ԗڂ̓���\�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						End If
					Loop 
					
					i = i + 1
					buf2 = Trim(buf)
					If buf2 <> "" Then
						.AddFeature(buf2)
					Else
						DataErrorMessage(VB6.Format(i) & "�Ԗڂ̓���\�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Else
					err_msg = "����\�͂̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				
				'�ő�g�o
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�ő�d�m�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If IsNumeric(buf2) Then
					.HP = MinLng(CInt(buf2), 9999999)
				Else
					DataErrorMessage("�ő�g�o�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.HP = 1000
				End If
				
				'�ő�d�m
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "���b�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.EN = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�ő�d�m�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.EN = 100
				End If
				
				'���b
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�^�����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Armor = MinLng(CInt(buf2), 99999)
				Else
					DataErrorMessage("���b�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
				End If
				
				'�^����
				buf2 = Trim(buf)
				If Len(buf2) = 0 Then
					err_msg = "�^�����̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				If IsNumeric(buf2) Then
					.Mobility = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("�^�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
				End If
				
				'�n�`�K��, �r�b�g�}�b�v
				GetLine(FileNumber, line_buf, line_num)
				
				'�n�`�K��
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�r�b�g�}�b�v�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Len(buf2) = 4 Then
					.Adaption = buf2
				Else
					DataErrorMessage("�n�`�K���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.Adaption = "AAAA"
				End If
				
				'�r�b�g�}�b�v
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "�r�b�g�}�b�v�̐ݒ肪�����Ă��܂��B"
					Error(0)
				End If
				If LCase(Right(buf, 4)) = ".bmp" Then
					.Bitmap = buf
				Else
					DataErrorMessage("�r�b�g�}�b�v�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
					.IsBitmapMissing = True
				End If
				
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
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
						DataErrorMessage(wname & "�̍U���͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̍ŏ��˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̍ő�˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̖������̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(wname & "�̒e���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(wname & "�̏���d�m�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(wname & "�̕K�v�C�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̒n�`�K���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̃N���e�B�J�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Critical = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'���푮��
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(wname & "�̕��푮���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(wname & "�̑����̃��x���w�肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						err_msg = "�A�r���e�B�f�[�^�̏I��ɋ�s��u���Ă��������B"
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
						DataErrorMessage(sname & "�̎˒��̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(sname & "�̉񐔂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(sname & "�̏���d�m�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
							DataErrorMessage(sname & "�̕K�v�C�͂̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'�A�r���e�B����
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(sname & "�̃A�r���e�B�����̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
						DataErrorMessage(sname & "�̑����̃��x���w�肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, .Name)
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
			ErrorMessage(fname & "���J���܂���B")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class