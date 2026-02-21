using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System.Text;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// SrcEveReader の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcEveReaderMoreTests
    {
        private SrcEveReader CreateReader(string content, string fname = "test.eve")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new SrcEveReader(fname, stream);
        }

        // ──────────────────────────────────────────────
        // LastLine は SrcEveReader.GetLine では更新されない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LastLine_NeverUpdatedByGetLine_RemainsNull()
        {
            // SrcEveReader の GetLine は LastLine に代入しないため、
            // 読み込み後も null のまま
            using var reader = CreateReader("some data");
            reader.GetLine();
            Assert.IsNull(reader.LastLine);
        }

        [TestMethod]
        public void LastLine_BeforeRead_IsNull()
        {
            using var reader = CreateReader("data");
            Assert.IsNull(reader.LastLine);
        }

        [TestMethod]
        public void InvalidData_WithNullLastLine_DoesNotThrow()
        {
            // LastLine が null でも InvalidData はクラッシュしない
            using var reader = CreateReader("bad line");
            reader.GetLine();
            var err = reader.InvalidData("msg", "dname");
            Assert.IsNull(err.line_buf); // line_buf は LastLine に依存
        }

        // ──────────────────────────────────────────────
        // コメントのみのファイル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_OnlyCommentLines_ReturnsEmptyAtEof()
        {
            using var reader = CreateReader("# comment1\n# comment2");
            var line = reader.GetLine();
            Assert.AreEqual("", line);
            Assert.IsTrue(reader.EOT);
        }

        [TestMethod]
        public void GetLine_CommentsOnlyFile_LineNumberCountsAll()
        {
            using var reader = CreateReader("# a\n# b\n# c");
            reader.GetLine();
            Assert.AreEqual(3, reader.LineNumber);
        }

        // ──────────────────────────────────────────────
        // 全角カンマは置換されない (SrcDataReader との違い)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_FullWidthComma_NotReplaced()
        {
            // SrcEveReader は全角カンマを半角に変換しない
            using var reader = CreateReader("a，b，c");
            var line = reader.GetLine();
            Assert.IsTrue(line.Contains("，"), "全角カンマはそのまま保持される");
        }

        // ──────────────────────────────────────────────
        // 連結行 + インラインコメント
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_ContinuationThenInlineComment_CommentRemoved()
        {
            // 連結行の次行にインラインコメントがある場合、コメントは除去される
            using var reader = CreateReader("first_\nsecond // inline comment");
            var line = reader.GetLine();
            Assert.AreEqual("firstsecond ", line);
        }

        [TestMethod]
        public void GetLine_InlineCommentOnly_ReturnsEmpty()
        {
            using var reader = CreateReader("// only comment");
            var line = reader.GetLine();
            Assert.AreEqual("", line);
        }

        // ──────────────────────────────────────────────
        // 空行の扱い
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_MultipleEmptyLines_ReturnsEmptyPerCall()
        {
            using var reader = CreateReader("\n\ndata");
            var line1 = reader.GetLine();
            var line2 = reader.GetLine();
            var line3 = reader.GetLine();
            Assert.AreEqual("", line1);
            Assert.AreEqual("", line2);
            Assert.AreEqual("data", line3);
        }

        // ──────────────────────────────────────────────
        // EOF 後の GetLine
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_AfterEof_ReturnsEmpty()
        {
            using var reader = CreateReader("only");
            reader.GetLine();
            var line = reader.GetLine();
            Assert.AreEqual("", line);
        }

        // ──────────────────────────────────────────────
        // LineNumber のカウント
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LineNumber_ContinuationLines_CountsAllPhysicalLines()
        {
            using var reader = CreateReader("a_\nb_\nc");
            reader.GetLine();
            Assert.AreEqual(3, reader.LineNumber);
        }

        [TestMethod]
        public void LineNumber_InitialValue_IsZero()
        {
            using var reader = CreateReader("data");
            Assert.AreEqual(0, reader.LineNumber);
        }

        // ──────────────────────────────────────────────
        // HasMore / EOT の挙動
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMore_AfterEmptyLineRead_TrueIfMoreContent()
        {
            using var reader = CreateReader("\nmore content");
            reader.GetLine(); // empty line
            Assert.IsTrue(reader.HasMore);
        }

        [TestMethod]
        public void EOT_IsFalseHasMoreIsTrue_WhenContentAvailable()
        {
            using var reader = CreateReader("content");
            Assert.IsFalse(reader.EOT);
            Assert.IsTrue(reader.HasMore);
        }

        // ──────────────────────────────────────────────
        // InvalidDataException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InvalidDataException_ExceptionListHasOneEntry()
        {
            using var reader = CreateReader("data");
            reader.GetLine();
            var ex = reader.InvalidDataException("msg", "d");
            Assert.AreEqual(1, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void InvalidData_FnameMatchesConstructorArg()
        {
            using var reader = CreateReader("data", "events.eve");
            reader.GetLine();
            var err = reader.InvalidData("msg", "d");
            Assert.AreEqual("events.eve", err.fname);
        }
    }
}
