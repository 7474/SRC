using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReplaceArgx
{
    class Program
    {
        static async Task Main(string[] args)
        {
            args = new string[]
            {
                @"C:\Users\koudenpa\source\repos\src\SRC\SRC.Sharp"
            };
            var sw = new Stopwatch();
            sw.Start();

            var hasError = false;
            var files = args
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "*.cs", SearchOption.AllDirectories))
                .Concat(args.Where(x => File.Exists(x)));

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                await ReplaceArgxAsync(fileInfo);
            }

            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            if (hasError)
            {
                Environment.ExitCode = -1;
            }
        }

        private static async Task ReplaceArgxAsync(FileInfo file)
        {
            Console.WriteLine($"{file.Name}");
            var argReg = new Regex("^[/ ]*[_a-zA-Z0-9]+ (arg[_a-zA-Z0-9]+) = (.*);");
            var argDic = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                using (var reader = new StreamReader(file.OpenRead()))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var m = argReg.Match(line);
                        if (m.Success)
                        {
                            argDic[m.Groups[1].Value] = m.Groups[2].Value;
                        }
                        else
                        {
                            foreach (var k in argDic.Keys)
                            {
                                var replacedLine = line.Replace(k, argDic[k]);
                                if (replacedLine != line)
                                {
                                    Console.WriteLine($"{file.Name}: {k} -> {argDic[k]}");
                                    argDic.Remove(k);
                                    line = replacedLine;
                                }
                            }
                            await writer.WriteLineAsync(line);
                        }
                    }
                }
                writer.Flush();
                await File.WriteAllBytesAsync(file.FullName, ms.ToArray());
            }
        }
    }
}
