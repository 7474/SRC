using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// Pilot.skill.cs（元の SRC の Pilot.cls「特殊能力関連処理」）のテスト。
    /// パイロット自身の特殊能力と、ユニットの特殊状態「○○付加」「○○強化」（付加２・強化２を含む）
    /// による修正を確認する。
    /// </summary>
    [TestClass]
    public class PilotSkillStatus_SkillTests
    {
        private const double NoLv = Constants.DEFAULT_LEVEL;

        // スキルを1つ持つパイロットを作る（sname が null ならスキルなし）
        private static (SRC src, Pilot p) CreatePilotWithSkill(string sname = null, double slevel = NoLv, string sdata = "")
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            if (sname != null)
            {
                pd.AddSkill(sname, slevel, sdata, 1);
            }
            var p = AddPilot(src, "テストパイロット");
            return (src, p);
        }

        // パイロットを1人乗せたユニットを作る
        private static Unit Board(SRC src, Pilot p)
        {
            AddUnitData(src, "テスト機体");
            return CreateUnit(src, "テスト機体", "味方", p);
        }

        // ──────────────────────────────────────────────
        // IsSkillAvailable / HasSupportSkill
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSkillAvailable_付加状態で未所有の能力が使用可能になる()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            Assert.IsFalse(p.IsSkillAvailable("底力"));

            u.AddCondition("底力付加", -1);
            Assert.IsTrue(p.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void IsSkillAvailable_付加２でも使用可能になる()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加２", -1);
            Assert.IsTrue(p.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void IsSkillAvailable_強化はレベルが正の場合のみ使用可能になる()
        {
            // VB: If .ConditionLevel(sname & "強化") > 0 Then IsSkillAvailable = True
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力強化", -1, 0d);
            Assert.IsFalse(p.IsSkillAvailable("底力"));

            u.AddCondition("底力強化", -1, 1d);
            Assert.IsTrue(p.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void IsSkillAvailable_強化２もレベルが正なら使用可能になる()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力強化２", -1, -1d);
            Assert.IsFalse(p.IsSkillAvailable("底力"));

            u.AddCondition("底力強化２", -1, 2d);
            Assert.IsTrue(p.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void IsSkillAvailable_付加はメインパイロットにだけ効きサブパイロットには効かない()
        {
            // VB: If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then Exit Function
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            AddPilotData(src, "サブ");
            var main = AddPilot(src, "メイン");
            var sub = AddPilot(src, "サブ");
            AddUnitData(src, "複座機");
            var u = CreateUnit(src, "複座機", "味方", main, sub);
            u.AddCondition("底力付加", -1);

            Assert.IsTrue(main.IsSkillAvailable("底力"));
            Assert.IsFalse(sub.IsSkillAvailable("底力"));
        }

        [TestMethod]
        public void HasSupportSkill_指揮付加でも支援系能力ありと判定される()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            Assert.IsFalse(p.HasSupportSkill());

            u.AddCondition("指揮付加", -1);
            Assert.IsTrue(p.HasSupportSkill());
        }

        // ──────────────────────────────────────────────
        // SkillLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillLevel_レベル指定のない所有能力はレベル1()
        {
            var (_, p) = CreatePilotWithSkill("底力");
            Assert.AreEqual(1d, p.SkillLevel("底力"));
            Assert.AreEqual(1d, p.SkillLevel(1));
        }

        [TestMethod]
        public void SkillLevel_未所有能力はレベル0()
        {
            var (_, p) = CreatePilotWithSkill("底力", 2d);
            Assert.AreEqual(0d, p.SkillLevel("集中力"));
        }

        [TestMethod]
        public void SkillLevel_強化レベルが加算され基本値と修正値で参照を切り替えられる()
        {
            var (src, p) = CreatePilotWithSkill("底力", 3d);
            var u = Board(src, p);
            u.AddCondition("底力強化", -1, 2d);

            Assert.AreEqual(5d, p.SkillLevel("底力"));
            // 基本値: 付加・強化を無視
            Assert.AreEqual(3d, p.SkillLevel("底力", "基本値"));
            // 修正値: 付加・強化による修正分のみ
            Assert.AreEqual(2d, p.SkillLevel("底力", "修正値"));
        }

        [TestMethod]
        public void SkillLevel_付加はレベルを置き換え強化２はさらに加算される()
        {
            var (src, p) = CreatePilotWithSkill("底力", 3d);
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, 1d);
            Assert.AreEqual(1d, p.SkillLevel("底力"));

            u.AddCondition("底力強化２", -1, 2d);
            Assert.AreEqual(3d, p.SkillLevel("底力"));
        }

        [TestMethod]
        public void SkillLevel_レベル指定なしの付加はレベル1として扱う()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加２", -1);

            Assert.AreEqual(1d, p.SkillLevel("底力"));
        }

        [TestMethod]
        public void SkillLevel_サブパイロットには強化が効かない()
        {
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            AddPilotData(src, "サブ").AddSkill("底力", 2d, "", 1);
            var main = AddPilot(src, "メイン");
            var sub = AddPilot(src, "サブ");
            AddUnitData(src, "複座機");
            var u = CreateUnit(src, "複座機", "味方", main, sub);
            u.AddCondition("底力強化", -1, 3d);

            Assert.AreEqual(2d, sub.SkillLevel("底力"));
            Assert.AreEqual(3d, main.SkillLevel("底力"));
        }

        // ──────────────────────────────────────────────
        // IsSkillLevelSpecified
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSkillLevelSpecified_所有能力はデータのレベル指定有無で判定()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "", 1);
            pd.AddSkill("集中力", NoLv, "", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.IsTrue(p.IsSkillLevelSpecified("底力"));
            Assert.IsFalse(p.IsSkillLevelSpecified("集中力"));
            Assert.IsTrue(p.IsSkillLevelSpecified(1));
            Assert.IsFalse(p.IsSkillLevelSpecified(2));
            // ユニットに乗っていない未所有能力
            Assert.IsFalse(p.IsSkillLevelSpecified("闘争心"));
        }

        [TestMethod]
        public void IsSkillLevelSpecified_付加はレベル付きの場合のみ強化は常にレベル指定あり()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1);
            u.AddCondition("集中力付加２", -1, 2d);
            u.AddCondition("闘争心強化", -1, 1d);
            u.AddCondition("気合強化２", -1, 1d);

            Assert.IsFalse(p.IsSkillLevelSpecified("底力"));
            Assert.IsTrue(p.IsSkillLevelSpecified("集中力"));
            Assert.IsTrue(p.IsSkillLevelSpecified("闘争心"));
            Assert.IsTrue(p.IsSkillLevelSpecified("気合"));
            Assert.IsFalse(p.IsSkillLevelSpecified("見切り"));
        }

        // ──────────────────────────────────────────────
        // SkillData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillData_所有能力のデータを返し付加データで置き換えられる()
        {
            var (src, p) = CreatePilotWithSkill("チーム", NoLv, "Ａチーム");
            Assert.AreEqual("Ａチーム", p.SkillData("チーム"));
            Assert.AreEqual("Ａチーム", p.SkillData(1));

            var u = Board(src, p);
            u.AddCondition("チーム付加", -1, NoLv, "Ｂチーム");
            Assert.AreEqual("Ｂチーム", p.SkillData("チーム"));
        }

        [TestMethod]
        public void SkillData_強化はデータが空でなければ置き換える()
        {
            // VB: If Len(.ConditionData(sname & "強化")) > 0 Then SkillData = .ConditionData(...)
            var (src, p) = CreatePilotWithSkill("チーム", NoLv, "Ａチーム");
            var u = Board(src, p);
            u.AddCondition("チーム強化", -1, 1d, "");
            Assert.AreEqual("Ａチーム", p.SkillData("チーム"));

            u.AddCondition("チーム強化２", -1, 1d, "Ｃチーム");
            Assert.AreEqual("Ｃチーム", p.SkillData("チーム"));
        }

        [TestMethod]
        public void SkillData_付加２のデータも参照される()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("チーム付加２", -1, NoLv, "Ｄチーム");
            Assert.AreEqual("Ｄチーム", p.SkillData("チーム"));
        }

        // ──────────────────────────────────────────────
        // SkillName（所有能力）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillName_所有能力はレベル付きで表示される()
        {
            var (_, p) = CreatePilotWithSkill("底力", 2d);
            Assert.AreEqual("底力Lv2", p.SkillName("底力"));
            Assert.AreEqual("底力Lv2", p.SkillName(1));
        }

        [TestMethod]
        public void SkillName_別名指定があれば別名にレベルを付ける()
        {
            var (_, p) = CreatePilotWithSkill("底力", 2d, "ガッツ");
            Assert.AreEqual("ガッツLv2", p.SkillName("底力"));
        }

        [TestMethod]
        public void SkillName_別名にLvが含まれていればレベルを付け足さない()
        {
            var (_, p) = CreatePilotWithSkill("底力", 2d, "ガッツLv9");
            Assert.AreEqual("ガッツLv9", p.SkillName("底力"));
        }

        [TestMethod]
        public void SkillName_別名が非表示や解説なら非表示()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "非表示", 1);
            pd.AddSkill("集中力", NoLv, "解説 説明文", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.AreEqual("非表示", p.SkillName("底力"));
            Assert.AreEqual("非表示", p.SkillName("集中力"));
        }

        [TestMethod]
        public void SkillName_能力強化系や追加レベルは非表示で得意技は名称固定()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("格闘ＵＰ", 5d, "", 1);
            pd.AddSkill("射撃ＤＯＷＮ", 5d, "", 1);
            pd.AddSkill("追加レベル", 3d, "", 1);
            pd.AddSkill("得意技", NoLv, "格闘", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.AreEqual("非表示", p.SkillName("格闘ＵＰ"));
            Assert.AreEqual("非表示", p.SkillName("射撃ＤＯＷＮ"));
            Assert.AreEqual("非表示", p.SkillName("追加レベル"));
            Assert.AreEqual("得意技", p.SkillName("得意技"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillName が所有能力の場合に後段（階級等の Lv 除去・括弧除去・強化表示）を飛ばして返す")]
        public void SkillName_所有する階級はレベル表示を除く()
        {
            // VB の SkillName は With sd ブロックの後 ErrorHandler に落ちて後処理を続け、
            // 「階級」「同調率」「霊力」「ＳＰ消費減少」から Lv を取り除く
            var (_, p) = CreatePilotWithSkill("階級", 3d);
            Assert.AreEqual("階級", p.SkillName("階級"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillName が所有能力の場合に後段（階級等の Lv 除去・括弧除去・強化表示）を飛ばして返す")]
        public void SkillName_所有能力でも強化分が表示される()
        {
            var (src, p) = CreatePilotWithSkill("底力", 1d);
            var u = Board(src, p);
            u.AddCondition("底力強化", -1, 2d);
            Assert.AreEqual("底力Lv1+2", p.SkillName("底力"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillName が所有能力の場合に後段（階級等の Lv 除去・括弧除去・強化表示）を飛ばして返す")]
        public void SkillName_所有能力のレベル非表示用括弧は取り除かれる()
        {
            var (_, p) = CreatePilotWithSkill("底力", 2d, "(ガッツ)");
            Assert.AreEqual("ガッツ", p.SkillName("底力"));
        }

        // ──────────────────────────────────────────────
        // SkillName（未所有能力・付加・強化）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillName_未所有でユニットなしなら能力名そのまま()
        {
            var (_, p) = CreatePilotWithSkill();
            Assert.AreEqual("底力", p.SkillName("底力"));
            Assert.AreEqual("非表示", p.SkillName("メッセージ"));
            Assert.AreEqual("非表示", p.SkillName("命中ＵＰ"));
        }

        [TestMethod]
        public void SkillName_防御力成長オプション時は耐久を非表示()
        {
            var (src, p) = CreatePilotWithSkill();
            Assert.AreEqual("耐久", p.SkillName("耐久"));

            DefineOption(src, "防御力成長");
            Assert.AreEqual("非表示", p.SkillName("耐久"));
        }

        [TestMethod]
        public void SkillName_付加は別名とレベルを付けて表示()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, 2d, "ガッツLv5");
            u.AddCondition("集中力付加２", -1, NoLv, "");

            // 付加のレベル指定で別名中の Lv を置き換える
            Assert.AreEqual("ガッツLv2", p.SkillName("底力"));
            // データもレベルもない場合は能力名のまま
            Assert.AreEqual("集中力", p.SkillName("集中力"));
        }

        [TestMethod]
        public void SkillName_付加２のデータが非表示なら非表示()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加２", -1, 2d, "非表示");
            Assert.AreEqual("非表示", p.SkillName("底力"));
        }

        [TestMethod]
        public void SkillName_未所有能力の強化はLv0に強化値を付けて表示()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力強化", -1, 2d);
            u.AddCondition("集中力強化２", -1, -1d, "コンセントレート");

            Assert.AreEqual("底力Lv0+2", p.SkillName("底力"));
            Assert.AreEqual("コンセントレートLv0-1", p.SkillName("集中力"));
        }

        [TestMethod]
        public void SkillName_同調率の強化は強化値を付けずLvも除く()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("同調率強化", -1, 10d);
            Assert.AreEqual("同調率", p.SkillName("同調率"));
        }

        [TestMethod]
        public void SkillName_付加の括弧付き別名は括弧を除いて表示()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, 2d, "(ガッツ)");
            Assert.AreEqual("ガッツ", p.SkillName("底力"));
        }

        [TestMethod]
        public void SkillName_SetSkillで封印された能力はパイロットデータの名称を使う()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("底力", 2d, "ガッツ", 1);
            var p = AddPilot(src, "テストパイロット");
            Assert.IsTrue(p.IsSkillAvailable("底力"));

            // SetSkill テストパイロット 底力 0 相当
            SetSkill(src, p, "底力", "0");
            p.Update();

            Assert.IsFalse(p.IsSkillAvailable("底力"));
            Assert.AreEqual("ガッツLv2", p.SkillName("底力"));
        }

        // ──────────────────────────────────────────────
        // SkillName0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillName0_所有能力はレベル表示なしの名称()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "", 1);
            pd.AddSkill("格闘ＵＰ", 2d, "", 1);
            pd.AddSkill("不得手", NoLv, "射撃", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.AreEqual("底力", p.SkillName0("底力"));
            Assert.AreEqual("底力", p.SkillName0(1));
            Assert.AreEqual("非表示", p.SkillName0("格闘ＵＰ"));
            Assert.AreEqual("不得手", p.SkillName0("不得手"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillName0 が所有能力の別名（データ1項目目）を返さず元の能力名を返す")]
        public void SkillName0_所有能力の別名を返す()
        {
            // VB: If Len(.StrData) > 0 Then SkillName0 = LIndex(.StrData, 1)
            var (_, p) = CreatePilotWithSkill("底力", 2d, "ガッツ");
            Assert.AreEqual("ガッツ", p.SkillName0("底力"));
        }

        [TestMethod]
        public void SkillName0_付加の別名からレベル表示を除く()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, 2d, "ガッツLv2");
            u.AddCondition("集中力付加２", -1, NoLv, "(コンセントレート)");
            u.AddCondition("闘争心付加", -1, NoLv, "非表示");
            Assert.AreEqual("ガッツ", p.SkillName0("底力"));
            Assert.AreEqual("コンセントレート", p.SkillName0("集中力"));
            Assert.AreEqual("非表示", p.SkillName0("闘争心"));
        }

        [TestMethod]
        public void SkillName0_未所有能力の強化は強化データの名称または能力名()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力強化", -1, 1d);
            u.AddCondition("集中力強化２", -1, 1d, "コンセントレート");
            Assert.AreEqual("底力", p.SkillName0("底力"));
            Assert.AreEqual("コンセントレート", p.SkillName0("集中力"));
        }

        [TestMethod]
        public void SkillName0_該当がなければエリアス名を返す()
        {
            var (src, p) = CreatePilotWithSkill();
            src.ALDList.Add("ガッツ").AddAlias("底力");
            Assert.AreEqual("ガッツ", p.SkillName0("底力"));
            Assert.AreEqual("集中力", p.SkillName0("集中力"));
        }

        [TestMethod]
        public void SkillName0_未所有の非表示系と得意技()
        {
            var (src, p) = CreatePilotWithSkill();
            Assert.AreEqual("得意技", p.SkillName0("得意技"));
            Assert.AreEqual("非表示", p.SkillName0("魔力所有"));
            Assert.AreEqual("非表示", p.SkillName0("回避ＤＯＷＮ"));
            Assert.AreEqual("耐久", p.SkillName0("耐久"));
            DefineOption(src, "防御力レベルアップ");
            Assert.AreEqual("非表示", p.SkillName0("耐久"));
        }

        // ──────────────────────────────────────────────
        // SkillNameForNS（必要技能判定用）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillNameForNS_能力強化系とメッセージは能力名そのまま()
        {
            var (_, p) = CreatePilotWithSkill();
            Assert.AreEqual("格闘ＵＰ", p.SkillNameForNS("格闘ＵＰ"));
            Assert.AreEqual("反応ＤＯＷＮ", p.SkillNameForNS("反応ＤＯＷＮ"));
            Assert.AreEqual("メッセージ", p.SkillNameForNS("メッセージ"));
        }

        [TestMethod]
        public void SkillNameForNS_所有能力の別名からレベルと括弧を除く()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "ガッツLv2", 1);
            pd.AddSkill("集中力", NoLv, "(コンセントレート)", 1);
            pd.AddSkill("闘争心", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.AreEqual("ガッツ", p.SkillNameForNS("底力"));
            Assert.AreEqual("コンセントレート", p.SkillNameForNS("集中力"));
            Assert.AreEqual("闘争心", p.SkillNameForNS("闘争心"));
        }

        [TestMethod]
        public void SkillNameForNS_非表示の別名は元の能力名かエリアス名を使う()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "非表示", 1);
            pd.AddSkill("集中力", 2d, "非表示", 1);
            var p = AddPilot(src, "テストパイロット");
            src.ALDList.Add("コンセントレート").AddAlias("集中力");

            Assert.AreEqual("底力", p.SkillNameForNS("底力"));
            Assert.AreEqual("コンセントレート", p.SkillNameForNS("集中力"));
        }

        [TestMethod]
        public void SkillNameForNS_付加と強化の名称を使う()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, 2d, "ガッツLv2");
            u.AddCondition("集中力付加２", -1, NoLv, "");
            u.AddCondition("闘争心強化", -1, 1d, "ファイト");
            u.AddCondition("気合強化２", -1, 1d, "");

            Assert.AreEqual("ガッツ", p.SkillNameForNS("底力"));
            Assert.AreEqual("集中力", p.SkillNameForNS("集中力"));
            Assert.AreEqual("ファイト", p.SkillNameForNS("闘争心"));
            Assert.AreEqual("気合", p.SkillNameForNS("気合"));
        }

        // ──────────────────────────────────────────────
        // SkillType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillType_能力名や別名から能力の種類を返す()
        {
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("底力", 2d, "ガッツ", 1);
            pd.AddSkill("集中力", NoLv, "(コンセントレート)", 1);
            var p = AddPilot(src, "テストパイロット");

            Assert.AreEqual("底力", p.SkillType("底力"));
            Assert.AreEqual("底力", p.SkillType("ガッツLv2"));
            Assert.AreEqual("集中力", p.SkillType("コンセントレート"));
            // 修得していない能力はレベル表示を除いた名称
            Assert.AreEqual("闘争心", p.SkillType("闘争心Lv3"));
        }

        [TestMethod]
        public void SkillType_エリアス定義があればエリアスの種類を返す()
        {
            var (src, p) = CreatePilotWithSkill();
            src.ALDList.Add("ガッツ").AddAlias("底力");
            Assert.AreEqual("底力", p.SkillType("ガッツ"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillType の特殊能力付加による修正で判定が反転している")]
        public void SkillType_付加で与えられた別名から能力の種類を返す()
        {
            // VB: If LIndex(.ConditionData(i), 1) = sname0 Then SkillType = Left$(.Condition(i), Len - 2)
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, NoLv, "ガッツ");
            Assert.AreEqual("底力", p.SkillType("ガッツ"));
        }

        [TestMethod]
        [Ignore("移植の不具合: SkillType の特殊能力付加による修正で判定が反転している")]
        public void SkillType_付加に該当しない未修得能力は名称そのまま()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("底力付加", -1, NoLv, "ガッツ");
            Assert.AreEqual("集中力", p.SkillType("集中力"));
        }
    }
}
