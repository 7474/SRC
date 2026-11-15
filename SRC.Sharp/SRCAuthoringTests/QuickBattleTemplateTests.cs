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

        [TestMethod]
        public void FileName_DropsCharactersFilesCannotHave()
        {
            Assert.AreEqual("決戦 前夜", QuickBattleTemplate.FileName(new QuickBattle { Title = " 決戦/ 前夜?. " }));
            Assert.AreEqual("かんたん作成", QuickBattleTemplate.FileName(new QuickBattle { Title = "  " }));
        }
    }
}
