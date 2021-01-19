Option Strict Off
Option Explicit On
Friend Class BattleConfigData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�o�g���R���t�B�O�f�[�^�̃N���X
	' --- �_���[�W�v�Z�A�������Z�o�ȂǁA�o�g���Ɋ֘A����G���A�X���`���܂��B
	
	'����
	Public Name As String
	
	'�v�Z��
	Public ConfigCalc As String
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		ConfigCalc = ""
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'�o�g���R���t�B�O�f�[�^�Ɋ�Â����u�����v�Z�̎��s
	'���s�O�Ɏg���\���̂���ϐ������O�ɑ�����Ă�������
	Public Function Calculate() As Double
		Dim expr As String
		Dim morales As Integer
		
		expr = ConfigCalc
		
		'�R���t�B�O�ϐ���L���ɂ���
		BCVariable.IsConfig = True
		
		'����]������
		Calculate = GetValueAsDouble(expr)
		
		'�R���t�B�O�ϐ��𖳌��ɂ���
		BCVariable.IsConfig = False
	End Function
End Class