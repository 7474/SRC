// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「チャージ」コマンド
        private void ChargeCommand()
        {
            throw new NotImplementedException();
            //int ret;
            //Unit[] partners;
            //int i;
            //GUI.LockGUI();
            //ret = Interaction.MsgBox("チャージを開始しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "チャージ開始");
            //if (ret == MsgBoxResult.Cancel)
            //{
            //    CancelCommand();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// 使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, "チャージ");
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //{
            //    var withBlock = SelectedUnit;
            //    // チャージのメッセージを表示
            //    if (withBlock.IsMessageDefined("チャージ"))
            //    {
            //        GUI.OpenMessageForm(u1: null, u2: null);
            //        withBlock.PilotMessage("チャージ", msg_mode: "");
            //        GUI.CloseMessageForm();
            //    }

            //    // アニメ表示を行う
            //    if (withBlock.IsAnimationDefined("チャージ", sub_situation: ""))
            //    {
            //        withBlock.PlayAnimation("チャージ", sub_situation: "");
            //    }
            //    else if (withBlock.IsSpecialEffectDefined("チャージ", sub_situation: ""))
            //    {
            //        withBlock.SpecialEffect("チャージ", sub_situation: "");
            //    }
            //    else
            //    {
            //        Sound.PlayWave("Charge.wav");
            //    }

            //    // チャージ攻撃のパートナーを探す
            //    partners = new Unit[1];
            //    var loopTo = withBlock.CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (withBlock.IsWeaponClassifiedAs(i, "Ｃ") & withBlock.IsWeaponClassifiedAs(i, "合"))
            //        {
            //            if (withBlock.IsWeaponAvailable(i, "チャージ"))
            //            {
            //                withBlock.CombinationPartner("武装", i, partners);
            //                break;
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        var loopTo1 = withBlock.CountAbility();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            if (withBlock.IsAbilityClassifiedAs(i, "Ｃ") & withBlock.IsAbilityClassifiedAs(i, "合"))
            //            {
            //                if (withBlock.IsAbilityAvailable(i, "チャージ"))
            //                {
            //                    withBlock.CombinationPartner("アビリティ", i, partners);
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    // ユニットの状態をチャージ中に
            //    withBlock.AddCondition("チャージ", 1, cdata: "");

            //    // チャージ攻撃のパートナーもチャージ中にする
            //    var loopTo2 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        {
            //            var withBlock1 = partners[i];
            //            withBlock1.AddCondition("チャージ", 1, cdata: "");
            //        }
            //    }
            //}

            //// 使用後イベント
            //Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, "チャージ");
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //GUI.UnlockGUI();

            //// 行動終了
            //WaitCommand();
        }

        // 「会話」コマンドを開始
        private void StartTalkCommand()
        {
            throw new NotImplementedException();
            //int i, j;
            //Unit t;
            //SelectedCommand = "会話";

            //// UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //t = null;

            //// 会話可能なユニットを表示
            //{
            //    var withBlock = SelectedUnit;
            //    Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, "");
            //    var loopTo = Map.MapWidth;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        var loopTo1 = Map.MapHeight;
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if (!Map.MaskData[i, j])
            //            {
            //                if (Map.MapDataForUnit[i, j] is object)
            //                {
            //                    bool localIsEventDefined() { var arglname = "会話 " + withBlock.MainPilot().ID + " " + Map.MapDataForUnit[i, j].MainPilot.ID; var ret = Event_Renamed.IsEventDefined(arglname); return ret; }

            //                    if (!localIsEventDefined())
            //                    {
            //                        Map.MaskData[i, j] = true;
            //                        t = Map.MapDataForUnit[i, j];
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
            //    if (t is object)
            //    {
            //        GUI.MoveCursorPos("ユニット選択", t);
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

        // 「会話」コマンドを終了
        private void FinishTalkCommand()
        {
            throw new NotImplementedException();
            //Pilot p;
            //GUI.LockGUI();
            //if (SelectedUnit.CountPilot() > 0)
            //{
            //    p = SelectedUnit.Pilot(1);
            //}
            //else
            //{
            //    // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    p = null;
            //}

            //// 会話イベントを実施
            //Event_Renamed.HandleEvent("会話", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    return;
            //}

            //if (p is object)
            //{
            //    if (p.Unit_Renamed is object)
            //    {
            //        SelectedUnit = p.Unit_Renamed;
            //    }
            //}

            //GUI.UnlockGUI();

            //// 行動終了
            //WaitCommand();
        }

        // 「命令」コマンドを開始
        private void StartOrderCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //string[] list;
            //int i, ret, j;
            //GUI.LockGUI();
            //list = new string[5];
            //GUI.ListItemFlag = new bool[5];

            //// 可能な命令内容一覧を作成
            //list[1] = "自由：自由に行動させる";
            //list[2] = "移動：指定した位置に移動";
            //list[3] = "攻撃：指定した敵を攻撃";
            //list[4] = "護衛：指定したユニットを護衛";
            //if (SelectedUnit.Summoner is object | SelectedUnit.Master is object)
            //{
            //    Array.Resize(list, 6);
            //    Array.Resize(GUI.ListItemFlag, 6);
            //    if (SelectedUnit.Master is object)
            //    {
            //        list[5] = "帰還：主人の所に戻る";
            //    }
            //    else
            //    {
            //        list[5] = "帰還：召喚主の所に戻る";
            //    }
            //}

            //// 命令する行動パターンを選択
            //ret = GUI.ListBox("命令", list, "行動パターン", lb_mode: "");

            //// 選択された行動パターンに応じてターゲット領域を表示
            //switch (ret)
            //{
            //    case 0:
            //        {
            //            CancelCommand();
            //            break;
            //        }

            //    case 1: // 自由
            //        {
            //            SelectedUnit.Mode = "通常";
            //            CommandState = "ユニット選択";
            //            Status.DisplayUnitStatus(SelectedUnit);
            //            break;
            //        }

            //    case 2: // 移動
            //        {
            //            SelectedCommand = "移動命令";
            //            var loopTo = Map.MapWidth;
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                var loopTo1 = Map.MapHeight;
            //                for (j = 1; j <= loopTo1; j++)
            //                    Map.MaskData[i, j] = false;
            //            }

            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 3: // 攻撃
            //        {
            //            SelectedCommand = "攻撃命令";
            //            Map.AreaWithUnit("味方の敵");
            //            Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 4: // 護衛
            //        {
            //            SelectedCommand = "護衛命令";
            //            Map.AreaWithUnit("味方");
            //            Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 5: // 帰還
            //        {
            //            if (SelectedUnit.Master is object)
            //            {
            //                SelectedUnit.Mode = SelectedUnit.Master.MainPilot().ID;
            //            }
            //            else
            //            {
            //                SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot().ID;
            //            }

            //            CommandState = "ユニット選択";
            //            Status.DisplayUnitStatus(SelectedUnit);
            //            break;
            //        }
            //}

            //GUI.UnlockGUI();
        }

        // 「命令」コマンドを終了
        private void FinishOrderCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //switch (SelectedCommand ?? "")
            //{
            //    case "移動命令":
            //        {
            //            SelectedUnit.Mode = SrcFormatter.Format(SelectedX) + " " + SrcFormatter.Format(SelectedY);
            //            break;
            //        }

            //    case "攻撃命令":
            //    case "護衛命令":
            //        {
            //            SelectedUnit.Mode = SelectedTarget.MainPilot().ID;
            //            break;
            //        }
            //}

            //if (ReferenceEquals(Status.DisplayedUnit, SelectedUnit))
            //{
            //    Status.DisplayUnitStatus(SelectedUnit);
            //}

            //GUI.RedrawScreen();
            //CommandState = "ユニット選択";
        }
    }
}
