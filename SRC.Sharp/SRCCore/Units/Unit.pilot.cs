using SRCCore.Exceptions;
using SRCCore.Pilots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Units
{
    // === パイロット関連処理 ===
    public partial class Unit
    {
        // パイロットを追加
        public void AddPilot(Pilot p)
        {
            colPilot.Add(p, p.ID);
        }

        // パイロットを削除
        public void DeletePilot(string Index)
        {
            colPilot.Remove(Index);
        }

        //// パイロットの入れ替え
        //public void ReplacePilot(Pilot p, string Index)
        //{
        //    int i;
        //    Pilot prev_p;
        //    Pilot[] pilot_list;
        //    p.Unit_Renamed = this;
        //    prev_p = (Pilot)colPilot[Index];
        //    pilot_list = new Pilot[colPilot.Count + 1];
        //    var loopTo = Information.UBound(pilot_list);
        //    for (i = 1; i <= loopTo; i++)
        //        pilot_list[i] = (Pilot)colPilot[i];
        //    var loopTo1 = Information.UBound(pilot_list);
        //    for (i = 1; i <= loopTo1; i++)
        //        colPilot.Remove(1);
        //    var loopTo2 = Information.UBound(pilot_list);
        //    for (i = 1; i <= loopTo2; i++)
        //    {
        //        if (erenceEquals(pilot_list[i], prev_p))
        //        {
        //            colPilot.Add(p, p.ID);
        //        }
        //        else
        //        {
        //            colPilot.Add(pilot_list[i], pilot_list[i].ID);
        //        }
        //    }
        //    // UPGRADE_NOTE: オブジェクト prev_p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //    prev_p.Unit_Renamed = null;
        //    prev_p.Alive = false;
        //}

        // 搭乗員数
        public int CountPilot()
        {
            return colPilot.Count;
        }

        // パイロット
        public Pilot Pilot(string Index)
        {
            return colPilot[Index];
        }

        // メインパイロット
        // 各判定にはこのパイロットの能力を用いる
        public Pilot MainPilot(bool without_update = false)
        {
            // パイロットが乗っていない？
            if (CountPilot() == 0)
            {
                if (!IsFeatureAvailable("追加パイロット"))
                {
                    throw new TerminateException("ユニット「" + Name + "」にパイロットが乗っていません");
                }
            }

            // 破棄された場合はメインパイロットの変更を行わない
            if (Status == "破棄")
            {
                return colPilot[1];
            }

            //// 能力コピー中は同じパイロットが複数のユニットのメインパイロットに使用されるのを防ぐため
            //// 追加パイロットと暴走時パイロットを使用しない
            //object argIndex1 = "能力コピー";
            //if (IsConditionSatisfied(argIndex1))
            //{
            //    MainPilotRet = colPilot[1];
            //    return MainPilotRet;
            //}

            //// 暴走時の特殊パイロット
            //object argIndex12 = "暴走";
            //if (IsConditionSatisfied(argIndex12))
            //{
            //    string argfname1 = "暴走時パイロット";
            //    if (IsFeatureAvailable(argfname1))
            //    {
            //        object argIndex2 = "暴走時パイロット";
            //        pname = FeatureData(argIndex2);
            //        object argIndex3 = pname;
            //        if (SRC.PDList.IsDefined(argIndex3))
            //        {
            //            PilotData localItem() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //            pname = localItem().Name;
            //        }
            //        else
            //        {
            //            string argmsg1 = "暴走時パイロット「" + pname + "」のデータが定義されていません";
            //            GUI.ErrorMessage(argmsg1);
            //        }

            //        object argIndex11 = pname;
            //        if (SRC.PList.IsDefined(argIndex11))
            //        {
            //            // 既に暴走時パイロットが作成済み
            //            object argIndex4 = pname;
            //            MainPilotRet = SRC.PList.Item(argIndex4);
            //            MainPilotRet.Unit_Renamed = this;
            //            object argIndex5 = 1;
            //            MainPilotRet.Morale = Pilot(argIndex5).Morale;
            //            object argIndex6 = 1;
            //            MainPilotRet.Level = Pilot(argIndex6).Level;
            //            object argIndex7 = 1;
            //            MainPilotRet.Exp = Pilot(argIndex7).Exp;
            //            if (!without_update)
            //            {
            //                if (!erenceEquals(MainPilotRet.Unit_Renamed, this))
            //                {
            //                    MainPilotRet.Unit_Renamed = this;
            //                    MainPilotRet.Update();
            //                    MainPilotRet.UpdateSupportMod();
            //                }
            //            }

            //            return MainPilotRet;
            //        }
            //        else
            //        {
            //            // 暴走時パイロットが作成されていないので作成する
            //            object argIndex8 = 1;
            //            string argpparty = Party0;
            //            string arggid = "";
            //            MainPilotRet = SRC.PList.Add(pname, Pilot(argIndex8).Level, argpparty, gid: arggid);
            //            this.Party0 = argpparty;
            //            object argIndex9 = 1;
            //            MainPilotRet.Morale = Pilot(argIndex9).Morale;
            //            object argIndex10 = 1;
            //            MainPilotRet.Exp = Pilot(argIndex10).Exp;
            //            MainPilotRet.Unit_Renamed = this;
            //            MainPilotRet.Update();
            //            MainPilotRet.UpdateSupportMod();
            //            return MainPilotRet;
            //        }
            //    }
            //}

            //// 追加パイロットがいれば、それを使用
            //string argfname2 = "追加パイロット";
            //if (IsFeatureAvailable(argfname2))
            //{
            //    object argIndex13 = "追加パイロット";
            //    pname = FeatureData(argIndex13);
            //    object argIndex14 = pname;
            //    if (SRC.PDList.IsDefined(argIndex14))
            //    {
            //        PilotData localItem1() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //        pname = localItem1().Name;
            //    }
            //    else
            //    {
            //        string argmsg2 = "追加パイロット「" + pname + "」のデータが定義されていません";
            //        GUI.ErrorMessage(argmsg2);
            //    }

            //    // 登録済みのパイロットをまずチェック
            //    if (pltAdditionalPilot is object)
            //    {
            //        if ((pltAdditionalPilot.Name ?? "") == (pname ?? ""))
            //        {
            //            MainPilotRet = pltAdditionalPilot;
            //            {
            //                var withBlock = pltAdditionalPilot;
            //                if (withBlock.IsAdditionalPilot & !erenceEquals(withBlock.Unit_Renamed, this))
            //                {
            //                    withBlock.Unit_Renamed = this;
            //                    withBlock.Party = Party0;
            //                    object argIndex15 = 1;
            //                    withBlock.Exp = Pilot(argIndex15).Exp;
            //                    if (withBlock.Personality != "機械")
            //                    {
            //                        object argIndex16 = 1;
            //                        withBlock.Morale = Pilot(argIndex16).Morale;
            //                    }

            //                    object argIndex18 = 1;
            //                    if (withBlock.Level != this.Pilot(argIndex18).Level)
            //                    {
            //                        object argIndex17 = 1;
            //                        withBlock.Level = Pilot(argIndex17).Level;
            //                        withBlock.Update();
            //                    }
            //                }
            //            }

            //            return MainPilotRet;
            //        }
            //    }

            //    var loopTo = CountOtherForm();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        Unit localOtherForm2() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //        Unit localOtherForm3() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //        if (localOtherForm3().pltAdditionalPilot is object)
            //        {
            //            Unit localOtherForm1() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //            {
            //                var withBlock1 = localOtherForm1().pltAdditionalPilot;
            //                if ((withBlock1.Name ?? "") == (pname ?? ""))
            //                {
            //                    Unit localOtherForm() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //                    pltAdditionalPilot = localOtherForm().pltAdditionalPilot;
            //                    withBlock1.Party = Party0;
            //                    withBlock1.Unit_Renamed = this;
            //                    if (withBlock1.IsAdditionalPilot & !erenceEquals(withBlock1.Unit_Renamed, this))
            //                    {
            //                        object argIndex19 = 1;
            //                        withBlock1.Level = Pilot(argIndex19).Level;
            //                        object argIndex20 = 1;
            //                        withBlock1.Exp = Pilot(argIndex20).Exp;
            //                        if (withBlock1.Personality != "機械")
            //                        {
            //                            object argIndex21 = 1;
            //                            withBlock1.Morale = Pilot(argIndex21).Morale;
            //                        }

            //                        withBlock1.Update();
            //                        withBlock1.UpdateSupportMod();
            //                    }

            //                    MainPilotRet = pltAdditionalPilot;
            //                    return MainPilotRet;
            //                }
            //            }
            //        }
            //    }

            //    // 次に搭乗しているパイロットから検索
            //    if (CountPilot() > 0)
            //    {
            //        // 単なるメインパイロットの交代として扱うため、IsAdditionalPilotのフラグは立てない
            //        var loopTo1 = CountPilot();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            if ((localPilot().Name ?? "") == (pname ?? ""))
            //            {
            //                object argIndex22 = i;
            //                pltAdditionalPilot = Pilot(argIndex22);
            //                MainPilotRet = pltAdditionalPilot;
            //                return MainPilotRet;
            //            }
            //        }
            //    }

            //    // 既に作成されていればそれを使う
            //    // (ただし複数作成可能なパイロットで、他のユニットの追加パイロットとして登録済みの場合は除く)
            //    object argIndex27 = pname;
            //    if (SRC.PList.IsDefined(argIndex27))
            //    {
            //        object argIndex23 = pname;
            //        p = SRC.PList.Item(argIndex23);
            //        if (!p.IsAdditionalPilot | Strings.InStr(pname, "(ザコ)") == 0 & Strings.InStr(pname, "(汎用)") == 0)
            //        {
            //            pltAdditionalPilot = p;
            //            {
            //                var withBlock2 = pltAdditionalPilot;
            //                withBlock2.IsAdditionalPilot = true;
            //                withBlock2.Party = Party0;
            //                object argIndex24 = 1;
            //                withBlock2.Level = Pilot(argIndex24).Level;
            //                object argIndex25 = 1;
            //                withBlock2.Exp = Pilot(argIndex25).Exp;
            //                if (withBlock2.Personality != "機械")
            //                {
            //                    object argIndex26 = 1;
            //                    withBlock2.Morale = Pilot(argIndex26).Morale;
            //                }

            //                if (!without_update)
            //                {
            //                    if (!erenceEquals(withBlock2.Unit_Renamed, this))
            //                    {
            //                        withBlock2.Unit_Renamed = this;
            //                        withBlock2.Update();
            //                        withBlock2.UpdateSupportMod();
            //                    }
            //                }
            //                else
            //                {
            //                    withBlock2.Unit_Renamed = this;
            //                }
            //            }

            //            MainPilotRet = pltAdditionalPilot;
            //            return MainPilotRet;
            //        }
            //    }

            //    // まだ作成されていないので作成する
            //    if (CountPilot() > 0)
            //    {
            //        object argIndex28 = 1;
            //        string argpparty1 = Party0;
            //        string arggid1 = "";
            //        pltAdditionalPilot = SRC.PList.Add(pname, Pilot(argIndex28).Level, argpparty1, gid: arggid1);
            //        this.Party0 = argpparty1;
            //        {
            //            var withBlock3 = pltAdditionalPilot;
            //            withBlock3.IsAdditionalPilot = true;
            //            object argIndex29 = 1;
            //            withBlock3.Exp = Pilot(argIndex29).Exp;
            //            if (withBlock3.Personality != "機械")
            //            {
            //                object argIndex30 = 1;
            //                withBlock3.Morale = Pilot(argIndex30).Morale;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        string argpparty2 = Party0;
            //        string arggid2 = "";
            //        pltAdditionalPilot = SRC.PList.Add(pname, 1, argpparty2, gid: arggid2);
            //        this.Party0 = argpparty2;
            //        pltAdditionalPilot.IsAdditionalPilot = true;
            //    }

            //    {
            //        var withBlock4 = pltAdditionalPilot;
            //        withBlock4.Unit_Renamed = this;
            //        if (!without_update)
            //        {
            //            withBlock4.Update();
            //            withBlock4.UpdateSupportMod();
            //        }
            //    }

            //    MainPilotRet = pltAdditionalPilot;
            //    return MainPilotRet;
            //}

            // そうでなければ第１パイロットを使用
            return colPilot[1];
        }

        public IEnumerable<Pilot> SubPilots => colPilot.List.Skip(1);
        public IEnumerable<Pilot> Supports => colPilot.List;

        // サポートパイロットを追加
        public void AddSupport(Pilot p)
        {
            colSupport.Add(p, p.Name);
        }

        // サポートパイロットを削除
        public void DeleteSupport(string Index)
        {
            colSupport.Remove(Index);
        }

        //// サポートパイロットの入れ替え
        //public void ReplaceSupport(Pilot p, string Index)
        //{
        //    int i;
        //    Pilot prev_p;
        //    Pilot[] support_list;
        //    p.Unit_Renamed = this;
        //    prev_p = (Pilot)colSupport[Index];
        //    support_list = new Pilot[colSupport.Count + 1];
        //    var loopTo = Information.UBound(support_list);
        //    for (i = 1; i <= loopTo; i++)
        //        support_list[i] = (Pilot)colSupport[i];
        //    var loopTo1 = Information.UBound(support_list);
        //    for (i = 1; i <= loopTo1; i++)
        //        colSupport.Remove(1);
        //    var loopTo2 = Information.UBound(support_list);
        //    for (i = 1; i <= loopTo2; i++)
        //    {
        //        if ((support_list[i].ID ?? "") == (prev_p.ID ?? ""))
        //        {
        //            colSupport.Add(p, p.ID);
        //        }
        //        else
        //        {
        //            colSupport.Add(support_list[i], support_list[i].ID);
        //        }
        //    }
        //    // UPGRADE_NOTE: オブジェクト prev_p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //    prev_p.Unit_Renamed = null;
        //    prev_p.Alive = false;
        //}

        // 総サポートパイロット数
        public int CountSupport()
        {
            return colSupport.Count;
        }

        // サポート
        public Pilot Support(string Index)
        {
            return colSupport[Index];
        }

        // 追加サポート
        public Pilot AdditionalSupport()
        {
            throw new NotImplementedException();
            //    Pilot AdditionalSupportRet = default;
            //    string pname;
            //    Pilot p;
            //    int i;

            //    // 追加サポートパイロットの名称
            //    object argIndex1 = "追加サポート";
            //    pname = FeatureData(argIndex1);

            //    // 追加サポートが存在しない？
            //    if (string.IsNullOrEmpty(pname))
            //    {
            //        return AdditionalSupportRet;
            //    }

            //    // 他にパイロットが乗っていない場合は無効
            //    if (CountPilot() == 0)
            //    {
            //        return AdditionalSupportRet;
            //    }

            //    // 既に登録済みであるかチェック
            //    if (pltAdditionalSupport is object)
            //    {
            //        if ((pltAdditionalSupport.Name ?? "") == (pname ?? ""))
            //        {
            //            AdditionalSupportRet = pltAdditionalSupport;
            //            pltAdditionalSupport.Unit_Renamed = this;
            //            return AdditionalSupportRet;
            //        }
            //    }

            //    var loopTo = CountOtherForm();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex2 = i;
            //        {
            //            var withBlock = OtherForm(argIndex2);
            //            if (withBlock.pltAdditionalSupport is object)
            //            {
            //                if ((withBlock.pltAdditionalSupport.Name ?? "") == (pname ?? ""))
            //                {
            //                    withBlock.pltAdditionalSupport.Unit_Renamed = this;
            //                    AdditionalSupportRet = withBlock.pltAdditionalSupport;
            //                    return AdditionalSupportRet;
            //                }
            //            }
            //        }
            //    }

            //    // 既に作成されていればそれを使う
            //    // (ただし他のユニットの追加サポートとして登録済みの場合は除く)
            //    object argIndex7 = pname;
            //    if (SRC.PList.IsDefined(argIndex7))
            //    {
            //        object argIndex3 = pname;
            //        p = SRC.PList.Item(argIndex3);
            //        if (!p.IsAdditionalSupport | Strings.InStr(pname, "(ザコ)") == 0 & Strings.InStr(pname, "(汎用)") == 0)
            //        {
            //            pltAdditionalSupport = p;
            //            {
            //                var withBlock1 = pltAdditionalSupport;
            //                withBlock1.IsAdditionalSupport = true;
            //                withBlock1.Party = Party0;
            //                withBlock1.Unit_Renamed = this;
            //                object argIndex4 = 1;
            //                withBlock1.Level = Pilot(argIndex4).Level;
            //                object argIndex5 = 1;
            //                withBlock1.Exp = Pilot(argIndex5).Exp;
            //                if (withBlock1.Personality != "機械")
            //                {
            //                    object argIndex6 = 1;
            //                    withBlock1.Morale = Pilot(argIndex6).Morale;
            //                }
            //            }

            //            AdditionalSupportRet = pltAdditionalSupport;
            //            return AdditionalSupportRet;
            //        }
            //    }

            //    // まだ作成されていないので作成する
            //    bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //    if (!localIsDefined())
            //    {
            //        string argmsg = "追加サポート「" + pname + "」のデータが定義されていません";
            //        GUI.ErrorMessage(argmsg);
            //        return AdditionalSupportRet;
            //    }

            //    object argIndex8 = 1;
            //    string argpparty = Party0;
            //    string arggid = "";
            //    pltAdditionalSupport = SRC.PList.Add(pname, Pilot(argIndex8).Level, argpparty, gid: arggid);
            //    this.Party0 = argpparty;
            //    {
            //        var withBlock2 = pltAdditionalSupport;
            //        withBlock2.IsAdditionalSupport = true;
            //        withBlock2.Unit_Renamed = this;
            //        object argIndex9 = 1;
            //        withBlock2.Exp = Pilot(argIndex9).Exp;
            //        if (withBlock2.Personality != "機械")
            //        {
            //            object argIndex10 = 1;
            //            withBlock2.Morale = Pilot(argIndex10).Morale;
            //        }
            //    }

            //    AdditionalSupportRet = pltAdditionalSupport;
            //    return AdditionalSupportRet;
        }

        //// いずれかのパイロットが特殊能力 sname を持っているか判定
        //public bool IsSkillAvailable(string sname)
        //{
        //    bool IsSkillAvailableRet = default;
        //    int i;
        //    if (CountPilot() == 0)
        //    {
        //        return IsSkillAvailableRet;
        //    }

        //    // メインパイロット
        //    if (MainPilot().IsSkillAvailable(sname))
        //    {
        //        IsSkillAvailableRet = true;
        //        return IsSkillAvailableRet;
        //    }

        //    // パイロット数が負の場合はメインパイロットの能力のみが有効
        //    if (Data.PilotNum > 0)
        //    {
        //        var loopTo = CountPilot();
        //        for (i = 2; i <= loopTo; i++)
        //        {
        //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //            if (localPilot().IsSkillAvailable(sname))
        //            {
        //                IsSkillAvailableRet = true;
        //                return IsSkillAvailableRet;
        //            }
        //        }
        //    }

        //    // サポート
        //    var loopTo1 = CountSupport();
        //    for (i = 1; i <= loopTo1; i++)
        //    {
        //        Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

        //        if (localSupport().IsSkillAvailable(sname))
        //        {
        //            IsSkillAvailableRet = true;
        //            return IsSkillAvailableRet;
        //        }
        //    }

        //    // 追加サポート
        //    string argfname = "追加サポート";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        if (AdditionalSupport().IsSkillAvailable(sname))
        //        {
        //            IsSkillAvailableRet = true;
        //            return IsSkillAvailableRet;
        //        }
        //    }

        //    IsSkillAvailableRet = false;
        //    return IsSkillAvailableRet;
        //}

        //// パイロット全員によるパイロット能力レベル
        //public double SkillLevel(string sname, double default_slevel = 1d)
        //{
        //    double SkillLevelRet = default;
        //    if (CountPilot() == 0)
        //    {
        //        return SkillLevelRet;
        //    }

        //    // エリアスが設定されてるかチェック
        //    object argIndex1 = sname;
        //    if (SRC.ALDList.IsDefined(argIndex1))
        //    {
        //        AliasDataType localItem() { object argIndex1 = sname; var ret = SRC.ALDList.Item(argIndex1); return ret; }

        //        sname = localItem().get_AliasType(1);
        //    }

        //    switch (sname ?? "")
        //    {
        //        case "同調率":
        //            {
        //                SkillLevelRet = SyncLevel();
        //                break;
        //            }

        //        case "霊力":
        //            {
        //                SkillLevelRet = PlanaLevel();
        //                break;
        //            }

        //        case "オーラ":
        //            {
        //                SkillLevelRet = AuraLevel();
        //                break;
        //            }

        //        case "超能力":
        //            {
        //                SkillLevelRet = PsychicLevel();
        //                break;
        //            }

        //        case "Ｓ防御":
        //        case "切り払い":
        //            {
        //                object argIndex2 = sname;
        //                string arg_mode = 1.ToString();
        //                SkillLevelRet = MainPilot().SkillLevel(argIndex2, arg_mode);
        //                break;
        //            }

        //        case "超感覚":
        //            {
        //                string argsname2 = "超感覚";
        //                string argsname3 = "知覚強化";
        //                if (MaxSkillLevel(argsname2, 1d) > MaxSkillLevel(argsname3, 1d))
        //                {
        //                    string argsname = "超感覚";
        //                    SkillLevelRet = MaxSkillLevel(argsname, 1d);
        //                }
        //                else
        //                {
        //                    string argsname1 = "知覚強化";
        //                    SkillLevelRet = MaxSkillLevel(argsname1, 1d);
        //                }

        //                break;
        //            }

        //        default:
        //            {
        //                SkillLevelRet = MaxSkillLevel(sname, default_slevel);
        //                break;
        //            }
        //    }

        //    return SkillLevelRet;
        //}

        //// パイロット中での最も高いパイロット能力レベルを返す
        //private double MaxSkillLevel(string sname, double default_slevel)
        //{
        //    double MaxSkillLevelRet = default;
        //    double slevel;
        //    int i;
        //    if (CountPilot() == 0)
        //    {
        //        return MaxSkillLevelRet;
        //    }

        //    // メインパイロット
        //    {
        //        var withBlock = MainPilot();
        //        object argIndex2 = sname;
        //        if (withBlock.IsSkillLevelSpecified(argIndex2))
        //        {
        //            object argIndex1 = sname;
        //            string arg_mode = "";
        //            MaxSkillLevelRet = withBlock.SkillLevel(argIndex1, _mode: arg_mode);
        //        }
        //        else if (withBlock.IsSkillAvailable(sname))
        //        {
        //            MaxSkillLevelRet = default_slevel;
        //        }
        //        else
        //        {
        //            MaxSkillLevelRet = 0d;
        //        }
        //    }

        //    // パイロット数が負の場合はメインパイロットの能力のみが有効
        //    if (Data.PilotNum > 0)
        //    {
        //        var loopTo = CountPilot();
        //        for (i = 2; i <= loopTo; i++)
        //        {
        //            object argIndex5 = i;
        //            {
        //                var withBlock1 = Pilot(argIndex5);
        //                object argIndex4 = sname;
        //                if (withBlock1.IsSkillLevelSpecified(argIndex4))
        //                {
        //                    object argIndex3 = sname;
        //                    string arg_mode1 = "";
        //                    slevel = withBlock1.SkillLevel(argIndex3, _mode: arg_mode1);
        //                }
        //                else if (withBlock1.IsSkillAvailable(sname))
        //                {
        //                    slevel = default_slevel;
        //                }
        //                else
        //                {
        //                    slevel = 0d;
        //                }

        //                if (slevel > MaxSkillLevelRet)
        //                {
        //                    MaxSkillLevelRet = slevel;
        //                }
        //            }
        //        }
        //    }

        //    // サポート
        //    var loopTo1 = CountSupport();
        //    for (i = 1; i <= loopTo1; i++)
        //    {
        //        object argIndex8 = i;
        //        {
        //            var withBlock2 = Support(argIndex8);
        //            object argIndex7 = sname;
        //            if (withBlock2.IsSkillLevelSpecified(argIndex7))
        //            {
        //                object argIndex6 = sname;
        //                string arg_mode2 = "";
        //                slevel = withBlock2.SkillLevel(argIndex6, _mode: arg_mode2);
        //            }
        //            else if (withBlock2.IsSkillAvailable(sname))
        //            {
        //                slevel = default_slevel;
        //            }
        //            else
        //            {
        //                slevel = 0d;
        //            }

        //            if (slevel > MaxSkillLevelRet)
        //            {
        //                MaxSkillLevelRet = slevel;
        //            }
        //        }
        //    }

        //    // 追加サポート
        //    string argfname = "追加サポート";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        {
        //            var withBlock3 = AdditionalSupport();
        //            object argIndex10 = sname;
        //            if (withBlock3.IsSkillLevelSpecified(argIndex10))
        //            {
        //                object argIndex9 = sname;
        //                string arg_mode3 = "";
        //                slevel = withBlock3.SkillLevel(argIndex9, _mode: arg_mode3);
        //            }
        //            else if (withBlock3.IsSkillAvailable(sname))
        //            {
        //                slevel = default_slevel;
        //            }
        //            else
        //            {
        //                slevel = 0d;
        //            }

        //            if (slevel > MaxSkillLevelRet)
        //            {
        //                MaxSkillLevelRet = slevel;
        //            }
        //        }
        //    }

        //    return MaxSkillLevelRet;
        //}

        //// ユニットのオーラ力レベル
        //public double AuraLevel(bool no_limit = false)
        //{
        //    double AuraLevelRet = default;
        //    switch (CountPilot())
        //    {
        //        case 0:
        //            {
        //                return AuraLevelRet;
        //            }

        //        case 1:
        //            {
        //                object argIndex1 = "オーラ";
        //                string arg_mode = "";
        //                AuraLevelRet = MainPilot().SkillLevel(argIndex1, _mode: arg_mode);
        //                break;
        //            }

        //        default:
        //            {
        //                // パイロットが２名以上の場合は２人目のオーラ力を加算
        //                object argIndex2 = "オーラ";
        //                string arg_mode1 = "";
        //                object argIndex3 = 2;
        //                object argIndex4 = "オーラ";
        //                string arg_mode2 = "";
        //                AuraLevelRet = MainPilot().SkillLevel(argIndex2, _mode: arg_mode1) + Pilot(argIndex3).SkillLevel(argIndex4, _mode: arg_mode2) / 2d;
        //                break;
        //            }
        //    }

        //    // サポートのオーラ力を加算
        //    string argfname = "追加サポート";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        object argIndex5 = "オーラ";
        //        string arg_mode3 = "";
        //        AuraLevelRet = AuraLevelRet + AdditionalSupport().SkillLevel(argIndex5, _mode: arg_mode3) / 2d;
        //    }
        //    else if (CountSupport() > 0)
        //    {
        //        object argIndex6 = 1;
        //        object argIndex7 = "オーラ";
        //        string arg_mode4 = "";
        //        AuraLevelRet = AuraLevelRet + Support(argIndex6).SkillLevel(argIndex7, _mode: arg_mode4) / 2d;
        //    }

        //    // オーラ変換器レベルによる制限
        //    string argfname1 = "オーラ変換器";
        //    if (IsFeatureAvailable(argfname1) & !no_limit)
        //    {
        //        object argIndex9 = "オーラ変換器";
        //        if (IsFeatureLevelSpecified(argIndex9))
        //        {
        //            object argIndex8 = "オーラ変換器";
        //            AuraLevelRet = GeneralLib.MinDbl(AuraLevelRet, FeatureLevel(argIndex8));
        //        }
        //    }

        //    return AuraLevelRet;
        //}

        //// ユニットの超能力レベル
        //public double PsychicLevel(bool no_limit = false)
        //{
        //    double PsychicLevelRet = default;
        //    switch (CountPilot())
        //    {
        //        case 0:
        //            {
        //                return PsychicLevelRet;
        //            }

        //        case 1:
        //            {
        //                object argIndex1 = "超能力";
        //                string arg_mode = "";
        //                PsychicLevelRet = MainPilot().SkillLevel(argIndex1, _mode: arg_mode);
        //                break;
        //            }

        //        default:
        //            {
        //                // パイロットが２名以上の場合は２人目の超能力を加算
        //                object argIndex2 = "超能力";
        //                string arg_mode1 = "";
        //                object argIndex3 = 2;
        //                object argIndex4 = "超能力";
        //                string arg_mode2 = "";
        //                PsychicLevelRet = MainPilot().SkillLevel(argIndex2, _mode: arg_mode1) + Pilot(argIndex3).SkillLevel(argIndex4, _mode: arg_mode2) / 2d;
        //                break;
        //            }
        //    }

        //    // サポートのオーラ力を加算
        //    string argfname = "追加サポート";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        object argIndex5 = "超能力";
        //        string arg_mode3 = "";
        //        PsychicLevelRet = PsychicLevelRet + AdditionalSupport().SkillLevel(argIndex5, _mode: arg_mode3) / 2d;
        //    }
        //    else if (CountSupport() > 0)
        //    {
        //        // サポートの超能力を加算
        //        object argIndex6 = 1;
        //        object argIndex7 = "超能力";
        //        string arg_mode4 = "";
        //        PsychicLevelRet = PsychicLevelRet + Support(argIndex6).SkillLevel(argIndex7, _mode: arg_mode4) / 2d;
        //    }

        //    // サイキックドライブによる制限
        //    string argfname1 = "サイキックドライブ";
        //    if (IsFeatureAvailable(argfname1) & !no_limit)
        //    {
        //        object argIndex9 = "サイキックドライブ";
        //        if (IsFeatureLevelSpecified(argIndex9))
        //        {
        //            object argIndex8 = "サイキックドライブ";
        //            PsychicLevelRet = GeneralLib.MinDbl(PsychicLevelRet, FeatureLevel(argIndex8));
        //        }
        //    }

        //    return PsychicLevelRet;
        //}

        //// ユニットの同調率
        //public double SyncLevel(bool no_limit = false)
        //{
        //    double SyncLevelRet = default;
        //    if (CountPilot() == 0)
        //    {
        //        return SyncLevelRet;
        //    }

        //    SyncLevelRet = MainPilot().SynchroRate();

        //    // シンクロドライブレベルによる制限
        //    string argfname = "シンクロドライブ";
        //    if (IsFeatureAvailable(argfname) & !no_limit)
        //    {
        //        object argIndex2 = "シンクロドライブ";
        //        if (IsFeatureLevelSpecified(argIndex2))
        //        {
        //            object argIndex1 = "シンクロドライブ";
        //            SyncLevelRet = GeneralLib.MinDbl(SyncLevelRet, FeatureLevel(argIndex1));
        //        }
        //    }

        //    return SyncLevelRet;
        //}

        //// ユニットの霊力レベル
        //public double PlanaLevel(bool no_limit = false)
        //{
        //    double PlanaLevelRet = default;
        //    if (CountPilot() == 0)
        //    {
        //        return PlanaLevelRet;
        //    }

        //    PlanaLevelRet = MainPilot().Plana;

        //    // 霊力変換器レベルによる制限
        //    string argfname = "霊力変換器";
        //    if (IsFeatureAvailable(argfname) & !no_limit)
        //    {
        //        object argIndex2 = "霊力変換器";
        //        if (IsFeatureLevelSpecified(argIndex2))
        //        {
        //            object argIndex1 = "霊力変換器";
        //            PlanaLevelRet = GeneralLib.MinDbl(PlanaLevelRet, FeatureLevel(argIndex1));
        //        }
        //    }

        //    return PlanaLevelRet;
        //}

        //// パイロット全員からパイロット能力名を検索
        //public string SkillName0(string sname)
        //{
        //    string SkillName0Ret = default;
        //    int i;
        //    object argIndex1 = sname;
        //    if (SRC.ALDList.IsDefined(argIndex1))
        //    {
        //        AliasDataType localItem() { object argIndex1 = sname; var ret = SRC.ALDList.Item(argIndex1); return ret; }

        //        sname = localItem().get_AliasType(1);
        //    }

        //    if (CountPilot() == 0)
        //    {
        //        SkillName0Ret = sname;
        //        return SkillName0Ret;
        //    }

        //    // メインパイロット
        //    object argIndex2 = sname;
        //    SkillName0Ret = MainPilot().SkillName0(argIndex2);
        //    if ((SkillName0Ret ?? "") != (sname ?? ""))
        //    {
        //        return SkillName0Ret;
        //    }

        //    // パイロット数が負の場合はメインパイロットの能力のみが有効
        //    if (Data.PilotNum > 0)
        //    {
        //        var loopTo = CountPilot();
        //        for (i = 2; i <= loopTo; i++)
        //        {
        //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //            object argIndex3 = sname;
        //            SkillName0Ret = localPilot().SkillName0(argIndex3);
        //            if ((SkillName0Ret ?? "") != (sname ?? ""))
        //            {
        //                return SkillName0Ret;
        //            }
        //        }
        //    }

        //    // サポート
        //    var loopTo1 = CountSupport();
        //    for (i = 1; i <= loopTo1; i++)
        //    {
        //        Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

        //        object argIndex4 = sname;
        //        SkillName0Ret = localSupport().SkillName0(argIndex4);
        //        if ((SkillName0Ret ?? "") != (sname ?? ""))
        //        {
        //            return SkillName0Ret;
        //        }
        //    }

        //    // 追加サポート
        //    string argfname = "追加サポート";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        object argIndex5 = sname;
        //        SkillName0Ret = AdditionalSupport().SkillName0(argIndex5);
        //    }

        //    return SkillName0Ret;
        //}

    }
}
