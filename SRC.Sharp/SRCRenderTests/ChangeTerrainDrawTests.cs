using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// ChangeTerrain・ChangeLayer は変えたマスだけを背景に描き直す（7474/SRC#1492）。
    /// 元の SRC（CmdData.cls の ExecChangeTerrainCmd・ExecChangeLayerCmd）は、そのマスの画像を
    /// 今の描画モード（夜・セピアなど）で picBack に書き込むだけなので、背景のほかのマスに描いたものは残り、
    /// マップの表示位置も変わらない。
    /// </summary>
    [TestClass]
    public class ChangeTerrainDrawTests
    {
        private const int Cell = 32;

        private static SKColor CellPixel(SKBitmap bitmap, int x, int y)
        {
            // マップは左上から表示している（ChangeMap で中央に合わせた位置）
            return bitmap.GetPixel(Cell * (x - 1) + Cell / 2, Cell * (y - 1) + Cell / 2);
        }

        [TestMethod]
        public void PlayScenario_ChangeTerrainKeepsBackgroundDrawing()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                File.WriteAllBytes(Path.Combine(workspace.AppPath, "Bitmap", "Map", "sea0000.bmp"), new byte[0]);
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    "ChangeTerrain 5 5 海 0",
                    "Refresh",
                    "Talk システム",
                    "昼",
                    "End",
                    "Night",
                    "Refresh",
                    // 背景に線を描いてから地形を変える
                    "DrawOption 背景",
                    "Line 0 40 300 40 #FF0000",
                    "DrawOption 通常",
                    "ChangeTerrain 6 5 海 0",
                    "ChangeLayer 7 5 海 0",
                    // 背景から画面を描き直す
                    "Redraw",
                    "Talk システム",
                    "夜",
                    "End",
                    // 背景を全部描き直す
                    "Night",
                    "Refresh",
                    "Talk システム",
                    "全体",
                    "End",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("昼", runner.WaitForMessage());
                SKColor daySea, dayPlain;
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    daySea = CellPixel(bitmap, 5, 5);
                    dayPlain = CellPixel(bitmap, 4, 5);
                    Assert.AreNotEqual(dayPlain, daySea);
                }
                runner.ClickMessage();

                SKColor changedTerrain, changedLayer;
                Assert.AreEqual("夜", runner.WaitForMessage());
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 背景に描いた線は残る
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100, 40));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(250, 40));
                    // 変えたマスは、Night で全体を描き直したマスと同じく夜の色で描く
                    var nightSea = CellPixel(bitmap, 5, 5);
                    Assert.AreNotEqual(daySea, nightSea);
                    Assert.AreEqual(nightSea, CellPixel(bitmap, 6, 5));
                    changedTerrain = CellPixel(bitmap, 6, 5);
                    changedLayer = CellPixel(bitmap, 7, 5);
                    // レイヤーを変えたマスも描き直している
                    Assert.AreNotEqual(CellPixel(bitmap, 4, 5), changedLayer);
                }
                runner.ClickMessage();

                Assert.AreEqual("全体", runner.WaitForMessage());
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 変えたマス（地形・レイヤー）は、背景を全部描き直したときと同じ絵
                    Assert.AreEqual(CellPixel(bitmap, 6, 5), changedTerrain);
                    Assert.AreEqual(CellPixel(bitmap, 7, 5), changedLayer);
                }
                runner.ClickMessage();

                runner.WaitForTermination();
            }
        }
    
        /// <summary>
        /// ClearLayer もレイヤーを消したマスだけを描き直す（7474/SRC#1494）。
        /// </summary>
        [TestMethod]
        public void PlayScenario_ClearLayerKeepsBackgroundDrawing()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                File.WriteAllBytes(Path.Combine(workspace.AppPath, "Bitmap", "Map", "sea0000.bmp"), new byte[0]);
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    "ChangeLayer 7 5 海 0",
                    "Sepia",
                    "DrawOption 背景",
                    "Line 0 40 300 40 #FF0000",
                    "DrawOption 通常",
                    "ClearLayer 7 5",
                    "Redraw",
                    "Talk システム",
                    "消去",
                    "End",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("消去", runner.WaitForMessage());
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 背景に描いた線は残る
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100, 40));
                    // レイヤーを消したマスは、セピアの平地に戻る
                    Assert.AreEqual(CellPixel(bitmap, 4, 5), CellPixel(bitmap, 7, 5));
                }
                runner.ClickMessage();

                runner.WaitForTermination();
            }
        }
    }
}
