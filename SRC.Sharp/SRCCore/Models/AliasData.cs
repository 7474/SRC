// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Models
{
    public class AliasDataElement
    {
        // エリアスの種類
        public string strAliasType;
        // エリアスのレベル
        public double dblAliasLevel;
        // エリアスのレベルが＋修正値かどうか
        public bool blnAliasLevelIsPlusMod;
        // エリアスのレベルが×修正値かどうか
        public bool blnAliasLevelIsMultMod;
        // エリアスのデータ
        public string strAliasData;
        // エリアスの必要技能
        public string strAliasNecessarySkill;
        // エリアスの必要条件
        public string strAliasNecessaryCondition;
    }

    // エリアスデータのクラス
    public class AliasDataType
    {
        // 名称
        public string Name;
        public IList<AliasDataElement> Elements = new List<AliasDataElement>();

        // エリアスの個数
        public int Count => Elements.Count;

        // エリアスを登録
        public void AddAlias(string adef)
        {
            var elm = new AliasDataElement();
            string buf;

            // 必要技能の切り出し
            if (Strings.Right(adef, 1) == ")")
            {
                var i = Strings.InStr(adef, " (");
                if (i > 0)
                {
                    elm.strAliasNecessarySkill = Strings.Trim(Strings.Mid(adef, i + 2, Strings.Len(adef) - i - 2));
                    buf = Strings.Trim(Strings.Left(adef, i));
                }
                else if (Strings.Left(adef, 1) == "(")
                {
                    elm.strAliasNecessarySkill = Strings.Trim(Strings.Mid(adef, 2, Strings.Len(adef) - 2));
                    buf = "";
                }
                else
                {
                    buf = adef;
                }
            }
            else
            {
                buf = adef;
            }

            // 必要条件の切り出し
            if (Strings.Right(buf, 1) == ">")
            {
                var i = Strings.InStr(adef, " <");
                if (i > 0)
                {
                    elm.strAliasNecessaryCondition = Strings.Trim(Strings.Mid(buf, i + 2, Strings.Len(buf) - i - 2));
                    buf = Strings.Trim(Strings.Left(buf, i));
                }
                else if (Strings.Left(buf, 1) == "<")
                {
                    elm.strAliasNecessaryCondition = Strings.Trim(Strings.Mid(buf, 2, Strings.Len(buf) - 2));
                    buf = "";
                }
            }

            // レベル指定が修正値か？
            if (buf.Contains("Lv+"))
            {
                buf = buf.Replace("Lv+", "Lv");
                elm.blnAliasLevelIsPlusMod = true;
            }
            else if (buf.Contains("Lv*"))
            {
                buf = buf.Replace("Lv*", "Lv");
                elm.blnAliasLevelIsMultMod = true;
            }

            // エリアスの種類、レベル、データを切り出し
            {
                double alevel = Constants.DEFAULT_LEVEL;
                var i = Strings.InStr(buf, "Lv");
                var j = Strings.InStr(buf, "=");
                if (i > 0 && j > 0 && i > j)
                {
                    i = 0;
                }

                string atype;
                string adata;
                if (i > 0)
                {
                    atype = Strings.Left(buf, i - 1);
                    if (j > 0)
                    {
                        alevel = Conversions.ToDouble(Strings.Mid(buf, i + 2, j - (i + 2)));
                        adata = Strings.Mid(buf, j + 1);
                    }
                    else
                    {
                        alevel = Conversions.ToDouble(Strings.Mid(buf, i + 2));
                        adata = "";
                    }
                }
                else if (j > 0)
                {
                    atype = Strings.Left(buf, j - 1);
                    adata = Strings.Mid(buf, j + 1);
                }
                else
                {
                    atype = buf;
                    adata = "";
                }

                // エリアスを登録
                elm.strAliasType = atype;
                elm.dblAliasLevel = alevel;
                elm.strAliasData = adata;

                Elements.Add(elm);
            }
        }

        // 能力の置き換え名を取得する
        public string ReplaceTypeName(string aname)
        {
            AliasDataType alias = this;
            var aliasElem = alias.Elements.FirstOrDefault(x => GeneralLib.LIndex(x.strAliasData, 1) == aname);

            if (aliasElem != null)
            {
                aname = aliasElem.strAliasType;
            }
            else
            {
                aname = alias.Elements.First().strAliasType;
            }

            return aname;
        }

        public bool HasType(string aname)
        {
            return Elements.Any(x => x.strAliasType == aname);
        }

        // 特殊能力 ftype をエリアスの定義に従って展開した特殊能力定義を返す
        // (ユニット、パイロット、アイテムの特殊能力で共通)
        public IList<FeatureData> ExpandFeature(string ftype, string fdata, double flevel, string nskill, string ncondition)
        {
            fdata = fdata ?? "";
            var results = new List<FeatureData>();
            foreach (var elm in Elements)
            {
                var fd = new FeatureData();

                // エリアスの定義に従って特殊能力定義を置き換える
                fd.Name = elm.strAliasType;
                var adata = elm.strAliasData ?? "";
                if ((elm.strAliasType ?? "") != (ftype ?? ""))
                {
                    if (elm.blnAliasLevelIsPlusMod)
                    {
                        if (flevel == Constants.DEFAULT_LEVEL)
                        {
                            flevel = 1d;
                        }

                        if (elm.dblAliasLevel == Constants.DEFAULT_LEVEL)
                        {
                            fd.Level = flevel + 1d;
                        }
                        else
                        {
                            fd.Level = flevel + elm.dblAliasLevel;
                        }
                    }
                    else if (elm.blnAliasLevelIsMultMod)
                    {
                        if (flevel == Constants.DEFAULT_LEVEL)
                        {
                            flevel = 1d;
                        }

                        if (elm.dblAliasLevel == Constants.DEFAULT_LEVEL)
                        {
                            fd.Level = flevel;
                        }
                        else
                        {
                            fd.Level = flevel * elm.dblAliasLevel;
                        }
                    }
                    else if (flevel != Constants.DEFAULT_LEVEL)
                    {
                        fd.Level = flevel;
                    }
                    else
                    {
                        fd.Level = elm.dblAliasLevel;
                    }

                    if (!string.IsNullOrEmpty(fdata) && Strings.InStr(adata, "非表示") != 1)
                    {
                        fd.StrData = fdata + " " + GeneralLib.ListTail(adata, GeneralLib.LLength(fdata) + 1);
                    }
                    else
                    {
                        fd.StrData = adata;
                    }

                    if (elm.blnAliasLevelIsMultMod)
                    {
                        fd.StrData = fd.StrData.Replace("Lv1", "Lv" + SrcFormatter.Format(flevel));
                    }
                }
                else
                {
                    // 特殊能力解説の定義
                    if (!string.IsNullOrEmpty(fdata) && GeneralLib.LIndex(fdata, 1) != "非表示")
                    {
                        fd.Name = GeneralLib.LIndex(fdata, 1);
                    }

                    fd.StrData = adata;
                }

                if (!string.IsNullOrEmpty(nskill))
                {
                    fd.NecessarySkill = nskill;
                }
                else
                {
                    fd.NecessarySkill = elm.strAliasNecessarySkill;
                }

                if (!string.IsNullOrEmpty(ncondition))
                {
                    fd.NecessaryCondition = ncondition;
                }
                else
                {
                    fd.NecessaryCondition = elm.strAliasNecessaryCondition;
                }

                results.Add(fd);
            }
            return results;
        }
    }
}
