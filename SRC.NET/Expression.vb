Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Module Expression
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'ã‚¤ãƒ™ãƒ³ãƒˆãƒ‡ãƒ¼ã‚¿ã®å¼è¨ˆç®—ã‚’è¡Œã†ãƒ¢ã‚¸ãƒ¥ãƒ¼ãƒ«
	
	'Invalid_string_refer_to_original_code
	Enum OperatorType
		PlusOp
		MinusOp
		MultOp
		DivOp
		IntDivOp
		ExpoOp
		ModOp
		CatOp
		EqOp
		NotEqOp
		LtOp
		LtEqOp
		GtOp
		GtEqOp
		NotOp
		AndOp
		OrOp
		LikeOp
	End Enum
	
	'Invalid_string_refer_to_original_code
	Enum ValueType
		UndefinedType = 0
		StringType
		NumericType
	End Enum
	
	'æ­£è¦è¡¨ç¾
	Private RegEx As Object
	Private Matches As Object
	
	
	'å¼ã‚’è©•ä¾¡
	Public Function EvalExpr(ByRef expr As String, ByRef etype As ValueType, ByRef str_result As String, ByRef num_result As Double) As ValueType
		Dim terms() As String
		Dim tnum As Short
		Dim op_idx, op_pri As Short
		Dim op_type As OperatorType
		Dim lop, rop As String
		Dim lstr, rstr As String
		Dim lnum, rnum As Double
		Dim is_lop_term, is_rop_term As Boolean
		Dim osize, i, ret, tsize As Short
		Dim buf As String
		
		'Invalid_string_refer_to_original_code
		tnum = ListSplit(expr, terms)
		
		Select Case tnum
			'ç©ºç™½
			Case 0
				EvalExpr = etype
				Exit Function
				
				'Invalid_string_refer_to_original_code
			Case 1
				EvalExpr = EvalTerm(terms(1), etype, str_result, num_result)
				Exit Function
				
				'Invalid_string_refer_to_original_code
			Case -1
				If etype = ValueType.NumericType Then
					'Invalid_string_refer_to_original_code
					EvalExpr = ValueType.NumericType
				Else
					EvalExpr = ValueType.StringType
					str_result = expr
				End If
				Exit Function
		End Select
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		op_idx = 0
		op_pri = 100
		For i = 1 To tnum - 1
			'Invalid_string_refer_to_original_code
			ret = Asc(terms(i))
			If ret < 0 Then
				GoTo NextTerm
			End If
			If ret > 111 Then
				GoTo NextTerm
			End If
			Select Case Len(terms(i))
				Case 1
					Select Case ret
						Case 94 '^
							If op_pri >= 10 Then
								op_type = OperatorType.ExpoOp
								op_pri = 10
								op_idx = i
							End If
						Case 42 '*
							If op_pri >= 9 Then
								op_type = OperatorType.MultOp
								op_pri = 9
								op_idx = i
							End If
						Case 47 '/
							If op_pri >= 9 Then
								op_type = OperatorType.DivOp
								op_pri = 9
								op_idx = i
							End If
						Case 92 '\
							If op_pri >= 8 Then
								op_type = OperatorType.IntDivOp
								op_pri = 8
								op_idx = i
							End If
						Case 43 '+
							If op_pri >= 6 Then
								op_type = OperatorType.PlusOp
								op_pri = 6
								op_idx = i
							End If
						Case 45 '-
							If op_pri >= 6 Then
								op_type = OperatorType.MinusOp
								op_pri = 6
								op_idx = i
							End If
						Case 38 '&
							If op_pri >= 5 Then
								op_type = OperatorType.CatOp
								op_pri = 5
								op_idx = i
							End If
						Case 60 '<
							If op_pri >= 4 Then
								op_type = OperatorType.LtOp
								op_pri = 4
								op_idx = i
							End If
						Case 61 '=
							If op_pri >= 4 Then
								op_type = OperatorType.EqOp
								op_pri = 4
								op_idx = i
							End If
						Case 62 '>
							If op_pri >= 4 Then
								op_type = OperatorType.GtOp
								op_pri = 4
								op_idx = i
							End If
					End Select
				Case 2
					Select Case ret
						Case 33 '!=
							If op_pri >= 4 Then
								If Right(terms(i), 1) = "=" Then
									op_type = OperatorType.NotEqOp
									op_pri = 4
									op_idx = i
								End If
							End If
						Case 60 '<>, <=
							If op_pri >= 4 Then
								Select Case Right(terms(i), 1)
									Case ">"
										op_type = OperatorType.NotEqOp
										op_pri = 4
										op_idx = i
									Case "="
										op_type = OperatorType.LtEqOp
										op_pri = 4
										op_idx = i
								End Select
							End If
						Case 62 '>=
							If op_pri >= 4 Then
								If Right(terms(i), 1) = "=" Then
									op_type = OperatorType.GtEqOp
									op_pri = 4
									op_idx = i
								End If
							End If
						Case 79, 111 'or
							If op_pri > 1 Then
								If LCase(terms(i)) = "or" Then
									op_type = OperatorType.OrOp
									op_pri = 1
									op_idx = i
								End If
							End If
					End Select
				Case 3
					Select Case ret
						Case 77, 109 'mod
							If op_pri >= 7 Then
								If LCase(terms(i)) = "mod" Then
									op_type = OperatorType.ModOp
									op_pri = 7
									op_idx = i
								End If
							End If
						Case 78, 110 'not
							If op_pri > 3 Then
								If LCase(terms(i)) = "not" Then
									op_type = OperatorType.NotOp
									op_pri = 3
									op_idx = i
								End If
							End If
						Case 65, 97 'and
							If op_pri > 2 Then
								If LCase(terms(i)) = "and" Then
									op_type = OperatorType.AndOp
									op_pri = 2
									op_idx = i
								End If
							End If
					End Select
				Case 4
					Select Case ret
						Case 76, 108 'like
							If op_pri >= 7 Then
								If LCase(terms(i)) = "like" Then
									op_type = OperatorType.LikeOp
									op_pri = 4
									op_idx = i
								End If
							End If
					End Select
			End Select
NextTerm: 
		Next 
		
		If op_idx = 0 Then
			'Invalid_string_refer_to_original_code
			EvalExpr = ValueType.StringType
			str_result = expr
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case op_idx
			Case 1
				'Invalid_string_refer_to_original_code
				is_lop_term = True
				lop = ""
			Case 2
				'Invalid_string_refer_to_original_code
				is_lop_term = True
				lop = terms(1)
			Case Else
				'Invalid_string_refer_to_original_code
				buf = New String(vbNullChar, Len(expr))
				tsize = Len(terms(1))
				Mid(buf, 1, tsize) = terms(1)
				osize = tsize
				For i = 2 To op_idx - 1
					Mid(buf, osize + 1, 1) = " "
					tsize = Len(terms(i))
					Mid(buf, osize + 2, tsize) = terms(i)
					osize = osize + tsize + 1
				Next 
				lop = Left(buf, osize)
		End Select
		If op_idx = tnum - 1 Then
			'Invalid_string_refer_to_original_code
			is_rop_term = True
			rop = terms(tnum)
		Else
			'Invalid_string_refer_to_original_code
			buf = New String(vbNullChar, Len(expr))
			tsize = Len(terms(op_idx + 1))
			Mid(buf, 1, tsize) = terms(op_idx + 1)
			osize = tsize
			For i = op_idx + 2 To tnum
				Mid(buf, osize + 1, 1) = " "
				tsize = Len(terms(i))
				Mid(buf, osize + 2, tsize) = terms(i)
				osize = osize + tsize + 1
			Next 
			rop = Left(buf, osize)
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case op_type
			Case OperatorType.PlusOp '+
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(lnum + rnum)
				Else
					EvalExpr = ValueType.NumericType
					num_result = lnum + rnum
				End If
				
			Case OperatorType.MinusOp '-
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(lnum - rnum)
				Else
					EvalExpr = ValueType.NumericType
					num_result = lnum - rnum
				End If
				
			Case OperatorType.MultOp
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(lnum * rnum)
				Else
					EvalExpr = ValueType.NumericType
					num_result = lnum * rnum
				End If
				
			Case OperatorType.DivOp '/
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If rnum <> 0 Then
					num_result = lnum / rnum
				Else
					num_result = 0
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(num_result)
				Else
					EvalExpr = ValueType.NumericType
				End If
				
			Case OperatorType.IntDivOp '\
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If rnum <> 0 Then
					num_result = lnum \ rnum
				Else
					num_result = 0
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(num_result)
				Else
					EvalExpr = ValueType.NumericType
				End If
				
			Case OperatorType.ModOp 'Mod
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					'UPGRADE_WARNING: Mod ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					str_result = FormatNum(lnum Mod rnum)
				Else
					EvalExpr = ValueType.NumericType
					'UPGRADE_WARNING: Mod ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					num_result = lnum Mod rnum
				End If
				
			Case OperatorType.ExpoOp '^
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					str_result = FormatNum(lnum ^ rnum)
				Else
					EvalExpr = ValueType.NumericType
					num_result = lnum ^ rnum
				End If
				
			Case OperatorType.CatOp '&
				If is_lop_term Then
					EvalTerm(lop, ValueType.StringType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.StringType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.StringType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.StringType, rstr, rnum)
				End If
				
				If etype = ValueType.NumericType Then
					EvalExpr = ValueType.NumericType
					num_result = StrToDbl(lstr & rstr)
				Else
					EvalExpr = ValueType.StringType
					str_result = lstr & rstr
				End If
				
			Case OperatorType.EqOp '=
				If IsNumber(lop) Or IsNumber(rop) Then
					If is_lop_term Then
						EvalTerm(lop, ValueType.NumericType, lstr, lnum)
					Else
						EvalExpr(lop, ValueType.NumericType, lstr, lnum)
					End If
					If is_rop_term Then
						EvalTerm(rop, ValueType.NumericType, rstr, rnum)
					Else
						EvalExpr(rop, ValueType.NumericType, rstr, rnum)
					End If
					
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						If lnum = rnum Then
							str_result = "1"
						Else
							str_result = "0"
						End If
					Else
						EvalExpr = ValueType.NumericType
						If lnum = rnum Then
							num_result = 1
						Else
							num_result = 0
						End If
					End If
				Else
					If is_lop_term Then
						EvalTerm(lop, ValueType.StringType, lstr, lnum)
					Else
						EvalExpr(lop, ValueType.StringType, lstr, lnum)
					End If
					If is_rop_term Then
						EvalTerm(rop, ValueType.StringType, rstr, rnum)
					Else
						EvalExpr(rop, ValueType.StringType, rstr, rnum)
					End If
					
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						If lstr = rstr Then
							str_result = "1"
						Else
							str_result = "0"
						End If
					Else
						EvalExpr = ValueType.NumericType
						If lstr = rstr Then
							num_result = 1
						Else
							num_result = 0
						End If
					End If
				End If
				
			Case OperatorType.NotEqOp '<>, !=
				If IsNumber(lop) Or IsNumber(rop) Then
					If is_lop_term Then
						EvalTerm(lop, ValueType.NumericType, lstr, lnum)
					Else
						EvalExpr(lop, ValueType.NumericType, lstr, lnum)
					End If
					If is_rop_term Then
						EvalTerm(rop, ValueType.NumericType, rstr, rnum)
					Else
						EvalExpr(rop, ValueType.NumericType, rstr, rnum)
					End If
					
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						If lnum <> rnum Then
							str_result = "1"
						Else
							str_result = "0"
						End If
					Else
						EvalExpr = ValueType.NumericType
						If lnum <> rnum Then
							num_result = 1
						Else
							num_result = 0
						End If
					End If
				Else
					If is_lop_term Then
						EvalTerm(lop, ValueType.StringType, lstr, lnum)
					Else
						EvalExpr(lop, ValueType.StringType, lstr, lnum)
					End If
					If is_rop_term Then
						EvalTerm(rop, ValueType.StringType, rstr, rnum)
					Else
						EvalExpr(rop, ValueType.StringType, rstr, rnum)
					End If
					
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						If lstr <> rstr Then
							str_result = "1"
						Else
							str_result = "0"
						End If
					Else
						EvalExpr = ValueType.NumericType
						If lstr <> rstr Then
							num_result = 1
						Else
							num_result = 0
						End If
					End If
				End If
				
			Case OperatorType.LtOp '<
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If lnum < rnum Then
						str_result = "1"
					Else
						str_result = "0"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If lnum < rnum Then
						num_result = 1
					Else
						num_result = 0
					End If
				End If
				
			Case OperatorType.LtEqOp '<=
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If lnum <= rnum Then
						str_result = "1"
					Else
						str_result = "0"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If lnum <= rnum Then
						num_result = 1
					Else
						num_result = 0
					End If
				End If
				
			Case OperatorType.GtOp '>
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If lnum > rnum Then
						str_result = "1"
					Else
						str_result = "0"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If lnum > rnum Then
						num_result = 1
					Else
						num_result = 0
					End If
				End If
				
			Case OperatorType.GtEqOp '>=
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If lnum >= rnum Then
						str_result = "1"
					Else
						str_result = "0"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If lnum >= rnum Then
						num_result = 1
					Else
						num_result = 0
					End If
				End If
				
			Case OperatorType.LikeOp 'Like
				If is_lop_term Then
					EvalTerm(lop, ValueType.StringType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.StringType, lstr, lnum)
				End If
				If is_rop_term Then
					EvalTerm(rop, ValueType.StringType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.StringType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If lstr Like rstr Then
						str_result = "1"
					Else
						str_result = "0"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If lstr Like rstr Then
						num_result = 1
					Else
						num_result = 0
					End If
				End If
				
			Case OperatorType.NotOp 'Not
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If rnum <> 0 Then
						str_result = "0"
					Else
						str_result = "1"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If rnum <> 0 Then
						num_result = 0
					Else
						num_result = 1
					End If
				End If
				
			Case OperatorType.AndOp 'And
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				
				If lnum = 0 Then
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						str_result = "0"
					Else
						EvalExpr = ValueType.NumericType
						num_result = 0
					End If
					Exit Function
				End If
				
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If rnum = 0 Then
						str_result = "0"
					Else
						str_result = "1"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If rnum = 0 Then
						num_result = 0
					Else
						num_result = 1
					End If
				End If
				
			Case OperatorType.OrOp 'Or
				If is_lop_term Then
					EvalTerm(lop, ValueType.NumericType, lstr, lnum)
				Else
					EvalExpr(lop, ValueType.NumericType, lstr, lnum)
				End If
				
				If lnum <> 0 Then
					If etype = ValueType.StringType Then
						EvalExpr = ValueType.StringType
						str_result = "1"
					Else
						EvalExpr = ValueType.NumericType
						num_result = 1
					End If
					Exit Function
				End If
				
				If is_rop_term Then
					EvalTerm(rop, ValueType.NumericType, rstr, rnum)
				Else
					EvalExpr(rop, ValueType.NumericType, rstr, rnum)
				End If
				
				If etype = ValueType.StringType Then
					EvalExpr = ValueType.StringType
					If rnum = 0 Then
						str_result = "0"
					Else
						str_result = "1"
					End If
				Else
					EvalExpr = ValueType.NumericType
					If rnum = 0 Then
						num_result = 0
					Else
						num_result = 1
					End If
				End If
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EvalTerm(ByRef expr As String, ByRef etype As ValueType, ByRef str_result As String, ByRef num_result As Double) As ValueType
		
		'Invalid_string_refer_to_original_code
		If Len(expr) = 0 Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case Asc(expr)
			Case 9 'Invalid_string_refer_to_original_code
				'ã‚¿ãƒ–ã‚’Trimã™ã‚‹ãŸã‚EvalExprã§è©•ä¾¡
				EvalTerm = EvalExpr(expr, etype, str_result, num_result)
				Exit Function
			Case 32 'ç©ºç™½
				'Invalid_string_refer_to_original_code
				EvalTerm = EvalTerm(Trim(expr), etype, str_result, num_result)
				Exit Function
			Case 34 '"
				'Invalid_string_refer_to_original_code
				If Right(expr, 1) = """" Then
					EvalTerm = ValueType.StringType
					str_result = Mid(expr, 2, Len(expr) - 2)
					ReplaceSubExpression(str_result)
				Else
					str_result = expr
				End If
				If etype <> ValueType.StringType Then
					num_result = StrToDbl(str_result)
				End If
				EvalTerm = ValueType.StringType
				Exit Function
			Case 35 '#
				'Invalid_string_refer_to_original_code
				EvalTerm = ValueType.StringType
				str_result = expr
				Exit Function
			Case 40 '(
				'Invalid_string_refer_to_original_code
				If Right(expr, 1) = ")" Then
					EvalTerm = EvalExpr(Mid(expr, 2, Len(expr) - 2), etype, str_result, num_result)
				Else
					str_result = expr
					If etype <> ValueType.StringType Then
						num_result = StrToDbl(str_result)
					End If
					EvalTerm = ValueType.StringType
				End If
				Exit Function
			Case 43, 45, 48 To 57 'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If IsNumeric(expr) Then
					Select Case etype
						Case ValueType.StringType
							str_result = expr
							EvalTerm = ValueType.StringType
						Case ValueType.NumericType, ValueType.UndefinedType
							num_result = CDbl(expr)
							EvalTerm = ValueType.NumericType
					End Select
					Exit Function
				End If
			Case 96 '`
				'Invalid_string_refer_to_original_code
				If Right(expr, 1) = "`" Then
					str_result = Mid(expr, 2, Len(expr) - 2)
				Else
					str_result = expr
				End If
				If etype <> ValueType.StringType Then
					num_result = StrToDbl(str_result)
				End If
				EvalTerm = ValueType.StringType
				Exit Function
		End Select
		
		'Invalid_string_refer_to_original_code
		EvalTerm = CallFunction(expr, etype, str_result, num_result)
		If EvalTerm <> ValueType.UndefinedType Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		EvalTerm = GetVariable(expr, etype, str_result, num_result)
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Function CallFunction(ByRef expr As String, ByRef etype As ValueType, ByRef str_result As String, ByRef num_result As Double) As ValueType
		Dim fname As String
		Dim start_idx As Short
		Dim num, i, j, num2 As Short
		Dim buf, buf2 As String
		Dim ldbl, rdbl As Double
		Dim pname2, pname, uname As String
		Dim ret As Integer
		Dim cur_depth As Short
		Dim var As VarData
		Dim it As Item
		Dim depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim params(MaxArgIndex) As String
		Dim pcount As Short
		Dim is_term(MaxArgIndex) As Boolean
		Dim dir_path As String
		Static dir_list() As String
		Static dir_index As Short
		Static regexp_index As Short
		
		'Invalid_string_refer_to_original_code
		If Right(expr, 1) <> ")" Then
			CallFunction = ValueType.UndefinedType
			Exit Function
		End If
		i = InStr(expr, " ")
		j = InStr(expr, "(")
		If i > 0 Then
			If i < j Then
				CallFunction = ValueType.UndefinedType
				Exit Function
			End If
		Else
			If j = 0 Then
				CallFunction = ValueType.UndefinedType
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		
		'ãƒ‘ãƒ©ãƒ¡ãƒ¼ã‚¿ã®æŠ½å‡º
		pcount = 0
		start_idx = j + 1
		depth = 0
		in_single_quote = False
		in_double_quote = False
		num = Len(expr)
		Dim counter As Short
		counter = start_idx
		For i = counter To num - 1
			If in_single_quote Then
				If Asc(Mid(expr, i, 1)) = 96 Then '`
					in_single_quote = False
				End If
			ElseIf in_double_quote Then 
				If Asc(Mid(expr, i, 1)) = 34 Then '"
					in_double_quote = False
				End If
			Else
				Select Case Asc(Mid(expr, i, 1))
					Case 9, 32 'Invalid_string_refer_to_original_code
						If start_idx = i Then
							start_idx = i + 1
						Else
							is_term(pcount + 1) = False
						End If
					Case 40, 91 '(, [
						depth = depth + 1
					Case 41, 93 '), ]
						depth = depth - 1
					Case 44 ',
						If depth = 0 Then
							pcount = pcount + 1
							params(pcount) = Mid(expr, start_idx, i - start_idx)
							start_idx = i + 1
							is_term(pcount + 1) = True
						End If
					Case 96 '`
						in_single_quote = True
					Case 34 '"
						in_double_quote = True
				End Select
			End If
		Next 
		If num > start_idx Then
			pcount = pcount + 1
			params(pcount) = Mid(expr, start_idx, num - start_idx)
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case Asc(expr)
			Case 95 '_
				'Invalid_string_refer_to_original_code
				fname = Left(expr, j - 1)
				GoTo LookUpUserDefinedID
			Case 65 To 90, 97 To 122 'A To z
				'Invalid_string_refer_to_original_code
				fname = Left(expr, j - 1)
			Case Else
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If UDList.IsDefined(expr) Then
					CallFunction = ValueType.UndefinedType
					Exit Function
				End If
				If PDList.IsDefined(expr) Then
					CallFunction = ValueType.UndefinedType
					Exit Function
				End If
				If NPDList.IsDefined(expr) Then
					CallFunction = ValueType.UndefinedType
					Exit Function
				End If
				If IDList.IsDefined(expr) Then
					CallFunction = ValueType.UndefinedType
					Exit Function
				End If
				fname = Left(expr, j - 1)
				GoTo LookUpUserDefinedID
		End Select
		
		'Invalid_string_refer_to_original_code
		Dim PT As POINTAPI
		Dim in_window As Boolean
		Dim x2, x1, y1, y2 As Short
		Dim d1, d2 As Date
		Dim list() As String
		Dim flag As Boolean
		Select Case LCase(fname)
			'Invalid_string_refer_to_original_code
			Case "args"
				'Invalid_string_refer_to_original_code
				num = UpVarLevel
				i = CallDepth
				Do While num > 0
					i = i - num
					If i < 1 Then
						i = 1
						Exit Do
					End If
					num = UpVarLevelStack(i)
				Loop 
				If i < 1 Then
					i = 1
				End If
				
				'Invalid_string_refer_to_original_code
				num = GetValueAsLong(params(1), is_term(1))
				If num <= ArgIndex - ArgIndexStack(i - 1) Then
					str_result = ArgStack(ArgIndex - num + 1)
				End If
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "call"
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				ret = FindNormalLabel(params(1))
				If ret = 0 Then
					'Invalid_string_refer_to_original_code
					ret = FindNormalLabel(GetValueAsString(params(1), is_term(1)))
					If ret = 0 Then
						DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						Exit Function
					End If
				End If
				ret = ret + 1
				
				'Invalid_string_refer_to_original_code
				If CallDepth > MaxCallDepth Then
					CallDepth = MaxCallDepth
					DisplayEventErrorMessage(CurrentLineNum, FormatNum(MaxCallDepth) & "Invalid_string_refer_to_original_code")
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If ArgIndex + pcount > MaxArgIndex Then
					DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Exit Function
				End If
				
				'å¼•æ•°ã‚’è©•ä¾¡ã—ã¦ãŠã
				For i = 2 To pcount
					params(i) = GetValueAsString(params(i), is_term(i))
				Next 
				
				'Invalid_string_refer_to_original_code
				CallStack(CallDepth) = CurrentLineNum
				ArgIndexStack(CallDepth) = ArgIndex
				VarIndexStack(CallDepth) = VarIndex
				ForIndexStack(CallDepth) = ForIndex
				
				'Invalid_string_refer_to_original_code
				If UpVarLevel > 0 Then
					UpVarLevelStack(CallDepth) = UpVarLevel + UpVarLevelStack(CallDepth - 1)
				Else
					UpVarLevelStack(CallDepth) = 0
				End If
				
				'Invalid_string_refer_to_original_code
				UpVarLevel = 0
				
				'Invalid_string_refer_to_original_code
				CallDepth = CallDepth + 1
				cur_depth = CallDepth
				
				'Invalid_string_refer_to_original_code
				ArgIndex = ArgIndex + pcount - 1
				For i = 2 To pcount
					ArgStack(ArgIndex - i + 2) = params(i)
				Next 
				
				'Invalid_string_refer_to_original_code
				Do 
					CurrentLineNum = ret
					If CurrentLineNum > UBound(EventCmd) Then
						Exit Do
					End If
					With EventCmd(CurrentLineNum)
						If cur_depth = CallDepth And .Name = Event_Renamed.CmdType.ReturnCmd Then
							Exit Do
						End If
						ret = .Exec()
					End With
				Loop While ret > 0
				
				'è¿”ã‚Šå€¤
				With EventCmd(CurrentLineNum)
					If .ArgNum = 2 Then
						str_result = .GetArgAsString(2)
					Else
						str_result = ""
					End If
				End With
				
				'Invalid_string_refer_to_original_code
				CallDepth = CallDepth - 1
				
				'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³å®Ÿè¡Œå‰ã®çŠ¶æ…‹ã«å¾©å¸°
				CurrentLineNum = CallStack(CallDepth)
				ArgIndex = ArgIndexStack(CallDepth)
				VarIndex = VarIndexStack(CallDepth)
				ForIndex = ForIndexStack(CallDepth)
				UpVarLevel = UpVarLevelStack(CallDepth)
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "info"
				For i = 1 To pcount
					params(i) = GetValueAsString(params(i), is_term(i))
				Next 
				str_result = EvalInfoFunc(params)
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "instr"
				If pcount = 2 Then
					i = InStr(GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					i = InStr(GetValueAsLong(params(3), is_term(3)), GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(i)
					CallFunction = ValueType.StringType
				Else
					num_result = i
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "instrb"
				If pcount = 2 Then
					'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'UPGRADE_ISSUE: InStrB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = InStrB(StrConv(GetValueAsString(params(1), is_term(1)), vbFromUnicode), StrConv(GetValueAsString(params(2), is_term(2)), vbFromUnicode))
				Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'UPGRADE_ISSUE: InStrB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = InStrB(GetValueAsLong(params(3), is_term(3)), StrConv(GetValueAsString(params(1), is_term(1)), vbFromUnicode), StrConv(GetValueAsString(params(2), is_term(2)), vbFromUnicode))
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(i)
					CallFunction = ValueType.StringType
				Else
					num_result = i
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "lindex"
				str_result = ListIndex(GetValueAsString(params(1), is_term(1)), GetValueAsLong(params(2), is_term(2)))
				
				'Invalid_string_refer_to_original_code
				If Left(str_result, 1) = "(" And Right(str_result, 1) = ")" Then
					str_result = Mid(str_result, 2, Len(str_result) - 2)
				End If
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "llength"
				i = ListLength(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(i)
					CallFunction = ValueType.StringType
				Else
					num_result = i
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "list"
				str_result = GetValueAsString(params(1), is_term(1))
				For i = 2 To pcount
					str_result = str_result & " " & GetValueAsString(params(i), is_term(i))
				Next 
				CallFunction = ValueType.StringType
				Exit Function
				
				'Invalid_string_refer_to_original_code
			Case "abs"
				num_result = System.Math.Abs(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "action"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).Action
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									With .Unit_Renamed
										'Invalid_string_refer_to_original_code
										'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
										num_result = .Action
									End With
								Else
									num_result = 0
								End If
							End With
						End If
						'End With
						'End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.Action
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "area"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							str_result = UList.Item2(pname).Area
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									str_result = .Unit_Renamed.Area
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.Area
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "asc"
				num_result = Asc(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "atn"
				num_result = System.Math.Atan(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "chr"
				str_result = Chr(GetValueAsLong(params(1), is_term(1)))
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "condition"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						buf = GetValueAsString(params(2), is_term(2))
						If UList.IsDefined2(pname) Then
							If UList.Item2(pname).IsConditionSatisfied(buf) Then
								num_result = 1
							End If
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									If .Unit_Renamed.IsConditionSatisfied(buf) Then
										num_result = 1
									End If
								End If
							End With
						End If
					Case 1
						If Not SelectedUnitForEvent Is Nothing Then
							buf = GetValueAsString(params(1), is_term(1))
							If SelectedUnitForEvent.IsConditionSatisfied(buf) Then
								num_result = 1
							End If
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "count"
				expr = Trim(expr)
				buf = Mid(expr, 7, Len(expr) - 7) & "["
				num = 0
				
				'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°ã‚’æ¤œç´¢
				If CallDepth > 0 Then
					For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
						If InStr(VarStack(i).Name, buf) = 1 Then
							num = num + 1
						End If
					Next 
					If num > 0 Then
						If etype = ValueType.StringType Then
							str_result = FormatNum(num)
							CallFunction = ValueType.StringType
						Else
							num_result = num
							CallFunction = ValueType.NumericType
						End If
						Exit Function
					End If
				End If
				
				'ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°ã‚’æ¤œç´¢
				For	Each var In LocalVariableList
					If InStr(var.Name, buf) = 1 Then
						num = num + 1
					End If
				Next var
				If num > 0 Then
					If etype = ValueType.StringType Then
						str_result = FormatNum(num)
						CallFunction = ValueType.StringType
					Else
						num_result = num
						CallFunction = ValueType.NumericType
					End If
					Exit Function
				End If
				
				'ã‚°ãƒ­ãƒ¼ãƒãƒ«å¤‰æ•°ã‚’æ¤œç´¢
				For	Each var In GlobalVariableList
					If InStr(var.Name, buf) = 1 Then
						num = num + 1
					End If
				Next var
				If etype = ValueType.StringType Then
					str_result = FormatNum(num)
					CallFunction = ValueType.StringType
				Else
					num_result = num
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "countitem"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num = UList.Item2(pname).CountItem
						ElseIf Not PList.IsDefined(pname) Then 
							If pname = "Invalid_string_refer_to_original_code" Then
								num = 0
								For	Each it In IList
									With it
										If .Unit_Renamed Is Nothing And .Exist Then
											num = num + 1
										End If
									End With
								Next it
							End If
						Else
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num = .Unit_Renamed.CountItem
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num = SelectedUnitForEvent.CountItem
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num)
					CallFunction = ValueType.StringType
				Else
					num_result = num
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "countpartner"
				num_result = UBound(SelectedPartners)
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "countpilot"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							With UList.Item2(pname)
								num_result = .CountPilot + .CountSupport
							End With
						Else
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									With .Unit_Renamed
										num_result = .CountPilot + .CountSupport
									End With
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							With SelectedUnitForEvent
								num_result = .CountPilot + .CountSupport
							End With
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "cos"
				num_result = System.Math.Cos(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "damage"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							With UList.Item2(pname)
								num_result = 100 * (.MaxHP - .HP) \ .MaxHP
							End With
						ElseIf Not PList.IsDefined(pname) Then 
							num_result = 100
						ElseIf PList.Item(pname).Unit_Renamed Is Nothing Then 
							num_result = 100
						Else
							With PList.Item(pname).Unit_Renamed
								num_result = 100 * (.MaxHP - .HP) \ .MaxHP
							End With
						End If
					Case 0
						With SelectedUnitForEvent
							num_result = 100 * (.MaxHP - .HP) \ .MaxHP
						End With
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "dir"
				CallFunction = ValueType.StringType
				Select Case pcount
					Case 2
						fname = GetValueAsString(params(1), is_term(1))
						
						'Invalid_string_refer_to_original_code
						If Mid(fname, 2, 1) <> ":" Then
							fname = ScenarioPath & fname
						End If
						
						Select Case GetValueAsString(params(2), is_term(2))
							Case "ãƒ•ã‚¡ã‚¤ãƒ«"
								'UPGRADE_ISSUE: vbNormal ‚ğƒAƒbƒvƒOƒŒ[ƒh‚·‚é’è”‚ğŒˆ’è‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								num = vbNormal
							Case "ãƒ•ã‚©ãƒ«ãƒ€"
								num = FileAttribute.Directory
						End Select
						'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = Dir(fname, num)
						
						If Len(str_result) = 0 Then
							Exit Function
						End If
						
						'Invalid_string_refer_to_original_code
						dir_path = fname
						If num = FileAttribute.Directory Then
							i = InStr2(fname, "\")
							If i > 0 Then
								dir_path = Left(fname, i)
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						If InStr(fname, "*") = 0 Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							If num = FileAttribute.Directory Then
								If (GetAttr(dir_path & str_result) And num) = 0 Then
									str_result = ""
								End If
							End If
							Exit Function
						End If
						
						If str_result = "." Then
							'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = Dir()
						End If
						If str_result = ".." Then
							'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = Dir()
						End If
						
						'Invalid_string_refer_to_original_code
						ReDim dir_list(0)
						If num = FileAttribute.Directory Then
							Do While Len(str_result) > 0
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								If (GetAttr(dir_path & str_result) And num) <> 0 Then
									ReDim Preserve dir_list(UBound(dir_list) + 1)
									dir_list(UBound(dir_list)) = str_result
								End If
								'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								str_result = Dir()
							Loop 
						Else
							Do While Len(str_result) > 0
								ReDim Preserve dir_list(UBound(dir_list) + 1)
								dir_list(UBound(dir_list)) = str_result
								'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								str_result = Dir()
							Loop 
						End If
						
						If UBound(dir_list) > 0 Then
							str_result = dir_list(1)
							dir_index = 2
						Else
							str_result = ""
							dir_index = 1
						End If
						
					Case 1
						fname = GetValueAsString(params(1), is_term(1))
						
						'Invalid_string_refer_to_original_code
						If Mid(fname, 2, 1) <> ":" Then
							fname = ScenarioPath & fname
						End If
						
						'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = Dir(fname, FileAttribute.Directory)
						
						If Len(str_result) = 0 Then
							Exit Function
						End If
						
						'Invalid_string_refer_to_original_code
						If InStr(fname, "*") = 0 Then
							Exit Function
						End If
						
						If str_result = "." Then
							'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = Dir()
						End If
						If str_result = ".." Then
							'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = Dir()
						End If
						
						'Invalid_string_refer_to_original_code
						ReDim dir_list(0)
						Do While Len(str_result) > 0
							ReDim Preserve dir_list(UBound(dir_list) + 1)
							dir_list(UBound(dir_list)) = str_result
							'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = Dir()
						Loop 
						
						If UBound(dir_list) > 0 Then
							str_result = dir_list(1)
							dir_index = 2
						Else
							str_result = ""
							dir_index = 1
						End If
						
					Case 0
						If dir_index <= UBound(dir_list) Then
							str_result = dir_list(dir_index)
							dir_index = dir_index + 1
						Else
							str_result = ""
						End If
				End Select
				Exit Function
				
			Case "eof"
				If etype = ValueType.StringType Then
					If EOF(GetValueAsLong(params(1), is_term(1))) Then
						str_result = "1"
					Else
						str_result = "0"
					End If
					CallFunction = ValueType.StringType
				Else
					If EOF(GetValueAsLong(params(1), is_term(1))) Then
						num_result = 1
					End If
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "en"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).EN
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num_result = .Unit_Renamed.EN
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.EN
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "eval"
				buf = Trim(GetValueAsString(params(1), is_term(1)))
				CallFunction = EvalExpr(buf, etype, str_result, num_result)
				Exit Function
				
			Case "font"
				Select Case GetValueAsString(params(1), is_term(1))
					Case "ãƒ•ã‚©ãƒ³ãƒˆå"
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = MainForm.picMain(0).Font.Name
						CallFunction = ValueType.StringType
					Case "ã‚µã‚¤ã‚º"
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num_result = MainForm.picMain(0).Font.Size
						If etype = ValueType.StringType Then
							str_result = FormatNum(num_result)
							CallFunction = ValueType.StringType
						Else
							CallFunction = ValueType.NumericType
						End If
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If MainForm.picMain(0).Font.Bold Then
							num_result = 1
						Else
							num_result = 0
						End If
						If etype = ValueType.StringType Then
							str_result = FormatNum(num_result)
							CallFunction = ValueType.StringType
						Else
							CallFunction = ValueType.NumericType
						End If
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If MainForm.picMain(0).Font.Italic Then
							num_result = 1
						Else
							num_result = 0
						End If
						If etype = ValueType.StringType Then
							str_result = FormatNum(num_result)
							CallFunction = ValueType.StringType
						Else
							CallFunction = ValueType.NumericType
						End If
					Case "è‰²"
						'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = Hex(MainForm.picMain(0).ForeColor)
						For i = 1 To 6 - Len(str_result)
							str_result = "0" & str_result
						Next 
						str_result = "#" & str_result
						CallFunction = ValueType.StringType
					Case "æ›¸ãè¾¼ã¿"
						If PermanentStringMode Then
							str_result = "èƒŒæ™¯"
						ElseIf KeepStringMode Then 
							str_result = "ä¿æŒ"
						Else
							str_result = "é€šå¸¸"
						End If
						CallFunction = ValueType.StringType
				End Select
				Exit Function
				
			Case "format"
				str_result = VB6.Format(GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "keystate"
				
				If pcount <> 1 Then
					Exit Function
				End If
				
				'ã‚­ãƒ¼ç•ªå·
				i = GetValueAsLong(params(1), is_term(1))
				
				'Invalid_string_refer_to_original_code
				Select Case i
					Case System.Windows.Forms.Keys.LButton
						i = LButtonID
					Case System.Windows.Forms.Keys.RButton
						i = RButtonID
				End Select
				
				If i = System.Windows.Forms.Keys.LButton Or i = System.Windows.Forms.Keys.RButton Then
					'ãƒã‚¦ã‚¹ã‚«ãƒ¼ã‚½ãƒ«ã®ä½ç½®ã‚’å‚ç…§
					GetCursorPos(PT)
					
					'Invalid_string_refer_to_original_code
					If System.Windows.Forms.Form.ActiveForm Is MainForm Then
						With MainForm
							'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							x1 = VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX + .picMain(0).Left + 3
							'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							y1 = VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY + .picMain(0).Top + 28
							'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							x2 = x1 + .picMain(0).Width
							'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							y2 = y1 + .picMain(0).Height
						End With
						
						With PT
							If x1 <= .x And .x <= x2 And y1 <= .y And .y <= y2 Then
								in_window = True
							End If
						End With
					End If
				Else
					'Invalid_string_refer_to_original_code
					If System.Windows.Forms.Form.ActiveForm Is MainForm Then
						in_window = True
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				If Not in_window Then
					num_result = 0
					If etype = ValueType.StringType Then
						str_result = "0"
						CallFunction = ValueType.StringType
					Else
						CallFunction = ValueType.NumericType
					End If
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If GetAsyncKeyState(i) And &H8000 Then
					num_result = 1
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "gettime"
				num_result = timeGetTime()
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "hp"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).HP
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num_result = .Unit_Renamed.HP
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.HP
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "iif"
				
				num = ListSplit(params(1), list)
				
				Select Case num
					Case 1
						If PList.IsDefined(list(1)) Then
							With PList.Item(list(1))
								If .Unit_Renamed Is Nothing Then
									flag = False
								End If
							End With
						End If
						'End With
						If GetValueAsLong(params(1)) <> 0 Then
							flag = True
						Else
							flag = False
						End If
						'End If
					Case 2
						pname = ListIndex(expr, 2)
						If PList.IsDefined(list(2)) Then
							With PList.Item(list(2))
								If .Unit_Renamed Is Nothing Then
									flag = True
								End If
							End With
						End If
						'End With
						If GetValueAsLong(params(1), True) = 0 Then
							flag = True
						Else
							flag = False
						End If
						'End If
					Case Else
						If GetValueAsLong(params(1)) <> 0 Then
							flag = True
						Else
							flag = False
						End If
				End Select
				
				If flag Then
					str_result = GetValueAsString(params(2), is_term(2))
				Else
					str_result = GetValueAsString(params(3), is_term(3))
				End If
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "instrrev"
				buf = GetValueAsString(params(1), is_term(1))
				buf2 = GetValueAsString(params(2), is_term(2))
				
				If Len(buf2) > 0 And Len(buf) >= Len(buf2) Then
					If pcount = 2 Then
						num = Len(buf)
					Else
						num = GetValueAsLong(params(3), is_term(3))
					End If
					
					i = num - Len(buf2) + 1
					Do 
						j = InStr(i, buf, buf2)
						If i = j Then
							Exit Do
						End If
						i = i - 1
					Loop Until i = 0
				Else
					i = 0
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(i)
					CallFunction = ValueType.StringType
				Else
					num_result = i
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "instrrevb"
				buf = GetValueAsString(params(1), is_term(1))
				buf2 = GetValueAsString(params(2), is_term(2))
				
				'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If LenB(buf2) > 0 And LenB(buf) >= LenB(buf2) Then
					If pcount = 2 Then
						'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = LenB(buf)
					Else
						num = GetValueAsLong(params(3), is_term(3))
					End If
					
					'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					i = num - LenB(buf2) + 1
					Do 
						'UPGRADE_ISSUE: InStrB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						j = InStrB(i, buf, buf2)
						If i = j Then
							Exit Do
						End If
						i = i - 1
					Loop Until i = 0
				Else
					i = 0
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(i)
					CallFunction = ValueType.StringType
				Else
					num_result = i
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "int"
				num_result = Int(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "isavailable"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						buf = GetValueAsString(params(2), is_term(2))
						
						'Invalid_string_refer_to_original_code
						If ALDList.IsDefined(buf) Then
							With ALDList.Item(buf)
								For i = 1 To .Count
									If LIndex(.AliasData(i), 1) = buf Then
										buf = .AliasType(i)
										Exit For
									End If
								Next 
								If i > .Count Then
									buf = .AliasType(1)
								End If
							End With
						End If
						
						If UList.IsDefined2(pname) Then
							If UList.Item2(pname).IsFeatureAvailable(buf) Then
								num_result = 1
							End If
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									If .Unit_Renamed.IsFeatureAvailable(buf) Then
										num_result = 1
									End If
								End If
							End With
						End If
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						
						'Invalid_string_refer_to_original_code
						If ALDList.IsDefined(buf) Then
							buf = ALDList.Item(buf).AliasType(1)
						End If
						
						If Not SelectedUnitForEvent Is Nothing Then
							If SelectedUnitForEvent.IsFeatureAvailable(buf) Then
								num_result = 1
							End If
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "isdefined"
				pname = GetValueAsString(params(1), is_term(1))
				Select Case pcount
					Case 2
						Select Case GetValueAsString(params(2), is_term(2))
							Case "Invalid_string_refer_to_original_code"
								If PList.IsDefined(pname) Then
									If PList.Item(pname).Alive Then
										num_result = 1
									End If
								End If
							Case "Invalid_string_refer_to_original_code"
								If UList.IsDefined(pname) Then
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									num_result = 1
								End If
								'End If
							Case "Invalid_string_refer_to_original_code"
								If IList.IsDefined(pname) Then
									num_result = 1
								End If
						End Select
					Case 1
						If PList.IsDefined(pname) Then
							If PList.Item(pname).Alive Then
								num_result = 1
							End If
						ElseIf UList.IsDefined(pname) Then 
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = 1
						End If
						IList.IsDefined(pname)
						num_result = 1
						'End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "isequiped"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							If UList.Item2(pname).IsEquiped(GetValueAsString(params(2), is_term(2))) Then
								num_result = 1
							End If
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									If .Unit_Renamed.IsEquiped(GetValueAsString(params(2), is_term(2))) Then
										num_result = 1
									End If
								End If
							End With
						End If
					Case 1
						If Not SelectedUnitForEvent Is Nothing Then
							If SelectedUnitForEvent.IsEquiped(GetValueAsString(params(1), is_term(1))) Then
								num_result = 1
							End If
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "lsearch"
				buf = GetValueAsString(params(1), is_term(1))
				buf2 = GetValueAsString(params(2), is_term(2))
				num = IIf(pcount < 3, 1, GetValueAsLong(params(3), is_term(3)))
				num2 = ListLength(buf)
				
				For i = num To num2
					If ListIndex(buf, i) = buf2 Then
						If etype = ValueType.StringType Then
							str_result = VB6.Format(i)
							CallFunction = ValueType.StringType
						Else
							num_result = i
							CallFunction = ValueType.NumericType
						End If
						Exit Function
					End If
				Next 
				
				If etype = ValueType.StringType Then
					str_result = "0"
					CallFunction = ValueType.StringType
				Else
					num_result = 0
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "isnumeric"
				If IsNumber(GetValueAsString(params(1), is_term(1))) Then
					If etype = ValueType.StringType Then
						str_result = "1"
						CallFunction = ValueType.StringType
					Else
						num_result = 1
						CallFunction = ValueType.NumericType
					End If
				Else
					If etype = ValueType.StringType Then
						str_result = "0"
						CallFunction = ValueType.StringType
					Else
						num_result = 0
						CallFunction = ValueType.NumericType
					End If
				End If
				Exit Function
				
			Case "isvardefined"
				If IsVariableDefined(Trim(Mid(expr, 14, Len(expr) - 14))) Then
					If etype = ValueType.StringType Then
						str_result = "1"
						CallFunction = ValueType.StringType
					Else
						num_result = 1
						CallFunction = ValueType.NumericType
					End If
				Else
					If etype = ValueType.StringType Then
						str_result = "0"
						CallFunction = ValueType.StringType
					Else
						num_result = 0
						CallFunction = ValueType.NumericType
					End If
				End If
				Exit Function
				
			Case "item"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							i = GetValueAsLong(params(2), is_term(2))
							With UList.Item2(pname)
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).Name
								End If
							End With
						ElseIf Not PList.IsDefined(pname) Then 
							If pname = "Invalid_string_refer_to_original_code" Then
								i = 0
								j = GetValueAsLong(params(2), is_term(2))
								For	Each it In IList
									With it
										If .Unit_Renamed Is Nothing And .Exist Then
											i = i + 1
											If i = j Then
												str_result = .Name
												Exit For
											End If
										End If
									End With
								Next it
							End If
						ElseIf Not PList.Item(pname).Unit_Renamed Is Nothing Then 
							i = GetValueAsLong(params(2), is_term(2))
							With PList.Item(pname).Unit_Renamed
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).Name
								End If
							End With
						End If
					Case 1
						If Not SelectedUnitForEvent Is Nothing Then
							i = GetValueAsLong(params(1), is_term(1))
							With SelectedUnitForEvent
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).Name
								End If
							End With
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "itemid"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							i = GetValueAsLong(params(2), is_term(2))
							With UList.Item2(pname)
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).ID
								End If
							End With
						ElseIf Not PList.IsDefined(pname) Then 
							If pname = "Invalid_string_refer_to_original_code" Then
								i = 0
								j = GetValueAsLong(params(2), is_term(2))
								For	Each it In IList
									With it
										If .Unit_Renamed Is Nothing And .Exist Then
											i = i + 1
											If i = j Then
												str_result = .ID
												Exit For
											End If
										End If
									End With
								Next it
							End If
						ElseIf Not PList.Item(pname).Unit_Renamed Is Nothing Then 
							i = GetValueAsLong(params(2), is_term(2))
							With PList.Item(pname).Unit_Renamed
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).ID
								End If
							End With
						End If
					Case 1
						If Not SelectedUnitForEvent Is Nothing Then
							i = GetValueAsLong(params(1), is_term(1))
							With SelectedUnitForEvent
								If 1 <= i And i <= .CountItem Then
									str_result = .Item(i).ID
								End If
							End With
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "left"
				str_result = Left(GetValueAsString(params(1), is_term(1)), GetValueAsLong(params(2), is_term(2)))
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "leftb"
				buf = GetValueAsString(params(1), is_term(1))
				'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_ISSUE: LeftB$ ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				str_result = LeftB$(StrConv(buf, vbFromUnicode), GetValueAsLong(params(2), is_term(2)))
				'UPGRADE_ISSUE: ’è” vbUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				str_result = StrConv(str_result, vbUnicode)
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "len"
				num_result = Len(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "lenb"
				buf = GetValueAsString(params(1), is_term(1))
				'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				num_result = LenB(StrConv(buf, vbFromUnicode))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "level"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num = UList.Item2(pname).MainPilot.Level
						ElseIf Not PList.IsDefined(pname) Then 
							num_result = 0
						Else
							num_result = PList.Item(pname).Level
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							With SelectedUnitForEvent
								If .CountPilot > 0 Then
									num_result = .MainPilot.Level
								End If
							End With
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "lcase"
				str_result = LCase(GetValueAsString(params(1), is_term(1)))
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "lset"
				buf = GetValueAsString(params(1), is_term(1))
				i = GetValueAsLong(params(2), is_term(2))
				'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If LenB(StrConv(buf, vbFromUnicode)) < i Then
					'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					str_result = buf & Space(i - LenB(StrConv(buf, vbFromUnicode)))
				Else
					str_result = buf
				End If
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "max"
				num_result = GetValueAsDouble(params(1), is_term(1))
				For i = 2 To pcount
					rdbl = GetValueAsDouble(params(i), is_term(i))
					If num_result < rdbl Then
						num_result = rdbl
					End If
				Next 
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "mid"
				buf = GetValueAsString(params(1), is_term(1))
				Select Case pcount
					Case 3
						i = GetValueAsLong(params(2), is_term(2))
						j = GetValueAsLong(params(3), is_term(3))
						str_result = Mid(buf, i, j)
					Case 2
						i = GetValueAsLong(params(2), is_term(2))
						str_result = Mid(buf, i)
				End Select
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "midb"
				buf = GetValueAsString(params(1), is_term(1))
				Select Case pcount
					Case 3
						i = GetValueAsLong(params(2), is_term(2))
						j = GetValueAsLong(params(3), is_term(3))
						'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						'UPGRADE_ISSUE: MidB$ ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = MidB$(StrConv(buf, vbFromUnicode), i, j)
					Case 2
						i = GetValueAsLong(params(2), is_term(2))
						'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						'UPGRADE_ISSUE: MidB$ ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						str_result = MidB$(StrConv(buf, vbFromUnicode), i)
				End Select
				'UPGRADE_ISSUE: ’è” vbUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				str_result = StrConv(str_result, vbUnicode)
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "min"
				num_result = GetValueAsDouble(params(1), is_term(1))
				For i = 2 To pcount
					rdbl = GetValueAsDouble(params(i), is_term(i))
					If num_result > rdbl Then
						num_result = rdbl
					End If
				Next 
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "morale"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).MainPilot.Morale
						ElseIf PList.IsDefined(pname) Then 
							num_result = PList.Item(pname).Morale
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							With SelectedUnitForEvent
								If .CountPilot > 0 Then
									num_result = .MainPilot.Morale
								End If
							End With
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "nickname"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If PList.IsDefined(buf) Then
							str_result = PList.Item(buf).Nickname
						ElseIf PDList.IsDefined(buf) Then 
							str_result = PDList.Item(buf).Nickname
						ElseIf NPDList.IsDefined(buf) Then 
							str_result = NPDList.Item(buf).Nickname
						ElseIf UList.IsDefined(buf) Then 
							str_result = UList.Item(buf).Nickname0
						ElseIf UDList.IsDefined(buf) Then 
							str_result = UDList.Item(buf).Nickname
						ElseIf IDList.IsDefined(buf) Then 
							str_result = IDList.Item(buf).Nickname
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.Nickname0
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "partner"
				i = GetValueAsLong(params(1), is_term(1))
				If i = 0 Then
					str_result = SelectedUnitForEvent.ID
				ElseIf 1 <= i And i <= UBound(SelectedPartners) Then 
					str_result = SelectedPartners(i).ID
				End If
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "party"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							str_result = UList.Item2(pname).Party0
						ElseIf PList.IsDefined(pname) Then 
							str_result = PList.Item(pname).Party
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.Party0
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "pilot"
				Select Case pcount
					Case 2
						uname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined(uname) Then
							i = GetValueAsLong(params(2), is_term(2))
							With UList.Item(uname)
								If 0 < i And i <= .CountPilot Then
									str_result = .Pilot(i).Name
								ElseIf .CountPilot < i And i <= .CountPilot + .CountSupport Then 
									str_result = .Support(i - .CountPilot).Name
								End If
							End With
						End If
					Case 1
						uname = GetValueAsString(params(1), is_term(1))
						If IsNumber(uname) Then
							If Not SelectedUnitForEvent Is Nothing Then
								i = CShort(uname)
								With SelectedUnitForEvent
									If 0 < i And i <= .CountPilot Then
										str_result = .Pilot(i).Name
									ElseIf .CountPilot < i And i <= .CountPilot + .CountSupport Then 
										str_result = .Support(i - .CountPilot).Name
									End If
								End With
							End If
						ElseIf UList.IsDefined(uname) Then 
							With UList.Item(uname)
								If .CountPilot > 0 Then
									str_result = .MainPilot.Name
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							With SelectedUnitForEvent
								If .CountPilot > 0 Then
									str_result = .MainPilot.Name
								End If
							End With
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "pilotid"
				Select Case pcount
					Case 2
						uname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined(uname) Then
							i = GetValueAsLong(params(2), is_term(2))
							With UList.Item(uname)
								If 0 < i And i <= .CountPilot Then
									str_result = .Pilot(i).ID
								ElseIf .CountPilot < i And i <= .CountPilot + .CountSupport Then 
									str_result = .Support(i - .CountPilot).ID
								End If
							End With
						End If
					Case 1
						uname = GetValueAsString(params(1), is_term(1))
						If IsNumber(uname) Then
							If Not SelectedUnitForEvent Is Nothing Then
								i = CShort(uname)
								With SelectedUnitForEvent
									If 0 < i And i <= .CountPilot Then
										str_result = .Pilot(i).ID
									ElseIf .CountPilot < i And i <= .CountPilot + .CountSupport Then 
										str_result = .Support(i - .CountPilot).ID
									End If
								End With
							End If
						ElseIf UList.IsDefined(uname) Then 
							With UList.Item(uname)
								If .CountPilot > 0 Then
									str_result = .MainPilot.ID
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							With SelectedUnitForEvent
								If .CountPilot > 0 Then
									str_result = .MainPilot.ID
								End If
							End With
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "plana"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).MainPilot.Plana
						ElseIf PList.IsDefined(pname) Then 
							num_result = PList.Item(pname).Plana
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.MainPilot.Plana
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "random"
				num_result = Dice(GetValueAsLong(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "rank"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).Rank
						ElseIf Not PList.IsDefined(pname) Then 
							num_result = 0
						Else
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num_result = .Unit_Renamed.Rank
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.Rank
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "regexp"
				On Error GoTo RegExp_Error
				
				If RegEx Is Nothing Then
					RegEx = CreateObject("VBScript.RegExp")
				End If
				
				'Invalid_string_refer_to_original_code
				buf = ""
				If pcount > 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Global ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					RegEx.Global = True
					'Invalid_string_refer_to_original_code
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.IgnoreCase ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					RegEx.IgnoreCase = False
					If pcount >= 3 Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.IgnoreCase ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						RegEx.IgnoreCase = True
					End If
				End If
				'æ¤œç´¢ãƒ‘ã‚¿ãƒ¼ãƒ³
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Pattern ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				RegEx.Pattern = GetValueAsString(params(2), is_term(2))
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Execute ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Matches = RegEx.Execute(GetValueAsString(params(1), is_term(1)))
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Matches.Count ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Matches.Count = 0 Then
					regexp_index = -1
				Else
					regexp_index = 0
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Matches() ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					buf = Matches(regexp_index)
				End If
				If regexp_index >= 0 Then
					regexp_index = regexp_index + 1
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Matches.Count ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If regexp_index <= Matches.Count - 1 Then
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Matches() ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						buf = Matches(regexp_index)
					End If
				End If
				'End If
				str_result = buf
				CallFunction = ValueType.StringType
				Exit Function
RegExp_Error: 
				DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
				Exit Function
				
			Case "regexpreplace"
				'Invalid_string_refer_to_original_code
				
				On Error GoTo RegExpReplace_Error
				
				If RegEx Is Nothing Then
					RegEx = CreateObject("VBScript.RegExp")
				End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Global ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				RegEx.Global = True
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.IgnoreCase ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				RegEx.IgnoreCase = False
				If pcount >= 4 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.IgnoreCase ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					RegEx.IgnoreCase = True
				End If
				'End If
				'æ¤œç´¢ãƒ‘ã‚¿ãƒ¼ãƒ³
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Pattern ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				RegEx.Pattern = GetValueAsString(params(2), is_term(2))
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg RegEx.Replace ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				buf = RegEx.Replace(GetValueAsString(params(1), is_term(1)), GetValueAsString(params(3), is_term(3)))
				
				str_result = buf
				CallFunction = ValueType.StringType
				Exit Function
RegExpReplace_Error: 
				DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
				Exit Function
				
			Case "relation"
				pname = GetValueAsString(params(1), is_term(1))
				If Not PList.IsDefined(pname) Then
					num_result = 0
					If etype = ValueType.StringType Then
						str_result = "0"
						CallFunction = ValueType.StringType
					Else
						CallFunction = ValueType.NumericType
					End If
					Exit Function
				End If
				pname = PList.Item(pname).Name
				
				pname2 = GetValueAsString(params(2), is_term(2))
				If Not PList.IsDefined(pname2) Then
					num_result = 0
					If etype = ValueType.StringType Then
						str_result = "0"
						CallFunction = ValueType.StringType
					Else
						CallFunction = ValueType.NumericType
					End If
					Exit Function
				End If
				pname2 = PList.Item(pname2).Name
				
				num_result = GetValueAsLong("Invalid_string_refer_to_original_code" & pname & ":" & pname2)
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "replace"
				Select Case pcount
					Case 4
						buf = GetValueAsString(params(1), is_term(1))
						num = GetValueAsLong(params(4), is_term(4))
						buf2 = Right(buf, Len(buf) - num + 1)
						ReplaceString(buf2, GetValueAsString(params(2), is_term(2)), GetValueAsString(params(3), is_term(3)))
						str_result = Left(buf, num - 1) & buf2
					Case 5
						buf = GetValueAsString(params(1), is_term(1))
						num = GetValueAsLong(params(4), is_term(4))
						num2 = GetValueAsLong(params(5), is_term(5))
						buf2 = Mid(buf, num, num2)
						ReplaceString(buf2, GetValueAsString(params(2), is_term(2)), GetValueAsString(params(3), is_term(3)))
						str_result = Left(buf, num - 1) & buf2 & Right(buf, Len(buf) - (num + num2 - 1) - 1)
					Case Else
						str_result = GetValueAsString(params(1), is_term(1))
						ReplaceString(str_result, GetValueAsString(params(2), is_term(2)), GetValueAsString(params(3), is_term(3)))
				End Select
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "rgb"
				buf = Hex(RGB(GetValueAsLong(params(1), is_term(1)), GetValueAsLong(params(2), is_term(2)), GetValueAsLong(params(3), is_term(3))))
				For i = 1 To 6 - Len(buf)
					buf = "0" & buf
				Next 
				str_result = "#000000"
				Mid(str_result, 2, 2) = Mid(buf, 5, 2)
				Mid(str_result, 4, 2) = Mid(buf, 3, 2)
				Mid(str_result, 6, 2) = Mid(buf, 1, 2)
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "right"
				str_result = Right(GetValueAsString(params(1), is_term(1)), GetValueAsLong(params(2), is_term(2)))
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "rightb"
				buf = GetValueAsString(params(1), is_term(1))
				'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_ISSUE: RightB$ ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				str_result = RightB$(StrConv(buf, vbFromUnicode), GetValueAsLong(params(2), is_term(2)))
				'UPGRADE_ISSUE: ’è” vbUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				str_result = StrConv(str_result, vbUnicode)
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "round", "rounddown", "roundup"
				ldbl = GetValueAsDouble(params(1), is_term(1))
				If pcount = 1 Then
					num2 = 0
				Else
					num2 = GetValueAsLong(params(2), is_term(2))
				End If
				
				num = Int(ldbl * 10 ^ num2)
				Select Case LCase(fname)
					Case "round"
						If (ldbl * 10 ^ num2 - num) >= 0.5 Then
							num = num + 1
						End If
					Case "roundup"
						If (ldbl * 10 ^ num2 - num) > 0 Then
							num = num + 1
						End If
				End Select
				num_result = num / 10 ^ num2
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "rset"
				buf = GetValueAsString(params(1), is_term(1))
				i = GetValueAsLong(params(2), is_term(2))
				'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If LenB(StrConv(buf, vbFromUnicode)) < i Then
					'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					str_result = Space(i - LenB(StrConv(buf, vbFromUnicode))) & buf
				Else
					str_result = buf
				End If
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "sin"
				num_result = System.Math.Sin(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "skill"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						buf = GetValueAsString(params(2), is_term(2))
						
						'Invalid_string_refer_to_original_code
						If ALDList.IsDefined(buf) Then
							buf = ALDList.Item(buf).AliasType(1)
						End If
						
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).MainPilot.SkillLevel(buf)
						ElseIf PList.IsDefined(pname) Then 
							num_result = PList.Item(pname).SkillLevel(buf)
						End If
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						
						'Invalid_string_refer_to_original_code
						If ALDList.IsDefined(buf) Then
							buf = ALDList.Item(buf).AliasType(1)
						End If
						
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.MainPilot.SkillLevel(buf)
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "sp"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							num_result = UList.Item2(pname).MainPilot.SP
						ElseIf PList.IsDefined(pname) Then 
							num_result = PList.Item(pname).SP
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.MainPilot.SP
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "specialpower", "mind"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(1), is_term(1))
						buf = GetValueAsString(params(2), is_term(2))
						If UList.IsDefined2(pname) Then
							If UList.Item2(pname).IsSpecialPowerInEffect(buf) Then
								num_result = 1
							End If
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									If .Unit_Renamed.IsSpecialPowerInEffect(buf) Then
										num_result = 1
									End If
								End If
							End With
						End If
					Case 1
						If Not SelectedUnitForEvent Is Nothing Then
							buf = GetValueAsString(params(1), is_term(1))
							If SelectedUnitForEvent.IsSpecialPowerInEffect(buf) Then
								num_result = 1
							End If
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "sqr"
				num_result = System.Math.Sqrt(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "status"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							str_result = UList.Item2(pname).Status_Renamed
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									str_result = .Unit_Renamed.Status_Renamed
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.Status_Renamed
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "strcomp"
				num_result = StrComp(GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "string"
				buf = GetValueAsString(params(2), is_term(2))
				If Len(buf) <= 1 Then
					str_result = New String(buf, GetValueAsLong(params(1), is_term(1)))
				Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					str_result = ""
					For i = 1 To GetValueAsLong(params(1), is_term(1))
						str_result = str_result & buf
					Next 
				End If
				
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(str_result)
					CallFunction = ValueType.NumericType
				Else
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "tan"
				num_result = System.Math.Tan(GetValueAsDouble(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "term"
				Select Case pcount
					Case 2
						pname = GetValueAsString(params(2), is_term(2))
						If UList.IsDefined2(pname) Then
							str_result = Term(GetValueAsString(params(1), is_term(1)), UList.Item2(pname))
						Else
							str_result = Term(GetValueAsString(params(1), is_term(1)))
						End If
					Case 1
						str_result = Term(GetValueAsString(params(1), is_term(1)))
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "textheight"
				'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				num_result = MainForm.picMain(0).TextHeight(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "textwidth"
				'UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				num_result = MainForm.picMain(0).TextWidth(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "trim"
				str_result = Trim(GetValueAsString(params(1), is_term(1)))
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "unit"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							str_result = UList.Item2(pname).Name
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									str_result = .Unit_Renamed.Name
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.Name
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "unitid"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If UList.IsDefined2(pname) Then
							str_result = UList.Item2(pname).ID
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									str_result = .Unit_Renamed.ID
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							str_result = SelectedUnitForEvent.ID
						End If
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "x"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						Select Case pname
							Case "ç›®æ¨™åœ°ç‚¹"
								num_result = SelectedX
							Case "ãƒã‚¦ã‚¹"
								num_result = MouseX
							Case Else
								If UList.IsDefined2(pname) Then
									num_result = UList.Item2(pname).x
								ElseIf PList.IsDefined(pname) Then 
									With PList.Item(pname)
										If Not .Unit_Renamed Is Nothing Then
											num_result = .Unit_Renamed.x
										End If
									End With
								End If
						End Select
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.x
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "y"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						Select Case pname
							Case "ç›®æ¨™åœ°ç‚¹"
								num_result = SelectedY
							Case "ãƒã‚¦ã‚¹"
								num_result = MouseY
							Case Else
								If UList.IsDefined2(pname) Then
									num_result = UList.Item2(pname).y
								ElseIf PList.IsDefined(pname) Then 
									With PList.Item(pname)
										If Not .Unit_Renamed Is Nothing Then
											num_result = .Unit_Renamed.y
										End If
									End With
								End If
						End Select
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.y
						End If
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				'ADD START 240a
			Case "windowwidth"
				If etype = ValueType.NumericType Then
					num_result = MainPWidth
					CallFunction = ValueType.NumericType
				ElseIf etype = ValueType.StringType Then 
					str_result = CStr(MainPWidth)
					CallFunction = ValueType.StringType
				End If
				Exit Function
				
			Case "windowheight"
				If etype = ValueType.NumericType Then
					num_result = MainPHeight
					CallFunction = ValueType.NumericType
				ElseIf etype = ValueType.StringType Then 
					str_result = CStr(MainPHeight)
					CallFunction = ValueType.StringType
				End If
				Exit Function
				'ADD  END  240a
			Case "wx"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If IsNumber(pname) Then
							num_result = StrToLng(pname)
						ElseIf pname = "ç›®æ¨™åœ°ç‚¹" Then 
							num_result = SelectedX
						ElseIf UList.IsDefined2(pname) Then 
							num_result = UList.Item2(pname).x
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num_result = .Unit_Renamed.x
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.x
						End If
				End Select
				
				num_result = MapToPixelX(num_result)
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "wy"
				Select Case pcount
					Case 1
						pname = GetValueAsString(params(1), is_term(1))
						If IsNumber(pname) Then
							num_result = StrToLng(pname)
						ElseIf pname = "ç›®æ¨™åœ°ç‚¹" Then 
							num_result = SelectedY
						ElseIf UList.IsDefined2(pname) Then 
							num_result = UList.Item2(pname).y
						ElseIf PList.IsDefined(pname) Then 
							With PList.Item(pname)
								If Not .Unit_Renamed Is Nothing Then
									num_result = .Unit_Renamed.y
								End If
							End With
						End If
					Case 0
						If Not SelectedUnitForEvent Is Nothing Then
							num_result = SelectedUnitForEvent.y
						End If
				End Select
				
				num_result = MapToPixelY(num_result)
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "wide"
				str_result = StrConv(GetValueAsString(params(1), is_term(1)), VbStrConv.Wide)
				CallFunction = ValueType.StringType
				Exit Function
				
				'Invalid_string_refer_to_original_code
			Case "year"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = Year(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = Year(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "month"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = Month(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = Month(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "weekday"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							Select Case WeekDay(CDate(buf))
								Case FirstDayOfWeek.Sunday
									str_result = "Invalid_string_refer_to_original_code"
								Case FirstDayOfWeek.Monday
									str_result = "æœˆæ›œ"
								Case FirstDayOfWeek.Tuesday
									str_result = "Invalid_string_refer_to_original_code"
								Case FirstDayOfWeek.Wednesday
									str_result = "Invalid_string_refer_to_original_code"
								Case FirstDayOfWeek.Thursday
									str_result = "Invalid_string_refer_to_original_code"
								Case FirstDayOfWeek.Friday
									str_result = "é‡‘æ›œ"
								Case FirstDayOfWeek.Saturday
									str_result = "åœŸæ›œ"
							End Select
						End If
					Case 0
						Select Case WeekDay(Now)
							Case FirstDayOfWeek.Sunday
								str_result = "Invalid_string_refer_to_original_code"
							Case FirstDayOfWeek.Monday
								str_result = "æœˆæ›œ"
							Case FirstDayOfWeek.Tuesday
								str_result = "Invalid_string_refer_to_original_code"
							Case FirstDayOfWeek.Wednesday
								str_result = "Invalid_string_refer_to_original_code"
							Case FirstDayOfWeek.Thursday
								str_result = "Invalid_string_refer_to_original_code"
							Case FirstDayOfWeek.Friday
								str_result = "é‡‘æ›œ"
							Case FirstDayOfWeek.Saturday
								str_result = "åœŸæ›œ"
						End Select
				End Select
				CallFunction = ValueType.StringType
				Exit Function
				
			Case "day"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = VB.Day(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = VB.Day(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "hour"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = Hour(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = Hour(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "minute"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = Minute(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = Minute(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "second"
				Select Case pcount
					Case 1
						buf = GetValueAsString(params(1), is_term(1))
						If IsDate(buf) Then
							num_result = Second(CDate(buf))
						Else
							num_result = 0
						End If
					Case 0
						num_result = Second(Now)
				End Select
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "difftime"
				If pcount = 2 Then
					If params(1) = "Now" Then
						d1 = Now
					Else
						buf = GetValueAsString(params(1), is_term(1))
						If Not IsDate(buf) Then
							Exit Function
						End If
						d1 = CDate(buf)
					End If
					
					If params(2) = "Now" Then
						d2 = Now
					Else
						buf = GetValueAsString(params(2), is_term(2))
						If Not IsDate(buf) Then
							Exit Function
						End If
						d2 = CDate(buf)
					End If
					
					num_result = Second(System.Date.FromOADate(d2.ToOADate - d1.ToOADate))
				End If
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
				'ãƒ€ã‚¤ã‚¢ãƒ­ã‚°è¡¨ç¤º
			Case "loadfiledialog"
				Select Case pcount
					Case 2
						'Invalid_string_refer_to_original_code_
						'ScenarioPath, "", 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Case 3
						'Invalid_string_refer_to_original_code_
						'ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Case 4
						'Invalid_string_refer_to_original_code_
						'ScenarioPath & GetValueAsString(params(4), is_term(4)), _
						'GetValueAsString(params(3), is_term(3)), 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End Select
				
				CallFunction = ValueType.StringType
				
				'Invalid_string_refer_to_original_code
				If InStr(str_result, ScenarioPath) > 0 Then
					str_result = Mid(str_result, Len(ScenarioPath) + 1)
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If Right(Left(str_result, 3), 2) = ":\" Then
					str_result = ""
					Exit Function
				End If
				
				'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Do Until Dir(ScenarioPath & str_result, FileAttribute.Normal) <> ""
					If InStr(str_result, "\") = 0 Then
						'Invalid_string_refer_to_original_code
						str_result = ""
						Exit Function
					End If
					str_result = Mid(str_result, InStr(str_result, "\") + 1)
				Loop 
				Exit Function
				
			Case "savefiledialog"
				Select Case pcount
					Case 2
						'Invalid_string_refer_to_original_code_
						'ScenarioPath, "", 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Case 3
						'Invalid_string_refer_to_original_code_
						'ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Case 4
						'Invalid_string_refer_to_original_code_
						'ScenarioPath & GetValueAsString(params(4), is_term(4)), _
						'GetValueAsString(params(3), is_term(3)), 2, _
						'GetValueAsString(params(1), is_term(1)), _
						'GetValueAsString(params(2), is_term(2)))
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End Select
				
				CallFunction = ValueType.StringType
				
				'Invalid_string_refer_to_original_code
				If InStr(str_result, ScenarioPath) > 0 Then
					str_result = Mid(str_result, Len(ScenarioPath) + 1)
					Exit Function
				End If
				
				If InStr(str_result, "\") = 0 Then
					Exit Function
				End If
				
				For i = 1 To Len(str_result)
					If Mid(str_result, Len(str_result) - i + 1, 1) = "\" Then
						Exit For
					End If
				Next 
				buf = Left(str_result, Len(str_result) - i)
				str_result = Mid(str_result, Len(str_result) - i + 2)
				
				Do While InStr(buf, "\") > 0
					buf = Mid(buf, InStr(buf, "\") + 1)
					'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If Dir(ScenarioPath & buf, FileAttribute.Directory) <> "" Then
						str_result = buf & "\" & str_result
						Exit Function
					End If
				Loop 
				
				'UPGRADE_WARNING: Dir ‚ÉV‚µ‚¢“®ì‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Dir(ScenarioPath & buf, FileAttribute.Directory) <> "" Then
					str_result = buf & "\" & str_result
				End If
				Exit Function
				
		End Select
		
LookUpUserDefinedID: 
		'Invalid_string_refer_to_original_code
		ret = FindNormalLabel(fname)
		If ret > 0 Then
			'Invalid_string_refer_to_original_code
			ret = ret + 1
			
			'Invalid_string_refer_to_original_code
			If CallDepth > MaxCallDepth Then
				CallDepth = MaxCallDepth
				DisplayEventErrorMessage(CurrentLineNum, FormatNum(MaxCallDepth) & "Invalid_string_refer_to_original_code")
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If ArgIndex + pcount > MaxArgIndex Then
				DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			For i = 1 To pcount
				params(i) = GetValueAsString(params(i), is_term(i))
			Next 
			
			'Invalid_string_refer_to_original_code
			CallStack(CallDepth) = CurrentLineNum
			ArgIndexStack(CallDepth) = ArgIndex
			VarIndexStack(CallDepth) = VarIndex
			ForIndexStack(CallDepth) = ForIndex
			UpVarLevelStack(CallDepth) = UpVarLevel
			
			'Invalid_string_refer_to_original_code
			UpVarLevel = 0
			
			'Invalid_string_refer_to_original_code
			CallDepth = CallDepth + 1
			cur_depth = CallDepth
			
			'Invalid_string_refer_to_original_code
			ArgIndex = ArgIndex + pcount
			For i = 1 To pcount
				ArgStack(ArgIndex - i + 1) = params(i)
			Next 
			
			'Invalid_string_refer_to_original_code
			Do 
				CurrentLineNum = ret
				If CurrentLineNum > UBound(EventCmd) Then
					Exit Do
				End If
				With EventCmd(CurrentLineNum)
					If cur_depth = CallDepth And .Name = Event_Renamed.CmdType.ReturnCmd Then
						Exit Do
					End If
					ret = .Exec
				End With
			Loop While ret > 0
			
			'è¿”ã‚Šå€¤
			With EventCmd(CurrentLineNum)
				If .ArgNum > 1 Then
					str_result = .GetArgAsString(2)
				Else
					str_result = ""
				End If
			End With
			
			'Invalid_string_refer_to_original_code
			CallDepth = CallDepth - 1
			
			'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³å®Ÿè¡Œå‰ã®çŠ¶æ…‹ã«å¾©å¸°
			CurrentLineNum = CallStack(CallDepth)
			ArgIndex = ArgIndexStack(CallDepth)
			VarIndex = VarIndexStack(CallDepth)
			ForIndex = ForIndexStack(CallDepth)
			UpVarLevel = UpVarLevelStack(CallDepth)
			
			If etype = ValueType.NumericType Then
				num_result = StrToDbl(str_result)
				CallFunction = ValueType.NumericType
			Else
				CallFunction = ValueType.StringType
			End If
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined(expr) Then
			With GlobalVariableList.Item(expr)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(expr).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = StrToDbl(.StringValue)
						End If
						CallFunction = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(expr).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = FormatNum(.NumericValue)
						End If
						CallFunction = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(expr).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
							CallFunction = ValueType.StringType
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
							CallFunction = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		str_result = expr
		CallFunction = ValueType.StringType
	End Function
	
	'Infoé–¢æ•°ã®è©•ä¾¡
	Private Function EvalInfoFunc(ByRef params() As String) As String
		Dim u As Unit
		Dim ud As UnitData
		Dim p As Pilot
		Dim pd As PilotData
		Dim nd As NonPilotData
		Dim it As Item
		Dim itd As ItemData
		Dim spd As SpecialPowerData
		Dim i, idx, j As Short
		Dim buf As String
		Dim aname As String
		Dim max_value As Integer
		
		EvalInfoFunc = ""
		
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg u ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		u = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg ud ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ud = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg p ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		p = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg pd ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		pd = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg nd ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		nd = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg it ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		it = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg itd ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		itd = Nothing
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg spd ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		spd = Nothing
		
		'Invalid_string_refer_to_original_code
		Select Case params(1)
			Case "Invalid_string_refer_to_original_code"
				u = UList.Item(params(2))
				idx = 3
			Case "ãƒ¦ãƒ‹ãƒƒãƒˆãƒ‡ãƒ¼ã‚¿"
				ud = UDList.Item(params(2))
				idx = 3
			Case "Invalid_string_refer_to_original_code"
				p = PList.Item(params(2))
				idx = 3
			Case "Invalid_string_refer_to_original_code"
				pd = PDList.Item(params(2))
				idx = 3
			Case "éæˆ¦é—˜å“¡"
				nd = NPDList.Item(params(2))
				idx = 3
			Case "Invalid_string_refer_to_original_code"
				If IList.IsDefined(params(2)) Then
					it = IList.Item(params(2))
				Else
					itd = IDList.Item(params(2))
				End If
				idx = 3
			Case "Invalid_string_refer_to_original_code"
				itd = IDList.Item(params(2))
				idx = 3
			Case "ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼"
				spd = SPDList.Item(params(2))
				idx = 3
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				idx = 1
			Case ""
				Exit Function
			Case Else
				u = UList.Item(params(1))
				ud = UDList.Item(params(1))
				p = PList.Item(params(1))
				pd = PDList.Item(params(1))
				nd = NPDList.Item(params(1))
				it = IList.Item(params(1))
				itd = IDList.Item(params(1))
				spd = SPDList.Item(params(1))
				idx = 2
		End Select
		
		'UPGRADE_NOTE: my ‚Í my_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim mx, my_Renamed As Short
		Select Case params(idx)
			Case "åç§°"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Name
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Name
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Name
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Name
				ElseIf Not nd Is Nothing Then 
					EvalInfoFunc = nd.Name
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = it.Name
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Name
				ElseIf Not spd Is Nothing Then 
					EvalInfoFunc = spd.Name
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = u.KanaName
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.KanaName
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.KanaName
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.KanaName
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = it.Data.KanaName
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.KanaName
				ElseIf Not spd Is Nothing Then 
					EvalInfoFunc = spd.KanaName
				End If
			Case "æ„›ç§°"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Nickname0
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Nickname
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Nickname
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Nickname
				ElseIf Not nd Is Nothing Then 
					EvalInfoFunc = nd.Nickname
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = it.Nickname
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Nickname
				End If
			Case "æ€§åˆ¥"
				If Not p Is Nothing Then
					EvalInfoFunc = p.Sex
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Sex
				End If
				Exit Function
			Case "ãƒ¦ãƒ‹ãƒƒãƒˆã‚¯ãƒ©ã‚¹", "æ©Ÿä½“ã‚¯ãƒ©ã‚¹"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Class_Renamed
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Class_Renamed
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Class_Renamed
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Class_Renamed
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					For i = 1 To 4
						Select Case u.Adaption(i)
							Case 5
								EvalInfoFunc = EvalInfoFunc & "S"
							Case 4
								EvalInfoFunc = EvalInfoFunc & "A"
							Case 3
								EvalInfoFunc = EvalInfoFunc & "B"
							Case 2
								EvalInfoFunc = EvalInfoFunc & "C"
							Case 1
								EvalInfoFunc = EvalInfoFunc & "D"
							Case Else
								EvalInfoFunc = EvalInfoFunc & "E"
						End Select
					Next 
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Adaption
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Adaption
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Adaption
				End If
			Case "çµŒé¨“å€¤"
				If Not u Is Nothing Then
					EvalInfoFunc = CStr(u.ExpValue)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = CStr(ud.ExpValue)
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = CStr(p.ExpValue)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = CStr(pd.ExpValue)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Infight)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Infight)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Shooting)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Shooting)
				End If
				Exit Function
			Case "å‘½ä¸­"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Hit)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Hit)
				End If
			Case "å›é¿"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Dodge)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Dodge)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Technique)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Technique)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Intuition)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Intuition)
				End If
			Case "é˜²å¾¡"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Defense)
				End If
			Case "æ ¼é—˜åŸºæœ¬å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightBase)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingBase)
				End If
			Case "å‘½ä¸­åŸºæœ¬å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitBase)
				End If
			Case "å›é¿åŸºæœ¬å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeBase)
				End If
			Case "æŠ€é‡åŸºæœ¬å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueBase)
				End If
			Case "åå¿œåŸºæœ¬å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionBase)
				End If
			Case "æ ¼é—˜ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightMod)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingMod)
				End If
			Case "å‘½ä¸­ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitMod)
				End If
			Case "å›é¿ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeMod)
				End If
			Case "æŠ€é‡ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueMod)
				End If
			Case "åå¿œä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionMod)
				End If
			Case "æ ¼é—˜æ”¯æ´ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightMod2)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingMod2)
				End If
			Case "å‘½ä¸­æ”¯æ´ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitMod2)
				End If
			Case "å›é¿æ”¯æ´ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeMod2)
				End If
			Case "æŠ€é‡æ”¯æ´ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueMod2)
				End If
			Case "åå¿œæ”¯æ´ä¿®æ­£å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionMod2)
				End If
			Case "æ€§æ ¼"
				If Not p Is Nothing Then
					EvalInfoFunc = p.Personality
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Personality
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.MaxSP)
					If p.MaxSP = 0 And Not p.Unit_Renamed Is Nothing Then
						If p Is p.Unit_Renamed.MainPilot Then
							EvalInfoFunc = VB6.Format(p.Unit_Renamed.Pilot(1).MaxSP)
						End If
					End If
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SP)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.SP)
					If p.MaxSP = 0 And Not p.Unit_Renamed Is Nothing Then
						If p Is p.Unit_Renamed.MainPilot Then
							EvalInfoFunc = VB6.Format(p.Unit_Renamed.Pilot(1).SP)
						End If
					End If
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SP)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Bitmap(True)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Bitmap0
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Bitmap(True)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Bitmap0
				ElseIf Not nd Is Nothing Then 
					EvalInfoFunc = nd.Bitmap0
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = p.BGM
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.BGM
				End If
			Case "ãƒ¬ãƒ™ãƒ«"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Level)
				End If
			Case "ç´¯ç©çµŒé¨“å€¤"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Exp)
				End If
			Case "æ°—åŠ›"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Morale)
				End If
			Case "æœ€å¤§éœŠåŠ›", "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.MaxPlana)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SkillLevel(0, "éœŠåŠ›"))
				End If
			Case "éœŠåŠ›", "Invalid_string_refer_to_original_code"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Plana)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SkillLevel(0, "éœŠåŠ›"))
				End If
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.SynchroRate)
				ElseIf Not pd Is Nothing Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
			Case "ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not p Is Nothing Then
					If p.MaxSP = 0 And Not p.Unit_Renamed Is Nothing Then
						If p Is p.Unit_Renamed.MainPilot Then
							p = p.Unit_Renamed.Pilot(1)
						End If
					End If
					With p
						For i = 1 To .CountSpecialPower
							EvalInfoFunc = EvalInfoFunc & " " & .SpecialPower(i)
						Next 
					End With
					EvalInfoFunc = Trim(EvalInfoFunc)
				ElseIf Not pd Is Nothing Then 
					With pd
						For i = 1 To .CountSpecialPower(100)
							EvalInfoFunc = EvalInfoFunc & " " & .SpecialPower(100, i)
						Next 
					End With
					EvalInfoFunc = Trim(EvalInfoFunc)
				End If
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not p Is Nothing Then
					If p.MaxSP = 0 And Not p.Unit_Renamed Is Nothing Then
						If p Is p.Unit_Renamed.MainPilot Then
							p = p.Unit_Renamed.Pilot(1)
						End If
					End If
					If p.IsSpecialPowerAvailable(params(idx + 1)) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not pd Is Nothing Then 
					If pd.IsSpecialPowerAvailable(100, params(idx + 1)) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not p Is Nothing Then
					If p.MaxSP = 0 And Not p.Unit_Renamed Is Nothing Then
						If p Is p.Unit_Renamed.MainPilot Then
							p = p.Unit_Renamed.Pilot(1)
						End If
					End If
					EvalInfoFunc = VB6.Format(p.SpecialPowerCost(params(idx + 1)))
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SpecialPowerCost(params(idx + 1)))
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountFeature)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.CountFeature)
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = CStr(p.CountSkill)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = CStr(LLength(pd.Skill(100)))
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = VB6.Format(it.CountFeature)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.CountFeature)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = u.Feature(CShort(params(idx + 1)))
					End If
				ElseIf Not ud Is Nothing Then 
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = ud.Feature(CShort(params(idx + 1)))
					End If
				ElseIf Not p Is Nothing Then 
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = p.Skill(CShort(params(idx + 1)))
					End If
				ElseIf Not pd Is Nothing Then 
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = LIndex(pd.Skill(100), CShort(params(idx + 1)))
					End If
				ElseIf Not it Is Nothing Then 
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = it.Feature(CShort(params(idx + 1)))
					End If
				ElseIf Not itd Is Nothing Then 
					If IsNumber(params(idx + 1)) Then
						EvalInfoFunc = itd.Feature(CShort(params(idx + 1)))
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If IsNumber(aname) Then
						EvalInfoFunc = u.FeatureName(CShort(params(idx + 1)))
					Else
						EvalInfoFunc = u.FeatureName(aname)
					End If
				ElseIf Not ud Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = ud.FeatureName(CShort(aname))
					Else
						EvalInfoFunc = ud.FeatureName(aname)
					End If
				ElseIf Not p Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = p.SkillName(CShort(aname))
					Else
						EvalInfoFunc = p.SkillName(aname)
					End If
				ElseIf Not pd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = pd.SkillName(100, LIndex(pd.Skill(100), CShort(aname)))
					Else
						EvalInfoFunc = pd.SkillName(100, aname)
					End If
				ElseIf Not it Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = it.FeatureName(CShort(aname))
					Else
						EvalInfoFunc = it.FeatureName(aname)
					End If
				ElseIf Not itd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = itd.FeatureName(CShort(aname))
					Else
						EvalInfoFunc = itd.FeatureName(aname)
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If u.IsFeatureAvailable(aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not ud Is Nothing Then 
					If ud.IsFeatureAvailable(aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not p Is Nothing Then 
					If p.IsSkillAvailable(aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not pd Is Nothing Then 
					If pd.IsSkillAvailable(100, aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not it Is Nothing Then 
					If it.IsFeatureAvailable(aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				ElseIf Not itd Is Nothing Then 
					If itd.IsFeatureAvailable(aname) Then
						EvalInfoFunc = "1"
					Else
						EvalInfoFunc = "0"
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(u.FeatureLevel(CShort(aname)))
					Else
						EvalInfoFunc = VB6.Format(u.FeatureLevel(aname))
					End If
				ElseIf Not ud Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(ud.FeatureLevel(CShort(aname)))
					Else
						EvalInfoFunc = VB6.Format(ud.FeatureLevel(aname))
					End If
				ElseIf Not p Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(p.SkillLevel(CShort(aname)))
					Else
						EvalInfoFunc = VB6.Format(p.SkillLevel(aname))
					End If
				ElseIf Not pd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(pd.SkillLevel(100, LIndex(pd.Skill(100), CShort(aname))))
					Else
						EvalInfoFunc = VB6.Format(pd.SkillLevel(100, aname))
					End If
				ElseIf Not it Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(it.FeatureLevel(CShort(aname)))
					Else
						EvalInfoFunc = VB6.Format(it.FeatureLevel(aname))
					End If
				ElseIf Not itd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = VB6.Format(itd.FeatureLevel(CShort(aname)))
					Else
						EvalInfoFunc = VB6.Format(itd.FeatureLevel(aname))
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If IsNumber(aname) Then
						EvalInfoFunc = u.FeatureData(CShort(aname))
					Else
						EvalInfoFunc = u.FeatureData(aname)
					End If
				ElseIf Not ud Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = ud.FeatureData(CShort(aname))
					Else
						EvalInfoFunc = ud.FeatureData(aname)
					End If
				ElseIf Not p Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = p.SkillData(CShort(aname))
					Else
						EvalInfoFunc = p.SkillData(aname)
					End If
				ElseIf Not pd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = pd.SkillData(100, LIndex(pd.Skill(100), CShort(aname)))
					Else
						EvalInfoFunc = pd.SkillData(100, aname)
					End If
				ElseIf Not it Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = it.FeatureData(CShort(aname))
					Else
						EvalInfoFunc = it.FeatureData(aname)
					End If
				ElseIf Not itd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = itd.FeatureData(CShort(aname))
					Else
						EvalInfoFunc = itd.FeatureData(aname)
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If IsNumber(aname) Then
						EvalInfoFunc = u.FeatureNecessarySkill(CShort(aname))
					Else
						EvalInfoFunc = u.FeatureNecessarySkill(aname)
					End If
				ElseIf Not ud Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = ud.FeatureNecessarySkill(CShort(aname))
					Else
						EvalInfoFunc = ud.FeatureNecessarySkill(aname)
					End If
				ElseIf Not it Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = it.FeatureNecessarySkill(CShort(aname))
					Else
						EvalInfoFunc = it.FeatureNecessarySkill(aname)
					End If
				ElseIf Not itd Is Nothing Then 
					If IsNumber(aname) Then
						EvalInfoFunc = itd.FeatureNecessarySkill(CShort(aname))
					Else
						EvalInfoFunc = itd.FeatureNecessarySkill(aname)
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				aname = params(idx + 1)
				
				'Invalid_string_refer_to_original_code
				If ALDList.IsDefined(aname) Then
					With ALDList.Item(aname)
						For i = 1 To .Count
							If LIndex(.AliasData(i), 1) = aname Then
								aname = .AliasType(i)
								Exit For
							End If
						Next 
						If i > .Count Then
							aname = .AliasType(1)
						End If
					End With
				End If
				
				If Not u Is Nothing Then
					If IsNumber(aname) Then
						EvalInfoFunc = FeatureHelpMessage(u, CShort(aname), False)
					Else
						EvalInfoFunc = FeatureHelpMessage(u, aname, False)
					End If
					If EvalInfoFunc = "" And Not p Is Nothing Then
						EvalInfoFunc = SkillHelpMessage(p, aname)
					End If
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = SkillHelpMessage(p, aname)
					If EvalInfoFunc = "" And Not u Is Nothing Then
						If IsNumber(aname) Then
							EvalInfoFunc = FeatureHelpMessage(u, CShort(aname), False)
						Else
							EvalInfoFunc = FeatureHelpMessage(u, aname, False)
						End If
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Data.PilotNum)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.PilotNum)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountPilot)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.PilotNum)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountSupport)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Data.ItemNum)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.ItemNum)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountItem)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.ItemNum)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					If IsNumber(params(idx + 1)) Then
						i = CShort(params(idx + 1))
						If 0 < i And i <= u.CountItem Then
							EvalInfoFunc = VB6.Format(u.Item(i).Name)
						End If
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					If IsNumber(params(idx + 1)) Then
						i = CShort(params(idx + 1))
						If 0 < i And i <= u.CountItem Then
							EvalInfoFunc = VB6.Format(u.Item(i).ID)
						End If
					End If
				End If
			Case "ç§»å‹•å¯èƒ½åœ°å½¢"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Transportation
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Transportation
				End If
			Case "ç§»å‹•åŠ›"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Speed)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Speed)
				End If
			Case "ã‚µã‚¤ã‚º"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Size
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Size
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = CStr(u.Value)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = CStr(ud.Value)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxHP)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.HP)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.HP)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.HP)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxEN)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.EN)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.EN)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.EN)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Armor)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Armor)
				End If
			Case "é‹å‹•æ€§"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Mobility)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Mobility)
				End If
			Case "æ­¦å™¨æ•°"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountWeapon)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.CountWeapon)
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = VB6.Format(p.Data.CountWeapon)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.CountWeapon)
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = VB6.Format(it.CountWeapon)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.CountWeapon)
				End If
			Case "æ­¦å™¨"
				idx = idx + 1
				If Not u Is Nothing Then
					With u
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						Select Case params(idx)
							Case "", "åç§°"
								EvalInfoFunc = .Weapon(i).Name
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.WeaponPower(i, ""))
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								EvalInfoFunc = VB6.Format(.WeaponMaxRange(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.Weapon(i).MinRange)
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.WeaponPrecision(i))
							Case "æœ€å¤§å¼¾æ•°"
								EvalInfoFunc = VB6.Format(.MaxBullet(i))
							Case "å¼¾æ•°"
								EvalInfoFunc = VB6.Format(.Bullet(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.WeaponENConsumption(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.Weapon(i).NecessaryMorale)
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = .Weapon(i).Adaption
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.WeaponCritical(i))
							Case "å±æ€§"
								EvalInfoFunc = .WeaponClass(i)
							Case "Invalid_string_refer_to_original_code"
								If .IsWeaponClassifiedAs(i, params(idx + 1)) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
								EvalInfoFunc = CStr(.WeaponLevel(i, params(idx + 1)))
							Case "å±æ€§åç§°"
								EvalInfoFunc = AttributeName(u, params(idx + 1), False)
							Case "å±æ€§è§£èª¬"
								EvalInfoFunc = AttributeHelpMessage(u, params(idx + 1), i, False)
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = .Weapon(i).NecessarySkill
							Case "ä½¿ç”¨å¯"
								If .IsWeaponAvailable(i, "Invalid_string_refer_to_original_code") Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "Invalid_string_refer_to_original_code"
								If .IsWeaponMastered(i) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
						End Select
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Power)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "æœ€å¤§å¼¾æ•°", "å¼¾æ•°"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .Adaption
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not p Is Nothing Then 
					With p.Data
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Power)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "æœ€å¤§å¼¾æ•°", "å¼¾æ•°"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .Adaption
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not pd Is Nothing Then 
					With pd
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Power)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "æœ€å¤§å¼¾æ•°", "å¼¾æ•°"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .Adaption
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not it Is Nothing Then 
					With it
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Power)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "æœ€å¤§å¼¾æ•°", "å¼¾æ•°"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .Adaption
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not itd Is Nothing Then 
					With itd
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Power)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "æœ€å¤§å¼¾æ•°", "å¼¾æ•°"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .Adaption
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountAbility)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.CountAbility)
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = VB6.Format(p.Data.CountAbility)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.CountAbility)
				ElseIf Not it Is Nothing Then 
					EvalInfoFunc = VB6.Format(it.CountAbility)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.CountAbility)
				End If
			Case "Invalid_string_refer_to_original_code"
				idx = idx + 1
				If Not u Is Nothing Then
					With u
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						Select Case params(idx)
							Case "", "åç§°"
								EvalInfoFunc = .Ability(i).Name
							Case "åŠ¹æœæ•°"
								EvalInfoFunc = VB6.Format(.Ability(i).CountEffect)
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = .Ability(i).EffectType(j)
							Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
								'Invalid_string_refer_to_original_code
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = VB6.Format(.Ability(i).EffectLevel(j))
							Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
								'Invalid_string_refer_to_original_code
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = .Ability(i).EffectData(j)
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								EvalInfoFunc = VB6.Format(.AbilityMaxRange(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.AbilityMinRange(i))
							Case "æœ€å¤§ä½¿ç”¨å›æ•°"
								EvalInfoFunc = VB6.Format(.MaxStock(i))
							Case "ä½¿ç”¨å›æ•°"
								EvalInfoFunc = VB6.Format(.Stock(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.AbilityENConsumption(i))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(.Ability(i).NecessaryMorale)
							Case "å±æ€§"
								EvalInfoFunc = .Ability(i).Class_Renamed
							Case "Invalid_string_refer_to_original_code"
								If .IsAbilityClassifiedAs(i, params(idx + 1)) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
								EvalInfoFunc = CStr(.AbilityLevel(i, params(idx + 1)))
							Case "å±æ€§åç§°"
								EvalInfoFunc = AttributeName(u, params(idx + 1), True)
							Case "å±æ€§è§£èª¬"
								EvalInfoFunc = AttributeHelpMessage(u, params(idx + 1), i, True)
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = .Ability(i).NecessarySkill
							Case "ä½¿ç”¨å¯"
								If .IsAbilityAvailable(i, "ç§»å‹•å‰") Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "Invalid_string_refer_to_original_code"
								If .IsAbilityMastered(i) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
						End Select
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "åŠ¹æœæ•°"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "æœ€å¤§ä½¿ç”¨å›æ•°", "ä½¿ç”¨å›æ•°"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not p Is Nothing Then 
					With p.Data
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "åŠ¹æœæ•°"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "æœ€å¤§ä½¿ç”¨å›æ•°", "ä½¿ç”¨å›æ•°"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not pd Is Nothing Then 
					With pd
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "åŠ¹æœæ•°"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "æœ€å¤§ä½¿ç”¨å›æ•°", "ä½¿ç”¨å›æ•°"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not it Is Nothing Then 
					With it
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "åŠ¹æœæ•°"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "æœ€å¤§ä½¿ç”¨å›æ•°", "ä½¿ç”¨å›æ•°"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not itd Is Nothing Then 
					With itd
						'Invalid_string_refer_to_original_code
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'Invalid_string_refer_to_original_code
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "åç§°"
									EvalInfoFunc = .Name
								Case "åŠ¹æœæ•°"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
									'Invalid_string_refer_to_original_code
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "æœ€å¤§ä½¿ç”¨å›æ•°", "ä½¿ç”¨å›æ•°"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "å±æ€§"
									EvalInfoFunc = .Class_Renamed
								Case "Invalid_string_refer_to_original_code"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "å±æ€§ãƒ¬ãƒ™ãƒ«"
									j = InStrNotNest(.Class_Renamed, params(idx + 1) & "L")
									If j = 0 Then
										EvalInfoFunc = "0"
										Exit Function
									End If
									
									EvalInfoFunc = ""
									j = j + Len(params(idx + 1)) + 1
									Do 
										EvalInfoFunc = EvalInfoFunc & Mid(.Class_Renamed, j, 1)
										j = j + 1
									Loop While IsNumber(Mid(.Class_Renamed, j, 1))
									
									If Not IsNumber(EvalInfoFunc) Then
										EvalInfoFunc = "0"
									End If
								Case "Invalid_string_refer_to_original_code"
									EvalInfoFunc = .NecessarySkill
								Case "ä½¿ç”¨å¯", "Invalid_string_refer_to_original_code"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				End If
			Case "ãƒ©ãƒ³ã‚¯"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Rank)
				End If
			Case "ãƒœã‚¹ãƒ©ãƒ³ã‚¯"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.BossRank)
				End If
			Case "ã‚¨ãƒªã‚¢"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Area
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Mode
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					With u
						max_value = 0
						For i = 1 To .CountWeapon
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							If .WeaponPower(i, "") > max_value Then
								max_value = .WeaponPower(i, "")
							End If
						Next 
					End With
				End If
				'Next
				EvalInfoFunc = VB6.Format(max_value)
				'End With
				'UPGRADE_WARNING: EvalInfoFunc ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				With ud
					max_value = 0
					For i = 1 To .CountWeapon
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .Weapon(i).Power > max_value Then
							max_value = .Weapon(i).Power
						End If
						'End If
					Next 
					EvalInfoFunc = VB6.Format(max_value)
				End With
				'End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					With u
						max_value = 0
						For i = 1 To .CountWeapon
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							If .WeaponMaxRange(i) > max_value Then
								max_value = .WeaponMaxRange(i)
							End If
						Next 
					End With
				End If
				'Next
				EvalInfoFunc = VB6.Format(max_value)
				'End With
				'UPGRADE_WARNING: EvalInfoFunc ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				With ud
					max_value = 0
					For i = 1 To .CountWeapon
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .Weapon(i).MaxRange > max_value Then
							max_value = .Weapon(i).MaxRange
						End If
						'End If
					Next 
					EvalInfoFunc = VB6.Format(max_value)
				End With
				'End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSupportAttack - u.UsedSupportAttack)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSupportGuard - u.UsedSupportGuard)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSyncAttack - u.UsedSyncAttack)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxCounterAttack - u.UsedCounterAttack)
				End If
			Case "æ”¹é€ è²»"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(RankUpCost(u))
				End If
			Case "æœ€å¤§æ”¹é€ æ•°"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(MaxRank(u))
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not it Is Nothing Then
					EvalInfoFunc = it.Class_Renamed()
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Class_Renamed
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not it Is Nothing Then
					EvalInfoFunc = it.Part
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Part
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.HP)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.HP)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.EN)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.EN)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Armor)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Armor)
				End If
			Case "é‹å‹•æ€§ä¿®æ­£å€¤"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Mobility)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Mobility)
				End If
			Case "ç§»å‹•åŠ›ä¿®æ­£å€¤"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Speed)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Speed)
				End If
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not it Is Nothing Then
					EvalInfoFunc = it.Data.Comment
					ReplaceString(EvalInfoFunc, vbCr & vbLf, " ")
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Comment
					ReplaceString(EvalInfoFunc, vbCr & vbLf, " ")
				ElseIf Not spd Is Nothing Then 
					EvalInfoFunc = spd.Comment
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.ShortName
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not spd Is Nothing Then
					EvalInfoFunc = VB6.Format(spd.SPConsumption)
				End If
			Case "å¯¾è±¡"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.TargetType
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.Duration
				End If
			Case "é©ç”¨æ¡ä»¶"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.NecessaryCondition
				End If
			Case "ã‚¢ãƒ‹ãƒ¡"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.Animation
				End If
			Case "åŠ¹æœæ•°"
				If Not spd Is Nothing Then
					EvalInfoFunc = VB6.Format(spd.CountEffect)
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = spd.EffectType(i)
					End If
				End If
			Case "åŠ¹æœãƒ¬ãƒ™ãƒ«"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = VB6.Format(spd.EffectLevel(i))
					End If
				End If
			Case "åŠ¹æœãƒ‡ãƒ¼ã‚¿"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = spd.EffectData(i)
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				idx = idx + 1
				Select Case params(idx)
					Case "Invalid_string_refer_to_original_code"
						EvalInfoFunc = MapFileName
						If Len(EvalInfoFunc) > Len(ScenarioPath) Then
							If Left(EvalInfoFunc, Len(ScenarioPath)) = ScenarioPath Then
								EvalInfoFunc = Mid(EvalInfoFunc, Len(ScenarioPath) + 1)
							End If
						End If
					Case "Invalid_string_refer_to_original_code"
						EvalInfoFunc = VB6.Format(MapWidth)
					Case "æ™‚é–“å¸¯"
						If MapDrawMode <> "" Then
							If MapDrawMode = "ãƒ•ã‚£ãƒ«ã‚¿" Then
								buf = Hex(MapDrawFilterColor)
								For i = 1 To 6 - Len(buf)
									buf = "0" & buf
								Next 
								buf = "#" & Mid(buf, 5, 2) & Mid(buf, 3, 2) & Mid(buf, 1, 2) & " " & CStr(MapDrawFilterTransPercent * 100) & "%"
							Else
								buf = MapDrawMode
							End If
							If MapDrawIsMapOnly Then
								buf = buf & "Invalid_string_refer_to_original_code"
							End If
							EvalInfoFunc = buf
						Else
							EvalInfoFunc = "æ˜¼"
						End If
					Case "é«˜ã•"
						EvalInfoFunc = VB6.Format(MapHeight)
					Case Else
						
						If IsNumber(params(idx)) Then
							mx = CShort(params(idx))
						End If
						idx = idx + 1
						If IsNumber(params(idx)) Then
							my_Renamed = CShort(params(idx))
						End If
						
						If mx < 1 Or MapWidth < mx Or my_Renamed < 1 Or MapHeight < my_Renamed Then
							Exit Function
						End If
						
						idx = idx + 1
						Select Case params(idx)
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = TerrainName(mx, my_Renamed)
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								EvalInfoFunc = TerrainClass(mx, my_Renamed)
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								EvalInfoFunc = VB6.Format(TerrainMoveCost(mx, my_Renamed) / 2)
							Case "å›é¿ä¿®æ­£"
								EvalInfoFunc = VB6.Format(TerrainEffectForHit(mx, my_Renamed))
							Case "ãƒ€ãƒ¡ãƒ¼ã‚¸ä¿®æ­£"
								EvalInfoFunc = VB6.Format(TerrainEffectForDamage(mx, my_Renamed))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(TerrainEffectForHPRecover(mx, my_Renamed))
							Case "Invalid_string_refer_to_original_code"
								EvalInfoFunc = VB6.Format(TerrainEffectForENRecover(mx, my_Renamed))
							Case "Invalid_string_refer_to_original_code"
								'MOD START 240a
								'                            Select Case MapImageFileTypeData(mx, my)
								'                                Case SeparateDirMapImageFileType
								'                                    EvalInfoFunc = _
								''                                        TDList.Bitmap(MapData(mx, my, 0)) & "\" & _
								''                                        TDList.Bitmap(MapData(mx, my, 0)) & _
								''                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
								'                                Case FourFiguresMapImageFileType
								'                                    EvalInfoFunc = _
								''                                        TDList.Bitmap(MapData(mx, my, 0)) & _
								''                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
								'                                Case OldMapImageFileType
								'                                    EvalInfoFunc = _
								''                                        TDList.Bitmap(MapData(mx, my, 0)) & _
								''                                        Format$(MapData(mx, my, 1)) & ".bmp"
								'                            End Select
								Select Case MapImageFileTypeData(mx, my_Renamed)
									Case Map.MapImageFileType.SeparateDirMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.TerrainType)) & "\" & TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.TerrainType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.BitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.FourFiguresMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.TerrainType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.BitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.OldMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.TerrainType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.BitmapNo)) & ".bmp"
								End Select
								'MOD  END  240a
								'ADD START 240a
							Case "Invalid_string_refer_to_original_code"
								Select Case MapImageFileTypeData(mx, my_Renamed)
									Case Map.MapImageFileType.SeparateDirMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & "\" & TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.FourFiguresMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.OldMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo)) & ".bmp"
								End Select
								'ADD  END  240a
							Case "Invalid_string_refer_to_original_code"
								If Not MapDataForUnit(mx, my_Renamed) Is Nothing Then
									EvalInfoFunc = MapDataForUnit(mx, my_Renamed).ID
								End If
						End Select
				End Select
			Case "ã‚ªãƒ—ã‚·ãƒ§ãƒ³"
				idx = idx + 1
				Select Case params(idx)
					Case "MessageWait"
						EvalInfoFunc = VB6.Format(MessageWait)
					Case "BattleAnimation"
						If BattleAnimation Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
						' ADD START MARGE
					Case "ExtendedAnimation"
						If ExtendedAnimation Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
						' ADD END MARGE
					Case "SpecialPowerAnimation"
						If SpecialPowerAnimation Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
					Case "AutoDeffence"
						'UPGRADE_ISSUE: Control mnuMapCommandItem ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
					Case "UseDirectMusic"
						If UseDirectMusic Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
						' MOD START MARGE
						'                Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", _
						''                    "AutoMoveCursor", "DebugMode", "LastFolder", _
						''                    "MIDIPortID", "MP3Volume", _
						''                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
						''                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
						''                    "UseTransparentBlt"
						'Invalid_string_refer_to_original_code
					Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", "AutoMoveCursor", "DebugMode", "LastFolder", "MIDIPortID", "MP3Volume", "BattleAnimation", "WeaponAnimation", "MoveAnimation", "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", "UseTransparentBlt", "NewGUI"
						' MOD END MARGE
						EvalInfoFunc = ReadIni("Option", params(idx))
					Case Else
						'Invalid_string_refer_to_original_code
						If IsOptionDefined(params(idx)) Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
				End Select
		End Select
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'å¤‰æ•°ã®å€¤ã‚’è©•ä¾¡
	Public Function GetVariable(ByRef var_name As String, ByRef etype As ValueType, ByRef str_result As String, ByRef num_result As Double) As ValueType
		Dim vname As String
		Dim i, num As Short
		Dim u As Unit
		Dim ret As Integer
		Dim ipara, idx, buf As String
		Dim start_idx, depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim is_term As Boolean
		
		vname = var_name
		
		'Invalid_string_refer_to_original_code
		str_result = var_name
		
		'Invalid_string_refer_to_original_code
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'Invalid_string_refer_to_original_code
		If InStr(idx, ",") > 0 Then
			start_idx = 1
			depth = 0
			is_term = True
			For i = 1 To Len(idx)
				If in_single_quote Then
					If Asc(Mid(idx, i, 1)) = 96 Then '`
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If Asc(Mid(idx, i, 1)) = 34 Then '"
						in_double_quote = False
					End If
				Else
					Select Case Asc(Mid(idx, i, 1))
						Case 9, 32 'Invalid_string_refer_to_original_code
							If start_idx = i Then
								start_idx = i + 1
							Else
								is_term = False
							End If
						Case 40, 91 '(, [
							depth = depth + 1
						Case 41, 93 '), ]
							depth = depth - 1
						Case 44 ',
							If depth = 0 Then
								If Len(buf) > 0 Then
									buf = buf & ","
								End If
								ipara = Trim(Mid(idx, start_idx, i - start_idx))
								buf = buf & GetValueAsString(ipara, is_term)
								start_idx = i + 1
								is_term = True
							End If
						Case 96 '`
							in_single_quote = True
						Case 34 '"
							in_double_quote = True
					End Select
				End If
			Next 
			ipara = Trim(Mid(idx, start_idx, i - start_idx))
			If Len(buf) > 0 Then
				idx = buf & "," & GetValueAsString(ipara, is_term)
			Else
				idx = GetValueAsString(ipara, is_term)
			End If
		Else
			idx = GetValueAsString(idx)
		End If
		
		'Invalid_string_refer_to_original_code
		vname = Left(vname, ret) & idx & "]"
		
		'Invalid_string_refer_to_original_code
		str_result = ""
		
		'Invalid_string_refer_to_original_code
		
SkipArrayHandling: 
		
		'Invalid_string_refer_to_original_code
		
		'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				With VarStack(i)
					If vname = .Name Then
						Select Case etype
							Case ValueType.NumericType
								If .VariableType = ValueType.NumericType Then
									num_result = .NumericValue
								Else
									num_result = StrToDbl(.StringValue)
								End If
								GetVariable = ValueType.NumericType
							Case ValueType.StringType
								If .VariableType = ValueType.StringType Then
									str_result = .StringValue
								Else
									str_result = FormatNum(.NumericValue)
								End If
								GetVariable = ValueType.StringType
							Case ValueType.UndefinedType
								If .VariableType = ValueType.StringType Then
									str_result = .StringValue
									GetVariable = ValueType.StringType
								Else
									num_result = .NumericValue
									GetVariable = ValueType.NumericType
								End If
						End Select
						Exit Function
					End If
				End With
			Next 
		End If
		
		'ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If IsLocalVariableDefined(vname) Then
			With LocalVariableList.Item(vname)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = StrToDbl(.StringValue)
						End If
						GetVariable = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = FormatNum(.NumericValue)
						End If
						GetVariable = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
							GetVariable = ValueType.StringType
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
							GetVariable = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'ã‚°ãƒ­ãƒ¼ãƒãƒ«å¤‰æ•°
		If IsGlobalVariableDefined(vname) Then
			With GlobalVariableList.Item(vname)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = StrToDbl(.StringValue)
						End If
						GetVariable = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = FormatNum(.NumericValue)
						End If
						GetVariable = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item(vname).VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							str_result = .StringValue
							GetVariable = ValueType.StringType
						Else
							'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							num_result = .NumericValue
							GetVariable = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case vname
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not SelectedUnitForEvent Is Nothing Then
					With SelectedUnitForEvent
						If .CountPilot > 0 Then
							str_result = .MainPilot.ID
						Else
							str_result = ""
						End If
					End With
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not SelectedTargetForEvent Is Nothing Then
					With SelectedTargetForEvent
						If .CountPilot > 0 Then
							str_result = .MainPilot.ID
						Else
							str_result = ""
						End If
					End With
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If Not SelectedUnitForEvent Is Nothing Then
					str_result = SelectedUnitForEvent.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If Not SelectedTargetForEvent Is Nothing Then
					str_result = SelectedTargetForEvent.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					With SelectedUnitForEvent
						If SelectedWeapon > 0 Then
							str_result = SelectedWeaponName
						Else
							str_result = ""
						End If
					End With
				ElseIf SelectedUnitForEvent Is SelectedTarget Then 
					With SelectedUnitForEvent
						If SelectedTWeapon > 0 Then
							str_result = SelectedTWeaponName
						Else
							str_result = SelectedDefenseOption
						End If
					End With
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨"
				str_result = ""
				If SelectedTargetForEvent Is SelectedTarget Then
					With SelectedTargetForEvent
						If SelectedTWeapon > 0 Then
							str_result = SelectedTWeaponName
						Else
							str_result = SelectedDefenseOption
						End If
					End With
				ElseIf SelectedTargetForEvent Is SelectedUnit Then 
					With SelectedTargetForEvent
						If SelectedWeapon > 0 Then
							str_result = SelectedWeaponName
						Else
							str_result = ""
						End If
					End With
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨ç•ªå·"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					With SelectedUnitForEvent
						If etype = ValueType.StringType Then
							str_result = VB6.Format(SelectedWeapon)
							GetVariable = ValueType.StringType
						Else
							num_result = SelectedWeapon
							GetVariable = ValueType.NumericType
						End If
					End With
				ElseIf SelectedUnitForEvent Is SelectedTarget Then 
					With SelectedUnitForEvent
						If etype = ValueType.StringType Then
							str_result = VB6.Format(SelectedTWeapon)
							GetVariable = ValueType.StringType
						Else
							num_result = SelectedTWeapon
							GetVariable = ValueType.NumericType
						End If
					End With
				End If
				Exit Function
				
			Case "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨ç•ªå·"
				str_result = ""
				If SelectedTargetForEvent Is SelectedTarget Then
					With SelectedTargetForEvent
						If etype = ValueType.StringType Then
							str_result = VB6.Format(SelectedTWeapon)
							GetVariable = ValueType.StringType
						Else
							num_result = SelectedTWeapon
							GetVariable = ValueType.NumericType
						End If
					End With
				ElseIf SelectedTargetForEvent Is SelectedUnit Then 
					With SelectedTargetForEvent
						If etype = ValueType.StringType Then
							str_result = VB6.Format(SelectedWeapon)
							GetVariable = ValueType.StringType
						Else
							num_result = SelectedWeapon
							GetVariable = ValueType.NumericType
						End If
					End With
				End If
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					With SelectedUnitForEvent
						If SelectedAbility > 0 Then
							str_result = SelectedAbilityName
						Else
							str_result = ""
						End If
					End With
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					With SelectedUnitForEvent
						If etype = ValueType.StringType Then
							str_result = VB6.Format(SelectedAbility)
							GetVariable = ValueType.StringType
						Else
							num_result = SelectedAbility
							GetVariable = ValueType.NumericType
						End If
					End With
				End If
				Exit Function
				
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					str_result = SelectedSpecialPower
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If Not SupportAttackUnit Is Nothing Then
					str_result = SupportAttackUnit.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If Not SupportGuardUnit Is Nothing Then
					str_result = SupportGuardUnit.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(SelectedAlternative)
					GetVariable = ValueType.NumericType
				Else
					str_result = SelectedAlternative
					GetVariable = ValueType.StringType
				End If
				Exit Function
				
			Case "ã‚¿ãƒ¼ãƒ³æ•°"
				If etype = ValueType.StringType Then
					str_result = VB6.Format(Turn)
					GetVariable = ValueType.StringType
				Else
					num_result = Turn
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "ç·ã‚¿ãƒ¼ãƒ³æ•°"
				If etype = ValueType.StringType Then
					str_result = VB6.Format(TotalTurn)
					GetVariable = ValueType.StringType
				Else
					num_result = TotalTurn
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "ãƒ•ã‚§ã‚¤ã‚º"
				str_result = Stage
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "å‘³æ–¹æ•°"
				num = 0
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = num + 1
						'End If
					End With
				Next u
				If etype = ValueType.StringType Then
					str_result = VB6.Format(num)
					GetVariable = ValueType.StringType
				Else
					num_result = num
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				num = 0
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = num + 1
						'End If
					End With
				Next u
				If etype = ValueType.StringType Then
					str_result = VB6.Format(num)
					GetVariable = ValueType.StringType
				Else
					num_result = num
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "æ•µæ•°"
				num = 0
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = num + 1
						'End If
					End With
				Next u
				If etype = ValueType.StringType Then
					str_result = VB6.Format(num)
					GetVariable = ValueType.StringType
				Else
					num_result = num
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "ä¸­ç«‹æ•°"
				num = 0
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = num + 1
						'End If
					End With
				Next u
				If etype = ValueType.StringType Then
					str_result = VB6.Format(num)
					GetVariable = ValueType.StringType
				Else
					num_result = num
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "Invalid_string_refer_to_original_code"
				If etype = ValueType.StringType Then
					str_result = FormatNum(Money)
					GetVariable = ValueType.StringType
				Else
					num_result = Money
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case Else
				'Invalid_string_refer_to_original_code
				Select Case LCase(vname)
					Case "apppath"
						str_result = AppPath
						GetVariable = ValueType.StringType
						Exit Function
						
					Case "appversion"
						'UPGRADE_ISSUE: App ƒIƒuƒWƒFƒNƒg ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						With App
							num = 10000 * My.Application.Info.Version.Major + 100 * My.Application.Info.Version.Minor + My.Application.Info.Version.Revision
						End With
						If etype = ValueType.StringType Then
							str_result = VB6.Format(num)
							GetVariable = ValueType.StringType
						Else
							num_result = num
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "argnum"
						'Invalid_string_refer_to_original_code
						num = UpVarLevel
						i = CallDepth
						Do While num > 0
							i = i - num
							If i < 1 Then
								i = 1
								Exit Do
							End If
							num = UpVarLevelStack(i)
						Loop 
						
						num = ArgIndex - ArgIndexStack(i - 1)
						If etype = ValueType.StringType Then
							str_result = VB6.Format(num)
							GetVariable = ValueType.StringType
						Else
							num_result = num
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "basex"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(BaseX)
							GetVariable = ValueType.StringType
						Else
							num_result = BaseX
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "basey"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(BaseY)
							GetVariable = ValueType.StringType
						Else
							num_result = BaseY
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "extdatapath"
						str_result = ExtDataPath
						GetVariable = ValueType.StringType
						Exit Function
						
					Case "extdatapath2"
						str_result = ExtDataPath2
						GetVariable = ValueType.StringType
						Exit Function
						
					Case "mousex"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(MouseX)
							GetVariable = ValueType.StringType
						Else
							num_result = MouseX
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "mousey"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(MouseY)
							GetVariable = ValueType.StringType
						Else
							num_result = MouseY
							GetVariable = ValueType.NumericType
						End If
						Exit Function
						
					Case "now"
						str_result = CStr(Now)
						GetVariable = ValueType.StringType
						Exit Function
						
					Case "scenariopath"
						str_result = ScenarioPath
						GetVariable = ValueType.StringType
						Exit Function
				End Select
		End Select
		
		'Invalid_string_refer_to_original_code
		If BCVariable.IsConfig Then
			Select Case vname
				Case "Invalid_string_refer_to_original_code"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.AttackExp)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.AttackExp
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					str_result = BCVariable.AtkUnit.ID
					GetVariable = ValueType.StringType
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					If Not BCVariable.DefUnit Is Nothing Then
						str_result = BCVariable.DefUnit.ID
						GetVariable = ValueType.StringType
						Exit Function
					End If
				Case "æ­¦å™¨ç•ªå·"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.WeaponNumber)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.WeaponNumber
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.TerrainAdaption)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.TerrainAdaption
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "æ­¦å™¨å¨åŠ›"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.WeaponPower)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.WeaponPower
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "ã‚µã‚¤ã‚ºè£œæ­£"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.SizeMod)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.SizeMod
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.Armor)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.Armor
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "æœ€çµ‚å€¤"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.LastVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.LastVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.AttackVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.AttackVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "é˜²å¾¡å´è£œæ­£"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.DffenceVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.DffenceVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "ã‚¶ã‚³è£œæ­£"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.CommonEnemy)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.CommonEnemy
						GetVariable = ValueType.NumericType
					End If
					Exit Function
			End Select
			
			'Invalid_string_refer_to_original_code
			With BCVariable.MeUnit.MainPilot
				Select Case vname
					Case "æ°—åŠ›"
						num = 0
						
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = 50 + (.Morale + .MoraleMod) \ 2 ' æ°—åŠ›ã®è£œæ­£è¾¼ã¿å€¤ã‚’ä»£å…¥
				End Select
			End With
		Else
			'UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B ' æ°—åŠ›ã®è£œæ­£è¾¼ã¿å€¤ã‚’ä»£å…¥
		End If
		
		If etype = ValueType.StringType Then
			str_result = VB6.Format(num)
			GetVariable = ValueType.StringType
		Else
			num_result = num
			GetVariable = ValueType.NumericType
		End If
		Exit Function
		'UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Case "Invalid_string_refer_to_original_code"
			'If etype = ValueType.StringType Then
				''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'GetVariable = ValueType.StringType
			'Else
				''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'GetVariable = ValueType.NumericType
			'End If
			'Exit Function
			''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Case "Invalid_string_refer_to_original_code"
				'If etype = ValueType.StringType Then
					''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
					'GetVariable = ValueType.StringType
				'Else
					''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
					'GetVariable = ValueType.NumericType
				'End If
				'Exit Function
				''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'Case "Invalid_string_refer_to_original_code"
					'If etype = ValueType.StringType Then
						''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
						'GetVariable = ValueType.StringType
					'Else
						''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
						'GetVariable = ValueType.NumericType
					'End If
					'Exit Function
					''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
					'Case "Invalid_string_refer_to_original_code"
						'If etype = ValueType.StringType Then
							''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
							'GetVariable = ValueType.StringType
						'Else
							''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
							'GetVariable = ValueType.NumericType
						'End If
						'Exit Function
						''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
						'Case "éœŠåŠ›"
							'If etype = ValueType.StringType Then
								''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
								'GetVariable = ValueType.StringType
							'Else
								''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
								'GetVariable = ValueType.NumericType
							'End If
							'Exit Function
							''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
							'Case "Invalid_string_refer_to_original_code"
								'If etype = ValueType.StringType Then
									''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
									'GetVariable = ValueType.StringType
								'Else
									''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
									'GetVariable = ValueType.NumericType
								'End If
								'Exit Function
								''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
								'Case "Invalid_string_refer_to_original_code"
									'If etype = ValueType.StringType Then
										''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
										'GetVariable = ValueType.StringType
									'Else
										''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
										'GetVariable = ValueType.NumericType
									'End If
									'Exit Function
									''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
									'Case "å‘½ä¸­"
										'If etype = ValueType.StringType Then
											''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
											'GetVariable = ValueType.StringType
										'Else
											''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
											'GetVariable = ValueType.NumericType
										'End If
										'Exit Function
										''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
										'Case "å›é¿"
											'If etype = ValueType.StringType Then
												''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
												'GetVariable = ValueType.StringType
											'Else
												''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
												'GetVariable = ValueType.NumericType
											'End If
											'Exit Function
											''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
											'Case "Invalid_string_refer_to_original_code"
												'If etype = ValueType.StringType Then
													''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
													'GetVariable = ValueType.StringType
												'Else
													''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
													'GetVariable = ValueType.NumericType
												'End If
												'Exit Function
												''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
												'Case "Invalid_string_refer_to_original_code"
													'If etype = ValueType.StringType Then
														''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
														'GetVariable = ValueType.StringType
													'Else
														''UPGRADE_WARNING: GetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
														'GetVariable = ValueType.NumericType
													'End If
													'Exit Function
													'End Select
													'End With
													'
													'ãƒ¦ãƒ‹ãƒƒãƒˆã«é–¢ã™ã‚‹å¤‰æ•°
													'With BCVariable.MeUnit
														'Select Case vname
															'Case "Invalid_string_refer_to_original_code"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.MaxHP())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .MaxHP()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "Invalid_string_refer_to_original_code"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.HP())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .HP()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "Invalid_string_refer_to_original_code"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.MaxEN())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .MaxEN()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "Invalid_string_refer_to_original_code"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.EN())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .EN()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "ç§»å‹•åŠ›"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.Speed())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .Speed()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "Invalid_string_refer_to_original_code"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.Armor())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .Armor()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
															'Case "é‹å‹•æ€§"
																'If etype = ValueType.StringType Then
																	'str_result = VB6.Format(.Mobility())
																	'GetVariable = ValueType.StringType
																'Else
																	'num_result = .Mobility()
																	'GetVariable = ValueType.NumericType
																'End If
																'Exit Function
														'End Select
													'End With
													'End If
													'
													'If etype = ValueType.NumericType Then
														'num_result = 0
														'GetVariable = ValueType.NumericType
													'Else
														'GetVariable = ValueType.StringType
													'End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsVariableDefined(ByRef var_name As String) As Boolean
		Dim vname As String
		Dim i, ret As Short
		Dim ipara, idx, buf As String
		Dim start_idx, depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim is_term As Boolean
		
		Select Case Asc(var_name)
			Case 36 '$
				vname = Mid(var_name, 2)
			Case Else
				vname = var_name
		End Select
		
		'Invalid_string_refer_to_original_code
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'Invalid_string_refer_to_original_code
		If InStr(idx, ",") > 0 Then
			start_idx = 1
			depth = 0
			is_term = True
			For i = 1 To Len(idx)
				If in_single_quote Then
					If Asc(Mid(idx, i, 1)) = 96 Then '`
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If Asc(Mid(idx, i, 1)) = 34 Then '"
						in_double_quote = False
					End If
				Else
					Select Case Asc(Mid(idx, i, 1))
						Case 9, 32 'Invalid_string_refer_to_original_code
							If start_idx = i Then
								start_idx = i + 1
							Else
								is_term = False
							End If
						Case 40, 91 '(, 
							depth = depth + 1
						Case 41, 93 '), 
							depth = depth - 1
						Case 44 ',
							If depth = 0 Then
								If Len(buf) > 0 Then
									buf = buf & ","
								End If
								ipara = Trim(Mid(idx, start_idx, i - start_idx))
								buf = buf & GetValueAsString(ipara, is_term)
								start_idx = i + 1
								is_term = True
							End If
						Case 96 '`
							in_single_quote = True
						Case 34 '"
							in_double_quote = True
					End Select
				End If
			Next 
			ipara = Trim(Mid(idx, start_idx, i - start_idx))
			If Len(buf) > 0 Then
				idx = buf & "," & GetValueAsString(ipara, is_term)
			Else
				idx = GetValueAsString(ipara, is_term)
			End If
		Else
			idx = GetValueAsString(Trim(idx))
		End If
		
		'Invalid_string_refer_to_original_code
		vname = Left(vname, ret) & idx & "]"
		
		'Invalid_string_refer_to_original_code
		
SkipArrayHandling: 
		
		'Invalid_string_refer_to_original_code
		
		'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				If vname = VarStack(i).Name Then
					IsVariableDefined = True
					Exit Function
				End If
			Next 
		End If
		
		'ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If IsLocalVariableDefined(vname) Then
			IsVariableDefined = True
			Exit Function
		End If
		
		'ã‚°ãƒ­ãƒ¼ãƒãƒ«å¤‰æ•°
		If IsGlobalVariableDefined(vname) Then
			IsVariableDefined = True
			Exit Function
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsSubLocalVariableDefined(ByRef vname As String) As Boolean
		Dim i As Short
		
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				If vname = VarStack(i).Name Then
					IsSubLocalVariableDefined = True
					Exit Function
				End If
			Next 
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsLocalVariableDefined(ByRef vname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = LocalVariableList.Item(vname)
		IsLocalVariableDefined = True
		Exit Function
		
ErrorHandler: 
		IsLocalVariableDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsGlobalVariableDefined(ByRef vname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = GlobalVariableList.Item(vname)
		IsGlobalVariableDefined = True
		Exit Function
		
ErrorHandler: 
		IsGlobalVariableDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub SetVariable(ByRef var_name As String, ByRef etype As ValueType, ByRef str_value As String, ByRef num_value As Double)
		Dim new_var As VarData
		Dim vname As String
		Dim i, ret As Short
		Dim ipara, idx, buf As String
		Dim vname0 As String
		Dim p As Pilot
		Dim u As Unit
		Dim start_idx, depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim is_term As Boolean
		Dim is_subroutine_local_array As Boolean
		
		'Debug.Print "Set " & vname & " " & new_value
		
		vname = var_name
		
		'Invalid_string_refer_to_original_code
		ret = InStr(vname, "(")
		If ret > 1 And Right(vname, 1) = ")" Then
			Select Case LCase(Left(vname, ret - 1))
				Case "hp"
					idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
					idx = GetValueAsString(idx)
					
					If UList.IsDefined2(idx) Then
						u = UList.Item2(idx)
					ElseIf PList.IsDefined(idx) Then 
						u = PList.Item(idx).Unit_Renamed
					Else
						u = SelectedUnitForEvent
					End If
					
					If Not u Is Nothing Then
						If etype = ValueType.NumericType Then
							u.HP = num_value
						Else
							u.HP = StrToLng(str_value)
						End If
						If u.HP <= 0 Then
							u.HP = 1
						End If
					End If
					Exit Sub
					
				Case "en"
					idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
					idx = GetValueAsString(idx)
					
					If UList.IsDefined2(idx) Then
						u = UList.Item2(idx)
					ElseIf PList.IsDefined(idx) Then 
						u = PList.Item(idx).Unit_Renamed
					Else
						u = SelectedUnitForEvent
					End If
					
					If Not u Is Nothing Then
						If etype = ValueType.NumericType Then
							u.EN = num_value
						Else
							u.EN = StrToLng(str_value)
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						PaintUnitBitmap(u)
					End If
			End Select
		End If
		Exit Sub
		
		Dim new_var2 As VarData
		'UPGRADE_WARNING: SetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'Case "sp"
			'idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
			'idx = GetValueAsString(idx)
			'
			'If UList.IsDefined2(idx) Then
				'p = UList.Item2(idx).MainPilot
			'ElseIf PList.IsDefined(idx) Then 
				'p = PList.Item(idx)
			'Else
				'p = SelectedUnitForEvent.MainPilot
			'End If
			'
			'If Not p Is Nothing Then
				'With p
					'If .MaxSP > 0 Then
						'If etype = ValueType.NumericType Then
							'.SP = num_value
						'Else
							'.SP = StrToLng(str_value)
						'End If
					'End If
				'End With
			'End If
			'Exit Sub
			'
			''UPGRADE_WARNING: SetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'Case "plana"
				'idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
				'idx = GetValueAsString(idx)
				'
				'If UList.IsDefined2(idx) Then
					'p = UList.Item2(idx).MainPilot
				'ElseIf PList.IsDefined(idx) Then 
					'p = PList.Item(idx)
				'Else
					'p = SelectedUnitForEvent.MainPilot
				'End If
				'
				'If Not p Is Nothing Then
					'With p
						'If .MaxPlana > 0 Then
							'If etype = ValueType.NumericType Then
								'.Plana = num_value
							'Else
								'.Plana = StrToLng(str_value)
							'End If
						'End If
					'End With
				'End If
				'Exit Sub
				'
				''UPGRADE_WARNING: SetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'Case "action"
					'idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
					'idx = GetValueAsString(idx)
					'
					'If UList.IsDefined2(idx) Then
						'u = UList.Item2(idx)
					'ElseIf PList.IsDefined(idx) Then 
						'u = PList.Item(idx).Unit_Renamed
					'Else
						'u = SelectedUnitForEvent
					'End If
					'
					'If Not u Is Nothing Then
						'If etype = ValueType.NumericType Then
							'u.UsedAction = u.MaxAction - num_value
						'Else
							'u.UsedAction = u.MaxAction - StrToLng(str_value)
						'End If
					'End If
					'Exit Sub
					'
					''UPGRADE_WARNING: SetVariable ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
					'Case "eval"
						'vname = Trim(Mid(vname, ret + 1, Len(vname) - ret - 1))
						'vname = GetValueAsString(vname)
						'
						'End Select
						'End If
						'
						'Invalid_string_refer_to_original_code
						'ret = InStr(vname, "[")
						'If ret = 0 Then
							'GoTo SkipArrayHandling
						'End If
						'If Right(vname, 1) <> "]" Then
							'GoTo SkipArrayHandling
						'End If
						'
						'Invalid_string_refer_to_original_code
						'
						'Invalid_string_refer_to_original_code
						'idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
						'
						'Invalid_string_refer_to_original_code
						'If InStr(idx, ",") > 0 Then
							'start_idx = 1
							'depth = 0
							'is_term = True
							'For 'i = 1 To Len(idx)
								'If in_single_quote Then
									'If Asc(Mid(idx, i, 1)) = 96 Then '`
										'in_single_quote = False
									'End If
								'ElseIf in_double_quote Then 
									'If Asc(Mid(idx, i, 1)) = 34 Then '"
										'in_double_quote = False
									'End If
								'Else
									'Select Case Asc(Mid(idx, i, 1))
										'Case 9, 32 'Invalid_string_refer_to_original_code
											'If start_idx = i Then
												'start_idx = i + 1
											'Else
												'is_term = False
											'End If
										'Case 40, 91 '(, [
											'depth = depth + 1
										'Case 41, 93 '), ]
											'depth = depth - 1
										'Case 44 ',
											'If depth = 0 Then
												'If Len(buf) > 0 Then
													'buf = buf & ","
												'End If
												'ipara = Trim(Mid(idx, start_idx, i - start_idx))
												'buf = buf & GetValueAsString(ipara, is_term)
												'start_idx = i + 1
												'is_term = True
											'End If
										'Case 96 '`
											'in_single_quote = True
										'Case 34 '"
											'in_double_quote = True
									'End Select
								'End If
							'Next 
							'ipara = Trim(Mid(idx, start_idx, i - start_idx))
							'If Len(buf) > 0 Then
								'idx = buf & "," & GetValueAsString(ipara, is_term)
							'Else
								'idx = GetValueAsString(ipara, is_term)
							'End If
						'Else
							'idx = GetValueAsString(Trim(idx))
						'End If
						'
						'Invalid_string_refer_to_original_code
						'vname = Left(vname, ret) & idx & "]"
						'
						'Invalid_string_refer_to_original_code
						'vname0 = Left(vname, ret - 1)
						'
						'Invalid_string_refer_to_original_code
						'If IsSubLocalVariableDefined(vname0) Then
							'is_subroutine_local_array = True
						'End If
						'
						'Invalid_string_refer_to_original_code
						'
'SkipArrayHandling: '
						'
						'Invalid_string_refer_to_original_code
						'
						'Invalid_string_refer_to_original_code
						'If CallDepth > 0 Then
							'For 'i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
								'With VarStack(i)
									'If vname = .Name Then
										'.VariableType = etype
										'.StringValue = str_value
										'.NumericValue = num_value
										'Exit Sub
									'End If
								'End With
							'Next 
						'End If
						'
						'If is_subroutine_local_array Then
							'Invalid_string_refer_to_original_code
							'VarIndex = VarIndex + 1
							'If VarIndex > MaxVarIndex Then
								'VarIndex = MaxVarIndex
								'DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								'VB6.Format((MaxVarIndex) & "Invalid_string_refer_to_original_code")
								'Exit Sub
							'End If
							'With VarStack(VarIndex)
								'.Name = vname
								'.VariableType = etype
								'.StringValue = str_value
								'.NumericValue = num_value
							'End With
							'Exit Sub
						'End If
						'
						'Invalid_string_refer_to_original_code
						'If IsLocalVariableDefined(vname) Then
							'With LocalVariableList.Item(vname)
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().Name ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.Name = vname
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.VariableType = etype
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.StringValue = str_value
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg LocalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.NumericValue = num_value
							'End With
							'Exit Sub
						'End If
						'
						'Invalid_string_refer_to_original_code
						'If IsGlobalVariableDefined(vname) Then
							'With GlobalVariableList.Item(vname)
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().Name ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.Name = vname
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.VariableType = etype
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.StringValue = str_value
								''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'.NumericValue = num_value
							'End With
							'Exit Sub
						'End If
						'
						'Invalid_string_refer_to_original_code
						'Select Case LCase(vname)
							'Case "basex"
								'If etype = ValueType.NumericType Then
									'BaseX = num_value
								'Else
									'BaseX = StrToLng(str_value)
								'End If
								''UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'MainForm.picMain(0).CurrentX = BaseX
								'Exit Sub
							'Case "basey"
								'If etype = ValueType.NumericType Then
									'BaseY = num_value
								'Else
									'BaseY = StrToLng(str_value)
								'End If
								''UPGRADE_ISSUE: Control picMain ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								'MainForm.picMain(0).CurrentY = BaseY
								'Exit Sub
							'Case "ã‚¿ãƒ¼ãƒ³æ•°"
								'If etype = ValueType.NumericType Then
									'Turn = num_value
								'Else
									'Turn = StrToLng(str_value)
								'End If
								'Exit Sub
							'Case "ç·ã‚¿ãƒ¼ãƒ³æ•°"
								'If etype = ValueType.NumericType Then
									'TotalTurn = num_value
								'Else
									'TotalTurn = StrToLng(str_value)
								'End If
								'Exit Sub
							'Case "Invalid_string_refer_to_original_code"
								'Money = 0
								'If etype = ValueType.NumericType Then
									'IncrMoney(num_value)
								'Else
									'IncrMoney(StrToLng(str_value))
								'End If
								'Exit Sub
						'End Select
						'
						'Invalid_string_refer_to_original_code
						'
						'Invalid_string_refer_to_original_code
						'If Len(vname0) <> 0 Then
							'Invalid_string_refer_to_original_code
							'If IsLocalVariableDefined(vname0) Then
								'Nop
								'Invalid_string_refer_to_original_code
							'ElseIf IsGlobalVariableDefined(vname0) Then 
								'DefineGlobalVariable(vname)
								'With GlobalVariableList.Item(vname)
									''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().Name ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									'.Name = vname
									''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().VariableType ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									'.VariableType = etype
									''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									'.StringValue = str_value
									''UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().NumericValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									'.NumericValue = num_value
								'End With
								'Exit Sub
								'Invalid_string_refer_to_original_code
							'Else
								'Invalid_string_refer_to_original_code
								'new_var2 = New VarData
								'With new_var2
									'.Name = vname0
									'.VariableType = ValueType.StringType
									'If InStr(.Name, """") > 0 Then
										'DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
										'Invalid_string_refer_to_original_code
										'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
									'End If
								'End With
								'LocalVariableList.Add(new_var2, vname0)
							'End If
						'End If
						'
						'Invalid_string_refer_to_original_code
						'new_var = New VarData
						'With new_var
							'.Name = vname
							'.VariableType = etype
							'.StringValue = str_value
							'.NumericValue = num_value
							'If InStr(.Name, """") > 0 Then
								'DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							'End If
						'End With
						'LocalVariableList.Add(new_var, vname)
	End Sub
	
	Public Sub SetVariableAsString(ByRef vname As String, ByRef new_value As String)
		SetVariable(vname, ValueType.StringType, new_value, 0)
	End Sub
	
	Public Sub SetVariableAsDouble(ByRef vname As String, ByVal new_value As Double)
		SetVariable(vname, ValueType.NumericType, "", new_value)
	End Sub
	
	Public Sub SetVariableAsLong(ByRef vname As String, ByVal new_value As Integer)
		SetVariable(vname, ValueType.NumericType, "", CDbl(new_value))
	End Sub
	
	'ã‚°ãƒ­ãƒ¼ãƒãƒ«å¤‰æ•°ã‚’å®šç¾©
	Public Sub DefineGlobalVariable(ByRef vname As String)
		Dim new_var As New VarData
		
		With new_var
			.Name = vname
			.VariableType = ValueType.StringType
			.StringValue = ""
		End With
		GlobalVariableList.Add(new_var, vname)
	End Sub
	
	'ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°ã‚’å®šç¾©
	Public Sub DefineLocalVariable(ByRef vname As String)
		Dim new_var As New VarData
		
		With new_var
			.Name = vname
			.VariableType = ValueType.StringType
			.StringValue = ""
		End With
		LocalVariableList.Add(new_var, vname)
	End Sub
	
	'å¤‰æ•°ã‚’æ¶ˆå»
	Public Sub UndefineVariable(ByRef var_name As String)
		Dim var As VarData
		Dim vname, vname2 As String
		Dim i, ret As Short
		Dim idx, buf As String
		Dim start_idx, depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim is_term As Boolean
		
		If Asc(var_name) = 36 Then '$
			vname = Mid(var_name, 2)
		Else
			vname = var_name
		End If
		
		'Evalé–¢æ•°
		If LCase(Left(vname, 5)) = "eval(" Then
			If Right(vname, 1) = ")" Then
				vname = Mid(vname, 6, Len(vname) - 6)
				vname = GetValueAsString(vname)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'Invalid_string_refer_to_original_code
		If InStr(idx, ",") > 0 Then
			start_idx = 1
			depth = 0
			is_term = True
			For i = 1 To Len(idx)
				If in_single_quote Then
					If Asc(Mid(idx, i, 1)) = 96 Then '`
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If Asc(Mid(idx, i, 1)) = 34 Then '"
						in_double_quote = False
					End If
				Else
					Select Case Asc(Mid(idx, i, 1))
						Case 9, 32 'Invalid_string_refer_to_original_code
							If start_idx = i Then
								start_idx = i + 1
							Else
								is_term = False
							End If
						Case 40, 91 '(, [
							depth = depth + 1
						Case 41, 93 '), ]
							depth = depth - 1
						Case 44 ',
							If depth = 0 Then
								If Len(buf) > 0 Then
									buf = buf & ","
								End If
								buf = buf & GetValueAsString(Mid(idx, start_idx, i - start_idx), is_term)
								start_idx = i + 1
								is_term = True
							End If
						Case 96 '`
							in_single_quote = True
						Case 34 '"
							in_double_quote = True
					End Select
				End If
			Next 
			If Len(buf) > 0 Then
				idx = buf & "," & GetValueAsString(Mid(idx, start_idx, i - start_idx), is_term)
			Else
				idx = GetValueAsString(Mid(idx, start_idx, i - start_idx), is_term)
			End If
		Else
			idx = GetValueAsString(idx)
		End If
		
		'Invalid_string_refer_to_original_code
		vname = Left(vname, ret) & idx & "]"
		
		'Invalid_string_refer_to_original_code
		If IsSubLocalVariableDefined(vname) Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				With VarStack(i)
					If vname = .Name Then
						.Name = ""
						Exit Sub
					End If
				End With
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		If IsLocalVariableDefined(vname) Then
			LocalVariableList.Remove(vname)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined(vname) Then
			GlobalVariableList.Remove(vname)
		End If
		
		'Invalid_string_refer_to_original_code
		Exit Sub
		
SkipArrayHandling: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		vname2 = vname & "["
		
		'Invalid_string_refer_to_original_code
		If IsSubLocalVariableDefined(vname) Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				With VarStack(i)
					If vname = .Name Or InStr(.Name, vname2) = 1 Then
						.Name = ""
					End If
				End With
			Next 
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsLocalVariableDefined(vname) Then
			LocalVariableList.Remove(vname)
			For	Each var In LocalVariableList
				With var
					If InStr(.Name, vname2) = 1 Then
						LocalVariableList.Remove(.Name)
					End If
				End With
			Next var
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined(vname) Then
			GlobalVariableList.Remove(vname)
			For	Each var In GlobalVariableList
				With var
					If InStr(.Name, vname2) = 1 Then
						GlobalVariableList.Remove(.Name)
					End If
				End With
			Next var
			Exit Sub
		End If
	End Sub
	
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Function GetValueAsString(ByRef expr As String, Optional ByVal is_term As Boolean = False) As String
		Dim num As Double
		
		If is_term Then
			EvalTerm(expr, ValueType.StringType, GetValueAsString, num)
		Else
			EvalExpr(expr, ValueType.StringType, GetValueAsString, num)
		End If
	End Function
	
	'å¼ã‚’æµ®å‹•å°æ•°ç‚¹æ•°ã¨ã—ã¦è©•ä¾¡
	Public Function GetValueAsDouble(ByRef expr As String, Optional ByVal is_term As Boolean = False) As Double
		Dim buf As String
		
		If is_term Then
			EvalTerm(expr, ValueType.NumericType, buf, GetValueAsDouble)
		Else
			EvalExpr(expr, ValueType.NumericType, buf, GetValueAsDouble)
		End If
	End Function
	
	'å¼ã‚’æ•´æ•°ã¨ã—ã¦è©•ä¾¡
	Public Function GetValueAsLong(ByRef expr As String, Optional ByVal is_term As Boolean = False) As Integer
		Dim buf As String
		Dim num As Double
		
		If is_term Then
			EvalTerm(expr, ValueType.NumericType, buf, num)
		Else
			EvalExpr(expr, ValueType.NumericType, buf, num)
		End If
		GetValueAsLong = num
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Function IsExpr(ByRef str_Renamed As String) As Boolean
		Select Case Asc(str_Renamed)
			Case 36 '$
				IsExpr = True
			Case 40 '(
				IsExpr = True
		End Select
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function IsOptionDefined(ByRef oname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = GlobalVariableList.Item("Option(" & oname & ")")
		IsOptionDefined = True
		Exit Function
		
ErrorHandler: 
		IsOptionDefined = False
	End Function
	
	
	'str ã«å¯¾ã—ã¦å¼ç½®æ›ã‚’è¡Œã†
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Sub ReplaceSubExpression(ByRef str_Renamed As String)
		Dim start_idx, end_idx As Short
		Dim str_len As Short
		Dim i, n As Short
		
		Do While True
			'Invalid_string_refer_to_original_code
			start_idx = InStr(str_Renamed, "$(")
			If start_idx = 0 Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			str_len = Len(str_Renamed)
			n = 1
			For i = start_idx + 2 To str_len
				Select Case Mid(str_Renamed, i, 1)
					Case ")"
						n = n - 1
						If n = 0 Then
							end_idx = i
							Exit For
						End If
					Case "("
						n = n + 1
				End Select
			Next 
			If i > str_len Then
				Exit Sub
			End If
			
			'å¼ç½®æ›ã‚’å®Ÿæ–½
			str_Renamed = Left(str_Renamed, start_idx - 1) & GetValueAsString(Mid(str_Renamed, start_idx + 2, end_idx - start_idx - 2)) & Right(str_Renamed, str_len - end_idx)
		Loop 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub FormatMessage(ByRef msg As String)
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReplaceString(msg, "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		ReplaceString(msg, "ãƒ¼ãƒ¼", "â”€â”€")
		ReplaceString(msg, "â”€ãƒ¼", "â”€â”€")
		'End If
		
		'Invalid_string_refer_to_original_code
		ReplaceSubExpression(msg)
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function Term(ByRef tname As String, Optional ByRef u As Unit = Nothing, Optional ByVal tlen As Short = 0) As String
		Dim vname As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If Not u Is Nothing Then
			With u
				If .IsFeatureAvailable("ç”¨èªå") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "ç”¨èªå" Then
							If LIndex(.FeatureData(i), 1) = tname Then
								Term = LIndex(.FeatureData(i), 2)
								Exit For
							End If
						End If
					Next 
				End If
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(Term) = 0 Then
			Select Case tname
				Case "HP", "EN", "SP", "CT"
					vname = "ShortTerm(" & tname & ")"
				Case Else
					vname = "Term(" & tname & ")"
			End Select
			If IsGlobalVariableDefined(vname) Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg GlobalVariableList.Item().StringValue ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Term = GlobalVariableList.Item(vname).StringValue
			Else
				Term = tname
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If tlen > 0 Then
			'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If LenB(StrConv(Term, vbFromUnicode)) < tlen Then
				Term = RightPaddedString(Term, tlen)
			End If
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function GetVariableObject(ByRef var_name As String) As VarData
		Dim vname As String
		Dim i, num As Short
		Dim u As Unit
		Dim ret As Integer
		Dim ipara, idx, buf As String
		Dim start_idx, depth As Short
		Dim in_single_quote, in_double_quote As Boolean
		Dim is_term As Boolean
		
		Dim etype As ValueType
		Dim str_result As String
		Dim num_result As Double
		
		vname = var_name
		
		'Invalid_string_refer_to_original_code
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'Invalid_string_refer_to_original_code
		If InStr(idx, ",") > 0 Then
			start_idx = 1
			depth = 0
			is_term = True
			For i = 1 To Len(idx)
				If in_single_quote Then
					If Asc(Mid(idx, i, 1)) = 96 Then '`
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If Asc(Mid(idx, i, 1)) = 34 Then '"
						in_double_quote = False
					End If
				Else
					Select Case Asc(Mid(idx, i, 1))
						Case 9, 32 'Invalid_string_refer_to_original_code
							If start_idx = i Then
								start_idx = i + 1
							Else
								is_term = False
							End If
						Case 40, 91 '(, [
							depth = depth + 1
						Case 41, 93 '), ]
							depth = depth - 1
						Case 44 ',
							If depth = 0 Then
								If Len(buf) > 0 Then
									buf = buf & ","
								End If
								ipara = Trim(Mid(idx, start_idx, i - start_idx))
								buf = buf & GetValueAsString(ipara, is_term)
								start_idx = i + 1
								is_term = True
							End If
						Case 96 '`
							in_single_quote = True
						Case 34 '"
							in_double_quote = True
					End Select
				End If
			Next 
			ipara = Trim(Mid(idx, start_idx, i - start_idx))
			If Len(buf) > 0 Then
				idx = buf & "," & GetValueAsString(ipara, is_term)
			Else
				idx = GetValueAsString(ipara, is_term)
			End If
		Else
			idx = GetValueAsString(idx)
		End If
		
		'Invalid_string_refer_to_original_code
		vname = Left(vname, ret) & idx & "]"
		
		'Invalid_string_refer_to_original_code
		
SkipArrayHandling: 
		
		'Invalid_string_refer_to_original_code
		
		'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				If vname = VarStack(i).Name Then
					GetVariableObject = VarStack(i)
					Exit Function
				End If
			Next 
		End If
		
		'ãƒ­ãƒ¼ã‚«ãƒ«å¤‰æ•°
		If IsLocalVariableDefined(vname) Then
			GetVariableObject = LocalVariableList.Item(vname)
			Exit Function
		End If
		
		'ã‚°ãƒ­ãƒ¼ãƒãƒ«å¤‰æ•°
		If IsGlobalVariableDefined(vname) Then
			GetVariableObject = GlobalVariableList.Item(vname)
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		etype = ValueType.UndefinedType
		str_result = ""
		num_result = 0
		Select Case vname
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not SelectedUnitForEvent Is Nothing Then
					With SelectedUnitForEvent
						If .CountPilot > 0 Then
							str_result = .MainPilot.ID
						Else
							str_result = ""
						End If
					End With
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not SelectedTargetForEvent Is Nothing Then
					With SelectedTargetForEvent
						If .CountPilot > 0 Then
							str_result = .MainPilot.ID
						Else
							str_result = ""
						End If
					End With
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "Invalid_string_refer_to_original_code"
				If Not SelectedUnitForEvent Is Nothing Then
					str_result = SelectedUnitForEvent.ID
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "Invalid_string_refer_to_original_code"
				If Not SelectedTargetForEvent Is Nothing Then
					str_result = SelectedTargetForEvent.ID
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨"
				If SelectedUnitForEvent Is SelectedUnit Then
					If SelectedWeapon > 0 Then
						str_result = SelectedWeaponName
					Else
						str_result = ""
					End If
				ElseIf SelectedUnitForEvent Is SelectedTarget Then 
					If SelectedTWeapon > 0 Then
						str_result = SelectedTWeaponName
					Else
						str_result = SelectedDefenseOption
					End If
				End If
				etype = ValueType.StringType
			Case "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨"
				If SelectedTargetForEvent Is SelectedTarget Then
					If SelectedTWeapon > 0 Then
						str_result = SelectedTWeaponName
					Else
						str_result = SelectedDefenseOption
					End If
				ElseIf SelectedTargetForEvent Is SelectedUnit Then 
					If SelectedWeapon > 0 Then
						str_result = SelectedWeaponName
					Else
						str_result = ""
					End If
				End If
				etype = ValueType.StringType
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨ç•ªå·"
				If SelectedUnitForEvent Is SelectedUnit Then
					num_result = SelectedWeapon
				ElseIf SelectedUnitForEvent Is SelectedTarget Then 
					num_result = SelectedTWeapon
				End If
				etype = ValueType.NumericType
			Case "ç›¸æ‰‹ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨æ­¦å™¨ç•ªå·"
				If SelectedTargetForEvent Is SelectedTarget Then
					num_result = SelectedTWeapon
				ElseIf SelectedTargetForEvent Is SelectedUnit Then 
					num_result = SelectedWeapon
				End If
				etype = ValueType.NumericType
			Case "Invalid_string_refer_to_original_code"
				If SelectedUnitForEvent Is SelectedUnit Then
					If SelectedAbility > 0 Then
						str_result = SelectedAbilityName
					Else
						str_result = ""
					End If
				End If
				etype = ValueType.StringType
			Case "Invalid_string_refer_to_original_code"
				If SelectedUnitForEvent Is SelectedUnit Then
					num_result = SelectedAbility
				End If
				etype = ValueType.NumericType
			Case "å¯¾è±¡ãƒ¦ãƒ‹ãƒƒãƒˆä½¿ç”¨ã‚¹ãƒšã‚·ãƒ£ãƒ«ãƒ‘ãƒ¯ãƒ¼"
				If SelectedUnitForEvent Is SelectedUnit Then
					str_result = SelectedSpecialPower
				End If
				etype = ValueType.StringType
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(SelectedAlternative) Then
					num_result = StrToDbl(SelectedAlternative)
					etype = ValueType.NumericType
				Else
					str_result = SelectedAlternative
					etype = ValueType.StringType
				End If
			Case "ã‚¿ãƒ¼ãƒ³æ•°"
				num_result = Turn
				etype = ValueType.NumericType
			Case "ç·ã‚¿ãƒ¼ãƒ³æ•°"
				num_result = TotalTurn
				etype = ValueType.NumericType
			Case "ãƒ•ã‚§ã‚¤ã‚º"
				str_result = Stage
				etype = ValueType.StringType
			Case "å‘³æ–¹æ•°", "Invalid_string_refer_to_original_code", "æ•µæ•°", "ä¸­ç«‹æ•°"
				num = 0
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						num = num + 1
						'End If
					End With
				Next u
				num_result = num
				etype = ValueType.NumericType
			Case "Invalid_string_refer_to_original_code"
				num_result = Money
				etype = ValueType.NumericType
			Case Else
				'Invalid_string_refer_to_original_code
				Select Case LCase(vname)
					Case "apppath"
						str_result = AppPath
						etype = ValueType.StringType
					Case "appversion"
						'UPGRADE_ISSUE: App ƒIƒuƒWƒFƒNƒg ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						With App
							num = 10000 * My.Application.Info.Version.Major + 100 * My.Application.Info.Version.Minor + My.Application.Info.Version.Revision
						End With
						num_result = num
						etype = ValueType.NumericType
					Case "argnum"
						num = ArgIndex - ArgIndexStack(CallDepth - 1 - UpVarLevel)
						num_result = num
						etype = ValueType.NumericType
					Case "basex"
						num_result = BaseX
						etype = ValueType.NumericType
					Case "basey"
						num_result = BaseY
						etype = ValueType.NumericType
					Case "extdatapath"
						str_result = ExtDataPath
						etype = ValueType.StringType
					Case "extdatapath2"
						str_result = ExtDataPath2
						etype = ValueType.StringType
					Case "mousex"
						num_result = MouseX
						etype = ValueType.NumericType
					Case "mousey"
						num_result = MouseY
						etype = ValueType.NumericType
					Case "now"
						str_result = CStr(Now)
						etype = ValueType.StringType
					Case "scenariopath"
						str_result = ScenarioPath
						etype = ValueType.StringType
				End Select
		End Select
		
		If etype <> ValueType.UndefinedType Then
			GetVariableObject = New VarData
			With GetVariableObject
				.Name = vname
				.VariableType = etype
				.StringValue = str_result
				.NumericValue = num_result
			End With
		End If
	End Function
End Module