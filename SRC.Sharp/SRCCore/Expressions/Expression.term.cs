// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // 用語tnameの表示名を参照する
        // tlenが指定された場合は文字列長を強制的にtlenに合わせる
        public string Term(string tname, Unit u = null, int tlen = 0)
        {
            var TermRet = "";
            // ユニットが用語名能力を持っている場合はそちらを優先
            if (u != null)
            {
                if (u.IsFeatureAvailable("用語名"))
                {
                    foreach (var fd in u.Features.Where(x => x.Name == "用語名"))
                    {

                        if ((GeneralLib.LIndex(fd.Data, 1) ?? "") == (tname ?? ""))
                        {
                            TermRet = GeneralLib.LIndex(fd.Data, 2);
                            break;
                        }
                    }
                }
            }

            // RenameTermで用語名が変更されているかチェック
            if (Strings.Len(TermRet) == 0)
            {
                string vname;
                switch (tname ?? "")
                {
                    case "HP":
                    case "EN":
                    case "SP":
                    case "CT":
                        {
                            vname = "ShortTerm(" + tname + ")";
                            break;
                        }

                    default:
                        {
                            vname = "Term(" + tname + ")";
                            break;
                        }
                }

                if (IsGlobalVariableDefined(vname))
                {
                    TermRet = Conversions.ToString(Event.GlobalVariableList[vname].StringValue);
                }
                else
                {
                    TermRet = tname;
                }
            }

            // 表示幅の調整
            if (tlen > 0)
            {
                if (Strings.LenB(TermRet) < tlen)
                {
                    TermRet = GeneralLib.RightPaddedString(TermRet, tlen);
                }
            }

            return TermRet;
        }
    }
}
