using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// Item クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ItemTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Item CreateItemWithPart(SRC src, string itemName, string part)
        {
            var itemData = src.IDList.Add(itemName);
            itemData.Part = part;
            var item = src.IList.Add(itemName);
            return item;
        }

        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_ExistIsTrue()
        {
            var src = CreateSRC();
            var item = new Item(src);
            Assert.IsTrue(item.Exist);
        }

        [TestMethod]
        public void Constructor_ActivatedIsTrue()
        {
            var src = CreateSRC();
            var item = new Item(src);
            Assert.IsTrue(item.Activated);
        }

        [TestMethod]
        public void Constructor_UnitIdIsNull()
        {
            var src = CreateSRC();
            var item = new Item(src);
            Assert.IsNull(item.UnitId);
        }

        [TestMethod]
        public void Constructor_ItemNameIsNull()
        {
            var src = CreateSRC();
            var item = new Item(src);
            Assert.IsNull(item.ItemName);
        }

        // ──────────────────────────────────────────────
        // ID / ItemName プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ID_CanBeSet()
        {
            var src = CreateSRC();
            var item = new Item(src);
            item.ID = "TEST_ID";
            Assert.AreEqual("TEST_ID", item.ID);
        }

        [TestMethod]
        public void ItemName_CanBeSet()
        {
            var src = CreateSRC();
            var item = new Item(src);
            item.ItemName = "テスト";
            Assert.AreEqual("テスト", item.ItemName);
        }

        // ──────────────────────────────────────────────
        // Restore
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Restore_ItemStillWorks()
        {
            var src = CreateSRC();
            src.IDList.Add("復元アイテム");
            var item = src.IList.Add("復元アイテム");

            var src2 = CreateSRC();
            src2.IDList.Add("復元アイテム");
            item.Restore(src2);

            Assert.AreEqual("復元アイテム", item.Name);
        }

        // ──────────────────────────────────────────────
        // IsMatch - 完全一致
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_ExactMatch_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "体装備", "体");
            Assert.IsTrue(item.IsMatch("体"));
        }

        [TestMethod]
        public void IsMatch_NoMatch_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "体装備2", "体");
            Assert.IsFalse(item.IsMatch("頭"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 片手 → 右手 / 左手
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_KataTe_RightHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "片手武器", "片手");
            Assert.IsTrue(item.IsMatch("右手"));
        }

        [TestMethod]
        public void IsMatch_KataTe_LeftHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "片手武器2", "片手");
            Assert.IsTrue(item.IsMatch("左手"));
        }

        [TestMethod]
        public void IsMatch_KataTe_Body_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "片手武器3", "片手");
            Assert.IsFalse(item.IsMatch("体"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 両手 → 右手 / 左手
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_RyouTe_RightHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両手武器", "両手");
            Assert.IsTrue(item.IsMatch("右手"));
        }

        [TestMethod]
        public void IsMatch_RyouTe_LeftHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両手武器2", "両手");
            Assert.IsTrue(item.IsMatch("左手"));
        }

        [TestMethod]
        public void IsMatch_RyouTe_Shoulder_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両手武器3", "両手");
            Assert.IsFalse(item.IsMatch("右肩"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 盾 → 右手 / 左手
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Tate_RightHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "盾装備", "盾");
            Assert.IsTrue(item.IsMatch("右手"));
        }

        [TestMethod]
        public void IsMatch_Tate_LeftHand_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "盾装備2", "盾");
            Assert.IsTrue(item.IsMatch("左手"));
        }

        [TestMethod]
        public void IsMatch_Tate_Body_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "盾装備3", "盾");
            Assert.IsFalse(item.IsMatch("体"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 肩 → 右肩 / 左肩
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Kata_RightShoulder_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "肩装備", "肩");
            Assert.IsTrue(item.IsMatch("右肩"));
        }

        [TestMethod]
        public void IsMatch_Kata_LeftShoulder_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "肩装備2", "肩");
            Assert.IsTrue(item.IsMatch("左肩"));
        }

        [TestMethod]
        public void IsMatch_Kata_Hand_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "肩装備3", "肩");
            Assert.IsFalse(item.IsMatch("右手"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 両肩 → 右肩 / 左肩
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_RyouKata_RightShoulder_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両肩装備", "両肩");
            Assert.IsTrue(item.IsMatch("右肩"));
        }

        [TestMethod]
        public void IsMatch_RyouKata_LeftShoulder_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両肩装備2", "両肩");
            Assert.IsTrue(item.IsMatch("左肩"));
        }

        [TestMethod]
        public void IsMatch_RyouKata_Hand_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "両肩装備3", "両肩");
            Assert.IsFalse(item.IsMatch("左手"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - アイテム ↔ 強化パーツ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Item_KyoukaPartsSlot_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "アイテム装備", "アイテム");
            Assert.IsTrue(item.IsMatch("強化パーツ"));
        }

        [TestMethod]
        public void IsMatch_Item_ItemSlot_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "アイテム装備2", "アイテム");
            Assert.IsTrue(item.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_KyoukaParts_ItemSlot_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "強化パーツ装備", "強化パーツ");
            Assert.IsTrue(item.IsMatch("アイテム"));
        }

        [TestMethod]
        public void IsMatch_KyoukaParts_KyoukaPartsSlot_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "強化パーツ装備2", "強化パーツ");
            Assert.IsTrue(item.IsMatch("強化パーツ"));
        }

        [TestMethod]
        public void IsMatch_KyoukaParts_Hand_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "強化パーツ装備3", "強化パーツ");
            Assert.IsFalse(item.IsMatch("右手"));
        }

        // ──────────────────────────────────────────────
        // IsMatch - 非マッチ組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMatch_Head_Shoulder_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "頭装備", "頭");
            Assert.IsFalse(item.IsMatch("右肩"));
        }

        [TestMethod]
        public void IsMatch_KataTe_Shoulder_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = CreateItemWithPart(src, "片手武器4", "片手");
            Assert.IsFalse(item.IsMatch("右肩"));
        }

        // ──────────────────────────────────────────────
        // Exist / Activated フラグ変更
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Exist_CanBeSetToFalse()
        {
            var src = CreateSRC();
            var item = new Item(src);
            item.Exist = false;
            Assert.IsFalse(item.Exist);
        }

        [TestMethod]
        public void Activated_CanBeSetToFalse()
        {
            var src = CreateSRC();
            var item = new Item(src);
            item.Activated = false;
            Assert.IsFalse(item.Activated);
        }
    }
}
