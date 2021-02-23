using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === 行動パターンを規定するパラメータ関連処理 ===
    public partial class Unit
    {
        public bool CanAction => (Status == "出撃" || Status == "格納") && this.Action > 0;

        // === 行動数関連処理 ===

        // １ターンに可能な行動数
        public int MaxAction(bool ignore_en = false)
        {
            int MaxActionRet;
            //// ステータス異常？
            //object argIndex1 = "行動不能";
            //object argIndex2 = "麻痺";
            //object argIndex3 = "石化";
            //object argIndex4 = "凍結";
            //object argIndex5 = "睡眠";
            //object argIndex6 = "チャージ";
            //object argIndex7 = "消耗";
            //string argsptype = "行動不能";
            //if (IsConditionSatisfied(ref argIndex1) | IsConditionSatisfied(ref argIndex2) | IsConditionSatisfied(ref argIndex3) | IsConditionSatisfied(ref argIndex4) | IsConditionSatisfied(ref argIndex5) | IsConditionSatisfied(ref argIndex6) | IsConditionSatisfied(ref argIndex7) | IsUnderSpecialPowerEffect(ref argsptype))
            //{
            //    return MaxActionRet;
            //}

            //// ＥＮ切れ？
            //if (!ignore_en)
            //{
            //    if (EN == 0)
            //    {
            //        string argoname = "ＥＮ０時行動可";
            //        if (!Expression.IsOptionDefined(ref argoname))
            //        {
            //            return MaxActionRet;
            //        }
            //    }
            //}

            //if (CountPilot() == 0)
            //{
            //    return MaxActionRet;
            //}

            //// ２回行動可能？
            //string argoname1 = "２回行動能力使用";
            //if (Expression.IsOptionDefined(ref argoname1))
            //{
            //    string argsname = "２回行動";
            //    if (MainPilot().IsSkillAvailable(ref argsname))
            //    {
            //        MaxActionRet = 2;
            //    }
            //    else
            //    {
            //        MaxActionRet = 1;
            //    }
            //}
            //else
            if (MainPilot().Intuition >= 200)
            {
                MaxActionRet = 2;
            }
            else
            {
                MaxActionRet = 1;
            }

            return MaxActionRet;
        }

        // 行動数を消費
        public void UseAction()
        {
            UsedAction = UsedAction + 1;
            // TODO これ厳密にみる必要あるの？　貫通していると再動何かに影響がある？
            //int max_action;

            //// ２回行動可能？
            //string argoname = "２回行動能力使用";
            //if (CountPilot() == 0)
            //{
            //    max_action = 1;
            //}
            //else if (Expression.IsOptionDefined(ref argoname))
            //{
            //    string argsname = "２回行動";
            //    if (MainPilot().IsSkillAvailable(ref argsname))
            //    {
            //        max_action = 2;
            //    }
            //    else
            //    {
            //        max_action = 1;
            //    }
            //}
            //else if (this.MainPilot().Intuition >= 200)
            //{
            //    max_action = 2;
            //}
            //else
            //{
            //    max_action = 1;
            //}

            //// 最大行動数まで行動消費量をカウント
            //UsedAction = (int)GeneralLib.MinLng(UsedAction + 1, max_action);
        }

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
        public string Mode
        {
            get
            {
                return strMode;
                // TODO Impl
                //string ModeRet = default;
                //int i;
                //string argsptype = "挑発";
                //if (IsUnderSpecialPowerEffect(ref argsptype))
                //{
                //    // 挑発を最優先
                //    var loopTo = CountSpecialPower();
                //    for (i = 1; i <= loopTo; i++)
                //    {
                //        // UPGRADE_WARNING: オブジェクト SpecialPower(i).IsEffectAvailable(挑発) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        SpecialPowerData localSpecialPower() { object argIndex1 = i; var ret = SpecialPower(ref argIndex1); return ret; }

                //        string argename = "挑発";
                //        if (Conversions.ToBoolean(localSpecialPower().IsEffectAvailable(ref argename)))
                //        {
                //            object argIndex1 = i;
                //            ModeRet = SpecialPowerData(ref argIndex1);
                //            return default;
                //        }
                //    }
                //}

                //object argIndex2 = "暴走";
                //object argIndex3 = "混乱";
                //object argIndex4 = "憑依";
                //object argIndex5 = "狂戦士";
                //if (IsConditionSatisfied(ref argIndex2) | IsConditionSatisfied(ref argIndex3) | IsConditionSatisfied(ref argIndex4) | IsConditionSatisfied(ref argIndex5))
                //{
                //    // 正常な判断が出来ない場合は当初の目的を忘れてしまうため
                //    // 常に通常モードとして扱う
                //    ModeRet = "通常";
                //    return default;
                //}

                //object argIndex6 = "恐怖";
                //if (IsConditionSatisfied(ref argIndex6))
                //{
                //    // 恐怖にかられた場合は逃亡
                //    ModeRet = "逃亡";
                //    return default;
                //}

                //object argIndex7 = "踊り";
                //if (IsConditionSatisfied(ref argIndex7))
                //{
                //    // 踊るのに忙しい……
                //    ModeRet = "固定";
                //    return default;
                //}

                //ModeRet = strMode;
                //return ModeRet;
            }

            set
            {
                strMode = value;
            }
        }

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
