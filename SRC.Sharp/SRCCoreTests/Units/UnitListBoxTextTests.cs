using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 武器選択・アビリティ選択のリストボックスの表示内容（GUI.bas の WeaponListBox・AbilityListBox）のテスト
    /// （7474/SRC#1483・#1484）
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

        private static Unit CreateAbilityUnit(SRC src)
        {
            return CreateUnit(src, ud =>
            {
                var a1 = ud.AddAbility("修理装置");
                a1.SetEffect("回復Lv2");
                a1.MinRange = 1;
                a1.MaxRange = 1;
                a1.ENConsumption = 10;

                var a2 = ud.AddAbility("激励(説明付き)");
                a2.SetEffect("解説=味方ユニット１体の気力を大きく増加させ、さらに攻撃力を上げる");
                a2.MinRange = 1;
                a2.MaxRange = 3;
                a2.Stock = 2;
                a2.NecessaryMorale = 120;
                a2.Class = "Ｐ";

                var a3 = ud.AddAbility("鼓舞");
                a3.SetEffect("状態Lv3=攻撃力ＵＰ 状態Lv3=防御力ＵＰ");
            });
        }

        [TestMethod]
        public void AbilityListBox_ColumnsAreSameAsVB()
        {
            var src = CreateSrc();
            var u = CreateAbilityUnit(src);
            var list = new UnitAbilityList(AbilityListMode.List, u);

            var items = list.ToListBoxItems();

            // 効果の解説が 32 桁を超えると、残りを次の行に表示する（次の行も同じアビリティ）
            CollectionAssert.AreEqual(new[]
            {
                "修理装置             ＨＰを1000回復                     1  -  10 ",
                "激励                 味方ユニット１体の気力を大きく   1-3  2   - 気120 Ｐ",
                "                     増加させ、さらに攻撃力を上げる",
                "鼓舞                 攻撃力ＵＰ、防御力ＵＰ(3ターン)    -  -   - ",
            }, items.Select(x => x.Text).ToArray());
            CollectionAssert.AreEqual(new[] { "修理装置", "激励(説明付き)", "激励(説明付き)", "鼓舞" },
                items.Select(x => x.ListItemObject.Data.Name).ToArray());
            Assert.AreEqual("名称                 効果                            射程 数  EN 分類", list.ListBoxInfo(src));
        }

        [TestMethod]
        public void EffectName_SameAsVB()
        {
            var src = CreateSrc();
            CreateUnit(src, ud => { });
            var ad = new AbilityData(src);
            ad.SetEffect("回復Lv-1 補給Lv1 強化Lv2=超感覚Lv1 付加Lv2=格闘武器 状態=攻撃力ＵＰ 治癒 再行動");

            CollectionAssert.AreEqual(new[]
            {
                "ＨＰを500減少",
                "ＥＮを50回復",
                "超感覚Lv1強化(2ターン)",
                "付加(2ターン)",
                "攻撃力ＵＰ",
                "状態回復",
                "行動非消費",
            }, ad.Effects.Select(x => ad.EffectName(x)).ToArray());
        }
    }
}
