using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Unit/Pilot/Item/Map コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class UnitCmdTests
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
        // DisableCmd
        // ヘルプ: 指定した武器・アビリティ等を使用不可能にする
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DisableCmd_AbilityNameOnly_SetsGlobalVariable()
        {
            // ヘルプ: unit を省略した場合は敵・味方関係なくその武器等が封印される
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Disable 神剣オリオン");
            var result = cmd.Exec();

            Assert.AreEqual(1, result); // returns NextID (=1)
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Disable(神剣オリオン)"));
            Assert.AreEqual(1L, src.Expression.GetValueAsLong("Disable(神剣オリオン)"));
        }

        [TestMethod]
        public void DisableCmd_AlreadyDisabled_IsIdempotent()
        {
            // 既に封印済みの場合は二重設定しない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Disable 神剣オリオン");
            cmd.Exec(); // 1回目
            var result = cmd.Exec(); // 2回目

            Assert.AreEqual(1, result); // 2回目もエラーなし
            Assert.AreEqual(1L, src.Expression.GetValueAsLong("Disable(神剣オリオン)"));
        }

        [TestMethod]
        public void DisableCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Disable a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // EnableCmd
        // ヘルプ: Disableコマンドで封印された武器等の封印を解除
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EnableCmd_PreviouslyDisabled_RemovesGlobalVariable()
        {
            // ヘルプ: Disableコマンドで封印された能力を使用可能にする
            var src = CreateSrc();
            // まず封印する
            var disableCmd = CreateCmd(src, "Disable 反物質砲", id: 0);
            disableCmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Disable(反物質砲)"));

            // 封印を解除
            var enableCmd = CreateCmd(src, "Enable 反物質砲", id: 1);
            var result = enableCmd.Exec();

            Assert.AreEqual(2, result); // returns NextID (=2)
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("Disable(反物質砲)"));
        }

        [TestMethod]
        public void EnableCmd_NotPreviouslyDisabled_NoError()
        {
            // 封印されていない能力に対してもエラーにならない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Enable 存在しない能力");
            var result = cmd.Exec();

            Assert.AreEqual(1, result); // returns NextID
        }

        [TestMethod]
        public void EnableCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Enable a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangePartyCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangePartyCmd_InvalidPartyName_ReturnsError()
        {
            // ヘルプ: 味方/ＮＰＣ/敵/中立 以外の陣営名はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeParty 不正な陣営");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ChangePartyCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeParty a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // BossRankCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BossRankCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "BossRank a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void BossRankCmd_NonNumericRank_ReturnsError()
        {
            // ヘルプ: rank には数値を指定する
            var src = CreateSrc();
            var cmd = CreateCmd(src, "BossRank ユニット名 非数字");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearStatusCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearStatusCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearStatus a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeAreaCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeAreaCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeArea a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeModeCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeModeCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeMode a b c d e");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SplitCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SplitCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Split a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // StopSummoningCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StopSummoningCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "StopSummoning a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChargeCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChargeCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Charge a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RankUpCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RankUp a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // EquipCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EquipCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Equip a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ItemCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Item a b");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ItemCmd_NonExistentItem_ReturnsError()
        {
            // ヘルプ: 存在しないアイテムIDを指定するとエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Item 存在しないアイテム");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // PilotCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: ArgNum は 3 or 4 でなければならない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Pilot パイロット名");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void PilotCmd_NonExistentPilot_ReturnsError()
        {
            // ヘルプ: パイロットデータが存在しない場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Pilot 存在しないパイロット 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ReplacePilotCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplacePilotCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ReplacePilot a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // GetOffCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetOffCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "GetOff a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeMapCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeMapCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeMap a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ChangeMapCmd_InvalidOption_ReturnsError()
        {
            // ヘルプ: 3引数の場合、第3引数は「非同期」でなければエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeMap mapfile 不正なオプション");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeLayerCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeLayerCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeLayer a b");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeTerrainCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeTerrainCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeTerrain a b");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // CenterCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CenterCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 引数は2または3つ必要
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Center");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CenterCmd_WithCoordinates_ExecutesSuccessfully()
        {
            // ヘルプ: Center x y でマップの指定座標を中心に表示
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Center 5 5");
            var result = cmd.Exec();
            Assert.AreEqual(1, result); // returns NextID
        }

        // ──────────────────────────────────────────────
        // PaintPictureCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PaintPictureCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: 引数は4つ以上必要
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PaintPicture a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ExpUpCmd
        // ヘルプ: 指定したパイロットの経験値をexp分だけ上げる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExpUpCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: ExpUp [pilot] exp
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ExpUp");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LevelUpCmd
        // ヘルプ: 指定したパイロットのレベルをlevel分だけ上げる（最大99）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LevelUpCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: LevelUp [pilot] level
            var src = CreateSrc();
            var cmd = CreateCmd(src, "LevelUp");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RecoverHPCmd
        // ヘルプ: unitのHPをrate%だけ回復する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverHPCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: RecoverHP [unit] rate
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverHP");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RecoverENCmd
        // ヘルプ: unitのENをrate%だけ回復する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverENCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: RecoverEN [unit] rate
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverEN");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RecoverSPCmd
        // ヘルプ: pilotのSPをrate%だけ回復する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverSPCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: RecoverSP [pilot] rate
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverSP");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RecoverPlanaCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverPlanaCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: RecoverPlana [pilot] rate
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RecoverPlana");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IncreaseMoraleCmd
        // ヘルプ: ユニットに乗っているパイロットの気力をvalue増加させる（上限150）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncreaseMoraleCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: IncreaseMorale [unit] value
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IncreaseMorale");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetSkillCmd
        // ヘルプ: pilotにパイロット用特殊能力skillを追加する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetSkillCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: 書式: SetSkill pilot skill level [name] → 引数は3〜4個
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetSkill パイロット名 スキル名"); // ArgNum=3, need 4 or 5
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetSkillCmd_NonExistentPilot_ReturnsError()
        {
            // ヘルプ: pilotが見つからない場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetSkill 存在しないパイロット テスト 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearSkillCmd
        // ヘルプ: SetSkillで追加した特殊能力を削除する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearSkillCmd_NonExistentPilot_ReturnsError()
        {
            // ヘルプ: pilotが見つからない場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearSkill 存在しないパイロット テスト");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
