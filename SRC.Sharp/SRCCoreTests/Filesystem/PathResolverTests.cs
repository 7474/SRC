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
        public void Resolve_FileCreatedLater_FoundAfterInvalidateOrExpiry()
        {
            // 無いファイルを探して、フォルダの一覧をキャッシュさせる
            PathResolver.Resolve(root + @"\data\unit\Later.txt");
            File.WriteAllText(Path.Combine(root, "data", "Unit", "Later.txt"), "x");
            // 同じ名前（大文字・小文字も同じ）なら、キャッシュによらず見つかる
            Assert.IsTrue(File.Exists(PathResolver.Resolve(root + @"\data\Unit\Later.txt")));
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            PathResolver.Invalidate();
            Assert.IsTrue(File.Exists(PathResolver.Resolve(root + @"\DATA\unit\later.TXT")));

            // 一覧が古くなれば、Invalidate しなくても作り直して探す
            var lifetime = PathResolver.CacheLifetime;
            try
            {
                PathResolver.Resolve(root + @"\data\unit\Later2.txt");
                File.WriteAllText(Path.Combine(root, "data", "Unit", "Later2.txt"), "x");
                PathResolver.CacheLifetime = TimeSpan.Zero;
                System.Threading.Thread.Sleep(10);
                Assert.IsTrue(File.Exists(PathResolver.Resolve(root + @"\data\unit\LATER2.txt")));
            }
            finally
            {
                PathResolver.CacheLifetime = lifetime;
            }
        }

        [TestMethod]
        public void LocalFileSystem_OpenSafeWrite_InvalidatesCache()
        {
            var fs = new LocalFileSystem();
            fs.AddPath(root);
            fs.AddSafePath(root);
            Assert.IsFalse(fs.FileExists(@"data\unit\Saved.txt"));
            using (fs.OpenSafe(SafeOpenMode.Write, root, @"data\Unit\Saved.txt"))
            {
            }
            Assert.IsTrue(fs.FileExists(@"DATA\UNIT\SAVED.TXT"));
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
        // TrustDirectoryCache はプロセス全体の設定なので、ほかのテストと並べて走らせない
        [TestMethod]
        [DoNotParallelize]
        public void FileExists_TrustDirectoryCache_AnswersFromListing()
        {
            PathResolver.Invalidate();
            PathResolver.TrustDirectoryCache = true;
            try
            {
                Assert.IsTrue(PathResolver.FileExists(root + @"\Data\unit\robot.TXT"), "大文字・小文字と \\ の違い");
                Assert.IsFalse(PathResolver.FileExists(Path.Combine(root, "data", "Unit")), "フォルダはファイルではない");
                Assert.IsFalse(PathResolver.FileExists(Path.Combine(root, "data", "Unit", "Pilot.txt")));
                Assert.IsFalse(PathResolver.FileExists(Path.Combine(root, "data", "None", "Pilot.txt")));

                // 一覧を作った後に作ったファイルは、非Windowsでは Invalidate するまで見えない
                File.WriteAllText(Path.Combine(root, "data", "Unit", "Pilot.txt"), "x");
                Assert.AreEqual(OperatingSystem.IsWindows(), PathResolver.FileExists(Path.Combine(root, "data", "Unit", "Pilot.txt")));
                PathResolver.Invalidate();
                Assert.IsTrue(PathResolver.FileExists(Path.Combine(root, "data", "Unit", "Pilot.txt")));
            }
            finally
            {
                PathResolver.TrustDirectoryCache = false;
                PathResolver.Invalidate();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void LocalFileSystem_TrustDirectoryCache_SeesFilesItWrites()
        {
            PathResolver.Invalidate();
            PathResolver.TrustDirectoryCache = true;
            try
            {
                var fs = new LocalFileSystem();
                fs.AddPath(root);
                Assert.IsTrue(fs.FileExists(@"data\unit\robot.txt"));
                Assert.IsFalse(fs.FileExists(@"data\unit\save.srcs"));
                // LocalFileSystem で書いたファイルは、すぐに見える
                using (var stream = fs.Create(Path.Combine(root, "data", "Unit", "save.srcs")))
                {
                    stream.WriteByte(1);
                }
                Assert.IsTrue(fs.FileExists(@"data\unit\save.srcs"));
            }
            finally
            {
                PathResolver.TrustDirectoryCache = false;
                PathResolver.Invalidate();
            }
        }
    }
}
