using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission.SearchLibFile のテスト。
    /// 元の SRC（Intermission.bas の InterMissionCommand・CmdData.cls の CallInterMissionCommand）は
    /// Lib\パイロットステータス表示.eve などを シナリオ → ExtDataPath → ExtDataPath2 → SRC のフォルダの順に探す（7474/SRC#1488）
    /// </summary>
    [TestClass]
    public class InterMissionSearchLibFileTests
    {
        private string dir;
        private string scenario;
        private string ext;
        private string ext2;
        private string app;

        [TestInitialize]
        public void Initialize()
        {
            dir = Path.Combine(Path.GetTempPath(), "srclib-" + Guid.NewGuid().ToString("N"));
            scenario = Path.Combine(dir, "scenario");
            ext = Path.Combine(dir, "ext");
            ext2 = Path.Combine(dir, "ext2");
            app = Path.Combine(dir, "app");
            foreach (var d in new[] { scenario, ext, ext2, app })
            {
                Directory.CreateDirectory(Path.Combine(d, "Lib"));
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(dir, true);
        }

        private SRC CreateSrc(bool withExtDataPath = true)
        {
            var fs = new LocalFileSystem();
            fs.AddPath(app);
            fs.AddPath(scenario);
            var src = new SRC { GUI = new MockGUI(), FileSystem = fs };
            src.SystemConfig.AppPath = app;
            src.SystemConfig.ExtDataPath = withExtDataPath ? ext : "";
            src.SystemConfig.ExtDataPath2 = withExtDataPath ? ext2 : "";
            src.ScenarioPath = scenario;
            return src;
        }

        private static void Touch(string folder, string name)
        {
            File.WriteAllText(Path.Combine(folder, "Lib", name), "");
            // 作ったファイルを次の検索で見つけられるよう、フォルダの一覧のキャッシュを捨てる
            PathResolver.Invalidate();
        }

        private static string Normalize(string path) => path == null ? null : Path.GetFullPath(path);

        [TestMethod]
        public void FindsFileInExtDataPath()
        {
            // SRC 本体を拡張データフォルダに指定している構成でも、SRC 本体の Lib の eve を使う
            Touch(ext, "パイロットステータス表示.eve");
            Touch(ext2, "ユニットステータス表示.eve");
            var src = CreateSrc();
            Assert.AreEqual(Normalize(Path.Combine(ext, "Lib", "パイロットステータス表示.eve")),
                Normalize(src.InterMission.SearchLibFile("パイロットステータス表示.eve")));
            Assert.AreEqual(Normalize(Path.Combine(ext2, "Lib", "ユニットステータス表示.eve")),
                Normalize(src.InterMission.SearchLibFile("ユニットステータス表示.eve")));
        }

        [TestMethod]
        public void SearchesInOrder_ScenarioExtExt2App()
        {
            var src = CreateSrc();
            var name = "パイロットステータス表示.eve";
            Assert.IsNull(src.InterMission.SearchLibFile(name));
            Touch(app, name);
            Assert.AreEqual(Normalize(Path.Combine(app, "Lib", name)), Normalize(src.InterMission.SearchLibFile(name)));
            Touch(ext2, name);
            Assert.AreEqual(Normalize(Path.Combine(ext2, "Lib", name)), Normalize(src.InterMission.SearchLibFile(name)));
            Touch(ext, name);
            Assert.AreEqual(Normalize(Path.Combine(ext, "Lib", name)), Normalize(src.InterMission.SearchLibFile(name)));
            Touch(scenario, name);
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Lib", name)), Normalize(src.InterMission.SearchLibFile(name)));
        }

        [TestMethod]
        public void WithoutExtDataPath_SearchesScenarioAndApp()
        {
            var name = "ユニットステータス表示.eve";
            Touch(app, name);
            var src = CreateSrc(withExtDataPath: false);
            Assert.AreEqual(Normalize(Path.Combine(app, "Lib", name)), Normalize(src.InterMission.SearchLibFile(name)));
        }
    }
}
