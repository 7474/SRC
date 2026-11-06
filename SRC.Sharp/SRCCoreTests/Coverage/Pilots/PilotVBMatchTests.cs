using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// 元の SRC（Pilot.cls の CountSkill 以降の手続き）の動作を期待値にしたテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class PilotVBMatchTests
    {
        private const double NoLv = Constants.DEFAULT_LEVEL;

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

        private static Unit Board(SRC src, Pilot p)
        {
            AddUnitData(src, "テスト機体");
            return CreateUnit(src, "テスト機体", "味方", p);
        }

        // ──────────────────────────────────────────────
        // 番号（数値の Index）で参照した重複可能な能力は付加の影響を受けない
        // VB: Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動": If IsNumeric(Index) Then Exit Function
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillLevel_番号で参照したハンターは付加のレベルに置き換えられない()
        {
            var (src, p) = CreatePilotWithSkill("ハンター", 1d, "ハンター 竜");
            var u = Board(src, p);
            u.AddCondition("ハンター付加", -1, 3d, "ハンター 魔物");

            Assert.AreEqual(1d, p.SkillLevel(1));
            // 名称で参照した場合は付加のレベルになる
            Assert.AreEqual(3d, p.SkillLevel("ハンター"));
        }

        [TestMethod]
        public void SkillData_番号で参照したハンターは付加のデータに置き換えられない()
        {
            var (src, p) = CreatePilotWithSkill("ハンター", 1d, "ハンター 竜");
            var u = Board(src, p);
            u.AddCondition("ハンター付加", -1, 3d, "ハンター 魔物");

            Assert.AreEqual("ハンター 竜", p.SkillData(1));
            Assert.AreEqual("ハンター 魔物", p.SkillData("ハンター"));
        }

        [TestMethod]
        public void SkillName_番号で参照したＳＰ消費減少は付加の名称に置き換えられない()
        {
            // VB: Case "ＳＰ消費減少": If IsNumeric(Index) Then（括弧とレベル指定を除いて）Exit Function
            var (src, p) = CreatePilotWithSkill("ＳＰ消費減少", 2d, "");
            var u = Board(src, p);
            u.AddCondition("ＳＰ消費減少付加", -1, 3d, "SP節約");

            Assert.AreEqual("ＳＰ消費減少", p.SkillName(1));
            Assert.AreEqual("SP節約", p.SkillName("ＳＰ消費減少"));
        }

        // ──────────────────────────────────────────────
        // 見つからない番号・数値の文字列
        // VB: If sname = "" Then If IsNumeric(Index) Then Exit Function
        // ──────────────────────────────────────────────

        [TestMethod]
        public void 見つからない番号は既定値を返す()
        {
            var (_, p) = CreatePilotWithSkill();
            // VB: 範囲外の番号は On Error で ErrorHandler に飛び、IsNumeric(Index) なので既定値を返す
            Assert.AreEqual("", p.SkillData(1));
            Assert.AreEqual("", p.SkillData("底力"));
            Assert.AreEqual(0d, p.SkillLevel(1));
            Assert.IsFalse(p.IsSkillLevelSpecified(1));
            Assert.AreEqual("", p.SkillName(1));
            Assert.AreEqual("", p.SkillName0(1));
        }

        [TestMethod]
        public void SkillName_見つからない数値の文字列は空文字列を返す()
        {
            var (_, p) = CreatePilotWithSkill();
            Assert.AreEqual("", p.SkillName("1"));
            Assert.AreEqual("", p.SkillName0("1"));
        }

        [TestMethod]
        public void SkillLevel_見つからない数値の文字列は付加を見ない()
        {
            var (src, p) = CreatePilotWithSkill();
            var u = Board(src, p);
            u.AddCondition("1付加", -1, 3d);
            u.AddCondition("1強化", -1, 2d);

            Assert.AreEqual(0d, p.SkillLevel("1"));
            Assert.IsFalse(p.IsSkillLevelSpecified("1"));
        }

        // ──────────────────────────────────────────────
        // SkillType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillType_エリアスは最初の能力の種類を返す()
        {
            // VB: SkillType = ALDList.Item(sname0).AliasType(1)
            var (src, p) = CreatePilotWithSkill();
            var ad = src.ALDList.Add("超ガッツ");
            ad.AddAlias("底力");
            ad.AddAlias("気力上限Lv160=超ガッツ");

            Assert.AreEqual("底力", p.SkillType("超ガッツ"));
        }

        [TestMethod]
        public void SkillType_空文字列は空文字列を返す()
        {
            var (_, p) = CreatePilotWithSkill();
            Assert.AreEqual("", p.SkillType(""));
        }

        // ──────────────────────────────────────────────
        // Ride
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Ride_搭乗先の性別の指定で専用のクラスに通常のパイロットとして乗る()
        {
            // VB の Ride は IsSupport を呼ぶ前に Unit を設定するので、
            // IsSupport の「(性別専用)」の判定には搭乗先のユニットの「性別」が使われる
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.Sex = "男性";
            pd.Class = "MS(サポート) MS";
            var p = AddPilot(src, "テストパイロット");

            var ud = AddUnitData(src, "テスト機体", "性別=女性");
            ud.Class = "MS(女性専用)";
            var u = src.UList.Add("テスト機体", 0, "味方");

            p.Ride(u);

            Assert.AreSame(u, p.Unit);
            Assert.AreEqual(1, u.CountPilot());
            Assert.AreEqual(0, u.CountSupport());
        }

        // ──────────────────────────────────────────────
        // Restore・RestoreParameter
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Restore_追加パイロットの印は保存されない()
        {
            // VB の Dump は IsAdditionalPilot を書かないので、読み込んだ直後は False
            var (src, p) = CreatePilotWithSkill();
            p.IsAdditionalPilot = true;

            p.Restore(src);

            Assert.IsFalse(p.IsAdditionalPilot);
        }

        [TestMethod]
        public void RestoreParameter_ＳＰは最大値で切り詰められる()
        {
            // VB: SP = ibuf（Property Let SP で MaxSP に切り詰める）
            var (_, p) = CreatePilotWithSkill();
            p.SP0 = p.MaxSP + 50;

            p.RestoreParameter();

            Assert.AreEqual(p.MaxSP, p.SP0);
        }

        [TestMethod]
        public void RestoreParameter_霊力を持たないパイロットの霊力は0になる()
        {
            // VB の Dump は Plana プロパティ（霊力を持たなければ 0）を書き、RestoreParameter でそれを設定する
            var (_, p) = CreatePilotWithSkill();
            p.Plana0 = 30;

            p.RestoreParameter();

            Assert.AreEqual(0, p.Plana0);
        }
    }
}
