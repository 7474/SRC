using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class LoopCmd : CmdData
    {
        public LoopCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LoopCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 1:
                    break;

                case 3:
                    switch (GetArg(2)?.ToLower() ?? "")
                    {
                        case "while":
                            if (GetArgAsLong(3) == 0)
                            {
                                return EventData.NextID;
                            }
                            break;

                        case "until":
                            if (GetArgAsLong(3) != 0)
                            {
                                return EventData.NextID;
                            }
                            break;

                        default:
                            throw new EventErrorException(this, "Loop文の書式が間違っています");
                    }

                    break;

                default:
                    throw new EventErrorException(this, "Loop文の引数の数が違います");
            }

            // 条件式がTrueのため先頭に戻る
            var depth = 1;
            foreach (var i in BeforeEventIdRange())
            {
                var cmd = Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.DoCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            return cmd.EventData.ID;
                        }
                        break;

                    case CmdType.LoopCmd:
                        depth = (depth + 1);
                        break;
                }
            }

            throw new EventErrorException(this, "DoとLoopが対応していません");
        }
    }
}
