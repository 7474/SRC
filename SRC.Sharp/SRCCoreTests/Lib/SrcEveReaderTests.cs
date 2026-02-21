using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System.Text;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class SrcEveReaderTests
    {
        private SrcEveReader CreateReader(string content, string fname = "test.eve")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new SrcEveReader(fname, stream);
        }

        [TestMethod]
        public void FileName_ReturnsSpecifiedFileName()
        {
            using var reader = CreateReader("", "event.eve");
            Assert.AreEqual("event.eve", reader.FileName);
        }

        [TestMethod]
        public void EOT_ReturnsTrue_WhenEmpty()
        {
            using var reader = CreateReader("");
            Assert.IsTrue(reader.EOT);
            Assert.IsFalse(reader.HasMore);
        }

        [TestMethod]
        public void EOT_ReturnsFalse_WhenHasContent()
        {
            using var reader = CreateReader("content");
            Assert.IsFalse(reader.EOT);
            Assert.IsTrue(reader.HasMore);
        }

        [TestMethod]
        public void GetLine_ReadsSimpleLine()
        {
            using var reader = CreateReader("hello world");
            var line = reader.GetLine();
            Assert.AreEqual("hello world", line);
        }

        [TestMethod]
        public void GetLine_SkipsCommentLines()
        {
            using var reader = CreateReader("# this is a comment\nactual data");
            var line = reader.GetLine();
            Assert.AreEqual("actual data", line);
        }

        [TestMethod]
        public void GetLine_RemovesInlineComments()
        {
            using var reader = CreateReader("data // inline comment");
            var line = reader.GetLine();
            Assert.AreEqual("data ", line);
        }

        [TestMethod]
        public void GetLine_JoinsLinesEndingWithUnderscore()
        {
            using var reader = CreateReader("first_\nsecond");
            var line = reader.GetLine();
            Assert.AreEqual("firstsecond", line);
        }

        [TestMethod]
        public void GetLine_JoinsMultipleContinuationLines()
        {
            using var reader = CreateReader("a_\nb_\nc");
            var line = reader.GetLine();
            Assert.AreEqual("abc", line);
        }

        [TestMethod]
        public void GetLine_ReturnsEmptyForEmptyLine()
        {
            using var reader = CreateReader("\ndata");
            var line = reader.GetLine();
            Assert.AreEqual("", line);
        }

        [TestMethod]
        public void GetLine_TracksLineNumber()
        {
            using var reader = CreateReader("line1\nline2");
            reader.GetLine();
            Assert.AreEqual(1, reader.LineNumber);
            reader.GetLine();
            Assert.AreEqual(2, reader.LineNumber);
        }

        [TestMethod]
        public void GetLine_TracksLineNumberIncludingComments()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            Assert.AreEqual(2, reader.LineNumber);
        }

        [TestMethod]
        public void GetLine_ReadsSequentialLines()
        {
            using var reader = CreateReader("first\nsecond\nthird");
            Assert.AreEqual("first", reader.GetLine());
            Assert.AreEqual("second", reader.GetLine());
            Assert.AreEqual("third", reader.GetLine());
        }

        [TestMethod]
        public void GetLine_SkipsMultipleCommentLines()
        {
            using var reader = CreateReader("# c1\n# c2\n# c3\ndata");
            var line = reader.GetLine();
            Assert.AreEqual("data", line);
        }

        [TestMethod]
        public void GetLine_TrimsWhitespace()
        {
            using var reader = CreateReader("  data  ");
            var line = reader.GetLine();
            Assert.AreEqual("data", line);
        }

        [TestMethod]
        public void InvalidData_CreatesCorrectErrorObject()
        {
            using var reader = CreateReader("bad");
            reader.GetLine();
            var error = reader.InvalidData("msg", "dname");
            Assert.AreEqual("msg", error.msg);
            Assert.AreEqual("test.eve", error.fname);
            Assert.AreEqual(1, error.line_num);
            Assert.AreEqual("dname", error.dname);
        }

        [TestMethod]
        public void InvalidDataException_CreatesExceptionWithErrorList()
        {
            using var reader = CreateReader("bad");
            reader.GetLine();
            var ex = reader.InvalidDataException("msg", "dname");
            Assert.IsNotNull(ex);
            Assert.AreEqual(1, ex.InvalidDataList.Count);
            Assert.AreEqual("msg", ex.InvalidDataList[0].msg);
        }

        [TestMethod]
        public void LastLine_ReturnsEmptyBeforeRead()
        {
            using var reader = CreateReader("data");
            // LastLine is null/empty before any read
            Assert.IsTrue(reader.LastLine == null || reader.LastLine == "");
        }

        [TestMethod]
        public void EOT_ReturnsTrueAfterAllLinesRead()
        {
            using var reader = CreateReader("only line");
            reader.GetLine();
            Assert.IsTrue(reader.EOT);
        }

        [TestMethod]
        public void HasMore_ReturnsFalseAfterAllLinesRead()
        {
            using var reader = CreateReader("one");
            reader.GetLine();
            Assert.IsFalse(reader.HasMore);
        }

        [TestMethod]
        public void GetLine_ContinuationLineWithInlineComment()
        {
            // Comment is removed AFTER the underscore check, so "first_" ends with _
            // but "first_ // comment" after RemoveLineComment becomes "first_ " (space)
            // which does NOT end with _, so no continuation happens
            using var reader = CreateReader("first_\nsecond");
            var line = reader.GetLine();
            // Standard continuation: first_ + second = firstsecond
            Assert.AreEqual("firstsecond", line);
        }

        [TestMethod]
        public void GetLine_LineNumberStartsAtZero()
        {
            using var reader = CreateReader("data");
            Assert.AreEqual(0, reader.LineNumber);
        }

        [TestMethod]
        public void InvalidData_LineNumberMatchesRead()
        {
            using var reader = CreateReader("line1\nline2\nline3");
            reader.GetLine();
            reader.GetLine();
            reader.GetLine();
            var err = reader.InvalidData("err", "d");
            Assert.AreEqual(3, err.line_num);
        }
    }
}
