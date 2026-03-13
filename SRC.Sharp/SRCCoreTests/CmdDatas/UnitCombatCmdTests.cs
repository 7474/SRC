using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// DestroyCmd / TransformCmd / AttackCmd / SupplyCmd /
    /// ExchangeItemCmd / FixCmd / RemoveItemCmd /
    /// MapAttackCmd / MapAbilityCmd のユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class UnitCombatCmdTests
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

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // DestroyCmd
        // ヘルプ: Destroy [unit] — ユニットを強制的に破壊する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DestroyCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 0 または 1 個のユニット名
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Destroy unitA unitB unitC");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DestroyCmd_NonExistentUnit_ReturnsError()
        {
            // 存在しないユニット名を指定した場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Destroy 存在しないユニット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // TransformCmd
        // ヘルプ: Transform [unit] 形態名 — ユニットを指定形態に変形させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransformCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 1 個（形態名のみ）または 2 個（unit + 形態名）
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Transform");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void TransformCmd_TooManyArgs_ReturnsError()
        {
            // 引数が 3 個以上 → エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Transform unitA 形態A 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void TransformCmd_NonExistentUnit_ReturnsError()
        {
            // 存在しないユニットを指定した場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Transform 存在しないユニット 形態A");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // AttackCmd
        // ヘルプ: Attack unit1 weapon unit2 defense [通常戦闘]
        //        unit1 に weapon で unit2 を攻撃させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttackCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 5 個または 6 個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Attack unitA 武器 unitB");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void AttackCmd_InvalidOption_ReturnsError()
        {
            // 第6引数が「通常戦闘」以外 → オプション不正エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Attack unitA 武器 unitB 防御 無効オプション");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SupplyCmd
        // ヘルプ: Supply [unit] — ユニットの EN・弾数を全回復させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SupplyCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 0 または 1 個のユニット名
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Supply unitA unitB unitC");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SupplyCmd_NonExistentUnit_ReturnsError()
        {
            // 存在しないユニット名を指定した場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Supply 存在しないユニット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ExchangeItemCmd
        // ヘルプ: ExchangeItem [unit [部位]] — アイテム交換画面を開く
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeItemCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 0〜2 個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ExchangeItem unitA 部位 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // FixCmd
        // ヘルプ: Fix [パイロット名orアイテム名] — パイロット/アイテムを固定する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FixCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 0 または 1 個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Fix パイロットA 余分引数");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FixCmd_NonExistentPilot_ReturnsError()
        {
            // 存在しないパイロット名またはアイテム名を指定した場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Fix 存在しないパイロット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RemoveItemCmd
        // ヘルプ: RemoveItem [unit [アイテム名or番号]] — アイテムを外す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveItemCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 0〜2 個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveItem unitA アイテム 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // MapAttackCmd
        // ヘルプ: MapAttack [unit] マップ攻撃名 X Y [通常戦闘]
        //        unit に指定マップ攻撃をX,Y座標に実施させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapAttackCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 4 個または 5 個（+ 任意の「通常戦闘」）
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MapAttack unitA");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void MapAttackCmd_TooManyArgs_ReturnsError()
        {
            // 引数が 6 個以上かつ末尾が「通常戦闘」でない → エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MapAttack unitA 武器 1 2 余分A 余分B");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // MapAbilityCmd
        // ヘルプ: MapAbility [unit] アビリティ名 X Y
        //        unit のマップアビリティをX,Y座標で実行する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapAbilityCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は 4 個または 5 個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MapAbility unitA");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void MapAbilityCmd_TooManyArgs_ReturnsError()
        {
            // 引数が 6 個以上 → エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MapAbility unitA アビリティ 1 2 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
