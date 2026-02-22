using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// DiffTime の追加テストと Weekday の全曜日テスト
    /// </summary>
    [TestClass]
    public class TimeFunctionExtendedTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // DiffTime - 日をまたぐケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DiffTime_OneDayDifference_Returns86400()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("DiffTime(\"2024/01/01 00:00:00\",\"2024/01/02 00:00:00\")");
            Assert.AreEqual(86400d, result);
        }

        [TestMethod]
        public void DiffTime_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            var result = exp.GetValueAsString("DiffTime(\"2024/01/01 10:00:00\",\"2024/01/01 10:00:30\")");
            Assert.AreEqual("30", result);
        }

        // ──────────────────────────────────────────────
        // Weekday - 全曜日テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Weekday_Monday_Returns月曜()
        {
            var exp = Create();
            // 2024/03/18 は月曜日
            Assert.AreEqual("月曜", exp.GetValueAsString("Weekday(\"2024/03/18\")"));
        }

        [TestMethod]
        public void Weekday_Tuesday_Returns火曜()
        {
            var exp = Create();
            // 2024/03/19 は火曜日
            Assert.AreEqual("火曜", exp.GetValueAsString("Weekday(\"2024/03/19\")"));
        }

        [TestMethod]
        public void Weekday_Wednesday_Returns水曜()
        {
            var exp = Create();
            // 2024/03/20 は水曜日
            Assert.AreEqual("水曜", exp.GetValueAsString("Weekday(\"2024/03/20\")"));
        }

        [TestMethod]
        public void Weekday_Thursday_Returns木曜()
        {
            var exp = Create();
            // 2024/03/21 は木曜日
            Assert.AreEqual("木曜", exp.GetValueAsString("Weekday(\"2024/03/21\")"));
        }

        [TestMethod]
        public void Weekday_Saturday_Returns土曜()
        {
            var exp = Create();
            // 2024/03/16 は土曜日
            Assert.AreEqual("土曜", exp.GetValueAsString("Weekday(\"2024/03/16\")"));
        }

        // ──────────────────────────────────────────────
        // Year / Month / Day - 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Year_JanuaryFirst_ReturnsCorrectYear()
        {
            var exp = Create();
            Assert.AreEqual(2000d, exp.GetValueAsDouble("Year(\"2000/01/01\")"));
        }

        [TestMethod]
        public void Month_December_Returns12()
        {
            var exp = Create();
            Assert.AreEqual(12d, exp.GetValueAsDouble("Month(\"2024/12/25\")"));
        }

        [TestMethod]
        public void Day_LastDayOfMonth_Returns31()
        {
            var exp = Create();
            Assert.AreEqual(31d, exp.GetValueAsDouble("Day(\"2024/01/31\")"));
        }

        // ──────────────────────────────────────────────
        // Hour / Minute / Second - 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Hour_Midnight_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Hour(\"2024/01/01 00:00:00\")"));
        }

        [TestMethod]
        public void Hour_LastHour_Returns23()
        {
            var exp = Create();
            Assert.AreEqual(23d, exp.GetValueAsDouble("Hour(\"2024/01/01 23:59:59\")"));
        }

        [TestMethod]
        public void Minute_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Minute(\"2024/01/01 10:00:00\")"));
        }

        [TestMethod]
        public void Minute_Fifty9_Returns59()
        {
            var exp = Create();
            Assert.AreEqual(59d, exp.GetValueAsDouble("Minute(\"2024/01/01 10:59:00\")"));
        }

        [TestMethod]
        public void Second_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Second(\"2024/01/01 10:30:00\")"));
        }

        [TestMethod]
        public void Second_Fifty9_Returns59()
        {
            var exp = Create();
            Assert.AreEqual(59d, exp.GetValueAsDouble("Second(\"2024/01/01 10:30:59\")"));
        }

        // ──────────────────────────────────────────────
        // Year/Month/Day - StringType return
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Year_StringReturn_ReturnsFormattedString()
        {
            var exp = Create();
            Assert.AreEqual("2024", exp.GetValueAsString("Year(\"2024/06/15\")"));
        }

        [TestMethod]
        public void Month_StringReturn_ReturnsFormattedString()
        {
            var exp = Create();
            Assert.AreEqual("6", exp.GetValueAsString("Month(\"2024/06/15\")"));
        }

        [TestMethod]
        public void Day_StringReturn_ReturnsFormattedString()
        {
            var exp = Create();
            Assert.AreEqual("15", exp.GetValueAsString("Day(\"2024/06/15\")"));
        }
    }
}
