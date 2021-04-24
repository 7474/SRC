using SRCCore.Events;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class PlaySoundCmd : CmdData
    {
        public PlaySoundCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PlaySoundCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string fname = GeneralLib.ToList(EventData.Data).Skip(1).FirstOrDefault();
            if (GeneralLib.ListLength(fname) == 1)
            {
                fname = Expression.GetValueAsString(fname);
            }

            Sound.PlayWave(fname);

            return EventData.ID + 1;
        }
    }
}
