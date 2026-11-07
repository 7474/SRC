using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// Event.bas の HandleEvent・ClearEventData・サブルーチンローカル変数の参照を、
    /// 元の SRC（VB）の動作を期待値にして確かめる（#1498）。
    /// </summary>
    [TestClass]
    public class EventBasHandleVBMatchTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-evbas-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
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

        private SRC CreateSrc(params string[] scenario)
        {
            File.WriteAllText(Path.Combine(appPath, "test.eve"), string.Join("\r\n", scenario), new UTF8Encoding(false));
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;
            new HeadlessGUI().Install(src);
            src.ScenarioPath = appPath;
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            src.Expression.DefineGlobalVariable("記録");
            src.Expression.SetVariableAsString("記録", "");
            return src;
        }

        // VB: イベントデータの末尾を越えて実行したら（CurrentLineNum > UBound(EventCmd)）、
        // GoTo ExitLoop でキューに残ったイベントも実行せずに終わる
        [TestMethod]
        public void HandleEvent_RunningPastEndOfData_StopsRemainingQueue()
        {
            var src = CreateSrc(
                "ターン 1 味方:",
                "Set 記録 (記録 & \"B\")",
                "Exit",
                "ターン 全 味方:",
                "Set 記録 (記録 & \"A\")");

            src.Event.HandleEvent("ターン", "1", "味方");

            Assert.AreEqual("A", src.Expression.GetValueAsString("記録"));
            Assert.AreEqual(0, src.Event.EventQue.Count);
        }

        // Exit で終われば、キューの次のイベントへ進む
        [TestMethod]
        public void HandleEvent_ExitMovesToNextQueuedEvent()
        {
            var src = CreateSrc(
                "ターン 1 味方:",
                "Set 記録 (記録 & \"B\")",
                "Exit",
                "ターン 全 味方:",
                "Set 記録 (記録 & \"A\")",
                "Exit");

            src.Event.HandleEvent("ターン", "1", "味方");

            Assert.AreEqual("AB", src.Expression.GetValueAsString("記録"));
        }

        // VB: "ターン 全 " & Args(2)、"ターン " & Format$(Args(1)) & " " & Args(2)
        [TestMethod]
        public void HandleEvent_TurnEventQueueStrings()
        {
            var src = CreateSrc(
                "ターン 全 味方:",
                "Set 記録 (記録 & \"A\")",
                "Exit");
            var queues = new List<string>();
            src.Event.Debugger = new Hook(cmd => queues.Add(string.Join("|", src.Event.EventQue)));

            src.Event.HandleEvent("ターン", "1", "味方");

            Assert.AreEqual("ターン 全 味方|ターン 1 味方", queues.First());
        }

        // VB: CurrentLabel の初期値は 0（ラベルが無い）。HandleEvent を通らずに引数なしの ClearEvent を実行しても
        // ラベルを消さない（If CurrentLabel > 0 Then ClearLabel CurrentLabel）。C# では先頭の行が 0 なので -1
        [TestMethod]
        public void ClearEvent_WithoutHandleEvent_DoesNotClearFirstLabel()
        {
            var src = CreateSrc(
                "ターン 1 味方:",
                "Exit",
                "ClearEvent");
            var line = src.Event.EventData.First(x => x.Data == "ClearEvent");

            src.Event.EventCmd[line.ID].Exec();

            Assert.IsTrue(src.Event.colEventLabelList.List.All(x => x.Enable));
            Assert.AreEqual(-1, new SRC().Event.CurrentLabel);
        }

        // VB: ClearEventData は ReDim HotPointList(0)（要素の無い一覧）にする。後の HotPoint で追加できる
        [TestMethod]
        public void ClearEventData_HotPointListIsEmptyAndExtensible()
        {
            var src = CreateSrc("Exit");
            src.Event.HotPointList.Add(new HotPoint { Name = "前" });

            src.Event.ClearEventData();

            Assert.AreEqual(0, src.Event.HotPointList.Count);
            src.Event.HotPointList.Add(new HotPoint { Name = "後" });
            Assert.AreEqual(1, src.Event.HotPointList.Count);
        }

        // VB: サブルーチンローカル変数は CallDepth > 0 のときだけ見る（For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex）
        [TestMethod]
        public void SubLocalVars_CallDepthZero_IsEmpty()
        {
            var src = new SRC();
            src.Event.CallDepth = 0;
            src.Event.VarIndex = 1;
            src.Event.VarStack[0].SetValue("a", Expressions.ValueType.StringType, "x", 0d);
            src.Event.VarStack[1].SetValue("b", Expressions.ValueType.StringType, "y", 0d);

            Assert.AreEqual(0, src.Event.SubLocalVars().Count());
            Assert.IsNull(src.Event.SubLocalVar("a"));
        }

        [TestMethod]
        public void SubLocalVars_InSubroutine_ReturnsCurrentFrame()
        {
            var src = new SRC();
            src.Event.CallDepth = 2;
            src.Event.VarIndexStack[1] = 0;
            src.Event.VarIndex = 2;
            src.Event.VarStack[0].SetValue("a", Expressions.ValueType.StringType, "x", 0d);
            src.Event.VarStack[1].SetValue("b", Expressions.ValueType.StringType, "y", 0d);
            src.Event.VarStack[2].SetValue("c", Expressions.ValueType.StringType, "z", 0d);

            CollectionAssert.AreEqual(new[] { "b", "c" }, src.Event.SubLocalVars().Select(x => x.Name).ToArray());
        }

        private class Hook : IEventDebugger
        {
            private readonly Action<CmdDatas.CmdData> before;

            public Hook(Action<CmdDatas.CmdData> before)
            {
                this.before = before;
            }

            public void BeforeExecute(CmdDatas.CmdData command)
            {
                before(command);
            }

            public void OnError(EventDataLine line, string message)
            {
            }
        }
    }
}
