using System;
using System.Text.RegularExpressions;

namespace SRCCore.Expressions.Functions
{
    //RegExp正規表現で文字列を検索
    //RegExpReplace正規表現で検索した文字列を置換

    // 元の SRC（Expression.bas の CallFunction の Case "regexp"・"regexpreplace"）に合わせている。
    // 元の SRC は VBScript.RegExp を使い、検索の結果（Matches・regexp_index）は CallFunction の外に持つ。
    // SRC# では SRC ごとの関数のインスタンスに持つ（SRC が 1 つなら同じ動作）。
    public class RegExp : AFunction
    {
        private MatchCollection lastMatch;
        private int lastMatchIndex;

        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // RegExp(文字列, パターン[,大小区別あり|大小区別なし])
            var result = "";
            if (pcount > 0)
            {
                // 文字列全体を検索
                // 大文字小文字の区別（True=区別しない）
                var ignoreCase = false;
                if (pcount >= 3)
                {
                    if (SRC.Expression.GetValueAsString(@params[3], is_term[3]) == "大小区別なし")
                    {
                        ignoreCase = true;
                    }
                }
                // 検索パターン
                var pattern = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                var input = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (!RegExpOptions.TryCreate(SRC, pattern, ignoreCase, out var regex))
                {
                    return ValueType.UndefinedType;
                }
                lastMatch = regex.Matches(input);
                if (lastMatch.Count == 0)
                {
                    lastMatch = null;
                }
                else
                {
                    lastMatchIndex = 0;
                    result = lastMatch[lastMatchIndex].Value;
                }
            }
            else if (lastMatch != null)
            {
                lastMatchIndex++;
                if (lastMatch.Count > lastMatchIndex)
                {
                    result = lastMatch[lastMatchIndex].Value;
                }
            }

            str_result = result;
            return ValueType.StringType;
        }
    }

    internal static class RegExpOptions
    {
        // 元の SRC は VBScript.RegExp（JScript と同じ ECMAScript の正規表現）を使う。
        // \d・\w・\s は ASCII の文字だけに合う（全角の数字・全角の空白には合わない）ので、ECMAScript の動作にする。
        public static RegexOptions Of(bool ignoreCase)
        {
            return RegexOptions.ECMAScript | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        // 元の SRC はパターンの誤りなどで VBScript がエラーを出すと、エラーを表示して関数の値を求めずに続ける
        // （On Error GoTo RegExp_Error。VB の文言は「VBScriptがインストールされていません」）。
        public static bool TryCreate(SRC SRC, string pattern, bool ignoreCase, out Regex regex)
        {
            try
            {
                regex = new Regex(pattern, Of(ignoreCase));
                return true;
            }
            catch (ArgumentException)
            {
                regex = null;
                SRC.Event.DisplayEventErrorMessage(SRC.Event.CurrentLineNum, "正規表現のパターンが不正です");
                return false;
            }
        }
    }

    public class RegExpReplace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])
            var ignoreCase = false;
            if (pcount >= 4)
            {
                if (SRC.Expression.GetValueAsString(@params[4], is_term[4]) == "大小区別なし")
                {
                    ignoreCase = true;
                }
            }
            // 検索パターン
            var pattern = SRC.Expression.GetValueAsString(@params[2], is_term[2]);

            // 置換実行
            var input = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var replacement = SRC.Expression.GetValueAsString(@params[3], is_term[3]);
            if (!RegExpOptions.TryCreate(SRC, pattern, ignoreCase, out var regex))
            {
                return ValueType.UndefinedType;
            }
            str_result = regex.Replace(input, replacement);

            return ValueType.StringType;
        }
    }
}
