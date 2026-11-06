using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotData を元の SRC（PilotData.cls）の動作と突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class PilotDataVBMatchTests
    {
        private static PilotData CreatePilotData()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new PilotData(src) { Name = "テスト" };
        }

        [TestMethod]
        public void Skill_SameNameListedOnce()
        {
            // 同じ名前の特殊能力は 1 つにまとめる（VB: GoTo NextLoop で次の特殊能力へ）
            var pd = CreatePilotData();
            pd.AddSkill("底力", 1, "", 1);
            pd.AddSkill("切り払い", 1, "", 1);
            pd.AddSkill("底力", 2, "", 10);
            pd.AddSkill("見切り", 1, "", 20);

            Assert.AreEqual("底力 切り払い ", pd.Skill(10));
            Assert.AreEqual("底力 切り払い 見切り ", pd.Skill(20));
        }

        [TestMethod]
        public void Skill_MoreThan32Skills_ListsAll()
        {
            // 元の SRC は 32 個までの配列に入れる。SRC# は個数を制限しない
            var pd = CreatePilotData();
            for (var i = 1; i <= 33; i++)
            {
                pd.AddSkill("能力" + i, 1, "", 1);
            }

            Assert.AreEqual(33, pd.Skill(1).Trim().Split(' ').Length);
        }

        [TestMethod]
        public void SpecialPower_NotFound_ReturnsEmpty()
        {
            var pd = CreatePilotData();
            pd.AddSpecialPower("集中", 10, 0);

            Assert.AreEqual("", pd.SpecialPower(1, 1));
            Assert.AreEqual("集中", pd.SpecialPower(10, 1));
        }

        [TestMethod]
        public void IsSpecialPowerAvailable_Duplicate_UsesFirstDefinition()
        {
            // 同じスペシャルパワーが複数あるときは、最初の定義の獲得レベルだけを見る（VB: 見つけたら Exit Function）
            var pd = CreatePilotData();
            pd.AddSpecialPower("集中", 30, 0);
            pd.AddSpecialPower("集中", 10, 0);

            Assert.IsFalse(pd.IsSpecialPowerAvailable(20, "集中"));
            Assert.IsTrue(pd.IsSpecialPowerAvailable(30, "集中"));
        }

        [TestMethod]
        public void FeatureName_LevelZeroOrNegative_AppendsLevel()
        {
            // パイロットデータは DEFAULT_LEVEL でなければレベルを付ける（UnitData の Level > 0 とは異なる）
            var pd = CreatePilotData();
            pd.AddFeature("テスト能力Lv0");
            pd.AddFeature("別能力Lv-1");
            pd.AddFeature("無印能力");

            Assert.AreEqual("テスト能力Lv0", pd.FeatureName("テスト能力"));
            Assert.AreEqual("別能力Lv-1", pd.FeatureName("別能力"));
            Assert.AreEqual("無印能力", pd.FeatureName("無印能力"));
        }
    }
}
