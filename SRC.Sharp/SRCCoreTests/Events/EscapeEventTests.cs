using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Coverage.Tests;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// 進入イベントに続いて発生する脱出イベント（元の SRC の Event.bas HandleEvent の「進入」）。
    /// X 方向と Y 方向を別々に判定するので、四隅では左右と上下の両方が発生する（7474/SRC#1465）。
    /// </summary>
    [TestClass]
    public class EscapeEventTests
    {
        private static readonly string[] Scenario =
        {
            "脱出 テスト W:",
            "Set 記録 (記録 & \"W\")",
            "Exit",
            "",
            "脱出 テスト E:",
            "Set 記録 (記録 & \"E\")",
            "Exit",
            "",
            "脱出 テスト N:",
            "Set 記録 (記録 & \"N\")",
            "Exit",
            "",
            "脱出 テスト S:",
            "Set 記録 (記録 & \"S\")",
            "Exit",
            "",
        };

        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-escape-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
            File.WriteAllText(Path.Combine(appPath, "test.eve"), string.Join("\r\n", Scenario), new UTF8Encoding(false));
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Directory.Delete(appPath, true);
            }
            catch
            {
                // ignore
            }
        }

        private SRC CreateSrc()
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;
            new HeadlessGUI().Install(src);
            src.ScenarioPath = appPath;
            src.TDList.Load("terrain.txt", MapCoverageFixture.ToStream(MapCoverageFixture.TerrainText));
            src.Map.LoadMapData("test.map", MapCoverageFixture.ToStream(MapCoverageFixture.BuildMapText(
                "....",
                "....",
                "....")));
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            src.Expression.DefineGlobalVariable("記録");
            src.Expression.SetVariableAsString("記録", "");
            return src;
        }

        [TestMethod]
        [DataRow(1, 1, "WN")]
        [DataRow(4, 1, "EN")]
        [DataRow(1, 3, "WS")]
        [DataRow(4, 3, "ES")]
        public void Corner_RaisesBothHorizontalAndVerticalEscape(int x, int y, string expected)
        {
            var src = CreateSrc();

            src.Event.HandleEvent("進入", "テスト", x.ToString(), y.ToString());

            Assert.AreEqual(expected, src.Expression.GetValueAsString("記録"));
        }

        [TestMethod]
        [DataRow(1, 2, "W")]
        [DataRow(4, 2, "E")]
        [DataRow(2, 1, "N")]
        [DataRow(3, 3, "S")]
        [DataRow(2, 2, "")]
        public void Edge_RaisesOneEscape(int x, int y, string expected)
        {
            var src = CreateSrc();

            src.Event.HandleEvent("進入", "テスト", x.ToString(), y.ToString());

            Assert.AreEqual(expected, src.Expression.GetValueAsString("記録"));
        }
    }
}
