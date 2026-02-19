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
        public void PilotMassageIfDefined(string[] situations)
        {
            foreach (var situation in situations)
            {
                if (IsMessageDefined(situation))
                {
                    GUI.Center(x, y);
                    GUI.RefreshScreen();
                    GUI.OpenMessageForm(u1: null, u2: null);

                    PilotMessage(situation);

                    GUI.CloseMessageForm();
                    return;
                }
            }
        }

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

        // メッセージ中の変数を置換する
        private string SubstituteMessageVariables(string msg, string wname)
        {
            // ユニット名 / 機体
            if (msg.IndexOf("$(ユニット)") >= 0 || msg.IndexOf("$(機体)") >= 0)
            {
                var buf = Nickname;
                if (buf.IndexOf('(') >= 0)
                {
                    buf = buf.Substring(0, buf.IndexOf('('));
                }
                if (buf.IndexOf("専用") >= 0)
                {
                    buf = buf.Substring(buf.IndexOf("専用") + 2);
                }
                msg = msg.Replace("$(ユニット)", buf).Replace("$(機体)", buf);
            }

            // パイロット名
            if (msg.IndexOf("$(パイロット)") >= 0)
            {
                var buf = MainPilot().get_Nickname(false);
                if (buf.IndexOf('(') >= 0)
                {
                    buf = buf.Substring(0, buf.IndexOf('('));
                }
                msg = msg.Replace("$(パイロット)", buf);
            }

            // 武器名
            if (msg.IndexOf("$(武器)") >= 0)
            {
                var buf = wname;
                Expression.ReplaceSubExpression(ref buf);
                if (buf.IndexOf('(') >= 0)
                {
                    buf = buf.Substring(0, buf.IndexOf('('));
                }
                if (buf.IndexOf('<') >= 0)
                {
                    buf = buf.Substring(0, buf.IndexOf('<'));
                }
                if (buf.IndexOf('＜') >= 0)
                {
                    buf = buf.Substring(0, buf.IndexOf('＜'));
                }
                msg = msg.Replace("$(武器)", buf);
            }

            // 損傷率
            if (msg.IndexOf("$(損傷率)") >= 0)
            {
                msg = msg.Replace("$(損傷率)", SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP));
            }

            // 相手ユニットを設定
            Unit t;
            int tw;
            if (ReferenceEquals(Commands.SelectedUnit, this))
            {
                t = Commands.SelectedTarget;
                tw = Commands.SelectedTWeapon;
            }
            else
            {
                t = Commands.SelectedUnit;
                tw = Commands.SelectedWeapon;
            }

            if (t is object)
            {
                // 相手ユニット名 / 相手機体
                if (msg.IndexOf("$(相手ユニット)") >= 0 || msg.IndexOf("$(相手機体)") >= 0)
                {
                    var buf = t.Nickname;
                    if (buf.IndexOf('(') >= 0)
                    {
                        buf = buf.Substring(0, buf.IndexOf('('));
                    }
                    if (buf.IndexOf("専用") >= 0)
                    {
                        buf = buf.Substring(buf.IndexOf("専用") + 2);
                    }
                    msg = msg.Replace("$(相手ユニット)", buf).Replace("$(相手機体)", buf);
                }

                // 相手パイロット名
                if (msg.IndexOf("$(相手パイロット)") >= 0)
                {
                    var buf = t.MainPilot().get_Nickname(false);
                    if (buf.IndexOf('(') >= 0)
                    {
                        buf = buf.Substring(0, buf.IndexOf('('));
                    }
                    msg = msg.Replace("$(相手パイロット)", buf);
                }

                // 相手武器名
                if (msg.IndexOf("$(相手武器)") >= 0)
                {
                    string buf;
                    if (1 <= tw && tw <= t.CountWeapon())
                    {
                        buf = t.Weapons[tw - 1].WeaponNickname();
                    }
                    else
                    {
                        buf = "";
                    }
                    if (buf.IndexOf('<') >= 0)
                    {
                        buf = buf.Substring(0, buf.IndexOf('<'));
                    }
                    if (buf.IndexOf('＜') >= 0)
                    {
                        buf = buf.Substring(0, buf.IndexOf('＜'));
                    }
                    msg = msg.Replace("$(相手武器)", buf);
                }

                // 相手損傷率
                if (msg.IndexOf("$(相手損傷率)") >= 0)
                {
                    msg = msg.Replace("$(相手損傷率)", SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP));
                }
            }

            return msg;
        }

        // ダイアログの再生
        public void PlayDialog(Dialog dd, string wname)
        {
            // 画像描画が行われたかどうかの判定のためにフラグを初期化
            GUI.IsPictureDrawn = false;
            // ダイアログの個々のメッセージを表示
            foreach (var di in dd.Items)
            {
                var msg = SubstituteMessageVariables(di.strMessage, wname);

                // メッセージ表示のキャンセル
                if (msg == "-")
                {
                    return;
                }

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

            msg = SubstituteMessageVariables(msg, wname);
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

        public bool SysMessageIfDefined(string[] situations)
        {
            foreach(var situation in situations)
            {
                if (IsSysMessageDefined(situation))
                {
                    SysMessage(situation);
                    return true;
                }
            }
            return false;
        }

        // 解説メッセージを表示
        public void SysMessage(string main_situation, string sub_situation = "", string add_msg = "")
        {
            string msg = FindSysMessage(main_situation, sub_situation);
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            // メッセージ表示のキャンセル
            if (msg == "-")
            {
                return;
            }

            // 変数を置換
            string wname = "";
            if (ReferenceEquals(Commands.SelectedUnit, this))
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
            msg = SubstituteMessageVariables(msg, wname);

            if (!string.IsNullOrEmpty(add_msg))
            {
                msg = msg + add_msg;
            }

            GUI.DisplayBattleMessage("-", msg, msg_mode: "");
        }

        // 解説メッセージが定義されているか？
        public bool IsSysMessageDefined(string main_situation, string sub_situation = "")
        {
            return !string.IsNullOrEmpty(FindSysMessage(main_situation, sub_situation));
        }

        // 解説メッセージを検索する
        private string FindSysMessage(string main_situation, string sub_situation = "")
        {
            var situations = new System.Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(sub_situation) || (main_situation ?? "") == (sub_situation ?? ""))
            {
                situations.Add(main_situation + "(解説)");
            }
            else
            {
                situations.Add(main_situation + "(" + sub_situation + ")(解説)");
                situations.Add(main_situation + "(解説)");
            }

            // ユニット愛称の正規化
            var uname = Nickname0;
            var ret = uname.IndexOf('(');
            if (ret > 0)
            {
                uname = uname.Substring(0, ret);
            }
            ret = uname.IndexOf("用");
            if (ret >= 0 && ret < uname.Length - 1)
            {
                uname = uname.Substring(ret + 1);
            }
            ret = uname.IndexOf("型");
            if (ret >= 0 && ret < uname.Length - 1)
            {
                uname = uname.Substring(ret + 1);
            }
            if (uname.EndsWith("カスタム"))
            {
                uname = uname.Substring(0, uname.Length - 4);
            }
            if (uname.EndsWith("改"))
            {
                uname = uname.Substring(0, uname.Length - 1);
            }

            string found;

            // 拡張戦闘アニメデータで検索
            if (SRC.ExtendedAnimation)
            {
                found = SearchMessageInList(SRC.EADList, situations, "戦闘アニメ", uname);
                if (found != null) return found;
            }

            // パイロットメッセージデータで検索
            found = SearchMessageInList(SRC.MDList, situations, "解説メッセージ", uname);
            if (found != null) return found;

            // 特殊効果データで検索
            found = SearchMessageInList(SRC.EDList, situations, "特殊効果", uname);
            if (found != null) return found;

            return null;
        }

        private string SearchMessageInList(Models.MessageDataList list, System.Collections.Generic.List<string> situations, string featureName, string normalizedName)
        {
            foreach (var situation in situations)
            {
                string msg;

                // 指定された特殊能力名で検索
                if (IsFeatureAvailable(featureName))
                {
                    var fname = FeatureData(featureName);
                    if (list.IsDefined(fname))
                    {
                        msg = list.Item(fname).SelectMessage(situation, this);
                        if (!string.IsNullOrEmpty(msg)) return msg;
                    }
                }

                // ユニット名称で検索
                if (list.IsDefined(Name))
                {
                    msg = list.Item(Name).SelectMessage(situation, this);
                    if (!string.IsNullOrEmpty(msg)) return msg;
                }

                // 正規化されたユニット愛称で検索
                if (list.IsDefined(normalizedName))
                {
                    msg = list.Item(normalizedName).SelectMessage(situation, this);
                    if (!string.IsNullOrEmpty(msg)) return msg;
                }

                // ユニットクラスで検索
                if (list.IsDefined(Class0))
                {
                    msg = list.Item(Class0).SelectMessage(situation, this);
                    if (!string.IsNullOrEmpty(msg)) return msg;
                }

                // 汎用
                if (list.IsDefined("汎用"))
                {
                    msg = list.Item("汎用").SelectMessage(situation, this);
                    if (!string.IsNullOrEmpty(msg)) return msg;
                }
            }
            return null;
        }

    }
}
