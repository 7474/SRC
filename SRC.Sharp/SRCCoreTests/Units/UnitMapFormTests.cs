using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Lib;
using System;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 配置・変形・合体処理（Unit.map.cs）のテスト
    /// </summary>
    [TestClass]
    public class UnitMapFormTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI
                {
                    MainWidth = 10,
                    MainHeight = 10,
                },
            };
            // マップファイルなし（ステータス表示用の空マップ）
            // SetMapSize は GUIMap を必要とするため配列を直接用意する
            var map = src.Map;
            map.MapFileName = "";
            map.MapWidth = 10;
            map.MapHeight = 10;
            map.MapData = new Src2DArray<MapCell>(map.MapWidth, map.MapHeight);
            for (var x = 1; x <= map.MapWidth; x++)
            {
                for (var y = 1; y <= map.MapHeight; y++)
                {
                    map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(map.MapWidth, map.MapHeight);
            return src;
        }

        private UnitData AddUnitData(SRC src, string name, int hp = 1000, int en = 100, params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = hp;
            ud.EN = en;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        [TestMethod]
        public void StandBy_StoredUnit_IsUnloadedFromCarrier()
        {
            var src = CreateSrc();
            AddUnitData(src, "母艦");
            AddUnitData(src, "搭載機");
            var carrier = src.UList.Add("母艦", 0, "味方");
            var u = src.UList.Add("搭載機", 0, "味方");

            u.Status = "格納";
            carrier.LoadUnit(u);
            Assert.AreEqual(1, carrier.CountUnitOnBoard());

            u.StandBy(2, 2);

            Assert.AreEqual("出撃", u.Status);
            Assert.AreEqual(0, carrier.CountUnitOnBoard());
            Assert.AreSame(u, src.Map.MapDataForUnit[2, 2]);
        }

        [TestMethod]
        public void StandBy_NotStoredUnit_DoesNotThrowWithoutCarrier()
        {
            var src = CreateSrc();
            AddUnitData(src, "単独機");
            var u = src.UList.Add("単独機", 0, "味方");
            u.Status = "格納";

            u.StandBy(3, 3);

            Assert.AreEqual("出撃", u.Status);
        }

        [TestMethod]
        public void Transform_KeepsPermanentAndTimedConditions()
        {
            var src = CreateSrc();
            AddUnitData(src, "形態A", features: "変形=変形 形態B");
            AddUnitData(src, "形態B", features: "変形=変形 形態A");
            var ua = src.UList.Add("形態A", 0, "味方");
            ua.Status = "待機";
            var ub = ua.OtherForm("形態B");
            Assert.IsNotNull(ub);

            ua.AddCondition("永続状態", -1, cdata: "");
            ua.AddCondition("時限状態", 3, cdata: "");

            ua.Transform("形態B");

            Assert.AreEqual("待機", ub.Status);
            Assert.AreEqual("他形態", ua.Status);
            Assert.IsTrue(ub.IsConditionSatisfied("永続状態"));
            Assert.IsTrue(ub.IsConditionSatisfied("時限状態"));
            Assert.AreEqual(-1, ub.ConditionLifetime("永続状態"));
            Assert.AreEqual(3, ub.ConditionLifetime("時限状態"));
            Assert.AreEqual(0, ua.CountCondition());
        }

        [TestMethod]
        public void Transform_TransfersServantsAndSlaves()
        {
            var src = CreateSrc();
            AddUnitData(src, "形態A", features: "変形=変形 形態B");
            AddUnitData(src, "形態B", features: "変形=変形 形態A");
            AddUnitData(src, "召喚機");
            AddUnitData(src, "隷属機");
            var ua = src.UList.Add("形態A", 0, "味方");
            ua.Status = "待機";
            var servant = src.UList.Add("召喚機", 0, "味方");
            var slave = src.UList.Add("隷属機", 0, "敵");
            ua.AddServant(servant);
            ua.AddSlave(slave);

            ua.Transform("形態B");

            var ub = ua.OtherForm("形態B");
            Assert.AreEqual(1, ub.CountServant());
            Assert.AreSame(servant, ub.Servant(servant.ID));
            Assert.AreEqual(1, ub.CountSlave());
            Assert.AreSame(slave, ub.Slave(slave.ID));
            Assert.AreEqual(0, ua.CountServant());
            Assert.AreEqual(0, ua.CountSlave());
        }

        [TestMethod]
        public void DeleteSlave_RemovesSlaveOnlyByIdOrName()
        {
            var src = CreateSrc();
            AddUnitData(src, "主機");
            AddUnitData(src, "召喚機");
            AddUnitData(src, "隷属機");
            var master = src.UList.Add("主機", 0, "味方");
            var servant = src.UList.Add("召喚機", 0, "味方");
            var slave = src.UList.Add("隷属機", 0, "敵");
            var slave2 = src.UList.Add("隷属機", 0, "敵");
            master.AddServant(servant);
            master.AddSlave(slave);
            master.AddSlave(slave2);

            // 召喚ユニットの ID を指定しても召喚ユニットは消えない（元の SRC の DeleteSlave は隷属ユニットだけを扱う）
            master.DeleteSlave(servant.ID);
            Assert.AreEqual(1, master.CountServant());
            Assert.AreEqual(2, master.CountSlave());

            // ID で消す
            master.DeleteSlave(slave.ID);
            Assert.AreEqual(1, master.CountSlave());
            Assert.AreSame(slave2, master.Slave(slave2.ID));

            // 名前で消す
            master.DeleteSlave("隷属機");
            Assert.AreEqual(0, master.CountSlave());
            Assert.AreEqual(1, master.CountServant());
        }

        [TestMethod]
        public void Transform_CarriesOverHpRatioWithVbRounding()
        {
            var src = CreateSrc();
            AddUnitData(src, "形態A", hp: 1000, en: 100, features: "変形=変形 形態B");
            AddUnitData(src, "形態B", hp: 1001, en: 101, features: "変形=変形 形態A");
            var ua = src.UList.Add("形態A", 0, "味方");
            ua.Status = "待機";
            ua.HP = 750;
            ua.EN = 50;

            ua.Transform("形態B");

            var ub = ua.OtherForm("形態B");
            // 1001 * 75 / 100 = 750.75 → 751, 101 * 50 / 100 = 50.5 → 50（偶数丸め）
            Assert.AreEqual(751, ub.HP);
            Assert.AreEqual(50, ub.EN);
        }

        [TestMethod]
        public void Combine_SetsAverageHpRatioAndMovesUnitsOnBoard()
        {
            var src = CreateSrc();
            AddUnitData(src, "合体機", hp: 3001, en: 200, features: "分離=分離 パーツA パーツB");
            AddUnitData(src, "パーツA", hp: 1000, en: 100, features: "合体=合体 合体機 パーツB");
            AddUnitData(src, "パーツB", hp: 1000, en: 100, features: "合体=合体 合体機 パーツA");
            AddUnitData(src, "搭載機");
            var combined = src.UList.Add("合体機", 0, "味方");
            combined.Status = "他形態";
            var pa = src.UList.Add("パーツA", 0, "味方");
            var pb = src.UList.Add("パーツB", 0, "味方");
            pa.Status = "待機";
            pb.Status = "待機";
            pa.HP = 500;
            pb.HP = 1000;
            pa.EN = 100;
            pb.EN = 50;

            var stored = src.UList.Add("搭載機", 0, "味方");
            stored.Status = "格納";
            pb.LoadUnit(stored);

            pa.Combine("合体機", true);

            Assert.AreEqual("待機", combined.Status);
            Assert.AreEqual("旧主形態", pa.Status);
            Assert.AreEqual("旧形態", pb.Status);
            // HP: 3001 * (50 + 100) / 100 / 2 = 2250.75 → 2251
            Assert.AreEqual(2251, combined.HP);
            // EN: 200 * (100 + 50) / 100 / 2 = 150
            Assert.AreEqual(150, combined.EN);
            // 搭載ユニットは各パーツから乗せ換えられる
            Assert.AreEqual(1, combined.CountUnitOnBoard());
            Assert.AreEqual(0, pb.CountUnitOnBoard());
        }
    }
}
