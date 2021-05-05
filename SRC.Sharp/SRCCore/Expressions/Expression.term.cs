// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // 用語tnameの表示名を参照する
        // tlenが指定された場合は文字列長を強制的にtlenに合わせる
        public string Term(string tname, Unit u = null, short tlen = 0)
        {
            return tname;
            // TODO Impl
            //string TermRet = default;
            //string vname;
            //short i;

            //// ユニットが用語名能力を持っている場合はそちらを優先
            //if (u is object)
            //{
            //    if (u.IsFeatureAvailable("用語名"))
            //    {
            //        var loopTo = u.CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            if (u.Feature(i) == "用語名")
            //            {
            //                string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //                if ((GeneralLib.LIndex(localFeatureData1(), 1) ?? "") == (tname ?? ""))
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //                    TermRet = GeneralLib.LIndex(localFeatureData(), 2);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}

            //// RenameTermで用語名が変更されているかチェック
            //if (Strings.Len(TermRet) == 0)
            //{
            //    switch (tname ?? "")
            //    {
            //        case "HP":
            //        case "EN":
            //        case "SP":
            //        case "CT":
            //            {
            //                vname = "ShortTerm(" + tname + ")";
            //                break;
            //            }

            //        default:
            //            {
            //                vname = "Term(" + tname + ")";
            //                break;
            //            }
            //    }

            //    if (IsGlobalVariableDefined(vname))
            //    {
            //        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        TermRet = Conversions.ToString(Event.GlobalVariableList[vname].StringValue);
            //    }
            //    else
            //    {
            //        TermRet = tname;
            //    }
            //}

            //// 表示幅の調整
            //if (tlen > 0)
            //{
            //    // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            //    // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
            //    if (LenB(Strings.StrConv(TermRet, vbFromUnicode)) < tlen)
            //    {
            //        TermRet = GeneralLib.RightPaddedString(TermRet, tlen);
            //    }
            //}

            //return TermRet;
        }
    }
}
