using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>シナリオに必要な素材が揃っているかの一覧（7474/SRC#1654）</summary>
    [TestClass]
    public class AssetInventoryTests
    {
        private static string SolutionDirectory()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            Assert.IsNotNull(dir, "SRC.Sharp.sln が見つからない");
            return dir.FullName;
        }

        [TestMethod]
        public void ListsReferencesWithSearchOrderDynamicAndUnused()
        {
            using (var scenario = new TestScenario())
            using (var app = new TestScenario())
            {
                scenario.Write("ステージ1.eve",
                    "スタート:",
                    "StartBGM 戦闘.mid",
                    "PaintPicture Event\\背景.bmp 0 0",
                    "PaintPicture Event\\本体の絵.bmp 0 0",
                    "PaintPicture Event\\無い.bmp 0 0",
                    "PaintPicture Event\\$(番号).bmp 0 0",
                    "PlaySound 無い.wav",
                    "PlaySound 無い.wav",
                    "PaintPicture Event\\背景.bmp 10 10",
                    "Exit");
                scenario.Write("ステージ1.map", "dummy");
                scenario.Write(Path.Combine("Data", "pilot.txt"),
                    "テスト", "テスト, てすと, 男性, 汎用, AAAA, 100", "特殊能力なし", "100, 100, 100, 100, 100, 100, 普通", "テスト.bmp, -.mid");
                scenario.Write(Path.Combine("Bitmap", "Event", "背景.bmp"), "x");
                scenario.Write(Path.Combine("Bitmap", "Pilot", "テスト.bmp"), "x");
                scenario.Write(Path.Combine("Bitmap", "Pilot", "テスト(怒).bmp"), "x");
                scenario.Write(Path.Combine("Bitmap", "Event", "使っていない.bmp"), "x");
                scenario.Write(Path.Combine("Bitmap", "Map", "plain0000.bmp"), "x");
                scenario.Write(Path.Combine("Midi", "戦闘.mp3"), "x");
                app.Write(Path.Combine("Bitmap", "Event", "本体の絵.bmp"), "x");
                app.Write(Path.Combine("Bitmap", "Event", "背景.bmp"), "x");

                var report = AssetReport.FromDirectories(scenario.Root, app.Root);

                // 見つからないものが先
                CollectionAssert.AreEqual(
                    new[] { "Event/無い.bmp", "無い.wav", "Event/本体の絵.bmp", "Event/背景.bmp", "テスト.bmp", "戦闘.mid" },
                    report.Assets.Select(x => x.Name).ToArray());
                Assert.AreEqual(2, report.MissingCount);

                // シナリオ → SRC 本体の順に探す（両方にあればシナリオ）
                var background = report.Assets.Single(x => x.Name == "Event/背景.bmp");
                Assert.AreEqual("scenario", background.FoundIn);
                Assert.AreEqual("Bitmap/Event/背景.bmp", background.FoundPath);
                CollectionAssert.AreEqual(new[] { 3, 9 }, background.Usages.Select(u => u.Line).ToArray());
                Assert.AreEqual("ステージ1.eve", background.Usages[0].File);
                Assert.AreEqual("app", report.Assets.Single(x => x.Name == "Event/本体の絵.bmp").FoundIn);
                // MIDI は同じ名前の MP3 で足りる
                Assert.AreEqual("Midi/戦闘.mp3", report.Assets.Single(x => x.Name == "戦闘.mid").FoundPath);
                Assert.AreEqual("sound", report.Assets.Single(x => x.Name == "無い.wav").Kind);
                // 同じ行は 1 回
                Assert.AreEqual(2, report.Assets.Single(x => x.Name == "無い.wav").Usages.Count);

                // 式で組み立てる名前
                Assert.AreEqual(1, report.Dynamic.Count);
                Assert.AreEqual(6, report.Dynamic[0].Line);
                StringAssert.Contains(report.Dynamic[0].Text, "$(番号)");

                // 参照していない素材（ステージのマップ・表情の差分・地形の画像・MIDI の代わりの MP3 は除く）
                CollectionAssert.AreEqual(new[] { "Bitmap/Event/使っていない.bmp" }, report.Unused.ToArray());

                var md = report.ToMarkdown();
                StringAssert.Contains(md, "**見つからない**");
                StringAssert.Contains(md, "`ステージ1.eve:3`<br>`ステージ1.eve:9`");
                StringAssert.Contains(md, "Bitmap/Event/使っていない.bmp");

                // README の下書きは、シナリオに含まれない素材（SRC 本体にあるもの・見つからないもの）
                var readme = report.ToReadme();
                StringAssert.Contains(readme, "`Event/本体の絵.bmp`");
                StringAssert.Contains(readme, "`無い.wav`");
                Assert.IsFalse(readme.Contains("背景.bmp"), readme);

                var json = report.ToJson();
                StringAssert.Contains(json, "\"foundIn\": \"app\"");
                StringAssert.Contains(json, "\"missing\": 2");
            }
        }

        [TestMethod]
        public void DynamicTokens()
        {
            Assert.IsTrue(AssetReferenceChecker.IsDynamicAssetToken("$(x).bmp"));
            Assert.IsTrue(AssetReferenceChecker.IsDynamicAssetToken(".bmp"));
            Assert.IsTrue(AssetReferenceChecker.IsDynamicAssetToken("X)).bmp"));
            Assert.IsFalse(AssetReferenceChecker.IsDynamicAssetToken("-.bmp"));
            Assert.IsFalse(AssetReferenceChecker.IsDynamicAssetToken("a.bmp"));
            Assert.IsFalse(AssetReferenceChecker.IsDynamicAssetToken("ラベル:"));
        }

        [TestMethod]
        public void BuildFromFileListsWithoutDisk()
        {
            // SRC#Studio（ブラウザ）は、ファイルの一覧と中身だけを渡す
            var roots = new[]
            {
                new AssetRoot("scenario", "s", new[] { "a.eve", "Bitmap\\Unit\\ロボ.bmp" }),
                new AssetRoot("app", "basic-data", new[] { "Bitmap/Event/空.png" }),
            };
            var report = AssetReport.Build(roots, new[] { ("a.eve", "PaintPicture 空.png 0 0\nCreate 味方 ロボ.bmp") });
            Assert.AreEqual(0, report.MissingCount);
            Assert.AreEqual("app", report.Assets.Single(x => x.Name == "空.png").FoundIn);
            Assert.AreEqual(0, report.Unused.Count);
        }

        [TestMethod]
        public void RealScenarioHasNoMissingAssets()
        {
            // 同梱のランタン号（素材をすべて同梱）
            var lantern = Path.Combine(SolutionDirectory(), "Content", "lantern");
            if (!Directory.Exists(lantern))
            {
                Assert.Inconclusive("Content/lantern がありません");
            }
            var report = AssetReport.FromDirectories(lantern, Path.Combine(SolutionDirectory(), "Content", "basic-data"));
            Assert.IsTrue(report.Assets.Count > 0);
            Assert.AreEqual(0, report.MissingCount, string.Join(", ", report.Assets.Where(x => x.Missing).Select(x => x.Name)));
        }
    }
}
