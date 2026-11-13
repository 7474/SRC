using SRCCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace SRCAuthoring.Rules
{
    /// <summary>
    /// ロボダン（ロボッとダンジョンＢ！）の Lib\UnitChecker.eve の観点（グループ robodun。選んだときだけ動く。すべて「情報」）（7474/SRC#1619）。
    /// ロボダンの作者がロボダンのために決めた上限と決まりで、SRC やほかの作品の基準ではない。
    /// 値と判定は UnitChecker.eve（ロボダン v13404b）のとおりにし、一般化しない。
    /// UnitChecker.eve はゲームの中のユニット 1 体（乗っているパイロット・ほかの形態）を調べるが、
    /// ここではデータだけで調べるので、すべてのユニットとパイロットを 1 つずつ調べる。
    /// 「移動可能地形に海」「移動可能地形と地形適応の食い違い」は SrcRules に、src と robodun の両方のグループで入れてある
    /// </summary>
    public static class RobodunRules
    {
        private static readonly string[] Robodun = { RuleGroups.Robodun };
        private const string Origin = "ロボダン（ロボッとダンジョンＢ！ v13404b）の Lib\\UnitChecker.eve（ロボダンの観点）";

        /// <summary>ロボダン独自の属性「延」（延長攻撃。射程が延びる近接武器）。UnitChecker.eve は射程の規則から除く</summary>
        private static bool IsExtended(WeaponData w) => w.Has("延");

        public static readonly IDataRule PilotExp = new LambdaRule(
            "robodun-pilot-exp", Robodun, DiagnosticSeverity.Information,
            "パイロットの経験値が 200 を超える（ロボダンの上限）", Origin,
            (c, r) => c.Pilots.Where(p => p.ExpValue > 200)
                .Select(p => r.AtPilot(c, p, "", $"経験値が200を超えています：{p.ExpValue}")));

        public static readonly IDataRule PilotStat = new LambdaRule(
            "robodun-pilot-stat", Robodun, DiagnosticSeverity.Information,
            "パイロットの格闘・射撃・命中・回避が 165、技量が 190、反応が 170 を超える（ロボダンの上限）", Origin,
            (c, r) => c.Pilots.SelectMany(p => new[]
            {
                ("格闘", p.Infight, 165),
                ("射撃", p.Shooting, 165),
                ("命中", p.Hit, 165),
                ("回避", p.Dodge, 165),
                ("技量", p.Technique, 190),
                ("反応", p.Intuition, 170),
            }.Where(x => x.Item2 > x.Item3).Select(x => r.AtPilot(c, p, "格闘", $"{x.Item1}値が{x.Item3}を超えています：{x.Item2}"))));

        public static readonly IDataRule PilotAttackSum = new LambdaRule(
            "robodun-pilot-attack-sum", Robodun, DiagnosticSeverity.Information,
            "パイロットの格闘と射撃の合計が 300 を超える（ロボダンの上限）", Origin,
            (c, r) => c.Pilots.Where(p => p.Infight + p.Shooting > 300)
                .Select(p => r.AtPilot(c, p, "格闘", $"格闘値と射撃値の合計が300を超えています：{p.Infight + p.Shooting}")));

        // UnitChecker.eve の SP の分け方（夢は 2 倍・クリティカルの両方に数える）
        private static readonly string[] Double = { "熱血", "奇襲", "勇気", "夢" };
        private static readonly string[] Triple = { "魂", "決意", "捨て身", "奇跡" };
        private static readonly string[] Critical = { "痛撃", "闘志", "夢" };

        public static readonly IDataRule SpecialPowerFirepower = new LambdaRule(
            "robodun-sp-firepower", Robodun, DiagnosticSeverity.Information,
            "スペシャルパワーの組み合わせで最大火力が 3 倍（熱血・奇襲・勇気・夢＋痛撃・闘志・夢）か 4.5 倍（魂・決意・捨て身・奇跡＋痛撃・闘志・夢）になる（ロボダンの決まり。ロボダンはユニットに乗る全員で見るが、ここではパイロット 1 人ずつ見る）", Origin,
            (c, r) => c.Pilots.SelectMany(p =>
            {
                var sp = p.SpecialPowerNames();
                var list = new List<SrcDiagnostic>();
                if (Critical.Any(sp.Contains))
                {
                    if (Double.Any(sp.Contains))
                    {
                        list.Add(r.AtPilot(c, p, "スペシャルパワー", "ＳＰの効果で最大火力が3倍になります：2倍＋クリティカル"));
                    }
                    if (Triple.Any(sp.Contains))
                    {
                        list.Add(r.AtPilot(c, p, "スペシャルパワー", "ＳＰの効果で最大火力が4.5倍になります：3倍＋クリティカル"));
                    }
                }
                return list;
            }));

        private static IEnumerable<SrcDiagnostic> Combos(DataRuleContext c, LambdaRule r, (string A, string AName, string B, string Message)[] combos)
            => c.Pilots.SelectMany(p =>
            {
                var sp = p.SpecialPowerNames();
                return combos.Where(x => sp.Contains(x.A) && sp.Contains(x.B))
                    .Select(x => r.AtPilot(c, p, "スペシャルパワー", $"{x.Message}：{x.AName}＋{x.B}"));
            });

        public static readonly IDataRule SpecialPowerInfinite = new LambdaRule(
            "robodun-sp-infinite", Robodun, DiagnosticSeverity.Information,
            "スペシャルパワーの組み合わせで無限コンボができる（自爆か夢＋祈りか再動。ロボダンの決まり）", Origin,
            (c, r) => Combos(c, r, new[]
            {
                ("自爆", "自爆", "祈り", "ＳＰの効果で無限コンボができます"),
                ("夢", "夢(自爆)", "祈り", "ＳＰの効果で無限コンボができます"),
                ("自爆", "自爆", "再動", "ＳＰの効果で無限コンボができます"),
                ("夢", "夢(自爆)", "再動", "ＳＰの効果で無限コンボができます"),
            }));

        public static readonly IDataRule SpecialPowerCombo = new LambdaRule(
            "robodun-sp-combo", Robodun, DiagnosticSeverity.Information,
            "スペシャルパワーの組み合わせで強力なコンボができる（集中・ひらめき・捨て身・みがわり＋忍耐、捨て身＋復活。ロボダンの決まり）", Origin,
            (c, r) => Combos(c, r, new[]
            {
                ("集中", "集中", "忍耐", "ＳＰの効果で強力なコンボができます"),
                ("ひらめき", "ひらめき", "忍耐", "ＳＰの効果で強力なコンボができます"),
                ("捨て身", "捨て身", "忍耐", "ＳＰの効果で強力なコンボができます"),
                ("捨て身", "捨て身", "復活", "ＳＰの効果で強力なコンボができます"),
                ("みがわり", "みがわり", "忍耐", "ＳＰの効果で強力なコンボができます"),
            }));

        public static readonly IDataRule UnitExp = new LambdaRule(
            "robodun-unit-exp", Robodun, DiagnosticSeverity.Information,
            "ユニットの経験値が 200 を超える（ロボダンの上限）", Origin,
            (c, r) => c.Units.Where(u => u.ExpValue > 200)
                .Select(u => r.AtUnit(c, u, "移動可能地形", $"経験値が200を超えています：{u.ExpValue}")));

        public static readonly IDataRule UnitStat = new LambdaRule(
            "robodun-unit-stat", Robodun, DiagnosticSeverity.Information,
            "ユニットの HP が 15000、EN が 300、装甲が 2500、運動性が 140、移動力が 7 を超える（ロボダンの上限）", Origin,
            (c, r) => c.Units.SelectMany(u => new[]
            {
                ("ＨＰ", u.HP, 15000, "最大ＨＰ"),
                ("ＥＮ", u.EN, 300, "最大ＨＰ"),
                ("装甲", u.Armor, 2500, "最大ＨＰ"),
                ("運動性", u.Mobility, 140, "最大ＨＰ"),
                ("移動力", u.Speed, 7, "移動可能地形"),
            }.Where(x => x.Item2 > x.Item3).Select(x => r.AtUnit(c, u, x.Item4, $"{x.Item1}が{x.Item3}を超えています：{x.Item2}"))));

        private static IEnumerable<SrcDiagnostic> WeaponRule(DataRuleContext c, LambdaRule r, System.Func<WeaponData, bool> hit, string message)
            => c.Weapons().Where(x => hit(x.Weapon)).Select(x => r.AtWeapon(c, x.Unit, x.Weapon, $"{message}：{x.Weapon.Name}"));

        public static readonly IDataRule WeaponCrossRange = new LambdaRule(
            "robodun-weapon-cross-range", Robodun, DiagnosticSeverity.Information,
            "射程 2 以上の突・武・接の武器に、格・複が無い（ロボダンの観点）", Origin,
            (c, r) => WeaponRule(c, r, w => w.MaxRange > 1 && (w.Has("突") || w.Has("武") || w.Has("接")) && !w.Has("格") && !w.Has("複"),
                "遠距離接近攻撃に格/複属性がありません"));

        public static readonly IDataRule WeaponRange2Shooting = new LambdaRule(
            "robodun-weapon-range2-shooting", Robodun, DiagnosticSeverity.Information,
            "射程 2 以上の武器に射（ロボダン独自の属性「延」の武器を除く。ロボダンの観点）", Origin,
            (c, r) => WeaponRule(c, r, w => w.MaxRange > 1 && w.Has("射") && !IsExtended(w), "遠距離攻撃に射属性があります"));

        public static readonly IDataRule WeaponRange1Infight = new LambdaRule(
            "robodun-weapon-range1-infight", Robodun, DiagnosticSeverity.Information,
            "射程 1 の武器に格（ロボダン独自の属性「延」の武器を除く。ロボダンの観点）", Origin,
            (c, r) => WeaponRule(c, r, w => w.MaxRange == 1 && w.Has("格") && !IsExtended(w), "近接攻撃に格属性があります"));

        public static readonly IDataRule WeaponRange1P = new LambdaRule(
            "robodun-weapon-range1-p", Robodun, DiagnosticSeverity.Information,
            "射程 1 の武器にＰ（ロボダン独自の属性「延」の武器を除く。ロボダンの観点）", Origin,
            (c, r) => WeaponRule(c, r, w => w.MaxRange == 1 && w.Has("Ｐ") && !IsExtended(w), "近接攻撃にＰ属性があります"));

        public static readonly IDataRule WeaponBeamWater = new LambdaRule(
            "robodun-weapon-beam-water", Robodun, DiagnosticSeverity.Information,
            "Ｂの武器の水の地形適応が A か B（ロボダンの観点）", Origin,
            (c, r) => WeaponRule(c, r, w => w.Has("Ｂ") && (RuleData.AdaptionAt(w.Adaption, 3) == "A" || RuleData.AdaptionAt(w.Adaption, 3) == "B"),
                "水適応が高いビーム兵器があります"));

        public static IEnumerable<IDataRule> All => new[]
        {
            PilotExp,
            PilotStat,
            PilotAttackSum,
            SpecialPowerFirepower,
            SpecialPowerInfinite,
            SpecialPowerCombo,
            UnitExp,
            UnitStat,
            WeaponCrossRange,
            WeaponRange2Shooting,
            WeaponRange1Infight,
            WeaponRange1P,
            WeaponBeamWater,
        };
    }
}
