using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Commands
{
    public class ArrayCmd : CmdData
    {
        public ArrayCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ArrayCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            //            object array_buf;
            //            var array_buf2 = default(string[]);
            //            string buf;
            //            string var_name, vname;
            //            int i;
            //            short num;
            //            bool IsList;
            //            Expression.ValueType etype;
            //            string str_value;
            //            double num_value;
            //            string sep;
            //            if (ArgNum != 4)
            //            {
            //                Event.EventErrorMessage = "Arrayコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 73625


            //                Input:
            //                            Error(0)

            //                 */
            //            }
            //            else if (GetArgAsString(4) == "リスト")
            //            {
            //                IsList = true;
            //            }
            //            else
            //            {
            //                IsList = false;
            //            }

            //            // 代入先の変数名
            //            var_name = GetArg(2);
            //            if (Strings.Left(var_name, 1) == "$")
            //            {
            //                var_name = Strings.Mid(var_name, 2);
            //            }
            //            // Eval関数
            //            if (Strings.LCase(Strings.Left(var_name, 5)) == "eval(")
            //            {
            //                if (Strings.Right(var_name, 1) == ")")
            //                {
            //                    var_name = Strings.Mid(var_name, 6, Strings.Len(var_name) - 6);
            //                    var_name = Expression.GetValueAsString(var_name);
            //                }
            //            }

            //            // 代入先の変数を初期化した上で再設定
            //            // サブルーチンローカル変数の場合
            //            if (Expression.IsSubLocalVariableDefined(var_name))
            //            {
            //                Expression.UndefineVariable(var_name);
            //                Event.VarIndex = (Event.VarIndex + 1);
            //                {
            //                    var withBlock = Event.VarStack[Event.VarIndex];
            //                    withBlock.Name = var_name;
            //                    withBlock.VariableType = Expression.ValueType.NumericType;
            //                    withBlock.StringValue = "";
            //                    withBlock.NumericValue = 0d;
            //                }
            //            }
            //            // ローカル変数の場合
            //            else if (Expression.IsLocalVariableDefined(var_name))
            //            {
            //                Expression.UndefineVariable(var_name);
            //                Expression.DefineLocalVariable(var_name);
            //            }
            //            // グローバル変数の場合
            //            else if (Expression.IsGlobalVariableDefined(var_name))
            //            {
            //                Expression.UndefineVariable(var_name);
            //                Expression.DefineGlobalVariable(var_name);
            //            }

            //            if (IsList)
            //            {
            //                // リストを配列に変換
            //                num = GeneralLib.ListSplit(GetArgAsString(3), array_buf2);
            //                // UPGRADE_WARNING: オブジェクト array_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                array_buf = SrcFormatter.CopyArray(array_buf2);
            //            }
            //            // 文字列を分割して配列に代入
            //            else
            //            {
            //                ;
            //#error Cannot convert ReDimStatementSyntax - see comment for details
            //                /* Cannot convert ReDimStatementSyntax, System.InvalidCastException: 型 'Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbolWithEmittedNamespaceName' のオブジェクトを型 'Microsoft.CodeAnalysis.IArrayTypeSymbol' にキャストできません。
            //                   場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.CreateNewArrayAssignment(ExpressionSyntax vbArrayExpression, ExpressionSyntax csArrayExpression, List`1 convertedBounds, Int32 nodeSpanStart)
            //                   場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<ConvertRedimClauseAsync>d__41.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<<VisitReDimStatement>b__40_0>d.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.Shared.AsyncEnumerableTaskExtensions.<SelectAsync>d__3`2.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.Shared.AsyncEnumerableTaskExtensions.<SelectManyAsync>d__0`2.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitReDimStatement>d__40.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            //                --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //                   場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            //                Input:
            //                            '文字列を分割して配列に代入
            //                            ReDim array_buf(0)

            //                 */
            //                buf = GetArgAsString(3);
            //                sep = GetArgAsString(4);
            //                i = Strings.InStr(buf, sep);
            //                while (i > 0)
            //                {
            //                    Array.Resize(array_buf, Information.UBound((Array)array_buf) + 1 + 1);
            //                    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    array_buf((object)Information.UBound((Array)array_buf)) = Strings.Left(buf, i - 1);
            //                    buf = Strings.Mid(buf, i + Strings.Len(sep));
            //                    i = Strings.InStr(buf, sep);
            //                }

            //                Array.Resize(array_buf, Information.UBound((Array)array_buf) + 1 + 1);
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                array_buf((object)Information.UBound((Array)array_buf)) = buf;
            //            }

            //            var loopTo = Information.UBound((Array)array_buf);
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                buf = Conversions.ToString(array_buf((object)i));
            //                GeneralLib.TrimString(buf);
            //                if (Information.IsNumeric(buf))
            //                {
            //                    etype = Expression.ValueType.NumericType;
            //                    str_value = "";
            //                    num_value = GeneralLib.StrToDbl(buf);
            //                }
            //                else
            //                {
            //                    etype = Expression.ValueType.StringType;
            //                    str_value = buf;
            //                    num_value = 0d;
            //                }

            //                vname = var_name + "[" + i.ToString() + "]";
            //                Expression.SetVariable(vname, etype, str_value, num_value);
            //            }

            //            ExecArrayCmdRet = LineNum + 1;
            //            return ExecArrayCmdRet;
            return EventData.NextID;
        }
    }
}
