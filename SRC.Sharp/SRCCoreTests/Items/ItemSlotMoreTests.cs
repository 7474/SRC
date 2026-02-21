using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// ItemSlot の追加ユニットテスト（既存テストで未カバーの項目）
    /// </summary>
    [TestClass]
    public class ItemSlotMoreTests
    {
        // ──────────────────────────────────────────────
        // IsAvalable プロパティ（Item が null の場合）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAvalable_WhenItemIsNull_ReturnsTrue()
        {
            // IsEmpty=true のとき IsAvalable=true
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsTrue(slot.IsAvalable);
        }

        [TestMethod]
        public void IsAvalable_WhenSlotIsEmpty_IsEmpty_IsTrue()
        {
            var slot = new ItemSlot { SlotName = "頭" };
            Assert.IsTrue(slot.IsEmpty);
            Assert.IsTrue(slot.IsAvalable);
        }

        // ──────────────────────────────────────────────
        // IsOccupied のトグル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsOccupied_CanBeSetBackToFalse()
        {
            var slot = new ItemSlot { SlotName = "右手", IsOccupied = true };
            Assert.IsTrue(slot.IsOccupied);
            slot.IsOccupied = false;
            Assert.IsFalse(slot.IsOccupied);
        }

        // ──────────────────────────────────────────────
        // IsMatch - 空文字列のパーツ名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_EmptyPartName_ReturnsFalse_ForBodySlot()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch(""));
        }

        [TestMethod]
        public void IsMatch_EmptyPartName_ReturnsFalse_ForRightHandSlot()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsFalse(slot.IsMatch(""));
        }

        // ──────────────────────────────────────────────
        // IsMatch - null パーツ名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_NullPartName_ReturnsFalse_ForBodySlot()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch(null));
        }

        [TestMethod]
        public void IsMatch_NullPartName_ReturnsFalse_ForRightHandSlot()
        {
            var slot = new ItemSlot { SlotName = "右手" };
            Assert.IsFalse(slot.IsMatch(null));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 未知のパーツ名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_UnknownPartName_ReturnsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsFalse(slot.IsMatch("未知のパーツ"));
        }

        [TestMethod]
        public void IsMatch_UnknownPartName_ReturnsFalse_ForShoulderSlot()
        {
            var slot = new ItemSlot { SlotName = "右肩" };
            Assert.IsFalse(slot.IsMatch("謎パーツ"));
        }

        // ──────────────────────────────────────────────
        // SlotName の追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SlotName_CanBeUpdated()
        {
            var slot = new ItemSlot { SlotName = "体" };
            slot.SlotName = "頭";
            Assert.AreEqual("頭", slot.SlotName);
        }

        [TestMethod]
        public void SlotName_EmptyString_IsMatch_ReturnsFalse_ForBody()
        {
            // SlotName が空文字のスロット: IsMatch("体") → "体" == "" は false → false
            var slot = new ItemSlot { SlotName = "" };
            Assert.IsFalse(slot.IsMatch("体"));
        }

        // ──────────────────────────────────────────────
        // IsEmpty / IsOccupied の組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEmpty_DefaultSlot_IsTrue_IsOccupied_IsFalse()
        {
            var slot = new ItemSlot { SlotName = "体" };
            Assert.IsTrue(slot.IsEmpty);
            Assert.IsFalse(slot.IsOccupied);
        }

        [TestMethod]
        public void IsOccupied_True_SlotIsEmpty_BothCanCoexist()
        {
            // IsOccupied は Item の有無とは独立に設定できる
            var slot = new ItemSlot { SlotName = "左手", IsOccupied = true };
            Assert.IsTrue(slot.IsOccupied);
            Assert.IsTrue(slot.IsEmpty); // Item は null のまま
        }
    }
}
