using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace SRCCore.Logging
{
    /// <summary>
    /// ログを関数に渡す ILoggerProvider。DAP の output イベント・LSP の window/logMessage・ブラウザのコンソールなど、
    /// ホストの持つ出力先に送るのに使う。レベルの判定は LoggerFactory のフィルターに任せる。
    /// </summary>
    public sealed class DelegateLoggerProvider : ILoggerProvider
    {
        private readonly Action<string, LogLevel, string, Exception> write;
        // 送る処理の中で出たログで、送る処理を呼び直さない
        private readonly ThreadLocal<bool> writing = new ThreadLocal<bool>();

        /// <param name="write">カテゴリ・レベル・メッセージ・例外を受け取る</param>
        public DelegateLoggerProvider(Action<string, LogLevel, string, Exception> write)
        {
            this.write = write;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(this, categoryName);
        }

        public void Dispose()
        {
            writing.Dispose();
        }

        private sealed class Logger : ILogger
        {
            private readonly DelegateLoggerProvider provider;
            private readonly string category;

            public Logger(DelegateLoggerProvider provider, string category)
            {
                this.provider = provider;
                this.category = category;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel) || provider.writing.Value) { return; }
                try
                {
                    provider.writing.Value = true;
                    provider.write(category, logLevel, formatter(state, exception), exception);
                }
                catch
                {
                    // 出力先に送れなくても処理を止めない
                }
                finally
                {
                    provider.writing.Value = false;
                }
            }
        }

        /// <summary>レベルの短い表記（ファイルの書式と同じ TRC・DBG・INF・WRN・ERR・CRT）</summary>
        public static string ShortLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return "TRC";
                case LogLevel.Debug: return "DBG";
                case LogLevel.Information: return "INF";
                case LogLevel.Warning: return "WRN";
                case LogLevel.Error: return "ERR";
                case LogLevel.Critical: return "CRT";
                default: return "";
            }
        }
    }
}
