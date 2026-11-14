using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using SRCAuthoring.Linting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    /// <summary>イベントのコマンドが名前で指すデータの検査（srcsharp lint。7474/SRC#1642）</summary>
    [TestClass]
    public class DataReferenceTests
    {
        [TestMethod]
        public void Find_SkipsExpressionsVariablesAndComments()
        {
            var refs = DataReferenceChecker.Find("a.eve", string.Join("\n",
                "# Create 味方 コメント 0 コメント 1 1 1",
                "Set 機体 試作機",
                "Create 味方 \"試作機\" 0 主人公 1 1 1",
                "Create 味方 機体 0 $(名前) 1 1 1",
                "Unit 試作機 0",
                "Pilot 主人公 1",
                "Item 薬草"));
            CollectionAssert.AreEqual(
                new[] { "Unit:試作機", "Pilot:主人公", "Unit:試作機", "Pilot:主人公", "Item:薬草" },
                refs.Select(r => $"{r.Kind}:{r.Name}").ToArray());
            Assert.AreEqual(3, refs[0].Line);
            Assert.AreEqual("Create 味方 ".Length, refs[0].Column);
        }

        [TestMethod]
        public async Task Lint_WarnsMissingUnitButNotWhenScenarioHasNoUnits()
        {
            using var scenario = new TestScenario();
            scenario.Write(Path.Combine("Data", "robot.txt"),
                "試作機", "試作機, しさくき, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp", "===", "");
            scenario.Write("test.eve", "スタート:", "Create 味方 試作機 0 主人公 1 1 1", "Create 味方 無い機体 0 主人公 1 2 2", "Exit");
            var result = await new DataLinter().LintAsync(new[] { scenario.Root });
            var messages = result.Diagnostics.Select(d => d.Message).Where(m => m.Contains("シナリオのデータにありません")).ToList();
            // ユニットは無い機体だけ。パイロットのデータが 1 つも無いので、パイロットは調べない
            Assert.AreEqual(1, messages.Count, string.Join("\n", messages));
            StringAssert.Contains(messages[0], "ユニット「無い機体」");
            Assert.IsTrue(result.HasError);
        }
    }
}
