using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot のステータス・生死関連のユニットテスト
    /// </summary>
    [TestClass]
    public class PilotStatusTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1, string nickname = null)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            if (nickname != null)
            {
                pd.Nickname = nickname;
            }
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // Status (Alive フィールド)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Alive_Default_IsTrue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            // PList.Add は FullRecover を呼び出し、Alive = true になる
            Assert.IsTrue(pilot.Alive);
        }

        [TestMethod]
        public void Alive_SetFalse_IsNotAlive()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Alive = false;
            Assert.IsFalse(pilot.Alive);
        }

        [TestMethod]
        public void Alive_SetTrueAfterFalse_IsAlive()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Alive = false;
            pilot.Alive = true;
            Assert.IsTrue(pilot.Alive);
        }

        // ──────────────────────────────────────────────
        // FullRecover で Alive が復元される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FullRecover_RestoresSPToMax_AfterDepletion()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.SP = 0;
            pilot.FullRecover();
            Assert.AreEqual(pilot.MaxSP, pilot.SP);
        }

        [TestMethod]
        public void FullRecover_RestoresSPToMax()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.SP = 0;
            pilot.FullRecover();
            Assert.AreEqual(pilot.MaxSP, pilot.SP);
        }

        // ──────────────────────────────────────────────
        // Name プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_ReturnsDataName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "山田太郎");
            Assert.AreEqual("山田太郎", pilot.Name);
        }

        // ──────────────────────────────────────────────
        // Nickname vs Name
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname0_WhenNicknameSet_ReturnsDifferentFromName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "山田太郎", nickname: "タロウ");
            // Nickname0 はデータの愛称をそのまま返す
            Assert.AreEqual("タロウ", pilot.Nickname0);
            Assert.AreNotEqual(pilot.Name, pilot.Nickname0);
        }

        [TestMethod]
        public void Nickname0_SetAndGet_ReturnsExpectedValue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("花子");
            pd.SP = 10;
            pd.Nickname = "ハナ";
            var pilot = src.PList.Add("花子", 1, "味方");
            Assert.AreEqual("ハナ", pilot.Nickname0);
        }

        // ──────────────────────────────────────────────
        // Party
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Party_ReturnsAssignedParty()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("敵パイロット");
            pd.SP = 10;
            var pilot = src.PList.Add("敵パイロット", 1, "敵");
            Assert.AreEqual("敵", pilot.Party);
        }

        // ──────────────────────────────────────────────
        // Away フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Away_Default_IsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.Away);
        }

        [TestMethod]
        public void Away_SetTrue_IsAway()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Away = true;
            Assert.IsTrue(pilot.Away);
        }
    }
}
