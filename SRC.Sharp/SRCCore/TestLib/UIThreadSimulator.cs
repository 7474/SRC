using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace SRCCore.TestLib
{
    /// <summary>
    /// テスト用に UI スレッドを模擬する。
    /// 専用スレッドで SynchronizationContext のメッセージループを回す。
    /// </summary>
    public sealed class UIThreadSimulator : IDisposable
    {
        private readonly BlockingCollection<(SendOrPostCallback callback, object state)> queue
            = new BlockingCollection<(SendOrPostCallback, object)>();
        private readonly Thread thread;

        public UIThreadSimulator()
        {
            Context = new SimulatorSynchronizationContext(this);
            thread = new Thread(Run)
            {
                Name = "SRC# UI (simulated)",
                IsBackground = true,
            };
            thread.Start();
        }

        public SynchronizationContext Context { get; }

        public Thread Thread => thread;

        public bool IsCurrentThread => Thread.CurrentThread == thread;

        /// <summary>
        /// UI スレッドで関数を実行し、結果を返す。
        /// </summary>
        public T Invoke<T>(Func<T> func)
        {
            var result = default(T);
            Context.Send(_ => result = func(), null);
            return result;
        }

        public void Invoke(Action action)
        {
            Context.Send(_ => action(), null);
        }

        public void Dispose()
        {
            queue.CompleteAdding();
            thread.Join(TimeSpan.FromSeconds(5));
        }

        private void Run()
        {
            SynchronizationContext.SetSynchronizationContext(Context);
            foreach (var (callback, state) in queue.GetConsumingEnumerable())
            {
                callback(state);
            }
        }

        private sealed class SimulatorSynchronizationContext : SynchronizationContext
        {
            private readonly UIThreadSimulator owner;

            public SimulatorSynchronizationContext(UIThreadSimulator owner)
            {
                this.owner = owner;
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                owner.queue.Add((d, state));
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                if (owner.IsCurrentThread)
                {
                    d(state);
                    return;
                }

                Exception error = null;
                using (var done = new ManualResetEventSlim())
                {
                    owner.queue.Add((_ =>
                    {
                        try
                        {
                            d(state);
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                        }
                        finally
                        {
                            done.Set();
                        }
                    }, null));
                    done.Wait();
                }
                if (error != null)
                {
                    throw new TargetInvocationException(error);
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
