using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class OperatorTypeTests
    {
        [TestMethod]
        public void PlusOp_Value_Is_0()
        {
            Assert.AreEqual(0, (int)OperatorType.PlusOp);
        }

        [TestMethod]
        public void MemberCount_Is_18()
        {
            var values = System.Enum.GetValues(typeof(OperatorType));
            Assert.AreEqual(18, values.Length);
        }

        [TestMethod]
        public void IsDefined_ValidMembers_ReturnsTrue()
        {
            for (int i = 0; i < 18; i++)
            {
                Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), i), $"Value {i} should be defined");
            }
        }

        [TestMethod]
        public void IsDefined_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(System.Enum.IsDefined(typeof(OperatorType), 99));
        }

        [TestMethod]
        public void Parse_ValidNames()
        {
            Assert.AreEqual(OperatorType.PlusOp, System.Enum.Parse(typeof(OperatorType), "PlusOp"));
            Assert.AreEqual(OperatorType.MinusOp, System.Enum.Parse(typeof(OperatorType), "MinusOp"));
            Assert.AreEqual(OperatorType.LikeOp, System.Enum.Parse(typeof(OperatorType), "LikeOp"));
        }

        [TestMethod]
        public void AllMembers_Exist()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.PlusOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.MinusOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.MultOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.DivOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.IntDivOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.ExpoOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.ModOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.CatOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.EqOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.NotEqOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.LtOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.LtEqOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.GtOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.GtEqOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.NotOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.AndOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.OrOp));
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), (int)OperatorType.LikeOp));
        }

        [TestMethod]
        public void AllValues_AreUnique()
        {
            var values = (int[])System.Enum.GetValues(typeof(OperatorType));
            CollectionAssert.AllItemsAreUnique(values);
        }
    }
}
