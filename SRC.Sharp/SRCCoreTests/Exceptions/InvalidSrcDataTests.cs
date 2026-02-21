using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using System;
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

        [TestMethod]
        public void Constructor_SingleItem_MessageEqualsItemMsg()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("only error", "f.txt", 5, "l", "d"),
            };
            var ex = new InvalidSrcDataException(list);
            Assert.AreEqual("only error", ex.Message);
        }

        [TestMethod]
        public void Constructor_MultipleItems_PreservesAll()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("e1", "f1.txt", 1, "l1", "d1"),
                new InvalidSrcData("e2", "f2.txt", 2, "l2", "d2"),
                new InvalidSrcData("e3", "f3.txt", 3, "l3", "d3"),
            };
            var ex = new InvalidSrcDataException(list);
            Assert.AreEqual(3, ex.InvalidDataList.Count);
            Assert.AreEqual("e2", ex.InvalidDataList[1].msg);
            Assert.AreEqual("e3", ex.InvalidDataList[2].msg);
        }
    }

    [TestClass]
    public class EventErrorExceptionTests
    {
        [TestMethod]
        public void Constructor_WithNullCmd_SetsMessageAndNullEventData()
        {
            var ex = new SRCCore.Exceptions.EventErrorException(null, "test error");
            Assert.AreEqual("test error", ex.Message);
            Assert.IsNull(ex.EventData);
        }

        [TestMethod]
        public void Constructor_IsException()
        {
            var ex = new SRCCore.Exceptions.EventErrorException(null, "msg");
            Assert.IsInstanceOfType(ex, typeof(Exception));
        }
    }

    [TestClass]
    public class TerminateExceptionTests
    {
        [TestMethod]
        public void Constructor_WithMessage_SetsMessage()
        {
            var ex = new SRCCore.Exceptions.TerminateException("terminate now");
            Assert.AreEqual("terminate now", ex.Message);
        }

        [TestMethod]
        public void Constructor_WithMessageAndInner_BothSet()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new SRCCore.Exceptions.TerminateException("outer", inner);
            Assert.AreEqual("outer", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        [TestMethod]
        public void TerminateException_IsException()
        {
            var ex = new SRCCore.Exceptions.TerminateException("msg");
            Assert.IsInstanceOfType(ex, typeof(Exception));
        }
    }
}
