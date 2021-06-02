using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class BossRankCmd : CmdData
    {
        public BossRankCmd(SRC src, EventDataLine eventData) : base(src, CmdType.BossRankCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string buf;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2);
                        buf = GetArgAsString(3);
                        if (!Information.IsNumeric(buf))
                        {
                            throw new EventErrorException(this, "ボスランクが不正です");
                        }

                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        buf = GetArgAsString(2);
                        if (!Information.IsNumeric(buf))
                        {
                            throw new EventErrorException(this, "ボスランクが不正です");
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "BossRankコマンドの引数の数が違います");
            }

            if (u is object)
            {
                u.BossRank = Conversions.ToInteger(buf);
                u.HP = u.MaxHP;
                u.FullSupply();
            }

            return EventData.NextID;
        }
    }
}
