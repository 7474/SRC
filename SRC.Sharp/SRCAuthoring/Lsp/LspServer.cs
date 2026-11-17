using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SRCAuthoring.Analysis;
using SRCAuthoring.Linting;
using SRCAuthoring.Maps;
using SRCAuthoring.Protocol;
using SRCAuthoring.Rules;
using SRCCore.Events;
using SRCCore.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCAuthoring.Lsp
{
    /// <summary>
    /// イベントファイル（*.eve）とデータ（robot.txt・pilot.txt など）の Language Server。
    /// データは保存した内容で、シナリオの Data のフォルダ全体を DataLinter（srcsharp lint と同じ）で調べる。
    /// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/
    /// </summary>
    public sealed class LspServer
    {
        private readonly MessageStream stream;
        private readonly ScenarioAnalyzer analyzer = new ScenarioAnalyzer();
        private readonly ConcurrentDictionary<string, string> documents = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, ScenarioAnalysis> analyses = new ConcurrentDictionary<string, ScenarioAnalysis>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, CancellationTokenSource> pending = new ConcurrentDictionary<string, CancellationTokenSource>(StringComparer.OrdinalIgnoreCase);
        private bool shutdown;
        // データの診断を送ったファイル（Data のフォルダ → ファイル）。直ったファイルの診断を消すのに使う
        private readonly ConcurrentDictionary<string, HashSet<string>> dataPublished = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>データに動かす規則のグループ（初期化オプションの rules。VS Code 拡張の srcsharp.rules）</summary>
        public IReadOnlyList<string> RuleGroups { get; set; } = Rules.RuleGroups.Default;
        private readonly ILogger log;
        private readonly Action<LogLevel?> setLogLevel;

        /// <summary>変更してから解析するまでの待ち</summary>
        public TimeSpan Debounce { get; set; } = TimeSpan.FromMilliseconds(800);

        // 解析は重い（大きなシナリオでは include 先を含めて数十万行）ので、同時には 1 つだけ行う
        private readonly SemaphoreSlim analyzing = new SemaphoreSlim(1, 1);

        /// <param name="loggerFactory">ログの出力先。ログは window/logMessage でクライアントにも送る</param>
        /// <param name="setLogLevel">初期化オプションの logLevel を反映する</param>
        /// <summary>ヘルプの説明（コマンド・特殊能力・属性のホバーと補完）。srcsharp が同梱のヘルプから作って渡す</summary>
        public HelpReference Help { get; set; } = HelpReference.Empty;

        /// <summary>シナリオのフォルダごとの、データの名前の一覧（データを調べたときに作り直す）</summary>
        private readonly ConcurrentDictionary<string, DataCatalog> catalogs = new ConcurrentDictionary<string, DataCatalog>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, TerrainCatalog> terrainCatalogs = new ConcurrentDictionary<string, TerrainCatalog>(StringComparer.OrdinalIgnoreCase);

        public LspServer(Stream input, Stream output, ILoggerFactory loggerFactory = null, Action<LogLevel?> setLogLevel = null)
        {
            stream = new MessageStream(input, output);
            this.setLogLevel = setLogLevel;
            if (loggerFactory != null)
            {
                loggerFactory.AddProvider(new DelegateLoggerProvider((category, level, message, ex) =>
                    _ = stream.WriteAsync(new JsonObject
                    {
                        ["jsonrpc"] = "2.0",
                        ["method"] = "window/logMessage",
                        ["params"] = new JsonObject
                        {
                            // 1: Error、2: Warning、3: Info、4: Log
                            ["type"] = level >= LogLevel.Error ? 1 : level == LogLevel.Warning ? 2 : level == LogLevel.Information ? 3 : 4,
                            ["message"] = $"[{DelegateLoggerProvider.ShortLevel(level)}] {category}: {message}" + (ex == null ? "" : "\n" + ex),
                        },
                    })));
                log = loggerFactory.CreateLogger("SRCSharp.Lsp");
            }
            else
            {
                log = NullLogger.Instance;
            }
        }

        /// <summary>exit を受け取るか入力が終わるまで処理する。戻り値は終了コード</summary>
        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await stream.ReadAsync(cancellationToken);
                if (message == null)
                {
                    return shutdown ? 0 : 1;
                }
                var method = message["method"]?.GetValue<string>();
                var id = message["id"]?.DeepClone();
                var @params = message["params"];
                if (method == "exit")
                {
                    return shutdown ? 0 : 1;
                }
                try
                {
                    var result = Handle(method, @params);
                    if (id != null)
                    {
                        await stream.WriteAsync(new JsonObject { ["jsonrpc"] = "2.0", ["id"] = id, ["result"] = result });
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "{Method} を処理できません", method);
                    if (id != null)
                    {
                        await stream.WriteAsync(new JsonObject
                        {
                            ["jsonrpc"] = "2.0",
                            ["id"] = id,
                            ["error"] = new JsonObject { ["code"] = -32603, ["message"] = ex.Message },
                        });
                    }
                }
            }
            return 0;
        }

        private JsonNode Handle(string method, JsonNode @params)
        {
            switch (method)
            {
                case "initialize":
                    var appPath = @params?["initializationOptions"]?["appPath"]?.GetValue<string>();
                    if (!string.IsNullOrEmpty(appPath))
                    {
                        analyzer.AppPath = appPath;
                    }
                    // ログのレベル（VS Code 拡張の srcsharp.logLevel）
                    var logLevel = LogSettings.ParseLevel(@params?["initializationOptions"]?["logLevel"]?.GetValue<string>());
                    if (logLevel.HasValue)
                    {
                        setLogLevel?.Invoke(logLevel);
                    }
                    // 規則のグループ（VS Code 拡張の srcsharp.rules。文字列か配列）
                    var rules = @params?["initializationOptions"]?["rules"];
                    if (rules != null)
                    {
                        try
                        {
                            RuleGroups = Rules.RuleGroups.Parse(rules is JsonArray array
                                ? string.Join(",", array.Select(x => x?.GetValue<string>()))
                                : rules.GetValue<string>());
                        }
                        catch (ArgumentException ex)
                        {
                            log.LogWarning("規則のグループを読めません: {Message}", ex.Message);
                        }
                    }
                    log.LogInformation("Language Server を開始 SRC のフォルダ: {AppPath} 規則: {Rules}", analyzer.AppPath ?? "", string.Join(",", RuleGroups));
                    return new JsonObject
                    {
                        ["capabilities"] = new JsonObject
                        {
                            // 1: 全文で同期
                            ["textDocumentSync"] = new JsonObject { ["openClose"] = true, ["change"] = 1, ["save"] = true },
                            ["documentSymbolProvider"] = true,
                            ["definitionProvider"] = true,
                            ["hoverProvider"] = true,
                            ["completionProvider"] = new JsonObject { ["triggerCharacters"] = new JsonArray() },
                        },
                        ["serverInfo"] = new JsonObject { ["name"] = "srcsharp" },
                    };
                case "shutdown":
                    shutdown = true;
                    return null;
                case "textDocument/didOpen":
                    {
                        var doc = @params["textDocument"];
                        var path = UriToPath(doc["uri"].GetValue<string>());
                        documents[path] = doc["text"].GetValue<string>();
                        if (DataLinter.IsDataFileName(path))
                        {
                            ScheduleData(path);
                        }
                        else
                        {
                            Schedule(path, TimeSpan.Zero);
                        }
                        return null;
                    }
                case "textDocument/didChange":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        var changes = @params["contentChanges"].AsArray();
                        documents[path] = changes[changes.Count - 1]["text"].GetValue<string>();
                        // データは保存した内容で調べる
                        if (!DataLinter.IsDataFileName(path))
                        {
                            Schedule(path, Debounce);
                        }
                        return null;
                    }
                case "textDocument/didSave":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        if (DataLinter.IsDataFileName(path))
                        {
                            ScheduleData(path);
                        }
                        else
                        {
                            Schedule(path, TimeSpan.Zero);
                        }
                        return null;
                    }
                case "textDocument/didClose":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        documents.TryRemove(path, out _);
                        analyses.TryRemove(path, out _);
                        // データの診断はフォルダ全体のものなので、閉じても消さない
                        if (!DataLinter.IsDataFileName(path))
                        {
                            _ = PublishAsync(path, new List<SrcDiagnostic>());
                        }
                        return null;
                    }
                case "textDocument/documentSymbol":
                    return DocumentSymbols(UriToPath(@params["textDocument"]["uri"].GetValue<string>()));
                case "textDocument/definition":
                    return Definition(@params);
                case "textDocument/hover":
                    return Hover(@params);
                case "textDocument/completion":
                    return Completion(@params);
                case "srcsharp/placementAt":
                    return PlacementAt(@params);
                default:
                    if (method != null && method.StartsWith("$/"))
                    {
                        return null;
                    }
                    if (method == "initialized" || method == "workspace/didChangeConfiguration")
                    {
                        return null;
                    }
                    throw new NotSupportedException(method);
            }
        }

        public static string UriToPath(string uri)
        {
            return OverlayFileSystem.Normalize(new Uri(uri).LocalPath);
        }

        public static string PathToUri(string path)
        {
            return new Uri(OverlayFileSystem.Normalize(path)).AbsoluteUri;
        }

        private void Schedule(string path, TimeSpan delay)
        {
            var cts = new CancellationTokenSource();
            if (pending.TryRemove(path, out var old))
            {
                old.Cancel();
            }
            pending[path] = cts;
            _ = Task.Run(async () =>
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cts.Token);
                    }
                    await analyzing.WaitAsync(cts.Token);
                    ScenarioAnalysis analysis;
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        analysis = Analyze(path);
                    }
                    finally
                    {
                        analyzing.Release();
                    }
                    if (cts.IsCancellationRequested)
                    {
                        return;
                    }
                    await PublishAsync(path, analysis.DiagnosticsFor(path).ToList());
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "解析できません {Path}", path);
                }
            });
        }

        /// <summary>データのファイルのある Data のフォルダ（いちばん近い親の Data。無ければファイルのあるフォルダ）</summary>
        public static string DataRootOf(string file)
        {
            for (var d = Path.GetDirectoryName(file); !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
            {
                if (string.Equals(Path.GetFileName(d), "Data", StringComparison.OrdinalIgnoreCase))
                {
                    return d;
                }
            }
            return Path.GetDirectoryName(file);
        }

        private void ScheduleData(string path)
        {
            var root = DataRootOf(path);
            var key = "data:" + root;
            var cts = new CancellationTokenSource();
            if (pending.TryRemove(key, out var old))
            {
                old.Cancel();
            }
            pending[key] = cts;
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyzing.WaitAsync(cts.Token);
                    LintResult result;
                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        result = await LintDataAsync(root);
                    }
                    finally
                    {
                        analyzing.Release();
                    }
                    catalogs[Path.GetDirectoryName(root)] = result.Catalog;
                    if (!cts.IsCancellationRequested)
                    {
                        await PublishDataAsync(root, result);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "データを調べられません {Path}", root);
                }
            });
        }

        /// <summary>Data のフォルダを srcsharp lint と同じく調べる。SRC 本体の Data\System のデータを先に読む</summary>
        public Task<LintResult> LintDataAsync(string dataRoot)
        {
            var scenarioRoot = Path.GetDirectoryName(dataRoot);
            var appPath = analyzer.AppPath ?? ScenarioAnalyzer.FindAppPath(scenarioRoot);
            var system = new List<string>();
            var systemDir = appPath == null ? null : Path.Combine(appPath, "Data", "System");
            if (systemDir != null && Directory.Exists(systemDir) && !ScenarioAnalysis.SamePath(Path.GetFullPath(systemDir), Path.GetFullPath(Path.Combine(dataRoot, "System"))))
            {
                system.AddRange(Directory.EnumerateFiles(systemDir, "*.txt"));
            }
            return new DataLinter { RuleGroups = RuleGroups }.LintAsync(new[] { dataRoot }, scenarioRoot, system);
        }

        private async Task PublishDataAsync(string dataRoot, LintResult result)
        {
            // データのファイルの診断だけを送る（イベントの診断はイベントの解析が送る）
            var byFile = result.Diagnostics
                .Where(x => !string.IsNullOrEmpty(x.File) && DataLinter.IsDataFileName(x.File))
                .GroupBy(x => OverlayFileSystem.Normalize(x.File), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
            var previous = dataPublished.TryGetValue(dataRoot, out var p) ? p : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in previous.Where(x => !byFile.ContainsKey(x)))
            {
                await PublishAsync(file, new List<SrcDiagnostic>());
            }
            foreach (var (file, diagnostics) in byFile)
            {
                await PublishAsync(file, diagnostics);
            }
            dataPublished[dataRoot] = new HashSet<string>(byFile.Keys, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>path を開いている内容で解析し、結果を覚えておく</summary>
        public ScenarioAnalysis Analyze(string path)
        {
            var analysis = analyzer.Analyze(path, documents.ToDictionary(x => x.Key, x => x.Value));
            // イベントのコマンドが名前で指すデータ（ユニット・パイロット・アイテム）が、シナリオのデータにあるか（7474/SRC#1642）
            var catalog = CatalogFor(analysis.ScenarioRoot);
            analysis.Diagnostics.AddRange(DataReferenceChecker.Check(DataReferenceChecker.Find(path, DocumentText(path)), catalog));
            analyses[path] = analysis;
            return analysis;
        }

        /// <summary>シナリオのデータの名前の一覧。まだ無ければ Data のフォルダを調べて作る</summary>
        public DataCatalog CatalogFor(string scenarioRoot)
        {
            if (string.IsNullOrEmpty(scenarioRoot))
            {
                return DataCatalog.Empty;
            }
            return catalogs.GetOrAdd(scenarioRoot, root =>
            {
                var data = Path.Combine(root, "Data");
                if (!Directory.Exists(data))
                {
                    return DataCatalog.Empty;
                }
                try
                {
                    return LintDataAsync(data).GetAwaiter().GetResult().Catalog;
                }
                catch (Exception ex)
                {
                    log.LogWarning(ex, "データを読めません {Path}", data);
                    return DataCatalog.Empty;
                }
            });
        }

        private string DocumentText(string path)
        {
            if (documents.TryGetValue(path, out var text))
            {
                return text;
            }
            return File.Exists(path) ? DataReferenceText.Read(path) : "";
        }

        private ScenarioAnalysis AnalysisFor(string path)
        {
            return analyses.TryGetValue(path, out var analysis) ? analysis : Analyze(path);
        }

        private Task PublishAsync(string path, IList<SrcDiagnostic> diagnostics)
        {
            var items = new JsonArray();
            foreach (var d in diagnostics)
            {
                var item = new JsonObject
                {
                    ["range"] = LineRange(d.Line),
                    ["severity"] = d.Severity switch { DiagnosticSeverity.Error => 1, DiagnosticSeverity.Warning => 2, _ => 3 },
                    ["source"] = "srcsharp",
                    // データの診断には、どのデータか（ユニット・パイロットの名前）を付ける
                    ["message"] = d.Message + (string.IsNullOrEmpty(d.Source) || !DataLinter.IsDataFileName(path) ? "" : $" [{d.Source}]"),
                };
                if (!string.IsNullOrEmpty(d.RuleId))
                {
                    item["code"] = d.RuleId;
                }
                items.Add(item);
            }
            return stream.WriteAsync(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "textDocument/publishDiagnostics",
                ["params"] = new JsonObject { ["uri"] = PathToUri(path), ["diagnostics"] = items },
            });
        }

        /// <summary>1 始まりの行の全体</summary>
        private static JsonObject LineRange(int line, int startCharacter = 0, int endCharacter = 1000)
        {
            var l = Math.Max(0, line - 1);
            return new JsonObject
            {
                ["start"] = new JsonObject { ["line"] = l, ["character"] = startCharacter },
                ["end"] = new JsonObject { ["line"] = l, ["character"] = endCharacter },
            };
        }

        private JsonNode DocumentSymbols(string path)
        {
            var analysis = AnalysisFor(path);
            var symbols = new JsonArray();
            foreach (var label in analysis.Labels.Where(x => ScenarioAnalysis.SamePath(x.File, path)))
            {
                var range = LineRange(label.Line, 0, label.Name.Length + 1);
                symbols.Add(new JsonObject
                {
                    ["name"] = label.Name,
                    // 12: Function
                    ["kind"] = 12,
                    ["range"] = range,
                    ["selectionRange"] = range.DeepClone(),
                });
            }
            return symbols;
        }

        private string LineText(string path, int line)
        {
            string text = null;
            if (!documents.TryGetValue(path, out text) && File.Exists(path))
            {
                text = File.ReadAllText(path);
            }
            var lines = (text ?? "").Replace("\r\n", "\n").Split('\n');
            return line >= 0 && line < lines.Length ? lines[line] : "";
        }

        private static readonly char[] Separators = { ' ', '\t', ',', '(', ')', '"', '　', '<', '>', '[', ']' };

        /// <summary>行の character の位置にある語</summary>
        public static string WordAt(string text, int character)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            character = Math.Max(0, Math.Min(character, text.Length));
            var start = character;
            while (start > 0 && Array.IndexOf(Separators, text[start - 1]) < 0)
            {
                start--;
            }
            var end = character;
            while (end < text.Length && Array.IndexOf(Separators, text[end]) < 0)
            {
                end++;
            }
            return text.Substring(start, end - start).TrimEnd(':');
        }

        private (string path, int line, int character) Position(JsonNode @params)
        {
            return (
                UriToPath(@params["textDocument"]["uri"].GetValue<string>()),
                @params["position"]["line"].GetValue<int>(),
                @params["position"]["character"].GetValue<int>());
        }

        private JsonNode Definition(JsonNode @params)
        {
            var (path, line, character) = Position(@params);
            var analysis = AnalysisFor(path);
            var include = analysis.Includes.FirstOrDefault(x => ScenarioAnalysis.SamePath(x.File, path) && x.Line == line + 1);
            if (include?.ResolvedPath != null)
            {
                return Location(include.ResolvedPath, 1);
            }
            var word = WordAt(LineText(path, line), character);
            var label = analysis.Labels.FirstOrDefault(x => x.Name == word);
            if (label != null)
            {
                return Location(label.File, label.Line);
            }
            // データの名前（Create・Unit・Pilot・Item の引数）からデータの定義へ
            var entry = DataAt(path, line, character, analysis);
            return entry?.File == null ? null : Location(entry.File, entry.Line);
        }

        /// <summary>イベントの line 行（0 始まり）の character の位置にある、データの名前の参照が指すデータ</summary>
        private DataEntry DataAt(string path, int line, int character, ScenarioAnalysis analysis)
        {
            var reference = DataReferenceChecker.Find(path, DocumentText(path))
                .FirstOrDefault(r => r.Line == line + 1 && character >= r.Column && character <= r.Column + r.Length);
            return reference == null ? null : CatalogFor(analysis.ScenarioRoot).Find(reference.Kind, reference.Name);
        }

        /// <summary>行の最初の語（コマンド）にカーソルがあれば、その語</summary>
        private static string CommandAt(string text, int character)
        {
            var trimmed = text.TrimStart();
            var start = text.Length - trimmed.Length;
            var end = start;
            while (end < text.Length && !char.IsWhiteSpace(text[end]))
            {
                end++;
            }
            return character >= start && character <= end && end > start ? text.Substring(start, end - start) : null;
        }

        private static JsonObject Markdown(string value) => new JsonObject
        {
            ["contents"] = new JsonObject { ["kind"] = "markdown", ["value"] = value },
        };

        private static JsonObject Location(string file, int line)
        {
            return new JsonObject { ["uri"] = PathToUri(file), ["range"] = LineRange(line, 0, 0) };
        }

        private JsonNode Hover(JsonNode @params)
        {
            var (path, line, character) = Position(@params);
            var text = LineText(path, line);
            // データのファイル: 特殊能力・武器の属性の説明（ヘルプ）
            if (DataLinter.IsDataFileName(path))
            {
                var keyword = Help.Keyword(WordAt(text, character));
                return keyword == null ? null : Markdown(keyword);
            }
            var analysis = AnalysisFor(path);
            var word = WordAt(text, character);
            var label = analysis.Labels.FirstOrDefault(x => x.Name == word);
            if (label != null)
            {
                return Markdown($"ラベル `{label.Name}`  \n{Path.GetFileName(label.File)}:{label.Line}");
            }
            var info = InspectLinePlacement(path, line + 1, analysis.ScenarioRoot);
            var entry = DataAt(path, line, character, analysis);
            if (entry != null)
            {
                var summary = entry.Summary + (entry.File == null ? "" : $"  \n{Path.GetFileName(entry.File)}:{entry.Line}");
                if (info != null)
                {
                    summary += "\n\n---\n\n" + FormatPlacementMarkdown(info);
                }
                return Markdown(summary);
            }
            var command = Help.Command(CommandAt(text, character));
            if (info != null)
            {
                var coord = FormatPlacementMarkdown(info);
                return Markdown(command == null ? coord : command + "\n\n---\n\n" + coord);
            }
            return command == null ? null : Markdown(command);
        }

        private JsonNode PlacementAt(JsonNode @params)
        {
            var (path, line, _) = Position(@params);
            if (DataLinter.IsDataFileName(path))
            {
                return null;
            }
            var scenarioRoot = ScenarioAnalyzer.FindScenarioRoot(path);
            var info = InspectLinePlacement(path, line + 1, scenarioRoot);
            if (info == null)
            {
                return null;
            }
            return new JsonObject
            {
                ["mapPath"] = info.Item.MapPath ?? "",
                ["fullMapPath"] = info.FullMapPath ?? "",
                ["x"] = info.Item.X,
                ["y"] = info.Item.Y,
                ["command"] = info.Item.Command,
                ["party"] = info.Item.Party ?? "",
                ["unit"] = info.Item.Unit ?? "",
                ["pilot"] = info.Item.Pilot ?? "",
                ["label"] = info.Item.Label ?? "",
                ["terrainName"] = info.TerrainName ?? "",
                ["file"] = info.Item.File,
                ["line"] = info.Item.Line,
                ["editable"] = info.Item.Editable,
            };
        }

        private sealed class LinePlacementDetail
        {
            public PlacementItem Item { get; set; }
            public string FullMapPath { get; set; }
            public string TerrainName { get; set; }
        }

        private LinePlacementDetail InspectLinePlacement(string path, int lineNumber, string scenarioRoot)
        {
            var text = DocumentText(path);
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var relPath = !string.IsNullOrEmpty(scenarioRoot) && path.StartsWith(scenarioRoot, StringComparison.OrdinalIgnoreCase)
                ? EventPlacementParser.NormalizePath(Path.GetRelativePath(scenarioRoot, path))
                : Path.GetFileName(path);
            var item = EventPlacementParser.InspectLine(relPath, text, lineNumber);
            if (item == null)
            {
                return null;
            }
            string fullMapPath = null;
            string terrainName = null;
            if (!string.IsNullOrEmpty(scenarioRoot) && !string.IsNullOrEmpty(item.MapPath))
            {
                var candidate = Path.Combine(scenarioRoot, item.MapPath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(candidate))
                {
                    fullMapPath = OverlayFileSystem.Normalize(candidate);
                    terrainName = ResolveTerrainName(scenarioRoot, candidate, item.X, item.Y);
                }
            }
            return new LinePlacementDetail { Item = item, FullMapPath = fullMapPath, TerrainName = terrainName };
        }

        private string ResolveTerrainName(string scenarioRoot, string mapFile, int x, int y)
        {
            try
            {
                var doc = MapDocument.Parse(File.ReadAllBytes(mapFile));
                if (x < 1 || y < 1 || x > doc.Width || y > doc.Height)
                {
                    return null;
                }
                var tile = doc.Layer != null && !doc.Layer[x - 1, y - 1].IsEmpty
                    ? doc.Layer[x - 1, y - 1]
                    : doc.Tiles[x - 1, y - 1];
                var terrains = GetTerrainCatalog(scenarioRoot);
                var info = terrains.Terrains.FirstOrDefault(t => t.Id == tile.Terrain);
                return info != null ? $"{info.Name} ({tile.Terrain})" : tile.Terrain.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private TerrainCatalog GetTerrainCatalog(string scenarioRoot)
        {
            return terrainCatalogs.GetOrAdd(scenarioRoot ?? "", root =>
            {
                var files = new List<(string, byte[])>();
                var appPath = analyzer.AppPath ?? ScenarioAnalyzer.FindAppPath(root) ?? AppContext.BaseDirectory;
                foreach (var dir in new[] { appPath, root }.Where(d => !string.IsNullOrEmpty(d)))
                {
                    var terrainFile = Path.Combine(dir, "Data", "System", "terrain.txt");
                    if (File.Exists(terrainFile))
                    {
                        files.Add((terrainFile, File.ReadAllBytes(terrainFile)));
                    }
                }
                return TerrainCatalog.Load(files);
            });
        }

        private static string FormatPlacementMarkdown(LinePlacementDetail detail)
        {
            var item = detail.Item;
            var mapPart = string.IsNullOrEmpty(item.MapPath) ? "（マップ未指定）" : $"`{item.MapPath}`";
            var terrainPart = string.IsNullOrEmpty(detail.TerrainName) ? "" : $"・地形: {detail.TerrainName}";
            var who = string.IsNullOrEmpty(item.Unit) ? "" : $" — {item.Unit}";
            return $"マップ {mapPart} の座標 **({item.X}, {item.Y})**{terrainPart}{who}";
        }

        /// <summary>イベントコマンドの名前（CmdType から）</summary>
        public static IEnumerable<string> CommandNames()
        {
            return Enum.GetNames(typeof(CmdType))
                .Where(x => x.EndsWith("Cmd") && x != "NopCmd" && x != "NullCmd")
                .Select(x => x.Substring(0, x.Length - 3))
                .Distinct()
                .OrderBy(x => x);
        }

        private JsonNode Completion(JsonNode @params)
        {
            var (path, _, _) = Position(@params);
            var items = new JsonArray();
            // データのファイル: 特殊能力・武器の属性（ヘルプにあるもの）
            if (DataLinter.IsDataFileName(path))
            {
                foreach (var keyword in Help.Keywords.OrderBy(x => x, StringComparer.Ordinal))
                {
                    // 14: Keyword
                    items.Add(new JsonObject
                    {
                        ["label"] = keyword,
                        ["kind"] = 14,
                        ["documentation"] = new JsonObject { ["kind"] = "markdown", ["value"] = Help.Keyword(keyword) },
                    });
                }
                return items;
            }
            foreach (var name in CommandNames())
            {
                // 14: Keyword
                var item = new JsonObject { ["label"] = name, ["kind"] = 14 };
                var doc = Help.Command(name);
                if (doc != null)
                {
                    item["documentation"] = new JsonObject { ["kind"] = "markdown", ["value"] = doc };
                }
                items.Add(item);
            }
            if (analyses.TryGetValue(path, out var analysis))
            {
                foreach (var label in analysis.Labels.Select(x => x.Name).Distinct())
                {
                    // 3: Function
                    items.Add(new JsonObject { ["label"] = label, ["kind"] = 3, ["detail"] = "ラベル" });
                }
                // シナリオのデータの名前（12: Value）
                foreach (var entry in CatalogFor(analysis.ScenarioRoot).Entries)
                {
                    items.Add(new JsonObject
                    {
                        ["label"] = entry.Name,
                        ["kind"] = 12,
                        ["detail"] = DataReferenceChecker.KindName(entry.Kind),
                        ["documentation"] = new JsonObject { ["kind"] = "markdown", ["value"] = entry.Summary },
                    });
                }
            }
            return items;
        }
    }
}
