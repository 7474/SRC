using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExampleConv
{
    class FileConverter
    {
        static readonly Regex exampleRegex = new Regex("(\\*\\*例.*\\*\\*)\\r\\n((.*\\r\\n)+?)(\\*\\*|$)", RegexOptions.Multiline);

        public string SourcePath { get; private set; }
        public string DestPath => SourcePath;
        public string SourceContent { get; private set; }
        public string ConvertedContent { get; private set; }

        public FileConverter(string sourcePath)
        {
            SourcePath = sourcePath;
            SourceContent = File.ReadAllText(SourcePath);
        }

        public void ConvertContent()
        {
            ConvertedContent = exampleRegex.Replace(SourceContent, (m) =>
            {
                var heading = m.Groups[1].Value;
                var text = m.Groups[2].Value;
                var end = m.Groups[4].Value;
                return heading + Environment.NewLine
                    + "```sh" + Environment.NewLine
                    + text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine).TrimStart()
                    + "```" + Environment.NewLine
                    + Environment.NewLine
                    + end;
            });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            args = new string[]
            {
                @"C:\Users\koudenpa\source\repos\src\SRC\SRC.Sharp\Help"
            };
            var converters = args
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "*.md", SearchOption.AllDirectories))
                .Concat(args.Where(x => File.Exists(x)))
                .Select(x => new FileConverter(x))
                .ToList();

            converters.ForEach(x =>
            {
                x.ConvertContent();
                Console.Out.WriteLine($"{x.SourcePath} -> {x.DestPath}");
                File.WriteAllText(x.DestPath, x.ConvertedContent);
            });
        }
    }
}
