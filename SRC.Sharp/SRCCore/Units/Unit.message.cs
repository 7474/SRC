// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // === メッセージ関連処理 ===
    public partial class Unit
    {
        // 状況 Situation に応じたパイロットメッセージを表示
        public void PilotMessage(string Situation, string msg_mode = "")
        {
            string wname = "";

            // WAVEを演奏したかチェックするため、あらかじめクリア
            Sound.IsWavePlayed = false;

            // 対応メッセージが定義されていなかった場合に使用するシチュエーションを設定
            var situations = new List<string>();
            situations.Add(Situation);
            switch (Situation ?? "")
            {
                case "分身":
                case "切り払い":
                case "迎撃":
                case "反射":
                case "当て身技":
                case "阻止":
                case "ダミー":
                case "緊急テレポート":
                    situations.Add("回避");
                    break;

                case "ビーム無効化":
                case "攻撃無効化":
                case "シールド防御":
                    situations.Add("ダメージ小");
                    break;

                case "回避":
                case "破壊":
                case "ダメージ大":
                case "ダメージ中":
                case "ダメージ小":
                case "かけ声":
                    break;

                default:
                    {
                        if (msg_mode == "攻撃" || msg_mode == "カウンター")
                        {
                            // 攻撃メッセージ
                            wname = Situation;

                            // 武器番号を検索
                            var weapon = Weapons.First(x => x.Name == wname || x.WeaponNickname() == wname);

                            if (!IsDefense())
                            {
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘");
                                }
                                else
                                {
                                    situations.Add("射撃");
                                }
                            }
                            else if (msg_mode == "カウンター")
                            {
                                situations.Clear();
                                situations.Add(Situation + "(反撃)");
                                situations.Add(Situation);
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘");
                                }
                                else
                                {
                                    situations.Add("射撃");
                                }
                            }
                            else
                            {
                                situations.Clear();
                                situations.Add(Situation + "(反撃)");
                                situations.Add(Situation);
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘(反撃)");
                                    situations.Add("格闘");
                                }
                                else
                                {
                                    situations.Add("射撃(反撃)");
                                    situations.Add("射撃");
                                }
                            }
                        }
                        else if (msg_mode == "アビリティ")
                        {
                            situations.Add("アビリティ");
                        }
                        else if (Strings.InStr(Situation, "(命中)") > 0
                            || Strings.InStr(Situation, "(回避)") > 0
                            || Strings.InStr(Situation, "(とどめ)") > 0
                            || Strings.InStr(Situation, "(クリティカル)") > 0)
                        {
                            // サブシチュエーション付きの攻撃メッセージ

                            // 武器番号を検索
                            wname = Strings.Left(Situation, GeneralLib.InStr2(Situation, "(") - 1);
                            var weapon = Weapons.First(x => x.Name == wname || x.WeaponNickname() == wname);

                            if (!IsDefense())
                            {
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                                else
                                {
                                    situations.Add("射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                            }
                            else if (msg_mode == "カウンター")
                            {
                                situations.Clear();
                                situations.Add(Situation + "(反撃)");
                                situations.Add(Situation);
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                                else
                                {
                                    situations.Add("射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                            }
                            else
                            {
                                situations.Clear();
                                situations.Add(Situation + "(反撃)");
                                situations.Add(Situation);
                                if (weapon.IsWeaponClassifiedAs("格闘系"))
                                {
                                    situations.Add("格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")) + "(反撃)");
                                    situations.Add("格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                                else
                                {
                                    situations.Add("射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")) + "(反撃)");
                                    situations.Add("射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, "(")));
                                }
                            }
                        }
                        // 攻撃メッセージでなくても一応攻撃武器名を設定
                        else if (ReferenceEquals(Commands.SelectedUnit, this))
                        {
                            if (0 < Commands.SelectedWeapon && Commands.SelectedWeapon <= CountWeapon())
                            {
                                wname = Weapon(Commands.SelectedWeapon).Name;
                            }
                        }
                        else if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            if (0 < Commands.SelectedTWeapon && Commands.SelectedTWeapon <= CountWeapon())
                            {
                                wname = Weapon(Commands.SelectedTWeapon).Name;
                            }
                        }

                        break;
                    }
            }

            // SelectMessageコマンド
            string selected_pilot = "";
            string selected_msg = "";
            string selected_situation = "";
            foreach (var situ in situations)
            {
                var buf = "Message(" + MainPilot().ID + "," + situ + ")";
                if (Expression.IsLocalVariableDefined(buf))
                {
                    selected_msg = Conversions.ToString(Event.LocalVariableList[buf].StringValue);
                    selected_situation = situ;
                    Expression.UndefineVariable(buf);
                    break;
                }

                if (situ == "格闘" || situ == "射撃")
                {
                    buf = "Message(" + MainPilot().ID + ",攻撃)";
                    if (Expression.IsLocalVariableDefined(buf))
                    {
                        selected_msg = Conversions.ToString(Event.LocalVariableList[buf].StringValue);
                        selected_situation = "攻撃";
                        Expression.UndefineVariable(buf);
                        break;
                    }
                }

                if (situ == "格闘(反撃)" || situ == "射撃(反撃)")
                {
                    buf = "Message(" + MainPilot().ID + ",攻撃(反撃))";
                    if (Expression.IsLocalVariableDefined(buf))
                    {
                        selected_msg = Conversions.ToString(Event.LocalVariableList[buf].StringValue);
                        selected_situation = "攻撃(反撃)";
                        Expression.UndefineVariable(buf);
                        break;
                    }
                }
            }

            if (Strings.InStr(selected_msg, "::") > 0)
            {
                selected_pilot = Strings.Left(selected_msg, Strings.InStr(selected_msg, "::") - 1);
                selected_msg = Strings.Mid(selected_msg, Strings.InStr(selected_msg, "::") + 2);
            }

            // かけ声は３分の２の確率で表示
            if (string.IsNullOrEmpty(selected_msg))
            {
                if (Strings.InStr(Situation, "かけ声") == 1)
                {
                    if (GeneralLib.Dice(3) == 1)
                    {
                        return;
                    }
                }
            }

            // しゃべれない場合
            // ただしSetMessageコマンドでメッセージが設定されている場合は
            // そちらを使用。
            if (string.IsNullOrEmpty(selected_msg))
            {
                if (IsConditionSatisfied("石化") || IsConditionSatisfied("凍結") || IsConditionSatisfied("麻痺"))
                {
                    // 意識不明
                    return;
                }

                if (IsConditionSatisfied("沈黙") || IsConditionSatisfied("憑依"))
                {
                    // 無言
                    if (Strings.InStr(Situation, "(") == 0)
                    {
                        switch (Situation ?? "")
                        {
                            case "ダメージ中":
                            case "ダメージ大":
                            case "破壊":
                                {
                                    if (SRC.NPDList.IsDefined(MainPilot().Name + "(ダメージ)"))
                                    {
                                        GUI.DisplayBattleMessage(MainPilot().Name + "(ダメージ)", "…………！", msg_mode: "");
                                        return;
                                    }

                                    break;
                                }

                            case "かけ声":
                                {
                                    return;
                                }
                        }

                        if (!string.IsNullOrEmpty(wname))
                        {
                            if (SRC.NPDList.IsDefined(MainPilot().Name + "(攻撃)"))
                            {
                                GUI.DisplayBattleMessage(MainPilot().Name + "(攻撃)", "…………！", msg_mode: "");
                                return;
                            }
                        }

                        GUI.DisplayBattleMessage(MainPilot().ID, "…………", msg_mode: "");
                    }

                    return;
                }

                if (IsConditionSatisfied("睡眠"))
                {
                    // 寝言
                    if (Strings.InStr(Situation, "(") == 0)
                    {
                        GUI.DisplayBattleMessage(MainPilot().ID, "ＺＺＺ……", msg_mode: "");
                    }

                    return;
                }

                if (IsConditionSatisfied("恐怖"))
                {
                    if (IsMessageDefined("恐怖"))
                    {
                        // 恐怖状態用メッセージが定義されていればそちらを使う
                        situations.Clear();
                        situations.Add("恐怖");
                    }
                    else
                    {
                        // パニック時のメッセージを作成して表示
                        if (Strings.InStr(Situation, "(") == 0)
                        {
                            var msg = "";
                            switch (MainPilot().Sex ?? "")
                            {
                                case "男性":
                                    {
                                        switch (GeneralLib.Dice(4))
                                        {
                                            case 1:
                                                {
                                                    msg = "う、うわああああっ！";
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    msg = "うわあああっ！";
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    msg = "ひ、ひいいいっ！";
                                                    break;
                                                }

                                            case 4:
                                                {
                                                    msg = "ひいいいっ！";
                                                    break;
                                                }
                                        }

                                        break;
                                    }

                                case "女性":
                                    {
                                        switch (GeneralLib.Dice(4))
                                        {
                                            case 1:
                                                {
                                                    msg = "きゃあああああっ！";
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    msg = "きゃああっ！";
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    msg = "うわあああっ！";
                                                    break;
                                                }

                                            case 4:
                                                {
                                                    msg = "た、助けてええっ！";
                                                    break;
                                                }
                                        }

                                        break;
                                    }
                            }

                            if (!string.IsNullOrEmpty(msg))
                            {
                                if (SRC.NPDList.IsDefined(MainPilot().Name + "(泣き)"))
                                {
                                    GUI.DisplayBattleMessage(MainPilot().Name + "(泣き)", msg, msg_mode: "");
                                }
                                else if (SRC.NPDList.IsDefined(MainPilot().Name + "(ダメージ)"))
                                {
                                    GUI.DisplayBattleMessage(MainPilot().Name + "(ダメージ)", msg, msg_mode: "");
                                }
                                else
                                {
                                    GUI.DisplayBattleMessage(MainPilot().ID, msg, msg_mode: "");
                                }
                            }
                        }

                        return;
                    }
                }

                if (IsConditionSatisfied("混乱"))
                {
                    if (IsMessageDefined("混乱"))
                    {
                        // 混乱状態用メッセージが定義されていればそちらを使う
                        situations.Clear();
                        situations.Add("混乱");
                    }
                }
            }

            // ダイアログデータを使って判定
            var pnames = new List<string>();
            // [0] パイロット全員とサポート全員が指定されている
            pnames.Add(string.Join(" ", AllPilots.Select(x => x.MessageType)));
            // [1] パイロット全員が指定されている
            pnames.Add(string.Join(" ", MainPilots.Select(x => x.MessageType)));
            // [2] メインパイロットが指定されている
            if (Situation == Commands.SelectedSpecialPower)
            {
                pnames.Add(Commands.SelectedPilot.MessageType);
            }
            else
            {
                pnames.Add(MainPilot().MessageType);
            }
            // [3]
            if (IsFeatureAvailable("追加パイロット"))
            {
                pnames.Add(Pilots.First().MessageType);
            }
            for (var i = 0; i < pnames.Count; i++)
            {
                var pname = pnames[i];
                // 追加パイロットにメッセージデータがあればそちらを優先
                if (i == pnames.Count - 1)
                {
                    if (SRC.MDList.IsDefined(MainPilot().MessageType))
                    {
                        break;
                    }
                }

                if (SRC.DDList.IsDefined(pname))
                {
                    var dd = SRC.DDList.Item(pname);

                    if (!string.IsNullOrEmpty(selected_msg))
                    {
                        // SelectMessageで選択されたメッセージを検索
                        var k = 0;
                        foreach (var di in dd.Items)
                        {
                            if (di.Situation == selected_situation)
                            {
                                k = (k + 1);
                                if ("" + k == selected_msg)
                                {
                                    PlayDialog(di, wname);
                                    return;
                                }
                            }
                            else if (di.Situation == selected_msg)
                            {
                                PlayDialog(di, wname);
                                return;
                            }
                        }
                    }
                    else
                    {
                        // メッセージデータからメッセージを検索
                        foreach (var situation in situations)
                        {
                            var di = dd.SelectDialog(situation, this);
                            if (di != null)
                            {
                                PlayDialog(di, wname);
                                return;
                            }
                        }
                    }
                }
            }

            // ゲッターのようなユニットは必ずメインパイロットを使ってメッセージを表示
            Pilot p;
            if (Data.PilotNum > 0
                && ReferenceEquals(MainPilot(), Pilots.First())
                && (Situation ?? "") != (Commands.SelectedSpecialPower ?? ""))
            {
                p = AllPilots.ToList().Dice();
            }
            else
            {
                p = MainPilot();
            }

        TryAgain:
            ;

            // 選んだパイロットによるメッセージデータで判定
            if ((Situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
            {
                if (!SRC.MDList.IsDefined(Commands.SelectedPilot.MessageType))
                {
                    goto TrySelectedMessage;
                }
            }
            else if (p == MainPilot())
            {
                if (!SRC.MDList.IsDefined(p.MessageType) && !SRC.MDList.IsDefined(Pilots.First().MessageType))
                {
                    goto TrySelectedMessage;
                }
            }
            else
            {
                if (!SRC.MDList.IsDefined(p.MessageType))
                {
                    p = MainPilot();
                    goto TryAgain;
                }
            }

            // メッセージを表示
            MessageData md;
            if ((Situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
            {
                md = SRC.MDList.Item(Commands.SelectedPilot.MessageType);
                p = Commands.SelectedPilot;
            }
            else if (p == MainPilot())
            {
                md = SRC.MDList.Item(MainPilot().MessageType);
                p = MainPilot();
                if (md == null)
                {
                    md = SRC.MDList.Item(Pilots.First().MessageType);
                    p = Pilots.First();
                }
            }
            else
            {
                md = SRC.MDList.Item(Pilots.First().MessageType);
            }

            // メッセージデータが見つからない場合は他のパイロットで探しなおす
            if (md is null)
            {
                if (p != MainPilot() && p != Pilots.First())
                {
                    p = MainPilot();
                    goto TryAgain;
                }
                else
                {
                    goto TrySelectedMessage;
                }
            }

            if (!string.IsNullOrEmpty(selected_msg))
            {
                // SelectMessageで選択されたメッセージを検索
                var k = 0;
                foreach (var mi in md.Items)
                {
                    if (mi.Situation == selected_situation)
                    {
                        k = (k + 1);
                        if ("" + k == selected_msg)
                        {
                            PlayMessage(p, mi.Message, wname);
                            return;
                        }
                    }
                    else if (mi.Situation == selected_msg)
                    {
                        PlayMessage(p, mi.Message, wname);
                        return;
                    }
                }
            }
            else
            {
                // メッセージデータからメッセージを検索
                foreach (var situation in situations)
                {
                    var msg = md.SelectMessage(situation, this);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        PlayMessage(p, msg, wname);
                        return;
                    }
                }
            }

            if (p != MainPilot() && p != Pilots.First())
            {
                p = MainPilot();
                goto TryAgain;
            }

        TrySelectedMessage:
            ;

            // メッセージを表示
            if (!string.IsNullOrEmpty(selected_msg) && selected_msg != "-")
            {
                if (!string.IsNullOrEmpty(selected_pilot))
                {
                    GUI.DisplayBattleMessage(selected_pilot, selected_msg, msg_mode: "");
                }
                else
                {
                    GUI.DisplayBattleMessage(MainPilot().ID, selected_msg, msg_mode: "");
                }
            }
        }

        // ダイアログの再生
        public void PlayDialog(Dialog dd, string wname)
        {
            // TODO Impl
            //string msg, buf;
            //int i, idx;
            //Unit t;
            //int tw;

            // 画像描画が行われたかどうかの判定のためにフラグを初期化
            GUI.IsPictureDrawn = false;
            // ダイアログの個々のメッセージを表示
            foreach (var di in dd.Items)
            {
                var msg = di.strMessage;

                // メッセージ表示のキャンセル
                if (msg == "-")
                {
                    return;
                }

                //// ユニット名
                //while (Strings.InStr(msg, "$(ユニット)") > 0)
                //{
                //    idx = Strings.InStr(msg, "$(ユニット)");
                //    buf = Nickname;
                //    if (Strings.InStr(buf, "(") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //    }

                //    if (Strings.InStr(buf, "専用") > 0)
                //    {
                //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
                //    }

                //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
                //}

                //while (Strings.InStr(msg, "$(機体)") > 0)
                //{
                //    idx = Strings.InStr(msg, "$(機体)");
                //    buf = Nickname;
                //    if (Strings.InStr(buf, "(") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //    }

                //    if (Strings.InStr(buf, "専用") > 0)
                //    {
                //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
                //    }

                //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
                //}

                //// パイロット名
                //while (Strings.InStr(msg, "$(パイロット)") > 0)
                //{
                //    buf = MainPilot().get_Nickname(false);
                //    if (Strings.InStr(buf, "(") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //    }

                //    idx = Strings.InStr(msg, "$(パイロット)");
                //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
                //}

                //// 武器名
                //while (Strings.InStr(msg, "$(武器)") > 0)
                //{
                //    idx = Strings.InStr(msg, "$(武器)");
                //    buf = wname;
                //    Expression.ReplaceSubExpression(buf);
                //    if (Strings.InStr(buf, "(") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //    }

                //    if (Strings.InStr(buf, "<") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
                //    }

                //    if (Strings.InStr(buf, "＜") > 0)
                //    {
                //        buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
                //    }

                //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
                //}

                //// 損傷率
                //while (Strings.InStr(msg, "$(損傷率)") > 0)
                //{
                //    idx = Strings.InStr(msg, "$(損傷率)");
                //    msg = Strings.Left(msg, idx - 1) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, idx + 6);
                //}

                //// 相手ユニットを設定
                //if (ReferenceEquals(Commands.SelectedUnit, this))
                //{
                //    t = Commands.SelectedTarget;
                //    tw = Commands.SelectedTWeapon;
                //}
                //else
                //{
                //    t = Commands.SelectedUnit;
                //    tw = Commands.SelectedWeapon;
                //}

                //if (t is object)
                //{
                //    // 相手ユニット名
                //    while (Strings.InStr(msg, "$(相手ユニット)") > 0)
                //    {
                //        buf = t.Nickname;
                //        if (Strings.InStr(buf, "(") > 0)
                //        {
                //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //        }

                //        if (Strings.InStr(buf, "専用") > 0)
                //        {
                //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
                //        }

                //        idx = Strings.InStr(msg, "$(相手ユニット)");
                //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 9);
                //    }

                //    while (Strings.InStr(msg, "$(相手機体)") > 0)
                //    {
                //        buf = t.Nickname;
                //        if (Strings.InStr(buf, "(") > 0)
                //        {
                //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //        }

                //        if (Strings.InStr(buf, "専用") > 0)
                //        {
                //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
                //        }

                //        idx = Strings.InStr(msg, "$(相手機体)");
                //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
                //    }

                //    // 相手パイロット名
                //    while (Strings.InStr(msg, "$(相手パイロット)") > 0)
                //    {
                //        buf = t.MainPilot().get_Nickname(false);
                //        if (Strings.InStr(buf, "(") > 0)
                //        {
                //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                //        }

                //        idx = Strings.InStr(msg, "$(相手パイロット)");
                //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 10);
                //    }

                //    // 相手武器名
                //    while (Strings.InStr(msg, "$(相手武器)") > 0)
                //    {
                //        if (1 <= tw && tw <= t.CountWeapon())
                //        {
                //            buf = t.WeaponNickname(tw);
                //        }
                //        else
                //        {
                //            buf = "";
                //        }

                //        if (Strings.InStr(buf, "<") > 0)
                //        {
                //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
                //        }

                //        if (Strings.InStr(buf, "＜") > 0)
                //        {
                //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
                //        }

                //        idx = Strings.InStr(msg, "$(相手武器)");
                //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
                //    }

                //    // 相手損傷率
                //    while (Strings.InStr(msg, "$(相手損傷率)") > 0)
                //    {
                //        buf = SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP);
                //        idx = Strings.InStr(msg, "$(相手損傷率)");
                //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
                //    }
                //}

                // メッセージを表示
                if (di.strName == MainPilot().Name)
                {
                    GUI.DisplayBattleMessage(MainPilot().ID, msg, msg_mode: "");
                }
                else if (Strings.Left(di.strName, 1) == "@")
                {
                    GUI.DisplayBattleMessage(Strings.Mid(di.strName, 2), msg, msg_mode: "");
                }
                else
                {
                    GUI.DisplayBattleMessage(di.strName, msg, msg_mode: "");
                }
            }

            // カットインは消去しておく
            if (!Expression.IsOptionDefined("戦闘中画面初期化無効"))
            {
                if (GUI.IsPictureDrawn)
                {
                    GUI.ClearPicture();
                    GUI.UpdateScreen();
                }
            }
        }

        // メッセージを再生
        public void PlayMessage(Pilot p, string msg, string wname)
        {
            // メッセージ表示をキャンセル
            if (msg == "-")
            {
                return;
            }

            // 画像描画が行われたかどうかの判定のためにフラグを初期化
            GUI.IsPictureDrawn = false;

            // TODO Impl
            //// ユニット名
            //while (Strings.InStr(msg, "$(ユニット)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(ユニット)");
            //    buf = Nickname;
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "専用") > 0)
            //    {
            //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //}

            //while (Strings.InStr(msg, "$(機体)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(機体)");
            //    buf = Nickname;
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "専用") > 0)
            //    {
            //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //}

            //// パイロット名
            //while (Strings.InStr(msg, "$(パイロット)") > 0)
            //{
            //    buf = MainPilot().get_Nickname(false);
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    idx = Strings.InStr(msg, "$(パイロット)");
            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //}

            //// 武器名
            //while (Strings.InStr(msg, "$(武器)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(武器)");
            //    buf = wname;
            //    Expression.ReplaceSubExpression(ref buf);
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "<") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //    }

            //    if (Strings.InStr(buf, "＜") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //}

            //// 損傷率
            //while (Strings.InStr(msg, "$(損傷率)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(損傷率)");
            //    msg = Strings.Left(msg, idx - 1) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, idx + 6);
            //}

            //// 相手ユニットを設定
            //if (ReferenceEquals(Commands.SelectedUnit, this))
            //{
            //    t = Commands.SelectedTarget;
            //    tw = Commands.SelectedTWeapon;
            //}
            //else
            //{
            //    t = Commands.SelectedUnit;
            //    tw = Commands.SelectedWeapon;
            //}

            //if (t is object)
            //{
            //    // 相手ユニット名
            //    while (Strings.InStr(msg, "$(相手ユニット)") > 0)
            //    {
            //        buf = t.Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手ユニット)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 9);
            //    }

            //    while (Strings.InStr(msg, "$(相手機体)") > 0)
            //    {
            //        buf = t.Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手機体)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //    }

            //    // 相手パイロット名
            //    while (Strings.InStr(msg, "$(相手パイロット)") > 0)
            //    {
            //        buf = t.MainPilot().get_Nickname(false);
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手パイロット)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 10);
            //    }

            //    // 相手武器名
            //    while (Strings.InStr(msg, "$(相手武器)") > 0)
            //    {
            //        if (1 <= tw && tw <= t.CountWeapon())
            //        {
            //            buf = t.WeaponNickname(tw);
            //        }
            //        else
            //        {
            //            buf = "";
            //        }

            //        if (Strings.InStr(buf, "<") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //        }

            //        if (Strings.InStr(buf, "＜") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手武器)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //    }

            //    // 相手損傷率
            //    while (Strings.InStr(msg, "$(相手損傷率)") > 0)
            //    {
            //        buf = SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP);
            //        idx = Strings.InStr(msg, "$(相手損傷率)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //    }
            //}

            // メッセージを表示
            GUI.DisplayBattleMessage(p.ID, msg, msg_mode: "");

            // カットインは消去しておく
            if (!Expression.IsOptionDefined("戦闘中画面初期化無効"))
            {
                if (GUI.IsPictureDrawn)
                {
                    GUI.ClearPicture();
                    GUI.UpdateScreen();
                }
            }
        }

        // シチュエーション main_situation に対応するメッセージが定義されているか
        public bool IsMessageDefined(string main_situation, bool ignore_condition = false)
        {

            // しゃべれない場合
            if (!ignore_condition)
            {
                if (IsConditionSatisfied("沈黙") || IsConditionSatisfied("憑依") || IsConditionSatisfied("石化") || IsConditionSatisfied("凍結") || IsConditionSatisfied("麻痺") || IsConditionSatisfied("睡眠"))
                {
                    return false;
                }

                // 特殊状態用メッセージが定義されているか確認する場合を考慮
                if (IsConditionSatisfied("恐怖") && main_situation != "恐怖")
                {
                    return false;
                }

                if (IsConditionSatisfied("混乱") && main_situation != "混乱")
                {
                    return false;
                }
            }

            // SetMessageコマンドでメッセージが設定されているか判定
            if (Expression.IsLocalVariableDefined("Message(" + MainPilot().ID + "," + main_situation + ")"))
            {
                return true;
            }

            var pnames = new List<string>();
            // [0] パイロット全員とサポート全員が指定されている
            pnames.Add(string.Join(" ", AllPilots.Select(x => x.MessageType)));
            // [1] パイロット全員が指定されている
            pnames.Add(string.Join(" ", MainPilots.Select(x => x.MessageType)));
            // [2] メインパイロットが指定されている
            if (!string.IsNullOrEmpty(main_situation) && main_situation == Commands.SelectedSpecialPower)
            {
                pnames.Add(Commands.SelectedPilot.MessageType);
            }
            else
            {
                pnames.Add(MainPilot().MessageType);
            }
            // [3]
            if (IsFeatureAvailable("追加パイロット"))
            {
                pnames.Add(Pilots.First().MessageType);
            }

            foreach (var pname in pnames)
            {
                if (SRC.DDList.IsDefined(pname))
                {
                    var dd = SRC.DDList.Item(pname);
                    if (dd.SelectDialog(main_situation, this, ignore_condition) != null)
                    {
                        return true;
                    }
                }
            }

            // メッセージデータを使って判定
            var msg = "";
            if ((main_situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
            {
                if (SRC.MDList.IsDefined(Commands.SelectedPilot.MessageType))
                {
                    msg = SRC.MDList.Item(Commands.SelectedPilot.MessageType).SelectMessage(main_situation, this);
                }
            }
            else
            {
                {
                    var p = MainPilot();
                    if (SRC.MDList.IsDefined(p.MessageType))
                    {
                        msg = SRC.MDList.Item(p.MessageType).SelectMessage(main_situation, this);
                    }
                }

                if (Strings.Len(msg) == 0)
                {
                    var p = Pilots.First();
                    if (SRC.MDList.IsDefined(p.MessageType))
                    {
                        msg = SRC.MDList.Item(p.MessageType).SelectMessage(main_situation, this);
                    }
                }
            }

            if (Strings.Len(msg) > 0)
            {
                return true;
            }

            return false;
        }

        // 解説メッセージを表示
        public void SysMessage(string main_situation, string sub_situation = "", string add_msg = "")
        {
            // TODO Impl
            //    string uname, msg, uclass;
            //    string[] situations;
            //    string idx, buf;
            //    int i, ret;
            //    string wname;
            //    if (string.IsNullOrEmpty(sub_situation) || (main_situation ?? "") == (sub_situation ?? ""))
            //    {
            //        situations = new string[2];
            //        situations.Add(main_situation + "(解説)");
            //    }
            //    else
            //    {
            //        situations = new string[3];
            //        situations.Add(main_situation + "(" + sub_situation + ")(解説)");
            //        situations.Add(main_situation + "(解説)");
            //    }

            //    // ADD START MARGE
            //    // 拡張戦闘アニメデータで検索
            //    if (SRC.ExtendedAnimation)
            //    {
            //        {
            //            var withBlock = SRC.EADList;
            //            var loopTo = Information.UBound(situations);
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                // 戦闘アニメ能力で指定された名称で検索
            //                if (IsFeatureAvailable("戦闘アニメ"))
            //                {
            //                    uname = FeatureData("戦闘アニメ");
            //                    if (withBlock.IsDefined(uname))
            //                    {
            //                        MessageData localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                        msg = localItem().SelectMessage(situations[i], this);
            //                        if (Strings.Len(msg) > 0)
            //                        {
            //                            goto FoundMessage;
            //                        }
            //                    }
            //                }

            //                // ユニット名称で検索
            //                bool localIsDefined() { object argIndex1 = Name; var ret = withBlock.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                if (localIsDefined())
            //                {
            //                    MessageData localItem1() { object argIndex1 = Name; var ret = withBlock.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                    msg = localItem1().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }

            //                // ユニット愛称を修正したもので検索
            //                uname = Nickname0;
            //                ret = Strings.InStr(uname, "(");
            //                if (ret > 1)
            //                {
            //                    uname = Strings.Left(uname, ret - 1);
            //                }

            //                ret = Strings.InStr(uname, "用");
            //                if (ret > 0)
            //                {
            //                    if (ret < Strings.Len(uname))
            //                    {
            //                        uname = Strings.Mid(uname, ret + 1);
            //                    }
            //                }

            //                ret = Strings.InStr(uname, "型");
            //                if (ret > 0)
            //                {
            //                    if (ret < Strings.Len(uname))
            //                    {
            //                        uname = Strings.Mid(uname, ret + 1);
            //                    }
            //                }

            //                if (Strings.Right(uname, 4) == "カスタム")
            //                {
            //                    uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //                }

            //                if (Strings.Right(uname, 1) == "改")
            //                {
            //                    uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //                }

            //                if (withBlock.IsDefined(uname))
            //                {
            //                    MessageData localItem2() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                    msg = localItem2().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }

            //                // ユニットクラスで検索
            //                uclass = Class0;
            //                if (withBlock.IsDefined(uclass))
            //                {
            //                    MessageData localItem3() { object argIndex1 = uclass; var ret = withBlock.Item(argIndex1); return ret; }

            //                    msg = localItem3().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }

            //                // 汎用
            //                if (withBlock.IsDefined("汎用"))
            //                {
            //                    msg = withBlock.Item("汎用").SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    // ADD END MARGE

            //    // 戦闘アニメデータで検索
            //    {
            //        var withBlock1 = SRC.ADList;
            //        var loopTo1 = Information.UBound(situations);
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            // 戦闘アニメ能力で指定された名称で検索
            //            if (IsFeatureAvailable("戦闘アニメ"))
            //            {
            //                uname = FeatureData("戦闘アニメ");
            //                if (withBlock1.IsDefined(uname))
            //                {
            //                    MessageData localItem4() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //                    msg = localItem4().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }
            //            }

            //            // ユニット名称で検索
            //            bool localIsDefined1() { object argIndex1 = Name; var ret = withBlock1.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined1())
            //            {
            //                MessageData localItem5() { object argIndex1 = Name; var ret = withBlock1.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                msg = localItem5().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // ユニット愛称を修正したもので検索
            //            uname = Nickname0;
            //            ret = Strings.InStr(uname, "(");
            //            if (ret > 1)
            //            {
            //                uname = Strings.Left(uname, ret - 1);
            //            }

            //            ret = Strings.InStr(uname, "用");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            ret = Strings.InStr(uname, "型");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            if (Strings.Right(uname, 4) == "カスタム")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //            }

            //            if (Strings.Right(uname, 1) == "改")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //            }

            //            if (withBlock1.IsDefined(uname))
            //            {
            //                MessageData localItem6() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //                msg = localItem6().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // ユニットクラスで検索
            //            uclass = Class0;
            //            if (withBlock1.IsDefined(uclass))
            //            {
            //                MessageData localItem7() { object argIndex1 = uclass; var ret = withBlock1.Item(argIndex1); return ret; }

            //                msg = localItem7().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // 汎用
            //            if (withBlock1.IsDefined("汎用"))
            //            {
            //                msg = withBlock1.Item("汎用").SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }
            //        }
            //    }

            //    // 特殊効果データで検索
            //    {
            //        var withBlock2 = SRC.EDList;
            //        var loopTo2 = Information.UBound(situations);
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            // 特殊効果能力で指定された名称で検索
            //            if (IsFeatureAvailable("特殊効果"))
            //            {
            //                uname = FeatureData("特殊効果");
            //                if (withBlock2.IsDefined(uname))
            //                {
            //                    MessageData localItem8() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //                    msg = localItem8().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        goto FoundMessage;
            //                    }
            //                }
            //            }

            //            // ユニット名称で検索
            //            bool localIsDefined2() { object argIndex1 = Name; var ret = withBlock2.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined2())
            //            {
            //                MessageData localItem9() { object argIndex1 = Name; var ret = withBlock2.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                msg = localItem9().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // ユニット愛称を修正したもので検索
            //            uname = Nickname0;
            //            ret = Strings.InStr(uname, "(");
            //            if (ret > 1)
            //            {
            //                uname = Strings.Left(uname, ret - 1);
            //            }

            //            ret = Strings.InStr(uname, "用");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            ret = Strings.InStr(uname, "型");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            if (Strings.Right(uname, 4) == "カスタム")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //            }

            //            if (Strings.Right(uname, 1) == "改")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //            }

            //            if (withBlock2.IsDefined(uname))
            //            {
            //                MessageData localItem10() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //                msg = localItem10().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // ユニットクラスで検索
            //            uclass = Class0;
            //            if (withBlock2.IsDefined(uclass))
            //            {
            //                MessageData localItem11() { object argIndex1 = uclass; var ret = withBlock2.Item(argIndex1); return ret; }

            //                msg = localItem11().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }

            //            // 汎用
            //            if (withBlock2.IsDefined("汎用"))
            //            {
            //                msg = withBlock2.Item("汎用").SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    goto FoundMessage;
            //                }
            //            }
            //        }
            //    }

            //    // 対応するメッセージが見つからなかった
            //    return;
            //FoundMessage:
            //    ;


            //    // メッセージ表示のキャンセル
            //    if (msg == "-")
            //    {
            //        return;
            //    }

            //    // ユニット名
            //    while (Strings.InStr(msg, "$(ユニット)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(ユニット)").ToString();
            //        buf = Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
            //    }

            //    while (Strings.InStr(msg, "$(機体)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(機体)").ToString();
            //        buf = Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 5d));
            //    }

            //    // パイロット名
            //    while (Strings.InStr(msg, "$(パイロット)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(パイロット)").ToString();
            //        buf = MainPilot().get_Nickname(false);
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 8d));
            //    }

            //    // 武器名
            //    if (Strings.InStr(msg, "$(武器)") > 0)
            //    {
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            wname = Weapon(Commands.SelectedWeapon).Name;
            //        }
            //        else
            //        {
            //            wname = Weapon(Commands.SelectedTWeapon).Name;
            //        }

            //        if (Strings.InStr(wname, "(") > 0)
            //        {
            //            wname = Strings.Left(wname, Strings.InStr(wname, "(") - 1);
            //        }

            //        if (Strings.InStr(wname, "<") > 0)
            //        {
            //            wname = Strings.Left(wname, Strings.InStr(wname, "<") - 1);
            //        }

            //        while (Strings.InStr(msg, "$(武器)") > 0)
            //        {
            //            idx = Strings.InStr(msg, "$(武器)").ToString();
            //            msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + wname + Strings.Mid(msg, (Conversions.ToDouble(idx) + 5d));
            //        }
            //    }

            //    // 損傷率
            //    while (Strings.InStr(msg, "$(損傷率)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(損傷率)").ToString();
            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, (Conversions.ToDouble(idx) + 6d));
            //    }

            //    // 相手ユニット名
            //    while (Strings.InStr(msg, "$(相手ユニット)") > 0)
            //    {
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            if (Commands.SelectedTarget is object)
            //            {
            //                buf = Commands.SelectedTarget.Nickname;
            //            }
            //            else
            //            {
            //                buf = "";
            //            }
            //        }
            //        else
            //        {
            //            buf = Commands.SelectedUnit.Nickname;
            //        }

            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手ユニット)").ToString();
            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 9d));
            //    }

            //    while (Strings.InStr(msg, "$(相手機体)") > 0)
            //    {
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            if (Commands.SelectedTarget is object)
            //            {
            //                buf = Commands.SelectedTarget.Nickname;
            //            }
            //            else
            //            {
            //                buf = "";
            //            }
            //        }
            //        else
            //        {
            //            buf = Commands.SelectedUnit.Nickname;
            //        }

            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手機体)").ToString();
            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
            //    }

            //    // 相手パイロット名
            //    while (Strings.InStr(msg, "$(相手パイロット)") > 0)
            //    {
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            if (Commands.SelectedTarget is object)
            //            {
            //                buf = Commands.SelectedTarget.MainPilot().get_Nickname(false);
            //            }
            //            else
            //            {
            //                buf = "";
            //            }
            //        }
            //        else
            //        {
            //            buf = Commands.SelectedUnit.MainPilot().get_Nickname(false);
            //        }

            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手パイロット)").ToString();
            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 10d));
            //    }

            //    // 相手武器名
            //    while (Strings.InStr(msg, "$(相手武器)") > 0)
            //    {
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            if (Commands.SelectedTarget is object)
            //            {
            //                buf = Commands.SelectedTarget.Weapon(Commands.SelectedTWeapon).Name;
            //            }
            //            else
            //            {
            //                buf = "";
            //            }
            //        }
            //        else
            //        {
            //            buf = Commands.SelectedUnit.Weapon(Commands.SelectedWeapon).Name;
            //        }

            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "<") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //        }

            //        if (Strings.InStr(buf, "＜") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手武器)").ToString();
            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
            //    }

            //    // 相手損傷率
            //    while (Strings.InStr(msg, "$(相手損傷率)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(相手損傷率)").ToString();
            //        if (ReferenceEquals(Commands.SelectedUnit, this))
            //        {
            //            if (Commands.SelectedTarget is object)
            //            {
            //                {
            //                    var withBlock3 = Commands.SelectedTarget;
            //                    buf = SrcFormatter.Format(100 * (withBlock3.MaxHP - withBlock3.HP) / withBlock3.MaxHP);
            //                }
            //            }
            //            else
            //            {
            //                buf = "";
            //            }
            //        }
            //        else
            //        {
            //            {
            //                var withBlock4 = Commands.SelectedUnit;
            //                buf = SrcFormatter.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP);
            //            }
            //        }

            //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 8d));
            //    }

            //    if (!My.MyProject.Forms.frmMessage.Visible)
            //    {
            //        GUI.OpenMessageForm(u1: null, u2: null);
            //    }

            //    if (!string.IsNullOrEmpty(add_msg))
            //    {
            //        GUI.DisplayBattleMessage("-", msg + "." + add_msg, msg_mode: "");
            //    }
            //    else
            //    {
            //        GUI.DisplayBattleMessage("-", msg, msg_mode: "");
            //    }
        }

        // 解説メッセージが定義されているか？
        public bool IsSysMessageDefined(string main_situation, string sub_situation = "")
        {
            return false;
            // TODO Impl
            //bool IsSysMessageDefinedRet = default;
            //string uclass, uname, msg;
            //string[] situations;
            //int i, ret;
            //if (string.IsNullOrEmpty(sub_situation) || (main_situation ?? "") == (sub_situation ?? ""))
            //{
            //    situations = new string[2];
            //    situations.Add(main_situation + "(解説)");
            //}
            //else
            //{
            //    situations = new string[3];
            //    situations.Add(main_situation + "(" + sub_situation + ")(解説)");
            //    situations.Add(main_situation + "(解説)");
            //}

            //// ADD START MARGE
            //// 拡張戦闘アニメデータで検索
            //if (SRC.ExtendedAnimation)
            //{
            //    {
            //        var withBlock = SRC.EADList;
            //        var loopTo = Information.UBound(situations);
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            // 戦闘アニメ能力で指定された名称で検索
            //            if (IsFeatureAvailable("戦闘アニメ"))
            //            {
            //                uname = FeatureData("戦闘アニメ");
            //                if (withBlock.IsDefined(uname))
            //                {
            //                    MessageData localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                    msg = localItem().SelectMessage(situations[i], this);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        IsSysMessageDefinedRet = true;
            //                        return IsSysMessageDefinedRet;
            //                    }
            //                }
            //            }

            //            // ユニット名称で検索
            //            bool localIsDefined() { object argIndex1 = Name; var ret = withBlock.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined())
            //            {
            //                MessageData localItem1() { object argIndex1 = Name; var ret = withBlock.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                msg = localItem1().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // ユニット愛称を修正したもので検索
            //            uname = Nickname0;
            //            ret = Strings.InStr(uname, "(");
            //            if (ret > 1)
            //            {
            //                uname = Strings.Left(uname, ret - 1);
            //            }

            //            ret = Strings.InStr(uname, "用");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            ret = Strings.InStr(uname, "型");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            if (Strings.Right(uname, 4) == "カスタム")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //            }

            //            if (Strings.Right(uname, 1) == "改")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //            }

            //            if (withBlock.IsDefined(uname))
            //            {
            //                MessageData localItem2() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                msg = localItem2().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // ユニットクラスで検索
            //            uclass = Class0;
            //            if (withBlock.IsDefined(uclass))
            //            {
            //                MessageData localItem3() { object argIndex1 = uclass; var ret = withBlock.Item(argIndex1); return ret; }

            //                msg = localItem3().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // 汎用
            //            if (withBlock.IsDefined("汎用"))
            //            {
            //                msg = withBlock.Item("汎用").SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }
            //    }
            //}
            //// ADD END MARGE

            //// 戦闘アニメデータで検索
            //{
            //    var withBlock1 = SRC.ADList;
            //    var loopTo1 = Information.UBound(situations);
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        // 戦闘アニメ能力で指定された名称で検索
            //        if (IsFeatureAvailable("戦闘アニメ"))
            //        {
            //            uname = FeatureData("戦闘アニメ");
            //            if (withBlock1.IsDefined(uname))
            //            {
            //                MessageData localItem4() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //                msg = localItem4().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }

            //        // ユニット名称で検索
            //        bool localIsDefined1() { object argIndex1 = Name; var ret = withBlock1.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined1())
            //        {
            //            MessageData localItem5() { object argIndex1 = Name; var ret = withBlock1.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            msg = localItem5().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニット愛称を修正したもので検索
            //        uname = Nickname0;
            //        ret = Strings.InStr(uname, "(");
            //        if (ret > 1)
            //        {
            //            uname = Strings.Left(uname, ret - 1);
            //        }

            //        ret = Strings.InStr(uname, "用");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        ret = Strings.InStr(uname, "型");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        if (Strings.Right(uname, 4) == "カスタム")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //        }

            //        if (Strings.Right(uname, 1) == "改")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //        }

            //        if (withBlock1.IsDefined(uname))
            //        {
            //            MessageData localItem6() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //            msg = localItem6().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニットクラスで検索
            //        uclass = Class0;
            //        if (withBlock1.IsDefined(uclass))
            //        {
            //            MessageData localItem7() { object argIndex1 = uclass; var ret = withBlock1.Item(argIndex1); return ret; }

            //            msg = localItem7().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // 汎用
            //        if (withBlock1.IsDefined("汎用"))
            //        {
            //            msg = withBlock1.Item("汎用").SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }
            //    }
            //}

            //// 特殊効果データで検索
            //{
            //    var withBlock2 = SRC.EDList;
            //    var loopTo2 = Information.UBound(situations);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        // 特殊効果能力で指定された名称で検索
            //        if (IsFeatureAvailable("特殊効果"))
            //        {
            //            uname = FeatureData("特殊効果");
            //            if (withBlock2.IsDefined(uname))
            //            {
            //                MessageData localItem8() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //                msg = localItem8().SelectMessage(situations[i], this);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }

            //        // ユニット名称で検索
            //        bool localIsDefined2() { object argIndex1 = Name; var ret = withBlock2.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined2())
            //        {
            //            MessageData localItem9() { object argIndex1 = Name; var ret = withBlock2.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            msg = localItem9().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニット愛称を修正したもので検索
            //        uname = Nickname0;
            //        ret = Strings.InStr(uname, "(");
            //        if (ret > 1)
            //        {
            //            uname = Strings.Left(uname, ret - 1);
            //        }

            //        ret = Strings.InStr(uname, "用");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        ret = Strings.InStr(uname, "型");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        if (Strings.Right(uname, 4) == "カスタム")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //        }

            //        if (Strings.Right(uname, 1) == "改")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //        }

            //        if (withBlock2.IsDefined(uname))
            //        {
            //            MessageData localItem10() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //            msg = localItem10().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニットクラスで検索
            //        uclass = Class0;
            //        if (withBlock2.IsDefined(uclass))
            //        {
            //            MessageData localItem11() { object argIndex1 = uclass; var ret = withBlock2.Item(argIndex1); return ret; }

            //            msg = localItem11().SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // 汎用
            //        if (withBlock2.IsDefined("汎用"))
            //        {
            //            msg = withBlock2.Item("汎用").SelectMessage(situations[i], this);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }
            //    }
            //}

            //IsSysMessageDefinedRet = false;
            //return IsSysMessageDefinedRet;
        }
    }
}
