using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Dir 関数はパスを除いたファイル名・フォルダ名を返す（ヘルプ「ファイル処理関数」、VB: Expression.bas の Dir$）
    /// </summary>
    [TestClass]
    public class DirFunctionTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-dir-" + Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(appPath, "セーブデータ"));
            File.WriteAllText(Path.Combine(appPath, "保存.src"), "");
            File.WriteAllText(Path.Combine(appPath, "セーブデータ", "a.txt"), "");
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
            return src;
        }

        [TestMethod]
        public void Dir_ReturnsFileNameWithoutPath()
        {
            var src = CreateSrc();
            Assert.AreEqual("保存.src", src.Expression.GetValueAsString("Dir(\"保存.src\")"));
            Assert.AreEqual("a.txt", src.Expression.GetValueAsString("Dir(\"セーブデータ\\a.txt\")"));
        }

        [TestMethod]
        public void Dir_WithWildcard_ReturnsFileName()
        {
            var src = CreateSrc();
            Assert.AreEqual("保存.src", src.Expression.GetValueAsString("Dir(\"*.src\")"));
            Assert.AreEqual("", src.Expression.GetValueAsString("Dir(\"*.srcs\")"));
        }

        [TestMethod]
        public void Dir_Folder_ReturnsFolderName()
        {
            var src = CreateSrc();
            Assert.AreEqual("セーブデータ", src.Expression.GetValueAsString("Dir(\"セーブデータ\", フォルダ)"));
            Assert.AreEqual("", src.Expression.GetValueAsString("Dir(\"保存.src\", フォルダ)"));
        }
    }
}
