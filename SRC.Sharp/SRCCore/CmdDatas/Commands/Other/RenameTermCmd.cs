using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameTermCmd : CmdData
    {
        public RenameTermCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameTermCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "RenameTermの引数の数が違います");
            }

            var tname = GetArgAsString(2);
            string vname;
            switch (tname ?? "")
            {
                case "HP":
                case "EN":
                case "SP":
                case "CT":
                    {
                        vname = "ShortTerm(" + tname + ")";
                        break;
                    }

                default:
                    {
                        vname = "Term(" + tname + ")";
                        break;
                    }
            }

            if (!Expression.IsGlobalVariableDefined(vname))
            {
                Expression.DefineGlobalVariable(vname);
            }

            Expression.SetVariableAsString(vname, GetArgAsString(3));
            return EventData.NextID;
        }
    }
}
