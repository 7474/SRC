using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.SyncBullet（VB: Unit.cls SyncBullet）の「他の形態の弾数も変更」を元の SRC に合わせたことのテスト（7474/SRC#1502）。
    /// 元の SRC は他の形態の武器・アビリティを、直前に合わせたものの次から探す。
    /// </summary>
    [TestClass]
    public class UnitSyncBulletVBMatchTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Map.MapWidth = 10;
            src.Map.MapHeight = 10;
            return src;
        }

        private static UnitData AddUnitData(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            for (var i = 0; i < 2; i++)
            {
                var wd = ud.AddWeapon("ミサイル");
                wd.Power = 1000;
                wd.Class = "射";
                wd.MinRange = 1;
                wd.MaxRange = 3;
                wd.Bullet = 4;
            }
            for (var i = 0; i < 2; i++)
            {
                var ad = ud.AddAbility("修理");
                ad.Class = "";
                ad.Stock = 4;
            }
            return ud;
        }

        private static Unit CreateUnit(SRC src, UnitData ud)
        {
            var u = src.UList.Add(ud.Name, 0, "味方");
            u.Update();
            u.FullRecover();
            return u;
        }

        // 同名の武器・アビリティが 2 つずつあるとき、1 つ目同士・2 つ目同士を合わせる
        [TestMethod]
        public void SyncBullet_OtherForm_MatchesDuplicatedNamesInOrder()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, AddUnitData(src, "変形機"));
            var form = CreateUnit(src, AddUnitData(src, "変形機改"));
            u.AddOtherForm(form);
            u.Weapons[1].SetBullet(1);
            u.Abilities[1].SetStock(2);

            u.SyncBullet();

            Assert.AreEqual(4, form.Weapons[0].Bullet());
            Assert.AreEqual(1, form.Weapons[1].Bullet());
            Assert.AreEqual(4, form.Abilities[0].Stock());
            Assert.AreEqual(2, form.Abilities[1].Stock());
        }
    }
}
