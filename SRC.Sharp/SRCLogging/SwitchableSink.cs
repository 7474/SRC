using Serilog.Core;
using Serilog.Events;
using System;

namespace SRCLogging
{
    /// <summary>出力先（ファイル）を実行中に差し替えられるシンク</summary>
    internal sealed class SwitchableSink : ILogEventSink, IDisposable
    {
        private readonly object sync = new object();
        private Logger inner;

        public void Emit(LogEvent logEvent)
        {
            lock (sync)
            {
                inner?.Write(logEvent);
            }
        }

        public void Replace(Logger logger)
        {
            Logger old;
            lock (sync)
            {
                old = inner;
                inner = logger;
            }
            old?.Dispose();
        }

        public void Dispose()
        {
            Replace(null);
        }
    }
}
