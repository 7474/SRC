using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 拡張データフォルダ（srcs.json の ExtDataPath）だけに SRC 本体の素材がある場合。
    /// SRC# を SRC 本体と別のフォルダに置き、拡張データフォルダで SRC 本体を指す使い方。
    /// </summary>
    [TestClass]
    public class ExtDataPathTests
    {
        [TestMethod]
        public void SystemDataAndAssets_AreReadFromExtDataPath()
        {
            using (var app = new ScenarioWorkspace(withAssets: false))
            using (var ext = new ScenarioWorkspace(withAssets: true))
            {
                // 地形データ（Data\System\terrain.txt）・マップの画像・BGM は拡張データフォルダにだけある
                ext.Write(Path.Combine("Midi", "Map1.mid"), "x");
                app.Write("srcs.json", "{ \"ExtDataPath\": " + Newtonsoft.Json.JsonConvert.SerializeObject(ext.AppPath) + " }");
                Directory.CreateDirectory(Path.Combine(app.AppPath, "scenario"));
                app.WriteMap(Path.Combine("scenario", "test.map"), 5, 5, (x, y) => 0);
                app.Write(Path.Combine("scenario", "test.eve"), "スタート:", "ChangeMap test.map", "Talk システム", "待機", "End", "Quit", "");

                using (var runner = new ScreenScenarioRunner(app.AppPath))
                {
                    runner.Start(Path.Combine(app.AppPath, "scenario", "test.eve"));
                    runner.WaitForMessage();
                    var src = runner.Session.SRC;

                    Assert.AreEqual(ext.AppPath, src.ExtDataPath);
                    // 仮の地形（地形0・画像なし）ではなく、拡張データフォルダの地形データを使う
                    Assert.AreEqual("平地", src.TDList.Item(0).Name);
                    Assert.AreEqual("plain", src.TDList.Item(0).Bitmap);
                    StringAssert.EndsWith(src.Map.SearchTerrainImageFile(src.Map.MapData[1, 1]).Replace('\\', '/'), "Bitmap/Map/plain0000.bmp");
                    StringAssert.EndsWith(src.Sound.SearchMidiFile("Map1.mid").Replace('\\', '/'), "Midi/Map1.mid");
                }
            }
        }
    }
}
