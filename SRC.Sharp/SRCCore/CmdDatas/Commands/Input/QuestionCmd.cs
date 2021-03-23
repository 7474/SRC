using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class QuestionCmd : CmdData
    {
        public QuestionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuestionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecQuestionCmdRet = default;
            string[] list;
            int i;
            string buf;
            list = new string[1];
            GUI.ListItemID = new string[1];
            GUI.ListItemFlag = new bool[1];
            GUI.ListItemID[0] = "0";
            var loopTo = Information.UBound(Event_Renamed.EventData);
            for (i = LineNum + 1; i <= loopTo; i++)
            {
                buf = Event_Renamed.EventData[i];
                Expression.FormatMessage(ref buf);
                if (Strings.Len(buf) > 0)
                {
                    if (Strings.LCase(buf) == "end")
                    {
                        break;
                    }

                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
                    Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = buf;
                    GUI.ListItemID[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i - LineNum);
                    GUI.ListItemFlag[Information.UBound(list)] = false;
                }
            }

            if (i == Information.UBound(Event_Renamed.EventData))
            {
                Event_Renamed.EventErrorMessage = "QuestionとEndが対応していません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 399008


                Input:
                            Error(0)

                 */
            }

            if (Information.UBound(list) > 0)
            {
                switch (ArgNum)
                {
                    case 3:
                        {
                            string arglb_caption = "選択";
                            string arglb_info = GetArgAsString((short)3);
                            Commands.SelectedItem = GUI.LIPS(ref arglb_caption, ref list, ref arglb_info, (short)GetArgAsLong((short)2));
                            break;
                        }

                    case 2:
                        {
                            string arglb_caption1 = "選択";
                            string arglb_info1 = "さあ、どうする？";
                            Commands.SelectedItem = GUI.LIPS(ref arglb_caption1, ref list, ref arglb_info1, (short)GetArgAsLong((short)2));
                            break;
                        }

                    default:
                        {
                            Event_Renamed.EventErrorMessage = "Questionコマンドの引数の数が違います";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 399492


                            Input:
                                                Error(0)

                             */
                            break;
                        }
                }
            }
            else
            {
                Commands.SelectedItem = (short)0;
            }

            Event_Renamed.SelectedAlternative = GUI.ListItemID[Commands.SelectedItem];
            GUI.ListItemID = new string[1];
            ExecQuestionCmdRet = i + 1;
            return EventData.ID + 1;
        }
    }
}
