using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Pilots;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void DeletePilot(Pilot p)
        {
            colPilot.Remove(p);
        }

        //// パイロットの入れ替え
        //public void ReplacePilot(Pilot p, string Index)
        //{
        //    int i;
        //    Pilot prev_p;
        //    Pilot[] pilot_list;
        //    p.Unit = this;
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
        //    prev_p.Unit = null;
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
            //if (IsConditionSatisfied("能力コピー"))
            //{
            //    MainPilotRet = colPilot[1];
            //    return MainPilotRet;
            //}

            //// 暴走時の特殊パイロット
            //if (IsConditionSatisfied("暴走"))
            //{
            //    if (IsFeatureAvailable("暴走時パイロット"))
            //    {
            //        pname = FeatureData("暴走時パイロット");
            //        if (SRC.PDList.IsDefined(pname))
            //        {
            //            PilotData localItem() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //            pname = localItem().Name;
            //        }
            //        else
            //        {
            //            GUI.ErrorMessage("暴走時パイロット「" + pname + "」のデータが定義されていません");
            //        }

            //        if (SRC.PList.IsDefined(pname))
            //        {
            //            // 既に暴走時パイロットが作成済み
            //            MainPilotRet = SRC.PList.Item(pname);
            //            MainPilotRet.Unit = this;
            //            MainPilotRet.Morale = Pilot(1).Morale;
            //            MainPilotRet.Level = Pilot(1).Level;
            //            MainPilotRet.Exp = Pilot(1).Exp;
            //            if (!without_update)
            //            {
            //                if (!erenceEquals(MainPilotRet.Unit, this))
            //                {
            //                    MainPilotRet.Unit = this;
            //                    MainPilotRet.Update();
            //                    MainPilotRet.UpdateSupportMod();
            //                }
            //            }

            //            return MainPilotRet;
            //        }
            //        else
            //        {
            //            // 暴走時パイロットが作成されていないので作成する
            //            MainPilotRet = SRC.PList.Add(pname, Pilot(1).Level, Party0, gid: "");
            //            this.Party0 = argpparty;
            //            MainPilotRet.Morale = Pilot(1).Morale;
            //            MainPilotRet.Exp = Pilot(1).Exp;
            //            MainPilotRet.Unit = this;
            //            MainPilotRet.Update();
            //            MainPilotRet.UpdateSupportMod();
            //            return MainPilotRet;
            //        }
            //    }
            //}

            //// 追加パイロットがいれば、それを使用
            //if (IsFeatureAvailable("追加パイロット"))
            //{
            //    pname = FeatureData("追加パイロット");
            //    if (SRC.PDList.IsDefined(pname))
            //    {
            //        PilotData localItem1() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //        pname = localItem1().Name;
            //    }
            //    else
            //    {
            //        GUI.ErrorMessage("追加パイロット「" + pname + "」のデータが定義されていません");
            //    }

            //    // 登録済みのパイロットをまずチェック
            //    if (pltAdditionalPilot is object)
            //    {
            //        if ((pltAdditionalPilot.Name ?? "") == (pname ?? ""))
            //        {
            //            MainPilotRet = pltAdditionalPilot;
            //            {
            //                var withBlock = pltAdditionalPilot;
            //                if (withBlock.IsAdditionalPilot && !erenceEquals(withBlock.Unit, this))
            //                {
            //                    withBlock.Unit = this;
            //                    withBlock.Party = Party0;
            //                    withBlock.Exp = Pilot(1).Exp;
            //                    if (withBlock.Personality != "機械")
            //                    {
            //                        withBlock.Morale = Pilot(1).Morale;
            //                    }

            //                    if (withBlock.Level != this.Pilot(1).Level)
            //                    {
            //                        withBlock.Level = Pilot(1).Level;
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
            //                    withBlock1.Unit = this;
            //                    if (withBlock1.IsAdditionalPilot && !erenceEquals(withBlock1.Unit, this))
            //                    {
            //                        withBlock1.Level = Pilot(1).Level;
            //                        withBlock1.Exp = Pilot(1).Exp;
            //                        if (withBlock1.Personality != "機械")
            //                        {
            //                            withBlock1.Morale = Pilot(1).Morale;
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
            //                pltAdditionalPilot = Pilot(i);
            //                MainPilotRet = pltAdditionalPilot;
            //                return MainPilotRet;
            //            }
            //        }
            //    }

            //    // 既に作成されていればそれを使う
            //    // (ただし複数作成可能なパイロットで、他のユニットの追加パイロットとして登録済みの場合は除く)
            //    if (SRC.PList.IsDefined(pname))
            //    {
            //        p = SRC.PList.Item(pname);
            //        if (!p.IsAdditionalPilot | Strings.InStr(pname, "(ザコ)") == 0 && Strings.InStr(pname, "(汎用)") == 0)
            //        {
            //            pltAdditionalPilot = p;
            //            {
            //                var withBlock2 = pltAdditionalPilot;
            //                withBlock2.IsAdditionalPilot = true;
            //                withBlock2.Party = Party0;
            //                withBlock2.Level = Pilot(1).Level;
            //                withBlock2.Exp = Pilot(1).Exp;
            //                if (withBlock2.Personality != "機械")
            //                {
            //                    withBlock2.Morale = Pilot(1).Morale;
            //                }

            //                if (!without_update)
            //                {
            //                    if (!erenceEquals(withBlock2.Unit, this))
            //                    {
            //                        withBlock2.Unit = this;
            //                        withBlock2.Update();
            //                        withBlock2.UpdateSupportMod();
            //                    }
            //                }
            //                else
            //                {
            //                    withBlock2.Unit = this;
            //                }
            //            }

            //            MainPilotRet = pltAdditionalPilot;
            //            return MainPilotRet;
            //        }
            //    }

            //    // まだ作成されていないので作成する
            //    if (CountPilot() > 0)
            //    {
            //        pltAdditionalPilot = SRC.PList.Add(pname, Pilot(1).Level, Party0, gid: "");
            //        this.Party0 = argpparty1;
            //        {
            //            var withBlock3 = pltAdditionalPilot;
            //            withBlock3.IsAdditionalPilot = true;
            //            withBlock3.Exp = Pilot(1).Exp;
            //            if (withBlock3.Personality != "機械")
            //            {
            //                withBlock3.Morale = Pilot(1).Morale;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        pltAdditionalPilot = SRC.PList.Add(pname, 1, Party0, gid: "");
            //        this.Party0 = argpparty2;
            //        pltAdditionalPilot.IsAdditionalPilot = true;
            //    }

            //    {
            //        var withBlock4 = pltAdditionalPilot;
            //        withBlock4.Unit = this;
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
        public IEnumerable<Pilot> Supports => colSupport.List;

        public IEnumerable<Pilot> MainPilots => Enumerable.Empty<Pilot>()
            .Append(MainPilot())
            .Concat(SubPilots);
        /// <summary>
        /// 全てのパイロット
        /// MainPilotを解決、追加サポート含む
        /// TODO 使用している個所でもともと追加サポートを処理していたか精査
        /// </summary>
        public IEnumerable<Pilot> AllPilots => Enumerable.Empty<Pilot>()
            .Append(MainPilot())
            .Concat(SubPilots)
            .Concat(Supports)
            .Append(AdditionalSupport())
            .Where(x => x != null);
        /// <summary>
        /// 全てのパイロット
        /// MainPilotを未解決、追加サポートは含まない
        /// </summary>
        public IEnumerable<Pilot> AllRawPilots => Pilots
                    .Concat(Supports);


        // サポートパイロットを追加
        public void AddSupport(Pilot p)
        {
            colSupport.Add(p, p.Name);
        }

        // サポートパイロットを削除
        public void DeleteSupport(Pilot p)
        {
            colSupport.Remove(p);
        }

        //// サポートパイロットの入れ替え
        //public void ReplaceSupport(Pilot p, string Index)
        //{
        //    int i;
        //    Pilot prev_p;
        //    Pilot[] support_list;
        //    p.Unit = this;
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
        //    prev_p.Unit = null;
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
            return null;
            // TODO Impl AdditionalSupport
            //    Pilot AdditionalSupportRet = default;
            //    string pname;
            //    Pilot p;
            //    int i;

            //    // 追加サポートパイロットの名称
            //    pname = FeatureData("追加サポート");

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
            //            pltAdditionalSupport.Unit = this;
            //            return AdditionalSupportRet;
            //        }
            //    }

            //    var loopTo = CountOtherForm();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        {
            //            var withBlock = OtherForm(i);
            //            if (withBlock.pltAdditionalSupport is object)
            //            {
            //                if ((withBlock.pltAdditionalSupport.Name ?? "") == (pname ?? ""))
            //                {
            //                    withBlock.pltAdditionalSupport.Unit = this;
            //                    AdditionalSupportRet = withBlock.pltAdditionalSupport;
            //                    return AdditionalSupportRet;
            //                }
            //            }
            //        }
            //    }

            //    // 既に作成されていればそれを使う
            //    // (ただし他のユニットの追加サポートとして登録済みの場合は除く)
            //    if (SRC.PList.IsDefined(pname))
            //    {
            //        p = SRC.PList.Item(pname);
            //        if (!p.IsAdditionalSupport | Strings.InStr(pname, "(ザコ)") == 0 && Strings.InStr(pname, "(汎用)") == 0)
            //        {
            //            pltAdditionalSupport = p;
            //            {
            //                var withBlock1 = pltAdditionalSupport;
            //                withBlock1.IsAdditionalSupport = true;
            //                withBlock1.Party = Party0;
            //                withBlock1.Unit = this;
            //                withBlock1.Level = Pilot(1).Level;
            //                withBlock1.Exp = Pilot(1).Exp;
            //                if (withBlock1.Personality != "機械")
            //                {
            //                    withBlock1.Morale = Pilot(1).Morale;
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
            //        GUI.ErrorMessage("追加サポート「" + pname + "」のデータが定義されていません");
            //        return AdditionalSupportRet;
            //    }

            //    pltAdditionalSupport = SRC.PList.Add(pname, Pilot(1).Level, Party0, gid: "");
            //    this.Party0 = argpparty;
            //    {
            //        var withBlock2 = pltAdditionalSupport;
            //        withBlock2.IsAdditionalSupport = true;
            //        withBlock2.Unit = this;
            //        withBlock2.Exp = Pilot(1).Exp;
            //        if (withBlock2.Personality != "機械")
            //        {
            //            withBlock2.Morale = Pilot(1).Morale;
            //        }
            //    }

            //    AdditionalSupportRet = pltAdditionalSupport;
            //    return AdditionalSupportRet;
        }

        // いずれかのパイロットが特殊能力 sname を持っているか判定
        public bool IsSkillAvailable(string sname)
        {
            bool IsSkillAvailableRet = default;
            int i;
            if (CountPilot() == 0)
            {
                return IsSkillAvailableRet;
            }

            // メインパイロット
            if (MainPilot().IsSkillAvailable(sname))
            {
                IsSkillAvailableRet = true;
                return IsSkillAvailableRet;
            }

            // パイロット数が負の場合はメインパイロットの能力のみが有効
            if (Data.PilotNum > 0)
            {
                foreach (var p in SubPilots)
                {
                    if (p.IsSkillAvailable(sname))
                    {
                        IsSkillAvailableRet = true;
                        return IsSkillAvailableRet;
                    }
                }
            }

            // サポート
            foreach (var p in Supports)
            {
                if (p.IsSkillAvailable(sname))
                {
                    IsSkillAvailableRet = true;
                    return IsSkillAvailableRet;
                }
            }

            // 追加サポート
            if (IsFeatureAvailable("追加サポート"))
            {
                if (AdditionalSupport().IsSkillAvailable(sname))
                {
                    IsSkillAvailableRet = true;
                    return IsSkillAvailableRet;
                }
            }

            IsSkillAvailableRet = false;
            return IsSkillAvailableRet;
        }

        // パイロット全員によるパイロット能力レベル
        public double SkillLevel(string sname, double default_slevel = 1d)
        {
            double SkillLevelRet = default;
            if (CountPilot() == 0)
            {
                return SkillLevelRet;
            }

            // エリアスが設定されてるかチェック
            if (SRC.ALDList.IsDefined(sname))
            {
                sname = SRC.ALDList.Item(sname).Elements.First().strAliasType;
            }

            switch (sname ?? "")
            {
                case "同調率":
                    {
                        SkillLevelRet = SyncLevel();
                        break;
                    }

                case "霊力":
                    {
                        SkillLevelRet = PlanaLevel();
                        break;
                    }

                case "オーラ":
                    {
                        SkillLevelRet = AuraLevel();
                        break;
                    }

                case "超能力":
                    {
                        SkillLevelRet = PsychicLevel();
                        break;
                    }

                case "Ｓ防御":
                case "切り払い":
                    {
                        SkillLevelRet = MainPilot().SkillLevel(sname, 1.ToString());
                        break;
                    }

                case "超感覚":
                    {
                        if (MaxSkillLevel("超感覚", 1d) > MaxSkillLevel("知覚強化", 1d))
                        {
                            SkillLevelRet = MaxSkillLevel("超感覚", 1d);
                        }
                        else
                        {
                            SkillLevelRet = MaxSkillLevel("知覚強化", 1d);
                        }

                        break;
                    }

                default:
                    {
                        SkillLevelRet = MaxSkillLevel(sname, default_slevel);
                        break;
                    }
            }

            return SkillLevelRet;
        }

        // パイロット中での最も高いパイロット能力レベルを返す
        private double MaxSkillLevel(string sname, double default_slevel)
        {
            double MaxSkillLevelRet = default;
            double slevel;
            int i;
            if (CountPilot() == 0)
            {
                return MaxSkillLevelRet;
            }

            // メインパイロット
            {
                var withBlock = MainPilot();
                if (withBlock.IsSkillLevelSpecified(sname))
                {
                    MaxSkillLevelRet = withBlock.SkillLevel(sname, ref_mode: "");
                }
                else if (withBlock.IsSkillAvailable(sname))
                {
                    MaxSkillLevelRet = default_slevel;
                }
                else
                {
                    MaxSkillLevelRet = 0d;
                }
            }

            // パイロット数が負の場合はメインパイロットの能力のみが有効
            if (Data.PilotNum > 0)
            {
                foreach (var p in SubPilots)
                {
                    if (p.IsSkillLevelSpecified(sname))
                    {
                        slevel = p.SkillLevel(sname, ref_mode: "");
                    }
                    else if (p.IsSkillAvailable(sname))
                    {
                        slevel = default_slevel;
                    }
                    else
                    {
                        slevel = 0d;
                    }

                    if (slevel > MaxSkillLevelRet)
                    {
                        MaxSkillLevelRet = slevel;
                    }
                }
            }

            // サポート
            foreach (var p in Supports)
            {
                if (p.IsSkillLevelSpecified(sname))
                {
                    slevel = p.SkillLevel(sname, ref_mode: "");
                }
                else if (p.IsSkillAvailable(sname))
                {
                    slevel = default_slevel;
                }
                else
                {
                    slevel = 0d;
                }

                if (slevel > MaxSkillLevelRet)
                {
                    MaxSkillLevelRet = slevel;
                }
            }

            // 追加サポート
            if (IsFeatureAvailable("追加サポート"))
            {
                var p = AdditionalSupport();
                if (p.IsSkillLevelSpecified(sname))
                {
                    slevel = p.SkillLevel(sname, ref_mode: "");
                }
                else if (p.IsSkillAvailable(sname))
                {
                    slevel = default_slevel;
                }
                else
                {
                    slevel = 0d;
                }

                if (slevel > MaxSkillLevelRet)
                {
                    MaxSkillLevelRet = slevel;
                }
            }

            return MaxSkillLevelRet;
        }

        // ユニットのオーラ力レベル
        public double AuraLevel(bool no_limit = false)
        {
            double AuraLevelRet = default;
            switch (CountPilot())
            {
                case 0:
                    {
                        return AuraLevelRet;
                    }

                case 1:
                    {
                        AuraLevelRet = MainPilot().SkillLevel("オーラ", ref_mode: "");
                        break;
                    }

                default:
                    {
                        // パイロットが２名以上の場合は２人目のオーラ力を加算
                        AuraLevelRet = MainPilot().SkillLevel("オーラ", ref_mode: "") + Pilots[1].SkillLevel("オーラ", ref_mode: "") / 2d;
                        break;
                    }
            }

            // サポートのオーラ力を加算
            if (IsFeatureAvailable("追加サポート"))
            {
                AuraLevelRet = AuraLevelRet + AdditionalSupport().SkillLevel("オーラ", ref_mode: "") / 2d;
            }
            else if (CountSupport() > 0)
            {
                AuraLevelRet = AuraLevelRet + Supports.First().SkillLevel("オーラ", ref_mode: "") / 2d;
            }

            // オーラ変換器レベルによる制限
            if (IsFeatureAvailable("オーラ変換器") && !no_limit)
            {
                if (IsFeatureLevelSpecified("オーラ変換器"))
                {
                    AuraLevelRet = GeneralLib.MinDbl(AuraLevelRet, FeatureLevel("オーラ変換器"));
                }
            }

            return AuraLevelRet;
        }

        // ユニットの超能力レベル
        public double PsychicLevel(bool no_limit = false)
        {
            double PsychicLevelRet = 0d;
            switch (CountPilot())
            {
                case 0:
                    {
                        return PsychicLevelRet;
                    }

                case 1:
                    {
                        PsychicLevelRet = MainPilot().SkillLevel("超能力", ref_mode: "");
                        break;
                    }

                default:
                    {
                        // パイロットが２名以上の場合は２人目の超能力を加算
                        PsychicLevelRet = MainPilot().SkillLevel("超能力", ref_mode: "") + Pilots[1].SkillLevel("超能力", ref_mode: "") / 2d;
                        break;
                    }
            }

            // サポートの超能力を加算
            if (IsFeatureAvailable("追加サポート"))
            {
                PsychicLevelRet = PsychicLevelRet + AdditionalSupport().SkillLevel("超能力", ref_mode: "") / 2d;
            }
            else if (CountSupport() > 0)
            {
                // サポートの超能力を加算
                PsychicLevelRet = PsychicLevelRet + Supports.First().SkillLevel("超能力", ref_mode: "") / 2d;
            }

            // サイキックドライブによる制限
            if (IsFeatureAvailable("サイキックドライブ") && !no_limit)
            {
                if (IsFeatureLevelSpecified("サイキックドライブ"))
                {
                    PsychicLevelRet = GeneralLib.MinDbl(PsychicLevelRet, FeatureLevel("サイキックドライブ"));
                }
            }

            return PsychicLevelRet;
        }

        // ユニットの同調率
        public double SyncLevel(bool no_limit = false)
        {
            double SyncLevelRet = default;
            if (CountPilot() == 0)
            {
                return SyncLevelRet;
            }

            SyncLevelRet = MainPilot().SynchroRate();

            // シンクロドライブレベルによる制限
            if (IsFeatureAvailable("シンクロドライブ") && !no_limit)
            {
                if (IsFeatureLevelSpecified("シンクロドライブ"))
                {
                    SyncLevelRet = GeneralLib.MinDbl(SyncLevelRet, FeatureLevel("シンクロドライブ"));
                }
            }

            return SyncLevelRet;
        }

        // ユニットの霊力レベル
        public double PlanaLevel(bool no_limit = false)
        {
            double PlanaLevelRet = default;
            if (CountPilot() == 0)
            {
                return PlanaLevelRet;
            }

            PlanaLevelRet = MainPilot().Plana;

            // 霊力変換器レベルによる制限
            if (IsFeatureAvailable("霊力変換器") && !no_limit)
            {
                if (IsFeatureLevelSpecified("霊力変換器"))
                {
                    PlanaLevelRet = Math.Min(PlanaLevelRet, FeatureLevel("霊力変換器"));
                }
            }

            return PlanaLevelRet;
        }

        //// パイロット全員からパイロット能力名を検索
        //public string SkillName0(string sname)
        //{
        //    string SkillName0Ret = default;
        //    int i;
        //    if (SRC.ALDList.IsDefined(sname))
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
        //    SkillName0Ret = MainPilot().SkillName0(sname);
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

        //            SkillName0Ret = localPilot().SkillName0(sname);
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

        //        SkillName0Ret = localSupport().SkillName0(sname);
        //        if ((SkillName0Ret ?? "") != (sname ?? ""))
        //        {
        //            return SkillName0Ret;
        //        }
        //    }

        //    // 追加サポート
        //    if (IsFeatureAvailable("追加サポート"))
        //    {
        //        SkillName0Ret = AdditionalSupport().SkillName0(sname);
        //    }

        //    return SkillName0Ret;
        //}

        public IList<Pilot> PilotsHaveSpecialPower()
        {
            var u = this;
            // スペシャルパワーを使用可能なパイロットの一覧を作成
            var pilots = new List<Pilot>();
            // メインパイロット＆サブパイロット
            // １番目のパイロットの場合はメインパイロットを使用
            // ただし２人乗り以上のユニットで、メインパイロットが
            // スペシャルパワーを持たない場合はそのまま１番目のパイロットを使用
            pilots.Add(u.CountPilot() > 1 && u.MainPilot().Data.SP <= 0 && u.Pilots.First().Data.SP > 0
                ? u.Pilots.First() : u.MainPilot());
            pilots.AddRange(u.SubPilots);
            // サポートパイロット
            pilots.AddRange(u.Supports.Skip(1));
            // 追加サポートパイロット
            if (u.IsFeatureAvailable("追加サポート"))
            {
                pilots.Add(u.AdditionalSupport());
            }
            return pilots.Where(x => x.CountSpecialPower > 0).ToList();
        }
    }
}
