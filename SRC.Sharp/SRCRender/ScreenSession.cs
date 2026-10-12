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
                // 設定ファイルが無い場合は、元の SRC が設定ファイルを作る際の既定値で動かす
                ApplyDefaultConfig(config);
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
            // 設定の音量を反映する
            SRC.Sound.Player.SoundVolume = config.SoundVolume / 100f;
            // 汎用グラフィック集などの素材が無くても動かす
            SRC.AllowMissingAssets = true;
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

        /// <summary>
        /// 元の SRC（SRC.bas の CreateIniFile）が設定ファイルを作る際の既定値を設定する。
        /// </summary>
        public static void ApplyDefaultConfig(LocalFileConfig config)
        {
            config.ShowSquareLine = false;
            config.KeepEnemyBGM = false;
            config.AutoDefense = false;
            config.AutoMoveCursor = true;
            config.SpecialPowerAnimation = true;
            config.BattleAnimation = true;
            config.ExtendedAnimation = true;
            config.WeaponAnimation = true;
            config.MoveAnimation = true;
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
            // ゲームスレッドの終了を待ってから破棄する。
            // 終わらない（待ちを伴わない処理を続けている）場合、使用中のバッファを解放すると落ちるため破棄しない。
            if (Game.Join(TimeSpan.FromSeconds(2)))
            {
                GUI.Dispose();
            }
        }
    }
}
