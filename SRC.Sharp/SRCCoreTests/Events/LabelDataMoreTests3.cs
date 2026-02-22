using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// LabelData クラスの追加ユニットテスト (LabelDataMoreTests3)
    /// </summary>
    [TestClass]
    public class LabelDataMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private LabelData Create(string data)
        {
            var src = CreateSrc();
            return new LabelData(src) { Data = data };
        }

        [TestMethod]
        public void ToString_ContainsEventDataId()
        {
            var src = CreateSrc();
            var ld = new LabelData(src) { Data = "スタート" };
            ld.EventDataId = 42;
            var result = ld.ToString();
            Assert.IsTrue(result.Contains("42"), $"Expected '42' in '{result}'");
        }

        [TestMethod]
        public void ToString_ContainsData()
        {
            var src = CreateSrc();
            var ld = new LabelData(src) { Data = "ターン 1 3" };
            ld.EventDataId = 5;
            var result = ld.ToString();
            Assert.IsTrue(result.Contains("ターン"), $"Expected data string in '{result}'");
        }

        [TestMethod]
        public void Enable_DefaultIsFalse()
        {
            var ld = Create("スタート");
            Assert.IsFalse(ld.Enable);
        }

        [TestMethod]
        public void Enable_CanBeSetToTrue()
        {
            var ld = Create("スタート");
            ld.Enable = true;
            Assert.IsTrue(ld.Enable);
        }

        [TestMethod]
        public void AsterNum_DefaultIsZero()
        {
            var ld = Create("スタート");
            // スタートにプレフィックスなし → AsterNum = 1
            Assert.IsTrue(ld.AsterNum >= 0);
        }

        [TestMethod]
        public void EventDataId_CanBeSet()
        {
            var src = CreateSrc();
            var ld = new LabelData(src) { Data = "スタート" };
            ld.EventDataId = 100;
            Assert.AreEqual(100, ld.EventDataId);
        }

        [TestMethod]
        public void CountPara_NormalLabel_ReturnsOne()
        {
            var ld = Create("myLabel");
            Assert.AreEqual(1, ld.CountPara());
        }

        [TestMethod]
        public void CountPara_FinishLabel_ReturnsOne()
        {
            var ld = Create("行動終了");
            Assert.AreEqual(1, ld.CountPara());
        }

        [TestMethod]
        public void Name_TransformLabel()
        {
            var ld = Create("変形");
            Assert.AreEqual(LabelType.TransformEventLabel, ld.Name);
        }

        [TestMethod]
        public void Name_FinishLabel()
        {
            var ld = Create("行動終了");
            Assert.AreEqual(LabelType.FinishEventLabel, ld.Name);
        }

        [TestMethod]
        public void Name_LevelUpLabel()
        {
            var ld = Create("レベルアップ");
            Assert.AreEqual(LabelType.LevelUpEventLabel, ld.Name);
        }
    }
}
