using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Linq;
using static SRCCore.Coverage.Tests.UnitItem.UnitItemTestHelper;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.cls の ExecuteMapAbility・UseAbility・DeleteItem を、元の SRC と同じ動作にしたことのテスト（7474/SRC#1502）
    /// </summary>
    [TestClass]
    public class UnitMapAbilityVBMatchTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        private void SetupMap(int width, int height)
        {
            var plain = new TerrainData { ID = 0, Name = "平地", Bitmap = "plain", Class = "陸", MoveCost = 2 };
            var map = fx.Src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = plain, UpperTerrain = plain };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
            map.TotalMoveCost = new int[width + 2, height + 2];
            map.PointInZOC = new int[width + 2, height + 2];
        }

        // ──────────────────────────────────────────────
        // UseAbility
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UseAbility_Simultaneous_TakesIntegerQuotientAndKeepsSmallerStock()
        {
            // VB: SetStock i, MinLng(MaxStock(i) * Stock(a) \ MaxStock(a), Stock(i))
            // 「斉」を 4 → 3 回に減らすと、最大 5 回のものは 5 * 3 \ 4 = 3、
            // 残り 1 回（最大 3 回）のものは 3 * 3 \ 4 = 2 と 1 の小さいほうで 1 のまま
            // （残りの割合 0.75 を写すと 4 回・2 回に増えていた）
            var ud = fx.AddUnitData("使用機");
            var ad1 = ud.AddAbility("一斉");
            ad1.SetEffect("回復Lv1");
            ad1.Class = "斉";
            ad1.Stock = 4;
            var ad2 = ud.AddAbility("五回");
            ad2.SetEffect("回復Lv1");
            ad2.Stock = 5;
            var ad3 = ud.AddAbility("三回");
            ad3.SetEffect("回復Lv1");
            ad3.Stock = 3;
            var u = fx.CreateUnit("使用機", "使用者");
            u.Ability(3).SetStock(1);

            u.Ability(1).UseAbility();

            Assert.AreEqual(3, u.Ability(1).Stock());
            Assert.AreEqual(3, u.Ability(2).Stock());
            Assert.AreEqual(1, u.Ability(3).Stock());
        }

        [TestMethod]
        public void UseAbility_EternalClass_StillConsumesStock()
        {
            // VB の UseAbility は「永」を見ない（「永」が効くのは武器の弾数だけ）
            var ud = fx.AddUnitData("使用機");
            var ad = ud.AddAbility("永続");
            ad.SetEffect("回復Lv1");
            ad.Class = "永";
            ad.Stock = 3;
            var u = fx.CreateUnit("使用機", "使用者");

            u.Ability(1).UseAbility();

            Assert.AreEqual(2, u.Ability(1).Stock());
        }

        // ──────────────────────────────────────────────
        // ExecuteMapAbility
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExecuteMapAbility_TargetsWithSameDistance_OrderedBySwapSelectionSort()
        {
            // VB は起点からの距離で選択ソート（入れ替え）する。
            // 列順の一覧 [A(4,5) 距離1, B(5,4) 距離1, 自分(5,5) 距離0] は
            // 先頭と自分が入れ替わって [自分, B, A] になる（安定な並べ替えなら [自分, A, B]）
            SetupMap(8, 8);
            var ud = fx.AddUnitData("支援機");
            var ad = ud.AddAbility("全体修理");
            ad.SetEffect("回復Lv1");
            ad.Class = "Ｍ全";
            ad.MinRange = 1;
            ad.MaxRange = 2;
            fx.AddUnitData("僚機");
            var user = fx.CreateUnit("支援機", "支援者");
            var a = fx.CreateUnit("僚機", "Ａ");
            var b = fx.CreateUnit("僚機", "Ｂ");
            user.StandBy(5, 5);
            a.StandBy(4, 5);
            b.StandBy(5, 4);
            foreach (var u in new[] { user, a, b })
            {
                u.HP = 500;
            }

            user.ExecuteMapAbility(user.Ability(1), 5, 5, true);

            // 各ユニットにアビリティを使う前の UpdateMessageForm(t, 使用者) の順で確かめる
            var order = fx.Gui.CallsOf(nameof(IGUI.UpdateMessageForm))
                .Where(x => x.Args.Length >= 2 && ReferenceEquals(x.Args[1], user))
                .Select(x => (Unit)x.Args[0])
                .Distinct()
                .ToList();
            CollectionAssert.AreEqual(new[] { b, a }, order);
            Assert.AreEqual(1000, a.HP);
            Assert.AreEqual(1000, b.HP);
        }

        // ──────────────────────────────────────────────
        // DeleteItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteItem_DuplicateItems_RemainingWeaponKeepsItsOwnBullets()
        {
            // VB の DeleteItem は外すアイテムが付加した武器の WData(i) を Nothing にしてから Update するので、
            // 同じアイテムを２つ装備しているとき、残ったほうの武器は自分の残弾数のまま
            // （武器データの参照で引き継ぐと、外したほうの残弾数が引き継がれていた）
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var rifle1 = CreateItem(src, "ライフル", "アイテム");
            var wd = src.IDList.Item("ライフル").AddWeapon("ライフル");
            wd.Power = 1000;
            wd.MinRange = 1;
            wd.MaxRange = 3;
            wd.Bullet = 3;
            var rifle2 = src.IList.Add("ライフル");
            u.AddItem(rifle1);
            u.AddItem(rifle2);
            Assert.AreEqual(2, u.CountWeapon());
            u.Weapon(1).SetBulletFull();
            u.Weapon(2).SetBullet(1);

            u.DeleteItem(rifle1);

            Assert.AreEqual(1, u.CountWeapon());
            Assert.AreEqual(1, u.Weapon(1).Bullet());
        }
    }
}
