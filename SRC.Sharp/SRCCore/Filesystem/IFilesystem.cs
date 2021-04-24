using System.IO;

namespace SRCCore.Filesystem
{
    public interface IFileSystem
    {
        string PathCombine(params string[] paths);
        bool FileExists(params string[] paths);
        Stream Open(params string[] paths);
    }
}
