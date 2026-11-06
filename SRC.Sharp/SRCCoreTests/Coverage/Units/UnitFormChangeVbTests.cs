using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 合体・分離・自爆（Unit.cls の Combine・Split・SuicidalExplosion）を元の SRC の動作と突き合わせたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class UnitFormChangeVbTests
    {
        private static Item CreateItem(UnitMapActionFixture f, string name)
        {
            if (!f.Src.IDList.IsDefined(name))
            {
                var id = f.Src.IDList.Add(name);
                id.Nickname = name;
                id.Class = "汎用";
                id.Part = "強化パーツ";
            }
            return f.Src.IList.Add(name);
        }

        [TestMethod]
        public void Combine_NoName_PrefersPartnerNameOverOtherFormName()
        {
            // 元の SRC は、まず相手のユニット名が一致する「合体」を探し、無いときだけ
            // 相手の他形態の名前が一致する（IsEqual）ものを探す
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機X", features: "分離=分離 機体A 機体B");
            f.AddUnitData("合体機Y", features: "分離=分離 機体A 機体B");
            f.AddUnitData("機体A", features: new[] { "合体=合体 合体機X 機体B変形", "合体=合体 合体機Y 機体B" });
            f.AddUnitData("機体B", features: "変形=変形 機体B変形");
            f.AddUnitData("機体B変形", features: "変形=変形 機体B");
            var cx = f.CreateUnit("合体機X");
            var cy = f.CreateUnit("合体機Y");
            cx.Status = "他形態";
            cy.Status = "他形態";
            var a = f.CreateUnit("機体A");
            var b = f.CreateUnit("機体B");
            a.Status = "待機";
            b.Status = "待機";
            a.x = 3;
            a.y = 3;
            f.Map.MapDataForUnit[3, 3] = b;

            a.Combine();

            Assert.AreEqual("待機", cy.Status);
            Assert.AreEqual("他形態", cx.Status);
        }

        [TestMethod]
        public void Combine_NoName_SkipsCombineFeatureWithoutThreeItems()
        {
            // データが 3 つでない「合体」は飛ばす（元の SRC の LLength(fdata) = 3）
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機", features: "分離=分離 機体A 機体B");
            f.AddUnitData("機体A", features: new[] { "合体=合体", "合体=合体 合体機 機体B" });
            f.AddUnitData("機体B");
            var c = f.CreateUnit("合体機");
            c.Status = "他形態";
            var a = f.CreateUnit("機体A");
            var b = f.CreateUnit("機体B");
            a.Status = "待機";
            b.Status = "待機";
            a.x = 3;
            a.y = 3;
            f.Map.MapDataForUnit[3, 3] = b;

            a.Combine();

            Assert.AreEqual("待機", c.Status);
            Assert.AreEqual("旧主形態", a.Status);
            Assert.AreEqual("旧形態", b.Status);
        }

        [TestMethod]
        public void Split_FirstUnitEquipsItemsWithAddItem()
        {
            // 元の SRC は分離後の１番機に AddItem で装備させるので、装備個所が足りなければ前のアイテムを外す
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機", features: "分離=分離 機体A 機体B").ItemNum = 2;
            f.AddUnitData("機体A").ItemNum = 1;
            f.AddUnitData("機体B").ItemNum = 1;
            var c = f.CreateUnit("合体機");
            var a = f.CreateUnit("機体A");
            var b = f.CreateUnit("機体B");
            c.Status = "待機";
            a.Status = "旧主形態";
            b.Status = "旧形態";
            var item1 = CreateItem(f, "パーツ1");
            var item2 = CreateItem(f, "パーツ2");
            c.AddItem(item1);
            c.AddItem(item2);
            Assert.AreEqual(2, c.CountItem());

            c.Split();

            Assert.AreEqual(0, c.CountItem());
            Assert.AreEqual(1, a.CountItem());
            Assert.AreSame(item2, a.Item(1));
            Assert.IsNull(item1.Unit);
            Assert.AreSame(a, item2.Unit);
        }

        [TestMethod]
        public void Split_UsedActionFollowsSplitOrderNotFirstUnit()
        {
            // 元の SRC は、行動回数を受け継ぐのを１番機（旧主形態）ではなく分離先の並びの１番目で決める。
            // ２番目以降は行動回数を大きいほうに合わせ、援護などの回数を 0 にする
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機", features: "分離=分離 機体A 機体B");
            f.AddUnitData("機体A");
            f.AddUnitData("機体B");
            var a = f.CreateUnit("機体A");
            var b = f.CreateUnit("機体B");
            a.Status = "旧形態";
            b.Status = "旧主形態";
            a.UsedAction = 2;
            b.UsedAction = 0;
            var c = f.Deploy(f.CreateUnit("合体機"), 5, 5);
            c.UsedAction = 1;
            c.UsedSupportAttack = 3;

            c.Split();

            Assert.AreEqual("出撃", a.Status);
            Assert.AreEqual("出撃", b.Status);
            // 並びの１番目（機体A）は合体機の行動回数をそのまま受け継ぐ
            Assert.AreEqual(1, a.UsedAction);
            // １番機（機体B）は援護攻撃の回数を受け継いだあと、２番目なので 0 にされる
            Assert.AreEqual(1, b.UsedAction);
            Assert.AreEqual(0, b.UsedSupportAttack);
        }

        [TestMethod]
        public void SuicidalExplosion_PartsSplit_RestoresHpWithoutMarkingActed()
        {
            // 自爆のあとパーツ分離した場合、元の SRC は分離後の形態の HP を全快にするだけで、行動済みにはしない
            var f = new UnitMapActionFixture();
            f.AddUnitData("本体", hp: 1000, features: "パーツ分離=パーツ分離 コア");
            f.AddUnitData("コア", hp: 500);
            var bomber = f.Deploy(f.CreateUnit("本体", "味方", f.CreatePilot("自爆者")), 5, 5);
            bomber.HP = 300;
            bomber.UsedAction = 0;

            bomber.SuicidalExplosion(is_event: true);

            var core = bomber.OtherForm("コア");
            Assert.AreSame(core, f.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual(core.MaxHP, core.HP);
            Assert.AreEqual(0, core.UsedAction);
        }
    }
}
