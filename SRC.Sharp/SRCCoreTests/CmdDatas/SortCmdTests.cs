using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    [TestClass]
    public class SortCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private SortCmd CreateSortCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new SortCmd(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        [TestMethod]
        public void SortAscendingNumericTest()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 3);
            src.Expression.SetVariableAsDouble("arr[2]", 1);
            src.Expression.SetVariableAsDouble("arr[3]", 2);

            var cmd = CreateSortCmd(src, "Sort arr 昇順");
            var next = cmd.Exec();

            Assert.AreEqual(1, next);
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("arr[3]"));
        }

        [TestMethod]
        public void SortDescendingNumericTest()
        {
            // For numeric-keyed arrays, VB6 SRC's 降順 sorts keys descending first
            // then values descending within fixed key positions.
            // This produces the same mapping as ascending (smallest key = smallest value).
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 3);
            src.Expression.SetVariableAsDouble("arr[2]", 1);
            src.Expression.SetVariableAsDouble("arr[3]", 2);

            var cmd = CreateSortCmd(src, "Sort arr 降順");
            cmd.Exec();

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("arr[3]"));
        }

        [TestMethod]
        public void SortAscendingStringValueTest()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "Charlie");
            src.Expression.SetVariableAsString("arr[2]", "Alice");
            src.Expression.SetVariableAsString("arr[3]", "Bob");

            var cmd = CreateSortCmd(src, "Sort arr 昇順 文字");
            cmd.Exec();

            Assert.AreEqual("Alice", src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("Bob", src.Expression.GetValueAsString("arr[2]"));
            Assert.AreEqual("Charlie", src.Expression.GetValueAsString("arr[3]"));
        }

        [TestMethod]
        public void SortKeyOnlyTest()
        {
            // "インデックスのみ" sorts by key, keeping values with their keys
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[3]", 100);
            src.Expression.SetVariableAsDouble("arr[1]", 200);
            src.Expression.SetVariableAsDouble("arr[2]", 300);

            var cmd = CreateSortCmd(src, "Sort arr 昇順 インデックスのみ");
            cmd.Exec();

            // After key-ascending sort: keys become [1,2,3] with their original values
            Assert.AreEqual(200d, src.Expression.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(300d, src.Expression.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(100d, src.Expression.GetValueAsDouble("arr[3]"));
        }

        [TestMethod]
        public void SortEmptyArrayTest()
        {
            var src = CreateSrc();
            // No variables defined for "arr"

            var cmd = CreateSortCmd(src, "Sort arr 昇順");
            var next = cmd.Exec();

            // Should return NextID without error
            Assert.AreEqual(1, next);
        }

        [TestMethod]
        public void SortAlreadySortedTest()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 1);
            src.Expression.SetVariableAsDouble("arr[2]", 2);
            src.Expression.SetVariableAsDouble("arr[3]", 3);

            var cmd = CreateSortCmd(src, "Sort arr 昇順");
            cmd.Exec();

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("arr[3]"));
        }

        [TestMethod]
        public void SortInvalidOptionTest()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 1);

            var cmd = CreateSortCmd(src, "Sort arr 不正なオプション");
            var next = cmd.Exec();

            // EventErrorException should be caught by Exec() and return -1
            Assert.AreEqual(-1, next);
            // Variable should be unchanged after the error
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("arr[1]"));
        }
    }
}
