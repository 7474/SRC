using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ArrayCmd の機能テスト（分割・リスト展開）
    /// </summary>
    [TestClass]
    public class ArrayCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // Array コマンド - リスト形式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_ListType_CreatesIndexedElements()
        {
            // ヘルプ: Array 配列名 リスト リスト → リストを展開して配列を作成
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array arr \"a b c\" リスト");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("a", src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("b", src.Expression.GetValueAsString("arr[2]"));
            Assert.AreEqual("c", src.Expression.GetValueAsString("arr[3]"));
        }

        [TestMethod]
        public void ArrayCmd_ListType_SingleElement()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array arr \"単独要素\" リスト");
            cmd.Exec();
            Assert.AreEqual("単独要素", src.Expression.GetValueAsString("arr[1]"));
        }

        [TestMethod]
        public void ArrayCmd_ListType_NumericValues()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array nums \"1 2 3\" リスト");
            cmd.Exec();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("nums[1]"));
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("nums[2]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("nums[3]"));
        }

        // ──────────────────────────────────────────────
        // Array コマンド - 区切り文字形式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_Delimiter_SplitsOnComma()
        {
            // ヘルプ: Array 配列名 文字列 区切り文字 → 文字列を区切り文字で分割して配列に代入
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array arr \"a,b,c\" \",\"");
            cmd.Exec();
            Assert.AreEqual("a", src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("b", src.Expression.GetValueAsString("arr[2]"));
            Assert.AreEqual("c", src.Expression.GetValueAsString("arr[3]"));
        }

        [TestMethod]
        public void ArrayCmd_Delimiter_SplitsOnSlash()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array path \"dir/sub/file\" \"/\"");
            cmd.Exec();
            Assert.AreEqual("dir", src.Expression.GetValueAsString("path[1]"));
            Assert.AreEqual("sub", src.Expression.GetValueAsString("path[2]"));
            Assert.AreEqual("file", src.Expression.GetValueAsString("path[3]"));
        }

        [TestMethod]
        public void ArrayCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array arr val");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // Array コマンド - 既存変数への上書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_OverwritesExistingGlobalVariable()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("myArr");
            src.Expression.SetVariableAsString("myArr", "old");

            var cmd = CreateCmd(src, "Array myArr \"x y\" リスト");
            cmd.Exec();
            Assert.AreEqual("x", src.Expression.GetValueAsString("myArr[1]"));
            Assert.AreEqual("y", src.Expression.GetValueAsString("myArr[2]"));
        }

        // ──────────────────────────────────────────────
        // SortCmd - ソート機能
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SortCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sort");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SortCmd_AscendingSort_SortsArray()
        {
            // ヘルプ: Sort 配列名 → 配列要素を昇順にソート
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("arr");
            src.Expression.SetVariableAsString("arr[1]", "c");
            src.Expression.SetVariableAsString("arr[2]", "a");
            src.Expression.SetVariableAsString("arr[3]", "b");

            var cmd = CreateCmd(src, "Sort arr");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            // After ascending sort, arr[1] should be "a"
            Assert.AreEqual("a", src.Expression.GetValueAsString("arr[1]"));
        }

        [TestMethod]
        public void SortCmd_NumericAscendingSort_SortsNumerically()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("nums");
            src.Expression.SetVariableAsDouble("nums[1]", 30d);
            src.Expression.SetVariableAsDouble("nums[2]", 10d);
            src.Expression.SetVariableAsDouble("nums[3]", 20d);

            var cmd = CreateCmd(src, "Sort nums");
            cmd.Exec();
            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("nums[1]"));
        }
    }
}
