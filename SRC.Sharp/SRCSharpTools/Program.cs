using SRCAuthoring.Balance;
using SRCDebugAdapter;
using SRCAuthoring.Linting;
using SRCAuthoring.Lsp;
using SRCCore.Logging;
using SRCLogging;
using SRCPlay;
using SRCSharpTools.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRCSharpTools
{
    public static class Program
    {
        private static LogSettings logSettings = LogSettings.Empty;

        /// <summary>ログの構成。play・lsp・dap で使う</summary>
        private static LogHost CreateLogHost(string command, LogOutputs outputs)
        {
            return new LogHost("srcsharp-" + command, logSettings, outputs: outputs);
        }

        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            args = UiLanguage.Apply(args);
            // ログの引数（docs/logging.md）
            logSettings = LogSettings.FromArgs(args, out args);
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                Console.Write(Strings.Usage);
                return args.Length == 0 ? 2 : 0;
            }
            if (args[0] == "--version")
            {
                // VS Code 拡張が入っている版を調べるのに使う
                Console.WriteLine(typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "");
                return 0;
            }
            var (positional, options) = Parse(args.Skip(1));
            switch (args[0])
            {
                case "lint":
                    return await LintAsync(positional, options);
                case "play":
                    return Play(positional, options);
                case "balance":
                    return Balance(positional, options);
                case "lsp":
                    // stdout はプロトコルに使うため、ログはファイルとプロトコルのログ通知に出す
                    using (var logHost = CreateLogHost("lsp", LogOutputs.None))
                    {
                        var server = new LspServer(Console.OpenStandardInput(), Console.OpenStandardOutput(), logHost.LoggerFactory, logHost.OverrideLevel);
                        logHost.LogStartup(logHost.LoggerFactory.CreateLogger("SRCSharp.Tools"), "srcsharp lsp", args);
                        return await server.RunAsync();
                    }
                case "dap":
                    using (var logHost = CreateLogHost("dap", LogOutputs.None))
                    {
                        var server = new DapServer(Console.OpenStandardInput(), Console.OpenStandardOutput(), logHost);
                        logHost.LogStartup(logHost.LoggerFactory.CreateLogger("SRCSharp.Tools"), "srcsharp dap", args);
                        return await server.RunAsync();
                    }
                default:
                    Console.Error.WriteLine(Strings.UnknownCommand(args[0]));
                    Console.Error.Write(Strings.Usage);
                    return 2;
            }
        }

        private static (List<string> positional, Dictionary<string, string> options) Parse(IEnumerable<string> args)
        {
            var positional = new List<string>();
            var options = new Dictionary<string, string>();
            var list = args.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].StartsWith("--"))
                {
                    var key = list[i].Substring(2);
                    options[key] = i + 1 < list.Count && !list[i + 1].StartsWith("--") ? list[++i] : "true";
                }
                else
                {
                    positional.Add(list[i]);
                }
            }
            return (positional, options);
        }

        private static void WriteOutput(string text, Dictionary<string, string> options)
        {
            if (options.TryGetValue("output", out var path))
            {
                File.WriteAllText(path, text, new UTF8Encoding(false));
            }
            else
            {
                Console.Write(text);
            }
        }

        private static async Task<int> LintAsync(List<string> paths, Dictionary<string, string> options)
        {
            if (paths.Count == 0)
            {
                paths.Add(Environment.CurrentDirectory);
            }
            var result = await new DataLinter().LintAsync(paths);
            if (options.TryGetValue("format", out var format) && format == "sarif")
            {
                WriteOutput(SarifWriter.Write(result.Diagnostics, Environment.CurrentDirectory), options);
            }
            else
            {
                foreach (var d in result.Diagnostics)
                {
                    Console.Error.WriteLine(d);
                }
                Console.WriteLine(Strings.LintSummary(result.CheckedFiles.Count, result.Diagnostics.Count));
            }
            return result.HasError ? 1 : 0;
        }

        private static int Play(List<string> paths, Dictionary<string, string> options)
        {
            if (paths.Count != 1)
            {
                Console.Error.WriteLine(Strings.PlayRequiresOneEventFile);
                return 2;
            }
            var eve = Path.GetFullPath(paths[0]);
            options.TryGetValue("app-path", out var appPath);
            // 相対パスのままだとスペシャルパワーデータ（Data\System\sp.txt）などが読み込まれないため、絶対パスにする
            if (!string.IsNullOrEmpty(appPath))
            {
                appPath = Path.GetFullPath(appPath);
            }
            options.TryGetValue("out", out var outDir);
            using (var logHost = CreateLogHost("play", LogOutputs.StandardError))
            using (var runner = new ScreenScenarioRunner(appPath ?? SRCAuthoring.Analysis.ScenarioAnalyzer.FindScenarioRoot(eve), logHost.LoggerFactory))
            {
                var log = logHost.LoggerFactory.CreateLogger("SRCSharp.Tools");
                logHost.ApplyConfig(runner.Session.SRC.SystemConfig);
                logHost.LogStartup(log, "srcsharp play", new[] { eve });
                runner.Start(eve);
                try
                {
                    if (options.TryGetValue("script", out var script))
                    {
                        var play = new PlayScript(runner) { SnapshotDirectory = outDir, Trace = Console.WriteLine };
                        play.Run(File.ReadAllLines(script));
                    }
                    else
                    {
                        var steps = options.TryGetValue("steps", out var s) ? int.Parse(s) : 1000;
                        var auto = new AutoPlayer(runner) { SnapshotDirectory = outDir, Trace = m => { if (m.StartsWith("stall")) Console.WriteLine(m); } };
                        auto.Run(steps);
                        foreach (var message in auto.Messages)
                        {
                            Console.WriteLine("message: " + message);
                        }
                    }
                }
                catch (PlayScriptException ex)
                {
                    Console.Error.WriteLine("error: " + ex.Message);
                    return 1;
                }
                if (runner.Error != null && !(runner.Error is SRCCore.TestLib.HeadlessTerminatedException))
                {
                    log.LogError(runner.Error, "シナリオの実行中のエラー");
                    Console.Error.WriteLine("error: " + runner.Error);
                    return 1;
                }
                Console.WriteLine(runner.IsTerminated ? "terminated" : "stopped");
                return 0;
            }
        }

        private static int Balance(List<string> paths, Dictionary<string, string> options)
        {
            if (paths.Count == 0)
            {
                paths.Add(Environment.CurrentDirectory);
            }
            var report = BalanceReport.Create(paths);
            foreach (var d in report.Diagnostics)
            {
                Console.Error.WriteLine(d);
            }
            options.TryGetValue("format", out var format);
            switch (format)
            {
                case "csv":
                    WriteOutput(report.UnitsCsv(), options);
                    break;
                case "weapons-csv":
                    WriteOutput(report.WeaponsCsv(), options);
                    break;
                default:
                    WriteOutput(report.ToMarkdown(), options);
                    break;
            }
            return 0;
        }
    }
}
