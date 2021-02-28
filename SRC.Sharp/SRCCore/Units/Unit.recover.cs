using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === ステータス回復関連処理 ===
    public partial class Unit
    {
        // ステータスを全回復
        public void FullRecover()
        {
            // XXX
            Update();
            //    short i, j;

            //    // パイロットのステータスを全回復
            //    var loopTo = CountPilot();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //        localPilot().FullRecover();
            //    }

            //    var loopTo1 = CountSupport();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        Pilot localSupport() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //        localSupport().FullRecover();
            //    }

            //    string argfname = "追加パイロット";
            //    if (IsFeatureAvailable(ref argfname))
            //    {
            //        object argIndex1 = "追加パイロット";
            //        object argIndex2 = FeatureData(ref argIndex1);
            //        if (SRC.PList.IsDefined(ref argIndex2))
            //        {
            //            Pilot localItem() { object argIndex1 = "追加パイロット"; object argIndex2 = FeatureData(ref argIndex1); var ret = SRC.PList.Item(ref argIndex2); return ret; }

            //            localItem().FullRecover();
            //        }
            //    }

            //    {
            //        var withBlock = CurrentForm();
            //        // ＨＰを回復
            //        withBlock.HP = withBlock.MaxHP;

            //        // ＥＮ、弾数を回復
            //        withBlock.FullSupply();

            //        // ステータス異常のみを消去
            //        i = 1;
            //        while (i <= withBlock.CountCondition())
            //        {
            //            string localCondition() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition1() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition2() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition3() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition4() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition5() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            string localCondition6() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

            //            if (localCondition() == "残り時間" | localCondition1() == "非操作" | Strings.Right(localCondition2(), 2) == "付加" | Strings.Right(localCondition3(), 2) == "強化" | Strings.Right(localCondition4(), 3) == "付加２" | Strings.Right(localCondition5(), 3) == "強化２" | Strings.Right(localCondition6(), 2) == "ＵＰ")
            //            {
            //                i = (short)(i + 1);
            //            }
            //            else
            //            {
            //                object argIndex3 = i;
            //                withBlock.DeleteCondition(ref argIndex3);
            //            }
            //        }

            //        // サポートアタック＆ガード、同時援護攻撃、カウンター攻撃回数回復
            //        withBlock.UsedSupportAttack = 0;
            //        withBlock.UsedSupportGuard = 0;
            //        withBlock.UsedSyncAttack = 0;
            //        withBlock.UsedCounterAttack = 0;
            //        withBlock.Mode = "通常";

            //        // 他形態も回復
            //        var loopTo2 = withBlock.CountOtherForm();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            object argIndex4 = i;
            //            {
            //                var withBlock1 = withBlock.OtherForm(ref argIndex4);
            //                withBlock1.HP = withBlock1.MaxHP;
            //                withBlock1.EN = withBlock1.MaxEN;
            //                var loopTo3 = withBlock1.CountWeapon();
            //                for (j = 1; j <= loopTo3; j++)
            //                    withBlock1.SetBullet(j, withBlock1.MaxBullet(j));
            //                var loopTo4 = withBlock1.CountAbility();
            //                for (j = 1; j <= loopTo4; j++)
            //                    withBlock1.SetStock(j, withBlock1.MaxStock(j));
            //            }
            //        }
            //    }
        }

        // ＥＮ＆弾数を回復
        public void FullSupply()
        {
            //    short i, j;

            //    // ＥＮ回復
            //    EN = MaxEN;

            //    // 弾数回復
            //    var loopTo = CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //        dblBullet[i] = 1d;
            //    var loopTo1 = CountAbility();
            //    for (i = 1; i <= loopTo1; i++)
            //        dblStock[i] = 1d;

            //    // 他形態も回復
            //    var loopTo2 = CountOtherForm();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        object argIndex1 = i;
            //        {
            //            var withBlock = OtherForm(ref argIndex1);
            //            withBlock.EN = withBlock.MaxEN;
            //            var loopTo3 = withBlock.CountWeapon();
            //            for (j = 1; j <= loopTo3; j++)
            //                withBlock.SetBullet(j, withBlock.MaxBullet(j));
            //            var loopTo4 = withBlock.CountAbility();
            //            for (j = 1; j <= loopTo4; j++)
            //                withBlock.SetStock(j, withBlock.MaxStock(j));
            //        }
            //    }
        }

        // 弾数のみを回復
        public void BulletSupply()
        {
            //    short i, j;
            //    var loopTo = CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //        dblBullet[i] = 1d;

            //    // 他形態も回復
            //    var loopTo1 = CountOtherForm();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        object argIndex1 = i;
            //        {
            //            var withBlock = OtherForm(ref argIndex1);
            //            var loopTo2 = withBlock.CountWeapon();
            //            for (j = 1; j <= loopTo2; j++)
            //                withBlock.SetBullet(j, withBlock.MaxBullet(j));
            //        }
            //    }
        }

        // ＨＰを percent ％回復
        public void RecoverHP(double percent)
        {
            //    HP = (int)(HP + MaxHP * percent / 100d);
            //    if (HP <= 0)
            //    {
            //        HP = 1;
            //    }

            //    // 特殊能力「不安定」による暴走チェック
            //    string argfname = "不安定";
            //    if (IsFeatureAvailable(ref argfname))
            //    {
            //        object argIndex1 = "暴走";
            //        if (HP <= MaxHP / 4 & !IsConditionSatisfied(ref argIndex1))
            //        {
            //            string argcname = "暴走";
            //            string argcdata = "";
            //            AddCondition(ref argcname, -1, cdata: ref argcdata);
            //        }
            //    }
        }

        // ＥＮを percent ％回復
        public void RecoverEN(double percent)
        {
            EN = (int)(EN + MaxEN * percent / 100d);
            if (EN <= 0)
            {
                EN = 0;
            }
        }

        // ターン経過によるステータス回復
        public void Rest()
        {
            //    int hp_recovery = default, en_recovery = default;
            //    int hp_up = default, en_up = default;
            //    double hp_ratio, en_ratio;
            //    string spname, buf;
            //    short i, j;
            //    Unit u;
            //    TerrainData td;
            //    string cname;
            //    var is_time_limit = default(bool);
            //    var next_form = default(string);
            //    // ADD START MARGE
            //    var is_terrain_effective = default(bool);
            //    var is_immune_to_terrain_effect = default(bool);
            //    // ADD END MARGE

            //    // 味方ステージの1ターン目(スタートイベント直後)は回復を行わない
            //    if (SRC.Stage == "味方" & SRC.Turn == 1)
            //    {
            //        return;
            //    }

            // データ更新
            Update();

            //    // 変形に対応して自分を登録
            //    u = this;
            //    {
            //        var withBlock = MainPilot();
            //        // 霊力回復
            //        if (withBlock.MaxPlana() > 0)
            //        {
            //            hp_ratio = 100 * HP / (double)MaxHP;
            //            en_ratio = 100 * EN / (double)MaxEN;
            //            object argIndex1 = "霊力回復";
            //            object argIndex2 = "霊力消費";
            //            withBlock.Plana = (int)(withBlock.Plana + withBlock.MaxPlana() / 16 + (long)(withBlock.MaxPlana() * FeatureLevel(ref argIndex1)) / 10L - (long)(withBlock.MaxPlana() * FeatureLevel(ref argIndex2)) / 10L);
            //            HP = (int)((long)(MaxHP * hp_ratio) / 100L);
            //            EN = (int)((long)(MaxEN * en_ratio) / 100L);
            //        }

            //        // ＳＰ回復
            //        string argsname = "ＳＰ回復";
            //        if (withBlock.IsSkillAvailable(ref argsname))
            //        {
            //            withBlock.SP = withBlock.SP + withBlock.Level / 8 + 5;
            //        }

            //        string argsname1 = "精神統一";
            //        if (withBlock.IsSkillAvailable(ref argsname1))
            //        {
            //            if (withBlock.SP < withBlock.MaxSP / 5)
            //            {
            //                withBlock.SP = withBlock.SP + withBlock.MaxSP / 10;
            //            }
            //        }
            //    }

            //    // ＳＰ回復
            //    var loopTo = CountPilot();
            //    for (i = 2; i <= loopTo; i++)
            //    {
            //        object argIndex3 = i;
            //        {
            //            var withBlock1 = Pilot(ref argIndex3);
            //            string argsname2 = "ＳＰ回復";
            //            if (withBlock1.IsSkillAvailable(ref argsname2))
            //            {
            //                withBlock1.SP = withBlock1.SP + withBlock1.Level / 8 + 5;
            //            }

            //            string argsname3 = "精神統一";
            //            if (withBlock1.IsSkillAvailable(ref argsname3))
            //            {
            //                if (withBlock1.SP < withBlock1.MaxSP / 5)
            //                {
            //                    withBlock1.SP = withBlock1.SP + withBlock1.MaxSP / 10;
            //                }
            //            }
            //        }
            //    }

            //    var loopTo1 = CountSupport();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        object argIndex4 = i;
            //        {
            //            var withBlock2 = Support(ref argIndex4);
            //            string argsname4 = "ＳＰ回復";
            //            if (withBlock2.IsSkillAvailable(ref argsname4))
            //            {
            //                withBlock2.SP = withBlock2.SP + withBlock2.Level / 8 + 5;
            //            }

            //            string argsname5 = "精神統一";
            //            if (withBlock2.IsSkillAvailable(ref argsname5))
            //            {
            //                if (withBlock2.SP < withBlock2.MaxSP / 5)
            //                {
            //                    withBlock2.SP = withBlock2.SP + withBlock2.MaxSP / 10;
            //                }
            //            }
            //        }
            //    }

            //    string argfname = "追加サポート";
            //    if (IsFeatureAvailable(ref argfname))
            //    {
            //        {
            //            var withBlock3 = AdditionalSupport();
            //            string argsname6 = "ＳＰ回復";
            //            if (withBlock3.IsSkillAvailable(ref argsname6))
            //            {
            //                withBlock3.SP = withBlock3.SP + withBlock3.Level / 8 + 5;
            //            }

            //            string argsname7 = "精神統一";
            //            if (withBlock3.IsSkillAvailable(ref argsname7))
            //            {
            //                if (withBlock3.SP < withBlock3.MaxSP / 5)
            //                {
            //                    withBlock3.SP = withBlock3.SP + withBlock3.MaxSP / 10;
            //                }
            //            }
            //        }
            //    }

            //    // 行動回数
            //    UsedAction = 0;

            //    // スペシャルパワー効果を解除
            //    string argstype = "ターン";
            //    RemoveSpecialPowerInEffect(ref argstype);

            //    // スペシャルパワー自動発動
            //    {
            //        var withBlock4 = MainPilot();
            //        var loopTo2 = withBlock4.CountSkill();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            object argIndex5 = i;
            //            if (withBlock4.Skill(ref argIndex5) == "スペシャルパワー自動発動")
            //            {
            //                string localSkillData() { object argIndex1 = i; var ret = withBlock4.SkillData(ref argIndex1); return ret; }

            //                string arglist = localSkillData();
            //                spname = GeneralLib.LIndex(ref arglist, 2);
            //                string localSkillData1() { object argIndex1 = i; var ret = withBlock4.SkillData(ref argIndex1); return ret; }

            //                string localLIndex() { string arglist = hsbeebf58ad14b49d88a05b39a41264136(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

            //                int localStrToLng() { string argexpr = hsa615f5f7d41941adb5ff8c50a645e7c9(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                if (withBlock4.Morale >= localStrToLng() & !IsSpecialPowerInEffect(ref spname))
            //                {
            //                    GUI.Center(x, y);
            //                    withBlock4.UseSpecialPower(ref spname, 0d);
            //                    if (Status_Renamed == "他形態")
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //        }

            //        object argIndex7 = "スペシャルパワー自動発動付加";
            //        object argIndex8 = "スペシャルパワー自動発動付加２";
            //        if (IsConditionSatisfied(ref argIndex7) | IsConditionSatisfied(ref argIndex8))
            //        {
            //            object argIndex6 = "スペシャルパワー自動発動";
            //            string arglist1 = withBlock4.SkillData(ref argIndex6);
            //            spname = GeneralLib.LIndex(ref arglist1, 2);
            //            string localLIndex1() { object argIndex1 = "スペシャルパワー自動発動"; string arglist = withBlock4.SkillData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

            //            int localStrToLng1() { string argexpr = hs007c2f9d021b4299932cdd690ee4914e(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //            if (withBlock4.Morale >= localStrToLng1() & !IsSpecialPowerInEffect(ref spname))
            //            {
            //                GUI.Center(x, y);
            //                withBlock4.UseSpecialPower(ref spname, 0d);
            //                if (Status_Renamed == "他形態")
            //                {
            //                    return;
            //                }
            //            }
            //        }
            //    }

            //    var loopTo3 = CountPilot();
            //    for (i = 2; i <= loopTo3; i++)
            //    {
            //        object argIndex10 = i;
            //        {
            //            var withBlock5 = Pilot(ref argIndex10);
            //            var loopTo4 = withBlock5.CountSkill();
            //            for (j = 1; j <= loopTo4; j++)
            //            {
            //                object argIndex9 = j;
            //                if (withBlock5.Skill(ref argIndex9) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData2() { object argIndex1 = j; var ret = withBlock5.SkillData(ref argIndex1); return ret; }

            //                    string arglist2 = localSkillData2();
            //                    spname = GeneralLib.LIndex(ref arglist2, 2);
            //                    string localSkillData3() { object argIndex1 = j; var ret = withBlock5.SkillData(ref argIndex1); return ret; }

            //                    string localLIndex2() { string arglist = hsa05862d2d473454a8c0d7941dff82ac5(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

            //                    int localStrToLng2() { string argexpr = hs1050a377ee804d6d86fc7e98398e12ee(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (withBlock5.Morale >= localStrToLng2() & !IsSpecialPowerInEffect(ref spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock5.UseSpecialPower(ref spname, 0d);
            //                        if (Status_Renamed == "他形態")
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    var loopTo5 = CountSupport();
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        object argIndex12 = i;
            //        {
            //            var withBlock6 = Support(ref argIndex12);
            //            var loopTo6 = withBlock6.CountSkill();
            //            for (j = 1; j <= loopTo6; j++)
            //            {
            //                object argIndex11 = j;
            //                if (withBlock6.Skill(ref argIndex11) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData4() { object argIndex1 = j; var ret = withBlock6.SkillData(ref argIndex1); return ret; }

            //                    string arglist3 = localSkillData4();
            //                    spname = GeneralLib.LIndex(ref arglist3, 2);
            //                    string localSkillData5() { object argIndex1 = j; var ret = withBlock6.SkillData(ref argIndex1); return ret; }

            //                    string localLIndex3() { string arglist = hsfa97fc36239f45ffacc7690204644669(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

            //                    int localStrToLng3() { string argexpr = hsaaa9e6d3323e4453a1b656c0d1ba1987(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (withBlock6.Morale >= localStrToLng3() & !IsSpecialPowerInEffect(ref spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock6.UseSpecialPower(ref spname, 0d);
            //                        if (Status_Renamed == "他形態")
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    string argfname1 = "追加サポート";
            //    if (IsFeatureAvailable(ref argfname1))
            //    {
            //        {
            //            var withBlock7 = AdditionalSupport();
            //            var loopTo7 = withBlock7.CountSkill();
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                object argIndex13 = i;
            //                if (withBlock7.Skill(ref argIndex13) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData6() { object argIndex1 = i; var ret = withBlock7.SkillData(ref argIndex1); return ret; }

            //                    string arglist4 = localSkillData6();
            //                    spname = GeneralLib.LIndex(ref arglist4, 2);
            //                    string localSkillData7() { object argIndex1 = i; var ret = withBlock7.SkillData(ref argIndex1); return ret; }

            //                    string localLIndex4() { string arglist = hs393cfbef7a894559b16f4706c8e49b64(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

            //                    int localStrToLng4() { string argexpr = hsd3fcfdcfd593490696f3d091f42d35e5(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (withBlock7.Morale >= localStrToLng4() & !IsSpecialPowerInEffect(ref spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock7.UseSpecialPower(ref spname, 0d);
            //                        if (Status_Renamed == "他形態")
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 起死回生
            //    {
            //        var withBlock8 = MainPilot();
            //        string argsname8 = "起死回生";
            //        if (withBlock8.IsSkillAvailable(ref argsname8) & withBlock8.SP <= withBlock8.MaxSP / 5 & HP <= MaxHP / 5 & EN <= MaxEN / 5)
            //        {
            //            withBlock8.SP = withBlock8.MaxSP;
            //            HP = MaxHP;
            //            EN = MaxEN;
            //            if (SRC.SpecialPowerAnimation)
            //            {
            //                GUI.Center(x, y);
            //                object argIndex15 = "ド根性";
            //                if (SRC.SPDList.IsDefined(ref argIndex15))
            //                {
            //                    object argIndex14 = "ド根性";
            //                    SRC.SPDList.Item(ref argIndex14).PlayAnimation();
            //                }
            //            }
            //        }
            //    }

            //    // ＨＰとＥＮ回復＆消費
            //    // MOD START MARGE
            //    // If Not IsConditionSatisfied("回復不能") Then
            //    object argIndex18 = "回復不能";
            //    string argsname9 = "回復不能";
            //    if (!IsConditionSatisfied(ref argIndex18) & !IsSpecialPowerInEffect(ref argsname9))
            //    {
            //        // MOD END MARGE
            //        string argfname2 = "ＨＰ回復";
            //        if (IsFeatureAvailable(ref argfname2))
            //        {
            //            object argIndex16 = "ＨＰ回復";
            //            hp_recovery = (int)(10d * FeatureLevel(ref argIndex16));
            //        }

            //        string argfname3 = "ＥＮ回復";
            //        if (IsFeatureAvailable(ref argfname3))
            //        {
            //            object argIndex17 = "ＥＮ回復";
            //            en_recovery = (int)(10d * FeatureLevel(ref argIndex17));
            //        }
            //    }

            //    string argfname4 = "ＨＰ消費";
            //    if (IsFeatureAvailable(ref argfname4))
            //    {
            //        object argIndex19 = "ＨＰ消費";
            //        hp_recovery = (int)(hp_recovery - 10d * FeatureLevel(ref argIndex19));
            //    }

            //    string argfname5 = "ＥＮ消費";
            //    if (IsFeatureAvailable(ref argfname5))
            //    {
            //        object argIndex20 = "ＥＮ消費";
            //        en_recovery = (int)(en_recovery - 10d * FeatureLevel(ref argIndex20));
            //    }

            //    // 毒によるＨＰ減少
            //    short plv;
            //    object argIndex21 = "毒";
            //    if (IsConditionSatisfied(ref argIndex21))
            //    {
            //        string argoname = "毒効果大";
            //        if (Expression.IsOptionDefined(ref argoname) & BossRank < 0)
            //        {
            //            plv = 25;
            //        }
            //        else
            //        {
            //            plv = 10;
            //        }

            //        string arganame = "毒";
            //        string arganame1 = "毒";
            //        // 変化なし
            //        string arganame2 = "毒";
            //        string arganame3 = "毒";
            //        string arganame4 = "毒";
            //        if (Weakness(ref arganame))
            //        {
            //            plv = (short)(2 * plv);
            //        }
            //        else if (Effective(ref arganame1))
            //        {
            //        }
            //        else if (Immune(ref arganame2) | Absorb(ref arganame3))
            //        {
            //            plv = 0;
            //        }
            //        else if (Resist(ref arganame4))
            //        {
            //            plv = (short)(plv / 2);
            //        }

            //        hp_recovery = hp_recovery - plv;
            //    }

            //    // 活動限界時間切れ？
            //    object argIndex22 = "活動限界";
            //    if (ConditionLifetime(ref argIndex22) == 1)
            //    {
            //        GUI.Center(x, y);
            //        Escape();
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            //        GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //        GUI.CloseMessageForm();
            //        Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //    }

            //    // 死の宣告
            //    object argIndex23 = "死の宣告";
            //    if (ConditionLifetime(ref argIndex23) == 1)
            //    {
            //        hp_recovery = hp_recovery - 1000;
            //    }

            //    // 残り時間
            //    object argIndex25 = "残り時間";
            //    if (ConditionLifetime(ref argIndex25) == 1)
            //    {
            //        is_time_limit = true;
            //        string argfname6 = "ノーマルモード";
            //        if (IsFeatureAvailable(ref argfname6))
            //        {
            //            // ハイパーモード＆変身の時間切れの場合は戻り先の形態を記録しておく
            //            object argIndex24 = "ノーマルモード";
            //            string arglist5 = FeatureData(ref argIndex24);
            //            next_form = GeneralLib.LIndex(ref arglist5, 1);
            //        }
            //    }

            //    // ＨＰ回復などを付加した場合のことを考えて状態のアップデートは
            //    // ＨＰ＆ＥＮ回復量を計算した後に行う
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;
            //    UpdateCondition(true);
            //    HP = (int)((long)(MaxHP * hp_ratio) / 100L);
            //    EN = (int)((long)(MaxEN * en_ratio) / 100L);

            //    // サポートアタック＆ガード
            //    UsedSupportAttack = 0;
            //    UsedSupportGuard = 0;

            //    // 同時援護攻撃
            //    UsedSyncAttack = 0;

            //    // カウンター攻撃
            //    UsedCounterAttack = 0;

            //    // チャージ完了？
            //    object argIndex26 = "チャージ";
            //    if (ConditionLifetime(ref argIndex26) == 0)
            //    {
            //        string argcname = "チャージ完了";
            //        string argcdata = "";
            //        AddCondition(ref argcname, 1, cdata: ref argcdata);
            //    }

            //    // 付加された移動能力が切れた場合の処理
            //    if (Status_Renamed == "出撃" & !string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        switch (Area ?? "")
            //        {
            //            case "空中":
            //                {
            //                    string argarea_name = "空";
            //                    if (!IsTransAvailable(ref argarea_name))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            Unit argu11 = null;
            //                            Unit argu21 = null;
            //                            GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                            return;
            //                        }
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[x, y] = null;
            //                        GUI.EraseUnitBitmap(x, y);
            //                        StandBy(x, y);
            //                    }

            //                    break;
            //                }

            //            case "地上":
            //                {
            //                    string argarea_name1 = "陸";
            //                    if (!IsTransAvailable(ref argarea_name1))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            Unit argu12 = null;
            //                            Unit argu22 = null;
            //                            GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                            return;
            //                        }
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[x, y] = null;
            //                        GUI.EraseUnitBitmap(x, y);
            //                        StandBy(x, y);
            //                    }

            //                    break;
            //                }

            //            case "水上":
            //                {
            //                    string argfname7 = "水上移動";
            //                    string argfname8 = "ホバー移動";
            //                    if (!IsFeatureAvailable(ref argfname7) & !IsFeatureAvailable(ref argfname8))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            Unit argu13 = null;
            //                            Unit argu23 = null;
            //                            GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                            return;
            //                        }
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[x, y] = null;
            //                        GUI.EraseUnitBitmap(x, y);
            //                        StandBy(x, y);
            //                    }

            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    if (get_Adaption(3) == 0)
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            Unit argu14 = null;
            //                            Unit argu24 = null;
            //                            GUI.OpenMessageForm(u1: ref argu14, u2: ref argu24);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                            return;
            //                        }
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[x, y] = null;
            //                        GUI.EraseUnitBitmap(x, y);
            //                        StandBy(x, y);
            //                    }

            //                    break;
            //                }
            //        }
            //    }

            //    if (Status_Renamed == "格納")
            //    {
            //        // 格納時は回復率ＵＰ
            //        // MOD START MARGE
            //        // If Not IsConditionSatisfied("回復不能") Then
            //        object argIndex27 = "回復不能";
            //        string argsname10 = "回復不能";
            //        if (!IsConditionSatisfied(ref argIndex27) & !IsSpecialPowerInEffect(ref argsname10))
            //        {
            //            // MOD END MARGE
            //            hp_recovery = hp_recovery + 50;
            //            en_recovery = en_recovery + 50;
            //        }

            //        // 弾数回復
            //        var loopTo8 = CountWeapon();
            //        for (i = 1; i <= loopTo8; i++)
            //            dblBullet[i] = 1d;
            //        var loopTo9 = CountAbility();
            //        for (i = 1; i <= loopTo9; i++)
            //            dblStock[i] = 1d;
            //    }
            //    else
            //    {
            //        // MOD START MARGE
            //        // '格納されてない場合は地形による回復修正
            //        // If Not IsConditionSatisfied("回復不能") Then
            //        // hp_recovery = hp_recovery + TerrainEffectForHPRecover(X, Y)
            //        // en_recovery = en_recovery + TerrainEffectForENRecover(X, Y)
            //        // End If
            //        // 
            //        // '地形による減少修正＆状態付加
            //        // Set td = TDList.Item(MapData(X, Y, 0))
            //        // With td
            //        // For i = 1 To .CountFeature
            //        // Select Case .Feature(i)
            //        // Case "ＨＰ減少"
            //        // If Weakness(.FeatureData(i)) Then
            //        // hp_recovery = hp_recovery - 20 * .FeatureLevel(i)
            //        // ElseIf Effective(.FeatureData(i)) Then
            //        // hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
            //        // ElseIf Not Immune(.FeatureData(i)) Then
            //        // If Absorb(.FeatureData(i)) Then
            //        // hp_recovery = hp_recovery + 10 * .FeatureLevel(i)
            //        // ElseIf Resist(.FeatureData(i)) Then
            //        // hp_recovery = hp_recovery - 5 * .FeatureLevel(i)
            //        // Else
            //        // hp_recovery = hp_recovery - 10 * .FeatureLevel(i)
            //        // End If
            //        // End If
            //        // 
            //        // Case "ＥＮ減少"
            //        // If Weakness(.FeatureData(i)) Then
            //        // en_recovery = en_recovery - 20 * .FeatureLevel(i)
            //        // ElseIf Effective(.FeatureData(i)) Then
            //        // en_recovery = en_recovery - 10 * .FeatureLevel(i)
            //        // ElseIf Not Immune(.FeatureData(i)) Then
            //        // If Absorb(.FeatureData(i)) Then
            //        // en_recovery = en_recovery + 10 * .FeatureLevel(i)
            //        // ElseIf Resist(.FeatureData(i)) Then
            //        // en_recovery = en_recovery - 5 * .FeatureLevel(i)
            //        // Else
            //        // en_recovery = en_recovery - 10 * .FeatureLevel(i)
            //        // End If
            //        // End If
            //        // 
            //        // Case "ＨＰ増加"
            //        // If Not IsConditionSatisfied("回復不能") Then
            //        // hp_up = hp_up + 1000 * .FeatureLevel(i)
            //        // End If
            //        // 
            //        // Case "ＥＮ増加"
            //        // If Not IsConditionSatisfied("回復不能") Then
            //        // en_up = en_up + 10 * .FeatureLevel(i)
            //        // End If
            //        // 
            //        // Case "ＨＰ低下"
            //        // If Weakness(.FeatureData(i)) Then
            //        // hp_up = hp_up - 2000 * .FeatureLevel(i)
            //        // ElseIf Effective(.FeatureData(i)) Then
            //        // hp_up = hp_up - 1000 * .FeatureLevel(i)
            //        // ElseIf Not Immune(.FeatureData(i)) Then
            //        // If Absorb(.FeatureData(i)) Then
            //        // hp_up = hp_up + 1000 * .FeatureLevel(i)
            //        // ElseIf Resist(.FeatureData(i)) Then
            //        // hp_up = hp_up - 500 * .FeatureLevel(i)
            //        // Else
            //        // hp_up = hp_up - 1000 * .FeatureLevel(i)
            //        // End If
            //        // End If
            //        // 
            //        // Case "ＥＮ低下"
            //        // If Weakness(.FeatureData(i)) Then
            //        // en_up = en_up - 20 * .FeatureLevel(i)
            //        // ElseIf Effective(.FeatureData(i)) Then
            //        // en_up = en_up - 10 * .FeatureLevel(i)
            //        // ElseIf Not Immune(.FeatureData(i)) Then
            //        // If Absorb(.FeatureData(i)) Then
            //        // en_up = en_up + 10 * .FeatureLevel(i)
            //        // ElseIf Resist(.FeatureData(i)) Then
            //        // en_up = en_up - 5 * .FeatureLevel(i)
            //        // Else
            //        // en_up = en_up - 10 * .FeatureLevel(i)
            //        // End If
            //        // End If
            //        // 
            //        // Case "状態付加"
            //        // cname = .FeatureData(i)
            //        // 
            //        // '状態が無効化されるかチェック
            //        // Select Case cname
            //        // Case "装甲劣化"
            //        // If SpecialEffectImmune("劣") Then
            //        // cname = ""
            //        // End If
            //        // Case "混乱"
            //        // If SpecialEffectImmune("乱") Then
            //        // cname = ""
            //        // End If
            //        // Case "恐怖"
            //        // If SpecialEffectImmune("恐") Then
            //        // cname = ""
            //        // End If
            //        // Case "踊り"
            //        // If SpecialEffectImmune("踊") Then
            //        // cname = ""
            //        // End If
            //        // Case "狂戦士"
            //        // If SpecialEffectImmune("狂") Then
            //        // cname = ""
            //        // End If
            //        // Case "ゾンビ"
            //        // If SpecialEffectImmune("ゾ") Then
            //        // cname = ""
            //        // End If
            //        // Case "回復不能"
            //        // If SpecialEffectImmune("害") Then
            //        // cname = ""
            //        // End If
            //        // Case "石化"
            //        // If SpecialEffectImmune("石") Then
            //        // cname = ""
            //        // End If
            //        // Case "凍結"
            //        // If SpecialEffectImmune("凍") Then
            //        // cname = ""
            //        // End If
            //        // Case "麻痺"
            //        // If SpecialEffectImmune("痺") Then
            //        // cname = ""
            //        // End If
            //        // Case "睡眠"
            //        // If SpecialEffectImmune("眠") Then
            //        // cname = ""
            //        // End If
            //        // Case "毒"
            //        // If SpecialEffectImmune("毒") Then
            //        // cname = ""
            //        // End If
            //        // Case "盲目"
            //        // If SpecialEffectImmune("盲") Then
            //        // cname = ""
            //        // End If
            //        // Case "沈黙"
            //        // If SpecialEffectImmune("黙") Then
            //        // cname = ""
            //        // End If
            //        // '属性使用不能状態
            //        // Case "オーラ使用不能"
            //        // If SpecialEffectImmune("剋オ") Then
            //        // cname = ""
            //        // End If
            //        // Case "超能力使用不能"
            //        // If SpecialEffectImmune("剋超") Then
            //        // cname = ""
            //        // End If
            //        // Case "同調率使用不能"
            //        // If SpecialEffectImmune("剋シ") Then
            //        // cname = ""
            //        // End If
            //        // Case "超感覚使用不能"
            //        // If SpecialEffectImmune("剋サ") Then
            //        // cname = ""
            //        // End If
            //        // Case "知覚強化使用不能"
            //        // If SpecialEffectImmune("剋サ") Then
            //        // cname = ""
            //        // End If
            //        // Case "霊力使用不能"
            //        // If SpecialEffectImmune("剋霊") Then
            //        // cname = ""
            //        // End If
            //        // Case "術使用不能"
            //        // If SpecialEffectImmune("剋術") Then
            //        // cname = ""
            //        // End If
            //        // Case "技使用不能"
            //        // If SpecialEffectImmune("剋技") Then
            //        // cname = ""
            //        // End If
            //        // Case Else
            //        // If Len(cname) > 6 Then
            //        // If Right$(cname, 6) = "属性弱点付加" Then
            //        // If SpecialEffectImmune("弱" & Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Immune(Left$(cname, Len(cname) - 6)) _
            //        // '                                        Then
            //        // cname = ""
            //        // End If
            //        // ElseIf Right$(cname, 6) = "属性有効付加" Then
            //        // If SpecialEffectImmune("有" & Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Immune(Left$(cname, Len(cname) - 6)) _
            //        // '                                        Then
            //        // cname = ""
            //        // End If
            //        // ElseIf Right$(cname, 6) = "属性使用不能" Then
            //        // If SpecialEffectImmune("剋" & Left$(cname, Len(cname) - 6)) Then
            //        // cname = ""
            //        // End If
            //        // End If
            //        // End If
            //        // End Select
            //        // 
            //        // If cname <> "" Then
            //        // If .IsFeatureLevelSpecified(i) Then
            //        // AddCondition cname, .FeatureLevel(i)
            //        // Else
            //        // AddCondition cname, 10000
            //        // End If
            //        // End If
            //        // End Select
            //        // Next
            //        // End With
            //        // 格納されてない場合は地形による各種修正＆状態付加
            //        // MOD START 240a
            //        // Set td = TDList.Item(MapData(X, Y, 0))
            //        // レイヤーの状態に応じて上層下層どちらを取得するか判別
            //        switch (Map.MapData[x, y, Map.MapDataIndex.BoxType])
            //        {
            //            case (short)Map.BoxTypes.Under:
            //            case (short)Map.BoxTypes.UpperBmpOnly:
            //                {
            //                    td = SRC.TDList.Item(Map.MapData[x, y, Map.MapDataIndex.TerrainType]);
            //                    break;
            //                }

            //            default:
            //                {
            //                    td = SRC.TDList.Item(Map.MapData[x, y, Map.MapDataIndex.LayerType]);
            //                    break;
            //                }
            //        }
            //        // MOD START 240a
            //        // 地形効果が適用される位置にいるかを判定
            //        string argfname9 = "効果範囲";
            //        if (td.IsFeatureAvailable(ref argfname9))
            //        {
            //            object argIndex28 = "効果範囲";
            //            string arglist6 = td.FeatureData(ref argIndex28);
            //            var loopTo10 = GeneralLib.LLength(ref arglist6);
            //            for (i = 1; i <= loopTo10; i++)
            //            {
            //                string localLIndex5() { object argIndex1 = "効果範囲"; string arglist = td.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, i); return ret; }

            //                if ((Area ?? "") == (localLIndex5() ?? ""))
            //                {
            //                    is_terrain_effective = true;
            //                    break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            is_terrain_effective = true;
            //        }

            //        // 地形効果に対する無効化能力を持っているか
            //        string argfname10 = "地形効果無効化";
            //        string argsname11 = "地形効果無効化";
            //        if (IsFeatureAvailable(ref argfname10))
            //        {
            //            object argIndex30 = "地形効果無効化";
            //            string arglist8 = FeatureData(ref argIndex30);
            //            if (GeneralLib.LLength(ref arglist8) > 1)
            //            {
            //                object argIndex29 = "地形効果無効化";
            //                string arglist7 = FeatureData(ref argIndex29);
            //                var loopTo11 = GeneralLib.LLength(ref arglist7);
            //                for (i = 2; i <= loopTo11; i++)
            //                {
            //                    string localLIndex6() { object argIndex1 = "地形効果無効化"; string arglist = FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, i); return ret; }

            //                    if ((td.Name ?? "") == (localLIndex6() ?? ""))
            //                    {
            //                        is_immune_to_terrain_effect = true;
            //                        break;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                is_immune_to_terrain_effect = true;
            //            }
            //        }
            //        else if (IsSpecialPowerInEffect(ref argsname11))
            //        {
            //            is_immune_to_terrain_effect = true;
            //        }

            //        // 地形効果を適用
            //        if (is_terrain_effective)
            //        {
            //            var loopTo12 = td.CountFeature();
            //            for (i = 1; i <= loopTo12; i++)
            //            {
            //                object argIndex32 = "回復不能";
            //                string argsname12 = "回復不能";
            //                if (!IsConditionSatisfied(ref argIndex32) & !IsSpecialPowerInEffect(ref argsname12))
            //                {
            //                    object argIndex31 = i;
            //                    switch (td.Feature(ref argIndex31) ?? "")
            //                    {
            //                        case "ＨＰ回復":
            //                            {
            //                                double localFeatureLevel() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                hp_recovery = (int)(hp_recovery + 10d * localFeatureLevel());
            //                                break;
            //                            }

            //                        case "ＥＮ回復":
            //                            {
            //                                double localFeatureLevel1() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                en_recovery = (int)(en_recovery + 10d * localFeatureLevel1());
            //                                break;
            //                            }

            //                        case "ＨＰ増加":
            //                            {
            //                                double localFeatureLevel2() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                hp_up = (int)(hp_up + 1000d * localFeatureLevel2());
            //                                break;
            //                            }

            //                        case "ＥＮ増加":
            //                            {
            //                                double localFeatureLevel3() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                en_up = (int)(en_up + 10d * localFeatureLevel3());
            //                                break;
            //                            }
            //                    }
            //                }

            //                if (!is_immune_to_terrain_effect)
            //                {
            //                    object argIndex35 = i;
            //                    switch (td.Feature(ref argIndex35) ?? "")
            //                    {
            //                        case "ＨＰ減少":
            //                            {
            //                                string localFeatureData() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                string localFeatureData1() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localEffective() { string arganame = hs910888dc06c349a7b21a4df1e60d8c7a(); var ret = Effective(ref arganame); return ret; }

            //                                string localFeatureData2() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localAbsorb() { string arganame = hs52033841100047b99c1279b816aeef70(); var ret = Absorb(ref arganame); return ret; }

            //                                string localFeatureData3() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localResist() { string arganame = hsc6f55b713dcb4a289541bb10d274fe96(); var ret = Resist(ref arganame); return ret; }

            //                                string localFeatureData4() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localImmune() { string arganame = hs7f2e711ffdc34440b08ea8add7c269ee(); var ret = Immune(ref arganame); return ret; }

            //                                string arganame5 = localFeatureData();
            //                                if (Weakness(ref arganame5))
            //                                {
            //                                    double localFeatureLevel4() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 20d * localFeatureLevel4());
            //                                }
            //                                else if (localEffective())
            //                                {
            //                                    double localFeatureLevel5() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 10d * localFeatureLevel5());
            //                                }
            //                                else if (localAbsorb())
            //                                {
            //                                    double localFeatureLevel6() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery + 10d * localFeatureLevel6());
            //                                }
            //                                else if (localResist())
            //                                {
            //                                    double localFeatureLevel7() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 5d * localFeatureLevel7());
            //                                }
            //                                else if (!localImmune())
            //                                {
            //                                    double localFeatureLevel8() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 10d * localFeatureLevel8());
            //                                }

            //                                break;
            //                            }

            //                        case "ＥＮ減少":
            //                            {
            //                                string localFeatureData5() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                string localFeatureData6() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localEffective1() { string arganame = hs3c05c0f0f5c74d50b2ebe668e4cefb50(); var ret = Effective(ref arganame); return ret; }

            //                                string localFeatureData7() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localAbsorb1() { string arganame = hsf2d45f6c0d3242c088a863c924424bdc(); var ret = Absorb(ref arganame); return ret; }

            //                                string localFeatureData8() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localResist1() { string arganame = hs2feadcdb67784d8bb37649df03e02004(); var ret = Resist(ref arganame); return ret; }

            //                                string localFeatureData9() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localImmune1() { string arganame = hs2ea0185e20964cc686bdf2ee374b1db5(); var ret = Immune(ref arganame); return ret; }

            //                                string arganame6 = localFeatureData5();
            //                                if (Weakness(ref arganame6))
            //                                {
            //                                    double localFeatureLevel9() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 20d * localFeatureLevel9());
            //                                }
            //                                else if (localEffective1())
            //                                {
            //                                    double localFeatureLevel10() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 10d * localFeatureLevel10());
            //                                }
            //                                else if (localAbsorb1())
            //                                {
            //                                    double localFeatureLevel11() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery + 10d * localFeatureLevel11());
            //                                }
            //                                else if (localResist1())
            //                                {
            //                                    double localFeatureLevel12() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 5d * localFeatureLevel12());
            //                                }
            //                                else if (!localImmune1())
            //                                {
            //                                    double localFeatureLevel13() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 10d * localFeatureLevel13());
            //                                }

            //                                break;
            //                            }

            //                        case "ＨＰ低下":
            //                            {
            //                                string localFeatureData10() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                string localFeatureData11() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localEffective2() { string arganame = hsde07f92cead745858b4d820263f53c91(); var ret = Effective(ref arganame); return ret; }

            //                                string localFeatureData12() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localAbsorb2() { string arganame = hs94c6cc3a1d3d4ca68ebec987f086a962(); var ret = Absorb(ref arganame); return ret; }

            //                                string localFeatureData13() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localResist2() { string arganame = hs736b9f12f5c24c189d82474b29c54514(); var ret = Resist(ref arganame); return ret; }

            //                                string localFeatureData14() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localImmune2() { string arganame = hs125e0e0f0c7f4295afab6bc4e0bdd8b7(); var ret = Immune(ref arganame); return ret; }

            //                                string arganame7 = localFeatureData10();
            //                                if (Weakness(ref arganame7))
            //                                {
            //                                    double localFeatureLevel14() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 2000d * localFeatureLevel14());
            //                                }
            //                                else if (localEffective2())
            //                                {
            //                                    double localFeatureLevel15() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 1000d * localFeatureLevel15());
            //                                }
            //                                else if (localAbsorb2())
            //                                {
            //                                    double localFeatureLevel16() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up + 1000d * localFeatureLevel16());
            //                                }
            //                                else if (localResist2())
            //                                {
            //                                    double localFeatureLevel17() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 500d * localFeatureLevel17());
            //                                }
            //                                else if (!localImmune2())
            //                                {
            //                                    double localFeatureLevel18() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 1000d * localFeatureLevel18());
            //                                }

            //                                break;
            //                            }

            //                        case "ＥＮ低下":
            //                            {
            //                                string localFeatureData15() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                string localFeatureData16() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localEffective3() { string arganame = hs19293752f23e4276b0cbef58767f8358(); var ret = Effective(ref arganame); return ret; }

            //                                string localFeatureData17() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localAbsorb3() { string arganame = hs5c8089f996604cf3acee553527084f88(); var ret = Absorb(ref arganame); return ret; }

            //                                string localFeatureData18() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localResist3() { string arganame = hs7282195c4a7b4004aadd6adbd33a9a60(); var ret = Resist(ref arganame); return ret; }

            //                                string localFeatureData19() { object argIndex1 = i; var ret = td.FeatureData(ref argIndex1); return ret; }

            //                                bool localImmune3() { string arganame = hs18e2d5e0cbb14782bbf344a497c3c2ed(); var ret = Immune(ref arganame); return ret; }

            //                                string arganame8 = localFeatureData15();
            //                                if (Weakness(ref arganame8))
            //                                {
            //                                    double localFeatureLevel19() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 20d * localFeatureLevel19());
            //                                }
            //                                else if (localEffective3())
            //                                {
            //                                    double localFeatureLevel20() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 10d * localFeatureLevel20());
            //                                }
            //                                else if (localAbsorb3())
            //                                {
            //                                    double localFeatureLevel21() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_up = (int)(en_up + 10d * localFeatureLevel21());
            //                                }
            //                                else if (localResist3())
            //                                {
            //                                    double localFeatureLevel22() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 5d * localFeatureLevel22());
            //                                }
            //                                else if (!localImmune3())
            //                                {
            //                                    double localFeatureLevel23() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 10d * localFeatureLevel23());
            //                                }

            //                                break;
            //                            }

            //                        case "状態付加":
            //                            {
            //                                object argIndex33 = i;
            //                                cname = td.FeatureData(ref argIndex33);

            //                                // 状態が無効化されるかチェック
            //                                switch (cname ?? "")
            //                                {
            //                                    case "装甲劣化":
            //                                        {
            //                                            string arganame9 = "劣";
            //                                            if (SpecialEffectImmune(ref arganame9))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "混乱":
            //                                        {
            //                                            string arganame10 = "乱";
            //                                            if (SpecialEffectImmune(ref arganame10))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "恐怖":
            //                                        {
            //                                            string arganame11 = "恐";
            //                                            if (SpecialEffectImmune(ref arganame11))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "踊り":
            //                                        {
            //                                            string arganame12 = "踊";
            //                                            if (SpecialEffectImmune(ref arganame12))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "狂戦士":
            //                                        {
            //                                            string arganame13 = "狂";
            //                                            if (SpecialEffectImmune(ref arganame13))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "ゾンビ":
            //                                        {
            //                                            string arganame14 = "ゾ";
            //                                            if (SpecialEffectImmune(ref arganame14))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "回復不能":
            //                                        {
            //                                            string arganame15 = "害";
            //                                            if (SpecialEffectImmune(ref arganame15))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "石化":
            //                                        {
            //                                            string arganame16 = "石";
            //                                            if (SpecialEffectImmune(ref arganame16))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "凍結":
            //                                        {
            //                                            string arganame17 = "凍";
            //                                            if (SpecialEffectImmune(ref arganame17))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "麻痺":
            //                                        {
            //                                            string arganame18 = "痺";
            //                                            if (SpecialEffectImmune(ref arganame18))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "睡眠":
            //                                        {
            //                                            string arganame19 = "眠";
            //                                            if (SpecialEffectImmune(ref arganame19))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "毒":
            //                                        {
            //                                            string arganame20 = "毒";
            //                                            if (SpecialEffectImmune(ref arganame20))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "盲目":
            //                                        {
            //                                            string arganame21 = "盲";
            //                                            if (SpecialEffectImmune(ref arganame21))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "沈黙":
            //                                        {
            //                                            string arganame22 = "黙";
            //                                            if (SpecialEffectImmune(ref arganame22))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }
            //                                    // 属性使用不能状態
            //                                    case "オーラ使用不能":
            //                                        {
            //                                            string arganame23 = "剋オ";
            //                                            if (SpecialEffectImmune(ref arganame23))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "超能力使用不能":
            //                                        {
            //                                            string arganame24 = "剋超";
            //                                            if (SpecialEffectImmune(ref arganame24))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "同調率使用不能":
            //                                        {
            //                                            string arganame25 = "剋シ";
            //                                            if (SpecialEffectImmune(ref arganame25))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "超感覚使用不能":
            //                                        {
            //                                            string arganame26 = "剋サ";
            //                                            if (SpecialEffectImmune(ref arganame26))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "知覚強化使用不能":
            //                                        {
            //                                            string arganame27 = "剋サ";
            //                                            if (SpecialEffectImmune(ref arganame27))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力使用不能":
            //                                        {
            //                                            string arganame28 = "剋霊";
            //                                            if (SpecialEffectImmune(ref arganame28))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "術使用不能":
            //                                        {
            //                                            string arganame29 = "剋術";
            //                                            if (SpecialEffectImmune(ref arganame29))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "技使用不能":
            //                                        {
            //                                            string arganame30 = "剋技";
            //                                            if (SpecialEffectImmune(ref arganame30))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (Strings.Len(cname) > 6)
            //                                            {
            //                                                if (Strings.Right(cname, 6) == "属性弱点付加")
            //                                                {
            //                                                    bool localSpecialEffectImmune() { string arganame = "弱" + Strings.Left(cname, Strings.Len(cname) - 6); var ret = SpecialEffectImmune(ref arganame); return ret; }

            //                                                    bool localAbsorb4() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Absorb(ref arganame); return ret; }

            //                                                    bool localImmune4() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Immune(ref arganame); return ret; }

            //                                                    if (localSpecialEffectImmune() | localAbsorb4() | localImmune4())
            //                                                    {
            //                                                        cname = "";
            //                                                    }
            //                                                }
            //                                                else if (Strings.Right(cname, 6) == "属性有効付加")
            //                                                {
            //                                                    bool localSpecialEffectImmune1() { string arganame = "有" + Strings.Left(cname, Strings.Len(cname) - 6); var ret = SpecialEffectImmune(ref arganame); return ret; }

            //                                                    bool localAbsorb5() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Absorb(ref arganame); return ret; }

            //                                                    bool localImmune5() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Immune(ref arganame); return ret; }

            //                                                    if (localSpecialEffectImmune1() | localAbsorb5() | localImmune5())
            //                                                    {
            //                                                        cname = "";
            //                                                    }
            //                                                }
            //                                                else if (Strings.Right(cname, 6) == "属性使用不能")
            //                                                {
            //                                                    string arganame31 = "剋" + Strings.Left(cname, Strings.Len(cname) - 6);
            //                                                    if (SpecialEffectImmune(ref arganame31))
            //                                                    {
            //                                                        cname = "";
            //                                                    }
            //                                                }
            //                                            }

            //                                            break;
            //                                        }
            //                                }

            //                                if (!string.IsNullOrEmpty(cname))
            //                                {
            //                                    object argIndex34 = i;
            //                                    if (td.IsFeatureLevelSpecified(ref argIndex34))
            //                                    {
            //                                        double localFeatureLevel24() { object argIndex1 = i; var ret = td.FeatureLevel(ref argIndex1); return ret; }

            //                                        string argcdata1 = "";
            //                                        AddCondition(ref cname, (short)localFeatureLevel24(), cdata: ref argcdata1);
            //                                    }
            //                                    else
            //                                    {
            //                                        string argcdata2 = "";
            //                                        AddCondition(ref cname, 10000, cdata: ref argcdata2);
            //                                    }
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //        }
            //        // MOD END MARGE
            //    }

            //    // ＥＮは毎ターン5回復
            //    // MOD START MARGE
            //    // If Not IsConditionSatisfied("回復不能") _
            //    // '        And Not IsOptionDefined("ＥＮ自然回復無効") _
            //    // '    Then
            //    object argIndex36 = "回復不能";
            //    string argsname13 = "回復不能";
            //    string argoname1 = "ＥＮ自然回復無効";
            //    if (!IsConditionSatisfied(ref argIndex36) & !IsSpecialPowerInEffect(ref argsname13) & !Expression.IsOptionDefined(ref argoname1))
            //    {
            //        // MOD END MARGE
            //        EN = EN + 5;
            //    }

            //    // 算出した回復率を使ってＨＰを回復
            //    HP = HP + MaxHP * hp_recovery / 100 + hp_up;
            //    if (HP <= 0)
            //    {
            //        HP = 1;
            //    }

            //    // 特殊能力「不安定」による暴走チェック
            //    string argfname11 = "不安定";
            //    if (IsFeatureAvailable(ref argfname11))
            //    {
            //        object argIndex37 = "暴走";
            //        if (HP <= MaxHP / 4 & !IsConditionSatisfied(ref argIndex37))
            //        {
            //            string argcname1 = "暴走";
            //            string argcdata3 = "";
            //            AddCondition(ref argcname1, -1, cdata: ref argcdata3);
            //        }
            //    }

            //    // 算出した回復率を使ってＥＮを回復
            //    if (EN + MaxEN * en_recovery / 100 + en_up > 0)
            //    {
            //        EN = EN + MaxEN * en_recovery / 100 + en_up;
            //    }
            //    else
            //    {
            //        // ＥＮが減少して０になる場合はハイパーモード解除もしくは行動不能
            //        string argfname13 = "ノーマルモード";
            //        string argfname14 = "変形";
            //        if (IsFeatureAvailable(ref argfname13))
            //        {
            //            // ただしノーマルモードに戻れない地形だとそのまま退却……
            //            string localLIndex8() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

            //            Unit localOtherForm() { object argIndex1 = (object)hs35732b48c6874150a97edd31eb6e765a(); var ret = OtherForm(ref argIndex1); return ret; }

            //            if (localOtherForm().IsAbleToEnter(x, y))
            //            {
            //                string localLIndex7() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

            //                string argnew_form = localLIndex7();
            //                Transform(ref argnew_form);
            //            }
            //            else
            //            {
            //                GUI.Center(x, y);
            //                Escape();
            //                Unit argu15 = null;
            //                Unit argu25 = null;
            //                GUI.OpenMessageForm(u1: ref argu15, u2: ref argu25);
            //                GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                GUI.CloseMessageForm();
            //                Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                return;
            //            }
            //        }
            //        else if (IsFeatureAvailable(ref argfname14))
            //        {
            //            // 変形できれば変形
            //            object argIndex38 = "変形";
            //            buf = FeatureData(ref argIndex38);
            //            var loopTo13 = GeneralLib.LLength(ref buf);
            //            for (i = 2; i <= loopTo13; i++)
            //            {
            //                object argIndex39 = GeneralLib.LIndex(ref buf, i);
            //                {
            //                    var withBlock9 = OtherForm(ref argIndex39);
            //                    string argfname12 = "ＥＮ消費";
            //                    if (withBlock9.IsAbleToEnter(x, y) & !withBlock9.IsFeatureAvailable(ref argfname12))
            //                    {
            //                        string argnew_form1 = GeneralLib.LIndex(ref buf, i);
            //                        Transform(ref argnew_form1);
            //                        break;
            //                    }
            //                }
            //            }

            //            if (i > GeneralLib.LLength(ref buf))
            //            {
            //                EN = 0;
            //            }
            //        }
            //        else
            //        {
            //            EN = 0;
            //        }
            //    }

            // データ更新
            Update();

            //    // 時間切れ？
            //    if (is_time_limit)
            //    {
            //        string argfname17 = "分離";
            //        if (!string.IsNullOrEmpty(next_form))
            //        {
            //            // ハイパーモード＆変身の時間切れ
            //            object argIndex40 = next_form;
            //            u = OtherForm(ref argIndex40);
            //            if (u.IsAbleToEnter(x, y))
            //            {
            //                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //                string argfname15 = "追加パイロット";
            //                if (u.IsFeatureAvailable(ref argfname15))
            //                {
            //                    bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

            //                    if (!localIsDefined())
            //                    {
            //                        object argIndex41 = "追加パイロット";
            //                        string argpname = u.FeatureData(ref argIndex41);
            //                        string argpparty = Party0;
            //                        string arggid = "";
            //                        SRC.PList.Add(ref argpname, MainPilot().Level, ref argpparty, gid: ref arggid);
            //                        this.Party0 = argpparty;
            //                    }
            //                }

            //                // ノーマルモードメッセージ
            //                bool localIsMessageDefined() { string argmain_situation = "ノーマルモード(" + u.Name + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //                string argmain_situation = "ノーマルモード(" + Name + "=>" + u.Name + ")";
            //                string argmain_situation1 = "ノーマルモード";
            //                if (IsMessageDefined(ref argmain_situation))
            //                {
            //                    Unit argu16 = null;
            //                    Unit argu26 = null;
            //                    GUI.OpenMessageForm(u1: ref argu16, u2: ref argu26);
            //                    string argSituation = "ノーマルモード(" + Name + "=>" + u.Name + ")";
            //                    string argmsg_mode = "";
            //                    PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
            //                    GUI.CloseMessageForm();
            //                }
            //                else if (localIsMessageDefined())
            //                {
            //                    Unit argu17 = null;
            //                    Unit argu27 = null;
            //                    GUI.OpenMessageForm(u1: ref argu17, u2: ref argu27);
            //                    string argSituation1 = "ノーマルモード(" + u.Name + ")";
            //                    string argmsg_mode1 = "";
            //                    PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
            //                    GUI.CloseMessageForm();
            //                }
            //                else if (IsMessageDefined(ref argmain_situation1))
            //                {
            //                    Unit argu18 = null;
            //                    Unit argu28 = null;
            //                    GUI.OpenMessageForm(u1: ref argu18, u2: ref argu28);
            //                    string argSituation2 = "ノーマルモード";
            //                    string argmsg_mode2 = "";
            //                    PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
            //                    GUI.CloseMessageForm();
            //                }
            //                // 特殊効果
            //                bool localIsSpecialEffectDefined() { string argmain_situation = "ノーマルモード(" + u.Name + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //                string argmain_situation5 = "ノーマルモード(" + Name + "=>" + u.Name + ")";
            //                string argsub_situation3 = "";
            //                string argmain_situation6 = "ノーマルモード";
            //                string argsub_situation4 = "";
            //                if (IsSpecialEffectDefined(ref argmain_situation5, sub_situation: ref argsub_situation3))
            //                {
            //                    string argmain_situation2 = "ノーマルモード(" + Name + "=>" + u.Name + ")";
            //                    string argsub_situation = "";
            //                    SpecialEffect(ref argmain_situation2, sub_situation: ref argsub_situation);
            //                }
            //                else if (localIsSpecialEffectDefined())
            //                {
            //                    string argmain_situation3 = "ノーマルモード(" + u.Name + ")";
            //                    string argsub_situation1 = "";
            //                    SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
            //                }
            //                else if (IsSpecialEffectDefined(ref argmain_situation6, sub_situation: ref argsub_situation4))
            //                {
            //                    string argmain_situation4 = "ノーマルモード";
            //                    string argsub_situation2 = "";
            //                    SpecialEffect(ref argmain_situation4, sub_situation: ref argsub_situation2);
            //                }

            //                // 変形
            //                string localLIndex9() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

            //                string argnew_form2 = localLIndex9();
            //                Transform(ref argnew_form2);
            //            }
            //            else
            //            {
            //                // 変形するとその地形にいれなくなる場合は退却
            //                GUI.Center(x, y);
            //                Escape();
            //                Unit argu19 = null;
            //                Unit argu29 = null;
            //                GUI.OpenMessageForm(u1: ref argu19, u2: ref argu29);
            //                GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                GUI.CloseMessageForm();
            //                Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //                return;
            //            }
            //        }
            //        else if (IsFeatureAvailable(ref argfname17))
            //        {
            //            // 合体時間切れ

            //            // メッセージ表示
            //            bool localIsMessageDefined2() { string argmain_situation = "分離(" + Name + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //            bool localIsMessageDefined3() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(ref argIndex1) + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //            string argmain_situation8 = "分離";
            //            if (localIsMessageDefined2() | localIsMessageDefined3() | IsMessageDefined(ref argmain_situation8))
            //            {
            //                string argfname16 = "分離ＢＧＭ";
            //                if (IsFeatureAvailable(ref argfname16))
            //                {
            //                    object argIndex42 = "分離ＢＧＭ";
            //                    string argbgm_name = FeatureData(ref argIndex42);
            //                    Sound.StartBGM(ref argbgm_name);
            //                    GUI.Sleep(500);
            //                }
            //                else if (MainPilot().BGM != "-")
            //                {
            //                    string argbgm_name1 = MainPilot().BGM;
            //                    Sound.StartBGM(ref argbgm_name1);
            //                    MainPilot().BGM = argbgm_name1;
            //                    GUI.Sleep(500);
            //                }

            //                GUI.Center(x, y);
            //                GUI.RefreshScreen();
            //                Unit argu111 = null;
            //                Unit argu211 = null;
            //                GUI.OpenMessageForm(u1: ref argu111, u2: ref argu211);
            //                bool localIsMessageDefined1() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(ref argIndex1) + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //                string argmain_situation7 = "分離(" + Name + ")";
            //                if (IsMessageDefined(ref argmain_situation7))
            //                {
            //                    string argSituation3 = "分離(" + Name + ")";
            //                    string argmsg_mode3 = "";
            //                    PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
            //                }
            //                else if (localIsMessageDefined1())
            //                {
            //                    object argIndex43 = "分離";
            //                    string argSituation5 = "分離(" + FeatureName(ref argIndex43) + ")";
            //                    string argmsg_mode5 = "";
            //                    PilotMessage(ref argSituation5, msg_mode: ref argmsg_mode5);
            //                }
            //                else
            //                {
            //                    string argSituation4 = "分離";
            //                    string argmsg_mode4 = "";
            //                    PilotMessage(ref argSituation4, msg_mode: ref argmsg_mode4);
            //                }

            //                GUI.CloseMessageForm();
            //            }
            //            // 特殊効果
            //            bool localIsSpecialEffectDefined1() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //            string argmain_situation12 = "分離(" + Name + ")";
            //            string argsub_situation8 = "";
            //            if (IsSpecialEffectDefined(ref argmain_situation12, sub_situation: ref argsub_situation8))
            //            {
            //                string argmain_situation9 = "分離(" + Name + ")";
            //                string argsub_situation5 = "";
            //                SpecialEffect(ref argmain_situation9, sub_situation: ref argsub_situation5);
            //            }
            //            else if (localIsSpecialEffectDefined1())
            //            {
            //                object argIndex44 = "分離";
            //                string argmain_situation11 = "分離(" + FeatureName(ref argIndex44) + ")";
            //                string argsub_situation7 = "";
            //                SpecialEffect(ref argmain_situation11, sub_situation: ref argsub_situation7);
            //            }
            //            else
            //            {
            //                string argmain_situation10 = "分離";
            //                string argsub_situation6 = "";
            //                SpecialEffect(ref argmain_situation10, sub_situation: ref argsub_situation6);
            //            }

            //            // 分離
            //            Split_Renamed();
            //        }
            //        else
            //        {
            //            // 制限時間切れ
            //            GUI.Center(x, y);
            //            GUI.RefreshScreen();
            //            Unit argu110 = null;
            //            Unit argu210 = null;
            //            GUI.OpenMessageForm(u1: ref argu110, u2: ref argu210);
            //            GUI.DisplaySysMessage(Nickname + "は制限時間切れのため退却します。");
            //            GUI.CloseMessageForm();
            //            Escape();
            //            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
            //            return;
            //        }
            //    }

            //    // ハイパーモード＆ノーマルモードの自動発動をチェック
            //    CurrentForm().CheckAutoHyperMode();
            //    CurrentForm().CheckAutoNormalMode();
        }

        // ハイパーモードの自動発動チェック
        public void CheckAutoHyperMode()
        {
            //    bool is_available, message_window_visible = default;
            //    string fname, fdata;
            //    double flevel;
            //    string uname;
            //    short i;

            //    // ハイパーモードが自動発動するか判定

            //    if (Status_Renamed != "出撃")
            //    {
            //        return;
            //    }

            //    string argfname = "ハイパーモード";
            //    if (!IsFeatureAvailable(ref argfname))
            //    {
            //        return;
            //    }

            //    object argIndex1 = "ハイパーモード";
            //    fname = FeatureName(ref argIndex1);
            //    object argIndex2 = "ハイパーモード";
            //    flevel = FeatureLevel(ref argIndex2);
            //    object argIndex3 = "ハイパーモード";
            //    fdata = FeatureData(ref argIndex3);
            //    if (Strings.InStr(fdata, "自動発動") == 0)
            //    {
            //        return;
            //    }

            //    // 発動条件を満たす？
            //    if (this.MainPilot().Morale < (short)(10d * flevel) + 100 & (HP > MaxHP / 4 | Strings.InStr(fdata, "気力発動") > 0))
            //    {
            //        return;
            //    }

            //    // 変身中・能力コピー中はハイパーモードを使用できない
            //    object argIndex4 = "ノーマルモード付加";
            //    if (IsConditionSatisfied(ref argIndex4))
            //    {
            //        return;
            //    }

            //    // ハイパーモード先の形態が利用可能？
            //    uname = GeneralLib.LIndex(ref fdata, 2);
            //    is_available = false;
            //    object argIndex5 = uname;
            //    {
            //        var withBlock = OtherForm(ref argIndex5);
            //        switch (Map.TerrainClass(x, y) ?? "")
            //        {
            //            case "空":
            //                {
            //                    string argarea_name = "空";
            //                    if (withBlock.IsTransAvailable(ref argarea_name))
            //                    {
            //                        is_available = true;
            //                    }

            //                    break;
            //                }

            //            case "深水":
            //                {
            //                    string argarea_name1 = "空";
            //                    string argarea_name2 = "水";
            //                    string argarea_name3 = "水上";
            //                    if (withBlock.IsTransAvailable(ref argarea_name1) | withBlock.IsTransAvailable(ref argarea_name2) | withBlock.IsTransAvailable(ref argarea_name3))
            //                    {
            //                        is_available = true;
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    is_available = true;
            //                    break;
            //                }
            //        }

            //        if (!withBlock.IsAbleToEnter(x, y))
            //        {
            //            is_available = false;
            //        }
            //    }

            //    // 自動発動する？
            //    if (!is_available)
            //    {
            //        return;
            //    }

            //    // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //    object argIndex8 = uname;
            //    if (SRC.UDList.IsDefined(ref argIndex8))
            //    {
            //        object argIndex7 = uname;
            //        {
            //            var withBlock1 = SRC.UDList.Item(ref argIndex7);
            //            string argfname1 = "追加パイロット";
            //            if (IsFeatureAvailable(ref argfname1))
            //            {
            //                bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

            //                if (!localIsDefined())
            //                {
            //                    object argIndex6 = "追加パイロット";
            //                    string argpname = FeatureData(ref argIndex6);
            //                    string argpparty = Party0;
            //                    string arggid = "";
            //                    SRC.PList.Add(ref argpname, MainPilot().Level, ref argpparty, gid: ref arggid);
            //                    this.Party0 = argpparty;
            //                }
            //            }
            //        }
            //    }

            //    // ＢＧＭを切り替え
            //    string argfname2 = "ハイパーモードＢＧＭ";
            //    if (IsFeatureAvailable(ref argfname2))
            //    {
            //        var loopTo = CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            string localFeature() { object argIndex1 = i; var ret = Feature(ref argIndex1); return ret; }

            //            string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //            string localLIndex() { string arglist = hs2eb6d7953f284c87970de1e6c7ca058d(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

            //            if (localFeature() == "ハイパーモードＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
            //            {
            //                string localFeatureData() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //                string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //                string argbgm_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
            //                Sound.StartBGM(ref argbgm_name);
            //                GUI.Sleep(500);
            //                break;
            //            }
            //        }
            //    }

            //    // メッセージを表示
            //    bool localIsMessageDefined2() { string argmain_situation = "ハイパーモード(" + Name + "=>" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //    bool localIsMessageDefined3() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //    bool localIsMessageDefined4() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //    string argmain_situation1 = "ハイパーモード";
            //    if (localIsMessageDefined2() | localIsMessageDefined3() | localIsMessageDefined4() | IsMessageDefined(ref argmain_situation1))
            //    {
            //        GUI.Center(x, y);
            //        GUI.RefreshScreen();
            //        if (!message_window_visible)
            //        {
            //            Unit argu1 = null;
            //            Unit argu2 = null;
            //            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            //        }
            //        else
            //        {
            //            message_window_visible = true;
            //        }

            //        // メッセージを表示
            //        bool localIsMessageDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //        bool localIsMessageDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //        string argmain_situation = "ハイパーモード(" + Name + "=>" + uname + ")";
            //        if (IsMessageDefined(ref argmain_situation))
            //        {
            //            string argSituation = "ハイパーモード(" + Name + "=>" + uname + ")";
            //            string argmsg_mode = "";
            //            PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
            //        }
            //        else if (localIsMessageDefined())
            //        {
            //            string argSituation2 = "ハイパーモード(" + uname + ")";
            //            string argmsg_mode2 = "";
            //            PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
            //        }
            //        else if (localIsMessageDefined1())
            //        {
            //            string argSituation3 = "ハイパーモード(" + fname + ")";
            //            string argmsg_mode3 = "";
            //            PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
            //        }
            //        else
            //        {
            //            string argSituation1 = "ハイパーモード";
            //            string argmsg_mode1 = "";
            //            PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
            //        }

            //        if (!message_window_visible)
            //        {
            //            GUI.CloseMessageForm();
            //        }
            //    }

            //    // 特殊効果
            //    Commands.SaveSelections();
            //    Commands.SelectedUnit = this;
            //    Event_Renamed.SelectedUnitForEvent = this;
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Event_Renamed.SelectedTargetForEvent = null;
            //    bool localIsAnimationDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsAnimationDefined1() { object argIndex1 = "ハイパーモード"; string argmain_situation = "ハイパーモード(" + FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined() { string argmain_situation = "ハイパーモード(" + Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined1() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined2() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    string argmain_situation10 = "ハイパーモード(" + Name + "=>" + uname + ")";
            //    string argsub_situation8 = "";
            //    string argmain_situation11 = "ハイパーモード";
            //    string argsub_situation9 = "";
            //    if (IsAnimationDefined(ref argmain_situation10, sub_situation: ref argsub_situation8))
            //    {
            //        string argmain_situation2 = "ハイパーモード(" + Name + "=>" + uname + ")";
            //        string argsub_situation = "";
            //        PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
            //    }
            //    else if (localIsAnimationDefined())
            //    {
            //        string argmain_situation4 = "ハイパーモード(" + uname + ")";
            //        string argsub_situation2 = "";
            //        PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
            //    }
            //    else if (localIsAnimationDefined1())
            //    {
            //        object argIndex9 = "ハイパーモード";
            //        string argmain_situation5 = "ハイパーモード(" + FeatureName(ref argIndex9) + ")";
            //        string argsub_situation3 = "";
            //        PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
            //    }
            //    else if (IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation9))
            //    {
            //        string argmain_situation6 = "ハイパーモード";
            //        string argsub_situation4 = "";
            //        PlayAnimation(ref argmain_situation6, sub_situation: ref argsub_situation4);
            //    }
            //    else if (localIsSpecialEffectDefined())
            //    {
            //        string argmain_situation7 = "ハイパーモード(" + Name + "=>" + uname + ")";
            //        string argsub_situation5 = "";
            //        SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
            //    }
            //    else if (localIsSpecialEffectDefined1())
            //    {
            //        string argmain_situation8 = "ハイパーモード(" + uname + ")";
            //        string argsub_situation6 = "";
            //        SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation6);
            //    }
            //    else if (localIsSpecialEffectDefined2())
            //    {
            //        string argmain_situation9 = "ハイパーモード(" + fname + ")";
            //        string argsub_situation7 = "";
            //        SpecialEffect(ref argmain_situation9, sub_situation: ref argsub_situation7);
            //    }
            //    else
            //    {
            //        string argmain_situation3 = "ハイパーモード";
            //        string argsub_situation1 = "";
            //        SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
            //    }

            //    Commands.RestoreSelections();

            //    // ハイパーモードに変形
            //    Transform(ref uname);

            //    // ユニット変数を置き換え
            //    if (Commands.SelectedUnit is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
            //        {
            //            Commands.SelectedUnit = CurrentForm();
            //        }
            //    }

            //    if (Event_Renamed.SelectedUnitForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event_Renamed.SelectedUnitForEvent.ID ?? ""))
            //        {
            //            Event_Renamed.SelectedUnitForEvent = CurrentForm();
            //        }
            //    }

            //    if (Commands.SelectedTarget is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
            //        {
            //            Commands.SelectedTarget = CurrentForm();
            //        }
            //    }

            //    if (Event_Renamed.SelectedTargetForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event_Renamed.SelectedTargetForEvent.ID ?? ""))
            //        {
            //            Event_Renamed.SelectedTargetForEvent = CurrentForm();
            //        }
            //    }

            //    // 変形イベント
            //    {
            //        var withBlock2 = CurrentForm();
            //        Event_Renamed.HandleEvent("変形", withBlock2.MainPilot().ID, withBlock2.Name);
            //    }
        }

        // ノーマルモードの自動発動チェック
        public bool CheckAutoNormalMode(bool without_redraw = false)
        {
            return false;
            //    bool CheckAutoNormalModeRet = default;
            //    var message_window_visible = default(bool);
            //    string uname;
            //    short i;

            //    // ノーマルモードが自動発動するか判定

            //    if (Status_Renamed != "出撃")
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    string argfname = "ノーマルモード";
            //    if (!IsFeatureAvailable(ref argfname))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // まだ元の形態でもＯＫ？
            //    if (IsAbleToEnter(x, y))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // ノーマルモード先が利用可能？
            //    object argIndex1 = "ノーマルモード";
            //    string arglist = FeatureData(ref argIndex1);
            //    uname = GeneralLib.LIndex(ref arglist, 1);
            //    Unit localOtherForm() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

            //    if (!localOtherForm().IsAbleToEnter(x, y))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //    object argIndex4 = uname;
            //    if (SRC.UDList.IsDefined(ref argIndex4))
            //    {
            //        object argIndex3 = uname;
            //        {
            //            var withBlock = SRC.UDList.Item(ref argIndex3);
            //            string argfname1 = "追加パイロット";
            //            if (IsFeatureAvailable(ref argfname1))
            //            {
            //                bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

            //                if (!localIsDefined())
            //                {
            //                    object argIndex2 = "追加パイロット";
            //                    string argpname = FeatureData(ref argIndex2);
            //                    string argpparty = Party0;
            //                    string arggid = "";
            //                    SRC.PList.Add(ref argpname, MainPilot().Level, ref argpparty, gid: ref arggid);
            //                    this.Party0 = argpparty;
            //                }
            //            }
            //        }
            //    }

            //    // メッセージを表示
            //    bool localIsMessageDefined1() { string argmain_situation = "ノーマルモード(" + Name + "=>" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //    bool localIsMessageDefined2() { string argmain_situation = "ノーマルモード(" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //    string argmain_situation1 = "ノーマルモード";
            //    if (localIsMessageDefined1() | localIsMessageDefined2() | IsMessageDefined(ref argmain_situation1))
            //    {
            //        // ＢＧＭを切り替え
            //        string argfname2 = "ノーマルモードＢＧＭ";
            //        if (IsFeatureAvailable(ref argfname2))
            //        {
            //            var loopTo = CountFeature();
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                string localFeature() { object argIndex1 = i; var ret = Feature(ref argIndex1); return ret; }

            //                string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //                string localLIndex() { string arglist = hsbe3464a93f8d41b68a3072a6446fded5(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

            //                if (localFeature() == "ノーマルモードＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //                    string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

            //                    string argbgm_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
            //                    Sound.StartBGM(ref argbgm_name);
            //                    GUI.Sleep(500);
            //                    break;
            //                }
            //            }
            //        }

            //        GUI.Center(x, y);
            //        GUI.RefreshScreen();
            //        if (!message_window_visible)
            //        {
            //            Unit argu1 = null;
            //            Unit argu2 = null;
            //            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            //        }
            //        else
            //        {
            //            message_window_visible = true;
            //        }

            //        // メッセージを表示
            //        bool localIsMessageDefined() { string argmain_situation = "ノーマルモード(" + uname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

            //        string argmain_situation = "ノーマルモード(" + Name + "=>" + uname + ")";
            //        if (IsMessageDefined(ref argmain_situation))
            //        {
            //            string argSituation = "ノーマルモード(" + Name + "=>" + uname + ")";
            //            string argmsg_mode = "";
            //            PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
            //        }
            //        else if (localIsMessageDefined())
            //        {
            //            string argSituation2 = "ノーマルモード(" + uname + ")";
            //            string argmsg_mode2 = "";
            //            PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
            //        }
            //        else
            //        {
            //            string argSituation1 = "ノーマルモード";
            //            string argmsg_mode1 = "";
            //            PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
            //        }

            //        if (!message_window_visible)
            //        {
            //            GUI.CloseMessageForm();
            //        }
            //    }

            //    // 特殊効果
            //    Commands.SaveSelections();
            //    Commands.SelectedUnit = this;
            //    Event_Renamed.SelectedUnitForEvent = this;
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Event_Renamed.SelectedTargetForEvent = null;
            //    bool localIsAnimationDefined() { string argmain_situation = "ノーマルモード(" + uname + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined() { string argmain_situation = "ノーマルモード(" + Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined1() { string argmain_situation = "ノーマルモード(" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            //    string argmain_situation8 = "ノーマルモード(" + Name + "=>" + uname + ")";
            //    string argsub_situation6 = "";
            //    string argmain_situation9 = "ノーマルモード";
            //    string argsub_situation7 = "";
            //    if (IsAnimationDefined(ref argmain_situation8, sub_situation: ref argsub_situation6))
            //    {
            //        string argmain_situation2 = "ノーマルモード(" + Name + "=>" + uname + ")";
            //        string argsub_situation = "";
            //        PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
            //    }
            //    else if (localIsAnimationDefined())
            //    {
            //        string argmain_situation4 = "ノーマルモード(" + uname + ")";
            //        string argsub_situation2 = "";
            //        PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
            //    }
            //    else if (IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation7))
            //    {
            //        string argmain_situation5 = "ノーマルモード";
            //        string argsub_situation3 = "";
            //        PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
            //    }
            //    else if (localIsSpecialEffectDefined())
            //    {
            //        string argmain_situation6 = "ノーマルモード(" + Name + "=>" + uname + ")";
            //        string argsub_situation4 = "";
            //        SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation4);
            //    }
            //    else if (localIsSpecialEffectDefined1())
            //    {
            //        string argmain_situation7 = "ノーマルモード(" + uname + ")";
            //        string argsub_situation5 = "";
            //        SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
            //    }
            //    else
            //    {
            //        string argmain_situation3 = "ノーマルモード";
            //        string argsub_situation1 = "";
            //        SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
            //    }

            //    Commands.RestoreSelections();

            //    // ノーマルモードに変形
            //    Transform(ref uname);

            //    // ユニット変数を置き換え
            //    if (Commands.SelectedUnit is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
            //        {
            //            Commands.SelectedUnit = CurrentForm();
            //        }
            //    }

            //    if (Event_Renamed.SelectedUnitForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event_Renamed.SelectedUnitForEvent.ID ?? ""))
            //        {
            //            Event_Renamed.SelectedUnitForEvent = CurrentForm();
            //        }
            //    }

            //    if (Commands.SelectedTarget is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
            //        {
            //            Commands.SelectedTarget = CurrentForm();
            //        }
            //    }

            //    if (Event_Renamed.SelectedTargetForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event_Renamed.SelectedTargetForEvent.ID ?? ""))
            //        {
            //            Event_Renamed.SelectedTargetForEvent = CurrentForm();
            //        }
            //    }

            //    // 画面の再描画が必要？
            //    object argIndex5 = "消耗";
            //    if (CurrentForm().IsConditionSatisfied(ref argIndex5))
            //    {
            //        CheckAutoNormalModeRet = true;
            //        if (!without_redraw)
            //        {
            //            GUI.RedrawScreen();
            //        }
            //    }

            //    // 変形イベント
            //    {
            //        var withBlock1 = CurrentForm();
            //        Event_Renamed.HandleEvent("変形", withBlock1.MainPilot().ID, withBlock1.Name);
            //    }

            //    return CheckAutoNormalModeRet;
        }

        // データをリセット
        // UPGRADE_NOTE: Reset は Reset_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public void Reset_Renamed()
        {
            //    short i;
            //    string pname;
            //    var loopTo = CountCondition();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex1 = 1;
            //        DeleteCondition0(ref argIndex1);
            //    }

            //    RemoveAllSpecialPowerInEffect();
            Update();
            //    var loopTo1 = CountPilot();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //        localPilot().FullRecover();
            //    }

            //    var loopTo2 = CountSupport();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        Pilot localSupport() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //        localSupport().FullRecover();
            //    }

            //    string argfname = "追加パイロット";
            //    if (IsFeatureAvailable(ref argfname))
            //    {
            //        object argIndex2 = "追加パイロット";
            //        pname = FeatureData(ref argIndex2);
            //        object argIndex3 = pname;
            //        if (SRC.PList.IsDefined(ref argIndex3))
            //        {
            //            Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

            //            localItem().FullRecover();
            //        }
            //    }

            //    string argfname1 = "追加サポート";
            //    if (IsFeatureAvailable(ref argfname1))
            //    {
            //        object argIndex4 = "追加サポート";
            //        pname = FeatureData(ref argIndex4);
            //        object argIndex5 = pname;
            //        if (SRC.PList.IsDefined(ref argIndex5))
            //        {
            //            Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

            //            localItem1().FullRecover();
            //        }
            //    }

            //    HP = MaxHP;
            //    FullSupply();
            //    Mode = "通常";
        }
    }
}
