using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SRCSharpWeb
{
    /// <summary>
    /// 画面の下に出すログ（ブラウザのコンソールの代わり。スマートフォンのように開発者ツールが無いブラウザでもログを見る）。
    /// ログはどのスレッドからも来るので、ここにためておき、表示している間だけ srcweb.js が <see cref="WebInterop.TakeLogLines"/> で取り出す。
    /// 出すログはコンソールと同じ（設定の「ログ」のレベル）
    /// </summary>
    public sealed class WebLogPanelProvider : ILoggerProvider
    {
        /// <summary>ためておく行の数（古いものから捨てる）</summary>
        public const int Capacity = 500;

        private static readonly ConcurrentQueue<string> lines = new ConcurrentQueue<string>();
        private static int count;

        /// <summary>行を足す（JS の入力のトレースなども同じ場所に出す）</summary>
        public static void Add(string line)
        {
            lines.Enqueue(line);
            if (Interlocked.Increment(ref count) > Capacity && lines.TryDequeue(out _))
            {
                Interlocked.Decrement(ref count);
            }
        }

        /// <summary>たまっている行を取り出す（取り出したものは消える）</summary>
        public static string Take()
        {
            var result = new System.Text.StringBuilder();
            while (lines.TryDequeue(out var line))
            {
                Interlocked.Decrement(ref count);
                if (result.Length > 0)
                {
                    result.Append('\n');
                }
                result.Append(line);
            }
            return result.ToString();
        }

        public ILogger CreateLogger(string categoryName) => new Logger(categoryName);

        public void Dispose()
        {
        }

        private sealed class Logger : ILogger
        {
            private readonly string category;

            public Logger(string category)
            {
                this.category = category;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

            // レベルは WebLogging の LoggerFactory のフィルターが決める
            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                if (exception != null)
                {
                    message += "\n" + exception;
                }
                Add($"{DateTime.Now:HH:mm:ss.fff} {Short(logLevel)} {category}: {message}");
            }

            private static string Short(LogLevel level) => level switch
            {
                LogLevel.Trace => "TRC",
                LogLevel.Debug => "DBG",
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "CRT",
                _ => "",
            };
        }
    }
}
