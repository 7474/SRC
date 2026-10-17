using Avalonia;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;
using SRCCore.Logging;
using SRCLogging;
using System;

namespace SRCSharpAvalonia
{
    internal static class Program
    {
        /// <summary>ログ（docs/logging.md）。設定ファイル（srcs.json）は App で読んで反映する</summary>
        internal static LogHost LogHost { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            LogHost = new LogHost("srcsavalonia", LogSettings.FromArgs(args, out var rest), outputs: LogOutputs.Debug);
            var log = LogHost.LoggerFactory.CreateLogger("SRCSharpAvalonia");
            LogHost.LogStartup(log, "SRC#Avalonia", args);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => log.LogCritical(e.ExceptionObject as Exception, "UnhandledException");
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(rest);
            }
            finally
            {
                log.LogInformation("SRC#Avalonia終了");
                LogHost.Dispose();
            }
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect();
            if (LogHost == null)
            {
                return builder.LogToTrace();
            }
            // Avalonia 自体の警告以上もログに残す
            var avaloniaLog = LogHost.LoggerFactory.CreateLogger("Avalonia");
            return builder.LogToDelegate(message => avaloniaLog.LogWarning("{Message}", message), LogEventLevel.Warning);
        }
    }
}
