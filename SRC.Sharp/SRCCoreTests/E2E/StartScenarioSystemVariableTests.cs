using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRCCore.E2E.Tests
{
    /// <summary>
    /// StartScenario で読み込むイベントファイル名に合わせてシステム変数を設定する（元の SRC の StartScenario）
    /// </summary>
    [TestClass]
    public class StartScenarioSystemVariableTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        private static readonly string[] Scenario =
        {
            "プロローグ:",
            "Global 確認ステージ",
            "Set 確認ステージ ステージ",
            "Quit",
            "",
        };

        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-e2e-" + Guid.NewGuid());
            // SRC.ValidateEnvironment / LoadIni が要求する最小構成
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Event"));
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Map"));
            File.WriteAllBytes(Path.Combine(appPath, "Bitmap", "Map", "plain0000.bmp"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Sound"));
            File.WriteAllBytes(Path.Combine(appPath, "Sound", "dummy.wav"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Data", "System"));
            File.WriteAllText(Path.Combine(appPath, "Data", "System", "terrain.txt"),
                "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n", new UTF8Encoding(false));
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

        private (SRC src, HeadlessGUI gui) CreateSrc()
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;

            var gui = new HeadlessGUI();
            // Ask の選択肢は1番目を選ぶ
            gui.Handle(nameof(IGUI.ListBox), _ => 1);
            gui.Install(src);
            return (src, gui);
        }

        /// <summary>
        /// Wait Click のクリック待ちに入ったら、clickAction でクリックを模擬する。
        /// </summary>
        [TestMethod]
        public void StartScenario_DefinesSystemVariables()
        {
            var (src, gui) = CreateSrc();
            Exception error = null;
            var game = new Thread(() =>
            {
                try
                {
                    src.Execute(Path.Combine(appPath, "test.eve"));
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            })
            {
                IsBackground = true,
            };
            game.Start();
            Assert.IsTrue(game.Join(Timeout), "scenario timeout");
            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error?.ToString());

            Assert.AreEqual("test.eve", src.Expression.GetValueAsString("確認ステージ"));
            Assert.AreEqual("testまでクリア.src", src.Expression.GetValueAsString("セーブデータファイル名"));
            Assert.AreEqual("", src.Expression.GetValueAsString("次ステージ"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("次ステージ"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("セーブデータファイル名"));
        }
    }
}
