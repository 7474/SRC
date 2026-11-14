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
        public void Lsp_DataFileDiagnosticsWithRules()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var robot = scenario.Write(System.IO.Path.Combine("Data", "robot.txt"),
                    "海の機体", "海の機体, うみのきたい, 汎用, 1, 2", "陸海, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp",
                    "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 格突", "");
                var uri = LspServer.PathToUri(robot);
                var server = new LspServer(client.ServerInput, client.ServerOutput);
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject
                {
                    ["capabilities"] = new JsonObject(),
                    ["initializationOptions"] = new JsonObject { ["rules"] = new JsonArray("src", "gsc-robo") },
                });
                Notify(client, "initialized", new JsonObject());
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-data", ["version"] = 1, ["text"] = System.IO.File.ReadAllText(robot) },
                });

                var diagnostics = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics"
                    && x["params"]["uri"].GetValue<string>() == uri, "publishDiagnostics");
                var items = diagnostics["params"]["diagnostics"].AsArray();
                var codes = items.Select(x => x["code"]?.GetValue<string>()).ToList();
                CollectionAssert.Contains(codes, "unit-transportation-invalid", diagnostics.ToJsonString());
                CollectionAssert.Contains(codes, "weapon-range1-infight", diagnostics.ToJsonString());
                var invalid = items.First(x => x["code"]?.GetValue<string>() == "unit-transportation-invalid");
                // 3 行目（0 始まりで 2）の移動可能地形。情報（3）。どのユニットか
                Assert.AreEqual(2, invalid["range"]["start"]["line"].GetValue<int>());
                Assert.AreEqual(3, invalid["severity"].GetValue<int>());
                StringAssert.EndsWith(invalid["message"].GetValue<string>(), "[海の機体]");

                // 直して保存すると、診断が消える
                System.IO.File.WriteAllText(robot, string.Join("\n",
                    "海の機体", "海の機体, うみのきたい, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp", ""));
                Notify(client, "textDocument/didSave", new JsonObject { ["textDocument"] = new JsonObject { ["uri"] = uri } });
                var cleared = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics"
                    && x["params"]["uri"].GetValue<string>() == uri && x["params"]["diagnostics"].AsArray().Count == 0, "cleared");
                Assert.IsNotNull(cleared);

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.AreEqual(0, run.Result);
            }
        }

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

        /// <summary>リポジトリの同梱のヘルプ（SRC.Sharp.Help/src）</summary>
        private static HelpReference RepositoryHelp()
        {
            var dir = new System.IO.DirectoryInfo(AppContext.BaseDirectory);
            while (!System.IO.Directory.Exists(System.IO.Path.Combine(dir.FullName, "SRC.Sharp.Help")))
            {
                dir = dir.Parent;
            }
            var src = System.IO.Path.Combine(dir.FullName, "SRC.Sharp.Help", "src");
            return new HelpReference(System.IO.Directory.GetFiles(src, "*.md")
                .Select(f => (System.IO.Path.GetFileNameWithoutExtension(f), System.IO.File.ReadAllText(f))));
        }

        [TestMethod]
        public void HelpReference_CommandsAndKeywords()
        {
            var help = RepositoryHelp();
            var create = help.Command("Create");
            StringAssert.Contains(create, "ユニットを作成");
            StringAssert.Contains(create, "*party unit rank pilot level x y*");
            StringAssert.Contains(create, "srcsharp help Createコマンド");
            Assert.IsNull(help.Command("存在しない"));
            // 特殊能力（レベル・値を付けて書いても引ける）と武器の属性
            StringAssert.Contains(help.Keyword("ＨＰ回復Lv2"), "ＨＰ回復");
            StringAssert.Contains(help.Keyword("不安定=擬似テレパスシステム"), "不安定");
            StringAssert.Contains(help.Keyword("格"), "格闘型");
            CollectionAssert.Contains(help.Keywords.ToList(), "バリア");
            Assert.IsFalse(help.Keywords.Contains("書式"));
        }

        /// <summary>イベントのデータの名前の参照（診断・ホバー・定義へ移動・補完）とコマンドのホバー（7474/SRC#1642）</summary>
        [TestMethod]
        public void Lsp_DataReferencesAndCommandHelp()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var robot = scenario.Write(System.IO.Path.Combine("Data", "robot.txt"),
                    "試作機", "試作機, しさくき, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp",
                    "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突", "===", "");
                scenario.Write(System.IO.Path.Combine("Data", "pilot.txt"),
                    "敵兵(ザコ)", "敵兵, てきへい, 男性, 汎用, AAAA, 60", "特殊能力なし", "100, 100, 100, 100, 100, 100, 普通", "ＳＰ, 0", "-.bmp, -.mid", "");
                var eve = scenario.Write("test.eve", "スタート:", "Exit");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10), Help = RepositoryHelp() };
                var run = Task.Run(() => server.RunAsync());
                Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Notify(client, "initialized", new JsonObject());

                // 0: スタート: / 1: Create（ある名前・括弧のある名前）/ 2: Create（無いユニット）/ 3: 変数に入れた名前・式は見ない
                var text = "スタート:\nCreate 敵 試作機 0 敵兵(ザコ) 1 3 3\nCreate 敵 無い機体 0 敵兵 1 4 4\nSet 機体 試作機\nCreate 敵 機体 0 $(パイロット) 1 5 5\nExit";
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = text },
                });
                var diagnostics = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics"
                    && x["params"]["uri"].GetValue<string>() == uri, "publishDiagnostics");
                var items = diagnostics["params"]["diagnostics"].AsArray();
                Assert.AreEqual(1, items.Count, diagnostics.ToJsonString());
                Assert.AreEqual(2, items[0]["range"]["start"]["line"].GetValue<int>());
                StringAssert.Contains(items[0]["message"].GetValue<string>(), "無い機体");
                Assert.AreEqual(2, items[0]["severity"].GetValue<int>());

                // データの名前のホバー（要約）と定義へ移動（robot.txt の名前の行）
                var hover = Request(client, "textDocument/hover", Position(uri, 1, 11));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "HP 3000");
                var definition = Request(client, "textDocument/definition", Position(uri, 1, 11));
                Assert.AreEqual(LspServer.PathToUri(robot), definition["uri"].GetValue<string>());
                Assert.AreEqual(0, definition["range"]["start"]["line"].GetValue<int>());
                // 括弧のある名前（パイロットの愛称でも引ける）
                hover = Request(client, "textDocument/hover", Position(uri, 1, 18));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "パイロット **敵兵(ザコ)**");

                // コマンドのホバー（同梱のヘルプ）
                hover = Request(client, "textDocument/hover", Position(uri, 1, 2));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "ユニットを作成");

                // 補完にデータの名前と、コマンドの説明
                var completion = Request(client, "textDocument/completion", Position(uri, 5, 0)).AsArray();
                var unit = completion.First(x => x["label"].GetValue<string>() == "試作機");
                Assert.AreEqual("ユニット", unit["detail"].GetValue<string>());
                var create = completion.First(x => x["label"].GetValue<string>() == "Create");
                StringAssert.Contains(create["documentation"]["value"].GetValue<string>(), "ユニットを作成");

                // データのファイルでは、特殊能力・属性の説明と補完
                var robotUri = LspServer.PathToUri(robot);
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = robotUri, ["languageId"] = "src-data", ["version"] = 1, ["text"] = System.IO.File.ReadAllText(robot) },
                });
                hover = Request(client, "textDocument/hover", Position(robotUri, 6, 44));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "突");
                completion = Request(client, "textDocument/completion", Position(robotUri, 3, 0)).AsArray();
                CollectionAssert.Contains(completion.Select(x => x["label"].GetValue<string>()).ToList(), "バリア");

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
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
