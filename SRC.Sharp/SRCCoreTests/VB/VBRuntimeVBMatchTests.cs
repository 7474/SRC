using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.TestLib;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VB6 のランタイム（Collection.Remove・IsDate・CDate）の動作と突き合わせたテスト
    /// </summary>
    [TestClass]
    public class VBRuntimeVBMatchTests
    {
        // VB の Collection.Remove に番号を渡すと、その番号の要素を消す（値で探さない）
        [TestMethod]
        public void SrcCollection_RemoveAt_RemovesItemAtIndexEvenIfValueIsDuplicated()
        {
            // 同じオブジェクトを 2 つのキーで持つ
            var a = new object();
            var col = new SrcCollection<object>();
            col.Add(a, "k1");
            col.Add(a, "k2");
            col.Add(new object(), "k3");

            col.RemoveAt(2);

            Assert.AreEqual(2, col.Count);
            Assert.IsTrue(col.ContainsKey("k1"));
            Assert.IsFalse(col.ContainsKey("k2"));
            Assert.IsTrue(col.ContainsKey("k3"));
        }

        private static Expression CreateExpression()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // 日本語版の Windows の IsDate・CDate は「2024年1月2日」を日付として読む
        [TestMethod]
        public void Year_JapaneseDateString_IsDate()
        {
            var exp = CreateExpression();
            Assert.AreEqual(2024d, exp.GetValueAsDouble("Year(\"2024年1月2日\")"));
            Assert.AreEqual(1d, exp.GetValueAsDouble("Month(\"2024年1月2日\")"));
            Assert.AreEqual(2d, exp.GetValueAsDouble("Day(\"2024年1月2日\")"));
        }

        // CDate は時刻だけの文字列を、日付の 0（1899/12/30）のその時刻にする（今日の日付にはしない）
        [TestMethod]
        public void DateFunctions_TimeOnlyString_UsesDateZero()
        {
            var exp = CreateExpression();
            Assert.AreEqual(1899d, exp.GetValueAsDouble("Year(\"12:34:56\")"));
            Assert.AreEqual(12d, exp.GetValueAsDouble("Month(\"12:34:56\")"));
            Assert.AreEqual(30d, exp.GetValueAsDouble("Day(\"12:34:56\")"));
            Assert.AreEqual("土曜", exp.GetValueAsString("Weekday(\"12:34:56\")"));
            Assert.AreEqual(12d, exp.GetValueAsDouble("Hour(\"12:34:56\")"));
            Assert.AreEqual(34d, exp.GetValueAsDouble("Minute(\"12:34:56\")"));
            Assert.AreEqual(56d, exp.GetValueAsDouble("Second(\"12:34:56\")"));
        }

        // 日本語版の Windows の時刻の「午前」「午後」
        [TestMethod]
        public void Hour_JapaneseAmPm_IsDate()
        {
            var exp = CreateExpression();
            Assert.AreEqual(13d, exp.GetValueAsDouble("Hour(\"午後 1:30:00\")"));
            Assert.AreEqual(1d, exp.GetValueAsDouble("Hour(\"午前 1:30:00\")"));
        }
    }
}
