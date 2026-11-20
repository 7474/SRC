using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SRCAuthoring.Tests
{
    /// <summary>
    /// 汎用グラフィック集のマップチップの番号と意味（docs/assets/map-chips/*.json。7474/SRC#1663）の書式を確かめる。
    /// 絵はリポジトリに無いので、各ファイルの exists（実在する番号）に対して、範囲が番号をちょうど一度ずつ覆うことを見る
    /// </summary>
    [TestClass]
    public class MapChipCatalogTests
    {
        private static readonly HashSet<string> Kinds = new HashSet<string>
        {
            "base", "variant", "edge", "corner", "inner_corner", "strip", "end", "junction",
            "object", "style", "animation", "decoration", "placeholder", "other",
        };
        private static readonly HashSet<string> Sides = new HashSet<string> { "N", "E", "S", "W", "NE", "SE", "SW", "NW" };

        private static string Folder()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            return Path.Combine(dir.Parent.FullName, "docs", "assets", "map-chips");
        }

        private static IEnumerable<int> Expand(string ranges)
        {
            foreach (var part in ranges.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var ab = part.Split('-');
                var a = int.Parse(ab[0]);
                var b = ab.Length > 1 ? int.Parse(ab[1]) : a;
                for (var n = a; n <= b; n++)
                {
                    yield return n;
                }
            }
        }

        [TestMethod]
        public void EveryFile_CoversEachExistingNumberOnceWithValidValues()
        {
            var files = Directory.GetFiles(Folder(), "*.json");
            Assert.AreEqual(84, files.Length, "汎用グラフィック集の 84 地形");
            var errors = new List<string>();
            var total = 0;
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                using var doc = JsonDocument.Parse(File.ReadAllText(file));
                var root = doc.RootElement;
                if (root.GetProperty("terrain").GetString() != name)
                {
                    errors.Add($"{name}: terrain がファイル名と違う");
                }
                var exists = Expand(root.GetProperty("exists").GetString()).ToHashSet();
                total += exists.Count;
                if (root.GetProperty("count").GetInt32() != exists.Count)
                {
                    errors.Add($"{name}: count と exists の数が違う");
                }
                var covered = new Dictionary<int, int>();
                var last = -1;
                foreach (var r in root.GetProperty("ranges").EnumerateArray())
                {
                    var from = r.GetProperty("from").GetInt32();
                    var to = r.GetProperty("to").GetInt32();
                    if (from > to || from <= last)
                    {
                        errors.Add($"{name}: 範囲 {from}-{to} の並びか重なりがおかしい");
                    }
                    last = to;
                    var kind = r.GetProperty("kind").GetString();
                    if (!Kinds.Contains(kind))
                    {
                        errors.Add($"{name}: 範囲 {from}-{to} の kind「{kind}」");
                    }
                    if (string.IsNullOrWhiteSpace(r.GetProperty("description").GetString()))
                    {
                        errors.Add($"{name}: 範囲 {from}-{to} の description が空");
                    }
                    for (var n = from; n <= to; n++)
                    {
                        if (exists.Contains(n))
                        {
                            covered[n] = covered.GetValueOrDefault(n) + 1;
                        }
                    }
                    if (r.TryGetProperty("tiles", out var tiles))
                    {
                        foreach (var t in tiles.EnumerateObject())
                        {
                            var n = int.Parse(t.Name);
                            if (n < from || n > to || !exists.Contains(n))
                            {
                                errors.Add($"{name}: tiles の {n} が範囲 {from}-{to} の外か、実在しない番号");
                            }
                            var sides = t.Value.ValueKind == JsonValueKind.Array ? t.Value : t.Value.GetProperty("sides");
                            foreach (var s in sides.EnumerateArray())
                            {
                                if (!Sides.Contains(s.GetString()))
                                {
                                    errors.Add($"{name}: {n} の方角「{s.GetString()}」");
                                }
                            }
                        }
                    }
                }
                var missing = exists.Where(n => !covered.ContainsKey(n)).Take(5).ToList();
                if (missing.Count > 0)
                {
                    errors.Add($"{name}: 範囲に無い番号 {string.Join(",", missing)}");
                }
            }
            Assert.AreEqual(7215, total, "汎用グラフィック集のマップチップは 7,215 枚");
            Assert.AreEqual(0, errors.Count, string.Join("\n", errors.Take(30)));
        }
    }
}
