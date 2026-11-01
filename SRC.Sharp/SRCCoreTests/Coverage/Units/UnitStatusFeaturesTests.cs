using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.HelpCoverage;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitStatusFeatures
{
    /// <summary>
    /// ステータス画面の特殊能力一覧 (VB: Status.bas DisplayUnitStatus) に並べる特殊能力のテスト。
    /// 別名に「非表示」「解説」を指定した能力や、常に非表示の能力は表示名が空になり、表示されないこと。
    /// 付加された特殊能力は一覧に含めないこと。
    /// </summary>
    [TestClass]
    public class UnitStatusFeaturesTests
    {
        private static string[] VisibleNames(params string[] fdefs)
        {
            var fx = new HelpTestFixture();
            foreach (var fdef in fdefs)
            {
                fx.UnitData.AddFeature(fdef);
            }
            var u = fx.Build();
            return u.StatusFeatures()
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }

        [TestMethod]
        public void HiddenAlias_IsNotDisplayed()
        {
            // 別名が「非表示」の能力は表示しない
            CollectionAssert.AreEqual(new[] { "分身" },
                VisibleNames("バリア=非表示 全 1000 10", "分身"));
        }

        [TestMethod]
        public void ExplanationAlias_IsNotDisplayed()
        {
            // 別名が「解説」の能力は表示しない
            CollectionAssert.AreEqual(new[] { "分身" },
                VisibleNames("謎能力=解説 \"説明文\"", "分身"));
        }

        [TestMethod]
        public void HiddenAlias_WithNecessaryCondition_IsNotDisplayed()
        {
            // 必要条件を満たさない能力も一覧に並ぶが、別名が「非表示」なら表示しない
            CollectionAssert.AreEqual(new[] { "分身" },
                VisibleNames("バリア=非表示 全 1000 10 <気力Lv5>", "分身"));
        }

        [TestMethod]
        public void PilotFeature_HiddenAlias_IsNotDisplayed()
        {
            var fx = new HelpTestFixture();
            fx.Src.ALDList.Add("隠し能力").AddAlias("バリア=非表示 全 1000 10");
            fx.PilotData.AddFeature("隠し能力");
            var u = fx.Build();

            CollectionAssert.AreEqual(new string[0],
                u.StatusFeatures().Select(x => x.Name).Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }

        [TestMethod]
        public void AlwaysHiddenFeature_IsNotDisplayed()
        {
            // 「非表示」「吸収」「ＢＧＭ」等の常に非表示の能力は表示しない
            CollectionAssert.AreEqual(new[] { "分身" },
                VisibleNames("非表示", "吸収=火", "ＢＧＭ=test.mid", "分身"));
        }

        [TestMethod]
        public void Alias_IsDisplayedByAliasName()
        {
            // 別名の指定があれば別名で表示する
            CollectionAssert.AreEqual(new[] { "Ｉフィールド" },
                VisibleNames("バリア=Ｉフィールド ビーム 1000 10"));
        }

        [TestMethod]
        public void AdditionalFeature_IsNotListed()
        {
            // 付加された特殊能力は特殊ステータス側で表示するので一覧に含めない
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("分身");
            var u = fx.Build();
            u.AddCondition("超回避付加", 3, 1d, "");
            u.Update();

            Assert.AreEqual(1, u.AdditionalFeaturesNum);
            CollectionAssert.AreEqual(new[] { "分身" },
                u.StatusFeatures().Select(x => x.Name).ToArray());
        }
    }
}
