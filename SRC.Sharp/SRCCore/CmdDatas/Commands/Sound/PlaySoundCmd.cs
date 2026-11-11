using SRCCore.Events;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class PlaySoundCmd : CmdData
    {
        public PlaySoundCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PlaySoundCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 2 番目以降の引数をまとめてファイル名にする（スペースを含むファイル名を「"」で囲わずに書ける。元の SRC と同じ）
            string fname = GeneralLib.ListTail(EventData.Data, 2);
            if (GeneralLib.ListLength(fname) == 1)
            {
                fname = Expression.GetValueAsString(fname);
            }

            Sound.PlayWave(fname);

            return EventData.NextID;
        }
    }
}
