using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class ValueTypeTests
    {
        [TestMethod]
        public void UndefinedType_Value_Is_0()
        {
            Assert.AreEqual(0, (int)ValueType.UndefinedType);
        }

        [TestMethod]
        public void StringType_Value_Is_1()
        {
            Assert.AreEqual(1, (int)ValueType.StringType);
        }

        [TestMethod]
        public void NumericType_Value_Is_2()
        {
            Assert.AreEqual(2, (int)ValueType.NumericType);
        }

        [TestMethod]
        public void MemberCount_Is_3()
        {
            var values = System.Enum.GetValues(typeof(ValueType));
            Assert.AreEqual(3, values.Length);
        }

        [TestMethod]
        public void IsDefined_ValidMembers_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), 0));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), 1));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), 2));
        }

        [TestMethod]
        public void IsDefined_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(System.Enum.IsDefined(typeof(ValueType), 99));
        }

        [TestMethod]
        public void Parse_ValidNames()
        {
            Assert.AreEqual(ValueType.UndefinedType, System.Enum.Parse(typeof(ValueType), "UndefinedType"));
            Assert.AreEqual(ValueType.StringType, System.Enum.Parse(typeof(ValueType), "StringType"));
            Assert.AreEqual(ValueType.NumericType, System.Enum.Parse(typeof(ValueType), "NumericType"));
        }

        [TestMethod]
        public void AllValues_AreUnique()
        {
            var values = (int[])System.Enum.GetValues(typeof(ValueType));
            CollectionAssert.AllItemsAreUnique(values);
        }
    }
}
