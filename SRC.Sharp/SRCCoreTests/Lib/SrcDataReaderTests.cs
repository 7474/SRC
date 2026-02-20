using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System.Text;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class SrcDataReaderTests
    {
        private SrcDataReader CreateReader(string content, string fname = "test.txt")
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new SrcDataReader(fname, stream);
        }

        [TestMethod]
        public void FileName_ReturnsSpecifiedFileName()
        {
            using var reader = CreateReader("", "data.txt");
            Assert.AreEqual("data.txt", reader.FileName);
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
            using var reader = CreateReader("hello");
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
            using var reader = CreateReader("# comment\ndata line");
            var line = reader.GetLine();
            Assert.AreEqual("data line", line);
        }

        [TestMethod]
        public void GetLine_RemovesInlineComments()
        {
            using var reader = CreateReader("data // this is a comment");
            var line = reader.GetLine();
            Assert.AreEqual("data ", line);
        }

        [TestMethod]
        public void GetLine_JoinsLinesWithUnderscore()
        {
            using var reader = CreateReader("first_\nsecond");
            var line = reader.GetLine();
            Assert.AreEqual("firstsecond", line);
        }

        [TestMethod]
        public void GetLine_JoinsMultipleLinesWithUnderscore()
        {
            using var reader = CreateReader("a_\nb_\nc");
            var line = reader.GetLine();
            Assert.AreEqual("abc", line);
        }

        [TestMethod]
        public void GetLine_ReplacesFullWidthCommaWithHalfWidth()
        {
            using var reader = CreateReader("a，b，c");
            var line = reader.GetLine();
            Assert.AreEqual("a, b, c", line);
        }

        [TestMethod]
        public void GetLine_ReturnsEmptyStringForEmptyLine()
        {
            using var reader = CreateReader("\ndata");
            var line = reader.GetLine();
            Assert.AreEqual("", line);
        }

        [TestMethod]
        public void GetLine_TracksLineNumber()
        {
            using var reader = CreateReader("line1\nline2\nline3");
            Assert.AreEqual(0, reader.LineNumber);
            reader.GetLine();
            Assert.AreEqual(1, reader.LineNumber);
            reader.GetLine();
            Assert.AreEqual(2, reader.LineNumber);
        }

        [TestMethod]
        public void GetLine_TracksLineNumberWithComments()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            Assert.AreEqual(2, reader.LineNumber);
        }

        [TestMethod]
        public void GetLine_SetsLastLine()
        {
            using var reader = CreateReader("hello");
            reader.GetLine();
            Assert.AreEqual("hello", reader.LastLine);
        }

        [TestMethod]
        public void GetLine_ReadsMultipleLines()
        {
            using var reader = CreateReader("line1\nline2\nline3");
            Assert.AreEqual("line1", reader.GetLine());
            Assert.AreEqual("line2", reader.GetLine());
            Assert.AreEqual("line3", reader.GetLine());
        }

        [TestMethod]
        public void Raw_AccumulatesAllReadLines()
        {
            using var reader = CreateReader("line1\nline2");
            reader.GetLine();
            reader.GetLine();
            Assert.IsTrue(reader.Raw.Contains("line1"));
            Assert.IsTrue(reader.Raw.Contains("line2"));
        }

        [TestMethod]
        public void ClearRaw_ClearsRawBuffer()
        {
            using var reader = CreateReader("line1\nline2");
            reader.GetLine();
            Assert.IsTrue(reader.Raw.Length > 0);
            reader.ClearRaw();
            Assert.AreEqual("", reader.Raw);
        }

        [TestMethod]
        public void RawComment_AccumulatesCommentLines()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            Assert.IsTrue(reader.RawComment.Contains("# comment"));
        }

        [TestMethod]
        public void ClearRawComment_ClearsCommentBuffer()
        {
            using var reader = CreateReader("# comment\ndata");
            reader.GetLine();
            reader.ClearRawComment();
            Assert.AreEqual("", reader.RawComment);
        }

        [TestMethod]
        public void RawData_AccumulatesDataLines()
        {
            using var reader = CreateReader("data line");
            reader.GetLine();
            Assert.IsTrue(reader.RawData.Contains("data line"));
        }

        [TestMethod]
        public void ClearRawData_ClearsDataBuffer()
        {
            using var reader = CreateReader("data line");
            reader.GetLine();
            reader.ClearRawData();
            Assert.AreEqual("", reader.RawData);
        }

        [TestMethod]
        public void InvalidData_CreatesInvalidSrcData()
        {
            using var reader = CreateReader("bad data");
            reader.GetLine();
            var error = reader.InvalidData("error msg", "data name");
            Assert.AreEqual("error msg", error.msg);
            Assert.AreEqual("test.txt", error.fname);
            Assert.AreEqual(1, error.line_num);
            Assert.AreEqual("data name", error.dname);
        }

        [TestMethod]
        public void InvalidDataException_CreatesException()
        {
            using var reader = CreateReader("bad data");
            reader.GetLine();
            var ex = reader.InvalidDataException("error msg", "data name");
            Assert.IsNotNull(ex);
            Assert.AreEqual(1, ex.InvalidDataList.Count);
            Assert.AreEqual("error msg", ex.InvalidDataList[0].msg);
        }

        [TestMethod]
        public void GetLine_TrimsWhitespace()
        {
            using var reader = CreateReader("  trimmed  ");
            var line = reader.GetLine();
            Assert.AreEqual("trimmed", line);
        }

        [TestMethod]
        public void GetLine_SkipsMultipleCommentLines()
        {
            using var reader = CreateReader("# comment1\n# comment2\n# comment3\ndata");
            var line = reader.GetLine();
            Assert.AreEqual("data", line);
        }
    }
}
