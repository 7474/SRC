using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStockCmd : CmdData
    {
        public SetStockCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStockCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string aname;
            int aid, num;
            Unit u;
            switch (ArgNum)
            {
                case 4:
                    {
                        u = GetArgAsUnit(2);
                        aname = GetArgAsString(3);
                        num = GetArgAsLong(4);
                        break;
                    }

                case 3:
                    {
                        u = Event.SelectedUnitForEvent;
                        aname = GetArgAsString(2);
                        num = GetArgAsLong(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "SetStockコマンドの引数の数が違います");
            }

            if (Information.IsNumeric(aname))
            {
                aid = GeneralLib.StrToLng(aname);
                if (aid < 1 || u.CountAbility() < aid)
                {
                    throw new EventErrorException(this, "アビリティの番号「" + aname + "」が間違っています");
                }
            }
            else
            {
                var loopTo = u.CountAbility();
                for (aid = 1; aid <= loopTo; aid++)
                {
                    if ((u.Ability(aid).Data.Name ?? "") == (aname ?? ""))
                    {
                        break;
                    }
                }

                if (aid < 1 || u.CountAbility() < aid)
                {
                    throw new EventErrorException(this, u.Name + "はアビリティ「" + aname + "」を持っていません");
                }
            }

            var ua = u.Ability(aid);
            ua.SetStock(GeneralLib.MinLng(num, ua.MaxStock()));
            return EventData.NextID;
        }
    }
}
