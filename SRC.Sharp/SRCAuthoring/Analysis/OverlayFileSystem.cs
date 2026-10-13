using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCAuthoring.Analysis
{
    /// <summary>
    /// ローカルのファイルシステムに、エディタで編集中（未保存）の内容を重ねる。
    /// 重ねたファイルは UTF-8 で返す。
    /// </summary>
    public sealed class OverlayFileSystem : IFileSystem
    {
        private readonly IFileSystem inner;
        private readonly Dictionary<string, string> overlays = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public OverlayFileSystem(IFileSystem inner, IDictionary<string, string> overlays = null)
        {
            this.inner = inner;
            if (overlays != null)
            {
                foreach (var kv in overlays)
                {
                    this.overlays[Normalize(kv.Key)] = kv.Value;
                }
            }
        }

        /// <summary>パスを比べられる形にする（区切り文字を揃えて絶対パスにする）</summary>
        public static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            return Path.GetFullPath(path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
        }

        private bool TryGetOverlay(string[] paths, out string text)
        {
            text = null;
            try
            {
                return overlays.Count > 0 && overlays.TryGetValue(Normalize(inner.PathCombine(paths)), out text);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public bool FileExists(params string[] paths) => TryGetOverlay(paths, out _) || inner.FileExists(paths);

        public Stream Open(params string[] paths)
        {
            if (TryGetOverlay(paths, out var text))
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(text));
            }
            return inner.Open(paths);
        }

        public string PathCombine(params string[] paths) => inner.PathCombine(paths);
        public bool PathEquals(string a, string b) => inner.PathEquals(a, b);
        public void AddPath(string basePath) => inner.AddPath(basePath);
        public void AddAchive(string basePath, string archivePath) => inner.AddAchive(basePath, archivePath);
        public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions) => inner.GetFileSystemEntries(path, searchPattern, enumerationOptions);
        public void AddSafePath(string basePath) => inner.AddSafePath(basePath);
        public Stream OpenSafe(SafeOpenMode mode, params string[] paths) => inner.OpenSafe(mode, paths);
        public bool Remove(params string[] paths) => inner.Remove(paths);
        public bool MkDir(params string[] paths) => inner.MkDir(paths);
        public bool RmDir(params string[] paths) => inner.RmDir(paths);
        public bool RelativePathEuqals(string scenarioPath, string a, string b) => inner.RelativePathEuqals(scenarioPath, a, b);
        public bool IsAbsolutePath(string path) => inner.IsAbsolutePath(path);
        public string ToAbsolutePath(string scenarioPath, string path) => inner.ToAbsolutePath(scenarioPath, path);
        public string ToRelativePath(string scenarioPath, string path) => inner.ToRelativePath(scenarioPath, path);
        public string NormalizePath(string path) => inner.NormalizePath(path);
    }
}
