Option Strict Off
Option Explicit On
Friend Class frmNowLoading
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�f�[�^���[�h�̐i�s�󋵂������t�H�[��
	
	'�f�[�^����
	'UPGRADE_NOTE: Size �� Size_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Size_Renamed As Short
	'�ǂݍ��ݏI�����f�[�^�̐�
	Public Value As Short
	
	'���[�h���P�i�K�i�s������
	Public Sub Progress()
		Value = Value + 1
		'UPGRADE_ISSUE: PictureBox ���\�b�h picBar.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		picBar.Cls()
		'UPGRADE_ISSUE: PictureBox ���\�b�h picBar.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		picBar.Line (0, 0) - (picBar.ClientRectangle.Width * Value \ Size_Renamed, picBar.ClientRectangle.Height), BF
		Refresh()
	End Sub
End Class