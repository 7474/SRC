using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using System;
using System.IO;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// PathResolver（Windows 前提のパスを実行環境で解決する）のユニットテスト
    /// </summary>
    [TestClass]
    public class PathResolverTests
    {
        private string root;

        [TestInitialize]
        public void Initialize()
        {
            root = Path.Combine(Path.GetTempPath(), "srcs-path-" + Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(root, "data", "Unit"));
            File.WriteAllText(Path.Combine(root, "data", "Unit", "Robot.txt"), "x");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(root, true);
        }

        [TestMethod]
        public void Resolve_ExistingPath_ReturnsAsIs()
        {
            var path = Path.Combine(root, "data", "Unit", "Robot.txt");
            Assert.AreEqual(path, PathResolver.Resolve(path));
        }

        [TestMethod]
        public void Resolve_BackslashAndDifferentCase_FindsFile()
        {
            var resolved = PathResolver.Resolve(root + @"\Data\unit\robot.TXT");
            Assert.IsTrue(File.Exists(resolved), resolved);
        }

        [TestMethod]
        public void Resolve_MissingFile_KeepsMissingPart()
        {
            var resolved = PathResolver.Resolve(root + @"\DATA\Unit\new.txt");
            // Windows は大文字・小文字を区別しないためそのまま返す
            Assert.AreEqual(Path.Combine(root, "data", "Unit", "new.txt"), resolved, OperatingSystem.IsWindows());
        }

        [TestMethod]
        public void LocalFileSystem_FileExists_IgnoresCaseAndSeparator()
        {
            var fs = new LocalFileSystem();
            fs.AddPath(root);
            Assert.IsTrue(fs.FileExists(root, @"Data\Unit\robot.txt"));
            Assert.IsTrue(fs.FileExists(@"DATA\UNIT\ROBOT.TXT"));
            using (var stream = fs.Open(@"data\unit\robot.txt"))
            {
                Assert.IsNotNull(stream);
            }
        }

        [TestMethod]
        public void LocalFileSystem_GetFileSystemEntries_IgnoresCase()
        {
            var fs = new LocalFileSystem();
            Assert.AreEqual(1, System.Linq.Enumerable.Count(fs.GetFileSystemEntries(Path.Combine(root, "DATA", "unit"), "ROBOT.*", EntryOption.File)));
        }

        [TestMethod]
        public void ToAbsolutePath_AbsolutePathInScenarioFolder_StaysAbsolute()
        {
            // セーブデータには絶対パスが入る。Linux・ブラウザでは / から始まる
            var fs = new LocalFileSystem();
            var eve = Path.Combine(root, "data", "02_霧の渓谷.eve");
            Assert.AreEqual(eve, fs.ToAbsolutePath(Path.Combine(root, "data"), eve));
        }

        [TestMethod]
        public void ToAbsolutePath_RelativePath_CombinesWithScenarioFolder()
        {
            var fs = new LocalFileSystem();
            Assert.AreEqual(Path.Combine(root, "data", "Save"), fs.ToAbsolutePath(Path.Combine(root, "data"), "Save"));
        }
    }
}
