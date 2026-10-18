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

        [TestMethod]
        public void WaitPending_RunsPostedActionWithoutWaitingInterval()
        {
            using (var game = new GameThread())
            using (var waiting = new ManualResetEventSlim())
            using (var done = new ManualResetEventSlim())
            {
                game.Start();
                var handled = false;
                long elapsed = -1;
                // 待ちのループ（メッセージのクリック待ちなど）。間隔は長くても、渡された入力にはすぐ応じる
                game.Post(() =>
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    waiting.Set();
                    while (!handled && watch.ElapsedMilliseconds < 5000)
                    {
                        game.WaitPending(5000);
                    }
                    elapsed = watch.ElapsedMilliseconds;
                    done.Set();
                });
                Assert.IsTrue(waiting.Wait(Timeout));
                game.Post(() => handled = true);
                Assert.IsTrue(done.Wait(Timeout));
                Assert.IsTrue(handled);
                Assert.IsTrue(elapsed < 2000, $"elapsed {elapsed}ms");
                game.Stop();
                game.Join(Timeout);
            }
        }

        [TestMethod]
        public void WaitActivity_ReturnsWhenActionFinishes()
        {
            using (var game = new GameThread())
            using (var release = new ManualResetEventSlim())
            {
                game.Start();
                game.Post(() => release.Wait(Timeout));
                Assert.IsTrue(WaitUntil(() => game.IsBusy));
                var seen = game.Activity;

                // 処理が終わるまでは変わらない
                Assert.IsFalse(game.WaitActivity(seen, 50));
                release.Set();
                Assert.IsTrue(game.WaitActivity(seen, (int)Timeout.TotalMilliseconds), "処理を終えて手が空いたら戻る");
                game.Stop();
                game.Join(Timeout);
            }
        }
    }
}
