using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 攻撃の武器選択リストの援護攻撃の使用・不使用を切り替える行（GUI.bas の WeaponListBox）のテスト
    /// （7474/SRC#1489）
    /// </summary>
    [TestClass]
    public class UnitWeaponListSupportAttackTests
    {
        private const string Rifle = "ライフル, 1500, 1, 3, +0, -, -, -, AAAA, +0, 射";
        private const string Punch = "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突";

        private static UnitLookupTestHelper CreateWorld()
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク", skills: "援護Lv1, 1");
            h.DefineUnit("自機", weapons: Punch + "\n" + Rifle);
            h.DefineUnit("僚機", weapons: Rifle);
            return h;
        }

        // リストボックスの代わりに、表示された内容を記録して answers の順に選ぶ
        private static (List<string[]> shown, System.Func<ListBoxArgs, int> listBox) Recorder(params int[] answers)
        {
            var shown = new List<string[]>();
            var queue = new Queue<int>(answers);
            return (shown, args =>
            {
                shown.Add(args.Items.Select(x => x.Text).ToArray());
                return queue.Dequeue();
            });
        }

        [TestMethod]
        public void SupportAttackItem_IsAddedAndToggled()
        {
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            h.Src.Commands.UseSupportAttack = false;
            var weapons = new UnitWeaponList(WeaponListMode.BeforeMove, me);
            // 援護攻撃の行（3 行目）を 2 回選んでから、パンチを選ぶ
            var (shown, listBox) = Recorder(3, 3, 3, 1);

            var selected = weapons.SelectWeapon(h.Src, "武器選択", listBox);

            Assert.AreEqual("パンチ", selected.Name);
            Assert.AreEqual(4, shown.Count);
            CollectionAssert.AreEqual(new[] { "援護攻撃：使用する", "援護攻撃：使用しない", "援護攻撃：使用する", "援護攻撃：使用しない" },
                shown.Select(x => x.Last()).ToArray());
            Assert.IsTrue(shown.All(x => x.Length == 3));
            Assert.IsFalse(h.Src.Commands.UseSupportAttack);
        }

        [TestMethod]
        public void SupportAttackItem_DefaultIsUse()
        {
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            h.Src.Commands.UseSupportAttack = false;
            var (shown, listBox) = Recorder(2);

            var selected = new UnitWeaponList(WeaponListMode.AfterMove, me).SelectWeapon(h.Src, "武器選択", listBox);

            Assert.AreEqual("ライフル", selected.Name);
            Assert.AreEqual("援護攻撃：使用する", shown.Single().Last());
            Assert.IsTrue(h.Src.Commands.UseSupportAttack);
        }

        [TestMethod]
        public void SupportAttackItem_NotAddedWithoutSupporter()
        {
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            // 隣にいなければ援護攻撃の行は出さない
            h.Place("僚機", "ベルク", 6, 3);
            var (shown, listBox) = Recorder(0);

            var selected = new UnitWeaponList(WeaponListMode.BeforeMove, me).SelectWeapon(h.Src, "武器選択", listBox);

            Assert.IsNull(selected);
            Assert.AreEqual(2, shown.Single().Length);
        }

        [TestMethod]
        public void SupportAttackItem_NotAddedInList()
        {
            // 武装一覧では援護攻撃の行は出さない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var (shown, listBox) = Recorder(0);

            new UnitWeaponList(WeaponListMode.List, me).SelectWeapon(h.Src, "武装一覧", listBox);

            Assert.AreEqual(2, shown.Single().Length);
        }

        [TestMethod]
        public void Cancel_ReturnsNull()
        {
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var (_, listBox) = Recorder(3, 0);

            Assert.IsNull(new UnitWeaponList(WeaponListMode.BeforeMove, me).SelectWeapon(h.Src, "武器選択", listBox));
        }
    }
}
