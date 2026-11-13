using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Linting;
using SRCAuthoring.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace SRCAuthoring.Tests
{
    /// <summary>規則の仕組み（グループで選ぶ・ID と場所の付いた診断・終了コード・SARIF）（7474/SRC#1610）</summary>
    [TestClass]
    public class DataRuleFrameworkTests
    {
        /// <summary>HP が 10000 を超えるユニットを知らせる、テスト用の規則</summary>
        private sealed class HugeHpRule : DataRule
        {
            private readonly DiagnosticSeverity severity;
            private readonly string[] groups;

            public HugeHpRule(DiagnosticSeverity severity = DiagnosticSeverity.Information, params string[] groups)
            {
                this.severity = severity;
                this.groups = groups.Length == 0 ? new[] { "test" } : groups;
            }

            public override string Id => "test-huge-hp";
            public override IReadOnlyList<string> Groups => groups;
            public override DiagnosticSeverity Severity => severity;
            public override string Description => "HP が 10000 を超える";
            public override string Origin => "テスト";

            public override IEnumerable<SrcDiagnostic> Check(DataRuleContext context)
            {
                return context.Units
                    .Where(x => x.HP > 10000)
                    .Select(x => Report(context, x.Location, x.Location.FieldLine("最大ＨＰ"), $"HP {x.HP} が 10000 を超えています", x.Name));
            }
        }

        private static string[] Unit(string name, int hp)
        {
            return new[]
            {
                name,
                $"{name}, {name}, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                $"{hp}, 100, 600, 60",
                "AAAA, -.bmp",
                "",
            };
        }

        private static LintResult Lint(TestScenario scenario, IReadOnlyList<string> groups, IDataRule rule, IEnumerable<string> systemFiles = null)
        {
            return new DataLinter { RuleGroups = groups, AvailableRules = new[] { rule } }
                .LintAsync(new[] { Path.Combine(scenario.Root, "Data") }, scenario.Root, systemFiles).Result;
        }

        [TestMethod]
        public void RuleReportsIdFileAndFieldLine()
        {
            using (var scenario = new TestScenario())
            {
                var robot = scenario.Write(Path.Combine("Data", "robot.txt"), Unit("普通機", 3000).Concat(Unit("巨大機", 30000)).ToArray());

                var result = Lint(scenario, new[] { "test" }, new HugeHpRule());

                var d = result.Diagnostics.Single(x => x.RuleId == "test-huge-hp");
                Assert.AreEqual(Path.GetFullPath(robot), d.File);
                Assert.AreEqual(12, d.Line);
                Assert.AreEqual("巨大機", d.Source);
                Assert.AreEqual(DiagnosticSeverity.Information, d.Severity);
                Assert.AreEqual($"{robot}(12): info: HP 30000 が 10000 を超えています[巨大機] (test-huge-hp)", d.ToString());
                // 「情報」は問題に数えない
                Assert.IsFalse(result.HasError, string.Join("\n", result.Diagnostics));
            }
        }

        [TestMethod]
        public void RuleRunsOnlyWhenItsGroupIsSelected()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"), Unit("巨大機", 30000));
                var rule = new HugeHpRule(DiagnosticSeverity.Information, "test", "other");

                Assert.IsFalse(Lint(scenario, RuleGroups.Default, rule).Diagnostics.Any());
                Assert.IsFalse(Lint(scenario, Array.Empty<string>(), rule).Diagnostics.Any());
                Assert.AreEqual(1, Lint(scenario, new[] { "other" }, rule).Diagnostics.Count);
                // 複数のグループに入る規則も、1 回だけ動く
                Assert.AreEqual(1, Lint(scenario, new[] { "test", "other" }, rule).Diagnostics.Count);
            }
        }

        [TestMethod]
        public void WarningRuleCountsAsProblem()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"), Unit("巨大機", 30000));

                Assert.IsTrue(Lint(scenario, new[] { "test" }, new HugeHpRule(DiagnosticSeverity.Warning)).HasError);
            }
        }

        [TestMethod]
        public void SystemFilesAreNotChecked()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"), Unit("普通機", 3000));
                var system = scenario.Write(Path.Combine("System", "robot.txt"), Unit("汎用の巨大機", 30000));

                var result = Lint(scenario, new[] { "test" }, new HugeHpRule(), new[] { system });

                Assert.IsFalse(result.Diagnostics.Any(), string.Join("\n", result.Diagnostics));
            }
        }

        [TestMethod]
        public void SarifHasRuleWithGroupsAsTags()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"), Unit("巨大機", 30000));
                var result = Lint(scenario, new[] { "test" }, new HugeHpRule());

                var sarif = JsonNode.Parse(SarifWriter.Write(result.Diagnostics, scenario.Root));

                var res = sarif["runs"][0]["results"][0];
                Assert.AreEqual("test-huge-hp", (string)res["ruleId"]);
                Assert.AreEqual("note", (string)res["level"]);
                Assert.AreEqual("Data/robot.txt", (string)res["locations"][0]["physicalLocation"]["artifactLocation"]["uri"]);
                var ids = sarif["runs"][0]["tool"]["driver"]["rules"].AsArray().Select(x => (string)x["id"]).ToList();
                CollectionAssert.Contains(ids, "test-huge-hp");
            }
        }

        [TestMethod]
        public void ParseGroups()
        {
            CollectionAssert.AreEqual(new[] { "src" }, RuleGroups.Parse(null).ToArray());
            CollectionAssert.AreEqual(new[] { "src", "robodun" }, RuleGroups.Parse("src, ROBODUN,src").ToArray());
            Assert.AreEqual(0, RuleGroups.Parse("none").Count);
            Assert.ThrowsExactly<ArgumentException>(() => RuleGroups.Parse("src,gsc"));
        }

        [TestMethod]
        public void BuiltInRulesHaveValidIdsAndGroups()
        {
            foreach (var rule in DataRuleCatalog.All)
            {
                StringAssert.Matches(rule.Id, new System.Text.RegularExpressions.Regex("^[a-z0-9]+(-[a-z0-9]+)*$"), rule.Id);
                Assert.IsTrue(rule.Groups.Count > 0, rule.Id);
                foreach (var g in rule.Groups)
                {
                    CollectionAssert.Contains(RuleGroups.All.ToList(), g, rule.Id);
                }
                // 作品の観点の規則は、グループ名で始める
                if (rule.Groups.Contains(RuleGroups.Robodun) && !rule.Groups.Contains(RuleGroups.Src))
                {
                    StringAssert.StartsWith(rule.Id, "robodun-");
                }
                Assert.IsFalse(string.IsNullOrEmpty(rule.Description), rule.Id);
                Assert.IsFalse(string.IsNullOrEmpty(rule.Origin), rule.Id);
            }
            Assert.AreEqual(DataRuleCatalog.All.Count, DataRuleCatalog.All.Select(x => x.Id).Distinct().Count());
        }
    }
}
