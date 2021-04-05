using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public abstract class ATalkCmd : CmdData
    {
        public ATalkCmd(SRC src, CmdType cmdType, EventDataLine eventData) : base(src, cmdType, eventData)
        {
        }

        protected abstract void DisplayMessage(string pname, string msg, string msg_mode = "");

        protected int ProcessTalk()
        {
            string pname, current_pname = default;
            int i;
            int j;
            int lnum;
            var without_cursor = default(bool);
            string options = default, opt;

            for (i = EventData.ID; i < Event.EventData.Count; i++)
            {
                var currentCmd = Event.EventCmd[i];
                switch (currentCmd.Name)
                {
                    case CmdType.TalkCmd:
                    case CmdType.AutoTalkCmd:
                        {
                            // XXX
                            if (currentCmd.Name != Name)
                            {
                                throw new EventErrorException(this, "Talk中またはAutoTalk中に他方のコマンドが実行されました");
                            }

                            if (currentCmd.ArgNum > 1)
                            {
                                pname = currentCmd.GetArgAsString(2);
                            }
                            else
                            {
                                pname = "";
                            }

                            // TODO Impl
                            //if (Strings.Left(pname, 1) == "@")
                            //{
                            //    // メインパイロットの強制指定
                            //    pname = Strings.Mid(pname, 2);
                            //    if (SRC.PList.IsDefined(pname))
                            //    {
                            //        var p = SRC.PList.Item(pname);
                            //        if (p.Unit_Renamed != null)
                            //        {
                            //            pname = p.Unit_Renamed.MainPilot().Name;
                            //        }
                            //    }
                            //}

                            // 話者名チェック
                            if (
                                !SRC.PList.IsDefined(pname)
                                && !SRC.PDList.IsDefined(pname)
                                && !SRC.NPDList.IsDefined(pname)
                                && !(pname == "システム")
                                && !string.IsNullOrEmpty(pname))
                            {
                                //LineNum = i; // XXX これ要るの？
                                throw new EventErrorException(currentCmd, "「" + pname + "」というパイロットが定義されていません");
                            }

                            if (currentCmd.ArgNum > 1)
                            {
                                options = "";
                                without_cursor = false;
                                j = 2;
                                lnum = 1;
                                while (j <= currentCmd.ArgNum)
                                {
                                    opt = currentCmd.GetArgAsString(j);
                                    switch (opt ?? "")
                                    {
                                        case "非表示":
                                            without_cursor = true;
                                            break;

                                        case "枠外":
                                            GUI.MessageWindowIsOut = true;
                                            break;

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
                                            if (j > 2)
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

                                        case "右回転":
                                            j = (j + 1);
                                            options = options + "右回転 " + currentCmd.GetArgAsString(j) + " ";
                                            break;

                                        case "左回転":
                                            j = (j + 1);
                                            options = options + "左回転 " + currentCmd.GetArgAsString(j) + " ";
                                            break;

                                        // TODO 色指定の処理全般保留
                                        //case "フィルタ":
                                        //    {
                                        //        j = (j + 1);
                                        //        buf = currentCmd.GetArgAsString(j);
                                        //        cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                        //        StringType.MidStmtStr(cname, 1, 2, "&H");
                                        //        var midTmp = Strings.Mid(buf, 6, 2);
                                        //        StringType.MidStmtStr(cname, 3, 2, midTmp);
                                        //        var midTmp1 = Strings.Mid(buf, 4, 2);
                                        //        StringType.MidStmtStr(cname, 5, 2, midTmp1);
                                        //        var midTmp2 = Strings.Mid(buf, 2, 2);
                                        //        StringType.MidStmtStr(cname, 7, 2, midTmp2);
                                        //        tcolor = Conversions.ToInteger(cname);
                                        //        j = (j + 1);
                                        //        // 空白のオプションをスキップ
                                        //        options = options + "フィルタ " + SrcFormatter.Format(tcolor) + " " + currentCmd.GetArgAsString(j) + " ";
                                        //        break;
                                        //    }

                                        case var @case when @case == "":
                                            break;

                                        default:
                                            // 通常の引数をスキップ
                                            lnum = j;
                                            break;
                                    }

                                    j = (j + 1);
                                }
                            }
                            else
                            {
                                lnum = 1;
                            }

                            switch (lnum)
                            {
                                case 0:
                                case 1:
                                    {
                                        // 引数なし
                                        // TODO Impl
                                        //if (!My.MyProject.Forms.frmMessage.Visible)
                                        //{
                                        //    GUI.OpenMessageForm(u1: null, u2: null);
                                        //}

                                        // メッセージウィンドウのパイロット画像を以前指定された
                                        // ものに確定させる
                                        if (!string.IsNullOrEmpty(current_pname))
                                        {
                                            DisplayMessage(current_pname, options);
                                        }

                                        current_pname = "";
                                        break;
                                    }

                                case 2:
                                    // パイロット名のみ指定
                                    current_pname = pname;

                                    // 話者中心に画面位置を変更
                                    // プロローグイベントやエピローグイベント時はキャンセル
                                    if (SRC.Stage == "プロローグ" | SRC.Stage == "エピローグ")
                                    {
                                        break;
                                    }

                                    // 画面書き換え可能？
                                    if (!GUI.MainFormVisible)
                                    {
                                        break;
                                    }
                                    if (GUI.IsPictureVisible)
                                    {
                                        break;
                                    }
                                    if (Map.IsStatusView)
                                    {
                                        break;
                                    }

                                    // 話者を中央表示
                                    CenterUnit(pname, without_cursor);
                                    break;

                                // TODO Impl
                                case 3:
                                    current_pname = pname;
                                    switch (currentCmd.GetArgAsString(3) ?? "")
                                    {
                                        case "母艦":
                                            // 母艦の中央表示
                                            CenterUnit("母艦", without_cursor);
                                            break;

                                        case "中央":
                                            // 話者の中央表示
                                            CenterUnit(pname, without_cursor);
                                            break;

                                        case "固定":
                                            break;
                                            // 表示位置固定
                                    }
                                    break;

                                case 4:
                                    current_pname = pname;
                                    CenterUnit(pname, without_cursor, currentCmd.GetArgAsLong(3), currentCmd.GetArgAsLong(4));
                                    break;

                                case -1:
                                    //LineNum = i; // XXX これ要るの？
                                    throw new EventErrorException(currentCmd, "Talkコマンドのパラメータの括弧の対応が取れていません");

                                default:
                                    throw new EventErrorException(currentCmd, "Talkコマンドの引数の数が違います");
                            }

                            if (!GUI.MessageFormVisible)
                            {
                                GUI.OpenMessageForm(null, null);
                            }

                            break;
                        }

                    case CmdType.EndCmd:
                        {
                            GUI.CloseMessageForm();
                            GUI.MessageWindowIsOut = false;
                            if (currentCmd.ArgNum != 1)
                            {
                                ////LineNum = i; // XXX これ要るの？
                                throw new EventErrorException(currentCmd, "End部分の引数の数が違います");
                            }
                            return i + 1;
                        }

                    case CmdType.SuspendCmd:
                        {
                            if (currentCmd.ArgNum != 1)
                            {
                                //LineNum = i; // XXX これ要るの？
                                throw new EventErrorException(currentCmd, "Suspend部分の引数の数が違います");
                            }
                            return i + 1;
                        }

                    default:
                        {
                            if (!GUI.MessageFormVisible)
                            {
                                GUI.OpenMessageForm(null, null);
                            }

                            DisplayMessage(current_pname, Event.EventData[i].Data, options);
                            break;
                        }
                }
            }

            if (i >= Event.EventData.Count)
            {
                GUI.CloseMessageForm();
                throw new EventErrorException(this, "TalkとEndが対応していません");
            }

            return i + 1;
        }
    }
}
