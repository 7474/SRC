using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRCCore.Filesystem
{
    public class LocalFileSystem : IFileSystem
    {
        private IList<ILocalFileSystemEntrySet> entrySets;
        private IList<ILocalFileSystemEntrySet> safeEntrySets;

        public LocalFileSystem()
        {
            entrySets = new List<ILocalFileSystemEntrySet>();
            entrySets.Add(new LocalFileSystemAbsolute());
            safeEntrySets = new List<ILocalFileSystemEntrySet>();
        }

        public string PathCombine(params string[] paths)
        {
            // 先頭の `\` は絶対パスのルートとして扱われないことを期待されている。
            return Path.Combine(paths.Select(x => Regex.Replace(x ?? "", @"^\\", "")).ToArray());
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
            if (Directory.Exists(basePath))
            {
                AddEntrySet(new LocalFileSystemPath(
                    basePath
                ));
            }
            // XXX Log
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

        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions)
        {
            // TODO ILocalFileSystemEntrySet 対応、アーカイブを検索
            try
            {
                return Directory.GetFileSystemEntries(path, searchPattern)
                                .Where(x => enumerationOptions == EntryOption.All
                                    || enumerationOptions == EntryOption.File && !Directory.Exists(x)
                                    || enumerationOptions == EntryOption.File && Directory.Exists(x));
            }
            catch (DirectoryNotFoundException)
            {
                return new string[] { };
            }
            catch (Exception)
            {
                return new string[] { };
            }
        }

        public void AddSafePath(string basePath)
        {
            if (Directory.Exists(basePath))
            {
                safeEntrySets.Add(new LocalFileSystemPath(
                    basePath
                ));
            }
        }

        public Stream OpenSafe(SafeOpenMode mode, params string[] paths)
        {
            var path = PathCombine(paths);
            var entry = safeEntrySets.FirstOrDefault(x => x.Exists(path));
            if (entry == null)
            {
                if (mode == SafeOpenMode.Read)
                {
                    return null;
                }
                entry = safeEntrySets.First();
            }
            switch (mode)
            {
                case SafeOpenMode.Read: return entry.OpenRead(path);
                case SafeOpenMode.Write: return entry.OpenWrite(path);
                case SafeOpenMode.Append: return entry.OpenAppend(path);
                default: throw new NotSupportedException();
            }
        }

        public bool MkDir(params string[] paths)
        {
            var path = PathCombine(paths);
            var target = safeEntrySets.FirstOrDefault(x => x.MatchRoot(path));
            if (target != null)
            {
                target.MkDir(path);
            }
            return false;
        }

        public bool RelativePathEuqals(string scenarioPath, string a, string b)
        {
            return ToRelativePath(scenarioPath, a).ToLower() == ToRelativePath(scenarioPath, b).ToLower();
        }

        public bool IsAbsolutePath(string path)
        {
            return Path.IsPathRooted(path);
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
        bool MatchRoot(string entryName);
        bool Exists(string entryName);
        Stream OpenRead(string entryName);
        Stream OpenWrite(string entryName);
        Stream OpenAppend(string entryName);
        bool MkDir(string entryName);
    }

    public class LocalFileSystemAbsolute : ILocalFileSystemEntrySet
    {
        public LocalFileSystemAbsolute()
        {
        }

        public bool MatchRoot(string entryName)
        {
            return Path.IsPathRooted(entryName);
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

        public Stream OpenWrite(string entryName)
        {
            throw new NotSupportedException();
        }

        public Stream OpenAppend(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool MkDir(string entryName)
        {
            throw new NotSupportedException();
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

        public bool MatchRoot(string entryName)
        {
            return entryName.Replace("\\", "/").StartsWith(BasePath);
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

        public Stream OpenWrite(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.OpenWrite();
        }

        public Stream OpenAppend(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.Exists ?? false ? info.Open(FileMode.Append) : info?.OpenWrite();
        }

        public bool MkDir(string entryName)
        {
            var info = GetFileInfo(entryName);
            Directory.CreateDirectory(info.FullName);
            return true;
        }
    }

    public class LocalFileSystemArchive : ILocalFileSystemEntrySet
    {
        public string BasePath { get; private set; }
        private ZipArchive _archive;
        private SrcCollection<ZipArchiveEntry> _entryMap;
        private Task _entryMapResolveTask;

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

            _entryMapResolveTask = Task.Run(() =>
            {
                foreach (var entry in _archive.Entries.Where(x => !string.IsNullOrEmpty(x.Name)))
                {
                    _entryMap[entry.FullName] = entry;
                }
            });
        }
        public bool MatchRoot(string entryName)
        {
            return entryName.Replace("\\", "/").StartsWith(BasePath);
        }

        private ZipArchiveEntry GetEntry(string entryName)
        {
            while (!_entryMapResolveTask.IsCompleted)
            {
                Task.Delay(100).Wait();
            }
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

        public Stream OpenWrite(string entryName)
        {
            throw new NotSupportedException();
        }

        public Stream OpenAppend(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool MkDir(string entryName)
        {
            throw new NotSupportedException();
        }
    }
}
