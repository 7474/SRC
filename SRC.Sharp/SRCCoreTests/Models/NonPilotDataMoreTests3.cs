using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class NonPilotDataMoreTests3
    {
        [TestMethod]
        public void Name_DefaultIsNull()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Name);
        }

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var npd = new NonPilotData { Name = "ロボット" };
            Assert.AreEqual("ロボット", npd.Name);
        }

        [TestMethod]
        public void Name_CanBeChanged()
        {
            var npd = new NonPilotData();
            npd.Name = "初期名";
            npd.Name = "変更名";
            Assert.AreEqual("変更名", npd.Name);
        }

        [TestMethod]
        public void Nickname_DefaultIsNull()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Nickname0);
        }

        [TestMethod]
        public void Nickname_SetAndGet_ReturnsValue()
        {
            var npd = new NonPilotData();
            npd.Nickname = "ニックネーム";
            Assert.AreEqual("ニックネーム", npd.Nickname);
        }

        [TestMethod]
        public void Nickname0_MatchesNickname_AfterSet()
        {
            var npd = new NonPilotData();
            npd.Nickname = "愛称テスト";
            Assert.AreEqual("愛称テスト", npd.Nickname0);
        }

        [TestMethod]
        public void Bitmap_DefaultIsNull_WhenNotMissing()
        {
            var npd = new NonPilotData();
            Assert.IsNull(npd.Bitmap);
        }

        [TestMethod]
        public void Bitmap_SetAndGet_ReturnsValue()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "unit.bmp";
            Assert.AreEqual("unit.bmp", npd.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_MatchesBitmapValue()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "test.bmp";
            Assert.AreEqual("test.bmp", npd.Bitmap0);
        }

        [TestMethod]
        public void IsBitmapMissing_DefaultIsFalse()
        {
            var npd = new NonPilotData();
            Assert.IsFalse(npd.IsBitmapMissing);
        }

        [TestMethod]
        public void Bitmap_WhenIsBitmapMissing_ReturnsFallback()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "real.bmp";
            npd.IsBitmapMissing = true;
            Assert.AreEqual("-.bmp", npd.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_WhenIsBitmapMissing_ReturnsOriginalValue()
        {
            var npd = new NonPilotData();
            npd.Bitmap = "original.bmp";
            npd.IsBitmapMissing = true;
            Assert.AreEqual("original.bmp", npd.Bitmap0);
        }

        [TestMethod]
        public void IsBitmapMissing_CanBeSetTrue()
        {
            var npd = new NonPilotData();
            npd.IsBitmapMissing = true;
            Assert.IsTrue(npd.IsBitmapMissing);
        }

        [TestMethod]
        public void Nickname_CanBeChanged()
        {
            var npd = new NonPilotData();
            npd.Nickname = "古い愛称";
            npd.Nickname = "新しい愛称";
            Assert.AreEqual("新しい愛称", npd.Nickname);
        }
    }
}
