using SRCCore.VB;

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
            //if (IsConditionSatisfied("行動不能") | IsConditionSatisfied("麻痺") | IsConditionSatisfied("石化") | IsConditionSatisfied("凍結") | IsConditionSatisfied("睡眠") | IsConditionSatisfied("チャージ") | IsConditionSatisfied("消耗") | IsUnderSpecialPowerEffect("行動不能"))
            //{
            //    return MaxActionRet;
            //}

            //// ＥＮ切れ？
            //if (!ignore_en)
            //{
            //    if (EN == 0)
            //    {
            //        if (!Expression.IsOptionDefined("ＥＮ０時行動可"))
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
            //if (Expression.IsOptionDefined("２回行動能力使用"))
            //{
            //    if (MainPilot().IsSkillAvailable("２回行動"))
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
            //if (CountPilot() == 0)
            //{
            //    max_action = 1;
            //}
            //else if (Expression.IsOptionDefined("２回行動能力使用"))
            //{
            //    if (MainPilot().IsSkillAvailable("２回行動"))
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

                // TODO Impl 魅了されている場合
                //// 魅了されている場合
                //if (IsConditionSatisfied("魅了") & Master is object)
                //{
                //    PartyRet = Master.Party;
                //    if (PartyRet == "味方")
                //    {
                //        // 味方になっても自分では操作できない
                //        PartyRet = "ＮＰＣ";
                //    }
                //}

                //// 憑依されている場合
                //if (IsConditionSatisfied("憑依") & Master is object)
                //{
                //    PartyRet = Master.Party;
                //}

                //// コントロール不能の味方ユニットはＮＰＣとして扱う
                //if (PartyRet == "味方")
                //{
                //    if (IsConditionSatisfied("暴走") | IsConditionSatisfied("混乱") | IsConditionSatisfied("恐怖") | IsConditionSatisfied("踊り") | IsConditionSatisfied("狂戦士"))
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
                // TODO Impl Mode
                //string ModeRet = default;
                //int i;
                //if (IsUnderSpecialPowerEffect("挑発"))
                //{
                //    // 挑発を最優先
                //    var loopTo = CountSpecialPower();
                //    for (i = 1; i <= loopTo; i++)
                //    {
                //        // UPGRADE_WARNING: オブジェクト SpecialPower(i).IsEffectAvailable(挑発) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        SpecialPowerData localSpecialPower() { object argIndex1 = i; var ret = SpecialPower(argIndex1); return ret; }

                //        if (Conversions.ToBoolean(localSpecialPower().IsEffectAvailable("挑発")))
                //        {
                //            ModeRet = SpecialPowerData(i);
                //            return default;
                //        }
                //    }
                //}

                //if (IsConditionSatisfied("暴走") | IsConditionSatisfied("混乱") | IsConditionSatisfied("憑依") | IsConditionSatisfied("狂戦士"))
                //{
                //    // 正常な判断が出来ない場合は当初の目的を忘れてしまうため
                //    // 常に通常モードとして扱う
                //    ModeRet = "通常";
                //    return default;
                //}

                //if (IsConditionSatisfied("恐怖"))
                //{
                //    // 恐怖にかられた場合は逃亡
                //    ModeRet = "逃亡";
                //    return default;
                //}

                //if (IsConditionSatisfied("踊り"))
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

        // 地形 area_name での移動が可能かどうか
        public bool IsTransAvailable(string area_name)
        {
            bool IsTransAvailableRet = default;
            // 移動可能地形に含まれているか？
            if (Strings.InStr(Transportation, area_name) > 0)
            {
                IsTransAvailableRet = true;
            }
            else
            {
                IsTransAvailableRet = false;
            }

            // 水上移動の場合
            if (area_name == "水上")
            {
                if (IsFeatureAvailable("水上移動") || IsFeatureAvailable("ホバー移動"))
                {
                    IsTransAvailableRet = true;
                }
            }

            return IsTransAvailableRet;
        }
    }
}
