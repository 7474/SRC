using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.Tests
{
    /// <summary>
    /// PlaySoundConstants クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class PlaySoundConstantsTests
    {
        [TestMethod]
        public void CH_BGM_IsOne()
        {
            Assert.AreEqual(1, PlaySoundConstants.CH_BGM);
        }
    }
}
