Attribute VB_Name = "Expression"
Option Explicit

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
Public Function EvalExpr(expr As String, etype As ValueType, _
    str_result As String, num_result As Double) As ValueType
Dim terms() As String, tnum As Integer
Dim op_idx As Integer, op_pri As Integer, op_type As OperatorType
Dim lop As String, rop As String
Dim lstr As String, rstr As String
Dim lnum As Double, rnum As Double
Dim is_lop_term As Boolean, is_rop_term As Boolean
Dim i As Integer, ret As Integer, osize As Integer, tsize As Integer
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
            If etype = NumericType Then
                '0とみなす
                EvalExpr = NumericType
            Else
                EvalExpr = StringType
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
                            op_type = ExpoOp
                            op_pri = 10
                            op_idx = i
                        End If
                    Case 42 '*
                        If op_pri >= 9 Then
                            op_type = MultOp
                            op_pri = 9
                            op_idx = i
                        End If
                    Case 47 '/
                        If op_pri >= 9 Then
                            op_type = DivOp
                            op_pri = 9
                            op_idx = i
                        End If
                    Case 92 '\
                        If op_pri >= 8 Then
                            op_type = IntDivOp
                            op_pri = 8
                            op_idx = i
                        End If
                    Case 43 '+
                        If op_pri >= 6 Then
                            op_type = PlusOp
                            op_pri = 6
                            op_idx = i
                        End If
                    Case 45 '-
                        If op_pri >= 6 Then
                            op_type = MinusOp
                            op_pri = 6
                            op_idx = i
                        End If
                    Case 38 '&
                        If op_pri >= 5 Then
                            op_type = CatOp
                            op_pri = 5
                            op_idx = i
                        End If
                    Case 60 '<
                        If op_pri >= 4 Then
                            op_type = LtOp
                            op_pri = 4
                            op_idx = i
                        End If
                    Case 61 '=
                        If op_pri >= 4 Then
                            op_type = EqOp
                            op_pri = 4
                            op_idx = i
                        End If
                    Case 62 '>
                        If op_pri >= 4 Then
                            op_type = GtOp
                            op_pri = 4
                            op_idx = i
                        End If
                End Select
            Case 2
                Select Case ret
                    Case 33 '!=
                        If op_pri >= 4 Then
                            If Right$(terms(i), 1) = "=" Then
                                op_type = NotEqOp
                                op_pri = 4
                                op_idx = i
                            End If
                        End If
                    Case 60 '<>, <=
                        If op_pri >= 4 Then
                            Select Case Right$(terms(i), 1)
                                Case ">"
                                   op_type = NotEqOp
                                   op_pri = 4
                                   op_idx = i
                                Case "="
                                   op_type = LtEqOp
                                   op_pri = 4
                                   op_idx = i
                            End Select
                        End If
                    Case 62 '>=
                        If op_pri >= 4 Then
                            If Right$(terms(i), 1) = "=" Then
                                op_type = GtEqOp
                                op_pri = 4
                                op_idx = i
                            End If
                        End If
                    Case 79, 111 'or
                        If op_pri > 1 Then
                            If LCase$(terms(i)) = "or" Then
                                op_type = OrOp
                                op_pri = 1
                                op_idx = i
                            End If
                        End If
                End Select
            Case 3
                Select Case ret
                    Case 77, 109 'mod
                        If op_pri >= 7 Then
                            If LCase$(terms(i)) = "mod" Then
                                op_type = ModOp
                                op_pri = 7
                                op_idx = i
                            End If
                        End If
                    Case 78, 110 'not
                        If op_pri > 3 Then
                            If LCase$(terms(i)) = "not" Then
                                op_type = NotOp
                                op_pri = 3
                                op_idx = i
                            End If
                        End If
                    Case 65, 97 'and
                        If op_pri > 2 Then
                            If LCase$(terms(i)) = "and" Then
                                op_type = AndOp
                                op_pri = 2
                                op_idx = i
                            End If
                        End If
                End Select
            Case 4
                Select Case ret
                    Case 76, 108 'like
                        If op_pri >= 7 Then
                            If LCase$(terms(i)) = "like" Then
                                op_type = LikeOp
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
        EvalExpr = StringType
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
            buf = String$(Len(expr), vbNullChar)
            tsize = Len(terms(1))
            Mid(buf, 1, tsize) = terms(1)
            osize = tsize
            For i = 2 To op_idx - 1
                Mid(buf, osize + 1, 1) = " "
                tsize = Len(terms(i))
                Mid(buf, osize + 2, tsize) = terms(i)
                osize = osize + tsize + 1
            Next
            lop = Left$(buf, osize)
    End Select
    If op_idx = tnum - 1 Then
        '右辺引数は項
        is_rop_term = True
        rop = terms(tnum)
    Else
        '右辺引数の連結処理 (高速化のため、Midを使用)
        buf = String$(Len(expr), vbNullChar)
        tsize = Len(terms(op_idx + 1))
        Mid(buf, 1, tsize) = terms(op_idx + 1)
        osize = tsize
        For i = op_idx + 2 To tnum
            Mid(buf, osize + 1, 1) = " "
            tsize = Len(terms(i))
            Mid(buf, osize + 2, tsize) = terms(i)
            osize = osize + tsize + 1
        Next
        rop = Left$(buf, osize)
    End If
    
    '演算の実施
    Select Case op_type
        Case PlusOp '+
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(lnum + rnum)
            Else
                EvalExpr = NumericType
                num_result = lnum + rnum
            End If
            
        Case MinusOp '-
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(lnum - rnum)
            Else
                EvalExpr = NumericType
                num_result = lnum - rnum
            End If
            
        Case MultOp
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(lnum * rnum)
            Else
                EvalExpr = NumericType
                num_result = lnum * rnum
            End If
            
        Case DivOp '/
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If rnum <> 0 Then
                num_result = lnum / rnum
            Else
                num_result = 0
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(num_result)
            Else
                EvalExpr = NumericType
            End If
            
        Case IntDivOp '\
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If rnum <> 0 Then
                num_result = lnum \ rnum
            Else
                num_result = 0
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(num_result)
            Else
                EvalExpr = NumericType
            End If
            
        Case ModOp 'Mod
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(lnum Mod rnum)
            Else
                EvalExpr = NumericType
                num_result = lnum Mod rnum
            End If
            
        Case ExpoOp '^
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                str_result = FormatNum(lnum ^ rnum)
            Else
                EvalExpr = NumericType
                num_result = lnum ^ rnum
            End If
            
        Case CatOp '&
            If is_lop_term Then
                EvalTerm lop, StringType, lstr, lnum
            Else
                EvalExpr lop, StringType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, StringType, rstr, rnum
            Else
                EvalExpr rop, StringType, rstr, rnum
            End If
            
            If etype = NumericType Then
                EvalExpr = NumericType
                num_result = StrToDbl(lstr & rstr)
            Else
                EvalExpr = StringType
                str_result = lstr & rstr
            End If
            
        Case EqOp '=
            If IsNumber(lop) Or IsNumber(rop) Then
                If is_lop_term Then
                    EvalTerm lop, NumericType, lstr, lnum
                Else
                    EvalExpr lop, NumericType, lstr, lnum
                End If
                If is_rop_term Then
                    EvalTerm rop, NumericType, rstr, rnum
                Else
                    EvalExpr rop, NumericType, rstr, rnum
                End If
                
                If etype = StringType Then
                    EvalExpr = StringType
                    If lnum = rnum Then
                        str_result = "1"
                    Else
                        str_result = "0"
                    End If
                Else
                    EvalExpr = NumericType
                    If lnum = rnum Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                End If
            Else
                If is_lop_term Then
                    EvalTerm lop, StringType, lstr, lnum
                Else
                    EvalExpr lop, StringType, lstr, lnum
                End If
                If is_rop_term Then
                    EvalTerm rop, StringType, rstr, rnum
                Else
                    EvalExpr rop, StringType, rstr, rnum
                End If
                
                If etype = StringType Then
                    EvalExpr = StringType
                    If lstr = rstr Then
                        str_result = "1"
                    Else
                        str_result = "0"
                    End If
                Else
                    EvalExpr = NumericType
                    If lstr = rstr Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                End If
            End If
            
        Case NotEqOp '<>, !=
            If IsNumber(lop) Or IsNumber(rop) Then
                If is_lop_term Then
                    EvalTerm lop, NumericType, lstr, lnum
                Else
                    EvalExpr lop, NumericType, lstr, lnum
                End If
                If is_rop_term Then
                    EvalTerm rop, NumericType, rstr, rnum
                Else
                    EvalExpr rop, NumericType, rstr, rnum
                End If
                
                If etype = StringType Then
                    EvalExpr = StringType
                    If lnum <> rnum Then
                        str_result = "1"
                    Else
                        str_result = "0"
                    End If
                Else
                    EvalExpr = NumericType
                    If lnum <> rnum Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                End If
            Else
                If is_lop_term Then
                    EvalTerm lop, StringType, lstr, lnum
                Else
                    EvalExpr lop, StringType, lstr, lnum
                End If
                If is_rop_term Then
                    EvalTerm rop, StringType, rstr, rnum
                Else
                    EvalExpr rop, StringType, rstr, rnum
                End If
                
                If etype = StringType Then
                    EvalExpr = StringType
                    If lstr <> rstr Then
                        str_result = "1"
                    Else
                        str_result = "0"
                    End If
                Else
                    EvalExpr = NumericType
                    If lstr <> rstr Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                End If
            End If
            
        Case LtOp '<
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If lnum < rnum Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
            Else
                EvalExpr = NumericType
                If lnum < rnum Then
                    num_result = 1
                Else
                    num_result = 0
                End If
            End If
            
        Case LtEqOp '<=
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If lnum <= rnum Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
            Else
                EvalExpr = NumericType
                If lnum <= rnum Then
                    num_result = 1
                Else
                    num_result = 0
                End If
            End If
            
        Case GtOp '>
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If lnum > rnum Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
            Else
                EvalExpr = NumericType
                If lnum > rnum Then
                    num_result = 1
                Else
                    num_result = 0
                End If
            End If
            
        Case GtEqOp '>=
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If lnum >= rnum Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
            Else
                EvalExpr = NumericType
                If lnum >= rnum Then
                    num_result = 1
                Else
                    num_result = 0
                End If
            End If
            
        Case LikeOp 'Like
            If is_lop_term Then
                EvalTerm lop, StringType, lstr, lnum
            Else
                EvalExpr lop, StringType, lstr, lnum
            End If
            If is_rop_term Then
                EvalTerm rop, StringType, rstr, rnum
            Else
                EvalExpr rop, StringType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If lstr Like rstr Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
            Else
                EvalExpr = NumericType
                If lstr Like rstr Then
                    num_result = 1
                Else
                    num_result = 0
                End If
            End If
            
        Case NotOp 'Not
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If rnum <> 0 Then
                    str_result = "0"
                Else
                    str_result = "1"
                End If
            Else
                EvalExpr = NumericType
                If rnum <> 0 Then
                    num_result = 0
                Else
                    num_result = 1
                End If
            End If
            
        Case AndOp 'And
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            
            If lnum = 0 Then
                If etype = StringType Then
                    EvalExpr = StringType
                    str_result = "0"
                Else
                    EvalExpr = NumericType
                    num_result = 0
                End If
                Exit Function
            End If
            
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If rnum = 0 Then
                    str_result = "0"
                Else
                    str_result = "1"
                End If
            Else
                EvalExpr = NumericType
                If rnum = 0 Then
                    num_result = 0
                Else
                    num_result = 1
                End If
            End If
            
        Case OrOp 'Or
            If is_lop_term Then
                EvalTerm lop, NumericType, lstr, lnum
            Else
                EvalExpr lop, NumericType, lstr, lnum
            End If
            
            If lnum <> 0 Then
                If etype = StringType Then
                    EvalExpr = StringType
                    str_result = "1"
                Else
                    EvalExpr = NumericType
                    num_result = 1
                End If
                Exit Function
            End If
            
            If is_rop_term Then
                EvalTerm rop, NumericType, rstr, rnum
            Else
                EvalExpr rop, NumericType, rstr, rnum
            End If
            
            If etype = StringType Then
                EvalExpr = StringType
                If rnum = 0 Then
                    str_result = "0"
                Else
                    str_result = "1"
                End If
            Else
                EvalExpr = NumericType
                If rnum = 0 Then
                    num_result = 0
                Else
                    num_result = 1
                End If
            End If
    End Select
End Function

'項を評価
Public Function EvalTerm(expr As String, etype As ValueType, _
    str_result As String, num_result As Double) As ValueType
    
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
            EvalTerm = EvalTerm(Trim$(expr), etype, str_result, num_result)
            Exit Function
        Case 34 '"
            'ダブルクォートで囲まれた文字列
            If Right$(expr, 1) = """" Then
                EvalTerm = StringType
                str_result = Mid$(expr, 2, Len(expr) - 2)
                ReplaceSubExpression str_result
            Else
                str_result = expr
            End If
            If etype <> StringType Then
                num_result = StrToDbl(str_result)
            End If
            EvalTerm = StringType
            Exit Function
        Case 35 '#
            '色指定
            EvalTerm = StringType
            str_result = expr
            Exit Function
        Case 40 '(
            'カッコで囲まれた式
            If Right$(expr, 1) = ")" Then
                EvalTerm = EvalExpr(Mid$(expr, 2, Len(expr) - 2), _
                    etype, str_result, num_result)
            Else
                str_result = expr
                If etype <> StringType Then
                    num_result = StrToDbl(str_result)
                End If
                EvalTerm = StringType
            End If
            Exit Function
        Case 43, 45, 48 To 57 '+, -, 0～9
            '数値？
            If IsNumeric(expr) Then
                Select Case etype
                    Case StringType
                        str_result = expr
                        EvalTerm = StringType
                    Case NumericType, UndefinedType
                        num_result = CDbl(expr)
                        EvalTerm = NumericType
                End Select
                Exit Function
            End If
        Case 96 '`
            'バッククォートで囲まれた文字列
            If Right$(expr, 1) = "`" Then
                str_result = Mid$(expr, 2, Len(expr) - 2)
            Else
                str_result = expr
            End If
            If etype <> StringType Then
                num_result = StrToDbl(str_result)
            End If
            EvalTerm = StringType
            Exit Function
    End Select
    
    '関数呼び出し？
    EvalTerm = CallFunction(expr, etype, str_result, num_result)
    If EvalTerm <> UndefinedType Then
        Exit Function
    End If
    
    '変数？
    EvalTerm = GetVariable(expr, etype, str_result, num_result)
End Function


' === 関数に関する処理 ===

'式を関数呼び出しとして構文解析し、実行
Public Function CallFunction(expr As String, etype As ValueType, _
    str_result As String, num_result As Double) As ValueType
Dim fname As String
Dim start_idx As Integer
Dim i As Integer, j As Integer, num As Integer, num2 As Integer
Dim buf As String, buf2 As String
Dim ldbl As Double, rdbl As Double
Dim pname As String, pname2 As String, uname As String
Dim ret As Long, cur_depth As Integer
Dim var As VarData, it As Item
Dim depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim params(MaxArgIndex) As String, pcount As Integer, is_term(MaxArgIndex) As Boolean
Dim dir_path As String
Static dir_list() As String
Static dir_index As Integer
Static regexp_index As Integer
    
    '関数呼び出しの書式に合っているかチェック
    If Right$(expr, 1) <> ")" Then
        CallFunction = UndefinedType
        Exit Function
    End If
    i = InStr(expr, " ")
    j = InStr(expr, "(")
    If i > 0 Then
        If i < j Then
            CallFunction = UndefinedType
            Exit Function
        End If
    Else
        If j = 0 Then
            CallFunction = UndefinedType
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
    For i = start_idx To num - 1
        If in_single_quote Then
            If Asc(Mid$(expr, i, 1)) = 96 Then '`
                in_single_quote = False
            End If
        ElseIf in_double_quote Then
            If Asc(Mid$(expr, i, 1)) = 34 Then '"
                in_double_quote = False
            End If
        Else
            Select Case Asc(Mid$(expr, i, 1))
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
                        params(pcount) = Mid$(expr, start_idx, i - start_idx)
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
        params(pcount) = Mid$(expr, start_idx, num - start_idx)
    End If
    
    '先頭の文字で関数の種類を判断する
    Select Case Asc(expr)
        Case 95 '_
            '必ずユーザー定義関数
            fname = Left$(expr, j - 1)
            GoTo LookUpUserDefinedID
        Case 65 To 90, 97 To 122 'A To z
            'システム関数の可能性あり
            fname = Left$(expr, j - 1)
        Case Else
            '先頭がアルファベットでなければ必ずユーザー定義関数
            'ただし括弧を含むユニット名等である場合があるため、チェックが必要
            If UDList.IsDefined(expr) Then
                CallFunction = UndefinedType
                Exit Function
            End If
            If PDList.IsDefined(expr) Then
                CallFunction = UndefinedType
                Exit Function
            End If
            If NPDList.IsDefined(expr) Then
                CallFunction = UndefinedType
                Exit Function
            End If
            If IDList.IsDefined(expr) Then
                CallFunction = UndefinedType
                Exit Function
            End If
            fname = Left$(expr, j - 1)
            GoTo LookUpUserDefinedID
    End Select
    
    'システム関数？
    Select Case LCase$(fname)
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
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
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
                    DisplayEventErrorMessage CurrentLineNum, _
                        "指定されたサブルーチン「" & params(1) & "」が見つかりません"
                    Exit Function
                End If
            End If
            ret = ret + 1
            
            '呼び出し階層をチェック
            If CallDepth > MaxCallDepth Then
                CallDepth = MaxCallDepth
                DisplayEventErrorMessage CurrentLineNum, _
                    FormatNum(MaxCallDepth) & "階層を越えるサブルーチンの呼び出しは出来ません"
                Exit Function
            End If
            
            '引数用スタックが溢れないかチェック
            If ArgIndex + pcount > MaxArgIndex Then
                DisplayEventErrorMessage CurrentLineNum, _
                    "サブルーチンの引数の総数が" & FormatNum(MaxArgIndex) & _
                    "個を超えています"
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
                    If cur_depth = CallDepth _
                        And .Name = ReturnCmd _
                    Then
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
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "info"
            For i = 1 To pcount
                params(i) = GetValueAsString(params(i), is_term(i))
            Next
            str_result = EvalInfoFunc(params)
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "instr"
            If pcount = 2 Then
                i = InStr(GetValueAsString(params(1), is_term(1)), _
                    GetValueAsString(params(2), is_term(2)))
            Else
                'params(3)が指定されている場合は、それを検索開始位置似設定
                'VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                'eve上では引数3に設定するようにしています
                i = InStr(GetValueAsLong(params(3), is_term(3)), _
                    GetValueAsString(params(1), is_term(1)), _
                    GetValueAsString(params(2), is_term(2)))
            End If
            
            If etype = StringType Then
                str_result = FormatNum(i)
                CallFunction = StringType
            Else
                num_result = i
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "instrb"
            If pcount = 2 Then
                i = InStrB(StrConv(GetValueAsString(params(1), is_term(1)), vbFromUnicode), _
                    StrConv(GetValueAsString(params(2), is_term(2)), vbFromUnicode))
            Else
                'params(3)が指定されている場合は、それを検索開始位置似設定
                'VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                'eve上では引数3に設定するようにしています
                i = InStrB(GetValueAsLong(params(3), is_term(3)), _
                    StrConv(GetValueAsString(params(1), is_term(1)), vbFromUnicode), _
                    StrConv(GetValueAsString(params(2), is_term(2)), vbFromUnicode))
            End If
            
            If etype = StringType Then
                str_result = FormatNum(i)
                CallFunction = StringType
            Else
                num_result = i
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "lindex"
            str_result = ListIndex(GetValueAsString(params(1), is_term(1)), _
                GetValueAsLong(params(2), is_term(2)))
            
            '全体が()で囲まれている場合は()を外す
            If Left$(str_result, 1) = "(" _
                And Right$(str_result, 1) = ")" _
            Then
                str_result = Mid$(str_result, 2, Len(str_result) - 2)
            End If
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "llength"
            i = ListLength(GetValueAsString(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(i)
                CallFunction = StringType
            Else
                num_result = i
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "list"
            str_result = GetValueAsString(params(1), is_term(1))
            For i = 2 To pcount
                str_result = str_result & " " & GetValueAsString(params(i), is_term(i))
            Next
            CallFunction = StringType
            Exit Function
            
        'これ以降はアルファベット順
        Case "abs"
            num_result = Abs(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                With .Unit
                                    If .Status = "出撃" Or .Status = "格納" Then
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                str_result = .Unit.Area
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        str_result = SelectedUnitForEvent.Area
                    End If
            End Select
            CallFunction = StringType
            Exit Function
            
        Case "asc"
            num_result = Asc(GetValueAsString(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "atn"
            num_result = Atn(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "chr"
            str_result = Chr$(GetValueAsLong(params(1), is_term(1)))
            CallFunction = StringType
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
                            If Not .Unit Is Nothing Then
                                If .Unit.IsConditionSatisfied(buf) Then
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "count"
            expr = Trim$(expr)
            buf = Mid$(expr, 7, Len(expr) - 7) & "["
            num = 0
            
            'サブルーチンローカル変数を検索
            If CallDepth > 0 Then
                For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
                    If InStr(VarStack(i).Name, buf) = 1 Then
                        num = num + 1
                    End If
                Next
                If num > 0 Then
                    If etype = StringType Then
                        str_result = FormatNum(num)
                        CallFunction = StringType
                    Else
                        num_result = num
                        CallFunction = NumericType
                    End If
                    Exit Function
                End If
            End If
            
            'ローカル変数を検索
            For Each var In LocalVariableList
                If InStr(var.Name, buf) = 1 Then
                    num = num + 1
                End If
            Next
            If num > 0 Then
                If etype = StringType Then
                    str_result = FormatNum(num)
                    CallFunction = StringType
                Else
                    num_result = num
                    CallFunction = NumericType
                End If
                Exit Function
            End If
            
            'グローバル変数を検索
            For Each var In GlobalVariableList
                If InStr(var.Name, buf) = 1 Then
                    num = num + 1
                End If
            Next
            If etype = StringType Then
                str_result = FormatNum(num)
                CallFunction = StringType
            Else
                num_result = num
                CallFunction = NumericType
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
                            For Each it In IList
                                With it
                                    If .Unit Is Nothing And .Exist Then
                                        num = num + 1
                                    End If
                                End With
                            Next
                        End If
                    Else
                        With PList.Item(pname)
                           If Not .Unit Is Nothing Then
                               num = .Unit.CountItem
                           End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num = SelectedUnitForEvent.CountItem
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num)
                CallFunction = StringType
            Else
                num_result = num
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "countpartner"
            num_result = UBound(SelectedPartners)
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                With .Unit
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "cos"
            num_result = Cos(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                    ElseIf PList.Item(pname).Unit Is Nothing Then
                        num_result = 100
                    Else
                        With PList.Item(pname).Unit
                            num_result = 100 * (.MaxHP - .HP) \ .MaxHP
                        End With
                    End If
                Case 0
                    With SelectedUnitForEvent
                        num_result = 100 * (.MaxHP - .HP) \ .MaxHP
                    End With
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "dir"
            CallFunction = StringType
            Select Case pcount
                Case 2
                    fname = GetValueAsString(params(1), is_term(1))
                    
                    'フルパス指定でなければシナリオフォルダを起点に検索
                    If Mid$(fname, 2, 1) <> ":" Then
                        fname = ScenarioPath & fname
                    End If
                    
                    Select Case GetValueAsString(params(2), is_term(2))
                        Case "ファイル"
                            num = vbNormal
                        Case "フォルダ"
                            num = vbDirectory
                    End Select
                    str_result = Dir$(fname, num)
                    
                    If Len(str_result) = 0 Then
                        Exit Function
                    End If
                    
                    'ファイル属性チェック用に検索パスを作成
                    dir_path = fname
                    If num = vbDirectory Then
                        i = InStr2(fname, "\")
                        If i > 0 Then
                            dir_path = Left$(fname, i)
                        End If
                    End If
                    
                    '単一ファイルの検索？
                    If InStr(fname, "*") = 0 Then
                        'フォルダの検索の場合は見つかったファイルがフォルダ
                        'かどうかチェックする
                        If num = vbDirectory Then
                            If (GetAttr(dir_path & str_result) And num) = 0 Then
                                str_result = ""
                            End If
                        End If
                        Exit Function
                    End If
                    
                    If str_result = "." Then
                        str_result = Dir
                    End If
                    If str_result = ".." Then
                        str_result = Dir
                    End If
                    
                    '検索されたファイル一覧を作成
                    ReDim dir_list(0)
                    If num = vbDirectory Then
                        Do While Len(str_result) > 0
                            'フォルダの検索の場合は見つかったファイルがフォルダ
                            'かどうかチェックする
                            If (GetAttr(dir_path & str_result) And num) <> 0 Then
                                ReDim Preserve dir_list(UBound(dir_list) + 1)
                                dir_list(UBound(dir_list)) = str_result
                            End If
                            str_result = Dir
                        Loop
                    Else
                        Do While Len(str_result) > 0
                            ReDim Preserve dir_list(UBound(dir_list) + 1)
                            dir_list(UBound(dir_list)) = str_result
                            str_result = Dir
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
                    If Mid$(fname, 2, 1) <> ":" Then
                        fname = ScenarioPath & fname
                    End If
                    
                    str_result = Dir$(fname, vbDirectory)
                    
                    If Len(str_result) = 0 Then
                        Exit Function
                    End If
                    
                    '単一ファイルの検索？
                    If InStr(fname, "*") = 0 Then
                        Exit Function
                    End If
                    
                    If str_result = "." Then
                        str_result = Dir
                    End If
                    If str_result = ".." Then
                        str_result = Dir
                    End If
                    
                    '検索されたファイル一覧を作成
                    ReDim dir_list(0)
                    Do While Len(str_result) > 0
                        ReDim Preserve dir_list(UBound(dir_list) + 1)
                        dir_list(UBound(dir_list)) = str_result
                        str_result = Dir
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
            If etype = StringType Then
                If EOF(GetValueAsLong(params(1), is_term(1))) Then
                    str_result = "1"
                Else
                    str_result = "0"
                End If
                CallFunction = StringType
            Else
                If EOF(GetValueAsLong(params(1), is_term(1))) Then
                    num_result = 1
                End If
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                num_result = .Unit.EN
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.EN
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "eval"
            buf = Trim$(GetValueAsString(params(1), is_term(1)))
            CallFunction = EvalExpr(buf, etype, str_result, num_result)
            Exit Function
            
        Case "font"
            Select Case GetValueAsString(params(1), is_term(1))
                Case "フォント名"
                    str_result = MainForm.picMain(0).Font.Name
                    CallFunction = StringType
                Case "サイズ"
                    num_result = MainForm.picMain(0).Font.Size
                    If etype = StringType Then
                        str_result = FormatNum(num_result)
                        CallFunction = StringType
                    Else
                        CallFunction = NumericType
                    End If
                Case "太字"
                    If MainForm.picMain(0).Font.Bold Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                    If etype = StringType Then
                        str_result = FormatNum(num_result)
                        CallFunction = StringType
                    Else
                        CallFunction = NumericType
                    End If
                Case "斜体"
                    If MainForm.picMain(0).Font.Italic Then
                        num_result = 1
                    Else
                        num_result = 0
                    End If
                    If etype = StringType Then
                        str_result = FormatNum(num_result)
                        CallFunction = StringType
                    Else
                        CallFunction = NumericType
                    End If
                Case "色"
                    str_result = Hex(MainForm.picMain(0).ForeColor)
                    For i = 1 To 6 - Len(str_result)
                        str_result = "0" & str_result
                    Next
                    str_result = "#" & str_result
                    CallFunction = StringType
                Case "書き込み"
                    If PermanentStringMode Then
                        str_result = "背景"
                    ElseIf KeepStringMode Then
                        str_result = "保持"
                    Else
                        str_result = "通常"
                    End If
                    CallFunction = StringType
            End Select
            Exit Function
            
        Case "format"
            str_result = Format$(GetValueAsString(params(1), is_term(1)), _
                GetValueAsString(params(2), is_term(2)))
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "keystate"
            Dim PT As POINTAPI
            Dim in_window As Boolean
            Dim x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer
            
            If pcount <> 1 Then
                Exit Function
            End If
            
            'キー番号
            i = GetValueAsLong(params(1), is_term(1))
            
            '左利き設定に対応
            Select Case i
                Case vbKeyLButton
                    i = LButtonID
                Case vbKeyRButton
                    i = RButtonID
            End Select
            
            If i = vbKeyLButton Or i = vbKeyRButton Then
                'マウスカーソルの位置を参照
                GetCursorPos PT
                
                'メインウインドウ上でマウスボタンを押している？
                If Screen.ActiveForm Is MainForm Then
                    With MainForm
                        x1 = .Left \ Screen.TwipsPerPixelX + .picMain(0).Left + 3
                        y1 = .Top \ Screen.TwipsPerPixelY + .picMain(0).Top + 28
                        x2 = x1 + .picMain(0).Width
                        y2 = y1 + .picMain(0).Height
                    End With
                    
                    With PT
                        If x1 <= .x And .x <= x2 _
                            And y1 <= .y And .y <= y2 _
                        Then
                            in_window = True
                        End If
                    End With
                End If
            Else
                'メインウィンドウがアクティブになっている？
                If Screen.ActiveForm Is MainForm Then
                    in_window = True
                End If
            End If
            
            'ウィンドウが選択されていない場合は常に0を返す
            If Not in_window Then
                num_result = 0
                If etype = StringType Then
                    str_result = "0"
                    CallFunction = StringType
                Else
                    CallFunction = NumericType
                End If
                Exit Function
            End If
            
            'キーの状態を参照
            If GetAsyncKeyState(i) And &H8000 Then
                num_result = 1
            End If
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "gettime"
            num_result = timeGetTime()
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                num_result = .Unit.HP
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.HP
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "iif"
            Dim list() As String, flag As Boolean
            
            num = ListSplit(params(1), list)

            Select Case num
                Case 1
                    If PList.IsDefined(list(1)) Then
                        With PList.Item(list(1))
                            If .Unit Is Nothing Then
                                flag = False
                            Else
                                With .Unit
                                    If .Status = "出撃" _
                                        Or .Status = "格納" _
                                    Then
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
                            If .Unit Is Nothing Then
                                flag = True
                            Else
                                With .Unit
                                    If .Status = "出撃" _
                                        Or .Status = "格納" _
                                    Then
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
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
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
            
            If etype = StringType Then
                str_result = FormatNum(i)
                CallFunction = StringType
            Else
                num_result = i
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "instrrevb"
            buf = GetValueAsString(params(1), is_term(1))
            buf2 = GetValueAsString(params(2), is_term(2))
            
            If LenB(buf2) > 0 And LenB(buf) >= LenB(buf2) Then
                If pcount = 2 Then
                    num = LenB(buf)
                Else
                    num = GetValueAsLong(params(3), is_term(3))
                End If
                
                i = num - LenB(buf2) + 1
                Do
                    j = InStrB(i, buf, buf2)
                    If i = j Then
                        Exit Do
                    End If
                    i = i - 1
                Loop Until i = 0
            Else
                i = 0
            End If
            
            If etype = StringType Then
                str_result = FormatNum(i)
                CallFunction = StringType
            Else
                num_result = i
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "int"
            num_result = Int(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                If .Unit.IsFeatureAvailable(buf) Then
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                                If UList.Item(pname).Status <> "破棄" Then
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
                        If UList.Item(pname).Status <> "破棄" Then
                            num_result = 1
                        End If
                    ElseIf IList.IsDefined(pname) Then
                        num_result = 1
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                If .Unit.IsEquiped(GetValueAsString(params(2), is_term(2))) Then
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "lsearch"
            buf = GetValueAsString(params(1), is_term(1))
            buf2 = GetValueAsString(params(2), is_term(2))
            num = IIf(pcount < 3, 1, GetValueAsLong(params(3), is_term(3)))
            num2 = ListLength(buf)
            
            For i = num To num2
                If ListIndex(buf, i) = buf2 Then
                    If etype = StringType Then
                        str_result = Format$(i)
                        CallFunction = StringType
                    Else
                        num_result = i
                        CallFunction = NumericType
                    End If
                    Exit Function
                End If
            Next
            
            If etype = StringType Then
                str_result = "0"
                CallFunction = StringType
            Else
                num_result = 0
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "isnumeric"
            If IsNumber(GetValueAsString(params(1), is_term(1))) Then
                If etype = StringType Then
                    str_result = "1"
                    CallFunction = StringType
                Else
                    num_result = 1
                    CallFunction = NumericType
                End If
            Else
                If etype = StringType Then
                    str_result = "0"
                    CallFunction = StringType
                Else
                    num_result = 0
                    CallFunction = NumericType
                End If
            End If
            Exit Function
            
        Case "isvardefined"
            If IsVariableDefined(Trim$(Mid$(expr, 14, Len(expr) - 14))) Then
                If etype = StringType Then
                    str_result = "1"
                    CallFunction = StringType
                Else
                    num_result = 1
                    CallFunction = NumericType
                End If
            Else
                If etype = StringType Then
                    str_result = "0"
                    CallFunction = StringType
                Else
                    num_result = 0
                    CallFunction = NumericType
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
                            For Each it In IList
                                With it
                                    If .Unit Is Nothing And .Exist Then
                                        i = i + 1
                                        If i = j Then
                                            str_result = .Name
                                            Exit For
                                        End If
                                    End If
                                End With
                            Next
                        End If
                    ElseIf Not PList.Item(pname).Unit Is Nothing Then
                        i = GetValueAsLong(params(2), is_term(2))
                        With PList.Item(pname).Unit
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
            CallFunction = StringType
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
                            For Each it In IList
                                With it
                                    If .Unit Is Nothing And .Exist Then
                                        i = i + 1
                                        If i = j Then
                                            str_result = .ID
                                            Exit For
                                        End If
                                    End If
                                End With
                            Next
                        End If
                    ElseIf Not PList.Item(pname).Unit Is Nothing Then
                        i = GetValueAsLong(params(2), is_term(2))
                        With PList.Item(pname).Unit
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
            CallFunction = StringType
            Exit Function
            
        Case "left"
            str_result = Left$(GetValueAsString(params(1), is_term(1)), _
                GetValueAsLong(params(2), is_term(2)))
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "leftb"
            buf = GetValueAsString(params(1), is_term(1))
            str_result = LeftB$(StrConv(buf, vbFromUnicode), _
                GetValueAsLong(params(2), is_term(2)))
            str_result = StrConv(str_result, vbUnicode)
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "len"
            num_result = Len(GetValueAsString(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "lenb"
            buf = GetValueAsString(params(1), is_term(1))
            num_result = LenB(StrConv(buf, vbFromUnicode))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "lcase"
            str_result = LCase$(GetValueAsString(params(1), is_term(1)))
            CallFunction = StringType
            Exit Function
            
        Case "lset"
            buf = GetValueAsString(params(1), is_term(1))
            i = GetValueAsLong(params(2), is_term(2))
            If LenB(StrConv(buf, vbFromUnicode)) < i Then
                str_result = buf & Space$(i - LenB(StrConv(buf, vbFromUnicode)))
            Else
                str_result = buf
            End If
            CallFunction = StringType
            Exit Function
            
        Case "max"
            num_result = GetValueAsDouble(params(1), is_term(1))
            For i = 2 To pcount
                rdbl = GetValueAsDouble(params(i), is_term(i))
                If num_result < rdbl Then
                    num_result = rdbl
                End If
            Next
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "mid"
            buf = GetValueAsString(params(1), is_term(1))
            Select Case pcount
                Case 3
                    i = GetValueAsLong(params(2), is_term(2))
                    j = GetValueAsLong(params(3), is_term(3))
                    str_result = Mid$(buf, i, j)
                Case 2
                    i = GetValueAsLong(params(2), is_term(2))
                    str_result = Mid$(buf, i)
            End Select
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "midb"
            buf = GetValueAsString(params(1), is_term(1))
            Select Case pcount
                Case 3
                    i = GetValueAsLong(params(2), is_term(2))
                    j = GetValueAsLong(params(3), is_term(3))
                    str_result = MidB$(StrConv(buf, vbFromUnicode), i, j)
                Case 2
                    i = GetValueAsLong(params(2), is_term(2))
                    str_result = MidB$(StrConv(buf, vbFromUnicode), i)
            End Select
            str_result = StrConv(str_result, vbUnicode)
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            CallFunction = StringType
            Exit Function
            
        Case "partner"
            i = GetValueAsLong(params(1), is_term(1))
            If i = 0 Then
                str_result = SelectedUnitForEvent.ID
            ElseIf 1 <= i And i <= UBound(SelectedPartners) Then
                str_result = SelectedPartners(i).ID
            End If
            CallFunction = StringType
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
            CallFunction = StringType
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
                            ElseIf .CountPilot < i _
                                And i <= .CountPilot + .CountSupport _
                            Then
                                str_result = .Support(i - .CountPilot).Name
                            End If
                        End With
                    End If
                Case 1
                    uname = GetValueAsString(params(1), is_term(1))
                    If IsNumber(uname) Then
                        If Not SelectedUnitForEvent Is Nothing Then
                            i = CInt(uname)
                            With SelectedUnitForEvent
                                If 0 < i And i <= .CountPilot Then
                                    str_result = .Pilot(i).Name
                                ElseIf .CountPilot < i _
                                    And i <= .CountPilot + .CountSupport _
                                Then
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
            CallFunction = StringType
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
                            ElseIf .CountPilot < i _
                                And i <= .CountPilot + .CountSupport _
                            Then
                                str_result = .Support(i - .CountPilot).ID
                            End If
                        End With
                    End If
                Case 1
                    uname = GetValueAsString(params(1), is_term(1))
                    If IsNumber(uname) Then
                        If Not SelectedUnitForEvent Is Nothing Then
                            i = CInt(uname)
                            With SelectedUnitForEvent
                                If 0 < i And i <= .CountPilot Then
                                    str_result = .Pilot(i).ID
                                ElseIf .CountPilot < i _
                                    And i <= .CountPilot + .CountSupport _
                                Then
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
            CallFunction = StringType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "random"
            num_result = Dice(GetValueAsLong(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                num_result = .Unit.Rank
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.Rank
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "regexp"
            On Error GoTo RegExp_Error
            
            If RegEx Is Nothing Then
                Set RegEx = CreateObject("VBScript.RegExp")
            End If
        
            'RegExp(文字列, パターン[,大小区別あり|大小区別なし])
            buf = ""
            If pcount > 0 Then
                '文字列全体を検索
                RegEx.Global = True
                '大文字小文字の区別（True=区別しない）
                RegEx.IgnoreCase = False
                If pcount >= 3 Then
                    If GetValueAsString(params(3), is_term(3)) = "大小区別なし" Then
                        RegEx.IgnoreCase = True
                    End If
                End If
                '検索パターン
                RegEx.Pattern = GetValueAsString(params(2), is_term(2))
                Set Matches = RegEx.Execute(GetValueAsString(params(1), is_term(1)))
                If Matches.Count = 0 Then
                    regexp_index = -1
                Else
                    regexp_index = 0
                    buf = Matches(regexp_index)
                End If
            Else
                If regexp_index >= 0 Then
                    regexp_index = regexp_index + 1
                    If regexp_index <= Matches.Count - 1 Then
                        buf = Matches(regexp_index)
                    End If
                End If
            End If
            str_result = buf
            CallFunction = StringType
            Exit Function
RegExp_Error:
            DisplayEventErrorMessage CurrentLineNum, _
                "VBScriptがインストールされていません"
            Exit Function
            
        Case "regexpreplace"
            'RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])
            
            On Error GoTo RegExpReplace_Error
            
            If RegEx Is Nothing Then
                Set RegEx = CreateObject("VBScript.RegExp")
            End If
            
            '文字列全体を検索
            RegEx.Global = True
            '大文字小文字の区別（True=区別しない）
            RegEx.IgnoreCase = False
            If pcount >= 4 Then
                If GetValueAsString(params(4), is_term(4)) = "大小区別なし" Then
                    RegEx.IgnoreCase = True
                End If
            End If
            '検索パターン
            RegEx.Pattern = GetValueAsString(params(2), is_term(2))
            
            '置換実行
            buf = RegEx.Replace(GetValueAsString(params(1), is_term(1)), _
                        GetValueAsString(params(3), is_term(3)))
            
            str_result = buf
            CallFunction = StringType
            Exit Function
RegExpReplace_Error:
            DisplayEventErrorMessage CurrentLineNum, _
                "VBScriptがインストールされていません"
            Exit Function
            
        Case "relation"
            pname = GetValueAsString(params(1), is_term(1))
            If Not PList.IsDefined(pname) Then
                num_result = 0
                If etype = StringType Then
                    str_result = "0"
                    CallFunction = StringType
                Else
                    CallFunction = NumericType
                End If
                Exit Function
            End If
            pname = PList.Item(pname).Name
            
            pname2 = GetValueAsString(params(2), is_term(2))
            If Not PList.IsDefined(pname2) Then
                num_result = 0
                If etype = StringType Then
                    str_result = "0"
                    CallFunction = StringType
                Else
                    CallFunction = NumericType
                End If
                Exit Function
            End If
            pname2 = PList.Item(pname2).Name
            
            num_result = GetValueAsLong("関係:" & pname & ":" & pname2)
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "replace"
            Select Case pcount
                Case 4
                    buf = GetValueAsString(params(1), is_term(1))
                    num = GetValueAsLong(params(4), is_term(4))
                    buf2 = Right$(buf, Len(buf) - num + 1)
                    ReplaceString buf2, _
                        GetValueAsString(params(2), is_term(2)), _
                        GetValueAsString(params(3), is_term(3))
                    str_result = Left$(buf, num - 1) & buf2
                Case 5
                    buf = GetValueAsString(params(1), is_term(1))
                    num = GetValueAsLong(params(4), is_term(4))
                    num2 = GetValueAsLong(params(5), is_term(5))
                    buf2 = Mid$(buf, num, num2)
                    ReplaceString buf2, _
                        GetValueAsString(params(2), is_term(2)), _
                        GetValueAsString(params(3), is_term(3))
                    str_result = Left$(buf, num - 1) & buf2 & Right$(buf, Len(buf) - (num + num2 - 1) - 1)
                Case Else
                    str_result = GetValueAsString(params(1), is_term(1))
                    ReplaceString str_result, _
                        GetValueAsString(params(2), is_term(2)), _
                        GetValueAsString(params(3), is_term(3))
            End Select
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "rgb"
            buf = Hex(rgb(GetValueAsLong(params(1), is_term(1)), _
                    GetValueAsLong(params(2), is_term(2)), _
                    GetValueAsLong(params(3), is_term(3))))
            For i = 1 To 6 - Len(buf)
                buf = "0" & buf
            Next
            str_result = "#000000"
            Mid(str_result, 2, 2) = Mid$(buf, 5, 2)
            Mid(str_result, 4, 2) = Mid$(buf, 3, 2)
            Mid(str_result, 6, 2) = Mid$(buf, 1, 2)
            CallFunction = StringType
            Exit Function
            
        Case "right"
            str_result = Right$(GetValueAsString(params(1), is_term(1)), _
                GetValueAsLong(params(2), is_term(2)))
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "rightb"
            buf = GetValueAsString(params(1), is_term(1))
            str_result = RightB$(StrConv(buf, vbFromUnicode), _
                GetValueAsLong(params(2), is_term(2)))
            str_result = StrConv(str_result, vbUnicode)
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
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
            Select Case LCase$(fname)
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "rset"
            buf = GetValueAsString(params(1), is_term(1))
            i = GetValueAsLong(params(2), is_term(2))
            If LenB(StrConv(buf, vbFromUnicode)) < i Then
                str_result = Space$(i - LenB(StrConv(buf, vbFromUnicode))) & buf
            Else
                str_result = buf
            End If
            CallFunction = StringType
            Exit Function
            
        Case "sin"
            num_result = Sin(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                If .Unit.IsSpecialPowerInEffect(buf) Then
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "sqr"
            num_result = Sqr(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "status"
            Select Case pcount
                Case 1
                    pname = GetValueAsString(params(1), is_term(1))
                    If UList.IsDefined2(pname) Then
                        str_result = UList.Item2(pname).Status
                    ElseIf PList.IsDefined(pname) Then
                        With PList.Item(pname)
                            If Not .Unit Is Nothing Then
                                str_result = .Unit.Status
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        str_result = SelectedUnitForEvent.Status
                    End If
            End Select
            CallFunction = StringType
            Exit Function
            
        Case "strcomp"
            num_result = StrComp(GetValueAsString(params(1), is_term(1)), _
                GetValueAsString(params(2), is_term(2)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "string"
            buf = GetValueAsString(params(2), is_term(2))
            If Len(buf) <= 1 Then
                str_result = String$(GetValueAsLong(params(1), is_term(1)), buf)
            Else
                'String関数では文字列の先頭しか繰り返しされないので、
                '長さが2以上の文字列の場合は別処理
                str_result = ""
                For i = 1 To GetValueAsLong(params(1), is_term(1))
                    str_result = str_result & buf
                Next
            End If
            
            If etype = NumericType Then
                num_result = StrToDbl(str_result)
                CallFunction = NumericType
            Else
                CallFunction = StringType
            End If
            Exit Function
            
        Case "tan"
            num_result = Tan(GetValueAsDouble(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            CallFunction = StringType
            Exit Function
            
        Case "textheight"
            num_result = MainForm.picMain(0).TextHeight(GetValueAsString(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "textwidth"
            num_result = MainForm.picMain(0).TextWidth(GetValueAsString(params(1), is_term(1)))
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "trim"
            str_result = Trim$(GetValueAsString(params(1), is_term(1)))
            CallFunction = StringType
            Exit Function
            
        Case "unit"
            Select Case pcount
                Case 1
                    pname = GetValueAsString(params(1), is_term(1))
                    If UList.IsDefined2(pname) Then
                        str_result = UList.Item2(pname).Name
                    ElseIf PList.IsDefined(pname) Then
                        With PList.Item(pname)
                            If Not .Unit Is Nothing Then
                                str_result = .Unit.Name
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        str_result = SelectedUnitForEvent.Name
                    End If
            End Select
            CallFunction = StringType
            Exit Function
            
        Case "unitid"
            Select Case pcount
                Case 1
                    pname = GetValueAsString(params(1), is_term(1))
                    If UList.IsDefined2(pname) Then
                        str_result = UList.Item2(pname).ID
                    ElseIf PList.IsDefined(pname) Then
                        With PList.Item(pname)
                            If Not .Unit Is Nothing Then
                                str_result = .Unit.ID
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        str_result = SelectedUnitForEvent.ID
                    End If
            End Select
            CallFunction = StringType
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
                                    If Not .Unit Is Nothing Then
                                        num_result = .Unit.x
                                    End If
                                End With
                            End If
                    End Select
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.x
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                                    If Not .Unit Is Nothing Then
                                        num_result = .Unit.y
                                    End If
                                End With
                            End If
                    End Select
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.y
                    End If
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
'ADD START 240a
        Case "windowwidth"
            If etype = NumericType Then
                num_result = MainPWidth
                CallFunction = NumericType
            ElseIf etype = StringType Then
                str_result = CStr(MainPWidth)
                CallFunction = StringType
            End If
            Exit Function
        
        Case "windowheight"
            If etype = NumericType Then
                num_result = MainPHeight
                CallFunction = NumericType
            ElseIf etype = StringType Then
                str_result = CStr(MainPHeight)
                CallFunction = StringType
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
                            If Not .Unit Is Nothing Then
                                num_result = .Unit.x
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.x
                    End If
            End Select
            
            num_result = MapToPixelX(num_result)
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
                            If Not .Unit Is Nothing Then
                                num_result = .Unit.y
                            End If
                        End With
                    End If
                Case 0
                    If Not SelectedUnitForEvent Is Nothing Then
                        num_result = SelectedUnitForEvent.y
                    End If
            End Select
            
            num_result = MapToPixelY(num_result)
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "wide"
            str_result = StrConv(GetValueAsString(params(1), is_term(1)), vbWide)
            CallFunction = StringType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "weekday"
            Select Case pcount
                Case 1
                    buf = GetValueAsString(params(1), is_term(1))
                    If IsDate(buf) Then
                        Select Case WeekDay(CDate(buf))
                            Case vbSunday
                                str_result = "日曜"
                            Case vbMonday
                                str_result = "月曜"
                            Case vbTuesday
                                str_result = "火曜"
                            Case vbWednesday
                                str_result = "水曜"
                            Case vbThursday
                                str_result = "木曜"
                            Case vbFriday
                                str_result = "金曜"
                            Case vbSaturday
                                str_result = "土曜"
                        End Select
                    End If
                Case 0
                    Select Case WeekDay(Now)
                        Case vbSunday
                            str_result = "日曜"
                        Case vbMonday
                            str_result = "月曜"
                        Case vbTuesday
                            str_result = "火曜"
                        Case vbWednesday
                            str_result = "水曜"
                        Case vbThursday
                            str_result = "木曜"
                        Case vbFriday
                            str_result = "金曜"
                        Case vbSaturday
                            str_result = "土曜"
                    End Select
            End Select
            CallFunction = StringType
            Exit Function
            
        Case "day"
            Select Case pcount
                Case 1
                    buf = GetValueAsString(params(1), is_term(1))
                    If IsDate(buf) Then
                        num_result = Day(CDate(buf))
                    Else
                        num_result = 0
                    End If
                Case 0
                    num_result = Day(Now)
            End Select
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
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
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
            
        Case "difftime"
            If pcount = 2 Then
                Dim d1 As Date, d2 As Date
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
                
                num_result = Second(d2 - d1)
            End If
            
            If etype = StringType Then
                str_result = FormatNum(num_result)
                CallFunction = StringType
            Else
                CallFunction = NumericType
            End If
            Exit Function
        
        'ダイアログ表示
        Case "loadfiledialog"
            Select Case pcount
                Case 2
                    str_result = _
                        LoadFileDialog("ファイルを開く", _
                            ScenarioPath, "", 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
                Case 3
                    str_result = _
                        LoadFileDialog("ファイルを開く", _
                            ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
                Case 4
                    str_result = _
                        LoadFileDialog("ファイルを開く", _
                            ScenarioPath & GetValueAsString(params(4), is_term(4)), _
                            GetValueAsString(params(3), is_term(3)), 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
            End Select
            
            CallFunction = StringType
            
            '本当はこれだけでいいはずだけど……
            If InStr(str_result, ScenarioPath) > 0 Then
                str_result = Mid$(str_result, Len(ScenarioPath) + 1)
                Exit Function
            End If
            
            'フルパス指定ならここで終了
            If Right$(Left$(str_result, 3), 2) = ":\" Then
                str_result = ""
                Exit Function
            End If
            
            Do Until Dir$(ScenarioPath & str_result, vbNormal) <> ""
                If InStr(str_result, "\") = 0 Then
                    'シナリオフォルダ外のファイルだった
                    str_result = ""
                    Exit Function
                End If
                str_result = Mid$(str_result, InStr(str_result, "\") + 1)
            Loop
            Exit Function
            
        Case "savefiledialog"
            Select Case pcount
                Case 2
                    str_result = _
                        SaveFileDialog("ファイルを保存", _
                            ScenarioPath, "", 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
                Case 3
                    str_result = _
                        SaveFileDialog("ファイルを保存", _
                            ScenarioPath, GetValueAsString(params(3), is_term(3)), 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
                Case 4
                    str_result = _
                        SaveFileDialog("ファイルを保存", _
                            ScenarioPath & GetValueAsString(params(4), is_term(4)), _
                            GetValueAsString(params(3), is_term(3)), 2, _
                            GetValueAsString(params(1), is_term(1)), _
                            GetValueAsString(params(2), is_term(2)))
            End Select
            
            CallFunction = StringType
            
            '本当はこれだけでいいはずだけど……
            If InStr(str_result, ScenarioPath) > 0 Then
                str_result = Mid$(str_result, Len(ScenarioPath) + 1)
                Exit Function
            End If
            
            If InStr(str_result, "\") = 0 Then
                Exit Function
            End If
            
            For i = 1 To Len(str_result)
                If Mid$(str_result, Len(str_result) - i + 1, 1) = "\" Then
                    Exit For
                End If
            Next
            buf = Left$(str_result, Len(str_result) - i)
            str_result = Mid$(str_result, Len(str_result) - i + 2)
            
            Do While InStr(buf, "\") > 0
                buf = Mid$(buf, InStr(buf, "\") + 1)
                If Dir$(ScenarioPath & buf, vbDirectory) <> "" Then
                    str_result = buf & "\" & str_result
                    Exit Function
                End If
            Loop
            
            If Dir$(ScenarioPath & buf, vbDirectory) <> "" Then
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
            DisplayEventErrorMessage CurrentLineNum, _
                FormatNum(MaxCallDepth) & _
                "階層を越えるサブルーチンの呼び出しは出来ません"
            Exit Function
        End If
        
        '引数用スタックが溢れないかチェック
        If ArgIndex + pcount > MaxArgIndex Then
            DisplayEventErrorMessage CurrentLineNum, _
                "サブルーチンの引数の総数が" & FormatNum(MaxArgIndex) & _
                "個を超えています"
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
                If cur_depth = CallDepth _
                    And .Name = ReturnCmd _
                Then
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
        
        If etype = NumericType Then
            num_result = StrToDbl(str_result)
            CallFunction = NumericType
        Else
            CallFunction = StringType
        End If
        Exit Function
    End If
    
    '実はシステム定義のグローバル変数？
    If IsGlobalVariableDefined(expr) Then
        With GlobalVariableList.Item(expr)
            Select Case etype
                Case NumericType
                    If .VariableType = NumericType Then
                        num_result = .NumericValue
                    Else
                        num_result = StrToDbl(.StringValue)
                    End If
                    CallFunction = NumericType
                Case StringType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                    Else
                        str_result = FormatNum(.NumericValue)
                    End If
                    CallFunction = StringType
                Case UndefinedType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                        CallFunction = StringType
                    Else
                        num_result = .NumericValue
                        CallFunction = NumericType
                    End If
            End Select
        End With
        Exit Function
    End If
    
    '結局ただの文字列……
    str_result = expr
    CallFunction = StringType
End Function

'Info関数の評価
Private Function EvalInfoFunc(params() As String) As String
Dim u As Unit, ud As UnitData
Dim p As Pilot, pd As PilotData
Dim nd As NonPilotData
Dim it As Item, itd As ItemData
Dim spd As SpecialPowerData
Dim idx As Integer, i As Integer, j As Integer
Dim buf As String
Dim aname As String
Dim max_value As Long

    EvalInfoFunc = ""
    
    Set u = Nothing
    Set ud = Nothing
    Set p = Nothing
    Set pd = Nothing
    Set nd = Nothing
    Set it = Nothing
    Set itd = Nothing
    Set spd = Nothing
    
    '各オブジェクトの設定
    Select Case params(1)
        Case "ユニット"
            Set u = UList.Item(params(2))
            idx = 3
        Case "ユニットデータ"
            Set ud = UDList.Item(params(2))
            idx = 3
        Case "パイロット"
            Set p = PList.Item(params(2))
            idx = 3
        Case "パイロットデータ"
            Set pd = PDList.Item(params(2))
            idx = 3
        Case "非戦闘員"
            Set nd = NPDList.Item(params(2))
            idx = 3
        Case "アイテム"
            If IList.IsDefined(params(2)) Then
                Set it = IList.Item(params(2))
            Else
                Set itd = IDList.Item(params(2))
            End If
            idx = 3
        Case "アイテムデータ"
            Set itd = IDList.Item(params(2))
            idx = 3
        Case "スペシャルパワー"
            Set spd = SPDList.Item(params(2))
            idx = 3
        Case "マップ", "オプション"
            idx = 1
        Case ""
            Exit Function
        Case Else
            Set u = UList.Item(params(1))
            Set ud = UDList.Item(params(1))
            Set p = PList.Item(params(1))
            Set pd = PDList.Item(params(1))
            Set nd = NPDList.Item(params(1))
            Set it = IList.Item(params(1))
            Set itd = IDList.Item(params(1))
            Set spd = SPDList.Item(params(1))
            idx = 2
    End Select
    
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
                EvalInfoFunc = u.Class
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = ud.Class
            ElseIf Not p Is Nothing Then
                EvalInfoFunc = p.Class
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = pd.Class
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
                EvalInfoFunc = u.ExpValue
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = ud.ExpValue
            ElseIf Not p Is Nothing Then
                EvalInfoFunc = p.ExpValue
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = pd.ExpValue
            End If
        Case "格闘"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Infight)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Infight)
            End If
        Case "射撃"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Shooting)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Shooting)
            End If
            Exit Function
        Case "命中"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Hit)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Hit)
            End If
        Case "回避"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Dodge)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Dodge)
            End If
        Case "技量"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Technique)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Technique)
            End If
        Case "反応"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Intuition)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.Intuition)
            End If
        Case "防御"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Defense)
            End If
        Case "格闘基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.InfightBase)
            End If
        Case "射撃基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.ShootingBase)
            End If
        Case "命中基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.HitBase)
            End If
        Case "回避基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.DodgeBase)
            End If
        Case "技量基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.TechniqueBase)
            End If
        Case "反応基本値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.IntuitionBase)
            End If
        Case "格闘修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.InfightMod)
            End If
        Case "射撃修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.ShootingMod)
            End If
        Case "命中修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.HitMod)
            End If
        Case "回避修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.DodgeMod)
            End If
        Case "技量修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.TechniqueMod)
            End If
        Case "反応修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.IntuitionMod)
            End If
        Case "格闘支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.InfightMod2)
            End If
        Case "射撃支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.ShootingMod2)
            End If
        Case "命中支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.HitMod2)
            End If
        Case "回避支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.DodgeMod2)
            End If
        Case "技量支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.TechniqueMod2)
            End If
        Case "反応支援修正値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.IntuitionMod2)
            End If
        Case "性格"
            If Not p Is Nothing Then
                EvalInfoFunc = p.Personality
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = pd.Personality
            End If
       Case "最大ＳＰ"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.MaxSP)
                If p.MaxSP = 0 And Not p.Unit Is Nothing Then
                    If p Is p.Unit.MainPilot Then
                        EvalInfoFunc = Format$(p.Unit.Pilot(1).MaxSP)
                    End If
                End If
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SP)
            End If
        Case "ＳＰ"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.SP)
                If p.MaxSP = 0 And Not p.Unit Is Nothing Then
                    If p Is p.Unit.MainPilot Then
                        EvalInfoFunc = Format$(p.Unit.Pilot(1).SP)
                    End If
                End If
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SP)
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
                EvalInfoFunc = Format$(p.Level)
            End If
        Case "累積経験値"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Exp)
            End If
        Case "気力"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Morale)
            End If
        Case "最大霊力", "最大プラーナ"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.MaxPlana)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SkillLevel(0, "霊力"))
            End If
        Case "霊力", "プラーナ"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Plana)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SkillLevel(0, "霊力"))
            End If
        Case "同調率", "シンクロ率"
            If Not p Is Nothing Then
                EvalInfoFunc = Format$(p.SynchroRate)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SkillLevel(0, "同調率"))
            End If
        Case "スペシャルパワー", "精神コマンド", "精神"
            If Not p Is Nothing Then
                If p.MaxSP = 0 And Not p.Unit Is Nothing Then
                    If p Is p.Unit.MainPilot Then
                        Set p = p.Unit.Pilot(1)
                    End If
                End If
                With p
                    For i = 1 To .CountSpecialPower
                        EvalInfoFunc = EvalInfoFunc & " " & .SpecialPower(i)
                    Next
                End With
                EvalInfoFunc = Trim$(EvalInfoFunc)
            ElseIf Not pd Is Nothing Then
                With pd
                    For i = 1 To .CountSpecialPower(100)
                        EvalInfoFunc = EvalInfoFunc & " " & .SpecialPower(100, i)
                    Next
                End With
                EvalInfoFunc = Trim$(EvalInfoFunc)
            End If
        Case "スペシャルパワー所有", "精神コマンド所有"
            If Not p Is Nothing Then
                If p.MaxSP = 0 And Not p.Unit Is Nothing Then
                    If p Is p.Unit.MainPilot Then
                        Set p = p.Unit.Pilot(1)
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
                If p.MaxSP = 0 And Not p.Unit Is Nothing Then
                    If p Is p.Unit.MainPilot Then
                        Set p = p.Unit.Pilot(1)
                    End If
                End If
                EvalInfoFunc = Format$(p.SpecialPowerCost(params(idx + 1)))
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.SpecialPowerCost(params(idx + 1)))
            End If
        Case "特殊能力数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.CountFeature)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.CountFeature)
            ElseIf Not p Is Nothing Then
                EvalInfoFunc = p.CountSkill
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = LLength(pd.Skill(100))
            ElseIf Not it Is Nothing Then
                EvalInfoFunc = Format$(it.CountFeature)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.CountFeature)
            End If
        Case "特殊能力"
            If Not u Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = u.Feature(CInt(params(idx + 1)))
                End If
            ElseIf Not ud Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = ud.Feature(CInt(params(idx + 1)))
                End If
            ElseIf Not p Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = p.Skill(CInt(params(idx + 1)))
                End If
            ElseIf Not pd Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = LIndex(pd.Skill(100), CInt(params(idx + 1)))
                End If
            ElseIf Not it Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = it.Feature(CInt(params(idx + 1)))
                End If
            ElseIf Not itd Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    EvalInfoFunc = itd.Feature(CInt(params(idx + 1)))
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
                    EvalInfoFunc = u.FeatureName(CInt(params(idx + 1)))
                Else
                    EvalInfoFunc = u.FeatureName(aname)
                End If
            ElseIf Not ud Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = ud.FeatureName(CInt(aname))
                Else
                    EvalInfoFunc = ud.FeatureName(aname)
                End If
            ElseIf Not p Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = p.SkillName(CInt(aname))
                Else
                    EvalInfoFunc = p.SkillName(aname)
                End If
            ElseIf Not pd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = pd.SkillName(100, LIndex(pd.Skill(100), _
                        CInt(aname)))
                Else
                    EvalInfoFunc = pd.SkillName(100, aname)
                End If
            ElseIf Not it Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = it.FeatureName(CInt(aname))
                Else
                    EvalInfoFunc = it.FeatureName(aname)
                End If
            ElseIf Not itd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = itd.FeatureName(CInt(aname))
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
                    EvalInfoFunc = Format$(u.FeatureLevel(CInt(aname)))
                Else
                    EvalInfoFunc = Format$(u.FeatureLevel(aname))
                End If
            ElseIf Not ud Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = Format$(ud.FeatureLevel(CInt(aname)))
                Else
                    EvalInfoFunc = Format$(ud.FeatureLevel(aname))
                End If
            ElseIf Not p Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = _
                        Format$(p.SkillLevel(CInt(aname)))
                Else
                    EvalInfoFunc = Format$(p.SkillLevel(aname))
                End If
            ElseIf Not pd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = _
                        Format$(pd.SkillLevel(100, LIndex(pd.Skill(100), CInt(aname))))
                Else
                    EvalInfoFunc = Format$(pd.SkillLevel(100, aname))
                End If
            ElseIf Not it Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = Format$(it.FeatureLevel(CInt(aname)))
                Else
                    EvalInfoFunc = Format$(it.FeatureLevel(aname))
                End If
            ElseIf Not itd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = Format$(itd.FeatureLevel(CInt(aname)))
                Else
                    EvalInfoFunc = Format$(itd.FeatureLevel(aname))
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
                    EvalInfoFunc = u.FeatureData(CInt(aname))
                Else
                    EvalInfoFunc = u.FeatureData(aname)
                End If
            ElseIf Not ud Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = ud.FeatureData(CInt(aname))
                Else
                    EvalInfoFunc = ud.FeatureData(aname)
                End If
            ElseIf Not p Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = p.SkillData(CInt(aname))
                Else
                    EvalInfoFunc = p.SkillData(aname)
                End If
            ElseIf Not pd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = pd.SkillData(100, LIndex(pd.Skill(100), CInt(aname)))
                Else
                    EvalInfoFunc = pd.SkillData(100, aname)
                End If
            ElseIf Not it Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = it.FeatureData(CInt(aname))
                Else
                    EvalInfoFunc = it.FeatureData(aname)
                End If
            ElseIf Not itd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = itd.FeatureData(CInt(aname))
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
                    EvalInfoFunc = u.FeatureNecessarySkill(CInt(aname))
                Else
                    EvalInfoFunc = u.FeatureNecessarySkill(aname)
                End If
            ElseIf Not ud Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = ud.FeatureNecessarySkill(CInt(aname))
                Else
                    EvalInfoFunc = ud.FeatureNecessarySkill(aname)
                End If
            ElseIf Not it Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = it.FeatureNecessarySkill(CInt(aname))
                Else
                    EvalInfoFunc = it.FeatureNecessarySkill(aname)
                End If
            ElseIf Not itd Is Nothing Then
                If IsNumber(aname) Then
                    EvalInfoFunc = itd.FeatureNecessarySkill(CInt(aname))
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
                    EvalInfoFunc = FeatureHelpMessage(u, CInt(aname), False)
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
                        EvalInfoFunc = FeatureHelpMessage(u, CInt(aname), False)
                    Else
                        EvalInfoFunc = FeatureHelpMessage(u, aname, False)
                    End If
                End If
            End If
        Case "規定パイロット数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.Data.PilotNum)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.PilotNum)
            End If
        Case "パイロット数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.CountPilot)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.PilotNum)
            End If
        Case "サポート数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.CountSupport)
            End If
        Case "最大アイテム数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.Data.ItemNum)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.ItemNum)
            End If
        Case "アイテム数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.CountItem)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.ItemNum)
            End If
        Case "アイテム"
            If Not u Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    i = CInt(params(idx + 1))
                    If 0 < i And i <= u.CountItem Then
                        EvalInfoFunc = Format$(u.Item(i).Name)
                    End If
                End If
            End If
        Case "アイテムＩＤ"
            If Not u Is Nothing Then
                If IsNumber(params(idx + 1)) Then
                    i = CInt(params(idx + 1))
                    If 0 < i And i <= u.CountItem Then
                        EvalInfoFunc = Format$(u.Item(i).ID)
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
                EvalInfoFunc = Format$(u.Speed)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.Speed)
            End If
        Case "サイズ"
            If Not u Is Nothing Then
                EvalInfoFunc = u.Size
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = ud.Size
            End If
        Case "修理費"
            If Not u Is Nothing Then
                EvalInfoFunc = u.Value
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = ud.Value
            End If
        Case "最大ＨＰ"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxHP)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.HP)
            End If
        Case "ＨＰ"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.HP)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.HP)
            End If
        Case "最大ＥＮ"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxEN)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.EN)
            End If
        Case "ＥＮ"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.EN)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.EN)
            End If
        Case "装甲"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.Armor)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.Armor)
            End If
        Case "運動性"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.Mobility)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.Mobility)
            End If
        Case "武器数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.CountWeapon)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.CountWeapon)
            ElseIf Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Data.CountWeapon)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.CountWeapon)
            ElseIf Not it Is Nothing Then
                EvalInfoFunc = Format$(it.CountWeapon)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.CountWeapon)
            End If
        Case "武器"
            idx = idx + 1
            If Not u Is Nothing Then
                With u
                    '何番目の武器かを判定
                    If IsNumber(params(idx)) Then
                        i = CInt(params(idx))
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
                            EvalInfoFunc = Format$(.WeaponPower(i, ""))
                        Case "射程", "最大射程"
                            EvalInfoFunc = Format$(.WeaponMaxRange(i))
                        Case "最小射程"
                            EvalInfoFunc = Format$(.Weapon(i).MinRange)
                        Case "命中率"
                            EvalInfoFunc = Format$(.WeaponPrecision(i))
                        Case "最大弾数"
                            EvalInfoFunc = Format$(.MaxBullet(i))
                        Case "弾数"
                            EvalInfoFunc = Format$(.Bullet(i))
                        Case "消費ＥＮ"
                            EvalInfoFunc = Format$(.WeaponENConsumption(i))
                        Case "必要気力"
                            EvalInfoFunc = Format$(.Weapon(i).NecessaryMorale)
                        Case "地形適応"
                            EvalInfoFunc = .Weapon(i).Adaption
                        Case "クリティカル率"
                            EvalInfoFunc = Format$(.WeaponCritical(i))
                        Case "属性"
                            EvalInfoFunc = .WeaponClass(i)
                        Case "属性所有"
                            If .IsWeaponClassifiedAs(i, params(idx + 1)) Then
                                EvalInfoFunc = "1"
                            Else
                                EvalInfoFunc = "0"
                            End If
                        Case "属性レベル"
                            EvalInfoFunc = .WeaponLevel(i, params(idx + 1))
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.Power)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "命中率"
                                EvalInfoFunc = Format$(.Precision)
                            Case "最大弾数", "弾数"
                                EvalInfoFunc = Format$(.Bullet)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "地形適応"
                                EvalInfoFunc = .Adaption
                            Case "クリティカル率"
                                EvalInfoFunc = Format$(.Critical)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.Power)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "命中率"
                                EvalInfoFunc = Format$(.Precision)
                            Case "最大弾数", "弾数"
                                EvalInfoFunc = Format$(.Bullet)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "地形適応"
                                EvalInfoFunc = .Adaption
                            Case "クリティカル率"
                                EvalInfoFunc = Format$(.Critical)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.Power)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "命中率"
                                EvalInfoFunc = Format$(.Precision)
                            Case "最大弾数", "弾数"
                                EvalInfoFunc = Format$(.Bullet)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "地形適応"
                                EvalInfoFunc = .Adaption
                            Case "クリティカル率"
                                EvalInfoFunc = Format$(.Critical)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.Power)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "命中率"
                                EvalInfoFunc = Format$(.Precision)
                            Case "最大弾数", "弾数"
                                EvalInfoFunc = Format$(.Bullet)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "地形適応"
                                EvalInfoFunc = .Adaption
                            Case "クリティカル率"
                                EvalInfoFunc = Format$(.Critical)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.Power)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "命中率"
                                EvalInfoFunc = Format$(.Precision)
                            Case "最大弾数", "弾数"
                                EvalInfoFunc = Format$(.Bullet)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "地形適応"
                                EvalInfoFunc = .Adaption
                            Case "クリティカル率"
                                EvalInfoFunc = Format$(.Critical)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                EvalInfoFunc = Format$(u.CountAbility)
            ElseIf Not ud Is Nothing Then
                EvalInfoFunc = Format$(ud.CountAbility)
            ElseIf Not p Is Nothing Then
                EvalInfoFunc = Format$(p.Data.CountAbility)
            ElseIf Not pd Is Nothing Then
                EvalInfoFunc = Format$(pd.CountAbility)
            ElseIf Not it Is Nothing Then
                EvalInfoFunc = Format$(it.CountAbility)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.CountAbility)
            End If
        Case "アビリティ"
            idx = idx + 1
            If Not u Is Nothing Then
                With u
                    '何番目のアビリティかを判定
                    If IsNumber(params(idx)) Then
                        i = CInt(params(idx))
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
                            EvalInfoFunc = Format$(.Ability(i).CountEffect)
                        Case "効果タイプ"
                            '何番目の効果かを判定
                            If IsNumber(params(idx + 1)) Then
                                j = CInt(params(idx + 1))
                            End If
                            If j <= 0 And .Ability(i).CountEffect < j Then
                                Exit Function
                            End If
                            EvalInfoFunc = .Ability(i).EffectType(j)
                        Case "効果レベル"
                            '何番目の効果かを判定
                            If IsNumber(params(idx + 1)) Then
                                j = CInt(params(idx + 1))
                            End If
                            If j <= 0 And .Ability(i).CountEffect < j Then
                                Exit Function
                            End If
                            EvalInfoFunc = Format$(.Ability(i).EffectLevel(j))
                        Case "効果データ"
                            '何番目の効果かを判定
                            If IsNumber(params(idx + 1)) Then
                                j = CInt(params(idx + 1))
                            End If
                            If j <= 0 And .Ability(i).CountEffect < j Then
                                Exit Function
                            End If
                            EvalInfoFunc = .Ability(i).EffectData(j)
                        Case "射程", "最大射程"
                            EvalInfoFunc = Format$(.AbilityMaxRange(i))
                        Case "最小射程"
                            EvalInfoFunc = Format$(.AbilityMinRange(i))
                        Case "最大使用回数"
                            EvalInfoFunc = Format$(.MaxStock(i))
                        Case "使用回数"
                            EvalInfoFunc = Format$(.Stock(i))
                        Case "消費ＥＮ"
                            EvalInfoFunc = Format$(.AbilityENConsumption(i))
                        Case "必要気力"
                            EvalInfoFunc = Format$(.Ability(i).NecessaryMorale)
                        Case "属性"
                            EvalInfoFunc = .Ability(i).Class
                        Case "属性所有"
                            If .IsAbilityClassifiedAs(i, params(idx + 1)) Then
                                EvalInfoFunc = "1"
                            Else
                                EvalInfoFunc = "0"
                            End If
                        Case "属性レベル"
                            EvalInfoFunc = .AbilityLevel(i, params(idx + 1))
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.CountEffect)
                            Case "効果タイプ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectType(j)
                            Case "効果レベル"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = Format$(.EffectLevel(j))
                            Case "効果データ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectData(j)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "最大使用回数", "使用回数"
                                EvalInfoFunc = Format$(.Stock)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.CountEffect)
                            Case "効果タイプ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectType(j)
                            Case "効果レベル"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = Format$(.EffectLevel(j))
                            Case "効果データ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectData(j)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "最大使用回数", "使用回数"
                                EvalInfoFunc = Format$(.Stock)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.CountEffect)
                            Case "効果タイプ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectType(j)
                            Case "効果レベル"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = Format$(.EffectLevel(j))
                            Case "効果データ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectData(j)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "最大使用回数", "使用回数"
                                EvalInfoFunc = Format$(.Stock)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.CountEffect)
                            Case "効果タイプ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectType(j)
                            Case "効果レベル"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = Format$(.EffectLevel(j))
                            Case "効果データ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectData(j)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "最大使用回数", "使用回数"
                                EvalInfoFunc = Format$(.Stock)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                        i = CInt(params(idx))
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
                                EvalInfoFunc = Format$(.CountEffect)
                            Case "効果タイプ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectType(j)
                            Case "効果レベル"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = Format$(.EffectLevel(j))
                            Case "効果データ"
                                '何番目の効果かを判定
                                If IsNumber(params(idx + 1)) Then
                                    j = CInt(params(idx + 1))
                                End If
                                If j <= 0 Or .CountEffect < j Then
                                    Exit Function
                                End If
                                EvalInfoFunc = .EffectData(j)
                            Case "射程", "最大射程"
                                EvalInfoFunc = Format$(.MaxRange)
                            Case "最小射程"
                                EvalInfoFunc = Format$(.MinRange)
                            Case "最大使用回数", "使用回数"
                                EvalInfoFunc = Format$(.Stock)
                            Case "消費ＥＮ"
                                EvalInfoFunc = Format$(.ENConsumption)
                            Case "必要気力"
                                EvalInfoFunc = Format$(.NecessaryMorale)
                            Case "属性"
                                EvalInfoFunc = .Class
                            Case "属性所有"
                                If InStrNotNest(.Class, params(idx + 1)) > 0 Then
                                    EvalInfoFunc = "1"
                                Else
                                    EvalInfoFunc = "0"
                                End If
                            Case "属性レベル"
                                j = InStrNotNest(.Class, params(idx + 1) & "L")
                                If j = 0 Then
                                    EvalInfoFunc = "0"
                                    Exit Function
                                End If
                                
                                EvalInfoFunc = ""
                                j = j + Len(params(idx + 1)) + 1
                                Do
                                    EvalInfoFunc = EvalInfoFunc & Mid$(.Class, j, 1)
                                    j = j + 1
                                Loop While IsNumber(Mid$(.Class, j, 1))
                                
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
                EvalInfoFunc = Format$(u.Rank)
            End If
        Case "ボスランク"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.BossRank)
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
                        If .IsWeaponMastered(i) _
                            And Not .IsDisabled(.Weapon(i).Name) _
                            And Not .IsWeaponClassifiedAs(i, "合") _
                        Then
                            If .WeaponPower(i, "") > max_value Then
                                max_value = .WeaponPower(i, "")
                            End If
                        End If
                    Next
                    EvalInfoFunc = Format$(max_value)
                End With
            ElseIf Not ud Is Nothing Then
                With ud
                    max_value = 0
                    For i = 1 To .CountWeapon
                        If InStr(.Weapon(i).Class, "合") = 0 Then
                            If .Weapon(i).Power > max_value Then
                                max_value = .Weapon(i).Power
                            End If
                        End If
                    Next
                    EvalInfoFunc = Format$(max_value)
                End With
            End If
        Case "最長射程"
            If Not u Is Nothing Then
                With u
                    max_value = 0
                    For i = 1 To .CountWeapon
                        If .IsWeaponMastered(i) _
                            And Not .IsDisabled(.Weapon(i).Name) _
                            And Not .IsWeaponClassifiedAs(i, "合") _
                        Then
                            If .WeaponMaxRange(i) > max_value Then
                                max_value = .WeaponMaxRange(i)
                            End If
                        End If
                    Next
                    EvalInfoFunc = Format$(max_value)
                End With
            ElseIf Not ud Is Nothing Then
                With ud
                    max_value = 0
                    For i = 1 To .CountWeapon
                        If InStr(.Weapon(i).Class, "合") = 0 Then
                            If .Weapon(i).MaxRange > max_value Then
                                max_value = .Weapon(i).MaxRange
                            End If
                        End If
                    Next
                    EvalInfoFunc = Format$(max_value)
                End With
            End If
        Case "残りサポートアタック数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxSupportAttack - u.UsedSupportAttack)
            End If
        Case "残りサポートガード数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxSupportGuard - u.UsedSupportGuard)
            End If
        Case "残り同時援護攻撃数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxSyncAttack - u.UsedSyncAttack)
            End If
        Case "残りカウンター攻撃数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(u.MaxCounterAttack - u.UsedCounterAttack)
            End If
        Case "改造費"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(RankUpCost(u))
            End If
        Case "最大改造数"
            If Not u Is Nothing Then
                EvalInfoFunc = Format$(MaxRank(u))
            End If
        Case "アイテムクラス"
            If Not it Is Nothing Then
                EvalInfoFunc = it.Class
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = itd.Class
            End If
        Case "装備個所"
            If Not it Is Nothing Then
                EvalInfoFunc = it.Part
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = itd.Part
            End If
        Case "最大ＨＰ修正値"
            If Not it Is Nothing Then
                EvalInfoFunc = Format$(it.HP)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.HP)
            End If
        Case "最大ＥＮ修正値"
            If Not it Is Nothing Then
                EvalInfoFunc = Format$(it.EN)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.EN)
            End If
        Case "装甲修正値"
            If Not it Is Nothing Then
                EvalInfoFunc = Format$(it.Armor)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.Armor)
            End If
        Case "運動性修正値"
            If Not it Is Nothing Then
                EvalInfoFunc = Format$(it.Mobility)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.Mobility)
            End If
        Case "移動力修正値"
            If Not it Is Nothing Then
                EvalInfoFunc = Format$(it.Speed)
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = Format$(itd.Speed)
            End If
        Case "解説文", "コメント"
            If Not it Is Nothing Then
                EvalInfoFunc = it.Data.Comment
                ReplaceString EvalInfoFunc, vbCr & vbLf, " "
            ElseIf Not itd Is Nothing Then
                EvalInfoFunc = itd.Comment
                ReplaceString EvalInfoFunc, vbCr & vbLf, " "
            ElseIf Not spd Is Nothing Then
                EvalInfoFunc = spd.Comment
            End If
        Case "短縮名"
            If Not spd Is Nothing Then
                EvalInfoFunc = spd.ShortName
            End If
        Case "消費ＳＰ"
            If Not spd Is Nothing Then
                EvalInfoFunc = Format$(spd.SPConsumption)
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
                EvalInfoFunc = Format$(spd.CountEffect)
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
                    EvalInfoFunc = Format$(spd.EffectLevel(i))
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
                        If Left$(EvalInfoFunc, Len(ScenarioPath)) = ScenarioPath Then
                            EvalInfoFunc = Mid$(EvalInfoFunc, Len(ScenarioPath) + 1)
                        End If
                    End If
                Case "幅"
                    EvalInfoFunc = Format$(MapWidth)
                Case "時間帯"
                    If MapDrawMode <> "" Then
                        If MapDrawMode = "フィルタ" Then
                            buf = Hex(MapDrawFilterColor)
                            For i = 1 To 6 - Len(buf)
                                buf = "0" & buf
                            Next
                            buf = "#" & Mid$(buf, 5, 2) & Mid$(buf, 3, 2) & Mid$(buf, 1, 2) _
                                & " " & CStr(MapDrawFilterTransPercent * 100) & "%"
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
                    EvalInfoFunc = Format$(MapHeight)
                Case Else
                    Dim mx As Integer, my As Integer
                    
                    If IsNumber(params(idx)) Then
                        mx = CInt(params(idx))
                    End If
                    idx = idx + 1
                    If IsNumber(params(idx)) Then
                        my = CInt(params(idx))
                    End If
                    
                    If mx < 1 Or MapWidth < mx _
                        Or my < 1 Or MapHeight < my _
                    Then
                        Exit Function
                    End If
                    
                    idx = idx + 1
                    Select Case params(idx)
                        Case "地形名"
                            EvalInfoFunc = TerrainName(mx, my)
                        Case "地形タイプ", "地形クラス"
                            EvalInfoFunc = TerrainClass(mx, my)
                        Case "移動コスト"
                            '0.5刻みの移動コストを使えるようにするため、移動コストは
                            '実際の２倍の値で記録されている
                            EvalInfoFunc = Format$(TerrainMoveCost(mx, my) / 2)
                        Case "回避修正"
                            EvalInfoFunc = Format$(TerrainEffectForHit(mx, my))
                        Case "ダメージ修正"
                            EvalInfoFunc = Format$(TerrainEffectForDamage(mx, my))
                        Case "ＨＰ回復量"
                            EvalInfoFunc = Format$(TerrainEffectForHPRecover(mx, my))
                        Case "ＥＮ回復量"
                            EvalInfoFunc = Format$(TerrainEffectForENRecover(mx, my))
                        Case "ビットマップ名"
'MOD START 240a
'                            Select Case MapImageFileTypeData(mx, my)
'                                Case SeparateDirMapImageFileType
'                                    EvalInfoFunc = _
'                                        TDList.Bitmap(MapData(mx, my, 0)) & "\" & _
'                                        TDList.Bitmap(MapData(mx, my, 0)) & _
'                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
'                                Case FourFiguresMapImageFileType
'                                    EvalInfoFunc = _
'                                        TDList.Bitmap(MapData(mx, my, 0)) & _
'                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
'                                Case OldMapImageFileType
'                                    EvalInfoFunc = _
'                                        TDList.Bitmap(MapData(mx, my, 0)) & _
'                                        Format$(MapData(mx, my, 1)) & ".bmp"
'                            End Select
                            Select Case MapImageFileTypeData(mx, my)
                                Case SeparateDirMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.TerrainType)) & "\" & _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.TerrainType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.BitmapNo), "0000") & ".bmp"
                                Case FourFiguresMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.TerrainType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.BitmapNo), "0000") & ".bmp"
                                Case OldMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.TerrainType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.BitmapNo)) & ".bmp"
                            End Select
'MOD  END  240a
'ADD START 240a
                        Case "レイヤービットマップ名"
                            Select Case MapImageFileTypeData(mx, my)
                                Case SeparateDirMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.LayerType)) & "\" & _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.LayerType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
                                Case FourFiguresMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.LayerType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.LayerBitmapNo), "0000") & ".bmp"
                                Case OldMapImageFileType
                                    EvalInfoFunc = _
                                        TDList.Bitmap(MapData(mx, my, MapDataIndex.LayerType)) & _
                                        Format$(MapData(mx, my, MapDataIndex.LayerBitmapNo)) & ".bmp"
                            End Select
'ADD  END  240a
                        Case "ユニットＩＤ"
                            If Not MapDataForUnit(mx, my) Is Nothing Then
                                EvalInfoFunc = MapDataForUnit(mx, my).ID
                            End If
                    End Select
            End Select
        Case "オプション"
            idx = idx + 1
            Select Case params(idx)
                Case "MessageWait"
                    EvalInfoFunc = Format$(MessageWait)
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
'                    "AutoMoveCursor", "DebugMode", "LastFolder", _
'                    "MIDIPortID", "MP3Volume", _
'                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
'                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
'                    "UseTransparentBlt"
' 「NewGUI」で探しに来たらINIの状態を返す。「新ＧＵＩ」で探しに来たらOptionの状態を返す。
                Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", _
                    "AutoMoveCursor", "DebugMode", "LastFolder", _
                    "MIDIPortID", "MP3Volume", _
                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
                    "UseTransparentBlt", "NewGUI"
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
Public Function GetVariable(var_name As String, etype As ValueType, _
    str_result As String, num_result As Double) As ValueType
Dim vname As String
Dim i As Integer, num As Integer
Dim u As Unit
Dim ret As Long
Dim idx As String, ipara As String, buf As String
Dim start_idx As Integer, depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim is_term As Boolean

    vname = var_name
    
    '未定義値の設定
    str_result = var_name
    
    '変数が配列？
    ret = InStr(vname, "[")
    If ret = 0 Then
        GoTo SkipArrayHandling
    End If
    If Right$(vname, 1) <> "]" Then
        GoTo SkipArrayHandling
    End If
    
    'ここから配列専用の処理
    
    'インデックス部分の切りだし
    idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
    
    '多次元配列の処理
    If InStr(idx, ",") > 0 Then
        start_idx = 1
        depth = 0
        is_term = True
        For i = 1 To Len(idx)
            If in_single_quote Then
                If Asc(Mid$(idx, i, 1)) = 96 Then '`
                    in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If Asc(Mid$(idx, i, 1)) = 34 Then '"
                    in_double_quote = False
                End If
            Else
                Select Case Asc(Mid$(idx, i, 1))
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
                            ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
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
        ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
        If Len(buf) > 0 Then
            idx = buf & "," & GetValueAsString(ipara, is_term)
        Else
            idx = GetValueAsString(ipara, is_term)
        End If
    Else
        idx = GetValueAsString(idx)
    End If
    
    '変数名を配列のインデックス部を計算して再構築
    vname = Left$(vname, ret) & idx & "]"
    
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
                         Case NumericType
                             If .VariableType = NumericType Then
                                 num_result = .NumericValue
                             Else
                                 num_result = StrToDbl(.StringValue)
                             End If
                             GetVariable = NumericType
                         Case StringType
                             If .VariableType = StringType Then
                                 str_result = .StringValue
                             Else
                                 str_result = FormatNum(.NumericValue)
                             End If
                             GetVariable = StringType
                         Case UndefinedType
                             If .VariableType = StringType Then
                                 str_result = .StringValue
                                 GetVariable = StringType
                             Else
                                 num_result = .NumericValue
                                 GetVariable = NumericType
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
                Case NumericType
                    If .VariableType = NumericType Then
                        num_result = .NumericValue
                    Else
                        num_result = StrToDbl(.StringValue)
                    End If
                    GetVariable = NumericType
                Case StringType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                    Else
                        str_result = FormatNum(.NumericValue)
                    End If
                    GetVariable = StringType
                Case UndefinedType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                        GetVariable = StringType
                    Else
                        num_result = .NumericValue
                        GetVariable = NumericType
                    End If
            End Select
        End With
        Exit Function
    End If
    
    'グローバル変数
    If IsGlobalVariableDefined(vname) Then
        With GlobalVariableList.Item(vname)
            Select Case etype
                Case NumericType
                    If .VariableType = NumericType Then
                        num_result = .NumericValue
                    Else
                        num_result = StrToDbl(.StringValue)
                    End If
                    GetVariable = NumericType
                Case StringType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                    Else
                        str_result = FormatNum(.NumericValue)
                    End If
                    GetVariable = StringType
                Case UndefinedType
                    If .VariableType = StringType Then
                        str_result = .StringValue
                        GetVariable = StringType
                    Else
                        num_result = .NumericValue
                        GetVariable = NumericType
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
            GetVariable = StringType
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
            GetVariable = StringType
            Exit Function
            
        Case "対象ユニットＩＤ"
            If Not SelectedUnitForEvent Is Nothing Then
                str_result = SelectedUnitForEvent.ID
            Else
                str_result = ""
            End If
            GetVariable = StringType
            Exit Function
            
        Case "相手ユニットＩＤ"
            If Not SelectedTargetForEvent Is Nothing Then
                str_result = SelectedTargetForEvent.ID
            Else
                str_result = ""
            End If
            GetVariable = StringType
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
            GetVariable = StringType
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
            GetVariable = StringType
            Exit Function
            
        Case "対象ユニット使用武器番号"
            str_result = ""
            If SelectedUnitForEvent Is SelectedUnit Then
                With SelectedUnitForEvent
                    If etype = StringType Then
                        str_result = Format$(SelectedWeapon)
                        GetVariable = StringType
                    Else
                        num_result = SelectedWeapon
                        GetVariable = NumericType
                    End If
                End With
            ElseIf SelectedUnitForEvent Is SelectedTarget Then
                With SelectedUnitForEvent
                    If etype = StringType Then
                        str_result = Format$(SelectedTWeapon)
                        GetVariable = StringType
                    Else
                        num_result = SelectedTWeapon
                        GetVariable = NumericType
                    End If
                End With
            End If
            Exit Function
            
        Case "相手ユニット使用武器番号"
            str_result = ""
            If SelectedTargetForEvent Is SelectedTarget Then
                With SelectedTargetForEvent
                    If etype = StringType Then
                        str_result = Format$(SelectedTWeapon)
                        GetVariable = StringType
                    Else
                        num_result = SelectedTWeapon
                        GetVariable = NumericType
                    End If
                End With
            ElseIf SelectedTargetForEvent Is SelectedUnit Then
                With SelectedTargetForEvent
                    If etype = StringType Then
                        str_result = Format$(SelectedWeapon)
                        GetVariable = StringType
                    Else
                        num_result = SelectedWeapon
                        GetVariable = NumericType
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
            GetVariable = StringType
            Exit Function
            
        Case "対象ユニット使用アビリティ番号"
            str_result = ""
            If SelectedUnitForEvent Is SelectedUnit Then
                With SelectedUnitForEvent
                    If etype = StringType Then
                        str_result = Format$(SelectedAbility)
                        GetVariable = StringType
                    Else
                        num_result = SelectedAbility
                        GetVariable = NumericType
                    End If
                End With
            End If
            Exit Function
            
        Case "対象ユニット使用スペシャルパワー"
            str_result = ""
            If SelectedUnitForEvent Is SelectedUnit Then
                str_result = SelectedSpecialPower
            End If
            GetVariable = StringType
            Exit Function
            
        Case "サポートアタックユニットＩＤ"
            If Not SupportAttackUnit Is Nothing Then
                str_result = SupportAttackUnit.ID
            Else
                str_result = ""
            End If
            GetVariable = StringType
            Exit Function
            
        Case "サポートガードユニットＩＤ"
            If Not SupportGuardUnit Is Nothing Then
                str_result = SupportGuardUnit.ID
            Else
                str_result = ""
            End If
            GetVariable = StringType
            Exit Function
            
        Case "選択"
            If etype = NumericType Then
                num_result = StrToDbl(SelectedAlternative)
                GetVariable = NumericType
            Else
                str_result = SelectedAlternative
                GetVariable = StringType
            End If
            Exit Function
            
        Case "ターン数"
            If etype = StringType Then
                str_result = Format$(Turn)
                GetVariable = StringType
            Else
                num_result = Turn
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "総ターン数"
            If etype = StringType Then
                str_result = Format$(TotalTurn)
                GetVariable = StringType
            Else
                num_result = TotalTurn
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "フェイズ"
            str_result = Stage
            GetVariable = StringType
            Exit Function
            
        Case "味方数"
            num = 0
            For Each u In UList
                With u
                    If .Party0 = "味方" _
                        And (.Status = "出撃" Or .Status = "格納") _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If etype = StringType Then
                str_result = Format$(num)
                GetVariable = StringType
            Else
                num_result = num
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "ＮＰＣ数"
            num = 0
            For Each u In UList
                With u
                    If .Party0 = "ＮＰＣ" _
                        And (.Status = "出撃" Or .Status = "格納") _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If etype = StringType Then
                str_result = Format$(num)
                GetVariable = StringType
            Else
                num_result = num
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "敵数"
            num = 0
            For Each u In UList
                With u
                    If .Party0 = "敵" _
                        And (.Status = "出撃" Or .Status = "格納") _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If etype = StringType Then
                str_result = Format$(num)
                GetVariable = StringType
            Else
                num_result = num
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "中立数"
            num = 0
            For Each u In UList
                With u
                    If .Party0 = "中立" _
                        And (.Status = "出撃" Or .Status = "格納") _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If etype = StringType Then
                str_result = Format$(num)
                GetVariable = StringType
            Else
                num_result = num
                GetVariable = NumericType
            End If
            Exit Function
            
        Case "資金"
            If etype = StringType Then
                str_result = FormatNum(Money)
                GetVariable = StringType
            Else
                num_result = Money
                GetVariable = NumericType
            End If
            Exit Function
            
        Case Else
            'アルファベットの変数名はlow caseで判別
            Select Case LCase$(vname)
                Case "apppath"
                    str_result = AppPath
                    GetVariable = StringType
                    Exit Function
                    
                Case "appversion"
                    With App
                        num = 10000 * .Major + 100 * .Minor + .Revision
                    End With
                    If etype = StringType Then
                        str_result = Format$(num)
                        GetVariable = StringType
                    Else
                        num_result = num
                        GetVariable = NumericType
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
                    If etype = StringType Then
                        str_result = Format$(num)
                        GetVariable = StringType
                    Else
                        num_result = num
                        GetVariable = NumericType
                    End If
                    Exit Function
                    
                Case "basex"
                    If etype = StringType Then
                        str_result = Format$(BaseX)
                        GetVariable = StringType
                    Else
                        num_result = BaseX
                        GetVariable = NumericType
                    End If
                    Exit Function
                    
                Case "basey"
                    If etype = StringType Then
                        str_result = Format$(BaseY)
                        GetVariable = StringType
                    Else
                        num_result = BaseY
                        GetVariable = NumericType
                    End If
                    Exit Function
                    
                Case "extdatapath"
                    str_result = ExtDataPath
                    GetVariable = StringType
                    Exit Function
                    
                Case "extdatapath2"
                    str_result = ExtDataPath2
                    GetVariable = StringType
                    Exit Function
                    
                Case "mousex"
                    If etype = StringType Then
                        str_result = Format$(MouseX)
                        GetVariable = StringType
                    Else
                        num_result = MouseX
                        GetVariable = NumericType
                    End If
                    Exit Function
                    
                Case "mousey"
                    If etype = StringType Then
                        str_result = Format$(MouseY)
                        GetVariable = StringType
                    Else
                        num_result = MouseY
                        GetVariable = NumericType
                    End If
                    Exit Function
                    
                Case "now"
                    str_result = CStr(Now)
                    GetVariable = StringType
                    Exit Function
                    
                Case "scenariopath"
                    str_result = ScenarioPath
                    GetVariable = StringType
                    Exit Function
            End Select
    End Select
    
    'コンフィグ変数？
    If BCVariable.IsConfig Then
        Select Case vname
            Case "攻撃値"
                If etype = StringType Then
                    str_result = Format$(BCVariable.AttackExp)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.AttackExp
                    GetVariable = NumericType
                End If
                Exit Function
            Case "攻撃側ユニットＩＤ"
                str_result = BCVariable.AtkUnit.ID
                GetVariable = StringType
                Exit Function
            Case "防御側ユニットＩＤ"
                If Not BCVariable.DefUnit Is Nothing Then
                    str_result = BCVariable.DefUnit.ID
                    GetVariable = StringType
                    Exit Function
                End If
            Case "武器番号"
                If etype = StringType Then
                    str_result = Format$(BCVariable.WeaponNumber)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.WeaponNumber
                    GetVariable = NumericType
                End If
                Exit Function
            Case "地形適応"
                If etype = StringType Then
                    str_result = Format$(BCVariable.TerrainAdaption)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.TerrainAdaption
                    GetVariable = NumericType
                End If
                Exit Function
            Case "武器威力"
                If etype = StringType Then
                    str_result = Format$(BCVariable.WeaponPower)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.WeaponPower
                    GetVariable = NumericType
                End If
                Exit Function
            Case "サイズ補正"
                If etype = StringType Then
                    str_result = Format$(BCVariable.SizeMod)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.SizeMod
                    GetVariable = NumericType
                End If
                Exit Function
            Case "装甲値"
                If etype = StringType Then
                    str_result = Format$(BCVariable.Armor)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.Armor
                    GetVariable = NumericType
                End If
                Exit Function
            Case "最終値"
                If etype = StringType Then
                    str_result = Format$(BCVariable.LastVariable)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.LastVariable
                    GetVariable = NumericType
                End If
                Exit Function
            Case "攻撃側補正"
                If etype = StringType Then
                    str_result = Format$(BCVariable.AttackVariable)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.AttackVariable
                    GetVariable = NumericType
                End If
                Exit Function
            Case "防御側補正"
                If etype = StringType Then
                    str_result = Format$(BCVariable.DffenceVariable)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.DffenceVariable
                    GetVariable = NumericType
                End If
                Exit Function
            Case "ザコ補正"
                If etype = StringType Then
                    str_result = Format$(BCVariable.CommonEnemy)
                    GetVariable = StringType
                Else
                    num_result = BCVariable.CommonEnemy
                    GetVariable = NumericType
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
                    
                    If etype = StringType Then
                        str_result = Format$(num)
                        GetVariable = StringType
                    Else
                        num_result = num
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "耐久"
                    If etype = StringType Then
                        str_result = Format$(.Defense)
                        GetVariable = StringType
                    Else
                        num_result = .Defense
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "ＬＶ"
                    If etype = StringType Then
                        str_result = Format$(.Level)
                        GetVariable = StringType
                    Else
                        num_result = .Level
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "経験"
                    If etype = StringType Then
                        str_result = Format$(.Exp)
                        GetVariable = StringType
                    Else
                        num_result = .Exp
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "ＳＰ"
                    If etype = StringType Then
                        str_result = Format$(.SP)
                        GetVariable = StringType
                    Else
                        num_result = .SP
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "霊力"
                    If etype = StringType Then
                        str_result = Format$(.Plana)
                        GetVariable = StringType
                    Else
                        num_result = .Plana
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "格闘"
                    If etype = StringType Then
                        str_result = Format$(.Infight)
                        GetVariable = StringType
                    Else
                        num_result = .Infight
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "射撃"
                    If etype = StringType Then
                        str_result = Format$(.Shooting)
                        GetVariable = StringType
                    Else
                        num_result = .Shooting
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "命中"
                    If etype = StringType Then
                        str_result = Format$(.Hit)
                        GetVariable = StringType
                    Else
                        num_result = .Hit
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "回避"
                    If etype = StringType Then
                        str_result = Format$(.Dodge)
                        GetVariable = StringType
                    Else
                        num_result = .Dodge
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "技量"
                    If etype = StringType Then
                        str_result = Format$(.Technique)
                        GetVariable = StringType
                    Else
                        num_result = .Technique
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "反応"
                    If etype = StringType Then
                        str_result = Format$(.Intuition)
                        GetVariable = StringType
                    Else
                        num_result = .Intuition
                        GetVariable = NumericType
                    End If
                    Exit Function
            End Select
        End With
        
        'ユニットに関する変数
        With BCVariable.MeUnit
            Select Case vname
                Case "最大ＨＰ"
                    If etype = StringType Then
                        str_result = Format$(.MaxHP())
                        GetVariable = StringType
                    Else
                        num_result = .MaxHP()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "現在ＨＰ"
                    If etype = StringType Then
                        str_result = Format$(.HP())
                        GetVariable = StringType
                    Else
                        num_result = .HP()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "最大ＥＮ"
                    If etype = StringType Then
                        str_result = Format$(.MaxEN())
                        GetVariable = StringType
                    Else
                        num_result = .MaxEN()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "現在ＥＮ"
                    If etype = StringType Then
                        str_result = Format$(.EN())
                        GetVariable = StringType
                    Else
                        num_result = .EN()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "移動力"
                    If etype = StringType Then
                        str_result = Format$(.Speed())
                        GetVariable = StringType
                    Else
                        num_result = .Speed()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "装甲"
                    If etype = StringType Then
                        str_result = Format$(.Armor())
                        GetVariable = StringType
                    Else
                        num_result = .Armor()
                        GetVariable = NumericType
                    End If
                    Exit Function
                Case "運動性"
                    If etype = StringType Then
                        str_result = Format$(.Mobility())
                        GetVariable = StringType
                    Else
                        num_result = .Mobility()
                        GetVariable = NumericType
                    End If
                    Exit Function
            End Select
        End With
    End If
    
    If etype = NumericType Then
        num_result = 0
        GetVariable = NumericType
    Else
        GetVariable = StringType
    End If
End Function

'指定した変数が定義されているか？
Public Function IsVariableDefined(var_name As String) As Boolean
Dim vname As String
Dim i As Integer, ret As Integer
Dim idx As String, ipara As String, buf As String
Dim start_idx As Integer, depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim is_term As Boolean

    Select Case Asc(var_name)
        Case 36 '$
            vname = Mid$(var_name, 2)
        Case Else
            vname = var_name
    End Select
    
    '変数が配列？
    ret = InStr(vname, "[")
    If ret = 0 Then
        GoTo SkipArrayHandling
    End If
    If Right$(vname, 1) <> "]" Then
        GoTo SkipArrayHandling
    End If
    
    'ここから配列専用の処理
    
    'インデックス部分の切りだし
    idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
    
    '多次元配列の処理
    If InStr(idx, ",") > 0 Then
        start_idx = 1
        depth = 0
        is_term = True
        For i = 1 To Len(idx)
            If in_single_quote Then
                If Asc(Mid$(idx, i, 1)) = 96 Then '`
                    in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If Asc(Mid$(idx, i, 1)) = 34 Then '"
                    in_double_quote = False
                End If
            Else
                Select Case Asc(Mid$(idx, i, 1))
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
                            ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
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
        ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
        If Len(buf) > 0 Then
            idx = buf & "," & GetValueAsString(ipara, is_term)
        Else
            idx = GetValueAsString(ipara, is_term)
        End If
    Else
        idx = GetValueAsString(Trim$(idx))
    End If
    
    '変数名を配列のインデックス部を計算して再構築
    vname = Left$(vname, ret) & idx & "]"
    
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
Public Function IsSubLocalVariableDefined(vname As String) As Boolean
Dim i As Integer
    
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
Public Function IsLocalVariableDefined(vname As String) As Boolean
Dim dummy As VarData
    
    On Error GoTo ErrorHandler
    Set dummy = LocalVariableList.Item(vname)
    IsLocalVariableDefined = True
    Exit Function
    
ErrorHandler:
    IsLocalVariableDefined = False
End Function

'指定した名前のグローバル変数が定義されているか？
Public Function IsGlobalVariableDefined(vname As String) As Boolean
Dim dummy As VarData
    
    On Error GoTo ErrorHandler
    Set dummy = GlobalVariableList.Item(vname)
    IsGlobalVariableDefined = True
    Exit Function
    
ErrorHandler:
    IsGlobalVariableDefined = False
End Function

'変数の値を設定
Public Sub SetVariable(var_name As String, etype As ValueType, _
    str_value As String, num_value As Double)
Dim new_var As VarData
Dim vname As String
Dim i As Integer, ret As Integer
Dim idx As String, ipara As String, buf As String
Dim vname0 As String
Dim p As Pilot, u As Unit
Dim start_idx As Integer, depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim is_term As Boolean
Dim is_subroutine_local_array As Boolean

    'Debug.Print "Set " & vname & " " & new_value
    
    vname = var_name
    
    '左辺値を伴う関数
    ret = InStr(vname, "(")
    If ret > 1 And Right$(vname, 1) = ")" Then
        Select Case LCase(Left$(vname, ret - 1))
            Case "hp"
                idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
                idx = GetValueAsString(idx)
                
                If UList.IsDefined2(idx) Then
                    Set u = UList.Item2(idx)
                ElseIf PList.IsDefined(idx) Then
                    Set u = PList.Item(idx).Unit
                Else
                    Set u = SelectedUnitForEvent
                End If
                
                If Not u Is Nothing Then
                    If etype = NumericType Then
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
                idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
                idx = GetValueAsString(idx)
                
                If UList.IsDefined2(idx) Then
                    Set u = UList.Item2(idx)
                ElseIf PList.IsDefined(idx) Then
                    Set u = PList.Item(idx).Unit
                Else
                    Set u = SelectedUnitForEvent
                End If
                
                If Not u Is Nothing Then
                    If etype = NumericType Then
                        u.EN = num_value
                    Else
                        u.EN = StrToLng(str_value)
                    End If
                    If u.EN = 0 And u.Status = "出撃" Then
                        PaintUnitBitmap u
                    End If
                End If
                Exit Sub
                
            Case "sp"
                idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
                idx = GetValueAsString(idx)
                
                If UList.IsDefined2(idx) Then
                    Set p = UList.Item2(idx).MainPilot
                ElseIf PList.IsDefined(idx) Then
                    Set p = PList.Item(idx)
                Else
                    Set p = SelectedUnitForEvent.MainPilot
                End If
                
                If Not p Is Nothing Then
                    With p
                        If .MaxSP > 0 Then
                            If etype = NumericType Then
                                .SP = num_value
                            Else
                                .SP = StrToLng(str_value)
                            End If
                        End If
                    End With
                End If
                Exit Sub
                
            Case "plana"
                idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
                idx = GetValueAsString(idx)
                
                If UList.IsDefined2(idx) Then
                    Set p = UList.Item2(idx).MainPilot
                ElseIf PList.IsDefined(idx) Then
                    Set p = PList.Item(idx)
                Else
                    Set p = SelectedUnitForEvent.MainPilot
                End If
                
                If Not p Is Nothing Then
                    With p
                        If .MaxPlana > 0 Then
                            If etype = NumericType Then
                                .Plana = num_value
                            Else
                                .Plana = StrToLng(str_value)
                            End If
                        End If
                    End With
                End If
                Exit Sub
                
            Case "action"
                idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
                idx = GetValueAsString(idx)
                
                If UList.IsDefined2(idx) Then
                    Set u = UList.Item2(idx)
                ElseIf PList.IsDefined(idx) Then
                    Set u = PList.Item(idx).Unit
                Else
                    Set u = SelectedUnitForEvent
                End If
                
                If Not u Is Nothing Then
                    If etype = NumericType Then
                        u.UsedAction = u.MaxAction - num_value
                    Else
                        u.UsedAction = u.MaxAction - StrToLng(str_value)
                    End If
                End If
                Exit Sub
                
            Case "eval"
                vname = Trim$(Mid$(vname, ret + 1, Len(vname) - ret - 1))
                vname = GetValueAsString(vname)
                
        End Select
    End If
    
    '変数が配列？
    ret = InStr(vname, "[")
    If ret = 0 Then
        GoTo SkipArrayHandling
    End If
    If Right$(vname, 1) <> "]" Then
        GoTo SkipArrayHandling
    End If
    
    'ここから配列専用の処理
    
    'インデックス部分の切りだし
    idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
    
    '多次元配列の処理
    If InStr(idx, ",") > 0 Then
        start_idx = 1
        depth = 0
        is_term = True
        For i = 1 To Len(idx)
            If in_single_quote Then
                If Asc(Mid$(idx, i, 1)) = 96 Then '`
                    in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If Asc(Mid$(idx, i, 1)) = 34 Then '"
                    in_double_quote = False
                End If
            Else
                Select Case Asc(Mid$(idx, i, 1))
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
                            ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
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
        ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
        If Len(buf) > 0 Then
            idx = buf & "," & GetValueAsString(ipara, is_term)
        Else
            idx = GetValueAsString(ipara, is_term)
        End If
    Else
        idx = GetValueAsString(Trim$(idx))
    End If
    
    '変数名を配列のインデックス部を計算して再構築
    vname = Left$(vname, ret) & idx & "]"
    
    '配列名
    vname0 = Left$(vname, ret - 1)
    
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
            DisplayEventErrorMessage CurrentLineNum, _
                "作成したサブルーチンローカル変数の総数が" & _
                Format$(MaxVarIndex) & "個を超えています"
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
            .Name = vname
            .VariableType = etype
            .StringValue = str_value
            .NumericValue = num_value
        End With
        Exit Sub
    End If
    
    'グローバル変数として定義済み？
    If IsGlobalVariableDefined(vname) Then
        With GlobalVariableList.Item(vname)
            .Name = vname
            .VariableType = etype
            .StringValue = str_value
            .NumericValue = num_value
        End With
        Exit Sub
    End If
    
    'システム変数？
    Select Case LCase$(vname)
        Case "basex"
            If etype = NumericType Then
                BaseX = num_value
            Else
                BaseX = StrToLng(str_value)
            End If
            MainForm.picMain(0).CurrentX = BaseX
            Exit Sub
        Case "basey"
            If etype = NumericType Then
                BaseY = num_value
            Else
                BaseY = StrToLng(str_value)
            End If
            MainForm.picMain(0).CurrentY = BaseY
            Exit Sub
        Case "ターン数"
            If etype = NumericType Then
                Turn = num_value
            Else
                Turn = StrToLng(str_value)
            End If
            Exit Sub
        Case "総ターン数"
            If etype = NumericType Then
                TotalTurn = num_value
            Else
                TotalTurn = StrToLng(str_value)
            End If
            Exit Sub
        Case "資金"
            Money = 0
            If etype = NumericType Then
                IncrMoney num_value
            Else
                IncrMoney StrToLng(str_value)
            End If
            Exit Sub
    End Select
    
    '未定義だった場合
    
    '配列の要素として作成
    If Len(vname0) <> 0 Then
        'ローカル変数の配列の要素として定義
        If IsLocalVariableDefined(vname0) Then
            'Nop
        'グローバル変数の配列の要素として定義
        ElseIf IsGlobalVariableDefined(vname0) Then
            DefineGlobalVariable vname
            With GlobalVariableList.Item(vname)
                .Name = vname
                .VariableType = etype
                .StringValue = str_value
                .NumericValue = num_value
            End With
            Exit Sub
        '未定義の配列なのでローカル変数の配列を作成
        Else
            Dim new_var2 As VarData
            'ローカル変数の配列のメインＩＤを作成
            Set new_var2 = New VarData
            With new_var2
                .Name = vname0
                .VariableType = StringType
                If InStr(.Name, """") > 0 Then
                    DisplayEventErrorMessage CurrentLineNum, _
                        "不正な変数「" & .Name & "」が作成されました"
                End If
            End With
            LocalVariableList.Add new_var2, vname0
        End If
    End If
    
    'ローカル変数として作成
    Set new_var = New VarData
    With new_var
        .Name = vname
        .VariableType = etype
        .StringValue = str_value
        .NumericValue = num_value
        If InStr(.Name, """") > 0 Then
            DisplayEventErrorMessage CurrentLineNum, _
                "不正な変数「" & .Name & "」が作成されました"
        End If
    End With
    LocalVariableList.Add new_var, vname
End Sub

Public Sub SetVariableAsString(vname As String, new_value As String)
    SetVariable vname, StringType, new_value, 0
End Sub

Public Sub SetVariableAsDouble(vname As String, ByVal new_value As Double)
    SetVariable vname, NumericType, "", new_value
End Sub

Public Sub SetVariableAsLong(vname As String, ByVal new_value As Long)
    SetVariable vname, NumericType, "", CDbl(new_value)
End Sub

'グローバル変数を定義
Public Sub DefineGlobalVariable(vname As String)
Dim new_var As New VarData

    With new_var
        .Name = vname
        .VariableType = StringType
        .StringValue = ""
    End With
    GlobalVariableList.Add new_var, vname
End Sub

'ローカル変数を定義
Public Sub DefineLocalVariable(vname As String)
Dim new_var As New VarData

    With new_var
        .Name = vname
        .VariableType = StringType
        .StringValue = ""
    End With
    LocalVariableList.Add new_var, vname
End Sub

'変数を消去
Public Sub UndefineVariable(var_name As String)
Dim var As VarData
Dim vname As String, vname2 As String
Dim i As Integer, ret As Integer
Dim idx As String, buf As String
Dim start_idx As Integer, depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim is_term As Boolean
    
    If Asc(var_name) = 36 Then '$
        vname = Mid$(var_name, 2)
    Else
        vname = var_name
    End If
    
    'Eval関数
    If LCase$(Left$(vname, 5)) = "eval(" Then
        If Right$(vname, 1) = ")" Then
            vname = Mid$(vname, 6, Len(vname) - 6)
            vname = GetValueAsString(vname)
        End If
    End If
    
    '配列の要素？
    ret = InStr(vname, "[")
    If ret = 0 Then
        GoTo SkipArrayHandling:
    End If
    If Right$(vname, 1) <> "]" Then
        GoTo SkipArrayHandling:
    End If
    
    '配列の要素を指定された場合
    
    'インデックス部分の切りだし
    idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
    
    '多次元配列の処理
    If InStr(idx, ",") > 0 Then
        start_idx = 1
        depth = 0
        is_term = True
        For i = 1 To Len(idx)
            If in_single_quote Then
                If Asc(Mid$(idx, i, 1)) = 96 Then '`
                    in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If Asc(Mid$(idx, i, 1)) = 34 Then '"
                    in_double_quote = False
                End If
            Else
                Select Case Asc(Mid$(idx, i, 1))
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
                            buf = buf & _
                                GetValueAsString(Mid$(idx, start_idx, i - start_idx), _
                                    is_term)
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
            idx = buf & "," & _
                GetValueAsString(Mid$(idx, start_idx, i - start_idx), is_term)
        Else
            idx = GetValueAsString(Mid$(idx, start_idx, i - start_idx), is_term)
        End If
    Else
        idx = GetValueAsString(idx)
    End If
    
    'インデックス部分を評価して変数名を置き換え
    vname = Left$(vname, ret) & idx & "]"
    
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
        LocalVariableList.Remove vname
        Exit Sub
    End If
    
    'グローバル変数？
    If IsGlobalVariableDefined(vname) Then
        GlobalVariableList.Remove vname
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
        LocalVariableList.Remove vname
        For Each var In LocalVariableList
            With var
                If InStr(.Name, vname2) = 1 Then
                    LocalVariableList.Remove .Name
                End If
            End With
        Next
        Exit Sub
    End If
    
    'グローバル変数？
    If IsGlobalVariableDefined(vname) Then
        GlobalVariableList.Remove vname
        For Each var In GlobalVariableList
            With var
                If InStr(.Name, vname2) = 1 Then
                    GlobalVariableList.Remove .Name
                End If
            End With
        Next
        Exit Sub
    End If
End Sub



' === その他の関数 ===

'式を文字列として評価
Public Function GetValueAsString(expr As String, _
    Optional ByVal is_term As Boolean) As String
Dim num As Double

     If is_term Then
         EvalTerm expr, StringType, GetValueAsString, num
     Else
         EvalExpr expr, StringType, GetValueAsString, num
     End If
End Function

'式を浮動小数点数として評価
Public Function GetValueAsDouble(expr As String, _
    Optional ByVal is_term As Boolean) As Double
Dim buf As String

     If is_term Then
         EvalTerm expr, NumericType, buf, GetValueAsDouble
     Else
         EvalExpr expr, NumericType, buf, GetValueAsDouble
     End If
End Function

'式を整数として評価
Public Function GetValueAsLong(expr As String, _
    Optional ByVal is_term As Boolean) As Long
Dim buf As String, num As Double

     If is_term Then
         EvalTerm expr, NumericType, buf, num
     Else
         EvalExpr expr, NumericType, buf, num
     End If
     GetValueAsLong = num
End Function


'strが式かどうかチェック
'(疑わしきは式と判断している)
Public Function IsExpr(str As String) As Boolean
     Select Case Asc(str)
         Case 36 '$
             IsExpr = True
         Case 40 '(
             IsExpr = True
     End Select
End Function


'指定したオプションが設定されているか？
Public Function IsOptionDefined(oname As String) As Boolean
Dim dummy As VarData
    
    On Error GoTo ErrorHandler
    Set dummy = GlobalVariableList.Item("Option(" & oname & ")")
    IsOptionDefined = True
    Exit Function
    
ErrorHandler:
    IsOptionDefined = False
End Function


'str に対して式置換を行う
Public Sub ReplaceSubExpression(str As String)
Dim start_idx As Integer, end_idx As Integer
Dim str_len As Integer
Dim i As Integer, n As Integer

    Do While True
        '式置換が存在する？
        start_idx = InStr(str, "$(")
        If start_idx = 0 Then
            Exit Sub
        End If
        
        '式置換の終了位置を調べる
        str_len = Len(str)
        n = 1
        For i = start_idx + 2 To str_len
            Select Case Mid$(str, i, 1)
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
        str = Left$(str, start_idx - 1) & _
            GetValueAsString(Mid$(str, start_idx + 2, end_idx - start_idx - 2)) & _
            Right$(str, str_len - end_idx)
    Loop
End Sub

'msg に対して式置換等の処理を行う
Public Sub FormatMessage(msg As String)
    'ちゃんと横棒がつながって表示されるように罫線文字に置換
    If ReplaceString(msg, "――", "──") Then
        ReplaceString msg, "─―", "──"
    ElseIf ReplaceString(msg, "ーー", "──") Then
        ReplaceString msg, "─ー", "──"
    End If
    
    '式置換
    ReplaceSubExpression msg
End Sub


'用語tnameの表示名を参照する
'tlenが指定された場合は文字列長を強制的にtlenに合わせる
Public Function Term(tname As String, Optional u As Unit, _
    Optional ByVal tlen As Integer) As String
Dim vname As String, i As Integer

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
            Term = GlobalVariableList.Item(vname).StringValue
        Else
            Term = tname
        End If
    End If
    
    '表示幅の調整
    If tlen > 0 Then
        If LenB(StrConv(Term, vbFromUnicode)) < tlen Then
            Term = RightPaddedString(Term, tlen)
        End If
    End If
End Function


'引数1で指定した変数のオブジェクトを取得
Public Function GetVariableObject(var_name As String) As VarData
Dim vname As String
Dim i As Integer, num As Integer
Dim u As Unit
Dim ret As Long
Dim idx As String, ipara As String, buf As String
Dim start_idx As Integer, depth As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
Dim is_term As Boolean

Dim etype As ValueType, str_result As String, num_result As Double

    vname = var_name
    
    '変数が配列？
    ret = InStr(vname, "[")
    If ret = 0 Then
        GoTo SkipArrayHandling
    End If
    If Right$(vname, 1) <> "]" Then
        GoTo SkipArrayHandling
    End If
    
    'ここから配列専用の処理
    
    'インデックス部分の切りだし
    idx = Mid$(vname, ret + 1, Len(vname) - ret - 1)
    
    '多次元配列の処理
    If InStr(idx, ",") > 0 Then
        start_idx = 1
        depth = 0
        is_term = True
        For i = 1 To Len(idx)
            If in_single_quote Then
                If Asc(Mid$(idx, i, 1)) = 96 Then '`
                    in_single_quote = False
                End If
            ElseIf in_double_quote Then
                If Asc(Mid$(idx, i, 1)) = 34 Then '"
                    in_double_quote = False
                End If
            Else
                Select Case Asc(Mid$(idx, i, 1))
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
                            ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
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
        ipara = Trim$(Mid$(idx, start_idx, i - start_idx))
        If Len(buf) > 0 Then
            idx = buf & "," & GetValueAsString(ipara, is_term)
        Else
            idx = GetValueAsString(ipara, is_term)
        End If
    Else
        idx = GetValueAsString(idx)
    End If
    
    '変数名を配列のインデックス部を計算して再構築
    vname = Left$(vname, ret) & idx & "]"
    
    '配列専用の処理が終了
    
SkipArrayHandling:
    
    'ここから配列と通常変数の共通処理
    
    'サブルーチンローカル変数
    If CallDepth > 0 Then
        For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
            If vname = VarStack(i).Name Then
                Set GetVariableObject = VarStack(i)
                Exit Function
            End If
        Next
    End If
    
    'ローカル変数
    If IsLocalVariableDefined(vname) Then
        Set GetVariableObject = LocalVariableList.Item(vname)
        Exit Function
    End If
    
    'グローバル変数
    If IsGlobalVariableDefined(vname) Then
        Set GetVariableObject = GlobalVariableList.Item(vname)
        Exit Function
    End If
    
    'システム変数？
    etype = UndefinedType
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
            etype = StringType
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
            etype = StringType
        Case "対象ユニットＩＤ"
            If Not SelectedUnitForEvent Is Nothing Then
                str_result = SelectedUnitForEvent.ID
            Else
                str_result = ""
            End If
            etype = StringType
        Case "相手ユニットＩＤ"
            If Not SelectedTargetForEvent Is Nothing Then
                str_result = SelectedTargetForEvent.ID
            Else
                str_result = ""
            End If
            etype = StringType
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
            etype = StringType
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
            etype = StringType
        Case "対象ユニット使用武器番号"
            If SelectedUnitForEvent Is SelectedUnit Then
                num_result = SelectedWeapon
            ElseIf SelectedUnitForEvent Is SelectedTarget Then
                num_result = SelectedTWeapon
            End If
            etype = NumericType
        Case "相手ユニット使用武器番号"
            If SelectedTargetForEvent Is SelectedTarget Then
                num_result = SelectedTWeapon
            ElseIf SelectedTargetForEvent Is SelectedUnit Then
                num_result = SelectedWeapon
            End If
            etype = NumericType
        Case "対象ユニット使用アビリティ"
            If SelectedUnitForEvent Is SelectedUnit Then
                If SelectedAbility > 0 Then
                    str_result = SelectedAbilityName
                Else
                    str_result = ""
                End If
            End If
            etype = StringType
        Case "対象ユニット使用アビリティ番号"
            If SelectedUnitForEvent Is SelectedUnit Then
                num_result = SelectedAbility
            End If
            etype = NumericType
        Case "対象ユニット使用スペシャルパワー"
            If SelectedUnitForEvent Is SelectedUnit Then
                str_result = SelectedSpecialPower
            End If
            etype = StringType
        Case "選択"
            If IsNumeric(SelectedAlternative) Then
                num_result = StrToDbl(SelectedAlternative)
                etype = NumericType
            Else
                str_result = SelectedAlternative
                etype = StringType
            End If
        Case "ターン数"
            num_result = Turn
            etype = NumericType
        Case "総ターン数"
            num_result = TotalTurn
            etype = NumericType
        Case "フェイズ"
            str_result = Stage
            etype = StringType
        Case "味方数", "ＮＰＣ数", "敵数", "中立数"
            num = 0
            For Each u In UList
                With u
                    If .Party0 = Left(vname, Len(vname) - 1) _
                        And (.Status = "出撃" Or .Status = "格納") _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            num_result = num
            etype = NumericType
        Case "資金"
            num_result = Money
            etype = NumericType
        Case Else
            'アルファベットの変数名はlow caseで判別
            Select Case LCase$(vname)
                Case "apppath"
                    str_result = AppPath
                    etype = StringType
                Case "appversion"
                    With App
                        num = 10000 * .Major + 100 * .Minor + .Revision
                    End With
                    num_result = num
                    etype = NumericType
                Case "argnum"
                    num = ArgIndex - ArgIndexStack(CallDepth - 1 - UpVarLevel)
                    num_result = num
                    etype = NumericType
                Case "basex"
                    num_result = BaseX
                    etype = NumericType
                Case "basey"
                    num_result = BaseY
                    etype = NumericType
                Case "extdatapath"
                    str_result = ExtDataPath
                    etype = StringType
                Case "extdatapath2"
                    str_result = ExtDataPath2
                    etype = StringType
                Case "mousex"
                    num_result = MouseX
                    etype = NumericType
                Case "mousey"
                    num_result = MouseY
                    etype = NumericType
                Case "now"
                    str_result = CStr(Now)
                    etype = StringType
                Case "scenariopath"
                    str_result = ScenarioPath
                    etype = StringType
            End Select
    End Select
    
    If etype <> UndefinedType Then
        Set GetVariableObject = New VarData
        With GetVariableObject
            .Name = vname
            .VariableType = etype
            .StringValue = str_result
            .NumericValue = num_result
        End With
    End If
End Function
