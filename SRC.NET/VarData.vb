Option Strict Off
Option Explicit On
Friend Class VarData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�ϐ��̃N���X
	
	'����
	Public Name As String
	'�^
	Public VariableType As Expression.ValueType
	'������l
	Public StringValue As String
	'���l
	Public NumericValue As Double
End Class