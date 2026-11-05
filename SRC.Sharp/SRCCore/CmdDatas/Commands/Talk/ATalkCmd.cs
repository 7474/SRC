using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;
using System.Drawing;

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
            // エラーのメッセージのコマンド名（元の SRC は Talk と AutoTalk で別の手続き）
            var cmdName = Name == CmdType.AutoTalkCmd ? "AutoTalk" : "Talk";

            for (i = EventData.ID; i < Event.EventData.Count; i++)
            {
                var currentCmd = Event.EventCmd[i];
                switch (currentCmd.Name)
                {
                    case CmdType.TalkCmd:
                    case CmdType.AutoTalkCmd:
                        {
                            // 元の SRC では、Talk 中の AutoTalk（AutoTalk 中の Talk）はメッセージの行として表示されてしまう。
                            // 書き誤りなのでエラーにする
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

                            if (Strings.Left(pname, 1) == "@")
                            {
                                // メインパイロットの強制指定
                                pname = Strings.Mid(pname, 2);
                                if (SRC.PList.IsDefined(pname))
                                {
                                    var p = SRC.PList.Item(pname);
                                    if (p.Unit != null)
                                    {
                                        pname = p.Unit.MainPilot().Name;
                                    }
                                }
                            }

                            // 話者名チェック
                            if (
                                !SRC.PList.IsDefined(pname)
                                && !SRC.PDList.IsDefined(pname)
                                && !SRC.NPDList.IsDefined(pname)
                                && !(pname == "システム")
                                && !string.IsNullOrEmpty(pname))
                            {
                                // 元の SRC の LineNum = i に当たる（エラーの行を currentCmd にする）
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

                                        case "フィルタ":
                                            {
                                                j = (j + 1);
                                                var buf = currentCmd.GetArgAsString(j);
                                                Color filterColor = ColorExtension.FromHexString(buf);
                                                j = (j + 1);
                                                // 空白のオプションをスキップ
                                                options = options
                                                    + "フィルタ " + (filterColor.IsEmpty ? Color.Transparent : filterColor).ToHexString()
                                                    + " " + currentCmd.GetArgAsString(j) + " ";
                                                break;
                                            }

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
                                        if (!GUI.MessageFormVisible)
                                        {
                                            GUI.OpenMessageForm(u1: null, u2: null);
                                        }

                                        // メッセージウィンドウのパイロット画像を以前指定された
                                        // ものに確定させる
                                        if (!string.IsNullOrEmpty(current_pname))
                                        {
                                            DisplayMessage(current_pname, "", options);
                                        }

                                        current_pname = "";
                                        break;
                                    }

                                case 2:
                                    // パイロット名のみ指定
                                    current_pname = pname;

                                    // 話者中心に画面位置を変更
                                    // 元の SRC と同じく、中央表示しないときは次の行へ進む（メッセージウィンドウはまだ開かない）
                                    // プロローグイベントやエピローグイベント時はキャンセル
                                    if (SRC.Stage == "プロローグ" || SRC.Stage == "エピローグ")
                                    {
                                        continue;
                                    }

                                    // 画面書き換え可能？
                                    if (!GUI.MainFormVisible)
                                    {
                                        continue;
                                    }
                                    if (GUI.IsPictureVisible)
                                    {
                                        continue;
                                    }
                                    if (Map.IsStatusView)
                                    {
                                        continue;
                                    }

                                    // 話者を中央表示
                                    CenterUnit(pname, without_cursor);
                                    break;

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
                                    // 元の SRC の AutoTalk は話者名を渡さない（座標が 0 なら中央表示しない）
                                    CenterUnit(Name == CmdType.AutoTalkCmd ? "" : pname, without_cursor, currentCmd.GetArgAsLong(3), currentCmd.GetArgAsLong(4));
                                    break;

                                case -1:
                                    throw new EventErrorException(currentCmd, cmdName + "コマンドのパラメータの括弧の対応が取れていません");

                                default:
                                    throw new EventErrorException(currentCmd, cmdName + "コマンドの引数の数が違います");
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
                            // 元の SRC の AutoTalk は「枠外」を戻さない
                            if (Name == CmdType.TalkCmd)
                            {
                                GUI.MessageWindowIsOut = false;
                            }
                            if (currentCmd.ArgNum != 1)
                            {
                                throw new EventErrorException(currentCmd, "End部分の引数の数が違います");
                            }
                            return i + 1;
                        }

                    case CmdType.SuspendCmd:
                        {
                            if (currentCmd.ArgNum != 1)
                            {
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
                throw new EventErrorException(this, cmdName + "とEndが対応していません");
            }

            return i + 1;
        }
    }
}
