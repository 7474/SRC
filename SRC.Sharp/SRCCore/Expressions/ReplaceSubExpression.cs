// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.VB;

namespace SRC.Core.Expressions
{
    public static partial class Expression
    {
        // str に対して式置換を行う
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static void ReplaceSubExpression(ref string str_Renamed)
        {
            short start_idx, end_idx = default;
            short str_len;
            short i, n;
            while (true)
            {
                // 式置換が存在する？
                start_idx = (short)Strings.InStr(str_Renamed, "$(");
                if (start_idx == 0)
                {
                    return;
                }

                // 式置換の終了位置を調べる
                str_len = (short)Strings.Len(str_Renamed);
                n = 1;
                var loopTo = str_len;
                for (i = (short)(start_idx + 2); i <= loopTo; i++)
                {
                    switch (Strings.Mid(str_Renamed, i, 1) ?? "")
                    {
                        case ")":
                            {
                                n = (short)(n - 1);
                                if (n == 0)
                                {
                                    end_idx = i;
                                    break;
                                }

                                break;
                            }

                        case "(":
                            {
                                n = (short)(n + 1);
                                break;
                            }
                    }
                }

                if (i > str_len)
                {
                    return;
                }

                // 式置換を実施
                string localGetValueAsString(string localStr) { string argexpr = Strings.Mid(localStr, start_idx + 2, end_idx - start_idx - 2); var ret = GetValueAsString(ref argexpr); return ret; }

                str_Renamed = Strings.Left(str_Renamed, start_idx - 1) + localGetValueAsString(str_Renamed) + Strings.Right(str_Renamed, str_len - end_idx);
            }
        }
    }
}
