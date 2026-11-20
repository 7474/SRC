using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>シナリオの見通し（7474/SRC#1692）。ランタン号（SRC.Sharp/Content/lantern）で確かめる</summary>
    [TestClass]
    public class ScenarioOutlineTests
    {
        private static string Lantern()
        {
            var dir = Directory.GetCurrentDirectory();
            while (dir != null && !Directory.Exists(Path.Combine(dir, "Content", "lantern")))
            {
                dir = Path.GetDirectoryName(dir);
            }
            Assert.IsNotNull(dir, "SRC.Sharp/Content/lantern が見つからない");
            return Path.Combine(dir, "Content", "lantern");
        }

        [TestMethod]
        public void Analyze_ListsEpisodesEventsScriptVariablesAndCast()
        {
            var outline = ScenarioOutline.Analyze(Lantern());

            // 話は話のつながりの順。第1話が先
            var first = outline.Episodes.First();
            Assert.AreEqual("01_灯の港", first.Title);
            CollectionAssert.IsSubsetOf(new[] { "グスタフ", "ソラ", "リオ", "ミュウ" }, first.Joins);
            Assert.IsTrue(first.Next.Any(n => n.Contains("02_霧の渓谷")));
            Assert.IsTrue(outline.Episodes.Any(e => e.Title == "06_灯のゆくえ"));

            // 出来事: きっかけ・いつ・すること
            var turn2 = first.Events.Single(e => e.Label == "ターン 2 味方");
            Assert.AreEqual("turn", turn2.Kind);
            Assert.AreEqual("2 ターン目の味方のフェイズの始め", turn2.When);
            Assert.IsTrue(turn2.Actions.Contains("加入・出撃: ミュウ（祈り手）"), string.Join(",", turn2.Actions));
            Assert.IsTrue(turn2.Actions.Any(a => a.StartsWith("援軍: 煤鴉兵(ザコ)")));
            var annihilated = first.Events.Single(e => e.Label == "全滅 敵");
            Assert.AreEqual("敵が全滅したとき", annihilated.When);
            Assert.IsTrue(annihilated.Actions.Any(a => a.StartsWith("次の話へ")));
            // Switch・Case は「変数 = 値」で出す
            Assert.IsTrue(first.Events.Single(e => e.Label == "プロローグ").Conditions.Contains("選択 = 1"));

            // 台本: 話す人と行
            var line = outline.Script.First(l => l.Text.Contains("今日の荷物"));
            Assert.AreEqual("ソラ", line.Speaker);
            Assert.AreEqual("プロローグ", line.Event);
            Assert.AreEqual("01_灯の港.eve", line.File);
            Assert.IsTrue(first.Speakers["ソラ"] > 0);

            // 変数: 設定と読む場所（話をまたぐ Global）
            var reputation = outline.Variables.Single(v => v.Name == "港の評判");
            Assert.IsTrue(reputation.Global);
            Assert.IsTrue(reputation.Sets.Count >= 1);
            Assert.IsTrue(reputation.Reads.Any(r => r.file.Contains("06_灯のゆくえ")));

            // 書き出し
            var md = outline.ToMarkdown();
            StringAssert.Contains(md, "## 部隊と登場人物");
            StringAssert.Contains(md, "## 出来事");
            StringAssert.Contains(md, "## 変数");
            StringAssert.Contains(md, "## 台本");
            StringAssert.Contains(md, "#L270)");
            using var json = System.Text.Json.JsonDocument.Parse(outline.ToJson());
            Assert.IsTrue(json.RootElement.GetProperty("script").GetArrayLength() > 100);
        }

        [TestMethod]
        public void Findings_ReportUnreachableEventsUnusedOrMistypedVariablesAndMissingEnding()
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-outline-" + System.Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(dir, "Data"));
            try
            {
                File.WriteAllText(Path.Combine(dir, "第1話.eve"), string.Join("\r\n",
                    "スタート:",
                    "ChangeMap 第1話.map",
                    "Create 味方 機体A 0 主人公 1 2 2",
                    "Set 手がかり 1",
                    "Set 好感度 1",
                    "Exit",
                    "破壊 幽霊:",
                    "Talk 主人公",
                    "だれだ？",
                    "End",
                    "Exit",
                    "破壊 主人公:",
                    "GameOver",
                    "ターン 2 味方:",
                    "If 好感度 > 0 Then",
                    "  Set 好感渡 2",
                    "EndIf",
                    "Exit",
                    ""));
                var outline = ScenarioOutline.Analyze(dir);
                var messages = outline.Findings.Select(f => f.Message).ToList();
                CollectionAssert.Contains(messages, "「破壊 幽霊」の「幽霊」は、どの話でも Create・Launch されていない（起きない出来事かもしれない）");
                CollectionAssert.Contains(messages, "変数「手がかり」は設定するだけで、どこでも読んでいない");
                CollectionAssert.Contains(messages, "変数「好感度」と「好感渡」は 1 文字違い（書き間違いかもしれない）");
                CollectionAssert.Contains(messages, "第1話に勝ちの終わり方（全滅 敵・GameClear・Continue）が無い");
                // 出てくる人（主人公）の破壊は知らせない。読んでいる変数（好感度）も知らせない
                Assert.IsFalse(messages.Any(m => m.Contains("「主人公」は") || m == "変数「好感度」は設定するだけで、どこでも読んでいない"), string.Join("\n", messages));
                var ghost = outline.Findings.First(f => f.Message.Contains("幽霊"));
                Assert.AreEqual(("第1話.eve", 7), (ghost.File, ghost.Line));
                StringAssert.Contains(outline.ToMarkdown(), "## 気づいたこと");
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        /// <summary>途中の話を、手で決めた始めの値から遊ぶひな形（7474/SRC#1692）。前の話までの部隊・設定・話をまたぐ変数を作って Continue する</summary>
        [TestMethod]
        public void StartState_BuildsPartySetupAndGlobalsBeforeTheEpisode()
        {
            var outline = ScenarioOutline.Analyze(Lantern());
            var text = SRCAuthoring.Templates.StartStateTemplate.Build(outline, "ステージ/03_煤の工場.eve");
            var lines = text.Split("\r\n");
            Assert.AreEqual("@ランタン", lines[0]);
            CollectionAssert.IsSubsetOf(new[] { "プロローグ:", "Load 煤の軍勢", "Option アイテム交換", "Unit 灯機ホタル 0", "Pilot ソラ 7", "Ride ソラ", "Pilot ミュウ 8", "Money 0", "Global 港の評判", "Set 港の評判 0" }, lines);
            Assert.AreEqual("Continue ステージ\\03_煤の工場.eve", lines.Last(l => l.Length > 0));
            // 第1話の前には加わった味方がいない
            StringAssert.Contains(SRCAuthoring.Templates.StartStateTemplate.Build(outline, "01_灯の港.eve"), "前の話で加わった味方はいない");
            Assert.IsNull(SRCAuthoring.Templates.StartStateTemplate.Build(outline, "無い.eve"));
        }

        [TestMethod]
        [DataRow("ターン 全 敵", "turn", "毎ターンの敵のフェイズの始め")]
        [DataRow("破壊 シャア", "destroyed", "シャアが落ちたとき")]
        [DataRow("損傷率 リオ 60", "damaged", "リオの損傷率が 60% 以上になったとき")]
        [DataRow("攻撃 アムロ シャア", "attack", "アムロとシャアが戦うとき")]
        [DataRow("進入 味方 12 1", "enter", "味方が (12,1) に入ったとき")]
        [DataRow("章題", "sub", "Call・Goto の先")]
        public void Classify_DescribesWhenTheEventHappens(string label, string kind, string when)
        {
            Assert.AreEqual((kind, when), ScenarioOutline.Classify(label));
        }
    }
}
