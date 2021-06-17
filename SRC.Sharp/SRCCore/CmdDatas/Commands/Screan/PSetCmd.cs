using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PSetCmd : CmdData
    {
        public PSetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PSetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            PictureBox pic, pic2 = default;
            //            short xx, yy;
            //            string opt;
            //            string cname;
            //            int clr;
            //            if (ArgNum < 3)
            //            {
            //                Event.EventErrorMessage = "PSetコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 392694


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            // 座標
            //            xx = (GetArgAsLong(2) + Event.BaseX);
            //            yy = (GetArgAsLong(3) + Event.BaseY);

            //            // 座標は画面上にある？
            //            if (xx < 0 || GUI.MapPWidth <= xx || yy < 0 || GUI.MapPHeight <= yy)
            //            {
            //                ExecPSetCmdRet = LineNum + 1;
            //                return ExecPSetCmdRet;
            //            }

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
            //                if ((xx - tmp) < GUI.PaintedAreaX1)
            //                {
            //                    GUI.PaintedAreaX1 = (xx - tmp);
            //                }
            //                else if ((xx + tmp) > GUI.PaintedAreaX2)
            //                {
            //                    GUI.PaintedAreaX2 = (xx + tmp);
            //                }

            //                if ((yy - tmp) < GUI.PaintedAreaY1)
            //                {
            //                    GUI.PaintedAreaY1 = (yy - tmp);
            //                }
            //                else if ((yy + tmp) > GUI.PaintedAreaY2)
            //                {
            //                    GUI.PaintedAreaY2 = (yy + tmp);
            //                }
            //            }

            //            // 描画色
            //            if (ArgNum == 4)
            //            {
            //                opt = GetArgAsString(4);
            //                if (Strings.Asc(opt) != 35 || Strings.Len(opt) != 7)
            //                {
            //                    Event.EventErrorMessage = "色指定が不正です";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 395407


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
            //                StringType.MidStmtStr(cname, 1, 2, "&H");
            //                var midTmp = Strings.Mid(opt, 6, 2);
            //                StringType.MidStmtStr(cname, 3, 2, midTmp);
            //                var midTmp1 = Strings.Mid(opt, 4, 2);
            //                StringType.MidStmtStr(cname, 5, 2, midTmp1);
            //                var midTmp2 = Strings.Mid(opt, 2, 2);
            //                StringType.MidStmtStr(cname, 7, 2, midTmp2);
            //                if (!Information.IsNumeric(cname))
            //                {
            //                    Event.EventErrorMessage = "色指定が不正です";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 395908


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                clr = Conversions.ToInteger(cname);
            //            }
            //            else
            //            {
            //                clr = Event.ObjColor;
            //            }

            //            pic.DrawWidth = Event.ObjDrawWidth;

            //            // 点を描画
            //            pic.PSet(xx, yy);/* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */

            //            pic.DrawWidth = 1;
            //            if (pic2 is object)
            //            {
            //                pic2.DrawWidth = Event.ObjDrawWidth;

            //                // 点を描画
            //                pic2.PSet(xx, yy);/* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */

            //                pic2.DrawWidth = 1;
            //            }

            //return EventData.NextID;
        }
    }
}
