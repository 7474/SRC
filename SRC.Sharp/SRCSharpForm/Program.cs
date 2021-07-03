using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
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
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug();
            });
            UpdateLogger();

            // TODO この辺の例外処理設定シケてる
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RootForm(args));
        }

        internal static void UpdateLogger()
        {
            Log = LoggerFactory.CreateLogger("SRCSharpForm");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.LogError(e.ExceptionObject as Exception, "UnhandledException");
            MessageBox.Show($@"不明なエラーが発生しました。
詳細はログを確認してください。
{(e.ExceptionObject as Exception)?.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(-1);
        }
    }
}
