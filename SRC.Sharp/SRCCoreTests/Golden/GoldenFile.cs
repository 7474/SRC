using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Golden.Tests
{
    /// <summary>
    /// 記録を expected.txt と比べる。SRC_GOLDEN_UPDATE=1 のときは expected.txt を作り直す。
    /// </summary>
    internal static class GoldenFile
    {
        public static void AssertMatch(string actual, string expectedPath, string testName, string what)
        {
            if (Environment.GetEnvironmentVariable("SRC_GOLDEN_UPDATE") == "1")
            {
                File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
                Assert.Inconclusive("expected.txt を作り直した（SRC_GOLDEN_UPDATE=1）");
            }
            Assert.IsTrue(File.Exists(expectedPath), $"expected.txt が無い。SRC_GOLDEN_UPDATE=1 dotnet test --filter {testName} で作る");
            var expected = File.ReadAllText(expectedPath).Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var diffs = new List<string>();
            for (var i = 0; i < Math.Max(expectedLines.Length, actualLines.Length) && diffs.Count < 20; i++)
            {
                var e = i < expectedLines.Length ? expectedLines[i] : "(無し)";
                var a = i < actualLines.Length ? actualLines[i] : "(無し)";
                if (e != a)
                {
                    diffs.Add($"{i + 1} 行目\n- {e}\n+ {a}");
                }
            }
            Assert.Fail($"{what}が変わった。意図した変更なら SRC_GOLDEN_UPDATE=1 で作り直して差分を PR に載せる\n" + string.Join("\n", diffs));
        }
    }
}
