﻿// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

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
                    //string argoname = "レベル限界突破";
                    //if (Expression.IsOptionDefined(ref argoname))
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
    }
}
