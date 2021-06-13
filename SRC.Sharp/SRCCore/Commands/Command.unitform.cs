// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Units;
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
                // ユニットステータスコマンドの場合
                var forms = GeneralLib.ToL(fdata).Skip(1)
                    .Select(x => SelectedUnit.OtherForm(x))
                    .Where(x => x.IsAvailable())
                    .Select(x => new ListBoxItem
                    {
                        ListItemID = x.Name,
                        Text = x.Nickname0,
                    }).ToList();
                var ret = 1;
                if (forms.Count > 1)
                {

                    GUI.TopItem = 1;
                    ret = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = SelectedUnit.IsHero() ? "変身先" : "変形先",
                        lb_info = "名前",
                        lb_mode = "カーソル移動",
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

                // 変形を実施
                var u = SelectedUnit;
                u.Transform(uname);

                // ユニットリストの表示を更新
                Event.MakeUnitList(smode: "");

                // ステータスウィンドウの表示を更新
                Status.DisplayUnitStatus(u.CurrentForm());

                // コマンドを終了
                // XXX RedrawScreen 元はしてなかった気がする
                GUI.RedrawScreen();
                GUI.UnlockGUI();
                CommandState = "ユニット選択";
                return;
            }
            {
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
                string BGM;
                {
                    var u = SelectedUnit;
                    // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                    AddAdditionalPilotIfNotExist(uname, u);

                    // ＢＧＭの変更
                    if (u.IsFeatureAvailable("変形ＢＧＭ"))
                    {
                        foreach (var fd in u.Features.Where(x => x.Name == "変形ＢＧＭ")
                            .Where(x => GeneralLib.LIndex(x.Data, 1) == uname))
                        {
                            BGM = Sound.SearchMidiFile(Strings.Mid(fd.Data, Strings.InStr(fd.Data, " ") + 1));
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(BGM);
                                GUI.Sleep(500);
                            }
                            break;
                        }
                    }

                    // メッセージを表示
                    SelectedUnit.PilotMassageIfDefined(new string[] {
                        "変形(" + u.Name + "=>" + uname + ")",
                        "変形(" + uname + ")",
                        "変形(" + u.FeatureName("変形") + ")",
                    });

                    // アニメ表示
                    SelectedUnit.PlayAnimationIfDefined(new string[] {
                        "変形(" + u.Name + "=>" + uname + ")",
                        "変形(" + uname + ")",
                        "変形(" + u.FeatureName("変形") + ")",
                    });
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
            if (Map.IsStatusView)
            {
                // ユニットステータスコマンドの場合
                if (!SelectedUnit.IsFeatureAvailable("ハイパーモード"))
                {
                    uname = GeneralLib.LIndex(SelectedUnit.FeatureData("ノーマルモード"), 1);
                }

                // ハイパーモードを発動
                SelectedUnit.Transform(uname);

                // ユニットリストの表示を更新
                Event.MakeUnitList(smode: "");

                // ステータスウィンドウの表示を更新
                Status.DisplayUnitStatus(SelectedUnit.CurrentForm());

                // コマンドを終了
                GUI.UnlockGUI();
                CommandState = "ユニット選択";
                return;
            }

            // ハイパーモードを発動可能かどうかチェック
            {
                var withBlock1 = SelectedUnit.OtherForm(uname);
                if (!withBlock1.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) && !string.IsNullOrEmpty(Map.MapFileName))
                {
                    GUI.Confirm("この地形では変形できません", "", GuiConfirmOption.Ok);
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            AddAdditionalPilotIfNotExist(uname, SelectedUnit);

            {
                // ＢＧＭを変更
                if (SelectedUnit.IsFeatureAvailable("ハイパーモードＢＧＭ"))
                {
                    var loopTo11 = SelectedUnit.CountFeature();
                    for (var i = 1; i <= loopTo11; i++)
                    {
                        var fdata = SelectedUnit.Feature(i).Data;
                        if (SelectedUnit.Feature(i).Name == "ハイパーモードＢＧＭ"
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
                SelectedUnit.PilotMassageIfDefined(new string[]
                {
                    "ハイパーモード(" + SelectedUnit.Name + "=>" + uname + ")",
                    "ハイパーモード(" + uname + ")",
                    "ハイパーモード(" + fname + ")"
                });

                // アニメ表示
                SelectedUnit.PlayAnimation(new string[] {
                    "ハイパーモード(" + SelectedUnit.Name + "=>" + uname + ")",
                    "ハイパーモード(" + uname + ")",
                    "ハイパーモード(" + fname + ")",
                    "ハイパーモード",
                });
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

                // TODO Impl ユニットリストの表示を更新
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

                    // ＢＧＭ変更
                    if (u.IsFeatureAvailable("分離ＢＧＭ"))
                    {
                        var BGM = Sound.SearchMidiFile(u.FeatureData("分離ＢＧＭ"));
                        if (Strings.Len(BGM) > 0)
                        {
                            Sound.StartBGM(u.FeatureData("分離ＢＧＭ"));
                            GUI.Sleep(500);
                        }
                    }

                    fname = u.FeatureName("パーツ分離");

                    // メッセージを表示
                    SelectedUnit.PilotMassageIfDefined(new string[] {
                        "分離(" + u.Name + ")",
                        "分離(" + fname + ")",
                        "分離",
                    });

                    // アニメ表示
                    SelectedUnit.PlayAnimation(new string[] {
                        "分離(" + u.Name + ")",
                        "分離(" + fname + ")",
                        "分離",
                    });

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

                    // ＢＧＭ変更
                    if (u.IsFeatureAvailable("分離ＢＧＭ"))
                    {
                        var BGM = Sound.SearchMidiFile(u.FeatureData("分離ＢＧＭ"));
                        if (Strings.Len(BGM) > 0)
                        {
                            Sound.StartBGM(u.FeatureData("分離ＢＧＭ"));
                            GUI.Sleep(500);
                        }
                    }

                    fname = u.FeatureName("分離");

                    // メッセージを表示
                    SelectedUnit.PilotMassageIfDefined(new string[] {
                        "分離(" + u.Name + ")",
                        "分離(" + fname + ")",
                        "分離",
                    });

                    // アニメ表示
                    SelectedUnit.PlayAnimation(new string[] {
                        "分離(" + u.Name + ")",
                        "分離(" + fname + ")",
                        "分離",
                    });

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
                if (command.Label == "パーツ合体" && currentUnit.IsFeatureAvailable("パーツ合体"))
                {
                    // パーツ合体を実施
                    currentUnit.Transform(currentUnit.FeatureData("パーツ合体"));
                    Status.DisplayUnitStatus(Map.MapDataForUnit[currentUnit.x, currentUnit.y]);
                    Map.MapDataForUnit[currentUnit.x, currentUnit.y].CheckAutoHyperMode();
                    Map.MapDataForUnit[currentUnit.x, currentUnit.y].CheckAutoNormalMode();

                    // TODO Impl ユニットリストの表示を更新
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

                // TODO Impl ユニットリストの表示を更新
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
            GUI.LockGUI();
            var fd = SelectedUnit.Feature("換装");

            // 選択可能な換装先のリストを作成
            var uList = fd.DataL.Select(x => SelectedUnit.OtherForm(x))
                .Where(x => x.IsAvailable())
                .Select(u =>
                {
                    // 各形態の表示内容を作成
                    var msg = "";
                    msg += (u.Nickname == SelectedUnit.Nickname0)
                        ? GeneralLib.RightPaddedString(u.Name, 27)
                        : GeneralLib.RightPaddedString(u.Nickname0, 27);
                    // ユニットに関する情報
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5);

                    var weapons = u.Weapons.Where(uw => uw.IsDisplayFor(WeaponListMode.List)).ToList();
                    // 最大攻撃力
                    var maxPower = weapons.Max(uw => uw.WeaponPower(""));
                    msg += GeneralLib.LeftPaddedString("" + maxPower, 7);
                    // 最大射程
                    var maxRange = weapons.Max(uw => uw.WeaponMaxRange());
                    msg += GeneralLib.LeftPaddedString("" + maxRange, 5);

                    // 換装先が持つ特殊能力一覧
                    var comment = string.Join(" ", u.Features
                            .Select(fd => fd.FeatureName(u))
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct());
                    return new ListBoxItem(msg, u.ID)
                    {
                        ListItemComment = comment,
                    };
                })
                .ToList();

            // どの形態に換装するかを選択
            GUI.TopItem = 1;
            var tfRet = GUI.ListBox(new ListBoxArgs
            {
                Items = uList,
                lb_caption = "変更先選択",
                lb_info = "ユニット                     "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " 適応 攻撃力 射程",
                lb_mode = "連続表示,コメント",
            });
            // キャンセル？
            if (tfRet == 0)
            {
                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // 換装を実施
            SelectedUnit.Transform(SelectedUnit.OtherForm(uList[tfRet - 1].ListItemID).Name);

            // ユニットリストの再構築
            Event.MakeUnitList(smode: "");

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", SelectedUnit.CurrentForm());
            }

            Status.DisplayUnitStatus(SelectedUnit.CurrentForm());

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }
    }
}
