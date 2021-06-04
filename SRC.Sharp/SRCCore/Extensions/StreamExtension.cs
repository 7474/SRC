using ShiftJISExtension;
using System.IO;
using System.Threading.Tasks;

namespace SRCCore.Extensions
{
    public static class StreamExtension
    {
        /// <summary>
        /// Streamの内容を全て読み取ってMemoryStreamに格納して返す。
        /// 元のStreamは破棄される。
        /// </summary>
        public static MemoryStream ToMemoryStream(this Stream stream)
        {
            try
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }
            finally
            {
                stream.Dispose();
            }
        }

        public static Stream ToTextStream(this Stream stream, SRCCompatibilityMode srcCompatibilityMode)
        {
            if (srcCompatibilityMode.HasFlag(SRCCompatibilityMode.Read))
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
