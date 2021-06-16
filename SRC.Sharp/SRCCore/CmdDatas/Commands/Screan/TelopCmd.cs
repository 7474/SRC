using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class TelopCmd : CmdData
    {
        public TelopCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TelopCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var msg = GeneralLib.ListTail(EventData.Data, 2);
            if (GeneralLib.ListLength(msg) == 1)
            {
                msg = Expression.GetValueAsString(msg);
            }

            Expression.FormatMessage(ref msg);
            var BGM = Sound.SearchMidiFile(Sound.BGMName("Subtitle"));
            if (Strings.Len(BGM) > 0)
            {
                Sound.StartBGM(BGM, false);
                if (!GUI.IsRButtonPressed())
                {
                    GUI.Sleep(1000);
                }

                GUI.DisplayTelop(msg);
                if (!GUI.IsRButtonPressed())
                {
                    GUI.Sleep(2000);
                }
            }
            else
            {
                GUI.DisplayTelop(msg);
            }

            return EventData.NextID;
        }
    }
}
