using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    public sealed class ResourceCatalog
    {
        public int Format { get; set; }
        public string Name { get; set; }
        public List<ResourcePack> Entries { get; set; } = new List<ResourcePack>();
    }

    public sealed class ResourcePack
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Title { get; set; }
        public string Archive { get; set; }
        public string Version { get; set; }
        public string Root { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string Description { get; set; }
    }

    public sealed class PackResource
    {
        public string Path { get; }
        public ZipArchiveEntry Entry { get; }
        public string MediaType { get; }
        public bool IsImage => MediaType.StartsWith("image/", StringComparison.Ordinal);
        public bool IsAudio => MediaType.StartsWith("audio/", StringComparison.Ordinal);

        public PackResource(string path, ZipArchiveEntry entry)
        {
            Path = path;
            Entry = entry;
            MediaType = System.IO.Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                ".m4a" => "audio/mp4",
                ".flac" => "audio/flac",
                _ => "application/octet-stream",
            };
        }
    }

    public sealed class PackResources : IDisposable
    {
        public const int MaxArchiveBytes = 64 * 1024 * 1024;
        public const int MaxPreviewBytes = 16 * 1024 * 1024;
        private readonly ZipArchive archive;
        public IReadOnlyList<PackResource> Files { get; }

        public PackResources(Stream stream, string root)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                archive = new ZipArchive(stream, ZipArchiveMode.Read, false, Encoding.GetEncoding(932));
                if (archive.Entries.Count > 10000)
                {
                    throw new InvalidDataException("ZIP contains more than 10000 entries.");
                }
                var prefix = Normalize(root ?? "").Trim('/');
                if (prefix.Length > 0)
                {
                    prefix += "/";
                }
                Files = archive.Entries
                    .Where(e => !e.FullName.EndsWith("/") && !e.FullName.EndsWith("\\"))
                    .Select(e => (Entry: e, Name: Normalize(e.FullName)))
                    .Where(e => e.Name.StartsWith(prefix, StringComparison.Ordinal))
                    .Select(e => new PackResource(e.Name.Substring(prefix.Length), e.Entry))
                    .OrderBy(e => e.Path, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                if (Files.Count == 0)
                {
                    throw new InvalidDataException("No files found under the pack root.");
                }
            }
            catch
            {
                archive?.Dispose();
                stream.Dispose();
                throw;
            }
        }

        private static string Normalize(string path)
        {
            var value = path.Replace('\\', '/');
            if (value.StartsWith("/") || value.Contains(':')
                || value.Split('/').Any(part => part == ".." || part == "."))
            {
                throw new InvalidDataException("Invalid ZIP path.");
            }
            return value;
        }

        public static async Task<MemoryStream> ReadLimitedAsync(Stream input, int limit, CancellationToken token = default)
        {
            var output = new MemoryStream();
            try
            {
                var buffer = new byte[81920];
                int count;
                while ((count = await input.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                {
                    if (output.Length + count > limit)
                    {
                        throw new InvalidDataException($"File exceeds {limit / 1024 / 1024} MiB.");
                    }
                    await output.WriteAsync(buffer, 0, count, token);
                }
                output.Position = 0;
                return output;
            }
            catch
            {
                output.Dispose();
                throw;
            }
        }

        public void Dispose() => archive.Dispose();
    }
}
