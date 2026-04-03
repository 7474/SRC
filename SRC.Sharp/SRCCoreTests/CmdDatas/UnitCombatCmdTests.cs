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

        // ──────────────────────────────────────────────
        // RecoverHPCmd
        // ヘルプ: RecoverHP [unit] rate — ユニットの HP を rate ％回復
        // 解説: マイナス値でHP減少も可能。HP は最大HPを超えず、0以下にならない。
        // ──────────────────────────────────────────────

        /// <summary>
        /// RecoverHP rate (unit 省略) — SelectedUnitForEvent が null の場合、エラーなく NextID を返す
        /// </summary>
        [TestMethod]
        public void RecoverHPCmd_NullSelectedUnit_ReturnsNextId()
        {
            // ヘルプ: unit 省略時はデフォルトユニット。null でも例外を出さず NextID を返す
            var src = CreateSrc();
            // SelectedUnitForEvent は null のまま
            var cmd = CreateCmd(src, "RecoverHP 100");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// RecoverHP 50 — 半HPユニットの HP が回復する
        /// </summary>
        [TestMethod]
        public void RecoverHPCmd_WithUnit_RecoveryIncreasesHP()
        {
            // ヘルプ: unit の HP を rate % だけ回復する
            var src = CreateSrc();
            var ud = src.UDList.Add("HPテスト");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 10;
            // Unit コマンドでユニット作成
            var unitCmd = CreateCmd(src, "Unit HPテスト 0", 0);
            unitCmd.Exec();
            var u = src.Event.SelectedUnitForEvent;
            Assert.IsNotNull(u);
            // HP を半分に減らす
            u.HP = 50;
            Assert.AreEqual(50, u.HP);
            // RecoverHP 50 → 50 + (100 * 50 / 100) = 100
            var recoverCmd = CreateCmd(src, "RecoverHP 50", 1);
            recoverCmd.Exec();
            Assert.AreEqual(100, u.HP);
        }

        /// <summary>
        /// RecoverHP -200 — HP を大幅に減らしても HP は 1 以下にならない
        /// </summary>
        [TestMethod]
        public void RecoverHPCmd_NegativeRate_HpDoesNotGoToZero()
        {
            // ヘルプ: RecoverHP によってユニットが破壊されることはない（HP は最小 1）
            var src = CreateSrc();
            var ud = src.UDList.Add("HPテスト2");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 10;
            var unitCmd = CreateCmd(src, "Unit HPテスト2 0", 0);
            unitCmd.Exec();
            var u = src.Event.SelectedUnitForEvent;
            Assert.IsNotNull(u);
            // RecoverHP -200 → HP = max(1, 100 - 200) = 1
            var recoverCmd = CreateCmd(src, "RecoverHP -200", 1);
            recoverCmd.Exec();
            Assert.AreEqual(1, u.HP);
        }

        // ──────────────────────────────────────────────
        // RecoverENCmd
        // ヘルプ: RecoverEN [unit] rate — ユニットの EN を rate ％回復
        // 解説: マイナス値でEN減少も可能。EN は最大ENを超えず、0未満にならない。
        // ──────────────────────────────────────────────

        /// <summary>
        /// RecoverEN rate (unit 省略) — SelectedUnitForEvent が null の場合、エラーなく NextID を返す
        /// </summary>
        [TestMethod]
        public void RecoverENCmd_NullSelectedUnit_ReturnsNextId()
        {
            // ヘルプ: unit 省略時はデフォルトユニット。null でも例外なく NextID を返す
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverEN 100");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// RecoverEN 100 — 消費後の EN が最大まで回復する
        /// </summary>
        [TestMethod]
        public void RecoverENCmd_WithUnit_RecoveryIncreasesEN()
        {
            // ヘルプ: unit の EN を rate % だけ回復する
            var src = CreateSrc();
            var ud = src.UDList.Add("ENテスト");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var unitCmd = CreateCmd(src, "Unit ENテスト 0", 0);
            unitCmd.Exec();
            var u = src.Event.SelectedUnitForEvent;
            Assert.IsNotNull(u);
            // EN を 0 に
            u.EN = 0;
            // RecoverEN 100 → 0 + (50 * 100 / 100) = 50
            var recoverCmd = CreateCmd(src, "RecoverEN 100", 1);
            recoverCmd.Exec();
            Assert.AreEqual(50, u.EN);
        }

        /// <summary>
        /// RecoverEN -200 — EN はゼロより下には下がらない
        /// </summary>
        [TestMethod]
        public void RecoverENCmd_NegativeRate_ENDoesNotGoBelowZero()
        {
            // ヘルプ: RecoverEN によって EN が 0 未満になることはない
            var src = CreateSrc();
            var ud = src.UDList.Add("ENテスト2");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var unitCmd = CreateCmd(src, "Unit ENテスト2 0", 0);
            unitCmd.Exec();
            var u = src.Event.SelectedUnitForEvent;
            Assert.IsNotNull(u);
            // RecoverEN -200 → EN = max(0, 50 - 100) = 0
            var recoverCmd = CreateCmd(src, "RecoverEN -200", 1);
            recoverCmd.Exec();
            Assert.AreEqual(0, u.EN);
        }
    }
}
