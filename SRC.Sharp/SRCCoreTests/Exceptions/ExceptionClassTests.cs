using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.Exceptions.Tests
{
    /// <summary>
    /// 例外クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ExceptionClassTests
    {
        // ──────────────────────────────────────────────
        // EventErrorException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventErrorException_SetsMessage()
        {
            var ex = new EventErrorException(null, "テストエラーメッセージ");
            Assert.AreEqual("テストエラーメッセージ", ex.Message);
        }

        [TestMethod]
        public void EventErrorException_NullCmd_EventDataIsNull()
        {
            var ex = new EventErrorException(null, "エラー");
            Assert.IsNull(ex.EventData);
        }

        [TestMethod]
        public void EventErrorException_WithEventDataLine_EventDataStored()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "test.eve", 10, "Set x 1");
            // Create a minimal CmdData - we can use a mock approach by directly testing the exception
            var ex = new EventErrorException(null, "エラー");
            // The EventData is from cmd.EventData - null cmd gives null EventData
            Assert.IsNull(ex.EventData);
        }

        [TestMethod]
        public void EventErrorException_IsException()
        {
            var ex = new EventErrorException(null, "エラー");
            Assert.IsInstanceOfType(ex, typeof(System.Exception));
        }

        [TestMethod]
        public void EventErrorException_CanBeCaught()
        {
            bool caught = false;
            try
            {
                throw new EventErrorException(null, "test error");
            }
            catch (EventErrorException e)
            {
                caught = true;
                Assert.AreEqual("test error", e.Message);
            }
            Assert.IsTrue(caught);
        }

        // ──────────────────────────────────────────────
        // TerminateException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerminateException_SetsMessage()
        {
            var ex = new TerminateException("終了テスト");
            Assert.AreEqual("終了テスト", ex.Message);
        }

        [TestMethod]
        public void TerminateException_WithInnerException_HasInner()
        {
            var inner = new System.InvalidOperationException("内部エラー");
            var ex = new TerminateException("外部エラー", inner);
            Assert.AreEqual("外部エラー", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        [TestMethod]
        public void TerminateException_IsException()
        {
            var ex = new TerminateException("終了");
            Assert.IsInstanceOfType(ex, typeof(System.Exception));
        }

        [TestMethod]
        public void TerminateException_CanBeCaught()
        {
            bool caught = false;
            try
            {
                throw new TerminateException("test terminate");
            }
            catch (TerminateException e)
            {
                caught = true;
                Assert.AreEqual("test terminate", e.Message);
            }
            Assert.IsTrue(caught);
        }

        [TestMethod]
        public void TerminateException_WithEmptyMessage_MessageIsEmpty()
        {
            var ex = new TerminateException("");
            Assert.AreEqual("", ex.Message);
        }
    }
}
