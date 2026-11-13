using SRCCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace SRCAuthoring.Rules
{
    /// <summary>
    /// GSC データパックのロボ基準・拡張基準の書き方の決まり（グループ gsc-robo・gsc-ext。選んだときだけ動く）（7474/SRC#1612）。
    /// 規則と重さは docs/tools/balance-pack.md の「規則」、数値の刻みと SP は tools/pack-stats の check（データ作成ガイドライン）による
    /// </summary>
    public static class GscRules
    {
        private static readonly string[] Both = { RuleGroups.GscRobo, RuleGroups.GscExt };
        private static readonly string[] Robo = { RuleGroups.GscRobo };
        private static readonly string[] Ext = { RuleGroups.GscExt };

        private const string Guideline = "等身データ指針（GSC データパック）";

        private static IEnumerable<SrcDiagnostic> WeaponRule(DataRuleContext c, LambdaRule r, System.Func<WeaponData, bool> hit, System.Func<WeaponData, string> message)
            => c.Weapons().Where(x => !x.Weapon.IsMap() && hit(x.Weapon)).Select(x => r.AtWeapon(c, x.Unit, x.Weapon, message(x.Weapon)));

        public static readonly IDataRule WeaponRange1Infight = new LambdaRule(
            "weapon-range1-infight", Both, DiagnosticSeverity.Warning,
            "射程 1 の武器に格（射程 1 は格が無くても格闘として計算される）", Guideline,
            (c, r) => WeaponRule(c, r, w => w.MaxRange == 1 && w.Has("格"), w => $"{w.Name}は射程 1 なのに「格」があります"));

        public static readonly IDataRule WeaponRange2Shooting = new LambdaRule(
            "weapon-range2-shooting", Both, DiagnosticSeverity.Warning,
            "射程 2 以上の武器に射（射程 2 以上は射が無くても射撃として計算される）", Guideline,
            (c, r) => WeaponRule(c, r, w => w.MaxRange >= 2 && w.Has("射"), w => $"{w.Name}は射程 2 以上なのに「射」があります"));

        public static readonly IDataRule WeaponRange1P = new LambdaRule(
            "weapon-range1-p", Both, DiagnosticSeverity.Information,
            "射程 1 の武器にＰ（射程 1 の武器は書かなくても移動後に使える）", "使用可能時に関する属性（ヘルプ）",
            (c, r) => WeaponRule(c, r, w => w.MaxRange == 1 && w.Has("Ｐ"), w => $"{w.Name}は射程 1 なのに「Ｐ」があります"));

        public static readonly IDataRule WeaponCrossRangeShooting = new LambdaRule(
            "weapon-cross-range-shooting", Both, DiagnosticSeverity.Information,
            "射程 2 以上で突・武・接があり、格・複が無い（射撃として計算される）", Guideline,
            (c, r) => WeaponRule(c, r, w => w.MaxRange >= 2 && (w.Has("突") || w.Has("武") || w.Has("接")) && !w.Has("格") && !w.Has("複"),
                w => $"{w.Name}は射程 2 以上の突・武・接の武器ですが、「格」も「複」もありません（射撃として計算されます）"));

        public static readonly IDataRule WeaponBeamAdaption = new LambdaRule(
            "weapon-beam-adaption", Robo, DiagnosticSeverity.Information,
            "射程 2 以上のＢの武器で、地形適応が AA-A・AACA 以外", "ＧＳＣロボット基準データローカルルールマニュアル【３】",
            (c, r) => WeaponRule(c, r, w => w.MaxRange >= 2 && w.Has("Ｂ") && w.Adaption != "AA-A" && w.Adaption != "AACA",
                w => $"{w.Name}はＢの武器ですが、地形適応が「{w.Adaption}」です（AA-A か AACA）"));

        public static readonly IDataRule WeaponBeamMelee = new LambdaRule(
            "weapon-beam-melee", Robo, DiagnosticSeverity.Information,
            "射程 1 の武器にＢ（ビーム剣にはＢを付けない）", "ＧＳＣロボット基準データローカルルールマニュアル【３】",
            (c, r) => WeaponRule(c, r, w => w.MaxRange == 1 && w.Has("Ｂ"), w => $"{w.Name}は射程 1 なのに「Ｂ」があります"));

        public static readonly IDataRule UnitAirAdaptionNonflyer = new LambdaRule(
            "unit-air-adaption-nonflyer", Ext, DiagnosticSeverity.Information,
            "飛べないユニットの空の適応が -（拡張基準では、ジャンプ力に合わせて B〜D にする）", "等身データ指針 4",
            (c, r) => c.Units.Where(u => !(u.Transportation ?? "").Contains("空") && RuleData.AdaptionAt(u.Adaption, 1) == "-")
                .Select(u => r.AtUnit(c, u, "地形適応", $"飛べないユニットの空の適応が「-」です（地形適応「{u.Adaption}」）")));

        public static readonly IDataRule NameLength = new LambdaRule(
            "name-length", Both, DiagnosticSeverity.Information,
            "名前が長い（パイロットの愛称 10 文字・ユニットの愛称 16 文字・武器名 14 文字まで）", "等身データ指針 2",
            (c, r) => c.Pilots.Where(p => (p.Nickname ?? "").Length > 10)
                    .Select(p => r.AtPilot(c, p, "", $"愛称「{p.Nickname}」が {p.Nickname.Length} 文字です（10 文字まで）"))
                .Concat(c.Units.Where(u => (u.Nickname ?? "").Length > 16)
                    .Select(u => r.AtUnit(c, u, "", $"愛称「{u.Nickname}」が {u.Nickname.Length} 文字です（16 文字まで）")))
                .Concat(c.Weapons().Where(x => x.Weapon.Name.Length > 14)
                    .Select(x => r.AtWeapon(c, x.Unit, x.Weapon, $"武器名「{x.Weapon.Name}」が {x.Weapon.Name.Length} 文字です（14 文字まで）"))));

        public static readonly IDataRule NumberRounding = new LambdaRule(
            "number-rounding", Both, DiagnosticSeverity.Information,
            "数値の刻み（HP 100・EN 10・装甲 50・運動性 5・武器の攻撃力 100・消費 EN 5）に合わない", "データ作成ガイドライン 1.2 数値の刻み（SRC-Data）",
            (c, r) => c.Units.SelectMany(u =>
                new[]
                {
                    (u.HP % 100 != 0, $"HP {u.HP} が 100 刻みではありません"),
                    (u.EN % 10 != 0, $"EN {u.EN} が 10 刻みではありません"),
                    (u.Armor % 50 != 0, $"装甲 {u.Armor} が 50 刻みではありません"),
                    (u.Mobility % 5 != 0, $"運動性 {u.Mobility} が 5 刻みではありません"),
                }.Where(x => x.Item1).Select(x => r.AtUnit(c, u, "最大ＨＰ", x.Item2))
                .Concat(u.Weapons.SelectMany(w => new[]
                {
                    (w.Power % 100 != 0, $"{w.Name}の攻撃力 {w.Power} が 100 刻みではありません"),
                    (w.ENConsumption % 5 != 0, $"{w.Name}の消費 EN {w.ENConsumption} が 5 刻みではありません"),
                }.Where(x => x.Item1).Select(x => r.AtWeapon(c, u, w, x.Item2))))));

        public static readonly IDataRule PilotSpecialPowerCount = new LambdaRule(
            "pilot-sp-count", Both, DiagnosticSeverity.Information,
            "名前付きのパイロット（名前に (ザコ)・(汎用) が無い）のスペシャルパワーが 6 つでない", "データ作成ガイドライン 2.2 SP の構成（SRC-Data）",
            (c, r) => c.Pilots.Where(p => p.IsNamed() && p.SP > 0)
                .Select(p => (p, n: p.DefinedSpecialPowers().Count))
                .Where(x => x.n != 6)
                .Select(x => r.AtPilot(c, x.p, "スペシャルパワー", $"名前付きのパイロットのスペシャルパワーが {x.n} 個です（6 つにそろえる）")));

        public static readonly IDataRule PilotSpecialPowerLevelPattern = new LambdaRule(
            "pilot-sp-level-pattern", Both, DiagnosticSeverity.Information,
            "スペシャルパワーを覚えるレベルが、すべて 5 の倍数か、間隔が一定", "データ作成ガイドライン 2.2 SP の構成（SRC-Data）",
            (c, r) => c.Pilots.Where(p => p.IsNamed() && p.SP > 0).SelectMany(p =>
            {
                var sps = p.DefinedSpecialPowers();
                var list = new List<SrcDiagnostic>();
                var later = sps.Where(x => x.NecessaryLevel > 1).ToList();
                if (later.Count >= 3 && later.All(x => x.NecessaryLevel % 5 == 0))
                {
                    list.Add(r.AtPilot(c, p, "スペシャルパワー", "レベル 1 以外のスペシャルパワーを覚えるレベルが、すべて 5 の倍数です"));
                }
                var lvs = sps.Select(x => x.NecessaryLevel).OrderBy(x => x).ToList();
                var gaps = lvs.Zip(lvs.Skip(1), (a, b) => b - a).Where(g => g > 0).ToList();
                if (gaps.Count >= 3 && gaps.Distinct().Count() == 1)
                {
                    list.Add(r.AtPilot(c, p, "スペシャルパワー", $"スペシャルパワーを覚えるレベルの間隔が、すべて {gaps[0]} です"));
                }
                return list;
            }));

        public static readonly IDataRule MessageSpecialPowerSituation = new LambdaRule(
            "message-sp-situation", Both, DiagnosticSeverity.Information,
            "スペシャルパワーを使ったときのメッセージ（復活・自爆を除く）がある", "データ作成ガイドライン 7 メッセージ（SRC-Data）",
            (c, r) => c.Messages.SelectMany(m => m.Items
                .Where(x => x.Situation != "復活" && x.Situation != "自爆" && c.Src.SPDList.IsDefined(x.Situation))
                .GroupBy(x => x.Situation)
                .Select(g => r.At(c, g.First().Location ?? m.Location, g.First().Location?.Line ?? 1,
                    $"スペシャルパワー「{g.Key}」を使ったときのメッセージがあります（パックでは原作の決め台詞に限られます）", m.Name))));

        public static IEnumerable<IDataRule> All => new[]
        {
            WeaponRange1Infight,
            WeaponRange2Shooting,
            WeaponRange1P,
            WeaponCrossRangeShooting,
            WeaponBeamAdaption,
            WeaponBeamMelee,
            UnitAirAdaptionNonflyer,
            NameLength,
            NumberRounding,
            PilotSpecialPowerCount,
            PilotSpecialPowerLevelPattern,
            MessageSpecialPowerSituation,
        };
    }
}
