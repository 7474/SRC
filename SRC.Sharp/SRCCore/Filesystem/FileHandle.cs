using ShiftJISExtension;
using SRCCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Filesystem
{
    public class FileHandleManager
    {
        private int _currentHandle = 0;
        private IDictionary<int, FileHandle> _handles = new Dictionary<int, FileHandle>();

        public FileHandle Add(SafeOpenMode mode, SRCCompatibilityMode srcCompatibilityMode, Stream stream)
        {
            var h = new FileHandle(++_currentHandle, stream, mode, srcCompatibilityMode);
            _handles.Add(h.Handle, h);
            return h;
        }

        public FileHandle Get(int handle)
        {
            return _handles.ContainsKey(handle) ? _handles[handle] : null;
        }

        public void Close(int handle)
        {
            var h = Get(handle);
            if (h != null)
            {
                if (h.Reader != null) { h.Reader.Close(); }
                if (h.Writer != null) { h.Writer.Close(); }
                h.Dispose();
                _handles.Remove(handle);
            }
        }
    }

    public class FileHandle : IDisposable
    {
        public int Handle { get; private set; }
        public Stream Stream { get; private set; }
        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        public FileHandle(int handle, Stream stream, SafeOpenMode mode, SRCCompatibilityMode srcCompatibilityMode)
        {
            Handle = handle;
            Stream = stream;
            if (mode == SafeOpenMode.Read)
            {
                Reader = new StreamReader(stream.ToTextStream(srcCompatibilityMode));
            }
            else
            {
                Writer = new StreamWriter(stream, srcCompatibilityMode.HasFlag(SRCCompatibilityMode.Write) ? Encoding.GetEncoding(932) : Encoding.UTF8);
            }
        }

        public void Dispose()
        {
            ((IDisposable)Stream).Dispose();
        }
    }
}
