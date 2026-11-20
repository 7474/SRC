using SRCAuthoring.Analysis;
using SRCAuthoring.Maps;
using SRCAuthoring.Preview;
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
        private static readonly List<(string, byte[])> placementDataFiles = new List<(string, byte[])>();
        private static PlacementUnitCatalog unitCatalog = PlacementUnitCatalog.Empty;
        private static bool unitCatalogDirty;
        private static readonly List<ImageIndex> imageRoots = new List<ImageIndex>();
        private static readonly List<AssetRoot> assetRoots = new List<AssetRoot>();
        private static readonly List<(string path, string text)> assetTargets = new List<(string path, string text)>();
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

        /// <summary>地形の並びと座標から、マップチップ番号を自動で選ぶ（7474/SRC#1663）。{ "tiles": [...], "layer": [...] | null }</summary>
        [JSExport]
        internal static string AutoConnectMap(int width, int height, int[] tiles, int[] layer, bool filterAvailable)
        {
            Func<int, string> bitmapForTerrain = null;
            Func<string, int, bool> hasImage = null;
            if (filterAvailable && imageRoots.Count > 0)
            {
                var bitmapMap = new Dictionary<int, string>();
                foreach (var t in terrains.Terrains)
                {
                    if (!string.IsNullOrEmpty(t.Bitmap))
                    {
                        bitmapMap[t.Id] = t.Bitmap;
                    }
                }
                bitmapForTerrain = id => bitmapMap.TryGetValue(id, out var b) ? b : MapAutoConnect.DefaultBitmapName(id);
                var cache = new Dictionary<(string, int), bool>();
                hasImage = (bmp, no) =>
                {
                    var key = (bmp, no);
                    if (!cache.TryGetValue(key, out var ok))
                    {
                        ok = TerrainCatalog.FindImage(bmp, no, imageRoots) != null;
                        cache[key] = ok;
                    }
                    return ok;
                };
            }
            var (outTiles, outLayer) = MapAutoConnect.ApplyToRows(
                width,
                height,
                tiles,
                layer == null || layer.Length == 0 ? null : layer,
                bitmapForTerrain,
                hasImage);
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                WriteInts(w, "tiles", outTiles);
                if (outLayer == null)
                {
                    w.WriteNull("layer");
                }
                else
                {
                    WriteInts(w, "layer", outLayer);
                }
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// データのプレビュー（7474/SRC#1643）。name・text はプレビューするデータのファイル（robot.txt・pilot.txt など）、
        /// beforeNames・beforeTexts は先に読むデータ（汎用データの sp.txt など）。結果は SRCAuthoring.Preview.DataPreview の JSON
        /// </summary>
        [JSExport]
        internal static string PreviewData(string name, string text, string[] beforeNames, string[] beforeTexts)
        {
            return DataPreview.ToJson(name, text, beforeNames.Select((n, i) => (n, beforeTexts[i])).ToList());
        }

        /// <summary>
        /// かんたん作成（7474/SRC#1659）。場面の JSON（SRCAuthoring.Templates.QuickBattleTemplate.FromJson）から、
        /// { "errors": [理由], "files": { 相対パス: テキスト } } を返す。errors があれば files は空
        /// </summary>
        [JSExport]
        internal static string BuildQuickBattle(string json)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                IReadOnlyList<string> errors;
                SRCAuthoring.Templates.QuickBattle battle = null;
                try
                {
                    battle = SRCAuthoring.Templates.QuickBattleTemplate.FromJson(json);
                    errors = SRCAuthoring.Templates.QuickBattleTemplate.Validate(battle);
                }
                catch (JsonException ex)
                {
                    errors = new[] { ex.Message };
                }
                w.WriteStartArray("errors");
                foreach (var e in errors)
                {
                    w.WriteStringValue(e);
                }
                w.WriteEndArray();
                // zip とフォルダの名前（題名からファイル名に使えない文字を除いたもの）
                w.WriteString("name", battle != null ? SRCAuthoring.Templates.QuickBattleTemplate.FileName(battle) : "");
                w.WriteStartObject("files");
                if (errors.Count == 0)
                {
                    foreach (var (path, text) in SRCAuthoring.Templates.QuickBattleTemplate.Files(battle))
                    {
                        w.WriteString(path, text);
                    }
                }
                w.WriteEndObject();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>かんたん作成のキャンペーン（話を並べたもの。7474/SRC#1691）。{ errors, name, files }</summary>
        [JSExport]
        internal static string BuildQuickCampaign(string json)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                IReadOnlyList<string> errors;
                SRCAuthoring.Templates.QuickCampaign campaign = null;
                try
                {
                    campaign = SRCAuthoring.Templates.QuickCampaignTemplate.FromJson(json);
                    errors = SRCAuthoring.Templates.QuickCampaignTemplate.Validate(campaign);
                }
                catch (JsonException ex)
                {
                    errors = new[] { ex.Message };
                }
                w.WriteStartArray("errors");
                foreach (var e in errors)
                {
                    w.WriteStringValue(e);
                }
                w.WriteEndArray();
                w.WriteString("name", campaign != null ? SRCAuthoring.Templates.QuickBattleTemplate.FileName(new SRCAuthoring.Templates.QuickBattle { Title = campaign.Title }) : "");
                w.WriteStartObject("files");
                if (errors.Count == 0)
                {
                    foreach (var (path, text) in SRCAuthoring.Templates.QuickCampaignTemplate.Files(campaign))
                    {
                        w.WriteString(path, text);
                    }
                }
                w.WriteEndObject();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>候補のファイル（相対パス）のうち、画像を探すフォルダ（足した順）にある最初のもの。"フォルダの番号:パス" か空</summary>
        [JSExport]
        internal static string FindFile(string[] candidates)
        {
            for (var r = 0; r < imageRoots.Count; r++)
            {
                foreach (var c in candidates)
                {
                    if (imageRoots[r].TryGet(c, out var actual))
                    {
                        return r + ":" + actual;
                    }
                }
            }
            return "";
        }

        /// <summary>配置表示に使うユニット・パイロットのデータファイルを空にする（7474/SRC#1653）</summary>
        [JSExport]
        internal static void ClearPlacementDataFiles()
        {
            placementDataFiles.Clear();
            unitCatalog = PlacementUnitCatalog.Empty;
            unitCatalogDirty = false;
        }

        /// <summary>配置表示に使うユニット・パイロットのデータファイル（robot.txt・unit.txt・pilot.txt）を足す</summary>
        [JSExport]
        internal static void AddPlacementDataFile(string name, byte[] data)
        {
            placementDataFiles.Add((name, data));
            unitCatalogDirty = true;
        }

        private static PlacementUnitCatalog EnsureUnitCatalog()
        {
            if (unitCatalogDirty)
            {
                unitCatalog = PlacementUnitCatalog.LoadBytes(placementDataFiles);
                unitCatalogDirty = false;
            }
            return unitCatalog;
        }

        /// <summary>指定したマップ（targetMapPath）に対応するイベントファイルの配置一覧 JSON を返す</summary>
        [JSExport]
        internal static string ParsePlacements(string targetMapPath, string[] eventPaths, string[] eventTexts)
        {
            var events = new List<(string path, string text)>(eventPaths?.Length ?? 0);
            if (eventPaths != null && eventTexts != null)
            {
                var n = Math.Min(eventPaths.Length, eventTexts.Length);
                for (var i = 0; i < n; i++)
                {
                    events.Add((eventPaths[i], eventTexts[i]));
                }
            }
            var items = EventPlacementParser.ParseForMap(targetMapPath, events, EnsureUnitCatalog());
            return EventPlacementParser.ToJson(items);
        }

        /// <summary>イベント 1 行の座標だけを newX・newY に書き換える</summary>
        [JSExport]
        internal static string ReplacePlacementCoordinates(string line, int newX, int newY)
        {
            return EventPlacementParser.ReplaceLineCoordinates(line, newX, newY);
        }

        /// <summary>イベントテキスト全体の指定行（1 始まり）の座標だけを newX・newY に書き換える</summary>
        [JSExport]
        internal static string ReplacePlacementFileCoordinates(string text, int lineNumber, int newX, int newY)
        {
            return EventPlacementParser.ReplaceFileCoordinates(text, lineNumber, newX, newY);
        }

        /// <summary>移動力の範囲の目安を計算する（到達可能な 1 始まり座標 [x1, y1, x2, y2, ...]）</summary>
        [JSExport]
        internal static int[] ComputeMoveRange(
            int width, int height, int[] tiles, int[] layer,
            int startX, int startY, int speed, string transportation, string adaption)
        {
            return EventPlacementParser.ComputeMoveRange(
                width, height, tiles,
                layer == null || layer.Length == 0 ? null : layer,
                terrains, startX, startY, speed, transportation, adaption);
        }

        /// <summary>人形劇の編集（7474/SRC#1694）。イベントの塊（ラベル）の一覧 JSON</summary>
        [JSExport]
        internal static string TimelineBlocks(string eventText)
        {
            return EventTimeline.BlocksToJson(EventTimeline.Blocks(eventText));
        }

        /// <summary>
        /// 人形劇の編集（7474/SRC#1694）。line（1 始まり）を含む塊を一手ずつ進めた状態の JSON。
        /// マップ（width・height・tiles。tiles は ParseMap の形）と地形データで、VB 版と同じく埋まっている・入れないマスを避けた行き先を出す
        /// </summary>
        [JSExport]
        internal static string BuildTimeline(string eventText, int line, int width, int height, int[] tiles)
        {
            TimelineMap map = null;
            if (width > 0 && height > 0 && tiles != null && tiles.Length >= width * height * 2)
            {
                var doc = MapDocument.FromRows(width, height, tiles, null, null);
                map = TimelineMap.From(doc, terrains);
            }
            return EventTimeline.StepsToJson(EventTimeline.Build(eventText, line, map, EnsureUnitCatalog()));
        }

        /// <summary>人形劇の編集（7474/SRC#1694）。after（1 始まり）の行の後に Move を差し込む。結果は { "text", "line" }</summary>
        [JSExport]
        internal static string InsertTimelineMove(string eventText, int after, string name, int x, int y)
        {
            var (text, line) = EventTimeline.InsertMove(eventText, after, name, x, y);
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                w.WriteString("text", text);
                w.WriteNumber("line", line);
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>場面の再生（7474/SRC#1694）。line を含む塊だけを流すイベントファイルの中身（塊が無ければ空）</summary>
        [JSExport]
        internal static string TimelineScene(string eventText, int line, string mapPath)
            => EventTimeline.SceneScript(eventText, line, mapPath) ?? "";

        /// <summary>素材の一覧（7474/SRC#1654）の入力を空にする</summary>
        [JSExport]
        internal static void ClearAssetReportInputs()
        {
            assetRoots.Clear();
            assetTargets.Clear();
        }

        /// <summary>素材を探す場所（探す順に scenario・extdata・extdata2・app）を足す</summary>
        [JSExport]
        internal static void AddAssetRoot(string kind, string name, string[] files)
        {
            assetRoots.Add(new AssetRoot(kind, name ?? "", files ?? Array.Empty<string>()));
        }

        /// <summary>素材を参照するファイル（*.eve や robot.txt など）の中身を足す</summary>
        [JSExport]
        internal static void AddAssetTargetFile(string path, byte[] data)
        {
            assetTargets.Add((path, AssetReferenceChecker.DecodeText(data ?? Array.Empty<byte>())));
        }

        /// <summary>素材の一覧（json・md・readme）を返す</summary>
        [JSExport]
        internal static string BuildAssetReport(string format)
        {
            var report = AssetReport.Build(assetRoots, assetTargets);
            switch ((format ?? "json").ToLowerInvariant())
            {
                case "md":
                case "markdown":
                    return report.ToMarkdown();
                case "readme":
                    return report.ToReadme();
                default:
                    return report.ToJson();
            }
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
