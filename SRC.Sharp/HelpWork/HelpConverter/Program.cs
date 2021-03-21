using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HelpConverter
{
    class FileConverter
    {
        static readonly Regex nameRegex = new Regex("<TITLE>(.+)</TITLE>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public string SourcePath { get; private set; }
        public string DestPath { get; private set; }
        public string SourceContent { get; private set; }
        public string ConvertedContent { get; private set; }

        public FileConverter(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public void Convert()
        {
            SourceContent = File.ReadAllText(SourcePath);
            DestPath = ResolvePath();

            // Impl
            ConvertedContent = SourceContent;
        }

        private string ResolvePath()
        {
            return (nameRegex.Match(SourceContent).Groups.Values.Skip(1).FirstOrDefault().Value ?? SourcePath) + ".md";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            args = new string[]
            {
                @"C:\Users\koudenpa\source\repos\src\SRC\SRC.Sharp\HelpWork\html"
            };
            args
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "*.html", SearchOption.AllDirectories)
                    .Concat(Directory.EnumerateFiles(x, "*.htm", SearchOption.AllDirectories)))
                .Concat(args.Where(x => File.Exists(x)))
                .Select(x => new FileConverter(x))
                .ToList()
                .ForEach(x =>
                {
                    x.Convert();
                    Console.Out.WriteLine($"{x.SourcePath} -> {x.DestPath}");
                    File.WriteAllText(x.DestPath, x.ConvertedContent);
                });


        }


    }
}
