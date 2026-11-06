using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests;
using SRCCore.Units;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 中断データから復元したユニットの状態が元の SRC（Unit.cls の Dump・Restore・RestoreLinkInfo・RestoreParameter）と同じになるかのテスト。
    /// セーブの形式（SRC# は JSON）は問わず、読み込んだ後の値とリンクを比べる。
    /// </summary>
    [TestClass]
    public class UnitRestoreVBMatchTests
    {
        private static Unit PrepareUnit(SrcPlayDataSaveFixture f)
        {
            var path = f.Write("test.eve", "準備:", "Exit", "");
            f.Write("Data/robot.txt",
                "テスト機",
                "テスト機, てすとき, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 600, 60",
                "AAAA, -.bmp",
                "ライフル, 1500, 1, 3, +0, 5, -, -, AAAA, +0, 射",
                "===",
                "修理, 回復Lv1, 1, 3, 0, -, -",
                "");
            f.Write("Data/pilot.txt",
                "テストパイロット",
                "テスト, てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "180, 180, 180, 180, 180, 180, 普通",
                "ＳＰ, 50, 必中, 1",
                "-.bmp, -.mid",
                "",
                "サポート(ザコ)",
                "サポート, さぽーと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "180, 180, 180, 180, 180, 180, 普通",
                "ＳＰ, 50, 必中, 1",
                "-.bmp, -.mid",
                "");
            f.Src.LoadLocalData();
            f.Src.ScenarioFileName = path;
            f.Src.Event.LoadEventData(path, "");
            f.Src.Map.SetMapSize(3, 3);
            f.Src.Turn = 1;
            f.Src.Expression.DefineGlobalVariable("次ステージ");

            var unit = f.Src.UList.Add("テスト機", 0, "味方");
            var pilot = f.Src.PList.Add("テストパイロット", 1, "味方");
            pilot.Ride(unit);
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static Unit DumpAndRestore(SrcPlayDataSaveFixture f, Unit unit)
        {
            var id = unit.ID;
            var stream = new MemoryStream();
            f.Src.DumpData(stream, SRCSaveKind.Suspend);
            f.NewSession();
            f.Src.RestoreData(new MemoryStream(stream.ToArray()), SRCSaveKind.Suspend);
            Assert.IsFalse(f.Gui.IsTerminated);
            return f.Src.UList.Item(id);
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_KeepsConditions()
        {
            // VB: Dump は colCondition の Name・Lifetime・Level・StrData を書き、Restore は AddCondition で戻す
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            unit.AddCondition("攻撃不能", 2, 0d, "");
            unit.AddCondition("テスト付加", -1, 3d, "\"Ｖ-ＵＰ=ユニット\"");

            var loaded = DumpAndRestore(f, unit);

            Assert.IsTrue(loaded.IsConditionSatisfied("攻撃不能"));
            Assert.AreEqual(2, loaded.ConditionLifetime("攻撃不能"));
            Assert.AreEqual(-1, loaded.ConditionLifetime("テスト付加"));
            Assert.AreEqual(3d, loaded.ConditionLevel("テスト付加"));
            Assert.AreEqual("\"Ｖ-ＵＰ=ユニット\"", loaded.ConditionData("テスト付加"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_KeepsSpecialPowerInEffect()
        {
            // VB: Dump は colSpecialPowerInEffect の Name・StrData を書き、Restore は MakeSpecialPowerInEffect で戻す
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            unit.MakeSpecialPowerInEffect("必中", "テストパイロット");

            var loaded = DumpAndRestore(f, unit);

            Assert.IsTrue(loaded.IsSpecialPowerInEffect("必中"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_KeepsAbilityStockAndBullet()
        {
            // VB: RestoreLinkInfo は Update の後で SetBullet・SetStock を番号で戻す
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            unit.Weapon(1).SetBullet(2);
            unit.Ability(1).SetStock(1);

            var loaded = DumpAndRestore(f, unit);

            Assert.AreEqual(2, loaded.Weapon(1).Bullet());
            Assert.AreEqual(1, loaded.Ability(1).Stock());
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_KeepsHpEnAndUsedCounts()
        {
            // VB: RestoreParameter は HP・EN を戻してから Update する
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            unit.HP = 345;
            unit.EN = 67;
            unit.UsedSupportAttack = 1;
            unit.UsedSupportGuard = 2;
            unit.UsedSyncAttack = 3;
            unit.UsedCounterAttack = 4;
            unit.UsedAction = 1;
            unit.Mode = "固定";

            var loaded = DumpAndRestore(f, unit);

            Assert.AreEqual(345, loaded.HP);
            Assert.AreEqual(67, loaded.EN);
            Assert.AreEqual(1, loaded.UsedSupportAttack);
            Assert.AreEqual(2, loaded.UsedSupportGuard);
            Assert.AreEqual(3, loaded.UsedSyncAttack);
            Assert.AreEqual(4, loaded.UsedCounterAttack);
            Assert.AreEqual(1, loaded.UsedAction);
            Assert.AreEqual("固定", loaded.Mode);
            Assert.AreEqual("テストパイロット", loaded.MainPilot().Name);
            Assert.AreSame(loaded, loaded.MainPilot().Unit);
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_SupportIsKeyedByName()
        {
            // VB: Restore は AddSupport で戻すので、サポートはパイロットの名前で引ける（ザコの ID は名前と違う）
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            var support = f.Src.PList.Add("サポート(ザコ)", 1, "味方");
            unit.AddSupport(support);
            support.Unit = unit;

            var loaded = DumpAndRestore(f, unit);

            Assert.AreNotEqual(support.Name, support.ID);
            Assert.AreEqual(1, loaded.CountSupport());
            Assert.AreSame(f.Src.PList.Item(support.ID), loaded.Support("サポート(ザコ)"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void Restore_KeepsAdditionalSupport()
        {
            // VB: Restore は AdditionalSupport の ID から pltAdditionalSupport を戻し、Unit と IsAdditionalSupport を設定する
            using var f = new SrcPlayDataSaveFixture();
            var unit = PrepareUnit(f);
            var support = f.Src.PList.Add("サポート(ザコ)", 1, "味方");
            unit.pltAdditionalSupport = support;
            support.Unit = unit;
            support.IsAdditionalSupport = true;
            unit.Update();

            var loaded = DumpAndRestore(f, unit);

            Assert.IsNotNull(loaded.pltAdditionalSupport);
            Assert.AreEqual(support.ID, loaded.pltAdditionalSupport.ID);
            Assert.AreSame(f.Src.PList.Item(support.ID), loaded.pltAdditionalSupport);
            Assert.AreSame(loaded, loaded.pltAdditionalSupport.Unit);
            Assert.IsTrue(loaded.pltAdditionalSupport.IsAdditionalSupport);
        }
    }
}
