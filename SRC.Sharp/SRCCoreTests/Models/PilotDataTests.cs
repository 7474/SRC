using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class PilotDataTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src)
            {
                Name = "アムロ・レイ",
                Sex = "男性",
                Class = "エース",
                Infight = 300,
                Shooting = 280,
                Hit = 250,
                Dodge = 240,
                Intuition = 200,
                Technique = 220,
                SP = 50
            };

            Assert.AreEqual("アムロ・レイ", pd.Name);
            Assert.AreEqual("男性", pd.Sex);
            Assert.AreEqual("エース", pd.Class);
            Assert.AreEqual(300, pd.Infight);
            Assert.AreEqual(280, pd.Shooting);
            Assert.AreEqual(250, pd.Hit);
            Assert.AreEqual(240, pd.Dodge);
            Assert.AreEqual(200, pd.Intuition);
            Assert.AreEqual(220, pd.Technique);
            Assert.AreEqual(50, pd.SP);
        }

        // ──────────────────────────────────────────────
        // Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "アムロ" };
            pd.Nickname = "ホワイトベース隊員";
            Assert.AreEqual("ホワイトベース隊員", pd.Nickname);
        }

        // ──────────────────────────────────────────────
        // Bitmap / IsBitmapMissing
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bitmap_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.Bitmap = "amuro.bmp";
            Assert.AreEqual("amuro.bmp", pd.Bitmap);
        }

        [TestMethod]
        public void Bitmap_WhenMissing_ReturnsDashBmp()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.Bitmap = "amuro.bmp";
            pd.IsBitmapMissing = true;
            Assert.AreEqual("-.bmp", pd.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_ReturnsOriginalBitmap()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.Bitmap = "char.bmp";
            pd.IsBitmapMissing = true;
            Assert.AreEqual("char.bmp", pd.Bitmap0);
        }

        // ──────────────────────────────────────────────
        // AddSkill / CountSkill / IsSkillAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountSkill_NoSkills_ReturnsZero()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.Skills.Count);
        }

        [TestMethod]
        public void AddSkill_IncreasesCount()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気力アップ", 1d, "", 1);
            Assert.AreEqual(1, pd.Skills.Count);
        }

        [TestMethod]
        public void IsSkillAvailable_ExistingSkill_AtLevel_ReturnsTrue()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気力アップ", 1d, "", 1);
            Assert.IsTrue(pd.IsSkillAvailable(1, "気力アップ"));
        }

        [TestMethod]
        public void IsSkillAvailable_NonExistingSkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.IsFalse(pd.IsSkillAvailable(1, "存在しないスキル"));
        }

        [TestMethod]
        public void IsSkillAvailable_SkillBelowRequiredLevel_ReturnsFalse()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            // スキルが習得レベル5で追加
            pd.AddSkill("気力アップ", 1d, "", 5);
            // レベル3では使えない
            Assert.IsFalse(pd.IsSkillAvailable(3, "気力アップ"));
        }

        // ──────────────────────────────────────────────
        // Skill / SkillLevel / SkillData / SkillName / SkillType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Skill_ReturnsSkillNames_AtGivenLevel()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            var skills = pd.Skill(1);
            Assert.IsTrue(skills.Contains("気合"));
        }

        [TestMethod]
        public void Skill_NoSkillsAtLowLevel_ReturnsEmpty()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 5);
            var skills = pd.Skill(1);
            Assert.AreEqual("", skills.Trim());
        }

        [TestMethod]
        public void SkillLevel_ReturnsLevel_WhenSkillExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("格闘強化", 3d, "", 1);
            Assert.AreEqual(3d, pd.SkillLevel(1, "格闘強化"));
        }

        [TestMethod]
        public void SkillLevel_ReturnsZero_WhenSkillNotExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0d, pd.SkillLevel(1, "存在しない"));
        }

        [TestMethod]
        public void SkillData_ReturnsData_WhenSkillExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("ＳＰ消費減少", 2d, "集中 直感", 1);
            Assert.AreEqual("集中 直感", pd.SkillData(1, "ＳＰ消費減少"));
        }

        [TestMethod]
        public void SkillType_ReturnsSkillName_WhenExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("闘争心", 1d, "", 1);
            Assert.AreEqual("闘争心", pd.SkillType("闘争心"));
        }

        [TestMethod]
        public void SkillType_ReturnsSameName_WhenNotExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual("未知スキル", pd.SkillType("未知スキル"));
        }

        // ──────────────────────────────────────────────
        // AddSpecialPower / CountSpecialPower / IsSpecialPowerAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountSpecialPower_NoSpecialPowers_ReturnsZero()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.CountSpecialPower(1));
        }

        [TestMethod]
        public void AddSpecialPower_IncreasesCount()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("集中", 1, 10);
            Assert.AreEqual(1, pd.CountSpecialPower(1));
        }

        [TestMethod]
        public void IsSpecialPowerAvailable_ExistingPower_ReturnsTrue()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("集中", 1, 10);
            Assert.IsTrue(pd.IsSpecialPowerAvailable(1, "集中"));
        }

        [TestMethod]
        public void IsSpecialPowerAvailable_HighLevelRequired_ReturnsFalse()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("集中", 10, 10);
            Assert.IsFalse(pd.IsSpecialPowerAvailable(5, "集中"));
        }

        // ──────────────────────────────────────────────
        // AddWeapon / CountWeapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountWeapon_NoWeapons_ReturnsZero()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.CountWeapon());
        }

        [TestMethod]
        public void AddWeapon_IncreasesCount()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddWeapon("ビームライフル");
            Assert.AreEqual(1, pd.CountWeapon());
        }

        [TestMethod]
        public void AddWeapon_ReturnsWeaponWithCorrectName()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            var wd = pd.AddWeapon("ビームサーベル");
            Assert.AreEqual("ビームサーベル", wd.Name);
        }

        // ──────────────────────────────────────────────
        // AddAbility / CountAbility
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountAbility_NoAbilities_ReturnsZero()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.CountAbility());
        }

        [TestMethod]
        public void AddAbility_IncreasesCount()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddAbility("必殺技");
            Assert.AreEqual(1, pd.CountAbility());
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable / AddFeature / Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_NoFeatures_ReturnsFalse()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.IsFalse(pd.IsFeatureAvailable("ニュータイプ"));
        }

        [TestMethod]
        public void Clear_ResetsAllCollections()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            pd.AddWeapon("ビームライフル");
            pd.Clear();
            Assert.AreEqual(0, pd.Skills.Count);
            Assert.AreEqual(0, pd.CountWeapon());
        }

        // ──────────────────────────────────────────────
        // KanaName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KanaName_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.KanaName = "てすと";
            Assert.AreEqual("てすと", pd.KanaName);
        }
    }
}
