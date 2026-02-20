using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class ConditionTests
    {
        [TestMethod]
        public void IsEnable_ReturnsTrue_WhenLifetimeIsPositive()
        {
            var condition = new Condition { Name = "麻痺", Lifetime = 3, Level = 1.0, StrData = "" };
            Assert.IsTrue(condition.IsEnable);
        }

        [TestMethod]
        public void IsEnable_ReturnsTrue_WhenLifetimeIsNegative()
        {
            // 負数は無期限
            var condition = new Condition { Name = "強化", Lifetime = -1, Level = 2.0, StrData = "" };
            Assert.IsTrue(condition.IsEnable);
        }

        [TestMethod]
        public void IsEnable_ReturnsFalse_WhenLifetimeIsZero()
        {
            var condition = new Condition { Name = "睡眠", Lifetime = 0, Level = 1.0, StrData = "" };
            Assert.IsFalse(condition.IsEnable);
        }

        [TestMethod]
        public void DefaultValues_AreCorrect()
        {
            var condition = new Condition();
            Assert.IsNull(condition.Name);
            Assert.AreEqual(0, condition.Lifetime);
            Assert.AreEqual(0.0, condition.Level);
            Assert.IsNull(condition.StrData);
            Assert.IsFalse(condition.IsEnable);
        }

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var condition = new Condition
            {
                Name = "援護強化",
                Lifetime = 5,
                Level = 3.5,
                StrData = "援護攻撃"
            };

            Assert.AreEqual("援護強化", condition.Name);
            Assert.AreEqual(5, condition.Lifetime);
            Assert.AreEqual(3.5, condition.Level);
            Assert.AreEqual("援護攻撃", condition.StrData);
        }
    }
}
