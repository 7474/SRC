using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCSharpWeb.Tests
{
    /// <summary>ゲームが書いたセーブデータを受け取り、ブラウザに保存したものを戻す判断（SaveSync）</summary>
    [TestClass]
    public class SaveSyncTests
    {
        private string root;

        [TestInitialize]
        public void Setup()
        {
            root = Path.Combine(Path.GetTempPath(), "srcweb-save-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(root, true);
        }

        private string Write(string name, string text)
        {
            var path = Path.Combine(root, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text);
            return path;
        }

        [TestMethod]
        [DataRow("a.srcs", true)]
        [DataRow("dir/b.srcq", true)]
        [DataRow("C.SRCS", true)]
        [DataRow("test.eve", false)]
        [DataRow("a.srcs.bak", false)]
        public void IsSaveFile(string name, bool expected)
        {
            Assert.AreEqual(expected, SaveSync.IsSaveFile(name));
        }

        [TestMethod]
        public void Notify_OnlySaveFilesInRoot()
        {
            var sync = new SaveSync(root);
            Assert.IsFalse(sync.Notify(null));
            Assert.IsFalse(sync.Notify(""));
            Assert.IsFalse(sync.Notify(Path.Combine(root, "test.eve")));
            // 名前が同じ文字で始まる別のフォルダはシナリオのフォルダの外
            Assert.IsFalse(sync.Notify(root + "x/a.srcs"));
            Assert.IsFalse(sync.Notify(Path.Combine(root, "..", "a.srcs")));
            Assert.IsFalse(sync.HasWritten);

            Assert.IsTrue(sync.Notify(Path.Combine(root, "sub", "..", "a.srcs")));
            Assert.IsTrue(sync.HasWritten);
        }

        [TestMethod]
        public void CollectWritten_ReturnsRelativeNamesOnce()
        {
            var sync = new SaveSync(root);
            var a = Write("a.srcs", "A");
            var b = Write(Path.Combine("sub", "b.srcq"), "B");
            sync.Notify(a);
            sync.Notify(a);
            sync.Notify(b);

            var written = sync.CollectWritten().OrderBy(x => x.Name).ToList();
            CollectionAssert.AreEqual(new[] { "a.srcs", "sub/b.srcq" }, written.Select(x => x.Name).ToArray());
            Assert.AreEqual("A", Encoding.UTF8.GetString(written[0].Data));
            Assert.AreEqual("B", Encoding.UTF8.GetString(written[1].Data));

            // 一度返したものは返さない
            Assert.IsFalse(sync.HasWritten);
            Assert.AreEqual(0, sync.CollectWritten().Count);
        }

        [TestMethod]
        public void CollectWritten_SkipsDeletedFiles()
        {
            var sync = new SaveSync(root);
            var a = Write("a.srcs", "A");
            sync.Notify(a);
            File.Delete(a);
            Assert.AreEqual(0, sync.CollectWritten().Count);
            Assert.IsFalse(sync.HasWritten);
        }

        [TestMethod]
        public void CollectWritten_RetriesFilesBeingWritten()
        {
            var sync = new SaveSync(root);
            var a = Write("a.srcs", "A");
            sync.Notify(a);
            // 書き込み中（ほかから読めない）なら、次の機会に読む
            using (new FileStream(a, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var locked = sync.CollectWritten();
                if (locked.Count > 0)
                {
                    Assert.Inconclusive("この環境ではファイルを読めないようにできない");
                }
                Assert.IsTrue(sync.HasWritten);
            }
            Assert.AreEqual("a.srcs", sync.CollectWritten().Single().Name);
        }

        [TestMethod]
        public void Restore_WritesInsideRoot()
        {
            var sync = new SaveSync(root);
            var path = sync.Restore("sub/a.srcs", Encoding.UTF8.GetBytes("A"));
            Assert.AreEqual(Path.Combine(root, "sub", "a.srcs"), path);
            Assert.AreEqual("A", File.ReadAllText(path));
            // 戻したことは書き込みとして返さない（ブラウザへ保存し直さない）
            Assert.IsFalse(sync.HasWritten);
        }

        [TestMethod]
        [DataRow("../a.srcs")]
        [DataRow("sub/../../a.srcs")]
        [DataRow("test.eve")]
        public void Restore_RejectsOutsideOrNonSave(string name)
        {
            var sync = new SaveSync(root);
            Assert.IsNull(sync.Restore(name, new byte[] { 1 }));
            Assert.IsFalse(File.Exists(Path.Combine(root, "..", "a.srcs")));
            Assert.IsFalse(File.Exists(Path.Combine(root, "test.eve")));
        }
    }
}
