using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.ExecuteAbility の変身で、変身先の追加パイロットを前もって作るところのテスト（#1404）。
    /// VB 版 Unit.cls の ExecuteAbility は、変身先のユニットデータ（UDList.Item）の追加パイロットを作る。
    /// </summary>
    [TestClass]
    public class UnitAbilityTransformPilotTests
    {
        [TestMethod]
        public void Transform_CreatesAdditionalPilotOfTransformedUnit()
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("変身前");
            var ad = ud.AddAbility("変身");
            ad.SetEffect("変身Lv3=変身後");
            ad.Class = "";
            ad.MinRange = 1;
            ad.MaxRange = 1;
            var tud = fx.AddUnitData("変身後");
            tud.AddFeature("追加パイロット=変身パイロット");
            fx.AddPilotData("変身パイロット");
            // 対象は変身済み（ノーマルモードを持つ）なので、変身そのものは起きない。前もって作るところだけを見る
            fx.AddUnitData("変身済み").AddFeature("ノーマルモード=変身前");
            var user = fx.CreateUnit("変身前", "使用者");
            var target = fx.CreateUnit("変身済み", "対象者");

            // 元は対象のユニット（追加パイロットを持たない）の特殊能力を見て、空の名前のパイロットを作ろうとして止まっていた
            user.ExecuteAbility(user.Ability(1), target);

            Assert.IsTrue(fx.Src.PList.IsDefined("変身パイロット"));
            Assert.AreEqual("変身済み", target.CurrentForm().Name);
        }
    }
}
