using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SRCCore;
using SRCCore.Logging;

namespace SRCSharpWeb
{
    /// <summary>
    /// ログ（docs/logging.md）。出力先はブラウザのコンソール（Blazor の既定）で、ファイルは無い。
    /// レベルは wwwroot/index.html が URL の ?log=（無ければ localStorage の srcweb.logLevel）から決め、
    /// 環境変数 SRCSHARP_LOG_LEVEL で渡す（マルチスレッドの .NET は起動前に JS とやりとりできないため）。
    /// </summary>
    public static class WebLogging
    {
        public const string StorageKey = "srcweb.logLevel";

        private static LogSettings environment = LogSettings.Empty;
        private static LogSettings config = LogSettings.Empty;

        /// <summary>効いている設定。Program.cs の LoggerFactory のフィルターが使う</summary>
        public static LogOptions Options { get; } = LogOptions.Resolve(false);

        public static ILoggerFactory LoggerFactory { get; private set; } = NullLoggerFactory.Instance;

        /// <summary>SRC#Web 自身のログ</summary>
        public static ILogger Log { get; private set; } = NullLogger.Instance;

        /// <summary>音の再生のログ</summary>
        public static ILogger SoundLog { get; private set; } = NullLogger.Instance;

        /// <summary>環境変数を読む。builder.Build より前に呼ぶ</summary>
        public static void Configure(ILoggingBuilder logging)
        {
            environment = LogSettings.FromEnvironment();
            Options.Update(false, environment);
            logging
                .SetMinimumLevel(LogLevel.Trace)
                // レベルは実行中に変わるため、出すたびに Options で判定する
                .AddFilter((category, level) => Options.IsEnabled(category, level));
        }

        /// <summary>LoggerFactory を受け取る。builder.Build の後に呼ぶ</summary>
        public static void Attach(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Log = loggerFactory.CreateLogger("SRCSharpWeb");
            SoundLog = loggerFactory.CreateLogger(LogCategories.Sound);
        }

        /// <summary>設定画面で選んだレベルにする（URL の ?log= より後に選んだもの）</summary>
        public static void SetLevel(string level)
        {
            environment = new LogSettings { Level = LogSettings.ParseLevel(level) };
            Options.Update(false, environment, config);
        }

        /// <summary>シナリオの設定ファイル（srcs.json の Log セクション）を反映する。URL・localStorage の指定が優先する</summary>
        public static void ApplyConfig(ISystemConfig config)
        {
            WebLogging.config = LogSettings.FromConfig(config);
            Options.Update(false, environment, WebLogging.config);
        }
    }
}
