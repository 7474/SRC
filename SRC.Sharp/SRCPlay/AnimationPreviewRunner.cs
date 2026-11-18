using SRCCore;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCPlay
{
    public sealed class AnimationPreviewRequest
    {
        public string Kind { get; set; } = "animation";
        public string File { get; set; }
        public int Line { get; set; }
        public string Target { get; set; }
        public string Situation { get; set; }
        public string Spec { get; set; }
        public string AttackerUnit { get; set; }
        public string AttackerPilot { get; set; }
        public string DefenderUnit { get; set; }
        public string DefenderPilot { get; set; }
    }

    public sealed class AnimationPreviewPlayResult
    {
        public bool Played { get; set; }
        public string Message { get; set; }
        public string Attacker { get; set; }
        public string Defender { get; set; }
        public List<string> Warnings { get; } = new List<string>();
    }

    /// <summary>
    /// 戦闘アニメ・特殊効果・汎用戦闘アニメサブルーチンの 1 定義を、
    /// 決まった攻撃側・防御側ユニットで再生する（7474/SRC#1655）。
    /// </summary>
    public static class AnimationPreviewRunner
    {
        private static readonly HashSet<string> NonWeaponSituations = new HashSet<string>(StringComparer.Ordinal)
        {
            "回避", "破壊", "ダメージ", "小ダメージ", "中ダメージ", "大ダメージ",
            "変形", "ハイパーモード", "ノーマルモード", "パーツ分離",
            "合体", "分離", "修理", "補給", "終了", "切り払い", "迎撃", "ダミー", "汎用",
            "攻撃無効化", "シールド防御",
        };

        /// <summary>
        /// 既に起動中の SRC セッション（デバッガのターン 1 待機状態など）上でプレビューを 1 回再生する。
        /// ゲームスレッド上で呼ぶこと。
        /// </summary>
        public static AnimationPreviewPlayResult Play(SRC src, AnimationPreviewRequest request)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new AnimationPreviewPlayResult();
            var (attacker, defender) = EnsureUnits(src, request, result.Warnings);
            if (attacker == null || defender == null)
            {
                result.Message = "攻撃側または防御側のユニットを用意できませんでした。";
                return result;
            }

            result.Attacker = attacker.Name;
            result.Defender = defender.Name;

            var kind = (request.Kind ?? "animation").ToLowerInvariant();
            var situation = request.Situation ?? "";
            var spec = (request.Spec ?? "").Trim('\t', ' ', '\u3000');

            var prevSelectedUnit = src.Commands.SelectedUnit;
            var prevSelectedTarget = src.Commands.SelectedTarget;
            var prevEventUnit = src.Event.SelectedUnitForEvent;
            var prevEventTarget = src.Event.SelectedTargetForEvent;
            var prevBattleAnim = src.BattleAnimationOverride;
            var prevWeaponAnim = src.WeaponAnimationOverride;
            var prevExtAnim = src.ExtendedAnimationOverride;

            try
            {
                src.Commands.SelectedUnit = attacker;
                src.Commands.SelectedTarget = defender;
                src.Event.SelectedUnitForEvent = attacker;
                src.Event.SelectedTargetForEvent = defender;

                src.GUI.Center(7, 6);
                src.GUI.RedrawScreen();

                if (kind == "subroutine")
                {
                    src.BattleAnimationOverride = true;
                    src.WeaponAnimationOverride = true;
                    src.ExtendedAnimationOverride = true;
                    var sname = string.IsNullOrEmpty(spec) ? situation : spec;
                    if (string.IsNullOrEmpty(sname))
                    {
                        result.Message = "再生する戦闘アニメのサブルーチン名が空です。";
                        return result;
                    }
                    var exprHead = GeneralLib.LIndex(sname, 1);
                    if (!exprHead.StartsWith("戦闘アニメ_", StringComparison.Ordinal))
                    {
                        exprHead = "戦闘アニメ_" + exprHead;
                    }
                    if (src.Event.FindNormalLabel(exprHead) < 0 && !string.IsNullOrEmpty(request.File) && File.Exists(request.File))
                    {
                        src.Event.LoadDynamic(request.File);
                    }
                    if (src.Event.FindNormalLabel(exprHead) < 0)
                    {
                        result.Message = $"サブルーチン「{exprHead}」が見つかりません。";
                        result.Warnings.Add(result.Message);
                        return result;
                    }
                    src.Effect.ShowAnimation(sname);
                    result.Played = true;
                    result.Message = $"{exprHead} を再生しました";
                    return result;
                }

                if (string.IsNullOrEmpty(spec) || spec == "-")
                {
                    result.Message = "指定が空または「-」のため再生しません。";
                    return result;
                }

                if (kind == "effect")
                {
                    // effect.txt の定義そのものを確認できるよう戦闘アニメ自動置換を抑える
                    src.BattleAnimationOverride = false;
                    src.WeaponAnimationOverride = false;
                    src.ExtendedAnimationOverride = false;
                    var isWeapon = attacker.Weapons.Any(w => situation == w.Name)
                        || (!NonWeaponSituations.Contains(situation) && !situation.Contains("("));
                    var isAbility = attacker.Abilities.Any(a => situation == a.Data.Name);
                    attacker.SpecialEffectSpec(situation, spec, isWeapon, isAbility, w: 0, keep_message_form: false);
                    result.Played = true;
                    result.Message = $"{request.Target}: {situation} を再生しました";
                    return result;
                }

                // animation / ext_animation
                src.BattleAnimationOverride = true;
                src.WeaponAnimationOverride = true;
                src.ExtendedAnimationOverride = true;

                var (resolvedSituation, isWeaponAnim, isAbilityAnim, inBulk) = ClassifyAnimationSituation(src, attacker, situation, spec);
                attacker.PlayAnimationSpec(resolvedSituation, spec, isWeaponAnim, isAbilityAnim, inBulk, keep_message_form: false);
                result.Played = true;
                result.Message = $"{request.Target}: {situation} を再生しました";
                return result;
            }
            finally
            {
                src.GUI.ClearPicture();
                if (src.GUI.MessageFormVisible)
                {
                    src.GUI.CloseMessageForm();
                }
                src.BattleAnimationOverride = prevBattleAnim;
                src.WeaponAnimationOverride = prevWeaponAnim;
                src.ExtendedAnimationOverride = prevExtAnim;
                src.Commands.SelectedUnit = prevSelectedUnit;
                src.Commands.SelectedTarget = prevSelectedTarget;
                src.Event.SelectedUnitForEvent = prevEventUnit;
                src.Event.SelectedTargetForEvent = prevEventTarget;
                src.GUI.RedrawScreen();
            }
        }

        private static (string situation, bool isWeapon, bool isAbility, bool inBulk) ClassifyAnimationSituation(
            SRC src,
            Unit attacker,
            string situation,
            string spec)
        {
            if (situation.EndsWith("(攻撃)", StringComparison.Ordinal))
            {
                return (situation, true, false, false);
            }
            if (situation.EndsWith("(準備)", StringComparison.Ordinal) || situation.EndsWith("(命中)", StringComparison.Ordinal))
            {
                return (situation, false, false, false);
            }
            if (situation.EndsWith("(発動)", StringComparison.Ordinal))
            {
                var isAb = attacker.Abilities.Any(a => situation == a.Data.Name + "(発動)");
                return (situation, false, isAb, false);
            }
            if (situation.Contains("("))
            {
                return (situation, false, false, false);
            }

            if (attacker.Weapons.Any(w => w.Name == situation))
            {
                return (situation + "(攻撃)", true, false, true);
            }
            if (attacker.Abilities.Any(a => a.Data.Name == situation))
            {
                return (situation + "(発動)", false, true, false);
            }
            if (NonWeaponSituations.Contains(situation))
            {
                return (situation, false, false, false);
            }

            // 括弧なしの一括指定（例: ヒレブレード, 斬撃 や ビームライフル, ビーム）。
            // 最初の指定に「〜発動」が無く「〜攻撃」がある場合は武器攻撃として解決する
            var firstPart = spec.Split(';').FirstOrDefault(p => !string.IsNullOrWhiteSpace(p) && !SRCCore.VB.Information.IsNumeric(p.Trim()))?.Trim() ?? "";
            var token = GeneralLib.LIndex(firstPart, 1);
            if (!string.IsNullOrEmpty(token) && !token.StartsWith("@", StringComparison.Ordinal))
            {
                var hasHatsudou = src.Event.FindNormalLabel("戦闘アニメ_" + token + "発動") >= 0;
                var hasKougeki = src.Event.FindNormalLabel("戦闘アニメ_" + token + "攻撃") >= 0;
                if (!hasHatsudou && hasKougeki)
                {
                    return (situation + "(攻撃)", true, false, true);
                }
            }

            return (situation, false, false, false);
        }

        private static (Unit attacker, Unit defender) EnsureUnits(SRC src, AnimationPreviewRequest request, List<string> warnings)
        {
            Unit attacker = null;
            Unit defender = null;

            if (!string.IsNullOrEmpty(request.AttackerUnit))
            {
                attacker = src.UList.Items.FirstOrDefault(u => u.Party == "味方" && u.Name == request.AttackerUnit);
            }
            attacker ??= src.UList.Items.FirstOrDefault(u => u.Party == "味方");

            if (!string.IsNullOrEmpty(request.DefenderUnit))
            {
                defender = src.UList.Items.FirstOrDefault(u => u.Party == "敵" && u.Name == request.DefenderUnit);
            }
            defender ??= src.UList.Items.FirstOrDefault(u => u.Party == "敵");

            if (attacker == null)
            {
                attacker = CreateFallbackUnit(src, "味方", request.AttackerUnit ?? "__プレビュー攻撃側__", request.AttackerPilot ?? "__プレビュー攻撃側Ｐ__", 5, 6, warnings);
            }
            if (defender == null)
            {
                defender = CreateFallbackUnit(src, "敵", request.DefenderUnit ?? "__プレビュー防御側__", request.DefenderPilot ?? "__プレビュー防御側Ｐ__", 8, 6, warnings);
            }

            if (attacker != null && !string.IsNullOrEmpty(request.Target)
                && request.Target != "汎用" && request.Target != "汎用戦闘アニメ"
                && attacker.Name != request.Target)
            {
                // 既存のダミーユニットでターゲット名称を優先検索したい場合用のヒントは不要だがログには残さない
            }

            return (attacker, defender);
        }

        private static Unit CreateFallbackUnit(SRC src, string party, string unitName, string pilotName, int x, int y, List<string> warnings)
        {
            var ud = src.UDList.IsDefined(unitName) ? src.UDList.Item(unitName) : null;
            if (ud == null)
            {
                ud = src.UDList.Add(unitName);
                ud.Nickname = unitName;
                ud.Class = "汎用";
                ud.PilotNum = 1;
                ud.Size = "M";
                ud.Speed = 4;
                ud.HP = 3000;
                ud.EN = 100;
                ud.Armor = 600;
                ud.Mobility = 60;
                ud.Adaption = "AAAA";
                ud.Bitmap = "-.bmp";
                ud.Transportation = "陸";
            }

            var pd = src.PDList.IsDefined(pilotName) ? src.PDList.Item(pilotName) : null;
            if (pd == null)
            {
                pd = src.PDList.Add(pilotName);
                pd.Nickname = pilotName;
                pd.Class = "汎用";
                pd.Adaption = "AAAA";
                pd.Bitmap = "-.bmp";
                pd.Personality = "普通";
            }

            var u = src.UList.Add(ud.Name, 0, party);
            var isGenericPilot = pd.Name.Contains("(ザコ)", StringComparison.Ordinal) || pd.Name.Contains("(汎用)", StringComparison.Ordinal);
            var gid = !isGenericPilot && src.PList.IsDefined2(pd.Name) ? $"__preview_{party}_{pd.Name}__" : null;
            var p = src.PList.Add(pd.Name, 30, party, gid);
            p.Ride(u);
            u.MainPilot();
            if (src.Map.IsInside(x, y) && src.Map.Terrain(x, y) != null)
            {
                u.StandBy(x, y);
            }
            else
            {
                u.Status = "出撃";
                u.x = Math.Max(1, Math.Min(x, Math.Max(1, src.Map.MapWidth)));
                u.y = Math.Max(1, Math.Min(y, Math.Max(1, src.Map.MapHeight)));
            }
            warnings.Add($"シナリオにユニット/パイロットが無いため仮ユニット「{ud.Name}」を配置しました。");
            return u;
        }
    }
}
