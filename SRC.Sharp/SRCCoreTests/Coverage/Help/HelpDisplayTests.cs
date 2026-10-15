using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help.SkillHelp / FeatureHelp / AttributeHelp（VB: Help.bas の同名 Sub）の表示のテスト。
    /// 「<b>名称</b>;解説」をシステムメッセージとして表示し、自動メッセージモードを元に戻す。
    /// </summary>
    [TestClass]
    public class HelpDisplayTests
    {
        private static List<(string pname, string msg)> Capture(HelpTestFixture fx, List<string> calls)
        {
            var shown = new List<(string, string)>();
            fx.Gui.OpenMessageFormHandler = (u1, u2) => calls.Add("open:" + fx.Gui.AutoMessageMode);
            fx.Gui.DisplayMessageHandler = (p, m, mode) => shown.Add((p, m));
            fx.Gui.CloseMessageFormHandler = () => calls.Add("close");
            return shown;
        }

        [TestMethod]
        public void SkillHelp_DisplaysNameAndMessage_RestoresAutoMode()
        {
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("切り払い", 8, "", 1);
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);
            fx.Gui.AutoMessageMode = true;

            fx.Src.Help.SkillHelp(u.MainPilot(), "1");

            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual("システム", shown[0].pname);
            Assert.AreEqual("<b>切り払いLv8</b>;格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を 50% の確率で切り払って回避する。", shown[0].msg);
            // 表示中は自動送りを止め、終了後に元に戻す
            CollectionAssert.AreEqual(new[] { "open:False", "close" }, calls);
            Assert.IsTrue(fx.Gui.AutoMessageMode);
        }

        [TestMethod]
        public void SkillHelp_EmptyMessage_DisplaysNothing()
        {
            // 解説のない技能では何も表示しない
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("謎の技能", 1, "", 1);
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.SkillHelp(u.MainPilot(), "1");

            Assert.AreEqual(0, shown.Count);
            Assert.AreEqual(0, calls.Count);
        }

        [TestMethod]
        public void FeatureHelp_DisplaysFeatureName()
        {
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("ＨＰ回復Lv2");
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.FeatureHelp(u, 1, false);

            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual("<b>ＨＰ回復Lv2</b>;毎ターン最大ＨＰの20%分のＨＰを回復。", shown[0].msg);
        }

        [TestMethod]
        public void FeatureHelp_WeaponArmorClass_ListsProficiencies()
        {
            // 「武器・防具クラス」は装備可能な武器・防具クラスを表示する
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("武器クラス=剣 槍");
            fx.UnitData.AddFeature("防具クラス=鎧");
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.FeatureHelp(u, "武器・防具クラス", false);

            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual("<b>武器・防具クラス</b>;武器【剣 槍】;防具【鎧】", shown[0].msg);
        }

        [TestMethod]
        public void FeatureHelp_WeaponArmorClass_NoneShowsDash()
        {
            var fx = new HelpTestFixture();
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.FeatureHelp(u, "武器・防具クラス", false);

            Assert.AreEqual("<b>武器・防具クラス</b>;武器【-】;防具【-】", shown[0].msg);
        }

        [TestMethod]
        public void AttributeHelp_LeveledAttribute_NameHasWideLevel()
        {
            // レベル付き属性は「属性名レベル（全角）」で見出しを付ける
            var fx = new HelpTestFixture();
            fx.AddWeapon("石化光線", "石L2");
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.AttributeHelp(u, "石L2", 1);

            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual("<b>石化攻撃レベル２</b>;クリティカル発生時に相手を２ターン石化させる。", shown[0].msg);
        }

        [TestMethod]
        public void AttributeHelp_PlainAttribute()
        {
            var fx = new HelpTestFixture();
            fx.AddWeapon("パンチ", "格");
            var u = fx.Build();
            var calls = new List<string>();
            var shown = Capture(fx, calls);

            fx.Src.Help.AttributeHelp(u, "格", 1);

            Assert.AreEqual("<b>格闘系攻撃</b>;パイロットの格闘を使って攻撃力を算出。", shown[0].msg);
        }
    }
}
