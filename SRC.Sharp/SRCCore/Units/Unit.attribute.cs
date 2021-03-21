// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strAbsorb, argstring2) > 0)
            {
                AbsorbRet = true;
                return AbsorbRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                string argstring21 = "物";
                if (GeneralLib.InStrNotNest(strAbsorb, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aname, argstring22) == 0 & GeneralLib.InStrNotNest(aname, argstring23) == 0)
                            {
                                AbsorbRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            string argstring29 = "魔";
                            if (GeneralLib.InStrNotNest(aname, argstring29) > 0)
                            {
                                string argstring24 = "魔武";
                                string argstring25 = "魔突";
                                string argstring26 = "魔接";
                                string argstring27 = "魔銃";
                                string argstring28 = "魔実";
                                if (GeneralLib.InStrNotNest(aname, argstring24) == 0 & GeneralLib.InStrNotNest(aname, argstring25) == 0 & GeneralLib.InStrNotNest(aname, argstring26) == 0 & GeneralLib.InStrNotNest(aname, argstring27) == 0 & GeneralLib.InStrNotNest(aname, argstring28) == 0)
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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strImmune, argstring2) > 0)
            {
                ImmuneRet = true;
                return ImmuneRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                string argstring21 = "物";
                if (GeneralLib.InStrNotNest(strImmune, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aname, argstring22) == 0 & GeneralLib.InStrNotNest(aname, argstring23) == 0)
                            {
                                ImmuneRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            string argstring24 = "魔";
                            string argstring25 = "魔武";
                            string argstring26 = "魔突";
                            string argstring27 = "魔接";
                            string argstring28 = "魔銃";
                            string argstring29 = "魔実";
                            if (GeneralLib.InStrNotNest(aname, argstring24) > 0 & GeneralLib.InStrNotNest(aname, argstring25) == 0 & GeneralLib.InStrNotNest(aname, argstring26) == 0 & GeneralLib.InStrNotNest(aname, argstring27) == 0 & GeneralLib.InStrNotNest(aname, argstring28) == 0 & GeneralLib.InStrNotNest(aname, argstring29) == 0)
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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strResist, argstring2) > 0)
            {
                ResistRet = true;
                return ResistRet;
            }

            // 無属性は物理攻撃に分類される
            if (Strings.Len(aname) == 0)
            {
                string argstring21 = "物";
                if (GeneralLib.InStrNotNest(strResist, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aname, argstring22) == 0 & GeneralLib.InStrNotNest(aname, argstring23) == 0)
                            {
                                ResistRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            string argstring24 = "魔";
                            string argstring25 = "魔武";
                            string argstring26 = "魔突";
                            string argstring27 = "魔接";
                            string argstring28 = "魔銃";
                            string argstring29 = "魔実";
                            if (GeneralLib.InStrNotNest(aname, argstring24) > 0 & GeneralLib.InStrNotNest(aname, argstring25) == 0 & GeneralLib.InStrNotNest(aname, argstring26) == 0 & GeneralLib.InStrNotNest(aname, argstring27) == 0 & GeneralLib.InStrNotNest(aname, argstring28) == 0 & GeneralLib.InStrNotNest(aname, argstring29) == 0)
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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strWeakness, argstring2) > 0)
            {
                WeaknessRet = true;
                return WeaknessRet;
            }

            if (Strings.Len(aname) == 0)
            {
                string argstring21 = "物";
                if (GeneralLib.InStrNotNest(strWeakness, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aname, argstring22) == 0 & GeneralLib.InStrNotNest(aname, argstring23) == 0)
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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strEffective, argstring2) > 0)
            {
                EffectiveRet = true;
                return EffectiveRet;
            }

            if (Strings.Len(aname) == 0)
            {
                string argstring21 = "物";
                if (GeneralLib.InStrNotNest(strEffective, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aname, argstring22) == 0 & GeneralLib.InStrNotNest(aname, argstring23) == 0)
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
            string argstring2 = "全";
            if (GeneralLib.InStrNotNest(strSpecialEffectImmune, argstring2) > 0)
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
                string argstring2 = "物";
                if (GeneralLib.InStrNotNest(aclass1, argstring2) > 0)
                {
                    IsAttributeClassifiedRet = true;
                }

                string argstring21 = "!";
                if (GeneralLib.InStrNotNest(aclass1, argstring21) > 0)
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
                            string argstring22 = "魔";
                            string argstring23 = "精";
                            if (GeneralLib.InStrNotNest(aclass2, argstring22) == 0 & GeneralLib.InStrNotNest(aclass2, argstring23) == 0)
                            {
                                IsAttributeClassifiedRet = true;
                                break;
                            }

                            break;
                        }

                    case "魔":
                        {
                            // 魔法武器以外の魔属性なら特性が有効
                            string argstring29 = "魔";
                            if (GeneralLib.InStrNotNest(aclass2, argstring29) > 0)
                            {
                                string argstring24 = "魔武";
                                string argstring25 = "魔突";
                                string argstring26 = "魔接";
                                string argstring27 = "魔銃";
                                string argstring28 = "魔実";
                                if (GeneralLib.InStrNotNest(aclass2, argstring24) == 0 & GeneralLib.InStrNotNest(aclass2, argstring25) == 0 & GeneralLib.InStrNotNest(aclass2, argstring26) == 0 & GeneralLib.InStrNotNest(aclass2, argstring27) == 0 & GeneralLib.InStrNotNest(aclass2, argstring28) == 0)
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
