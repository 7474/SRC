using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    static class Program
    {
        internal static ILogger Log { get; private set; }
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
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug();
            });
            UpdateLogger();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
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
