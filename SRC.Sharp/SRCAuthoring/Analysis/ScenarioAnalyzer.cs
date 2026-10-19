using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Filesystem;
using SRCCore.Extensions;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Analysis
{
    /// <summary>イベントのラベル（「名前:」の行）</summary>
    public sealed class EveLabel
    {
        public string Name { get; set; }
        public string File { get; set; }
        /// <summary>1 始まりの行番号</summary>
        public int Line { get; set; }
    }

    /// <summary>include（「&lt;ファイル&gt;」の行）</summary>
    public sealed class EveInclude
    {
        public string File { get; set; }
        public int Line { get; set; }
        /// <summary>include の指定（&lt;&gt; の中）</summary>
        public string Target { get; set; }
        /// <summary>見つかったファイルの絶対パス（見つからなければ null）</summary>
        public string ResolvedPath { get; set; }
    }

    public sealed class ScenarioAnalysis
    {
        public string ScenarioRoot { get; set; }
        public string MainFile { get; set; }
        public List<SrcDiagnostic> Diagnostics { get; } = new List<SrcDiagnostic>();
        public List<EveLabel> Labels { get; } = new List<EveLabel>();
        public List<EveInclude> Includes { get; } = new List<EveInclude>();
        /// <summary>読み込んだイベントの行（include 先を含む）</summary>
        public IList<EventDataLine> Lines { get; set; } = new List<EventDataLine>();

        public IEnumerable<SrcDiagnostic> DiagnosticsFor(string file)
            => Diagnostics.Where(x => SamePath(x.File, file));

        public static bool SamePath(string a, string b)
            => string.Equals(OverlayFileSystem.Normalize(a), OverlayFileSystem.Normalize(b), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// イベントファイルを SRCCore の読み込み処理で解析する（エディタの診断・ラベル・include のため）。
    /// </summary>
    public sealed class ScenarioAnalyzer
    {
        static ScenarioAnalyzer()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>SRC 本体のフォルダ（Lib や Data の共通ファイルを探す）。null ならツールのフォルダ</summary>
        public string AppPath { get; set; }

        /// <summary>
        /// シナリオのフォルダを推定する。src.ini か Data フォルダのある、いちばん近い親フォルダ。無ければファイルのあるフォルダ。
        /// </summary>
        public static string FindScenarioRoot(string file)
        {
            var dir = Path.GetDirectoryName(OverlayFileSystem.Normalize(file));
            for (var d = dir; !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
            {
                if (File.Exists(Path.Combine(d, "src.ini")) || File.Exists(Path.Combine(d, "Src.ini"))
                    || Directory.Exists(Path.Combine(d, "Data")) || Directory.Exists(Path.Combine(d, "data")))
                {
                    return d;
                }
            }
            return dir;
        }

        /// <summary>
        /// イベントファイルを実行するときのシナリオのフォルダ（ScenarioPath）を推定する。
        /// ファイルのあるフォルダに src.ini か Data があれば、そのフォルダ（元の SRC と同じ）。
        /// 無ければ、それを持ついちばん近い親（シナリオのサブフォルダに置いたイベントファイル）。
        /// ただし Data\System のあるフォルダは SRC 本体とみなして選ばず、ファイルのあるフォルダにする。
        /// </summary>
        public static string FindRunRoot(string file)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(file));
            var root = FindScenarioRoot(file);
            if (string.IsNullOrEmpty(root) || root == dir)
            {
                return dir;
            }
            if (Directory.Exists(Path.Combine(root, "Data", "System")) || Directory.Exists(Path.Combine(root, "data", "System")))
            {
                return dir;
            }
            return root;
        }

        /// <summary>
        /// SRC 本体のフォルダを推定する。シナリオのフォルダの親のうち、Lib か Data\System のあるいちばん近いもの。無ければ null。
        /// </summary>
        public static string FindAppPath(string scenarioRoot)
        {
            for (var d = Path.GetDirectoryName(scenarioRoot ?? ""); !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
            {
                if (Directory.Exists(Path.Combine(d, "Lib")) || Directory.Exists(Path.Combine(d, "Data", "System")))
                {
                    return d;
                }
            }
            return null;
        }

        /// <summary>
        /// file を解析する。overlays は編集中のファイルの内容（パス → テキスト）。
        /// </summary>
        public ScenarioAnalysis Analyze(string file, IDictionary<string, string> overlays = null, string scenarioRoot = null)
        {
            file = OverlayFileSystem.Normalize(file);
            var root = scenarioRoot ?? FindScenarioRoot(file);
            var result = new ScenarioAnalysis { ScenarioRoot = root, MainFile = file };

            var src = new SRCCore.SRC { GUI = new MockGUI() };
            src.SystemConfig.AppPath = AppPath ?? FindAppPath(root) ?? AppContext.BaseDirectory;
            src.SystemConfig.SRCCompatibilityMode = SRCCore.SRCCompatibilityMode.Read;
            src.ScenarioPath = root;
            src.FileSystem = new OverlayFileSystem(new LocalFileSystem(), overlays);
            src.Event.SkipExternalSourceLoad = true;
            try
            {
                src.Event.InitEventData();
                src.Event.LoadEventData(file, "");
            }
            catch (Exception ex)
            {
                result.Diagnostics.Add(new SrcDiagnostic { File = file, Line = 1, Severity = DiagnosticSeverity.Error, Message = ex.Message });
            }

            foreach (var e in src.DataErrors)
            {
                result.Diagnostics.Add(new SrcDiagnostic
                {
                    File = string.IsNullOrEmpty(e.fname) ? file : OverlayFileSystem.Normalize(e.fname),
                    Line = Math.Max(1, e.line_num),
                    Severity = DiagnosticSeverity.Error,
                    Message = e.msg,
                });
            }

            var lines = src.Event.EventData ?? new List<EventDataLine>();
            result.Lines = lines;
            foreach (var line in lines)
            {
                var data = line.Data ?? "";
                if (data.Length > 1 && data.EndsWith(":"))
                {
                    result.Labels.Add(new EveLabel { Name = data.Substring(0, data.Length - 1), File = OverlayFileSystem.Normalize(line.File), Line = line.LineNum });
                }
            }

            CheckLabelReferences(src, result);
            CollectIncludes(src, file, overlays, result);
            return result;
        }

        /// <summary>Call・Goto の飛び先のラベルがあるか（変数で指定していそうなものは除く）</summary>
        private static void CheckLabelReferences(SRCCore.SRC src, ScenarioAnalysis result)
        {
            var cmds = src.Event.EventCmd ?? new List<CmdData>();
            // Set・Local などで値を入れている名前は、ラベル名を入れた変数かもしれないので調べない
            var assigned = new HashSet<string>(cmds
                .Where(x => x.Name == CmdType.SetCmd || x.Name == CmdType.LocalCmd || x.Name == CmdType.GlobalCmd)
                .Select(x => x.GetArg(2))
                .Where(x => !string.IsNullOrEmpty(x)));
            var required = RequiredLabels(src, cmds);
            foreach (var cmd in cmds)
            {
                if (cmd.EventData == null || cmd.EventData.IsSystemData)
                {
                    continue;
                }
                if (cmd.Name != CmdType.CallCmd && cmd.Name != CmdType.GotoCmd)
                {
                    continue;
                }
                // 書かれたとおりの Call・Goto だけを調べる（知らないコマンドの行も Call とみなされるが、Talk の本文や Ask の選択肢のことが多い）
                var data = src.Event.EventData;
                var original = cmd.EventData.ID >= 0 && cmd.EventData.ID < data.Count ? data[cmd.EventData.ID].Data ?? "" : "";
                var keyword = original.TrimStart().Split(' ', '\t')[0].ToLowerInvariant();
                if (keyword != "call" && keyword != "goto")
                {
                    continue;
                }
                var raw = cmd.GetArgRaw(2).RawArg ?? cmd.GetArg(2);
                if (string.IsNullOrEmpty(raw) || raw.IndexOfAny(new[] { '$', '(', '"', '`', '[' }) >= 0 || assigned.Contains(raw))
                {
                    continue;
                }
                if (src.Event.FindNormalLabel(raw) < 0 && !required.Contains(raw))
                {
                    result.Diagnostics.Add(new SrcDiagnostic
                    {
                        File = OverlayFileSystem.Normalize(cmd.EventData.File),
                        Line = cmd.EventData.LineNum,
                        Severity = DiagnosticSeverity.Warning,
                        Message = $"ラベル「{raw}」が見つかりません",
                    });
                }
            }
        }

        /// <summary>Require で（ファイル名を書いて）実行時に読み込むファイルにあるラベル</summary>
        private static HashSet<string> RequiredLabels(SRCCore.SRC src, IList<CmdData> cmds)
        {
            var labels = new HashSet<string>();
            var files = cmds
                .Where(x => x.Name == CmdType.RequireCmd)
                .Select(x => x.GetArg(2))
                .Where(x => !string.IsNullOrEmpty(x) && x.IndexOfAny(new[] { '$', '(' }) < 0)
                .Distinct();
            foreach (var name in files)
            {
                try
                {
                    var path = src.FileSystem.PathCombine(src.ScenarioPath, name);
                    if (!src.FileSystem.FileExists(path))
                    {
                        continue;
                    }
                    using (var stream = src.FileSystem.OpenText(SRCCore.SRCCompatibilityMode.Read, path))
                    using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Length > 1 && line.EndsWith(":") && !line.StartsWith("#"))
                            {
                                labels.Add(line.Substring(0, line.Length - 1));
                            }
                        }
                    }
                }
                catch (IOException)
                {
                }
            }
            return labels;
        }

        /// <summary>メインのファイルの include の行と、その先のファイル</summary>
        private static void CollectIncludes(SRCCore.SRC src, string file, IDictionary<string, string> overlays, ScenarioAnalysis result)
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
                using (var stream = src.FileSystem.OpenText(SRCCore.SRCCompatibilityMode.Read, file))
                using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                {
                    text = reader.ReadToEnd();
                }
            }
            var physical = text.Replace("\r\n", "\n").Split('\n');
            for (var i = 0; i < physical.Length; i++)
            {
                var line = physical[i].Trim();
                if (line.Length > 2 && line.StartsWith("<") && line.EndsWith(">") && line.IndexOf('>') == line.Length - 1)
                {
                    var target = line.Substring(1, line.Length - 2).Trim();
                    string resolved = null;
                    foreach (var dir in new[] { src.ScenarioPath, src.ExtDataPath, src.ExtDataPath2, src.AppPath })
                    {
                        if (string.IsNullOrEmpty(dir))
                        {
                            continue;
                        }
                        var path = src.FileSystem.PathCombine(dir, target);
                        if (src.FileSystem.FileExists(path))
                        {
                            resolved = OverlayFileSystem.Normalize(path);
                            break;
                        }
                    }
                    result.Includes.Add(new EveInclude { File = file, Line = i + 1, Target = target, ResolvedPath = resolved });
                    if (resolved == null)
                    {
                        result.Diagnostics.Add(new SrcDiagnostic { File = file, Line = i + 1, Severity = DiagnosticSeverity.Warning, Message = $"include するファイル「{target}」が見つかりません" });
                    }
                }
            }
        }
    }
}
