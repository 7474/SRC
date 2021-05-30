// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「修理」コマンドを開始
        private void StartFixCommand()
        {
            SelectedCommand = "修理";

            // 射程範囲？を表示
            var currentUnit = SelectedUnit;
            Map.AreaInRange(currentUnit.x, currentUnit.y, 1, 1, "味方");
            for (var i = 1; i <= Map.MapWidth; i++)
            {
                for (var j = 1; j <= Map.MapHeight; j++)
                {
                    if (!Map.MaskData[i, j] && Map.MapDataForUnit[i, j] is object)
                    {
                        var t = Map.MapDataForUnit[i, j];
                        Map.MaskData[i, j] = !t.CanFix(SelectedUnit);
                    }
                }
            }

            Map.MaskData[currentUnit.x, currentUnit.y] = false;

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                Unit t = null;
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃" && u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false && !ReferenceEquals(u, SelectedUnit))
                        {
                            if (t is null)
                            {
                                t = u;
                            }
                            else if (u.MaxHP - u.HP > t.MaxHP - t.HP)
                            {
                                t = u;
                            }
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                GUI.MoveCursorPos("ユニット選択", t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「修理」コマンドを終了
        private void FinishFixCommand()
        {
            GUI.LockGUI();
            GUI.OpenMessageForm(SelectedTarget, SelectedUnit);

            var currentUnit = SelectedUnit;
            // 選択内容を変更
            Event.SelectedUnitForEvent = SelectedUnit;
            Event.SelectedTargetForEvent = SelectedTarget;

            // 修理メッセージ＆特殊効果
            if (currentUnit.IsMessageDefined("修理"))
            {
                currentUnit.PilotMessage("修理", msg_mode: "");
            }

            if (currentUnit.IsAnimationDefined("修理", currentUnit.FeatureName("修理装置")))
            {
                currentUnit.PlayAnimation("修理", currentUnit.FeatureName("修理装置"));
            }
            else
            {
                currentUnit.SpecialEffect("修理", currentUnit.FeatureName("修理装置"));
            }

            GUI.DisplaySysMessage(currentUnit.Nickname + "は" + SelectedTarget.Nickname + "に" + currentUnit.FeatureName("修理装置") + "を使った。");

            // 修理を実行
            var tmp = SelectedTarget.HP;
            switch (currentUnit.FeatureLevel("修理装置"))
            {
                case 1d:
                case -1:
                    {
                        SelectedTarget.RecoverHP(30d + 3d * SelectedUnit.MainPilot().SkillLevel("修理", ref_mode: ""));
                        break;
                    }

                case 2d:
                    {
                        SelectedTarget.RecoverHP(50d + 5d * SelectedUnit.MainPilot().SkillLevel("修理", ref_mode: ""));
                        break;
                    }

                case 3d:
                    {
                        SelectedTarget.RecoverHP(100d);
                        break;
                    }
            }

            if (Information.IsNumeric(GeneralLib.LIndex(currentUnit.FeatureData("修理装置"), 2)))
            {
                currentUnit.EN = currentUnit.EN - Conversions.ToInteger(GeneralLib.LIndex(currentUnit.FeatureData("修理装置"), 2));
            }

            GUI.DrawSysString(SelectedTarget.x, SelectedTarget.y, "+" + SrcFormatter.Format(SelectedTarget.HP - tmp));
            GUI.UpdateMessageForm(SelectedTarget, SelectedUnit);
            GUI.DisplaySysMessage(SelectedTarget.Nickname + "の" + Expression.Term("ＨＰ", SelectedTarget) + "が" + SrcFormatter.Format(SelectedTarget.HP - tmp) + "回復した。");

            // 経験値獲得
            currentUnit.GetExp(SelectedTarget, "修理", exp_mode: "");
            if (GUI.MessageWait < 10000)
            {
                GUI.Sleep(GUI.MessageWait);
            }

            GUI.CloseMessageForm();

            // 形態変化のチェック
            SelectedTarget.Update();
            SelectedTarget.CurrentForm().CheckAutoHyperMode();
            SelectedTarget.CurrentForm().CheckAutoNormalMode();

            // 行動終了
            WaitCommand();
        }

        // 「補給」コマンドを開始
        private void StartSupplyCommand()
        {
            SelectedCommand = "補給";

            // 射程範囲？を表示
            var currentUnit = SelectedUnit;
            Map.AreaInRange(currentUnit.x, currentUnit.y, 1, 1, "味方");
            for (var i = 1; i <= Map.MapWidth; i++)
            {
                for (var j = 1; j <= Map.MapHeight; j++)
                {
                    if (!Map.MaskData[i, j] && Map.MapDataForUnit[i, j] is object)
                    {
                        var t = Map.MapDataForUnit[i, j];
                        Map.MaskData[i, j] = t.CanSupply;
                    }
                }
            }

            Map.MaskData[currentUnit.x, currentUnit.y] = false;

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                Unit t = null;
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃" && u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false && !ReferenceEquals(u, SelectedUnit))
                        {
                            t = u;
                            break;
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                GUI.MoveCursorPos("ユニット選択", t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「補給」コマンドを終了
        private void FinishSupplyCommand()
        {
            throw new NotImplementedException();

            //GUI.LockGUI();
            //GUI.OpenMessageForm(SelectedTarget, SelectedUnit);
            //{
            //    var withBlock = SelectedUnit;
            //    // 選択内容を変更
            //    Event.SelectedUnitForEvent = SelectedUnit;
            //    Event.SelectedTargetForEvent = SelectedTarget;

            //    // 補給メッセージ＆特殊効果
            //    if (withBlock.IsMessageDefined("補給"))
            //    {
            //        withBlock.PilotMessage("補給", msg_mode: "");
            //    }

            //    if (withBlock.IsAnimationDefined("補給", withBlock.FeatureName(argIndex3)))
            //    {
            //        withBlock.PlayAnimation("補給", withBlock.FeatureName("補給装置"));
            //    }
            //    else
            //    {
            //        withBlock.SpecialEffect("補給", withBlock.FeatureName(argIndex2));
            //    }

            //    GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName("補給装置") + "を使った。");

            //    // 補給を実施
            //    SelectedTarget.FullSupply();
            //    SelectedTarget.IncreaseMorale(-10);
            //    string localLIndex2() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //    if (Information.IsNumeric(localLIndex2()))
            //    {
            //        string localLIndex() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        string localLIndex1() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        withBlock.EN = withBlock.EN - Conversions.Toint(localLIndex1());
            //    }

            //    GUI.UpdateMessageForm(SelectedTarget, SelectedUnit);
            //    GUI.DisplaySysMessage(SelectedTarget.Nickname + "の弾数と" + Expression.Term("ＥＮ", SelectedTarget) + "が全快した。");

            //    // 経験値を獲得
            //    withBlock.GetExp(SelectedTarget, "補給", exp_mode: "");
            //    if (GUI.MessageWait < 10000)
            //    {
            //        GUI.Sleep(GUI.MessageWait);
            //    }
            //}

            //// 形態変化のチェック
            //SelectedTarget.Update();
            //SelectedTarget.CurrentForm().CheckAutoHyperMode();
            //SelectedTarget.CurrentForm().CheckAutoNormalMode();
            //GUI.CloseMessageForm();

            //// 行動終了
            //WaitCommand();
        }
    }
}
