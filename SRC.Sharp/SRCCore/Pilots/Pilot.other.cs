// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Pilots
{
    // === その他 ===
    public partial class Pilot
    {
        // 全回復
        public void FullRecover()
        {
            // 闘争本能によって初期気力は変化する
            if (IsSkillAvailable("闘争本能"))
            {
                if (MinMorale > 100)
                {
                    SetMorale((int)(MinMorale + 5d * SkillLevel("闘争本能", "")));
                }
                else
                {
                    SetMorale((int)(100d + 5d * SkillLevel("闘争本能", "")));
                }
            }
            else
            {
                SetMorale(100);
            }

            SP = MaxSP;
            Plana = MaxPlana();
        }

        //// 同調率
        //public int SynchroRate()
        //{
        //    int SynchroRateRet = default;
        //    int lv;
        //    string argsname = "同調率";
        //    if (!IsSkillAvailable(argsname))
        //    {
        //        return SynchroRateRet;
        //    }

        //    // 同調率基本値
        //    object argIndex1 = "同調率";
        //    string argref_mode = "";
        //    SynchroRateRet = SkillLevel(argIndex1, ref_mode: argref_mode);

        //    // レベルによる増加分
        //    lv = GeneralLib.MinLng(Level, 100);
        //    string argsname1 = "同調率成長";
        //    if (IsSkillAvailable(argsname1))
        //    {
        //        object argIndex2 = "同調率成長";
        //        string argref_mode1 = "";
        //        SynchroRateRet = (SynchroRateRet + (long)(lv * (10d + SkillLevel(argIndex2, ref_mode: argref_mode1))) / 10L);
        //    }
        //    else
        //    {
        //        SynchroRateRet = (SynchroRateRet + lv);
        //    }

        //    return SynchroRateRet;
        //}

        //// 指揮範囲
        //public int CommandRange()
        //{
        //    int CommandRangeRet = default;
        //    // 指揮能力を持っていなければ範囲は0
        //    string argsname = "指揮";
        //    if (!IsSkillAvailable(argsname))
        //    {
        //        CommandRangeRet = 0;
        //        return CommandRangeRet;
        //    }

        //    // 指揮能力を持っている場合は階級レベルに依存
        //    object argIndex1 = "階級";
        //    string argref_mode = "";
        //    switch (SkillLevel(argIndex1, ref_mode: argref_mode))
        //    {
        //        case var @case when 0d <= @case && @case <= 6d:
        //            {
        //                CommandRangeRet = 2;
        //                break;
        //            }

        //        case var case1 when 7d <= case1 && case1 <= 9d:
        //            {
        //                CommandRangeRet = 3;
        //                break;
        //            }

        //        case var case2 when 10d <= case2 && case2 <= 12d:
        //            {
        //                CommandRangeRet = 4;
        //                break;
        //            }

        //        default:
        //            {
        //                CommandRangeRet = 5;
        //                break;
        //            }
        //    }

        //    return CommandRangeRet;
        //}

        // 行動決定に用いられる戦闘判断力
        public int TacticalTechnique0()
        {
            return (int)(TechniqueBase - Level + 10d * SkillLevel("戦術", ref_mode: ""));
        }

        public int TacticalTechnique()
        {
            int TacticalTechniqueRet = default;
            // 正常な判断能力がある？
            if (Unit is object)
            {
                if (Unit.IsConditionSatisfied("混乱")
                    || Unit.IsConditionSatisfied("暴走")
                    || Unit.IsConditionSatisfied("狂戦士"))
                {
                    return 0;
                }
            }

            TacticalTechniqueRet = TacticalTechnique0();
            return TacticalTechniqueRet;
        }

        //// イベントコマンド SetRelation で設定した値を返す
        //public int Relation(Pilot t)
        //{
        //    int RelationRet = default;
        //    string argexpr = "関係:" + Name + ":" + t.Name;
        //    RelationRet = Expression.GetValueAsLong(argexpr);
        //    return RelationRet;
        //}

        // 射撃能力が「魔力」と表示されるかどうか
        public bool HasMana()
        {
            if (IsSkillAvailable("術") | IsSkillAvailable("魔力所有"))
            {
                return true;
            }
            if (Expression.IsOptionDefined("魔力使用"))
            {
                return true;
            }
            return false;
        }
    }
}
