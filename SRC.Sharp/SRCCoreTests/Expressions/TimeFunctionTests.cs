using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出す時刻関数のユニットテスト
    /// 固定の日時文字列を引数に渡してテストする
    /// </summary>
    [TestClass]
    public class TimeFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Year
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Year_WithDateString_ReturnsYear()
        {
            var exp = Create();
            Assert.AreEqual(2024d, exp.GetValueAsDouble("Year(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Year_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Year(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Month
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Month_WithDateString_ReturnsMonth()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Month(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Month_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Month(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Day
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Day_WithDateString_ReturnsDay()
        {
            var exp = Create();
            Assert.AreEqual(15d, exp.GetValueAsDouble("Day(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Day_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Day(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Hour
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Hour_WithDateTimeString_ReturnsHour()
        {
            var exp = Create();
            Assert.AreEqual(14d, exp.GetValueAsDouble("Hour(\"2024/03/15 14:30:45\")"));
        }

        [TestMethod]
        public void Hour_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Hour(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Minute
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Minute_WithDateTimeString_ReturnsMinute()
        {
            var exp = Create();
            Assert.AreEqual(30d, exp.GetValueAsDouble("Minute(\"2024/03/15 14:30:45\")"));
        }

        [TestMethod]
        public void Minute_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Minute(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Second
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Second_WithDateTimeString_ReturnsSecond()
        {
            var exp = Create();
            Assert.AreEqual(45d, exp.GetValueAsDouble("Second(\"2024/03/15 14:30:45\")"));
        }

        [TestMethod]
        public void Second_InvalidDate_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Second(\"notadate\")"));
        }

        // ──────────────────────────────────────────────
        // Weekday
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Weekday_WithDateString_ReturnsDayOfWeekName()
        {
            var exp = Create();
            // 2024/03/15 は金曜日
            Assert.AreEqual("金曜", exp.GetValueAsString("Weekday(\"2024/03/15\")"));
        }

        [TestMethod]
        public void Weekday_Sunday_ReturnsSunday()
        {
            var exp = Create();
            // 2024/03/17 は日曜日
            Assert.AreEqual("日曜", exp.GetValueAsString("Weekday(\"2024/03/17\")"));
        }

        [TestMethod]
        public void Weekday_InvalidDate_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Weekday(\"notadate\")"));
        }
    }
}
