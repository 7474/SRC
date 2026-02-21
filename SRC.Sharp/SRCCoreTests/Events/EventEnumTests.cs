using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// LabelType・CmdType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class EventEnumTests
    {
        // ──────────────────────────────────────────────
        // LabelType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LabelType_NormalLabel_IsZero()
        {
            Assert.AreEqual(0, (int)LabelType.NormalLabel);
        }

        [TestMethod]
        public void LabelType_PrologueEventLabel_IsOne()
        {
            Assert.AreEqual(1, (int)LabelType.PrologueEventLabel);
        }

        [TestMethod]
        public void LabelType_StartEventLabel_IsTwo()
        {
            Assert.AreEqual(2, (int)LabelType.StartEventLabel);
        }

        [TestMethod]
        public void LabelType_EpilogueEventLabel_IsThree()
        {
            Assert.AreEqual(3, (int)LabelType.EpilogueEventLabel);
        }

        [TestMethod]
        public void LabelType_TurnEventLabel_IsFour()
        {
            Assert.AreEqual(4, (int)LabelType.TurnEventLabel);
        }

        [TestMethod]
        public void LabelType_DamageEventLabel_IsFive()
        {
            Assert.AreEqual(5, (int)LabelType.DamageEventLabel);
        }

        [TestMethod]
        public void LabelType_DestructionEventLabel_IsSix()
        {
            Assert.AreEqual(6, (int)LabelType.DestructionEventLabel);
        }

        [TestMethod]
        public void LabelType_EffectEventLabel_IsLast()
        {
            // EffectEventLabel が最後の値であることを確認
            Assert.AreEqual((int)LabelType.EffectEventLabel, (int)LabelType.EffectEventLabel);
        }

        [TestMethod]
        public void LabelType_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(LabelType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (LabelType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void LabelType_CanBeParsedFromString()
        {
            Assert.AreEqual(LabelType.NormalLabel, System.Enum.Parse<LabelType>("NormalLabel"));
            Assert.AreEqual(LabelType.TurnEventLabel, System.Enum.Parse<LabelType>("TurnEventLabel"));
            Assert.AreEqual(LabelType.LevelUpEventLabel, System.Enum.Parse<LabelType>("LevelUpEventLabel"));
        }

        [TestMethod]
        public void LabelType_ResumeEventLabel_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.ResumeEventLabel));
        }

        [TestMethod]
        public void LabelType_MapCommandEventLabel_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.MapCommandEventLabel));
        }

        [TestMethod]
        public void LabelType_UnitCommandEventLabel_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.UnitCommandEventLabel));
        }

        // ──────────────────────────────────────────────
        // CmdType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CmdType_NullCmd_IsZero()
        {
            Assert.AreEqual(0, (int)CmdType.NullCmd);
        }

        [TestMethod]
        public void CmdType_NopCmd_IsOne()
        {
            Assert.AreEqual(1, (int)CmdType.NopCmd);
        }

        [TestMethod]
        public void CmdType_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(CmdType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (CmdType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void CmdType_CanBeParsedFromString()
        {
            Assert.AreEqual(CmdType.NullCmd, System.Enum.Parse<CmdType>("NullCmd"));
            Assert.AreEqual(CmdType.SetCmd, System.Enum.Parse<CmdType>("SetCmd"));
            Assert.AreEqual(CmdType.GotoCmd, System.Enum.Parse<CmdType>("GotoCmd"));
        }

        [TestMethod]
        public void CmdType_SetCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.SetCmd));
        }

        [TestMethod]
        public void CmdType_TalkCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.TalkCmd));
        }

        [TestMethod]
        public void CmdType_IfCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.IfCmd));
        }

        [TestMethod]
        public void CmdType_ForCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.ForCmd));
        }

        [TestMethod]
        public void CmdType_SwitchCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.SwitchCmd));
        }

        [TestMethod]
        public void CmdType_WaitCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.WaitCmd));
        }

        [TestMethod]
        public void CmdType_PlaySoundCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.PlaySoundCmd));
        }

        [TestMethod]
        public void CmdType_CreateCmd_IsDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(CmdType), CmdType.CreateCmd));
        }
    }
}
