Option Strict Off
Option Explicit On
Module Help
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'����\�́����푮���̉���\�����s�����W���[��
	
	
	'�p�C���b�g p �̓���\�͂̉����\��
	Public Sub SkillHelp(ByRef p As Pilot, ByRef sindex As String)
		Dim stype, sname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		'����\�̖͂��̂𒲂ׂ�
		If IsNumeric(sindex) Then
			sname = p.SkillName(CShort(sindex))
		Else
			'�t�����ꂽ�p�C���b�g�p����\��
			If InStr(sindex, "Lv") > 0 Then
				stype = Left(sindex, InStr(sindex, "Lv") - 1)
			Else
				stype = sindex
			End If
			sname = p.SkillName(stype)
		End If
		
		msg = SkillHelpMessage(p, sindex)
		
		'����̕\��
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("���b�Z�[�W�E�B���h�E")
			End If
			DisplayMessage("�V�X�e��", "<b>" & sname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'�p�C���b�g p �̓���\�͂̉��
	Public Function SkillHelpMessage(ByRef p As Pilot, ByRef sindex As String) As String
		Dim sname, stype, sname0 As String
		Dim slevel As Double
		Dim sdata As String
		Dim is_level_specified As Boolean
		Dim msg As String
		Dim u, u2 As Unit
		Dim uname, fdata As String
		Dim i As Short
		
		'����\�̖͂��́A���x���A�f�[�^�𒲂ׂ�
		With p
			If IsNumeric(sindex) Then
				stype = .Skill(CShort(sindex))
				slevel = .SkillLevel(CShort(sindex))
				sdata = .SkillData(CShort(sindex))
				sname = .SkillName(CShort(sindex))
				sname0 = .SkillName0(CShort(sindex))
				is_level_specified = .IsSkillLevelSpecified(CShort(sindex))
			Else
				'�t�����ꂽ�p�C���b�g�p����\��
				If InStr(sindex, "Lv") > 0 Then
					stype = Left(sindex, InStr(sindex, "Lv") - 1)
				Else
					stype = sindex
				End If
				stype = .SkillType(stype)
				slevel = .SkillLevel(stype)
				sdata = .SkillData(stype)
				sname = .SkillName(stype)
				sname0 = .SkillName0(stype)
				is_level_specified = .IsSkillLevelSpecified(stype)
			End If
			
			'�p�C���b�g������Ă��郆�j�b�g
			u = .Unit_Renamed
			If u.Name = "�X�e�[�^�X�\���p�_�~�[���j�b�g" Then
				If IsLocalVariableDefined("���惆�j�b�g[" & .ID & "]") Then
					'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					uname = LocalVariableList.Item("���惆�j�b�g[" & .ID & "]").StringValue
					If uname <> "" Then
						u2 = u
						u = UList.Item(uname)
					End If
				End If
			End If
		End With
		
		Select Case stype
			Case "�I�[��"
				If u.FeatureName0("�o���A") = "�I�[���o���A" Then
					msg = "�I�[���Z�u�I�v�̍U���͂�" & u.FeatureName0("�I�[���o���A") & "�̋��x��" & VB6.Format(CInt(100 * slevel)) & "�̏C����^����B"
				Else
					msg = "�I�[���Z�u�I�v�̍U���͂̋��x��" & VB6.Format(CInt(100 * slevel)) & "�̏C����^����B"
				End If
				If u.IsFeatureAvailable("�I�[���ϊ���") Then
					msg = msg & "�܂��A" & Term("�g�o", u) & "�A" & Term("�d�m", u) & "�A" & Term("���b", u) & "�A" & Term("�^����") & "�����x���ɍ��킹�Ă��ꂼ�ꑝ������B"
				End If
				
			Case "���g"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "% �̊m���ŕ��g���A�U�����������B"
				
			Case "�����o"
				msg = Term("����", u) & "�E" & Term("���", u)
				If slevel > 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel + 3)) & " �̏C����^����B"
				Else
					msg = msg & "�� +0 �̏C����^����B"
				End If
				If slevel > 3 Then
					msg = msg & ";�v�O�U���U��(�T)�̎˒���" & VB6.Format(CInt(slevel \ 4)) & "������������B"
				End If
				
			Case "�m�o����"
				msg = Term("����", u) & "�E" & Term("���", u)
				If slevel > 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel + 3)) & " �̏C����^����B;"
				Else
					msg = msg & "�� +0 �̏C����^����B;"
				End If
				If slevel > 3 Then
					msg = msg & "�v�O�U���U��(�T)�̎˒���" & VB6.Format(CInt(slevel \ 4)) & "������������B"
				End If
				msg = msg & "���_�s����ɂ��" & Term("�r�o", u) & "����ʂ�20%��������"
				
			Case "�O��"
				msg = Term("����", u) & "�E" & Term("���", u)
				If slevel > 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel + 3)) & " �̏C����^����B"
				Else
					msg = msg & "�� +0 �̏C����^����B"
				End If
				
			Case "�؂蕥��"
				msg = "�i������(��)�A�ːi�Z(��)�A���e�U��(��)�ɂ��U���� " & VB6.Format(CInt(100 * slevel \ 16)) & "% �̊m���Ő؂蕥���ĉ������B"
				
			Case "�}��"
				msg = "���e�U��(��)�ɂ��U���� " & VB6.Format(CInt(100 * slevel \ 16)) & "% �̊m���Ō}������B"
				
			Case "�T�C�{�[�O"
				msg = Term("����", u) & "�E" & Term("���", u)
				msg = msg & "�� +5 �̏C����^����B"
				
			Case "�r�h��"
				If u.IsFeatureAvailable("��") Then
					msg = "�V�[���h�h����s���A�_���[�W��" & VB6.Format(CInt(100 * slevel + 400)) & "����������B"
				Else
					msg = VB6.Format(CInt(100 * slevel \ 16)) & "% �̊m���ŃV�[���h�h����s���B"
				End If
				
			Case "�����l��"
				msg = "�G��|�������ɓ�����" & Term("����")
				If Not is_level_specified Then
					msg = msg & "�� 50% ��������B"
				ElseIf slevel >= 0 Then 
					msg = msg & "�� " & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = msg & "�� " & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "��"
				msg = "�򉻋Z(��)���g�����ƂœG��" & p.SkillName0("�Đ�") & "�\�͂𖳌����B"
				
			Case "������"
				If u.IsHero Then
					msg = "�����ɂ��"
				Else
					msg = "�@�̂ɓ�����"
				End If
				msg = msg & Term("�^����", u) & "�E�U���͂���������B"
				
			Case "����������"
				If slevel >= 0 Then
					msg = p.SkillName0("������") & "�̐������� " & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = p.SkillName0("������") & "�̐������� " & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "���"
				msg = "���݂�" & sname0 & "�l�ɂ��킹��" & Term("�g�o", u) & "�E" & Term("�d�m", u) & "�E" & Term("���b", u) & "�E" & Term("�ړ���", u) & "����������B"
				
			Case "��͐���"
				If slevel >= 0 Then
					msg = p.SkillName0("���") & "�̐������� " & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = p.SkillName0("���") & "�̐������� " & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "���"
				msg = Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/4�ȉ��̎��ɔ����B;" & "��������� +30%�A�N���e�B�J�������� +50%�B"
				
			Case "�����"
				msg = Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/4�ȉ��̎��ɔ����B;" & "��������� +50%�A�N���e�B�J�������� +50%�B"
				
			Case "�o��"
				msg = Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/4�ȉ��̎��ɔ����B;"
				If IsOptionDefined("�_���[�W�{���ቺ") Then
					msg = msg & "�U����10%�A�b�v�A�N���e�B�J�������� +50%�B"
				Else
					msg = msg & "�U����1.2�{�A�N���e�B�J�������� +50%�B"
				End If
				
			Case "�s��"
				msg = Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/2�ȉ��̎��ɔ����B;" & "�������ɉ����Ėh��͂���������B"
				
			Case "�f��"
				If Not is_level_specified Then
					msg = "���肷��o���l��50%��������B"
				ElseIf slevel >= 0 Then 
					msg = "���肷��o���l�� " & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = "���肷��o���l�� " & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "�x����"
				msg = "���肷��o���l����������B"
				
			Case "�Đ�", "�p�Y"
				msg = Term("�g�o", u) & "���O�ɂȂ�������" & VB6.Format(CInt(100 * slevel \ 16)) & "%�̊m���ŕ�������B"
				
			Case "���\��"
				msg = Term("����", u) & "�E" & Term("���", u) & "�E" & Term("�b�s��", u) & "�ɂ��ꂼ�� +5�B;" & "�T�C�L�b�N�U��(��)�̍U���͂� +" & VB6.Format(CInt(100 * slevel)) & "�B;" & Term("�r�o", u) & "����ʂ�20%�팸����B"
				
			Case "���"
				msg = Term("����", u) & "�E" & Term("���", u) & "�� +10 �̏C����^����B"
				
			Case "������"
				msg = Term("����", u) & "�E" & Term("���", u) & "�E" & Term("�b�s��", u)
				If slevel >= 0 Then
					msg = msg & "�ɂ��ꂼ�� +" & VB6.Format(CInt(2 * slevel)) & " �̏C����^����B"
				Else
					msg = msg & "�ɂ��ꂼ�� " & VB6.Format(CInt(2 * slevel)) & " �̏C����^����B"
				End If
				
			Case "�p"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "�p�������������E" & Term("�A�r���e�B", u) & "�y�ѕK�v�Z�\��" & sname0 & "�̕����E" & Term("�A�r���e�B", u) & "�̏���" & Term("�d�m", u) & "��" & VB6.Format(i) & "%����������B"
				
			Case "�Z"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "�Z�������������E" & Term("�A�r���e�B", u) & "�y�ѕK�v�Z�\��" & sname0 & "�̕����E" & Term("�A�r���e�B", u) & "�̏���" & Term("�d�m", u) & "��" & VB6.Format(i) & "%����������B"
				
			Case "�W����"
				msg = Term("�X�y�V�����p���[", u) & "��" & Term("�r�o", u) & "����ʂ�����80%�Ɍ�������B"
				
			Case "�����{�\"
				If p.MinMorale > 100 Then
					If Not p.IsSkillLevelSpecified("�����{�\") Then
						msg = "�o������" & Term("�C��", u) & "��" & VB6.Format(p.MinMorale + 5 * slevel) & "�ɑ�������B"
					ElseIf slevel >= 0 Then 
						msg = "�o������" & Term("�C��", u) & "��" & VB6.Format(p.MinMorale + 5 * slevel) & "�ɑ�������B"
					Else
						msg = "�o������" & Term("�C��", u) & "��" & VB6.Format(p.MinMorale + 5 * slevel) & "�Ɍ�������B"
					End If
				Else
					If Not p.IsSkillLevelSpecified("�����{�\") Then
						msg = "�o������" & Term("�C��", u) & "��105�ɑ�������B"
					ElseIf slevel >= 0 Then 
						msg = "�o������" & Term("�C��", u) & "��" & VB6.Format(100 + 5 * slevel) & "�ɑ�������B"
					Else
						msg = "�o������" & Term("�C��", u) & "��" & VB6.Format(100 + 5 * slevel) & "�Ɍ�������B"
					End If
				End If
				
			Case "���ݗ͊J��"
				If IsOptionDefined("�_���[�W�{���ቺ") Then
					msg = Term("�C��", u) & "130�ȏ�Ŕ������A�_���[�W�� 20% ����������B"
				Else
					msg = Term("�C��", u) & "130�ȏ�Ŕ������A�_���[�W�� 25% ����������B"
				End If
				
			Case "�w��"
				msg = "���a" & StrConv(VB6.Format(p.CommandRange), VbStrConv.Wide) & "�}�X�ȓ��ɂ��閡���U�R�E�ėp�y�ъK�����L�p�C���b�g��" & Term("����", u) & "�E" & Term("���", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(5 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(5 * slevel)) & "�B"
				End If
				
			Case "�K��"
				If InStr(sname, "�K��Lv") = 0 Then
					msg = "�K�����x��" & StrConv(VB6.Format(CInt(slevel)), VbStrConv.Wide) & "�ɑ�������B;"
				End If
				msg = msg & "���a" & StrConv(VB6.Format(p.CommandRange), VbStrConv.Wide) & "�}�X�ȓ��ɂ���U�R�y�ъK�����L�p�C���b�g�Ɏw�����ʂ�^����B"
				
			Case "�i���T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("�i��", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "�ˌ��T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("�ˌ�", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "���̓T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("����", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "�����T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("����", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "����T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("���", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "�Z�ʃT�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("�Z��", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "�����T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("����", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(2 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(2 * slevel)) & "�B"
				End If
				
			Case "�T�|�[�g"
				msg = "�������T�|�[�g�p�C���b�g�̎��Ƀ��C���p�C���b�g��" & Term("����", u) & "�E" & Term("���", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(3 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(3 * slevel)) & "�B"
				End If
				
			Case "�L��T�|�[�g"
				msg = "���a�Q�}�X�ȓ��ɂ��閡���p�C���b�g��" & Term("����", u) & "�E" & Term("���", u)
				If slevel >= 0 Then
					msg = msg & "�� +" & VB6.Format(CInt(5 * slevel)) & "�B"
				Else
					msg = msg & "�� " & VB6.Format(CInt(5 * slevel)) & "�B"
				End If
				
			Case "����"
				msg = "�אڂ��郆�j�b�g�ɃT�|�[�g�A�^�b�N�ƃT�|�[�g�K�[�h��" & "�P�^�[���ɂ��ꂼ��" & VB6.Format(CInt(slevel)) & "��s���B"
				
			Case "����U��"
				msg = "�אڂ��郆�j�b�g�ɃT�|�[�g�A�^�b�N���P�^�[����" & VB6.Format(CInt(slevel)) & "��s���B"
				
			Case "����h��"
				msg = "�אڂ��郆�j�b�g�ɃT�|�[�g�K�[�h���P�^�[����" & VB6.Format(CInt(slevel)) & "��s���B"
				
			Case "����"
				msg = "��������U�����������ꍇ�A" & "�T�|�[�g�A�^�b�N����������U���ɕύX�����B;" & "�i�P�^�[���� " & VB6.Format(CInt(slevel)) & "��j"
				
			Case "�`�[��"
				msg = sdata & "�ɏ�������B" & "����" & sdata & "�̃��j�b�g�ɑ΂��Ă̂݉����w�����s���B"
				
			Case "�J�E���^�["
				msg = "�P�^�[���� " & VB6.Format(CInt(slevel)) & "��" & "�������J�E���^�[�U���ɂȂ�A����̍U���ɐ搧���Ĕ������s���B"
				
			Case "���K��"
				If LLength(sdata) = 2 Then
					msg = "�p�C���b�g��" & Term("�C��", u) & "��" & LIndex(sdata, 2) & "�ȏ�Ŕ����B"
				Else
					msg = "�p�C���b�g��" & Term("�C��", u) & "��120�ȏ�Ŕ����B"
				End If
				msg = msg & "�������K���J�E���^�[�U���ɂȂ�A����̍U���ɐ搧���Ĕ������s���B"
				
			Case "��ǂ�"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "%�̊m����" & "�������J�E���^�[�U���ɂȂ�A����̍U���ɐ搧���Ĕ������s���B"
				
			Case "�čU��"
				msg = "�����̍U���̒���� " & VB6.Format(CInt(100 * slevel \ 16)) & "% �̊m���ōčU�����s���B" & "�������p�C���b�g��" & Term("����", u) & "������������ꍇ�A�m���͔����B"
				
			Case "�Q��s��"
				msg = "�P�^�[���ɂQ��A�s�����\�ɂȂ�B"
				
			Case "�ϋv"
				If slevel >= 0 Then
					msg = "�_���[�W�v�Z�̍ۂ�" & Term("���b", u) & "��" & VB6.Format(CInt(5 * slevel)) & "%����������B"
				Else
					msg = "�_���[�W�v�Z�̍ۂ�" & Term("���b", u) & "��" & VB6.Format(CInt(5 * System.Math.Abs(slevel))) & "%����������B"
				End If
				
			Case "�r�o�ᐬ��"
				msg = "���x���A�b�v���̍ő�" & Term("�r�o", u) & "�̑����ʂ��ʏ�̔����Ɍ�������B"
				
			Case "�r�o������"
				msg = "���x���A�b�v���̍ő�" & Term("�r�o", u) & "�̑����ʂ��ʏ��1.5�{�ɑ�������B"
				
			Case "�r�o��"
				msg = "���^�[��" & Term("�r�o", u) & "���p�C���b�g���x��/8+5�񕜂���(+" & VB6.Format(p.Level \ 8 + 5) & ")�B"
				
			Case "�i������"
				'�U���͒ᐬ���I�v�V�������w�肳��Ă��邩�ǂ����ŉ����ύX����B
				msg = "���x���A�b�v����" & Term("�i��", u) & "�̑����ʂ�"
				If IsOptionDefined("�U���͒ᐬ��") Then
					msg = msg & VB6.Format(slevel + 0.5) & "�ɂȂ�B"
				Else
					msg = msg & VB6.Format(slevel + 1) & "�ɂȂ�B"
				End If
				
			Case "�ˌ�����"
				'�U���͒ᐬ���I�v�V�����A�p�Z�\�̗L���ɂ���ăf�t�H���g�����ύX����B
				If p.HasMana() Then
					msg = "���x���A�b�v����" & Term("����", u) & "�̑����ʂ�"
				Else
					msg = "���x���A�b�v����" & Term("�ˌ�", u) & "�̑����ʂ�"
				End If
				If IsOptionDefined("�U���͒ᐬ��") Then
					msg = msg & VB6.Format(slevel + 0.5) & "�ɂȂ�B"
				Else
					msg = msg & VB6.Format(slevel + 1) & "�ɂȂ�B"
				End If
				
			Case "��������"
				msg = "���x���A�b�v����" & Term("����", u) & "�̑����ʂ�" & VB6.Format(slevel + 2) & "�ɂȂ�B"
				
			Case "��𐬒�"
				msg = "���x���A�b�v����" & Term("���", u) & "�̑����ʂ�" & VB6.Format(slevel + 2) & "�ɂȂ�B"
				
			Case "�Z�ʐ���"
				msg = "���x���A�b�v����" & Term("�Z��", u) & "�̑����ʂ�" & VB6.Format(slevel + 1) & "�ɂȂ�B"
				
			Case "��������"
				msg = "���x���A�b�v����" & Term("����", u) & "�̑����ʂ�" & VB6.Format(slevel + 1) & "�ɂȂ�B"
				
			Case "�h�䐬��"
				'�h��͒ᐬ���I�v�V�������w�肳��Ă��邩�ǂ����ŉ����ύX����B
				msg = "���x���A�b�v����" & Term("�h��", u) & "�̑����ʂ�"
				If IsOptionDefined("�h��͒ᐬ��") Then
					msg = msg & VB6.Format(slevel + 0.5) & "�ɂȂ�B"
				Else
					msg = msg & VB6.Format(slevel + 1) & "�ɂȂ�B"
				End If
				
			Case "���_����"
				msg = Term("�r�o", u) & "���ő�" & Term("�r�o", u) & "��20%����(" & VB6.Format(p.MaxSP \ 5) & "����)�̏ꍇ�A" & "�^�[���J�n����" & Term("�r�o", u) & "���ő�" & Term("�r�o", u) & "��10%���񕜂���(+" & VB6.Format(p.MaxSP \ 10) & ")�B"
				
			Case "�������C�͑���"
				If slevel >= -1 Then
					msg = "�_���[�W���󂯂��ۂ�" & Term("�C��", u) & "+" & VB6.Format(CInt(slevel + 1)) & "�B"
				Else
					msg = "�_���[�W���󂯂��ۂ�" & Term("�C��", u) & VB6.Format(CInt(slevel + 1)) & "�B"
				End If
				
			Case "�������C�͑���"
				If slevel >= 0 Then
					msg = "�U���𖽒��������ۂ�" & Term("�C��", u) & "+" & VB6.Format(CInt(slevel)) & "�B(�}�b�v�U���͗�O)"
				Else
					msg = "�U���𖽒��������ۂ�" & Term("�C��", u) & VB6.Format(CInt(slevel)) & "�B(�}�b�v�U���͗�O)"
				End If
				
			Case "���s���C�͑���"
				If slevel >= 0 Then
					msg = "�U�����O���Ă��܂����ۂ�" & Term("�C��", u) & "+" & VB6.Format(CInt(slevel)) & "�B(�}�b�v�U���͗�O)"
				Else
					msg = "�U�����O���Ă��܂����ۂ�" & Term("�C��", u) & VB6.Format(CInt(slevel)) & "�B(�}�b�v�U���͗�O)"
				End If
				
			Case "������C�͑���"
				If slevel >= 0 Then
					msg = "�U������������ۂ�" & Term("�C��", u) & "+" & VB6.Format(CInt(slevel)) & "�B"
				Else
					msg = "�U������������ۂ�" & Term("�C��", u) & VB6.Format(CInt(slevel)) & "�B"
				End If
				
			Case "�N����"
				msg = Term("�r�o", u) & "�A" & Term("�g�o", u) & "�A" & Term("�d�m", u) & "�̑S�Ă��ő�l��20%�ȉ��ɂȂ�Ɩ��^�[���ŏ��ɔ����B" & Term("�r�o", u) & "�A" & Term("�g�o", u) & "�A" & Term("�d�m", u) & "���S������B"
				
			Case "��p"
				msg = "�v�l�p�^�[������̍ۂɗp������" & Term("�Z��", u)
				If slevel >= 0 Then
					msg = msg & "�����l�����x���~10����(+" & VB6.Format(CInt(10 * slevel)) & ")�B"
				Else
					msg = msg & "�����l�����x���~10����(" & VB6.Format(CInt(10 * slevel)) & ")�B"
				End If
				
			Case "���ӋZ"
				msg = "�u" & p.SkillData(stype) & "�v������������E" & Term("�A�r���e�B", u) & "�ɂ��_���[�W�E���ʗʂ� 20% �����B" & "�܂��A" & Term("�A�r���e�B", u) & "�̌p�����Ԃ� 40% �����B"
				
			Case "�s����"
				msg = "�u" & p.SkillData(stype) & "�v������������E" & Term("�A�r���e�B", u) & "�ɂ��_���[�W�E���ʗʂ� 20% �����B" & "�܂��A" & Term("�A�r���e�B", u) & "�̌p�����Ԃ� 40% �����B"
				
			Case "�n���^�["
				msg = "�^�[�Q�b�g��"
				For i = 2 To LLength(sdata)
					If i = 3 Then
						msg = msg & "��"
					ElseIf 3 > 2 Then 
						msg = msg & "�A"
					End If
					msg = msg & LIndex(sdata, i)
				Next 
				msg = msg & "�ł���ꍇ�A�^�[�Q�b�g�ɗ^����_���[�W��"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "%��������B"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "%��������B"
				End If
				
			Case "�r�o�����"
				msg = Term("�X�y�V�����p���[", u)
				For i = 2 To LLength(sdata)
					msg = msg & "�u" & LIndex(sdata, i) & "�v"
				Next 
				msg = msg & "��" & Term("�r�o", u) & "����ʂ�"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "%��������B"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "%��������B"
				End If
				
			Case "�X�y�V�����p���[��������"
				msg = Term("�C��", u) & "��" & LIndex(sdata, 3) & "�ȏ�Ŕ������A" & "���^�[���ŏ��Ɂu" & LIndex(sdata, 2) & "�v�������ł�����B" & "�i" & Term("�r�o", u) & "�͏���Ȃ��j"
				
			Case "�C��"
				msg = "�C�����u���" & Term("�A�r���e�B", u) & "���g�����ۂ�" & Term("�g�o", u) & "�񕜗ʂ� "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "�⋋"
				If IsOptionDefined("�ړ���⋋�s��") Then
					msg = "�ړ���ɕ⋋���u���g�p�ł���悤�ɂȂ�B�܂��A"
				End If
				msg = msg & "�⋋" & Term("�A�r���e�B", u) & "���g�����ۂ�" & Term("�d�m", u) & "�񕜗ʂ� "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "% ��������B"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "% ��������B"
				End If
				
			Case "�C�͏��"
				i = 150
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("�C��", u) & "�̏����" & VB6.Format(i) & "�ɂȂ�B"
				
			Case "�C�͉���"
				i = 50
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("�C��", u) & "�̉�����" & VB6.Format(i) & "�ɂȂ�B"
				
				' ADD START MARGE
			Case "�V��"
				msg = "�ړ���g�p�\�ȕ���E" & Term("�A�r���e�B", u) & "���g������ɁA�c�����ړ��͂��g���Ĉړ��ł���B"
				' ADD END MARGE
				
			Case Else
				'�_�~�[�\��
				
				'�p�C���b�g���ŉ�����`���Ă���H
				With p
					sdata = .SkillData(sname0)
					If ListIndex(sdata, 1) = "���" Then
						msg = ListIndex(sdata, ListLength(sdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
						SkillHelpMessage = msg
						Exit Function
					End If
				End With
				
				'���j�b�g���ŉ�����`���Ă���H
				With u
					For i = 1 To .CountFeature
						If .Feature(i) = stype Then
							fdata = .FeatureData(i)
							If ListIndex(fdata, 1) = "���" Then
								msg = ListIndex(fdata, ListLength(fdata))
							End If
						End If
					Next 
				End With
				If Not u2 Is Nothing Then
					With u2
						For i = 1 To .CountFeature
							If .Feature(i) = stype Then
								fdata = .FeatureData(i)
								If ListIndex(fdata, 1) = "���" Then
									msg = ListIndex(fdata, ListLength(fdata))
								End If
							End If
						Next 
					End With
				End If
				
				If msg = "" Then
					Exit Function
				End If
				
				'���j�b�g���ŉ�����`���Ă���ꍇ
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
		End Select
		
		'�p�C���b�g���ŉ�����`���Ă���H
		With p
			sdata = .SkillData(sname0)
			If ListIndex(sdata, 1) = "���" Then
				msg = ListIndex(sdata, ListLength(sdata))
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
			End If
		End With
		
		'���j�b�g���ŉ�����`���Ă���H
		With u
			For i = 1 To .CountFeature
				If .Feature(i) = sname0 Then
					fdata = .FeatureData(i)
					If ListIndex(fdata, 1) = "���" Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
				End If
			Next 
		End With
		If Not u2 Is Nothing Then
			With u2
				For i = 1 To .CountFeature
					If .Feature(i) = sname0 Then
						fdata = .FeatureData(i)
						If ListIndex(fdata, 1) = "���" Then
							msg = ListIndex(fdata, ListLength(fdata))
							If Left(msg, 1) = """" Then
								msg = Mid(msg, 2, Len(msg) - 2)
							End If
						End If
					End If
				Next 
			End With
		End If
		
		'���g���̍ۂ́u�p�C���b�g�v�Ƃ�������g��Ȃ��悤�ɂ���
		If IsOptionDefined("���g��") Then
			ReplaceString(msg, "���C���p�C���b�g", "���j�b�g")
			ReplaceString(msg, "�T�|�[�g�p�C���b�g", "�T�|�[�g")
			ReplaceString(msg, "�p�C���b�g���x��", "���x��")
			ReplaceString(msg, "�p�C���b�g", "���j�b�g")
		End If
		
		SkillHelpMessage = msg
	End Function
	
	
	'���j�b�g u �� findex �Ԗڂ̓���\�͂̉����\��
	Public Sub FeatureHelp(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean)
		Dim fname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		With u
			'����\�̖͂��̂𒲂ׂ�
			'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If findex = "����E�h��N���X" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = .AllFeatureName(CShort(findex))
			Else
				fname = .AllFeatureName(findex)
			End If
		End With
		
		msg = FeatureHelpMessage(u, findex, is_additional)
		
		'����̕\��
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("���b�Z�[�W�E�B���h�E")
			End If
			DisplayMessage("�V�X�e��", "<b>" & fname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'���j�b�g u �� findex �Ԗڂ̓���\�͂̉��
	Public Function FeatureHelpMessage(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean) As String
		Dim fid As Short
		Dim fname, ftype, fname0 As String
		Dim fdata, opt As String
		Dim flevel, lv_mod As Double
		Dim flevel_specified As Boolean
		Dim msg As String
		Dim i, idx As Short
		Dim buf As String
		Dim prob As Short
		Dim p As Pilot
		Dim sname As String
		Dim slevel As Double
		Dim uname As String
		
		With u
			'���C���p�C���b�g
			p = .MainPilot
			
			'����\�̖͂��́A���x���A�f�[�^�𒲂ׂ�
			'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If findex = "����E�h��N���X" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				ftype = findex
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fid = CShort(findex)
				ftype = .AllFeature(fid)
				fname = .AllFeatureName(fid)
				fdata = .AllFeatureData(fid)
				flevel = .AllFeatureLevel(fid)
				flevel_specified = .AllFeatureLevelSpecified(fid)
			Else
				ftype = .AllFeature(findex)
				fname = .AllFeatureName(findex)
				fdata = .AllFeatureData(findex)
				flevel = .AllFeatureLevel(findex)
				flevel_specified = .AllFeatureLevelSpecified(findex)
				For fid = 1 To .CountFeature
					'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If .AllFeature(fid) = findex Then
						Exit For
					End If
				Next 
			End If
			If InStr(fname, "Lv") > 0 Then
				fname0 = Left(fname, InStr(fname, "Lv") - 1)
			Else
				fname0 = fname
			End If
			
			'�d���\�ȓ���\�͂̏ꍇ�A���x���݂̂��قȂ�\�͂̃��x���͗ݐς���
			Select Case ftype
				Case "�t�B�[���h", "�A�[�}�[", "���W�X�g", "�U�����"
					For i = 1 To u.CountAllFeature
						If i <> fid And .AllFeature(i) = ftype And .AllFeatureData(i) = fdata Then
							flevel = flevel + .AllFeatureLevel(i)
						End If
					Next 
			End Select
		End With
		
		Select Case ftype
			Case "�V�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W�𔼌��B"
				
			Case "��^�V�[���h"
				sname = p.SkillName0("�r�h��")
				If p.IsSkillAvailable("�r�h��") Then
					prob = (p.SkillLevel("�r�h��") + 1) * 100 \ 16
				End If
				msg = "(" & sname & "Lv+1)/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W�𔼌��B"
				
			Case "���^�V�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W��2/3�Ɍ����B"
				
			Case "�G�l���M�[�V�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				If flevel > 0 Then
					msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W�𔼌�������ōX��" & VB6.Format(100 * flevel) & "�����B"
				Else
					msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W�𔼌��B"
				End If
				msg = msg & "��������5�d�m����B�u���v������������ɂ͖����B"
				
			Case "�A�N�e�B�u�V�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				If p.IsSkillAvailable("�r�h��") Then
					prob = (p.SkillLevel("�r�h��") + 2) * 100 \ 16
				End If
				msg = "(" & sname & "Lv+2)/16�̊m��(" & VB6.Format(prob) & "%)�Ŗh����s���A" & "�_���[�W�𔼌��B"
				
			Case "��"
				sname = p.SkillName0("�r�h��")
				slevel = p.SkillLevel("�r�h��")
				If slevel > 0 Then
					slevel = 100 * slevel + 400
				End If
				msg = VB6.Format(flevel) & "��A�U���ɂ���Ċђʂ����܂ŃV�[���h�h����s���A" & "�_���[�W������������(-" & VB6.Format(CInt(slevel)) & ")�B;" & "�������U�������u�j�v�����������Ă����ꍇ�A��x�ɂQ�񕪔j�󂳂��B;" & "�_���[�W�̌����ʂ̓p�C���b�g��" & sname & "���x���ɂ���Č��܂�B"
				
			Case "�o���A"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				msg = msg & "�_���[�W" & VB6.Format(CInt(1000 * flevel)) & "�ȉ��̍U���𖳌����B"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
					End If
				Else
					msg = msg & ";��������10�d�m����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�o���A�V�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ������A"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				msg = msg & "�_���[�W" & VB6.Format(CInt(1000 * flevel)) & "�ȉ��̍U���𖳌����B"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & "��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
					End If
				Else
					msg = msg & "��������10" & Term("�d�m", u) & "����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�L��o���A"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "���a" & StrConv(LIndex(fdata, 2), VbStrConv.Wide) & "�}�X�ȓ��̖������j�b�g�ɑ΂���"
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "�אڂ��閡�����j�b�g�ɑ΂���"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				msg = msg & "�_���[�W" & VB6.Format(CInt(1000 * flevel)) & "�ȉ��̍U���𖳌����B"
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 4) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 4), 2) & Term("�d�m", u) & "�����B"
					End If
				Else
					msg = msg & ";��������" & VB6.Format(20 * i) & Term("�d�m", u) & "����B"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 5) & "�ȏ�Ŏg�p�\�B"
				End If
				msg = msg & ";�������U�������L���͈͓��ɂ���ꍇ�͖������B"
				
			Case "�t�B�[���h"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(500 * flevel)) & "����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-500 * flevel)) & "����������B"
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�A�N�e�B�u�t�B�[���h"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ������A"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(500 * flevel)) & "����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-500 * flevel)) & "����������B"
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�L��t�B�[���h"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "���a" & StrConv(LIndex(fdata, 2), VbStrConv.Wide) & "�}�X�ȓ��̖������j�b�g�ɑ΂���"
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "�אڂ��閡�����j�b�g�ɑ΂���"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(500 * flevel)) & "����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-500 * flevel)) & "����������B"
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 4) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 4), 2) & Term("�d�m", u) & "�����B"
					End If
				Else
					msg = msg & ";��������" & VB6.Format(20 * i) & Term("�d�m", u) & "����B"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 5) & "�ȏ�Ŏg�p�\�B"
				End If
				msg = msg & ";�������U�������L���͈͓��ɂ���ꍇ�͖������B"
				
			Case "�v���e�N�V����"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel > 10 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel - 100)) & "%�z������B"
				ElseIf flevel >= 0 Then 
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel)) & "%����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-10 * flevel)) & "%����������B"
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";��������10" & Term("�d�m", u) & "�����B"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�A�N�e�B�u�v���e�N�V����"
				sname = p.SkillName0("�r�h��")
				prob = p.SkillLevel("�r�h��") * 100 \ 16
				msg = sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ������A"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel > 10 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel - 100)) & "%�z������B"
				ElseIf flevel >= 0 Then 
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel)) & "%����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-10 * flevel)) & "%����������B"
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";��������10" & Term("�d�m", u) & "�����B"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�o���A����������"
							msg = msg & ";�o���A�������ɂ���Ė���������Ȃ��B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "%)�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "%)�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & "%)�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & "%)�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & "%)�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "%)�B"
					End Select
				Next 
				
			Case "�L��v���e�N�V����"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "���a" & StrConv(LIndex(fdata, 2), VbStrConv.Wide) & "�}�X�ȓ��̖������j�b�g�ɑ΂���"
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "�אڂ��閡�����j�b�g�ɑ΂���"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				If flevel > 10 Then
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel - 100)) & "%�z������B"
				ElseIf flevel >= 0 Then 
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(10 * flevel)) & "%����������B"
				Else
					msg = msg & "�U���̃_���[�W��" & VB6.Format(CInt(-10 * flevel)) & "%����������B"
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 4) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 4), 2) & Term("�d�m", u) & "�����B"
					End If
				Else
					msg = msg & ";��������" & VB6.Format(20 * i) & Term("�d�m", u) & "����B"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 5) & "�ȏ�Ŏg�p�\�B"
				End If
				msg = msg & ";�������U�������L���͈͓��ɂ���ꍇ�͖������B"
				
			Case "�A�[�}�["
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "�U���ɑ΂��đ��b��" & VB6.Format(CInt(100 * flevel)) & "����������B"
				Else
					msg = msg & "�U���ɑ΂��đ��b��" & VB6.Format(CInt(-100 * flevel)) & "����������B"
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 3) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "���W�X�g"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel > 10 Then
					msg = msg & "�U���ɑ΂��ă_���[�W��" & VB6.Format(100 - CInt(10 * flevel)) & "%�z������B"
				ElseIf flevel >= 0 Then 
					msg = msg & "�U���ɑ΂��ă_���[�W��" & VB6.Format(CInt(10 * flevel)) & "%�y��������B"
				Else
					msg = msg & "�U���ɑ΂��ă_���[�W��" & VB6.Format(CInt(-10 * flevel)) & "%����������B"
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 3) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "%)�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "%)�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & "%)�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & "%)�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & "%)�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "%)�B"
					End Select
				Next 
				
			Case "���Đg�Z"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "�_���[�W" & VB6.Format(CInt(500 * flevel)) & "�܂ł�"
				End If
				
				msg = msg & "�U����"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "%�̊m���Ŏ󂯎~�߁A"
					Else
						msg = msg & "�󂯎~�߁A"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)�Ŏ󂯎~�߁A"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŏ󂯎~�߁A"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "�Ŕ����B"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 5) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 5), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 6) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𑊎E�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "����"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "�_���[�W" & VB6.Format(CInt(500 * flevel)) & "�܂ł�"
				End If
				
				msg = msg & "�U����"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "%�̊m���Ŕ��ˁB"
					Else
						msg = msg & "���ˁB"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ��ˁB"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ��ˁB"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 4) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 4), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 5) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�j�~"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "�_���[�W" & VB6.Format(CInt(500 * flevel)) & "�ȉ���"
				End If
				msg = msg & "�U����"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "%�̊m���őj�~�B"
					Else
						' MOD START MARGE
						'                    msg = msg & buf & "�j�~�B"
						msg = msg & "�j�~�B"
						' MOD END MARGE
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)�őj�~�B"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�őj�~�B"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 4) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 4), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 5) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𒆘a�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�L��j�~"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "���a" & StrConv(LIndex(fdata, 2), VbStrConv.Wide) & "�}�X�ȓ��̖������j�b�g�ɑ΂���"
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "�אڂ��閡�����j�b�g�ɑ΂���"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = msg & "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "�_���[�W" & VB6.Format(CInt(500 * flevel)) & "�ȉ���"
				End If
				msg = msg & "�U����"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						' MOD START MARGE
						'                    msg = msg & "%�̊m���őj�~�B"
						msg = msg & buf & "%�̊m���őj�~�B"
						' MOD END MARGE
					Else
						msg = msg & "�j�~�B"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)�őj�~�B"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�őj�~�B"
				End If
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 5) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 5), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 6) & "�ȏ�Ŏg�p�\�B"
				End If
				msg = msg & ";�������U�������L���͈͓��ɂ���ꍇ�͖������B"
				
			Case "�Z��"
				prob = flevel * 100 \ 16
				msg = VB6.Format(flevel) & "/16�̊m��(" & VB6.Format(prob) & "%)�Ŕ������A" & "�_���[�W��" & Term("�g�o", u) & "�ɕϊ��B;" & "�������A�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
				
			Case "�ϊ�"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				msg = msg & "�U�����󂯂��ۂɃ_���[�W��" & Term("�d�m", u) & "�ɕϊ��B;" & "�ϊ������� " & Term("�d�m", u) & "���� �� "
				msg = msg & VB6.Format(0.01 * flevel)
				msg = msg & " �~ �_���[�W"
				
			Case "�r�[���z��"
				msg = "�r�[���ɂ��U���̃_���[�W���g�o�ɕϊ�"
				
			Case "��������"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "�S" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 3), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 3) & "�v����������"
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "�_���[�W" & VB6.Format(CInt(500 * flevel)) & "�܂ł�"
				End If
				
				msg = msg & "�U�����󂯂��ۂ�"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "%�̊m���ŁA"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)�ŁA"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)�ŁA"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "�ɂ�鎩����������������B"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 5) & Term("�d�m", u) & "����B"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 5), 2) & Term("�d�m", u) & "�����B"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 6) & "�ȏ�Ŏg�p�\�B"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "���E"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A�אڎ��Ɍ��ʂ͑��E�B"
						Case "���a"
							msg = msg & ";" & fname0 & "�������j�b�g���m�̏ꍇ�A" & "�אڎ��Ƀ��x�����������ʂ𑊎E�B"
						Case "�ߐږ���"
							msg = msg & ";�u���v�u�ˁv�u�ځv�ɂ��U���ɂ͖����B"
						Case "�蓮"
							msg = msg & ";�h��I�����ɂ̂ݔ����B"
						Case "�\�͕K�v"
							'�X�L�b�v
						Case "������"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x���ω�(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & ")�B"
							End If
						Case "���"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PlanaLevel) & ")�B"
						Case "�I�[��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.AuraLevel) & ")�B"
						Case "���\��"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "�ɂ�苭�x������(+" & VB6.Format(lv_mod * u.PsychicLevel) & ")�B"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & ";�p�C���b�g��" & sname & "���x���ɂ�苭�x������(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & ")�B"
					End Select
				Next 
				
			Case "�g�o��"
				msg = "���^�[���ő�" & Term("�g�o", u) & "��" & VB6.Format(10 * flevel) & "%����" & Term("�g�o", u) & "���񕜁B"
				
			Case "�d�m��"
				msg = "���^�[���ő�" & Term("�d�m", u) & "��" & VB6.Format(10 * flevel) & "%����" & Term("�d�m", u) & "���񕜁B"
				
			Case "��͉�"
				sname = p.SkillName0("���")
				msg = "���^�[���ő�" & sname & "��" & VB6.Format(10 * flevel) & "%����" & sname & "���񕜁B"
				
			Case "�g�o����"
				msg = "���^�[���ő�" & Term("�g�o", u) & "��" & VB6.Format(10 * flevel) & "%����" & Term("�g�o", u) & "������B"
				
			Case "�d�m����"
				msg = "���^�[���ő�" & Term("�d�m", u) & "��" & VB6.Format(10 * flevel) & "%����" & Term("�d�m", u) & "������B"
				
			Case "��͏���"
				sname = p.SkillName0("���")
				msg = "���^�[���ő�" & sname & "��" & VB6.Format(10 * flevel) & "%����" & sname & "������B"
				
			Case "���g"
				msg = "50%�̊m���ōU�������S�ɉ���B;" & "���������F" & Term("�C��", u) & "130�ȏ�"
				
			Case "�����"
				msg = "������U����" & VB6.Format(10 * flevel) & "%�̊m���ŉ���B"
				If IsNumeric(LIndex(fdata, 2)) Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 2) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 2)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 2), 2) & Term("�d�m", u) & "�����B"
					End If
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 3) & "�ȏ�Ŏg�p�\�B"
				End If
				If LIndex(fdata, 4) = "�蓮" Then
					msg = msg & ";���I�����ɂ̂ݔ����B"
				End If
				
			Case "�ً}�e���|�[�g"
				msg = "�U�����󂯂��ۂ�" & VB6.Format(10 * flevel) & "%�̊m����" & "�e���|�[�g���A�U��������B;" & "�e���|�[�g���" & LIndex(fdata, 2) & "�}�X�ȓ��͈̔͂̓��A" & "�ł��G���牓���n�_����I�΂��B"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";��������" & LIndex(fdata, 3) & Term("�d�m", u) & "����B"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";��������" & Mid(LIndex(fdata, 3), 2) & Term("�d�m", u) & "�����B"
					End If
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 4) & "�ȏ�Ŏg�p�\�B"
				End If
				If LIndex(fdata, 5) = "�蓮" Then
					msg = msg & ";���I�����ɂ̂ݔ����B"
				End If
				
			Case "�_�~�["
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = buf & "��g����ɂ��čU����" & VB6.Format(flevel) & "��܂ŉ���B"
				
			Case "�U�����"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "�S" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "�u" & Mid(LIndex(fdata, 2), 2) & "�v�����������Ȃ�"
					Else
						msg = "�u" & LIndex(fdata, 2) & "�v����������"
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "�U���̖�������{����" & VB6.Format(CInt(100 - 10 * flevel)) & "%�Ɍ���������B"
				Else
					msg = msg & "�U���̖�������{����" & VB6.Format(CInt(100 - 10 * flevel)) & "%�ɑ���������B"
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("�C��", u) & LIndex(fdata, 3) & "�ȏ�Ŏg�p�\�B"
				End If
				
			Case "��R��"
				If flevel >= 0 Then
					msg = "����̓�����ʂ��󂯂�m����" & VB6.Format(10 * flevel) & "%����������B"
				Else
					msg = "����̓�����ʂ��󂯂�m����" & VB6.Format(-10 * flevel) & "%����������B"
				End If
				
			Case "�C�����u"
				msg = "���̃��j�b�g��" & Term("�g�o", u)
				Select Case flevel
					Case 1
						msg = msg & "���ő�" & Term("�g�o", u) & "��30%�����񕜁B"
					Case 2
						msg = msg & "���ő�" & Term("�g�o", u) & "��50%�����񕜁B"
					Case 3
						msg = msg & "��S���B"
				End Select
				
			Case "�⋋���u"
				msg = "���̃��j�b�g��" & Term("�d�m", u) & "�ƒe���S���B;" & "���������j�b�g�̃p�C���b�g��" & Term("�C��", u) & "��-10�B"
				If IsOptionDefined("�ړ���⋋�s��") Then
					msg = msg & "�ړ���͎g�p�s�B"
				End If
				
			Case "�C���s��"
				For i = 2 To CInt(fdata)
					buf = LIndex(fdata, i)
					If Left(buf, 1) = "!" Then
						buf = Mid(buf, 2)
						msg = msg & buf & "�ȊO�ł�" & Term("�g�o", u) & "���񕜏o���Ȃ��B"
					Else
						msg = msg & buf & "�ł�" & Term("�g�o", u) & "���񕜏o���Ȃ��B"
					End If
				Next 
				msg = msg & buf & ";�������A" & Term("�X�y�V�����p���[", u) & "��n�`�A��͂ɂ��񕜂͉\�B"
				
			Case "��͕ϊ���"
				sname = p.SkillName0("���")
				msg = sname & "�ɍ��킹�Ċe��\�͂��㏸����B"
				If flevel_specified Then
					msg = msg & ";�i" & sname & "��� = " & VB6.Format(flevel) & "�j"
				End If
				
			Case "�I�[���ϊ���"
				sname = p.SkillName0("�I�[��")
				msg = sname & "���x���ɍ��킹�Ċe��\�͂��㏸����B"
				If flevel_specified Then
					msg = msg & ";�i" & sname & "������x�� = " & VB6.Format(flevel) & "�j"
				End If
				
			Case "�T�C�L�b�N�h���C�u"
				sname = p.SkillName0("���\��")
				msg = sname & "���x�����Ƃ�" & Term("���b", u) & "+100�A" & Term("�^����", u) & "+5"
				If flevel_specified Then
					msg = msg & ";�i" & sname & "������x�� = " & VB6.Format(flevel) & "�j"
				End If
				
			Case "�V���N���h���C�u"
				sname = p.SkillName0("������")
				msg = sname & "�ɍ��킹�Ċe��\�͂��㏸����B"
				If flevel_specified Then
					msg = msg & ";�i" & sname & "��� = " & VB6.Format(flevel) & "%�j"
				End If
				
			Case "�X�e���X"
				If flevel_specified Then
					msg = "�G����" & StrConv(VB6.Format(flevel), VbStrConv.Wide) & "�}�X�ȓ��ɂ��Ȃ����蔭������Ȃ��B" & "��������������U������ƂP�^�[�������B"
				Else
					msg = "�G����R�}�X�ȓ��ɂ��Ȃ����蔭������Ȃ��B" & "��������������U������ƂP�^�[�������B"
				End If
				
			Case "�X�e���X������"
				msg = "�G�̃X�e���X�\�͂𖳌�������B"
				
			Case "�e���|�[�g"
				msg = "�e���|�[�g���s���A" & Term("�ړ���", u) & VB6.Format(u.Speed + flevel) & "�Œn�`�𖳎����Ĉړ��B;"
				If LLength(fdata) > 1 Then
					If CShort(LIndex(fdata, 2)) > 0 Then
						msg = msg & LIndex(fdata, 2) & Term("�d�m", u) & "����B"
					End If
				Else
					msg = msg & "40" & Term("�d�m", u) & "����B"
				End If
				
			Case "�W�����v"
				msg = Term("�ړ���", u) & VB6.Format(u.Speed + flevel) & "�Œn��n�`�𖳎����Ȃ���W�����v�ړ��B"
				If LLength(fdata) > 1 Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";" & LIndex(fdata, 2) & Term("�d�m", u) & "����B"
					End If
				End If
				
			Case "���j"
				msg = "�������j���ňړ��\�B�[�C���̐[�����̒n�`�ɐi�����邱�Ƃ��o����B" & "�����������ł̈ړ��R�X�g���P�ɂȂ��ł͂Ȃ��B"
				
			Case "����ړ�"
				msg = "����ɕ�����ňړ��\�B"
				
			Case "�z�o�[�ړ�"
				msg = "�󒆂ɕ����Ȃ���ړ����邱�Ƃō����Ɛጴ�̈ړ��R�X�g���P�ɂȂ�B" & "�܂��A����ړ����\�B�������ړ�����5" & Term("�d�m", u) & "����B"
				
			Case "���߈ړ�"
				msg = "��Q���𖳎����Ĉړ��B"
				
			Case "���蔲���ړ�"
				msg = "�G���j�b�g������}�X��ʉ߉\�B"
				
			Case "���H�ړ�"
				msg = "���H��݂̂��ړ��\�B"
				
			Case "�ړ�����"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "�A" & LIndex(fdata, i)
				Next 
				msg = msg & "��݂̂��ړ��\�B"
				
			Case "�i���s��"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "�A" & LIndex(fdata, i)
				Next 
				msg = msg & "�ɂ͐i���s�B"
				
			Case "�n�`�K��"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "�A" & LIndex(fdata, i)
				Next 
				msg = msg & "�ɂ�����ړ��R�X�g���P�ɂȂ�B"
				
			Case "�ǉ��ړ���"
				msg = LIndex(fdata, 2) & "�ɂ���ƁA" & Term("�ړ���", u) & "��"
				If flevel >= 0 Then
					msg = msg & VB6.Format(flevel) & "�����B"
				Else
					msg = msg & VB6.Format(-flevel) & "�����B"
				End If
				
			Case "���"
				msg = "���̃��j�b�g���i�[���A�C���E�^���\�B"
				
			Case "�i�[�s��"
				msg = "��͂Ɋi�[���邱�Ƃ��o���Ȃ��B"
				
			Case "���藘��"
				msg = "����ɕ���𑕔��\�B"
				
			Case "�������j�b�g"
				msg = "�����̃��j�b�g�ɂ���č\�����ꂽ�������j�b�g�B"
				
			Case "�������j�b�g"
				msg = "�������ꂽ���j�b�g�B"
				
			Case "�ό`"
				If u.IsHero Then
					buf = "�ω�"
				Else
					buf = "�ό`"
				End If
				If LLength(fdata) > 2 Then
					msg = "�ȉ��̌`�Ԃ�" & buf & "; "
					For i = 2 To LLength(fdata)
						If u.OtherForm(LIndex(fdata, i)).IsAvailable() Then
							If u.Nickname = UDList.Item(LIndex(fdata, i)).Nickname Then
								uname = UDList.Item(LIndex(fdata, i)).Name
								If Right(uname, 5) = "(�O���^)" Then
									uname = Left(uname, Len(uname) - 5)
								ElseIf Right(uname, 5) = "�E�O���^)" Then 
									uname = Left(uname, Len(uname) - 5) & ")"
								ElseIf Right(uname, 5) = "(����^)" Then 
									uname = Left(uname, Len(uname) - 5)
								End If
							Else
								uname = UDList.Item(LIndex(fdata, i)).Nickname
							End If
							msg = msg & uname & "  "
						End If
					Next 
				Else
					If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
						uname = UDList.Item(LIndex(fdata, 2)).Name
					Else
						uname = UDList.Item(LIndex(fdata, 2)).Nickname
					End If
					If Right(uname, 5) = "(�O���^)" Then
						uname = Left(uname, Len(uname) - 5)
					ElseIf Right(uname, 5) = "�E�O���^)" Then 
						uname = Left(uname, Len(uname) - 5) & ")"
					ElseIf Right(uname, 5) = "(����^)" Then 
						uname = Left(uname, Len(uname) - 5)
					End If
					msg = "<B>" & uname & "</B>��" & buf & "�B"
				End If
				
			Case "�p�[�c����"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "(�O���^)" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "�E�O���^)" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "(����^)" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "�p�[�c�𕪗���" & uname & "�ɕό`�B"
				If flevel_specified Then
					msg = msg & ";���j�b�g�j�󎞂�" & VB6.Format(10 * flevel) & "%�̊m���Ŕ����B"
				End If
				
			Case "�p�[�c����"
				If u.Nickname = UDList.Item(fdata).Nickname Then
					uname = UDList.Item(fdata).Name
				Else
					uname = UDList.Item(fdata).Nickname
				End If
				If Right(uname, 5) = "(�O���^)" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "�E�O���^)" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "(����^)" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "�p�[�c�ƍ��̂�" & uname & "�ɕό`�B"
				
			Case "�n�C�p�[���[�h"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "(�O���^)" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "�E�O���^)" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "(����^)" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				If u.Nickname <> uname Then
					uname = "<B>" & uname & "</B>"
				Else
					uname = ""
				End If
				If InStr(fdata, "�C�͔���") > 0 Then
					msg = Term("�C��", u) & VB6.Format(100 + 10 * flevel) & "�œ���`��" & uname & "��"
				ElseIf flevel <= 5 Then 
					msg = Term("�C��", u) & VB6.Format(100 + 10 * flevel) & "�A" & "��������" & Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/4�ȉ��œ���`��" & uname & "��"
				Else
					msg = Term("�g�o", u) & "���ő�" & Term("�g�o", u) & "��1/4�ȉ��œ���`��" & uname & "��"
				End If
				If InStr(fdata, "��������") > 0 Then
					msg = msg & "����"
				End If
				If u.IsHero Then
					msg = msg & "�ϐg�B"
				Else
					msg = msg & "�ό`�B"
				End If
				
			Case "����"
				If u.IsHero Then
					buf = "�ω��B"
				Else
					buf = "�ό`�B"
				End If
				If LLength(fdata) > 3 Then
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = "�ȉ��̃��j�b�g�ƍ��̂�<B>" & UDList.Item(LIndex(fdata, 2)).Nickname & "</B>��" & buf & "; "
					Else
						msg = "�ȉ��̃��j�b�g�ƍ��̂�<B>" & LIndex(fdata, 2) & "</B>��" & buf & "; "
					End If
					
					For i = 3 To LLength(fdata)
						If UDList.IsDefined(LIndex(fdata, i)) Then
							msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
						Else
							msg = msg & LIndex(fdata, i) & "  "
						End If
					Next 
				Else
					If UDList.IsDefined(LIndex(fdata, 3)) Then
						msg = UDList.Item(LIndex(fdata, 3)).Nickname & "�ƍ��̂�"
					Else
						msg = LIndex(fdata, 3) & "�ƍ��̂�"
					End If
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = msg & UDList.Item(LIndex(fdata, 2)).Nickname & "��" & buf
					Else
						msg = msg & LIndex(fdata, 2) & "��" & buf
					End If
				End If
				
			Case "����"
				msg = "�ȉ��̃��j�b�g�ɕ����B; "
				For i = 2 To LLength(fdata)
					If UDList.IsDefined(LIndex(fdata, i)) Then
						msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
					Else
						msg = msg & LIndex(fdata, i) & "  "
					End If
				Next 
				
			Case "�s����"
				msg = Term("�g�o", u) & "���ő�l��1/4�ȉ��ɂȂ�Ɩ\������B"
				
			Case "�x�z"
				If LLength(fdata) = 2 Then
					If Not PDList.IsDefined(LIndex(fdata, 2)) Then
						ErrorMessage("�x�z�Ώۂ̃p�C���b�g�u" & LIndex(fdata, 2) & "�v�̃f�[�^����`����Ă��܂���")
						Exit Function
					End If
					msg = PDList.Item(LIndex(fdata, 2)).Nickname & "�̑��݂��ێ����A�d�������Ă���B"
				Else
					msg = "�ȉ��̃��j�b�g�̑��݂��ێ����A�d�������Ă���B;"
					For i = 2 To LLength(fdata)
						If Not PDList.IsDefined(LIndex(fdata, 2)) Then
							ErrorMessage("�x�z�Ώۂ̃p�C���b�g�u" & LIndex(fdata, i) & "�v�̃f�[�^����`����Ă��܂���")
							Exit Function
						End If
						msg = msg & PDList.Item(LIndex(fdata, i)).Nickname & "  "
					Next 
				End If
				
			Case "�d�b�l"
				msg = "���a�R�}�X�ȓ��̖������j�b�g�ɑ΂���U���̖�����������"
				If flevel >= 0 Then
					msg = msg & VB6.Format(100 - 5 * flevel) & "%�Ɍ���������B"
				Else
					msg = msg & VB6.Format(100 - 5 * flevel) & "%�ɑ���������B"
				End If
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = msg & "�����ɑ����" & buf & "�\�͂̌��ʂ𖳌����B"
				msg = msg & ";�v�O�U���U����ߐڍU���ɂ͖����B"
				
			Case "�u�[�X�g"
				If IsOptionDefined("�_���[�W�{���ቺ") Then
					msg = Term("�C��", u) & "130�ȏ�Ŕ������A�_���[�W�� 20% �A�b�v�B"
				Else
					msg = Term("�C��", u) & "130�ȏ�Ŕ������A�_���[�W�� 25% �A�b�v�B"
				End If
				
			Case "�h��s��"
				msg = "�U�����󂯂��ۂɖh��^������邱�Ƃ��o���Ȃ��B"
				
			Case "���s��"
				msg = "�U�����󂯂��ۂɉ���^������邱�Ƃ��o���Ȃ��B"
				
			Case "�i������"
				If flevel >= 0 Then
					msg = "�p�C���b�g��" & Term("�i��", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
				Else
					msg = "�p�C���b�g��" & Term("�i��", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�ˌ�����"
				If p.HasMana() Then
					If flevel >= 0 Then
						msg = "�p�C���b�g��" & Term("����", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
					Else
						msg = "�p�C���b�g��" & Term("����", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
					End If
				Else
					If flevel >= 0 Then
						msg = "�p�C���b�g��" & Term("�ˌ�", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
					Else
						msg = "�p�C���b�g��" & Term("�ˌ�", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
					End If
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "��������"
				If flevel >= 0 Then
					msg = "�p�C���b�g��" & Term("����", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
				Else
					msg = "�p�C���b�g��" & Term("����", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & "�C��" & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�������"
				If flevel >= 0 Then
					msg = "�p�C���b�g��" & Term("���", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
				Else
					msg = "�p�C���b�g��" & Term("���", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�Z�ʋ���"
				If flevel >= 0 Then
					msg = "�p�C���b�g��" & Term("�Z��", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
				Else
					msg = "�p�C���b�g��" & Term("�Z��", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "��������"
				If flevel >= 0 Then
					msg = "�p�C���b�g��" & Term("����", u) & "��+" & VB6.Format(CShort(5 * flevel)) & "�B"
				Else
					msg = "�p�C���b�g��" & Term("����", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�g�o����"
				If flevel >= 0 Then
					msg = "�ő�" & Term("�g�o", u) & "��" & VB6.Format(CShort(200 * flevel)) & "�����B"
				Else
					msg = "�ő�" & Term("�g�o", u) & "��" & VB6.Format(CShort(-200 * flevel)) & "�����B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�d�m����"
				If flevel >= 0 Then
					msg = "�ő�" & Term("�d�m", u) & "��" & VB6.Format(CShort(10 * flevel)) & "�����B"
				Else
					msg = "�ő�" & Term("�d�m", u) & "��" & VB6.Format(CShort(-10 * flevel)) & "�����B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "���b����"
				If flevel >= 0 Then
					msg = Term("���b", u) & "��" & VB6.Format(CShort(100 * flevel)) & "�����B"
				Else
					msg = Term("���b", u) & "��" & VB6.Format(CShort(-100 * flevel)) & "�����B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�^��������"
				If flevel >= 0 Then
					msg = Term("�^����", u) & "��" & VB6.Format(CShort(5 * flevel)) & "�����B"
				Else
					msg = Term("�^����", u) & "��" & VB6.Format(CShort(-5 * flevel)) & "�����B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�ړ��͋���"
				If flevel >= 0 Then
					msg = Term("�ړ���", u) & "��" & VB6.Format(CShort(flevel)) & "�����B"
				Else
					msg = Term("�ړ���", u) & "��" & VB6.Format(CShort(flevel)) & "�����B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�g�o��������"
				If flevel >= 0 Then
					msg = "�ő�" & Term("�g�o", u) & "��" & VB6.Format(CShort(5 * flevel)) & "%�������B"
				Else
					msg = "�ő�" & Term("�g�o", u) & "��" & VB6.Format(CShort(-5 * flevel)) & "%�������B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�d�m��������"
				If flevel >= 0 Then
					msg = "�ő�" & Term("�d�m", u) & "��" & VB6.Format(CShort(5 * flevel)) & "%�������B"
				Else
					msg = "�ő�" & Term("�d�m", u) & "��" & VB6.Format(CShort(-5 * flevel)) & "%�������B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "���b��������"
				If flevel >= 0 Then
					msg = Term("���b", u) & "��" & VB6.Format(CShort(5 * flevel)) & "%�������B"
				Else
					msg = Term("���b", u) & "��" & VB6.Format(CShort(-5 * flevel)) & "%�������B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "�^������������"
				If flevel >= 0 Then
					msg = Term("�^����", u) & "��" & VB6.Format(CShort(5 * flevel)) & "%�������B"
				Else
					msg = Term("�^����", u) & "��" & VB6.Format(CShort(-5 * flevel)) & "%�������B"
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 2) & "�ȏ�Ŕ����B"
				End If
				
			Case "����E�h��N���X"
				fdata = Trim(u.WeaponProficiency)
				If fdata <> "" Then
					msg = "����y" & fdata & "�z;"
				Else
					msg = "����y-�z;"
				End If
				fdata = Trim(u.ArmorProficiency)
				If fdata <> "" Then
					msg = msg & "�h��y" & fdata & "�z"
				Else
					msg = msg & "�h��y-�z"
				End If
				
			Case "�ǉ��U��"
				If LIndex(fdata, 3) <> "�S" Then
					buf = LIndex(fdata, 3)
					If Left(buf, 1) = "@" Then
						msg = Mid(buf, 2) & "�ɂ��"
					Else
						msg = "�u" & buf & "�v������������ɂ��"
					End If
				End If
				
				msg = msg & "�U���̌�ɁA"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "%�̊m����"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & ")/16�̊m��(" & VB6.Format(prob) & "%)��"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Lv/16�̊m��(" & VB6.Format(prob) & "%)��"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "�ɂ��ǌ����s���B"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";��������" & LIndex(fdata, 5) & "�d�m����B"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";��������" & Mid(LIndex(fdata, 5), 2) & "�d�m�����B"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("�C��", u) & LIndex(fdata, 6) & "�ȏ�Ŏg�p�\�B"
				End If
				If InStr(fdata, "�A���s��") > 0 Then
					msg = msg & "�A���s�B"
				End If
				
			Case "�y�n�b"
				If u.FeatureLevel("�y�n�b") < 0 Then
					msg = "���̃��j�b�g�͂y�n�b�ɂ��e����^���邱�Ƃ��o���Ȃ��B"
				Else
					msg = "���̃��j�b�g����"
					If LLength(fdata) < 2 Then
						buf = "1"
					Else
						buf = LIndex(fdata, 2)
					End If
					
					opt = LIndex(fdata, 3)
					If InStr(opt, "����") > 0 Then
						msg = msg & buf & "�}�X�ȓ��̒�����"
					ElseIf InStr(opt, " ����") > 0 Then 
						msg = msg & "���E" & buf & "�}�X�ȓ��̒�����"
					ElseIf InStr(opt, " ����") > 0 Then 
						msg = msg & "�㉺" & buf & "�}�X�ȓ��̒�����"
					Else
						msg = msg & buf & "�}�X�ȓ�"
					End If
					msg = msg & "��ʉ߂���G���j�b�g�ɁA�y�n�b�ɂ��e����^����B"
				End If
				
			Case "�y�n�b������"
				If flevel = 1 Then
					msg = "���̃��j�b�g�͓G���j�b�g�ɂ��y�n�b�̉e�����󂯂Ȃ��B"
				Else
					msg = "���̃��j�b�g�͓G���j�b�g�ɂ��" & VB6.Format(flevel) & "���x���ȉ��̂y�n�b�̉e�����󂯂Ȃ��B"
				End If
				
			Case "�אڃ��j�b�g�y�n�b������"
				If flevel = 1 Then
					msg = "���̃��j�b�g���אڂ���G���j�b�g�ɂ��y�n�b�𖳌�������B"
				Else
					msg = "���̃��j�b�g���אڂ���G���j�b�g�ɂ��" & VB6.Format(flevel) & "���x���ȉ��̂y�n�b�𖳌�������B"
				End If
				
			Case "�L��y�n�b������"
				msg = "���̃��j�b�g����"
				If LLength(fdata) < 2 Then
					buf = "1"
				Else
					buf = LIndex(fdata, 2)
				End If
				
				If flevel = 1 Then
					msg = msg & buf & "�}�X�ȓ��ɐݒ肳�ꂽ�y�n�b�̉e���𖳌�������B"
				Else
					msg = msg & buf & "�}�X�ȓ��ɐݒ肳�ꂽ" & VB6.Format(flevel) & "���x���ȉ��̂y�n�b�̉e���𖳌�������B"
				End If
				
				' ADD START MARGE
			Case "�n�`���ʖ�����"
				If LLength(fdata) > 1 Then
					For i = 2 To LLength(fdata)
						If i > 2 Then
							msg = msg & "�A"
						End If
						msg = msg & LIndex(fdata, i)
					Next 
					msg = msg & "��"
				Else
					msg = msg & "�S�n�`��"
				End If
				msg = msg & "�g�o�E�d�m�������ԕt�����̓�����ʂ𖳌�������B"
				' ADD END MARGE
				
			Case Else
				If is_additional Then
					'�t�����ꂽ�\�͂̏ꍇ�A���j�b�g�p����\�͂ɊY�����Ȃ����
					'�p�C���b�g�p����\�͂Ƃ݂Ȃ�
					msg = SkillHelpMessage(u.MainPilot, ftype)
					If Len(msg) > 0 Then
						Exit Function
					End If
					
					'���̓_�~�[�\�́H
					If Len(fdata) > 0 Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
					
					'��������݂��Ȃ��H
					If Len(msg) = 0 Then
						Exit Function
					End If
				ElseIf Len(fdata) > 0 Then 
					'�_�~�[�\�͂̏ꍇ
					msg = ListIndex(fdata, ListLength(fdata))
					If Left(msg, 1) = """" Then
						msg = Mid(msg, 2, Len(msg) - 2)
					End If
				ElseIf ListIndex(u.AllFeatureData(fname), 1) <> "���" Then 
					'������Ȃ��ꍇ
					Exit Function
				End If
				
		End Select
		
		fdata = u.AllFeatureData(fname0)
		If ListIndex(fdata, 1) = "���" Then
			'������`���Ă���ꍇ
			msg = ListTail(fdata, 2)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'���g���̍ۂ́u�p�C���b�g�v�Ƃ�������g��Ȃ��悤�ɂ���
		If IsOptionDefined("���g��") Then
			ReplaceString(msg, "�p�C���b�g", "���j�b�g")
		End If
		
		FeatureHelpMessage = msg
	End Function
	
	'���j�b�g u �̕��큕�A�r���e�B���� atr �̖���
	Public Function AttributeName(ByRef u As Unit, ByRef atr As String, Optional ByVal is_ability As Boolean = False) As String
		Dim fdata As String
		
		Select Case atr
			Case "�S"
				AttributeName = "�S�Ă̍U��"
			Case "�i"
				AttributeName = "�i���n�U��"
			Case "��"
				AttributeName = "�ˌ��n�U��"
			Case "��"
				AttributeName = "�����Z"
			Case "�o"
				AttributeName = "�ړ���g�p�\�U��"
			Case "�p"
				AttributeName = "�ړ���g�p�s�\�U��"
			Case "�q"
				AttributeName = "���������"
			Case "��"
				AttributeName = "���������"
			Case "�U"
				AttributeName = "�U����p"
			Case "��"
				AttributeName = "������p"
			Case "��"
				AttributeName = "�i������"
			Case "��"
				AttributeName = "�ːi�Z"
			Case "��"
				AttributeName = "�ڋߐ�U��"
			Case "�i"
				AttributeName = "�W�����v�U��"
			Case "�a"
				AttributeName = "�r�[���U��"
			Case "��"
				AttributeName = "���e�U��"
			Case "�I"
				AttributeName = "�I�[���Z"
			Case "��"
				AttributeName = "�T�C�L�b�N�U��"
			Case "�V"
				AttributeName = "�������ΏۍU��"
			Case "�T"
				AttributeName = "�v�O�U���U��"
			Case "��"
				AttributeName = "�����͊��Z�U��"
			Case "�z"
				AttributeName = Term("�g�o", u) & "�z���U��"
			Case "��"
				AttributeName = Term("�d�m", u) & "�j��U��"
			Case "�D"
				AttributeName = Term("�d�m", u) & "�z���U��"
			Case "��"
				AttributeName = "�ђʍU��"
			Case "��"
				AttributeName = "�o���A�������U��"
			Case "��"
				AttributeName = "�򉻋Z"
			Case "��"
				AttributeName = "����Z"
			Case "��"
				AttributeName = "����Z"
			Case "�E"
				AttributeName = "���E�U��"
			Case "�Z"
				AttributeName = "�Z�I�U��"
			Case "�j"
				AttributeName = "�V�[���h�ђʍU��"
			Case "��"
				AttributeName = "�Βj���p�U��"
			Case "��"
				AttributeName = "�Ώ����p�U��"
			Case "�`"
				AttributeName = "�����[�U���U��"
			Case "�b"
				AttributeName = "�`���[�W���U��"
			Case "��"
				AttributeName = "���̋Z"
			Case "��"
				If Not is_ability Then
					AttributeName = "�e�򋤗L����"
				Else
					AttributeName = "�g�p�񐔋��L" & Term("�A�r���e�B", u)
				End If
			Case "��"
				AttributeName = "��Ĕ���"
			Case "�i"
				AttributeName = "�i������"
			Case "�p"
				AttributeName = "�p"
			Case "�Z"
				AttributeName = "�Z"
			Case "��"
				AttributeName = "���o�U��"
			Case "��"
				If Not is_ability Then
					AttributeName = "���g�U��"
				Else
					AttributeName = "���g" & Term("�A�r���e�B", u)
				End If
			Case "�C"
				AttributeName = Term("�C��", u) & "����U��"
			Case "��", "�v"
				AttributeName = "��͏���U��"
			Case "��"
				AttributeName = Term("�g�o", u) & "����U��"
			Case "�K"
				AttributeName = Term("����", u) & "����U��"
			Case "��"
				AttributeName = "���ՋZ"
			Case "��"
				AttributeName = "�����U��"
			Case "��"
				AttributeName = "�ό`�Z"
			Case "��"
				AttributeName = "�ԐڍU��"
			Case "�l��"
				AttributeName = "�����^�}�b�v�U��"
			Case "�l�g"
				AttributeName = "�g�U�^�}�b�v�U��"
			Case "�l��"
				AttributeName = "��^�}�b�v�U��"
			Case "�l�S"
				AttributeName = "�S���ʌ^�}�b�v�U��"
			Case "�l��"
				AttributeName = "�����^�}�b�v�U��"
			Case "�l��"
				AttributeName = "�ړ��^�}�b�v�U��"
			Case "�l��"
				AttributeName = "����}�b�v�U��"
			Case "��"
				AttributeName = "���ʌ^�}�b�v�U��"
			Case "��"
				AttributeName = "�ߔ��U��"
			Case "�r"
				AttributeName = "�V���b�N�U��"
			Case "��"
				AttributeName = "���b�򉻍U��"
			Case "��"
				AttributeName = "�o���A���a�U��"
			Case "��"
				AttributeName = "�Ή��U��"
			Case "��"
				AttributeName = "�����U��"
			Case "�"
				AttributeName = "��ჍU��"
			Case "��"
				AttributeName = "�Ö��U��"
			Case "��"
				AttributeName = "�����U��"
			Case "��"
				AttributeName = "�����U��"
			Case "��"
				AttributeName = "�߈ˍU��"
			Case "��"
				AttributeName = "�ڒׂ��U��"
			Case "��"
				AttributeName = "�ōU��"
			Case "�h"
				AttributeName = "�h���U��"
			Case "��"
				AttributeName = "���|�U��"
			Case "�s"
				AttributeName = "�U������U��"
			Case "�~"
				AttributeName = "���~�ߍU��"
			Case "��"
				AttributeName = "���ٍU��"
			Case "��"
				AttributeName = "������ʏ����U��"
			Case "��"
				AttributeName = "�����U��"
			Case "��"
				AttributeName = "���̐鍐"
			Case "�E"
				AttributeName = Term("�C��", u) & "�����U��"
			Case "�c"
				AttributeName = Term("�C��", u) & "�z���U��"
			Case "��U"
				AttributeName = "�U���͒ቺ�U��"
			Case "��h"
				AttributeName = "�h��͒ቺ�U��"
			Case "��^"
				AttributeName = Term("�^����", u) & "�ቺ�U��"
			Case "���"
				AttributeName = Term("�ړ���", u) & "�ቺ�U��"
			Case "��"
				AttributeName = "���_�U��"
			Case "��"
				AttributeName = "�搧�U��"
			Case "��"
				AttributeName = "��U�U��"
			Case "�A"
				AttributeName = "�A���U��"
			Case "��"
				AttributeName = "�čU��"
			Case "��"
				AttributeName = "������΂��U��"
			Case "�j"
				AttributeName = "�m�b�N�o�b�N�U��"
			Case "��"
				AttributeName = "�����񂹍U��"
			Case "�]"
				AttributeName = "�����]�ڍU��"
			Case "�E"
				AttributeName = "�ÎE�Z"
			Case "�s"
				AttributeName = "�S" & Term("�d�m", u) & "����U��"
			Case "��"
				AttributeName = "����"
			Case "�g"
				AttributeName = "�z�[�~���O�U��"
			Case "��"
				AttributeName = "���Ȓǔ��U��"
			Case "�L"
				AttributeName = "�L�����U���U��"
			Case "�U"
				AttributeName = "����U���U��"
			Case "��"
				AttributeName = "�����U��"
			Case "��"
				AttributeName = "�΋�U��"
			Case "��"
				AttributeName = "�_���[�W�Œ�U��"
			Case "��"
				AttributeName = Term("�g�o", u) & "�����U��"
			Case "��"
				AttributeName = Term("�d�m", u) & "�����U��"
			Case "�x"
				AttributeName = "�x�点�U��"
			Case "��"
				AttributeName = "����m���U��"
			Case "�]"
				AttributeName = "�]���r���U��"
			Case "�Q"
				AttributeName = "�񕜔\�͑j�Q�U��"
			Case "�K"
				AttributeName = "���[�j���O"
			Case "��"
				AttributeName = "�\�̓R�s�["
			Case "��"
				AttributeName = "�ω�"
			Case "��"
				AttributeName = "�N���e�B�J��"
			Case "��"
				AttributeName = "�x����p" & Term("�A�r���e�B", u)
			Case "��"
				AttributeName = "����x" & Term("�A�r���e�B", u)
			Case "�n", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��"
				AttributeName = atr & "����"
			Case "��"
				AttributeName = "���@�U��"
			Case "��"
				AttributeName = "���ԑ���U��"
			Case "�d"
				AttributeName = "�d�͍U��"
			Case "�e", "��", "��", "��", "��", "�|"
				AttributeName = atr & "�U��"
			Case "�e"
			Case "�@"
				AttributeName = "�΋@�B�p�U��"
			Case "��"
				AttributeName = "�΃G�X�p�[�p�U��"
			Case "��"
				AttributeName = "���E���̕���"
			Case "�m"
				AttributeName = "�m��������U��"
			Case "��"
				AttributeName = "���背�x������U��"
			Case "��"
				AttributeName = "���[�j���O�\�Z"
			Case "��"
				AttributeName = "�g�p�֎~"
			Case "��"
				AttributeName = "�ŏ��˒�"
			Case "�U"
				AttributeName = "�g�U�U��"
			Case Else
				If Left(atr, 1) = "��" Then
					AttributeName = Mid(atr, 2) & "������_�t���U��"
				ElseIf Left(atr, 1) = "��" Then 
					AttributeName = Mid(atr, 2) & "�����L���t���U��"
				ElseIf Left(atr, 1) = "��" Then 
					AttributeName = Mid(atr, 2) & "�����g�p�W�Q�U��"
				End If
		End Select
		
		If Not u Is Nothing Then
			fdata = u.FeatureData(atr)
			If ListIndex(fdata, 1) = "���" Then
				'������`���Ă���ꍇ
				AttributeName = ListIndex(fdata, 2)
				Exit Function
			End If
		End If
		
		If is_ability Then
			If Right(AttributeName, 2) = "�U��" Or Right(AttributeName, 2) = "����" Then
				AttributeName = Left(AttributeName, Len(AttributeName) - 2) & Term("�A�r���e�B", u)
			End If
		End If
	End Function
	
	'���j�b�g u �� idx �Ԗڂ̕��큕�A�r���e�B�̑��� atr �̉����\��
	Public Sub AttributeHelp(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, Optional ByVal is_ability As Boolean = False)
		Dim msg, aname As String
		Dim prev_mode As Boolean
		
		msg = AttributeHelpMessage(u, atr, idx, is_ability)
		
		'����̕\��
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("���b�Z�[�W�E�B���h�E")
			End If
			If InStr(atr, "L") > 0 Then
				aname = AttributeName(u, Left(atr, InStr(atr, "L") - 1), is_ability) & "���x��" & StrConv(VB6.Format(Mid(atr, InStr(atr, "L") + 1)), VbStrConv.Wide)
			Else
				aname = AttributeName(u, atr, is_ability)
			End If
			DisplayMessage("�V�X�e��", "<b>" & aname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'���j�b�g u �� idx �Ԗڂ̕��큕�A�r���e�B�̑��� atr �̉����\��
	Public Function AttributeHelpMessage(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, ByVal is_ability As Boolean) As String
		Dim atype As String
		Dim alevel As Double
		Dim msg, whatsthis As String
		Dim wanickname, waname, uname As String
		Dim p As Pilot
		Dim i, j As Short
		Dim buf As String
		Dim fdata As String
		
		'�������x���̎���
		If InStr(atr, "L") > 0 Then
			atype = Left(atr, InStr(atr, "L") - 1)
			alevel = CDbl(Mid(atr, InStr(atr, "L") + 1))
		Else
			atype = atr
			alevel = DEFAULT_LEVEL
		End If
		
		With u
			'����(�A�r���e�B)��
			If Not is_ability Then
				waname = .Weapon(idx).Name
				wanickname = .WeaponNickname(idx)
				whatsthis = "�U��"
			Else
				waname = .Ability(idx).Name
				wanickname = .AbilityNickname(idx)
				whatsthis = Term("�A�r���e�B", u)
			End If
			
			'���C���p�C���b�g
			p = .MainPilot
		End With
		
		Select Case atype
			Case "�i"
				msg = "�p�C���b�g��" & Term("�i��", u) & "���g���čU���͂��Z�o�B"
			Case "��"
				If p.HasMana() Then
					msg = "�p�C���b�g��" & Term("����", u) & "���g���čU���͂��Z�o�B"
				Else
					msg = "�p�C���b�g��" & Term("�ˌ�", u) & "���g���čU���͂��Z�o�B"
				End If
			Case "��"
				If p.HasMana() Then
					msg = "�i���Ɩ��@�̗������g�����U���B" & "�p�C���b�g��" & Term("�i��", u) & "��" & Term("����", u) & "��" & "���ϒl���g���čU���͂��Z�o����B"
				Else
					msg = "�i���Ǝˌ��̗������g�����U���B" & "�p�C���b�g��" & Term("�i��", u) & "��" & Term("�ˌ�", u) & "��" & "���ϒl���g���čU���͂��Z�o����B"
				End If
			Case "�o"
				msg = "�˒��ɂ�����炸�ړ���Ɏg�p�\�B"
			Case "�p"
				msg = "�˒��ɂ�����炸�ړ���͎g�p�s�\�B"
			Case "�U"
				msg = "�U�����ɂ̂ݎg�p�\�B"
			Case "��"
				msg = "�������ɂ̂ݎg�p�\�B"
			Case "�q"
				If alevel = DEFAULT_LEVEL Then
					msg = "���j�b�g�����N�����\�͂ɂ��U���͏㏸���ʏ�̔����B"
				Else
					msg = "���j�b�g�����N�����\�͂ɂ��U���͏㏸��" & VB6.Format(10 * alevel) & "���ɂȂ�B"
				End If
				msg = "���j�b�g�����N�����\�͂ɂ��U���͏㏸���ʏ�̔����B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					msg = "���j�b�g�����N�ɂ��U���͏㏸���ʏ�̔����B"
				Else
					msg = "���j�b�g�����N�ɂ��U���͏㏸��" & VB6.Format(10 * alevel) & "���ɂȂ�B"
				End If
			Case "��"
				msg = "���̕�����g���Ď��e�U���Ȃǂ�؂蕥�����Ƃ��\�B" & "�؂蕥���̑ΏۂɂȂ�B"
			Case "��"
				msg = "�؂蕥���̑ΏۂɂȂ�B"
			Case "��"
				msg = "�����Z���A����ɖ������ČJ��o���i����U���B;" & "�؂蕥�������B"
			Case "�i"
				msg = "�W�����v�U�����̒n�`�K�����w�肵�����x�������グ��B"
			Case "�a"
				msg = "�΃r�[���p�h��\�͂̑ΏۂɂȂ�B"
			Case "��"
				msg = "�؂蕥���ƌ}���̑ΏۂɂȂ�B"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ��_���[�W���ቺ���Ȃ��B"
				End If
			Case "�I"
				msg = "�p�C���b�g��" & p.SkillName0("�I�[��") & "���x���ɂ���čU���͂��ω��B"
			Case "��"
				msg = "�p�C���b�g��" & p.SkillName0("���\��") & "���x���ɂ���čU���͂��ω��B"
			Case "�V"
				msg = "�p�C���b�g��" & p.SkillName0("������") & "�ɂ���čU���͂��ω��B"
			Case "�T"
				msg = "�p�C���b�g��" & p.SkillName0("�����o") & "���x���ɂ���Ď˒����ω��B"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�����ɂ�閽�����ቺ���Ȃ��B�܂��A"
				End If
				msg = msg & "�d�b�l�ɂ��e�����󂯂Ȃ��B"
			Case "��"
				msg = "�����͂��U���͂Ɋ�����U���B���j�b�g��" & Term("�g�o", u) & "�ɂ���čU���͂��ω�����B"
			Case "�z"
				msg = "�^�����_���[�W�̂P�^�S���z�����A������" & Term("�g�o", u) & "�ɕϊ��B"
			Case "��"
				msg = Term("�g�o", u) & "�Ƀ_���[�W��^����Ɠ����ɑ����" & Term("�d�m", u) & "������������B"
			Case "�D"
				msg = Term("�g�o", u) & "�Ƀ_���[�W��^����Ɠ����ɑ����" & Term("�d�m", u) & "�����������A" & "����������" & Term("�d�m", u) & "�̔����������̂��̂ɂ���B"
			Case "��"
				If alevel > 0 Then
					msg = "�����" & Term("���b", u) & "��{����" & VB6.Format(100 - 10 * alevel) & "���̒l�Ƃ݂Ȃ��ă_���[�W�v�Z���s���B"
				Else
					msg = "�����" & Term("���b", u) & "�𔼕��Ƃ݂Ȃ��ă_���[�W�v�Z���s���B"
				End If
			Case "��"
				msg = "�o���A��t�B�[���h�Ȃǂ̖h��\�͂̌��ʂ𖳎����ă_���[�W��^����B"
			Case "�Z"
				msg = "�V�[���h�h��𖳎����ă_���[�W��^����B"
			Case "�j"
				msg = "�V�[���h�h��̌��ʂ𔼌�������B"
			Case "��"
				msg = "�G��" & p.SkillName0("�Đ�") & "�\�͂𖳌����B"
			Case "��"
				msg = "����̎�_�����G�ɂ̂ݗL���ȕ����B" & "��_�������Ƃ��ɂ̂݃_���[�W��^���邱�Ƃ��o����B"
			Case "��"
				msg = "����̎�_�����G�ɂ̂ݗL���ȕ����B" & "���葮���ȍ~�Ɏw�肵��������;" & "��_�������Ƃ��ɂ̂݃_���[�W��^���邱�Ƃ��o����B"
			Case "�E"
				msg = "������ꌂ�œ|����ꍇ�ɂ̂ݗL���ȍU���B;" & "����͖h�䁕�V�[���h�h��o���Ȃ��B"
			Case "��"
				msg = "�j���ɂ̂ݗL���B"
			Case "��"
				msg = "�����ɂ̂ݗL���B"
			Case "�b"
				msg = "�`���[�W�R�}���h���g�p���ă`���[�W�����̏�ԂɂȂ�Ȃ��Ǝg�p�s�\�B"
			Case "�`"
				msg = "�g�p�����" & VB6.Format(alevel) & "�^�[����ɍă`���[�W����������܂Ŏg�p�s�\�B"
				If Not is_ability Then
					For i = 1 To u.CountWeapon
						If i <> idx And wanickname = u.WeaponNickname(i) Then
							msg = msg & "�����̕�����A�����Ďg�p�s�\�ɂȂ�B"
							Exit For
						End If
					Next 
					If u.IsWeaponClassifiedAs(idx, "��") And u.Weapon(idx).Bullet = 0 Then
						msg = msg & "�����x���̒e�򋤗L������A�����Ďg�p�s�\�ɂȂ�B"
					End If
				Else
					For i = 1 To u.CountAbility
						If i <> idx And wanickname = u.AbilityNickname(i) Then
							msg = msg & "������" & Term("�A�r���e�B", u) & "���A�����Ďg�p�s�\�ɂȂ�B"
							Exit For
						End If
					Next 
					If u.IsAbilityClassifiedAs(idx, "��") And u.Ability(idx).Stock = 0 Then
						msg = msg & "�����x���̎g�p�񐔋��L" & Term("�A�r���e�B", u) & "���A�����Ďg�p�s�\�ɂȂ�B"
					End If
				End If
			Case "��"
				For i = 1 To u.CountFeature
					If u.Feature(i) = "���̋Z" And LIndex(u.FeatureData(i), 1) = waname Then
						Exit For
					End If
				Next 
				If i > u.CountFeature Then
					ErrorMessage(u.Name & "�̍��̋Z�u" & waname & "�v�ɑΉ��������̋Z�\�͂�����܂���")
					Exit Function
				End If
				If LLength(u.FeatureData(i)) = 2 Then
					uname = LIndex(u.FeatureData(i), 2)
					If UDList.IsDefined(uname) Then
						uname = UDList.Item(uname).Nickname
					End If
					If uname = u.Nickname Then
						msg = "����" & uname & "�Ƌ��͂��čs���Z�B"
					Else
						msg = uname & "�Ƌ��͂��čs���Z�B"
					End If
				Else
					msg = "�ȉ��̃��j�b�g�Ƌ��͂��čs���Z�B;"
					For j = 2 To LLength(u.FeatureData(i))
						uname = LIndex(u.FeatureData(i), j)
						If UDList.IsDefined(uname) Then
							uname = UDList.Item(uname).Nickname
						End If
						msg = msg & uname & "  "
					Next 
				End If
			Case "��"
				If Not is_ability Then
					msg = "�����̕���Œe������L���Ă��邱�Ƃ������B"
					If alevel > 0 Then
						msg = msg & ";�����x���̒e�򋤗L����ԂŒe������L���Ă���B"
					End If
				Else
					msg = "������" & Term("�A�r���e�B", u) & "�Ŏg�p�񐔂����L���Ă��邱�Ƃ������B"
					If alevel > 0 Then
						msg = msg & ";�����x���̎g�p�񐔋��L" & Term("�A�r���e�B", u) & "�ԂŎg�p�񐔂����L���Ă���B"
					End If
				End If
			Case "��"
				If Not is_ability Then
					msg = "�e�����̕���S�Ă̒e��������čU�����s���B"
				Else
					msg = "�񐔐���" & Term("�A�r���e�B", u) & "�S�Ă̎g�p�񐔂������B"
				End If
			Case "�i"
				msg = "�؂蕥����}������Ȃ�����e�����������Ȃ��B"
			Case "�p"
				buf = p.SkillName0("�p")
				If buf = "��\��" Then
					buf = "�p"
				End If
				msg = buf & "�Z�\�ɂ����" & Term("�d�m", u) & "����ʂ������B"
				If is_ability Then
					msg = msg & ";�p�C���b�g��" & Term("����", u) & "�ɂ���ĈЗ͂���������B"
				End If
				msg = msg & ";���ُ�Ԃ̎��ɂ͎g�p�s�\�"
			Case "�Z"
				buf = p.SkillName0("�Z")
				If buf = "��\��" Then
					buf = "�Z"
				End If
				msg = buf & "�Z�\�ɂ����" & Term("�d�m", u) & "����ʂ������B"
			Case "��"
				If Not is_ability Then
					msg = "���Ȃǂ̉����g�����U���ł��邱�Ƃ������"
				Else
					msg = "���Ȃǂ̉����g����" & Term("�A�r���e�B", u) & "�ł��邱�Ƃ������"
				End If
				msg = msg & "���ُ�Ԃ̎��ɂ͎g�p�s�\� "
			Case "��"
				msg = "���o�ɓ���������U���B�Ӗڏ�Ԃ̃��j�b�g�ɂ͌����Ȃ��B"
			Case "�C"
				msg = "�g�p���ɋC��" & VB6.Format(5 * alevel) & "������B"
			Case "��", "�v"
				msg = "�g�p����" & VB6.Format(5 * alevel) & p.SkillName0("���") & "������B"
			Case "��"
				msg = "�g�p����" & VB6.Format(alevel * u.MaxHP \ 10) & "��" & Term("�g�o", u) & "�������B"
			Case "�K"
				msg = "�g�p����" & VB6.Format(MaxLng(alevel, 1) * u.Value \ 10) & "��" & Term("����", u) & "���K�v�B;" & Term("����", u) & "������Ȃ��ꍇ�͎g�p�s�B"
			Case "��"
				msg = "�g�p���1�^�[�����Տ�ԂɊׂ�A����E�����s�\�B"
			Case "�s"
				If Not is_ability Then
					If alevel > 0 Then
						msg = "�S" & Term("�d�m", u) & "���g���čU�����A�g�p���" & Term("�d�m", u) & "��0�ɂȂ�B;" & "(�c��" & Term("�d�m", u) & "�|�K�v" & Term("�d�m", u) & ")�~" & StrConv(VB6.Format(alevel), VbStrConv.Wide) & "�����U���͂��㏸�B"
					Else
						msg = "�S" & Term("�d�m", u) & "���g���čU�����A�g�p��ɂd�m��0�ɂȂ�B"
					End If
				Else
					msg = "�g�p���" & Term("�d�m", u) & "��0�ɂȂ�B"
				End If
			Case "��"
				msg = "�g�p��Ɏ����B"
			Case "��"
				If u.IsFeatureAvailable("�ό`�Z") Then
					For i = 1 To u.CountFeature
						If u.Feature(i) = "�ό`�Z" And LIndex(u.FeatureData(i), 1) = waname Then
							uname = LIndex(u.FeatureData(i), 2)
							Exit For
						End If
					Next 
				End If
				If uname = "" Then
					uname = LIndex(u.FeatureData("�m�[�}�����[�h"), 1)
				End If
				If UDList.IsDefined(uname) Then
					With UDList.Item(uname)
						If u.Nickname <> .Nickname Then
							uname = .Nickname
						Else
							uname = .Name
						End If
					End With
				End If
				msg = "�g�p���" & uname & "�֕ω�����B"
			Case "��"
				msg = "���E�O�Ȃǂ���ԐړI�ɍU�����s�����Ƃɂ��" & "����̔����𕕂��镐��B"
			Case "�l��"
				msg = "�㉺���E�̈�����ɑ΂��钼����̌��ʔ͈͂����B"
			Case "�l�g"
				msg = "�㉺���E�̈�����ɑ΂��镝�R�}�X�̒�����̌��ʔ͈͂����B"
			Case "�l��"
				msg = "�㉺���E�̈�����ɑ΂�����̌��ʔ͈͂����B;" & "��̍L������̓x�����̓��x���ɂ���ĈقȂ�B"
			Case "�l�S"
				msg = "���j�b�g�̎���S��ɑ΂�����ʔ͈͂����B"
			Case "�l��"
				msg = "�w�肵���n�_�𒆐S�Ƃ������͈͂̌��ʔ͈͂����B"
			Case "�l��"
				msg = "�g�p��Ɏw�肵���n�_�܂Ń��j�b�g���ړ����A" & "���j�b�g���ʉ߂����ꏊ�����ʔ͈͂ɂȂ�B"
			Case "�l��"
				msg = "�w�肵���n�_�ƃ��j�b�g�����Ԓ��������ʔ͈͂ɂȂ�B"
			Case "��"
				msg = "���ʔ͈͓��ɂ��閡�����j�b�g�������I�Ɏ��ʂ��A�G�݂̂Ƀ_���[�W��^����B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�s���s�\�ɂ���B"
			Case "�r"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�s���s�\�ɂ���B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					msg = "�N���e�B�J���������ɑ���̑��b�𔼌�������B"
				Else
					msg = "�N���e�B�J���������ɑ���̑��b��"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
					Else
						msg = msg & "���̐퓬���̂�"
					End If
					msg = msg & "����������B"
				End If
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "�N���e�B�J���������ɑ��肪���o���A���̖h��\�͂�"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "����������B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					msg = "�N���e�B�J���������ɑ����Ή�������B"
				Else
					msg = "�N���e�B�J���������ɑ����"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
					Else
						msg = msg & "���̐퓬���̂�"
					End If
					msg = msg & "�Ή�������B"
				End If
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "���点��B"
				msg = msg & ";�������������" & Term("���b", u) & "���������邪�A"
				msg = msg & "�_���[�W��^����Ɠ����͉��������B"
			Case "�"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "��Ⴢ�����B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "���点��B"
				msg = msg & ";����������ւ̍U���̃_���[�W�͂P.�T�{�ɂȂ邪�A���������������B"
				msg = msg & ";���i���@�B�̓G�ɂ͖����B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "����������B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "��������B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					msg = "�N���e�B�J���������ɑ�����������Ďx�z����B"
				Else
					msg = "�N���e�B�J���������ɑ����"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
					Else
						msg = msg & "���̐퓬���̂�"
					End If
					msg = msg & "�������Ďx�z����B"
				End If
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�Ӗڂɂ���B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ŏ�Ԃɂ���B"
			Case "�h"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�h������B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "���|�Ɋׂ��B"
			Case "�s"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�U���s�\�ɂ���B"
			Case "�~"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ړ��s�\�ɂ���B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "���ُ�Ԃɂ���B"
			Case "��"
				If Not is_ability Then
					msg = "�N���e�B�J���������ɑ���ɂ�����ꂽ" & Term("�A�r���e�B", u) & "�ɂ�������ʂ�ł������B"
				Else
					msg = Term("�A�r���e�B", u) & "���s���ɁA����܂łɑ���ɂ������Ă���" & Term("�A�r���e�B", u) & "�ɂ�������ʂ����������B"
				End If
			Case "��"
				msg = "�N���e�B�J���������ɑ���𑦎�������B"
			Case "��"
				If alevel > 0 Then
					msg = "�N���e�B�J���������ɑ�����u���̐鍐�v��Ԃɂ��A" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[�����" & Term("�g�o", u) & "���P�ɂ���B"
				Else
					msg = "�N���e�B�J���������ɑ����" & Term("�g�o", u) & "���P�ɂ���B"
				End If
			Case "�E"
				If alevel = DEFAULT_LEVEL Then
					msg = "�����" & Term("�C��", u) & "��10�ቺ������B"
				ElseIf alevel >= 0 Then 
					msg = "�����" & Term("�C��", u) & "��" & VB6.Format(CShort(5 * alevel)) & "�ቺ������B"
				Else
					msg = "�����" & Term("�C��", u) & "��" & VB6.Format(CShort(-5 * alevel)) & "����������B"
				End If
			Case "�c"
				If alevel = DEFAULT_LEVEL Then
					msg = "�����" & Term("�C��", u) & "��10�ቺ�����A���̔������z������B"
				ElseIf alevel >= 0 Then 
					msg = "�����" & Term("�C��", u) & "��" & VB6.Format(CShort(5 * alevel)) & "�ቺ�����A���̔������z������B"
				Else
					msg = "�����" & Term("�C��", u) & "��" & VB6.Format(CShort(-5 * alevel)) & "���������A���̔�����^����B"
				End If
			Case "��U"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ���̍U���͂�"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ቺ������B"
			Case "��h"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����" & Term("���b", u) & "��"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ቺ������B"
			Case "��^"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����" & Term("�^����", u) & "��"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ቺ������B"
			Case "���"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����" & Term("�ړ���", u) & "��"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�ቺ������B�B"
			Case "��"
				msg = "�������ł��������ɍU������B"
			Case "��"
				msg = "�������ł͂Ȃ��ꍇ���������ɍU������B"
			Case "��"
				If alevel > 0 Then
					msg = "���胆�j�b�g��" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�}�X������΂��B;" & "�N���e�B�J���������͐�����΂������{�P�B"
				Else
					msg = "�N���e�B�J���������ɑ��胆�j�b�g���P�}�X������΂��B"
				End If
			Case "�j"
				If alevel > 0 Then
					msg = "���胆�j�b�g��" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�}�X������΂��B;" & "�N���e�B�J���������͐�����΂������{�P�B" & Term("�T�C�Y", u) & "��������B"
				Else
					msg = "�N���e�B�J���������ɑ��胆�j�b�g���P�}�X������΂��B" & Term("�T�C�Y", u) & "��������B"
				End If
			Case "��"
				msg = "�N���e�B�J���������ɑ��胆�j�b�g��אڂ���}�X�܂ň����񂹂�B"
			Case "�]"
				msg = "�N���e�B�J���������ɑ��胆�j�b�g��" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�}�X�����e���|�[�g������B�e���|�[�g��̓����_���ɑI�΂��B"
			Case "�A"
				msg = VB6.Format(alevel) & "��A�����čU�����s���B;" & "�U���ɂ���ė^����_���[�W�͉��L�̎��Ōv�Z�����B;" & "  �ʏ�̃_���[�W�� �~ ������ �^ �U����"
			Case "��"
				msg = VB6.Format(100 * alevel \ 16) & "%�̊m���ōčU���B"
			Case "��"
				msg = "���_�ɓ���������U���B���i���u�@�B�v�̃��j�b�g�ɂ͌����Ȃ��B" & "�V�[���h�𖳌����B"
			Case "��"
				msg = "�����ȊO�̃��j�b�g�ɑ΂��Ă̂ݎg�p�\�B"
			Case "��"
				msg = VB6.Format(10 * alevel) & "%�̊m���Ŏ��s����B"
			Case "�E"
				msg = "�����𗧂Ă��ɍU�����A" & "�X�e���X��Ԃ̍ۂ�" & Term("�b�s��", u) & "��+10�̃{�[�i�X�B" & "�ꌂ�ő����|�����ꍇ�͎�������U���������Ă��X�e���X��Ԃ��ێ������B"
			Case "��"
				msg = "�N���e�B�J���������ɓG���玝�����𓐂ށB;" & "���߂���̂͒ʏ��" & Term("����", u) & "(���ʂɓ|�������̔����̊z)�����A" & "����ɂ���Ă̓A�C�e������肷�邱�Ƃ�����B"
			Case "�g"
				msg = "���[�_�[���Ń^�[�Q�b�g��ǔ�����U���B;"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ����������ቺ���Ȃ����A"
				End If
				msg = msg & "�d�b�l�ɂ��e���������󂯂�B"
				msg = msg & "�U�������h�����̏�ԂɊׂ��Ă����������ቺ���Ȃ��B"
			Case "��"
				msg = "���Ȕ��f�\�͂������A�^�[�Q�b�g��ǔ�����U���B;"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ����������ቺ���Ȃ��B�܂��A"
				End If
				msg = msg & "�U�������h�����̏�ԂɊׂ��Ă����������ቺ���Ȃ��B"
			Case "�L"
				msg = "�L���ɂ��U���Ń^�[�Q�b�g��ǔ�����U���B;"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ����������ቺ���Ȃ��B�܂��A"
				End If
				msg = msg & "�d�b�l�ɂ��e�����󂯂Ȃ��B"
				msg = msg & "�������A�X�y�V�����p���[��" & "�A�C�e���̌��ʂɂ���Ď˒����������Ȃ��B"
			Case "�U"
				msg = "�d�g�W�Q���󂯂Ȃ�����Ȏ�i�ɂ��U���Ń^�[�Q�b�g��ǔ�����U���B;"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ����������ቺ���Ȃ��B�܂��A"
				End If
				msg = msg & "�d�b�l�ɂ��e�����󂯂Ȃ��B"
			Case "��"
				msg = "�����ɂ��_���[�W��^����U���B;"
				If IsOptionDefined("�����C��") Then
					msg = msg & "�������̓G���U������ۂ��_���[�W���ቺ���Ȃ��B"
				End If
			Case "��"
				msg = "�󒆂ɂ���^�[�Q�b�g���U�����邱�Ƃ�ړI�Ƃ����U���B"
				If IsOptionDefined("���x�C��") Then
					msg = msg & "�n�ォ��󒆂ɂ���G���U������ۂɖ��������ቺ���Ȃ��B"
				End If
			Case "��"
				msg = "�p�C���b�g��" & Term("�C��", u) & "��U���́A�h�䑤��" & Term("���b", u) & "�ɂ�����炸" & "����̍U���͂Ɠ����_���[�W��^����U���B" & "�������A���j�b�g�����N���オ���Ă��U���͂͑����Ȃ��B" & Term("�X�y�V�����p���[", u) & "��" & Term("�n�`�K��", u) & "�ɂ��_���[�W�C���͗L���B"
			Case "��"
				msg = "�N���e�B�J���������ɓG��" & Term("�g�o", u) & "�����ݒl�� "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & " �܂Ō���������B"
			Case "��"
				msg = "�N���e�B�J���������ɓG��" & Term("�d�m", u) & "�����ݒl�� "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & " �܂Ō���������B"
			Case "�x"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "�x�点��B"
			Case "��"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "�N���e�B�J���������ɑ����"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
				Else
					msg = msg & "���̐퓬���̂�"
				End If
				msg = msg & "����m��Ԃɂ���B"
			Case "�]"
				If alevel = DEFAULT_LEVEL Then
					msg = "�N���e�B�J���������ɑ�����]���r��Ԃɂ���B"
				Else
					msg = "�N���e�B�J���������ɑ����"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
					Else
						msg = msg & "���̐퓬���̂�"
					End If
					msg = msg & "�]���r��Ԃɂ���B"
				End If
			Case "�Q"
				If alevel = DEFAULT_LEVEL Then
					msg = "�N���e�B�J���������ɑ���̎��ȉ񕜔\�͂�j�󂷂�B"
				Else
					msg = "�N���e�B�J���������ɑ����"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
					Else
						msg = msg & "���̐퓬���̂�"
					End If
					msg = msg & "���ȉ񕜕s�\��Ԃɂ���B"
				End If
			Case "�K"
				msg = "�N���e�B�J���������ɑ���̎��Z���K���o����B;" & "�������A�K���\�ȋZ�𑊎肪�����Ă��Ȃ���Ζ����B"
			Case "��"
				msg = "�N���e�B�J���������ɑ��胆�j�b�g�ɕϐg����B;" & "�������A���ɕϐg���Ă���ꍇ�͎g�p�ł��Ȃ��B" & "�܂��A����ƂQ�i�K�ȏ�" & Term("�T�C�Y", u) & "���قȂ�ꍇ�͖����B"
			Case "��"
				msg = "�N���e�B�J���������ɑ��胆�j�b�g�ɕϐg����B;" & "�������A���ɕϐg���Ă���ꍇ�͎g�p�ł��Ȃ��B"
			Case "��"
				msg = "�N���e�B�J���������ɒʏ�� "
				If IsOptionDefined("�_���[�W�{���ቺ") Then
					msg = msg & VB6.Format(100 + 10 * (alevel + 2))
				Else
					msg = msg & VB6.Format(100 + 25 * (alevel + 2))
				End If
				msg = msg & "% �̃_���[�W��^����B"
			Case "�n", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��"
				Select Case atype
					Case "��", "��", "��"
						msg = atype & "���g����"
					Case "��", "��", "��"
						msg = atype & "�̗͂��g����"
					Case "�n"
						msg = "��n�̗͂��؂肽"
					Case "��"
						msg = "��C�ɂ��"
					Case "��"
						msg = "�d���ɂ��"
					Case "��"
						msg = "���Ȃ�͂��؂肽"
					Case "��"
						msg = "���؂̗͂��؂肽"
				End Select
				msg = msg & whatsthis & "�B"
			Case "��"
				If Not is_ability Then
					msg = "���͂�тт��U���B"
				Else
					msg = "���@�ɂ��" & Term("�A�r���e�B", u) & "�B"
				End If
			Case "��"
				msg = "���̗���𑀂�" & whatsthis & "�B"
			Case "�d"
				msg = "�d�͂��g�����U���B"
			Case "�e", "��", "��", "��", "��", "�|"
				msg = atype & "���g�����U���B"
			Case "�@"
				msg = "�@�B(���{�b�g�A�A���h���C�h)�ɑ΂����ɗL���ȍU���B"
			Case "��"
				msg = "�G�X�p�[(���\�͎�)�ɑ΂����ɗL���ȍU���B"
			Case "��"
				msg = "����(�h���S��)�ɑ΂����ɗL���ȕ���B"
			Case "�m"
				msg = "�m�����ɂ̂ݎg�p�\��" & whatsthis & "�B"
			Case "��"
				msg = "���݂̏󋵉��ł͎g�p���邱�Ƃ��o���܂���B"
			Case "��"
				If Not is_ability Then
					whatsthis = "�U��"
				End If
				msg = "����̃��C���p�C���b�g�̃��x����" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�̔{���̏ꍇ�ɂ̂ݗL����" & whatsthis & "�B"
			Case "��"
				If Not is_ability Then
					whatsthis = "�U��"
				End If
				msg = "���[�j���O���\��" & whatsthis & "�B"
			Case "��"
				msg = "�ŏ��˒���" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�ɂȂ�B"
			Case "�U"
				msg = "���肩��Q�}�X�ȏ㗣��Ă���Ɩ��������㏸���A�^����_���[�W����������B"
			Case Else
				'��A���A������
				Select Case Left(atype, 1)
					Case "��"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "�N���e�B�J���������ɑ����" & Mid(atype, 2) & "�����ɑ΂����_��"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
						Else
							msg = msg & "���̐퓬���̂�"
						End If
						msg = msg & "�t������B"
					Case "��"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "�N���e�B�J���������ɑ����" & Mid(atype, 2) & "�����ɑ΂���L����"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
						Else
							msg = msg & "���̐퓬���̂�"
						End If
						msg = msg & "�t������B"
					Case "��"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "�N���e�B�J���������ɑ����"
						Select Case Mid(atype, 2)
							Case "�I"
								msg = msg & "�I�[��"
							Case "��"
								msg = msg & "���\��"
							Case "�V"
								msg = msg & "������"
							Case "�T"
								msg = msg & "�����o�A�m�o����"
							Case "��"
								msg = msg & "���"
							Case "�p"
								msg = msg & "�p"
							Case "�Z"
								msg = msg & "�Z"
							Case Else
								msg = msg & Mid(atype, 2) & "�����̕���A�A�r���e�B"
						End Select
						msg = msg & "��"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "�^�[��"
						Else
							msg = msg & "���̐퓬���̂�"
						End If
						msg = msg & "�g�p�s�\�ɂ���B"
				End Select
		End Select
		
		fdata = u.FeatureData(atype)
		If ListIndex(fdata, 1) = "���" Then
			'������`���Ă���ꍇ
			msg = ListTail(fdata, 3)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'���g���̍ۂ́u�p�C���b�g�v�Ƃ�������g��Ȃ��悤�ɂ���
		If IsOptionDefined("���g��") Then
			ReplaceString(msg, "���C���p�C���b�g", "���j�b�g")
			ReplaceString(msg, "�p�C���b�g", "���j�b�g")
			ReplaceString(msg, "����̃��j�b�g", "���胆�j�b�g")
		End If
		
		AttributeHelpMessage = msg
	End Function
End Module