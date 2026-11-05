using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.TryMapAttack（マップ攻撃の使用）と COM.TrySpecialPower のテスト。
    /// VB 版 COM.bas の TryMapAttack・TrySpecialPower に対応する。
    /// </summary>
    [TestClass]
    public class COMTryMapAttackTests
    {
        private static COMTestField CreateField()
        {
            var f = new COMTestField();
            // マップ攻撃は自軍のフェイズでしか使えない
            f.Src.Stage = "敵";
            f.Src.FileSystem = new LocalFileSystem();
            f.Src.Event.InitEventData();
            // 1 番目: 移動後にも使えるマップ攻撃（最強武器ではない）
            // 2 番目: 移動前にだけ使える通常武器（射程 2 以上でＰが無い）
            var ud = f.DefineUnit("マップ兵器機");
            COMTestField.AddWeapon(ud, "マップ兵器", 1500, minRange: 1, maxRange: 2, precision: 100, wclass: "Ｍ全Ｐ");
            COMTestField.AddWeapon(ud, "ビーム", 1200, minRange: 1, maxRange: 3, wclass: "射");
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        [TestMethod]
        public void TryMapAttack_Moved_ScoreLimitUsesBeforeMoveAvailability()
        {
            // VB 版は移動後に呼ばれたときも「移動前」で通常攻撃が使えるかを見て、
            // 使えればマップ攻撃を使うターゲット数の下限を 2 にする。
            // 通常攻撃は移動後には使えないが、敵が 1 体だけならマップ攻撃は使わない。
            var f = CreateField();
            var u = f.Deploy("マップ兵器機", "敵A", "敵", 5, 5);
            var t = f.Deploy("機体", "味方A", "味方", 5, 6);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryMapAttack(true));
            Assert.AreEqual(3000, t.HP);
        }

        [TestMethod]
        public void TryMapAttack_Moved_TwoTargets_UsesMapAttack()
        {
            // ターゲットが 2 体いれば下限を満たすのでマップ攻撃を使う
            var f = CreateField();
            var u = f.Deploy("マップ兵器機", "敵A", "敵", 5, 5);
            var t1 = f.Deploy("機体", "味方A", "味方", 5, 6);
            var t2 = f.Deploy("機体", "味方B", "味方", 6, 5);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryMapAttack(true));
            Assert.IsTrue(t1.HP < 3000);
            Assert.IsTrue(t2.HP < 3000);
        }

        [TestMethod]
        public void TrySpecialPower_SetsSelectedPilotEvenForZakoPilot()
        {
            // VB 版は最初に SelectedPilot を設定してから、ザコパイロットかを判定する
            var f = CreateField();
            f.DefinePilot("雑魚(ザコ)");
            var u = f.Deploy("機体", "雑魚(ザコ)", "敵", 5, 5);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedPilot = null;

            f.Src.COM.TrySpecialPower(u.MainPilot());

            Assert.AreSame(u.MainPilot(), f.Src.Commands.SelectedPilot);
        }
    }
}
