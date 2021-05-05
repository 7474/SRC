// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「修理」コマンドを開始
        private void StartFixCommand()
        {
            throw new NotImplementedException();
            //int j, i, k;
            //Unit t;
            //string fname;
            //SelectedCommand = "修理";

            //// 射程範囲？を表示
            //{
            //    var withBlock = SelectedUnit;
            //    Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, "味方");
            //    var loopTo = Map.MapWidth;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        var loopTo1 = Map.MapHeight;
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
            //            {
            //                {
            //                    var withBlock1 = Map.MapDataForUnit[i, j];
            //                    if (withBlock1.HP == withBlock1.MaxHP | withBlock1.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        Map.MaskData[i, j] = true;
            //                    }

            //                    if (withBlock1.IsFeatureAvailable("修理不可"))
            //                    {
            //                        var loopTo2 = Conversions.ToInteger(withBlock1.FeatureData("修理不可"));
            //                        for (k = 2; k <= loopTo2; k++)
            //                        {
            //                            fname = GeneralLib.LIndex(withBlock1.FeatureData(argIndex2), k);
            //                            if (Strings.Left(fname, 1) == "!")
            //                            {
            //                                fname = Strings.Mid(fname, 2);
            //                                if ((fname ?? "") != (SelectedUnit.FeatureName0("修理装置") ?? ""))
            //                                {
            //                                    Map.MaskData[i, j] = true;
            //                                    break;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if ((fname ?? "") == (SelectedUnit.FeatureName0("修理装置") ?? ""))
            //                                {
            //                                    Map.MaskData[i, j] = true;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    Map.MaskData[withBlock.x, withBlock.y] = false;
            //}

            //GUI.MaskScreen();

            //// カーソル自動移動
            //if (SRC.AutoMoveCursor)
            //{
            //    // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    t = null;
            //    foreach (Unit u in SRC.UList)
            //    {
            //        if (u.Status == "出撃" & u.Party == "味方")
            //        {
            //            if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
            //            {
            //                if (t is null)
            //                {
            //                    t = u;
            //                }
            //                else if (u.MaxHP - u.HP > t.MaxHP - t.HP)
            //                {
            //                    t = u;
            //                }
            //            }
            //        }
            //    }

            //    if (t is null)
            //    {
            //        t = SelectedUnit;
            //    }

            //    GUI.MoveCursorPos("ユニット選択", t);
            //    if (!ReferenceEquals(SelectedUnit, t))
            //    {
            //        Status.DisplayUnitStatus(t);
            //    }
            //}

            //if (CommandState == "コマンド選択")
            //{
            //    CommandState = "ターゲット選択";
            //}
            //else
            //{
            //    CommandState = "移動後ターゲット選択";
            //}
        }

        // 「修理」コマンドを終了
        private void FinishFixCommand()
        {
            throw new NotImplementedException();
            //int tmp;
            //GUI.LockGUI();
            //GUI.OpenMessageForm(SelectedTarget, SelectedUnit);
            //{
            //    var withBlock = SelectedUnit;
            //    // 選択内容を変更
            //    Event.SelectedUnitForEvent = SelectedUnit;
            //    Event.SelectedTargetForEvent = SelectedTarget;

            //    // 修理メッセージ＆特殊効果
            //    if (withBlock.IsMessageDefined("修理"))
            //    {
            //        withBlock.PilotMessage("修理", msg_mode: "");
            //    }

            //    if (withBlock.IsAnimationDefined("修理", withBlock.FeatureName(argIndex3)))
            //    {
            //        withBlock.PlayAnimation("修理", withBlock.FeatureName(argIndex1));
            //    }
            //    else
            //    {
            //        withBlock.SpecialEffect("修理", withBlock.FeatureName(argIndex2));
            //    }

            //    GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName("修理装置") + "を使った。");

            //    // 修理を実行
            //    tmp = SelectedTarget.HP;
            //    switch (withBlock.FeatureLevel("修理装置"))
            //    {
            //        case 1d:
            //        case -1:
            //            {
            //                SelectedTarget.RecoverHP(30d + 3d * SelectedUnit.MainPilot().SkillLevel("修理", ref_mode: ""));
            //                break;
            //            }

            //        case 2d:
            //            {
            //                SelectedTarget.RecoverHP(50d + 5d * SelectedUnit.MainPilot().SkillLevel("修理", ref_mode: ""));
            //                break;
            //            }

            //        case 3d:
            //            {
            //                SelectedTarget.RecoverHP(100d);
            //                break;
            //            }
            //    }

            //    string localLIndex2() { object "修理装置" = "修理装置"; string arglist = withBlock.FeatureData("修理装置"); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //    if (Information.IsNumeric(localLIndex2()))
            //    {
            //        string localLIndex() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        string localLIndex1() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        withBlock.EN = withBlock.EN - Conversions.Toint(localLIndex1());
            //    }

            //    GUI.DrawSysString(SelectedTarget.x, SelectedTarget.y, "+" + SrcFormatter.Format(SelectedTarget.HP - tmp));
            //    GUI.UpdateMessageForm(SelectedTarget, SelectedUnit);
            //    GUI.DisplaySysMessage(SelectedTarget.Nickname + "の" + Expression.Term("ＨＰ", SelectedTarget) + "が" + SrcFormatter.Format(SelectedTarget.HP - tmp) + "回復した。");

            //    // 経験値獲得
            //    withBlock.GetExp(SelectedTarget, "修理", exp_mode: "");
            //    if (GUI.MessageWait < 10000)
            //    {
            //        GUI.Sleep(GUI.MessageWait);
            //    }
            //}

            //GUI.CloseMessageForm();

            //// 形態変化のチェック
            //SelectedTarget.Update();
            //SelectedTarget.CurrentForm().CheckAutoHyperMode();
            //SelectedTarget.CurrentForm().CheckAutoNormalMode();

            //// 行動終了
            //WaitCommand();
        }

        // 「補給」コマンドを開始
        private void StartSupplyCommand()
        {
            throw new NotImplementedException();
            //int j, i, k;
            //Unit t;
            //SelectedCommand = "補給";

            //// 射程範囲？を表示
            //{
            //    var withBlock = SelectedUnit;
            //    Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, "味方");
            //    var loopTo = Map.MapWidth;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        var loopTo1 = Map.MapHeight;
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
            //            {
            //                Map.MaskData[i, j] = true;
            //                {
            //                    var withBlock1 = Map.MapDataForUnit[i, j];
            //                    if (withBlock1.EN < withBlock1.MaxEN & !withBlock1.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        Map.MaskData[i, j] = false;
            //                    }
            //                    else
            //                    {
            //                        var loopTo2 = withBlock1.CountWeapon();
            //                        for (k = 1; k <= loopTo2; k++)
            //                        {
            //                            if (withBlock1.Bullet(k) < withBlock1.MaxBullet(k))
            //                            {
            //                                Map.MaskData[i, j] = false;
            //                                break;
            //                            }
            //                        }

            //                        var loopTo3 = withBlock1.CountAbility();
            //                        for (k = 1; k <= loopTo3; k++)
            //                        {
            //                            if (withBlock1.Stock(k) < withBlock1.MaxStock(k))
            //                            {
            //                                Map.MaskData[i, j] = false;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    Map.MaskData[withBlock.x, withBlock.y] = false;
            //}

            //GUI.MaskScreen();

            //// カーソル自動移動
            //if (SRC.AutoMoveCursor)
            //{
            //    // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    t = null;
            //    foreach (Unit u in SRC.UList)
            //    {
            //        if (u.Status == "出撃" & u.Party == "味方")
            //        {
            //            if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
            //            {
            //                t = u;
            //                break;
            //            }
            //        }
            //    }

            //    if (t is null)
            //    {
            //        t = SelectedUnit;
            //    }

            //    GUI.MoveCursorPos("ユニット選択", t);
            //    if (!ReferenceEquals(SelectedUnit, t))
            //    {
            //        Status.DisplayUnitStatus(t);
            //    }
            //}

            //if (CommandState == "コマンド選択")
            //{
            //    CommandState = "ターゲット選択";
            //}
            //else
            //{
            //    CommandState = "移動後ターゲット選択";
            //}
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
