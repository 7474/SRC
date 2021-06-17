using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClsCmd : CmdData
    {
        public ClsCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClsCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            var BF = default(object);
            //            string cname, buf;
            //            int ret;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        buf = GetArgAsString(2);
            //                        if (Strings.Asc(buf) != 35 || Strings.Len(buf) != 7)
            //                        {
            //                            Event.EventErrorMessage = "色指定が不正です";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 189557


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
            //                        StringType.MidStmtStr(cname, 1, 2, "&H");
            //                        var midTmp = Strings.Mid(buf, 6, 2);
            //                        StringType.MidStmtStr(cname, 3, 2, midTmp);
            //                        var midTmp1 = Strings.Mid(buf, 4, 2);
            //                        StringType.MidStmtStr(cname, 5, 2, midTmp1);
            //                        var midTmp2 = Strings.Mid(buf, 2, 2);
            //                        StringType.MidStmtStr(cname, 7, 2, midTmp2);
            //                        if (!Information.IsNumeric(cname))
            //                        {
            //                            Event.EventErrorMessage = "色指定が不正です";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 190067


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }
            //                        GUI.MainForm.picMain(0).Line((0, 0) - (GUI.MainPWidth - 1, GUI.MainPHeight - 1), Conversions.ToInteger(cname), BF);
            //                        GUI.MainForm.picMain(1).Line((0, 0) - (GUI.MainPWidth - 1, GUI.MainPHeight - 1), Conversions.ToInteger(cname), BF);
            //                        GUI.ScreenIsSaved = true;
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        {
            //                            var withBlock = GUI.MainForm;
            //                            ret = GUI.PatBlt(withBlock.picMain(0).hDC, 0, 0, GUI.MainPWidth, GUI.MainPHeight, GUI.BLACKNESS);
            //                            ret = GUI.PatBlt(withBlock.picMain(1).hDC, 0, 0, GUI.MainPWidth, GUI.MainPHeight, GUI.BLACKNESS);
            //                        }

            //                        GUI.ScreenIsSaved = true;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Clsコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 191690


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            GUI.IsPictureVisible = true;
            //            GUI.IsCursorVisible = false;
            //            GUI.PaintedAreaX1 = GUI.MainPWidth;
            //            GUI.PaintedAreaY1 = GUI.MainPHeight;
            //            GUI.PaintedAreaX2 = -1;
            //            GUI.PaintedAreaY2 = -1;
            //return EventData.NextID;
        }
    }
}
