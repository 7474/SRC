using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、ユニット操作
    public class JoinCmd : CmdData
    {
        public JoinCmd(SRC src, EventDataLine eventData) : base(src, CmdType.JoinCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string pname = "";
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    {
                        pname = GetArgAsString(2);
                        if (SRC.PList.IsDefined(pname))
                        {
                            u = SRC.PList.Item(pname).Unit;
                        }
                        else if (SRC.NPDList.IsDefined(pname))
                        {
                            pname = "IsAway(" + SRC.NPDList.Item(pname).Name + ")";
                            if (Expression.IsGlobalVariableDefined(pname))
                            {
                                Expression.UndefineVariable(pname);
                            }

                            return EventData.NextID;
                        }
                        else if (SRC.UList.IsDefined(pname))
                        {
                            if ((pname ?? "") == (SRC.UList.Item(pname).ID ?? ""))
                            {
                                u = SRC.UList.Item(pname);
                            }
                            else
                            {
                                u = SRC.UList.Items
                                    .FirstOrDefault(x => x.Name == pname && x.Party0 == "味方" && x.CurrentForm().Status == "離脱")
                                    ?.CurrentForm();
                            }
                        }
                        else
                        {
                            throw new EventErrorException(this, "「" + pname + "」というパイロットまたはユニットが見つかりません");
                        }

                        break;
                    }

                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Joinコマンドの引数の数が違います");
            }

            if (u is null)
            {
                if (SRC.PList.IsDefined(pname))
                {
                    SRC.PList.Item(pname).Away = false;
                }
            }
            else
            {
                u.Status = "待機";
                foreach (var p in u.AllRawPilots)
                {
                    p.Away = false;
                }
            }

            return EventData.NextID;
        }
    }
}
