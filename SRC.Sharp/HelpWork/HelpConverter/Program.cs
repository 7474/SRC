using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HelpConverter
{
    class FileConverter
    {
        static readonly Regex nameRegex = new Regex("<TITLE>(.+)</TITLE>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static readonly Regex anchorRegex = new Regex("<A HREF=\".+?.htm?\">(.+?)</A>", RegexOptions.IgnoreCase);
        static readonly Regex tagRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);

        static readonly ReverseMarkdown.Converter converter = new ReverseMarkdown.Converter(new ReverseMarkdown.Config
        {
            // Objectタグを落とす
            UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Drop,
            GithubFlavored = true,
        });
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
            ConvertedContent = ConvertContent();
        }

        private string ResolvePath()
        {
            return "md/" + (nameRegex.Match(SourceContent).Groups.Values.Skip(1).FirstOrDefault()?.Value ?? SourcePath) + ".md";
        }

        private string ConvertContent()
        {
            var tmpContent = anchorRegex.Replace(SourceContent, (m) =>
            {
                var rawName = m.Groups[1].Value;
                var name = tagRegex.Replace(rawName, "");
                return $"<a href=\"{name}.md\">{rawName}</a>";
            });
            return converter.Convert(tmpContent).TrimStart();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("md");
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
