using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Config
{
    /// <summary>
    /// シナリオのフォルダにある設定ファイル（Src.ini・srcs.json）。アプリの設定（<see cref="LocalFileConfig"/>）とは別に読み書きする。
    /// <para>
    /// 元の SRC の ReadIni・WriteIni と同じく、シナリオのファイルにある項目だけを持ち、変えたらそのファイルに書き戻す。
    /// 無い項目は足さない（アプリの設定に書く）。両方のファイルにある項目は srcs.json を使い、srcs.json に書く。
    /// </para>
    /// 値は <see cref="ISystemConfig"/> のプロパティの形（真偽は true・false、画像バッファの大きさはバイト）の文字列で持つ。
    /// 型への変換は <see cref="ComposedConfig"/> が参照するときに行う。
    /// </summary>
    public class ScenarioConfig
    {
        public const string JsonFileName = "srcs.json";

        private enum Origin { Ini, Json }

        private sealed class Value
        {
            public string Text;
            public Origin Origin;
            // Src.ini の行と項目名
            public int IniLine;
            public string IniName;
            // srcs.json の値
            public JValue JsonValue;
        }

        // 元の SRC の Src.ini の [Option] の項目 → ISystemConfig のプロパティ
        private static readonly IReadOnlyDictionary<string, string> iniOptionProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SpecialPowerAnimation"] = nameof(ISystemConfig.SpecialPowerAnimation),
            ["MindEffect"] = nameof(ISystemConfig.SpecialPowerAnimation),
            ["BattleAnimation"] = nameof(ISystemConfig.BattleAnimation),
            ["ExtendedAnimation"] = nameof(ISystemConfig.ExtendedAnimation),
            ["WeaponAnimation"] = nameof(ISystemConfig.WeaponAnimation),
            ["MoveAnimation"] = nameof(ISystemConfig.MoveAnimation),
            ["Square"] = nameof(ISystemConfig.ShowSquareLine),
            ["KeepEnemyBGM"] = nameof(ISystemConfig.KeepEnemyBGM),
            ["AutoMoveCursor"] = nameof(ISystemConfig.AutoMoveCursor),
            ["KeepStretchedImage"] = nameof(ISystemConfig.KeepStretchedImage),
            ["ImageBufferNum"] = nameof(ISystemConfig.ImageBufferSize),
            ["MaxImageBufferSize"] = nameof(ISystemConfig.MaxImageBufferByteSize),
            ["MP3Volume"] = nameof(LocalFileConfig.SoundVolume),
            ["MessageWait"] = nameof(ISystemConfig.MessageWait),
        };

        // シナリオの設定では変えないもの（アプリの場所・素材の場所はシナリオから決めない）
        private static readonly ISet<string> ignoredProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(ISystemConfig.AppPath),
            nameof(ISystemConfig.ExtDataPath),
            nameof(ISystemConfig.ExtDataPath2),
        };

        private readonly Dictionary<string, Value> properties = new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Dictionary<string, Value>> sections = new Dictionary<string, Dictionary<string, Value>>(StringComparer.OrdinalIgnoreCase);

        private string iniPath;
        private string[] iniLines;
        private Encoding iniEncoding;
        private bool iniChanged;

        private string jsonPath;
        private JObject json;
        private bool jsonChanged;

        /// <summary>読んだファイル（Src.ini、srcs.json の順）</summary>
        public IReadOnlyList<string> Files => new[] { iniPath, jsonPath }.Where(x => x != null).ToList();

        /// <summary>まだファイルに書いていない変更があるか</summary>
        public bool HasChanges => iniChanged || jsonChanged;

        /// <summary>
        /// scenarioPath にある Src.ini と srcs.json を読む。どちらも無ければ null。
        /// </summary>
        public static ScenarioConfig Read(IFileSystem fs, string scenarioPath)
        {
            if (fs == null || string.IsNullOrEmpty(scenarioPath))
            {
                return null;
            }
            var config = new ScenarioConfig();
            var ini = fs.PathCombine(scenarioPath, SrcIni.FileName);
            if (fs.FileExists(ini))
            {
                config.LoadIni(ini, ReadAllBytes(fs, ini));
            }
            var json = fs.PathCombine(scenarioPath, JsonFileName);
            if (fs.FileExists(json))
            {
                config.LoadJson(json, Encoding.UTF8.GetString(ReadAllBytes(fs, json)));
            }
            return config.Files.Count > 0 ? config : null;
        }

        private static byte[] ReadAllBytes(IFileSystem fs, string path)
        {
            using (var stream = fs.Open(path))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>Src.ini の内容を読む</summary>
        public void LoadIni(string path, byte[] bytes)
        {
            iniPath = path;
            iniLines = SrcIni.SplitLines(SrcIni.Decode(bytes, out iniEncoding));
            var entries = SrcIni.Entries(iniLines).ToList();
            foreach (var entry in entries)
            {
                SetLoaded(ItemsOf(entry.Section), entry.Name, new Value
                {
                    Text = entry.Value,
                    Origin = Origin.Ini,
                    IniLine = entry.Line,
                    IniName = entry.Name,
                });
                if (!string.Equals(entry.Section, "Option", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (iniOptionProperties.TryGetValue(entry.Name, out var property) && !string.IsNullOrEmpty(entry.Value))
                {
                    // MindEffect は SpecialPowerAnimation の古い名前。両方あれば SpecialPowerAnimation を使う
                    if (string.Equals(entry.Name, "MindEffect", StringComparison.OrdinalIgnoreCase)
                        && entries.Any(x => string.Equals(x.Section, "Option", StringComparison.OrdinalIgnoreCase)
                            && string.Equals(x.Name, "SpecialPowerAnimation", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    SetLoaded(properties, property, new Value
                    {
                        Text = FromIni(property, entry.Value),
                        Origin = Origin.Ini,
                        IniLine = entry.Line,
                        IniName = entry.Name,
                    });
                }
                // 元の SRC の綴り違い
                if (string.Equals(entry.Name, "AutoDeffence", StringComparison.OrdinalIgnoreCase)
                    && !entries.Any(x => string.Equals(x.Section, "Option", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(x.Name, "AutoDefense", StringComparison.OrdinalIgnoreCase)))
                {
                    SetLoaded(ItemsOf("Option"), "AutoDefense", new Value
                    {
                        Text = entry.Value,
                        Origin = Origin.Ini,
                        IniLine = entry.Line,
                        IniName = entry.Name,
                    });
                }
            }
        }

        /// <summary>srcs.json（SRC# の設定ファイルの形）の内容を読む。Src.ini と重なる項目は srcs.json を使う</summary>
        public void LoadJson(string path, string text)
        {
            jsonPath = path;
            json = JObject.Parse(text);
            foreach (var prop in json.Properties())
            {
                if (string.Equals(prop.Name, "Sections", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var section in (prop.Value as JArray ?? new JArray()).OfType<JObject>())
                    {
                        var name = (string)section["Name"];
                        if (string.IsNullOrEmpty(name) || !(section["Items"] is JArray items))
                        {
                            continue;
                        }
                        foreach (var item in items.OfType<JObject>())
                        {
                            var itemName = (string)item["Name"];
                            if (string.IsNullOrEmpty(itemName))
                            {
                                continue;
                            }
                            if (!(item["Value"] is JValue value) || value.Type == JTokenType.Null)
                            {
                                value = new JValue("");
                                item["Value"] = value;
                            }
                            SetLoaded(ItemsOf(name), itemName, new Value { Text = (string)value, Origin = Origin.Json, JsonValue = value });
                        }
                    }
                    continue;
                }
                if (ignoredProperties.Contains(prop.Name) || !(prop.Value is JValue v))
                {
                    continue;
                }
                switch (v.Type)
                {
                    case JTokenType.Boolean:
                    case JTokenType.Integer:
                    case JTokenType.Float:
                    case JTokenType.String:
                        SetLoaded(properties, prop.Name, new Value
                        {
                            Text = v.Type == JTokenType.Boolean ? ((bool)v ? "true" : "false") : (string)v,
                            Origin = Origin.Json,
                            JsonValue = v,
                        });
                        break;
                }
            }
        }

        private Dictionary<string, Value> ItemsOf(string section)
        {
            if (!sections.TryGetValue(section, out var items))
            {
                items = new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase);
                sections[section] = items;
            }
            return items;
        }

        private static void SetLoaded(Dictionary<string, Value> values, string name, Value value)
        {
            values[name] = value;
        }

        /// <summary>シナリオで決めたプロパティの値</summary>
        public bool TryGetProperty(string name, out string value)
        {
            var found = properties.TryGetValue(name, out var v);
            value = v?.Text;
            return found;
        }

        /// <summary>シナリオで決めた項目の値</summary>
        public bool TryGetItem(string section, string name, out string value)
        {
            Value v = null;
            var found = sections.TryGetValue(section, out var items) && items.TryGetValue(name, out v);
            value = v?.Text;
            return found;
        }

        /// <summary>シナリオで決めたプロパティなら値を変えて true。決めていなければ何もせず false</summary>
        public bool TrySetProperty(string name, string value)
        {
            if (!properties.TryGetValue(name, out var v))
            {
                return false;
            }
            Update(v, value, ToIni(name, value));
            return true;
        }

        /// <summary>シナリオで決めた項目なら値を変えて true。決めていなければ何もせず false</summary>
        public bool TrySetItem(string section, string name, string value)
        {
            if (!sections.TryGetValue(section, out var items) || !items.TryGetValue(name, out var v))
            {
                return false;
            }
            Update(v, value, value);
            // Src.ini の [Option] の項目はプロパティと同じ行を指すことがある
            if (v.Origin == Origin.Ini && string.Equals(section, "Option", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var p in properties.Values.Where(x => x.Origin == Origin.Ini && x.IniLine == v.IniLine && x != v))
                {
                    p.Text = FromIni(PropertyNameOf(p), value);
                }
            }
            return true;
        }

        private string PropertyNameOf(Value value) => properties.First(x => x.Value == value).Key;

        private void Update(Value v, string value, string iniValue)
        {
            if (v.Text == value)
            {
                return;
            }
            v.Text = value;
            if (v.Origin == Origin.Ini)
            {
                var line = iniLines[v.IniLine];
                var cr = line.EndsWith("\r") ? "\r" : "";
                var indent = line.Substring(0, line.Length - line.TrimStart().Length);
                iniLines[v.IniLine] = indent + v.IniName + "=" + iniValue + cr;
                // 同じ行を指す項目（[Option] の項目とプロパティ）もそろえる
                foreach (var item in sections.Values.SelectMany(x => x.Values).Where(x => x.Origin == Origin.Ini && x.IniLine == v.IniLine && x != v))
                {
                    item.Text = iniValue;
                }
                iniChanged = true;
            }
            else
            {
                switch (v.JsonValue.Type)
                {
                    case JTokenType.Boolean:
                        v.JsonValue.Value = ComposedConfig.TryParseBool(value, out var b) ? b : (object)value;
                        break;
                    case JTokenType.Integer:
                        v.JsonValue.Value = long.TryParse(value, out var n) ? n : (object)value;
                        break;
                    default:
                        v.JsonValue.Value = value;
                        break;
                }
                jsonChanged = true;
            }
        }

        /// <summary>
        /// 変えた項目をシナリオのファイルに書く（元の SRC の WriteIni と同じ）。
        /// 書けなかったとき（読み取り専用の場所・アーカイブの中など）は例外を投げる。変えた値はこのオブジェクトに残る
        /// </summary>
        public void Save(IFileSystem fs)
        {
            if (iniChanged)
            {
                Write(fs, iniPath, iniEncoding.GetBytes(string.Join("\n", iniLines)));
                iniChanged = false;
            }
            if (jsonChanged)
            {
                Write(fs, jsonPath, new UTF8Encoding(false).GetBytes(json.ToString(Formatting.Indented)));
                jsonChanged = false;
            }
        }

        private static void Write(IFileSystem fs, string path, byte[] bytes)
        {
            using (var stream = fs.Create(path))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        // Src.ini の値 → プロパティの形
        private static string FromIni(string property, string value)
        {
            switch (property)
            {
                case nameof(ISystemConfig.ImageBufferSize):
                    // 元の SRC は 5 枚未満を 5 枚にする
                    return int.TryParse(value, out var num) ? Math.Max(5, num).ToString() : value;
                case nameof(ISystemConfig.MaxImageBufferByteSize):
                    // 元の SRC は MB で指定し、1 MB 未満を 1 MB にする
                    return double.TryParse(value, out var mb) ? ((int)(Math.Max(1d, mb) * 1024 * 1024)).ToString() : value;
                case nameof(LocalFileConfig.SoundVolume):
                    return int.TryParse(value, out var volume) ? Math.Max(0, Math.Min(100, volume)).ToString() : value;
                case nameof(ISystemConfig.MessageWait):
                    // 元の SRC は上限（手動送り）を超えたら上限にする
                    return int.TryParse(value, out var wait) ? MessageSpeeds.Normalize(wait).ToString() : value;
                default:
                    return ComposedConfig.TryParseBool(value, out var b) ? (b ? "true" : "false") : value;
            }
        }

        // プロパティの形 → Src.ini の値
        private static string ToIni(string property, string value)
        {
            switch (property)
            {
                case nameof(ISystemConfig.ImageBufferSize):
                case nameof(LocalFileConfig.SoundVolume):
                case nameof(ISystemConfig.MessageWait):
                    return value;
                case nameof(ISystemConfig.MaxImageBufferByteSize):
                    return long.TryParse(value, out var bytes) ? (bytes / 1024 / 1024).ToString() : value;
                default:
                    return ComposedConfig.TryParseBool(value, out var b) ? (b ? "On" : "Off") : value;
            }
        }
    }
}
