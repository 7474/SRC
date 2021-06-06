using ShiftJISExtension;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                .SelectMany(x => Directory.EnumerateFiles(x, "*.txt", SearchOption.AllDirectories))
                .Concat(args
                    .Where(x => Directory.Exists(x))
                    .SelectMany(x => Directory.EnumerateFiles(x, "*.eve", SearchOption.AllDirectories)))
                .Concat(args.Where(x => File.Exists(x)));

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                hasError |= await ValidateFileAsync(hasError, SRC, fileInfo);
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

        private static async Task<bool> ValidateFileAsync(bool hasError, SRCCore.SRC SRC, FileInfo file)
        {
            try
            {
                switch (file.Name.ToLower())
                {
                    case "unit.txt":
                    case "robot.txt":
                        SRC.UDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    case "pilot.txt":
                        SRC.PDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    case "non_pilot.txt":
                        SRC.NPDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    case "pilot_message.txt":
                        SRC.MDList.Load(file.Name, false, await OpenUtf8Async(SRC, file));
                        break;
                    case "pilot_dialog.txt":
                        SRC.DDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    case "effect.txt":
                        SRC.EDList.Load(file.Name, true, await OpenUtf8Async(SRC, file));
                        break;
                    case "animation.txt":
                        SRC.ADList.Load(file.Name, false, await OpenUtf8Async(SRC, file));
                        break;
                    case "item.txt":
                        SRC.IDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    case "alias.txt":
                        SRC.ALDList.Load(file.Name, await OpenUtf8Async(SRC, file));
                        break;
                    default:
                        if (file.Name.ToLower().EndsWith(".eve"))
                        {
                            SRC.Event.LoadEventData(file.FullName, "");
                        }
                        else
                        {
                            Console.WriteLine($"Not supported [{file.FullName}]");
                            return false;
                        }
                        break;
                }
            }
            catch (InvalidSrcDataException ex)
            {
                foreach (var id in ex.InvalidDataList)
                {
                    Console.Error.WriteLine($"{file.FullName}({id.line_num}): error: {id.msg}[{id.dname}]");
                }
                hasError = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{file.FullName}({1}): error: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                hasError = true;
            }
            foreach (var id in SRC.DataErrors)
            {
                Console.Error.WriteLine($"{file.FullName}({id.line_num}): warning: {id.msg}[{id.dname}]");
            }
            hasError |= SRC.HasDataError;
            SRC.ClearDataError();
            Console.WriteLine($"Checked [{file.FullName}]");

            return hasError;
        }
    }
}
