using System.Linq;

namespace SRCCore.Units
{
    // === ステータス回復関連処理 ===
    public partial class Unit
    {
        public bool CanHPRecovery => HP < MaxHP && !IsConditionSatisfied("ゾンビ");
        public bool CanFix(Unit fixer)
        {
            var canFix = CanHPRecovery;

            if (canFix && fixer != null && IsFeatureAvailable("修理不可"))
            {
                var fixFd = fixer.Feature("修理装置");
                var fixFname = fixFd?.FeatureName0(fixer) ?? "修理装置";
                var fd = Feature("修理不可");

                foreach (var fname in fd.DataL.Skip(1))
                {
                    // XXX ! 条件が複数あった時は？ そもそも修理不可Helpにも実装にもないけれどどういう存在なんだ？
                    if (fname.StartsWith("!") && fname != "!" + fixFname)
                    {
                        canFix = false;
                        break;
                    }
                    else if (fname == fixFname)
                    {
                        canFix = false;
                        break;
                    }
                }
            }

            return canFix;
        }
        public bool CanENRecovery => EN < MaxEN && !IsConditionSatisfied("ゾンビ");
        public bool CanBulletRecovery => Weapons.Any(uw => uw.Bullet() < uw.MaxBullet());
        public bool CanStockRecovery => Abilities.Any(ua => ua.Stock() < ua.MaxStock());
        public bool CanSupply => CanENRecovery || CanBulletRecovery || CanStockRecovery;

        // ステータスを全回復
        public void FullRecover()
        {
            Update();

            // パイロットのステータスを全回復
            foreach (var p in Pilots)
            {
                p.FullRecover();
            }
            foreach (var p in SupportPilots)
            {
                p.FullRecover();
            }

            if (IsFeatureAvailable("追加パイロット"))
            {
                if (SRC.PList.IsDefined(FeatureData("追加パイロット")))
                {
                    SRC.PList.Item(FeatureData("追加パイロット")).FullRecover();
                }
            }

            {
                var cf = CurrentForm();
                // ＨＰを回復
                cf.HP = cf.MaxHP;

                // ＥＮ、弾数を回復
                cf.FullSupply();

                //// ステータス異常のみを消去
                //i = 1;
                //while (i <= cf.CountCondition())
                //{
                //    string localCondition() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition1() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition2() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition3() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition4() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition5() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    string localCondition6() { object argIndex1 = i; var ret = cf.Condition(argIndex1); return ret; }

                //    if (localCondition() == "残り時間" || localCondition1() == "非操作" || Strings.Right(localCondition2(), 2) == "付加" || Strings.Right(localCondition3(), 2) == "強化" || Strings.Right(localCondition4(), 3) == "付加２" || Strings.Right(localCondition5(), 3) == "強化２" || Strings.Right(localCondition6(), 2) == "ＵＰ")
                //    {
                //        i = (short)(i + 1);
                //    }
                //    else
                //    {
                //        cf.DeleteCondition(i);
                //    }
                //}

                // サポートアタック＆ガード、同時援護攻撃、カウンター攻撃回数回復
                cf.UsedSupportAttack = 0;
                cf.UsedSupportGuard = 0;
                cf.UsedSyncAttack = 0;
                cf.UsedCounterAttack = 0;
                cf.Mode = "通常";

                // 他形態も回復
                foreach (var of in OtherForms)
                {
                    of.HP = of.MaxHP;
                    of.EN = of.MaxEN;
                    //var loopTo3 = of.CountWeapon();
                    //for (j = 1; j <= loopTo3; j++)
                    //    of.SetBullet(j, of.MaxBullet(j));
                    //var loopTo4 = of.CountAbility();
                    //for (j = 1; j <= loopTo4; j++)
                    //    of.SetStock(j, of.MaxStock(j));
                }
            }
        }

        // ＥＮ＆弾数を回復
        public void FullSupply()
        {
            // ＥＮ回復
            EN = MaxEN;
            // 他形態も回復
            foreach (var of in OtherForms)
            {
                of.EN = of.MaxEN;
            }

            // 弾数回復
            BulletSupply();
            StockSupply();
        }

        // 弾数のみを回復
        public void BulletSupply()
        {
            foreach (var uw in Weapons)
            {
                uw.SetBulletFull();
            }

            // 他形態も回復
            foreach (var of in OtherForms)
            {
                foreach (var uw in of.Weapons)
                {
                    uw.SetBulletFull();
                }
            }
        }

        public void StockSupply()
        {
            foreach (var ua in Abilities)
            {
                ua.SetStockFull();
            }

            // 他形態も回復
            foreach (var of in OtherForms)
            {
                foreach (var ua in of.Abilities)
                {
                    ua.SetStockFull();
                }
            }
        }

        // ＨＰを percent ％回復
        public void RecoverHP(double percent)
        {
            HP = (int)(HP + MaxHP * percent / 100d);
            if (HP <= 0)
            {
                HP = 1;
            }

            // 特殊能力「不安定」による暴走チェック
            if (IsFeatureAvailable("不安定"))
            {
                if (HP <= MaxHP / 4 && !IsConditionSatisfied("暴走"))
                {
                    AddCondition("暴走", -1, cdata: "");
                }
            }
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

            // 味方ステージの1ターン目(スタートイベント直後)は回復を行わない
            if (SRC.Stage == "味方" && SRC.Turn == 1)
            {
                return;
            }

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
            //            withBlock.Plana = (int)(withBlock.Plana + withBlock.MaxPlana() / 16 + (long)(withBlock.MaxPlana() * FeatureLevel("霊力回復")) / 10L - (long)(withBlock.MaxPlana() * FeatureLevel("霊力消費")) / 10L);
            //            HP = (int)((long)(MaxHP * hp_ratio) / 100L);
            //            EN = (int)((long)(MaxEN * en_ratio) / 100L);
            //        }

            //        // ＳＰ回復
            //        if (withBlock.IsSkillAvailable("ＳＰ回復"))
            //        {
            //            withBlock.SP = withBlock.SP + withBlock.Level / 8 + 5;
            //        }

            //        if (withBlock.IsSkillAvailable("精神統一"))
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
            //        {
            //            var withBlock1 = Pilot(i);
            //            if (withBlock1.IsSkillAvailable("ＳＰ回復"))
            //            {
            //                withBlock1.SP = withBlock1.SP + withBlock1.Level / 8 + 5;
            //            }

            //            if (withBlock1.IsSkillAvailable("精神統一"))
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
            //        {
            //            var withBlock2 = Support(i);
            //            if (withBlock2.IsSkillAvailable("ＳＰ回復"))
            //            {
            //                withBlock2.SP = withBlock2.SP + withBlock2.Level / 8 + 5;
            //            }

            //            if (withBlock2.IsSkillAvailable("精神統一"))
            //            {
            //                if (withBlock2.SP < withBlock2.MaxSP / 5)
            //                {
            //                    withBlock2.SP = withBlock2.SP + withBlock2.MaxSP / 10;
            //                }
            //            }
            //        }
            //    }

            //    if (IsFeatureAvailable("追加サポート"))
            //    {
            //        {
            //            var withBlock3 = AdditionalSupport();
            //            if (withBlock3.IsSkillAvailable("ＳＰ回復"))
            //            {
            //                withBlock3.SP = withBlock3.SP + withBlock3.Level / 8 + 5;
            //            }

            //            if (withBlock3.IsSkillAvailable("精神統一"))
            //            {
            //                if (withBlock3.SP < withBlock3.MaxSP / 5)
            //                {
            //                    withBlock3.SP = withBlock3.SP + withBlock3.MaxSP / 10;
            //                }
            //            }
            //        }
            //    }

            // 行動回数
            UsedAction = 0;

            // スペシャルパワー効果を解除
            RemoveSpecialPowerInEffect("ターン");

            //    // スペシャルパワー自動発動
            //    {
            //        var withBlock4 = MainPilot();
            //        var loopTo2 = withBlock4.CountSkill();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            if (withBlock4.Skill(i) == "スペシャルパワー自動発動")
            //            {
            //                string localSkillData() { object argIndex1 = i; var ret = withBlock4.SkillData(argIndex1); return ret; }

            //                spname = GeneralLib.LIndex(localSkillData(), 2);
            //                string localSkillData1() { object argIndex1 = i; var ret = withBlock4.SkillData(argIndex1); return ret; }

            //                string localLIndex() { string arglist = hsbeebf58ad14b49d88a05b39a41264136(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                int localStrToLng() { string argexpr = hsa615f5f7d41941adb5ff8c50a645e7c9(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                if (withBlock4.Morale >= localStrToLng() && !IsSpecialPowerInEffect(spname))
            //                {
            //                    GUI.Center(x, y);
            //                    withBlock4.UseSpecialPower(spname, 0d);
            //                    if (Status == "他形態")
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //        }

            //        if (IsConditionSatisfied("スペシャルパワー自動発動付加") || IsConditionSatisfied("スペシャルパワー自動発動付加２"))
            //        {
            //            spname = GeneralLib.LIndex(withBlock4.SkillData("スペシャルパワー自動発動"), 2);
            //            string localLIndex1() { object argIndex1 = "スペシャルパワー自動発動"; string arglist = withBlock4.SkillData(argIndex1); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //            int localStrToLng1() { string argexpr = hs007c2f9d021b4299932cdd690ee4914e(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //            if (withBlock4.Morale >= localStrToLng1() && !IsSpecialPowerInEffect(spname))
            //            {
            //                GUI.Center(x, y);
            //                withBlock4.UseSpecialPower(spname, 0d);
            //                if (Status == "他形態")
            //                {
            //                    return;
            //                }
            //            }
            //        }
            //    }

            //    var loopTo3 = CountPilot();
            //    for (i = 2; i <= loopTo3; i++)
            //    {
            //        {
            //            var withBlock5 = Pilot(i);
            //            var loopTo4 = withBlock5.CountSkill();
            //            for (j = 1; j <= loopTo4; j++)
            //            {
            //                if (withBlock5.Skill(j) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData2() { object argIndex1 = j; var ret = withBlock5.SkillData(argIndex1); return ret; }

            //                    spname = GeneralLib.LIndex(localSkillData2(), 2);
            //                    string localSkillData3() { object argIndex1 = j; var ret = withBlock5.SkillData(argIndex1); return ret; }

            //                    string localLIndex2() { string arglist = hsa05862d2d473454a8c0d7941dff82ac5(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                    int localStrToLng2() { string argexpr = hs1050a377ee804d6d86fc7e98398e12ee(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                    if (withBlock5.Morale >= localStrToLng2() && !IsSpecialPowerInEffect(spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock5.UseSpecialPower(spname, 0d);
            //                        if (Status == "他形態")
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
            //        {
            //            var withBlock6 = Support(i);
            //            var loopTo6 = withBlock6.CountSkill();
            //            for (j = 1; j <= loopTo6; j++)
            //            {
            //                if (withBlock6.Skill(j) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData4() { object argIndex1 = j; var ret = withBlock6.SkillData(argIndex1); return ret; }

            //                    spname = GeneralLib.LIndex(localSkillData4(), 2);
            //                    string localSkillData5() { object argIndex1 = j; var ret = withBlock6.SkillData(argIndex1); return ret; }

            //                    string localLIndex3() { string arglist = hsfa97fc36239f45ffacc7690204644669(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                    int localStrToLng3() { string argexpr = hsaaa9e6d3323e4453a1b656c0d1ba1987(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                    if (withBlock6.Morale >= localStrToLng3() && !IsSpecialPowerInEffect(spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock6.UseSpecialPower(spname, 0d);
            //                        if (Status == "他形態")
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (IsFeatureAvailable("追加サポート"))
            //    {
            //        {
            //            var withBlock7 = AdditionalSupport();
            //            var loopTo7 = withBlock7.CountSkill();
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                if (withBlock7.Skill(i) == "スペシャルパワー自動発動")
            //                {
            //                    string localSkillData6() { object argIndex1 = i; var ret = withBlock7.SkillData(argIndex1); return ret; }

            //                    spname = GeneralLib.LIndex(localSkillData6(), 2);
            //                    string localSkillData7() { object argIndex1 = i; var ret = withBlock7.SkillData(argIndex1); return ret; }

            //                    string localLIndex4() { string arglist = hs393cfbef7a894559b16f4706c8e49b64(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                    int localStrToLng4() { string argexpr = hsd3fcfdcfd593490696f3d091f42d35e5(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                    if (withBlock7.Morale >= localStrToLng4() && !IsSpecialPowerInEffect(spname))
            //                    {
            //                        GUI.Center(x, y);
            //                        withBlock7.UseSpecialPower(spname, 0d);
            //                        if (Status == "他形態")
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
            //        if (withBlock8.IsSkillAvailable("起死回生") && withBlock8.SP <= withBlock8.MaxSP / 5 && HP <= MaxHP / 5 && EN <= MaxEN / 5)
            //        {
            //            withBlock8.SP = withBlock8.MaxSP;
            //            HP = MaxHP;
            //            EN = MaxEN;
            //            if (SRC.SpecialPowerAnimation)
            //            {
            //                GUI.Center(x, y);
            //                if (SRC.SPDList.IsDefined("ド根性"))
            //                {
            //                    SRC.SPDList.Item("ド根性").PlayAnimation();
            //                }
            //            }
            //        }
            //    }

            //    // ＨＰとＥＮ回復＆消費
            //    // MOD START MARGE
            //    // If Not IsConditionSatisfied("回復不能") Then
            //    if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能"))
            //    {
            //        // MOD END MARGE
            //        if (IsFeatureAvailable("ＨＰ回復"))
            //        {
            //            hp_recovery = (int)(10d * FeatureLevel("ＨＰ回復"));
            //        }

            //        if (IsFeatureAvailable("ＥＮ回復"))
            //        {
            //            en_recovery = (int)(10d * FeatureLevel("ＥＮ回復"));
            //        }
            //    }

            //    if (IsFeatureAvailable("ＨＰ消費"))
            //    {
            //        hp_recovery = (int)(hp_recovery - 10d * FeatureLevel("ＨＰ消費"));
            //    }

            //    if (IsFeatureAvailable("ＥＮ消費"))
            //    {
            //        en_recovery = (int)(en_recovery - 10d * FeatureLevel("ＥＮ消費"));
            //    }

            //    // 毒によるＨＰ減少
            //    short plv;
            //    if (IsConditionSatisfied("毒"))
            //    {
            //        if (Expression.IsOptionDefined("毒効果大") && BossRank < 0)
            //        {
            //            plv = 25;
            //        }
            //        else
            //        {
            //            plv = 10;
            //        }

            //        // 変化なし
            //        if (Weakness("毒"))
            //        {
            //            plv = (short)(2 * plv);
            //        }
            //        else if (Effective("毒"))
            //        {
            //        }
            //        else if (Immune("毒") || Absorb("毒"))
            //        {
            //            plv = 0;
            //        }
            //        else if (Resist("毒"))
            //        {
            //            plv = (short)(plv / 2);
            //        }

            //        hp_recovery = hp_recovery - plv;
            //    }

            //    // 活動限界時間切れ？
            //    if (ConditionLifetime("活動限界") == 1)
            //    {
            //        GUI.Center(x, y);
            //        Escape();
            //        GUI.OpenMessageForm(u1: null, u2: null);
            //        GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //        GUI.CloseMessageForm();
            //        Event.HandleEvent("破壊", MainPilot().ID);
            //    }

            //    // 死の宣告
            //    if (ConditionLifetime("死の宣告") == 1)
            //    {
            //        hp_recovery = hp_recovery - 1000;
            //    }

            //    // 残り時間
            //    if (ConditionLifetime("残り時間") == 1)
            //    {
            //        is_time_limit = true;
            //        if (IsFeatureAvailable("ノーマルモード"))
            //        {
            //            // ハイパーモード＆変身の時間切れの場合は戻り先の形態を記録しておく
            //            next_form = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
            //        }
            //    }

            // ＨＰ回復などを付加した場合のことを考えて状態のアップデートは
            // ＨＰ＆ＥＮ回復量を計算した後に行う
            var hp_ratio = 100 * HP / (double)MaxHP;
            var en_ratio = 100 * EN / (double)MaxEN;
            UpdateCondition(true);
            HP = (int)((long)(MaxHP * hp_ratio) / 100L);
            EN = (int)((long)(MaxEN * en_ratio) / 100L);

            // サポートアタック＆ガード
            UsedSupportAttack = 0;
            UsedSupportGuard = 0;

            // 同時援護攻撃
            UsedSyncAttack = 0;

            // カウンター攻撃
            UsedCounterAttack = 0;

            // チャージ完了？
            if (ConditionLifetime("チャージ") == 0)
            {
                AddCondition("チャージ完了", 1, cdata: "");
            }

            //    // 付加された移動能力が切れた場合の処理
            //    if (Status == "出撃" && !string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        switch (Area ?? "")
            //        {
            //            case "空中":
            //                {
            //                    if (!IsTransAvailable("空"))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            GUI.OpenMessageForm(u1: null, u2: null);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event.HandleEvent("破壊", MainPilot().ID);
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
            //                    if (!IsTransAvailable("陸"))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            GUI.OpenMessageForm(u1: null, u2: null);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event.HandleEvent("破壊", MainPilot().ID);
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
            //                    if (!IsFeatureAvailable("水上移動") && !IsFeatureAvailable("ホバー移動"))
            //                    {
            //                        if (!IsAbleToEnter(x, y))
            //                        {
            //                            GUI.Center(x, y);
            //                            Escape();
            //                            GUI.OpenMessageForm(u1: null, u2: null);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event.HandleEvent("破壊", MainPilot().ID);
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
            //                            GUI.OpenMessageForm(u1: null, u2: null);
            //                            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                            GUI.CloseMessageForm();
            //                            Event.HandleEvent("破壊", MainPilot().ID);
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

            //    if (Status == "格納")
            //    {
            //        // 格納時は回復率ＵＰ
            //        // MOD START MARGE
            //        // If Not IsConditionSatisfied("回復不能") Then
            //        if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能"))
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
            //        // If SpecialEffectImmune("弱" && Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Immune(Left$(cname, Len(cname) - 6)) _
            //        // '                                        Then
            //        // cname = ""
            //        // End If
            //        // ElseIf Right$(cname, 6) = "属性有効付加" Then
            //        // If SpecialEffectImmune("有" && Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Absorb(Left$(cname, Len(cname) - 6)) _
            //        // '                                            Or Immune(Left$(cname, Len(cname) - 6)) _
            //        // '                                        Then
            //        // cname = ""
            //        // End If
            //        // ElseIf Right$(cname, 6) = "属性使用不能" Then
            //        // If SpecialEffectImmune("剋" && Left$(cname, Len(cname) - 6)) Then
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
            //        if (td.IsFeatureAvailable("効果範囲"))
            //        {
            //            var loopTo10 = GeneralLib.LLength(td.FeatureData("効果範囲"));
            //            for (i = 1; i <= loopTo10; i++)
            //            {
            //                string localLIndex5() { object argIndex1 = "効果範囲"; string arglist = td.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, i); return ret; }

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
            //        if (IsFeatureAvailable("地形効果無効化"))
            //        {
            //            if (GeneralLib.LLength(FeatureData("地形効果無効化")) > 1)
            //            {
            //                var loopTo11 = GeneralLib.LLength(FeatureData("地形効果無効化"));
            //                for (i = 2; i <= loopTo11; i++)
            //                {
            //                    string localLIndex6() { object argIndex1 = "地形効果無効化"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, i); return ret; }

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
            //        else if (IsSpecialPowerInEffect("地形効果無効化"))
            //        {
            //            is_immune_to_terrain_effect = true;
            //        }

            //        // 地形効果を適用
            //        if (is_terrain_effective)
            //        {
            //            var loopTo12 = td.CountFeature();
            //            for (i = 1; i <= loopTo12; i++)
            //            {
            //                if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能"))
            //                {
            //                    switch (td.Feature(i) ?? "")
            //                    {
            //                        case "ＨＰ回復":
            //                            {
            //                                double localFeatureLevel() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                hp_recovery = (int)(hp_recovery + 10d * localFeatureLevel());
            //                                break;
            //                            }

            //                        case "ＥＮ回復":
            //                            {
            //                                double localFeatureLevel1() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                en_recovery = (int)(en_recovery + 10d * localFeatureLevel1());
            //                                break;
            //                            }

            //                        case "ＨＰ増加":
            //                            {
            //                                double localFeatureLevel2() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                hp_up = (int)(hp_up + 1000d * localFeatureLevel2());
            //                                break;
            //                            }

            //                        case "ＥＮ増加":
            //                            {
            //                                double localFeatureLevel3() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                en_up = (int)(en_up + 10d * localFeatureLevel3());
            //                                break;
            //                            }
            //                    }
            //                }

            //                if (!is_immune_to_terrain_effect)
            //                {
            //                    switch (td.Feature(i) ?? "")
            //                    {
            //                        case "ＨＰ減少":
            //                            {
            //                                string localFeatureData() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                string localFeatureData1() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localEffective() { string arganame = hs910888dc06c349a7b21a4df1e60d8c7a(); var ret = Effective(arganame); return ret; }

            //                                string localFeatureData2() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localAbsorb() { string arganame = hs52033841100047b99c1279b816aeef70(); var ret = Absorb(arganame); return ret; }

            //                                string localFeatureData3() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localResist() { string arganame = hsc6f55b713dcb4a289541bb10d274fe96(); var ret = Resist(arganame); return ret; }

            //                                string localFeatureData4() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localImmune() { string arganame = hs7f2e711ffdc34440b08ea8add7c269ee(); var ret = Immune(arganame); return ret; }

            //                                if (Weakness(localFeatureData()))
            //                                {
            //                                    double localFeatureLevel4() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 20d * localFeatureLevel4());
            //                                }
            //                                else if (localEffective())
            //                                {
            //                                    double localFeatureLevel5() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 10d * localFeatureLevel5());
            //                                }
            //                                else if (localAbsorb())
            //                                {
            //                                    double localFeatureLevel6() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery + 10d * localFeatureLevel6());
            //                                }
            //                                else if (localResist())
            //                                {
            //                                    double localFeatureLevel7() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 5d * localFeatureLevel7());
            //                                }
            //                                else if (!localImmune())
            //                                {
            //                                    double localFeatureLevel8() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_recovery = (int)(hp_recovery - 10d * localFeatureLevel8());
            //                                }

            //                                break;
            //                            }

            //                        case "ＥＮ減少":
            //                            {
            //                                string localFeatureData5() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                string localFeatureData6() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localEffective1() { string arganame = hs3c05c0f0f5c74d50b2ebe668e4cefb50(); var ret = Effective(arganame); return ret; }

            //                                string localFeatureData7() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localAbsorb1() { string arganame = hsf2d45f6c0d3242c088a863c924424bdc(); var ret = Absorb(arganame); return ret; }

            //                                string localFeatureData8() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localResist1() { string arganame = hs2feadcdb67784d8bb37649df03e02004(); var ret = Resist(arganame); return ret; }

            //                                string localFeatureData9() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localImmune1() { string arganame = hs2ea0185e20964cc686bdf2ee374b1db5(); var ret = Immune(arganame); return ret; }

            //                                if (Weakness(localFeatureData5()))
            //                                {
            //                                    double localFeatureLevel9() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 20d * localFeatureLevel9());
            //                                }
            //                                else if (localEffective1())
            //                                {
            //                                    double localFeatureLevel10() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 10d * localFeatureLevel10());
            //                                }
            //                                else if (localAbsorb1())
            //                                {
            //                                    double localFeatureLevel11() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery + 10d * localFeatureLevel11());
            //                                }
            //                                else if (localResist1())
            //                                {
            //                                    double localFeatureLevel12() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 5d * localFeatureLevel12());
            //                                }
            //                                else if (!localImmune1())
            //                                {
            //                                    double localFeatureLevel13() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_recovery = (int)(en_recovery - 10d * localFeatureLevel13());
            //                                }

            //                                break;
            //                            }

            //                        case "ＨＰ低下":
            //                            {
            //                                string localFeatureData10() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                string localFeatureData11() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localEffective2() { string arganame = hsde07f92cead745858b4d820263f53c91(); var ret = Effective(arganame); return ret; }

            //                                string localFeatureData12() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localAbsorb2() { string arganame = hs94c6cc3a1d3d4ca68ebec987f086a962(); var ret = Absorb(arganame); return ret; }

            //                                string localFeatureData13() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localResist2() { string arganame = hs736b9f12f5c24c189d82474b29c54514(); var ret = Resist(arganame); return ret; }

            //                                string localFeatureData14() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localImmune2() { string arganame = hs125e0e0f0c7f4295afab6bc4e0bdd8b7(); var ret = Immune(arganame); return ret; }

            //                                if (Weakness(localFeatureData10()))
            //                                {
            //                                    double localFeatureLevel14() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 2000d * localFeatureLevel14());
            //                                }
            //                                else if (localEffective2())
            //                                {
            //                                    double localFeatureLevel15() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 1000d * localFeatureLevel15());
            //                                }
            //                                else if (localAbsorb2())
            //                                {
            //                                    double localFeatureLevel16() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up + 1000d * localFeatureLevel16());
            //                                }
            //                                else if (localResist2())
            //                                {
            //                                    double localFeatureLevel17() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 500d * localFeatureLevel17());
            //                                }
            //                                else if (!localImmune2())
            //                                {
            //                                    double localFeatureLevel18() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    hp_up = (int)(hp_up - 1000d * localFeatureLevel18());
            //                                }

            //                                break;
            //                            }

            //                        case "ＥＮ低下":
            //                            {
            //                                string localFeatureData15() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                string localFeatureData16() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localEffective3() { string arganame = hs19293752f23e4276b0cbef58767f8358(); var ret = Effective(arganame); return ret; }

            //                                string localFeatureData17() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localAbsorb3() { string arganame = hs5c8089f996604cf3acee553527084f88(); var ret = Absorb(arganame); return ret; }

            //                                string localFeatureData18() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localResist3() { string arganame = hs7282195c4a7b4004aadd6adbd33a9a60(); var ret = Resist(arganame); return ret; }

            //                                string localFeatureData19() { object argIndex1 = i; var ret = td.FeatureData(argIndex1); return ret; }

            //                                bool localImmune3() { string arganame = hs18e2d5e0cbb14782bbf344a497c3c2ed(); var ret = Immune(arganame); return ret; }

            //                                if (Weakness(localFeatureData15()))
            //                                {
            //                                    double localFeatureLevel19() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 20d * localFeatureLevel19());
            //                                }
            //                                else if (localEffective3())
            //                                {
            //                                    double localFeatureLevel20() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 10d * localFeatureLevel20());
            //                                }
            //                                else if (localAbsorb3())
            //                                {
            //                                    double localFeatureLevel21() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_up = (int)(en_up + 10d * localFeatureLevel21());
            //                                }
            //                                else if (localResist3())
            //                                {
            //                                    double localFeatureLevel22() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 5d * localFeatureLevel22());
            //                                }
            //                                else if (!localImmune3())
            //                                {
            //                                    double localFeatureLevel23() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                    en_up = (int)(en_up - 10d * localFeatureLevel23());
            //                                }

            //                                break;
            //                            }

            //                        case "状態付加":
            //                            {
            //                                cname = td.FeatureData(i);

            //                                // 状態が無効化されるかチェック
            //                                switch (cname ?? "")
            //                                {
            //                                    case "装甲劣化":
            //                                        {
            //                                            if (SpecialEffectImmune("劣"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "混乱":
            //                                        {
            //                                            if (SpecialEffectImmune("乱"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "恐怖":
            //                                        {
            //                                            if (SpecialEffectImmune("恐"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "踊り":
            //                                        {
            //                                            if (SpecialEffectImmune("踊"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "狂戦士":
            //                                        {
            //                                            if (SpecialEffectImmune("狂"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "ゾンビ":
            //                                        {
            //                                            if (SpecialEffectImmune("ゾ"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "回復不能":
            //                                        {
            //                                            if (SpecialEffectImmune("害"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "石化":
            //                                        {
            //                                            if (SpecialEffectImmune("石"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "凍結":
            //                                        {
            //                                            if (SpecialEffectImmune("凍"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "麻痺":
            //                                        {
            //                                            if (SpecialEffectImmune("痺"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "睡眠":
            //                                        {
            //                                            if (SpecialEffectImmune("眠"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "毒":
            //                                        {
            //                                            if (SpecialEffectImmune("毒"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "盲目":
            //                                        {
            //                                            if (SpecialEffectImmune("盲"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "沈黙":
            //                                        {
            //                                            if (SpecialEffectImmune("黙"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }
            //                                    // 属性使用不能状態
            //                                    case "オーラ使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋オ"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "超能力使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋超"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "同調率使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋シ"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "超感覚使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋サ"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "知覚強化使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋サ"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋霊"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "術使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋術"))
            //                                            {
            //                                                cname = "";
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "技使用不能":
            //                                        {
            //                                            if (SpecialEffectImmune("剋技"))
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
            //                                                    bool localSpecialEffectImmune() { string arganame = "弱" + Strings.Left(cname, Strings.Len(cname) - 6); var ret = SpecialEffectImmune(arganame); return ret; }

            //                                                    bool localAbsorb4() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Absorb(arganame); return ret; }

            //                                                    bool localImmune4() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Immune(arganame); return ret; }

            //                                                    if (localSpecialEffectImmune() || localAbsorb4() || localImmune4())
            //                                                    {
            //                                                        cname = "";
            //                                                    }
            //                                                }
            //                                                else if (Strings.Right(cname, 6) == "属性有効付加")
            //                                                {
            //                                                    bool localSpecialEffectImmune1() { string arganame = "有" + Strings.Left(cname, Strings.Len(cname) - 6); var ret = SpecialEffectImmune(arganame); return ret; }

            //                                                    bool localAbsorb5() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Absorb(arganame); return ret; }

            //                                                    bool localImmune5() { string arganame = Strings.Left(cname, Strings.Len(cname) - 6); var ret = Immune(arganame); return ret; }

            //                                                    if (localSpecialEffectImmune1() || localAbsorb5() || localImmune5())
            //                                                    {
            //                                                        cname = "";
            //                                                    }
            //                                                }
            //                                                else if (Strings.Right(cname, 6) == "属性使用不能")
            //                                                {
            //                                                    if (SpecialEffectImmune("剋" + Strings.Left(cname, Strings.Len(cname) - 6)))
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
            //                                    if (td.IsFeatureLevelSpecified(i))
            //                                    {
            //                                        double localFeatureLevel24() { object argIndex1 = i; var ret = td.FeatureLevel(argIndex1); return ret; }

            //                                        AddCondition(cname, (short)localFeatureLevel24(), cdata: "");
            //                                    }
            //                                    else
            //                                    {
            //                                        AddCondition(cname, 10000, cdata: "");
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
            //    if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能") && !Expression.IsOptionDefined("ＥＮ自然回復無効"))
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
            //    if (IsFeatureAvailable("不安定"))
            //    {
            //        if (HP <= MaxHP / 4 && !IsConditionSatisfied("暴走"))
            //        {
            //            AddCondition("暴走", -1, cdata: "");
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
            //        if (IsFeatureAvailable("ノーマルモード"))
            //        {
            //            // ただしノーマルモードに戻れない地形だとそのまま退却……
            //            string localLIndex8() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            Unit localOtherForm() { object argIndex1 = (object)hs35732b48c6874150a97edd31eb6e765a(); var ret = OtherForm(argIndex1); return ret; }

            //            if (localOtherForm().IsAbleToEnter(x, y))
            //            {
            //                string localLIndex7() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                Transform(localLIndex7());
            //            }
            //            else
            //            {
            //                GUI.Center(x, y);
            //                Escape();
            //                GUI.OpenMessageForm(u1: null, u2: null);
            //                GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                GUI.CloseMessageForm();
            //                Event.HandleEvent("破壊", MainPilot().ID);
            //                return;
            //            }
            //        }
            //        else if (IsFeatureAvailable("変形"))
            //        {
            //            // 変形できれば変形
            //            buf = FeatureData("変形");
            //            var loopTo13 = GeneralLib.LLength(buf);
            //            for (i = 2; i <= loopTo13; i++)
            //            {
            //                {
            //                    var withBlock9 = OtherForm(GeneralLib.LIndex(buf, i));
            //                    if (withBlock9.IsAbleToEnter(x, y) && !withBlock9.IsFeatureAvailable("ＥＮ消費"))
            //                    {
            //                        Transform(GeneralLib.LIndex(buf, i));
            //                        break;
            //                    }
            //                }
            //            }

            //            if (i > GeneralLib.LLength(buf))
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
            //        if (!string.IsNullOrEmpty(next_form))
            //        {
            //            // ハイパーモード＆変身の時間切れ
            //            u = OtherForm(next_form);
            //            if (u.IsAbleToEnter(x, y))
            //            {
            //                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //                if (u.IsFeatureAvailable("追加パイロット"))
            //                {
            //                    bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

            //                    if (!localIsDefined())
            //                    {
            //                        SRC.PList.Add(u.FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
            //                        this.Party0 = argpparty;
            //                    }
            //                }

            //                // ノーマルモードメッセージ
            //                bool localIsMessageDefined() { string argmain_situation = "ノーマルモード(" + u.Name + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //                if (IsMessageDefined("ノーマルモード(" + Name + "=>" + u.Name + ")"))
            //                {
            //                    GUI.OpenMessageForm(u1: null, u2: null);
            //                    PilotMessage("ノーマルモード(" + Name + "=>" + u.Name + ")", msg_mode: "");
            //                    GUI.CloseMessageForm();
            //                }
            //                else if (localIsMessageDefined())
            //                {
            //                    GUI.OpenMessageForm(u1: null, u2: null);
            //                    PilotMessage("ノーマルモード(" + u.Name + ")", msg_mode: "");
            //                    GUI.CloseMessageForm();
            //                }
            //                else if (IsMessageDefined("ノーマルモード"))
            //                {
            //                    GUI.OpenMessageForm(u1: null, u2: null);
            //                    PilotMessage("ノーマルモード", msg_mode: "");
            //                    GUI.CloseMessageForm();
            //                }
            //                // 特殊効果
            //                bool localIsSpecialEffectDefined() { string argmain_situation = "ノーマルモード(" + u.Name + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                if (IsSpecialEffectDefined("ノーマルモード(" + Name + "=>" + u.Name + ")", sub_situation: ""))
            //                {
            //                    SpecialEffect("ノーマルモード(" + Name + "=>" + u.Name + ")", sub_situation: "");
            //                }
            //                else if (localIsSpecialEffectDefined())
            //                {
            //                    SpecialEffect("ノーマルモード(" + u.Name + ")", sub_situation: "");
            //                }
            //                else if (IsSpecialEffectDefined("ノーマルモード", sub_situation: ""))
            //                {
            //                    SpecialEffect("ノーマルモード", sub_situation: "");
            //                }

            //                // 変形
            //                string localLIndex9() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                Transform(localLIndex9());
            //            }
            //            else
            //            {
            //                // 変形するとその地形にいれなくなる場合は退却
            //                GUI.Center(x, y);
            //                Escape();
            //                GUI.OpenMessageForm(u1: null, u2: null);
            //                GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            //                GUI.CloseMessageForm();
            //                Event.HandleEvent("破壊", MainPilot().ID);
            //                return;
            //            }
            //        }
            //        else if (IsFeatureAvailable("分離"))
            //        {
            //            // 合体時間切れ

            //            // メッセージ表示
            //            bool localIsMessageDefined2() { string argmain_situation = "分離(" + Name + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //            bool localIsMessageDefined3() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(argIndex1) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //            if (localIsMessageDefined2() || localIsMessageDefined3() || IsMessageDefined("分離"))
            //            {
            //                if (IsFeatureAvailable("分離ＢＧＭ"))
            //                {
            //                    Sound.StartBGM(FeatureData("分離ＢＧＭ"));
            //                    GUI.Sleep(500);
            //                }
            //                else if (MainPilot().BGM != "-")
            //                {
            //                    Sound.StartBGM(MainPilot().BGM);
            //                    MainPilot().BGM = argbgm_name1;
            //                    GUI.Sleep(500);
            //                }

            //                GUI.Center(x, y);
            //                GUI.RefreshScreen();
            //                GUI.OpenMessageForm(u1: null, u2: null);
            //                bool localIsMessageDefined1() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(argIndex1) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //                if (IsMessageDefined("分離(" + Name + ")"))
            //                {
            //                    PilotMessage("分離(" + Name + ")", msg_mode: "");
            //                }
            //                else if (localIsMessageDefined1())
            //                {
            //                    PilotMessage("分離(" + FeatureName("分離") + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    PilotMessage("分離", msg_mode: "");
            //                }

            //                GUI.CloseMessageForm();
            //            }
            //            // 特殊効果
            //            bool localIsSpecialEffectDefined1() { object argIndex1 = "分離"; string argmain_situation = "分離(" + FeatureName(argIndex1) + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            if (IsSpecialEffectDefined("分離(" + Name + ")", sub_situation: ""))
            //            {
            //                SpecialEffect("分離(" + Name + ")", sub_situation: "");
            //            }
            //            else if (localIsSpecialEffectDefined1())
            //            {
            //                SpecialEffect("分離(" + FeatureName("分離") + ")", sub_situation: "");
            //            }
            //            else
            //            {
            //                SpecialEffect("分離", sub_situation: "");
            //            }

            //            // 分離
            //            Split();
            //        }
            //        else
            //        {
            //            // 制限時間切れ
            //            GUI.Center(x, y);
            //            GUI.RefreshScreen();
            //            GUI.OpenMessageForm(u1: null, u2: null);
            //            GUI.DisplaySysMessage(Nickname + "は制限時間切れのため退却します。");
            //            GUI.CloseMessageForm();
            //            Escape();
            //            Event.HandleEvent("破壊", MainPilot().ID);
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

            //    if (Status != "出撃")
            //    {
            //        return;
            //    }

            //    if (!IsFeatureAvailable("ハイパーモード"))
            //    {
            //        return;
            //    }

            //    fname = FeatureName("ハイパーモード");
            //    flevel = FeatureLevel("ハイパーモード");
            //    fdata = FeatureData("ハイパーモード");
            //    if (Strings.InStr(fdata, "自動発動") == 0)
            //    {
            //        return;
            //    }

            //    // 発動条件を満たす？
            //    if (this.MainPilot().Morale < (short)(10d * flevel) + 100 && (HP > MaxHP / 4 || Strings.InStr(fdata, "気力発動") > 0))
            //    {
            //        return;
            //    }

            //    // 変身中・能力コピー中はハイパーモードを使用できない
            //    if (IsConditionSatisfied("ノーマルモード付加"))
            //    {
            //        return;
            //    }

            //    // ハイパーモード先の形態が利用可能？
            //    uname = GeneralLib.LIndex(fdata, 2);
            //    is_available = false;
            //    {
            //        var withBlock = OtherForm(uname);
            //        switch (Map.TerrainClass(x, y) ?? "")
            //        {
            //            case "空":
            //                {
            //                    if (withBlock.IsTransAvailable("空"))
            //                    {
            //                        is_available = true;
            //                    }

            //                    break;
            //                }

            //            case "深水":
            //                {
            //                    if (withBlock.IsTransAvailable("空") || withBlock.IsTransAvailable("水") || withBlock.IsTransAvailable("水上"))
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
            //    if (SRC.UDList.IsDefined(uname))
            //    {
            //        {
            //            var withBlock1 = SRC.UDList.Item(uname);
            //            if (IsFeatureAvailable("追加パイロット"))
            //            {
            //                bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

            //                if (!localIsDefined())
            //                {
            //                    SRC.PList.Add(FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
            //                    this.Party0 = argpparty;
            //                }
            //            }
            //        }
            //    }

            //    // ＢＧＭを切り替え
            //    if (IsFeatureAvailable("ハイパーモードＢＧＭ"))
            //    {
            //        var loopTo = CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            string localFeature() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //            string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //            string localLIndex() { string arglist = hs2eb6d7953f284c87970de1e6c7ca058d(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            if (localFeature() == "ハイパーモードＢＧＭ" && (localLIndex() ?? "") == (uname ?? ""))
            //            {
            //                string localFeatureData() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                Sound.StartBGM(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
            //                GUI.Sleep(500);
            //                break;
            //            }
            //        }
            //    }

            //    // メッセージを表示
            //    bool localIsMessageDefined2() { string argmain_situation = "ハイパーモード(" + Name + "=>" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //    bool localIsMessageDefined3() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //    bool localIsMessageDefined4() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //    if (localIsMessageDefined2() || localIsMessageDefined3() || localIsMessageDefined4() || IsMessageDefined("ハイパーモード"))
            //    {
            //        GUI.Center(x, y);
            //        GUI.RefreshScreen();
            //        if (!message_window_visible)
            //        {
            //            GUI.OpenMessageForm(u1: null, u2: null);
            //        }
            //        else
            //        {
            //            message_window_visible = true;
            //        }

            //        // メッセージを表示
            //        bool localIsMessageDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //        bool localIsMessageDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //        if (IsMessageDefined("ハイパーモード(" + Name + "=>" + uname + ")"))
            //        {
            //            PilotMessage("ハイパーモード(" + Name + "=>" + uname + ")", msg_mode: "");
            //        }
            //        else if (localIsMessageDefined())
            //        {
            //            PilotMessage("ハイパーモード(" + uname + ")", msg_mode: "");
            //        }
            //        else if (localIsMessageDefined1())
            //        {
            //            PilotMessage("ハイパーモード(" + fname + ")", msg_mode: "");
            //        }
            //        else
            //        {
            //            PilotMessage("ハイパーモード", msg_mode: "");
            //        }

            //        if (!message_window_visible)
            //        {
            //            GUI.CloseMessageForm();
            //        }
            //    }

            //    // 特殊効果
            //    Commands.SaveSelections();
            //    Commands.SelectedUnit = this;
            //    Event.SelectedUnitForEvent = this;
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Event.SelectedTargetForEvent = null;
            //    bool localIsAnimationDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsAnimationDefined1() { object argIndex1 = "ハイパーモード"; string argmain_situation = "ハイパーモード(" + FeatureName(argIndex1) + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined() { string argmain_situation = "ハイパーモード(" + Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined1() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined2() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    if (IsAnimationDefined("ハイパーモード(" + Name + "=>" + uname + ")", sub_situation: ""))
            //    {
            //        PlayAnimation("ハイパーモード(" + Name + "=>" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsAnimationDefined())
            //    {
            //        PlayAnimation("ハイパーモード(" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsAnimationDefined1())
            //    {
            //        PlayAnimation("ハイパーモード(" + FeatureName("ハイパーモード") + ")", sub_situation: "");
            //    }
            //    else if (IsAnimationDefined("ハイパーモード", sub_situation: ""))
            //    {
            //        PlayAnimation("ハイパーモード", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined())
            //    {
            //        SpecialEffect("ハイパーモード(" + Name + "=>" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined1())
            //    {
            //        SpecialEffect("ハイパーモード(" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined2())
            //    {
            //        SpecialEffect("ハイパーモード(" + fname + ")", sub_situation: "");
            //    }
            //    else
            //    {
            //        SpecialEffect("ハイパーモード", sub_situation: "");
            //    }

            //    Commands.RestoreSelections();

            //    // ハイパーモードに変形
            //    Transform(uname);

            //    // ユニット変数を置き換え
            //    if (Commands.SelectedUnit is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
            //        {
            //            Commands.SelectedUnit = CurrentForm();
            //        }
            //    }

            //    if (Event.SelectedUnitForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event.SelectedUnitForEvent.ID ?? ""))
            //        {
            //            Event.SelectedUnitForEvent = CurrentForm();
            //        }
            //    }

            //    if (Commands.SelectedTarget is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
            //        {
            //            Commands.SelectedTarget = CurrentForm();
            //        }
            //    }

            //    if (Event.SelectedTargetForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event.SelectedTargetForEvent.ID ?? ""))
            //        {
            //            Event.SelectedTargetForEvent = CurrentForm();
            //        }
            //    }

            //    // 変形イベント
            //    {
            //        var withBlock2 = CurrentForm();
            //        Event.HandleEvent("変形", withBlock2.MainPilot().ID, withBlock2.Name);
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

            //    if (Status != "出撃")
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    if (!IsFeatureAvailable("ノーマルモード"))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // まだ元の形態でもＯＫ？
            //    if (IsAbleToEnter(x, y))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // ノーマルモード先が利用可能？
            //    uname = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
            //    Unit localOtherForm() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //    if (!localOtherForm().IsAbleToEnter(x, y))
            //    {
            //        return CheckAutoNormalModeRet;
            //    }

            //    // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //    if (SRC.UDList.IsDefined(uname))
            //    {
            //        {
            //            var withBlock = SRC.UDList.Item(uname);
            //            if (IsFeatureAvailable("追加パイロット"))
            //            {
            //                bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

            //                if (!localIsDefined())
            //                {
            //                    SRC.PList.Add(FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
            //                    this.Party0 = argpparty;
            //                }
            //            }
            //        }
            //    }

            //    // メッセージを表示
            //    bool localIsMessageDefined1() { string argmain_situation = "ノーマルモード(" + Name + "=>" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //    bool localIsMessageDefined2() { string argmain_situation = "ノーマルモード(" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //    if (localIsMessageDefined1() || localIsMessageDefined2() || IsMessageDefined("ノーマルモード"))
            //    {
            //        // ＢＧＭを切り替え
            //        if (IsFeatureAvailable("ノーマルモードＢＧＭ"))
            //        {
            //            var loopTo = CountFeature();
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                string localFeature() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //                string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                string localLIndex() { string arglist = hsbe3464a93f8d41b68a3072a6446fded5(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                if (localFeature() == "ノーマルモードＢＧＭ" && (localLIndex() ?? "") == (uname ?? ""))
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    Sound.StartBGM(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
            //                    GUI.Sleep(500);
            //                    break;
            //                }
            //            }
            //        }

            //        GUI.Center(x, y);
            //        GUI.RefreshScreen();
            //        if (!message_window_visible)
            //        {
            //            GUI.OpenMessageForm(u1: null, u2: null);
            //        }
            //        else
            //        {
            //            message_window_visible = true;
            //        }

            //        // メッセージを表示
            //        bool localIsMessageDefined() { string argmain_situation = "ノーマルモード(" + uname + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //        if (IsMessageDefined("ノーマルモード(" + Name + "=>" + uname + ")"))
            //        {
            //            PilotMessage("ノーマルモード(" + Name + "=>" + uname + ")", msg_mode: "");
            //        }
            //        else if (localIsMessageDefined())
            //        {
            //            PilotMessage("ノーマルモード(" + uname + ")", msg_mode: "");
            //        }
            //        else
            //        {
            //            PilotMessage("ノーマルモード", msg_mode: "");
            //        }

            //        if (!message_window_visible)
            //        {
            //            GUI.CloseMessageForm();
            //        }
            //    }

            //    // 特殊効果
            //    Commands.SaveSelections();
            //    Commands.SelectedUnit = this;
            //    Event.SelectedUnitForEvent = this;
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Event.SelectedTargetForEvent = null;
            //    bool localIsAnimationDefined() { string argmain_situation = "ノーマルモード(" + uname + ")"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined() { string argmain_situation = "ノーマルモード(" + Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    bool localIsSpecialEffectDefined1() { string argmain_situation = "ノーマルモード(" + uname + ")"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    if (IsAnimationDefined("ノーマルモード(" + Name + "=>" + uname + ")", sub_situation: ""))
            //    {
            //        PlayAnimation("ノーマルモード(" + Name + "=>" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsAnimationDefined())
            //    {
            //        PlayAnimation("ノーマルモード(" + uname + ")", sub_situation: "");
            //    }
            //    else if (IsAnimationDefined("ノーマルモード", sub_situation: ""))
            //    {
            //        PlayAnimation("ノーマルモード", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined())
            //    {
            //        SpecialEffect("ノーマルモード(" + Name + "=>" + uname + ")", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined1())
            //    {
            //        SpecialEffect("ノーマルモード(" + uname + ")", sub_situation: "");
            //    }
            //    else
            //    {
            //        SpecialEffect("ノーマルモード", sub_situation: "");
            //    }

            //    Commands.RestoreSelections();

            //    // ノーマルモードに変形
            //    Transform(uname);

            //    // ユニット変数を置き換え
            //    if (Commands.SelectedUnit is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
            //        {
            //            Commands.SelectedUnit = CurrentForm();
            //        }
            //    }

            //    if (Event.SelectedUnitForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event.SelectedUnitForEvent.ID ?? ""))
            //        {
            //            Event.SelectedUnitForEvent = CurrentForm();
            //        }
            //    }

            //    if (Commands.SelectedTarget is object)
            //    {
            //        if ((ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
            //        {
            //            Commands.SelectedTarget = CurrentForm();
            //        }
            //    }

            //    if (Event.SelectedTargetForEvent is object)
            //    {
            //        if ((ID ?? "") == (Event.SelectedTargetForEvent.ID ?? ""))
            //        {
            //            Event.SelectedTargetForEvent = CurrentForm();
            //        }
            //    }

            //    // 画面の再描画が必要？
            //    if (CurrentForm().IsConditionSatisfied("消耗"))
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
            //        Event.HandleEvent("変形", withBlock1.MainPilot().ID, withBlock1.Name);
            //    }

            //    return CheckAutoNormalModeRet;
        }

        // データをリセット
        public void Reset()
        {
            //    short i;
            //    string pname;
            //    var loopTo = CountCondition();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        DeleteCondition0(1);
            //    }

            //    RemoveAllSpecialPowerInEffect();
            Update();
            //    var loopTo1 = CountPilot();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //        localPilot().FullRecover();
            //    }

            //    var loopTo2 = CountSupport();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //        localSupport().FullRecover();
            //    }

            //    if (IsFeatureAvailable("追加パイロット"))
            //    {
            //        pname = FeatureData("追加パイロット");
            //        if (SRC.PList.IsDefined(pname))
            //        {
            //            Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            localItem().FullRecover();
            //        }
            //    }

            //    if (IsFeatureAvailable("追加サポート"))
            //    {
            //        pname = FeatureData("追加サポート");
            //        if (SRC.PList.IsDefined(pname))
            //        {
            //            Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            localItem1().FullRecover();
            //        }
            //    }

            //    HP = MaxHP;
            //    FullSupply();
            //    Mode = "通常";
        }


        // 弾数・使用回数共有の処理
        public void SyncBullet()
        {
            // TODO Impl SyncBullet
            //int j, a, w, i, k;
            //int lv, idx;

            //// 共属性武器の処理
            //var loopTo = CountWeapon();
            //for (w = 1; w <= loopTo; w++)
            //{
            //    if (IsWeaponClassifiedAs(w, "共"))
            //    {
            //        lv = WeaponLevel(w, "共");
            //        // 弾数を合わせる
            //        var loopTo1 = CountWeapon();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            if (w != i && IsWeaponClassifiedAs(i, "共") && lv == WeaponLevel(i, "共") && MaxBullet(w) > 0)
            //            {
            //                if (MaxBullet(i) > MaxBullet(w))
            //                {
            //                    SetBullet(i, GeneralLib.MinLng(Bullet(i), (MaxBullet(i) * Bullet(w)) / MaxBullet(w)));
            //                }
            //                else
            //                {
            //                    SetBullet(i, GeneralLib.MinLng(Bullet(i), (MaxBullet(i) * Bullet(w) / (double)MaxBullet(w) + 0.49999d)));
            //                }
            //            }
            //        }
            //        // アビリティの使用回数を合わせる
            //        var loopTo2 = CountAbility();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            if (IsAbilityClassifiedAs(i, "共") && lv == AbilityLevel(i, "共") && MaxBullet(w) > 0)
            //            {
            //                if (MaxStock(i) > MaxBullet(w))
            //                {
            //                    SetStock(i, GeneralLib.MinLng(Stock(i), (MaxStock(i) * Bullet(w)) / MaxBullet(w)));
            //                }
            //                else
            //                {
            //                    SetStock(i, GeneralLib.MinLng(Stock(i), (MaxStock(i) * Bullet(w) / (double)MaxBullet(w) + 0.49999d)));
            //                }
            //            }
            //        }
            //    }
            //}

            //// 共属性アビリティの処理
            //var loopTo3 = CountAbility();
            //for (a = 1; a <= loopTo3; a++)
            //{
            //    if (IsAbilityClassifiedAs(a, "共"))
            //    {
            //        lv = AbilityLevel(a, "共");
            //        // 使用回数を合わせる
            //        var loopTo4 = CountAbility();
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            if (a != i && IsAbilityClassifiedAs(i, "共") && lv == AbilityLevel(i, "共") && MaxStock(a) > 0)
            //            {
            //                if (MaxStock(i) > MaxStock(a))
            //                {
            //                    SetStock(i, GeneralLib.MinLng(Stock(i), (MaxStock(i) * Stock(a)) / MaxStock(a)));
            //                }
            //                else
            //                {
            //                    SetStock(i, GeneralLib.MinLng(Stock(i), (MaxStock(i) * Stock(a) / (double)MaxStock(a) + 0.49999d)));
            //                }
            //            }
            //        }
            //        // 弾数を合わせる
            //        var loopTo5 = CountWeapon();
            //        for (i = 1; i <= loopTo5; i++)
            //        {
            //            if (IsWeaponClassifiedAs(i, "共") && lv == WeaponLevel(i, "共") && MaxStock(a) > 0)
            //            {
            //                if (MaxBullet(i) > MaxStock(a))
            //                {
            //                    SetBullet(i, GeneralLib.MinLng(Bullet(i), (MaxBullet(i) * Stock(a)) / MaxStock(a)));
            //                }
            //                else
            //                {
            //                    SetBullet(i, GeneralLib.MinLng(Bullet(i), (MaxBullet(i) * Stock(a) / (double)MaxStock(a) + 0.49999d)));
            //                }
            //            }
            //        }
            //    }
            //}

            //// 斉属性武器の処理
            //var loopTo6 = CountWeapon();
            //for (w = 1; w <= loopTo6; w++)
            //{
            //    if (IsWeaponClassifiedAs(w, "斉"))
            //    {
            //        // 弾数を合わせる
            //        var loopTo7 = CountWeapon();
            //        for (i = 1; i <= loopTo7; i++)
            //        {
            //            if (w != i && MaxBullet(i) > 0)
            //            {
            //                SetBullet(w, GeneralLib.MinLng(Bullet(w), (MaxBullet(w) * Bullet(i) / (double)MaxBullet(i) + 0.49999d)));
            //            }
            //        }
            //    }
            //}

            //// 他の形態の弾数も変更
            //int counter;
            //var loopTo8 = CountOtherForm();
            //for (i = 1; i <= loopTo8; i++)
            //{
            //    {
            //        var withBlock = OtherForm(i);
            //        idx = 1;
            //        var loopTo9 = CountWeapon();
            //        for (j = 1; j <= loopTo9; j++)
            //        {
            //            counter = idx;
            //            var loopTo10 = withBlock.CountWeapon();
            //            for (k = counter; k <= loopTo10; k++)
            //            {
            //                if ((Weapon(j).Name ?? "") == (withBlock.Weapon(k).Name ?? "") && MaxBullet(j) > 0 && withBlock.MaxBullet(k) > 0)
            //                {
            //                    withBlock.SetBullet(k, ((withBlock.MaxBullet(k) * Bullet(j)) / MaxBullet(j)));
            //                    idx = (k + 1);
            //                    break;
            //                }
            //            }
            //        }

            //        idx = 1;
            //        var loopTo11 = CountAbility();
            //        for (j = 1; j <= loopTo11; j++)
            //        {
            //            counter = idx;
            //            var loopTo12 = withBlock.CountAbility();
            //            for (k = counter; k <= loopTo12; k++)
            //            {
            //                if ((Ability(j).Name ?? "") == (withBlock.Ability(k).Name ?? "") && MaxStock(j) > 0 && withBlock.MaxStock(k) > 0)
            //                {
            //                    withBlock.SetStock(k, ((withBlock.MaxStock(k) * Stock(j)) / MaxStock(j)));
            //                    idx = (k + 1);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
