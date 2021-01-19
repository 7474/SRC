Option Strict Off
Option Explicit On
Module Effect
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'������ʂ̎����I�����Đ�����
	
	
	'�\���Ă��镐��̎��
	Private WeaponInHand As String
	
	'�U����i�̎��
	Private CurrentWeaponType As String
	
	
	'�퓬�A�j���Đ��p�T�u���[�`��
	Public Sub ShowAnimation(ByRef aname As String)
		Dim buf As String
		Dim ret As Double
		Dim i As Short
		Dim expr As String
		
		If Not BattleAnimation Then
			Exit Sub
		End If
		
		'�E�N���b�N���͓�����ʂ��X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'�T�u���[�`���Ăяo���̂��߂̎����쐬
		expr = LIndex(aname, 1)
		If InStr(expr, "�퓬�A�j��_") <> 1 Then
			expr = "�퓬�A�j��_" & LIndex(aname, 1)
		End If
		If FindNormalLabel(expr) = 0 Then
			ErrorMessage("�T�u���[�`���u" & expr & "�v��������܂���")
			Exit Sub
		End If
		expr = "Call(`" & expr & "`"
		For i = 2 To LLength(aname)
			expr = expr & ",`" & LIndex(aname, i) & "`"
		Next 
		expr = expr & ")"
		
		'�摜�`�悪�s��ꂽ���ǂ����̔���̂��߂Ƀt���O��������
		IsPictureDrawn = False
		
		'���b�Z�[�W�E�B���h�E�̏�Ԃ��L�^
		SaveMessageFormStatus()
		
		'�퓬�A�j���Đ�
		SaveBasePoint()
		CallFunction(expr, Expression.ValueType.StringType, buf, ret)
		RestoreBasePoint()
		
		'���b�Z�[�W�E�B���h�E�̏�Ԃ��ω����Ă���ꍇ�͕���
		KeepMessageFormStatus()
		
		'�摜���������Ă���
		If IsPictureDrawn And LCase(buf) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
		
		Exit Sub
		
ErrorHandler: 
		
		'�퓬�A�j�����s���ɔ��������G���[�̏���
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Sub
	
	
	'���폀�����̓������
	Public Sub PrepareWeaponEffect(ByRef u As Unit, ByVal w As Short)
		'�E�N���b�N���͓�����ʂ��X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			PrepareWeaponAnimation(u, w)
		Else
			PrepareWeaponSound(u, w)
		End If
	End Sub
	
	'���폀�����̃A�j���[�V����
	Public Sub PrepareWeaponAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wclass, wname, wtype As String
		Dim double_weapon As Boolean
		Dim sname, aname, cname As String
		Dim with_face_up As Boolean
		Dim i As Short
		
		'�퓬�A�j���񎩓��I��
		If IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			Exit Sub
		End If
		
		With u
			'�܂������A�j���\���̍ۂɃt�F�C�X�A�b�v��\�����邩���肷��
			If .CountWeapon >= 4 And w >= .CountWeapon - 1 And .Weapon(w).Power >= 1800 And ((.Weapon(w).Bullet > 0 And .Weapon(w).Bullet <= 4) Or .Weapon(w).ENConsumption >= 35) Then
				'�S�ȏ�̕���������j�b�g�����̃��j�b�g�̍ō��З�
				'�������͂Q�Ԗڂɋ��͂ȕ�����g�p���A
				'���̕���̍U����1800�ȏ�ł�����g�p�\�񐔂����肳��Ă����
				'�K�E�Z�ƌ��Ȃ��ăt�F�C�X�A�b�v�\��
				'            with_face_up = True
			End If
			
			'�󒆈ړ���p�`�Ԃ͕������ō\���Ȃ�
			If .Data.Transportation = "��" Then
				WeaponInHand = ""
				GoTo SkipWeaponAnimation
			End If
			
			'���g���̏ꍇ�A��l�ԃ��j�b�g�̓��J�ł��邱�Ƃ������̂œ��������D�悷��
			If IsOptionDefined("���g��") And Not .IsHero() Then
				WeaponInHand = ""
				GoTo SkipWeaponAnimation
			End If
			
			wname = .WeaponNickname(w)
			wclass = .Weapon(w).Class_Renamed
		End With
		
		'���폀���̃A�j���[�V�������\���ɂ���I�v�V������I�����Ă���H
		' MOD START MARGE
		'    If Not WeaponAnimation Or IsOptionDefined("���폀���A�j����\��") Then
		If (Not WeaponAnimation And Not ExtendedAnimation) Or IsOptionDefined("���폀���A�j����\��") Then
			' MOD END MARGE
			WeaponInHand = ""
			GoTo SkipWeaponAnimation
		End If
		
		'�񓁗��H
		If InStr(wname, "�_�u��") > 0 Or InStr(wname, "�c�C��") > 0 Or InStr(wname, "�o") > 0 Or InStr(wname, "��") > 0 Then
			double_weapon = True
		End If
		
		'�u�u�[���v�Ƃ������ʉ���炷�H
		If InStr(wname, "�����g") > 0 Or InStr(wname, "�d��") > 0 Then
			sname = "BeamSaber.wav"
		End If
		
		'���ꂩ�畐��̎�ނ𔻒�
		
		If InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 Then
			GoTo SkipInfightWeapon
		End If
		
		'���햼���畐��̎�ނ𔻒�
		wtype = CheckWeaponType(wname, wclass)
		If wtype = "�藠��" Then
			'�藠���͍\�����ɂ����Ȃ蓊�����ق��������������Ǝv���̂�
			Exit Sub
		End If
		If wtype <> "" Then
			GoTo FoundWeaponType
		End If
		
		'�ڍׂ�������Ȃ���������
		If InStrNotNest(wclass, "��") > 0 Then
			'�������Ă���A�C�e�����畐�������
			For i = 1 To u.CountItem
				With u.Item(i)
					If .Activated And (.Part = "����" Or .Part = "�Ў�" Or .Part = "����") Then
						wtype = CheckWeaponType(.Nickname, "")
						If wtype <> "" Then
							GoTo FoundWeaponType
						End If
						wtype = CheckWeaponType(.Class0, "")
						If wtype <> "" Then
							GoTo FoundWeaponType
						End If
						Exit For
					End If
				End With
			Next 
			GoTo SkipShootingWeapon
		End If
		
		If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
			GoTo SkipShootingWeapon
		End If
		
SkipInfightWeapon: 
		
		'�܂��̓r�[���U�����ǂ�������
		If Not IsBeamWeapon(wname, wclass, cname) Then
			GoTo SkipBeamWeapon
		End If
		
		'�莝���H
		If InStr(wname, "���C�t��") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Or Right(wname, 2) = "�K��" Or (Right(wname, 1) = "�e" And Right(wname, 2) <> "�@�e") Then
			If InStrNotNest(wclass, "�l") > 0 Then
				wtype = "�l�`�o�o�X�^�[�r�[�����C�t��"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�n�C���K") > 0 Or InStr(wname, "�o�X�^�[") > 0 Or InStr(wname, "��") > 0 Or Left(wname, 2) = "�M�K" Then
				wtype = "�o�X�^�[�r�[�����C�t��"
			ElseIf InStr(wname, "���K") > 0 Or InStr(wname, "�n�C") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Then 
				If double_weapon Then
					wtype = "�_�u���r�[�������`���["
				Else
					wtype = "�r�[�������`���["
				End If
				If InStr(wname, "���C�t��") > 0 Then
					wtype = "�o�X�^�[�r�[�����C�t��"
				End If
			ElseIf CountAttack0(u, w) >= 4 Then 
				wtype = "�}�V���K��"
			ElseIf InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "�~�j") > 0 Or InStr(wname, "��") > 0 Then 
				wtype = "���[�U�[�K��"
			Else
				If double_weapon Then
					wtype = "�_�u���r�[�����C�t��"
				Else
					wtype = "�r�[�����C�t��"
				End If
			End If
			GoTo FoundWeaponType
		End If
		
SkipBeamWeapon: 
		
		If InStr(wname, "�|") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�����O�{�E") > 0 Then
			wtype = "�|"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�N���X�{�E") > 0 Or InStr(wname, "�{�E�K��") > 0 Then
			wtype = "�N���X�{�E"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�o�Y�[�J") > 0 Then
			wtype = "�o�Y�[�J"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�T�u�}�V���K��") > 0 Then
			wtype = "�T�u�}�V���K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�@�֏e") > 0 Then
			If InStr(wname, "�w�r�[") > 0 Or InStr(wname, "�d") > 0 Then
				wtype = "�w�r�[�}�V���K��"
			Else
				wtype = "�}�V���K��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�K�g�����O") > 0 Then
			wtype = "�K�g�����O"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "���e") > 0 Then
			wtype = "�s�X�g��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���{���o�[") > 0 Or InStr(wname, "���{�����@�[") > 0 Then
			wtype = "���{���o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���b�g�K��") > 0 Or InStr(wname, "���C�A�b�g�K��") > 0 Then
			wtype = "�V���b�g�K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�[�p�[�K��") > 0 Then
			wtype = "�X�[�p�[�K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�[�p�[�L���m��") > 0 Then
			wtype = "�X�[�p�[�L���m��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���C�t��") > 0 Or (Right(wname, 1) = "�e" And Right(wname, 2) <> "�@�e") Or Right(wname, 2) = "�K��" Then
			wtype = "���C�t��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�ΐ�ԃ��C�t��") > 0 Then
			wtype = "�ΐ�ԃ��C�t��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ε����C�t��") > 0 Then
			wtype = "�Ε����C�t��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���Ί�") > 0 Then
			wtype = "���Ί�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "���ˊ�") > 0 Then
			wtype = "�����e"
			GoTo FoundWeaponType
		End If
		
SkipShootingWeapon: 
		
		'�Ή����镐��͌�����Ȃ�����
		WeaponInHand = ""
		GoTo SkipWeaponAnimation
		
FoundWeaponType: 
		
		'�\���Ă��镐����L�^
		WeaponInHand = wtype
		
		'�\�����鏀���A�j���̎��
		aname = wtype & "����"
		
		'�F
		If InStr(wtype, "�r�[���T�[�x��") > 0 Or InStr(wtype, "�r�[���J�b�^�[") > 0 Or wtype = "�r�[���i�C�t" Or wtype = "���C�g�Z�C�o�[" Then
			If InStr(wname, "�r�[��") > 0 Then
				aname = aname & " �s���N"
			ElseIf InStr(wname, "�v���Y�}") > 0 Then 
				aname = aname & " �O���[��"
			ElseIf InStr(wname, "���[�U�[") > 0 Then 
				aname = aname & " �u���["
			ElseIf InStr(wname, "���C�g") > 0 Then 
				aname = aname & " �C�G���["
			End If
		End If
		
		'���ʉ�
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'�񓁗�
		If double_weapon Then
			aname = aname & " �񓁗�"
		End If
		
		'�����A�j���\��
		ShowAnimation(aname)
		
SkipWeaponAnimation: 
		
		'����̏����A�j�����X�L�b�v����ꍇ�͂�������
		
		If with_face_up Then
			'�t�F�C�X�A�b�v��\������
			aname = "�t�F�C�X�A�b�v����"
			
			'�Ռ���\���H
			If InStrNotNest(wclass, "�T") > 0 Then
				aname = aname & " �Ռ�"
			End If
			
			'�t�F�C�X�A�b�v�A�j���\��
			ShowAnimation(aname)
		End If
	End Sub
	
	'����̖��̂��畐��̎�ނ𔻒�
	Private Function CheckWeaponType(ByRef wname As String, ByRef wclass As String) As String
		If InStr(wname, "�r�[��") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "���[�U�[") > 0 Or InStr(wname, "�u���X�^�[") > 0 Or InStr(wname, "���C�g") > 0 Then
			If InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�Z�C�o�[") > 0 Or InStr(wname, "�u���[�h") > 0 Or InStr(wname, "�\�[�h") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
				If InStr(wname, "�n�C�p�[") > 0 Or InStr(wname, "�����O") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
					CheckWeaponType = "�n�C�p�[�r�[���T�[�x��"
				ElseIf InStr(wname, "�Z�C�o�[") > 0 Then 
					CheckWeaponType = "���C�g�Z�C�o�["
				Else
					CheckWeaponType = "�r�[���T�[�x��"
				End If
				Exit Function
			End If
			
			If InStr(wname, "�J�b�^�[") > 0 Then
				If InStr(wname, "�n�C�p�[") > 0 Or InStr(wname, "�����O") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
					CheckWeaponType = "�G�i�W�[�u���[�h"
				Else
					CheckWeaponType = "�G�i�W�[�J�b�^�["
				End If
				Exit Function
			End If
			
			If InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Then
				CheckWeaponType = "�r�[���i�C�t"
				Exit Function
			End If
		End If
		
		If InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�Z��") > 0 Or InStr(wname, "����") > 0 Then
			If InStr(wname, "��") > 0 Or InStr(wname, "���") > 0 Or Right(wname, 3) = "�X���[" Or Right(wname, 3) = "�X���E" Or InStrNotNest(wclass, "��") > 0 Then
				CheckWeaponType = "�����i�C�t"
			Else
				CheckWeaponType = "�i�C�t"
			End If
			Exit Function
		End If
		
		If InStr(wname, "�V���[�g�\�[�h") > 0 Or InStr(wname, "�Z��") > 0 Or InStr(wname, "�X���[���\�[�h") > 0 Or InStr(wname, "����") > 0 Then
			CheckWeaponType = "�V���[�g�\�[�h"
			Exit Function
		End If
		
		If InStr(wname, "�O���[�g�\�[�h") > 0 Or InStr(wname, "�匕") > 0 Or InStr(wname, "�n���f�b�h�\�[�h") > 0 Or InStr(wname, "���茕") > 0 Then
			CheckWeaponType = "�匕"
			Exit Function
		End If
		
		If InStr(wname, "�����O�\�[�h") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�o�X�^�[�h�\�[�h") > 0 Or wname = "�\�[�h" Then
			CheckWeaponType = "�\�[�h"
			Exit Function
		End If
		
		If InStr(wname, "�藠��") > 0 Then
			CheckWeaponType = "�藠��"
			Exit Function
		End If
		
		If Right(wname, 1) = "��" And (Len(wname) <= 3 Or Right(wname, 2) = "�̌�") Then
			If InStr(wname, "�u���b�N") > 0 Or InStr(wname, "��") > 0 Then
				CheckWeaponType = "����"
			Else
				CheckWeaponType = "��"
			End If
			Exit Function
		End If
		
		If InStr(wname, "�\�[�h�u���C�J�[") > 0 Then
			CheckWeaponType = "�\�[�h�u���C�J�["
			Exit Function
		End If
		
		If InStr(wname, "���C�s�A") > 0 Then
			CheckWeaponType = "���C�s�A"
			Exit Function
		End If
		
		If InStr(wname, "�V�~�^�[") > 0 Or InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�J�b�g���X") > 0 Or InStr(wname, "�O������") > 0 Then
			CheckWeaponType = "�V�~�^�["
			Exit Function
		End If
		
		If InStr(wname, "�V���[�e��") > 0 Then
			CheckWeaponType = "�V���[�e��"
			Exit Function
		End If
		
		If InStr(wname, "�i�M�i�^") > 0 Or InStr(wname, "�㓁") > 0 Or InStr(wname, "�O���C�u") > 0 Then
			CheckWeaponType = "�i�M�i�^"
			Exit Function
		End If
		
		If InStr(wname, "�|��") > 0 Then
			CheckWeaponType = "�|��"
			Exit Function
		End If
		
		If InStr(wname, "�e��") > 0 Or InStr(wname, "������") > 0 Then
			CheckWeaponType = "�e��"
			Exit Function
		End If
		
		If wname = "��" Or wname = "���{��" Or InStr(wname, "����") > 0 Then
			CheckWeaponType = "���{��"
			Exit Function
		End If
		
		If InStr(wname, "�E�ғ�") > 0 Then
			CheckWeaponType = "�E�ғ�"
			Exit Function
		End If
		
		If InStr(wname, "�\��") > 0 Then
			CheckWeaponType = "�\��"
			Exit Function
		End If
		
		If InStr(wname, "����") > 0 Then
			CheckWeaponType = "����"
			Exit Function
		End If
		
		If InStr(wname, "�g�}�z�[�N") > 0 Then
			CheckWeaponType = "�g�}�z�[�N"
			Exit Function
		End If
		
		If InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "��") > 0 Then
			If InStr(wname, "�O���[�g") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�o�g��") > 0 Then
				CheckWeaponType = "���n��"
			Else
				CheckWeaponType = "�Аn��"
			End If
			Exit Function
		End If
		
		If InStr(wname, "�T�C�Y") > 0 Or InStr(wname, "�劙") > 0 Then
			CheckWeaponType = "�劙"
			Exit Function
		End If
		
		If InStr(wname, "��") > 0 Then
			CheckWeaponType = "��"
			Exit Function
		End If
		
		If InStr(wname, "�X�^�b�t") > 0 Or InStr(wname, "��") > 0 Then
			CheckWeaponType = "��"
			Exit Function
		End If
		
		If InStr(wname, "���_") > 0 Then
			CheckWeaponType = "���_"
			Exit Function
		End If
		
		If InStr(wname, "�x�_") > 0 Then
			CheckWeaponType = "�x�_"
			Exit Function
		End If
		
		If wname = "�_" Then
			CheckWeaponType = "�_"
			Exit Function
		End If
		
		If InStr(wname, "�S�p�C�v") > 0 Then
			CheckWeaponType = "�S�p�C�v"
			Exit Function
		End If
		
		If InStr(wname, "�X�^�����b�h") > 0 Then
			CheckWeaponType = "�X�^�����b�h"
			Exit Function
		End If
		
		If InStr(wname, "�X�p�i") > 0 Then
			CheckWeaponType = "�X�p�i"
			Exit Function
		End If
		
		If InStr(wname, "���C�X") > 0 Then
			CheckWeaponType = "���C�X"
			Exit Function
		End If
		
		
		If InStr(wname, "�p���`") > 0 Or InStr(wname, "�i�b�N��") > 0 Then
			'�n���}�[�p���`�����n���}�[�ɂЂ�������ƍ��邽�߁A�����Ŕ���
			If InStrNotNest(wclass, "��") > 0 Then
				CheckWeaponType = "���P�b�g�p���`"
			End If
			Exit Function
		End If
		
		
		If InStr(wname, "�E�H�[�n���}�[") > 0 Then
			CheckWeaponType = "�E�H�[�n���}�["
			Exit Function
		End If
		
		If InStr(wname, "�ؒ�") > 0 Then
			CheckWeaponType = "�ؒ�"
			Exit Function
		End If
		
		If InStr(wname, "�s�R�s�R�n���}�[") > 0 Then
			CheckWeaponType = "�s�R�s�R�n���}�["
			Exit Function
		End If
		
		If InStr(wname, "�n���}�[") > 0 Then
			If InStrNotNest(wclass, "��") > 0 Then
				CheckWeaponType = "���S��"
			Else
				CheckWeaponType = "�n���}�["
			End If
			Exit Function
		End If
		
		If InStr(wname, "��") > 0 Then
			CheckWeaponType = "�n���}�["
			Exit Function
		End If
		
		If Right(wname, 3) = "���[��" Then
			CheckWeaponType = "���[��"
			Exit Function
		End If
		
		If Right(wname, 2) = "���`" Or InStr(wname, "��") > 0 Or InStr(wname, "�E�B�b�v") > 0 Then
			CheckWeaponType = "��"
			Exit Function
		End If
		
		If wname = "�T�C" Then
			CheckWeaponType = "�T�C"
			Exit Function
		End If
		
		If InStr(wname, "�g���t�@�[") > 0 Then
			CheckWeaponType = "�g���t�@�["
			Exit Function
		End If
		
		If InStr(wname, "�S�̒�") > 0 Then
			CheckWeaponType = "�N���["
			Exit Function
		End If
		
		If InStr(wname, "�n���o�[�h") > 0 Then
			CheckWeaponType = "�n���o�[�h"
			Exit Function
		End If
		
		
		If InStr(wname, "���[�j���O�X�^�[") > 0 Then
			CheckWeaponType = "���[�j���O�X�^�["
			Exit Function
		End If
		
		If InStr(wname, "�t���C��") > 0 Then
			CheckWeaponType = "�t���C��"
			Exit Function
		End If
		
		If InStr(wname, "���S��") > 0 Then
			CheckWeaponType = "���S��"
			Exit Function
		End If
		
		If InStr(wname, "����") > 0 Then
			CheckWeaponType = "����"
			Exit Function
		End If
		
		If InStr(wname, "�k���`���N") > 0 Then
			CheckWeaponType = "�k���`���N"
			Exit Function
		End If
		
		If InStr(wname, "�O�ߞ�") > 0 Then
			CheckWeaponType = "�O�ߞ�"
			Exit Function
		End If
		
		If InStr(wname, "�`�F�[��") > 0 Then
			CheckWeaponType = "�`�F�[��"
			Exit Function
		End If
		
		
		If InStr(wname, "�u�[������") > 0 Then
			CheckWeaponType = "�u�[������"
			Exit Function
		End If
		
		If InStr(wname, "�`���N����") > 0 Then
			CheckWeaponType = "�`���N����"
			Exit Function
		End If
		
		If InStr(wname, "�\�[�T�[") > 0 Then
			CheckWeaponType = "�\�[�T�["
			Exit Function
		End If
		
		If InStr(wname, "�N�i�C") > 0 Then
			CheckWeaponType = "�N�i�C"
			Exit Function
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�I") > 0 Then
			CheckWeaponType = "��"
			Exit Function
		End If
		
		If InStr(wname, "��") > 0 Then
			CheckWeaponType = "��"
			Exit Function
		End If
		
		If InStr(wname, "�S��") > 0 Then
			CheckWeaponType = "�S��"
			Exit Function
		End If
		
		If InStr(wname, "��֒e") > 0 Then
			CheckWeaponType = "��֒e"
			Exit Function
		End If
		
		If InStr(wname, "�|�e�g�X�}�b�V���[") > 0 Then
			CheckWeaponType = "�|�e�g�X�}�b�V���["
			Exit Function
		End If
		
		If InStr(wname, "�_�C�i�}�C�g") > 0 Then
			CheckWeaponType = "�_�C�i�}�C�g"
			Exit Function
		End If
		
		If InStr(wname, "���e") > 0 Then
			If InStr(wname, "����") > 0 Then
				CheckWeaponType = "���e"
				Exit Function
			End If
		End If
		
		If InStr(wname, "�Ή��r") > 0 Then
			CheckWeaponType = "�Ή��r"
			Exit Function
		End If
		
		If InStr(wname, "�l�b�g") > 0 Or InStr(wname, "��") > 0 Then
			CheckWeaponType = "�l�b�g"
			Exit Function
		End If
		
		If InStr(wname, "���") > 0 Then
			CheckWeaponType = "�l�b�g"
			Exit Function
		End If
		
		If Right(wname, 2) = "�R�}" Then
			CheckWeaponType = "�R�}"
			Exit Function
		End If
		
		If InStr(wname, "�D") > 0 Then
			CheckWeaponType = "���D"
			Exit Function
		End If
		
		
		If InStr(wname, "���{��") > 0 Then
			CheckWeaponType = "���{��"
			Exit Function
		End If
		
		If InStr(wname, "�t�[�v") > 0 Then
			CheckWeaponType = "�t�[�v"
			Exit Function
		End If
		
		
		If InStr(wname, "�J�^���O") > 0 Then
			CheckWeaponType = "�J�^���O"
			Exit Function
		End If
		
		If InStr(wname, "�t���C�p��") > 0 Then
			CheckWeaponType = "�t���C�p��"
			Exit Function
		End If
		
		If InStr(wname, "�g���{") > 0 Then
			CheckWeaponType = "�g���{"
			Exit Function
		End If
		
		If InStr(wname, "���b�v") > 0 Then
			CheckWeaponType = "���b�v"
			Exit Function
		End If
		
		If InStr(wname, "���P") > 0 Then
			CheckWeaponType = "���P"
			Exit Function
		End If
		
		If InStr(wname, "�����o�b�g") > 0 Then
			CheckWeaponType = "�����o�b�g"
			Exit Function
		End If
		
		If InStr(wname, "�B�o�b�g") > 0 Then
			CheckWeaponType = "�B�o�b�g"
			Exit Function
		End If
		
		If Right(wname, 3) = "�o�b�g" Then
			If InStr(wname, "�w�b�h�o�b�g") = 0 Then
				CheckWeaponType = "�o�b�g"
				Exit Function
			End If
		End If
		
		If InStr(wname, "��q") > 0 Then
			CheckWeaponType = "��q"
			Exit Function
		End If
		
		If InStr(wname, "�M�^�[") > 0 Then
			CheckWeaponType = "�M�^�["
			Exit Function
		End If
		
		If InStr(wname, "�n���Z��") > 0 Then
			CheckWeaponType = "�n���Z��"
			Exit Function
		End If
		
		If wname = "�S���t�h���C�o�[" Then
			CheckWeaponType = "�S���t�h���C�o�["
			Exit Function
		End If
		
		
		If InStr(wname, "�g���C�f���g") > 0 Or InStr(wname, "�O����") > 0 Or InStr(wname, "�W���x����") > 0 Then
			CheckWeaponType = "�g���C�f���g"
			Exit Function
		End If
		
		If InStr(wname, "�X�s�A") > 0 Then
			CheckWeaponType = "�X�s�A"
			Exit Function
		End If
		
		If InStr(wname, "��") > 0 Then
			CheckWeaponType = "�a��"
			Exit Function
		End If
		
		If InStr(wname, "�����X") > 0 Or InStr(wname, "�����T�[") > 0 Then
			CheckWeaponType = "�����X"
			Exit Function
		End If
		
		If InStr(wname, "�p�C�N") > 0 Then
			CheckWeaponType = "�����X"
			Exit Function
		End If
		
		If InStr(wname, "�G�X�g�b�N") > 0 Then
			CheckWeaponType = "�G�X�g�b�N"
			Exit Function
		End If
		
		If wname = "���b�h" Then
			CheckWeaponType = "���b�h"
			Exit Function
		End If
		
		If InStr(wname, "�h����") > 0 Then
			CheckWeaponType = "�h����"
			Exit Function
		End If
	End Function
	
	'���폀�����̌��ʉ�
	Public Sub PrepareWeaponSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		
		'�t���O���N���A
		IsWavePlayed = False
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
			If InStr(wname, "�r�[��") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "���[�U�[") > 0 Or InStr(wname, "�u���X�^�[") > 0 Or InStr(wname, "�����g") > 0 Or InStr(wname, "�d��") > 0 Or wname = "�Z�C�o�[" Or wname = "���C�g�Z�C�o�[" Or wname = "�����T�[" Then
				PlayWave("BeamSaber.wav")
			End If
		End If
		
		'�t���O���N���A
		IsWavePlayed = False
	End Sub
	
	
	'����g�p���̓������
	Public Sub AttackEffect(ByRef u As Unit, ByVal w As Short)
		'�E�N���b�N���͓�����ʂ��X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			AttackAnimation(u, w)
		Else
			AttackSound(u, w)
		End If
	End Sub
	
	'����g�p���̃A�j���[�V����
	Public Sub AttackAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, bmpname, cname0 As String
		Dim sname, sname0 As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim is_handy_weapon As Boolean
		Dim i As Short
		
		'�퓬�A�j���񎩓��I���I�v�V����
		If IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			ShowAnimation("�f�t�H���g�U��")
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'�񓁗��H
		If InStr(wname, "�_�u��") > 0 Or InStr(wname, "�c�C��") > 0 Or InStr(wname, "�f���A��") > 0 Or InStr(wname, "�o") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�Q�A") > 0 Or InStr(wname, "��A") > 0 Or InStr(wname, "�A��") > 0 Then
			double_weapon = True
		End If
		
		'�A���U���H
		If InStr(wname, "�_�u��") > 0 Or InStr(wname, "�c�C��") > 0 Or InStr(wname, "�R���r�l�[�V����") > 0 Or InStr(wname, "�R���{") > 0 Or InStr(wname, "�A") > 0 Or InStrNotNest(wclass, "�A") > 0 Then
			double_attack = True
		End If
		
		'���ŁH
		If InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�S��") > 0 Or (Right(wname, 4) = "���b�V��" And InStr(wname, "�N���b�V��") = 0 And InStr(wname, "�X���b�V��") = 0 And InStr(wname, "�X�v���b�V��") = 0 And InStr(wname, "�t���b�V��") = 0) Then
			combo_attack = True
		End If
		
		'���ꂩ�畐��̎�ނ𔻒�
		
		'�܂��͔�����p����̔���
		If InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "�i") = 0 Then
			GoTo SkipInfightWeapon
		End If
		
		'�������������
		If InStr(wname, "��") > 0 Or InStr(wname, "���") > 0 Or Right(wname, 3) = "�X���[" Or Right(wname, 3) = "�X���E" Or InStrNotNest(wclass, "��") > 0 Then
			GoTo SkipInfightWeapon
		End If
		
		'�ړ��}�b�v�U��
		If InStrNotNest(wclass, "�l��") > 0 Then
			wtype = "�l�`�o�ړ��^�b�N��"
			GoTo FoundWeaponType
		End If
		
		'�ˌ��n(������\���ēːi����)
		
		If InStr(wname, "�ˌ�") > 0 Or InStr(wname, "�ːi") > 0 Or InStr(wname, "�`���[�W") > 0 Then
			Select Case WeaponInHand
				Case ""
					'�Y������
				Case Else
					wtype = WeaponInHand & "�ˌ�"
					GoTo FoundWeaponType
			End Select
		End If
		
		'�Ō��n�̍U��
		
		If InStr(wname, "���@") > 0 Or Right(wname, 2) = "�A�[�c" Or Right(wname, 5) = "�X�g���C�N" Then
			wtype = "�A��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Then
			wtype = "�����A��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�p���`") > 0 Or InStr(wname, "�`���b�v") > 0 Or InStr(wname, "�i�b�N��") > 0 Or InStr(wname, "�u���[") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or Right(wname, 1) = "��" Or Right(wname, 1) = "�r" Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Then 
				wtype = "�A��"
			ElseIf InStrNotNest(wclass, "�i") > 0 Then 
				wtype = "�A�b�p�["
			Else
				wtype = "�œ�"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�i��") > 0 Or InStr(wname, "����") > 0 Then
			wtype = "�i��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�^�b�N��") > 0 Or InStr(wname, "�̓�") > 0 Or InStr(wname, "�`���[�W") > 0 Or InStr(wname, "�Ԃ����܂�") > 0 Or InStr(wname, "���݂�") > 0 Then
			wtype = "�^�b�N��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�L�b�N") > 0 Or InStr(wname, "�R") > 0 Or InStr(wname, "�r") > 0 Or Right(wname, 1) = "��" Then
			wtype = "�L�b�N"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�w�b�h�o�b�g") > 0 Or InStr(wname, "����") > 0 Then
			wtype = "�w�b�h�o�b�g"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�A�b�p�[") > 0 Then
			wtype = "�A�b�p�["
			GoTo FoundWeaponType
		End If
		
		'�U���čU�����镐��
		
		If InStr(wname, "�\�[�h") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�V�~�^�[") > 0 Or InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�J�b�g���X") > 0 Or InStr(wname, "�J�b�^�[") > 0 Or Right(wname, 2) = "���`" Or InStr(wname, "��") > 0 Or InStr(wname, "�E�B�b�v") > 0 Or InStr(wname, "�n���}�[") > 0 Or InStr(wname, "���b�h") > 0 Or InStr(wname, "�N���[") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�Ђ�����") > 0 Or InStr(wname, "�A�[��") > 0 Or Right(wname, 1) = "��" Then
			If combo_attack Then
				wtype = "��������"
			ElseIf double_attack Then 
				wtype = "�����A��"
			ElseIf InStr(wname, "��]") > 0 Then 
				wtype = "������]"
			ElseIf InStrNotNest(wclass, "�i") > 0 Then 
				wtype = "�U��グ"
			Else
				wtype = "��������"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�a") > 0 Or InStr(wname, "�u���[�h") > 0 Or InStr(wname, "�n") > 0 Or InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�J�b�g") > 0 Or InStr(wname, "�J�b�^�[") > 0 Or InStr(wname, "�X���b�V��") > 0 Or InStr(wname, "����") > 0 Then
			If combo_attack Then
				wtype = "��������"
			ElseIf double_attack Then 
				wtype = "�_�u���a��"
			ElseIf InStr(wname, "��]") > 0 Then 
				wtype = "������]"
			ElseIf InStrNotNest(wclass, "�i") > 0 Then 
				wtype = "�U��グ"
			ElseIf InStr(wname, "�u���b�N") > 0 Or InStr(wname, "��") > 0 Then 
				wtype = "���a��"
			Else
				wtype = "�a��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�T�C�Y") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�O���C�u") > 0 Or InStr(wname, "�i�M�i�^") > 0 Then
			wtype = "�U�艺�낵"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���[�e��") > 0 Then
			wtype = "�_�u���a��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�~���E�@") > 0 Then
			wtype = "�~���E�@"
			GoTo FoundWeaponType
		End If
		
		'�傫���U��܂킷����
		
		If InStr(wname, "���S��") > 0 Then
			wtype = "���S��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���[�j���O�X�^�[") > 0 Then
			wtype = "���[�j���O�X�^�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�t���C��") > 0 Then
			wtype = "�t���C��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�`�F�[��") > 0 And InStr(wname, "�`�F�[���\�[") = 0 Then
			wtype = "�`�F�[��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�k���`���N") > 0 Then
			wtype = "�k���`���N"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�O�ߞ�") > 0 Then
			wtype = "�O�ߞ�"
			GoTo FoundWeaponType
		End If
		
		'�˂��h������
		
		If InStr(wname, "�X�s�A") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�����X") > 0 Or InStr(wname, "�����T�[") > 0 Or InStr(wname, "�g���C�f���g") > 0 Or InStr(wname, "�W���x����") > 0 Or InStr(wname, "���C�s�A") > 0 Or wname = "���b�h" Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Then 
				wtype = "�A��"
			Else
				wtype = "�h��"
			End If
			GoTo FoundWeaponType
		End If
		
		'����Ȋi������
		
		If InStr(wname, "�h����") > 0 Then
			wtype = "�h����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�`�F�[���\�[") > 0 Then
			wtype = "�`�F�[���\�["
			GoTo FoundWeaponType
		End If
		
		'�ڍׂ�������Ȃ���������
		If InStrNotNest(wclass, "��") > 0 Then
			'�������Ă���A�C�e�����畐�������
			For i = 1 To u.CountItem
				With u.Item(i)
					If .Activated And (.Part = "����" Or .Part = "�Ў�" Or .Part = "����") Then
						wtype = CheckWeaponType(.Nickname, "")
						If wtype = "" Then
							wtype = CheckWeaponType(.Class0, "")
						End If
						Exit For
					End If
				End With
			Next 
			Select Case wtype
				Case "�X�s�A", "�����X", "�g���C�f���g", "�a��", "�G�X�g�b�N"
					If combo_attack Then
						wtype = "����"
					ElseIf double_attack Then 
						wtype = "�A��"
					Else
						wtype = "�h��"
					End If
				Case Else
					If combo_attack Then
						wtype = "��������"
					ElseIf double_attack Then 
						wtype = "�����A��"
					ElseIf InStr(wname, "��]") > 0 Then 
						wtype = "������]"
					ElseIf InStrNotNest(wclass, "�i") > 0 Then 
						wtype = "�U��グ"
					Else
						wtype = "��������"
					End If
			End Select
			GoTo FoundWeaponType
		End If
		
		'�ڍׂ�������Ȃ������ߐڋZ
		If InStrNotNest(wclass, "��") > 0 And InStrNotNest(wclass, "��") > 0 Then
			wtype = "�i��"
			GoTo FoundWeaponType
		End If
		
SkipInfightWeapon: 
		
		If InStrNotNest(wclass, "��") = 0 Then
			GoTo SkipThrowingWeapon
		End If
		
		'��������
		'(�^��������ԕ���)
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�X�s�A") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�N�i�C") > 0 Or InStr(wname, "�ꖳ") > 0 Then
			wtype = "�����i�C�t"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�I") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�S��") > 0 Then
			wtype = "�S��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�_�C�i�}�C�g") > 0 Then
			wtype = "�_�C�i�}�C�g"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���e") > 0 Then
			If InStr(wname, "����") > 0 Then
				wtype = "���e"
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "�n���h�O���l�[�h") > 0 Then
			wtype = "�O���l�[�h����"
			GoTo FoundWeaponType
		End If
		
		'(��]���Ȃ����ԕ���)
		
		If InStr(wname, "�g�}�z�[�N") > 0 Then
			wtype = "�g�}�z�[�N����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "��") > 0 Then
			If InStr(wname, "�O���[�g") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�o�g��") > 0 Then
				wtype = "���n������"
			Else
				wtype = "�Аn������"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�T�C�Y") > 0 Or InStr(wname, "�劙") > 0 Then
			wtype = "�劙����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�u�[������") > 0 Then
			wtype = "�u�[������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�`���N����") > 0 Then
			wtype = "�`���N����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�藠��") > 0 Then
			wtype = "�藠��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��֒e") > 0 Then
			wtype = "��֒e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�|�e�g�}�b�V���[") > 0 Then
			wtype = "�|�e�g�}�b�V���["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ή��r") > 0 Then
			wtype = "�Ή��r"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���") > 0 Then
			wtype = "���"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�t�[�v") > 0 Then
			wtype = "�t�[�v"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��q") > 0 Then
			wtype = "��q"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�D") > 0 Then
			wtype = "���D"
			GoTo FoundWeaponType
		End If
		
		'�|��
		
		If InStr(wname, "�|") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�����O�{�E") > 0 Then
			wtype = "�|��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�A���[") > 0 Then
			If CountAttack0(u, w) > 1 Then
				wtype = "��A��"
			Else
				wtype = "��"
			End If
			GoTo FoundWeaponType
		End If
		
		'�������n�̊i������
		
		'�U�镐��
		
		If Right(wname, 2) = "���`" Or InStr(wname, "��") > 0 Or InStr(wname, "�E�B�b�v") > 0 Then
			wtype = "��������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Then
			wtype = "�����A��"
			GoTo FoundWeaponType
		End If
		
		'�傫���U��܂킷����
		
		If InStr(wname, "���S��") > 0 Or InStr(wname, "�n���}�[") > 0 Then
			wtype = "���S��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�`�F�[��") > 0 Then
			wtype = "�`�F�[��"
			GoTo FoundWeaponType
		End If
		
		'���̑��i���n
		
		If InStr(wname, "�p���`") > 0 Or InStr(wname, "�i�b�N��") > 0 Then
			wtype = "���P�b�g�p���`"
			GoTo FoundWeaponType
		End If
		
SkipThrowingWeapon: 
		
		'������ʏ�ˌ��U��
		
		'�܂��͎莝������̔���
		is_handy_weapon = True
		
		'�����n�̍U�����ǂ����𔻒肷��
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "�r�[��"
			
			'���e�n���픻����X�L�b�v
			GoTo SkipNormalHandWeapon
		End If
		
		'��Ɏ��ˌ�����
		
		'(�傫�ڂ̎��e���΂��^�C�v)
		
		If InStr(wname, "�N���X�{�E") > 0 Or InStr(wname, "�{�E�K��") > 0 Then
			wtype = "�N���X�{�E"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�o�Y�[�J") > 0 Then
			wtype = "�o�Y�[�J"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�ΐ�ԃ��C�t��") > 0 Then
			wtype = "�ΐ�ԃ��C�t��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ε����C�t��") > 0 Then
			wtype = "�Ε����C�t��"
			GoTo FoundWeaponType
		End If
		
		'(�����Ȓe��P���Ō��^�C�v�̎莝���Ί�)
		
		If InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "���e") > 0 Then
			wtype = "�s�X�g��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���{���o�[") > 0 Or InStr(wname, "���{�����@�[") > 0 Then
			wtype = "���{���o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���C�t��") > 0 Or (Right(wname, 1) = "�e" And Right(wname, 2) <> "�@�e") Then
			wtype = "���C�t��"
			GoTo FoundWeaponType
		End If
		
		'(�A�˂���^�C�v�̎莝���Ί�)
		
		If InStr(wname, "�T�u�}�V���K��") > 0 Then
			wtype = "�T�u�}�V���K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�@�֏e") > 0 Then
			If InStr(wname, "�w�r�[") > 0 Or InStr(wname, "�d") > 0 Then
				wtype = "�w�r�[�}�V���K��"
			Else
				wtype = "�}�V���K��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�K�g�����O") > 0 Then
			wtype = "�K�g�����O"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���b�g�K��") > 0 Or InStr(wname, "���C�A�b�g�K��") > 0 Then
			wtype = "�V���b�g�K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���[���K��") > 0 Or InStr(wname, "���j�A�K��") > 0 Then
			PlayWave("Thunder.wav")
			Sleep(300)
			wtype = "�L���m���C"
			GoTo FoundWeaponType
		End If
		
		'�悭������Ȃ��̂Ń��C�t������
		If Right(wname, 2) = "�K��" Then
			wtype = "���C�t��"
			GoTo FoundWeaponType
		End If
		
		GoTo SkipHandWeapon
		
SkipNormalHandWeapon: 
		
		'(�莝���̃r�[���U��)
		
		If InStr(wname, "���C�t��") > 0 Or InStr(wname, "�K��") > 0 Or InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Or (Right(wname, 1) = "�e" And Right(wname, 2) <> "�@�e") Then
			If InStrNotNest(wclass, "�l") > 0 Then
				wtype = "�l�`�o�o�X�^�[�r�[�����C�t��"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�n�C���K") > 0 Or InStr(wname, "�o�X�^�[") > 0 Or InStr(wname, "��") > 0 Or Left(wname, 2) = "�M�K" Then
				wtype = "�o�X�^�[�r�[�����C�t��"
			ElseIf InStr(wname, "���K") > 0 Or InStr(wname, "�n�C") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Then 
				If double_weapon Then
					wtype = "�_�u���r�[�������`���["
				Else
					wtype = "�r�[�������`���["
				End If
				If InStr(wname, "���C�t��") > 0 Then
					bmpname = "Weapon\EFFECT_BusterRifle01.bmp"
				End If
			ElseIf CountAttack0(u, w) >= 4 Then 
				wtype = "���[�U�[�}�V���K��"
				bmpname = "Weapon\EFFECT_Rifle01.bmp"
			ElseIf InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "�~�j") > 0 Or InStr(wname, "��") > 0 Then 
				wtype = "���[�U�[�K��"
			Else
				If double_weapon Then
					wtype = "�_�u���r�[�����C�t��"
				Else
					wtype = "�r�[�����C�t��"
				End If
			End If
			
			If wtype = "�o�X�^�[" Then
				wtype0 = "���q�W��"
			End If
			
			GoTo FoundWeaponType
		End If
		
SkipHandWeapon: 
		
		'�����^�ˌ�����
		is_handy_weapon = False
		
		'(��^�̎��e�Ί�)
		
		If InStr(wname, "�~�T�C��") > 0 Or InStr(wname, "���P�b�g") > 0 Then
			wtype = "�~�T�C��"
			
			If InStr(wname, "�h����") > 0 Then
				wtype = "�h�����~�T�C��"
				GoTo FoundWeaponType
			End If
			
			attack_times = CountAttack0(u, w)
			
			If InStr(wname, "��^") > 0 Or InStr(wname, "�r�b�O") > 0 Or InStr(wname, "�Ί�") > 0 Then
				wtype = "�X�[�p�[�~�T�C��"
				attack_times = 1
			ElseIf InStr(wname, "���^") > 0 Then 
				wtype = "���^�~�T�C��"
			ElseIf InStr(wname, "�����`���[") > 0 Or InStr(wname, "�|�b�h") > 0 Or InStr(wname, "�}�C�N��") > 0 Or InStr(wname, "�X�v���[") > 0 Then 
				wtype = "���^�~�T�C��"
				attack_times = 6
			End If
			
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�O���l�[�h") > 0 Or InStr(wname, "�f�B�X�`���[�W���[") > 0 Then
			wtype = "�O���l�[�h"
			
			attack_times = CountAttack0(u, w)
			
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���c�����t�@�E�X�g") > 0 Then
			wtype = "���e����"
			
			bmpname = "Bullet\EFFECT_BazookaBullet01.bmp"
			attack_times = CountAttack0(u, w)
			
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���e") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Then
			If u.Weapon(w).MaxRange = 1 Then
				wtype = "�������e"
			Else
				wtype = "�O���l�[�h"
				attack_times = CountAttack0(u, w)
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���q����") > 0 Then
			wtype = "���q����"
			GoTo FoundWeaponType
		End If
		
		'(�������n)
		
		If InStr(wname, "������") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�j�����") > 0 Then
			wtype = "�j�����"
			GoTo FoundWeaponType
		End If
		
		'����ȕ������o������
		
		If InStr(wname, "����") > 0 Then
			wtype = "���Ί�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Then
			wtype = "�����e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���S�C") > 0 Or Right(wname, 1) = "�t" Then
			wtype = "���e����"
			sname = "Bow.wav"
			If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
				bmpname = "Bullet\EFFECT_Venom01.bmp"
			Else
				bmpname = "Bullet\EFFECT_WaterShot01.bmp"
			End If
			GoTo FoundWeaponType
		End If
		
		'�������یn�̍U��(������Ȃ�)
		
		If InStr(wname, "�d��") > 0 Or InStr(wname, "�O���r") > 0 Or InStr(wname, "�u���b�N�z�[��") > 0 Or InStr(wname, "�k��") > 0 Then
			wtype = "�d�͒e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or Right(wname, 2) = "���" Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "���C�g�j���O") > 0 Or InStr(wname, "�T���_�[") > 0 Then
			If InStrNotNest(wclass, "��") = 0 Then
				If u.Weapon(w).MaxRange = 1 Then
					wtype = "�j�����"
					sname = "Thunder.wav"
				Else
					wtype = "����"
				End If
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "�d��") > 0 Or InStr(wname, "�d��") > 0 Or InStr(wname, "�G���N�g") > 0 Then
			wtype = "�j�����"
			sname = "Thunder.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�G�l���M�[�e") > 0 Then
			wtype = "���d"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�A") > 0 Or InStr(wname, "�o�u��") > 0 Then
			wtype = "�A"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���g") > 0 Or InStr(wname, "�T�E���h") > 0 Or InStr(wname, "�\�j�b�N") > 0 Or (InStrNotNest(wclass, "��") > 0 And InStr(wname, "�V���b�N") > 0) Or InStr(wname, "�E�F�[�u") > 0 Or InStr(wname, "����") > 0 Or (InStrNotNest(wclass, "��") > 0 And InStr(wname, "���K") > 0) Then
			wtype = "���g"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�\���O") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�j") > 0 Or InStr(wname, "�j�[�h��") > 0 Then
			wtype = "�j�[�h��"
			If CountAttack0(u, w) > 1 Then
				wtype = "�j�[�h���A��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ôg") > 0 Or InStr(wname, "�_�C�_��") > 0 Then
			wtype = "�Ôg"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�R���b�g") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���e�I") > 0 Or InStr(wname, "覐�") > 0 Then
			wtype = "覐�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "�Q��") > 0 Or InStr(wname, "�g���l�[�h") > 0 Or InStr(wname, "�T�C�N����") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���") > 0 Then
			wtype = "�X�e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ԃ�") > 0 Then
			wtype = "��e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "�u���U�[�h") > 0 Or InStr(wname, "�A�C�X�X�g�[��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�g�[��") > 0 Or InStr(wname, "�n���P�[��") > 0 Or InStr(wname, "�^�C�t�[��") > 0 Or InStr(wname, "�䕗") > 0 Or InStr(wname, "��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�E�B���h") > 0 Or InStr(wname, "�E�C���h") > 0 Or InStr(wname, "��") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�X���[�N") > 0 Or Right(wname, 2) = "�K�X" Or Right(wname, 1) = "��" Or InStr(wname, "�E�q") > 0 Then
			wtype = "��"
			If InStr(wname, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
				cname = "��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ή��e") > 0 Then
			wtype = "�Ή��e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ή�����") > 0 Or Right(wname, 2) = "�Ή�" Then
			wtype = "�Ή�����"
			sname = "AntiShipMissile.wav"
			GoTo FoundWeaponType
		End If
		
		If Right(wname, 5) = "�t�@�C�A�[" Or Right(wname, 5) = "�t�@�C���[" Or Right(wname, 4) = "�t�@�C�A" Or Right(wname, 4) = "�t�@�C��" Then
			If InStrNotNest(wclass, "��") = 0 And Left(wname, 2) <> "�t��" Then
				If InStrNotNest(wclass, "�p") > 0 Then
					wtype = "������"
				Else
					wtype = "�Ή�����"
					sname = "AntiShipMissile.wav"
				End If
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "��") > 0 Or Right(wname, 3) = "�u���X" Then
			If InStrNotNest(wclass, "��") = 0 Then
				wtype = "�Ή�����"
				sname = "Breath.wav"
				
				Select Case SpellColor(wname, wclass)
					Case "��", "��", "��", "��", "��", "��"
						cname = SpellColor(wname, wclass)
				End Select
				
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "�G�l���M�[�g") > 0 Then
			wtype = "�g������"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ռ�") > 0 Then
			wtype = "�g������"
			cname = "��"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		'��I�A���@�I�ȍU��
		
		If InStr(wname, "�C�e") > 0 Then
			wtype = "�C�e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�\�j�b�N�u���[�h") > 0 Then
			wtype = "�C�a"
			GoTo FoundWeaponType
		End If
		
		If u.IsSpellWeapon(w) Or InStrNotNest(wclass, "��") > 0 Then
			'        wtype = "���@����"
			'        cname = SpellColor(wname, wclass)
			wtype = "�f�t�H���g"
			sname = "Whiz.wav"
			GoTo FoundWeaponType
		End If
		
		'(�r�[���U��)
		
		If wtype = "�r�[��" Then
			If InStrNotNest(wclass, "�l") > 0 Then
				wtype = "�l�`�o�r�[��"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�n�C���K") > 0 Or InStr(wname, "�o�X�^�[") > 0 Or InStr(wname, "��") > 0 Or Left(wname, 2) = "�M�K" Then
				wtype = "��r�[��"
			ElseIf InStr(wname, "���K") > 0 Or InStr(wname, "�n�C") > 0 Then 
				wtype = "���r�[��"
			ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "�΋�") > 0 Then 
				wtype = "�j�[�h�����[�U�[�A��"
			ElseIf InStr(wname, "�~�j") > 0 Or InStr(wname, "��") > 0 Then 
				wtype = "�j�[�h�����[�U�["
			ElseIf InStr(wname, "�����`���[") > 0 Or InStr(wname, "�L���m��") > 0 Or InStr(wname, "�J�m��") > 0 Or InStr(wname, "�C") > 0 Then 
				wtype = "���r�[��"
			Else
				wtype = "���r�[��"
			End If
			
			If wtype = "��r�[��" Then
				wtype0 = "���q�W��"
			End If
			
			Select Case wtype
				Case "���r�[��", "���r�[��"
					If double_weapon Then
						wtype = "�Q�A" & wtype
					End If
			End Select
			
			If InStr(wname, "�g�U") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�z�[�~���O") > 0 Or InStr(wname, "�U��") > 0 Then
				wtype = "�g�U�r�[��"
			End If
			
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		'(���^�ŘA�˂���Ί�)
		
		If InStr(wname, "�o���J��") > 0 Then
			wtype = "�o���J��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�@�e") > 0 Or InStr(wname, "�@�֖C") > 0 Then
			wtype = "�@�֖C"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�`�F�[���K��") > 0 Or InStr(wname, "�K�������`���[") > 0 Then
			wtype = "�����K�g�����O"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�}�V���L���m��") > 0 Or InStr(wname, "�I�[�g�L���m��") > 0 Or InStr(wname, "���˖C") > 0 Then
			wtype = "�d�@�֖C"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�x�A�����O") > 0 Or InStr(wname, "�N���C���A") > 0 Then
			wtype = "�x�A�����O"
			GoTo FoundWeaponType
		End If
		
		'(�I�[�������W�U��)
		
		If InStr(wname, "�L��") > 0 Then
			wtype = "�Q�v�`�x�ˏo"
			GoTo FoundWeaponType
		End If
		
		'�ėp�I�ȁu�C�v�̎w��͍Ō�ɔ���
		If InStr(wname, "�C") > 0 Or InStr(wname, "�L���m��") > 0 Or InStr(wname, "�J�m��") > 0 Or InStr(wname, "�e") > 0 Then
			If InStr(wname, "���j�A") > 0 Or InStr(wname, "���[��") > 0 Or InStr(wname, "�d��") > 0 Then
				PlayWave("Thunder.wav")
				Sleep(300)
			End If
			
			wtype = "�L���m���C"
			
			attack_times = CountAttack0(u, w)
			
			GoTo FoundWeaponType
		End If
		
SkipShootingWeapon: 
		
		'�Ή����镐��͌�����Ȃ�����
		wtype = "�f�t�H���g"
		
FoundWeaponType: 
		
		'�󒆈ړ���p�`�Ԃ͕������ō\���Ȃ��B
		'�܂����g���̏ꍇ�A��l�ԃ��j�b�g�̓��J�ł��邱�Ƃ������̂ł������
		'���������D�悷��B
		If is_handy_weapon And (u.Data.Transportation = "��" Or (IsOptionDefined("���g��") And Not u.IsHero())) Then
			Select Case wtype
				Case "�l�`�o�o�X�^�[�r�[�����C�t��"
					wtype = "�l�`�o�r�[��"
				Case "�o�X�^�[�r�[�����C�t��"
					wtype = "��r�[��"
				Case "�_�u���r�[�������`���["
					wtype = "�Q�A���r�[��"
				Case "�r�[�������`���["
					wtype = "���r�[��"
				Case "�_�u���r�[�����C�t��"
					wtype = "�Q�A���r�[��"
				Case "�r�[�����C�t��"
					wtype = "���r�[��"
				Case "���[�U�[�}�V���K��"
					wtype = "�j�[�h�����[�U�[�A��"
				Case "���[�U�[�K��"
					wtype = "�j�[�h�����[�U�["
				Case "�T�u�}�V���K��", "�}�V���K��"
					wtype = "�@�֖C"
				Case "�w�r�[�}�V���K��"
					wtype = "�d�@�֖C"
				Case "�K�g�����O"
					wtype = "�����K�g�����O"
				Case "�V���b�g�K��"
					wtype = "�x�A�����O"
				Case Else
					'�莝������̉摜����ɂ���
					bmpname = "-.bmp"
			End Select
		End If
		
		'�}�b�v�U���H
		If InStrNotNest(wclass, "�l") > 0 Then
			'�}�b�v�U���Ή��A�j���ɒu������
			Select Case wtype
				Case "��", "���^�~�T�C��", "�~�T�C��", "�X�[�p�[�~�T�C��", "�O���l�[�h", "�L���m���C", "��L���m���C", "�h�b�a�l", "�V���[�g�J�b�^�[", "���d", "�X�e", "�Ή��e", "��e", "����", "����", "���d", "�X��", "���", "����", "����", "��", "����", "����", "�Ôg", "�A", "����", "�I�[�������W", "��", "�C�e", "�A�C�e", "�C�a", "�g������"
					wtype = "�l�`�o" & wtype
				Case "��", "������", "�Ή�����"
					wtype = "�l�`�o��"
				Case "�j�[�h��", "�j�[�h���A��"
					wtype = "�l�`�o�j�[�h��"
				Case "�������e"
					wtype = "�l�`�o����"
				Case "�d�͒e"
					wtype = "�l�`�o�u���b�N�z�[��"
				Case Else
					If InStr(wname, "�t���b�V��") > 0 Or InStr(wname, "�M��") > 0 Then
						wtype = "�l�`�o�t���b�V��"
					ElseIf InStr(wname, "�_�[�N") > 0 Or InStr(wname, "��") > 0 Then 
						wtype = "�l�`�o�_�[�N�l�X"
					ElseIf InStr(wname, "�n�k") > 0 Or InStr(wname, "�N�E�F�C�N") > 0 Or InStr(wname, "�N�G�C�N") > 0 Then 
						wtype = "�l�`�o�n�k"
						sname = " Explode(Far).wav"
					ElseIf InStr(wname, "�j") > 0 Or InStr(wname, "�A�g�~�b�N") > 0 Then 
						wtype = "�l�`�o�j����"
					End If
			End Select
		End If
		
		
		'�g�p�����U����i���L�^
		CurrentWeaponType = wtype
		
		'�`��F���ŏI����
		If InStr(wname, "���b�h") > 0 Or InStr(wname, "��") > 0 Then
			cname = "��"
		ElseIf InStr(wname, "�u���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�C�G���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�O���[��") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�s���N") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�u���E��") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�u���b�N") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�_�[�N") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�z���C�g") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�z�[���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		End If
		
		'�Q��ނ̃A�j����g�ݍ��킹��ꍇ
		If Len(wtype0) > 0 Then
			'�\�����鏀���A�j���̎��
			aname = wtype0 & "����"
			
			'�F
			If Len(cname0) > 0 Then
				aname = aname & " " & cname0
			ElseIf Len(cname) > 0 Then 
				aname = aname & " " & cname
			End If
			
			'���ʉ�
			If Len(sname0) > 0 Then
				aname = aname & " " & sname0
			End If
			
			'�퓬�A�j���\��
			ShowAnimation(aname)
		End If
		
		'�\������U���A�j���̎��
		aname = wtype & "�U��"
		
		'���ˉ�
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'�摜
		If Len(bmpname) > 0 Then
			aname = aname & " " & bmpname
		End If
		
		'�F
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'���ʉ�
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'�U���A�j���\��
		ShowAnimation(aname)
	End Sub
	
	'����g�p���̌��ʉ�
	Public Sub AttackSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'�t���O���N���A
		IsWavePlayed = False
		
		'�E�N���b�N���͌��ʉ����X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'���ʉ����K�v�Ȃ�����
		If u.IsWeaponClassifiedAs(w, "��") Or u.IsWeaponClassifiedAs(w, "��") Or u.IsWeaponClassifiedAs(w, "��") Then
			Exit Sub
		End If
		If InStr(wname, "�r�[���T�[�x��") > 0 Then
			Exit Sub
		End If
		If InStrNotNest(wclass, "��") > 0 Then
			If InStr(wname, "�e��") > 0 Then
				Exit Sub
			End If
		End If
		
		'���ʉ��̍Đ���
		num = CountAttack(u, w)
		
		'���햼�ɉ����Č��ʉ���I��
		If InStr(wname, "��C") > 0 Or InStr(wname, "���C") > 0 Then
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Cannon.wav"
			End If
		ElseIf InStr(wname, "�΋�C") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
				num = 4
			Else
				sname = "MachineCannon.wav"
			End If
		ElseIf InStr(wname, "���[�U�[") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�ÏW��") > 0 Or InStr(wname, "�M��") > 0 Or InStr(wname, "���") > 0 Or InStr(wname, "�Ռ��g") > 0 Or InStr(wname, "�d���g") > 0 Or InStr(wname, "�d�g") > 0 Or InStr(wname, "���g") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�u���b�N�z�[��") > 0 Or InStr(wname, "�k��") > 0 Or InStr(wname, "�E�F�[�u") > 0 Or InStr(wname, "�g��") > 0 Or InStr(wname, "�\�j�b�N") > 0 Or InStr(wname, "�X�p�[�N") > 0 Or InStr(wname, "�G�l���M�[") > 0 Then 
			sname = "LaserGun.wav"
		ElseIf InStr(wname, "���q") > 0 Or InStr(wname, "�z�d�q") > 0 Or InStr(wname, "�z�q") > 0 Or InStr(wname, "�u���X�^�[") > 0 Or InStr(wname, "�u���X�g") > 0 Or InStr(wname, "�t�F�C�U�[") > 0 Or InStr(wname, "�f�B�X���v�^�[") > 0 Or InStr(wname, "�X�}�b�V���[") > 0 Or InStr(wname, "�X���b�V���[") > 0 Or InStr(wname, "�t���b�V���[") > 0 Or InStr(wname, "�f�B�o�C�_�[") > 0 Or InStr(wname, "�h���C�o�[") > 0 Or InStr(wname, "�V���g���[��") > 0 Or InStr(wname, "�j���[�g����") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "�C�I��") > 0 Or InStr(wname, "�v���~�l���X") > 0 Or InStr(wname, "�n�C�h��") > 0 Or InStr(wname, "�C���p���X") > 0 Or InStr(wname, "�t���C��") > 0 Or InStr(wname, "�T���V���C��") > 0 Then 
			sname = "Beam.wav"
		ElseIf InStr(wname, "�V���[�^�[") > 0 Then 
			If InStrNotNest(wclass, "��") > 0 Then
				sname = "Missile.wav"
			Else
				sname = "Beam.wav"
			End If
		ElseIf InStr(wname, "�r�[��") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "LaserGun.wav"
			End If
			If InStr(wname, "�o���J��") > 0 Or InStr(wname, "�}�V���K��") > 0 Then
				num = 4
			End If
		ElseIf InStr(wname, "�@�֏e") > 0 Or InStr(wname, "�@�e") > 0 Or InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�A�T���g���C�t��") > 0 Or InStr(wname, "�`�F�[�����C�t��") > 0 Or InStr(wname, "�p���b�g���C�t��") > 0 Or InStr(wname, "�}�E���[�C") > 0 Or InStr(wname, "�r�l�f") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "LaserGun.wav"
			Else
				sname = "MachineGun.wav"
			End If
			num = 1
		ElseIf InStr(wname, "�@�֖C") > 0 Or InStr(wname, "���˖C") > 0 Or InStr(wname, "�}�V���L���m��") > 0 Or InStr(wname, "���[�^�[�J�m��") > 0 Or InStr(wname, "�K���N���X�^�[") > 0 Or InStr(wname, "�`�F�[���K��") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "LaserGun.wav"
			Else
				sname = "MachineCannon.wav"
			End If
			num = 1
		ElseIf InStr(wname, "�K���|�b�h") > 0 Or InStr(wname, "�o���J��") > 0 Or InStr(wname, "�K�g�����O") > 0 Or InStr(wname, "�n���h���[���K��") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "LaserGun.wav"
			Else
				sname = "GunPod.wav"
			End If
			num = 1
		ElseIf InStr(wname, "���j�A�L���m��") > 0 Or InStr(wname, "���[���L���m��") > 0 Or InStr(wname, "���j�A�J�m��") > 0 Or InStr(wname, "���[���J�m��") > 0 Or InStr(wname, "���j�A�K��") > 0 Or InStr(wname, "���[���K��") > 0 Or (InStr(wname, "�d��") > 0 And InStr(wname, "�C") > 0) Then 
			PlayWave("Thunder.wav")
			Sleep(300)
			PlayWave("Cannon.wav")
			For i = 2 To num
				Sleep(130)
				PlayWave("Cannon.wav")
			Next 
		ElseIf InStr(wname, "���C�t��") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Rifle.wav"
			End If
		ElseIf InStr(wname, "�o�Y�[�J") > 0 Or InStr(wname, "�W���C�A���g�o�Y") > 0 Or InStr(wname, "�V���c�����t�@�E�X�g") > 0 Or InStr(wname, "�O���l�[�h") > 0 Or InStr(wname, "�O���l�C�h") > 0 Or InStr(wname, "�i�p�[��") > 0 Or InStr(wname, "�N���C���A") > 0 Or InStr(wname, "���P�b�g�C") > 0 Or InStr(wname, "�����C") > 0 Or InStr(wname, "�������C") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Bazooka.wav"
			End If
		ElseIf InStr(wname, "�����C") > 0 Or InStr(wname, "�I�[�g�L���m��") > 0 Then 
			sname = "FastGun.wav"
			num = 1
		ElseIf InStr(wname, "�|") > 0 Or InStr(wname, "�A���[") > 0 Or InStr(wname, "�{�[�K��") > 0 Or InStr(wname, "�{�E�K��") > 0 Or InStr(wname, "�����O�{�E") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�j") > 0 Or InStr(wname, "��") > 0 Then 
			sname = "Bow.wav"
		ElseIf InStr(wname, "�}�C��") > 0 Or InStr(wname, "�N���b�J�[") > 0 Or InStr(wname, "�蓊�e") > 0 Or InStr(wname, "��֒e") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�X�����O") > 0 Or InStr(wname, "�藠��") > 0 Or InStr(wname, "�ꖳ") > 0 Or InStr(wname, "�N�i�C") > 0 Then 
			sname = "Swing.wav"
		ElseIf InStr(wname, "���e") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Then 
			sname = "Bomb.wav"
		ElseIf InStr(wname, "�@��") > 0 Then 
			sname = "Explode.wav"
		ElseIf InStr(wname, "�}�C�N���~�T�C��") > 0 And InStrNotNest(wclass, "�l") > 0 Then 
			sname = "MicroMissile.wav"
			num = 1
		ElseIf InStr(wname, "�S���ʃ~�T�C��") > 0 Then 
			sname = "MicroMissile.wav"
			num = 1
		ElseIf InStr(wname, "�~�T�C��") > 0 Or InStr(wname, "���P�b�g") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�����e") > 0 Or InStr(wname, "�}���`�|�b�h") > 0 Or InStr(wname, "�}���`�����`���[") > 0 Or InStr(wname, "�V���b�g") > 0 Or InStr(wname, "�t���t�@�C�A") > 0 Or InStr(wname, "�X�g���[��") > 0 Or InStr(wname, "�i�b�N��") > 0 Or InStr(wname, "�p���`") > 0 Or InStr(wname, "�S�r") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�ˏo") > 0 Or InStr(wname, "�����`���[") > 0 Or InStr(wname, "�`�s�l") > 0 Or InStr(wname, "�`�`�l") > 0 Or InStr(wname, "�`�f�l") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Missile.wav"
			End If
		ElseIf InStr(wname, "�C") > 0 Or InStr(wname, "�e") > 0 Or InStr(wname, "�L���m��") > 0 Or InStr(wname, "�J�m��") > 0 Or InStr(wname, "�{��") > 0 Or InStr(wname, "�΋�") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Cannon.wav"
			End If
		ElseIf InStr(wname, "�K��") > 0 Or InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "���{�����@�[") > 0 Or InStr(wname, "�}�O�i��") > 0 Or InStr(wname, "���C�A�b�g") > 0 Or InStr(wname, "�e") > 0 Then 
			If InStrNotNest(wclass, "�a") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Gun.wav"
			End If
		ElseIf InStr(wname, "�\�j�b�N�u���[�h") > 0 Or InStr(wname, "�r�[���J�b�^�[") > 0 Or InStr(wname, "�X���C�T�[") > 0 Then 
			sname = "Saber.wav"
		ElseIf InStr(wname, "�d��") > 0 Or InStr(wname, "�O���r") > 0 Then 
			sname = "Shock(Low).wav"
		ElseIf InStr(wname, "�X�g�[��") > 0 Or InStr(wname, "�g���l�[�h") > 0 Or InStr(wname, "�n���P�[��") > 0 Or InStr(wname, "�^�C�t�[��") > 0 Or InStr(wname, "�T�C�N����") > 0 Or InStr(wname, "�u���U�[�h") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�Q��") > 0 Or InStr(wname, "�䕗") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�t���[�U�[") > 0 Or InStr(wname, "�e���L�l�V�X") > 0 Then 
			sname = "Storm.wav"
			num = 1
		ElseIf InStr(wname, "�u�[������") > 0 Or InStr(wname, "�E�F�b�u") > 0 Then 
			sname = "Swing.wav"
			num = 5
		ElseIf InStr(wname, "�T���_�[") > 0 Or InStr(wname, "���C�g�j���O") > 0 Or InStr(wname, "�{���g") > 0 Or InStr(wname, "���") > 0 Or InStr(wname, "���d") > 0 Or InStr(wname, "�d��") > 0 Or InStr(wname, "�d��") > 0 Or InStr(wname, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then 
			sname = "Thunder.wav"
			num = 1
		ElseIf InStr(wname, "�Ή�����") > 0 Then 
			sname = "AntiShipMissile.wav"
		ElseIf InStr(wname, "�Ή�") > 0 Or InStr(wname, "��") > 0 Then 
			sname = "Fire.wav"
			num = 1
		ElseIf InStr(wname, "���@") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStr(wname, "�T�C�R�L�l�V�X") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�A���J�[") > 0 Then 
			sname = "Whiz.wav"
		ElseIf InStr(wname, "�A") > 0 Or InStr(wname, "�o�u��") > 0 Then 
			sname = "Bubble.wav"
		ElseIf Right(wname, 1) = "�t" Then 
			sname = "Shower.wav"
		ElseIf Right(wname, 3) = "�u���X" Or Right(wname, 3) = "�̑�" Then 
			If InStrNotNest(wclass, "��") > 0 Then
				sname = "AntiShipMissile.wav"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				sname = "Storm.wav"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				sname = "GunPod.wav"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				sname = "Hide.wav"
			Else
				sname = "AntiShipMissile.wav"
			End If
		ElseIf InStr(wname, "��Ďˌ�") > 0 Then 
			sname = "MultipleRocketLauncher(Light).wav"
			num = 1
		ElseIf InStrNotNest(wclass, "�a") > 0 Then 
			'�Ȃ񂩕�����񂯂ǃr�[��
			sname = "Beam.wav"
		ElseIf InStrNotNest(wclass, "�e") > 0 Then 
			'�Ȃ񂩕�����񂯂Ǐe
			sname = "Gun.wav"
		End If
		
		'���ʉ��Ȃ��H
		If sname = "" Then
			'�t���O���N���A
			IsWavePlayed = False
			Exit Sub
		End If
		
		For i = 1 To num
			PlayWave(sname)
			
			'�E�F�C�g������
			Sleep(130)
			If sname = "Swing.wav" Then
				Sleep(150)
			End If
		Next 
		
		'�t���O���N���A
		IsWavePlayed = False
	End Sub
	
	
	'���햽�����̓������
	Public Sub HitEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, Optional ByVal hit_count As Short = 0)
		
		'�E�N���b�N���͓�����ʂ��X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			HitAnimation(u, w, t, hit_count)
		Else
			HitSound(u, w, t, hit_count)
		End If
	End Sub
	
	'���햽�����̃A�j���[�V����
	Public Sub HitAnimation(ByRef u As Unit, ByVal w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, sname As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim i As Short
		
		'�퓬�A�j���񎩓��I���I�v�V����
		If IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			ShowAnimation("�_���[�W����")
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'�}�b�v�U���̏ꍇ�͕���ɂ�����炸�_���[�W���g��
		If InStrNotNest(wclass, "�l") > 0 Then
			'�U����0�̍U���̏ꍇ�́u�_���[�W�v�̃A�j�����g�p���Ȃ�
			If u.WeaponPower(w, "") = 0 Then
				Exit Sub
			End If
			
			wtype = "�_���[�W"
			
			If IsBeamWeapon(wname, wclass, cname) Or InStr(wname, "�~�T�C��") > 0 Or InStr(wname, "���P�b�g") > 0 Then
				sname = "Explode.wav"
			End If
			
			GoTo FoundWeaponType
		End If
		
		'�񓁗��H
		If InStr(wname, "�_�u��") > 0 Or InStr(wname, "�c�C��") > 0 Or InStr(wname, "�f���A��") > 0 Or InStr(wname, "�o") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�Q�A") > 0 Or InStr(wname, "��A") > 0 Or InStr(wname, "�A��") > 0 Then
			double_weapon = True
		End If
		
		'�A���U���H
		If InStr(wname, "�_�u��") > 0 Or InStr(wname, "�c�C��") > 0 Or InStr(wname, "�R���r�l�[�V����") > 0 Or InStr(wname, "�A") > 0 Or InStrNotNest(wclass, "�A") > 0 Then
			double_attack = True
		End If
		
		'���ŁH
		If InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�S��") > 0 Then
			combo_attack = True
		End If
		
		'���ꂩ�畐��̎�ނ𔻒�
		
		'�܂��͔�����p����̔���
		If InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And InStrNotNest(wclass, "��") = 0 And Not (InStrNotNest(wclass, "�i") > 0 And InStrNotNest(wclass, "��") > 0) Then
			GoTo SkipInfightWeapon
		End If
		
		'�ˌ��n(������\���ēːi����)
		
		If InStr(wname, "�ˌ�") > 0 Or InStr(wname, "�ːi") > 0 Or InStr(wname, "�`���[�W") > 0 Then
			Select Case WeaponInHand
				Case ""
					'�Y������
				Case Else
					wtype = WeaponInHand & "�ˌ�"
					GoTo FoundWeaponType
			End Select
		End If
		
		'�Ō��n
		
		If InStrNotNest(wclass, "��") > 0 And (InStr(wname, "�p���`") > 0 Or InStr(wname, "�i�b�N��") > 0) Then
			wtype = "���P�b�g�p���`"
			GoTo FoundWeaponType
		End If
		
		'����
		If InStr(wname, "���@") > 0 Or Right(wname, 2) = "�A�[�c" Or Right(wname, 5) = "�X�g���C�N" Then
			wtype = "�A��"
			GoTo FoundWeaponType
		End If
		
		'�ʏ�Ō�
		If InStr(wname, "�p���`") > 0 Or InStr(wname, "�i�b�N��") > 0 Or InStr(wname, "�u���[") > 0 Or InStr(wname, "�`���b�v") > 0 Or InStr(wname, "�r���^") > 0 Or InStr(wname, "��") > 0 Or Right(wname, 1) = "��" Or Right(wname, 1) = "�r" Or InStr(wname, "�i��") > 0 Or InStr(wname, "�g���t�@�[") > 0 Or InStr(wname, "�_") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�X�^�b�t") > 0 Or InStr(wname, "���C�X") > 0 Or Right(wname, 2) = "���`" Or InStr(wname, "��") > 0 Or InStr(wname, "�E�B�b�v") > 0 Or InStr(wname, "�`�F�[��") > 0 Or InStr(wname, "���b�h") > 0 Or InStr(wname, "���[�j���O�X�^�[") > 0 Or InStr(wname, "�t���C��") > 0 Or InStr(wname, "�k���`���N") > 0 Or InStr(wname, "�O�ߍ�") > 0 Or (InStr(wname, "�`�F�[��") > 0 And InStr(wname, "�`�F�[���\�[") = 0) Or InStr(wname, "�o�b�g") > 0 Or InStr(wname, "�M�^�[") > 0 Or InStr(wname, "�|��") > 0 Or InStr(wname, "�n���Z��") > 0 Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Or InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Then 
				wtype = "�A��"
			Else
				wtype = "�Ō�"
			End If
			
			If Right(wname, 2) = "���`" Or InStr(wname, "��") > 0 Or InStr(wname, "�E�B�b�v") > 0 Or InStr(wname, "�`�F�[��") > 0 Or InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Or (InStr(wname, "���b�h") > 0 And wname <> "���b�h") Or InStr(wname, "�|��") > 0 Or InStr(wname, "�n���Z��") > 0 Then
				sname = "Whip.wav"
			ElseIf InStr(wname, "�����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�r���^") > 0 Then 
				sname = "Slap.wav"
			End If
			
			GoTo FoundWeaponType
		End If
		
		'���Ō�
		If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�����A�[�g") > 0 Or InStr(wname, "�L�b�N") > 0 Or InStr(wname, "�R") > 0 Or InStr(wname, "�r") > 0 Or Right(wname, 1) = "��" Or InStr(wname, "�w�b�h�o�b�h") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�n���}�[") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "���[��") > 0 Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Then 
				wtype = "�A��"
			Else
				wtype = "����"
			End If
			
			If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
				PlayWave("Bazooka.wav")
			End If
			
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�A�b�p�[") > 0 Then
			wtype = "�A�b�p�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�^�b�N��") > 0 Or InStr(wname, "�̓�") > 0 Or InStr(wname, "�`���[�W") > 0 Or InStr(wname, "�Ԃ����܂�") > 0 Or InStr(wname, "�o���J�[") > 0 Then
			wtype = "����"
			sname = "Crash.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�o���J�[") > 0 Then
			wtype = "����"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "����"
			sname = "Crash.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�i��") > 0 Then
			wtype = "�Ō�"
			GoTo FoundWeaponType
		End If
		
		'�a���n
		
		If InStr(wname, "�r�[��") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "���[�U�[") > 0 Or InStr(wname, "�u���X�^�[") > 0 Or InStr(wname, "���C�g") > 0 Then
			If InStr(wname, "�v���Y�}") > 0 Then
				cname = "�O���[��"
			ElseIf InStr(wname, "���[�U�[") > 0 Then 
				cname = "�u���["
			ElseIf InStr(wname, "���C�g") > 0 Then 
				cname = "�C�G���["
			End If
			
			If InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�Z�C�o�[") > 0 Or InStr(wname, "�u���[�h") > 0 Or InStr(wname, "�\�[�h") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
				If InStr(wname, "�n�C�p�[") > 0 Or InStr(wname, "�����O") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
					wtype = "�n�C�p�[�r�[���T�[�x��"
				Else
					wtype = "�r�[���T�[�x��"
				End If
				
				If double_weapon Then
					wtype = "�_�u��" & wtype
				ElseIf InStr(wname, "��]") > 0 Then 
					wtype = "��]" & wtype
				End If
				
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�J�b�^�[") > 0 Then
				If InStr(wname, "�n�C�p�[") > 0 Or InStr(wname, "�����O") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
					wtype = "�G�i�W�[�u���[�h"
				Else
					wtype = "�G�i�W�[�J�b�^�["
				End If
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Then
				wtype = "�r�[���i�C�t"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "�i�M�i�^") > 0 Then
				wtype = "��]�r�[���T�[�x��"
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "�\�[�h") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�V�~�^�[") > 0 Or InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�J�b�g���X") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�a") > 0 Or InStr(wname, "�u���[�h") > 0 Or InStr(wname, "�n") > 0 Or InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�O���C�u") > 0 Or InStr(wname, "�i�M�i�^") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�J�b�g") > 0 Or InStr(wname, "�J�b�^�[") > 0 Or InStr(wname, "�X���b�V��") > 0 Or InStr(wname, "����") > 0 Then
			If combo_attack Then
				wtype = "�a������"
			ElseIf double_weapon Then 
				wtype = "�A�a��"
			ElseIf double_attack Then 
				wtype = "�_�u���a��"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				wtype = "���a��"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				wtype = "���a��"
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				wtype = "���a��"
			ElseIf InStr(wname, "���|��") > 0 Or InStr(wname, "�c") > 0 Then 
				wtype = "���|��"
			ElseIf InStr(wname, "����") > 0 Or InStr(wname, "��") > 0 Then 
				wtype = "�Ȃ�����"
			ElseIf InStr(wname, "�a") > 0 Then 
				wtype = "��a��"
			ElseIf InStrNotNest(wclass, "�i") > 0 Then 
				wtype = "�a��グ"
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�_�[�N") > 0 Or InStr(wname, "�f�X") > 0 Then 
				wtype = "���a��"
			Else
				wtype = "�a��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���[�e��") > 0 Then
			wtype = "�_�u���a��"
			GoTo FoundWeaponType
		End If
		
		'�h�ˌn
		
		If InStr(wname, "�X�s�A") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�����X") > 0 Or InStr(wname, "�����T�[") > 0 Or InStr(wname, "�g���C�f���g") > 0 Or InStr(wname, "�W���x����") > 0 Or InStr(wname, "���C�s�A") > 0 Or wname = "���b�h" Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Then 
				wtype = "�A��"
			Else
				wtype = "�h��"
			End If
			GoTo FoundWeaponType
		End If
		
		'���̑��i���n
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�N���[") > 0 Or InStr(wname, "�Ђ�����") > 0 Then
			If InStr(wname, "�A�[��") > 0 Then
				wtype = "�Ō�"
				sname = "Crash.wav"
			Else
				wtype = "�܌�"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "���݂�") > 0 Then
			wtype = "���ݕt��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�h����") > 0 Then
			wtype = "�h����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���{��") > 0 Then
			wtype = "���{��"
			GoTo FoundWeaponType
		End If
		
		'�͂݌n
		
		If InStr(wname, "�X�[�v���b�N�X") > 0 Or InStr(wname, "����") > 0 Or wname = "�Ԃ�" Then
			wtype = "������΂�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�q�[���z�[���h") > 0 Then
			wtype = "���ł�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�u���[�J�[") > 0 Then
			wtype = "�w�����ł�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�ł�") > 0 Or InStr(wname, "�z�[���h") > 0 Or InStr(wname, "�c�C�X�g") > 0 Or InStr(wname, "�i��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�܂�") > 0 Then
			wtype = "�����ł�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�W���C�A���g�X�C���O") > 0 Then
			wtype = "�W���C�A���g�X�C���O"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�n����") > 0 Then
			wtype = "�n����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�u���[���o�X�^�[") > 0 Then
			wtype = "�u���[���o�X�^�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�N�����[�o�b�N�h���C�o�[") > 0 Then
			wtype = "�X�N�����[�o�b�N�h���C�o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�N�����[�h���C�o�[") > 0 Then
			wtype = "�X�N�����[�h���C�o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�o�b�N�h���C�o�[") > 0 Then
			wtype = "�o�b�N�h���C�o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�h���C�o�[") > 0 Then
			wtype = "�h���C�o�["
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Then
			wtype = "���ݒׂ�"
			GoTo FoundWeaponType
		End If
		
		'�ڍׂ�������Ȃ���������
		If InStrNotNest(wclass, "��") > 0 Then
			'�������Ă���A�C�e�����畐�������
			For i = 1 To u.CountItem
				With u.Item(i)
					If .Activated And (.Part = "����" Or .Part = "�Ў�" Or .Part = "����") Then
						wtype = CheckWeaponType(.Nickname, "")
						If wtype = "" Then
							wtype = CheckWeaponType(.Class0, "")
						End If
						Exit For
					End If
				End With
			Next 
			Select Case wtype
				Case "�X�s�A", "�����X", "�g���C�f���g", "�a��", "�G�X�g�b�N"
					If combo_attack Then
						wtype = "����"
					ElseIf double_attack Then 
						wtype = "�A��"
					Else
						wtype = "�h��"
					End If
				Case Else
					If combo_attack Then
						wtype = "�a������"
					ElseIf double_weapon Then 
						wtype = "�_�u���a��"
					ElseIf double_attack Then 
						wtype = "�A�a��"
					ElseIf InStrNotNest(wclass, "��") > 0 Then 
						wtype = "���a��"
					ElseIf InStrNotNest(wclass, "��") > 0 Then 
						wtype = "���a��"
					ElseIf InStrNotNest(wclass, "��") > 0 Then 
						wtype = "���a��"
					ElseIf InStrNotNest(wclass, "�i") > 0 Then 
						wtype = "�a��グ"
					Else
						wtype = "�a��"
					End If
			End Select
			GoTo FoundWeaponType
		End If
		
		'�ڍׂ�������Ȃ������ߐڋZ
		If InStrNotNest(wclass, "��") > 0 And InStrNotNest(wclass, "��") > 0 Then
			If combo_attack Then
				wtype = "����"
			ElseIf double_attack Then 
				wtype = "�A��"
			Else
				wtype = "�Ō�"
			End If
			GoTo FoundWeaponType
		End If
		
SkipInfightWeapon: 
		
		'�ˌ�����(�i������)
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "�g�}�z�[�N") > 0 Or InStr(wname, "�\�[�T�[") > 0 Or InStr(wname, "�`���N����") > 0 Then
			wtype = "�_���[�W"
			sname = "Saber.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�p���`") > 0 Or InStr(wname, "�n���}�[") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�S��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�I") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�u�[������") > 0 Then
			wtype = "�Ō�"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�藠��") > 0 Or InStr(wname, "�N�i�C") > 0 Or InStr(wname, "�ꖳ") > 0 Then
			wtype = "�h��"
			GoTo FoundWeaponType
		End If
		
		'������ʏ�ˌ��U��
		
		'�܂��͌����n�̍U�����ǂ����𔻒肷��
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "�r�[��"
		End If
		
		If wtype = "�r�[��" Then
			'���e�n���픻����X�L�b�v
			GoTo SkipNormalWeapon
		End If
		
		'�ˌ�����(���e�n)
		
		If InStr(wname, "�|") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�����O�{�E") > 0 Or InStr(wname, "�{�E�K��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�A���[") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�o���J��") > 0 Then
			wtype = "�o���J��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�K�g�����O") > 0 Or InStr(wname, "�`�F�[���K��") Or InStr(wname, "�K�������`���[") Then
			wtype = "�K�g�����O"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�@�֏e") > 0 Then
			If InStr(wname, "�w�r�[") > 0 Or InStr(wname, "�d") > 0 Then
				wtype = "�w�r�[�}�V���K��"
			Else
				wtype = "�}�V���K��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�@�e") > 0 Or InStr(wname, "�@�֖C") > 0 Then
			wtype = "�}�V���K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�}�V���L���m��") > 0 Or InStr(wname, "�I�[�g�L���m��") > 0 Or InStr(wname, "���˖C") > 0 Then
			wtype = "�w�r�[�}�V���K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�V���b�g�K��") > 0 Or InStr(wname, "�U�e") > 0 Or InStr(wname, "�g�U�o�Y�[�J") > 0 Then
			wtype = "�V���b�g�K��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�x�A�����O") > 0 Or InStr(wname, "�N���C���A") > 0 Then
			wtype = "�x�A�����O"
			GoTo FoundWeaponType
		End If
		
SkipNormalWeapon: 
		
		'�ˌ�����(�G�l���M�[�n)
		
		If InStr(wname, "������") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�j�����") > 0 Then
			wtype = "�j�����"
			GoTo FoundWeaponType
		End If
		
		If wtype = "�r�[��" Then
			If InStr(CurrentWeaponType, "�r�[��") > 0 Or InStr(CurrentWeaponType, "���[�U�[") > 0 Then
				'�\�ł���Δ��ˎ��̃G�t�F�N�g�Ɠ��ꂷ��
				Select Case CurrentWeaponType
					Case "�r�[�����C�t��"
						wtype = "���r�[��"
					Case "�_�u���r�[�����C�t��"
						wtype = "�Q�A���r�[��"
					Case "�r�[�������`���["
						wtype = "���r�[��"
					Case "�_�u���r�[�������`���["
						wtype = "�Q�A���r�[��"
					Case "�o�X�^�[�r�[�����C�t��"
						wtype = "��r�[��"
					Case "���[�U�[�K��"
						wtype = "�j�[�h�����[�U�["
					Case "���[�U�[�}�V���K��"
						wtype = "�j�[�h�����[�U�[�A��"
					Case Else
						wtype = CurrentWeaponType
				End Select
			Else
				If InStr(wname, "�n�C���K") > 0 Or InStr(wname, "�o�X�^�[") > 0 Or InStr(wname, "��") > 0 Or Left(wname, 2) = "�M�K" Then
					wtype = "��r�[��"
				ElseIf InStr(wname, "���K") > 0 Or InStr(wname, "�n�C") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Then 
					wtype = "���r�[��"
				ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "�΋�") > 0 Then 
					wtype = "�j�[�h�����[�U�[�A��"
				ElseIf InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "�~�j") > 0 Or InStr(wname, "��") > 0 Then 
					wtype = "�j�[�h�����[�U�["
				ElseIf InStr(wname, "�����`���[") > 0 Or InStr(wname, "�L���m��") > 0 Or InStr(wname, "�J�m��") > 0 Or InStr(wname, "�C") > 0 Then 
					wtype = "���r�[��"
				Else
					wtype = "���r�[��"
				End If
				
				Select Case wtype
					Case "���r�[��", "���r�[��"
						If double_weapon Then
							wtype = "�Q�A" & wtype
						End If
				End Select
				
				If InStr(wname, "�g�U") > 0 Or InStr(wname, "����") > 0 Then
					wtype = "�g�U�r�[��"
				End If
				
				If InStr(wname, "�z�[�~���O") > 0 Or InStr(wname, "�U��") > 0 Then
					wtype = "�z�[�~���O���[�U�["
				End If
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "������"
			GoTo FoundWeaponType
		End If
		
		'�����n
		
		If InStr(wname, "�s�X�g��") > 0 Or InStr(wname, "���e") > 0 Or InStr(wname, "���{���o�[") > 0 Or InStr(wname, "���{�����@�[") > 0 Or InStr(wname, "�e") > 0 Or Right(wname, 2) = "�K��" Or InStr(wname, "���C�t��") > 0 Then
			wtype = "�e�e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "����") > 0 Or CurrentWeaponType = "�������e" Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�~�T�C��") > 0 Or InStr(wname, "���P�b�g") > 0 Or InStr(wname, "���e") > 0 Or InStr(wname, "�_�C�i�}�C�g") > 0 Or InStr(wname, "�֒e") > 0 Or InStr(wname, "�����e") > 0 Or InStr(wname, "�O���l�[�h") > 0 Or InStr(wname, "��֒e") > 0 Or InStr(wname, "�N���b�J�[") > 0 Or InStr(wname, "�f�B�X�`���[�W���[") > 0 Or InStr(wname, "�}�C��") > 0 Or InStr(wname, "�{��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�@��") > 0 Or InStr(wname, "�o�Y�[�J") > 0 Or InStr(wname, "�V���c�����t�@�E�X�g") > 0 Then
			If InStr(wname, "�j") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�A�g�~�b�N") > 0 Or InStr(wname, "��") > 0 Then
				wtype = "������"
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "�r�b�N") > 0 Or InStr(wname, "�W���C�A���g") > 0 Or InStr(wname, "���K") > 0 Then 
				wtype = "�唚��"
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "�~�j") > 0 Or InStr(wname, "�}�C�N��") > 0 Then 
				wtype = "������"
			Else
				wtype = "����"
			End If
			
			'�A�������H
			
			If wtype = "������" Then
				GoTo FoundWeaponType
			End If
			
			attack_times = CountAttack0(u, w)
			If InStrNotNest(wclass, "�A") > 0 Then
				attack_times = hit_count
			End If
			
			If attack_times = 1 Then
				attack_times = 0
				GoTo FoundWeaponType
			End If
			
			If wtype = "������" Then
				wtype = "�A������"
			Else
				wtype = "�A��" & wtype
			End If
			
			GoTo FoundWeaponType
		End If
		
		'���̑�����n
		
		If InStr(wname, "�d��") > 0 Or InStr(wname, "�d��") > 0 Or InStr(wname, "�G���N�g") > 0 Then
			wtype = "�j�����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "���C�g�j���O") > 0 Or InStr(wname, "�T���_�[") > 0 Or Right(wname, 2) = "���" Or InStrNotNest(wclass, "�d") > 0 Then
			If InStrNotNest(wclass, "��") = 0 Then
				wtype = "���d"
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "����") > 0 Or InStr(wname, "�u���U�[�h") > 0 Or InStr(wname, "�A�C�X�X�g�[��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X�g�[��") > 0 Or InStr(wname, "�n���P�[��") > 0 Or InStr(wname, "�^�C�t�[��") > 0 Or InStr(wname, "�䕗") > 0 Or InStr(wname, "��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�E�B���h") > 0 Or InStr(wname, "�E�C���h") > 0 Or InStr(wname, "��") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�g���l�[�h") > 0 Or InStr(wname, "�T�C�N����") Or InStr(wname, "����") > 0 Or InStr(wname, "�Q��") > 0 Then
			wtype = "����"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�A") > 0 Or InStr(wname, "�o�u��") > 0 Or InStr(wname, "����") > 0 Then
			wtype = "�A"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�d��") > 0 Or InStr(wname, "�O���r") > 0 Or InStr(wname, "�u���b�N�z�[��") > 0 Or InStr(wname, "�k��") > 0 Then
			wtype = "�d�͈��k"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�X���E") > 0 Then
			wtype = "���ԋt�s"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "�X���[�N") > 0 Or Right(wname, 2) = "�K�X" Or Right(wname, 1) = "��" Or InStr(wname, "�E�q") > 0 Then
			wtype = "��"
			If InStr(wname, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
				cname = "��"
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ή��e") > 0 Then
			wtype = "�Ή��e"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�Ή�����") > 0 Or Right(wname, 2) = "�Ή�" Then
			wtype = "�Ή�����"
			GoTo FoundWeaponType
		End If
		
		If Right(wname, 5) = "�t�@�C�A�[" Or Right(wname, 5) = "�t�@�C���[" Or Right(wname, 4) = "�t�@�C�A" Or Right(wname, 4) = "�t�@�C��" Then
			If InStrNotNest(wclass, "��") = 0 And Left(wname, 2) <> "�t��" Then
				If InStrNotNest(wclass, "�p") > 0 Then
					wtype = "��"
				Else
					wtype = "�Ή�����"
				End If
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "��") > 0 Or Right(wname, 3) = "�u���X" Then
			If InStrNotNest(wclass, "��") = 0 Then
				wtype = "�Ή�����"
				
				Select Case SpellColor(wname, wclass)
					Case "��", "��", "��", "��", "��"
						cname = SpellColor(wname, wclass)
						sname = "Breath.wav"
				End Select
				
				GoTo FoundWeaponType
			End If
		End If
		
		If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�t�@�C���[") > 0 Then
			wtype = "��"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "���S�C") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�_����") > 0 Or Right(wname, 1) = "�t" Or Right(wname, 1) = "�_" Then
			wtype = "��"
			If InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
				cname = "��"
			ElseIf InStr(wname, "�_") > 0 Then 
				cname = "��"
			Else
				cname = "��"
			End If
			sname = "Splash.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "�z��") > 0 Or InStr(wname, "�h���C��") > 0 Or InStrNotNest(wclass, "�z") > 0 Or InStrNotNest(wclass, "��") > 0 Then
			wtype = "�z��"
			GoTo FoundWeaponType
		End If
		
		'�U����0�̍U���̏ꍇ�́u�_���[�W�v�̃A�j�����g�p���Ȃ�
		If u.WeaponPower(w, "") = 0 Then
			Exit Sub
		End If
		
		'�f�t�H���g
		wtype = "�_���[�W"
		
FoundWeaponType: 
		
		'�A�j���̕s������h�����߁A������΂����̓A�j�����ʂ�Ō��ɗ}���Ă���
		Select Case wtype
			Case "����", "����"
				If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "�j") > 0 Then
					wtype = "�Ō�"
				End If
		End Select
		
		'�\���F���ŏI����
		If InStr(wname, "���b�h") > 0 Or InStr(wname, "��") > 0 Then
			cname = "��"
		ElseIf InStr(wname, "�u���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�C�G���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�O���[��") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�s���N") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�u���E��") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�u���b�N") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�_�[�N") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		ElseIf InStr(wname, "�z���C�g") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�z�[���[") > 0 Or InStr(wname, "��") > 0 Then 
			cname = "��"
		End If
		
		'�Q��ނ̃A�j����g�ݍ��킹��ꍇ
		If Len(wtype0) > 0 Then
			'�\�����閽���A�j���̎��
			aname = wtype0 & "����"
			
			'�F
			If Len(cname) > 0 Then
				aname = aname & " " & cname
			End If
			
			'�����A�j���\��
			ShowAnimation(aname)
		End If
		
		'�\�����閽���A�j���̎��
		aname = wtype & "����"
		
		'�F
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'���ʉ�
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'������
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'�����A�j���\��
		ShowAnimation(aname)
	End Sub
	
	'���햽�����̌��ʉ�
	Public Sub HitSound(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wname, wclass As String
		Dim num, i As Short
		
		'�E�N���b�N���͌��ʉ����X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'���ʉ��̍Đ���
		num = CountAttack(u, w)
		
		'����ɉ����Č��ʉ����Đ�
		If InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Or InStrNotNest(wclass, "��") > 0 Then
			If InStr(wname, "�f�B�X�J�b�^�[") > 0 Or InStr(wname, "���b�p�[") > 0 Or InStr(wname, "�X�p�C�h") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�M") > 0 Then
				PlayWave("Swing.wav")
				Sleep(200)
				PlayWave("Sword.wav")
				For i = 2 To num
					Sleep(200)
					PlayWave("Sword.wav")
				Next 
			ElseIf InStr(wname, "�v���O���b�V�u�i�C�t") > 0 Or InStr(wname, "�h����") > 0 Then 
				PlayWave("Drill.wav")
			ElseIf InStr(wname, "�T�[�x��") > 0 Or InStr(wname, "�Z�C�o�[") > 0 Or InStr(wname, "�\�[�h") > 0 Or InStr(wname, "�u���[�h") > 0 Or InStr(wname, "�X�p�b�h") > 0 Or InStr(wname, "�Z�[�o�[") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "�i�C�t") > 0 Or InStr(wname, "�g�}�z�[�N") > 0 Or InStr(wname, "���C�X") > 0 Or InStr(wname, "�A�b�N�X") > 0 Or InStr(wname, "�O���C�u") > 0 Or InStr(wname, "�i�M�i�^") > 0 Or InStr(wname, "�r�A���L") > 0 Or InStr(wname, "�E�F�b�u") > 0 Or InStr(wname, "�U���o�[") > 0 Or InStr(wname, "�}�[�J�[") > 0 Or InStr(wname, "�o�X�^�[") > 0 Or InStr(wname, "�u���X�^�[") > 0 Or InStr(wname, "�N���[") > 0 Or InStr(wname, "�W�U�[�X") > 0 Or InStr(wname, "�u�[������") > 0 Or InStr(wname, "�\�[�T�[") > 0 Or InStr(wname, "���U�[") > 0 Or InStr(wname, "���C�o�[") > 0 Or InStr(wname, "�T�C�Y") > 0 Or InStr(wname, "�V���[�e��") > 0 Or InStr(wname, "�J�b�^�[") > 0 Or InStr(wname, "�X�p�C�N") > 0 Or InStr(wname, "�J�g���X") > 0 Or InStr(wname, "�G�b�W") > 0 Or (InStr(wname, "��") > 0 And InStr(wname, "�藠��") = 0) Or InStr(wname, "��") > 0 Or InStr(wname, "�a") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�n") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�J�}") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�����Â�") > 0 Or InStr(wname, "�n�T�~") > 0 Or InStr(wname, "�o�T�~") > 0 Or InStr(wname, "�H") > 0 Then 
				If Not t.IsHero Or InStr(wname, "�r�[��") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "���[�U�[") > 0 Or InStr(wname, "�Z�C�o�[") > 0 Then
					PlayWave("Saber.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Saber.wav")
					Next 
				Else
					PlayWave("Swing.wav")
					Sleep(190)
					PlayWave("Slash.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Slash.wav")
					Next 
				End If
			ElseIf InStr(wname, "�����T�[") > 0 Or InStr(wname, "�����X") > 0 Or InStr(wname, "�X�s�A") > 0 Or InStr(wname, "�g���C�f���g") > 0 Or InStr(wname, "�n�[�P��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�藠��") > 0 Or InStr(wname, "�ꖳ") > 0 Or InStr(wname, "�N�i�C") > 0 Or (InStr(wname, "�˂�") > 0 And InStr(wname, "��") = 0 And InStr(wname, "��") = 0) Then 
				If Not t.IsHero Or InStr(wname, "�r�[��") > 0 Or InStr(wname, "�v���Y�}") > 0 Or InStr(wname, "���[�U�[") > 0 Or InStr(wname, "�����T�[") > 0 Then
					PlayWave("Saber.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Saber.wav")
					Next 
				Else
					PlayWave("Swing.wav")
					Sleep(190)
					PlayWave("Stab.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Stab.wav")
					Next 
				End If
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "�t�@���O") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "���݂�") > 0 Or InStr(wname, "�{") > 0 Then 
				If Not t.IsHero Then
					PlayWave("Saber.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Saber.wav")
					Next 
				Else
					PlayWave("Stab.wav")
					For i = 2 To num
						Sleep(350)
						PlayWave("Stab.wav")
					Next 
				End If
			ElseIf InStr(wname, "�X�g���C�N") > 0 Or InStr(wname, "�A�[�c") > 0 Or InStr(wname, "���@") > 0 Or InStr(wname, "�U����") > 0 Then 
				PlayWave("Combo.wav")
			ElseIf InStr(wname, "�i��") > 0 Or InStr(wname, "�p���`") > 0 Or InStr(wname, "�L�b�N") > 0 Or InStr(wname, "�`���b�v") > 0 Or InStr(wname, "�i�b�N��") > 0 Or InStr(wname, "�u���[") > 0 Or InStr(wname, "�n���}�[") > 0 Or InStr(wname, "�g���t�@�[") > 0 Or InStr(wname, "�k���`���N") > 0 Or InStr(wname, "�p�C�v") > 0 Or InStr(wname, "�����A�b�g") > 0 Or InStr(wname, "�A�[��") > 0 Or InStr(wname, "�w�b�h�o�b�g") > 0 Or InStr(wname, "�X�����O") > 0 Or InStr(wname, "���˂�") > 0 Or InStr(wname, "�r") > 0 Or InStr(wname, "�R") > 0 Or InStr(wname, "�_") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�p") > 0 Or InStr(wname, "�K��") > 0 Or InStr(wname, "�S�r") > 0 Then 
				PlayWave("Punch.wav")
				For i = 2 To num
					Sleep(120)
					PlayWave("Punch.wav")
				Next 
			ElseIf InStr(wname, "�̓�����") > 0 Or InStr(wname, "�^�b�N��") > 0 Or InStr(wname, "�Ԃ����܂�") > 0 Or InStr(wname, "�ːi") > 0 Or InStr(wname, "�ˌ�") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�S��") > 0 Or InStr(wname, "���K�g���p���`") > 0 Or InStr(wname, "�S��") > 0 Or InStr(wname, "�{�[��") > 0 Or InStr(wname, "�ԗ�") > 0 Or InStr(wname, "�L���^�s��") > 0 Or InStr(wname, "�V�[���h") > 0 Then 
				PlayWave("Crash.wav")
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then 
				PlayWave("Bazooka.wav")
				For i = 2 To num
					Sleep(120)
					PlayWave("Bazooka.wav")
				Next 
			ElseIf InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�h���b�v") > 0 Then 
				PlayWave("Shock(Low).wav")
			ElseIf InStr(wname, "�����") > 0 Or InStr(wname, "�r���^") > 0 Then 
				PlayWave("Slap.wav")
				For i = 2 To num
					Sleep(120)
					PlayWave("Slap.wav")
				Next 
			ElseIf InStr(wname, "�|") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�A���[") > 0 Or InStr(wname, "�{�[�K��") > 0 Or InStr(wname, "�{�E�K��") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�����O�{�E") > 0 Or InStr(wname, "�j") > 0 Or InStr(wname, "�j�[�h��") > 0 Then 
				PlayWave("Stab.wav")
				For i = 2 To num
					Sleep(120)
					PlayWave("Stab.wav")
				Next 
			ElseIf InStr(wname, "��") > 0 Or InStr(wname, "���`") > 0 Or InStr(wname, "�E�C�b�v") > 0 Or InStr(wname, "�`�F�[��") > 0 Or InStr(wname, "���b�h") > 0 Or InStr(wname, "�e���^�N") > 0 Or InStr(wname, "�e�C��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "��") > 0 Then 
				PlayWave("Whip.wav")
			ElseIf InStr(wname, "����") > 0 Or InStr(wname, "�X�[�v���b�N") > 0 Or (InStr(wname, "�Ԃ�") > 0 And InStrNotNest(wclass, "��") > 0) Then 
				PlayWave("Swing.wav")
				Sleep(500)
				PlayWave("Shock(Low).wav")
				For i = 2 To num
					Sleep(700)
					PlayWave("Swing.wav")
					Sleep(500)
					PlayWave("Shock(Low).wav")
				Next 
			ElseIf InStr(wname, "���R���낵") > 0 Then 
				PlayWave("Swing.wav")
				Sleep(700)
				PlayWave("Swing.wav")
				Sleep(500)
				PlayWave("Swing.wav")
				Sleep(300)
				PlayWave("Shock(Low).wav")
			ElseIf InStr(wname, "�֐�") > 0 Or InStr(wname, "�ł�") > 0 Or InStr(wname, "�܂�") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�i��") > 0 Or InStr(wname, "�A�[�����b�N") > 0 Or InStr(wname, "�z�[���h") > 0 Then 
				PlayWave("Swing.wav")
				Sleep(190)
				PlayWave("BreakOff.wav")
			ElseIf InStrNotNest(wclass, "�j") > 0 Or InStr(wname, "�j") > 0 Or InStr(wname, "�����e") > 0 Then 
				PlayWave("Explode(Nuclear).wav")
			ElseIf InStr(wname, "�~�T�C��") > 0 Or InStr(wname, "���P�b�g") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�}���`�|�b�h") > 0 Or InStr(wname, "�}���`�����`���[") > 0 Or InStr(wname, "���e") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or Right(wname, 3) = "�}�C��" Or Right(wname, 2) = "�{��" Then 
				PlayWave("Explode(Small).wav")
				For i = 2 To num
					Sleep(130)
					PlayWave("Explode(Small).wav")
				Next 
			ElseIf InStr(wname, "�A���J�[") > 0 Then 
				'����
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				'�Ȃ񂩕�����񂯂Ǖ���
				PlayWave("Saber.wav")
				For i = 2 To num
					Sleep(350)
					PlayWave("Saber.wav")
				Next 
			ElseIf InStrNotNest(wclass, "��") > 0 Then 
				'�Ȃ񂩕�����񂯂Ǔːi�Z
				PlayWave("Punch.wav")
				For i = 2 To num
					Sleep(120)
					PlayWave("Punch.wav")
				Next 
			Else
				If Not t.IsHero Then
					PlayWave("Explode(Small).wav")
					For i = 2 To num
						Sleep(130)
						PlayWave("Explode(Small).wav")
					Next 
				End If
			End If
		Else
			If InStr(wname, "�X�g�[��") > 0 Or InStr(wname, "�g���l�[�h") > 0 Or InStr(wname, "�n���P�[��") > 0 Or InStr(wname, "�^�C�t�[��") > 0 Or InStr(wname, "�T�C�N����") > 0 Or InStr(wname, "�u���U�[�h") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�Q��") > 0 Or InStr(wname, "�䕗") > 0 Or InStr(wname, "��") > 0 Then
				'�������͖���
			ElseIf Right(wname, 1) = "�t" Then 
				PlayWave("Inori.wav")
			ElseIf InStr(wname, "����") > 0 Or InStr(wname, "�p�C���L�l�V�X") > 0 Then 
				PlayWave("Fire.wav")
			ElseIf wname = "�e���L�l�V�X" Then 
				PlayWave("Crash.wav")
			ElseIf InStr(wname, "�z��") > 0 Then 
				PlayWave("Charge.wav")
			ElseIf InStrNotNest(wclass, "�j") > 0 Then 
				PlayWave("Explode(Nuclear).wav")
			Else
				If Not t.IsHero Then
					PlayWave("Explode(Small).wav")
					For i = 2 To num
						Sleep(130)
						PlayWave("Explode(Small).wav")
					Next 
				End If
			End If
		End If
		
		'�t���O���N���A
		IsWavePlayed = False
	End Sub
	
	'������̌��ʉ�
	Public Sub DodgeEffect(ByRef u As Unit, ByRef w As Short)
		Dim wname, wclass As String
		Dim sname As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'������ʂ��w�肳��Ă���΂�����g�p
		If u.IsSpecialEffectDefined(wname & "(���)") Then
			u.SpecialEffect(wname & "(���)")
			Exit Sub
		End If
		
		If BattleAnimation Then
			Exit Sub
		End If
		
		'�U�����̌��ʉ������؂艹�݂̂ł���Ε��؂艹�͕s�v
		sname = u.SpecialEffectData(wname)
		If InStr(sname, ";") > 0 Then
			sname = Mid(sname, InStr(sname, ";"))
		End If
		If sname = "Swing.wav" Then
			Exit Sub
		End If
		
		'���؂艹���K�v���ǂ�������
		If InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Then
			PlayWave("Swing.wav")
		ElseIf InStrNotNest(wclass, "��") Then 
			If InStr(wname, "��") > 0 Or InStr(wname, "���`") > 0 Or InStr(wname, "�E�C�b�v") > 0 Or InStr(wname, "�`�F�[��") > 0 Or InStr(wname, "���b�h") > 0 Or InStr(wname, "�e���^�N") > 0 Or InStr(wname, "�e�C��") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "�G��") > 0 Or InStr(wname, "�G�r") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "��") > 0 Then
				PlayWave("Swing.wav")
			End If
		End If
	End Sub
	
	'����؂蕥�����̌��ʉ�
	Public Sub ParryEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'�E�N���b�N���͌��ʉ����X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'���ʉ������񐔂�ݒ�
		num = CountAttack(u, w)
		If InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�@�֏e") > 0 Or InStr(wname, "�A�T���g���C�t��") > 0 Or InStr(wname, "�o���J��") > 0 Then
			num = 4
		End If
		
		'��������ݒ�
		If InStrNotNest(wclass, "�e") Or InStrNotNest(wclass, "�i") Or InStrNotNest(wclass, "��") Or InStrNotNest(wclass, "��") Or InStr(wname, "�|") > 0 Or InStr(wname, "�A���[") > 0 Or InStr(wname, "�����O�{�E") > 0 Or InStr(wname, "�V���[�g�{�E") > 0 Or InStr(wname, "�{�[�K��") > 0 Or InStr(wname, "�{�E�K��") > 0 Or InStr(wname, "�j") > 0 Or InStr(wname, "�j�[�h��") > 0 Or InStr(wname, "�����T�[") > 0 Or InStr(wname, "�_�K�[") > 0 Or InStr(wname, "��") > 0 Then
			sname = "Sword.wav"
		ElseIf InStrNotNest(wclass, "��") Then 
			sname = "Explode(Small).wav"
		ElseIf InStrNotNest(wclass, "�a") Then 
			sname = "BeamCoat.wav"
		Else
			sname = "Explode(Small).wav"
		End If
		
		'�؂蕥�������Đ�
		PlayWave("Saber.wav")
		Sleep(100)
		PlayWave(sname)
		For i = 2 To num
			Sleep(130)
			PlayWave("Saber.wav")
			Sleep(100)
			PlayWave(sname)
		Next 
		
		'�t���O���N���A
		IsWavePlayed = False
	End Sub
	
	'�V�[���h�h�䎞�̓������
	Public Sub ShieldEffect(ByRef u As Unit)
		'�퓬�A�j���񎩓��I���I�v�V����
		If IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			ShowAnimation("�V�[���h�h�䔭��")
			Exit Sub
		End If
		
		'�V�[���h�̃^�C�v������
		With u
			If .IsFeatureAvailable("�G�l���M�[�V�[���h") Then
				ShowAnimation("�r�[���V�[���h����")
			ElseIf .IsFeatureAvailable("���^�V�[���h") Then 
				ShowAnimation("�V�[���h�h�䔭�� 28")
			ElseIf .IsFeatureAvailable("��^�V�[���h") Then 
				ShowAnimation("�V�[���h�h�䔭�� 40")
			Else
				ShowAnimation("�V�[���h�h�䔭��")
			End If
		End With
	End Sub
	
	'�z���E�Z���̓������
	Public Sub AbsorbEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wclass, wname, cname As String
		
		'�E�N���b�N���͓�����ʂ��X�L�b�v
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'�퓬�A�j���I�t�̏ꍇ�͌��ʉ��Đ��̂�
		If Not BattleAnimation Or IsOptionDefined("�퓬�A�j���񎩓��I��") Then
			PlayWave("Charge.wav")
			Exit Sub
		End If
		
		With u.Weapon(w)
			wname = .Nickname
			wclass = .Class_Renamed
		End With
		
		'�`��F������
		cname = SpellColor(wname, wclass)
		If cname = "" Then
			IsBeamWeapon(wname, wclass, cname)
		End If
		
		'�A�j����\��
		ShowAnimation("���q�W������ " & cname)
	End Sub
	
	
	'��ԕω����̓������
	'UPGRADE_NOTE: ctype �� ctype_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Sub CriticalEffect(ByRef ctype_Renamed As String, ByVal w As Short, ByVal ignore_death As Boolean)
		Dim aname, sname As String
		Dim i As Short
		
		If Len(ctype_Renamed) = 0 Then
			ShowAnimation("�f�t�H���g�N���e�B�J��")
		Else
			For i = 1 To LLength(ctype_Renamed)
				aname = LIndex(ctype_Renamed, i) & "�N���e�B�J��"
				
				If aname = "�����N���e�B�J��" And ignore_death Then
					GoTo NextLoop
				End If
				
				If FindNormalLabel("�퓬�A�j��_" & aname) = 0 Then
					GoTo NextLoop
				End If
				
				sname = ""
				
				If aname = "�V���b�N�N���e�B�J��" Then
					If SelectedUnit.IsWeaponClassifiedAs(w, "��") Then
						'��C�ɂ��U���ōs���s�\�ɂȂ����ꍇ�͌��ʉ����I�t
						sname = "-.wav"
					End If
				End If
				
				If sname <> "" Then
					ShowAnimation(aname & " " & sname)
				Else
					ShowAnimation(aname)
				End If
NextLoop: 
			Next 
		End If
	End Sub
	
	
	'���ʉ��̍Đ��񐔂�����
	Private Function CountAttack(ByRef u As Unit, ByVal w As Short, Optional ByVal hit_count As Short = 0) As Short
		'���b�Z�[�W�X�s�[�h���u�������v�Ȃ�J��Ԃ������P�ɐݒ�
		If MessageWait <= 200 Then
			CountAttack = 1
			Exit Function
		End If
		
		'�A���U���̏ꍇ�A���������w�肳�ꂽ�Ȃ炻����ɂ��킹��
		If hit_count > 0 And InStr(u.Weapon(w).Class_Renamed, "�A") > 0 Then
			CountAttack = hit_count
			Exit Function
		End If
		
		CountAttack = MinLng(CountAttack0(u, w), 4)
	End Function
	
	Private Function CountAttack0(ByRef u As Unit, ByVal w As Short) As Short
		Dim wname, wclass As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'�A���U���̏ꍇ�͍U���񐔂ɂ��킹��
		If InStrNotNest(wclass, "�A") > 0 Then
			CountAttack0 = u.WeaponLevel(w, "�A")
			Exit Function
		End If
		
		If InStr(wname, "�A") > 0 Then
			If InStr(wname, "�Q�S�A") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "�Q�Q�A") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "�Q�O�A") > 0 Or InStr(wname, "��\�A") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "�P�W�A") > 0 Or InStr(wname, "�\���A") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "�P�U�A") > 0 Or InStr(wname, "�\�Z�A") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "�P�S�A") > 0 Or InStr(wname, "�\�l�A") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "�P�Q�A") > 0 Or InStr(wname, "�\��A") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "�P�A") > 0 Or InStr(wname, "��A") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "�P�O�A") > 0 Or InStr(wname, "�\�A") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "�X�A") > 0 Or InStr(wname, "��A") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "�W�A") > 0 Or InStr(wname, "���A") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "�V�A") > 0 Or InStr(wname, "���A") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "�U�A") > 0 Or InStr(wname, "�Z�A") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "�T�A") > 0 Or InStr(wname, "�ܘA") > 0 Then
				CountAttack0 = 4
			End If
			If InStr(wname, "�S�A") > 0 Or InStr(wname, "�l�A") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "�R�A") > 0 Or InStr(wname, "�O�A") > 0 Then
				CountAttack0 = 3
				Exit Function
			End If
			If InStr(wname, "�Q�A") > 0 Or InStr(wname, "��A") > 0 Then
				CountAttack0 = 2
				Exit Function
			End If
			
			If InStr(wname, "�A��") > 0 Or InStr(wname, "�A��") > 0 Or InStr(wname, "���A") > 0 Then
				CountAttack0 = 3
				Exit Function
			End If
			
			CountAttack0 = 2
			Exit Function
		End If
		
		If InStr(wname, "�S�e") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�t���t�@�C�A") > 0 Or InStr(wname, "�X�v���b�g") > 0 Or InStr(wname, "�}���`") > 0 Or InStr(wname, "�p������") > 0 Or InStr(wname, "���g") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�S��") > 0 Or InStr(wname, "��{") > 0 Or InStr(wname, "���") > 0 Or InStr(wname, "�t�@���l��") > 0 Or InStr(wname, "�r�b�g") > 0 Then
			CountAttack0 = 4
			Exit Function
		End If
		
		If InStr(wname, "�}�V���K��") > 0 Or InStr(wname, "�@�e") > 0 Or InStr(wname, "�@�֏e") > 0 Or InStr(wname, "�o���J��") > 0 Or InStr(wname, "�K�g�����O") > 0 Or (InStr(wname, "�p���X") > 0 And InStr(wname, "�C���p���X") = 0) Or InStr(wname, "����") > 0 Or InStr(wname, "���P�b�g�����`���[") > 0 Or InStr(wname, "�~�T�C�������`���[") > 0 Or InStr(wname, "�~�T�C���|�b�h") > 0 Then
			CountAttack0 = 4
			Exit Function
		End If
		
		If InStr(wname, "�g���v��") > 0 Or InStr(wname, "�C���R��") > 0 Or InStr(wname, "�t�@�~���A") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "���e") > 0 Or InStr(wname, "����") > 0 Or InStr(wname, "�͍ڋ@") > 0 Then
			CountAttack0 = 3
			Exit Function
		End If
		
		If InStr(wname, "�c�C��") > 0 Or InStr(wname, "�_�u��") > 0 Or InStr(wname, "�f���A��") > 0 Or InStr(wname, "�}�C�N��") > 0 Or InStr(wname, "�o") > 0 Or InStr(wname, "��") > 0 Or InStr(wname, "��") > 0 Then
			CountAttack0 = 2
			Exit Function
		End If
		
		CountAttack0 = 1
	End Function
	
	'�����n�̍U�����ǂ����𔻒肵�A�\���F������
	Private Function IsBeamWeapon(ByRef wname As String, ByVal wclass As String, ByRef cname As String) As Boolean
		If InStrNotNest(wclass, "��") > 0 Then
			'�����n�U���ł͂��蓾�Ȃ�
			Exit Function
		End If
		
		If InStr(wname, "�r�[��") > 0 Or InStrNotNest(wclass, "�a") > 0 Then
			IsBeamWeapon = True
		Else
			If Right(wname, 2) = "�K�X" Then
				Exit Function
			End If
		End If
		
		If InStr(wname, "������") > 0 Or InStr(wname, "�M��") > 0 Or InStr(wname, "�u���X�^�[") > 0 Then
			IsBeamWeapon = True
			cname = "���b�h"
		ElseIf InStr(wname, "�t�F�C�U�[") > 0 Or InStr(wname, "���q") > 0 Then 
			IsBeamWeapon = True
			If InStr(wname, "���K���q") > 0 Then
				cname = "�C�G���["
			Else
				cname = "�s���N"
			End If
		ElseIf InStr(wname, "�Ⓚ") > 0 Or InStr(wname, "���") > 0 Or InStr(wname, "�t���[�U�[") > 0 Then 
			IsBeamWeapon = True
			cname = "�u���["
		ElseIf InStr(wname, "���Ԏq") > 0 Or InStr(wname, "�����q") > 0 Or InStr(wname, "�j���[�g����") > 0 Or InStr(wname, "�j���[�g���m") > 0 Then 
			IsBeamWeapon = True
			cname = "�O���[��"
		ElseIf InStr(wname, "�v���Y�}") > 0 Then 
			IsBeamWeapon = True
			cname = "�I�����W"
		ElseIf InStr(wname, "���[�U�[") > 0 Or InStr(wname, "���q") > 0 Then 
			IsBeamWeapon = True
			cname = "�C�G���["
		ElseIf InStr(wname, "�z�q") > 0 Then 
			IsBeamWeapon = True
			cname = "�z���C�g"
		End If
		
		If cname = "" Then
			If InStr(wname, "���q") > 0 Then
				If InStr(wname, "���K���q") > 0 Then
					cname = "�C�G���["
				Else
					cname = "�s���N"
				End If
			ElseIf InStr(wname, "�C�I��") > 0 Or InStr(wname, "�Ⓚ") > 0 Or InStr(wname, "�d�q") > 0 Then 
				cname = "�u���["
			End If
		End If
		
		If Not IsBeamWeapon And cname <> "" Then
			If Right(wname, 2) = "����" Or Right(wname, 1) = "�C" Or Right(wname, 1) = "�e" Then
				IsBeamWeapon = True
			End If
		End If
	End Function
	
	'���@�̕\���F
	Private Function SpellColor(ByRef wname As String, ByVal wclass As String) As String
		Dim sclass As String
		Dim i As Short
		
		sclass = wname & wclass
		
		'���햼�������Ɋ܂܂�銿�����画��
		For i = 1 To Len(sclass)
			Select Case Mid(sclass, i, 1)
				Case "��", "��", "��", "��", "��", "�M", "�n"
					SpellColor = "��"
					Exit Function
				Case "��", "�C", "��", "�g", "��"
					SpellColor = "��"
					Exit Function
				Case "��", "��", "��", "��", "��", "��", "�t", "��", "��"
					SpellColor = "��"
					Exit Function
				Case "��", "��", "��", "��", "��", "��", "��", "��", "�d", "�e", "�A", "��", "�E"
					SpellColor = "��"
					Exit Function
				Case "�y", "�n", "��", "��", "��", "��", "�R", "�x"
					SpellColor = "��"
					Exit Function
				Case "��", "��", "��", "�U", "��", "��", "��"
					SpellColor = "��"
					Exit Function
				Case "��", "��", "��", "��", "�X", "��", "��", "��", "�~"
					SpellColor = "��"
					Exit Function
				Case "��", "�z"
					SpellColor = "��"
					Exit Function
			End Select
		Next 
		
		'���햼���画��
		If InStr(wname, "�t�@�C���[") > 0 Or InStr(wname, "�t���A") > 0 Or InStr(wname, "�q�[�g") > 0 Or InStr(wname, "�u���b�h") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�E�H�[�^�[") > 0 Or InStr(wname, "�A�N�A") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�E�b�h") > 0 Or InStr(wname, "�t�H���X�g") > 0 Or InStr(wname, "�|�C�Y��") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�C�r��") > 0 Or InStr(wname, "�G�r��") > 0 Or InStr(wname, "�_�[�N") > 0 Or InStr(wname, "�f�X") > 0 Or InStr(wname, "�i�C�g") > 0 Or InStr(wname, "�V���h�E") > 0 Or InStr(wname, "�J�[�X") > 0 Or InStr(wname, "�J�[�Y") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�A�[�X") > 0 Or InStr(wname, "�T���h") > 0 Or InStr(wname, "���b�N") > 0 Or InStr(wname, "�X�g�[��") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "���C�t") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�z�[���[") > 0 Or InStr(wname, "�X�^�[") > 0 Or InStr(wname, "���[��") > 0 Or InStr(wname, "�R�[���h") > 0 Or InStr(wname, "�A�C�X") > 0 Or InStr(wname, "�t���[�Y") > 0 Then
			SpellColor = "��"
			Exit Function
		End If
		
		If InStr(wname, "�T��") Then
			SpellColor = "��"
			Exit Function
		End If
	End Function
	
	
	'�j��A�j���[�V������\������
	Public Sub DieAnimation(ByRef u As Unit)
		Dim i As Short
		Dim PT As POINTAPI
		Dim fname, draw_mode As String
		
		With u
			EraseUnitBitmap(.X, .Y)
			
			'�l�ԃ��j�b�g�łȂ��ꍇ�͔�����\��
			If Not .IsHero Then
				ExplodeAnimation(.Size, .X, .Y)
				Exit Sub
			End If
			
			GetCursorPos(PT)
			
			'���b�Z�[�W�E�C���h�E��Ń}�E�X�{�^�����������ꍇ
			If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
				With frmMessage
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'�E�{�^���Ŕ����X�L�b�v
							Exit Sub
						End If
					End If
				End With
			End If
			
			'���C���E�C���h�E��Ń}�E�X�{�^�����������ꍇ
			If System.Windows.Forms.Form.ActiveForm Is MainForm Then
				With MainForm
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'�E�{�^���Ŕ����X�L�b�v
							Exit Sub
						End If
					End If
				End With
			End If
			
			'�|��鉹
			Select Case .Area
				Case "�n��"
					PlayWave("FallDown.wav")
				Case "��"
					If MessageWait > 0 Then
						PlayWave("Bomb.wav")
						Sleep(500)
					End If
					If TerrainClass(.X, .Y) = "��" Or TerrainClass(.X, .Y) = "�[�C" Then
						PlayWave("Splash.wav")
					Else
						PlayWave("FallDown.wav")
					End If
			End Select
			
			'���j�b�g���ł̃A�j���[�V����
			
			'���b�Z�[�W���E�G�C�g�����Ȃ�A�j���[�V�������X�L�b�v
			If MessageWait = 0 Then
				Exit Sub
			End If
			
			Select Case .Party0
				Case "����", "�m�o�b"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Ally)"
				Case "�G"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Enemy)"
				Case "����"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Neutral)"
			End Select
			If FileExists(ScenarioPath & fname & ".bmp") Then
				fname = ScenarioPath & fname
			Else
				fname = AppPath & fname
			End If
			If Not FileExists(fname & "01.bmp") Then
				Exit Sub
			End If
			
			Select Case MapDrawMode
				Case "��"
					draw_mode = "��"
				Case Else
					draw_mode = MapDrawMode
			End Select
			
			For i = 1 To 6
				DrawPicture(fname & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, draw_mode)
				DrawPicture("Unit\" & .Bitmap, MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "���� " & draw_mode)
				DrawPicture(fname & "0" & VB6.Format(i) & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "���� " & draw_mode)
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
				Sleep(50)
			Next 
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End With
	End Sub
	
	'�����A�j���[�V������\������
	Public Sub ExplodeAnimation(ByRef tsize As String, ByVal tx As Short, ByVal ty As Short)
		Dim i As Short
		Dim PT As POINTAPI
		Static init_explode_animation As Boolean
		Static explode_image_path As String
		Static explode_image_num As Short
		
		'���߂Ď��s����ۂɁA�����p�摜������t�H���_���`�F�b�N
		If Not init_explode_animation Then
			'�����p�摜�̃p�X
			If FileExists(ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then
				explode_image_path = ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			ElseIf FileExists(ScenarioPath & "Bitmap\Event\Explode01.bmp") Then 
				explode_image_path = ScenarioPath & "Bitmap\Event\Explode"
			ElseIf FileExists(AppPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then 
				explode_image_path = AppPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			Else
				explode_image_path = AppPath & "Bitmap\Event\Explode"
			End If
			
			'�����p�摜�̌�
			i = 2
			Do While FileExists(explode_image_path & VB6.Format(i, "00") & ".bmp")
				i = i + 1
			Loop 
			explode_image_num = i - 1
		End If
		
		GetCursorPos(PT)
		
		'���b�Z�[�W�E�C���h�E��Ń}�E�X�{�^�����������ꍇ
		If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
			With frmMessage
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'�E�{�^���Ŕ����X�L�b�v
						Exit Sub
					End If
				End If
			End With
		End If
		
		'���C���E�C���h�E��Ń}�E�X�{�^�����������ꍇ
		If System.Windows.Forms.Form.ActiveForm Is MainForm Then
			With MainForm
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'�E�{�^���Ŕ����X�L�b�v
						Exit Sub
					End If
				End If
			End With
		End If
		
		'������
		Select Case tsize
			Case "XL", "LL"
				PlayWave("Explode(Far).wav")
			Case "L", "M", "S", "SS"
				PlayWave("Explode.wav")
		End Select
		
		'���b�Z�[�W���E�G�C�g�����Ȃ甚�����X�L�b�v
		If MessageWait = 0 Then
			Exit Sub
		End If
		
		'�����̕\��
		If InStr(explode_image_path, "\Anime\") > 0 Then
			'�퓬�A�j���ł̉摜���g�p
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 56, MapToPixelY(ty) - 56, 144, 144, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 40, MapToPixelY(ty) - 40, 112, 112, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 24, MapToPixelY(ty) - 24, 80, 80, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		Else
			'�ėp�C�x���g�摜�ł̉摜���g�p
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 32, MapToPixelY(ty) - 32, 96, 96, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 16, MapToPixelY(ty) - 16, 64, 64, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx), MapToPixelY(ty), 32, 32, 0, 0, 0, 0, "����")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
	End Sub
	
	'�U�����������̓�����ʂƃ��b�Z�[�W��\������
	Public Sub NegateEffect(ByRef u As Unit, ByRef t As Unit, ByVal w As Short, ByRef wname As String, ByVal dmg As Integer, ByRef fname As String, ByRef fdata As String, ByVal ecost As Short, ByRef msg As String, ByVal be_quiet As Boolean)
		Dim defined As Boolean
		
		If LIndex(fdata, 1) = "�a" Or LIndex(fdata, 2) = "�a" Or LIndex(fdata, 3) = "�a" Then
			If Not be_quiet Then
				If t.IsMessageDefined("�r�[��������(" & fname & ")") Then
					t.PilotMessage("�r�[��������(" & fname & ")")
				Else
					t.PilotMessage("�r�[��������")
				End If
			End If
			
			If t.IsAnimationDefined("�r�[��������", fname) Then
				t.PlayAnimation("�r�[��������", fname)
			ElseIf t.IsSpecialEffectDefined("�r�[��������", fname) Then 
				t.SpecialEffect("�r�[��������", fname)
			ElseIf dmg < 0 Then 
				AbsorbEffect(u, w, t)
			ElseIf BattleAnimation Then 
				ShowAnimation("�r�[���R�[�g���� - " & fname)
			ElseIf Not IsWavePlayed Then 
				PlayWave("BeamCoat.wav")
			End If
			
			If u.IsAnimationDefined(wname & "(�U��������)") Then
				u.PlayAnimation(wname & "(�U��������)")
			ElseIf u.IsSpecialEffectDefined(wname & "(�U��������)") Then 
				u.SpecialEffect(wname & "(�U��������)")
			End If
			
			If t.IsSysMessageDefined("�r�[��������", fname) Then
				t.SysMessage("�r�[��������", fname)
			ElseIf fname = "" Then 
				If dmg < 0 Then
					DisplaySysMessage(msg & t.Nickname & "���U�����z�������B")
				Else
					DisplaySysMessage(msg & t.Nickname & "���U����h�����B")
				End If
			Else
				If dmg < 0 Then
					DisplaySysMessage(msg & t.Nickname & "��[" & fname & "]���U�����z�������B")
				Else
					DisplaySysMessage(msg & t.Nickname & "��[" & fname & "]���U����h�����B")
				End If
			End If
		Else
			If Not be_quiet Then
				If t.IsMessageDefined("�U��������(" & fname & ")") Then
					t.PilotMessage("�U��������(" & fname & ")")
				Else
					t.PilotMessage("�U��������")
				End If
			End If
			
			If t.IsAnimationDefined("�U��������", fname) Then
				t.PlayAnimation("�U��������", fname)
				defined = True
			ElseIf t.IsSpecialEffectDefined("�U��������", fname) Then 
				t.SpecialEffect("�U��������", fname)
				defined = True
			ElseIf dmg < 0 Then 
				AbsorbEffect(u, w, t)
				defined = True
			ElseIf BattleAnimation Then 
				If InStr(fdata, "�o���A����������") = 0 Or ecost > 0 Then
					If fname = "�o���A" Then
						ShowAnimation("�o���A����")
					ElseIf fname = "" Then 
						ShowAnimation("�o���A���� - �U��������")
					Else
						ShowAnimation("�o���A���� - " & fname)
					End If
					defined = True
				End If
			End If
			
			If u.IsAnimationDefined(wname & "(�U��������)") Then
				u.PlayAnimation(wname & "(�U��������)")
				defined = True
			ElseIf u.IsSpecialEffectDefined(wname & "(�U��������)") Then 
				u.SpecialEffect(wname & "(�U��������)")
				defined = True
			End If
			
			If Not defined Then
				HitEffect(u, w, t)
			End If
			
			If t.IsSysMessageDefined("�U��������", fname) Then
				t.SysMessage("�U��������", fname)
			ElseIf fname = "" Then 
				If dmg < 0 Then
					DisplaySysMessage(msg & t.Nickname & "�͍U�����z�������B")
				Else
					DisplaySysMessage(msg & t.Nickname & "�͍U����h�����B")
				End If
			Else
				If dmg < 0 Then
					DisplaySysMessage(msg & t.Nickname & "��[" & fname & "]���U�����z�������B")
				Else
					DisplaySysMessage(msg & t.Nickname & "��[" & fname & "]���U����h�����B")
				End If
			End If
		End If
	End Sub
End Module