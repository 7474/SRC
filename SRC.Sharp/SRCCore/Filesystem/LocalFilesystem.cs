using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCCore.Filesystem
{
    public class LocalFileSystem : IFileSystem
    {
        public string PathCombine(params string[] paths)
        {
            // 先頭の `\` は絶対パスのルートとして扱われないことを期待されている。
            return Path.Combine(paths.Select(x => Regex.Replace(x, @"^\\", "")).ToArray());
        }

        public bool FileExists(params string[] paths)
        {
            string path = PathCombine(paths);
            return File.Exists(path);
        }

        public Stream Open(params string[] paths)
        {
            return new FileStream(PathCombine(paths), FileMode.Open);
        }
    }
}
