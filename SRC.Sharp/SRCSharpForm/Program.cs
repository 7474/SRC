using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
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
        static void Main(string[] args)
        {
            var mutex = new Mutex(false, "SRC_SHARP_FORM");
            if (mutex.WaitOne(0, false) == false)
            {
                // 多分多重起動して不整合は起きないと思うけれど
                // いいこともないだろうから抑制しておく
                Debug.WriteLine("２重起動はできません");
                Console.Error.WriteLine("２重起動はできません");
                return;
            }

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            LoggerFactory.AddProvider(new SerilogLoggerProvider(new LoggerConfiguration()
                    .WriteTo.Debug()
                    .MinimumLevel.Debug()
                    .CreateLogger()));
            UpdateLogger();

            Application.ApplicationExit += Application_ApplicationExit;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RootForm(args));
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                Log.LogInformation("SRC#Form終了");
            }
            catch
            {
                // ignore
            }
        }

        internal static void UpdateLogger()
        {
            Log = LoggerFactory.CreateLogger("SRCSharpForm");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ProccessUnknownError(e.Exception);
            Environment.Exit(-1);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProccessUnknownError(e.ExceptionObject as Exception);
            Environment.Exit(-1);
        }

        private static void ProccessUnknownError(Exception ex)
        {
            try
            {
                Log.LogError(ex, "UnhandledException");
                MessageBox.Show($@"不明なエラーが発生しました。
詳細はログを確認してください。
{ex?.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // ignore
            }
        }
    }
}