using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using System;
using System.Collections.Generic;

namespace SRCCore.Exceptions.Tests
{
    /// <summary>
    /// 例外クラス群の追加ユニットテスト（既存テストで未カバーの項目）
    /// </summary>

    // ══════════════════════════════════════════════════════
    // EventErrorException 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class EventErrorExceptionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // 継承階層
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventErrorException_CanBeCaughtAsBaseException()
        {
            bool caught = false;
            try
            {
                throw new EventErrorException(null, "基底例外として補足");
            }
            catch (Exception ex)
            {
                caught = true;
                Assert.IsTrue(ex is EventErrorException);
                Assert.AreEqual("基底例外として補足", ex.Message);
            }
            Assert.IsTrue(caught);
        }

        // ──────────────────────────────────────────────
        // 各種メッセージのバリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventErrorException_LongMessage_PreservesMessage()
        {
            var longMsg = new string('あ', 500);
            var ex = new EventErrorException(null, longMsg);
            Assert.AreEqual(longMsg, ex.Message);
        }

        [TestMethod]
        public void EventErrorException_SpecialCharsInMessage_PreservesMessage()
        {
            const string msg = "エラー: <テスト> & \"引用符\" \\バックスラッシュ\\";
            var ex = new EventErrorException(null, msg);
            Assert.AreEqual(msg, ex.Message);
        }

        [TestMethod]
        public void EventErrorException_EmptyMessage_MessageIsEmpty()
        {
            var ex = new EventErrorException(null, "");
            Assert.AreEqual("", ex.Message);
        }

        // ──────────────────────────────────────────────
        // EventData は常に null (null cmd の場合)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventErrorException_MultipleInstances_Independent()
        {
            var ex1 = new EventErrorException(null, "エラー1");
            var ex2 = new EventErrorException(null, "エラー2");
            Assert.AreNotEqual(ex1.Message, ex2.Message);
            Assert.IsNull(ex1.EventData);
            Assert.IsNull(ex2.EventData);
        }
    }

    // ══════════════════════════════════════════════════════
    // TerminateException 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class TerminateExceptionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // InnerException の有無
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerminateException_NoInnerException_InnerIsNull()
        {
            var ex = new TerminateException("終了");
            Assert.IsNull(ex.InnerException);
        }

        [TestMethod]
        public void TerminateException_CanBeCaughtAsBaseException()
        {
            bool caught = false;
            try
            {
                throw new TerminateException("terminate");
            }
            catch (Exception ex)
            {
                caught = true;
                Assert.IsTrue(ex is TerminateException);
                Assert.AreEqual("terminate", ex.Message);
            }
            Assert.IsTrue(caught);
        }

        // ──────────────────────────────────────────────
        // メッセージのバリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerminateException_LongMessage_PreservesMessage()
        {
            var longMsg = new string('X', 1000);
            var ex = new TerminateException(longMsg);
            Assert.AreEqual(longMsg, ex.Message);
        }

        [TestMethod]
        public void TerminateException_WithInnerException_MessageAndInnerBothPreserved()
        {
            var inner = new InvalidOperationException("内部起因");
            var ex = new TerminateException("外側メッセージ", inner);
            Assert.AreEqual("外側メッセージ", ex.Message);
            Assert.AreEqual("内部起因", ex.InnerException.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    // InvalidSrcDataException 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class InvalidSrcDataExceptionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // 継承階層
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InvalidSrcDataException_IsApplicationException()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("err", "f.txt", 1, "line", "data"),
            };
            var ex = new InvalidSrcDataException(list);
            Assert.IsInstanceOfType(ex, typeof(ApplicationException));
        }

        [TestMethod]
        public void InvalidSrcDataException_CanBeCaughtAsException()
        {
            bool caught = false;
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("テストエラー", "f.txt", 1, "l", "d"),
            };
            try
            {
                throw new InvalidSrcDataException(list);
            }
            catch (Exception ex)
            {
                caught = true;
                Assert.AreEqual("テストエラー", ex.Message);
            }
            Assert.IsTrue(caught);
        }

        // ──────────────────────────────────────────────
        // InvalidDataList の独立コピー確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InvalidSrcDataException_InvalidDataList_IsReadOnly()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("e1", "f1.txt", 1, "l1", "d1"),
            };
            var ex = new InvalidSrcDataException(list);
            // IList だが内部はコピーなのでオリジナルをクリアしても影響しない
            list.Clear();
            Assert.AreEqual(1, ex.InvalidDataList.Count);
        }

        [TestMethod]
        public void InvalidSrcDataException_WithCustomMessage_CustomMessageHasPriority()
        {
            var list = new List<InvalidSrcData>
            {
                new InvalidSrcData("元メッセージ", "f.txt", 1, "l", "d"),
            };
            var ex = new InvalidSrcDataException("カスタムメッセージ", list);
            Assert.AreEqual("カスタムメッセージ", ex.Message);
            // InvalidDataList は変わらない
            Assert.AreEqual("元メッセージ", ex.InvalidDataList[0].msg);
        }

        // ──────────────────────────────────────────────
        // InvalidSrcData プロパティの検証
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InvalidSrcData_AllPropertiesAccessible()
        {
            var data = new InvalidSrcData("msg", "file.txt", 42, "bad line", "data name");
            Assert.AreEqual("msg", data.msg);
            Assert.AreEqual("file.txt", data.fname);
            Assert.AreEqual(42, data.line_num);
            Assert.AreEqual("bad line", data.line_buf);
            Assert.AreEqual("data name", data.dname);
        }

        [TestMethod]
        public void InvalidSrcData_NegativeLineNum_IsPreserved()
        {
            var data = new InvalidSrcData("e", "f", -1, "l", "d");
            Assert.AreEqual(-1, data.line_num);
        }
    }
}
