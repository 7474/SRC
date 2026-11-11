using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.Update の前半（元の SRC の Unit.cls の Sub Update の 1250〜2336 行。ランクアップ・特殊能力・
    /// パイロット能力付加・能力値・地形適応・耐性・武器属性）を元の SRC と突き合わせる（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class UnitUpdateFirstHalfVBTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static UnitData AddUnitData(SRC src, string name, string transportation = "陸")
        {
            var ud = src.UDList.Add(name);
            ud.Class = "汎用";
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 500;
            ud.Mobility = 50;
            ud.Speed = 4;
            ud.Transportation = transportation;
            ud.Adaption = "AAAA";
            return ud;
        }

        private static Unit CreateUnit(SRC src, string uname)
        {
            var unit = src.UList.Add(uname, 0, "味方");
            src.PDList.Add("テストパイロット");
            var pilot = src.PList.Add("テストパイロット", 1, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            return unit;
        }

        [TestMethod]
        public void RankUp_NecessarySkillGrantedByPilotSkillGrant_RanksUpInSameUpdate()
        {
            // 元の SRC はパイロット能力付加のあとでランクアップを調べ直し、できるようになっていれば
            // 最初からやり直す（Unit.cls 1667〜1676 行）。付加した技能を必要技能にするランクアップは
            // 1 回の Update で行われる
            var src = CreateSrc();
            var udA = AddUnitData(src, "ランクアップ前");
            udA.AddFeature("ランクアップLv1=ランクアップ後 (超感覚)");
            udA.AddFeature("パイロット能力付加=超感覚");
            AddUnitData(src, "ランクアップ後");
            var unit = CreateUnit(src, "ランクアップ前");
            Assert.AreEqual("ランクアップ前", unit.Data.Name);

            // 前の Update で付加した状態を消して、付加が無いところから 1 回だけ Update する
            unit.DeleteCondition("超感覚付加２");
            unit.Rank = 1;

            Assert.AreEqual("ランクアップ後", unit.Data.Name);
        }

        [TestMethod]
        public void Item_AvailabilityChangedBySkillGrant_IsReactivatedInSameUpdate()
        {
            // 元の SRC はパイロット能力付加のあとでアイテムの使用可否を調べ直し、変わっていれば
            // 最初からやり直す（Unit.cls 1652〜1665 行）
            var src = CreateSrc();
            var ud = AddUnitData(src, "テスト機体");
            ud.ItemNum = 4;
            ud.AddFeature("パイロット能力付加=超感覚");
            var unit = CreateUnit(src, "テスト機体");

            var id = src.IDList.Add("テスト装備");
            id.Class = "汎用";
            id.Part = "強化パーツ";
            id.HP = 300;
            id.AddFeature("必要技能=超感覚");
            var itm = src.IList.Add("テスト装備");
            unit.AddItem0(itm);

            unit.DeleteCondition("超感覚付加２");
            itm.Activated = false;
            unit.Update();

            Assert.IsTrue(itm.Activated);
            Assert.AreEqual(1300, unit.MaxHP);
        }

        private static void SetupMap(SRC src, string terrainClass)
        {
            src.Map.MapWidth = 3;
            src.Map.MapHeight = 3;
            src.Map.MapDataForUnit = new Src2DArray<Unit>(3, 3);
            src.Map.MapData = new Src2DArray<MapCell>(3, 3);
            var td = new TerrainData { Class = terrainClass };
            src.Map.MapData[2, 2] = new MapCell { UnderTerrain = td, UpperTerrain = td };
        }

        [TestMethod]
        [DataRow("陸", "陸", "地上")]
        [DataRow("屋内", "陸", "地上")]
        [DataRow("水", "陸", "水中")]
        [DataRow("深水", "水上", "水上")]
        [DataRow("陸", "空陸", "空中")]
        public void Update_AirborneUnitThatCannotFly_LandsOnTerrain(string terrainClass, string transportation, string expectedArea)
        {
            // 元の SRC は空を飛べなくなったユニットを地形に合わせて地上・水上・水中に戻す（Unit.cls 2049〜2069 行）
            var src = CreateSrc();
            AddUnitData(src, "テスト機体", transportation);
            var unit = CreateUnit(src, "テスト機体");
            SetupMap(src, terrainClass);
            unit.x = 2;
            unit.y = 2;
            unit.Status = "出撃";
            unit.Area = "空中";

            unit.Update(true);

            Assert.AreEqual(expectedArea, unit.Area);
        }

        [TestMethod]
        public void Update_AirborneUnitNotOnMap_KeepsArea()
        {
            // 出撃していないユニットはそのまま
            var src = CreateSrc();
            AddUnitData(src, "テスト機体");
            var unit = CreateUnit(src, "テスト機体");
            SetupMap(src, "陸");
            unit.x = 2;
            unit.y = 2;
            unit.Status = "待機";
            unit.Area = "空中";

            unit.Update(true);

            Assert.AreEqual("空中", unit.Area);
        }

        [DataRow(false, 1)]
        [DataRow(true, 0)]
        [TestMethod]
        public void Update_BitmapChanged_RepaintsUnitUnlessWithoutRefresh(bool withoutRefresh, int expectedPaints)
        {
            // 元の SRC はユニット画像のファイル名が変わったら、出撃中のユニットを描き直す（Unit.cls 1684〜1698 行）
            var src = CreateSrc();
            AddUnitData(src, "テスト機体");
            var unit = CreateUnit(src, "テスト機体");
            SetupMap(src, "陸");
            src.Map.MapFileName = "test.map";
            unit.x = 2;
            unit.y = 2;
            unit.Status = "出撃";
            var paints = 0;
            ((MockGUI)src.GUI).PaintUnitBitmapHandler = (u, smode) => paints++;

            unit.AddCondition("ユニット画像付加", -1, Constants.DEFAULT_LEVEL, "new.bmp");
            unit.Update(withoutRefresh);

            Assert.AreEqual("new.bmp", unit.get_Bitmap(false));
            Assert.AreEqual(expectedPaints, paints);

            // 変わらなければ描き直さない
            paints = 0;
            unit.Update();
            Assert.AreEqual(0, paints);
        }
    }
}
