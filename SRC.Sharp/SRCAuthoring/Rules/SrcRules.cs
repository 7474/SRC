using SRCCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace SRCAuthoring.Rules
{
    /// <summary>
    /// SRC の仕様と食い違い、意図どおりに動かないデータの規則（グループ src。既定で動く。すべて「情報」）（7474/SRC#1611）。
    /// 根拠は元の SRC（SRC/SRC_20121125）の実装。
    /// </summary>
    public static class SrcRules
    {
        private static readonly string[] SrcAndRobodun = { RuleGroups.Src, RuleGroups.Robodun };
        private static readonly string[] SrcOnly = { RuleGroups.Src };

        /// <summary>移動可能地形の判定に使われる地形。元の SRC は InStr(Transportation, "空") などで判定し、ほかの文字は見ない</summary>
        private static readonly string[] TransportationAreas = { "地中", "宇宙", "空", "陸", "水", "-" };

        public static readonly IDataRule UnitTransportationInvalid = new LambdaRule(
            "unit-transportation-invalid", SrcAndRobodun, DiagnosticSeverity.Information,
            "移動可能地形に、空・陸・水・地中・宇宙・- 以外がある（その部分は無いものとして扱われる）",
            "ユニットデータの (7)。元の SRC は移動可能地形を文字列のまま持ち、Unit.IsTransAvailable の InStr で「空」「陸」「水」「地中」「宇宙」が含まれるかだけを見る。ロボダンの UnitChecker.eve（水の移動タイプが海）",
            (c, r) => c.Units.Select(u =>
            {
                var rest = TransportationAreas.Aggregate(u.Transportation ?? "", (s, area) => s.Replace(area, ""));
                return rest.Length == 0 ? null
                    : r.AtUnit(c, u, "移動可能地形", $"移動可能地形「{u.Transportation}」の「{rest}」は使われません（空・陸・水・地中・宇宙を組み合わせて書きます）");
            }).Where(x => x != null));

        public static readonly IDataRule UnitTransportationAdaption = new LambdaRule(
            "unit-transportation-adaption", SrcAndRobodun, DiagnosticSeverity.Information,
            "移動可能地形に空・陸・水・宇宙があるのに、その地形の地形適応が -（その地形を移動できない）",
            "元の SRC の移動範囲の計算（Map.bas の AreaInSpeed）は、IsTransAvailable(\"空\") And Adaption(1) <> 0 のように、移動可能地形と地形適応の両方を見る。ロボダンの UnitChecker.eve",
            (c, r) => c.Units.SelectMany(u =>
                new[] { ("空", 1), ("陸", 2), ("水", 3), ("宇宙", 4) }
                    .Where(x => (u.Transportation ?? "").Contains(x.Item1) && RuleData.AdaptionAt(u.Adaption, x.Item2) == "-")
                    .Select(x => r.AtUnit(c, u, "地形適応", $"移動可能地形に「{x.Item1}」があるのに、地形適応「{u.Adaption}」の{x.Item1}が「-」です（{x.Item1}を移動できません）"))));

        public static readonly IDataRule PilotSpecialPowerDuplicate = new LambdaRule(
            "pilot-sp-duplicate", SrcOnly, DiagnosticSeverity.Information,
            "1 人のパイロットが同じスペシャルパワーを重ねて持つ",
            "パイロットデータのスペシャルパワーの欄。元の SRC は同じ名前を別の枠として持ち、効果は増えない。tools/pack-stats の check",
            (c, r) => c.Pilots.Select(p =>
            {
                var dup = p.DefinedSpecialPowers().GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                return dup.Count == 0 ? null
                    : r.AtPilot(c, p, "スペシャルパワー", $"同じスペシャルパワーを重ねて持っています: {string.Join("、", dup)}");
            }).Where(x => x != null));

        /// <summary>
        /// 元の SRC が PilotMessage・IsMessageDefined・SelectMessage に渡すシチュエーション（括弧の前の部分）。
        /// 武器・アビリティ・特殊能力・スペシャルパワーの名前は、読み込んだデータから足す
        /// </summary>
        private static readonly HashSet<string> KnownSituations = new HashSet<string>
        {
            // Unit.cls・Commands など（SRC/SRC_20121125 の PilotMessage と IsMessageDefined の文字列）
            "かけ声", "かばう", "アビリティ", "カウンター", "サポートアタック", "サポートガード", "シールド防御", "ジャンプ", "ダミー",
            "ダメージ中", "ダメージ大", "ダメージ小", "チャージ", "テレポート", "ノーマルモード", "ハイパーモード", "バリア発動", "バリア貫通",
            "ビーム無効化", "フィールド発動", "プロテクション発動", "レベルアップ", "修理", "再攻撃", "分身", "分離", "切り払い", "反射", "合体",
            "回避", "変形", "射程外", "当て身技", "復活", "恐怖", "攻撃無効化", "浄化", "浄解", "混乱", "発進", "着艦", "破壊", "破壊時",
            "破壊時分離", "緊急テレポート", "脱出", "自動反撃", "自爆", "補給", "迎撃", "追加攻撃", "連携攻撃", "阻止",
            // Unit.PilotMessage が武器のメッセージの代わりに探すもの
            "攻撃", "格闘", "射撃",
            // ヘルプのメッセージデータに載っているもの
            "地上", "水上", "水中", "空中", "地中",
        };

        /// <summary>シチュエーションの括弧の前の部分。元の SRC は半角の「(」で区切る（全角の「（」は区切りにならない）</summary>
        public static string SituationBase(string situation)
        {
            var i = (situation ?? "").IndexOf('(');
            return (i < 0 ? situation ?? "" : situation.Substring(0, i)).Trim();
        }

        /// <summary>データの中の、武器・アビリティ・特殊能力の名前</summary>
        private static HashSet<string> ActionNames(DataRuleContext c)
        {
            var src = c.Src;
            var units = src.UDList.Items.SelectMany(u => u.Weapons.Select(w => w.Name).Concat(u.Abilities.Select(a => a.Name)).Concat(u.Features.Select(f => f.Name)));
            var pilots = src.PDList.Items.SelectMany(p => p.Weapons.Select(w => w.Name).Concat(p.Abilities.Select(a => a.Name)).Concat(p.Features.Select(f => f.Name)));
            var items = src.IDList.Items.SelectMany(i => i.Weapons.Select(w => w.Name).Concat(i.Abilities.Select(a => a.Name)).Concat(i.Features.Select(f => f.Name)));
            var names = new HashSet<string>(units.Concat(pilots).Concat(items).Where(x => !string.IsNullOrEmpty(x)));
            names.UnionWith(c.ActionNames);
            return names;
        }

        public static readonly IDataRule MessageSituationUnknown = new LambdaRule(
            "message-situation-unknown", SrcOnly, DiagnosticSeverity.Information,
            "メッセージのシチュエーションが、SRC が使うものでも、武器・アビリティ・特殊能力・スペシャルパワーの名前でもない（表示されない）",
            "元の SRC の Unit.PilotMessage・IsMessageDefined に渡す文字列と、武器・アビリティの名前・スペシャルパワーの名前。シチュエーションは半角の「(」の前で比べる。ユニットのデータを 1 つも読んでいなければ調べない。tools/pack-stats の check",
            (c, r) =>
            {
                if (!c.Src.UDList.Items.Any(x => x.Location != null))
                {
                    return Enumerable.Empty<SrcDiagnostic>();
                }
                var names = ActionNames(c);
                bool Known(string b) => KnownSituations.Contains(b) || names.Contains(b) || c.Src.SPDList.IsDefined(b);
                return c.Messages.SelectMany(m => m.Items
                    .Where(x => !Known(SituationBase(x.Situation)))
                    .GroupBy(x => x.Situation)
                    .Select(g => r.At(c, g.First().Location ?? m.Location, g.First().Location?.Line ?? m.Location?.Line ?? 1,
                        $"シチュエーション「{g.Key}」は、SRC が使うものでも、武器・アビリティ・特殊能力・スペシャルパワーの名前でもありません（表示されません）", m.Name)));
            });

        public static IEnumerable<IDataRule> All => new[]
        {
            UnitTransportationInvalid,
            UnitTransportationAdaption,
            PilotSpecialPowerDuplicate,
            MessageSituationUnknown,
        };
    }
}
