using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class AutoTalkCmd : CmdData
    {
        public AutoTalkCmd(SRC src, EventDataLine eventData) : base(src, CmdType.AutoTalkCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecAutoTalkCmdRet = default;
            string pname, current_pname = default;
            Unit u;
            short ux, uy;
            int i;
            short j;
            short lnum;
            int prev_msg_wait;
            var without_cursor = default(bool);
            string options = default, opt;
            string buf;

            // メッセージ表示速度を「普通」の値に設定
            prev_msg_wait = GUI.MessageWait;
            GUI.MessageWait = 700;
            short counter;
            counter = (short)LineNum;
            string cname;
            int tcolor;
            var loopTo = Information.UBound(Event_Renamed.EventData);
            for (i = (int)counter; i <= loopTo; i++)
            {
                {
                    var withBlock = Event_Renamed.EventCmd[i];
                    switch (withBlock.Name)
                    {
                        case Event_Renamed.CmdType.AutoTalkCmd:
                            {
                                if ((int)withBlock.ArgNum > 1)
                                {
                                    pname = withBlock.GetArgAsString((short)2);
                                }
                                else
                                {
                                    pname = "";
                                }

                                if (Strings.Left(pname, 1) == "@")
                                {
                                    // メインパイロットの強制指定
                                    pname = Strings.Mid(pname, 2);
                                    object argIndex2 = (object)pname;
                                    if (SRC.PList.IsDefined(ref argIndex2))
                                    {
                                        object argIndex1 = (object)pname;
                                        {
                                            var withBlock1 = SRC.PList.Item(ref argIndex1);
                                            if (withBlock1.Unit_Renamed is object)
                                            {
                                                pname = withBlock1.Unit_Renamed.MainPilot().Name;
                                            }
                                        }
                                    }
                                }

                                // 話者名チェック
                                bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                                bool localIsDefined2() { object argIndex1 = (object)pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                                if (!localIsDefined() & !localIsDefined1() & !localIsDefined2() & !(pname == "システム") & !string.IsNullOrEmpty(pname))
                                {
                                    Event_Renamed.EventErrorMessage = "「" + pname + "」というパイロットが定義されていません";
                                    LineNum = i;
                                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 93710


                                    Input:
                                                                Error(0)

                                     */
                                }

                                if ((int)withBlock.ArgNum > 1)
                                {
                                    options = "";
                                    without_cursor = false;
                                    j = (short)2;
                                    lnum = (short)1;
                                    while (j <= withBlock.ArgNum)
                                    {
                                        opt = withBlock.GetArgAsString(j);
                                        switch (opt ?? "")
                                        {
                                            case "非表示":
                                                {
                                                    without_cursor = true;
                                                    break;
                                                }

                                            case "枠外":
                                                {
                                                    GUI.MessageWindowIsOut = true;
                                                    break;
                                                }

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
                                            case "通常":
                                                {
                                                    if ((int)j > 2)
                                                    {
                                                        // これらのパイロット画像描画に関するオプションは
                                                        // パイロット名が指定されている場合にのみ有効
                                                        options = options + opt + " ";
                                                    }
                                                    else
                                                    {
                                                        lnum = j;
                                                    }

                                                    break;
                                                }

                                            case "右回転":
                                                {
                                                    j = (short)((int)j + 1);
                                                    options = options + "右回転 " + withBlock.GetArgAsString(j) + " ";
                                                    break;
                                                }

                                            case "左回転":
                                                {
                                                    j = (short)((int)j + 1);
                                                    options = options + "左回転 " + withBlock.GetArgAsString(j) + " ";
                                                    break;
                                                }

                                            case "フィルタ":
                                                {
                                                    j = (short)((int)j + 1);
                                                    buf = withBlock.GetArgAsString(j);
                                                    cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                                    StringType.MidStmtStr(ref cname, 1, 2, "&H");
                                                    var midTmp = Strings.Mid(buf, 6, 2);
                                                    StringType.MidStmtStr(ref cname, 3, 2, midTmp);
                                                    var midTmp1 = Strings.Mid(buf, 4, 2);
                                                    StringType.MidStmtStr(ref cname, 5, 2, midTmp1);
                                                    var midTmp2 = Strings.Mid(buf, 2, 2);
                                                    StringType.MidStmtStr(ref cname, 7, 2, midTmp2);
                                                    tcolor = Conversions.ToInteger(cname);
                                                    j = (short)((int)j + 1);
                                                    // 空白のオプションをスキップ
                                                    options = options + "フィルタ " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)tcolor) + " " + withBlock.GetArgAsString(j) + " ";
                                                    break;
                                                }

                                            case var @case when @case == "":
                                                {
                                                    break;
                                                }

                                            default:
                                                {
                                                    // 通常の引数をスキップ
                                                    lnum = j;
                                                    break;
                                                }
                                        }

                                        j = (short)((int)j + 1);
                                    }
                                }
                                else
                                {
                                    lnum = (short)1;
                                }

                                switch (lnum)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            // 引数なし

                                            if (!My.MyProject.Forms.frmMessage.Visible)
                                            {
                                                Unit argu1 = null;
                                                Unit argu2 = null;
                                                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                                            }

                                            // メッセージウィンドウのパイロット画像を以前指定された
                                            // ものに確定させる
                                            if (!string.IsNullOrEmpty(current_pname))
                                            {
                                                GUI.DisplayBattleMessage(ref current_pname, "", ref options);
                                            }

                                            current_pname = "";
                                            break;
                                        }

                                    case 2:
                                        {
                                            // パイロット名のみ指定
                                            current_pname = pname;

                                            // 話者中心に画面位置を変更

                                            // プロローグイベントやエピローグイベント時はキャンセル
                                            if (SRC.Stage == "プロローグ" | SRC.Stage == "エピローグ")
                                            {
                                                goto NextLoop;
                                            }

                                            // 画面書き換え可能？
                                            if (!GUI.MainForm.Visible)
                                            {
                                                goto NextLoop;
                                            }

                                            if (GUI.IsPictureVisible)
                                            {
                                                goto NextLoop;
                                            }

                                            if (string.IsNullOrEmpty(Map.MapFileName))
                                            {
                                                goto NextLoop;
                                            }

                                            // 話者を中央表示
                                            CenterUnit(pname, without_cursor);
                                            break;
                                        }

                                    case 3:
                                        {
                                            current_pname = pname;
                                            switch (withBlock.GetArgAsString((short)3) ?? "")
                                            {
                                                case "母艦":
                                                    {
                                                        // 母艦の中央表示
                                                        CenterUnit("母艦", without_cursor);
                                                        break;
                                                    }

                                                case "中央":
                                                    {
                                                        // 話者を中央表示
                                                        CenterUnit(pname, without_cursor);
                                                        break;
                                                    }

                                                case "固定":
                                                    {
                                                        break;
                                                    }
                                                    // 表示位置固定
                                            }

                                            break;
                                        }

                                    case 4:
                                        {
                                            // 表示の座標指定あり
                                            current_pname = pname;
                                            CenterUnit("", without_cursor, (short)withBlock.GetArgAsLong((short)3), (short)withBlock.GetArgAsLong((short)4));
                                            break;
                                        }

                                    case -1:
                                        {
                                            Event_Renamed.EventErrorMessage = "AutoTalkコマンドのパラメータの括弧の対応が取れていません";
                                            LineNum = i;
                                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 97215


                                            Input:
                                                                            Error(0)

                                             */
                                            break;
                                        }

                                    default:
                                        {
                                            Event_Renamed.EventErrorMessage = "AutoTalkコマンドの引数の数が違います";
                                            LineNum = i;
                                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 97369


                                            Input:
                                                                            Error(0)

                                             */
                                            break;
                                        }
                                }

                                if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    Unit argu11 = null;
                                    Unit argu21 = null;
                                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                                }

                                break;
                            }

                        case Event_Renamed.CmdType.EndCmd:
                            {
                                GUI.CloseMessageForm();
                                if ((int)withBlock.ArgNum != 1)
                                {
                                    Event_Renamed.EventErrorMessage = "End部分の引数の数が違います";
                                    LineNum = i;
                                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 97766


                                    Input:
                                                                Error(0)

                                     */
                                }

                                break;
                            }

                        case Event_Renamed.CmdType.SuspendCmd:
                            {
                                if ((int)withBlock.ArgNum != 1)
                                {
                                    Event_Renamed.EventErrorMessage = "Suspend部分の引数の数が違います";
                                    LineNum = i;
                                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 98012


                                    Input:
                                                                Error(0)

                                     */
                                }

                                break;
                            }

                        default:
                            {
                                if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    Unit argu12 = null;
                                    Unit argu22 = null;
                                    GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                                }

                                GUI.DisplayBattleMessage(ref current_pname, Event_Renamed.EventData[i], ref options);
                                break;
                            }
                    }
                }

            NextLoop:
                ;
            }

            // メッセージ表示速度を元に戻す
            GUI.MessageWait = prev_msg_wait;
            if (i > Information.UBound(Event_Renamed.EventData))
            {
                GUI.CloseMessageForm();
                Event_Renamed.EventErrorMessage = "AutoTalkとEndが対応していません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 98673


                Input:
                            Error(0)

                 */
            }

            ExecAutoTalkCmdRet = i + 1;
            return ExecAutoTalkCmdRet;
            return EventData.ID + 1;
        }
    }
}
