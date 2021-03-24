using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === 特殊状態関連処理 ===
    public partial class Unit
    {
        // 特殊状態を付加
        public void AddCondition(string cname, int ltime, double clevel = Constants.DEFAULT_LEVEL, string cdata = "")
        {
            var new_condition = new Condition();

            // 同じ特殊状態が既に付加されている？
            foreach (Condition cnd in colCondition.List)
            {
                if ((cnd.Name ?? "") == (cname ?? ""))
                {
                    if (cnd.Lifetime < 0 | ltime < 0)
                    {
                        cnd.Lifetime = -1;
                    }
                    else
                    {
                        cnd.Lifetime = GeneralLib.MaxLng(cnd.Lifetime, ltime);
                    }

                    cnd.Name = cname;
                    cnd.Level = clevel;
                    cnd.StrData = cdata;
                    return;
                }
            }

            // 特殊状態を付加
            new_condition.Name = cname;
            new_condition.Lifetime = ltime;
            new_condition.Level = clevel;
            new_condition.StrData = cdata;
            colCondition.Add(new_condition, cname);
        }

        // 特殊状態を削除
        public void DeleteCondition(string Index)
        {
            {
                var withBlock = colCondition[Index];
                colCondition.Remove(Index);

                // 特殊能力付加の場合はユニットのステータスをアップデート
                if (Strings.Right(Conversions.ToString(withBlock.Name), 2) == "付加"
                    && Strings.InStr(Conversions.ToString(withBlock.StrData), "パイロット能力付加") == 0)
                {
                    Update();
                }
            }
        }

        public void DeleteCondition0(string Index)
        {
            colCondition.Remove(Index);
        }

        // 付加された特殊状態の総数
        public int CountCondition()
        {
            return colCondition.Count;
        }

        // 特殊状態
        public string Condition(string Index)
        {
            return colCondition[Index].Name;
        }

        // 特殊状態の残りターン数
        public int ConditionLifetime(string Index)
        {
            return colCondition[Index]?.Lifetime ?? -1;
        }

        // 指定した特殊能力が付加されているか？
        public bool IsConditionSatisfied(string Index)
        {
            return ConditionLifetime(Index) >= 0;
        }

        //// 特殊状態のレベル
        //public double ConditionLevel(string Index)
        //{
        //    double ConditionLevelRet = default;
        //    ;
        //    // UPGRADE_WARNING: オブジェクト colCondition.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    ConditionLevelRet = Conversions.ToDouble(colCondition[Index].Level);
        //    return ConditionLevelRet;
        //ErrorHandler:
        //    ;
        //    ConditionLevelRet = 0d;
        //}

        //// 特殊状態のレベルの変更
        //public void SetConditionLevel(string Index, double lv)
        //{
        //    ;
        //    // UPGRADE_WARNING: オブジェクト colCondition.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    colCondition[Index].Level = lv;
        //ErrorHandler:
        //    ;
        //}

        //// 特殊能力のデータ
        //public string ConditionData(string Index)
        //{
        //    string ConditionDataRet = default;
        //    ;
        //    // UPGRADE_WARNING: オブジェクト colCondition.Item().StrData の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    ConditionDataRet = Conversions.ToString(colCondition[Index].StrData);
        //    return ConditionDataRet;
        //ErrorHandler:
        //    ;
        //    ConditionDataRet = "";
        //}

        // 特殊能力の残りターン数を更新
        public void UpdateCondition(bool decrement_lifetime = false)
        {
            // TODO Impl
            //    var update_is_necessary = default(bool);
            //    var charge_complete = default(bool);
            //    foreach (Condition cnd in colCondition)
            //    {
            //        if (decrement_lifetime)
            //        {
            //            // 残りターン数を1減らす
            //            if (cnd.Lifetime > 0)
            //            {
            //                cnd.Lifetime = (cnd.Lifetime - 1);
            //            }
            //        }

            //        if (cnd.Lifetime == 0)
            //        {
            //            // 残りターン数が0なら削除
            //            colCondition.Remove(cnd.Name);
            //            switch (cnd.Name ?? "")
            //            {
            //                case "魅了":
            //                    {
            //                        // 魅了を解除
            //                        if (Master is object)
            //                        {
            //                            object argIndex1 = ID;
            //                            Master.CurrentForm().DeleteSlave(ref argIndex1);
            //                            // UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                            Master = null;
            //                        }

            //                        Mode = "通常";
            //                        break;
            //                    }

            //                case "チャージ":
            //                    {
            //                        // チャージ完了
            //                        charge_complete = true;
            //                        break;
            //                    }

            //                case "活動限界":
            //                    {
            //                        // 活動限界時間切れ
            //                        GUI.Center(x, y);
            //                        Escape();
            //                        Unit argu1 = null;
            //                        Unit argu2 = null;
            //                        GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            //                        GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                        GUI.CloseMessageForm();
            //                        Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        // 特殊能力付加を解除
            //                        if (Strings.Right(cnd.Name, 2) == "付加" | Strings.Right(cnd.Name, 2) == "強化")
            //                        {
            //                            update_is_necessary = true;
            //                        }

            //                        break;
            //                    }
            //            }
            //        }
            //    }

            //    // チャージ状態が終了したらチャージ完了状態にする
            //    if (charge_complete)
            //    {
            //        string argcname = "チャージ完了";
            //        string argcdata = "";
            //        AddCondition(ref argcname, 1, cdata: ref argcdata);
            //    }

            //    // ユニットのステータス変化あり？
            //    if (update_is_necessary)
            //    {
            //        Update();
            //    }
        }
    }
}
