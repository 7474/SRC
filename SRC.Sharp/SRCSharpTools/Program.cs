using SRCAuthoring;
using SRCAuthoring.Analysis;
using SRCAuthoring.Balance;
using SRCAuthoring.Preview;
using SRCAuthoring.Rules;
using SRCDebugAdapter;
using SRCHelpSearch;
using SRCAuthoring.Linting;
using SRCAuthoring.Lsp;
using SRCAuthoring.Templates;
using SRCCore.Logging;
using SRCLogging;
using SRCPlay;
using SRCSharpTools.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCSharpTools
{
    public static class Program
    {
        private static LogSettings logSettings = LogSettings.Empty;

        private static readonly HashSet<string> BooleanOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "list", "list-rules", "skip-wait", "force", "fail-on-missing", "no-images", "stdin", "play-once",
        };

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
                case "playtest":
                    return Playtest(positional, options);
                case "balance":
                    return Balance(positional, options);
                case "new":
                    return New(positional, options);
                case "flow":
                    return Flow(positional, options);
                case "assets":
                    return Assets(positional, options);
                case "trial":
                    return await TrialAsync(positional, options);
                case "event-flow":
                    return EventFlow(positional, options);
                case "talk-preview":
                    return TalkPreview(positional, options);
                case "anime-preview":
                    return await AnimePreviewAsync(positional, options);
                case "help":
                    return Help(positional, options);
                case "lsp":
                    // stdout はプロトコルに使うため、ログはファイルとプロトコルのログ通知に出す
                    using (var logHost = CreateLogHost("lsp", LogOutputs.None))
                    {
                        var server = new LspServer(Console.OpenStandardInput(), Console.OpenStandardOutput(), logHost.LoggerFactory, logHost.OverrideLevel);
                        // コマンド・特殊能力・属性のホバーと補完に、同梱のヘルプを使う（7474/SRC#1642）
                        server.Help = new HelpReference(HelpDocument.Topics.Select(x => (x.Name, x.Body)));
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
                    var hasValue = !BooleanOptions.Contains(key) && i + 1 < list.Count && !list[i + 1].StartsWith("--");
                    options[key] = hasValue ? list[++i] : "true";
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
            IReadOnlyList<string> groups;
            options.TryGetValue("rules", out var rules);
            try
            {
                groups = RuleGroups.Parse(rules);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine(Strings.UnknownRuleGroup(rules, string.Join(", ", RuleGroups.All.Append("none"))));
                return 2;
            }
            if (options.ContainsKey("list-rules"))
            {
                WriteOutput(ListRules(groups, options.ContainsKey("rules")), options);
                return 0;
            }
            if (paths.Count == 0)
            {
                paths.Add(Environment.CurrentDirectory);
            }
            // --app-path: SRC 本体（か最小の汎用データ）の Data/System のデータ（sp.txt など）を先に読む。検査はせず、参照の先として使う
            IEnumerable<string> systemFiles = null;
            if (options.TryGetValue("app-path", out var appPath))
            {
                var system = Path.Combine(Path.GetFullPath(appPath), "Data", "System");
                systemFiles = Directory.Exists(system) ? Directory.GetFiles(system, "*.txt").OrderBy(x => x, StringComparer.Ordinal).ToList() : new List<string>();
            }
            var result = await new DataLinter { RuleGroups = groups }.LintAsync(paths, systemFiles: systemFiles);
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

        /// <summary>規則の一覧（ID・グループ・重さ・説明・出どころをタブで区切る）。--rules があればそのグループの規則だけ</summary>
        private static string ListRules(IReadOnlyList<string> groups, bool filtered)
        {
            var sb = new StringBuilder();
            foreach (var group in RuleGroups.All)
            {
                sb.Append("# ").Append(group).Append('\t').AppendLine(RuleGroups.Describe(group));
            }
            var rules = filtered ? DataRuleCatalog.Select(groups) : DataRuleCatalog.All;
            foreach (var rule in rules)
            {
                var severity = rule.Severity switch { DiagnosticSeverity.Error => "error", DiagnosticSeverity.Warning => "warning", _ => "info" };
                sb.Append(rule.Id).Append('\t')
                    .Append(string.Join(",", rule.Groups)).Append('\t')
                    .Append(severity).Append('\t')
                    .Append(rule.Description).Append('\t')
                    .AppendLine(rule.Origin);
            }
            return sb.ToString();
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
            // シナリオのフォルダ（Data・Lib・Continue の基準）。無ければ、サブフォルダのイベントファイルなら Data のある親
            var scenarioRoot = options.TryGetValue("scenario-root", out var root)
                ? Path.GetFullPath(root)
                : SRCAuthoring.Analysis.ScenarioAnalyzer.FindRunRoot(eve);
            options.TryGetValue("out", out var outDir);
            // 画面を見ないので、決まった時間だけ待つ処理（戦闘アニメ、メッセージ、移動、Wait コマンドなど）を飛ばす
            SRCCore.Lib.GeneralLib.SkipWait = options.ContainsKey("skip-wait");
            using (var logHost = CreateLogHost("play", LogOutputs.StandardError))
            using (var runner = new ScreenScenarioRunner(appPath ?? scenarioRoot, logHost.LoggerFactory))
            {
                var log = logHost.LoggerFactory.CreateLogger("SRCSharp.Tools");
                logHost.ApplyConfig(runner.Session.SRC.SystemConfig);
                // PlayScript の resume（中断データからの再開）で SRC を起動し直したときも、設定を反映する
                runner.SessionCreated += session => logHost.ApplyConfig(session.SRC.SystemConfig);
                logHost.LogStartup(log, "srcsharp play", new[] { eve });
                // 乱数の種を決める（同じ手順なら毎回同じ結果にする。#1551）
                if (options.TryGetValue("seed", out var seed))
                {
                    runner.Session.SRC.InitialRndSeed = int.Parse(seed, CultureInfo.InvariantCulture);
                }
                // 各話の終わりの状態を記録する（#1551）
                options.TryGetValue("state", out var statePath);
                var recorder = string.IsNullOrEmpty(statePath) ? null : new ScenarioStateRecorder(runner);
                runner.Start(eve, scenarioRoot);
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
                    WriteState(runner, recorder, statePath);
                    return 1;
                }
                WriteState(runner, recorder, statePath);
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

        /// <summary>
        /// 強さの確かめ（7474/SRC#1690）。1 話を、乱数を変えて --runs 回自動で遊び、勝ち負け・ターン数・落ちた味方と直し方の手がかりを出す。
        /// 結果は Markdown（既定）か JSON。止まった回（自動プレイが進めなくなった）は勝ち負けに数えない
        /// </summary>
        private static int Playtest(List<string> paths, Dictionary<string, string> options)
        {
            if (paths.Count != 1)
            {
                Console.Error.WriteLine(Strings.PlayRequiresOneEventFile);
                return 2;
            }
            var eve = Path.GetFullPath(paths[0]);
            options.TryGetValue("app-path", out var appPath);
            if (!string.IsNullOrEmpty(appPath))
            {
                appPath = Path.GetFullPath(appPath);
            }
            var scenarioRoot = options.TryGetValue("scenario-root", out var root)
                ? Path.GetFullPath(root)
                : SRCAuthoring.Analysis.ScenarioAnalyzer.FindRunRoot(eve);
            var runs = options.TryGetValue("runs", out var r) ? int.Parse(r, CultureInfo.InvariantCulture) : 5;
            var firstSeed = options.TryGetValue("seed", out var s) ? int.Parse(s, CultureInfo.InvariantCulture) : 1;
            var turns = options.TryGetValue("turns", out var t) ? int.Parse(t, CultureInfo.InvariantCulture) : 30;
            var policy = options.TryGetValue("policy", out var p) ? p : "smart";
            // 画面を見ないので、決まった時間だけ待つ処理は飛ばす
            SRCCore.Lib.GeneralLib.SkipWait = true;
            var summary = new PlaytestSummary { Scenario = Path.GetFileName(eve), TurnLimit = turns, Policy = policy };
            using (var logHost = CreateLogHost("playtest", LogOutputs.None))
            {
                for (var i = 0; i < runs; i++)
                {
                    using var runner = new ScreenScenarioRunner(appPath ?? scenarioRoot, logHost.LoggerFactory);
                    logHost.ApplyConfig(runner.Session.SRC.SystemConfig);
                    var run = SRCPlay.Playtest.RunOnce(runner, eve, scenarioRoot, firstSeed + i, turns, policy);
                    summary.Runs.Add(run);
                    // 進み具合（標準エラー。結果は標準出力かファイル）
                    Console.Error.WriteLine($"{i + 1}/{runs} seed {run.Seed}: {PlaytestSummary.Label(run.Outcome)} {run.Turns}T");
                }
            }
            var text = options.TryGetValue("format", out var format) && format == "json" ? summary.ToJson() : summary.ToMarkdown();
            if (options.TryGetValue("output", out var output))
            {
                File.WriteAllText(output, text, new UTF8Encoding(false));
            }
            else
            {
                Console.Write(text);
            }
            // --min-wins を下回れば終了コード 1（CI で場面の強さを見張る）
            if (options.TryGetValue("min-wins", out var minWins) && summary.Count("win") < int.Parse(minWins, CultureInfo.InvariantCulture))
            {
                return 1;
            }
            return 0;
        }

        private static void WriteState(ScreenScenarioRunner runner, ScenarioStateRecorder recorder, string path)
        {
            if (recorder == null)
            {
                return;
            }
            // 終わっていなければゲームのスレッドで読む（終わっていれば、ゲームのスレッドはもう状態を変えない）
            if (runner.IsFinished)
            {
                recorder.CaptureFinal(true);
            }
            else
            {
                runner.Session.Game.Post(() => recorder.CaptureFinal(false));
                runner.TrySync(TimeSpan.FromSeconds(10));
            }
            File.WriteAllText(path, recorder.ToText(), new UTF8Encoding(false));
        }

        private static int Help(List<string> topics, Dictionary<string, string> options)
        {
            if (topics.Count == 0 && options.Count == 0)
            {
                Console.Write(Strings.Usage);
                return 0;
            }
            if (HelpDocument.Topics.Count == 0)
            {
                Console.Error.WriteLine(Strings.HelpNotBundled);
                return 1;
            }
            if (options.TryGetValue("export", out var directory))
            {
                var count = HelpDocument.Export(directory);
                Console.Error.WriteLine(Strings.HelpExported(count, directory));
                return 0;
            }
            if (options.ContainsKey("list"))
            {
                foreach (var topic in HelpDocument.Topics)
                {
                    Console.WriteLine(topic.Title == topic.Name ? topic.Name : topic.Name + "\t" + topic.Title);
                }
                return 0;
            }
            if (options.TryGetValue("search", out var text))
            {
                var limit = options.TryGetValue("limit", out var l) ? int.Parse(l) : 10;
                var method = HelpSearchMethod.Hybrid;
                if (options.TryGetValue("method", out var m) && !Enum.TryParse(m, true, out method))
                {
                    Console.Error.WriteLine(Strings.UnknownSearchMethod(m));
                    return 2;
                }
                if (method != HelpSearchMethod.Bm25 && !HelpDocument.CanSearchVector)
                {
                    Console.Error.WriteLine(Strings.HelpVectorUnavailable);
                }
                var results = HelpDocument.Search(text, limit, method);
                foreach (var result in results)
                {
                    var topic = result.Topic;
                    Console.WriteLine((topic.Title == topic.Name ? topic.Name : topic.Name + "\t" + topic.Title) + "\t" + result.Score.ToString("0.0000", CultureInfo.InvariantCulture));
                    foreach (var (line, content) in result.Lines)
                    {
                        Console.WriteLine($"  {line}: {content}");
                    }
                }
                return results.Count > 0 ? 0 : 1;
            }
            if (options.TryGetValue("grep", out var pattern))
            {
                var found = false;
                foreach (var (topic, line, content) in HelpDocument.Grep(pattern))
                {
                    Console.WriteLine($"{topic.Name}:{line}: {content}");
                    found = true;
                }
                return found ? 0 : 1;
            }
            var query = string.Join(" ", topics);
            var matches = HelpDocument.Find(query);
            if (matches.Count == 1)
            {
                Console.Write(matches[0].Body);
                return 0;
            }
            if (matches.Count == 0)
            {
                Console.Error.WriteLine(Strings.HelpNotFound(query));
                return 1;
            }
            Console.Error.WriteLine(Strings.HelpAmbiguous(query));
            foreach (var topic in matches)
            {
                Console.Error.WriteLine("  " + (topic.Title == topic.Name ? topic.Name : topic.Name + "\t" + topic.Title));
            }
            return 1;
        }

        /// <summary>新しいシナリオのひな形を作る（7474/SRC#1640・#1652）。既にあるファイルは上書きしない（--force で上書き）</summary>
        private static int New(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count != 1)
            {
                Console.Error.WriteLine(Strings.NewRequiresFolder);
                return 2;
            }
            var episodes = 1;
            if (options.TryGetValue("episodes", out var epText))
            {
                if (!int.TryParse(epText, NumberStyles.Integer, CultureInfo.InvariantCulture, out episodes) || episodes < 1)
                {
                    Console.Error.WriteLine(Strings.NewInvalidEpisodes(epText));
                    return 2;
                }
            }
            var folder = Path.GetFullPath(positional[0]);
            options.TryGetValue("title", out var title);
            options.TryGetValue("app-path", out var appPath);
            var written = ScenarioTemplate.Write(folder, title ?? Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar)), appPath == null ? null : Path.GetFullPath(appPath), options.ContainsKey("force"), out var skipped, episodes);
            foreach (var path in written)
            {
                Console.WriteLine("  " + path);
            }
            Console.WriteLine(Strings.NewCreated(folder, written.Count));
            if (skipped.Count > 0)
            {
                Console.Error.WriteLine(Strings.NewSkipped(string.Join(", ", skipped)));
            }
            return 0;
        }

        /// <summary>話のつながりの図を出す（7474/SRC#1652）</summary>
        private static int Flow(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count > 1)
            {
                Console.Error.WriteLine(Strings.FlowRequiresAtMostOnePath);
                return 2;
            }
            var target = positional.Count == 1 ? Path.GetFullPath(positional[0]) : Environment.CurrentDirectory;
            if (!Directory.Exists(target) && !File.Exists(target))
            {
                Console.Error.WriteLine(Strings.FlowPathNotFound(target));
                return 2;
            }
            options.TryGetValue("app-path", out var appPath);
            options.TryGetValue("scenario-root", out var scenarioRoot);
            var analyzer = new ScenarioFlowAnalyzer
            {
                AppPath = string.IsNullOrEmpty(appPath) ? null : Path.GetFullPath(appPath),
            };
            var flow = analyzer.Analyze(target, scenarioRoot: string.IsNullOrEmpty(scenarioRoot) ? null : Path.GetFullPath(scenarioRoot));
            options.TryGetValue("format", out var format);
            switch ((format ?? "md").ToLowerInvariant())
            {
                case "md":
                case "markdown":
                    WriteOutput(flow.ToMarkdown(), options);
                    return 0;
                case "mmd":
                case "mermaid":
                    WriteOutput(flow.ToMermaid(), options);
                    return 0;
                case "json":
                    WriteOutput(flow.ToJson(), options);
                    return 0;
                default:
                    Console.Error.WriteLine(Strings.UnknownFlowFormat(format));
                    return 2;
            }
        }

        /// <summary>
        /// シナリオに必要な素材が揃っているかの一覧（7474/SRC#1654）。--fail-on-missing で見つからない素材があれば 1 を返す（CI 用）
        /// </summary>
        private static int Assets(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count > 1)
            {
                Console.Error.WriteLine(Strings.AssetsRequiresAtMostOneFolder);
                return 2;
            }
            var root = positional.Count == 1 ? Path.GetFullPath(positional[0]) : Environment.CurrentDirectory;
            if (!Directory.Exists(root))
            {
                Console.Error.WriteLine(Strings.AssetsFolderNotFound(root));
                return 2;
            }
            options.TryGetValue("format", out var format);
            format = (format ?? "md").ToLowerInvariant();
            if (format != "md" && format != "markdown" && format != "json" && format != "readme")
            {
                Console.Error.WriteLine(Strings.UnknownAssetsFormat(format));
                return 2;
            }
            options.TryGetValue("app-path", out var appPath);
            options.TryGetValue("ext-data", out var extData);
            options.TryGetValue("ext-data2", out var extData2);
            appPath = string.IsNullOrEmpty(appPath) ? ScenarioAnalyzer.FindAppPath(root) : Path.GetFullPath(appPath);
            var report = AssetReport.FromDirectories(
                root,
                appPath,
                string.IsNullOrEmpty(extData) ? null : Path.GetFullPath(extData),
                string.IsNullOrEmpty(extData2) ? null : Path.GetFullPath(extData2));
            WriteOutput(format == "json" ? report.ToJson() : format == "readme" ? report.ToReadme() : report.ToMarkdown(), options);
            if (!options.ContainsKey("output") && format != "json")
            {
                Console.Error.WriteLine(Strings.AssetsSummary(report.Assets.Count, report.MissingCount, report.Dynamic.Count, report.Unused.Count));
            }
            return options.ContainsKey("fail-on-missing") && report.MissingCount > 0 ? 1 : 0;
        }

        /// <summary>
        /// 試し戦闘のイベントをシナリオのフォルダの .srcsharp/trial/ に作る（7474/SRC#1643）。--list でデータのユニット・パイロットを JSON で出す
        /// （VS Code の「試し戦闘」が選ばせるのに使う）
        /// </summary>
        private static async Task<int> TrialAsync(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count != 1 || !Directory.Exists(positional[0]))
            {
                Console.Error.WriteLine(Strings.TrialRequiresFolder);
                return 2;
            }
            var root = Path.GetFullPath(positional[0]);
            var data = Path.Combine(root, "Data");
            options.TryGetValue("app-path", out var appPath);
            appPath ??= SRCAuthoring.Analysis.ScenarioAnalyzer.FindAppPath(root);
            var system = appPath == null || !Directory.Exists(Path.Combine(appPath, "Data", "System"))
                ? new List<string>()
                : Directory.GetFiles(Path.Combine(appPath, "Data", "System"), "*.txt").OrderBy(x => x, StringComparer.Ordinal).ToList();
            var lint = await new DataLinter { RuleGroups = Array.Empty<string>() }.LintAsync(Directory.Exists(data) ? new[] { data } : Array.Empty<string>(), root, system);
            var catalog = lint.Catalog;
            if (options.ContainsKey("list"))
            {
                var messageTypes = new System.Text.Json.Nodes.JsonObject();
                foreach (var mt in catalog.MessageTypes)
                {
                    var p = catalog.FindPilotOrMessageType(mt);
                    if (p != null)
                    {
                        messageTypes[mt] = p.Name;
                    }
                }
                var json = new System.Text.Json.Nodes.JsonObject
                {
                    ["units"] = new System.Text.Json.Nodes.JsonArray(catalog.Entries.Where(e => e.Kind == SRCAuthoring.Analysis.DataKind.Unit).Select(e => (System.Text.Json.Nodes.JsonNode)e.Name).ToArray()),
                    ["pilots"] = new System.Text.Json.Nodes.JsonArray(catalog.Entries.Where(e => e.Kind == SRCAuthoring.Analysis.DataKind.Pilot).Select(e => (System.Text.Json.Nodes.JsonNode)e.Name).ToArray()),
                    ["messageTypes"] = messageTypes,
                };
                Console.WriteLine(json.ToJsonString(new System.Text.Json.JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
                return 0;
            }
            options.TryGetValue("unit", out var unit);
            options.TryGetValue("pilot", out var pilot);
            options.TryGetValue("enemy", out var enemy);
            options.TryGetValue("enemy-pilot", out var enemyPilot);
            options.TryGetValue("focus", out var focus);
            var setup = TrialTemplate.Complete(new TrialSetup
            {
                Unit = unit,
                Pilot = pilot,
                EnemyUnit = enemy,
                EnemyPilot = enemyPilot,
                Level = options.TryGetValue("level", out var level) && int.TryParse(level, out var l) ? l : 30,
                Enemies = options.TryGetValue("enemies", out var enemies) && int.TryParse(enemies, out var n) ? n : 1,
                Focus = focus ?? "normal",
            }, catalog);
            var problem = TrialTemplate.Validate(setup, catalog);
            if (problem != null)
            {
                Console.Error.WriteLine(problem);
                return 1;
            }
            var eve = TrialTemplate.Write(root, setup);
            Console.WriteLine(eve);
            Console.Error.WriteLine(Strings.TrialCreated(setup.Unit, setup.Pilot, setup.EnemyUnit, setup.EnemyPilot));
            return 0;
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

        /// <summary>1 つのイベントファイル（と include・Require 先）の流れの図を出す（7474/SRC#1655）</summary>
        private static int EventFlow(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count != 1 || !File.Exists(positional[0]))
            {
                Console.Error.WriteLine(Strings.EventFlowRequiresEventFile(positional.Count > 0 ? positional[0] : ""));
                return 2;
            }
            var eve = Path.GetFullPath(positional[0]);
            options.TryGetValue("app-path", out var appPath);
            options.TryGetValue("scenario-root", out var scenarioRoot);
            var resolvedRoot = string.IsNullOrEmpty(scenarioRoot) ? null : Path.GetFullPath(scenarioRoot);
            var resolvedAppPath = string.IsNullOrEmpty(appPath) ? null : Path.GetFullPath(appPath);
            var analyzer = new EventFlowAnalyzer { AppPath = resolvedAppPath };
            var graph = analyzer.Analyze(eve, scenarioRoot: resolvedRoot);

            options.TryGetValue("format", out var format);
            switch ((format ?? "md").ToLowerInvariant())
            {
                case "md":
                case "markdown":
                    WriteOutput(graph.ToMarkdown(), options);
                    return 0;
                case "mmd":
                case "mermaid":
                    WriteOutput(graph.ToMermaid(), options);
                    return 0;
                case "json":
                    WriteOutput(graph.ToJson(), options);
                    return 0;
                default:
                    Console.Error.WriteLine(Strings.UnknownFlowFormat(format));
                    return 2;
            }
        }

        /// <summary>Talk・AutoTalk のブロックを SRCRender で描画してプレビューを出す（7474/SRC#1655）</summary>
        private static int TalkPreview(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count != 1)
            {
                Console.Error.WriteLine(Strings.TalkPreviewRequiresEventFile);
                return 2;
            }
            var eve = Path.GetFullPath(positional[0]);
            string content = null;
            if (options.ContainsKey("stdin"))
            {
                content = Console.In.ReadToEnd();
            }
            else if (options.TryGetValue("input", out var inputPath) && File.Exists(inputPath))
            {
                content = File.ReadAllText(inputPath);
            }
            else if (!File.Exists(eve))
            {
                Console.Error.WriteLine(Strings.EventFlowRequiresEventFile(eve));
                return 2;
            }

            var line = options.TryGetValue("line", out var lineStr) && int.TryParse(lineStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedLine)
                ? parsedLine
                : 1;
            int? blockIndex = options.TryGetValue("block", out var blockStr) && int.TryParse(blockStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedBlock)
                ? parsedBlock
                : null;
            options.TryGetValue("scenario-root", out var scenarioRoot);
            options.TryGetValue("app-path", out var appPath);

            var result = TalkPreviewRunner.Render(
                eve,
                text: content,
                cursorLine: line,
                blockIndex: blockIndex,
                scenarioRoot: string.IsNullOrEmpty(scenarioRoot) ? null : Path.GetFullPath(scenarioRoot),
                appPath: string.IsNullOrEmpty(appPath) ? null : Path.GetFullPath(appPath));

            options.TryGetValue("format", out var format);
            if (string.Equals(format, "png", StringComparison.OrdinalIgnoreCase))
            {
                var outDir = options.TryGetValue("out", out var dir) ? Path.GetFullPath(dir) : Environment.CurrentDirectory;
                Directory.CreateDirectory(outDir);
                for (var i = 0; i < result.Pages.Count; i++)
                {
                    var page = result.Pages[i];
                    var path = Path.Combine(outDir, $"page-{i:D3}.png");
                    if (page.PngBytes != null && page.PngBytes.Length > 0)
                    {
                        File.WriteAllBytes(path, page.PngBytes);
                    }
                    Console.WriteLine(path);
                }
                Console.Error.WriteLine(Strings.TalkPreviewPagesWritten(result.Pages.Count, outDir));
                return 0;
            }

            WriteOutput(result.ToJson(!options.ContainsKey("no-images")), options);
            return 0;
        }

        /// <summary>
        /// 戦闘アニメ・特殊効果・汎用戦闘アニメの 1 定義を固定の攻撃側・防御側で再生する準備ファイルを作り、
        /// --play-once があれば 1 回再生する（7474/SRC#1655）
        /// </summary>
        private static async Task<int> AnimePreviewAsync(List<string> positional, Dictionary<string, string> options)
        {
            if (positional.Count > 1 || (positional.Count == 0 && !options.ContainsKey("file")))
            {
                Console.Error.WriteLine(Strings.AnimePreviewPathNotFound(positional.FirstOrDefault() ?? ""));
                return 2;
            }

            var targetPath = positional.Count == 1 ? Path.GetFullPath(positional[0]) : Path.GetFullPath(options["file"]);
            if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
            {
                Console.Error.WriteLine(Strings.AnimePreviewPathNotFound(targetPath));
                return 2;
            }

            var filePath = options.TryGetValue("file", out var explicitFile)
                ? Path.GetFullPath(explicitFile)
                : (File.Exists(targetPath) ? targetPath : null);
            var scenarioRoot = options.TryGetValue("scenario-root", out var rootOpt) && !string.IsNullOrEmpty(rootOpt)
                ? Path.GetFullPath(rootOpt)
                : (Directory.Exists(targetPath) ? targetPath : ScenarioAnalyzer.FindRunRoot(filePath));
            options.TryGetValue("app-path", out var appPathOpt);
            var appPath = !string.IsNullOrEmpty(appPathOpt)
                ? Path.GetFullPath(appPathOpt)
                : ScenarioAnalyzer.FindAppPath(scenarioRoot) ?? scenarioRoot;

            var dataDir = Path.Combine(scenarioRoot, "Data");
            var systemDir = Path.Combine(appPath, "Data", "System");
            var systemFiles = Directory.Exists(systemDir)
                ? Directory.GetFiles(systemDir, "*.txt").OrderBy(x => x, StringComparer.Ordinal).ToList()
                : new List<string>();
            var lint = await new DataLinter { RuleGroups = Array.Empty<string>() }
                .LintAsync(Directory.Exists(dataDir) ? new[] { dataDir } : Array.Empty<string>(), scenarioRoot, systemFiles);
            var catalog = lint.Catalog;

            options.TryGetValue("kind", out var kindOpt);
            var entries = filePath != null && File.Exists(filePath)
                ? AnimationDataParser.ParseFile(filePath, overrideKind: kindOpt)
                : new List<AnimationPreviewEntry>();

            var cursorLine = options.TryGetValue("line", out var lineStr) && int.TryParse(lineStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedLine)
                ? parsedLine
                : 0;
            var selected = AnimationDataParser.SelectAtLine(entries, cursorLine);
            var hasSit = options.TryGetValue("situation", out var sitOpt);
            var hasSpec = options.TryGetValue("spec", out var specOpt);
            var hasTgt = options.TryGetValue("target", out var tgtOpt);
            if (hasSit || hasSpec || hasTgt)
            {
                selected = new AnimationPreviewEntry
                {
                    Kind = kindOpt ?? selected?.Kind ?? AnimationPreviewKind.Animation,
                    File = filePath ?? selected?.File ?? "",
                    Line = cursorLine > 0 ? cursorLine : (selected?.Line ?? 0),
                    Target = tgtOpt ?? selected?.Target ?? "汎用",
                    Situation = sitOpt ?? selected?.Situation ?? "",
                    Spec = specOpt ?? selected?.Spec ?? "",
                };
            }

            options.TryGetValue("attacker", out var attackerUnit);
            options.TryGetValue("attacker-pilot", out var attackerPilot);
            options.TryGetValue("defender", out var defenderUnit);
            options.TryGetValue("defender-pilot", out var defenderPilot);
            var setup = AnimationPreviewTemplate.Complete(new AnimationPreviewSetup
            {
                Unit = attackerUnit,
                Pilot = attackerPilot,
                EnemyUnit = defenderUnit,
                EnemyPilot = defenderPilot,
            }, catalog, selected?.Target);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            if (options.ContainsKey("list"))
            {
                var selectedIdx = selected != null ? entries.IndexOf(selected) : -1;
                var listJson = new JsonObject
                {
                    ["file"] = filePath ?? "",
                    ["scenarioRoot"] = scenarioRoot,
                    ["appPath"] = appPath,
                    ["defaultSetup"] = new JsonObject
                    {
                        ["attackerUnit"] = setup.Unit,
                        ["attackerPilot"] = setup.Pilot,
                        ["defenderUnit"] = setup.EnemyUnit,
                        ["defenderPilot"] = setup.EnemyPilot,
                    },
                    ["units"] = new JsonArray(catalog.Entries.Where(e => e.Kind == DataKind.Unit).Select(e => (JsonNode)e.Name).ToArray()),
                    ["pilots"] = new JsonArray(catalog.Entries.Where(e => e.Kind == DataKind.Pilot).Select(e => (JsonNode)e.Name).ToArray()),
                    ["selectedIndex"] = selectedIdx,
                    ["entries"] = new JsonArray(entries.Select((e, idx) => (JsonNode)new JsonObject
                    {
                        ["index"] = idx,
                        ["kind"] = e.Kind,
                        ["file"] = e.File,
                        ["line"] = e.Line,
                        ["target"] = e.Target,
                        ["situation"] = e.Situation,
                        ["spec"] = e.Spec,
                    }).ToArray()),
                };
                WriteOutput(listJson.ToJsonString(jsonOptions), options);
                return 0;
            }

            if (selected == null)
            {
                Console.Error.WriteLine(Strings.AnimePreviewNoEntry);
                return 1;
            }

            var previewEve = AnimationPreviewTemplate.Write(scenarioRoot, setup, catalog);
            var animPreviewObj = new JsonObject
            {
                ["kind"] = selected.Kind,
                ["file"] = selected.File,
                ["line"] = selected.Line,
                ["target"] = selected.Target,
                ["situation"] = selected.Situation,
                ["spec"] = selected.Spec,
                ["attackerUnit"] = setup.Unit,
                ["attackerPilot"] = setup.Pilot,
                ["defenderUnit"] = setup.EnemyUnit,
                ["defenderPilot"] = setup.EnemyPilot,
            };

            if (!options.ContainsKey("play-once"))
            {
                var prepJson = new JsonObject
                {
                    ["program"] = previewEve,
                    ["scenarioRoot"] = scenarioRoot,
                    ["appPath"] = appPath,
                    ["animationPreview"] = animPreviewObj,
                };
                WriteOutput(prepJson.ToJsonString(jsonOptions), options);
                return 0;
            }

            var prevSkipWait = SRCCore.Lib.GeneralLib.SkipWait;
            SRCCore.Lib.GeneralLib.SkipWait = options.ContainsKey("skip-wait");
            try
            {
                using (var logHost = CreateLogHost("anime-preview", LogOutputs.StandardError))
                using (var runner = new ScreenScenarioRunner(appPath, logHost.LoggerFactory))
                {
                    runner.Start(previewEve, scenarioRoot);
                    var waitDeadline = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                    while (!runner.IsFinished && DateTime.UtcNow < waitDeadline)
                    {
                        var s = runner.Session.SRC;
                        if (s.Turn >= 1 && s.Stage == "味方" && !runner.GUI.IsGUILocked && s.Commands.CommandState == "ユニット選択")
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }

                    var req = new AnimationPreviewRequest
                    {
                        Kind = selected.Kind,
                        File = selected.File,
                        Line = selected.Line,
                        Target = selected.Target,
                        Situation = selected.Situation,
                        Spec = selected.Spec,
                        AttackerUnit = setup.Unit,
                        AttackerPilot = setup.Pilot,
                        DefenderUnit = setup.EnemyUnit,
                        DefenderPilot = setup.EnemyPilot,
                    };

                    var tcs = new TaskCompletionSource<AnimationPreviewPlayResult>(TaskCreationOptions.RunContinuationsAsynchronously);
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            tcs.TrySetResult(AnimationPreviewRunner.Play(runner.Session.SRC, req));
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    });
                    var playResult = await tcs.Task;
                    if (options.TryGetValue("out", out var outPng) && !string.IsNullOrEmpty(outPng))
                    {
                        using var fs = File.Create(Path.GetFullPath(outPng));
                        runner.GUI.SnapshotPng(fs);
                    }
                    runner.Session.GUI.Terminate();

                    var resultJson = new JsonObject
                    {
                        ["program"] = previewEve,
                        ["scenarioRoot"] = scenarioRoot,
                        ["appPath"] = appPath,
                        ["played"] = playResult.Played,
                        ["message"] = playResult.Message ?? "",
                        ["attacker"] = playResult.Attacker ?? "",
                        ["defender"] = playResult.Defender ?? "",
                        ["warnings"] = new JsonArray(playResult.Warnings.Select(w => (JsonNode)w).ToArray()),
                        ["animationPreview"] = animPreviewObj,
                    };
                    WriteOutput(resultJson.ToJsonString(jsonOptions), options);
                    return playResult.Played ? 0 : 1;
                }
            }
            finally
            {
                SRCCore.Lib.GeneralLib.SkipWait = prevSkipWait;
            }
        }
    }
}
