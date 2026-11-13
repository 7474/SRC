using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Linting;
using SRCAuthoring.Rules;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>組み込みの規則（src・gsc-robo・gsc-ext・robodun）（7474/SRC#1611・#1612・#1619）</summary>
    [TestClass]
    public class DataRulesTests
    {
        private static string[] Unit(string name, string trans = "陸", string adaption = "AAAA", string stats = "3000, 100, 600, 60",
            int exp = 100, string nickname = null, params string[] weapons)
        {
            var lines = new List<string>
            {
                name,
                $"{nickname ?? name}, よみ, 汎用, 1, 2",
                $"{trans}, 4, M, 1000, {exp}",
                "特殊能力なし",
                stats,
                $"{adaption}, -.bmp",
            };
            if (weapons.Length > 0)
            {
                lines.AddRange(weapons);
            }
            lines.Add("");
            return lines.ToArray();
        }

        private static string Weapon(string name, int maxRange, string cls, string adaption = "AAAA", int power = 1000, string en = "-")
            => $"{name}, {power}, 1, {maxRange}, +0, -, {en}, -, {adaption}, +0, {cls}";

        private static string[] Pilot(string name, string stats = "100, 100, 100, 100, 100, 100, 普通", string sp = "ＳＰなし", int exp = 100, string nickname = null)
            => new[] { name, $"{nickname ?? name}, よみ, 男性, 汎用, AAAA, {exp}", "特殊能力なし", stats, sp, "-.bmp, -.mid", "" };

        private static List<SrcDiagnostic> Lint(string groups, string[] robot = null, string[] pilot = null, string[] message = null, string[] sp = null)
        {
            using (var scenario = new TestScenario())
            {
                if (robot != null)
                {
                    scenario.Write(Path.Combine("Data", "robot.txt"), robot);
                }
                if (pilot != null)
                {
                    scenario.Write(Path.Combine("Data", "pilot.txt"), pilot);
                }
                if (message != null)
                {
                    scenario.Write(Path.Combine("Data", "pilot_message.txt"), message);
                }
                var system = new List<string>();
                if (sp != null)
                {
                    system.Add(scenario.Write(Path.Combine("System", "sp.txt"), sp));
                }
                var result = new DataLinter { RuleGroups = RuleGroups.Parse(groups) }
                    .LintAsync(new[] { Path.Combine(scenario.Root, "Data") }, scenario.Root, system).Result;
                return result.Diagnostics.Where(x => x.RuleId != null).ToList();
            }
        }

        private static string[] Ids(IEnumerable<SrcDiagnostic> diagnostics) => diagnostics.Select(x => x.RuleId).ToArray();

        /// <summary>スペシャルパワーの定義（名前だけ使う）</summary>
        private static string[] SpData(params string[] names)
            => names.SelectMany(n => new[] { $"{n}, よみ", $"{n.Substring(0, 1)}, 10, 自分, 即効, -, -, -", "気力増加Lv1", "説明", "" }).ToArray();

        [TestMethod]
        public void Src_TransportationInvalid_SeaIsNotWater()
        {
            var d = Lint("src", Unit("海の機体", trans: "陸海").Concat(Unit("陸地中機", trans: "陸地中")).ToArray());

            var x = d.Single(y => y.RuleId == "unit-transportation-invalid");
            Assert.AreEqual("海の機体", x.Source);
            Assert.AreEqual(3, x.Line);
            StringAssert.Contains(x.Message, "「海」");
            Assert.AreEqual(DiagnosticSeverity.Information, x.Severity);
        }

        [TestMethod]
        public void Src_TransportationAdaption()
        {
            var d = Lint("src", Unit("飛べない機体", trans: "空陸", adaption: "-AAA").Concat(Unit("普通", trans: "空陸", adaption: "AAAA")).ToArray());

            var x = d.Single(y => y.RuleId == "unit-transportation-adaption");
            Assert.AreEqual("飛べない機体", x.Source);
            Assert.AreEqual(6, x.Line);
        }

        [TestMethod]
        public void Src_SpecialPowerDuplicate()
        {
            var d = Lint("src", pilot: Pilot("重複", sp: "ＳＰ, 30, 集中, 1, 集中, 5"), sp: SpData("集中"));

            Assert.AreEqual(5, d.Single(y => y.RuleId == "pilot-sp-duplicate").Line);
        }

        [TestMethod]
        public void Src_MessageSituationUnknown()
        {
            var d = Lint("src",
                robot: Unit("機体", weapons: new[] { Weapon("ビーム", 3, "Ｂ") }),
                message: new[] { "テスト", "回避, よけた", "ビーム, 撃つ", "ビーム(反撃), 反撃", "攻撃(対瀕死), とどめ", "ビームー, 打ち間違い", "攻撃（反撃）, 全角の括弧", "" });

            CollectionAssert.AreEqual(new[] { 6, 7 }, d.Where(y => y.RuleId == "message-situation-unknown").Select(x => x.Line).ToArray(), string.Join("\n", d));
        }

        [TestMethod]
        public void Src_MessageSituationUnknown_SkippedWithoutUnitData()
        {
            var d = Lint("src", message: new[] { "テスト", "ビーム, 撃つ", "" });

            Assert.AreEqual(0, d.Count, string.Join("\n", d));
        }

        [TestMethod]
        public void Src_DoesNotRunOtherGroups()
        {
            var d = Lint("src", Unit("機体", weapons: new[] { Weapon("パンチ", 1, "格突"), Weapon("ミサイル", 5, "射") }));

            Assert.AreEqual(0, d.Count, string.Join("\n", d));
        }

        [TestMethod]
        public void Gsc_WeaponRules()
        {
            var robot = Unit("機体", weapons: new[]
            {
                Weapon("パンチ", 1, "格突"),          // 7 weapon-range1-infight
                Weapon("ミサイル", 5, "射"),          // 8 weapon-range2-shooting
                Weapon("キック", 1, "Ｐ"),            // 9 weapon-range1-p
                Weapon("ジャンプ斬り", 2, "武"),      // 10 weapon-cross-range-shooting
                Weapon("ビーム", 3, "Ｂ", "AAAA"),    // 11 weapon-beam-adaption（ロボだけ）
                Weapon("ビームサーベル", 1, "Ｂ", "AA-A"), // 12 weapon-beam-melee（ロボだけ）
                Weapon("正しいビーム", 3, "Ｂ", "AA-A"),
                Weapon("マップ兵器", 3, "射Ｍ直"),    // マップ兵器は除く
            });

            var robo = Lint("gsc-robo", robot);
            CollectionAssert.AreEquivalent(new[] { "weapon-range1-infight", "weapon-range2-shooting", "weapon-range1-p", "weapon-cross-range-shooting", "weapon-beam-adaption", "weapon-beam-melee" },
                Ids(robo.Where(x => x.RuleId.StartsWith("weapon-"))), string.Join("\n", robo));
            Assert.AreEqual(7, robo.Single(x => x.RuleId == "weapon-range1-infight").Line);
            Assert.AreEqual(DiagnosticSeverity.Warning, robo.Single(x => x.RuleId == "weapon-range1-infight").Severity);

            var ext = Lint("gsc-ext", robot);
            Assert.IsFalse(ext.Any(x => x.RuleId.StartsWith("weapon-beam")), string.Join("\n", ext));
        }

        [TestMethod]
        public void Gsc_UnitAirAdaptionNonflyer_ExtOnly()
        {
            var robot = Unit("歩く人", trans: "陸", adaption: "-AAA");

            Assert.IsTrue(Lint("gsc-ext", robot).Any(x => x.RuleId == "unit-air-adaption-nonflyer"));
            Assert.IsFalse(Lint("gsc-robo", robot).Any(x => x.RuleId == "unit-air-adaption-nonflyer"));
        }

        [TestMethod]
        public void Gsc_NameLengthAndRounding()
        {
            var d = Lint("gsc-robo",
                robot: Unit("機体", stats: "3050, 105, 620, 62", nickname: "とてもとても長いユニットの愛称です", weapons: new[] { Weapon("とてもとても長い武器の名前です", 1, "突", power: 1050, en: "7") }),
                pilot: Pilot("長い", nickname: "とても長いパイロットの愛称"));

            Assert.AreEqual(3, d.Count(x => x.RuleId == "name-length"), string.Join("\n", d));
            Assert.AreEqual(6, d.Count(x => x.RuleId == "number-rounding"), string.Join("\n", d));
        }

        [TestMethod]
        public void Gsc_PilotSpecialPowerRules()
        {
            var sps = SpData("集中", "必中", "熱血", "気合", "ひらめき", "魂");
            var d = Lint("gsc-ext",
                pilot: Pilot("五つ", sp: "ＳＰ, 30, 集中, 1, 必中, 5, 熱血, 10, 気合, 15, ひらめき, 20")
                    .Concat(Pilot("六つ", sp: "ＳＰ, 30, 集中, 1, 必中, 3, 熱血, 8, 気合, 14, ひらめき, 22, 魂, 31"))
                .Concat(Pilot("等間隔", sp: "ＳＰ, 30, 集中, 1, 必中, 4, 熱血, 7, 気合, 10, ひらめき, 13, 魂, 16"))
                    .Concat(Pilot("ザコ(ザコ)", sp: "ＳＰ, 30, 集中, 1"))
                    .ToArray(),
                message: new[] { "六つ", "熱血, 燃えろ", "自爆, さらば", "" },
                sp: sps.Concat(SpData("自爆")).ToArray());

            Assert.AreEqual("五つ", d.Single(x => x.RuleId == "pilot-sp-count").Source, string.Join("\n", d));
            // 五つ: レベル 1 以外（5・10・15・20）がすべて 5 の倍数。間隔（4・5・5・5）は一定でない
            Assert.AreEqual(1, d.Count(x => x.RuleId == "pilot-sp-level-pattern" && x.Source == "五つ"), string.Join("\n", d));
            Assert.IsFalse(d.Any(x => x.RuleId == "pilot-sp-level-pattern" && x.Source == "六つ"), string.Join("\n", d));
            StringAssert.Contains(d.Single(x => x.RuleId == "pilot-sp-level-pattern" && x.Source == "等間隔").Message, "間隔が、すべて 3");
            Assert.AreEqual(2, d.Single(x => x.RuleId == "message-sp-situation").Line);
        }

        [TestMethod]
        public void Robodun_PilotAndUnitLimits()
        {
            var d = Lint("robodun",
                robot: Unit("強い機体", stats: "16000, 310, 2600, 145", exp: 201),
                pilot: Pilot("強い人", stats: "170, 140, 166, 100, 191, 171, 普通", exp: 210));

            CollectionAssert.AreEquivalent(new[]
            {
                "robodun-pilot-exp",
                "robodun-pilot-stat", "robodun-pilot-stat", "robodun-pilot-stat", "robodun-pilot-stat",
                "robodun-pilot-attack-sum",
                "robodun-unit-exp",
                "robodun-unit-stat", "robodun-unit-stat", "robodun-unit-stat", "robodun-unit-stat",
            }, Ids(d), string.Join("\n", d));
            Assert.IsTrue(d.All(x => x.Severity == DiagnosticSeverity.Information));
            Assert.AreEqual(4, d.First(x => x.RuleId == "robodun-pilot-stat").Line);
        }

        [TestMethod]
        public void Robodun_SpecialPowerCombos()
        {
            var d = Lint("robodun",
                pilot: Pilot("火力", sp: "ＳＰ, 30, 熱血, 1, 痛撃, 5")
                    .Concat(Pilot("無限", sp: "ＳＰ, 30, 夢, 1, 祈り, 5"))
                    .Concat(Pilot("強い", sp: "ＳＰ, 30, 捨て身, 1, 忍耐, 5, 復活, 10"))
                    .ToArray(),
                sp: SpData("熱血", "痛撃", "夢", "祈り", "捨て身", "忍耐", "復活"));

            // 夢は 2 倍とクリティカルの両方に数える。捨て身（3 倍）はクリティカルの SP が無いので当たらない
            CollectionAssert.AreEquivalent(new[] { "火力", "無限" }, d.Where(x => x.RuleId == "robodun-sp-firepower").Select(x => x.Source).ToArray(), string.Join("\n", d));
            Assert.AreEqual("無限", d.Single(x => x.RuleId == "robodun-sp-infinite").Source);
            Assert.AreEqual(2, d.Count(x => x.RuleId == "robodun-sp-combo" && x.Source == "強い"));
        }

        [TestMethod]
        public void Robodun_WeaponRulesSkipExtendedAttack()
        {
            var robot = Unit("機体", trans: "陸海", weapons: new[]
            {
                Weapon("パンチ", 1, "格突"),
                Weapon("伸びるパンチ", 1, "格突延"),
                Weapon("ミサイル", 5, "射"),
                Weapon("ジャンプ斬り", 2, "武"),
                Weapon("ビームサーベル", 1, "Ｂ", "AAAA"),
            });

            var d = Lint("robodun", robot);

            CollectionAssert.AreEquivalent(new[]
            {
                "unit-transportation-invalid",
                "robodun-weapon-range1-infight",
                "robodun-weapon-range2-shooting",
                "robodun-weapon-cross-range",
                "robodun-weapon-beam-water",
            }, Ids(d), string.Join("\n", d));
            Assert.AreEqual("機体", d.Single(x => x.RuleId == "robodun-weapon-range1-infight").Source);
            StringAssert.Contains(d.Single(x => x.RuleId == "robodun-weapon-range1-infight").Message, "：パンチ");

            // GSC の規則は「延」を除かない
            Assert.AreEqual(2, Lint("gsc-robo", robot).Count(x => x.RuleId == "weapon-range1-infight"));
        }

        /// <summary>
        /// srcsharp lint（DataLinter）と Language Server（LintDataAsync）が、同じデータに同じ規則の診断を出す（7474/SRC#1615）。
        /// ブラウザの lint（SRCLintWeb）も同じ DataLinter で、SRCSharpWeb.E2E の lint.spec.js が同じデータの ID と行を確かめる
        /// </summary>
        [TestMethod]
        public void CliAndLanguageServerReportSameRuleDiagnostics()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"), Unit("海の機体", trans: "陸海", adaption: "-AAA",
                    weapons: new[] { Weapon("パンチ", 1, "格突"), Weapon("ミサイル", 5, "射") }));
                scenario.Write(Path.Combine("Data", "pilot.txt"), Pilot("強い人", stats: "170, 140, 100, 100, 100, 100, 普通"));
                var groups = RuleGroups.Parse("src,gsc-ext,robodun");
                string Key(SrcDiagnostic d) => $"{d.RuleId} {Path.GetFileName(d.File)}:{d.Line} {d.Severity} {d.Message}";

                var cli = new DataLinter { RuleGroups = groups }.LintAsync(new[] { Path.Combine(scenario.Root, "Data") }, scenario.Root).Result;
                var lsp = new SRCAuthoring.Lsp.LspServer(new MemoryStream(), new MemoryStream()) { RuleGroups = groups }
                    .LintDataAsync(Path.Combine(scenario.Root, "Data")).Result;

                var expected = cli.Diagnostics.Where(x => x.RuleId != null).Select(Key).OrderBy(x => x).ToList();
                Assert.IsTrue(expected.Count >= 6, string.Join("\n", expected));
                CollectionAssert.AreEqual(expected, lsp.Diagnostics.Where(x => x.RuleId != null).Select(Key).OrderBy(x => x).ToList());
            }
        }

        [TestMethod]
        public void Robodun_AndSrcTogetherRunSharedRuleOnce()
        {
            var d = Lint("src,robodun", Unit("海の機体", trans: "陸海"));

            Assert.AreEqual(1, d.Count(x => x.RuleId == "unit-transportation-invalid"));
        }
    }
}
