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

        [TestMethod]
        [DataRow("SRCRender")]
        [DataRow("SRCSharpAvalonia")]
        public void EnglishResourcesHaveAllKeys(string project)
        {
            var dir = Path.Combine(SolutionDirectory(), project, "Resources");
            var neutral = ResxKeys(Path.Combine(dir, "Strings.resx"));
            var english = ResxKeys(Path.Combine(dir, "Strings.en.resx"));
            Assert.IsTrue(neutral.Length > 0);
            CollectionAssert.AreEquivalent(neutral, english);
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
