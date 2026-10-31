using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.Models;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.TryHealing（回復アビリティの使用）のテスト。
    /// VB 版 COM.bas の TryHealing に対応する（#1404）。
    /// </summary>
    [TestClass]
    public class COMTryHealingTests
    {
        private static COMTestField CreateField()
        {
            var f = new COMTestField();
            f.Src.FileSystem = new LocalFileSystem();
            // 使用イベントを呼ぶためイベントデータを初期化しておく（イベントは空）
            f.Src.Event.InitEventData();
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        private static AbilityData AddHealing(UnitData ud, string name, int level, int maxRange, int en)
        {
            var ad = ud.AddAbility(name);
            ad.SetEffect("回復Lv" + level);
            ad.Class = "";
            ad.MinRange = 1;
            ad.MaxRange = maxRange;
            ad.ENConsumption = en;
            return ad;
        }

        [TestMethod]
        public void TryHealing_FullHealingAbilityBeforeSmallerOne_DoesNotThrow()
        {
            // 全快できるアビリティ（1000 回復）を先に、回復しきれないもの（500 回復）を後に持つ。
            // 元は後のアビリティを比べるときに、対象のユニットの 0 番目のアビリティを参照して落ちていた。
            // VB 版は使うユニットのいま選んでいるアビリティと比べる
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddHealing(ud, "修理ビーム", 2, 3, 30);
            AddHealing(ud, "応急修理", 1, 3, 10);
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 4, 3);
            t.HP = t.MaxHP - 800;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryHealing(false));

            // 全快するアビリティを選んでいるとき、全快しないものの方が EN が少なければそちらに替える（VB 版と同じ）
            Assert.AreEqual("応急修理", f.Src.Commands.SelectedAbilityName);
            Assert.AreEqual(t.MaxHP - 300, t.HP);
        }

        [TestMethod]
        public void TryHealing_UserInAirWithoutEnoughEN_DoesNotMoveToHeal()
        {
            // 空中・宇宙にいて、使ったあとの EN が 5 未満になるなら、移動してまで回復しない。
            // VB 版は使うユニットの地形と EN で判定する（対象のユニットのものではない）
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            ud.Transportation = "陸空";
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddHealing(ud, "応急修理", 1, 1, 10);
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            u.Area = "空中";
            u.EN = 12;
            var t = f.Deploy("機体", "敵B", "敵", 6, 3);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryHealing(false));
            Assert.AreEqual(3, u.x);
            Assert.AreEqual(1000, t.HP);
        }

        [TestMethod]
        public void TryHealing_UserInAirWithEnoughEN_MovesToHeal()
        {
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            ud.Transportation = "陸空";
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddHealing(ud, "応急修理", 1, 1, 10);
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            u.Area = "空中";
            u.EN = 100;
            var t = f.Deploy("機体", "敵B", "敵", 6, 3);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryHealing(false));
            Assert.AreEqual(1, System.Math.Abs(u.x - t.x) + System.Math.Abs(u.y - t.y));
            Assert.AreEqual(1500, t.HP);
        }
    }
}
