// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Pilots
{
    // === ユニット搭乗＆下乗関連処理 ===
    public partial class Pilot
    {
        // ユニット u に搭乗
        public void Ride(Unit u, bool is_support = false)
        {
            double hp_ratio, en_ratio;
            double plana_ratio;

            // 既に乗っていればなにもしない
            if (ReferenceEquals(Unit, u))
            {
                return;
            }

            // TODO Impl Ride
            {
                //var u = u;
                //hp_ratio = 100 * u.HP / (double)u.MaxHP;
                //en_ratio = 100 * u.EN / (double)u.MaxEN;

                //// 現在の霊力値を記録
                //if (MaxPlana() > 0)
                //{
                //    plana_ratio = 100 * Plana / (double)MaxPlana();
                //}
                //else
                //{
                //    plana_ratio = -1;
                //}

                // XXX 仮
                Unit = u;
                u.AddPilot(this);
                //short localInStrNotNest1() { string argstring1 = Class; string argstring2 = "サポート)"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); this.Class = argstring1; return ret; }

                //short localLLength() { string arglist = Class; var ret = GeneralLib.LLength(arglist); this.Class = arglist; return ret; }

                //if (localInStrNotNest1() > 0 & localLLength() == 1 & !u.IsFeatureAvailable("ダミーユニット"))
                //{
                //    // サポートにしかなれないパイロットの場合
                //    u.AddSupport(this);
                //}
                //else if (IsSupport(u))
                //{
                //    // 同じユニットクラスに対して通常パイロットとサポートの両方のパターン
                //    // がいける場合は通常パイロットを優先
                //    short localInStrNotNest() { string argstring1 = Class; string argstring2 = u.Class0 + " "; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); this.Class = argstring1; return ret; }

                //    if (u.CountPilot() < Math.Abs(u.Data.PilotNum) & localInStrNotNest() > 0 & !is_support)
                //    {
                //        u.AddPilot(this);
                //    }
                //    else
                //    {
                //        u.AddSupport(this);
                //    }
                //}
                //else
                //{
                //    // パイロットが既に規定数の場合は全パイロットを降ろす
                //    if (u.CountPilot() == Math.Abs(u.Data.PilotNum))
                //    {
                //        u.Pilot(1).GetOff();
                //    }

                //    u.AddPilot(this);
                //}

                // Pilotコマンドで作成されたパイロットは全て味方なので搭乗時に変更が必要
                Party = u.Party0;

                // ユニットのステータスをアップデート
                u.Update();

                //// 霊力値のアップデート
                //if (plana_ratio >= 0d)
                //{
                //    Plana = (int)((long)(MaxPlana() * plana_ratio) / 100L);
                //}
                //else
                //{
                //    Plana = MaxPlana();
                //}

                //// パイロットが乗り込むことによるＨＰ＆ＥＮの増減に対応
                //u.HP = (int)((long)(u.MaxHP * hp_ratio) / 100L);
                //u.EN = (int)((long)(u.MaxEN * en_ratio) / 100L);
            }
        }

        // パイロットをユニットから降ろす
        public void GetOff(bool without_leave = false)
        {
            short i;

            // 既に降りている？
            if (Unit is null)
            {
                return;
            }

            var u = Unit;
            var loopTo = u.CountSupport();
            if (u.Supports.Any(x => x == this))
            {
                // サポートパイロットとして乗り込んでいる場合
                u.DeleteSupport(this);
                u.Update();
                Unit = null;
                Update();
                return;
            }

            // 出撃していた場合は退却
            if (!without_leave)
            {
                if (u.Status == "出撃")
                {
                    u.Status = "待機";
                    // XXX 外から操作したいものではない
                    SRC.Map.MapDataForUnit[u.x, u.y] = null;
                    GUI.EraseUnitBitmap(u.x, u.y, false);
                }
            }

            // 通常のパイロットの場合は、そのユニットに乗っていた他のパイロットも降ろされる
            foreach (var p in u.Pilots)
            {
                p.Unit = null;
                u.DeletePilot(p);
            }
            foreach (var p in u.Supports)
            {
                p.Unit = null;
                u.DeleteSupport(p);
            }
            u.Update();

            Unit = null;
            Update();
        }

        // パイロットがユニット u のサポートかどうか
        public bool IsSupport(Unit u)
        {
            if (u.IsFeatureAvailable("ダミーユニット"))
            {
                // ダミーユニットの場合はサポートパイロットも通常のパイロットとして扱う
                return false;
            }

            // サポート指定が存在する？
            if (GeneralLib.InStrNotNest(Class, "サポート)") == 0)
            {
                return false;
            }

            if (u.CountPilot() == 0)
            {
                // パイロットが乗っていないユニットの場合は通常パイロットを優先
                foreach (var pclass in GeneralLib.ToL(Class))
                {
                    var uclass = u.Class;
                    if (uclass == pclass
                        || uclass == (pclass + "(" + get_Nickname(false) + "専用)")
                        || uclass == (pclass + "(" + Name + "専用)")
                        || uclass == (pclass + "(" + Sex + "専用)"))
                    {
                        // 通常のパイロットとして搭乗可能であればサポートでないとみなす
                        return false;
                    }
                }
            }
            else
            {
                // 通常のパイロットとして搭乗している？
                if (u.Pilots.Contains(this))
                {
                    return false;
                }
            }

            {
                var uclass = u.Class0;
                // 通常のサポート？
                foreach (var pclass in GeneralLib.ToL(Class))
                {
                    if ((uclass + "(サポート)" ?? "") == pclass)
                    {
                        return true;
                    }
                }

                // パイロットが乗っていないユニットの場合はここで終了
                if (u.CountPilot() == 0)
                {
                    return false;
                }

                // 専属サポート？
                var p = u.MainPilot();
                foreach (var pclass in GeneralLib.ToL(Class))
                {
                    if (pclass == (uclass + "(" + p.Name + "専属サポート)")
                        || pclass == (uclass + "(" + p.get_Nickname(false) + "専属サポート)")
                        || pclass == (uclass + "(" + p.Sex + "専属サポート)"))
                    {
                        return true;
                    }
                    foreach (var sname in p.Skills.Select(skill => skill.Name))
                    {
                        if (pclass == (uclass + "(" + sname + "専属サポート)" ?? ""))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // ユニット u に乗ることができるかどうか
        public bool IsAbleToRide(Unit u)
        {
            // 汎用ユニットは必要技能を満たせばＯＫ
            if (u.Class == "汎用")
            {
                goto CheckNecessarySkill;
            }

            // 人間ユニット指定を除いて判定
            string uclass;
            if (Strings.Left(u.Class, 1) == "(" & Strings.Right(u.Class, 1) == ")")
            {
                uclass = Strings.Mid(u.Class, 2, Strings.Len(u.Class) - 2);
            }
            else
            {
                uclass = u.Class;
            }

            // サポートかどうかをまず判定しておく
            if (IsSupport(u))
            {
                // 必要技能をチェックする
                goto CheckNecessarySkill;
            }

            // ユニットクラスは複数設定可能
            foreach (var pclass in GeneralLib.ToL(Class))
            {
                if (uclass == pclass
                    || uclass == (pclass + "(" + get_Nickname(false) + "専用)")
                    || uclass == (pclass + "(" + Name + "専用)")
                    || uclass == (pclass + "(" + Sex + "専用)"))
                {
                    // 必要技能をチェックする
                    goto CheckNecessarySkill;
                }
            }

            // クラスが合わない
            return false;
        CheckNecessarySkill:
            ;


            // 必要技能＆不必要技能をチェック

            // 両能力を持っていない場合はチェック不要
            if (!u.IsFeatureAvailable("必要技能") & !u.IsFeatureAvailable("不必要技能"))
            {
                return true;
            }

            foreach (var fd in u.Features.Where(fd => fd.Name == "必要技能"))
            {
                if (!u.IsNecessarySkillSatisfied(fd.Data, this))
                {
                    return false;
                }
            }
            foreach (var fd in u.Features.Where(fd => fd.Name == "不必要技能"))
            {
                if (u.IsNecessarySkillSatisfied(fd.Data, this))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
