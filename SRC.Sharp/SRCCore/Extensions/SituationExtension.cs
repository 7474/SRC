using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class SituationExtension
    {
        // ユニット u のシチュエーション msg_situation におけるメッセージを選択
        public static T SelectMessage<T>(this IList<T> Items, SRC SRC, string msg_situation, Unit u = null, bool ignore_condition = false)
            where T : ISituationItem
        {
            // シチュエーションを設定
            var situations = new List<string>();
            situations.Add(msg_situation);
            switch (msg_situation ?? "")
            {
                case "格闘":
                case "射撃":
                    situations.Add("攻撃");
                    break;

                case "格闘(命中)":
                case "射撃(命中)":
                    situations.Add("攻撃(命中)");
                    break;

                case "格闘(回避)":
                case "射撃(回避)":
                    situations.Add("攻撃(回避)");
                    break;

                case "格闘(とどめ)":
                case "射撃(とどめ)":
                    situations.Add("攻撃(とどめ)");
                    break;

                case "格闘(クリティカル)":
                case "射撃(クリティカル)":
                    situations.Add("攻撃(クリティカル)");
                    break;

                case "格闘(反撃)":
                case "射撃(反撃)":
                    situations.Add("攻撃(反撃)");
                    break;

                case "格闘(命中)(反撃)":
                case "射撃(命中)(反撃)":
                    situations.Add("攻撃(命中)(反撃)");
                    break;

                case "格闘(回避)(反撃)":
                case "射撃(回避)(反撃)":
                    situations.Add("攻撃(回避)(反撃)");
                    break;

                case "格闘(とどめ)(反撃)":
                case "射撃(とどめ)(反撃)":
                    situations.Add("攻撃(とどめ)(反撃)");
                    break;

                case "格闘(クリティカル)(反撃)":
                case "射撃(クリティカル)(反撃)":
                    situations.Add("攻撃(クリティカル)(反撃)");
                    break;

                default:
                    break;
            }

            // メッセージの候補リスト第一次審査
            var list0 = Items
                .Where(m => m.IsAvailable(u, ignore_condition))
                .Where(m => situations.Any(s => m.Situation.StartsWith(s)))
                .ToList();

            if (!list0.Any())
            {
                return default;
            }

            // 最初に相手限定のシチュエーションのみで検索
            if (u is null)
            {
                goto SkipMessagesWithTarget;
            }

            Unit t = null;
            if (ReferenceEquals(u, SRC.Commands.SelectedUnit))
            {
                t = SRC.Commands.SelectedTarget;
            }
            else if (ReferenceEquals(u, SRC.Commands.SelectedTarget))
            {
                t = SRC.Commands.SelectedUnit;
            }

            if (t is null)
            {
                goto SkipMessagesWithTarget;
            }

            // 相手限定メッセージのリストを作成
            var tlist = list0.Where(m => Strings.InStr(m.Situation, "(対") > 0).ToList();

            if (!tlist.Any())
            {
                // 相手限定メッセージがない
                goto SkipMessagesWithTarget;
            }

            // 自分自身にアビリティを使う場合は必ず「(対自分)」を優先
            if (ReferenceEquals(t, u))
            {
                var list = list0.Where(m => situations.Any(s => m.Situation == s + "(対自分)")).ToList();
                if (list.Any())
                {
                    return list.Dice();
                }
            }

            if (t.Status != "出撃")
            {
                goto SkipMessagesWithTarget;
            }

            var sub_situations = new List<string>();
            // 対パイロット名称
            sub_situations.Add("(対" + t.MainPilot().Name + ")");
            // 対パイロット愛称
            sub_situations.Add("(対" + t.MainPilot().get_Nickname(false) + ")");
            // 対ユニット名称
            sub_situations.Add("(対" + t.Name + ")");
            // 対ユニット愛称
            sub_situations.Add("(対" + t.Nickname + ")");
            // 対ユニットクラス
            sub_situations.Add("(対" + t.Class0 + ")");
            // 対ユニットサイズ
            sub_situations.Add("(対" + t.Size + ")");
            // 対地形名
            sub_situations.Add("(対" + SRC.Map.Terrain(t.x, t.y).Name + ")");
            // 対エリア
            sub_situations.Add("(対" + t.Area + ")");

            // 対メッセージクラス
            if (t.IsFeatureAvailable("メッセージクラス"))
            {
                sub_situations.Add("(対" + t.FeatureData("メッセージクラス") + ")");
            }

            // 対性別
            switch (t.MainPilot().Sex ?? "")
            {
                case "男性":
                    {
                        sub_situations.Add("(対男性)");
                        break;
                    }

                case "女性":
                    {
                        sub_situations.Add("(対女性)");
                        break;
                    }
            }

            // 対特殊能力
            {
                var p = t.MainPilot();
                var loopTo5 = p.CountSkill();
                for (var i = 1; i <= loopTo5; i++)
                {
                    var subSituationName = "(対" + p.SkillName0(i) + ")";
                    if (subSituationName == "(対非表示)")
                    {
                        subSituationName = "(対" + p.Skill(i) + ")";
                    }
                    sub_situations.Add(subSituationName);
                }
            }

            foreach (var fd in t.Features)
            {
                var subSituationName = "(対" + fd.FeatureName0(t) + ")";
                if (subSituationName == "(対)")
                {
                    subSituationName = "(対" + fd.Name + ")";
                }
                sub_situations.Add(subSituationName);
            }

            // 対弱点
            if (Strings.Len(t.strWeakness) > 0)
            {
                var loopTo7 = Strings.Len(t.strWeakness);
                for (var i = 1; i <= loopTo7; i++)
                {
                    sub_situations.Add("(対弱点=" + GeneralLib.GetClassBundle(t.strWeakness, ref i) + ")");
                }
            }

            // 対有効
            if (Strings.Len(t.strEffective) > 0)
            {
                var loopTo8 = Strings.Len(t.strEffective);
                for (var i = 1; i <= loopTo8; i++)
                {
                    sub_situations.Add("(対有効=" + GeneralLib.GetClassBundle(t.strEffective, ref i) + ")");
                }
            }

            // 対ザコ
            if (Strings.InStr(t.MainPilot().Name, "(ザコ)") > 0
                && (u.MainPilot().Technique > t.MainPilot().Technique || u.HP > t.HP / 2))
            {
                sub_situations.Add("(対ザコ)");
            }

            // 対強敵
            if (t.BossRank >= 0 || Strings.InStr(t.MainPilot().Name, "(ザコ)") == 0
                && u.MainPilot().Technique <= t.MainPilot().Technique)
            {
                sub_situations.Add("(対強敵)");
            }

            // 自分が使用する武器をチェック
            var w = 0;
            if (ReferenceEquals(SRC.Commands.SelectedUnit, u))
            {
                if (0 < SRC.Commands.SelectedWeapon && SRC.Commands.SelectedWeapon <= u.CountWeapon())
                {
                    w = SRC.Commands.SelectedWeapon;
                }
            }
            else if (ReferenceEquals(SRC.Commands.SelectedTarget, u))
            {
                if (0 < SRC.Commands.SelectedTWeapon && SRC.Commands.SelectedTWeapon <= u.CountWeapon())
                {
                    w = SRC.Commands.SelectedTWeapon;
                }
            }

            if (w > 0)
            {
                var uw = u.Weapon(w);
                // 対瀕死
                if (t.HP <= uw.Damage(t, u.Party == "味方"))
                {
                    sub_situations.Add("(対瀕死)");
                }

                switch (uw.HitProbability(t, u.Party == "味方"))
                {
                    case var @case when @case < 50:
                        {
                            // 対高回避率
                            sub_situations.Add("(対高回避率)");
                            break;
                        }

                    case var case1 when case1 >= 100:
                        {
                            // 対低回避率
                            sub_situations.Add("(対低回避率)");
                            break;
                        }
                }
            }

            // 相手が使用する武器をチェック
            var tw = 0;
            if (ReferenceEquals(SRC.Commands.SelectedUnit, t))
            {
                if (0 < SRC.Commands.SelectedWeapon && SRC.Commands.SelectedWeapon <= t.CountWeapon())
                {
                    tw = SRC.Commands.SelectedWeapon;
                }
            }
            else if (ReferenceEquals(SRC.Commands.SelectedTarget, t))
            {
                if (0 < SRC.Commands.SelectedTWeapon && SRC.Commands.SelectedTWeapon <= t.CountWeapon())
                {
                    tw = SRC.Commands.SelectedTWeapon;
                }
            }

            if (tw > 0)
            {
                var tuw = t.Weapon(tw);
                // 対武器名
                sub_situations.Add("(対" + t.Weapon(tw).Name + ")");

                // 対武器属性
                var wclass = tuw.WeaponClass();
                var loopTo9 = Strings.Len(wclass);
                for (var i = 1; i <= loopTo9; i++)
                {
                    var ch = GeneralLib.GetClassBundle(wclass, ref i).FirstOrDefault();
                    // TODO .NET 6
                    //if(!ch.IsAscii)
                    {
                        sub_situations.Add("(対" + ch + "属性)");
                    }
                }

                switch (tuw.HitProbability(u, t.Party == "味方"))
                {
                    case var case3 when case3 > 75:
                        {
                            // 対高命中率
                            sub_situations.Add("(対高命中率)");
                            break;
                        }

                    case var case4 when case4 < 25:
                        {
                            // 対低命中率
                            sub_situations.Add("(対低命中率)");
                            break;
                        }
                }
            }


            // 定義されている相手限定メッセージのうち、状況に合ったメッセージを抜き出す
            {
                var list = tlist.Where(m => situations.Zip(sub_situations, (s, subs) => s + subs).Any(s => m.Situation == s)).ToList();
                // 状況に合った相手限定メッセージが一つでもあれば、その中からメッセージを選択
                if (list.Any())
                {
                    var res = list.Dice();
                    if (GeneralLib.Dice(2) == 1
                        || Strings.InStr(msg_situation, "(とどめ)") > 0
                        || msg_situation == "挑発"
                        || msg_situation == "脱力"
                        || msg_situation == "魅惑"
                        || msg_situation == "威圧"
                        || (u.Party ?? "") == (t.Party ?? ""))
                    {
                        return res;
                    }
                }
            }

        SkipMessagesWithTarget:
            ;

            // 次にサブシチュエーションなしとユニット限定のサブシチュエーションで検索
            sub_situations = new List<string>();
            if (u is object)
            {
                sub_situations.Add("(" + u.Name + ")");
                sub_situations.Add("(" + u.Nickname0 + ")");
                sub_situations.Add("(" + u.Class0 + ")");
                switch (msg_situation ?? "")
                {
                    case "格闘":
                    case "射撃":
                    case "格闘(反撃)":
                    case "射撃(反撃)":
                        {
                            if (ReferenceEquals(SRC.Commands.SelectedUnit, u))
                            {
                                // 自分が使用する武器をチェック
                                if (0 < SRC.Commands.SelectedWeapon && SRC.Commands.SelectedWeapon <= u.CountWeapon())
                                {
                                    sub_situations.Add("(" + u.Weapon(SRC.Commands.SelectedWeapon).WeaponNickname() + ")");
                                }
                            }

                            break;
                        }
                }

                if (u.IsFeatureAvailable("メッセージクラス"))
                {
                    sub_situations.Add("(" + u.FeatureData("メッセージクラス") + ")");
                }
            }

            // 上で見つかったメッセージリストの中からシチュエーションに合ったメッセージを抜き出す
            {
                var list = list0
                    .Where(m => situations.Any(s => m.Situation == s))
                    .Concat(list0.Where(m => situations.Zip(sub_situations, (s, subs) => s + subs).Any(s => m.Situation == s)))
                    .ToList();
                // シチュエーションに合ったメッセージが見つかれば、その中からメッセージを選択
                if (list.Any())
                {
                    return list.Dice();
                }
            }

            return default;
        }
    }
}
