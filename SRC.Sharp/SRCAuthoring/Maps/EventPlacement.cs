using SRCCore.Lib;
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCAuthoring.Maps
{
    /// <summary>配置ツールが使うユニットの見た目と移動力の情報（unit.txt・robot.txt から読む）</summary>
    public sealed class UnitAppearance
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Class { get; set; }
        public string Bitmap { get; set; }
        public int Speed { get; set; }
        public string Transportation { get; set; }
        public string Adaption { get; set; }
    }

    /// <summary>
    /// シナリオのユニット・パイロットのデータ（robot.txt・unit.txt・pilot.txt）から、ユニットの画像・移動力・移動可能地形を引く辞書。
    /// 名前・愛称のどちらでも引ける。後のファイルが同じ名前を上書きする（SRCCore と同じ順）。
    /// </summary>
    public sealed class PlacementUnitCatalog
    {
        private readonly Dictionary<string, UnitAppearance> unitsByName = new Dictionary<string, UnitAppearance>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> pilotCanonical = new Dictionary<string, string>(StringComparer.Ordinal);

        public static readonly PlacementUnitCatalog Empty = new PlacementUnitCatalog();

        public static PlacementUnitCatalog Load(IEnumerable<(string name, string text)> files)
        {
            var src = new SRCCore.SRC { GUI = new SRCCore.TestLib.MockGUI() };
            foreach (var (name, text) in files ?? Enumerable.Empty<(string, string)>())
            {
                LoadInto(src, name, text);
            }
            return FromSrc(src);
        }

        public static PlacementUnitCatalog LoadBytes(IEnumerable<(string name, byte[] bytes)> files)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var src = new SRCCore.SRC { GUI = new SRCCore.TestLib.MockGUI() };
            foreach (var (name, bytes) in files ?? Enumerable.Empty<(string, byte[])>())
            {
                if (bytes == null)
                {
                    continue;
                }
                try
                {
                    using var stream = new MemoryStream(bytes);
                    LoadStreamInto(src, name, stream);
                }
                catch
                {
                    // 壊れたデータファイルは無視して読める分だけ使う
                }
            }
            return FromSrc(src);
        }

        private static void LoadInto(SRCCore.SRC src, string name, string text)
        {
            if (text == null)
            {
                return;
            }
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                LoadStreamInto(src, name, stream);
            }
            catch
            {
                // 壊れたデータファイルは無視して読める分だけ使う
            }
        }

        private static void LoadStreamInto(SRCCore.SRC src, string name, Stream stream)
        {
            switch (Path.GetFileName(name ?? "").ToLowerInvariant())
            {
                case "robot.txt":
                case "unit.txt":
                    src.UDList.Load(name, stream);
                    break;
                case "pilot.txt":
                    src.PDList.Load(name, stream);
                    break;
            }
        }

        public static PlacementUnitCatalog FromSrc(SRCCore.SRC src)
        {
            var catalog = new PlacementUnitCatalog();
            foreach (var u in src.UDList.Items)
            {
                var info = new UnitAppearance
                {
                    Name = u.Name ?? "",
                    Nickname = u.Nickname ?? "",
                    Class = u.Class ?? "",
                    Bitmap = u.Bitmap0 ?? "",
                    Speed = u.Speed,
                    Transportation = u.Transportation ?? "",
                    Adaption = u.Adaption ?? "",
                };
                if (!string.IsNullOrEmpty(info.Name))
                {
                    catalog.unitsByName[info.Name] = info;
                }
                if (!string.IsNullOrEmpty(info.Nickname) && !catalog.unitsByName.ContainsKey(info.Nickname))
                {
                    catalog.unitsByName[info.Nickname] = info;
                }
            }
            foreach (var p in src.PDList.Items)
            {
                if (!string.IsNullOrEmpty(p.Name))
                {
                    catalog.pilotCanonical[p.Name] = p.Name;
                    if (!string.IsNullOrEmpty(p.Nickname) && !catalog.pilotCanonical.ContainsKey(p.Nickname))
                    {
                        catalog.pilotCanonical[p.Nickname] = p.Name;
                    }
                }
            }
            return catalog;
        }

        public UnitAppearance FindUnit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return unitsByName.TryGetValue(name, out var u) ? u : null;
        }

        public string CanonicalPilot(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name ?? "";
            }
            return pilotCanonical.TryGetValue(name, out var c) ? c : name;
        }
    }

    /// <summary>行の中の 1 つのトークンと文字範囲（0 始まり）</summary>
    public readonly struct EventTokenSpan
    {
        public string Value { get; }
        public int Start { get; }
        public int Length { get; }

        public EventTokenSpan(string value, int start, int length)
        {
            Value = value;
            Start = start;
            Length = length;
        }
    }

    /// <summary>イベントファイルの 1 行にあるマップ上の配置（Create・Launch・Move・Organize など）</summary>
    public sealed class PlacementItem
    {
        public string Id { get; set; }
        public string File { get; set; }
        /// <summary>1 始まりの行番号</summary>
        public int Line { get; set; }
        public string Command { get; set; }
        /// <summary>味方・敵・中立・ＮＰＣ、または空</summary>
        public string Party { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Pilot { get; set; } = "";
        public string Bitmap { get; set; } = "";
        public int Speed { get; set; }
        public string Transportation { get; set; } = "";
        public string Adaption { get; set; } = "";
        /// <summary>1 始まりのマップ座標</summary>
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>直前のイベントラベル名（あれば）</summary>
        public string Label { get; set; } = "";
        /// <summary>同じファイル内で移動元ユニットの座標が分かる場合</summary>
        public int? FromX { get; set; }
        public int? FromY { get; set; }
        /// <summary>整数リテラルで書かれており、ドラッグで書き換えられるか</summary>
        public bool Editable { get; set; }
        /// <summary>この項目が指すマップ（シナリオ相対パス）</summary>
        public string MapPath { get; set; } = "";
    }

    /// <summary>
    /// イベントファイルの配置（Create・Launch・Move・Organize）と座標の解析・書き換え・移動力範囲の目安（7474/SRC#1653）。
    /// SRC#Studio（WebAssembly）と Language Server（VS Code）が共通で使う。
    /// </summary>
    public static class EventPlacementParser
    {
        private static readonly string[] OptionKeywords = { "非同期", "アニメ非表示", "アニメ表示" };

        public static string NormalizePath(string path)
        {
            var p = (path ?? "").Replace('\\', '/').Trim();
            while (p.StartsWith("./", StringComparison.Ordinal))
            {
                p = p.Substring(2);
            }
            return p.TrimStart('/');
        }

        public static bool SamePath(string a, string b)
            => string.Equals(NormalizePath(a), NormalizePath(b), StringComparison.OrdinalIgnoreCase);

        public static string Unquote(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return "";
            }
            if (token.Length >= 2 && ((token[0] == '"' && token[^1] == '"') || (token[0] == '`' && token[^1] == '`')))
            {
                return token.Substring(1, token.Length - 2);
            }
            return token;
        }

        /// <summary>
        /// GeneralLib.ListSplit と同じ規則でトークンを切り出し、元の文字列内の開始位置と長さも返す。
        /// 括弧・クォートの対応が取れていない場合は false を返す。
        /// </summary>
        public static bool TrySplitWithSpans(string line, out List<EventTokenSpan> tokens)
        {
            tokens = new List<EventTokenSpan>();
            if (string.IsNullOrEmpty(line))
            {
                return true;
            }
            var len = line.Length;
            var i = 0;
            while (true)
            {
                while (i < len && (line[i] == ' ' || line[i] == '\t'))
                {
                    i++;
                }
                if (i >= len)
                {
                    return true;
                }
                var start = i;
                var paren = 0;
                var inSingle = false;
                var inDouble = false;
                while (i < len)
                {
                    var ch = line[i];
                    if (inSingle)
                    {
                        if (ch == '`')
                        {
                            inSingle = false;
                        }
                    }
                    else if (inDouble)
                    {
                        if (ch == '"')
                        {
                            inDouble = false;
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case '(':
                            case '[':
                                paren++;
                                break;
                            case ')':
                            case ']':
                                paren--;
                                if (paren < 0)
                                {
                                    tokens.Add(new EventTokenSpan(line.Substring(start), start, len - start));
                                    return false;
                                }
                                break;
                            case '`':
                                inSingle = true;
                                break;
                            case '"':
                                inDouble = true;
                                break;
                            case ' ':
                            case '\t':
                                if (paren == 0)
                                {
                                    goto DoneToken;
                                }
                                break;
                        }
                    }
                    i++;
                }
            DoneToken:
                if (inSingle || inDouble || paren != 0)
                {
                    tokens.Add(new EventTokenSpan(line.Substring(start), start, len - start));
                    return false;
                }
                tokens.Add(new EventTokenSpan(line.Substring(start, i - start), start, i - start));
            }
        }

        private sealed class UnitContext
        {
            public Dictionary<string, (string party, string unit, string pilot, int? x, int? y)> ByKey { get; }
                = new Dictionary<string, (string, string, string, int?, int?)>(StringComparer.Ordinal);
            public Dictionary<string, string> VarToName { get; }
                = new Dictionary<string, string>(StringComparer.Ordinal);
            public (string party, string unit, string pilot)? LastSelected { get; set; }
        }

        /// <summary>全イベントの Create から「パイロット名/ユニット名 → (所属, ユニット名)」の補助辞書を作る（話数をまたぐ Launch 等の解決用）</summary>
        private static UnitContext BuildGlobalHint(IEnumerable<(string path, string text)> events, PlacementUnitCatalog catalog)
        {
            var ctx = new UnitContext();
            foreach (var (_, text) in events ?? Enumerable.Empty<(string, string)>())
            {
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }
                var lines = text.Replace("\r\n", "\n").Split('\n');
                var inBlock = false;
                for (var n = 0; n < lines.Length; n++)
                {
                    var trimmed = lines[n].Trim();
                    if (trimmed == "" || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    if (inBlock)
                    {
                        if (trimmed.Equals("End", StringComparison.OrdinalIgnoreCase))
                        {
                            inBlock = false;
                        }
                        continue;
                    }
                    if (!TrySplitWithSpans(lines[n], out var tokens) || tokens.Count == 0)
                    {
                        continue;
                    }
                    var cmd = tokens[0].Value;
                    if (IsTextBlockStart(cmd))
                    {
                        inBlock = true;
                        continue;
                    }
                    if (cmd.Equals("Create", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 8)
                    {
                        var party = NormalizeParty(Unquote(tokens[1].Value));
                        var unit = Unquote(tokens[2].Value);
                        var pilot = Unquote(tokens[4].Value);
                        if (!string.IsNullOrEmpty(pilot))
                        {
                            ctx.ByKey.TryAdd(pilot, (party, unit, pilot, null, null));
                            var canon = catalog.CanonicalPilot(pilot);
                            if (!string.IsNullOrEmpty(canon))
                            {
                                ctx.ByKey.TryAdd(canon, (party, unit, pilot, null, null));
                            }
                        }
                        if (!string.IsNullOrEmpty(unit))
                        {
                            ctx.ByKey.TryAdd(unit, (party, unit, pilot, null, null));
                        }
                    }
                }
            }
            return ctx;
        }

        /// <summary>
        /// 指定したマップ（targetMapPath、シナリオルート相対）に対応する全イベントの配置コマンドを返す。
        /// 最初の ChangeMap までは「イベントと同名の .map」、ChangeMap 以降は「指定された .map」を出しているとみなす。
        /// </summary>
        public static IReadOnlyList<PlacementItem> ParseForMap(
            string targetMapPath,
            IEnumerable<(string path, string text)> events,
            PlacementUnitCatalog catalog = null)
        {
            catalog ??= PlacementUnitCatalog.Empty;
            var eventList = (events ?? Enumerable.Empty<(string, string)>()).ToList();
            var globalHint = BuildGlobalHint(eventList, catalog);
            var target = NormalizePath(targetMapPath);
            var result = new List<PlacementItem>();

            foreach (var (rawPath, text) in eventList)
            {
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }
                var path = NormalizePath(rawPath);
                var currentMap = DefaultMapForEvent(path);
                var ctx = new UnitContext();
                var currentLabel = "";
                var lines = text.Replace("\r\n", "\n").Split('\n');
                var inBlock = false;
                var prevContinued = false;

                for (var n = 0; n < lines.Length; n++)
                {
                    var rawLine = lines[n];
                    var trimmed = rawLine.Trim();
                    var lineContinued = prevContinued || HasTrailingContinuation(rawLine);
                    prevContinued = HasTrailingContinuation(rawLine);

                    if (trimmed == "" || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    if (inBlock)
                    {
                        if (trimmed.Equals("End", StringComparison.OrdinalIgnoreCase))
                        {
                            inBlock = false;
                        }
                        continue;
                    }
                    if (trimmed.EndsWith(":", StringComparison.Ordinal) && trimmed.Length > 1)
                    {
                        currentLabel = trimmed.Substring(0, trimmed.Length - 1).Trim();
                        continue;
                    }
                    if (!TrySplitWithSpans(rawLine, out var tokens) || tokens.Count == 0)
                    {
                        continue;
                    }
                    var cmd = tokens[0].Value;
                    if (IsTextBlockStart(cmd))
                    {
                        inBlock = true;
                        continue;
                    }
                    if (cmd.Equals("ChangeMap", StringComparison.OrdinalIgnoreCase))
                    {
                        currentMap = ParseChangeMapTarget(tokens);
                        continue;
                    }
                    UpdateContext(tokens, ctx, globalHint, catalog);
                    if (currentMap == null || !SamePath(currentMap, target))
                    {
                        continue;
                    }
                    if (TryExtractPlacement(path, n + 1, tokens, currentLabel, currentMap, !lineContinued, ctx, globalHint, catalog, out var item))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 単一イベントファイルの特定行（1 始まり）にある座標コマンド・進入ラベルを調べる（VS Code ホバー・マップ確認用）。
        /// </summary>
        public static PlacementItem InspectLine(
            string eventPath,
            string eventText,
            int lineNumber,
            IEnumerable<(string path, string text)> allEvents = null,
            PlacementUnitCatalog catalog = null)
        {
            if (string.IsNullOrEmpty(eventText) || lineNumber < 1)
            {
                return null;
            }
            catalog ??= PlacementUnitCatalog.Empty;
            var path = NormalizePath(eventPath);
            var globalHint = BuildGlobalHint(allEvents, catalog);
            var currentMap = DefaultMapForEvent(path);
            var ctx = new UnitContext();
            var currentLabel = "";
            var lines = eventText.Replace("\r\n", "\n").Split('\n');
            if (lineNumber > lines.Length)
            {
                return null;
            }
            var inBlock = false;
            var prevContinued = false;

            for (var n = 0; n < lineNumber; n++)
            {
                var rawLine = lines[n];
                var trimmed = rawLine.Trim();
                var lineContinued = prevContinued || HasTrailingContinuation(rawLine);
                prevContinued = HasTrailingContinuation(rawLine);

                if (trimmed == "" || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }
                if (inBlock)
                {
                    if (trimmed.Equals("End", StringComparison.OrdinalIgnoreCase))
                    {
                        inBlock = false;
                    }
                    continue;
                }
                var isTargetLine = (n + 1) == lineNumber;
                if (trimmed.EndsWith(":", StringComparison.Ordinal) && trimmed.Length > 1)
                {
                    currentLabel = trimmed.Substring(0, trimmed.Length - 1).Trim();
                    if (isTargetLine)
                    {
                        if (TryExtractEnterLabel(path, lineNumber, rawLine, currentMap, !lineContinued, out var enterItem))
                        {
                            return enterItem;
                        }
                    }
                    continue;
                }
                if (!TrySplitWithSpans(rawLine, out var tokens) || tokens.Count == 0)
                {
                    continue;
                }
                var cmd = tokens[0].Value;
                if (IsTextBlockStart(cmd))
                {
                    inBlock = true;
                    continue;
                }
                if (cmd.Equals("ChangeMap", StringComparison.OrdinalIgnoreCase))
                {
                    currentMap = ParseChangeMapTarget(tokens);
                    continue;
                }
                UpdateContext(tokens, ctx, globalHint, catalog);
                if (isTargetLine)
                {
                    if (TryExtractPlacement(path, lineNumber, tokens, currentLabel, currentMap ?? "", !lineContinued, ctx, globalHint, catalog, out var item))
                    {
                        return item;
                    }
                    if (TryExtractAuxiliaryCoord(path, lineNumber, tokens, currentLabel, currentMap ?? "", !lineContinued, out var aux))
                    {
                        return aux;
                    }
                    return null;
                }
            }
            return null;
        }

        /// <summary>1 行の座標の欄だけを newX・newY に書き換える。座標が特定できない・編集不可なら元の行をそのまま返す</summary>
        public static string ReplaceLineCoordinates(string line, int newX, int newY)
        {
            if (string.IsNullOrEmpty(line))
            {
                return line;
            }
            if (!TryGetCoordinateTokenIndices(line, out var xIdx, out var yIdx, out var tokens))
            {
                return line;
            }
            var xSpan = tokens[xIdx];
            var ySpan = tokens[yIdx];
            if (!TryParseInt(xSpan.Value, out _) || !TryParseInt(ySpan.Value, out _))
            {
                return line;
            }
            var xStr = newX.ToString(CultureInfo.InvariantCulture);
            var yStr = newY.ToString(CultureInfo.InvariantCulture);
            // 後ろのトークンから書き換えると前の開始位置がずれない
            var sb = new StringBuilder(line);
            sb.Remove(ySpan.Start, ySpan.Length);
            sb.Insert(ySpan.Start, yStr);
            sb.Remove(xSpan.Start, xSpan.Length);
            sb.Insert(xSpan.Start, xStr);
            return sb.ToString();
        }

        /// <summary>イベントテキスト全体の指定行（1 始まり）の座標だけを書き換える（元の改行コードを保持）</summary>
        public static string ReplaceFileCoordinates(string text, int lineNumber, int newX, int newY)
        {
            if (string.IsNullOrEmpty(text) || lineNumber < 1)
            {
                return text;
            }
            var eol = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
            var lines = text.Replace("\r\n", "\n").Split('\n');
            if (lineNumber > lines.Length)
            {
                return text;
            }
            if (HasTrailingContinuation(lines[lineNumber - 1]) || (lineNumber >= 2 && HasTrailingContinuation(lines[lineNumber - 2])))
            {
                return text;
            }
            lines[lineNumber - 1] = ReplaceLineCoordinates(lines[lineNumber - 1], newX, newY);
            return string.Join(eol, lines);
        }

        /// <summary>移動力の範囲の目安を計算する（到達可能な 1 始まりのマス (x, y) の一覧、{x1, y1, ...}）</summary>
        public static int[] ComputeMoveRange(
            int width, int height, int[] tiles, int[] layer,
            TerrainCatalog terrains,
            int startX, int startY, int speed, string transportation, string adaption)
        {
            if (width <= 0 || height <= 0 || tiles == null || tiles.Length < width * height * 2
                || speed <= 0 || startX < 1 || startY < 1 || startX > width || startY > height)
            {
                return Array.Empty<int>();
            }
            var byId = (terrains?.Terrains ?? Array.Empty<TerrainInfo>()).ToDictionary(t => t.Id, t => t);
            var canFly = (transportation ?? "").Contains('空') && GetAdaptionChar(adaption, 0) != '-';
            var canGround = (transportation ?? "").Contains('陸') && GetAdaptionChar(adaption, 1) != '-';
            var canWater = (transportation ?? "").Contains('水') && GetAdaptionChar(adaption, 2) != '-';
            var canSpace = (transportation ?? "").Contains("宇宙") && GetAdaptionChar(adaption, 3) != '-';
            var budget = speed * 2;

            var costMap = new int[width, height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var idx = (y * width + x) * 2;
                    var tid = (layer != null && idx < layer.Length && layer[idx] != MapTile.NoLayer) ? layer[idx] : tiles[idx];
                    costMap[x, y] = StepCost(byId.TryGetValue(tid, out var info) ? info : null, canFly, canGround, canWater, canSpace);
                }
            }

            var dist = new int[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    dist[x, y] = int.MaxValue;
                }
            }

            var sx = startX - 1;
            var sy = startY - 1;
            dist[sx, sy] = 0;
            var queue = new PriorityQueue<(int x, int y), int>();
            queue.Enqueue((sx, sy), 0);
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.TryDequeue(out var cur, out var d))
            {
                if (d > dist[cur.x, cur.y])
                {
                    continue;
                }
                for (var dir = 0; dir < 4; dir++)
                {
                    var nx = cur.x + dx[dir];
                    var ny = cur.y + dy[dir];
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    {
                        continue;
                    }
                    var step = costMap[nx, ny];
                    if (step >= 1000)
                    {
                        continue;
                    }
                    var nd = d + step;
                    if (nd <= budget && nd < dist[nx, ny])
                    {
                        dist[nx, ny] = nd;
                        queue.Enqueue((nx, ny), nd);
                    }
                }
            }

            var coords = new List<int>();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (dist[x, y] <= budget)
                    {
                        coords.Add(x + 1);
                        coords.Add(y + 1);
                    }
                }
            }
            return coords.ToArray();
        }

        private static char GetAdaptionChar(string adaption, int index)
        {
            if (string.IsNullOrEmpty(adaption) || index >= adaption.Length)
            {
                return 'A';
            }
            return adaption[index];
        }

        private static int StepCost(TerrainInfo info, bool canFly, bool canGround, bool canWater, bool canSpace)
        {
            if (info == null)
            {
                return 2;
            }
            if (info.MoveCost == "-")
            {
                return canFly && info.Class != "屋内" ? 2 : 1000;
            }
            var baseCost = double.TryParse(info.MoveCost, NumberStyles.Float, CultureInfo.InvariantCulture, out var mc)
                ? Math.Max(1, (int)Math.Round(mc * 2.0))
                : 2;
            if (canFly)
            {
                return info.Class == "屋内" ? baseCost : 2;
            }
            switch (info.Class)
            {
                case "水":
                case "深水":
                    if (canWater)
                    {
                        return baseCost;
                    }
                    return canGround && info.Class == "水" ? Math.Max(baseCost, 4) : 1000;
                case "空":
                    return 1000;
                case "宇宙":
                    return canSpace ? baseCost : 1000;
                default:
                    return baseCost;
            }
        }

        public static string ToJson(IReadOnlyList<PlacementItem> items)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartArray();
                foreach (var item in items ?? Array.Empty<PlacementItem>())
                {
                    WritePlacementItem(w, item);
                }
                w.WriteEndArray();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static void WritePlacementItem(Utf8JsonWriter w, PlacementItem item)
        {
            w.WriteStartObject();
            w.WriteString("id", item.Id);
            w.WriteString("file", item.File);
            w.WriteNumber("line", item.Line);
            w.WriteString("command", item.Command);
            w.WriteString("party", item.Party ?? "");
            w.WriteString("unit", item.Unit ?? "");
            w.WriteString("pilot", item.Pilot ?? "");
            w.WriteString("bitmap", item.Bitmap ?? "");
            w.WriteNumber("speed", item.Speed);
            w.WriteString("transportation", item.Transportation ?? "");
            w.WriteString("adaption", item.Adaption ?? "");
            w.WriteNumber("x", item.X);
            w.WriteNumber("y", item.Y);
            w.WriteString("label", item.Label ?? "");
            if (item.FromX.HasValue && item.FromY.HasValue)
            {
                w.WriteNumber("fromX", item.FromX.Value);
                w.WriteNumber("fromY", item.FromY.Value);
            }
            w.WriteBoolean("editable", item.Editable);
            w.WriteString("mapPath", item.MapPath ?? "");
            w.WriteEndObject();
        }

        private static string DefaultMapForEvent(string eventPath)
        {
            if (string.IsNullOrEmpty(eventPath) || !eventPath.EndsWith(".eve", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return eventPath.Substring(0, eventPath.Length - 4) + ".map";
        }

        private static bool IsTextBlockStart(string cmd)
            => cmd.Equals("Talk", StringComparison.OrdinalIgnoreCase)
            || cmd.Equals("AutoTalk", StringComparison.OrdinalIgnoreCase)
            || cmd.Equals("Ask", StringComparison.OrdinalIgnoreCase)
            || cmd.Equals("Question", StringComparison.OrdinalIgnoreCase);

        private static bool HasTrailingContinuation(string rawLine)
        {
            var trimmed = rawLine.TrimEnd();
            return trimmed.EndsWith("_", StringComparison.Ordinal);
        }

        private static string ParseChangeMapTarget(List<EventTokenSpan> tokens)
        {
            if (tokens.Count < 2)
            {
                return null;
            }
            var raw = Unquote(tokens[1].Value);
            if (string.IsNullOrEmpty(raw))
            {
                return "";
            }
            if (raw.IndexOfAny(new[] { '$', '(', '[' }) >= 0)
            {
                return null;
            }
            return NormalizePath(raw);
        }

        private static void UpdateContext(List<EventTokenSpan> tokens, UnitContext ctx, UnitContext globalHint, PlacementUnitCatalog catalog)
        {
            var cmd = tokens[0].Value;
            // 代入文: 変数 = UnitID(名前) / Local 変数 = UnitID(名前) / Global 変数 = UnitID(名前)
            if (tokens.Count >= 3 && tokens[1].Value == "=")
            {
                var target = TryExtractUnitIdArg(tokens[2].Value);
                if (!string.IsNullOrEmpty(target))
                {
                    ctx.VarToName[tokens[0].Value] = target;
                }
                return;
            }
            if (tokens.Count >= 4 && tokens[2].Value == "="
                && (cmd.Equals("Local", StringComparison.OrdinalIgnoreCase) || cmd.Equals("Global", StringComparison.OrdinalIgnoreCase)))
            {
                var target = TryExtractUnitIdArg(tokens[3].Value);
                if (!string.IsNullOrEmpty(target))
                {
                    ctx.VarToName[tokens[1].Value] = target;
                }
                return;
            }
            if (cmd.Equals("Set", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 3)
            {
                var target = TryExtractUnitIdArg(tokens[2].Value);
                if (!string.IsNullOrEmpty(target))
                {
                    ctx.VarToName[tokens[1].Value] = target;
                }
                return;
            }
            if (cmd.Equals("Unit", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 2)
            {
                var unit = Unquote(tokens[1].Value);
                ctx.LastSelected = ("味方", unit, "");
                ctx.ByKey[unit] = ("味方", unit, "", null, null);
            }
        }

        private static string TryExtractUnitIdArg(string expr)
        {
            const string prefix = "UnitID(";
            if (expr != null && expr.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && expr.EndsWith(")", StringComparison.Ordinal))
            {
                return Unquote(expr.Substring(prefix.Length, expr.Length - prefix.Length - 1).Trim());
            }
            return null;
        }

        private static bool TryExtractPlacement(
            string file, int lineNo, List<EventTokenSpan> tokens, string label, string mapPath, bool editable,
            UnitContext ctx, UnitContext globalHint, PlacementUnitCatalog catalog, out PlacementItem item)
        {
            item = null;
            var cmd = tokens[0].Value;

            if (cmd.Equals("Create", StringComparison.OrdinalIgnoreCase))
            {
                if (tokens.Count < 8 || !TryParseInt(tokens[6].Value, out var x) || !TryParseInt(tokens[7].Value, out var y))
                {
                    return false;
                }
                var party = NormalizeParty(Unquote(tokens[1].Value));
                var unit = Unquote(tokens[2].Value);
                var pilot = Unquote(tokens[4].Value);
                ctx.LastSelected = (party, unit, pilot);
                ctx.ByKey[unit] = (party, unit, pilot, x, y);
                if (!string.IsNullOrEmpty(pilot))
                {
                    ctx.ByKey[pilot] = (party, unit, pilot, x, y);
                    var canon = catalog.CanonicalPilot(pilot);
                    if (!string.IsNullOrEmpty(canon))
                    {
                        ctx.ByKey[canon] = (party, unit, pilot, x, y);
                    }
                }
                var app = catalog.FindUnit(unit);
                item = new PlacementItem
                {
                    Id = $"{file}:{lineNo}",
                    File = file,
                    Line = lineNo,
                    Command = "Create",
                    Party = party,
                    Unit = unit,
                    Pilot = pilot,
                    Bitmap = app?.Bitmap ?? "",
                    Speed = app?.Speed ?? 0,
                    Transportation = app?.Transportation ?? "",
                    Adaption = app?.Adaption ?? "",
                    X = x,
                    Y = y,
                    Label = label,
                    Editable = editable,
                    MapPath = mapPath,
                };
                return true;
            }

            if (cmd.Equals("Launch", StringComparison.OrdinalIgnoreCase))
            {
                var eff = EffectiveArgCount(tokens);
                string targetName;
                int xIdx, yIdx;
                if (eff == 4)
                {
                    targetName = ResolveName(Unquote(tokens[1].Value), ctx);
                    xIdx = 2;
                    yIdx = 3;
                }
                else if (eff == 3)
                {
                    targetName = ctx.LastSelected?.pilot ?? ctx.LastSelected?.unit ?? "";
                    xIdx = 1;
                    yIdx = 2;
                }
                else
                {
                    return false;
                }
                if (!TryParseInt(tokens[xIdx].Value, out var x) || !TryParseInt(tokens[yIdx].Value, out var y))
                {
                    return false;
                }
                var resolved = ResolveUnitInfo(targetName, eff == 3, ctx, globalHint, catalog);
                ctx.LastSelected = (resolved.party, resolved.unit, resolved.pilot);
                if (!string.IsNullOrEmpty(targetName))
                {
                    ctx.ByKey[targetName] = (resolved.party, resolved.unit, resolved.pilot, x, y);
                }
                item = new PlacementItem
                {
                    Id = $"{file}:{lineNo}",
                    File = file,
                    Line = lineNo,
                    Command = "Launch",
                    Party = resolved.party,
                    Unit = string.IsNullOrEmpty(resolved.unit) ? targetName : resolved.unit,
                    Pilot = resolved.pilot,
                    Bitmap = resolved.app?.Bitmap ?? "",
                    Speed = resolved.app?.Speed ?? 0,
                    Transportation = resolved.app?.Transportation ?? "",
                    Adaption = resolved.app?.Adaption ?? "",
                    X = x,
                    Y = y,
                    Label = label,
                    Editable = editable,
                    MapPath = mapPath,
                };
                return true;
            }

            if (cmd.Equals("Move", StringComparison.OrdinalIgnoreCase))
            {
                if (tokens.Count < 3)
                {
                    return false;
                }
                string targetName;
                int xIdx, yIdx;
                bool useLast;
                if (TryParseInt(tokens[1].Value, out _))
                {
                    useLast = true;
                    targetName = ctx.LastSelected?.unit ?? ctx.LastSelected?.pilot ?? "";
                    xIdx = 1;
                    yIdx = 2;
                }
                else
                {
                    useLast = false;
                    if (tokens.Count < 4)
                    {
                        return false;
                    }
                    targetName = ResolveName(Unquote(tokens[1].Value), ctx);
                    xIdx = 2;
                    yIdx = 3;
                }
                if (yIdx >= tokens.Count || !TryParseInt(tokens[xIdx].Value, out var x) || !TryParseInt(tokens[yIdx].Value, out var y))
                {
                    return false;
                }
                var resolved = ResolveUnitInfo(targetName, useLast, ctx, globalHint, catalog);
                if (!string.IsNullOrEmpty(targetName))
                {
                    ctx.ByKey[targetName] = (resolved.party, resolved.unit, resolved.pilot, x, y);
                }
                item = new PlacementItem
                {
                    Id = $"{file}:{lineNo}",
                    File = file,
                    Line = lineNo,
                    Command = "Move",
                    Party = resolved.party,
                    Unit = string.IsNullOrEmpty(resolved.unit) ? targetName : resolved.unit,
                    Pilot = resolved.pilot,
                    Bitmap = resolved.app?.Bitmap ?? "",
                    Speed = resolved.app?.Speed ?? 0,
                    Transportation = resolved.app?.Transportation ?? "",
                    Adaption = resolved.app?.Adaption ?? "",
                    X = x,
                    Y = y,
                    FromX = resolved.fromX,
                    FromY = resolved.fromY,
                    Label = label,
                    Editable = editable,
                    MapPath = mapPath,
                };
                return true;
            }

            if (cmd.Equals("Organize", StringComparison.OrdinalIgnoreCase))
            {
                if (tokens.Count < 4 || !TryParseInt(tokens[1].Value, out var count)
                    || !TryParseInt(tokens[2].Value, out var x) || !TryParseInt(tokens[3].Value, out var y))
                {
                    return false;
                }
                item = new PlacementItem
                {
                    Id = $"{file}:{lineNo}",
                    File = file,
                    Line = lineNo,
                    Command = "Organize",
                    Party = "味方",
                    Unit = $"出撃位置 ({count})",
                    X = x,
                    Y = y,
                    Label = label,
                    Editable = editable,
                    MapPath = mapPath,
                };
                return true;
            }

            return false;
        }

        private static bool TryExtractAuxiliaryCoord(
            string file, int lineNo, List<EventTokenSpan> tokens, string label, string mapPath, bool editable, out PlacementItem item)
        {
            item = null;
            var cmd = tokens[0].Value;
            if (cmd.Equals("Center", StringComparison.OrdinalIgnoreCase))
            {
                var eff = EffectiveArgCount(tokens);
                if (eff == 3 && TryParseInt(tokens[1].Value, out var x) && TryParseInt(tokens[2].Value, out var y))
                {
                    item = new PlacementItem { Id = $"{file}:{lineNo}", File = file, Line = lineNo, Command = "Center", Unit = "Center", X = x, Y = y, Label = label, Editable = editable, MapPath = mapPath };
                    return true;
                }
            }
            else if (cmd.Equals("ChangeTerrain", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 4
                && TryParseInt(tokens[1].Value, out var tx) && TryParseInt(tokens[2].Value, out var ty))
            {
                item = new PlacementItem { Id = $"{file}:{lineNo}", File = file, Line = lineNo, Command = "ChangeTerrain", Unit = Unquote(tokens[3].Value), X = tx, Y = ty, Label = label, Editable = editable, MapPath = mapPath };
                return true;
            }
            else if (cmd.Equals("Explode", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 4
                && TryParseInt(tokens[2].Value, out var ex) && TryParseInt(tokens[3].Value, out var ey))
            {
                item = new PlacementItem { Id = $"{file}:{lineNo}", File = file, Line = lineNo, Command = "Explode", Unit = "Explode", X = ex, Y = ey, Label = label, Editable = editable, MapPath = mapPath };
                return true;
            }
            return false;
        }

        private static bool TryExtractEnterLabel(string file, int lineNo, string rawLine, string mapPath, bool editable, out PlacementItem item)
        {
            item = null;
            var body = rawLine.TrimEnd();
            if (!body.EndsWith(":", StringComparison.Ordinal))
            {
                return false;
            }
            body = body.Substring(0, body.Length - 1);
            if (!TrySplitWithSpans(body, out var tokens) || tokens.Count < 4)
            {
                return false;
            }
            if (!tokens[0].Value.Equals("進入", StringComparison.Ordinal))
            {
                return false;
            }
            if (!TryParseInt(tokens[2].Value, out var x) || !TryParseInt(tokens[3].Value, out var y))
            {
                return false;
            }
            item = new PlacementItem
            {
                Id = $"{file}:{lineNo}",
                File = file,
                Line = lineNo,
                Command = "進入",
                Unit = Unquote(tokens[1].Value),
                X = x,
                Y = y,
                Label = body.Trim(),
                Editable = editable,
                MapPath = mapPath ?? "",
            };
            return true;
        }

        private static bool TryGetCoordinateTokenIndices(string line, out int xIdx, out int yIdx, out List<EventTokenSpan> tokens)
        {
            xIdx = -1;
            yIdx = -1;
            var trimmed = line.Trim();
            if (trimmed.EndsWith(":", StringComparison.Ordinal))
            {
                if (!TrySplitWithSpans(trimmed.Substring(0, trimmed.Length - 1), out var lTokens))
                {
                    tokens = lTokens;
                    return false;
                }
                // ラベルの場合は位置がずれるので元の行から ":" の直前までを split する
                var colon = line.LastIndexOf(':');
                if (!TrySplitWithSpans(line.Substring(0, colon), out tokens))
                {
                    return false;
                }
                if (tokens.Count >= 4 && tokens[0].Value == "進入")
                {
                    xIdx = 2;
                    yIdx = 3;
                    return true;
                }
                return false;
            }

            if (!TrySplitWithSpans(line, out tokens) || tokens.Count < 3)
            {
                return false;
            }
            var cmd = tokens[0].Value;
            if (cmd.Equals("Create", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 8)
            {
                xIdx = 6;
                yIdx = 7;
                return true;
            }
            if (cmd.Equals("Launch", StringComparison.OrdinalIgnoreCase))
            {
                var eff = EffectiveArgCount(tokens);
                if (eff == 4)
                {
                    xIdx = 2;
                    yIdx = 3;
                    return true;
                }
                if (eff == 3)
                {
                    xIdx = 1;
                    yIdx = 2;
                    return true;
                }
            }
            if (cmd.Equals("Move", StringComparison.OrdinalIgnoreCase))
            {
                if (TryParseInt(tokens[1].Value, out _))
                {
                    xIdx = 1;
                    yIdx = 2;
                    return true;
                }
                if (tokens.Count >= 4)
                {
                    xIdx = 2;
                    yIdx = 3;
                    return true;
                }
            }
            if (cmd.Equals("Organize", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 4)
            {
                xIdx = 2;
                yIdx = 3;
                return true;
            }
            if (cmd.Equals("Center", StringComparison.OrdinalIgnoreCase) && EffectiveArgCount(tokens) == 3)
            {
                xIdx = 1;
                yIdx = 2;
                return true;
            }
            if (cmd.Equals("ChangeTerrain", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 4)
            {
                xIdx = 1;
                yIdx = 2;
                return true;
            }
            if (cmd.Equals("Explode", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 4)
            {
                xIdx = 2;
                yIdx = 3;
                return true;
            }
            return false;
        }

        private static int EffectiveArgCount(List<EventTokenSpan> tokens)
        {
            var count = tokens.Count;
            if (count > 1 && OptionKeywords.Any(k => k.Equals(Unquote(tokens[count - 1].Value), StringComparison.Ordinal)))
            {
                count--;
            }
            return count;
        }

        private static string ResolveName(string rawName, UnitContext ctx)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                return "";
            }
            return ctx.VarToName.TryGetValue(rawName, out var mapped) ? mapped : rawName;
        }

        private static (string party, string unit, string pilot, UnitAppearance app, int? fromX, int? fromY) ResolveUnitInfo(
            string targetName, bool useLast, UnitContext ctx, UnitContext globalHint, PlacementUnitCatalog catalog)
        {
            string party = "";
            string unit = "";
            string pilot = "";
            int? fromX = null;
            int? fromY = null;

            if (useLast && ctx.LastSelected.HasValue)
            {
                party = ctx.LastSelected.Value.party;
                unit = ctx.LastSelected.Value.unit;
                pilot = ctx.LastSelected.Value.pilot;
            }
            if (!string.IsNullOrEmpty(targetName))
            {
                if (ctx.ByKey.TryGetValue(targetName, out var hit))
                {
                    party = hit.party;
                    unit = hit.unit;
                    pilot = hit.pilot;
                    fromX = hit.x;
                    fromY = hit.y;
                }
                else if (ctx.ByKey.TryGetValue(catalog.CanonicalPilot(targetName), out hit))
                {
                    party = hit.party;
                    unit = hit.unit;
                    pilot = hit.pilot;
                    fromX = hit.x;
                    fromY = hit.y;
                }
                else if (globalHint.ByKey.TryGetValue(targetName, out hit))
                {
                    party = hit.party;
                    unit = hit.unit;
                    pilot = hit.pilot;
                }
                else if (globalHint.ByKey.TryGetValue(catalog.CanonicalPilot(targetName), out hit))
                {
                    party = hit.party;
                    unit = hit.unit;
                    pilot = hit.pilot;
                }
            }
            var app = catalog.FindUnit(unit) ?? catalog.FindUnit(targetName);
            if (app != null && string.IsNullOrEmpty(unit))
            {
                unit = app.Name;
            }
            if (string.IsNullOrEmpty(pilot) && !string.IsNullOrEmpty(targetName) && catalog.CanonicalPilot(targetName) != targetName)
            {
                pilot = targetName;
            }
            return (party, unit, pilot, app, fromX, fromY);
        }

        private static string NormalizeParty(string party)
        {
            if (string.Equals(party, "NPC", StringComparison.OrdinalIgnoreCase))
            {
                return "ＮＰＣ";
            }
            return party ?? "";
        }

        private static bool TryParseInt(string s, out int value)
            => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
