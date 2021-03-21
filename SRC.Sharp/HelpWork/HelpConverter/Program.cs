using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HelpConverter
{
    class FileConverter
    {
        static readonly Regex nameRegex = new Regex("<TITLE>(.+)</TITLE>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static readonly Regex anchorRegex = new Regex("<A HREF=\"(.+?.htm?)\">(.+?)</A>", RegexOptions.IgnoreCase);
        static readonly Regex tagRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);

        static readonly ReverseMarkdown.Converter converter = new ReverseMarkdown.Converter(new ReverseMarkdown.Config
        {
            // Objectタグを落とす
            UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Drop,
            // GitHubかそのWikiターゲットなので
            GithubFlavored = true,
        });
        public string SourcePath { get; private set; }
        public string DestPath { get; private set; }
        public string SourceContent { get; private set; }
        public string ConvertedContent { get; private set; }

        public FileConverter(string sourcePath)
        {
            SourcePath = sourcePath;
            SourceContent = File.ReadAllText(SourcePath);
        }

        public void ResolvePath()
        {
            DestPath = "md/" + (
                nameRegex.Match(SourceContent).Groups.Values.Skip(1).FirstOrDefault()?.Value
                ?? Path.GetFileName(SourcePath)
            ) + ".md";
        }

        public void ConvertContent(IDictionary<string, string> fileNameMap)
        {
            var tmpContent = anchorRegex.Replace(SourceContent, (m) =>
            {
                var href = m.Groups[1].Value;
                var text = m.Groups[2].Value;
                var name = tagRegex.Replace(text, "");
                string newHref;
                if (!fileNameMap.TryGetValue(href.ToLower(), out newHref))
                {
                    newHref = name + ".md";
                }
                return $"<a href=\"{newHref}\">{text}</a>";
            });
            ConvertedContent = converter.Convert(tmpContent).TrimStart();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("md");
            var converters = args
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "*.html", SearchOption.AllDirectories)
                    .Concat(Directory.EnumerateFiles(x, "*.htm", SearchOption.AllDirectories)))
                .Concat(args.Where(x => File.Exists(x)))
                .Select(x => new FileConverter(x))
                .ToList();

            converters.ForEach(x => x.ResolvePath());

            var fileNameMap = converters.ToDictionary(x => Path.GetFileName(x.SourcePath).ToLower(), x => Path.GetFileName(x.DestPath));

            converters.ForEach(x =>
            {
                x.ConvertContent(fileNameMap);
                Console.Out.WriteLine($"{x.SourcePath} -> {x.DestPath}");
                File.WriteAllText(x.DestPath, x.ConvertedContent);
            });
        }
    }
}
