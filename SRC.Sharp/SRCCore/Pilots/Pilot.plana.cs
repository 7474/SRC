// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
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

                // 追加パイロットかどうか判定
                if (IsMainAdditionalPilot)
                {
                    // 追加パイロットだったので第１パイロットの霊力を代わりに使う
                    if (IsSkillAvailable("霊力"))
                    {
                        {
                            var p = Unit.Pilots.First();
                            if (p.MaxPlana() > 0)
                            {
                                proPlana = (MaxPlana() * p.Plana0 / p.MaxPlana());
                                PlanaRet = proPlana;
                            }
                        }
                    }
                    else
                    {
                        PlanaRet = Unit.Pilots.First().Plana0;
                    }
                }

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

                // 追加パイロットかどうか判定
                if (IsMainAdditionalPilot)
                // 追加パイロットだったので第１パイロットの霊力値を代わりに使う
                {
                    var p = Unit.Pilots.First();
                    if (IsSkillAvailable("霊力"))
                    {
                        // 追加パイロットが霊力を持つ場合は第１パイロットと消費率を一致させる
                        if (p.MaxSP > 0)
                        {
                            p.Plana0 = p.MaxPlana() * proPlana / MaxPlana();
                            proPlana = (MaxPlana() * p.Plana0 / p.MaxPlana());
                        }
                    }
                    // 追加パイロットが霊力を持たない場合は第１パイロットの霊力値をそのまま使う
                    else if (value > p.MaxPlana())
                    {
                        p.Plana0 = p.MaxPlana();
                    }
                    else if (value < 0)
                    {
                        p.Plana0 = 0;
                    }
                    else
                    {
                        p.Plana0 = value;
                    }
                }
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

                // 追加パイロットの場合は第１パイロットの霊力を代わりに使う
                if (IsMainAdditionalPilot)
                {
                    MaxPlanaRet = Unit.Pilots.First().MaxPlana();
                }

                return MaxPlanaRet;
            }

            // 霊力基本値
            MaxPlanaRet = VBMath.CLng(SkillLevel("霊力", ""));

            // レベルによる増加分
            lv = GeneralLib.MinLng(Level, 100);
            if (IsSkillAvailable("霊力成長"))
            {
                // VB: MaxPlana + 1.5 * lv * (10 + SkillLevel("霊力成長")) \ 10
                // \ の被除数は偶数丸めで整数にしてから割る
                MaxPlanaRet = (int)(MaxPlanaRet + VBMath.IntDiv(1.5d * lv * (10d + SkillLevel("霊力成長", "")), 10L));
            }
            else
            {
                // VB: MaxPlana + 1.5 * lv を Long へ代入するので偶数丸め
                MaxPlanaRet = VBMath.CLng(MaxPlanaRet + 1.5d * lv);
            }

            return MaxPlanaRet;
        }
    }
}
