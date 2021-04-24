using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class DoCmd : CmdData
    {
        public DoCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DoCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 1:
                    // Do - Loop While
                    return EventData.ID + 1;

                case 3:
                    switch (GetArg(2)?.ToLower() ?? "")
                    {
                        case "while":
                            if (GetArgAsLong(3) != 0)
                            {
                                return EventData.ID + 1;
                            }
                            break;

                        case "until":
                            if (GetArgAsLong(3) == 0)
                            {
                                return EventData.ID + 1;
                            }
                            break;

                        default:
                            throw new EventErrorException(this, "Doコマンドの書式が間違っています");
                    }
                    break;

                default:
                    throw new EventErrorException(this, "Doコマンドの引数の数が違います");
            }

            // 条件式がFalseのため本体をスキップ
            var depth = 1;
            foreach (var i in AfterEventIdRange())
            {
                var cmd = Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.DoCmd:
                        depth = (depth + 1);
                        break;

                    case CmdType.LoopCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            return cmd.EventData.ID + 1;
                        }
                        break;
                }
            }

            throw new EventErrorException(this, "DoとLoopが対応していません");
        }
    }
}
