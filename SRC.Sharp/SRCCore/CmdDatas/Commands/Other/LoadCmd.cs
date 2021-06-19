using SRCCore.Events;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class LoadCmd : CmdData
    {
        public LoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var new_titles = new List<string>();
            for (var i = 2; i <= ArgNum; i++)
            {
                var tname = GetArgAsString(i);
                if (!SRC.Titles.Contains(tname))
                {
                    new_titles.Add(tname);
                }
            }

            // 新規のデータがなかった？
            if (!new_titles.Any())
            {
                return EventData.NextID;
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // XXX 読み込み順序の解決がシナリオ読み込み時と大分食い違っているはず
            Event.LoadTitles(new_titles);
            foreach (var title in new_titles)
            {
                var tfolder = SRC.SearchDataFolder(title);
                if (SRC.FileSystem.FileExists(tfolder, "include.eve"))
                {
                    Event.LoadDynamic(SRC.FileSystem.PathCombine(tfolder, "include.eve"));
                }
            }

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
            return EventData.NextID;
        }
    }
}
