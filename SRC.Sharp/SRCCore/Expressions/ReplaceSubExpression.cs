// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.VB;

namespace SRC.Core.Expressions
{
    public partial class Expression
    {
        // str に対して式置換を行う
        public void ReplaceSubExpression(ref string str)
        {
            int start_idx, end_idx = default;
            int str_len;
            int i, n;
            while (true)
            {
                // 式置換が存在する？
                start_idx = Strings.InStr(str, "$(");
                if (start_idx == 0)
                {
                    return;
                }

                // 式置換の終了位置を調べる
                str_len = Strings.Len(str);
                n = 1;
                var loopTo = str_len;
                for (i = (start_idx + 2); i <= loopTo; i++)
                {
                    switch (Strings.Mid(str, i, 1) ?? "")
                    {
                        case ")":
                            {
                                n = n - 1;
                                if (n == 0)
                                {
                                    end_idx = i;
                                    break;
                                }

                                break;
                            }

                        case "(":
                            {
                                n = n + 1;
                                break;
                            }
                    }
                }

                if (i > str_len)
                {
                    return;
                }

                // 式置換を実施
                string localGetValueAsString(string localStr)
                {
                    string argexpr = Strings.Mid(localStr, start_idx + 2, end_idx - start_idx - 2);
                    return GetValueAsString(argexpr);
                }
                str = Strings.Left(str, start_idx - 1) + localGetValueAsString(str) + Strings.Right(str, str_len - end_idx);
            }
        }
    }
}
