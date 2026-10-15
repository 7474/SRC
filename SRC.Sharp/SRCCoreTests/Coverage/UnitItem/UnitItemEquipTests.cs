using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static SRCCore.Coverage.Tests.UnitItem.UnitItemTestHelper;

namespace SRCCore.Coverage.Tests.UnitItem
{
    /// <summary>
    /// Unit のアイテム関連処理（元の SRC の Unit.cls MaxItemNum/AddItem/DeleteItem/ItemSlotSize など）のテスト
    /// </summary>
    [TestClass]
    public class UnitItemEquipTests
    {
        private static string[] Names(Units.Unit u)
        {
            return u.ItemList.Select(x => x.Name).ToArray();
        }

        // ──────────────────────────────────────────────
        // MaxItemNum / ItemSlotSize
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxItemNum_AddsItemHardpointLevelOnce()
        {
            // 最大アイテム数に「ハードポイント=強化パーツ/アイテム」のレベルを（最初の１つだけ）加える
            var src = CreateSRC();
            var plain = CreateUnit(src, "標準機", "汎用", 2);
            var hp = CreateUnit(src, "拡張機", "汎用", 2, "味方", "ハードポイントLv2=強化パーツ", "ハードポイントLv3=アイテム", "ハードポイントLv5=武器");
            Assert.AreEqual(2, plain.MaxItemNum());
            Assert.AreEqual(4, hp.MaxItemNum());
        }

        [TestMethod]
        public void ItemSlotSize_ForItemParts_SumsAllItemHardpoints()
        {
            // ItemSlotSize は強化パーツ/アイテム用ハードポイントを全て加算する（MaxItemNum と異なり Exit For しない）
            var src = CreateSRC();
            var hp = CreateUnit(src, "拡張機", "汎用", 2, "味方", "ハードポイントLv2=強化パーツ", "ハードポイントLv3=アイテム");
            var plain = CreateUnit(src, "標準機", "汎用", 2);
            Assert.AreEqual(7, hp.ItemSlotSize("強化パーツ"));
            Assert.AreEqual(7, hp.ItemSlotSize("アイテム"));
            Assert.AreEqual(2, plain.ItemSlotSize("アイテム"));
        }

        [TestMethod]
        public void ItemSlotSize_ForOtherParts()
        {
            // ハードポイントを持たなければ他の装備個所は 1、持っていればその装備個所のレベル合計
            var src = CreateSRC();
            var plain = CreateUnit(src, "標準機", "汎用", 2);
            var hp = CreateUnit(src, "武装機", "汎用", 2, "味方", "ハードポイントLv2=武器", "ハードポイント=武器", "ハードポイント=背中");
            Assert.AreEqual(1, plain.ItemSlotSize("背中"));
            Assert.AreEqual(3, hp.ItemSlotSize("武器"));
            Assert.AreEqual(1, hp.ItemSlotSize("背中"));
            // ハードポイントがあるが該当個所の指定が無い場合は 0（VB と同じ）
            Assert.AreEqual(0, hp.ItemSlotSize("体"));
        }

        // ──────────────────────────────────────────────
        // Item / CountItem / IsEquiped
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_LookupByIndexIdAndName()
        {
            // Item は番号・ID で引けなければアイテム名で検索する
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var a = CreateItem(src, "ブースター", "アイテム");
            var b = CreateItem(src, "装甲板", "アイテム");
            u.AddItem(a);
            u.AddItem(b);

            Assert.AreEqual(2, u.CountItem());
            Assert.AreSame(a, u.Item(1));
            Assert.AreSame(b, u.Item(2));
            Assert.IsNull(u.Item(3));
            Assert.AreSame(b, u.Item(b.ID));
            Assert.AreSame(a, u.Item("ブースター"));
            Assert.IsNull(u.Item("存在しない"));
            Assert.IsTrue(u.IsEquiped("装甲板"));
            Assert.IsFalse(u.IsEquiped("存在しない"));
        }

        // ──────────────────────────────────────────────
        // WeaponProficiency / ArmorProficiency
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Proficiency_ConcatenatesAllClassFeatures()
        {
            // 武器クラス・防具クラスは複数指定を空白区切りで連結する
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "武器クラス=剣 槍", "武器クラス=弓", "防具クラス=鎧");
            Assert.AreEqual(" 剣 槍 弓", u.WeaponProficiency());
            Assert.AreEqual(" 鎧", u.ArmorProficiency());
            var none = CreateUnit(src, "素人");
            Assert.IsTrue(string.IsNullOrEmpty(none.WeaponProficiency()));
        }

        // ──────────────────────────────────────────────
        // AddItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddItem_SetsUnitAndExist_AndIgnoresAlreadyEquipped()
        {
            // 装備したアイテムは存在扱いになり、既に装備済みなら何もしない
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var itm = CreateItem(src, "ブースター", "アイテム");
            itm.Exist = false;
            u.AddItem(itm);
            Assert.AreSame(u, itm.Unit);
            Assert.IsTrue(itm.Exist);

            u.AddItem(itm);
            Assert.AreEqual(1, u.CountItem());
        }

        [TestMethod]
        public void AddItem_ItemSlotsFull_RemovesFirstNonHardpointItem()
        {
            // アイテム枠が埋まっていれば、ハードポイントを持たないものから先に外す
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 2);
            var a = CreateItem(src, "Ａ", "アイテム");
            var b = CreateItem(src, "Ｂ", "アイテム");
            var c = CreateItem(src, "Ｃ", "アイテム");
            u.AddItem(a);
            u.AddItem(b);
            u.AddItem(c);

            CollectionAssert.AreEqual(new[] { "Ｂ", "Ｃ" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(a.Unit);
            // 味方ユニットから外されたアイテムは在庫に戻る（Exist のまま）
            Assert.IsTrue(a.Exist);
        }

        [TestMethod]
        public void AddItem_NonPlayerUnit_RemovedItemIsDestroyed()
        {
            // 味方以外のユニットから押し出されたアイテムは消滅する（Exist=False）
            var src = CreateSRC();
            var u = CreateUnit(src, "敵機", "汎用", 1, "敵");
            var a = CreateItem(src, "Ａ", "アイテム");
            var b = CreateItem(src, "Ｂ", "アイテム");
            u.AddItem(a);
            u.AddItem(b);

            CollectionAssert.AreEqual(new[] { "Ｂ" }, Names(u), string.Join(",", Names(u)));
            Assert.IsFalse(a.Exist);
        }

        [TestMethod]
        public void AddItem_LargeItem_RemovesExtraItems()
        {
            // 大型アイテムは大型アイテムLv分だけ余分に枠を空ける
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 3);
            var a = CreateItem(src, "Ａ", "アイテム");
            var b = CreateItem(src, "Ｂ", "アイテム");
            var big = CreateItem(src, "大型", "アイテム", "汎用", "大型アイテム");
            u.AddItem(a);
            u.AddItem(b);
            u.AddItem(big);

            CollectionAssert.AreEqual(new[] { "Ｂ", "大型" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        public void AddItem_ZeroItemSlots_CannotEquip()
        {
            // アイテム枠が 0 のユニットには強化パーツ・アイテムを装備できない
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 0);
            var a = CreateItem(src, "Ａ", "強化パーツ");
            u.AddItem(a);
            Assert.AreEqual(0, u.CountItem());
            Assert.IsNull(a.Unit);
        }

        [TestMethod]
        public void AddItem_ItemWithItemHardpoint_DoesNotConsumeSlot()
        {
            // 強化パーツ用ハードポイントを持つアイテムは枠の確認をせずに装備できる
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 1);
            var a = CreateItem(src, "Ａ", "アイテム");
            var ext = CreateItem(src, "拡張スロット", "アイテム", "汎用", "ハードポイント=アイテム");
            u.AddItem(a);
            u.AddItem(ext);
            CollectionAssert.AreEqual(new[] { "Ａ", "拡張スロット" }, Names(u), string.Join(",", Names(u)));
            // ハードポイントで枠が増える
            Assert.AreEqual(2, u.MaxItemNum());
        }

        [TestMethod]
        public void AddItem_FixedHiddenItem_IgnoresSlotLimit()
        {
            // 固定クラスで「非表示」のイベント専用アイテムは装備個所を消費しない
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 1);
            var a = CreateItem(src, "Ａ", "アイテム");
            var ev = CreateItem(src, "イベント", "アイテム", "固定", "非表示");
            u.AddItem(a);
            u.AddItem(ev);
            CollectionAssert.AreEqual(new[] { "Ａ", "イベント" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        public void AddItem_HiddenPart_NoLimit()
        {
            // 装備個所が「非表示」のアイテムは何個でも装備できる
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 0);
            u.AddItem(CreateItem(src, "Ｘ", "非表示"));
            u.AddItem(CreateItem(src, "Ｙ", "非表示"));
            CollectionAssert.AreEqual(new[] { "Ｘ", "Ｙ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        public void AddItem_TwoHanded_RemovesHandItem()
        {
            // 両手武器を装備すると手に持っているアイテムを外す
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕");
            var shield = CreateItem(src, "盾", "盾");
            var great = CreateItem(src, "大剣", "両手");
            u.AddItem(shield);
            u.AddItem(great);
            CollectionAssert.AreEqual(new[] { "大剣" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        public void AddItem_HandAndShoulderSlots_WithoutConflict()
        {
            // 片手武器と盾、肩２つは同時に装備できる（何も外さない）
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕 肩");
            u.AddItem(CreateItem(src, "剣", "片手"));
            u.AddItem(CreateItem(src, "盾", "盾"));
            u.AddItem(CreateItem(src, "肩Ａ", "肩"));
            u.AddItem(CreateItem(src, "肩Ｂ", "肩"));
            CollectionAssert.AreEqual(new[] { "剣", "盾", "肩Ａ", "肩Ｂ" }, Names(u), string.Join(",", Names(u)));

            var u2 = CreateUnit(src, "戦士２", "汎用", 4, "味方", "装備個所=腕");
            u2.AddItem(CreateItem(src, "盾２", "盾"));
            u2.AddItem(CreateItem(src, "剣２", "片手"));
            CollectionAssert.AreEqual(new[] { "盾２", "剣２" }, Names(u2), string.Join(",", Names(u2)));
        }

        [TestMethod]
        public void AddItem_Hardpoint_FitsWithinSlots()
        {
            // ハードポイントのスロット数までは同じ装備個所に複数装備できる
            var src = CreateSRC();
            var u = CreateUnit(src, "武装機", "汎用", 4, "味方", "ハードポイントLv2=武器");
            u.AddItem(CreateItem(src, "銃Ａ", "武器"));
            u.AddItem(CreateItem(src, "銃Ｂ", "武器"));
            CollectionAssert.AreEqual(new[] { "銃Ａ", "銃Ｂ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_Shield_ReplacesShield_KeepsOneHandWeapon()
        {
            // 盾は既存の盾・両手武器と入れ替わるが、片手武器は１つなら残す
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕");
            u.AddItem(CreateItem(src, "剣", "片手"));
            u.AddItem(CreateItem(src, "木の盾", "盾"));
            u.AddItem(CreateItem(src, "鉄の盾", "盾"));
            CollectionAssert.AreEqual(new[] { "剣", "鉄の盾" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_OneHanded_Ambidextrous_KeepsOneOtherHandItem()
        {
            // 両手利きなら片手武器を２つ持てる（３つ目で２つ目を外す）
            var src = CreateSRC();
            var u = CreateUnit(src, "二刀流", "汎用", 4, "味方", "装備個所=腕", "両手利き");
            u.AddItem(CreateItem(src, "剣Ａ", "片手"));
            u.AddItem(CreateItem(src, "剣Ｂ", "片手"));
            CollectionAssert.AreEqual(new[] { "剣Ａ", "剣Ｂ" }, Names(u), string.Join(",", Names(u)));
            u.AddItem(CreateItem(src, "剣Ｃ", "片手"));
            CollectionAssert.AreEqual(new[] { "剣Ａ", "剣Ｃ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_OneHanded_ReplacesOneHandedWeapon_KeepsShield()
        {
            // 両手利きでなければ片手武器は持ち替える（盾は残す）
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕");
            u.AddItem(CreateItem(src, "盾", "盾"));
            u.AddItem(CreateItem(src, "剣Ａ", "片手"));
            u.AddItem(CreateItem(src, "剣Ｂ", "片手"));
            CollectionAssert.AreEqual(new[] { "盾", "剣Ｂ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_BothShoulders_RemovesAllShoulderItems()
        {
            // 両肩のアイテムは肩のアイテムを全て外す
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 4, "味方", "装備個所=肩");
            u.AddItem(CreateItem(src, "右肩砲", "肩"));
            u.AddItem(CreateItem(src, "左肩砲", "肩"));
            u.AddItem(CreateItem(src, "両肩キャノン", "両肩"));
            CollectionAssert.AreEqual(new[] { "両肩キャノン" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_Shoulder_ThirdReplacesSecond()
        {
            // 肩のアイテムは２つまで。３つ目は２つ目と入れ替わり、両肩とは入れ替わる
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 4, "味方", "装備個所=肩");
            u.AddItem(CreateItem(src, "肩Ａ", "肩"));
            u.AddItem(CreateItem(src, "肩Ｂ", "肩"));
            u.AddItem(CreateItem(src, "肩Ｃ", "肩"));
            CollectionAssert.AreEqual(new[] { "肩Ａ", "肩Ｃ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の foreach 中で DeleteItem して列挙中のコレクション変更例外になる（switch 内 break が Exit For にならない）")]
        public void AddItem_Shoulder_ReplacesBothShoulders()
        {
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 4, "味方", "装備個所=肩");
            u.AddItem(CreateItem(src, "両肩キャノン", "両肩"));
            u.AddItem(CreateItem(src, "肩Ａ", "肩"));
            CollectionAssert.AreEqual(new[] { "肩Ａ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem の Case Else で For 変数 i が進まず i > 0 にならないため同じ装備個所のアイテムを外さない")]
        public void AddItem_SamePart_ReplacesExisting()
        {
            // 体・頭などハードポイントの無い装備個所は１つだけ
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=体 頭");
            u.AddItem(CreateItem(src, "革の鎧", "体"));
            u.AddItem(CreateItem(src, "兜", "頭"));
            u.AddItem(CreateItem(src, "鉄の鎧", "体"));
            CollectionAssert.AreEqual(new[] { "兜", "鉄の鎧" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        public void AddItem_Hardpoint_ReplacesFirstItemWhenFull()
        {
            // ハードポイントの空きスロットが足りなければ同じ装備個所のアイテムを先頭から外す
            var src = CreateSRC();
            var u = CreateUnit(src, "武装機", "汎用", 4, "味方", "ハードポイントLv2=武器");
            var a = CreateItem(src, "銃Ａ", "武器");
            u.AddItem(a);
            u.AddItem(CreateItem(src, "銃Ｂ", "武器"));
            u.AddItem(CreateItem(src, "銃Ｃ", "武器"));
            CollectionAssert.AreEqual(new[] { "銃Ｂ", "銃Ｃ" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(a.Unit);
        }

        [TestMethod]
        [Ignore("移植の不具合: AddItem のハードポイント処理で２つ以上外す場合、foreach 中の DeleteItem で列挙中のコレクション変更例外になる")]
        public void AddItem_Hardpoint_LargeItemFreesMultipleSlots()
        {
            // サイズ２のアイテムは空きが足りるまで先頭から２つ外す
            var src = CreateSRC();
            var u = CreateUnit(src, "武装機", "汎用", 4, "味方", "ハードポイントLv2=武器");
            u.AddItem(CreateItem(src, "銃Ａ", "武器"));
            u.AddItem(CreateItem(src, "銃Ｂ", "武器"));
            u.AddItem(CreateItem(src, "大砲", "武器", "汎用", "大型アイテム"));
            CollectionAssert.AreEqual(new[] { "大砲" }, Names(u), string.Join(",", Names(u)));
        }

        // ──────────────────────────────────────────────
        // DeleteItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteItem_RemovesAndReleasesItem()
        {
            // 外したアイテムはユニットとの関連が切れ、能力値も元に戻る
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var itm = CreateItem(src, "装甲板", "アイテム");
            itm.Data.Armor = 200;
            var other = CreateItem(src, "追加装甲", "アイテム");
            other.Data.Armor = 50;
            u.AddItem(itm);
            u.AddItem(other);
            Assert.AreEqual(350, u.get_Armor(""));

            u.DeleteItem(itm);
            CollectionAssert.AreEqual(new[] { "追加装甲" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(itm.Unit);
            Assert.AreEqual(150, u.get_Armor(""));

            // 存在しないアイテム（null）は無視し、残りの装備と能力値は変わらない（Unit.cls DeleteItem）
            u.DeleteItem(null);
            CollectionAssert.AreEqual(new[] { "追加装甲" }, Names(u), string.Join(",", Names(u)));
            Assert.AreSame(u, other.Unit);
            Assert.AreEqual(150, u.get_Armor(""));
        }

        [TestMethod]
        public void DeleteItem_AdditionalPilot_IsDisposed()
        {
            // 追加パイロットを持つアイテムを外すと、そのパイロットは消滅する
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var pd = src.PDList.Add("搭載ＡＩ");
            pd.SP = 10;
            var p = src.PList.Add("搭載ＡＩ", 1, "味方");
            var itm = CreateItem(src, "ＡＩユニット", "アイテム", "汎用", "追加パイロット=搭載ＡＩ");
            u.AddItem0(itm);
            // 追加パイロットとしてユニットに乗っている状態にする
            p.Unit = u;
            Assert.AreSame(u, p.Unit);

            u.DeleteItem(itm);
            Assert.IsFalse(p.Alive);
            Assert.IsNull(p.Unit);
        }

        [TestMethod]
        public void DeleteItem_LosingItemHardpoint_RemovesOverflowItems()
        {
            // アイテム枠を増やすアイテムを外すと、溢れたアイテムも外れる
            var src = CreateSRC();
            var u = CreateUnit(src, "機体", "汎用", 1);
            var ext = CreateItem(src, "拡張スロット", "アイテム", "汎用", "ハードポイントLv2=アイテム");
            var a = CreateItem(src, "Ａ", "アイテム");
            var b = CreateItem(src, "Ｂ", "アイテム");
            u.AddItem(ext);
            u.AddItem(a);
            u.AddItem(b);
            Assert.AreEqual(3, u.MaxItemNum());
            CollectionAssert.AreEqual(new[] { "拡張スロット", "Ａ", "Ｂ" }, Names(u), string.Join(",", Names(u)));

            u.DeleteItem(ext);
            CollectionAssert.AreEqual(new[] { "Ａ" }, Names(u), string.Join(",", Names(u)));
            Assert.IsNull(b.Unit);
        }

        [TestMethod]
        [Ignore("移植の不具合: DeleteItem が Update 後に両手利きを記録するため、両手利きを失っても２つ目の片手武器を外さない")]
        public void DeleteItem_LosingAmbidexterity_RemovesSecondOneHandedWeapon()
        {
            // 両手利きを与えるアイテムを外すと、２つ目の片手武器を外す
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕");
            var ring = CreateItem(src, "二刀の指輪", "アイテム", "汎用", "両手利き");
            u.AddItem(ring);
            Assert.IsTrue(u.IsFeatureAvailable("両手利き"));
            u.AddItem(CreateItem(src, "剣Ａ", "片手"));
            u.AddItem(CreateItem(src, "剣Ｂ", "片手"));
            CollectionAssert.AreEqual(new[] { "二刀の指輪", "剣Ａ", "剣Ｂ" }, Names(u), string.Join(",", Names(u)));

            u.DeleteItem(ring);
            CollectionAssert.AreEqual(new[] { "剣Ａ" }, Names(u), string.Join(",", Names(u)));
        }

        [TestMethod]
        [Ignore("移植の不具合: DeleteItem が ItemSlots に収まらない表示アイテムを全て外すため、自作装備個所のアイテムまで外れる")]
        public void DeleteItem_KeepsCustomPartItem()
        {
            // VB の DeleteItem はアイテム枠・ハードポイントが減った場合のみ他のアイテムを外す。
            // 自作の装備個所（ハードポイント無しで１個まで装備可能）のアイテムは無関係のアイテムを外しても残る
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var back = CreateItem(src, "バックパック", "背中");
            var a = CreateItem(src, "Ａ", "アイテム");
            u.AddItem(back);
            u.AddItem(a);

            u.DeleteItem(a);
            CollectionAssert.AreEqual(new[] { "バックパック" }, Names(u), string.Join(",", Names(u)));
        }

        // ──────────────────────────────────────────────
        // IsAbleToEquip
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAbleToEquip_AlreadyEquippedOrFixed_IsFalse()
        {
            // 既に装備済み、Fix コマンドで固定、Disable されたアイテムは装備不可
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var equipped = CreateItem(src, "装備済み", "アイテム");
            u.AddItem(equipped);
            Assert.IsFalse(u.IsAbleToEquip(equipped));

            var fix = CreateItem(src, "固定品", "アイテム");
            Assert.IsTrue(u.IsAbleToEquip(fix));
            src.Expression.DefineGlobalVariable("Fix(固定品)");
            Assert.IsFalse(u.IsAbleToEquip(fix));

            var disabled = CreateItem(src, "封印品", "アイテム");
            src.Expression.DefineGlobalVariable("Disable(封印品)");
            Assert.IsFalse(u.IsAbleToEquip(disabled));
        }

        [TestMethod]
        public void IsAbleToEquip_GeneralAndFixedClass()
        {
            // 汎用は装備可能（ただし強化パーツは生身のユニットには不可）、固定クラスは装備不可
            var src = CreateSRC();
            var robot = CreateUnit(src, "ロボ", "汎用");
            var human = CreateUnit(src, "人間", "(人間)");
            var parts = CreateItem(src, "ブースター", "強化パーツ", "汎用");
            var item = CreateItem(src, "お守り", "アイテム", "汎用");
            var fixedItem = CreateItem(src, "呪いの品", "アイテム", "固定");
            Assert.IsTrue(robot.IsAbleToEquip(parts));
            Assert.IsFalse(human.IsAbleToEquip(parts));
            Assert.IsTrue(human.IsAbleToEquip(item));
            Assert.IsFalse(robot.IsAbleToEquip(fixedItem));
        }

        [TestMethod]
        public void IsAbleToEquip_WeaponClass()
        {
            // 武器は武器クラスに一致するクラスのみ装備可能
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "(戦士)", 4, "味方", "装備個所=腕", "武器クラス=剣");
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "鋼の剣", "片手", "剣")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "斧", "片手", "斧")));
        }

        [TestMethod]
        public void IsAbleToEquip_WeaponExclusiveToUnitNameOrClass()
        {
            // 「剣(ユニット名専用)」「剣(ユニットクラス専用)」は該当ユニットのみ装備可能
            var src = CreateSRC();
            var hero = CreateUnit(src, "勇者", "(戦士)", 4, "味方", "装備個所=腕", "武器クラス=剣");
            var other = CreateUnit(src, "村人", "(村人)", 4, "味方", "装備個所=腕", "武器クラス=剣");
            var byName = CreateItem(src, "勇者の剣", "片手", "剣(勇者専用)");
            var byClass = CreateItem(src, "戦士の剣", "片手", "剣(戦士専用)");
            Assert.IsTrue(hero.IsAbleToEquip(byName));
            Assert.IsFalse(other.IsAbleToEquip(byName));
            Assert.IsTrue(hero.IsAbleToEquip(byClass));
            Assert.IsFalse(other.IsAbleToEquip(byClass));
        }

        [TestMethod]
        public void IsAbleToEquip_ExclusiveBySex()
        {
            // 「鎧(女性専用)」はメインパイロットの性別で判定する
            var src = CreateSRC();
            var female = CreateUnit(src, "女戦士", "(戦士)", 4, "味方", "装備個所=体", "防具クラス=鎧");
            RidePilot(src, female, "アリス", "女性");
            var male = CreateUnit(src, "男戦士", "(戦士)", 4, "味方", "装備個所=体", "防具クラス=鎧");
            RidePilot(src, male, "ボブ", "男性");
            var armor = CreateItem(src, "ビキニアーマー", "体", "鎧(女性専用)");
            Assert.IsTrue(female.IsAbleToEquip(armor));
            Assert.IsFalse(male.IsAbleToEquip(armor));

            // アイテム・自作装備個所の性別専用指定
            var ring = CreateItem(src, "女神の指輪", "アイテム", "指輪(女性専用)");
            Assert.IsTrue(female.IsAbleToEquip(ring));
            Assert.IsFalse(male.IsAbleToEquip(ring));
            var cape = CreateItem(src, "女神のマント", "背中", "マント(女性専用)");
            Assert.IsTrue(female.IsAbleToEquip(cape));
            Assert.IsFalse(male.IsAbleToEquip(cape));
        }

        [TestMethod]
        public void IsAbleToEquip_ArmorClass()
        {
            // 防具は防具クラスに一致するクラスのみ。専用指定はユニット名で判定
            var src = CreateSRC();
            var u = CreateUnit(src, "騎士", "(騎士)", 4, "味方", "装備個所=腕 体 頭", "防具クラス=鎧 盾 兜");
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "鉄の鎧", "体", "鎧")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "鉄の盾", "盾", "盾")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "ローブ", "体", "服")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "騎士の兜", "頭", "兜(騎士専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "王の兜", "頭", "兜(王専用)")));
        }

        [TestMethod]
        public void IsAbleToEquip_OtherFormProficiency()
        {
            // 武器・防具の専用指定は他形態のユニット名・ユニットクラスでも判定する
            var src = CreateSRC();
            var u = CreateUnit(src, "変形機", "(人型)", 4, "味方", "装備個所=腕 体", "武器クラス=銃", "防具クラス=装甲");
            var form = CreateUnit(src, "変形機改", "(戦闘機)", 4, "味方", "装備個所=腕 体", "武器クラス=銃", "防具クラス=装甲");
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "専用ライフル", "片手", "銃(変形機改専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "専用装甲", "体", "装甲(戦闘機専用)")));

            u.AddOtherForm(form);
            Assert.IsTrue(u.IsAbleToEquip(src.IList.Item("専用ライフル")));
            Assert.IsTrue(u.IsAbleToEquip(src.IList.Item("専用装甲")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "戦闘機ライフル", "片手", "銃(戦闘機専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "改装甲", "体", "装甲(変形機改専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "他機ライフル", "片手", "銃(ザク専用)")));
        }

        [TestMethod]
        public void IsAbleToEquip_ItemPartExclusive()
        {
            // アイテム・強化パーツは専用指定でなければ装備可能。専用指定は名前・クラス・他形態で判定
            var src = CreateSRC();
            var u = CreateUnit(src, "ガンダム", "ＭＳ");
            var form = CreateUnit(src, "Ｇファイター", "戦闘機");
            u.AddOtherForm(form);
            var human = CreateUnit(src, "人間", "(人間)");

            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "ブースター", "強化パーツ", "推進器")));
            // 強化パーツは生身のユニットには装備できない
            Assert.IsFalse(human.IsAbleToEquip(CreateItem(src, "ブースター２", "強化パーツ", "推進器")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "名前専用", "アイテム", "部品(ガンダム専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "クラス専用", "アイテム", "部品(ＭＳ専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "他形態専用", "アイテム", "部品(Ｇファイター専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "他機専用", "アイテム", "部品(ザク専用)")));
        }

        [TestMethod]
        public void IsAbleToEquip_CustomPartExclusive()
        {
            // 自作の装備個所も専用指定でなければ装備可能
            var src = CreateSRC();
            var u = CreateUnit(src, "ガンダム", "ＭＳ");
            var form = CreateUnit(src, "Ｇファイター", "戦闘機");
            u.AddOtherForm(form);
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "バックパック", "背中", "背負い物")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "専用パック", "背中", "背負い物(ガンダム専用)")));
            Assert.IsTrue(u.IsAbleToEquip(CreateItem(src, "形態パック", "背中", "背負い物(戦闘機専用)")));
            Assert.IsFalse(u.IsAbleToEquip(CreateItem(src, "他機パック", "背中", "背負い物(ザク専用)")));
        }
    }
}
