using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression IsVarDefined, DiffTime, GetTime 等の追加関数テスト
    /// </summary>
    [TestClass]
    public class AdditionalFunctionTests
    {
        private (Expression exp, SRC src) Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return (src.Expression, src);
        }

        // ──────────────────────────────────────────────
        // IsVarDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVarDefined_DefinedVariable_ReturnsOne()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("myVar", 42d);
            // 引数はクォートなしで変数名を直接渡す
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsVarDefined(myVar)"));
        }

        [TestMethod]
        public void IsVarDefined_UndefinedVariable_ReturnsZero()
        {
            var (exp, src) = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsVarDefined(存在しない変数)"));
        }

        [TestMethod]
        public void IsVarDefined_StringReturnType_ReturnsStringOne()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("myVar", 1d);
            Assert.AreEqual("1", exp.GetValueAsString("IsVarDefined(myVar)"));
        }

        [TestMethod]
        public void IsVarDefined_StringReturnType_UndefinedReturnsStringZero()
        {
            var (exp, src) = Create();
            Assert.AreEqual("0", exp.GetValueAsString("IsVarDefined(存在しない変数)"));
        }

        // ──────────────────────────────────────────────
        // DiffTime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DiffTime_SameDateTime_ReturnsZero()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsDouble("DiffTime(\"2024/01/01 10:00:05\",\"2024/01/01 10:00:05\")");
            Assert.AreEqual(0d, result);
        }

        [TestMethod]
        public void DiffTime_SecondsDifference_ReturnsSecondsDelta()
        {
            var (exp, src) = Create();
            // DiffTime は d2.Second - d1.Second の差を返す実装
            var result = exp.GetValueAsDouble("DiffTime(\"2024/01/01 10:00:05\",\"2024/01/01 10:00:15\")");
            Assert.AreEqual(10d, result);
        }

        [TestMethod]
        public void DiffTime_NegativeSecondsDifference_ReturnsNegative()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsDouble("DiffTime(\"2024/01/01 10:00:15\",\"2024/01/01 10:00:05\")");
            Assert.AreEqual(-10d, result);
        }

        // ──────────────────────────────────────────────
        // Time/Date functions
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Year_ValidDate_ReturnsYear()
        {
            var (exp, src) = Create();
            Assert.AreEqual(2024d, exp.GetValueAsDouble("Year(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Month_ValidDate_ReturnsMonth()
        {
            var (exp, src) = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Month(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Day_ValidDate_ReturnsDay()
        {
            var (exp, src) = Create();
            Assert.AreEqual(15d, exp.GetValueAsDouble("Day(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Hour_ValidDateTime_ReturnsHour()
        {
            var (exp, src) = Create();
            Assert.AreEqual(14d, exp.GetValueAsDouble("Hour(\"2024/03/15 14:30:45\")"));
        }

        [TestMethod]
        public void Minute_ValidDateTime_ReturnsMinute()
        {
            var (exp, src) = Create();
            Assert.AreEqual(30d, exp.GetValueAsDouble("Minute(\"2024/03/15 14:30:45\")"));
        }

        [TestMethod]
        public void Second_ValidDateTime_ReturnsSecond()
        {
            var (exp, src) = Create();
            Assert.AreEqual(45d, exp.GetValueAsDouble("Second(\"2024/03/15 14:30:45\")"));
        }

        // ──────────────────────────────────────────────
        // Like演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Like_ExactMatch_ReturnsTrue()
        {
            var (exp, src) = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("\"hello\" Like \"hello\""));
        }

        [TestMethod]
        public void Like_NoMatch_ReturnsFalse()
        {
            var (exp, src) = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("\"hello\" Like \"world\""));
        }

        [TestMethod]
        public void Like_CaseDifference_ReturnsFalse()
        {
            var (exp, src) = Create();
            // Like は大文字小文字を区別する
            Assert.AreEqual(0d, exp.GetValueAsDouble("\"Hello\" Like \"hello\""));
        }
    }
}
