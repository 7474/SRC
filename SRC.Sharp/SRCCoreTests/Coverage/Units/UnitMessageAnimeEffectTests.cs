using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.se.cs (SpecialEffectData / SpecialEffect / IsSpecialEffectDefined) と
    /// Unit.anime.cs (AnimationData / PlayAnimation / IsAnimationDefined / PlayAnimationIfDefined) のテスト。
    /// 元の SRC の Unit.cls の同名メソッドと、ヘルプ「戦闘アニメデータ」「特殊効果データ」に従う。
    /// </summary>
    [TestClass]
    public class UnitMessageAnimeEffectTests
    {
        // ──────────────────────────────────────────────
        // SpecialEffectData の検索順
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialEffectData_SearchOrder_FeatureNameUnitNameNicknameClassGeneric()
        {
            // 特殊効果能力の指定名 → ユニット名称 → 修正した愛称 → ユニットクラス → 汎用 の順に検索する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "試作機", unickname: "試作型ガンダム改(2号機)", uclass: "連邦系",
                setup: ud => ud.AddFeature("特殊効果=専用効果"));
            f.Src.EDList.Add("専用効果").AddMessage("A", "能力指定A");
            f.Src.EDList.Add("試作機").AddMessage("A", "名称A");
            f.Src.EDList.Item("試作機").AddMessage("B", "名称B");
            // 「試作型ガンダム改(2号機)」→ 括弧以降・「型」以前・末尾の「改」を除いて「ガンダム」
            f.Src.EDList.Add("ガンダム").AddMessage("B", "愛称B");
            f.Src.EDList.Item("ガンダム").AddMessage("C", "愛称C");
            f.Src.EDList.Add("連邦系").AddMessage("C", "クラスC");
            f.Src.EDList.Item("連邦系").AddMessage("D", "クラスD");
            f.Src.EDList.Add("汎用").AddMessage("D", "汎用D");
            f.Src.EDList.Item("汎用").AddMessage("E", "汎用E");

            Assert.AreEqual("能力指定A", u.SpecialEffectData("A"));
            Assert.AreEqual("名称B", u.SpecialEffectData("B"));
            Assert.AreEqual("愛称C", u.SpecialEffectData("C"));
            Assert.AreEqual("クラスD", u.SpecialEffectData("D"));
            Assert.AreEqual("汎用E", u.SpecialEffectData("E"));
            Assert.AreEqual("", u.SpecialEffectData("F"));
            Assert.IsTrue(u.IsSpecialEffectDefined("E"));
            Assert.IsFalse(u.IsSpecialEffectDefined("F"));
        }

        [TestMethod]
        public void SpecialEffectData_SubSituation_PrefersCombinedSituationAcrossAllSources()
        {
            // 「状況(サブ)」をすべての検索先で探してから「状況」を探す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("テスト機").AddMessage("修理", "名称の修理");
            f.Src.EDList.Add("汎用").AddMessage("修理(修理装置)", "汎用の修理装置");

            Assert.AreEqual("汎用の修理装置", u.SpecialEffectData("修理", "修理装置"));
            Assert.AreEqual("名称の修理", u.SpecialEffectData("修理", "別の装置"));
            Assert.AreEqual("名称の修理", u.SpecialEffectData("修理", "修理"));
        }

        // ──────────────────────────────────────────────
        // SpecialEffect の再生
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialEffect_TextPart_OpensMessageFormDisplaysAndCloses()
        {
            // 特殊効果指定がメッセージなら、メッセージウィンドウを開いて話者「-」で表示し、終了後に閉じる
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "ひらりとかわした");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { ("-", "ひらりとかわした") }, f.Messages);
            CollectionAssert.AreEqual(new[] { "OpenMessageForm", "DisplayBattleMessage", "CloseMessageForm" }, f.Calls);
            // SelectedTarget が自分でなければ (SelectedTarget, 自分) で開く
            Assert.IsNull(f.OpenArgs[0].u1);
            Assert.AreSame(u, f.OpenArgs[0].u2);
        }

        [TestMethod]
        public void SpecialEffect_MessageFormAlreadyOpenOrKeep_DoesNotClose()
        {
            // 最初からメッセージウィンドウが開いていれば開閉しない。keep_message_form 指定でも閉じない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "かわした");

            f.Gui.MessageFormVisible = true;
            u.SpecialEffect("回避");
            CollectionAssert.AreEqual(new[] { "DisplayBattleMessage" }, f.Calls);

            f.Calls.Clear();
            f.Gui.MessageFormVisible = false;
            u.SpecialEffect("回避", "", true);
            CollectionAssert.AreEqual(new[] { "OpenMessageForm", "DisplayBattleMessage" }, f.Calls);
        }

        [TestMethod]
        public void SpecialEffect_SelectedTargetIsSelf_OpensFormWithSelfOnly()
        {
            // 自分が SelectedTarget なら自分だけを指定してメッセージウィンドウを開く
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.Commands.SelectedTarget = u;
            f.Src.EDList.Add("汎用").AddMessage("回避", "かわした");

            u.SpecialEffect("回避");

            Assert.AreSame(u, f.OpenArgs[0].u1);
            Assert.IsNull(f.OpenArgs[0].u2);
        }

        [TestMethod]
        public void SpecialEffect_WaitIsPrefixedToNextMessageAndTrailingWaitSleeps()
        {
            // 数値はウェイト (秒) で、次のメッセージに「秒数;」として付加される。
            // 末尾のウェイトは再生後に Sleep する (1 秒 = 100)
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "0.5;かわした;1");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { ("-", "0.5;かわした") }, f.Messages);
            CollectionAssert.AreEqual(new[] { 100 }, f.Sleeps);
        }

        [TestMethod]
        public void SpecialEffect_DrawingCommandsAndClear_RefreshScreenWithoutOpeningForm()
        {
            // Line 等の画面処理コマンドはメッセージウィンドウを開かずに表示処理へ渡し、
            // clear は画像を消去する。どちらも最後に画面を更新する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "clear;Line 0 0 10 10");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { ("-", "Line 0 0 10 10") }, f.Messages);
            CollectionAssert.AreEqual(new[] { "ClearPicture", "DisplayBattleMessage", "UpdateScreen", "CloseMessageForm" }, f.Calls);
        }

        [TestMethod]
        public void SpecialEffect_ImageAfterWait_IsDisplayedWithWaitPrefix()
        {
            // 画像 (カットイン) はウェイトを前置して表示し、メッセージウィンドウは開かない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "0.3;cutin.bmp");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { ("-", "0.3;cutin.bmp") }, f.Messages);
            CollectionAssert.DoesNotContain(f.Calls, "OpenMessageForm");
        }

        [TestMethod]
        public void SpecialEffect_CancelledByDashOrRightButton()
        {
            // 「-」は表示キャンセル。右ボタンが押されていれば式評価のみで表示しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "-");
            f.Src.EDList.Item("汎用").AddMessage("破壊", "どかーん");

            u.SpecialEffect("回避");
            f.RButtonPressed = true;
            u.SpecialEffect("破壊");

            Assert.AreEqual(0, f.Calls.Count);
        }

        [TestMethod]
        public void SpecialEffect_SetsSelfAsEventTargetDuringPlaybackAndRestores()
        {
            // 武器・アビリティ以外ではイベント用ターゲットが未設定なら自分を設定し、再生後に元へ戻す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "かわした");

            u.SpecialEffect("回避");

            Assert.AreSame(u, f.TargetsAtDisplay[0]);
            Assert.IsNull(f.Src.Event.SelectedTargetForEvent);
        }

        // ──────────────────────────────────────────────
        // AnimationData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AnimationData_BattleAnimationDisabled_IsNotDefined()
        {
            // 戦闘アニメ表示が無効ならデータがあっても未定義扱い
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.ADList.Add("汎用").AddMessage("回避", "かわした");
            f.Config.BattleAnimation = false;

            Assert.IsTrue(string.IsNullOrEmpty(u.AnimationData("回避", "")));
            Assert.IsFalse(u.IsAnimationDefined("回避"));
        }

        [TestMethod]
        public void AnimationData_SearchOrder_AndExtendedAnimationPriority()
        {
            // 戦闘アニメ能力の指定名 → ユニット名称 → 修正した愛称 → クラス → 汎用 の順。
            // 拡張戦闘アニメが有効なら各段階で拡張戦闘アニメデータを先に探す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "試作機", unickname: "試作機(1号)", uclass: "連邦系",
                setup: ud => ud.AddFeature("戦闘アニメ=専用アニメ"));
            f.Config.BattleAnimation = true;
            f.Config.ExtendedAnimation = true;
            f.Src.ADList.Add("専用アニメ").AddMessage("A", "能力指定A");
            f.Src.EADList.Add("試作機").AddMessage("A", "拡張名称A");
            f.Src.ADList.Add("試作機").AddMessage("B", "名称B");
            f.Src.EADList.Add("連邦系").AddMessage("B", "拡張クラスB");
            f.Src.ADList.Add("汎用").AddMessage("C", "汎用C");
            f.Src.EADList.Add("汎用").AddMessage("C", "拡張汎用C");

            Assert.AreEqual("能力指定A", u.AnimationData("A", ""));
            Assert.AreEqual("名称B", u.AnimationData("B", ""));
            Assert.AreEqual("拡張汎用C", u.AnimationData("C", ""));
            // ext_anime_only では通常の戦闘アニメデータを使わない
            Assert.AreEqual("拡張名称A", u.AnimationData("A", "", true));
            Assert.AreEqual("拡張クラスB", u.AnimationData("B", "", true));
            Assert.IsFalse(u.IsAnimationDefined("D"));
            Assert.IsTrue(u.IsAnimationDefined("C", "", true));
        }

        [TestMethod]
        public void AnimationData_ExtendedAnimationDisabled_IgnoresExtendedData()
        {
            // 拡張戦闘アニメが無効なら拡張戦闘アニメデータは参照しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Config.ExtendedAnimation = false;
            f.Src.EADList.Add("汎用").AddMessage("回避", "拡張");

            Assert.IsFalse(u.IsAnimationDefined("回避"));
        }

        [TestMethod]
        public void AnimationData_SubSituation_PrefersCombinedSituation()
        {
            // 元の SRC の AnimationData は sub_situation が空でもメインと同じでもなければ
            // 「状況(サブ)」を先に探す。例: PlayAnimation "修理", FeatureName("修理装置")
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            var md = f.Src.ADList.Add("汎用");
            md.AddMessage("修理", "通常の修理");
            md.AddMessage("修理(修理装置)", "修理装置の修理");

            Assert.AreEqual("修理装置の修理", u.AnimationData("修理", "修理装置"));
            Assert.AreEqual("通常の修理", u.AnimationData("修理", "修理"));
        }

        // ──────────────────────────────────────────────
        // PlayAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayAnimation_TextWithoutSubroutine_DisplaysAsMessage()
        {
            // 戦闘アニメ用サブルーチン (戦闘アニメ_○○発動) が見つからない指定はメッセージとして表示する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("チャージ", "エネルギー充填");

            u.PlayAnimation("チャージ");

            CollectionAssert.AreEqual(new[] { ("-", "エネルギー充填") }, f.Messages);
            CollectionAssert.AreEqual(new[] { "OpenMessageForm", "DisplayBattleMessage", "CloseMessageForm" }, f.Calls);
            Assert.IsNull(f.Src.Event.SelectedTargetForEvent);
        }

        [TestMethod]
        public void PlayAnimation_ActivationSuffix_FallsBackToBaseSituation()
        {
            // 「名前(発動)」が無ければ「名前」のアニメ指定を使う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("修理装置", "repair.bmp");

            u.PlayAnimation("修理装置(発動)");

            CollectionAssert.AreEqual(new[] { ("-", "repair.bmp") }, f.Messages);
        }

        [TestMethod]
        public void PlayAnimation_BulkPreparationWithoutSubroutine_IsSkipped()
        {
            // 一括指定 (「武器名」で「武器名(準備)」を代用) でサブルーチンが無い場合、準備アニメは表示しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("ビームライフル", "ビーム");

            u.PlayAnimation("ビームライフル(準備)");

            Assert.AreEqual(0, f.Messages.Count);
            CollectionAssert.DoesNotContain(f.Calls, "OpenMessageForm");
        }

        [TestMethod]
        public void PlayAnimation_RightButtonCancelsExceptTermination()
        {
            // 右ボタンでアニメはキャンセルされるが、終了処理 (「終了」「(終了)」) はキャンセルしない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            var md = f.Src.ADList.Add("汎用");
            md.AddMessage("チャージ", "charge.bmp");
            md.AddMessage("ビームライフル(終了)", "end.bmp");
            f.RButtonPressed = true;

            u.PlayAnimation("チャージ");
            u.PlayAnimation("ビームライフル(終了)");

            CollectionAssert.AreEqual(new[] { ("-", "end.bmp") }, f.Messages);
        }

        [TestMethod]
        public void PlayAnimation_ImageDrawingAndWait_AreProcessedInOrder()
        {
            // 画像・画面処理コマンドはサブルーチンを呼ばずに表示処理へ渡す。
            // 直前のウェイトは「秒数;」として前置され、末尾のウェイトは再生後に Sleep する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("チャージ", "0.2;cut.bmp;Line 1 1 2 2;0.5");

            u.PlayAnimation("チャージ");

            CollectionAssert.AreEqual(new[] { ("-", "0.2;cut.bmp"), ("-", "Line 1 1 2 2") }, f.Messages);
            CollectionAssert.AreEqual(
                new[] { "DisplayBattleMessage", "DisplayBattleMessage", "UpdateScreen", "Sleep", "CloseMessageForm" },
                f.Calls);
            CollectionAssert.AreEqual(new[] { 50 }, f.Sleeps);
        }

        [TestMethod]
        public void PlayAnimation_EventTarget_SetToSelfOnlyForNonWeaponSituation()
        {
            // 武器の「武器名(攻撃)」以外では、イベント用ターゲットが未設定なら自分を設定し、再生後に戻す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(setup: ud =>
            {
                var w = ud.AddWeapon("ビームライフル");
                w.MinRange = 1;
                w.MaxRange = 3;
            });
            f.Config.BattleAnimation = true;
            var md = f.Src.ADList.Add("汎用");
            md.AddMessage("ビームライフル(攻撃)", "beam.bmp");
            md.AddMessage("チャージ", "charge.bmp");

            u.PlayAnimation("ビームライフル(攻撃)");
            u.PlayAnimation("チャージ");

            Assert.IsNull(f.TargetsAtDisplay[0]);
            Assert.AreSame(u, f.TargetsAtDisplay[1]);
            Assert.IsNull(f.Src.Event.SelectedTargetForEvent);
        }

        [TestMethod]
        public void PlayAnimation_DashCancels()
        {
            // 「-」は表示キャンセル
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("チャージ", "-");

            u.PlayAnimation("チャージ");

            Assert.AreEqual(0, f.Calls.Count);
        }

        [TestMethod]
        public void PlayAnimationIfDefined_PrefersAnimationThenSpecialEffect()
        {
            // 戦闘アニメが定義されていれば最初に見つかったものを再生し、
            // どれも無ければ特殊効果を探して再生する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("B", "anime.bmp");
            f.Src.EDList.Add("汎用").AddMessage("A", "効果A");
            f.Src.EDList.Item("汎用").AddMessage("C", "効果C");

            u.PlayAnimationIfDefined(new[] { "A", "B" });
            u.PlayAnimationIfDefined(new[] { "X", "C" });
            u.PlayAnimationIfDefined(new[] { "X", "Y" });

            CollectionAssert.AreEqual(new[] { ("-", "anime.bmp"), ("-", "効果C") }, f.Messages);
        }

        [TestMethod]
        public void PlayAnimation_Array_LastSituationAlwaysTriedAsSpecialEffect()
        {
            // 配列版はアニメが無ければ最後以外の特殊効果を探し、無ければ最後の状況の特殊効果を再生する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Config.BattleAnimation = true;
            f.Src.EDList.Add("汎用").AddMessage("変形", "変形した");
            f.Src.EDList.Item("汎用").AddMessage("変形(B形態)", "B形態に変形");

            u.PlayAnimation(new[] { "変形(B形態)", "変形" });
            u.PlayAnimation(new[] { "合体(X)", "変形" });

            CollectionAssert.AreEqual(new[] { ("-", "B形態に変形"), ("-", "変形した") }, f.Messages);
        }
    }
}
