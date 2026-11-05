using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// ChangeTerrain・ChangeLayer コマンドで地形を変えたマスにいるユニットを配置し直すことを確かめる（7474/SRC#1462）。
    /// 元の SRC（CmdData.cls の ExecChangeTerrainCmd・ExecChangeLayerCmd）は、そのマスのユニットを
    /// StandBy で配置し直し、入れない地形なら近くの空いているマスへ移す。
    /// </summary>
    [TestClass]
    public class MapChangeTerrainStandByTests
    {
        private string tempDir;

        [TestInitialize]
        public void Setup()
        {
            tempDir = Path.Combine(Path.GetTempPath(), "SRCChangeTerrain_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(tempDir, "Bitmap", "Map"));
            // ChangeTerrain は変更先の地形の画像があることを確かめる
            File.WriteAllBytes(Path.Combine(tempDir, "Bitmap", "Map", "wall0000.bmp"), new byte[0]);
            File.WriteAllBytes(Path.Combine(tempDir, "Bitmap", "Map", "mountain0000.bmp"), new byte[0]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }

        private MapCoverageFixture CreateFixture()
        {
            var f = new MapCoverageFixture();
            var fs = new LocalFileSystem();
            fs.AddPath(tempDir);
            f.Src.FileSystem = fs;
            f.GUI.SetupBackgroundHandler = (_, _, _, _) => { };
            f.GUI.EraseUnitBitmapHandler = (_, _, _) => { };
            f.GUI.PaintUnitBitmapHandler = (_, _) => { };
            f.Src.Event.EventData = new List<EventDataLine>();
            f.Src.Event.EventCmd = new List<CmdData>();
            f.LoadGrid(
                ".....",
                ".....",
                ".....");
            f.DefineUnit("歩兵", "陸", 3);
            return f;
        }

        private static CmdData CreateCmd(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        [TestMethod]
        public void ChangeTerrain_ToImpassableTerrain_MovesUnitToNearbyCell()
        {
            var f = CreateFixture();
            var u = f.Place("歩兵", "味方", 3, 2);

            CreateCmd(f.Src, "ChangeTerrain 3 2 壁 0").Exec();

            Assert.AreEqual("壁", f.Map.Terrain(3, 2).Name);
            Assert.IsNull(f.Map.MapDataForUnit[3, 2]);
            Assert.AreEqual(1, Math.Abs(u.x - 3) + Math.Abs(u.y - 2));
            Assert.AreSame(u, f.Map.MapDataForUnit[u.x, u.y]);
            Assert.AreEqual("出撃", u.Status);
        }

        [TestMethod]
        public void ChangeTerrain_ToPassableTerrain_KeepsUnitInPlace()
        {
            var f = CreateFixture();
            var u = f.Place("歩兵", "味方", 3, 2);

            CreateCmd(f.Src, "ChangeTerrain 3 2 山 0").Exec();

            Assert.AreEqual(3, u.x);
            Assert.AreEqual(2, u.y);
            Assert.AreSame(u, f.Map.MapDataForUnit[3, 2]);
        }

        [TestMethod]
        public void ChangeTerrain_UpdatesTerrainTypeForSaveData()
        {
            // セーブデータ（中断・クイックセーブ）には地形の番号を書くので、番号も変える（元の SRC の MapData(X, Y, TerrainType)）
            var f = CreateFixture();

            CreateCmd(f.Src, "ChangeTerrain 3 2 壁 0").Exec();

            var cell = f.Map.CellAtPoint(3, 2);
            Assert.AreEqual(MapCoverageFixture.Wall, cell.TerrainType);
            cell.Restore(f.Src);
            Assert.AreEqual("壁", f.Map.Terrain(3, 2).Name);
        }

        [TestMethod]
        public void ChangeLayer_ToImpassableTerrain_MovesUnitToNearbyCell()
        {
            var f = CreateFixture();
            var u = f.Place("歩兵", "味方", 3, 2);

            CreateCmd(f.Src, "ChangeLayer 3 2 壁 0").Exec();

            Assert.IsNull(f.Map.MapDataForUnit[3, 2]);
            Assert.AreEqual(1, Math.Abs(u.x - 3) + Math.Abs(u.y - 2));
            Assert.AreSame(u, f.Map.MapDataForUnit[u.x, u.y]);
        }

        [TestMethod]
        public void ChangeLayer_DataOnly_DoesNotMoveUnit()
        {
            // 元の SRC は「情報限定」では画像を書き込まず、ユニットも配置し直さない
            var f = CreateFixture();
            var u = f.Place("歩兵", "味方", 3, 2);

            CreateCmd(f.Src, "ChangeLayer 3 2 壁 0 情報限定").Exec();

            Assert.AreEqual(3, u.x);
            Assert.AreEqual(2, u.y);
            Assert.AreSame(u, f.Map.MapDataForUnit[3, 2]);
        }
    }
}
