// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「攻撃」コマンドを開始
        private void StartAttackCommand()
        {
            LogDebug();

            int i, j;
            Unit t;
            int min_range, max_range;
            var BGM = default(string);
            GUI.LockGUI();
            var partners = default(Unit[]);
            var currentUnit = SelectedUnit;
            //// ＢＧＭの設定
            //string argfname = "ＢＧＭ";
            //if (currentUnit.IsFeatureAvailable(argfname))
            //{
            //    object argIndex1 = "ＢＧＭ";
            //    string argmidi_name = currentUnit.FeatureData(argIndex1);
            //    BGM = Sound.SearchMidiFile(argmidi_name);
            //}

            //if (Strings.Len(BGM) == 0)
            //{
            //    string argmidi_name1 = currentUnit.MainPilot().BGM;
            //    BGM = Sound.SearchMidiFile(argmidi_name1);
            //    currentUnit.MainPilot().BGM = argmidi_name1;
            //}

            //if (Strings.Len(BGM) == 0)
            //{
            //    string argbgm_name = "default";
            //    BGM = Sound.BGMName(argbgm_name);
            //}

            // 武器の選択
            UseSupportAttack = true;
            if (CommandState == "コマンド選択")
            {
                string argcaption_msg = "武器選択";
                string arglb_mode = "移動前";
                // TODO SelectedUnit.Weapons のフィルタ
                SelectedWeapon = GUI.WeaponListBox(SelectedUnit, SelectedUnit.Weapons, argcaption_msg, arglb_mode, BGM);
            }
            else
            {
                string argcaption_msg1 = "武器選択";
                string arglb_mode1 = "移動後";
                // TODO SelectedUnit.Weapons のフィルタ
                SelectedWeapon = GUI.WeaponListBox(SelectedUnit, SelectedUnit.Weapons, argcaption_msg1, arglb_mode1, BGM);
            }

            // キャンセル
            if (SelectedWeapon == 0)
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }

                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            var currentWeapon = currentUnit.Weapon(SelectedWeapon);

            //// 武器ＢＧＭの演奏
            //string argfname1 = "武器ＢＧＭ";
            //if (currentUnit.IsFeatureAvailable(argfname1))
            //{
            //    var loopTo = currentUnit.CountFeature();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string localFeature() { object argIndex1 = i; var ret = currentUnit.Feature(argIndex1); return ret; }

            //        string localFeatureData2() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //        string localLIndex() { string arglist = hs644c0746afde449eb03788895da90548(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //        if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (currentUnit.Weapon(SelectedWeapon).Name ?? ""))
            //        {
            //            string localFeatureData() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //            string localFeatureData1() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //            string argmidi_name2 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
            //            BGM = Sound.SearchMidiFile(argmidi_name2);
            //            if (Strings.Len(BGM) > 0)
            //            {
            //                Sound.ChangeBGM(BGM);
            //            }

            //            break;
            //        }
            //    }
            //}

            //// 選択した武器の種類により、この後のコマンドの進行の仕方が異なる
            //string argattr = "Ｍ";
            //if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr))
            //{
            //    SelectedCommand = "マップ攻撃";
            //}
            //else
            //{
            //    SelectedCommand = "攻撃";
            //}

            // 武器の射程を求めておく
            min_range = currentWeapon.WeaponMinRange();
            max_range = currentWeapon.WeaponMaxRange();

            // 攻撃範囲の表示
            //string argattr2 = "Ｍ直";
            //string argattr3 = "Ｍ拡";
            //string argattr4 = "Ｍ扇";
            //string argattr5 = "Ｍ全";
            //string argattr6 = "Ｍ投";
            //string argattr7 = "Ｍ線";
            //string argattr8 = "Ｍ移";
            //if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr2))
            //{
            //    Map.AreaInCross(currentUnit.x, currentUnit.y, min_range, max_range);
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr3))
            //{
            //    Map.AreaInWideCross(currentUnit.x, currentUnit.y, min_range, max_range);
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr4))
            //{
            //    string argattr1 = "Ｍ扇";
            //    Map.AreaInSectorCross(currentUnit.x, currentUnit.y, min_range, max_range, currentUnit.WeaponLevel(SelectedWeapon, argattr1));
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr5) | currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr6) | currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr7))
            //{
            //    string arguparty1 = "すべて";
            //    Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, arguparty1);
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr8))
            //{
            //    Map.AreaInMoveAction(SelectedUnit, max_range);
            //}
            //else
            {
                Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, "味方の敵");
            }

            //// 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
            //string argattr9 = "合";
            //string argattr10 = "Ｍ";
            //if (max_range == 1 & currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr9) & !currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr10))
            //{
            //    for (i = 1; i <= 4; i++)
            //    {
            //        t = null;
            //        switch (i)
            //        {
            //            case 1:
            //                {
            //                    if (currentUnit.x > 1)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    if (currentUnit.x < Map.MapWidth)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
            //                    }

            //                    break;
            //                }

            //            case 3:
            //                {
            //                    if (currentUnit.y > 1)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
            //                    }

            //                    break;
            //                }

            //            case 4:
            //                {
            //                    if (currentUnit.y < Map.MapHeight)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
            //                    }

            //                    break;
            //                }
            //        }

            //        if (t is object)
            //        {
            //            if (currentUnit.IsEnemy(t))
            //            {
            //                string argctype_Renamed = "武装";
            //                currentUnit.CombinationPartner(argctype_Renamed, SelectedWeapon, partners, t.x, t.y);
            //                if (Information.UBound(partners) == 0)
            //                {
            //                    Map.MaskData[t.x, t.y] = true;
            //                }
            //            }
            //        }
            //    }
            //}

            //// ユニットに対するマスクの設定
            //string argattr15 = "Ｍ投";
            //string argattr16 = "Ｍ線";
            //if (!currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr15) & !currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr16))
            //{
            //    var loopTo1 = GeneralLib.MinLng(currentUnit.x + max_range, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(currentUnit.x - max_range, 1); i <= loopTo1; i++)
            //    {
            //        var loopTo2 = GeneralLib.MinLng(currentUnit.y + max_range, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(currentUnit.y - max_range, 1); j <= loopTo2; j++)
            //        {
            //            if (Map.MaskData[i, j])
            //            {
            //                goto NextLoop;
            //            }

            //            t = Map.MapDataForUnit[i, j];
            //            if (t is null)
            //            {
            //                goto NextLoop;
            //            }

            //            // 武器の地形適応が有効？
            //            if (currentUnit.WeaponAdaption(SelectedWeapon, t.Area) == 0d)
            //            {
            //                Map.MaskData[i, j] = true;
            //                goto NextLoop;
            //            }

            //            // 封印武器の対象属性外でない？
            //            string argattr11 = "封";
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr11))
            //            {
            //                if (currentUnit.Weapon(SelectedWeapon).Power > 0 & currentUnit.Damage(SelectedWeapon, t, true) == 0 | currentUnit.CriticalProbability(SelectedWeapon, t) == 0)
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // 限定武器の対象属性外でない？
            //            string argattr12 = "限";
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr12))
            //            {
            //                if (currentUnit.Weapon(SelectedWeapon).Power > 0 & currentUnit.Damage(SelectedWeapon, t, true) == 0 | currentUnit.Weapon(SelectedWeapon).Power == 0 & currentUnit.CriticalProbability(SelectedWeapon, t) == 0)
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // 識別攻撃の場合の処理
            //            string argattr13 = "識";
            //            string argsptype = "識別攻撃";
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr13) | currentUnit.IsUnderSpecialPowerEffect(argsptype))
            //            {
            //                if (currentUnit.IsAlly(t))
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // ステルス＆隠れ身チェック
            //            string argattr14 = "Ｍ";
            //            if (!currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr14))
            //            {
            //                if (!currentUnit.IsTargetWithinRange(SelectedWeapon, t))
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //        NextLoop:
            //            ;
            //        }
            //    }
            //}

            //Map.MaskData[currentUnit.x, currentUnit.y] = false;
            //string argoname = "大型マップ";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    GUI.Center(currentUnit.x, currentUnit.y);
            //}

            GUI.MaskScreen();

            // ターゲット選択へ
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            //// カーソル自動移動を行う？
            //if (!SRC.AutoMoveCursor)
            //{
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// ＨＰがもっとも低いターゲットを探す
            //t = null;
            //foreach (Unit u in SRC.UList)
            //{
            //    if (u.Status == "出撃" & (u.Party == "敵" | u.Party == "中立"))
            //    {
            //        if (Map.MaskData[u.x, u.y] == false)
            //        {
            //            if (t is null)
            //            {
            //                t = u;
            //            }
            //            else if (u.HP < t.HP)
            //            {
            //                t = u;
            //            }
            //            else if (u.HP == t.HP)
            //            {
            //                if (Math.Pow(Math.Abs((SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - t.y)), 2d))
            //                {
            //                    t = u;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 適当なターゲットが見つからなければ自分自身を選択
            //if (t is null)
            //{
            //    t = SelectedUnit;
            //}

            //// カーソルを移動
            //string argcursor_mode = "ユニット選択";
            //GUI.MoveCursorPos(argcursor_mode, t);

            //// ターゲットのステータスを表示
            //if (!ReferenceEquals(SelectedUnit, t))
            //{
            //    Status.DisplayUnitStatus(t);
            //}

            GUI.UnlockGUI();
        }

        // 「攻撃」コマンドを終了
        private void FinishAttackCommand()
        {
            LogDebug();

            int i;
            var earnings = false;
            string def_mode = "";
            var partners = default(Unit[]);
            var BGM = default(string);
            var is_suiside = default(bool);
            string wname, twname = default;
            int tx, ty;
            Unit attack_target;
            double attack_target_hp_ratio;
            Unit defense_target;
            double defense_target_hp_ratio;
            Unit defense_target2;
            var defense_target2_hp_ratio = default(double);
            var support_attack_done = default(bool);
            int w2;
            bool is_p_weapon;
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            var currentUnit = SelectedUnit;
            var currentWeapon = currentUnit.Weapon(SelectedWeapon);
            wname = currentWeapon.Name;
            SelectedWeaponName = wname;

            // 移動後使用後可能な武器か記録しておく
            is_p_weapon = currentWeapon.IsWeaponClassifiedAs("移動後攻撃可");

            //// 合体技のパートナーを設定
            //{
            //    var withBlock = SelectedUnit;
            //    string argattr1 = "合";
            //    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr1))
            //    {
            //        if (withBlock.WeaponMaxRange(SelectedWeapon) == 1)
            //        {
            //            string argctype_Renamed = "武装";
            //            withBlock.CombinationPartner(argctype_Renamed, SelectedWeapon, partners, SelectedTarget.x, SelectedTarget.y);
            //        }
            //        else
            //        {
            //            string argctype_Renamed1 = "武装";
            //            withBlock.CombinationPartner(argctype_Renamed1, SelectedWeapon, partners);
            //        }
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[1];
            //        partners = new Unit[1];
            //    }
            //}

            // 敵の反撃手段を設定
            UnitWeapon currentTWeapon = null;
            UseSupportGuard = true;
            int argmax_prob = 0;
            int argmax_dmg = 0;
            //SelectedTWeapon = COM.SelectWeapon(SelectedTarget, SelectedUnit, "反撃", max_prob: argmax_prob, max_dmg: argmax_dmg);
            //if (currentWeapon.IsWeaponClassifiedAs("間"))
            //{
            //    SelectedTWeapon = 0;
            //}

            //if (SelectedTWeapon > 0)
            //{
            //    currentTWeapon = SelectedTarget.Weapon(SelectedTWeapon);
            //    twname = currentTWeapon.Name;
            //    SelectedTWeaponName = twname;
            //}
            //else
            //{
            //    SelectedTWeaponName = "";
            //}

            //// 敵の防御行動を設定
            //// UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //def_mode = Conversions.ToString(COM.SelectDefense(SelectedUnit, SelectedWeapon, SelectedTarget, SelectedTWeapon));
            //if (!string.IsNullOrEmpty(def_mode))
            //{
            //    if (SelectedTWeapon > 0)
            //    {
            //        SelectedTWeapon = -1;
            //    }
            //}

            //SelectedDefenseOption = def_mode;

            // 戦闘前に一旦クリア
            SupportAttackUnit = null;
            SupportGuardUnit = null;
            SupportGuardUnit2 = null;

            //// 攻撃側の武器使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, wname);
            //if (SRC.IsScenarioFinished)
            //{
            //    GUI.UnlockGUI();
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    WaitCommand();
            //    return;
            //}

            //// 敵の武器使用イベント
            //if (SelectedTWeapon > 0)
            //{
            //    SaveSelections();
            //    SwapSelections();
            //    Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, twname);
            //    RestoreSelections();
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[1];
            //        WaitCommand();
            //        return;
            //    }
            //}

            //// 攻撃イベント
            //Event_Renamed.HandleEvent("攻撃", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners = new Unit[1];
            //    WaitCommand();
            //    return;
            //}

            //// 敵がＢＧＭ能力を持つ場合はＢＧＭを変更
            //{
            //    var withBlock1 = SelectedTarget;
            //    string argfname = "ＢＧＭ";
            //    if (withBlock1.IsFeatureAvailable(argfname) & Strings.InStr(withBlock1.MainPilot().Name, "(ザコ)") == 0)
            //    {
            //        object argIndex1 = "ＢＧＭ";
            //        string argmidi_name = withBlock1.FeatureData(argIndex1);
            //        BGM = Sound.SearchMidiFile(argmidi_name);
            //        if (Strings.Len(BGM) > 0)
            //        {
            //            Sound.BossBGM = false;
            //            Sound.ChangeBGM(BGM);
            //            Sound.BossBGM = true;
            //        }
            //    }
            //}

            //// そうではなく、ボス用ＢＧＭが流れていれば味方のＢＧＭに切り替え
            //if (Strings.Len(BGM) == 0 & Sound.BossBGM)
            //{
            //    Sound.BossBGM = false;
            //    BGM = "";
            //    {
            //        var withBlock2 = SelectedUnit;
            //        string argfname1 = "武器ＢＧＭ";
            //        if (withBlock2.IsFeatureAvailable(argfname1))
            //        {
            //            var loopTo = withBlock2.CountFeature();
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                string localFeature() { object argIndex1 = i; var ret = withBlock2.Feature(argIndex1); return ret; }

            //                string localFeatureData2() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                string localLIndex() { string arglist = hs51a192742c114976b44a246d492333eb(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (withBlock2.Weapon(SelectedWeapon).Name ?? ""))
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                    string localFeatureData1() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                    string argmidi_name1 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
            //                    BGM = Sound.SearchMidiFile(argmidi_name1);
            //                    break;
            //                }
            //            }
            //        }

            //        if (Strings.Len(BGM) == 0)
            //        {
            //            string argfname2 = "ＢＧＭ";
            //            if (withBlock2.IsFeatureAvailable(argfname2))
            //            {
            //                object argIndex2 = "ＢＧＭ";
            //                string argmidi_name2 = withBlock2.FeatureData(argIndex2);
            //                BGM = Sound.SearchMidiFile(argmidi_name2);
            //            }
            //        }

            //        if (Strings.Len(BGM) == 0)
            //        {
            //            string argmidi_name3 = withBlock2.MainPilot().BGM;
            //            BGM = Sound.SearchMidiFile(argmidi_name3);
            //            withBlock2.MainPilot().BGM = argmidi_name3;
            //        }

            //        if (Strings.Len(BGM) == 0)
            //        {
            //            string argbgm_name = "default";
            //            BGM = Sound.BGMName(argbgm_name);
            //        }

            //        Sound.ChangeBGM(BGM);
            //    }
            //}

            //{
            //    var withBlock3 = SelectedUnit;
            //    // 攻撃参加ユニット以外はマスク
            //    foreach (Unit u in SRC.UList)
            //    {
            //        if (u.Status == "出撃")
            //        {
            //            Map.MaskData[u.x, u.y] = true;
            //        }
            //    }

            //    // 合体技パートナーのハイライト表示
            //    var loopTo1 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        {
            //            var withBlock4 = partners[i];
            //            Map.MaskData[withBlock4.x, withBlock4.y] = false;
            //        }
            //    }

            //    Map.MaskData[withBlock3.x, withBlock3.y] = false;
            //    Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
            //    if (!SRC.BattleAnimation)
            //    {
            //        GUI.MaskScreen();
            //    }
            //}

            //// イベント用に戦闘に参加するユニットの情報を記録しておく
            //AttackUnit = SelectedUnit;
            //attack_target = SelectedUnit;
            //attack_target_hp_ratio = SelectedUnit.HP / (double)SelectedUnit.MaxHP;
            //defense_target = SelectedTarget;
            //defense_target_hp_ratio = SelectedTarget.HP / (double)SelectedTarget.MaxHP;
            //defense_target2 = null;
            //SupportAttackUnit = null;
            //SupportGuardUnit = null;
            //SupportGuardUnit2 = null;

            //// ターゲットの位置を記録
            //tx = SelectedTarget.x;
            //ty = SelectedTarget.y;
            //GUI.OpenMessageForm(SelectedTarget, SelectedUnit);

            //// 相手の先制攻撃？
            //{
            //    var withBlock5 = SelectedTarget;
            //    // MOD START MARGE
            //    // If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "移動前") Then
            //    // SelectedTWeapon > 0の判定は、IsWeaponAvailableで行うようにした
            //    string argref_mode1 = "移動前";
            //    if (withBlock5.MaxAction() > 0 & withBlock5.IsWeaponAvailable(SelectedTWeapon, argref_mode1))
            //    {
            //        // MOD END MARGE
            //        string argattr8 = "後";
            //        if (!withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, argattr8))
            //        {
            //            string argattr6 = "後";
            //            string argattr7 = "先";
            //            object argIndex3 = "先読み";
            //            string argref_mode = "";
            //            string argsptype = "カウンター";
            //            if (SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr6))
            //            {
            //                def_mode = "先制攻撃";
            //                string argattr3 = "自";
            //                if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, argattr3))
            //                {
            //                    is_suiside = true;
            //                }

            //                // 先制攻撃攻撃を実施
            //                withBlock5.Attack(SelectedTWeapon, SelectedUnit, "先制攻撃", "");
            //                SelectedTarget = withBlock5.CurrentForm();
            //            }
            //            else if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, argattr7) | withBlock5.MainPilot().SkillLevel(argIndex3, ref_mode: argref_mode) >= GeneralLib.Dice(16) | withBlock5.IsUnderSpecialPowerEffect(argsptype))
            //            {
            //                def_mode = "先制攻撃";
            //                string argattr4 = "自";
            //                if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, argattr4))
            //                {
            //                    is_suiside = true;
            //                }

            //                // カウンター攻撃を実施
            //                withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
            //                SelectedTarget = withBlock5.CurrentForm();
            //            }
            //            else if (withBlock5.MaxCounterAttack() > withBlock5.UsedCounterAttack)
            //            {
            //                def_mode = "先制攻撃";
            //                string argattr5 = "自";
            //                if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, argattr5))
            //                {
            //                    is_suiside = true;
            //                }

            //                // カウンター攻撃の残り回数を減少
            //                withBlock5.UsedCounterAttack = (withBlock5.UsedCounterAttack + 1);

            //                // カウンター攻撃を実施
            //                withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
            //                SelectedTarget = withBlock5.CurrentForm();
            //            }
            //        }

            //        // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
            //        if (SupportGuardUnit2 is object)
            //        {
            //            attack_target = SupportGuardUnit2;
            //            attack_target_hp_ratio = SupportGuardUnitHPRatio2;
            //        }
            //    }
            //}

            //// サポートアタックのパートナーを探す
            //{
            //    var withBlock6 = SelectedUnit;
            //    if (withBlock6.Status == "出撃" & SelectedTarget.Status == "出撃" & UseSupportAttack)
            //    {
            //        SupportAttackUnit = withBlock6.LookForSupportAttack(SelectedTarget);

            //        // 合体技ではサポートアタック不能
            //        if (0 < SelectedWeapon & SelectedWeapon <= withBlock6.CountWeapon())
            //        {
            //            string argattr9 = "合";
            //            if (withBlock6.IsWeaponClassifiedAs(SelectedWeapon, argattr9))
            //            {
            //                SupportAttackUnit = null;
            //            }
            //        }

            //        // 魅了された場合
            //        object argIndex4 = "魅了";
            //        if (withBlock6.IsConditionSatisfied(argIndex4) & ReferenceEquals(withBlock6.Master, SelectedTarget))
            //        {
            //            SupportAttackUnit = null;
            //        }

            //        // 憑依された場合
            //        object argIndex5 = "憑依";
            //        if (withBlock6.IsConditionSatisfied(argIndex5))
            //        {
            //            if ((withBlock6.Master.Party ?? "") == (SelectedTarget.Party ?? ""))
            //            {
            //                SupportAttackUnit = null;
            //            }
            //        }

            //        // 踊らされた場合
            //        object argIndex6 = "踊り";
            //        if (withBlock6.IsConditionSatisfied(argIndex6))
            //        {
            //            SupportAttackUnit = null;
            //        }
            //    }
            //}

            // 攻撃の実施
            {
                //if (currentUnit.Status == "出撃" & currentUnit.MaxAction(true) > 0 & !currentUnit.IsConditionSatisfied("攻撃不能") & SelectedTarget.Status == "出撃")
                //{
                //    // まだ武器は使用可能か？
                //    if (SelectedWeapon > currentUnit.CountWeapon())
                //    {
                //        SelectedWeapon = -1;
                //    }
                //    else if ((wname ?? "") != (currentUnit.Weapon(SelectedWeapon).Name ?? ""))
                //    {
                //        SelectedWeapon = -1;
                //    }
                //    else if (CommandState == "移動後ターゲット選択")
                //    {
                //        if (!currentUnit.IsWeaponAvailable(SelectedWeapon, "移動後"))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }
                //    else
                //    {
                //        if (!currentUnit.IsWeaponAvailable(SelectedWeapon, "移動前"))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }

                //    if (SelectedWeapon > 0)
                //    {
                //        if (!currentUnit.IsTargetWithinRange(SelectedWeapon, SelectedTarget))
                //        {
                //            SelectedWeapon = 0;
                //        }
                //    }

                //    // 魅了された場合
                //    object argIndex7 = "魅了";
                //    if (currentUnit.IsConditionSatisfied(argIndex7) & ReferenceEquals(currentUnit.Master, SelectedTarget))
                //    {
                //        SelectedWeapon = -1;
                //    }

                //    // 憑依された場合
                //    object argIndex8 = "憑依";
                //    if (currentUnit.IsConditionSatisfied(argIndex8))
                //    {
                //        if ((currentUnit.Master.Party ?? "") == (SelectedTarget.Party0 ?? ""))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }

                //    // 踊らされた場合
                //    object argIndex9 = "踊り";
                //    if (currentUnit.IsConditionSatisfied(argIndex9))
                //    {
                //        SelectedWeapon = -1;
                //    }

                if (SelectedWeapon > 0)
                {
                    //if (SupportAttackUnit is object & currentUnit.MaxSyncAttack() > currentUnit.UsedSyncAttack)
                    //{
                    //    // 同時援護攻撃
                    //    currentUnit.Attack(SelectedWeapon, SelectedTarget, "統率", def_mode);
                    //}
                    //else
                    {
                        // 通常攻撃
                        currentUnit.Attack(currentWeapon, SelectedTarget, "", def_mode);
                    }
                }
                //else if (SelectedWeapon == 0)
                //{
                //    if (currentUnit.IsAnimationDefined("射程外", sub_situation: ""))
                //    {
                //        currentUnit.PlayAnimation("射程外", sub_situation: "");
                //    }
                //    else
                //    {
                //        currentUnit.SpecialEffect("射程外", sub_situation: "");
                //    }
                //    currentUnit.PilotMessage("射程外", msg_mode: "");
                //}
                //}
                //    else
                //{
                //    SelectedWeapon = -1;
                //}

                SelectedUnit = currentUnit.CurrentForm();

                // 防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
                if (SupportGuardUnit is object)
                {
                    defense_target2 = SupportGuardUnit;
                    defense_target2_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            //// 同時攻撃
            //if (SupportAttackUnit is object)
            //{
            //    if (SupportAttackUnit.Status != "出撃" | SelectedUnit.Status != "出撃" | SelectedTarget.Status != "出撃")
            //    {
            //        SupportAttackUnit = null;
            //    }
            //}

            //if (SupportAttackUnit is object)
            //{
            //    if (SelectedUnit.MaxSyncAttack() > SelectedUnit.UsedSyncAttack)
            //    {
            //        {
            //            var withBlock8 = SupportAttackUnit;
            //            // サポートアタックに使う武器を決定
            //            string argamode1 = "サポートアタック";
            //            int argmax_prob1 = 0;
            //            int argmax_dmg1 = 0;
            //            w2 = COM.SelectWeapon(SupportAttackUnit, SelectedTarget, argamode1, max_prob: argmax_prob1, max_dmg: argmax_dmg1);
            //            if (w2 > 0)
            //            {
            //                // サポートアタックを実施
            //                Map.MaskData[withBlock8.x, withBlock8.y] = false;
            //                if (!SRC.BattleAnimation)
            //                {
            //                    GUI.MaskScreen();
            //                }

            //                string argmain_situation4 = "サポートアタック開始";
            //                string argsub_situation4 = "";
            //                if (withBlock8.IsAnimationDefined(argmain_situation4, sub_situation: argsub_situation4))
            //                {
            //                    string argmain_situation3 = "サポートアタック開始";
            //                    string argsub_situation3 = "";
            //                    withBlock8.PlayAnimation(argmain_situation3, sub_situation: argsub_situation3);
            //                }

            //                object argu2 = SupportAttackUnit;
            //                GUI.UpdateMessageForm(SelectedTarget, argu2);
            //                withBlock8.Attack(w2, SelectedTarget, "同時援護攻撃", def_mode);
            //            }
            //        }

            //        // 後始末
            //        {
            //            var withBlock9 = SupportAttackUnit.CurrentForm();
            //            if (w2 > 0)
            //            {
            //                string argmain_situation6 = "サポートアタック終了";
            //                string argsub_situation6 = "";
            //                if (withBlock9.IsAnimationDefined(argmain_situation6, sub_situation: argsub_situation6))
            //                {
            //                    string argmain_situation5 = "サポートアタック終了";
            //                    string argsub_situation5 = "";
            //                    withBlock9.PlayAnimation(argmain_situation5, sub_situation: argsub_situation5);
            //                }

            //                // サポートアタックの残り回数を減らす
            //                withBlock9.UsedSupportAttack = (withBlock9.UsedSupportAttack + 1);

            //                // 同時援護攻撃の残り回数を減らす
            //                SelectedUnit.UsedSyncAttack = (SelectedUnit.UsedSyncAttack + 1);
            //            }
            //        }

            //        support_attack_done = true;

            //        // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
            //        // 入れ替えて記録
            //        if (SupportGuardUnit is object)
            //        {
            //            defense_target = SupportGuardUnit;
            //            defense_target_hp_ratio = SupportGuardUnitHPRatio;
            //        }
            //    }
            //}

            // 反撃の実施
            {
                //var withBlock10 = SelectedTarget;
                //if (def_mode != "先制攻撃")
                //{
                //    if (withBlock10.Status == "出撃" & withBlock10.Party != "味方" & SelectedUnit.Status == "出撃")
                //    {
                //        // まだ武器は使用可能か？
                //        if (SelectedTWeapon > 0)
                //        {
                //            string argref_mode4 = "移動前";
                //            if (SelectedTWeapon > withBlock10.CountWeapon())
                //            {
                //                SelectedTWeapon = -1;
                //            }
                //            else if ((twname ?? "") != (withBlock10.Weapon(SelectedTWeapon).Name ?? "") | !withBlock10.IsWeaponAvailable(SelectedTWeapon, argref_mode4))
                //            {
                //                SelectedTWeapon = -1;
                //            }
                //        }

                //        if (SelectedTWeapon > 0)
                //        {
                //            if (!withBlock10.IsTargetWithinRange(SelectedTWeapon, SelectedUnit))
                //            {
                //                // 敵が射程外に逃げていたら武器を再選択
                //                string argamode2 = "反撃";
                //                int argmax_prob2 = 0;
                //                int argmax_dmg2 = 0;
                //                SelectedTWeapon = COM.SelectWeapon(SelectedTarget, SelectedUnit, argamode2, max_prob: argmax_prob2, max_dmg: argmax_dmg2);
                //            }
                //        }

                //        // 行動不能な場合
                //        if (withBlock10.MaxAction() == 0)
                //        {
                //            SelectedTWeapon = -1;
                //        }

                //        // 魅了された場合
                //        object argIndex11 = "魅了";
                //        if (withBlock10.IsConditionSatisfied(argIndex11) & ReferenceEquals(withBlock10.Master, SelectedUnit))
                //        {
                //            SelectedTWeapon = -1;
                //        }

                //        // 憑依された場合
                //        object argIndex12 = "憑依";
                //        if (withBlock10.IsConditionSatisfied(argIndex12))
                //        {
                //            if ((withBlock10.Master.Party ?? "") == (SelectedUnit.Party ?? ""))
                //            {
                //                SelectedWeapon = -1;
                //            }
                //        }

                //        // 踊らされた場合
                //        object argIndex13 = "踊り";
                //        if (withBlock10.IsConditionSatisfied(argIndex13))
                //        {
                //            SelectedTWeapon = -1;
                //        }

                //        if (SelectedTWeapon > 0 & string.IsNullOrEmpty(def_mode))
                //        {
                //            // 反撃を実施
                //            string argattr10 = "自";
                //            if (withBlock10.IsWeaponClassifiedAs(SelectedTWeapon, argattr10))
                //            {
                //                is_suiside = true;
                //            }

                //            withBlock10.Attack(SelectedTWeapon, SelectedUnit, "", "");

                //            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                //            if (SupportGuardUnit2 is object)
                //            {
                //                attack_target = SupportGuardUnit2;
                //                attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                //            }
                //        }
                //        else if (SelectedTWeapon == 0 & withBlock10.x == tx & withBlock10.y == ty)
                //        {
                //            // 反撃出来る武器がなかった場合は射程外メッセージを表示
                //            string argmain_situation9 = "射程外";
                //            string argsub_situation9 = "";
                //            if (withBlock10.IsAnimationDefined(argmain_situation9, sub_situation: argsub_situation9))
                //            {
                //                string argmain_situation7 = "射程外";
                //                string argsub_situation7 = "";
                //                withBlock10.PlayAnimation(argmain_situation7, sub_situation: argsub_situation7);
                //            }
                //            else
                //            {
                //                string argmain_situation8 = "射程外";
                //                string argsub_situation8 = "";
                //                withBlock10.SpecialEffect(argmain_situation8, sub_situation: argsub_situation8);
                //            }

                //            string argSituation1 = "射程外";
                //            string argmsg_mode1 = "";
                //            withBlock10.PilotMessage(argSituation1, msg_mode: argmsg_mode1);
                //        }
                //        else
                //        {
                //            SelectedTWeapon = -1;
                //        }
                //    }
                //    else
                //    {
                //        SelectedTWeapon = -1;
                //    }
                //}
            }

            //// サポートアタック
            //if (SupportAttackUnit is object)
            //{
            //    if (SupportAttackUnit.Status != "出撃" | SelectedUnit.Status != "出撃" | SelectedTarget.Status != "出撃" | support_attack_done)
            //    {
            //        SupportAttackUnit = null;
            //    }
            //}

            //if (SupportAttackUnit is object)
            //{
            //    {
            //        var withBlock11 = SupportAttackUnit;
            //        // サポートアタックに使う武器を決定
            //        string argamode3 = "サポートアタック";
            //        int argmax_prob3 = 0;
            //        int argmax_dmg3 = 0;
            //        w2 = COM.SelectWeapon(SupportAttackUnit, SelectedTarget, argamode3, max_prob: argmax_prob3, max_dmg: argmax_dmg3);
            //        if (w2 > 0)
            //        {
            //            // サポートアタックを実施
            //            Map.MaskData[withBlock11.x, withBlock11.y] = false;
            //            if (!SRC.BattleAnimation)
            //            {
            //                GUI.MaskScreen();
            //            }

            //            string argmain_situation11 = "サポートアタック開始";
            //            string argsub_situation11 = "";
            //            if (withBlock11.IsAnimationDefined(argmain_situation11, sub_situation: argsub_situation11))
            //            {
            //                string argmain_situation10 = "サポートアタック開始";
            //                string argsub_situation10 = "";
            //                withBlock11.PlayAnimation(argmain_situation10, sub_situation: argsub_situation10);
            //            }

            //            object argu21 = SupportAttackUnit;
            //            GUI.UpdateMessageForm(SelectedTarget, argu21);
            //            withBlock11.Attack(w2, SelectedTarget, "援護攻撃", def_mode);
            //        }
            //    }

            //    // 後始末
            //    {
            //        var withBlock12 = SupportAttackUnit.CurrentForm();
            //        string argmain_situation13 = "サポートアタック終了";
            //        string argsub_situation13 = "";
            //        if (withBlock12.IsAnimationDefined(argmain_situation13, sub_situation: argsub_situation13))
            //        {
            //            string argmain_situation12 = "サポートアタック終了";
            //            string argsub_situation12 = "";
            //            withBlock12.PlayAnimation(argmain_situation12, sub_situation: argsub_situation12);
            //        }

            //        // サポートアタックの残り回数を減らす
            //        if (w2 > 0)
            //        {
            //            withBlock12.UsedSupportAttack = (withBlock12.UsedSupportAttack + 1);
            //        }
            //    }

            //    // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
            //    // 入れ替えて記録
            //    if (SupportGuardUnit is object)
            //    {
            //        defense_target = SupportGuardUnit;
            //        defense_target_hp_ratio = SupportGuardUnitHPRatio;
            //    }
            //}

            //SelectedTarget = SelectedTarget.CurrentForm();
            //{
            //    var withBlock13 = SelectedUnit;
            //    if (withBlock13.Status == "出撃")
            //    {
            //        // 攻撃をかけたユニットがまだ生き残っていれば経験値＆資金を獲得

            //        if (SelectedTarget.Status == "破壊" & !is_suiside)
            //        {
            //            // 敵を破壊した場合

            //            // 経験値を獲得
            //            string argexp_situation = "破壊";
            //            string argexp_mode = "";
            //            withBlock13.GetExp(SelectedTarget, argexp_situation, exp_mode: argexp_mode);
            //            string argoname = "合体技パートナー経験値無効";
            //            if (!Expression.IsOptionDefined(argoname))
            //            {
            //                var loopTo2 = Information.UBound(partners);
            //                for (i = 1; i <= loopTo2; i++)
            //                {
            //                    string argexp_situation1 = "破壊";
            //                    string argexp_mode1 = "パートナー";
            //                    partners[i].CurrentForm().GetExp(SelectedTarget, argexp_situation1, argexp_mode1);
            //                }
            //            }

            //            // 獲得する資金を算出
            //            earnings = SelectedTarget.Value / 2;

            //            // スペシャルパワーによる獲得資金増加
            //            string argsptype1 = "獲得資金増加";
            //            if (withBlock13.IsUnderSpecialPowerEffect(argsptype1))
            //            {
            //                string argsname = "獲得資金増加";
            //                earnings = (earnings * (1d + 0.1d * withBlock13.SpecialPowerEffectLevel(argsname)));
            //            }

            //            // パイロット能力による獲得資金増加
            //            string argsname1 = "資金獲得";
            //            if (withBlock13.IsSkillAvailable(argsname1))
            //            {
            //                string argsptype2 = "獲得資金増加";
            //                string argoname1 = "収得効果重複";
            //                if (!withBlock13.IsUnderSpecialPowerEffect(argsptype2) | Expression.IsOptionDefined(argoname1))
            //                {
            //                    earnings = GeneralLib.MinDbl(earnings * ((10d + withBlock13.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
            //                }
            //            }

            //            // 資金を獲得
            //            SRC.IncrMoney(earnings);
            //            if (earnings > 0)
            //            {
            //                string argtname = "資金";
            //                GUI.DisplaySysMessage(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(earnings) + "の" + Expression.Term(argtname, SelectedUnit) + "を得た。");
            //            }

            //            // スペシャルパワー効果「敵破壊時再行動」
            //            string argsptype3 = "敵破壊時再行動";
            //            if (withBlock13.IsUnderSpecialPowerEffect(argsptype3))
            //            {
            //                withBlock13.UsedAction = (withBlock13.UsedAction - 1);
            //            }
            //        }
            //        else
            //        {
            //            // 相手を破壊できなかった場合

            //            // 経験値を獲得
            //            string argexp_situation2 = "攻撃";
            //            string argexp_mode2 = "";
            //            withBlock13.GetExp(SelectedTarget, argexp_situation2, exp_mode: argexp_mode2);
            //            string argoname2 = "合体技パートナー経験値無効";
            //            if (!Expression.IsOptionDefined(argoname2))
            //            {
            //                var loopTo3 = Information.UBound(partners);
            //                for (i = 1; i <= loopTo3; i++)
            //                {
            //                    string argexp_situation3 = "攻撃";
            //                    string argexp_mode3 = "パートナー";
            //                    partners[i].CurrentForm().GetExp(SelectedTarget, argexp_situation3, argexp_mode3);
            //                }
            //            }
            //        }

            //        // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
            //        string argstype = "戦闘終了";
            //        withBlock13.RemoveSpecialPowerInEffect(argstype);
            //        if (earnings > 0)
            //        {
            //            string argstype1 = "敵破壊";
            //            withBlock13.RemoveSpecialPowerInEffect(argstype1);
            //        }
            //    }
            //}

            //{
            //    var withBlock14 = SelectedTarget;
            //    if (withBlock14.Status == "出撃")
            //    {
            //        // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
            //        string argstype2 = "戦闘終了";
            //        withBlock14.RemoveSpecialPowerInEffect(argstype2);
            //    }
            //}

            GUI.CloseMessageForm();
            Status.ClearUnitStatus();

            //// 状態＆データ更新
            //{
            //    var withBlock15 = attack_target.CurrentForm();
            //    withBlock15.UpdateCondition();
            //    withBlock15.Update();
            //}

            //if (SupportAttackUnit is object)
            //{
            //    {
            //        var withBlock16 = SupportAttackUnit.CurrentForm();
            //        withBlock16.UpdateCondition();
            //        withBlock16.Update();
            //    }
            //}

            //{
            //    var withBlock17 = defense_target.CurrentForm();
            //    withBlock17.UpdateCondition();
            //    withBlock17.Update();
            //}

            //if (defense_target2 is object)
            //{
            //    {
            //        var withBlock18 = defense_target2.CurrentForm();
            //        withBlock18.UpdateCondition();
            //        withBlock18.Update();
            //    }
            //}

            //// 破壊＆損傷率イベント発生

            //if (SelectedWeapon <= 0)
            //{
            //    SelectedWeaponName = "";
            //}

            //if (SelectedTWeapon <= 0)
            //{
            //    SelectedTWeaponName = "";
            //}

            //// 攻撃を受けた攻撃側ユニット
            //{
            //    var withBlock19 = attack_target.CurrentForm();
            //    if (withBlock19.Status == "破壊")
            //    {
            //        Event_Renamed.HandleEvent("破壊", withBlock19.MainPilot().ID);
            //    }
            //    else if (withBlock19.Status == "出撃" & withBlock19.HP / (double)withBlock19.MaxHP < attack_target_hp_ratio)
            //    {
            //        Event_Renamed.HandleEvent("損傷率", withBlock19.MainPilot().ID, 100 * (withBlock19.MaxHP - withBlock19.HP) / withBlock19.MaxHP);
            //    }
            //}

            //if (SRC.IsScenarioFinished)
            //{
            //    GUI.UnlockGUI();
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    goto EndAttack;
            //}

            //SelectedUnit = SelectedUnit.CurrentForm();

            //// ターゲット側のイベント処理を行うためにユニットの入れ替えを行う
            //SaveSelections();
            //SwapSelections();

            //// 攻撃を受けた防御側ユニット
            //{
            //    var withBlock20 = defense_target.CurrentForm();
            //    if (withBlock20.CountPilot() > 0)
            //    {
            //        if (withBlock20.Status == "破壊")
            //        {
            //            Event_Renamed.HandleEvent("破壊", withBlock20.MainPilot().ID);
            //        }
            //        else if (withBlock20.Status == "出撃" & withBlock20.HP / (double)withBlock20.MaxHP < defense_target_hp_ratio)
            //        {
            //            Event_Renamed.HandleEvent("損傷率", withBlock20.MainPilot().ID, 100 * (withBlock20.MaxHP - withBlock20.HP) / withBlock20.MaxHP);
            //        }
            //    }
            //}

            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    return;
            //}

            //// 攻撃を受けた防御側ユニットその2
            //if (defense_target2 is object)
            //{
            //    if (!ReferenceEquals(defense_target2.CurrentForm(), defense_target.CurrentForm()))
            //    {
            //        {
            //            var withBlock21 = defense_target2.CurrentForm();
            //            if (withBlock21.CountPilot() > 0)
            //            {
            //                if (withBlock21.Status == "破壊")
            //                {
            //                    Event_Renamed.HandleEvent("破壊", withBlock21.MainPilot().ID);
            //                }
            //                else if (withBlock21.Status == "出撃" & withBlock21.HP / (double)withBlock21.MaxHP < defense_target2_hp_ratio)
            //                {
            //                    Event_Renamed.HandleEvent("損傷率", withBlock21.MainPilot().ID, 100 * (withBlock21.MaxHP - withBlock21.HP) / withBlock21.MaxHP);
            //                }
            //            }
            //        }
            //    }
            //}

            //RestoreSelections();
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners = new Unit[1];
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// 武器の使用後イベント
            //if (SelectedUnit.Status == "出撃" & SelectedWeapon > 0)
            //{
            //    Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, wname);
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //if (SelectedTarget.Status == "出撃" & SelectedTWeapon > 0)
            //{
            //    SaveSelections();
            //    SwapSelections();
            //    Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, twname);
            //    RestoreSelections();
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //// 攻撃後イベント
            //if (SelectedUnit.Status == "出撃" & SelectedTarget.Status == "出撃")
            //{
            //    Event_Renamed.HandleEvent("攻撃後", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //// もし敵が移動していれば進入イベント
            //{
            //    var withBlock22 = SelectedTarget;
            //    SelectedTarget = null;
            //    if (withBlock22.Status == "出撃")
            //    {
            //        if (withBlock22.x != tx | withBlock22.y != ty)
            //        {
            //            Event_Renamed.HandleEvent("進入", withBlock22.MainPilot().ID, withBlock22.x, withBlock22.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                SelectedPartners = new Unit[1];
            //                GUI.UnlockGUI();
            //                return;
            //            }

            //            if (SRC.IsCanceled)
            //            {
            //                SRC.IsCanceled = false;
            //                SelectedPartners = new Unit[1];
            //                GUI.UnlockGUI();
            //                return;
            //            }
            //        }
            //    }
            //}

            //EndAttack:
            //;


            //// 合体技のパートナーの行動数を減らす
            //string argoname3 = "合体技パートナー行動数無消費";
            //if (!Expression.IsOptionDefined(argoname3))
            //{
            //    var loopTo4 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo4; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners = new Unit[1];

            //// ハイパーモード＆ノーマルモードの自動発動をチェック
            //SRC.UList.CheckAutoHyperMode();
            //SRC.UList.CheckAutoNormalMode();

            //// ADD START MARGE
            //// 再移動
            //if (is_p_weapon & SelectedUnit.Status == "出撃")
            //{
            //    string argsname2 = "遊撃";
            //    if (SelectedUnit.MainPilot().IsSkillAvailable(argsname2) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        string argoname4 = "大型マップ";
            //        if (!Expression.IsOptionDefined(argoname4))
            //        {
            //            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            //        }

            //        GUI.MaskScreen();
            //        if (GUI.NewGUIMode)
            //        {
            //            Application.DoEvents();
            //            Status.ClearUnitStatus();
            //        }
            //        else
            //        {
            //            Status.DisplayUnitStatus(SelectedUnit);
            //        }

            //        CommandState = "ターゲット選択";
            //        return;
            //    }
            //}
            //// ADD END MARGE

            // 行動数を減らす
            WaitCommand();
        }

        // マップ攻撃による「攻撃」コマンドを終了
        private void MapAttackCommand()
        {
            LogDebug();

            throw new NotImplementedException();
            //// MOD END MARGE
            //int i;
            //var partners = default(Unit[]);
            //// ADD START MARGE
            //bool is_p_weapon;
            //// ADD END MARGE

            //{
            //    var withBlock = SelectedUnit;
            //    // ADD START MARGE
            //    // 移動後使用後可能な武器か記録しておく
            //    string argattr = "移動後攻撃可";
            //    is_p_weapon = withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr);
            //    // ADD END MARGE
            //    // 攻撃目標地点を選択して初めて攻撃範囲が分かるタイプのマップ攻撃
            //    // の場合は再度プレイヤーの選択を促す必要がある
            //    if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
            //    {
            //        string argattr4 = "Ｍ投";
            //        string argattr5 = "Ｍ移";
            //        string argattr6 = "Ｍ線";
            //        if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr4))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃範囲を設定
            //            string argattr3 = "識";
            //            string argsptype = "識別攻撃";
            //            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr3) | withBlock.IsUnderSpecialPowerEffect(argsptype))
            //            {
            //                string argattr1 = "Ｍ投";
            //                string arguparty = "味方の敵";
            //                Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, argattr1), 1, arguparty);
            //            }
            //            else
            //            {
            //                string argattr2 = "Ｍ投";
            //                string arguparty1 = "すべて";
            //                Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, argattr2), 1, arguparty1);
            //            }

            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr5))
            //        {
            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃目標地点に他のユニットがいては駄目
            //            if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
            //            {
            //                GUI.MaskScreen();
            //                return;
            //            }

            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr6))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //    }

            //    // 合体技パートナーの設定
            //    string argattr7 = "合";
            //    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr7))
            //    {
            //        string argctype_Renamed = "武装";
            //        withBlock.CombinationPartner(argctype_Renamed, SelectedWeapon, partners);
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[1];
            //        partners = new Unit[1];
            //    }

            //    if (GUI.MainWidth != 15)
            //    {
            //        Status.ClearUnitStatus();
            //    }

            //    GUI.LockGUI();
            //    SelectedTWeapon = 0;

            //    // マップ攻撃による攻撃を行う
            //    withBlock.MapAttack(SelectedWeapon, SelectedX, SelectedY);
            //    SelectedUnit = withBlock.CurrentForm();
            //    SelectedTarget = null;
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[1];
            //        WaitCommand();
            //        return;
            //    }
            //}

            //// 合体技のパートナーの行動数を減らす
            //string argoname = "合体技パートナー行動数無消費";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    var loopTo = Information.UBound(partners);
            //    for (i = 1; i <= loopTo; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners = new Unit[1];

            //// ADD START MARGE
            //// 再移動
            //if (is_p_weapon & SelectedUnit.Status == "出撃")
            //{
            //    string argsname = "遊撃";
            //    if (SelectedUnit.MainPilot().IsSkillAvailable(argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        string argoname1 = "大型マップ";
            //        if (!Expression.IsOptionDefined(argoname1))
            //        {
            //            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            //        }

            //        GUI.MaskScreen();
            //        if (GUI.NewGUIMode)
            //        {
            //            Application.DoEvents();
            //            Status.ClearUnitStatus();
            //        }
            //        else
            //        {
            //            Status.DisplayUnitStatus(SelectedUnit);
            //        }

            //        CommandState = "ターゲット選択";
            //        return;
            //    }
            //}
            //// ADD END MARGE

            //// 行動終了
            //WaitCommand();
        }
    }
}