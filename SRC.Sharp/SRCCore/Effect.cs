// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore
{
    // 特殊効果の自動選択＆再生処理
    public class Effect
    {
        // 構えている武器の種類
        private string WeaponInHand;

        // 攻撃手段の種類
        private string CurrentWeaponType;

        private SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private Maps.Map Map => SRC.Map;
        private Events.Event Event => SRC.Event;
        private Expressions.Expression Expression => SRC.Expression;
        private Sound Sound => SRC.Sound;

        public Effect(SRC src)
        {
            SRC = src;
        }

        // 戦闘アニメ再生用サブルーチン
        public void ShowAnimation(string aname)
        {
            try
            {
                string buf;
                double ret;
                int i;
                string expr;
                if (!SRC.BattleAnimation)
                {
                    return;
                }

                // 右クリック中は特殊効果をスキップ
                if (GUI.IsRButtonPressed())
                {
                    return;
                }

                // サブルーチン呼び出しのための式を作成
                expr = GeneralLib.LIndex(aname, 1);
                if (Strings.InStr(expr, "戦闘アニメ_") != 1)
                {
                    expr = "戦闘アニメ_" + GeneralLib.LIndex(aname, 1);
                }

                if (Event.FindNormalLabel(expr) == 0)
                {
                    GUI.ErrorMessage("サブルーチン「" + expr + "」が見つかりません");
                    return;
                }

                expr = "Call(`" + expr + "`";
                var loopTo = GeneralLib.LLength(aname);
                for (i = 2; i <= loopTo; i++)
                    expr = expr + ",`" + GeneralLib.LIndex(aname, i) + "`";
                expr = expr + ")";

                // 画像描画が行われたかどうかの判定のためにフラグを初期化
                GUI.IsPictureDrawn = false;

                // メッセージウィンドウの状態を記録
                GUI.SaveMessageFormStatus();

                // 戦闘アニメ再生
                Event.SaveBasePoint();
                Expression.CallFunction(expr, Expressions.ValueType.StringType, out buf, out ret);
                Event.RestoreBasePoint();

                // メッセージウィンドウの状態が変化している場合は復元
                GUI.KeepMessageFormStatus();

                // 画像を消去しておく
                if (GUI.IsPictureDrawn && Strings.LCase(buf) != "keep")
                {
                    GUI.ClearPicture();
                    GUI.UpdateScreen();
                }
            }
            catch (Exception ex)
            {
                throw;
                // TODO 戦闘アニメ実行中に発生したエラーの処理
                //if (Strings.Len(Event.EventErrorMessage) > 0)
                //{
                //    Event.DisplayEventErrorMessage(Event.CurrentLineNum, Event.EventErrorMessage);
                //    Event.EventErrorMessage = "";
                //}
                //else
                //{
                //    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "");
                //}
            }
        }

        // 武器準備時の特殊効果
        public void PrepareWeaponEffect(Unit u, UnitWeapon w)
        {
            // 右クリック中は特殊効果をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            if (SRC.BattleAnimation)
            {
                PrepareWeaponAnimation(u, w);
            }
            else
            {
                PrepareWeaponSound(u, w);
            }
        }

        // 武器準備時のアニメーション
        public void PrepareWeaponAnimation(Unit u, UnitWeapon w)
        {
            string wclass = default, wname, wtype;
            var double_weapon = default(bool);
            string sname = default, aname, cname = default;
            var with_face_up = default(bool);
            int i;

            // 戦闘アニメ非自動選択
            if (Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            {
                return;
            }

            {
                var withBlock = u;
                // まず準備アニメ表示の際にフェイスアップを表示するか決定する
                if (withBlock.CountWeapon() >= 4
                    && w.WeaponNo() >= withBlock.CountWeapon() - 1
                    && w.UpdatedWeaponData.Power >= 1800
                    && (w.UpdatedWeaponData.Bullet > 0 && w.UpdatedWeaponData.Bullet <= 4
                        || w.UpdatedWeaponData.ENConsumption >= 35))
                {
                    // ４つ以上の武器を持つユニットがそのユニットの最高威力
                    // もしくは２番目に強力な武器を使用し、
                    // その武器の攻撃力1800以上でかつ武器使用可能回数が限定されていれば
                    // 必殺技と見なしてフェイスアップ表示
                    // with_face_up = True
                }

                // 空中移動専用形態は武器を手で構えない
                if (withBlock.Data.Transportation == "空")
                {
                    WeaponInHand = "";
                    goto SkipWeaponAnimation;
                }

                // 等身大基準の場合、非人間ユニットはメカであることが多いので内蔵武器を優先する
                if (Expression.IsOptionDefined("等身大基準") && !withBlock.IsHero())
                {
                    WeaponInHand = "";
                    goto SkipWeaponAnimation;
                }

                wname = w.WeaponNickname();
                wclass = w.UpdatedWeaponData.Class;
            }

            // 武器準備のアニメーションを非表示にするオプションを選択している？
            if (!SRC.WeaponAnimation && !SRC.ExtendedAnimation
                || Expression.IsOptionDefined("武器準備アニメ非表示"))
            {
                // MOD END MARGE
                WeaponInHand = "";
                goto SkipWeaponAnimation;
            }

            // 二刀流？
            if (Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "双") > 0
                || Strings.InStr(wname, "二刀") > 0)
            {
                double_weapon = true;
            }

            // 「ブーン」という効果音を鳴らす？
            if (Strings.InStr(wname, "高周波") > 0
                || Strings.InStr(wname, "電磁") > 0)
            {
                sname = "BeamSaber.wav";
            }

            // これから武器の種類を判定
            if (GeneralLib.InStrNotNest(wclass, "武") == 0
                && GeneralLib.InStrNotNest(wclass, "突") == 0
                && GeneralLib.InStrNotNest(wclass, "接") == 0
                && GeneralLib.InStrNotNest(wclass, "実") == 0)
            {
                goto SkipInfightWeapon;
            }

            // 武器名から武器の種類を判定
            wtype = CheckWeaponType(wname, wclass);
            if (wtype == "手裏剣")
            {
                // 手裏剣は構えずにいきなり投げたほうがかっこいいと思うのでy
                return;
            }

            if (!string.IsNullOrEmpty(wtype))
            {
                goto FoundWeaponType;
            }

            // 詳細が分からなかった武器
            if (GeneralLib.InStrNotNest(wclass, "武") > 0)
            {
                // TODO Impl item
                //// 装備しているアイテムから武器を検索
                //var loopTo = u.CountItem();
                //for (i = 1; i <= loopTo; i++)
                //{
                //    {
                //        var withBlock1 = u.Item(i);
                //        if (withBlock1.Activated
                //&& (withBlock1.Part() == "両手"
                //|| withBlock1.Part() == "片手"
                //|| withBlock1.Part() == "武器"))
                //        {
                //            wtype = CheckWeaponType(withBlock1.Nickname(), "");
                //            if (!string.IsNullOrEmpty(wtype))
                //            {
                //                goto FoundWeaponType;
                //            }

                //            wtype = CheckWeaponType(withBlock1.Class0(), "");
                //            if (!string.IsNullOrEmpty(wtype))
                //            {
                //                goto FoundWeaponType;
                //            }

                //            break;
                //        }
                //    }
                //}

                goto SkipShootingWeapon;
            }

            if (GeneralLib.InStrNotNest(wclass, "突") > 0
                || GeneralLib.InStrNotNest(wclass, "接") > 0)
            {
                goto SkipShootingWeapon;
            }

        SkipInfightWeapon:
            ;


            // まずはビーム攻撃かどうか判定
            if (!IsBeamWeapon(wname, wclass, cname))
            {
                goto SkipBeamWeapon;
            }

            // 手持ち？
            if (Strings.InStr(wname, "ライフル") > 0
                || Strings.InStr(wname, "バズーカ") > 0
                || Strings.Right(wname, 2) == "ガン"
                || Strings.Right(wname, 1) == "銃" && Strings.Right(wname, 2) != "機銃")
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
                {
                    wtype = "ＭＡＰバスタービームライフル";
                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "ハイメガ") > 0
                    || Strings.InStr(wname, "バスター") > 0
                    || Strings.InStr(wname, "大") > 0
                    || Strings.Left(wname, 2) == "ギガ")
                {
                    wtype = "バスタービームライフル";
                }
                else if (Strings.InStr(wname, "メガ") > 0
                    || Strings.InStr(wname, "ハイ") > 0
                    || Strings.InStr(wname, "バズーカ") > 0)
                {
                    if (double_weapon)
                    {
                        wtype = "ダブルビームランチャー";
                    }
                    else
                    {
                        wtype = "ビームランチャー";
                    }

                    if (Strings.InStr(wname, "ライフル") > 0)
                    {
                        wtype = "バスタービームライフル";
                    }
                }
                else if (CountAttack0(u, w) >= 4)
                {
                    wtype = "マシンガン";
                }
                else if (Strings.InStr(wname, "ピストル") > 0
                    || Strings.InStr(wname, "ミニ") > 0
                    || Strings.InStr(wname, "小") > 0)
                {
                    wtype = "レーザーガン";
                }
                else if (double_weapon)
                {
                    wtype = "ダブルビームライフル";
                }
                else
                {
                    wtype = "ビームライフル";
                }

                goto FoundWeaponType;
            }

        SkipBeamWeapon:
            ;
            if (Strings.InStr(wname, "弓") > 0
                || Strings.InStr(wname, "ショートボウ") > 0
                || Strings.InStr(wname, "ロングボウ") > 0)
            {
                wtype = "弓";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "クロスボウ") > 0
                || Strings.InStr(wname, "ボウガン") > 0)
            {
                wtype = "クロスボウ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "バズーカ") > 0)
            {
                wtype = "バズーカ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "サブマシンガン") > 0)
            {
                wtype = "サブマシンガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "機関銃") > 0)
            {
                if (Strings.InStr(wname, "ヘビー") > 0
                    || Strings.InStr(wname, "重") > 0)
                {
                    wtype = "ヘビーマシンガン";
                }
                else
                {
                    wtype = "マシンガン";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ガトリング") > 0)
            {
                wtype = "ガトリング";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ピストル") > 0
                || Strings.InStr(wname, "拳銃") > 0)
            {
                wtype = "ピストル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "リボルバー") > 0
                || Strings.InStr(wname, "リボルヴァー") > 0)
            {
                wtype = "リボルバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ショットガン") > 0
                || Strings.InStr(wname, "ライアットガン") > 0)
            {
                wtype = "ショットガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "スーパーガン") > 0)
            {
                wtype = "スーパーガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "スーパーキャノン") > 0)
            {
                wtype = "スーパーキャノン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ライフル") > 0
                || Strings.Right(wname, 1) == "銃" && Strings.Right(wname, 2) != "機銃"
                || Strings.Right(wname, 2) == "ガン")
            {
                wtype = "ライフル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "対戦車ライフル") > 0)
            {
                wtype = "対戦車ライフル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "対物ライフル") > 0)
            {
                wtype = "対物ライフル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "消火器") > 0)
            {
                wtype = "消火器";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "放水") > 0
                || Strings.InStr(wname, "放射器") > 0)
            {
                wtype = "放水銃";
                goto FoundWeaponType;
            }

        SkipShootingWeapon:
            ;


            // 対応する武器は見つからなかった
            WeaponInHand = "";
            goto SkipWeaponAnimation;
        FoundWeaponType:
            ;


            // 構えている武器を記録
            WeaponInHand = wtype;

            // 表示する準備アニメの種類
            aname = wtype + "準備";

            // 色
            if (Strings.InStr(wtype, "ビームサーベル") > 0
                || Strings.InStr(wtype, "ビームカッター") > 0
                || wtype == "ビームナイフ"
                || wtype == "ライトセイバー")
            {
                if (Strings.InStr(wname, "ビーム") > 0)
                {
                    aname = aname + " ピンク";
                }
                else if (Strings.InStr(wname, "プラズマ") > 0)
                {
                    aname = aname + " グリーン";
                }
                else if (Strings.InStr(wname, "レーザー") > 0)
                {
                    aname = aname + " ブルー";
                }
                else if (Strings.InStr(wname, "ライト") > 0)
                {
                    aname = aname + " イエロー";
                }
            }

            // 効果音
            if (Strings.Len(sname) > 0)
            {
                aname = aname + " " + sname;
            }

            // 二刀流
            if (double_weapon)
            {
                aname = aname + " 二刀流";
            }

            // 準備アニメ表示
            ShowAnimation(aname);
        SkipWeaponAnimation:
            ;


            // 武器の準備アニメをスキップする場合はここから

            if (with_face_up)
            {
                // フェイスアップを表示する
                aname = "フェイスアップ準備";

                // 衝撃を表示？
                if (GeneralLib.InStrNotNest(wclass, "サ") > 0)
                {
                    aname = aname + " 衝撃";
                }

                // フェイスアップアニメ表示
                ShowAnimation(aname);
            }
        }

        // 武器の名称から武器の種類を判定
        private string CheckWeaponType(string wname, string wclass)
        {
            string CheckWeaponTypeRet = default;
            if (Strings.InStr(wname, "ビーム") > 0
                || Strings.InStr(wname, "プラズマ") > 0
                || Strings.InStr(wname, "レーザー") > 0
                || Strings.InStr(wname, "ブラスター") > 0
                || Strings.InStr(wname, "ライト") > 0)
            {
                if (Strings.InStr(wname, "サーベル") > 0
                    || Strings.InStr(wname, "セイバー") > 0
                    || Strings.InStr(wname, "ブレード") > 0
                    || Strings.InStr(wname, "ソード") > 0
                    || Strings.InStr(wname, "剣") > 0
                    || Strings.InStr(wname, "刀") > 0)
                {
                    if (Strings.InStr(wname, "ハイパー") > 0
                        || Strings.InStr(wname, "ロング") > 0
                        || Strings.InStr(wname, "大") > 0
                        || Strings.InStr(wname, "高") > 0)
                    {
                        CheckWeaponTypeRet = "ハイパービームサーベル";
                    }
                    else if (Strings.InStr(wname, "セイバー") > 0)
                    {
                        CheckWeaponTypeRet = "ライトセイバー";
                    }
                    else
                    {
                        CheckWeaponTypeRet = "ビームサーベル";
                    }

                    return CheckWeaponTypeRet;
                }

                if (Strings.InStr(wname, "カッター") > 0)
                {
                    if (Strings.InStr(wname, "ハイパー") > 0
                        || Strings.InStr(wname, "ロング") > 0
                        || Strings.InStr(wname, "大") > 0
                        || Strings.InStr(wname, "高") > 0)
                    {
                        CheckWeaponTypeRet = "エナジーブレード";
                    }
                    else
                    {
                        CheckWeaponTypeRet = "エナジーカッター";
                    }

                    return CheckWeaponTypeRet;
                }

                if (Strings.InStr(wname, "ナイフ") > 0
                    || Strings.InStr(wname, "ダガー") > 0)
                {
                    CheckWeaponTypeRet = "ビームナイフ";
                    return CheckWeaponTypeRet;
                }
            }

            if (Strings.InStr(wname, "ナイフ") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "短刀") > 0
                || Strings.InStr(wname, "小刀") > 0)
            {
                if (Strings.InStr(wname, "投") > 0
                    || Strings.InStr(wname, "飛び") > 0
                    || Strings.Right(wname, 3) == "スロー"
                    || Strings.Right(wname, 3) == "スロウ"
                    || GeneralLib.InStrNotNest(wclass, "実") > 0)
                {
                    CheckWeaponTypeRet = "投げナイフ";
                }
                else
                {
                    CheckWeaponTypeRet = "ナイフ";
                }

                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ショートソード") > 0
                || Strings.InStr(wname, "短剣") > 0
                || Strings.InStr(wname, "スモールソード") > 0
                || Strings.InStr(wname, "小剣") > 0)
            {
                CheckWeaponTypeRet = "ショートソード";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "グレートソード") > 0
                || Strings.InStr(wname, "大剣") > 0
                || Strings.InStr(wname, "ハンデッドソード") > 0
                || Strings.InStr(wname, "両手剣") > 0)
            {
                CheckWeaponTypeRet = "大剣";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ロングソード") > 0
                || Strings.InStr(wname, "長剣") > 0
                || Strings.InStr(wname, "バスタードソード") > 0
                || wname == "ソード")
            {
                CheckWeaponTypeRet = "ソード";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "手裏剣") > 0)
            {
                CheckWeaponTypeRet = "手裏剣";
                return CheckWeaponTypeRet;
            }

            if (Strings.Right(wname, 1) == "剣" && (Strings.Len(wname) <= 3
                || Strings.Right(wname, 2) == "の剣"))
            {
                if (Strings.InStr(wname, "ブラック") > 0
                    || Strings.InStr(wname, "黒") > 0)
                {
                    CheckWeaponTypeRet = "黒剣";
                }
                else
                {
                    CheckWeaponTypeRet = "剣";
                }

                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ソードブレイカー") > 0)
            {
                CheckWeaponTypeRet = "ソードブレイカー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "レイピア") > 0)
            {
                CheckWeaponTypeRet = "レイピア";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "シミター") > 0
                || Strings.InStr(wname, "サーベル") > 0
                || Strings.InStr(wname, "カットラス") > 0
                || Strings.InStr(wname, "三日月刀") > 0)
            {
                CheckWeaponTypeRet = "シミター";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ショーテル") > 0)
            {
                CheckWeaponTypeRet = "ショーテル";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ナギナタ") > 0
                || Strings.InStr(wname, "薙刀") > 0
                || Strings.InStr(wname, "グレイブ") > 0)
            {
                CheckWeaponTypeRet = "ナギナタ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "竹刀") > 0)
            {
                CheckWeaponTypeRet = "竹刀";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "脇差") > 0
                || Strings.InStr(wname, "小太刀") > 0)
            {
                CheckWeaponTypeRet = "脇差";
                return CheckWeaponTypeRet;
            }

            if (wname == "刀"
                || wname == "日本刀"
                || Strings.InStr(wname, "太刀") > 0)
            {
                CheckWeaponTypeRet = "日本刀";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "忍者刀") > 0)
            {
                CheckWeaponTypeRet = "忍者刀";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "十手") > 0)
            {
                CheckWeaponTypeRet = "十手";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "青龍刀") > 0)
            {
                CheckWeaponTypeRet = "青龍刀";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "トマホーク") > 0)
            {
                CheckWeaponTypeRet = "トマホーク";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "アックス") > 0
                || Strings.InStr(wname, "斧") > 0)
            {
                if (Strings.InStr(wname, "グレート") > 0
                    || Strings.InStr(wname, "両") > 0
                    || Strings.InStr(wname, "バトル") > 0)
                {
                    CheckWeaponTypeRet = "両刃斧";
                }
                else
                {
                    CheckWeaponTypeRet = "片刃斧";
                }

                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "サイズ") > 0
                || Strings.InStr(wname, "大鎌") > 0)
            {
                CheckWeaponTypeRet = "大鎌";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "鎌") > 0)
            {
                CheckWeaponTypeRet = "鎌";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "スタッフ") > 0
                || Strings.InStr(wname, "杖") > 0)
            {
                CheckWeaponTypeRet = "杖";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "棍棒") > 0)
            {
                CheckWeaponTypeRet = "棍棒";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "警棒") > 0)
            {
                CheckWeaponTypeRet = "警棒";
                return CheckWeaponTypeRet;
            }

            if (wname == "棒")
            {
                CheckWeaponTypeRet = "棒";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "鉄パイプ") > 0)
            {
                CheckWeaponTypeRet = "鉄パイプ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "スタンロッド") > 0)
            {
                CheckWeaponTypeRet = "スタンロッド";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "スパナ") > 0)
            {
                CheckWeaponTypeRet = "スパナ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "メイス") > 0)
            {
                CheckWeaponTypeRet = "メイス";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "ナックル") > 0)
            {
                // ハンマーパンチ等がハンマーにひっかかると困るため、ここで判定
                if (GeneralLib.InStrNotNest(wclass, "実") > 0)
                {
                    CheckWeaponTypeRet = "ロケットパンチ";
                }

                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ウォーハンマー") > 0)
            {
                CheckWeaponTypeRet = "ウォーハンマー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "木槌") > 0)
            {
                CheckWeaponTypeRet = "木槌";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ピコピコハンマー") > 0)
            {
                CheckWeaponTypeRet = "ピコピコハンマー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ハンマー") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "実") > 0)
                {
                    CheckWeaponTypeRet = "鎖鉄球";
                }
                else
                {
                    CheckWeaponTypeRet = "ハンマー";
                }

                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "槌") > 0)
            {
                CheckWeaponTypeRet = "ハンマー";
                return CheckWeaponTypeRet;
            }

            if (Strings.Right(wname, 3) == "モール")
            {
                CheckWeaponTypeRet = "モール";
                return CheckWeaponTypeRet;
            }

            if (Strings.Right(wname, 2) == "ムチ"
                || Strings.InStr(wname, "鞭") > 0
                || Strings.InStr(wname, "ウィップ") > 0)
            {
                CheckWeaponTypeRet = "鞭";
                return CheckWeaponTypeRet;
            }

            if (wname == "サイ")
            {
                CheckWeaponTypeRet = "サイ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "トンファー") > 0)
            {
                CheckWeaponTypeRet = "トンファー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "鉄の爪") > 0)
            {
                CheckWeaponTypeRet = "クロー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ハルバード") > 0)
            {
                CheckWeaponTypeRet = "ハルバード";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "モーニングスター") > 0)
            {
                CheckWeaponTypeRet = "モーニングスター";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "フレイル") > 0)
            {
                CheckWeaponTypeRet = "フレイル";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "鎖鉄球") > 0)
            {
                CheckWeaponTypeRet = "鎖鉄球";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "分銅") > 0)
            {
                CheckWeaponTypeRet = "分銅";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ヌンチャク") > 0)
            {
                CheckWeaponTypeRet = "ヌンチャク";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "三節棍") > 0)
            {
                CheckWeaponTypeRet = "三節棍";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "チェーン") > 0)
            {
                CheckWeaponTypeRet = "チェーン";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ブーメラン") > 0)
            {
                CheckWeaponTypeRet = "ブーメラン";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "チャクラム") > 0)
            {
                CheckWeaponTypeRet = "チャクラム";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ソーサー") > 0)
            {
                CheckWeaponTypeRet = "ソーサー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "クナイ") > 0)
            {
                CheckWeaponTypeRet = "クナイ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "石") > 0
                || Strings.InStr(wname, "礫") > 0)
            {
                CheckWeaponTypeRet = "石";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "岩") > 0)
            {
                CheckWeaponTypeRet = "岩";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "鉄球") > 0)
            {
                CheckWeaponTypeRet = "鉄球";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "手榴弾") > 0)
            {
                CheckWeaponTypeRet = "手榴弾";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ポテトスマッシャー") > 0)
            {
                CheckWeaponTypeRet = "ポテトスマッシャー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ダイナマイト") > 0)
            {
                CheckWeaponTypeRet = "ダイナマイト";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "爆弾") > 0)
            {
                if (Strings.InStr(wname, "投げ") > 0)
                {
                    CheckWeaponTypeRet = "爆弾";
                    return CheckWeaponTypeRet;
                }
            }

            if (Strings.InStr(wname, "火炎瓶") > 0)
            {
                CheckWeaponTypeRet = "火炎瓶";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ネット") > 0
                || Strings.InStr(wname, "網") > 0)
            {
                CheckWeaponTypeRet = "ネット";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "手錠") > 0)
            {
                CheckWeaponTypeRet = "ネット";
                return CheckWeaponTypeRet;
            }

            if (Strings.Right(wname, 2) == "コマ")
            {
                CheckWeaponTypeRet = "コマ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "札") > 0)
            {
                CheckWeaponTypeRet = "お札";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "リボン") > 0)
            {
                CheckWeaponTypeRet = "リボン";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "フープ") > 0)
            {
                CheckWeaponTypeRet = "フープ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "カタログ") > 0)
            {
                CheckWeaponTypeRet = "カタログ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "フライパン") > 0)
            {
                CheckWeaponTypeRet = "フライパン";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "トンボ") > 0)
            {
                CheckWeaponTypeRet = "トンボ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "モップ") > 0)
            {
                CheckWeaponTypeRet = "モップ";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "唐傘") > 0)
            {
                CheckWeaponTypeRet = "唐傘";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "金属バット") > 0)
            {
                CheckWeaponTypeRet = "金属バット";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "釘バット") > 0)
            {
                CheckWeaponTypeRet = "釘バット";
                return CheckWeaponTypeRet;
            }

            if (Strings.Right(wname, 3) == "バット")
            {
                if (Strings.InStr(wname, "ヘッドバット") == 0)
                {
                    CheckWeaponTypeRet = "バット";
                    return CheckWeaponTypeRet;
                }
            }

            if (Strings.InStr(wname, "扇子") > 0)
            {
                CheckWeaponTypeRet = "扇子";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ギター") > 0)
            {
                CheckWeaponTypeRet = "ギター";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ハリセン") > 0)
            {
                CheckWeaponTypeRet = "ハリセン";
                return CheckWeaponTypeRet;
            }

            if (wname == "ゴルフドライバー")
            {
                CheckWeaponTypeRet = "ゴルフドライバー";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "トライデント") > 0
                || Strings.InStr(wname, "三叉槍") > 0
                || Strings.InStr(wname, "ジャベリン") > 0)
            {
                CheckWeaponTypeRet = "トライデント";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "スピア") > 0)
            {
                CheckWeaponTypeRet = "スピア";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "槍") > 0)
            {
                CheckWeaponTypeRet = "和槍";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ランス") > 0
                || Strings.InStr(wname, "ランサー") > 0)
            {
                CheckWeaponTypeRet = "ランス";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "パイク") > 0)
            {
                CheckWeaponTypeRet = "ランス";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "エストック") > 0)
            {
                CheckWeaponTypeRet = "エストック";
                return CheckWeaponTypeRet;
            }

            if (wname == "ロッド")
            {
                CheckWeaponTypeRet = "ロッド";
                return CheckWeaponTypeRet;
            }

            if (Strings.InStr(wname, "ドリル") > 0)
            {
                CheckWeaponTypeRet = "ドリル";
                return CheckWeaponTypeRet;
            }

            return CheckWeaponTypeRet;
        }

        // 武器準備時の効果音
        public void PrepareWeaponSound(Unit u, UnitWeapon w)
        {
            string wname, wclass;

            // フラグをクリア
            Sound.IsWavePlayed = false;
            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;
            if (GeneralLib.InStrNotNest(wclass, "武") > 0
                || GeneralLib.InStrNotNest(wclass, "突") > 0)
            {
                if (Strings.InStr(wname, "ビーム") > 0
                    || Strings.InStr(wname, "プラズマ") > 0
                    || Strings.InStr(wname, "レーザー") > 0
                    || Strings.InStr(wname, "ブラスター") > 0
                    || Strings.InStr(wname, "高周波") > 0
                    || Strings.InStr(wname, "電磁") > 0
                    || wname == "セイバー"
                    || wname == "ライトセイバー"
                    || wname == "ランサー")
                {
                    Sound.PlayWave("BeamSaber.wav");
                }
            }

            // フラグをクリア
            Sound.IsWavePlayed = false;
        }

        // 武器使用時の特殊効果
        public void AttackEffect(Unit u, UnitWeapon w)
        {
            // 右クリック中は特殊効果をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            if (SRC.BattleAnimation)
            {
                AttackAnimation(u, w);
            }
            else
            {
                AttackSound(u, w);
            }
        }

        // 武器使用時のアニメーション
        public void AttackAnimation(Unit u, UnitWeapon w)
        {
            string wtype = default, wname, wclass, wtype0 = default;
            string cname = default, aname, bmpname = default, cname0 = default;
            string sname = default, sname0 = default;
            int attack_times = default;
            var double_weapon = default(bool);
            var double_attack = default(bool);
            var combo_attack = default(bool);
            var is_handy_weapon = default(bool);
            int i;

            // 戦闘アニメ非自動選択オプション
            if (Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            {
                ShowAnimation("デフォルト攻撃");
                return;
            }

            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 二刀流？
            if (Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "デュアル") > 0
                || Strings.InStr(wname, "双") > 0
                || Strings.InStr(wname, "二刀") > 0
                || Strings.InStr(wname, "２連") > 0
                || Strings.InStr(wname, "二連") > 0
                || Strings.InStr(wname, "連装") > 0)
            {
                double_weapon = true;
            }

            // 連続攻撃？
            if (Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "コンビネーション") > 0
                || Strings.InStr(wname, "コンボ") > 0
                || Strings.InStr(wname, "連") > 0
                || GeneralLib.InStrNotNest(wclass, "連") > 0)
            {
                double_attack = true;
            }

            // 乱打？
            if (Strings.InStr(wname, "乱打") > 0
                || Strings.InStr(wname, "乱舞") > 0
                || Strings.InStr(wname, "乱れ") > 0
                || Strings.InStr(wname, "百烈") > 0
                || Strings.Right(wname, 4) == "ラッシュ"
                    && Strings.InStr(wname, "クラッシュ") == 0
                    && Strings.InStr(wname, "スラッシュ") == 0
                    && Strings.InStr(wname, "スプラッシュ") == 0
                    && Strings.InStr(wname, "フラッシュ") == 0)
            {
                combo_attack = true;
            }

            // これから武器の種類を判定

            // まずは白兵戦用武器の判定
            if (GeneralLib.InStrNotNest(wclass, "武") == 0
                && GeneralLib.InStrNotNest(wclass, "突") == 0
                && GeneralLib.InStrNotNest(wclass, "接") == 0
                && GeneralLib.InStrNotNest(wclass, "格") == 0)
            {
                goto SkipInfightWeapon;
            }

            // 投擲武器を除く
            if (Strings.InStr(wname, "投") > 0
                || Strings.InStr(wname, "飛び") > 0
                || Strings.Right(wname, 3) == "スロー"
                || Strings.Right(wname, 3) == "スロウ"
                || GeneralLib.InStrNotNest(wclass, "実") > 0)
            {
                goto SkipInfightWeapon;
            }

            // 移動マップ攻撃
            if (GeneralLib.InStrNotNest(wclass, "Ｍ移") > 0)
            {
                wtype = "ＭＡＰ移動タックル";
                goto FoundWeaponType;
            }

            // 突撃系(武器を構えて突進する)

            if (Strings.InStr(wname, "突撃") > 0
                || Strings.InStr(wname, "突進") > 0
                || Strings.InStr(wname, "チャージ") > 0)
            {
                // 該当せず
                switch (WeaponInHand ?? "")
                {
                    case var @case when @case == "":
                        {
                            break;
                        }

                    default:
                        {
                            wtype = WeaponInHand + "突撃";
                            goto FoundWeaponType;
                        }
                }
            }

            // 打撃系の攻撃

            if (Strings.InStr(wname, "拳法") > 0
                || Strings.Right(wname, 2) == "アーツ"
                || Strings.Right(wname, 5) == "ストライク")
            {
                wtype = "連打";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "触手") > 0
                || Strings.InStr(wname, "触腕") > 0)
            {
                wtype = "白兵連撃";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "チョップ") > 0
                || Strings.InStr(wname, "ナックル") > 0
                || Strings.InStr(wname, "ブロー") > 0
                || Strings.InStr(wname, "拳") > 0
                || Strings.InStr(wname, "掌") > 0
                || Strings.InStr(wname, "打") > 0
                || Strings.InStr(wname, "勁") > 0
                || Strings.InStr(wname, "殴") > 0
                || Strings.Right(wname, 1) == "手"
                || Strings.Right(wname, 1) == "腕")
            {
                if (combo_attack)
                {
                    wtype = "乱打";
                }
                else if (double_attack)
                {
                    wtype = "連打";
                }
                else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                {
                    wtype = "アッパー";
                }
                else
                {
                    wtype = "打突";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "格闘") > 0
                || Strings.InStr(wname, "怪力") > 0)
            {
                wtype = "格闘";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "タックル") > 0
                || Strings.InStr(wname, "体当") > 0
                || Strings.InStr(wname, "チャージ") > 0
                || Strings.InStr(wname, "ぶちかまし") > 0
                || Strings.InStr(wname, "かみつき") > 0)
            {
                wtype = "タックル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "キック") > 0
                || Strings.InStr(wname, "蹴") > 0
                || Strings.InStr(wname, "脚") > 0
                || Strings.Right(wname, 1) == "足")
            {
                wtype = "キック";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ヘッドバット") > 0
                || Strings.InStr(wname, "頭突") > 0)
            {
                wtype = "ヘッドバット";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "アッパー") > 0)
            {
                wtype = "アッパー";
                goto FoundWeaponType;
            }

            // 振って攻撃する武器

            if (Strings.InStr(wname, "ソード") > 0
                || Strings.InStr(wname, "剣") > 0
                || Strings.InStr(wname, "ナイフ") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "シミター") > 0
                || Strings.InStr(wname, "サーベル") > 0
                || Strings.InStr(wname, "カットラス") > 0
                || Strings.InStr(wname, "カッター") > 0
                || Strings.Right(wname, 2) == "ムチ"
                || Strings.InStr(wname, "鞭") > 0
                || Strings.InStr(wname, "ウィップ") > 0
                || Strings.InStr(wname, "ハンマー") > 0
                || Strings.InStr(wname, "ロッド") > 0
                || Strings.InStr(wname, "クロー") > 0
                || Strings.InStr(wname, "爪") > 0
                || Strings.InStr(wname, "ひっかき") > 0
                || Strings.InStr(wname, "アーム") > 0
                || Strings.Right(wname, 1) == "尾")
            {
                if (combo_attack)
                {
                    wtype = "白兵乱撃";
                }
                else if (double_attack)
                {
                    wtype = "白兵連撃";
                }
                else if (Strings.InStr(wname, "回転") > 0)
                {
                    wtype = "白兵回転";
                }
                else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                {
                    wtype = "振り上げ";
                }
                else
                {
                    wtype = "白兵武器";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "刀") > 0
                || Strings.InStr(wname, "斬") > 0
                || Strings.InStr(wname, "ブレード") > 0
                || Strings.InStr(wname, "刃") > 0
                || Strings.InStr(wname, "アックス") > 0
                || Strings.InStr(wname, "斧") > 0
                || Strings.InStr(wname, "カット") > 0
                || Strings.InStr(wname, "カッター") > 0
                || Strings.InStr(wname, "スラッシュ") > 0
                || Strings.InStr(wname, "居合") > 0)
            {
                if (combo_attack)
                {
                    wtype = "白兵乱撃";
                }
                else if (double_attack)
                {
                    wtype = "ダブル斬撃";
                }
                else if (Strings.InStr(wname, "回転") > 0)
                {
                    wtype = "白兵回転";
                }
                else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                {
                    wtype = "振り上げ";
                }
                else if (Strings.InStr(wname, "ブラック") > 0
                    || Strings.InStr(wname, "黒") > 0)
                {
                    wtype = "黒斬撃";
                }
                else
                {
                    wtype = "斬撃";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "サイズ") > 0
                || Strings.InStr(wname, "鎌") > 0
                || Strings.InStr(wname, "グレイブ") > 0
                || Strings.InStr(wname, "ナギナタ") > 0)
            {
                wtype = "振り下ろし";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ショーテル") > 0)
            {
                wtype = "ダブル斬撃";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "円月殺法") > 0)
            {
                wtype = "円月殺法";
                goto FoundWeaponType;
            }

            // 大きく振りまわす武器

            if (Strings.InStr(wname, "鎖鉄球") > 0)
            {
                wtype = "鎖鉄球";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "モーニングスター") > 0)
            {
                wtype = "モーニングスター";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "フレイル") > 0)
            {
                wtype = "フレイル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "分銅") > 0)
            {
                wtype = "分銅";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "チェーン") > 0
                && Strings.InStr(wname, "チェーンソー") == 0)
            {
                wtype = "チェーン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ヌンチャク") > 0)
            {
                wtype = "ヌンチャク";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "三節棍") > 0)
            {
                wtype = "三節棍";
                goto FoundWeaponType;
            }

            // 突き刺す武器

            if (Strings.InStr(wname, "スピア") > 0
                || Strings.InStr(wname, "槍") > 0
                || Strings.InStr(wname, "ランス") > 0
                || Strings.InStr(wname, "ランサー") > 0
                || Strings.InStr(wname, "トライデント") > 0
                || Strings.InStr(wname, "ジャベリン") > 0
                || Strings.InStr(wname, "レイピア") > 0
                || wname == "ロッド")
            {
                if (combo_attack)
                {
                    wtype = "乱突";
                }
                else if (double_attack)
                {
                    wtype = "連突";
                }
                else
                {
                    wtype = "刺突";
                }

                goto FoundWeaponType;
            }

            // 特殊な格闘武器

            if (Strings.InStr(wname, "ドリル") > 0)
            {
                wtype = "ドリル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "チェーンソー") > 0)
            {
                wtype = "チェーンソー";
                goto FoundWeaponType;
            }

            // 詳細が分からなかった武器
            if (GeneralLib.InStrNotNest(wclass, "武") > 0)
            {
                // TODO Impl item
                //// 装備しているアイテムから武器を検索
                //var loopTo = u.CountItem();
                //for (i = 1; i <= loopTo; i++)
                //{
                //    {
                //        var withBlock = u.Item(i);
                //        if (withBlock.Activated
                //&& (withBlock.Part() == "両手"
                //|| withBlock.Part() == "片手"
                //|| withBlock.Part() == "武器"))
                //        {
                //            wtype = CheckWeaponType(withBlock.Nickname(), "");
                //            if (string.IsNullOrEmpty(wtype))
                //            {
                //                wtype = CheckWeaponType(withBlock.Class0(), "");
                //            }

                //            break;
                //        }
                //    }
                //}

                //switch (wtype ?? "")
                //{
                //    case "スピア":
                //    case "ランス":
                //    case "トライデント":
                //    case "和槍":
                //    case "エストック":
                //        {
                //            if (combo_attack)
                //            {
                //                wtype = "乱突";
                //            }
                //            else if (double_attack)
                //            {
                //                wtype = "連突";
                //            }
                //            else
                //            {
                //                wtype = "刺突";
                //            }

                //            break;
                //        }

                //    default:
                //        {
                //            if (combo_attack)
                //            {
                //                wtype = "白兵乱撃";
                //            }
                //            else if (double_attack)
                //            {
                //                wtype = "白兵連撃";
                //            }
                //            else if (Strings.InStr(wname, "回転") > 0)
                //            {
                //                wtype = "白兵回転";
                //            }
                //            else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                //            {
                //                wtype = "振り上げ";
                //            }
                //            else
                //            {
                //                wtype = "白兵武器";
                //            }

                //            break;
                //        }
                //}

                goto FoundWeaponType;
            }

            // 詳細が分からなかった近接技
            if (GeneralLib.InStrNotNest(wclass, "突") > 0
                && GeneralLib.InStrNotNest(wclass, "接") > 0)
            {
                wtype = "格闘";
                goto FoundWeaponType;
            }

        SkipInfightWeapon:
            ;
            if (GeneralLib.InStrNotNest(wclass, "実") == 0)
            {
                goto SkipThrowingWeapon;
            }

            // 投擲武器
            // (真っ直ぐ飛ぶ武器)

            if (Strings.InStr(wname, "槍") > 0
                || Strings.InStr(wname, "スピア") > 0)
            {
                wtype = "投げ槍";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ナイフ") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "クナイ") > 0
                || Strings.InStr(wname, "苦無") > 0)
            {
                wtype = "投げナイフ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "石") > 0
                || Strings.InStr(wname, "礫") > 0)
            {
                wtype = "石";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "岩") > 0)
            {
                wtype = "岩";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "鉄球") > 0)
            {
                wtype = "鉄球";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ダイナマイト") > 0)
            {
                wtype = "ダイナマイト";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "爆弾") > 0)
            {
                if (Strings.InStr(wname, "投げ") > 0)
                {
                    wtype = "爆弾";
                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "ハンドグレネード") > 0)
            {
                wtype = "グレネード投げ";
                goto FoundWeaponType;
            }

            // (回転しながら飛ぶ武器)

            if (Strings.InStr(wname, "トマホーク") > 0)
            {
                wtype = "トマホーク投擲";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "アックス") > 0
                || Strings.InStr(wname, "斧") > 0)
            {
                if (Strings.InStr(wname, "グレート") > 0
                    || Strings.InStr(wname, "両") > 0
                    || Strings.InStr(wname, "バトル") > 0)
                {
                    wtype = "両刃斧投擲";
                }
                else
                {
                    wtype = "片刃斧投擲";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "サイズ") > 0
                || Strings.InStr(wname, "大鎌") > 0)
            {
                wtype = "大鎌投擲";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "鎌") > 0)
            {
                wtype = "鎌投擲";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ブーメラン") > 0)
            {
                wtype = "ブーメラン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "チャクラム") > 0)
            {
                wtype = "チャクラム";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "手裏剣") > 0)
            {
                wtype = "手裏剣";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "手榴弾") > 0)
            {
                wtype = "手榴弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ポテトマッシャー") > 0)
            {
                wtype = "ポテトマッシャー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "火炎瓶") > 0)
            {
                wtype = "火炎瓶";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "手錠") > 0)
            {
                wtype = "手錠";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "フープ") > 0)
            {
                wtype = "フープ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "扇子") > 0)
            {
                wtype = "扇子";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "札") > 0)
            {
                wtype = "お札";
                goto FoundWeaponType;
            }

            // 弓矢

            if (Strings.InStr(wname, "弓") > 0
                || Strings.InStr(wname, "ショートボウ") > 0
                || Strings.InStr(wname, "ロングボウ") > 0)
            {
                wtype = "弓矢";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "矢") > 0
                || Strings.InStr(wname, "アロー") > 0)
            {
                if (CountAttack0(u, w) > 1)
                {
                    wtype = "矢連射";
                }
                else
                {
                    wtype = "矢";
                }

                goto FoundWeaponType;
            }

            // 遠距離系の格闘武器

            // 振る武器

            if (Strings.Right(wname, 2) == "ムチ"
                || Strings.InStr(wname, "鞭") > 0
                || Strings.InStr(wname, "ウィップ") > 0)
            {
                wtype = "白兵武器";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "触手") > 0
                || Strings.InStr(wname, "触腕") > 0)
            {
                wtype = "白兵連撃";
                goto FoundWeaponType;
            }

            // 大きく振りまわす武器

            if (Strings.InStr(wname, "鎖鉄球") > 0
                || Strings.InStr(wname, "ハンマー") > 0)
            {
                wtype = "鎖鉄球";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "分銅") > 0)
            {
                wtype = "分銅";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "チェーン") > 0)
            {
                wtype = "チェーン";
                goto FoundWeaponType;
            }

            // その他格闘系

            if (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "ナックル") > 0)
            {
                wtype = "ロケットパンチ";
                goto FoundWeaponType;
            }

        SkipThrowingWeapon:
            ;


            // これより通常射撃攻撃

            // まずは手持ち武器の判定
            is_handy_weapon = true;

            // 光線系の攻撃かどうかを判定する

            if (IsBeamWeapon(wname, wclass, cname))
            {
                wtype = "ビーム";

                // 実弾系武器判定をスキップ
                goto SkipNormalHandWeapon;
            }

            // 手に持つ射撃武器

            // (大き目の実弾を飛ばすタイプ)

            if (Strings.InStr(wname, "クロスボウ") > 0
                || Strings.InStr(wname, "ボウガン") > 0)
            {
                wtype = "クロスボウ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "バズーカ") > 0)
            {
                wtype = "バズーカ";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "対戦車ライフル") > 0)
            {
                wtype = "対戦車ライフル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "対物ライフル") > 0)
            {
                wtype = "対物ライフル";
                goto FoundWeaponType;
            }

            // (小さな弾を単発で撃つタイプの手持ち火器)

            if (Strings.InStr(wname, "ピストル") > 0
                || Strings.InStr(wname, "拳銃") > 0)
            {
                wtype = "ピストル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "リボルバー") > 0
                || Strings.InStr(wname, "リボルヴァー") > 0)
            {
                wtype = "リボルバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ライフル") > 0
                || Strings.Right(wname, 1) == "銃" && Strings.Right(wname, 2) != "機銃")
            {
                wtype = "ライフル";
                goto FoundWeaponType;
            }

            // (連射するタイプの手持ち火器)

            if (Strings.InStr(wname, "サブマシンガン") > 0)
            {
                wtype = "サブマシンガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "機関銃") > 0)
            {
                if (Strings.InStr(wname, "ヘビー") > 0
                    || Strings.InStr(wname, "重") > 0)
                {
                    wtype = "ヘビーマシンガン";
                }
                else
                {
                    wtype = "マシンガン";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ガトリング") > 0)
            {
                wtype = "ガトリング";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ショットガン") > 0
                || Strings.InStr(wname, "ライアットガン") > 0)
            {
                wtype = "ショットガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "レールガン") > 0
                || Strings.InStr(wname, "リニアガン") > 0)
            {
                Sound.PlayWave("Thunder.wav");
                GUI.Sleep(300);
                wtype = "キャノン砲";
                goto FoundWeaponType;
            }

            // よく分からないのでライフル扱い
            if (Strings.Right(wname, 2) == "ガン")
            {
                wtype = "ライフル";
                goto FoundWeaponType;
            }

            goto SkipHandWeapon;
        SkipNormalHandWeapon:
            ;


            // (手持ちのビーム攻撃)

            if (Strings.InStr(wname, "ライフル") > 0
                || Strings.InStr(wname, "ガン") > 0
                || Strings.InStr(wname, "ピストル") > 0
                || Strings.InStr(wname, "バズーカ") > 0
                || Strings.Right(wname, 1) == "銃" && Strings.Right(wname, 2) != "機銃")
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
                {
                    wtype = "ＭＡＰバスタービームライフル";
                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "ハイメガ") > 0
                    || Strings.InStr(wname, "バスター") > 0
                    || Strings.InStr(wname, "大") > 0
                    || Strings.Left(wname, 2) == "ギガ")
                {
                    wtype = "バスタービームライフル";
                }
                else if (Strings.InStr(wname, "メガ") > 0
                    || Strings.InStr(wname, "ハイ") > 0
                    || Strings.InStr(wname, "バズーカ") > 0)
                {
                    if (double_weapon)
                    {
                        wtype = "ダブルビームランチャー";
                    }
                    else
                    {
                        wtype = "ビームランチャー";
                    }

                    if (Strings.InStr(wname, "ライフル") > 0)
                    {
                        bmpname = @"Weapon\EFFECT_BusterRifle01.bmp";
                    }
                }
                else if (CountAttack0(u, w) >= 4)
                {
                    wtype = "レーザーマシンガン";
                    bmpname = @"Weapon\EFFECT_Rifle01.bmp";
                }
                else if (Strings.InStr(wname, "ピストル") > 0
                    || Strings.InStr(wname, "ミニ") > 0
                    || Strings.InStr(wname, "小") > 0)
                {
                    wtype = "レーザーガン";
                }
                else if (double_weapon)
                {
                    wtype = "ダブルビームライフル";
                }
                else
                {
                    wtype = "ビームライフル";
                }

                if (wtype == "バスター")
                {
                    wtype0 = "粒子集中";
                }

                goto FoundWeaponType;
            }

        SkipHandWeapon:
            ;


            // 内蔵型射撃武器
            is_handy_weapon = false;

            // (大型の実弾火器)

            if (Strings.InStr(wname, "ミサイル") > 0
                || Strings.InStr(wname, "ロケット") > 0)
            {
                wtype = "ミサイル";
                if (Strings.InStr(wname, "ドリル") > 0)
                {
                    wtype = "ドリルミサイル";
                    goto FoundWeaponType;
                }

                attack_times = CountAttack0(u, w);
                if (Strings.InStr(wname, "大型") > 0
                    || Strings.InStr(wname, "ビッグ") > 0
                    || Strings.InStr(wname, "対艦") > 0)
                {
                    wtype = "スーパーミサイル";
                    attack_times = 1;
                }
                else if (Strings.InStr(wname, "小型") > 0)
                {
                    wtype = "小型ミサイル";
                }
                else if (Strings.InStr(wname, "ランチャー") > 0
                    || Strings.InStr(wname, "ポッド") > 0
                    || Strings.InStr(wname, "マイクロ") > 0
                    || Strings.InStr(wname, "スプレー") > 0)
                {
                    wtype = "小型ミサイル";
                    attack_times = 6;
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "グレネード") > 0
                || Strings.InStr(wname, "ディスチャージャー") > 0)
            {
                wtype = "グレネード";
                attack_times = CountAttack0(u, w);
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "シュツルムファウスト") > 0)
            {
                wtype = "実弾発射";
                bmpname = @"Bullet\EFFECT_BazookaBullet01.bmp";
                attack_times = CountAttack0(u, w);
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "爆弾") > 0
                || Strings.InStr(wname, "爆撃") > 0
                || Strings.InStr(wname, "爆雷") > 0)
            {
                if (w.UpdatedWeaponData.MaxRange == 1)
                {
                    wtype = "投下爆弾";
                }
                else
                {
                    wtype = "グレネード";
                    attack_times = CountAttack0(u, w);
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "光子魚雷") > 0)
            {
                wtype = "光子魚雷";
                goto FoundWeaponType;
            }

            // (怪光線系)

            if (Strings.InStr(wname, "怪光線") > 0)
            {
                wtype = "怪光線";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "破壊光線") > 0)
            {
                wtype = "破壊光線";
                goto FoundWeaponType;
            }

            // 特殊な物質を出す武器

            if (Strings.InStr(wname, "消火") > 0)
            {
                wtype = "消火器";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "放水") > 0
                || Strings.InStr(wname, "水流") > 0)
            {
                wtype = "放水銃";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "水鉄砲") > 0
                || Strings.Right(wname, 1) == "液")
            {
                wtype = "実弾発射";
                sname = "Bow.wav";
                if (Strings.InStr(wname, "毒") > 0
                    || Strings.InStr(wname, "毒") > 0)
                {
                    bmpname = @"Bullet\EFFECT_Venom01.bmp";
                }
                else
                {
                    bmpname = @"Bullet\EFFECT_WaterShot01.bmp";
                }

                goto FoundWeaponType;
            }

            // 物理現象系の攻撃(炎や光など)

            if (Strings.InStr(wname, "重力") > 0
                || Strings.InStr(wname, "グラビ") > 0
                || Strings.InStr(wname, "ブラックホール") > 0
                || Strings.InStr(wname, "縮退") > 0)
            {
                wtype = "重力弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "落雷") > 0
                || Strings.Right(wname, 2) == "稲妻")
            {
                wtype = "落雷";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "雷") > 0
                || Strings.InStr(wname, "ライトニング") > 0
                || Strings.InStr(wname, "サンダー") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0)
                {
                    if (w.UpdatedWeaponData.MaxRange == 1)
                    {
                        wtype = "破壊光線";
                        sname = "Thunder.wav";
                    }
                    else
                    {
                        wtype = "落雷";
                    }

                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "電撃") > 0
                || Strings.InStr(wname, "電流") > 0
                || Strings.InStr(wname, "エレクト") > 0)
            {
                wtype = "破壊光線";
                sname = "Thunder.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "エネルギー弾") > 0)
            {
                wtype = "球電";
                sname = "Beam.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "泡") > 0
                || Strings.InStr(wname, "バブル") > 0)
            {
                wtype = "泡";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "音波") > 0
                || Strings.InStr(wname, "サウンド") > 0
                || Strings.InStr(wname, "ソニック") > 0
                || GeneralLib.InStrNotNest(wclass, "音") > 0 && Strings.InStr(wname, "ショック") > 0
                || Strings.InStr(wname, "ウェーブ") > 0
                || Strings.InStr(wname, "叫び") > 0
                || GeneralLib.InStrNotNest(wclass, "音") > 0 && Strings.InStr(wname, "咆哮") > 0)
            {
                wtype = "音波";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "歌") > 0
                || Strings.InStr(wname, "ソング") > 0)
            {
                wtype = "音符";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "針") > 0
                || Strings.InStr(wname, "ニードル") > 0)
            {
                wtype = "ニードル";
                if (CountAttack0(u, w) > 1)
                {
                    wtype = "ニードル連射";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "津波") > 0
                || Strings.InStr(wname, "ダイダル") > 0)
            {
                wtype = "津波";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "コメット") > 0)
            {
                wtype = "流星";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "メテオ") > 0
                || Strings.InStr(wname, "隕石") > 0)
            {
                wtype = "隕石";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "竜巻") > 0
                || Strings.InStr(wname, "渦巻") > 0
                || Strings.InStr(wname, "トルネード") > 0
                || Strings.InStr(wname, "サイクロン") > 0)
            {
                wtype = "竜巻";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "つらら") > 0)
            {
                wtype = "氷弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "つぶて") > 0)
            {
                wtype = "岩弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "吹雪") > 0
                || Strings.InStr(wname, "ブリザード") > 0
                || Strings.InStr(wname, "アイスストーム") > 0)
            {
                wtype = "吹雪";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ストーム") > 0
                || Strings.InStr(wname, "ハリケーン") > 0
                || Strings.InStr(wname, "タイフーン") > 0
                || Strings.InStr(wname, "台風") > 0
                || Strings.InStr(wname, "嵐") > 0)
            {
                wtype = "強風";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ウィンド") > 0
                || Strings.InStr(wname, "ウインド") > 0
                || Strings.InStr(wname, "風") > 0)
            {
                wtype = "風";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "煙") > 0
                || Strings.InStr(wname, "スモーク") > 0
                || Strings.Right(wname, 2) == "ガス"
                || Strings.Right(wname, 1) == "霧"
                || Strings.InStr(wname, "胞子") > 0)
            {
                wtype = "煙";
                if (Strings.InStr(wname, "毒") > 0
                    || GeneralLib.InStrNotNest(wclass, "毒") > 0)
                {
                    cname = "緑";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "火炎弾") > 0)
            {
                wtype = "火炎弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "火炎放射") > 0
                || Strings.Right(wname, 2) == "火炎")
            {
                wtype = "火炎放射";
                sname = "AntiShipMissile.wav";
                goto FoundWeaponType;
            }

            if (Strings.Right(wname, 5) == "ファイアー"
                || Strings.Right(wname, 5) == "ファイヤー"
                || Strings.Right(wname, 4) == "ファイア"
                || Strings.Right(wname, 4) == "ファイヤ")
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0
                    && Strings.Left(wname, 2) != "フル")
                {
                    if (GeneralLib.InStrNotNest(wclass, "術") > 0)
                    {
                        wtype = "炎投射";
                    }
                    else
                    {
                        wtype = "火炎放射";
                        sname = "AntiShipMissile.wav";
                    }

                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "息") > 0
                || Strings.Right(wname, 3) == "ブレス")
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0)
                {
                    wtype = "火炎放射";
                    sname = "Breath.wav";
                    switch (SpellColor(wname, wclass) ?? "")
                    {
                        case "赤":
                        case "青":
                        case "黄":
                        case "緑":
                        case "白":
                        case "黒":
                            {
                                cname = SpellColor(wname, wclass);
                                break;
                            }
                    }

                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "エネルギー波") > 0)
            {
                wtype = "波動放射";
                sname = "Beam.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "衝撃") > 0)
            {
                wtype = "波動放射";
                cname = "白";
                sname = "Bazooka.wav";
                goto FoundWeaponType;
            }

            // 霊的、魔法的な攻撃

            if (Strings.InStr(wname, "気弾") > 0)
            {
                wtype = "気弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ソニックブレード") > 0)
            {
                wtype = "気斬";
                goto FoundWeaponType;
            }

            if (w.IsSpellWeapon()
                || GeneralLib.InStrNotNest(wclass, "魔") > 0)
            {
                // wtype = "魔法放射"
                // cname = SpellColor(wname, wclass)
                wtype = "デフォルト";
                sname = "Whiz.wav";
                goto FoundWeaponType;
            }

            // (ビーム攻撃)

            if (wtype == "ビーム")
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
                {
                    wtype = "ＭＡＰビーム";
                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "ハイメガ") > 0
                    || Strings.InStr(wname, "バスター") > 0
                    || Strings.InStr(wname, "大") > 0
                    || Strings.Left(wname, 2) == "ギガ")
                {
                    wtype = "大ビーム";
                }
                else if (Strings.InStr(wname, "メガ") > 0
                    || Strings.InStr(wname, "ハイ") > 0)
                {
                    wtype = "中ビーム";
                }
                else if (CountAttack0(u, w) >= 4
                    || Strings.InStr(wname, "対空") > 0)
                {
                    wtype = "ニードルレーザー連射";
                }
                else if (Strings.InStr(wname, "ミニ") > 0
                    || Strings.InStr(wname, "小") > 0)
                {
                    wtype = "ニードルレーザー";
                }
                else if (Strings.InStr(wname, "ランチャー") > 0
                    || Strings.InStr(wname, "キャノン") > 0
                    || Strings.InStr(wname, "カノン") > 0
                    || Strings.InStr(wname, "砲") > 0)
                {
                    wtype = "中ビーム";
                }
                else
                {
                    wtype = "小ビーム";
                }

                if (wtype == "大ビーム")
                {
                    wtype0 = "粒子集中";
                }

                switch (wtype ?? "")
                {
                    case "小ビーム":
                    case "中ビーム":
                        {
                            if (double_weapon)
                            {
                                wtype = "２連" + wtype;
                            }

                            break;
                        }
                }

                if (Strings.InStr(wname, "拡散") > 0
                    || Strings.InStr(wname, "放射") > 0
                    || Strings.InStr(wname, "ホーミング") > 0
                    || Strings.InStr(wname, "誘導") > 0)
                {
                    wtype = "拡散ビーム";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "光線") > 0)
            {
                wtype = "怪光線";
                goto FoundWeaponType;
            }

            // (小型で連射する火器)

            if (Strings.InStr(wname, "バルカン") > 0)
            {
                wtype = "バルカン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "機銃") > 0
                || Strings.InStr(wname, "機関砲") > 0)
            {
                wtype = "機関砲";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "チェーンガン") > 0
                || Strings.InStr(wname, "ガンランチャー") > 0)
            {
                wtype = "内蔵ガトリング";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "マシンキャノン") > 0
                || Strings.InStr(wname, "オートキャノン") > 0
                || Strings.InStr(wname, "速射砲") > 0)
            {
                wtype = "重機関砲";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ベアリング") > 0
                || Strings.InStr(wname, "クレイモア") > 0)
            {
                wtype = "ベアリング";
                goto FoundWeaponType;
            }

            // (オールレンジ攻撃)

            if (Strings.InStr(wname, "有線") > 0)
            {
                wtype = "２ＷＡＹ射出";
                goto FoundWeaponType;
            }

            // 汎用的な「砲」の指定は最後に判定
            if (Strings.InStr(wname, "砲") > 0
                || Strings.InStr(wname, "キャノン") > 0
                || Strings.InStr(wname, "カノン") > 0
                || Strings.InStr(wname, "弾") > 0)
            {
                if (Strings.InStr(wname, "リニア") > 0
                    || Strings.InStr(wname, "レール") > 0
                    || Strings.InStr(wname, "電磁") > 0)
                {
                    Sound.PlayWave("Thunder.wav");
                    GUI.Sleep(300);
                }

                wtype = "キャノン砲";
                attack_times = CountAttack0(u, w);
                goto FoundWeaponType;
            }

        SkipShootingWeapon:
            ;


            // 対応する武器は見つからなかった
            wtype = "デフォルト";
        FoundWeaponType:
            ;


            // 空中移動専用形態は武器を手で構えない。
            // また等身大基準の場合、非人間ユニットはメカであることが多いのでこちらも
            // 内蔵武器を優先する。
            if (is_handy_weapon && (u.Data.Transportation == "空"
                || Expression.IsOptionDefined("等身大基準") && !u.IsHero()))
            {
                switch (wtype ?? "")
                {
                    case "ＭＡＰバスタービームライフル":
                        {
                            wtype = "ＭＡＰビーム";
                            break;
                        }

                    case "バスタービームライフル":
                        {
                            wtype = "大ビーム";
                            break;
                        }

                    case "ダブルビームランチャー":
                        {
                            wtype = "２連中ビーム";
                            break;
                        }

                    case "ビームランチャー":
                        {
                            wtype = "中ビーム";
                            break;
                        }

                    case "ダブルビームライフル":
                        {
                            wtype = "２連小ビーム";
                            break;
                        }

                    case "ビームライフル":
                        {
                            wtype = "小ビーム";
                            break;
                        }

                    case "レーザーマシンガン":
                        {
                            wtype = "ニードルレーザー連射";
                            break;
                        }

                    case "レーザーガン":
                        {
                            wtype = "ニードルレーザー";
                            break;
                        }

                    case "サブマシンガン":
                    case "マシンガン":
                        {
                            wtype = "機関砲";
                            break;
                        }

                    case "ヘビーマシンガン":
                        {
                            wtype = "重機関砲";
                            break;
                        }

                    case "ガトリング":
                        {
                            wtype = "内蔵ガトリング";
                            break;
                        }

                    case "ショットガン":
                        {
                            wtype = "ベアリング";
                            break;
                        }

                    default:
                        {
                            // 手持ち武器の画像を空にする
                            bmpname = "-.bmp";
                            break;
                        }
                }
            }

            // マップ攻撃？
            if (GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
            {
                // マップ攻撃対応アニメに置き換え
                switch (wtype ?? "")
                {
                    case "矢":
                    case "小型ミサイル":
                    case "ミサイル":
                    case "スーパーミサイル":
                    case "グレネード":
                    case "キャノン砲":
                    case "大キャノン砲":
                    case "ＩＣＢＭ":
                    case "シュートカッター":
                    case "球電":
                    case "氷弾":
                    case "火炎弾":
                    case "岩弾":
                    case "発光":
                    case "落雷":
                    case "放電":
                    case "氷柱":
                    case "つらら":
                    case "凍結":
                    case "吹雪":
                    case "風":
                    case "強風":
                    case "竜巻":
                    case "津波":
                    case "泡":
                    case "音符":
                    case "オールレンジ":
                    case "煙":
                    case "気弾":
                    case "連気弾":
                    case "気斬":
                    case "波動放射":
                        {
                            wtype = "ＭＡＰ" + wtype;
                            break;
                        }

                    case "炎":
                    case "炎投射":
                    case "火炎放射":
                        {
                            wtype = "ＭＡＰ炎";
                            break;
                        }

                    case "ニードル":
                    case "ニードル連射":
                        {
                            wtype = "ＭＡＰニードル";
                            break;
                        }

                    case "投下爆弾":
                        {
                            wtype = "ＭＡＰ爆発";
                            break;
                        }

                    case "重力弾":
                        {
                            wtype = "ＭＡＰブラックホール";
                            break;
                        }

                    default:
                        {
                            if (Strings.InStr(wname, "フラッシュ") > 0
                                || Strings.InStr(wname, "閃光") > 0)
                            {
                                wtype = "ＭＡＰフラッシュ";
                            }
                            else if (Strings.InStr(wname, "ダーク") > 0
                                || Strings.InStr(wname, "闇") > 0)
                            {
                                wtype = "ＭＡＰダークネス";
                            }
                            else if (Strings.InStr(wname, "地震") > 0
                                || Strings.InStr(wname, "クウェイク") > 0
                                || Strings.InStr(wname, "クエイク") > 0)
                            {
                                wtype = "ＭＡＰ地震";
                                sname = " Explode(Far).wav";
                            }
                            else if (Strings.InStr(wname, "核") > 0
                                || Strings.InStr(wname, "アトミック") > 0)
                            {
                                wtype = "ＭＡＰ核爆発";
                            }

                            break;
                        }
                }
            }


            // 使用した攻撃手段を記録
            CurrentWeaponType = wtype;

            // 描画色を最終決定
            if (Strings.InStr(wname, "レッド") > 0
                || Strings.InStr(wname, "赤") > 0)
            {
                cname = "赤";
            }
            else if (Strings.InStr(wname, "ブルー") > 0
                || Strings.InStr(wname, "青") > 0)
            {
                cname = "青";
            }
            else if (Strings.InStr(wname, "イエロー") > 0
                || Strings.InStr(wname, "黄") > 0)
            {
                cname = "黄";
            }
            else if (Strings.InStr(wname, "グリーン") > 0
                || Strings.InStr(wname, "緑") > 0)
            {
                cname = "緑";
            }
            else if (Strings.InStr(wname, "ピンク") > 0
                || Strings.InStr(wname, "桃") > 0)
            {
                cname = "桃";
            }
            else if (Strings.InStr(wname, "ブラウン") > 0
                || Strings.InStr(wname, "橙") > 0)
            {
                cname = "橙";
            }
            else if (Strings.InStr(wname, "ブラック") > 0
                || Strings.InStr(wname, "黒") > 0
                || Strings.InStr(wname, "ダーク") > 0
                || Strings.InStr(wname, "闇") > 0)
            {
                cname = "黒";
            }
            else if (Strings.InStr(wname, "ホワイト") > 0
                || Strings.InStr(wname, "白") > 0
                || Strings.InStr(wname, "ホーリー") > 0
                || Strings.InStr(wname, "聖") > 0)
            {
                cname = "白";
            }

            // ２種類のアニメを組み合わせる場合
            if (Strings.Len(wtype0) > 0)
            {
                // 表示する準備アニメの種類
                aname = wtype0 + "準備";

                // 色
                if (Strings.Len(cname0) > 0)
                {
                    aname = aname + " " + cname0;
                }
                else if (Strings.Len(cname) > 0)
                {
                    aname = aname + " " + cname;
                }

                // 効果音
                if (Strings.Len(sname0) > 0)
                {
                    aname = aname + " " + sname0;
                }

                // 戦闘アニメ表示
                ShowAnimation(aname);
            }

            // 表示する攻撃アニメの種類
            aname = wtype + "攻撃";

            // 発射回数
            if (attack_times > 0)
            {
                aname = aname + " " + SrcFormatter.Format(attack_times);
            }

            // 画像
            if (Strings.Len(bmpname) > 0)
            {
                aname = aname + " " + bmpname;
            }

            // 色
            if (Strings.Len(cname) > 0)
            {
                aname = aname + " " + cname;
            }

            // 効果音
            if (Strings.Len(sname) > 0)
            {
                aname = aname + " " + sname;
            }

            // 攻撃アニメ表示
            ShowAnimation(aname);
        }

        // 武器使用時の効果音
        public void AttackSound(Unit u, UnitWeapon w)
        {
            string wname, wclass;
            var sname = default(string);
            int num;
            int i;

            // フラグをクリア
            Sound.IsWavePlayed = false;

            // 右クリック中は効果音をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 効果音が必要ないもの
            if (w.IsWeaponClassifiedAs("武")
                || w.IsWeaponClassifiedAs("突")
                || w.IsWeaponClassifiedAs("接"))
            {
                return;
            }

            if (Strings.InStr(wname, "ビームサーベル") > 0)
            {
                return;
            }

            if (GeneralLib.InStrNotNest(wclass, "武") > 0)
            {
                if (Strings.InStr(wname, "銃剣") > 0)
                {
                    return;
                }
            }

            // 効果音の再生回数
            num = CountAttack(u, w);

            // 武器名に応じて効果音を選択
            if (Strings.InStr(wname, "主砲") > 0
                || Strings.InStr(wname, "副砲") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Cannon.wav";
                }
            }
            else if (Strings.InStr(wname, "対空砲") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                    num = 4;
                }
                else
                {
                    sname = "MachineCannon.wav";
                }
            }
            else if (Strings.InStr(wname, "レーザー") > 0
                || Strings.InStr(wname, "光線") > 0
                || Strings.InStr(wname, "凝集光") > 0
                || Strings.InStr(wname, "熱線") > 0
                || Strings.InStr(wname, "冷線") > 0
                || Strings.InStr(wname, "衝撃波") > 0
                || Strings.InStr(wname, "電磁波") > 0
                || Strings.InStr(wname, "電波") > 0
                || Strings.InStr(wname, "音波") > 0
                || Strings.InStr(wname, "磁力") > 0
                || Strings.InStr(wname, "ブラックホール") > 0
                || Strings.InStr(wname, "縮退") > 0
                || Strings.InStr(wname, "ウェーブ") > 0
                || Strings.InStr(wname, "波動") > 0
                || Strings.InStr(wname, "ソニック") > 0
                || Strings.InStr(wname, "スパーク") > 0
                || Strings.InStr(wname, "エネルギー") > 0)
            {
                sname = "LaserGun.wav";
            }
            else if (Strings.InStr(wname, "粒子") > 0
                || Strings.InStr(wname, "陽電子") > 0
                || Strings.InStr(wname, "陽子") > 0
                || Strings.InStr(wname, "ブラスター") > 0
                || Strings.InStr(wname, "ブラスト") > 0
                || Strings.InStr(wname, "フェイザー") > 0
                || Strings.InStr(wname, "ディスラプター") > 0
                || Strings.InStr(wname, "スマッシャー") > 0
                || Strings.InStr(wname, "スラッシャー") > 0
                || Strings.InStr(wname, "フラッシャー") > 0
                || Strings.InStr(wname, "ディバイダー") > 0
                || Strings.InStr(wname, "ドライバー") > 0
                || Strings.InStr(wname, "シュトラール") > 0
                || Strings.InStr(wname, "ニュートロン") > 0
                || Strings.InStr(wname, "プラズマ") > 0
                || Strings.InStr(wname, "イオン") > 0
                || Strings.InStr(wname, "プロミネンス") > 0
                || Strings.InStr(wname, "ハイドロ") > 0
                || Strings.InStr(wname, "インパルス") > 0
                || Strings.InStr(wname, "フレイム") > 0
                || Strings.InStr(wname, "サンシャイン") > 0)
            {
                sname = "Beam.wav";
            }
            else if (Strings.InStr(wname, "シューター") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "実") > 0)
                {
                    sname = "Missile.wav";
                }
                else
                {
                    sname = "Beam.wav";
                }
            }
            else if (Strings.InStr(wname, "ビーム") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "LaserGun.wav";
                }

                if (Strings.InStr(wname, "バルカン") > 0
                    || Strings.InStr(wname, "マシンガン") > 0)
                {
                    num = 4;
                }
            }
            else if (Strings.InStr(wname, "機関銃") > 0
                || Strings.InStr(wname, "機銃") > 0
                || Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "アサルトライフル") > 0
                || Strings.InStr(wname, "チェーンライフル") > 0
                || Strings.InStr(wname, "パレットライフル") > 0
                || Strings.InStr(wname, "マウラー砲") > 0
                || Strings.InStr(wname, "ＳＭＧ") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "LaserGun.wav";
                }
                else
                {
                    sname = "MachineGun.wav";
                }

                num = 1;
            }
            else if (Strings.InStr(wname, "機関砲") > 0
                || Strings.InStr(wname, "速射砲") > 0
                || Strings.InStr(wname, "マシンキャノン") > 0
                || Strings.InStr(wname, "モーターカノン") > 0
                || Strings.InStr(wname, "ガンクラスター") > 0
                || Strings.InStr(wname, "チェーンガン") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "LaserGun.wav";
                }
                else
                {
                    sname = "MachineCannon.wav";
                }

                num = 1;
            }
            else if (Strings.InStr(wname, "ガンポッド") > 0
                || Strings.InStr(wname, "バルカン") > 0
                || Strings.InStr(wname, "ガトリング") > 0
                || Strings.InStr(wname, "ハンドレールガン") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "LaserGun.wav";
                }
                else
                {
                    sname = "GunPod.wav";
                }

                num = 1;
            }
            else if (Strings.InStr(wname, "リニアキャノン") > 0
                || Strings.InStr(wname, "レールキャノン") > 0
                || Strings.InStr(wname, "リニアカノン") > 0
                || Strings.InStr(wname, "レールカノン") > 0
                || Strings.InStr(wname, "リニアガン") > 0
                || Strings.InStr(wname, "レールガン") > 0
                || Strings.InStr(wname, "電磁") > 0 && Strings.InStr(wname, "砲") > 0)
            {
                Sound.PlayWave("Thunder.wav");
                GUI.Sleep(300);
                Sound.PlayWave("Cannon.wav");
                var loopTo = num;
                for (i = 2; i <= loopTo; i++)
                {
                    GUI.Sleep(130);
                    Sound.PlayWave("Cannon.wav");
                }
            }
            else if (Strings.InStr(wname, "ライフル") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Rifle.wav";
                }
            }
            else if (Strings.InStr(wname, "バズーカ") > 0
                || Strings.InStr(wname, "ジャイアントバズ") > 0
                || Strings.InStr(wname, "シュツルムファウスト") > 0
                || Strings.InStr(wname, "グレネード") > 0
                || Strings.InStr(wname, "グレネイド") > 0
                || Strings.InStr(wname, "ナパーム") > 0
                || Strings.InStr(wname, "クレイモア") > 0
                || Strings.InStr(wname, "ロケット砲") > 0
                || Strings.InStr(wname, "迫撃砲") > 0
                || Strings.InStr(wname, "無反動砲") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Bazooka.wav";
                }
            }
            else if (Strings.InStr(wname, "自動砲") > 0
                || Strings.InStr(wname, "オートキャノン") > 0)
            {
                sname = "FastGun.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "弓") > 0
                || Strings.InStr(wname, "アロー") > 0
                || Strings.InStr(wname, "ボーガン") > 0
                || Strings.InStr(wname, "ボウガン") > 0
                || Strings.InStr(wname, "ロングボウ") > 0
                || Strings.InStr(wname, "ショートボウ") > 0
                || Strings.InStr(wname, "針") > 0
                || Strings.InStr(wname, "髪") > 0)
            {
                sname = "Bow.wav";
            }
            else if (Strings.InStr(wname, "マイン") > 0
                || Strings.InStr(wname, "クラッカー") > 0
                || Strings.InStr(wname, "手投弾") > 0
                || Strings.InStr(wname, "手榴弾") > 0
                || Strings.InStr(wname, "投げ") > 0
                || Strings.InStr(wname, "スリング") > 0
                || Strings.InStr(wname, "手裏剣") > 0
                || Strings.InStr(wname, "苦無") > 0
                || Strings.InStr(wname, "クナイ") > 0)
            {
                sname = "Swing.wav";
            }
            else if (Strings.InStr(wname, "爆弾") > 0
                || Strings.InStr(wname, "爆雷") > 0
                || Strings.InStr(wname, "爆撃") > 0)
            {
                sname = "Bomb.wav";
            }
            else if (Strings.InStr(wname, "機雷") > 0)
            {
                sname = "Explode.wav";
            }
            else if (Strings.InStr(wname, "マイクロミサイル") > 0
                && GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
            {
                sname = "MicroMissile.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "全方位ミサイル") > 0)
            {
                sname = "MicroMissile.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "ミサイル") > 0
                || Strings.InStr(wname, "ロケット") > 0
                || Strings.InStr(wname, "魚雷") > 0
                || Strings.InStr(wname, "反応弾") > 0
                || Strings.InStr(wname, "マルチポッド") > 0
                || Strings.InStr(wname, "マルチランチャー") > 0
                || Strings.InStr(wname, "ショット") > 0
                || Strings.InStr(wname, "フルファイア") > 0
                || Strings.InStr(wname, "ストリーム") > 0
                || Strings.InStr(wname, "ナックル") > 0
                || Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "鉄腕") > 0
                || Strings.InStr(wname, "発射") > 0
                || Strings.InStr(wname, "射出") > 0
                || Strings.InStr(wname, "ランチャー") > 0
                || Strings.InStr(wname, "ＡＴＭ") > 0
                || Strings.InStr(wname, "ＡＡＭ") > 0
                || Strings.InStr(wname, "ＡＧＭ") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Missile.wav";
                }
            }
            else if (Strings.InStr(wname, "砲") > 0
                || Strings.InStr(wname, "弾") > 0
                || Strings.InStr(wname, "キャノン") > 0
                || Strings.InStr(wname, "カノン") > 0
                || Strings.InStr(wname, "ボム") > 0
                || Strings.InStr(wname, "火球") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Cannon.wav";
                }
            }
            else if (Strings.InStr(wname, "ガン") > 0
                || Strings.InStr(wname, "ピストル") > 0
                || Strings.InStr(wname, "リボルヴァー") > 0
                || Strings.InStr(wname, "マグナム") > 0
                || Strings.InStr(wname, "ライアット") > 0
                || Strings.InStr(wname, "銃") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
                {
                    sname = "Beam.wav";
                }
                else
                {
                    sname = "Gun.wav";
                }
            }
            else if (Strings.InStr(wname, "ソニックブレード") > 0
                || Strings.InStr(wname, "ビームカッター") > 0
                || Strings.InStr(wname, "スライサー") > 0)
            {
                sname = "Saber.wav";
            }
            else if (Strings.InStr(wname, "重力") > 0
                || Strings.InStr(wname, "グラビ") > 0)
            {
                sname = "Shock(Low).wav";
            }
            else if (Strings.InStr(wname, "ストーム") > 0
                || Strings.InStr(wname, "トルネード") > 0
                || Strings.InStr(wname, "ハリケーン") > 0
                || Strings.InStr(wname, "タイフーン") > 0
                || Strings.InStr(wname, "サイクロン") > 0
                || Strings.InStr(wname, "ブリザード") > 0
                || Strings.InStr(wname, "竜巻") > 0
                || Strings.InStr(wname, "渦巻") > 0
                || Strings.InStr(wname, "台風") > 0
                || Strings.InStr(wname, "嵐") > 0
                || Strings.InStr(wname, "吹雪") > 0
                || Strings.InStr(wname, "フリーザー") > 0
                || Strings.InStr(wname, "テレキネシス") > 0)
            {
                sname = "Storm.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "ブーメラン") > 0
                || Strings.InStr(wname, "ウェッブ") > 0)
            {
                sname = "Swing.wav";
                num = 5;
            }
            else if (Strings.InStr(wname, "サンダー") > 0
                || Strings.InStr(wname, "ライトニング") > 0
                || Strings.InStr(wname, "ボルト") > 0
                || Strings.InStr(wname, "稲妻") > 0
                || Strings.InStr(wname, "放電") > 0
                || Strings.InStr(wname, "電撃") > 0
                || Strings.InStr(wname, "電流") > 0
                || Strings.InStr(wname, "雷") > 0
                || GeneralLib.InStrNotNest(wclass, "雷") > 0)
            {
                sname = "Thunder.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "火炎放射") > 0)
            {
                sname = "AntiShipMissile.wav";
            }
            else if (Strings.InStr(wname, "火炎") > 0
                || Strings.InStr(wname, "焔") > 0)
            {
                sname = "Fire.wav";
                num = 1;
            }
            else if (Strings.InStr(wname, "魔法") > 0
                || GeneralLib.InStrNotNest(wclass, "魔") > 0
                || Strings.InStr(wname, "サイコキネシス") > 0
                || Strings.InStr(wname, "糸") > 0
                || Strings.InStr(wname, "アンカー") > 0)
            {
                sname = "Whiz.wav";
            }
            else if (Strings.InStr(wname, "泡") > 0
                || Strings.InStr(wname, "バブル") > 0)
            {
                sname = "Bubble.wav";
            }
            else if (Strings.Right(wname, 1) == "液")
            {
                sname = "Shower.wav";
            }
            else if (Strings.Right(wname, 3) == "ブレス"
                || Strings.Right(wname, 3) == "の息")
            {
                if (GeneralLib.InStrNotNest(wclass, "火") > 0)
                {
                    sname = "AntiShipMissile.wav";
                }
                else if (GeneralLib.InStrNotNest(wclass, "冷") > 0)
                {
                    sname = "Storm.wav";
                }
                else if (GeneralLib.InStrNotNest(wclass, "闇") > 0)
                {
                    sname = "GunPod.wav";
                }
                else if (GeneralLib.InStrNotNest(wclass, "水") > 0)
                {
                    sname = "Hide.wav";
                }
                else
                {
                    sname = "AntiShipMissile.wav";
                }
            }
            else if (Strings.InStr(wname, "一斉射撃") > 0)
            {
                sname = "MultipleRocketLauncher(Light).wav";
                num = 1;
            }
            else if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
            {
                // なんか分からんけどビーム
                sname = "Beam.wav";
            }
            else if (GeneralLib.InStrNotNest(wclass, "銃") > 0)
            {
                // なんか分からんけど銃
                sname = "Gun.wav";
            }

            // 効果音なし？
            if (string.IsNullOrEmpty(sname))
            {
                // フラグをクリア
                Sound.IsWavePlayed = false;
                return;
            }

            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                Sound.PlayWave(sname);

                // ウェイトを入れる
                GUI.Sleep(130);
                if (sname == "Swing.wav")
                {
                    GUI.Sleep(150);
                }
            }

            // フラグをクリア
            Sound.IsWavePlayed = false;
        }


        // 武器命中時の特殊効果
        public void HitEffect(Unit u, UnitWeapon w, Unit t, int hit_count = 0)
        {

            // 右クリック中は特殊効果をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            if (SRC.BattleAnimation)
            {
                HitAnimation(u, w, t, hit_count);
            }
            else
            {
                HitSound(u, w, t, hit_count);
            }
        }

        // 武器命中時のアニメーション
        public void HitAnimation(Unit u, UnitWeapon w, Unit t, int hit_count)
        {
            string wtype = default, wname, wclass, wtype0 = default;
            string cname = default, aname, sname = default;
            int attack_times = default;
            var double_weapon = default(bool);
            var double_attack = default(bool);
            var combo_attack = default(bool);
            int i;

            // 戦闘アニメ非自動選択オプション
            if (Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            {
                ShowAnimation("ダメージ命中");
                return;
            }

            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // マップ攻撃の場合は武器にかかわらずダメージを使う
            if (GeneralLib.InStrNotNest(wclass, "Ｍ") > 0)
            {
                // 攻撃力0の攻撃の場合は「ダメージ」のアニメを使用しない
                if (w.WeaponPower("") == 0)
                {
                    return;
                }

                wtype = "ダメージ";
                if (IsBeamWeapon(wname, wclass, cname)
                    || Strings.InStr(wname, "ミサイル") > 0
                    || Strings.InStr(wname, "ロケット") > 0)
                {
                    sname = "Explode.wav";
                }

                goto FoundWeaponType;
            }

            // 二刀流？
            if (Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "デュアル") > 0
                || Strings.InStr(wname, "双") > 0
                || Strings.InStr(wname, "二刀") > 0
                || Strings.InStr(wname, "２連") > 0
                || Strings.InStr(wname, "二連") > 0
                || Strings.InStr(wname, "連装") > 0)
            {
                double_weapon = true;
            }

            // 連続攻撃？
            if (Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "コンビネーション") > 0
                || Strings.InStr(wname, "連") > 0
                || GeneralLib.InStrNotNest(wclass, "連") > 0)
            {
                double_attack = true;
            }

            // 乱打？
            if (Strings.InStr(wname, "乱打") > 0
                || Strings.InStr(wname, "乱舞") > 0
                || Strings.InStr(wname, "乱れ") > 0
                || Strings.InStr(wname, "百烈") > 0)
            {
                combo_attack = true;
            }

            // これから武器の種類を判定

            // まずは白兵戦用武器の判定
            if (GeneralLib.InStrNotNest(wclass, "武") == 0
                && GeneralLib.InStrNotNest(wclass, "突") == 0
                && GeneralLib.InStrNotNest(wclass, "接") == 0
                && !(GeneralLib.InStrNotNest(wclass, "格") > 0
                && GeneralLib.InStrNotNest(wclass, "実") > 0))
            {
                goto SkipInfightWeapon;
            }

            // 突撃系(武器を構えて突進する)

            if (Strings.InStr(wname, "突撃") > 0
                || Strings.InStr(wname, "突進") > 0
                || Strings.InStr(wname, "チャージ") > 0)
            {
                // 該当せず
                switch (WeaponInHand ?? "")
                {
                    case var @case when @case == "":
                        {
                            break;
                        }

                    default:
                        {
                            wtype = WeaponInHand + "突撃";
                            goto FoundWeaponType;
                        }
                }
            }

            // 打撃系

            if (GeneralLib.InStrNotNest(wclass, "実") > 0 && (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "ナックル") > 0))
            {
                wtype = "ロケットパンチ";
                goto FoundWeaponType;
            }

            // 乱打
            if (Strings.InStr(wname, "拳法") > 0
                || Strings.Right(wname, 2) == "アーツ"
                || Strings.Right(wname, 5) == "ストライク")
            {
                wtype = "連打";
                goto FoundWeaponType;
            }

            // 通常打撃
            if (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "ナックル") > 0
                || Strings.InStr(wname, "ブロー") > 0
                || Strings.InStr(wname, "チョップ") > 0
                || Strings.InStr(wname, "ビンタ") > 0
                || Strings.InStr(wname, "殴") > 0
                || Strings.Right(wname, 1) == "手"
                || Strings.Right(wname, 1) == "腕"
                || Strings.InStr(wname, "格闘") > 0
                || Strings.InStr(wname, "トンファー") > 0
                || Strings.InStr(wname, "棒") > 0
                || Strings.InStr(wname, "杖") > 0
                || Strings.InStr(wname, "スタッフ") > 0
                || Strings.InStr(wname, "メイス") > 0
                || Strings.Right(wname, 2) == "ムチ"
                || Strings.InStr(wname, "鞭") > 0
                || Strings.InStr(wname, "ウィップ") > 0
                || Strings.InStr(wname, "チェーン") > 0
                || Strings.InStr(wname, "ロッド") > 0
                || Strings.InStr(wname, "モーニングスター") > 0
                || Strings.InStr(wname, "フレイル") > 0
                || Strings.InStr(wname, "ヌンチャク") > 0
                || Strings.InStr(wname, "三節根") > 0
                || Strings.InStr(wname, "チェーン") > 0 && Strings.InStr(wname, "チェーンソー") == 0
                || Strings.InStr(wname, "バット") > 0
                || Strings.InStr(wname, "ギター") > 0
                || Strings.InStr(wname, "竹刀") > 0
                || Strings.InStr(wname, "ハリセン") > 0)
            {
                if (combo_attack)
                {
                    wtype = "乱打";
                }
                else if (double_attack
                    || Strings.InStr(wname, "触手") > 0
                    || Strings.InStr(wname, "触腕") > 0)
                {
                    wtype = "連打";
                }
                else
                {
                    wtype = "打撃";
                }

                if (Strings.Right(wname, 2) == "ムチ"
                    || Strings.InStr(wname, "鞭") > 0
                    || Strings.InStr(wname, "ウィップ") > 0
                    || Strings.InStr(wname, "チェーン") > 0
                    || Strings.InStr(wname, "触手") > 0
                    || Strings.InStr(wname, "触腕") > 0
                    || Strings.InStr(wname, "ロッド") > 0 && wname != "ロッド"
                    || Strings.InStr(wname, "竹刀") > 0
                    || Strings.InStr(wname, "ハリセン") > 0)
                {
                    sname = "Whip.wav";
                }
                else if (Strings.InStr(wname, "張り手") > 0
                    || Strings.InStr(wname, "平手") > 0
                    || Strings.InStr(wname, "ビンタ") > 0)
                {
                    sname = "Slap.wav";
                }

                goto FoundWeaponType;
            }

            // 強打撃
            if (Strings.InStr(wname, "拳") > 0
                || Strings.InStr(wname, "掌") > 0
                || Strings.InStr(wname, "打") > 0
                || Strings.InStr(wname, "勁") > 0
                || Strings.InStr(wname, "ラリアート") > 0
                || Strings.InStr(wname, "キック") > 0
                || Strings.InStr(wname, "蹴") > 0
                || Strings.InStr(wname, "脚") > 0
                || Strings.Right(wname, 1) == "足"
                || Strings.InStr(wname, "ヘッドバッド") > 0
                || Strings.InStr(wname, "頭突") > 0
                || Strings.InStr(wname, "ハンマー") > 0
                || Strings.InStr(wname, "槌") > 0
                || Strings.InStr(wname, "モール") > 0)
            {
                if (combo_attack)
                {
                    wtype = "乱打";
                }
                else if (double_attack)
                {
                    wtype = "連打";
                }
                else
                {
                    wtype = "強打";
                }

                if (Strings.InStr(wname, "拳") > 0
                    || Strings.InStr(wname, "掌") > 0
                    || Strings.InStr(wname, "打") > 0
                    || Strings.InStr(wname, "勁") > 0)
                {
                    Sound.PlayWave("Bazooka.wav");
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "アッパー") > 0)
            {
                wtype = "アッパー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "タックル") > 0
                || Strings.InStr(wname, "体当") > 0
                || Strings.InStr(wname, "チャージ") > 0
                || Strings.InStr(wname, "ぶちかまし") > 0
                || Strings.InStr(wname, "バンカー") > 0)
            {
                wtype = "強打";
                sname = "Crash.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "バンカー") > 0)
            {
                wtype = "強打";
                sname = "Bazooka.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "怪力") > 0)
            {
                wtype = "超打";
                sname = "Crash.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "格闘") > 0)
            {
                wtype = "打撃";
                goto FoundWeaponType;
            }

            // 斬撃系

            if (Strings.InStr(wname, "ビーム") > 0
                || Strings.InStr(wname, "プラズマ") > 0
                || Strings.InStr(wname, "レーザー") > 0
                || Strings.InStr(wname, "ブラスター") > 0
                || Strings.InStr(wname, "ライト") > 0)
            {
                if (Strings.InStr(wname, "プラズマ") > 0)
                {
                    cname = "グリーン";
                }
                else if (Strings.InStr(wname, "レーザー") > 0)
                {
                    cname = "ブルー";
                }
                else if (Strings.InStr(wname, "ライト") > 0)
                {
                    cname = "イエロー";
                }

                if (Strings.InStr(wname, "サーベル") > 0
                    || Strings.InStr(wname, "セイバー") > 0
                    || Strings.InStr(wname, "ブレード") > 0
                    || Strings.InStr(wname, "ソード") > 0
                    || Strings.InStr(wname, "剣") > 0
                    || Strings.InStr(wname, "刀") > 0)
                {
                    if (Strings.InStr(wname, "ハイパー") > 0
                        || Strings.InStr(wname, "ロング") > 0
                        || Strings.InStr(wname, "大") > 0
                        || Strings.InStr(wname, "高") > 0)
                    {
                        wtype = "ハイパービームサーベル";
                    }
                    else
                    {
                        wtype = "ビームサーベル";
                    }

                    if (double_weapon)
                    {
                        wtype = "ダブル" + wtype;
                    }
                    else if (Strings.InStr(wname, "回転") > 0)
                    {
                        wtype = "回転" + wtype;
                    }

                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "カッター") > 0)
                {
                    if (Strings.InStr(wname, "ハイパー") > 0
                        || Strings.InStr(wname, "ロング") > 0
                        || Strings.InStr(wname, "大") > 0
                        || Strings.InStr(wname, "高") > 0)
                    {
                        wtype = "エナジーブレード";
                    }
                    else
                    {
                        wtype = "エナジーカッター";
                    }

                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "ナイフ") > 0
                    || Strings.InStr(wname, "ダガー") > 0)
                {
                    wtype = "ビームナイフ";
                    goto FoundWeaponType;
                }

                if (Strings.InStr(wname, "ナギナタ") > 0)
                {
                    wtype = "回転ビームサーベル";
                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "ソード") > 0
                || Strings.InStr(wname, "剣") > 0
                || Strings.InStr(wname, "ナイフ") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "シミター") > 0
                || Strings.InStr(wname, "サーベル") > 0
                || Strings.InStr(wname, "カットラス") > 0
                || Strings.InStr(wname, "刀") > 0
                || Strings.InStr(wname, "斬") > 0
                || Strings.InStr(wname, "ブレード") > 0
                || Strings.InStr(wname, "刃") > 0
                || Strings.InStr(wname, "アックス") > 0
                || Strings.InStr(wname, "斧") > 0
                || Strings.InStr(wname, "グレイブ") > 0
                || Strings.InStr(wname, "ナギナタ") > 0
                || Strings.InStr(wname, "切") > 0
                || Strings.InStr(wname, "裂") > 0
                || Strings.InStr(wname, "カット") > 0
                || Strings.InStr(wname, "カッター") > 0
                || Strings.InStr(wname, "スラッシュ") > 0
                || Strings.InStr(wname, "居合") > 0)
            {
                if (combo_attack)
                {
                    wtype = "斬撃乱舞";
                }
                else if (double_weapon)
                {
                    wtype = "連斬撃";
                }
                else if (double_attack)
                {
                    wtype = "ダブル斬撃";
                }
                else if (GeneralLib.InStrNotNest(wclass, "火") > 0)
                {
                    wtype = "炎斬撃";
                }
                else if (GeneralLib.InStrNotNest(wclass, "雷") > 0)
                {
                    wtype = "雷斬撃";
                }
                else if (GeneralLib.InStrNotNest(wclass, "冷") > 0)
                {
                    wtype = "凍斬撃";
                }
                else if (Strings.InStr(wname, "唐竹割") > 0
                    || Strings.InStr(wname, "縦") > 0)
                {
                    wtype = "唐竹割";
                }
                else if (Strings.InStr(wname, "居合") > 0
                    || Strings.InStr(wname, "横") > 0)
                {
                    wtype = "なぎ払い";
                }
                else if (Strings.InStr(wname, "斬") > 0)
                {
                    wtype = "大斬撃";
                }
                else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                {
                    wtype = "斬り上げ";
                }
                else if (Strings.InStr(wname, "黒") > 0
                    || Strings.InStr(wname, "闇") > 0
                    || Strings.InStr(wname, "死") > 0
                    || Strings.InStr(wname, "ダーク") > 0
                    || Strings.InStr(wname, "デス") > 0)
                {
                    wtype = "黒斬撃";
                }
                else
                {
                    wtype = "斬撃";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ショーテル") > 0)
            {
                wtype = "ダブル斬撃";
                goto FoundWeaponType;
            }

            // 刺突系

            if (Strings.InStr(wname, "スピア") > 0
                || Strings.InStr(wname, "槍") > 0
                || Strings.InStr(wname, "ランス") > 0
                || Strings.InStr(wname, "ランサー") > 0
                || Strings.InStr(wname, "トライデント") > 0
                || Strings.InStr(wname, "ジャベリン") > 0
                || Strings.InStr(wname, "レイピア") > 0
                || wname == "ロッド")
            {
                if (combo_attack)
                {
                    wtype = "乱突";
                }
                else if (double_attack)
                {
                    wtype = "連突";
                }
                else
                {
                    wtype = "刺突";
                }

                goto FoundWeaponType;
            }

            // その他格闘系

            if (Strings.InStr(wname, "爪") > 0
                || Strings.InStr(wname, "クロー") > 0
                || Strings.InStr(wname, "ひっかき") > 0)
            {
                if (Strings.InStr(wname, "アーム") > 0)
                {
                    wtype = "打撃";
                    sname = "Crash.wav";
                }
                else
                {
                    wtype = "爪撃";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "噛") > 0
                || Strings.InStr(wname, "牙") > 0
                || Strings.InStr(wname, "かみつき") > 0)
            {
                wtype = "噛み付き";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ドリル") > 0)
            {
                wtype = "ドリル";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "リボン") > 0)
            {
                wtype = "リボン";
                goto FoundWeaponType;
            }

            // 掴み系

            if (Strings.InStr(wname, "スープレックス") > 0
                || Strings.InStr(wname, "投げ") > 0
                || wname == "返し")
            {
                wtype = "投げ飛ばし";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ヒールホールド") > 0)
            {
                wtype = "足固め";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ブリーカー") > 0)
            {
                wtype = "背負い固め";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "固め") > 0
                || Strings.InStr(wname, "ホールド") > 0
                || Strings.InStr(wname, "ツイスト") > 0
                || Strings.InStr(wname, "絞め") > 0
                || Strings.InStr(wname, "締め") > 0
                || Strings.InStr(wname, "折り") > 0)
            {
                wtype = "立ち固め";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ジャイアントスイング") > 0)
            {
                wtype = "ジャイアントスイング";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "地獄車") > 0)
            {
                wtype = "地獄車";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ブレーンバスター") > 0)
            {
                wtype = "ブレーンバスター";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "スクリューバックドライバー") > 0)
            {
                wtype = "スクリューバックドライバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "スクリュードライバー") > 0)
            {
                wtype = "スクリュードライバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "バックドライバー") > 0)
            {
                wtype = "バックドライバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ドライバー") > 0)
            {
                wtype = "ドライバー";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "踏み") > 0
                || Strings.InStr(wname, "押し") > 0)
            {
                wtype = "踏み潰し";
                goto FoundWeaponType;
            }

            // 詳細が分からなかった武器
            if (GeneralLib.InStrNotNest(wclass, "武") > 0)
            {
                // TODO Impl item
                //// 装備しているアイテムから武器を検索
                //var loopTo = u.CountItem();
                //for (i = 1; i <= loopTo; i++)
                //{
                //    {
                //        var withBlock = u.Item(i);
                //        if (withBlock.Activated
                //&& (withBlock.Part() == "両手"
                //|| withBlock.Part() == "片手"
                //|| withBlock.Part() == "武器"))
                //        {
                //            wtype = CheckWeaponType(withBlock.Nickname(), "");
                //            if (string.IsNullOrEmpty(wtype))
                //            {
                //                wtype = CheckWeaponType(withBlock.Class0(), "");
                //            }

                //            break;
                //        }
                //    }
                //}

                //switch (wtype ?? "")
                //{
                //    case "スピア":
                //    case "ランス":
                //    case "トライデント":
                //    case "和槍":
                //    case "エストック":
                //        {
                //            if (combo_attack)
                //            {
                //                wtype = "乱突";
                //            }
                //            else if (double_attack)
                //            {
                //                wtype = "連突";
                //            }
                //            else
                //            {
                //                wtype = "刺突";
                //            }

                //            break;
                //        }

                //    default:
                //        {
                //            if (combo_attack)
                //            {
                //                wtype = "斬撃乱舞";
                //            }
                //            else if (double_weapon)
                //            {
                //                wtype = "ダブル斬撃";
                //            }
                //            else if (double_attack)
                //            {
                //                wtype = "連斬撃";
                //            }
                //            else if (GeneralLib.InStrNotNest(wclass, "火") > 0)
                //            {
                //                wtype = "炎斬撃";
                //            }
                //            else if (GeneralLib.InStrNotNest(wclass, "雷") > 0)
                //            {
                //                wtype = "雷斬撃";
                //            }
                //            else if (GeneralLib.InStrNotNest(wclass, "冷") > 0)
                //            {
                //                wtype = "凍斬撃";
                //            }
                //            else if (GeneralLib.InStrNotNest(wclass, "Ｊ") > 0)
                //            {
                //                wtype = "斬り上げ";
                //            }
                //            else
                //            {
                //                wtype = "斬撃";
                //            }

                //            break;
                //        }
                //}

                goto FoundWeaponType;
            }

            // 詳細が分からなかった近接技
            if (GeneralLib.InStrNotNest(wclass, "突") > 0
                && GeneralLib.InStrNotNest(wclass, "接") > 0)
            {
                if (combo_attack)
                {
                    wtype = "乱打";
                }
                else if (double_attack)
                {
                    wtype = "連打";
                }
                else
                {
                    wtype = "打撃";
                }

                goto FoundWeaponType;
            }

        SkipInfightWeapon:
            ;


            // 射撃武器(格闘投擲)

            if (Strings.InStr(wname, "斧") > 0
                || Strings.InStr(wname, "アックス") > 0
                || Strings.InStr(wname, "トマホーク") > 0
                || Strings.InStr(wname, "ソーサー") > 0
                || Strings.InStr(wname, "チャクラム") > 0)
            {
                wtype = "ダメージ";
                sname = "Saber.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "パンチ") > 0
                || Strings.InStr(wname, "ハンマー") > 0
                || Strings.InStr(wname, "岩") > 0
                || Strings.InStr(wname, "鉄球") > 0)
            {
                wtype = "強打";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "石") > 0
                || Strings.InStr(wname, "礫") > 0
                || Strings.InStr(wname, "分銅") > 0
                || Strings.InStr(wname, "ブーメラン") > 0)
            {
                wtype = "打撃";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ナイフ") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "手裏剣") > 0
                || Strings.InStr(wname, "クナイ") > 0
                || Strings.InStr(wname, "苦無") > 0)
            {
                wtype = "刺突";
                goto FoundWeaponType;
            }

            // これより通常射撃攻撃

            // まずは光線系の攻撃かどうかを判定する

            if (IsBeamWeapon(wname, wclass, cname))
            {
                wtype = "ビーム";
            }

            if (wtype == "ビーム")
            {
                // 実弾系武器判定をスキップ
                goto SkipNormalWeapon;
            }

            // 射撃武器(実弾系)

            if (Strings.InStr(wname, "弓") > 0
                || Strings.InStr(wname, "ショートボウ") > 0
                || Strings.InStr(wname, "ロングボウ") > 0
                || Strings.InStr(wname, "ボウガン") > 0
                || Strings.InStr(wname, "矢") > 0
                || Strings.InStr(wname, "アロー") > 0)
            {
                wtype = "矢";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "バルカン") > 0)
            {
                wtype = "バルカン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ガトリング") > 0
                || Strings.InStr(wname, "チェーンガン") > 0
                || Strings.InStr(wname, "ガンランチャー") > 0)
            {
                wtype = "ガトリング";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "機関銃") > 0)
            {
                if (Strings.InStr(wname, "ヘビー") > 0
                    || Strings.InStr(wname, "重") > 0)
                {
                    wtype = "ヘビーマシンガン";
                }
                else
                {
                    wtype = "マシンガン";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "機銃") > 0
                || Strings.InStr(wname, "機関砲") > 0)
            {
                wtype = "マシンガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "マシンキャノン") > 0
                || Strings.InStr(wname, "オートキャノン") > 0
                || Strings.InStr(wname, "速射砲") > 0)
            {
                wtype = "ヘビーマシンガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ショットガン") > 0
                || Strings.InStr(wname, "散弾") > 0
                || Strings.InStr(wname, "拡散バズーカ") > 0)
            {
                wtype = "ショットガン";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ベアリング") > 0
                || Strings.InStr(wname, "クレイモア") > 0)
            {
                wtype = "ベアリング";
                goto FoundWeaponType;
            }

        SkipNormalWeapon:
            ;


            // 射撃武器(エネルギー系)

            if (Strings.InStr(wname, "怪光線") > 0)
            {
                wtype = "怪光線";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "破壊光線") > 0)
            {
                wtype = "破壊光線";
                goto FoundWeaponType;
            }

            if (wtype == "ビーム")
            {
                if (Strings.InStr(CurrentWeaponType, "ビーム") > 0
                    || Strings.InStr(CurrentWeaponType, "レーザー") > 0)
                {
                    // 可能であれば発射時のエフェクトと統一する
                    switch (CurrentWeaponType ?? "")
                    {
                        case "ビームライフル":
                            {
                                wtype = "小ビーム";
                                break;
                            }

                        case "ダブルビームライフル":
                            {
                                wtype = "２連小ビーム";
                                break;
                            }

                        case "ビームランチャー":
                            {
                                wtype = "中ビーム";
                                break;
                            }

                        case "ダブルビームランチャー":
                            {
                                wtype = "２連中ビーム";
                                break;
                            }

                        case "バスタービームライフル":
                            {
                                wtype = "大ビーム";
                                break;
                            }

                        case "レーザーガン":
                            {
                                wtype = "ニードルレーザー";
                                break;
                            }

                        case "レーザーマシンガン":
                            {
                                wtype = "ニードルレーザー連射";
                                break;
                            }

                        default:
                            {
                                wtype = CurrentWeaponType;
                                break;
                            }
                    }
                }
                else
                {
                    if (Strings.InStr(wname, "ハイメガ") > 0
                        || Strings.InStr(wname, "バスター") > 0
                        || Strings.InStr(wname, "大") > 0
                        || Strings.Left(wname, 2) == "ギガ")
                    {
                        wtype = "大ビーム";
                    }
                    else if (Strings.InStr(wname, "メガ") > 0
                        || Strings.InStr(wname, "ハイ") > 0
                        || Strings.InStr(wname, "バズーカ") > 0)
                    {
                        wtype = "中ビーム";
                    }
                    else if (CountAttack0(u, w) >= 4
                        || Strings.InStr(wname, "対空") > 0)
                    {
                        wtype = "ニードルレーザー連射";
                    }
                    else if (Strings.InStr(wname, "ピストル") > 0
                        || Strings.InStr(wname, "ミニ") > 0
                        || Strings.InStr(wname, "小") > 0)
                    {
                        wtype = "ニードルレーザー";
                    }
                    else if (Strings.InStr(wname, "ランチャー") > 0
                        || Strings.InStr(wname, "キャノン") > 0
                        || Strings.InStr(wname, "カノン") > 0
                        || Strings.InStr(wname, "砲") > 0)
                    {
                        wtype = "中ビーム";
                    }
                    else
                    {
                        wtype = "小ビーム";
                    }

                    switch (wtype ?? "")
                    {
                        case "小ビーム":
                        case "中ビーム":
                            {
                                if (double_weapon)
                                {
                                    wtype = "２連" + wtype;
                                }

                                break;
                            }
                    }

                    if (Strings.InStr(wname, "拡散") > 0
                        || Strings.InStr(wname, "放射") > 0)
                    {
                        wtype = "拡散ビーム";
                    }

                    if (Strings.InStr(wname, "ホーミング") > 0
                        || Strings.InStr(wname, "誘導") > 0)
                    {
                        wtype = "ホーミングレーザー";
                    }
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "光線") > 0)
            {
                wtype = "怪光線";
                goto FoundWeaponType;
            }

            // 爆発系

            if (Strings.InStr(wname, "ピストル") > 0
                || Strings.InStr(wname, "拳銃") > 0
                || Strings.InStr(wname, "リボルバー") > 0
                || Strings.InStr(wname, "リボルヴァー") > 0
                || Strings.InStr(wname, "銃") > 0
                || Strings.Right(wname, 2) == "ガン"
                || Strings.InStr(wname, "ライフル") > 0)
            {
                wtype = "銃弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "爆雷") > 0)
            {
                wtype = "爆雷";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "爆撃") > 0
                || CurrentWeaponType == "投下爆弾")
            {
                wtype = "爆撃";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ミサイル") > 0
                || Strings.InStr(wname, "ロケット") > 0
                || Strings.InStr(wname, "爆弾") > 0
                || Strings.InStr(wname, "ダイナマイト") > 0
                || Strings.InStr(wname, "榴弾") > 0
                || Strings.InStr(wname, "反応弾") > 0
                || Strings.InStr(wname, "グレネード") > 0
                || Strings.InStr(wname, "手榴弾") > 0
                || Strings.InStr(wname, "クラッカー") > 0
                || Strings.InStr(wname, "ディスチャージャー") > 0
                || Strings.InStr(wname, "マイン") > 0
                || Strings.InStr(wname, "ボム") > 0
                || Strings.InStr(wname, "魚雷") > 0
                || Strings.InStr(wname, "機雷") > 0
                || Strings.InStr(wname, "バズーカ") > 0
                || Strings.InStr(wname, "シュツルムファウスト") > 0)
            {
                if (Strings.InStr(wname, "核") > 0
                    || Strings.InStr(wname, "反応") > 0
                    || Strings.InStr(wname, "アトミック") > 0
                    || Strings.InStr(wname, "超") > 0)
                {
                    wtype = "超爆発";
                }
                else if (Strings.InStr(wname, "大") > 0
                    || Strings.InStr(wname, "ビック") > 0
                    || Strings.InStr(wname, "ジャイアント") > 0
                    || Strings.InStr(wname, "メガ") > 0)
                {
                    wtype = "大爆発";
                }
                else if (Strings.InStr(wname, "小") > 0
                    || Strings.InStr(wname, "ミニ") > 0
                    || Strings.InStr(wname, "マイクロ") > 0)
                {
                    wtype = "小爆発";
                }
                else
                {
                    wtype = "爆発";
                }

                // 連続爆発？

                if (wtype == "超爆発")
                {
                    goto FoundWeaponType;
                }

                attack_times = CountAttack0(u, w);
                if (GeneralLib.InStrNotNest(wclass, "連") > 0)
                {
                    attack_times = hit_count;
                }

                if (attack_times == 1)
                {
                    attack_times = 0;
                    goto FoundWeaponType;
                }

                if (wtype == "小爆発")
                {
                    wtype = "連続爆発";
                }
                else
                {
                    wtype = "連続" + wtype;
                }

                goto FoundWeaponType;
            }

            // その他特殊系

            if (Strings.InStr(wname, "電撃") > 0
                || Strings.InStr(wname, "電流") > 0
                || Strings.InStr(wname, "エレクト") > 0)
            {
                wtype = "破壊光線";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "雷") > 0
                || Strings.InStr(wname, "ライトニング") > 0
                || Strings.InStr(wname, "サンダー") > 0
                || Strings.Right(wname, 2) == "稲妻"
                || GeneralLib.InStrNotNest(wclass, "電") > 0)
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0)
                {
                    wtype = "放電";
                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "吹雪") > 0
                || Strings.InStr(wname, "ブリザード") > 0
                || Strings.InStr(wname, "アイスストーム") > 0)
            {
                wtype = "吹雪";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ストーム") > 0
                || Strings.InStr(wname, "ハリケーン") > 0
                || Strings.InStr(wname, "タイフーン") > 0
                || Strings.InStr(wname, "台風") > 0
                || Strings.InStr(wname, "嵐") > 0)
            {
                wtype = "強風";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "ウィンド") > 0
                || Strings.InStr(wname, "ウインド") > 0
                || Strings.InStr(wname, "風") > 0)
            {
                wtype = "風";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "トルネード") > 0
                || Strings.InStr(wname, "サイクロン") > 0
                || Strings.InStr(wname, "竜巻") > 0
                || Strings.InStr(wname, "渦巻") > 0)
            {
                wtype = "竜巻";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "泡") > 0
                || Strings.InStr(wname, "バブル") > 0
                || Strings.InStr(wname, "消火") > 0)
            {
                wtype = "泡";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "重力") > 0
                || Strings.InStr(wname, "グラビ") > 0
                || Strings.InStr(wname, "ブラックホール") > 0
                || Strings.InStr(wname, "縮退") > 0)
            {
                wtype = "重力圧縮";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "スロウ") > 0)
            {
                wtype = "時間逆行";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "煙") > 0
                || Strings.InStr(wname, "スモーク") > 0
                || Strings.Right(wname, 2) == "ガス"
                || Strings.Right(wname, 1) == "霧"
                || Strings.InStr(wname, "胞子") > 0)
            {
                wtype = "煙";
                if (Strings.InStr(wname, "毒") > 0
                    || GeneralLib.InStrNotNest(wclass, "毒") > 0)
                {
                    cname = "緑";
                }

                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "火炎弾") > 0)
            {
                wtype = "火炎弾";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "火炎放射") > 0
                || Strings.Right(wname, 2) == "火炎")
            {
                wtype = "火炎放射";
                goto FoundWeaponType;
            }

            if (Strings.Right(wname, 5) == "ファイアー"
                || Strings.Right(wname, 5) == "ファイヤー"
                || Strings.Right(wname, 4) == "ファイア"
                || Strings.Right(wname, 4) == "ファイヤ")
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0
                    && Strings.Left(wname, 2) != "フル")
                {
                    if (GeneralLib.InStrNotNest(wclass, "術") > 0)
                    {
                        wtype = "炎";
                    }
                    else
                    {
                        wtype = "火炎放射";
                    }

                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "息") > 0
                || Strings.Right(wname, 3) == "ブレス")
            {
                if (GeneralLib.InStrNotNest(wclass, "実") == 0)
                {
                    wtype = "火炎放射";
                    switch (SpellColor(wname, wclass) ?? "")
                    {
                        case "青":
                        case "黄":
                        case "緑":
                        case "白":
                        case "黒":
                            {
                                cname = SpellColor(wname, wclass);
                                sname = "Breath.wav";
                                break;
                            }
                    }

                    goto FoundWeaponType;
                }
            }

            if (Strings.InStr(wname, "火") > 0
                || Strings.InStr(wname, "炎") > 0
                || Strings.InStr(wname, "焔") > 0
                || Strings.InStr(wname, "ファイヤー") > 0)
            {
                wtype = "炎";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "水鉄砲") > 0
                || Strings.InStr(wname, "放水") > 0
                || Strings.InStr(wname, "水流") > 0
                || Strings.InStr(wname, "酸かけ") > 0
                || Strings.Right(wname, 1) == "液"
                || Strings.Right(wname, 1) == "酸")
            {
                wtype = "飛沫";
                if (Strings.InStr(wname, "毒") > 0
                    || Strings.InStr(wname, "毒") > 0)
                {
                    cname = "緑";
                }
                else if (Strings.InStr(wname, "酸") > 0)
                {
                    cname = "白";
                }
                else
                {
                    cname = "青";
                }

                sname = "Splash.wav";
                goto FoundWeaponType;
            }

            if (Strings.InStr(wname, "吸収") > 0
                || Strings.InStr(wname, "ドレイン") > 0
                || GeneralLib.InStrNotNest(wclass, "吸") > 0
                || GeneralLib.InStrNotNest(wclass, "減") > 0)
            {
                wtype = "吸収";
                goto FoundWeaponType;
            }

            // 攻撃力0の攻撃の場合は「ダメージ」のアニメを使用しない
            if (w.WeaponPower("") == 0)
            {
                return;
            }

            // デフォルト
            wtype = "ダメージ";
        FoundWeaponType:
            ;


            // アニメの不整合を防ぐため、吹き飛ばし時はアニメ効果を打撃に抑えておく
            switch (wtype ?? "")
            {
                case "強打":
                case "超打":
                    {
                        if (GeneralLib.InStrNotNest(wclass, "吹") > 0
                            || GeneralLib.InStrNotNest(wclass, "Ｋ") > 0)
                        {
                            wtype = "打撃";
                        }

                        break;
                    }
            }

            // 表示色を最終決定
            if (Strings.InStr(wname, "レッド") > 0
                || Strings.InStr(wname, "赤") > 0)
            {
                cname = "赤";
            }
            else if (Strings.InStr(wname, "ブルー") > 0
                || Strings.InStr(wname, "青") > 0)
            {
                cname = "青";
            }
            else if (Strings.InStr(wname, "イエロー") > 0
                || Strings.InStr(wname, "黄") > 0)
            {
                cname = "黄";
            }
            else if (Strings.InStr(wname, "グリーン") > 0
                || Strings.InStr(wname, "緑") > 0)
            {
                cname = "緑";
            }
            else if (Strings.InStr(wname, "ピンク") > 0
                || Strings.InStr(wname, "桃") > 0)
            {
                cname = "桃";
            }
            else if (Strings.InStr(wname, "ブラウン") > 0
                || Strings.InStr(wname, "橙") > 0)
            {
                cname = "橙";
            }
            else if (Strings.InStr(wname, "ブラック") > 0
                || Strings.InStr(wname, "黒") > 0
                || Strings.InStr(wname, "ダーク") > 0
                || Strings.InStr(wname, "闇") > 0)
            {
                cname = "黒";
            }
            else if (Strings.InStr(wname, "ホワイト") > 0
                || Strings.InStr(wname, "白") > 0
                || Strings.InStr(wname, "ホーリー") > 0
                || Strings.InStr(wname, "聖") > 0)
            {
                cname = "白";
            }

            // ２種類のアニメを組み合わせる場合
            if (Strings.Len(wtype0) > 0)
            {
                // 表示する命中アニメの種類
                aname = wtype0 + "命中";

                // 色
                if (Strings.Len(cname) > 0)
                {
                    aname = aname + " " + cname;
                }

                // 命中アニメ表示
                ShowAnimation(aname);
            }

            // 表示する命中アニメの種類
            aname = wtype + "命中";

            // 色
            if (Strings.Len(cname) > 0)
            {
                aname = aname + " " + cname;
            }

            // 効果音
            if (Strings.Len(sname) > 0)
            {
                aname = aname + " " + sname;
            }

            // 命中数
            if (attack_times > 0)
            {
                aname = aname + " " + SrcFormatter.Format(attack_times);
            }

            // 命中アニメ表示
            ShowAnimation(aname);
        }

        // 武器命中時の効果音
        public void HitSound(Unit u, UnitWeapon w, Unit t, int hit_count)
        {
            string wname, wclass;
            int num, i;

            // 右クリック中は効果音をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 効果音の再生回数
            num = CountAttack(u, w);

            // 武器に応じて効果音を再生
            if (GeneralLib.InStrNotNest(wclass, "武") > 0
                || GeneralLib.InStrNotNest(wclass, "突") > 0
                || GeneralLib.InStrNotNest(wclass, "接") > 0
                || GeneralLib.InStrNotNest(wclass, "実") > 0)
            {
                // 無音
                if (Strings.InStr(wname, "ディスカッター") > 0
                    || Strings.InStr(wname, "リッパー") > 0
                    || Strings.InStr(wname, "スパイド") > 0
                    || Strings.InStr(wname, "居合") > 0
                    || Strings.InStr(wname, "閃") > 0)
                {
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(200);
                    Sound.PlayWave("Sword.wav");
                    var loopTo = num;
                    for (i = 2; i <= loopTo; i++)
                    {
                        GUI.Sleep(200);
                        Sound.PlayWave("Sword.wav");
                    }
                }
                else if (Strings.InStr(wname, "プログレッシブナイフ") > 0
                    || Strings.InStr(wname, "ドリル") > 0)
                {
                    Sound.PlayWave("Drill.wav");
                }
                else if (Strings.InStr(wname, "サーベル") > 0
                    || Strings.InStr(wname, "セイバー") > 0
                    || Strings.InStr(wname, "ソード") > 0
                    || Strings.InStr(wname, "ブレード") > 0
                    || Strings.InStr(wname, "スパッド") > 0
                    || Strings.InStr(wname, "セーバー") > 0
                    || Strings.InStr(wname, "ダガー") > 0
                    || Strings.InStr(wname, "ナイフ") > 0
                    || Strings.InStr(wname, "トマホーク") > 0
                    || Strings.InStr(wname, "メイス") > 0
                    || Strings.InStr(wname, "アックス") > 0
                    || Strings.InStr(wname, "グレイブ") > 0
                    || Strings.InStr(wname, "ナギナタ") > 0
                    || Strings.InStr(wname, "ビアンキ") > 0
                    || Strings.InStr(wname, "ウェッブ") > 0
                    || Strings.InStr(wname, "ザンバー") > 0
                    || Strings.InStr(wname, "マーカー") > 0
                    || Strings.InStr(wname, "バスター") > 0
                    || Strings.InStr(wname, "ブラスター") > 0
                    || Strings.InStr(wname, "クロー") > 0
                    || Strings.InStr(wname, "ジザース") > 0
                    || Strings.InStr(wname, "ブーメラン") > 0
                    || Strings.InStr(wname, "ソーサー") > 0
                    || Strings.InStr(wname, "レザー") > 0
                    || Strings.InStr(wname, "レイバー") > 0
                    || Strings.InStr(wname, "サイズ") > 0
                    || Strings.InStr(wname, "ショーテル") > 0
                    || Strings.InStr(wname, "カッター") > 0
                    || Strings.InStr(wname, "スパイク") > 0
                    || Strings.InStr(wname, "カトラス") > 0
                    || Strings.InStr(wname, "エッジ") > 0
                    || Strings.InStr(wname, "剣") > 0
                    && Strings.InStr(wname, "手裏剣") == 0
                    || Strings.InStr(wname, "切") > 0
                    || Strings.InStr(wname, "斬") > 0
                    || Strings.InStr(wname, "刀") > 0
                    || Strings.InStr(wname, "刃") > 0
                    || Strings.InStr(wname, "斧") > 0
                    || Strings.InStr(wname, "鎌") > 0
                    || Strings.InStr(wname, "かま") > 0
                    || Strings.InStr(wname, "カマ") > 0
                    || Strings.InStr(wname, "爪") > 0
                    || Strings.InStr(wname, "かぎづめ") > 0
                    || Strings.InStr(wname, "ハサミ") > 0
                    || Strings.InStr(wname, "バサミ") > 0
                    || Strings.InStr(wname, "羽") > 0)
                {
                    if (!t.IsHero()
                        || Strings.InStr(wname, "ビーム") > 0
                        || Strings.InStr(wname, "プラズマ") > 0
                        || Strings.InStr(wname, "レーザー") > 0
                        || Strings.InStr(wname, "セイバー") > 0)
                    {
                        Sound.PlayWave("Saber.wav");
                        var loopTo2 = num;
                        for (i = 2; i <= loopTo2; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Saber.wav");
                        }
                    }
                    else
                    {
                        Sound.PlayWave("Swing.wav");
                        GUI.Sleep(190);
                        Sound.PlayWave("Slash.wav");
                        var loopTo3 = num;
                        for (i = 2; i <= loopTo3; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Slash.wav");
                        }
                    }
                }
                else if (Strings.InStr(wname, "ランサー") > 0
                    || Strings.InStr(wname, "ランス") > 0
                    || Strings.InStr(wname, "スピア") > 0
                    || Strings.InStr(wname, "トライデント") > 0
                    || Strings.InStr(wname, "ハーケン") > 0
                    || Strings.InStr(wname, "槍") > 0
                    || Strings.InStr(wname, "もり") > 0
                    || Strings.InStr(wname, "手裏剣") > 0
                    || Strings.InStr(wname, "苦無") > 0
                    || Strings.InStr(wname, "クナイ") > 0
                    || Strings.InStr(wname, "突き") > 0 && Strings.InStr(wname, "拳") == 0 && Strings.InStr(wname, "頭") == 0)
                {
                    if (!t.IsHero()
                        || Strings.InStr(wname, "ビーム") > 0
                        || Strings.InStr(wname, "プラズマ") > 0
                        || Strings.InStr(wname, "レーザー") > 0
                        || Strings.InStr(wname, "ランサー") > 0)
                    {
                        Sound.PlayWave("Saber.wav");
                        var loopTo4 = num;
                        for (i = 2; i <= loopTo4; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Saber.wav");
                        }
                    }
                    else
                    {
                        Sound.PlayWave("Swing.wav");
                        GUI.Sleep(190);
                        Sound.PlayWave("Stab.wav");
                        var loopTo5 = num;
                        for (i = 2; i <= loopTo5; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Stab.wav");
                        }
                    }
                }
                else if (Strings.InStr(wname, "牙") > 0
                    || Strings.InStr(wname, "ファング") > 0
                    || Strings.InStr(wname, "噛") > 0
                    || Strings.InStr(wname, "かみつき") > 0
                    || Strings.InStr(wname, "顎") > 0)
                {
                    if (!t.IsHero())
                    {
                        Sound.PlayWave("Saber.wav");
                        var loopTo6 = num;
                        for (i = 2; i <= loopTo6; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Saber.wav");
                        }
                    }
                    else
                    {
                        Sound.PlayWave("Stab.wav");
                        var loopTo7 = num;
                        for (i = 2; i <= loopTo7; i++)
                        {
                            GUI.Sleep(350);
                            Sound.PlayWave("Stab.wav");
                        }
                    }
                }
                else if (Strings.InStr(wname, "ストライク") > 0
                    || Strings.InStr(wname, "アーツ") > 0
                    || Strings.InStr(wname, "拳法") > 0
                    || Strings.InStr(wname, "振動拳") > 0)
                {
                    Sound.PlayWave("Combo.wav");
                }
                else if (Strings.InStr(wname, "格闘") > 0
                    || Strings.InStr(wname, "パンチ") > 0
                    || Strings.InStr(wname, "キック") > 0
                    || Strings.InStr(wname, "チョップ") > 0
                    || Strings.InStr(wname, "ナックル") > 0
                    || Strings.InStr(wname, "ブロー") > 0
                    || Strings.InStr(wname, "ハンマー") > 0
                    || Strings.InStr(wname, "トンファー") > 0
                    || Strings.InStr(wname, "ヌンチャク") > 0
                    || Strings.InStr(wname, "パイプ") > 0
                    || Strings.InStr(wname, "ラリアット") > 0
                    || Strings.InStr(wname, "アーム") > 0
                    || Strings.InStr(wname, "ヘッドバット") > 0
                    || Strings.InStr(wname, "スリング") > 0
                    || Strings.InStr(wname, "頭突き") > 0
                    || Strings.InStr(wname, "脚") > 0
                    || Strings.InStr(wname, "蹴") > 0
                    || Strings.InStr(wname, "棒") > 0
                    || Strings.InStr(wname, "石") > 0
                    || Strings.InStr(wname, "角") > 0
                    || Strings.InStr(wname, "尻尾") > 0
                    || Strings.InStr(wname, "鉄腕") > 0)
                {
                    Sound.PlayWave("Punch.wav");
                    var loopTo8 = num;
                    for (i = 2; i <= loopTo8; i++)
                    {
                        GUI.Sleep(120);
                        Sound.PlayWave("Punch.wav");
                    }
                }
                else if (Strings.InStr(wname, "体当たり") > 0
                    || Strings.InStr(wname, "タックル") > 0
                    || Strings.InStr(wname, "ぶちかまし") > 0
                    || Strings.InStr(wname, "突進") > 0
                    || Strings.InStr(wname, "突撃") > 0
                    || Strings.InStr(wname, "怪力") > 0
                    || Strings.InStr(wname, "鉄拳") > 0
                    || Strings.InStr(wname, "メガトンパンチ") > 0
                    || Strings.InStr(wname, "鉄球") > 0
                    || Strings.InStr(wname, "ボール") > 0
                    || Strings.InStr(wname, "車輪") > 0
                    || Strings.InStr(wname, "キャタピラ") > 0
                    || Strings.InStr(wname, "シールド") > 0)
                {
                    Sound.PlayWave("Crash.wav");
                }
                else if (Strings.InStr(wname, "拳") > 0
                    || Strings.InStr(wname, "掌") > 0
                    || Strings.InStr(wname, "打") > 0
                    || Strings.InStr(wname, "勁") > 0)
                {
                    Sound.PlayWave("Bazooka.wav");
                    var loopTo9 = num;
                    for (i = 2; i <= loopTo9; i++)
                    {
                        GUI.Sleep(120);
                        Sound.PlayWave("Bazooka.wav");
                    }
                }
                else if (Strings.InStr(wname, "踏み") > 0
                    || Strings.InStr(wname, "押し") > 0
                    || Strings.InStr(wname, "ドロップ") > 0)
                {
                    Sound.PlayWave("Shock(Low).wav");
                }
                else if (Strings.InStr(wname, "張り手") > 0
                    || Strings.InStr(wname, "ビンタ") > 0)
                {
                    Sound.PlayWave("Slap.wav");
                    var loopTo10 = num;
                    for (i = 2; i <= loopTo10; i++)
                    {
                        GUI.Sleep(120);
                        Sound.PlayWave("Slap.wav");
                    }
                }
                else if (Strings.InStr(wname, "弓") > 0
                    || Strings.InStr(wname, "矢") > 0
                    || Strings.InStr(wname, "アロー") > 0
                    || Strings.InStr(wname, "ボーガン") > 0
                    || Strings.InStr(wname, "ボウガン") > 0
                    || Strings.InStr(wname, "ショートボウ") > 0
                    || Strings.InStr(wname, "ロングボウ") > 0
                    || Strings.InStr(wname, "針") > 0
                    || Strings.InStr(wname, "ニードル") > 0)
                {
                    Sound.PlayWave("Stab.wav");
                    var loopTo11 = num;
                    for (i = 2; i <= loopTo11; i++)
                    {
                        GUI.Sleep(120);
                        Sound.PlayWave("Stab.wav");
                    }
                }
                else if (Strings.InStr(wname, "鞭") > 0
                    || Strings.InStr(wname, "ムチ") > 0
                    || Strings.InStr(wname, "ウイップ") > 0
                    || Strings.InStr(wname, "チェーン") > 0
                    || Strings.InStr(wname, "ロッド") > 0
                    || Strings.InStr(wname, "テンタク") > 0
                    || Strings.InStr(wname, "テイル") > 0
                    || Strings.InStr(wname, "尾") > 0
                    || Strings.InStr(wname, "触手") > 0
                    || Strings.InStr(wname, "触腕") > 0
                    || Strings.InStr(wname, "舌") > 0
                    || Strings.InStr(wname, "巻き") > 0
                    || Strings.InStr(wname, "糸") > 0)
                {
                    Sound.PlayWave("Whip.wav");
                }
                else if (Strings.InStr(wname, "投げ") > 0
                    || Strings.InStr(wname, "スープレック") > 0
                    || Strings.InStr(wname, "返し") > 0 && GeneralLib.InStrNotNest(wclass, "突") > 0)
                {
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(500);
                    Sound.PlayWave("Shock(Low).wav");
                    var loopTo12 = num;
                    for (i = 2; i <= loopTo12; i++)
                    {
                        GUI.Sleep(700);
                        Sound.PlayWave("Swing.wav");
                        GUI.Sleep(500);
                        Sound.PlayWave("Shock(Low).wav");
                    }
                }
                else if (Strings.InStr(wname, "大雪山おろし") > 0)
                {
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(700);
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(500);
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(300);
                    Sound.PlayWave("Shock(Low).wav");
                }
                else if (Strings.InStr(wname, "関節") > 0
                    || Strings.InStr(wname, "固め") > 0
                    || Strings.InStr(wname, "折り") > 0
                    || Strings.InStr(wname, "締め") > 0
                    || Strings.InStr(wname, "絞め") > 0
                    || Strings.InStr(wname, "アームロック") > 0
                    || Strings.InStr(wname, "ホールド") > 0)
                {
                    Sound.PlayWave("Swing.wav");
                    GUI.Sleep(190);
                    Sound.PlayWave("BreakOff.wav");
                }
                else if (GeneralLib.InStrNotNest(wclass, "核") > 0
                    || Strings.InStr(wname, "核") > 0
                    || Strings.InStr(wname, "反応弾") > 0)
                {
                    Sound.PlayWave("Explode(Nuclear).wav");
                }
                else if (Strings.InStr(wname, "ミサイル") > 0
                    || Strings.InStr(wname, "ロケット") > 0
                    || Strings.InStr(wname, "魚雷") > 0
                    || Strings.InStr(wname, "マルチポッド") > 0
                    || Strings.InStr(wname, "マルチランチャー") > 0
                    || Strings.InStr(wname, "爆弾") > 0
                    || Strings.InStr(wname, "爆雷") > 0
                    || Strings.InStr(wname, "爆撃") > 0
                    || Strings.Right(wname, 3) == "マイン"
                    || Strings.Right(wname, 2) == "ボム")
                {
                    Sound.PlayWave("Explode(Small).wav");
                    var loopTo13 = num;
                    for (i = 2; i <= loopTo13; i++)
                    {
                        GUI.Sleep(130);
                        Sound.PlayWave("Explode(Small).wav");
                    }
                }
                else if (Strings.InStr(wname, "アンカー") > 0)
                {
                }
                else if (GeneralLib.InStrNotNest(wclass, "武") > 0)
                {
                    // なんか分からんけど武器
                    Sound.PlayWave("Saber.wav");
                    var loopTo14 = num;
                    for (i = 2; i <= loopTo14; i++)
                    {
                        GUI.Sleep(350);
                        Sound.PlayWave("Saber.wav");
                    }
                }
                else if (GeneralLib.InStrNotNest(wclass, "突") > 0)
                {
                    // なんか分からんけど突進技
                    Sound.PlayWave("Punch.wav");
                    var loopTo15 = num;
                    for (i = 2; i <= loopTo15; i++)
                    {
                        GUI.Sleep(120);
                        Sound.PlayWave("Punch.wav");
                    }
                }
                else if (!t.IsHero())
                {
                    Sound.PlayWave("Explode(Small).wav");
                    var loopTo1 = num;
                    for (i = 2; i <= loopTo1; i++)
                    {
                        GUI.Sleep(130);
                        Sound.PlayWave("Explode(Small).wav");
                    }
                }
            }
            else
            {
                if (Strings.InStr(wname, "ストーム") > 0
                    || Strings.InStr(wname, "トルネード") > 0
                    || Strings.InStr(wname, "ハリケーン") > 0
                    || Strings.InStr(wname, "タイフーン") > 0
                    || Strings.InStr(wname, "サイクロン") > 0
                    || Strings.InStr(wname, "ブリザード") > 0
                    || Strings.InStr(wname, "竜巻") > 0
                    || Strings.InStr(wname, "渦巻") > 0
                    || Strings.InStr(wname, "台風") > 0
                    || Strings.InStr(wname, "嵐") > 0)
                {
                }
                // 命中時は無音
                else if (Strings.Right(wname, 1) == "液")
                {
                    Sound.PlayWave("Inori.wav");
                }
                else if (Strings.InStr(wname, "発火") > 0
                    || Strings.InStr(wname, "パイロキネシス") > 0)
                {
                    Sound.PlayWave("Fire.wav");
                }
                else if (wname == "テレキネシス")
                {
                    Sound.PlayWave("Crash.wav");
                }
                else if (Strings.InStr(wname, "吸収") > 0)
                {
                    Sound.PlayWave("Charge.wav");
                }
                else if (GeneralLib.InStrNotNest(wclass, "核") > 0)
                {
                    Sound.PlayWave("Explode(Nuclear).wav");
                }
                else if (!t.IsHero())
                {
                    Sound.PlayWave("Explode(Small).wav");
                    var loopTo16 = num;
                    for (i = 2; i <= loopTo16; i++)
                    {
                        GUI.Sleep(130);
                        Sound.PlayWave("Explode(Small).wav");
                    }
                }
            }

            // フラグをクリア
            Sound.IsWavePlayed = false;
        }

        // 回避時の効果音
        public void DodgeEffect(Unit u, UnitWeapon w)
        {
            string wname, wclass;
            string sname;
            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 特殊効果が指定されていればそれを使用
            if (u.IsSpecialEffectDefined(wname + "(回避)", sub_situation: ""))
            {
                u.SpecialEffect(wname + "(回避)", sub_situation: "");
                return;
            }

            if (SRC.BattleAnimation)
            {
                return;
            }

            // 攻撃時の効果音が風切り音のみであれば風切り音は不要
            sname = u.SpecialEffectData(wname, sub_situation: "");
            if (Strings.InStr(sname, ";") > 0)
            {
                sname = Strings.Mid(sname, Strings.InStr(sname, ";"));
            }

            if (sname == "Swing.wav")
            {
                return;
            }

            // 風切り音が必要かどうか判定
            if (GeneralLib.InStrNotNest(wclass, "武") > 0
                || GeneralLib.InStrNotNest(wclass, "突") > 0
                || GeneralLib.InStrNotNest(wclass, "接") > 0)
            {
                Sound.PlayWave("Swing.wav");
            }
            else if (GeneralLib.InStrNotNest(wclass, "実") > 0)
            {
                if (Strings.InStr(wname, "鞭") > 0
                    || Strings.InStr(wname, "ムチ") > 0
                    || Strings.InStr(wname, "ウイップ") > 0
                    || Strings.InStr(wname, "チェーン") > 0
                    || Strings.InStr(wname, "ロッド") > 0
                    || Strings.InStr(wname, "テンタク") > 0
                    || Strings.InStr(wname, "テイル") > 0
                    || Strings.InStr(wname, "尾") > 0
                    || Strings.InStr(wname, "触手") > 0
                    || Strings.InStr(wname, "触腕") > 0
                    || Strings.InStr(wname, "舌") > 0
                    || Strings.InStr(wname, "巻き") > 0
                    || Strings.InStr(wname, "糸") > 0)
                {
                    Sound.PlayWave("Swing.wav");
                }
            }
        }

        // 武器切り払い時の効果音
        public void ParryEffect(Unit u, UnitWeapon w, Unit t)
        {
            string wname, wclass;
            string sname;
            int num;
            int i;

            // 右クリック中は効果音をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 効果音生成回数を設定
            num = CountAttack(u, w);
            if (Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "機関銃") > 0
                || Strings.InStr(wname, "アサルトライフル") > 0
                || Strings.InStr(wname, "バルカン") > 0)
            {
                num = 4;
            }

            // 命中音を設定
            if (GeneralLib.InStrNotNest(wclass, "銃") > 0
                || GeneralLib.InStrNotNest(wclass, "格") > 0
                || GeneralLib.InStrNotNest(wclass, "武") > 0
                || GeneralLib.InStrNotNest(wclass, "突") > 0
                || Strings.InStr(wname, "弓") > 0
                || Strings.InStr(wname, "アロー") > 0
                || Strings.InStr(wname, "ロングボウ") > 0
                || Strings.InStr(wname, "ショートボウ") > 0
                || Strings.InStr(wname, "ボーガン") > 0
                || Strings.InStr(wname, "ボウガン") > 0
                || Strings.InStr(wname, "針") > 0
                || Strings.InStr(wname, "ニードル") > 0
                || Strings.InStr(wname, "ランサー") > 0
                || Strings.InStr(wname, "ダガー") > 0
                || Strings.InStr(wname, "剣") > 0)
            {
                sname = "Sword.wav";
            }
            else if (GeneralLib.InStrNotNest(wclass, "実") > 0)
            {
                sname = "Explode(Small).wav";
            }
            else if (GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
            {
                sname = "BeamCoat.wav";
            }
            else
            {
                sname = "Explode(Small).wav";
            }

            // 切り払い音を再生
            Sound.PlayWave("Saber.wav");
            GUI.Sleep(100);
            Sound.PlayWave(sname);
            var loopTo = num;
            for (i = 2; i <= loopTo; i++)
            {
                GUI.Sleep(130);
                Sound.PlayWave("Saber.wav");
                GUI.Sleep(100);
                Sound.PlayWave(sname);
            }

            // フラグをクリア
            Sound.IsWavePlayed = false;
        }

        // シールド防御時の特殊効果
        public void ShieldEffect(Unit u)
        {
            // 戦闘アニメ非自動選択オプション
            if (Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            {
                ShowAnimation("シールド防御発動");
                return;
            }

            // シールドのタイプを識別
            if (u.IsFeatureAvailable("エネルギーシールド"))
            {
                ShowAnimation("ビームシールド発動");
            }
            else if (u.IsFeatureAvailable("小型シールド"))
            {
                ShowAnimation("シールド防御発動 28");
            }
            else if (u.IsFeatureAvailable("大型シールド"))
            {
                ShowAnimation("シールド防御発動 40");
            }
            else
            {
                ShowAnimation("シールド防御発動");
            }
        }

        // 吸収・融合の特殊効果
        public void AbsorbEffect(Unit u, UnitWeapon w, Unit t)
        {
            string wclass, wname, cname;

            // 右クリック中は特殊効果をスキップ
            if (GUI.IsRButtonPressed())
            {
                return;
            }

            // 戦闘アニメオフの場合は効果音再生のみ
            if (!SRC.BattleAnimation || Expression.IsOptionDefined("戦闘アニメ非自動選択"))
            {
                Sound.PlayWave("Charge.wav");
                return;
            }

            {
                var withBlock = w.UpdatedWeaponData;
                wname = withBlock.Nickname();
                wclass = withBlock.Class;
            }

            // 描画色を決定
            cname = SpellColor(wname, wclass);
            if (string.IsNullOrEmpty(cname))
            {
                IsBeamWeapon(wname, wclass, cname);
            }

            // アニメを表示
            ShowAnimation("粒子集中発動 " + cname);
        }

        // 状態変化時の特殊効果
        public void CriticalEffect(string ctype, UnitWeapon w, bool ignore_death)
        {
            string aname, sname;
            int i;
            if (Strings.Len(ctype) == 0)
            {
                ShowAnimation("デフォルトクリティカル");
            }
            else
            {
                var loopTo = GeneralLib.LLength(ctype);
                for (i = 1; i <= loopTo; i++)
                {
                    aname = GeneralLib.LIndex(ctype, i) + "クリティカル";
                    if (aname == "即死クリティカル" && ignore_death)
                    {
                        goto NextLoop;
                    }

                    if (Event.FindNormalLabel("戦闘アニメ_" + aname) == 0)
                    {
                        goto NextLoop;
                    }

                    sname = "";
                    if (aname == "ショッククリティカル")
                    {
                        if (w.IsWeaponClassifiedAs("冷"))
                        {
                            // 冷気による攻撃で行動不能になった場合は効果音をオフ
                            sname = "-.wav";
                        }
                    }

                    if (!string.IsNullOrEmpty(sname))
                    {
                        ShowAnimation(aname + " " + sname);
                    }
                    else
                    {
                        ShowAnimation(aname);
                    }

                NextLoop:
                    ;
                }
            }
        }

        // 効果音の再生回数を決定
        private int CountAttack(Unit u, UnitWeapon w, int hit_count = 0)
        {
            int CountAttackRet = default;
            // メッセージスピードが「超高速」なら繰り返し数を１に設定
            if (GUI.MessageWait <= 200)
            {
                CountAttackRet = 1;
                return CountAttackRet;
            }

            // 連続攻撃の場合、命中数が指定されたならそちらにあわせる
            if (hit_count > 0 && Strings.InStr(w.UpdatedWeaponData.Class, "連") > 0)
            {
                CountAttackRet = hit_count;
                return CountAttackRet;
            }

            CountAttackRet = GeneralLib.MinLng(CountAttack0(u, w), 4);
            return CountAttackRet;
        }

        private int CountAttack0(Unit u, UnitWeapon w)
        {
            int CountAttack0Ret = default;
            string wname, wclass;
            wname = w.WeaponNickname();
            wclass = w.UpdatedWeaponData.Class;

            // 連続攻撃の場合は攻撃回数にあわせる
            if (GeneralLib.InStrNotNest(wclass, "連") > 0)
            {
                CountAttack0Ret = (int)w.WeaponLevel("連");
                return CountAttack0Ret;
            }

            if (Strings.InStr(wname, "連") > 0)
            {
                if (Strings.InStr(wname, "２４連") > 0)
                {
                    CountAttack0Ret = 8;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "２２連") > 0)
                {
                    CountAttack0Ret = 8;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "２０連") > 0
                    || Strings.InStr(wname, "二十連") > 0)
                {
                    CountAttack0Ret = 8;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１８連") > 0
                    || Strings.InStr(wname, "十八連") > 0)
                {
                    CountAttack0Ret = 7;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１６連") > 0
                    || Strings.InStr(wname, "十六連") > 0)
                {
                    CountAttack0Ret = 7;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１４連") > 0
                    || Strings.InStr(wname, "十四連") > 0)
                {
                    CountAttack0Ret = 7;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１２連") > 0
                    || Strings.InStr(wname, "十二連") > 0)
                {
                    CountAttack0Ret = 6;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１連") > 0
                    || Strings.InStr(wname, "一連") > 0)
                {
                    CountAttack0Ret = 6;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "１０連") > 0
                    || Strings.InStr(wname, "十連") > 0)
                {
                    CountAttack0Ret = 6;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "９連") > 0
                    || Strings.InStr(wname, "九連") > 0)
                {
                    CountAttack0Ret = 5;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "８連") > 0
                    || Strings.InStr(wname, "八連") > 0)
                {
                    CountAttack0Ret = 5;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "７連") > 0
                    || Strings.InStr(wname, "七連") > 0)
                {
                    CountAttack0Ret = 5;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "６連") > 0
                    || Strings.InStr(wname, "六連") > 0)
                {
                    CountAttack0Ret = 4;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "５連") > 0
                    || Strings.InStr(wname, "五連") > 0)
                {
                    CountAttack0Ret = 4;
                }

                if (Strings.InStr(wname, "４連") > 0
                    || Strings.InStr(wname, "四連") > 0)
                {
                    CountAttack0Ret = 4;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "３連") > 0
                    || Strings.InStr(wname, "三連") > 0)
                {
                    CountAttack0Ret = 3;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "２連") > 0
                    || Strings.InStr(wname, "二連") > 0)
                {
                    CountAttack0Ret = 2;
                    return CountAttack0Ret;
                }

                if (Strings.InStr(wname, "連打") > 0
                    || Strings.InStr(wname, "連射") > 0
                    || Strings.InStr(wname, "多連") > 0)
                {
                    CountAttack0Ret = 3;
                    return CountAttack0Ret;
                }

                CountAttack0Ret = 2;
                return CountAttack0Ret;
            }

            if (Strings.InStr(wname, "全弾") > 0
                || Strings.InStr(wname, "斉") > 0
                || Strings.InStr(wname, "乱射") > 0
                || Strings.InStr(wname, "フルファイア") > 0
                || Strings.InStr(wname, "スプリット") > 0
                || Strings.InStr(wname, "マルチ") > 0
                || Strings.InStr(wname, "パラレル") > 0
                || Strings.InStr(wname, "分身") > 0
                || Strings.InStr(wname, "乱打") > 0
                || Strings.InStr(wname, "乱舞") > 0
                || Strings.InStr(wname, "乱れ") > 0
                || Strings.InStr(wname, "百烈") > 0
                || Strings.InStr(wname, "千本") > 0
                || Strings.InStr(wname, "千手") > 0
                || Strings.InStr(wname, "ファンネル") > 0
                || Strings.InStr(wname, "ビット") > 0)
            {
                CountAttack0Ret = 4;
                return CountAttack0Ret;
            }

            if (Strings.InStr(wname, "マシンガン") > 0
                || Strings.InStr(wname, "機銃") > 0
                || Strings.InStr(wname, "機関銃") > 0
                || Strings.InStr(wname, "バルカン") > 0
                || Strings.InStr(wname, "ガトリング") > 0
                || Strings.InStr(wname, "パルス") > 0 && Strings.InStr(wname, "インパルス") == 0
                || Strings.InStr(wname, "速射") > 0
                || Strings.InStr(wname, "ロケットランチャー") > 0
                || Strings.InStr(wname, "ミサイルランチャー") > 0
                || Strings.InStr(wname, "ミサイルポッド") > 0)
            {
                CountAttack0Ret = 4;
                return CountAttack0Ret;
            }

            if (Strings.InStr(wname, "トリプル") > 0
                || Strings.InStr(wname, "インコム") > 0
                || Strings.InStr(wname, "ファミリア") > 0
                || Strings.InStr(wname, "爆撃") > 0
                || Strings.InStr(wname, "爆弾") > 0
                || Strings.InStr(wname, "爆雷") > 0
                || Strings.InStr(wname, "艦載機") > 0)
            {
                CountAttack0Ret = 3;
                return CountAttack0Ret;
            }

            if (Strings.InStr(wname, "ツイン") > 0
                || Strings.InStr(wname, "ダブル") > 0
                || Strings.InStr(wname, "デュアル") > 0
                || Strings.InStr(wname, "マイクロ") > 0
                || Strings.InStr(wname, "双") > 0
                || Strings.InStr(wname, "二丁") > 0
                || Strings.InStr(wname, "二刀") > 0)
            {
                CountAttack0Ret = 2;
                return CountAttack0Ret;
            }

            CountAttack0Ret = 1;
            return CountAttack0Ret;
        }

        // 光線系の攻撃かどうかを判定し、表示色を決定
        private bool IsBeamWeapon(string wname, string wclass, string cname)
        {
            bool IsBeamWeaponRet = default;
            if (GeneralLib.InStrNotNest(wclass, "実") > 0)
            {
                // 光線系攻撃ではあり得ない
                return IsBeamWeaponRet;
            }

            if (Strings.InStr(wname, "ビーム") > 0
                || GeneralLib.InStrNotNest(wclass, "Ｂ") > 0)
            {
                IsBeamWeaponRet = true;
            }
            else if (Strings.Right(wname, 2) == "ガス")
            {
                return IsBeamWeaponRet;
            }

            if (Strings.InStr(wname, "反物質") > 0
                || Strings.InStr(wname, "熱線") > 0
                || Strings.InStr(wname, "ブラスター") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "レッド";
            }
            else if (Strings.InStr(wname, "フェイザー") > 0
                || Strings.InStr(wname, "粒子") > 0)
            {
                IsBeamWeaponRet = true;
                if (Strings.InStr(wname, "メガ粒子") > 0)
                {
                    cname = "イエロー";
                }
                else
                {
                    cname = "ピンク";
                }
            }
            else if (Strings.InStr(wname, "冷凍") > 0
                || Strings.InStr(wname, "冷線") > 0
                || Strings.InStr(wname, "フリーザー") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "ブルー";
            }
            else if (Strings.InStr(wname, "中間子") > 0
                || Strings.InStr(wname, "中性子") > 0
                || Strings.InStr(wname, "ニュートロン") > 0
                || Strings.InStr(wname, "ニュートリノ") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "グリーン";
            }
            else if (Strings.InStr(wname, "プラズマ") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "オレンジ";
            }
            else if (Strings.InStr(wname, "レーザー") > 0
                || Strings.InStr(wname, "光子") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "イエロー";
            }
            else if (Strings.InStr(wname, "陽子") > 0)
            {
                IsBeamWeaponRet = true;
                cname = "ホワイト";
            }

            if (string.IsNullOrEmpty(cname))
            {
                if (Strings.InStr(wname, "粒子") > 0)
                {
                    if (Strings.InStr(wname, "メガ粒子") > 0)
                    {
                        cname = "イエロー";
                    }
                    else
                    {
                        cname = "ピンク";
                    }
                }
                else if (Strings.InStr(wname, "イオン") > 0
                    || Strings.InStr(wname, "冷凍") > 0
                    || Strings.InStr(wname, "電子") > 0)
                {
                    cname = "ブルー";
                }
            }

            if (!IsBeamWeaponRet && !string.IsNullOrEmpty(cname))
            {
                if (Strings.Right(wname, 2) == "光線"
                    || Strings.Right(wname, 1) == "砲"
                    || Strings.Right(wname, 1) == "銃")
                {
                    IsBeamWeaponRet = true;
                }
            }

            return IsBeamWeaponRet;
        }

        // 魔法の表示色
        private string SpellColor(string wname, string wclass)
        {
            string SpellColorRet = default;
            string sclass;
            int i;
            sclass = wname + wclass;

            // 武器名＆属性に含まれる漢字から判定
            var loopTo = Strings.Len(sclass);
            for (i = 1; i <= loopTo; i++)
            {
                switch (Strings.Mid(sclass, i, 1) ?? "")
                {
                    case "炎":
                    case "焔":
                    case "火":
                    case "血":
                    case "灼":
                    case "熱":
                    case "溶":
                        {
                            SpellColorRet = "赤";
                            return SpellColorRet;
                        }

                    case "水":
                    case "海":
                    case "流":
                    case "波":
                    case "河":
                        {
                            SpellColorRet = "青";
                            return SpellColorRet;
                        }

                    case "風":
                    case "嵐":
                    case "旋":
                    case "樹":
                    case "木":
                    case "草":
                    case "葉":
                    case "芽":
                    case "毒":
                        {
                            SpellColorRet = "緑";
                            return SpellColorRet;
                        }

                    case "邪":
                    case "闇":
                    case "暗":
                    case "死":
                    case "冥":
                    case "獄":
                    case "悪":
                    case "夜":
                    case "重":
                    case "影":
                    case "陰":
                    case "呪":
                    case "殺":
                        {
                            SpellColorRet = "黒";
                            return SpellColorRet;
                        }

                    case "土":
                    case "地":
                    case "金":
                    case "砂":
                    case "岩":
                    case "石":
                    case "山":
                    case "岳":
                        {
                            SpellColorRet = "黄";
                            return SpellColorRet;
                        }

                    case "生":
                    case "命":
                    case "魅":
                    case "誘":
                    case "乱":
                    case "♂":
                    case "♀":
                        {
                            SpellColorRet = "桃";
                            return SpellColorRet;
                        }

                    case "聖":
                    case "光":
                    case "星":
                    case "月":
                    case "氷":
                    case "雪":
                    case "冷":
                    case "凍":
                    case "冬":
                        {
                            SpellColorRet = "白";
                            return SpellColorRet;
                        }

                    case "日":
                    case "陽":
                        {
                            SpellColorRet = "橙";
                            return SpellColorRet;
                        }
                }
            }

            // 武器名から判定
            if (Strings.InStr(wname, "ファイヤー") > 0
                || Strings.InStr(wname, "フレア") > 0
                || Strings.InStr(wname, "ヒート") > 0
                || Strings.InStr(wname, "ブラッド") > 0)
            {
                SpellColorRet = "赤";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "ウォーター") > 0
                || Strings.InStr(wname, "アクア") > 0)
            {
                SpellColorRet = "青";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "ウッド") > 0
                || Strings.InStr(wname, "フォレスト") > 0
                || Strings.InStr(wname, "ポイズン") > 0)
            {
                SpellColorRet = "緑";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "イビル") > 0
                || Strings.InStr(wname, "エビル") > 0
                || Strings.InStr(wname, "ダーク") > 0
                || Strings.InStr(wname, "デス") > 0
                || Strings.InStr(wname, "ナイト") > 0
                || Strings.InStr(wname, "シャドウ") > 0
                || Strings.InStr(wname, "カース") > 0
                || Strings.InStr(wname, "カーズ") > 0)
            {
                SpellColorRet = "黒";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "アース") > 0
                || Strings.InStr(wname, "サンド") > 0
                || Strings.InStr(wname, "ロック") > 0
                || Strings.InStr(wname, "ストーン") > 0)
            {
                SpellColorRet = "黄";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "ライフ") > 0)
            {
                SpellColorRet = "桃";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "ホーリー") > 0
                || Strings.InStr(wname, "スター") > 0
                || Strings.InStr(wname, "ムーン") > 0
                || Strings.InStr(wname, "コールド") > 0
                || Strings.InStr(wname, "アイス") > 0
                || Strings.InStr(wname, "フリーズ") > 0)
            {
                SpellColorRet = "白";
                return SpellColorRet;
            }

            if (Strings.InStr(wname, "サン") > 0)
            {
                SpellColorRet = "橙";
                return SpellColorRet;
            }

            return SpellColorRet;
        }

        // 破壊アニメーションを表示する
        public void DieAnimation(Unit u)
        {
            int i;
            string fname, draw_mode;
            GUI.EraseUnitBitmap(u.x, u.y);

            // 人間ユニットでない場合は爆発を表示
            if (!u.IsHero())
            {
                ExplodeAnimation(u.Size, u.x, u.y);
                return;
            }

            // TODO Impl DieAnimation
            //GUI.GetCursorPos(PT);

            //// メッセージウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, My.MyProject.Forms.m_frmMessage))
            //{
            //    {
            //        var withBlock = My.MyProject.Forms.frmMessage;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// メインウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //{
            //    {
            //        var withBlock1 = GUI.MainForm;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// 倒れる音
            //switch (u.Area ?? "")
            //{
            //    case "地上":
            //        {
            //            Sound.PlayWave("FallDown.wav");
            //            break;
            //        }

            //    case "空中":
            //        {
            //            if (GUI.MessageWait > 0)
            //            {
            //                Sound.PlayWave("Bomb.wav");
            //                GUI.Sleep(500);
            //            }

            //            if (Map.TerrainClass(u.x, u.y) == "水"
            //|| Map.TerrainClass(u.x, u.y) == "深海")
            //            {
            //                Sound.PlayWave("Splash.wav");
            //            }
            //            else
            //            {
            //                Sound.PlayWave("FallDown.wav");
            //            }

            //            break;
            //        }
            //}

            //// ユニット消滅のアニメーション

            //// メッセージがウエイト無しならアニメーションもスキップ
            //if (GUI.MessageWait == 0)
            //{
            //    return;
            //}

            //switch (u.Party0 ?? "")
            //{
            //    case "味方":
            //    case "ＮＰＣ":
            //        {
            //            fname = @"Bitmap\Anime\Common\EFFECT_Tile(Ally)";
            //            break;
            //        }

            //    case "敵":
            //        {
            //            fname = @"Bitmap\Anime\Common\EFFECT_Tile(Enemy)";
            //            break;
            //        }

            //    case "中立":
            //        {
            //            fname = @"Bitmap\Anime\Common\EFFECT_Tile(Neutral)";
            //            break;
            //        }
            //}

            //if (GeneralLib.FileExists(SRC.ScenarioPath + fname + ".bmp"))
            //{
            //    fname = SRC.ScenarioPath + fname;
            //}
            //else
            //{
            //    fname = SRC.AppPath + fname;
            //}

            //bool localFileExists() { string argfname = fname + "01.bmp"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //if (!localFileExists())
            //{
            //    return;
            //}

            //switch (Map.MapDrawMode ?? "")
            //{
            //    case "夜":
            //        {
            //            draw_mode = "暗";
            //            break;
            //        }

            //    default:
            //        {
            //            draw_mode = Map.MapDrawMode;
            //            break;
            //        }
            //}

            //for (i = 1; i <= 6; i++)
            //{
            //    GUI.DrawPicture(fname + ".bmp", GUI.MapToPixelX(u.x), GUI.MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, draw_mode);
            //    GUI.DrawPicture(@"Unit\" + u.get_Bitmap(false), GUI.MapToPixelX(u.x), GUI.MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + draw_mode);
            //    GUI.DrawPicture(fname + "0" + SrcFormatter.Format(i) + ".bmp", GUI.MapToPixelX(u.x), GUI.MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + draw_mode);
            //    GUI.UpdateScreen();
            //    GUI.Sleep(50);
            //}

            GUI.ClearPicture();
            GUI.UpdateScreen();
        }

        private bool init_explode_animation;
        private string explode_image_path;
        // 爆発アニメーションを表示する
        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            // TODO Impl ExplodeAnimation
            //int i;
            //int explode_image_num;
            //var PT = default(GUI.POINTAPI);

            //// 初めて実行する際に、爆発用画像があるフォルダをチェック
            //if (!init_explode_animation)
            //{
            //    // 爆発用画像のパス
            //    bool localFileExists() { string argfname = SRC.ScenarioPath + @"Bitmap\Event\Explode01.bmp"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //    bool localFileExists1() { string argfname = SRC.AppPath + @"Bitmap\Anime\Explode\EFFECT_Explode01.bmp"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //    if (GeneralLib.FileExists(SRC.ScenarioPath + @"Bitmap\Anime\Explode\EFFECT_Explode01.bmp"))
            //    {
            //        explode_image_path = SRC.ScenarioPath + @"Bitmap\Anime\Explode\EFFECT_Explode";
            //    }
            //    else if (localFileExists())
            //    {
            //        explode_image_path = SRC.ScenarioPath + @"Bitmap\Event\Explode";
            //    }
            //    else if (localFileExists1())
            //    {
            //        explode_image_path = SRC.AppPath + @"Bitmap\Anime\Explode\EFFECT_Explode";
            //    }
            //    else
            //    {
            //        explode_image_path = SRC.AppPath + @"Bitmap\Event\Explode";
            //    }

            //    // 爆発用画像の個数
            //    i = 2;
            //    while (GeneralLib.FileExists(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp"))
            //        i = (i + 1);
            //    explode_image_num = (i - 1);
            //}

            //GUI.GetCursorPos(PT);

            //// メッセージウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, My.MyProject.Forms.m_frmMessage))
            //{
            //    {
            //        var withBlock = My.MyProject.Forms.frmMessage;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// メインウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //{
            //    {
            //        var withBlock1 = GUI.MainForm;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// 爆発音
            //switch (tsize ?? "")
            //{
            //    case "XL":
            //    case "LL":
            //        {
            //            Sound.PlayWave("Explode(Far).wav");
            //            break;
            //        }

            //    case "L":
            //    case "M":
            //    case "S":
            //    case "SS":
            //        {
            //            Sound.PlayWave("Explode.wav");
            //            break;
            //        }
            //}

            //// メッセージがウエイト無しなら爆発もスキップ
            //if (GUI.MessageWait == 0)
            //{
            //    return;
            //}

            //// 爆発の表示
            //if (Strings.InStr(explode_image_path, @"\Anime\") > 0)
            //{
            //    // 戦闘アニメ版の画像を使用
            //    switch (tsize ?? "")
            //    {
            //        case "XL":
            //            {
            //                var loopTo = explode_image_num;
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 64, GUI.MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(130);
            //                }

            //                break;
            //            }

            //        case "LL":
            //            {
            //                var loopTo1 = explode_image_num;
            //                for (i = 1; i <= loopTo1; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 56, GUI.MapToPixelY(ty) - 56, 144, 144, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(100);
            //                }

            //                break;
            //            }

            //        case "L":
            //            {
            //                var loopTo2 = explode_image_num;
            //                for (i = 1; i <= loopTo2; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 48, GUI.MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(70);
            //                }

            //                break;
            //            }

            //        case "M":
            //            {
            //                var loopTo3 = explode_image_num;
            //                for (i = 1; i <= loopTo3; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 40, GUI.MapToPixelY(ty) - 40, 112, 112, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(50);
            //                }

            //                break;
            //            }

            //        case "S":
            //            {
            //                var loopTo4 = explode_image_num;
            //                for (i = 1; i <= loopTo4; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 24, GUI.MapToPixelY(ty) - 24, 80, 80, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(40);
            //                }

            //                break;
            //            }

            //        case "SS":
            //            {
            //                var loopTo5 = explode_image_num;
            //                for (i = 1; i <= loopTo5; i++)
            //                {
            //                    GUI.ClearPicture();
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 8, GUI.MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(40);
            //                }

            //                break;
            //            }
            //    }

            //    GUI.ClearPicture();
            //    GUI.UpdateScreen();
            //}
            //else
            //{
            //    // 汎用イベント画像版の画像を使用
            //    switch (tsize ?? "")
            //    {
            //        case "XL":
            //            {
            //                var loopTo6 = explode_image_num;
            //                for (i = 1; i <= loopTo6; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 64, GUI.MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(130);
            //                }

            //                break;
            //            }

            //        case "LL":
            //            {
            //                var loopTo7 = explode_image_num;
            //                for (i = 1; i <= loopTo7; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 48, GUI.MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(100);
            //                }

            //                break;
            //            }

            //        case "L":
            //            {
            //                var loopTo8 = explode_image_num;
            //                for (i = 1; i <= loopTo8; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 32, GUI.MapToPixelY(ty) - 32, 96, 96, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(70);
            //                }

            //                break;
            //            }

            //        case "M":
            //            {
            //                var loopTo9 = explode_image_num;
            //                for (i = 1; i <= loopTo9; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 16, GUI.MapToPixelY(ty) - 16, 64, 64, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(50);
            //                }

            //                break;
            //            }

            //        case "S":
            //            {
            //                var loopTo10 = explode_image_num;
            //                for (i = 1; i <= loopTo10; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx) - 8, GUI.MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(40);
            //                }

            //                break;
            //            }

            //        case "SS":
            //            {
            //                var loopTo11 = explode_image_num;
            //                for (i = 1; i <= loopTo11; i++)
            //                {
            //                    GUI.DrawPicture(explode_image_path + SrcFormatter.Format(i, "00") + ".bmp", GUI.MapToPixelX(tx), GUI.MapToPixelY(ty), 32, 32, 0, 0, 0, 0, "透過");
            //                    GUI.UpdateScreen();
            //                    GUI.Sleep(40);
            //                }

            //                break;
            //            }
            //    }

            //    GUI.ClearPicture();
            //    GUI.UpdateScreen();
            //}
        }

        // 攻撃無効化時の特殊効果とメッセージを表示する
        public void NegateEffect(Unit u, Unit t, UnitWeapon w, string wname, int dmg, string fname, string fdata, int ecost, string msg, bool be_quiet)
        {
            var defined = default(bool);
            if (GeneralLib.LIndex(fdata, 1) == "Ｂ"
|| GeneralLib.LIndex(fdata, 2) == "Ｂ"
|| GeneralLib.LIndex(fdata, 3) == "Ｂ")
            {
                if (!be_quiet)
                {
                    if (t.IsMessageDefined("ビーム無効化(" + fname + ")"))
                    {
                        t.PilotMessage("ビーム無効化(" + fname + ")", msg_mode: "");
                    }
                    else
                    {
                        t.PilotMessage("ビーム無効化", msg_mode: "");
                    }
                }

                if (t.IsAnimationDefined("ビーム無効化", fname))
                {
                    t.PlayAnimation("ビーム無効化", fname);
                }
                else if (t.IsSpecialEffectDefined("ビーム無効化", fname))
                {
                    t.SpecialEffect("ビーム無効化", fname);
                }
                else if (dmg < 0)
                {
                    AbsorbEffect(u, w, t);
                }
                else if (SRC.BattleAnimation)
                {
                    ShowAnimation("ビームコート発動 - " + fname);
                }
                else if (!Sound.IsWavePlayed)
                {
                    Sound.PlayWave("BeamCoat.wav");
                }

                bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(攻撃無効化)"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                if (u.IsAnimationDefined(wname + "(攻撃無効化)", sub_situation: ""))
                {
                    u.PlayAnimation(wname + "(攻撃無効化)", sub_situation: "");
                }
                else if (localIsSpecialEffectDefined())
                {
                    u.SpecialEffect(wname + "(攻撃無効化)", sub_situation: "");
                }

                if (t.IsSysMessageDefined("ビーム無効化", fname))
                {
                    t.SysMessage("ビーム無効化", fname, add_msg: "");
                }
                else if (string.IsNullOrEmpty(fname))
                {
                    if (dmg < 0)
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "が攻撃を吸収した。");
                    }
                    else
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "が攻撃を防いだ。");
                    }
                }
                else if (dmg < 0)
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "の[" + fname + "]が攻撃を吸収した。");
                }
                else
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "の[" + fname + "]が攻撃を防いだ。");
                }
            }
            else
            {
                if (!be_quiet)
                {
                    if (t.IsMessageDefined("攻撃無効化(" + fname + ")"))
                    {
                        t.PilotMessage("攻撃無効化(" + fname + ")", msg_mode: "");
                    }
                    else
                    {
                        t.PilotMessage("攻撃無効化", msg_mode: "");
                    }
                }

                if (t.IsAnimationDefined("攻撃無効化", fname))
                {
                    t.PlayAnimation("攻撃無効化", fname);
                    defined = true;
                }
                else if (t.IsSpecialEffectDefined("攻撃無効化", fname))
                {
                    t.SpecialEffect("攻撃無効化", fname);
                    defined = true;
                }
                else if (dmg < 0)
                {
                    AbsorbEffect(u, w, t);
                    defined = true;
                }
                else if (SRC.BattleAnimation)
                {
                    if (Strings.InStr(fdata, "バリア無効化無効") == 0
|| ecost > 0)
                    {
                        if (fname == "バリア")
                        {
                            ShowAnimation("バリア発動");
                        }
                        else if (string.IsNullOrEmpty(fname))
                        {
                            ShowAnimation("バリア発動 - 攻撃無効化");
                        }
                        else
                        {
                            ShowAnimation("バリア発動 - " + fname);
                        }

                        defined = true;
                    }
                }

                bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(攻撃無効化)"; string argsub_situation = ""; var ret = u.IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                if (u.IsAnimationDefined(wname + "(攻撃無効化)", sub_situation: ""))
                {
                    u.PlayAnimation(wname + "(攻撃無効化)", sub_situation: "");
                    defined = true;
                }
                else if (localIsSpecialEffectDefined1())
                {
                    u.SpecialEffect(wname + "(攻撃無効化)", sub_situation: "");
                    defined = true;
                }

                if (!defined)
                {
                    HitEffect(u, w, t);
                }

                if (t.IsSysMessageDefined("攻撃無効化", fname))
                {
                    t.SysMessage("攻撃無効化", fname, add_msg: "");
                }
                else if (string.IsNullOrEmpty(fname))
                {
                    if (dmg < 0)
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃を吸収した。");
                    }
                    else
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃を防いだ。");
                    }
                }
                else if (dmg < 0)
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "の[" + fname + "]が攻撃を吸収した。");
                }
                else
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "の[" + fname + "]が攻撃を防いだ。");
                }
            }
        }
    }
}
