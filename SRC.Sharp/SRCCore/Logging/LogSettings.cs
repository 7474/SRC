using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Logging
{
    /// <summary>
    /// 1 つの指定元（引数・環境変数・設定ファイル）から読んだログの設定。指定されなかった項目は null。
    /// </summary>
    public sealed class LogSettings
    {
        /// <summary>設定ファイル（srcs.json）のセクション名</summary>
        public const string ConfigSection = "Log";
        /// <summary>カテゴリごとのレベルの項目名の接頭辞（Level:SRCCore.Expression など）</summary>
        public const string CategoryLevelPrefix = "Level:";

        public const string EnvLevel = "SRCSHARP_LOG_LEVEL";
        public const string EnvFile = "SRCSHARP_LOG_FILE";
        public const string EnvDirectory = "SRCSHARP_LOG_DIR";

        public LogLevel? Level { get; set; }
        public IDictionary<string, LogLevel> CategoryLevels { get; } = new Dictionary<string, LogLevel>(StringComparer.OrdinalIgnoreCase);
        public bool? File { get; set; }
        public string Directory { get; set; }
        public int? RetainDays { get; set; }

        public static LogSettings Empty => new LogSettings();

        /// <summary>
        /// 引数から読む。ログの引数を除いた残りを <paramref name="rest"/> に返す。
        /// -v（Debug）・-vv（Trace）・--log-level &lt;レベル&gt;・--log-dir &lt;フォルダ&gt;・--no-log-file
        /// </summary>
        public static LogSettings FromArgs(IEnumerable<string> args, out string[] rest)
        {
            var settings = new LogSettings();
            var others = new List<string>();
            var list = (args ?? Enumerable.Empty<string>()).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var arg = list[i];
                string Value(string name)
                {
                    if (arg.StartsWith(name + "=", StringComparison.Ordinal)) { return arg.Substring(name.Length + 1); }
                    if (arg == name && i + 1 < list.Count) { return list[++i]; }
                    return null;
                }
                string v;
                if (arg == "-v") { settings.Level = LogLevel.Debug; }
                else if (arg == "-vv") { settings.Level = LogLevel.Trace; }
                else if (arg == "--no-log-file") { settings.File = false; }
                else if ((v = Value("--log-level")) != null) { settings.Level = ParseLevel(v) ?? settings.Level; }
                else if ((v = Value("--log-dir")) != null) { settings.Directory = v; settings.File = settings.File ?? true; }
                else { others.Add(arg); }
            }
            rest = others.ToArray();
            return settings;
        }

        public static LogSettings FromArgs(IEnumerable<string> args)
        {
            return FromArgs(args, out _);
        }

        /// <summary>環境変数から読む。SRCSHARP_LOG_LEVEL・SRCSHARP_LOG_FILE（On/Off）・SRCSHARP_LOG_DIR</summary>
        public static LogSettings FromEnvironment(Func<string, string> getEnvironmentVariable = null)
        {
            getEnvironmentVariable = getEnvironmentVariable ?? Environment.GetEnvironmentVariable;
            var settings = new LogSettings
            {
                Level = ParseLevel(getEnvironmentVariable(EnvLevel)),
                File = ParseOnOff(getEnvironmentVariable(EnvFile)),
            };
            var dir = getEnvironmentVariable(EnvDirectory);
            if (!string.IsNullOrWhiteSpace(dir)) { settings.Directory = dir; }
            return settings;
        }

        /// <summary>
        /// 設定ファイルの Log セクションから読む。Level・Level:&lt;カテゴリ&gt;・File（On/Off）・Path・RetainDays
        /// </summary>
        /// <param name="categories">カテゴリごとのレベルを読むカテゴリ</param>
        public static LogSettings FromConfig(ISystemConfig config, IEnumerable<string> categories = null)
        {
            var settings = new LogSettings();
            if (config == null) { return settings; }
            string Get(string name)
            {
                try { return config.GetItem(ConfigSection, name); }
                catch { return null; }
            }
            settings.Level = ParseLevel(Get("Level"));
            settings.File = ParseOnOff(Get("File"));
            var path = Get("Path");
            if (!string.IsNullOrWhiteSpace(path)) { settings.Directory = path; }
            if (int.TryParse(Get("RetainDays"), out var days) && days > 0) { settings.RetainDays = days; }
            foreach (var category in categories ?? LogCategories.All)
            {
                var level = ParseLevel(Get(CategoryLevelPrefix + category));
                if (level.HasValue) { settings.CategoryLevels[category] = level.Value; }
            }
            return settings;
        }

        /// <summary>
        /// レベル名を読む。Trace・Debug・Information・Warning・Error・Critical・None のほか、
        /// Serilog の Verbose・Fatal、短い Info・Warn も受け付ける。
        /// </summary>
        public static LogLevel? ParseLevel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            switch (value.Trim().ToLowerInvariant())
            {
                case "trace":
                case "verbose":
                    return LogLevel.Trace;
                case "debug":
                    return LogLevel.Debug;
                case "information":
                case "info":
                    return LogLevel.Information;
                case "warning":
                case "warn":
                    return LogLevel.Warning;
                case "error":
                    return LogLevel.Error;
                case "critical":
                case "fatal":
                    return LogLevel.Critical;
                case "none":
                case "off":
                    return LogLevel.None;
                default:
                    return null;
            }
        }

        private static bool? ParseOnOff(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            switch (value.Trim().ToLowerInvariant())
            {
                case "on":
                case "true":
                case "1":
                    return true;
                case "off":
                case "false":
                case "0":
                    return false;
                default:
                    return null;
            }
        }
    }
}
