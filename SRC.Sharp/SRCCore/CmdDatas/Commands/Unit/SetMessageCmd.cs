using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class SetMessageCmd : CmdData
    {
        public SetMessageCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetMessageCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string pname = default, pname0 = default;
            string sit;
            string selected_msg;
            switch (ArgNum)
            {
                case 4:
                    {
                        pname = GetArgAsString(2);
                        u = SRC.UList.Item2(pname);
                        if (u is null)
                        {
                            if (!SRC.PList.IsDefined(pname))
                            {
                                pname0 = pname;
                                if (Strings.InStr(pname0, "(") > 0)
                                {
                                    pname0 = Strings.Left(pname0, GeneralLib.InStr2(pname0, "(") - 1);
                                }

                                if (!SRC.PList.IsDefined(pname0))
                                {
                                    throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                                }

                                u = SRC.PList.Item(pname0).Unit;
                            }
                            else
                            {
                                u = SRC.PList.Item(pname).Unit;
                            }

                            if (u is null)
                            {
                                throw new EventErrorException(this, "「" + pname + "」はユニットに乗っていません");
                            }
                        }
                        else if (u.CountPilot() == 0)
                        {
                            throw new EventErrorException(this, "指定されたユニットにはパイロットが乗っていません");
                        }

                        sit = GetArgAsString(3);
                        selected_msg = GetArgAsString(4);
                        break;
                    }

                case 3:
                    {
                        u = Event.SelectedUnitForEvent;
                        if (u.CountPilot() == 0)
                        {
                            throw new EventErrorException(this, "指定されたユニットにはパイロットが乗っていません");
                        }

                        sit = GetArgAsString(2);
                        selected_msg = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "SetMessageコマンドの引数の数が違います");
            }

            if (selected_msg == "解除")
            {
                // メッセージ用変数を削除
                Expression.UndefineVariable("Message(" + u.MainPilot().ID + "," + sit + ")");
            }
            else if (!string.IsNullOrEmpty(pname0))
            {
                // 表情指定付きメッセージをローカル変数として登録する
                Expression.SetVariableAsString("Message(" + u.MainPilot().ID + "," + sit + ")", pname + "::" + selected_msg);
            }
            else
            {
                // メッセージをローカル変数として登録する
                Expression.SetVariableAsString("Message(" + u.MainPilot().ID + "," + sit + ")", selected_msg);
            }
            return EventData.NextID;
        }
    }
}
