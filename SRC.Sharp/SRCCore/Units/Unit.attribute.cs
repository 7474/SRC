// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Units
{
    // === 防御属性判定処理 ===
    public partial class Unit
    {
        // 属性 aname に対して吸収属性を持つか？
        public bool Absorb(string aname)
        {
            bool AbsorbRet = default;
            string c;
            int i;
            int slen;

            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strAbsorb, "全") > 0)
            {
                AbsorbRet = true;
                return AbsorbRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                if (GeneralLib.InStrNotNest(strAbsorb, "物") > 0)
                {
                    AbsorbRet = true;
                }

                return AbsorbRet;
            }

            // 属性に該当するかを判定
            i = 1;
            slen = Strings.Len(strAbsorb);
            while (i <= slen)
            {
                // 属性をひとまとめずつ取得
                c = GeneralLib.GetClassBundle(strAbsorb, ref i);
                switch (c ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aname, "魔") == 0 & GeneralLib.InStrNotNest(aname, "精") == 0)
                            {
                                AbsorbRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            if (GeneralLib.InStrNotNest(aname, "魔") > 0)
                            {
                                if (GeneralLib.InStrNotNest(aname, "魔武") == 0 & GeneralLib.InStrNotNest(aname, "魔突") == 0 & GeneralLib.InStrNotNest(aname, "魔接") == 0 & GeneralLib.InStrNotNest(aname, "魔銃") == 0 & GeneralLib.InStrNotNest(aname, "魔実") == 0)
                                {
                                    AbsorbRet = true;
                                    break;
                                }
                            }

                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aname, c) > 0)
                            {
                                AbsorbRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            return AbsorbRet;
        }

        // 属性 aname に対して無効化属性を持つか？
        public bool Immune(string aname)
        {
            bool ImmuneRet = default;
            string c;
            int i;
            int slen;

            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strImmune, "全") > 0)
            {
                ImmuneRet = true;
                return ImmuneRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                if (GeneralLib.InStrNotNest(strImmune, "物") > 0)
                {
                    ImmuneRet = true;
                }

                return ImmuneRet;
            }

            // 属性に該当するかを判定
            i = 1;
            slen = Strings.Len(strImmune);
            while (i <= slen)
            {
                // 属性をひとまとめずつ取得
                c = GeneralLib.GetClassBundle(strImmune, ref i);
                switch (c ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aname, "魔") == 0 & GeneralLib.InStrNotNest(aname, "精") == 0)
                            {
                                ImmuneRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            if (GeneralLib.InStrNotNest(aname, "魔") > 0 & GeneralLib.InStrNotNest(aname, "魔武") == 0 & GeneralLib.InStrNotNest(aname, "魔突") == 0 & GeneralLib.InStrNotNest(aname, "魔接") == 0 & GeneralLib.InStrNotNest(aname, "魔銃") == 0 & GeneralLib.InStrNotNest(aname, "魔実") == 0)
                            {
                                ImmuneRet = true;
                                break;
                            }

                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aname, c) > 0)
                            {
                                ImmuneRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            return ImmuneRet;
        }

        // 属性 aname に対して耐性属性を持つか？
        public bool Resist(string aname)
        {
            bool ResistRet = default;
            string c;
            int i;
            int slen;

            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strResist, "全") > 0)
            {
                ResistRet = true;
                return ResistRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                if (GeneralLib.InStrNotNest(strResist, "物") > 0)
                {
                    ResistRet = true;
                }

                return ResistRet;
            }

            // 属性に該当するかを判定
            i = 1;
            slen = Strings.Len(strResist);
            while (i <= slen)
            {
                // 属性をひとまとめずつ取得
                c = GeneralLib.GetClassBundle(strResist, ref i);
                switch (c ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aname, "魔") == 0 & GeneralLib.InStrNotNest(aname, "精") == 0)
                            {
                                ResistRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            if (GeneralLib.InStrNotNest(aname, "魔") > 0 & GeneralLib.InStrNotNest(aname, "魔武") == 0 & GeneralLib.InStrNotNest(aname, "魔突") == 0 & GeneralLib.InStrNotNest(aname, "魔接") == 0 & GeneralLib.InStrNotNest(aname, "魔銃") == 0 & GeneralLib.InStrNotNest(aname, "魔実") == 0)
                            {
                                ResistRet = true;
                                break;
                            }

                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aname, c) > 0)
                            {
                                ResistRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            return ResistRet;
        }

        // 属性 aname に対して弱点属性を持つか？
        public bool Weakness(string aname)
        {
            bool WeaknessRet = default;
            string c;
            int i;
            int slen;

            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strWeakness, "全") > 0)
            {
                WeaknessRet = true;
                return WeaknessRet;
            }

            if (Strings.Len(aname) == 0)
            {
                if (GeneralLib.InStrNotNest(strWeakness, "物") > 0)
                {
                    WeaknessRet = true;
                }

                return WeaknessRet;
            }

            i = 1;
            slen = Strings.Len(strWeakness);
            while (i <= slen)
            {
                // 属性をひとまとめずつ取得
                c = GeneralLib.GetClassBundle(strWeakness, ref i);
                switch (c ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aname, "魔") == 0 & GeneralLib.InStrNotNest(aname, "精") == 0)
                            {
                                WeaknessRet = true;
                                break;
                            }

                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aname, c) > 0)
                            {
                                WeaknessRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            return WeaknessRet;
        }

        // 属性 aname に対して有効属性を持つか？
        public bool Effective(string aname)
        {
            bool EffectiveRet = default;
            string c;
            int i;
            int slen;

            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strEffective, "全") > 0)
            {
                EffectiveRet = true;
                return EffectiveRet;
            }

            if (Strings.Len(aname) == 0)
            {
                if (GeneralLib.InStrNotNest(strEffective, "物") > 0)
                {
                    EffectiveRet = true;
                }

                return EffectiveRet;
            }

            i = 1;
            slen = Strings.Len(strEffective);
            while (i <= slen)
            {
                // 属性をひとまとめずつ取得
                c = GeneralLib.GetClassBundle(strEffective, ref i);
                switch (c ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aname, "魔") == 0 & GeneralLib.InStrNotNest(aname, "精") == 0)
                            {
                                EffectiveRet = true;
                                break;
                            }

                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aname, c) > 0)
                            {
                                EffectiveRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            return EffectiveRet;
        }

        // 属性 aname に対して特殊効果無効化属性を持つか？
        public bool SpecialEffectImmune(string aname)
        {
            bool SpecialEffectImmuneRet = default;
            // 全属性に有効な場合
            if (GeneralLib.InStrNotNest(strSpecialEffectImmune, "全") > 0)
            {
                SpecialEffectImmuneRet = true;
                return SpecialEffectImmuneRet;
            }

            if (Strings.Len(aname) == 0)
            {
                return SpecialEffectImmuneRet;
            }

            if (GeneralLib.InStrNotNest(strSpecialEffectImmune, aname) > 0)
            {
                SpecialEffectImmuneRet = true;
                return SpecialEffectImmuneRet;
            }

            // 無効化や弱点と違い、クリティカル率のみなので
            // 「火」に対する防御特性が「弱火」のクリティカル率に影響する点について
            // 直接関数内に記述できる。
            if (Strings.Left(aname, 1) == "弱" | Strings.Left(aname, 1) == "効")
            {
                if (GeneralLib.InStrNotNest(strSpecialEffectImmune, aname) > 0)
                {
                    SpecialEffectImmuneRet = true;
                    return SpecialEffectImmuneRet;
                }
            }

            return SpecialEffectImmuneRet;
        }

        // 属性の該当判定
        // aclass1 が防御属性、aclass2 が武器属性
        public bool IsAttributeClassified(string aclass1, string aclass2)
        {
            bool IsAttributeClassifiedRet = default;
            string attr;
            int alen, i;
            var with_not = default(bool);
            if (Strings.Len(aclass1) == 0)
            {
                IsAttributeClassifiedRet = true;
                return IsAttributeClassifiedRet;
            }

            if (aclass1 == "全")
            {
                IsAttributeClassifiedRet = true;
                return IsAttributeClassifiedRet;
            }

            // 無属性の攻撃は物理攻撃に分類される
            if (Strings.Len(aclass2) == 0)
            {
                if (GeneralLib.InStrNotNest(aclass1, "物") > 0)
                {
                    IsAttributeClassifiedRet = true;
                }

                if (GeneralLib.InStrNotNest(aclass1, "!") > 0)
                {
                    IsAttributeClassifiedRet = !IsAttributeClassifiedRet;
                }

                goto EndOfFunction;
            }

            i = 1;
            alen = Strings.Len(aclass1);
            while (i <= alen)
            {
                attr = GeneralLib.GetClassBundle(aclass1, ref i);
                switch (attr ?? "")
                {
                    case "物":
                        {
                            if (GeneralLib.InStrNotNest(aclass2, "魔") == 0 & GeneralLib.InStrNotNest(aclass2, "精") == 0)
                            {
                                IsAttributeClassifiedRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            if (GeneralLib.InStrNotNest(aclass2, "魔") > 0)
                            {
                                if (GeneralLib.InStrNotNest(aclass2, "魔武") == 0 & GeneralLib.InStrNotNest(aclass2, "魔突") == 0 & GeneralLib.InStrNotNest(aclass2, "魔接") == 0 & GeneralLib.InStrNotNest(aclass2, "魔銃") == 0 & GeneralLib.InStrNotNest(aclass2, "魔実") == 0)
                                {
                                    IsAttributeClassifiedRet = true;
                                }
                                else if (with_not)
                                {
                                    IsAttributeClassifiedRet = true;
                                }

                                break;
                            }

                            break;
                        }

                    case "!":
                        {
                            with_not = true;
                            break;
                        }

                    default:
                        {
                            if (GeneralLib.InStrNotNest(aclass2, attr) > 0)
                            {
                                IsAttributeClassifiedRet = true;
                                break;
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

        EndOfFunction:
            ;
            if (with_not)
            {
                IsAttributeClassifiedRet = !IsAttributeClassifiedRet;
            }

            return IsAttributeClassifiedRet;
        }
    }
}
