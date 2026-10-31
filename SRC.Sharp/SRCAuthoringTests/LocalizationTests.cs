using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>表示の言語の切り替え（srcsharp とデータビューアの .resx）</summary>
    [TestClass]
    public class LocalizationTests
    {
        /// <summary>既定（日本語）の .resx と英語の .resx の組</summary>
        public static IEnumerable<object[]> ResxPairs => new[]
        {
            new object[] { Path.Combine("SRCSharpTools", "Resources", "Strings") },
            new object[] { Path.Combine("SRCDataViewer", "Resources", "UIStrings") },
        };

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

        private static Dictionary<string, string> ReadResx(string path)
        {
            return XDocument.Load(path).Root.Elements("data")
                .ToDictionary(x => (string)x.Attribute("name"), x => (string)x.Element("value") ?? "");
        }

        private static string[] Placeholders(string value)
        {
            return Regex.Matches(value, @"\{\d+\}").Select(m => m.Value).Distinct().OrderBy(x => x).ToArray();
        }

        [TestMethod]
        [DynamicData(nameof(ResxPairs))]
        public void Resx_EnglishHasSameKeys(string baseName)
        {
            var root = SolutionDirectory();
            var neutral = ReadResx(Path.Combine(root, baseName + ".resx"));
            var english = ReadResx(Path.Combine(root, baseName + ".en.resx"));

            Assert.IsTrue(neutral.Count > 0);
            CollectionAssert.AreEquivalent(neutral.Keys.ToList(), english.Keys.ToList(), baseName);
            foreach (var (key, value) in neutral)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(english[key]), $"{baseName}: {key} が空");
                CollectionAssert.AreEqual(Placeholders(value), Placeholders(english[key]), $"{baseName}: {key} の埋め込みが違う");
            }
        }

        private static (int exitCode, string stdout, string stderr) RunCli(string language, params string[] args)
        {
            var dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
            var psi = new ProcessStartInfo(dotnet)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            psi.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "srcsharp.dll"));
            foreach (var arg in args)
            {
                psi.ArgumentList.Add(arg);
            }
            // CI の英語の OS でも既定は日本語であることを確かめるため、OS の言語は英語にしておく
            psi.Environment["LANG"] = "en_US.UTF-8";
            psi.Environment["LC_ALL"] = "en_US.UTF-8";
            if (language == null)
            {
                psi.Environment.Remove("SRCSHARP_LANG");
            }
            else
            {
                psi.Environment["SRCSHARP_LANG"] = language;
            }
            using (var process = Process.Start(psi))
            {
                var stderr = process.StandardError.ReadToEndAsync();
                var stdout = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return (process.ExitCode, stdout, stderr.Result);
            }
        }

        [TestMethod]
        public void Cli_LangEn_PrintsEnglishUsage()
        {
            var (exitCode, stdout, _) = RunCli(null, "--lang", "en", "--help");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "Usage:");
            StringAssert.Contains(stdout, "srcsharp lint <directory or file>");
        }

        [TestMethod]
        public void Cli_DefaultIsJapanese()
        {
            var (exitCode, stdout, _) = RunCli(null, "--help");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "使い方:");
        }

        [TestMethod]
        public void Cli_EnvironmentVariable_OptionWins()
        {
            var (_, _, stderr) = RunCli("en", "unknown-command");
            StringAssert.Contains(stderr, "Unknown command: unknown-command");

            (_, _, stderr) = RunCli("en", "--lang", "ja", "unknown-command");
            StringAssert.Contains(stderr, "不明なコマンド: unknown-command");
        }
    }
}
