using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class QuestionCmd : CmdData
    {
        public QuestionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuestionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 選択肢の読みこみ
            var answerList = new List<ListBoxItem>();
            CmdData hitEndCmd = null;
            foreach (var eventId in AfterEventIdRange())
            {
                var buf = Event.EventData[eventId].Data;
                Expression.FormatMessage(ref buf);
                if (Strings.Len(buf) > 0)
                {
                    // 元の SRC と同じく、式置換の後の行が「End」（大文字・小文字は区別しない）のときに終わる。
                    // 「End」に引数がある行は選択肢になる
                    if (Strings.LCase(buf) == "end")
                    {
                        hitEndCmd = Event.EventCmd[eventId];
                        break;
                    }

                    answerList.Add(new ListBoxItem
                    {
                        Text = buf,
                        ListItemID = (eventId - EventData.ID).ToString(),
                        ListItemFlag = false,
                    });
                }
            }

            // 元の SRC は End が無いとき（判定の誤りで）エラーにせず最後の行まで選択肢にするが、SRC# はエラーにする
            if (hitEndCmd == null)
            {
                throw new EventErrorException(this, "QuestionとEndが対応していません");
            }

            if (answerList.Any())
            {
                // 元の SRC と同じく、引数の数は選択肢があるときだけ確かめる
                string msg;
                switch (ArgNum)
                {
                    case 3:
                        msg = GetArgAsString(3);
                        break;
                    case 2:
                        msg = "さあ、どうする？";
                        break;
                    default:
                        throw new EventErrorException(this, "Questionコマンドの引数の数が違います");
                }
                var timeLimit = GetArgAsLong(2);

                Commands.SelectedItem = GUI.LIPS(new ListBoxArgs
                {
                    lb_caption = "選択",
                    Items = answerList,
                    lb_info = msg,
                }, timeLimit);
                Event.SelectedAlternative = Commands.SelectedItem > 0
                    ? answerList[Commands.SelectedItem - 1].ListItemID
                    : "0";
            }
            else
            {
                Commands.SelectedItem = 0;
                Event.SelectedAlternative = "0";
            }

            return hitEndCmd.EventData.NextID;
        }
    }
}
