using SRCCore.Lib;
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCAuthoring.Rules
{
    /// <summary>検査を関数で書く規則</summary>
    public sealed class LambdaRule : DataRule
    {
        private readonly Func<DataRuleContext, LambdaRule, IEnumerable<SrcDiagnostic>> check;

        public LambdaRule(string id, string[] groups, DiagnosticSeverity severity, string description, string origin,
            Func<DataRuleContext, LambdaRule, IEnumerable<SrcDiagnostic>> check)
        {
            Id = id;
            Groups = groups;
            Severity = severity;
            Description = description;
            Origin = origin;
            this.check = check;
        }

        public override string Id { get; }
        public override IReadOnlyList<string> Groups { get; }
        public override DiagnosticSeverity Severity { get; }
        public override string Description { get; }
        public override string Origin { get; }

        public override IEnumerable<SrcDiagnostic> Check(DataRuleContext context) => check(context, this);

        public SrcDiagnostic At(DataRuleContext context, DataLocation location, int line, string message, string source)
            => Report(context, location, line, message, source);

        /// <summary>ユニットの欄の行で知らせる</summary>
        public SrcDiagnostic AtUnit(DataRuleContext context, UnitData unit, string field, string message)
            => Report(context, unit.Location, unit.Location?.FieldLine(field) ?? 1, message, unit.Name);

        /// <summary>武器の行で知らせる</summary>
        public SrcDiagnostic AtWeapon(DataRuleContext context, UnitData unit, WeaponData weapon, string message)
            => Report(context, weapon.Location ?? unit.Location, weapon.Location?.Line ?? unit.Location?.Line ?? 1, message, unit.Name);

        /// <summary>パイロットの欄の行で知らせる</summary>
        public SrcDiagnostic AtPilot(DataRuleContext context, PilotData pilot, string field, string message)
            => Report(context, pilot.Location, pilot.Location?.FieldLine(field) ?? 1, message, pilot.Name);
    }

    internal static class RuleData
    {
        /// <summary>武器属性を持つか。元の SRC の IsWeaponClassifiedAs と同じく、括弧の中を除いて含むかで決める</summary>
        public static bool Has(this WeaponData w, string attr) => GeneralLib.InStrNotNest(w.Class ?? "", attr) > 0;

        /// <summary>マップ兵器（Ｍ属性）か</summary>
        public static bool IsMap(this WeaponData w) => w.Has("Ｍ");

        public static IEnumerable<(UnitData Unit, WeaponData Weapon)> Weapons(this DataRuleContext context)
            => context.Units.SelectMany(u => u.Weapons.Select(w => (u, w)));

        public static ISet<string> SpecialPowerNames(this PilotData p)
            => new HashSet<string>(p.DefinedSpecialPowers().Select(x => x.Name));

        /// <summary>名前付きのパイロット（名前に (ザコ)・(汎用) が無い）</summary>
        public static bool IsNamed(this PilotData p) => !p.Name.Contains("(ザコ)") && !p.Name.Contains("(汎用)");

        /// <summary>地形適応の i 文字目（1 始まり）。無ければ空</summary>
        public static string AdaptionAt(string adaption, int i) => adaption != null && adaption.Length >= i ? adaption.Substring(i - 1, 1) : "";
    }
}
