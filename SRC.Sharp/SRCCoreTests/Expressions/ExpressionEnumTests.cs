using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// OperatorType・ValueType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionEnumTests
    {
        // ──────────────────────────────────────────────
        // OperatorType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OperatorType_PlusOp_IsZero()
        {
            Assert.AreEqual(0, (int)OperatorType.PlusOp);
        }

        [TestMethod]
        public void OperatorType_MinusOp_IsOne()
        {
            Assert.AreEqual(1, (int)OperatorType.MinusOp);
        }

        [TestMethod]
        public void OperatorType_MultOp_IsTwo()
        {
            Assert.AreEqual(2, (int)OperatorType.MultOp);
        }

        [TestMethod]
        public void OperatorType_DivOp_IsThree()
        {
            Assert.AreEqual(3, (int)OperatorType.DivOp);
        }

        [TestMethod]
        public void OperatorType_EqOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.EqOp));
        }

        [TestMethod]
        public void OperatorType_NotEqOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.NotEqOp));
        }

        [TestMethod]
        public void OperatorType_LtOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.LtOp));
        }

        [TestMethod]
        public void OperatorType_LtEqOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.LtEqOp));
        }

        [TestMethod]
        public void OperatorType_GtOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.GtOp));
        }

        [TestMethod]
        public void OperatorType_GtEqOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.GtEqOp));
        }

        [TestMethod]
        public void OperatorType_NotOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.NotOp));
        }

        [TestMethod]
        public void OperatorType_AndOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.AndOp));
        }

        [TestMethod]
        public void OperatorType_OrOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.OrOp));
        }

        [TestMethod]
        public void OperatorType_LikeOp_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(OperatorType), OperatorType.LikeOp));
        }

        [TestMethod]
        public void OperatorType_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(OperatorType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (OperatorType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        // ──────────────────────────────────────────────
        // ValueType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ValueType_UndefinedType_IsZero()
        {
            Assert.AreEqual(0, (int)ValueType.UndefinedType);
        }

        [TestMethod]
        public void ValueType_StringType_IsOne()
        {
            Assert.AreEqual(1, (int)ValueType.StringType);
        }

        [TestMethod]
        public void ValueType_NumericType_IsTwo()
        {
            Assert.AreEqual(2, (int)ValueType.NumericType);
        }

        [TestMethod]
        public void ValueType_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(ValueType)).Length);
        }

        [TestMethod]
        public void ValueType_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(ValueType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (ValueType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void ValueType_CanBeParsedFromString()
        {
            Assert.AreEqual(ValueType.UndefinedType, System.Enum.Parse<ValueType>("UndefinedType"));
            Assert.AreEqual(ValueType.StringType, System.Enum.Parse<ValueType>("StringType"));
            Assert.AreEqual(ValueType.NumericType, System.Enum.Parse<ValueType>("NumericType"));
        }

        [TestMethod]
        public void ValueType_IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), ValueType.UndefinedType));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), ValueType.StringType));
            Assert.IsTrue(System.Enum.IsDefined(typeof(ValueType), ValueType.NumericType));
        }
    }
}
