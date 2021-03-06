// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「チャージ」コマンド
        private void ChargeCommand()
        {
            GUI.LockGUI();
            var ret = GUI.Confirm("チャージを開始しますか？", "チャージ開始", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
            if (ret != GuiDialogResult.Ok)
            {
                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // 使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            var currentUnit = SelectedUnit;
            // チャージのメッセージを表示
            if (currentUnit.IsMessageDefined("チャージ"))
            {
                GUI.OpenMessageForm(u1: null, u2: null);
                currentUnit.PilotMessage("チャージ", msg_mode: "");
                GUI.CloseMessageForm();
            }

            // アニメ表示を行う
            if (currentUnit.IsAnimationDefined("チャージ", sub_situation: ""))
            {
                currentUnit.PlayAnimation("チャージ", sub_situation: "");
            }
            else if (currentUnit.IsSpecialEffectDefined("チャージ", sub_situation: ""))
            {
                currentUnit.SpecialEffect("チャージ", sub_situation: "");
            }
            else
            {
                Sound.PlayWave("Charge.wav");
            }

            // チャージ攻撃のパートナーを探す
            // XXX 条件を満たす最初のアビリティ、武器、の順に選ばれる状態
            IList<Unit> partners = new List<Unit>();
            var cuw = currentUnit.Weapons
                .FirstOrDefault(uw => uw.IsWeaponClassifiedAs("Ｃ") && uw.IsWeaponClassifiedAs("合"));
            if (cuw != null)
            {
                partners = cuw.CombinationPartner();
            }
            var cua = currentUnit.Abilities
                .FirstOrDefault(ua => ua.IsAbilityClassifiedAs("Ｃ") && ua.IsAbilityClassifiedAs("合"));
            if (cua != null)
            {
                partners = cua.CombinationPartner();
            }

            // ユニットの状態をチャージ中に
            currentUnit.AddCondition("チャージ", 1, cdata: "");

            // チャージ攻撃のパートナーもチャージ中にする
            foreach (var pu in partners)
            {
                pu.AddCondition("チャージ", 1, cdata: "");
            }

            // 使用後イベント
            Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「会話」コマンドを開始
        private void StartTalkCommand()
        {
            SelectedCommand = "会話";

            Unit t = null;

            // 会話可能なユニットを表示
            {
                var currentUnit = SelectedUnit;
                Map.AreaInRange(currentUnit.x, currentUnit.y, 1, 1, "");
                var loopTo = Map.MapWidth;
                for (var i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (var j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j])
                        {
                            if (Map.MapDataForUnit[i, j] is object)
                            {
                                if (!Event.IsEventDefined("会話 " + currentUnit.MainPilot().ID + " " + Map.MapDataForUnit[i, j].MainPilot().ID))
                                {
                                    Map.MaskData[i, j] = true;
                                    t = Map.MapDataForUnit[i, j];
                                }
                            }
                        }
                    }
                }

                Map.MaskData[currentUnit.x, currentUnit.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                if (t is object)
                {
                    GUI.MoveCursorPos("ユニット選択", t);
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

        // 「会話」コマンドを終了
        private void FinishTalkCommand()
        {
            Pilot p;
            GUI.LockGUI();
            if (SelectedUnit.CountPilot() > 0)
            {
                p = SelectedUnit.Pilots.First();
            }
            else
            {
                p = null;
            }

            // 会話イベントを実施
            Event.HandleEvent("会話", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (p is object)
            {
                if (p.Unit is object)
                {
                    SelectedUnit = p.Unit;
                }
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「命令」コマンドを開始
        private void StartOrderCommand()
        {
            GUI.LockGUI();

            // 可能な命令内容一覧を作成
            var list = new List<ListBoxItem>()
            {
                new ListBoxItem("自由：自由に行動させる", "自由"),
                new ListBoxItem("移動：指定した位置に移動", "移動"),
                new ListBoxItem("攻撃：指定した敵を攻撃", "攻撃"),
                new ListBoxItem("護衛：指定したユニットを護衛", "護衛"),
            };
            if (SelectedUnit.Summoner is object || SelectedUnit.Master is object)
            {
                if (SelectedUnit.Master is object)
                {
                    list.Add(new ListBoxItem("帰還：主人の所に戻る", "帰還"));
                }
                else
                {
                    list.Add(new ListBoxItem("帰還：召喚主の所に戻る", "帰還"));
                }
            }

            // 命令する行動パターンを選択
            var ret = GUI.ListBox(new ListBoxArgs
            {
                Items = list,
                lb_caption = "命令",
                lb_info = "行動パターン",
                lb_mode = "",
            });

            // 選択された行動パターンに応じてターゲット領域を表示
            switch (ret)
            {
                case 0:
                    {
                        CancelCommand();
                        break;
                    }

                case 1: // 自由
                    {
                        SelectedUnit.Mode = "通常";
                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(SelectedUnit);
                        break;
                    }

                case 2: // 移動
                    {
                        SelectedCommand = "移動命令";
                        var loopTo = Map.MapWidth;
                        for (var i = 1; i <= loopTo; i++)
                        {
                            var loopTo1 = Map.MapHeight;
                            for (var j = 1; j <= loopTo1; j++)
                                Map.MaskData[i, j] = false;
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 3: // 攻撃
                    {
                        SelectedCommand = "攻撃命令";
                        Map.AreaWithUnit("味方の敵");
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 4: // 護衛
                    {
                        SelectedCommand = "護衛命令";
                        Map.AreaWithUnit("味方");
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 5: // 帰還
                    {
                        if (SelectedUnit.Master is object)
                        {
                            SelectedUnit.Mode = SelectedUnit.Master.MainPilot().ID;
                        }
                        else
                        {
                            SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot().ID;
                        }

                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(SelectedUnit);
                        break;
                    }
            }

            GUI.UnlockGUI();
        }

        // 「命令」コマンドを終了
        private void FinishOrderCommand()
        {
            switch (SelectedCommand ?? "")
            {
                case "移動命令":
                    {
                        SelectedUnit.Mode = SrcFormatter.Format(SelectedX) + " " + SrcFormatter.Format(SelectedY);
                        break;
                    }

                case "攻撃命令":
                case "護衛命令":
                    {
                        SelectedUnit.Mode = SelectedTarget.MainPilot().ID;
                        break;
                    }
            }

            if (ReferenceEquals(Status.DisplayedUnit, SelectedUnit))
            {
                Status.DisplayUnitStatus(SelectedUnit);
            }

            GUI.RedrawScreen();
            CommandState = "ユニット選択";
        }
    }
}
