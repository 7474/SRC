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
using System.Text.RegularExpressions;

namespace SRCSharpWeb
{
    /// <summary>
    /// シナリオに同梱された文書（.txt・.md・.htm・.html。はじめに読む文章や更新履歴など）。
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
            public bool IsHtml => Path.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || Path.EndsWith(".html", StringComparison.OrdinalIgnoreCase);
            /// <summary>ブラウザ風（往年の Internet Explorer）に表示するもの。.md・.htm・.html</summary>
            public bool IsPage => IsMarkdown || IsHtml;
            /// <summary>README.txt・README.md・README.htm(l)（大文字小文字は問わない）</summary>
            public bool IsReadme => System.IO.Path.GetFileNameWithoutExtension(Path).Equals("README", StringComparison.OrdinalIgnoreCase);
        }

        private static readonly HashSet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".htm", ".html",
        };

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
                .Where(x => Extensions.Contains(System.IO.Path.GetExtension(x)))
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
                    var target = ResolveLink(dir, document, link.Url);
                    if (target != null)
                    {
                        // シナリオの中の別の文書は、ページの中で開く（srcweb.js の文書のリンク）
                        link.Url = "#";
                        link.GetAttributes().AddPropertyIfNotExist(LinkAttribute, target);
                    }
                    else
                    {
                        // javascript: や文書でないファイルなど、開けない先へのリンクは文字だけにする
                        Unwrap(link);
                    }
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

        /// <summary>
        /// 日本語の Internet Explorer 4 の既定の表示に近づけるスタイル（app.css の .doc-page と同じ）。文書のスタイルより先に置き、文書が決めたものはそちらを使う。
        /// Web ページのフォントは ＭＳ Ｐゴシック、文字のサイズは「中」（12pt = 16px）、余白は上下 15px・左右 10px、リンクは #0000ff・訪問済みは #800080。
        /// 見出しは既定の大きさ（h1 が 2em）だと大き過ぎるため、本文から少しずつ大きくする程度にとどめる
        /// </summary>
        private const string PageStyle = "<base target=\"_blank\"><style>"
            + "body{margin:15px 10px;color:#000;background:#fff;font-family:\"MS PGothic\",\"ＭＳ Ｐゴシック\",\"Hiragino Kaku Gothic ProN\",\"Noto Sans CJK JP\",sans-serif;font-size:16px;line-height:1.2}"
            + "h1{font-size:1.4em}h2{font-size:1.25em}h3{font-size:1.12em}h4,h5,h6{font-size:1em}"
            + "a:link{color:#0000ff}a:visited{color:#800080}img{max-width:100%}"
            + "code,pre,tt,kbd,samp{font-family:\"MS Gothic\",\"ＭＳ ゴシック\",\"Osaka-Mono\",\"Noto Sans Mono CJK JP\",monospace;font-size:13px}"
            + "</style>";

        /// <summary>シナリオの中の別の文書へのリンクに付ける属性。値はシナリオのフォルダからの相対パス</summary>
        public const string LinkAttribute = "data-srcweb-doc";

        private static readonly Regex AnchorHref = new Regex(@"(<a\b[^>]*?\bhref\s*=\s*)(""([^""]*)""|'([^']*)'|([^\s>]+))", RegexOptions.IgnoreCase);

        /// <summary>
        /// 文書の中の相対リンクの先にある文書（シナリオのフォルダからの相対パス）。文書でないもの・無いもの・外を指すものは null。
        /// ? と # より後は捨てる
        /// </summary>
        public static string ResolveLink(string dir, Document from, string href)
        {
            if (string.IsNullOrEmpty(href) || Uri.TryCreate(href, UriKind.Absolute, out _))
            {
                return null;
            }
            var path = href.Split('#', '?')[0];
            if (path.Length == 0)
            {
                return null;
            }
            try
            {
                path = Uri.UnescapeDataString(path).Replace('\\', '/');
                var baseDir = from == null ? "" : System.IO.Path.GetDirectoryName(from.Path) ?? "";
                var full = Resolve(dir, path.StartsWith("/", StringComparison.Ordinal) ? path.TrimStart('/') : System.IO.Path.Combine(baseDir, path));
                if (!File.Exists(full) || !Extensions.Contains(System.IO.Path.GetExtension(full)))
                {
                    return null;
                }
                return System.IO.Path.GetRelativePath(dir, full).Replace('\\', '/');
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>アドレス欄に入れたパス（シナリオのフォルダからの相対パス）の文書。無ければ null</summary>
        public static Document Open(string dir, string path)
        {
            var target = ResolveLink(dir, null, path);
            return target == null ? null : new Document { Path = target };
        }

        private static readonly Regex TitleTag = new Regex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex HeadTag = new Regex(@"<head(\s[^>]*)?>", RegexOptions.IgnoreCase);
        private static readonly Regex ImageSource = new Regex(@"(<img\b[^>]*?\bsrc\s*=\s*)(""([^""]*)""|'([^']*)'|([^\s>]+))", RegexOptions.IgnoreCase);

        /// <summary>
        /// HTML の文書を、サンドボックスの iframe（srcdoc。スクリプトは動かさない）に入れる文字列にする。
        /// 既定のスタイルを先頭に入れ、シナリオの中の画像（相対パス）は data URI にする。リンクは別のタブで開く。
        /// </summary>
        /// <param name="title">HTML の title（タイトルバーに出す）。無ければ null</param>
        public static string RenderHtml(string dir, Document document, out string title)
        {
            var html = ReadText(dir, document);
            var titleMatch = TitleTag.Match(html);
            title = titleMatch.Success ? System.Net.WebUtility.HtmlDecode(Regex.Replace(titleMatch.Groups[1].Value, @"\s+", " ").Trim()) : null;
            if (string.IsNullOrEmpty(title))
            {
                title = null;
            }
            var baseDir = System.IO.Path.GetDirectoryName(document.Path.Replace('/', System.IO.Path.DirectorySeparatorChar)) ?? "";
            html = ImageSource.Replace(html, m =>
            {
                var url = System.Net.WebUtility.HtmlDecode(m.Groups[3].Success ? m.Groups[3].Value : m.Groups[4].Success ? m.Groups[4].Value : m.Groups[5].Value);
                return m.Groups[1].Value + "\"" + System.Net.WebUtility.HtmlEncode(ImageUrl(dir, baseDir, url)) + "\"";
            });
            html = AnchorHref.Replace(html, m =>
            {
                var url = System.Net.WebUtility.HtmlDecode(m.Groups[3].Success ? m.Groups[3].Value : m.Groups[4].Success ? m.Groups[4].Value : m.Groups[5].Value);
                if (IsExternal(url))
                {
                    return m.Value;
                }
                if (url.StartsWith("#", StringComparison.Ordinal))
                {
                    // ページの中の移動は同じ枠で（base の target="_blank" で別のタブにしない）
                    return m.Groups[1].Value.Insert(2, " target=\"_self\"") + m.Groups[2].Value;
                }
                var target = ResolveLink(dir, document, url);
                // シナリオの中の別の文書は、ページの中で開く（srcweb.js の文書のリンク）。開けない先は何もしない
                return m.Groups[1].Value.Insert(2, target == null ? " target=\"_self\"" : $" {LinkAttribute}=\"{System.Net.WebUtility.HtmlEncode(target)}\" target=\"_self\"") + "\"#\"";
            });
            var head = HeadTag.Match(html);
            return head.Success
                ? html.Insert(head.Index + head.Length, PageStyle)
                : PageStyle + html;
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
