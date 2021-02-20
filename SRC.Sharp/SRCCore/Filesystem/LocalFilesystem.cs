using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Filesystem
{
    public class LocalFilesystem : IFilesystem
    {
        public string ToPath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool FileExists(params string[] paths)
        {
            string path = ToPath(paths);
            return File.Exists(path);
        }

        public Stream Open(params string[] paths)
        {
            return new FileStream(ToPath(paths), FileMode.Open);
        }
    }
}
