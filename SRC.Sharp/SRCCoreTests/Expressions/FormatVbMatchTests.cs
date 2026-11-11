using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Format 関数を、元の SRC（Expression.bas の CallFunction の "format"）と突き合わせたテスト。
    /// 元の SRC は Format$(GetValueAsString(引数 1), GetValueAsString(引数 2)) を呼ぶ。
    /// VB5 の Format$ は、書式の名前（Fixed・Long Date など）と日付・時刻の書式文字（yyyy・m・d・aaa など）を解釈する。
    /// 期待値は Microsoft の Format 関数の資料と、日本語版の Windows の地域の設定（日本）から読んだもの。
    /// 資料に無い細部（Currency の記号・Medium Date・Medium Time・Yes/No などの文言・和暦）は推定で、
    /// 実機との比較（#1550。SRC.Sharp/Compat/PROBE.md）で確かめる。
    /// https://learn.microsoft.com/ja-jp/office/vba/language/reference/user-interface-help/format-function-visual-basic-for-applications
    /// </summary>
    [TestClass]
    public class FormatVbMatchTests
    {
        private static Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // 名前の付いた数値の書式
        [TestMethod]
        [DataRow("Format(1234.567,\"Fixed\")", "1234.57")]
        [DataRow("Format(0.5,\"Fixed\")", "0.50")]
        [DataRow("Format(1234.567,\"Standard\")", "1,234.57")]
        [DataRow("Format(0.123,\"Percent\")", "12.30%")]
        [DataRow("Format(1234.5,\"Scientific\")", "1.23E+03")]
        [DataRow("Format(1234.5,\"General Number\")", "1234.5")]
        [DataRow("Format(\"1,234.5\",\"General Number\")", "1234.5")]
        [DataRow("Format(1234.5,\"Currency\")", "\\1,235")]
        [DataRow("Format(-1234.5,\"Currency\")", "-\\1,235")]
        [DataRow("Format(0,\"Yes/No\")", "No")]
        [DataRow("Format(2,\"Yes/No\")", "Yes")]
        [DataRow("Format(0,\"True/False\")", "False")]
        [DataRow("Format(-1,\"True/False\")", "True")]
        [DataRow("Format(0,\"On/Off\")", "Off")]
        [DataRow("Format(1,\"On/Off\")", "On")]
        [DataRow("Format(1234.567,\"fixed\")", "1234.57")]   // 名前の大文字・小文字は区別しない
        [DataRow("Format(\"abc\",\"Fixed\")", "abc")]        // 数値でない文字列はそのまま
        public void NamedNumberFormat(string expr, string expected)
        {
            Assert.AreEqual(expected, Create().GetValueAsString(expr));
        }

        // 名前の付いた日付・時刻の書式（日本語版の Windows の地域の設定）
        [TestMethod]
        [DataRow("Format(\"2012/11/25 13:04:05\",\"General Date\")", "2012/11/25 13:04:05")]
        [DataRow("Format(\"2012/11/25\",\"General Date\")", "2012/11/25")]
        [DataRow("Format(\"13:04:05\",\"General Date\")", "13:04:05")]
        [DataRow("Format(\"2012/11/25\",\"Long Date\")", "2012年11月25日")]
        [DataRow("Format(\"2012/11/25\",\"Short Date\")", "2012/11/25")]
        [DataRow("Format(\"2012/11/25\",\"Medium Date\")", "12/11/25")]
        [DataRow("Format(\"13:04:05\",\"Long Time\")", "13:04:05")]
        [DataRow("Format(\"13:04:05\",\"Medium Time\")", "01:04 午後")]
        [DataRow("Format(\"13:04:05\",\"Short Time\")", "13:04")]
        [DataRow("Format(41238,\"Long Date\")", "2012年11月25日")]   // 数値は日付のシリアル値
        [DataRow("Format(\"abc\",\"Long Date\")", "abc")]            // 日付でない文字列はそのまま
        public void NamedDateFormat(string expr, string expected)
        {
            Assert.AreEqual(expected, Create().GetValueAsString(expr));
        }

        // 日付・時刻の書式文字
        [TestMethod]
        [DataRow("yyyy年m月d日", "2012年11月25日")]
        [DataRow("yyyy/mm/dd", "2012/11/25")]
        [DataRow("yy/m/d", "12/11/25")]
        [DataRow("YYYY-MM-DD", "2012-11-25")]       // 書式文字の大文字・小文字は区別しない
        [DataRow("aaa", "日")]
        [DataRow("aaaa", "日曜日")]
        [DataRow("ddd", "日")]
        [DataRow("dddd", "日曜日")]
        [DataRow("ddddd", "2012/11/25")]
        [DataRow("dddddd", "2012年11月25日")]
        [DataRow("mmm", "11")]
        [DataRow("mmmm", "11月")]
        [DataRow("ggge年", "平成24年")]
        [DataRow("gge", "平24")]
        [DataRow("ge", "H24")]
        [DataRow("ee", "24")]
        [DataRow("q", "4")]
        [DataRow("y", "330")]
        [DataRow("w", "1")]
        [DataRow("ww", "48")]
        [DataRow("hh:mm:ss", "13:04:05")]           // h の後の m は分
        [DataRow("h:n:s", "13:4:5")]
        [DataRow("h時m分", "13時4分")]
        [DataRow("hh:nn AM/PM", "01:04 PM")]
        [DataRow("hh:nn am/pm", "01:04 pm")]
        [DataRow("h A/P", "1 P")]
        [DataRow("h AMPM", "1 午後")]
        [DataRow("ttttt", "13:04:05")]
        [DataRow("c", "2012/11/25 13:04:05")]
        [DataRow("\"y\"yyyy", "y2012")]             // 引用符の中は文字のまま
        [DataRow("\\yyyyy", "y2012")]               // \ の次の 1 文字は文字のまま
        public void UserDefinedDateFormat(string format, string expected)
        {
            var exp = Create();
            exp.SetVariableAsString("fmt", format);
            Assert.AreEqual(expected, exp.GetValueAsString("Format(\"2012/11/25 13:04:05\",fmt)"));
        }

        [TestMethod]
        public void DateFormat_Reiwa()
        {
            Assert.AreEqual("令和1年5月1日", Create().GetValueAsString("Format(\"2019/05/01\",\"ggge年m月d日\")"));
            Assert.AreEqual("平成31年4月30日", Create().GetValueAsString("Format(\"2019/04/30\",\"ggge年m月d日\")"));
        }

        [TestMethod]
        public void DateFormat_SerialNumber()
        {
            // 数値の文字列は日付のシリアル値として書く
            Assert.AreEqual("2012/11/25", Create().GetValueAsString("Format(41238,\"yyyy/mm/dd\")"));
            Assert.AreEqual("12:00", Create().GetValueAsString("Format(0.5,\"hh:nn\")"));
        }

        [TestMethod]
        public void DateFormat_NotDate_ReturnsAsIs()
        {
            Assert.AreEqual("abc", Create().GetValueAsString("Format(\"abc\",\"yyyy/mm/dd\")"));
        }

        [TestMethod]
        public void NumberFormat_DateCharsAfterNumberTokenAreLiteral()
        {
            // 最初の書式文字で書式の種類が決まる。数値の書式の後の d・h などは文字のまま
            Assert.AreEqual("12d", Create().GetValueAsString("Format(12,\"0d\")"));
        }

        [TestMethod]
        public void EmptyFormat_ReturnsStringAsIs()
        {
            // 書式が空なら、文字列の引数をそのまま返す
            Assert.AreEqual("01.50", Create().GetValueAsString("Format(\"01.50\",\"\")"));
        }

        [TestMethod]
        public void StringFormat_CaseConversionOnNumber()
        {
            // < と > は文字列の書式。数値の文字列にも、文字列として当てる
            Assert.AreEqual("123", Create().GetValueAsString("Format(123,\"<\")"));
        }
    }
}
