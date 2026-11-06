using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.FullRecover（元の SRC の Unit.FullRecover。Unit.cls 27211 行）
    /// </summary>
    [TestClass]
    public class UnitFullRecoverTests
    {
        private static (SRC src, Unit a, Unit b) CreateTwoForms()
        {
            var src = new SRC { GUI = new MockGUI() };
            var uda = src.UDList.Add("形態Ａ");
            uda.HP = 1000;
            uda.EN = 100;
            var udb = src.UDList.Add("形態Ｂ");
            udb.HP = 2000;
            udb.EN = 200;
            var a = src.UList.Add("形態Ａ", 0, "味方");
            var b = src.UList.Add("形態Ｂ", 0, "味方");
            a.AddOtherForm(b);
            b.AddOtherForm(a);
            return (src, a, b);
        }

        [TestMethod]
        public void FullRecover_FromOtherForm_RecoversOtherFormsOfCurrentForm()
        {
            // 元の SRC は With CurrentForm の中で .OtherForm(i) を回復する。
            // 他形態になっている形態の FullRecover でも、現在の形態から見た他形態（呼んだ形態自身）の HP が回復する
            var (_, a, b) = CreateTwoForms();
            a.Status = "他形態";
            b.Status = "出撃";
            a.HP = 10;
            b.HP = 20;

            a.FullRecover();

            Assert.AreEqual(1000, a.HP);
            Assert.AreEqual(2000, b.HP);
        }

        [TestMethod]
        public void FullRecover_DeletesOnlyStatusConditions()
        {
            // 「残り時間」「非操作」と、名前が「付加」「強化」「付加２」「強化２」「ＵＰ」で終わる状態は残し、ほかは消す
            var (_, a, _) = CreateTwoForms();
            a.AddCondition("毒", 3, 0d, "");
            a.AddCondition("残り時間", 3, 0d, "");
            a.AddCondition("非操作", -1, 0d, "");
            a.AddCondition("攻撃力ＵＰ", 3, 0d, "");
            a.AddCondition("麻痺", 3, 0d, "");
            a.AddCondition("ＨＰ回復強化", 3, 0d, "");

            a.FullRecover();

            Assert.IsFalse(a.IsConditionSatisfied("毒"));
            Assert.IsFalse(a.IsConditionSatisfied("麻痺"));
            Assert.IsTrue(a.IsConditionSatisfied("残り時間"));
            Assert.IsTrue(a.IsConditionSatisfied("非操作"));
            Assert.IsTrue(a.IsConditionSatisfied("攻撃力ＵＰ"));
            Assert.IsTrue(a.IsConditionSatisfied("ＨＰ回復強化"));
            Assert.AreEqual(4, a.CountCondition());
        }
    }
}
