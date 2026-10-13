using System;
using System.Collections.Concurrent;
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

        private void Run()
        {
            foreach (var action in queue.GetConsumingEnumerable())
            {
                Execute(action);
            }
        }

        private void Execute(Action action)
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
            catch (Exception ex)
            {
                handler(this, new GameThreadExceptionEventArgs(ex));
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
