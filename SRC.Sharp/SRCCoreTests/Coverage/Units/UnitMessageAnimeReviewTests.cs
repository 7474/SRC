using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.message.cs / Unit.anime.cs / Unit.se.cs の追加テスト。
    /// 特殊効果・戦闘アニメの center / 効果音 / keep の処理と、
    /// PilotMessage のアビリティ・スペシャルパワー・武器名の扱い、PlayDialog の話者を検証する。
    /// </summary>
    [TestClass]
    public class UnitMessageAnimeReviewTests
    {
        private const string PName = "テストパイロット";

        private static Unit CreateArmedUnit(UnitMessageAnimeFixture f, string pname = PName)
        {
            return f.CreateUnit(pname: pname, setup: ud =>
            {
                var saber = ud.AddWeapon("ビームサーベル");
                saber.Class = "格";
                saber.MinRange = 1;
                saber.MaxRange = 1;
                var rifle = ud.AddWeapon("ビームライフル");
                rifle.Class = "射";
                rifle.MinRange = 1;
                rifle.MaxRange = 5;
            });
        }

        // ──────────────────────────────────────────────
        // SpecialEffect: center / 効果音 / keep
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialEffect_Center_CentersOnNamedUnitAndRedraws()
        {
            // 「center ユニット」は指定ユニットの座標を中央に表示して画面を再描画する。
            // 再描画済みなので直前の画面処理による再表示 (UpdateScreen) は不要になる (Unit.cls SpecialEffect)
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            u.x = 7;
            u.y = 4;
            var centers = new List<(int, int)>();
            f.Gui.CenterHandler = (x, y) => { centers.Add((x, y)); f.Calls.Add("Center"); };
            f.Src.EDList.Add("汎用").AddMessage("回避", "Line 0 0 1 1;center テスト機;center 存在しない機体");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { (7, 4) }, centers);
            CollectionAssert.AreEqual(
                new[] { "DisplayBattleMessage", "Center", "RedrawScreen", "CloseMessageForm" },
                f.Calls);
        }

        [TestMethod]
        public void SpecialEffect_SoundAfterWait_RefreshesThenSleepsBeforeContinuing()
        {
            // 効果音の前にウェイトがあれば、必要なら画面を更新してから待つ。
            // 待ちを消費するので、後続のメッセージには秒数が前置されない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "Line 0 0 1 1;0.5;-.wav;かわした");

            u.SpecialEffect("回避");

            CollectionAssert.AreEqual(new[] { ("-", "Line 0 0 1 1"), ("-", "かわした") }, f.Messages);
            CollectionAssert.AreEqual(
                new[] { "DisplayBattleMessage", "UpdateScreen", "Sleep", "OpenMessageForm", "DisplayBattleMessage", "CloseMessageForm" },
                f.Calls);
            CollectionAssert.AreEqual(new[] { 50 }, f.Sleeps);
        }

        [TestMethod]
        public void SpecialEffect_Keep_StopsProcessingRemainingParts()
        {
            // 元の SRC の SpecialEffect では keep で Exit For し、以降の指定は処理しない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避", "keep;かわした");

            u.SpecialEffect("回避");

            Assert.AreEqual(0, f.Messages.Count);
            CollectionAssert.DoesNotContain(f.Calls, "OpenMessageForm");
        }

        // ──────────────────────────────────────────────
        // PlayAnimation: center / 効果音
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayAnimation_CenterAndSoundAfterWait()
        {
            // 戦闘アニメでも center は中央表示 + 再描画、ウェイト後の効果音は画面更新してから待つ
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            u.x = 2;
            u.y = 9;
            var centers = new List<(int, int)>();
            f.Gui.CenterHandler = (x, y) => { centers.Add((x, y)); f.Calls.Add("Center"); };
            f.Config.BattleAnimation = true;
            f.Src.ADList.Add("汎用").AddMessage("チャージ", "center テスト機;Line 0 0 1 1;0.3;-.wav");

            u.PlayAnimation("チャージ");

            CollectionAssert.AreEqual(new[] { (2, 9) }, centers);
            CollectionAssert.AreEqual(
                new[] { "Center", "RedrawScreen", "DisplayBattleMessage", "UpdateScreen", "Sleep", "CloseMessageForm" },
                f.Calls);
            CollectionAssert.AreEqual(new[] { 30 }, f.Sleeps);
        }

        // ──────────────────────────────────────────────
        // PilotMessage: アビリティ / スペシャルパワー / 武器名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_AbilityMode_FallsBackToAbilitySituation()
        {
            // msg_mode が「アビリティ」ならアビリティ名のメッセージが無いとき「アビリティ」を使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.MDList.Add(PName).AddMessage("アビリティ", "任せろ");

            u.PilotMessage("ヒール");
            Assert.AreEqual(0, f.Messages.Count);

            u.PilotMessage("ヒール", "アビリティ");
            CollectionAssert.AreEqual(new[] { (PName, "任せろ") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedSpecialPower_UsesSelectedPilotMessageAndSpeaker()
        {
            // 使用中のスペシャルパワー名のシチュエーションでは、
            // SelectedPilot (スペシャルパワーを使ったパイロット) のメッセージデータで表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.MDList.Add(PName).AddMessage("熱血", "メインの熱血");
            f.Src.PDList.Add("応援者").Sex = "女性";
            var supporter = f.Src.PList.Add("応援者", 1, "味方");
            f.Src.MDList.Add("応援者").AddMessage("熱血", "がんばって！");
            f.Src.Commands.SelectedSpecialPower = "熱血";
            f.Src.Commands.SelectedPilot = supporter;

            u.PilotMessage("熱血");

            CollectionAssert.AreEqual(new[] { (supporter.ID, "がんばって！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_NonAttackSituation_SubstitutesSelectedWeapon()
        {
            // 攻撃メッセージでなくても、攻撃側なら SelectedWeapon、反撃側なら SelectedTWeapon の武器名を $(武器) に使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.MDList.Add(PName).AddMessage("チャージ", "$(武器)で決める");

            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedWeapon = 2;
            u.PilotMessage("チャージ");

            f.Src.Commands.SelectedUnit = null;
            f.Src.Commands.SelectedTarget = u;
            f.Src.Commands.SelectedTWeapon = 1;
            u.PilotMessage("チャージ");

            CollectionAssert.AreEqual(
                new[] { (PName, "ビームライフルで決める"), (PName, "ビームサーベルで決める") },
                f.Messages);
        }

        // ──────────────────────────────────────────────
        // PlayDialog / IsSysMessageDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayDialog_MainPilotNameMapsToIdAndSubstitutesUnitName()
        {
            // ダイアログの話者がメインパイロット名ならパイロット ID で表示する (ザコは名前と ID が異なる)。
            // $(ユニット) は愛称の括弧以降と「専用」までを除いた名前に置換する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(uname: "量産機", unickname: "隊長専用ザク(指揮官機)", pname: "兵士(ザコ)");
            var p = u.MainPilot();
            Assert.AreNotEqual(p.Name, p.ID);
            var dd = new Dialog(f.Src);
            dd.AddMessage("兵士(ザコ)", "$(ユニット)、出るぞ");

            u.PlayDialog(dd, "");

            CollectionAssert.AreEqual(new[] { (p.ID, "ザク、出るぞ") }, f.Messages);
        }

        [TestMethod]
        public void IsSysMessageDefined_SubSituation_ChecksCombinedThenMain()
        {
            // サブシチュエーション指定時は「状況(サブ)(解説)」→「状況(解説)」の順に探す
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("修理(修理装置)(解説)", "修理装置で修理した");

            Assert.IsTrue(u.IsSysMessageDefined("修理", "修理装置"));
            Assert.IsFalse(u.IsSysMessageDefined("修理", "別の装置"));
            Assert.IsFalse(u.IsSysMessageDefined("修理"));

            f.Src.EDList.Item("汎用").AddMessage("修理(解説)", "修理した");
            Assert.IsTrue(u.IsSysMessageDefined("修理", "別の装置"));
        }
    }
}
