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
    // === 霊力関連処理 ===
    public partial class Pilot
    {
        // 霊力

        public int Plana
        {
            get
            {
                int PlanaRet = default;
                if (IsSkillAvailable("霊力"))
                {
                    PlanaRet = proPlana;
                }

                // TODO Impl
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

                //    if (ReferenceEquals(withBlock.Pilot(1), this))
                //    {
                //        return default;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return default;
                //    }

                //    // 追加パイロットだったので第１パイロットの霊力を代わりに使う
                //    if (IsSkillAvailable("霊力"))
                //    {
                //        {
                //            var withBlock1 = withBlock.Pilot(1);
                //            if (withBlock1.MaxPlana() > 0)
                //            {
                //                proPlana = (MaxPlana() * withBlock1.Plana0 / withBlock1.MaxPlana());
                //                PlanaRet = proPlana;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        PlanaRet = withBlock.Pilot(1).Plana0;
                //    }
                //}

                return PlanaRet;
            }

            set
            {
                int prev_plana;
                prev_plana = proPlana;
                if (value > MaxPlana())
                {
                    proPlana = MaxPlana();
                }
                else if (value < 0)
                {
                    proPlana = 0;
                }
                else
                {
                    proPlana = value;
                }

                // TODO Impl
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

                //    if (ReferenceEquals(withBlock.Pilot(1), this))
                //    {
                //        return;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return;
                //    }

                //    // 追加パイロットだったので第１パイロットの霊力値を代わりに使う
                //    {
                //        var withBlock1 = withBlock.Pilot(1);
                //        if (IsSkillAvailable("霊力"))
                //        {
                //            // 追加パイロットが霊力を持つ場合は第１パイロットと消費率を一致させる
                //            if (withBlock1.MaxSP > 0)
                //            {
                //                withBlock1.Plana0 = withBlock1.MaxPlana() * proPlana / MaxPlana();
                //                proPlana = (MaxPlana() * withBlock1.Plana0 / withBlock1.MaxPlana());
                //            }
                //        }
                //        // 追加パイロットが霊力を持たない場合は第１パイロットの霊力値をそのまま使う
                //        else if (value > withBlock1.MaxPlana())
                //        {
                //            withBlock1.Plana0 = withBlock1.MaxPlana();
                //        }
                //        else if (value < 0)
                //        {
                //            withBlock1.Plana0 = 0;
                //        }
                //        else
                //        {
                //            withBlock1.Plana0 = value;
                //        }
                //    }
                //}
            }
        }

        public int Plana0
        {
            get => proPlana;
            set
            {
                proPlana = value;
            }
        }


        // 霊力最大値
        public int MaxPlana()
        {
            int MaxPlanaRet;
            int lv;
            if (!IsSkillAvailable("霊力"))
            {
                // 霊力能力を持たない場合
                MaxPlanaRet = 0;

                // TODO Impl
                // XXX ぼちぼち追加パイロット厳しくなってきた
                //// 追加パイロットの場合は第１パイロットの霊力を代わりに使う
                //if (Unit is object)
                //{
                //    {
                //        var withBlock = Unit;
                //        if (withBlock.CountPilot() > 0)
                //        {
                //            if (!ReferenceEquals(withBlock.Pilot(1), this))
                //            {
                //                if (ReferenceEquals(withBlock.MainPilot(), this))
                //                {
                //                    MaxPlanaRet = withBlock.Pilot(1).MaxPlana();
                //                }
                //            }
                //        }
                //    }
                //}

                return MaxPlanaRet;
            }

            // 霊力基本値
            MaxPlanaRet = (int)SkillLevel("霊力", "");

            // レベルによる増加分
            lv = GeneralLib.MinLng(Level, 100);
            if (IsSkillAvailable("霊力成長"))
            {
                MaxPlanaRet = (int)(MaxPlanaRet + (long)(1.5d * lv * (10d + SkillLevel("霊力成長", ""))) / 10L);
            }
            else
            {
                MaxPlanaRet = (int)(MaxPlanaRet + 1.5d * lv);
            }

            return MaxPlanaRet;
        }
    }
}
