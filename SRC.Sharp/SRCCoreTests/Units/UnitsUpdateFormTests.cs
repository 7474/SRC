using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// ステージ終了時の Units.Update による形態の整理（元の SRC の Units.Update）と
    /// Unit.FullRecover による特殊状態の消去
    /// </summary>
    [TestClass]
    public class UnitsUpdateFormTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            var ud = src.UDList.Add("基本形");
            ud.HP = 1000;
            ud.EN = 100;
            ud.AddFeature("変形=変形 変形形態");
            var ud2 = src.UDList.Add("変形形態");
            ud2.HP = 1000;
            ud2.EN = 100;
            ud2.AddFeature("変形=変形 基本形");
            var ud3 = src.UDList.Add("一時形態");
            ud3.HP = 1000;
            ud3.EN = 100;
            return src;
        }

        [TestMethod]
        public void Update_TransformsToStandardForm()
        {
            var src = CreateSrc();
            var unit = src.UList.Add("基本形", 0, "味方");
            unit.Status = "待機";
            unit.Transform("変形形態");
            Assert.AreEqual("変形形態", unit.CurrentForm().Name);

            src.UList.Update();

            // ID の最も小さいユニットデータの形態（標準形態）に戻る
            var cf = unit.CurrentForm();
            Assert.AreEqual("基本形", cf.Name);
            Assert.AreEqual("待機", cf.Status);
            Assert.AreEqual("他形態", cf.OtherForms.Single(x => x.Name == "変形形態").Status);
        }

        [TestMethod]
        public void Update_DeletesTemporaryOtherForm()
        {
            var src = CreateSrc();
            var unit = src.UList.Add("基本形", 0, "味方");
            unit.Status = "待機";
            var tmp = src.UList.Add("一時形態", 0, "味方");
            tmp.Status = "他形態";
            unit.AddOtherForm(tmp);
            Assert.AreEqual(2, unit.CountOtherForm());

            src.UList.Update();

            // 変形先として必要な形態は残し、一時的な形態は削除する
            Assert.AreEqual(1, unit.CountOtherForm());
            Assert.AreEqual("変形形態", unit.OtherForms.Single().Name);
            Assert.IsFalse(src.UList.Items.Any(x => x.ID == tmp.ID));
        }

        [TestMethod]
        public void FullRecover_ClearsAbnormalConditionsOnly()
        {
            var src = CreateSrc();
            var unit = src.UList.Add("一時形態", 0, "味方");
            unit.AddCondition("毒", 3, 0d, "");
            unit.AddCondition("麻痺", 3, 0d, "");
            unit.AddCondition("バリア付加", -1, 1d, "");
            unit.AddCondition("装甲強化", -1, 1d, "");
            unit.AddCondition("運動性ＵＰ", -1, 1d, "");
            unit.AddCondition("残り時間", 3, 0d, "");
            unit.AddCondition("非操作", -1, 0d, "");

            unit.FullRecover();

            Assert.IsFalse(unit.IsConditionSatisfied("毒"));
            Assert.IsFalse(unit.IsConditionSatisfied("麻痺"));
            Assert.IsTrue(unit.IsConditionSatisfied("バリア付加"));
            Assert.IsTrue(unit.IsConditionSatisfied("装甲強化"));
            Assert.IsTrue(unit.IsConditionSatisfied("運動性ＵＰ"));
            Assert.IsTrue(unit.IsConditionSatisfied("残り時間"));
            Assert.IsTrue(unit.IsConditionSatisfied("非操作"));
        }

        [TestMethod]
        public void RankUp_RecoversSplitPartsWithoutOtherForms()
        {
            var src = new SRC { GUI = new MockGUI() };
            foreach (var name in new[] { "合体形", "パーツＡ", "パーツＢ" })
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
            }
            src.UDList.Item("合体形").AddFeature("分離=分離 パーツＡ パーツＢ");
            var combined = src.UList.Add("合体形", 0, "味方");
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 0, "味方");
            partA.HP = 1;
            partA.EN = 1;
            partB.HP = 1;

            combined.RankUp();

            // 他形態を持たない分離形態もランクが上がり、ＨＰ・ＥＮが最大値になる
            Assert.AreEqual(1, partA.Rank);
            Assert.AreEqual(partA.MaxHP, partA.HP);
            Assert.AreEqual(partA.MaxEN, partA.EN);
            Assert.AreEqual(partB.MaxHP, partB.HP);
        }
    }
}
