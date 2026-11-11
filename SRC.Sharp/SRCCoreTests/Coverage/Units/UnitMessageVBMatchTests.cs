using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 元の SRC の Unit.cls PilotMessage / PlayDialog / IsMessageDefined / SysMessage と
    /// 突き合わせて見つかった違いのテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class UnitMessageVBMatchTests
    {
        private const string PName = "テストパイロット";

        private static Pilot AddSupport(UnitMessageAnimeFixture f, Unit u, string name)
        {
            f.Src.PDList.Add(name).Sex = "男性";
            var s = f.Src.PList.Add(name, 1, "味方");
            u.AddSupport(s);
            s.Unit = u;
            return s;
        }

        // ──────────────────────────────────────────────
        // PilotMessage のパイロットの選び方
        // ──────────────────────────────────────────────

        [TestMethod]
        [Timeout(10000)]
        public void PilotMessage_SpecialPowerOfSupportWithoutMessage_DisplaysNothing()
        {
            // スペシャルパワーを使ったサポートのメッセージデータに、そのシチュエーションが無い場合。
            // 元の SRC は i = 1 のままなので TrySelectedMessage に進み、何も表示しない
            // （SRC# はパイロットの比較で探しなおして止まらなくなっていた）
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName);
            var s = AddSupport(f, u, "サポート");
            f.Src.MDList.Add("サポート").AddMessage("回避", "よけた");
            f.Src.Commands.SelectedSpecialPower = "熱血";
            f.Src.Commands.SelectedPilot = s;

            u.PilotMessage("熱血");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_SpecialPowerOfSupport_UsesSupportMessage()
        {
            // スペシャルパワーのメッセージは、使ったパイロットのメッセージデータで表示する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName);
            var s = AddSupport(f, u, "サポート");
            f.Src.MDList.Add("サポート").AddMessage("熱血", "燃えろ！");
            f.Src.Commands.SelectedSpecialPower = "熱血";
            f.Src.Commands.SelectedPilot = s;

            u.PilotMessage("熱血");

            CollectionAssert.AreEqual(new[] { (s.ID, "燃えろ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SupportChosenByDice_UsesSupportMessage()
        {
            // Dice(CountPilot + CountSupport) が CountPilot より大きければ Support(i - CountPilot) を使う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName);
            var s = AddSupport(f, u, "サポート");
            f.Src.MDList.Add(PName).AddMessage("回避", "メインの回避");
            f.Src.MDList.Add("サポート").AddMessage("回避", "サポートの回避");

            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                // Dice(2) = 2 → サポート
                u.PilotMessage("回避");
            }

            CollectionAssert.AreEqual(new[] { (s.ID, "サポートの回避") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_AdditionalSupportIsNotChosenByDice()
        {
            // 元の SRC はメッセージを話すパイロットを Dice(CountPilot + CountSupport) で選び、追加サポートは候補に入れない。
            // パイロット 1 人と追加サポートなら Dice(1) = 1 でメインパイロットになる
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName, setup: ud => ud.AddFeature("追加サポート=追加"));
            f.Src.PDList.Add("追加").Sex = "男性";
            Assert.IsNotNull(u.AdditionalSupport());
            f.Src.MDList.Add(PName).AddMessage("回避", "メインの回避");
            f.Src.MDList.Add("追加").AddMessage("回避", "追加サポートの回避");

            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                u.PilotMessage("回避");
            }

            CollectionAssert.AreEqual(new[] { (u.MainPilot().ID, "メインの回避") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_MainPilotMessage_CallsSelectMessageTwiceLikeVB()
        {
            // 元の SRC はメインパイロットのメッセージデータを取り出したところで、
            // 結果を使わずに SelectMessage を呼ぶ。SelectMessage は Dice を使うので、乱数を 2 回分進める
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName);
            var md = f.Src.MDList.Add(PName);
            md.AddMessage("回避", "よけた1");
            md.AddMessage("回避", "よけた2");

            var before = GeneralLib.RndIndex;
            md.SelectMessage("回避", u);
            var once = GeneralLib.RndIndex - before;
            Assert.IsTrue(once > 0);

            before = GeneralLib.RndIndex;
            u.PilotMessage("回避");
            Assert.AreEqual(2 * once, GeneralLib.RndIndex - before);
            Assert.AreEqual(1, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_AttackWithUnknownWeapon_TreatedAsShooting()
        {
            // 武器が見つからないとき、元の SRC は範囲外の武器番号で属性を調べる（不定）。
            // SRC# は例外にせず、格闘系でないもの（射撃）として扱う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName);
            f.Src.Stage = "味方";
            f.Src.MDList.Add(PName).AddMessage("射撃", "撃つ！");

            u.PilotMessage("存在しない武器", "攻撃");

            CollectionAssert.AreEqual(new[] { (u.MainPilot().ID, "撃つ！") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // PlayDialog
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayDialog_CancelIsCheckedBeforeSubstitution()
        {
            // 元の SRC は置換の前のメッセージが「-」かで表示を打ち切る。
            // 置換して「-」になったメッセージはそのまま表示する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName, pnickname: "-");
            var dd = new Dialog(f.Src);
            dd.AddMessage(PName, "$(パイロット)");
            dd.AddMessage(PName, "続き");

            u.PlayDialog(dd, "");

            CollectionAssert.AreEqual(new[] { (u.MainPilot().ID, "-"), (u.MainPilot().ID, "続き") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // IsMessageDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMessageDefined_ChecksFirstPilotDialogWithoutAdditionalPilot()
        {
            // 元の SRC の IsMessageDefined は、追加パイロットが無くても pnames(4) = Pilot(1).MessageType のダイアログを調べる。
            // 暴走時パイロットがメインパイロットのとき、1 番目のパイロットのダイアログも定義済みとみなす
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName, setup: ud => ud.AddFeature("暴走時パイロット=暴走"));
            f.Src.PDList.Add("暴走").Sex = "男性";
            var first = u.MainPilot();
            u.AddCondition("暴走", -1);
            Assert.AreNotSame(first, u.MainPilot());
            f.Src.DDList.Add(PName).AddDialog("回避").AddMessage("暴走", "ぐおお");

            Assert.IsTrue(u.IsMessageDefined("回避"));
            Assert.IsFalse(u.IsMessageDefined("破壊"));
        }

        // ──────────────────────────────────────────────
        // SysMessage
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SysMessage_OpensMessageFormIfNotVisible()
        {
            // 元の SRC はメッセージウィンドウが開いていなければ開いてから表示する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.EDList.Add("汎用").AddMessage("回避(解説)", "回避した");

            u.SysMessage("回避");
            u.SysMessage("回避");

            CollectionAssert.AreEqual(
                new[] { "OpenMessageForm", "DisplayBattleMessage", "DisplayBattleMessage" },
                f.Calls);
            Assert.IsNull(f.OpenArgs[0].u1);
            Assert.IsNull(f.OpenArgs[0].u2);
        }

        [TestMethod]
        public void SysMessage_NoTarget_SubstitutesOpponentVariablesWithEmpty()
        {
            // 自分が SelectedUnit で SelectedTarget が無ければ、$(相手…) は空文字列にする
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedTarget = null;
            f.Src.EDList.Add("汎用").AddMessage("回避(解説)", "[$(相手ユニット)][$(相手機体)][$(相手パイロット)][$(相手武器)][$(相手損傷率)]");

            u.SysMessage("回避");

            CollectionAssert.AreEqual(new[] { ("-", "[][][][][]") }, f.Messages);
        }

        [TestMethod]
        public void SysMessage_WeaponOfUnitNotSelected_UsesSelectedTWeapon()
        {
            // 元の SRC の SysMessage の $(武器) は、自分が SelectedUnit でなければ
            // （SelectedTarget でなくても）自分の Weapon(SelectedTWeapon) の名称を使う
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(setup: ud =>
            {
                var w1 = ud.AddWeapon("パンチ");
                w1.MinRange = 1;
                w1.MaxRange = 1;
                var w2 = ud.AddWeapon("ビームライフル(高出力)<試作>");
                w2.MinRange = 1;
                w2.MaxRange = 3;
            });
            var other = f.CreateUnit(uname: "他機", pname: "他人");
            f.Src.Commands.SelectedUnit = other;
            f.Src.Commands.SelectedTarget = null;
            f.Src.Commands.SelectedWeapon = 1;
            f.Src.Commands.SelectedTWeapon = 2;
            f.Src.EDList.Add("汎用").AddMessage("攻撃(解説)", "$(武器)！");

            u.SysMessage("攻撃");

            CollectionAssert.AreEqual(new[] { ("-", "ビームライフル！") }, f.Messages);
        }
    }
}
