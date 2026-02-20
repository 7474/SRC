using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// NonPilotDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class NonPilotDataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 初期状態 (ナレーターが常に追加される)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InitialCount_ContainsNarrator()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            // コンストラクタでナレーターが追加される
            Assert.IsTrue(list.Count() >= 1);
        }

        [TestMethod]
        public void IsDefined_Narrator_ReturnsTrue()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            Assert.IsTrue(list.IsDefined2("ナレーター"));
        }

        // ──────────────────────────────────────────────
        // Add / Count / IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            var before = list.Count();
            list.Add("博士");
            Assert.AreEqual(before + 1, list.Count());
        }

        [TestMethod]
        public void IsDefined2_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            list.Add("研究員");
            Assert.IsTrue(list.IsDefined2("研究員"));
        }

        [TestMethod]
        public void IsDefined2_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            Assert.IsFalse(list.IsDefined2("存在しないノンパイロット_テスト用"));
        }

        [TestMethod]
        public void Item_ByExactName_ReturnsData()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            list.Add("政治家");
            var item = list.Item("政治家");
            Assert.IsNotNull(item);
            Assert.AreEqual("政治家", item.Name);
        }

        [TestMethod]
        public void Delete_RemovesData()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            list.Add("謎の人物");
            Assert.IsTrue(list.IsDefined2("謎の人物"));
            list.Delete("謎の人物");
            Assert.IsFalse(list.IsDefined2("謎の人物"));
        }

        [TestMethod]
        public void IsDefined_ByNickname_ReturnsTrue()
        {
            var src = CreateSRC();
            var list = src.NPDList;
            var npd = list.Add("実名キャラ");
            npd.Nickname = "呼び名";
            Assert.IsTrue(list.IsDefined("呼び名"));
        }
    }
}
