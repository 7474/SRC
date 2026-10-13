using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpWeb
{
    /// <summary>
    /// シナリオのフォルダにあるセーブデータ（.srcs・.srcq）の変化を見つける。
    /// 見つけたものはブラウザ（IndexedDB）に保存し、次にシナリオを開くときに戻す。
    /// </summary>
    public sealed class SaveSync
    {
        private static readonly string[] Extensions = { ".srcs", ".srcq" };
        private readonly Dictionary<string, (long Length, DateTime Written)> known = new Dictionary<string, (long, DateTime)>();

        public SaveSync(string root)
        {
            Root = root;
        }

        public string Root { get; }

        public static bool IsSaveFile(string name)
        {
            return Extensions.Any(x => name.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<string> Files()
        {
            return Directory.Exists(Root)
                ? Directory.GetFiles(Root, "*", SearchOption.AllDirectories).Where(IsSaveFile)
                : Enumerable.Empty<string>();
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
            return path;
        }
    }
}
