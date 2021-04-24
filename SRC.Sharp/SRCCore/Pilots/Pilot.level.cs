// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;

namespace SRCCore.Pilots
{
    // === レベル＆経験値関連処理 ===
    public partial class Pilot
    {
        // レベル
        public int Level
        {
            get => proLevel;
            set
            {
                if (proLevel == value)
                {
                    // 変化なし
                    return;
                }

                proLevel = value;
                Update();
            }
        }

        // 経験値

        public int Exp
        {
            get => proEXP;
            set
            {
                int prev_level;
                prev_level = proLevel;

                // 500ごとにレベルアップ
                proEXP = (value % 500);
                proLevel = (proLevel + value / 500);

                // 経験値が下がる場合はレベルを下げる
                if (proEXP < 0)
                {
                    if (proLevel > 1)
                    {
                        proEXP = (proEXP + 500);
                        proLevel = (proLevel - 1);
                    }
                    else
                    {
                        // これ以上はレベルを下げられないので
                        proEXP = 0;
                    }
                }

                // レベル上限チェック
                if (value / 500 > 0)
                {
                    // TODO Impl
                    //if (Expression.IsOptionDefined(ref "レベル限界突破"))
                    //{
                    //    if (proLevel > 999) // レベル999で打ち止め
                    //    {
                    //        proLevel = 999;
                    //        proEXP = 500;
                    //    }
                    //}
                    //else if (proLevel > 99) // レベル99で打ち止め
                    {
                        proLevel = 99;
                        proEXP = 500;
                    }
                }

                if (prev_level != proLevel)
                {
                    Update();
                }
            }
        }

        // 気力
        public int Morale
        {
            get => proMorale;
            set
            {
                SetMorale(value);
            }
        }

        public int MaxMorale
        {
            get
            {
                int MaxMoraleRet = 150;
                if (IsSkillAvailable("気力上限"))
                {
                    if (IsSkillLevelSpecified("気力上限"))
                    {
                        MaxMoraleRet = GeneralLib.MaxLng((int)SkillLevel("気力上限", ref_mode: ""), 0);
                    }
                }

                return MaxMoraleRet;
            }
        }

        public int MinMorale
        {
            get
            {
                int MinMoraleRet = 50;
                if (IsSkillAvailable("気力下限"))
                {
                    if (IsSkillLevelSpecified("気力下限"))
                    {
                        MinMoraleRet = (int)GeneralLib.MaxLng((int)SkillLevel("気力下限", ref_mode: ""), 0);
                    }
                }

                return MinMoraleRet;
            }
        }

        private void SetMorale(int new_morale)
        {
            var maxm = MaxMorale;
            var minm = MinMorale;
            if (new_morale > maxm)
            {
                proMorale = maxm;
            }
            else if (new_morale < minm)
            {
                proMorale = minm;
            }
            else
            {
                proMorale = new_morale;
            }
        }
    }
}
