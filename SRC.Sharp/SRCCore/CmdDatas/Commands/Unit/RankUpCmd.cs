using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class RankUpCmd : CmdData
    {
        public RankUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RankUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            int rk;
            switch (ArgNum)
            {
                case 3:
                    {
                        var uname = GetArgAsString(2);
                        u = SRC.UList.Item(uname);
                        if (u is null)
                        {
                            throw new EventErrorException(this, uname + "というユニットは存在しません");
                        }

                        rk = GetArgAsLong(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        rk = GetArgAsLong(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "RankUpコマンドの引数の数が違います");
            }

            var hp_ratio = u.HP / (double)u.MaxHP;
            var en_ratio = u.EN / (double)u.MaxEN;

            u.RankUp(rk, hp_ratio, en_ratio);

            return EventData.NextID;
        }
    }
}
