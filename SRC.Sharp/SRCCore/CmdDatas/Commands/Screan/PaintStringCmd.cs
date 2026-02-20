using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class PaintStringCmd : CmdData
    {
        private bool without_cr;

        public PaintStringCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PaintStringCmd, eventData)
        {
            without_cr = false;

            // TODO PaintString文の処理の高速化のため、あらかじめ構文解析しておく
            // 「;」を含む場合は改めて項に分解
            // (正しくリストの処理が行えないため)
            var orgEdata = eventData.Data;
            if (Strings.Right(orgEdata, 1) == ";")
            {
                without_cr = true;

                var edata = Strings.Left(orgEdata, Strings.Len(orgEdata) - 1);
                if (Strings.Right(orgEdata, 1) == " ")
                {
                    // メッセージが空文字列
                    edata = edata + "\"\"";
                }
                SetEventData(new EventDataLine(eventData.ID, eventData.Source, eventData.File, eventData.LineNum, edata));
            }

            //switch (ArgNum)
            //{
            //    case 2:
            //        {
            //            // 引数が１個の場合
            //            ArgNum = 2;
            //            strArgs = new string[3];
            //            lngArgs = new int[3];
            //            dblArgs = new double[3];
            //            ArgsType = new Expressions.ValueType[3];
            //            buf = list[2];

            //            // 表示文字列が式の場合にも対応
            //            if (Strings.Left(buf, 1) == "\"" && Strings.Right(buf, 1) == "\"")
            //            {
            //                if (Strings.InStr(buf, "$(") > 0)
            //                {
            //                    strArgs[2] = buf;
            //                }
            //                else
            //                {
            //                    strArgs[2] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
            //                    ArgsType[2] = Expressions.ValueType.StringType;
            //                }
            //            }
            //            else if (Strings.Left(buf, 1) == "`" && Strings.Right(buf, 1) == "`")
            //            {
            //                strArgs[2] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
            //                ArgsType[2] = Expressions.ValueType.StringType;
            //            }
            //            else if (Strings.InStr(buf, "$(") > 0)
            //            {
            //                strArgs[2] = "\"" + buf + "\"";
            //            }
            //            else
            //            {
            //                strArgs[2] = buf;
            //            }

            //            break;
            //        }

            //    case 3:
            //        {
            //            // 引数が２個の場合
            //            ArgNum = 2;
            //            strArgs = new string[3];
            //            lngArgs = new int[3];
            //            dblArgs = new double[3];
            //            ArgsType = new Expressions.ValueType[3];

            //            // 表示文字列は必ず文字列
            //            buf = GeneralLib.ListTail(edata, 2);
            //            if (Strings.InStr(buf, "$(") > 0)
            //            {
            //                strArgs[2] = "\"" + buf + "\"";
            //            }
            //            else
            //            {
            //                strArgs[2] = buf;
            //                ArgsType[2] = Expressions.ValueType.StringType;
            //            }

            //            break;
            //        }

            //    case 4:
            //        {
            //            // 引数が３個の場合

            //            // 座標指定があるかどうかが確定しているか？
            //            if ((list[2] == "-" || Information.IsNumeric(list[2]) || Expression.IsExpr(list[2])) && (list[3] == "-" || Information.IsNumeric(list[3]) || Expression.IsExpr(list[3])))
            //            {
            //                // 座標指定があることが確定
            //                ArgNum = 4;
            //                strArgs = new string[5];
            //                lngArgs = new int[5];
            //                dblArgs = new double[5];
            //                ArgsType = new Expressions.ValueType[5];
            //                strArgs[2] = list[2];
            //                strArgs[3] = list[3];
            //                if (!Expression.IsExpr(list[2]))
            //                {
            //                    ArgsType[2] = Expressions.ValueType.StringType;
            //                }

            //                if (!Expression.IsExpr(list[3]))
            //                {
            //                    ArgsType[3] = Expressions.ValueType.StringType;
            //                }
            //            }
            //            else
            //            {
            //                // 実行時まで座標指定があるかどうか不明
            //                ArgNum = 5;
            //                strArgs = new string[6];
            //                lngArgs = new int[6];
            //                dblArgs = new double[6];
            //                ArgsType = new Expressions.ValueType[6];
            //                strArgs[2] = list[2];
            //                strArgs[3] = list[3];

            //                // 座標指定がなかった場合の表示文字列
            //                buf = GeneralLib.ListTail(edata, 2);
            //                if (Strings.InStr(buf, "$(") > 0)
            //                {
            //                    strArgs[5] = "\"" + buf + "\"";
            //                }
            //                else
            //                {
            //                    strArgs[5] = buf;
            //                    ArgsType[5] = Expressions.ValueType.StringType;
            //                }
            //            }

            //            // 座標指定があった場合の表示文字列
            //            buf = list[4];
            //            if (Strings.Left(buf, 1) == "\"" && Strings.Right(buf, 1) == "\"")
            //            {
            //                if (Strings.InStr(buf, "$(") > 0)
            //                {
            //                    strArgs[4] = buf;
            //                }
            //                else
            //                {
            //                    strArgs[4] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
            //                    ArgsType[4] = Expressions.ValueType.StringType;
            //                }
            //            }
            //            else if (Strings.Left(buf, 1) == "`" && Strings.Right(buf, 1) == "`")
            //            {
            //                strArgs[4] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
            //                ArgsType[4] = Expressions.ValueType.StringType;
            //            }
            //            else if (Strings.InStr(buf, "$(") > 0)
            //            {
            //                strArgs[4] = "\"" + buf + "\"";
            //            }
            //            else
            //            {
            //                strArgs[4] = buf;
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            // 引数が４個以上の場合

            //            // 座標指定があるかどうかが確定しているか？
            //            if ((list[2] == "-" || Information.IsNumeric(list[2]) || Expression.IsExpr(list[2])) && (list[3] == "-" || Information.IsNumeric(list[3]) || Expression.IsExpr(list[3])))
            //            {
            //                // 座標指定があることが確定
            //                ArgNum = 4;
            //                strArgs = new string[5];
            //                lngArgs = new int[5];
            //                dblArgs = new double[5];
            //                ArgsType = new Expressions.ValueType[5];
            //                strArgs[2] = list[2];
            //                strArgs[3] = list[3];
            //                if (!Expression.IsExpr(list[2]))
            //                {
            //                    ArgsType[2] = Expressions.ValueType.StringType;
            //                }

            //                if (!Expression.IsExpr(list[3]))
            //                {
            //                    ArgsType[3] = Expressions.ValueType.StringType;
            //                }
            //            }
            //            else
            //            {
            //                // 実行時まで座標指定があるかどうか不明
            //                ArgNum = 5;
            //                strArgs = new string[6];
            //                lngArgs = new int[6];
            //                dblArgs = new double[6];
            //                ArgsType = new Expressions.ValueType[6];
            //                strArgs[2] = list[2];
            //                strArgs[3] = list[3];

            //                // 座標指定がなかった場合の表示文字列
            //                buf = GeneralLib.ListTail(edata, 2);
            //                if (Strings.InStr(buf, "$(") > 0)
            //                {
            //                    strArgs[5] = "\"" + buf + "\"";
            //                }
            //                else
            //                {
            //                    strArgs[5] = buf;
            //                    ArgsType[5] = Expressions.ValueType.StringType;
            //                }
            //            }

            //            // 座標指定があった場合の表示文字列
            //            buf = GeneralLib.ListTail(edata, 4);
            //            if (Strings.InStr(buf, "$(") > 0)
            //            {
            //                strArgs[4] = "\"" + buf + "\"";
            //            }
            //            else
            //            {
            //                strArgs[4] = buf;
            //                ArgsType[4] = Expressions.ValueType.StringType;
            //            }

            //            break;
            //        }
            //}
        }

        protected override int ExecInternal()
        {
            string sx, sy;
            int xx, yy;

            // PaintStringはあらかじめ構文解析済み
            switch (ArgNum)
            {
                case 2:
                    // 座標指定がないことが確定
                    GUI.DrawString(GetArgAsString(2), Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, without_cr);
                    break;
                case 4:
                    // 座標指定付きであることが確定
                    sx = GetArgAsString(2);
                    sy = GetArgAsString(3);
                    if (sx == "-")
                    {
                        GUI.HCentering = true;
                        xx = -1;
                    }
                    else
                    {
                        GUI.HCentering = false;
                        xx = Conversions.ToInteger(sx) + Event.BaseX;
                    }

                    if (sy == "-")
                    {
                        GUI.VCentering = true;
                        yy = -1;
                    }
                    else
                    {
                        GUI.VCentering = false;
                        yy = Conversions.ToInteger(sy) + Event.BaseY;
                    }

                    GUI.DrawString(GetArgAsString(4), xx, yy, without_cr);
                    break;

                case 5:
                    // 座標指定付きかどうか実行時まで不明
                    sx = GetArgAsString(2);
                    sy = GetArgAsString(3);

                    // 最初の2引数が有効な座標指定かどうかで判断する
                    if ((Information.IsNumeric(sx) || sx == "-") && (Information.IsNumeric(sy) || sy == "-"))
                    {
                        if (sx == "-")
                        {
                            GUI.HCentering = true;
                            xx = -1;
                        }
                        else
                        {
                            GUI.HCentering = false;
                            xx = Conversions.ToInteger(sx) + Event.BaseX;
                        }

                        if (sy == "-")
                        {
                            GUI.VCentering = true;
                            yy = -1;
                        }
                        else
                        {
                            GUI.VCentering = false;
                            yy = Conversions.ToInteger(sy) + Event.BaseY;
                        }

                        GUI.DrawString(GetArgAsString(4), xx, yy, without_cr);
                    }
                    else
                    {
                        GUI.DrawString(GetArgAsString(5), -1, -1, without_cr);
                    }

                    break;
            }

            return EventData.NextID;
        }
    }
}
