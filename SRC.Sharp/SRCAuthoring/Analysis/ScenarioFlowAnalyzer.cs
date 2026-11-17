using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Extensions;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SRCAuthoring.Analysis
{
    public enum FlowNodeKind
    {
        Stage,
        Intermission,
        SubStage,
        Include,
        LoadData,
        GameClear,
        Missing,
    }

    public enum FlowEdgeKind
    {
        Continue,
        NextStage,
        Exec,
        IntermissionCommand,
        IntermissionReturn,
        Load,
        GameClear,
    }

    public sealed class FlowNode
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Title { get; set; }
        public FlowNodeKind Kind { get; set; }
        public bool Exists { get; set; }
        public bool IsEntry { get; set; }
        public int FirstLine { get; set; } = 1;
        public List<string> Labels { get; } = new List<string>();
    }

    public sealed class FlowEdge
    {
        public string From { get; set; }
        public string To { get; set; }
        public FlowEdgeKind Kind { get; set; }
        public string File { get; set; }
        public string FullFile { get; set; }
        public int Line { get; set; }
        public string EnclosingLabel { get; set; }
        public string Condition { get; set; }
        public string Command { get; set; }
        public string CommandArg { get; set; }
        public string Target { get; set; }
        public string ResolvedTarget { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsMissing { get; set; }
    }

    public sealed class FlowWarning
    {
        public string File { get; set; }
        public string FullFile { get; set; }
        public int Line { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
    }

    public sealed class ScenarioFlow
    {
        public string ScenarioRoot { get; set; }
        public string ScenarioTitle { get; set; }
        public string AppPath { get; set; }
        public List<FlowNode> Nodes { get; } = new List<FlowNode>();
        public List<FlowEdge> Edges { get; } = new List<FlowEdge>();
        public List<FlowWarning> Warnings { get; } = new List<FlowWarning>();

        public string ToMermaid()
        {
            var sb = new StringBuilder();
            sb.AppendLine("flowchart TD");
            var byId = Nodes.ToDictionary(n => n.Id);
            foreach (var node in Nodes)
            {
                var label = EscapeMermaid(FormatNodeLabel(node));
                switch (node.Kind)
                {
                    case FlowNodeKind.Intermission:
                        sb.AppendLine($"  {node.Id}([\"{label}\"])");
                        break;
                    case FlowNodeKind.LoadData:
                        sb.AppendLine($"  {node.Id}[/\"{label}\"/]");
                        break;
                    case FlowNodeKind.GameClear:
                        sb.AppendLine($"  {node.Id}((\"{label}\"))");
                        break;
                    case FlowNodeKind.SubStage:
                    case FlowNodeKind.Include:
                        sb.AppendLine($"  {node.Id}[[\"{label}\"]]");
                        break;
                    default:
                        sb.AppendLine($"  {node.Id}[\"{label}\"]");
                        break;
                }
            }
            foreach (var edge in Edges)
            {
                if (string.IsNullOrEmpty(edge.From) || string.IsNullOrEmpty(edge.To))
                {
                    continue;
                }
                if (!byId.ContainsKey(edge.From) || !byId.ContainsKey(edge.To))
                {
                    continue;
                }
                var label = FormatEdgeLabel(edge);
                var arrow = edge.Kind switch
                {
                    FlowEdgeKind.IntermissionCommand => string.IsNullOrEmpty(label) ? "-.->" : $"-.->|\"{EscapeMermaid(label)}\"|",
                    FlowEdgeKind.IntermissionReturn => string.IsNullOrEmpty(label) ? "-.->" : $"-.->|\"{EscapeMermaid(label)}\"|",
                    FlowEdgeKind.Load => string.IsNullOrEmpty(label) ? "-.->" : $"-.->|\"{EscapeMermaid(label)}\"|",
                    FlowEdgeKind.Exec => string.IsNullOrEmpty(label) ? "==>" : $"==>|\"{EscapeMermaid(label)}\"|",
                    _ => string.IsNullOrEmpty(label) ? "-->" : $"-->|\"{EscapeMermaid(label)}\"|",
                };
                sb.AppendLine($"  {edge.From} {arrow} {edge.To}");
            }
            foreach (var node in Nodes.Where(n => n.Kind == FlowNodeKind.Missing))
            {
                sb.AppendLine($"  style {node.Id} stroke:#d73a49,stroke-dasharray: 5 5");
            }
            foreach (var node in Nodes.Where(n => n.Kind == FlowNodeKind.GameClear))
            {
                sb.AppendLine($"  style {node.Id} stroke:#2da44e,stroke-width:2px");
            }
            return sb.ToString();
        }

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            var title = string.IsNullOrEmpty(ScenarioTitle) ? "シナリオ" : ScenarioTitle;
            sb.AppendLine($"# 話のつながり（{title}）");
            sb.AppendLine();
            sb.AppendLine("```mermaid");
            sb.Append(ToMermaid().TrimEnd('\r', '\n'));
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine();

            var byId = Nodes.ToDictionary(n => n.Id);
            var stageEdges = Edges.Where(e => e.Kind is FlowEdgeKind.Continue or FlowEdgeKind.NextStage or FlowEdgeKind.Exec or FlowEdgeKind.GameClear).ToList();
            if (stageEdges.Count > 0)
            {
                sb.AppendLine("## 話の流れ");
                sb.AppendLine();
                sb.AppendLine("| 元 | 行 | コマンド | 条件 | 先 |");
                sb.AppendLine("|---|---|---|---|---|");
                foreach (var e in stageEdges)
                {
                    var from = byId.TryGetValue(e.From, out var fn) ? fn.Path : e.File;
                    var to = byId.TryGetValue(e.To, out var tn) ? (tn.Kind == FlowNodeKind.GameClear ? "GameClear" : tn.Path) : e.Target;
                    var cond = FormatConditionSummary(e);
                    sb.AppendLine($"| `{MdCell(from)}` | {e.Line} | `{MdCell(e.Command)}` | {MdCell(cond)} | `{MdCell(to)}` |");
                }
                sb.AppendLine();
            }

            var intermissions = Edges.Where(e => e.Kind == FlowEdgeKind.IntermissionCommand).ToList();
            if (intermissions.Count > 0)
            {
                sb.AppendLine("## インターミッションのコマンド");
                sb.AppendLine();
                sb.AppendLine("| 元 | 行 | コマンド名 | 先 |");
                sb.AppendLine("|---|---|---|---|");
                foreach (var e in intermissions)
                {
                    var from = byId.TryGetValue(e.From, out var fn) ? fn.Path : e.File;
                    var to = byId.TryGetValue(e.To, out var tn) ? tn.Path : e.Target;
                    sb.AppendLine($"| `{MdCell(from)}` | {e.Line} | {MdCell(e.CommandArg)} | `{MdCell(to)}` |");
                }
                sb.AppendLine();
            }

            var loads = Edges.Where(e => e.Kind == FlowEdgeKind.Load).ToList();
            if (loads.Count > 0)
            {
                sb.AppendLine("## 途中で読み込むデータ（Load）");
                sb.AppendLine();
                sb.AppendLine("| 元 | 行 | 作品・フォルダ | 条件 |");
                sb.AppendLine("|---|---|---|---|");
                foreach (var e in loads)
                {
                    var from = byId.TryGetValue(e.From, out var fn) ? fn.Path : e.File;
                    var cond = FormatConditionSummary(e);
                    sb.AppendLine($"| `{MdCell(from)}` | {e.Line} | `{MdCell(e.Target)}` | {MdCell(cond)} |");
                }
                sb.AppendLine();
            }

            if (Warnings.Count > 0)
            {
                sb.AppendLine("## 見つからない先（警告）");
                sb.AppendLine();
                foreach (var w in Warnings)
                {
                    sb.AppendLine($"- `{w.File}:{w.Line}`: {w.Message}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string ToJson()
        {
            var payload = new
            {
                scenarioRoot = ScenarioRoot,
                scenarioTitle = ScenarioTitle,
                appPath = AppPath,
                nodes = Nodes.Select(n => new
                {
                    id = n.Id,
                    path = n.Path,
                    fullPath = n.FullPath,
                    title = n.Title,
                    kind = n.Kind.ToString(),
                    exists = n.Exists,
                    entry = n.IsEntry,
                    line = n.FirstLine,
                    labels = n.Labels,
                }),
                edges = Edges.Select(e => new
                {
                    from = e.From,
                    to = e.To,
                    kind = e.Kind.ToString(),
                    file = e.File,
                    fullFile = e.FullFile,
                    line = e.Line,
                    label = e.EnclosingLabel,
                    condition = e.Condition,
                    command = e.Command,
                    commandArg = e.CommandArg,
                    target = e.Target,
                    resolvedTarget = e.ResolvedTarget,
                    dynamic = e.IsDynamic,
                    missing = e.IsMissing,
                }),
                warnings = Warnings.Select(w => new
                {
                    file = w.File,
                    fullFile = w.FullFile,
                    line = w.Line,
                    target = w.Target,
                    message = w.Message,
                }),
                mermaid = ToMermaid(),
            };
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            });
        }

        private static string FormatNodeLabel(FlowNode node)
        {
            return node.Kind switch
            {
                FlowNodeKind.Intermission => $"{node.Title}\n(インターミッション)",
                FlowNodeKind.LoadData => $"Load: {node.Title}",
                FlowNodeKind.GameClear => "クリア\n(GameClear)",
                FlowNodeKind.Missing => $"? {node.Title}\n(見つかりません)",
                _ => node.Path,
            };
        }

        private static string FormatEdgeLabel(FlowEdge edge)
        {
            var parts = new List<string>();
            switch (edge.Kind)
            {
                case FlowEdgeKind.IntermissionCommand:
                    parts.Add(string.IsNullOrEmpty(edge.CommandArg) ? "インターミッション" : $"メニュー: {edge.CommandArg}");
                    break;
                case FlowEdgeKind.IntermissionReturn:
                    parts.Add("戻る");
                    break;
                case FlowEdgeKind.Load:
                    parts.Add("Load");
                    break;
                case FlowEdgeKind.Exec:
                    parts.Add(string.IsNullOrEmpty(edge.CommandArg) ? "Exec" : $"Exec ({edge.CommandArg})");
                    break;
                case FlowEdgeKind.GameClear:
                    parts.Add("GameClear");
                    break;
            }
            if (!string.IsNullOrEmpty(edge.Condition))
            {
                parts.Add(edge.Condition);
            }
            else if (edge.Kind is FlowEdgeKind.Continue or FlowEdgeKind.NextStage
                && !string.IsNullOrEmpty(edge.EnclosingLabel)
                && !edge.EnclosingLabel.Equals("全滅 敵", StringComparison.Ordinal))
            {
                parts.Add(edge.EnclosingLabel);
            }
            return string.Join("\n", parts);
        }

        private static string FormatConditionSummary(FlowEdge e)
        {
            if (!string.IsNullOrEmpty(e.EnclosingLabel) && !string.IsNullOrEmpty(e.Condition))
            {
                return $"{e.EnclosingLabel} / {e.Condition}";
            }
            return !string.IsNullOrEmpty(e.Condition) ? e.Condition : (e.EnclosingLabel ?? "");
        }

        private static string EscapeMermaid(string text)
        {
            return (text ?? "")
                .Replace("\r\n", "\n")
                .Replace("\"", "#quot;")
                .Replace("\n", "<br/>");
        }

        private static string MdCell(string text)
        {
            return (text ?? "").Replace("|", "\\|").Replace("\r\n", " ").Replace("\n", " ");
        }
    }

    /// <summary>
    /// シナリオのフォルダやイベントファイルから、Continue・次ステージ・Exec・IntermissionCommand・Load・GameClear をたどり、
    /// 話の順と分岐の図（Mermaid / Markdown / JSON）を作る（7474/SRC#1652）。
    /// </summary>
    public sealed class ScenarioFlowAnalyzer
    {
        private static readonly HashSet<string> IgnoredDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".srcsharp", ".git", "node_modules", "save", "logs", "srcsharp-logs", "bin", "obj"
        };

        private static readonly char[] DynamicChars = { '$', '(', '`', '[' };

        public string AppPath { get; set; }

        public ScenarioFlow Analyze(string targetPath, IDictionary<string, string> overlays = null, string scenarioRoot = null)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentException("解析するパスを指定してください", nameof(targetPath));
            }

            var fullTarget = Path.GetFullPath(targetPath);
            var isFile = File.Exists(fullTarget);
            var resolvedRoot = !string.IsNullOrWhiteSpace(scenarioRoot)
                ? Path.GetFullPath(scenarioRoot)
                : isFile
                    ? ScenarioAnalyzer.FindRunRoot(fullTarget)
                    : fullTarget;

            var appPath = AppPath ?? ScenarioAnalyzer.FindAppPath(resolvedRoot);
            var flow = new ScenarioFlow
            {
                ScenarioRoot = resolvedRoot,
                ScenarioTitle = Path.GetFileName(resolvedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)),
                AppPath = appPath,
            };

            var eveFiles = CollectEventFiles(resolvedRoot, isFile ? fullTarget : null);
            var fileInfos = new Dictionary<string, ScannedFile>(StringComparer.OrdinalIgnoreCase);

            foreach (var eve in eveFiles)
            {
                var rel = NormalizeRelPath(resolvedRoot, eve);
                var scanned = ScanFile(appPath, resolvedRoot, eve, rel, overlays);
                fileInfos[rel] = scanned;
            }

            // IntermissionCommand / Exec / Continue の宛先になっているファイルを把握する
            var intermissionTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var execTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var stageTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var includedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var scanned in fileInfos.Values)
            {
                foreach (var inc in scanned.Includes)
                {
                    includedFiles.Add(inc);
                }
                foreach (var raw in scanned.RawEdges)
                {
                    if (raw.IsDynamic || string.IsNullOrEmpty(raw.Target))
                    {
                        continue;
                    }
                    var resolved = ResolveEventTarget(resolvedRoot, appPath, scanned.FullPath, raw.Target);
                    if (resolved == null)
                    {
                        continue;
                    }
                    var targetRel = NormalizeRelPath(resolvedRoot, resolved);
                    switch (raw.Kind)
                    {
                        case FlowEdgeKind.IntermissionCommand:
                            intermissionTargets.Add(targetRel);
                            break;
                        case FlowEdgeKind.Exec:
                            if (string.Equals(raw.CommandArg, "通常ステージ", StringComparison.Ordinal))
                            {
                                stageTargets.Add(targetRel);
                            }
                            else
                            {
                                execTargets.Add(targetRel);
                            }
                            break;
                        case FlowEdgeKind.Continue:
                        case FlowEdgeKind.NextStage:
                            stageTargets.Add(targetRel);
                            break;
                    }
                }
            }

            // ノードに含めるファイルを選別（フロー遷移に無関係な純サブルーチン Lib/*.eve や include.eve を除き、図を見やすくする）
            var activeFiles = new List<ScannedFile>();
            foreach (var scanned in fileInfos.Values.OrderBy(x => x.RelPath, StringComparer.OrdinalIgnoreCase))
            {
                var isTarget = stageTargets.Contains(scanned.RelPath)
                    || intermissionTargets.Contains(scanned.RelPath)
                    || execTargets.Contains(scanned.RelPath);
                var hasFlowOut = scanned.RawEdges.Count > 0 || scanned.NextStageAssignments.Count > 0;
                var isEntryFile = isFile && ScenarioAnalysis.SamePath(scanned.FullPath, fullTarget);
                var isIncludeOnly = (includedFiles.Contains(scanned.RelPath)
                    || string.Equals(Path.GetFileName(scanned.RelPath), "include.eve", StringComparison.OrdinalIgnoreCase)
                    || scanned.RelPath.StartsWith("Lib/", StringComparison.OrdinalIgnoreCase))
                    && !scanned.HasStageLabels;

                if (isEntryFile || isTarget || hasFlowOut || (scanned.HasStageLabels && !isIncludeOnly))
                {
                    activeFiles.Add(scanned);
                }
            }

            var nodeByRel = new Dictionary<string, FlowNode>(StringComparer.OrdinalIgnoreCase);
            var stageCounter = 0;
            var imCounter = 0;
            var subCounter = 0;
            var incCounter = 0;

            FlowNode EnsureFileNode(string relPath, string fullPath, bool exists, FlowNodeKind preferredKind)
            {
                if (nodeByRel.TryGetValue(relPath, out var existing))
                {
                    return existing;
                }
                var id = preferredKind switch
                {
                    FlowNodeKind.Intermission => $"im{imCounter++}",
                    FlowNodeKind.SubStage => $"sub{subCounter++}",
                    FlowNodeKind.Include => $"inc{incCounter++}",
                    _ => $"s{stageCounter++}",
                };
                var node = new FlowNode
                {
                    Id = id,
                    Path = relPath,
                    FullPath = fullPath,
                    Title = StripEveExtension(relPath),
                    Kind = preferredKind,
                    Exists = exists,
                    FirstLine = 1,
                };
                if (fileInfos.TryGetValue(relPath, out var sf))
                {
                    node.Labels.AddRange(sf.Labels);
                    if (sf.Labels.Count > 0)
                    {
                        node.FirstLine = sf.FirstLabelLine;
                    }
                }
                nodeByRel[relPath] = node;
                flow.Nodes.Add(node);
                return node;
            }

            foreach (var scanned in activeFiles)
            {
                var kind = FlowNodeKind.Stage;
                if (intermissionTargets.Contains(scanned.RelPath) && !stageTargets.Contains(scanned.RelPath))
                {
                    kind = FlowNodeKind.Intermission;
                }
                else if (execTargets.Contains(scanned.RelPath) && !stageTargets.Contains(scanned.RelPath))
                {
                    kind = FlowNodeKind.SubStage;
                }
                else if (!scanned.HasStageLabels
                    && (includedFiles.Contains(scanned.RelPath) || string.Equals(Path.GetFileName(scanned.RelPath), "include.eve", StringComparison.OrdinalIgnoreCase)))
                {
                    kind = FlowNodeKind.Include;
                }
                EnsureFileNode(scanned.RelPath, scanned.FullPath, true, kind);
            }

            var loadNodes = new Dictionary<string, FlowNode>(StringComparer.OrdinalIgnoreCase);
            var missingNodes = new Dictionary<string, FlowNode>(StringComparer.OrdinalIgnoreCase);
            var dynamicCounter = 0;
            FlowNode gameClearNode = null;

            FlowNode EnsureGameClearNode()
            {
                if (gameClearNode == null)
                {
                    gameClearNode = new FlowNode
                    {
                        Id = "clear",
                        Path = "(GameClear)",
                        Title = "クリア",
                        Kind = FlowNodeKind.GameClear,
                        Exists = true,
                    };
                    flow.Nodes.Add(gameClearNode);
                }
                return gameClearNode;
            }

            FlowNode EnsureLoadNode(string title)
            {
                if (loadNodes.TryGetValue(title, out var existing))
                {
                    return existing;
                }
                var dataDir = FindDataFolder(resolvedRoot, appPath, title);
                var includeEve = dataDir != null ? Path.Combine(dataDir, "include.eve") : null;
                var fullPath = includeEve != null && File.Exists(includeEve) ? includeEve : dataDir;
                var node = new FlowNode
                {
                    Id = $"load{loadNodes.Count}",
                    Path = $"Data/{title}",
                    FullPath = fullPath,
                    Title = title,
                    Kind = FlowNodeKind.LoadData,
                    Exists = dataDir != null,
                };
                loadNodes[title] = node;
                flow.Nodes.Add(node);
                return node;
            }

            FlowNode EnsureMissingNode(string rawTarget, string normRel)
            {
                var key = normRel ?? rawTarget;
                if (missingNodes.TryGetValue(key, out var existing))
                {
                    return existing;
                }
                var node = new FlowNode
                {
                    Id = $"miss{missingNodes.Count}",
                    Path = key,
                    Title = key,
                    Kind = FlowNodeKind.Missing,
                    Exists = false,
                };
                missingNodes[key] = node;
                flow.Nodes.Add(node);
                return node;
            }

            foreach (var scanned in activeFiles)
            {
                var fromNode = nodeByRel[scanned.RelPath];
                foreach (var raw in scanned.RawEdges)
                {
                    if (raw.Kind == FlowEdgeKind.GameClear)
                    {
                        var toNode = EnsureGameClearNode();
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = toNode.Id,
                            Kind = FlowEdgeKind.GameClear,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = "GameClear",
                        });
                        continue;
                    }

                    if (raw.Kind == FlowEdgeKind.Load)
                    {
                        if (raw.IsDynamic)
                        {
                            var dynNode = new FlowNode
                            {
                                Id = $"dyn{dynamicCounter++}",
                                Path = raw.Target,
                                Title = $"Load {raw.Target}",
                                Kind = FlowNodeKind.LoadData,
                                Exists = true,
                            };
                            flow.Nodes.Add(dynNode);
                            flow.Edges.Add(new FlowEdge
                            {
                                From = fromNode.Id,
                                To = dynNode.Id,
                                Kind = FlowEdgeKind.Load,
                                File = scanned.RelPath,
                                FullFile = scanned.FullPath,
                                Line = raw.Line,
                                EnclosingLabel = raw.EnclosingLabel,
                                Condition = raw.Condition,
                                Command = "Load",
                                Target = raw.Target,
                                IsDynamic = true,
                            });
                            continue;
                        }
                        var loadNode = EnsureLoadNode(raw.Target);
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = loadNode.Id,
                            Kind = FlowEdgeKind.Load,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = "Load",
                            Target = raw.Target,
                            ResolvedTarget = loadNode.FullPath != null ? NormalizeRelPath(resolvedRoot, loadNode.FullPath) : loadNode.Path,
                            IsMissing = !loadNode.Exists,
                        });
                        if (!loadNode.Exists)
                        {
                            flow.Warnings.Add(new FlowWarning
                            {
                                File = scanned.RelPath,
                                FullFile = scanned.FullPath,
                                Line = raw.Line,
                                Target = raw.Target,
                                Message = $"Load 先のフォルダ「{raw.Target}」が見つかりません",
                            });
                        }
                        continue;
                    }

                    if (raw.Kind == FlowEdgeKind.IntermissionReturn)
                    {
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = fromNode.Id,
                            Kind = FlowEdgeKind.IntermissionReturn,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = "Continue",
                        });
                        continue;
                    }

                    // Continue (with arg), NextStage, Exec, IntermissionCommand
                    if (raw.IsDynamic)
                    {
                        var dynNode = new FlowNode
                        {
                            Id = $"dyn{dynamicCounter++}",
                            Path = raw.Target ?? "(動的)",
                            Title = raw.Target ?? "(動的)",
                            Kind = FlowNodeKind.SubStage,
                            Exists = true,
                        };
                        flow.Nodes.Add(dynNode);
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = dynNode.Id,
                            Kind = raw.Kind,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = raw.Command,
                            CommandArg = raw.CommandArg,
                            Target = raw.Target,
                            IsDynamic = true,
                        });
                        continue;
                    }

                    var resolvedTarget = ResolveEventTarget(resolvedRoot, appPath, scanned.FullPath, raw.Target);
                    if (resolvedTarget != null)
                    {
                        var targetRel = NormalizeRelPath(resolvedRoot, resolvedTarget);
                        var targetKind = raw.Kind == FlowEdgeKind.IntermissionCommand
                            ? FlowNodeKind.Intermission
                            : (raw.Kind == FlowEdgeKind.Exec && !string.Equals(raw.CommandArg, "通常ステージ", StringComparison.Ordinal)
                                ? FlowNodeKind.SubStage
                                : FlowNodeKind.Stage);
                        var toNode = EnsureFileNode(targetRel, resolvedTarget, true, targetKind);
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = toNode.Id,
                            Kind = raw.Kind,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = raw.Command,
                            CommandArg = raw.CommandArg,
                            Target = raw.Target,
                            ResolvedTarget = targetRel,
                        });
                    }
                    else
                    {
                        var normTarget = (raw.Target ?? "").Replace('\\', '/');
                        var missNode = EnsureMissingNode(raw.Target, normTarget);
                        flow.Edges.Add(new FlowEdge
                        {
                            From = fromNode.Id,
                            To = missNode.Id,
                            Kind = raw.Kind,
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            EnclosingLabel = raw.EnclosingLabel,
                            Condition = raw.Condition,
                            Command = raw.Command,
                            CommandArg = raw.CommandArg,
                            Target = raw.Target,
                            IsMissing = true,
                        });
                        flow.Warnings.Add(new FlowWarning
                        {
                            File = scanned.RelPath,
                            FullFile = scanned.FullPath,
                            Line = raw.Line,
                            Target = raw.Target,
                            Message = $"{raw.Command} 先のイベントファイル「{raw.Target}」が見つかりません",
                        });
                    }
                }
            }

            // 入口（他のステージからの Continue/Exec に入らない Stage ノード）を判定
            var incomingStageTargets = new HashSet<string>(
                flow.Edges
                    .Where(e => e.Kind is FlowEdgeKind.Continue or FlowEdgeKind.NextStage or FlowEdgeKind.Exec)
                    .Select(e => e.To),
                StringComparer.Ordinal);
            foreach (var node in flow.Nodes)
            {
                if (node.Kind == FlowNodeKind.Stage && !incomingStageTargets.Contains(node.Id))
                {
                    node.IsEntry = true;
                }
            }

            return flow;
        }

        private static List<string> CollectEventFiles(string root, string explicitTarget)
        {
            var list = new List<string>();
            if (Directory.Exists(root))
            {
                WalkDirectory(root, root, list);
            }
            if (!string.IsNullOrEmpty(explicitTarget) && File.Exists(explicitTarget)
                && !list.Any(f => ScenarioAnalysis.SamePath(f, explicitTarget)))
            {
                list.Add(explicitTarget);
            }
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list;
        }

        private static void WalkDirectory(string root, string dir, List<string> result)
        {
            foreach (var file in Directory.GetFiles(dir, "*.eve"))
            {
                result.Add(Path.GetFullPath(file));
            }
            foreach (var sub in Directory.GetDirectories(dir))
            {
                var name = Path.GetFileName(sub);
                if (IgnoredDirectories.Contains(name))
                {
                    continue;
                }
                // ScenarioRoot 直下の Data フォルダ内の include.eve 等は Load で参照されるデータなので通常のステージ列挙から外す
                if (dir == root && name.Equals("Data", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                WalkDirectory(root, sub, result);
            }
        }

        private static ScannedFile ScanFile(string appPath, string root, string fullPath, string relPath, IDictionary<string, string> overlays)
        {
            var scanned = new ScannedFile { FullPath = fullPath, RelPath = relPath };

            var src = new SRCCore.SRC { GUI = new SRCCore.TestLib.MockGUI() };
            src.SystemConfig.AppPath = appPath ?? AppContext.BaseDirectory;
            src.SystemConfig.SRCCompatibilityMode = SRCCore.SRCCompatibilityMode.Read;
            src.ScenarioPath = root;
            src.FileSystem = new OverlayFileSystem(new LocalFileSystem(), overlays);
            src.Event.SkipExternalSourceLoad = true;
            try
            {
                src.Event.InitEventData();
                src.Event.LoadEventData(fullPath, "");
            }
            catch
            {
                // エラーが出ても読めた範囲のコマンドをスキャンする
            }

            var lines = src.Event.EventData ?? new List<EventDataLine>();
            var fileLabels = new List<EveLabel>();
            foreach (var line in lines)
            {
                if (!ScenarioAnalysis.SamePath(line.File, fullPath))
                {
                    continue;
                }
                var data = line.Data ?? "";
                if (data.Length > 1 && data.EndsWith(":"))
                {
                    var name = data.Substring(0, data.Length - 1);
                    fileLabels.Add(new EveLabel { Name = name, File = fullPath, Line = line.LineNum });
                    scanned.Labels.Add(name);
                    if (name is "プロローグ" or "スタート" or "エピローグ")
                    {
                        scanned.HasStageLabels = true;
                    }
                }
            }
            fileLabels.Sort((a, b) => a.Line.CompareTo(b.Line));
            if (fileLabels.Count > 0)
            {
                scanned.FirstLabelLine = fileLabels[0].Line;
            }

            CollectFileIncludes(src, fullPath, root, overlays, scanned.Includes);

            var assignedVars = new HashSet<string>(StringComparer.Ordinal);
            var cmds = src.Event.EventCmd ?? new List<CmdData>();
            foreach (var c in cmds)
            {
                if (c.EventData == null || !ScenarioAnalysis.SamePath(c.EventData.File, fullPath))
                {
                    continue;
                }
                if ((c.Name == CmdType.SetCmd || c.Name == CmdType.LocalCmd || c.Name == CmdType.GlobalCmd) && c.ArgNum >= 2)
                {
                    var varName = c.GetArg(2);
                    if (!string.IsNullOrEmpty(varName))
                    {
                        assignedVars.Add(varName);
                    }
                }
            }

            var condStack = new Stack<BlockFrame>();
            string currentLabel = null;
            var labelIdx = 0;

            foreach (var cmd in cmds)
            {
                if (cmd.EventData == null || cmd.EventData.IsSystemData || !ScenarioAnalysis.SamePath(cmd.EventData.File, fullPath))
                {
                    continue;
                }
                var lineNum = cmd.EventData.LineNum;
                while (labelIdx < fileLabels.Count && fileLabels[labelIdx].Line <= lineNum)
                {
                    var newLabel = fileLabels[labelIdx].Name;
                    if (!string.Equals(currentLabel, newLabel, StringComparison.Ordinal))
                    {
                        currentLabel = newLabel;
                        condStack.Clear();
                    }
                    labelIdx++;
                }

                var rawLine = cmd.EventData.ID >= 0 && cmd.EventData.ID < lines.Count ? (lines[cmd.EventData.ID].Data ?? "") : "";

                switch (cmd.Name)
                {
                    case CmdType.IfCmd:
                        if ((cmd as AIfCmd)?.IfCmdType == IfCmdType.Then)
                        {
                            condStack.Push(new BlockFrame { IsSwitch = false, Condition = ExtractIfCondition(rawLine) ?? (cmd as AIfCmd)?.Expr ?? "If" });
                        }
                        continue;

                    case CmdType.ElseIfCmd:
                        if (condStack.Count > 0 && !condStack.Peek().IsSwitch)
                        {
                            condStack.Peek().Condition = "ElseIf " + (ExtractIfCondition(rawLine) ?? (cmd as AIfCmd)?.Expr ?? "");
                        }
                        continue;

                    case CmdType.ElseCmd:
                        if (condStack.Count > 0 && !condStack.Peek().IsSwitch)
                        {
                            condStack.Peek().Condition = "Else";
                        }
                        continue;

                    case CmdType.EndIfCmd:
                        if (condStack.Count > 0 && !condStack.Peek().IsSwitch)
                        {
                            condStack.Pop();
                        }
                        continue;

                    case CmdType.SwitchCmd:
                        condStack.Push(new BlockFrame
                        {
                            IsSwitch = true,
                            SwitchExpr = cmd.ArgNum >= 2 ? RawArgOrValue(cmd, 2) : "Switch",
                            Condition = null,
                        });
                        continue;

                    case CmdType.CaseCmd:
                        if (condStack.Count > 0 && condStack.Peek().IsSwitch)
                        {
                            var vals = new List<string>();
                            for (var j = 2; j <= cmd.ArgNum; j++)
                            {
                                vals.Add(RawArgOrValue(cmd, j));
                            }
                            var frame = condStack.Peek();
                            frame.Condition = $"{frame.SwitchExpr} = {string.Join(", ", vals)}";
                        }
                        continue;

                    case CmdType.CaseElseCmd:
                        if (condStack.Count > 0 && condStack.Peek().IsSwitch)
                        {
                            var frame = condStack.Peek();
                            frame.Condition = $"{frame.SwitchExpr}: その他";
                        }
                        continue;

                    case CmdType.EndSwCmd:
                        if (condStack.Count > 0 && condStack.Peek().IsSwitch)
                        {
                            condStack.Pop();
                        }
                        continue;
                }

                var cond = BuildConditionString(condStack);

                if (cmd.Name == CmdType.SetCmd && cmd.ArgNum >= 3)
                {
                    var varName = cmd.GetArg(2);
                    if (varName == "次ステージ")
                    {
                        var val = cmd.GetArg(3);
                        var raw = cmd.GetArgRaw(3).RawArg ?? val;
                        var isDyn = IsDynamicExpression(raw, assignedVars);
                        scanned.NextStageAssignments.Add(new RawEdge
                        {
                            Kind = FlowEdgeKind.NextStage,
                            Line = lineNum,
                            EnclosingLabel = currentLabel,
                            Condition = cond,
                            Command = "次ステージ",
                            Target = isDyn ? raw : val,
                            IsDynamic = isDyn,
                        });
                    }
                    else if (varName != null && varName.StartsWith("IntermissionCommand(", StringComparison.Ordinal) && varName.EndsWith(")", StringComparison.Ordinal))
                    {
                        var cmdTitle = varName.Substring("IntermissionCommand(".Length, varName.Length - "IntermissionCommand()".Length);
                        var val = cmd.GetArg(3);
                        var raw = cmd.GetArgRaw(3).RawArg ?? val;
                        if (val != "削除")
                        {
                            var isDyn = IsDynamicExpression(raw, assignedVars);
                            scanned.RawEdges.Add(new RawEdge
                            {
                                Kind = FlowEdgeKind.IntermissionCommand,
                                Line = lineNum,
                                EnclosingLabel = currentLabel,
                                Condition = cond,
                                Command = "IntermissionCommand",
                                CommandArg = cmdTitle,
                                Target = isDyn ? raw : val,
                                IsDynamic = isDyn,
                            });
                        }
                    }
                    continue;
                }

                switch (cmd.Name)
                {
                    case CmdType.ContinueCmd:
                        if (cmd.ArgNum >= 2)
                        {
                            var val = cmd.GetArg(2);
                            var raw = cmd.GetArgRaw(2).RawArg ?? val;
                            var isDyn = IsDynamicExpression(raw, assignedVars);
                            scanned.RawEdges.Add(new RawEdge
                            {
                                Kind = FlowEdgeKind.Continue,
                                Line = lineNum,
                                EnclosingLabel = currentLabel,
                                Condition = cond,
                                Command = "Continue",
                                Target = isDyn ? raw : val,
                                IsDynamic = isDyn,
                            });
                        }
                        else
                        {
                            // 引数なしの Continue（次ステージ が同じファイルに設定されていればそこへの遷移、無ければ IntermissionReturn）
                            if (scanned.NextStageAssignments.Count > 0)
                            {
                                foreach (var next in scanned.NextStageAssignments)
                                {
                                    scanned.RawEdges.Add(new RawEdge
                                    {
                                        Kind = FlowEdgeKind.NextStage,
                                        Line = next.Line,
                                        EnclosingLabel = next.EnclosingLabel ?? currentLabel,
                                        Condition = CombineConditions(cond, next.Condition),
                                        Command = "Continue (次ステージ)",
                                        Target = next.Target,
                                        IsDynamic = next.IsDynamic,
                                    });
                                }
                            }
                            else
                            {
                                scanned.RawEdges.Add(new RawEdge
                                {
                                    Kind = FlowEdgeKind.IntermissionReturn,
                                    Line = lineNum,
                                    EnclosingLabel = currentLabel,
                                    Condition = cond,
                                    Command = "Continue",
                                });
                            }
                        }
                        break;

                    case CmdType.ExecCmd:
                        if (cmd.ArgNum >= 2)
                        {
                            var val = cmd.GetArg(2);
                            var raw = cmd.GetArgRaw(2).RawArg ?? val;
                            var opt = cmd.ArgNum >= 3 ? cmd.GetArg(3) : null;
                            var isDyn = IsDynamicExpression(raw, assignedVars);
                            scanned.RawEdges.Add(new RawEdge
                            {
                                Kind = FlowEdgeKind.Exec,
                                Line = lineNum,
                                EnclosingLabel = currentLabel,
                                Condition = cond,
                                Command = "Exec",
                                CommandArg = opt,
                                Target = isDyn ? raw : val,
                                IsDynamic = isDyn,
                            });
                        }
                        break;

                    case CmdType.IntermissionCommandCmd:
                        if (cmd.ArgNum >= 3)
                        {
                            var menuName = cmd.GetArg(2);
                            var val = cmd.GetArg(3);
                            var raw = cmd.GetArgRaw(3).RawArg ?? val;
                            if (val != "削除")
                            {
                                var isDyn = IsDynamicExpression(raw, assignedVars);
                                scanned.RawEdges.Add(new RawEdge
                                {
                                    Kind = FlowEdgeKind.IntermissionCommand,
                                    Line = lineNum,
                                    EnclosingLabel = currentLabel,
                                    Condition = cond,
                                    Command = "IntermissionCommand",
                                    CommandArg = menuName,
                                    Target = isDyn ? raw : val,
                                    IsDynamic = isDyn,
                                });
                            }
                        }
                        break;

                    case CmdType.LoadCmd:
                        for (var i = 2; i <= cmd.ArgNum; i++)
                        {
                            var val = cmd.GetArg(i);
                            var raw = cmd.GetArgRaw(i).RawArg ?? val;
                            var isDyn = IsDynamicExpression(raw, assignedVars);
                            scanned.RawEdges.Add(new RawEdge
                            {
                                Kind = FlowEdgeKind.Load,
                                Line = lineNum,
                                EnclosingLabel = currentLabel,
                                Condition = cond,
                                Command = "Load",
                                Target = isDyn ? raw : val,
                                IsDynamic = isDyn,
                            });
                        }
                        break;

                    case CmdType.GameClearCmd:
                        scanned.RawEdges.Add(new RawEdge
                        {
                            Kind = FlowEdgeKind.GameClear,
                            Line = lineNum,
                            EnclosingLabel = currentLabel,
                            Condition = cond,
                            Command = "GameClear",
                        });
                        break;
                }
            }

            return scanned;
        }

        private static void CollectFileIncludes(SRCCore.SRC src, string file, string root, IDictionary<string, string> overlays, List<string> includes)
        {
            string text = null;
            if (overlays != null)
            {
                text = overlays.FirstOrDefault(x => ScenarioAnalysis.SamePath(x.Key, file)).Value;
            }
            if (text == null)
            {
                if (!File.Exists(file))
                {
                    return;
                }
                try
                {
                    using var stream = src.FileSystem.OpenText(SRCCore.SRCCompatibilityMode.Read, file);
                    using var reader = new StreamReader(stream, Encoding.UTF8, true);
                    text = reader.ReadToEnd();
                }
                catch
                {
                    return;
                }
            }
            var physical = text.Replace("\r\n", "\n").Split('\n');
            foreach (var raw in physical)
            {
                var line = raw.Trim();
                if (line.Length > 2 && line.StartsWith("<") && line.EndsWith(">") && line.IndexOf('>') == line.Length - 1)
                {
                    var target = line.Substring(1, line.Length - 2).Trim();
                    foreach (var dir in new[] { src.ScenarioPath, src.ExtDataPath, src.ExtDataPath2, src.AppPath })
                    {
                        if (string.IsNullOrEmpty(dir))
                        {
                            continue;
                        }
                        var path = src.FileSystem.PathCombine(dir, target);
                        if (src.FileSystem.FileExists(path))
                        {
                            includes.Add(NormalizeRelPath(root, OverlayFileSystem.Normalize(path)));
                            break;
                        }
                    }
                }
            }
        }

        private static string RawArgOrValue(CmdData cmd, int index)
        {
            return cmd.GetArgRaw(index).RawArg ?? cmd.GetArg(index);
        }

        private static bool IsDynamicExpression(string raw, HashSet<string> assignedVars)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return true;
            }
            if (raw.IndexOfAny(DynamicChars) >= 0)
            {
                return true;
            }
            return assignedVars.Contains(raw);
        }

        private static string ExtractIfCondition(string rawLine)
        {
            var trimmed = (rawLine ?? "").Trim();
            if (trimmed.StartsWith("ElseIf", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(6).TrimStart();
            }
            else if (trimmed.StartsWith("If", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(2).TrimStart();
            }
            else
            {
                return null;
            }
            if (trimmed.EndsWith("Then", StringComparison.OrdinalIgnoreCase) && trimmed.Length > 4
                && char.IsWhiteSpace(trimmed[trimmed.Length - 5]))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 4).TrimEnd();
            }
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        private static string BuildConditionString(Stack<BlockFrame> stack)
        {
            if (stack.Count == 0)
            {
                return null;
            }
            var list = stack.Reverse().Select(f => f.Condition).Where(c => !string.IsNullOrEmpty(c)).ToList();
            return list.Count == 0 ? null : string.Join(" / ", list);
        }

        private static string CombineConditions(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b;
            if (string.IsNullOrEmpty(b)) return a;
            if (string.Equals(a, b, StringComparison.Ordinal)) return a;
            return a + " / " + b;
        }

        private static string ResolveEventTarget(string scenarioRoot, string appPath, string currentFile, string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return null;
            }
            var norm = target.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
            var candidates = new List<string>
            {
                Path.Combine(scenarioRoot, norm),
            };
            var dir = Path.GetDirectoryName(currentFile);
            if (!string.IsNullOrEmpty(dir))
            {
                candidates.Add(Path.Combine(dir, norm));
            }
            if (!string.IsNullOrEmpty(appPath))
            {
                candidates.Add(Path.Combine(appPath, norm));
            }
            foreach (var c in candidates)
            {
                if (File.Exists(c))
                {
                    return Path.GetFullPath(c);
                }
            }
            return null;
        }

        private static string FindDataFolder(string scenarioRoot, string appPath, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }
            foreach (var baseDir in new[] { scenarioRoot, appPath })
            {
                if (string.IsNullOrEmpty(baseDir))
                {
                    continue;
                }
                var dataDir = Path.Combine(baseDir, "Data");
                if (!Directory.Exists(dataDir))
                {
                    continue;
                }
                var direct = Path.Combine(dataDir, title);
                if (Directory.Exists(direct))
                {
                    return direct;
                }
                foreach (var sub in Directory.GetDirectories(dataDir))
                {
                    if (string.Equals(Path.GetFileName(sub), title, StringComparison.OrdinalIgnoreCase))
                    {
                        return sub;
                    }
                }
            }
            return null;
        }

        private static string NormalizeRelPath(string root, string fullPath)
        {
            var rel = Path.GetRelativePath(root, fullPath).Replace('\\', '/');
            return rel;
        }

        private static string StripEveExtension(string path)
        {
            return path.EndsWith(".eve", StringComparison.OrdinalIgnoreCase)
                ? path.Substring(0, path.Length - 4)
                : path;
        }

        private sealed class BlockFrame
        {
            public bool IsSwitch { get; set; }
            public string SwitchExpr { get; set; }
            public string Condition { get; set; }
        }

        private sealed class RawEdge
        {
            public FlowEdgeKind Kind { get; set; }
            public int Line { get; set; }
            public string EnclosingLabel { get; set; }
            public string Condition { get; set; }
            public string Command { get; set; }
            public string CommandArg { get; set; }
            public string Target { get; set; }
            public bool IsDynamic { get; set; }
        }

        private sealed class ScannedFile
        {
            public string FullPath { get; set; }
            public string RelPath { get; set; }
            public bool HasStageLabels { get; set; }
            public int FirstLabelLine { get; set; } = 1;
            public List<string> Labels { get; } = new List<string>();
            public List<string> Includes { get; } = new List<string>();
            public List<RawEdge> NextStageAssignments { get; } = new List<RawEdge>();
            public List<RawEdge> RawEdges { get; } = new List<RawEdge>();
        }
    }
}
