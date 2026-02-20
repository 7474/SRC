using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.VB;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitMainPilotTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        // Helper: directly add a feature (name=data) to unit's colFeature without calling Update()
        private static void AddFeatureToUnit(Unit unit, string name, string data = "")
        {
            var field = typeof(Unit).GetField("colFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var colFeature = (SrcCollection<FeatureData>)field.GetValue(unit);
            var fd = new FeatureData { Name = name, StrData = data };
            if (!colFeature.ContainsKey(name))
            {
                colFeature.Add(fd, name);
            }
        }

        // Helper: create a pilot (with pilot data) and add it to a unit
        private static Pilots.Pilot CreateAndBoardPilot(SRC src, Unit unit, string name, int level = 1)
        {
            if (!src.PDList.IsDefined(name))
            {
                src.PDList.Add(name);
            }
            var pilot = src.PList.Add(name, level, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            return pilot;
        }

        // ===== MainPilot() — normal (no special feature) =====

        [TestMethod]
        public void MainPilot_ReturnsPilot1_WhenNoAdditionalPilotFeature()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 5);

            var result = unit.MainPilot();

            Assert.AreSame(pilot, result);
        }

        [TestMethod]
        public void MainPilot_ThrowsTerminateException_WhenNoPilotAndNoAdditionalPilotFeature()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.ThrowsException<Exceptions.TerminateException>(() => unit.MainPilot());
        }

        // ===== MainPilot() — 能力コピー condition =====

        [TestMethod]
        public void MainPilot_ReturnsPilot1_WhenNoryokuCopyConditionIsActive()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "パイロット1", level: 3);
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");
            unit.AddCondition("能力コピー", -1);

            var result = unit.MainPilot();

            // 能力コピー中は追加パイロットを使用しない → pilot1 が返る
            Assert.AreSame(pilot, result);
        }

        // ===== MainPilot() — 追加パイロット feature =====

        [TestMethod]
        public void MainPilot_CreatesAdditionalPilot_WhenFeaturePresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var mainPilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 7);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            var result = unit.MainPilot();

            Assert.AreEqual("追加パイロット", result.Name);
            Assert.IsTrue(result.IsAdditionalPilot);
            // Level should match the main pilot
            Assert.AreEqual(7, result.Level);
        }

        [TestMethod]
        public void MainPilot_ReturnsCachedAdditionalPilot_WhenCalledTwice()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "基本パイロット", level: 5);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            var first = unit.MainPilot();
            var second = unit.MainPilot();

            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void MainPilot_AdditionalPilotLevelMatchesMainPilot()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "基本パイロット", level: 20);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            var addPilot = unit.MainPilot();

            Assert.AreEqual(20, addPilot.Level);
        }

        [TestMethod]
        public void MainPilot_CreatesAdditionalPilotAtLevel1_WhenNoPilotBoarded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");
            // No pilot boarded on unit

            var result = unit.MainPilot();

            Assert.AreEqual("追加パイロット", result.Name);
            Assert.AreEqual(1, result.Level);
        }

        [TestMethod]
        public void MainPilot_UsesExistingPilotFromPList_WhenAdditionalPilotAlreadyCreated()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "基本パイロット", level: 10);
            src.PDList.Add("追加パイロット");
            // Pre-create the additional pilot in PList
            var preCreated = src.PList.Add("追加パイロット", 1, "味方");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            var result = unit.MainPilot();

            Assert.AreSame(preCreated, result);
            Assert.IsTrue(result.IsAdditionalPilot);
            Assert.AreEqual(10, result.Level);
        }

        // ===== MainPilot() — 暴走時パイロット feature =====

        [TestMethod]
        public void MainPilot_ReturnsBerserkPilot_WhenBerserkConditionAndFeaturePresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "基本パイロット", level: 8);
            src.PDList.Add("暴走パイロット");
            AddFeatureToUnit(unit, "暴走時パイロット", "暴走パイロット");
            unit.AddCondition("暴走", -1);

            var result = unit.MainPilot();

            Assert.AreEqual("暴走パイロット", result.Name);
        }

        [TestMethod]
        public void MainPilot_BerserkPilotLevelMatchesMainPilot()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "基本パイロット", level: 15);
            src.PDList.Add("暴走パイロット");
            AddFeatureToUnit(unit, "暴走時パイロット", "暴走パイロット");
            unit.AddCondition("暴走", -1);

            var result = unit.MainPilot();

            Assert.AreEqual(15, result.Level);
        }

        [TestMethod]
        public void MainPilot_ReturnsPilot1_WhenBerserkFeatureAbsentDespiteCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 5);
            // 暴走 condition but no 暴走時パイロット feature
            unit.AddCondition("暴走", -1);

            var result = unit.MainPilot();

            Assert.AreSame(pilot, result);
        }

        // ===== MainPilot() — 破棄 status =====

        [TestMethod]
        public void MainPilot_ReturnsPilot1_WhenStatusIsHaiki()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Status = "破棄" };
            var pilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 3);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            var result = unit.MainPilot();

            // 破棄時は追加パイロットを使用しない → pilot1 が返る
            Assert.AreSame(pilot, result);
        }

        // ===== MainPilot() — 暴走時パイロット: pilot data not found =====

        [TestMethod]
        public void MainPilot_ReturnsPilot1_WhenBerserkPilotDataNotFound()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 5);
            // 暴走時パイロット feature with undefined pilot name
            AddFeatureToUnit(unit, "暴走時パイロット", "存在しないパイロット");
            unit.AddCondition("暴走", -1);

            // Should fall back to main pilot without crashing
            var result = unit.MainPilot();

            Assert.AreSame(pilot, result);
        }

        // ===== MainPilot() — 追加パイロット: level sync on repeated calls =====

        [TestMethod]
        public void MainPilot_SyncsLevelOnRepeatedCalls()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var basePilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 1);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            // First call creates the additional pilot
            var result1 = unit.MainPilot();
            Assert.AreEqual(1, result1.Level);

            // Advance the base pilot's level (simulating ExpUpCmd)
            basePilot.Level = 10;

            // Second call should sync level from base pilot
            var result2 = unit.MainPilot();
            Assert.AreEqual(10, result2.Level);
        }

        [TestMethod]
        public void MainPilot_SyncsExpOnRepeatedCalls()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var basePilot = CreateAndBoardPilot(src, unit, "基本パイロット", level: 1);
            src.PDList.Add("追加パイロット");
            AddFeatureToUnit(unit, "追加パイロット", "追加パイロット");

            // First call creates the additional pilot
            var result1 = unit.MainPilot();
            Assert.AreEqual(0, result1.Exp);

            // Give the base pilot experience
            basePilot.Exp = basePilot.Exp + 200;

            // Second call should sync exp from base pilot
            var result2 = unit.MainPilot();
            Assert.AreEqual(200, result2.Exp);
        }
    }
}
