using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class GlobalCmd : CmdData
    {
        public GlobalCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GlobalCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            for (var i = 2; i <= ArgNum; i++)
            {
                var vname = GetArg(i);
                if (Strings.InStr(vname, "\"") > 0)
                {
                    throw new EventErrorException(this, "ïœêîñºÅu" + vname + "ÅvÇ™ïsê≥Ç≈Ç∑");
                }

                if (Strings.Asc(vname) == 36) // $
                {
                    vname = Strings.Mid(vname, 2);
                }

                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }
            }

            return EventData.NextID;
        }
    }
}
