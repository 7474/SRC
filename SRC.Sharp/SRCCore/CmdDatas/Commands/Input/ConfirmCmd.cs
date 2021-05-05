using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ConfirmCmd : CmdData
    {
        public ConfirmCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ConfirmCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Confirmコマンドの引数の数が違います");
            }

            var res = GUI.Confirm(GetArgAsString(2), "選択", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
            Event.SelectedAlternative = res == GuiDialogResult.Ok ? "1" : "0";

            return EventData.NextID;
        }
    }
}
