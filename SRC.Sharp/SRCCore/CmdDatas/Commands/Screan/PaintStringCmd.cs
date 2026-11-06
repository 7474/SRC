using SRCCore.Events;
using SRCCore.Exceptions;
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

            // 元の SRC は PaintString の引数を構文解析のとき（CmdData.cls の Parse）に整理していた。
            // SRC# は同じ解釈を ExecInternal で行う（引数の数による分け方・座標指定が確定しているかの判定）。
            // 「;」を含む場合は改めて項に分解
            // (正しくリストの処理が行えないため)
            var orgEdata = eventData.Data;
            if (Strings.Right(orgEdata, 1) == ";")
            {
                without_cr = true;

                var edata = Strings.Left(orgEdata, Strings.Len(orgEdata) - 1);
                if (Strings.Right(edata, 1) == " ")
                {
                    // メッセージが空文字列
                    edata = edata + "\"\"";
                }
                SetEventData(new EventDataLine(eventData.ID, eventData.Source, eventData.File, eventData.LineNum, edata));
            }
        }

        protected override int ExecInternal()
        {
            // VB版では構文解析時に引数を整理していた（CmdData.cls の Parse）。
            // ここでは実行時に同じ解釈を行う。
            switch (ArgNum)
            {
                case 2:
                    // 座標指定がないことが確定
                    GUI.DrawString(ArgString(2), Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, without_cr);
                    break;

                case 3:
                    // 引数が２個の場合は座標指定なし。表示文字列は必ず文字列
                    GUI.DrawString(TailString(2), Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, without_cr);
                    break;

                default:
                    {
                        if (ArgNum < 4)
                        {
                            break;
                        }

                        // 元の SRC は構文解析のときに、最初の 2 つの項がどちらも「-」・数値・式（「$」「(」で始まる）なら
                        // 座標指定があることが確定したとみなす。そのときは値が数値でなければ CInt の実行時エラーになる
                        var raw2 = GetArgRaw(2).RawArg ?? "";
                        var raw3 = GetArgRaw(3).RawArg ?? "";
                        var fixedCoord = IsCoordinateTerm(raw2) && IsCoordinateTerm(raw3);
                        string sx, sy;
                        if (fixedCoord)
                        {
                            // 式でない項は書いたままの文字列として扱う
                            sx = IsExpr(raw2) ? GetArgAsString(2) : raw2;
                            sy = IsExpr(raw3) ? GetArgAsString(3) : raw3;
                        }
                        else
                        {
                            // 実行時まで座標指定があるかどうか不明。最初の2引数が有効な座標指定かどうかで判断する
                            sx = GetArgAsString(2);
                            sy = GetArgAsString(3);
                        }

                        if (fixedCoord || ((Information.IsNumeric(sx) || sx == "-") && (Information.IsNumeric(sy) || sy == "-")))
                        {
                            int xx, yy;
                            if (sx == "-")
                            {
                                GUI.HCentering = true;
                                xx = -1;
                            }
                            else
                            {
                                GUI.HCentering = false;
                                xx = CInt(sx) + Event.BaseX;
                            }

                            if (sy == "-")
                            {
                                GUI.VCentering = true;
                                yy = -1;
                            }
                            else
                            {
                                GUI.VCentering = false;
                                yy = CInt(sy) + Event.BaseY;
                            }

                            // 座標指定があった場合の表示文字列
                            var msg = ArgNum == 4 ? ArgString(4) : TailString(4);
                            GUI.DrawString(msg, xx, yy, without_cr);
                        }
                        else
                        {
                            // 座標指定がなかった場合の表示文字列
                            GUI.DrawString(TailString(2), -1, -1, without_cr);
                        }

                        break;
                    }
            }

            return EventData.NextID;
        }

        // 元の SRC の IsExpr（先頭が「$」か「(」）
        private static bool IsExpr(string term)
        {
            return term.StartsWith("$") || term.StartsWith("(");
        }

        // 座標指定があることが確定する項か（「-」・数値・式）
        private static bool IsCoordinateTerm(string term)
        {
            return term == "-" || Information.IsNumeric(term) || IsExpr(term);
        }

        // 元の SRC の CInt。数値でなければ実行時エラー（「イベントデータが不正です」）
        private int CInt(string value)
        {
            if (!Information.IsNumeric(value))
            {
                throw new EventErrorException(this, "イベントデータが不正です");
            }
            return Conversions.ToInteger(value);
        }

        // idx 番目の引数を表示文字列として返す（VB版の構文解析時の処理）
        private string ArgString(int idx)
        {
            var raw = GetArgRaw(idx).RawArg ?? "";
            if ((Strings.Left(raw, 1) == "\"" && Strings.Right(raw, 1) == "\"")
                || (Strings.Left(raw, 1) == "`" && Strings.Right(raw, 1) == "`"))
            {
                // 引用符で囲まれた文字列（式置換を含む場合は式置換を行う）
                return GetArgAsString(idx);
            }
            if (Strings.InStr(raw, "$(") > 0)
            {
                // 引用符で囲まれていない引数に式置換が含まれる場合は文字列として式置換を行う
                Expression.ReplaceSubExpression(ref raw);
                return raw;
            }
            // VB版は型を未定にし直して項として評価する（数値も書いたままの文字列になる）
            return Expression.GetValueAsString(raw, true);
        }

        // idx 番目以降の引数を空白で区切られたままひとつの文字列として返す（式置換は行う）
        private string TailString(int idx)
        {
            var buf = GeneralLib.ListTail(EventData.Data, idx);
            if (Strings.InStr(buf, "$(") > 0)
            {
                Expression.ReplaceSubExpression(ref buf);
            }
            return buf;
        }
    }
}
