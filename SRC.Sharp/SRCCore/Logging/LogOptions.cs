using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRCCore.Logging
{
    /// <summary>
    /// 効いているログの設定。引数 &gt; 環境変数 &gt; 設定ファイル &gt; 既定値 の順に、先に指定されたものを使う。
    /// </summary>
    public sealed class LogOptions
    {
        public const LogLevel DefaultLevel = LogLevel.Information;
        public const int DefaultRetainDays = 7;

        private volatile IReadOnlyDictionary<string, LogLevel> categoryLevels = new Dictionary<string, LogLevel>();

        public LogLevel Level { get; private set; } = DefaultLevel;
        public IReadOnlyDictionary<string, LogLevel> CategoryLevels => categoryLevels;
        public bool File { get; private set; }
        public string Directory { get; private set; }
        public int RetainDays { get; private set; } = DefaultRetainDays;

        /// <summary>引数・環境変数で指定され、設定ファイルや設定画面では変えない項目</summary>
        public bool LevelFixed { get; private set; }

        /// <param name="fileByDefault">既定でファイルに出すか</param>
        /// <param name="sources">優先する順の指定元（引数・環境変数・設定ファイル）</param>
        public static LogOptions Resolve(bool fileByDefault, params LogSettings[] sources)
        {
            var options = new LogOptions();
            options.Update(fileByDefault, sources);
            return options;
        }

        /// <summary>指定元を読み直す（設定ファイルを読んだ後など）</summary>
        public void Update(bool fileByDefault, params LogSettings[] sources)
        {
            var list = sources.Where(x => x != null).ToList();
            Level = list.Select(x => x.Level).FirstOrDefault(x => x.HasValue) ?? DefaultLevel;
            File = list.Select(x => x.File).FirstOrDefault(x => x.HasValue) ?? fileByDefault;
            Directory = list.Select(x => x.Directory).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? DefaultDirectory();
            RetainDays = list.Select(x => x.RetainDays).FirstOrDefault(x => x.HasValue) ?? DefaultRetainDays;
            var levels = new Dictionary<string, LogLevel>(StringComparer.OrdinalIgnoreCase);
            // 優先度の低いものから上書きする
            foreach (var source in Enumerable.Reverse(list))
            {
                foreach (var pair in source.CategoryLevels)
                {
                    levels[pair.Key] = pair.Value;
                }
            }
            categoryLevels = levels;
        }

        /// <summary>レベルを変える（設定画面から）</summary>
        public void SetLevel(LogLevel level)
        {
            Level = level;
        }

        /// <summary>
        /// カテゴリとレベルを出すか。カテゴリごとのレベルは、前方一致する最も長いカテゴリのものを使う
        /// （SRCCore.Event は SRCCore の指定も受ける）。
        /// </summary>
        public bool IsEnabled(string category, LogLevel level)
        {
            if (level == LogLevel.None) { return false; }
            return level >= MinimumLevel(category);
        }

        public LogLevel MinimumLevel(string category)
        {
            var levels = categoryLevels;
            if (levels.Count > 0 && !string.IsNullOrEmpty(category))
            {
                var name = category;
                while (true)
                {
                    if (levels.TryGetValue(name, out var level)) { return level; }
                    var dot = name.LastIndexOf('.');
                    if (dot < 0) { break; }
                    name = name.Substring(0, dot);
                }
            }
            return Level;
        }

        /// <summary>
        /// 既定のログのフォルダ。
        /// Windows: %LOCALAPPDATA%\SRCSharp\logs、macOS: ~/Library/Logs/SRCSharp、
        /// Linux: $XDG_STATE_HOME/srcsharp/logs（未設定なら ~/.local/state/srcsharp/logs）
        /// </summary>
        public static string DefaultDirectory(Func<string, string> getEnvironmentVariable = null)
        {
            getEnvironmentVariable = getEnvironmentVariable ?? Environment.GetEnvironmentVariable;
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(string.IsNullOrEmpty(local) ? home : local, "SRCSharp", "logs");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(home, "Library", "Logs", "SRCSharp");
            }
            var state = getEnvironmentVariable("XDG_STATE_HOME");
            if (string.IsNullOrWhiteSpace(state) || !Path.IsPathRooted(state))
            {
                state = Path.Combine(home, ".local", "state");
            }
            return Path.Combine(state, "srcsharp", "logs");
        }
    }
}
