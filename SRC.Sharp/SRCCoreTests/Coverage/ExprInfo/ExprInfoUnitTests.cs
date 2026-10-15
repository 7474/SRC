using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数の「ユニット」「ユニットデータ」に関するテスト
    /// 仕様: Info関数.md / VB の EvalInfoFunc（Expression.bas）
    /// </summary>
    [TestClass]
    public class ExprInfoUnitTests
    {
        private static string U(Units.Unit u, string rest)
        {
            return $"Info(ユニット,\"{u.ID}\",{rest})";
        }

        [TestMethod]
        public void Unit_BasicProperties_ComeFromLoadedUnitData()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);

            Assert.AreEqual("テスト機", Eval(src, U(u, "名称")));
            Assert.AreEqual("てすとき", Eval(src, U(u, "読み仮名")));
            Assert.AreEqual("汎用", Eval(src, U(u, "ユニットクラス")));
            Assert.AreEqual("汎用", Eval(src, U(u, "機体クラス")));
            Assert.AreEqual("陸", Eval(src, U(u, "移動可能地形")));
            Assert.AreEqual("4", Eval(src, U(u, "移動力")));
            Assert.AreEqual("M", Eval(src, U(u, "サイズ")));
            Assert.AreEqual("1200", Eval(src, U(u, "修理費")));
            Assert.AreEqual("150", Eval(src, U(u, "経験値")));
            Assert.AreEqual("1000", Eval(src, U(u, "最大ＨＰ")));
            Assert.AreEqual("100", Eval(src, U(u, "最大ＥＮ")));
            Assert.AreEqual("600", Eval(src, U(u, "装甲")));
            Assert.AreEqual("60", Eval(src, U(u, "運動性")));
            Assert.AreEqual("test_unit.bmp", Eval(src, U(u, "グラフィック")));
        }

        [TestMethod]
        public void Unit_Adaption_ConvertsLevelsToLetters()
        {
            // ユニットの地形適応は Adaption(1..4) の値を S/A/B/C/D/E の文字に変換して並べる
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("AABC", Eval(src, U(u, "地形適応")));
        }

        [TestMethod]
        public void Unit_CurrentHpAndEn_ReflectDamage()
        {
            // ＨＰ・ＥＮ は現在値、最大ＨＰ・最大ＥＮ は最大値を返す
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            u.HP = 400;
            u.EN = 30;
            Assert.AreEqual("400", Eval(src, U(u, "ＨＰ")));
            Assert.AreEqual("30", Eval(src, U(u, "ＥＮ")));
            Assert.AreEqual("1000", Eval(src, U(u, "最大ＨＰ")));
            Assert.AreEqual("100", Eval(src, U(u, "最大ＥＮ")));
        }

        [TestMethod]
        public void Unit_Item_ModifiesStatusAndIsListed()
        {
            // アイテムを装備するとその修正値が最大ＨＰ等に反映され、アイテム／アイテムＩＤで参照できる
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            var it = src.IList.Add("テスト装甲");
            u.AddItem(it);
            u.Update();

            Assert.AreEqual("1500", Eval(src, U(u, "最大ＨＰ")));
            Assert.AreEqual("130", Eval(src, U(u, "最大ＥＮ")));
            Assert.AreEqual("700", Eval(src, U(u, "装甲")));
            Assert.AreEqual("65", Eval(src, U(u, "運動性")));
            Assert.AreEqual("5", Eval(src, U(u, "移動力")));
            Assert.AreEqual("1", Eval(src, U(u, "アイテム数")));
            Assert.AreEqual("2", Eval(src, U(u, "最大アイテム数")));
            Assert.AreEqual("テスト装甲", Eval(src, U(u, "アイテム,1")));
            Assert.AreEqual(it.ID, Eval(src, U(u, "アイテムＩＤ,1")));
            // 範囲外の番号は空文字列
            Assert.AreEqual("", Eval(src, U(u, "アイテム,2")));
            Assert.AreEqual("", Eval(src, U(u, "アイテムＩＤ,0")));
            // アイテムの武器も武器数に含まれる
            Assert.AreEqual("4", Eval(src, U(u, "武器数")));
        }

        [TestMethod]
        public void Unit_PilotCounts()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("1", Eval(src, U(u, "規定パイロット数")));
            Assert.AreEqual("1", Eval(src, U(u, "パイロット数")));
            Assert.AreEqual("0", Eval(src, U(u, "サポート数")));
        }

        [TestMethod]
        public void Unit_Weapon_ByIndexAndByName()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);

            Assert.AreEqual("3", Eval(src, U(u, "武器数")));
            // 番号指定・名称指定のどちらでも武器を特定できる。属性省略時は名称
            Assert.AreEqual("テストビーム", Eval(src, U(u, "武器,1")));
            Assert.AreEqual("テストパンチ", Eval(src, U(u, "武器,テストパンチ,名称")));

            Assert.AreEqual("1800", Eval(src, U(u, "武器,1,攻撃力")));
            Assert.AreEqual("4", Eval(src, U(u, "武器,1,射程")));
            Assert.AreEqual("4", Eval(src, U(u, "武器,1,最大射程")));
            Assert.AreEqual("2", Eval(src, U(u, "武器,1,最小射程")));
            Assert.AreEqual("20", Eval(src, U(u, "武器,1,命中率")));
            Assert.AreEqual("10", Eval(src, U(u, "武器,1,消費ＥＮ")));
            Assert.AreEqual("110", Eval(src, U(u, "武器,1,必要気力")));
            Assert.AreEqual("AAAA", Eval(src, U(u, "武器,1,地形適応")));
            Assert.AreEqual("10", Eval(src, U(u, "武器,1,クリティカル率")));
            Assert.AreEqual("Ｂ", Eval(src, U(u, "武器,1,属性")));
            Assert.AreEqual("1", Eval(src, U(u, "武器,1,属性所有,Ｂ")));
            Assert.AreEqual("0", Eval(src, U(u, "武器,1,属性所有,実")));
            Assert.AreEqual("1", Eval(src, U(u, "武器,1,修得")));

            // 弾数のある武器
            Assert.AreEqual("5", Eval(src, U(u, "武器,テストパンチ,最大弾数")));
            Assert.AreEqual("5", Eval(src, U(u, "武器,テストパンチ,弾数")));
        }

        [TestMethod]
        public void Unit_Weapon_NotOwned_ReturnsEmpty()
        {
            // 指定した武器を持っていない場合は空文字列
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("", Eval(src, U(u, "武器,9,名称")));
            Assert.AreEqual("", Eval(src, U(u, "武器,存在しない武器,攻撃力")));
        }

        [TestMethod]
        public void Unit_MaxPowerAndRange_ExcludeCombinationWeapons()
        {
            // 最大攻撃力・最長射程は合体技（属性「合」）を除いた武器で求める
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("1800", Eval(src, U(u, "最大攻撃力")));
            Assert.AreEqual("4", Eval(src, U(u, "最長射程")));
            Assert.AreEqual("1800", Eval(src, "Info(ユニットデータ,テスト機,最大攻撃力)"));
            Assert.AreEqual("4", Eval(src, "Info(ユニットデータ,テスト機,最長射程)"));
        }

        [TestMethod]
        public void Unit_Ability_Properties()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);

            Assert.AreEqual("1", Eval(src, U(u, "アビリティ数")));
            Assert.AreEqual("テスト修理", Eval(src, U(u, "アビリティ,1")));
            Assert.AreEqual("テスト修理", Eval(src, U(u, "アビリティ,テスト修理,名称")));
            Assert.AreEqual("1", Eval(src, U(u, "アビリティ,1,効果数")));
            Assert.AreEqual("回復", Eval(src, U(u, "アビリティ,1,効果タイプ,1")));
            Assert.AreEqual("2", Eval(src, U(u, "アビリティ,1,効果レベル,1")));
            Assert.AreEqual("3", Eval(src, U(u, "アビリティ,1,射程")));
            Assert.AreEqual("2", Eval(src, U(u, "アビリティ,1,最大使用回数")));
            // 使用回数は現在の残り回数
            u.FullRecover();
            Assert.AreEqual("2", Eval(src, U(u, "アビリティ,1,使用回数")));
            u.Abilities[0].SetStock(1);
            Assert.AreEqual("1", Eval(src, U(u, "アビリティ,1,使用回数")));
            Assert.AreEqual("10", Eval(src, U(u, "アビリティ,1,消費ＥＮ")));
            // 存在しないアビリティは空文字列
            Assert.AreEqual("", Eval(src, U(u, "アビリティ,2,名称")));
            Assert.AreEqual("", Eval(src, U(u, "アビリティ,存在しない,名称")));
        }

        [TestMethod]
        public void Unit_Features()
        {
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);

            Assert.AreEqual("2", Eval(src, U(u, "特殊能力数")));
            Assert.AreEqual("ＨＰ回復", Eval(src, U(u, "特殊能力,1")));
            Assert.AreEqual("バリア", Eval(src, U(u, "特殊能力,2")));
            Assert.AreEqual("1", Eval(src, U(u, "特殊能力所有,バリア")));
            Assert.AreEqual("0", Eval(src, U(u, "特殊能力所有,ＥＮ回復")));
            Assert.AreEqual("2", Eval(src, U(u, "特殊能力レベル,ＨＰ回復")));
            Assert.AreEqual("バリア 1000 10", Eval(src, U(u, "特殊能力データ,バリア")));
        }

        [TestMethod]
        public void Unit_RankAndRemodelCost_Default10Steps()
        {
            // 改造費は通常の１０段改造の表に従う（Intermission.bas RankUpCost）
            // 最大改造数は既定で 10（Intermission.bas MaxRank）
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("0", Eval(src, U(u, "ランク")));
            Assert.AreEqual("10000", Eval(src, U(u, "改造費")));
            Assert.AreEqual("10", Eval(src, U(u, "最大改造数")));

            var u5 = src.UList.Add("テスト機", 5, "味方");
            Assert.AreEqual("5", Eval(src, U(u5, "ランク")));
            Assert.AreEqual("150000", Eval(src, U(u5, "改造費")));

            // 最大改造数に達していればそれ以上改造できない（999999999）
            var u10 = src.UList.Add("テスト機", 10, "味方");
            Assert.AreEqual("999999999", Eval(src, U(u10, "改造費")));
        }

        [TestMethod]
        public void Unit_RankAndRemodelCost_Options()
        {
            // Option(低改造費) ではランク5→6 の改造費は 50000
            var src = CreateSrc();
            var u5 = src.UList.Add("テスト機", 5, "味方");
            src.Expression.DefineGlobalVariable("Option(低改造費)");
            Assert.AreEqual("50000", Eval(src, U(u5, "改造費")));

            // Option(５段階改造) では最大改造数 5 で、ランク5 はもう改造できない
            src.Expression.DefineGlobalVariable("Option(５段階改造)");
            Assert.AreEqual("5", Eval(src, U(u5, "最大改造数")));
            Assert.AreEqual("999999999", Eval(src, U(u5, "改造費")));
        }

        [TestMethod]
        public void Unit_RemainingSupportCounts_SubtractUsed()
        {
            // 援護Lv2（レベル5で修得）ならサポートアタック・サポートガードとも2回
            // 残り回数 = 最大数 - 使用済み数（Expression.bas 残りサポートアタック数／残りサポートガード数）
            var src = CreateSrc();
            var u = CreatePilotedUnit(src, level: 5);
            Assert.AreEqual("2", Eval(src, U(u, "残りサポートアタック数")));
            Assert.AreEqual("2", Eval(src, U(u, "残りサポートガード数")));

            u.UsedSupportAttack = 1;
            Assert.AreEqual("1", Eval(src, U(u, "残りサポートアタック数")));
            Assert.AreEqual("2", Eval(src, U(u, "残りサポートガード数")));

            u.UsedSupportGuard = 2;
            Assert.AreEqual("0", Eval(src, U(u, "残りサポートガード数")));
        }

        [TestMethod]
        public void Unit_RemainingSupportCounts_NoSkill_IsZero()
        {
            // 援護を持たないレベル1のパイロットでは 0
            var src = CreateSrc();
            var u = CreatePilotedUnit(src, level: 1);
            Assert.AreEqual("0", Eval(src, U(u, "残りサポートアタック数")));
            Assert.AreEqual("0", Eval(src, U(u, "残りサポートガード数")));
        }

        [TestMethod]
        public void UnitData_ReturnsRawDataValues()
        {
            // ユニットデータはユニット生成前でもデータの値を返す
            var src = CreateSrc();
            Assert.AreEqual("テスト機", Eval(src, "Info(ユニットデータ,テスト機,名称)"));
            Assert.AreEqual("AABC", Eval(src, "Info(ユニットデータ,テスト機,地形適応)"));
            Assert.AreEqual("1000", Eval(src, "Info(ユニットデータ,テスト機,ＨＰ)"));
            Assert.AreEqual("100", Eval(src, "Info(ユニットデータ,テスト機,ＥＮ)"));
            Assert.AreEqual("600", Eval(src, "Info(ユニットデータ,テスト機,装甲)"));
            Assert.AreEqual("60", Eval(src, "Info(ユニットデータ,テスト機,運動性)"));
            Assert.AreEqual("2", Eval(src, "Info(ユニットデータ,テスト機,アイテム数)"));
            Assert.AreEqual("2", Eval(src, "Info(ユニットデータ,テスト機,特殊能力数)"));
            Assert.AreEqual("3", Eval(src, "Info(ユニットデータ,テスト機,武器数)"));
            Assert.AreEqual("1", Eval(src, "Info(ユニットデータ,テスト機,アビリティ数)"));
            Assert.AreEqual("test_unit.bmp", Eval(src, "Info(ユニットデータ,テスト機,グラフィック)"));
        }

        [TestMethod]
        public void UnitData_WeaponData()
        {
            var src = CreateSrc();
            Assert.AreEqual("テストパンチ", Eval(src, "Info(ユニットデータ,テスト機,武器,2)"));
            Assert.AreEqual("1500", Eval(src, "Info(ユニットデータ,テスト機,武器,テストパンチ,攻撃力)"));
            Assert.AreEqual("5", Eval(src, "Info(ユニットデータ,テスト機,武器,2,最大弾数)"));
            Assert.AreEqual("突", Eval(src, "Info(ユニットデータ,テスト機,武器,2,属性)"));
            Assert.AreEqual("110", Eval(src, "Info(ユニットデータ,テスト機,武器,1,必要気力)"));
            Assert.AreEqual("", Eval(src, "Info(ユニットデータ,テスト機,武器,存在しない,攻撃力)"));
        }

        [TestMethod]
        public void UnitData_AbilityData()
        {
            var src = CreateSrc();
            Assert.AreEqual("テスト修理", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,名称)"));
            Assert.AreEqual("回復", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,効果タイプ,1)"));
            Assert.AreEqual("3", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,最大射程)"));
            Assert.AreEqual("10", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,消費ＥＮ)"));
        }

        [TestMethod]
        public void NoCategory_SearchesAllLists()
        {
            // 種類を省略した場合は名前で全てのリストを検索し、idx=2 から属性を読む
            var src = CreateSrc();
            Assert.AreEqual("テスト機", Eval(src, "Info(テスト機,名称)"));
            Assert.AreEqual("1200", Eval(src, "Info(テスト機,修理費)"));
            Assert.AreEqual("テスト", Eval(src, "Info(テストパイロット,愛称)"));
        }

        [TestMethod]
        public void NumericContext_ConvertsResultToNumber()
        {
            // 数値として評価される場合は結果の文字列を数値に変換する
            var src = CreateSrc();
            Assert.AreEqual(1201d, src.Expression.GetValueAsDouble("Info(ユニットデータ,テスト機,修理費) + 1"));
        }

        [TestMethod]
        public void UnknownObject_ReturnsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", Eval(src, "Info(ユニット,存在しない,名称)"));
            Assert.AreEqual("", Eval(src, "Info(ユニットデータ,存在しない,最大攻撃力)"));
        }
    }
}
