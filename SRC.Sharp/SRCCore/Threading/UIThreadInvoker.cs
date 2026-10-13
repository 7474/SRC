using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace SRCCore.Threading
{
    /// <summary>
    /// 処理を UI スレッドで同期的に実行する。
    /// UI スレッド上で生成すること。
    /// </summary>
    public sealed class UIThreadInvoker
    {
        private readonly SynchronizationContext context;
        private readonly Thread uiThread;

        public UIThreadInvoker(SynchronizationContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            uiThread = Thread.CurrentThread;
        }

        public bool IsUIThread => Thread.CurrentThread == uiThread;

        public object Invoke(Func<object> func)
        {
            if (IsUIThread)
            {
                return func();
            }

            object result = null;
            ExceptionDispatchInfo error = null;
            context.Send(_ =>
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    error = ExceptionDispatchInfo.Capture(ex);
                }
            }, null);
            error?.Throw();
            return result;
        }
    }
}
