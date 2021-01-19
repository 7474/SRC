Option Strict Off
Option Explicit On
Friend Class WeaponData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Name As String
	'Invalid_string_refer_to_original_code
	Public Power As Integer
	'Invalid_string_refer_to_original_code
	Public MinRange As Short
	'Invalid_string_refer_to_original_code
	Public MaxRange As Short
	'Invalid_string_refer_to_original_code
	Public Precision As Short
	'蠑ｾ謨ｰ
	Public Bullet As Short
	'Invalid_string_refer_to_original_code
	Public ENConsumption As Short
	'Invalid_string_refer_to_original_code
	Public NecessaryMorale As Short
	'Invalid_string_refer_to_original_code
	Public Adaption As String
	'Invalid_string_refer_to_original_code
	Public Critical As Short
	'螻樊ｧ
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Class_Renamed As String
	'Invalid_string_refer_to_original_code
	Public NecessarySkill As String
	'Invalid_string_refer_to_original_code
	Public NecessaryCondition As String
	
	'豁ｦ蝎ｨ諢帷ｧｰ
	Public Function Nickname() As String
		Nickname = Name
		ReplaceSubExpression(Nickname)
		If InStr(Nickname, "(") > 0 Then
			Nickname = Left(Nickname, InStr(Nickname, "(") - 1)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsItem() As Boolean
		Dim i As Short
		
		For i = 1 To LLength(NecessarySkill)
			If LIndex(NecessarySkill, i) = "Invalid_string_refer_to_original_code" Then
				IsItem = True
				Exit Function
			End If
		Next 
	End Function
End Class