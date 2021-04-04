using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class SkipCmd : CmdData
    {
        public SkipCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SkipCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 対応するループの末尾を探す
            var depth = 1;
            foreach (var i in AfterEventIdRange())
            {
                var cmd = Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.DoCmd:
                    case CmdType.ForCmd:
                    case CmdType.ForEachCmd:
                        depth = (depth + 1);
                        break;

                    case CmdType.LoopCmd:
                    case CmdType.NextCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            return cmd.EventData.ID;
                        }
                        break;
                }
            }
            throw new EventErrorException(this, "Skipコマンドがループの外で使われています");
        }
    }
}
