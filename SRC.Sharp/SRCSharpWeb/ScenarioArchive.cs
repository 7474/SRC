using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SRCSharpWeb
{
    /// <summary>
    /// シナリオの zip をブラウザ内のファイルシステム（.NET のメモリ上のファイルシステム）へ展開する。
    /// </summary>
    public static class ScenarioArchive
    {
        public const string Root = "/srcweb";
        public static string AppPath => Root + "/app";

        static ScenarioArchive()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// zip を展開し、展開先のフォルダを返す。
        /// Windows で作った zip のファイル名は Shift_JIS のことが多いため、UTF-8 の印が無いものは Shift_JIS として読む。
        /// </summary>
        public static string Extract(string id, byte[] zip)
        {
            var dir = Path.Combine(Root, "scenarios", Sanitize(id));
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            var root = Path.GetFullPath(dir) + "/";
            using (var stream = new MemoryStream(zip))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, false, Encoding.GetEncoding(932)))
            {
                foreach (var entry in archive.Entries)
                {
                    var name = entry.FullName.Replace('\\', '/');
                    if (name.EndsWith("/"))
                    {
                        continue;
                    }
                    var path = Path.GetFullPath(Path.Combine(dir, name));
                    if (!path.StartsWith(root, StringComparison.Ordinal))
                    {
                        // 展開先の外を指すものは無視する
                        continue;
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    entry.ExtractToFile(path, true);
                }
            }
            return dir;
        }

        /// <summary>シナリオ（.eve）の一覧。浅い階層・名前順</summary>
        public static IList<string> FindEventFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.eve", SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(dir, x).Replace('\\', '/'))
                .OrderBy(x => x.Count(c => c == '/'))
                .ThenBy(x => x, StringComparer.Ordinal)
                .ToList();
        }

        private static string Sanitize(string id)
        {
            var invalid = Path.GetInvalidFileNameChars().Concat(new[] { '/', '\\' }).ToArray();
            return new string(id.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        }
    }
}
