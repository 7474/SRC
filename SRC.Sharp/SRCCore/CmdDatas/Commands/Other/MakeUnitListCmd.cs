using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class MakeUnitListCmd : CmdData
    {
        public MakeUnitListCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MakeUnitListCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // ユニット一覧を作成
            Event.MakeUnitList(GetArgAsString(2));
            return EventData.NextID;
        }
    }
}
