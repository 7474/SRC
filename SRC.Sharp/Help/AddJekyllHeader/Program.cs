using System;
using System.IO;
using System.Linq;

namespace AddJekyllHeader
{
    class FileConverter
    {
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
            ConvertedContent = $@"---
layout: default
title: {Path.GetFileNameWithoutExtension(SourcePath)}
---
"
                + SourceContent;
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
