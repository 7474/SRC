// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「変形」コマンド
        private void TransformCommand()
        {
            // MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            var fdata = SelectedUnit.FeatureData("変形");
            if (Map.IsStatusView)
            {
                // TODO Impl
                //// ユニットステータスコマンドの場合
                //{
                //    var withBlock = SelectedUnit;
                //    list = new string[1];
                //    list_id = new string[1];
                //    // 変形可能な形態一覧を作成
                //    var loopTo = GeneralLib.LLength(fdata);
                //    for (i = 2; i <= loopTo; i++)
                //    {
                //        {
                //            var withBlock1 = withBlock.OtherForm(GeneralLib.LIndex(fdata, i));
                //            if (withBlock1.IsAvailable())
                //            {
                //                Array.Resize(list, Information.UBound(list) + 1 + 1);
                //                Array.Resize(list_id, Information.UBound(list) + 1);
                //                list[Information.UBound(list)] = withBlock1.Nickname;
                //                list_id[Information.UBound(list)] = withBlock1.Name;
                //            }
                //        }
                //    }

                //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                //    // 変形する形態を選択
                //    if (Information.UBound(list) > 1)
                //    {
                //        GUI.TopItem = 1;
                //        ret = GUI.ListBox("変形", list, "名前", "カーソル移動");
                //        if (ret == 0)
                //        {
                //            CancelCommand();
                //            GUI.UnlockGUI();
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        ret = 1;
                //    }

                //    // 変形を実施
                //    Unit localOtherForm() { var tmp = list_id; object argIndex1 = tmp[ret]; var ret = withBlock.OtherForm(argIndex1); return ret; }

                //    withBlock.Transform(localOtherForm().Name);
                //    localOtherForm().Name = argnew_form;

                //    // ユニットリストの表示を更新
                //    Event.MakeUnitList(smode: "");

                //    // ステータスウィンドウの表示を更新
                //    Status.DisplayUnitStatus(withBlock.CurrentForm());

                //    // コマンドを終了
                //    GUI.UnlockGUI();
                //    CommandState = "ユニット選択";
                //    return;
                //}
            }

            // 変形可能な形態の一覧を作成
            var forms = GeneralLib.ToL(fdata).Skip(1)
                .Select(x => SelectedUnit.OtherForm(x))
                .Where(x => x.IsAvailable())
                .Select(x => new ListBoxItem
                {
                    ListItemID = x.Name,
                    Text = x.Nickname0,
                    ListItemFlag = !(x.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) || Map.IsStatusView)
                }).ToList();
            // 変形先の形態を選択
            int ret;
            if (forms.Count() == 1)
            {
                if (forms.First().ListItemFlag)
                {
                    GUI.Confirm("この地形では" + GeneralLib.LIndex(fdata, 1) + "できません", "", GuiConfirmOption.Ok);
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                ret = 1;
            }
            else
            {
                GUI.TopItem = 1;
                ret = GUI.ListBox(new ListBoxArgs
                {
                    lb_caption = SelectedUnit.IsHero() ? "変身先" : "変形先",
                    lb_info = "名前",
                    lb_mode = "カーソル移動",
                    HasFlag = true,
                    Items = forms,
                });

                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }
            }

            var uname = forms[ret - 1].ListItemID;
            var targetUnit = SRC.UDList.Item(uname);
            // TODO Impl
            string BGM;
            {
                var u = SelectedUnit;
                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                AddAdditionalPilotIfNotExist(uname, u);

                //// ＢＧＭの変更
                //if (u.IsFeatureAvailable("変形ＢＧＭ"))
                //{
                //    var loopTo2 = u.CountFeature();
                //    for (i = 1; i <= loopTo2; i++)
                //    {
                //        string localFeature() { object argIndex1 = i; var ret = u.Feature(argIndex1); return ret; }

                //        string localFeatureData2() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

                //        string localLIndex() { string arglist = hs6c18ebb7075745309751cd168b7bf5f0(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                //        if (localFeature() == "変形ＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
                //        {
                //            string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

                //            string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

                //            BGM = Sound.SearchMidiFile(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
                //            if (Strings.Len(BGM) > 0)
                //            {
                //                Sound.ChangeBGM(BGM);
                //                GUI.Sleep(500);
                //            }

                //            break;
                //        }
                //    }
                //}

                // メッセージを表示
                if (u.IsMessageDefined("変形(" + u.Name + "=>" + uname + ")")
                    || u.IsMessageDefined("変形(" + uname + ")")
                    || u.IsMessageDefined("変形(" + u.FeatureName("変形") + ")"))
                {
                    GUI.Center(u.x, u.y);
                    GUI.RefreshScreen();
                    GUI.OpenMessageForm(u1: null, u2: null);

                    if (u.IsMessageDefined("変形(" + u.Name + "=>" + uname + ")"))
                    {
                        u.PilotMessage("変形(" + u.Name + "=>" + uname + ")", msg_mode: "");
                    }
                    else if (u.IsMessageDefined("変形(" + uname + ")"))
                    {
                        u.PilotMessage("変形(" + uname + ")", msg_mode: "");
                    }
                    else if (u.IsMessageDefined("変形(" + u.FeatureName("変形") + ")"))
                    {
                        u.PilotMessage("変形(" + u.FeatureName("変形") + ")", msg_mode: "");
                    }

                    GUI.CloseMessageForm();
                }

                //// アニメ表示
                //bool localIsAnimationDefined() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = u.IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                //bool localIsAnimationDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + u.FeatureName(argIndex1) + ")"; string argsub_situation = ""; var ret = u.IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                //bool localIsSpecialEffectDefined() { string argmain_situation = "変形(" + u.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                //bool localIsSpecialEffectDefined1() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                //bool localIsSpecialEffectDefined2() { object argIndex1 = "変形"; string argmain_situation = "変形(" + u.FeatureName(argIndex1) + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                //if (u.IsAnimationDefined("変形(" + u.Name + "=>" + uname + ")", sub_situation: ""))
                //{
                //    u.PlayAnimation("変形(" + u.Name + "=>" + uname + ")", sub_situation: "");
                //}
                //else if (localIsAnimationDefined())
                //{
                //    u.PlayAnimation("変形(" + uname + ")", sub_situation: "");
                //}
                //else if (localIsAnimationDefined1())
                //{
                //    u.PlayAnimation("変形(" + u.FeatureName("変形") + ")", sub_situation: "");
                //}
                //else if (localIsSpecialEffectDefined())
                //{
                //    u.SpecialEffect("変形(" + u.Name + "=>" + uname + ")", sub_situation: "");
                //}
                //else if (localIsSpecialEffectDefined1())
                //{
                //    u.SpecialEffect("変形(" + uname + ")", sub_situation: "");
                //}
                //else if (localIsSpecialEffectDefined2())
                //{
                //    u.SpecialEffect("変形(" + u.FeatureName("変形") + ")", sub_situation: "");
                //}
            }

            // 変形
            var prev_uname = SelectedUnit.Name;
            SelectedUnit.Transform(uname);
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形をキャンセルする？
            if (SelectedUnit.Action == 0)
            {
                var confirmRet = GUI.Confirm(
                    "この形態ではこれ以上の行動が出来ません。" + Constants.vbCr + Constants.vbLf + "それでも変形しますか？",
                    "変形",
                    GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                if (confirmRet == GuiDialogResult.Cancel)
                {
                    SelectedUnit.Transform(prev_uname);
                    SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];
                    if (SelectedUnit.IsConditionSatisfied("消耗"))
                    {
                        SelectedUnit.DeleteCondition("消耗");
                    }
                }

                GUI.RedrawScreen();
            }

            // 変形イベント
            {
                Event.HandleEvent("変形", SelectedUnit.CurrentForm().MainPilot().ID, SelectedUnit.CurrentForm().Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();

            // カーソル自動移動
            if (SelectedUnit.Status == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("ユニット選択", SelectedUnit);
                }

                Status.DisplayUnitStatus(SelectedUnit);
            }

            // XXX RedrawScreen 元はしてなかった気がする
            GUI.RedrawScreen();
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        private void AddAdditionalPilotIfNotExist(string uname, Units.Unit u)
        {
            var ud = SRC.UDList.Item(uname);
            if (ud.IsFeatureAvailable("追加パイロット"))
            {
                var fd = ud.Feature("追加パイロット");
                if (!SRC.PList.IsDefined(fd.Data))
                {
                    if (!SRC.PDList.IsDefined(fd.Data))
                    {
                        GUI.ErrorMessage(uname + "の追加パイロット「" + fd.Data + "」のデータが見つかりません");
                        SRC.TerminateSRC();
                    }

                    SRC.PList.Add(fd.Data, u.MainPilot().Level, u.Party0, gid: "");
                }
            }
        }

        // 「ハイパーモード」コマンド
        private void HyperModeCommand()
        {
            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            var uname = GeneralLib.LIndex(SelectedUnit.FeatureData("ハイパーモード"), 2);
            var fname = SelectedUnit.FeatureName("ハイパーモード");
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合
                {
                    var withBlock = SelectedUnit;
                    if (!withBlock.IsFeatureAvailable("ハイパーモード"))
                    {
                        uname = GeneralLib.LIndex(SelectedUnit.FeatureData("ノーマルモード"), 1);
                    }

                    // ハイパーモードを発動
                    withBlock.Transform(uname);

                    // ユニットリストの表示を更新
                    Event.MakeUnitList(smode: "");

                    // ステータスウィンドウの表示を更新
                    Status.DisplayUnitStatus(withBlock.CurrentForm());

                    // コマンドを終了
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }
            }

            // ハイパーモードを発動可能かどうかチェック
            {
                var withBlock1 = SelectedUnit.OtherForm(uname);
                if (!withBlock1.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) & !string.IsNullOrEmpty(Map.MapFileName))
                {
                    GUI.Confirm("この地形では変形できません", "", GuiConfirmOption.Ok);
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }
            }

            var u = SelectedUnit;
            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            AddAdditionalPilotIfNotExist(uname, u);

            //string BGM;
            {
                var currentUnit = SelectedUnit;
                // ＢＧＭを変更
                if (currentUnit.IsFeatureAvailable("ハイパーモードＢＧＭ"))
                {
                    var loopTo11 = currentUnit.CountFeature();
                    for (var i = 1; i <= loopTo11; i++)
                    {
                        var fdata = currentUnit.Feature(i).Data;
                        if (currentUnit.Feature(i).Name == "ハイパーモードＢＧＭ"
                            && (GeneralLib.LIndex(fdata, 1) ?? "") == uname)
                        {
                            var BGM = Sound.SearchMidiFile(Strings.Mid(fdata, Strings.InStr(fdata, " ") + 1));
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(BGM);
                                GUI.Sleep(500);
                            }
                            break;
                        }
                    }
                }

                // メッセージを表示
                if (u.IsMessageDefined("ハイパーモード(" + u.Name + "=>" + uname + ")")
                    || u.IsMessageDefined("ハイパーモード(" + uname + ")")
                    || u.IsMessageDefined("ハイパーモード(" + fname + ")"))
                {
                    GUI.Center(u.x, u.y);
                    GUI.RefreshScreen();
                    GUI.OpenMessageForm(u1: null, u2: null);

                    if (u.IsMessageDefined("ハイパーモード(" + u.Name + "=>" + uname + ")"))
                    {
                        u.PilotMessage("ハイパーモード(" + u.Name + "=>" + uname + ")", msg_mode: "");
                    }
                    else if (u.IsMessageDefined("ハイパーモード(" + uname + ")"))
                    {
                        u.PilotMessage("ハイパーモード(" + uname + ")", msg_mode: "");
                    }
                    else if (u.IsMessageDefined("ハイパーモード(" + fname + ")"))
                    {
                        u.PilotMessage("ハイパーモード(" + fname + ")", msg_mode: "");
                    }

                    GUI.CloseMessageForm();
                }

                // アニメ表示
                if (currentUnit.IsAnimationDefined("ハイパーモード(" + currentUnit.Name + "=>" + uname + ")", sub_situation: ""))
                {
                    currentUnit.PlayAnimation("ハイパーモード(" + currentUnit.Name + "=>" + uname + ")", sub_situation: "");
                }
                else if (currentUnit.IsAnimationDefined("ハイパーモード(" + uname + ")"))
                {
                    currentUnit.PlayAnimation("ハイパーモード(" + uname + ")", sub_situation: "");
                }
                else if (currentUnit.IsAnimationDefined("ハイパーモード(" + fname + ")"))
                {
                    currentUnit.PlayAnimation("ハイパーモード(" + fname + ")", sub_situation: "");
                }
                else if (currentUnit.IsAnimationDefined("ハイパーモード", sub_situation: ""))
                {
                    currentUnit.PlayAnimation("ハイパーモード", sub_situation: "");
                }
                else if (currentUnit.IsSpecialEffectDefined("ハイパーモード(" + currentUnit.Name + "=>" + uname + ")"))
                {
                    currentUnit.SpecialEffect("ハイパーモード(" + currentUnit.Name + "=>" + uname + ")", sub_situation: "");
                }
                else if (currentUnit.IsSpecialEffectDefined("ハイパーモード(" + uname + ")"))
                {
                    currentUnit.SpecialEffect("ハイパーモード(" + uname + ")", sub_situation: "");
                }
                else if (currentUnit.IsSpecialEffectDefined("ハイパーモード(" + fname + ")"))
                {
                    currentUnit.SpecialEffect("ハイパーモード(" + fname + ")", sub_situation: "");
                }
                else
                {
                    currentUnit.SpecialEffect("ハイパーモード", sub_situation: "");
                }
            }

            // ハイパーモード発動
            SelectedUnit.Transform(uname);

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形イベント
            {
                var withBlock4 = SelectedUnit.CurrentForm();
                Event.HandleEvent("変形", withBlock4.MainPilot().ID, withBlock4.Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SelectedUnit.Status == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("ユニット選択", SelectedUnit);
                }

                Status.DisplayUnitStatus(SelectedUnit);
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「変身解除」コマンド
        private void CancelTransformationCommand()
        {
            throw new NotImplementedException();
            //int ret;

            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            //GUI.LockGUI();
            //{
            //    var withBlock = SelectedUnit;
            //    if (string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        // ユニットステータスコマンドの場合
            //        string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //        withBlock.Transform(localLIndex());
            //        Event.MakeUnitList(smode: "");
            //        Status.DisplayUnitStatus(withBlock.CurrentForm());
            //        GUI.UnlockGUI();
            //        CommandState = "ユニット選択";
            //        return;
            //    }

            //    if (withBlock.IsHero())
            //    {
            //        ret = Interaction.MsgBox("変身を解除しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "変身解除");
            //    }
            //    else
            //    {
            //        ret = Interaction.MsgBox("特殊モードを解除しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "特殊モード解除");
            //    }

            //    if (ret == MsgBoxResult.Cancel)
            //    {
            //        GUI.UnlockGUI();
            //        CancelCommand();
            //        return;
            //    }

            //    string localLIndex1() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //    withBlock.Transform(localLIndex1());
            //    SelectedUnit = Map.MapDataForUnit[withBlock.x, withBlock.y];
            //}

            //// カーソル自動移動
            //if (SRC.AutoMoveCursor)
            //{
            //    GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            //}

            //Status.DisplayUnitStatus(SelectedUnit);
            //GUI.RedrawScreen();

            //// 変形イベント
            //Event.HandleEvent("変形", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
            //SRC.IsScenarioFinished = false;
            //SRC.IsCanceled = false;
            //CommandState = "ユニット選択";
            //GUI.UnlockGUI();
        }

        // 「分離」コマンド
        private void SplitCommand()
        {
            string uname, tname, fname;
            //int ret;
            //string BGM;

            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            if (Map.IsStatusView)
            {
                // ユニットステータスコマンドの場合

                // 分離を実施
                {
                    var u = SelectedUnit;
                    if (u.IsFeatureAvailable("パーツ分離"))
                    {
                        tname = GeneralLib.LIndex(u.FeatureData("パーツ分離"), 2);
                        u.Transform(tname);
                    }
                    else
                    {
                        u.Split();
                    }

                    SRC.UList.CheckAutoHyperMode();
                    SRC.UList.CheckAutoNormalMode();
                    Status.DisplayUnitStatus(Map.MapDataForUnit[u.x, u.y]);
                }

                // TODO Impl
                //// ユニットリストの表示を更新
                //Event.MakeUnitList(smode: "");

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            {
                var u = SelectedUnit;
                if (u.IsFeatureAvailable("パーツ分離"))
                {
                    // パーツ分離を行う場合
                    var confirmRes = GUI.Confirm("パーツを分離しますか？", "パーツ分離", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                    if (confirmRes == GuiDialogResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    tname = GeneralLib.LIndex(u.FeatureData("パーツ分離"), 2);

                    if (!u.OtherForm(tname).IsAbleToEnter(u.x, u.y))
                    {
                        GUI.Confirm("この地形では分離できません", "分離", GuiConfirmOption.Ok);
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // TODO Impl
                    //// ＢＧＭ変更
                    //if (u.IsFeatureAvailable("分離ＢＧＭ"))
                    //{
                    //    BGM = Sound.SearchMidiFile(u.FeatureData("分離ＢＧＭ"));
                    //    if (Strings.Len(BGM) > 0)
                    //    {
                    //        Sound.StartBGM(u.FeatureData("分離ＢＧＭ"));
                    //        GUI.Sleep(500);
                    //    }
                    //}

                    fname = u.FeatureName("パーツ分離");

                    //// メッセージを表示
                    if (u.IsMessageDefined("分離(" + u.Name + ")")
                        || u.IsMessageDefined("分離(" + fname + ")")
                        || u.IsMessageDefined("分離"))
                    {
                        GUI.Center(u.x, u.y);
                        GUI.RefreshScreen();
                        GUI.OpenMessageForm(u1: null, u2: null);

                        if (u.IsMessageDefined("分離(" + u.Name + ")"))
                        {
                            u.PilotMessage("分離(" + u.Name + ")", msg_mode: "");
                        }
                        else if (u.IsMessageDefined("分離(" + fname + ")"))
                        {
                            u.PilotMessage("分離(" + fname + ")", msg_mode: "");
                        }
                        else if (u.IsMessageDefined("分離"))
                        {
                            u.PilotMessage("分離", msg_mode: "");
                        }

                        GUI.CloseMessageForm();
                    }

                    //// アニメ表示
                    //bool localIsAnimationDefined() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = u.IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //bool localIsSpecialEffectDefined() { string argmain_situation = "分離(" + u.Name + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //bool localIsSpecialEffectDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //if (u.IsAnimationDefined("分離(" + u.Name + ")", sub_situation: ""))
                    //{
                    //    u.PlayAnimation("分離(" + u.Name + ")", sub_situation: "");
                    //}
                    //else if (localIsAnimationDefined())
                    //{
                    //    u.PlayAnimation("分離(" + fname + ")", sub_situation: "");
                    //}
                    //else if (u.IsAnimationDefined("分離", sub_situation: ""))
                    //{
                    //    u.PlayAnimation("分離", sub_situation: "");
                    //}
                    //else if (localIsSpecialEffectDefined())
                    //{
                    //    u.SpecialEffect("分離(" + u.Name + ")", sub_situation: "");
                    //}
                    //else if (localIsSpecialEffectDefined1())
                    //{
                    //    u.SpecialEffect("分離(" + fname + ")", sub_situation: "");
                    //}
                    //else
                    //{
                    //    u.SpecialEffect("分離", sub_situation: "");
                    //}

                    // パーツ分離
                    uname = u.Name;
                    u.Transform(tname);
                    SelectedUnit = Map.MapDataForUnit[u.x, u.y];
                    Status.DisplayUnitStatus(SelectedUnit);
                }
                else
                {
                    // 通常の分離を行う場合

                    var confirmRes = GUI.Confirm("分離しますか？", "分離", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                    if (confirmRes == GuiDialogResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // TODO Impl
                    //// ＢＧＭを変更
                    //if (u.IsFeatureAvailable("分離ＢＧＭ"))
                    //{
                    //    BGM = Sound.SearchMidiFile(u.FeatureData("分離ＢＧＭ"));
                    //    if (Strings.Len(BGM) > 0)
                    //    {
                    //        Sound.StartBGM(u.FeatureData("分離ＢＧＭ"));
                    //        GUI.Sleep(500);
                    //    }
                    //}

                    //fname = u.FeatureName("分離");

                    //// メッセージを表示
                    //bool localIsMessageDefined4() { string argmain_situation = "分離(" + u.Name + ")"; var ret = u.IsMessageDefined(argmain_situation); return ret; }

                    //bool localIsMessageDefined5() { string argmain_situation = "分離(" + fname + ")"; var ret = u.IsMessageDefined(argmain_situation); return ret; }

                    //if (localIsMessageDefined4() || localIsMessageDefined5() || u.IsMessageDefined("分離"))
                    //{
                    //    GUI.Center(u.x, u.y);
                    //    GUI.RefreshScreen();
                    //    GUI.OpenMessageForm(u1: null, u2: null);
                    //    bool localIsMessageDefined3() { string argmain_situation = "分離(" + fname + ")"; var ret = u.IsMessageDefined(argmain_situation); return ret; }

                    //    if (u.IsMessageDefined("分離(" + u.Name + ")"))
                    //    {
                    //        u.PilotMessage("分離(" + u.Name + ")", msg_mode: "");
                    //    }
                    //    else if (localIsMessageDefined3())
                    //    {
                    //        u.PilotMessage("分離(" + fname + ")", msg_mode: "");
                    //    }
                    //    else
                    //    {
                    //        u.PilotMessage("分離", msg_mode: "");
                    //    }

                    //    GUI.CloseMessageForm();
                    //}

                    //// アニメ表示
                    //bool localIsAnimationDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = u.IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //bool localIsSpecialEffectDefined2() { string argmain_situation = "分離(" + u.Name + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //bool localIsSpecialEffectDefined3() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                    //if (u.IsAnimationDefined("分離(" + u.Name + ")", sub_situation: ""))
                    //{
                    //    u.PlayAnimation("分離(" + u.Name + ")", sub_situation: "");
                    //}
                    //else if (localIsAnimationDefined1())
                    //{
                    //    u.PlayAnimation("分離(" + fname + ")", sub_situation: "");
                    //}
                    //else if (u.IsAnimationDefined("分離", sub_situation: ""))
                    //{
                    //    u.PlayAnimation("分離", sub_situation: "");
                    //}
                    //else if (localIsSpecialEffectDefined2())
                    //{
                    //    u.SpecialEffect("分離(" + u.Name + ")", sub_situation: "");
                    //}
                    //else if (localIsSpecialEffectDefined3())
                    //{
                    //    u.SpecialEffect("分離(" + fname + ")", sub_situation: "");
                    //}
                    //else
                    //{
                    //    u.SpecialEffect("分離", sub_situation: "");
                    //}

                    // 分離
                    uname = u.Name;
                    u.Split();

                    // 選択ユニットを再設定
                    SelectedUnit = SRC.UList.Item(GeneralLib.LIndex(u.FeatureData("分離"), 2));
                    Status.DisplayUnitStatus(SelectedUnit);
                }
            }

            // 分離イベント
            Event.HandleEvent("分離", SelectedUnit.MainPilot().ID, uname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            }

            // ハイパーモード＆ノーマルモードの自動発動チェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
            // XXX RedrawScreen 元はしてなかった気がする
            GUI.RedrawScreen();
        }

        // 「合体」コマンド
        private void CombineCommand(UiCommand command)
        {
            //// MOD END MARGE
            //int i, j;
            //string[] list;
            //Unit u;

            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            //list = new string[1];
            //GUI.ListItemFlag = new bool[1];
            var currentUnit = SelectedUnit;
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの時
                // パーツ合体ならば……
                if (command.Label == "パーツ合体" & currentUnit.IsFeatureAvailable("パーツ合体"))
                {
                    // パーツ合体を実施
                    currentUnit.Transform(currentUnit.FeatureData("パーツ合体"));
                    Status.DisplayUnitStatus(Map.MapDataForUnit[currentUnit.x, currentUnit.y]);
                    Map.MapDataForUnit[currentUnit.x, currentUnit.y].CheckAutoHyperMode();
                    Map.MapDataForUnit[currentUnit.x, currentUnit.y].CheckAutoNormalMode();

                    // TODO Impl
                    //// ユニットリストの表示を更新
                    //Event.MakeUnitList(smode: "");

                    // コマンドを終了
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 選択可能な合体パターンのリストを作成
            var combines = currentUnit.CombineFeatures(SRC);
            int i;

            // どの合体を行うかを選択
            if (combines.Count == 1)
            {
                i = 1;
            }
            else
            {
                GUI.TopItem = 1;
                i = GUI.ListBox(new ListBoxArgs
                {
                    lb_caption = "合体後の形態",
                    lb_info = "名前",
                    lb_mode = "",
                    HasFlag = false,
                    Items = combines.Select(x => new ListBoxItem
                    {
                        Text = x.CombineName,
                    }).ToList(),
                });
                if (i == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }
            }
            var combineunitname = combines[i - 1].ConbineUnitName;

            if (Map.IsStatusView)
            {
                // ユニットステータスコマンドの時
                SelectedUnit.Combine(combineunitname, true);

                // ハイパーモード・ノーマルモードの自動発動をチェック
                SRC.UList.CheckAutoHyperMode();
                SRC.UList.CheckAutoNormalMode();

                // TODO Impl
                //// ユニットリストの表示を更新
                //Event.MakeUnitList(smode: "");

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // 合体！
            SelectedUnit.Combine(combineunitname);

            // ハイパーモード＆ノーマルモードの自動発動
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            // 合体後のユニットを選択しておく
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 行動数消費
            SelectedUnit.UseAction();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            }

            Status.DisplayUnitStatus(SelectedUnit);

            // 合体イベント
            Event.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // 行動終了
            WaitCommand(true);
        }

        // 「換装」コマンド
        public void ExchangeFormCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //string[] list;
            //string[] id_list;
            //int j, i, k;
            //int max_value;
            //int ret;
            //string fdata;
            //string[] farray;
            //GUI.LockGUI();
            //{
            //    var withBlock = SelectedUnit;
            //    fdata = withBlock.FeatureData("換装");

            //    // 選択可能な換装先のリストを作成
            //    list = new string[1];
            //    id_list = new string[1];
            //    GUI.ListItemComment = new string[1];
            //    var loopTo = GeneralLib.LLength(fdata);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        {
            //            var withBlock1 = withBlock.OtherForm(GeneralLib.LIndex(fdata, i));
            //            if (withBlock1.IsAvailable())
            //            {
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                Array.Resize(id_list, Information.UBound(list) + 1);
            //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);
            //                id_list[Information.UBound(list)] = withBlock1.Name;

            //                // 各形態の表示内容を作成
            //                if ((SelectedUnit.Nickname0 ?? "") == (withBlock1.Nickname ?? ""))
            //                {
            //                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock1.Name, 27);
            //                    withBlock1.Name = argbuf;
            //                }
            //                else
            //                {
            //                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock1.Nickname0, 27);
            //                    withBlock1.Nickname0 = argbuf1;
            //                }

            //                string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock1.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

            //                string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(withBlock1.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(withBlock1.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(withBlock1.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString() + localLeftPaddedString1() + localLeftPaddedString2() + localLeftPaddedString3() + " " + withBlock1.Data.Adaption;

            //                // 最大攻撃力
            //                max_value = 0;
            //                var loopTo1 = withBlock1.CountWeapon();
            //                for (j = 1; j <= loopTo1; j++)
            //                {
            //                    if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, "合"))
            //                    {
            //                        if (withBlock1.WeaponPower(j, "") > max_value)
            //                        {
            //                            max_value = withBlock1.WeaponPower(j, "");
            //                        }
            //                    }
            //                }

            //                string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(max_value); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4();

            //                // 最大射程
            //                max_value = 0;
            //                var loopTo2 = withBlock1.CountWeapon();
            //                for (j = 1; j <= loopTo2; j++)
            //                {
            //                    if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, "合"))
            //                    {
            //                        if (withBlock1.WeaponMaxRange(j) > max_value)
            //                        {
            //                            max_value = withBlock1.WeaponMaxRange(j);
            //                        }
            //                    }
            //                }

            //                string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(max_value); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString5();

            //                // 換装先が持つ特殊能力一覧
            //                farray = new string[1];
            //                var loopTo3 = withBlock1.CountFeature();
            //                for (j = 1; j <= loopTo3; j++)
            //                {
            //                    if (!string.IsNullOrEmpty(withBlock1.FeatureName(j)))
            //                    {
            //                        // 重複する特殊能力は表示しないようチェック
            //                        var loopTo4 = Information.UBound(farray);
            //                        for (k = 1; k <= loopTo4; k++)
            //                        {
            //                            if ((withBlock1.FeatureName(j) ?? "") == (farray[k] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (k > Information.UBound(farray))
            //                        {
            //                            string localFeatureName() { object argIndex1 = j; var ret = withBlock1.FeatureName(argIndex1); return ret; }

            //                            GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + localFeatureName() + " ";
            //                            Array.Resize(farray, Information.UBound(farray) + 1 + 1);
            //                            farray[Information.UBound(farray)] = withBlock1.FeatureName(j);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

            //    // どの形態に換装するかを選択
            //    GUI.TopItem = 1;
            //    ret = GUI.ListBox("変更先選択", list, "ユニット                     " + Expression.Term(argtname, argu, 4) + " " + Expression.Term(argtname1, argu1, 4) + " " + Expression.Term(argtname2, argu2, 4) + " " + Expression.Term(argtname3, argu3, 4) + " " + "適応 攻撃力 射程", "カーソル移動,コメント");
            //    if (ret == 0)
            //    {
            //        CancelCommand();
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    // 換装を実施
            //    Unit localOtherForm() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //    withBlock.Transform(localOtherForm().Name);
            //    localOtherForm().Name = argnew_form;

            //    // ユニットリストの再構築
            //    Event.MakeUnitList(smode: "");

            //    // カーソル自動移動
            //    if (SRC.AutoMoveCursor)
            //    {
            //        GUI.MoveCursorPos("ユニット選択", withBlock.CurrentForm());
            //    }

            //    Status.DisplayUnitStatus(withBlock.CurrentForm());
            //}

            //CommandState = "ユニット選択";
            //GUI.UnlockGUI();
        }
    }
}
