using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace SRCTestForm
{
    internal static class Program
    {
        internal static ILogger Log { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug();
            });
            Log = loggerFactory.CreateLogger("SRCTestForm");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTeatMain());
        }
    }
}
