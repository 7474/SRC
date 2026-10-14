using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.TestLib;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// 正方形でないマップでの範囲選択（AreaInRange → SelectUnitInMask）のテスト
    /// </summary>
    [TestClass]
    public class MapAreaNonSquareTests
    {
        private static SRC CreateSrc(int width, int height)
        {
            var src = new SRC { GUI = new MockGUI() };
            // SetMapSize は GUI のマップの実装を要するため、フィールドに直接置く
            src.Map.MapWidth = width;
            src.Map.MapHeight = height;
            src.Map.MapDataForUnit = new Src2DArray<Units.Unit>(width, height);
            src.Map.MaskData = new Src2DArray<bool>(width, height);
            return src;
        }

        private static Units.Unit PlaceUnit(SRC src, string party, int x, int y)
        {
            var ud = src.UDList.IsDefined("機体") ? src.UDList.Item("機体") : src.UDList.Add("機体");
            ud.HP = 1000;
            var u = src.UList.Add("機体", 0, party);
            u.Update();
            src.Map.MapDataForUnit[x, y] = u;
            return u;
        }

        [TestMethod]
        public void AreaInRange_WideMap_DoesNotThrow()
        {
            // 幅 > 高さ のマップで、高さを超えて走査しない
            var src = CreateSrc(20, 15);
            src.Map.AreaInRange(10, 8, 30, 0, "味方");
            Assert.IsFalse(src.Map.MaskData[20, 15]);
        }

        [TestMethod]
        public void AreaInRange_TallMap_MasksUnitsBeyondWidth()
        {
            // 高さ > 幅 のマップで、幅を超える行のユニットも判定する
            var src = CreateSrc(3, 6);
            PlaceUnit(src, "敵", 1, 6);
            PlaceUnit(src, "味方", 2, 5);
            src.Map.AreaInRange(1, 1, 10, 0, "味方");
            // 味方からは敵を選べず、味方は選べる
            Assert.IsTrue(src.Map.MaskData[1, 6]);
            Assert.IsFalse(src.Map.MaskData[2, 5]);
        }
    }
}
