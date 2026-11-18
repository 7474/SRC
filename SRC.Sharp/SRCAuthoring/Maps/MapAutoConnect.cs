using System;
using System.Collections.Generic;

namespace SRCAuthoring.Maps
{
    /// <summary>
    /// 地形の並びと座標のハッシュから、汎用グラフィック集・basic-data に合わせたマップチップ番号を自動で選ぶ（7474/SRC#1663）。
    /// </summary>
    public static class MapAutoConnect
    {
        private static readonly Dictionary<int, string> StandardBitmaps = new Dictionary<int, string>
        {
            [0] = "plain",
            [2] = "road",
            [5] = "space",
            [8] = "moon_ground",
            [10] = "waste_land",
            [11] = "woods",
            [12] = "asteroids",
            [14] = "small_crater",
            [15] = "mountain",
            [17] = "sea",
            [19] = "river",
            [21] = "town",
            [25] = "bridge",
            [83] = "lava",
        };

        /// <summary>地形番号に対する標準の Bitmap フォルダ名（basic-data / 汎用データ）</summary>
        public static string DefaultBitmapName(int terrain) =>
            StandardBitmaps.TryGetValue(terrain, out var name) ? name : null;

        private static bool IsWater(int terrain) => terrain == 17 || terrain == 19 || terrain == 25;

        private static bool IsRoadLike(int terrain) => terrain == 2 || terrain == 25;

        private static int Clamp(int value, int min, int max) =>
            value < min ? min : (value > max ? max : value);

        private static int HashCoord(int x, int y)
        {
            unchecked
            {
                return (int)(((x * 73856093L) ^ (y * 19349663L)) & 0x7fffffffL);
            }
        }

        /// <summary>MapDocument の地形（およびレイヤー）に自動接続と変種を適用する</summary>
        public static void Apply(
            MapDocument doc,
            Func<int, string> bitmapForTerrain = null,
            Func<string, int, bool> hasImage = null)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            var (tiles, layer) = ApplyToRows(
                doc.Width,
                doc.Height,
                doc.TileRows(),
                doc.LayerRows(),
                bitmapForTerrain,
                hasImage);

            for (var y = 0; y < doc.Height; y++)
            {
                for (var x = 0; x < doc.Width; x++)
                {
                    var i = (y * doc.Width + x) * 2;
                    doc.Tiles[x, y] = new MapTile(tiles[i], tiles[i + 1]);
                    if (doc.Layer != null && layer != null)
                    {
                        doc.Layer[x, y] = new MapTile(layer[i], layer[i + 1]);
                    }
                }
            }
        }

        /// <summary>行優先の配列（tiles・layer）に対して自動接続と変種を適用した新しい配列を返す</summary>
        public static (int[] tiles, int[] layer) ApplyToRows(
            int width,
            int height,
            int[] tiles,
            int[] layer = null,
            Func<int, string> bitmapForTerrain = null,
            Func<string, int, bool> hasImage = null)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            if (tiles == null || tiles.Length != width * height * 2)
            {
                throw new ArgumentException("マスの数が大きさと合いません", nameof(tiles));
            }
            var hasLayer = layer != null && layer.Length > 0;
            if (hasLayer && layer.Length != width * height * 2)
            {
                throw new ArgumentException("レイヤーのマスの数が大きさと合いません", nameof(layer));
            }

            var outTiles = (int[])tiles.Clone();
            var outLayer = hasLayer ? (int[])layer.Clone() : null;

            int GetTileTerrain(int nx, int ny)
            {
                var cx = Clamp(nx, 0, width - 1);
                var cy = Clamp(ny, 0, height - 1);
                return tiles[(cy * width + cx) * 2];
            }

            int GetLayerOrTileTerrain(int nx, int ny)
            {
                var cx = Clamp(nx, 0, width - 1);
                var cy = Clamp(ny, 0, height - 1);
                var idx = (cy * width + cx) * 2;
                var lt = layer[idx];
                return lt != MapTile.NoLayer ? lt : tiles[idx];
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (y * width + x) * 2;
                    outTiles[i + 1] = ChooseBitmap(tiles[i], tiles[i + 1], x, y, GetTileTerrain, bitmapForTerrain, hasImage);
                    if (hasLayer && layer[i] != MapTile.NoLayer)
                    {
                        outLayer[i + 1] = ChooseBitmap(layer[i], layer[i + 1], x, y, GetLayerOrTileTerrain, bitmapForTerrain, hasImage);
                    }
                }
            }

            return (outTiles, outLayer);
        }

        private static int ChooseBitmap(
            int terrain,
            int currentBitmap,
            int x,
            int y,
            Func<int, int, int> at,
            Func<int, string> bitmapForTerrain,
            Func<string, int, bool> hasImage)
        {
            var candidates = Candidates(terrain, x, y, at);
            var bmpName = bitmapForTerrain?.Invoke(terrain) ?? DefaultBitmapName(terrain);

            if (candidates == null)
            {
                if (hasImage != null && !string.IsNullOrEmpty(bmpName) && !hasImage(bmpName, currentBitmap) && hasImage(bmpName, 0))
                {
                    return 0;
                }
                return currentBitmap;
            }

            if (hasImage == null || string.IsNullOrEmpty(bmpName))
            {
                return candidates[0];
            }

            foreach (var c in candidates)
            {
                if (hasImage(bmpName, c))
                {
                    return c;
                }
            }
            return 0;
        }

        private static int[] Candidates(int terrain, int x, int y, Func<int, int, int> at)
        {
            var hv = HashCoord(x, y);
            int n = at(x, y - 1);
            int e = at(x + 1, y);
            int s = at(x, y + 1);
            int w = at(x - 1, y);

            switch (terrain)
            {
                case 0: // plain
                    if (hv % 5 == 0)
                    {
                        return new[] { 1 + (hv / 5) % 5, 0 };
                    }
                    return new[] { 0 };

                case 2: // road
                    {
                        var mask = (IsRoadLike(n) ? 1 : 0)
                            | (IsRoadLike(e) ? 2 : 0)
                            | (IsRoadLike(s) ? 4 : 0)
                            | (IsRoadLike(w) ? 8 : 0);
                        int chip = mask switch
                        {
                            0 => 0,
                            1 => 1,
                            4 => 1,
                            5 => 1,
                            2 => 0,
                            8 => 0,
                            10 => 0,
                            12 => 2,
                            9 => 3,
                            3 => 4,
                            6 => 5,
                            14 => 6,
                            13 => 7,
                            11 => 8,
                            7 => 9,
                            15 => 10,
                            _ => 0,
                        };
                        return chip == 0 ? new[] { 0 } : new[] { chip, 0 };
                    }

                case 25: // bridge
                    if ((n == 2 || s == 2) && !(e == 2 || w == 2))
                    {
                        return new[] { 0, 2 };
                    }
                    if (e == 2 || w == 2)
                    {
                        return new[] { 501, 1, 0 };
                    }
                    if (IsRoadLike(n) || IsRoadLike(s))
                    {
                        return new[] { 0, 2 };
                    }
                    if (!IsWater(n) && !IsWater(s))
                    {
                        return new[] { 0, 2 };
                    }
                    return new[] { 501, 1, 0 };

                case 17: // sea
                    {
                        bool ln = !IsWater(n), le = !IsWater(e), ls = !IsWater(s), lw = !IsWater(w);
                        if (ln && le && ls) return new[] { 44, 4, 0 };
                        if (ln && lw) return new[] { 35, 3, 2, 0 };
                        if (ln && le) return new[] { 40, 3, 4, 0 };
                        if (ls && lw) return new[] { 38, 1, 2, 0 };
                        if (le && ls) return new[] { 43, 1, 4, 0 };
                        if (ls) return new[] { 1, 0 };
                        if (lw) return new[] { 2, 0 };
                        if (ln) return new[] { 3, 0 };
                        if (le) return new[] { 4, 0 };

                        if (!IsWater(at(x - 1, y - 1))) return new[] { 28, 0 };
                        if (!IsWater(at(x + 1, y - 1))) return new[] { 29, 0 };
                        if (!IsWater(at(x - 1, y + 1))) return new[] { 31, 0 };
                        if (!IsWater(at(x + 1, y + 1))) return new[] { 32, 0 };

                        var allWater5x5 = true;
                        for (var dy = -2; dy <= 2 && allWater5x5; dy++)
                        {
                            for (var dx = -2; dx <= 2; dx++)
                            {
                                if (!IsWater(at(x + dx, y + dy)))
                                {
                                    allWater5x5 = false;
                                    break;
                                }
                            }
                        }
                        return allWater5x5 ? new[] { 6, 0 } : new[] { 0 };
                    }

                case 19: // river
                    {
                        var mask = (IsWater(n) ? 1 : 0)
                            | (IsWater(e) ? 2 : 0)
                            | (IsWater(s) ? 4 : 0)
                            | (IsWater(w) ? 8 : 0);
                        switch (mask)
                        {
                            case 15:
                                {
                                    bool hasDiagLand = !IsWater(at(x + 1, y - 1))
                                        || !IsWater(at(x + 1, y + 1))
                                        || !IsWater(at(x - 1, y + 1))
                                        || !IsWater(at(x - 1, y - 1));
                                    return hasDiagLand ? new[] { 4, 82, 0 } : new[] { 82, 4, 0 };
                                }
                            case 10:
                                return new[] { 9 + (hv % 2), 9, 0 };
                            case 0: return new[] { 4, 82, 0 };
                            case 1: return new[] { 150, 11, 0 };
                            case 2: return new[] { 145, 9, 0 };
                            case 3: return new[] { 6, 0 };
                            case 4: return new[] { 147, 11, 0 };
                            case 5: return new[] { 11, 0 };
                            case 6: return new[] { 0 };
                            case 7: return new[] { 3, 0 };
                            case 8: return new[] { 146, 9, 0 };
                            case 9: return new[] { 8, 0 };
                            case 11: return new[] { 7, 0 };
                            case 12: return new[] { 2, 0 };
                            case 13: return new[] { 5, 0 };
                            case 14: return new[] { 1, 0 };
                            default: return new[] { 0 };
                        }
                    }

                case 11: // woods
                    {
                        bool ln = n != 11, le = e != 11, ls = s != 11, lw = w != 11;
                        int non = (ln ? 1 : 0) + (le ? 1 : 0) + (ls ? 1 : 0) + (lw ? 1 : 0);
                        if (non == 4) return new[] { 8, 0 };
                        if (non == 3 && !ln) return new[] { 7, 0 };
                        if (ls && lw) return new[] { 4, 12, 13, 0 };
                        if (ln) return new[] { 10, 0 };
                        if (le) return new[] { 11, 0 };
                        if (ls) return new[] { 12, 0 };
                        if (lw) return new[] { 13, 0 };
                        if (at(x + 1, y - 1) != 11) return new[] { 2, 0 };
                        if (at(x - 1, y - 1) != 11) return new[] { 9, 0 };
                        var r = hv % 10;
                        if (r == 0) return new[] { 14, 1, 0 };
                        if (r == 1) return new[] { 15, 1, 0 };
                        if (r < 6) return new[] { 1, 0 };
                        return new[] { 0 };
                    }

                case 15: // mountain
                    {
                        bool ln = n != 15, le = e != 15, ls = s != 15, lw = w != 15;
                        if (ln && le && ls && lw) return new[] { 49, 0 };
                        if (ln && lw) return new[] { 0 };
                        if (ls && lw) return new[] { 3, 0 };
                        if (ln) return new[] { 1, 0 };
                        if (ls) return new[] { 4, 0 };
                        if (at(x - 1, y + 1) != 15) return new[] { 2, 5, 0 };
                        if (at(x + 1, y + 1) != 15) return new[] { 5, 2, 0 };
                        return hv % 2 == 0 ? new[] { 2, 0 } : new[] { 5, 0 };
                    }

                case 10: // waste_land
                    {
                        bool ln = n != 10, le = e != 10, ls = s != 10, lw = w != 10;
                        if (ln && lw) return new[] { 17, 0 };
                        if (ln && le) return new[] { 19, 0 };
                        if (ls && lw) return new[] { 22, 0 };
                        if (ls && le) return new[] { 24, 0 };
                        if (ln) return new[] { 18, 0 };
                        if (ls) return new[] { 23, 0 };
                        if (lw) return new[] { 20, 0 };
                        if (le) return new[] { 21, 0 };
                        return new[] { 0 };
                    }

                case 83: // lava
                    {
                        bool ln = n != 83, le = e != 83, ls = s != 83, lw = w != 83;
                        if (ln && lw) return new[] { 0, 4 };
                        if (ln && le) return new[] { 2, 4, 0 };
                        if (ls && lw) return new[] { 6, 4, 0 };
                        if (ls && le) return new[] { 8, 4, 0 };
                        if (ln) return new[] { 1, 4, 0 };
                        if (ls) return new[] { 7, 4, 0 };
                        if (lw) return new[] { 3, 4, 0 };
                        if (le) return new[] { 5, 4, 0 };
                        return new[] { 4, 0 };
                    }

                case 5: // space
                    if (hv % 5 == 0)
                    {
                        return new[] { 7 + (hv % 7), hv % 7, 0 };
                    }
                    return new[] { hv % 7, 0 };

                case 12: // asteroids
                    {
                        int cnt = (n == 12 ? 1 : 0) + (e == 12 ? 1 : 0) + (s == 12 ? 1 : 0) + (w == 12 ? 1 : 0);
                        return cnt switch
                        {
                            >= 3 => new[] { 3, 2, 1, 0 },
                            2 => new[] { 2, 1, 0 },
                            1 => new[] { 1, 0 },
                            _ => new[] { 0 },
                        };
                    }

                case 21: // town
                    return new[] { hv % 3, 0 };

                case 8: // moon_ground
                    if (hv % 7 == 0)
                    {
                        return new[] { 1 + (hv / 7) % 2, 0 };
                    }
                    return new[] { 0 };

                case 14: // small_crater
                    return (hv % 3) switch
                    {
                        1 => new[] { 6, 0 },
                        2 => new[] { 7, 0 },
                        _ => new[] { 0 },
                    };

                default:
                    return null;
            }
        }
    }
}
