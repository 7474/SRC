using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CircleCmd : CmdData
    {
        public CircleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CircleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 4)
            {
                Event.EventErrorMessage = "Circleコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 152388


                Input:
                            Error(0)

                 */
            }

            x1 = (GetArgAsLong(2) + Event.BaseX);
            y1 = (GetArgAsLong(3) + Event.BaseY);
            rad = GetArgAsLong(4);
            GUI.SaveScreen();

            // 描画先
            switch (Event.ObjDrawOption ?? "")
            {
                case "背景":
                    {
                        pic = GUI.MainForm.picBack;
                        pic2 = GUI.MainForm.picMaskedBack;
                        Map.IsMapDirty = true;
                        break;
                    }

                case "保持":
                    {
                        pic = GUI.MainForm.picMain(0);
                        pic2 = GUI.MainForm.picMain(1);
                        break;
                    }

                default:
                    {
                        pic = GUI.MainForm.picMain(0);
                        break;
                    }
            }

            // 描画領域
            short tmp;
            if (Event.ObjDrawOption != "背景")
            {
                GUI.IsPictureVisible = true;
                tmp = (rad + Event.ObjDrawWidth - 1);
                GUI.PaintedAreaX1 = GeneralLib.MinLng(GUI.PaintedAreaX1, GeneralLib.MaxLng(x1 - tmp, 0));
                GUI.PaintedAreaY1 = GeneralLib.MinLng(GUI.PaintedAreaY1, GeneralLib.MaxLng(y1 - tmp, 0));
                GUI.PaintedAreaX2 = GeneralLib.MaxLng(GUI.PaintedAreaX2, GeneralLib.MinLng(x1 + tmp, GUI.MapPWidth - 1));
                GUI.PaintedAreaY2 = GeneralLib.MaxLng(GUI.PaintedAreaY2, GeneralLib.MinLng(y1 + tmp, GUI.MapPHeight - 1));
            }

            clr = Event.ObjColor;
            var loopTo = ArgNum;
            for (i = 5; i <= loopTo; i++)
            {
                opt = GetArgAsString(i);
                if (Strings.Asc(opt) == 35) // #
                {
                    if (Strings.Len(opt) != 7)
                    {
                        Event.EventErrorMessage = "色指定が不正です";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 155229


                        Input:
                                            Error(0)

                         */
                    }

                    cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                    StringType.MidStmtStr(cname, 1, 2, "&H");
                    var midTmp = Strings.Mid(opt, 6, 2);
                    StringType.MidStmtStr(cname, 3, 2, midTmp);
                    var midTmp1 = Strings.Mid(opt, 4, 2);
                    StringType.MidStmtStr(cname, 5, 2, midTmp1);
                    var midTmp2 = Strings.Mid(opt, 2, 2);
                    StringType.MidStmtStr(cname, 7, 2, midTmp2);
                    if (!Information.IsNumeric(cname))
                    {
                        Event.EventErrorMessage = "色指定が不正です";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 155739


                        Input:
                                            Error(0)

                         */
                    }

                    clr = Conversions.ToInteger(cname);
                }
                else
                {
                    Event.EventErrorMessage = "Circleコマンドに不正なオプション「" + opt + "」が使われています";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 155895


                    Input:
                                    Error(0)

                     */
                }
            }
            pic.DrawWidth = Event.ObjDrawWidth;
            pic.FillColor = Event.ObjFillColor;
            pic.FillStyle = Event.ObjFillStyle;

            pic.Circle(x1, y1);/* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            pic.DrawWidth = 1;
            pic.FillColor = ColorTranslator.ToOle(Color.White);
            pic.FillStyle = vbFSTransparent;
            if (pic2 is object)
            {
                pic2.DrawWidth = Event.ObjDrawWidth;
                pic2.FillColor = Event.ObjFillColor;
                pic2.FillStyle = Event.ObjFillStyle;

                pic2.Circle(x1, y1);/* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                pic2.DrawWidth = 1;
                pic2.FillColor = ColorTranslator.ToOle(Color.White);
                pic2.FillStyle = vbFSTransparent;
            }
            return EventData.NextID;
        }
    }
}
