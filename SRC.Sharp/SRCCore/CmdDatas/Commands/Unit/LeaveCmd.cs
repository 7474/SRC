using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、ユニット操作
    public class LeaveCmd : CmdData
    {
        public LeaveCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LeaveCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string pname = "";
            string vname;
            Unit u;
            string opt = "";
            var num = ArgNum;
            if (num > 1)
            {
                if (GetArgAsString(num) == "非同期")
                {
                    opt = "非同期";
                    num = (num - 1);
                }
            }

            switch (num)
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
                            vname = "IsAway(" + SRC.NPDList.Item(pname).Name + ")";
                            if (!Expression.IsGlobalVariableDefined(vname))
                            {
                                Expression.DefineGlobalVariable(vname);
                            }

                            Expression.SetVariableAsLong(vname, 1);
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
                                    .FirstOrDefault(x => x.Name == pname && x.Party0 == "味方" && x.CurrentForm().Status != "離脱")
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
                    throw new EventErrorException(this, "Leaveコマンドの引数の数が違います");
            }

            if (u is null)
            {
                SRC.PList.Item(pname).Away = true;
            }
            else
            {
                if (u.Status == "出撃" || u.Status == "格納")
                {
                    u.Escape(opt);
                }

                if (u.Party0 != "味方")
                {
                    u.ChangeParty("味方");
                }

                if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
                {
                    u.Status = "離脱";
                }

                foreach (var p in u.AllRawPilots)
                {
                    p.Away = true;
                }
            }

            return EventData.NextID;
        }
    }
}
