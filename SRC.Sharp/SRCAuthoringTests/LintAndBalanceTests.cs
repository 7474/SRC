using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Balance;
using SRCAuthoring.Linting;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class LintAndBalanceTests
    {
        private static string[] Unit(string name, string cls, int hp, int armor, int power)
        {
            return new[]
            {
                name,
                $"{name}, {name}, {cls}, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                $"{hp}, 100, {armor}, 60",
                "AAAA, -.bmp",
                $"ビーム, {power}, 1, 3, +0, -, -, -, AAAA, +0, Ｂ",
                "===",
                "",
            };
        }

        [TestMethod]
        public void Balance_FindsOutlierWithinClass()
        {
            using (var scenario = new TestScenario())
            {
                var lines = Enumerable.Range(1, 6).SelectMany(i => Unit("量産機" + i, "汎用", 3000, 600, 1500))
                    .Concat(Unit("強すぎる機体", "汎用", 30000, 600, 1500))
                    .ToArray();
                scenario.Write(Path.Combine("Data", "robot.txt"), lines);

                var report = BalanceReport.Create(new[] { Path.Combine(scenario.Root, "Data") });

                Assert.AreEqual(7, report.Units.Count);
                Assert.AreEqual(7, report.Weapons.Count);
                var outlier = report.Outliers.Single();
                Assert.AreEqual("強すぎる機体", outlier.Unit);
                Assert.AreEqual("HP", outlier.Item);
                StringAssert.Contains(report.ToMarkdown(), "| 強すぎる機体 | 汎用 | HP | 30000 |");
                StringAssert.StartsWith(report.UnitsCsv(), "unit,class,hp");
            }
        }

        [TestMethod]
        public void Lint_ReportsEventErrorsAsSarif()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("test.eve", "スタート:", "If 1 = 1 Then", "Exit");

                var result = new DataLinter().LintAsync(new[] { eve }, scenario.Root).Result;

                Assert.IsTrue(result.HasError);
                var diagnostic = result.Diagnostics.First();
                Assert.IsTrue(Analysis.ScenarioAnalysis.SamePath(eve, diagnostic.File), diagnostic.ToString());
                var sarif = JsonNode.Parse(SarifWriter.Write(result.Diagnostics, scenario.Root));
                Assert.AreEqual("2.1.0", sarif["version"].GetValue<string>());
                var location = sarif["runs"][0]["results"][0]["locations"][0]["physicalLocation"];
                Assert.AreEqual("test.eve", location["artifactLocation"]["uri"].GetValue<string>());
                Assert.IsTrue(location["region"]["startLine"].GetValue<int>() >= 1);
            }
        }
    }
}
