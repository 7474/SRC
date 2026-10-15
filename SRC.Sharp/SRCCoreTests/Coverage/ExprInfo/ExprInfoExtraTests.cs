using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数のうち、追加のデータ（地形適応 S/D/-、属性レベル、霊力・統率・カウンターなど）を要するテスト
    /// 仕様: Info関数.md / VB の EvalInfoFunc（Expression.bas）
    /// </summary>
    [TestClass]
    public class ExprInfoExtraTests
    {
        private const string ExtraRobotData = @"テスト機改
テスト機改, てすときかい, 汎用, 1, 2
陸, 4, M, 1200, 150
特殊能力
バリア=Ｉフィールド 1000 10 (底力)
1000, 100, 600, 60
SD-E, test_unit2.bmp
レベル付きビーム, 1800, 1, 3, +0, -, -, -, AAAA, +0, ＢL2
レベル付き実弾, 1600, 1, 3, +0, -, -, -, AAAA, +0, 実L3突

";

        private const string ExtraPilotData = @"テスト指揮官
指揮官, しきかん, 女性, 汎用, SSSS, 100
特殊能力
底力, 1
霊力Lv20, 1
統率Lv1, 1
カウンターLv2, 1
先手必勝, 1
耐久Lv2, 1
120, 120, 120, 120, 120, 120, 普通
ＳＰ, 40, 加速, 1
cmd.bmp, -

";

        private static SRC CreateExtraSrc()
        {
            var src = CreateSrc();
            src.UDList.Load("robot2.txt", new MemoryStream(Encoding.UTF8.GetBytes(ExtraRobotData)));
            src.PDList.Load("pilot2.txt", new MemoryStream(Encoding.UTF8.GetBytes(ExtraPilotData)));
            return src;
        }

        private static Units.Unit CreateCommanderUnit(SRC src, int level = 2)
        {
            var p = src.PList.Add("テスト指揮官", level, "味方");
            var u = src.UList.Add("テスト機改", 0, "味方");
            p.Ride(u);
            u.Update();
            return u;
        }

        private static string U(Units.Unit u, string rest)
        {
            return $"Info(ユニット,\"{u.ID}\",{rest})";
        }

        // ──────────────────────────────────────────────
        // ユニット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Unit_Adaption_ConvertsSDAndHyphen()
        {
            // ユニット SD-E とパイロット SSSS の低い方を取り、5→S 1→D 0→E に変換する
            // 「-」は適応無し（0）なので E になる（Unit.cls Adaption / Expression.bas 地形適応）
            var src = CreateExtraSrc();
            var u = CreateCommanderUnit(src);
            Assert.AreEqual("SDEE", Eval(src, U(u, "地形適応")));
            // ユニットデータの地形適応はデータの文字列そのまま
            Assert.AreEqual("SD-E", Eval(src, "Info(ユニットデータ,テスト機改,地形適応)"));
        }

        [TestMethod]
        public void Unit_Adaption_UsesLowerOfUnitAndPilot()
        {
            // テスト機（AABC）にテスト指揮官（SSSS）が乗ればユニット側の値、
            // テスト機改（SD-E）にテストパイロット（AABA）が乗れば S は A に下がる
            var src = CreateExtraSrc();
            var p = src.PList.Add("テストパイロット", 1, "味方");
            var u = src.UList.Add("テスト機改", 0, "味方");
            p.Ride(u);
            u.Update();
            Assert.AreEqual("ADEE", Eval(src, U(u, "地形適応")));
        }

        [TestMethod]
        public void Unit_Weapon_AvailableDependsOnMorale()
        {
            // テストビームは必要気力 110。気力 100 では使用不可、110 で使用可
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            var p = u.MainPilot();
            p.Morale = 100;
            Assert.AreEqual("0", Eval(src, U(u, "武器,テストビーム,使用可")));
            // 必要気力の無いテストパンチは使用可
            Assert.AreEqual("1", Eval(src, U(u, "武器,テストパンチ,使用可")));

            p.Morale = 110;
            Assert.AreEqual("1", Eval(src, U(u, "武器,テストビーム,使用可")));
        }

        [TestMethod]
        public void Unit_Weapon_AvailableDependsOnEN()
        {
            // テストビームは消費ＥＮ 10。ＥＮ 9 では使用不可
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            u.MainPilot().Morale = 110;
            u.EN = 9;
            Assert.AreEqual("0", Eval(src, U(u, "武器,テストビーム,使用可")));
            u.EN = 10;
            Assert.AreEqual("1", Eval(src, U(u, "武器,テストビーム,使用可")));
        }

        [TestMethod]
        public void Unit_Weapon_AttributeLevel_AtEndOfClass()
        {
            // 属性「ＢL2」のレベルは 2、レベル指定の無い属性は 0（Unit.cls WeaponLevel）
            var src = CreateExtraSrc();
            var u = CreateCommanderUnit(src);
            Assert.AreEqual("2", Eval(src, U(u, "武器,レベル付きビーム,属性レベル,Ｂ")));
            Assert.AreEqual("0", Eval(src, U(u, "武器,レベル付きビーム,属性レベル,実")));
        }

        [TestMethod]
        public void Unit_Weapon_AttributeLevel_FollowedByOtherAttribute()
        {
            // 「実L3突」の実のレベルは 3。VB の WeaponLevel は数字以外の文字で切り出しを止める
            var src = CreateExtraSrc();
            var u = CreateCommanderUnit(src);
            Assert.AreEqual("3", Eval(src, U(u, "武器,レベル付き実弾,属性レベル,実")));
        }

        [TestMethod]
        public void Unit_Weapon_AttributeName()
        {
            // 属性名称は属性の正式名称（Help.bas AttributeName）
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("ビーム攻撃", Eval(src, U(u, "武器,1,属性名称,Ｂ")));
            Assert.AreEqual("突進技", Eval(src, U(u, "武器,テストパンチ,属性名称,突")));
        }

        [TestMethod]
        public void Unit_FeatureNameAndNecessarySkill()
        {
            // 「バリア=Ｉフィールド 1000 10 (底力)」の名称はデータ先頭の Ｉフィールド、必要技能は 底力
            var src = CreateExtraSrc();
            var u = CreateCommanderUnit(src);
            Assert.AreEqual("Ｉフィールド", Eval(src, U(u, "特殊能力名称,バリア")));
            Assert.AreEqual("Ｉフィールド", Eval(src, U(u, "特殊能力名称,1")));
            Assert.AreEqual("底力", Eval(src, U(u, "特殊能力必要技能,バリア")));
            Assert.AreEqual("底力", Eval(src, "Info(ユニットデータ,テスト機改,特殊能力必要技能,バリア)"));
            // 必要技能の無い特殊能力は空文字列
            var u2 = CreatePilotedUnit(src);
            Assert.AreEqual("", Eval(src, U(u2, "特殊能力必要技能,バリア")));
        }

        [TestMethod]
        public void Unit_FeatureHelp_DescribesBarrier()
        {
            // 特殊能力解説はヘルプの解説文（Help.bas FeatureHelpMessage）。バリアの値が入る
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            var help = Eval(src, U(u, "特殊能力解説,バリア"));
            StringAssert.Contains(help, "1000");
            StringAssert.Contains(help, "10");
        }

        [TestMethod]
        public void Unit_RemainingSyncAndCounterAttack()
        {
            // 残り同時援護攻撃数 = 統率レベル - 使用数、残りカウンター攻撃数 = カウンターレベル - 使用数
            var src = CreateExtraSrc();
            var u = CreateCommanderUnit(src);
            var p = u.MainPilot();
            p.Morale = 100;
            Assert.AreEqual("1", Eval(src, U(u, "残り同時援護攻撃数")));
            Assert.AreEqual("2", Eval(src, U(u, "残りカウンター攻撃数")));

            u.UsedSyncAttack = 1;
            u.UsedCounterAttack = 1;
            Assert.AreEqual("0", Eval(src, U(u, "残り同時援護攻撃数")));
            Assert.AreEqual("1", Eval(src, U(u, "残りカウンター攻撃数")));

            // 先手必勝は気力 120 以上でカウンター回数 1000（Unit.cls MaxCounterAttack）
            p.Morale = 120;
            Assert.AreEqual("999", Eval(src, U(u, "残りカウンター攻撃数")));
        }

        [TestMethod]
        public void Unit_BossRank_DefaultsToMinusOne()
        {
            // ボスランクの初期値は -1（Unit.cls Class_Initialize）
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("-1", Eval(src, U(u, "ボスランク")));
        }

        // ──────────────────────────────────────────────
        // パイロット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Pilot_Plana_GrowsWithLevel()
        {
            // 最大霊力 = 霊力Lv + 1.5 × レベル（Pilot.cls MaxPlana）。レベル2 で 20 + 3
            var src = CreateExtraSrc();
            CreateCommanderUnit(src, level: 2);
            Assert.AreEqual("23", Eval(src, "Info(パイロット,テスト指揮官,最大霊力)"));
            Assert.AreEqual("23", Eval(src, "Info(パイロット,テスト指揮官,最大プラーナ)"));
            // 作成直後は全快しているので現在値も最大値
            Assert.AreEqual("23", Eval(src, "Info(パイロット,テスト指揮官,霊力)"));
            Assert.AreEqual("23", Eval(src, "Info(パイロット,テスト指揮官,プラーナ)"));
        }

        [TestMethod]
        public void Pilot_Plana_WithoutSkill_IsZero()
        {
            var src = CreateSrc();
            CreatePilotedUnit(src, level: 5);
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,最大霊力)"));
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,霊力)"));
        }

        [TestMethod]
        public void Pilot_Defense_DependsOnEnduranceAndOption()
        {
            // 防御 = 100 + 5 × 耐久Lv（Pilot.cls Defense）
            var src = CreateExtraSrc();
            CreateCommanderUnit(src, level: 2);
            Assert.AreEqual("110", Eval(src, "Info(パイロット,テスト指揮官,防御)"));
            // Option(防御力成長) ではレベル分が加わる
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            Assert.AreEqual("112", Eval(src, "Info(パイロット,テスト指揮官,防御)"));
        }

        [TestMethod]
        public void Pilot_Exp_Over500_LevelsUp()
        {
            // 累積経験値は 500 ごとにレベルへ繰り上がった残り（Pilot.cls Exp）
            var src = CreateSrc();
            var p = CreatePilotedUnit(src, level: 3).MainPilot();
            Assert.AreEqual("0", Eval(src, "Info(パイロット,テストパイロット,累積経験値)"));
            p.Exp = 650;
            Assert.AreEqual("150", Eval(src, "Info(パイロット,テストパイロット,累積経験値)"));
            Assert.AreEqual("4", Eval(src, "Info(パイロット,テストパイロット,レベル)"));
        }

        // ──────────────────────────────────────────────
        // オプション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Option_ExtendedAnimation_ReturnsOnOff()
        {
            var src = CreateSrc();
            src.SystemConfig.ExtendedAnimation = true;
            Assert.AreEqual("On", Eval(src, "Info(オプション,ExtendedAnimation)"));
            src.SystemConfig.ExtendedAnimation = false;
            Assert.AreEqual("Off", Eval(src, "Info(オプション,ExtendedAnimation)"));
        }

        [TestMethod]
        public void Option_UseDirectMusic_IsOff()
        {
            // SRC# は DirectMusic を使わないので常に Off（VB で UseDirectMusic=False の場合と同じ）
            var src = CreateSrc();
            Assert.AreEqual("Off", Eval(src, "Info(オプション,UseDirectMusic)"));
        }

        [TestMethod]
        public void Option_IniItems_ReturnIniValue()
        {
            // Turn, Square, NewGUI などは On/Off ではなく設定ファイルの Option セクションの値を返す
            // （Expression.bas: ReadIni("Option", name)）。未設定なら空文字列
            var src = CreateSrc();
            src.SystemConfig.SetItem("Option", "Turn", "1");
            src.SystemConfig.SetItem("Option", "MP3Volume", "70");
            Assert.AreEqual("1", Eval(src, "Info(オプション,Turn)"));
            Assert.AreEqual("70", Eval(src, "Info(オプション,MP3Volume)"));
            Assert.AreEqual("", Eval(src, "Info(オプション,NewGUI)"));

            // Option コマンドで定義していても INI 項目名なら INI を参照する
            src.Expression.DefineGlobalVariable("Option(NewGUI)");
            Assert.AreEqual("", Eval(src, "Info(オプション,NewGUI)"));
            // 「新ＧＵＩ」は Option コマンドの設定を参照する
            Assert.AreEqual("Off", Eval(src, "Info(オプション,新ＧＵＩ)"));
            src.Expression.DefineGlobalVariable("Option(新ＧＵＩ)");
            Assert.AreEqual("On", Eval(src, "Info(オプション,新ＧＵＩ)"));
        }
    }
}
