using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Extensions
{
    public static class StreamExtension
    {
        public static Stream CloneStream(this Stream stream)
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
