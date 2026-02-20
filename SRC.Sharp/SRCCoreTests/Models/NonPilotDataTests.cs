using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// NonPilotData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class NonPilotDataTests
    {
        // ──────────────────────────────────────────────
        // Name フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var npd = new NonPilotData { Name = "テストキャラ" };
            Assert.AreEqual("テストキャラ", npd.Name);
        }

        [TestMethod]
        public void Name_DefaultIsNull()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Name);
        }

        // ──────────────────────────────────────────────
        // Nickname / Nickname0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SetAndGet_ReturnsValue()
        {
            var npd = new NonPilotData();
            npd.Nickname = "テストちゃん";
            Assert.AreEqual("テストちゃん", npd.Nickname);
        }

        [TestMethod]
        public void Nickname0_ReturnsSameAsNickname()
        {
            var npd = new NonPilotData();
            npd.Nickname = "あだなA";
            Assert.AreEqual("あだなA", npd.Nickname0);
        }

        [TestMethod]
        public void Nickname_DefaultIsNull()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Nickname);
        }

        // ──────────────────────────────────────────────
        // Bitmap / Bitmap0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bitmap_SetAndGet_ReturnsValue()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "chara.bmp";
            Assert.AreEqual("chara.bmp", npd.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_ReturnsSameAsBitmap()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "face.bmp";
            Assert.AreEqual("face.bmp", npd.Bitmap0);
        }

        [TestMethod]
        public void Bitmap_DefaultIsNull()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Bitmap);
        }

        // ──────────────────────────────────────────────
        // IsBitmapMissing フラグ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsBitmapMissing_DefaultIsFalse()
        {
            var npd = new NonPilotData();
            Assert.IsFalse(npd.IsBitmapMissing);
        }

        [TestMethod]
        public void Bitmap_WhenIsBitmapMissing_ReturnsDashBmp()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "original.bmp";
            npd.IsBitmapMissing = true;

            // IsBitmapMissing が true の場合 "-.bmp" を返す
            Assert.AreEqual("-.bmp", npd.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_WhenIsBitmapMissing_StillReturnsOriginal()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "original.bmp";
            npd.IsBitmapMissing = true;

            // Bitmap0 は IsBitmapMissing に関わらず元の値を返す
            Assert.AreEqual("original.bmp", npd.Bitmap0);
        }

        [TestMethod]
        public void Bitmap_WhenIsBitmapMissingIsFalse_ReturnsOriginal()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "char01.bmp";
            npd.IsBitmapMissing = false;

            Assert.AreEqual("char01.bmp", npd.Bitmap);
        }
    }
}
