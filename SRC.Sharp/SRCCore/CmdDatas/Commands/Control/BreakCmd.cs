using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class BreakCmd : CmdData
    {
        public BreakCmd(SRC src, EventDataLine eventData) : base(src, CmdType.BreakCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 対応するLoopもしくはNextコマンドを探す
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
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            return cmd.EventData.NextID;
                        }
                        break;

                    case CmdType.NextCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            Event.ForIndex = (Event.ForIndex - 1);
                            return cmd.EventData.NextID;
                        }
                        break;
                }
            }

            throw new EventErrorException(this, "Breakコマンドがループの外で使われています");
        }
    }
}
