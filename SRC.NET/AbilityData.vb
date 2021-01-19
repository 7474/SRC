Option Strict Off
Option Explicit On
Friend Class AbilityData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�A�r���e�B�f�[�^�̃N���X
	
	'����
	Public Name As String
	'�g�p�\��
	Public Stock As Short
	'�d�m�����
	Public ENConsumption As Short
	'�K�v�C��
	Public NecessaryMorale As Short
	'�ŏ��˒�
	Public MinRange As Short
	'�ő�˒�
	Public MaxRange As Short
	'����
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Class_Renamed As String
	'�K�v�Z�\
	Public NecessarySkill As String
	'�K�v����
	Public NecessaryCondition As String
	
	'����
	Private colEffects As New Collection
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colEffects
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colEffects ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colEffects = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�A�r���e�B����
	Public Function Nickname() As String
		Nickname = Name
		ReplaceSubExpression(Nickname)
		If InStr(Nickname, "(") > 0 Then
			Nickname = Left(Nickname, InStr(Nickname, "(") - 1)
		End If
	End Function
	
	'�A�r���e�B�Ɍ��ʂ�ǉ�
	Public Sub SetEffect(ByRef elist As String)
		Dim j, i, k As Short
		Dim buf As String
		Dim dat, dat2 As AbilityEffect
		Dim elevel, etype, edata As String
		
		TrimString(elist)
		For i = 1 To ListLength(elist)
			dat = NewAbilityEffect
			With dat
				buf = ListIndex(elist, i)
				j = InStr(buf, "Lv")
				k = InStr(buf, "=")
				If j > 0 And (k = 0 Or j < k) Then
					'���x���w��̂������(�f�[�^�w�肪������̂��܂�)
					.Name = Left(buf, j - 1)
					If k > 0 Then
						'�f�[�^�w�肪�������
						.Level = CDbl(Mid(buf, j + 2, k - (j + 2)))
						buf = Mid(buf, k + 1)
						If Left(buf, 1) = """" Then
							buf = Mid(buf, 2, Len(buf) - 2)
						End If
						
						j = InStr(buf, "Lv")
						k = InStr(buf, "=")
						
						If j > 0 And (k = 0 Or j < k) Then
							'�f�[�^�w����Ƀ��x���w�肪����
							etype = Left(buf, j - 1)
							If k > 0 Then
								elevel = Mid(buf, j + 2, k - (j + 2))
								edata = Mid(buf, k + 1)
							Else
								elevel = Mid(buf, j + 2)
								edata = ""
							End If
						ElseIf k > 0 Then 
							'�f�[�^�w����Ƀf�[�^�w�肪����
							etype = Left(buf, k - 1)
							elevel = ""
							edata = Mid(buf, k + 1)
						Else
							'�P���ȃf�[�^�w��
							etype = buf
							elevel = ""
							edata = ""
						End If
						
						If .Name = "�t��" And elevel = "" Then
							elevel = VB6.Format(DEFAULT_LEVEL)
						End If
						
						.Data = Trim(etype & " " & elevel & " " & edata)
					Else
						'�f�[�^�w�肪�Ȃ�����
						.Level = CDbl(Mid(buf, j + 2))
					End If
				ElseIf k > 0 Then 
					'�f�[�^�w����܂ތ���
					.Name = Left(buf, k - 1)
					buf = Mid(buf, k + 1)
					If Asc(buf) = 34 Then '"
						buf = Mid(buf, 2, Len(buf) - 2)
					End If
					
					j = InStr(buf, "Lv")
					k = InStr(buf, "=")
					
					If .Name = "���" Then
						'����̎w��
						etype = buf
						elevel = ""
						edata = ""
					ElseIf j > 0 Then 
						'�f�[�^�w����Ƀ��x���w�肪����
						etype = Left(buf, j - 1)
						If k > 0 Then
							elevel = Mid(buf, j + 2, k - (j + 2))
							edata = Mid(buf, k + 1)
						Else
							elevel = Mid(buf, j + 2)
							edata = ""
						End If
					ElseIf k > 0 Then 
						'�f�[�^�w����Ƀf�[�^�w�肪����
						etype = Left(buf, k - 1)
						elevel = ""
						edata = Mid(buf, k + 1)
					Else
						'�P���ȃf�[�^�w��
						etype = buf
						elevel = ""
						edata = ""
					End If
					
					If .Name = "�t��" And elevel = "" Then
						elevel = VB6.Format(DEFAULT_LEVEL)
					End If
					
					.Data = Trim(etype & " " & elevel & " " & edata)
				Else
					'���ʖ��̂�
					.Name = buf
				End If
				
				j = 1
				For	Each dat2 In colEffects
					If .Name = dat2.Name Then
						j = j + 1
					End If
				Next dat2
				If j = 1 Then
					colEffects.Add(dat, .Name)
				Else
					colEffects.Add(dat, .Name & VB6.Format(j))
				End If
			End With
		Next 
	End Sub
	
	Private Function NewAbilityEffect() As AbilityEffect
		Dim dat As New AbilityEffect
		NewAbilityEffect = dat
	End Function
	
	'���ʂ̑���
	Public Function CountEffect() As Short
		CountEffect = colEffects.Count()
	End Function
	
	'���ʂ̎��
	Public Function EffectType(ByRef Index As Object) As String
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectType = colEffects.Item(Index).Name
	End Function
	
	'���ʂ̃��x��
	Public Function EffectLevel(ByRef Index As Object) As Double
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Level �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectLevel = colEffects.Item(Index).Level
	End Function
	
	'���ʂ̃f�[�^
	Public Function EffectData(ByRef Index As Object) As String
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Data �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectData = colEffects.Item(Index).Data
	End Function
	
	'���ʓ��e�̉��
	Public Function EffectName(ByRef Index As Object) As String
		Dim ae As AbilityEffect
		Dim elevel, elevel2 As Double
		Dim uname, wclass As String
		Dim flevel As Double
		Dim heal_lv, supply_lv As Double
		Dim i As Short
		Dim buf As String
		Dim cname, aname As String
		
		ae = colEffects.Item(Index)
		With ae
			'���ʃ��x�����񕜁E�����ʂ��Ӗ�����A�r���e�B�p
			elevel = .Level
			'���ʃ��x�����^�[�������Ӗ�����A�r���e�B�y�я����A�r���e�B�p
			elevel2 = elevel
			
			If SelectedUnit.CountPilot > 0 Then
				With SelectedUnit.MainPilot
					'���ӋZ
					If .IsSkillAvailable("���ӋZ") Then
						buf = .SkillData("���ӋZ")
						For i = 1 To Len(buf)
							If InStrNotNest(Class_Renamed, GetClassBundle(buf, i)) > 0 Then
								elevel = 1.2 * elevel
								elevel2 = 1.4 * elevel2
								Exit For
							End If
						Next 
					End If
					
					'�s����
					If .IsSkillAvailable("�s����") Then
						buf = .SkillData("�s����")
						For i = 1 To Len(buf)
							If InStrNotNest(Class_Renamed, GetClassBundle(buf, i)) > 0 Then
								elevel = 0.8 * elevel
								elevel2 = 0.6 * elevel2
								Exit For
							End If
						Next 
					End If
					
					'�p�A�r���e�B�̏ꍇ�͖��͂ɂ���Č��ʃ��x�����C�����󂯂�
					If InStrNotNest(Class_Renamed, "�p") > 0 Then
						elevel = elevel * .Shooting / 100
					Else
						For i = 1 To LLength(NecessarySkill)
							If .SkillType(LIndex(NecessarySkill, i)) = "�p" Then
								elevel = elevel * .Shooting / 100
								Exit For
							End If
						Next 
					End If
					
					'�C�����⋋�Z�\
					heal_lv = .SkillLevel("�C��")
					supply_lv = .SkillLevel("�⋋")
				End With
			End If
			
			'�A�r���e�B�̌��ʂ͍Œ�ł��P�^�[������
			If elevel2 <> 0 Then
				elevel2 = MaxLng(elevel2, 1)
			End If
			
			Select Case .Name
				Case "��"
					EffectName = Term("�g�o") & "��"
					If elevel > 0 Then
						EffectName = EffectName & VB6.Format(CInt(500 * elevel * (10 + heal_lv) \ 10)) & "��"
					ElseIf elevel < 0 Then 
						EffectName = EffectName & VB6.Format(CInt(-500 * elevel)) & "����"
					End If
					
				Case "�⋋"
					EffectName = Term("�d�m") & "��"
					If elevel > 0 Then
						EffectName = EffectName & VB6.Format(CInt(50 * elevel * (10 + supply_lv)) \ 10) & "��"
					ElseIf elevel < 0 Then 
						EffectName = EffectName & VB6.Format(CInt(-50 * elevel)) & "����"
					End If
					
				Case "��͉�", "�v���[�i��"
					If SelectedUnit.CountPilot > 0 Then
						EffectName = SelectedUnit.MainPilot.SkillName0("���")
					Else
						EffectName = "���"
					End If
					If elevel > 0 Then
						EffectName = EffectName & "��" & VB6.Format(CInt(10 * elevel)) & "��"
					ElseIf elevel < 0 Then 
						EffectName = EffectName & "��" & VB6.Format(CInt(-10 * elevel)) & "����"
					End If
					
				Case "�r�o��"
					EffectName = Term("�r�o") & "��"
					If elevel > 0 Then
						EffectName = EffectName & VB6.Format(CInt(10 * elevel)) & "��"
					ElseIf elevel < 0 Then 
						EffectName = EffectName & VB6.Format(CInt(-10 * elevel)) & "����"
					End If
					
				Case "�C�͑���"
					EffectName = Term("�C��") & "��"
					If elevel > 0 Then
						EffectName = EffectName & VB6.Format(CInt(10 * elevel)) & "����"
					ElseIf elevel < 0 Then 
						EffectName = EffectName & VB6.Format(CInt(-10 * elevel)) & "����"
					End If
					
				Case "���U"
					If Len(.Data) = 0 Then
						EffectName = "����̒e������"
					Else
						For i = 1 To SelectedUnit.CountWeapon
							If SelectedUnit.WeaponNickname(i) = .Data Then
								EffectName = .Data & "�̒e������"
								Exit Function
							End If
						Next 
						EffectName = .Data & "������������̒e������"
					End If
					
				Case "����"
					For i = 1 To LLength(.Data)
						cname = LIndex(.Data, i)
						Select Case cname
							Case "���b��"
								cname = Term("���b") & "��"
							Case "�^�����t�o"
								cname = Term("�^����") & "�t�o"
							Case "�^�����c�n�v�m"
								cname = Term("�^����") & "�c�n�v�m"
							Case "�ړ��͂t�o"
								cname = Term("�ړ���") & "�t�o"
							Case "�ړ��͂c�n�v�m"
								cname = Term("�ړ���") & "�c�n�v�m"
						End Select
						EffectName = EffectName & " " & cname
					Next 
					EffectName = Trim(EffectName)
					If Len(EffectName) > 0 Then
						EffectName = EffectName & "����"
					Else
						EffectName = "��ԉ�"
					End If
					
				Case "���"
					cname = LIndex(.Data, 1)
					Select Case cname
						Case "���b��"
							cname = Term("���b") & "��"
						Case "�^�����t�o"
							cname = Term("�^����") & "�t�o"
						Case "�^�����c�n�v�m"
							cname = Term("�^����") & "�c�n�v�m"
						Case "�ړ��͂t�o"
							cname = Term("�ړ���") & "�t�o"
						Case "�ړ��͂c�n�v�m"
							cname = Term("�ړ���") & "�c�n�v�m"
					End Select
					If 0 < elevel2 And elevel2 <= 10 Then
						EffectName = cname & "(" & VB6.Format(CInt(elevel2)) & "�^�[��)"
					Else
						EffectName = cname
					End If
					
				Case "�t��"
					Select Case LIndex(.Data, 1)
						Case "�ϐ�"
							aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
							If aname = "" Then
								aname = LIndex(.Data, 3) & "�U��"
							End If
							EffectName = aname & "�̃_���[�W�𔼌�"
						Case "������"
							aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
							If aname = "" Then
								aname = LIndex(.Data, 3) & "�U��"
							End If
							EffectName = aname & "�𖳌���"
						Case "������ʖ�����"
							aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
							If aname = "" Then
								aname = LIndex(.Data, 3) & "�U��"
							End If
							EffectName = aname & "�̓�����ʂ𖳌���"
						Case "�z��"
							aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
							If aname = "" Then
								aname = LIndex(.Data, 3) & "�U��"
							End If
							EffectName = aname & "���z��"
						Case "�ǉ��p�C���b�g"
							EffectName = "�p�C���b�g�ω�"
						Case "�ǉ��T�|�[�g"
							EffectName = "�T�|�[�g�ǉ�"
						Case "���i�ύX"
							EffectName = "�p�C���b�g�̐��i��" & LIndex(.Data, 3) & "�ɕύX"
						Case "�a�f�l"
							EffectName = "�a�f�l�ύX"
						Case "�U������"
							For i = 4 To LLength(.Data)
								If InStr(LIndex(.Data, i), "!") = 0 Then
									wclass = wclass & LIndex(.Data, i)
								End If
							Next 
							EffectName = WeaponType(wclass) & "�̑�����" & LIndex(.Data, 3) & "��ǉ�"
						Case "���틭��"
							For i = 3 To LLength(.Data)
								If InStr(LIndex(.Data, i), "!") = 0 Then
									wclass = wclass & LIndex(.Data, i)
								End If
							Next 
							flevel = StrToDbl(LIndex(.Data, 2))
							If flevel >= 0 Then
								EffectName = WeaponType(wclass) & "�̍U���͂�+" & VB6.Format(100 * flevel)
							Else
								EffectName = WeaponType(wclass) & "�̍U���͂�" & VB6.Format(100 * flevel)
							End If
						Case "����������"
							For i = 3 To LLength(.Data)
								If InStr(LIndex(.Data, i), "!") = 0 Then
									wclass = wclass & LIndex(.Data, i)
								End If
							Next 
							flevel = StrToDbl(LIndex(.Data, 2))
							If flevel >= 0 Then
								EffectName = WeaponType(wclass) & "�̖�������+" & VB6.Format(5 * flevel)
							Else
								EffectName = WeaponType(wclass) & "�̖�������" & VB6.Format(5 * flevel)
							End If
						Case "�b�s������", "������ʔ���������"
							For i = 3 To LLength(.Data)
								If InStr(LIndex(.Data, i), "!") = 0 Then
									wclass = wclass & LIndex(.Data, i)
								End If
							Next 
							flevel = StrToDbl(LIndex(.Data, 2))
							If flevel >= 0 Then
								EffectName = WeaponType(wclass) & "�̂b�s����+" & VB6.Format(5 * flevel)
							Else
								EffectName = WeaponType(wclass) & "�̂b�s����" & VB6.Format(5 * flevel)
							End If
						Case "�˒�����"
							For i = 3 To LLength(.Data)
								If InStr(LIndex(.Data, i), "!") = 0 Then
									wclass = wclass & LIndex(.Data, i)
								End If
							Next 
							flevel = StrToLng(LIndex(.Data, 2))
							If flevel >= 0 Then
								EffectName = WeaponType(wclass) & "�̎˒���+" & VB6.Format(flevel)
							Else
								EffectName = WeaponType(wclass) & "�̎˒���" & VB6.Format(flevel)
							End If
						Case "�T�C�Y�ύX"
							EffectName = "�T�C�Y��" & LIndex(.Data, 3) & "�ɕω�"
						Case "�n�`�K���ύX"
							If StrToLng(LIndex(.Data, 3)) > 0 Then
								EffectName = "��ւ̓K��������"
							ElseIf StrToLng(LIndex(.Data, 4)) > 0 Then 
								EffectName = "���ւ̓K��������"
							ElseIf StrToLng(LIndex(.Data, 5)) > 0 Then 
								EffectName = "�����ւ̓K��������"
							ElseIf StrToLng(LIndex(.Data, 6)) > 0 Then 
								EffectName = "�F���ւ̓K��������"
							End If
						Case "�n�`�K���Œ�ύX"
							If StrToLng(LIndex(.Data, 3)) <= 5 And StrToLng(LIndex(.Data, 3)) >= 0 Then
								If LIndex(.Data, 6) = "����" Then
									EffectName = "��ւ̓K���������I�ɕω�"
								Else
									EffectName = "��ւ̓K����ω�"
								End If
							ElseIf StrToLng(LIndex(.Data, 4)) <= 5 And StrToLng(LIndex(.Data, 4)) >= 0 Then 
								If LIndex(.Data, 6) = "����" Then
									EffectName = "���ւ̓K���������I�ɕω�"
								Else
									EffectName = "���ւ̓K����ω�"
								End If
							ElseIf StrToLng(LIndex(.Data, 5)) <= 5 And StrToLng(LIndex(.Data, 5)) >= 0 Then 
								If LIndex(.Data, 6) = "����" Then
									EffectName = "�����ւ̓K���������I�ɕω�"
								Else
									EffectName = "�����ւ̓K����ω�"
								End If
							ElseIf StrToLng(LIndex(.Data, 6)) <= 5 And StrToLng(LIndex(.Data, 6)) >= 0 Then 
								If LIndex(.Data, 6) = "����" Then
									EffectName = "�F���ւ̓K���������I�ɕω�"
								Else
									EffectName = "�F���ւ̓K����ω�"
								End If
							End If
						Case "�u�|�t�o"
							Select Case LIndex(.Data, 3)
								Case "����"
									EffectName = "����U���͂�����"
								Case "���j�b�g"
									EffectName = "�e�p�����[�^������"
								Case Else
									EffectName = "���j�b�g������"
							End Select
						Case "�i������", "�}������", "��������"
							EffectName = EffectName & "�t��"
						Case "�p�C���b�g����", "�p�C���b�g�摜", "���̕ύX", "���j�b�g�摜"
							EffectName = ""
						Case Else
							EffectName = ListIndex(.Data, 3)
							If Left(EffectName, 1) = """" Then
								EffectName = ListIndex(Mid(EffectName, 2, Len(EffectName) - 2), 1)
							End If
							If EffectName = "" Or EffectName = "��\��" Then
								If LIndex(.Data, 2) <> VB6.Format(DEFAULT_LEVEL) And LLength(.Data) <= 3 Then
									EffectName = LIndex(.Data, 1) & "Lv" & LIndex(.Data, 2) & "�t��"
								Else
									EffectName = LIndex(.Data, 1) & "�t��"
								End If
							Else
								If LIndex(.Data, 2) <> VB6.Format(DEFAULT_LEVEL) And LLength(.Data) <= 3 Then
									EffectName = EffectName & "Lv" & LIndex(.Data, 2)
								End If
								EffectName = EffectName & "�t��"
							End If
					End Select
					If EffectName <> "" Then
						If 0 < elevel2 And elevel2 <= 10 Then
							EffectName = EffectName & "(" & VB6.Format(CInt(elevel2)) & "�^�[��)"
						End If
					End If
					
				Case "����"
					EffectName = ListIndex(.Data, 3)
					If EffectName = "" Or EffectName = "��\��" Then
						If StrToLng(LIndex(.Data, 2)) > 0 Then
							EffectName = LIndex(.Data, 1) & "Lv" & LIndex(.Data, 2)
						Else
							EffectName = LIndex(.Data, 1)
						End If
					End If
					If 0 < elevel2 And elevel2 <= 10 Then
						EffectName = EffectName & "����(" & VB6.Format(CInt(elevel2)) & "�^�[��)"
					End If
					
				Case "����"
					If Not UDList.IsDefined(.Data) Then
						ErrorMessage("�������j�b�g�u" & .Data & "�v����`����Ă��܂���")
						Exit Function
					End If
					If elevel2 > 1 Then
						EffectName = UDList.Item(.Data).Nickname & "��" & StrConv(VB6.Format(CInt(elevel2)), VbStrConv.Wide) & "�̏���"
					Else
						EffectName = UDList.Item(.Data).Nickname & "������"
					End If
					
				Case "�ϐg"
					uname = LIndex(.Data, 1)
					If Not UDList.IsDefined(uname) Then
						ErrorMessage("�ϐg��̃f�[�^�u" & uname & "�v����`����Ă��܂���")
						Exit Function
					End If
					If 0 < elevel2 And elevel2 <= 10 Then
						EffectName = UDList.Item(uname).Nickname & "�ɕϐg" & "(" & VB6.Format(CInt(elevel2)) & "�^�[��)"
					Else
						EffectName = UDList.Item(uname).Nickname & "�ɕϐg"
					End If
					
				Case "�\�̓R�s�["
					If 0 < elevel2 And elevel2 <= 10 Then
						EffectName = "�C�ӂ̖������j�b�g�ɕϐg" & "(" & VB6.Format(CInt(elevel2)) & "�^�[��)"
					Else
						EffectName = "�C�ӂ̖������j�b�g�ɕϐg"
					End If
					
				Case "�čs��"
					If MaxRange <> 0 Then
						EffectName = "�s���ς݃��j�b�g���čs��"
					Else
						EffectName = "�s�������"
					End If
					
				Case "���"
					EffectName = .Data
					
			End Select
		End With
	End Function
	
	'�t�����镐�틭���n�\�͂̑Ώە\���p�ɕ���̎�ނ𔻒�
	Private Function WeaponType(ByRef wclass As String) As String
		If wclass = "�S" Or wclass = "" Then
			WeaponType = "����"
		ElseIf Len(wclass) = 1 Then 
			WeaponType = AttributeName(Nothing, wclass)
		ElseIf Right(wclass, 2) = "����" Then 
			WeaponType = Left(wclass, Len(wclass) - 2)
		Else
			WeaponType = wclass & "�����U��"
		End If
	End Function
	
	'�g���̂ăA�C�e���ɂ��A�r���e�B���ǂ�����Ԃ�
	Public Function IsItem() As Boolean
		Dim i As Short
		
		For i = 1 To LLength(NecessarySkill)
			If LIndex(NecessarySkill, i) = "�A�C�e��" Then
				IsItem = True
				Exit Function
			End If
		Next 
	End Function
End Class