using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Maps
{
    /// <summary>マップの 1 マス（地形の番号と画像の番号）</summary>
    public struct MapTile : IEquatable<MapTile>
    {
        /// <summary>レイヤーが無いマス（SRCCore の MapCell.NO_LAYER_NUM）</summary>
        public const int NoLayer = 10000;

        public int Terrain;
        public int Bitmap;

        public MapTile(int terrain, int bitmap)
        {
            Terrain = terrain;
            Bitmap = bitmap;
        }

        public static MapTile Empty => new MapTile(NoLayer, NoLayer);

        public bool IsEmpty => Terrain == NoLayer;

        public bool Equals(MapTile other) => Terrain == other.Terrain && Bitmap == other.Bitmap;
        public override bool Equals(object obj) => obj is MapTile t && Equals(t);
        public override int GetHashCode() => HashCode.Combine(Terrain, Bitmap);
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1}", Terrain, Bitmap);
    }

    /// <summary>
    /// マップデータ（.map）の読み書き。書式は SRCCore の Map.LoadMapData と同じ:
    /// 1 行目「MapData」（引用符で囲んでもよい）、2 行目は予約行、3 行目「幅,高さ」、続いて x（列）ごとに y（行）の順で「地形の番号,画像の番号」を 1 行ずつ。
    /// その後に「Layer」の行があれば、同じ順でレイヤーの「地形の番号,画像の番号」（無ければ 10000,10000）。
    /// 1 行目が MapData でなければ、20×20 の旧形式（1 行目から地形）。
    /// 読み込んだ書き方（改行・1 行目・予約行）を覚え、変えていないところは同じ書き方で書き出す。
    /// </summary>
    public sealed class MapDocument
    {
        /// <summary>SRCCore の MapCell.NO_LAYER_NUM と同じ</summary>
        public const int NoLayer = MapTile.NoLayer;
        /// <summary>元の MapEdit の上限</summary>
        public const int MaxSize = 50;

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>地形。[x - 1, y - 1]</summary>
        public MapTile[,] Tiles { get; private set; }

        /// <summary>レイヤー。無ければ null</summary>
        public MapTile[,] Layer { get; private set; }

        /// <summary>旧形式（20×20、ヘッダなし）で読んだ</summary>
        public bool Legacy { get; private set; }

        /// <summary>1 行目（MapData。引用符の付け方を残す）</summary>
        public string HeaderLine { get; set; } = "MapData";

        /// <summary>2 行目（予約行）</summary>
        public string ReservedLine { get; set; } = "";

        /// <summary>改行（読んだファイルのもの。新規は CRLF。元の SRC は Windows のため）</summary>
        public string NewLine { get; set; } = "\r\n";

        /// <summary>最後の行の後に改行があるか</summary>
        public bool TrailingNewLine { get; set; } = true;

        public MapDocument(int width, int height, MapTile fill)
        {
            Resize(width, height, fill);
        }

        private MapDocument()
        {
        }

        /// <summary>
        /// 画面（SRC#Studio）が持つ配列から作る。tiles・layer は行（y）ごとに x の順で「地形の番号, 画像の番号」を並べたもの（長さ 幅×高さ×2）。
        /// layer が null ならレイヤーなし。書き方（1 行目・予約行・改行）は template から引き継ぐ（null なら新規の書き方）
        /// </summary>
        public static MapDocument FromRows(int width, int height, int[] tiles, int[] layer, MapDocument template)
        {
            var doc = new MapDocument(width, height, new MapTile(0, 0));
            if (template != null)
            {
                doc.HeaderLine = template.HeaderLine;
                doc.ReservedLine = template.ReservedLine;
                doc.NewLine = template.NewLine;
                doc.TrailingNewLine = template.TrailingNewLine;
                doc.Legacy = template.Legacy && width == 20 && height == 20;
            }
            if (tiles.Length != width * height * 2 || (layer != null && layer.Length != width * height * 2))
            {
                throw new ArgumentException("マスの数が大きさと合いません");
            }
            if (layer != null)
            {
                doc.SetLayerEnabled(true);
            }
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (y * width + x) * 2;
                    doc.Tiles[x, y] = new MapTile(tiles[i], tiles[i + 1]);
                    if (layer != null)
                    {
                        doc.Layer[x, y] = new MapTile(layer[i], layer[i + 1]);
                    }
                }
            }
            return doc;
        }

        /// <summary>FromRows の形の配列（地形）</summary>
        public int[] TileRows() => Rows(Tiles);

        /// <summary>FromRows の形の配列（レイヤー。無ければ null）</summary>
        public int[] LayerRows() => Layer == null ? null : Rows(Layer);

        private int[] Rows(MapTile[,] source)
        {
            var a = new int[Width * Height * 2];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var i = (y * Width + x) * 2;
                    a[i] = source[x, y].Terrain;
                    a[i + 1] = source[x, y].Bitmap;
                }
            }
            return a;
        }

        public MapTile this[int x, int y]
        {
            get => Tiles[x - 1, y - 1];
            set => Tiles[x - 1, y - 1] = value;
        }

        /// <summary>サイズを変える。左上をそろえ、増えたマスは fill で埋める（レイヤーは空）</summary>
        public void Resize(int width, int height, MapTile fill)
        {
            if (width < 1 || height < 1 || width > MaxSize || height > MaxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"マップの大きさは 1〜{MaxSize}");
            }
            var tiles = new MapTile[width, height];
            MapTile[,] layer = Layer == null ? null : new MapTile[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var inside = Tiles != null && x < Width && y < Height;
                    tiles[x, y] = inside ? Tiles[x, y] : fill;
                    if (layer != null)
                    {
                        layer[x, y] = inside ? Layer[x, y] : MapTile.Empty;
                    }
                }
            }
            Width = width;
            Height = height;
            Tiles = tiles;
            Layer = layer;
            Legacy = Legacy && width == 20 && height == 20;
        }

        /// <summary>レイヤーを持たせる（全て空）か、外す</summary>
        public void SetLayerEnabled(bool enabled)
        {
            if (!enabled)
            {
                Layer = null;
                return;
            }
            if (Layer != null)
            {
                return;
            }
            Layer = new MapTile[Width, Height];
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Layer[x, y] = MapTile.Empty;
                }
            }
        }

        /// <summary>レイヤーに空でないマスがあるか</summary>
        public bool HasLayerContent => Layer != null && Layer.Cast<MapTile>().Any(t => !t.IsEmpty);

        public static MapDocument Parse(byte[] data)
        {
            return Parse(DecodeText(data));
        }

        /// <summary>マップのテキストは数字と ASCII だけだが、1 行目などに日本語があっても読めるよう、UTF-8 として読めなければ Shift_JIS で読む</summary>
        public static string DecodeText(byte[] data)
        {
            try
            {
                return new UTF8Encoding(false, true).GetString(data).TrimStart('﻿');
            }
            catch (DecoderFallbackException)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding(932).GetString(data);
            }
        }

        public static MapDocument Parse(string text)
        {
            var doc = new MapDocument
            {
                NewLine = text.Contains("\r\n") ? "\r\n" : text.Contains('\n') ? "\n" : "\r\n",
                TrailingNewLine = text.EndsWith("\n", StringComparison.Ordinal),
            };
            var lines = text.Replace("\r\n", "\n").Split('\n').ToList();
            if (doc.TrailingNewLine)
            {
                lines.RemoveAt(lines.Count - 1);
            }
            var i = 0;
            string Next(string what)
            {
                if (i >= lines.Count)
                {
                    throw new InvalidDataException($"{i + 1} 行目: {what}が足りません");
                }
                return lines[i++];
            }

            var first = Next("1 行目");
            if (first.Trim().Trim('"', '\'') != "MapData")
            {
                // 旧形式（SRCCore と同じく 20×20 として、1 行目から地形を読む）
                doc.Legacy = true;
                doc.Width = doc.Height = 20;
                i = 0;
            }
            else
            {
                doc.HeaderLine = first;
                doc.ReservedLine = Next("予約行");
                var size = Next("マップの大きさ").Split(',');
                if (size.Length < 2 || !TryInt(size[0], out var w) || !TryInt(size[1], out var h) || w < 1 || h < 1)
                {
                    throw new InvalidDataException($"{i} 行目: マップの大きさ（幅,高さ）が読めません");
                }
                doc.Width = w;
                doc.Height = h;
            }
            doc.Tiles = new MapTile[doc.Width, doc.Height];
            for (var x = 0; x < doc.Width; x++)
            {
                for (var y = 0; y < doc.Height; y++)
                {
                    doc.Tiles[x, y] = ParseTile(Next($"({x + 1},{y + 1}) の地形"), i);
                }
            }
            // 空行を飛ばして Layer を探す（SRCCore は次の行が Layer のときだけ読む）
            if (i < lines.Count && lines[i].Trim() == "Layer")
            {
                i++;
                doc.Layer = new MapTile[doc.Width, doc.Height];
                for (var x = 0; x < doc.Width; x++)
                {
                    for (var y = 0; y < doc.Height; y++)
                    {
                        doc.Layer[x, y] = ParseTile(Next($"({x + 1},{y + 1}) のレイヤー"), i);
                    }
                }
            }
            return doc;
        }

        private static MapTile ParseTile(string line, int lineNo)
        {
            var c = line.Split(',');
            if (c.Length < 2 || !TryInt(c[0], out var t) || !TryInt(c[1], out var b))
            {
                throw new InvalidDataException($"{lineNo} 行目: 「地形の番号,画像の番号」が読めません: {line}");
            }
            return new MapTile(t, b);
        }

        private static bool TryInt(string s, out int v) =>
            int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

        /// <summary>書き出す。旧形式で読んで大きさを変えていなければ旧形式のまま。レイヤーは中身があるか、読んだときにあれば書く</summary>
        public string Serialize()
        {
            var lines = new List<string>();
            if (!Legacy)
            {
                lines.Add(HeaderLine);
                lines.Add(ReservedLine);
                lines.Add(string.Format(CultureInfo.InvariantCulture, "{0},{1}", Width, Height));
            }
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    lines.Add(Tiles[x, y].ToString());
                }
            }
            if (Layer != null)
            {
                lines.Add("Layer");
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        lines.Add(Layer[x, y].ToString());
                    }
                }
            }
            var text = string.Join(NewLine, lines);
            return TrailingNewLine ? text + NewLine : text;
        }
    }
}
