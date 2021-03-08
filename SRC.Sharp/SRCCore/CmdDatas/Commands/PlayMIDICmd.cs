using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class PlayMIDICmd : CmdData
    {
        public PlayMIDICmd(SRC src, EventDataLine eventData) : base(src, CmdType.PlayMIDICmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecPlayMIDICmdRet = default;
            var fname = default(string);
            int play_bgm_end;
            int i;

            // PlayMIDIコマンドが連続してる場合、最後のPlayMIDIコマンドの位置を検索
            var loopTo = Information.UBound(Event_Renamed.EventCmd);
            for (i = LineNum + 1; i <= loopTo; i++)
            {
                if (Event_Renamed.EventCmd[i].Name != Event_Renamed.CmdType.PlayMIDICmd)
                {
                    break;
                }
            }

            play_bgm_end = i - 1;

            // 最後のSPlayMIDIから順にMIDIファイルを検索
            var loopTo1 = LineNum;
            for (i = play_bgm_end; i >= loopTo1; i -= 1)
            {
                fname = GeneralLib.ListTail(ref Event_Renamed.EventData[i], 2);
                if (GeneralLib.ListLength(ref fname) == 1)
                {
                    if (Strings.Left(fname, 2) == "$(")
                    {
                        fname = "\"" + fname + "\"";
                    }

                    fname = Expression.GetValueAsString(ref fname, true);
                }
                else
                {
                    fname = "(" + fname + ")";
                }

                fname = Sound.SearchMidiFile(ref fname);
                if (!string.IsNullOrEmpty(fname))
                {
                    // MIDIファイルが存在したので選択
                    break;
                }
            }

            // MIDIファイルを再生
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StartBGM(ref fname, false);

            // 次のコマンド実行位置は最後のPlayMIDIコマンドの後
            ExecPlayMIDICmdRet = play_bgm_end + 1;
            return ExecPlayMIDICmdRet;
            return EventData.ID + 1;
        }
    }
}
