using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.Pilots.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.Pilots
{
    /// <summary>
    /// PilotSkillStatus のレビューで指摘された未検証の振る舞いのテスト。
    /// Pilot.Update の追加サポート・早期終了・気力の補正、
    /// Personality の追加パイロット、UpdateSupportMod の指揮範囲とＮＰＣの陣営判定を確認する。
    /// </summary>
    [TestClass]
    public class PilotSkillStatus_ReviewTests
    {
        private const int BaseHit = 102;

        // ──────────────────────────────────────────────
        // Update: 特殊能力一覧
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_SetSkillのハンターとＳＰ消費減少はデータが異なれば別の能力として追加される()
        {
            // VB Pilot.Update: SetSkill 分は .StrData = sdata で同一判定する（データ違いは別能力）
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("ハンター", 1d, "ハンター 竜", 1);
            pd.AddSkill("ＳＰ消費減少", 1d, "ＳＰ消費減少 集中", 1);
            var p = AddPilot(src, "テストパイロット");
            Assert.AreEqual(2, p.CountSkill());

            SetSkill(src, p, "ハンター", "1 ハンター 魔物");
            SetSkill(src, p, "ＳＰ消費減少", "1 ＳＰ消費減少 熱血");
            p.Update();

            Assert.AreEqual(4, p.CountSkill());
        }

        [TestMethod]
        public void Update_SetSkillでデータまで同一のハンターは置き換えられる()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("ハンター", 1d, "ハンター 竜", 1);
            var p = AddPilot(src, "テストパイロット");

            // データ同一・レベル0 なら封印される
            SetSkill(src, p, "ハンター", "0 ハンター 竜");
            p.Update();

            Assert.AreEqual(0, p.CountSkill());
            Assert.IsFalse(p.IsSkillAvailable("ハンター"));
        }

        // ──────────────────────────────────────────────
        // Update: ユニットによる修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_追加サポートのサポート能力がメインに加算される()
        {
            // VB Pilot.Update: If .IsFeatureAvailable("追加サポート") Then With .AdditionalSupport ...
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            var ad = AddPilotData(src, "追加サポート役");
            ad.AddSkill("格闘サポート", 1d, "", 1);
            ad.AddSkill("サポート", 2d, "", 1);
            var main = AddPilot(src, "メイン");
            AddUnitData(src, "テスト機体", "追加サポート=追加サポート役");
            var u = CreateUnit(src, "テスト機体", "味方", main);

            // 出撃していなければ加算されない
            main.Update();
            Assert.AreEqual(0, main.InfightMod);

            u.Status = "出撃";
            main.Update();

            Assert.AreEqual(2, main.InfightMod);
            Assert.AreEqual(6, main.HitMod);
            Assert.AreEqual(6, main.DodgeMod);
            Assert.AreEqual(101 + 2, main.Infight);
            Assert.AreEqual(BaseHit + 6, main.Hit);
        }

        [TestMethod]
        public void Update_パイロットのいないユニットに乗っている場合は能力値を再計算しない()
        {
            // VB Pilot.Update: クイックセーブ処理などで実際には乗っていない場合は Exit Sub
            // （基本値は更新されるが、最後の能力値算出は行われない）
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット", 1);
            Assert.AreEqual(101, p.Infight);

            AddUnitData(src, "空の機体");
            var empty = src.UList.Add("空の機体", 0, "味方");
            p.Unit = empty;
            p.Level = 10;

            Assert.AreEqual(110, p.InfightBase);
            Assert.AreEqual(101, p.Infight);
            Assert.AreEqual(BaseHit, p.Hit);
        }

        [TestMethod]
        [Ignore("移植の不具合: Update の最後の SetMorale(Morale)（気力を気力上限・下限に収める処理）が未移植（コメントアウト）")]
        public void Update_気力上限が下がると気力が上限に収められる()
        {
            // VB Pilot.cls: ユニット修正の最後に SetMorale Morale を呼び、気力を上限・下限の範囲にする
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット");
            AddUnitData(src, "テスト機体");
            CreateUnit(src, "テスト機体", "味方", p);
            p.Morale = 150;
            Assert.AreEqual(150, p.Morale);

            SetSkill(src, p, "気力上限", "120");
            p.Update();

            Assert.AreEqual(120, p.Morale);
        }

        // ──────────────────────────────────────────────
        // Personality
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Personality_追加パイロットがいれば1人目のパイロットは追加パイロットの性格になる()
        {
            // VB get_Personality: .Pilot(1) Is Me なら .MainPilot.Data.Personality
            var src = CreateSrc();
            AddPilotData(src, "本来のパイロット").Personality = "普通";
            AddPilotData(src, "相棒").Personality = "弱気";
            AddPilotData(src, "追加パイロット").Personality = "超強気";
            var first = AddPilot(src, "本来のパイロット");
            var second = AddPilot(src, "相棒");
            AddUnitData(src, "テスト機体", "追加パイロット=追加パイロット");
            var u = CreateUnit(src, "テスト機体", "味方", first, second);

            var main = u.MainPilot();
            Assert.AreEqual("追加パイロット", main.Name);
            Assert.AreEqual("超強気", first.Personality);
            // 2人目のパイロットは自分の性格のまま
            Assert.AreEqual("弱気", second.Personality);
        }

        // ──────────────────────────────────────────────
        // UpdateSupportMod
        // ──────────────────────────────────────────────

        private int unitSeq;

        private Pilot Deploy(SRC src, string pname, int x, int y, string party, params (string name, double lv)[] skills)
        {
            var pd = AddPilotData(src, pname);
            foreach (var s in skills)
            {
                pd.AddSkill(s.name, s.lv, "", 1);
            }
            var p = AddPilot(src, pname, 1, party);
            var uname = "機体" + (++unitSeq);
            AddUnitData(src, uname);
            var u = CreateUnit(src, uname, party, p);
            Place(src, u, x, y);
            p.Update();
            return p;
        }

        [TestMethod]
        public void UpdateSupportMod_指揮範囲は指揮官の階級で決まり範囲外の指揮官は無視される()
        {
            // VB UpdateSupportMod: If range > .CommandRange Then GoTo NextUnit
            // 階級Lv0-6 は範囲2、Lv7-9 は範囲3
            var src = CreateSrc();
            SetupMap(src);
            var me = Deploy(src, "自分", 5, 5, "味方", ("階級", 1d));
            Deploy(src, "上官", 5, 8, "味方", ("階級", 3d), ("指揮", 2d));

            me.UpdateSupportMod();
            Assert.AreEqual(0, me.HitMod2);
            Assert.AreEqual(BaseHit, me.Hit);

            Deploy(src, "将軍", 2, 5, "味方", ("階級", 7d), ("指揮", 1d));
            me.UpdateSupportMod();
            Assert.AreEqual(5, me.HitMod2);
            Assert.AreEqual(5, me.DodgeMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_ＮＰＣは敵や中立の支援を受けない()
        {
            // VB UpdateSupportMod: 味方・ＮＰＣ は 敵・中立 のユニットから支援を受けない
            var src = CreateSrc();
            SetupMap(src);
            var me = Deploy(src, "自分", 5, 5, "ＮＰＣ");
            Deploy(src, "敵の支援者", 5, 6, "敵", ("広域サポート", 3d));
            Deploy(src, "中立の支援者", 5, 4, "中立", ("広域サポート", 3d));

            me.UpdateSupportMod();
            Assert.AreEqual(0, me.HitMod2);

            Deploy(src, "味方の支援者", 6, 5, "味方", ("広域サポート", 1d));
            me.UpdateSupportMod();
            Assert.AreEqual(5, me.HitMod2);
        }
    }
}
