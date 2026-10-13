using System;
using System.Threading;

namespace SRCCore.Threading
{
    /// <summary>
    /// SRCCore をゲームスレッドで実行し、GUI 呼び出しを UI スレッドへマーシャリングする。
    /// UI スレッド上で生成すること。
    /// </summary>
    public sealed class CoreThreading : IDisposable
    {
        public CoreThreading(SynchronizationContext uiContext)
        {
            UI = new UIThreadInvoker(uiContext);
            Game = new GameThread();
        }

        public UIThreadInvoker UI { get; }
        public GameThread Game { get; }

        /// <summary>
        /// GUI 系インターフェースの実装を、呼び出しを UI スレッドで実行するプロキシで包む。
        /// </summary>
        public T Wrap<T>(T target) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException($"{typeof(T)} is not an interface.");
            }
            if (target is CoreThreadProxy)
            {
                return target;
            }
            return CoreThreadProxy.Create(target, this);
        }

        /// <summary>
        /// SRC に設定済みの GUI 系インターフェースの実装をプロキシで包む。
        /// </summary>
        public void Install(SRC src)
        {
            src.GUI = Wrap(src.GUI);
            src.GUIMap = Wrap(src.GUIMap);
            src.GUIStatus = Wrap(src.GUIStatus);
            src.GUIScrean = Wrap(src.GUIScrean);
            if (src.Sound != null)
            {
                src.Sound.Player = Wrap(src.Sound.Player);
            }
        }

        public void Start()
        {
            Game.Start();
        }

        /// <summary>
        /// 処理をゲームスレッドで実行する。
        /// </summary>
        public void Post(Action action)
        {
            Game.Post(action);
        }

        public void Dispose()
        {
            Game.Dispose();
        }
    }
}
