using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SetWindowFrameWidthCmd : CmdData
    {
        public SetWindowFrameWidthCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetWindowFrameWidthCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "SetWindowFrameWidthコマンドの引数の数が違います");
            }

            var width = GetArgAsLong(2);

            var vname = "StatusWindow(FrameWidth)";
            if (!Expression.IsGlobalVariableDefined(vname))
            {
                Expression.DefineGlobalVariable(vname);
            }
            Expression.SetVariableAsLong(vname, width);

            return EventData.NextID;
        }
    }
}
