using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCDebugAdapter;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class DapServerTests
    {
        private int seq;

        private JsonNode Request(ProtocolClient client, string command, JsonObject arguments = null)
        {
            var requestSeq = ++seq;
            client.Send(new JsonObject { ["seq"] = requestSeq, ["type"] = "request", ["command"] = command, ["arguments"] = arguments });
            var response = client.WaitFor(x => x["type"]?.GetValue<string>() == "response" && x["request_seq"]?.GetValue<int>() == requestSeq, command);
            Assert.IsTrue(response["success"].GetValue<bool>(), response.ToJsonString());
            return response["body"];
        }

        private static JsonNode WaitEvent(ProtocolClient client, string name)
        {
            return client.WaitFor(x => x["type"]?.GetValue<string>() == "event" && x["event"]?.GetValue<string>() == name, name + " イベント", 60);
        }

        [TestMethod]
        public void Dap_BreakpointStackVariablesStepAndContinue()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve",
                    "プロローグ:",            // 1
                    "Exit",                   // 2
                    "",                       // 3
                    "スタート:",              // 4
                    "Set x 1",                // 5
                    "Call サブ",              // 6
                    "Talk システム",          // 7
                    "こんにちは",             // 8
                    "End",                    // 9
                    "Quit",                   // 10
                    "",                       // 11
                    "サブ:",                  // 12
                    "Local y = 2",            // 13
                    "Set z (y + 1)",          // 14
                    "Return");                // 15
                var server = new DapServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["adapterID"] = "srcsharp" });
                WaitEvent(client, "initialized");
                Request(client, "launch", new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root });
                var breakpoints = Request(client, "setBreakpoints", new JsonObject
                {
                    ["source"] = new JsonObject { ["path"] = eve },
                    ["breakpoints"] = new JsonArray { new JsonObject { ["line"] = 14 } },
                });
                Assert.IsTrue(breakpoints["breakpoints"][0]["verified"].GetValue<bool>());
                Request(client, "configurationDone");

                var stopped = WaitEvent(client, "stopped");
                Assert.AreEqual("breakpoint", stopped["body"]["reason"].GetValue<string>());

                var frames = Request(client, "stackTrace", new JsonObject { ["threadId"] = 1 })["stackFrames"].AsArray();
                Assert.AreEqual(14, frames[0]["line"].GetValue<int>());
                Assert.AreEqual(6, frames[1]["line"].GetValue<int>(), "Call した行");
                StringAssert.EndsWith(frames[0]["source"]["path"].GetValue<string>(), "test.eve");

                var locals = Request(client, "variables", new JsonObject { ["variablesReference"] = 1 })["variables"].AsArray();
                Assert.IsTrue(locals.Any(x => x["name"].GetValue<string>() == "y" && x["value"].GetValue<string>() == "2"), locals.ToJsonString());
                // Global で宣言せずに Set した変数は、イベントのローカル変数になる
                Assert.IsTrue(locals.Any(x => x["name"].GetValue<string>() == "x" && x["value"].GetValue<string>() == "1"), locals.ToJsonString());
                var globals = Request(client, "variables", new JsonObject { ["variablesReference"] = 2 })["variables"].AsArray();
                Assert.IsFalse(globals.Any(x => x["name"].GetValue<string>() == "x"), globals.ToJsonString());
                Assert.AreEqual("3", Request(client, "evaluate", new JsonObject { ["expression"] = "y + 1" })["result"].GetValue<string>());

                // 止まっている間も画面を撮れる（VS Code 拡張の画面のビュー）
                var screen = Request(client, DapServer.ScreenCommand);
                Assert.IsTrue(screen["paused"].GetValue<bool>());
                var png = Convert.FromBase64String(screen["png"].GetValue<string>());
                CollectionAssert.AreEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png.Take(4).ToArray(), "PNG");

                Request(client, "next", new JsonObject { ["threadId"] = 1 });
                WaitEvent(client, "stopped");
                frames = Request(client, "stackTrace", new JsonObject { ["threadId"] = 1 })["stackFrames"].AsArray();
                Assert.AreEqual(15, frames[0]["line"].GetValue<int>());

                Request(client, "continue", new JsonObject { ["threadId"] = 1 });
                // 終わる時に最後の画面を送る
                var lastScreen = WaitEvent(client, DapServer.ScreenCommand);
                Assert.IsFalse(lastScreen["body"]["running"].GetValue<bool>());
                Assert.IsNotNull(lastScreen["body"]["png"], lastScreen.ToJsonString());
                WaitEvent(client, "terminated");
                Request(client, "disconnect");
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        [TestMethod]
        public void Dap_InteractiveSendsFramesAndTakesInput()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "Input 名前 名前を入れてください 既定",
                    "Talk システム",
                    "こんにちは",
                    "End",
                    "Set 確認 1",
                    "Quit");
                var server = new DapServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["adapterID"] = "srcsharp" });
                Request(client, "launch", new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root, ["interactive"] = true });
                var breakpoints = Request(client, "setBreakpoints", new JsonObject
                {
                    ["source"] = new JsonObject { ["path"] = eve },
                    ["breakpoints"] = new JsonArray { new JsonObject { ["line"] = 10 } },
                });
                Request(client, "configurationDone");

                // 文字入力はイベントで尋ね、答えをリクエストで受ける
                var prompt = WaitEvent(client, InteractiveScreen.PromptEvent);
                Assert.AreEqual("input", prompt["body"]["kind"].GetValue<string>());
                Assert.AreEqual("既定", prompt["body"]["value"].GetValue<string>());
                Request(client, InteractiveScreen.PromptResultCommand, new JsonObject { ["id"] = prompt["body"]["id"].GetValue<int>(), ["value"] = "ゲスト" });

                // 表示したフレームを送る
                var frame = WaitEvent(client, InteractiveScreen.FrameEvent);
                Assert.AreEqual("webp", frame["body"]["format"].GetValue<string>());
                var webp = Convert.FromBase64String(frame["body"]["data"].GetValue<string>());
                CollectionAssert.AreEqual(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }, webp.Take(4).ToArray(), "WebP");

                // メッセージは送らない限り進まない（自動の入力は無い）。キー（Enter）で送る
                System.Threading.Thread.Sleep(1000);
                Request(client, InteractiveScreen.InputCommand, new JsonObject { ["type"] = "key", ["kind"] = "down", ["keyCode"] = 13 });
                Request(client, InteractiveScreen.InputCommand, new JsonObject { ["type"] = "key", ["kind"] = "up", ["keyCode"] = 13 });

                var stopped = WaitEvent(client, "stopped");
                Assert.AreEqual("breakpoint", stopped["body"]["reason"].GetValue<string>());
                Assert.AreEqual("ゲスト", Request(client, "evaluate", new JsonObject { ["expression"] = "名前" })["result"].GetValue<string>());

                Request(client, "continue", new JsonObject { ["threadId"] = 1 });
                WaitEvent(client, "terminated");
                Request(client, "disconnect");
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        [TestMethod]
        public void Dap_StopsOnEventError()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "Set x (1 / )",
                    "Call 無いラベル",
                    "Quit");
                var server = new DapServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["adapterID"] = "srcsharp" });
                Request(client, "launch", new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root });
                Request(client, "configurationDone");

                var stopped = WaitEvent(client, "stopped");
                Assert.AreEqual("exception", stopped["body"]["reason"].GetValue<string>(), stopped.ToJsonString());
                var frames = Request(client, "stackTrace", new JsonObject { ["threadId"] = 1 })["stackFrames"].AsArray();
                Assert.AreEqual(6, frames[0]["line"].GetValue<int>(), stopped.ToJsonString());

                Request(client, "disconnect");
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }
    }
}
