using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
