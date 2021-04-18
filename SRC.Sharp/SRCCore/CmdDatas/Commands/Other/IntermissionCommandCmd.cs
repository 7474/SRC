using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class IntermissionCommandCmd : CmdData
    {
        public IntermissionCommandCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IntermissionCommandCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "IntermissionCommandコマンドの引数の数が違います");
            }

            string vname = "IntermissionCommand(" + GetArgAsString(2) + ")";
            if (GetArg(3) == "削除")
            {
                Expression.UndefineVariable(vname);
            }
            else
            {
                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }

                Expression.SetVariableAsString(vname, GetArgAsString(3));
            }
            return EventData.NextID;
        }
    }
}
