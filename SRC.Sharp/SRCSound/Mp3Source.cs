using NLayer;
using System.IO;

namespace SRCSound
{
    /// <summary>
    /// MP3 を NLayer で復号する。
    /// </summary>
    public sealed class Mp3Source : ISampleSource
    {
        private readonly MpegFile file;

        public Mp3Source(Stream stream)
        {
            file = new MpegFile(stream);
        }

        public int SampleRate => file.SampleRate;
        public int Channels => file.Channels;

        public int Read(float[] buffer, int offset, int count)
        {
            return file.ReadSamples(buffer, offset, count);
        }

        public void Rewind()
        {
            file.Position = 0;
        }
    }
}
