using System;
using System.IO;

namespace SRCCore.Filesystem
{
    /// <summary>
    /// 書き込みのストリームを包み、閉じたときに書き終えたことを知らせる。
    /// </summary>
    public sealed class WriteNotifyStream : Stream
    {
        private readonly Stream inner;
        private readonly string path;
        private readonly Action<string> written;
        private bool disposed;

        public WriteNotifyStream(Stream inner, string path, Action<string> written)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.path = path;
            this.written = written;
        }

        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => inner.CanSeek;
        public override bool CanWrite => inner.CanWrite;
        public override long Length => inner.Length;
        public override long Position { get => inner.Position; set => inner.Position = value; }
        public override void Flush() => inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);
        public override void SetLength(long value) => inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);
        public override void Write(ReadOnlySpan<byte> buffer) => inner.Write(buffer);

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                inner.Dispose();
                written?.Invoke(path);
            }
            base.Dispose(disposing);
        }
    }
}
