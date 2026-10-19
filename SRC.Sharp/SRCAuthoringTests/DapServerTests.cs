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
        [DoNotParallelize]
        public void Dap_AutoPlayWithoutScriptSendsFramesAndSkipsWait()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "Talk システム",
                    "こんにちは",
                    "End",
                    "Ask どうする",
                    "進む",
                    "戻る",
                    "End",
                    "Set 答え 選択",
                    "Wait 600",
                    "Set 確認 1",
                    "Quit");
                var server = new DapServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["adapterID"] = "srcsharp" });
                // script も interactive も無ければ自動で進める。skipWait で Wait 600（60 秒）を待たない
                Request(client, "launch", new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root, ["skipWait"] = true });
                Request(client, "setBreakpoints", new JsonObject
                {
                    ["source"] = new JsonObject { ["path"] = eve },
                    ["breakpoints"] = new JsonArray { new JsonObject { ["line"] = 14 } },
                });
                Request(client, "configurationDone");

                // 自動で進めるときも、表示したフレームを送る
                var frame = WaitEvent(client, InteractiveScreen.FrameEvent);
                Assert.AreEqual("webp", frame["body"]["format"].GetValue<string>());

                var stopped = client.WaitFor(x => x["type"]?.GetValue<string>() == "event" && x["event"]?.GetValue<string>() == "stopped", "stopped イベント", 30);
                Assert.AreEqual("breakpoint", stopped["body"]["reason"].GetValue<string>());
                Assert.AreEqual("1", Request(client, "evaluate", new JsonObject { ["expression"] = "答え" })["result"].GetValue<string>(), "選択肢は先頭");

                Request(client, "continue", new JsonObject { ["threadId"] = 1 });
                WaitEvent(client, "terminated");
                Request(client, "disconnect");
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        private static void CopyDirectory(string from, string to)
        {
            foreach (var dir in System.IO.Directory.GetDirectories(from, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.Directory.CreateDirectory(dir.Replace(from, to));
            }
            foreach (var file in System.IO.Directory.GetFiles(from, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Copy(file, file.Replace(from, to), true);
            }
        }

        private static JsonNode WaitReload(ProtocolClient client, string status)
        {
            return client.WaitFor(x => x["type"]?.GetValue<string>() == "event"
                && x["event"]?.GetValue<string>() == HotReloader.ReloadEvent
                && x["body"]?["status"]?.GetValue<string>() == status, "読み直し: " + status, 60);
        }

        [TestMethod]
        public void Dap_HotReloadKeepsStateAndReloadsEventsAndData()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                // 味方フェイズのマップまで進むシナリオ（E2E の battle-basic）
                var dir = new System.IO.DirectoryInfo(AppContext.BaseDirectory);
                while (!System.IO.File.Exists(System.IO.Path.Combine(dir.FullName, "SRC.Sharp.sln")))
                {
                    dir = dir.Parent;
                }
                CopyDirectory(System.IO.Path.Combine(dir.FullName, "E2E", "battle-basic"), scenario.Root);
                var eve = System.IO.Path.Combine(scenario.Root, "test.eve");
                var robot = System.IO.Path.Combine(scenario.Root, "Data", "テスト", "robot.txt");
                var server = new DapServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["adapterID"] = "srcsharp" });
                Request(client, "launch", new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root, ["interactive"] = true });
                Request(client, "configurationDone");

                // 「戦闘開始」のメッセージを送る前に頼むと、マップでコマンドを待つまで待つ
                System.Threading.Thread.Sleep(1500);
                var original = System.IO.File.ReadAllText(eve);
                var lines = original.TrimEnd('\n').Split('\n').ToList();
                lines.AddRange(new[] { "", "再開:", "Set リロード確認 1", "Exit" });
                System.IO.File.WriteAllText(eve, string.Join("\n", lines) + "\n");
                System.IO.File.WriteAllText(robot, System.IO.File.ReadAllText(robot).Replace("1000, 100, 600, 60", "1500, 100, 600, 60"));
                Request(client, "setBreakpoints", new JsonObject
                {
                    ["source"] = new JsonObject { ["path"] = eve },
                    ["breakpoints"] = new JsonArray { new JsonObject { ["line"] = lines.Count - 1 } },
                });
                Request(client, HotReloader.ReloadCommand, new JsonObject { ["events"] = true, ["data"] = true });
                WaitReload(client, "waiting");

                // 読み直したイベントの「再開」が動く。データも読み直している
                // メッセージが出る前に押したキーは捨てられるため、止まるまで押し直す（遅い環境では 1.5 秒で出ていないことがある）
                JsonNode stopped = null;
                for (var i = 0; stopped == null && i < 30; i++)
                {
                    Request(client, InteractiveScreen.InputCommand, new JsonObject { ["type"] = "key", ["kind"] = "down", ["keyCode"] = 13 });
                    Request(client, InteractiveScreen.InputCommand, new JsonObject { ["type"] = "key", ["kind"] = "up", ["keyCode"] = 13 });
                    try
                    {
                        stopped = client.WaitFor(x => x["type"]?.GetValue<string>() == "event" && x["event"]?.GetValue<string>() == "stopped", "stopped イベント", 2);
                    }
                    catch (TimeoutException)
                    {
                    }
                }
                Assert.IsNotNull(stopped, "stopped イベントが来ません");
                Assert.AreEqual("breakpoint", stopped["body"]["reason"].GetValue<string>());
                Assert.AreEqual("1500", Request(client, "evaluate", new JsonObject { ["expression"] = "Info(ユニットデータ, テスト機, 最大ＨＰ)" })["result"].GetValue<string>());
                // マップにいるユニットは残り、新しいデータで動く
                Assert.AreEqual("1500", Request(client, "evaluate", new JsonObject { ["expression"] = "Info(ユニット, テスト機, 最大ＨＰ)" })["result"].GetValue<string>());
                Request(client, "continue", new JsonObject { ["threadId"] = 1 });
                var reloaded = WaitReload(client, "reloaded");
                StringAssert.Contains(reloaded["body"]["message"].GetValue<string>(), "イベント");

                // エラーのあるイベントは読み直さない
                System.IO.File.WriteAllText(eve, original + "\nIf x = 1 Then\n");
                Request(client, HotReloader.ReloadCommand, new JsonObject { ["events"] = true });
                var failed = WaitReload(client, "failed");
                Assert.IsTrue(failed["body"]["diagnostics"].AsArray().Count > 0, failed.ToJsonString());

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
