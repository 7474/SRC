using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// 描画の基準座標の保存・復元（SaveBasePoint / RestoreBasePoint）。元の SRC（Event.bas）と同じ動きをする。
    /// </summary>
    [TestClass]
    public class EventBasePointTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void RestoreWithoutSave_DoesNotThrow()
        {
            // 保存より復元が多い（ロボダンの演出など）。先頭以下なら末尾に戻る
            var src = CreateSrc();
            src.Event.ResetBasePoint();
            for (var i = 0; i < 30; i++)
            {
                src.Event.RestoreBasePoint();
            }
            Assert.AreEqual(0, src.Event.BaseX);
        }

        [TestMethod]
        public void SaveAndRestore_RestoresSavedPoint()
        {
            var src = CreateSrc();
            src.Event.ResetBasePoint();
            src.Event.BaseX = 10;
            src.Event.BaseY = 20;
            src.Event.SaveBasePoint();
            src.Event.BaseX = 30;
            src.Event.BaseY = 40;
            src.Event.SaveBasePoint();
            src.Event.BaseX = 0;
            src.Event.BaseY = 0;

            src.Event.RestoreBasePoint();
            Assert.AreEqual(30, src.Event.BaseX);
            Assert.AreEqual(40, src.Event.BaseY);
            src.Event.RestoreBasePoint();
            Assert.AreEqual(10, src.Event.BaseX);
            Assert.AreEqual(20, src.Event.BaseY);
        }
    }
}
