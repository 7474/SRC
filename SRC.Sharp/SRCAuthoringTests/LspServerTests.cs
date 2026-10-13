using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Lsp;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class LspServerTests
    {
        private static int id;

        private static JsonNode Request(ProtocolClient client, string method, JsonObject @params)
        {
            var requestId = ++id;
            client.Send(new JsonObject { ["jsonrpc"] = "2.0", ["id"] = requestId, ["method"] = method, ["params"] = @params });
            var response = client.WaitFor(x => x["id"]?.GetValue<int>() == requestId, method);
            Assert.IsNull(response["error"], response.ToJsonString());
            return response["result"];
        }

        private static void Notify(ProtocolClient client, string method, JsonObject @params)
        {
            client.Send(new JsonObject { ["jsonrpc"] = "2.0", ["method"] = method, ["params"] = @params });
        }

        private static JsonObject Position(string uri, int line, int character) => new JsonObject
        {
            ["textDocument"] = new JsonObject { ["uri"] = uri },
            ["position"] = new JsonObject { ["line"] = line, ["character"] = character },
        };

        [TestMethod]
        public void Lsp_DiagnosticsSymbolsDefinitionAndCompletion()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("test.eve", "スタート:", "Exit");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10) };
                var run = Task.Run(() => server.RunAsync());

                var init = Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Assert.AreEqual(true, init["capabilities"]["definitionProvider"].GetValue<bool>());
                Notify(client, "initialized", new JsonObject());

                // 未保存の内容で解析する（存在しないラベルを Call している）
                var text = "スタート:\nCall サブ\nCall 無い\nExit\n\nサブ:\nReturn";
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = text },
                });
                var diagnostics = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics", "publishDiagnostics");
                var items = diagnostics["params"]["diagnostics"].AsArray();
                Assert.AreEqual(1, items.Count, diagnostics.ToJsonString());
                Assert.AreEqual(2, items[0]["range"]["start"]["line"].GetValue<int>());
                StringAssert.Contains(items[0]["message"].GetValue<string>(), "無い");

                var symbols = Request(client, "textDocument/documentSymbol", new JsonObject { ["textDocument"] = new JsonObject { ["uri"] = uri } }).AsArray();
                CollectionAssert.AreEqual(new[] { "スタート", "サブ" }, symbols.Select(x => x["name"].GetValue<string>()).ToArray());

                // 「Call サブ」のサブから定義（6 行目、0 始まりで 5）へ
                var definition = Request(client, "textDocument/definition", Position(uri, 1, 6));
                Assert.AreEqual(uri, definition["uri"].GetValue<string>());
                Assert.AreEqual(5, definition["range"]["start"]["line"].GetValue<int>());

                var hover = Request(client, "textDocument/hover", Position(uri, 1, 6));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "サブ");

                var completion = Request(client, "textDocument/completion", Position(uri, 3, 0)).AsArray();
                var labels = completion.Select(x => x["label"].GetValue<string>()).ToList();
                CollectionAssert.Contains(labels, "Talk");
                CollectionAssert.Contains(labels, "サブ");

                // 直すと診断が消える
                Notify(client, "textDocument/didChange", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["version"] = 2 },
                    ["contentChanges"] = new JsonArray { new JsonObject { ["text"] = "スタート:\nCall サブ\nExit\n\nサブ:\nReturn" } },
                });
                var cleared = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics", "publishDiagnostics (2)");
                Assert.AreEqual(0, cleared["params"]["diagnostics"].AsArray().Count, cleared.ToJsonString());

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
                Assert.AreEqual(0, run.Result);
            }
        }

        [TestMethod]
        public void WordAt_SplitsOnSeparators()
        {
            Assert.AreEqual("サブ", LspServer.WordAt("Call サブ 1, 2", 6));
            Assert.AreEqual("ラベル", LspServer.WordAt("ラベル:", 1));
            Assert.AreEqual("x", LspServer.WordAt("Set x (y + 1)", 4));
        }
    }
}
