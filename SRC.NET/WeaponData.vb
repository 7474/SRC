Option Strict Off
Option Explicit On
Friend Class WeaponData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'����f�[�^�N���X
	
	'���햼
	Public Name As String
	'�U����
	Public Power As Integer
	'�ŏ��˒�
	Public MinRange As Short
	'�ő�˒�
	Public MaxRange As Short
	'������
	Public Precision As Short
	'�e��
	Public Bullet As Short
	'����d�m
	Public ENConsumption As Short
	'�K�v�C��
	Public NecessaryMorale As Short
	'�n�`�K��
	Public Adaption As String
	'�b�s��
	Public Critical As Short
	'����
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Class_Renamed As String
	'�K�v�Z�\
	Public NecessarySkill As String
	'�K�v����
	Public NecessaryCondition As String
	
	'���툤��
	Public Function Nickname() As String
		Nickname = Name
		ReplaceSubExpression(Nickname)
		If InStr(Nickname, "(") > 0 Then
			Nickname = Left(Nickname, InStr(Nickname, "(") - 1)
		End If
	End Function
	
	'�g���̂ăA�C�e���ɂ�镐�킩�ǂ�����Ԃ�
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