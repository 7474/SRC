using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.Tests
{
    /// <summary>
    /// GuiButton / GuiConfirmOption / GuiDialogResult enum のユニットテスト
    /// </summary>
    [TestClass]
    public class GuiEnumTests
    {
        // ──────────────────────────────────────────────
        // GuiButton
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GuiButton_None_IsZero()
        {
            Assert.AreEqual(0, (int)GuiButton.None);
        }

        [TestMethod]
        public void GuiButton_Left_IsOne()
        {
            Assert.AreEqual(1, (int)GuiButton.Left);
        }

        [TestMethod]
        public void GuiButton_Right_IsTwo()
        {
            Assert.AreEqual(2, (int)GuiButton.Right);
        }

        [TestMethod]
        public void GuiButton_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(GuiButton)).Length);
        }

        [TestMethod]
        public void GuiButton_AllValuesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GuiButton), GuiButton.None));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GuiButton), GuiButton.Left));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GuiButton), GuiButton.Right));
        }

        // ──────────────────────────────────────────────
        // GuiDialogResult
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GuiDialogResult_Ok_IsZero()
        {
            Assert.AreEqual(0, (int)GuiDialogResult.Ok);
        }

        [TestMethod]
        public void GuiDialogResult_Cancel_IsOne()
        {
            Assert.AreEqual(1, (int)GuiDialogResult.Cancel);
        }

        [TestMethod]
        public void GuiDialogResult_HasTwoValues()
        {
            Assert.AreEqual(2, System.Enum.GetValues(typeof(GuiDialogResult)).Length);
        }

        // ──────────────────────────────────────────────
        // GuiConfirmOption (Flags)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GuiConfirmOption_Ok_IsOne()
        {
            Assert.AreEqual(1, (int)GuiConfirmOption.Ok);
        }

        [TestMethod]
        public void GuiConfirmOption_OkCancel_IsTwo()
        {
            Assert.AreEqual(2, (int)GuiConfirmOption.OkCancel);
        }

        [TestMethod]
        public void GuiConfirmOption_Question_IsFour()
        {
            Assert.AreEqual(4, (int)GuiConfirmOption.Question);
        }

        [TestMethod]
        public void GuiConfirmOption_OkCanceQestion_IsSix()
        {
            Assert.AreEqual(6, (int)GuiConfirmOption.OkCanceQestion);
        }

        [TestMethod]
        public void GuiConfirmOption_OkCanceQestion_HasOkCancelAndQuestion()
        {
            Assert.IsTrue(GuiConfirmOption.OkCanceQestion.HasFlag(GuiConfirmOption.OkCancel));
            Assert.IsTrue(GuiConfirmOption.OkCanceQestion.HasFlag(GuiConfirmOption.Question));
        }
    }
}
