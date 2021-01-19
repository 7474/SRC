Option Strict Off
Option Explicit On
Friend Class BattleConfigData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'蜷咲ｧｰ
	Public Name As String
	
	'Invalid_string_refer_to_original_code
	Public ConfigCalc As String
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
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
		
		'繧ｳ繝ｳ繝輔ぅ繧ｰ螟画焚繧呈怏蜉ｹ縺ｫ縺吶ｋ
		BCVariable.IsConfig = True
		
		'蠑上ｒ隧穂ｾ｡縺吶ｋ
		Calculate = GetValueAsDouble(expr)
		
		'繧ｳ繝ｳ繝輔ぅ繧ｰ螟画焚繧堤┌蜉ｹ縺ｫ縺吶ｋ
		BCVariable.IsConfig = False
	End Function
End Class