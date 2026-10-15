using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static SRCCore.Coverage.Tests.UnitItem.UnitItemTestHelper;

namespace SRCCore.Coverage.Tests.UnitItem
{
    /// <summary>
    /// Unit/Item のアイテム処理のうち、レビューで指摘された分岐のテスト
    /// （元の SRC の Unit.cls AddItem/DeleteItem/IsAbleToEquip、Item.cls IsAvailable）
    /// </summary>
    [TestClass]
    public class UnitItemReviewTests
    {
        private static string[] Names(Units.Unit u)
        {
            return u.ItemList.Select(x => x.Name).ToArray();
        }

        // ──────────────────────────────────────────────
        // AddItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddItem_OnlyHardpointItems_RemovesFirstItem()
        {
            // アイテム枠が埋まっていて、装備中のものが全てハードポイント付きなら先頭のアイテムを外す
            // （Unit.cls AddItem「ハードポイント付きのものしかない場合」）
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 2);
            var mountA = CreateItem(src, "武器ラックＡ", "アイテム", "汎用", "ハードポイント=武器");
            var mountB = CreateItem(src, "武器ラックＢ", "アイテム", "汎用", "ハードポイント=武器");
            var plain = CreateItem(src, "お守り", "アイテム");
            u.AddItem(mountA);
            u.AddItem(mountB);
            u.AddItem(plain);

            CollectionAssert.AreEqual(new[] { "武器ラックＢ", "お守り" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(mountA.Unit);
        }

        [TestMethod]
        public void AddItem_TwoHanded_RemovesOnlyFirstHandItem()
        {
            // 両手武器は手に持っている最初のアイテムだけを外す（Exit For）。片手武器と盾なら盾は残る
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕");
            var sword = CreateItem(src, "剣", "片手");
            var shield = CreateItem(src, "盾", "盾");
            u.AddItem(sword);
            u.AddItem(shield);
            u.AddItem(CreateItem(src, "大剣", "両手"));

            CollectionAssert.AreEqual(new[] { "盾", "大剣" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(sword.Unit);
            // 味方ユニットから外したアイテムは在庫に残る
            Assert.IsTrue(sword.Exist);
        }

        [TestMethod]
        public void AddItem_TwoHanded_NonPlayerUnit_DisplacedItemIsDestroyed()
        {
            // 味方以外のユニットで両手武器に押し出された手持ちアイテムは消滅する（Exist=False）
            var src = CreateSRC();
            var u = CreateUnit(src, "敵戦士", "汎用", 4, "敵", "装備個所=腕");
            var sword = CreateItem(src, "剣", "片手");
            u.AddItem(sword);
            u.AddItem(CreateItem(src, "大剣", "両手"));

            CollectionAssert.AreEqual(new[] { "大剣" }, Names(u), string.Join(",", Names(u)));
            Assert.IsFalse(sword.Exist);
        }

        [TestMethod]
        public void AddItem_DiscardedUnit_DoesNotMarkItemExist()
        {
            // 破棄されたユニットに装備したアイテムは存在扱いにしない（Status <> "破棄" の時のみ Exist=True）
            var src = CreateSRC();
            var u = CreateUnit(src, "廃棄機");
            u.Status = "破棄";
            var itm = CreateItem(src, "部品", "アイテム");
            itm.Exist = false;
            u.AddItem(itm);

            Assert.AreSame(u, itm.Unit);
            Assert.IsFalse(itm.Exist);
        }

        // ──────────────────────────────────────────────
        // DeleteItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteItem_LosingWeaponHardpoint_RemovesItemsThatNoLongerFit()
        {
            // 武器用ハードポイントを付加するアイテムを外すと、減ったスロットに収まらない武器を後ろから外す
            // （Unit.cls DeleteItem「ハードポイントが減少？」）
            var src = CreateSRC();
            var u = CreateUnit(src, "武装機", "汎用", 4, "味方", "ハードポイント=武器");
            var rack = CreateItem(src, "増設ラック", "アイテム", "汎用", "ハードポイントLv2=武器");
            u.AddItem(rack);
            Assert.AreEqual(3, u.ItemSlotSize("武器"));
            var a = CreateItem(src, "銃Ａ", "武器");
            var b = CreateItem(src, "銃Ｂ", "武器");
            var c = CreateItem(src, "銃Ｃ", "武器");
            u.AddItem(a);
            u.AddItem(b);
            u.AddItem(c);
            CollectionAssert.AreEqual(new[] { "増設ラック", "銃Ａ", "銃Ｂ", "銃Ｃ" }, Names(u), string.Join(",", Names(u)));

            u.DeleteItem(rack);
            Assert.AreEqual(1, u.ItemSlotSize("武器"));
            CollectionAssert.AreEqual(new[] { "銃Ａ" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(b.Unit);
            Assert.IsNull(c.Unit);
        }

        // ──────────────────────────────────────────────
        // Item.IsAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAvailable_NecessarySkill_SelfEquipReferenceByClass()
        {
            // 必要技能「<クラス>装備」も自分自身を指すものとみなし、装備中なら使用可能（Item.cls IsAvailable）
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            RidePilot(src, u, "パイロット", "男性");
            var itm = CreateItem(src, "光の珠", "アイテム", "宝珠", "必要技能=宝珠装備");
            var other = CreateItem(src, "闇の珠", "アイテム", "汎用", "必要技能=宝珠装備");
            u.AddItem0(itm);
            u.AddItem0(other);
            itm.Activated = false;

            Assert.IsTrue(itm.IsAvailable(u));
            // クラスも名前も一致しない他のアイテムは自己参照にならない
            Assert.IsFalse(other.IsAvailable(u));
        }

        [TestMethod]
        public void IsAvailable_Armor_WithoutArmorProficiency_OnlyGeneralOrFixed()
        {
            // 防具クラスを持たないユニットでも、クラス「汎用」「固定」の防具は使用可能
            var src = CreateSRC();
            var u = CreateUnit(src, "素人", "汎用", 4, "味方", "装備個所=腕 体 頭");
            Assert.IsTrue(CreateItem(src, "布の服", "体", "汎用").IsAvailable(u));
            Assert.IsTrue(CreateItem(src, "呪いの兜", "頭", "固定").IsAvailable(u));
            Assert.IsTrue(CreateItem(src, "木の盾", "盾", "汎用").IsAvailable(u));
            Assert.IsFalse(CreateItem(src, "鉄の鎧", "体", "鎧").IsAvailable(u));
            Assert.IsFalse(CreateItem(src, "鉄の盾", "盾", "盾").IsAvailable(u));
        }

        // ──────────────────────────────────────────────
        // IsAbleToEquip: 愛称による専用指定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAbleToEquip_ExclusiveByUnitNickname()
        {
            // 専用指定はユニット名だけでなくユニットの愛称でも判定する（武器・防具・アイテム）
            var src = CreateSRC();
            var u = CreateUnit(src, "RX-78-2", "(ＭＳ)", 4, "味方", "装備個所=腕 体", "武器クラス=銃", "防具クラス=装甲");
            u.Data.Nickname = "ガンダム";
            Assert.AreEqual("ガンダム", u.Nickname);

            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "専用ライフル", "片手", "銃(ガンダム専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "専用装甲", "体", "装甲(ガンダム専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "専用パーツ", "アイテム", "部品(ガンダム専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "専用パック", "背中", "背負い物(ガンダム専用)")));
            // 愛称も名前も一致しない専用指定は不可
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "他機ライフル", "片手", "銃(ザク専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "他機パーツ", "アイテム", "部品(ザク専用)")));
        }
    }
}
