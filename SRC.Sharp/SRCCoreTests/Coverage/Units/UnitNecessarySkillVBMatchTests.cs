using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 必要技能の判定（Unit.cls の IsNecessarySkillSatisfied2）を元の SRC の動作を期待値にして確かめるテスト。
    /// </summary>
    [TestClass]
    public class UnitNecessarySkillVBMatchTests
    {
        [TestMethod]
        public void Sex_ChecksFirstPilotEvenWhenPilotIsGiven()
        {
            // VB: 「男性」「女性」は PilotNum > 1 なら Pilot(1) から CountPilot までを調べる。
            // 判定に使うパイロット（p）がサブパイロットでも、メインの Pilot(1) の性別を見る
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン", "男性");
            h.DefinePilot("ミナ", "女性");
            h.LoadUnits(string.Join("\n",
                "複座機",
                "複座機, ふくざき, 汎用, 2, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "3000, 200, 1000, 100",
                "AAAA, -.bmp",
                ""));
            var u = h.Create("複座機", "アレン");
            var sub = h.Src.PList.Add("ミナ", 1, "味方");
            sub.Ride(u);

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("男性", sub));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("女性", sub));
        }
    }
}
