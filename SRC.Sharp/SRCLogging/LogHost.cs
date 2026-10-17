using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SRCCore;
using SRCCore.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SRCLogging
{
    /// <summary>
    /// デスクトップのアプリ・ツールのログの構成（docs/logging.md）。
    /// 起動直後は引数・環境変数・既定値で構成し、設定ファイルを読んだ後に <see cref="ApplyConfig"/> で反映する。
    /// </summary>
    public sealed class LogHost : IDisposable
    {
        public const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

        private readonly LogSettings commandLine;
        private readonly LogSettings environment;
        private readonly bool fileByDefault;
        private readonly SwitchableSink fileSink = new SwitchableSink();
        private readonly Serilog.Core.Logger serilog;
        private LogSettings config = LogSettings.Empty;
        private string currentFilePath;
        private string currentFileKey;

        /// <param name="appName">ログのファイル名に使うアプリの名前（srcsform など）</param>
        /// <param name="commandLine">引数で指定された設定</param>
        /// <param name="environment">環境変数で指定された設定（null なら環境変数から読む）</param>
        /// <param name="outputs">ファイルのほかに出す先</param>
        /// <param name="fileByDefault">既定でファイルに出すか</param>
        public LogHost(string appName, LogSettings commandLine, LogSettings environment = null, LogOutputs outputs = LogOutputs.Debug, bool fileByDefault = true)
        {
            AppName = appName;
            this.commandLine = commandLine ?? LogSettings.Empty;
            this.environment = environment ?? LogSettings.FromEnvironment();
            this.fileByDefault = fileByDefault;
            Options = LogOptions.Resolve(fileByDefault, this.commandLine, this.environment);

            var conf = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(fileSink);
            if (outputs.HasFlag(LogOutputs.Debug))
            {
                conf = conf.WriteTo.Debug(outputTemplate: OutputTemplate);
            }
            if (outputs.HasFlag(LogOutputs.StandardError))
            {
                conf = conf.WriteTo.Console(outputTemplate: OutputTemplate, standardErrorFromLevel: LogEventLevel.Verbose);
            }
            serilog = conf.CreateLogger();
            UpdateFileSink();

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    // レベルは実行中に変わるため、出すたびに Options で判定する（後から足すプロバイダーにも効かせる）
                    .AddFilter((category, level) => Options.IsEnabled(category, level))
                    .AddSerilog(serilog, dispose: false)
                    // AddSerilog は Serilog 用の規則（Trace 以上）を足すため、その後に同じ判定を足し直す
                    .AddFilter<SerilogLoggerProvider>((category, level) => Options.IsEnabled(category, level));
            });
        }

        public string AppName { get; }

        /// <summary>効いている設定</summary>
        public LogOptions Options { get; }

        /// <summary>各ライブラリ・アプリに渡す</summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>レベルが引数・環境変数で指定されていて、設定ファイル・設定画面では変わらないか</summary>
        public bool LevelFixed => commandLine.Level.HasValue || environment.Level.HasValue;

        /// <summary>ログのファイルを出しているフォルダ。ファイルに出していなければ null</summary>
        public string FileDirectory => currentFilePath == null ? null : Path.GetDirectoryName(currentFilePath);

        /// <summary>設定ファイル（srcs.json の Log セクション）を反映する</summary>
        public void ApplyConfig(ISystemConfig systemConfig)
        {
            config = LogSettings.FromConfig(systemConfig);
            Options.Update(fileByDefault, commandLine, environment, config);
            UpdateFileSink();
        }

        /// <summary>起動の記録（版・OS・.NET・引数）を出す</summary>
        public void LogStartup(Microsoft.Extensions.Logging.ILogger log, string appTitle, string[] args)
        {
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "";
            log.LogInformation("{App} {Version} 起動 OS: {OS} ({Arch}) .NET: {Runtime} 引数: {Args}",
                appTitle,
                version,
                RuntimeInformation.OSDescription,
                RuntimeInformation.OSArchitecture,
                RuntimeInformation.FrameworkDescription,
                string.Join(" ", args ?? Array.Empty<string>()));
            log.LogInformation("ログ レベル: {Level} ファイル: {File}", Options.Level, FileDirectory ?? "なし");
        }

        private void UpdateFileSink()
        {
            var path = Options.File ? Path.Combine(Options.Directory, AppName + "-.log") : null;
            var key = path == null ? null : path + "|" + Options.RetainDays;
            if (key == currentFileKey) { return; }
            currentFileKey = key;
            if (path == null)
            {
                fileSink.Replace(null);
                currentFilePath = null;
                return;
            }
            try
            {
                Directory.CreateDirectory(Options.Directory);
                fileSink.Replace(new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.File(path,
                        outputTemplate: OutputTemplate,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: Options.RetainDays,
                        rollOnFileSizeLimit: true,
                        shared: true)
                    .CreateLogger());
                currentFilePath = path;
            }
            catch (Exception ex)
            {
                // ファイルに書けなくても動かす
                fileSink.Replace(null);
                currentFilePath = null;
                serilog?.Warning(ex, "ログのファイルを作れません {Path}", path);
            }
        }

        public void Dispose()
        {
            LoggerFactory.Dispose();
            fileSink.Dispose();
            serilog.Dispose();
        }
    }
}
