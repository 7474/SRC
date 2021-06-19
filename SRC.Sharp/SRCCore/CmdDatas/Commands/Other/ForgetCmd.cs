using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ForgetCmd : CmdData
    {
        public ForgetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ForgetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Forgetコマンドの引数の数が違います");
            }

            var tname = GetArgAsString(2);
            SRC.Titles.Remove(tname);

            return EventData.NextID;
        }
    }
}
