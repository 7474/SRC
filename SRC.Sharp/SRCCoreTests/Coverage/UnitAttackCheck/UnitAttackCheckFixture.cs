using SRCCore.Lib;
using SRCCore.TestLib;
using SRCCore.Units;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.attackcheck.cs のテスト用に、データテキストから実ユニットを組み立てて
    /// マップ上に配置するためのヘルパー。GUI 呼び出しは HeadlessGUI に記録する。
    /// </summary>
    internal sealed class UnitAttackCheckFixture
    {
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }

        public UnitAttackCheckFixture(int width = 10, int height = 10)
        {
            Src = new SRC();
            Gui = new HeadlessGUI();
            Gui.Install(Src);
            // 効果音の検索でファイルシステムを参照するため設定しておく（ファイルは置かない）
            Src.FileSystem = new SRCCore.Filesystem.LocalFileSystem();
            // SetMapSize は GUI のマップ実装を要するため、フィールドに直接置く
            Src.Map.MapWidth = width;
            Src.Map.MapHeight = height;
            Src.Map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            Src.Map.MaskData = new Src2DArray<bool>(width, height);
        }

        /// <summary>
        /// 全マスを平地（陸、移動コスト1）で埋める。強制移動系の判定で地形を参照するため。
        /// </summary>
        public void FillPlainTerrain()
        {
            var plain = new SRCCore.Models.TerrainData { ID = 0, Name = "平地", Class = "陸", MoveCost = 1 };
            var w = Src.Map.MapWidth;
            var h = Src.Map.MapHeight;
            Src.Map.MapData = new Src2DArray<SRCCore.Maps.MapCell>(w, h);
            for (var i = 1; i <= w; i++)
            {
                for (var j = 1; j <= h; j++)
                {
                    Src.Map.MapData[i, j] = new SRCCore.Maps.MapCell { X = i, Y = j, UnderTerrain = plain };
                }
            }
        }

        public void LoadPilots(string text)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            Src.PDList.Load("pilot.txt", stream);
        }

        public void LoadUnits(string text)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            Src.UDList.Load("unit.txt", stream);
        }

        /// <summary>
        /// スペシャルパワーを効果付きで定義し、ユニットをその影響下に置く。
        /// </summary>
        public void ApplySpecialPower(Unit u, string spname, string effect)
        {
            if (!Src.SPDList.IsDefined(spname))
            {
                var sd = Src.SPDList.Add(spname);
                sd.SetEffect(effect);
            }
            u.MakeSpecialPowerInEffect(spname);
        }

        /// <summary>
        /// ユニットを作成してパイロットを乗せ、(x, y) に配置する。
        /// </summary>
        public Unit Place(string uname, string pname, string party, int x, int y)
        {
            var u = Src.UList.Add(uname, 0, party);
            var p = Src.PList.Add(pname, 1, party);
            p.Ride(u);
            u.FullRecover();
            u.x = x;
            u.y = y;
            u.Status = "出撃";
            u.Area = "地上";
            Src.Map.MapDataForUnit[x, y] = u;
            return u;
        }

        public string[] SysMessages()
        {
            return Gui.CallsOf(nameof(IGUI.DisplaySysMessage)).Select(c => (string)c.Args[0]).ToArray();
        }
    }
}
