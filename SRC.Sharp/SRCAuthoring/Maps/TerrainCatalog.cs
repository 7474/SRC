using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Maps
{
    /// <summary>地形データの 1 件（マップ作成ツールのパレットに出す）</summary>
    public sealed class TerrainInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>地形タイプ（陸・水・空など）</summary>
        public string Class { get; set; }
        /// <summary>画像の名前（Bitmap\Map\名前\名前0000.bmp など）</summary>
        public string Bitmap { get; set; }
        /// <summary>移動コスト（- は通れない。SRCCore は 2 倍で持つので戻す）</summary>
        public string MoveCost { get; set; }
        public int HitMod { get; set; }
        public int DamageMod { get; set; }
        public List<string> Features { get; set; } = new List<string>();
    }

    /// <summary>
    /// 地形データ（terrain.txt）と、マップの画像（Bitmap\Map）を探す規則。SRCCore と同じ処理で読む:
    /// SRC 本体（と拡張データのフォルダ）の Data\System\terrain.txt を読み、シナリオの Data\System\terrain.txt があれば重ねる。
    /// </summary>
    public sealed class TerrainCatalog
    {
        private readonly TerrainDataList list;

        private TerrainCatalog(TerrainDataList list)
        {
            this.list = list;
        }

        /// <summary>地形データを順に読む（後のものが同じ番号を上書きする）。読めないファイルは errors に理由を入れて飛ばす</summary>
        public static TerrainCatalog Load(IEnumerable<(string name, byte[] data)> files, List<string> errors = null)
        {
            var src = new SRCCore.SRC();
            var list = new TerrainDataList(src);
            foreach (var (name, data) in files)
            {
                try
                {
                    using var stream = new MemoryStream(data);
                    list.Load(name, stream);
                }
                catch (Exception ex)
                {
                    errors?.Add($"{name}: {(ex.InnerException ?? ex).Message}");
                }
            }
            return new TerrainCatalog(list);
        }

        /// <summary>登録した順の地形</summary>
        public IReadOnlyList<TerrainInfo> Terrains =>
            Enumerable.Range(1, list.Count).Select(n => list.Item(list.OrderedID(n))).Select(t => new TerrainInfo
            {
                Id = t.ID,
                Name = t.Name,
                Class = t.Class,
                Bitmap = t.Bitmap,
                MoveCost = t.MoveCost >= 1000 ? "-" : (t.MoveCost / 2.0).ToString(CultureInfo.InvariantCulture),
                HitMod = t.HitMod,
                DamageMod = t.DamageMod,
                Features = t.colFeature.List.Select(f => f.Name + (f.StrData != "" && f.StrData != null ? "=" + f.StrData : "")).ToList(),
            }).ToList();

        public bool IsDefined(int id) => list.IsDefined(id);

        /// <summary>
        /// マップの画像のファイルの候補（SRCCore の Map.SearchMapImageFile と同じ順）。
        /// フォルダ（シナリオ → 拡張データ → SRC 本体）ごとに、この順に探す
        /// </summary>
        public static IEnumerable<string> ImageCandidates(string bitmap, int no)
        {
            yield return $"Bitmap/Map/{bitmap}/{bitmap}{no.ToString("0000", CultureInfo.InvariantCulture)}.bmp";
            yield return $"Bitmap/Map/{bitmap}{no.ToString("0000", CultureInfo.InvariantCulture)}.bmp";
            yield return $"Bitmap/Map/{bitmap}{no.ToString(CultureInfo.InvariantCulture)}.bmp";
        }

        /// <summary>
        /// 画像のファイルを探す。roots はフォルダ（探す順）ごとの、手元にあるファイルの相対パス（区切りは / か \。大文字・小文字は区別しない）。
        /// 見つかれば (フォルダの番号, そのフォルダの中のパス（手元の書き方のまま）)。無ければ null
        /// </summary>
        public static (int root, string path)? FindImage(string bitmap, int no, IReadOnlyList<ImageIndex> roots)
        {
            if (string.IsNullOrEmpty(bitmap) || no == MapTile.NoLayer)
            {
                return null;
            }
            for (var r = 0; r < roots.Count; r++)
            {
                foreach (var candidate in ImageCandidates(bitmap, no))
                {
                    if (roots[r].TryGet(candidate, out var actual))
                    {
                        return (r, actual);
                    }
                }
            }
            return null;
        }

        /// <summary>この画像の名前で、手元にある画像の番号（パレットに並べる）</summary>
        public static IReadOnlyList<int> ImageNumbers(string bitmap, IReadOnlyList<ImageIndex> roots)
        {
            if (string.IsNullOrEmpty(bitmap))
            {
                return Array.Empty<int>();
            }
            var pattern = new Regex(@"^bitmap/map/(?:" + Regex.Escape(bitmap.ToLowerInvariant()) + "/)?" + Regex.Escape(bitmap.ToLowerInvariant()) + @"(\d+)\.bmp$");
            return roots.SelectMany(r => r.Keys)
                .Select(k => pattern.Match(k))
                .Where(m => m.Success)
                .Select(m => int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }

    /// <summary>1 つのフォルダの、手元にあるファイルの相対パスの索引（大文字・小文字と区切りを区別しない）</summary>
    public sealed class ImageIndex
    {
        private readonly Dictionary<string, string> files = new Dictionary<string, string>(StringComparer.Ordinal);

        public ImageIndex(IEnumerable<string> paths)
        {
            foreach (var p in paths)
            {
                files.TryAdd(Normalize(p), p);
            }
        }

        public IEnumerable<string> Keys => files.Keys;

        public bool TryGet(string path, out string actual) => files.TryGetValue(Normalize(path), out actual);

        public static string Normalize(string path) => path.Replace('\\', '/').TrimStart('/').ToLowerInvariant();
    }
}
