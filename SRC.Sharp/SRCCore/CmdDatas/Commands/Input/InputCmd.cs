using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Commands
{
    public class InputCmd : CmdData
    {
        public InputCmd(SRC src, EventDataLine eventData) : base(src, CmdType.InputCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string input;
            GuiDialogResult res;
            switch (ArgNum)
            {
                case 3:
                    res = GUI.Input(GetArgAsString(3), "SRC", "", out input);
                    break;

                case 4:
                    res = GUI.Input(GetArgAsString(3), "SRC", GetArgAsString(4), out input);
                    break;

                default:
                    throw new EventErrorException(this, "Inputコマンドの引数の数が違います");
            }

            Expression.SetVariableAsString(GetArg(2), res == GuiDialogResult.Ok ? input : "");
            return EventData.NextID;
        }
    }
}
