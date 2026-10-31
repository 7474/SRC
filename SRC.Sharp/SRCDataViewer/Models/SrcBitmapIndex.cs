using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCDataViewer.Models
{
    public class SrcBitmapIndex
    {
        public ICollection<BitmapFolder> Folders { get; set; }

        private IDictionary<string, string> index = new Dictionary<string, string>();
        private IDictionary<string, string> overlay = new Dictionary<string, string>();
        private Uri baseUri;


        public void BuildIndex(string baseUri)
        {
            this.baseUri = new Uri(baseUri);
            index = Folders
                .SelectMany(x => x.Files.Select(y => new
                {
                    Key = $"{x.Name}/{y}".ToLower(),
                    Value = $"{x.Path}/{y}"
                }))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        /// <summary>
        /// アーカイブ（zip）の中の画像を、7474/SRC-Data の画像より先に使う（ArchivePage）。
        /// 後ろのものほど優先する。分類（Pilot・Unit など）が違っても名前が同じなら使う（SRC が Bitmap の下のフォルダを順に探すのと同じ）
        /// </summary>
        public void SetOverlay(IEnumerable<(string Category, string FileName, string Uri)> images)
        {
            var newOverlay = new Dictionary<string, string>();
            foreach (var (category, fileName, uri) in images)
            {
                newOverlay[$"{category}/{fileName}".ToLower()] = uri;
                newOverlay[$"*/{fileName}".ToLower()] = uri;
            }
            overlay = newOverlay;
        }

        public void ClearOverlay()
        {
            overlay = new Dictionary<string, string>();
        }

        /// <summary>7474/SRC-Data の画像の一覧を読み直したときに、アーカイブの画像を引き継ぐ</summary>
        public void CopyOverlayFrom(SrcBitmapIndex other)
        {
            overlay = other.overlay;
        }

        public string GetBitmapUri(string categoryName, string fileName)
        {
            if (overlay.Count > 0 && !string.IsNullOrEmpty(fileName))
            {
                var name = fileName.ToLower().Replace('\\', '/');
                name = name.Substring(name.LastIndexOf('/') + 1);
                if (overlay.TryGetValue($"{categoryName}/{name}".ToLower(), out var uri) || overlay.TryGetValue($"*/{name}", out uri))
                {
                    return uri;
                }
            }

            var key = $"{categoryName}/{fileName}".ToLower().Replace(".bmp", ".png");

            return index.ContainsKey(key)
                ? new Uri(baseUri, index[key]).ToString()
                : "images/-.png";
        }
    }
    public class BitmapFolder
    {
        public string Name { get; set; }
        public string Base { get; set; }
        public string Path { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> Files { get; set; }
    }
}
