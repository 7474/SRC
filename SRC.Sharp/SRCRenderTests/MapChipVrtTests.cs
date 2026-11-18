using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCAuthoring.Maps;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// basic-data のマップチップ（一覧・SRCRender 上の描画 1x / 2x・青ユニット枠との視認性）の VRT（7474/SRC#1663）。
    /// フォント描画に依存せず実行環境ごとの差が出ないよう、カタログの文字には 3x5 ビットマップフォントを使い、
    /// ゲーム画面は MapArea（480x480）を切り出して比較する。基準画像の更新は SRC_VRT_UPDATE=1。
    /// </summary>
    [TestClass]
    public class MapChipVrtTests
    {
        private static string ContentDirectory => Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Content");
        private static string BasicDataDirectory => Path.Combine(ContentDirectory, "basic-data");

        [TestMethod]
        public void Catalog_MatchesBaseline()
        {
            var mapDir = Path.Combine(BasicDataDirectory, "Bitmap", "Map");
            var groups = Directory.GetDirectories(mapDir)
                .OrderBy(Path.GetFileName, StringComparer.Ordinal)
                .Select(dir => new
                {
                    Name = Path.GetFileName(dir),
                    Files = Directory.GetFiles(dir, "*.bmp")
                        .Select(p => new { Path = p, No = ParseTileNumber(Path.GetFileNameWithoutExtension(p), Path.GetFileName(dir)) })
                        .OrderBy(x => x.No)
                        .ThenBy(x => x.Path, StringComparer.Ordinal)
                        .ToList(),
                })
                .Where(g => g.Files.Count > 0)
                .ToList();

            const int cols = 12;
            const int tilePx = 32;
            const int gapX = 4;
            const int cellW = tilePx + gapX;
            const int cellH = tilePx + 10; // 32px + 2px + 5px(3x5数字) + 3px
            const int pad = 12;
            const int headerH = 12;

            int width = pad * 2 + cols * cellW - gapX;
            int totalHeight = pad;
            foreach (var g in groups)
            {
                int rows = (g.Files.Count + cols - 1) / cols;
                totalHeight += headerH + rows * cellH + 6;
            }
            totalHeight += pad - 6;

            using (var bmp = new SKBitmap(width, totalHeight, SKColorType.Bgra8888, SKAlphaType.Premul))
            using (var canvas = new SKCanvas(bmp))
            using (var paint = new SKPaint { IsAntialias = false })
            {
                canvas.Clear(new SKColor(28, 32, 40));
                int y = pad;
                foreach (var g in groups)
                {
                    DrawPixelText(canvas, paint, pad, y + 2, $"{g.Name} ({g.Files.Count})", new SKColor(220, 226, 236));
                    y += headerH;
                    for (int i = 0; i < g.Files.Count; i++)
                    {
                        int col = i % cols;
                        int row = i / cols;
                        int cx = pad + col * cellW;
                        int cy = y + row * cellH;
                        using (var tile = SKBitmap.Decode(g.Files[i].Path))
                        using (var tileImg = SKImage.FromBitmap(tile))
                        {
                            canvas.DrawImage(tileImg, SKRect.Create(cx, cy, tilePx, tilePx), new SKSamplingOptions(SKFilterMode.Nearest), paint);
                        }
                        string numText = g.Files[i].No >= 0 ? g.Files[i].No.ToString() : "?";
                        DrawPixelText(canvas, paint, cx + 1, cy + tilePx + 2, numText, new SKColor(170, 180, 195));
                    }
                    int rows = (g.Files.Count + cols - 1) / cols;
                    y += rows * cellH + 6;
                }

                var png = EncodePng(bmp);
                VisualAssert.MatchesBaseline(png, "map-chips-catalog");
            }
        }

        [TestMethod]
        public void Scenes_1xAnd2x_MatchBaselines()
        {
            var groundDoc = BuildGroundSceneMap();
            MapAutoConnect.Apply(groundDoc);
            var groundFull = RenderMapScreen(groundDoc, BuildGroundEvents());
            var groundMap1x = CropMapArea(groundFull);
            var groundMap2x = ScaleNearest(groundMap1x, 2);

            VisualAssert.MatchesBaseline(groundMap1x, "map-chips-scene-1x");
            VisualAssert.MatchesBaseline(groundMap2x, "map-chips-scene-2x");

            var spaceDoc = BuildSpaceSceneMap();
            MapAutoConnect.Apply(spaceDoc);
            var spaceFull = RenderMapScreen(spaceDoc, BuildSpaceEvents());
            var spaceMap1x = CropMapArea(spaceFull);
            VisualAssert.MatchesBaseline(spaceMap1x, "map-chips-space-1x");
        }

        [TestMethod]
        public void WaterAndTerrains_MaintainContrastAndColorblindSeparationWithUnitFrames()
        {
            var allyFrame = UnitFrames.Ally;
            Assert.IsNotNull(allyFrame, "UnitFrames.Ally が読み込めません");
            var allyBorderColors = SampleFrameColors(allyFrame, c => c.Blue >= 180 && c.Red <= 240);
            Assert.IsTrue(allyBorderColors.Count > 0, "UnitFrames.Ally の青枠画素が取得できません");
            var allyFrameAvg = AverageColor(allyBorderColors);
            // SRC#Studio の PARTY_COLORS['味方'] (#2f6fd6)
            var studioAlly = (R: 47.0, G: 111.0, B: 214.0);

            var waterTiles = new[]
            {
                ("sea0000", Path.Combine(BasicDataDirectory, "Bitmap", "Map", "sea", "sea0000.bmp"), false),
                ("sea0006", Path.Combine(BasicDataDirectory, "Bitmap", "Map", "sea", "sea0006.bmp"), false),
                ("river0082", Path.Combine(BasicDataDirectory, "Bitmap", "Map", "river", "river0082.bmp"), false),
                ("river0004", Path.Combine(BasicDataDirectory, "Bitmap", "Map", "river", "river0004.bmp"), true),
                ("river0011", Path.Combine(BasicDataDirectory, "Bitmap", "Map", "river", "river0011.bmp"), true),
            };

            foreach (var (label, path, centerOnly) in waterTiles)
            {
                var bg = centerOnly ? AverageBitmapColor(path, 11, 11, 21, 21) : AverageBitmapColor(path);
                // 1. 水面自体がティール（緑寄り・青抑えめ）で真っ青なユニット枠と同化しないこと
                Assert.IsTrue(bg.B <= 140.0, $"{label} の青成分が高すぎます: B={bg.B:F1}");
                Assert.IsTrue(bg.G >= bg.B * 0.78, $"{label} は青単色よりティール寄りであるべきです: G={bg.G:F1}, B={bg.B:F1}");

                // 2. SRCRender の UnitFrames.Ally との距離および輝度コントラスト比
                var wcag = ContrastRatio(allyFrameAvg, bg);
                var dist = ColorDistance(allyFrameAvg, bg);
                Assert.IsTrue(wcag >= 3.5, $"{label} と UnitFrames.Ally の輝度コントラスト不足: {wcag:F2}");
                Assert.IsTrue(dist >= 150.0, $"{label} と UnitFrames.Ally の色距離不足: {dist:F1}");

                // 3. Studio 青枠 (#2f6fd6) との色差（青チャネルおよび全体距離）
                var studioDist = ColorDistance(studioAlly, bg);
                Assert.IsTrue(studioDist >= 100.0, $"{label} と #2f6fd6 の色距離不足: {studioDist:F1}");

                // 4. 1型・2型2色覚シミュレーション後も区別できること
                foreach (var mode in new[] { "Protanopia", "Deuteranopia" })
                {
                    var simBg = SimulateCvd(bg, mode);
                    var simFrame = SimulateCvd(allyFrameAvg, mode);
                    var simStudio = SimulateCvd(studioAlly, mode);
                    Assert.IsTrue(ContrastRatio(simFrame, simBg) >= 3.2, $"{label} ({mode}) UnitFrames.Ally コントラスト不足");
                    Assert.IsTrue(ColorDistance(simFrame, simBg) >= 130.0, $"{label} ({mode}) UnitFrames.Ally 色距離不足");
                    Assert.IsTrue(ColorDistance(simStudio, simBg) >= 95.0, $"{label} ({mode}) #2f6fd6 色距離不足");
                }
            }

            // 平地・宇宙も青枠と混同しないこと
            foreach (var (label, rel) in new[] { ("plain0000", "plain/plain0000.bmp"), ("space0000", "space/space0000.bmp") })
            {
                var bg = AverageBitmapColor(Path.Combine(BasicDataDirectory, "Bitmap", "Map", rel.Replace('/', Path.DirectorySeparatorChar)));
                Assert.IsTrue(ColorDistance(allyFrameAvg, bg) >= 120.0, $"{label} と UnitFrames.Ally の色距離不足");
                Assert.IsTrue(ColorDistance(studioAlly, bg) >= 85.0, $"{label} と #2f6fd6 の色距離不足");
            }
        }

        private static MapDocument BuildGroundSceneMap()
        {
            var doc = new MapDocument(15, 15, new MapTile(0, 0));
            // 海（左下 6x7 + 湾）: (1,14) 付近は端クランプで 5x5 内すべて水となり深海 6 が出る
            for (int y = 9; y <= 15; y++)
                for (int x = 1; x <= 6; x++)
                    doc[x, y] = new MapTile(17, 0);
            doc[3, 8] = new MapTile(17, 0);
            doc[4, 8] = new MapTile(17, 0);
            doc[5, 8] = new MapTile(17, 0);

            // 川（北から曲がりつつ海へ）
            for (int y = 1; y <= 5; y++) doc[7, y] = new MapTile(19, 0);
            for (int x = 7; x <= 10; x++) doc[x, 5] = new MapTile(19, 0);
            for (int y = 5; y <= 9; y++) doc[10, y] = new MapTile(19, 0);
            for (int x = 7; x <= 10; x++) doc[x, 9] = new MapTile(19, 0);

            // 道路網と橋
            for (int x = 1; x <= 14; x++) doc[x, 3] = new MapTile(x == 7 ? 25 : 2, 0);
            for (int y = 3; y <= 13; y++) doc[12, y] = new MapTile(2, 0);
            for (int x = 9; x <= 14; x++) doc[x, 7] = new MapTile(x == 10 ? 25 : 2, 0);

            // 山（連山＋孤立峰）
            for (int y = 1; y <= 4; y++)
                for (int x = 12; x <= 15; x++)
                    doc[x, y] = new MapTile(15, 0);
            doc[9, 2] = new MapTile(15, 0);

            // 林（まとまり＋孤立）
            for (int y = 4; y <= 7; y++)
                for (int x = 1; x <= 4; x++)
                    doc[x, y] = new MapTile(11, 0);
            doc[5, 2] = new MapTile(11, 0);

            // 荒れ地・街・マグマ
            for (int y = 1; y <= 2; y++)
                for (int x = 1; x <= 3; x++)
                    doc[x, y] = new MapTile(10, 0);
            for (int y = 11; y <= 13; y++)
                for (int x = 8; x <= 9; x++)
                    doc[x, y] = new MapTile(21, 0);
            for (int y = 12; y <= 15; y++)
                for (int x = 13; x <= 15; x++)
                    doc[x, y] = new MapTile(83, 0);

            return doc;
        }

        private static string[] BuildGroundEvents() => new[]
        {
            "@テスト",
            "プロローグ:",
            "Exit",
            "",
            "スタート:",
            "ChangeMap scene.map",
            "Create 味方 陸ロボ 0 陸パイロット 5 8 8",
            "Create 味方 水戦艦 0 水パイロット 5 2 13",
            "Create 味方 沿岸艦 0 沿岸パイロット 5 4 11",
            "Create 敵 海敵 0 海敵パイロット 5 5 13",
            "Create 敵 陸敵 0 陸敵パイロット 5 13 8",
            "Create 中立 中立機 0 中立パイロット 5 9 11",
            "Exit",
        };

        private static MapDocument BuildSpaceSceneMap()
        {
            var doc = new MapDocument(15, 15, new MapTile(5, 0));
            // 暗礁空間・残骸・地球・月・コロニー・宇宙要塞
            for (int y = 2; y <= 4; y++)
                for (int x = 2; x <= 4; x++)
                    doc[x, y] = new MapTile(12, 0);
            doc[6, 3] = new MapTile(13, 0);
            doc[8, 2] = new MapTile(28, 0);
            doc[10, 2] = new MapTile(35, 0);
            doc[13, 2] = new MapTile(6, 0);
            doc[13, 5] = new MapTile(7, 0);
            doc[10, 5] = new MapTile(36, 0);

            // 下半分: 月面・クレーター・月面都市・月面基地
            for (int y = 9; y <= 15; y++)
                for (int x = 1; x <= 15; x++)
                    doc[x, y] = new MapTile(8, 0);
            foreach (var (cx, cy) in new[] { (3, 11), (5, 13), (8, 12), (12, 11), (13, 14) })
                doc[cx, cy] = new MapTile(14, 0);
            doc[6, 11] = new MapTile(24, 0);
            doc[7, 11] = new MapTile(24, 0);
            doc[10, 13] = new MapTile(62, 0);
            return doc;
        }

        private static string[] BuildSpaceEvents() => new[]
        {
            "@テスト",
            "プロローグ:",
            "Exit",
            "",
            "スタート:",
            "ChangeMap scene.map",
            "Create 味方 宇宙艦 0 宇宙パイロット 5 8 8",
            "Create 敵 宇宙敵 0 宇宙敵パイロット 5 11 4",
            "Create 味方 陸ロボ 0 月面パイロット 5 7 12",
            "Create 敵 陸敵 0 月面敵パイロット 5 11 13",
            "Exit",
        };

        private static byte[] RenderMapScreen(MapDocument doc, string[] eveLines)
        {
            var app = Path.Combine(Path.GetTempPath(), "srcs-mapchip-app-" + Guid.NewGuid());
            try
            {
                CopyDirectory(BasicDataDirectory, app);
                var testDataDir = Path.Combine(app, "Data", "テスト");
                Directory.CreateDirectory(testDataDir);
                File.WriteAllText(Path.Combine(testDataDir, "pilot.txt"), BuildTestPilots(), new System.Text.UTF8Encoding(false));
                File.WriteAllText(Path.Combine(testDataDir, "robot.txt"), BuildTestUnits(), new System.Text.UTF8Encoding(false));
                File.WriteAllText(Path.Combine(app, "scene.map"), doc.Serialize(), new System.Text.UTF8Encoding(false));
                File.WriteAllLines(Path.Combine(app, "test.eve"), eveLines, new System.Text.UTF8Encoding(false));

                using (var runner = new ScreenScenarioRunner(app))
                {
                    var script = new PlayScript(runner) { DefaultTimeout = TimeSpan.FromSeconds(10) };
                    runner.Start(Path.Combine(app, "test.eve"));
                    try
                    {
                        script.Run(new[] { "wait ready", "expect error none" });
                        using (var ms = new MemoryStream())
                        {
                            runner.GUI.SnapshotPng(ms);
                            Assert.IsNull(runner.Error, runner.Error?.ToString());
                            return ms.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        var st = runner.State;
                        var dlg = st?.Dialog;
                        var msg = st?.Message;
                        Assert.Fail($"{ex.Message}\nShowMap={st?.ShowMap} Main={st?.MainFormVisible} Title={st?.Title != null} List={st?.List?.Caption}\nDialog={dlg?.Title}:{dlg?.Text}\nMessage={string.Join("/", msg?.Lines ?? Array.Empty<string>())}\nErrors={string.Join(" | ", script.ErrorMessages)}\nLog={string.Join("\n", script.TraceLog)}\nRunnerErr={runner.Error}");
                        throw;
                    }
                }
            }
            finally
            {
                if (Directory.Exists(app))
                {
                    Directory.Delete(app, true);
                }
            }
        }

        private static string BuildTestPilots() => string.Join("\r\n", new[]
        {
            BuildPilot("水パイロット", "水兵", "汎用"),
            BuildPilot("沿岸パイロット", "沿岸兵", "汎用"),
            BuildPilot("陸パイロット", "陸兵", "汎用"),
            BuildPilot("海敵パイロット", "海敵兵", "ザコ"),
            BuildPilot("陸敵パイロット", "陸敵兵", "ザコ"),
            BuildPilot("中立パイロット", "中立兵", "汎用"),
            BuildPilot("宇宙パイロット", "宇宙兵", "汎用"),
            BuildPilot("宇宙敵パイロット", "宇宙敵兵", "ザコ"),
            BuildPilot("月面パイロット", "月面兵", "汎用"),
            BuildPilot("月面敵パイロット", "月面敵兵", "ザコ"),
        });

        private static string BuildPilot(string name, string nick, string cls) => string.Join("\r\n", new[]
        {
            name,
            $"{nick}, テスト, 男性, {cls}, AAAA, 100",
            "特殊能力なし",
            "100, 100, 100, 100, 100, 100, 普通",
            "ＳＰ, 10",
            "-.bmp, -.mid",
            "",
        });

        private static string BuildTestUnits() => string.Join("\r\n", new[]
        {
            BuildUnit("水戦艦", "水", "OBJ_SailShip01.bmp"),
            BuildUnit("沿岸艦", "水", "OBJ_SubMarine01.bmp"),
            BuildUnit("陸ロボ", "陸", "OBJ_Robot01.bmp"),
            BuildUnit("海敵", "水", "OBJ_Fish01.bmp"),
            BuildUnit("陸敵", "陸", "OBJ_Robot02.bmp"),
            BuildUnit("中立機", "陸", "OBJ_Etc01.bmp"),
            BuildUnit("宇宙艦", "宇", "OBJ_SpaceShip01.bmp"),
            BuildUnit("宇宙敵", "宇", "OBJ_UFO01.bmp"),
        });

        private static string BuildUnit(string name, string area, string bmp) => string.Join("\r\n", new[]
        {
            name,
            $"{name}, テスト, 汎用, 1, 2",
            $"{area}, 4, M, 1000, 100",
            "特殊能力なし",
            "3000, 100, 500, 80",
            $"AAAA, {bmp}",
            "===",
            "",
        });

        private static byte[] CropMapArea(byte[] fullPng)
        {
            using (var src = SKBitmap.Decode(fullPng))
            using (var srcImg = SKImage.FromBitmap(src))
            using (var dst = new SKBitmap(ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx, src.ColorType, src.AlphaType))
            using (var canvas = new SKCanvas(dst))
            {
                canvas.DrawImage(srcImg,
                    new SKRect(0, 0, ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx),
                    new SKRect(0, 0, ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx),
                    new SKSamplingOptions(SKFilterMode.Nearest));
                return EncodePng(dst);
            }
        }

        private static byte[] ScaleNearest(byte[] png, int factor)
        {
            using (var src = SKBitmap.Decode(png))
            using (var srcImg = SKImage.FromBitmap(src))
            using (var dst = new SKBitmap(src.Width * factor, src.Height * factor, src.ColorType, src.AlphaType))
            using (var canvas = new SKCanvas(dst))
            using (var paint = new SKPaint { IsAntialias = false })
            {
                canvas.DrawImage(srcImg,
                    new SKRect(0, 0, src.Width, src.Height),
                    new SKRect(0, 0, dst.Width, dst.Height),
                    new SKSamplingOptions(SKFilterMode.Nearest),
                    paint);
                return EncodePng(dst);
            }
        }

        private static byte[] EncodePng(SKBitmap bmp)
        {
            using (var image = SKImage.FromBitmap(bmp))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return data.ToArray();
            }
        }

        private static int ParseTileNumber(string stem, string prefix)
        {
            if (stem.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && int.TryParse(stem.Substring(prefix.Length), out var n))
            {
                return n;
            }
            return -1;
        }

        private static List<(double R, double G, double B)> SampleFrameColors(SKBitmap bmp, Func<SKColor, bool> pred)
        {
            var list = new List<(double, double, double)>();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.Alpha >= 200 && pred(c))
                    {
                        list.Add((c.Red, c.Green, c.Blue));
                    }
                }
            }
            return list;
        }

        private static (double R, double G, double B) AverageBitmapColor(string path, int x0 = 0, int y0 = 0, int? x1 = null, int? y1 = null)
        {
            using (var bmp = SKBitmap.Decode(path))
            {
                int maxX = x1 ?? bmp.Width;
                int maxY = y1 ?? bmp.Height;
                var list = new List<(double, double, double)>((maxX - x0) * (maxY - y0));
                for (int y = y0; y < maxY; y++)
                    for (int x = x0; x < maxX; x++)
                    {
                        var c = bmp.GetPixel(x, y);
                        list.Add((c.Red, c.Green, c.Blue));
                    }
                return AverageColor(list);
            }
        }

        private static (double R, double G, double B) AverageColor(IReadOnlyList<(double R, double G, double B)> list) =>
            (list.Average(x => x.R), list.Average(x => x.G), list.Average(x => x.B));

        private static double ColorDistance((double R, double G, double B) a, (double R, double G, double B) b)
        {
            var dr = a.R - b.R;
            var dg = a.G - b.G;
            var db = a.B - b.B;
            return Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        private static double RelativeLuminance((double R, double G, double B) c)
        {
            double Lin(double v)
            {
                v /= 255.0;
                return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            }
            return 0.2126 * Lin(c.R) + 0.7152 * Lin(c.G) + 0.0722 * Lin(c.B);
        }

        private static double ContrastRatio((double R, double G, double B) a, (double R, double G, double B) b)
        {
            var l1 = RelativeLuminance(a);
            var l2 = RelativeLuminance(b);
            return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
        }

        // Viénot, Brettel & Mollon (1999) による sRGB リニア色空間での 1型・2型2色覚シミュレーション
        private static (double R, double G, double B) SimulateCvd((double R, double G, double B) c, string mode)
        {
            double ToLin(double v)
            {
                v /= 255.0;
                return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            }
            double FromLin(double v)
            {
                v = Math.Max(0.0, Math.Min(1.0, v));
                return 255.0 * (v <= 0.0031308 ? 12.92 * v : 1.055 * Math.Pow(v, 1.0 / 2.4) - 0.055);
            }

            var r = ToLin(c.R);
            var g = ToLin(c.G);
            var b = ToLin(c.B);
            double sr, sg, sb;
            if (mode == "Protanopia")
            {
                sr = 0.152286 * r + 1.052583 * g - 0.204868 * b;
                sg = 0.114503 * r + 0.786281 * g + 0.099216 * b;
                sb = -0.003882 * r - 0.048116 * g + 1.051998 * b;
            }
            else // Deuteranopia
            {
                sr = 0.367322 * r + 0.860646 * g - 0.227968 * b;
                sg = 0.280085 * r + 0.672501 * g + 0.047413 * b;
                sb = -0.011820 * r + 0.042940 * g + 0.968881 * b;
            }
            return (FromLin(sr), FromLin(sg), FromLin(sb));
        }

        private static readonly Dictionary<char, string[]> Font3x5 = new Dictionary<char, string[]>
        {
            ['0'] = new[] { "111", "101", "101", "101", "111" },
            ['1'] = new[] { "010", "110", "010", "010", "111" },
            ['2'] = new[] { "111", "001", "111", "100", "111" },
            ['3'] = new[] { "111", "001", "111", "001", "111" },
            ['4'] = new[] { "101", "101", "111", "001", "001" },
            ['5'] = new[] { "111", "100", "111", "001", "111" },
            ['6'] = new[] { "111", "100", "111", "101", "111" },
            ['7'] = new[] { "111", "001", "001", "010", "010" },
            ['8'] = new[] { "111", "101", "111", "101", "111" },
            ['9'] = new[] { "111", "101", "111", "001", "111" },
            ['a'] = new[] { "000", "111", "101", "111", "101" },
            ['b'] = new[] { "100", "111", "101", "101", "111" },
            ['c'] = new[] { "000", "111", "100", "100", "111" },
            ['d'] = new[] { "001", "111", "101", "101", "111" },
            ['e'] = new[] { "111", "101", "111", "100", "111" },
            ['f'] = new[] { "011", "010", "111", "010", "010" },
            ['g'] = new[] { "111", "101", "111", "001", "111" },
            ['h'] = new[] { "100", "100", "111", "101", "101" },
            ['i'] = new[] { "010", "000", "010", "010", "010" },
            ['j'] = new[] { "001", "001", "001", "101", "111" },
            ['k'] = new[] { "101", "101", "110", "101", "101" },
            ['l'] = new[] { "100", "100", "100", "100", "111" },
            ['m'] = new[] { "000", "111", "111", "101", "101" },
            ['n'] = new[] { "000", "111", "101", "101", "101" },
            ['o'] = new[] { "000", "111", "101", "101", "111" },
            ['p'] = new[] { "111", "101", "111", "100", "100" },
            ['q'] = new[] { "111", "101", "111", "001", "001" },
            ['r'] = new[] { "000", "111", "100", "100", "100" },
            ['s'] = new[] { "011", "100", "111", "001", "110" },
            ['t'] = new[] { "010", "111", "010", "010", "011" },
            ['u'] = new[] { "000", "101", "101", "101", "111" },
            ['v'] = new[] { "000", "101", "101", "111", "010" },
            ['w'] = new[] { "000", "101", "111", "111", "101" },
            ['x'] = new[] { "000", "101", "010", "010", "101" },
            ['y'] = new[] { "101", "101", "111", "001", "111" },
            ['z'] = new[] { "111", "001", "010", "100", "111" },
            ['_'] = new[] { "000", "000", "000", "000", "111" },
            ['('] = new[] { "010", "100", "100", "100", "010" },
            [')'] = new[] { "010", "001", "001", "001", "010" },
            [' '] = new[] { "000", "000", "000", "000", "000" },
            ['?'] = new[] { "111", "001", "010", "000", "010" },
        };

        private static void DrawPixelText(SKCanvas canvas, SKPaint paint, int x, int y, string text, SKColor color)
        {
            paint.Color = color;
            int cx = x;
            foreach (char ch in text.ToLowerInvariant())
            {
                if (Font3x5.TryGetValue(ch, out var rows))
                {
                    for (int ry = 0; ry < 5; ry++)
                        for (int rx = 0; rx < 3; rx++)
                            if (rows[ry][rx] == '1')
                                canvas.DrawRect(cx + rx, y + ry, 1, 1, paint);
                }
                cx += 4;
            }
        }

        private static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                CopyDirectory(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }
    }
}
