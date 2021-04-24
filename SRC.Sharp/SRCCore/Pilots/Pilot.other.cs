// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;

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
        //    if (!IsSkillAvailable("同調率"))
        //    {
        //        return SynchroRateRet;
        //    }

        //    // 同調率基本値
        //    SynchroRateRet = SkillLevel("同調率", ref_mode: "");

        //    // レベルによる増加分
        //    lv = GeneralLib.MinLng(Level, 100);
        //    if (IsSkillAvailable("同調率成長"))
        //    {
        //        SynchroRateRet = (SynchroRateRet + (long)(lv * (10d + SkillLevel("同調率成長", ref_mode: ""))) / 10L);
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
        //    if (!IsSkillAvailable("指揮"))
        //    {
        //        CommandRangeRet = 0;
        //        return CommandRangeRet;
        //    }

        //    // 指揮能力を持っている場合は階級レベルに依存
        //    switch (SkillLevel("階級", ref_mode: ""))
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
        //    RelationRet = Expression.GetValueAsLong("関係:" + Name + ":" + t.Name);
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
