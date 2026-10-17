using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SRCAuthoring.Analysis;
using SRCAuthoring.Protocol;
using SRCCore.Events;
using SRCCore.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCAuthoring.Lsp
{
    /// <summary>
    /// イベントファイル（*.eve）の Language Server。
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
        private readonly ILogger log;
        private readonly Action<LogLevel?> setLogLevel;

        /// <summary>変更してから解析するまでの待ち</summary>
        public TimeSpan Debounce { get; set; } = TimeSpan.FromMilliseconds(800);

        // 解析は重い（大きなシナリオでは include 先を含めて数十万行）ので、同時には 1 つだけ行う
        private readonly SemaphoreSlim analyzing = new SemaphoreSlim(1, 1);

        /// <param name="loggerFactory">ログの出力先。ログは window/logMessage でクライアントにも送る</param>
        /// <param name="setLogLevel">初期化オプションの logLevel を反映する</param>
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
                    log.LogInformation("Language Server を開始 SRC のフォルダ: {AppPath}", analyzer.AppPath ?? "");
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
                        Schedule(path, TimeSpan.Zero);
                        return null;
                    }
                case "textDocument/didChange":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        var changes = @params["contentChanges"].AsArray();
                        documents[path] = changes[changes.Count - 1]["text"].GetValue<string>();
                        Schedule(path, Debounce);
                        return null;
                    }
                case "textDocument/didSave":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        Schedule(path, TimeSpan.Zero);
                        return null;
                    }
                case "textDocument/didClose":
                    {
                        var path = UriToPath(@params["textDocument"]["uri"].GetValue<string>());
                        documents.TryRemove(path, out _);
                        analyses.TryRemove(path, out _);
                        _ = PublishAsync(path, new List<SrcDiagnostic>());
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

        /// <summary>path を開いている内容で解析し、結果を覚えておく</summary>
        public ScenarioAnalysis Analyze(string path)
        {
            var analysis = analyzer.Analyze(path, documents.ToDictionary(x => x.Key, x => x.Value));
            analyses[path] = analysis;
            return analysis;
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
                items.Add(new JsonObject
                {
                    ["range"] = LineRange(d.Line),
                    ["severity"] = d.Severity switch { DiagnosticSeverity.Error => 1, DiagnosticSeverity.Warning => 2, _ => 3 },
                    ["source"] = "srcsharp",
                    ["message"] = d.Message,
                });
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
            return label == null ? null : Location(label.File, label.Line);
        }

        private static JsonObject Location(string file, int line)
        {
            return new JsonObject { ["uri"] = PathToUri(file), ["range"] = LineRange(line, 0, 0) };
        }

        private JsonNode Hover(JsonNode @params)
        {
            var (path, line, character) = Position(@params);
            var analysis = AnalysisFor(path);
            var word = WordAt(LineText(path, line), character);
            var label = analysis.Labels.FirstOrDefault(x => x.Name == word);
            if (label == null)
            {
                return null;
            }
            return new JsonObject
            {
                ["contents"] = new JsonObject
                {
                    ["kind"] = "markdown",
                    ["value"] = $"ラベル `{label.Name}`  \n{Path.GetFileName(label.File)}:{label.Line}",
                },
            };
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
            foreach (var name in CommandNames())
            {
                // 14: Keyword
                items.Add(new JsonObject { ["label"] = name, ["kind"] = 14 });
            }
            if (analyses.TryGetValue(path, out var analysis))
            {
                foreach (var label in analysis.Labels.Select(x => x.Name).Distinct())
                {
                    // 3: Function
                    items.Add(new JsonObject { ["label"] = label, ["kind"] = 3, ["detail"] = "ラベル" });
                }
            }
            return items;
        }
    }
}
