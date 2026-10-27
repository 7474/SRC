using SRCAuthoring;
using SRCAuthoring.Analysis;
using SRCAuthoring.Linting;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// JavaScript（wwwroot/srclint-core.js。srclint.js が Web Worker の中で読み込む）から LintExports を呼ぶ。Main では何もしない
return;

namespace SRCLintWeb
{
    /// <summary>
    /// ブラウザのメモリ上のファイルシステムにシナリオのファイルを置き、DataLinter（srcsharp lint）と ScenarioAnalyzer（静的解析）で検査する。
    /// scenario/ の下に zip の中身（*.eve・*.txt）、app/ の下に汎用データ（Data/System の *.txt と Lib の *.eve）を置く。
    /// </summary>
    public static partial class LintExports
    {
        private const string Root = "/srclint";
        private static string ScenarioDir => Path.Combine(Root, "scenario");
        // SRC 本体のフォルダの代わり（Data\System と Lib）
        private static string AppDir => Path.Combine(Root, "app");

        /// <summary>
        /// 進み具合を JavaScript に知らせる（srclint-core.js が setModuleImports("srclint", { progress }) で渡す）。
        /// phase: data（データとイベントの検査）・analyze（静的解析）・assets（素材の参照）。done は済んだ件数
        /// </summary>
        [JSImport("progress", "srclint")]
        internal static partial void ReportProgress(string phase, string file, int done, int total);

        // 素材の参照を確かめるときの、手元のファイルの一覧（AddKnownFiles で渡す。渡さなければ確かめない）
        private static AssetReferenceChecker assets;

        /// <summary>前の検査で置いたファイルを消す</summary>
        [JSExport]
        internal static void Reset()
        {
            assets = null;
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }

        /// <summary>ファイルを置く。path は "scenario/..." か "app/..."</summary>
        [JSExport]
        internal static void AddFile(string path, byte[] data)
        {
            var full = Path.GetFullPath(Path.Combine(Root, path));
            if (!full.StartsWith(Root + "/", StringComparison.Ordinal))
            {
                throw new ArgumentException("不正なパス: " + path);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllBytes(full, data);
        }

        /// <summary>
        /// 手元にあるファイルのパス（シナリオの zip と汎用データの zip の中の、画像・音などを含むすべて）を加える。
        /// 一度でも呼ぶと、イベントとデータが参照する素材のファイルがあるかも確かめる（見つからないものは info）
        /// </summary>
        [JSExport]
        internal static void AddKnownFiles(string[] paths)
        {
            assets ??= new AssetReferenceChecker();
            assets.AddKnownFiles(paths);
        }

        /// <summary>
        /// 検査して結果を JSON で返す。scenarioRoot はシナリオのフォルダ（scenario/ からの相対。イベントの include の基準）。
        /// 結果: { "diagnostics": [{ "file", "line", "severity"（error|warning|info）, "message", "source" }], "checkedFiles": 数 }
        /// </summary>
        [JSExport]
        internal static async Task<string> LintAsync(string scenarioRoot)
        {
            Directory.CreateDirectory(ScenarioDir);
            Directory.CreateDirectory(AppDir);
            var systemDir = Path.Combine(AppDir, "Data", "System");
            var system = Directory.Exists(systemDir)
                ? Directory.EnumerateFiles(systemDir, "*.txt", SearchOption.AllDirectories).ToList()
                : new System.Collections.Generic.List<string>();
            var scenarioPath = Path.Combine(ScenarioDir, scenarioRoot ?? "");
            var linter = new DataLinter { Progress = (file, done, total) => ReportProgress("data", Relative(file), done - 1, total) };
            var result = await linter.LintAsync(new[] { ScenarioDir }, scenarioPath, system);

            // 静的解析（Call・Goto の飛び先のラベル、include するファイル）。汎用データの Lib を本体のフォルダとして読む
            var diagnostics = result.Diagnostics.ToList();
            var analyzer = new ScenarioAnalyzer { AppPath = AppDir };
            var entries = EntryEvents(scenarioPath).ToList();
            for (var i = 0; i < entries.Count; i++)
            {
                var eve = entries[i];
                ReportProgress("analyze", Relative(eve), i, entries.Count);
                var analysis = analyzer.Analyze(eve, null, scenarioPath);
                // イベントの読み込みの問題は DataLinter も出すので、ラベル・include の問題だけを足す
                diagnostics.AddRange(analysis.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Warning));
            }

            // 素材の参照（画像・音・マップのファイル）
            if (assets != null)
            {
                var targets = Directory.EnumerateFiles(ScenarioDir, "*", SearchOption.AllDirectories).Where(AssetReferenceChecker.IsTarget).ToList();
                for (var i = 0; i < targets.Count; i++)
                {
                    ReportProgress("assets", Relative(targets[i]), i, targets.Count);
                    diagnostics.AddRange(assets.Check(new[] { targets[i] }));
                }
            }

            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                w.WriteStartArray("diagnostics");
                // SRC# は Shift_JIS のデータも読めるので、文字コードの警告は出さない
                var seen = new System.Collections.Generic.HashSet<string>();
                foreach (var d in diagnostics.Where(x => x.Message != "Encoding is not UTF-8." && seen.Add($"{x.File}\n{x.Line}\n{x.Message}")))
                {
                    w.WriteStartObject();
                    w.WriteString("file", Relative(d.File));
                    w.WriteNumber("line", d.Line);
                    w.WriteString("severity", d.Severity switch
                    {
                        DiagnosticSeverity.Error => "error",
                        DiagnosticSeverity.Warning => "warning",
                        _ => "info",
                    });
                    w.WriteString("message", d.Message);
                    if (!string.IsNullOrEmpty(d.Source))
                    {
                        w.WriteString("source", d.Source);
                    }
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteNumber("checkedFiles", result.CheckedFiles.Count);
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// 入口のイベントファイル。ほかのイベントファイルから include されるものと、シナリオの Lib の下（自動で読み込まれる）は除く。
        /// include される側を単独で解析すると、呼び出し元にあるラベルを「見つからない」としてしまうため
        /// </summary>
        private static System.Collections.Generic.IEnumerable<string> EntryEvents(string scenarioPath)
        {
            var eves = Directory.EnumerateFiles(ScenarioDir, "*.eve", SearchOption.AllDirectories).ToList();
            var included = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var eve in eves)
            {
                foreach (var raw in ReadText(eve).Split('\n'))
                {
                    var line = raw.Trim();
                    if (line.Length > 2 && line.StartsWith("<") && line.EndsWith(">") && line.IndexOf('>') == line.Length - 1)
                    {
                        var target = line.Substring(1, line.Length - 2).Trim().Replace('\\', '/');
                        included.Add(Path.GetFullPath(Path.Combine(scenarioPath, target)));
                    }
                }
            }
            var lib = Path.Combine(scenarioPath, "Lib") + "/";
            return eves.Where(x => !included.Contains(Path.GetFullPath(x)) && !x.StartsWith(lib, StringComparison.OrdinalIgnoreCase));
        }

        // UTF-8 として正しく読めれば UTF-8、だめなら Shift_JIS（SRC のデータの文字コード）
        private static string ReadText(string path)
        {
            var bytes = File.ReadAllBytes(path);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(932).GetString(bytes);
            }
        }

        private static string Relative(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return "";
            }
            var prefix = ScenarioDir + "/";
            return file.StartsWith(prefix, StringComparison.Ordinal) ? file.Substring(prefix.Length) : Path.GetFileName(file);
        }
    }
}
