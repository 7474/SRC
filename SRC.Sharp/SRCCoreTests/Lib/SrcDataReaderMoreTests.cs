using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System.Text;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// SrcDataReader の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcDataReaderMoreTests
    {
        private SrcDataReader CreateReader(string content, string fname = "test.txt")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new SrcDataReader(fname, stream);
        }

        // ──────────────────────────────────────────────
        // コメントのみのファイル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_OnlyComments_ReturnsEmptyAtEof()
        {
            // コメント行のみで終わるとき、GetLine はループを抜けて "" を返す
            using var reader = CreateReader("# comment1\n# comment2\n# comment3");
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
        // Raw / RawComment / RawData の分離
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RawData_DoesNotContainCommentLines()
        {
            using var reader = CreateReader("# comment\ndata line");
            reader.GetLine();
            Assert.IsFalse(reader.RawData.Contains("# comment"));
            Assert.IsTrue(reader.RawData.Contains("data line"));
        }

        [TestMethod]
        public void RawComment_ContainsCommentLines_NotDataLines()
        {
            using var reader = CreateReader("# comment\ndata line");
            reader.GetLine();
            Assert.IsTrue(reader.RawComment.Contains("# comment"));
            Assert.IsFalse(reader.RawComment.Contains("data line"));
        }

        [TestMethod]
        public void Raw_ContainsAllLines_IncludingComments()
        {
            using var reader = CreateReader("# comment\ndata line");
            reader.GetLine();
            Assert.IsTrue(reader.Raw.Contains("# comment"));
            Assert.IsTrue(reader.Raw.Contains("data line"));
        }

        [TestMethod]
        public void RawComment_ContainsEmptyLines()
        {
            // 空行は _commentBuffer に追加される
            using var reader = CreateReader("\ndata");
            reader.GetLine(); // reads empty line (returns "")
            Assert.IsTrue(reader.RawComment.Contains("\n") || reader.RawComment.Length > 0);
        }

        [TestMethod]
        public void RawData_DoesNotContainEmptyLines()
        {
            // 空行は _rawDataBuffer には追加されない
            using var reader = CreateReader("\ndata");
            reader.GetLine(); // empty line
            Assert.AreEqual("", reader.RawData.Trim());
        }

        // ──────────────────────────────────────────────
        // Clear メソッドの独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearRaw_DoesNotAffectRawComment()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            reader.ClearRaw();
            // RawComment はクリアされない
            Assert.IsTrue(reader.RawComment.Contains("# comment"));
        }

        [TestMethod]
        public void ClearRawComment_DoesNotAffectRawData()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            reader.ClearRawComment();
            Assert.IsTrue(reader.RawData.Contains("data"));
        }

        [TestMethod]
        public void ClearRawData_DoesNotAffectRaw()
        {
            using var reader = CreateReader("data");
            reader.GetLine();
            reader.ClearRawData();
            Assert.IsTrue(reader.Raw.Contains("data"));
        }

        // ──────────────────────────────────────────────
        // LastLine – 特殊ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LastLine_WithFullWidthComma_IsReplaced()
        {
            // LastLine は全角カンマを半角カンマ+スペースに変換した値を保持する
            using var reader = CreateReader("a，b");
            reader.GetLine();
            Assert.AreEqual("a, b", reader.LastLine);
        }

        [TestMethod]
        public void LastLine_InlineCommentRemoved_LineContentOnly()
        {
            // LastLine にはコメント削除前のラインバッファが含まれないことを確認
            // GetLine は "data " を返し、LastLine も "data " になる
            using var reader = CreateReader("data // comment");
            var line = reader.GetLine();
            Assert.AreEqual("data ", reader.LastLine);
            Assert.AreEqual(line, reader.LastLine);
        }

        // ──────────────────────────────────────────────
        // 行連結と LineNumber
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_ContinuationWithComment_CountsAllLines()
        {
            // "_" 行連結でも各行はカウントされる
            using var reader = CreateReader("a_\nb_\nc");
            reader.GetLine();
            Assert.AreEqual(3, reader.LineNumber);
        }

        [TestMethod]
        public void GetLine_ContinuationMixedWithComment_SkipsComments()
        {
            // コメント行は連結をスキップせずに読み飛ばされる
            using var reader = CreateReader("# skip\ndata");
            var line = reader.GetLine();
            Assert.AreEqual("data", line);
            Assert.AreEqual(2, reader.LineNumber);
        }

        // ──────────────────────────────────────────────
        // 複数の GetLine 呼び出し後の状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetLine_AfterEof_ReturnsEmpty()
        {
            using var reader = CreateReader("only");
            reader.GetLine();
            // すでに EOT になっているので再度呼んでも "" が返る
            var line = reader.GetLine();
            Assert.AreEqual("", line);
        }

        [TestMethod]
        public void Raw_AccumulatesAcrossMultipleGetLineCalls()
        {
            using var reader = CreateReader("line1\nline2\nline3");
            reader.GetLine();
            reader.GetLine();
            reader.GetLine();
            Assert.IsTrue(reader.Raw.Contains("line1"));
            Assert.IsTrue(reader.Raw.Contains("line2"));
            Assert.IsTrue(reader.Raw.Contains("line3"));
        }

        [TestMethod]
        public void ClearRaw_ThenReadAgain_OnlyNewContent()
        {
            using var reader = CreateReader("line1\nline2");
            reader.GetLine();
            reader.ClearRaw();
            reader.GetLine();
            Assert.IsFalse(reader.Raw.Contains("line1"));
            Assert.IsTrue(reader.Raw.Contains("line2"));
        }

        // ──────────────────────────────────────────────
        // InvalidDataException のリスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InvalidDataException_ExceptionListHasOneEntry()
        {
            using var reader = CreateReader("bad data\nmore bad");
            reader.GetLine();
            reader.GetLine();
            var ex = reader.InvalidDataException("error", "unit");
            Assert.AreEqual(1, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void InvalidData_FnameMatchesConstructorArg()
        {
            using var reader = CreateReader("data", "myfile.dat");
            reader.GetLine();
            var err = reader.InvalidData("msg", "d");
            Assert.AreEqual("myfile.dat", err.fname);
        }
    }
}
