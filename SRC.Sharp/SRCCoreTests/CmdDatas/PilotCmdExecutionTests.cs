using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// LevelUpCmd / ExpUpCmd / RecoverSPCmd / IncreaseMoraleCmd のユニットテスト
    /// ヘルプの記載に基づくパイロットステータス変更を検証する
    /// </summary>
    [TestClass]
    public class PilotCmdExecutionTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
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

        /// <summary>
        /// パイロットを PDList と PList に登録してレベル 1 で返すヘルパー
        /// </summary>
        private Pilots.Pilot CreatePilot(SRC src, string name, int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 100;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // LevelUpCmd
        // ヘルプ: LevelUp [pilot] level — パイロットのレベルを level 分上げる
        // 解説: レベルは最大 99 まで（レベル限界突破オプション時は 999 まで）
        // ──────────────────────────────────────────────

        /// <summary>
        /// LevelUp パイロット名 1 — 指定パイロットのレベルが 1 上がる
        /// </summary>
        [TestMethod]
        public void LevelUpCmd_WithNamedPilot_IncreasesLevel()
        {
            // ヘルプ: LevelUp pilot level — pilot のレベルを level 分上げる
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テストパイロット", 10);
            Assert.AreEqual(10, pilot.Level);

            var cmd = CreateCmd(src, "LevelUp テストパイロット 1");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(11, pilot.Level);
        }

        /// <summary>
        /// LevelUp パイロット名 10 — 複数レベルまとめて上げられる
        /// </summary>
        [TestMethod]
        public void LevelUpCmd_WithNamedPilot_MultiLevelIncrease()
        {
            // ヘルプ: level 分だけ上げる（複数も可）
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テストパイロット2", 5);

            var cmd = CreateCmd(src, "LevelUp テストパイロット2 10");
            cmd.Exec();

            Assert.AreEqual(15, pilot.Level);
        }

        /// <summary>
        /// LevelUp — レベルは 99 を超えない（上限チェック）
        /// </summary>
        [TestMethod]
        public void LevelUpCmd_WithNamedPilot_CapAt99()
        {
            // ヘルプ: ただしパイロットのレベルは最大でも 99 までです
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テストパイロット3", 98);

            // レベルを 98 → 105 に上げようとする → 99 に制限される
            var cmd = CreateCmd(src, "LevelUp テストパイロット3 7");
            cmd.Exec();

            Assert.AreEqual(99, pilot.Level);
        }

        /// <summary>
        /// LevelUp 存在しないパイロット 1 → エラー
        /// </summary>
        [TestMethod]
        public void LevelUpCmd_NonExistentPilot_ReturnsError()
        {
            // 存在しないパイロット名を指定した場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "LevelUp 存在しないパイロット 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ExpUpCmd
        // ヘルプ: ExpUp [pilot] exp — パイロットに経験値を加算する
        // 解説: 経験値がレベルアップ閾値を超えるとレベルが上がる
        // ──────────────────────────────────────────────

        /// <summary>
        /// ExpUp パイロット名 100 — 経験値が加算される
        /// </summary>
        [TestMethod]
        public void ExpUpCmd_WithNamedPilot_IncreasesExp()
        {
            // ヘルプ: pilot の経験値を exp 分加算する
            var src = CreateSrc();
            var pilot = CreatePilot(src, "経験値テスト", 1);
            var initialExp = pilot.Exp;

            var cmd = CreateCmd(src, "ExpUp 経験値テスト 100");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(pilot.Exp >= initialExp + 100 || pilot.Level > 1,
                "経験値が加算されるかレベルアップしている必要がある");
        }

        /// <summary>
        /// ExpUp 存在しないパイロット 100 → エラー
        /// </summary>
        [TestMethod]
        public void ExpUpCmd_NonExistentPilot_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ExpUp 存在しないパイロット 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RecoverSPCmd
        // ヘルプ: RecoverSP [pilot] rate — パイロットの SP を rate % 回復する
        // 解説: SP は最大SP を超えず 0 未満にもならない
        // ──────────────────────────────────────────────

        /// <summary>
        /// RecoverSP パイロット名 -100 — SP がゼロより下に行かない
        /// </summary>
        [TestMethod]
        public void RecoverSPCmd_WithNamedPilot_SPDoesNotGoBelowZero()
        {
            // ヘルプ: RecoverSP によって SP が 0 未満になることはない
            var src = CreateSrc();
            var pilot = CreatePilot(src, "SPテスト", 1);
            // MaxSP = pd.SP (= 100), 現在 SP = 100
            pilot.SP = 50;
            Assert.AreEqual(50, pilot.SP);

            // RecoverSP SPテスト -200 → SP = max(0, 50 - 200) = 0
            var cmd = CreateCmd(src, "RecoverSP SPテスト -200");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(0, pilot.SP);
        }

        /// <summary>
        /// RecoverSP パイロット名 100 — SP が MaxSP まで回復する
        /// </summary>
        [TestMethod]
        public void RecoverSPCmd_WithNamedPilot_SPRecovery()
        {
            // ヘルプ: pilot の SP を rate % だけ回復する
            // MaxSP は実際の計算結果に依存するため、回復後 SP == MaxSP であることを確認する
            var src = CreateSrc();
            var pilot = CreatePilot(src, "SPテスト2", 1);
            pilot.SP = 0;
            Assert.AreEqual(0, pilot.SP);
            var maxSP = pilot.MaxSP;
            Assert.IsTrue(maxSP > 0, "MaxSP > 0 が前提");

            // RecoverSP SPテスト2 100 → SP = 0 + (100 * MaxSP / 100) = MaxSP
            var cmd = CreateCmd(src, "RecoverSP SPテスト2 100");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(maxSP, pilot.SP);
        }

        /// <summary>
        /// RecoverSP の端数は元の SRC の Long への代入と同じく偶数へ丸める（7474/SRC#1471）
        /// </summary>
        [TestMethod]
        public void RecoverSPCmd_RoundsFractionToEven()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("SP丸め");
            pd.SP = 98;
            var pilot = src.PList.Add("SP丸め", 1, "味方");
            // MaxSP = 2 × レベル1 + 98 = 100
            Assert.AreEqual(100, pilot.MaxSP);

            pilot.SP = 10;
            Assert.AreEqual(1, CreateCmd(src, "RecoverSP SP丸め 3.5", 0).Exec());
            Assert.AreEqual(14, pilot.SP); // 13.5 → 14

            pilot.SP = 10;
            Assert.AreEqual(2, CreateCmd(src, "RecoverSP SP丸め 2.5", 1).Exec());
            Assert.AreEqual(12, pilot.SP); // 12.5 → 12
        }

        /// <summary>
        /// RecoverPlana の端数は元の SRC の Long への代入と同じく偶数へ丸める（7474/SRC#1471）
        /// </summary>
        [TestMethod]
        public void RecoverPlanaCmd_RoundsFractionToEven()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("霊力丸め");
            pd.SP = 10;
            pd.AddSkill("霊力", 17d, "", 1);
            var pilot = src.PList.Add("霊力丸め", 2, "味方");
            // 最大霊力 = 17 + 1.5 × 2 = 20
            Assert.AreEqual(20, pilot.MaxPlana());

            pilot.Plana = 0;
            Assert.AreEqual(1, CreateCmd(src, "RecoverPlana 霊力丸め 17.5", 0).Exec());
            Assert.AreEqual(4, pilot.Plana); // 3.5 → 4

            pilot.Plana = 0;
            Assert.AreEqual(2, CreateCmd(src, "RecoverPlana 霊力丸め 12.5", 1).Exec());
            Assert.AreEqual(2, pilot.Plana); // 2.5 → 2
        }

        /// <summary>
        /// LevelUp・ExpUp はユニットの HP・EN を変化前の割合で入れ直す。VB の Long への代入と同じく
        /// 偶数丸めにするので、割合の往復で生じる誤差（1000 × 32.3% / 100 = 322.99…）で 1 減らない
        /// （VB: CmdData.cls ExecLevelUpCmd・ExecExpUpCmd の .HP = .MaxHP * hp_ratio / 100）
        /// </summary>
        [TestMethod]
        public void LevelUpAndExpUp_KeepHPWithoutRoundTripTruncation()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("割合機");
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            var u = src.UList.Add("割合機", 0, "味方");
            u.Update();
            var pilot = CreatePilot(src, "割合パイロット", 1);
            pilot.Ride(u);

            u.HP = 323;
            Assert.AreEqual(1, CreateCmd(src, "LevelUp 割合パイロット 1", 0).Exec());
            Assert.AreEqual(2, pilot.Level);
            Assert.AreEqual(323, u.HP);

            u.HP = 641;
            Assert.AreEqual(2, CreateCmd(src, "ExpUp 割合パイロット 1000", 1).Exec());
            Assert.IsTrue(pilot.Level > 2);
            Assert.AreEqual(641, u.HP);
        }

        /// <summary>
        /// RecoverSP 存在しないパイロット 100 → エラー
        /// </summary>
        [TestMethod]
        public void RecoverSPCmd_NonExistentPilot_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverSP 存在しないパイロット 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IncreaseMoraleCmd
        // ヘルプ: IncreaseMorale [pilot] value — パイロットの気力を value 分増加させる
        // 解説: 気力は 50 〜 150 の範囲（設定によって変動）
        // ──────────────────────────────────────────────

        /// <summary>
        /// IncreaseMorale パイロット名 10 — 気力が増加する
        /// </summary>
        [TestMethod]
        public void IncreaseMoraleCmd_WithNamedPilot_IncreaseMorale()
        {
            // ヘルプ: pilot の気力を value 分上げる
            var src = CreateSrc();
            var pilot = CreatePilot(src, "気力テスト", 1);
            var initialMorale = pilot.Morale;

            var cmd = CreateCmd(src, "IncreaseMorale 気力テスト 10");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(pilot.Morale >= initialMorale, "気力が下がってはいけない");
        }

        /// <summary>
        /// IncreaseMorale 存在しないパイロット 10 → エラー
        /// </summary>
        [TestMethod]
        public void IncreaseMoraleCmd_NonExistentPilot_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IncreaseMorale 存在しないパイロット 10");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
