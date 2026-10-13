using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace SRCCore.Threading.Tests
{
    [TestClass]
    public class GameThreadTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        private static bool WaitUntil(Func<bool> condition)
        {
            var deadline = DateTime.UtcNow + Timeout;
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                {
                    return false;
                }
                Thread.Sleep(5);
            }
            return true;
        }

        [TestMethod]
        public void IsBusy_TrueWhileActionRuns()
        {
            using (var game = new GameThread())
            using (var started = new ManualResetEventSlim())
            using (var release = new ManualResetEventSlim())
            {
                game.Start();
                Assert.IsFalse(game.IsBusy);

                // イベントが入力を待つループのように、処理の途中で止まっている間は実行中
                game.Post(() =>
                {
                    started.Set();
                    release.Wait(Timeout);
                });
                Assert.IsTrue(started.Wait(Timeout));
                Assert.IsTrue(game.IsBusy);

                release.Set();
                Assert.IsTrue(WaitUntil(() => !game.IsBusy), "処理が終われば次の処理を待つ（実行中でない）");
                game.Stop();
                game.Join(Timeout);
            }
        }
    }
}
