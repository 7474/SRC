using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

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

        /// <summary>Shift_JIS（コードページ 932）。Windows で作った zip のファイル名や文書に使われる</summary>
        public static Encoding ShiftJis => Encoding.GetEncoding(932);

        /// <summary>
        /// zip のファイル名の文字コード（UTF-8 の印が無い名前に使う）。UTF-8 として正しく読めれば UTF-8、読めなければ Shift_JIS として読む。
        /// Windows で作った zip は Shift_JIS、macOS・Linux で作った zip は UTF-8 の名前に印を付けないことがあるため
        /// </summary>
        private sealed class ZipNameEncoding : Encoding
        {
            private static readonly Encoding Utf8 = new UTF8Encoding(false, true);

            private static Encoding Pick(byte[] bytes, int index, int count)
            {
                try
                {
                    Utf8.GetCharCount(bytes, index, count);
                    return Utf8;
                }
                catch (DecoderFallbackException)
                {
                    return ShiftJis;
                }
            }

            public override int GetByteCount(char[] chars, int index, int count) => Utf8.GetByteCount(chars, index, count);
            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => Utf8.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
            public override int GetCharCount(byte[] bytes, int index, int count) => Pick(bytes, index, count).GetCharCount(bytes, index, count);
            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => Pick(bytes, byteIndex, byteCount).GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            public override int GetMaxByteCount(int charCount) => Utf8.GetMaxByteCount(charCount);
            // UTF-8・Shift_JIS とも、1 バイトから 2 文字以上にはならない
            public override int GetMaxCharCount(int byteCount) => byteCount;
        }

        private static readonly Encoding ZipNames = new ZipNameEncoding();

        /// <summary>
        /// シナリオの zip を展開し、展開先のフォルダを返す。
        /// root を指定すると、zip の中のそのフォルダ以下だけを展開する。
        /// </summary>
        public static string Extract(string id, byte[] zip, string root = null)
        {
            using (var stream = new MemoryStream(zip, false))
            {
                return Extract(id, stream, root);
            }
        }

        /// <summary>シナリオの zip（読み込み位置を変えられるストリーム。ファイルなど）を展開し、展開先のフォルダを返す。</summary>
        public static string Extract(string id, Stream zip, string root = null, CancellationToken token = default)
        {
            var dir = Path.Combine(Root, "scenarios", Sanitize(id));
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            ExtractTo(dir, zip, root, token);
            return dir;
        }

        /// <summary>
        /// 素材（汎用グラフィック集など）の zip を SRC のフォルダ（AppPath）に重ねて展開する。
        /// 元の SRC で素材を SRC のフォルダに置くのと同じ扱いで、後から展開したものが優先される。
        /// </summary>
        public static void ExtractAsset(byte[] zip, string root = null, CancellationToken token = default)
        {
            using (var stream = new MemoryStream(zip, false))
            {
                ExtractAsset(stream, root, token);
            }
        }

        /// <inheritdoc cref="ExtractAsset(byte[], string, CancellationToken)"/>
        public static void ExtractAsset(Stream zip, string root = null, CancellationToken token = default)
        {
            ExtractTo(AppPath, zip, root, token);
        }

        /// <summary>ダウンロードした zip を一時的に置くファイルのパス（展開したら消す）</summary>
        public static string NewDownloadPath()
        {
            var dir = Path.Combine(Root, "downloads");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, Guid.NewGuid().ToString("N") + ".zip");
        }

        /// <summary>
        /// ダウンロードした zip を読むために開く。
        /// ブラウザ内のファイルシステムの読み書きは 1 回ごとに UI スレッドへの呼び出しになるため、大きいバッファでまとめて読む。
        /// </summary>
        public static FileStream OpenDownload(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024);
        }

        /// <summary>
        /// zip を dir に展開する。
        /// UTF-8 の印が無いファイル名は、UTF-8 として正しく読めれば UTF-8、読めなければ Shift_JIS として読む（ZipNameEncoding）。
        /// token が取り消されたら、途中まで展開したところで OperationCanceledException を投げる。
        /// </summary>
        private static void ExtractTo(string dir, Stream zip, string root, CancellationToken token)
        {
            Directory.CreateDirectory(dir);
            var fullDir = Path.GetFullPath(dir) + "/";
            var prefix = string.IsNullOrEmpty(root) ? "" : root.Replace('\\', '/').Trim('/') + "/";
            try
            {
                ExtractEntries(dir, fullDir, zip, prefix, token);
            }
            finally
            {
                // 展開したファイルを探せるよう、フォルダの一覧のキャッシュを捨てる（取り消して途中までのときも）
                SRCCore.Filesystem.PathResolver.Invalidate();
            }
        }

        private static void ExtractEntries(string dir, string fullDir, Stream zip, string prefix, CancellationToken token)
        {
            using (var archive = new ZipArchive(zip, ZipArchiveMode.Read, true, ZipNames))
            {
                foreach (var entry in archive.Entries)
                {
                    token.ThrowIfCancellationRequested();
                    var name = entry.FullName.Replace('\\', '/');
                    if (name.EndsWith("/"))
                    {
                        continue;
                    }
                    if (prefix.Length > 0)
                    {
                        if (!name.StartsWith(prefix, StringComparison.Ordinal))
                        {
                            continue;
                        }
                        name = name.Substring(prefix.Length);
                    }
                    var path = Path.GetFullPath(Path.Combine(dir, name));
                    if (!path.StartsWith(fullDir, StringComparison.Ordinal))
                    {
                        // 展開先の外を指すものは無視する
                        continue;
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    entry.ExtractToFile(path, true);
                }
            }
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
