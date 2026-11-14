using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;

// JavaScript（wwwroot/studio-core.js）から StudioExports を呼ぶ。Main では何もしない
return;

namespace SRCStudio
{
    /// <summary>
    /// SRC#Studio の計算（7474/SRC#1641）。画面は JavaScript で、ファイルの読み書きは画面の側（ブラウザか VS Code）が行い、
    /// ここには中身（バイト列）とファイルの一覧だけを渡す。
    /// </summary>
    public static partial class StudioExports
    {
        private static TerrainCatalog terrains = TerrainCatalog.Load(Array.Empty<(string, byte[])>());
        private static readonly List<(string, byte[])> terrainFiles = new List<(string, byte[])>();
        private static readonly List<ImageIndex> imageRoots = new List<ImageIndex>();
        private static readonly Dictionary<int, MapDocument> templates = new Dictionary<int, MapDocument>();
        private static int nextTemplate = 1;

        /// <summary>
        /// AddTerrainFile で足した地形データを読む。
        /// 結果: { "terrains": [{ id, name, class, bitmap, moveCost, hitMod, damageMod, features }], "errors": [文字列] }
        /// </summary>
        [JSExport]
        internal static string LoadTerrains()
        {
            var errors = new List<string>();
            terrains = TerrainCatalog.Load(terrainFiles, errors);
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                w.WriteStartArray("terrains");
                foreach (var t in terrains.Terrains)
                {
                    w.WriteStartObject();
                    w.WriteNumber("id", t.Id);
                    w.WriteString("name", t.Name);
                    w.WriteString("class", t.Class);
                    w.WriteString("bitmap", t.Bitmap);
                    w.WriteString("moveCost", t.MoveCost);
                    w.WriteNumber("hitMod", t.HitMod);
                    w.WriteNumber("damageMod", t.DamageMod);
                    w.WriteStartArray("features");
                    foreach (var f in t.Features)
                    {
                        w.WriteStringValue(f);
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("errors");
                foreach (var e in errors)
                {
                    w.WriteStringValue(e);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>地形データのファイルを足す（SRC 本体 → 拡張データ → シナリオの Data\System\terrain.txt の順に。後のものが同じ番号を上書きする）</summary>
        [JSExport]
        internal static void AddTerrainFile(string name, byte[] data)
        {
            terrainFiles.Add((name, data));
        }

        /// <summary>足した地形データのファイルを空にする</summary>
        [JSExport]
        internal static void ClearTerrainFiles()
        {
            terrainFiles.Clear();
        }

        /// <summary>画像を探すフォルダを空にする</summary>
        [JSExport]
        internal static void ClearImageRoots()
        {
            imageRoots.Clear();
        }

        /// <summary>画像を探すフォルダを、探す順（シナリオ → 拡張データ → SRC 本体）に足す。paths はそのフォルダの中のファイルの相対パス</summary>
        [JSExport]
        internal static int AddImageRoot(string[] paths)
        {
            imageRoots.Add(new ImageIndex(paths));
            return imageRoots.Count - 1;
        }

        /// <summary>画像のファイル（SRCCore と同じ順で探す）。見つかれば "フォルダの番号:パス"、無ければ空</summary>
        [JSExport]
        internal static string FindImage(string bitmap, int no)
        {
            var found = TerrainCatalog.FindImage(bitmap, no, imageRoots);
            return found == null ? "" : found.Value.root + ":" + found.Value.path;
        }

        /// <summary>この画像の名前で、手元にある画像の番号</summary>
        [JSExport]
        internal static int[] ImageNumbers(string bitmap)
        {
            return TerrainCatalog.ImageNumbers(bitmap, imageRoots).ToArray();
        }

        /// <summary>
        /// マップを読む。結果: { "width", "height", "tiles": [地形, 画像, …（行ごと）], "layer": 同じ形か null, "template": 書き方の番号, "legacy": bool }。
        /// 読めなければ { "error": 理由 }
        /// </summary>
        [JSExport]
        internal static string ParseMap(byte[] data)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                try
                {
                    var doc = MapDocument.Parse(data);
                    var id = nextTemplate++;
                    templates[id] = doc;
                    w.WriteNumber("width", doc.Width);
                    w.WriteNumber("height", doc.Height);
                    WriteInts(w, "tiles", doc.TileRows());
                    if (doc.Layer == null)
                    {
                        w.WriteNull("layer");
                    }
                    else
                    {
                        WriteInts(w, "layer", doc.LayerRows());
                    }
                    w.WriteNumber("template", id);
                    w.WriteBoolean("legacy", doc.Legacy);
                }
                catch (Exception ex)
                {
                    w.WriteString("error", ex.Message);
                }
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>マップを書き出す（テキスト）。template は ParseMap の番号（新規は 0）。layer が空の配列ならレイヤーなし</summary>
        [JSExport]
        internal static string SerializeMap(int width, int height, int[] tiles, int[] layer, int template)
        {
            templates.TryGetValue(template, out var t);
            var doc = MapDocument.FromRows(width, height, tiles, layer == null || layer.Length == 0 ? null : layer, t);
            return doc.Serialize();
        }

        private static void WriteInts(Utf8JsonWriter w, string name, int[] values)
        {
            w.WriteStartArray(name);
            foreach (var v in values)
            {
                w.WriteNumberValue(v);
            }
            w.WriteEndArray();
        }
    }
}
