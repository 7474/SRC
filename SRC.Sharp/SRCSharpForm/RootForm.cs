using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCSharpForm.Resoruces;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    public partial class RootForm : Form
    {
        private SRCCore.SRC SRC;
        private string[] _args;
        private bool _firstShown;

        private bool IsDebugLogEnabled()
        {
            return _args.Any(x => x == "-v");
        }

        private bool IsTraceLogEnabled()
        {
            return _args.Any(x => x == "-vv");
        }

        // http://nanoappli.com/blog/archives/2363
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        private bool IsConsoleLogEnabled()
        {
            return _args.Any(x => x == "--console");
        }

        public RootForm(string[] args)
            : base()
        {
            _args = args;
            _firstShown = true;
            InitializeComponent();

            var config = new LocalFileConfig();
            try
            {
                config.Load();
            }
            catch
            {
                config.Save();
            }
            var fileSystem = new LocalFileSystem();
            var soundPlayer = new WindowsManagedPlayer();

            // TODO 設定の反映処理を設ける
            // XXX 単位変更をこんな感じでやるのは下策だなー
            soundPlayer.SoundVolume = config.SoundVolume / 100f;

            var logConf = new LoggerConfiguration()
                .WriteTo.File(
                    // 実行ファイル配下の logs フォルダに出力する
                    Path.Combine(AppContext.BaseDirectory, "logs\\srcsform..log"),
                    rollingInterval: RollingInterval.Day);
            if (IsTraceLogEnabled())
            {
                logConf = logConf.MinimumLevel.Verbose();
            }
            else if (IsDebugLogEnabled())
            {
                logConf = logConf.MinimumLevel.Debug();
            }
            else
            {
                logConf = logConf.MinimumLevel.Information();
            }
            if (IsConsoleLogEnabled())
            {
                AttachConsole(UInt32.MaxValue);
                Program.LoggerFactory.AddProvider(new SerilogLoggerProvider(new LoggerConfiguration()
                    .WriteTo.TextWriter(new StreamWriter(System.Console.OpenStandardOutput()))
                    .MinimumLevel.Information()
                    .CreateLogger()
                ));
            }
            Program.LoggerFactory.AddProvider(new SerilogLoggerProvider(logConf.CreateLogger()));
            Program.UpdateLogger();
            Program.Log.LogInformation("SRC#Form起動");

            SRC = new SRCCore.SRC(Program.LoggerFactory);
            SRC.SystemConfig = config;
            SRC.FileSystem = fileSystem;
            SRC.Sound.Player = soundPlayer;

            // XXX ファイルシステムへのエントリー追加はお試し中
            try
            {
                fileSystem.AddAchive(SRC.AppPath, fileSystem.PathCombine(SRC.AppPath, "assets.zip"));
                fileSystem.AddPath(SRC.AppPath);
                fileSystem.AddPath(SRC.ExtDataPath2);
                fileSystem.AddPath(SRC.ExtDataPath);
            }
            catch (Exception ex)
            {
                // ignore
                SRC.LogError(ex);
            }

            SRC.GUI = new SRCSharpFormGUI(SRC);
            SRC.GUI.MessageWait = 700;

            Environment.Exit(1);
        }

        private void LoadGameFile()
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "SRC# files (*.eve;*.srcs;*srcsq)|*.eve;*.srcs;*.srcq|event files (*.eve)|*.eve|save files (*.srcs;*srcsq)|*.srcs;*.srcq";

                var res = fbd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    var filePath = fbd.FileName;
                    ExecuteFile(filePath);
                }
            }
        }

        private void ExecuteFile(string filePath)
        {
            Program.Log.LogInformation($"ExecuteFile {filePath}");
            Hide();
            SRC.FileSystem.AddPath(Path.GetDirectoryName(filePath));
            SRC.FileSystem.AddSafePath(Path.GetDirectoryName(filePath));
            SRC.Execute(filePath);
        }

        private void SRCSharpForm_Shown(object sender, EventArgs e)
        {
            // 引数を参照して指定があればそれを読む。
            var executed = false;
            if (_firstShown)
            {
                _firstShown = false;
                foreach (var arg in _args.Where(x => File.Exists(x)))
                {
                    ExecuteFile(arg);
                    executed = true;
                }
            }
            if (!executed)
            {
                LoadGameFile();
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            LoadGameFile();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            SRC.GUI.Configure();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 投げっぱなしのタスクの例外は浮くので良しなにやる
            // https://docs.microsoft.com/ja-jp/dotnet/standard/parallel-programming/exception-handling-task-parallel-library
            System.Threading.Tasks.Task.Run(() =>
            {
                throw new Exception("hoge");
            });

            // CurrentDomain.UnhandledException
            //var newThread = new System.Threading.Thread(() =>
            //{
            //    throw new Exception("hoge");
            //});
            //newThread.Start();

            // ThreadException（UIスレッド）
            //throw new Exception("hoge");
        }
    }
}
