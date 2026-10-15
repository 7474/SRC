using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.PlayMessage のメッセージ変数置換と、
    /// SysMessage / IsSysMessageDefined / SysMessageIfDefined (解説メッセージ) のテスト。
    /// 元の SRC の Unit.cls PlayMessage / SysMessage / IsSysMessageDefined に従う。
    /// </summary>
    [TestClass]
    public class UnitMessageAnimeSysMessageTests
    {
        // ──────────────────────────────────────────────
        // PlayMessage の変数置換
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayMessage_SubstitutesOwnUnitPilotWeaponAndDamage()
        {
            // $(ユニット)/$(機体) は愛称の括弧以降と「専用」以前を除いたもの、
            // $(パイロット) は愛称の括弧以前、$(武器) は武器名の括弧・<> 以前、$(損傷率) は損傷率
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(unickname: "隊長専用ザク(指揮官機)", pname: "シャア", pnickname: "シャア(赤い彗星)");
            u.HP = u.MaxHP * 3 / 4;
            var p = u.MainPilot();

            u.PlayMessage(p, "$(ユニット)/$(機体)/$(パイロット)/$(武器)/$(損傷率)", "ヒートホーク(投擲)<改>");

            CollectionAssert.AreEqual(new[] { (p.ID, "ザク/ザク/シャア/ヒートホーク/25") }, f.Messages);
        }

        [TestMethod]
        public void PlayMessage_SubstitutesOpponentWhenSelectedUnit()
        {
            // 自分が SelectedUnit なら SelectedTarget と SelectedTWeapon を相手として置換する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "自機", pname: "自分");
            var t = f.CreateUnit(uname: "敵機", unickname: "量産型敵機(先行型)", pname: "相手", pnickname: "相手(ザコ)", party: "敵",
                setup: ud =>
                {
                    var w = ud.AddWeapon("マシンガン");
                    w.MinRange = 1;
                    w.MaxRange = 3;
                });
            t.HP = t.MaxHP / 2;
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedTarget = t;
            f.Src.Commands.SelectedTWeapon = 1;

            u.PlayMessage(u.MainPilot(), "$(相手ユニット)|$(相手機体)|$(相手パイロット)|$(相手武器)|$(相手損傷率)", "");

            CollectionAssert.AreEqual(new[] { ("自分", "量産型敵機|量産型敵機|相手|マシンガン|50") }, f.Messages);
        }

        [TestMethod]
        public void PlayMessage_OpponentWeaponOutOfRange_SubstitutesEmpty()
        {
            // 相手の武器番号が範囲外なら $(相手武器) は空文字列になる。
            // 自分が SelectedUnit でなければ SelectedUnit を相手とする
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "自機", pname: "自分");
            var t = f.CreateUnit(uname: "敵機", pname: "相手", party: "敵");
            f.Src.Commands.SelectedUnit = t;
            f.Src.Commands.SelectedTarget = u;
            f.Src.Commands.SelectedWeapon = 5;

            u.PlayMessage(u.MainPilot(), "[$(相手武器)]$(相手ユニット)", "");

            CollectionAssert.AreEqual(new[] { ("自分", "[]敵機") }, f.Messages);
        }

        [TestMethod]
        public void PlayMessage_PictureDrawn_ClearsCutInUnlessOptionDefined()
        {
            // メッセージ表示中にカットインが描画されたら消去する。
            // Option「戦闘中画面初期化無効」が定義されていれば消去しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Gui.DisplayBattleMessageHandler = (p, m, mode) => { f.Calls.Add("DisplayBattleMessage"); f.Gui.IsPictureDrawn = true; };

            u.PlayMessage(u.MainPilot(), "カットイン", "");
            CollectionAssert.AreEqual(new[] { "DisplayBattleMessage", "ClearPicture", "UpdateScreen" }, f.Calls);

            f.Calls.Clear();
            f.Src.Expression.DefineGlobalVariable("Option(戦闘中画面初期化無効)");
            u.PlayMessage(u.MainPilot(), "カットイン", "");
            CollectionAssert.AreEqual(new[] { "DisplayBattleMessage" }, f.Calls);
        }

        // ──────────────────────────────────────────────
        // SysMessage
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SysMessage_EffectDataGeneric_DisplaysWithDashSpeakerAndSubstitution()
        {
            // 特殊効果データの「状況(解説)」を話者「-」で表示し、変数を置換する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(unickname: "ガンダム");
            f.Src.EDList.Add("汎用").AddMessage("分身(解説)", "$(ユニット)は分身した！");

            u.SysMessage("分身");

            CollectionAssert.AreEqual(new[] { ("-", "ガンダムは分身した！") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_SubSituation_PrefersCombinedSituationThenMain()
        {
            // サブシチュエーション付きの「状況(サブ)(解説)」を優先し、無ければ「状況(解説)」を使う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            var md = f.Src.EDList.Add("汎用");
            md.AddMessage("修理(解説)", "修理した");
            md.AddMessage("修理(修理装置)(解説)", "修理装置で修理した");

            u.SysMessage("修理", "修理装置");
            u.SysMessage("修理", "修理キット");
            // メインと同じサブシチュエーションはサブ無しとして扱う
            u.SysMessage("修理", "修理");

            CollectionAssert.AreEqual(
                new[] { ("-", "修理装置で修理した"), ("-", "修理した"), ("-", "修理した") },
                f.Messages);
        }

        [TestMethod]
        public void SysMessage_AppendsAdditionalMessageAndCancelsOnDash()
        {
            // add_msg はメッセージの後ろに連結される。メッセージが「-」なら表示しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            var md = f.Src.EDList.Add("汎用");
            md.AddMessage("回復(解説)", "ＨＰが回復した");
            md.AddMessage("消滅(解説)", "-");

            u.SysMessage("回復", "", "（+100）");
            u.SysMessage("消滅");

            CollectionAssert.AreEqual(new[] { ("-", "ＨＰが回復した（+100）") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_SearchOrder_UnitNameThenNormalizedNicknameThenClassThenGeneric()
        {
            // 検索順はユニット名称 → 正規化した愛称 → ユニットクラス → 汎用
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "量産型ザク改カスタム機", unickname: "量産型ザク改(緑)", uclass: "ジオン系");
            f.Src.EDList.Add("汎用").AddMessage("A(解説)", "汎用A");
            f.Src.EDList.Item("汎用").AddMessage("B(解説)", "汎用B");
            f.Src.EDList.Item("汎用").AddMessage("C(解説)", "汎用C");
            f.Src.EDList.Add("ジオン系").AddMessage("A(解説)", "クラスA");
            f.Src.EDList.Item("ジオン系").AddMessage("B(解説)", "クラスB");
            // 「量産型ザク改(緑)」→ 括弧以降と「型」以前、末尾の「改」を除いて「ザク」
            f.Src.EDList.Add("ザク").AddMessage("A(解説)", "愛称A");
            f.Src.EDList.Add("量産型ザク改カスタム機").AddMessage("C(解説)", "名称C");

            u.SysMessage("A");
            u.SysMessage("B");
            u.SysMessage("C");

            CollectionAssert.AreEqual(new[] { ("-", "愛称A"), ("-", "クラスB"), ("-", "名称C") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_ExtendedAnimationDataIsSearchedFirst()
        {
            // 拡張戦闘アニメが有効なら拡張戦闘アニメデータを特殊効果データより先に検索する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避(解説)", "特殊効果");
            f.Src.EADList.Add("汎用").AddMessage("回避(解説)", "拡張アニメ");

            f.Config.ExtendedAnimation = false;
            u.SysMessage("回避");
            f.Config.ExtendedAnimation = true;
            u.SysMessage("回避");

            CollectionAssert.AreEqual(new[] { ("-", "特殊効果"), ("-", "拡張アニメ") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_BattleAnimationDataIsSearched()
        {
            // 元の SRC の SysMessage は拡張戦闘アニメデータ → 戦闘アニメデータ → 特殊効果データの順に検索する。
            // ヘルプ「戦闘アニメデータ」にも「(解説)」を付けたシチュエーションで解説メッセージを定義できるとある
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.ADList.Add("汎用").AddMessage("奇妙な踊り(解説)", "$(ユニット)は奇妙な踊りを踊った！");

            Assert.IsTrue(u.IsSysMessageDefined("奇妙な踊り"));
            u.SysMessage("奇妙な踊り");

            CollectionAssert.AreEqual(new[] { ("-", "テスト機は奇妙な踊りを踊った！") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_SubstitutesSelectedWeaponName()
        {
            // 自分が SelectedUnit なら SelectedWeapon の武器名を $(武器) に使う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(setup: ud =>
            {
                var w = ud.AddWeapon("ビームライフル");
                w.MaxRange = 3;
                w.MinRange = 1;
            });
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedWeapon = 1;
            f.Src.EDList.Add("汎用").AddMessage("攻撃(解説)", "$(ユニット)の$(武器)！");

            u.SysMessage("攻撃");

            CollectionAssert.AreEqual(new[] { ("-", "テスト機のビームライフル！") }, f.Messages);
        }

        [TestMethod]
        public void IsSysMessageDefined_And_SysMessageIfDefined()
        {
            // IsSysMessageDefined は解説メッセージの有無を返し、
            // SysMessageIfDefined は最初に定義されているものを表示して真を返す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避(解説)", "回避した");

            Assert.IsTrue(u.IsSysMessageDefined("回避"));
            Assert.IsFalse(u.IsSysMessageDefined("破壊"));

            Assert.IsFalse(u.SysMessageIfDefined(new[] { "破壊" }));
            Assert.AreEqual(0, f.Messages.Count);
            Assert.IsTrue(u.SysMessageIfDefined(new[] { "破壊", "回避" }));
            CollectionAssert.AreEqual(new[] { ("-", "回避した") }, f.Messages);
        }
    }
}
