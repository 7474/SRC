Option Strict Off
Option Explicit On
Friend Class LabelData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�C�x���g�f�[�^�̃��x���̃N���X
	
	'���x����
	Public Name As Event.LabelType
	'�s�ԍ�
	Public LineNum As Integer
	'���x�����L�����H
	Public Enable As Boolean
	'�A�X�^���X�N�̎w���
	Public AsterNum As Short
	
	'���x���S��
	Private StrData As String
	'���x���̌�
	Private intParaNum As Short
	'���x���̊e�p�����[�^
	Private strParas() As String
	'�p�����[�^���Œ�l�H
	Private blnConst() As Boolean
	
	'�p�����[�^�̌�
	Public Function CountPara() As Short
		CountPara = intParaNum
	End Function
	
	'���x���� idx �Ԗڂ̃p�����[�^
	Public Function Para(ByVal idx As Short) As String
		If idx <= intParaNum Then
			If blnConst(idx) Then
				Para = strParas(idx)
			Else
				Para = GetValueAsString(strParas(idx), True)
			End If
		End If
	End Function
	
	'���x���S�̂����o��
	
	'���x���S�̂�ݒ�
	Public Property Data() As String
		Get
			DataControl = StrData
		End Get
		Set(ByVal Value As String)
			Dim i As Integer
			Dim lname As String
			
			'���x���S��
			StrData = Value
			
			'���x����
			lname = ListIndex(Value, 1)
			'�u*�v�͏Ȃ�
			Select Case Asc(lname)
				Case 42 '*
					lname = Mid(lname, 2)
					Select Case Asc(lname)
						Case 42 '*
							lname = Mid(lname, 2)
							AsterNum = 3
						Case 45 '-
							lname = Mid(lname, 2)
							AsterNum = 2
						Case Else
							AsterNum = 2
					End Select
				Case 45 '-
					lname = Mid(lname, 2)
					Select Case Asc(lname)
						Case 42 '*
							lname = Mid(lname, 2)
							AsterNum = 1
						Case 45 '-
							lname = Mid(lname, 2)
					End Select
			End Select
			Select Case lname
				Case "�v�����[�O"
					Name = Event_Renamed.LabelType.PrologueEventLabel
				Case "�X�^�[�g"
					Name = Event_Renamed.LabelType.StartEventLabel
				Case "�G�s���[�O"
					Name = Event_Renamed.LabelType.EpilogueEventLabel
				Case "�^�[��"
					Name = Event_Renamed.LabelType.TurnEventLabel
				Case "������"
					Name = Event_Renamed.LabelType.DamageEventLabel
				Case "�j��"
					Name = Event_Renamed.LabelType.DestructionEventLabel
				Case "�S��"
					Name = Event_Renamed.LabelType.TotalDestructionEventLabel
				Case "�U��"
					Name = Event_Renamed.LabelType.AttackEventLabel
				Case "�U����"
					Name = Event_Renamed.LabelType.AfterAttackEventLabel
				Case "��b"
					Name = Event_Renamed.LabelType.TalkEventLabel
				Case "�ڐG"
					Name = Event_Renamed.LabelType.ContactEventLabel
				Case "�i��"
					Name = Event_Renamed.LabelType.EnterEventLabel
				Case "�E�o"
					Name = Event_Renamed.LabelType.EscapeEventLabel
				Case "���["
					Name = Event_Renamed.LabelType.LandEventLabel
				Case "�g�p"
					Name = Event_Renamed.LabelType.UseEventLabel
				Case "�g�p��"
					Name = Event_Renamed.LabelType.AfterUseEventLabel
				Case "�ό`"
					Name = Event_Renamed.LabelType.TransformEventLabel
				Case "����"
					Name = Event_Renamed.LabelType.CombineEventLabel
				Case "����"
					Name = Event_Renamed.LabelType.SplitEventLabel
				Case "�s���I��"
					Name = Event_Renamed.LabelType.FinishEventLabel
				Case "���x���A�b�v"
					Name = Event_Renamed.LabelType.LevelUpEventLabel
				Case "��������"
					Name = Event_Renamed.LabelType.RequirementEventLabel
				Case "�ĊJ"
					Name = Event_Renamed.LabelType.ResumeEventLabel
				Case "�}�b�v�R�}���h"
					Name = Event_Renamed.LabelType.MapCommandEventLabel
				Case "���j�b�g�R�}���h"
					Name = Event_Renamed.LabelType.UnitCommandEventLabel
				Case "�������"
					Name = Event_Renamed.LabelType.EffectEventLabel
				Case Else
					Name = Event_Renamed.LabelType.NormalLabel
			End Select
			
			'�p�����[�^
			intParaNum = ListLength(Value)
			If intParaNum = -1 Then
				DisplayEventErrorMessage(CurrentLineNum, "���x���̈����̊��ʂ̑Ή������Ă��܂���")
				Exit Property
			End If
			ReDim strParas(intParaNum)
			ReDim blnConst(intParaNum)
			For i = 2 To intParaNum
				strParas(i) = ListIndex(Value, i)
				'�p�����[�^���Œ�l���ǂ�������
				If IsNumeric(strParas(i)) Then
					blnConst(i) = True
				ElseIf PDList.IsDefined(strParas(i)) Then 
					If InStr(strParas(i), "��l��") <> 1 And InStr(strParas(i), "�q���C��") <> 1 Then
						blnConst(i) = True
					End If
				ElseIf UDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				ElseIf IDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				Else
					Select Case strParas(i)
						Case "����", "�m�o�b", "�G", "����", "�S"
							blnConst(i) = True
						Case "N", "W", "S", "E"
							If Name = Event_Renamed.LabelType.EscapeEventLabel Then
								blnConst(i) = True
							End If
						Case Else
							If Left(strParas(i), 1) = """" And Right(strParas(i), 1) = """" Then
								If InStr(strParas(i), "$(") = 0 Then
									strParas(i) = Mid(strParas(i), 2, Len(strParas(i)) - 2)
									blnConst(i) = True
								End If
							End If
					End Select
				End If
			Next 
		End Set
	End Property
End Class