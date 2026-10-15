using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.Pilots.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.Pilots
{
    /// <summary>
    /// Pilot.Update（元の SRC の Pilot.Update）のテスト。
    /// 使用可能な特殊能力一覧の作成、レベルによる能力値の成長、特殊能力・ユニットによる修正値を確認する。
    /// パイロットデータの能力値はすべて 100（PilotSkillStatusFixture.AddPilotData）。
    /// </summary>
    [TestClass]
    public class PilotSkillStatus_UpdateTests
    {
        private const double NoLv = Constants.DEFAULT_LEVEL;

        // ──────────────────────────────────────────────
        // 特殊能力一覧の作成
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_習得レベルに達していない能力は使用できない()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("底力", 2d, "", 5);
            var p = AddPilot(src, "テストパイロット", 3);
            Assert.IsFalse(p.IsSkillAvailable("底力"));
            Assert.AreEqual(0, p.CountSkill());

            // Level の変更で Update が呼ばれ、習得レベルに達する
            p.Level = 5;
            Assert.IsTrue(p.IsSkillAvailable("底力"));
            Assert.AreEqual(2d, p.SkillLevel("底力"));
        }

        [TestMethod]
        public void Update_同名能力は習得レベルの高いものが優先される()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("超感覚", 1d, "", 1);
            pd.AddSkill("超感覚", 3d, "", 10);
            var p = AddPilot(src, "テストパイロット", 5);
            Assert.AreEqual(1d, p.SkillLevel("超感覚"));
            Assert.AreEqual(1, p.CountSkill());

            p.Level = 10;
            Assert.AreEqual(3d, p.SkillLevel("超感覚"));
            Assert.AreEqual(1, p.CountSkill());
        }

        [TestMethod]
        [Ignore("移植の不具合: ハンター等の重複判定が x.StrData == x.StrData と自己比較になっておりデータ違いの同名能力が登録されない")]
        public void Update_ハンターはデータが異なれば複数持てる()
        {
            // VB: If .StrData = skill_data(i).StrData Then Exit For （データまで同一の場合のみ同じ能力と見なす）
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("ハンター", 1d, "ハンター 竜", 1);
            pd.AddSkill("ハンター", 1d, "ハンター 魔物", 1);
            var p = AddPilot(src, "テストパイロット");
            Assert.AreEqual(2, p.CountSkill());
        }

        [TestMethod]
        public void Update_SetSkillで新しい能力を付加できる()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット");

            SetSkill(src, p, "集中力", "2");
            SetSkill(src, p, "闘争心", "-1");
            p.Update();

            Assert.AreEqual(2d, p.SkillLevel("集中力"));
            Assert.IsTrue(p.IsSkillLevelSpecified("集中力"));
            // レベル -1 はレベル指定なし
            Assert.AreEqual(1d, p.SkillLevel("闘争心"));
            Assert.IsFalse(p.IsSkillLevelSpecified("闘争心"));
        }

        [TestMethod]
        public void Update_SetSkillで既存の能力のレベルとデータを置き換える()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("底力", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット");

            SetSkill(src, p, "底力", "3 ガッツ");
            p.Update();

            Assert.AreEqual(1, p.CountSkill());
            Assert.AreEqual(3d, p.SkillLevel("底力"));
            Assert.AreEqual("ガッツ", p.SkillData("底力"));
            Assert.AreEqual("ガッツLv3", p.SkillName("底力"));
        }

        [TestMethod]
        public void Update_使用不能状態の能力は封印される()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("底力", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット");
            AddUnitData(src, "テスト機体");
            var u = CreateUnit(src, "テスト機体", "味方", p);
            Assert.IsTrue(p.IsSkillAvailable("底力"));

            u.AddCondition("底力使用不能", 2);
            p.Update();

            Assert.IsFalse(p.IsSkillAvailable("底力"));
        }

        // ──────────────────────────────────────────────
        // 基本値の成長
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_レベルによる能力値の成長()
        {
            // 格闘・射撃・技量・反応は +Lv、命中・回避は +2Lv
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット", 10);

            Assert.AreEqual(110, p.Infight);
            Assert.AreEqual(110, p.Shooting);
            Assert.AreEqual(120, p.Hit);
            Assert.AreEqual(120, p.Dodge);
            Assert.AreEqual(110, p.Technique);
            Assert.AreEqual(110, p.Intuition);
        }

        [TestMethod]
        public void Update_成長系能力で成長率が上がる()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("格闘成長", 1d, "", 1);
            pd.AddSkill("射撃成長", 2d, "", 1);
            pd.AddSkill("命中成長", 1d, "", 1);
            pd.AddSkill("回避成長", 1d, "", 1);
            pd.AddSkill("技量成長", 1d, "", 1);
            pd.AddSkill("反応成長", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット", 10);

            Assert.AreEqual(120, p.Infight);
            Assert.AreEqual(130, p.Shooting);
            Assert.AreEqual(130, p.Hit);
            Assert.AreEqual(130, p.Dodge);
            Assert.AreEqual(120, p.Technique);
            Assert.AreEqual(120, p.Intuition);
        }

        [TestMethod]
        public void Update_攻撃力低成長オプションでは格闘射撃の成長が半分()
        {
            // VB: InfightBase + (lv * (1 + 2 * SkillLevel("格闘成長"))) \ 2
            var src = CreateSrc();
            DefineOption(src, "攻撃力低成長");
            AddPilotData(src, "テストパイロット").AddSkill("格闘成長", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット", 10);

            Assert.AreEqual(115, p.Infight);
            Assert.AreEqual(105, p.Shooting);
            // 命中は通常通り
            Assert.AreEqual(120, p.Hit);
        }

        [TestMethod]
        public void Update_追加レベルはレベル成長に加算される()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("追加レベル", 5d, "", 1);
            var p = AddPilot(src, "テストパイロット", 10);

            Assert.AreEqual(115, p.Infight);
            Assert.AreEqual(130, p.Hit);
        }

        [TestMethod]
        public void Update_能力ＵＰとＤＯＷＮで基本値が増減する()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("格闘ＵＰ", 7d, "", 1);
            pd.AddSkill("射撃ＵＰ", 1d, "", 1);
            pd.AddSkill("命中ＵＰ", 2d, "", 1);
            pd.AddSkill("回避ＤＯＷＮ", 3d, "", 1);
            pd.AddSkill("技量ＤＯＷＮ", 4d, "", 1);
            pd.AddSkill("反応ＵＰ", 5d, "", 1);
            var p = AddPilot(src, "テストパイロット", 1);

            Assert.AreEqual(108, p.InfightBase);
            Assert.AreEqual(102, p.ShootingBase);
            Assert.AreEqual(104, p.HitBase);
            Assert.AreEqual(99, p.DodgeBase);
            Assert.AreEqual(97, p.TechniqueBase);
            Assert.AreEqual(106, p.IntuitionBase);
        }

        [TestMethod]
        public void Update_基本値は9999を上限とする()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.Infight = 9990;
            pd.Hit = 9998;
            var p = AddPilot(src, "テストパイロット", 20);

            Assert.AreEqual(9999, p.InfightBase);
            Assert.AreEqual(9999, p.Infight);
            Assert.AreEqual(9999, p.Hit);
        }

        // ──────────────────────────────────────────────
        // パイロット用特殊能力による修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_超感覚などは命中回避に2Lv加算3()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("超感覚", 2d, "", 1);
            pd.AddSkill("知覚強化", 1d, "", 1);
            pd.AddSkill("念力", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット", 1);

            // (2*2+3) + (2*1+3) + (2*1+3)
            Assert.AreEqual(17, p.HitMod);
            Assert.AreEqual(17, p.DodgeMod);
            Assert.AreEqual(102 + 17, p.Hit);
        }

        [TestMethod]
        public void Update_超反応とサイボーグ悟り超能力の修正()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("超反応", 2d, "", 1);
            pd.AddSkill("サイボーグ", NoLv, "", 1);
            pd.AddSkill("悟り", NoLv, "", 1);
            pd.AddSkill("超能力", NoLv, "", 1);
            var p = AddPilot(src, "テストパイロット", 1);

            // 2*2 + 5 + 10 + 5
            Assert.AreEqual(24, p.HitMod);
            Assert.AreEqual(24, p.DodgeMod);
            Assert.AreEqual(0, p.InfightMod);
        }

        // ──────────────────────────────────────────────
        // ユニットによる修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_出撃中ユニットのサブパイロットのサポート能力がメインに加算される()
        {
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            var sd = AddPilotData(src, "サブ");
            sd.AddSkill("格闘サポート", 2d, "", 1);
            sd.AddSkill("射撃サポート", 1d, "", 1);
            sd.AddSkill("サポート", 1d, "", 1);
            sd.AddSkill("命中サポート", 1d, "", 1);
            sd.AddSkill("回避サポート", 2d, "", 1);
            sd.AddSkill("技量サポート", 3d, "", 1);
            sd.AddSkill("反応サポート", 1d, "", 1);
            var main = AddPilot(src, "メイン");
            var sub = AddPilot(src, "サブ");
            AddUnitData(src, "複座機");
            var u = CreateUnit(src, "複座機", "味方", main, sub);

            // 出撃していなければサポートは受けられない
            u.Status = "待機";
            main.Update();
            Assert.AreEqual(0, main.InfightMod);

            u.Status = "出撃";
            main.Update();
            Assert.AreEqual(4, main.InfightMod);
            Assert.AreEqual(2, main.ShootingMod);
            Assert.AreEqual(3 + 2, main.HitMod);
            Assert.AreEqual(3 + 4, main.DodgeMod);
            Assert.AreEqual(6, main.TechniqueMod);
            Assert.AreEqual(2, main.IntuitionMod);

            // サブパイロット自身はサポートを受けない
            sub.Update();
            Assert.AreEqual(0, sub.InfightMod);
        }

        [TestMethod]
        public void Update_魔力を持つメインには魔力サポートが射撃に加算される()
        {
            var src = CreateSrc();
            AddPilotData(src, "メイン").AddSkill("術", 1d, "", 1);
            var sd = AddPilotData(src, "サブ");
            sd.AddSkill("魔力サポート", 2d, "", 1);
            sd.AddSkill("射撃サポート", 5d, "", 1);
            var main = AddPilot(src, "メイン");
            var sub = AddPilot(src, "サブ");
            AddUnitData(src, "複座機");
            var u = CreateUnit(src, "複座機", "味方", main, sub);
            u.Status = "出撃";

            main.Update();

            Assert.AreEqual(4, main.ShootingMod);
        }

        [TestMethod]
        public void Update_サポートパイロットのサポート能力も加算される()
        {
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            AddPilotData(src, "サポート役").AddSkill("格闘サポート", 1d, "", 1);
            var main = AddPilot(src, "メイン");
            var sup = AddPilot(src, "サポート役");
            AddUnitData(src, "テスト機体");
            var u = CreateUnit(src, "テスト機体", "味方", main);
            u.AddSupport(sup);
            sup.Unit = u;
            u.Status = "出撃";

            main.Update();

            Assert.AreEqual(2, main.InfightMod);
        }

        [TestMethod]
        public void Update_ユニットの能力強化は気力条件を満たすと5Lv加算()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット", 1);
            AddUnitData(src, "テスト機体",
                "格闘強化Lv2=格闘強化 120",
                "射撃強化Lv1",
                "命中強化Lv1",
                "回避強化Lv2",
                "技量強化Lv1",
                "反応強化Lv3");
            CreateUnit(src, "テスト機体", "味方", p);

            p.Morale = 100;
            p.Update();
            // 気力 120 未満なので格闘強化は無効、他は気力条件なし
            Assert.AreEqual(0, p.InfightMod);
            Assert.AreEqual(5, p.ShootingMod);
            Assert.AreEqual(5, p.HitMod);
            Assert.AreEqual(10, p.DodgeMod);
            Assert.AreEqual(5, p.TechniqueMod);
            Assert.AreEqual(15, p.IntuitionMod);

            p.Morale = 120;
            p.Update();
            Assert.AreEqual(10, p.InfightMod);
            Assert.AreEqual(101 + 10, p.Infight);
        }

        [TestMethod]
        [Ignore("移植の不具合: Update のパイロット地形適応変更による地形適応の修正が未移植（コメントアウト）")]
        public void Update_パイロット地形適応変更で地形適応が変わる()
        {
            // VB: 修正値がプラスなら A を上限に加算、マイナスなら常に加算
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット"); // 地形適応 BBBB
            var p = AddPilot(src, "テストパイロット", 1);
            AddUnitData(src, "テスト機体", "パイロット地形適応変更=1 2 0 -1");
            CreateUnit(src, "テスト機体", "味方", p);

            p.Update();

            Assert.AreEqual("AABC", p.Adaption);
        }
    }
}
