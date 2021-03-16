using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === 特殊効果関連処理 ===
    public partial class Unit
    {
        // 特殊効果データを検索
        public string SpecialEffectData(ref string main_situation, [Optional, DefaultParameterValue("")] ref string sub_situation)
        {
            string SpecialEffectDataRet = default;
            string uname, uclass;
            string[] situations;
            short i, ret;

            // シチュエーションのリストを構築
            if (string.IsNullOrEmpty(sub_situation) | (main_situation ?? "") == (sub_situation ?? ""))
            {
                situations = new string[2];
                situations[1] = main_situation;
            }
            else
            {
                situations = new string[3];
                situations[1] = main_situation + "(" + sub_situation + ")";
                situations[2] = main_situation;
            }

            var loopTo = (short)Information.UBound(situations);
            for (i = 1; i <= loopTo; i++)
            {
                // 特殊効果能力で指定された名称で検索
                string argfname = "特殊効果";
                if (IsFeatureAvailable(ref argfname))
                {
                    object argIndex1 = "特殊効果";
                    uname = FeatureData(ref argIndex1);
                    object argIndex2 = uname;
                    if (SRC.EDList.IsDefined(ref argIndex2))
                    {
                        MessageData localItem() { object argIndex1 = uname; var ret = SRC.EDList.Item(ref argIndex1); return ret; }

                        var argu = this;
                        SpecialEffectDataRet = localItem().SelectMessage(ref situations[i], ref argu);
                        if (Strings.Len(SpecialEffectDataRet) > 0)
                        {
                            return SpecialEffectDataRet;
                        }
                    }
                }

                // ユニット名称で検索
                bool localIsDefined() { object argIndex1 = Name; var ret = SRC.EDList.IsDefined(ref argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                if (localIsDefined())
                {
                    MessageData localItem1() { object argIndex1 = Name; var ret = SRC.EDList.Item(ref argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                    var argu1 = this;
                    SpecialEffectDataRet = localItem1().SelectMessage(ref situations[i], ref argu1);
                    if (Strings.Len(SpecialEffectDataRet) > 0)
                    {
                        return SpecialEffectDataRet;
                    }
                }

                // ユニット愛称を修正したもので検索
                uname = Nickname;
                ret = (short)Strings.InStr(uname, "(");
                if (ret > 1)
                {
                    uname = Strings.Left(uname, ret - 1);
                }

                ret = (short)Strings.InStr(uname, "用");
                if (ret > 0)
                {
                    if (ret < Strings.Len(uname))
                    {
                        uname = Strings.Mid(uname, ret + 1);
                    }
                }

                ret = (short)Strings.InStr(uname, "型");
                if (ret > 0)
                {
                    if (ret < Strings.Len(uname))
                    {
                        uname = Strings.Mid(uname, ret + 1);
                    }
                }

                if (Strings.Right(uname, 4) == "カスタム")
                {
                    uname = Strings.Left(uname, Strings.Len(uname) - 4);
                }

                if (Strings.Right(uname, 1) == "改")
                {
                    uname = Strings.Left(uname, Strings.Len(uname) - 1);
                }

                object argIndex3 = uname;
                if (SRC.EDList.IsDefined(ref argIndex3))
                {
                    MessageData localItem2() { object argIndex1 = uname; var ret = SRC.EDList.Item(ref argIndex1); return ret; }

                    var argu2 = this;
                    SpecialEffectDataRet = localItem2().SelectMessage(ref situations[i], ref argu2);
                    if (Strings.Len(SpecialEffectDataRet) > 0)
                    {
                        return SpecialEffectDataRet;
                    }
                }

                // ユニットクラスで検索
                uclass = Class0;
                object argIndex4 = uclass;
                if (SRC.EDList.IsDefined(ref argIndex4))
                {
                    MessageData localItem3() { object argIndex1 = uclass; var ret = SRC.EDList.Item(ref argIndex1); return ret; }

                    var argu3 = this;
                    SpecialEffectDataRet = localItem3().SelectMessage(ref situations[i], ref argu3);
                    if (Strings.Len(SpecialEffectDataRet) > 0)
                    {
                        return SpecialEffectDataRet;
                    }
                }

                // 汎用
                object argIndex6 = "汎用";
                if (SRC.EDList.IsDefined(ref argIndex6))
                {
                    object argIndex5 = "汎用";
                    var argu4 = this;
                    SpecialEffectDataRet = SRC.EDList.Item(ref argIndex5).SelectMessage(ref situations[i], ref argu4);
                    if (Strings.Len(SpecialEffectDataRet) > 0)
                    {
                        return SpecialEffectDataRet;
                    }
                }
            }

            return SpecialEffectDataRet;
        }

        // 特殊効果データを再生
        public void SpecialEffect(ref string main_situation, [Optional, DefaultParameterValue("")] ref string sub_situation, bool keep_message_form = false)
        {
            string anime, sname;
            string[] animes;
            short idx, i, j, w = default;
            double ret;
            string buf;
            short anime_head;
            bool is_message_form_opened;
            var is_weapon = default(bool);
            var is_ability = default(bool);
            bool in_bulk;
            var wait_time = default(int);
            var need_refresh = default(bool);
            int prev_obj_color;
            int prev_obj_fill_color;
            int prev_obj_fill_style;
            int prev_obj_draw_width;
            string prev_obj_draw_option;
            Unit prev_selected_target;

            // 特殊効果データを検索
            anime = SpecialEffectData(ref main_situation, ref sub_situation);
            GeneralLib.TrimString(ref anime);

            // 表示キャンセル
            if (string.IsNullOrEmpty(anime) | anime == "-")
            {
                return;
            }

            // マウスの右ボタンでキャンセル
            if (GUI.IsRButtonPressed())
            {
                // 式評価のみ行う
                Expression.FormatMessage(ref anime);
                return;
            }

            string argoname = "戦闘アニメ非自動選択";
            if (SRC.BattleAnimation & !Expression.IsOptionDefined(ref argoname))
            {
                var loopTo = CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    if ((Weapon(i).Name ?? "") == (main_situation ?? ""))
                    {
                        w = i;
                        break;
                    }
                }

                if (w > 0)
                {
                    switch (Strings.LCase(anime) ?? "")
                    {
                        case "swing.wav":
                            {
                                string argattr = "武";
                                string argattr1 = "実";
                                if (Strings.InStr(main_situation, "槍") > 0 | Strings.InStr(main_situation, "スピア") > 0 | Strings.InStr(main_situation, "ランス") > 0 | Strings.InStr(main_situation, "ジャベリン") > 0)
                                {
                                    string arganame = "刺突攻撃";
                                    Effect.ShowAnimation(ref arganame);
                                    return;
                                }
                                else if (IsWeaponClassifiedAs(w, ref argattr) | IsWeaponClassifiedAs(w, ref argattr1))
                                {
                                    string arganame1 = "白兵武器攻撃";
                                    Effect.ShowAnimation(ref arganame1);
                                    return;
                                }

                                break;
                            }
                    }
                }
                else if (Strings.InStr(main_situation, "(命中)") > 0)
                {
                    switch (Strings.LCase(anime) ?? "")
                    {
                        case "break.wav":
                            {
                                string arganame2 = "打撃命中";
                                Effect.ShowAnimation(ref arganame2);
                                return;
                            }

                        case "combo.wav":
                            {
                                string arganame3 = "乱打命中";
                                Effect.ShowAnimation(ref arganame3);
                                return;
                            }

                        case "crash.wav":
                            {
                                string arganame4 = "強打命中 Crash.wav";
                                Effect.ShowAnimation(ref arganame4);
                                return;
                            }

                        case "explode.wav":
                            {
                                string arganame5 = "爆発命中";
                                Effect.ShowAnimation(ref arganame5);
                                return;
                            }

                        case "explode(far).wav":
                            {
                                string arganame6 = "超爆発命中 Explode(Far).wav";
                                Effect.ShowAnimation(ref arganame6);
                                return;
                            }

                        case "explode(nuclear).wav":
                            {
                                string arganame7 = "超爆発命中 Explode(Nuclear).wav";
                                Effect.ShowAnimation(ref arganame7);
                                return;
                            }

                        case "fire.wav":
                            {
                                string arganame8 = "炎命中";
                                Effect.ShowAnimation(ref arganame8);
                                return;
                            }

                        case "glass.wav":
                            {
                                string argattr2 = "冷";
                                if (IsWeaponClassifiedAs(w, ref argattr2))
                                {
                                    string arganame9 = "凍結命中 Glass.wav";
                                    Effect.ShowAnimation(ref arganame9);
                                }

                                return;
                            }

                        case "punch.wav":
                            {
                                string arganame10 = "打撃命中";
                                Effect.ShowAnimation(ref arganame10);
                                return;
                            }

                        case "punch(2).wav":
                        case "punch(3).wav":
                        case "punch(4).wav":
                            {
                                string arganame11 = "連打命中";
                                Effect.ShowAnimation(ref arganame11);
                                return;
                            }

                        case "saber.wav":
                        case "slash.wav":
                            {
                                string arganame12 = "斬撃命中 " + anime;
                                Effect.ShowAnimation(ref arganame12);
                                return;
                            }

                        case "shock(low).wav":
                            {
                                string arganame13 = "強打命中 Shock(Low).wav";
                                Effect.ShowAnimation(ref arganame13);
                                return;
                            }

                        case "stab.wav":
                            {
                                string arganame14 = "刺突命中";
                                Effect.ShowAnimation(ref arganame14);
                                return;
                            }

                        case "thunder.wav":
                            {
                                string arganame15 = "放電命中 Thunder.wav";
                                Effect.ShowAnimation(ref arganame15);
                                return;
                            }

                        case "whip.wav":
                            {
                                string arganame16 = "打撃命中 Whip.wav";
                                Effect.ShowAnimation(ref arganame16);
                                return;
                            }
                    }
                }
            }

            // メッセージウィンドウは表示されている？
            is_message_form_opened = My.MyProject.Forms.frmMessage.Visible;

            // オブジェクト色等を記録しておく
            prev_obj_color = Event_Renamed.ObjColor;
            prev_obj_fill_color = Event_Renamed.ObjFillColor;
            prev_obj_fill_style = Event_Renamed.ObjFillStyle;
            prev_obj_draw_width = Event_Renamed.ObjDrawWidth;
            prev_obj_draw_option = Event_Renamed.ObjDrawOption;

            // オブジェクト色等をデフォルトに戻す
            Event_Renamed.ObjColor = ColorTranslator.ToOle(Color.White);
            Event_Renamed.ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            Event_Renamed.ObjFillStyle = vbFSTransparent;
            Event_Renamed.ObjDrawWidth = 1;
            Event_Renamed.ObjDrawOption = "";

            // 検索するシチュエーションが武器名かどうか調べる
            var loopTo1 = CountWeapon();
            for (i = 1; i <= loopTo1; i++)
            {
                if ((main_situation ?? "") == (Weapon(i).Name ?? ""))
                {
                    is_weapon = true;
                    break;
                }
            }

            // 検索するシチュエーションがアビリティかどうか調べる
            var loopTo2 = CountAbility();
            for (i = 1; i <= loopTo2; i++)
            {
                if ((main_situation ?? "") == (Ability(i).Name ?? ""))
                {
                    is_ability = true;
                    break;
                }
            }

            // イベント用ターゲットを記録しておく
            prev_selected_target = Event_Renamed.SelectedTargetForEvent;

            // 攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
            // 自分自身をターゲットに設定する
            // (発動アニメではアニメ表示にSelectedTargetForEventが使われるため)
            if (!is_weapon & !is_ability)
            {
                if (Event_Renamed.SelectedTargetForEvent is null)
                {
                    Event_Renamed.SelectedTargetForEvent = this;
                }
            }

            // 特殊効果指定を分割
            animes = new string[2];
            anime_head = 1;
            var loopTo3 = (short)Strings.Len(anime);
            for (i = 1; i <= loopTo3; i++)
            {
                if (Strings.Mid(anime, i, 1) == ";")
                {
                    animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head, i - anime_head);
                    Array.Resize(ref animes, Information.UBound(animes) + 1 + 1);
                    anime_head = (short)(i + 1);
                }
            }

            animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head);
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 743599


            Input:

                    On Error GoTo ErrorHandler

             */
            var loopTo4 = (short)Information.UBound(animes);
            for (i = 1; i <= loopTo4; i++)
            {
                anime = animes[i];

                // 式評価
                Expression.FormatMessage(ref anime);

                // 画面クリア？
                if (Strings.LCase(anime) == "clear")
                {
                    GUI.ClearPicture();
                    need_refresh = true;
                    goto NextAnime;
                }

                // 特殊効果
                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(ref anime, 1), 4)) ?? "")
                {
                    case ".wav":
                    case ".mp3":
                        {
                            // 効果音
                            Sound.PlayWave(ref anime);
                            if (wait_time > 0)
                            {
                                if (need_refresh)
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.picMain(0).Refresh();
                                    need_refresh = false;
                                }

                                GUI.Sleep(wait_time);
                                wait_time = 0;
                            }

                            goto NextAnime;
                            break;
                        }

                    case ".bmp":
                    case ".jpg":
                    case ".gif":
                    case ".png":
                        {
                            // カットインの表示
                            if (wait_time > 0)
                            {
                                anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                                wait_time = 0;
                                need_refresh = false;
                            }
                            else if (Strings.Left(anime, 1) == "@")
                            {
                                need_refresh = false;
                            }
                            else
                            {
                                need_refresh = true;
                            }

                            string argpname = "-";
                            string argmsg_mode = "";
                            GUI.DisplayBattleMessage(ref argpname, anime, msg_mode: ref argmsg_mode);
                            goto NextAnime;
                            break;
                        }
                }

                switch (Strings.LCase(GeneralLib.LIndex(ref anime, 1)) ?? "")
                {
                    case "line":
                    case "circle":
                    case "arc":
                    case "oval":
                    case "color":
                    case "fillcolor":
                    case "fillstyle":
                    case "drawwidth":
                        {
                            // 画面処理コマンド
                            if (wait_time > 0)
                            {
                                anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                                wait_time = 0;
                                need_refresh = false;
                            }
                            else
                            {
                                need_refresh = true;
                            }

                            string argpname1 = "-";
                            string argmsg_mode1 = "";
                            GUI.DisplayBattleMessage(ref argpname1, anime, msg_mode: ref argmsg_mode1);
                            goto NextAnime;
                            break;
                        }

                    case "center":
                        {
                            // 指定したユニットを中央表示
                            string argexpr = GeneralLib.ListIndex(ref anime, 2);
                            buf = Expression.GetValueAsString(ref argexpr);
                            object argIndex2 = buf;
                            if (SRC.UList.IsDefined(ref argIndex2))
                            {
                                object argIndex1 = buf;
                                {
                                    var withBlock = SRC.UList.Item(ref argIndex1);
                                    GUI.Center(withBlock.x, withBlock.y);
                                    GUI.RedrawScreen();
                                    need_refresh = false;
                                }
                            }

                            goto NextAnime;
                            break;
                        }

                    case "keep":
                        {
                            // そのまま終了
                            break;
                        }
                }

                // ウェイト？
                if (Information.IsNumeric(anime))
                {
                    wait_time = (int)(100d * Conversions.ToDouble(anime));
                    goto NextAnime;
                }

                // メッセージ表示として処理
                if (wait_time > 0)
                {
                    anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                    wait_time = 0;
                }

                if (!My.MyProject.Forms.frmMessage.Visible)
                {
                    if (ReferenceEquals(Commands.SelectedTarget, this))
                    {
                        var argu1 = this;
                        Unit argu2 = null;
                        GUI.OpenMessageForm(ref argu1, u2: ref argu2);
                    }
                    else
                    {
                        var argu21 = this;
                        GUI.OpenMessageForm(ref Commands.SelectedTarget, ref argu21);
                    }
                }

                string argpname2 = "-";
                string argmsg_mode2 = "";
                GUI.DisplayBattleMessage(ref argpname2, anime, msg_mode: ref argmsg_mode2);
                goto NextAnime;
            NextAnime:
                ;
            }

            string argoname1 = "戦闘アニメ非自動選択";
            if (SRC.BattleAnimation & !GUI.IsPictureDrawn & !Expression.IsOptionDefined(ref argoname1))
            {
                if (w > 0)
                {
                    string arganame17 = "デフォルト攻撃";
                    Effect.ShowAnimation(ref arganame17);
                }
                else if (Strings.InStr(main_situation, "(命中)") > 0)
                {
                    string arganame18 = "ダメージ命中 -.wav";
                    Effect.ShowAnimation(ref arganame18);
                }
            }

            // 特殊効果再生後にウェイトを入れる？
            if (need_refresh)
            {
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                GUI.MainForm.picMain(0).Refresh();
                need_refresh = false;
            }

            if (wait_time > 0)
            {
                GUI.Sleep(wait_time);
                wait_time = 0;
            }

            // 画像を消去しておく
            if (GUI.IsPictureDrawn & Strings.InStr(main_situation, "(準備)") == 0 & Strings.LCase(anime) != "keep")
            {
                GUI.ClearPicture();
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                GUI.MainForm.picMain(0).Refresh();
            }

            // 最初から表示されていたのでなければメッセージウィンドウを閉じる
            if (!is_message_form_opened & !keep_message_form)
            {
                GUI.CloseMessageForm();
            }

            // オブジェクト色等を元に戻す
            Event_Renamed.ObjColor = prev_obj_color;
            Event_Renamed.ObjFillColor = prev_obj_fill_color;
            Event_Renamed.ObjFillStyle = prev_obj_fill_style;
            Event_Renamed.ObjDrawWidth = prev_obj_draw_width;
            Event_Renamed.ObjDrawOption = prev_obj_draw_option;

            // イベント用ターゲットを元に戻す
            Event_Renamed.SelectedTargetForEvent = prev_selected_target;
            return;
        ErrorHandler:
            ;
            if (Strings.Len(Event_Renamed.EventErrorMessage) > 0)
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, Event_Renamed.EventErrorMessage);
                Event_Renamed.EventErrorMessage = "";
            }
            else
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "");
            }
        }

        // 特殊効果データが定義されているか？
        public bool IsSpecialEffectDefined(ref string main_situation, [Optional, DefaultParameterValue("")] ref string sub_situation)
        {
            bool IsSpecialEffectDefinedRet = default;
            string msg;
            msg = SpecialEffectData(ref main_situation, ref sub_situation);
            if (Strings.Len(msg) > 0)
            {
                IsSpecialEffectDefinedRet = true;
            }

            return IsSpecialEffectDefinedRet;
        }
    }
}
