using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// Pilot.UpdateSupportMod（元の SRC の Pilot.UpdateSupportMod）のテスト。
    /// 周囲のユニットによる広域サポート・指揮効果・信頼補正を確認する。
    /// 自機はマップ (5,5) に置き、レベル1・能力値100 のパイロット（命中・回避の基本値は 102）。
    /// </summary>
    [TestClass]
    public class PilotSkillStatus_SupportModTests
    {
        private const double NoLv = Constants.DEFAULT_LEVEL;
        private const int BaseHit = 102;

        private SRC src;
        private int unitSeq;

        [TestInitialize]
        public void Init()
        {
            src = CreateSrc();
            SetupMap(src);
            unitSeq = 0;
        }

        // パイロットを作ってユニットに乗せ、マップに配置する
        private Pilot Deploy(string pname, int x, int y, string party = "味方", params (string name, double lv, string data)[] skills)
        {
            var pd = AddPilotData(src, pname);
            foreach (var s in skills)
            {
                pd.AddSkill(s.name, s.lv, s.data, 1);
            }
            var p = AddPilot(src, pname, 1, party);
            var uname = "機体" + (++unitSeq);
            AddUnitData(src, uname);
            var u = CreateUnit(src, uname, party, p);
            Place(src, u, x, y);
            p.Update();
            return p;
        }

        // ──────────────────────────────────────────────
        // 前提条件
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UpdateSupportMod_マップ未読込時は支援効果を無視し修正値を初期化する()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者", 5, 6, "味方", ("広域サポート", 1d, ""));
            me.UpdateSupportMod();
            Assert.AreEqual(BaseHit + 5, me.Hit);

            // ステータス表示時（マップファイル名なし）
            src.Map.MapFileName = "";
            me.UpdateSupportMod();
            Assert.AreEqual(0, me.HitMod2);
            Assert.AreEqual(BaseHit, me.Hit);
        }

        [TestMethod]
        public void UpdateSupportMod_出撃していないユニットは支援を受けない()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者", 5, 6, "味方", ("広域サポート", 1d, ""));
            me.Unit.Status = "待機";

            me.UpdateSupportMod();

            Assert.AreEqual(BaseHit, me.Hit);
        }

        [TestMethod]
        public void UpdateSupportMod_混乱中のユニットは支援を受けない()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者", 5, 6, "味方", ("広域サポート", 1d, ""));
            me.Unit.AddCondition("混乱", 1);

            me.UpdateSupportMod();

            Assert.AreEqual(BaseHit, me.Hit);
        }

        // ──────────────────────────────────────────────
        // 広域サポート
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UpdateSupportMod_距離2以内の広域サポートで命中回避が5Lv上がる()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者", 6, 6, "味方", ("広域サポート", 2d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(10, me.HitMod2);
            Assert.AreEqual(10, me.DodgeMod2);
            Assert.AreEqual(BaseHit + 10, me.Hit);
            Assert.AreEqual(BaseHit + 10, me.Dodge);
        }

        [TestMethod]
        public void UpdateSupportMod_複数の広域サポートは最大レベルのみ有効()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者Ａ", 5, 6, "味方", ("広域サポート", 1d, ""));
            Deploy("支援者Ｂ", 5, 4, "味方", ("広域サポート", 3d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(15, me.HitMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_距離3以上の広域サポートは無効()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("支援者", 5, 8, "味方", ("広域サポート", 2d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(0, me.HitMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_敵陣営や暴走中のユニットからは支援を受けない()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("敵の支援者", 5, 6, "敵", ("広域サポート", 2d, ""));
            var berserk = Deploy("暴走した支援者", 5, 4, "味方", ("広域サポート", 2d, ""));
            berserk.Unit.AddCondition("暴走", 1);

            me.UpdateSupportMod();

            Assert.AreEqual(0, me.HitMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_ＮＰＣは味方の支援を受けられる()
        {
            var me = Deploy("自分", 5, 5, "ＮＰＣ");
            Deploy("支援者", 5, 6, "味方", ("広域サポート", 1d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(5, me.HitMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_別チームのパイロットからは支援を受けない()
        {
            var me = Deploy("自分", 5, 5, "味方", ("チーム", NoLv, "Ａ隊"));
            Deploy("他チーム", 5, 6, "味方", ("広域サポート", 2d, ""), ("チーム", NoLv, "Ｂ隊"));
            Deploy("無所属", 5, 4, "味方", ("広域サポート", 1d, ""));

            me.UpdateSupportMod();

            // チーム指定のない支援者だけが有効
            Assert.AreEqual(5, me.HitMod2);
        }

        // ──────────────────────────────────────────────
        // 指揮効果
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UpdateSupportMod_ザコは指揮範囲内の指揮官から指揮効果を受ける()
        {
            var me = Deploy("兵士(ザコ)", 5, 5, "敵");
            // 階級Lv1 の指揮範囲は 2
            Deploy("隊長", 5, 7, "敵", ("階級", 1d, ""), ("指揮", 2d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(10, me.HitMod2);
            Assert.AreEqual(10, me.DodgeMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_指揮官が範囲外ならザコの命中回避は変化しない()
        {
            // VB: Dim cmd_level As Double（初期値 0）で、Case 0 では HitMod2 + 5 * cmd_level
            var me = Deploy("兵士(ザコ)", 5, 5, "敵");
            Deploy("隊長", 5, 8, "敵", ("階級", 1d, ""), ("指揮", 2d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(0, me.HitMod2);
            Assert.AreEqual(BaseHit, me.Hit);
        }

        [TestMethod]
        public void UpdateSupportMod_階級持ちはより高い階級の指揮官からのみ指揮効果を受ける()
        {
            var me = Deploy("自分", 5, 5, "味方", ("階級", 2d, ""));
            Deploy("同格", 5, 6, "味方", ("階級", 2d, ""), ("指揮", 3d, ""));
            me.UpdateSupportMod();
            Assert.AreEqual(0, me.HitMod2);

            Deploy("上官", 5, 4, "味方", ("階級", 3d, ""), ("指揮", 1d, ""));
            me.UpdateSupportMod();
            Assert.AreEqual(5, me.HitMod2);
        }

        [TestMethod]
        public void UpdateSupportMod_階級のない固有パイロットは指揮効果を受けない()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("上官", 5, 6, "味方", ("階級", 3d, ""), ("指揮", 2d, ""));

            me.UpdateSupportMod();

            Assert.AreEqual(0, me.HitMod2);
        }

        // ──────────────────────────────────────────────
        // 信頼補正
        // ──────────────────────────────────────────────

        private void SetRelation(string a, string b, int lv)
        {
            var vname = "関係:" + a + ":" + b;
            if (!src.Expression.IsGlobalVariableDefined(vname))
            {
                src.Expression.DefineGlobalVariable(vname);
            }
            src.Expression.SetVariableAsLong(vname, lv);
        }

        [TestMethod]
        public void UpdateSupportMod_信頼補正は隣接ユニットとの関係で気力修正が付く()
        {
            DefineOption(src, "信頼補正");
            var me = Deploy("自分", 5, 5);
            Deploy("相棒", 5, 6);

            SetRelation("自分", "相棒", 1);
            me.UpdateSupportMod();
            Assert.AreEqual(5, me.MoraleMod);

            SetRelation("自分", "相棒", 2);
            me.UpdateSupportMod();
            Assert.AreEqual(8, me.MoraleMod);

            // 3 以上は 2*Lv+4
            SetRelation("自分", "相棒", 3);
            me.UpdateSupportMod();
            Assert.AreEqual(10, me.MoraleMod);
        }

        [TestMethod]
        public void UpdateSupportMod_信頼補正オプションがなければ気力修正なし()
        {
            var me = Deploy("自分", 5, 5);
            Deploy("相棒", 5, 6);
            SetRelation("自分", "相棒", 2);

            me.UpdateSupportMod();

            Assert.AreEqual(0, me.MoraleMod);
        }

        [TestMethod]
        public void UpdateSupportMod_信頼補正は重複オプションがなければ最大値のみ()
        {
            DefineOption(src, "信頼補正");
            var me = Deploy("自分", 5, 5);
            Deploy("相棒Ａ", 5, 6);
            Deploy("相棒Ｂ", 5, 4);
            SetRelation("自分", "相棒Ａ", 1);
            SetRelation("自分", "相棒Ｂ", 1);

            me.UpdateSupportMod();
            Assert.AreEqual(5, me.MoraleMod);

            // 重複時は合計（1+1=2 → +8）
            DefineOption(src, "信頼補正重複");
            me.UpdateSupportMod();
            Assert.AreEqual(8, me.MoraleMod);
        }

        [TestMethod]
        public void UpdateSupportMod_信頼補正の範囲は通常1で範囲拡大オプションなら2()
        {
            DefineOption(src, "信頼補正");
            var me = Deploy("自分", 5, 5);
            Deploy("相棒", 5, 7);
            SetRelation("自分", "相棒", 1);

            me.UpdateSupportMod();
            Assert.AreEqual(0, me.MoraleMod);

            DefineOption(src, "信頼補正範囲拡大");
            me.UpdateSupportMod();
            Assert.AreEqual(5, me.MoraleMod);
        }

        [TestMethod]
        public void UpdateSupportMod_同じユニットのサポートパイロットとの信頼補正()
        {
            DefineOption(src, "信頼補正");
            var me = Deploy("自分", 5, 5);
            AddPilotData(src, "サポート役");
            var sup = AddPilot(src, "サポート役");
            me.Unit.AddSupport(sup);
            sup.Unit = me.Unit;
            SetRelation("自分", "サポート役", 2);

            me.UpdateSupportMod();

            Assert.AreEqual(8, me.MoraleMod);
        }
    }
}
