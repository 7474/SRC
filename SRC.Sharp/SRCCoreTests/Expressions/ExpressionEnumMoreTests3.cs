using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using System;
using System.Linq;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// OperatorType・ValueType enum の追加ユニットテスト (ExpressionEnumMoreTests3)
    /// </summary>
    [TestClass]
    public class ExpressionEnumMoreTests3
    {
        // ──────────────────────────────────────────────
        // OperatorType の整数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OperatorType_IntDivOp_IsFour()
        {
            Assert.AreEqual(4, (int)OperatorType.IntDivOp);
        }

        [TestMethod]
        public void OperatorType_ExpoOp_IsFive()
        {
            Assert.AreEqual(5, (int)OperatorType.ExpoOp);
        }

        [TestMethod]
        public void OperatorType_ModOp_IsSix()
        {
            Assert.AreEqual(6, (int)OperatorType.ModOp);
        }

        [TestMethod]
        public void OperatorType_CatOp_IsSeven()
        {
            Assert.AreEqual(7, (int)OperatorType.CatOp);
        }

        [TestMethod]
        public void OperatorType_EqOp_IsEight()
        {
            Assert.AreEqual(8, (int)OperatorType.EqOp);
        }

        [TestMethod]
        public void OperatorType_TotalCount_Is18()
        {
            Assert.AreEqual(18, Enum.GetValues(typeof(OperatorType)).Length);
        }

        [TestMethod]
        public void OperatorType_CanBeCastFromInt()
        {
            var op = (OperatorType)0;
            Assert.AreEqual(OperatorType.PlusOp, op);
        }

        [TestMethod]
        public void OperatorType_CanBeCastToInt()
        {
            int val = (int)OperatorType.LikeOp;
            Assert.AreEqual(17, val);
        }

        [TestMethod]
        public void OperatorType_AllNamesDefined()
        {
            var names = Enum.GetNames(typeof(OperatorType));
            CollectionAssert.Contains(names, "PlusOp");
            CollectionAssert.Contains(names, "MinusOp");
            CollectionAssert.Contains(names, "LikeOp");
        }

        // ──────────────────────────────────────────────
        // ValueType の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ValueType_UndefinedType_CastFromZero()
        {
            var vt = (ValueType)0;
            Assert.AreEqual(ValueType.UndefinedType, vt);
        }

        [TestMethod]
        public void ValueType_StringType_CastFromOne()
        {
            var vt = (ValueType)1;
            Assert.AreEqual(ValueType.StringType, vt);
        }

        [TestMethod]
        public void ValueType_NumericType_CastFromTwo()
        {
            var vt = (ValueType)2;
            Assert.AreEqual(ValueType.NumericType, vt);
        }

        [TestMethod]
        public void ValueType_AllNamesDefined()
        {
            var names = Enum.GetNames(typeof(ValueType));
            CollectionAssert.Contains(names, "UndefinedType");
            CollectionAssert.Contains(names, "StringType");
            CollectionAssert.Contains(names, "NumericType");
        }
    }
}
