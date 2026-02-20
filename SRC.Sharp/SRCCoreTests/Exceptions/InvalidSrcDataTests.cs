using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using System.Collections.Generic;

namespace SRCCore.Exceptions.Tests
{
    [TestClass]
    public class InvalidSrcDataTests
    {
        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var data = new InvalidSrcData("error message", "file.txt", 42, "bad line", "data name");

            Assert.AreEqual("error message", data.msg);
            Assert.AreEqual("file.txt", data.fname);
            Assert.AreEqual(42, data.line_num);
            Assert.AreEqual("bad line", data.line_buf);
            Assert.AreEqual("data name", data.dname);
        }

        [TestMethod]
        public void Constructor_AcceptsEmptyStrings()
        {
            var data = new InvalidSrcData("", "", 0, "", "");

            Assert.AreEqual("", data.msg);
            Assert.AreEqual("", data.fname);
            Assert.AreEqual(0, data.line_num);
            Assert.AreEqual("", data.line_buf);
            Assert.AreEqual("", data.dname);
        }

        [TestMethod]
        public void Constructor_AcceptsNullStrings()
        {
            var data = new InvalidSrcData(null, null, 0, null, null);

            Assert.IsNull(data.msg);
            Assert.IsNull(data.fname);
            Assert.IsNull(data.line_buf);
            Assert.IsNull(data.dname);
        }
    }

    [TestClass]
    public class InvalidSrcDataExceptionTests
    {
        [TestMethod]
        public void Constructor_WithList_SetsMessageFromFirstItem()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("first error", "file1.txt", 1, "line1", "data1"),
                new InvalidSrcData("second error", "file2.txt", 2, "line2", "data2"),
            };

            var ex = new InvalidSrcDataException(list);

            Assert.AreEqual("first error", ex.Message);
            Assert.AreEqual(2, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void Constructor_WithEmptyList_SetsDefaultMessage()
        {
            var list = new List<InvalidSrcData>();

            var ex = new InvalidSrcDataException(list);

            Assert.AreEqual("InvalidSrcData", ex.Message);
            Assert.AreEqual(0, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void Constructor_WithMessageAndList_SetsCustomMessage()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("detail", "file.txt", 1, "line", "data"),
            };

            var ex = new InvalidSrcDataException("custom message", list);

            Assert.AreEqual("custom message", ex.Message);
            Assert.AreEqual(1, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void InvalidDataList_IsIndependentCopy()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("error", "file.txt", 1, "line", "data"),
            };

            var ex = new InvalidSrcDataException(list);
            list.Clear();

            // The exception should still have the original data
            Assert.AreEqual(1, ex.InvalidDataList.Count);
        }
    }
}
