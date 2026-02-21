using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// WeaponListMode enum のユニットテスト
    /// </summary>
    [TestClass]
    public class WeaponListModeEnumTests
    {
        [TestMethod]
        public void List_IsZero()
        {
            Assert.AreEqual(0, (int)WeaponListMode.List);
        }

        [TestMethod]
        public void BeforeMove_IsOne()
        {
            Assert.AreEqual(1, (int)WeaponListMode.BeforeMove);
        }

        [TestMethod]
        public void AfterMove_IsTwo()
        {
            Assert.AreEqual(2, (int)WeaponListMode.AfterMove);
        }

        [TestMethod]
        public void Counter_IsThree()
        {
            Assert.AreEqual(3, (int)WeaponListMode.Counter);
        }

        [TestMethod]
        public void HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(WeaponListMode)).Length);
        }

        [TestMethod]
        public void AllValuesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponListMode), WeaponListMode.List));
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponListMode), WeaponListMode.BeforeMove));
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponListMode), WeaponListMode.AfterMove));
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponListMode), WeaponListMode.Counter));
        }

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(WeaponListMode));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (WeaponListMode v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }
    }
}
