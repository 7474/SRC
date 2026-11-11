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
                    SRC.Titles.Add(tname);
                }
            }

            // 新規のデータがなかった？
            if (!new_titles.Any())
            {
                return EventData.NextID;
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            var cur_data_size = Event.EventData.Count;

            // 使用しているタイトルのデータをロード
            // VB と同じく、タイトルごとにデータと include.eve を交互に読む。
            // include.eve は読み込み済みか（EventFileNames）を見ずに読む
            foreach (var title in new_titles)
            {
                SRC.IncludeData(title);
                var tfolder = SRC.SearchDataFolder(title);
                // フォルダが無ければ IncludeData が SRC を終了する。VB の FileExists("\include.eve") はドライブの最上位を見る。
                // SRC# で空のフォルダと結合すると相対パスになり、シナリオのフォルダの include.eve を読んでしまうので飛ばす
                if (string.IsNullOrEmpty(tfolder))
                {
                    continue;
                }
                Event.LoadEventData2IfExist(SRC.FileSystem.PathCombine(tfolder, "include.eve"), EventDataSource.Scenario);
            }

            // ローカルデータの読みこみ
            LoadLocalData();

            // 新しい行だけのラベルを登録し、構文解析する
            Event.RegisterAndParseAddedEventData(cur_data_size);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
            return EventData.NextID;
        }

        // VB の ExecLoadCmd が読み直すローカルデータ。
        // SRC.LoadLocalData と違い、effect・animation・ext_animation は読まず、ADList.AddDefaultAnimation も呼ばない
        private void LoadLocalData()
        {
            var fs = SRC.FileSystem;
            var dataPath = fs.PathCombine(SRC.ScenarioPath, "Data");
            string LocalFile(string name) => fs.PathCombine(dataPath, name);

            if (fs.FileExists(LocalFile("alias.txt")))
            {
                SRC.ALDList.Load(LocalFile("alias.txt"));
            }
            if (fs.FileExists(LocalFile("sp.txt")))
            {
                SRC.SPDList.Load(LocalFile("sp.txt"));
            }
            else if (fs.FileExists(LocalFile("mind.txt")))
            {
                SRC.SPDList.Load(LocalFile("mind.txt"));
            }
            if (fs.FileExists(LocalFile("pilot.txt")))
            {
                SRC.PDList.Load(LocalFile("pilot.txt"));
            }
            if (fs.FileExists(LocalFile("non_pilot.txt")))
            {
                SRC.NPDList.Load(LocalFile("non_pilot.txt"));
            }
            if (fs.FileExists(LocalFile("robot.txt")))
            {
                SRC.UDList.Load(LocalFile("robot.txt"));
            }
            if (fs.FileExists(LocalFile("unit.txt")))
            {
                SRC.UDList.Load(LocalFile("unit.txt"));
            }

            if (fs.FileExists(LocalFile("pilot_message.txt")))
            {
                SRC.MDList.Load(LocalFile("pilot_message.txt"), false);
            }
            if (fs.FileExists(LocalFile("pilot_dialog.txt")))
            {
                SRC.DDList.Load(LocalFile("pilot_dialog.txt"));
            }
            if (fs.FileExists(LocalFile("item.txt")))
            {
                SRC.IDList.Load(LocalFile("item.txt"));
            }
        }
    }
}
