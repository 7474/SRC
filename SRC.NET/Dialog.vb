Option Strict Off
Option Explicit On
Friend Class Dialog
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�_�C�A���O�̃N���X
	
	'�_�C�A���O�Ɋ܂܂�郁�b�Z�[�W��
	Private intMessageNum As Object
	'���b�Z�[�W�̘b��
	Private strName() As String
	'���b�Z�[�W
	Private strMessage() As String
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		'UPGRADE_WARNING: �I�u�W�F�N�g intMessageNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		intMessageNum = 0
		ReDim strName(0)
		ReDim strMessage(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�N���X�̒ǉ�
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		ReDim strName(0)
		ReDim strMessage(0)
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'�_�C�A���O�Ƀ��b�Z�[�W��ǉ�
	Public Sub AddMessage(ByRef Name As String, ByRef Message As String)
		'UPGRADE_WARNING: �I�u�W�F�N�g intMessageNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		intMessageNum = intMessageNum + 1
		ReDim Preserve strName(intMessageNum)
		ReDim Preserve strMessage(intMessageNum)
		'UPGRADE_WARNING: �I�u�W�F�N�g intMessageNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		strName(intMessageNum) = Name
		'UPGRADE_WARNING: �I�u�W�F�N�g intMessageNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		strMessage(intMessageNum) = Message
	End Sub
	
	'�_�C�A���O�Ɋ܂܂�郁�b�Z�[�W��
	Public ReadOnly Property Count() As Short
		Get
			'UPGRADE_WARNING: �I�u�W�F�N�g intMessageNum �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			Count = intMessageNum
		End Get
	End Property
	
	'�_�C�A���O�Ɏg���Ă���p�C���b�g���S�ė��p�\������
	Public ReadOnly Property IsAvailable(ByVal u As Unit, ByVal ignore_condition As Boolean) As Boolean
		Get
			Dim pname, pname2 As String
			Dim i, j As Short
			Dim mpnickname, mpname, mtype As String
			
			With u.MainPilot
				mpname = .Name
				mpnickname = .Nickname
				mtype = .MessageType
			End With
			
			For i = 1 To Count
				pname = strName(i)
				
				'���̋Z�̃p�[�g�i�[���w�肳��Ă���ꍇ
				If Left(pname, 1) = "@" Then
					pname = Mid(pname, 2)
					For j = 1 To UBound(SelectedPartners)
						With SelectedPartners(j)
							If .CountPilot > 0 Then
								'�p�[�g�i�[�̖��O�ƈ�v����H
								With .MainPilot
									If pname <> .Name And InStr(pname, .Name & "(") <> 1 And pname <> .Nickname And InStr(pname, .Nickname & "(") <> 1 Then
										GoTo NextPartner
									End If
								End With
								
								'����邩�ǂ����`�F�b�N
								If Not ignore_condition Then
									If .IsConditionSatisfied("����") Or .IsConditionSatisfied("���") Or .IsConditionSatisfied("�Ή�") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("����") Then
										IsAvailable = False
										Exit Property
									End If
								End If
								
								'���b�Z�[�W�͎g�p�\
								GoTo NextMessage
							End If
						End With
NextPartner: 
					Next 
					
					IsAvailable = False
					Exit Property
				End If
				
				'�\��p�^�[�����w�肳��Ă���H
				If InStr(pname, "(") > 0 Then
					If Not PDList.IsDefined2(pname) And NPDList.IsDefined2(pname) Then
						'���ʕ������폜
						For j = 2 To Len(pname)
							If Mid(pname, Len(pname) - j, 1) = "(" Then
								pname2 = Left(pname, Len(pname) - j - 1)
								Exit For
							End If
						Next 
						
						'�\��p�^�[�����ǂ�������
						If PDList.IsDefined2(pname2) Or NPDList.IsDefined2(pname2) Then
							'�\��p�^�[���Ƃ݂Ȃ�
							pname = pname2
						End If
					End If
				End If
				
				'���C���p�C���b�g�͏�ɑ���
				If pname = mpname Then
					GoTo NextMessage
				End If
				If pname = mpnickname Then
					GoTo NextMessage
				End If
				If pname = mtype Then
					GoTo NextMessage
				End If
				
				'�m���p�C���b�g��Leave���Ă��Ȃ������ɑ���
				If NPDList.IsDefined(pname) Then
					If IsGlobalVariableDefined("IsAway(" & pname & ")") Then
						IsAvailable = False
						Exit Property
					End If
					GoTo NextMessage
				End If
				
				If PDList.IsDefined(pname) Then
					'�p�C���b�g�̏ꍇ
					
					'�p�C���b�g���쐬����Ă��Ȃ��H
					If Not PList.IsDefined(pname) Then
						IsAvailable = False
						Exit Property
					End If
					
					With PList.Item(pname)
						'Leave���H
						If .Away Then
							IsAvailable = False
							Exit Property
						End If
						
						'����邩�ǂ����`�F�b�N
						If Not ignore_condition And Not .Unit Is Nothing Then
							With .Unit
								If .IsConditionSatisfied("����") Or .IsConditionSatisfied("���") Or .IsConditionSatisfied("�Ή�") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("����") Then
									IsAvailable = False
									Exit Property
								End If
							End With
						End If
					End With
				End If
				
NextMessage: 
			Next 
			
			IsAvailable = True
		End Get
	End Property
	
	'idx�Ԗڂ̃��b�Z�[�W�̘b��
	Public Function Name(ByVal idx As Short) As String
		Name = strName(idx)
	End Function
	
	'idx�Ԗڂ̃��b�Z�[�W
	Public Function Message(ByVal idx As Short) As String
		Message = strMessage(idx)
	End Function
End Class