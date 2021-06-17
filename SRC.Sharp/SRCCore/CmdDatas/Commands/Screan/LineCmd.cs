using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LineCmd : CmdData
    {
        public LineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            PictureBox pic, pic2 = default;
            //            short x1, y1;
            //            short x2, y2;
            //            string opt, dtype = default;
            //            string cname;
            //            int clr;
            //            short i;
            //            if (ArgNum < 5)
            //            {
            //                Event.EventErrorMessage = "Lineコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 308150


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            x1 = (GetArgAsLong(2) + Event.BaseX);
            //            y1 = (GetArgAsLong(3) + Event.BaseY);
            //            x2 = (GetArgAsLong(4) + Event.BaseX);
            //            y2 = (GetArgAsLong(5) + Event.BaseY);
            //            GUI.SaveScreen();

            //            // 描画先
            //            switch (Event.ObjDrawOption ?? "")
            //            {
            //                case "背景":
            //                    {
            //                        pic = GUI.MainForm.picBack;
            //                        pic2 = GUI.MainForm.picMaskedBack;
            //                        Map.IsMapDirty = true;
            //                        break;
            //                    }

            //                case "保持":
            //                    {
            //                        pic = GUI.MainForm.picMain(0);
            //                        pic2 = GUI.MainForm.picMain(1);
            //                        GUI.IsPictureVisible = true;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        pic = GUI.MainForm.picMain(0);
            //                        break;
            //                    }
            //            }

            //            // 描画領域
            //            short tmp;
            //            if (Event.ObjDrawOption != "背景")
            //            {
            //                GUI.IsPictureVisible = true;
            //                tmp = (Event.ObjDrawWidth - 1);
            //                GUI.PaintedAreaX1 = GeneralLib.MaxLng(GeneralLib.MinLng(GUI.PaintedAreaX1, GeneralLib.MinLng(x1 - tmp, x2 - tmp)), 0);
            //                GUI.PaintedAreaY1 = GeneralLib.MaxLng(GeneralLib.MinLng(GUI.PaintedAreaY1, GeneralLib.MinLng(y1 - tmp, y2 - tmp)), 0);
            //                GUI.PaintedAreaX2 = GeneralLib.MaxLng(GeneralLib.MinLng(GUI.PaintedAreaX2, GeneralLib.MinLng(x1 + tmp, x2 + tmp)), GUI.MapPWidth - 1);
            //                GUI.PaintedAreaY2 = GeneralLib.MaxLng(GeneralLib.MinLng(GUI.PaintedAreaY2, GeneralLib.MinLng(y1 + tmp, y2 + tmp)), GUI.MapPHeight - 1);
            //            }

            //            clr = Event.ObjColor;
            //            var loopTo = ArgNum;
            //            for (i = 6; i <= loopTo; i++)
            //            {
            //                opt = GetArgAsString(i);
            //                if (Strings.Asc(opt) == 35) // #
            //                {
            //                    if (Strings.Len(opt) != 7)
            //                    {
            //                        Event.EventErrorMessage = "色指定が不正です";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 311316


            //                        Input:
            //                                            Error(0)

            //                         */
            //                    }

            //                    cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
            //                    StringType.MidStmtStr(cname, 1, 2, "&H");
            //                    var midTmp = Strings.Mid(opt, 6, 2);
            //                    StringType.MidStmtStr(cname, 3, 2, midTmp);
            //                    var midTmp1 = Strings.Mid(opt, 4, 2);
            //                    StringType.MidStmtStr(cname, 5, 2, midTmp1);
            //                    var midTmp2 = Strings.Mid(opt, 2, 2);
            //                    StringType.MidStmtStr(cname, 7, 2, midTmp2);
            //                    if (!Information.IsNumeric(cname))
            //                    {
            //                        Event.EventErrorMessage = "色指定が不正です";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 311826


            //                        Input:
            //                                            Error(0)

            //                         */
            //                    }

            //                    clr = Conversions.ToInteger(cname);
            //                }
            //                else
            //                {
            //                    if (opt != "B" && opt != "BF")
            //                    {
            //                        Event.EventErrorMessage = "Lineコマンドに不正なオプション「" + opt + "」が使われています";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 312022


            //                        Input:
            //                                            Error(0)

            //                         */
            //                    }

            //                    dtype = opt;
            //                }
            //            }
            //            pic.DrawWidth = Event.ObjDrawWidth;
            //            pic.FillColor = Event.ObjFillColor;
            //            pic.FillStyle = Event.ObjFillStyle;
            //            switch (dtype ?? "")
            //            {
            //                case "B":
            //                    {
            //                        pic.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                        break;
            //                    }

            //                case "BF":
            //                    {
            //                        pic.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        pic.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                        break;
            //                    }
            //            }
            //            pic.DrawWidth = 1;
            //            pic.FillColor = ColorTranslator.ToOle(Color.White);
            //            pic.FillStyle = vbFSTransparent;
            //            if (pic2 is object)
            //            {
            //                pic2.DrawWidth = Event.ObjDrawWidth;
            //                pic2.FillColor = Event.ObjFillColor;
            //                pic2.FillStyle = Event.ObjFillStyle;
            //                switch (dtype ?? "")
            //                {
            //                    case "B":
            //                        {
            //                            pic2.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                            break;
            //                        }

            //                    case "BF":
            //                        {
            //                            pic2.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            pic2.Line(x1, y1); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //                            break;
            //                        }
            //                }
            //                pic2.DrawWidth = 1;
            //                pic2.FillColor = ColorTranslator.ToOle(Color.White);
            //                pic2.FillStyle = vbFSTransparent;
            //            }

            //return EventData.NextID;
        }
    }
}
