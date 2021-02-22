using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class PaintPictureCmd : CmdData
    {
        public PaintPictureCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PaintPictureCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 4)
            {
                throw new EventErrorException(this, "PaintPictureコマンドの引数の数が違います");
            }

            var buf = "";
            var options = "";
            var tcolor = Color.White;
            var i = 5;
            var opt_n = 4;
            while (i <= ArgNum)
            {
                buf = GetArgAsString(i);
                switch (buf ?? "")
                {
                    case "透過":
                    case "背景":
                    case "白黒":
                    case "セピア":
                    case "明":
                    case "暗":
                    case "上下反転":
                    case "左右反転":
                    case "上半分":
                    case "下半分":
                    case "右半分":
                    case "左半分":
                    case "右上":
                    case "左上":
                    case "右下":
                    case "左下":
                    case "ネガポジ反転":
                    case "シルエット":
                    case "夕焼け":
                    case "水中":
                    case "保持":
                    case "フィルタ":
                        {
                            options = options + buf + " ";
                            break;
                        }

                    case "右回転":
                        {
                            i = (i + 1);
                            options = options + "右回転 " + GetArgAsString(i) + " ";
                            break;
                        }

                    case "左回転":
                        {
                            i = (i + 1);
                            options = options + "左回転 " + GetArgAsString(i) + " ";
                            break;
                        }

                    case "-":
                        {
                            // スキップ
                            // スキップ
                            opt_n = i;
                            break;
                        }

                    case var @case when @case == "":
                        {
                            break;
                        }

                    default:
                        {
                            if (Strings.Asc(buf) == 35 && Strings.Len(buf) == 7)
                            {
                                // TODO
                                //cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                //StringType.MidStmtStr(cname, 1, 2, "&H");
                                //var midTmp = Strings.Mid(buf, 6, 2);
                                //StringType.MidStmtStr(cname, 3, 2, midTmp);
                                //var midTmp1 = Strings.Mid(buf, 4, 2);
                                //StringType.MidStmtStr(cname, 5, 2, midTmp1);
                                //var midTmp2 = Strings.Mid(buf, 2, 2);
                                //StringType.MidStmtStr(cname, 7, 2, midTmp2);
                                //if (Information.IsNumeric(cname))
                                //{
                                //    tcolor = Conversions.ToInteger(cname);
                                //    if (tcolor != ColorTranslator.ToOle(Color.White) | GetArgAsString((i - 1)) == "フィルタ")
                                //    {
                                //        options = options + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)tcolor) + " ";
                                //    }
                                //}
                            }
                            else if (Information.IsNumeric(buf))
                            {
                                // スキップ
                                opt_n = i;
                            }
                            else if (Strings.InStr(buf, " ") > 0)
                            {
                                options = options + buf + " ";
                            }
                            else if (Strings.Right(buf, 1) == "%" & Information.IsNumeric(Strings.Left(buf, Strings.Len(buf) - 1)))
                            {
                                options = options + buf + " ";
                            }
                            else
                            {
                                throw new EventErrorException(this, "PaintPictureコマンドの" + i + "番目のパラメータ「" + buf + "」が不正です");
                            }

                            break;
                        }
                }

                i = (i + 1);
            }

            var fname = GetArgAsString(2);
            switch (Strings.Right(Strings.LCase(fname), 4) ?? "")
            {
                // 正しい画像ファイル名
                case ".bmp":
                case ".jpg":
                case ".gif":
                case ".png":
                    break;

                default:
                    if (SRC.PDList.IsDefined(fname))
                    {
                        fname = @"Pilot\" + SRC.PDList.Item(fname).Bitmap;
                    }
                    else if (SRC.NPDList.IsDefined(fname))
                    {
                        fname = @"Pilot\" + SRC.NPDList.Item(fname).Bitmap;
                    }
                    else if (SRC.UDList.IsDefined(fname))
                    {
                        fname = @"Unit\" + SRC.UDList.Item(fname).Bitmap;
                    }
                    else
                    {
                        throw new EventErrorException(this, "不正な画像ファイル名「" + fname + "」が指定されています");
                    }

                    break;
            }

            // 描画先の画像
            int dx, dy, dw, dh;
            buf = GetArgAsString(3);
            if (buf == "-")
            {
                dx = Constants.DEFAULT_LEVEL;
            }
            else
            {
                dx = GeneralLib.StrToLng(buf) + Event.BaseX;
            }

            buf = GetArgAsString(4);
            if (buf == "-")
            {
                dy = Constants.DEFAULT_LEVEL;
            }
            else
            {
                dy = GeneralLib.StrToLng(buf) + Event.BaseY;
            }

            // 描画サイズ
            if (opt_n >= 6)
            {
                buf = GetArgAsString(5);
                if (buf == "-")
                {
                    dw = Constants.DEFAULT_LEVEL;
                }
                else
                {
                    dw = GeneralLib.StrToLng(buf);
                    if (dw <= 0)
                    {
                        return EventData.ID + 1;
                    }
                }

                buf = GetArgAsString(6);
                if (buf == "-")
                {
                    dh = Constants.DEFAULT_LEVEL;
                }
                else
                {
                    dh = GeneralLib.StrToLng(buf);
                    if (dh <= 0)
                    {
                        return EventData.ID + 1;
                    }
                }
            }
            else
            {
                dw = Constants.DEFAULT_LEVEL;
                dh = Constants.DEFAULT_LEVEL;
            }

            // 原画像における転送元座標＆サイズ
            int sx, sy, sw, sh;
            if (opt_n == 10)
            {
                buf = GetArgAsString(7);
                if (buf == "-")
                {
                    sx = Constants.DEFAULT_LEVEL;
                }
                else
                {
                    sx = GeneralLib.StrToLng(buf);
                }

                buf = GetArgAsString(8);
                if (buf == "-")
                {
                    sy = Constants.DEFAULT_LEVEL;
                }
                else
                {
                    sy = GeneralLib.StrToLng(buf);
                }

                sw = GetArgAsLong(9);
                sh = GetArgAsLong(10);
            }
            else
            {
                sx = 0;
                sy = 0;
                sw = 0;
                sh = 0;
            }

            GUI.DrawPicture(fname, dx, dy, dw, dh, sx, sy, sw, sh, options);

            return EventData.ID + 1;
        }
    }
}
