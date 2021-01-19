Option Strict Off
Option Explicit On
Friend Class SpecialPowerData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�X�y�V�����p���[�f�[�^�̃N���X
	
	'�X�y�V�����p���[��
	Public Name As String
	'�X�y�V�����p���[���̓ǂ݉���
	Public KanaName As String
	'�X�y�V�����p���[���̒Z�k�`
	Public ShortName As String
	'����r�o
	Public SPConsumption As Short
	'�Ώ�
	Public TargetType As String
	'���ʎ���
	Public Duration As String
	'�K�p����
	Public NecessaryCondition As String
	'�A�j��
	Public Animation As String
	
	'���ʖ�
	Private strEffectType() As String
	'���ʃ��x��
	Private dblEffectLevel() As Double
	'���ʃf�[�^
	Private strEffectData() As String
	
	'���
	Public Comment As String
	
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		ReDim strEffectType(0)
		ReDim dblEffectLevel(0)
		ReDim strEffectData(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	
	'�X�y�V�����p���[�Ɍ��ʂ�ǉ�
	Public Sub SetEffect(ByRef elist As String)
		Dim j, i, k As Short
		Dim buf As String
		Dim elevel, etype, edata As String
		
		ReDim strEffectType(ListLength(elist))
		ReDim dblEffectLevel(ListLength(elist))
		ReDim strEffectData(ListLength(elist))
		
		TrimString(elist)
		For i = 1 To ListLength(elist)
			buf = ListIndex(elist, i)
			j = InStr(buf, "Lv")
			k = InStr(buf, "=")
			If j > 0 And (k = 0 Or j < k) Then
				'���x���w��̂������(�f�[�^�w��𔺂����̂��܂�)
				strEffectType(i) = Left(buf, j - 1)
				
				If k > 0 Then
					'�f�[�^�w��𔺂�����
					dblEffectLevel(i) = CDbl(Mid(buf, j + 2, k - (j + 2)))
					
					buf = Mid(buf, k + 1)
					If Left(buf, 1) = """" Then
						buf = Mid(buf, 2, Len(buf) - 2)
					End If
					
					j = InStr(buf, "Lv")
					k = InStr(buf, "=")
					
					If j > 0 And (k = 0 Or j < k) Then
						'�f�[�^�w�蒆�Ƀ��x���w�肪�������
						etype = Left(buf, j - 1)
						If k > 0 Then
							elevel = Mid(buf, j + 2, k - (j + 2))
							edata = Mid(buf, k + 1)
						Else
							elevel = Mid(buf, j + 2)
							edata = ""
						End If
					ElseIf k > 0 Then 
						'�f�[�^�w�蒆�Ƀf�[�^�w�肪�������
						etype = Left(buf, k - 1)
						elevel = ""
						edata = Mid(buf, k + 1)
					Else
						'�f�[�^�w��̂�
						etype = buf
						elevel = ""
						edata = ""
					End If
					
					If Name = "�t��" And elevel = "" Then
						elevel = VB6.Format(DEFAULT_LEVEL)
					End If
					
					strEffectData(i) = Trim(etype & " " & elevel & " " & edata)
				Else
					'�f�[�^�w��𔺂�Ȃ�����
					dblEffectLevel(i) = CDbl(Mid(buf, j + 2))
				End If
			ElseIf k > 0 Then 
				'�f�[�^�w��𔺂�����
				strEffectType(i) = Left(buf, k - 1)
				
				buf = Mid(buf, k + 1)
				If Asc(buf) = 34 Then '"
					buf = Mid(buf, 2, Len(buf) - 2)
				End If
				
				j = InStr(buf, "Lv")
				k = InStr(buf, "=")
				
				If j > 0 Then
					'�f�[�^�w�蒆�Ƀ��x���w�肪�������
					etype = Left(buf, j - 1)
					If k > 0 Then
						elevel = Mid(buf, j + 2, k - (j + 2))
						edata = Mid(buf, k + 1)
					Else
						elevel = Mid(buf, j + 2)
						edata = ""
					End If
				ElseIf k > 0 Then 
					'�f�[�^�w�蒆�Ƀf�[�^�w�肪�������
					etype = Left(buf, k - 1)
					elevel = ""
					edata = Mid(buf, k + 1)
				Else
					'�f�[�^�w��̂�
					etype = buf
					elevel = ""
					edata = ""
				End If
				
				If Name = "�t��" And elevel = "" Then
					elevel = VB6.Format(DEFAULT_LEVEL)
				End If
				
				strEffectData(i) = Trim(etype & " " & elevel & " " & edata)
			Else
				'���ʖ��̂�
				strEffectType(i) = buf
			End If
		Next 
	End Sub
	
	
	'������ʂ̌�
	Public Function CountEffect() As Short
		CountEffect = UBound(strEffectType)
	End Function
	
	'idx�Ԗڂ̓�����ʃ^�C�v
	Public Function EffectType(ByVal idx As Short) As String
		EffectType = strEffectType(idx)
	End Function
	
	'idx�Ԗڂ̓�����ʃ��x��
	Public Function EffectLevel(ByVal idx As Short) As Double
		EffectLevel = dblEffectLevel(idx)
	End Function
	
	'idx�Ԗڂ̓�����ʃf�[�^
	Public Function EffectData(ByVal idx As Short) As String
		EffectData = strEffectData(idx)
	End Function
	
	'������� ename �������Ă��邩
	Public Function IsEffectAvailable(ByRef ename As String) As Object
		Dim i As Short
		
		For i = 1 To CountEffect
			If ename = EffectType(i) Then
				'UPGRADE_WARNING: �I�u�W�F�N�g IsEffectAvailable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				IsEffectAvailable = True
				Exit Function
			End If
			
			If EffectType(i) = "�X�y�V�����p���[" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SPDList.Item(EffectData(i)).IsEffectAvailable(ename) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If SPDList.Item(EffectData(i)).IsEffectAvailable(ename) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g IsEffectAvailable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					IsEffectAvailable = True
					Exit Function
				End If
			End If
		Next 
	End Function
	
	
	'�X�y�V�����p���[�����̎��_�Ŗ��ɗ����ǂ���
	'(�p�C���b�g p ���g�p�����ꍇ)
	Public Function Useful(ByRef p As Pilot) As Boolean
		Dim u As Unit
		Dim i As Short
		
		Select Case TargetType
			Case "����"
				Useful = Effective(p, (p.Unit_Renamed))
				Exit Function
				
			Case "����", "�S����"
				For	Each u In UList
					With u
						'�o�����Ă���H
						If .Status_Renamed <> "�o��" Then
							GoTo NextUnit1
						End If
						
						'�������j�b�g�H
						Select Case p.Party
							Case "����", "�m�o�b"
								If .Party <> "����" And .Party0 <> "����" And .Party <> "�m�o�b" And .Party0 <> "�m�o�b" Then
									GoTo NextUnit1
								End If
							Case Else
								If p.Party <> .Party Then
									GoTo NextUnit1
								End If
						End Select
						
						'���ʂ�����H
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit1: 
				Next u
				
			Case "�j�󖡕�"
				For	Each u In UList
					With u
						'�j�󂳂�Ă���H
						If .Status_Renamed <> "�j��" Then
							GoTo NextUnit2
						End If
						
						'�������j�b�g�H
						If p.Party <> .Party0 Then
							GoTo NextUnit2
						End If
						
						'���ʂ�����H
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit2: 
				Next u
				
			Case "�G", "�S�G"
				For	Each u In UList
					With u
						'�o�����Ă���H
						If .Status_Renamed <> "�o��" Then
							GoTo NextUnit3
						End If
						
						'�G���j�b�g�H
						Select Case p.Party
							Case "����", "�m�o�b"
								If (.Party = "����" And .Party0 = "����") Or (.Party = "�m�o�b" And .Party0 = "�m�o�b") Then
									GoTo NextUnit3
								End If
							Case Else
								If p.Party = .Party Then
									GoTo NextUnit3
								End If
						End Select
						
						'���ʂ�����H
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit3: 
				Next u
				
			Case "�C��", "�S"
				For	Each u In UList
					'�o�����Ă���H
					If u.Status_Renamed = "�o��" Then
						'���ʂ�����H
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End If
				Next u
		End Select
	End Function
	
	'�X�y�V�����p���[�����j�b�g t �ɑ΂��Č��ʂ����邩�ǂ���
	'(�p�C���b�g p ���g�p�����ꍇ)
	Public Function Effective(ByRef p As Pilot, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim ncond As String
		Dim my_unit As Unit
		
		'�����ǉ��p�C���b�g�������j�b�g����������ꍇ�A�p�C���b�g��Unit��
		'�ω����Ă��܂����Ƃ����邽�߁A����Unit���L�^���Ă���
		my_unit = p.Unit_Renamed
		
		With t
			'�K�p�����𖞂����Ă���H
			For i = 1 To LLength(NecessaryCondition)
				ncond = LIndex(NecessaryCondition, i)
				Select Case ncond
					Case "�Z��"
						If p.Technique < .MainPilot.Technique Then
							GoTo ExitFunc
						End If
					Case "��{�X"
						If .BossRank >= 0 Then
							GoTo ExitFunc
						End If
					Case "�x��"
						If my_unit Is t Then
							GoTo ExitFunc
						End If
					Case "�א�"
						With my_unit
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) <> 1 Then
								GoTo ExitFunc
							End If
						End With
					Case Else
						If InStr(ncond, "�˒�Lv") = 1 Then
							With my_unit
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > StrToLng(Mid(ncond, 5)) Then
									GoTo ExitFunc
								End If
							End With
						End If
				End Select
				
				'Unit���ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
				If Not my_unit Is p.Unit_Renamed Then
					my_unit.MainPilot()
				End If
			Next 
			
			'����������Ă���H
			Select Case TargetType
				Case "�G", "�S�G", "�C��", "�S"
					If .IsConditionSatisfied("�X�y�V�����p���[������") Or .IsConditionSatisfied("���_�R�}���h������") Then
						GoTo ExitFunc
					End If
			End Select
			
			'�������ʂ�������͓̂����X�y�V�����p���[�����ɓK�p����Ă��Ȃ����
			'���ʂ�����Ƃ݂Ȃ�
			If Duration <> "����" Then
				If Not .IsSpecialPowerInEffect(Name) Then
					Effective = True
				End If
				
				'�������݂����͎������g�ɂ͎g���Ȃ��̂Ń`�F�b�N���Ă���
				If EffectType(1) = "�݂����" Then
					If my_unit Is t Then
						Effective = False
						GoTo ExitFunc
					End If
				End If
				
				GoTo ExitFunc
			End If
			
			'�X�̌��ʂɊւ��ėL�����ǂ�������
			For i = 1 To CountEffect
				Select Case EffectType(i)
					Case "�g�o��", "�g�o����"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .IsConditionSatisfied("�]���r") Then
							GoTo NextEffect
						End If
						If .HP < .MaxHP Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�d�m��", "�d�m����"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .IsConditionSatisfied("�]���r") Then
							GoTo NextEffect
						End If
						If .EN < .MaxEN Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "��͉�", "��͑���"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .IsConditionSatisfied("�]���r") Then
							GoTo NextEffect
						End If
						If .MainPilot.Plana < .MainPilot.MaxPlana Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�r�o��"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .IsConditionSatisfied("�]���r") Then
							GoTo NextEffect
						End If
						If .MainPilot.SP < .MainPilot.MaxSP Then
							Effective = True
							GoTo ExitFunc
						End If
						For j = 2 To .CountPilot
							If .Pilot(j).SP < .Pilot(j).MaxSP Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
						For j = 1 To .CountSupport
							If .Support(j).SP < .Support(j).MaxSP Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
						If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
							If .AdditionalSupport.SP < .AdditionalSupport.MaxSP Then
								Effective = True
								GoTo ExitFunc
							End If
						End If
					Case "��ԉ�"
						If .ConditionLifetime("�U���s�\") > 0 Or .ConditionLifetime("�ړ��s�\") > 0 Or .ConditionLifetime("���b��") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("�߈�") > 0 Or .ConditionLifetime("�Ή�") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("���") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("��") > 0 Or .ConditionLifetime("�Ӗ�") > 0 Or .ConditionLifetime("�h��") > 0 Or .ConditionLifetime("���|") > 0 Or .ConditionLifetime("����") > 0 Or .ConditionLifetime("�]���r") > 0 Or .ConditionLifetime("�񕜕s�\") > 0 Or .ConditionLifetime("�I�[���g�p�s�\") > 0 Or .ConditionLifetime("���\�͎g�p�s�\") > 0 Or .ConditionLifetime("�������g�p�s�\") > 0 Or .ConditionLifetime("�����o�g�p�s�\") > 0 Or .ConditionLifetime("�m�o�����g�p�s�\") > 0 Or .ConditionLifetime("��͎g�p�s�\") > 0 Or .ConditionLifetime("�p�g�p�s�\") > 0 Or .ConditionLifetime("�Z�g�p�s�\") > 0 Then
							Effective = True
							GoTo ExitFunc
						Else
							For j = 1 To .CountCondition
								If Len(.Condition(j)) > 6 Then
									If Right(.Condition(j), 6) = "�����g�p�s�\" Then
										If .ConditionLifetime(.Condition(j)) > 0 Then
											Effective = True
											GoTo ExitFunc
										End If
									End If
								End If
							Next 
						End If
					Case "���U"
						For j = 1 To .CountWeapon
							If .Bullet(j) < .MaxBullet(j) Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
					Case "�s������"
						If .Action = 0 And .MaxAction > 0 Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�s��������"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .Action < 3 And .MaxAction > 0 Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�X�y�V�����p���[", "���_�R�}���h"
						If Not .IsSpecialPowerInEffect(EffectData(i)) Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�C�͑���"
						If .MainPilot.Personality <> "�@�B" And .MainPilot.Morale < .MainPilot.MaxMorale Then
							Effective = True
							GoTo ExitFunc
						End If
						For j = 2 To .CountPilot
							If .Pilot(j).Personality <> "�@�B" And .Pilot(j).Morale < .MainPilot.MaxMorale Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
					Case "�C�͒ቺ"
						If .MainPilot.Personality = "�@�B" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale > .MainPilot.MinMorale Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�����_���_���[�W", "�g�o����", "�d�m����"
						If Not .IsConditionSatisfied("���G") Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "�C�͑���", "����", "����", "��@", "�����X�y�V�����p���[���s", "�C�x���g"
						Effective = True
						GoTo ExitFunc
				End Select
NextEffect: 
			Next 
		End With
		
ExitFunc: 
		
		'Unit���ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
		If Not my_unit Is p.Unit_Renamed Then
			my_unit.MainPilot()
		End If
	End Function
	
	
	'�X�y�V�����p���[���g�p����
	'(�p�C���b�g p ���g�p�����ꍇ)
	Public Sub Execute(ByRef p As Pilot, Optional ByVal is_event As Boolean = False)
		Dim u As Unit
		Dim i, j As Short
		
		Select Case TargetType
			Case "����"
				If Apply(p, p.Unit_Renamed, is_event) And Not is_event Then
					Sleep(300)
				End If
				
			Case "�S����"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If u Is Nothing Then
							GoTo NextUnit1
						End If
						With u
							'�������j�b�g�H
							Select Case p.Party
								Case "����", "�m�o�b"
									If .Party <> "����" And .Party0 <> "����" And .Party <> "�m�o�b" And .Party0 <> "�m�o�b" Then
										GoTo NextUnit1
									End If
								Case Else
									If p.Party <> .Party Then
										GoTo NextUnit1
									End If
							End Select
							
							Apply(p, u, is_event)
						End With
NextUnit1: 
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case "�S�G"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If u Is Nothing Then
							GoTo NextUnit2
						End If
						With u
							'�G���j�b�g�H
							Select Case p.Party
								Case "����", "�m�o�b"
									If .Party = "����" Or .Party = "�m�o�b" Then
										GoTo NextUnit2
									End If
								Case Else
									If p.Party = .Party Then
										GoTo NextUnit2
									End If
							End Select
							
							Apply(p, u, is_event)
						End With
NextUnit2: 
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case "�S"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If Not u Is Nothing Then
							Apply(p, u, is_event)
						End If
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case Else
				If Apply(p, SelectedTarget, is_event) And Not is_event Then
					Sleep(300)
				End If
		End Select
		
		If Not is_event Then
			CloseMessageForm()
			RedrawScreen()
		End If
	End Sub
	
	'�X�y�V�����p���[�����j�b�g t �ɑ΂��ēK�p
	'(�p�C���b�g p ���g�p)
	'���s��ɃE�F�C�g���K�v���ǂ�����Ԃ�
	Public Function Apply(ByRef p As Pilot, ByVal t As Unit, Optional ByVal is_event As Boolean = False, Optional ByVal as_instant As Boolean = False) As Boolean
		Dim j, i, n As Short
		Dim tmp As Integer
		Dim need_update, is_invalid, displayed_string As Boolean
		Dim msg, ncond As String
		Dim my_unit As Unit
		
		'�����ǉ��p�C���b�g�������j�b�g����������ꍇ�A�p�C���b�g��Unit��
		'�ω����Ă��܂����Ƃ����邽�߁A����Unit���L�^���Ă���
		my_unit = p.Unit_Renamed
		
		With t
			'�K�p�����𖞂����Ă���H
			For i = 1 To LLength(NecessaryCondition)
				ncond = LIndex(NecessaryCondition, i)
				Select Case ncond
					Case "�Z��"
						If p.Technique < .MainPilot.Technique Then
							is_invalid = True
						End If
					Case "��{�X"
						If .BossRank >= 0 Then
							is_invalid = True
						End If
					Case "�x��"
						If my_unit Is t Then
							is_invalid = True
						End If
					Case "�א�"
						With my_unit
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) <> 1 Then
								is_invalid = True
							End If
						End With
					Case Else
						If InStr(ncond, "�˒�Lv") = 1 Then
							With my_unit
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > StrToLng(Mid(ncond, 5)) Then
									is_invalid = True
								End If
							End With
						End If
				End Select
				
				'Unit���ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
				If Not my_unit Is p.Unit_Renamed Then
					my_unit.CurrentForm.MainPilot()
				End If
			Next 
			
			'����������Ă���H
			Select Case TargetType
				Case "�G", "�S�G"
					If .IsConditionSatisfied("�X�y�V�����p���[����") Then
						is_invalid = True
					End If
			End Select
			
			'�X�y�V�����p���[���K�p�\�H
			If is_invalid Then
				Exit Function
			End If
			
			'�������ʂ�����ꍇ�͒P�ɃX�y�V�����p���[�̌��ʂ�t�����邾���ł悢
			If Duration <> "����" And Not as_instant Then
				.MakeSpecialPowerInEffect(Name, my_unit.MainPilot.ID)
				Exit Function
			End If
		End With
		
		'����ȍ~�͎������ʂ������ł���X�y�V�����p���[�̏���
		
		'�X�̌��ʂ�K�p
		For i = 1 To CountEffect
			With t
				Select Case EffectType(i)
					Case "�g�o��", "�g�o����"
						'���ʂ��K�p�\���ǂ�������
						If EffectLevel(i) > 0 Then
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextEffect
							End If
							If .HP = .MaxHP Then
								GoTo NextEffect
							End If
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'�g�o���񕜂�����
						tmp = .HP
						If EffectType(i) = "�g�o����" Then
							.HP = .HP + 1000 * EffectLevel(i)
						Else
							.RecoverHP(10 * EffectLevel(i))
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.HP - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.HP - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��" & VB6.Format(.HP - tmp) & "�񕜂����B")
							Else
								DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��" & VB6.Format(tmp - .HP) & "���������B")
							End If
						End If
						
						need_update = True
						
					Case "�d�m��", "�d�m����"
						'���ʂ��K�p�\���ǂ�������
						If EffectLevel(i) > 0 Then
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextEffect
							End If
							If .EN = .MaxEN Then
								GoTo NextEffect
							End If
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'�d�m���񕜂�����
						tmp = .EN
						If EffectType(i) = "�d�m����" Then
							.EN = .EN + 10 * EffectLevel(i)
						Else
							.RecoverEN(10 * EffectLevel(i))
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.EN - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.EN - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��" & VB6.Format(.EN - tmp) & "�񕜂����B")
							Else
								DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��" & VB6.Format(tmp - .EN) & "���������B")
							End If
						End If
						
						need_update = True
						
					Case "��͉�", "��͑���"
						'���ʂ��K�p�\���ǂ�������
						If EffectLevel(i) > 0 Then
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextEffect
							End If
							If .MainPilot.Plana = .MainPilot.MaxPlana Then
								GoTo NextEffect
							End If
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'��͂��񕜂�����
						With .MainPilot
							tmp = .Plana
							If EffectType(i) = "��͑���" Then
								.Plana = .Plana + 10 * EffectLevel(i)
							Else
								.Plana = .Plana + .MaxPlana * EffectLevel(i) \ 10
							End If
						End With
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.Plana - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.MainPilot.Plana - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "��" & .MainPilot.SkillName0("���") & "��" & VB6.Format(.MainPilot.Plana - tmp) & "�񕜂����B")
							Else
								DisplaySysMessage(.Nickname & "��" & .MainPilot.SkillName0("���") & "��" & VB6.Format(tmp - .MainPilot.Plana) & "���������B")
							End If
						End If
						
						need_update = True
						
					Case "�r�o��"
						'���ʂ��K�p�\���ǂ�������
						If EffectLevel(i) > 0 Then
							If .IsConditionSatisfied("�]���r") Then
								GoTo NextEffect
							End If
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'�񕜑ΏۂƂȂ�p�C���b�g�����Z�o
						n = .CountPilot + .CountSupport
						If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
							n = n + 1
						End If
						
						'�r�o����
						If n = 1 Then
							'���C���p�C���b�g�݂̂̂r�o����
							tmp = .MainPilot.SP
							.MainPilot.SP = .MainPilot.SP + 10 * EffectLevel(i)
							
							If Not is_event Then
								If Not displayed_string Then
									If EffectLevel(i) >= 0 Then
										DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.SP - tmp))
									Else
										DrawSysString(.X, .Y, VB6.Format(.MainPilot.SP - tmp))
									End If
								End If
								displayed_string = True
								
								If EffectLevel(i) >= 0 Then
									DisplaySysMessage(.MainPilot.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.MainPilot.SP - tmp) & "�񕜂����B")
								Else
									DisplaySysMessage(.MainPilot.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(tmp - .MainPilot.SP) & "���������B")
								End If
							End If
						Else
							'���C���p�C���b�g�̂r�o����
							tmp = .MainPilot.SP
							.MainPilot.SP = .MainPilot.SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
							
							If Not is_event Then
								If Not displayed_string Then
									If EffectLevel(i) >= 0 Then
										DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.SP - tmp))
									Else
										DrawSysString(.X, .Y, VB6.Format(.MainPilot.SP - tmp))
									End If
								End If
								displayed_string = True
								
								If EffectLevel(i) >= 0 Then
									DisplaySysMessage(.MainPilot.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.MainPilot.SP - tmp) & "�񕜂����B")
								Else
									DisplaySysMessage(.MainPilot.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(tmp - .MainPilot.SP) & "���������B")
								End If
							End If
							
							'�T�u�p�C���b�g�̂r�o����
							For j = 2 To .CountPilot
								With .Pilot(j)
									tmp = .SP
									.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
									If Not is_event Then
										If .SP <> tmp Then
											If EffectLevel(i) >= 0 Then
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - tmp) & "�񕜂����B")
											Else
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(tmp - .SP) & "���������B")
											End If
										End If
									End If
								End With
							Next 
							
							'�T�|�[�g�p�C���b�g�̂r�o����
							For j = 1 To .CountSupport
								With .Support(j)
									tmp = .SP
									.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
									If Not is_event Then
										If .SP <> tmp Then
											If EffectLevel(i) >= 0 Then
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - tmp) & "�񕜂����B")
											Else
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(tmp - .SP) & "���������B")
											End If
										End If
									End If
								End With
							Next 
							
							'�ǉ��T�|�[�g�p�C���b�g�̂r�o����
							If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
								With .AdditionalSupport
									tmp = .SP
									.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
									If Not is_event Then
										If .SP <> tmp Then
											If EffectLevel(i) >= 0 Then
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(.SP - tmp) & "�񕜂����B")
											Else
												DisplaySysMessage(.Nickname & "��" & Term("�r�o", t) & "��" & VB6.Format(tmp - .SP) & "���������B")
											End If
										End If
									End If
								End With
							End If
						End If
						
						If Not is_event Then
							If TargetType = "�S����" Then
								Sleep(150)
							End If
						End If
						
					Case "���U"
						'���ʂ��K�p�\���ǂ�������
						For j = 1 To .CountWeapon
							If .Bullet(j) < .MaxBullet(j) Then
								Exit For
							End If
						Next 
						If j > .CountWeapon Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'�e���⋋
						.BulletSupply()
						
						If Not is_event Then
							DisplaySysMessage(.Nickname & "�̒e�����S�������B")
						End If
						
					Case "��ԉ�"
						If .ConditionLifetime("�U���s�\") <= 0 And .ConditionLifetime("�ړ��s�\") <= 0 And .ConditionLifetime("���b��") <= 0 And .ConditionLifetime("����") <= 0 And .ConditionLifetime("����") <= 0 And .ConditionLifetime("�߈�") <= 0 And .ConditionLifetime("�Ή�") <= 0 And .ConditionLifetime("����") <= 0 And .ConditionLifetime("���") <= 0 And .ConditionLifetime("����") <= 0 And .ConditionLifetime("��") <= 0 And .ConditionLifetime("�Ӗ�") <= 0 And .ConditionLifetime("�h��") <= 0 And .ConditionLifetime("���|") <= 0 And .ConditionLifetime("����") <= 0 And .ConditionLifetime("�]���r") <= 0 And .ConditionLifetime("�񕜕s�\") <= 0 And .ConditionLifetime("�I�[���g�p�s�\") <= 0 And .ConditionLifetime("���\�͎g�p�s�\") <= 0 And .ConditionLifetime("�������g�p�s�\") <= 0 And .ConditionLifetime("�����o�g�p�s�\") <= 0 And .ConditionLifetime("�m�o�����g�p�s�\") <= 0 And .ConditionLifetime("��͎g�p�s�\") <= 0 And .ConditionLifetime("�p�g�p�s�\") <= 0 And .ConditionLifetime("�Z�g�p�s�\") <= 0 Then
							For j = 1 To .CountCondition
								If Len(.Condition(j)) > 6 Then
									'��A�������͏�ԉ񕜂��珜�O�B
									If Right(.Condition(j), 6) = "�����g�p�s�\" Then
										If .ConditionLifetime(.Condition(j)) > 0 Then
											Exit For
										End If
									End If
								End If
							Next 
							If (j > .CountCondition) Then
								GoTo NextEffect
							End If
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'�S�ẴX�e�[�^�X�ُ����
						If .ConditionLifetime("�U���s�\") > 0 Then
							.DeleteCondition("�U���s�\")
						End If
						If .ConditionLifetime("�ړ��s�\") > 0 Then
							.DeleteCondition("�ړ��s�\")
						End If
						If .ConditionLifetime("���b��") > 0 Then
							.DeleteCondition("���b��")
						End If
						If .ConditionLifetime("����") > 0 Then
							.DeleteCondition("����")
						End If
						If .ConditionLifetime("����") > 0 Then
							.DeleteCondition("����")
						End If
						If .ConditionLifetime("�߈�") > 0 Then
							.DeleteCondition("�߈�")
						End If
						If .ConditionLifetime("�Ή�") > 0 Then
							.DeleteCondition("�Ή�")
						End If
						If .ConditionLifetime("����") > 0 Then
							.DeleteCondition("����")
						End If
						If .ConditionLifetime("���") > 0 Then
							.DeleteCondition("���")
						End If
						If .ConditionLifetime("����") > 0 Then
							.DeleteCondition("����")
						End If
						If .ConditionLifetime("��") > 0 Then
							.DeleteCondition("��")
						End If
						If .ConditionLifetime("�Ӗ�") > 0 Then
							.DeleteCondition("�Ӗ�")
						End If
						If .ConditionLifetime("�h��") > 0 Then
							.DeleteCondition("�h��")
						End If
						If .ConditionLifetime("���|") > 0 Then
							.DeleteCondition("���|")
						End If
						If .ConditionLifetime("����") > 0 Then
							.DeleteCondition("����")
						End If
						If .ConditionLifetime("�]���r") > 0 Then
							.DeleteCondition("�]���r")
						End If
						If .ConditionLifetime("�񕜕s�\") > 0 Then
							.DeleteCondition("�񕜕s�\")
						End If
						
						If .ConditionLifetime("�I�[���g�p�s�\") > 0 Then
							.DeleteCondition("�I�[���g�p�s�\")
						End If
						If .ConditionLifetime("���\�͎g�p�s�\") > 0 Then
							.DeleteCondition("���\�͎g�p�s�\")
						End If
						If .ConditionLifetime("�������g�p�s�\") > 0 Then
							.DeleteCondition("�������g�p�s�\")
						End If
						If .ConditionLifetime("�����o�g�p�s�\") > 0 Then
							.DeleteCondition("�����o�g�p�s�\")
						End If
						If .ConditionLifetime("�m�o�����g�p�s�\") > 0 Then
							.DeleteCondition("�m�o�����g�p�s�\")
						End If
						If .ConditionLifetime("��͎g�p�s�\") > 0 Then
							.DeleteCondition("��͎g�p�s�\")
						End If
						If .ConditionLifetime("�p�g�p�s�\") > 0 Then
							.DeleteCondition("�p�g�p�s�\")
						End If
						If .ConditionLifetime("�Z�g�p�s�\") > 0 Then
							.DeleteCondition("�Z�g�p�s�\")
						End If
						For j = 1 To .CountCondition
							If Len(.Condition(j)) > 6 Then
								'��A�������͏�ԉ񕜂��珜�O�B
								If Right(.Condition(j), 6) = "�����g�p�s�\" Then
									If .ConditionLifetime(.Condition(j)) > 0 Then
										.DeleteCondition(.Condition(j))
									End If
								End If
							End If
						Next 
						
						If Not is_event Then
							DisplaySysMessage(.Nickname & "�̏�Ԃ��񕜂����B")
						End If
						
					Case "�s������"
						'���ʂ��K�p�\���ǂ�������
						If .Action > 0 Or .MaxAction = 0 Then
							GoTo NextEffect
						End If
						
						'�s�������񕜂�����
						.UsedAction = .UsedAction - 1
						
						'���̌��ʂ̕\���̂��߂Ƀ��b�Z�[�W�E�B���h�E���\������Ă���̂�
						'�Ȃ���Γ��Ƀ��b�Z�[�W�͕\�����Ȃ� (���ʂ͌���Ε�����̂�)
						If Not is_event Then
							If frmMessage.Visible Then
								If t Is SelectedUnit Then
									UpdateMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
								
								DisplaySysMessage(.Nickname & "�͍s���\�ɂȂ����B")
							End If
						End If
						
					Case "�s��������"
						'���ʂ��K�p�\���ǂ�������
						If .Action > 3 Or .MaxAction = 0 Then
							GoTo NextEffect
						End If
						
						'�s�����𑝂₷
						.UsedAction = .UsedAction - 1
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							
							DisplaySysMessage(.Nickname & "��" & StrConv(VB6.Format(.Action), VbStrConv.Wide) & "��s���\�ɂȂ����B")
						End If
						
					Case "�X�y�V�����p���[", "���_�R�}���h"
						If SPDList.IsDefined(EffectData(i)) Then
							.MakeSpecialPowerInEffect(EffectData(i), my_unit.MainPilot.ID)
						Else
							ErrorMessage("�X�y�V�����p���[�u" & Name & "�v�Ŏg���Ă���X�y�V�����p���[�u" & EffectData(i) & "�v�͒�`����Ă��܂���B")
						End If
						
					Case "�C�͑���"
						If .MainPilot.Personality = "�@�B" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale = .MainPilot.MaxMorale Then
							GoTo NextEffect
						End If
						
						'�C�͂𑝉�������
						tmp = .MainPilot.Morale
						.IncreaseMorale(10 * EffectLevel(i))
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.Morale - tmp))
							End If
							displayed_string = True
						End If
						
						need_update = True
						
					Case "�C�͒ቺ"
						'���ʂ��K�p�\���ǂ�������
						If .MainPilot.Personality = "�@�B" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale = .MainPilot.MinMorale Then
							GoTo NextEffect
						End If
						
						'�C�͂�ቺ������
						tmp = .MainPilot.Morale
						.IncreaseMorale(-10 * EffectLevel(i))
						
						If Not is_event Then
							If TargetType = "�G" Or TargetType = "�S�G" Then
								If Not displayed_string Then
									DrawSysString(.X, .Y, VB6.Format(.MainPilot.Morale - tmp))
									displayed_string = True
								End If
							End If
						End If
						
						need_update = True
						
					Case "�����_���_���[�W"
						'���ʂ��K�p�\���ǂ�������
						If .IsConditionSatisfied("���G") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'�_���[�W��^����
						tmp = .HP
						.HP = MaxLng(.HP - 10 * Dice(10 * EffectLevel(i)), 10)
						If TargetType = "�S�G" Then
							Sleep(150)
						End If
						
						'����\�́u�s����v�ɂ��\���`�F�b�N
						If .IsFeatureAvailable("�s����") Then
							If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("�\��") Then
								.AddCondition("�\��", -1)
							End If
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, VB6.Format(tmp - .HP))
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							DisplaySysMessage(.Nickname & "��" & VB6.Format(tmp - .HP) & "�̃_���[�W��^�����B")
						End If
						
						need_update = True
						
					Case "�g�o����"
						'���ʂ��K�p�\���ǂ�������
						If .IsConditionSatisfied("���G") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'�g�o������������
						tmp = .HP
						.HP = .HP - .HP * EffectLevel(i) \ 10
						If TargetType = "�S�G" Then
							Sleep(150)
						End If
						
						'����\�́u�s����v�ɂ��\���`�F�b�N
						If .IsFeatureAvailable("�s����") Then
							If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("�\��") Then
								.AddCondition("�\��", -1)
							End If
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, VB6.Format(tmp - .HP))
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If SelectedUnit Is t Then
								DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��" & VB6.Format(tmp - .HP) & "���������B")
							Else
								DisplaySysMessage(.Nickname & "��" & Term("�g�o", t) & "��" & VB6.Format(tmp - .HP) & "�����������B")
							End If
						End If
						
						need_update = True
						
					Case "�d�m����"
						'���ʂ��K�p�\���ǂ�������
						If .IsConditionSatisfied("���G") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'�d�m������������
						tmp = .EN
						.EN = .EN - .EN * EffectLevel(i) \ 10
						If TargetType = "�S�G" Then
							Sleep(150)
						End If
						
						If Not displayed_string Then
							DrawSysString(.X, .Y, VB6.Format(tmp - .EN))
						End If
						displayed_string = True
						
						If t Is SelectedUnit Then
							UpdateMessageForm(SelectedUnit)
						Else
							UpdateMessageForm(t, SelectedUnit)
						End If
						
						If SelectedUnit Is t Then
							DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��" & VB6.Format(tmp - .EN) & "���������B")
						Else
							DisplaySysMessage(.Nickname & "��" & Term("�d�m", t) & "��" & VB6.Format(tmp - .EN) & "�����������B")
						End If
						
						need_update = True
						
					Case "��@"
						'�����ʂ̃��j�b�g�͎��ʂ��Ă���
						If IsOptionDefined("���j�b�g���B��") Then
							If Not .IsConditionSatisfied("���ʍς�") Then
								.AddCondition("���ʍς�", -1, 0, "��\��")
								DisplayUnitStatus(t)
							End If
						End If
						If .IsConditionSatisfied("���j�b�g���B��") Then
							.DeleteCondition("���j�b�g���B��")
							DisplayUnitStatus(t)
						End If
						
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						DisplayMessage("�V�X�e��", Term("�g�o", t, 6) & "�F" & VB6.Format(.HP) & "/" & VB6.Format(.MaxHP) & ";" & Term("�d�m", t, 6) & "�F" & VB6.Format(.EN) & "/" & VB6.Format(.MaxEN) & ";" & Term("����", t, 6) & "�F" & VB6.Format(.Value \ 2) & ";" & "�o���l�F" & VB6.Format(.ExpValue + .MainPilot.ExpValue))
						If .IsFeatureAvailable("�A�C�e�����L") Then
							If IDList.IsDefined(.FeatureData("�A�C�e�����L")) Then
								msg = IDList.Item(.FeatureData("�A�C�e�����L")).Nickname & "�𓐂ނ��Ƃ��o����B;"
							Else
								ErrorMessage(.Name & "�̏��L�A�C�e���u" & .FeatureData("�A�C�e�����L") & "�v�̃f�[�^��������܂���")
							End If
						End If
						If .IsFeatureAvailable("���A�A�C�e�����L") Then
							If IDList.IsDefined(.FeatureData("���A�A�C�e�����L")) Then
								If Len(msg) > 0 Then
									msg = msg & "�܂��A"
								End If
								msg = msg & "�܂��" & IDList.Item(.FeatureData("���A�A�C�e�����L")).Nickname & "�𓐂ނ��Ƃ��o����B;"
							Else
								ErrorMessage(.Name & "�̏��L���A�A�C�e���u" & .FeatureData("���A�A�C�e�����L") & "�v�̃f�[�^��������܂���")
							End If
						End If
						If .IsFeatureAvailable("���[�j���O�\�Z") Then
							msg = msg & "�u" & .FeatureData("���[�j���O�\�Z") & "�v�����[�j���O�\�B"
						End If
						If Len(msg) > 0 Then
							DisplayMessage("�V�X�e��", msg)
						End If
						
					Case "����"
						OpenMessageForm(t)
						.SuicidalExplosion()
						Exit Function
						
					Case "����"
						If Duration = "�j��" Then
							'�j�󒼌�ɕ�������ꍇ
							.HP = .MaxHP
						Else
							'�j���ɑ��̃p�C���b�g�̗͂ŕ�������ꍇ
							
							'�������͒ʏ�`�Ԃɖ߂�
							If .IsFeatureAvailable("�m�[�}�����[�h") Then
								.Transform(LIndex(.FeatureData("�m�[�}�����[�h"), 1))
								t = .CurrentForm
								n = 0
							Else
								n = .ConditionLifetime("�c�莞��")
								
								'���Rest�Ŏc�莞�Ԃ�0�ɂȂ�Ȃ��悤�Ɉ�U���Ԃ������߂�
								If n > 0 Then
									.AddCondition("�c�莞��", 10)
								End If
							End If
							
							'���j�b�g�𕜊�������
							With t
								.FullRecover()
								.UsedAction = 0
								.StandBy(my_unit.X, my_unit.Y)
								.Rest()
								
								'�c�莞�Ԃ����ɖ߂�
								If n > 0 Then
									.DeleteCondition("�c�莞��")
									.AddCondition("�c�莞��", n)
								End If
								
								RedrawScreen()
							End With
						End If
						
						With t
							If Not frmMessage.Visible Then
								OpenMessageForm()
							End If
							If .IsMessageDefined("����") Then
								.PilotMessage("����")
							End If
							If .IsAnimationDefined("����") Then
								.PlayAnimation("����")
							Else
								.SpecialEffect("����")
							End If
							DisplaySysMessage(.Nickname & "�͕��������B")
						End With
						
					Case "�C�x���g"
						'�C�x���g�R�}���h�Œ�`���ꂽ�X�y�V�����p���[
						'�Ώۃ��j�b�g�h�c�y�ё��胆�j�b�g�h�c��ݒ�
						SelectedUnitForEvent = my_unit.CurrentForm
						SelectedTargetForEvent = .CurrentForm
						'�w�肳�ꂽ�T�u���[�`�������s
						GetValueAsString("Call(" & EffectData(i) & ")")
				End Select
			End With
NextEffect: 
		Next 
		
		'Unit���ω����Ă��܂����ꍇ�͌��ɖ߂��Ă���
		If Not my_unit Is p.Unit_Renamed Then
			my_unit.CurrentForm.MainPilot()
		End If
		
		'�X�e�[�^�X�̍X�V���K�v�H
		If need_update Then
			With t
				.CheckAutoHyperMode()
				.CurrentForm.CheckAutoNormalMode()
				.CurrentForm.Update()
				PList.UpdateSupportMod(.CurrentForm)
			End With
		End If
		
		Apply = displayed_string
	End Function
	
	
	'�X�y�V�����p���[���L���ȃ^�[�Q�b�g�̑�����Ԃ�
	'(�p�C���b�g p ���g�p�����ꍇ)
	Public Function CountTarget(ByRef p As Pilot) As Short
		Dim u As Unit
		Dim i As Short
		
		Select Case TargetType
			Case "����"
				If Effective(p, (p.Unit_Renamed)) Then
					CountTarget = 1
				End If
				
			Case "����", "�S����"
				For	Each u In UList
					With u
						'�o�����Ă���H
						If .Status_Renamed <> "�o��" Then
							GoTo NextUnit1
						End If
						
						'�������j�b�g�H
						Select Case p.Party
							Case "����", "�m�o�b"
								If .Party <> "����" And .Party0 <> "����" And .Party <> "�m�o�b" And .Party0 <> "�m�o�b" Then
									GoTo NextUnit1
								End If
							Case Else
								If p.Party <> .Party Then
									GoTo NextUnit1
								End If
						End Select
						
						'���ʂ�����H
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit1: 
				Next u
				
			Case "�j�󖡕�"
				For	Each u In UList
					With u
						'�j�󂳂�Ă���H
						If .Status_Renamed <> "�j��" Then
							GoTo NextUnit2
						End If
						
						'�������j�b�g�H
						If p.Party <> .Party0 Then
							GoTo NextUnit2
						End If
						
						'���ʂ�����H
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit2: 
				Next u
				
			Case "�G", "�S�G"
				For	Each u In UList
					With u
						'�o�����Ă���H
						If .Status_Renamed <> "�o��" Then
							GoTo NextUnit3
						End If
						
						'�G���j�b�g�H
						Select Case p.Party
							Case "����", "�m�o�b"
								If (.Party = "����" And .Party0 = "����") Or (.Party = "�m�o�b" And .Party0 = "�m�o�b") Then
									GoTo NextUnit3
								End If
							Case Else
								If p.Party = .Party Then
									GoTo NextUnit3
								End If
						End Select
						
						'���ʂ�����H
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit3: 
				Next u
				
			Case "�C��", "�S"
				For	Each u In UList
					'�o�����Ă���H
					If u.Status_Renamed = "�o��" Then
						'���ʂ�����H
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End If
				Next u
		End Select
	End Function
	
	'�X�y�V�����p���[�̃A�j���[�V������\��
	Public Function PlayAnimation() As Boolean
		Dim anime As String
		Dim animes() As String
		Dim anime_head As Short
		Dim buf As String
		Dim ret As Double
		Dim i, j As Short
		Dim expr As String
		Dim wait_time As Integer
		Dim prev_obj_color As Integer
		Dim prev_obj_fill_color As Integer
		Dim prev_obj_fill_style As Integer
		Dim prev_obj_draw_width As Integer
		Dim prev_obj_draw_option As String
		
		If Not SpecialPowerAnimation Then
			Exit Function
		End If
		
		If Animation = "-" Then
			PlayAnimation = True
			Exit Function
		End If
		
		'�A�j���w�肪�Ȃ���Ă��Ȃ��ꍇ�̓A�j���\���p�T�u���[�`��������Ȃ���΂��̂܂܏I��
		If Animation = Name Then
			If FindNormalLabel("�r�o�A�j��_" & Animation) = 0 Then
				If Name <> "����" And Name <> "�F��" Then
					If IsLabelDefined("������� " & Name) Then
						HandleEvent("�������", Name)
						PlayAnimation = True
					End If
				End If
				Exit Function
			End If
		End If
		
		'�E�N���b�N���̓A�j���\�����X�L�b�v
		If IsRButtonPressed() Then
			PlayAnimation = True
			Exit Function
		End If
		
		'�I�u�W�F�N�g�F�����L�^���Ă���
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'�I�u�W�F�N�g�F�����f�t�H���g�ɖ߂�
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'�A�j���w��𕪊�
		ReDim animes(1)
		anime_head = 1
		For i = 1 To Len(Animation)
			If Mid(Animation, i, 1) = ";" Then
				animes(UBound(animes)) = Mid(Animation, anime_head, i - anime_head)
				ReDim Preserve animes(UBound(animes) + 1)
				anime_head = i + 1
			End If
		Next 
		animes(UBound(animes)) = Mid(Animation, anime_head)
		
		On Error GoTo ErrorHandler
		
		For i = 1 To UBound(animes)
			anime = animes(i)
			
			'���]��
			FormatMessage(anime)
			
			'��ʃN���A�H
			If LCase(anime) = "clear" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
				GoTo NextAnime
			End If
			
			'�퓬�A�j���ȊO�̓������
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'���ʉ�
					PlayWave(anime)
					If wait_time > 0 Then
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'�J�b�g�C���̕\��
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
					End If
					DisplayBattleMessage("", anime)
					GoTo NextAnime
			End Select
			
			Select Case LCase(LIndex(anime, 1))
				Case "line", "circle", "arc", "oval", "color", "fillcolor", "fillstyle", "drawwidth"
					'��ʏ����R�}���h
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
					End If
					DisplayBattleMessage("", anime)
					GoTo NextAnime
				Case "center"
					'�w�肵�����j�b�g�𒆉��\��
					buf = GetValueAsString(ListIndex(anime, 2))
					If UList.IsDefined(buf) Then
						With UList.Item(buf)
							Center(.X, .Y)
							RedrawScreen()
						End With
					End If
					GoTo NextAnime
			End Select
			
			'�E�F�C�g�H
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'�T�u���[�`���̌Ăяo�����m��
			
			'�퓬�A�j���Đ��O�ɃE�F�C�g������H
			If wait_time > 0 Then
				Sleep(wait_time)
				wait_time = 0
			End If
			
			'�T�u���[�`���Ăяo���̂��߂̎����쐬
			If Left(anime, 1) = "@" Then
				expr = Mid(ListIndex(anime, 1), 2) & "("
				'�����̍\�z
				For j = 2 To ListLength(anime)
					If j > 2 Then
						expr = expr & ","
					End If
					expr = expr & ListIndex(anime, j)
				Next 
				expr = expr & ")"
			ElseIf Not SelectedTarget Is Nothing Then 
				expr = "�r�o�A�j��_" & anime & "(" & SelectedUnit.ID & "," & SelectedTarget.ID & ")"
			Else
				expr = "�r�o�A�j��_" & anime & "(" & SelectedUnit.ID & ",-)"
			End If
			
			'�摜�`�悪�s��ꂽ���ǂ����̔���̂��߂Ƀt���O��������
			IsPictureDrawn = False
			
			'�A�j���Đ�
			SaveBasePoint()
			CallFunction(expr, Expression.ValueType.StringType, buf, ret)
			RestoreBasePoint()
			
			'�摜���������Ă���
			If IsPictureDrawn And LCase(buf) <> "keep" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
			
NextAnime: 
		Next 
		
		'�퓬�A�j���Đ���ɃE�F�C�g������H
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'���b�Z�[�W�E�B���h�E�����
		CloseMessageForm()
		
		'�I�u�W�F�N�g�F�������ɖ߂�
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		PlayAnimation = True
		Exit Function
		
ErrorHandler: 
		
		'�A�j���Đ����ɔ��������G���[�̏���
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Function
End Class