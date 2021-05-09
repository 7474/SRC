using System.IO;

namespace SRCCore.Filesystem
{
    public interface IFileSystem
    {
        string PathCombine(params string[] paths);
        bool FileExists(params string[] paths);
        Stream Open(params string[] paths);

        /// <summary>
        /// 読み込みアクセス対象とするアーカイブファイルを追加します。
        /// アーカイブはファイルシステムが規定でサポートする要素より先に走査されます。
        /// アーカイブは後に追加したものから先に走査されます。
        /// </summary>
        /// <param name="basePath">アーカイブファイルが配置されているディレクトリのパス</param>
        /// <param name="archivePath">アーカイブファイルのパス</param>
        void AddAchive(string basePath, string archivePath);

        //
        bool RelativePathEuqals(string scenarioPath, string a, string b);
        string ToAbsolutePath(string scenarioPath, string path);
        string ToRelativePath(string scenarioPath, string path);
        string NormalizePath(string path);
    }
}
