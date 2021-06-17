using SRCCore.Events;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class DebugCmd : CmdData
    {
        public DebugCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DebugCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var message = string.Join(" ", Enumerable.Range(2, ArgNum - 1)
                .Select(x => GetArg(x))
                .Select(x =>
                {
                    Expression.FormatMessage(ref x);
                    return Strings.Trim(x);
                }));
            SRC.LogInfo(message);
            return EventData.NextID;
        }
    }
}
