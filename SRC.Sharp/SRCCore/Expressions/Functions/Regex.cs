using System.Text.RegularExpressions;

namespace SRCCore.Expressions.Functions
{
    //RegExp正規表現で文字列を検索
    //RegExpReplace正規表現で検索した文字列を置換

    // XXX 全般に未疎通かつ互換性分からん
    public class RegExp : AFunction
    {
        // XXX 現状状態はここに持っていて動作上の問題ないがそうじゃないだろという感じ
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
                var regex = new Regex(SRC.Expression.GetValueAsString(@params[2], is_term[2]), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                lastMatch = regex.Matches(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
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

    public class RegExpReplace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])
            var ignoreCase = false;
            if (pcount >= 3)
            {
                if (SRC.Expression.GetValueAsString(@params[4], is_term[4]) == "大小区別なし")
                {
                    ignoreCase = true;
                }
            }
            // 検索パターン
            var regex = new Regex(SRC.Expression.GetValueAsString(@params[2], is_term[2]), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            // 置換実行
            str_result = regex.Replace(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));

            return ValueType.StringType;
        }
    }
}