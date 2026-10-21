using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class AssetReferenceCheckerTests
    {
        [TestMethod]
        public void ReportsMissingAssetsAsInformation()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("シナリオ.eve",
                    "スタート:",
                    "StartBGM 戦闘.mid",
                    "PaintPicture Event\\背景.bmp 0 0",
                    "PaintPicture Event\\無い.bmp 0 0",
                    "PaintPicture $(画像).bmp 0 0",
                    "PlaySound 無い.wav",
                    "PlaySound 無い.wav",
                    "# PaintPicture 注釈.bmp",
                    "ChangeMap 1 \"マップ\\始まり.map\"",
                    "Exit");
                var robot = scenario.Write(Path.Combine("Data", "robot.txt"),
                    "ロボ",
                    "ロボ, ろぼ, 汎用, 1, 2",
                    "陸, 4, M, 1000, 100",
                    "特殊能力なし",
                    "1000, 100, 1000, 60",
                    "AAAA, ロボ.bmp",
                    "===");
                var pilot = scenario.Write(Path.Combine("Data", "pilot.txt"),
                    "テスト", "テスト, てすと, 男性, 汎用, AAAA, 100", "特殊能力なし", "100, 100, 100, 100, 100, 100, 普通", "-.bmp, -.mid");
                var readme = scenario.Write("readme.txt", "画像.bmp を使っています");
                // 特殊効果は ; で続ける
                var effect = scenario.Write(Path.Combine("Data", "effect.txt"), "ビーム", "1;Beam.wav;1;無い音(low).wav;1;Beam.wav");

                var checker = new AssetReferenceChecker();
                // 汎用データの zip のように、シナリオと違うフォルダの下にあってもよい
                checker.AddKnownFiles(new[] { "src-sound/Sound/Beam.wav", "src-data/Bitmap/Event/背景.bmp", "src-data/Midi/戦闘.mp3", "scenario/マップ/始まり.map", "Bitmap/Unit/ロボ.BMP" });

                var found = checker.Check(new[] { eve, robot, pilot, readme, effect }).ToList();

                CollectionAssert.AreEqual(new[] { "Event/無い.bmp", "無い.wav", "無い音(low).wav" }, found.Select(x => x.Source).ToArray());
                Assert.IsTrue(found.All(x => x.Severity == DiagnosticSeverity.Information));
                Assert.AreEqual(4, found[0].Line);
                Assert.AreEqual(eve, found[0].File);
            }
        }

        [TestMethod]
        public void AssetNameSkipsExpressionsAndDummies()
        {
            Assert.IsNull(AssetReferenceChecker.AssetName("-.bmp"));
            Assert.IsNull(AssetReferenceChecker.AssetName("$(x).bmp"));
            Assert.IsNull(AssetReferenceChecker.AssetName("(a & \".bmp\")"));
            Assert.IsNull(AssetReferenceChecker.AssetName("ラベル:"));
            Assert.AreEqual("Bitmap/Unit/a.bmp", AssetReferenceChecker.AssetName("Bitmap\\Unit\\a.bmp"));
            Assert.AreEqual("a.mid", AssetReferenceChecker.AssetName("(a.mid)"));
        }
    }
}
