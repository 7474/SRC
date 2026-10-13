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
        private static readonly ConcurrentDictionary<string, IDictionary<string, string>> directoryEntries
            = new ConcurrentDictionary<string, IDictionary<string, string>>();

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
                var candidate = Path.Combine(current, component);
                if (File.Exists(candidate) || Directory.Exists(candidate))
                {
                    current = candidate;
                    continue;
                }
                var actual = FindEntry(current, component);
                if (actual == null)
                {
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

        private static string FindEntry(string directory, string name)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }
            var key = name.ToLowerInvariant();
            if (directoryEntries.TryGetValue(directory, out var entries) && entries.TryGetValue(key, out var cached))
            {
                return cached;
            }
            // 見つからない場合は一覧を作り直す（その後に作られたファイルに対応するため）
            try
            {
                entries = Directory.EnumerateFileSystemEntries(directory)
                    .Select(Path.GetFileName)
                    .GroupBy(x => x.ToLowerInvariant())
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y, StringComparer.Ordinal).First());
            }
            catch (Exception)
            {
                return null;
            }
            directoryEntries[directory] = entries;
            return entries.TryGetValue(key, out var actual) ? actual : null;
        }
    }
}
