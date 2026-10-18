using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib.TimedSleep と SkipWait（srcsharp play --skip-wait）のテスト
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class GeneralLibTimedSleepTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            GeneralLib.SkipWait = false;
        }

        [TestMethod]
        public void TimedSleep_SkipWait_AdvancesClockWithoutSleeping()
        {
            GeneralLib.SkipWait = true;
            var watch = Stopwatch.StartNew();
            var before = GeneralLib.timeGetTime();
            GeneralLib.TimedSleep(5000);
            var after = GeneralLib.timeGetTime();
            watch.Stop();

            Assert.IsTrue(after - before >= 5000, $"時計が進んでいない: {after - before}");
            Assert.IsTrue(watch.ElapsedMilliseconds < 1000, $"実際に眠っている: {watch.ElapsedMilliseconds}");
        }

        [TestMethod]
        public void TimedSleep_SkipWait_EndsWaitLoopBasedOnClock()
        {
            // WaitCmd と同じ形のループ（時計を見ながら少しずつ眠る）がすぐ終わる
            GeneralLib.SkipWait = true;
            var watch = Stopwatch.StartNew();
            var start = GeneralLib.timeGetTime();
            while (start + 3000 > GeneralLib.timeGetTime())
            {
                GeneralLib.TimedSleep(25);
            }
            watch.Stop();

            Assert.IsTrue(watch.ElapsedMilliseconds < 1000, $"ループに時間がかかっている: {watch.ElapsedMilliseconds}");
        }

        [TestMethod]
        public void TimedSleep_Normal_SleepsInRealTime()
        {
            GeneralLib.SkipWait = false;
            var watch = Stopwatch.StartNew();
            GeneralLib.TimedSleep(50);
            watch.Stop();

            Assert.IsTrue(watch.ElapsedMilliseconds >= 40, $"眠っていない: {watch.ElapsedMilliseconds}");
        }

        [TestMethod]
        public void TimedSleep_ZeroOrNegative_DoesNothing()
        {
            GeneralLib.SkipWait = true;
            var before = GeneralLib.timeGetTime();
            GeneralLib.TimedSleep(0);
            GeneralLib.TimedSleep(-10);
            var after = GeneralLib.timeGetTime();

            Assert.IsTrue(after - before < 1000);
        }
    }
}
