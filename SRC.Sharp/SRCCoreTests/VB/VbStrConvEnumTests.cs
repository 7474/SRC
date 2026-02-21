using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VbStrConv enum のユニットテスト
    /// </summary>
    [TestClass]
    public class VbStrConvEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_IsZero()
        {
            Assert.AreEqual(0, (int)VbStrConv.Wide);
        }

        [TestMethod]
        public void Narrow_IsOne()
        {
            Assert.AreEqual(1, (int)VbStrConv.Narrow);
        }

        [TestMethod]
        public void Hiragana_IsTwo()
        {
            Assert.AreEqual(2, (int)VbStrConv.Hiragana);
        }

        // ──────────────────────────────────────────────
        // Enum 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(VbStrConv)).Length);
        }

        [TestMethod]
        public void AllValuesAreDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(VbStrConv), VbStrConv.Wide));
            Assert.IsTrue(System.Enum.IsDefined(typeof(VbStrConv), VbStrConv.Narrow));
            Assert.IsTrue(System.Enum.IsDefined(typeof(VbStrConv), VbStrConv.Hiragana));
        }

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(VbStrConv));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (VbStrConv v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void CanBeParsedFromString()
        {
            Assert.AreEqual(VbStrConv.Wide, System.Enum.Parse<VbStrConv>("Wide"));
            Assert.AreEqual(VbStrConv.Narrow, System.Enum.Parse<VbStrConv>("Narrow"));
            Assert.AreEqual(VbStrConv.Hiragana, System.Enum.Parse<VbStrConv>("Hiragana"));
        }
    }
}
