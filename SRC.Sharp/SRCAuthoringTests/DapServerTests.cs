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
