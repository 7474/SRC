using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SRCCore.TestLib
{
    /// <summary>
    /// 画面を持たない GUI 実装。
    /// IGUI / IGUIMap / IGUIStatus / IGUIScrean / IPlaySound の呼び出しをすべて記録し、
    /// 既定ではプロパティは設定値を、メソッドは戻り値型の既定値を返す。
    /// シナリオをヘッドレスで実行するテスト（E2E テスト）での利用を想定している。
    /// IGUI.Terminate が呼ばれるとプロセスを終了させずに終了状態になり、
    /// 以降の呼び出しでは HeadlessTerminatedException を送出してシナリオの実行を打ち切る。
    /// </summary>
    public sealed class HeadlessGUI
    {
        private readonly object syncRoot = new object();
        private readonly List<HeadlessGUICall> calls = new List<HeadlessGUICall>();
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();
        private readonly Dictionary<string, Func<object[], object>> handlers = new Dictionary<string, Func<object[], object>>();

        public HeadlessGUI()
        {
            GUI = Recorder.Create<IGUI>(this);
            GUIMap = Recorder.Create<IGUIMap>(this);
            GUIStatus = Recorder.Create<IGUIStatus>(this);
            GUIScrean = Recorder.Create<IGUIScrean>(this);
            SoundPlayer = Recorder.Create<IPlaySound>(this);
        }

        public IGUI GUI { get; }

        /// <summary>
        /// IGUI.Terminate が呼ばれたか。
        /// </summary>
        public bool IsTerminated { get; private set; }
        public IGUIMap GUIMap { get; }
        public IGUIStatus GUIStatus { get; }
        public IGUIScrean GUIScrean { get; }
        public IPlaySound SoundPlayer { get; }

        /// <summary>
        /// 記録した呼び出し（プロパティの取得は含まない）。
        /// </summary>
        public IReadOnlyList<HeadlessGUICall> Calls
        {
            get
            {
                lock (syncRoot)
                {
                    return calls.ToList();
                }
            }
        }

        public IEnumerable<HeadlessGUICall> CallsOf(string member)
        {
            return Calls.Where(x => x.Member == member);
        }

        /// <summary>
        /// メンバーの振る舞いを設定する。プロパティの取得は "get_名前" を指定する。
        /// </summary>
        public HeadlessGUI Handle(string member, Func<object[], object> handler)
        {
            lock (syncRoot)
            {
                handlers[member] = handler;
            }
            return this;
        }

        /// <summary>
        /// SRC に GUI 系インターフェースの実装を設定する。
        /// </summary>
        public void Install(SRC src)
        {
            src.GUI = GUI;
            src.GUIMap = GUIMap;
            src.GUIStatus = GUIStatus;
            src.GUIScrean = GUIScrean;
            src.Sound.Player = SoundPlayer;
        }

        private object OnInvoke(Type interfaceType, MethodInfo method, object[] args)
        {
            var name = method.Name;
            Func<object[], object> handler;
            lock (syncRoot)
            {
                if (IsTerminated)
                {
                    throw new HeadlessTerminatedException();
                }
                if (interfaceType == typeof(IGUI) && name == nameof(IGUI.Terminate))
                {
                    calls.Add(new HeadlessGUICall(interfaceType.Name, name, (object[])args.Clone(), Thread.CurrentThread));
                    IsTerminated = true;
                    // false を返して Environment.Exit させない
                    return false;
                }
                if (!name.StartsWith("get_"))
                {
                    calls.Add(new HeadlessGUICall(interfaceType.Name, name, (object[])args.Clone(), Thread.CurrentThread));
                }
                handlers.TryGetValue(name, out handler);
                if (handler == null && method.IsSpecialName)
                {
                    var key = interfaceType.Name + "." + name.Substring(4);
                    if (name.StartsWith("set_"))
                    {
                        properties[key] = args[0];
                        return null;
                    }
                    if (name.StartsWith("get_") && properties.TryGetValue(key, out var value))
                    {
                        return value;
                    }
                }
            }

            if (handler != null)
            {
                return handler(args);
            }
            return DefaultValue(method.ReturnType);
        }

        private static object DefaultValue(Type type)
        {
            if (type == typeof(void) || !type.IsValueType)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        public class Recorder : DispatchProxy
        {
            private HeadlessGUI owner;
            private Type interfaceType;

            internal static T Create<T>(HeadlessGUI owner) where T : class
            {
                var proxy = Create<T, Recorder>();
                var recorder = (Recorder)(object)proxy;
                recorder.owner = owner;
                recorder.interfaceType = typeof(T);
                return proxy;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                return owner.OnInvoke(interfaceType, targetMethod, args);
            }
        }
    }

    public class HeadlessGUICall
    {
        public HeadlessGUICall(string interfaceName, string member, object[] args, Thread thread)
        {
            InterfaceName = interfaceName;
            Member = member;
            Args = args;
            Thread = thread;
        }

        public string InterfaceName { get; }
        public string Member { get; }
        public object[] Args { get; }
        public Thread Thread { get; }

        public override string ToString()
        {
            return $"{InterfaceName}.{Member}({string.Join(", ", Args.Select(x => x?.ToString() ?? "null"))})";
        }
    }

    /// <summary>
    /// HeadlessGUI の終了後に GUI が呼び出された。
    /// </summary>
    public class HeadlessTerminatedException : Exception
    {
        public HeadlessTerminatedException() : base("HeadlessGUI is terminated.")
        {
        }
    }
}
