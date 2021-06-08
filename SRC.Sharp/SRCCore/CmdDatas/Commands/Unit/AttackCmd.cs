using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class AttackCmd : CmdData
    {
        public AttackCmd(SRC src, EventDataLine eventData) : base(src, CmdType.AttackCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var is_event = true;
            switch (ArgNum)
            {
                case 5:
                    break;
                // ＯＫ
                case 6:
                    if (GetArgAsString(6) == "通常戦闘")
                    {
                        is_event = false;
                    }
                    else
                    {
                        throw new EventErrorException(this, "Attackコマンドのオプションが不正です");
                    }
                    break;
                default:
                    throw new EventErrorException(this, "Attackコマンドの引数の数が違います");
            }

            var u1 = GetArgAsUnit(2);
            var u2 = GetArgAsUnit(4);
            var w1 = 0;
            var w2 = 0;
            if (u1.Status == "出撃" && u2.Status == "出撃")
            {
                if (GetArgAsString(3) == "自動")
                {
                    w1 = SRC.COM.SelectWeapon(u1, u2, "イベント", out _, out _);
                }
                else
                {
                    var loopTo = u1.CountWeapon();
                    for (w1 = 1; w1 <= loopTo; w1++)
                    {
                        if ((GetArgAsString(3) ?? "") == (u1.Weapon(w1).Name ?? "") && !u1.Weapon(w1).IsWeaponClassifiedAs("マップ攻撃"))
                        {
                            break;
                        }
                    }

                    if (w1 > u1.CountWeapon())
                    {
                        throw new EventErrorException(this, "ユニット「" + u1.Name + "」には武装「" + GetArgAsString(3) + "」は存在しません");
                    }
                }

                var def_option = GetArgAsString(5);
                var def_mode = "";
                switch (def_option ?? "")
                {
                    case "防御":
                    case "回避":
                    case "無抵抗":
                        def_mode = GetArgAsString(5);
                        break;

                    case "反撃不能":
                        def_mode = "反撃";
                        w2 = 0;
                        break;

                    case "自動":
                        def_mode = "反撃";
                        w2 = SRC.COM.SelectWeapon(u2, u1, "反撃 イベント", out _, out _);
                        break;

                    default:
                        def_mode = "反撃";
                        var loopTo1 = u2.CountWeapon();
                        for (w2 = 1; w2 <= loopTo1; w2++)
                        {
                            if ((GetArgAsString(5) ?? "") == (u2.Weapon(w2).Name ?? "") && !u2.Weapon(w2).IsWeaponClassifiedAs("マップ攻撃"))
                            {
                                break;
                            }
                        }

                        if (w2 > u2.CountWeapon())
                        {
                            throw new EventErrorException(this, "ユニット「" + u2.Name + "」には武装「" + GetArgAsString(5) + "」は存在しません");
                        }
                        break;
                }

                if (w1 > 0)
                {
                    // XXX Push/Pop
                    var prev_su = Commands.SelectedUnit;
                    var prev_st = Commands.SelectedTarget;
                    var prev_w = Commands.SelectedWeapon;
                    var prev_tw = Commands.SelectedTWeapon;
                    Commands.SelectedUnit = u1;
                    Commands.SelectedTarget = u2;
                    Commands.SelectedWeapon = w1;
                    Commands.SelectedTWeapon = w2;
                    if (u1.Party0 == "味方" || u1.Party0 == "ＮＰＣ")
                    {
                        GUI.OpenMessageForm(u2, u1);
                    }
                    else
                    {
                        GUI.OpenMessageForm(u1, u2);
                    }

                    // 攻撃を実行
                    var cur_stage = SRC.Stage;
                    SRC.Stage = u1.Party;
                    var uw1 = u1.Weapon(w1);
                    var uw2 = u2.Weapon(w2);
                    u1.Attack(uw1, u2, "", def_mode, is_event);
                    u1 = u1.CurrentForm();

                    // 反撃用武器がまだ使用可能かチェック
                    if (def_option == "自動" && u2.Status == "出撃")
                    {
                        if (!uw2.IsTargetWithinRange(u1) || !uw2.IsWeaponAvailable("移動前"))
                        {
                            w2 = SRC.COM.SelectWeapon(u2, u1, "反撃 イベント", out _, out _);
                            uw2 = u2.Weapon(w2);
                            Commands.SelectedTWeapon = w2;
                        }
                    }

                    // 反撃を実行
                    if (def_mode == "反撃" && u2.Status == "出撃" && u2.MaxAction() > 0 && !u2.IsConditionSatisfied("攻撃不能"))
                    {
                        if (w2 > 0)
                        {
                            u2.Attack(uw2, u1, "", "", is_event);
                        }
                        else
                        {
                            u2.PilotMessage("射程外", msg_mode: "");
                        }
                    }

                    SRC.Stage = cur_stage;
                    GUI.CloseMessageForm();
                    u1.CurrentForm().UpdateCondition();
                    u2.CurrentForm().UpdateCondition();
                    u1.CurrentForm().CheckAutoHyperMode();
                    u1.CurrentForm().CheckAutoNormalMode();
                    u2.CurrentForm().CheckAutoHyperMode();
                    u2.CurrentForm().CheckAutoNormalMode();
                    Commands.SelectedUnit = prev_su;
                    Commands.SelectedTarget = prev_st;
                    Commands.SelectedWeapon = prev_w;
                    Commands.SelectedTWeapon = prev_tw;
                }
            }

            GUI.RedrawScreen();
            return EventData.NextID;
        }
    }
}
