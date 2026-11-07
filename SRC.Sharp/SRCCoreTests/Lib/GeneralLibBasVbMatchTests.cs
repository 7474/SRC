using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using System;
using System.IO;
using System.Text;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// 元の SRC の GeneralLib.bas と分岐ごとに突き合わせて見つかった違い（7474/SRC#1498）。
    /// 期待値は元の SRC（SRC/SRC_20121125/GeneralLib.bas）の動作から取る。
    /// </summary>
    [TestClass]
    public class GeneralLibBasVbMatchTests
    {
        // ──────────────────────────────────────────────
        // ListLength・ListSplit・ListIndex・ListTail
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListLength_UnclosedParenOrQuote_ReturnsMinusOne()
        {
            // GeneralLib.bas:481 文字列の終わりでクォートや括弧が閉じていなければ -1
            Assert.AreEqual(-1, GeneralLib.ListLength("a (b c"));
            Assert.AreEqual(-1, GeneralLib.ListLength("a \"b c"));
            Assert.AreEqual(-1, GeneralLib.ListLength("a `b c"));
            // GeneralLib.bas:466 閉じ括弧が多すぎても -1
            Assert.AreEqual(-1, GeneralLib.ListLength("a b) c"));
            Assert.AreEqual(3, GeneralLib.ListLength("a (b c) d"));
            Assert.AreEqual(0, GeneralLib.ListLength(" \t "));
        }

        [TestMethod]
        public void ListSplit_UnclosedAtEnd_ReturnsMinusOneWithRestAsLastElement()
        {
            // GeneralLib.bas:578 文字列の終わりで閉じていなければ -1。最後の要素は残り全体
            Assert.AreEqual(-1, GeneralLib.ListSplit("a (b c", out var larray));
            CollectionAssert.AreEqual(new[] { "a", "(b c" }, larray);
        }

        [TestMethod]
        public void ListSplit_TooManyClosingParens_ReturnsMinusOneWithRestAsLastElement()
        {
            // GeneralLib.bas:563 閉じ括弧が多すぎれば、その要素の先頭から文字列の終わりまでを最後の要素にして -1
            Assert.AreEqual(-1, GeneralLib.ListSplit("a b) c d", out var larray));
            CollectionAssert.AreEqual(new[] { "a", "b) c d" }, larray);
        }

        [TestMethod]
        public void ListIndex_TooManyClosingParens_ReturnsEmpty()
        {
            // GeneralLib.bas:325・377 括弧の対応が取れていなければ空文字列
            Assert.AreEqual("", GeneralLib.ListIndex("a) b", 1));
            Assert.AreEqual("", GeneralLib.ListIndex("a) b", 2));
            Assert.AreEqual("a", GeneralLib.ListIndex("a b) c", 1));
            Assert.AreEqual("", GeneralLib.ListIndex("a b) c", 2));
            Assert.AreEqual("", GeneralLib.ListIndex("a b) c", 3));
            // 末尾で閉じていない要素はそのまま返す
            Assert.AreEqual("(b c", GeneralLib.ListIndex("a (b c", 2));
        }

        [TestMethod]
        public void ListTail_ReturnsRestOfOriginalString()
        {
            // GeneralLib.bas:710 ListTail = Mid$(list, i)。要素の間の空白・末尾の空白はそのまま
            Assert.AreEqual("b  c\td ", GeneralLib.ListTail("a  b  c\td ", 2));
            // idx = 1 なら最初の要素から
            Assert.AreEqual("a  b", GeneralLib.ListTail("  a  b", 1));
            Assert.AreEqual("", GeneralLib.ListTail("a b", 0));
            Assert.AreEqual("", GeneralLib.ListTail("a b", 3));
            // 読み飛ばす要素で括弧の対応が取れていなければ空文字列
            Assert.AreEqual("", GeneralLib.ListTail("a) b c", 2));
            // 読み飛ばす要素が閉じていなければ空文字列
            Assert.AreEqual("", GeneralLib.ListTail("(a b", 2));
        }

        // ──────────────────────────────────────────────
        // IsSpace・MaxDbl・MinDbl・LeftPaddedString・RightPaddedString・TrimString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSpace_LooksAtFirstCharacterOnly()
        {
            // GeneralLib.bas:220 空文字列か、Asc(ch) が 9・13・32・160 なら空白
            Assert.IsTrue(GeneralLib.IsSpace(""));
            Assert.IsTrue(GeneralLib.IsSpace(" a"));
            Assert.IsTrue(GeneralLib.IsSpace("\r"));
            Assert.IsFalse(GeneralLib.IsSpace("\n"));
            Assert.IsFalse(GeneralLib.IsSpace("　"));
            Assert.IsFalse(GeneralLib.IsSpace("a "));
        }

        [TestMethod]
        public void MaxDblMinDbl_NaN_ComparesLikeVB()
        {
            // GeneralLib.bas:815 If a > b Then a Else b（NaN との比較は偽）
            Assert.AreEqual(1d, GeneralLib.MaxDbl(double.NaN, 1d));
            Assert.IsTrue(double.IsNaN(GeneralLib.MaxDbl(1d, double.NaN)));
            Assert.AreEqual(1d, GeneralLib.MinDbl(double.NaN, 1d));
            Assert.IsTrue(double.IsNaN(GeneralLib.MinDbl(1d, double.NaN)));
        }

        [TestMethod]
        public void PaddedString_UsesShiftJisByteLength()
        {
            // GeneralLib.bas:834 LenB(StrConv(buf, vbFromUnicode))。半角カナ（｡ など）・タブは 1 バイト
            Assert.AreEqual("  ｡", GeneralLib.LeftPaddedString("｡", 3));
            Assert.AreEqual("\t ", GeneralLib.RightPaddedString("\t", 2));
            Assert.AreEqual(" 日本", GeneralLib.LeftPaddedString("日本", 5));
        }

        [TestMethod]
        public void TrimString_TrimsTabSpaceAndFullWidthSpace()
        {
            // GeneralLib.bas:714 タブ・半角スペース・全角スペースを前後から取り除く
            Assert.AreEqual("a \t b", GeneralLib.TrimString("\t　 a \t b 　\t"));
            Assert.AreEqual("", GeneralLib.TrimString(" \t　"));
            Assert.AreEqual("", GeneralLib.TrimString(null));
        }

        // ──────────────────────────────────────────────
        // Dice
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_WithUnsavedRandomOption_DoesNotUseRandomHistory()
        {
            // GeneralLib.bas:81 Option「乱数系列非保存」なら乱数系列を使わずに Rnd を引く（RndIndex は進まない）
            var src = new SRC();
            try
            {
                src.Expression.DefineGlobalVariable("Option(乱数系列非保存)");
                GeneralLib.RndSeed = 1;
                GeneralLib.RndReset();
                for (var i = 0; i < 10; i++)
                {
                    var d = GeneralLib.Dice(6);
                    Assert.IsTrue(1 <= d && d <= 6);
                }
                Assert.AreEqual(0, GeneralLib.RndIndex);

                // オプションが無ければ乱数系列を使う
                src.Expression.UndefineVariable("Option(乱数系列非保存)");
                GeneralLib.Dice(6);
                Assert.AreEqual(1, GeneralLib.RndIndex);
            }
            finally
            {
                // ほかのテストに影響しないよう、オプションの無い SRC を作り直しておく
                _ = new SRC();
            }
        }

        // ──────────────────────────────────────────────
        // GetLine（SrcDataReader.GetLine）
        // ──────────────────────────────────────────────

        private static SrcDataReader CreateReader(string content)
        {
            return new SrcDataReader("test.txt", new MemoryStream(Encoding.UTF8.GetBytes(content)));
        }

        [TestMethod]
        public void GetLine_IndentedSharp_IsNotComment()
        {
            // GeneralLib.bas:956 Left$(buf, 1) = "#" は空白を取り除く前の行で見る
            using var reader = CreateReader("  #abc\nxyz\n");
            Assert.AreEqual("#abc", reader.GetLine());
        }

        [TestMethod]
        public void GetLine_TrimsAfterRemovingComment()
        {
            // GeneralLib.bas:968 コメントを取り除いてから TrimString
            using var reader = CreateReader("abc, def // comment\n");
            Assert.AreEqual("abc, def", reader.GetLine());
        }

        [TestMethod]
        public void GetLine_UnderscoreFollowedBySpace_IsNotContinuation()
        {
            // GeneralLib.bas:967 行末（空白を取り除く前）が「_」でなければ結合しない
            using var reader = CreateReader("abc _ \ndef\n");
            Assert.AreEqual("abc _", reader.GetLine());
            Assert.AreEqual("def", reader.GetLine());
        }

        [TestMethod]
        public void GetLine_Continuation_TrimsTabsAndFullWidthSpaces()
        {
            using var reader = CreateReader("\tabc,_\n　def\t\n");
            Assert.AreEqual("abc,def", reader.GetLine());
        }

        // ──────────────────────────────────────────────
        // ReadIni（ComposedConfig.GetItem）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReadIni_EmptyScenarioEntry_FallsBackToAppIni()
        {
            // GeneralLib.bas:852 シナリオの Src.ini の値が空なら（ret = 0）本体側の Src.ini から読む
            var root = Path.Combine(Path.GetTempPath(), "srcini-" + Guid.NewGuid().ToString("N"));
            var scenarioDir = Path.Combine(root, "scenario");
            Directory.CreateDirectory(scenarioDir);
            try
            {
                File.WriteAllBytes(Path.Combine(scenarioDir, SrcIni.FileName), ZipNameEncoding.ShiftJis.GetBytes("[BGM]\r\nMap1=\r\nMap2=b.mid\r\n"));
                using var fs = new LocalFileSystem();
                var baseConfig = new LocalFileConfig { AppPath = Path.Combine(root, "app") };
                baseConfig.SetItem("BGM", "Map1", "a.mid");
                baseConfig.SetItem("BGM", "Map2", "c.mid");
                var config = new ComposedConfig(baseConfig, ScenarioConfig.Read(fs, scenarioDir), fs);

                Assert.AreEqual("a.mid", config.GetItem("BGM", "Map1"));
                Assert.AreEqual("b.mid", config.GetItem("BGM", "Map2"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
