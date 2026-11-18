using SRCAuthoring.Analysis;
using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Templates
{
    /// <summary>演出プレビュー用の攻撃側・防御側の組</summary>
    public sealed class AnimationPreviewSetup
    {
        public string Unit { get; set; }
        public string Pilot { get; set; }
        public string EnemyUnit { get; set; }
        public string EnemyPilot { get; set; }
        public int Level { get; set; } = 30;
    }

    /// <summary>
    /// 戦闘アニメ・特殊効果プレビュー（7474/SRC#1655）のための固定マップ・イベントを
    /// シナリオのフォルダの .srcsharp/preview/ に作る。
    /// 味方ユニット（攻撃側・対象ユニット）を (5, 6)、敵ユニット（防御側・ターゲット）を (8, 6) に配置し、
    /// 1 ターン目のマップ状態でデバッガや AnimationPreviewRunner がアニメ／特殊効果を再生できるようにする。
    /// </summary>
    public static class AnimationPreviewTemplate
    {
        public const string Folder = ".srcsharp/preview";
        public const string EventName = "演出プレビュー.eve";
        public const string MapName = "演出プレビュー.map";

        public static AnimationPreviewSetup Complete(AnimationPreviewSetup setup, DataCatalog catalog, string preferredTarget = null)
        {
            var units = catalog?.Entries.Where(e => e.Kind == DataKind.Unit).ToList() ?? new List<DataEntry>();
            var pilots = catalog?.Entries.Where(e => e.Kind == DataKind.Pilot).ToList() ?? new List<DataEntry>();
            bool IsGeneric(DataEntry p) => p.Name.Contains("(ザコ)") || p.Name.Contains("(汎用)");

            var matchedUnit = !string.IsNullOrEmpty(preferredTarget) && units.Any(u => string.Equals(u.Name, preferredTarget, StringComparison.OrdinalIgnoreCase))
                ? units.First(u => string.Equals(u.Name, preferredTarget, StringComparison.OrdinalIgnoreCase)).Name
                : null;

            var attackerUnit = !string.IsNullOrEmpty(setup?.Unit) ? setup.Unit : (matchedUnit ?? units.FirstOrDefault()?.Name ?? "__プレビュー攻撃側__");
            var attackerPilot = !string.IsNullOrEmpty(setup?.Pilot)
                ? setup.Pilot
                : (pilots.FirstOrDefault(p => !IsGeneric(p))?.Name ?? pilots.FirstOrDefault()?.Name ?? "__プレビュー攻撃側Ｐ__");
            var defenderUnit = !string.IsNullOrEmpty(setup?.EnemyUnit) ? setup.EnemyUnit : attackerUnit;
            var defenderPilot = !string.IsNullOrEmpty(setup?.EnemyPilot)
                ? setup.EnemyPilot
                : (pilots.FirstOrDefault(p => IsGeneric(p) && p.Name != attackerPilot)?.Name
                    ?? pilots.LastOrDefault(p => p.Name != attackerPilot)?.Name
                    ?? "__プレビュー防御側Ｐ__");

            return new AnimationPreviewSetup
            {
                Unit = attackerUnit,
                Pilot = attackerPilot,
                EnemyUnit = defenderUnit,
                EnemyPilot = defenderPilot,
                Level = setup?.Level > 0 ? setup.Level : 30,
            };
        }

        public static IReadOnlyDictionary<string, string> Files(AnimationPreviewSetup setup, DataCatalog catalog = null)
        {
            var s = setup ?? new AnimationPreviewSetup();
            var mapPath = Folder.Replace('/', '\\') + "\\" + MapName;

            var hasUnitsInCatalog = catalog != null
                && !string.IsNullOrEmpty(s.Unit) && catalog.Find(DataKind.Unit, s.Unit) != null
                && !string.IsNullOrEmpty(s.Pilot) && catalog.Find(DataKind.Pilot, s.Pilot) != null
                && !string.IsNullOrEmpty(s.EnemyUnit) && catalog.Find(DataKind.Unit, s.EnemyUnit) != null
                && !string.IsNullOrEmpty(s.EnemyPilot) && catalog.Find(DataKind.Pilot, s.EnemyPilot) != null;

            var body = new List<string>
            {
                "# 演出プレビュー（srcsharp anime-preview・VS Code の「戦闘アニメ・特殊効果をプレビュー」が作る。作り直すと上書きする）",
                $"# 攻撃側: {s.Unit}／{s.Pilot}　防御側: {s.EnemyUnit}／{s.EnemyPilot}",
                "スタート:",
                $"ChangeMap {mapPath}",
            };
            if (hasUnitsInCatalog)
            {
                body.Add($"Create 味方 \"{s.Unit}\" 0 \"{s.Pilot}\" {s.Level} 5 6");
                var sameNamedPilot = string.Equals(s.Pilot, s.EnemyPilot, StringComparison.Ordinal)
                    && !s.EnemyPilot.Contains("(ザコ)", StringComparison.Ordinal)
                    && !s.EnemyPilot.Contains("(汎用)", StringComparison.Ordinal);
                if (!sameNamedPilot)
                {
                    body.Add($"Create 敵 \"{s.EnemyUnit}\" 0 \"{s.EnemyPilot}\" {s.Level} 8 6");
                }
            }
            body.Add("Center 7 6");
            body.Add("Exit");

            var eve = string.Join("\r\n", body) + "\r\n";
            var map = new MapDocument(16, 11, new MapTile(0, 0));
            return new Dictionary<string, string>
            {
                [Folder + "/" + EventName] = eve,
                [Folder + "/" + MapName] = map.Serialize(),
            };
        }

        public static string Write(string scenarioRoot, AnimationPreviewSetup setup, DataCatalog catalog = null)
        {
            string eve = null;
            foreach (var (path, text) in Files(setup, catalog))
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
