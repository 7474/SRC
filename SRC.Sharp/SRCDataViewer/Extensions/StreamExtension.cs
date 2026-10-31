using System.IO;
using System.Threading.Tasks;

namespace SRCDataViewer.Extensions
{
    public static class StreamExtension
    {
        public static async Task<Stream> ToSyncStreamAsync(this Stream stream)
        {
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
