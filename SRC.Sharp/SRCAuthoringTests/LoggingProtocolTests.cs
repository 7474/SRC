using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Lsp;
using SRCCore.Logging;
using SRCDebugAdapter;
using SRCLogging;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    /// <summary>DAP・LSP がログをプロトコル（output イベント・window/logMessage）で送る</summary>
    [TestClass]
    public class LoggingProtocolTests
    {
        private string logDir;

        [TestInitialize]
        public void Init()
        {
            logDir = Path.Combine(Path.GetTempPath(), "srclog-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Directory.Delete(logDir, true); } catch { }
        }

        private LogHost CreateLogHost(string name)
        {
            return new LogHost(name, new LogSettings { Directory = logDir }, LogSettings.Empty, LogOutputs.None);
        }

        [TestMethod]
        public void Dap_SendsLogAsOutputEvent_WithLaunchLogLevel()
        {
            using (var logHost = CreateLogHost("test-dap"))
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve",
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "Debug ログの確認",
                    "Quit");
                var server = new DapServer(client.ServerInput, client.ServerOutput, logHost);
                var run = Task.Run(() => server.RunAsync());

                client.Send(new JsonObject { ["seq"] = 1, ["type"] = "request", ["command"] = "initialize", ["arguments"] = new JsonObject { ["adapterID"] = "srcsharp" } });
                client.WaitFor(x => x["event"]?.GetValue<string>() == "initialized", "initialized");
                client.Send(new JsonObject { ["seq"] = 2, ["type"] = "request", ["command"] = "launch", ["arguments"] = new JsonObject { ["program"] = eve, ["appPath"] = scenario.Root, ["logLevel"] = "Debug" } });
                client.Send(new JsonObject { ["seq"] = 3, ["type"] = "request", ["command"] = "configurationDone" });

                var info = client.WaitFor(x => x["event"]?.GetValue<string>() == "output"
                    && (x["body"]?["output"]?.GetValue<string>() ?? "").Contains("[INF] SRCCore.Event: ログの確認"), "Debug コマンドのログ", 60);
                Assert.AreEqual("console", info["body"]["category"].GetValue<string>());
                client.WaitFor(x => x["event"]?.GetValue<string>() == "output"
                    && (x["body"]?["output"]?.GetValue<string>() ?? "").StartsWith("[DBG] "), "Debug のログ", 60);
                Assert.AreEqual(LogLevel.Debug, logHost.Options.Level);

                client.WaitFor(x => x["event"]?.GetValue<string>() == "terminated", "terminated", 60);
                client.Send(new JsonObject { ["seq"] = 4, ["type"] = "request", ["command"] = "disconnect" });
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        [TestMethod]
        public void Lsp_SendsLogAsLogMessage_WithInitializationLogLevel()
        {
            using (var logHost = CreateLogHost("test-lsp"))
            using (var client = new ProtocolClient())
            {
                var server = new LspServer(client.ServerInput, client.ServerOutput, logHost.LoggerFactory, logHost.OverrideLevel);
                var run = Task.Run(() => server.RunAsync());

                client.Send(new JsonObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1,
                    ["method"] = "initialize",
                    ["params"] = new JsonObject
                    {
                        ["capabilities"] = new JsonObject(),
                        ["initializationOptions"] = new JsonObject { ["logLevel"] = "Warning" },
                    },
                });
                client.WaitFor(x => x["id"]?.GetValue<int>() == 1, "initialize");
                Assert.AreEqual(LogLevel.Warning, logHost.Options.Level);

                logHost.LoggerFactory.CreateLogger("SRCSharp.Lsp").LogWarning("警告の確認");
                var message = client.WaitFor(x => x["method"]?.GetValue<string>() == "window/logMessage", "window/logMessage");
                Assert.AreEqual(2, message["params"]["type"].GetValue<int>());
                Assert.AreEqual("[WRN] SRCSharp.Lsp: 警告の確認", message["params"]["message"].GetValue<string>());

                client.Send(new JsonObject { ["jsonrpc"] = "2.0", ["id"] = 2, ["method"] = "shutdown" });
                client.WaitFor(x => x["id"]?.GetValue<int>() == 2, "shutdown");
                client.Send(new JsonObject { ["jsonrpc"] = "2.0", ["method"] = "exit" });
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }
    }
}
