// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // === 攻撃関連処理 ===
    public partial class Unit
    {
        // マップ攻撃 w で (tx,ty) を攻撃
        public void MapAttack(UnitWeapon w, int tx, int ty, bool is_event = false)
        {
            var wname = w.Name;
            var wnickname = w.WeaponNickname();
            Commands.SelectedWeaponName = wname;

            // 効果範囲を設定
            var min_range = w.WeaponMinRange();
            var max_range = w.WeaponMaxRange();
            if (w.IsWeaponClassifiedAs("Ｍ直"))
            {
                if (ty < y)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "N");
                }
                else if (ty > y)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "S");
                }
                else if (tx < x)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "W");
                }
                else
                {
                    Map.AreaInLine(x, y, min_range, max_range, "E");
                }
            }
            else if (w.IsWeaponClassifiedAs("Ｍ拡"))
            {
                if (ty < y && Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "N");
                }
                else if (ty > y && Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "S");
                }
                else if (tx < x && Math.Abs((x - tx)) > Math.Abs((y - ty)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "W");
                }
                else
                {
                    Map.AreaInCone(x, y, min_range, max_range, "E");
                }
            }
            else if (w.IsWeaponClassifiedAs("Ｍ扇"))
            {
                if (ty < y && Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "N", (int)w.WeaponLevel("Ｍ扇"));
                }
                else if (ty > y && Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "S", (int)w.WeaponLevel("Ｍ扇"));
                }
                else if (tx < x && Math.Abs((x - tx)) >= Math.Abs((y - ty)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "W", (int)w.WeaponLevel("Ｍ扇"));
                }
                else
                {
                    Map.AreaInSector(x, y, min_range, max_range, "E", (int)w.WeaponLevel("Ｍ扇"));
                }
            }
            else if (w.IsWeaponClassifiedAs("Ｍ投"))
            {
                Map.AreaInRange(tx, ty, (int)w.WeaponLevel("Ｍ投"), 1, "すべて");
            }
            else if (w.IsWeaponClassifiedAs("Ｍ全"))
            {
                Map.AreaInRange(x, y, max_range, min_range, "すべて");
            }
            else if (w.IsWeaponClassifiedAs("Ｍ移") || w.IsWeaponClassifiedAs("Ｍ線"))
            {
                Map.AreaInPointToPoint(x, y, tx, ty);
            }

            Map.MaskData[x, y] = false;

            // 識別型マップ攻撃
            if (w.IsWeaponClassifiedAs("識") || IsUnderSpecialPowerEffect("識別攻撃"))
            {
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃")
                    {
                        if (IsAlly(u) || w.WeaponAdaption(u.Area) == 0d)
                        {
                            Map.MaskData[u.x, u.y] = true;
                        }
                    }
                }

                Map.MaskData[x, y] = false;
            }

            // 合体技の処理
            IList<Unit> partners;
            if (w.IsWeaponClassifiedAs("合"))
            {

                // 合体技のパートナーのハイライト表示
                // MaskDataを保存して使用している
                var TmpMaskData = new Src2DArray<bool>(Map.MapWidth + 1, Map.MapHeight + 1);
                var loopTo = Map.MapWidth;
                for (var i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (var j = 1; j <= loopTo1; j++)
                        TmpMaskData[i, j] = Map.MaskData[i, j];
                }

                partners = w.CombinationPartner("武装");
                foreach (var pu in partners)
                {

                    Map.MaskData[pu.x, pu.y] = false;
                    TmpMaskData[pu.x, pu.y] = true;
                }

                GUI.MaskScreen();
                var loopTo3 = Map.MapWidth;
                for (var i = 1; i <= loopTo3; i++)
                {
                    var loopTo4 = Map.MapHeight;
                    for (var j = 1; j <= loopTo4; j++)
                        Map.MaskData[i, j] = TmpMaskData[i, j];
                }
            }
            else
            {
                partners = new List<Unit>();
                Commands.SelectedPartners = new Unit[1];
                GUI.MaskScreen();
            }

            // 自分自身には攻撃しない
            Map.MaskData[x, y] = true;

            // マップ攻撃の影響を受けるユニットのリストを作成
            IList<Unit> targets = new List<Unit>();
            var loopTo5 = Map.MapWidth;
            for (var i = 1; i <= loopTo5; i++)
            {
                var loopTo6 = Map.MapHeight;
                for (var j = 1; j <= loopTo6; j++)
                {
                    // マップ攻撃の影響をうけるかチェック
                    if (Map.MaskData[i, j])
                    {
                        goto NextLoop;
                    }

                    if (Map.MapDataForUnit[i, j] is null)
                    {
                        goto NextLoop;
                    }

                    var t = Map.MapDataForUnit[i, j];
                    {
                        if (w.WeaponAdaption((string)t.Area) == 0d)
                        {
                            goto NextLoop;
                        }

                        if (w.IsWeaponClassifiedAs("識") || IsUnderSpecialPowerEffect("識別攻撃"))
                        {
                            if (IsAlly(t))
                            {
                                goto NextLoop;
                            }
                        }

                        if (t.IsUnderSpecialPowerEffect("隠れ身"))
                        {
                            goto NextLoop;
                        }

                        targets.Add(t);
                    }

                NextLoop:
                    ;
                }
            }

            // 攻撃の起点を設定
            int rx, ry;
            if (w.IsWeaponClassifiedAs("Ｍ投"))
            {
                rx = tx;
                ry = ty;
            }
            else
            {
                rx = x;
                ry = y;
            }

            // 起点からの距離に応じて並べ替え
            // 近い順
            targets = targets.OrderBy(t => Math.Abs(t.x - rx) + Math.Abs(t.y - ry)).ToList();

            // 戦闘前に一旦クリア
            Commands.SupportAttackUnit = null;
            Commands.SupportGuardUnit = null;
            Commands.SupportGuardUnit2 = null;

            // イベントの処理
            if (!is_event)
            {
                // 使用イベント
                Event.HandleEvent("使用", MainPilot().ID, wname);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    return;
                }

                // マップ攻撃開始前にあらかじめ攻撃イベントを発生させる
                foreach (var t in targets)
                {
                    Commands.SaveSelections();
                    Commands.SelectedTarget = t;
                    Event.HandleEvent("攻撃", MainPilot().ID, t.MainPilot().ID);
                    Commands.RestoreSelections();
                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
                    {
                        return;
                    }
                }
            }

            // まだ攻撃可能？
            if (!is_event)
            {
                if (Status != "出撃" || MaxAction(true) == 0 || IsConditionSatisfied("攻撃不能"))
                {
                    return;
                }
            }

            // ターゲットに関する情報を記録
            var targetsInfo = targets.Select(t => new
            {
                t = t,
                targets_hp_ratio = t.HP / (double)t.MaxHP,
                targets_x = t.x,
                targets_y = t.y,
            }).ToList();

            GUI.OpenMessageForm(this, u2: null);

            // 現在の選択状況をセーブ
            Commands.SaveSelections();

            // 選択内容を切り替え
            Commands.SelectedUnit = this;
            Event.SelectedUnitForEvent = this;
            Commands.SelectedWeapon = w.WeaponNo();
            Commands.SelectedX = tx;
            Commands.SelectedY = ty;

            // 変な「対～」メッセージが表示されないようにターゲットをオフ
            Commands.SelectedTarget = null;
            Event.SelectedTargetForEvent = null;

            // 攻撃準備の効果音
            bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(準備)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            if (IsAnimationDefined(wname + "(準備)", sub_situation: ""))
            {
                PlayAnimation(wname + "(準備)", sub_situation: "");
            }
            else if (IsAnimationDefined(wname, sub_situation: "") && !Expression.IsOptionDefined("武器準備アニメ非表示") && SRC.WeaponAnimation)
            {
                PlayAnimation(wname + "(準備)", sub_situation: "");
            }
            else if (localIsSpecialEffectDefined())
            {
                SpecialEffect(wname + "(準備)", sub_situation: "");
            }

            // マップ攻撃攻撃開始のメッセージ
            if (IsMessageDefined(wname))
            {
                if (IsMessageDefined("かけ声(" + wname + ")"))
                {
                    PilotMessage("かけ声(" + wname + ")", msg_mode: "");
                }
                else
                {
                    PilotMessage("かけ声", msg_mode: "");
                }
            }

            // 攻撃メッセージ
            PilotMessage(wname, "攻撃");

            // 戦闘アニメ or 効果音
            if (IsAnimationDefined(wname + "(攻撃)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
            {
                PlayAnimation(wname + "(攻撃)", sub_situation: "");
            }
            else if (IsSpecialEffectDefined(wname, sub_situation: ""))
            {
                SpecialEffect(wname, sub_situation: "");
            }
            else
            {
                Effect.AttackEffect(this, w);
            }

            // 攻撃中の攻撃力変動を避けるため、あらかじめ攻撃力を保存しておく
            SelectedMapAttackPower = 0;
            SelectedMapAttackPower = w.WeaponPower("初期値");

            // 「永」属性武器が破壊されることによるマップ攻撃キャンセル処理の初期化
            IsMapAttackCanceled = false;

            // 武器使用による弾数＆ＥＮ消費
            w.UseWeapon();
            GUI.UpdateMessageForm(this, u2: null);

            // 攻撃時のシステムメッセージ
            if (IsSysMessageDefined(wname, sub_situation: ""))
            {
                // 「武器名(解説)」のメッセージを使用
                SysMessage(wname, sub_situation: "", add_msg: "");
            }
            else if (IsSysMessageDefined("攻撃", sub_situation: ""))
            {
                // 「攻撃(解説)」のメッセージを使用
                SysMessage(wname, sub_situation: "", add_msg: "");
            }
            else
            {
                string msg = "";
                switch (partners.Count)
                {
                    case 0:
                        {
                            // 通常攻撃
                            msg = Nickname + "は";
                            break;
                        }

                    case 1:
                        {
                            // ２体合体攻撃
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "と[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    case 2:
                        {
                            // ３体合体攻撃
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]、[" + partners[2].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "、[" + partners[1].MainPilot().get_Nickname(false) + "]、[" + partners[2].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    default:
                        {
                            // ３体以上による合体攻撃
                            msg = Nickname + "達は";
                            break;
                        }
                }

                // 攻撃の種類によってメッセージを切り替え
                if (Strings.Right(wnickname, 2) == "攻撃" || Strings.Right(wnickname, 4) == "アタック" || wnickname == "突撃")
                {
                    msg = msg + "[" + wnickname + "]をかけた。";
                }
                else if (w.IsSpellWeapon())
                {
                    if (Strings.Right(wnickname, 2) == "呪文")
                    {
                        msg = msg + "[" + wnickname + "]を唱えた。";
                    }
                    else if (Strings.Right(wnickname, 2) == "の杖")
                    {
                        msg = msg + "[" + Strings.Left(wnickname, Strings.Len(wnickname) - 2) + "]の呪文を唱えた。";
                    }
                    else
                    {
                        msg = msg + "[" + wnickname + "]の呪文を唱えた。";
                    }
                }
                else if (w.IsWeaponClassifiedAs("実") && (Strings.InStr(wnickname, "ミサイル") > 0 || Strings.InStr(wnickname, "ロケット") > 0))
                {
                    msg = msg + "[" + wnickname + "]を発射した。";
                }
                else if (Strings.Right(wnickname, 1) == "息" || Strings.Right(wnickname, 3) == "ブレス" || Strings.Right(wnickname, 2) == "光線" || Strings.Right(wnickname, 1) == "光" || Strings.Right(wnickname, 3) == "ビーム" || Strings.Right(wnickname, 4) == "レーザー")
                {
                    msg = msg + "[" + wnickname + "]を放った。";
                }
                else if (Strings.Right(wnickname, 1) == "歌")
                {
                    msg = msg + "[" + wnickname + "]を歌った。";
                }
                else if (Strings.Right(wnickname, 2) == "踊り")
                {
                    msg = msg + "[" + wnickname + "]を踊った。";
                }
                else
                {
                    msg = msg + "[" + wnickname + "]で攻撃をかけた。";
                }

                // メッセージを表示
                GUI.DisplaySysMessage(msg);
            }

            // 命中後メッセージ
            PilotMessage(wname + "(命中)", msg_mode: "");

            // 選択状況を復元
            Commands.RestoreSelections();

            // 経験値＆資金獲得のメッセージ表示用に各種データを保存
            var mp = MainPilot();
            var prev_level = mp.Level;
            var prev_special_power = mp.SpecialPowerNames;
            var prev_skills = Enumerable.Range(1, mp.CountSkill()).Select(i => new
            {
                prev_stype = mp.Skill(i),
                prev_slevel = mp.SkillLevel(i, "基本値"),
                prev_sname = mp.SkillName(i),
            }).ToList();

            var prev_money = SRC.Money;

            // 攻撃元ユニットは SelectedTarget に設定していないといけない
            Commands.SelectedTarget = this;

            // 移動型マップ攻撃による移動を行う
            if (w.IsWeaponClassifiedAs("Ｍ移"))
            {
                Jump(tx, ty);
            }

            // 個々のユニットに対して攻撃
            foreach (var tu in targets)
            {
                var t = tu.CurrentForm();
                if (t.Status == "出撃")
                {
                    if (Party == "味方" || Party == "ＮＰＣ")
                    {
                        GUI.UpdateMessageForm(t, this);
                    }
                    else
                    {
                        GUI.UpdateMessageForm(this, t);
                    }

                    // 攻撃を行う
                    Attack(w, t, "マップ攻撃", "", is_event);

                    // TODO Impl SupportGuardUnit
                    //// かばうによりターゲットが変化している？
                    //if (Commands.SupportGuardUnit is object)
                    //{
                    //    targets[i] = Commands.SupportGuardUnit.CurrentForm();
                    //    targets_hp_ratio[i] = Commands.SupportGuardUnitHPRatio;
                    //    targets_x[i] = targets[i].x;
                    //    targets_y[i] = targets[i].y;
                    //}

                    // これ以上攻撃を続けられない場合
                    if (Status != "出撃" || CountPilot() == 0 || IsMapAttackCanceled)
                    {
                        GUI.CloseMessageForm();
                        SelectedMapAttackPower = 0;
                        goto DoEvent;
                    }

                    GUI.ClearMessageForm();
                }
            }

            // とどめメッセージ
            if (IsMessageDefined(wname + "(とどめ)"))
            {
                PilotMessage(wname + "(とどめ)", msg_mode: "");
            }

            bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(とどめ)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            if (IsAnimationDefined(wname + "(とどめ)", sub_situation: ""))
            {
                PlayAnimation(wname + "(とどめ)", sub_situation: "");
            }
            else if (localIsSpecialEffectDefined1())
            {
                SpecialEffect(wname + "(とどめ)", sub_situation: "");
            }

            // カットインは消去しておく
            if (GUI.IsPictureVisible)
            {
                GUI.ClearPicture();
                GUI.UpdateScreen();
            }

            // 保存した攻撃力の使用を止める
            SelectedMapAttackPower = 0;

            // 戦闘アニメ終了処理
            if (IsAnimationDefined(wname + "(終了)", sub_situation: ""))
            {
                PlayAnimation(wname + "(終了)", sub_situation: "");
            }
            else if (IsAnimationDefined("終了", sub_situation: ""))
            {
                PlayAnimation("終了", sub_situation: "");
            }

            // 戦闘アニメで変更されたユニット画像を元に戻す
            if (IsConditionSatisfied("ユニット画像"))
            {
                DeleteCondition("ユニット画像");
                BitmapID = GUI.MakeUnitBitmap(this);
                if (GUI.IsPictureVisible)
                {
                    GUI.PaintUnitBitmap(this, "リフレッシュ無し");
                }
                else
                {
                    GUI.PaintUnitBitmap(this);
                }
            }

            if (IsConditionSatisfied("非表示付加"))
            {
                DeleteCondition("非表示付加");
                BitmapID = GUI.MakeUnitBitmap(this);
                if (GUI.IsPictureVisible)
                {
                    GUI.PaintUnitBitmap(this, "リフレッシュ無し");
                }
                else
                {
                    GUI.PaintUnitBitmap(this);
                }
            }

            foreach (var pu in partners)
            {
                var u = pu.CurrentForm();
                if (u.IsConditionSatisfied("ユニット画像"))
                {
                    u.DeleteCondition("ユニット画像");
                    //u.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                    GUI.PaintUnitBitmap(pu.CurrentForm());
                }

                if (u.IsConditionSatisfied("非表示付加"))
                {
                    u.DeleteCondition("非表示付加");
                    //u.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                    GUI.PaintUnitBitmap(pu.CurrentForm());
                }
            }

            var isDestroyed = false;
            if (Party == "味方" && !is_event)
            {
                // 経験値＆資金の獲得
                var earned_exp = 0;
                foreach (var tu in targets)
                {
                    var t = tu.CurrentForm();
                    if (!IsEnemy(t))
                    {
                    }
                    // 味方からは経験値＆資金は得られない
                    else if (t.Status == "破壊")
                    {
                        isDestroyed = true;

                        // 経験値の獲得
                        earned_exp = earned_exp + GetExp(t, "破壊", "マップ");

                        // 合体技のパートナーへの経験値
                        if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
                        {
                            foreach (var pu in partners)
                            {
                                pu.CurrentForm().GetExp(t, "破壊", "パートナー");
                            }
                        }

                        // 獲得する資金を算出
                        int earnings = t.Value / 2;

                        // スペシャルパワーによる獲得資金増加
                        if (IsUnderSpecialPowerEffect("獲得資金増加"))
                        {
                            earnings = (int)GeneralLib.MinDbl(earnings * (1d + 0.1d * SpecialPowerEffectLevel("獲得資金増加")), 999999999d);
                        }

                        // パイロット能力による獲得資金増加
                        if (IsSkillAvailable("資金獲得"))
                        {
                            if (!IsUnderSpecialPowerEffect("獲得資金増加") || Expression.IsOptionDefined("収得効果重複"))
                            {
                                earnings = (int)GeneralLib.MinDbl(earnings * ((10d + SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
                            }
                        }

                        // 資金を獲得
                        SRC.IncrMoney(earnings);
                    }
                    else
                    {
                        // 経験値の獲得
                        earned_exp = earned_exp + GetExp(t, "攻撃", "マップ");

                        // 合体技のパートナーへの経験値
                        if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
                        {
                            foreach (var pu in partners)
                            {
                                pu.CurrentForm().GetExp(t, "攻撃", "パートナー");
                            }
                        }
                    }
                }

                // 獲得した経験値＆資金の表示
                if (SRC.Money > prev_money)
                {
                    GUI.DisplaySysMessage(SrcFormatter.Format(SRC.Money - prev_money) + "の" + Expression.Term("資金", w.Unit) + "を得た。");
                }

                {
                    var withBlock8 = MainPilot();
                    // レベルアップの処理
                    if (withBlock8.Level > prev_level)
                    {
                        if (IsAnimationDefined("レベルアップ", sub_situation: ""))
                        {
                            PlayAnimation("レベルアップ", sub_situation: "");
                        }
                        else if (IsSpecialEffectDefined("レベルアップ", sub_situation: ""))
                        {
                            SpecialEffect("レベルアップ", sub_situation: "");
                        }

                        if (IsMessageDefined("レベルアップ"))
                        {
                            PilotMessage("レベルアップ", msg_mode: "");
                        }

                        var msg = withBlock8.get_Nickname(false) + "は" + SrcFormatter.Format(earned_exp) + "の経験値を獲得し、" + "レベル[" + SrcFormatter.Format(withBlock8.Level) + "]にレベルアップ。";

                        // TODO Impl
                        //// 特殊能力の習得
                        //var loopTo18 = withBlock8.CountSkill();
                        //for (var i = 1; i <= loopTo18; i++)
                        //{
                        //    var sname = withBlock8.Skill(i);
                        //    string localSkillName2() { object argIndex1 = i; var ret = withBlock8.SkillName(argIndex1); return ret; }

                        //    if (Strings.InStr(localSkillName2(), "非表示") == 0)
                        //    {
                        //        switch (sname ?? "")
                        //        {
                        //            case "同調率":
                        //            case "霊力":
                        //            case "追加レベル":
                        //            case "魔力所有":
                        //                {
                        //                    break;
                        //                }

                        //            default:
                        //                {
                        //                    var loopTo19 = Information.UBound(prev_stype);
                        //                    for (j = 1; j <= loopTo19; j++)
                        //                    {
                        //                        if ((sname ?? "") == (prev_stype[j] ?? ""))
                        //                        {
                        //                            break;
                        //                        }
                        //                    }

                        //                    double localSkillLevel() { object argIndex1 = sname; string argref_mode = "基本値"; var ret = withBlock8.SkillLevel(argIndex1, argref_mode); return ret; }

                        //                    if (j > Information.UBound(prev_stype))
                        //                    {
                        //                        string localSkillName() { object argIndex1 = i; var ret = withBlock8.SkillName(argIndex1); return ret; }

                        //                        msg = msg + ";" + localSkillName() + "を習得。";
                        //                    }
                        //                    else if (localSkillLevel() > prev_slevel[j])
                        //                    {
                        //                        string localSkillName1() { object argIndex1 = i; var ret = withBlock8.SkillName(argIndex1); return ret; }

                        //                        msg = msg + ";" + prev_sname[j] + " => " + localSkillName1() + "。";
                        //                    }

                        //                    break;
                        //                }
                        //        }
                        //    }
                        //}

                        //// スペシャルパワーの習得
                        //if (withBlock8.CountSpecialPower > Information.UBound(prev_special_power))
                        //{
                        //    msg = msg + ";スペシャルパワー";
                        //    var loopTo20 = withBlock8.CountSpecialPower;
                        //    for (i = 1; i <= loopTo20; i++)
                        //    {
                        //        sname = withBlock8.get_SpecialPower(i);
                        //        var loopTo21 = Information.UBound(prev_special_power);
                        //        for (j = 1; j <= loopTo21; j++)
                        //        {
                        //            if ((sname ?? "") == (prev_special_power[j] ?? ""))
                        //            {
                        //                break;
                        //            }
                        //        }

                        //        if (j > Information.UBound(prev_special_power))
                        //        {
                        //            msg = msg + "「" + sname + "」";
                        //        }
                        //    }

                        //    msg = msg + "を習得。";
                        //}

                        //GUI.DisplaySysMessage(msg);
                        Event.HandleEvent("レベルアップ", withBlock8.ID);
                        SRC.PList.UpdateSupportMod(this);
                    }
                    else if (earned_exp > 0)
                    {
                        GUI.DisplaySysMessage(withBlock8.get_Nickname(false) + "は" + SrcFormatter.Format(earned_exp) + "の経験値を得た。");
                    }
                }
            }

            // スペシャルパワー効果の解除
            if (IsUnderSpecialPowerEffect("攻撃後消耗"))
            {
                AddCondition("消耗", 1, cdata: "");
            }

            RemoveSpecialPowerInEffect("攻撃");
            RemoveSpecialPowerInEffect("戦闘終了");
            if (isDestroyed)
            {
                RemoveSpecialPowerInEffect("敵破壊");
            }

            foreach (var tu in targets)
            {
                tu.CurrentForm().RemoveSpecialPowerInEffect("戦闘終了");
            }

            // 状態の解除
            foreach (var tu in targets)
            {
                tu.CurrentForm().UpdateCondition();
            }

            // ステルスが解ける？
            if (IsFeatureAvailable("ステルス"))
            {
                if (w.IsWeaponClassifiedAs("忍"))
                {
                    foreach (var tu in targets)
                    {
                        var t = tu.CurrentForm();
                        if (t.Status == "出撃" && t.MaxAction() > 0)
                        {
                            AddCondition("ステルス無効", 1, cdata: "");
                            break;
                        }
                    }
                }
                else
                {
                    AddCondition("ステルス無効", 1, cdata: "");
                }
            }

            // TODO Impl
            //// 合体技のパートナーの弾数＆ＥＮを消費
            //var loopTo25 = Information.UBound(partners);
            //for (i = 1; i <= loopTo25; i++)
            //{
            //    {
            //        var withBlock10 = partners[i].CurrentForm();
            //        var loopTo26 = withBlock10.CountWeapon();
            //        for (j = 1; j <= loopTo26; j++)
            //        {
            //            if ((withBlock10.Weapon(j).Name ?? "") == (wname ?? ""))
            //            {
            //                withBlock10.UseWeapon(j);
            //                if (withBlock10.IsWeaponClassifiedAs(j, "自"))
            //                {
            //                    if (withBlock10.IsFeatureAvailable("パーツ分離"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock10.FeatureData("パーツ分離"), 2);
            //                        Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                        {
            //                            withBlock10.Transform(uname);
            //                            {
            //                                var withBlock11 = withBlock10.CurrentForm();
            //                                withBlock11.HP = withBlock11.MaxHP;
            //                                withBlock11.UsedAction = withBlock11.MaxAction();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock10.Die();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        withBlock10.Die();
            //                    }
            //                }
            //                else if (withBlock10.IsWeaponClassifiedAs(j, "失") && withBlock10.HP == 0)
            //                {
            //                    withBlock10.Die();
            //                }
            //                else if (withBlock10.IsWeaponClassifiedAs(j, "変"))
            //                {
            //                    if (withBlock10.IsFeatureAvailable("変形技"))
            //                    {
            //                        var loopTo27 = withBlock10.CountFeature();
            //                        for (k = 1; k <= loopTo27; k++)
            //                        {
            //                            string localFeature() { object argIndex1 = k; var ret = withBlock10.Feature(argIndex1); return ret; }

            //                            string localFeatureData1() { object argIndex1 = k; var ret = withBlock10.FeatureData(argIndex1); return ret; }

            //                            string localLIndex() { string arglist = hs681d62aaff664a9daf19517e6120dc9d(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            if (localFeature() == "変形技" && (localLIndex() ?? "") == (wname ?? ""))
            //                            {
            //                                string localFeatureData() { object argIndex1 = k; var ret = withBlock10.FeatureData(argIndex1); return ret; }

            //                                uname = GeneralLib.LIndex(localFeatureData(), 2);
            //                                Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm1().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                                {
            //                                    withBlock10.Transform(uname);
            //                                }

            //                                break;
            //                            }
            //                        }

            //                        if ((uname ?? "") != (withBlock10.CurrentForm().Name ?? ""))
            //                        {
            //                            if (withBlock10.IsFeatureAvailable("ノーマルモード"))
            //                            {
            //                                uname = GeneralLib.LIndex(withBlock10.FeatureData("ノーマルモード"), 1);
            //                                Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm2().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                                {
            //                                    withBlock10.Transform(uname);
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else if (withBlock10.IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock10.FeatureData(argIndex16), 1);
            //                        Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm3().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                        {
            //                            withBlock10.Transform(uname);
            //                        }
            //                    }
            //                }

            //                break;
            //            }
            //        }

            //        // 同名の武器がなかった場合は自分のデータを使って処理
            //        if (j > withBlock10.CountWeapon())
            //        {
            //            if (this.Weapon(w).ENConsumption > 0)
            //            {
            //                withBlock10.EN = withBlock10.EN - WeaponENConsumption(w);
            //            }

            //            if (w.IsWeaponClassifiedAs("消"))
            //            {
            //                withBlock10.AddCondition("消耗", 1, cdata: "");
            //            }

            //            if (w.IsWeaponClassifiedAs("Ｃ") && withBlock10.IsConditionSatisfied("チャージ完了"))
            //            {
            //                withBlock10.DeleteCondition("チャージ完了");
            //            }

            //            if (w.IsWeaponClassifiedAs("気"))
            //            {
            //                withBlock10.IncreaseMorale((-5 * (int)w.WeaponLevel("気")));
            //            }

            //            if (w.IsWeaponClassifiedAs("霊"))
            //            {
            //                hp_ratio = 100 * withBlock10.HP / (double)withBlock10.MaxHP;
            //                en_ratio = 100 * withBlock10.EN / (double)withBlock10.MaxEN;
            //                withBlock10.MainPilot().Plana = (withBlock10.MainPilot().Plana - 5d * (int)w.WeaponLevel("霊"));
            //                withBlock10.HP = (withBlock10.MaxHP * hp_ratio / 100d);
            //                withBlock10.EN = (withBlock10.MaxEN * en_ratio / 100d);
            //            }
            //            else if (w.IsWeaponClassifiedAs("プ"))
            //            {
            //                hp_ratio = 100 * withBlock10.HP / (double)withBlock10.MaxHP;
            //                en_ratio = 100 * withBlock10.EN / (double)withBlock10.MaxEN;
            //                withBlock10.MainPilot().Plana = (withBlock10.MainPilot().Plana - 5d * (int)w.WeaponLevel("プ"));
            //                withBlock10.HP = (withBlock10.MaxHP * hp_ratio / 100d);
            //                withBlock10.EN = (withBlock10.MaxEN * en_ratio / 100d);
            //            }

            //            if (w.IsWeaponClassifiedAs("失"))
            //            {
            //                withBlock10.HP = GeneralLib.MaxLng((withBlock10.HP - (long)(withBlock10.MaxHP * (int)w.WeaponLevel("失")) / 10L), 0);
            //            }

            //            if (w.IsWeaponClassifiedAs("自"))
            //            {
            //                if (withBlock10.IsFeatureAvailable("パーツ分離"))
            //                {
            //                    uname = GeneralLib.LIndex(withBlock10.FeatureData("パーツ分離"), 2);
            //                    Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm4().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                    {
            //                        withBlock10.Transform(uname);
            //                        {
            //                            var withBlock12 = withBlock10.CurrentForm();
            //                            withBlock12.HP = withBlock12.MaxHP;
            //                            withBlock12.UsedAction = withBlock12.MaxAction();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        withBlock10.Die();
            //                    }
            //                }
            //                else
            //                {
            //                    withBlock10.Die();
            //                }
            //            }
            //            else if (w.IsWeaponClassifiedAs("失") && withBlock10.HP == 0)
            //            {
            //                withBlock10.Die();
            //            }
            //            else if (w.IsWeaponClassifiedAs("変"))
            //            {
            //                if (withBlock10.IsFeatureAvailable("ノーマルモード"))
            //                {
            //                    uname = GeneralLib.LIndex(withBlock10.FeatureData(argIndex20), 1);
            //                    Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock10.OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm5().IsAbleToEnter(withBlock10.x, withBlock10.y))
            //                    {
            //                        withBlock10.Transform(uname);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            // TODO IMpl
            //// 以下の特殊効果は武器データが変化する可能性があるため、同時には適用されない

            //// 自爆の処理

            //// ＨＰ消費攻撃による自殺

            //// 変形技
            //if (w.IsWeaponClassifiedAs("自"))
            //{
            //    if (IsFeatureAvailable("パーツ分離"))
            //    {
            //        uname = GeneralLib.LIndex(FeatureData("パーツ分離"), 2);
            //        Unit localOtherForm6() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //        if (localOtherForm6().IsAbleToEnter(x, y))
            //        {
            //            Transform(uname);
            //            {
            //                var withBlock13 = CurrentForm();
            //                withBlock13.HP = withBlock13.MaxHP;
            //                withBlock13.UsedAction = withBlock13.MaxAction();
            //            }

            //            fname = FeatureName("パーツ分離");
            //            bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            if (IsSysMessageDefined("破壊時分離(" + Name + ")", sub_situation: ""))
            //            {
            //                SysMessage("破壊時分離(" + Name + ")", sub_situation: "", add_msg: "");
            //            }
            //            else if (localIsSysMessageDefined())
            //            {
            //                SysMessage("破壊時分離(" + fname + ")", sub_situation: "", add_msg: "");
            //            }
            //            else if (IsSysMessageDefined("破壊時分離", sub_situation: ""))
            //            {
            //                SysMessage("破壊時分離", sub_situation: "", add_msg: "");
            //            }
            //            else if (localIsSysMessageDefined1())
            //            {
            //                SysMessage("分離(" + Name + ")", sub_situation: "", add_msg: "");
            //            }
            //            else if (localIsSysMessageDefined2())
            //            {
            //                SysMessage("分離(" + fname + ")", sub_situation: "", add_msg: "");
            //            }
            //            else if (IsSysMessageDefined("分離", sub_situation: ""))
            //            {
            //                SysMessage("分離", sub_situation: "", add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
            //            }
            //        }
            //        else
            //        {
            //            Die();
            //            GUI.CloseMessageForm();
            //            if (!is_event)
            //            {
            //                Event.HandleEvent("破壊", MainPilot().ID);
            //                if (SRC.IsScenarioFinished)
            //                {
            //                    return;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Die();
            //        GUI.CloseMessageForm();
            //        if (!is_event)
            //        {
            //            Event.HandleEvent("破壊", MainPilot().ID);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                return;
            //            }
            //        }
            //    }
            //}
            //else if (w.IsWeaponClassifiedAs("失") && HP == 0)
            //{
            //    Die();
            //    GUI.CloseMessageForm();
            //    if (!is_event)
            //    {
            //        Event.HandleEvent("破壊", MainPilot().ID);
            //        if (SRC.IsScenarioFinished)
            //        {
            //            return;
            //        }
            //    }
            //}
            //else if (w.IsWeaponClassifiedAs("変"))
            //{
            //    if (IsFeatureAvailable("変形技"))
            //    {
            //        var loopTo28 = CountFeature();
            //        for (i = 1; i <= loopTo28; i++)
            //        {
            //            string localFeature1() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //            string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //            string localLIndex1() { string arglist = hs367294acea73430b9129abf82f13a58e(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            if (localFeature1() == "変形技" && (localLIndex1() ?? "") == (wname ?? ""))
            //            {
            //                string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                uname = GeneralLib.LIndex(localFeatureData2(), 2);
            //                Unit localOtherForm7() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                if (localOtherForm7().IsAbleToEnter(x, y))
            //                {
            //                    Transform(uname);
            //                }

            //                break;
            //            }
            //        }

            //        if ((uname ?? "") != (CurrentForm().Name ?? ""))
            //        {
            //            if (IsFeatureAvailable("ノーマルモード"))
            //            {
            //                uname = GeneralLib.LIndex(FeatureData(argIndex23), 1);
            //                Unit localOtherForm8() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                if (localOtherForm8().IsAbleToEnter(x, y))
            //                {
            //                    Transform(uname);
            //                }
            //            }
            //        }
            //    }
            //    else if (IsFeatureAvailable("ノーマルモード"))
            //    {
            //        uname = GeneralLib.LIndex(FeatureData(argIndex24), 1);
            //        Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //        if (localOtherForm9().IsAbleToEnter(x, y))
            //        {
            //            Transform(uname);
            //        }
            //    }
            //}

            //// アイテムを消費
            //else if (Weapon(w).IsItem() && Bullet(w) == 0 && MaxBullet(w) > 0)
            //{
            //    // アイテムを削除
            //    num = Data.CountWeapon();
            //    num = (num + MainPilot().Data.CountWeapon());
            //    var loopTo29 = CountPilot();
            //    for (i = 2; i <= loopTo29; i++)
            //    {
            //        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //        num = (num + localPilot().Data.CountWeapon());
            //    }

            //    var loopTo30 = CountSupport();
            //    for (i = 2; i <= loopTo30; i++)
            //    {
            //        Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //        num = (num + localSupport().Data.CountWeapon());
            //    }

            //    if (IsFeatureAvailable("追加サポート"))
            //    {
            //        num = (num + AdditionalSupport().Data.CountWeapon());
            //    }

            //    foreach (Item itm in colItem)
            //    {
            //        num = (num + itm.CountWeapon());
            //        if (w <= num)
            //        {
            //            itm.Exist = false;
            //            DeleteItem((object)itm.ID);
            //            break;
            //        }
            //    }
            //}

            GUI.CloseMessageForm();
        DoEvent:
            ;


            // イベント処理
            if (!is_event)
            {
                foreach (var tu in targets)
                {
                    var t = tu.CurrentForm();
                    var prev = targetsInfo.First(x => x.t.ID == t.ID);
                    if (t.Status == "破壊")
                    {
                        // 破壊イベントを発生
                        Commands.SaveSelections();
                        Commands.SwapSelections();
                        Event.HandleEvent("マップ攻撃破壊", t.MainPilot().ID);
                        Commands.RestoreSelections();
                        if (SRC.IsScenarioFinished || SRC.IsCanceled)
                        {
                            return;
                        }
                    }
                    else if (t.Status == "出撃")
                    {
                        if (t.HP / (double)t.MaxHP < prev.targets_hp_ratio)
                        {
                            // 損傷率イベント
                            Commands.SaveSelections();
                            Commands.SwapSelections();
                            Event.HandleEvent("損傷率", t.MainPilot().ID, "" + (100 * (t.MaxHP - t.HP) / t.MaxHP));
                            Commands.RestoreSelections();
                            if (SRC.IsScenarioFinished || SRC.IsCanceled)
                            {
                                return;
                            }
                        }

                        // ターゲットが動いていたら進入イベントを発生
                        {
                            var withBlock14 = t.CurrentForm();
                            if (withBlock14.Status == "出撃" && (withBlock14.x != prev.targets_x || withBlock14.y != prev.targets_y))
                            {
                                Event.HandleEvent("進入", withBlock14.MainPilot().ID, "" + withBlock14.x, "" + withBlock14.y);
                                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }

                // 全滅イベント
                foreach (var uparty in new string[] { "味方", "ＮＰＣ", "敵", "中立" })
                {
                    var found = false;
                    foreach (var tu in targets)
                    {
                        var t = tu.CurrentForm();
                        if ((t.Party0 ?? "") == (uparty ?? "") && t.Status != "出撃")
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        found = false;
                        foreach (Unit u in SRC.UList.Items)
                        {
                            if ((u.Party0 ?? "") == (uparty ?? "") && u.Status == "出撃" && !u.IsConditionSatisfied("憑依"))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            Event.HandleEvent("全滅", uparty);
                            if (SRC.IsScenarioFinished || SRC.IsCanceled)
                            {
                                return;
                            }
                        }
                    }
                }

                // 使用後イベント
                if (CurrentForm().Status == "出撃")
                {
                    Event.HandleEvent("使用後", CurrentForm().MainPilot().ID, wname);
                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
                    {
                        return;
                    }
                }

                // 攻撃後イベント
                if (CurrentForm().Status == "出撃")
                {
                    Commands.SaveSelections();
                    foreach (var tu in targets)
                    {
                        Commands.SelectedTarget = tu.CurrentForm();
                        {
                            var withBlock16 = Commands.SelectedTarget;
                            if (withBlock16.Status == "出撃")
                            {
                                Event.HandleEvent("攻撃後", CurrentForm().MainPilot().ID, withBlock16.MainPilot().ID);
                                if (SRC.IsScenarioFinished)
                                {
                                    Commands.RestoreSelections();
                                    return;
                                }
                            }
                        }
                    }

                    Commands.RestoreSelections();
                }
            }

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
        }
    }
}
