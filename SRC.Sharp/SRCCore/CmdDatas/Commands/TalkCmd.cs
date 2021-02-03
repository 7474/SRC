using SRC.Core.Events;
using SRC.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas.Commands
{
    public class TalkCmd : CmdData
    {
        public TalkCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TalkCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string pname, current_pname = default;
            Unit u;
            int ux, uy;
            int i;
            int j;
            int lnum;
            var without_cursor = default(bool);
            string options = default, opt;
            string buf;
            string cname;
            int tcolor;

            for (i = EventData.ID; i < Event.EventData.Count; i++)
            {
                var currentCmd = Event.EventCmd[i];
                switch (currentCmd.Name)
                {
                    case CmdType.TalkCmd:
                        {
                            // Impl
                            //                            if (currentCmd.ArgNum > 1)
                            //                            {
                            //                                pname = currentCmd.GetArgAsString(2);
                            //                            }
                            //                            else
                            //                            {
                            //                                pname = "";
                            //                            }

                            //                            if (Strings.Left(pname, 1) == "@")
                            //                            {
                            //                                // メインパイロットの強制指定
                            //                                pname = Strings.Mid(pname, 2);
                            //                                object argIndex2 = (object)pname;
                            //                                if (SRC.PList.IsDefined(ref argIndex2))
                            //                                {
                            //                                    object argIndex1 = (object)pname;
                            //                                    {
                            //                                        var withBlock1 = SRC.PList.Item(ref argIndex1);
                            //                                        if (withBlock1.Unit_Renamed is object)
                            //                                        {
                            //                                            pname = withBlock1.Unit_Renamed.MainPilot().Name;
                            //                                        }
                            //                                    }
                            //                                }
                            //                            }

                            //                            // 話者名チェック
                            //                            bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            //                            bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                            //                            bool localIsDefined2() { object argIndex1 = (object)pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                            //                            if (!localIsDefined() & !localIsDefined1() & !localIsDefined2() & !(pname == "システム") & !string.IsNullOrEmpty(pname))
                            //                            {
                            //                                Event.EventErrorMessage = "「" + pname + "」というパイロットが定義されていません";
                            //                                //LineNum = i; // XXX これ要るの？
                            //                            }

                            //                            if (currentCmd.ArgNum > 1)
                            //                            {
                            //                                options = "";
                            //                                without_cursor = false;
                            //                                j = 2;
                            //                                lnum = 1;
                            //                                while (j <= currentCmd.ArgNum)
                            //                                {
                            //                                    opt = currentCmd.GetArgAsString(j);
                            //                                    switch (opt ?? "")
                            //                                    {
                            //                                        case "非表示":
                            //                                            {
                            //                                                without_cursor = true;
                            //                                                break;
                            //                                            }

                            //                                        case "枠外":
                            //                                            {
                            //                                                GUI.MessageWindowIsOut = true;
                            //                                                break;
                            //                                            }

                            //                                        case "白黒":
                            //                                        case "セピア":
                            //                                        case "明":
                            //                                        case "暗":
                            //                                        case "上下反転":
                            //                                        case "左右反転":
                            //                                        case "上半分":
                            //                                        case "下半分":
                            //                                        case "右半分":
                            //                                        case "左半分":
                            //                                        case "右上":
                            //                                        case "左上":
                            //                                        case "右下":
                            //                                        case "左下":
                            //                                        case "ネガポジ反転":
                            //                                        case "シルエット":
                            //                                        case "夕焼け":
                            //                                        case "水中":
                            //                                        case "通常":
                            //                                            {
                            //                                                if (j > 2)
                            //                                                {
                            //                                                    // これらのパイロット画像描画に関するオプションは
                            //                                                    // パイロット名が指定されている場合にのみ有効
                            //                                                    options = options + opt + " ";
                            //                                                }
                            //                                                else
                            //                                                {
                            //                                                    lnum = j;
                            //                                                }

                            //                                                break;
                            //                                            }

                            //                                        case "右回転":
                            //                                            {
                            //                                                j = (j + 1);
                            //                                                options = options + "右回転 " + currentCmd.GetArgAsString(j) + " ";
                            //                                                break;
                            //                                            }

                            //                                        case "左回転":
                            //                                            {
                            //                                                j = (j + 1);
                            //                                                options = options + "左回転 " + currentCmd.GetArgAsString(j) + " ";
                            //                                                break;
                            //                                            }

                            //                                        case "フィルタ":
                            //                                            {
                            //                                                j = (j + 1);
                            //                                                buf = currentCmd.GetArgAsString(j);
                            //                                                cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                            //                                                StringType.MidStmtStr(ref cname, 1, 2, "&H");
                            //                                                var midTmp = Strings.Mid(buf, 6, 2);
                            //                                                StringType.MidStmtStr(ref cname, 3, 2, midTmp);
                            //                                                var midTmp1 = Strings.Mid(buf, 4, 2);
                            //                                                StringType.MidStmtStr(ref cname, 5, 2, midTmp1);
                            //                                                var midTmp2 = Strings.Mid(buf, 2, 2);
                            //                                                StringType.MidStmtStr(ref cname, 7, 2, midTmp2);
                            //                                                tcolor = Conversions.ToInteger(cname);
                            //                                                j = (j + 1);
                            //                                                // 空白のオプションをスキップ
                            //                                                options = options + "フィルタ " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)tcolor) + " " + currentCmd.GetArgAsString(j) + " ";
                            //                                                break;
                            //                                            }

                            //                                        case var @case when @case == "":
                            //                                            {
                            //                                                break;
                            //                                            }

                            //                                        default:
                            //                                            {
                            //                                                // 通常の引数をスキップ
                            //                                                lnum = j;
                            //                                                break;
                            //                                            }
                            //                                    }

                            //                                    j = (j + 1);
                            //                                }
                            //                            }
                            //                            else
                            //                            {
                            //                                lnum = 1;
                            //                            }

                            //                            switch (lnum)
                            //                            {
                            //                                case 0:
                            //                                case 1:
                            //                                    {
                            //                                        // 引数なし

                            //                                        if (!My.MyProject.Forms.frmMessage.Visible)
                            //                                        {
                            //                                            Unit argu1 = null;
                            //                                            Unit argu2 = null;
                            //                                            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                            //                                        }

                            //                                        // メッセージウィンドウのパイロット画像を以前指定された
                            //                                        // ものに確定させる
                            //                                        if (!string.IsNullOrEmpty(current_pname))
                            //                                        {
                            //                                            GUI.DisplayMessage(ref current_pname, "", options);
                            //                                        }

                            //                                        current_pname = "";
                            //                                        break;
                            //                                    }

                            //                                case 2:
                            //                                    {
                            //                                        // パイロット名のみ指定
                            //                                        current_pname = pname;

                            //                                        // 話者中心に画面位置を変更

                            //                                        // プロローグイベントやエピローグイベント時はキャンセル
                            //                                        if (SRC.Stage == "プロローグ" | SRC.Stage == "エピローグ")
                            //                                        {
                            //                                            goto NextLoop;
                            //                                        }

                            //                                        // 画面書き換え可能？
                            //                                        if (!GUI.MainForm.Visible)
                            //                                        {
                            //                                            goto NextLoop;
                            //                                        }

                            //                                        if (GUI.IsPictureVisible)
                            //                                        {
                            //                                            goto NextLoop;
                            //                                        }

                            //                                        if (string.IsNullOrEmpty(Map.MapFileName))
                            //                                        {
                            //                                            goto NextLoop;
                            //                                        }

                            //                                        // 話者を中央表示
                            //                                        CenterUnit(pname, without_cursor);
                            //                                        break;
                            //                                    }

                            //                                case 3:
                            //                                    {
                            //                                        current_pname = pname;
                            //                                        switch (currentCmd.GetArgAsString(3) ?? "")
                            //                                        {
                            //                                            case "母艦":
                            //                                                {
                            //                                                    // 母艦の中央表示
                            //                                                    CenterUnit("母艦", without_cursor);
                            //                                                    break;
                            //                                                }

                            //                                            case "中央":
                            //                                                {
                            //                                                    // 話者の中央表示
                            //                                                    CenterUnit(pname, without_cursor);
                            //                                                    break;
                            //                                                }

                            //                                            case "固定":
                            //                                                {
                            //                                                    break;
                            //                                                }
                            //                                                // 表示位置固定
                            //                                        }

                            //                                        break;
                            //                                    }

                            //                                case 4:
                            //                                    {
                            //                                        // 表示の座標指定あり
                            //                                        current_pname = pname;
                            //                                        CenterUnit(pname, without_cursor, currentCmd.GetArgAsLong(3), currentCmd.GetArgAsLong(4));
                            //                                        break;
                            //                                    }

                            //                                case -1:
                            //                                    {
                            //                                        Event.EventErrorMessage = "Talkコマンドのパラメータの括弧の対応が取れていません";
                            //                                        //LineNum = i; // XXX これ要るの？
                            //                                        ;
                            //#error Cannot convert ErrorStatementSyntax - see comment for details
                            //                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 511026


                            //                                        Input:
                            //                                                                        Error(0)

                            //                                         */
                            //                                        break;
                            //                                    }

                            //                                default:
                            //                                    {
                            //                                        Event.EventErrorMessage = "Talkコマンドの引数の数が違います";
                            //                                        //LineNum = i; // XXX これ要るの？
                            //                                        ;
                            //#error Cannot convert ErrorStatementSyntax - see comment for details
                            //                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 511176


                            //                                        Input:
                            //                                                                        Error(0)

                            //                                         */
                            //                                        break;
                            //                                    }
                            //                            }

                            //                            if (!My.MyProject.Forms.frmMessage.Visible)
                            //                            {
                            //                                Unit argu11 = null;
                            //                                Unit argu21 = null;
                            //                                GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                            //                            }

                            GUI.OpenMessageForm();
                            break;
                        }

                    case CmdType.EndCmd:
                        {
                            GUI.CloseMessageForm();
                            GUI.MessageWindowIsOut = false;
                            if (currentCmd.ArgNum != 1)
                            {
                                Event.EventErrorMessage = "End部分の引数の数が違います";
                                ////LineNum = i; // XXX これ要るの？ // XXX これ要るの？
                                throw new Exception();
                            }
                            break;
                        }

                    case CmdType.SuspendCmd:
                        {
                            if (currentCmd.ArgNum != 1)
                            {
                                Event.EventErrorMessage = "Suspend部分の引数の数が違います";
                                //LineNum = i; // XXX これ要るの？
                                throw new Exception();
                            }
                            break;
                        }

                    default:
                        {
                            //if (!My.MyProject.Forms.frmMessage.Visible)
                            //{
                            //    Unit argu12 = null;
                            //    Unit argu22 = null;
                            //    GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                            //}

                            GUI.OpenMessageForm();
                            GUI.DisplayMessage(current_pname, Event.EventData[i].Data, options);
                            break;
                        }
                }
            }

            if (i >= Event.EventData.Count)
            {
                // Impl
                //GUI.CloseMessageForm();
                //Event.EventErrorMessage = "TalkとEndが対応していません";
                throw new Exception();
            }

            return i + 1;
        }
    }
}
