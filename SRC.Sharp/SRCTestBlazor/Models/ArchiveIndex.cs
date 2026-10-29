using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// データビューアの設定（wwwroot/viewer.json。置いたサイトが書く）。
    /// 形は docs/gui/dataviewer.md の「アーカイブを読む」
    /// </summary>
    public class ViewerConfig
    {
        /// <summary>アーカイブの一覧（ArchiveIndex）の URL。相対ならデータビューアの置き場所から</summary>
        public ICollection<string> ArchiveIndexes { get; set; }
    }

    /// <summary>サイトが公開している zip（シナリオ・素材・パック）の一覧</summary>
    public class ArchiveIndex
    {
        public int Format { get; set; }
        public string Name { get; set; }
        public List<ArchiveInfo> Archives { get; set; }
    }

    public class ArchiveInfo
    {
        public string Id { get; set; }
        /// <summary>scenario・resource・pack</summary>
        public string Kind { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string License { get; set; }
        /// <summary>サイトの頁</summary>
        public string Page { get; set; }
        /// <summary>zip の中のファイルの一覧（ArchiveFileList）の URL</summary>
        public string Files { get; set; }
        /// <summary>zip の中のファイルの URL の前に付けるもの</summary>
        public string Base { get; set; }
        /// <summary>zip の中のフォルダ。データ・画像はこの下を SRC のフォルダとして読む</summary>
        public string Root { get; set; }
        /// <summary>重ねるアーカイブ（シナリオ）。前から順に重ね、後ろほど優先する</summary>
        public List<string> Requires { get; set; }
        /// <summary>投稿先のパック（素材）</summary>
        public string Pack { get; set; }
        /// <summary>パックの中の素材（重ねる順）</summary>
        public List<string> Members { get; set; }

        /// <summary>読んだ一覧の名前</summary>
        [JsonIgnore]
        public string Source { get; set; }

        [JsonIgnore]
        public bool HasFiles => !string.IsNullOrEmpty(Files) && !string.IsNullOrEmpty(Base);

        /// <summary>一覧の URL を基準に、相対の URL を絶対にする</summary>
        public void ResolveUris(Uri indexUri)
        {
            Page = Resolve(indexUri, Page);
            Files = Resolve(indexUri, Files);
            Base = Resolve(indexUri, Base);
        }

        private static string Resolve(Uri baseUri, string value)
        {
            return string.IsNullOrEmpty(value) ? value : new Uri(baseUri, value).ToString();
        }
    }

    public class ArchiveFileList
    {
        public List<ArchiveFile> Files { get; set; }
    }

    public class ArchiveFile
    {
        /// <summary>zip の中の道（区切りは /）</summary>
        public string Path { get; set; }
        public long Size { get; set; }

        /// <summary>ファイルの URL</summary>
        [JsonIgnore]
        public string Url { get; set; }
        /// <summary>入っていたアーカイブ</summary>
        [JsonIgnore]
        public ArchiveInfo Archive { get; set; }
        /// <summary>root から見た道</summary>
        [JsonIgnore]
        public string RelativePath { get; set; }
        [JsonIgnore]
        public ArchiveFileKind Kind { get; set; }
        [JsonIgnore]
        public string Name => System.IO.Path.GetFileName(Path);
        [JsonIgnore]
        public string Folder => System.IO.Path.GetDirectoryName(RelativePath)?.Replace('\\', '/') ?? "";
    }

    public enum ArchiveFileKind
    {
        Image,
        Audio,
        Midi,
        Data,
        Event,
        Map,
        Document,
        Other,
    }

    public static class ArchiveFiles
    {
        /// <summary>データビューアが読むデータのファイル（LoadTitleView と同じ）</summary>
        public static readonly IReadOnlyCollection<string> DataFileNames = new[]
        {
            "unit.txt", "robot.txt", "pilot.txt", "non_pilot.txt", "pilot_message.txt", "pilot_dialog.txt", "item.txt",
        };

        public static ArchiveFileKind KindOf(string path)
        {
            var name = System.IO.Path.GetFileName(path).ToLowerInvariant();
            var ext = System.IO.Path.GetExtension(name);
            switch (ext)
            {
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                    return ArchiveFileKind.Image;
                case ".wav":
                case ".mp3":
                case ".ogg":
                    return ArchiveFileKind.Audio;
                case ".mid":
                case ".midi":
                    return ArchiveFileKind.Midi;
                case ".eve":
                    return ArchiveFileKind.Event;
                case ".map":
                    return ArchiveFileKind.Map;
                case ".md":
                case ".htm":
                case ".html":
                    return ArchiveFileKind.Document;
                case ".txt":
                    return name.StartsWith("readme") ? ArchiveFileKind.Document : ArchiveFileKind.Data;
                default:
                    return name.StartsWith("readme") ? ArchiveFileKind.Document : ArchiveFileKind.Other;
            }
        }

        /// <summary>zip の中の道から root を除く（root の外なら null）</summary>
        public static string RelativeTo(string path, string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                return path;
            }
            var prefix = root.TrimEnd('/') + "/";
            return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? path.Substring(prefix.Length) : null;
        }

        /// <summary>道の区切りごとに URL エンコードする</summary>
        public static string EscapePath(string path)
        {
            return string.Join("/", path.Split('/').Select(Uri.EscapeDataString));
        }

        /// <summary>
        /// Bitmap の下の画像を、SRC が探すときの分類（Pilot・Unit など。Bitmap の直下のフォルダ）とファイル名にする。Bitmap の下でなければ null
        /// </summary>
        public static (string Category, string FileName)? BitmapKey(string relativePath)
        {
            var parts = relativePath.Split('/');
            if (parts.Length < 3 || !parts[0].Equals("Bitmap", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return (parts[1], parts[parts.Length - 1]);
        }
    }
}
