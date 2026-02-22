using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.pilot.cs のパイロット関連処理の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitPilotMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static Pilot CreateAndBoardPilot(SRC src, Unit unit, string name, int level = 1)
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

        // ──────────────────────────────────────────────
        // CountPilot
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountPilot_NewUnit_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountPilot());
        }

        [TestMethod]
        public void CountPilot_AfterAddPilot_ReturnsOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "テストパイロット");
            Assert.AreEqual(1, unit.CountPilot());
        }

        [TestMethod]
        public void CountPilot_AfterTwoPilots_ReturnsTwo()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            CreateAndBoardPilot(src, unit, "パイロットA");
            CreateAndBoardPilot(src, unit, "パイロットB");
            Assert.AreEqual(2, unit.CountPilot());
        }

        // ──────────────────────────────────────────────
        // AddPilot
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddPilot_PilotIsAccessibleByID()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "アムロ");
            Assert.AreSame(pilot, unit.Pilot(pilot.ID));
        }

        [TestMethod]
        public void AddPilot_MultiplePilots_AllAccessible()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var p1 = CreateAndBoardPilot(src, unit, "パイロット1");
            var p2 = CreateAndBoardPilot(src, unit, "パイロット2");
            Assert.AreSame(p1, unit.Pilot(p1.ID));
            Assert.AreSame(p2, unit.Pilot(p2.ID));
        }

        // ──────────────────────────────────────────────
        // DeletePilot
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeletePilot_DecreasesCountByOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var p1 = CreateAndBoardPilot(src, unit, "削除パイロット1");
            CreateAndBoardPilot(src, unit, "削除パイロット2");
            Assert.AreEqual(2, unit.CountPilot());
            unit.DeletePilot(p1);
            Assert.AreEqual(1, unit.CountPilot());
        }

        [TestMethod]
        public void DeletePilot_SinglePilot_ResultsInZeroCount()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var p = CreateAndBoardPilot(src, unit, "唯一のパイロット");
            unit.DeletePilot(p);
            Assert.AreEqual(0, unit.CountPilot());
        }

        // ──────────────────────────────────────────────
        // MainPilot (normal cases)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MainPilot_ReturnsSinglePilot_WhenOnlyOnePilotBoarded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "メインパイロット");
            var result = unit.MainPilot();
            Assert.AreSame(pilot, result);
        }

        [TestMethod]
        public void MainPilot_CalledTwice_ReturnsSamePilot()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "繰り返しパイロット");
            var first = unit.MainPilot();
            var second = unit.MainPilot();
            Assert.AreSame(first, second);
        }

        // ──────────────────────────────────────────────
        // Pilots プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Pilots_NewUnit_IsEmpty()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.Pilots.Count);
        }

        [TestMethod]
        public void Pilots_AfterAddPilot_ContainsPilot()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreateAndBoardPilot(src, unit, "パイロットZ");
            Assert.IsTrue(unit.Pilots.Contains(pilot));
        }
    }
}
