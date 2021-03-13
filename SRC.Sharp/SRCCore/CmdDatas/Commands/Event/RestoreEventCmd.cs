using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class RestoreEventCmd : CmdData
    {
        public RestoreEventCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RestoreEventCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if ((int)ArgNum != 2)
            {
                Event_Renamed.EventErrorMessage = "RestoreEventコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 439974


                Input:
                            Error(0)

                 */
            }

            string arglname = GetArgAsString(2);
            Event_Renamed.RestoreLabel(ref arglname);

            return EventData.ID + 1;
        }
    }
}
