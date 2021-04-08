using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === 戦闘アニメ関連処理 ===
    public partial class Unit
    {
        // 戦闘アニメデータを検索
        public string AnimationData(string main_situation, string sub_situation, bool ext_anime_only = false)
        {
            string AnimationDataRet = default;
            // MOD END MARGE
            string uname, uclass;
            string[] situations;
            int i, ret;
            if (!SRC.BattleAnimation)
            {
                return AnimationDataRet;
            }

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

            var loopTo = Information.UBound(situations);
            for (i = 1; i <= loopTo; i++)
            {
                // 戦闘アニメ能力で指定された名称で検索
                if (IsFeatureAvailable("戦闘アニメ"))
                {
                    uname = FeatureData("戦闘アニメ");
                    // MOD START MARGE
                    // If ADList.IsDefined(uname) Then
                    // AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
                    // If Len(AnimationData) > 0 Then
                    // Exit Function
                    // End If
                    // End If
                    if (SRC.ExtendedAnimation)
                    {
                        if (SRC.EADList.IsDefined(uname))
                        {
                            MessageData localItem() { object argIndex1 = uname; var ret = SRC.EADList.Item(argIndex1); return ret; }

                            AnimationDataRet = localItem().SelectMessage(situations[i], this);
                            if (Strings.Len(AnimationDataRet) > 0)
                            {
                                return AnimationDataRet;
                            }
                        }
                    }

                    if (!ext_anime_only)
                    {
                        if (SRC.ADList.IsDefined(uname))
                        {
                            MessageData localItem1() { object argIndex1 = uname; var ret = SRC.ADList.Item(argIndex1); return ret; }

                            AnimationDataRet = localItem1().SelectMessage(situations[i], this);
                            if (Strings.Len(AnimationDataRet) > 0)
                            {
                                return AnimationDataRet;
                            }
                        }
                    }
                    // MOD END MARGE
                }

                // ユニット名称で検索
                // MOD START MARGE
                // If ADList.IsDefined(Name) Then
                // AnimationData = ADList.Item(Name).SelectMessage(situations(i), Me)
                // If Len(AnimationData) > 0 Then
                // Exit Function
                // End If
                // End If
                if (SRC.ExtendedAnimation)
                {
                    bool localIsDefined() { object argIndex1 = Name; var ret = SRC.EADList.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                    if (localIsDefined())
                    {
                        MessageData localItem2() { object argIndex1 = Name; var ret = SRC.EADList.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                        AnimationDataRet = localItem2().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                if (!ext_anime_only)
                {
                    bool localIsDefined1() { object argIndex1 = Name; var ret = SRC.ADList.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                    if (localIsDefined1())
                    {
                        MessageData localItem3() { object argIndex1 = Name; var ret = SRC.ADList.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

                        AnimationDataRet = localItem3().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }
                // MOD END MARGE

                // ユニット愛称を修正したもので検索
                uname = Nickname0;
                ret = Strings.InStr(uname, "(");
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
                // MOD START MARGE
                // If ADList.IsDefined(uname) Then
                // AnimationData = ADList.Item(uname).SelectMessage(situations(i), Me)
                // If Len(AnimationData) > 0 Then
                // Exit Function
                // End If
                // End If
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined(uname))
                    {
                        MessageData localItem4() { object argIndex1 = uname; var ret = SRC.EADList.Item(argIndex1); return ret; }

                        AnimationDataRet = localItem4().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                if (!ext_anime_only)
                {
                    if (SRC.ADList.IsDefined(uname))
                    {
                        MessageData localItem5() { object argIndex1 = uname; var ret = SRC.ADList.Item(argIndex1); return ret; }

                        AnimationDataRet = localItem5().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }
                // MOD END MARGE

                // ユニットクラスで検索
                uclass = Class0;
                // MOD START MARGE
                // If ADList.IsDefined(uclass) Then
                // AnimationData = ADList.Item(uclass).SelectMessage(situations(i), Me)
                // If Len(AnimationData) > 0 Then
                // Exit Function
                // End If
                // End If
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined(uclass))
                    {
                        MessageData localItem6() { object argIndex1 = uclass; var ret = SRC.EADList.Item(argIndex1); return ret; }

                        AnimationDataRet = localItem6().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                if (!ext_anime_only)
                {
                    if (SRC.ADList.IsDefined(uclass))
                    {
                        MessageData localItem7() { object argIndex1 = uclass; var ret = SRC.ADList.Item(argIndex1); return ret; }

                        AnimationDataRet = localItem7().SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }
                // MOD END MARGE

                // 汎用
                // MOD START MARGE
                // If ADList.IsDefined("汎用") Then
                // AnimationData = ADList.Item("汎用").SelectMessage(situations(i), Me)
                // If Len(AnimationData) > 0 Then
                // Exit Function
                // End If
                // End If
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined("汎用"))
                    {
                        AnimationDataRet = SRC.EADList.Item("汎用").SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                if (!ext_anime_only)
                {
                    if (SRC.ADList.IsDefined("汎用"))
                    {
                        AnimationDataRet = SRC.ADList.Item("汎用").SelectMessage(situations[i], this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }
                // MOD END MARGE
            }

            return AnimationDataRet;
        }

        // 戦闘アニメを再生
        public void PlayAnimation(string main_situation, [Optional, DefaultParameterValue("")] string sub_situation, bool keep_message_form = false)
        {
            string anime, sname = default;
            string[] animes;
            int j, i, idx;
            var ret = default(double);
            var buf = default(string);
            int anime_head;
            bool is_message_form_opened;
            var is_weapon = default(bool);
            var is_ability = default(bool);
            var in_bulk = default(bool);
            var wait_time = default;
            var need_refresh = default(bool);
            int prev_obj_color;
            int prev_obj_fill_color;
            int prev_obj_fill_style;
            int prev_obj_draw_width;
            string prev_obj_draw_option;
            Unit prev_selected_target;

            // 戦闘アニメデータを検索
            anime = AnimationData(main_situation, sub_situation);

            // 見つからなかった場合は一括指定を試してみる
            if (string.IsNullOrEmpty(anime))
            {
                switch (Strings.Right(main_situation, 4) ?? "")
                {
                    case "(準備)":
                    case "(攻撃)":
                    case "(命中)":
                        {
                            anime = AnimationData(Strings.Left(main_situation, Strings.Len(main_situation) - 4), sub_situation);
                            in_bulk = true;
                            break;
                        }

                    case "(発動)":
                        {
                            anime = AnimationData(Strings.Left(main_situation, Strings.Len(main_situation) - 4), sub_situation);
                            break;
                        }
                }
            }

            GeneralLib.TrimString(anime);

            // 表示キャンセル
            if (string.IsNullOrEmpty(anime) | anime == "-")
            {
                return;
            }

            // マウスの右ボタンでキャンセル
            if (GUI.IsRButtonPressed())
            {
                // MOD START MARGE
                // '式評価のみ行う
                // FormatMessage anime
                // Exit Sub
                // アニメの終了処理はキャンセルしない
                if (main_situation != "終了" & Strings.Right(main_situation, 4) != "(終了)")
                {
                    // 式評価のみ行う
                    Expression.FormatMessage(anime);
                    return;
                }
                // MOD END MARGE
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
            var loopTo = CountWeapon();
            for (i = 1; i <= loopTo; i++)
            {
                if ((main_situation ?? "") == (Weapon(i).Name + "(攻撃)" ?? ""))
                {
                    is_weapon = true;
                    break;
                }
            }

            // 検索するシチュエーションがアビリティかどうか調べる
            var loopTo1 = CountAbility();
            for (i = 1; i <= loopTo1; i++)
            {
                if ((main_situation ?? "") == (Ability(i).Name + "(発動)" ?? ""))
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

            // アニメ指定を分割
            animes = new string[2];
            anime_head = 1;
            var loopTo2 = Strings.Len(anime);
            for (i = 1; i <= loopTo2; i++)
            {
                if (Strings.Mid(anime, i, 1) == ";")
                {
                    animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head, i - anime_head);
                    Array.Resize(animes, Information.UBound(animes) + 1 + 1);
                    anime_head = (i + 1);
                }
            }

            animes[Information.UBound(animes)] = Strings.Mid(anime, anime_head);
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 761038


            Input:

                    On Error GoTo ErrorHandler

             */
            var loopTo3 = Information.UBound(animes);
            for (i = 1; i <= loopTo3; i++)
            {
                anime = animes[i];

                // 最後に実行されたのがサブルーチン呼び出しかどうかを判定するため
                // サブルーチン名をあらかじめクリアしておく
                sname = "";

                // 式評価
                Expression.FormatMessage(anime);

                // 画面クリア？
                if (Strings.LCase(anime) == "clear")
                {
                    GUI.ClearPicture();
                    need_refresh = true;
                    goto NextAnime;
                }

                // 戦闘アニメ以外の特殊効果
                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(anime, 1), 4)) ?? "")
                {
                    case ".wav":
                    case ".mp3":
                        {
                            // 効果音
                            Sound.PlayWave(anime);
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

                            GUI.DisplayBattleMessage("-", anime, msg_mode: "");
                            goto NextAnime;
                            break;
                        }
                }

                switch (Strings.LCase(GeneralLib.LIndex(anime, 1)) ?? "")
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

                            GUI.DisplayBattleMessage("-", anime, msg_mode: "");
                            goto NextAnime;
                            break;
                        }

                    case "center":
                        {
                            // 指定したユニットを中央表示
                            buf = Expression.GetValueAsString(GeneralLib.ListIndex(anime, 2));
                            if (SRC.UList.IsDefined(buf))
                            {
                                {
                                    var withBlock = SRC.UList.Item(buf);
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
                    wait_time = (100d * Conversions.ToDouble(anime));
                    goto NextAnime;
                }

                // サブルーチンの呼び出しが確定

                // 戦闘アニメ再生用のサブルーチン名を作成
                sname = GeneralLib.LIndex(anime, 1);
                if (Strings.Left(sname, 1) == "@")
                {
                    sname = Strings.Mid(sname, 2);
                }
                else if (is_weapon)
                {
                    // 武器名の場合
                    sname = "戦闘アニメ_" + sname + "攻撃";
                }
                else
                {
                    // その他の場合
                    // 括弧を含んだ武器名に対応するため、"("は後ろから検索
                    idx = GeneralLib.InStr2(main_situation, "(");

                    // 変形系のシチュエーションではサフィックスを無視
                    if (idx > 0)
                    {
                        switch (Strings.Left(main_situation, idx - 1) ?? "")
                        {
                            case "変形":
                            case "ハイパーモード":
                            case "ノーマルモード":
                            case "パーツ分離":
                            case "合体":
                            case "分離":
                                {
                                    idx = 0;
                                    break;
                                }
                        }
                    }

                    // 武器名(攻撃無効化)の場合もサフィックスを無視
                    if (idx > 0)
                    {
                        if (Strings.Mid(main_situation, idx) == "(攻撃無効化)")
                        {
                            idx = 0;
                        }
                    }

                    if (idx > 0)
                    {
                        // サフィックスあり
                        sname = "戦闘アニメ_" + sname + Strings.Mid(main_situation, idx + 1, Strings.Len(main_situation) - idx - 1);
                    }
                    else
                    {
                        sname = "戦闘アニメ_" + sname + "発動";
                    }
                }

                // サブルーチンが見つからなかった
                if (Event_Renamed.FindNormalLabel(sname) == 0)
                {
                    if (in_bulk)
                    {
                        // 一括指定を利用している場合
                        switch (Strings.Right(main_situation, 4) ?? "")
                        {
                            case "(準備)":
                                {
                                    // 表示をキャンセル
                                    goto NextAnime;
                                    break;
                                }

                            case "(攻撃)":
                                {
                                    // 複数のアニメ指定がある場合は諦めて他のものを使う
                                    if (Information.UBound(animes) > 1)
                                    {
                                        goto NextAnime;
                                    }
                                    // そうでなければ「デフォルト」を使用
                                    sname = "戦闘アニメ_デフォルト攻撃";
                                    break;
                                }

                            case "(命中)":
                                {
                                    // 複数のアニメ指定がある場合は諦めて他のものを使う
                                    if (Information.UBound(animes) > 1)
                                    {
                                        goto NextAnime;
                                    }
                                    // そうでなければ「ダメージ」を使用
                                    sname = "戦闘アニメ_ダメージ命中";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        if (wait_time > 0)
                        {
                            anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                            wait_time = 0;
                        }

                        if (!My.MyProject.Forms.frmMessage.Visible)
                        {
                            if (ReferenceEquals(Commands.SelectedTarget, this))
                            {
                                GUI.OpenMessageForm(this, u2: null);
                            }
                            else
                            {
                                GUI.OpenMessageForm(Commands.SelectedTarget, this);
                            }
                        }

                        GUI.DisplayBattleMessage("-", anime, msg_mode: "");
                        goto NextAnime;
                    }
                }

                sname = "`" + sname + "`";

                // 引数の構築
                var loopTo4 = GeneralLib.ListLength(anime);
                for (j = 2; j <= loopTo4; j++)
                    sname = sname + "," + GeneralLib.ListIndex(anime, j);
                if (in_bulk)
                {
                    sname = sname + ",`一括指定`";
                }

                // 戦闘アニメ再生前にウェイトを入れる
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

                // 画像描画が行われたかどうかの判定のためにフラグを初期化
                GUI.IsPictureDrawn = false;

                // 戦闘アニメ再生
                Event_Renamed.SaveBasePoint();
                Expression.CallFunction("Call(" + sname + ")", Expression.ValueType.StringType, buf, ret);
                Event_Renamed.RestoreBasePoint();

                // 画像を消去しておく
                if (GUI.IsPictureDrawn & Strings.LCase(buf) != "keep")
                {
                    GUI.ClearPicture();
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.picMain(0).Refresh();
                }

            NextAnime:
                ;
            }

            // 戦闘アニメ再生後にウェイトを入れる？
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
            if (GUI.IsPictureDrawn & string.IsNullOrEmpty(sname) & Strings.InStr(main_situation, "(準備)") == 0 & Strings.LCase(anime) != "keep")
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

        // 戦闘アニメが定義されているか？
        public bool IsAnimationDefined(string main_situation, [Optional, DefaultParameterValue("")] string sub_situation, bool ext_anime_only = false)
        {
            bool IsAnimationDefinedRet = default;
            // MOD END MARGE
            string anime;

            // MOD START MARGE
            // anime = AnimationData(main_situation, sub_situation)
            anime = AnimationData(main_situation, sub_situation, ext_anime_only);
            // MOD END MARGE

            if (Strings.Len(anime) > 0)
            {
                IsAnimationDefinedRet = true;
            }
            else
            {
                IsAnimationDefinedRet = false;
            }

            return IsAnimationDefinedRet;
        }
    }
}
