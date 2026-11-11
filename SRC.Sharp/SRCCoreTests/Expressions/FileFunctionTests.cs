using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions.Functions;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class FileFunctionTests
    {
        private static SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        [TestMethod]
        public void Loadfiledialog_StringType_DefaultResultIsEmptyString()
        {
            var src = CreateSrc();
            var sut = new Loadfiledialog();

            var resultType = sut.Invoke(src, ValueType.StringType, new string[1], 0, new bool[1], out var strResult, out var numResult);

            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("", strResult);
            Assert.AreEqual(0d, numResult);
        }

        [TestMethod]
        public void LoadFileDialog_PassesDefaultFileToDialog()
        {
            // 元の SRC（Expression.bas）は第 3 引数をデフォルトファイルとしてダイアログに渡す（7474/SRC#1479）
            var gui = new MockGUI();
            var calls = new System.Collections.Generic.List<string[]>();
            gui.SelectLoadFileHandler = (title, dir, file, type, ext) =>
            {
                calls.Add(new[] { title, file, type, ext });
                return "";
            };
            var src = new SRC { GUI = gui };

            Assert.AreEqual("", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\", \"a.txt\")"));
            Assert.AreEqual("", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\")"));

            Assert.AreEqual(2, calls.Count);
            CollectionAssert.AreEqual(new[] { "ファイルを開く", "a.txt", "テキスト", "txt" }, calls[0]);
            Assert.AreEqual("", calls[1][1]);
        }

        [TestMethod]
        public void FileDialogs_ReturnPathRelativeToScenarioFolder()
        {
            // ScenarioPath は末尾に区切りが無い（Path.GetDirectoryName）。選んだファイルはシナリオのフォルダからの相対パスで返す
            // （VB: Mid$(str_result, Len(ScenarioPath) + 1)。VB の ScenarioPath は末尾が \）。シナリオのフォルダの外なら空
            var scenario = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "srcdlg-" + System.Guid.NewGuid().ToString("N"));
            var selected = "";
            var gui = new MockGUI { SelectLoadFileHandler = (title, dir, file, type, ext) => selected };
            var src = new SRC { GUI = gui, FileSystem = new SRCCore.Filesystem.LocalFileSystem(), ScenarioPath = scenario };

            selected = System.IO.Path.Combine(scenario, "b.txt");
            Assert.AreEqual("b.txt", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\")"));
            selected = System.IO.Path.Combine(scenario, "sub", "c.txt");
            Assert.AreEqual(@"sub\c.txt", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\")"));
            selected = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "other", "d.txt");
            Assert.AreEqual("", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\")"));
        }

        [TestMethod]
        public void Savefiledialog_StringType_DefaultResultIsEmptyString()
        {
            var src = CreateSrc();
            var sut = new Savefiledialog();

            var resultType = sut.Invoke(src, ValueType.StringType, new string[1], 0, new bool[1], out var strResult, out var numResult);

            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("", strResult);
            Assert.AreEqual(0d, numResult);
        }
    }
}
