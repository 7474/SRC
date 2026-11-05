using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;

namespace SRCSharpWeb.Tests
{
    /// <summary>
    /// SRC#Web のゲームの設定（ブラウザに残し、シナリオを始めるときに srcs.json の設定へ重ねる）。
    /// </summary>
    [TestClass]
    public class WebGameSettingsTests
    {
        [TestMethod]
        public void RoundTrip_ThroughJsonAndConfig()
        {
            var config = new LocalFileConfig();
            config.BattleAnimation = true;
            config.SoundVolume = 50;
            var settings = WebGameSettings.FromConfig(config);
            settings.BattleAnimation = false;
            settings.ShowTurn = true;
            settings.AutoDefense = true;
            settings.SoundVolume = 120;

            var restored = WebGameSettings.FromJson(settings.ToJson());
            restored.ApplyTo(config);

            Assert.IsFalse(config.BattleAnimation);
            Assert.AreEqual("On", config.GetItem("Option", "Turn"));
            Assert.IsTrue(config.AutoDefense);
            // 音量は 0〜100
            Assert.AreEqual(100, config.SoundVolume);
            Assert.IsTrue(WebGameSettings.FromConfig(config).SameAs(WebGameSettings.FromConfig(config)));
            Assert.IsFalse(WebGameSettings.FromConfig(config).SameAs(settings));
        }

        [TestMethod]
        public void MessageWait_RoundTripsAndDefaultsToNormal()
        {
            var config = new LocalFileConfig();
            var settings = WebGameSettings.FromConfig(config);
            Assert.AreEqual(700, settings.MessageWait);
            settings.MessageWait = 10000000;
            WebGameSettings.FromJson(settings.ToJson()).ApplyTo(config);
            Assert.AreEqual(10000000, config.MessageWait);

            // 前の版で保存した設定（メッセージスピードが無い）は「普通」
            Assert.AreEqual(700, WebGameSettings.FromJson("{\"BattleAnimation\":true}").MessageWait);
        }

        [TestMethod]
        public void FromJson_EmptyOrBroken_IsNull()
        {
            Assert.IsNull(WebGameSettings.FromJson(null));
            Assert.IsNull(WebGameSettings.FromJson(""));
            Assert.IsNull(WebGameSettings.FromJson("{"));
        }
    }
}
