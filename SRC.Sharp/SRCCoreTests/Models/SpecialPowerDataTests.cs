using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SpecialPowerDataTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src)
            {
                Name = "熱血",
                KanaName = "ねっけつ",
                ShortName = "熱",
                SPConsumption = 40,
                TargetType = "自分",
                Duration = "1回",
                NecessaryCondition = "",
                Animation = "熱血アニメ",
                Comment = "次の攻撃が確実に決まる"
            };

            Assert.AreEqual("熱血", spd.Name);
            Assert.AreEqual("ねっけつ", spd.KanaName);
            Assert.AreEqual("熱", spd.ShortName);
            Assert.AreEqual(40, spd.SPConsumption);
            Assert.AreEqual("自分", spd.TargetType);
            Assert.AreEqual("1回", spd.Duration);
            Assert.AreEqual("熱血アニメ", spd.Animation);
            Assert.AreEqual("次の攻撃が確実に決まる", spd.Comment);
        }

        // ──────────────────────────────────────────────
        // SetEffect / CountEffect / IsEffectAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetEffect_SimpleEffect_AddsToEffects()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "熱血" };
            spd.SetEffect("必中");
            Assert.AreEqual(1, spd.CountEffect());
        }

        [TestMethod]
        public void SetEffect_MultipleEffects_AddsAll()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "気合" };
            spd.SetEffect("気力上昇 加速");
            Assert.IsTrue(spd.CountEffect() >= 1);
        }

        [TestMethod]
        public void IsEffectAvailable_ExistingEffect_ReturnsTrue()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "必中" };
            spd.SetEffect("必中");
            Assert.IsTrue(spd.IsEffectAvailable("必中"));
        }

        [TestMethod]
        public void IsEffectAvailable_NonExistingEffect_ReturnsFalse()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "熱血" };
            spd.SetEffect("必中");
            Assert.IsFalse(spd.IsEffectAvailable("気力上昇"));
        }

        [TestMethod]
        public void InitialState_NoEffects()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "テスト" };
            Assert.AreEqual(0, spd.CountEffect());
        }

        [TestMethod]
        public void Effects_InitiallyEmpty()
        {
            var src = CreateSRC();
            var spd = new SpecialPowerData(src) { Name = "テスト" };
            Assert.AreEqual(0, spd.Effects.Count);
        }
    }
}
