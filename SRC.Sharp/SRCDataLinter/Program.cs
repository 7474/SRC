using SRCAuthoring;
using SRCAuthoring.Linting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SRCDataLinter
{
    class Program
    {
        // 検査の処理は SRCAuthoring の DataLinter（srcsharp lint と共通）。出力の書式は src-lint.json の problemMatcher に合わせる
        static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = await new DataLinter().LintAsync(args, Environment.CurrentDirectory);

            foreach (var d in result.Diagnostics)
            {
                // 以前からの書式（データ名が無くても [] を付ける）
                var severity = d.Severity == DiagnosticSeverity.Error ? "error" : "warning";
                Console.Error.WriteLine($"{d.File}({d.Line}): {severity}: {d.Message}[{d.Source}]");
            }
            foreach (var file in result.NotSupportedFiles)
            {
                Console.WriteLine($"Not supported [{file}]");
            }
            foreach (var file in result.CheckedFiles)
            {
                Console.WriteLine($"Checked [{file}]");
            }

            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            if (result.HasError)
            {
                Environment.ExitCode = -1;
            }
        }
    }
}
