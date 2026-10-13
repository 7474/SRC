using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.ChangeParty（説得などで陣営が変わる）
    /// </summary>
    [TestClass]
    public class UnitChangePartyTests
    {
        [TestMethod]
        public void ChangeParty_WithOtherForms_ChangesAllFormsWithoutRecursion()
        {
            // 他形態は互いに他形態として登録されている。元の SRC と同じく陣営だけを変え、呼び合いで止まらなくならない（ロボダンの説得）
            var src = new SRC { GUI = new MockGUI() };
            src.UDList.Add("形態Ａ");
            src.UDList.Add("形態Ｂ");
            var a = src.UList.Add("形態Ａ", 0, "敵");
            var b = src.UList.Add("形態Ｂ", 0, "敵");
            a.AddOtherForm(b);
            b.AddOtherForm(a);

            a.ChangeParty("味方");

            Assert.AreEqual("味方", a.Party0);
            Assert.AreEqual("味方", b.Party0);
        }
    }
}
