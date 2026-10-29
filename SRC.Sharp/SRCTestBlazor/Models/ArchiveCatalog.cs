using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// 置いたサイトが公開している zip（シナリオ・素材・パック）の一覧と、zip の中のファイルの一覧（docs/gui/dataviewer.md）。
    /// 設定（viewer.json）が無ければ使わない
    /// </summary>
    public class ArchiveCatalog
    {
        public const string ConfigPath = "viewer.json";

        private readonly SemaphoreSlim loadLock = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, Task<IReadOnlyList<ArchiveFile>>> files = new Dictionary<string, Task<IReadOnlyList<ArchiveFile>>>();
        private bool loaded;

        public IReadOnlyList<ArchiveInfo> Archives { get; private set; } = Array.Empty<ArchiveInfo>();
        /// <summary>アーカイブの一覧が設定されているか</summary>
        public bool Enabled { get; private set; }

        public event Action OnChange;

        public async Task EnsureLoadedAsync(HttpClient http)
        {
            if (loaded)
            {
                return;
            }
            await loadLock.WaitAsync();
            try
            {
                if (loaded)
                {
                    return;
                }
                var config = await TryGetAsync(http, new Uri(http.BaseAddress, ConfigPath), SrcJsonContext.Default.ViewerConfig);
                var archives = new List<ArchiveInfo>();
                foreach (var indexUri in config?.ArchiveIndexes ?? Array.Empty<string>())
                {
                    var uri = new Uri(http.BaseAddress, indexUri);
                    var index = await TryGetAsync(http, uri, SrcJsonContext.Default.ArchiveIndex);
                    foreach (var archive in index?.Archives ?? new List<ArchiveInfo>())
                    {
                        archive.Source = index.Name;
                        archive.ResolveUris(uri);
                        archives.Add(archive);
                    }
                }
                Archives = archives;
                Enabled = config?.ArchiveIndexes?.Any() ?? false;
                loaded = true;
            }
            finally
            {
                loadLock.Release();
            }
            OnChange?.Invoke();
        }

        // 設定や一覧が無い・読めないサイトでも、データビューアは今までどおり動かす
        private static async Task<T> TryGetAsync<T>(HttpClient http, Uri uri, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> type) where T : class
        {
            try
            {
                return await http.GetFromJsonAsync(uri, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{uri} was not loaded: {ex.Message}");
                return null;
            }
        }

        public ArchiveInfo Find(string id)
        {
            return Archives.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 中身を持つアーカイブに展開する。パックは中の素材、シナリオは重ねるもの（requires）と自分を、重ねる順（後ろほど優先）に
        /// </summary>
        public IReadOnlyList<ArchiveInfo> Layers(ArchiveInfo archive)
        {
            var ids = archive.Members?.Any() ?? false
                ? archive.Members
                : (archive.Requires ?? new List<string>()).Append(archive.Id);
            return ids.Select(Find)
                .Where(x => x != null && x.HasFiles)
                .Distinct()
                .ToList();
        }

        /// <summary>zip の中のファイルの一覧（一度読んだものは覚えておく）</summary>
        public Task<IReadOnlyList<ArchiveFile>> GetFilesAsync(HttpClient http, ArchiveInfo archive)
        {
            if (!archive.HasFiles)
            {
                return Task.FromResult<IReadOnlyList<ArchiveFile>>(Array.Empty<ArchiveFile>());
            }
            lock (files)
            {
                if (!files.TryGetValue(archive.Files, out var task))
                {
                    task = LoadFilesAsync(http, archive);
                    files[archive.Files] = task;
                }
                return task;
            }
        }

        private static async Task<IReadOnlyList<ArchiveFile>> LoadFilesAsync(HttpClient http, ArchiveInfo archive)
        {
            var list = await TryGetAsync(http, new Uri(archive.Files), SrcJsonContext.Default.ArchiveFileList);
            var baseUri = new Uri(archive.Base);
            return (list?.Files ?? new List<ArchiveFile>())
                .Select(file =>
                {
                    file.Archive = archive;
                    // root の外（zip の説明書など）は zip の中の道のまま
                    file.RelativePath = ArchiveFiles.RelativeTo(file.Path, archive.Root) ?? file.Path;
                    file.Kind = ArchiveFiles.KindOf(file.Path);
                    file.Url = new Uri(baseUri, ArchiveFiles.EscapePath(file.Path)).ToString();
                    return file;
                })
                .ToList();
        }
    }
}
