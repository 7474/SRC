// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // スペシャルパワーコマンドを開始
        private void StartSpecialPowerCommand()
        {
            GUI.LockGUI();
            SelectedCommand = "スペシャルパワー";
            {
                var u = SelectedUnit;

                // スペシャルパワーを使用可能なパイロットの一覧を作成
                var pilots = u.PilotsHaveSpecialPower();
                var listItems = pilots.Select(p =>
                {
                    return new ListBoxItem
                    {
                        Text = GeneralLib.RightPaddedString(p.get_Nickname(false), 17)
                            + GeneralLib.RightPaddedString($"{p.SP}/{p.MaxSP}", 8)
                            + string.Join("", Enumerable.Range(1, p.CountSpecialPower).Select(i =>
                            {
                                var sname = p.get_SpecialPower(i);
                                if (p.SP >= p.SpecialPowerCost(sname))
                                {
                                    return SRC.SPDList.Item(sname).ShortName;
                                }
                                else
                                {
                                    return "";
                                }
                            })),
                    };
                }).ToList();

                GUI.TopItem = 1;
                int i;
                if (pilots.Count > 1)
                {
                    // どのパイロットを使うか選択
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        i = GUI.ListBox(new ListBoxArgs
                        {
                            Items = listItems,
                            HasFlag = false,
                            lb_caption = "キャラクター選択",
                            lb_info = "キャラクター     " + Expression.Term("SP", SelectedUnit, 2) + "/Max" + Expression.Term("SP", SelectedUnit, 2),
                            lb_mode = "連続表示,カーソル移動"
                        });
                    }
                    else
                    {
                        i = GUI.ListBox(new ListBoxArgs
                        {
                            Items = listItems,
                            HasFlag = false,
                            lb_caption = "パイロット選択",
                            lb_info = "パイロット       " + Expression.Term("SP", SelectedUnit, 2) + "/Max" + Expression.Term("SP", SelectedUnit, 2),
                            lb_mode = "連続表示,カーソル移動"
                        });
                    }
                }
                else
                {
                    // 一人しかいないので選択の必要なし
                    i = 1;
                }

                // 誰もスペシャルパワーを使えなければキャンセル
                if (i == 0)
                {
                    GUI.CloseListBox();
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // スペシャルパワーを使うパイロットを設定
                SelectedPilot = pilots[i - 1];
                // そのパイロットのステータスを表示
                if (pilots.Count > 1)
                {
                    Status.DisplayPilotStatus(SelectedPilot);
                }
            }

            {
                var p = SelectedPilot;
                // 使用可能なスペシャルパワーの一覧を作成
                var spList = p.SpecialPowerNames.Select(sname =>
                {
                    var cost = p.SpecialPowerCost(sname);
                    var spd = SRC.SPDList.Item(sname);

                    return new ListBoxItem
                    {
                        Text = GeneralLib.RightPaddedString(sname, 13) + GeneralLib.LeftPaddedString("" + cost, 3) + " " + spd.Comment,
                        ListItemFlag = p.SP < cost || !p.IsSpecialPowerUseful(sname),
                    };
                }).ToList();

                // どのコマンドを使用するかを選択
                GUI.TopItem = 1;
                var i = GUI.ListBox(new ListBoxArgs
                {
                    Items = spList,
                    HasFlag = true,
                    lb_caption = Expression.Term("スペシャルパワー", SelectedUnit) + "選択",
                    lb_info = "名称         消費" + Expression.Term("SP", SelectedUnit) + "（" + p.get_Nickname(false) + " " + Expression.Term("SP", SelectedUnit) + "=" + SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP) + "）",
                    lb_mode = "カーソル移動(行きのみ)"
                });
                // キャンセル
                if (i == 0)
                {
                    Status.DisplayUnitStatus(SelectedUnit);
                    // カーソル自動移動
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.MoveCursorPos("ユニット選択", SelectedUnit);
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // 使用するスペシャルパワーを設定
                SelectedSpecialPower = SelectedPilot.get_SpecialPower(i);
            }

            // 味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
            // 使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
            if (SRC.SPDList.Item(SelectedSpecialPower).Effects.Any(x => x.strEffectType == "味方スペシャルパワー実行"))
            {
                // スペシャルパワー一覧
                var list = SRC.SPDList.Items.Where(x =>
                        !x.Effects.Any(x => x.strEffectType == "味方スペシャルパワー実行")
                        && x.ShortName != "非表示")
                    .OrderBy(x => x.KanaName)
                    .Select(spd =>
                    {
                        var learned = false;
                        // スペシャルパワーを使用可能なパイロットがいるかどうかを判定
                        foreach (var p in SRC.PList.Items)
                        {
                            if (p.Party == "味方")
                            {
                                if (p.Unit != null)
                                {
                                    if (p.Unit.Status == "出撃" && !p.Unit.IsConditionSatisfied("憑依"))
                                    {
                                        // 本当に乗っている？
                                        if (p.Unit.AllPilots.Any(x => x == p))
                                        {
                                            if (p.IsSpecialPowerAvailable(spd.Name))
                                            {
                                                learned = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // 各スペシャルパワーが使用可能か判定
                        var colP = SelectedPilot;
                        var canUse = learned
                            && colP.SP >= 2 * colP.SpecialPowerCost(spd.Name)
                            && colP.IsSpecialPowerUseful(spd.Name);
                        return new
                        {
                            spd,
                            learned,
                            canUse,
                        };
                    })
                    .Select(x => new ListBoxItem(x.spd.Name)
                    {
                        // スペシャルパワーの解説を設定
                        ListItemComment = x.spd.Comment,
                        ListItemFlag = !x.canUse,
                    })
                    .ToList();

                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                var ret = GUI.MultiColumnListBox(new ListBoxArgs
                {
                    Items = list,
                    HasFlag = true,
                    lb_caption = Expression.Term("スペシャルパワー") + "検索",
                }, true);
                if (ret == 0)
                {
                    SelectedSpecialPower = "";
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // スペシャルパワー使用メッセージ
                if (SelectedUnit.IsMessageDefined(SelectedSpecialPower))
                {
                    GUI.OpenMessageForm(u1: null, u2: null);
                    SelectedUnit.PilotMessage(SelectedSpecialPower, msg_mode: "");
                    GUI.CloseMessageForm();
                }

                SelectedSpecialPower = list[ret - 1].ListItemID;
                WithDoubleSPConsumption = true;
            }
            else
            {
                WithDoubleSPConsumption = false;
            }

            var sd = SRC.SPDList.Item(SelectedSpecialPower);

            // ターゲットを選択する必要があるスペシャルパワーの場合
            switch (sd.TargetType ?? "")
            {
                case "味方":
                case "敵":
                case "任意":
                    {
                        // マップ上のユニットからターゲットを選択する
                        GUI.OpenMessageForm(null, null);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "ターゲットを選んでください。");
                        GUI.CloseMessageForm();

                        // ターゲットのエリアを設定
                        for (var i = 1; i <= Map.MapWidth; i++)
                        {
                            for (var j = 1; j <= Map.MapHeight; j++)
                            {
                                Map.MaskData[i, j] = true;
                                var u = Map.MapDataForUnit[i, j];
                                if (u is null)
                                {
                                    goto NextLoop;
                                }

                                // 陣営が合っている？
                                switch (sd.TargetType ?? "")
                                {
                                    case "味方":
                                        {
                                            {
                                                var withBlock8 = u;
                                                if (withBlock8.Party != "味方" && withBlock8.Party0 != "味方" && withBlock8.Party != "ＮＰＣ" && withBlock8.Party0 != "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }

                                    case "敵":
                                        {
                                            {
                                                var withBlock9 = u;
                                                if (withBlock9.Party == "味方" && withBlock9.Party0 == "味方" || withBlock9.Party == "ＮＰＣ" && withBlock9.Party0 == "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }
                                }

                                // スペシャルパワーを適用可能？
                                if (!sd.Effective(SelectedPilot, u))
                                {
                                    goto NextLoop;
                                }

                                Map.MaskData[i, j] = false;
                            NextLoop:
                                ;
                            }
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                case "破壊味方":
                    {
                        // 破壊された味方ユニットの中からターゲットを選択する
                        GUI.OpenMessageForm(null, null);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "復活させるユニットを選んでください。");
                        GUI.CloseMessageForm();

                        // 破壊された味方ユニットのリストを作成
                        var units = SRC.UList.Items
                            .Where(u => u.Party0 == "味方" && u.Status == "破壊" && (u.CountPilot() > 0 || u.Data.PilotNum == 0))
                            .Select(u => new ListBoxItem
                            {
                                Text = GeneralLib.RightPaddedString(u.Nickname, 28)
                                    + GeneralLib.RightPaddedString(u.MainPilot().get_Nickname(false), 18)
                                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 3),
                                ListItemID = u.ID,
                            }).ToList();

                        GUI.TopItem = 1;
                        var ret = GUI.ListBox(new ListBoxArgs
                        {
                            Items = units,
                            lb_caption = "ユニット選択",
                            lb_info = "ユニット名                  パイロット     レベル",
                            lb_mode = "",
                        });

                        if (ret == 0)
                        {
                            GUI.UnlockGUI();
                            CancelCommand();
                            return;
                        }

                        SelectedTarget = SRC.UList.Item(units[ret - 1].ListItemID);
                        break;
                    }
            }

            // 自爆を選択した場合は確認を取る
            if (sd.IsEffectAvailable("自爆"))
            {
                var ret = GUI.Confirm("自爆させますか？", "自爆", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                if (ret == GuiDialogResult.Ok)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                GUI.UnlockGUI();
                return;
            }

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            }

            // ステータスウィンドウを更新
            Status.DisplayUnitStatus(SelectedUnit);

            // 使用後イベント
            Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            SelectedSpecialPower = "";
            GUI.UnlockGUI();
            CommandState = "ユニット選択";
        }

        // スペシャルパワーコマンドを終了
        private void FinishSpecialPowerCommand()
        {
            GUI.LockGUI();

            // 自爆を選択した場合は確認を取る
            {
                var spd = SRC.SPDList.Item(SelectedSpecialPower);
                if (spd.Effects.Any(x => x.strEffectType == "自爆"))
                {
                    if (GUI.Confirm("自爆させますか？", "自爆", GuiConfirmOption.OkCancel | GuiConfirmOption.Question) != GuiDialogResult.Ok)
                    {
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // ステータスウィンドウを更新
            if (SelectedTarget is object)
            {
                if (SelectedTarget.CurrentForm().Status == "出撃")
                {
                    Status.DisplayUnitStatus(SelectedTarget);
                }
            }

            // 使用後イベント
            Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            SelectedSpecialPower = "";
            GUI.UnlockGUI();
            CommandState = "ユニット選択";
        }
    }
}
