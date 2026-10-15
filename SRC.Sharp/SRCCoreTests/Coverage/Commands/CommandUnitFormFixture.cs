using SRCCore.CmdDatas;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.Events;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// CommandUnitForm 系テスト（Command.unitform / preview / unitcommand / unitxxx）の共通準備。
    /// HeadlessGUI で GUI 呼び出しを記録し、地形付きのマップとユニット・パイロットを用意する。
    /// </summary>
    internal sealed class CommandUnitFormFixture : IDisposable
    {
        private string eventDir;
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }
        public TerrainData Plain { get; }

        /// <summary>
        /// GUI.Confirm に渡されたメッセージとタイトル。
        /// </summary>
        public List<(string message, string title)> Confirms { get; } = new List<(string, string)>();

        /// <summary>
        /// GUI.ListBox に渡された引数。
        /// </summary>
        public List<ListBoxArgs> ListBoxes { get; } = new List<ListBoxArgs>();

        /// <summary>
        /// Confirm の応答（既定は OK）。
        /// </summary>
        public Func<string, GuiDialogResult> ConfirmAnswer { get; set; } = _ => GuiDialogResult.Ok;

        /// <summary>
        /// ListBox の応答（既定はキャンセル=0）。
        /// </summary>
        public Func<ListBoxArgs, int> ListBoxAnswer { get; set; } = _ => 0;

        /// <param name="onMap">true ならマップ上（MapFileName あり）、false ならユニットステータス表示（インターミッション）</param>
        public CommandUnitFormFixture(bool onMap, int width = 8, int height = 8)
        {
            Gui = new HeadlessGUI();
            Src = new SRC();
            Gui.Install(Src);
            // 効果音などのファイル検索用（パス未登録なのでファイルは見つからない）
            Src.FileSystem = new LocalFileSystem();
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();

            Gui.Handle("Confirm", args =>
            {
                Confirms.Add(((string)args[0], (string)args[1]));
                return ConfirmAnswer((string)args[0]);
            });
            Gui.Handle("ListBox", args =>
            {
                var lb = (ListBoxArgs)args[0];
                ListBoxes.Add(lb);
                return ListBoxAnswer(lb);
            });

            // インターミッションのステータス表示用マップ（MakeUnitList）は MainWidth x MainHeight で作られる
            Src.GUI.MainWidth = 15;
            Src.GUI.MainHeight = 15;

            Plain = new TerrainData { ID = 0, Name = "平地", Class = "陸", MoveCost = 2 };
            Src.Map.SetMapSize(width, height);
            SetTerrainAll(Plain);
            Src.Map.MapFileName = onMap ? "test.map" : "";
        }

        public void SetTerrainAll(TerrainData td)
        {
            for (var x = 1; x <= Src.Map.MapWidth; x++)
            {
                for (var y = 1; y <= Src.Map.MapHeight; y++)
                {
                    SetTerrain(x, y, td);
                }
            }
        }

        public void SetTerrain(int x, int y, TerrainData td)
        {
            var cell = Src.Map.MapData[x, y];
            cell.TerrainType = td.ID;
            cell.UnderTerrain = td;
        }

        public UnitData AddUnitData(string name, params string[] features)
        {
            var ud = Src.UDList.Add(name);
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 800;
            ud.Mobility = 70;
            ud.Speed = 4;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        /// <summary>
        /// 地形適応 AAAA のパイロットデータを追加する（ユニットの地形適応はパイロットの適応と合成される）。
        /// </summary>
        public PilotData AddPilotData(string pname)
        {
            var pd = Src.PDList.Add(pname);
            pd.Adaption = "AAAA";
            return pd;
        }

        /// <summary>
        /// パイロットを乗せたユニットを作成する。x, y が正ならマップに出撃させる。
        /// </summary>
        public Unit CreateUnit(string uname, string pname, string party = "味方", int x = 0, int y = 0)
        {
            if (!Src.PDList.IsDefined(pname))
            {
                AddPilotData(pname);
            }
            var u = Src.UList.Add(uname, 0, party);
            var p = Src.PList.Add(pname, 1, party);
            u.AddPilot(p);
            p.Unit = u;
            u.Update();
            u.FullRecover();
            if (x > 0 && y > 0)
            {
                u.StandBy(x, y);
            }
            return u;
        }

        /// <summary>
        /// インターミッションのユニットリスト表示と同様に、ステータス表示用マップへユニットを並べる。
        /// </summary>
        public void MakeStatusViewUnitList()
        {
            Src.Event.MakeUnitList(smode: "");
        }

        /// <summary>
        /// 一時ディレクトリにイベントファイルを書き出して読み込む（Dispose で削除）。
        /// 変数「記録」を初期化しておく。
        /// </summary>
        public void LoadEvents(params string[] lines)
        {
            LoadEventsCore(true, lines);
        }

        /// <summary>
        /// ダミーのラベルを置かずにイベントファイルを読み込む（先頭行のラベルの扱いを確認する用）。
        /// </summary>
        public void LoadEventsWithoutLeadingLabel(params string[] lines)
        {
            LoadEventsCore(false, lines);
        }

        private void LoadEventsCore(bool withLeadingLabel, string[] lines)
        {
            eventDir = Path.Combine(Path.GetTempPath(), "srcs-cuf-" + Guid.NewGuid());
            Directory.CreateDirectory(eventDir);
            var fname = Path.Combine(eventDir, "test.eve");
            // 先頭行（EventDataId = 0）のラベルは IsEventDefined の検索開始位置(1)より前になるため、ダミーのラベルを置く
            var all = (withLeadingLabel ? new[] { "ダミー:", "Exit" } : new string[0]).Concat(lines);
            File.WriteAllText(fname, string.Join("\r\n", all) + "\r\n", new UTF8Encoding(false));
            Src.SystemConfig = new LocalFileConfig { AppPath = eventDir };
            var fs = new LocalFileSystem();
            fs.AddPath(eventDir);
            Src.FileSystem = fs;
            Src.ScenarioPath = eventDir;
            Src.Event.InitEventData();
            Src.Event.LoadEventData(fname);
            Src.Expression.DefineGlobalVariable("記録");
            Src.Expression.SetVariableAsString("記録", "");
        }

        public string Record => Src.Expression.GetValueAsString("記録");

        public void Dispose()
        {
            if (eventDir != null)
            {
                try
                {
                    Directory.Delete(eventDir, true);
                }
                catch
                {
                    // ignore
                }
            }
        }

        public int CountCalls(string member)
        {
            return Gui.CallsOf(member).Count();
        }
    }
}
