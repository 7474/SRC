using SRCAuthoring.Analysis;
using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Templates
{
    /// <summary>試し戦闘の組（味方と敵のユニット・パイロット）</summary>
    public sealed class TrialSetup
    {
        public string Unit { get; set; }
        public string Pilot { get; set; }
        public string EnemyUnit { get; set; }
        public string EnemyPilot { get; set; }
        /// <summary>パイロットのレベル</summary>
        public int Level { get; set; } = 30;
        /// <summary>敵の数</summary>
        public int Enemies { get; set; } = 1;
    }

    /// <summary>
    /// 試し戦闘（7474/SRC#1643）。シナリオのデータのユニット・パイロットを、小さなマップで敵と戦わせるイベントを、
    /// シナリオのフォルダの .srcsharp/trial/ に作る。シナリオのフォルダ（Data のある所）で動くので、シナリオのデータをそのまま使い、
    /// デバッガのホットリロードで、データを直しながら戦える。
    /// </summary>
    public static class TrialTemplate
    {
        /// <summary>シナリオのフォルダからの、試し戦闘のフォルダ</summary>
        public const string Folder = ".srcsharp/trial";
        public const string EventName = "試し戦闘.eve";
        public const string MapName = "試し戦闘.map";

        /// <summary>省いた組を、データから選ぶ。パイロットはデータの最初の名前付き、敵のパイロットはザコか汎用（無ければ最後のパイロット）</summary>
        public static TrialSetup Complete(TrialSetup setup, DataCatalog catalog)
        {
            var units = catalog.Entries.Where(e => e.Kind == DataKind.Unit).ToList();
            var pilots = catalog.Entries.Where(e => e.Kind == DataKind.Pilot).ToList();
            bool IsGeneric(DataEntry p) => p.Name.Contains("(ザコ)") || p.Name.Contains("(汎用)");
            var result = new TrialSetup
            {
                Unit = setup.Unit ?? units.FirstOrDefault()?.Name,
                Pilot = setup.Pilot ?? pilots.FirstOrDefault(p => !IsGeneric(p))?.Name ?? pilots.FirstOrDefault()?.Name,
                Level = setup.Level,
                Enemies = Math.Max(1, Math.Min(5, setup.Enemies)),
            };
            result.EnemyUnit = setup.EnemyUnit ?? result.Unit;
            result.EnemyPilot = setup.EnemyPilot ?? pilots.FirstOrDefault(p => IsGeneric(p) && p.Name != result.Pilot)?.Name ?? pilots.LastOrDefault(p => p.Name != result.Pilot)?.Name;
            return result;
        }

        /// <summary>組が揃っていて、データにあるか。足りなければ理由</summary>
        public static string Validate(TrialSetup setup, DataCatalog catalog)
        {
            foreach (var (name, kind, label) in new[]
            {
                (setup.Unit, DataKind.Unit, "味方のユニット"),
                (setup.Pilot, DataKind.Pilot, "味方のパイロット"),
                (setup.EnemyUnit, DataKind.Unit, "敵のユニット"),
                (setup.EnemyPilot, DataKind.Pilot, "敵のパイロット（味方と別の、ザコか汎用のパイロット）"),
            })
            {
                if (string.IsNullOrEmpty(name))
                {
                    return $"{label}がありません";
                }
                if (catalog.Find(kind, name) == null)
                {
                    return $"{label}「{name}」がシナリオのデータにありません";
                }
            }
            return null;
        }

        private static string Lines(params string[] lines) => string.Join("\r\n", lines) + "\r\n";

        /// <summary>作るファイル（シナリオのフォルダからの相対パスと中身）</summary>
        public static IReadOnlyDictionary<string, string> Files(TrialSetup s)
        {
            var mapPath = Folder.Replace('/', '\\') + "\\" + MapName;
            var enemies = Enumerable.Range(0, s.Enemies).Select(i => $"Create 敵 \"{s.EnemyUnit}\" 0 \"{s.EnemyPilot}\" {s.Level} {12 + i % 2} {4 + i * 2}").ToArray();
            var eve = Lines(new[]
            {
                "# 試し戦闘（srcsharp trial・VS Code の「試し戦闘」が作る。作り直すと上書きする）",
                $"# 味方: {s.Unit}／{s.Pilot}　敵: {s.EnemyUnit}／{s.EnemyPilot} ×{s.Enemies}　レベル {s.Level}",
                "スタート:",
                $"ChangeMap {mapPath}",
                $"Create 味方 \"{s.Unit}\" 0 \"{s.Pilot}\" {s.Level} 4 6",
            }.Concat(enemies).Concat(new[]
            {
                "Talk システム",
                "試し戦闘です。データを直して保存すると、続きから読み直します（デバッガのホットリロード）。",
                "End",
                "Exit",
                "",
                "全滅 敵:",
                "Talk システム",
                "勝利",
                "End",
                "GameClear",
                "",
                "全滅 味方:",
                "Talk システム",
                "敗北",
                "End",
                "GameOver",
            }).ToArray());
            var map = new MapDocument(16, 11, new MapTile(0, 0));
            return new Dictionary<string, string>
            {
                [Folder + "/" + EventName] = eve,
                [Folder + "/" + MapName] = map.Serialize(),
            };
        }

        /// <summary>シナリオのフォルダに書き、イベントファイルの絶対パスを返す（いつも上書きする）</summary>
        public static string Write(string scenarioRoot, TrialSetup setup)
        {
            string eve = null;
            foreach (var (path, text) in Files(setup))
            {
                var full = Path.Combine(scenarioRoot, path.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(full));
                File.WriteAllText(full, text, new UTF8Encoding(false));
                if (path.EndsWith(".eve", StringComparison.Ordinal))
                {
                    eve = full;
                }
            }
            return eve;
        }
    }
}
