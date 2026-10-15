using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
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

                // ステータス異常のみを消去
                foreach (var cnd in cf.Conditions.CloneList())
                {
                    var cname = cnd.Name ?? "";
                    if (cname == "残り時間"
                        || cname == "非操作"
                        || Strings.Right(cname, 2) == "付加"
                        || Strings.Right(cname, 2) == "強化"
                        || Strings.Right(cname, 3) == "付加２"
                        || Strings.Right(cname, 3) == "強化２"
                        || Strings.Right(cname, 2) == "ＵＰ")
                    {
                        continue;
                    }

                    cf.DeleteCondition(cname);
                }

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
                    foreach (var uw in of.Weapons)
                    {
                        uw.SetBulletFull();
                    }
                    foreach (var ua in of.Abilities)
                    {
                        ua.SetStockFull();
                    }
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
            var hp_recovery = 0;
            var en_recovery = 0;
            var hp_up = 0;
            var en_up = 0;
            var is_time_limit = false;
            var next_form = "";

            // 味方ステージの1ターン目(スタートイベント直後)は回復を行わない
            if (SRC.Stage == "味方" && SRC.Turn == 1)
            {
                return;
            }

            // データ更新
            Update();

            {
                var p = MainPilot();
                // 霊力回復
                if (p.MaxPlana() > 0)
                {
                    var plana_hp_ratio = 100 * HP / (double)MaxHP;
                    var plana_en_ratio = 100 * EN / (double)MaxEN;
                    p.Plana = (int)(p.Plana + p.MaxPlana() / 16
                        + (long)(p.MaxPlana() * FeatureLevel("霊力回復")) / 10L
                        - (long)(p.MaxPlana() * FeatureLevel("霊力消費")) / 10L);
                    HP = (int)((long)(MaxHP * plana_hp_ratio) / 100L);
                    EN = (int)((long)(MaxEN * plana_en_ratio) / 100L);
                }

                // ＳＰ回復
                RecoverSPByTurn(p);
            }

            // ＳＰ回復
            foreach (var p in SubPilots.ToList())
            {
                RecoverSPByTurn(p);
            }
            foreach (var p in Supports.ToList())
            {
                RecoverSPByTurn(p);
            }
            if (IsFeatureAvailable("追加サポート"))
            {
                RecoverSPByTurn(AdditionalSupport());
            }

            // 行動回数
            UsedAction = 0;

            // スペシャルパワー効果を解除
            RemoveSpecialPowerInEffect("ターン");

            // スペシャルパワー自動発動
            {
                var p = MainPilot();
                if (AutoUseSpecialPower(p))
                {
                    return;
                }

                if (IsConditionSatisfied("スペシャルパワー自動発動付加")
                    || IsConditionSatisfied("スペシャルパワー自動発動付加２"))
                {
                    var sdata = p.SkillData("スペシャルパワー自動発動");
                    var spname = GeneralLib.LIndex(sdata, 2);
                    if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(sdata, 3))
                        && !IsSpecialPowerInEffect(spname))
                    {
                        GUI.Center(x, y);
                        p.UseSpecialPower(spname, 0d);
                        if (Status == "他形態")
                        {
                            return;
                        }
                    }
                }
            }
            foreach (var p in SubPilots.ToList())
            {
                if (AutoUseSpecialPower(p))
                {
                    return;
                }
            }
            foreach (var p in Supports.ToList())
            {
                if (AutoUseSpecialPower(p))
                {
                    return;
                }
            }
            if (IsFeatureAvailable("追加サポート"))
            {
                if (AutoUseSpecialPower(AdditionalSupport()))
                {
                    return;
                }
            }

            // 起死回生
            {
                var p = MainPilot();
                if (p.IsSkillAvailable("起死回生")
                    && p.SP <= p.MaxSP / 5
                    && HP <= MaxHP / 5
                    && EN <= MaxEN / 5)
                {
                    p.SP = p.MaxSP;
                    HP = MaxHP;
                    EN = MaxEN;
                    if (SRC.SpecialPowerAnimation)
                    {
                        GUI.Center(x, y);
                        if (SRC.SPDList.IsDefined("ド根性"))
                        {
                            SRC.SPDList.Item("ド根性").PlayAnimation();
                        }
                    }
                }
            }

            // ＨＰとＥＮ回復＆消費
            if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能"))
            {
                if (IsFeatureAvailable("ＨＰ回復"))
                {
                    hp_recovery = (int)(10d * FeatureLevel("ＨＰ回復"));
                }

                if (IsFeatureAvailable("ＥＮ回復"))
                {
                    en_recovery = (int)(10d * FeatureLevel("ＥＮ回復"));
                }
            }

            if (IsFeatureAvailable("ＨＰ消費"))
            {
                hp_recovery = (int)(hp_recovery - 10d * FeatureLevel("ＨＰ消費"));
            }

            if (IsFeatureAvailable("ＥＮ消費"))
            {
                en_recovery = (int)(en_recovery - 10d * FeatureLevel("ＥＮ消費"));
            }

            // 毒によるＨＰ減少
            if (IsConditionSatisfied("毒"))
            {
                int plv;
                if (Expression.IsOptionDefined("毒効果大") && BossRank < 0)
                {
                    plv = 25;
                }
                else
                {
                    plv = 10;
                }

                if (Weakness("毒"))
                {
                    plv = 2 * plv;
                }
                else if (Effective("毒"))
                {
                    // 変化なし
                }
                else if (Immune("毒") || Absorb("毒"))
                {
                    plv = 0;
                }
                else if (Resist("毒"))
                {
                    plv = plv / 2;
                }

                hp_recovery = hp_recovery - plv;
            }

            // 活動限界時間切れによる退却は UpdateCondition で行う

            // 死の宣告
            if (ConditionLifetime("死の宣告") == 1)
            {
                hp_recovery = hp_recovery - 1000;
            }

            // 残り時間
            if (ConditionLifetime("残り時間") == 1)
            {
                is_time_limit = true;
                if (IsFeatureAvailable("ノーマルモード"))
                {
                    // ハイパーモード＆変身の時間切れの場合は戻り先の形態を記録しておく
                    next_form = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
                }
            }

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

            // 付加された移動能力が切れた場合の処理
            if (Status == "出撃" && !string.IsNullOrEmpty(Map.MapFileName))
            {
                bool is_trans_lost;
                switch (Area ?? "")
                {
                    case "空中":
                        is_trans_lost = !IsTransAvailable("空");
                        break;

                    case "地上":
                        is_trans_lost = !IsTransAvailable("陸");
                        break;

                    case "水上":
                        is_trans_lost = !IsFeatureAvailable("水上移動") && !IsFeatureAvailable("ホバー移動");
                        break;

                    case "水中":
                        is_trans_lost = get_Adaption(3) == 0;
                        break;

                    default:
                        is_trans_lost = false;
                        break;
                }

                if (is_trans_lost)
                {
                    if (!IsAbleToEnter(x, y))
                    {
                        ForceEscape();
                        return;
                    }

                    Map.MapDataForUnit[x, y] = null;
                    GUI.EraseUnitBitmap(x, y);
                    StandBy(x, y);
                }
            }

            if (Status == "格納")
            {
                // 格納時は回復率ＵＰ
                if (!IsConditionSatisfied("回復不能") && !IsSpecialPowerInEffect("回復不能"))
                {
                    hp_recovery = hp_recovery + 50;
                    en_recovery = en_recovery + 50;
                }

                // 弾数回復
                foreach (var uw in Weapons)
                {
                    uw.SetBulletFull();
                }
                foreach (var ua in Abilities)
                {
                    ua.SetStockFull();
                }
            }
            else
            {
                // 格納されてない場合は地形による各種修正＆状態付加
                var td = Map.Terrain(x, y);
                if (td != null)
                {
                    // 地形効果が適用される位置にいるかを判定
                    bool is_terrain_effective;
                    if (td.IsFeatureAvailable("効果範囲"))
                    {
                        is_terrain_effective = GeneralLib.ToL(td.Feature("効果範囲").Data).Contains(Area ?? "");
                    }
                    else
                    {
                        is_terrain_effective = true;
                    }

                    // 地形効果に対する無効化能力を持っているか
                    var is_immune_to_terrain_effect = false;
                    if (IsFeatureAvailable("地形効果無効化"))
                    {
                        var fdata = FeatureData("地形効果無効化");
                        if (GeneralLib.LLength(fdata) > 1)
                        {
                            is_immune_to_terrain_effect = GeneralLib.ToL(fdata).Skip(1).Contains(td.Name ?? "");
                        }
                        else
                        {
                            is_immune_to_terrain_effect = true;
                        }
                    }
                    else if (IsSpecialPowerInEffect("地形効果無効化"))
                    {
                        is_immune_to_terrain_effect = true;
                    }

                    // 地形効果を適用
                    if (is_terrain_effective)
                    {
                        foreach (var fd in td.colFeature.List)
                        {
                            var flevel = fd.FeatureLevel;
                            if (!IsConditionSatisfied("回復不能")
                                && !IsSpecialPowerInEffect("回復不能"))
                            {
                                switch (fd.Name ?? "")
                                {
                                    case "ＨＰ回復":
                                        hp_recovery = (int)(hp_recovery + 10 * flevel);
                                        break;

                                    case "ＥＮ回復":
                                        en_recovery = (int)(en_recovery + 10 * flevel);
                                        break;

                                    case "ＨＰ増加":
                                        hp_up = (int)(hp_up + 1000 * flevel);
                                        break;

                                    case "ＥＮ増加":
                                        en_up = (int)(en_up + 10 * flevel);
                                        break;
                                }
                            }

                            if (!is_immune_to_terrain_effect)
                            {
                                switch (fd.Name ?? "")
                                {
                                    case "ＨＰ減少":
                                        hp_recovery += TerrainDecreaseRate(fd.Data, 10 * flevel);
                                        break;

                                    case "ＥＮ減少":
                                        en_recovery += TerrainDecreaseRate(fd.Data, 10 * flevel);
                                        break;

                                    case "ＨＰ低下":
                                        hp_up += TerrainDecreaseRate(fd.Data, 1000 * flevel);
                                        break;

                                    case "ＥＮ低下":
                                        en_up += TerrainDecreaseRate(fd.Data, 10 * flevel);
                                        break;

                                    case "状態付加":
                                        {
                                            // 状態が無効化されるかチェック
                                            var cname = fd.Data;
                                            if (!string.IsNullOrEmpty(cname) && !IsImmuneToTerrainCondition(cname))
                                            {
                                                if (fd.HasLevel)
                                                {
                                                    AddCondition(cname, (int)fd.Level, cdata: "");
                                                }
                                                else
                                                {
                                                    AddCondition(cname, 10000, cdata: "");
                                                }
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }

            // ＥＮは毎ターン5回復
            if (!IsConditionSatisfied("回復不能")
                && !IsSpecialPowerInEffect("回復不能")
                && !Expression.IsOptionDefined("ＥＮ自然回復無効"))
            {
                EN = EN + 5;
            }

            // 算出した回復率を使ってＨＰを回復
            HP = HP + MaxHP * hp_recovery / 100 + hp_up;
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

            // 算出した回復率を使ってＥＮを回復
            if (EN + MaxEN * en_recovery / 100 + en_up > 0)
            {
                EN = EN + MaxEN * en_recovery / 100 + en_up;
            }
            // ＥＮが減少して０になる場合はハイパーモード解除もしくは行動不能
            else if (IsFeatureAvailable("ノーマルモード"))
            {
                // ただしノーマルモードに戻れない地形だとそのまま退却……
                var normal_form = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
                if (OtherForm(normal_form).IsAbleToEnter(x, y))
                {
                    Transform(normal_form);
                }
                else
                {
                    ForceEscape();
                    return;
                }
            }
            else if (IsFeatureAvailable("変形"))
            {
                // 変形できれば変形
                var is_transformed = false;
                foreach (var uname in GeneralLib.ToL(FeatureData("変形")).Skip(1))
                {
                    var of = OtherForm(uname);
                    if (of.IsAbleToEnter(x, y) && !of.IsFeatureAvailable("ＥＮ消費"))
                    {
                        Transform(uname);
                        is_transformed = true;
                        break;
                    }
                }

                if (!is_transformed)
                {
                    EN = 0;
                }
            }
            else
            {
                EN = 0;
            }

            // データ更新
            Update();

            // 時間切れ？
            if (is_time_limit)
            {
                if (!string.IsNullOrEmpty(next_form))
                {
                    // ハイパーモード＆変身の時間切れ
                    var u = OtherForm(next_form);
                    if (u.IsAbleToEnter(x, y))
                    {
                        // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                        if (u.IsFeatureAvailable("追加パイロット"))
                        {
                            if (!SRC.PList.IsDefined(u.FeatureData("追加パイロット")))
                            {
                                SRC.PList.Add(u.FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
                            }
                        }

                        // ノーマルモードメッセージ
                        foreach (var situation in new[] { "ノーマルモード(" + Name + "=>" + u.Name + ")", "ノーマルモード(" + u.Name + ")", "ノーマルモード" })
                        {
                            if (IsMessageDefined(situation))
                            {
                                GUI.OpenMessageForm(u1: null, u2: null);
                                PilotMessage(situation, msg_mode: "");
                                GUI.CloseMessageForm();
                                break;
                            }
                        }

                        // 特殊効果
                        foreach (var situation in new[] { "ノーマルモード(" + Name + "=>" + u.Name + ")", "ノーマルモード(" + u.Name + ")", "ノーマルモード" })
                        {
                            if (IsSpecialEffectDefined(situation))
                            {
                                SpecialEffect(situation);
                                break;
                            }
                        }

                        // 変形
                        Transform(GeneralLib.LIndex(FeatureData("ノーマルモード"), 1));
                    }
                    else
                    {
                        // 変形するとその地形にいれなくなる場合は退却
                        ForceEscape();
                        return;
                    }
                }
                else if (IsFeatureAvailable("分離"))
                {
                    // 合体時間切れ
                    var fname = FeatureName("分離");

                    // メッセージ表示
                    if (IsMessageDefined("分離(" + Name + ")")
                        || IsMessageDefined("分離(" + fname + ")")
                        || IsMessageDefined("分離"))
                    {
                        if (IsFeatureAvailable("分離ＢＧＭ"))
                        {
                            Sound.StartBGM(FeatureData("分離ＢＧＭ"));
                            GUI.Sleep(500);
                        }
                        else if (MainPilot().BGM != "-")
                        {
                            Sound.StartBGM(MainPilot().BGM);
                            GUI.Sleep(500);
                        }

                        GUI.Center(x, y);
                        GUI.RefreshScreen();
                        GUI.OpenMessageForm(u1: null, u2: null);
                        if (IsMessageDefined("分離(" + Name + ")"))
                        {
                            PilotMessage("分離(" + Name + ")", msg_mode: "");
                        }
                        else if (IsMessageDefined("分離(" + fname + ")"))
                        {
                            PilotMessage("分離(" + fname + ")", msg_mode: "");
                        }
                        else
                        {
                            PilotMessage("分離", msg_mode: "");
                        }

                        GUI.CloseMessageForm();
                    }

                    // 特殊効果
                    if (IsSpecialEffectDefined("分離(" + Name + ")"))
                    {
                        SpecialEffect("分離(" + Name + ")");
                    }
                    else if (IsSpecialEffectDefined("分離(" + fname + ")"))
                    {
                        SpecialEffect("分離(" + fname + ")");
                    }
                    else
                    {
                        SpecialEffect("分離");
                    }

                    // 分離
                    Split();
                }
                else
                {
                    // 制限時間切れ
                    GUI.Center(x, y);
                    GUI.RefreshScreen();
                    GUI.OpenMessageForm(u1: null, u2: null);
                    GUI.DisplaySysMessage(Nickname + "は制限時間切れのため退却します。");
                    GUI.CloseMessageForm();
                    Escape();
                    Event.HandleEvent("破壊", MainPilot().ID);
                    return;
                }
            }

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            CurrentForm().CheckAutoHyperMode();
            CurrentForm().CheckAutoNormalMode();
        }

        // ターン経過によるＳＰ回復
        private static void RecoverSPByTurn(Pilot p)
        {
            if (p == null)
            {
                return;
            }

            if (p.IsSkillAvailable("ＳＰ回復"))
            {
                p.SP = p.SP + p.Level / 8 + 5;
            }

            if (p.IsSkillAvailable("精神統一"))
            {
                if (p.SP < p.MaxSP / 5)
                {
                    p.SP = p.SP + p.MaxSP / 10;
                }
            }
        }

        // パイロット p の「スペシャルパワー自動発動」を処理
        // 発動の結果ユニットが他形態になった場合は true を返す
        private bool AutoUseSpecialPower(Pilot p)
        {
            if (p == null)
            {
                return false;
            }

            var loopTo = p.CountSkill();
            for (var i = 1; i <= loopTo; i++)
            {
                if (p.Skill(i) != "スペシャルパワー自動発動")
                {
                    continue;
                }

                var sdata = p.SkillData(i);
                var spname = GeneralLib.LIndex(sdata, 2);
                if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(sdata, 3))
                    && !IsSpecialPowerInEffect(spname))
                {
                    GUI.Center(x, y);
                    p.UseSpecialPower(spname, 0d);
                    if (Status == "他形態")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 強制退却
        private void ForceEscape()
        {
            GUI.Center(x, y);
            Escape();
            GUI.OpenMessageForm(u1: null, u2: null);
            GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
            GUI.CloseMessageForm();
            Event.HandleEvent("破壊", MainPilot().ID);
        }

        // 地形効果「ＨＰ減少」「ＥＮ減少」「ＨＰ低下」「ＥＮ低下」による変化量
        // rate は属性 attr に対して特に耐性等が無い場合の減少量
        private int TerrainDecreaseRate(string attr, double rate)
        {
            if (Weakness(attr))
            {
                return (int)(-2d * rate);
            }
            else if (Effective(attr))
            {
                return (int)-rate;
            }
            else if (Absorb(attr))
            {
                return (int)rate;
            }
            else if (Resist(attr))
            {
                return (int)(-rate / 2d);
            }
            else if (!Immune(attr))
            {
                return (int)-rate;
            }

            return 0;
        }

        // 地形効果「状態付加」による状態 cname が無効化されるか
        private bool IsImmuneToTerrainCondition(string cname)
        {
            switch (cname ?? "")
            {
                case "装甲劣化": return SpecialEffectImmune("劣");
                case "混乱": return SpecialEffectImmune("乱");
                case "恐怖": return SpecialEffectImmune("恐");
                case "踊り": return SpecialEffectImmune("踊");
                case "狂戦士": return SpecialEffectImmune("狂");
                case "ゾンビ": return SpecialEffectImmune("ゾ");
                case "回復不能": return SpecialEffectImmune("害");
                case "石化": return SpecialEffectImmune("石");
                case "凍結": return SpecialEffectImmune("凍");
                case "麻痺": return SpecialEffectImmune("痺");
                case "睡眠": return SpecialEffectImmune("眠");
                case "毒": return SpecialEffectImmune("毒");
                case "盲目": return SpecialEffectImmune("盲");
                case "沈黙": return SpecialEffectImmune("黙");
                // 属性使用不能状態
                case "オーラ使用不能": return SpecialEffectImmune("剋オ");
                case "超能力使用不能": return SpecialEffectImmune("剋超");
                case "同調率使用不能": return SpecialEffectImmune("剋シ");
                case "超感覚使用不能": return SpecialEffectImmune("剋サ");
                case "知覚強化使用不能": return SpecialEffectImmune("剋サ");
                case "霊力使用不能": return SpecialEffectImmune("剋霊");
                case "術使用不能": return SpecialEffectImmune("剋術");
                case "技使用不能": return SpecialEffectImmune("剋技");
            }

            if (Strings.Len(cname) > 6)
            {
                var attr = Strings.Left(cname, Strings.Len(cname) - 6);
                switch (Strings.Right(cname, 6))
                {
                    case "属性弱点付加":
                        return SpecialEffectImmune("弱" + attr) || Absorb(attr) || Immune(attr);

                    case "属性有効付加":
                        return SpecialEffectImmune("有" + attr) || Absorb(attr) || Immune(attr);

                    case "属性使用不能":
                        return SpecialEffectImmune("剋" + attr);
                }
            }

            return false;
        }

        // ハイパーモードの自動発動チェック
        public void CheckAutoHyperMode()
        {
            // ハイパーモードが自動発動するか判定

            if (Status != "出撃")
            {
                return;
            }

            if (!IsFeatureAvailable("ハイパーモード"))
            {
                return;
            }

            var fname = FeatureName("ハイパーモード");
            var flevel = FeatureLevel("ハイパーモード");
            var fdata = FeatureData("ハイパーモード");

            if (Strings.InStr(fdata, "自動発動") == 0)
            {
                return;
            }

            // 発動条件を満たす？
            if (MainPilot().Morale < (int)Math.Round(10d * flevel) + 100
                && (HP > MaxHP / 4 || Strings.InStr(fdata, "気力発動") > 0))
            {
                return;
            }

            // 変身中・能力コピー中はハイパーモードを使用できない
            if (IsConditionSatisfied("ノーマルモード付加"))
            {
                return;
            }

            // ハイパーモード先の形態が利用可能？
            var uname = GeneralLib.LIndex(fdata, 2);
            var is_available = false;
            {
                var of = OtherForm(uname);
                switch (Map.Terrain(x, y)?.Class ?? "")
                {
                    case "空":
                        if (of.IsTransAvailable("空"))
                        {
                            is_available = true;
                        }

                        break;

                    case "深水":
                        if (of.IsTransAvailable("空")
                            || of.IsTransAvailable("水")
                            || of.IsTransAvailable("水上"))
                        {
                            is_available = true;
                        }

                        break;

                    default:
                        is_available = true;
                        break;
                }

                if (!of.IsAbleToEnter(x, y))
                {
                    is_available = false;
                }
            }

            // 自動発動する？
            if (!is_available)
            {
                return;
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            CreateAdditionalPilotForMessage(uname);

            // ＢＧＭを切り替え
            if (IsFeatureAvailable("ハイパーモードＢＧＭ"))
            {
                StartTransformBGM("ハイパーモードＢＧＭ", uname);
            }

            // メッセージを表示
            var message_situations = new[]
            {
                "ハイパーモード(" + Name + "=>" + uname + ")",
                "ハイパーモード(" + uname + ")",
                "ハイパーモード(" + fname + ")",
                "ハイパーモード",
            };
            var message_situation = message_situations.FirstOrDefault(s => IsMessageDefined(s));
            if (message_situation != null)
            {
                GUI.Center(x, y);
                GUI.RefreshScreen();
                GUI.OpenMessageForm(u1: null, u2: null);
                PilotMessage(message_situation, msg_mode: "");
                GUI.CloseMessageForm();
            }

            // 特殊効果
            Commands.SaveSelections();
            Commands.SelectedUnit = this;
            Event.SelectedUnitForEvent = this;
            Commands.SelectedTarget = null;
            Event.SelectedTargetForEvent = null;
            var anime_situation = new[]
            {
                "ハイパーモード(" + Name + "=>" + uname + ")",
                "ハイパーモード(" + uname + ")",
                "ハイパーモード(" + FeatureName("ハイパーモード") + ")",
                "ハイパーモード",
            }.FirstOrDefault(s => IsAnimationDefined(s));
            if (anime_situation != null)
            {
                PlayAnimation(anime_situation);
            }
            else
            {
                SpecialEffect(message_situations.Take(3).FirstOrDefault(s => IsSpecialEffectDefined(s)) ?? "ハイパーモード");
            }
            Commands.RestoreSelections();

            // ハイパーモードに変形
            Transform(uname);

            // ユニット変数を置き換え
            ReplaceSelectionsWithCurrentForm();

            // 変形イベント
            {
                var cf = CurrentForm();
                Event.HandleEvent("変形", cf.MainPilot().ID, cf.Name);
            }
        }

        // ノーマルモードの自動発動チェック
        public bool CheckAutoNormalMode(bool without_redraw = false)
        {
            var CheckAutoNormalModeRet = false;

            // ノーマルモードが自動発動するか判定

            if (Status != "出撃")
            {
                return false;
            }

            if (!IsFeatureAvailable("ノーマルモード"))
            {
                return false;
            }

            // まだ元の形態でもＯＫ？
            if (IsAbleToEnter(x, y))
            {
                return false;
            }

            // ノーマルモード先が利用可能？
            var uname = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
            if (!OtherForm(uname).IsAbleToEnter(x, y))
            {
                return false;
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            CreateAdditionalPilotForMessage(uname);

            // メッセージを表示
            var situations = new[]
            {
                "ノーマルモード(" + Name + "=>" + uname + ")",
                "ノーマルモード(" + uname + ")",
                "ノーマルモード",
            };
            var message_situation = situations.FirstOrDefault(s => IsMessageDefined(s));
            if (message_situation != null)
            {
                // ＢＧＭを切り替え
                if (IsFeatureAvailable("ノーマルモードＢＧＭ"))
                {
                    StartTransformBGM("ノーマルモードＢＧＭ", uname);
                }

                GUI.Center(x, y);
                GUI.RefreshScreen();
                GUI.OpenMessageForm(u1: null, u2: null);
                PilotMessage(message_situation, msg_mode: "");
                GUI.CloseMessageForm();
            }

            // 特殊効果
            Commands.SaveSelections();
            Commands.SelectedUnit = this;
            Event.SelectedUnitForEvent = this;
            Commands.SelectedTarget = null;
            Event.SelectedTargetForEvent = null;
            var anime_situation = situations.FirstOrDefault(s => IsAnimationDefined(s));
            if (anime_situation != null)
            {
                PlayAnimation(anime_situation);
            }
            else
            {
                SpecialEffect(situations.Take(2).FirstOrDefault(s => IsSpecialEffectDefined(s)) ?? "ノーマルモード");
            }
            Commands.RestoreSelections();

            // ノーマルモードに変形
            Transform(uname);

            // ユニット変数を置き換え
            ReplaceSelectionsWithCurrentForm();

            // 画面の再描画が必要？
            if (CurrentForm().IsConditionSatisfied("消耗"))
            {
                CheckAutoNormalModeRet = true;
                if (!without_redraw)
                {
                    GUI.RedrawScreen();
                }
            }

            // 変形イベント
            {
                var cf = CurrentForm();
                Event.HandleEvent("変形", cf.MainPilot().ID, cf.Name);
            }

            return CheckAutoNormalModeRet;
        }

        // 変形先 uname で表示するメッセージのために追加パイロットをあらかじめ作成
        private void CreateAdditionalPilotForMessage(string uname)
        {
            if (!SRC.UDList.IsDefined(uname))
            {
                return;
            }

            if (IsFeatureAvailable("追加パイロット"))
            {
                if (!SRC.PList.IsDefined(FeatureData("追加パイロット")))
                {
                    SRC.PList.Add(FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
                }
            }
        }

        // 変形先 uname 用に指定されたＢＧＭへ切り替え
        private void StartTransformBGM(string bgm_feature, string uname)
        {
            foreach (var fd in Features)
            {
                if (fd.Name == bgm_feature && GeneralLib.LIndex(fd.Data, 1) == uname)
                {
                    Sound.StartBGM(Strings.Mid(fd.Data, Strings.InStr(fd.Data, " ") + 1));
                    GUI.Sleep(500);
                    break;
                }
            }
        }

        // 選択中のユニット変数を変形後の形態に置き換え
        private void ReplaceSelectionsWithCurrentForm()
        {
            if (Commands.SelectedUnit is object && ID == Commands.SelectedUnit.ID)
            {
                Commands.SelectedUnit = CurrentForm();
            }

            if (Event.SelectedUnitForEvent is object && ID == Event.SelectedUnitForEvent.ID)
            {
                Event.SelectedUnitForEvent = CurrentForm();
            }

            if (Commands.SelectedTarget is object && ID == Commands.SelectedTarget.ID)
            {
                Commands.SelectedTarget = CurrentForm();
            }

            if (Event.SelectedTargetForEvent is object && ID == Event.SelectedTargetForEvent.ID)
            {
                Event.SelectedTargetForEvent = CurrentForm();
            }
        }

        // データをリセット
        public void Reset()
        {
            ClearCondition();
            RemoveAllSpecialPowerInEffect();
            Update();
            foreach (var p in Pilots.ToList())
            {
                p.FullRecover();
            }
            foreach (var p in Supports.ToList())
            {
                p.FullRecover();
            }
            foreach (var feature in new[] { "追加パイロット", "追加サポート" })
            {
                if (IsFeatureAvailable(feature))
                {
                    var pname = FeatureData(feature);
                    if (SRC.PList.IsDefined(pname))
                    {
                        SRC.PList.Item(pname).FullRecover();
                    }
                }
            }

            HP = MaxHP;
            FullSupply();
            Mode = "通常";
        }


        // 弾数・使用回数共有の処理
        public void SyncBullet()
        {
            // 共属性武器の処理
            foreach (var ww in Weapons)
            {
                if (!ww.IsWeaponClassifiedAs("共"))
                {
                    continue;
                }

                var lv = (int)ww.WeaponLevel("共");
                // 弾数を合わせる
                foreach (var wi in Weapons)
                {
                    if (!ReferenceEquals(ww, wi) && wi.IsWeaponClassifiedAs("共") && lv == (int)wi.WeaponLevel("共") && ww.MaxBullet() > 0)
                    {
                        if (wi.MaxBullet() > ww.MaxBullet())
                        {
                            wi.SetBullet(GeneralLib.MinLng(wi.Bullet(), (wi.MaxBullet() * ww.Bullet()) / ww.MaxBullet()));
                        }
                        else
                        {
                            wi.SetBullet(GeneralLib.MinLng(wi.Bullet(), VBMath.CLng(wi.MaxBullet() * ww.Bullet() / (double)ww.MaxBullet() + 0.49999d)));
                        }
                    }
                }

                // アビリティの使用回数を合わせる
                foreach (var ai in Abilities)
                {
                    if (ai.IsAbilityClassifiedAs("共") && lv == (int)ai.AbilityLevel("共") && ww.MaxBullet() > 0)
                    {
                        if (ai.MaxStock() > ww.MaxBullet())
                        {
                            ai.SetStock(GeneralLib.MinLng(ai.Stock(), (ai.MaxStock() * ww.Bullet()) / ww.MaxBullet()));
                        }
                        else
                        {
                            ai.SetStock(GeneralLib.MinLng(ai.Stock(), VBMath.CLng(ai.MaxStock() * ww.Bullet() / (double)ww.MaxBullet() + 0.49999d)));
                        }
                    }
                }
            }

            // 共属性アビリティの処理
            foreach (var aa in Abilities)
            {
                if (!aa.IsAbilityClassifiedAs("共"))
                {
                    continue;
                }

                var lv = (int)aa.AbilityLevel("共");
                // 使用回数を合わせる
                foreach (var ai in Abilities)
                {
                    if (!ReferenceEquals(aa, ai) && ai.IsAbilityClassifiedAs("共") && lv == (int)ai.AbilityLevel("共") && aa.MaxStock() > 0)
                    {
                        if (ai.MaxStock() > aa.MaxStock())
                        {
                            ai.SetStock(GeneralLib.MinLng(ai.Stock(), (ai.MaxStock() * aa.Stock()) / aa.MaxStock()));
                        }
                        else
                        {
                            ai.SetStock(GeneralLib.MinLng(ai.Stock(), VBMath.CLng(ai.MaxStock() * aa.Stock() / (double)aa.MaxStock() + 0.49999d)));
                        }
                    }
                }

                // 弾数を合わせる
                foreach (var wi in Weapons)
                {
                    if (wi.IsWeaponClassifiedAs("共") && lv == (int)wi.WeaponLevel("共") && aa.MaxStock() > 0)
                    {
                        if (wi.MaxBullet() > aa.MaxStock())
                        {
                            wi.SetBullet(GeneralLib.MinLng(wi.Bullet(), (wi.MaxBullet() * aa.Stock()) / aa.MaxStock()));
                        }
                        else
                        {
                            wi.SetBullet(GeneralLib.MinLng(wi.Bullet(), VBMath.CLng(wi.MaxBullet() * aa.Stock() / (double)aa.MaxStock() + 0.49999d)));
                        }
                    }
                }
            }

            // 斉属性武器の処理
            foreach (var ww in Weapons)
            {
                if (!ww.IsWeaponClassifiedAs("斉"))
                {
                    continue;
                }

                // 弾数を合わせる
                foreach (var wi in Weapons)
                {
                    if (!ReferenceEquals(ww, wi) && wi.MaxBullet() > 0)
                    {
                        ww.SetBullet(GeneralLib.MinLng(ww.Bullet(), VBMath.CLng(ww.MaxBullet() * wi.Bullet() / (double)wi.MaxBullet() + 0.49999d)));
                    }
                }
            }

            // 他の形態の弾数も変更
            foreach (var of in OtherForms)
            {
                foreach (var wj in Weapons)
                {
                    if (wj.MaxBullet() <= 0)
                    {
                        continue;
                    }

                    foreach (var wk in of.Weapons)
                    {
                        if ((wj.Name ?? "") == (wk.Name ?? "") && wk.MaxBullet() > 0)
                        {
                            wk.SetBullet((wk.MaxBullet() * wj.Bullet()) / wj.MaxBullet());
                            break;
                        }
                    }
                }

                foreach (var aj in Abilities)
                {
                    if (aj.MaxStock() <= 0)
                    {
                        continue;
                    }

                    foreach (var ak in of.Abilities)
                    {
                        if ((aj.Data.Name ?? "") == (ak.Data.Name ?? "") && ak.MaxStock() > 0)
                        {
                            ak.SetStock((ak.MaxStock() * aj.Stock()) / aj.MaxStock());
                            break;
                        }
                    }
                }
            }
        }
    }
}
