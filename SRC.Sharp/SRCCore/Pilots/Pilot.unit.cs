// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

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

            // TODO Impl
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
                //short localInStrNotNest1() { string argstring1 = Class_Renamed; string argstring2 = "サポート)"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); this.Class_Renamed = argstring1; return ret; }

                //short localLLength() { string arglist = Class_Renamed; var ret = GeneralLib.LLength(arglist); this.Class_Renamed = arglist; return ret; }

                //string argfname = "ダミーユニット";
                //if (localInStrNotNest1() > 0 & localLLength() == 1 & !u.IsFeatureAvailable(argfname))
                //{
                //    // サポートにしかなれないパイロットの場合
                //    var argp = this;
                //    u.AddSupport(argp);
                //}
                //else if (IsSupport(u))
                //{
                //    // 同じユニットクラスに対して通常パイロットとサポートの両方のパターン
                //    // がいける場合は通常パイロットを優先
                //    short localInStrNotNest() { string argstring1 = Class_Renamed; string argstring2 = u.Class0 + " "; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); this.Class_Renamed = argstring1; return ret; }

                //    if (u.CountPilot() < Math.Abs(u.Data.PilotNum) & localInStrNotNest() > 0 & !is_support)
                //    {
                //        var argp2 = this;
                //        u.AddPilot(argp2);
                //    }
                //    else
                //    {
                //        var argp3 = this;
                //        u.AddSupport(argp3);
                //    }
                //}
                //else
                //{
                //    // パイロットが既に規定数の場合は全パイロットを降ろす
                //    if (u.CountPilot() == Math.Abs(u.Data.PilotNum))
                //    {
                //        object argIndex1 = 1;
                //        u.Pilot(argIndex1).GetOff();
                //    }

                //    var argp1 = this;
                //    u.AddPilot(argp1);
                //}

                //// Pilotコマンドで作成されたパイロットは全て味方なので搭乗時に変更が必要
                //Party = u.Party0;

                //// ユニットのステータスをアップデート
                //u.Update();

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

            // TODO Impl
            //{
            //    var withBlock = Unit_Renamed;
            //    var loopTo = withBlock.CountSupport();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex2 = i;
            //        if (ReferenceEquals(withBlock.Support(argIndex2), this))
            //        {
            //            // サポートパイロットとして乗り込んでいる場合
            //            object argIndex1 = i;
            //            withBlock.DeleteSupport(argIndex1);
            //            withBlock.Update();
            //            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            Unit_Renamed = null;
            //            Update();
            //            return;
            //        }
            //    }

            //    // 出撃していた場合は退却
            //    if (!without_leave)
            //    {
            //        if (withBlock.Status == "出撃")
            //        {
            //            withBlock.Status = "待機";
            //            // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            Map.MapDataForUnit[withBlock.x, withBlock.y] = null;
            //            GUI.EraseUnitBitmap(withBlock.x, withBlock.y, false);
            //        }
            //    }

            //    // 通常のパイロットの場合は、そのユニットに乗っていた他のパイロットも降ろされる
            //    var loopTo1 = withBlock.CountPilot();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        // UPGRADE_NOTE: オブジェクト Unit_Renamed.Pilot().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        object argIndex3 = 1;
            //        withBlock.Pilot(argIndex3).Unit_Renamed = null;
            //        object argIndex4 = 1;
            //        withBlock.DeletePilot(argIndex4);
            //    }

            //    var loopTo2 = withBlock.CountSupport();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        // UPGRADE_NOTE: オブジェクト Unit_Renamed.Support().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        object argIndex5 = 1;
            //        withBlock.Support(argIndex5).Unit_Renamed = null;
            //        object argIndex6 = 1;
            //        withBlock.DeleteSupport(argIndex6);
            //    }

            //    withBlock.Update();
            //}

            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Unit = null;
            Update();
        }

        // パイロットがユニット u のサポートかどうか
        public bool IsSupport(Unit u)
        {
            return false;
            // TODO Impl
            //bool IsSupportRet = default;
            //string uclass, pclass;
            //short i, j;
            //string argfname = "ダミーユニット";
            //if (u.IsFeatureAvailable(argfname))
            //{
            //    // ダミーユニットの場合はサポートパイロットも通常のパイロットとして扱う
            //    IsSupportRet = false;
            //    return IsSupportRet;
            //}

            //// サポート指定が存在する？
            //short localInStrNotNest() { string argstring1 = Class_Renamed; string argstring2 = "サポート)"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); this.Class_Renamed = argstring1; return ret; }

            //if (localInStrNotNest() == 0)
            //{
            //    IsSupportRet = false;
            //    return IsSupportRet;
            //}

            //if (u.CountPilot() == 0)
            //{
            //    // パイロットが乗っていないユニットの場合は通常パイロットを優先
            //    string arglist1 = Class_Renamed;
            //    var loopTo = GeneralLib.LLength(arglist1);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string arglist = Class_Renamed;
            //        pclass = GeneralLib.LIndex(arglist, i);
            //        this.Class_Renamed = arglist;
            //        if ((u.Class_Renamed ?? "") == (pclass ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + Name + "専用)" ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + get_Nickname(false) + "専用)" ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + Sex + "専用)" ?? ""))
            //        {
            //            // 通常のパイロットとして搭乗可能であればサポートでないとみなす
            //            IsSupportRet = false;
            //            return IsSupportRet;
            //        }
            //    }

            //    this.Class_Renamed = arglist1;
            //}
            //else
            //{
            //    // 通常のパイロットとして搭乗している？
            //    var loopTo1 = u.CountPilot();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        object argIndex1 = i;
            //        if (ReferenceEquals(u.Pilot(argIndex1), this))
            //        {
            //            IsSupportRet = false;
            //            return IsSupportRet;
            //        }
            //    }
            //}

            //uclass = u.Class0;

            //// 通常のサポート？
            //string arglist2 = Class_Renamed;
            //var loopTo2 = GeneralLib.LLength(arglist2);
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    string localLIndex() { string arglist = Class_Renamed; var ret = GeneralLib.LIndex(arglist, i); this.Class_Renamed = arglist; return ret; }

            //    if ((uclass + "(サポート)" ?? "") == (localLIndex() ?? ""))
            //    {
            //        IsSupportRet = true;
            //        return IsSupportRet;
            //    }
            //}

            //this.Class_Renamed = arglist2;

            //// パイロットが乗っていないユニットの場合はここで終了
            //if (u.CountPilot() == 0)
            //{
            //    IsSupportRet = false;
            //    return IsSupportRet;
            //}

            //// 専属サポート？
            //{
            //    var withBlock = u.MainPilot();
            //    string arglist4 = Class_Renamed;
            //    var loopTo3 = GeneralLib.LLength(arglist4);
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        string arglist3 = Class_Renamed;
            //        pclass = GeneralLib.LIndex(arglist3, i);
            //        this.Class_Renamed = arglist3;
            //        if ((pclass ?? "") == (uclass + "(" + withBlock.Name + "専属サポート)" ?? "") | (pclass ?? "") == (uclass + "(" + withBlock.get_Nickname(false) + "専属サポート)" ?? "") | (pclass ?? "") == (uclass + "(" + withBlock.Sex + "専属サポート)" ?? ""))
            //        {
            //            IsSupportRet = true;
            //            return IsSupportRet;
            //        }

            //        var loopTo4 = withBlock.CountSkill();
            //        for (j = 1; j <= loopTo4; j++)
            //        {
            //            string localSkill() { object argIndex1 = j; var ret = withBlock.Skill(argIndex1); return ret; }

            //            if ((pclass ?? "") == (uclass + "(" + localSkill() + "専属サポート)" ?? ""))
            //            {
            //                IsSupportRet = true;
            //                return IsSupportRet;
            //            }
            //        }
            //    }

            //    this.Class_Renamed = arglist4;
            //}

            //IsSupportRet = false;
            //return IsSupportRet;
        }

        // ユニット u に乗ることができるかどうか
        public bool IsAbleToRide(Unit u)
        {
            return true;
            // TODO Impl
            //bool IsAbleToRideRet = default;
            //string uclass, pclass;
            //short i;
            //{
            //    var withBlock = u;
            //    // 汎用ユニットは必要技能を満たせばＯＫ
            //    if (withBlock.Class_Renamed == "汎用")
            //    {
            //        IsAbleToRideRet = true;
            //        goto CheckNecessarySkill;
            //    }

            //    // 人間ユニット指定を除いて判定
            //    if (Strings.Left(withBlock.Class_Renamed, 1) == "(" & Strings.Right(withBlock.Class_Renamed, 1) == ")")
            //    {
            //        uclass = Strings.Mid(withBlock.Class_Renamed, 2, Strings.Len(withBlock.Class_Renamed) - 2);
            //    }
            //    else
            //    {
            //        uclass = withBlock.Class_Renamed;
            //    }

            //    // サポートかどうかをまず判定しておく
            //    if (IsSupport(u))
            //    {
            //        IsAbleToRideRet = true;
            //        // 必要技能をチェックする
            //        goto CheckNecessarySkill;
            //    }

            //    string arglist1 = Class_Renamed;
            //    var loopTo = GeneralLib.LLength(arglist1);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string arglist = Class_Renamed;
            //        pclass = GeneralLib.LIndex(arglist, i);
            //        this.Class_Renamed = arglist;
            //        if ((uclass ?? "") == (pclass ?? "") | (uclass ?? "") == (pclass + "(" + get_Nickname(false) + "専用)" ?? "") | (uclass ?? "") == (pclass + "(" + Name + "専用)" ?? "") | (uclass ?? "") == (pclass + "(" + Sex + "専用)" ?? ""))
            //        {
            //            IsAbleToRideRet = true;
            //            // 必要技能をチェックする
            //            goto CheckNecessarySkill;
            //        }
            //    }

            //    this.Class_Renamed = arglist1; // ユニットクラスは複数設定可能

            //    // クラスが合わない
            //    IsAbleToRideRet = false;
            //    return IsAbleToRideRet;
            //CheckNecessarySkill:
            //    ;


            //    // 必要技能＆不必要技能をチェック

            //    // 両能力を持っていない場合はチェック不要
            //    string argfname = "必要技能";
            //    string argfname1 = "不必要技能";
            //    if (!withBlock.IsFeatureAvailable(argfname) & !withBlock.IsFeatureAvailable(argfname1))
            //    {
            //        return IsAbleToRideRet;
            //    }

            //    var loopTo1 = withBlock.CountFeature();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(argIndex1); return ret; }

            //        object argIndex1 = i;
            //        if (withBlock.Feature(argIndex1) == "必要技能")
            //        {
            //            string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(argIndex1); return ret; }

            //            bool localIsNecessarySkillSatisfied() { string argnabilities = hs98e3424915b54fdbb83dbc9aa23c37a5(); var argp = this; var ret = withBlock.IsNecessarySkillSatisfied(argnabilities, argp); return ret; }

            //            if (!localIsNecessarySkillSatisfied())
            //            {
            //                IsAbleToRideRet = false;
            //                return IsAbleToRideRet;
            //            }
            //        }
            //        else if (localFeature() == "不必要技能")
            //        {
            //            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(argIndex1); return ret; }

            //            string argnabilities = localFeatureData1();
            //            var argp = this;
            //            if (withBlock.IsNecessarySkillSatisfied(argnabilities, argp))
            //            {
            //                IsAbleToRideRet = false;
            //                return IsAbleToRideRet;
            //            }
            //        }
            //    }
            //}

            //return IsAbleToRideRet;
        }
    }
}
