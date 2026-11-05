using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help の解説文で、小数のレベルから求める数値の丸め方が VB 版と同じかを確かめる。
    /// VB の Help.bas は Format$(CLng(5 * flevel)) のように CLng・CInt（偶数への丸め）で整数にし、
    /// 確率は CLng(100 * slevel \ 16) のように \ の前に被除数を偶数への丸めで整数にする。
    /// C# の (int) キャストは切り捨てなので、移植で値が変わっていた。
    /// </summary>
    [TestClass]
    public class HelpLevelRoundingTests
    {
        [TestMethod]
        public void SkillBonus_IsRoundedLikeCLng()
        {
            // 超感覚: Format$(CLng(2 * slevel + 3))。Lv1.25 → 5.5 → 6（切り捨てなら 5）
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("超感覚", 1.25, "", 1);
            var u = fx.Build();
            Assert.AreEqual("命中・回避に +6 の修正を与える。", fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }

        [TestMethod]
        public void SkillProbability_RoundsDividendBeforeIntDiv()
        {
            // 切り払い: Format$(CLng(100 * slevel \ 16))。Lv4.64 → 464 \ 16 = 29。
            // 100 * 4.64 は 463.99999999999994 なので、切り捨ててから割ると 28 になる
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("切り払い", 4.64, "", 1);
            var u = fx.Build();
            Assert.AreEqual(
                "格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を 29% の確率で切り払って回避する。",
                fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }

        [TestMethod]
        public void FeatureBonus_IsRoundedLikeCInt()
        {
            // 格闘強化: Format$(CInt(5 * flevel))。Lv1.5 → 7.5 → 8（切り捨てなら 7）
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("格闘強化Lv1.5");
            var u = fx.Build();
            Assert.AreEqual("パイロットの格闘を+8。", fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }

        [TestMethod]
        public void AttributeValue_IsRoundedLikeCInt()
        {
            // 脱: Format$(CInt(5 * alevel))。L1.5 → 7.5 → 8（切り捨てなら 7）
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "脱L1.5", bullet: 5);
            var u = fx.Build();
            Assert.AreEqual("相手の気力を8低下させる。", fx.Src.Help.AttributeHelpMessage(u, "脱L1.5", 1, false));
        }
    }
}
