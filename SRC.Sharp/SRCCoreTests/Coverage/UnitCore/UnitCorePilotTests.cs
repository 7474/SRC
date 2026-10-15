using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Unit.pilot.cs（VB: Unit.cls のパイロット関連処理）のテスト
    /// </summary>
    [TestClass]
    public class UnitCorePilotTests
    {
        private static string UnitBlock(string name, object pilotNum = null, int mobility = 90, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, {pilotNum ?? 1}, 2
陸, 5, M, 1000, 100
{feat}
3000, 100, 800, {mobility}
AAAA, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static string PilotBlock(string name, int sp = 0, params string[] skills)
        {
            var sk = skills.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", skills);
            var spLine = sp > 0 ? $"ＳＰ, {sp}" : "ＳＰなし";
            return $@"
{name}
{name}, ぱいろっと, 男性, 汎用, AAAA, 100
{sk}
100, 100, 100, 100, 100, 100, 普通
{spLine}
-.bmp, -.mid
";
        }

        private static SRC CreateSrc(string units, string pilots)
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, units);
            UnitCoreFixture.LoadPilots(src, pilots);
            return src;
        }

        // サポートパイロットとして搭乗させる
        private static Pilot BoardSupport(SRC src, Unit u, string pname, int level = 1)
        {
            var p = src.PList.Add(pname, level, u.Party0, gid: "");
            p.Unit = u;
            u.AddSupport(p);
            p.Update();
            return p;
        }

        // ──────────────────────────────────────────────
        // 入れ替え
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplacePilot_KeepsOrderAndRetiresOriginal()
        {
            // 入れ替えたパイロットは同じ位置に入り、元のパイロットは降ろされて死亡扱いになる
            var src = CreateSrc(UnitBlock("テスト機", pilotNum: 3),
                PilotBlock("一番") + PilotBlock("二番") + PilotBlock("三番") + PilotBlock("代役"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var p1 = UnitCoreFixture.Board(src, u, "一番");
            var p2 = UnitCoreFixture.Board(src, u, "二番");
            var p3 = UnitCoreFixture.Board(src, u, "三番");
            var sub = src.PList.Add("代役", 1, "味方", gid: "");

            u.ReplacePilot(p2, sub);

            CollectionAssert.AreEqual(new[] { p1, sub, p3 }, u.Pilots.ToList());
            Assert.AreSame(u, sub.Unit);
            Assert.IsNull(p2.Unit);
            Assert.IsFalse(p2.Alive);
        }

        [TestMethod]
        public void ReplaceSupport_KeepsOrderAndRetiresOriginal()
        {
            var src = CreateSrc(UnitBlock("テスト機"),
                PilotBlock("本体") + PilotBlock("支援Ａ") + PilotBlock("支援Ｂ") + PilotBlock("代役"));
            var u = src.UList.Add("テスト機", 0, "味方");
            UnitCoreFixture.Board(src, u, "本体");
            var s1 = BoardSupport(src, u, "支援Ａ");
            var s2 = BoardSupport(src, u, "支援Ｂ");
            var sub = src.PList.Add("代役", 1, "味方", gid: "");

            u.ReplaceSupport(s1, sub);

            CollectionAssert.AreEqual(new[] { sub, s2 }, u.Supports.ToList());
            Assert.AreSame(sub, u.Support("代役"));
            Assert.AreSame(u, sub.Unit);
            Assert.IsNull(s1.Unit);
            Assert.IsFalse(s1.Alive);
            Assert.AreEqual(2, u.CountSupport());
        }

        // ──────────────────────────────────────────────
        // 追加サポート
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AdditionalSupport_CreatedWithMainPilotLevelAndExp()
        {
            // 追加サポートは第１パイロットのレベル・経験値を引き継いで作成され、2回目以降は同じものを返す
            var src = CreateSrc(UnitBlock("テスト機", features: "追加サポート=妖精"), PilotBlock("本体") + PilotBlock("妖精"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = src.PList.Add("本体", 12, "味方", gid: "");
            main.Exp = 300;
            main.Ride(u);

            var support = u.AdditionalSupport();

            Assert.AreEqual("妖精", support.Name);
            Assert.IsTrue(support.IsAdditionalSupport);
            Assert.AreEqual(12, support.Level);
            Assert.AreEqual(300, support.Exp);
            Assert.AreSame(u, support.Unit);
            Assert.AreSame(support, u.AdditionalSupport());
            // AllPilots には追加サポートも含まれる
            CollectionAssert.Contains(u.AllPilots.ToList(), support);
        }

        [TestMethod]
        public void AdditionalSupport_ReturnsNullWithoutPilotOrData()
        {
            // 他にパイロットが乗っていない場合は無効
            var src = CreateSrc(UnitBlock("テスト機", features: "追加サポート=妖精") + UnitBlock("未定義機", features: "追加サポート=未定義"),
                PilotBlock("本体") + PilotBlock("妖精"));
            var u = src.UList.Add("テスト機", 0, "味方");
            Assert.IsNull(u.AdditionalSupport());

            // パイロットデータが定義されていなければ作成できない
            var undefinedUnit = src.UList.Add("未定義機", 0, "味方");
            UnitCoreFixture.Board(src, undefinedUnit, "本体");
            Assert.IsNull(undefinedUnit.AdditionalSupport());

            // 追加サポートを持たなければ null
            var src2 = CreateSrc(UnitBlock("普通機"), PilotBlock("本体"));
            var plain = src2.UList.Add("普通機", 0, "味方");
            UnitCoreFixture.Board(src2, plain, "本体");
            Assert.IsNull(plain.AdditionalSupport());
        }

        [TestMethod]
        public void AdditionalSupport_ReusesExistingPilotInList()
        {
            // 既に作成済み（他ユニットの追加サポートでない）のパイロットは再利用される
            var src = CreateSrc(UnitBlock("テスト機", features: "追加サポート=妖精"), PilotBlock("本体") + PilotBlock("妖精"));
            var existing = src.PList.Add("妖精", 1, "味方", gid: "");
            var u = src.UList.Add("テスト機", 0, "味方");
            UnitCoreFixture.Board(src, u, "本体", level: 8);

            var support = u.AdditionalSupport();

            Assert.AreSame(existing, support);
            Assert.IsTrue(support.IsAdditionalSupport);
            Assert.AreEqual(8, support.Level);
        }

        // ──────────────────────────────────────────────
        // パイロット能力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillLevel_TakesMaximumAmongPilotsAndSupports()
        {
            // パイロット全員（サブパイロット・サポートを含む）の中で最も高いレベルを使う
            var src = CreateSrc(UnitBlock("複座機", pilotNum: 2),
                PilotBlock("主", skills: "底力Lv1, 1") + PilotBlock("副", skills: "底力Lv3, 1") + PilotBlock("支援", skills: "援護Lv2, 1"));
            var u = src.UList.Add("複座機", 0, "味方");
            UnitCoreFixture.Board(src, u, "主");
            UnitCoreFixture.Board(src, u, "副");
            BoardSupport(src, u, "支援");

            Assert.AreEqual(3d, u.SkillLevel("底力"));
            Assert.AreEqual(2d, u.SkillLevel("援護"));
            Assert.AreEqual(0d, u.SkillLevel("見切り"));
            Assert.IsTrue(u.IsSkillAvailable("底力"));
            Assert.IsTrue(u.IsSkillAvailable("援護"));
            Assert.IsFalse(u.IsSkillAvailable("見切り"));
        }

        [TestMethod]
        public void SkillLevel_NegativePilotNum_IgnoresSubPilots()
        {
            // パイロット数が「(2)」指定（内部的に負数）の場合はメインパイロットの能力のみが有効
            var src = CreateSrc(UnitBlock("複座機", pilotNum: "(2)"),
                PilotBlock("主") + PilotBlock("副", skills: "底力Lv3, 1"));
            var u = src.UList.Add("複座機", 0, "味方");
            UnitCoreFixture.Board(src, u, "主");
            UnitCoreFixture.Board(src, u, "副");
            Assert.AreEqual(2, u.CountPilot());

            Assert.AreEqual(0d, u.SkillLevel("底力"));
            Assert.IsFalse(u.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void SkillLevel_NoPilot_IsZero()
        {
            var src = CreateSrc(UnitBlock("テスト機"), PilotBlock("主"));
            var u = src.UList.Add("テスト機", 0, "味方");
            Assert.AreEqual(0d, u.SkillLevel("底力"));
            Assert.IsFalse(u.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void SkillLevel_SuperSense_UsesHigherOfSuperSenseAndPerception()
        {
            // 超感覚のレベルは「超感覚」と「知覚強化」の高い方
            var src = CreateSrc(UnitBlock("テスト機"), PilotBlock("強化人間", skills: new[] { "超感覚Lv1, 1", "知覚強化Lv3, 1" }));
            var u = src.UList.Add("テスト機", 0, "味方");
            UnitCoreFixture.Board(src, u, "強化人間");
            Assert.AreEqual(3d, u.SkillLevel("超感覚"));
        }

        [TestMethod]
        public void SkillLevel_AdditionalSupportSkillCounts()
        {
            // 追加サポートの能力も判定に含まれる
            var src = CreateSrc(UnitBlock("テスト機", features: "追加サポート=妖精"),
                PilotBlock("本体") + PilotBlock("妖精", skills: "援護Lv2, 1"));
            var u = src.UList.Add("テスト機", 0, "味方");
            UnitCoreFixture.Board(src, u, "本体");

            Assert.IsTrue(u.IsSkillAvailable("援護"));
            Assert.AreEqual(2d, u.SkillLevel("援護"));
        }

        [TestMethod]
        public void AuraLevel_AddsHalfOfSecondPilotAndFirstSupport()
        {
            // オーラ力 = メイン + ２人目/2 + サポート(1人目)/2
            var src = CreateSrc(UnitBlock("複座機", pilotNum: 2),
                PilotBlock("主", skills: "オーラLv2, 1") + PilotBlock("副", skills: "オーラLv2, 1")
                + PilotBlock("支援Ａ", skills: "オーラLv4, 1") + PilotBlock("支援Ｂ", skills: "オーラLv6, 1"));
            var u = src.UList.Add("複座機", 0, "味方");
            UnitCoreFixture.Board(src, u, "主");
            Assert.AreEqual(2d, u.AuraLevel());
            UnitCoreFixture.Board(src, u, "副");
            Assert.AreEqual(3d, u.AuraLevel());
            BoardSupport(src, u, "支援Ａ");
            BoardSupport(src, u, "支援Ｂ");
            Assert.AreEqual(5d, u.AuraLevel());
            // ユニット全体の能力レベルとしても同じ値
            Assert.AreEqual(5d, u.SkillLevel("オーラ"));
        }

        [TestMethod]
        public void PsychicLevel_IsLimitedByPsychicDriveLevel()
        {
            // 超能力 = メイン + ２人目/2、サイキックドライブのレベルで上限
            var src = CreateSrc(UnitBlock("複座機", pilotNum: 2, features: "サイキックドライブLv3"),
                PilotBlock("主", skills: "超能力Lv3, 1") + PilotBlock("副", skills: "超能力Lv2, 1"));
            var u = src.UList.Add("複座機", 0, "味方");
            UnitCoreFixture.Board(src, u, "主");
            UnitCoreFixture.Board(src, u, "副");

            Assert.AreEqual(3d, u.PsychicLevel());
            Assert.AreEqual(4d, u.PsychicLevel(no_limit: true));
            Assert.AreEqual(3d, u.SkillLevel("超能力"));
        }

        [TestMethod]
        public void SyncLevel_IsLimitedBySynchroDriveAndAffectsMobility()
        {
            // 同調率はメインパイロットの同調率（基本値＋レベル）、シンクロドライブのレベルで上限
            // 運動性は (同調率 - 50) / 2 だけ修正される
            var src = CreateSrc(
                UnitBlock("シンクロ機", mobility: 90, features: "シンクロドライブLv35")
                + UnitBlock("無制限機", mobility: 90, features: "シンクロドライブ"),
                PilotBlock("適格者", skills: "同調率Lv30, 1") + PilotBlock("適格者２", skills: "同調率Lv30, 1"));
            var limited = src.UList.Add("シンクロ機", 0, "味方");
            UnitCoreFixture.Board(src, limited, "適格者", level: 10);
            var free = src.UList.Add("無制限機", 0, "味方");
            UnitCoreFixture.Board(src, free, "適格者２", level: 30);

            Assert.AreEqual(35d, limited.SyncLevel());
            Assert.AreEqual(40d, limited.SyncLevel(no_limit: true));
            Assert.AreEqual(35d, limited.SkillLevel("同調率"));
            Assert.AreEqual(83, limited.get_Mobility(""));
            Assert.AreEqual(60d, free.SyncLevel());
            Assert.AreEqual(95, free.get_Mobility(""));
        }

        [TestMethod]
        public void PlanaLevel_IsLimitedByConverterLevel()
        {
            // 霊力はメインパイロットの霊力値、霊力変換器のレベルで上限
            var src = CreateSrc(UnitBlock("霊力機", features: "霊力変換器Lv5"), PilotBlock("霊能者", skills: "霊力Lv100, 1"));
            var u = src.UList.Add("霊力機", 0, "味方");
            var p = UnitCoreFixture.Board(src, u, "霊能者");
            Assert.IsTrue(p.Plana > 5);

            Assert.AreEqual(5d, u.PlanaLevel());
            Assert.AreEqual((double)p.Plana, u.PlanaLevel(no_limit: true));
            Assert.AreEqual(5d, u.SkillLevel("霊力"));
        }

        [TestMethod]
        public void LevelFunctions_NoPilot_ReturnZero()
        {
            var src = CreateSrc(UnitBlock("テスト機"), PilotBlock("主"));
            var u = src.UList.Add("テスト機", 0, "味方");
            Assert.AreEqual(0d, u.AuraLevel());
            Assert.AreEqual(0d, u.PsychicLevel());
            Assert.AreEqual(0d, u.SyncLevel());
            Assert.AreEqual(0d, u.PlanaLevel());
        }

        // ──────────────────────────────────────────────
        // スペシャルパワーを使えるパイロット
        // ──────────────────────────────────────────────

        private static SRC CreateSpSrc(string unitBlock)
        {
            var src = CreateSrc(unitBlock,
                PilotBlock("主", sp: 50) + PilotBlock("副", sp: 40) + PilotBlock("支援Ａ", sp: 30) + PilotBlock("支援Ｂ", sp: 30) + PilotBlock("無能"));
            foreach (var pname in new[] { "主", "副", "支援Ａ", "支援Ｂ" })
            {
                src.PDList.Item(pname).AddSpecialPower("集中", 1, 10);
            }
            return src;
        }

        [TestMethod]
        public void PilotsHaveSpecialPower_ListsPilotsWithSpecialPowers()
        {
            // メイン・サブパイロットのうちスペシャルパワーを持つ者を列挙する
            var src = CreateSpSrc(UnitBlock("三座機", pilotNum: 3));
            var u = src.UList.Add("三座機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "主");
            UnitCoreFixture.Board(src, u, "無能");
            var sub = UnitCoreFixture.Board(src, u, "副");

            CollectionAssert.AreEqual(new[] { main, sub }, u.PilotsHaveSpecialPower().ToList());
        }

        [TestMethod]
        public void PilotsHaveSpecialPower_AdditionalPilotWithoutSp_UsesFirstPilot()
        {
            // ２人乗り以上でメインパイロット（追加パイロット）がＳＰを持たない場合は１番目のパイロットを使う
            var src = CreateSpSrc(UnitBlock("複座機", pilotNum: 2, features: "追加パイロット=無能"));
            var u = src.UList.Add("複座機", 0, "味方");
            var first = UnitCoreFixture.Board(src, u, "主");
            var second = UnitCoreFixture.Board(src, u, "副");
            Assert.AreEqual("無能", u.MainPilot().Name);

            CollectionAssert.AreEqual(new[] { first, second }, u.PilotsHaveSpecialPower().ToList());
        }

        [TestMethod]
        [Ignore("移植の不具合: PilotsHaveSpecialPower が Supports.Skip(1) で１人目のサポートを除外している（VB は Support(1) から全て）")]
        public void PilotsHaveSpecialPower_IncludesAllSupports()
        {
            // VB Command.bas: サポートパイロットは i - CountPilot = 1 .. CountSupport の全員が対象
            var src = CreateSpSrc(UnitBlock("テスト機"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "主");
            var s1 = BoardSupport(src, u, "支援Ａ");
            var s2 = BoardSupport(src, u, "支援Ｂ");

            CollectionAssert.AreEqual(new[] { main, s1, s2 }, u.PilotsHaveSpecialPower().ToList());
        }
    }
}
