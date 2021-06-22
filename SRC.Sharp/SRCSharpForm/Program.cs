using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    static class Program
    {
        internal static Microsoft.Extensions.Logging.ILogger Log { get; private set; }
        internal static ILoggerFactory LoggerFactory { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug);
            });
            LoggerFactory.AddProvider(new SerilogLoggerProvider(
                new LoggerConfiguration().MinimumLevel.Debug()
                    .Enrich.WithThreadId()
                    .WriteTo.Debug(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] <{ThreadId}> {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger()));
            UpdateLogger();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RootForm());
        }

        internal static void UpdateLogger()
        {
            Log = LoggerFactory.CreateLogger("SRCSharpForm");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.LogError(e.ExceptionObject as Exception, "UnhandledException");
        }
    }
}
