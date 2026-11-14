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
            CollectionAssert.AreEqual(new[] { "集中（Lv1）" }, p["specialPowers"].AsArray().Select(x => x.GetValue<string>()).ToArray());
            Assert.AreEqual(0, json["errors"].AsArray().Count, json.ToJsonString());
        }

        [TestMethod]
        public void Errors_HaveLineAndMessage()
        {
            var json = JsonNode.Parse(DataPreview.ToJson("robot.txt", "壊れ機\n壊れ機, こわれき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\nたくさん, 100, 600, 60\nAAAA, -.bmp\n===\n"));
            var errors = json["errors"].AsArray().Select(x => x.GetValue<string>()).ToList();
            Assert.IsTrue(errors.Count > 0);
            StringAssert.Contains(errors[0], "行目");
        }
    }
}
