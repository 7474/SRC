using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// Condition クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ConditionClassTests
    {
        [TestMethod]
        public void Condition_DefaultName_IsNull()
        {
            var cond = new Condition();
            Assert.IsNull(cond.Name);
        }

        [TestMethod]
        public void Condition_SetName_ReturnsName()
        {
            var cond = new Condition { Name = "麻痺" };
            Assert.AreEqual("麻痺", cond.Name);
        }

        [TestMethod]
        public void Condition_DefaultLifetime_IsZero()
        {
            var cond = new Condition();
            Assert.AreEqual(0, cond.Lifetime);
        }

        [TestMethod]
        public void Condition_SetLifetime_ReturnsLifetime()
        {
            var cond = new Condition { Lifetime = 3 };
            Assert.AreEqual(3, cond.Lifetime);
        }

        [TestMethod]
        public void Condition_DefaultLevel_IsZero()
        {
            var cond = new Condition();
            Assert.AreEqual(0d, cond.Level);
        }

        [TestMethod]
        public void Condition_SetLevel_ReturnsLevel()
        {
            var cond = new Condition { Level = 2.5 };
            Assert.AreEqual(2.5, cond.Level);
        }

        [TestMethod]
        public void Condition_DefaultStrData_IsNull()
        {
            var cond = new Condition();
            Assert.IsNull(cond.StrData);
        }

        [TestMethod]
        public void Condition_SetStrData_ReturnsStrData()
        {
            var cond = new Condition { StrData = "援護攻撃" };
            Assert.AreEqual("援護攻撃", cond.StrData);
        }

        [TestMethod]
        public void IsEnable_WhenLifetimeIsZero_ReturnsFalse()
        {
            var cond = new Condition { Lifetime = 0 };
            Assert.IsFalse(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_WhenLifetimeIsPositive_ReturnsTrue()
        {
            var cond = new Condition { Lifetime = 5 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_WhenLifetimeIsNegativeOne_ReturnsTrue()
        {
            // 永続状態 (lifetime = -1)
            var cond = new Condition { Lifetime = -1 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_WhenLifetimeIsNegative_ReturnsTrue()
        {
            var cond = new Condition { Lifetime = -99 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void Condition_AllFieldsSetTogether_ReturnsCorrectValues()
        {
            var cond = new Condition
            {
                Name = "催眠",
                Lifetime = 2,
                Level = 1.0,
                StrData = "testdata"
            };
            Assert.AreEqual("催眠", cond.Name);
            Assert.AreEqual(2, cond.Lifetime);
            Assert.AreEqual(1.0, cond.Level);
            Assert.AreEqual("testdata", cond.StrData);
            Assert.IsTrue(cond.IsEnable);
        }
    }
}
