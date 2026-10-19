using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCSharpWeb
{
    /// <summary>
    /// シナリオに同梱された文書（.txt・.md。はじめに読む文章や更新履歴など）。
    /// シナリオのデータ（unit.txt など）は除く。
    /// </summary>
    public static class ScenarioDocuments
    {
        public class Document
        {
            /// <summary>シナリオのフォルダからの相対パス（区切りは /）</summary>
            public string Path { get; set; }
            public string Name => System.IO.Path.GetFileName(Path);
            public bool IsMarkdown => Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase);
            /// <summary>README.txt・README.md（大文字小文字は問わない）</summary>
            public bool IsReadme => System.IO.Path.GetFileNameWithoutExtension(Path).Equals("README", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>データを置くフォルダ。この下の .txt はデータとして扱い、文書に含めない</summary>
        private static readonly HashSet<string> DataFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Data", "Lib", "Bitmap", "Sound", "Midi",
        };

        /// <summary>SRC が読むデータのファイル名（Data の外に置かれていても文書に含めない）</summary>
        private static readonly HashSet<string> DataFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "unit.txt", "robot.txt", "pilot.txt", "non_pilot.txt", "pilot_message.txt", "pilot_dialog.txt",
            "effect.txt", "animation.txt", "ext_animation.txt", "item.txt", "sp.txt", "mind.txt", "alias.txt",
            "terrain.txt", "battle.txt",
        };

        /// <summary>文書を見つけやすい順（README、浅い階層、名前順）に並べる</summary>
        public static IList<Document> Find(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                return new List<Document>();
            }
            return Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                .Where(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                .Select(x => System.IO.Path.GetRelativePath(dir, x).Replace('\\', '/'))
                .Where(x => !x.Split('/').SkipLast(1).Any(DataFolders.Contains) && !DataFiles.Contains(System.IO.Path.GetFileName(x)))
                .Select(x => new Document { Path = x })
                .OrderBy(x => x.IsReadme ? 0 : 1)
                .ThenBy(x => x.Path.Count(c => c == '/'))
                .ThenBy(x => x.Path, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// 文字コードを判定して読む。BOM があればそれに従い、無ければ UTF-8 として正しく読めるものは UTF-8、
        /// そうでなければ Shift_JIS（Windows で書かれた古いシナリオの文書に多い）として読む。
        /// </summary>
        public static string Decode(byte[] data)
        {
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
            {
                return Encoding.UTF8.GetString(data, 3, data.Length - 3);
            }
            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
            {
                return Encoding.Unicode.GetString(data, 2, data.Length - 2);
            }
            if (data.Length >= 2 && data[0] == 0xFE && data[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode.GetString(data, 2, data.Length - 2);
            }
            try
            {
                return new UTF8Encoding(false, true).GetString(data);
            }
            catch (DecoderFallbackException)
            {
                // ScenarioArchive の静的コンストラクタで CodePagesEncodingProvider を登録している
                return ScenarioArchive.ShiftJis.GetString(data);
            }
        }

        public static string ReadText(string dir, Document document)
        {
            return Decode(File.ReadAllBytes(Resolve(dir, document.Path)));
        }

        private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            // 文書の中の HTML はそのまま出さない（スクリプトを動かさない）
            .DisableHtml()
            .Build();

        /// <summary>画像として埋め込む大きさの上限</summary>
        private const long MaxImageSize = 4 * 1024 * 1024;

        /// <summary>
        /// Markdown を HTML にする。HTML の要素は文字として出し、リンクは http・https・mailto だけを残して別のタブで開く。
        /// シナリオの中の画像（相対パス）は data URI にして表示する。
        /// </summary>
        public static string RenderMarkdown(string dir, Document document)
        {
            var markdown = Markdown.Parse(ReadText(dir, document), Pipeline);
            var baseDir = System.IO.Path.GetDirectoryName(document.Path.Replace('/', System.IO.Path.DirectorySeparatorChar)) ?? "";
            foreach (var link in markdown.Descendants<LinkInline>().ToList())
            {
                if (link.IsImage)
                {
                    link.Url = ImageUrl(dir, baseDir, link.Url);
                }
                else if (IsExternal(link.Url))
                {
                    var attributes = link.GetAttributes();
                    attributes.AddPropertyIfNotExist("target", "_blank");
                    attributes.AddPropertyIfNotExist("rel", "noopener noreferrer");
                }
                else if (!(link.Url ?? "").StartsWith("#", StringComparison.Ordinal))
                {
                    // javascript: やシナリオの中のファイルなど、開けない先へのリンクは文字だけにする
                    Unwrap(link);
                }
            }
            foreach (var link in markdown.Descendants<AutolinkInline>().ToList())
            {
                if (!IsExternal(link.IsEmail ? "mailto:" + link.Url : link.Url))
                {
                    link.ReplaceBy(new LiteralInline(link.Url));
                }
            }
            using (var writer = new StringWriter())
            {
                var renderer = new HtmlRenderer(writer);
                Pipeline.Setup(renderer);
                renderer.Render(markdown);
                writer.Flush();
                return writer.ToString();
            }
        }

        /// <summary>リンクを外し、中の文字だけを残す</summary>
        private static void Unwrap(LinkInline link)
        {
            var child = link.FirstChild;
            while (child != null)
            {
                var next = child.NextSibling;
                child.Remove();
                link.InsertBefore(child);
                child = next;
            }
            link.Remove();
        }

        private static bool IsExternal(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeMailto);
        }

        private static string ImageUrl(string dir, string baseDir, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp ? url : "";
            }
            try
            {
                var path = Resolve(dir, System.IO.Path.Combine(baseDir, Uri.UnescapeDataString(url)));
                var info = new FileInfo(path);
                if (!info.Exists || info.Length > MaxImageSize)
                {
                    return "";
                }
                var type = System.IO.Path.GetExtension(path).ToLowerInvariant() switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    _ => null,
                };
                return type == null ? "" : $"data:{type};base64,{Convert.ToBase64String(File.ReadAllBytes(path))}";
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                return "";
            }
        }

        /// <summary>シナリオのフォルダの中のパス。外を指すものは例外にする</summary>
        private static string Resolve(string dir, string relative)
        {
            var root = System.IO.Path.GetFullPath(dir).TrimEnd('/') + "/";
            var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(dir, relative.Replace('\\', '/')));
            if (!path.StartsWith(root, StringComparison.Ordinal))
            {
                throw new ArgumentException("シナリオのフォルダの外を指しています: " + relative);
            }
            return path;
        }
    }
}
