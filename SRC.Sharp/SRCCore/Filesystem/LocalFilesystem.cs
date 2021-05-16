using SRCCore.VB;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCCore.Filesystem
{
    public class LocalFileSystem : IFileSystem
    {
        private IList<ILocalFileSystemEntrySet> entrySets;

        public LocalFileSystem()
        {
            entrySets = new List<ILocalFileSystemEntrySet>();
            entrySets.Add(new LocalFileSystemAbsolute());
        }

        public string PathCombine(params string[] paths)
        {
            // 先頭の `\` は絶対パスのルートとして扱われないことを期待されている。
            return Path.Combine(paths.Select(x => Regex.Replace(x, @"^\\", "")).ToArray());
        }

        public bool FileExists(params string[] paths)
        {
            string path = PathCombine(paths);
            return entrySets.Any(x => x.Exists(path));
        }

        public Stream Open(params string[] paths)
        {
            var path = PathCombine(paths);
            return entrySets.FirstOrDefault(x => x.Exists(path))?.OpenRead(path);
        }

        public void AddPath(string basePath)
        {
            AddEntrySet(new LocalFileSystemPath(
                basePath
            ));
        }

        public void AddAchive(string basePath, string archivePath)
        {
            AddEntrySet(new LocalFileSystemArchive(
                basePath, ZipFile.Open(archivePath, ZipArchiveMode.Read)
            ));
        }

        private void AddEntrySet(ILocalFileSystemEntrySet entrySet)
        {
            // 後に追加したものは絶対パスの次に走査する
            entrySets.Insert(1, entrySet);
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

    public interface ILocalFileSystemEntrySet
    {
        bool Exists(string entryName);
        Stream OpenRead(string entryName);
    }

    public class LocalFileSystemAbsolute : ILocalFileSystemEntrySet
    {
        public LocalFileSystemAbsolute()
        {
        }

        private FileInfo GetFileInfo(string entryName)
        {
            return Path.IsPathRooted(entryName) ? new FileInfo(entryName) : null;
        }

        public bool Exists(string entryName)
        {
            return GetFileInfo(entryName)?.Exists ?? false;
        }

        public Stream OpenRead(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.Exists ?? false ? info.OpenRead() : null;
        }
    }

    public class LocalFileSystemPath : ILocalFileSystemEntrySet
    {
        public string BasePath { get; private set; }
        public LocalFileSystemPath(string basePath)
        {
            var tmpBasePath = basePath.Replace("\\", "/");
            if (!tmpBasePath.EndsWith("/"))
            {
                tmpBasePath += "/";
            }
            BasePath = tmpBasePath;
        }

        private FileInfo GetFileInfo(string entryName)
        {
            var archivePath = entryName.Replace("\\", "/");
            var isAbsolute = Path.IsPathRooted(entryName);
            if (isAbsolute && !archivePath.StartsWith(BasePath))
            {
                return null;
            }
            var entryPath = isAbsolute ? archivePath.Replace(BasePath, "") : archivePath;

            return new FileInfo(Path.Combine(BasePath, entryPath));
        }

        public bool Exists(string entryName)
        {
            return GetFileInfo(entryName)?.Exists ?? false;
        }

        public Stream OpenRead(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.Exists ?? false ? info.OpenRead() : null;
        }
    }

    public class LocalFileSystemArchive : ILocalFileSystemEntrySet
    {
        public string BasePath { get; private set; }
        private ZipArchive _archive;
        private SrcCollection<ZipArchiveEntry> _entryMap;

        public LocalFileSystemArchive(string basePath, ZipArchive archive)
        {
            var tmpBasePath = basePath.Replace("\\", "/");
            if (!tmpBasePath.EndsWith("/"))
            {
                tmpBasePath += "/";
            }
            BasePath = tmpBasePath;
            _archive = archive;
            _entryMap = new SrcCollection<ZipArchiveEntry>();

            foreach (var entry in _archive.Entries.Where(x => !string.IsNullOrEmpty(x.Name)))
            {
                _entryMap[entry.FullName] = entry;
            }
        }

        private ZipArchiveEntry GetEntry(string entryName)
        {
            var archivePath = entryName.Replace("\\", "/");
            var isAbsolute = Path.IsPathRooted(entryName);
            if (isAbsolute && !archivePath.StartsWith(BasePath))
            {
                return null;
            }
            var entryPath = isAbsolute ? archivePath.Replace(BasePath, "") : archivePath;

            var entry = _archive.GetEntry(entryPath);
            if (entry == null)
            {
                entry = _entryMap[entryPath];
            }
            return entry;
        }

        public bool Exists(string entryName)
        {
            return GetEntry(entryName) != null;
        }

        public Stream OpenRead(string entryName)
        {
            return GetEntry(entryName).Open();
        }
    }
}
