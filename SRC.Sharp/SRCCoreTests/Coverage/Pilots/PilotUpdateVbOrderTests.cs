using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// 元の SRC の Pilot.cls の Update（特殊能力の一覧の作成）と Nickname を期待値にしたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class PilotUpdateVbOrderTests
    {
        [TestMethod]
        public void Update_習得レベルの高い同名の能力は元の位置で置き換える()
        {
            // VB: ElseIf .NecessaryLevel > skill_data(i).NecessaryLevel Then Set skill_data(i) = sd
            // 番号 i はそのままなので、一覧の順番は最初に出てきた位置になる
            var src = CreateSrc();
            var pd = AddPilotData(src, "テスト");
            pd.AddSkill("格闘ＵＰ", 1d, "", 1);
            pd.AddSkill("射撃ＵＰ", 1d, "", 1);
            pd.AddSkill("格闘ＵＰ", 2d, "", 5);
            var p = AddPilot(src, "テスト", 10);

            Assert.AreEqual(2, p.CountSkill());
            Assert.AreEqual("格闘ＵＰ", p.Skill(1));
            Assert.AreEqual("射撃ＵＰ", p.Skill(2));
            Assert.AreEqual(2d, p.SkillLevel("格闘ＵＰ"));
        }

        [TestMethod]
        public void Update_封印した同種の能力の後ろの能力は番号付きのキーで登録する()
        {
            // VB: SetSkill のレベル0は Set skill_data(j) = Nothing で、skill_name(j) は残る。
            // 登録のときは skill_name で前に同じ名前があるかを見るので、2 つ目のハンターは "ハンター:2" のキーになり、
            // colSkill.Item("ハンター") は見つからない
            var src = CreateSrc();
            var pd = AddPilotData(src, "テスト");
            pd.AddSkill("ハンター", 1d, "A", 1);
            pd.AddSkill("ハンター", 1d, "B", 1);
            var p = AddPilot(src, "テスト");
            Assert.IsTrue(p.IsSkillAvailable("ハンター"));

            SetSkill(src, p, "ハンター", "0 A");
            p.Update();

            Assert.AreEqual(1, p.CountSkill());
            Assert.AreEqual("ハンター", p.Skill(1));
            Assert.AreEqual("B", p.SkillData(1));
            Assert.IsFalse(p.IsSkillAvailable("ハンター"));
        }

        [TestMethod]
        public void Update_SetSkillで封印した能力を同じ番号でまた付け直せる()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テスト");
            pd.AddSkill("格闘ＵＰ", 1d, "", 1);
            pd.AddSkill("射撃ＵＰ", 1d, "", 1);
            var p = AddPilot(src, "テスト");

            SetSkill(src, p, "格闘ＵＰ", "3");
            p.Update();

            Assert.AreEqual("格闘ＵＰ", p.Skill(1));
            Assert.AreEqual("射撃ＵＰ", p.Skill(2));
            Assert.AreEqual(3d, p.SkillLevel("格闘ＵＰ"));
        }

        [TestMethod]
        public void Nickname_無限呼び出し防止で消すのはNicknameとnicknameの書き方だけ()
        {
            // VB: ReplaceString Nickname, "Nickname()", "" と ReplaceString Nickname, "nickname()", ""
            // ReplaceString は InStr（バイナリ比較）なので、ほかの大文字・小文字の組み合わせは消さない
            var src = CreateSrc();
            AddPilotData(src, "アムロ").Nickname = "アムロ";
            var p = AddPilot(src, "アムロ");
            AddUnitData(src, "ガンダム", "パイロット愛称=$(愛称)NICKNAME()");
            CreateUnit(src, "ガンダム", "味方", p);

            Assert.AreEqual("アムロNICKNAME()", p.get_Nickname(true));
        }
    }
}
