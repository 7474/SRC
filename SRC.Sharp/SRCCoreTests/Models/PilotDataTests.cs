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
    }
}
