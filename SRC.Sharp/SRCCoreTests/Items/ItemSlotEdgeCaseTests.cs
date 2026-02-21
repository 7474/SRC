using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// ItemSlot.IsMatch メソッドのより詳細なエッジケーステスト
    /// </summary>
    [TestClass]
    public class ItemSlotEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // アイテムスロット名と装備個所の完全一致 - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_WeaponPartExactMatch_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "武器" };
            Assert.IsTrue(slot.IsMatch("武器"));
        }

        [TestMethod]
        public void IsMatch_WeaponPart_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("武器"));
        }

        [TestMethod]
        public void IsMatch_EmptySlotName_WithEmptyPartName_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "" };
            Assert.IsTrue(slot.IsMatch(""));
        }

        [TestMethod]
        public void IsMatch_EmptySlotName_WithNonEmptyPartName_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "" };
            Assert.IsFalse(slot.IsMatch("体"));
        }

        [TestMethod]
        public void IsMatch_CustomPartName_ExactMatch_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "カスタムスロット" };
            Assert.IsTrue(slot.IsMatch("カスタムスロット"));
        }

        [TestMethod]
        public void IsMatch_CustomPartName_DifferentName_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "スロットA" };
            Assert.IsFalse(slot.IsMatch("スロットB"));
        }

        // ──────────────────────────────────────────────
        // 片手/両手/盾と右手/左手 - 境界テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Tate_LeftHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsTrue(slot.IsMatch("盾"));
        }

        [TestMethod]
        public void IsMatch_Tate_HeadSlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsFalse(slot.IsMatch("盾"));
        }

        [TestMethod]
        public void IsMatch_RyoHand_HeadSlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsFalse(slot.IsMatch("両手"));
        }

        // ──────────────────────────────────────────────
        // 肩/両肩と右肩/左肩 - 境界テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_RyoKata_HeadSlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsFalse(slot.IsMatch("両肩"));
        }

        [TestMethod]
        public void IsMatch_Kata_RightHandSlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsFalse(slot.IsMatch("肩"));
        }

        // ──────────────────────────────────────────────
        // アイテム/強化パーツ - 混合テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_ItemPart_ItemSlot_ReturnsTrue_2()
        {
            var slot = new ItemSlot { SlotName = "アイテム" };
            Assert.IsTrue(slot.IsMatch("強化パーツ"));
        }

        [TestMethod]
        public void IsMatch_KoukaParts_ItemSlot_ReturnsTrue_2()
        {
            var slot = new ItemSlot { SlotName = "強化パーツ" };
            Assert.IsTrue(slot.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_ItemPart_BodySlot_ReturnsFalse_2()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("強化パーツ"));
        }

        // ──────────────────────────────────────────────
        // IsEmpty / IsOccupied / IsAvalable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEmpty_WithNullItem_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右手", Item = null };
            Assert.IsTrue(slot.IsEmpty);
        }

        [TestMethod]
        public void IsOccupied_Default_IsFalse()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsFalse(slot.IsOccupied);
        }

        [TestMethod]
        public void IsOccupied_SetTrue_IsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手", IsOccupied = true };
            Assert.IsTrue(slot.IsOccupied);
        }
    }
}
