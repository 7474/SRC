Option Strict Off
Option Explicit On
Friend Class DialogData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'����̃p�C���b�g�Ɏw�肳�ꂽ�S�_�C�A���O�̃N���X
	
	'�p�C���b�g�ꗗ
	Public Name As String
	
	'�_�C�A���O����
	Private intDialogNum As Short
	'�V�`���G�[�V�����ꗗ
	Private strSituation() As String
	'�_�C�A���O�ꗗ
	Private Dialoges() As Dialog
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		intDialogNum = 0
		ReDim strSituation(0)
		ReDim Dialoges(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		ReDim strSituation(0)
		
		For i = 1 To UBound(Dialoges)
			'UPGRADE_NOTE: �I�u�W�F�N�g Dialoges() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			Dialoges(i) = Nothing
		Next 
		'    ReDim Dialoges(0)
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�_�C�A���O��ǉ�
	Public Function AddDialog(ByRef msg_situation As String) As Dialog
		Dim new_dialog As New Dialog
		
		intDialogNum = intDialogNum + 1
		ReDim Preserve strSituation(intDialogNum)
		ReDim Preserve Dialoges(intDialogNum)
		strSituation(intDialogNum) = msg_situation
		Dialoges(intDialogNum) = new_dialog
		AddDialog = new_dialog
	End Function
	
	'�_�C�A���O�̑���
	Public Function CountDialog() As Integer
		CountDialog = intDialogNum
	End Function
	
	'idx�Ԗڂ̃V�`���G�[�V����
	Public Function Situation(ByVal idx As Integer) As String
		Situation = strSituation(idx)
	End Function
	
	'idx�Ԗڂ̃_�C�A���O
	Public Function Dialog(ByVal idx As Integer) As Dialog
		Dialog = Dialoges(idx)
	End Function
	
	'���j�b�g u �̃V�`���G�[�V���� msg_situation �ɂ�����_�C�A���O��I��
	Public Function SelectDialog(ByRef msg_situation As String, ByRef u As Unit, Optional ByVal ignore_condition As Boolean = False) As Dialog
		Dim situations() As String
		Dim sub_situations() As String
		Dim list0() As Short
		Dim list0_num As Short
		Dim tlist() As Short
		Dim tlist_num As Short
		Dim list() As Short
		Dim list_num As Short
		Dim j, i, k As Short
		Dim found As Boolean
		Dim t As Unit
		Dim w, tw As Short
		
		'�z��̈�m��
		ReDim list0(300)
		ReDim tlist(100)
		ReDim list(200)
		
		'�V�`���G�[�V������ݒ�
		Select Case msg_situation
			Case "�i��", "�ˌ�"
				ReDim situations(2)
				situations(2) = "�U��"
			Case "�i��(����)", "�ˌ�(����)"
				ReDim situations(2)
				situations(2) = "�U��(����)"
			Case "�i��(���)", "�ˌ�(���)"
				ReDim situations(2)
				situations(2) = "�U��(���)"
			Case "�i��(�Ƃǂ�)", "�ˌ�(�Ƃǂ�)"
				ReDim situations(2)
				situations(2) = "�U��(�Ƃǂ�)"
			Case "�i��(�N���e�B�J��)", "�ˌ�(�N���e�B�J��)"
				ReDim situations(2)
				situations(2) = "�U��(�N���e�B�J��)"
			Case "�i��(����)", "�ˌ�(����)"
				ReDim situations(2)
				situations(2) = "�U��(����)"
			Case "�i��(����)(����)", "�ˌ�(����)(����)"
				ReDim situations(2)
				situations(2) = "�U��(����)(����)"
			Case "�i��(���)(����)", "�ˌ�(���)(����)"
				ReDim situations(2)
				situations(2) = "�U��(���)(����)"
			Case "�i��(�Ƃǂ�)(����)", "�ˌ�(�Ƃǂ�)(����)"
				ReDim situations(2)
				situations(2) = "�U��(�Ƃǂ�)(����)"
			Case "�i��(�N���e�B�J��)(����)", "�ˌ�(�N���e�B�J��)(����)"
				ReDim situations(2)
				situations(2) = "�U��(�N���e�B�J��)(����)"
			Case Else
				ReDim situations(1)
		End Select
		situations(1) = msg_situation
		
		'���b�Z�[�W�̌�⃊�X�g��ꎟ�R��
		list0_num = 0
		For i = 1 To intDialogNum
			For j = 1 To UBound(situations)
				If Left(strSituation(i), Len(situations(j))) = situations(j) Then
					If Dialoges(i).IsAvailable(u, ignore_condition) Then
						list0_num = list0_num + 1
						If list0_num > UBound(list0) Then
							ReDim Preserve list0(list0_num)
						End If
						list0(list0_num) = i
					End If
					Exit For
				End If
			Next 
		Next 
		If list0_num = 0 Then
			Exit Function
		End If
		
		'�ŏ��ɑ������̃V�`���G�[�V�����݂̂Ō���
		If u Is SelectedUnit Then
			t = SelectedTarget
		ElseIf u Is SelectedTarget Then 
			t = SelectedUnit
		End If
		If t Is Nothing Then
			GoTo SkipMessagesWithTarget
		End If
		
		'������胁�b�Z�[�W�̃��X�g���쐬
		tlist_num = 0
		For i = 1 To list0_num
			If InStr(strSituation(list0(i)), "(��") > 0 Then
				tlist_num = tlist_num + 1
				If tlist_num > UBound(tlist) Then
					ReDim Preserve tlist(tlist_num)
				End If
				tlist(tlist_num) = list0(i)
			End If
		Next 
		If tlist_num = 0 Then
			'������胁�b�Z�[�W���Ȃ�
			GoTo SkipMessagesWithTarget
		End If
		
		'�������g�ɃA�r���e�B���g���ꍇ�͕K���u(�Ύ���)�v��D��
		If t Is u Then
			list_num = 0
			For i = 1 To tlist_num
				For j = 1 To UBound(situations)
					If strSituation(tlist(i)) = situations(j) & "(�Ύ���)" Then
						list_num = list_num + 1
						If list_num > UBound(list) Then
							ReDim Preserve list(list_num)
						End If
						list(list_num) = tlist(i)
						Exit For
					End If
				Next 
			Next 
			If list_num > 0 Then
				SelectDialog = Dialoges(list(Dice(list_num)))
				Exit Function
			End If
		End If
		
		Dim wclass, ch As String
		With t
			If .Status <> "�o��" Then
				GoTo SkipMessagesWithTarget
			End If
			
			ReDim sub_situations(8)
			'�΃p�C���b�g����
			sub_situations(1) = "(��" & .MainPilot.Name & ")"
			'�΃p�C���b�g����
			sub_situations(2) = "(��" & .MainPilot.Nickname & ")"
			'�΃��j�b�g����
			sub_situations(3) = "(��" & .Name & ")"
			'�΃��j�b�g����
			sub_situations(4) = "(��" & .Nickname & ")"
			'�΃��j�b�g�N���X
			sub_situations(5) = "(��" & .Class0 & ")"
			'�΃��j�b�g�T�C�Y
			sub_situations(6) = "(��" & .Size & ")"
			'�Βn�`��
			sub_situations(7) = "(��" & TerrainName(.X, .Y) & ")"
			'�΃G���A
			sub_situations(8) = "(��" & .Area & ")"
			
			'�΃��b�Z�[�W�N���X
			If .IsFeatureAvailable("���b�Z�[�W�N���X") Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(��" & .FeatureData("���b�Z�[�W�N���X") & ")"
			End If
			
			'�ΐ���
			Select Case .MainPilot.Sex
				Case "�j��"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(�Βj��)"
				Case "����"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(�Ώ���)"
			End Select
			
			'�Γ���\��
			With .MainPilot
				For i = 1 To .CountSkill
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(��" & .SkillName0(i) & ")"
					If sub_situations(UBound(sub_situations)) = "(�Δ�\��)" Then
						sub_situations(UBound(sub_situations)) = "(��" & .Skill(i) & ")"
					End If
				Next 
			End With
			For i = 1 To .CountFeature
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(��" & .FeatureName0(i) & ")"
				If sub_situations(UBound(sub_situations)) = "(��)" Then
					sub_situations(UBound(sub_situations)) = "(��" & .Feature(i) & ")"
				End If
			Next 
			
			'�Ύ�_
			If Len(.strWeakness) > 0 Then
				For i = 1 To Len(.strWeakness)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(�Ύ�_=" & Mid(.strWeakness, i, 1) & ")"
				Next 
			End If
			
			'�ΗL��
			If Len(.strEffective) > 0 Then
				For i = 1 To Len(.strEffective)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(�Ύ�_=" & Mid(.strEffective, i, 1) & ")"
				Next 
			End If
			
			'�΃U�R
			If InStr(.MainPilot.Name, "(�U�R)") > 0 And (u.MainPilot.Technique > .MainPilot.Technique Or u.HP > .HP \ 2) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(�΃U�R)"
			End If
			
			'�΋��G
			If .BossRank >= 0 Or (InStr(.MainPilot.Name, "(�U�R)") = 0 And u.MainPilot.Technique <= .MainPilot.Technique) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(�΋��G)"
			End If
			
			'�������g�p���镐����`�F�b�N
			w = 0
			If SelectedUnit Is u Then
				If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
					w = SelectedWeapon
				End If
			ElseIf SelectedTarget Is u Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= u.CountWeapon Then
					w = SelectedTWeapon
				End If
			End If
			
			If w > 0 Then
				'�Εm��
				If .HP <= u.Damage(w, t, u.Party = "����") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(�Εm��)"
				End If
				
				Select Case u.HitProbability(w, t, u.Party = "����")
					Case Is < 50
						'�΍����
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(�΍����)"
					Case Is >= 100
						'�Β���
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(�Β���)"
				End Select
			End If
			
			'���肪�g�p���镐����`�F�b�N
			tw = 0
			If SelectedUnit Is t Then
				If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
					tw = SelectedWeapon
				End If
			ElseIf SelectedTarget Is t Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= .CountWeapon Then
					tw = SelectedTWeapon
				End If
			End If
			
			If tw > 0 Then
				'�Ε��햼
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(��" & .Weapon(tw).Name & ")"
				
				'�Ε��푮��
				wclass = .WeaponClass(tw)
				For i = 1 To Len(wclass)
					ch = Mid(wclass, i, 1)
					Select Case ch
						Case CStr(0) To CStr(127)
						Case Else
							ReDim Preserve sub_situations(UBound(sub_situations) + 1)
							sub_situations(UBound(sub_situations)) = "(��" & ch & "����)"
					End Select
				Next 
				
				Select Case .HitProbability(tw, u, .Party = "����")
					Case Is > 75
						'�΍�������
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(�΍�������)"
					Case Is < 25
						'�Βᖽ����
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(�Βᖽ����)"
				End Select
			End If
		End With
		
		'��`����Ă��鑊����胁�b�Z�[�W�̂����A�󋵂ɍ��������b�Z�[�W�𔲂��o��
		list_num = 0
		For i = 1 To tlist_num
			found = False
			For j = 1 To UBound(situations)
				For k = 1 To UBound(sub_situations)
					If strSituation(tlist(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = tlist(i)
			End If
		Next 
		
		'�󋵂ɍ�����������胁�b�Z�[�W����ł�����΁A���̒����烁�b�Z�[�W��I��
		If list_num > 0 Then
			SelectDialog = Dialoges(list(Dice(list_num)))
			If Dice(2) = 1 Or InStr(msg_situation, "(�Ƃǂ�)") > 0 Or msg_situation = "����" Or msg_situation = "�E��" Or msg_situation = "���f" Or msg_situation = "�Ј�" Or u.Party = t.Party Then
				Exit Function
			End If
		End If
		
SkipMessagesWithTarget: 
		
		'���ɃT�u�V�`���G�[�V�����Ȃ��ƃ��j�b�g����̃T�u�V�`���G�[�V�����Ō���
		If Not u Is Nothing Then
			ReDim sub_situations(3)
			With u
				sub_situations(1) = "(" & .Name & ")"
				sub_situations(2) = "(" & .Nickname0 & ")"
				sub_situations(3) = "(" & .Class0 & ")"
				Select Case msg_situation
					Case "�i��", "�ˌ�", "�i��(����)", "�ˌ�(����)"
						If SelectedUnit Is u Then
							'�������g�p���镐����`�F�b�N
							If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
								ReDim Preserve sub_situations(4)
								sub_situations(4) = "(" & .WeaponNickname(SelectedWeapon) & ")"
							End If
						End If
				End Select
				If .IsFeatureAvailable("���b�Z�[�W�N���X") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(" & .FeatureData("���b�Z�[�W�N���X") & ")"
				End If
			End With
		Else
			ReDim sub_situations(0)
		End If
		
		'��Ō����������b�Z�[�W���X�g�̒�����V�`���G�[�V�����ɍ��������b�Z�[�W�𔲂��o��
		list_num = 0
		For i = 1 To list0_num
			found = False
			For j = 1 To UBound(situations)
				If strSituation(list0(i)) = situations(j) Then
					found = True
					Exit For
				End If
				For k = 1 To UBound(sub_situations)
					If strSituation(list0(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = list0(i)
			End If
		Next 
		
		'�V�`���G�[�V�����ɍ��������b�Z�[�W��������΁A���̒����烁�b�Z�[�W��I��
		If list_num > 0 Then
			SelectDialog = Dialoges(list(Dice(list_num)))
		End If
	End Function
End Class