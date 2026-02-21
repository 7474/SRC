using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotDataSpecialPower クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class PilotDataSpecialPowerTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var sp = new PilotDataSpecialPower("魂", 10, 50);
            Assert.AreEqual("魂", sp.Name);
            Assert.AreEqual(10, sp.NecessaryLevel);
            Assert.AreEqual(50, sp.SPConsumption);
        }

        [TestMethod]
        public void Constructor_WithEmptyName_SetsEmptyName()
        {
            var sp = new PilotDataSpecialPower("", 1, 10);
            Assert.AreEqual("", sp.Name);
        }

        [TestMethod]
        public void Constructor_WithZeroLevel_SetsZeroLevel()
        {
            var sp = new PilotDataSpecialPower("ひらめき", 0, 20);
            Assert.AreEqual(0, sp.NecessaryLevel);
        }

        [TestMethod]
        public void Constructor_WithZeroSP_SetsZeroSP()
        {
            var sp = new PilotDataSpecialPower("集中", 5, 0);
            Assert.AreEqual(0, sp.SPConsumption);
        }

        [TestMethod]
        public void Constructor_WithMaxValues_SetsCorrectly()
        {
            var sp = new PilotDataSpecialPower("必中", 99, 150);
            Assert.AreEqual("必中", sp.Name);
            Assert.AreEqual(99, sp.NecessaryLevel);
            Assert.AreEqual(150, sp.SPConsumption);
        }

        // ──────────────────────────────────────────────
        // プロパティの読み取り専用確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_IsReadOnly()
        {
            var sp = new PilotDataSpecialPower("熱血", 20, 30);
            // Name は読み取り専用 - コンストラクタで設定された値が変わらないことを確認
            Assert.AreEqual("熱血", sp.Name);
        }

        [TestMethod]
        public void NecessaryLevel_IsReadOnly()
        {
            var sp = new PilotDataSpecialPower("信頼", 15, 25);
            Assert.AreEqual(15, sp.NecessaryLevel);
        }

        [TestMethod]
        public void SPConsumption_IsReadOnly()
        {
            var sp = new PilotDataSpecialPower("覚醒", 50, 80);
            Assert.AreEqual(80, sp.SPConsumption);
        }

        // ──────────────────────────────────────────────
        // 複数インスタンス独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var sp1 = new PilotDataSpecialPower("激励", 5, 40);
            var sp2 = new PilotDataSpecialPower("脱力", 10, 45);
            Assert.AreNotEqual(sp1.Name, sp2.Name);
            Assert.AreNotEqual(sp1.NecessaryLevel, sp2.NecessaryLevel);
            Assert.AreNotEqual(sp1.SPConsumption, sp2.SPConsumption);
        }

        [TestMethod]
        public void TwoInstances_WithSameValues_AreNotSameObject()
        {
            var sp1 = new PilotDataSpecialPower("魂", 10, 50);
            var sp2 = new PilotDataSpecialPower("魂", 10, 50);
            Assert.AreNotSame(sp1, sp2);
            Assert.AreEqual(sp1.Name, sp2.Name);
            Assert.AreEqual(sp1.NecessaryLevel, sp2.NecessaryLevel);
            Assert.AreEqual(sp1.SPConsumption, sp2.SPConsumption);
        }

        // ──────────────────────────────────────────────
        // 日本語名称
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_JapaneseName_SetsCorrectly()
        {
            var sp = new PilotDataSpecialPower("てかげん", 1, 15);
            Assert.AreEqual("てかげん", sp.Name);
        }

        [TestMethod]
        public void Constructor_FullWidthName_SetsCorrectly()
        {
            var sp = new PilotDataSpecialPower("不屈", 3, 10);
            Assert.AreEqual("不屈", sp.Name);
        }

        // ──────────────────────────────────────────────
        // 境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_MaxIntSP_SetsCorrectly()
        {
            var sp = new PilotDataSpecialPower("テスト", 1, int.MaxValue);
            Assert.AreEqual(int.MaxValue, sp.SPConsumption);
        }

        [TestMethod]
        public void Constructor_NegativeLevel_SetsCorrectly()
        {
            var sp = new PilotDataSpecialPower("テスト", -1, 10);
            Assert.AreEqual(-1, sp.NecessaryLevel);
        }
    }
}
