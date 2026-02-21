using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出すパイロット情報関数 (Level, Morale, Plana, SP, Skill, Relation) のユニットテスト
    /// </summary>
    [TestClass]
    public class PilotFunctionTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilots.Pilot CreatePilot(SRC src, string name, int level = 1, int sp = 10)
        {
            var pd = src.PDList.Add(name);
            pd.SP = sp;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // Level
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Level_ExistingPilot_ReturnsLevel()
        {
            var src = CreateSrc();
            CreatePilot(src, "アムロ", level: 5);
            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("Level(\"アムロ\")"));
        }

        [TestMethod]
        public void Level_NonExistentPilot_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Level(\"存在しない\")"));
        }

        [TestMethod]
        public void Level_StringReturn_ReturnsFormattedString()
        {
            var src = CreateSrc();
            CreatePilot(src, "カミーユ", level: 10);
            Assert.AreEqual("10", src.Expression.GetValueAsString("Level(\"カミーユ\")"));
        }

        [TestMethod]
        public void Level_LevelOne_ReturnsOne()
        {
            var src = CreateSrc();
            CreatePilot(src, "シン", level: 1);
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("Level(\"シン\")"));
        }

        // ──────────────────────────────────────────────
        // Morale
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Morale_ExistingPilot_ReturnsDefaultMorale()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "クワトロ");
            var expected = pilot.Morale;
            Assert.AreEqual(expected, src.Expression.GetValueAsDouble("Morale(\"クワトロ\")"));
        }

        [TestMethod]
        public void Morale_NonExistentPilot_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Morale(\"未登録\")"));
        }

        [TestMethod]
        public void Morale_AfterSettingMorale_ReturnsNewMorale()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ジュドー");
            pilot.Morale = 130;
            Assert.AreEqual(130d, src.Expression.GetValueAsDouble("Morale(\"ジュドー\")"));
        }

        [TestMethod]
        public void Morale_StringReturn_ReturnsFormattedString()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ドモン");
            pilot.Morale = 100;
            Assert.AreEqual("100", src.Expression.GetValueAsString("Morale(\"ドモン\")"));
        }

        // ──────────────────────────────────────────────
        // Plana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Plana_ExistingPilot_ReturnsPlana()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ラクス");
            var expected = pilot.Plana;
            Assert.AreEqual(expected, src.Expression.GetValueAsDouble("Plana(\"ラクス\")"));
        }

        [TestMethod]
        public void Plana_NonExistentPilot_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Plana(\"未登録\")"));
        }

        [TestMethod]
        public void Plana_StringReturn_ReturnsFormattedString()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "フレイ");
            var expected = pilot.Plana.ToString();
            Assert.IsNotNull(src.Expression.GetValueAsString("Plana(\"フレイ\")"));
        }

        // ──────────────────────────────────────────────
        // SP
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SP_ExistingPilot_ReturnsSP()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "シャア", sp: 15);
            var expected = pilot.SP;
            Assert.AreEqual(expected, src.Expression.GetValueAsDouble("SP(\"シャア\")"));
        }

        [TestMethod]
        public void SP_NonExistentPilot_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("SP(\"未登録\")"));
        }

        [TestMethod]
        public void SP_StringReturn_ReturnsFormattedString()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "マリュー", sp: 20);
            var expectedSp = pilot.SP.ToString();
            var result = src.Expression.GetValueAsString("SP(\"マリュー\")");
            Assert.IsNotNull(result);
        }

        // ──────────────────────────────────────────────
        // Skill
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Skill_PilotWithoutSkill_ReturnsZero()
        {
            var src = CreateSrc();
            CreatePilot(src, "ガロード");
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Skill(\"ガロード\",\"格闘Lv5\")"));
        }

        [TestMethod]
        public void Skill_NonExistentPilot_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Skill(\"未登録\",\"格闘\")"));
        }

        // ──────────────────────────────────────────────
        // Relation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Relation_TwoPilotsWithNoRelation_ReturnsZero()
        {
            var src = CreateSrc();
            CreatePilot(src, "アムロ");
            CreatePilot(src, "シャア");
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Relation(\"アムロ\",\"シャア\")"));
        }

        [TestMethod]
        public void Relation_FirstPilotNotExist_ReturnsZero()
        {
            var src = CreateSrc();
            CreatePilot(src, "シャア");
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Relation(\"未登録\",\"シャア\")"));
        }

        [TestMethod]
        public void Relation_BothPilotsNotExist_ReturnsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Relation(\"未登録1\",\"未登録2\")"));
        }

        [TestMethod]
        public void Relation_StringReturn_ReturnsFormattedString()
        {
            var src = CreateSrc();
            CreatePilot(src, "カガリ");
            CreatePilot(src, "アスラン");
            Assert.AreEqual("0", src.Expression.GetValueAsString("Relation(\"カガリ\",\"アスラン\")"));
        }
    }
}
