using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    // === 特殊効果関連処理 ===
    public partial class Unit
    {
        // 特殊効果データを検索
        public string SpecialEffectData(string main_situation, string sub_situation = "")
        {
            var situations = new List<string>() { main_situation };
            // シチュエーションのリストを構築
            if (!string.IsNullOrEmpty(sub_situation) && (main_situation ?? "") != (sub_situation ?? ""))
            {
                situations.Insert(0, main_situation + "(" + sub_situation + ")");
            }

            for (var i = 0; i < situations.Count; i++)
            {
                // 特殊効果能力で指定された名称で検索
                if (IsFeatureAvailable("特殊効果"))
                {
                    var uname = FeatureData("特殊効果");
                    if (SRC.EDList.IsDefined(uname))
                    {
                        var res = SRC.EDList.Item(uname).SelectMessage(situations[i], this);
                        if (Strings.Len(res) > 0)
                        {
                            return res;
                        }
                    }
                }

                // ユニット名称で検索
                if (SRC.EDList.IsDefined(Name))
                {
                    var res = SRC.EDList.Item(Name).SelectMessage(situations[i], this);
                    if (Strings.Len(res) > 0)
                    {
                        return res;
                    }
                }

                // ユニット愛称を修正したもので検索
                {
                    var uname = Nickname;
                    var ret = Strings.InStr(uname, "(");
                    if (ret > 1)
                    {
                        uname = Strings.Left(uname, ret - 1);
                    }

                    ret = Strings.InStr(uname, "用");
                    if (ret > 0)
                    {
                        if (ret < Strings.Len(uname))
                        {
                            uname = Strings.Mid(uname, ret + 1);
                        }
                    }

                    ret = Strings.InStr(uname, "型");
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

                    if (SRC.EDList.IsDefined(uname))
                    {
                        var res = SRC.EDList.Item(uname).SelectMessage(situations[i], this);
                        if (Strings.Len(res) > 0)
                        {
                            return res;
                        }
                    }
                }
                // ユニットクラスで検索
                var uclass = Class0;
                if (SRC.EDList.IsDefined(uclass))
                {
                    var res = SRC.EDList.Item(uclass).SelectMessage(situations[i], this);
                    if (Strings.Len(res) > 0)
                    {
                        return res;
                    }
                }

                // 汎用
                if (SRC.EDList.IsDefined("汎用"))
                {
                    var res = SRC.EDList.Item("汎用").SelectMessage(situations[i], this);
                    if (Strings.Len(res) > 0)
                    {
                        return res;
                    }
                }
            }

            return "";
        }

        // 特殊効果データを再生
        public void SpecialEffect(string main_situation, string sub_situation = "", bool keep_message_form = false)
        {
            // TODO Impl SpecialEffect
            //            string anime, sname;
            //            string[] animes;
            //            int idx, i, j, w = default;
            //            double ret;
            //            string buf;
            //            int anime_head;
            //            bool is_message_form_opened;
            //            var is_weapon = default(bool);
            //            var is_ability = default(bool);
            //            bool in_bulk;
            //            var wait_time = default;
            //            var need_refresh = default(bool);
            //            int prev_obj_color;
            //            int prev_obj_fill_color;
            //            int prev_obj_fill_style;
            //            int prev_obj_draw_width;
            //            string prev_obj_draw_option;
            //            Unit prev_selected_target;

            //            // 特殊効果データを検索
            //            anime = SpecialEffectData(main_situation, sub_situation);
            //            GeneralLib.TrimString(anime);

            //            // 表示キャンセル
            //            if (string.IsNullOrEmpty(anime) || anime == "-")
            //            {
            //                return;
            //            }

            //            // マウスの右ボタンでキャンセル
            //            if (GUI.IsRButtonPressed())
            //            {
            //                // 式評価のみ行う
            //                Expression.FormatMessage(anime);
            //                return;
            //            }

            //            if (SRC.BattleAnimation && !Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            //            {
            //                var loopTo = CountWeapon();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    if ((Weapon(i).Name ?? "") == (main_situation ?? ""))
            //                    {
            //                        w = i;
            //                        break;
            //                    }
            //                }

            //                if (w > 0)
            //                {
            //                    switch (Strings.LCase(anime) ?? "")
            //                    {
            //                        case "swing.wav":
            //                            {
            //                                if (Strings.InStr(main_situation, "槍") > 0 || Strings.InStr(main_situation, "スピア") > 0 || Strings.InStr(main_situation, "ランス") > 0 || Strings.InStr(main_situation, "ジャベリン") > 0)
            //                                {
            //                                    Effect.ShowAnimation("刺突攻撃");
            //                                    return;
            //                                }
            //                                else if (IsWeaponClassifiedAs(w, "武") || IsWeaponClassifiedAs(w, "武"1))
            //                                {
            //                                    Effect.ShowAnimation("白兵武器攻撃");
            //                                    return;
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }
            //                else if (Strings.InStr(main_situation, "(命中)") > 0)
            //                {
            //                    switch (Strings.LCase(anime) ?? "")
            //                    {
            //                        case "break.wav":
            //                            {
            //                                Effect.ShowAnimation("打撃命中");
            //                                return;
            //                            }

            //                        case "combo.wav":
            //                            {
            //                                Effect.ShowAnimation("乱打命中");
            //                                return;
            //                            }

            //                        case "crash.wav":
            //                            {
            //                                Effect.ShowAnimation("強打命中 Crash.wav");
            //                                return;
            //                            }

            //                        case "explode.wav":
            //                            {
            //                                Effect.ShowAnimation("爆発命中");
            //                                return;
            //                            }

            //                        case "explode(far).wav":
            //                            {
            //                                Effect.ShowAnimation("超爆発命中 Explode(Far).wav");
            //                                return;
            //                            }

            //                        case "explode(nuclear).wav":
            //                            {
            //                                Effect.ShowAnimation("超爆発命中 Explode(Nuclear).wav");
            //                                return;
            //                            }

            //                        case "fire.wav":
            //                            {
            //                                Effect.ShowAnimation("炎命中");
            //                                return;
            //                            }

            //                        case "glass.wav":
            //                            {
            //                                if (IsWeaponClassifiedAs(w, "冷"))
            //                                {
            //                                    Effect.ShowAnimation("凍結命中 Glass.wav");
            //                                }

            //                                return;
            //                            }

            //                        case "punch.wav":
            //                            {
            //                                Effect.ShowAnimation("打撃命中");
            //                                return;
            //                            }

            //                        case "punch(2).wav":
            //                        case "punch(3).wav":
            //                        case "punch(4).wav":
            //                            {
            //                                Effect.ShowAnimation("連打命中");
            //                                return;
            //                            }

            //                        case "saber.wav":
            //                        case "slash.wav":
            //                            {
            //                                Effect.ShowAnimation("斬撃命中 " + anime);
            //                                return;
            //                            }

            //                        case "shock(low).wav":
            //                            {
            //                                Effect.ShowAnimation("強打命中 Shock(Low).wav");
            //                                return;
            //                            }

            //                        case "stab.wav":
            //                            {
            //                                Effect.ShowAnimation("刺突命中");
            //                                return;
            //                            }

            //                        case "thunder.wav":
            //                            {
            //                                Effect.ShowAnimation("放電命中 Thunder.wav");
            //                                return;
            //                            }

            //                        case "whip.wav":
            //                            {
            //                                Effect.ShowAnimation("打撃命中 Whip.wav");
            //                                return;
            //                            }
            //                    }
            //                }
            //            }

            //            // メッセージウィンドウは表示されている？
            //            is_message_form_opened = My.MyProject.Forms.frmMessage.Visible;

            //            // オブジェクト色等を記録しておく
            //            prev_obj_color = Event.ObjColor;
            //            prev_obj_fill_color = Event.ObjFillColor;
            //            prev_obj_fill_style = Event.ObjFillStyle;
            //            prev_obj_draw_width = Event.ObjDrawWidth;
            //            prev_obj_draw_option = Event.ObjDrawOption;

            //            // オブジェクト色等をデフォルトに戻す
            //            Event.ObjColor = ColorTranslator.ToOle(Color.White);
            //            Event.ObjFillColor = ColorTranslator.ToOle(Color.White);
            //            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            //            Event.ObjFillStyle = vbFSTransparent;
            //            Event.ObjDrawWidth = 1;
            //            Event.ObjDrawOption = "";

            //            // 検索するシチュエーションが武器名かどうか調べる
            //            var loopTo1 = CountWeapon();
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                if ((main_situation ?? "") == (Weapon(i).Name ?? ""))
            //                {
            //                    is_weapon = true;
            //                    break;
            //                }
            //            }

            //            // 検索するシチュエーションがアビリティかどうか調べる
            //            var loopTo2 = CountAbility();
            //            for (i = 1; i <= loopTo2; i++)
            //            {
            //                if ((main_situation ?? "") == (Ability(i).Name ?? ""))
            //                {
            //                    is_ability = true;
            //                    break;
            //                }
            //            }

            //            // イベント用ターゲットを記録しておく
            //            prev_selected_target = Event.SelectedTargetForEvent;

            //            // 攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
            //            // 自分自身をターゲットに設定する
            //            // (発動アニメではアニメ表示にSelectedTargetForEventが使われるため)
            //            if (!is_weapon && !is_ability)
            //            {
            //                if (Event.SelectedTargetForEvent is null)
            //                {
            //                    Event.SelectedTargetForEvent = this;
            //                }
            //            }

            //            // 特殊効果指定を分割
            //            animes = new string[2];
            //            anime_head = 1;
            //            var loopTo3 = Strings.Len(anime);
            //            for (i = 1; i <= loopTo3; i++)
            //            {
            //                if (Strings.Mid(anime, i, 1) == ";")
            //                {
            //                    animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head, i - anime_head);
            //                    Array.Resize(animes, Information.UBound(animes) + 1 + 1);
            //                    anime_head = (i + 1);
            //                }
            //            }

            //            animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head);
            //            ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 743599


            //            Input:

            //                    On Error GoTo ErrorHandler

            //             */
            //            var loopTo4 = Information.UBound(animes);
            //            for (i = 1; i <= loopTo4; i++)
            //            {
            //                anime = animes[i];

            //                // 式評価
            //                Expression.FormatMessage(anime);

            //                // 画面クリア？
            //                if (Strings.LCase(anime) == "clear")
            //                {
            //                    GUI.ClearPicture();
            //                    need_refresh = true;
            //                    goto NextAnime;
            //                }

            //                // 特殊効果
            //                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(anime, 1), 4)) ?? "")
            //                {
            //                    case ".wav":
            //                    case ".mp3":
            //                        {
            //                            // 効果音
            //                            Sound.PlayWave(anime);
            //                            if (wait_time > 0)
            //                            {
            //                                if (need_refresh)
            //                                {
            //                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.picMain(0).Refresh();
            //                                    need_refresh = false;
            //                                }

            //                                GUI.Sleep(wait_time);
            //                                wait_time = 0;
            //                            }

            //                            goto NextAnime;
            //                            break;
            //                        }

            //                    case ".bmp":
            //                    case ".jpg":
            //                    case ".gif":
            //                    case ".png":
            //                        {
            //                            // カットインの表示
            //                            if (wait_time > 0)
            //                            {
            //                                anime = SrcFormatter.Format(wait_time / 100d) + ";" + anime;
            //                                wait_time = 0;
            //                                need_refresh = false;
            //                            }
            //                            else if (Strings.Left(anime, 1) == "@")
            //                            {
            //                                need_refresh = false;
            //                            }
            //                            else
            //                            {
            //                                need_refresh = true;
            //                            }

            //                            GUI.DisplayBattleMessage("-", anime, msg_mode: "");
            //                            goto NextAnime;
            //                            break;
            //                        }
            //                }

            //                switch (Strings.LCase(GeneralLib.LIndex(anime, 1)) ?? "")
            //                {
            //                    case "line":
            //                    case "circle":
            //                    case "arc":
            //                    case "oval":
            //                    case "color":
            //                    case "fillcolor":
            //                    case "fillstyle":
            //                    case "drawwidth":
            //                        {
            //                            // 画面処理コマンド
            //                            if (wait_time > 0)
            //                            {
            //                                anime = SrcFormatter.Format(wait_time / 100d) + ";" + anime;
            //                                wait_time = 0;
            //                                need_refresh = false;
            //                            }
            //                            else
            //                            {
            //                                need_refresh = true;
            //                            }

            //                            GUI.DisplayBattleMessage("-", anime, msg_mode: "");
            //                            goto NextAnime;
            //                            break;
            //                        }

            //                    case "center":
            //                        {
            //                            // 指定したユニットを中央表示
            //                            buf = Expression.GetValueAsString(GeneralLib.ListIndex(anime, 2));
            //                            if (SRC.UList.IsDefined(buf))
            //                            {
            //                                {
            //                                    var withBlock = SRC.UList.Item(buf);
            //                                    GUI.Center(withBlock.x, withBlock.y);
            //                                    GUI.RedrawScreen();
            //                                    need_refresh = false;
            //                                }
            //                            }

            //                            goto NextAnime;
            //                            break;
            //                        }

            //                    case "keep":
            //                        {
            //                            // そのまま終了
            //                            break;
            //                        }
            //                }

            //                // ウェイト？
            //                if (Information.IsNumeric(anime))
            //                {
            //                    wait_time = (100d * Conversions.ToDouble(anime));
            //                    goto NextAnime;
            //                }

            //                // メッセージ表示として処理
            //                if (wait_time > 0)
            //                {
            //                    anime = SrcFormatter.Format(wait_time / 100d) + ";" + anime;
            //                    wait_time = 0;
            //                }

            //                if (!My.MyProject.Forms.frmMessage.Visible)
            //                {
            //                    if (ReferenceEquals(Commands.SelectedTarget, this))
            //                    {
            //                        GUI.OpenMessageForm(this, u2: null);
            //                    }
            //                    else
            //                    {
            //                        GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                    }
            //                }

            //                GUI.DisplayBattleMessage("-", anime, msg_mode: "");
            //                goto NextAnime;
            //            NextAnime:
            //                ;
            //            }

            //            if (SRC.BattleAnimation && !GUI.IsPictureDrawn && !Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            //            {
            //                if (w > 0)
            //                {
            //                    Effect.ShowAnimation("デフォルト攻撃");
            //                }
            //                else if (Strings.InStr(main_situation, "(命中)") > 0)
            //                {
            //                    Effect.ShowAnimation("ダメージ命中 -.wav");
            //                }
            //            }

            //            // 特殊効果再生後にウェイトを入れる？
            //            if (need_refresh)
            //            {
            //                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.picMain(0).Refresh();
            //                need_refresh = false;
            //            }

            //            if (wait_time > 0)
            //            {
            //                GUI.Sleep(wait_time);
            //                wait_time = 0;
            //            }

            //            // 画像を消去しておく
            //            if (GUI.IsPictureDrawn && Strings.InStr(main_situation, "(準備)") == 0 && Strings.LCase(anime) != "keep")
            //            {
            //                GUI.ClearPicture();
            //                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.picMain(0).Refresh();
            //            }

            //            // 最初から表示されていたのでなければメッセージウィンドウを閉じる
            //            if (!is_message_form_opened && !keep_message_form)
            //            {
            //                GUI.CloseMessageForm();
            //            }

            //            // オブジェクト色等を元に戻す
            //            Event.ObjColor = prev_obj_color;
            //            Event.ObjFillColor = prev_obj_fill_color;
            //            Event.ObjFillStyle = prev_obj_fill_style;
            //            Event.ObjDrawWidth = prev_obj_draw_width;
            //            Event.ObjDrawOption = prev_obj_draw_option;

            //            // イベント用ターゲットを元に戻す
            //            Event.SelectedTargetForEvent = prev_selected_target;
            //            return;
            //        ErrorHandler:
            //            ;
            //            if (Strings.Len(Event.EventErrorMessage) > 0)
            //            {
            //                Event.DisplayEventErrorMessage(Event.CurrentLineNum, Event.EventErrorMessage);
            //                Event.EventErrorMessage = "";
            //            }
            //            else
            //            {
            //                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "");
            //            }
        }

        // 特殊効果データが定義されているか？
        public bool IsSpecialEffectDefined(string main_situation, string sub_situation = "")
        {
            bool IsSpecialEffectDefinedRet = default;
            string msg;
            msg = SpecialEffectData(main_situation, sub_situation);
            if (Strings.Len(msg) > 0)
            {
                IsSpecialEffectDefinedRet = true;
            }

            return IsSpecialEffectDefinedRet;
        }
    }
}
