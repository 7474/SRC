using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.Rest（ターン経過によるステータス回復。元の SRC の Unit.Rest）、
    /// Unit.MaxAction、Unit.CheckAutoHyperMode
    /// </summary>
    [TestClass]
    public class UnitRestTests
    {
        private static MockGUI CreateGUI()
        {
            return new MockGUI
            {
                CenterHandler = (x, y) => { },
                RedrawScreenHandler = (late) => { },
                RefreshScreenHandler = (a, b) => { },
                EraseUnitBitmapHandler = (x, y, r) => { },
                PaintUnitBitmapHandler = (u, m) => { },
                OpenMessageFormHandler = (u1, u2) => { },
                CloseMessageFormHandler = () => { },
                DisplaySysMessageHandler = (m, b) => { },
                UnlockGUIHandler = () => { },
                LockGUIHandler = () => { },
                DisplayTelopHandler = (m) => { },
                SleepHandler = (ms, e) => { },
                ResetDrawStringHandler = () => { },
                UpdateScreenHandler = () => { },
                MainFormShowHandler = () => { },
            };
        }

        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = CreateGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            src.Titles = new List<string>();
            return src;
        }

        private static UnitData AddUnitData(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            return ud;
        }

        private static (SRC src, Unit unit) Create(params string[] features)
        {
            var src = CreateSrc();
            // 味方ステージの1ターン目は回復しないため2ターン目とする
            src.Stage = "味方";
            src.Turn = 2;
            var ud = AddUnitData(src, "テスト機体");
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            src.PDList.Add("テストパイロット");
            var unit = src.UList.Add("テスト機体", 0, "味方");
            var pilot = src.PList.Add("テストパイロット", 1, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return (src, unit);
        }

        [TestMethod]
        public void Rest_RecoversEN5PerTurn()
        {
            var (_, unit) = Create();
            unit.EN = 50;

            unit.Rest();

            Assert.AreEqual(55, unit.EN);
        }

        [TestMethod]
        public void Rest_FirstTurnOfPlayerStage_DoesNotRecover()
        {
            var (src, unit) = Create();
            src.Turn = 1;
            unit.EN = 50;

            unit.Rest();

            Assert.AreEqual(50, unit.EN);
        }

        [TestMethod]
        public void Rest_ENRecoveryDisabledByOption()
        {
            var (src, unit) = Create();
            src.Expression.DefineGlobalVariable("Option(ＥＮ自然回復無効)");
            src.Expression.SetVariableAsLong("Option(ＥＮ自然回復無効)", 1);
            unit.EN = 50;

            unit.Rest();

            Assert.AreEqual(50, unit.EN);
        }

        [TestMethod]
        public void Rest_HPRecoveryFeature_RecoversHPPerTurn()
        {
            // ＨＰ回復Lv1 は毎ターン最大ＨＰの 10% を回復
            var (_, unit) = Create("ＨＰ回復Lv1");
            unit.HP = 500;

            unit.Rest();

            Assert.AreEqual(600, unit.HP);
        }

        [TestMethod]
        public void Rest_ENRecoveryFeature_RecoversENPerTurn()
        {
            // ＥＮ回復Lv2 は毎ターン最大ＥＮの 20% と自然回復の 5 を回復
            var (_, unit) = Create("ＥＮ回復Lv2");
            unit.EN = 10;

            unit.Rest();

            Assert.AreEqual(35, unit.EN);
        }

        [TestMethod]
        public void Rest_HPRecoveryFeature_DisabledByRecoveryBlock()
        {
            var (_, unit) = Create("ＨＰ回復Lv1");
            unit.HP = 500;
            unit.EN = 50;
            unit.AddCondition("回復不能", 3);

            unit.Rest();

            Assert.AreEqual(500, unit.HP);
            Assert.AreEqual(50, unit.EN);
        }

        [TestMethod]
        public void Rest_Poison_DecreasesHP()
        {
            // 毒は毎ターン最大ＨＰの 10% を減らす
            var (_, unit) = Create();
            unit.HP = 500;
            unit.AddCondition("毒", 3);

            unit.Rest();

            Assert.AreEqual(400, unit.HP);
            Assert.AreEqual(2, unit.ConditionLifetime("毒"));
        }

        [TestMethod]
        public void Rest_Poison_DoesNotKill()
        {
            var (_, unit) = Create();
            unit.HP = 50;
            unit.AddCondition("毒", 3);

            unit.Rest();

            Assert.AreEqual(1, unit.HP);
        }

        [TestMethod]
        public void Rest_ENConsumption_ToZero_SetsENZero()
        {
            var (_, unit) = Create("ＥＮ消費Lv5");
            unit.EN = 10;

            unit.Rest();

            Assert.AreEqual(0, unit.EN);
        }

        [TestMethod]
        public void Rest_Stored_RecoversHalfAndFullBullet()
        {
            var (_, unit) = Create();
            unit.Status = "格納";
            unit.HP = 100;
            unit.EN = 10;

            unit.Rest();

            Assert.AreEqual(600, unit.HP);
            Assert.AreEqual(65, unit.EN);
        }

        [TestMethod]
        public void Rest_ResetsUsedActionAndSupportCounters()
        {
            var (_, unit) = Create();
            unit.UsedAction = 1;
            unit.UsedSupportAttack = 2;
            unit.UsedCounterAttack = 1;

            unit.Rest();

            Assert.AreEqual(0, unit.UsedAction);
            Assert.AreEqual(0, unit.UsedSupportAttack);
            Assert.AreEqual(0, unit.UsedCounterAttack);
        }

        [TestMethod]
        public void Rest_TerrainHPRecovery_RecoversHP()
        {
            var (src, unit) = Create();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ回復Lv2");
            unit.StandBy(2, 2);
            unit.HP = 500;

            unit.Rest();

            Assert.AreEqual(700, unit.HP);
        }

        [TestMethod]
        public void Rest_TerrainCondition_AddsCondition()
        {
            var (src, unit) = Create();
            var td = CreateMap(src);
            td.AddFeature("状態付加Lv2=毒");
            unit.StandBy(2, 2);

            unit.Rest();

            Assert.IsTrue(unit.IsConditionSatisfied("毒"));
        }

        [TestMethod]
        public void MaxAction_Normal_IsOne()
        {
            var (_, unit) = Create();

            Assert.AreEqual(1, unit.MaxAction());
        }

        [TestMethod]
        public void MaxAction_Paralysis_IsZero()
        {
            var (_, unit) = Create();
            unit.AddCondition("麻痺", 1);

            Assert.AreEqual(0, unit.MaxAction());
        }

        [TestMethod]
        public void MaxAction_ENZero_IsZeroUnlessIgnored()
        {
            var (_, unit) = Create();
            unit.EN = 0;

            Assert.AreEqual(0, unit.MaxAction());
            Assert.AreEqual(1, unit.MaxAction(true));
        }

        [TestMethod]
        public void MaxAction_ENZero_ActionableWithOption()
        {
            var (src, unit) = Create();
            src.Expression.DefineGlobalVariable("Option(ＥＮ０時行動可)");
            src.Expression.SetVariableAsLong("Option(ＥＮ０時行動可)", 1);
            unit.EN = 0;

            Assert.AreEqual(1, unit.MaxAction());
        }

        [TestMethod]
        public void MaxAction_NoPilot_IsZero()
        {
            var src = CreateSrc();
            AddUnitData(src, "無人機");
            var unit = src.UList.Add("無人機", 0, "味方");
            unit.Update();
            unit.FullRecover();

            Assert.AreEqual(0, unit.MaxAction());
        }

        [TestMethod]
        public void CheckAutoHyperMode_LowHP_TransformsToHyperForm()
        {
            var src = CreateSrc();
            AddUnitData(src, "テスト機体改").AddFeature("ノーマルモード=テスト機体");
            AddUnitData(src, "テスト機体").AddFeature("ハイパーモード=ハイパーモード テスト機体改 自動発動");
            src.PDList.Add("テストパイロット");
            var unit = src.UList.Add("テスト機体", 0, "味方");
            var pilot = src.PList.Add("テストパイロット", 1, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            CreateMap(src);
            unit.StandBy(2, 2);

            // 気力が足りず、ＨＰも十分ある場合は発動しない
            unit.CheckAutoHyperMode();
            Assert.AreEqual("テスト機体", unit.CurrentForm().Name);

            // ＨＰが 1/4 以下になると発動する
            unit.HP = 200;
            unit.CheckAutoHyperMode();
            Assert.AreEqual("テスト機体改", unit.CurrentForm().Name);
            Assert.AreEqual("出撃", unit.CurrentForm().Status);
        }

        private static TerrainData CreateMap(SRC src)
        {
            var td = new TerrainData
            {
                ID = 1,
                Name = "平地",
                Class = "陸",
                MoveCost = 2,
            };
            var map = src.Map;
            map.MapFileName = "test.map";
            map.MapWidth = 3;
            map.MapHeight = 3;
            map.MapData = new Src2DArray<MapCell>(3, 3);
            foreach (var x in map.MapData.XRange)
            {
                foreach (var y in map.MapData.YRange)
                {
                    map.MapData[x, y] = new MapCell
                    {
                        X = x,
                        Y = y,
                        UnderTerrain = td,
                        BoxType = BoxTypes.Under,
                    };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(3, 3);
            map.MaskData = new Src2DArray<bool>(3, 3);
            return td;
        }
    }
}
