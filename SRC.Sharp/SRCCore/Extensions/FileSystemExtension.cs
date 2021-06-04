using ShiftJISExtension;
using SRCCore.Filesystem;
using System.IO;
using System.Threading.Tasks;

namespace SRCCore.Extensions
{
    public static class FileSystemExtension
    {
        public static Stream OpenText(this IFileSystem fileSystem, SRCCompatibilityMode srcCompatibilityMode, params string[] paths)
        {
            return fileSystem.Open(paths).ToTextStream(srcCompatibilityMode);
        }
    }
}
