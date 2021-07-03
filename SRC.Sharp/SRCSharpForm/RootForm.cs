using Serilog;
using Serilog.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCSharpForm.Resoruces;
using System;
using System.IO;
using System.Linq;
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
            Program.LoggerFactory.AddProvider(new SerilogLoggerProvider(logConf.CreateLogger()));
            Program.UpdateLogger();

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
    }
}
