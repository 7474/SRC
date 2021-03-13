using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearEventCmd : CmdData
    {
        public ClearEventCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearEventCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 2:
                    {
                        string arglname = GetArgAsString((short)2);
                        ret = Event_Renamed.FindLabel(ref arglname);
                        if (ret > 0)
                        {
                            Event_Renamed.ClearLabel(ret);
                        }

                        break;
                    }

                case 1:
                    {
                        if (Event_Renamed.CurrentLabel > 0)
                        {
                            Event_Renamed.ClearLabel(Event_Renamed.CurrentLabel);
                        }

                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "ClearEventコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 160477


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            ExecClearEventCmdRet = LineNum + 1;
            return EventData.ID + 1;
        }
    }
}
