using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCSharpFormTestWinAppDriver
{
    [TestClass]
    public class ExecuteTest : SRCSharpFormSession
    {
        [TestMethod]
        public void Execute()
        {
            Assert.AreEqual("SRC#Form", session.Title);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }
    }
}
