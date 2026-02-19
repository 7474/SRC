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
            if (ArgNum < 2)
            {
                throw new EventErrorException(this, "Questionコマンドの引数の数が違います");
            }

            var timeLimit = GetArgAsLong(2);
            var msg = ArgNum >= 3 ? GetArgAsString(3) : "さあ、どうする？";

            // 選択肢の読みこみ
            var answerList = new List<ListBoxItem>();
            CmdData hitEndCmd = null;
            foreach (var eventId in AfterEventIdRange())
            {
                var buf = Event.EventData[eventId].Data;
                Expression.FormatMessage(ref buf);
                if (Strings.Len(buf) > 0)
                {
                    if (Event.EventCmd[eventId].Name == CmdType.EndCmd)
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

            if (hitEndCmd == null)
            {
                throw new EventErrorException(this, "QuestionとEndが対応していません");
            }

            if (answerList.Any())
            {
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
