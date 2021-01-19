Option Strict Off
Option Explicit On
Friend Class AliasData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'蜷咲ｧｰ
	Public Name As String
	
	Private strAliasType() As String
	Private dblAliasLevel() As Double
	Private blnAliasLevelIsPlusMod() As Boolean
	Private blnAliasLevelIsMultMod() As Boolean
	Private strAliasData() As String
	Private strAliasNecessarySkill() As String
	Private strAliasNecessaryCondition() As String
	
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		ReDim strAliasType(0)
		ReDim dblAliasLevel(0)
		ReDim blnAliasLevelIsPlusMod(0)
		ReDim blnAliasLevelIsMultMod(0)
		ReDim strAliasData(0)
		ReDim strAliasNecessarySkill(0)
		ReDim strAliasNecessaryCondition(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	
	'繧ｨ繝ｪ繧｢繧ｹ縺ｮ蛟区焚
	Public ReadOnly Property Count() As Short
		Get
			Count = UBound(strAliasType)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasType(ByVal idx As Short) As String
		Get
			AliasType = strAliasType(idx)
		End Get
	End Property
	
	'繧ｨ繝ｪ繧｢繧ｹ縺ｮ繝ｬ繝吶Ν
	Public ReadOnly Property AliasLevel(ByVal idx As Short) As Double
		Get
			AliasLevel = dblAliasLevel(idx)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasLevelIsPlusMod(ByVal idx As Short) As Boolean
		Get
			AliasLevelIsPlusMod = blnAliasLevelIsPlusMod(idx)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasLevelIsMultMod(ByVal idx As Short) As Boolean
		Get
			AliasLevelIsMultMod = blnAliasLevelIsMultMod(idx)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasData(ByVal idx As Short) As String
		Get
			AliasData = strAliasData(idx)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasNecessarySkill(ByVal idx As Short) As String
		Get
			AliasNecessarySkill = strAliasNecessarySkill(idx)
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property AliasNecessaryCondition(ByVal idx As Short) As String
		Get
			AliasNecessaryCondition = strAliasNecessaryCondition(idx)
		End Get
	End Property
	
	'繧ｨ繝ｪ繧｢繧ｹ繧堤匳骭ｲ
	Public Sub AddAlias(ByVal adef As String)
		Dim atype, adata As String
		Dim alevel As Double
		Dim j, i, n As Short
		Dim buf As String
		
		'Invalid_string_refer_to_original_code
		n = UBound(strAliasType) + 1
		ReDim Preserve strAliasType(n)
		ReDim Preserve dblAliasLevel(n)
		ReDim Preserve blnAliasLevelIsPlusMod(n)
		ReDim Preserve blnAliasLevelIsMultMod(n)
		ReDim Preserve strAliasData(n)
		ReDim Preserve strAliasNecessarySkill(n)
		ReDim Preserve strAliasNecessaryCondition(n)
		
		'Invalid_string_refer_to_original_code
		If Right(adef, 1) = ")" Then
			i = InStr(adef, " (")
			If i > 0 Then
				strAliasNecessarySkill(n) = Trim(Mid(adef, i + 2, Len(adef) - i - 2))
				buf = Trim(Left(adef, i))
			ElseIf Left(adef, 1) = "(" Then 
				strAliasNecessarySkill(n) = Trim(Mid(adef, 2, Len(adef) - 2))
				buf = ""
			Else
				buf = adef
			End If
		Else
			buf = adef
		End If
		
		'Invalid_string_refer_to_original_code
		If Right(buf, 1) = ">" Then
			i = InStr(adef, " <")
			If i > 0 Then
				strAliasNecessaryCondition(n) = Trim(Mid(buf, i + 2, Len(buf) - i - 2))
				buf = Trim(Left(buf, i))
			ElseIf Left(buf, 1) = "<" Then 
				strAliasNecessaryCondition(n) = Trim(Mid(buf, 2, Len(buf) - 2))
				buf = ""
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If ReplaceString(buf, "Lv+", "Lv") Then
			blnAliasLevelIsPlusMod(n) = True
		ElseIf ReplaceString(buf, "Lv*", "Lv") Then 
			blnAliasLevelIsMultMod(n) = True
		End If
		
		'Invalid_string_refer_to_original_code
		alevel = DEFAULT_LEVEL
		i = InStr(buf, "Lv")
		j = InStr(buf, "=")
		If i > 0 And j > 0 And i > j Then
			i = 0
		End If
		If i > 0 Then
			atype = Left(buf, i - 1)
			If j > 0 Then
				alevel = CDbl(Mid(buf, i + 2, j - (i + 2)))
				adata = Mid(buf, j + 1)
			Else
				alevel = CDbl(Mid(buf, i + 2))
			End If
		ElseIf j > 0 Then 
			atype = Left(buf, j - 1)
			adata = Mid(buf, j + 1)
		Else
			atype = buf
		End If
		
		'繧ｨ繝ｪ繧｢繧ｹ繧堤匳骭ｲ
		strAliasType(n) = atype
		dblAliasLevel(n) = alevel
		strAliasData(n) = adata
	End Sub
End Class