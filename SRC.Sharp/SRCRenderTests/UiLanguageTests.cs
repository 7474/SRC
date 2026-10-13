using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCRender.Compat;
using SRCRender.Resources;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 表示言語の切り替え（UiLanguage・Resources/Strings.resx）。
    /// </summary>
    [TestClass]
    public class UiLanguageTests
    {
        private CultureInfo savedUICulture;
        private CultureInfo savedDefaultUICulture;

        [TestInitialize]
        public void Initialize()
        {
            savedUICulture = CultureInfo.CurrentUICulture;
            savedDefaultUICulture = CultureInfo.DefaultThreadCurrentUICulture;
        }

        [TestCleanup]
        public void Cleanup()
        {
            CultureInfo.CurrentUICulture = savedUICulture;
            CultureInfo.DefaultThreadCurrentUICulture = savedDefaultUICulture;
        }

        [TestMethod]
        public void EnglishStrings()
        {
            UiLanguage.Apply("en");
            Assert.AreEqual("en", CultureInfo.CurrentUICulture.Name);
            Assert.AreEqual("Settings", Strings.ConfigTitle);
            Assert.AreEqual("Cancel", Strings.Cancel);
            Assert.AreEqual("Auto", Strings.AutoAdvance);
            Assert.AreEqual("Settings", new ConfigWindow().Text);
            Assert.AreEqual("Select from top", new MultiSelectListWindow().cmdSelectAll.Text);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("ja")]
        public void JapaneseStrings(string language)
        {
            // OS の言語が英語でも既定は日本語
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            UiLanguage.Apply(language);
            Assert.AreEqual("ja", CultureInfo.CurrentUICulture.Name);
            Assert.AreEqual("設定変更", Strings.ConfigTitle);
            Assert.AreEqual("キャンセル", Strings.Cancel);
            Assert.AreEqual("自動送り", Strings.AutoAdvance);
            Assert.AreEqual("先頭から選択", new MultiSelectListWindow().cmdSelectAll.Text);
        }

        [TestMethod]
        public void UnknownLanguageIsJapanese()
        {
            UiLanguage.Apply("xx-invalid-culture-name");
            Assert.AreEqual("設定変更", Strings.ConfigTitle);
        }

        [TestMethod]
        public void OnlyUICultureChanges()
        {
            var culture = CultureInfo.CurrentCulture;
            UiLanguage.Apply("en");
            Assert.AreEqual(culture, CultureInfo.CurrentCulture);
            Assert.AreEqual("en", CultureInfo.DefaultThreadCurrentUICulture.Name);
        }

        [TestMethod]
        public void ApplyFromConfig()
        {
            var config = new LocalFileConfig();
            config.SetItem(UiLanguage.ConfigSection, UiLanguage.ConfigKey, "en");
            UiLanguage.Apply(config);
            Assert.AreEqual("Settings", Strings.ConfigTitle);

            UiLanguage.Apply(new LocalFileConfig());
            Assert.AreEqual("設定変更", Strings.ConfigTitle);
        }

        public static System.Collections.Generic.IEnumerable<object[]> ResourceFiles()
        {
            var files = new[]
            {
                "SRCRender/Resources/Strings",
                "SRCSharpAvalonia/Resources/Strings",
                "SRCSharpTools/Resources/Strings",
                "SRCSharpWeb/Resources/WebStrings",
                "SRCTestBlazor/Resources/UIStrings",
            };
            foreach (var file in files)
            {
                foreach (var language in SRCCore.UiLanguages.Supported.Where(x => x.Code != SRCCore.UiLanguages.Default))
                {
                    yield return new object[] { file, language.Code };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(ResourceFiles))]
        public void TranslatedResourcesHaveAllKeys(string file, string language)
        {
            // 対応言語ごとに、既定（日本語）の resx と同じキーがあり、{0} などの置き換えも揃っている
            var basePath = Path.Combine(SolutionDirectory(), file.Replace('/', Path.DirectorySeparatorChar));
            var neutral = ResxValues(basePath + ".resx");
            var translated = ResxValues(basePath + "." + language + ".resx");
            Assert.IsTrue(neutral.Count > 0);
            CollectionAssert.AreEquivalent(neutral.Keys.ToArray(), translated.Keys.ToArray());
            foreach (var (key, value) in neutral)
            {
                // 文の切れ端（リンクの後ろなど）は言語によって空でもよい
                Assert.IsTrue(translated[key] != "" || key.EndsWith("After"), key);
                CollectionAssert.AreEquivalent(Placeholders(value), Placeholders(translated[key]), key);
            }
        }

        [TestMethod]
        public void SatelliteLanguagesMatchSupported()
        {
            // csproj の SatelliteResourceLanguages に対応言語がすべてある
            var expected = SRCCore.UiLanguages.Supported.Select(x => x.Code).ToArray();
            // SRC#Web はサテライトアセンブリを使わず、訳をアセンブリに埋め込む（SRCSharpWeb.csproj）
            foreach (var project in new[] { "SRCRender", "SRCSharpAvalonia", "SRCSharpTools", "SRCTestBlazor" })
            {
                var csproj = XDocument.Load(Path.Combine(SolutionDirectory(), project, project + ".csproj"));
                var value = csproj.Descendants("SatelliteResourceLanguages").Single().Value;
                CollectionAssert.AreEquivalent(expected, value.Split(';'), project);
            }
        }

        [TestMethod]
        [DataRow("zh-Hans", "简体中文")]
        [DataRow("zh-Hant", "繁體中文")]
        [DataRow("ko", "한국어")]
        [DataRow("de", "Deutsch")]
        public void TranslatedStrings(string language, string unused)
        {
            UiLanguage.Apply(language);
            Assert.AreEqual(language, CultureInfo.CurrentUICulture.Name);
            Assert.AreNotEqual("設定変更", Strings.ConfigTitle);
        }

        [TestMethod]
        [DataRow("", "en-US", "ja")]
        [DataRow(null, "en-US", "ja")]
        [DataRow("ja", "en-US", "ja")]
        [DataRow("en", "ja-JP", "en")]
        [DataRow("en-GB", "ja-JP", "en")]
        [DataRow("EN", "ja-JP", "en")]
        [DataRow("fr-CA", "ja-JP", "fr")]
        [DataRow("pt-PT", "ja-JP", "pt-BR")]
        [DataRow("zh", "ja-JP", "zh-Hans")]
        [DataRow("zh-CN", "ja-JP", "zh-Hans")]
        [DataRow("zh-TW", "ja-JP", "zh-Hant")]
        [DataRow("zh-HK", "ja-JP", "zh-Hant")]
        [DataRow("zh-Hant-TW", "ja-JP", "zh-Hant")]
        [DataRow("auto", "ko-KR", "ko")]
        [DataRow("auto", "zh-TW", "zh-Hant")]
        [DataRow("auto", "en-US", "en")]
        [DataRow("auto", "th-TH", "th")]
        [DataRow("auto", "vi-VN", "ja")]
        [DataRow("auto", "", "ja")]
        [DataRow("xx-invalid", "en-US", "ja")]
        public void ResolveLanguages(string setting, string platform, string expected)
        {
            Assert.AreEqual(expected, SRCCore.UiLanguages.Resolve(setting, platform));
        }

        [TestMethod]
        public void UnknownLanguageIsReported()
        {
            SRCCore.UiLanguages.Resolve("xx-invalid", "en-US", out var known);
            Assert.IsFalse(known);
            SRCCore.UiLanguages.Resolve("de-AT", "en-US", out known);
            Assert.IsTrue(known);
        }

        [TestMethod]
        public void PropertiesMatchResources()
        {
            // Strings のプロパティがすべて resx のキーを指している
            var neutral = ResxKeys(Path.Combine(SolutionDirectory(), "SRCRender", "Resources", "Strings.resx"));
            var properties = typeof(Strings).GetProperties().Select(x => x.Name).ToArray();
            CollectionAssert.AreEquivalent(neutral, properties);
            UiLanguage.Apply("ja");
            foreach (var p in typeof(Strings).GetProperties())
            {
                Assert.IsFalse(string.IsNullOrEmpty((string)p.GetValue(null)), p.Name);
            }
        }

        private static System.Collections.Generic.Dictionary<string, string> ResxValues(string path)
        {
            return XDocument.Load(path).Root.Elements("data").ToDictionary(x => (string)x.Attribute("name"), x => (string)x.Element("value") ?? "");
        }

        private static string[] Placeholders(string value)
        {
            return System.Text.RegularExpressions.Regex.Matches(value, @"\{\d+[^}]*\}").Select(x => x.Value).Distinct().ToArray();
        }

        private static string[] ResxKeys(string path)
        {
            return XDocument.Load(path).Root.Elements("data").Select(x => (string)x.Attribute("name")).ToArray();
        }

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
    }
}
