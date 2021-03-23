// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // TODO ref 取って戻り値か out 変数を受けるようにする決断をする。

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
                    var endKakko = false;
                    switch (Strings.Mid(str, i, 1) ?? "")
                    {
                        case ")":
                            {
                                n = n - 1;
                                if (n == 0)
                                {
                                    end_idx = i;
                                    endKakko = true;
                                }
                                break;
                            }

                        case "(":
                            {
                                n = n + 1;
                                break;
                            }
                    }
                    if (endKakko) { break; }
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

        // msg に対して式置換等の処理を行う
        public void FormatMessage(ref string msg)
        {
            // ちゃんと横棒がつながって表示されるように罫線文字に置換
            msg = msg.Replace("――", "──")
                .Replace("ーー", "──")
                .Replace("─―", "──")
                .Replace("─ー", "──");
            // 式置換
            ReplaceSubExpression(ref msg);
        }
    }
}
