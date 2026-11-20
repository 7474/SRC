using SRCAuthoring.Rules;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
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
    public enum EventNodeKind
    {
        Entry,
        Trigger,
        Command,
        Subroutine,
        RequireFile,
        System,
        Dynamic,
        Missing,
    }

    public enum EventEdgeKind
    {
        Call,
        Goto,
        Require,
    }

    public sealed class EventFlowNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EventNodeKind Kind { get; set; }
        public string TriggerType { get; set; }
        public string File { get; set; }
        public string FullFile { get; set; }
        public int Line { get; set; }
        public bool Exists { get; set; } = true;
        public bool Included { get; set; }
        public bool Required { get; set; }
        public int IncomingCount { get; set; }
        public int OutgoingCount { get; set; }
    }

    public sealed class EventFlowEdge
    {
        public string From { get; set; }
        public string To { get; set; }
        public EventEdgeKind Kind { get; set; }
        public string File { get; set; }
        public string FullFile { get; set; }
        public int Line { get; set; }
        public string EnclosingLabel { get; set; }
        public string Condition { get; set; }
        public string Command { get; set; }
        public string Target { get; set; }
        public string Args { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsMissing { get; set; }
    }

    public sealed class EventFlowWarning
    {
        public string File { get; set; }
        public string FullFile { get; set; }
        public int Line { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
    }

    public sealed class EventFlowGraph
    {
        public string File { get; set; }
        public string FullFile { get; set; }
        public string ScenarioRoot { get; set; }
        public string AppPath { get; set; }
        public List<EventFlowNode> Nodes { get; } = new List<EventFlowNode>();
        public List<EventFlowEdge> Edges { get; } = new List<EventFlowEdge>();
        public List<EventFlowWarning> Warnings { get; } = new List<EventFlowWarning>();

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
                    case EventNodeKind.Entry:
                        sb.AppendLine($"  {node.Id}([\"{label}\"])");
                        break;
                    case EventNodeKind.Trigger:
                    case EventNodeKind.Command:
                        sb.AppendLine($"  {node.Id}{{{{\"{label}\"}}}}");
                        break;
                    case EventNodeKind.RequireFile:
                        sb.AppendLine($"  {node.Id}[[\"{label}\"]]");
                        break;
                    case EventNodeKind.System:
                        sb.AppendLine($"  {node.Id}([\"{label}\"])");
                        break;
                    default:
                        sb.AppendLine($"  {node.Id}[\"{label}\"]");
                        break;
                }
            }
            foreach (var edge in Edges)
            {
                if (!byId.ContainsKey(edge.From) || !byId.ContainsKey(edge.To))
                {
                    continue;
                }
                var label = FormatEdgeLabel(edge);
                var arrow = edge.Kind switch
                {
                    EventEdgeKind.Require => string.IsNullOrEmpty(label) ? "-.->" : $"-.->|\"{EscapeMermaid(label)}\"|",
                    EventEdgeKind.Goto => string.IsNullOrEmpty(label) ? "==>" : $"==>|\"{EscapeMermaid(label)}\"|",
                    _ => string.IsNullOrEmpty(label) ? "-->" : $"-->|\"{EscapeMermaid(label)}\"|",
                };
                sb.AppendLine($"  {edge.From} {arrow} {edge.To}");
            }
            foreach (var node in Nodes.Where(n => n.Kind == EventNodeKind.Entry))
            {
                sb.AppendLine($"  style {node.Id} stroke:#2da44e,stroke-width:2px");
            }
            foreach (var node in Nodes.Where(n => n.Kind == EventNodeKind.Trigger))
            {
                sb.AppendLine($"  style {node.Id} stroke:#d29922,stroke-width:2px");
            }
            foreach (var node in Nodes.Where(n => n.Kind == EventNodeKind.Command))
            {
                sb.AppendLine($"  style {node.Id} stroke:#8957e5,stroke-width:2px");
            }
            foreach (var node in Nodes.Where(n => n.Kind == EventNodeKind.Missing))
            {
                sb.AppendLine($"  style {node.Id} stroke:#d73a49,stroke-dasharray: 5 5");
            }
            return sb.ToString();
        }

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            var title = string.IsNullOrEmpty(File) ? "イベント" : File;
            sb.AppendLine($"# イベントの流れ（{title}）");
            sb.AppendLine();
            sb.AppendLine("```mermaid");
            sb.Append(ToMermaid().TrimEnd('\r', '\n'));
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine();

            var triggers = Nodes
                .Where(n => n.Kind is EventNodeKind.Entry or EventNodeKind.Trigger or EventNodeKind.Command)
                .ToList();
            if (triggers.Count > 0)
            {
                sb.AppendLine("## 発火するイベント・入口");
                sb.AppendLine();
                sb.AppendLine("| 種類 | ラベル | ファイル | 行 |");
                sb.AppendLine("|---|---|---|---|");
                foreach (var n in triggers)
                {
                    sb.AppendLine($"| {KindTitle(n)} | `{MdCell(n.Name)}` | `{MdCell(n.File)}` | {n.Line} |");
                }
                sb.AppendLine();
            }

            if (Edges.Count > 0)
            {
                var byId = Nodes.ToDictionary(n => n.Id);
                sb.AppendLine("## Call・Goto・Require");
                sb.AppendLine();
                sb.AppendLine("| 元のラベル | 行 | コマンド | 条件 | 先 | 先の場所 |");
                sb.AppendLine("|---|---|---|---|---|---|");
                foreach (var e in Edges)
                {
                    var from = byId.TryGetValue(e.From, out var fn) ? fn.Name : e.EnclosingLabel;
                    var toNode = byId.TryGetValue(e.To, out var tn) ? tn : null;
                    var toName = toNode?.Name ?? e.Target;
                    var toLoc = toNode != null && toNode.Exists && toNode.Line > 0
                        ? $"{toNode.File}:{toNode.Line}"
                        : toNode?.File ?? "";
                    sb.AppendLine($"| `{MdCell(from)}` | {e.Line} | `{MdCell(e.Command)}` | {MdCell(e.Condition)} | `{MdCell(toName)}` | `{MdCell(toLoc)}` |");
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
                file = File,
                fullFile = FullFile,
                scenarioRoot = ScenarioRoot,
                appPath = AppPath,
                nodes = Nodes.Select(n => new
                {
                    id = n.Id,
                    name = n.Name,
                    kind = n.Kind.ToString(),
                    triggerType = n.TriggerType,
                    file = n.File,
                    fullFile = n.FullFile,
                    line = n.Line,
                    exists = n.Exists,
                    included = n.Included,
                    required = n.Required,
                    incoming = n.IncomingCount,
                    outgoing = n.OutgoingCount,
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
                    target = e.Target,
                    args = e.Args,
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

        private static string KindTitle(EventFlowNode node)
        {
            return node.Kind switch
            {
                EventNodeKind.Entry => "入口",
                EventNodeKind.Trigger => $"発火（{node.TriggerType}）",
                EventNodeKind.Command => $"コマンド（{node.TriggerType}）",
                EventNodeKind.RequireFile => "Require",
                EventNodeKind.System => "共通サブルーチン",
                EventNodeKind.Missing => "見つかりません",
                _ => "サブルーチン",
            };
        }

        private static string FormatNodeLabel(EventFlowNode node)
        {
            return node.Kind switch
            {
                EventNodeKind.RequireFile => $"Require: {node.Name}",
                EventNodeKind.Missing => $"? {node.Name}",
                EventNodeKind.System => $"{node.Name}\n(共通)",
                _ => node.Name,
            };
        }

        private static string FormatEdgeLabel(EventFlowEdge edge)
        {
            var parts = new List<string>();
            if (edge.Kind != EventEdgeKind.Call)
            {
                parts.Add(edge.Kind.ToString());
            }
            if (!string.IsNullOrEmpty(edge.Args))
            {
                parts.Add(edge.Args);
            }
            if (!string.IsNullOrEmpty(edge.Condition))
            {
                parts.Add(edge.Condition);
            }
            return string.Join("\n", parts);
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
    /// 1 つのイベントファイル（と include・Require 先）について、発火イベント・ラベル・Call・Goto・Require のつながりを
    /// 解析して図（Mermaid / Markdown / JSON）を作る（7474/SRC#1655）。
    /// LSP の解析結果（<see cref="ScenarioAnalysis"/>）をそのまま使っても、ファイルから直接解析しても動く。
    /// </summary>
    public sealed class EventFlowAnalyzer
    {
        private static readonly HashSet<string> EntryLabels = new HashSet<string>(StringComparer.Ordinal)
        {
            "プロローグ", "スタート", "再開", "エピローグ", "勝利条件",
        };

        private static readonly HashSet<string> TriggerKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "ターン", "損傷率", "破壊", "マップ攻撃破壊", "全滅",
            "攻撃", "攻撃後", "会話", "接触", "進入", "脱出",
            "収納", "使用", "使用後", "変形", "合体", "分離",
            "行動終了", "レベルアップ", "特殊効果",
        };

        private static readonly HashSet<string> CommandKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "マップコマンド", "ユニットコマンド",
        };

        private static readonly char[] DynamicChars = { '$', '(', '"', '`', '[' };

        public string AppPath { get; set; }

        public EventFlowGraph Analyze(string file, IDictionary<string, string> overlays = null, string scenarioRoot = null)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException("解析するイベントファイルを指定してください", nameof(file));
            }
            var analyzer = new ScenarioAnalyzer { AppPath = AppPath };
            var analysis = analyzer.Analyze(file, overlays, scenarioRoot);
            return Analyze(analysis, overlays, AppPath);
        }

        public EventFlowGraph Analyze(ScenarioAnalysis analysis, IDictionary<string, string> overlays = null, string appPath = null)
        {
            if (analysis == null)
            {
                throw new ArgumentNullException(nameof(analysis));
            }
            var root = analysis.ScenarioRoot;
            var mainFile = OverlayFileSystem.Normalize(analysis.MainFile);
            var resolvedAppPath = appPath ?? AppPath ?? ScenarioAnalyzer.FindAppPath(root);

            var graph = new EventFlowGraph
            {
                File = NormalizeRelPath(root, mainFile),
                FullFile = mainFile,
                ScenarioRoot = root,
                AppPath = resolvedAppPath,
            };

            var lines = analysis.Lines ?? Array.Empty<EventDataLine>();
            var cmds = analysis.Commands ?? Array.Empty<CmdData>();

            var assignedVars = new HashSet<string>(StringComparer.Ordinal);
            foreach (var c in cmds)
            {
                if (c.EventData == null || c.EventData.IsSystemData)
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

            var nodeByLineId = new Dictionary<int, EventFlowNode>();
            var userNormalNodes = new Dictionary<string, EventFlowNode>(StringComparer.Ordinal);
            var systemLabels = new Dictionary<string, (EventDataLine line, EventFlowNode node)>(StringComparer.Ordinal);
            var labelCounter = 0;

            foreach (var line in lines)
            {
                var data = line.Data ?? "";
                if (data.Length <= 1 || !data.EndsWith(":", StringComparison.Ordinal))
                {
                    continue;
                }
                var labelName = data.Substring(0, data.Length - 1);
                var fullFile = string.IsNullOrEmpty(line.File) ? mainFile : OverlayFileSystem.Normalize(line.File);
                var relFile = NormalizeRelPath(root, fullFile);
                var (kind, triggerType) = ClassifyLabel(labelName, line.IsSystemData);

                var node = new EventFlowNode
                {
                    Id = $"lbl{labelCounter++}",
                    Name = labelName,
                    Kind = kind,
                    TriggerType = triggerType,
                    File = relFile,
                    FullFile = fullFile,
                    Line = line.LineNum,
                    Exists = true,
                    Included = !ScenarioAnalysis.SamePath(fullFile, mainFile),
                };

                if (line.IsSystemData)
                {
                    if (!systemLabels.ContainsKey(labelName))
                    {
                        systemLabels[labelName] = (line, node);
                    }
                }
                else
                {
                    nodeByLineId[line.ID] = node;
                    graph.Nodes.Add(node);
                    if (kind == EventNodeKind.Subroutine || kind == EventNodeKind.Entry)
                    {
                        if (!userNormalNodes.ContainsKey(labelName))
                        {
                            userNormalNodes[labelName] = node;
                        }
                    }
                }
            }

            var fs = new OverlayFileSystem(new LocalFileSystem(), overlays);
            var requireFileNodes = new Dictionary<string, EventFlowNode>(StringComparer.OrdinalIgnoreCase);
            var requiredLabelNodes = new Dictionary<string, EventFlowNode>(StringComparer.Ordinal);
            var missingNodes = new Dictionary<string, EventFlowNode>(StringComparer.Ordinal);
            var topLevelNode = default(EventFlowNode);
            var dynamicCounter = 0;

            EventFlowNode EnsureTopLevelNode(string filePath)
            {
                if (topLevelNode != null)
                {
                    return topLevelNode;
                }
                topLevelNode = new EventFlowNode
                {
                    Id = "top",
                    Name = "(ファイル先頭)",
                    Kind = EventNodeKind.Subroutine,
                    File = NormalizeRelPath(root, filePath),
                    FullFile = filePath,
                    Line = 1,
                    Exists = true,
                };
                graph.Nodes.Insert(0, topLevelNode);
                return topLevelNode;
            }

            EventFlowNode EnsureMissingNode(string key, string displayName)
            {
                if (missingNodes.TryGetValue(key, out var existing))
                {
                    return existing;
                }
                var node = new EventFlowNode
                {
                    Id = $"miss{missingNodes.Count}",
                    Name = displayName,
                    Kind = EventNodeKind.Missing,
                    Exists = false,
                };
                missingNodes[key] = node;
                graph.Nodes.Add(node);
                return node;
            }

            EventFlowNode EnsureDynamicNode(string expr)
            {
                var node = new EventFlowNode
                {
                    Id = $"dyn{dynamicCounter++}",
                    Name = expr ?? "(動的)",
                    Kind = EventNodeKind.Dynamic,
                    Exists = true,
                };
                graph.Nodes.Add(node);
                return node;
            }

            EventFlowNode EnsureRequireFile(string targetName, string currentFile)
            {
                if (requireFileNodes.TryGetValue(targetName, out var existing))
                {
                    return existing;
                }
                var resolved = ResolveRequirePath(fs, root, resolvedAppPath, currentFile, targetName);
                var node = new EventFlowNode
                {
                    Id = $"req{requireFileNodes.Count}",
                    Name = targetName,
                    Kind = resolved != null ? EventNodeKind.RequireFile : EventNodeKind.Missing,
                    File = resolved != null ? NormalizeRelPath(root, resolved) : targetName,
                    FullFile = resolved,
                    Line = resolved != null ? 1 : 0,
                    Exists = resolved != null,
                    Required = true,
                };
                requireFileNodes[targetName] = node;
                graph.Nodes.Add(node);

                if (resolved != null)
                {
                    foreach (var (lblName, lblLine) in ReadRequiredFileLabels(fs, resolved))
                    {
                        if (!userNormalNodes.ContainsKey(lblName) && !requiredLabelNodes.ContainsKey(lblName))
                        {
                            var (lkind, ltrig) = ClassifyLabel(lblName, isSystemData: false);
                            var rnode = new EventFlowNode
                            {
                                Id = $"lbl{labelCounter++}",
                                Name = lblName,
                                Kind = lkind,
                                TriggerType = ltrig,
                                File = NormalizeRelPath(root, resolved),
                                FullFile = resolved,
                                Line = lblLine,
                                Exists = true,
                                Required = true,
                            };
                            requiredLabelNodes[lblName] = rnode;
                        }
                    }
                }
                return node;
            }

            // First pass on Require commands so all required labels are known before resolving Call/Goto
            foreach (var cmd in cmds)
            {
                if (cmd.EventData == null || cmd.EventData.IsSystemData || cmd.Name != CmdType.RequireCmd || cmd.ArgNum < 2)
                {
                    continue;
                }
                var target = cmd.GetArg(2);
                var raw = cmd.GetArgRaw(2).RawArg ?? target;
                if (!IsDynamic(raw, assignedVars) && !string.IsNullOrWhiteSpace(target))
                {
                    EnsureRequireFile(target, cmd.EventData.File);
                }
            }

            var condStack = new Stack<BlockFrame>();
            EventFlowNode currentLabel = null;
            var lineCursor = 0;

            foreach (var cmd in cmds)
            {
                if (cmd.EventData == null)
                {
                    continue;
                }
                var cmdId = cmd.EventData.ID;
                while (lineCursor < lines.Count && lines[lineCursor].ID <= cmdId)
                {
                    if (nodeByLineId.TryGetValue(lines[lineCursor].ID, out var lblNode))
                    {
                        currentLabel = lblNode;
                        condStack.Clear();
                    }
                    lineCursor++;
                }

                if (cmd.EventData.IsSystemData)
                {
                    continue;
                }

                var rawLine = cmdId >= 0 && cmdId < lines.Count ? (lines[cmdId].Data ?? "") : (cmd.EventData.Data ?? "");
                var fullFile = string.IsNullOrEmpty(cmd.EventData.File) ? mainFile : OverlayFileSystem.Normalize(cmd.EventData.File);
                var relFile = NormalizeRelPath(root, fullFile);
                var lineNum = cmd.EventData.LineNum;

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
                            SwitchExpr = cmd.ArgNum >= 2 ? (cmd.GetArgRaw(2).RawArg ?? cmd.GetArg(2)) : "Switch",
                            Condition = null,
                        });
                        continue;

                    case CmdType.CaseCmd:
                        if (condStack.Count > 0 && condStack.Peek().IsSwitch)
                        {
                            var vals = new List<string>();
                            for (var j = 2; j <= cmd.ArgNum; j++)
                            {
                                vals.Add(cmd.GetArgRaw(j).RawArg ?? cmd.GetArg(j));
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

                if (cmd.Name == CmdType.RequireCmd && cmd.ArgNum >= 2)
                {
                    var fromNode = currentLabel ?? EnsureTopLevelNode(fullFile);
                    var val = cmd.GetArg(2);
                    var raw = cmd.GetArgRaw(2).RawArg ?? val;
                    var isDyn = IsDynamic(raw, assignedVars);
                    if (isDyn)
                    {
                        var dynNode = EnsureDynamicNode($"Require {raw}");
                        AddEdge(graph, fromNode, dynNode, EventEdgeKind.Require, relFile, fullFile, lineNum, cond, "Require", raw, null, isDynamic: true, isMissing: false);
                    }
                    else
                    {
                        var reqNode = EnsureRequireFile(val, fullFile);
                        AddEdge(graph, fromNode, reqNode, EventEdgeKind.Require, relFile, fullFile, lineNum, cond, "Require", val, null, isDynamic: false, isMissing: !reqNode.Exists);
                        if (!reqNode.Exists)
                        {
                            graph.Warnings.Add(new EventFlowWarning
                            {
                                File = relFile,
                                FullFile = fullFile,
                                Line = lineNum,
                                Target = val,
                                Message = $"Require 先のファイル「{val}」が見つかりません",
                            });
                        }
                    }
                    continue;
                }

                if (cmd.Name == CmdType.CallCmd || cmd.Name == CmdType.GotoCmd)
                {
                    var keyword = rawLine.TrimStart().Split(' ', '\t')[0].ToLowerInvariant();
                    if (keyword != "call" && keyword != "goto")
                    {
                        continue;
                    }
                    if (cmd.ArgNum < 2)
                    {
                        continue;
                    }
                    var edgeKind = cmd.Name == CmdType.CallCmd ? EventEdgeKind.Call : EventEdgeKind.Goto;
                    var cmdName = cmd.Name == CmdType.CallCmd ? "Call" : "Goto";
                    var fromNode = currentLabel ?? EnsureTopLevelNode(fullFile);
                    var val = cmd.GetArg(2);
                    var raw = cmd.GetArgRaw(2).RawArg ?? val;
                    var args = cmd.ArgNum >= 3
                        ? string.Join(" ", Enumerable.Range(3, cmd.ArgNum - 2).Select(idx => cmd.GetArgRaw(idx).RawArg ?? cmd.GetArg(idx)))
                        : null;

                    if (IsDynamic(raw, assignedVars))
                    {
                        var dynNode = EnsureDynamicNode(raw);
                        AddEdge(graph, fromNode, dynNode, edgeKind, relFile, fullFile, lineNum, cond, cmdName, raw, args, isDynamic: true, isMissing: false);
                        continue;
                    }

                    EventFlowNode toNode = null;
                    if (userNormalNodes.TryGetValue(raw, out var userNode))
                    {
                        toNode = userNode;
                    }
                    else if (requiredLabelNodes.TryGetValue(raw, out var reqLabelNode))
                    {
                        if (!graph.Nodes.Contains(reqLabelNode))
                        {
                            graph.Nodes.Add(reqLabelNode);
                        }
                        toNode = reqLabelNode;
                    }
                    else if (systemLabels.TryGetValue(raw, out var sys))
                    {
                        if (!graph.Nodes.Contains(sys.node))
                        {
                            graph.Nodes.Add(sys.node);
                        }
                        toNode = sys.node;
                    }

                    if (toNode != null)
                    {
                        AddEdge(graph, fromNode, toNode, edgeKind, relFile, fullFile, lineNum, cond, cmdName, raw, args, isDynamic: false, isMissing: false);
                    }
                    else
                    {
                        var missNode = EnsureMissingNode("label:" + raw, raw);
                        AddEdge(graph, fromNode, missNode, edgeKind, relFile, fullFile, lineNum, cond, cmdName, raw, args, isDynamic: false, isMissing: true);
                        graph.Warnings.Add(new EventFlowWarning
                        {
                            File = relFile,
                            FullFile = fullFile,
                            Line = lineNum,
                            Target = raw,
                            Message = $"ラベル「{raw}」が見つかりません",
                        });
                    }
                }
            }

            return graph;
        }

        private static void AddEdge(
            EventFlowGraph graph,
            EventFlowNode from,
            EventFlowNode to,
            EventEdgeKind kind,
            string relFile,
            string fullFile,
            int line,
            string condition,
            string command,
            string target,
            string args,
            bool isDynamic,
            bool isMissing)
        {
            from.OutgoingCount++;
            to.IncomingCount++;
            graph.Edges.Add(new EventFlowEdge
            {
                From = from.Id,
                To = to.Id,
                Kind = kind,
                File = relFile,
                FullFile = fullFile,
                Line = line,
                EnclosingLabel = from.Name,
                Condition = condition,
                Command = command,
                Target = target,
                Args = string.IsNullOrEmpty(args) ? null : args,
                IsDynamic = isDynamic,
                IsMissing = isMissing,
            });
        }

        private static (EventNodeKind kind, string triggerType) ClassifyLabel(string labelName, bool isSystemData)
        {
            if (isSystemData)
            {
                return (EventNodeKind.System, null);
            }
            var stripped = (labelName ?? "").TrimStart('*').TrimStart();
            var first = stripped.Split(new[] { ' ', '\t', '　' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            if (EntryLabels.Contains(first))
            {
                return (EventNodeKind.Entry, first);
            }
            if (TriggerKeywords.Contains(first))
            {
                return (EventNodeKind.Trigger, first);
            }
            if (CommandKeywords.Contains(first))
            {
                return (EventNodeKind.Command, first);
            }
            return (EventNodeKind.Subroutine, null);
        }

        private static bool IsDynamic(string raw, HashSet<string> assignedVars)
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

        private static string ResolveRequirePath(IFileSystem fs, string scenarioRoot, string appPath, string currentFile, string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return null;
            }
            var candidates = new[]
            {
                scenarioRoot,
                Path.GetDirectoryName(currentFile ?? ""),
                appPath,
            };
            foreach (var dir in candidates)
            {
                if (string.IsNullOrEmpty(dir))
                {
                    continue;
                }
                var path = fs.PathCombine(dir, target);
                if (fs.FileExists(path))
                {
                    return OverlayFileSystem.Normalize(path);
                }
            }
            return null;
        }

        private static IEnumerable<(string name, int line)> ReadRequiredFileLabels(IFileSystem fs, string path)
        {
            var result = new List<(string, int)>();
            try
            {
                using var stream = fs.Open(path);
                using var reader = new StreamReader(stream, Encoding.UTF8, true);
                string line;
                var lineNum = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNum++;
                    var trimmed = line.Trim();
                    if (trimmed.Length > 1 && trimmed.EndsWith(":", StringComparison.Ordinal) && !trimmed.StartsWith("#", StringComparison.Ordinal) && !trimmed.StartsWith("//", StringComparison.Ordinal))
                    {
                        result.Add((trimmed.Substring(0, trimmed.Length - 1), lineNum));
                    }
                }
            }
            catch (IOException)
            {
            }
            return result;
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

        private static string NormalizeRelPath(string root, string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return "";
            }
            if (string.IsNullOrEmpty(root))
            {
                return Path.GetFileName(fullPath);
            }
            return Path.GetRelativePath(root, fullPath).Replace('\\', '/');
        }

        private sealed class BlockFrame
        {
            public bool IsSwitch { get; set; }
            public string SwitchExpr { get; set; }
            public string Condition { get; set; }
        }
    }
}
