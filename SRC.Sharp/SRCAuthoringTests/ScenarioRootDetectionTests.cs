using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System;
using System.IO;

namespace SRCAuthoring.Tests
{
    /// <summary>
    /// イベントファイルを実行するときのシナリオのフォルダ（ScenarioAnalyzer.FindRunRoot）。
    /// </summary>
    [TestClass]
    public class ScenarioRootDetectionTests
    {
        private string root;

        [TestInitialize]
        public void Initialize()
        {
            root = Path.Combine(Path.GetTempPath(), "srcs-root-" + Guid.NewGuid());
            Directory.CreateDirectory(root);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(root, true);
        }

        private string WriteEvent(params string[] parts)
        {
            var path = Path.Combine(root, Path.Combine(parts));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, "");
            return path;
        }

        [TestMethod]
        public void FileInScenarioFolder_UsesThatFolder()
        {
            Directory.CreateDirectory(Path.Combine(root, "シナリオ", "Data"));
            var eve = WriteEvent("シナリオ", "序章.eve");
            Assert.AreEqual(Path.Combine(root, "シナリオ"), ScenarioAnalyzer.FindRunRoot(eve));
        }

        [TestMethod]
        public void FileInSubfolder_UsesTheParentWithData()
        {
            Directory.CreateDirectory(Path.Combine(root, "シナリオ", "Data"));
            var eve = WriteEvent("シナリオ", "章", "チャプター01.eve");
            Assert.AreEqual(Path.Combine(root, "シナリオ"), ScenarioAnalyzer.FindRunRoot(eve));
        }

        [TestMethod]
        public void SubfolderWithItsOwnData_UsesThatFolder()
        {
            Directory.CreateDirectory(Path.Combine(root, "シナリオ", "Data"));
            Directory.CreateDirectory(Path.Combine(root, "シナリオ", "外伝", "Data"));
            var eve = WriteEvent("シナリオ", "外伝", "外伝.eve");
            Assert.AreEqual(Path.Combine(root, "シナリオ", "外伝"), ScenarioAnalyzer.FindRunRoot(eve));
        }

        [TestMethod]
        public void ParentIsTheSrcFolder_UsesTheFileFolder()
        {
            // Data\System のある親は SRC 本体とみなし、シナリオのフォルダにはしない
            Directory.CreateDirectory(Path.Combine(root, "Data", "System"));
            var eve = WriteEvent("シナリオ", "test.eve");
            Assert.AreEqual(Path.Combine(root, "シナリオ"), ScenarioAnalyzer.FindRunRoot(eve));
        }
    }
}
