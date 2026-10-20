using SRCAuthoring;
using SRCAuthoring.Linting;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// JavaScript（wwwroot/srclint.js）から LintExports を呼ぶ。Main では何もしない
return;

namespace SRCLintWeb
{
    /// <summary>
    /// ブラウザのメモリ上のファイルシステムにシナリオのファイルを置き、DataLinter で検査する。
    /// scenario/ の下に zip の中身（*.eve・*.txt）、system/ の下に汎用データ（Data/System の *.txt）を置く。
    /// </summary>
    public static partial class LintExports
    {
        private const string Root = "/srclint";
        private static string ScenarioDir => Path.Combine(Root, "scenario");
        private static string SystemDir => Path.Combine(Root, "system");

        /// <summary>前の検査で置いたファイルを消す</summary>
        [JSExport]
        internal static void Reset()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }

        /// <summary>ファイルを置く。path は "scenario/..." か "system/..."</summary>
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
        /// 検査して結果を JSON で返す。scenarioRoot はシナリオのフォルダ（scenario/ からの相対。イベントの include の基準）。
        /// 結果: { "diagnostics": [{ "file", "line", "severity"（error|warning）, "message", "source" }], "checkedFiles": 数 }
        /// </summary>
        [JSExport]
        internal static async Task<string> LintAsync(string scenarioRoot)
        {
            Directory.CreateDirectory(ScenarioDir);
            var system = Directory.Exists(SystemDir)
                ? Directory.EnumerateFiles(SystemDir, "*.txt", SearchOption.AllDirectories).ToList()
                : new System.Collections.Generic.List<string>();
            var scenarioPath = Path.Combine(ScenarioDir, scenarioRoot ?? "");
            var result = await new DataLinter().LintAsync(new[] { ScenarioDir }, scenarioPath, system);

            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                w.WriteStartArray("diagnostics");
                // SRC# は Shift_JIS のデータも読めるので、文字コードの警告は出さない
                foreach (var d in result.Diagnostics.Where(x => x.Message != "Encoding is not UTF-8."))
                {
                    w.WriteStartObject();
                    w.WriteString("file", Relative(d.File));
                    w.WriteNumber("line", d.Line);
                    w.WriteString("severity", d.Severity == DiagnosticSeverity.Error ? "error" : "warning");
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
