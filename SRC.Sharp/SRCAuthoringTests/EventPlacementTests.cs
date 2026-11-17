using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class EventPlacementTests
    {
        private static string SolutionDir()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            return dir.FullName;
        }

        [TestMethod]
        public void TrySplitWithSpans_MatchesQuotesParensAndOffsets()
        {
            Assert.IsTrue(EventPlacementParser.TrySplitWithSpans(
                "Create  味方 \"ランタン号\" 0\tグスタフ(艦長) 10  3  8", out var tokens));
            Assert.AreEqual(8, tokens.Count);
            Assert.AreEqual("Create", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].Start);
            Assert.AreEqual("グスタフ(艦長)", tokens[4].Value);
            Assert.AreEqual("3", tokens[6].Value);
            Assert.AreEqual("8", tokens[7].Value);

            var line = "Create  味方 \"ランタン号\" 0\tグスタフ(艦長) 10  3  8";
            Assert.AreEqual("3", line.Substring(tokens[6].Start, tokens[6].Length));
            Assert.AreEqual("8", line.Substring(tokens[7].Start, tokens[7].Length));

            // 括弧の中の空白は 1 つの引数、対応が取れていないときは false
            Assert.IsTrue(EventPlacementParser.TrySplitWithSpans("Launch (X(A) + 1) 5 6", out var exprTokens));
            Assert.AreEqual(4, exprTokens.Count);
            Assert.AreEqual("(X(A) + 1)", exprTokens[1].Value);
            Assert.IsFalse(EventPlacementParser.TrySplitWithSpans("Create 味方 \"unclosed", out _));
        }

        [TestMethod]
        public void ParseForMap_TracksDefaultAndChangeMapAndBlocks()
        {
            var catalog = PlacementUnitCatalog.Load(new[]
            {
                ("robot.txt", "ランタン号\nランタン号, らんたん, 戦艦, 1, 2\n空陸, 5, LL, 5000, 50\n特殊能力なし\n8000, 200, 1200, 80\nAAAA, lantern.bmp\n"),
                ("pilot.txt", "グスタフ\nグスタフ, ぐすたふ, 男性, 戦艦, AAAA, 50\n特殊能力なし\n100, 100, 100, 100, 100, 100, 普通\nＳＰ, 0\n-.bmp, -.mid\n"),
            });

            var eve1 = string.Join("\n",
                "プロローグ:",
                "Talk グスタフ",
                "Create 敵 偽物 0 敵兵 1 9 9", // Talk 内は無視
                "End",
                "スタート:",
                "Create 味方 ランタン号 0 グスタフ 10 3 8",
                "先陣 = UnitID(グスタフ)",
                "Move 先陣 6 8 アニメ表示",
                "ChangeMap ステージ\\02_霧の渓谷.map",
                "Create 敵 ザコ 0 ザコ兵 1 12 14",
                "Organize 2 5 7",
                "");

            var eve2 = string.Join("\n",
                "スタート:",
                "Launch グスタフ 4 9 非同期",
                "Launch (X(グスタフ) + 1) (Y(グスタフ) + 1)", // 式は無視
                "Exit");

            var events = new[]
            {
                ("01_灯の港.eve", eve1),
                ("ステージ/02_霧の渓谷.eve", eve2),
            };

            // 1 話のマップ: ChangeMap より前の Create と Move だけが入る
            var map1 = EventPlacementParser.ParseForMap("01_灯の港.map", events, catalog);
            Assert.AreEqual(2, map1.Count);
            Assert.AreEqual("Create", map1[0].Command);
            Assert.AreEqual("味方", map1[0].Party);
            Assert.AreEqual("ランタン号", map1[0].Unit);
            Assert.AreEqual("lantern.bmp", map1[0].Bitmap);
            Assert.AreEqual(5, map1[0].Speed);
            Assert.AreEqual(3, map1[0].X);
            Assert.AreEqual(8, map1[0].Y);

            Assert.AreEqual("Move", map1[1].Command);
            Assert.AreEqual("味方", map1[1].Party);
            Assert.AreEqual("ランタン号", map1[1].Unit);
            Assert.AreEqual(6, map1[1].X);
            Assert.AreEqual(8, map1[1].Y);
            Assert.AreEqual(3, map1[1].FromX);
            Assert.AreEqual(8, map1[1].FromY);

            // 2 話のマップ: 1 話の ChangeMap 後の Create・Organize と、2 話の cross-file Launch が入る
            var map2 = EventPlacementParser.ParseForMap("ステージ/02_霧の渓谷.map", events, catalog);
            Assert.AreEqual(3, map2.Count);
            Assert.AreEqual("01_灯の港.eve", map2[0].File);
            Assert.AreEqual("Create", map2[0].Command);
            Assert.AreEqual(12, map2[0].X);
            Assert.AreEqual(14, map2[0].Y);
            Assert.AreEqual("Organize", map2[1].Command);
            Assert.AreEqual(5, map2[1].X);
            Assert.AreEqual(7, map2[1].Y);

            Assert.AreEqual("ステージ/02_霧の渓谷.eve", map2[2].File);
            Assert.AreEqual("Launch", map2[2].Command);
            Assert.AreEqual("味方", map2[2].Party);
            Assert.AreEqual("ランタン号", map2[2].Unit);
            Assert.AreEqual("lantern.bmp", map2[2].Bitmap);
            Assert.AreEqual(4, map2[2].X);
            Assert.AreEqual(9, map2[2].Y);
        }

        [TestMethod]
        public void ParseForMap_LanternScenario_ResolvesUnitsAndCrossEpisodeLaunch()
        {
            var lantern = Path.Combine(SolutionDir(), "Content", "lantern");
            var catalog = PlacementUnitCatalog.LoadBytes(new[]
            {
                ("unit.txt", File.ReadAllBytes(Path.Combine(lantern, "Data", "ランタン", "unit.txt"))),
                ("pilot.txt", File.ReadAllBytes(Path.Combine(lantern, "Data", "ランタン", "pilot.txt"))),
                ("unit.txt", File.ReadAllBytes(Path.Combine(lantern, "Data", "煤の軍勢", "unit.txt"))),
            });
            var events = Directory.EnumerateFiles(lantern, "*.eve", SearchOption.AllDirectories)
                .Select(f => (EventPlacementParser.NormalizePath(Path.GetRelativePath(lantern, f)), File.ReadAllText(f)))
                .ToList();

            var map1 = EventPlacementParser.ParseForMap("01_灯の港.map", events, catalog);
            Assert.IsTrue(map1.Any(p => p.Command == "Create" && p.Unit == "ランタン号" && p.X == 3 && p.Y == 8));
            Assert.IsTrue(map1.Any(p => p.Command == "Move" && p.Unit == "ススガラス" && p.X == 8 && p.Y == 4 && p.FromX == 18 && p.FromY == 7));
            Assert.IsTrue(map1.Any(p => p.Command == "Create" && p.Unit == "祈り手" && p.X == 13 && p.Y == 13));

            var map2 = EventPlacementParser.ParseForMap("ステージ/02_霧の渓谷.map", events, catalog);
            var launch = map2.FirstOrDefault(p => p.Command == "Launch");
            Assert.IsNotNull(launch);
            Assert.AreEqual("味方", launch.Party);
            Assert.AreEqual("ランタン号", launch.Unit);
            Assert.IsFalse(string.IsNullOrEmpty(launch.Bitmap));
            Assert.IsTrue(map2.Any(p => p.Command == "Organize" && p.X == 5 && p.Y == 8));
        }

        [TestMethod]
        public void ReplaceCoordinates_PreservesFormattingSpacesTabsAndCrlf()
        {
            var line1 = "Create  味方 \"ランタン号\" 0\tグスタフ 10  3   8  # メモ";
            Assert.AreEqual("Create  味方 \"ランタン号\" 0\tグスタフ 10  11   15  # メモ",
                EventPlacementParser.ReplaceLineCoordinates(line1, 11, 15));

            var line2 = "  Launch グスタフ 3 8 非同期";
            Assert.AreEqual("  Launch グスタフ 12 4 非同期",
                EventPlacementParser.ReplaceLineCoordinates(line2, 12, 4));

            var line3 = "Move 10 20";
            Assert.AreEqual("Move 1 2",
                EventPlacementParser.ReplaceLineCoordinates(line3, 1, 2));

            var line4 = "Organize  2  5  8  通常ユニット";
            Assert.AreEqual("Organize  2  9  12  通常ユニット",
                EventPlacementParser.ReplaceLineCoordinates(line4, 9, 12));

            // 式が入っている場合はそのまま
            var expr = "Create 敵 ザコ 0 ザコ兵 1 $(px) 8";
            Assert.AreEqual(expr, EventPlacementParser.ReplaceLineCoordinates(expr, 5, 6));

            // CRLF を保持して指定行だけ直す
            var text = "スタート:\r\nCreate 味方 ランタン号 0 グスタフ 10 3 8\r\nExit\r\n";
            var updated = EventPlacementParser.ReplaceFileCoordinates(text, 2, 7, 14);
            Assert.AreEqual("スタート:\r\nCreate 味方 ランタン号 0 グスタフ 10 7 14\r\nExit\r\n", updated);

            // 行の続き (_) は編集しない
            var cont = "Create 味方 _\nランタン号 0 グスタフ 10 3 8";
            Assert.AreEqual(cont, EventPlacementParser.ReplaceFileCoordinates(cont, 2, 9, 9));
        }

        [TestMethod]
        public void InspectLine_FindsPlacementAuxiliaryAndEnterLabel()
        {
            var text = string.Join("\n",
                "スタート:",
                "Create 味方 ランタン号 0 グスタフ 10 3 8",
                "Center 10 12",
                "Explode L 6 7",
                "ChangeTerrain 8 9 川 0",
                "ChangeMap ステージ/02.map",
                "進入 ソラ 4 5:",
                "Exit");

            var p1 = EventPlacementParser.InspectLine("ep1.eve", text, 2);
            Assert.AreEqual("ep1.map", p1.MapPath);
            Assert.AreEqual(3, p1.X);
            Assert.AreEqual(8, p1.Y);

            var p2 = EventPlacementParser.InspectLine("ep1.eve", text, 3);
            Assert.AreEqual("Center", p2.Command);
            Assert.AreEqual(10, p2.X);
            Assert.AreEqual(12, p2.Y);

            var p3 = EventPlacementParser.InspectLine("ep1.eve", text, 4);
            Assert.AreEqual("Explode", p3.Command);
            Assert.AreEqual(6, p3.X);
            Assert.AreEqual(7, p3.Y);

            var p4 = EventPlacementParser.InspectLine("ep1.eve", text, 5);
            Assert.AreEqual("ChangeTerrain", p4.Command);
            Assert.AreEqual(8, p4.X);
            Assert.AreEqual(9, p4.Y);

            var p5 = EventPlacementParser.InspectLine("ep1.eve", text, 7);
            Assert.AreEqual("進入", p5.Command);
            Assert.AreEqual("ステージ/02.map", p5.MapPath);
            Assert.AreEqual(4, p5.X);
            Assert.AreEqual(5, p5.Y);
        }

        [TestMethod]
        public void ComputeMoveRange_RespectsTerrainCostsAndLayerAndTransportation()
        {
            var terrainBytes = Encoding.UTF8.GetBytes(string.Join("\n",
                "0\n平地, plain\n陸, 1, 0, 0\n",
                "17\n海, sea\n水, 2, 0, 0\n",
                "99\n壁, wall\n屋内, -, 0, 0\n"));
            var terrains = TerrainCatalog.Load(new[] { ("terrain.txt", terrainBytes) });

            // 4x1 のマップ: 平地・海・壁・平地。移動力 2 (コスト予算 4)
            int[] tiles = { 0, 0, 17, 0, 99, 0, 0, 0 };
            int[] layer = { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };

            // 陸ユニット (水適応 A) は 平地(コスト2) と 海(コスト4) までは届き、壁の先には行けない
            var ground = EventPlacementParser.ComputeMoveRange(4, 1, tiles, layer, terrains, 1, 1, 2, "陸", "AAAA");
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 1 }, ground);

            // 壁(99) の上にレイヤーで平地(0)を置けば奥まで届く（移動力 3 → 予算 6: 平地2+海4? いや 海コスト4 のため 1+2 で 3,4? 平地->海4 + 1?  budgets: (1,1)=0 -> (2,1)=4 -> (3,1)=6 で届く！）
            layer[4] = 0; layer[5] = 0;
            var layered = EventPlacementParser.ComputeMoveRange(4, 1, tiles, layer, terrains, 1, 1, 3, "陸", "AAAA");
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 1, 3, 1 }, layered);

            // 空ユニットも 屋内(- の壁) は飛び越せないが、空飛べるので海はコスト 2 で越せる
            layer[4] = 10000; layer[5] = 10000;
            var air = EventPlacementParser.ComputeMoveRange(4, 1, tiles, layer, terrains, 1, 1, 2, "空", "AAAA");
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 1 }, air);
        }
    }
}
