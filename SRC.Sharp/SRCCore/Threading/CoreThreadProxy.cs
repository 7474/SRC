using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace SRCCore.Threading
{
    /// <summary>
    /// GUI 系インターフェースの呼び出しを UI スレッドへマーシャリングするプロキシ。
    /// ゲームスレッドからの IGUI.Sleep / IGUI.DoEvents は UI スレッドへ送らず、
    /// ゲームスレッド上でキューに溜まった処理を実行する。
    /// </summary>
    public class CoreThreadProxy : DispatchProxy
    {
        private object target;
        private CoreThreading threading;

        internal static T Create<T>(T target, CoreThreading threading) where T : class
        {
            if (target == null)
            {
                return null;
            }
            var proxy = Create<T, CoreThreadProxy>();
            var p = (CoreThreadProxy)(object)proxy;
            p.target = target;
            p.threading = threading;
            return proxy;
        }

        internal object Target => target;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (threading.UI.IsUIThread)
            {
                return InvokeTarget(targetMethod, args);
            }

            if (threading.Game.IsCurrentThread && targetMethod.DeclaringType == typeof(IGUI))
            {
                switch (targetMethod.Name)
                {
                    case nameof(IGUI.DoEvents):
                        threading.Game.ProcessPending();
                        return null;

                    case nameof(IGUI.Sleep):
                        var milliseconds = (int)args[0];
                        var withEvents = (bool)args[1];
                        if (withEvents)
                        {
                            threading.Game.ProcessPending();
                        }
                        Lib.GeneralLib.TimedSleep(milliseconds);
                        return null;
                }
            }

            return threading.UI.Invoke(() => InvokeTarget(targetMethod, args));
        }

        private object InvokeTarget(MethodInfo targetMethod, object[] args)
        {
            try
            {
                return targetMethod.Invoke(target, args);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
