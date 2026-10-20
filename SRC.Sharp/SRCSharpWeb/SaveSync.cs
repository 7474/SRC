using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpWeb
{
    /// <summary>
    /// ゲームがシナリオのフォルダに書いたセーブデータ（.srcs・.srcq）を受け取る。
    /// 受け取ったものはブラウザ（IndexedDB）に保存し、次にシナリオを開くときに戻す。
    /// 書き込みはフォルダを走査せず、ファイルシステムの書き込みの通知（<see cref="SRCCore.Filesystem.IFileSystem.FileWritten"/>）で知る。
    /// </summary>
    public sealed class SaveSync
    {
        private static readonly string[] Extensions = { ".srcs", ".srcq" };
        private readonly HashSet<string> written = new HashSet<string>(StringComparer.Ordinal);

        public SaveSync(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public string Root { get; }

        public static bool IsSaveFile(string name)
        {
            return Extensions.Any(x => name.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsInRoot(string fullPath)
        {
            return fullPath.StartsWith(Root.TrimEnd('/') + "/", StringComparison.Ordinal);
        }

        private string Relative(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');

        /// <summary>
        /// ファイルを書き終えたことを知らせる（どのスレッドから呼んでもよい）。
        /// シナリオのフォルダにあるセーブデータなら次の <see cref="CollectWritten"/> で返し、true を返す。
        /// </summary>
        public bool Notify(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var full = Path.GetFullPath(path);
            if (!IsInRoot(full) || !IsSaveFile(full))
            {
                return false;
            }
            lock (written)
            {
                written.Add(full);
            }
            return true;
        }

        /// <summary>まだブラウザに保存していないセーブデータがあるか</summary>
        public bool HasWritten
        {
            get
            {
                lock (written)
                {
                    return written.Count > 0;
                }
            }
        }

        /// <summary>前回から書き込まれたセーブデータ（相対パスと内容）</summary>
        public IList<(string Name, byte[] Data)> CollectWritten()
        {
            string[] files;
            lock (written)
            {
                files = written.ToArray();
                written.Clear();
            }
            var changed = new List<(string, byte[])>();
            foreach (var file in files)
            {
                try
                {
                    changed.Add((Relative(file), File.ReadAllBytes(file)));
                }
                catch (FileNotFoundException)
                {
                    // 書いた後に消された
                }
                catch (IOException)
                {
                    // 書き込み中。次の機会に読む
                    Notify(file);
                }
            }
            return changed;
        }

        /// <summary>保存しておいたセーブデータをシナリオのフォルダに戻す</summary>
        public string Restore(string name, byte[] data)
        {
            var path = Path.GetFullPath(Path.Combine(Root, name));
            if (!IsInRoot(path) || !IsSaveFile(path))
            {
                return null;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, data);
            SRCCore.Filesystem.PathResolver.Invalidate();
            return path;
        }
    }
}
