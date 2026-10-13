using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// Items.Update（インターミッションの前などに呼ばれる）
    /// </summary>
    [TestClass]
    public class ItemsUpdateTests
    {
        [TestMethod]
        public void Update_RemovesDestroyedItems()
        {
            // 破棄されたアイテムが複数あっても、列挙中の削除で例外にならない（ロボダンのゲームオーバー後の Continue）
            var src = new SRC { GUI = new MockGUI() };
            src.IDList.Add("テストアイテム");
            var a = src.IList.Add("テストアイテム");
            var b = src.IList.Add("テストアイテム");
            var c = src.IList.Add("テストアイテム");
            a.Exist = false;
            c.Exist = false;

            src.IList.Update();

            Assert.AreEqual(1, src.IList.Count());
            Assert.AreSame(b, src.IList.List[0]);
        }
    }
}
