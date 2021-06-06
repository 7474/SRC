using System.Collections.Generic;
using System.IO;

namespace SRCCore.Filesystem
{
    public interface IFileSystem
    {
        string PathCombine(params string[] paths);
        bool FileExists(params string[] paths);
        Stream Open(params string[] paths);

        /// <summary>
        /// 読み込みアクセス対象とするパスを追加します。
        /// パスやアーカイブは後に追加したものから先に走査されます。
        /// </summary>
        /// <param name="basePath">ファイルが配置されているディレクトリのパス</param>
        void AddPath(string basePath);

        /// <summary>
        /// 読み込みアクセス対象とするアーカイブファイルを追加します。
        /// パスやアーカイブは後に追加したものから先に走査されます。
        /// </summary>
        /// <param name="basePath">アーカイブファイルが配置されているディレクトリのパス</param>
        /// <param name="archivePath">アーカイブファイルのパス</param>
        void AddAchive(string basePath, string archivePath);

        IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions);

        //
        /// <summary>
        /// シナリオから操作できるパスを追加します。
        /// </summary>
        /// <param name="basePath"></param>
        void AddSafePath(string basePath);
        /// <summary>
        /// <see cref="AddSafePath"/>で追加したパスからの相対パスでファイルを開きます。
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        Stream OpenSafe(SafeOpenMode mode, params string[] paths);

        //
        bool RelativePathEuqals(string scenarioPath, string a, string b);
        bool IsAbsolutePath(string path);
        string ToAbsolutePath(string scenarioPath, string path);
        string ToRelativePath(string scenarioPath, string path);
        string NormalizePath(string path);
    }

    public enum EntryOption
    {
        File = 1,
        Directory = 2,
        All = 3,
    }

    public enum SafeOpenMode
    {
        Read,
        Write,
        Append,
    }
}
