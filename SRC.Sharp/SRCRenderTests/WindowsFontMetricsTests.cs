using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCRender.Compat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCRender.Tests
{
    /// <summary>
    /// フォントの縦の寸法（WindowsFontMetrics）。Windows では実フォントと照合し、測った値を書き出す。
    /// </summary>
    [TestClass]
    public class WindowsFontMetricsTests
    {
        /// <summary>表に無いが、シナリオでよく使われ Windows で測っておきたいフォント</summary>
        private static readonly string[] ExtraNames =
        {
            "Yu Mincho", "BIZ UDGothic", "BIZ UDPGothic", "BIZ UDMincho", "Meiryo UI",
        };

        [DataTestMethod]
        [DataRow("ＭＳ ゴシック", "MS Gothic")]
        [DataRow("ＭＳ　Ｐ明朝", "MS PMincho")]
        [DataRow("ＭＳ UI Gothic", "MS UI Gothic")]
        [DataRow("メイリオ", "Meiryo")]
        [DataRow("游ゴシック", "Yu Gothic")]
        [DataRow("Ｐゴシック", "MS PGothic")]
        [DataRow("Noto Sans CJK JP", "Noto Sans CJK JP")]
        public void Normalize(string name, string expected)
        {
            Assert.AreEqual(expected, WindowsFontMetrics.Normalize(name));
        }

        [TestMethod]
        public void TextHeightUsesWindowsMetrics()
        {
            // 20pt = 26.67px。メイリオの行の高さは約 1.5em、ＭＳ ゴシックは 1em
            using (var meiryo = SkiaFonts.Create("メイリオ", 20 * 96f / 72f, true, false))
            using (var gothic = SkiaFonts.Create("ＭＳ ゴシック", 20 * 96f / 72f, false, false))
            using (var unknown = SkiaFonts.Create("存在しないフォント", 20 * 96f / 72f, false, false))
            {
                WindowsFontMetrics.TryGet("Meiryo", out var m);
                Assert.AreEqual(26.67f * m.LineHeight, SkiaFonts.LineHeight(meiryo), 0.05f);
                Assert.AreEqual(26.67f * m.Ascent, SkiaFonts.Ascent(meiryo), 0.05f);
                Assert.AreEqual(26.67f, SkiaFonts.LineHeight(gothic), 0.05f);
                Assert.IsFalse(WindowsFontMetrics.TryGet("存在しないフォント", out _));
                Assert.IsTrue(SkiaFonts.LineHeight(unknown) > 0);
            }
        }

        /// <summary>
        /// Windows に入っているフォントの寸法を GDI+（元の SRC の GDI と同じ usWinAscent / usWinDescent）で測り、表と照合する。
        /// 測った値は表に貼れる形で TestResults/fontmetrics.txt に書き出す。
        /// </summary>
        [TestMethod]
        public void MatchesInstalledWindowsFonts()
        {
            if (!OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Windows でのみ実フォントと照合する");
            }

            var report = new StringBuilder();
            var mismatches = new List<string>();
            var unverified = new List<string>();
            var measured = 0;
            foreach (var name in WindowsFontMetrics.Names.Concat(ExtraNames).Distinct())
            {
                System.Drawing.FontFamily family;
                try
                {
                    family = new System.Drawing.FontFamily(name);
                }
                catch (ArgumentException)
                {
                    report.AppendLine($"// {name}: 入っていない");
                    continue;
                }
                using (family)
                {
                    var style = System.Drawing.FontStyle.Regular;
                    var em = family.GetEmHeight(style);
                    var ascent = family.GetCellAscent(style);
                    var descent = family.GetCellDescent(style);
                    measured++;
                    report.AppendLine($"[\"{name}\"] = new Metrics({ascent}f / {em}, {descent}f / {em}, true),");
                    if (!WindowsFontMetrics.TryGet(name, out var expected))
                    {
                        continue;
                    }
                    var differs = Math.Abs(expected.Ascent - (float)ascent / em) > 0.005f || Math.Abs(expected.Descent - (float)descent / em) > 0.005f;
                    var line = $"{name}: 表 {expected.Ascent:F4}/{expected.Descent:F4}、実測 {(float)ascent / em:F4}/{(float)descent / em:F4}";
                    if (differs && expected.Verified)
                    {
                        mismatches.Add(line);
                    }
                    else if (!expected.Verified)
                    {
                        // 暫定値。実測値で表を更新し Verified = true にする
                        unverified.Add((differs ? "違う " : "一致 ") + line);
                    }
                }
            }

            var dir = Path.Combine(AppContext.BaseDirectory, "TestResults");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "fontmetrics.txt"), report.ToString(), new UTF8Encoding(false));
            Console.WriteLine(report);

            if (measured == 0)
            {
                Assert.Inconclusive("日本語フォントが入っていない");
            }
            Assert.AreEqual(0, mismatches.Count, string.Join(Environment.NewLine, mismatches));
            if (unverified.Count > 0)
            {
                Assert.Inconclusive("暫定値があります（TestResults/fontmetrics.txt の値で表を更新してください）:" + Environment.NewLine + string.Join(Environment.NewLine, unverified));
            }
        }
    }
}
