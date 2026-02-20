using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.VB;
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

        // パイロットの入れ替え
        public void ReplacePilot(Pilot orgPilot, Pilot newPilot)
        {
            var newList = new List<Pilot>();

            foreach (var p0 in Pilots)
            {
                if (p0.ID == orgPilot.ID)
                {
                    newList.Add(newPilot);
                    orgPilot.Unit = null;
                    orgPilot.Alive = false;
                }
                else
                {
                    newList.Add(p0);
                }
            }
            colPilot.Clear();
            foreach (var p0 in newList)
            {
                AddPilot(p0);
            }
        }

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
            string pname;
            Pilot p;

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

            // 能力コピー中は同じパイロットが複数のユニットのメインパイロットに使用されるのを防ぐため
            // 追加パイロットと暴走時パイロットを使用しない
            if (IsConditionSatisfied("能力コピー"))
            {
                return colPilot[1];
            }

            // 暴走時の特殊パイロット
            if (IsConditionSatisfied("暴走"))
            {
                if (IsFeatureAvailable("暴走時パイロット"))
                {
                    pname = FeatureData("暴走時パイロット");
                    if (SRC.PDList.IsDefined(pname))
                    {
                        pname = SRC.PDList.Item(pname).Name;
                    }
                    else
                    {
                        GUI.ErrorMessage("暴走時パイロット「" + pname + "」のデータが定義されていません");
                    }

                    if (SRC.PList.IsDefined(pname))
                    {
                        // 既に暴走時パイロットが作成済み
                        var berserkPilot = SRC.PList.Item(pname);
                        berserkPilot.Unit = this;
                        berserkPilot.Morale = Pilots.First().Morale;
                        berserkPilot.Level = Pilots.First().Level;
                        berserkPilot.Exp = Pilots.First().Exp;
                        if (!without_update)
                        {
                            if (!ReferenceEquals(berserkPilot.Unit, this))
                            {
                                berserkPilot.Unit = this;
                                berserkPilot.Update();
                                berserkPilot.UpdateSupportMod();
                            }
                        }

                        return berserkPilot;
                    }
                    else
                    {
                        // 暴走時パイロットが作成されていないので作成する
                        var berserkPilot = SRC.PList.Add(pname, Pilots.First().Level, Party0, gid: "");
                        berserkPilot.Morale = Pilots.First().Morale;
                        berserkPilot.Exp = Pilots.First().Exp;
                        berserkPilot.Unit = this;
                        berserkPilot.Update();
                        berserkPilot.UpdateSupportMod();
                        return berserkPilot;
                    }
                }
            }

            // 追加パイロットがいれば、それを使用
            if (IsFeatureAvailable("追加パイロット"))
            {
                pname = FeatureData("追加パイロット");
                if (SRC.PDList.IsDefined(pname))
                {
                    pname = SRC.PDList.Item(pname).Name;
                }
                else
                {
                    GUI.ErrorMessage("追加パイロット「" + pname + "」のデータが定義されていません");
                }

                // 登録済みのパイロットをまずチェック
                if (pltAdditionalPilot is object)
                {
                    if ((pltAdditionalPilot.Name ?? "") == (pname ?? ""))
                    {
                        if (pltAdditionalPilot.IsAdditionalPilot && !ReferenceEquals(pltAdditionalPilot.Unit, this))
                        {
                            pltAdditionalPilot.Unit = this;
                            pltAdditionalPilot.Party = Party0;
                            pltAdditionalPilot.Exp = Pilots.First().Exp;
                            if (pltAdditionalPilot.Personality != "機械")
                            {
                                pltAdditionalPilot.Morale = Pilots.First().Morale;
                            }

                            if (pltAdditionalPilot.Level != Pilots.First().Level)
                            {
                                pltAdditionalPilot.Level = Pilots.First().Level;
                                pltAdditionalPilot.Update();
                            }
                        }

                        return pltAdditionalPilot;
                    }
                }

                // 他形態に登録されているパイロットをチェック
                foreach (var otherForm in OtherForms)
                {
                    if (otherForm.pltAdditionalPilot is object)
                    {
                        var additionalPilot = otherForm.pltAdditionalPilot;
                        if ((additionalPilot.Name ?? "") == (pname ?? ""))
                        {
                            pltAdditionalPilot = additionalPilot;
                            additionalPilot.Party = Party0;
                            additionalPilot.Unit = this;
                            if (additionalPilot.IsAdditionalPilot && !ReferenceEquals(additionalPilot.Unit, this))
                            {
                                additionalPilot.Level = Pilots.First().Level;
                                additionalPilot.Exp = Pilots.First().Exp;
                                if (additionalPilot.Personality != "機械")
                                {
                                    additionalPilot.Morale = Pilots.First().Morale;
                                }

                                additionalPilot.Update();
                                additionalPilot.UpdateSupportMod();
                            }

                            return pltAdditionalPilot;
                        }
                    }
                }

                // 次に搭乗しているパイロットから検索
                if (CountPilot() > 0)
                {
                    // 単なるメインパイロットの交代として扱うため、IsAdditionalPilotのフラグは立てない
                    foreach (var pilot in Pilots)
                    {
                        if ((pilot.Name ?? "") == (pname ?? ""))
                        {
                            pltAdditionalPilot = pilot;
                            return pltAdditionalPilot;
                        }
                    }
                }

                // 既に作成されていればそれを使う
                // (ただし複数作成可能なパイロットで、他のユニットの追加パイロットとして登録済みの場合は除く)
                if (SRC.PList.IsDefined(pname))
                {
                    p = SRC.PList.Item(pname);
                    if (!p.IsAdditionalPilot || Strings.InStr(pname, "(ザコ)") == 0 && Strings.InStr(pname, "(汎用)") == 0)
                    {
                        pltAdditionalPilot = p;
                        pltAdditionalPilot.IsAdditionalPilot = true;
                        pltAdditionalPilot.Party = Party0;
                        if (CountPilot() > 0)
                        {
                            pltAdditionalPilot.Level = Pilots.First().Level;
                            pltAdditionalPilot.Exp = Pilots.First().Exp;
                            if (pltAdditionalPilot.Personality != "機械")
                            {
                                pltAdditionalPilot.Morale = Pilots.First().Morale;
                            }
                        }

                        if (!without_update)
                        {
                            if (!ReferenceEquals(pltAdditionalPilot.Unit, this))
                            {
                                pltAdditionalPilot.Unit = this;
                                pltAdditionalPilot.Update();
                                pltAdditionalPilot.UpdateSupportMod();
                            }
                        }
                        else
                        {
                            pltAdditionalPilot.Unit = this;
                        }

                        return pltAdditionalPilot;
                    }
                }

                // まだ作成されていないので作成する
                if (CountPilot() > 0)
                {
                    pltAdditionalPilot = SRC.PList.Add(pname, Pilots.First().Level, Party0, gid: "");
                    pltAdditionalPilot.IsAdditionalPilot = true;
                    pltAdditionalPilot.Exp = Pilots.First().Exp;
                    if (pltAdditionalPilot.Personality != "機械")
                    {
                        pltAdditionalPilot.Morale = Pilots.First().Morale;
                    }
                }
                else
                {
                    pltAdditionalPilot = SRC.PList.Add(pname, 1, Party0, gid: "");
                    pltAdditionalPilot.IsAdditionalPilot = true;
                }

                pltAdditionalPilot.Unit = this;
                if (!without_update)
                {
                    pltAdditionalPilot.Update();
                    pltAdditionalPilot.UpdateSupportMod();
                }

                return pltAdditionalPilot;
            }

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

        // サポートパイロットの入れ替え
        public void ReplaceSupport(Pilot orgPilot, Pilot newPilot)
        {
            var newList = new List<Pilot>();

            foreach (var p0 in Supports)
            {
                if (p0.ID == orgPilot.ID)
                {
                    newList.Add(newPilot);
                    orgPilot.Unit = null;
                    orgPilot.Alive = false;
                }
                else
                {
                    newList.Add(p0);
                }
            }
            colSupport.Clear();
            foreach (var p0 in newList)
            {
                AddSupport(p0);
            }
        }

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
            // 追加サポートパイロットの名称
            string pname = FeatureData("追加サポート");

            // 追加サポートが存在しない？
            if (string.IsNullOrEmpty(pname))
            {
                return null;
            }

            // 他にパイロットが乗っていない場合は無効
            if (CountPilot() == 0)
            {
                return null;
            }

            // 既に登録済みであるかチェック
            if (pltAdditionalSupport is object)
            {
                if ((pltAdditionalSupport.Name ?? "") == (pname ?? ""))
                {
                    pltAdditionalSupport.Unit = this;
                    return pltAdditionalSupport;
                }
            }

            foreach (var of in OtherForms)
            {
                if (of.pltAdditionalSupport is object)
                {
                    if ((of.pltAdditionalSupport.Name ?? "") == (pname ?? ""))
                    {
                        of.pltAdditionalSupport.Unit = this;
                        pltAdditionalSupport = of.pltAdditionalSupport;
                        return pltAdditionalSupport;
                    }
                }
            }

            // 既に作成されていればそれを使う
            // (ただし他のユニットの追加サポートとして登録済みの場合は除く)
            if (SRC.PList.IsDefined(pname))
            {
                var p = SRC.PList.Item(pname);
                if (!p.IsAdditionalSupport || (Strings.InStr(pname, "(ザコ)") == 0 && Strings.InStr(pname, "(汎用)") == 0))
                {
                    pltAdditionalSupport = p;
                    pltAdditionalSupport.IsAdditionalSupport = true;
                    pltAdditionalSupport.Party = Party0;
                    pltAdditionalSupport.Unit = this;
                    pltAdditionalSupport.Level = Pilots[0].Level;
                    pltAdditionalSupport.Exp = Pilots[0].Exp;
                    if (pltAdditionalSupport.Personality != "機械")
                    {
                        pltAdditionalSupport.Morale = Pilots[0].Morale;
                    }

                    return pltAdditionalSupport;
                }
            }

            // まだ作成されていないので作成する
            if (!SRC.PDList.IsDefined(pname))
            {
                GUI.ErrorMessage("追加サポート「" + pname + "」のデータが定義されていません");
                return null;
            }

            pltAdditionalSupport = SRC.PList.Add(pname, Pilots[0].Level, Party0, gid: "");
            pltAdditionalSupport.IsAdditionalSupport = true;
            pltAdditionalSupport.Unit = this;
            pltAdditionalSupport.Exp = Pilots[0].Exp;
            if (pltAdditionalSupport.Personality != "機械")
            {
                pltAdditionalSupport.Morale = Pilots[0].Morale;
            }

            return pltAdditionalSupport;
        }
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

        // パイロット全員からパイロット能力名を検索
        public string SkillName0(string sname)
        {
            string SkillName0Ret = default;
            int i;
            if (SRC.ALDList.IsDefined(sname))
            {
                sname = SRC.ALDList.Item(sname).Elements.First().strAliasType;
            }

            if (CountPilot() == 0)
            {
                SkillName0Ret = sname;
                return SkillName0Ret;
            }

            // メインパイロット
            SkillName0Ret = MainPilot().SkillName0(sname);
            if ((SkillName0Ret ?? "") != (sname ?? ""))
            {
                return SkillName0Ret;
            }

            // パイロット数が負の場合はメインパイロットの能力のみが有効
            if (Data.PilotNum > 0)
            {
                foreach (var p in SubPilots)
                {
                    SkillName0Ret = p.SkillName0(sname);
                    if ((SkillName0Ret ?? "") != (sname ?? ""))
                    {
                        return SkillName0Ret;
                    }
                }
            }

            // サポート
            foreach (var p in Supports)
            {
                SkillName0Ret = p.SkillName0(sname);
                if ((SkillName0Ret ?? "") != (sname ?? ""))
                {
                    return SkillName0Ret;
                }
            }

            // 追加サポート
            if (IsFeatureAvailable("追加サポート"))
            {
                SkillName0Ret = AdditionalSupport().SkillName0(sname);
            }

            return SkillName0Ret;
        }

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
