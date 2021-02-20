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
            //// 闘争本能によって初期気力は変化する
            //string argsname = "闘争本能";
            //if (IsSkillAvailable(ref argsname))
            //{
            //    if (MinMorale > 100)
            //    {
            //        object argIndex1 = "闘争本能";
            //        string argref_mode = "";
            //        SetMorale((short)(MinMorale + 5d * SkillLevel(ref argIndex1, ref_mode: ref argref_mode)));
            //    }
            //    else
            //    {
            //        object argIndex2 = "闘争本能";
            //        string argref_mode1 = "";
            //        SetMorale((short)(100d + 5d * SkillLevel(ref argIndex2, ref_mode: ref argref_mode1)));
            //    }
            //}
            //else
            //{
            //    SetMorale(100);
            //}

            //SP = MaxSP;
            //Plana = MaxPlana();
        }

        //// 同調率
        //public short SynchroRate()
        //{
        //    short SynchroRateRet = default;
        //    short lv;
        //    string argsname = "同調率";
        //    if (!IsSkillAvailable(ref argsname))
        //    {
        //        return SynchroRateRet;
        //    }

        //    // 同調率基本値
        //    object argIndex1 = "同調率";
        //    string argref_mode = "";
        //    SynchroRateRet = (short)SkillLevel(ref argIndex1, ref_mode: ref argref_mode);

        //    // レベルによる増加分
        //    lv = (short)GeneralLib.MinLng(Level, 100);
        //    string argsname1 = "同調率成長";
        //    if (IsSkillAvailable(ref argsname1))
        //    {
        //        object argIndex2 = "同調率成長";
        //        string argref_mode1 = "";
        //        SynchroRateRet = (short)(SynchroRateRet + (long)(lv * (10d + SkillLevel(ref argIndex2, ref_mode: ref argref_mode1))) / 10L);
        //    }
        //    else
        //    {
        //        SynchroRateRet = (short)(SynchroRateRet + lv);
        //    }

        //    return SynchroRateRet;
        //}

        //// 指揮範囲
        //public short CommandRange()
        //{
        //    short CommandRangeRet = default;
        //    // 指揮能力を持っていなければ範囲は0
        //    string argsname = "指揮";
        //    if (!IsSkillAvailable(ref argsname))
        //    {
        //        CommandRangeRet = 0;
        //        return CommandRangeRet;
        //    }

        //    // 指揮能力を持っている場合は階級レベルに依存
        //    object argIndex1 = "階級";
        //    string argref_mode = "";
        //    switch (SkillLevel(ref argIndex1, ref_mode: ref argref_mode))
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

        //// 行動決定に用いられる戦闘判断力
        //public short TacticalTechnique0()
        //{
        //    short TacticalTechnique0Ret = default;
        //    object argIndex1 = "戦術";
        //    string argref_mode = "";
        //    TacticalTechnique0Ret = (short)(TechniqueBase - Level + 10d * SkillLevel(ref argIndex1, ref_mode: ref argref_mode));
        //    return TacticalTechnique0Ret;
        //}

        //public short TacticalTechnique()
        //{
        //    short TacticalTechniqueRet = default;
        //    // 正常な判断能力がある？
        //    if (Unit_Renamed is object)
        //    {
        //        {
        //            var withBlock = Unit_Renamed;
        //            object argIndex1 = "混乱";
        //            object argIndex2 = "暴走";
        //            object argIndex3 = "狂戦士";
        //            if (withBlock.IsConditionSatisfied(ref argIndex1) | withBlock.IsConditionSatisfied(ref argIndex2) | withBlock.IsConditionSatisfied(ref argIndex3))
        //            {
        //                return TacticalTechniqueRet;
        //            }
        //        }
        //    }

        //    TacticalTechniqueRet = TacticalTechnique0();
        //    return TacticalTechniqueRet;
        //}

        //// イベントコマンド SetRelation で設定した値を返す
        //public short Relation(ref Pilot t)
        //{
        //    short RelationRet = default;
        //    string argexpr = "関係:" + Name + ":" + t.Name;
        //    RelationRet = (short)Expression.GetValueAsLong(ref argexpr);
        //    return RelationRet;
        //}

        //// 射撃能力が「魔力」と表示されるかどうか
        //public bool HasMana()
        //{
        //    bool HasManaRet = default;
        //    string argsname = "術";
        //    string argsname1 = "魔力所有";
        //    if (IsSkillAvailable(ref argsname) | IsSkillAvailable(ref argsname1))
        //    {
        //        HasManaRet = true;
        //        return HasManaRet;
        //    }

        //    string argoname = "魔力使用";
        //    if (Expression.IsOptionDefined(ref argoname))
        //    {
        //        HasManaRet = true;
        //        return HasManaRet;
        //    }

        //    HasManaRet = false;
        //    return HasManaRet;
        //}
    }
}
