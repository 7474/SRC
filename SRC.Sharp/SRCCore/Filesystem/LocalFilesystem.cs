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
    public class LocalFileSystem : IFileSystem, IDisposable
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
            return Path.Combine(paths.Select(x => PathResolver.ToNative(Regex.Replace(x ?? "", @"^\\", ""))).ToArray());
        }

        public bool PathEquals(string a, string b)
        {
            // XXX NormalizePath で先頭 `\` を処理する？
            return NormalizePath(PathCombine(a)).ToLower() == NormalizePath(PathCombine(b)).ToLower();
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

        /// <summary>重ねた zip アーカイブを閉じる（Windows では開いている間、zip を削除・置き換えできない）</summary>
        public void Dispose()
        {
            foreach (var entrySet in entrySets.OfType<IDisposable>())
            {
                entrySet.Dispose();
            }
        }

        public void AddAchive(string basePath, string archivePath)
        {
            AddAchive(basePath, archivePath, null);
        }

        /// <summary>
        /// zip アーカイブを basePath のフォルダに重ねる。root を指定すると、zip の中のそのフォルダ以下を重ねる。
        /// 後に重ねたものほど優先する（実在するファイルは常に優先する）
        /// </summary>
        public void AddAchive(string basePath, string archivePath, string root)
        {
            AddEntrySet(new LocalFileSystemArchive(
                basePath, ZipFile.Open(archivePath, ZipArchiveMode.Read, ZipNameEncoding.Instance), root
            ));
        }

        private void AddEntrySet(ILocalFileSystemEntrySet entrySet)
        {
            // 後に追加したものは絶対パスの次に走査する
            entrySets.Insert(1, entrySet);
        }

        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions)
        {
            foreach (var entrySet in entrySets)
            {
                var entries = entrySet.GetFileSystemEntries(path, searchPattern, enumerationOptions);
                if (entries.Any())
                {
                    return entries;
                }
            }

            return new string[] { };
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
                case SafeOpenMode.Write: return Changed(NotifyOnClose(entry.OpenWrite(path), path));
                case SafeOpenMode.Append: return Changed(NotifyOnClose(entry.OpenAppend(path), path));
                default: throw new NotSupportedException();
            }
        }

        /// <summary>作った・消したファイルやフォルダを探せるよう、フォルダの一覧のキャッシュを捨てる</summary>
        private static T Changed<T>(T result)
        {
            PathResolver.Invalidate();
            return result;
        }

        public event Action<string> FileWritten;

        public Stream Create(string path)
        {
            return Changed(NotifyOnClose(new FileStream(path, FileMode.Create), path));
        }

        private Stream NotifyOnClose(Stream stream, string path)
        {
            if (stream == null)
            {
                return null;
            }
            var fullPath = stream is FileStream fileStream ? fileStream.Name : Path.GetFullPath(path);
            return new WriteNotifyStream(stream, fullPath, x => FileWritten?.Invoke(x));
        }

        public bool Remove(params string[] paths)
        {
            var path = PathCombine(paths);
            var entry = safeEntrySets.FirstOrDefault(x => x.Exists(path));
            if (entry != null)
            {
                return Changed(entry.Remove(path));
            }
            return false;
        }

        public bool MkDir(params string[] paths)
        {
            var path = PathCombine(paths);
            var target = safeEntrySets.FirstOrDefault(x => x.MatchRoot(path));
            if (target != null)
            {
                return Changed(target.MkDir(path));
            }
            return false;
        }

        public bool RmDir(params string[] paths)
        {
            var path = PathCombine(paths);
            var target = safeEntrySets.FirstOrDefault(x => x.MatchRoot(path));
            if (target != null)
            {
                return Changed(target.RmDir(path));
            }
            return false;
        }

        public bool RelativePathEuqals(string scenarioPath, string a, string b)
        {
            return ToRelativePath(scenarioPath, a).ToLower() == ToRelativePath(scenarioPath, b).ToLower();
        }

        public bool IsAbsolutePath(string path)
        {
            return Path.IsPathRooted(PathResolver.ToNative(path));
        }

        public string ToAbsolutePath(string scenarioPath, string path)
        {
            // scenarioPath は正規化しない。Linux・ブラウザの絶対パス（/ から始まる）を \ に変えると、
            // PathCombine が先頭の \ を外して相対パスにしてしまう
            return string.IsNullOrEmpty(scenarioPath)
                ? NormalizePath(path)
                : PathCombine(scenarioPath, ToRelativePath(scenarioPath, path));
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
        IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions);

        Stream OpenRead(string entryName);
        Stream OpenWrite(string entryName);
        Stream OpenAppend(string entryName);
        bool Remove(string entryName);
        bool MkDir(string entryName);
        bool RmDir(string entryName);
    }

    public class LocalFileSystemAbsolute : ILocalFileSystemEntrySet
    {
        public LocalFileSystemAbsolute()
        {
        }

        public bool MatchRoot(string entryName)
        {
            return Path.IsPathRooted(PathResolver.ToNative(entryName));
        }

        private FileInfo GetFileInfo(string entryName)
        {
            return MatchRoot(entryName) ? new FileInfo(PathResolver.Resolve(entryName)) : null;
        }

        public bool Exists(string entryName)
        {
            return GetFileInfo(entryName)?.Exists ?? false;
        }

        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions)
        {
            if (!MatchRoot(path))
            {
                return new string[] { };
            }
            try
            {
                return Directory.GetFileSystemEntries(PathResolver.Resolve(path), searchPattern, PathResolver.CaseInsensitiveEnumeration)
                                .Where(x => enumerationOptions == EntryOption.All
                                    || enumerationOptions == EntryOption.File && !Directory.Exists(x)
                                    || enumerationOptions == EntryOption.Directory && Directory.Exists(x));
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

        public bool Remove(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool MkDir(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool RmDir(string entryName)
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

        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions)
        {
            if (!MatchRoot(path))
            {
                return new string[] { };
            }
            try
            {
                return Directory.GetFileSystemEntries(PathResolver.Resolve(path), searchPattern, PathResolver.CaseInsensitiveEnumeration)
                                .Where(x => enumerationOptions == EntryOption.All
                                    || enumerationOptions == EntryOption.File && !Directory.Exists(x)
                                    || enumerationOptions == EntryOption.Directory && Directory.Exists(x));
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

        /// <summary>entryName のこのフォルダでのパス。このフォルダの外を指す絶対パスなら null</summary>
        private string EntryPath(string entryName)
        {
            var archivePath = entryName.Replace("\\", "/");
            var isAbsolute = Path.IsPathRooted(entryName);
            if (isAbsolute && !archivePath.StartsWith(BasePath))
            {
                return null;
            }
            var entryPath = isAbsolute ? archivePath.Replace(BasePath, "") : archivePath;
            return Path.Combine(BasePath, entryPath);
        }

        private FileInfo GetFileInfo(string entryName)
        {
            var path = EntryPath(entryName);
            return path == null ? null : new FileInfo(PathResolver.Resolve(path));
        }

        public bool Exists(string entryName)
        {
            var path = EntryPath(entryName);
            return path != null && PathResolver.FileExists(path);
        }

        public Stream OpenRead(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.Exists ?? false ? info.OpenRead() : null;
        }

        public Stream OpenWrite(string entryName)
        {
            var info = GetFileInfo(entryName);
            // 既にあるファイルは空にしてから書く（元の SRC の Open … For Output と同じ。FileInfo.OpenWrite は長さを 0 にしない）
            return info?.Open(FileMode.Create, FileAccess.Write);
        }

        public Stream OpenAppend(string entryName)
        {
            var info = GetFileInfo(entryName);
            return info?.Exists ?? false ? info.Open(FileMode.Append) : info?.Open(FileMode.Create, FileAccess.Write);
        }

        public bool Remove(string entryName)
        {
            var info = GetFileInfo(entryName);
            if (info?.Exists ?? false)
            {
                info.Delete();
                return true;
            }
            return false;
        }

        public bool MkDir(string entryName)
        {
            var info = GetFileInfo(entryName);
            Directory.CreateDirectory(info.FullName);
            return true;
        }

        public bool RmDir(string entryName)
        {
            var info = GetFileInfo(entryName);
            if (Directory.Exists(info.FullName))
            {
                Directory.Delete(info.FullName);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// zip アーカイブを basePath のフォルダに重ねる。root を指定すると、zip の中のそのフォルダ以下を basePath に重ねる。
    /// zip の中の名前は大文字・小文字（と全角・半角）を区別せずに探す。
    /// </summary>
    public class LocalFileSystemArchive : ILocalFileSystemEntrySet, IDisposable
    {
        public string BasePath { get; private set; }
        /// <summary>zip の中で basePath にあたるフォルダ（"Pack/" など。zip の最上位なら空）</summary>
        public string Root { get; private set; }
        private ZipArchive _archive;
        private SrcCollection<ZipArchiveEntry> _entryMap;
        private Task _entryMapResolveTask;

        public LocalFileSystemArchive(string basePath, ZipArchive archive) : this(basePath, archive, null)
        {
        }

        public LocalFileSystemArchive(string basePath, ZipArchive archive, string root)
        {
            var tmpBasePath = basePath.Replace("\\", "/");
            if (!tmpBasePath.EndsWith("/"))
            {
                tmpBasePath += "/";
            }
            BasePath = tmpBasePath;
            Root = string.IsNullOrEmpty(root) ? "" : root.Replace("\\", "/").Trim('/') + "/";
            _archive = archive;
            _entryMap = new SrcCollection<ZipArchiveEntry>();

            _entryMapResolveTask = Task.Run(() =>
            {
                foreach (var entry in _archive.Entries.Where(x => !string.IsNullOrEmpty(x.Name)))
                {
                    _entryMap[entry.FullName.Replace("\\", "/")] = entry;
                }
            });
        }
        public bool MatchRoot(string entryName)
        {
            return entryName.Replace("\\", "/").StartsWith(BasePath);
        }

        private void WaitForEntryMap()
        {
            while (!_entryMapResolveTask.IsCompleted)
            {
                Task.Delay(100).Wait();
            }
        }

        /// <summary>entryName の zip の中での名前。basePath の外を指す絶対パスなら null</summary>
        private string ArchivePath(string entryName)
        {
            var archivePath = entryName.Replace("\\", "/");
            if (Path.IsPathRooted(entryName))
            {
                if (!archivePath.StartsWith(BasePath))
                {
                    return null;
                }
                archivePath = archivePath.Substring(BasePath.Length);
            }
            return Root + archivePath;
        }

        private ZipArchiveEntry GetEntry(string entryName)
        {
            WaitForEntryMap();
            var entryPath = ArchivePath(entryName);
            if (entryPath == null)
            {
                return null;
            }
            return _archive.GetEntry(entryPath) ?? _entryMap[entryPath];
        }

        public bool Exists(string entryName)
        {
            return GetEntry(entryName) != null;
        }

        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions)
        {
            if (!MatchRoot(path))
            {
                return new string[] { };
            }
            WaitForEntryMap();
            var entryPath = ArchivePath(path);
            if (entryPath == null)
            {
                return new string[] { };
            }
            var regBase = Regex.Escape(Path.Combine(entryPath, searchPattern).Replace("\\", "/"))
                    .Replace(@"\*", "[^/]*").Replace(@"\?", ".");
            var dirRegex = new Regex("^" + regBase + (regBase.EndsWith("/") ? "" : "/") + "$", RegexOptions.IgnoreCase);
            var fileRegex = new Regex("^" + regBase + "$", RegexOptions.IgnoreCase);

            // zip の中の名前（Root を除いたもの）を basePath のパスにする
            string ToPath(string fullName) => Path.Combine(BasePath, fullName.Replace("\\", "/").Substring(Root.Length));
            var names = _archive.Entries.Select(x => x.FullName.Replace("\\", "/")).Where(x => x.StartsWith(Root, StringComparison.OrdinalIgnoreCase)).ToList();
            var dirs = new List<string>();
            var files = new List<string>();
            if (enumerationOptions.HasFlag(EntryOption.Directory))
            {
                // フォルダの項目が無い zip もあるため、ファイルの名前からもフォルダを求める
                dirs = names
                    .SelectMany(ParentDirectories)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Where(x => dirRegex.IsMatch(x))
                    .Select(x => ToPath(x).TrimEnd('/'))
                    .ToList();
            }
            if (enumerationOptions.HasFlag(EntryOption.File))
            {
                files = names
                    .Where(x => !x.EndsWith("/") && fileRegex.IsMatch(x))
                    .Select(ToPath)
                    .ToList();
            }
            return dirs.Concat(files).OrderBy(x => x);
        }

        /// <summary>zip の中の名前の親のフォルダ（"a/b/c.txt" なら "a/"・"a/b/"。フォルダの項目ならそれ自身も）</summary>
        private static IEnumerable<string> ParentDirectories(string name)
        {
            for (var i = name.IndexOf('/'); i >= 0; i = name.IndexOf('/', i + 1))
            {
                yield return name.Substring(0, i + 1);
            }
        }

        public Stream OpenRead(string entryName)
        {
            return GetEntry(entryName).Open();
        }

        public void Dispose()
        {
            WaitForEntryMap();
            _archive.Dispose();
        }

        public Stream OpenWrite(string entryName)
        {
            throw new NotSupportedException();
        }

        public Stream OpenAppend(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool Remove(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool MkDir(string entryName)
        {
            throw new NotSupportedException();
        }

        public bool RmDir(string entryName)
        {
            throw new NotSupportedException();
        }
    }
}
