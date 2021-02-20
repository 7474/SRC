using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === 行動パターンを規定するパラメータ関連処理 ===
    public partial class Unit
    {
        // 陣営
        public string Party0 => strParty;

        public string Party
        {
            get
            {
                string PartyRet = strParty;

                // TODO Impl
                //// 魅了されている場合
                //object argIndex1 = "魅了";
                //if (IsConditionSatisfied(ref argIndex1) & Master is object)
                //{
                //    PartyRet = Master.Party;
                //    if (PartyRet == "味方")
                //    {
                //        // 味方になっても自分では操作できない
                //        PartyRet = "ＮＰＣ";
                //    }
                //}

                //// 憑依されている場合
                //object argIndex2 = "憑依";
                //if (IsConditionSatisfied(ref argIndex2) & Master is object)
                //{
                //    PartyRet = Master.Party;
                //}

                //// コントロール不能の味方ユニットはＮＰＣとして扱う
                //if (PartyRet == "味方")
                //{
                //    object argIndex3 = "暴走";
                //    object argIndex4 = "混乱";
                //    object argIndex5 = "恐怖";
                //    object argIndex6 = "踊り";
                //    object argIndex7 = "狂戦士";
                //    if (IsConditionSatisfied(ref argIndex3) | IsConditionSatisfied(ref argIndex4) | IsConditionSatisfied(ref argIndex5) | IsConditionSatisfied(ref argIndex6) | IsConditionSatisfied(ref argIndex7))
                //    {
                //        PartyRet = "ＮＰＣ";
                //    }
                //}

                return PartyRet;
            }

            set
            {
                strParty = value;
            }
        }

        //// 思考モード

        //public string Mode
        //{
        //    get
        //    {
        //        string ModeRet = default;
        //        short i;
        //        string argsptype = "挑発";
        //        if (IsUnderSpecialPowerEffect(ref argsptype))
        //        {
        //            // 挑発を最優先
        //            var loopTo = CountSpecialPower();
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                // UPGRADE_WARNING: オブジェクト SpecialPower(i).IsEffectAvailable(挑発) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                SpecialPowerData localSpecialPower() { object argIndex1 = i; var ret = SpecialPower(ref argIndex1); return ret; }

        //                string argename = "挑発";
        //                if (Conversions.ToBoolean(localSpecialPower().IsEffectAvailable(ref argename)))
        //                {
        //                    object argIndex1 = i;
        //                    ModeRet = SpecialPowerData(ref argIndex1);
        //                    return default;
        //                }
        //            }
        //        }

        //        object argIndex2 = "暴走";
        //        object argIndex3 = "混乱";
        //        object argIndex4 = "憑依";
        //        object argIndex5 = "狂戦士";
        //        if (IsConditionSatisfied(ref argIndex2) | IsConditionSatisfied(ref argIndex3) | IsConditionSatisfied(ref argIndex4) | IsConditionSatisfied(ref argIndex5))
        //        {
        //            // 正常な判断が出来ない場合は当初の目的を忘れてしまうため
        //            // 常に通常モードとして扱う
        //            ModeRet = "通常";
        //            return default;
        //        }

        //        object argIndex6 = "恐怖";
        //        if (IsConditionSatisfied(ref argIndex6))
        //        {
        //            // 恐怖にかられた場合は逃亡
        //            ModeRet = "逃亡";
        //            return default;
        //        }

        //        object argIndex7 = "踊り";
        //        if (IsConditionSatisfied(ref argIndex7))
        //        {
        //            // 踊るのに忙しい……
        //            ModeRet = "固定";
        //            return default;
        //        }

        //        ModeRet = strMode;
        //        return ModeRet;
        //    }

        //    set
        //    {
        //        strMode = value;
        //    }
        //}

        //// 地形 area_name での移動が可能かどうか
        //public bool IsTransAvailable(ref string area_name)
        //{
        //    bool IsTransAvailableRet = default;
        //    // 移動可能地形に含まれているか？
        //    if (Strings.InStr(Transportation, area_name) > 0)
        //    {
        //        IsTransAvailableRet = true;
        //    }
        //    else
        //    {
        //        IsTransAvailableRet = false;
        //    }

        //    // 水上移動の場合
        //    if (area_name == "水上")
        //    {
        //        string argfname = "水上移動";
        //        string argfname1 = "ホバー移動";
        //        if (IsFeatureAvailable(ref argfname) | IsFeatureAvailable(ref argfname1))
        //        {
        //            IsTransAvailableRet = true;
        //        }
        //    }

        //    return IsTransAvailableRet;
        //}
    }
}
