using System.IO;

namespace SRCCore.Filesystem
{
    public interface IFileSystem
    {
        string PathCombine(params string[] paths);
        bool FileExists(params string[] paths);
        Stream Open(params string[] paths);

        //
        bool RelativePathEuqals(string scenarioPath, string a, string b);
        string ToAbsolutePath(string scenarioPath, string path);
        string ToRelativePath(string scenarioPath, string path);
        string NormalizePath(string path);
    }
}
