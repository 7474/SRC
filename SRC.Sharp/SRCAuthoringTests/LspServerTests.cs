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

        /// <summary>名前を変える（7474/SRC#1692）: イベントとデータの全体で、語として書いた所だけを変える</summary>
        [TestMethod]
        public void Lsp_RenameAcrossEventsAndData()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var pilot = scenario.Write(System.IO.Path.Combine("Data", "pilot.txt"),
                    "リオ", "リオ, りお, 女性, 汎用, AAAA, 60", "特殊能力なし", "100, 100, 100, 100, 100, 100, 普通", "ＳＰ, 0", "-.bmp, -.mid", "");
                var other = scenario.Write("第2話.eve", "スタート:", "Launch リオ 3 3", "Talk リオ", "リオ、ここが正念場よ。", "End", "Exit");
                var eve = scenario.Write("第1話.eve", "スタート:", "Exit");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10) };
                var run = Task.Run(() => server.RunAsync());
                var init = Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Assert.IsTrue(init["capabilities"]["renameProvider"]["prepareProvider"].GetValue<bool>());
                Notify(client, "initialized", new JsonObject());
                // 保存前の内容で探す
                var text = "スタート:\nCreate 敵 機体 0 リオ 1 3 3\nTalk リオ\nリオンの風、$(リオ)の想い\nEnd\nExit\n\n破壊 リオ:\nExit";
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = text },
                });

                var prepare = Request(client, "textDocument/prepareRename", Position(uri, 1, 15));
                Assert.AreEqual("リオ", prepare["placeholder"].GetValue<string>());
                Assert.AreEqual(14, prepare["range"]["start"]["character"].GetValue<int>());

                var rename = Request(client, "textDocument/rename", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri },
                    ["position"] = new JsonObject { ["line"] = 7, ["character"] = 4 },
                    ["newName"] = "リオ＝ナギ",
                });
                var changes = rename["changes"].AsObject();
                // 第1話: Create・Talk・$(リオ)・破壊 のラベル（「リオンの風」は変えない）
                var first = changes[uri].AsArray().Select(x => (x["range"]["start"]["line"].GetValue<int>(), x["range"]["start"]["character"].GetValue<int>())).ToList();
                CollectionAssert.AreEquivalent(new[] { (1, 14), (2, 5), (3, 8), (7, 3) }, first);
                // 第2話: Launch・Talk（会話の文の「リオ、」は変えない）
                var second = changes[LspServer.PathToUri(other)].AsArray().Select(x => x["range"]["start"]["line"].GetValue<int>()).ToList();
                CollectionAssert.AreEquivalent(new[] { 1, 2 }, second);
                // データ: 名前の行と、愛称の欄
                var data = changes[LspServer.PathToUri(pilot)].AsArray().Select(x => (x["range"]["start"]["line"].GetValue<int>(), x["range"]["start"]["character"].GetValue<int>())).ToList();
                CollectionAssert.AreEquivalent(new[] { (0, 0), (1, 0) }, data);
                Assert.IsTrue(changes[uri].AsArray().All(x => x["newText"].GetValue<string>() == "リオ＝ナギ"));

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        /// <summary>見通しの気づいたこと（7474/SRC#1692）を、開いた・保存したイベントの診断（情報）として出す</summary>
        [TestMethod]
        public void Lsp_PublishesOutlineFindingsAsInformation()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("第1話.eve", "スタート:", "ChangeMap 第1話.map", "Create 味方 機体A 0 主人公 1 2 2", "Set 手がかり 1", "Exit", "破壊 幽霊:", "Exit", "全滅 敵:", "GameClear");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10) };
                var run = Task.Run(() => server.RunAsync());
                Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Notify(client, "initialized", new JsonObject());
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = System.IO.File.ReadAllText(eve) },
                });
                var diagnostics = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics"
                    && x["params"]["uri"].GetValue<string>() == uri
                    && x["params"]["diagnostics"].AsArray().Any(d => d["code"]?.GetValue<string>() == "outline"), "outline findings")["params"]["diagnostics"].AsArray();
                var ghost = diagnostics.Single(d => d["message"].GetValue<string>().Contains("幽霊"));
                Assert.AreEqual(5, ghost["range"]["start"]["line"].GetValue<int>());
                Assert.AreEqual(3, ghost["severity"].GetValue<int>());
                Assert.IsTrue(diagnostics.Any(d => d["message"].GetValue<string>().Contains("「手がかり」")));

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
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

        [TestMethod]
        public void Lsp_HoverAndPlacementAtForMapCoordinates()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                scenario.Write(System.IO.Path.Combine("Data", "System", "terrain.txt"),
                    "0\n平地, plain\n陸, 1, 0, 0\n\n2\n道路, road\n陸, 1, 0, 0\n");
                scenario.Write(System.IO.Path.Combine("Data", "robot.txt"),
                    "試作機\n試作機, しさくき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\n3000, 100, 600, 60\nAAAA, -.bmp\n");
                scenario.Write("stage.map", "MapData\n\n3,3\n0,0\n0,0\n0,0\n0,0\n2,0\n0,0\n0,0\n0,0\n0,0\n");
                var eve = scenario.Write("stage.eve",
                    "スタート:",
                    "Create 味方 試作機 0 パイロット 1 2 2",
                    "Exit");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10), Help = RepositoryHelp() };
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Notify(client, "initialized", new JsonObject());
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = System.IO.File.ReadAllText(eve) },
                });

                // 座標の欄にホバーすると、マップ・座標・地形名が出る
                var hover = Request(client, "textDocument/hover", Position(uri, 1, 25));
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "stage.map");
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "(2, 2)");
                StringAssert.Contains(hover["contents"]["value"].GetValue<string>(), "道路 (2)");

                // カスタムリクエスト srcsharp/placementAt でも配置情報とマップのフルパスが返る
                var placement = Request(client, "srcsharp/placementAt", Position(uri, 1, 0));
                Assert.AreEqual("stage.map", placement["mapPath"].GetValue<string>());
                Assert.AreEqual(2, placement["x"].GetValue<int>());
                Assert.AreEqual(2, placement["y"].GetValue<int>());
                StringAssert.Contains(placement["terrainName"].GetValue<string>(), "道路");
                StringAssert.EndsWith(placement["fullMapPath"].GetValue<string>().Replace('\\', '/'), "/stage.map");

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        [TestMethod]
        public void Lsp_MessageAndDialogSupport()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var pilot = scenario.Write(System.IO.Path.Combine("Data", "pilot.txt"),
                    "主人公", "主人公, しゅじんこう, 男性, 汎用, AAAA, 120", "特殊能力", "メッセージ=ヒーロー, 1", "150, 145, 150, 150, 150, 155, 普通", "ＳＰ, 0", "-.bmp, -.mid", "");
                var robot = scenario.Write(System.IO.Path.Combine("Data", "robot.txt"),
                    "試作機", "試作機, しさくき, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp",
                    "ライフル, 1200, 2, 4, +10, 8, -, -, AAAA, +5, 実", "===", "");
                var msg = scenario.Write(System.IO.Path.Combine("Data", "pilot_message.txt"),
                    "主人公\n回避, かわした\nライフル(対ザコ), 撃つ！\n攻撃(無い武器), だめ\n<BIG>テスト</BIG> $(武器)\n");
                var uri = LspServer.PathToUri(msg);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10), Help = RepositoryHelp() };
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject
                {
                    ["capabilities"] = new JsonObject(),
                    ["initializationOptions"] = new JsonObject { ["rules"] = new JsonArray("src") },
                });
                Notify(client, "initialized", new JsonObject());
                var msgText = "ヒーロー\n回避, かわした\nライフル(対ザコ), 撃つ！\n攻撃(無い武器), だめ\n主人公\n";
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-data", ["version"] = 1, ["text"] = msgText },
                });

                // 「攻撃(無い武器)」に診断が出る（「ヒーロー」は MessageType なので OK）
                var diagnostics = client.WaitFor(x => x["method"]?.GetValue<string>() == "textDocument/publishDiagnostics"
                    && x["params"]["uri"].GetValue<string>() == uri, "publishDiagnostics");
                var items = diagnostics["params"]["diagnostics"].AsArray();
                Assert.IsTrue(items.Any(d => d["code"]?.GetValue<string>() == "message-situation-unknown" && d["range"]["start"]["line"].GetValue<int>() == 3), diagnostics.ToJsonString());
                Assert.IsFalse(items.Any(d => d["range"]["start"]["line"].GetValue<int>() == 0), diagnostics.ToJsonString());

                // 補完に標準シチュエーション・修飾子・タグ・変数・パイロット・武器が含まれる
                var completion = Request(client, "textDocument/completion", Position(uri, 4, 0)).AsArray();
                var labels = completion.Select(x => x["label"].GetValue<string>()).ToList();
                CollectionAssert.Contains(labels, "回避");
                CollectionAssert.Contains(labels, "(対ザコ)");
                CollectionAssert.Contains(labels, "<BIG>");
                CollectionAssert.Contains(labels, "$(武器)");
                CollectionAssert.Contains(labels, "主人公");
                CollectionAssert.Contains(labels, "ライフル");

                // ホバー: シチュエーション・修飾子・武器・パイロット（MessageType）
                var hoverSit = Request(client, "textDocument/hover", Position(uri, 1, 1));
                StringAssert.Contains(hoverSit["contents"]["value"].GetValue<string>(), "回避");
                var hoverMod = Request(client, "textDocument/hover", Position(uri, 2, 6));
                StringAssert.Contains(hoverMod["contents"]["value"].GetValue<string>(), "対ザコ");
                var hoverWeapon = Request(client, "textDocument/hover", Position(uri, 2, 1));
                StringAssert.Contains(hoverWeapon["contents"]["value"].GetValue<string>(), "ライフル");
                var hoverType = Request(client, "textDocument/hover", Position(uri, 0, 1));
                StringAssert.Contains(hoverType["contents"]["value"].GetValue<string>(), "主人公");

                // 定義へ移動: パイロット・武器
                var defPilot = Request(client, "textDocument/definition", Position(uri, 4, 1));
                Assert.AreEqual(LspServer.PathToUri(pilot), defPilot["uri"].GetValue<string>());
                Assert.AreEqual(0, defPilot["range"]["start"]["line"].GetValue<int>());
                var defWeapon = Request(client, "textDocument/definition", Position(uri, 2, 1));
                Assert.AreEqual(LspServer.PathToUri(robot), defWeapon["uri"].GetValue<string>());
                Assert.AreEqual(6, defWeapon["range"]["start"]["line"].GetValue<int>());

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }

        [TestMethod]
        public void Lsp_EventFlowReturnsGraphAndUsesUnsavedChanges()
        {
            using (var scenario = new TestScenario())
            using (var client = new ProtocolClient())
            {
                var eve = scenario.Write("ep01.eve",
                    "スタート:",
                    "Call 保存済みサブ",
                    "Exit",
                    "",
                    "保存済みサブ:",
                    "Return");
                var uri = LspServer.PathToUri(eve);
                var server = new LspServer(client.ServerInput, client.ServerOutput) { Debounce = TimeSpan.FromMilliseconds(10), Help = RepositoryHelp() };
                var run = Task.Run(() => server.RunAsync());

                Request(client, "initialize", new JsonObject { ["capabilities"] = new JsonObject() });
                Notify(client, "initialized", new JsonObject());
                Notify(client, "textDocument/didOpen", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["languageId"] = "src-eve", ["version"] = 1, ["text"] = System.IO.File.ReadAllText(eve) },
                });

                var flow1 = Request(client, "srcsharp/eventFlow", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri },
                });
                var nodes1 = flow1["nodes"].AsArray().Select(n => n["name"].GetValue<string>()).ToList();
                CollectionAssert.Contains(nodes1, "保存済みサブ");

                // 未保存の編集を didChange で送ると、保存前でもイベントの流れの図に反映される
                var editedText = string.Join("\r\n",
                    "スタート:",
                    "If フラグ = 1 Then",
                    "Call 未保存サブ",
                    "EndIf",
                    "Exit",
                    "",
                    "未保存サブ:",
                    "Return");
                Notify(client, "textDocument/didChange", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri, ["version"] = 2 },
                    ["contentChanges"] = new JsonArray { new JsonObject { ["text"] = editedText } },
                });

                var flow2 = Request(client, "srcsharp/eventFlow", new JsonObject
                {
                    ["textDocument"] = new JsonObject { ["uri"] = uri },
                });
                var nodes2 = flow2["nodes"].AsArray().Select(n => n["name"].GetValue<string>()).ToList();
                CollectionAssert.Contains(nodes2, "未保存サブ");
                var edges2 = flow2["edges"].AsArray();
                Assert.IsTrue(edges2.Any(e => e["target"]?.GetValue<string>() == "未保存サブ" && (e["condition"]?.GetValue<string>() ?? "").Contains("フラグ = 1")), flow2.ToJsonString());

                Request(client, "shutdown", null);
                Notify(client, "exit", null);
                Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(10)));
            }
        }
    }
}
