Option Strict Off
Option Explicit On
Module InterMission
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�C���^�[�~�b�V�����Ɋւ��鏈�����s�����W���[��
	
	'�C���^�[�~�b�V����
	Public Sub InterMissionCommand(Optional ByVal skip_update As Boolean = False)
		Dim cmd_list() As String
		Dim name_list() As String
		Dim j, i, ret As Short
		Dim buf As String
		Dim u As Unit
		Dim var As VarData
		Dim fname, save_path As String
		
		Stage = "�C���^�[�~�b�V����"
		IsSubStage = False
		
		'�a�f�l��ύX
		KeepBGM = False
		BossBGM = False
		If InStr(BGMFileName, "\" & BGMName("Intermission")) = 0 Then
			StopBGM()
			StartBGM(BGMName("Intermission"))
		End If
		
		'�}�b�v���N���A
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		
		'�e��f�[�^���A�b�v�f�[�g
		If Not skip_update Then
			UList.Update()
			PList.Update()
			IList.Update()
		End If
		ClearEventData()
		ClearMap()
		
		'�I��p�_�C�A���O���g��
		EnlargeListBoxHeight()
		
		Do While True
			'���p�\�ȃC���^�[�~�b�V�����R�}���h��I��
			
			ReDim cmd_list(0)
			ReDim ListItemFlag(0)
			ReDim ListItemID(0)
			cmd_list(0) = "�L�����Z��"
			
			'�u���̃X�e�[�W�ցv�R�}���h
			If GetValueAsString("���X�e�[�W") <> "" Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "���̃X�e�[�W��"
			End If
			
			'�f�[�^�Z�[�u�R�}���h
			If Not IsOptionDefined("�f�[�^�Z�[�u�s��") Or IsOptionDefined("�f�o�b�O") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "�f�[�^�Z�[�u"
			End If
			
			'�@�̉����R�}���h
			If Not IsOptionDefined("�����s��") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				If IsOptionDefined("���g��") Then
					cmd_list(UBound(cmd_list)) = "���j�b�g�̋���"
				Else
					cmd_list(UBound(cmd_list)) = "�@�̉���"
					For	Each u In UList
						With u
							If .Party0 = "����" And .Status_Renamed = "�ҋ@" Then
								If Left(.Class_Renamed, 1) = "(" Then
									cmd_list(UBound(cmd_list)) = "���j�b�g�̋���"
									Exit For
								End If
							End If
						End With
					Next u
				End If
			End If
			
			'��芷���R�}���h
			If IsOptionDefined("��芷��") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "��芷��"
			End If
			
			'�A�C�e�������R�}���h
			If IsOptionDefined("�A�C�e������") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "�A�C�e������"
			End If
			
			'�����R�}���h
			For	Each u In UList
				With u
					If .Party0 = "����" And .Status_Renamed = "�ҋ@" Then
						If .IsFeatureAvailable("����") Then
							For i = 1 To LLength(.FeatureData("����"))
								If .OtherForm(LIndex(.FeatureData("����"), i)).IsAvailable Then
									Exit For
								End If
							Next 
							If i <= LLength(.FeatureData("����")) Then
								ReDim Preserve cmd_list(UBound(cmd_list) + 1)
								ReDim Preserve ListItemFlag(UBound(cmd_list))
								cmd_list(UBound(cmd_list)) = "����"
								Exit For
							End If
						End If
					End If
				End With
			Next u
			
			'�p�C���b�g�X�e�[�^�X�R�}���h
			If Not IsOptionDefined("���g��") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "�p�C���b�g�X�e�[�^�X"
			End If
			
			'���j�b�g�X�e�[�^�X�R�}���h
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "���j�b�g�X�e�[�^�X"
			
			'���[�U�[��`�̃C���^�[�~�b�V�����R�}���h
			For	Each var In GlobalVariableList
				If InStr(var.Name, "IntermissionCommand(") = 1 Then
					ret = Len("IntermissionCommand(")
					buf = Mid(var.Name, ret + 1, Len(var.Name) - ret - 1)
					buf = GetValueAsString(buf)
					FormatMessage(buf)
					ReDim Preserve cmd_list(UBound(cmd_list) + 1)
					ReDim Preserve ListItemFlag(UBound(cmd_list))
					ReDim Preserve ListItemID(UBound(cmd_list))
					cmd_list(UBound(cmd_list)) = buf
					ListItemID(UBound(cmd_list)) = var.Name
				End If
			Next var
			
			'�I���R�}���h
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "SRC���I��"
			
			'�C���^�[�~�b�V�����̃R�}���h���̂ɃG���A�X��K�p
			ReDim name_list(UBound(cmd_list))
			For i = 1 To UBound(name_list)
				name_list(i) = cmd_list(i)
				With ALDList
					For j = 1 To .Count
						With .Item(j)
							If .AliasType(1) = cmd_list(i) Then
								name_list(i) = .Name
								Exit For
							End If
						End With
					Next 
				End With
			Next 
			
			'�v���C���[�ɂ��R�}���h�I��
			TopItem = 1
			ret = ListBox("�C���^�[�~�b�V�����F ���^�[����" & VB6.Format(TotalTurn) & " " & Term("����") & VB6.Format(Money), name_list, "�R�}���h", "�A���\��")
			
			'�I�����ꂽ�C���^�[�~�b�V�����R�}���h�����s
			Select Case cmd_list(ret)
				Case "���̃X�e�[�W��"
					If MsgBox("���̃X�e�[�W�֐i�݂܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "���X�e�[�W") = 1 Then
						
						UList.Update() '�ǉ��p�C���b�g������
						
						frmListBox.Hide()
						ReduceListBoxHeight()
						StopBGM()
						Exit Sub
					End If
					
				Case "�f�[�^�Z�[�u"
					'��U�u��Ɏ�O�ɕ\���v������
					If frmListBox.Visible Then
						ret = SetWindowPos(frmListBox.Handle.ToInt32, -2, 0, 0, 0, 0, &H3)
					End If
					
					fname = SaveFileDialog("�f�[�^�Z�[�u", ScenarioPath, GetValueAsString("�Z�[�u�f�[�^�t�@�C����"), 2, "�����ް�", "src")
					
					'�Ăсu��Ɏ�O�ɕ\���v
					If frmListBox.Visible Then
						ret = SetWindowPos(frmListBox.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
					End If
					
					'�L�����Z���H
					If fname = "" Then
						GoTo NextLoop
					End If
					
					'�Z�[�u��̓V�i���I�t�H���_�H
					If InStr(fname, "\") > 0 Then
						save_path = Left(fname, InStr2(fname, "\"))
					End If
					'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
					If Dir(save_path) <> Dir(ScenarioPath) Then
						If MsgBox("�Z�[�u�t�@�C���̓V�i���I�t�H���_�ɂȂ��Ɠǂݍ��߂܂���B" & vbCr & vbLf & "���̂܂܃Z�[�u���܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question) <> 1 Then
							GoTo NextLoop
						End If
					End If
					
					If fname <> "" Then
						UList.Update() '�ǉ��p�C���b�g������
						SaveData(fname)
					End If
					
				Case "�@�̉���", "���j�b�g�̋���"
					RankUpCommand()
					
				Case "��芷��"
					ExchangeUnitCommand()
					
				Case "�A�C�e������"
					ExchangeItemCommand()
					
				Case "����"
					ExchangeFormCommand()
					
				Case "SRC���I��"
					If MsgBox("SRC���I�����܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "�I��") = 1 Then
						frmListBox.Hide()
						ReduceListBoxHeight()
						ExitGame()
					End If
					
				Case "�p�C���b�g�X�e�[�^�X"
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					If FileExists(ScenarioPath & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve") Then
						StartScenario(ScenarioPath & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve")
					ElseIf FileExists(ExtDataPath & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve") Then 
						StartScenario(ExtDataPath & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve")
					ElseIf FileExists(ExtDataPath2 & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve") Then 
						StartScenario(ExtDataPath2 & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve")
					Else
						StartScenario(AppPath & "Lib\�p�C���b�g�X�e�[�^�X�\��.eve")
					End If
					'�T�u�X�e�[�W��ʏ�̃X�e�[�W�Ƃ��Ď��s
					IsSubStage = True
					Exit Sub
					
				Case "���j�b�g�X�e�[�^�X"
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					If FileExists(ScenarioPath & "Lib\���j�b�g�X�e�[�^�X�\��.eve") Then
						StartScenario(ScenarioPath & "Lib\���j�b�g�X�e�[�^�X�\��.eve")
					ElseIf FileExists(ExtDataPath & "Lib\���j�b�g�X�e�[�^�X�\��.eve") Then 
						StartScenario(ExtDataPath & "Lib\���j�b�g�X�e�[�^�X�\��.eve")
					ElseIf FileExists(ExtDataPath2 & "Lib\���j�b�g�X�e�[�^�X�\��.eve") Then 
						StartScenario(ExtDataPath2 & "Lib\���j�b�g�X�e�[�^�X�\��.eve")
					Else
						StartScenario(AppPath & "Lib\���j�b�g�X�e�[�^�X�\��.eve")
					End If
					'�T�u�X�e�[�W��ʏ�̃X�e�[�W�Ƃ��Ď��s
					IsSubStage = True
					Exit Sub
					
				Case "�L�����Z��"
					'�L�����Z��
					
					'���[�U�[��`�̃C���^�[�~�b�V�����R�}���h
				Case Else
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					StartScenario(GetValueAsString(ListItemID(ret)))
					If IsSubStage Then
						'�C���^�[�~�b�V�������ĊJ
						KeepBGM = False
						BossBGM = False
						ChangeBGM(BGMName("Intermission"))
						UList.Update()
						PList.Update()
						IList.Update()
						ClearEventData()
						If MapWidth > 1 Then
							ClearMap()
						End If
						IsSubStage = False
						EnlargeListBoxHeight()
					Else
						'�T�u�X�e�[�W��ʏ�̃X�e�[�W�Ƃ��Ď��s
						IsSubStage = True
						Exit Sub
					End If
			End Select
NextLoop: 
		Loop 
	End Sub
	
	'�@�̉����R�}���h
	Public Sub RankUpCommand()
		Dim k, i, j, urank As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim cost As Integer
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim use_max_rank As Boolean
		Dim name_width As Short
		
		TopItem = 1
		
		'�f�t�H���g�̃\�[�g���@
		If IsOptionDefined("���g��") Then
			sort_mode = "���x��"
		Else
			sort_mode = "�g�o"
		End If
		
		'�ő�����������j�b�g���ɕύX����Ă��邩�����炩���߃`�F�b�N
		For	Each u In UList
			If u.IsFeatureAvailable("�ő������") Then
				use_max_rank = True
				Exit For
			End If
		Next u
		
		'���j�b�g���̍��̕�������ݒ�
		name_width = 33
		If use_max_rank Then
			name_width = name_width - 2
		End If
		If IsOptionDefined("���g��") Then
			name_width = name_width + 8
		End If
		
		'���j�b�g�̃��X�g���쐬
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemFlag(1)
		ReDim ListItemComment(1)
		list(1) = "�����בւ���"
		For	Each u In UList
			With u
				If .Party0 <> "����" Or .Status_Renamed <> "�ҋ@" Then
					GoTo NextLoop
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemFlag(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'�������\�H
				cost = RankUpCost(u)
				If cost > Money Or cost > 10000000 Then
					ListItemFlag(UBound(list)) = True
				End If
				
				'���j�b�g�����N
				If use_max_rank Then
					list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
					If MaxRank(u) > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
					Else
						list(UBound(list)) = list(UBound(list)) & "--"
					End If
				Else
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
					End If
				End If
				
				'�����ɕK�v�Ȏ���
				If cost < 10000000 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(cost), 7)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("----", 7)
				End If
				
				'���g���̏ꍇ�̓p�C���b�g���x�����\��
				If IsOptionDefined("���g��") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
					End If
				End If
				
				'���j�b�g�Ɋւ�����
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
				
				'���g���łȂ��ꍇ�̓p�C���b�g����\��
				If Not IsOptionDefined("���g��") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & "  " & .MainPilot.Nickname
					End If
				End If
				
				'�������Ă���A�C�e�����R�����g���ɗ�L
				For k = 1 To .CountItem
					With .Item(k)
						If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						End If
					End With
				Next 
				
				'���j�b�g�h�c���L�^���Ă���
				id_list(UBound(list)) = .ID
			End With
NextLoop: 
		Next u
		
Beginning: 
		
		'�\�[�g
		If InStr(sort_mode, "����") = 0 Then
			'���l���g�����\�[�g
			
			'�܂����בւ��Ɏg���L�[�̃��X�g���쐬
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "�g�o"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "�d�m"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "���b"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "�^����"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "���j�b�g�����N"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "���x��"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'�L�[���g���ĕ��׊���
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'���l���g�����\�[�g
			
			'�܂����בւ��Ɏg���L�[�̃��X�g���쐬
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "����", "���j�b�g����"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "�p�C���b�g����"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'�L�[���g���ĕ��׊���
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'�������郆�j�b�g��I��
		If IsOptionDefined("���g��") Then
			If use_max_rank Then
				ret = ListBox("���j�b�g�I���F " & Term("����") & VB6.Format(Money), list, "���j�b�g                               " & Term("�����N", Nothing, 6) & "  ��p Lv  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��"), "�A���\��,�R�����g")
			Else
				ret = ListBox("���j�b�g�I���F " & Term("����") & VB6.Format(Money), list, "���j�b�g                             " & Term("�����N", Nothing, 6) & "   ��p Lv  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��"), "�A���\��,�R�����g")
			End If
		Else
			If use_max_rank Then
				ret = ListBox("���j�b�g�I���F " & Term("����") & VB6.Format(Money), list, "���j�b�g                       " & Term("�����N", Nothing, 6) & "  ��p  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " �p�C���b�g", "�A���\��,�R�����g")
			Else
				ret = ListBox("���j�b�g�I���F " & Term("����") & VB6.Format(Money), list, "���j�b�g                     " & Term("�����N", Nothing, 6) & "   ��p  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " �p�C���b�g", "�A���\��,�R�����g")
			End If
		End If
		
		Select Case ret
			Case 0
				'�L�����Z��
				Exit Sub
			Case 1
				'�\�[�g���@��I��
				If IsOptionDefined("���g��") Then
					sort_mode_type(1) = "����"
					sort_mode_list(1) = "����"
					sort_mode_type(2) = "���x��"
					sort_mode_list(2) = "���x��"
					sort_mode_type(3) = "�g�o"
					sort_mode_list(3) = Term("�g�o")
					sort_mode_type(4) = "�d�m"
					sort_mode_list(4) = Term("�d�m")
					sort_mode_type(5) = "���b"
					sort_mode_list(5) = Term("���b")
					sort_mode_type(6) = "�^����"
					sort_mode_list(6) = Term("�^����")
					sort_mode_type(7) = "���j�b�g�����N"
					sort_mode_list(7) = Term("�����N")
				Else
					sort_mode_type(1) = "�g�o"
					sort_mode_list(1) = Term("�g�o")
					sort_mode_type(2) = "�d�m"
					sort_mode_list(2) = Term("�d�m")
					sort_mode_type(3) = "���b"
					sort_mode_list(3) = Term("���b")
					sort_mode_type(4) = "�^����"
					sort_mode_list(4) = Term("�^����")
					sort_mode_type(5) = "���j�b�g�����N"
					sort_mode_list(5) = Term("�����N")
					sort_mode_type(6) = "���j�b�g����"
					sort_mode_list(6) = "���j�b�g����"
					sort_mode_type(7) = "�p�C���b�g����"
					sort_mode_list(7) = "�p�C���b�g����"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				ret = ListBox("�ǂ�ŕ��בւ��܂����H", sort_mode_list, "���בւ��̕��@", "�A���\��,�R�����g")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'�\�[�g���@��ύX���čĕ\��
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo Beginning
		End Select
		
		'�������郆�j�b�g������
		u = UList.Item(id_list(ret))
		
		'�������邩�m�F
		If u.IsHero Then
			If MsgBox(u.Nickname0 & "���p���[�A�b�v�����܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "�p���[�A�b�v") <> 1 Then
				GoTo Beginning
			End If
		Else
			If MsgBox(u.Nickname0 & "���������܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "����") <> 1 Then
				GoTo Beginning
			End If
		End If
		
		'���������炷
		IncrMoney(-RankUpCost(u))
		
		'���j�b�g�����N����i�K�グ��
		With u
			.Rank = .Rank + 1
			.HP = .MaxHP
			.EN = .MaxEN
			
			'���`�Ԃ̃����N���グ�Ă���
			For i = 1 To .CountOtherForm
				.OtherForm(i).Rank = .Rank
				.OtherForm(i).HP = .OtherForm(i).MaxHP
				.OtherForm(i).EN = .OtherForm(i).MaxEN
			Next 
			
			'���̌`�Ԃ���`�Ԃ̕����`�Ԃ��������ꂽ�ꍇ�͑��̕����`�Ԃ̃��j�b�g��
			'�����N���グ��
			If .IsFeatureAvailable("����") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "����" Then
						buf = LIndex(.FeatureData(i), 2)
						If LLength(.FeatureData(i)) = 3 Then
							If UDList.IsDefined(buf) Then
								If UDList.Item(buf).IsFeatureAvailable("��`��") Then
									Exit For
								End If
							End If
						Else
							If UDList.IsDefined(buf) Then
								If Not UDList.Item(buf).IsFeatureAvailable("��������") Then
									Exit For
								End If
							End If
						End If
					End If
				Next 
				If i <= .CountFeature Then
					urank = .Rank
					buf = UDList.Item(LIndex(.FeatureData(i), 2)).FeatureData("����")
					For i = 2 To LLength(buf)
						If Not UList.IsDefined(LIndex(buf, i)) Then
							GoTo NextForm
						End If
						
						With UList.Item(LIndex(buf, i))
							.Rank = MaxLng(urank, .Rank)
							.HP = .MaxHP
							.EN = .MaxEN
							For j = 1 To .CountOtherForm
								.OtherForm(j).Rank = .Rank
								.OtherForm(j).HP = .OtherForm(j).MaxHP
								.OtherForm(j).EN = .OtherForm(j).MaxEN
							Next 
							
							For j = 1 To UBound(id_list)
								If .CurrentForm.ID = id_list(j) Then
									Exit For
								End If
							Next 
							
							If j > UBound(id_list) Then
								GoTo NextForm
							End If
							
							If use_max_rank Then
								list(j) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
								If MaxRank(u) > 0 Then
									list(j) = list(j) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
								Else
									list(j) = list(j) & "--"
								End If
							Else
								If .Rank < 10 Then
									list(j) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
								Else
									list(j) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
								End If
							End If
							
							If RankUpCost(u) < 1000000 Then
								list(j) = list(j) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
							Else
								list(j) = list(j) & LeftPaddedString("----", 7)
							End If
							
							If IsOptionDefined("���g��") Then
								If .CountPilot > 0 Then
									list(j) = list(j) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
								End If
							End If
							list(j) = list(j) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
							If Not IsOptionDefined("���g��") Then
								If .CountPilot > 0 Then
									list(j) = list(j) & "  " & .MainPilot.Nickname
								End If
							End If
						End With
NextForm: 
					Next 
				End If
			End If
			
			'���̃��j�b�g�̏ꍇ�͕����`�Ԃ̃��j�b�g�̃����N���グ��
			If .IsFeatureAvailable("����") Then
				urank = .Rank
				buf = .FeatureData("����")
				For i = 2 To LLength(buf)
					If UList.IsDefined(LIndex(buf, i)) Then
						With UList.Item(LIndex(buf, i))
							.Rank = MaxLng(urank, .Rank)
							.HP = .MaxHP
							.EN = .MaxEN
							For j = 1 To .CountOtherForm
								.OtherForm(j).Rank = .Rank
								.OtherForm(j).HP = .OtherForm(j).MaxHP
								.OtherForm(j).EN = .OtherForm(j).MaxEN
							Next 
						End With
					End If
				Next 
			End If
			
			'���j�b�g���X�g�̕\�����e���X�V
			
			If use_max_rank Then
				list(ret) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
				If MaxRank(u) > 0 Then
					list(ret) = list(ret) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
				Else
					list(ret) = list(ret) & "--"
				End If
			Else
				If .Rank < 10 Then
					list(ret) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
				Else
					list(ret) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
				End If
			End If
			
			If RankUpCost(u) < 10000000 Then
				list(ret) = list(ret) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
			Else
				list(ret) = list(ret) & LeftPaddedString("----", 7)
			End If
			
			If IsOptionDefined("���g��") Then
				If .CountPilot > 0 Then
					list(ret) = list(ret) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
				End If
			End If
			list(ret) = list(ret) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
			If Not IsOptionDefined("���g��") Then
				If .CountPilot > 0 Then
					list(ret) = list(ret) & "  " & .MainPilot.Nickname
				End If
			End If
		End With
		
		'���߂Ď����Ɖ�����𒲂ׁA�e���j�b�g�������\���`�F�b�N����
		For i = 2 To UBound(list)
			cost = RankUpCost(UList.Item(id_list(i)))
			If cost > Money Or cost > 10000000 Then
				ListItemFlag(i) = True
			Else
				ListItemFlag(i) = False
			End If
		Next 
		
		GoTo Beginning
	End Sub
	
	'���j�b�g�����N���グ�邽�߂̃R�X�g���Z�o
	Public Function RankUpCost(ByRef u As Unit) As Integer
		With u
			'����ȏ�����ł��Ȃ��H
			If .Rank >= MaxRank(u) Then
				RankUpCost = 999999999
				Exit Function
			End If
			
			'���̏�Ԃɂ���ꍇ�͂��ꂪ��`�ԂłȂ���������s��
			If .IsFeatureAvailable("����") Then
				If (LLength(.FeatureData("����")) = 3 And Not .IsFeatureAvailable("��`��")) Or .IsFeatureAvailable("��������") Then
					RankUpCost = 999999999
					Exit Function
				End If
			End If
			
			If IsOptionDefined("�������") Then
				'�������̏ꍇ
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 30000
					Case 4
						RankUpCost = 40000
					Case 5
						RankUpCost = 50000
					Case 6
						RankUpCost = 60000
					Case 7
						RankUpCost = 70000
					Case 8
						RankUpCost = 80000
					Case 9
						RankUpCost = 100000
					Case 10
						RankUpCost = 120000
					Case 11
						RankUpCost = 140000
					Case 12
						RankUpCost = 160000
					Case 13
						RankUpCost = 180000
					Case 14
						RankUpCost = 200000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			ElseIf IsOptionDefined("�P�T�i�K����") Then 
				'�ʏ�̂P�T�i����
				'(�P�O�i��������肨���߈������i�ɂȂ��Ă���܂��c�c)
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 120000
					Case 6
						RankUpCost = 160000
					Case 7
						RankUpCost = 200000
					Case 8
						RankUpCost = 250000
					Case 9
						RankUpCost = 300000
					Case 10
						RankUpCost = 350000
					Case 11
						RankUpCost = 400000
					Case 12
						RankUpCost = 450000
					Case 13
						RankUpCost = 500000
					Case 14
						RankUpCost = 550000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			Else
				'�ʏ�̂P�O�i����
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 150000
					Case 6
						RankUpCost = 200000
					Case 7
						RankUpCost = 300000
					Case 8
						RankUpCost = 400000
					Case 9
						RankUpCost = 500000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			End If
			
			'���j�b�g�p����\�́u������C���v�ɂ��C��
			If .IsFeatureAvailable("������C��") Then
				RankUpCost = RankUpCost * (1# + .FeatureLevel("������C��") / 10)
			End If
		End With
	End Function
	
	'���j�b�g�̍ő���������Z�o
	Public Function MaxRank(ByRef u As Unit) As Integer
		If IsOptionDefined("�T�i�K����") Then
			'�T�i�K�����܂ł����o���Ȃ�
			MaxRank = 5
		ElseIf IsOptionDefined("�P�T�i�K����") Then 
			'�P�T�i�K�����܂ŉ\
			MaxRank = 15
		Else
			'�f�t�H���g�͂P�O�i�K�܂�
			MaxRank = 10
		End If
		
		With u
			'Disable�R�}���h�ŉ����s�ɂ���Ă���H
			If IsGlobalVariableDefined("Disable(" & .Name & ",����)") Then
				MaxRank = 0
				Exit Function
			End If
			
			'�ő���������ݒ肳��Ă���H
			If .IsFeatureAvailable("�ő������") Then
				MaxRank = MinLng(MaxRank, .FeatureLevel("�ő������"))
			End If
		End With
	End Function
	
	'��芷���R�}���h
	Public Sub ExchangeUnitCommand()
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode, sort_mode2 As String
		Dim sort_mode_type() As String
		Dim sort_mode_list() As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim p As Pilot
		Dim pname As String
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim is_support As Boolean
		Dim caption_str As String
		Dim top_item As Short
		
		top_item = 1
		
		'�f�t�H���g�̃\�[�g���@
		sort_mode = "���x��"
		sort_mode2 = "����"
		
Beginning: 
		
		'��芷����p�C���b�g�̈ꗗ���쐬
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "�����בւ���"
		For	Each p In PList
			With p
				If .Party <> "����" Or .Away Or IsGlobalVariableDefined("Fix(" & .Name & ")") Then
					GoTo NextLoop
				End If
				
				'�ǉ��p�C���b�g���T�|�[�g�͏�芷���s��
				If .IsAdditionalPilot Or .IsAdditionalSupport Then
					GoTo NextLoop
				End If
				
				is_support = False
				If Not .Unit_Renamed Is Nothing Then
					'�T�|�[�g����������Ă���ꍇ�͏��~��s��
					If .Unit_Renamed.CountSupport > 1 Then
						GoTo NextLoop
					End If
					
					'�T�|�[�g�p�C���b�g�Ƃ��ď�荞��ł��邩�𔻒�
					If .Unit_Renamed.CountSupport = 1 Then
						If .ID = .Unit_Renamed.Support(1).ID Then
							is_support = True
						End If
					End If
					
					'�ʏ�̃p�C���b�g�̏ꍇ
					If Not is_support Then
						'�R�l���ȏ�͏��~��s��
						If .Unit_Renamed.Data.PilotNum <> 1 And System.Math.Abs(.Unit_Renamed.Data.PilotNum) <> 2 Then
							GoTo NextLoop
						End If
					End If
				End If
				
				If is_support Then
					'�T�|�[�g�p�C���b�g�̏ꍇ
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'�p�C���b�g�̃X�e�[�^�X
					list(UBound(list)) = RightPaddedString("*" & .Nickname, 25) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4)
					
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'���j�b�g�̃X�e�[�^�X
							list(UBound(list)) = list(UBound(list)) & "  " & RightPaddedString(.Nickname0, 29) & "(" & .MainPilot.Nickname & ")"
							
							'���j�b�g���������Ă���A�C�e���ꗗ
							For k = 1 To .CountItem
								With .Item(k)
									If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
										ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
									End If
								End With
							Next 
						End With
					End If
					
					'�p�C���b�g�h�c���L�^���Ă���
					id_list(UBound(list)) = .ID
				ElseIf .Unit_Renamed Is Nothing Then 
					'���j�b�g�ɏ���Ă��Ȃ��p�C���b�g�̏ꍇ
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'�p�C���b�g�̃X�e�[�^�X
					list(UBound(list)) = RightPaddedString(" " & .Nickname, 25) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4)
					
					'�p�C���b�g�h�c���L�^���Ă���
					id_list(UBound(list)) = .ID
				ElseIf .Unit_Renamed.CountPilot <= 2 Then 
					'�������̃��j�b�g�ɏ���Ă���p�C���b�g�̏ꍇ
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'�p�C���b�g������Ȃ��H
					If .Unit_Renamed.CountPilot < System.Math.Abs(.Unit_Renamed.Data.PilotNum) Then
						list(UBound(list)) = "-"
					Else
						list(UBound(list)) = " "
					End If
					
					If .Unit_Renamed.IsFeatureAvailable("�ǉ��p�C���b�g") Then
						pname = .Unit_Renamed.MainPilot.Nickname
					Else
						pname = .Nickname
					End If
					
					'�������̏ꍇ�͉��Ԗڂ̃p�C���b�g���\��
					If System.Math.Abs(.Unit_Renamed.Data.PilotNum) > 1 Then
						For k = 1 To .Unit_Renamed.CountPilot
							If .Unit_Renamed.Pilot(k) Is p Then
								pname = pname & "(" & VB6.Format(k) & ")"
							End If
						Next 
					End If
					
					'�p�C���b�g�����j�b�g�̃X�e�[�^�X
					list(UBound(list)) = list(UBound(list)) & RightPaddedString(pname, 24) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4) & "  " & RightPaddedString((.Unit_Renamed.Nickname0), 29)
					If .Unit_Renamed.CountSupport > 0 Then
						list(UBound(list)) = list(UBound(list)) & "(" & .Unit_Renamed.Support(1).Nickname & ")"
					End If
					
					'���j�b�g���������Ă���A�C�e���ꗗ
					With .Unit_Renamed
						For k = 1 To .CountItem
							With .Item(k)
								If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
									ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
								End If
							End With
						Next 
					End With
					
					'�p�C���b�g�h�c���L�^���Ă���
					id_list(UBound(list)) = .ID
				End If
			End With
NextLoop: 
		Next p
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'�\�[�g
		If sort_mode = "���x��" Then
			'���x���ɂ��\�[�g
			
			'�܂����x���̃��X�g���쐬
			ReDim key_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					With .Item(id_list(i))
						key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
					End With
				Next 
			End With
			
			'���x�����g���ĕ��׊���
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'�ǂ݉����ɂ��\�[�g
			
			'�܂��ǂ݉����̃��X�g���쐬
			ReDim strkey_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'�ǂ݉������g���ĕ��בւ�
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'�p�C���b�g��I��
		TopItem = top_item
		If IsOptionDefined("���g��") Then
			caption_str = " �L�����N�^�[          ���x��  ���j�b�g"
			ret = ListBox("�L�����N�^�[�I��", list, caption_str, "�A���\��,�R�����g")
		Else
			caption_str = " �p�C���b�g            ���x��  ���j�b�g"
			ret = ListBox("�p�C���b�g�I��", list, caption_str, "�A���\��,�R�����g")
		End If
		top_item = TopItem
		
		Select Case ret
			Case 0
				'�L�����Z��
				Exit Sub
			Case 1
				'�\�[�g���@��I��
				ReDim sort_mode_list(2)
				sort_mode_list(1) = "���x��"
				sort_mode_list(2) = "����"
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				ret = ListBox("�ǂ�ŕ��בւ��܂����H", sort_mode_list, "���בւ��̕��@", "�A���\��,�R�����g")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'�\�[�g���@��ύX���čĕ\��
				If ret > 0 Then
					sort_mode = sort_mode_list(ret)
				End If
				GoTo SortAgain
		End Select
		
		'��芷��������p�C���b�g
		p = PList.Item(id_list(ret))
		
		'��芷���惆�j�b�g�ꗗ�쐬
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "�����בւ���"
		For	Each u In UList
			With u
				If .Party0 <> "����" Or .Status_Renamed <> "�ҋ@" Then
					GoTo NextUnit
				End If
				
				If .CountSupport > 1 Then
					If InStr(p.Class_Renamed, "�ꑮ�T�|�[�g") = 0 Then
						GoTo NextUnit
					End If
				End If
				
				If u Is p.Unit_Renamed Then
					GoTo NextUnit
				End If
				
				If Not p.IsAbleToRide(u) Then
					GoTo NextUnit
				End If
				
				'�T�|�[�g�L�����łȂ���Ώ�芷������p�C���b�g���ɐ���������
				If Not p.IsSupport(u) Then
					If .Data.PilotNum <> 1 And System.Math.Abs(.Data.PilotNum) <> 2 Then
						GoTo NextUnit
					End If
				End If
				
				If .CountPilot > 0 Then
					If IsGlobalVariableDefined("Fix(" & .Pilot(1).Name & ")") And Not p.IsSupport(u) Then
						'Fix�R�}���h�Ńp�C���b�g���Œ肳�ꂽ���j�b�g�̓T�|�[�g�łȂ�
						'�����芷���s��
						GoTo NextUnit
					End If
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'�p�C���b�g������Ă���H
					If .CountPilot < System.Math.Abs(.Data.PilotNum) Then
						list(UBound(list)) = "-"
					Else
						list(UBound(list)) = " "
					End If
					
					list(UBound(list)) = list(UBound(list)) & RightPaddedString(.Nickname0, 35) & RightPaddedString(.MainPilot.Nickname, 21)
					If .Rank < 10 Then
						list(UBound(list)) = list(UBound(list)) & " " & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & VB6.Format(.Rank)
					End If
					If .CountSupport > 0 Then
						list(UBound(list)) = list(UBound(list)) & " (" & .Support(1).Nickname & ")"
					End If
					
					'���j�b�g�ɑ�������Ă���A�C�e�����R�����g���ɗ�L
					For j = 1 To .CountItem
						With .Item(j)
							If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
								ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							End If
						End With
					Next 
					
					'���j�b�g�h�c���L�^���Ă���
					id_list(UBound(list)) = .ID
				ElseIf Not p.IsSupport(u) Then 
					'�N������ĂȂ����j�b�g�ɏ���̂͒ʏ�p�C���b�g�̂�
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					list(UBound(list)) = " " & RightPaddedString(.Nickname0, 35) & Space(21)
					If .Rank < 10 Then
						list(UBound(list)) = list(UBound(list)) & " " & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & VB6.Format(.Rank)
					End If
					
					'���j�b�g�ɑ�������Ă���A�C�e�����R�����g���ɗ�L
					For j = 1 To .CountItem
						With .Item(j)
							If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
								ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							End If
						End With
					Next 
					
					'���j�b�g�h�c���L�^���Ă���
					id_list(UBound(list)) = .ID
				End If
			End With
NextUnit: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
SortAgain2: 
		
		'�\�[�g
		If InStr(sort_mode2, "����") = 0 Then
			'���l�ɂ��\�[�g
			
			'�܂��L�[�̃��X�g���쐬
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode2
					Case "�g�o"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "�d�m"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "���b"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "�^����"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "���j�b�g�����N"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
				End Select
			End With
			
			'�L�[���g���ĕ��בւ�
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'�ǂ݉����ɂ��\�[�g
			
			'�܂��ǂ݉����̃��X�g���쐬
			ReDim strkey_list(UBound(list))
			With UList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'�ǂ݉������g���ĕ��בւ�
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'��芷�����I��
		TopItem = 1
		u = p.Unit_Renamed
		If IsOptionDefined("���g��") Then
			caption_str = " ���j�b�g                           �L�����N�^�[       " & Term("�����N")
		Else
			caption_str = " ���j�b�g                           �p�C���b�g         " & Term("�����N")
		End If
		If Not u Is Nothing Then
			If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
				ret = ListBox("��芷����I�� �F " & u.MainPilot.Nickname & " (" & u.Nickname & ")", list, caption_str, "�A���\��,�R�����g")
			Else
				ret = ListBox("��芷����I�� �F " & p.Nickname & " (" & u.Nickname & ")", list, caption_str, "�A���\��,�R�����g")
			End If
		Else
			ret = ListBox("��芷����I�� �F " & p.Nickname, list, caption_str, "�A���\��,�R�����g")
		End If
		
		Select Case ret
			Case 0
				'�L�����Z��
				Exit Sub
			Case 1
				'�\�[�g���@��I��
				ReDim sort_mode_type(6)
				ReDim sort_mode_list(6)
				If IsOptionDefined("���g��") Then
					sort_mode_type(1) = "����"
					sort_mode_list(1) = "����"
					sort_mode_type(2) = "�g�o"
					sort_mode_list(2) = Term("�g�o")
					sort_mode_type(3) = "�d�m"
					sort_mode_list(3) = Term("�d�m")
					sort_mode_type(4) = "���b"
					sort_mode_list(4) = Term("���b")
					sort_mode_type(5) = "�^����"
					sort_mode_list(5) = Term("�^����")
					sort_mode_type(6) = "���j�b�g�����N"
					sort_mode_list(6) = Term("�����N")
				Else
					sort_mode_type(1) = "�g�o"
					sort_mode_list(1) = Term("�g�o")
					sort_mode_type(2) = "�d�m"
					sort_mode_list(2) = Term("�d�m")
					sort_mode_type(3) = "���b"
					sort_mode_list(3) = Term("���b")
					sort_mode_type(4) = "�^����"
					sort_mode_list(4) = Term("�^����")
					sort_mode_type(5) = "���j�b�g�����N"
					sort_mode_list(5) = Term("�����N")
					sort_mode_type(6) = "���j�b�g����"
					sort_mode_list(6) = "���j�b�g����"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				ret = ListBox("�ǂ�ŕ��בւ��܂����H", sort_mode_list, "���בւ��̕��@", "�A���\��,�R�����g")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'�\�[�g���@��ύX���čĕ\��
				If ret > 0 Then
					sort_mode2 = sort_mode_type(ret)
				End If
				GoTo SortAgain2
		End Select
		
		'�L�����Z���H
		If ret = 0 Then
			GoTo Beginning
		End If
		
		u = UList.Item(id_list(ret))
		
		'���̃��j�b�g����~�낷
		p.GetOff()
		
		'��芷��
		With u
			If Not p.IsSupport(u) Then
				'�ʏ�̃p�C���b�g
				If .CountPilot = .Data.PilotNum Then
					.Pilot(1).GetOff()
				End If
			Else
				'�T�|�[�g�p�C���b�g
				For i = 1 To .CountSupport
					.Support(1).GetOff()
				Next 
			End If
		End With
		p.Ride(UList.Item(id_list(ret)))
		
		GoTo Beginning
	End Sub
	
	'�A�C�e�������R�}���h
	Public Sub ExchangeItemCommand(Optional ByRef selected_unit As Unit = Nothing, Optional ByRef selected_part As String = "")
		Dim j, i, k As Short
		Dim inum, inum2 As Short
		Dim list() As String
		Dim id_list() As String
		Dim iid As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim caption_str As String
		Dim u As Unit
		Dim it As Item
		Dim iname As String
		Dim buf As String
		Dim ret As Short
		Dim part_list() As String
		Dim part_item() As String
		Dim arm_point, shoulder_point As Short
		Dim ipart As String
		Dim empty_slot As Short
		Dim is_right_hand_available As Boolean
		Dim is_left_hand_available As Boolean
		Dim item_list() As String
		Dim top_item1, top_item2 As Short
		
		top_item1 = 1
		top_item2 = 1
		
		'�f�t�H���g�̃\�[�g���@
		If IsOptionDefined("���g��") Then
			sort_mode = "���x��"
		Else
			sort_mode = "�g�o"
		End If
		
		'���j�b�g�����炩���ߑI������Ă���ꍇ
		'(���j�b�g�X�e�[�^�X����̃A�C�e��������)
		If Not selected_unit Is Nothing Then
			EnlargeListBoxHeight()
			ReduceListBoxWidth()
			
			u = selected_unit
			If MainForm.Visible Then
				If Not u Is DisplayedUnit Then
					DisplayUnitStatus(u)
				End If
			End If
			
			GoTo MakeEquipedItemList
		End If
		
Beginning: 
		
		'���j�b�g�ꗗ�̍쐬
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "�����בւ���"
		For	Each u In UList
			With u
				If .Party0 <> "����" Or .Status_Renamed <> "�ҋ@" Then
					GoTo NextUnit
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'�������Ă���A�C�e���̐��𐔂���
				inum = 0
				inum2 = 0
				For i = 1 To .CountItem
					With .Item(i)
						If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
								inum = inum + .Size
							Else
								inum2 = inum2 + .Size
							End If
						End If
					End With
				Next 
				
				'���X�g���쐬
				If IsOptionDefined("���g��") Then
					list(UBound(list)) = RightPaddedString(.Nickname0, 39)
				Else
					list(UBound(list)) = RightPaddedString(.Nickname0, 31)
				End If
				list(UBound(list)) = list(UBound(list)) & VB6.Format(inum) & "/" & VB6.Format(.MaxItemNum)
				If inum2 > 0 Then
					list(UBound(list)) = list(UBound(list)) & "(" & VB6.Format(inum2) & ")   "
				Else
					list(UBound(list)) = list(UBound(list)) & "      "
				End If
				If .Rank < 10 Then
					list(UBound(list)) = list(UBound(list)) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
				Else
					list(UBound(list)) = list(UBound(list)) & VB6.Format(.Rank)
				End If
				If IsOptionDefined("���g��") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
					End If
				End If
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5)
				If Not IsOptionDefined("���g��") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & " " & .MainPilot.Nickname
					End If
				End If
				
				'���j�b�g�h�c���L�^���Ă���
				id_list(UBound(list)) = .ID
			End With
NextUnit: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'�\�[�g
		If InStr(sort_mode, "����") = 0 Then
			'���l�ɂ��\�[�g
			
			'�܂��L�[�̃��X�g���쐬
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "�g�o"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "�d�m"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "���b"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "�^����"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "���j�b�g�����N"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "���x��"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'�L�[���g���ĕ��בւ�
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'������ɂ��\�[�g
			
			'�܂��̓L�[�̃��X�g���쐬
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "����", "���j�b�g����"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "�p�C���b�g����"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'�L�[���g���ĕ��בւ�
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'�A�C�e�����������郆�j�b�g��I��
		TopItem = top_item1
		If IsOptionDefined("���g��") Then
			ret = ListBox("�A�C�e�����������郆�j�b�g��I��", list, "���j�b�g                               �A�C�e�� " & Term("RK", Nothing, 2) & " Lv  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��"), "�A���\��,�R�����g")
		Else
			ret = ListBox("�A�C�e�����������郆�j�b�g��I��", list, "���j�b�g                       �A�C�e�� " & Term("RK", Nothing, 2) & "  " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " �p�C���b�g", "�A���\��,�R�����g")
		End If
		top_item1 = TopItem
		
		Select Case ret
			Case 0
				'�L�����Z��
				Exit Sub
			Case 1
				'�\�[�g���@��I��
				If IsOptionDefined("���g��") Then
					sort_mode_type(1) = "����"
					sort_mode_list(1) = "����"
					sort_mode_type(2) = "���x��"
					sort_mode_list(2) = "���x��"
					sort_mode_type(3) = "�g�o"
					sort_mode_list(3) = Term("�g�o")
					sort_mode_type(4) = "�d�m"
					sort_mode_list(4) = Term("�d�m")
					sort_mode_type(5) = "���b"
					sort_mode_list(5) = Term("���b")
					sort_mode_type(6) = "�^����"
					sort_mode_list(6) = Term("�^����")
					sort_mode_type(7) = "���j�b�g�����N"
					sort_mode_list(7) = Term("�����N")
				Else
					sort_mode_type(1) = "�g�o"
					sort_mode_list(1) = Term("�g�o")
					sort_mode_type(2) = "�d�m"
					sort_mode_list(2) = Term("�d�m")
					sort_mode_type(3) = "���b"
					sort_mode_list(3) = Term("���b")
					sort_mode_type(4) = "�^����"
					sort_mode_list(4) = Term("�^����")
					sort_mode_type(5) = "���j�b�g�����N"
					sort_mode_list(5) = Term("�����N")
					sort_mode_type(6) = "���j�b�g����"
					sort_mode_list(6) = "���j�b�g����"
					sort_mode_type(7) = "�p�C���b�g����"
					sort_mode_list(7) = "�p�C���b�g����"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				ret = ListBox("�ǂ�ŕ��בւ��܂����H", sort_mode_list, "���בւ��̕��@", "�A���\��,�R�����g")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'�\�[�g���@��ύX���čĕ\��
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo SortAgain
		End Select
		
		'���j�b�g��I��
		u = UList.Item(id_list(ret))
		
MakeEquipedItemList: 
		
		'�I�����ꂽ���j�b�g���������Ă���A�C�e���ꗗ�̍쐬
		Dim tmp_part_list() As String
		With u
			Do While True
				'�A�C�e���̑������ꗗ���쐬
				ReDim part_list(0)
				If .IsFeatureAvailable("������") Then
					buf = .FeatureData("������")
					If InStr(buf, "�r") > 0 Then
						arm_point = UBound(part_list) + 1
						ReDim Preserve part_list(UBound(part_list) + 2)
						part_list(1) = "�E��"
						part_list(2) = "����"
					End If
					If InStr(buf, "��") > 0 Then
						shoulder_point = UBound(part_list) + 1
						ReDim Preserve part_list(UBound(part_list) + 2)
						part_list(UBound(part_list) - 1) = "�E��"
						part_list(UBound(part_list)) = "����"
					End If
					If InStr(buf, "��") > 0 Then
						ReDim Preserve part_list(UBound(part_list) + 1)
						part_list(UBound(part_list)) = "��"
					End If
					If InStr(buf, "��") > 0 Then
						ReDim Preserve part_list(UBound(part_list) + 1)
						part_list(UBound(part_list)) = "��"
					End If
				End If
				For i = 1 To .CountFeature
					If .Feature(i) = "�n�[�h�|�C���g" Then
						ipart = .FeatureData(i)
						Select Case ipart
							Case "�����p�[�c", "�A�C�e��", "��\��"
								'�\�����Ȃ�
							Case Else
								For j = 1 To UBound(part_list)
									If part_list(j) = ipart Then
										Exit For
									End If
								Next 
								If j > UBound(part_list) Then
									ReDim Preserve part_list(UBound(part_list) + .ItemSlotSize(ipart))
									For j = UBound(part_list) - .ItemSlotSize(ipart) + 1 To UBound(part_list)
										part_list(j) = ipart
									Next 
								End If
						End Select
					End If
				Next 
				
				ReDim Preserve part_list(UBound(part_list) + .MaxItemNum)
				If .IsHero Then
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "�A�C�e��"
					Next 
				Else
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "�����p�[�c"
					Next 
				End If
				
				'����̑������̃A�C�e���݂̂���������H
				If selected_part <> "" Then
					
					ReDim tmp_part_list(UBound(part_list))
					For i = 1 To UBound(part_list)
						tmp_part_list(i) = part_list(i)
					Next 
					
					ReDim part_list(0)
					arm_point = 0
					shoulder_point = 0
					For i = 1 To UBound(tmp_part_list)
						If tmp_part_list(i) = selected_part Or ((selected_part = "�Ў�" Or selected_part = "����" Or selected_part = "��") And (tmp_part_list(i) = "�E��" Or tmp_part_list(i) = "����")) Or ((selected_part = "��" Or selected_part = "����") And (tmp_part_list(i) = "�E��" Or tmp_part_list(i) = "����")) Or ((selected_part = "�A�C�e��" Or selected_part = "�����p�[�c") And (tmp_part_list(i) = "�A�C�e��" Or tmp_part_list(i) = "�����p�[�c")) Then
							ReDim Preserve part_list(UBound(part_list) + 1)
							part_list(UBound(part_list)) = tmp_part_list(i)
							Select Case part_list(UBound(part_list))
								Case "�E��"
									arm_point = UBound(part_list)
								Case "�E��"
									shoulder_point = UBound(part_list)
							End Select
						End If
					Next 
				End If
				
				ReDim part_item(UBound(part_list))
				
				'�������Ɍ��ݑ������Ă���A�C�e�������蓖��
				For i = 1 To .CountItem
					With .Item(i)
						If .Class_Renamed() = "�Œ�" And .IsFeatureAvailable("��\��") Then
							GoTo NextEquipedItem
						End If
						
						Select Case .Part
							Case "����"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point) = .ID
								part_item(arm_point + 1) = ":"
							Case "�Ў�"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(arm_point) = "" Then
									part_item(arm_point) = .ID
								Else
									part_item(arm_point + 1) = .ID
								End If
							Case "��"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point + 1) = .ID
							Case "����"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(shoulder_point) = .ID
							Case "��"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(shoulder_point) = "" Then
									part_item(shoulder_point) = .ID
								Else
									part_item(shoulder_point + 1) = .ID
								End If
							Case "��\��"
								'����
							Case Else
								If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
									For j = 1 To UBound(part_list)
										If (part_list(j) = "�����p�[�c" Or part_list(j) = "�A�C�e��") And part_item(j) = "" Then
											part_item(j) = .ID
											For k = j + 1 To j + .Size - 1
												If k > UBound(part_item) Then
													Exit For
												End If
												part_item(k) = ":"
											Next 
											Exit For
										End If
									Next 
								Else
									For j = 1 To UBound(part_list)
										If part_list(j) = .Part And part_item(j) = "" Then
											part_item(j) = .ID
											For k = j + 1 To j + .Size - 1
												If k > UBound(part_item) Then
													Exit For
												End If
												part_item(k) = ":"
											Next 
											Exit For
										End If
									Next 
								End If
								If j > UBound(part_list) And selected_part = "" Then
									ReDim Preserve part_list(UBound(part_list) + 1)
									ReDim Preserve part_item(UBound(part_list))
									part_list(UBound(part_list)) = .Part
									part_item(UBound(part_list)) = .ID
								End If
						End Select
					End With
NextEquipedItem: 
				Next 
				
				ReDim list(UBound(part_list) + 1)
				ReDim id_list(UBound(list))
				ReDim ListItemComment(UBound(list))
				ReDim ListItemFlag(UBound(list))
				
				'���X�g���\�z
				For i = 1 To UBound(part_item)
					Select Case part_item(i)
						Case ""
							list(i) = RightPaddedString("----", 23) & part_list(i)
						Case ":"
							list(i) = RightPaddedString(" :  ", 23) & part_list(i)
							ListItemComment(i) = ListItemComment(i - 1)
							ListItemFlag(i) = ListItemFlag(i - 1)
						Case Else
							With IList.Item(part_item(i))
								list(i) = RightPaddedString(.Nickname, 22) & " " & part_list(i)
								ListItemComment(i) = .Data.Comment
								id_list(i) = .ID
								For j = i + 1 To i + .Size - 1
									If j > UBound(part_item) Then
										Exit For
									End If
									id_list(j) = .ID
								Next 
								If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
									ListItemFlag(i) = True
									For j = i + 1 To i + .Size - 1
										If j > UBound(part_item) Then
											Exit For
										End If
										ListItemFlag(j) = True
									Next 
								End If
							End With
					End Select
				Next 
				list(UBound(list)) = "������������"
				
				'��������A�C�e����I��
				caption_str = "��������I�� �F " & .Nickname
				If .CountPilot > 0 And Not IsOptionDefined("���g��") Then
					caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
				End If
				caption_str = caption_str & "  " & Term("�g�o", u) & "=" & VB6.Format(.MaxHP) & " " & Term("�d�m", u) & "=" & VB6.Format(.MaxEN) & " " & Term("���b", u) & "=" & VB6.Format(.Armor) & " " & Term("�^����", u) & "=" & VB6.Format(.Mobility) & " " & Term("�ړ���", u) & "=" & VB6.Format(.Speed)
				TopItem = top_item2
				ret = ListBox(caption_str, list, "�A�C�e��               ����", "�A���\��,�R�����g")
				top_item2 = TopItem
				If ret = 0 Then
					Exit Do
				End If
				
				'��������������ꍇ
				If ret = UBound(list) Then
					list(UBound(list)) = "���S�ĊO����"
					caption_str = "�O���A�C�e����I�� �F " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("���g��") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("�g�o", u) & "=" & VB6.Format(.MaxHP) & " " & Term("�d�m", u) & "=" & VB6.Format(.MaxEN) & " " & Term("���b", u) & "=" & VB6.Format(.Armor) & " " & Term("�^����", u) & "=" & VB6.Format(.Mobility) & " " & Term("�ړ���", u) & "=" & VB6.Format(.Speed)
					ret = ListBox(caption_str, list, "�A�C�e��               ����", "�A���\��,�R�����g")
					If ret <> 0 Then
						If ret < UBound(list) Then
							'�w�肳�ꂽ�A�C�e�����O��
							If id_list(ret) <> "" Then
								.DeleteItem(id_list(ret), False)
							ElseIf LIndex(list(ret), 1) = ":" Then 
								.DeleteItem(id_list(ret - 1), False)
							End If
						Else
							'�S�ẴA�C�e�����O��
							For i = 1 To UBound(list) - 1
								If Not ListItemFlag(i) And id_list(i) <> "" Then
									.DeleteItem(id_list(i), False)
								End If
							Next 
						End If
						If MapFileName = "" Then
							.FullRecover()
						End If
						If MainForm.Visible Then
							DisplayUnitStatus(u)
						End If
					End If
					GoTo NextLoop2
				End If
				
				'��������A�C�e���̑�����
				iid = id_list(ret)
				If iid <> "" Then
					ipart = IList.Item(iid).Part
				Else
					ipart = LIndex(list(ret), 2)
				End If
				
				'�󂫃X���b�g�𒲂ׂĂ���
				Select Case ipart
					Case "�E��", "����", "�Ў�", "����", "��"
						is_right_hand_available = True
						is_left_hand_available = True
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "�Ў�" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
										If is_right_hand_available Then
											is_right_hand_available = False
										Else
											is_left_hand_available = False
										End If
									End If
								ElseIf .Part = "��" Then 
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
										is_left_hand_available = False
									End If
								End If
							End With
						Next 
					Case "�E��", "����", "��"
						empty_slot = 2
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "��" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
										empty_slot = empty_slot - 1
									End If
								End If
							End With
						Next 
					Case "�����p�[�c", "�A�C�e��"
						empty_slot = .MaxItemNum
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "�����p�[�c" Or .Part = "�A�C�e��" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
										empty_slot = empty_slot - .Size
									End If
								End If
							End With
						Next 
					Case Else
						empty_slot = 0
						For i = 1 To .CountFeature
							If .Feature(i) = "�n�[�h�|�C���g" And .FeatureData(i) = ipart Then
								empty_slot = empty_slot + .FeatureLevel(i)
							End If
						Next 
						If empty_slot = 0 Then
							empty_slot = 1
						End If
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = ipart Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "�Œ�" Or .IsFeatureAvailable("��") Then
										empty_slot = empty_slot - .Size
									End If
								End If
							End With
						Next 
				End Select
				
				Do While True
					'�����\�ȃA�C�e���𒲂ׂ�
					ReDim item_list(0)
					For	Each it In IList
						With it
							If Not .Exist Then
								GoTo NextItem
							End If
							
							'�����X���b�g���󂢂Ă���H
							Select Case ipart
								Case "�E��", "����", "�Ў�", "����"
									Select Case .Part
										Case "����"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "�Ў�"
											If u.IsFeatureAvailable("���莝��") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												If Not is_right_hand_available Then
													GoTo NextItem
												End If
											End If
										Case "��"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "��"
									Select Case .Part
										Case "����"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "�Ў�"
											If u.IsFeatureAvailable("���莝��") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												GoTo NextItem
											End If
										Case "��"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "�E��", "����", "��"
									If .Part <> "����" And .Part <> "��" Then
										GoTo NextItem
									End If
									If .Part = "����" Then
										If empty_slot < 2 Then
											GoTo NextItem
										End If
									End If
								Case "�����p�[�c", "�A�C�e��"
									If .Part <> "�����p�[�c" And .Part <> "�A�C�e��" Then
										GoTo NextItem
									End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
								Case Else
									If .Part <> ipart Then
										GoTo NextItem
									End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
							End Select
							
							If Not .Unit_Renamed Is Nothing Then
								With .Unit_Renamed.CurrentForm
									'���E�������j�b�g���������Ă���
									If .Status_Renamed = "���E" Then
										GoTo NextItem
									End If
									
									'�G���j�b�g���������Ă���
									If .Party <> "����" Then
										GoTo NextItem
									End If
								End With
								
								'����Ă���̂ŊO���Ȃ��c�c
								If .IsFeatureAvailable("��") Then
									GoTo NextItem
								End If
							End If
							
							'���ɓo�^�ς݁H
							For i = 1 To UBound(item_list)
								If item_list(i) = .Name Then
									GoTo NextItem
								End If
							Next 
						End With
						
						'�����\�H
						If Not .IsAbleToEquip(it) Then
							GoTo NextItem
						End If
						
						ReDim Preserve item_list(UBound(item_list) + 1)
						item_list(UBound(item_list)) = it.Name
NextItem: 
					Next it
					
					'�����\�ȃA�C�e���̈ꗗ��\��
					ReDim list(UBound(item_list))
					ReDim strkey_list(UBound(item_list))
					ReDim id_list(UBound(item_list))
					ReDim ListItemFlag(UBound(item_list))
					ReDim ListItemComment(UBound(item_list))
					For i = 1 To UBound(item_list)
						iname = item_list(i)
						With IDList.Item(iname)
							list(i) = RightPaddedString(.Nickname, 22) & " "
							
							If .IsFeatureAvailable("��^�A�C�e��") Then
								list(i) = list(i) & RightPaddedString(.Part & "[" & VB6.Format(.Size) & "]", 15)
							Else
								list(i) = list(i) & RightPaddedString(.Part, 15)
							End If
							
							'�A�C�e���̐����J�E���g
							inum = 0
							inum2 = 0
							For	Each it In IList
								With it
									If .Name = iname Then
										If .Exist Then
											If .Unit_Renamed Is Nothing Then
												inum = inum + 1
												inum2 = inum2 + 1
											Else
												If .Unit_Renamed.CurrentForm.Status_Renamed <> "���E" Then
													If Not .IsFeatureAvailable("��") Then
														inum = inum + 1
													End If
												End If
											End If
										End If
									End If
								End With
							Next it
							
							list(i) = list(i) & LeftPaddedString(VB6.Format(inum2), 2) & "/" & LeftPaddedString(VB6.Format(inum), 2)
							
							id_list(i) = .Name
							strkey_list(i) = .KanaName
							ListItemComment(i) = .Comment
						End With
					Next 
					
					'�A�C�e���𖼑O���Ƀ\�[�g
					For i = 1 To UBound(strkey_list) - 1
						max_item = i
						max_str = strkey_list(i)
						For j = i + 1 To UBound(strkey_list)
							If StrComp(strkey_list(j), max_str, 1) = -1 Then
								max_item = j
								max_str = strkey_list(j)
							End If
						Next 
						If max_item <> i Then
							buf = list(i)
							list(i) = list(max_item)
							list(max_item) = buf
							
							buf = id_list(i)
							id_list(i) = id_list(max_item)
							id_list(max_item) = buf
							
							buf = ListItemComment(i)
							ListItemComment(i) = ListItemComment(max_item)
							ListItemComment(max_item) = buf
							
							strkey_list(max_item) = strkey_list(i)
						End If
					Next 
					
					'��������A�C�e���̎�ނ�I��
					caption_str = "��������A�C�e����I�� �F " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("���g��") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("�g�o", u) & "=" & VB6.Format(.MaxHP) & " " & Term("�d�m", u) & "=" & VB6.Format(.MaxEN) & " " & Term("���b", u) & "=" & VB6.Format(.Armor) & " " & Term("�^����", u) & "=" & VB6.Format(.Mobility) & " " & Term("�ړ���", u) & "=" & VB6.Format(.Speed)
					ret = ListBox(caption_str, list, "�A�C�e��               ����            ����", "�A���\��,�R�����g")
					
					'�L�����Z�����ꂽ�H
					If ret = 0 Then
						Exit Do
					End If
					
					iname = id_list(ret)
					
					'�������̃A�C�e�������邩�ǂ����T��
					For	Each it In IList
						With it
							If .Name = iname And .Exist Then
								If .Unit_Renamed Is Nothing Then
									'�������̑��������������̂ł���𑕔�
									If iid <> "" Then
										u.DeleteItem(iid)
									End If
									'�􂢂̃A�C�e���𑕔��c�c
									If .IsFeatureAvailable("��") Then
										MsgBox(.Nickname & "�͎���Ă����I")
									End If
									u.AddItem(it)
									If MapFileName = "" Then
										u.FullRecover()
									End If
									If MainForm.Visible Then
										DisplayUnitStatus(u)
									End If
									Exit Do
								End If
							End If
						End With
					Next it
					
					'�I�����ꂽ�A�C�e�����
					ReDim list(0)
					ReDim id_list(0)
					ReDim ListItemComment(0)
					inum = 0
					If Not IDList.Item(iname).IsFeatureAvailable("��") Then
						For	Each it In IList
							With it
								If .Name <> iname Or Not .Exist Then
									GoTo NextItem2
								End If
								If .Unit_Renamed Is Nothing Then
									GoTo NextItem2
								End If
								With .Unit_Renamed.CurrentForm
									If .Status_Renamed = "���E" Then
										GoTo NextItem2
									End If
									If .Party <> "����" Then
										GoTo NextItem2
									End If
									
									ReDim Preserve list(UBound(list) + 1)
									ReDim Preserve id_list(UBound(list))
									ReDim Preserve ListItemComment(UBound(list))
									
									If Not IsOptionDefined("���g��") And .CountPilot > 0 Then
										list(UBound(list)) = RightPaddedString(.Nickname0, 36) & " " & .MainPilot.Nickname
									Else
										list(UBound(list)) = .Nickname0
									End If
									id_list(UBound(list)) = it.ID
									
									For i = 1 To .CountItem
										With .Item(i)
											If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
												ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
											End If
										End With
									Next 
									
									inum = inum + 1
								End With
							End With
NextItem2: 
						Next it
					End If
					ReDim ListItemFlag(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'�ǂ̃A�C�e���𑕔����邩�I��
					caption_str = IList.Item(id_list(1)).Nickname & "�̓�����I�� �F " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("���g��") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("�g�o", u) & "=" & VB6.Format(.MaxHP) & " " & Term("�d�m", u) & "=" & VB6.Format(.MaxEN) & " " & Term("���b", u) & "=" & VB6.Format(.Armor) & " " & Term("�^����", u) & "=" & VB6.Format(.Mobility) & " " & Term("�ړ���", u) & "=" & VB6.Format(.Speed)
					TopItem = 1
					If IsOptionDefined("���g��") Then
						ret = ListBox(caption_str, list, "���j�b�g", "�A���\��,�R�����g")
					Else
						ret = ListBox(caption_str, list, "���j�b�g                             �p�C���b�g", "�A���\��,�R�����g")
					End If
					
					'�A�C�e��������
					If ret > 0 Then
						If iid <> "" Then
							.DeleteItem(iid)
						End If
						With IList.Item(id_list(ret))
							If Not .Unit_Renamed Is Nothing Then
								.Unit_Renamed.DeleteItem(.ID)
							End If
							
							'�􂢂̃A�C�e���𑕔��c�c
							If .IsFeatureAvailable("��") Then
								MsgBox(.Nickname & "�͎���Ă����I")
							End If
						End With
						.AddItem(IList.Item(id_list(ret)))
						If MapFileName = "" Then
							.FullRecover()
						End If
						If MainForm.Visible Then
							DisplayUnitStatus(u)
						End If
						Exit Do
					End If
NextLoop: 
				Loop 
NextLoop2: 
			Loop 
		End With
		
		'���j�b�g�����炩���ߑI������Ă���ꍇ
		'(���j�b�g�X�e�[�^�X����̃A�C�e��������)
		If Not selected_unit Is Nothing Then
			With frmListBox
				.Hide()
				If .txtComment.Enabled Then
					.txtComment.Enabled = False
					.txtComment.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
				End If
			End With
			ReduceListBoxHeight()
			EnlargeListBoxWidth()
			Exit Sub
		End If
		
		GoTo Beginning
	End Sub
	
	'�����R�}���h
	' MOD START MARGE
	'Public Sub ExchangeFormCommand()
	Private Sub ExchangeFormCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim key_list() As Integer
		Dim list2() As String
		Dim id_list2() As String
		Dim max_item, max_value As Short
		Dim u As Unit
		Dim buf As String
		Dim ret As Short
		Dim top_item As Short
		Dim farray() As String
		
Beginning: 
		
		top_item = 1
		
		'�����\�ȃ��j�b�g�̃��X�g���쐬
		ReDim list(0)
		ReDim id_list(0)
		ReDim ListItemComment(0)
		For	Each u In UList
			With u
				'�ҋ@���̖������j�b�g�H
				If .Party0 <> "����" Or .Status_Renamed <> "�ҋ@" Then
					GoTo NextLoop
				End If
				
				'�����\�͂������Ă���H
				If Not .IsFeatureAvailable("����") Then
					GoTo NextLoop
				End If
				
				'�����ꂩ�̌`�ԂɊ����\�H
				For i = 1 To LLength(.FeatureData("����"))
					If .OtherForm(LIndex(.FeatureData("����"), i)).IsAvailable Then
						Exit For
					End If
				Next 
				If i > LLength(.FeatureData("����")) Then
					GoTo NextLoop
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'���j�b�g�̃X�e�[�^�X��\��
				If IsOptionDefined("���g��") Then
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, 37) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, 37) & VB6.Format(.Rank)
					End If
				Else
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, 33) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, 33) & VB6.Format(.Rank)
					End If
				End If
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5)
				If .CountPilot > 0 Then
					If IsOptionDefined("���g��") Then
						list(UBound(list)) = list(UBound(list)) & "  " & LeftPaddedString(VB6.Format(.MainPilot.Level), 6)
					Else
						list(UBound(list)) = list(UBound(list)) & "  " & .MainPilot.Nickname
					End If
				End If
				
				'���j�b�g�ɑ�������Ă���A�C�e�����R�����g���ɗ�L
				For k = 1 To .CountItem
					With .Item(k)
						If (.Class_Renamed() <> "�Œ�" Or Not .IsFeatureAvailable("��\��")) And .Part <> "��\��" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						End If
					End With
				Next 
				
				'���j�b�g�h�c���L�^���Ă���
				id_list(UBound(list)) = .ID
			End With
NextLoop: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
		'���X�g�����j�b�g�̂g�o�Ń\�[�g
		ReDim key_list(UBound(list))
		With UList
			For i = 1 To UBound(list)
				key_list(i) = .Item(id_list(i)).MaxHP
			Next 
		End With
		For i = 1 To UBound(list) - 1
			max_item = i
			max_value = key_list(i)
			For j = i + 1 To UBound(list)
				If key_list(j) > max_value Then
					max_item = j
					max_value = key_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = list(i)
				list(i) = list(max_item)
				list(max_item) = buf
				
				buf = id_list(i)
				id_list(i) = id_list(max_item)
				id_list(max_item) = buf
				
				buf = ListItemComment(i)
				ListItemComment(i) = ListItemComment(max_item)
				ListItemComment(max_item) = buf
				
				key_list(max_item) = key_list(i)
			End If
		Next 
		
		'�������郆�j�b�g��I��
		TopItem = top_item
		If IsOptionDefined("���g��") Then
			ret = ListBox("���j�b�g�I��", list, "���j�b�g                         " & Term("�����N", Nothing, 6) & "   " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " ���x��", "�A���\��,�R�����g")
		Else
			ret = ListBox("���j�b�g�I��", list, "���j�b�g                     " & Term("�����N", Nothing, 6) & "   " & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " �p�C���b�g", "�A���\��,�R�����g")
		End If
		top_item = TopItem
		
		'�L�����Z���H
		If ret = 0 Then
			Exit Sub
		End If
		
		'�I�����ꂽ���j�b�g������
		u = UList.Item(id_list(ret))
		
		'�����\�Ȍ`�Ԃ̃��X�g���쐬
		With u
			buf = .FeatureData("����")
			ReDim list2(0)
			ReDim id_list2(0)
			ReDim ListItemComment(0)
			For i = 1 To LLength(buf)
				With .OtherForm(LIndex(buf, i))
					If .IsAvailable Then
						ReDim Preserve list2(UBound(list2) + 1)
						ReDim Preserve id_list2(UBound(list2))
						ReDim Preserve ListItemComment(UBound(list2))
						
						'���j�b�g�����N�����킹��
						.Rank = u.Rank
						.BossRank = u.BossRank
						.Update()
						
						'������̃��X�g���쐬
						id_list2(UBound(list2)) = .Name
						If u.Nickname0 = .Nickname Then
							list2(UBound(list2)) = RightPaddedString(.Name, 27)
						Else
							list2(UBound(list2)) = RightPaddedString(.Nickname0, 27)
						End If
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5) & " " & .Data.Adaption
						
						'�ő�U����
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "��") Then
								If .WeaponPower(j, "") > max_value Then
									max_value = .WeaponPower(j, "")
								End If
							End If
						Next 
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 7)
						
						'�ő�˒�
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "��") Then
								If .WeaponMaxRange(j) > max_value Then
									max_value = .WeaponMaxRange(j)
								End If
							End If
						Next 
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 5)
						
						'�����悪������\�͈ꗗ
						ReDim farray(0)
						For j = 1 To .CountFeature
							If .FeatureName(j) <> "" Then
								'�d���������\�͕͂\�����Ȃ��悤�`�F�b�N
								For k = 1 To UBound(farray)
									If .FeatureName(j) = farray(k) Then
										Exit For
									End If
								Next 
								If k > UBound(farray) Then
									ListItemComment(UBound(list2)) = ListItemComment(UBound(list2)) & .FeatureName(j) & " "
									ReDim Preserve farray(UBound(farray) + 1)
									farray(UBound(farray)) = .FeatureName(j)
								End If
							End If
						Next 
					End If
				End With
			Next 
			ReDim ListItemFlag(UBound(list2))
			
			'������̌`�Ԃ�I��
			TopItem = 1
			ret = ListBox("�ύX��I��", list2, "���j�b�g                     " & Term("�g�o", u, 4) & " " & Term("�d�m", u, 4) & " " & Term("���b", u, 4) & " " & Term("�^��", u, 4) & " �K�� �U���� �˒�", "�A���\��,�R�����g")
			
			'�L�����Z���H
			If ret = 0 Then
				GoTo Beginning
			End If
			
			'���������{
			.Transform(id_list2(ret))
		End With
		
		GoTo Beginning
	End Sub
	
	'�X�e�[�^�X�R�}���h�����ǂ�����Ԃ�
	Public Function InStatusCommand() As Boolean
		If MapFileName = "" Then
			If InStr(ScenarioFileName, "\���j�b�g�X�e�[�^�X�\��.eve") > 0 Or InStr(ScenarioFileName, "\�p�C���b�g�X�e�[�^�X�\��.eve") > 0 Or IsSubStage Then
				InStatusCommand = True
			End If
		End If
	End Function
End Module