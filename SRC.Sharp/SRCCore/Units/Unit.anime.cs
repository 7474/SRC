using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // === 戦闘アニメ関連処理 ===
    public partial class Unit
    {
        public void PlayAnimationIfDefined(string[] situations)
        {
            foreach (var situation in situations)
            {
                if (IsAnimationDefined(situation))
                {
                    PlayAnimation(situation);
                    return;
                }
            }
            foreach (var situation in situations)
            {
                if (IsSpecialEffectDefined(situation))
                {
                    SpecialEffect(situation);
                    return;
                }
            }
        }

        public void PlayAnimation(string[] situations)
        {
            foreach (var situation in situations)
            {
                if (IsAnimationDefined(situation))
                {
                    PlayAnimation(situation);
                    return;
                }
            }
            foreach (var situation in situations.Take(situations.Length - 1))
            {
                if (IsSpecialEffectDefined(situation))
                {
                    SpecialEffect(situation);
                    return;
                }
            }
            SpecialEffect(situations.Last());
        }

        // 戦闘アニメデータを検索
        public string AnimationData(string main_situation, string sub_situation, bool ext_anime_only = false)
        {
            string AnimationDataRet = null;
            if (!SRC.BattleAnimation)
            {
                return AnimationDataRet;
            }

            // シチュエーションのリストを構築
            var situations = new List<string>();
            if (!string.IsNullOrEmpty(sub_situation) && main_situation == sub_situation)
            {
                situations.Add(main_situation + "(" + sub_situation + ")");
            }
            situations.Add(main_situation);

            foreach (var situation in situations)
            {
                // 戦闘アニメ能力で指定された名称で検索
                if (IsFeatureAvailable("戦闘アニメ"))
                {
                    var uname = FeatureData("戦闘アニメ");
                    if (SRC.ExtendedAnimation)
                    {
                        if (SRC.EADList.IsDefined(uname))
                        {
                            AnimationDataRet = SRC.EADList.Item(uname).SelectMessage(situation, this);
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
                            AnimationDataRet = SRC.ADList.Item(uname).SelectMessage(situation, this);
                            if (Strings.Len(AnimationDataRet) > 0)
                            {
                                return AnimationDataRet;
                            }
                        }
                    }
                }

                // ユニット名称で検索
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined(Name))
                    {
                        AnimationDataRet = SRC.EADList.Item(Name).SelectMessage(situation, this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                if (!ext_anime_only)
                {
                    if (SRC.ADList.IsDefined(Name))
                    {
                        AnimationDataRet = SRC.ADList.Item(Name).SelectMessage(situation, this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                // ユニット愛称を修正したもので検索
                {
                    var uname = Nickname0;
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
                    if (SRC.ExtendedAnimation)
                    {
                        if (SRC.EADList.IsDefined(uname))
                        {
                            AnimationDataRet = SRC.EADList.Item(uname).SelectMessage(situation, this);
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
                            AnimationDataRet = SRC.ADList.Item(uname).SelectMessage(situation, this);
                            if (Strings.Len(AnimationDataRet) > 0)
                            {
                                return AnimationDataRet;
                            }
                        }
                    }
                }

                // ユニットクラスで検索
                var uclass = Class0;
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined(uclass))
                    {
                        AnimationDataRet = SRC.EADList.Item(uclass).SelectMessage(situation, this);
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
                        AnimationDataRet = SRC.ADList.Item(uclass).SelectMessage(situation, this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }

                // 汎用
                if (SRC.ExtendedAnimation)
                {
                    if (SRC.EADList.IsDefined("汎用"))
                    {
                        AnimationDataRet = SRC.EADList.Item("汎用").SelectMessage(situation, this);
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
                        AnimationDataRet = SRC.ADList.Item("汎用").SelectMessage(situation, this);
                        if (Strings.Len(AnimationDataRet) > 0)
                        {
                            return AnimationDataRet;
                        }
                    }
                }
            }

            return AnimationDataRet;
        }

        // 戦闘アニメを再生
        public void PlayAnimation(string main_situation, string sub_situation = "", bool keep_message_form = false)
        {
            var in_bulk = false;

            // 戦闘アニメデータを検索
            var anime = AnimationData(main_situation, sub_situation);

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

            anime = anime.Trim();

            // 表示キャンセル
            if (string.IsNullOrEmpty(anime) || anime == "-")
            {
                return;
            }

            // マウスの右ボタンでキャンセル
            if (GUI.IsRButtonPressed())
            {
                // アニメの終了処理はキャンセルしない
                if (main_situation != "終了" & Strings.Right(main_situation, 4) != "(終了)")
                {
                    // 式評価のみ行う
                    Expression.FormatMessage(ref anime);
                    return;
                }
            }

            // メッセージウィンドウは表示されている？
            var is_message_form_opened = GUI.MessageFormVisible;

            // TODO Impl オブジェクト色等
            //// オブジェクト色等を記録しておく
            //prev_obj_color = Event.ObjColor;
            //prev_obj_fill_color = Event.ObjFillColor;
            //prev_obj_fill_style = Event.ObjFillStyle;
            //prev_obj_draw_width = Event.ObjDrawWidth;
            //prev_obj_draw_option = Event.ObjDrawOption;

            //// オブジェクト色等をデフォルトに戻す
            //Event.ObjColor = ColorTranslator.ToOle(Color.White);
            //Event.ObjFillColor = ColorTranslator.ToOle(Color.White);
            //Event.ObjFillStyle = vbFSTransparent;
            //Event.ObjDrawWidth = 1;
            //Event.ObjDrawOption = "";

            // 検索するシチュエーションが武器名かどうか調べる
            var is_weapon = Weapons.Any(w => main_situation == w.Name + "(攻撃)");

            // 検索するシチュエーションがアビリティかどうか調べる
            var is_ability = Abilities.Any(a => main_situation == a.Data.Name + "(発動)");

            // イベント用ターゲットを記録しておく
            var prev_selected_target = Event.SelectedTargetForEvent;

            // 攻撃でもアビリティでもない場合、ターゲットが設定されていなければ
            // 自分自身をターゲットに設定する
            // (発動アニメではアニメ表示にSelectedTargetForEventが使われるため)
            if (!is_weapon & !is_ability)
            {
                if (Event.SelectedTargetForEvent is null)
                {
                    Event.SelectedTargetForEvent = this;
                }
            }

            // アニメ指定を分割
            var animes = anime.Split(";").ToList();
            try
            {
                var need_refresh = false;
                var wait_time = 0;
                var sname = "";
                var buf = "";
                foreach (var a in animes)
                {
                    var animepart = a;
                    // 最後に実行されたのがサブルーチン呼び出しかどうかを判定するため
                    // サブルーチン名をあらかじめクリアしておく
                    sname = "";

                    // 式評価
                    Expression.FormatMessage(ref animepart);

                    // 画面クリア？
                    if (Strings.LCase(animepart) == "clear")
                    {
                        GUI.ClearPicture();
                        need_refresh = true;
                        goto NextAnime;
                    }

                    // 戦闘アニメ以外の特殊効果
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
                                buf = Expression.GetValueAsString(GeneralLib.ListIndex(animepart, 2));
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

                    // サブルーチンの呼び出しが確定

                    // 戦闘アニメ再生用のサブルーチン名を作成
                    sname = GeneralLib.LIndex(animepart, 1);
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
                        var idx = GeneralLib.InStr2(main_situation, "(");

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
                    if (Event.FindNormalLabel(sname) == 0)
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
                                    }

                                case "(攻撃)":
                                    {
                                        // 複数のアニメ指定がある場合は諦めて他のものを使う
                                        if (animes.Count > 1)
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
                                        if (animes.Count > 1)
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
                            goto NextAnime;
                        }
                    }

                    sname = "`" + sname + "`";

                    // 引数の構築
                    sname = sname + "," + string.Join(",", GeneralLib.ToList(animepart).Skip(1));
                    if (in_bulk)
                    {
                        sname = sname + ",`一括指定`";
                    }

                    // 戦闘アニメ再生前にウェイトを入れる
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

                    // 画像描画が行われたかどうかの判定のためにフラグを初期化
                    GUI.IsPictureDrawn = false;

                    // 戦闘アニメ再生
                    Event.SaveBasePoint();
                    Expression.CallFunction("Call(" + sname + ")", Expressions.ValueType.StringType, out buf, out _);
                    Event.RestoreBasePoint();

                    // 画像を消去しておく
                    if (GUI.IsPictureDrawn & Strings.LCase(buf) != "keep")
                    {
                        GUI.ClearPicture();
                        GUI.UpdateScreen();
                    }

                NextAnime:
                    ;
                }

                // 戦闘アニメ再生後にウェイトを入れる？
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
                if (GUI.IsPictureDrawn & string.IsNullOrEmpty(sname) & Strings.InStr(main_situation, "(準備)") == 0 & Strings.LCase(anime) != "keep")
                {
                    GUI.ClearPicture();
                    GUI.UpdateScreen();
                }

                // 最初から表示されていたのでなければメッセージウィンドウを閉じる
                if (!is_message_form_opened & !keep_message_form)
                {
                    GUI.CloseMessageForm();
                }

                //// オブジェクト色等を元に戻す
                //Event.ObjColor = prev_obj_color;
                //Event.ObjFillColor = prev_obj_fill_color;
                //Event.ObjFillStyle = prev_obj_fill_style;
                //Event.ObjDrawWidth = prev_obj_draw_width;
                //Event.ObjDrawOption = prev_obj_draw_option;

                // イベント用ターゲットを元に戻す
                Event.SelectedTargetForEvent = prev_selected_target;
            }
            catch (Exception ex)
            {
                // TODO Handle error
                //if (Strings.Len(Event.EventErrorMessage) > 0)
                //{
                //    Event.DisplayEventErrorMessage(Event.CurrentLineNum, Event.EventErrorMessage);
                //    Event.EventErrorMessage = "";
                //}
                //else
                //{
                //    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "");
                //}
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, ex.Message);
            }
        }

        // 戦闘アニメが定義されているか？
        public bool IsAnimationDefined(string main_situation, string sub_situation = "", bool ext_anime_only = false)
        {
            bool IsAnimationDefinedRet = default;
            string anime = AnimationData(main_situation, sub_situation, ext_anime_only);

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
