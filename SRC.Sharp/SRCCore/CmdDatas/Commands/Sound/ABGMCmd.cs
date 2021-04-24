using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public abstract class ABGMCmd : CmdData
    {
        public ABGMCmd(SRC src, CmdType cmdType, EventDataLine eventData) : base(src, cmdType, eventData)
        {
        }

        protected abstract bool Repeat { get; }

        protected override int ExecInternal()
        {
            string fname = "";
            int play_bgm_end;

            // PlayMIDIコマンドが連続してる場合、最後のPlayMIDIコマンドの位置を検索
            var playCmds = Event.EventCmd
                .Skip(EventData.ID)
                .TakeWhile(x => x.Name == Name)
                .ToList();
            play_bgm_end = playCmds.Max(x => x.EventData.ID);

            // 最後のSPlayMIDIから順にMIDIファイルを検索
            foreach (var cmd in playCmds.Reverse<CmdData>())
            {
                var fnameList = GeneralLib.ToList(cmd.EventData.Data).Skip(1).ToList();
                fname = string.Join(" ", fnameList);
                if (fnameList.Count == 1)
                {
                    if (Strings.Left(fname, 2) == "$(")
                    {
                        fname = "\"" + fname + "\"";
                    }

                    fname = Expression.GetValueAsString(fname, true);
                }
                else
                {
                    fname = "(" + fname + ")";
                }

                fname = Sound.SearchMidiFile(fname);
                if (!string.IsNullOrEmpty(fname))
                {
                    // MIDIファイルが存在したので選択
                    break;
                }
            }
            // MIDIファイルを再生
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StartBGM(fname, Repeat);

            // 次のコマンド実行位置は最後のPlayMIDIコマンドの後
            return play_bgm_end + 1;
        }
    }
}
