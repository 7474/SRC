using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit の地形適応・装甲値テストの追加 (UnitAdaptionArmorMoreTests)
    /// </summary>
    [TestClass]
    public class UnitAdaptionArmorMoreTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static void SetAdaption(Unit unit, string adaption)
        {
            var f = typeof(Unit).GetField("strAdaption", BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(unit, adaption);
        }

        private static void SetArmor(Unit unit, int armor)
        {
            var f = typeof(Unit).GetField("lngArmor", BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(unit, armor);
        }

        private static void AddPilotWithAdaption(SRC src, Unit unit, string adaption)
        {
            var name = "パイロット_" + System.Guid.NewGuid().ToString("N");
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            var pilot = src.PList.Add(name, 1, "味方");
            pilot.Adaption = adaption;
            unit.AddPilot(pilot);
            pilot.Unit = unit;
        }

        // ──────────────────────────────────────────────
        // get_Adaption — 位置3 (水中) と 位置4 (宇宙)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetAdaption_NoPilot_Position3_B_Returns3()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AABA");
            Assert.AreEqual(3, unit.get_Adaption(3));
        }

        [TestMethod]
        public void GetAdaption_NoPilot_Position4_C_Returns2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAC");
            Assert.AreEqual(2, unit.get_Adaption(4));
        }

        [TestMethod]
        public void GetAdaption_WithPilot_BothA_Returns4()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            AddPilotWithAdaption(src, unit, "AAAA");
            Assert.AreEqual(4, unit.get_Adaption(2));
        }

        [TestMethod]
        public void GetAdaption_PilotBLimitsUnitS_Position2_Returns3()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "ASAA");
            AddPilotWithAdaption(src, unit, "ABAA");
            // unit pos2=S(5), pilot pos2=B(3) → min(5,3)=3
            Assert.AreEqual(3, unit.get_Adaption(2));
        }

        // ──────────────────────────────────────────────
        // get_AdaptionMod — 全位置テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AdaptionMod_Position3_B_Returns1_0()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AABA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(3, 0));
        }

        [TestMethod]
        public void AdaptionMod_Position4_C_Returns0_8()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAC");
            Assert.AreEqual(0.8d, unit.get_AdaptionMod(4, 0));
        }

        [TestMethod]
        public void AdaptionMod_Position2_D_Returns0_6()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "ADAA");
            Assert.AreEqual(0.6d, unit.get_AdaptionMod(2, 0));
        }

        // ──────────────────────────────────────────────
        // get_Armor — 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Armor_LargeValue_ReturnsLargeValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 9999);
            Assert.AreEqual(9999, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_1500_ReturnsIt()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1500);
            Assert.AreEqual(1500, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_Zero_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 0);
            Assert.AreEqual(0, unit.get_Armor(""));
        }

        [TestMethod]
        public void GetAdaption_FiveCharAdaption_DoesNotThrow()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SABCD");
            // Reading index 1-4 should not throw
            for (int i = 1; i <= 4; i++)
            {
                int v = unit.get_Adaption(i);
                Assert.IsTrue(v >= 0 && v <= 5, $"idx {i}: {v}");
            }
        }
    }
}
