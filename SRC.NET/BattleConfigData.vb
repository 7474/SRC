Option Strict Off
Option Explicit On
Friend Class BattleConfigData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'名称
	Public Name As String
	
	'Invalid_string_refer_to_original_code
	Public ConfigCalc As String
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		ConfigCalc = ""
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function Calculate() As Double
		Dim expr As String
		Dim morales As Integer
		
		expr = ConfigCalc
		
		'コンフィグ変数を有効にする
		BCVariable.IsConfig = True
		
		'式を評価する
		Calculate = GetValueAsDouble(expr)
		
		'コンフィグ変数を無効にする
		BCVariable.IsConfig = False
	End Function
End Class