using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Commands
{
    /// <summary>
    /// CmdData.cls の ExecSepiaCmd・ExecSortCmd を、元の SRC の動作を期待値にして確かめる（7474/SRC#1505）。
    /// </summary>
    [TestClass]
    public class CmdDataSepiaSortVBMatchTests
    {
        private static CmdData CreateCmd(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        [TestMethod]
        public void Sepia_RestoresMapPositionAfterSetupBackground()
        {
            // 元の SRC: prev_x = MapX: prev_y = MapY → SetupBackground "セピア" → Center prev_x, prev_y → RedrawScreen
            var f = new MapCoverageFixture();
            f.Src.Event.EventData = new List<EventDataLine>();
            f.Src.Event.EventCmd = new List<CmdData>();
            f.Src.Event.EventQue = new List<string>();
            f.GUI.ChangeStatusHandler = _ => { };
            f.LoadGrid(
                ".....",
                ".....",
                ".....");
            f.GUI.MapX = 4;
            f.GUI.MapY = 2;
            var log = new List<string>();
            f.GUI.SetupBackgroundHandler = (mode, opt, color, trans) =>
            {
                log.Add("SetupBackground " + mode);
                // SetupBackground はマップの表示位置を中央にする
                f.GUI.MapX = f.GUI.MainWidth / 2 + 1;
                f.GUI.MapY = f.GUI.MainHeight / 2 + 1;
            };
            f.GUI.CenterHandler = (x, y) =>
            {
                log.Add("Center " + x + "," + y);
                f.GUI.MapX = x;
                f.GUI.MapY = y;
            };
            f.GUI.RedrawScreenHandler = late => log.Add("RedrawScreen " + late);

            var ret = CreateCmd(f.Src, "Sepia 非同期").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SetupBackground セピア", "Center 4,2", "RedrawScreen True" }, log);
            Assert.AreEqual(4, f.GUI.MapX);
            Assert.AreEqual(2, f.GUI.MapY);
        }

        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        [TestMethod]
        public void Sort_StringKeys_TiesAreReorderedLikeVBExchangeSort()
        {
            // 元の SRC: For i = 0 To num - 1: For j = num To i + 1 Step -1 で a(i) > a(j) なら入れ替える（安定ではない）。
            // [x]=1, [y]=1, [z]=0 → i=0, j=2 で x と z を入れ替え → z, y, x の順に定義し直す
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[x]", 1);
            src.Expression.SetVariableAsDouble("arr[y]", 1);
            src.Expression.SetVariableAsDouble("arr[z]", 0);

            Assert.AreEqual(1, CreateCmd(src, "Sort arr").Exec());

            var names = src.Event.LocalVariableList.Values.Select(v => v.Name).Where(n => n.StartsWith("arr[")).ToList();
            CollectionAssert.AreEqual(new[] { "arr[z]", "arr[y]", "arr[x]" }, names);
        }

        [TestMethod]
        public void Sort_StringValues_CompareLikeVbTextCompare()
        {
            // 元の SRC: StrComp(…, vbTextCompare)。全角・半角、大文字・小文字、ひらがな・カタカナを区別しないので、
            // 同じとみなす値は入れ替えない
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "ア");
            src.Expression.SetVariableAsString("arr[2]", "あ");
            src.Expression.SetVariableAsString("arr[3]", "b");
            src.Expression.SetVariableAsString("brr[1]", "Ａ");
            src.Expression.SetVariableAsString("brr[2]", "a");

            Assert.AreEqual(1, CreateCmd(src, "Sort arr 文字").Exec());
            Assert.AreEqual(2, CreateCmd(src, "Sort brr 文字").Exec());

            // i=0: 「ア」と「b」を入れ替える → b, あ, ア。「あ」と「ア」は同じなので入れ替えない
            Assert.AreEqual("b", src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("あ", src.Expression.GetValueAsString("arr[2]"));
            Assert.AreEqual("ア", src.Expression.GetValueAsString("arr[3]"));
            // 「Ａ」と「a」は同じなので入れ替えない
            Assert.AreEqual("Ａ", src.Expression.GetValueAsString("brr[1]"));
            Assert.AreEqual("a", src.Expression.GetValueAsString("brr[2]"));
        }

        [TestMethod]
        public void Sort_FullWidthNumericStrings_AreComparedAsNumbers()
        {
            // 元の SRC: IsNumeric・CDbl は全角の数字を数値として読む
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "１０");
            src.Expression.SetVariableAsString("arr[2]", "9");

            Assert.AreEqual(1, CreateCmd(src, "Sort arr").Exec());

            Assert.AreEqual("9", src.Expression.GetValueAsString("arr[1]"));
            Assert.AreEqual("１０", src.Expression.GetValueAsString("arr[2]"));
        }

        [TestMethod]
        public void Sort_ArrayNamePrefix_IsCaseSensitive()
        {
            // 元の SRC: InStr(.Name, vname & "[") = 1（Binary の比較）。名前の大文字・小文字が違う要素は並べ替えない
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("arr[1]", 3);
            src.Expression.SetVariableAsDouble("arr[2]", 2);
            src.Expression.SetVariableAsDouble("Arr[3]", 1);

            Assert.AreEqual(1, CreateCmd(src, "Sort arr").Exec());

            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("Arr[3]"));
        }
    }
}
