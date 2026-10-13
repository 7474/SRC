using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpWeb
{
    /// <summary>
    /// シナリオのフォルダにあるセーブデータ（.srcs・.srcq）の変化を見つける。
    /// 見つけたものはブラウザ（IndexedDB）に保存し、次にシナリオを開くときに戻す。
    /// 大きいシナリオ（ロボダンは 1 万ファイル以上）のフォルダ全体を毎回たどると重いため、
    /// セーブデータが書かれ得るフォルダ（シナリオのフォルダの直下・イベントファイルのフォルダ・
    /// 復元したセーブデータのフォルダ・保存のダイアログで選んだフォルダ）の直下だけを調べる。
    /// </summary>
    public sealed class SaveSync
    {
        private static readonly string[] Extensions = { ".srcs", ".srcq" };
        private readonly Dictionary<string, (long Length, DateTime Written)> known = new Dictionary<string, (long, DateTime)>();
        private readonly HashSet<string> watched = new HashSet<string>(StringComparer.Ordinal);

        public SaveSync(string root)
        {
            Root = root;
            watched.Add(Path.GetFullPath(root));
        }

        public string Root { get; }

        public static bool IsSaveFile(string name)
        {
            return Extensions.Any(x => name.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<string> Files()
        {
            string[] dirs;
            lock (watched)
            {
                dirs = watched.ToArray();
            }
            return dirs.Where(Directory.Exists)
                .SelectMany(dir => Extensions.SelectMany(ext => Directory.GetFiles(dir, "*" + ext, SearchOption.TopDirectoryOnly)))
                .Distinct(StringComparer.Ordinal);
        }

        /// <summary>
        /// セーブデータが書かれ得るフォルダを調べる対象に加える（どのスレッドから呼んでもよい）。
        /// そのフォルダに既にあるセーブデータは、次に調べたときに一度だけブラウザに保存される。
        /// </summary>
        public void Watch(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }
            var full = Path.GetFullPath(directory);
            if (!full.StartsWith(Path.GetFullPath(Root), StringComparison.Ordinal))
            {
                return;
            }
            lock (watched)
            {
                watched.Add(full);
            }
        }

        private string Relative(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');

        /// <summary>今あるファイルを保存済みとして扱う（展開・復元の直後に呼ぶ）</summary>
        public void MarkAll()
        {
            foreach (var file in Files())
            {
                var info = new FileInfo(file);
                known[Relative(file)] = (info.Length, info.LastWriteTimeUtc);
            }
        }

        /// <summary>前回から書き込まれたセーブデータ（相対パスと内容）</summary>
        public IList<(string Name, byte[] Data)> CollectChanged()
        {
            var changed = new List<(string, byte[])>();
            foreach (var file in Files())
            {
                var info = new FileInfo(file);
                var name = Relative(file);
                if (known.TryGetValue(name, out var k) && k.Length == info.Length && k.Written == info.LastWriteTimeUtc)
                {
                    continue;
                }
                try
                {
                    var data = File.ReadAllBytes(file);
                    known[name] = (info.Length, info.LastWriteTimeUtc);
                    changed.Add((name, data));
                }
                catch (IOException)
                {
                    // 書き込み中。次の機会に読む
                }
            }
            return changed;
        }

        /// <summary>保存しておいたセーブデータをシナリオのフォルダに戻す</summary>
        public string Restore(string name, byte[] data)
        {
            var path = Path.GetFullPath(Path.Combine(Root, name));
            if (!path.StartsWith(Path.GetFullPath(Root) + "/", StringComparison.Ordinal) || !IsSaveFile(path))
            {
                return null;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, data);
            lock (watched)
            {
                watched.Add(Path.GetDirectoryName(path));
            }
            return path;
        }
    }
}
