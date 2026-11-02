using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SRCCore.Filesystem
{
    /// <summary>
    /// 素材パック（汎用グラフィック集・開発パックなどの zip）。SRC のフォルダ（AppPath）の Packs フォルダに置いた zip を、
    /// 展開せずに SRC のフォルダに重ねて読む（SRC#Web で素材の zip を追加するのと同じ扱い）。
    /// </summary>
    public static class AssetPacks
    {
        /// <summary>素材パックを置くフォルダの名前（SRC のフォルダの中）</summary>
        public const string FolderName = "Packs";

        /// <summary>素材のフォルダにあるはずのフォルダ（元の SRC の本体・開発パックと同じ構成）</summary>
        public static readonly IReadOnlyList<string> AssetFolders = new[] { "Bitmap", "Data", "Lib", "Midi", "Sound" };

        /// <summary>素材パックのフォルダ</summary>
        public static string Directory(string appPath) => Path.Combine(appPath ?? "", FolderName);

        /// <summary>素材パックの zip（名前順）。フォルダが無ければ空</summary>
        public static IReadOnlyList<string> Find(string appPath)
        {
            var dir = Directory(appPath);
            if (!System.IO.Directory.Exists(dir))
            {
                return new string[0];
            }
            return System.IO.Directory.GetFiles(dir, "*.zip")
                .Where(x => string.Equals(Path.GetExtension(x), ".zip", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// zip の中で SRC のフォルダにあたるフォルダ。最上位に Bitmap・Data などがあれば ""（最上位）。
        /// 無く、最上位のフォルダが 1 つだけで、その中にあれば、そのフォルダ（"汎用グラフィック集/" など）。
        /// どちらでもなければ null（素材のフォルダが無い）
        /// </summary>
        public static string FindRoot(ZipArchive archive)
        {
            var names = archive.Entries.Select(x => x.FullName.Replace('\\', '/')).ToList();
            if (HasAssetFolder(names, ""))
            {
                return "";
            }
            var tops = names.Where(x => x.Contains('/')).Select(x => x.Substring(0, x.IndexOf('/') + 1)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (tops.Count == 1 && HasAssetFolder(names, tops[0]))
            {
                return tops[0];
            }
            return null;
        }

        /// <summary>FindRoot の zip のファイル版</summary>
        public static string FindRoot(string zipPath)
        {
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Read, ZipNameEncoding.Instance))
            {
                return FindRoot(archive);
            }
        }

        /// <summary>zip の中の root の直下にある素材のフォルダ</summary>
        public static IReadOnlyList<string> FoundAssetFolders(ZipArchive archive, string root)
        {
            var names = archive.Entries.Select(x => x.FullName.Replace('\\', '/')).ToList();
            return AssetFolders.Where(folder => names.Any(x => x.StartsWith((root ?? "") + folder + "/", StringComparison.OrdinalIgnoreCase))).ToList();
        }

        private static bool HasAssetFolder(IEnumerable<string> names, string root)
        {
            return names.Any(x => AssetFolders.Any(folder => x.StartsWith(root + folder + "/", StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Packs フォルダの zip を SRC のフォルダに重ねる。名前順に重ね、後のもの（名前が後ろのもの）ほど優先する。
        /// SRC のフォルダに実在するファイルは、素材パックより優先する。素材のフォルダが無い zip は重ねない。
        /// 重ねた zip のパスを返す
        /// </summary>
        public static IReadOnlyList<string> Mount(LocalFileSystem fileSystem, string appPath, Action<string, Exception> onError = null)
        {
            var mounted = new List<string>();
            foreach (var zip in Find(appPath))
            {
                try
                {
                    var root = FindRoot(zip);
                    if (root == null)
                    {
                        onError?.Invoke(zip, null);
                        continue;
                    }
                    fileSystem.AddAchive(appPath, zip, root);
                    mounted.Add(zip);
                }
                catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is UnauthorizedAccessException)
                {
                    onError?.Invoke(zip, ex);
                }
            }
            return mounted;
        }
    }
}
