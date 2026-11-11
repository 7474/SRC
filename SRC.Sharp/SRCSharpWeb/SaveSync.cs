using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpWeb
{
    /// <summary>
    /// ゲームがシナリオのフォルダに書いたセーブデータ（.srcs・.srcq。取り込んだ元の SRC のセーブデータ .src も）と設定ファイル（Src.ini・srcs.json）を受け取る。
    /// 受け取ったものはブラウザ（IndexedDB）に保存し、次にシナリオを開くときに戻す。
    /// 設定ファイルは、プレイ中に変えたシナリオの設定（SRCCore の ScenarioConfig が書き戻す）をシナリオごとに残すため
    /// （元の SRC がシナリオの Src.ini に書き込むのと同じ）。
    /// 書き込みはフォルダを走査せず、ファイルシステムの書き込みの通知（<see cref="SRCCore.Filesystem.IFileSystem.FileWritten"/>）で知る。
    /// </summary>
    public sealed class SaveSync
    {
        // .src は元の SRC のセーブデータ（取り込んで続きを遊ぶ。#1577）
        private static readonly string[] Extensions = { ".srcs", ".srcq", ".src" };
        // SRCCore.Config.SrcIni.FileName・ScenarioConfig.JsonFileName
        private static readonly string[] SettingsFiles = { "Src.ini", "srcs.json" };
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

        /// <summary>シナリオの設定ファイル（Src.ini・srcs.json）か。セーブデータの一覧には出さない</summary>
        public static bool IsSettingsFile(string name)
        {
            var file = Path.GetFileName(name.Replace('\\', '/'));
            return SettingsFiles.Any(x => string.Equals(file, x, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>ブラウザに残すファイルか</summary>
        public static bool IsSynced(string name) => IsSaveFile(name) || IsSettingsFile(name);

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
            if (!IsInRoot(full) || !IsSynced(full))
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

        /// <summary>保存しておいたセーブデータ・設定ファイルをシナリオのフォルダに戻す（設定ファイルはシナリオに同梱のものを上書きする）</summary>
        public string Restore(string name, byte[] data)
        {
            var path = Path.GetFullPath(Path.Combine(Root, name));
            if (!IsInRoot(path) || !IsSynced(path))
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
