using SRCCore.Exceptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace SRCDataLinter
{
    class Program
    {
        static void Main(string[] args)
        {
            var hasError = false;
            var macher = new Matcher();
            macher.AddIncludePatterns(args);

            var SRC = new SRCCore.SRC();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var fileMatch in macher.Execute(new DirectoryInfoWrapper(new DirectoryInfo("."))).Files)
            {
                var file = new FileInfo(fileMatch.Path);
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
                        default:
                            Console.WriteLine($"Not supported file [{file.Name}]");
                            break;
                    }
                    //Console.WriteLine($"{sw.ElapsedMilliseconds}ms [{file.Name}]");
                }
                catch (InvalidSrcDataException ex)
                {
                    Console.WriteLine($"{file.FullName}");
                    foreach (var id in ex.InvalidDataList)
                    {
                        Console.WriteLine($"{id.line_num}: error {id.msg} {id.dname}");
                    }
                    hasError = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{file.FullName}");
                    Console.WriteLine($"1: error {ex.Message}");
                    hasError = true;
                }
            }
            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            if (hasError)
            {
                Environment.ExitCode = -1;
            }
        }
    }
}
