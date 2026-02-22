using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class DialogDataMoreTests3
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.Name = "アムロ";
            Assert.AreEqual("アムロ", dd.Name);
        }

        [TestMethod]
        public void Name_DefaultIsNull()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            Assert.IsNull(dd.Name);
        }

        [TestMethod]
        public void CountDialog_Initially_IsZero()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            Assert.AreEqual(0, dd.CountDialog());
        }

        [TestMethod]
        public void AddDialog_IncreasesCount()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("格闘");
            Assert.AreEqual(1, dd.CountDialog());
        }

        [TestMethod]
        public void AddDialog_MultipleItems_CountIsCorrect()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("格闘");
            dd.AddDialog("射撃");
            dd.AddDialog("回避");
            Assert.AreEqual(3, dd.CountDialog());
        }

        [TestMethod]
        public void AddDialog_ReturnsNonNull()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("攻撃");
            Assert.IsNotNull(dialog);
        }

        [TestMethod]
        public void AddDialog_SetsSituation()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("移動");
            Assert.AreEqual("移動", dialog.Situation);
        }

        [TestMethod]
        public void Items_Initially_IsEmpty()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            Assert.AreEqual(0, dd.Items.Count);
        }

        [TestMethod]
        public void Items_AfterAdd_HasCorrectCount()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("挑発");
            Assert.AreEqual(1, dd.Items.Count);
        }

        [TestMethod]
        public void AddDialog_MultipleWithSameSituation_AllAdded()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("格闘");
            dd.AddDialog("格闘");
            Assert.AreEqual(2, dd.CountDialog());
        }

        [TestMethod]
        public void Name_CanBeChanged()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.Name = "シャア";
            dd.Name = "ランバ・ラル";
            Assert.AreEqual("ランバ・ラル", dd.Name);
        }

        [TestMethod]
        public void CountDialog_MatchesItemsCount()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("a");
            dd.AddDialog("b");
            Assert.AreEqual(dd.Items.Count, dd.CountDialog());
        }
    }
}
