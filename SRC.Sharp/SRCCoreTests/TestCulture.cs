using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace SRCCore.Tests
{
    /// <summary>
    /// 環境変数 SRC_TEST_CULTURE（例: de-DE・tr-TR）で、テストを実行する言語の設定を切り替える（7474/SRC#1554）。
    /// SRCCore が実行する環境の言語に左右されないことを確かめるために使う。
    /// Linux・macOS では LANG（LANG=de_DE.UTF-8）でも切り替わるが、Windows の言語の設定は環境変数では変えられないため、こちらを使う。
    /// </summary>
    [TestClass]
    public static class TestCulture
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var name = Environment.GetEnvironmentVariable("SRC_TEST_CULTURE");
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            // 無いカルチャの名前は CultureNotFoundException にする（気づかずに既定の設定で流さないため）
            var culture = CultureInfo.GetCultureInfo(name, predefinedOnly: true);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
