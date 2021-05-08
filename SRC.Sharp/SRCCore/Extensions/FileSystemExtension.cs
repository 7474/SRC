using ShiftJISExtension;
using SRCCore.Filesystem;
using System.IO;
using System.Threading.Tasks;

namespace SRCCore.Extensions
{
    public static class FileSystemExtension
    {
        public static Stream OpenText(this IFileSystem fileSystem, bool SRCCompatibilityMode, params string[] paths)
        {
            var stream = fileSystem.Open(paths);
            if (SRCCompatibilityMode)
            {
                // XXX あまりいい同期化ではないはず
                // Streamには同期メソッドがあるはずなので、同期処理したいなら同期メソッドを一貫して使うメソッドにするのがよさそう
                return Task.Run(() => stream.ToUTF8Async()).Result;
            }
            else
            {
                return stream;
            }
        }
    }
}
