using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// ItemSlot クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ItemSlotTests
    {
        // ──────────────────────────────────────────────
        // IsMatch - 完全一致
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_ExactSlotName_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsTrue(slot.IsMatch("体"));
        }

        [TestMethod]
        public void IsMatch_DifferentSlotName_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("頭"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 片手 / 両手 / 盾 → 右手 or 左手
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_KataHand_RightHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsTrue(slot.IsMatch("片手"));
        }

        [TestMethod]
        public void IsMatch_KataHand_LeftHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsTrue(slot.IsMatch("片手"));
        }

        [TestMethod]
        public void IsMatch_RyoHand_RightHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsTrue(slot.IsMatch("両手"));
        }

        [TestMethod]
        public void IsMatch_RyoHand_LeftHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsTrue(slot.IsMatch("両手"));
        }

        [TestMethod]
        public void IsMatch_Tate_RightHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsTrue(slot.IsMatch("盾"));
        }

        [TestMethod]
        public void IsMatch_Tate_LeftHandSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsTrue(slot.IsMatch("盾"));
        }

        [TestMethod]
        public void IsMatch_KataHand_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("片手"));
        }

        [TestMethod]
        public void IsMatch_RyoHand_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("両手"));
        }

        [TestMethod]
        public void IsMatch_Tate_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("盾"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 肩 / 両肩 → 右肩 or 左肩
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Kata_RightShoulderSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右肩" };
            Assert.IsTrue(slot.IsMatch("肩"));
        }

        [TestMethod]
        public void IsMatch_Kata_LeftShoulderSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左肩" };
            Assert.IsTrue(slot.IsMatch("肩"));
        }

        [TestMethod]
        public void IsMatch_RyoKata_RightShoulderSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右肩" };
            Assert.IsTrue(slot.IsMatch("両肩"));
        }

        [TestMethod]
        public void IsMatch_RyoKata_LeftShoulderSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左肩" };
            Assert.IsTrue(slot.IsMatch("両肩"));
        }

        [TestMethod]
        public void IsMatch_Kata_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("肩"));
        }

        [TestMethod]
        public void IsMatch_RyoKata_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("両肩"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - アイテム / 強化パーツ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_ItemPart_ItemSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "アイテム" };
            Assert.IsTrue(slot.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_ItemPart_KoukaParts_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "強化パーツ" };
            Assert.IsTrue(slot.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_KoukaPartsPart_ItemSlot_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "アイテム" };
            Assert.IsTrue(slot.IsMatch("強化パーツ"));
        }

        [TestMethod]
        public void IsMatch_KoukaPartsPart_KoukaParts_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "強化パーツ" };
            Assert.IsTrue(slot.IsMatch("強化パーツ"));
        }

        [TestMethod]
        public void IsMatch_ItemPart_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_KoukaPartsPart_BodySlot_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("強化パーツ"));
        }

        // ──────────────────────────────────────────────
        // IsEmpty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEmpty_WhenItemIsNull_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsTrue(slot.IsEmpty);
        }

        // ──────────────────────────────────────────────
        // IsOccupied default
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsOccupied_DefaultIsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsOccupied);
        }

        [TestMethod]
        public void IsOccupied_SetToTrue_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "体", IsOccupied = true };
            Assert.IsTrue(slot.IsOccupied);
        }

        // ──────────────────────────────────────────────
        // SlotName property
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SlotName_CanBeSetAndRead()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.AreEqual("頭", slot.SlotName);
        }

        // ──────────────────────────────────────────────
        // IsMatch - 頭 スロット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_HeadSlot_ExactMatch_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsTrue(slot.IsMatch("頭"));
        }

        [TestMethod]
        public void IsMatch_HeadSlot_NonMatchPart_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsFalse(slot.IsMatch("体"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 右手 スロット直接指定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_RightHandSlot_ExactRightHand_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsTrue(slot.IsMatch("右手"));
        }

        [TestMethod]
        public void IsMatch_LeftHandSlot_ExactLeftHand_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左手" };
            Assert.IsTrue(slot.IsMatch("左手"));
        }

        [TestMethod]
        public void IsMatch_RightShoulderSlot_ExactMatch_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "右肩" };
            Assert.IsTrue(slot.IsMatch("右肩"));
        }

        [TestMethod]
        public void IsMatch_LeftShoulderSlot_ExactMatch_ReturnsTrue()
        {
            var slot = new ItemSlot { SlotName = "左肩" };
            Assert.IsTrue(slot.IsMatch("左肩"));
        }
    }
}
