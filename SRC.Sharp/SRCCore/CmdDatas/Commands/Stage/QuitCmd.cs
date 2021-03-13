using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class QuitCmd : CmdData
    {
        public QuitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            SRC.TerminateSRC();
            return -1;
        }
    }
}
