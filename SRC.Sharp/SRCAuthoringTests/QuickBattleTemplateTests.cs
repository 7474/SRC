using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Templates;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>かんたん作成のひな形（7474/SRC#1659）。遊べることは CliTests.QuickBattle_LintsCleanAndPlaysToTheEnd で確かめる</summary>
    [TestClass]
    public class QuickBattleTemplateTests
    {
        private const string Json = @"{
  ""title"": ""はじめての戦い"",
  ""dataTitles"": [""作品A"", ""作品B"", ""作品A""],
  ""width"": 12, ""height"": 8,
  ""allies"": [{ ""unit"": ""見習い機"", ""pilot"": ""主人公"", ""level"": 5, ""x"": 2, ""y"": 3 }],
  ""enemies"": [
    { ""unit"": ""偵察機"", ""pilot"": ""敵兵(ザコ)"", ""level"": 3, ""x"": 10, ""y"": 4 },
    { ""unit"": ""指揮官機"", ""pilot"": ""敵将"", ""level"": 8, ""rank"": 2, ""bossRank"": 3, ""x"": 11, ""y"": 5 }
  ],
  ""opening"": [
    { ""speaker"": """", ""text"": ""荒野に風が吹く。"" },
    { ""speaker"": ""主人公"", ""text"": ""行くぞ！\nEnd ではない\n\n# 見出しでもない"" },
    { ""speaker"": ""主人公"", ""text"": ""続けて話す"" },
    { ""speaker"": ""敵兵(ザコ)"", ""text"": ""来たな"" }
  ],
  ""victory"": [],
  ""defeat"": [{ ""speaker"": ""主人公"", ""text"": ""くっ……"" }]
}";

        [TestMethod]
        public void Files_WritesEventAndMap()
        {
            var battle = QuickBattleTemplate.FromJson(Json);
            Assert.AreEqual(0, QuickBattleTemplate.Validate(battle).Count, string.Join("\n", QuickBattleTemplate.Validate(battle)));
            var files = QuickBattleTemplate.Files(battle);
            CollectionAssert.AreEquivalent(new[] { "はじめての戦い.eve", "はじめての戦い.map" }, files.Keys.ToArray());
            var eve = files["はじめての戦い.eve"].Split("\r\n");

            // 作品は重ねずに 1 回ずつ
            CollectionAssert.AreEqual(new[] { "@作品A", "@作品B" }, eve.Where(x => x.StartsWith("@")).ToArray());
            CollectionAssert.Contains(eve, "ChangeMap はじめての戦い.map");
            CollectionAssert.Contains(eve, "Create 味方 見習い機 0 主人公 5 2 3");
            CollectionAssert.Contains(eve, "Create 敵 偵察機 0 \"敵兵(ザコ)\" 3 10 4");
            // ランクは Create の 3 番目の引数。ボスランクは Create のすぐ後の BossRank（Create で選ばれたユニットに付く）。ボスでなければ書かない
            var boss = System.Array.IndexOf(eve, "Create 敵 指揮官機 2 敵将 8 11 5");
            Assert.IsTrue(boss > 0);
            Assert.AreEqual("BossRank 3", eve[boss + 1]);
            Assert.AreEqual(1, eve.Count(x => x.StartsWith("BossRank")));

            // 話し手が変わるたびに Talk。ナレーションは名前なし。命令や注釈に読まれる行は全角の空白を足す
            var start = System.Array.IndexOf(eve, "スタート:");
            var exit = System.Array.IndexOf(eve, "Exit", start);
            CollectionAssert.AreEqual(new[]
            {
                "Talk", "荒野に風が吹く。",
                "Talk 主人公", "行くぞ！", "　End ではない", "　# 見出しでもない", "続けて話す",
                "Talk \"敵兵(ザコ)\"", "来たな",
                "End",
            }, eve.Skip(start + 6).Take(exit - start - 6).ToArray());

            // 勝ちのセリフが無ければ「勝利！」
            var victory = System.Array.IndexOf(eve, "全滅 敵:");
            CollectionAssert.AreEqual(new[] { "Talk システム", "勝利！", "End", "GameClear" }, eve.Skip(victory + 1).Take(4).ToArray());

            StringAssert.StartsWith(files["はじめての戦い.map"], "MapData");
            var map = SRCAuthoring.Maps.MapDocument.Parse(System.Text.Encoding.UTF8.GetBytes(files["はじめての戦い.map"]));
            Assert.AreEqual(12, map.Width);
            Assert.AreEqual(8, map.Height);
        }

        [TestMethod]
        public void Validate_ReportsMissingAndConflictingPieces()
        {
            var battle = QuickBattleTemplate.FromJson(@"{
  ""width"": 10, ""height"": 10,
  ""allies"": [{ ""unit"": ""A"", ""pilot"": ""a"", ""x"": 3, ""y"": 3 }, { ""unit"": ""B"", ""pilot"": ""b"", ""x"": 11, ""y"": 3 }],
  ""enemies"": [{ ""unit"": ""C"", ""pilot"": ""c"", ""x"": 3, ""y"": 3 }, { ""unit"": """", ""pilot"": ""d"", ""x"": 5, ""y"": 5 },
    { ""unit"": ""E"", ""pilot"": ""e"", ""rank"": 21, ""bossRank"": 6, ""x"": 7, ""y"": 7 }],
  ""opening"": [{ ""speaker"": ""だれか"", ""text"": ""…"" }, { ""speaker"": ""システム"", ""text"": ""…"" }]
}");
            var errors = QuickBattleTemplate.Validate(battle);
            CollectionAssert.Contains(errors.ToList(), "味方の「B」がマップの外にあります");
            CollectionAssert.Contains(errors.ToList(), "敵の「C」がほかのユニットと同じ場所にあります");
            CollectionAssert.Contains(errors.ToList(), "敵のユニットかパイロットが選ばれていません");
            CollectionAssert.Contains(errors.ToList(), "セリフの「だれか」は、置いたユニットのパイロットではありません");
            CollectionAssert.Contains(errors.ToList(), "敵の「E」のランクは 0〜20 にしてください");
            CollectionAssert.Contains(errors.ToList(), "敵の「E」のボスランクは 0〜5（ボスにしないなら -1）にしてください");
            Assert.AreEqual(6, errors.Count, string.Join("\n", errors));

            var empty = QuickBattleTemplate.Validate(QuickBattleTemplate.FromJson("{}"));
            CollectionAssert.Contains(empty.ToList(), "味方のユニットがありません");
            CollectionAssert.Contains(empty.ToList(), "敵のユニットがありません");
        }

        private const string EventsJson = @"{
  ""title"": ""砦の戦い"", ""width"": 12, ""height"": 8,
  ""allies"": [{ ""unit"": ""見習い機"", ""pilot"": ""主人公"", ""x"": 2, ""y"": 3 }, { ""unit"": ""支援機"", ""pilot"": ""相棒"", ""x"": 2, ""y"": 4 }],
  ""enemies"": [{ ""unit"": ""偵察機"", ""pilot"": ""敵兵(ザコ)"", ""x"": 10, ""y"": 4 }, { ""unit"": ""指揮官機"", ""pilot"": ""敵将"", ""bossRank"": 2, ""x"": 11, ""y"": 5 }],
  ""victory"": [{ ""speaker"": ""主人公"", ""text"": ""勝った！"" }],
  ""defeat"": [{ ""speaker"": ""主人公"", ""text"": ""くっ……"" }],
  ""goal"": { ""kind"": ""boss"", ""target"": ""敵将"" },
  ""loss"": { ""kind"": ""unit"", ""target"": ""相棒"" },
  ""events"": [
    { ""trigger"": ""turn"", ""turn"": 2, ""phase"": ""敵"", ""talk"": [{ ""speaker"": ""敵将"", ""text"": ""増援を出せ！"" }],
      ""enemies"": [{ ""unit"": ""偵察機"", ""pilot"": ""敵兵(ザコ)"", ""level"": 4, ""x"": 12, ""y"": 1 }] },
    { ""trigger"": ""attack"", ""unit"": ""主人公"", ""target"": ""敵将"", ""talk"": [{ ""speaker"": ""敵将"", ""text"": ""来たか、小僧！"" }] },
    { ""trigger"": ""damaged"", ""unit"": ""敵将"", ""ratio"": 70, ""retreat"": true, ""talk"": [{ ""speaker"": ""敵将"", ""text"": ""覚えていろ！"" }] },
    { ""trigger"": ""destroyed"", ""unit"": ""敵将"", ""talk"": [{ ""speaker"": ""敵将"", ""text"": ""ば、ばかな……"" }] }
  ]
}";

        /// <summary>出来事（7474/SRC#1689）: 勝敗の条件・援軍・戦闘中の会話・撤退を、SRC のヘルプにある普通の書き方のイベントにする</summary>
        [TestMethod]
        public void Files_WritesGoalLossAndEventsAsPlainEventLabels()
        {
            var battle = QuickBattleTemplate.FromJson(EventsJson);
            Assert.AreEqual(0, QuickBattleTemplate.Validate(battle).Count, string.Join("\n", QuickBattleTemplate.Validate(battle)));
            var eve = QuickBattleTemplate.Files(battle)["砦の戦い.eve"].Split("\r\n");
            string[] Block(string label)
            {
                var i = System.Array.IndexOf(eve, label);
                Assert.IsTrue(i >= 0, label + "\n" + string.Join("\n", eve));
                return eve.Skip(i + 1).TakeWhile(x => x.Length > 0).ToArray();
            }

            // 作戦目的
            CollectionAssert.AreEqual(new[] { "Talk システム", "勝利条件：敵将の撃破", "敗北条件：味方の全滅、または相棒の撃墜", "End", "Exit" }, Block("勝利条件:"));
            // 援軍（2 ターン目の敵のフェイズの始め）
            CollectionAssert.AreEqual(new[] { "Talk 敵将", "増援を出せ！", "End", "Create 敵 偵察機 0 \"敵兵(ザコ)\" 4 12 1", "Exit" }, Block("ターン 2 敵:"));
            // 戦闘中の会話
            CollectionAssert.AreEqual(new[] { "Talk 敵将", "来たか、小僧！", "End", "Exit" }, Block("攻撃 主人公 敵将:"));
            // HP が減ったボスが退く。倒す相手が退いたら勝ち
            CollectionAssert.AreEqual(new[] { "Talk 敵将", "覚えていろ！", "End", "Escape 敵将", "Talk 主人公", "勝った！", "End", "GameClear" }, Block("損傷率 敵将 70:"));
            // 倒す相手が落ちたときの会話と勝ちは、1 つのラベルにまとめる
            CollectionAssert.AreEqual(new[] { "Talk 敵将", "ば、ばかな……", "End", "Talk 主人公", "勝った！", "End", "GameClear" }, Block("破壊 敵将:"));
            Assert.AreEqual(1, eve.Count(x => x == "破壊 敵将:"));
            // 相棒が落ちたら負け
            CollectionAssert.AreEqual(new[] { "Talk 主人公", "くっ……", "End", "GameOver" }, Block("破壊 相棒:"));
            // 全滅は今までどおり
            CollectionAssert.AreEqual(new[] { "Talk 主人公", "勝った！", "End", "GameClear" }, Block("全滅 敵:"));
            CollectionAssert.AreEqual(new[] { "Talk 主人公", "くっ……", "End", "GameOver" }, Block("全滅 味方:"));
            // 注釈を付ける
            Assert.AreEqual("# 敵将を倒したら勝ち", eve[System.Array.IndexOf(eve, "破壊 敵将:") - 1]);
        }

        [TestMethod]
        public void Files_SurviveReachAndTurnLimit()
        {
            var battle = QuickBattleTemplate.FromJson(@"{
  ""width"": 12, ""height"": 8,
  ""allies"": [{ ""unit"": ""A"", ""pilot"": ""a"", ""x"": 2, ""y"": 3 }],
  ""enemies"": [{ ""unit"": ""B"", ""pilot"": ""b"", ""x"": 10, ""y"": 4 }],
  ""goal"": { ""kind"": ""survive"", ""turns"": 3 },
  ""loss"": { ""kind"": ""turns"", ""turns"": 5 },
  ""events"": [{ ""trigger"": ""turn"", ""turn"": 4, ""phase"": ""味方"", ""talk"": [{ ""speaker"": ""a"", ""text"": ""あと少し"" }] }]
}");
            Assert.AreEqual(0, QuickBattleTemplate.Validate(battle).Count, string.Join("\n", QuickBattleTemplate.Validate(battle)));
            var eve = QuickBattleTemplate.Files(battle).Values.First(x => x.Contains("スタート:")).Split("\r\n");
            // 3 ターン守りきる = 4 ターン目の味方のフェイズの始めに勝ち。同じラベルの会話は先に出す
            var survive = System.Array.IndexOf(eve, "ターン 4 味方:");
            CollectionAssert.AreEqual(new[] { "Talk a", "あと少し", "End", "Talk システム", "勝利！", "End", "GameClear" }, eve.Skip(survive + 1).Take(7).ToArray());
            CollectionAssert.Contains(eve, "ターン 6 味方:");
            Assert.AreEqual("GameOver", eve[System.Array.IndexOf(eve, "ターン 6 味方:") + 1]);

            battle.Goal = new QuickGoal { Kind = "reach", X = 12, Y = 1 };
            battle.Loss = new QuickLoss();
            eve = QuickBattleTemplate.Files(battle).Values.First(x => x.Contains("スタート:")).Split("\r\n");
            CollectionAssert.Contains(eve, "進入 味方 12 1:");
            battle.Goal.Target = "a";
            CollectionAssert.Contains(QuickBattleTemplate.Files(battle).Values.First(x => x.Contains("スタート:")).Split("\r\n"), "進入 a 12 1:");
        }

        [TestMethod]
        public void Validate_ReportsBrokenConditionsAndEvents()
        {
            var battle = QuickBattleTemplate.FromJson(@"{
  ""width"": 10, ""height"": 10,
  ""allies"": [{ ""unit"": ""A"", ""pilot"": ""a"", ""x"": 2, ""y"": 3 }],
  ""enemies"": [{ ""unit"": ""B"", ""pilot"": ""b"", ""x"": 8, ""y"": 4 }],
  ""goal"": { ""kind"": ""boss"", ""target"": ""a"" },
  ""loss"": { ""kind"": ""unit"", ""target"": ""b"" },
  ""events"": [
    { ""trigger"": ""destroyed"", ""unit"": ""b"", ""retreat"": true },
    { ""trigger"": ""attack"", ""unit"": ""a"", ""talk"": [{ ""speaker"": ""a"", ""text"": ""…"" }] },
    { ""trigger"": ""turn"", ""turn"": 2 },
    { ""trigger"": ""turn"", ""turn"": 3, ""enemies"": [{ ""unit"": ""C"", ""pilot"": ""c"", ""x"": 11, ""y"": 1 }] }
  ]
}");
            var errors = QuickBattleTemplate.Validate(battle).ToList();
            CollectionAssert.Contains(errors, "勝利の条件で倒す相手は、置いた敵のパイロットから選んでください");
            CollectionAssert.Contains(errors, "敗北の条件で落ちてはいけないユニットは、置いた味方のパイロットから選んでください");
            CollectionAssert.Contains(errors, "出来事 1: 退くのは、落ちる前のユニットだけです（きっかけを損傷率などにしてください）");
            CollectionAssert.Contains(errors, "出来事 2: 戦う相手を、置いたユニットのパイロットから選んでください");
            CollectionAssert.Contains(errors, "出来事 3: 会話・援軍・退くのどれかを足してください");
            CollectionAssert.Contains(errors, "敵の「C」がマップの外にあります");
            Assert.AreEqual(6, errors.Count, string.Join("\n", errors));

            // 援軍の置く位置は、はじめのユニットと重なってもよい（SRC が近くの空いたマスに置く）
            var reinforce = QuickBattleTemplate.FromJson(@"{
  ""allies"": [{ ""unit"": ""A"", ""pilot"": ""a"", ""x"": 2, ""y"": 3 }],
  ""enemies"": [{ ""unit"": ""B"", ""pilot"": ""b"", ""x"": 8, ""y"": 4 }],
  ""events"": [{ ""trigger"": ""destroyed"", ""unit"": ""b"", ""enemies"": [{ ""unit"": ""B"", ""pilot"": ""b2"", ""x"": 8, ""y"": 4 }] }]
}");
            Assert.AreEqual(0, QuickBattleTemplate.Validate(reinforce).Count, string.Join("\n", QuickBattleTemplate.Validate(reinforce)));
        }

        /// <summary>名前のあるパイロットは 1 人だけ（2 度目の Create はエラー）。落ちたあとに別の機体で出直すときは、乗り換えて出す</summary>
        [TestMethod]
        public void Reinforcement_NamedPilotReturnsInANewUnitByRiding()
        {
            var battle = QuickBattleTemplate.FromJson(@"{
  ""allies"": [{ ""unit"": ""A"", ""pilot"": ""a"", ""x"": 2, ""y"": 3 }],
  ""enemies"": [{ ""unit"": ""ジオング"", ""pilot"": ""シャア"", ""x"": 8, ""y"": 4 }, { ""unit"": ""B"", ""pilot"": ""兵(ザコ)"", ""x"": 8, ""y"": 6 }],
  ""events"": [
    { ""trigger"": ""destroyed"", ""unit"": ""シャア"", ""enemies"": [{ ""unit"": ""ジオングヘッド"", ""pilot"": ""シャア"", ""bossRank"": 0, ""x"": 8, ""y"": 4 }] },
    { ""trigger"": ""turn"", ""turn"": 2, ""enemies"": [{ ""unit"": ""B"", ""pilot"": ""兵(ザコ)"", ""x"": 9, ""y"": 6 }] }
  ]
}");
            Assert.AreEqual(0, QuickBattleTemplate.Validate(battle).Count, string.Join("\n", QuickBattleTemplate.Validate(battle)));
            var eve = QuickBattleTemplate.Files(battle).Values.First(x => x.Contains("スタート:")).Split("\r\n");
            var i = System.Array.IndexOf(eve, "破壊 シャア:");
            CollectionAssert.AreEqual(new[]
            {
                "# シャアは落ちたユニットから、新しい機体に乗り換えて出る",
                "Unit ジオングヘッド 0", "Ride シャア", "ChangeParty シャア 敵", "BossRank 0", "Launch シャア 8 4", "Exit",
            }, eve.Skip(i + 1).Take(7).ToArray());
            // ザコは何人でも Create できる
            CollectionAssert.Contains(eve, "Create 敵 B 0 \"兵(ザコ)\" 1 9 6");

            // まだ乗っている名前のあるパイロットは、2 機目には乗せられない
            battle.Events[0].Trigger = "turn";
            battle.Enemies.Add(new QuickUnit { Unit = "C", Pilot = "シャア", X = 5, Y = 5 });
            var errors = QuickBattleTemplate.Validate(battle).ToList();
            CollectionAssert.Contains(errors, "「シャア」は名前のあるパイロットなので、2 機には乗せられません（ザコ・汎用のパイロットなら何機でも置けます）");
            CollectionAssert.Contains(errors, "出来事 1: 「シャア」はもう出ています。名前のあるパイロットが別の機体で出直せるのは、そのパイロットが落ちたときの出来事だけです");
        }

        [TestMethod]
        public void FileName_DropsCharactersFilesCannotHave()
        {
            Assert.AreEqual("決戦 前夜", QuickBattleTemplate.FileName(new QuickBattle { Title = " 決戦/ 前夜?. " }));
            Assert.AreEqual("かんたん作成", QuickBattleTemplate.FileName(new QuickBattle { Title = "  " }));
        }
    }
}
