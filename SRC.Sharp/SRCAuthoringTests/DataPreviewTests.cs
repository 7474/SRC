using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Preview;
using System.Linq;
using System.Text.Json.Nodes;

namespace SRCAuthoring.Tests
{
    /// <summary>データのプレビュー（SRC#Studio。7474/SRC#1643）</summary>
    [TestClass]
    public class DataPreviewTests
    {
        private const string Robot = "# コメント\n試作機\n試作機, しさくき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力\nバリアLv1=Ｂ, ＨＰ回復Lv2\n3000, 100, 600, 60\nAAAA, OBJ_Robot01.bmp\nパンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突\nライフル, 1200, 2, 4, +10, 8, -, -, AAAA, +5, 実\n===\n\n量産機\n量産機, りょうさんき, 汎用, 1, 2\n陸, 4, M, 800, 80\n特殊能力なし\n2000, 80, 500, 50\nAAAA, -.bmp\nキック, 900, 1, 1, +0, -, -, -, AAAA, +0, 突\n===\n";

        [TestMethod]
        public void Units_WithRangesStatsFeaturesAndWeapons()
        {
            var json = JsonNode.Parse(DataPreview.ToJson("robot.txt", Robot));
            Assert.AreEqual("unit", json["kind"].GetValue<string>());
            var entries = json["entries"].AsArray();
            Assert.AreEqual(2, entries.Count);
            var u = entries[0];
            Assert.AreEqual("試作機", u["name"].GetValue<string>());
            Assert.AreEqual(2, u["startLine"].GetValue<int>());
            Assert.AreEqual(12, u["endLine"].GetValue<int>());
            Assert.AreEqual(13, entries[1]["startLine"].GetValue<int>());
            Assert.AreEqual(3000, u["hp"].GetValue<int>());
            Assert.AreEqual("OBJ_Robot01.bmp", u["bitmap"].GetValue<string>());
            Assert.AreEqual(4, u["statLines"]["move"].GetValue<int>());
            Assert.AreEqual(7, u["statLines"]["hp"].GetValue<int>());
            Assert.AreEqual(8, u["statLines"]["adaption"].GetValue<int>());
            CollectionAssert.AreEqual(new[] { "バリアLv1=Ｂ", "ＨＰ回復Lv2" }, u["features"].AsArray().Select(x => x.GetValue<string>()).ToArray());
            var rifle = u["weapons"][1];
            Assert.AreEqual("ライフル", rifle["name"].GetValue<string>());
            Assert.AreEqual("2-4", rifle["range"].GetValue<string>());
            Assert.AreEqual(8, rifle["bullet"].GetValue<int>());
            Assert.AreEqual(0, json["errors"].AsArray().Count, json.ToJsonString());
        }

        [TestMethod]
        public void Pilots_ResolveSpecialPowersFromSystemData()
        {
            var sp = "集中, しゅうちゅう\n集, 15, 自分, ターン, -, -, 集中\n命中強化Lv3 回避強化Lv3\n１ターンの間、命中＆回避+30\n";
            var pilot = "主人公\n主人公, しゅじんこう, 男性, 汎用, AAAA, 120\n特殊能力なし\n150, 145, 150, 150, 150, 155, 強気\nＳＰ, 40, 集中, 1\n-.bmp, -.mid\n";
            var json = JsonNode.Parse(DataPreview.ToJson("pilot.txt", pilot, new[] { ("sp.txt", sp) }));
            Assert.AreEqual("pilot", json["kind"].GetValue<string>());
            var p = json["entries"][0];
            Assert.AreEqual(145, p["stats"]["射撃"].GetValue<int>());
            Assert.AreEqual(2, p["statLines"]["adaption"].GetValue<int>());
            Assert.AreEqual(4, p["statLines"]["stats"].GetValue<int>());
            CollectionAssert.AreEqual(new[] { "集中（Lv1）" }, p["specialPowers"].AsArray().Select(x => x.GetValue<string>()).ToArray());
            Assert.AreEqual(0, json["errors"].AsArray().Count, json.ToJsonString());
        }

        [TestMethod]
        public void Pilots_StatLinesRecordHeaderAndStatsWithShortHeaders()
        {
            var pilot = "省略例\n省略例, 汎用, SAAA, 150\n特殊能力\n底力, 1\n160, 155, 150, 145, 140, 165, 普通\nＳＰ, 0\n-.bmp, -.mid\n";
            var json = JsonNode.Parse(DataPreview.ToJson("pilot.txt", pilot));
            Assert.AreEqual(0, json["errors"].AsArray().Count, json.ToJsonString());
            var p = json["entries"][0];
            Assert.AreEqual(2, p["statLines"]["adaption"].GetValue<int>());
            Assert.AreEqual(5, p["statLines"]["stats"].GetValue<int>());
        }

        [TestMethod]
        public void Errors_HaveLineAndMessage()
        {
            var json = JsonNode.Parse(DataPreview.ToJson("robot.txt", "壊れ機\n壊れ機, こわれき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\nたくさん, 100, 600, 60\nAAAA, -.bmp\n===\n"));
            var errors = json["errors"].AsArray().Select(x => x.GetValue<string>()).ToList();
            Assert.IsTrue(errors.Count > 0);
            StringAssert.Contains(errors[0], "行目");
        }

        [TestMethod]
        public void Messages_CoverageWeaponsAndMissingPilots()
        {
            var pilot = "主人公\n主人公, しゅじんこう, 男性, 汎用, AAAA, 120\n特殊能力なし\n150, 145, 150, 150, 150, 155, 普通\nＳＰ, 0\nHero.bmp, -.mid\n\n未定義人\n未定義人, み, 男性, 汎用, AAAA, 100\n特殊能力なし\n100, 100, 100, 100, 100, 100, 普通\nＳＰ, 0\n-.bmp, -.mid\n";
            var robot = "試作機\n試作機, しさくき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\n3000, 100, 600, 60\nAAAA, -.bmp\nパンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突\nビーム(強), 1500, 2, 4, +10, 8, -, -, AAAA, +5, Ｂ\n===\n";
            var eve = "プロローグ:\nCreate 味方 試作機 0 主人公 1 2 2\nExit\n";
            var msg = "主人公\n回避, かわした\nビーム, 発射！\n\n未書込\n回避, テスト\n";
            var json = JsonNode.Parse(DataPreview.ToJson("Data/pilot_message.txt", msg, new[]
            {
                ("Data/pilot.txt", pilot),
                ("Data/robot.txt", robot),
                ("01.eve", eve),
            }));
            Assert.AreEqual("message", json["kind"].GetValue<string>());
            Assert.IsTrue(json["standardSituations"].AsArray().Count >= 5);
            var entries = json["entries"].AsArray();
            Assert.AreEqual(2, entries.Count);
            var hero = entries[0];
            Assert.AreEqual("主人公", hero["name"].GetValue<string>());
            Assert.AreEqual("Hero.bmp", hero["bitmap"].GetValue<string>());
            CollectionAssert.AreEqual(new[] { "試作機" }, hero["units"].AsArray().Select(x => x.GetValue<string>()).ToArray());
            CollectionAssert.AreEqual(new[] { "パンチ", "ビーム(強)" }, hero["weapons"].AsArray().Select(w => w["name"].GetValue<string>()).ToArray());
            var beam = hero["coverage"].AsArray().First(w => w["kind"].GetValue<string>() == "weapon" && w["label"].GetValue<string>() == "ビーム(強)");
            Assert.AreEqual(1, beam["count"].GetValue<int>(), "カッコ抜きの名前「ビーム」も武器のカバレッジに一致する");
            var punch = hero["coverage"].AsArray().First(w => w["kind"].GetValue<string>() == "weapon" && w["label"].GetValue<string>() == "パンチ");
            Assert.AreEqual(0, punch["count"].GetValue<int>());

            var missing = json["missingPilots"].AsArray().Select(m => m["name"].GetValue<string>()).ToList();
            CollectionAssert.Contains(missing, "未定義人");
            CollectionAssert.DoesNotContain(missing, "主人公");
        }

        [TestMethod]
        public void Dialogs_SpeakerBitmapsFromPilotAndNonPilot()
        {
            var pilot = "主人公\n主人公, しゅじんこう, 男性, 汎用, AAAA, 120\n特殊能力なし\n150, 145, 150, 150, 150, 155, 普通\nＳＰ, 0\nHero.bmp, -.mid\n";
            var nonPilot = "通信士\n通信士, Comms.bmp\n";
            var dialog = "主人公\n攻撃\n主人公, 行くぞ！\n通信士, 了解！\n";
            var json = JsonNode.Parse(DataPreview.ToJson("Data/pilot_dialog.txt", dialog, new[]
            {
                ("Data/pilot.txt", pilot),
                ("Data/non_pilot.txt", nonPilot),
            }));
            Assert.AreEqual("dialog", json["kind"].GetValue<string>());
            var speakers = json["entries"][0]["items"][0]["messages"].AsArray();
            Assert.AreEqual("主人公", speakers[0]["speaker"].GetValue<string>());
            Assert.AreEqual("Hero.bmp", speakers[0]["bitmap"].GetValue<string>());
            Assert.AreEqual("通信士", speakers[1]["speaker"].GetValue<string>());
            Assert.AreEqual("Comms.bmp", speakers[1]["bitmap"].GetValue<string>());
        }
    }
}
