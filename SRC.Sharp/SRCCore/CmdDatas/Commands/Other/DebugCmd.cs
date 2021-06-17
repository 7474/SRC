using SRCCore.Events;
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
            SRC.LogInfo(string.Join(", ", Enumerable.Range(2, ArgNum - 1).Select(x => GetArgAsString(x))));
            return EventData.NextID;
        }
    }
}
