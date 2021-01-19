Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Module Expression
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'イベントデータの式計算を行うモジュール
	
	'演算子の種類
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
	
	'型の種類
	Enum ValueType
		UndefinedType = 0
		StringType
		NumericType
	End Enum
	
	'正規表現
	Private RegEx As Object
	Private Matches As Object
	
	
	'式を評価
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
		
		'式をあらかじめ要素に分解
		tnum = ListSplit(expr, terms)
		
		Select Case tnum
			'空白
			Case 0
				EvalExpr = etype
				Exit Function
				
				'項
			Case 1
				EvalExpr = EvalTerm(terms(1), etype, str_result, num_result)
				Exit Function
				
				'括弧の対応が取れてない文字列
			Case -1
				If etype = ValueType.NumericType Then
					'0とみなす
					EvalExpr = ValueType.NumericType
				Else
					EvalExpr = ValueType.StringType
					str_result = expr
				End If
				Exit Function
		End Select
		
		'項数が２個以上の場合は演算子を含む式
		
		'優先度に合わせ、どの演算が実行されるかを判定
		op_idx = 0
		op_pri = 100
		For i = 1 To tnum - 1
			'演算子の種類を判定
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
			'単なる文字列
			EvalExpr = ValueType.StringType
			str_result = expr
			Exit Function
		End If
		
		'演算子の引数の作成
		Select Case op_idx
			Case 1
				'左辺引数無し
				is_lop_term = True
				lop = ""
			Case 2
				'左辺引数は項
				is_lop_term = True
				lop = terms(1)
			Case Else
				'左辺引数の連結処理 (高速化のため、Midを使用)
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
			'右辺引数は項
			is_rop_term = True
			rop = terms(tnum)
		Else
			'右辺引数の連結処理 (高速化のため、Midを使用)
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
		
		'演算の実施
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
					'UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
					str_result = FormatNum(lnum Mod rnum)
				Else
					EvalExpr = ValueType.NumericType
					'UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
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
	
	'項を評価
	Public Function EvalTerm(ByRef expr As String, ByRef etype As ValueType, ByRef str_result As String, ByRef num_result As Double) As ValueType
		
		'空白？
		If Len(expr) = 0 Then
			Exit Function
		End If
		
		'先頭の一文字で見分ける
		Select Case Asc(expr)
			Case 9 'タブ
				'タブをTrimするためEvalExprで評価
				EvalTerm = EvalExpr(expr, etype, str_result, num_result)
				Exit Function
			Case 32 '空白
				'Trimされてない？
				EvalTerm = EvalTerm(Trim(expr), etype, str_result, num_result)
				Exit Function
			Case 34 '"
				'ダブルクォートで囲まれた文字列
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
				'色指定
				EvalTerm = ValueType.StringType
				str_result = expr
				Exit Function
			Case 40 '(
				'カッコで囲まれた式
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
			Case 43, 45, 48 To 57 '+, -, 0～9
				'数値？
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
				'バッククォートで囲まれた文字列
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
		
		'関数呼び出し？
		EvalTerm = CallFunction(expr, etype, str_result, num_result)
		If EvalTerm <> ValueType.UndefinedType Then
			Exit Function
		End If
		
		'変数？
		EvalTerm = GetVariable(expr, etype, str_result, num_result)
	End Function
	
	
	' === 関数に関する処理 ===
	
	'式を関数呼び出しとして構文解析し、実行
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
		
		'関数呼び出しの書式に合っているかチェック
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
		
		'ここまでくれば関数呼び出しと断定
		
		'パラメータの抽出
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
					Case 9, 32 'タブ, 空白
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
		
		'先頭の文字で関数の種類を判断する
		Select Case Asc(expr)
			Case 95 '_
				'必ずユーザー定義関数
				fname = Left(expr, j - 1)
				GoTo LookUpUserDefinedID
			Case 65 To 90, 97 To 122 'A To z
				'システム関数の可能性あり
				fname = Left(expr, j - 1)
			Case Else
				'先頭がアルファベットでなければ必ずユーザー定義関数
				'ただし括弧を含むユニット名等である場合があるため、チェックが必要
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
		
		'システム関数？
		Dim PT As POINTAPI
		Dim in_window As Boolean
		Dim x2, x1, y1, y2 As Short
		Dim d1, d2 As Date
		Dim list() As String
		Dim flag As Boolean
		Select Case LCase(fname)
			'多用される関数を先に判定
			Case "args"
				'UpVarコマンドの呼び出し回数を累計
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
				
				'引数の範囲内に納まっているかチェック
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
				'サブルーチンの場所は？
				'まずはサブルーチン名が式でないと仮定して検索
				ret = FindNormalLabel(params(1))
				If ret = 0 Then
					'式で指定されている？
					ret = FindNormalLabel(GetValueAsString(params(1), is_term(1)))
					If ret = 0 Then
						DisplayEventErrorMessage(CurrentLineNum, "指定されたサブルーチン「" & params(1) & "」が見つかりません")
						Exit Function
					End If
				End If
				ret = ret + 1
				
				'呼び出し階層をチェック
				If CallDepth > MaxCallDepth Then
					CallDepth = MaxCallDepth
					DisplayEventErrorMessage(CurrentLineNum, FormatNum(MaxCallDepth) & "階層を越えるサブルーチンの呼び出しは出来ません")
					Exit Function
				End If
				
				'引数用スタックが溢れないかチェック
				If ArgIndex + pcount > MaxArgIndex Then
					DisplayEventErrorMessage(CurrentLineNum, "サブルーチンの引数の総数が" & FormatNum(MaxArgIndex) & "個を超えています")
					Exit Function
				End If
				
				'引数を評価しておく
				For i = 2 To pcount
					params(i) = GetValueAsString(params(i), is_term(i))
				Next 
				
				'現在の状態を保存
				CallStack(CallDepth) = CurrentLineNum
				ArgIndexStack(CallDepth) = ArgIndex
				VarIndexStack(CallDepth) = VarIndex
				ForIndexStack(CallDepth) = ForIndex
				
				'UpVarが実行された場合、UpVar実行数は累計する
				If UpVarLevel > 0 Then
					UpVarLevelStack(CallDepth) = UpVarLevel + UpVarLevelStack(CallDepth - 1)
				Else
					UpVarLevelStack(CallDepth) = 0
				End If
				
				'UpVarの階層数を初期化
				UpVarLevel = 0
				
				'呼び出し階層数をインクリメント
				CallDepth = CallDepth + 1
				cur_depth = CallDepth
				
				'引数をスタックに積む
				ArgIndex = ArgIndex + pcount - 1
				For i = 2 To pcount
					ArgStack(ArgIndex - i + 2) = params(i)
				Next 
				
				'サブルーチン本体を実行
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
				
				'返り値
				With EventCmd(CurrentLineNum)
					If .ArgNum = 2 Then
						str_result = .GetArgAsString(2)
					Else
						str_result = ""
					End If
				End With
				
				'呼び出し階層数をデクリメント
				CallDepth = CallDepth - 1
				
				'サブルーチン実行前の状態に復帰
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
					'params(3)が指定されている場合は、それを検索開始位置似設定
					'VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
					'eve上では引数3に設定するようにしています
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
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					i = InStrB(StrConv(GetValueAsString(params(1), is_term(1)), vbFromUnicode), StrConv(GetValueAsString(params(2), is_term(2)), vbFromUnicode))
				Else
					'params(3)が指定されている場合は、それを検索開始位置似設定
					'VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
					'eve上では引数3に設定するようにしています
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
				
				'全体が()で囲まれている場合は()を外す
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
				
				'これ以降はアルファベット順
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
										If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
											num_result = .Action
										Else
											num_result = 0
										End If
									End With
								End If
							End With
						End If
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
				
				'サブルーチンローカル変数を検索
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
				
				'ローカル変数を検索
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
				
				'グローバル変数を検索
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
							If pname = "未装備" Then
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
						
						'フルパス指定でなければシナリオフォルダを起点に検索
						If Mid(fname, 2, 1) <> ":" Then
							fname = ScenarioPath & fname
						End If
						
						Select Case GetValueAsString(params(2), is_term(2))
							Case "ファイル"
								'UPGRADE_ISSUE: vbNormal をアップグレードする定数を決定できません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"' をクリックしてください。
								num = vbNormal
							Case "フォルダ"
								num = FileAttribute.Directory
						End Select
						'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
						str_result = Dir(fname, num)
						
						If Len(str_result) = 0 Then
							Exit Function
						End If
						
						'ファイル属性チェック用に検索パスを作成
						dir_path = fname
						If num = FileAttribute.Directory Then
							i = InStr2(fname, "\")
							If i > 0 Then
								dir_path = Left(fname, i)
							End If
						End If
						
						'単一ファイルの検索？
						If InStr(fname, "*") = 0 Then
							'フォルダの検索の場合は見つかったファイルがフォルダ
							'かどうかチェックする
							If num = FileAttribute.Directory Then
								If (GetAttr(dir_path & str_result) And num) = 0 Then
									str_result = ""
								End If
							End If
							Exit Function
						End If
						
						If str_result = "." Then
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
							str_result = Dir()
						End If
						If str_result = ".." Then
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
							str_result = Dir()
						End If
						
						'検索されたファイル一覧を作成
						ReDim dir_list(0)
						If num = FileAttribute.Directory Then
							Do While Len(str_result) > 0
								'フォルダの検索の場合は見つかったファイルがフォルダ
								'かどうかチェックする
								If (GetAttr(dir_path & str_result) And num) <> 0 Then
									ReDim Preserve dir_list(UBound(dir_list) + 1)
									dir_list(UBound(dir_list)) = str_result
								End If
								'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
								str_result = Dir()
							Loop 
						Else
							Do While Len(str_result) > 0
								ReDim Preserve dir_list(UBound(dir_list) + 1)
								dir_list(UBound(dir_list)) = str_result
								'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
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
						
						'フルパス指定でなければシナリオフォルダを起点に検索
						If Mid(fname, 2, 1) <> ":" Then
							fname = ScenarioPath & fname
						End If
						
						'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
						str_result = Dir(fname, FileAttribute.Directory)
						
						If Len(str_result) = 0 Then
							Exit Function
						End If
						
						'単一ファイルの検索？
						If InStr(fname, "*") = 0 Then
							Exit Function
						End If
						
						If str_result = "." Then
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
							str_result = Dir()
						End If
						If str_result = ".." Then
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
							str_result = Dir()
						End If
						
						'検索されたファイル一覧を作成
						ReDim dir_list(0)
						Do While Len(str_result) > 0
							ReDim Preserve dir_list(UBound(dir_list) + 1)
							dir_list(UBound(dir_list)) = str_result
							'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
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
					Case "フォント名"
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						str_result = MainForm.picMain(0).Font.Name
						CallFunction = ValueType.StringType
					Case "サイズ"
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						num_result = MainForm.picMain(0).Font.Size
						If etype = ValueType.StringType Then
							str_result = FormatNum(num_result)
							CallFunction = ValueType.StringType
						Else
							CallFunction = ValueType.NumericType
						End If
					Case "太字"
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
					Case "斜体"
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
					Case "色"
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						str_result = Hex(MainForm.picMain(0).ForeColor)
						For i = 1 To 6 - Len(str_result)
							str_result = "0" & str_result
						Next 
						str_result = "#" & str_result
						CallFunction = ValueType.StringType
					Case "書き込み"
						If PermanentStringMode Then
							str_result = "背景"
						ElseIf KeepStringMode Then 
							str_result = "保持"
						Else
							str_result = "通常"
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
				
				'キー番号
				i = GetValueAsLong(params(1), is_term(1))
				
				'左利き設定に対応
				Select Case i
					Case System.Windows.Forms.Keys.LButton
						i = LButtonID
					Case System.Windows.Forms.Keys.RButton
						i = RButtonID
				End Select
				
				If i = System.Windows.Forms.Keys.LButton Or i = System.Windows.Forms.Keys.RButton Then
					'マウスカーソルの位置を参照
					GetCursorPos(PT)
					
					'メインウインドウ上でマウスボタンを押している？
					If System.Windows.Forms.Form.ActiveForm Is MainForm Then
						With MainForm
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							x1 = VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX + .picMain(0).Left + 3
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							y1 = VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY + .picMain(0).Top + 28
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							x2 = x1 + .picMain(0).Width
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							y2 = y1 + .picMain(0).Height
						End With
						
						With PT
							If x1 <= .x And .x <= x2 And y1 <= .y And .y <= y2 Then
								in_window = True
							End If
						End With
					End If
				Else
					'メインウィンドウがアクティブになっている？
					If System.Windows.Forms.Form.ActiveForm Is MainForm Then
						in_window = True
					End If
				End If
				
				'ウィンドウが選択されていない場合は常に0を返す
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
				
				'キーの状態を参照
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
								Else
									With .Unit_Renamed
										If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
											flag = True
										Else
											flag = False
										End If
									End With
								End If
							End With
						Else
							If GetValueAsLong(params(1)) <> 0 Then
								flag = True
							Else
								flag = False
							End If
						End If
					Case 2
						pname = ListIndex(expr, 2)
						If PList.IsDefined(list(2)) Then
							With PList.Item(list(2))
								If .Unit_Renamed Is Nothing Then
									flag = True
								Else
									With .Unit_Renamed
										If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
											flag = False
										Else
											flag = True
										End If
									End With
								End If
							End With
						Else
							If GetValueAsLong(params(1), True) = 0 Then
								flag = True
							Else
								flag = False
							End If
						End If
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
				
				'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
				If LenB(buf2) > 0 And LenB(buf) >= LenB(buf2) Then
					If pcount = 2 Then
						'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
						num = LenB(buf)
					Else
						num = GetValueAsLong(params(3), is_term(3))
					End If
					
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					i = num - LenB(buf2) + 1
					Do 
						'UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
						
						'エリアスが定義されている？
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
						
						'エリアスが定義されている？
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
							Case "パイロット"
								If PList.IsDefined(pname) Then
									If PList.Item(pname).Alive Then
										num_result = 1
									End If
								End If
							Case "ユニット"
								If UList.IsDefined(pname) Then
									If UList.Item(pname).Status_Renamed <> "破棄" Then
										num_result = 1
									End If
								End If
							Case "アイテム"
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
							If UList.Item(pname).Status_Renamed <> "破棄" Then
								num_result = 1
							End If
						ElseIf IList.IsDefined(pname) Then 
							num_result = 1
						End If
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
							If pname = "未装備" Then
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
							If pname = "未装備" Then
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
				'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: LeftB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
				str_result = LeftB$(StrConv(buf, vbFromUnicode), GetValueAsLong(params(2), is_term(2)))
				'UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
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
				'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
				'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
				If LenB(StrConv(buf, vbFromUnicode)) < i Then
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
						'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
						'UPGRADE_ISSUE: MidB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
						str_result = MidB$(StrConv(buf, vbFromUnicode), i, j)
					Case 2
						i = GetValueAsLong(params(2), is_term(2))
						'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
						'UPGRADE_ISSUE: MidB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
						str_result = MidB$(StrConv(buf, vbFromUnicode), i)
				End Select
				'UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
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
				
				'RegExp(文字列, パターン[,大小区別あり|大小区別なし])
				buf = ""
				If pcount > 0 Then
					'文字列全体を検索
					'UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					RegEx.Global = True
					'大文字小文字の区別（True=区別しない）
					'UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					RegEx.IgnoreCase = False
					If pcount >= 3 Then
						If GetValueAsString(params(3), is_term(3)) = "大小区別なし" Then
							'UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							RegEx.IgnoreCase = True
						End If
					End If
					'検索パターン
					'UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					RegEx.Pattern = GetValueAsString(params(2), is_term(2))
					'UPGRADE_WARNING: オブジェクト RegEx.Execute の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					Matches = RegEx.Execute(GetValueAsString(params(1), is_term(1)))
					'UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					If Matches.Count = 0 Then
						regexp_index = -1
					Else
						regexp_index = 0
						'UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						buf = Matches(regexp_index)
					End If
				Else
					If regexp_index >= 0 Then
						regexp_index = regexp_index + 1
						'UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If regexp_index <= Matches.Count - 1 Then
							'UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							buf = Matches(regexp_index)
						End If
					End If
				End If
				str_result = buf
				CallFunction = ValueType.StringType
				Exit Function
RegExp_Error: 
				DisplayEventErrorMessage(CurrentLineNum, "VBScriptがインストールされていません")
				Exit Function
				
			Case "regexpreplace"
				'RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])
				
				On Error GoTo RegExpReplace_Error
				
				If RegEx Is Nothing Then
					RegEx = CreateObject("VBScript.RegExp")
				End If
				
				'文字列全体を検索
				'UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				RegEx.Global = True
				'大文字小文字の区別（True=区別しない）
				'UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				RegEx.IgnoreCase = False
				If pcount >= 4 Then
					If GetValueAsString(params(4), is_term(4)) = "大小区別なし" Then
						'UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						RegEx.IgnoreCase = True
					End If
				End If
				'検索パターン
				'UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				RegEx.Pattern = GetValueAsString(params(2), is_term(2))
				
				'置換実行
				'UPGRADE_WARNING: オブジェクト RegEx.Replace の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				buf = RegEx.Replace(GetValueAsString(params(1), is_term(1)), GetValueAsString(params(3), is_term(3)))
				
				str_result = buf
				CallFunction = ValueType.StringType
				Exit Function
RegExpReplace_Error: 
				DisplayEventErrorMessage(CurrentLineNum, "VBScriptがインストールされていません")
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
				
				num_result = GetValueAsLong("関係:" & pname & ":" & pname2)
				
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
				'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: RightB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
				str_result = RightB$(StrConv(buf, vbFromUnicode), GetValueAsLong(params(2), is_term(2)))
				'UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
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
				'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
				If LenB(StrConv(buf, vbFromUnicode)) < i Then
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
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
						
						'エリアスが定義されている？
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
						
						'エリアスが定義されている？
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
					'String関数では文字列の先頭しか繰り返しされないので、
					'長さが2以上の文字列の場合は別処理
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
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				num_result = MainForm.picMain(0).TextHeight(GetValueAsString(params(1), is_term(1)))
				
				If etype = ValueType.StringType Then
					str_result = FormatNum(num_result)
					CallFunction = ValueType.StringType
				Else
					CallFunction = ValueType.NumericType
				End If
				Exit Function
				
			Case "textwidth"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
							Case "目標地点"
								num_result = SelectedX
							Case "マウス"
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
							Case "目標地点"
								num_result = SelectedY
							Case "マウス"
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
						ElseIf pname = "目標地点" Then 
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
						ElseIf pname = "目標地点" Then 
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
				
				'Date型の処理
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
									str_result = "日曜"
								Case FirstDayOfWeek.Monday
									str_result = "月曜"
								Case FirstDayOfWeek.Tuesday
									str_result = "火曜"
								Case FirstDayOfWeek.Wednesday
									str_result = "水曜"
								Case FirstDayOfWeek.Thursday
									str_result = "木曜"
								Case FirstDayOfWeek.Friday
									str_result = "金曜"
								Case FirstDayOfWeek.Saturday
									str_result = "土曜"
							End Select
						End If
					Case 0
						Select Case WeekDay(Now)
							Case FirstDayOfWeek.Sunday
								str_result = "日曜"
							Case FirstDayOfWeek.Monday
								str_result = "月曜"
							Case FirstDayOfWeek.Tuesday
								str_result = "火曜"
							Case FirstDayOfWeek.Wednesday
								str_result = "水曜"
							Case FirstDayOfWeek.Thursday
								str_result = "木曜"
							Case FirstDayOfWeek.Friday
								str_result = "金曜"
							Case FirstDayOfWeek.Saturday
								str_result = "土曜"
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
				
				'ダイアログ表示
			Case "loadfiledialog"
				Select Case pcount
					Case 2
						str_result = LoadFileDialog("ファイルを開く", ScenarioPath, "", 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
					Case 3
						str_result = LoadFileDialog("ファイルを開く", ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
					Case 4
						str_result = LoadFileDialog("ファイルを開く", ScenarioPath & GetValueAsString(params(4), is_term(4)), GetValueAsString(params(3), is_term(3)), 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				End Select
				
				CallFunction = ValueType.StringType
				
				'本当はこれだけでいいはずだけど……
				If InStr(str_result, ScenarioPath) > 0 Then
					str_result = Mid(str_result, Len(ScenarioPath) + 1)
					Exit Function
				End If
				
				'フルパス指定ならここで終了
				If Right(Left(str_result, 3), 2) = ":\" Then
					str_result = ""
					Exit Function
				End If
				
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				Do Until Dir(ScenarioPath & str_result, FileAttribute.Normal) <> ""
					If InStr(str_result, "\") = 0 Then
						'シナリオフォルダ外のファイルだった
						str_result = ""
						Exit Function
					End If
					str_result = Mid(str_result, InStr(str_result, "\") + 1)
				Loop 
				Exit Function
				
			Case "savefiledialog"
				Select Case pcount
					Case 2
						str_result = SaveFileDialog("ファイルを保存", ScenarioPath, "", 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
					Case 3
						str_result = SaveFileDialog("ファイルを保存", ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
					Case 4
						str_result = SaveFileDialog("ファイルを保存", ScenarioPath & GetValueAsString(params(4), is_term(4)), GetValueAsString(params(3), is_term(3)), 2, GetValueAsString(params(1), is_term(1)), GetValueAsString(params(2), is_term(2)))
				End Select
				
				CallFunction = ValueType.StringType
				
				'本当はこれだけでいいはずだけど……
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
					'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
					If Dir(ScenarioPath & buf, FileAttribute.Directory) <> "" Then
						str_result = buf & "\" & str_result
						Exit Function
					End If
				Loop 
				
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Dir(ScenarioPath & buf, FileAttribute.Directory) <> "" Then
					str_result = buf & "\" & str_result
				End If
				Exit Function
				
		End Select
		
LookUpUserDefinedID: 
		'ユーザー定義関数？
		ret = FindNormalLabel(fname)
		If ret > 0 Then
			'関数が見つかった
			ret = ret + 1
			
			'呼び出し階層をチェック
			If CallDepth > MaxCallDepth Then
				CallDepth = MaxCallDepth
				DisplayEventErrorMessage(CurrentLineNum, FormatNum(MaxCallDepth) & "階層を越えるサブルーチンの呼び出しは出来ません")
				Exit Function
			End If
			
			'引数用スタックが溢れないかチェック
			If ArgIndex + pcount > MaxArgIndex Then
				DisplayEventErrorMessage(CurrentLineNum, "サブルーチンの引数の総数が" & FormatNum(MaxArgIndex) & "個を超えています")
				Exit Function
			End If
			
			'引数の値を先に求めておく
			'(スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
			For i = 1 To pcount
				params(i) = GetValueAsString(params(i), is_term(i))
			Next 
			
			'現在の状態を保存
			CallStack(CallDepth) = CurrentLineNum
			ArgIndexStack(CallDepth) = ArgIndex
			VarIndexStack(CallDepth) = VarIndex
			ForIndexStack(CallDepth) = ForIndex
			UpVarLevelStack(CallDepth) = UpVarLevel
			
			'UpVarの階層数を初期化
			UpVarLevel = 0
			
			'呼び出し階層数をインクリメント
			CallDepth = CallDepth + 1
			cur_depth = CallDepth
			
			'引数をスタックに積む
			ArgIndex = ArgIndex + pcount
			For i = 1 To pcount
				ArgStack(ArgIndex - i + 1) = params(i)
			Next 
			
			'サブルーチン本体を実行
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
			
			'返り値
			With EventCmd(CurrentLineNum)
				If .ArgNum > 1 Then
					str_result = .GetArgAsString(2)
				Else
					str_result = ""
				End If
			End With
			
			'呼び出し階層数をデクリメント
			CallDepth = CallDepth - 1
			
			'サブルーチン実行前の状態に復帰
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
		
		'実はシステム定義のグローバル変数？
		If IsGlobalVariableDefined(expr) Then
			With GlobalVariableList.Item(expr)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = StrToDbl(.StringValue)
						End If
						CallFunction = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = FormatNum(.NumericValue)
						End If
						CallFunction = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
							CallFunction = ValueType.StringType
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
							CallFunction = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'結局ただの文字列……
		str_result = expr
		CallFunction = ValueType.StringType
	End Function
	
	'Info関数の評価
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
		
		'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u = Nothing
		'UPGRADE_NOTE: オブジェクト ud をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		ud = Nothing
		'UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		p = Nothing
		'UPGRADE_NOTE: オブジェクト pd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		pd = Nothing
		'UPGRADE_NOTE: オブジェクト nd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		nd = Nothing
		'UPGRADE_NOTE: オブジェクト it をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		it = Nothing
		'UPGRADE_NOTE: オブジェクト itd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		itd = Nothing
		'UPGRADE_NOTE: オブジェクト spd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		spd = Nothing
		
		'各オブジェクトの設定
		Select Case params(1)
			Case "ユニット"
				u = UList.Item(params(2))
				idx = 3
			Case "ユニットデータ"
				ud = UDList.Item(params(2))
				idx = 3
			Case "パイロット"
				p = PList.Item(params(2))
				idx = 3
			Case "パイロットデータ"
				pd = PDList.Item(params(2))
				idx = 3
			Case "非戦闘員"
				nd = NPDList.Item(params(2))
				idx = 3
			Case "アイテム"
				If IList.IsDefined(params(2)) Then
					it = IList.Item(params(2))
				Else
					itd = IDList.Item(params(2))
				End If
				idx = 3
			Case "アイテムデータ"
				itd = IDList.Item(params(2))
				idx = 3
			Case "スペシャルパワー"
				spd = SPDList.Item(params(2))
				idx = 3
			Case "マップ", "オプション"
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
		
		'UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim mx, my_Renamed As Short
		Select Case params(idx)
			Case "名称"
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
			Case "読み仮名"
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
			Case "愛称"
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
			Case "性別"
				If Not p Is Nothing Then
					EvalInfoFunc = p.Sex
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Sex
				End If
				Exit Function
			Case "ユニットクラス", "機体クラス"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Class_Renamed
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Class_Renamed
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = p.Class_Renamed
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Class_Renamed
				End If
			Case "地形適応"
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
			Case "経験値"
				If Not u Is Nothing Then
					EvalInfoFunc = CStr(u.ExpValue)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = CStr(ud.ExpValue)
				ElseIf Not p Is Nothing Then 
					EvalInfoFunc = CStr(p.ExpValue)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = CStr(pd.ExpValue)
				End If
			Case "格闘"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Infight)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Infight)
				End If
			Case "射撃"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Shooting)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Shooting)
				End If
				Exit Function
			Case "命中"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Hit)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Hit)
				End If
			Case "回避"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Dodge)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Dodge)
				End If
			Case "技量"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Technique)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Technique)
				End If
			Case "反応"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Intuition)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.Intuition)
				End If
			Case "防御"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Defense)
				End If
			Case "格闘基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightBase)
				End If
			Case "射撃基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingBase)
				End If
			Case "命中基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitBase)
				End If
			Case "回避基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeBase)
				End If
			Case "技量基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueBase)
				End If
			Case "反応基本値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionBase)
				End If
			Case "格闘修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightMod)
				End If
			Case "射撃修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingMod)
				End If
			Case "命中修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitMod)
				End If
			Case "回避修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeMod)
				End If
			Case "技量修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueMod)
				End If
			Case "反応修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionMod)
				End If
			Case "格闘支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.InfightMod2)
				End If
			Case "射撃支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.ShootingMod2)
				End If
			Case "命中支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.HitMod2)
				End If
			Case "回避支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.DodgeMod2)
				End If
			Case "技量支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.TechniqueMod2)
				End If
			Case "反応支援修正値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.IntuitionMod2)
				End If
			Case "性格"
				If Not p Is Nothing Then
					EvalInfoFunc = p.Personality
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.Personality
				End If
			Case "最大ＳＰ"
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
			Case "ＳＰ"
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
			Case "グラフィック"
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
			Case "ＭＩＤＩ"
				If Not p Is Nothing Then
					EvalInfoFunc = p.BGM
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = pd.BGM
				End If
			Case "レベル"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Level)
				End If
			Case "累積経験値"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Exp)
				End If
			Case "気力"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Morale)
				End If
			Case "最大霊力", "最大プラーナ"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.MaxPlana)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SkillLevel(0, "霊力"))
				End If
			Case "霊力", "プラーナ"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.Plana)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SkillLevel(0, "霊力"))
				End If
			Case "同調率", "シンクロ率"
				If Not p Is Nothing Then
					EvalInfoFunc = VB6.Format(p.SynchroRate)
				ElseIf Not pd Is Nothing Then 
					EvalInfoFunc = VB6.Format(pd.SkillLevel(0, "同調率"))
				End If
			Case "スペシャルパワー", "精神コマンド", "精神"
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
			Case "スペシャルパワー所有", "精神コマンド所有"
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
			Case "スペシャルパワーコスト", "精神コマンドコスト"
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
			Case "特殊能力数"
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
			Case "特殊能力"
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
			Case "特殊能力名称"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "特殊能力所有"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "特殊能力レベル"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "特殊能力データ"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "特殊能力必要技能"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "特殊能力解説"
				aname = params(idx + 1)
				
				'エリアスが定義されている？
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
			Case "規定パイロット数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Data.PilotNum)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.PilotNum)
				End If
			Case "パイロット数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountPilot)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.PilotNum)
				End If
			Case "サポート数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountSupport)
				End If
			Case "最大アイテム数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Data.ItemNum)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.ItemNum)
				End If
			Case "アイテム数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.CountItem)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.ItemNum)
				End If
			Case "アイテム"
				If Not u Is Nothing Then
					If IsNumber(params(idx + 1)) Then
						i = CShort(params(idx + 1))
						If 0 < i And i <= u.CountItem Then
							EvalInfoFunc = VB6.Format(u.Item(i).Name)
						End If
					End If
				End If
			Case "アイテムＩＤ"
				If Not u Is Nothing Then
					If IsNumber(params(idx + 1)) Then
						i = CShort(params(idx + 1))
						If 0 < i And i <= u.CountItem Then
							EvalInfoFunc = VB6.Format(u.Item(i).ID)
						End If
					End If
				End If
			Case "移動可能地形"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Transportation
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Transportation
				End If
			Case "移動力"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Speed)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Speed)
				End If
			Case "サイズ"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Size
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = ud.Size
				End If
			Case "修理費"
				If Not u Is Nothing Then
					EvalInfoFunc = CStr(u.Value)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = CStr(ud.Value)
				End If
			Case "最大ＨＰ"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxHP)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.HP)
				End If
			Case "ＨＰ"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.HP)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.HP)
				End If
			Case "最大ＥＮ"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxEN)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.EN)
				End If
			Case "ＥＮ"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.EN)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.EN)
				End If
			Case "装甲"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Armor)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Armor)
				End If
			Case "運動性"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Mobility)
				ElseIf Not ud Is Nothing Then 
					EvalInfoFunc = VB6.Format(ud.Mobility)
				End If
			Case "武器数"
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
			Case "武器"
				idx = idx + 1
				If Not u Is Nothing Then
					With u
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						Select Case params(idx)
							Case "", "名称"
								EvalInfoFunc = .Weapon(i).Name
							Case "攻撃力"
								EvalInfoFunc = VB6.Format(.WeaponPower(i, ""))
							Case "射程", "最大射程"
								EvalInfoFunc = VB6.Format(.WeaponMaxRange(i))
							Case "最小射程"
								EvalInfoFunc = VB6.Format(.Weapon(i).MinRange)
							Case "命中率"
								EvalInfoFunc = VB6.Format(.WeaponPrecision(i))
							Case "最大弾数"
								EvalInfoFunc = VB6.Format(.MaxBullet(i))
							Case "弾数"
								EvalInfoFunc = VB6.Format(.Bullet(i))
							Case "消費ＥＮ"
								EvalInfoFunc = VB6.Format(.WeaponENConsumption(i))
							Case "必要気力"
								EvalInfoFunc = VB6.Format(.Weapon(i).NecessaryMorale)
							Case "地形適応"
								EvalInfoFunc = .Weapon(i).Adaption
							Case "クリティカル率"
								EvalInfoFunc = VB6.Format(.WeaponCritical(i))
							Case "属性"
								EvalInfoFunc = .WeaponClass(i)
							Case "属性所有"
								If .IsWeaponClassifiedAs(i, params(idx + 1)) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "属性レベル"
								EvalInfoFunc = CStr(.WeaponLevel(i, params(idx + 1)))
							Case "属性名称"
								EvalInfoFunc = AttributeName(u, params(idx + 1), False)
							Case "属性解説"
								EvalInfoFunc = AttributeHelpMessage(u, params(idx + 1), i, False)
							Case "必要技能"
								EvalInfoFunc = .Weapon(i).NecessarySkill
							Case "使用可"
								If .IsWeaponAvailable(i, "ステータス") Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "修得"
								If .IsWeaponMastered(i) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
						End Select
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "攻撃力"
									EvalInfoFunc = VB6.Format(.Power)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "命中率"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "最大弾数", "弾数"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "地形適応"
									EvalInfoFunc = .Adaption
								Case "クリティカル率"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not p Is Nothing Then 
					With p.Data
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "攻撃力"
									EvalInfoFunc = VB6.Format(.Power)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "命中率"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "最大弾数", "弾数"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "地形適応"
									EvalInfoFunc = .Adaption
								Case "クリティカル率"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not pd Is Nothing Then 
					With pd
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "攻撃力"
									EvalInfoFunc = VB6.Format(.Power)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "命中率"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "最大弾数", "弾数"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "地形適応"
									EvalInfoFunc = .Adaption
								Case "クリティカル率"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not it Is Nothing Then 
					With it
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "攻撃力"
									EvalInfoFunc = VB6.Format(.Power)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "命中率"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "最大弾数", "弾数"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "地形適応"
									EvalInfoFunc = .Adaption
								Case "クリティカル率"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not itd Is Nothing Then 
					With itd
						'何番目の武器かを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountWeapon
								If params(idx) = .Weapon(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定した武器を持っていない
						If i <= 0 Or .CountWeapon < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Weapon(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "攻撃力"
									EvalInfoFunc = VB6.Format(.Power)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "命中率"
									EvalInfoFunc = VB6.Format(.Precision)
								Case "最大弾数", "弾数"
									EvalInfoFunc = VB6.Format(.Bullet)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "地形適応"
									EvalInfoFunc = .Adaption
								Case "クリティカル率"
									EvalInfoFunc = VB6.Format(.Critical)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				End If
			Case "アビリティ数"
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
			Case "アビリティ"
				idx = idx + 1
				If Not u Is Nothing Then
					With u
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						Select Case params(idx)
							Case "", "名称"
								EvalInfoFunc = .Ability(i).Name
							Case "効果数"
								EvalInfoFunc = VB6.Format(.Ability(i).CountEffect)
							Case "効果タイプ"
								'何番目の効果かを判定
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = .Ability(i).EffectType(j)
							Case "効果レベル"
								'何番目の効果かを判定
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = VB6.Format(.Ability(i).EffectLevel(j))
							Case "効果データ"
								'何番目の効果かを判定
								If IsNumber(params(idx + 1)) Then
									j = CShort(params(idx + 1))
								End If
								If j <= 0 And .Ability(i).CountEffect < j Then
									Exit Function
								End If
								EvalInfoFunc = .Ability(i).EffectData(j)
							Case "射程", "最大射程"
								EvalInfoFunc = VB6.Format(.AbilityMaxRange(i))
							Case "最小射程"
								EvalInfoFunc = VB6.Format(.AbilityMinRange(i))
							Case "最大使用回数"
								EvalInfoFunc = VB6.Format(.MaxStock(i))
							Case "使用回数"
								EvalInfoFunc = VB6.Format(.Stock(i))
							Case "消費ＥＮ"
								EvalInfoFunc = VB6.Format(.AbilityENConsumption(i))
							Case "必要気力"
								EvalInfoFunc = VB6.Format(.Ability(i).NecessaryMorale)
							Case "属性"
								EvalInfoFunc = .Ability(i).Class_Renamed
							Case "属性所有"
								If .IsAbilityClassifiedAs(i, params(idx + 1)) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "属性レベル"
								EvalInfoFunc = CStr(.AbilityLevel(i, params(idx + 1)))
							Case "属性名称"
								EvalInfoFunc = AttributeName(u, params(idx + 1), True)
							Case "属性解説"
								EvalInfoFunc = AttributeHelpMessage(u, params(idx + 1), i, True)
							Case "必要技能"
								EvalInfoFunc = .Ability(i).NecessarySkill
							Case "使用可"
								If .IsAbilityAvailable(i, "移動前") Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
							Case "修得"
								If .IsAbilityMastered(i) Then
									EvalInfoFunc = "1"
								Else
									EvalInfoFunc = "0"
								End If
						End Select
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "効果数"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "効果タイプ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "効果レベル"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "効果データ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "最大使用回数", "使用回数"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not p Is Nothing Then 
					With p.Data
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "効果数"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "効果タイプ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "効果レベル"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "効果データ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "最大使用回数", "使用回数"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not pd Is Nothing Then 
					With pd
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "効果数"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "効果タイプ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "効果レベル"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "効果データ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "最大使用回数", "使用回数"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not it Is Nothing Then 
					With it
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "効果数"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "効果タイプ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "効果レベル"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "効果データ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "最大使用回数", "使用回数"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				ElseIf Not itd Is Nothing Then 
					With itd
						'何番目のアビリティかを判定
						If IsNumber(params(idx)) Then
							i = CShort(params(idx))
						Else
							For i = 1 To .CountAbility
								If params(idx) = .Ability(i).Name Then
									Exit For
								End If
							Next 
						End If
						'指定したアビリティを持っていない
						If i <= 0 Or .CountAbility < i Then
							Exit Function
						End If
						
						idx = idx + 1
						With .Ability(i)
							Select Case params(idx)
								Case "", "名称"
									EvalInfoFunc = .Name
								Case "効果数"
									EvalInfoFunc = VB6.Format(.CountEffect)
								Case "効果タイプ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectType(j)
								Case "効果レベル"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = VB6.Format(.EffectLevel(j))
								Case "効果データ"
									'何番目の効果かを判定
									If IsNumber(params(idx + 1)) Then
										j = CShort(params(idx + 1))
									End If
									If j <= 0 Or .CountEffect < j Then
										Exit Function
									End If
									EvalInfoFunc = .EffectData(j)
								Case "射程", "最大射程"
									EvalInfoFunc = VB6.Format(.MaxRange)
								Case "最小射程"
									EvalInfoFunc = VB6.Format(.MinRange)
								Case "最大使用回数", "使用回数"
									EvalInfoFunc = VB6.Format(.Stock)
								Case "消費ＥＮ"
									EvalInfoFunc = VB6.Format(.ENConsumption)
								Case "必要気力"
									EvalInfoFunc = VB6.Format(.NecessaryMorale)
								Case "属性"
									EvalInfoFunc = .Class_Renamed
								Case "属性所有"
									If InStrNotNest(.Class_Renamed, params(idx + 1)) > 0 Then
										EvalInfoFunc = "1"
									Else
										EvalInfoFunc = "0"
									End If
								Case "属性レベル"
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
								Case "必要技能"
									EvalInfoFunc = .NecessarySkill
								Case "使用可", "修得"
									EvalInfoFunc = "1"
							End Select
						End With
					End With
				End If
			Case "ランク"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.Rank)
				End If
			Case "ボスランク"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.BossRank)
				End If
			Case "エリア"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Area
				End If
			Case "思考モード"
				If Not u Is Nothing Then
					EvalInfoFunc = u.Mode
				End If
			Case "最大攻撃力"
				If Not u Is Nothing Then
					With u
						max_value = 0
						For i = 1 To .CountWeapon
							If .IsWeaponMastered(i) And Not .IsDisabled((.Weapon(i).Name)) And Not .IsWeaponClassifiedAs(i, "合") Then
								If .WeaponPower(i, "") > max_value Then
									max_value = .WeaponPower(i, "")
								End If
							End If
						Next 
						EvalInfoFunc = VB6.Format(max_value)
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						max_value = 0
						For i = 1 To .CountWeapon
							If InStr(.Weapon(i).Class_Renamed, "合") = 0 Then
								If .Weapon(i).Power > max_value Then
									max_value = .Weapon(i).Power
								End If
							End If
						Next 
						EvalInfoFunc = VB6.Format(max_value)
					End With
				End If
			Case "最長射程"
				If Not u Is Nothing Then
					With u
						max_value = 0
						For i = 1 To .CountWeapon
							If .IsWeaponMastered(i) And Not .IsDisabled((.Weapon(i).Name)) And Not .IsWeaponClassifiedAs(i, "合") Then
								If .WeaponMaxRange(i) > max_value Then
									max_value = .WeaponMaxRange(i)
								End If
							End If
						Next 
						EvalInfoFunc = VB6.Format(max_value)
					End With
				ElseIf Not ud Is Nothing Then 
					With ud
						max_value = 0
						For i = 1 To .CountWeapon
							If InStr(.Weapon(i).Class_Renamed, "合") = 0 Then
								If .Weapon(i).MaxRange > max_value Then
									max_value = .Weapon(i).MaxRange
								End If
							End If
						Next 
						EvalInfoFunc = VB6.Format(max_value)
					End With
				End If
			Case "残りサポートアタック数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSupportAttack - u.UsedSupportAttack)
				End If
			Case "残りサポートガード数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSupportGuard - u.UsedSupportGuard)
				End If
			Case "残り同時援護攻撃数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxSyncAttack - u.UsedSyncAttack)
				End If
			Case "残りカウンター攻撃数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(u.MaxCounterAttack - u.UsedCounterAttack)
				End If
			Case "改造費"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(RankUpCost(u))
				End If
			Case "最大改造数"
				If Not u Is Nothing Then
					EvalInfoFunc = VB6.Format(MaxRank(u))
				End If
			Case "アイテムクラス"
				If Not it Is Nothing Then
					EvalInfoFunc = it.Class_Renamed()
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Class_Renamed
				End If
			Case "装備個所"
				If Not it Is Nothing Then
					EvalInfoFunc = it.Part
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Part
				End If
			Case "最大ＨＰ修正値"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.HP)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.HP)
				End If
			Case "最大ＥＮ修正値"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.EN)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.EN)
				End If
			Case "装甲修正値"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Armor)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Armor)
				End If
			Case "運動性修正値"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Mobility)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Mobility)
				End If
			Case "移動力修正値"
				If Not it Is Nothing Then
					EvalInfoFunc = VB6.Format(it.Speed)
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = VB6.Format(itd.Speed)
				End If
			Case "解説文", "コメント"
				If Not it Is Nothing Then
					EvalInfoFunc = it.Data.Comment
					ReplaceString(EvalInfoFunc, vbCr & vbLf, " ")
				ElseIf Not itd Is Nothing Then 
					EvalInfoFunc = itd.Comment
					ReplaceString(EvalInfoFunc, vbCr & vbLf, " ")
				ElseIf Not spd Is Nothing Then 
					EvalInfoFunc = spd.Comment
				End If
			Case "短縮名"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.ShortName
				End If
			Case "消費ＳＰ"
				If Not spd Is Nothing Then
					EvalInfoFunc = VB6.Format(spd.SPConsumption)
				End If
			Case "対象"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.TargetType
				End If
			Case "持続期間"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.Duration
				End If
			Case "適用条件"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.NecessaryCondition
				End If
			Case "アニメ"
				If Not spd Is Nothing Then
					EvalInfoFunc = spd.Animation
				End If
			Case "効果数"
				If Not spd Is Nothing Then
					EvalInfoFunc = VB6.Format(spd.CountEffect)
				End If
			Case "効果タイプ"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = spd.EffectType(i)
					End If
				End If
			Case "効果レベル"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = VB6.Format(spd.EffectLevel(i))
					End If
				End If
			Case "効果データ"
				If Not spd Is Nothing Then
					idx = idx + 1
					i = StrToLng(params(idx))
					If 1 <= i And i <= spd.CountEffect Then
						EvalInfoFunc = spd.EffectData(i)
					End If
				End If
			Case "マップ"
				idx = idx + 1
				Select Case params(idx)
					Case "ファイル名"
						EvalInfoFunc = MapFileName
						If Len(EvalInfoFunc) > Len(ScenarioPath) Then
							If Left(EvalInfoFunc, Len(ScenarioPath)) = ScenarioPath Then
								EvalInfoFunc = Mid(EvalInfoFunc, Len(ScenarioPath) + 1)
							End If
						End If
					Case "幅"
						EvalInfoFunc = VB6.Format(MapWidth)
					Case "時間帯"
						If MapDrawMode <> "" Then
							If MapDrawMode = "フィルタ" Then
								buf = Hex(MapDrawFilterColor)
								For i = 1 To 6 - Len(buf)
									buf = "0" & buf
								Next 
								buf = "#" & Mid(buf, 5, 2) & Mid(buf, 3, 2) & Mid(buf, 1, 2) & " " & CStr(MapDrawFilterTransPercent * 100) & "%"
							Else
								buf = MapDrawMode
							End If
							If MapDrawIsMapOnly Then
								buf = buf & " マップ限定"
							End If
							EvalInfoFunc = buf
						Else
							EvalInfoFunc = "昼"
						End If
					Case "高さ"
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
							Case "地形名"
								EvalInfoFunc = TerrainName(mx, my_Renamed)
							Case "地形タイプ", "地形クラス"
								EvalInfoFunc = TerrainClass(mx, my_Renamed)
							Case "移動コスト"
								'0.5刻みの移動コストを使えるようにするため、移動コストは
								'実際の２倍の値で記録されている
								EvalInfoFunc = VB6.Format(TerrainMoveCost(mx, my_Renamed) / 2)
							Case "回避修正"
								EvalInfoFunc = VB6.Format(TerrainEffectForHit(mx, my_Renamed))
							Case "ダメージ修正"
								EvalInfoFunc = VB6.Format(TerrainEffectForDamage(mx, my_Renamed))
							Case "ＨＰ回復量"
								EvalInfoFunc = VB6.Format(TerrainEffectForHPRecover(mx, my_Renamed))
							Case "ＥＮ回復量"
								EvalInfoFunc = VB6.Format(TerrainEffectForENRecover(mx, my_Renamed))
							Case "ビットマップ名"
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
							Case "レイヤービットマップ名"
								Select Case MapImageFileTypeData(mx, my_Renamed)
									Case Map.MapImageFileType.SeparateDirMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & "\" & TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.FourFiguresMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
									Case Map.MapImageFileType.OldMapImageFileType
										EvalInfoFunc = TDList.Bitmap(MapData(mx, my_Renamed, Map.MapDataIndex.LayerType)) & VB6.Format(MapData(mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo)) & ".bmp"
								End Select
								'ADD  END  240a
							Case "ユニットＩＤ"
								If Not MapDataForUnit(mx, my_Renamed) Is Nothing Then
									EvalInfoFunc = MapDataForUnit(mx, my_Renamed).ID
								End If
						End Select
				End Select
			Case "オプション"
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
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
						' 「NewGUI」で探しに来たらINIの状態を返す。「新ＧＵＩ」で探しに来たらOptionの状態を返す。
					Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", "AutoMoveCursor", "DebugMode", "LastFolder", "MIDIPortID", "MP3Volume", "BattleAnimation", "WeaponAnimation", "MoveAnimation", "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", "UseTransparentBlt", "NewGUI"
						' MOD END MARGE
						EvalInfoFunc = ReadIni("Option", params(idx))
					Case Else
						'Optionコマンドのオプションを参照
						If IsOptionDefined(params(idx)) Then
							EvalInfoFunc = "On"
						Else
							EvalInfoFunc = "Off"
						End If
				End Select
		End Select
	End Function
	
	
	' === 変数に関する処理 ===
	
	'変数の値を評価
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
		
		'未定義値の設定
		str_result = var_name
		
		'変数が配列？
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'ここから配列専用の処理
		
		'インデックス部分の切りだし
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'多次元配列の処理
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
						Case 9, 32 'タブ, 空白
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
		
		'変数名を配列のインデックス部を計算して再構築
		vname = Left(vname, ret) & idx & "]"
		
		'定義されていない要素を使って配列を読み出した場合は空文字列を返す
		str_result = ""
		
		'配列専用の処理が終了
		
SkipArrayHandling: 
		
		'ここから配列と通常変数の共通処理
		
		'サブルーチンローカル変数
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
		
		'ローカル変数
		If IsLocalVariableDefined(vname) Then
			With LocalVariableList.Item(vname)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = StrToDbl(.StringValue)
						End If
						GetVariable = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = FormatNum(.NumericValue)
						End If
						GetVariable = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
							GetVariable = ValueType.StringType
						Else
							'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
							GetVariable = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'グローバル変数
		If IsGlobalVariableDefined(vname) Then
			With GlobalVariableList.Item(vname)
				Select Case etype
					Case ValueType.NumericType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.NumericType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = StrToDbl(.StringValue)
						End If
						GetVariable = ValueType.NumericType
					Case ValueType.StringType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = FormatNum(.NumericValue)
						End If
						GetVariable = ValueType.StringType
					Case ValueType.UndefinedType
						'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						If .VariableType = ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							str_result = .StringValue
							GetVariable = ValueType.StringType
						Else
							'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							num_result = .NumericValue
							GetVariable = ValueType.NumericType
						End If
				End Select
			End With
			Exit Function
		End If
		
		'システム変数？
		Select Case vname
			Case "対象ユニット", "対象パイロット"
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
				
			Case "相手ユニット", "相手パイロット"
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
				
			Case "対象ユニットＩＤ"
				If Not SelectedUnitForEvent Is Nothing Then
					str_result = SelectedUnitForEvent.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "相手ユニットＩＤ"
				If Not SelectedTargetForEvent Is Nothing Then
					str_result = SelectedTargetForEvent.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "対象ユニット使用武器"
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
				
			Case "相手ユニット使用武器"
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
				
			Case "対象ユニット使用武器番号"
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
				
			Case "相手ユニット使用武器番号"
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
				
			Case "対象ユニット使用アビリティ"
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
				
			Case "対象ユニット使用アビリティ番号"
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
				
			Case "対象ユニット使用スペシャルパワー"
				str_result = ""
				If SelectedUnitForEvent Is SelectedUnit Then
					str_result = SelectedSpecialPower
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "サポートアタックユニットＩＤ"
				If Not SupportAttackUnit Is Nothing Then
					str_result = SupportAttackUnit.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "サポートガードユニットＩＤ"
				If Not SupportGuardUnit Is Nothing Then
					str_result = SupportGuardUnit.ID
				Else
					str_result = ""
				End If
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "選択"
				If etype = ValueType.NumericType Then
					num_result = StrToDbl(SelectedAlternative)
					GetVariable = ValueType.NumericType
				Else
					str_result = SelectedAlternative
					GetVariable = ValueType.StringType
				End If
				Exit Function
				
			Case "ターン数"
				If etype = ValueType.StringType Then
					str_result = VB6.Format(Turn)
					GetVariable = ValueType.StringType
				Else
					num_result = Turn
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "総ターン数"
				If etype = ValueType.StringType Then
					str_result = VB6.Format(TotalTurn)
					GetVariable = ValueType.StringType
				Else
					num_result = TotalTurn
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case "フェイズ"
				str_result = Stage
				GetVariable = ValueType.StringType
				Exit Function
				
			Case "味方数"
				num = 0
				For	Each u In UList
					With u
						If .Party0 = "味方" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
							num = num + 1
						End If
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
				
			Case "ＮＰＣ数"
				num = 0
				For	Each u In UList
					With u
						If .Party0 = "ＮＰＣ" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
							num = num + 1
						End If
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
				
			Case "敵数"
				num = 0
				For	Each u In UList
					With u
						If .Party0 = "敵" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
							num = num + 1
						End If
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
				
			Case "中立数"
				num = 0
				For	Each u In UList
					With u
						If .Party0 = "中立" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
							num = num + 1
						End If
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
				
			Case "資金"
				If etype = ValueType.StringType Then
					str_result = FormatNum(Money)
					GetVariable = ValueType.StringType
				Else
					num_result = Money
					GetVariable = ValueType.NumericType
				End If
				Exit Function
				
			Case Else
				'アルファベットの変数名はlow caseで判別
				Select Case LCase(vname)
					Case "apppath"
						str_result = AppPath
						GetVariable = ValueType.StringType
						Exit Function
						
					Case "appversion"
						'UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
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
						'UpVarの呼び出し回数を累計
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
		
		'コンフィグ変数？
		If BCVariable.IsConfig Then
			Select Case vname
				Case "攻撃値"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.AttackExp)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.AttackExp
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "攻撃側ユニットＩＤ"
					str_result = BCVariable.AtkUnit.ID
					GetVariable = ValueType.StringType
					Exit Function
				Case "防御側ユニットＩＤ"
					If Not BCVariable.DefUnit Is Nothing Then
						str_result = BCVariable.DefUnit.ID
						GetVariable = ValueType.StringType
						Exit Function
					End If
				Case "武器番号"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.WeaponNumber)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.WeaponNumber
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "地形適応"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.TerrainAdaption)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.TerrainAdaption
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "武器威力"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.WeaponPower)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.WeaponPower
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "サイズ補正"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.SizeMod)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.SizeMod
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "装甲値"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.Armor)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.Armor
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "最終値"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.LastVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.LastVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "攻撃側補正"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.AttackVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.AttackVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "防御側補正"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.DffenceVariable)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.DffenceVariable
						GetVariable = ValueType.NumericType
					End If
					Exit Function
				Case "ザコ補正"
					If etype = ValueType.StringType Then
						str_result = VB6.Format(BCVariable.CommonEnemy)
						GetVariable = ValueType.StringType
					Else
						num_result = BCVariable.CommonEnemy
						GetVariable = ValueType.NumericType
					End If
					Exit Function
			End Select
			
			'パイロットに関する変数
			With BCVariable.MeUnit.MainPilot
				Select Case vname
					Case "気力"
						num = 0
						
						If IsOptionDefined("気力効果小") Then
							num = 50 + (.Morale + .MoraleMod) \ 2 ' 気力の補正込み値を代入
						Else
							num = .Morale + .MoraleMod ' 気力の補正込み値を代入
						End If
						
						If etype = ValueType.StringType Then
							str_result = VB6.Format(num)
							GetVariable = ValueType.StringType
						Else
							num_result = num
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "耐久"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Defense)
							GetVariable = ValueType.StringType
						Else
							num_result = .Defense
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "ＬＶ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Level)
							GetVariable = ValueType.StringType
						Else
							num_result = .Level
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "経験"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Exp)
							GetVariable = ValueType.StringType
						Else
							num_result = .Exp
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "ＳＰ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.SP)
							GetVariable = ValueType.StringType
						Else
							num_result = .SP
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "霊力"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Plana)
							GetVariable = ValueType.StringType
						Else
							num_result = .Plana
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "格闘"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Infight)
							GetVariable = ValueType.StringType
						Else
							num_result = .Infight
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "射撃"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Shooting)
							GetVariable = ValueType.StringType
						Else
							num_result = .Shooting
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "命中"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Hit)
							GetVariable = ValueType.StringType
						Else
							num_result = .Hit
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "回避"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Dodge)
							GetVariable = ValueType.StringType
						Else
							num_result = .Dodge
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "技量"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Technique)
							GetVariable = ValueType.StringType
						Else
							num_result = .Technique
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "反応"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Intuition)
							GetVariable = ValueType.StringType
						Else
							num_result = .Intuition
							GetVariable = ValueType.NumericType
						End If
						Exit Function
				End Select
			End With
			
			'ユニットに関する変数
			With BCVariable.MeUnit
				Select Case vname
					Case "最大ＨＰ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.MaxHP())
							GetVariable = ValueType.StringType
						Else
							num_result = .MaxHP()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "現在ＨＰ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.HP())
							GetVariable = ValueType.StringType
						Else
							num_result = .HP()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "最大ＥＮ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.MaxEN())
							GetVariable = ValueType.StringType
						Else
							num_result = .MaxEN()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "現在ＥＮ"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.EN())
							GetVariable = ValueType.StringType
						Else
							num_result = .EN()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "移動力"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Speed())
							GetVariable = ValueType.StringType
						Else
							num_result = .Speed()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "装甲"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Armor())
							GetVariable = ValueType.StringType
						Else
							num_result = .Armor()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
					Case "運動性"
						If etype = ValueType.StringType Then
							str_result = VB6.Format(.Mobility())
							GetVariable = ValueType.StringType
						Else
							num_result = .Mobility()
							GetVariable = ValueType.NumericType
						End If
						Exit Function
				End Select
			End With
		End If
		
		If etype = ValueType.NumericType Then
			num_result = 0
			GetVariable = ValueType.NumericType
		Else
			GetVariable = ValueType.StringType
		End If
	End Function
	
	'指定した変数が定義されているか？
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
		
		'変数が配列？
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'ここから配列専用の処理
		
		'インデックス部分の切りだし
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'多次元配列の処理
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
						Case 9, 32 'タブ, 空白
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
		
		'変数名を配列のインデックス部を計算して再構築
		vname = Left(vname, ret) & idx & "]"
		
		'配列専用の処理が終了
		
SkipArrayHandling: 
		
		'ここから配列と通常変数の共通処理
		
		'サブルーチンローカル変数
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				If vname = VarStack(i).Name Then
					IsVariableDefined = True
					Exit Function
				End If
			Next 
		End If
		
		'ローカル変数
		If IsLocalVariableDefined(vname) Then
			IsVariableDefined = True
			Exit Function
		End If
		
		'グローバル変数
		If IsGlobalVariableDefined(vname) Then
			IsVariableDefined = True
			Exit Function
		End If
	End Function
	
	'指定した名前のサブルーチンローカル変数が定義されているか？
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
	
	'指定した名前のローカル変数が定義されているか？
	Public Function IsLocalVariableDefined(ByRef vname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = LocalVariableList.Item(vname)
		IsLocalVariableDefined = True
		Exit Function
		
ErrorHandler: 
		IsLocalVariableDefined = False
	End Function
	
	'指定した名前のグローバル変数が定義されているか？
	Public Function IsGlobalVariableDefined(ByRef vname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = GlobalVariableList.Item(vname)
		IsGlobalVariableDefined = True
		Exit Function
		
ErrorHandler: 
		IsGlobalVariableDefined = False
	End Function
	
	'変数の値を設定
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
		
		'左辺値を伴う関数
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
						If u.EN = 0 And u.Status_Renamed = "出撃" Then
							PaintUnitBitmap(u)
						End If
					End If
					Exit Sub
					
				Case "sp"
					idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
					idx = GetValueAsString(idx)
					
					If UList.IsDefined2(idx) Then
						p = UList.Item2(idx).MainPilot
					ElseIf PList.IsDefined(idx) Then 
						p = PList.Item(idx)
					Else
						p = SelectedUnitForEvent.MainPilot
					End If
					
					If Not p Is Nothing Then
						With p
							If .MaxSP > 0 Then
								If etype = ValueType.NumericType Then
									.SP = num_value
								Else
									.SP = StrToLng(str_value)
								End If
							End If
						End With
					End If
					Exit Sub
					
				Case "plana"
					idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
					idx = GetValueAsString(idx)
					
					If UList.IsDefined2(idx) Then
						p = UList.Item2(idx).MainPilot
					ElseIf PList.IsDefined(idx) Then 
						p = PList.Item(idx)
					Else
						p = SelectedUnitForEvent.MainPilot
					End If
					
					If Not p Is Nothing Then
						With p
							If .MaxPlana > 0 Then
								If etype = ValueType.NumericType Then
									.Plana = num_value
								Else
									.Plana = StrToLng(str_value)
								End If
							End If
						End With
					End If
					Exit Sub
					
				Case "action"
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
							u.UsedAction = u.MaxAction - num_value
						Else
							u.UsedAction = u.MaxAction - StrToLng(str_value)
						End If
					End If
					Exit Sub
					
				Case "eval"
					vname = Trim(Mid(vname, ret + 1, Len(vname) - ret - 1))
					vname = GetValueAsString(vname)
					
			End Select
		End If
		
		'変数が配列？
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'ここから配列専用の処理
		
		'インデックス部分の切りだし
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'多次元配列の処理
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
						Case 9, 32 'タブ, 空白
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
			idx = GetValueAsString(Trim(idx))
		End If
		
		'変数名を配列のインデックス部を計算して再構築
		vname = Left(vname, ret) & idx & "]"
		
		'配列名
		vname0 = Left(vname, ret - 1)
		
		'サブルーチンローカルな配列として定義済みかどうかチェック
		If IsSubLocalVariableDefined(vname0) Then
			is_subroutine_local_array = True
		End If
		
		'配列専用の処理が終了
		
SkipArrayHandling: 
		
		'ここから配列と通常変数の共通処理
		
		'サブルーチンローカル変数として定義済み？
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				With VarStack(i)
					If vname = .Name Then
						.VariableType = etype
						.StringValue = str_value
						.NumericValue = num_value
						Exit Sub
					End If
				End With
			Next 
		End If
		
		If is_subroutine_local_array Then
			'サブルーチンローカル変数の配列の要素として定義
			VarIndex = VarIndex + 1
			If VarIndex > MaxVarIndex Then
				VarIndex = MaxVarIndex
				DisplayEventErrorMessage(CurrentLineNum, "作成したサブルーチンローカル変数の総数が" & VB6.Format(MaxVarIndex) & "個を超えています")
				Exit Sub
			End If
			With VarStack(VarIndex)
				.Name = vname
				.VariableType = etype
				.StringValue = str_value
				.NumericValue = num_value
			End With
			Exit Sub
		End If
		
		'ローカル変数として定義済み？
		If IsLocalVariableDefined(vname) Then
			With LocalVariableList.Item(vname)
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.Name = vname
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.VariableType = etype
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.StringValue = str_value
				'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.NumericValue = num_value
			End With
			Exit Sub
		End If
		
		'グローバル変数として定義済み？
		If IsGlobalVariableDefined(vname) Then
			With GlobalVariableList.Item(vname)
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.Name = vname
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.VariableType = etype
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.StringValue = str_value
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.NumericValue = num_value
			End With
			Exit Sub
		End If
		
		'システム変数？
		Select Case LCase(vname)
			Case "basex"
				If etype = ValueType.NumericType Then
					BaseX = num_value
				Else
					BaseX = StrToLng(str_value)
				End If
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).CurrentX = BaseX
				Exit Sub
			Case "basey"
				If etype = ValueType.NumericType Then
					BaseY = num_value
				Else
					BaseY = StrToLng(str_value)
				End If
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).CurrentY = BaseY
				Exit Sub
			Case "ターン数"
				If etype = ValueType.NumericType Then
					Turn = num_value
				Else
					Turn = StrToLng(str_value)
				End If
				Exit Sub
			Case "総ターン数"
				If etype = ValueType.NumericType Then
					TotalTurn = num_value
				Else
					TotalTurn = StrToLng(str_value)
				End If
				Exit Sub
			Case "資金"
				Money = 0
				If etype = ValueType.NumericType Then
					IncrMoney(num_value)
				Else
					IncrMoney(StrToLng(str_value))
				End If
				Exit Sub
		End Select
		
		'未定義だった場合
		
		'配列の要素として作成
		Dim new_var2 As VarData
		If Len(vname0) <> 0 Then
			'ローカル変数の配列の要素として定義
			If IsLocalVariableDefined(vname0) Then
				'Nop
				'グローバル変数の配列の要素として定義
			ElseIf IsGlobalVariableDefined(vname0) Then 
				DefineGlobalVariable(vname)
				With GlobalVariableList.Item(vname)
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.Name = vname
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.VariableType = etype
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.StringValue = str_value
					'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					.NumericValue = num_value
				End With
				Exit Sub
				'未定義の配列なのでローカル変数の配列を作成
			Else
				'ローカル変数の配列のメインＩＤを作成
				new_var2 = New VarData
				With new_var2
					.Name = vname0
					.VariableType = ValueType.StringType
					If InStr(.Name, """") > 0 Then
						DisplayEventErrorMessage(CurrentLineNum, "不正な変数「" & .Name & "」が作成されました")
					End If
				End With
				LocalVariableList.Add(new_var2, vname0)
			End If
		End If
		
		'ローカル変数として作成
		new_var = New VarData
		With new_var
			.Name = vname
			.VariableType = etype
			.StringValue = str_value
			.NumericValue = num_value
			If InStr(.Name, """") > 0 Then
				DisplayEventErrorMessage(CurrentLineNum, "不正な変数「" & .Name & "」が作成されました")
			End If
		End With
		LocalVariableList.Add(new_var, vname)
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
	
	'グローバル変数を定義
	Public Sub DefineGlobalVariable(ByRef vname As String)
		Dim new_var As New VarData
		
		With new_var
			.Name = vname
			.VariableType = ValueType.StringType
			.StringValue = ""
		End With
		GlobalVariableList.Add(new_var, vname)
	End Sub
	
	'ローカル変数を定義
	Public Sub DefineLocalVariable(ByRef vname As String)
		Dim new_var As New VarData
		
		With new_var
			.Name = vname
			.VariableType = ValueType.StringType
			.StringValue = ""
		End With
		LocalVariableList.Add(new_var, vname)
	End Sub
	
	'変数を消去
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
		
		'Eval関数
		If LCase(Left(vname, 5)) = "eval(" Then
			If Right(vname, 1) = ")" Then
				vname = Mid(vname, 6, Len(vname) - 6)
				vname = GetValueAsString(vname)
			End If
		End If
		
		'配列の要素？
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'配列の要素を指定された場合
		
		'インデックス部分の切りだし
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'多次元配列の処理
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
						Case 9, 32 'タブ, 空白
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
		
		'インデックス部分を評価して変数名を置き換え
		vname = Left(vname, ret) & idx & "]"
		
		'サブルーチンローカル変数？
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
		
		'ローカル変数？
		If IsLocalVariableDefined(vname) Then
			LocalVariableList.Remove(vname)
			Exit Sub
		End If
		
		'グローバル変数？
		If IsGlobalVariableDefined(vname) Then
			GlobalVariableList.Remove(vname)
		End If
		
		'配列の場合はここで終了
		Exit Sub
		
SkipArrayHandling: 
		
		'通常の変数名を指定された場合
		
		'配列要素の判定用
		vname2 = vname & "["
		
		'サブルーチンローカル変数？
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
		
		'ローカル変数？
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
		
		'グローバル変数？
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
	
	
	
	' === その他の関数 ===
	
	'式を文字列として評価
	Public Function GetValueAsString(ByRef expr As String, Optional ByVal is_term As Boolean = False) As String
		Dim num As Double
		
		If is_term Then
			EvalTerm(expr, ValueType.StringType, GetValueAsString, num)
		Else
			EvalExpr(expr, ValueType.StringType, GetValueAsString, num)
		End If
	End Function
	
	'式を浮動小数点数として評価
	Public Function GetValueAsDouble(ByRef expr As String, Optional ByVal is_term As Boolean = False) As Double
		Dim buf As String
		
		If is_term Then
			EvalTerm(expr, ValueType.NumericType, buf, GetValueAsDouble)
		Else
			EvalExpr(expr, ValueType.NumericType, buf, GetValueAsDouble)
		End If
	End Function
	
	'式を整数として評価
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
	
	
	'strが式かどうかチェック
	'(疑わしきは式と判断している)
	'UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Function IsExpr(ByRef str_Renamed As String) As Boolean
		Select Case Asc(str_Renamed)
			Case 36 '$
				IsExpr = True
			Case 40 '(
				IsExpr = True
		End Select
	End Function
	
	
	'指定したオプションが設定されているか？
	Public Function IsOptionDefined(ByRef oname As String) As Boolean
		Dim dummy As VarData
		
		On Error GoTo ErrorHandler
		dummy = GlobalVariableList.Item("Option(" & oname & ")")
		IsOptionDefined = True
		Exit Function
		
ErrorHandler: 
		IsOptionDefined = False
	End Function
	
	
	'str に対して式置換を行う
	'UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Sub ReplaceSubExpression(ByRef str_Renamed As String)
		Dim start_idx, end_idx As Short
		Dim str_len As Short
		Dim i, n As Short
		
		Do While True
			'式置換が存在する？
			start_idx = InStr(str_Renamed, "$(")
			If start_idx = 0 Then
				Exit Sub
			End If
			
			'式置換の終了位置を調べる
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
			
			'式置換を実施
			str_Renamed = Left(str_Renamed, start_idx - 1) & GetValueAsString(Mid(str_Renamed, start_idx + 2, end_idx - start_idx - 2)) & Right(str_Renamed, str_len - end_idx)
		Loop 
	End Sub
	
	'msg に対して式置換等の処理を行う
	Public Sub FormatMessage(ByRef msg As String)
		'ちゃんと横棒がつながって表示されるように罫線文字に置換
		If ReplaceString(msg, "――", "──") Then
			ReplaceString(msg, "─―", "──")
		ElseIf ReplaceString(msg, "ーー", "──") Then 
			ReplaceString(msg, "─ー", "──")
		End If
		
		'式置換
		ReplaceSubExpression(msg)
	End Sub
	
	
	'用語tnameの表示名を参照する
	'tlenが指定された場合は文字列長を強制的にtlenに合わせる
	Public Function Term(ByRef tname As String, Optional ByRef u As Unit = Nothing, Optional ByVal tlen As Short = 0) As String
		Dim vname As String
		Dim i As Short
		
		'ユニットが用語名能力を持っている場合はそちらを優先
		If Not u Is Nothing Then
			With u
				If .IsFeatureAvailable("用語名") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "用語名" Then
							If LIndex(.FeatureData(i), 1) = tname Then
								Term = LIndex(.FeatureData(i), 2)
								Exit For
							End If
						End If
					Next 
				End If
			End With
		End If
		
		'RenameTermで用語名が変更されているかチェック
		If Len(Term) = 0 Then
			Select Case tname
				Case "HP", "EN", "SP", "CT"
					vname = "ShortTerm(" & tname & ")"
				Case Else
					vname = "Term(" & tname & ")"
			End Select
			If IsGlobalVariableDefined(vname) Then
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Term = GlobalVariableList.Item(vname).StringValue
			Else
				Term = tname
			End If
		End If
		
		'表示幅の調整
		If tlen > 0 Then
			'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
			If LenB(StrConv(Term, vbFromUnicode)) < tlen Then
				Term = RightPaddedString(Term, tlen)
			End If
		End If
	End Function
	
	
	'引数1で指定した変数のオブジェクトを取得
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
		
		'変数が配列？
		ret = InStr(vname, "[")
		If ret = 0 Then
			GoTo SkipArrayHandling
		End If
		If Right(vname, 1) <> "]" Then
			GoTo SkipArrayHandling
		End If
		
		'ここから配列専用の処理
		
		'インデックス部分の切りだし
		idx = Mid(vname, ret + 1, Len(vname) - ret - 1)
		
		'多次元配列の処理
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
						Case 9, 32 'タブ, 空白
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
		
		'変数名を配列のインデックス部を計算して再構築
		vname = Left(vname, ret) & idx & "]"
		
		'配列専用の処理が終了
		
SkipArrayHandling: 
		
		'ここから配列と通常変数の共通処理
		
		'サブルーチンローカル変数
		If CallDepth > 0 Then
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				If vname = VarStack(i).Name Then
					GetVariableObject = VarStack(i)
					Exit Function
				End If
			Next 
		End If
		
		'ローカル変数
		If IsLocalVariableDefined(vname) Then
			GetVariableObject = LocalVariableList.Item(vname)
			Exit Function
		End If
		
		'グローバル変数
		If IsGlobalVariableDefined(vname) Then
			GetVariableObject = GlobalVariableList.Item(vname)
			Exit Function
		End If
		
		'システム変数？
		etype = ValueType.UndefinedType
		str_result = ""
		num_result = 0
		Select Case vname
			Case "対象ユニット", "対象パイロット"
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
			Case "相手ユニット", "相手パイロット"
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
			Case "対象ユニットＩＤ"
				If Not SelectedUnitForEvent Is Nothing Then
					str_result = SelectedUnitForEvent.ID
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "相手ユニットＩＤ"
				If Not SelectedTargetForEvent Is Nothing Then
					str_result = SelectedTargetForEvent.ID
				Else
					str_result = ""
				End If
				etype = ValueType.StringType
			Case "対象ユニット使用武器"
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
			Case "相手ユニット使用武器"
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
			Case "対象ユニット使用武器番号"
				If SelectedUnitForEvent Is SelectedUnit Then
					num_result = SelectedWeapon
				ElseIf SelectedUnitForEvent Is SelectedTarget Then 
					num_result = SelectedTWeapon
				End If
				etype = ValueType.NumericType
			Case "相手ユニット使用武器番号"
				If SelectedTargetForEvent Is SelectedTarget Then
					num_result = SelectedTWeapon
				ElseIf SelectedTargetForEvent Is SelectedUnit Then 
					num_result = SelectedWeapon
				End If
				etype = ValueType.NumericType
			Case "対象ユニット使用アビリティ"
				If SelectedUnitForEvent Is SelectedUnit Then
					If SelectedAbility > 0 Then
						str_result = SelectedAbilityName
					Else
						str_result = ""
					End If
				End If
				etype = ValueType.StringType
			Case "対象ユニット使用アビリティ番号"
				If SelectedUnitForEvent Is SelectedUnit Then
					num_result = SelectedAbility
				End If
				etype = ValueType.NumericType
			Case "対象ユニット使用スペシャルパワー"
				If SelectedUnitForEvent Is SelectedUnit Then
					str_result = SelectedSpecialPower
				End If
				etype = ValueType.StringType
			Case "選択"
				If IsNumeric(SelectedAlternative) Then
					num_result = StrToDbl(SelectedAlternative)
					etype = ValueType.NumericType
				Else
					str_result = SelectedAlternative
					etype = ValueType.StringType
				End If
			Case "ターン数"
				num_result = Turn
				etype = ValueType.NumericType
			Case "総ターン数"
				num_result = TotalTurn
				etype = ValueType.NumericType
			Case "フェイズ"
				str_result = Stage
				etype = ValueType.StringType
			Case "味方数", "ＮＰＣ数", "敵数", "中立数"
				num = 0
				For	Each u In UList
					With u
						If .Party0 = Left(vname, Len(vname) - 1) And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
							num = num + 1
						End If
					End With
				Next u
				num_result = num
				etype = ValueType.NumericType
			Case "資金"
				num_result = Money
				etype = ValueType.NumericType
			Case Else
				'アルファベットの変数名はlow caseで判別
				Select Case LCase(vname)
					Case "apppath"
						str_result = AppPath
						etype = ValueType.StringType
					Case "appversion"
						'UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
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