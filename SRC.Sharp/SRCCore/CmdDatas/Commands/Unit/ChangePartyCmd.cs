using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangePartyCmd : CmdData
    {
        public ChangePartyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangePartyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string new_party;
            string pname;
            switch (ArgNum)
            {
                case 2:
                    new_party = GetArgAsString(2);
                    if (new_party != "味方" && new_party != "ＮＰＣ" && new_party != "敵" && new_party != "中立")
                    {
                        throw new EventErrorException(this, "陣営の指定が間違っています");
                    }

                    Event.SelectedUnitForEvent.ChangeParty(new_party);
                    break;

                case 3:
                    new_party = GetArgAsString(3);
                    if (new_party != "味方" && new_party != "ＮＰＣ" && new_party != "敵" && new_party != "中立")
                    {
                        throw new EventErrorException(this, "陣営の指定が間違っています");
                    }

                    pname = GetArgAsString(2);
                    var u = SRC.UList.Item2(pname);
                    if (u is null)
                    {
                        if (!SRC.PList.IsDefined(pname))
                        {
                            throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                        }

                        {
                            var p = SRC.PList.Item(pname);
                            if (p.Unit is null)
                            {
                                p.Party = new_party;
                            }
                            else
                            {
                                p.Unit.ChangeParty(new_party);
                            }
                        }
                    }
                    else
                    {
                        u.ChangeParty(new_party);
                    }

                    break;

                default:
                    throw new EventErrorException(this, "ChangePartyコマンドの引数の数が違います");
            }

            // カーソルが陣営変更されたユニット上にあるとカーソルは消去されるので
            // XXX 実装次第要素
            GUI.IsCursorVisible = false;

            return EventData.NextID;
        }
    }
}
