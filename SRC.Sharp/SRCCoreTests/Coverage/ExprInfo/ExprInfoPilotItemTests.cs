using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数の「パイロット」「パイロットデータ」「非戦闘員」「アイテム」「スペシャルパワー」に関するテスト
    /// 仕様: Info関数.md / VB の EvalInfoFunc（Expression.bas）
    /// </summary>
    [TestClass]
    public class ExprInfoPilotItemTests
    {
        // ──────────────────────────────────────────────
        // パイロット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Pilot_BasicProperties()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src, level: 5);

            Assert.AreEqual("テスト", Eval(src, "Info(パイロット,テストパイロット,愛称)"));
            Assert.AreEqual("てすと", Eval(src, "Info(パイロット,テストパイロット,読み仮名)"));
            Assert.AreEqual("男性", Eval(src, "Info(パイロット,テストパイロット,性別)"));
            Assert.AreEqual("汎用", Eval(src, "Info(パイロット,テストパイロット,ユニットクラス)"));
            Assert.AreEqual("AABA", Eval(src, "Info(パイロット,テストパイロット,地形適応)"));
            Assert.AreEqual("強気", Eval(src, "Info(パイロット,テストパイロット,性格)"));
            Assert.AreEqual("test.mid", Eval(src, "Info(パイロット,テストパイロット,ＭＩＤＩ)"));
            Assert.AreEqual("5", Eval(src, "Info(パイロット,テストパイロット,レベル)"));
            Assert.AreEqual("100", Eval(src, "Info(パイロット,テストパイロット,気力)"));
            // パイロットの武器数はパイロットデータの武器数
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,武器数)"));
        }

        [TestMethod]
        public void Pilot_SpecialPowers_DependOnLevel()
        {
            // レベル5のパイロットはレベル5までに修得するスペシャルパワーのみ持つ
            var src = CreateSrc();
            var u = CreatePilotedUnit(src, level: 5);
            var p = u.MainPilot();

            Assert.AreEqual("加速 必中", Eval(src, "Info(パイロット,テストパイロット,スペシャルパワー)"));
            Assert.AreEqual("加速 必中", Eval(src, "Info(パイロット,テストパイロット,精神)"));
            Assert.AreEqual("1", Eval(src, "Info(パイロット,テストパイロット,スペシャルパワー所有,必中)"));
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,精神コマンド所有,熱血)"));
            // 消費ＳＰ減少系の技能が無いのでデータどおり 25
            Assert.AreEqual("25", Eval(src, "Info(パイロット,テストパイロット,スペシャルパワーコスト,必中)"));
            // 最大ＳＰ = 2 * レベル + ＳＰ基本値 = 10 + 50（Pilot.cls MaxSP）
            Assert.AreEqual("60", Eval(src, "Info(パイロット,テストパイロット,最大ＳＰ)"));

            // ＳＰ は現在値
            p.SP = 7;
            Assert.AreEqual("7", Eval(src, "Info(パイロット,テストパイロット,ＳＰ)"));
        }

        [TestMethod]
        public void Pilot_Skills_DependOnLevel()
        {
            // 援護Lv2 はレベル5で修得する
            var src = CreateSrc();
            var p1 = src.PList.Add("テストパイロット", 1, "味方");
            Assert.AreEqual("1", Eval(src, "Info(パイロット,テストパイロット,特殊能力数)"));
            Assert.AreEqual("底力", Eval(src, "Info(パイロット,テストパイロット,特殊能力,1)"));
            Assert.AreEqual("1", Eval(src, "Info(パイロット,テストパイロット,特殊能力所有,底力)"));
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,特殊能力所有,援護)"));

            p1.Level = 5;
            p1.Update();
            Assert.AreEqual("2", Eval(src, "Info(パイロット,テストパイロット,特殊能力数)"));
            Assert.AreEqual("1", Eval(src, "Info(パイロット,テストパイロット,特殊能力所有,援護)"));
            Assert.AreEqual("2", Eval(src, "Info(パイロット,テストパイロット,特殊能力レベル,援護)"));
        }

        [TestMethod]
        public void Pilot_StatusValues_BaseIncludesLevelGrowth()
        {
            // 基本値 = データの値 + レベルによる上昇分（Pilot.cls Update）
            // 格闘・射撃・技量・反応は Lv×1、命中・回避は Lv×2
            var src = CreateSrc();
            CreatePilotedUnit(src, level: 1);
            var expected = new (string name, string value)[]
            {
                ("格闘", "151"), ("射撃", "141"), ("命中", "132"),
                ("回避", "122"), ("技量", "111"), ("反応", "101"),
            };
            foreach (var (name, value) in expected)
            {
                Assert.AreEqual(value, Eval(src, $"Info(パイロット,テストパイロット,{name}基本値)"), name);
                // 能力を修正する技能もユニットの特殊能力も無いので修正値は 0、合計は基本値と等しい
                Assert.AreEqual("0", Eval(src, $"Info(パイロット,テストパイロット,{name}修正値)"), name);
                Assert.AreEqual("0", Eval(src, $"Info(パイロット,テストパイロット,{name}支援修正値)"), name);
                Assert.AreEqual(value, Eval(src, $"Info(パイロット,テストパイロット,{name})"), name);
            }
        }

        [TestMethod]
        public void Pilot_StatusValues_FollowLevel()
        {
            // レベル10 では格闘 150+10、命中 130+20
            var src = CreateSrc();
            CreatePilotedUnit(src, level: 10);
            Assert.AreEqual("160", Eval(src, "Info(パイロット,テストパイロット,格闘)"));
            Assert.AreEqual("150", Eval(src, "Info(パイロット,テストパイロット,命中)"));
        }

        // ──────────────────────────────────────────────
        // パイロットデータ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotData_ReturnsRawDataValues()
        {
            var src = CreateSrc();
            Assert.AreEqual("テスト", Eval(src, "Info(パイロットデータ,テストパイロット,愛称)"));
            Assert.AreEqual("てすと", Eval(src, "Info(パイロットデータ,テストパイロット,読み仮名)"));
            Assert.AreEqual("AABA", Eval(src, "Info(パイロットデータ,テストパイロット,地形適応)"));
            Assert.AreEqual("100", Eval(src, "Info(パイロットデータ,テストパイロット,経験値)"));
            Assert.AreEqual("140", Eval(src, "Info(パイロットデータ,テストパイロット,射撃)"));
            Assert.AreEqual("130", Eval(src, "Info(パイロットデータ,テストパイロット,命中)"));
            Assert.AreEqual("120", Eval(src, "Info(パイロットデータ,テストパイロット,回避)"));
            Assert.AreEqual("110", Eval(src, "Info(パイロットデータ,テストパイロット,技量)"));
            Assert.AreEqual("100", Eval(src, "Info(パイロットデータ,テストパイロット,反応)"));
            Assert.AreEqual("強気", Eval(src, "Info(パイロットデータ,テストパイロット,性格)"));
            // パイロットデータの最大ＳＰ・ＳＰはデータのＳＰ値
            Assert.AreEqual("50", Eval(src, "Info(パイロットデータ,テストパイロット,最大ＳＰ)"));
            Assert.AreEqual("50", Eval(src, "Info(パイロットデータ,テストパイロット,ＳＰ)"));
            Assert.AreEqual("test_pilot.bmp", Eval(src, "Info(パイロットデータ,テストパイロット,グラフィック)"));
            Assert.AreEqual("test.mid", Eval(src, "Info(パイロットデータ,テストパイロット,ＭＩＤＩ)"));
        }

        [TestMethod]
        public void PilotData_SpecialPowersAndSkills_AtMaxLevel()
        {
            // パイロットデータはレベル100時点で修得しているものを返す
            var src = CreateSrc();
            Assert.AreEqual("加速 必中 熱血", Eval(src, "Info(パイロットデータ,テストパイロット,精神)"));
            Assert.AreEqual("1", Eval(src, "Info(パイロットデータ,テストパイロット,精神コマンド所有,熱血)"));
            Assert.AreEqual("0", Eval(src, "Info(パイロットデータ,テストパイロット,精神コマンド所有,ひらめき)"));
            Assert.AreEqual("25", Eval(src, "Info(パイロットデータ,テストパイロット,精神コマンドコスト,必中)"));
            Assert.AreEqual("2", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力数)"));
            Assert.AreEqual("底力", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力,1)"));
            Assert.AreEqual("援護", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力,2)"));
            Assert.AreEqual("1", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力所有,援護)"));
            Assert.AreEqual("2", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力レベル,援護)"));
            // 番号指定の特殊能力レベルは番号の特殊能力名で引く
            Assert.AreEqual("2", Eval(src, "Info(パイロットデータ,テストパイロット,特殊能力レベル,2)"));
        }

        // ──────────────────────────────────────────────
        // 非戦闘員
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NonPilot_Properties()
        {
            var src = CreateSrc();
            Assert.AreEqual("町長", Eval(src, "Info(非戦闘員,町長,名称)"));
            Assert.AreEqual("マルタ町長", Eval(src, "Info(非戦闘員,町長,愛称)"));
            Assert.AreEqual("mayor.bmp", Eval(src, "Info(非戦闘員,町長,グラフィック)"));
        }

        // ──────────────────────────────────────────────
        // アイテム・アイテムデータ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemData_Properties()
        {
            var src = CreateSrc();
            Assert.AreEqual("てすとそうこう", Eval(src, "Info(アイテムデータ,テスト装甲,読み仮名)"));
            Assert.AreEqual("汎用", Eval(src, "Info(アイテムデータ,テスト装甲,アイテムクラス)"));
            Assert.AreEqual("強化パーツ", Eval(src, "Info(アイテムデータ,テスト装甲,装備個所)"));
            Assert.AreEqual("500", Eval(src, "Info(アイテムデータ,テスト装甲,最大ＨＰ修正値)"));
            Assert.AreEqual("30", Eval(src, "Info(アイテムデータ,テスト装甲,最大ＥＮ修正値)"));
            Assert.AreEqual("100", Eval(src, "Info(アイテムデータ,テスト装甲,装甲修正値)"));
            Assert.AreEqual("5", Eval(src, "Info(アイテムデータ,テスト装甲,運動性修正値)"));
            Assert.AreEqual("1", Eval(src, "Info(アイテムデータ,テスト装甲,移動力修正値)"));
            Assert.AreEqual("1", Eval(src, "Info(アイテムデータ,テスト装甲,特殊能力数)"));
            Assert.AreEqual("ＨＰ回復", Eval(src, "Info(アイテムデータ,テスト装甲,特殊能力,1)"));
            Assert.AreEqual("1", Eval(src, "Info(アイテムデータ,テスト装甲,武器数)"));
            Assert.AreEqual("アイテム弾", Eval(src, "Info(アイテムデータ,テスト装甲,武器,1)"));
            Assert.AreEqual("3", Eval(src, "Info(アイテムデータ,テスト装甲,武器,アイテム弾,射程)"));
            Assert.AreEqual("3", Eval(src, "Info(アイテムデータ,テスト装甲,武器,1,最大弾数)"));
            // 解説文の改行は空白に置き換えられる
            Assert.AreEqual("説明文の1行目 説明文の2行目", Eval(src, "Info(アイテムデータ,テスト装甲,解説文)"));
        }

        [TestMethod]
        public void Item_ByNameFallsBackToItemData_AndByIdUsesInstance()
        {
            // 「アイテム」指定で IList に無い名前はアイテムデータとして扱う
            var src = CreateSrc();
            Assert.AreEqual("強化パーツ", Eval(src, "Info(アイテム,テスト装甲,装備個所)"));

            // 実体のアイテムは ID で参照できる
            var it = src.IList.Add("テスト装甲");
            Assert.AreEqual("テスト装甲", Eval(src, $"Info(アイテム,\"{it.ID}\",名称)"));
            Assert.AreEqual("500", Eval(src, $"Info(アイテム,\"{it.ID}\",最大ＨＰ修正値)"));
            Assert.AreEqual("1", Eval(src, $"Info(アイテム,\"{it.ID}\",移動力修正値)"));
            Assert.AreEqual("汎用", Eval(src, $"Info(アイテム,\"{it.ID}\",アイテムクラス)"));
            Assert.AreEqual("説明文の1行目 説明文の2行目", Eval(src, $"Info(アイテム,\"{it.ID}\",コメント)"));
        }

        // ──────────────────────────────────────────────
        // スペシャルパワー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPower_Properties()
        {
            var src = CreateSrc();
            Assert.AreEqual("必中", Eval(src, "Info(スペシャルパワー,必中,名称)"));
            Assert.AreEqual("ひっちゅう", Eval(src, "Info(スペシャルパワー,必中,読み仮名)"));
            Assert.AreEqual("必", Eval(src, "Info(スペシャルパワー,必中,短縮名)"));
            Assert.AreEqual("25", Eval(src, "Info(スペシャルパワー,必中,消費ＳＰ)"));
            Assert.AreEqual("自分", Eval(src, "Info(スペシャルパワー,必中,対象)"));
            Assert.AreEqual("ターン", Eval(src, "Info(スペシャルパワー,必中,持続期間)"));
            Assert.AreEqual("攻撃", Eval(src, "Info(スペシャルパワー,熱血,持続期間)"));
            Assert.AreEqual("必中", Eval(src, "Info(スペシャルパワー,必中,アニメ)"));
            Assert.AreEqual("１ターンの間、命中率が100%になる", Eval(src, "Info(スペシャルパワー,必中,解説文)"));
        }

        [TestMethod]
        public void SpecialPower_Effects()
        {
            var src = CreateSrc();
            Assert.AreEqual("2", Eval(src, "Info(スペシャルパワー,加速,効果数)"));
            Assert.AreEqual("移動力強化", Eval(src, "Info(スペシャルパワー,加速,効果タイプ,1)"));
            Assert.AreEqual("2", Eval(src, "Info(スペシャルパワー,加速,効果レベル,1)"));
            Assert.AreEqual("気力増加", Eval(src, "Info(スペシャルパワー,加速,効果タイプ,2)"));
            Assert.AreEqual("1.5", Eval(src, "Info(スペシャルパワー,加速,効果レベル,2)"));
            // 範囲外の番号は空文字列
            Assert.AreEqual("", Eval(src, "Info(スペシャルパワー,加速,効果タイプ,3)"));
            Assert.AreEqual("", Eval(src, "Info(スペシャルパワー,加速,効果レベル,0)"));
            Assert.AreEqual("", Eval(src, "Info(スペシャルパワー,加速,効果データ,3)"));
        }
    }
}
