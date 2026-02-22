using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;

namespace SRCCore.Tests
{
    [TestClass]
    public class SRCDataErrorTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void DataErrors_Initially_Empty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void HasDataError_Initially_False()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.HasDataError);
        }

        [TestMethod]
        public void AddDataError_SingleError_HasDataErrorTrue()
        {
            var src = CreateSRC();
            var error = new InvalidSrcData("エラーメッセージ", "test.txt", 1, "line content", "data1");

            src.AddDataError(error);

            Assert.IsTrue(src.HasDataError);
            Assert.AreEqual(1, src.DataErrors.Count);
        }

        [TestMethod]
        public void AddDataError_SingleError_ContainsError()
        {
            var src = CreateSRC();
            var error = new InvalidSrcData("エラーメッセージ", "test.txt", 1, "line content", "data1");

            src.AddDataError(error);

            Assert.AreSame(error, src.DataErrors[0]);
        }

        [TestMethod]
        public void AddDataError_MultipleErrors_CountIncreases()
        {
            var src = CreateSRC();
            var error1 = new InvalidSrcData("msg1", "file1.txt", 1, "buf1", "d1");
            var error2 = new InvalidSrcData("msg2", "file2.txt", 2, "buf2", "d2");
            var error3 = new InvalidSrcData("msg3", "file3.txt", 3, "buf3", "d3");

            src.AddDataError(error1);
            src.AddDataError(error2);
            src.AddDataError(error3);

            Assert.AreEqual(3, src.DataErrors.Count);
        }

        [TestMethod]
        public void ClearDataError_AfterAddingErrors_ResetsToEmpty()
        {
            var src = CreateSRC();
            src.AddDataError(new InvalidSrcData("msg1", "file1.txt", 1, "buf1", "d1"));
            src.AddDataError(new InvalidSrcData("msg2", "file2.txt", 2, "buf2", "d2"));

            src.ClearDataError();

            Assert.AreEqual(0, src.DataErrors.Count);
            Assert.IsFalse(src.HasDataError);
        }

        [TestMethod]
        public void AddDataError_PreservesAllProperties()
        {
            var src = CreateSRC();
            var error = new InvalidSrcData("テストエラー", "scenario.evt", 42, "invalid line", "dataName");

            src.AddDataError(error);

            var stored = src.DataErrors[0];
            Assert.AreEqual("テストエラー", stored.msg);
            Assert.AreEqual("scenario.evt", stored.fname);
            Assert.AreEqual(42, stored.line_num);
            Assert.AreEqual("invalid line", stored.line_buf);
            Assert.AreEqual("dataName", stored.dname);
        }

        [TestMethod]
        public void ScenarioFileName_Default_EmptyString()
        {
            var src = CreateSRC();
            Assert.AreEqual("", src.ScenarioFileName);
        }

        [TestMethod]
        public void ScenarioPath_Default_EmptyString()
        {
            var src = CreateSRC();
            Assert.AreEqual("", src.ScenarioPath);
        }

        [TestMethod]
        public void Money_Default_Zero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.Money);
        }
    }
}
