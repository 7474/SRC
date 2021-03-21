using SRCCore.Exceptions;
using SRCCore.Filesystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SRCDataLinter
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var SRC = new SRCCore.SRC();
            SRC.AppPath = AppContext.BaseDirectory;
            SRC.ScenarioPath = Environment.CurrentDirectory;
            SRC.GUI = new LinterGUI();
            SRC.FileSystem = new LocalFileSystem();
            SRC.Event.InitEventData();

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
                hasError |= ValidateFile(hasError, SRC, fileInfo);
            }

            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            if (hasError)
            {
                Environment.ExitCode = -1;
            }
        }

        private static bool ValidateFile(bool hasError, SRCCore.SRC SRC, FileInfo file)
        {
            try
            {
                switch (file.Name.ToLower())
                {
                    case "unit.txt":
                    case "robot.txt":
                        SRC.UDList.Load(file.Name, file.OpenRead());
                        break;
                    case "pilot.txt":
                        SRC.PDList.Load(file.Name, file.OpenRead());
                        break;
                    case "pilot_message.txt":
                        SRC.MDList.Load(file.Name, false, file.OpenRead());
                        break;
                    case "pilot_dialog.txt":
                        SRC.DDList.Load(file.Name, file.OpenRead());
                        break;
                    case "item.txt":
                        SRC.IDList.Load(file.Name, file.OpenRead());
                        break;
                    default:
                        if (file.Name.ToLower().EndsWith(".eve"))
                        {
                            // TODO 検証用のモードを用意してもいいかもしれない。
                            SRC.Event.LoadEventData(file.FullName, "");
                        }
                        else
                        {
                            Console.WriteLine($"Not supported file [{file.Name}]");
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
                hasError = true;
            }
            foreach (var id in SRC.DataErrors)
            {
                Console.Error.WriteLine($"{file.FullName}({id.line_num}): warning: {id.msg}[{id.dname}]");
            }
            hasError |= SRC.HasDataError;
            SRC.ClearDataError();

            return hasError;
        }
    }
}
