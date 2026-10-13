using ShiftJISExtension;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRCAuthoring.Linting
{
    /// <summary>lint の結果</summary>
    public sealed class LintResult
    {
        public List<SrcDiagnostic> Diagnostics { get; } = new List<SrcDiagnostic>();
        public List<string> CheckedFiles { get; } = new List<string>();
        /// <summary>問題があったか（警告を含む。SRCDataLinter の終了コードと同じ判定）</summary>
        public bool HasError { get; set; }
    }

    /// <summary>
    /// データ（*.txt）とイベント（*.eve）の検査。ファイル名からデータの種類を決めて、SRCCore の読み込み処理で検査する。
    /// </summary>
    public sealed class DataLinter
    {
        static DataLinter()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>ディレクトリ（中の *.txt と *.eve）またはファイルを検査する</summary>
        public async Task<LintResult> LintAsync(IEnumerable<string> paths, string scenarioPath = null)
        {
            var src = new SRCCore.SRC();
            src.SystemConfig.AppPath = AppContext.BaseDirectory;
            src.ScenarioPath = scenarioPath ?? Environment.CurrentDirectory;
            src.GUI = new MockGUI();
            src.FileSystem = new LocalFileSystem();
            src.Event.InitEventData();
            src.Event.SkipExternalSourceLoad = true;

            var targets = paths.ToList();
            var files = targets
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "*.txt", SearchOption.AllDirectories))
                .Concat(targets
                    .Where(x => Directory.Exists(x))
                    .SelectMany(x => Directory.EnumerateFiles(x, "*.eve", SearchOption.AllDirectories)))
                .Concat(targets.Where(x => File.Exists(x)))
                .ToList();

            var result = new LintResult();
            var processed = new HashSet<string>();
            foreach (var proc in LintProcs)
            {
                foreach (var file in files.Where(x => !processed.Contains(x)))
                {
                    if (await proc.ExecuteAsync(src, new FileInfo(file), result))
                    {
                        processed.Add(file);
                        result.CheckedFiles.Add(Path.GetFullPath(file));
                    }
                }
            }
            return result;
        }

        private static async Task<Stream> OpenUtf8Async(SRCCore.SRC src, FileInfo file)
        {
            var fromEnc = Encoding.GetEncoding(932);
            var stream = new MemoryStream();
            using (var fs = file.OpenRead())
            {
                var wasConverted = await fs.ConvertEncodingAsync(stream, fromEnc, Encoding.UTF8);
                if (wasConverted)
                {
                    src.AddDataError(new InvalidSrcData("Encoding is not UTF-8.", file.Name, 1, "", ""));
                }
                stream.Position = 0;
                return stream;
            }
        }

        private delegate Task LintProc(SRCCore.SRC src, FileInfo file);

        private sealed class LintProcDef
        {
            private readonly Regex filePattern;
            private readonly LintProc lintProc;

            public LintProcDef(string filePattern, LintProc lintProc)
            {
                this.filePattern = new Regex(filePattern, RegexOptions.IgnoreCase);
                this.lintProc = lintProc;
            }

            /// <summary>対象のファイルなら検査して true</summary>
            public async Task<bool> ExecuteAsync(SRCCore.SRC src, FileInfo file, LintResult result)
            {
                if (!filePattern.IsMatch(file.Name))
                {
                    return false;
                }
                try
                {
                    await lintProc(src, file);
                }
                catch (InvalidSrcDataException ex)
                {
                    foreach (var id in ex.InvalidDataList)
                    {
                        result.Diagnostics.Add(new SrcDiagnostic { File = file.FullName, Line = id.line_num, Severity = DiagnosticSeverity.Error, Message = id.msg, Source = id.dname });
                    }
                    result.HasError = true;
                }
                catch (Exception ex)
                {
                    result.Diagnostics.Add(new SrcDiagnostic { File = file.FullName, Line = 1, Severity = DiagnosticSeverity.Error, Message = ex.Message });
                    result.HasError = true;
                }
                foreach (var id in src.DataErrors)
                {
                    // イベントのエラーは読み込んだファイル（include 先を含む）のものとして出す
                    var path = string.IsNullOrEmpty(id.fname) || id.fname == file.Name ? file.FullName : id.fname;
                    result.Diagnostics.Add(new SrcDiagnostic { File = path, Line = id.line_num, Severity = DiagnosticSeverity.Warning, Message = id.msg, Source = id.dname });
                }
                result.HasError |= src.HasDataError;
                src.ClearDataError();
                return true;
            }
        }

        private static readonly LintProcDef[] LintProcs = new LintProcDef[]
        {
            new LintProcDef("^sp.txt$|^mind.txt$", async (src, file) => src.SPDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^alias.txt$", async (src, file) => src.ALDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^unit.txt$|^robot.txt$", async (src, file) => src.UDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^pilot.txt$", async (src, file) => src.PDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^non_pilot.txt$", async (src, file) => src.NPDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^pilot_message.txt$", async (src, file) => src.MDList.Load(file.Name, false, await OpenUtf8Async(src, file))),
            new LintProcDef("^pilot_dialog.txt$", async (src, file) => src.DDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("^effect.txt$", async (src, file) => src.EDList.Load(file.Name, true, await OpenUtf8Async(src, file))),
            new LintProcDef("^animation.txt$", async (src, file) => src.ADList.Load(file.Name, false, await OpenUtf8Async(src, file))),
            new LintProcDef("^item.txt$", async (src, file) => src.IDList.Load(file.Name, await OpenUtf8Async(src, file))),
            new LintProcDef("\\.eve$", (src, file) =>
            {
                src.Event.LoadEventData(file.FullName, "");
                return Task.CompletedTask;
            }),
        };
    }
}
