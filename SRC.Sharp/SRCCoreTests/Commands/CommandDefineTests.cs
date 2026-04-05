using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.define.cs で定義されているコマンドID定数のユニットテスト
    /// </summary>
    [TestClass]
    public class CommandDefineTests
    {
        // ──────────────────────────────────────────────
        // ユニットコマンド メニュー番号
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCmdID_IsZero()
            => Assert.AreEqual(0, Command.MoveCmdID);

        [TestMethod]
        public void TeleportCmdID_IsOne()
            => Assert.AreEqual(1, Command.TeleportCmdID);

        [TestMethod]
        public void JumpCmdID_IsTwo()
            => Assert.AreEqual(2, Command.JumpCmdID);

        [TestMethod]
        public void TalkCmdID_IsThree()
            => Assert.AreEqual(3, Command.TalkCmdID);

        [TestMethod]
        public void AttackCmdID_IsFour()
            => Assert.AreEqual(4, Command.AttackCmdID);

        [TestMethod]
        public void FixCmdID_IsFive()
            => Assert.AreEqual(5, Command.FixCmdID);

        [TestMethod]
        public void SupplyCmdID_IsSix()
            => Assert.AreEqual(6, Command.SupplyCmdID);

        [TestMethod]
        public void AbilityCmdID_IsSeven()
            => Assert.AreEqual(7, Command.AbilityCmdID);

        [TestMethod]
        public void ChargeCmdID_IsEight()
            => Assert.AreEqual(8, Command.ChargeCmdID);

        [TestMethod]
        public void SpecialPowerCmdID_IsNine()
            => Assert.AreEqual(9, Command.SpecialPowerCmdID);

        [TestMethod]
        public void TransformCmdID_IsTen()
            => Assert.AreEqual(10, Command.TransformCmdID);

        [TestMethod]
        public void SplitCmdID_IsEleven()
            => Assert.AreEqual(11, Command.SplitCmdID);

        [TestMethod]
        public void CombineCmdID_IsTwelve()
            => Assert.AreEqual(12, Command.CombineCmdID);

        [TestMethod]
        public void HyperModeCmdID_IsThirteen()
            => Assert.AreEqual(13, Command.HyperModeCmdID);

        [TestMethod]
        public void GroundCmdID_IsFourteen()
            => Assert.AreEqual(14, Command.GroundCmdID);

        [TestMethod]
        public void SkyCmdID_IsFifteen()
            => Assert.AreEqual(15, Command.SkyCmdID);

        [TestMethod]
        public void UndergroundCmdID_IsSixteen()
            => Assert.AreEqual(16, Command.UndergroundCmdID);

        [TestMethod]
        public void WaterCmdID_IsSeventeen()
            => Assert.AreEqual(17, Command.WaterCmdID);

        [TestMethod]
        public void LaunchCmdID_IsEighteen()
            => Assert.AreEqual(18, Command.LaunchCmdID);

        [TestMethod]
        public void ItemCmdID_IsNineteen()
            => Assert.AreEqual(19, Command.ItemCmdID);

        [TestMethod]
        public void DismissCmdID_IsTwenty()
            => Assert.AreEqual(20, Command.DismissCmdID);

        [TestMethod]
        public void OrderCmdID_IsTwentyOne()
            => Assert.AreEqual(21, Command.OrderCmdID);

        [TestMethod]
        public void ExchangeFormCmdID_IsTwentyTwo()
            => Assert.AreEqual(22, Command.ExchangeFormCmdID);

        [TestMethod]
        public void FeatureListCmdID_IsTwentyThree()
            => Assert.AreEqual(23, Command.FeatureListCmdID);

        [TestMethod]
        public void WeaponListCmdID_IsTwentyFour()
            => Assert.AreEqual(24, Command.WeaponListCmdID);

        [TestMethod]
        public void AbilityListCmdID_IsTwentyFive()
            => Assert.AreEqual(25, Command.AbilityListCmdID);

        [TestMethod]
        public void UnitCommandCmdID_Is127()
            => Assert.AreEqual(127, Command.UnitCommandCmdID);

        [TestMethod]
        public void WaitCmdID_Is255()
            => Assert.AreEqual(255, Command.WaitCmdID);

        // ──────────────────────────────────────────────
        // マップコマンド メニュー番号
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EndTurnCmdID_IsZero()
            => Assert.AreEqual(0, Command.EndTurnCmdID);

        [TestMethod]
        public void DumpCmdID_IsOne()
            => Assert.AreEqual(1, Command.DumpCmdID);

        [TestMethod]
        public void UnitListCmdID_IsTwo()
            => Assert.AreEqual(2, Command.UnitListCmdID);

        [TestMethod]
        public void SearchSpecialPowerCmdID_IsThree()
            => Assert.AreEqual(3, Command.SearchSpecialPowerCmdID);

        [TestMethod]
        public void GlobalMapCmdID_IsFour()
            => Assert.AreEqual(4, Command.GlobalMapCmdID);

        [TestMethod]
        public void OperationObjectCmdID_IsFive()
            => Assert.AreEqual(5, Command.OperationObjectCmdID);

        [TestMethod]
        public void MapCommandCmdID_IsSix()
            => Assert.AreEqual(6, Command.MapCommandCmdID);

        [TestMethod]
        public void AutoDefenseCmdID_IsSixteen()
            => Assert.AreEqual(16, Command.AutoDefenseCmdID);

        [TestMethod]
        public void ConfigurationCmdID_IsSeventeen()
            => Assert.AreEqual(17, Command.ConfigurationCmdID);

        [TestMethod]
        public void RestartCmdID_IsEighteen()
            => Assert.AreEqual(18, Command.RestartCmdID);

        [TestMethod]
        public void QuickLoadCmdID_IsNineteen()
            => Assert.AreEqual(19, Command.QuickLoadCmdID);

        [TestMethod]
        public void QuickSaveCmdID_IsTwenty()
            => Assert.AreEqual(20, Command.QuickSaveCmdID);

        // ──────────────────────────────────────────────
        // 定数の相対順序 (回帰的整合性確認)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommandIDs_AreInAscendingOrder_Move_To_WaterCmdID()
        {
            // 移動系コマンドの順序: Move < Teleport < Jump < Talk < Attack < Fix < Supply
            Assert.IsTrue(Command.MoveCmdID < Command.TeleportCmdID);
            Assert.IsTrue(Command.TeleportCmdID < Command.JumpCmdID);
            Assert.IsTrue(Command.JumpCmdID < Command.TalkCmdID);
            Assert.IsTrue(Command.TalkCmdID < Command.AttackCmdID);
            Assert.IsTrue(Command.AttackCmdID < Command.FixCmdID);
            Assert.IsTrue(Command.FixCmdID < Command.SupplyCmdID);
        }

        [TestMethod]
        public void MapCommandIDs_AreInAscendingOrder()
        {
            Assert.IsTrue(Command.EndTurnCmdID < Command.DumpCmdID);
            Assert.IsTrue(Command.DumpCmdID < Command.UnitListCmdID);
            Assert.IsTrue(Command.UnitListCmdID < Command.SearchSpecialPowerCmdID);
        }

        [TestMethod]
        public void UnitCommandCmdID_IsSpecialValue_127()
        {
            // UnitCommandCmdID はメニューの末尾付近を示す特別な値
            Assert.IsTrue(Command.UnitCommandCmdID > Command.LaunchCmdID);
            Assert.IsTrue(Command.UnitCommandCmdID < Command.WaitCmdID);
        }

        [TestMethod]
        public void WaitCmdID_IsLargestUnitCmdID()
        {
            // WaitCmdID (255) は全ユニットコマンドIDの中で最大
            Assert.IsTrue(Command.WaitCmdID > Command.UnitCommandCmdID);
        }
    }
}
