using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot.props.cs で定義されるプロパティのユニットテスト
    /// </summary>
    [TestClass]
    public class PilotPropsTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // Sex (性別)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sex_Default_ReturnsDataSex()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("男パイロット");
            pd.SP = 10;
            pd.Sex = "男";
            var pilot = src.PList.Add("男パイロット", 1, "味方");
            Assert.AreEqual("男", pilot.Sex);
        }

        [TestMethod]
        public void Sex_Female_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("女パイロット");
            pd.SP = 10;
            pd.Sex = "女";
            var pilot = src.PList.Add("女パイロット", 1, "味方");
            Assert.AreEqual("女", pilot.Sex);
        }

        [TestMethod]
        public void Sex_EmptySex_ReturnsEmptyString()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("無性別パイロット");
            pd.SP = 10;
            pd.Sex = "";
            var pilot = src.PList.Add("無性別パイロット", 1, "味方");
            Assert.AreEqual("", pilot.Sex);
        }

        // ──────────────────────────────────────────────
        // Class (ユニットクラス)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Class_Default_ReturnsDataClass()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("エースパイロット");
            pd.SP = 10;
            pd.Class = "エース";
            var pilot = src.PList.Add("エースパイロット", 1, "味方");
            Assert.AreEqual("エース", pilot.Class);
        }

        [TestMethod]
        public void Class_EmptyClass_ReturnsEmpty()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("無クラスパイロット");
            pd.SP = 10;
            pd.Class = "";
            var pilot = src.PList.Add("無クラスパイロット", 1, "味方");
            Assert.AreEqual("", pilot.Class);
        }

        // ──────────────────────────────────────────────
        // ExpValue (経験値)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExpValue_Default_ReturnsDataExpValue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("経験値テスト");
            pd.SP = 10;
            pd.ExpValue = 150;
            var pilot = src.PList.Add("経験値テスト", 1, "味方");
            Assert.AreEqual(150, pilot.ExpValue);
        }

        [TestMethod]
        public void ExpValue_Zero_ReturnsZero()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("経験値ゼロ");
            pd.SP = 10;
            pd.ExpValue = 0;
            var pilot = src.PList.Add("経験値ゼロ", 1, "味方");
            Assert.AreEqual(0, pilot.ExpValue);
        }

        // ──────────────────────────────────────────────
        // Personality (性格)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Personality_Default_ReturnsDataPersonality()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("熱血パイロット");
            pd.SP = 10;
            pd.Personality = "強気";
            var pilot = src.PList.Add("熱血パイロット", 1, "味方");
            Assert.AreEqual("強気", pilot.Personality);
        }

        [TestMethod]
        public void Personality_EmptyPersonality_ReturnsEmpty()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("無性格パイロット");
            pd.SP = 10;
            pd.Personality = "";
            var pilot = src.PList.Add("無性格パイロット", 1, "味方");
            Assert.AreEqual("", pilot.Personality);
        }

        // ──────────────────────────────────────────────
        // BGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BGM_Default_ReturnsDataBGM()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("BGMパイロット");
            pd.SP = 10;
            pd.BGM = "battle_theme.mp3";
            var pilot = src.PList.Add("BGMパイロット", 1, "味方");
            Assert.AreEqual("battle_theme.mp3", pilot.BGM);
        }

        [TestMethod]
        public void BGM_EmptyBGM_ReturnsEmpty()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("BGMなし");
            pd.SP = 10;
            pd.BGM = "";
            var pilot = src.PList.Add("BGMなし", 1, "味方");
            Assert.AreEqual("", pilot.BGM);
        }

        // ──────────────────────────────────────────────
        // Defense (防御力)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Defense_Level1_NoSkills_ReturnsBaseValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            // 防御力成長オプションなし → 100 + 5 * SkillLevel("耐久", "") = 100 + 0 = 100
            Assert.AreEqual(100, pilot.Defense);
        }

        [TestMethod]
        public void Defense_HighLevel_NoSkills_StillBaseValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 10);
            // 防御力成長オプションなし → 100
            Assert.AreEqual(100, pilot.Defense);
        }

        // ──────────────────────────────────────────────
        // MessageType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageType_NoSkill_ReturnsName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "アムロ");
            // メッセージスキルがない場合は Name を返す
            Assert.AreEqual("アムロ", pilot.MessageType);
        }

        // ──────────────────────────────────────────────
        // IsFix
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFix_WhenNotDefined_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "カミーユ");
            // Fix(カミーユ) グローバル変数が未定義 → false
            Assert.IsFalse(pilot.IsFix);
        }

        [TestMethod]
        public void IsFix_WhenDefined_ReturnsTrue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "シン");
            // Fix(シン) グローバル変数を定義
            src.Expression.DefineGlobalVariable("Fix(シン)");
            Assert.IsTrue(pilot.IsFix);
        }

        // ──────────────────────────────────────────────
        // IsRidingAdSupport
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsRidingAdSupport_WhenNoUnit_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "無乗機");
            // Unit が null の場合は false
            Assert.IsFalse(pilot.IsRidingAdSupport);
        }
    }
}
