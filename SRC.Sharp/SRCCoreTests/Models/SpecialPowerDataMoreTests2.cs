using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class SpecialPowerDataMoreTests2
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { Name = "ひらめき" };
            Assert.AreEqual("ひらめき", sp.Name);
        }

        [TestMethod]
        public void KanaName_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { KanaName = "ひらめき" };
            Assert.AreEqual("ひらめき", sp.KanaName);
        }

        [TestMethod]
        public void ShortName_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { ShortName = "ひら" };
            Assert.AreEqual("ひら", sp.ShortName);
        }

        [TestMethod]
        public void SPConsumption_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { SPConsumption = 20 };
            Assert.AreEqual(20, sp.SPConsumption);
        }

        [TestMethod]
        public void TargetType_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { TargetType = "自分" };
            Assert.AreEqual("自分", sp.TargetType);
        }

        [TestMethod]
        public void Duration_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { Duration = "1ターン" };
            Assert.AreEqual("1ターン", sp.Duration);
        }

        [TestMethod]
        public void Effects_InitiallyEmpty()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src);
            Assert.IsNotNull(sp.Effects);
            Assert.AreEqual(0, sp.Effects.Count);
        }

        [TestMethod]
        public void SetEffect_AddsEffect()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src);
            sp.SetEffect("必中");
            Assert.AreEqual(1, sp.Effects.Count);
        }

        [TestMethod]
        public void Comment_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var sp = new SpecialPowerData(src) { Comment = "必ず命中する" };
            Assert.AreEqual("必ず命中する", sp.Comment);
        }
    }
}
