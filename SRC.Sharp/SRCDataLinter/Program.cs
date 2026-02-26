using ShiftJISExtension;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRCDataLinter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var SRC = new SRCCore.SRC();
            SRC.SystemConfig.AppPath = AppContext.BaseDirectory;
            SRC.ScenarioPath = Environment.CurrentDirectory;
            SRC.GUI = new MockGUI();
            SRC.FileSystem = new LocalFileSystem();
            SRC.Event.InitEventData();
            SRC.Event.SkipExternalSourceLoad = true;

            var hasError = false;
            var files = args
                .Where(x => Directory.Exists(x))
                .SelectMany(x =>
                {
                    var systemDir = Path.Combine(x, "data", "system");
                    if (Directory.Exists(systemDir))
                    {
                        // data/system が存在する場合はシステムデータを先頭に列挙し、
                        // その後に全体を再帰探索することでゲームと同様の読み込み順を実現する。
                        // Distinct により data/system 配下の重複は除去される。
                        Console.WriteLine($"Using data/system directory first: [{systemDir}]");
                        return Directory.EnumerateFiles(systemDir, "*.txt", SearchOption.AllDirectories)
                            .Concat(Directory.EnumerateFiles(systemDir, "*.eve", SearchOption.AllDirectories))
                            .Concat(Directory.EnumerateFiles(x, "*.txt", SearchOption.AllDirectories))
                            .Concat(Directory.EnumerateFiles(x, "*.eve", SearchOption.AllDirectories));
                    }
                    Console.WriteLine($"Searching all files in: [{x}]");
                    return Directory.EnumerateFiles(x, "*.txt", SearchOption.AllDirectories)
                        .Concat(Directory.EnumerateFiles(x, "*.eve", SearchOption.AllDirectories));
                })
                .Concat(args.Where(x => File.Exists(x)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var procedFiles = new HashSet<string>();
            foreach (var lintProc in LintProcs)
            {
                foreach (var file in files.Where(x => !procedFiles.Contains(x)))
                {
                    var fileInfo = new FileInfo(file);
                    var res = await lintProc.Execute(SRC, fileInfo);
                    hasError |= res.HasError;
                    if (res.Processed) { procedFiles.Add(file); }
                }
            }

            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            if (hasError)
            {
                Environment.ExitCode = -1;
            }
        }

        private static async Task<Stream> OpenUtf8Async(SRCCore.SRC SRC, FileInfo file)
        {
            var fromEnc = Encoding.GetEncoding(932);

            var stream = new MemoryStream();
            using (var fs = file.OpenRead())
            {
                var wasConverted = await fs.ConvertEncodingAsync(stream, fromEnc, Encoding.UTF8);
                if (wasConverted)
                {
                    SRC.AddDataError(new InvalidSrcData("Encoding is not UTF-8.", file.Name, 1, "", ""));
                }
                stream.Position = 0;
                return stream;
            }
        }

        static LintProcDef[] LintProcs = new LintProcDef[] {
            new LintProcDef("^sp.txt$|^mind.txt$", async (SRC, file) => {
                SRC.SPDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^alias.txt$", async (SRC, file) => {
                SRC.ALDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^unit.txt$|^robot.txt$", async (SRC, file) => {
                SRC.UDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^pilot.txt$", async (SRC, file) => {
                SRC.PDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^non_pilot.txt$", async (SRC, file) => {
                SRC.NPDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^pilot_message.txt$", async (SRC, file) => {
                SRC.MDList.Load(file.Name, false, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^pilot_dialog.txt$", async (SRC, file) => {
                SRC.DDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^effect.txt$", async (SRC, file) => {
                SRC.EDList.Load(file.Name, true, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^animation.txt$", async (SRC, file) => {
                SRC.ADList.Load(file.Name, false, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef("^item.txt$", async (SRC, file) => {
                SRC.IDList.Load(file.Name, await OpenUtf8Async(SRC, file));
            }),
            new LintProcDef(".*", (SRC, file) => {
                if (file.Name.ToLower().EndsWith(".eve"))
                {
                    SRC.Event.LoadEventData(file.FullName, "");
                }
                else
                {
                    Console.WriteLine($"Not supported [{file.FullName}]");
                }
                return Task.CompletedTask;
            }),
        };
    }

    class ProcResult
    {
        public bool HasError { get; set; }
        public bool Processed { get; set; }
    }

    // delegate ProcResult LintProc(SRCCore.SRC SRC, FileInfo file);
    delegate Task LintProc(SRCCore.SRC SRC, FileInfo file);
    class LintProcDef
    {
        Regex FilePattern;
        LintProc LintProc;
        public LintProcDef(string filePattern, LintProc lintProc)
        {
            FilePattern = new Regex(filePattern, RegexOptions.IgnoreCase);
            LintProc = lintProc;
        }

        public async Task<ProcResult> Execute(SRCCore.SRC SRC, FileInfo file)
        {
            var result = new ProcResult();
            if (!FilePattern.IsMatch(file.Name)) { return result; }
            result.Processed = true;
            try
            {
                await LintProc(SRC, file);
            }
            catch (InvalidSrcDataException ex)
            {
                foreach (var id in ex.InvalidDataList)
                {
                    Console.Error.WriteLine($"{file.FullName}({id.line_num}): error: {id.msg}[{id.dname}]");
                }
                result.HasError = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{file.FullName}({1}): error: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                result.HasError = true;
            }
            foreach (var id in SRC.DataErrors)
            {
                Console.Error.WriteLine($"{file.FullName}({id.line_num}): warning: {id.msg}[{id.dname}]");
            }
            result.HasError |= SRC.HasDataError;
            SRC.ClearDataError();
            Console.WriteLine($"Checked [{file.FullName}]");

            return result;
        }
    }
}
