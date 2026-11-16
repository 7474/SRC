using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainDataList.DefinePlaceholder のユニットテスト
    /// </summary>
    [TestClass]
    public class TerrainDataListPlaceholderTests
    {
        [TestMethod]
        public void DefinePlaceholder_UndefinedId_DefinesLandTerrain()
        {
            var src = new SRC { GUI = new MockGUI() };
            Assert.IsFalse(src.TDList.IsDefined(5));

            var td = src.TDList.DefinePlaceholder(5);

            Assert.IsTrue(src.TDList.IsDefined(5));
            Assert.AreSame(td, src.TDList.Item(5));
            Assert.AreEqual(5, td.ID);
            Assert.AreEqual("陸", td.Class);
            Assert.AreEqual(2, td.MoveCost);
            Assert.AreEqual(0, td.HitMod);
            Assert.AreEqual(0, td.DamageMod);
        }

        [TestMethod]
        public void DefinePlaceholder_DefinedId_ReturnsExisting()
        {
            var src = new SRC { GUI = new MockGUI() };
            var first = src.TDList.DefinePlaceholder(7);
            var second = src.TDList.DefinePlaceholder(7);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void ItemByName_SkipsUndefinedIds()
        {
            // 定義されていない番号（null）を挟んでいても名前で引ける（ChangeTerrain で使う）
            var src = new SRC { GUI = new MockGUI() };
            src.TDList.DefinePlaceholder(3);
            var td = src.TDList.DefinePlaceholder(10);

            Assert.AreSame(td, src.TDList.ItemByName("地形10"));
            Assert.IsNull(src.TDList.ItemByName("コンテナ"));
        }

        private sealed class StubGUIMap : IGUIMap
        {
            public void InitMapSize(int w, int h) { }
            public void SetMapSize(int w, int h) { }
        }

        [TestMethod]
        public void AreaInSpeed_PlaceholderTerrain_MatchesUnitSpeed()
        {
            // AllowMissingAssets で登録した仮の地形（移動コスト1 = MoveCost 2）では移動範囲が移動力と一致する
            var gui = new MockGUI { MainWidth = 15, MainHeight = 15 };
            gui.RedrawScreenHandler = _ => { };
            var src = new SRC { GUI = gui, GUIMap = new StubGUIMap(), AllowMissingAssets = true };
            const int w = 11;
            const int h = 11;
            src.Map.SetMapSize(w, h);

            var td = src.TDList.DefinePlaceholder(0);
            for (var x = 1; x <= w; x++)
            {
                for (var y = 1; y <= h; y++)
                {
                    src.Map.MapData[x, y].UnderTerrain = td;
                }
            }

            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(
                "歩兵\r\n歩兵, ほへい, テスト, 1, 1\r\n陸, 3, M, 100, 10\r\n特殊能力なし\r\n1000, 100, 100, 100\r\nAAAA, dummy.bmp\r\n\r\n")))
            {
                src.UDList.Load("unit.txt", stream);
            }
            var u = src.UList.Add("歩兵", 0, "味方");
            u.Update();
            u.x = 6;
            u.y = 6;
            u.Area = "地上";
            u.Status = "出撃";
            src.Map.MapDataForUnit[6, 6] = u;

            src.Map.AreaInSpeed(u);

            // 移動力 3 なので距離 3 は移動でき、距離 4 はマスクされる（MoveCost = 1 のときのような倍化は起きない）
            Assert.IsFalse(src.Map.MaskData[9, 6]);
            Assert.IsTrue(src.Map.MaskData[10, 6]);
            Assert.AreEqual(6, src.Map.TotalMoveCost[9, 6]);
        }
    }
}
