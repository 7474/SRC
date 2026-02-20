using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // 特殊効果データを検索
            var anime = SpecialEffectData(main_situation, sub_situation);
            anime = anime.Trim();

            // 表示キャンセル
            if (string.IsNullOrEmpty(anime) || anime == "-")
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

            // メッセージウィンドウは表示されている？
            var is_message_form_opened = GUI.MessageFormVisible;

            // オブジェクト色等を記録しておく
            var prevObjectDrawSetting = Event.GetObjectDrawSetting();
            // オブジェクト色等をデフォルトに戻す
            Event.ResetObjectDrawSetting();

            // 検索するシチュエーションが武器名かどうか調べる
            var is_weapon = Weapons.Any(w => main_situation == w.Name);

            // 検索するシチュエーションがアビリティかどうか調べる
            var is_ability = Abilities.Any(a => main_situation == a.Data.Name);

            // イベント用ターゲットを記録しておく
            var prev_selected_target = Event.SelectedTargetForEvent;

            // 攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
            // 自分自身をターゲットに設定する
            if (!is_weapon && !is_ability)
            {
                if (Event.SelectedTargetForEvent is null)
                {
                    Event.SelectedTargetForEvent = this;
                }
            }

            // 特殊効果指定を分割
            var animes = anime.Split(";").ToList();
            try
            {
                var need_refresh = false;
                var wait_time = 0;
                foreach (var a in animes)
                {
                    var animepart = a;

                    // 式評価
                    Expression.FormatMessage(ref animepart);

                    // 画面クリア？
                    if (Strings.LCase(animepart) == "clear")
                    {
                        GUI.ClearPicture();
                        need_refresh = true;
                        goto NextAnime;
                    }

                    // 特殊効果
                    switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(animepart, 1), 4)) ?? "")
                    {
                        case ".wav":
                        case ".mp3":
                            {
                                // 効果音
                                Sound.PlayWave(animepart);
                                if (wait_time > 0)
                                {
                                    if (need_refresh)
                                    {
                                        GUI.UpdateScreen();
                                        need_refresh = false;
                                    }

                                    GUI.Sleep(wait_time);
                                    wait_time = 0;
                                }

                                goto NextAnime;
                            }

                        case ".bmp":
                        case ".jpg":
                        case ".gif":
                        case ".png":
                            {
                                // カットインの表示
                                if (wait_time > 0)
                                {
                                    animepart = (wait_time / 100d) + ";" + animepart;
                                    wait_time = 0;
                                    need_refresh = false;
                                }
                                else if (Strings.Left(animepart, 1) == "@")
                                {
                                    need_refresh = false;
                                }
                                else
                                {
                                    need_refresh = true;
                                }

                                GUI.DisplayBattleMessage("-", animepart, msg_mode: "");
                                goto NextAnime;
                            }
                    }

                    switch (Strings.LCase(GeneralLib.LIndex(animepart, 1)) ?? "")
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
                                    animepart = (wait_time / 100d) + ";" + animepart;
                                    wait_time = 0;
                                    need_refresh = false;
                                }
                                else
                                {
                                    need_refresh = true;
                                }

                                GUI.DisplayBattleMessage("-", animepart, msg_mode: "");
                                goto NextAnime;
                            }

                        case "center":
                            {
                                // 指定したユニットを中央表示
                                var buf = Expression.GetValueAsString(GeneralLib.ListIndex(animepart, 2));
                                if (SRC.UList.IsDefined(buf))
                                {
                                    var withBlock = SRC.UList.Item(buf);
                                    GUI.Center(withBlock.x, withBlock.y);
                                    GUI.RedrawScreen();
                                    need_refresh = false;
                                }

                                goto NextAnime;
                            }

                        case "keep":
                            {
                                // そのまま終了
                                break;
                            }
                    }

                    // ウェイト？
                    if (Information.IsNumeric(animepart))
                    {
                        wait_time = (int)(100d * Conversions.ToDouble(animepart));
                        goto NextAnime;
                    }

                    // メッセージ表示として処理
                    if (wait_time > 0)
                    {
                        animepart = (wait_time / 100d) + ";" + animepart;
                        wait_time = 0;
                    }

                    if (!GUI.MessageFormVisible)
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

                    GUI.DisplayBattleMessage("-", animepart, msg_mode: "");

                NextAnime:
                    ;
                }

                // 特殊効果再生後にウェイトを入れる？
                if (need_refresh)
                {
                    GUI.UpdateScreen();
                    need_refresh = false;
                }

                if (wait_time > 0)
                {
                    GUI.Sleep(wait_time);
                    wait_time = 0;
                }

                // 画像を消去しておく
                if (GUI.IsPictureDrawn && Strings.InStr(main_situation, "(準備)") == 0 && Strings.LCase(anime) != "keep")
                {
                    GUI.ClearPicture();
                    GUI.UpdateScreen();
                }

                // 最初から表示されていたのでなければメッセージウィンドウを閉じる
                if (!is_message_form_opened && !keep_message_form)
                {
                    GUI.CloseMessageForm();
                }

                // オブジェクト色等を元に戻す
                Event.SetObjectDrawSetting(prevObjectDrawSetting);

                // イベント用ターゲットを元に戻す
                Event.SelectedTargetForEvent = prev_selected_target;
            }
            catch (Exception ex)
            {
                SRC.LogError(ex);
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, ex.Message);
            }
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
