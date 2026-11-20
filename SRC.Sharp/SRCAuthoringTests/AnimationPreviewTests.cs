using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using SRCAuthoring.Linting;
using SRCAuthoring.Preview;
using SRCAuthoring.Templates;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class AnimationPreviewTests
    {
        [TestMethod]
        public void Parser_ExtractsAnimationEffectAndSubroutinesAndSelectsAtLine()
        {
            var animText = string.Join("\n",
                "試作機",
                "ビームライフル, ビーム",
                "攻撃, 斬撃 試作機\\slash.bmp",
                "",
                "汎用",
                "回避, -");
            var animEntries = AnimationDataParser.ParseText("Data/animation.txt", animText);
            Assert.AreEqual(3, animEntries.Count);
            Assert.AreEqual(AnimationPreviewKind.Animation, animEntries[0].Kind);
            Assert.AreEqual("試作機", animEntries[0].Target);
            Assert.AreEqual("ビームライフル", animEntries[0].Situation);
            Assert.AreEqual("ビーム", animEntries[0].Spec);

            var pickedAtLine3 = AnimationDataParser.SelectAtLine(animEntries, 3);
            Assert.AreEqual("攻撃", pickedAtLine3.Situation);

            // 見出し行（1行目）にカーソルを置いたときは、すぐ下の最初のエントリを選ぶ
            var pickedAtHeader = AnimationDataParser.SelectAtLine(animEntries, 1);
            Assert.AreEqual("ビームライフル", pickedAtHeader.Situation);

            var effectEntries = AnimationDataParser.ParseText("Data/effect.txt", "試作機\nビーム, Beam.wav\n");
            Assert.AreEqual(1, effectEntries.Count);
            Assert.AreEqual(AnimationPreviewKind.Effect, effectEntries[0].Kind);

            var subText = string.Join("\n",
                "戦闘アニメ_テスト斬撃:",
                "Wait 1",
                "Return",
                "",
                "Call 戦闘アニメ_テスト斬撃 赤",
                "@戦闘アニメ_ビーム準備 試作機");
            var subEntries = AnimationDataParser.ParseText("Lib/汎用戦闘アニメ/test.eve", subText);
            Assert.AreEqual(3, subEntries.Count);
            Assert.AreEqual(AnimationPreviewKind.Subroutine, subEntries[0].Kind);
            Assert.AreEqual("戦闘アニメ_テスト斬撃", subEntries[0].Spec);
            Assert.AreEqual("戦闘アニメ_テスト斬撃 赤", subEntries[1].Spec);
            Assert.AreEqual("戦闘アニメ_ビーム準備 試作機", subEntries[2].Spec);
        }

        [TestMethod]
        public async Task Template_CompletesSetupFromCatalogAndWritesPreviewFiles()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write(Path.Combine("Data", "robot.txt"),
                    "試作機", "試作機, しさくき, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp", "===", "",
                    "量産機", "量産機, りょうさんき, 汎用, 1, 2", "陸, 4, M, 1000, 100", "特殊能力なし", "3000, 100, 600, 60", "AAAA, -.bmp", "===", "");
                scenario.Write(Path.Combine("Data", "pilot.txt"),
                    "主人公", "主人公, しゅじんこう, 男性, 汎用, AAAA, 120", "特殊能力なし", "150, 145, 150, 150, 150, 155, 普通", "ＳＰ, 0", "-.bmp, -.mid", "",
                    "兵士(ザコ)", "兵士, へいし, 男性, 汎用, AAAA, 100", "特殊能力なし", "120, 120, 120, 120, 120, 120, 普通", "ＳＰ, 0", "-.bmp, -.mid", "");
                var lint = await new DataLinter().LintAsync(new[] { scenario.Root });
                var catalog = lint.Catalog;

                var setup = AnimationPreviewTemplate.Complete(null, catalog, "量産機");
                Assert.AreEqual("量産機", setup.Unit);
                Assert.AreEqual("主人公", setup.Pilot);
                Assert.AreEqual("量産機", setup.EnemyUnit);
                Assert.AreEqual("兵士(ザコ)", setup.EnemyPilot);

                var evePath = AnimationPreviewTemplate.Write(scenario.Root, setup, catalog);
                Assert.IsTrue(File.Exists(evePath));
                StringAssert.Contains(evePath.Replace('\\', '/'), ".srcsharp/preview/演出プレビュー.eve");
                var eveText = File.ReadAllText(evePath);
                StringAssert.Contains(eveText, "ChangeMap .srcsharp\\preview\\演出プレビュー.map");
                StringAssert.Contains(eveText, "Create 味方 \"量産機\" 0 \"主人公\" 30 5 6");
                StringAssert.Contains(eveText, "Create 敵 \"量産機\" 0 \"兵士(ザコ)\" 30 8 6");

                var mapPath = Path.Combine(Path.GetDirectoryName(evePath), AnimationPreviewTemplate.MapName);
                Assert.IsTrue(File.Exists(mapPath));
            }
        }
    }
}
