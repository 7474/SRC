// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // 経験値を入手
        // t:ターゲット
        // exp_situation:経験値入手の理由
        // exp_mode:マップ攻撃による入手？
        public int GetExp(ref Unit t, ref string exp_situation, [Optional, DefaultParameterValue("")] ref string exp_mode)
        {
            int GetExpRet = default;
            var xp = default(int);
            short j, i, n;
            short prev_level;
            string[] prev_stype;
            string[] prev_sname;
            double[] prev_slevel;
            string[] prev_special_power;
            string stype, sname;
            Pilot p;
            string msg;

            // 経験値を入手するのは味方ユニット及びＮＰＣの召喚ユニットのみ
            string argfname = "召喚ユニット";
            if ((Party != "味方" | Party0 != "味方") & (Party != "ＮＰＣ" | Party0 != "ＮＰＣ" | !IsFeatureAvailable(ref argfname)))
            {
                return GetExpRet;
            }

            // メインパイロットの現在の能力を記録
            {
                var withBlock = MainPilot();
                prev_level = withBlock.Level;
                prev_special_power = new string[(withBlock.CountSpecialPower + 1)];
                var loopTo = withBlock.CountSpecialPower;
                for (i = 1; i <= loopTo; i++)
                    prev_special_power[i] = withBlock.get_SpecialPower(i);
                prev_stype = new string[(withBlock.CountSkill() + 1)];
                prev_sname = new string[(withBlock.CountSkill() + 1)];
                prev_slevel = new double[(withBlock.CountSkill() + 1)];
                var loopTo1 = withBlock.CountSkill();
                for (i = 1; i <= loopTo1; i++)
                {
                    object argIndex1 = i;
                    prev_stype[i] = withBlock.Skill(ref argIndex1);
                    object argIndex2 = i;
                    prev_sname[i] = withBlock.SkillName(ref argIndex2);
                    object argIndex3 = i;
                    string argref_mode = "基本値";
                    prev_slevel[i] = withBlock.SkillLevel(ref argIndex3, ref argref_mode);
                }
            }

            // ターゲットが指定されていない場合は自分がターゲット
            if (t is null)
            {
                t = this;
            }

            // ターゲットにパイロットが乗っていない場合は経験値なし
            if (t.CountPilot() == 0)
            {
                return GetExpRet;
            }

            // ユニットに乗っているパイロット総数を計算
            n = (short)(CountPilot() + CountSupport());
            string argfname1 = "追加サポート";
            if (IsFeatureAvailable(ref argfname1))
            {
                n = (short)(n + 1);
            }

            // 各パイロットが経験値を入手
            var loopTo2 = n;
            for (i = 1; i <= loopTo2; i++)
            {
                if (i <= CountPilot())
                {
                    object argIndex4 = i;
                    p = Pilot(ref argIndex4);
                }
                else if (i <= (short)(CountPilot() + CountSupport()))
                {
                    object argIndex5 = i - CountPilot();
                    p = Support(ref argIndex5);
                }
                else
                {
                    p = AdditionalSupport();
                }

                switch (exp_situation ?? "")
                {
                    case "破壊":
                        {
                            xp = t.ExpValue + t.MainPilot().ExpValue;
                            string argsptype = "獲得経験値増加";
                            if (IsUnderSpecialPowerEffect(ref argsptype) & exp_mode != "パートナー")
                            {
                                string argsname = "獲得経験値増加";
                                xp = (int)(xp * (1d + 0.1d * SpecialPowerEffectLevel(ref argsname)));
                            }

                            break;
                        }

                    case "攻撃":
                        {
                            xp = (t.ExpValue + t.MainPilot().ExpValue) / 10;
                            string argsptype1 = "獲得経験値増加";
                            if (IsUnderSpecialPowerEffect(ref argsptype1) & exp_mode != "パートナー")
                            {
                                string argsname1 = "獲得経験値増加";
                                xp = (int)(xp * (1d + 0.1d * SpecialPowerEffectLevel(ref argsname1)));
                            }

                            break;
                        }

                    case "アビリティ":
                        {
                            if (ReferenceEquals(t, this))
                            {
                                xp = 50;
                            }
                            else
                            {
                                xp = 100;
                            }

                            break;
                        }

                    case "修理":
                        {
                            xp = 100;
                            break;
                        }

                    case "補給":
                        {
                            xp = 150;
                            break;
                        }
                }

                string argsptype2 = "獲得経験値増加";
                string argoname = "収得効果重複";
                if (!IsUnderSpecialPowerEffect(ref argsptype2) | Expression.IsOptionDefined(ref argoname))
                {
                    string argsname2 = "素質";
                    if (p.IsSkillAvailable(ref argsname2))
                    {
                        object argIndex7 = "素質";
                        if (p.IsSkillLevelSpecified(ref argIndex7))
                        {
                            object argIndex6 = "素質";
                            string argref_mode1 = "";
                            xp = (int)((long)(xp * (10d + p.SkillLevel(ref argIndex6, ref_mode: ref argref_mode1))) / 10L);
                        }
                        else
                        {
                            xp = (int)(1.5d * xp);
                        }
                    }
                }

                string argsname3 = "遅成長";
                if (p.IsSkillAvailable(ref argsname3))
                {
                    xp = xp / 2;
                }

                // 対象のパイロットのレベル差による修正
                switch ((short)(t.MainPilot().Level - p.Level))
                {
                    case var @case when @case > 7:
                        {
                            xp = 5 * xp;
                            break;
                        }

                    case 7:
                        {
                            xp = (int)(4.5d * xp);
                            break;
                        }

                    case 6:
                        {
                            xp = 4 * xp;
                            break;
                        }

                    case 5:
                        {
                            xp = (int)(3.5d * xp);
                            break;
                        }

                    case 4:
                        {
                            xp = 3 * xp;
                            break;
                        }

                    case 3:
                        {
                            xp = (int)(2.5d * xp);
                            break;
                        }

                    case 2:
                        {
                            xp = 2 * xp;
                            break;
                        }

                    case 1:
                        {
                            xp = (int)(1.5d * xp);
                            break;
                        }

                    case 0:
                        {
                            break;
                        }

                    case -1:
                        {
                            xp = xp / 2;
                            break;
                        }

                    case -2:
                        {
                            xp = xp / 4;
                            break;
                        }

                    case -3:
                        {
                            xp = xp / 6;
                            break;
                        }

                    case -4:
                        {
                            xp = xp / 8;
                            break;
                        }

                    case -5:
                        {
                            xp = xp / 10;
                            break;
                        }

                    case var case1 when case1 < -5:
                        {
                            xp = xp / 12;
                            break;
                        }
                }

                p.Exp = p.Exp + xp;

                // 一番目のパイロットが獲得した経験値を返す
                if (i == 1)
                {
                    GetExpRet = xp;
                }
            }

            // 追加パイロットの場合、一番目のパイロットにレベル、経験値を合わせる
            object argIndex10 = 1;
            object argIndex11 = 1;
            if (!ReferenceEquals(MainPilot(), Pilot(ref argIndex11)))
            {
                object argIndex8 = 1;
                MainPilot().Level = Pilot(ref argIndex8).Level;
                object argIndex9 = 1;
                MainPilot().Exp = Pilot(ref argIndex9).Exp;
            }

            // 召喚主も経験値を入手
            if (Summoner is object)
            {
                string argexp_mode = "パートナー";
                Summoner.CurrentForm().GetExp(ref t, ref exp_situation, ref argexp_mode);
            }

            // マップ攻撃による経験値収得の場合はメッセージ表示を省略
            if (exp_mode == "マップ")
            {
                return GetExpRet;
            }

            // 経験値入手時のメッセージ
            {
                var withBlock1 = MainPilot();
                if (withBlock1.Level > prev_level)
                {
                    // レベルアップ

                    string argmain_situation2 = "レベルアップ";
                    string argsub_situation2 = "";
                    string argmain_situation3 = "レベルアップ";
                    string argsub_situation3 = "";
                    if (IsAnimationDefined(ref argmain_situation2, sub_situation: ref argsub_situation2))
                    {
                        string argmain_situation = "レベルアップ";
                        string argsub_situation = "";
                        PlayAnimation(ref argmain_situation, sub_situation: ref argsub_situation);
                    }
                    else if (IsSpecialEffectDefined(ref argmain_situation3, sub_situation: ref argsub_situation3))
                    {
                        string argmain_situation1 = "レベルアップ";
                        string argsub_situation1 = "";
                        SpecialEffect(ref argmain_situation1, sub_situation: ref argsub_situation1);
                    }

                    string argmain_situation4 = "レベルアップ";
                    if (IsMessageDefined(ref argmain_situation4))
                    {
                        string argSituation = "レベルアップ";
                        string argmsg_mode = "";
                        PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }

                    msg = withBlock1.get_Nickname(false) + "は経験値[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GetExpRet) + "]を獲得、" + "レベル[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.Level) + "]にレベルアップ。";

                    // 特殊能力の習得
                    var loopTo3 = withBlock1.CountSkill();
                    for (i = 1; i <= loopTo3; i++)
                    {
                        object argIndex12 = i;
                        stype = withBlock1.Skill(ref argIndex12);
                        object argIndex13 = i;
                        sname = withBlock1.SkillName(ref argIndex13);
                        if (Strings.InStr(sname, "非表示") == 0)
                        {
                            switch (stype ?? "")
                            {
                                case "同調率":
                                case "霊力":
                                case "追加レベル":
                                case "魔力所有":
                                    {
                                        break;
                                    }

                                case "ＳＰ消費減少":
                                case "スペシャルパワー自動発動":
                                case "ハンター":
                                    {
                                        var loopTo4 = (short)Information.UBound(prev_stype);
                                        for (j = 1; j <= loopTo4; j++)
                                        {
                                            if ((stype ?? "") == (prev_stype[j] ?? ""))
                                            {
                                                if ((sname ?? "") == (prev_sname[j] ?? ""))
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (j > Information.UBound(prev_stype))
                                        {
                                            msg = msg + ";" + sname + "を習得した。";
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        var loopTo5 = (short)Information.UBound(prev_stype);
                                        for (j = 1; j <= loopTo5; j++)
                                        {
                                            if ((stype ?? "") == (prev_stype[j] ?? ""))
                                            {
                                                break;
                                            }
                                        }

                                        double localSkillLevel() { object argIndex1 = i; string argref_mode = "基本値"; var ret = withBlock1.SkillLevel(ref argIndex1, ref argref_mode); return ret; }

                                        if (j > Information.UBound(prev_stype))
                                        {
                                            msg = msg + ";" + sname + "を習得した。";
                                        }
                                        else if (localSkillLevel() > prev_slevel[j])
                                        {
                                            msg = msg + ";" + prev_sname[j] + " => " + sname + "。";
                                        }

                                        break;
                                    }
                            }
                        }
                    }

                    // スペシャルパワーの習得
                    if (withBlock1.CountSpecialPower > Information.UBound(prev_special_power))
                    {
                        string argtname = "スペシャルパワー";
                        var argu = this;
                        msg = msg + ";" + Expression.Term(ref argtname, ref argu);
                        var loopTo6 = withBlock1.CountSpecialPower;
                        for (i = 1; i <= loopTo6; i++)
                        {
                            sname = withBlock1.get_SpecialPower(i);
                            var loopTo7 = (short)Information.UBound(prev_special_power);
                            for (j = 1; j <= loopTo7; j++)
                            {
                                if ((sname ?? "") == (prev_special_power[j] ?? ""))
                                {
                                    break;
                                }
                            }

                            if (j > Information.UBound(prev_special_power))
                            {
                                msg = msg + "「" + sname + "」";
                            }
                        }

                        msg = msg + "を習得した。";
                    }

                    GUI.DisplaySysMessage(msg);
                    if (GUI.MessageWait < 10000)
                    {
                        GUI.Sleep(GUI.MessageWait);
                    }

                    Event_Renamed.HandleEvent("レベルアップ", withBlock1.ID);
                    SRC.PList.UpdateSupportMod(this);
                }
                else if (GetExpRet > 0)
                {
                    GUI.DisplaySysMessage(withBlock1.get_Nickname(false) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GetExpRet) + "の経験値を得た。");
                }
            }

            return GetExpRet;
        }

        // ユニットの陣営を変更
        public void ChangeParty(ref string new_party)
        {
            short i;

            // 陣営を変更
            Party = new_party;

            // ビットマップを作り直す
            var argu = this;
            BitmapID = GUI.MakeUnitBitmap(ref argu);

            // パイロットの陣営を変更
            var loopTo = CountPilot();
            for (i = 1; i <= loopTo; i++)
            {
                Pilot localPilot() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

                localPilot().Party = new_party;
            }

            var loopTo1 = CountSupport();
            for (i = 1; i <= loopTo1; i++)
            {
                Pilot localSupport() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

                localSupport().Party = new_party;
            }

            string argfname = "追加サポート";
            if (IsFeatureAvailable(ref argfname))
            {
                AdditionalSupport().Party = new_party;
            }

            // 他形態の陣営を変更
            var loopTo2 = CountOtherForm();
            for (i = 1; i <= loopTo2; i++)
            {
                Unit localOtherForm() { object argIndex1 = i; var ret = OtherForm(ref argIndex1); return ret; }

                localOtherForm().Party = new_party;
                Unit localOtherForm1() { object argIndex1 = i; var ret = OtherForm(ref argIndex1); return ret; }

                localOtherForm1().BitmapID = 0;
            }

            // 出撃中？
            if (Status_Renamed == "出撃")
            {
                // 自分の陣営のステージなら行動可能に
                if ((Party ?? "") == (SRC.Stage ?? ""))
                {
                    Rest();
                }
                // マップ上のユニット画像を更新
                var argu1 = this;
                GUI.PaintUnitBitmap(ref argu1);
            }

            SRC.PList.UpdateSupportMod(this);

            // 思考モードを通常に
            Mode = "通常";
        }

        // ユニットに乗っているパイロットの気力をnumだけ増減
        // is_event:イベントによる気力増減(性格を無視して気力操作)
        public void IncreaseMorale(short num, bool is_event = false)
        {
            Pilot p;
            if (CountPilot() == 0)
            {
                return;
            }

            // メインパイロット
            {
                var withBlock = MainPilot();
                if (withBlock.Personality != "機械" | is_event)
                {
                    withBlock.Morale = (short)(withBlock.Morale + num);
                }
            }

            // サブパイロット
            foreach (Pilot currentP in colPilot)
            {
                p = currentP;
                if ((MainPilot().ID ?? "") != (p.ID ?? "") & (p.Personality != "機械" | is_event))
                {
                    p.Morale = (short)(p.Morale + num);
                }
            }

            // サポート
            foreach (Pilot currentP1 in colSupport)
            {
                p = currentP1;
                if (p.Personality != "機械" | is_event)
                {
                    p.Morale = (short)(p.Morale + num);
                }
            }

            // 追加サポート
            string argfname = "追加サポート";
            if (IsFeatureAvailable(ref argfname))
            {
                {
                    var withBlock1 = AdditionalSupport();
                    if (withBlock1.Personality != "機械" | is_event)
                    {
                        withBlock1.Morale = (short)(withBlock1.Morale + num);
                    }
                }
            }
        }


        // ユニットが破壊された時の処理
        public void Die(bool without_update = false)
        {
            short i, j;
            string pname;
            HP = 0;
            Status_Renamed = "破壊";

            // 破壊をキャンセルし、破壊イベント内で処理をしたい場合
            object argIndex2 = "破壊キャンセル";
            if (IsConditionSatisfied(ref argIndex2))
            {
                object argIndex1 = "破壊キャンセル";
                DeleteCondition(ref argIndex1);
                goto SkipExplode;
            }

            // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Map.MapDataForUnit[x, y] = null;

            // 爆発表示
            GUI.ClearPicture();
            string argmain_situation2 = "脱出";
            string argsub_situation2 = "";
            string argmain_situation3 = "脱出";
            string argsub_situation3 = "";
            if (IsAnimationDefined(ref argmain_situation2, sub_situation: ref argsub_situation2))
            {
                GUI.EraseUnitBitmap(x, y, false);
                string argmain_situation = "脱出";
                string argsub_situation = "";
                PlayAnimation(ref argmain_situation, sub_situation: ref argsub_situation);
            }
            else if (IsSpecialEffectDefined(ref argmain_situation3, sub_situation: ref argsub_situation3))
            {
                GUI.EraseUnitBitmap(x, y, false);
                string argmain_situation1 = "脱出";
                string argsub_situation1 = "";
                SpecialEffect(ref argmain_situation1, sub_situation: ref argsub_situation1);
            }
            else
            {
                var argu = this;
                Effect.DieAnimation(ref argu);
            }

        SkipExplode:
            ;


            // 召喚したユニットを解放
            DismissServant();

            // 魅了・憑依したユニットを解放
            DismissSlave();
            if (Master is object)
            {
                object argIndex3 = ID;
                Master.CurrentForm().DeleteSlave(ref argIndex3);
                // UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Master = null;
            }

            if (Summoner is object)
            {
                object argIndex4 = ID;
                Summoner.CurrentForm().DeleteServant(ref argIndex4);
                // UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Summoner = null;
            }

            // 支配しているユニットを強制退却
            string argfname = "支配";
            if (IsFeatureAvailable(ref argfname))
            {
                object argIndex6 = "支配";
                string arglist1 = FeatureData(ref argIndex6);
                var loopTo = GeneralLib.LLength(ref arglist1);
                for (i = 2; i <= loopTo; i++)
                {
                    object argIndex5 = "支配";
                    string arglist = FeatureData(ref argIndex5);
                    pname = GeneralLib.LIndex(ref arglist, i);
                    foreach (Pilot p in SRC.PList)
                    {
                        if ((p.Name ?? "") == (pname ?? "") | (p.get_Nickname(false) ?? "") == (pname ?? ""))
                        {
                            if (p.Unit_Renamed is object)
                            {
                                if (p.Unit_Renamed.Status_Renamed == "出撃" | p.Unit_Renamed.Status_Renamed == "格納")
                                {
                                    p.Unit_Renamed.Die(true);
                                }
                            }
                        }
                    }
                }
            }

            // 情報更新
            if (!without_update)
            {
                SRC.PList.UpdateSupportMod(this);
            }
        }

        // スペシャルパワー自爆による自爆
        public void SuicidalExplosion(bool is_event = false)
        {
            short i, j;
            int prev_hp;
            Unit u, t;
            int dmg, tdmg;
            string uname, fname;
            string argSituation = "自爆";
            string argmsg_mode = "";
            PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
            GUI.DisplaySysMessage(Nickname + "は自爆した。");

            // ダメージ量設定
            dmg = HP;

            // 効果範囲の設定
            string arguparty = "";
            Map.AreaInRange(x, y, 1, 1, ref arguparty);
            Map.MaskData[x, y] = true;

            // 爆発
            GUI.EraseUnitBitmap(x, y);
            string argtsize = Size;
            Effect.ExplodeAnimation(ref argtsize, x, y);
            this.Size = argtsize;

            // パーツ分離できれば自爆後にパーツ分離
            string argfname = "パーツ分離";
            if (IsFeatureAvailable(ref argfname))
            {
                object argIndex1 = "パーツ分離";
                string arglist = FeatureData(ref argIndex1);
                uname = GeneralLib.LIndex(ref arglist, 2);
                Unit localOtherForm() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

                if (localOtherForm().IsAbleToEnter(x, y))
                {
                    Transform(ref uname);
                    Map.MapDataForUnit[x, y].HP = Map.MapDataForUnit[x, y].MaxHP;
                    object argIndex2 = "パーツ分離";
                    fname = FeatureName(ref argIndex2);
                    bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation6 = "破壊時分離(" + Name + ")";
                    string argsub_situation6 = "";
                    string argmain_situation7 = "破壊時分離";
                    string argsub_situation7 = "";
                    string argmain_situation8 = "分離";
                    string argsub_situation8 = "";
                    if (IsSysMessageDefined(ref argmain_situation6, sub_situation: ref argsub_situation6))
                    {
                        string argmain_situation = "破壊時分離(" + Name + ")";
                        string argsub_situation = "";
                        string argadd_msg = "";
                        SysMessage(ref argmain_situation, sub_situation: ref argsub_situation, add_msg: ref argadd_msg);
                    }
                    else if (localIsSysMessageDefined())
                    {
                        string argmain_situation1 = "破壊時分離(" + fname + ")";
                        string argsub_situation1 = "";
                        string argadd_msg1 = "";
                        SysMessage(ref argmain_situation1, sub_situation: ref argsub_situation1, add_msg: ref argadd_msg1);
                    }
                    else if (IsSysMessageDefined(ref argmain_situation7, sub_situation: ref argsub_situation7))
                    {
                        string argmain_situation2 = "破壊時分離";
                        string argsub_situation2 = "";
                        string argadd_msg2 = "";
                        SysMessage(ref argmain_situation2, sub_situation: ref argsub_situation2, add_msg: ref argadd_msg2);
                    }
                    else if (localIsSysMessageDefined1())
                    {
                        string argmain_situation3 = "分離(" + Name + ")";
                        string argsub_situation3 = "";
                        string argadd_msg3 = "";
                        SysMessage(ref argmain_situation3, sub_situation: ref argsub_situation3, add_msg: ref argadd_msg3);
                    }
                    else if (localIsSysMessageDefined2())
                    {
                        string argmain_situation4 = "分離(" + fname + ")";
                        string argsub_situation4 = "";
                        string argadd_msg4 = "";
                        SysMessage(ref argmain_situation4, sub_situation: ref argsub_situation4, add_msg: ref argadd_msg4);
                    }
                    else if (IsSysMessageDefined(ref argmain_situation8, sub_situation: ref argsub_situation8))
                    {
                        string argmain_situation5 = "分離";
                        string argsub_situation5 = "";
                        string argadd_msg5 = "";
                        SysMessage(ref argmain_situation5, sub_situation: ref argsub_situation5, add_msg: ref argadd_msg5);
                    }
                    else
                    {
                        GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
                    }

                    goto SkipSuicide;
                }
            }

            // 自分を破壊
            HP = 0;
            var argu1 = this;
            object argu2 = null;
            GUI.UpdateMessageForm(ref argu1, u2: ref argu2);
            // 既に爆発アニメーションを表示しているので
            string argcname = "破壊キャンセル";
            string argcdata = "";
            AddCondition(ref argcname, 1, cdata: ref argcdata);
            // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Map.MapDataForUnit[x, y] = null;
            Die();
            if (!is_event)
            {
                u = Commands.SelectedUnit;
                Commands.SelectedUnit = this;
                Event_Renamed.HandleEvent("破壊", MainPilot().ID);
                Commands.SelectedUnit = u;
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    return;
                }
            }

        SkipSuicide:
            ;


            // 周りのエリアに爆発効果を適用
            var loopTo = Map.MapWidth;
            for (i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (j = 1; j <= loopTo1; j++)
                {
                    if (Map.MaskData[i, j])
                    {
                        goto NextLoop;
                    }

                    t = Map.MapDataForUnit[i, j];
                    if (t is null)
                    {
                        goto NextLoop;
                    }

                    {
                        var withBlock = t;
                        GUI.ClearMessageForm();
                        if (CurrentForm().Party == "味方" | CurrentForm().Party == "ＮＰＣ")
                        {
                            object argu21 = CurrentForm();
                            GUI.UpdateMessageForm(ref t, ref argu21);
                        }
                        else
                        {
                            object argu22 = t;
                            GUI.UpdateMessageForm(ref CurrentForm(), ref argu22);
                        }

                        // ダメージの適用
                        prev_hp = withBlock.HP;
                        object argIndex3 = "無敵";
                        object argIndex4 = "不死身";
                        if (withBlock.IsConditionSatisfied(ref argIndex3))
                        {
                            tdmg = 0;
                        }
                        else if (withBlock.IsConditionSatisfied(ref argIndex4))
                        {
                            withBlock.HP = GeneralLib.MaxLng(withBlock.HP - dmg, 10);
                            tdmg = prev_hp - withBlock.HP;
                        }
                        else
                        {
                            withBlock.HP = withBlock.HP - dmg;
                            tdmg = prev_hp - withBlock.HP;
                        }

                        // 特殊能力「不安定」による暴走チェック
                        string argfname1 = "不安定";
                        if (withBlock.IsFeatureAvailable(ref argfname1))
                        {
                            object argIndex5 = "暴走";
                            if (withBlock.HP <= withBlock.MaxHP / 4 & !withBlock.IsConditionSatisfied(ref argIndex5))
                            {
                                string argcname1 = "暴走";
                                string argcdata1 = "";
                                withBlock.AddCondition(ref argcname1, -1, cdata: ref argcdata1);
                                withBlock.Update();
                            }
                        }

                        // ダメージを受ければ眠りからさめる
                        object argIndex7 = "睡眠";
                        if (withBlock.IsConditionSatisfied(ref argIndex7))
                        {
                            object argIndex6 = "睡眠";
                            withBlock.DeleteCondition(ref argIndex6);
                        }

                        if (CurrentForm().Party == "味方" | CurrentForm().Party == "ＮＰＣ")
                        {
                            object argu23 = CurrentForm();
                            GUI.UpdateMessageForm(ref t, ref argu23);
                        }
                        else
                        {
                            object argu24 = t;
                            GUI.UpdateMessageForm(ref CurrentForm(), ref argu24);
                        }

                        if (withBlock.HP > 0)
                        {
                            string argmsg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tdmg);
                            GUI.DrawSysString(withBlock.x, withBlock.y, ref argmsg);
                            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.picMain(0).Refresh();
                        }

                        if (withBlock.HP == 0)
                        {
                            string argsptype = "復活";
                            if (withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                            {
                                withBlock.HP = withBlock.MaxHP;
                                string argstype = "破壊";
                                withBlock.RemoveSpecialPowerInEffect(ref argstype);
                                GUI.DisplaySysMessage(withBlock.Nickname + "は復活した！");
                                goto NextLoop;
                            }

                            string argfname2 = "パーツ分離";
                            if (withBlock.IsFeatureAvailable(ref argfname2))
                            {
                                object argIndex8 = "パーツ分離";
                                string arglist1 = withBlock.FeatureData(ref argIndex8);
                                uname = GeneralLib.LIndex(ref arglist1, 2);
                                Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                                if (localOtherForm1().IsAbleToEnter(withBlock.x, withBlock.y))
                                {
                                    withBlock.Transform(ref uname);
                                    {
                                        var withBlock1 = withBlock.CurrentForm();
                                        withBlock1.HP = withBlock1.MaxHP;
                                        withBlock1.UsedAction = withBlock1.MaxAction();
                                    }

                                    GUI.DisplaySysMessage(withBlock.Nickname + "は破壊されたパーツを分離させた。");
                                    goto NextLoop;
                                }
                            }

                            withBlock.Die();
                        }

                        if (!is_event)
                        {
                            u = Commands.SelectedUnit;
                            Commands.SelectedUnit = withBlock.CurrentForm();
                            Commands.SelectedTarget = this;
                            if (withBlock.Status_Renamed == "破壊")
                            {
                                GUI.DisplaySysMessage(withBlock.Nickname + "は破壊された");
                                Event_Renamed.HandleEvent("破壊", withBlock.MainPilot().ID);
                            }
                            else
                            {
                                GUI.DisplaySysMessage(withBlock.Nickname + "は" + tdmg + "のダメージを受けた。;" + "残りＨＰは" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.HP) + "（損傷率 = " + 100 * (withBlock.MaxHP - withBlock.HP) / withBlock.MaxHP + "％）");
                                Event_Renamed.HandleEvent("損傷率", withBlock.MainPilot().ID, 100 - withBlock.HP * 100 / withBlock.MaxHP);
                            }

                            Commands.SelectedUnit = u;
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                return;
                            }
                        }
                    }

                NextLoop:
                    ;
                }
            }
        }
    }
}
