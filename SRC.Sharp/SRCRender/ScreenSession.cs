using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.Threading;
using System;
using System.IO;
using System.Threading;

namespace SRCRender
{
    /// <summary>
    /// SRC を初期化し、ゲームスレッドでシナリオを実行する。
    /// </summary>
    public sealed class ScreenSession : IDisposable
    {
        public ScreenSession(IScreenHost host, string appPath = null, IPlaySound soundPlayer = null, ILoggerFactory loggerFactory = null)
        {
            var config = new LocalFileConfig();
            if (!string.IsNullOrEmpty(appPath))
            {
                config.AppPath = appPath;
            }
            try
            {
                config.Load();
            }
            catch
            {
                // 設定ファイルが無い場合は既定値で動かす
            }
            if (!string.IsNullOrEmpty(appPath))
            {
                config.AppPath = appPath;
            }

            var fileSystem = new LocalFileSystem();
            SRC = loggerFactory == null ? new SRC() : new SRC(loggerFactory);
            SRC.SystemConfig = config;
            SRC.FileSystem = fileSystem;
            SRC.Sound.Player = soundPlayer ?? new NullSoundPlayer();
            try
            {
                fileSystem.AddPath(SRC.AppPath);
                if (!string.IsNullOrEmpty(SRC.ExtDataPath2))
                {
                    fileSystem.AddPath(SRC.ExtDataPath2);
                }
                if (!string.IsNullOrEmpty(SRC.ExtDataPath))
                {
                    fileSystem.AddPath(SRC.ExtDataPath);
                }
            }
            catch (Exception ex)
            {
                SRC.LogError(ex);
            }

            Game = new GameThread();
            GUI = new ScreenGUI(SRC, Game, host);
            GUI.Install();
        }

        public SRC SRC { get; }
        public GameThread Game { get; }
        public ScreenGUI GUI { get; }

        /// <summary>
        /// ゲームスレッドを開始し、シナリオ（.eve）またはセーブデータを実行する。
        /// </summary>
        public void Start(string filePath)
        {
            var path = Path.GetFullPath(filePath);
            SRC.FileSystem.AddPath(Path.GetDirectoryName(path));
            SRC.FileSystem.AddSafePath(Path.GetDirectoryName(path));
            Game.Start();
            Game.Post(() => SRC.Execute(path));
        }

        public void Dispose()
        {
            Game.Dispose();
            GUI.Dispose();
        }
    }
}
