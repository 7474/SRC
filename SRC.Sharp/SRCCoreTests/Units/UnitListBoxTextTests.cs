using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 武器選択のリストボックスの表示内容（GUI.bas の WeaponListBox）のテスト（7474/SRC#1483）
    /// </summary>
    [TestClass]
    public class UnitListBoxTextTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static Unit CreateUnit(SRC src, System.Action<UnitData> setup)
        {
            var pd = src.PDList.Add("テストパイロット");
            pd.SP = 10;
            var p = src.PList.Add("テストパイロット", 1, "味方");
            var ud = src.UDList.Add("テスト機");
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            setup(ud);
            var u = src.UList.Add("テスト機", 0, "味方");
            p.Ride(u);
            u.Update();
            src.Commands.SelectedUnit = u;
            return u;
        }

        private static void AddWeapon(UnitData ud, string name, int power, int minRange, int maxRange,
            int precision, int bullet, int en, int critical, string adaption, int morale, string wclass)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.MinRange = minRange;
            wd.MaxRange = maxRange;
            wd.Precision = precision;
            wd.Bullet = bullet;
            wd.ENConsumption = en;
            wd.Critical = critical;
            wd.Adaption = adaption;
            wd.NecessaryMorale = morale;
            wd.Class = wclass;
        }

        private static Unit CreateWeaponUnit(SRC src)
        {
            return CreateUnit(src, ud =>
            {
                // 攻撃力の順に並べ替わることを確かめるため、攻撃力の大きいものから定義する
                AddWeapon(ud, "必殺剣(愛称テスト)", 12000, 1, 1, -10, 1, 0, -5, "A-AA", 0, "武");
                AddWeapon(ud, "バルカン", 800, 1, 1, 10, 20, 0, 0, "AAAA", 0, "実");
                AddWeapon(ud, "ビームライフル", 1500, 1, 4, 0, 0, 10, 10, "AABA", 110, "Ｂ");
            });
        }

        [TestMethod]
        public void WeaponListBox_List_ColumnsAndOrderAreSameAsVB()
        {
            var src = CreateSrc();
            var u = CreateWeaponUnit(src);
            var list = new UnitWeaponList(WeaponListMode.List, u);

            var items = list.ToListBoxItems(src);

            // 攻撃力の小さい順。名称は愛称、射程・必要気力を表示し、弾数制・ＥＮ消費制でない武器は「-」、
            // 命中・CT の修正値には「+」を付ける
            CollectionAssert.AreEqual(new[]
            {
                "バルカン                    800    1 +10 20   -  +0 AAAA 実",
                "ビームライフル             1500  1-4  +0  -  10 +10 AABA 気110 Ｂ",
                "必殺剣                    12000    1 -10  1   -  -5 A-AA 武",
            }, items.Select(x => x.Text).ToArray());
            CollectionAssert.AreEqual(new[] { "バルカン", "ビームライフル", "必殺剣(愛称テスト)" },
                items.Select(x => x.ListItemObject.Name).ToArray());
            Assert.AreEqual("名称                       攻撃 射程  命 弾  EN  CT 適応 分類", list.ListBoxInfo(src));
        }

        [TestMethod]
        public void WeaponListBox_Counter_ColumnsAreSameAsVB()
        {
            var src = CreateSrc();
            var u = CreateWeaponUnit(src);
            // 予測命中率非表示のときは命中・CT の修正値を表示する
            src.Expression.DefineGlobalVariable("Option(予測命中率非表示)");

            var text = UnitWeaponList.CounterListBoxText(src, u.Weapons.First(x => x.Name == "ビームライフル"), null);

            Assert.AreEqual("ビームライフル               1500   +0  +10  -  10 AABA 気110 Ｂ", text);
            var counter = new UnitWeaponList(WeaponListMode.List, u) { Mode = WeaponListMode.Counter };
            Assert.AreEqual("名称                         攻撃 命中 CT   弾  EN 適応 分類", counter.ListBoxInfo(src));
        }

        [TestMethod]
        public void WeaponListBox_HeaderUsesShortTerm()
        {
            var src = CreateSrc();
            var u = CreateWeaponUnit(src);
            src.Expression.DefineGlobalVariable("ShortTerm(EN)");
            src.Expression.SetVariableAsString("ShortTerm(EN)", "MP");

            Assert.AreEqual("名称                       攻撃 射程  命 弾  MP  CT 適応 分類",
                new UnitWeaponList(WeaponListMode.List, u).ListBoxInfo(src));
        }
    }
}
