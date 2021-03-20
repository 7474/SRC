// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Pilots
{
    // === スペシャルパワー関連処理 ===
    public partial class Pilot
    {
        // === ＳＰ値関連処理 ===

        // 最大ＳＰ
        public int MaxSP
        {
            get
            {
                int MaxSPRet = default;
                int lv;

                // Impl
                //// ＳＰなしの場合はレベルに関わらず0
                //if (Data.SP <= 0)
                //{
                //    MaxSPRet = 0;
                //    // ただし追加パイロットの場合は第１パイロットの最大ＳＰを使用
                //    if (Unit is object)
                //    {
                //        {
                //            var withBlock = Unit;
                //            if (withBlock.CountPilot() > 0)
                //            {
                //                object argIndex2 = 1;
                //                object argIndex3 = 1;
                //                if (!ReferenceEquals(withBlock.Pilot(argIndex3), this))
                //                {
                //                    if (ReferenceEquals(withBlock.MainPilot(), this))
                //                    {
                //                        object argIndex1 = 1;
                //                        MaxSPRet = withBlock.Pilot(argIndex1).MaxSP;
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    return default;
                //}

                // レベルによる上昇値
                lv = Level;
                if (lv > 99)
                {
                    lv = 100;
                }

                lv = (int)(lv + SkillLevel("追加レベル", ""));
                if (lv > 40)
                {
                    MaxSPRet = lv + 40;
                }
                else
                {
                    MaxSPRet = 2 * lv;
                }

                string argsname = "ＳＰ低成長";
                string argsname1 = "ＳＰ高成長";
                if (IsSkillAvailable(argsname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }
                else if (IsSkillAvailable(argsname1))
                {
                    MaxSPRet = (int)(1.5d * MaxSPRet);
                }

                string argoname = "ＳＰ低成長";
                if (Expression.IsOptionDefined(argoname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }

                // 基本値を追加
                MaxSPRet = MaxSPRet + Data.SP;

                // 能力ＵＰ
                MaxSPRet = (int)(MaxSPRet + SkillLevel("ＳＰＵＰ", ""));

                // 能力ＤＯＷＮ
                MaxSPRet = (int)(MaxSPRet - SkillLevel("ＳＰＤＯＷＮ", ""));

                // 上限を超えないように
                if (MaxSPRet > 9999)
                {
                    MaxSPRet = 9999;
                }

                return MaxSPRet;
            }
        }

        // ＳＰ値

        public int SP
        {
            get
            {
                int SPRet = default;
                SPRet = proSP;

                // Impl
                //// 追加パイロットかどうか判定

                //if (Unit is null)
                //{
                //    return default;
                //}

                //{
                //    var withBlock = Unit;
                //    if (withBlock.CountPilot() == 0)
                //    {
                //        return default;
                //    }

                //    object argIndex1 = 1;
                //    if (ReferenceEquals(withBlock.Pilot(argIndex1), this))
                //    {
                //        return default;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return default;
                //    }

                //    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                //    if (Data.SP > 0)
                //    {
                //        // ＳＰを持つ場合は消費量を一致させる
                //        object argIndex2 = 1;
                //        {
                //            var withBlock1 = withBlock.Pilot(argIndex2);
                //            if (withBlock1.MaxSP > 0)
                //            {
                //                proSP = (MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                //                SPRet = proSP;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // ＳＰを持たない場合はそのまま使う
                //        object argIndex3 = 1;
                //        SPRet = withBlock.Pilot(argIndex3).SP0;
                //    }
                //}

                return SPRet;
            }

            set
            {
                int prev_sp;
                prev_sp = proSP;
                if (value > MaxSP)
                {
                    proSP = MaxSP;
                }
                else if (value < 0)
                {
                    proSP = 0;
                }
                else
                {
                    proSP = value;
                }

                // Impl
                //// 追加パイロットかどうか判定

                //if (Unit is null)
                //{
                //    return;
                //}

                //{
                //    var withBlock = Unit;
                //    if (withBlock.CountPilot() == 0)
                //    {
                //        return;
                //    }

                //    object argIndex1 = 1;
                //    if (ReferenceEquals(withBlock.Pilot(argIndex1), this))
                //    {
                //        return;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return;
                //    }

                //    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                //    object argIndex2 = 1;
                //    {
                //        var withBlock1 = withBlock.Pilot(argIndex2);
                //        if (Data.SP > 0)
                //        {
                //            // 追加パイロットがＳＰを持つ場合は第１パイロットと消費率を一致させる
                //            if (withBlock1.MaxSP > 0)
                //            {
                //                withBlock1.SP0 = withBlock1.MaxSP * proSP / MaxSP;
                //                proSP = (MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                //            }
                //        }
                //        // 追加パイロットがＳＰを持たない場合は第１パイロットのＳＰ値をそのまま使う
                //        else if (value > withBlock1.MaxSP)
                //        {
                //            withBlock1.SP0 = withBlock1.MaxSP;
                //        }
                //        else if (value < 0)
                //        {
                //            withBlock1.SP0 = 0;
                //        }
                //        else
                //        {
                //            withBlock1.SP0 = value;
                //        }
                //    }
                //}
            }
        }

        public int SP0
        {
            get
            {
                int SP0Ret = default;
                SP0Ret = proSP;
                return SP0Ret;
            }

            set
            {
                proSP = value;
            }
        }


        // スペシャルパワーの個数
        public int CountSpecialPower
        {
            get
            {
                int CountSpecialPowerRet = default;
                if (Data.SP <= 0)
                {
                    // Impl
                    //// ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                    //if (Unit is object)
                    //{
                    //    {
                    //        var withBlock = Unit;
                    //        if (withBlock.CountPilot() > 0)
                    //        {
                    //            object argIndex2 = 1;
                    //            object argIndex3 = 1;
                    //            if (!ReferenceEquals(withBlock.Pilot(argIndex3), this))
                    //            {
                    //                if (ReferenceEquals(withBlock.MainPilot(), this))
                    //                {
                    //                    object argIndex1 = 1;
                    //                    CountSpecialPowerRet = withBlock.Pilot(argIndex1).Data.CountSpecialPower(Level);
                    //                    return default;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

                CountSpecialPowerRet = Data.CountSpecialPower(Level);
                return CountSpecialPowerRet;
            }
        }

        // idx番目のスペシャルパワー
        public string get_SpecialPower(int idx)
        {
            string SpecialPowerRet = default;
            if (Data.SP <= 0)
            {
                // Impl
                //// ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                //if (Unit is object)
                //{
                //    {
                //        var withBlock = Unit;
                //        if (withBlock.CountPilot() > 0)
                //        {
                //            object argIndex2 = 1;
                //            object argIndex3 = 1;
                //            if (!ReferenceEquals(withBlock.Pilot(argIndex3), this))
                //            {
                //                if (ReferenceEquals(withBlock.MainPilot(), this))
                //                {
                //                    object argIndex1 = 1;
                //                    SpecialPowerRet = withBlock.Pilot(argIndex1).Data.SpecialPower(Level, idx);
                //                    return default;
                //                }
                //            }
                //        }
                //    }
                //}
            }

            SpecialPowerRet = Data.SpecialPower(Level, idx);
            return SpecialPowerRet;
        }

    }
}
