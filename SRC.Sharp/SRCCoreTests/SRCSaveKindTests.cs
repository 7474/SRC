using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRCSaveKind enum の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SRCSaveKindTests
    {
        // ──────────────────────────────────────────────
        // 値の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Normal_IsZero()
        {
            Assert.AreEqual(0, (int)SRCSaveKind.Normal);
        }

        [TestMethod]
        public void Suspend_IsOne()
        {
            Assert.AreEqual(1, (int)SRCSaveKind.Suspend);
        }

        [TestMethod]
        public void Quik_IsTwo()
        {
            Assert.AreEqual(2, (int)SRCSaveKind.Quik);
        }

        [TestMethod]
        public void Restart_IsThree()
        {
            Assert.AreEqual(3, (int)SRCSaveKind.Restart);
        }

        [TestMethod]
        public void HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(SRCSaveKind)).Length);
        }

        [TestMethod]
        public void AllValuesAreDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCSaveKind), SRCSaveKind.Normal));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCSaveKind), SRCSaveKind.Suspend));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCSaveKind), SRCSaveKind.Quik));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCSaveKind), SRCSaveKind.Restart));
        }

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(SRCSaveKind));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (SRCSaveKind v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void CanBeParsedFromString()
        {
            Assert.AreEqual(SRCSaveKind.Normal, System.Enum.Parse<SRCSaveKind>("Normal"));
            Assert.AreEqual(SRCSaveKind.Suspend, System.Enum.Parse<SRCSaveKind>("Suspend"));
            Assert.AreEqual(SRCSaveKind.Quik, System.Enum.Parse<SRCSaveKind>("Quik"));
            Assert.AreEqual(SRCSaveKind.Restart, System.Enum.Parse<SRCSaveKind>("Restart"));
        }

        // ──────────────────────────────────────────────
        // 使用例の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Normal_IsDefaultValue()
        {
            SRCSaveKind defaultValue = default;
            Assert.AreEqual(SRCSaveKind.Normal, defaultValue);
        }

        [TestMethod]
        public void Suspend_NotEqualNormal()
        {
            Assert.AreNotEqual(SRCSaveKind.Normal, SRCSaveKind.Suspend);
        }

        [TestMethod]
        public void Quik_NotEqualSuspend()
        {
            Assert.AreNotEqual(SRCSaveKind.Suspend, SRCSaveKind.Quik);
        }

        [TestMethod]
        public void Restart_NotEqualQuik()
        {
            Assert.AreNotEqual(SRCSaveKind.Quik, SRCSaveKind.Restart);
        }

        [TestMethod]
        public void CanBeUsedInSwitch()
        {
            var kind = SRCSaveKind.Quik;
            string result = "";
            switch (kind)
            {
                case SRCSaveKind.Normal:
                    result = "通常";
                    break;
                case SRCSaveKind.Suspend:
                    result = "中断";
                    break;
                case SRCSaveKind.Quik:
                    result = "クイック";
                    break;
                case SRCSaveKind.Restart:
                    result = "リスタート";
                    break;
            }
            Assert.AreEqual("クイック", result);
        }
    }
}
