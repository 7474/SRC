using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    //RegExp正規表現で文字列を検索
    //RegExpReplace正規表現で検索した文字列を置換

    public class RegExp : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Regexp
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExp_Error' at character 111360


            //                        Input:
            //                                        On Error GoTo RegExp_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // RegExp(文字列, パターン[,大小区別あり|大小区別なし])
            //                        buf = "";
            //                        if (pcount > 0)
            //                        {
            //                            // 文字列全体を検索
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.Global = (object)true;
            //                            // 大文字小文字の区別（True=区別しない）
            //                            // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.IgnoreCase = (object)false;
            //                            if (pcount >= 3)
            //                            {
            //                                if (GetValueAsString(@params[3], is_term[3]) == "大小区別なし")
            //                                {
            //                                    // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                    RegEx.IgnoreCase = (object)true;
            //                                }
            //                            }
            //                            // 検索パターン
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Execute の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            Matches = RegEx.Execute(GetValueAsString(@params[1], is_term[1]));
            //                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Matches.Count, 0, false)))
            //                            {
            //                                regexp_index = -1;
            //                            }
            //                            else
            //                            {
            //                                regexp_index = 0;
            //                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }
            //                        else if (regexp_index >= 0)
            //                        {
            //                            regexp_index = (regexp_index + 1);
            //                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLessEqual(regexp_index, Operators.SubtractObject(Matches.Count, 1), false)))
            //                            {
            //                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }

            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExp_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
            //                        return CallFunctionRet;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class RegExpReplace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Regexpreplace
            //                // RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

            //                case "regexpreplace":
            //                    {
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExpReplace...' at character 114835


            //                        Input:
            //                                        'RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

            //                                        On Error GoTo RegExpReplace_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // 文字列全体を検索
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.Global = (object)true;
            //                        // 大文字小文字の区別（True=区別しない）
            //                        // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.IgnoreCase = (object)false;
            //                        if (pcount >= 4)
            //                        {
            //                            if (GetValueAsString(@params[4], is_term[4]) == "大小区別なし")
            //                            {
            //                                // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                RegEx.IgnoreCase = (object)true;
            //                            }
            //                        }
            //                        // 検索パターン
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);

            //                        // 置換実行
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Replace の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        buf = Conversions.ToString(RegEx.Replace(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[3], is_term[3])));
            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExpReplace_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
            //                        return CallFunctionRet;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}