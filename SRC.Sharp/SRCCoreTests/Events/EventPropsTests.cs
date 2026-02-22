using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// Event クラスのプロパティ初期値テスト
    /// </summary>
    [TestClass]
    public class EventPropsTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        // ──────────────────────────────────────────────
        // イベントコレクション初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventData_InitiallyEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.EventData.Count);
        }

        [TestMethod]
        public void EventCmd_InitiallyEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.EventCmd.Count);
        }

        [TestMethod]
        public void EventFileNames_InitiallyEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.EventFileNames.Count);
        }

        [TestMethod]
        public void AdditionalEventFileNames_InitiallyEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.AdditionalEventFileNames.Count);
        }

        // ──────────────────────────────────────────────
        // BCVariable 初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BCVariable_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event.BCVariable);
        }

        [TestMethod]
        public void BCVariable_IsConfig_DefaultFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Event.BCVariable.IsConfig);
        }

        [TestMethod]
        public void BCVariable_MeUnit_DefaultNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Event.BCVariable.MeUnit);
        }

        [TestMethod]
        public void BCVariable_AtkUnit_DefaultNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Event.BCVariable.AtkUnit);
        }

        [TestMethod]
        public void BCVariable_DefUnit_DefaultNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Event.BCVariable.DefUnit);
        }

        [TestMethod]
        public void BCVariable_WeaponNumber_DefaultZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.BCVariable.WeaponNumber);
        }

        // ──────────────────────────────────────────────
        // CallDepth 初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CallDepth_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.CallDepth);
        }

        [TestMethod]
        public void ArgIndex_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.ArgIndex);
        }

        [TestMethod]
        public void VarIndex_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.VarIndex);
        }

        [TestMethod]
        public void ForIndex_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.ForIndex);
        }

        // ──────────────────────────────────────────────
        // CurrentLabel / CurrentLineNum 初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CurrentLineNum_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.CurrentLineNum);
        }

        [TestMethod]
        public void CurrentLabel_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.CurrentLabel);
        }

        // ──────────────────────────────────────────────
        // SelectedAlternative 初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedAlternative_InitiallyNullOrEmpty()
        {
            var src = CreateSrc();
            // 初期値が null または空文字列であることを確認
            Assert.IsTrue(string.IsNullOrEmpty(src.Event.SelectedAlternative));
        }

        // ──────────────────────────────────────────────
        // MaxCallDepth / MaxArgIndex / MaxVarIndex 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxCallDepth_IsFifty()
        {
            Assert.AreEqual(50, Event.MaxCallDepth);
        }

        [TestMethod]
        public void MaxArgIndex_IsTwoHundred()
        {
            Assert.AreEqual(200, Event.MaxArgIndex);
        }

        [TestMethod]
        public void MaxVarIndex_IsTwoThousand()
        {
            Assert.AreEqual(2000, Event.MaxVarIndex);
        }

        // ──────────────────────────────────────────────
        // CallStack / ArgStack / VarStack の初期サイズ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CallStack_HasCorrectCapacity()
        {
            var src = CreateSrc();
            Assert.AreEqual(Event.MaxCallDepth + 1, src.Event.CallStack.Count);
        }

        [TestMethod]
        public void ArgStack_HasCorrectCapacity()
        {
            var src = CreateSrc();
            Assert.AreEqual(Event.MaxArgIndex + 1, src.Event.ArgStack.Count);
        }

        [TestMethod]
        public void VarStack_HasCorrectCapacity()
        {
            var src = CreateSrc();
            Assert.AreEqual(Event.MaxVarIndex + 1, src.Event.VarStack.Count);
        }

        // ──────────────────────────────────────────────
        // WaitStartTime / WaitTimeCount
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WaitStartTime_CanBeSet()
        {
            var src = CreateSrc();
            src.Event.WaitStartTime = -1;
            Assert.AreEqual(-1, src.Event.WaitStartTime);
        }

        [TestMethod]
        public void WaitTimeCount_InitiallyZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.WaitTimeCount);
        }

        // ──────────────────────────────────────────────
        // GlobalVariableList / LocalVariableList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalVariableList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event.GlobalVariableList);
        }

        [TestMethod]
        public void LocalVariableList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event.LocalVariableList);
        }

        [TestMethod]
        public void colEventLabelList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event.colEventLabelList);
        }
    }
}
