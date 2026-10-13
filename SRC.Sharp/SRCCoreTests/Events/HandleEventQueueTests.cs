using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HandleEvent はイベントキューのうち自分が追加した分だけを処理する（元の SRC の HandleEvent）
    /// </summary>
    [TestClass]
    public class HandleEventQueueTests
    {
        private static readonly string[] Scenario =
        {
            "再開:",
            "Set 記録 (記録 & \"O\")",
            "Exit",
            "",
            "エピローグ:",
            "Set 記録 (記録 & \"I\")",
            "Exit",
            "",
        };

        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-que-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
            File.WriteAllText(Path.Combine(appPath, "test.eve"), string.Join("\r\n", Scenario), new UTF8Encoding(false));
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
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            src.Expression.DefineGlobalVariable("記録");
            src.Expression.SetVariableAsString("記録", "");
            return src;
        }

        [TestMethod]
        public void HandleEvent_DoesNotRunEventsQueuedBeforeIt()
        {
            var src = CreateSrc();
            // 呼び出し元で処理待ちのイベント（再開）
            src.Event.EventQue.Add("再開");

            src.Event.HandleEvent("エピローグ");

            // 入れ子の HandleEvent は呼び出し元の分を実行しない
            Assert.AreEqual("I", src.Expression.GetValueAsString("記録"));
            CollectionAssert.AreEqual(new List<string> { "再開" }, src.Event.EventQue);
        }

        [TestMethod]
        public void HandleEvent_RunsOwnEventAndRestoresQueue()
        {
            var src = CreateSrc();

            src.Event.HandleEvent("再開");
            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("OI", src.Expression.GetValueAsString("記録"));
            Assert.AreEqual(0, src.Event.EventQue.Count);
        }

        [TestMethod]
        public void HandleEvent_RunsEventsRegisteredWhileRunning()
        {
            var src = CreateSrc();
            // エピローグの実行中に（Destroy の全滅のように）イベントを登録する
            src.Event.Debugger = new Hook(cmd =>
            {
                if (cmd.EventData.Data.StartsWith("Set 記録 (記録 & \"I\")"))
                {
                    src.Event.RegisterEvent("再開");
                }
            });

            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("IO", src.Expression.GetValueAsString("記録"));
            Assert.AreEqual(0, src.Event.EventQue.Count);
        }

        [TestMethod]
        public void HandleEvent_DropsRestWhenScenarioFinished()
        {
            var src = CreateSrc();
            // エピローグの実行中にステージが終了したら（Continue など）、後ろに積まれたイベントは実行せずに捨てる
            src.Event.Debugger = new Hook(cmd =>
            {
                if (cmd.EventData.Data.StartsWith("Set 記録 (記録 & \"I\")"))
                {
                    src.Event.RegisterEvent("再開");
                    src.IsScenarioFinished = true;
                }
            });

            src.Event.HandleEvent("エピローグ");

            Assert.AreEqual("I", src.Expression.GetValueAsString("記録"));
            Assert.AreEqual(0, src.Event.EventQue.Count);
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
