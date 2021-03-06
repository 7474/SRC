using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameBGMCmd : CmdData
    {
        public RenameBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameBGMCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string bname, vname;
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "RenameBGMの引数の数が違います");
            }

            bname = GetArgAsString(2);
            switch (bname ?? "")
            {
                case "Map1":
                case "Map2":
                case "Map3":
                case "Map4":
                case "Map5":
                case "Map6":
                case "Briefing":
                case "Intermission":
                case "Subtitle":
                case "End":
                case "default":
                    {
                        vname = "BGM(" + bname + ")";
                        break;
                    }

                default:
                    throw new EventErrorException(this, "BGM名が不正です");
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
