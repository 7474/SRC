using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace SRCCore.Threading
{
    /// <summary>
    /// SRCCore の処理を UI スレッドとは別の専用スレッドで順に実行する。
    /// </summary>
    public sealed class GameThread : IDisposable
    {
        private readonly BlockingCollection<Action> queue = new BlockingCollection<Action>();
        private Thread thread;

        /// <summary>
        /// 実行した処理から例外が送出された場合に呼ばれる。
        /// 未設定の場合は例外をそのまま送出する（未処理例外としてプロセスを終了させる）。
        /// </summary>
        public event EventHandler<GameThreadExceptionEventArgs> UnhandledException;

        public bool IsCurrentThread => thread != null && Thread.CurrentThread == thread;

        public bool IsRunning => thread != null && thread.IsAlive;

        public void Start()
        {
            if (thread != null)
            {
                throw new InvalidOperationException("GameThread is already started.");
            }
            thread = new Thread(Run)
            {
                Name = "SRC# Game",
                IsBackground = true,
            };
            thread.Start();
        }

        /// <summary>
        /// 処理をキューに追加する。停止後に追加された処理は破棄する。
        /// </summary>
        public void Post(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            try
            {
                queue.Add(action);
            }
            catch (InvalidOperationException)
            {
                // Stop 済み
            }
        }

        /// <summary>
        /// キューに溜まっている処理をこのスレッドで実行する。
        /// WinForms の Application.DoEvents に相当し、ゲームスレッド上でのみ呼び出せる。
        /// </summary>
        public void ProcessPending()
        {
            if (!IsCurrentThread)
            {
                throw new InvalidOperationException("ProcessPending must be called on the game thread.");
            }
            while (queue.TryTake(out var action))
            {
                Execute(action);
            }
        }

        /// <summary>
        /// 待ちのループから呼ぶ。キューに処理が来るまで（最長 millis ミリ秒）待ち、来た処理を実行する。
        /// 決まった時間だけ眠るのと違い、UI から渡された入力にすぐ応じる。ゲームスレッド上でのみ呼び出せる。
        /// </summary>
        public void WaitPending(int millis)
        {
            if (!IsCurrentThread)
            {
                throw new InvalidOperationException("WaitPending must be called on the game thread.");
            }
            NotifyActivity();
            if (queue.TryTake(out var action, millis))
            {
                Execute(action);
                ProcessPending();
            }
        }

        private readonly object activityLock = new object();
        private long activity;

        /// <summary>
        /// ゲームスレッドが手を空けた（キューの処理を終えた、または待ちのループに入った）回数。
        /// 画面なしで操作するランナーが、決まった時間だけ眠らずに次の状態を待つのに使う（WaitActivity）。
        /// </summary>
        public long Activity => Interlocked.Read(ref activity);

        /// <summary>
        /// Activity が seen から変わるまで、最長 millis ミリ秒待つ。変わったら true。
        /// </summary>
        public bool WaitActivity(long seen, int millis)
        {
            var watch = Stopwatch.StartNew();
            lock (activityLock)
            {
                while (activity == seen)
                {
                    var remaining = millis - watch.ElapsedMilliseconds;
                    if (remaining <= 0 || !Monitor.Wait(activityLock, (int)remaining))
                    {
                        return activity != seen;
                    }
                }
                return true;
            }
        }

        private void NotifyActivity()
        {
            lock (activityLock)
            {
                activity++;
                Monitor.PulseAll(activityLock);
            }
        }

        /// <summary>
        /// Stop が呼ばれたか。待ちのループはこれを見て抜ける。
        /// </summary>
        public bool IsStopping => queue.IsAddingCompleted;

        /// <summary>
        /// 新たな処理の受け付けを止める。キューに残った処理を実行し終えるとスレッドは終了する。
        /// </summary>
        public void Stop()
        {
            if (!queue.IsAddingCompleted)
            {
                queue.CompleteAdding();
            }
        }

        public bool Join(TimeSpan timeout)
        {
            return thread?.Join(timeout) ?? true;
        }

        public void Dispose()
        {
            Stop();
        }

        private volatile bool busy;

        /// <summary>
        /// 処理を実行中か。キューの処理が終わって次の処理を待っている間は false。
        /// イベントが KeyState などで入力を待つループの間は、処理の途中なので true のまま。
        /// </summary>
        public bool IsBusy => busy;

        private void Run()
        {
            foreach (var action in queue.GetConsumingEnumerable())
            {
                busy = true;
                try
                {
                    Execute(action);
                }
                finally
                {
                    busy = queue.Count > 0;
                    if (!busy)
                    {
                        NotifyActivity();
                    }
                }
            }
        }

        private int depth;

        /// <summary>
        /// 実行中の処理が、待ちのループ（ProcessPending）から呼ばれたものでないか。ゲームスレッドの上でのみ意味を持つ。
        /// true の間は、イベントやコマンドの処理の途中ではない（デバッガのホットリロードがセーブとロードをしてよい）。
        /// </summary>
        public bool IsTopLevel => IsCurrentThread && depth == 1;

        private bool IsStopSignal(Exception ex) => ex is OperationCanceledException && IsStopping;

        private void Execute(Action action)
        {
            depth++;
            try
            {
                var handler = UnhandledException;
                if (handler == null)
                {
                    action();
                    return;
                }
                try
                {
                    action();
                }
                catch (Exception ex) when (!IsStopSignal(ex))
                {
                    handler(this, new GameThreadExceptionEventArgs(ex));
                }
            }
            // 終了処理中に待ちのループ（ScreenGUI.DoEvents など）が投げる OperationCanceledException は、
            // ゲームスレッドを終わらせるための合図。外に出すとプロセスが落ちる（ウィンドウを閉じたときなど。#1359）
            catch (OperationCanceledException) when (IsStopping)
            {
            }
            finally
            {
                depth--;
            }
        }
    }

    public class GameThreadExceptionEventArgs : EventArgs
    {
        public GameThreadExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
