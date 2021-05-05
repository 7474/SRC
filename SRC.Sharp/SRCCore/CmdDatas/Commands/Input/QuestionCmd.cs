using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class QuestionCmd : CmdData
    {
        public QuestionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuestionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            int ExecQuestionCmdRet = default;
            //            string[] list;
            //            int i;
            //            string buf;
            //            list = new string[1];
            //            GUI.ListItemID = new string[1];
            //            GUI.ListItemFlag = new bool[1];
            //            GUI.ListItemID[0] = "0";
            //            var loopTo = Information.UBound(Event.EventData);
            //            for (i = LineNum + 1; i <= loopTo; i++)
            //            {
            //                buf = Event.EventData[i];
            //                Expression.FormatMessage(ref buf);
            //                if (Strings.Len(buf) > 0)
            //                {
            //                    if (Strings.LCase(buf) == "end")
            //                    {
            //                        break;
            //                    }

            //                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
            //                    Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
            //                    Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
            //                    list[Information.UBound(list)] = buf;
            //                    GUI.ListItemID[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i - LineNum);
            //                    GUI.ListItemFlag[Information.UBound(list)] = false;
            //                }
            //            }

            //            if (i == Information.UBound(Event.EventData))
            //            {
            //                Event.EventErrorMessage = "QuestionとEndが対応していません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 399008


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            if (Information.UBound(list) > 0)
            //            {
            //                switch (ArgNum)
            //                {
            //                    case 3:
            //                        {
            //                            Commands.SelectedItem = GUI.LIPS(ref "選択", ref list, ref GetArgAsString((short)3), (short)GetArgAsLong((short)2));
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            Commands.SelectedItem = GUI.LIPS(ref "選択", ref list, ref "さあ、どうする？", (short)GetArgAsLong((short)2));
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            Event.EventErrorMessage = "Questionコマンドの引数の数が違います";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 399492


            //                            Input:
            //                                                Error(0)

            //                             */
            //                            break;
            //                        }
            //                }
            //            }
            //            else
            //            {
            //                Commands.SelectedItem = (short)0;
            //            }

            //            Event.SelectedAlternative = GUI.ListItemID[Commands.SelectedItem];
            //            GUI.ListItemID = new string[1];
            //            ExecQuestionCmdRet = i + 1;
        }
    }
}
