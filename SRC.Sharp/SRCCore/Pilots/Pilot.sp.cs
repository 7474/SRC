// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Pilots
{
    // === スペシャルパワー関連処理 ===
    public partial class Pilot
    {
        // === ＳＰ値関連処理 ===

        // 最大ＳＰ
        public int MaxSP
        {
            get
            {
                int MaxSPRet = default;
                int lv;

                // Impl
                //// ＳＰなしの場合はレベルに関わらず0
                //if (Data.SP <= 0)
                //{
                //    MaxSPRet = 0;
                //    // ただし追加パイロットの場合は第１パイロットの最大ＳＰを使用
                //    if (Unit is object)
                //    {
                //        {
                //            var withBlock = Unit;
                //            if (withBlock.CountPilot() > 0)
                //            {
                //                object argIndex2 = 1;
                //                object argIndex3 = 1;
                //                if (!ReferenceEquals(withBlock.Pilot(argIndex3), this))
                //                {
                //                    if (ReferenceEquals(withBlock.MainPilot(), this))
                //                    {
                //                        object argIndex1 = 1;
                //                        MaxSPRet = withBlock.Pilot(argIndex1).MaxSP;
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    return default;
                //}

                // レベルによる上昇値
                lv = Level;
                if (lv > 99)
                {
                    lv = 100;
                }

                lv = (int)(lv + SkillLevel("追加レベル", ""));
                if (lv > 40)
                {
                    MaxSPRet = lv + 40;
                }
                else
                {
                    MaxSPRet = 2 * lv;
                }

                string argsname = "ＳＰ低成長";
                string argsname1 = "ＳＰ高成長";
                if (IsSkillAvailable(argsname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }
                else if (IsSkillAvailable(argsname1))
                {
                    MaxSPRet = (int)(1.5d * MaxSPRet);
                }

                string argoname = "ＳＰ低成長";
                if (Expression.IsOptionDefined(argoname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }

                // 基本値を追加
                MaxSPRet = MaxSPRet + Data.SP;

                // 能力ＵＰ
                MaxSPRet = (int)(MaxSPRet + SkillLevel("ＳＰＵＰ", ""));

                // 能力ＤＯＷＮ
                MaxSPRet = (int)(MaxSPRet - SkillLevel("ＳＰＤＯＷＮ", ""));

                // 上限を超えないように
                if (MaxSPRet > 9999)
                {
                    MaxSPRet = 9999;
                }

                return MaxSPRet;
            }
        }

        // ＳＰ値

        public int SP
        {
            get
            {
                int SPRet = default;
                SPRet = proSP;

                // Impl
                //// 追加パイロットかどうか判定

                //if (Unit is null)
                //{
                //    return default;
                //}

                //{
                //    var withBlock = Unit;
                //    if (withBlock.CountPilot() == 0)
                //    {
                //        return default;
                //    }

                //    object argIndex1 = 1;
                //    if (ReferenceEquals(withBlock.Pilot(argIndex1), this))
                //    {
                //        return default;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return default;
                //    }

                //    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                //    if (Data.SP > 0)
                //    {
                //        // ＳＰを持つ場合は消費量を一致させる
                //        object argIndex2 = 1;
                //        {
                //            var withBlock1 = withBlock.Pilot(argIndex2);
                //            if (withBlock1.MaxSP > 0)
                //            {
                //                proSP = (MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                //                SPRet = proSP;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // ＳＰを持たない場合はそのまま使う
                //        object argIndex3 = 1;
                //        SPRet = withBlock.Pilot(argIndex3).SP0;
                //    }
                //}

                return SPRet;
            }

            set
            {
                int prev_sp;
                prev_sp = proSP;
                if (value > MaxSP)
                {
                    proSP = MaxSP;
                }
                else if (value < 0)
                {
                    proSP = 0;
                }
                else
                {
                    proSP = value;
                }

                // Impl
                //// 追加パイロットかどうか判定

                //if (Unit is null)
                //{
                //    return;
                //}

                //{
                //    var withBlock = Unit;
                //    if (withBlock.CountPilot() == 0)
                //    {
                //        return;
                //    }

                //    object argIndex1 = 1;
                //    if (ReferenceEquals(withBlock.Pilot(argIndex1), this))
                //    {
                //        return;
                //    }

                //    if (!ReferenceEquals(withBlock.MainPilot(), this))
                //    {
                //        return;
                //    }

                //    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                //    object argIndex2 = 1;
                //    {
                //        var withBlock1 = withBlock.Pilot(argIndex2);
                //        if (Data.SP > 0)
                //        {
                //            // 追加パイロットがＳＰを持つ場合は第１パイロットと消費率を一致させる
                //            if (withBlock1.MaxSP > 0)
                //            {
                //                withBlock1.SP0 = withBlock1.MaxSP * proSP / MaxSP;
                //                proSP = (MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                //            }
                //        }
                //        // 追加パイロットがＳＰを持たない場合は第１パイロットのＳＰ値をそのまま使う
                //        else if (value > withBlock1.MaxSP)
                //        {
                //            withBlock1.SP0 = withBlock1.MaxSP;
                //        }
                //        else if (value < 0)
                //        {
                //            withBlock1.SP0 = 0;
                //        }
                //        else
                //        {
                //            withBlock1.SP0 = value;
                //        }
                //    }
                //}
            }
        }

        public int SP0
        {
            get
            {
                int SP0Ret = default;
                SP0Ret = proSP;
                return SP0Ret;
            }

            set
            {
                proSP = value;
            }
        }

        // ＳＰ関連処理で適用されるパイロット
        // ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
        private Pilot ForSP()
        {
            if (Data.SP <= 0)
            {
                if (IsActiveAdditionalPilot())
                {
                    return Unit.Pilots.First();
                }
            }
            return this;
        }

        public bool IsActiveAdditionalPilot()
        {
            if (Unit is object)
            {
                if (Unit.CountPilot() > 0)
                {
                    if (!ReferenceEquals(Unit.Pilots.First(), this))
                    {
                        if (ReferenceEquals(Unit.MainPilot(), this))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // スペシャルパワーの個数
        public int CountSpecialPower
        {
            get
            {
                return ForSP().Data.CountSpecialPower(Level);
            }
        }
        public IList<string> SpecialPowerNames
        {
            get
            {
                return Enumerable.Range(1, ForSP().Data.CountSpecialPower(Level))
                    .Select(i => get_SpecialPower(i))
                    .ToList();
            }
        }

        // idx番目のスペシャルパワー
        public string get_SpecialPower(int idx)
        {
            return ForSP().Data.SpecialPower(Level, idx);
        }

        // スペシャルパワー sname を修得しているか？
        public bool IsSpecialPowerAvailable(string sname)
        {
            return ForSP().Data.IsSpecialPowerAvailable(Level, sname);
        }

        // スペシャルパワー sname が有用か？
        public bool IsSpecialPowerUseful(string sname)
        {
            return SRC.SPDList.Item(sname).Useful(this);
        }

        // スペシャルパワー sname に必要なＳＰ値
        public int SpecialPowerCost(string sname)
        {
            return ForSP().SpecialPowerCostInternal(sname);
        }

        private int SpecialPowerCostInternal(string sname)
        {
            // 基本消費ＳＰ値
            var SpecialPowerCostRet = Data.SpecialPowerCost(sname);

            // 特殊能力による消費ＳＰ値修正
            if (IsSkillAvailable("超能力") || IsSkillAvailable("集中力"))
            {
                SpecialPowerCostRet = (int)(0.8d * SpecialPowerCostRet);
            }

            if (IsSkillAvailable("知覚強化"))
            {
                SpecialPowerCostRet = (int)(1.2d * SpecialPowerCostRet);
            }

            // ＳＰ消費減少能力
            if (Unit is object)
            {
                if (Unit.CountPilot() > 0)
                {
                    if (ReferenceEquals(Unit.MainPilot(), this))
                    {
                        if (Unit.IsConditionSatisfied("ＳＰ消費減少付加")
                            || Unit.IsConditionSatisfied("ＳＰ消費減少付加２"))
                        {
                            var adata = SkillData("ＳＰ消費減少");
                            var loopTo = GeneralLib.LLength(adata);
                            for (var i = 2; i <= loopTo; i++)
                            {
                                if ((sname ?? "") == (GeneralLib.LIndex(adata, i) ?? ""))
                                {
                                    return (int)((10d - SkillLevel("ＳＰ消費減少", "")) * SpecialPowerCostRet / 10L);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var skill in colSkill.List.Where(x => x.Name == "ＳＰ消費減少"))
            {
                var adata = skill.StrData;
                var loopTo2 = GeneralLib.LLength(adata);
                for (var j = 2; j <= loopTo2; j++)
                {
                    if ((sname ?? "") == (GeneralLib.LIndex(adata, j) ?? ""))
                    {
                        return (int)((10d - skill.LevelOrDefault(1d)) * SpecialPowerCostRet / 10L);
                    }
                }
            }

            return SpecialPowerCostRet;
        }

        // スペシャルパワー sname を実行する
        public void UseSpecialPower(string sname, double sp_mod = 1d)
        {
            Unit my_unit;

            if (!SRC.SPDList.IsDefined(sname))
            {
                return;
            }

            SRC.GUIStatus.ClearUnitStatus();
            Commands.SelectedPilot = this;

            // TODO Impl
            //// スペシャルパワー使用メッセージ
            //SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //SpecialPowerData localItem1() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //string argename = "復活";
            //string argename1 = "自爆";
            //if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(sp_mod != 2d, !localItem().IsEffectAvailable(argename)), !localItem1().IsEffectAvailable(argename1))))
            //{
            //    if (Unit.IsMessageDefined(sname))
            //    {
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //        string argmsg_mode = "";
            //        Unit.PilotMessage(sname, msg_mode: argmsg_mode);
            //        GUI.CloseMessageForm();
            //    }
            //}

            // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            // 変化してしまうことがあるため、元のUnitを記録しておく
            my_unit = Unit;

            //// スペシャルパワーアニメを表示
            //SpecialPowerData localItem2() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //if (!localItem2().PlayAnimation())
            //{
            //    // メッセージ表示のみ
            //    Unit argu21 = null;
            //    GUI.OpenMessageForm(Unit, u2: argu21);
            //    GUI.DisplaySysMessage(get_Nickname(false) + "は" + sname + "を使った。");
            //}

            //// Unitが変化した場合に元に戻す
            //if (!ReferenceEquals(my_unit, Unit))
            //{
            //    my_unit.MainPilot();
            //}

            // スペシャルパワーを実行
            SRC.SPDList.Item(sname).Execute(SRC, this);

            // Unitが変化した場合に元に戻す
            if (!ReferenceEquals(my_unit, Unit))
            {
                my_unit.CurrentForm().MainPilot();
            }

            SP = (int)(SP - sp_mod * SpecialPowerCost(sname));
            GUI.CloseMessageForm();
        }
    }
}
