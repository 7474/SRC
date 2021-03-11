using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class TalkCmd : ATalkCmd
    {
        public TalkCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TalkCmd, eventData)
        {
        }

        protected override void DisplayMessage(string pname, string msg, string msg_mode = "")
        {
            GUI.DisplayMessage(pname, msg, msg_mode);
        }

        protected override int ExecInternal()
        {
            return ProcessTalk();
        }
    }
}
