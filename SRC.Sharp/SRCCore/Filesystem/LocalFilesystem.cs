using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCCore.Filesystem
{
    public class LocalFileSystem : IFileSystem
    {
        private IList<LocalFileSystemArchive> archives;

        public LocalFileSystem()
        {
            archives = new List<LocalFileSystemArchive>();
        }

        public string PathCombine(params string[] paths)
        {
            // 先頭の `\` は絶対パスのルートとして扱われないことを期待されている。
            return Path.Combine(paths.Select(x => Regex.Replace(x, @"^\\", "")).ToArray());
        }

        public bool FileExists(params string[] paths)
        {
            string path = PathCombine(paths);
            return GetArchiveEntry(path) != null
                || File.Exists(path);
        }

        public Stream Open(params string[] paths)
        {
            var path = PathCombine(paths);
            var archiveEntry = GetArchiveEntry(path);
            if (archiveEntry != null)
            {
                return archiveEntry.Open();
            }
            return new FileStream(path, FileMode.Open);
        }

        private ZipArchiveEntry GetArchiveEntry(string path)
        {
            // ZipArchive のパス区切り文字は / である様子
            // マルチバイト文字はUTF-8にしておけばよい
            // XXX 大文字小文字はどうなってるんだろうか？
            var archivePath = path.Replace("\\", "/");
            var isAbsolute = Path.IsPathRooted(path);
            foreach (var archive in archives)
            {
                if (isAbsolute && !archivePath.StartsWith(archive.BasePath))
                {
                    continue;
                }
                var entryPath = isAbsolute ? archivePath.Replace(archive.BasePath, "") : archivePath;
                try
                {
                    // XXX これもしかして線形検索してるんだろうか？
                    var entry = archive.Archive.GetEntry(entryPath);
                    if (entry != null)
                    {
                        return entry;
                    }
                }
                catch
                {
                    // ignore
                }
            }
            return null;
        }

        public void AddAchive(string basePath, string archivePath)
        {
            var tmpBasePath = basePath.Replace("\\", "/");
            if (!tmpBasePath.EndsWith("/"))
            {
                tmpBasePath += "/";
            }
            archives.Insert(0, new LocalFileSystemArchive
            {
                BasePath = tmpBasePath,
                Archive = ZipFile.Open(archivePath, ZipArchiveMode.Read),
            });
        }

        public bool RelativePathEuqals(string scenarioPath, string a, string b)
        {
            return ToRelativePath(scenarioPath, a).ToLower() == ToRelativePath(scenarioPath, b).ToLower();
        }

        public string ToAbsolutePath(string scenarioPath, string path)
        {
            return string.IsNullOrEmpty(scenarioPath)
                ? NormalizePath(path)
                : PathCombine(NormalizePath(scenarioPath), ToRelativePath(scenarioPath, path));
        }

        public string ToRelativePath(string scenarioPath, string path)
        {
            return NormalizePath(path).Replace(NormalizePath(scenarioPath), "");
        }

        public string NormalizePath(string path)
        {
            return (path ?? "").Replace('/', '\\');
        }
    }
    public class LocalFileSystemArchive
    {
        public string BasePath { get; set; }
        public ZipArchive Archive { get; set; }
    }
}
