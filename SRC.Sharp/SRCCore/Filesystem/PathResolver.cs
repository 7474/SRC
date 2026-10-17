using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Filesystem
{
    /// <summary>
    /// Windows 前提で書かれたパス（\ 区切り、大文字・小文字の違い）を、実行環境のパスとして解決する。
    /// Windows ではそのまま扱い、それ以外では \ を / に置き換え、存在しない場合は大文字・小文字を区別せずに探す。
    /// </summary>
    public static class PathResolver
    {
        private static readonly bool isBackslashSeparator = Path.DirectorySeparatorChar == '\\';

        // ディレクトリごとの「小文字の名前 → 実際の名前」
        private static readonly ConcurrentDictionary<string, DirectoryEntries> directoryEntries
            = new ConcurrentDictionary<string, DirectoryEntries>();

        /// <summary>
        /// 大文字・小文字を区別せずにファイル・フォルダを列挙するオプション。
        /// </summary>
        public static readonly EnumerationOptions CaseInsensitiveEnumeration = new EnumerationOptions
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            MatchType = MatchType.Win32,
            AttributesToSkip = 0,
            IgnoreInaccessible = false,
        };

        /// <summary>
        /// パス区切りを実行環境のものにする。
        /// </summary>
        public static string ToNative(string path)
        {
            if (path == null || isBackslashSeparator)
            {
                return path;
            }
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 実在するパスへ解決する。見つからない部分はそのまま残す。
        /// </summary>
        public static string Resolve(string path)
        {
            var native = ToNative(path);
            if (string.IsNullOrEmpty(native) || isBackslashSeparator)
            {
                return native;
            }
            if (File.Exists(native) || Directory.Exists(native))
            {
                return native;
            }

            // 大文字・小文字の違うパスや、無いファイルは、フォルダの一覧（キャッシュ）でたどる。
            // ブラウザ（マルチスレッドの WebAssembly）ではファイルシステムの操作が 1 回ごとに UI スレッドへの呼び出しになり、
            // 素材を探すたびに各階層の存在の確認と一覧を作り直していたのが、シナリオの読み込みの時間の半分ほどを占めていた
            var root = Path.GetPathRoot(native);
            var rest = native.Substring(root.Length);
            var components = rest.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var current = string.IsNullOrEmpty(root) ? "." : root;
            for (var i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (component == "." || component == "..")
                {
                    current = Path.Combine(current, component);
                    continue;
                }
                var actual = FindEntry(current, component);
                if (actual == null)
                {
                    // 一覧を作った後に作られたもの（同じ名前）は、一覧によらず確かめる
                    var candidate = Path.Combine(current, component);
                    if (File.Exists(candidate) || Directory.Exists(candidate))
                    {
                        directoryEntries.TryRemove(current, out _);
                        current = candidate;
                        continue;
                    }
                    // 以降は存在しないので、残りをそのまま連結する
                    current = Path.Combine(new[] { current }.Concat(components.Skip(i)).ToArray());
                    break;
                }
                current = Path.Combine(current, actual);
            }
            if (string.IsNullOrEmpty(root) && current.StartsWith("./"))
            {
                current = current.Substring(2);
            }
            return current;
        }

        /// <summary>
        /// フォルダの一覧のキャッシュを捨てる。ファイル・フォルダを作った・消した後に呼ぶ（LocalFileSystem・ブラウザでの展開など）。
        /// 呼ばなくても、見つからなかった名前は <see cref="CacheLifetime"/> を過ぎると一覧を作り直して探す
        /// </summary>
        public static void Invalidate()
        {
            directoryEntries.Clear();
        }

        /// <summary>見つからなかった名前で、フォルダの一覧を作り直すまでの時間</summary>
        public static TimeSpan CacheLifetime { get; set; } = TimeSpan.FromSeconds(3);

        private sealed class DirectoryEntries
        {
            public DirectoryEntries(IEnumerable<string> names)
            {
                CreatedAt = DateTime.UtcNow;
                if (names == null)
                {
                    return;
                }
                Names = new HashSet<string>(names, StringComparer.Ordinal);
                ByLowerName = Names
                    .GroupBy(x => x.ToLowerInvariant())
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y, StringComparer.Ordinal).First());
            }

            public DateTime CreatedAt { get; }
            /// <summary>フォルダが無ければ null</summary>
            public HashSet<string> Names { get; }
            public Dictionary<string, string> ByLowerName { get; }
            public bool IsExpired => DateTime.UtcNow - CreatedAt > CacheLifetime;

            public string Find(string name)
            {
                if (Names == null)
                {
                    return null;
                }
                // 大文字・小文字だけが違う名前が複数あれば、同じ名前のものを優先する
                if (Names.Contains(name))
                {
                    return name;
                }
                return ByLowerName.TryGetValue(name.ToLowerInvariant(), out var actual) ? actual : null;
            }
        }

        private static string FindEntry(string directory, string name)
        {
            if (directoryEntries.TryGetValue(directory, out var entries))
            {
                var cached = entries.Find(name);
                if (cached != null || !entries.IsExpired)
                {
                    return cached;
                }
            }
            // 見つからず、一覧が古い場合は作り直す（その後に作られたファイルに対応するため）
            entries = new DirectoryEntries(Enumerate(directory));
            directoryEntries[directory] = entries;
            return entries.Find(name);
        }

        private static IEnumerable<string> Enumerate(string directory)
        {
            try
            {
                return Directory.Exists(directory)
                    ? Directory.EnumerateFileSystemEntries(directory).Select(Path.GetFileName).ToList()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
