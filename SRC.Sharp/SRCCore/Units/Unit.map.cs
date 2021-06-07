using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // ユニットを(new_x,new_y)に配置
        public void StandBy(int new_x, int new_y, string smode = "")
        {
            // とりあえず地形を考慮せずにデフォルトのポジションを決めておく
            // (Createコマンドの後で空中移動用アイテムを付けるときのため)
            var defaultPositionSet = false;
            for (var i = 0; i <= 20; i++)
            {
                var loopTo = GeneralLib.MinLng(new_x + i, Map.MapWidth);
                for (var j = GeneralLib.MaxLng(new_x - i, 1); j <= loopTo; j++)
                {
                    var loopTo1 = GeneralLib.MinLng(new_y + i, Map.MapHeight);
                    for (var k = GeneralLib.MaxLng(new_y - i, 1); k <= loopTo1; k++)
                    {
                        if ((Math.Abs((new_x - j)) + Math.Abs((new_y - k))) == i)
                        {
                            if (Map.MapDataForUnit[j, k] is null)
                            {
                                x = j;
                                y = k;
                                defaultPositionSet = true;
                                break;
                            }
                        }
                    }
                    if (defaultPositionSet) { break; }
                }
                if (defaultPositionSet) { break; }
            }

            // 空いた場所を検索
            // XXX Goto 潰しておきたい
            for (var i = 0; i <= 20; i++)
            {
                // ユニット同士を隣接させずに配置する？
                if (Strings.InStr(smode, "部隊配置") > 0)
                {
                    if (i % 2 != 0)
                    {
                        goto NextDistance;
                    }
                }
                // 指定した場所の周りを調べる
                var loopTo2 = GeneralLib.MinLng(new_x + i, Map.MapWidth);
                for (var j = GeneralLib.MaxLng(new_x - i, 1); j <= loopTo2; j++)
                {
                    var loopTo3 = GeneralLib.MinLng(new_y + i, Map.MapHeight);
                    for (var k = GeneralLib.MaxLng(new_y - i, 1); k <= loopTo3; k++)
                    {
                        if ((Math.Abs((new_x - j)) + Math.Abs((new_y - k))) != i)
                        {
                            goto NextLoop;
                        }

                        // 既に他のユニットがいる？
                        if (Map.MapDataForUnit[j, k] is object)
                        {
                            goto NextLoop;
                        }

                        // 進入不能の地形？
                        if (Map.Terrain(j, k).MoveCost > 100)
                        {
                            goto NextLoop;
                        }

                        switch (Map.Terrain(j, k).Class ?? "")
                        {
                            case "空":
                                {
                                    if (!IsTransAvailable("空"))
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case "水":
                                {
                                    if (!IsTransAvailable("水上") && !IsTransAvailable("空") && get_Adaption(3) == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case "深水":
                                {
                                    if (!IsTransAvailable("水上") && !IsTransAvailable("空") && !IsTransAvailable("水"))
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }
                        }

                        // 空き位置が見つかった
                        x = j;
                        y = k;
                        goto ExitFor;
                    NextLoop:
                        ;
                    }
                }

            NextDistance:
                ;
            }

        ExitFor:
            ;

            // 空いた場所がなかった？
            if (x == 0 && y == 0)
            {
                Status = "待機";
                return;
            }

            // 他の形態と格納したユニットの座標も合わせておく
            foreach (var of in OtherForms)
            {
                of.x = x;
                of.y = y;
            }

            foreach (var u in UnitOnBoards)
            {
                u.x = x;
                u.y = y;
            }

            // 格納されていた場合はあらかじめ降ろしておく
            if (Status == "格納")
            {
                var boardUnit = SRC.UList.Items.First(x => x.UnitOnBoard(ID) != null);
                boardUnit?.UnitOnBoard(ID);
            }

            // Statusを更新
            Status = "出撃";
            foreach (var of in OtherForms)
            {
                of.Status = "他形態";
            }


            // ユニットのいる地形は？
            switch (Map.Terrain(x, y).Class ?? "")
            {
                case "空":
                    Area = "空中";
                    break;

                case "陸":
                    if (IsTransAvailable("地中") && Area == "地中")
                    {
                        Area = "地中";
                    }
                    else if (IsTransAvailable("空") && get_Adaption(1) >= get_Adaption(2))
                    {
                        Area = "空中";
                    }
                    else if (IsTransAvailable("陸"))
                    {
                        Area = "地上";
                    }
                    else
                    {
                        Area = "空中";
                    }

                    break;

                case "屋内":
                    if (IsTransAvailable("空") && get_Adaption(1) >= get_Adaption(2))
                    {
                        Area = "空中";
                    }
                    else if (IsTransAvailable("陸"))
                    {
                        Area = "地上";
                    }
                    else
                    {
                        Area = "空中";
                    }

                    break;

                case "月面":
                    if (IsTransAvailable("空") || IsTransAvailable("宇宙"))
                    {
                        Area = "宇宙";
                    }
                    else if (IsTransAvailable("陸"))
                    {
                        Area = "地上";
                    }
                    else
                    {
                        Area = "宇宙";
                    }

                    break;

                case "水":
                case "深水":
                    if (IsTransAvailable("空") && get_Adaption(1) >= get_Adaption(2))
                    {
                        Area = "空中";
                    }
                    else if (IsTransAvailable("水上"))
                    {
                        Area = "水上";
                    }
                    else
                    {
                        Area = "水中";
                    }

                    break;

                case "宇宙":
                    Area = "宇宙";
                    break;

                default:
                    Area = "地上";
                    break;
            }

            // マップに登録
            Map.MapDataForUnit[x, y] = this;

            // TODO Impl 登場時アニメを表示他
            //// 登場時アニメを表示
            //// MOD START MARGE
            //// If (smode = "出撃" Or smode = "部隊配置") _
            //// '        And MainForm.Visible _
            //// '        And Not IsPictureVisible _
            //// '        And Not IsRButtonPressed() _
            //// '        And BitmapID > 0 _
            //// '    Then
            //var fname = default(string);
            //int start_time, current_time;
            //if ((Strings.InStr(smode, "出撃") > 0 || Strings.InStr(smode, "部隊配置") > 0) 
            //    && GUI.MainFormVisible && !GUI.IsPictureVisible && !GUI.IsRButtonPressed() && BitmapID > 0)
            //{
            //    // MOD END MARGE

            //    // ユニット出現音
            //    Sound.PlayWave("UnitOn.wav");

            //    // 表示させる画像
            //    switch (Party0 ?? "")
            //    {
            //        case "味方":
            //        case "ＮＰＣ":
            //            {
            //                fname = @"Bitmap\Event\AUnitOn0";
            //                break;
            //            }

            //        case "敵":
            //            {
            //                fname = @"Bitmap\Event\EUnitOn0";
            //                break;
            //            }

            //        case "中立":
            //            {
            //                fname = @"Bitmap\Event\NUnitOn0";
            //                break;
            //            }
            //    }

            //    if (SRC.FileSystem.FileExists(SRC.AppPath + fname + "1.bmp"))
            //    {
            //        // アニメ表示開始時刻を記録
            //        start_time = GeneralLib.timeGetTime();
            //        for (i = 1; i <= 4; i++)
            //        {
            //            // 画像を透過表示
            //            if (GUI.DrawPicture(fname + SrcFormatter.Format(i) + ".bmp", GUI.MapToPixelX(x), GUI.MapToPixelY(y), 32, 32, 0, 0, 0, 0, "透過") == false)
            //            {
            //                break;
            //            }
            //            GUI.MainForm.picMain(0).Refresh();

            //            // ウェイト
            //            do
            //            {
            //                Application.DoEvents();
            //                current_time = GeneralLib.timeGetTime();
            //            }
            //            while (current_time < start_time + 15);
            //            start_time = current_time;

            //            // 画像を消去
            //            GUI.ClearPicture();
            //        }

            //        // アニメ画像は上書きして消してしまうので……
            //        GUI.IsPictureVisible = false;
            //    }
            //}

            //// ユニット画像をマップに描画
            //if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // MOD START MARGE
            //    // If smode = "非同期" Then
            //    if (Strings.InStr(smode, "非同期") > 0)
            //    {
            //        // MOD END MARGE
            //        GUI.PaintUnitBitmap(this, "リフレッシュ無し");
            //    }
            //    else
            //    {
            //        GUI.PaintUnitBitmap(this);
            //    }
            //}

            //// 制御不能？
            //if (IsFeatureAvailable("制御不可"))
            //{
            //    AddCondition("暴走", -1, cdata: "");
            //}

            Update();
            SRC.PList.UpdateSupportMod(this);
        }

        // ユニットを(new_x,new_y)に移動
        public void Move(int new_x, int new_y, bool without_en_consumption = false, bool by_cancel = false, bool by_teleport_or_jump = false)
        {
            // ユニットをマップからいったん削除
            if (Map.MapDataForUnit[x, y]?.ID == ID)
            {
                Map.MapDataForUnit[x, y] = null;
            }

            // XXX 何が違うんだ？
            if (GUI.IsPictureVisible)
            {
                GUI.EraseUnitBitmap(x, y, false);
            }
            else
            {
                GUI.EraseUnitBitmap(x, y, false);
            }

            SRC.PList.UpdateSupportMod(this);

            // ユニット位置を指定された座標に
            var prev_x = x;
            var prev_y = y;
            x = new_x;
            y = new_y;
            foreach (var of in OtherForms)
            {
                of.x = x;
                of.y = y;
            }

            foreach (var u in UnitOnBoards)
            {
                u.x = x;
                u.y = y;
            }

            // 指定された場所に既にユニットが存在？
            if (Map.MapDataForUnit[x, y] != null)
            {
                var targetUnit = Map.MapDataForUnit[x, y];
                // 合体？
                foreach (var cfd in this.TwoUnitCombineFeatures(SRC)
                    .Where(x =>
                    {
                        // XXX 合体制限は変形してる時だけでいい？
                        var pu = SRC.UList.Item(x.PartUnitNames.First());
                        return targetUnit.Name == x.PartUnitNames.First()
                            || targetUnit.Name == pu.CurrentForm().Name && !targetUnit.IsFeatureAvailable("合体制限");
                    }))
                {
                    Combine();
                    return;
                }

                // 着艦？
                if (!targetUnit.IsFeatureAvailable("母艦"))
                {
                    // XXX そういう理由でエラーするの？」
                    GUI.ErrorMessage("合体元ユニット「" + Name + "」が複数あるため合体処理が出来ません");
                    return;
                }

                // 着艦処理
                Land(targetUnit, by_cancel);
                return;
            }

            // 移動先によるユニット位置変更
            ResolveAreaForMoved();

            // マップにユニットを登録
            Map.MapDataForUnit[x, y] = this;

            // ユニット描画
            if (!GUI.IsPictureVisible)
            {
                if (SRC.MoveAnimation && !by_cancel && !by_teleport_or_jump)
                {
                    GUI.MoveUnitBitmap2(this, 20);
                }
                else
                {
                    GUI.PaintUnitBitmap(this);
                }
            }

            // 移動によるＥＮ消費
            if (!without_en_consumption)
            {
                switch (Area ?? "")
                {
                    case "地上":
                    case "水上":
                        if (IsFeatureAvailable("ホバー移動"))
                        {
                            EN = EN - 5;
                        }

                        break;

                    case "空中":
                    case "宇宙":
                        EN = EN - 5;
                        break;

                    case "地中":
                        EN = EN - 10;
                        break;
                }
            }

            // 情報更新
            Update();
            SRC.PList.UpdateSupportMod(this);
        }

        private void ResolveAreaForMoved()
        {
            switch (Map.Terrain(x, y).Class ?? "")
            {
                case "空":
                    {
                        Area = "空中";
                        break;
                    }

                case "陸":
                case "屋内":
                    {
                        switch (Area ?? "")
                        {
                            case "水中":
                            case "水上":
                                {
                                    Area = "地上";
                                    break;
                                }

                            case "宇宙":
                                {
                                    if (IsTransAvailable("空") && get_Adaption(1) >= get_Adaption(2))
                                    {
                                        Area = "空中";
                                    }
                                    else if (IsTransAvailable("陸"))
                                    {
                                        Area = "地上";
                                    }
                                    else
                                    {
                                        Area = "空中";
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case "月面":
                    {
                        switch (Area ?? "")
                        {
                            // 変更なし
                            case "地上":
                            case "地中":
                                {
                                    break;
                                }

                            default:
                                {
                                    if ((IsTransAvailable("空") || IsTransAvailable("宇宙")) && get_Adaption(4) >= get_Adaption(2))
                                    {
                                        Area = "宇宙";
                                    }
                                    else if (IsTransAvailable("陸"))
                                    {
                                        Area = "地上";
                                    }
                                    else
                                    {
                                        Area = "宇宙";
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case "水":
                case "深水":
                    {
                        switch (Area ?? "")
                        {
                            case "地上":
                                {
                                    if (IsTransAvailable("水上"))
                                    {
                                        Area = "水上";
                                    }
                                    else
                                    {
                                        Area = "水中";
                                    }

                                    break;
                                }

                            case "宇宙":
                                {
                                    Area = "水中";
                                    break;
                                }
                        }

                        break;
                    }

                case "宇宙":
                    {
                        Area = "宇宙";
                        break;
                    }
            }
        }

        // ユニットを(new_x,new_y)にジャンプ
        public void Jump(int new_x, int new_y, bool do_refresh = true)
        {
            // ユニットを一旦マップから削除
            Map.MapDataForUnit[x, y] = null;
            GUI.EraseUnitBitmap(x, y, do_refresh);
            SRC.PList.UpdateSupportMod(this);

            // 空き位置を検索
            var prev_x = x;
            var prev_y = y;
            x = new_x;
            y = new_y;
            for (var i = 0; i <= 10; i++)
            {
                var loopTo = GeneralLib.MinLng(new_x + i, Map.MapWidth);
                for (var j = GeneralLib.MaxLng(new_x - i, 1); j <= loopTo; j++)
                {
                    var loopTo1 = GeneralLib.MinLng(new_y + i, Map.MapHeight);
                    for (var k = GeneralLib.MaxLng(new_y - i, 1); k <= loopTo1; k++)
                    {
                        if ((Math.Abs((new_x - j)) + Math.Abs((new_y - k))) != i)
                        {
                            goto NextLoop;
                        }

                        if (Map.MapDataForUnit[j, k] is object)
                        {
                            goto NextLoop;
                        }

                        if (Map.Terrain(j, k).MoveCost > 100)
                        {
                            goto NextLoop;
                        }

                        switch (Map.Terrain(j, k).Class ?? "")
                        {
                            case "空":
                                {
                                    if (!IsTransAvailable("空"))
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case "水":
                            case "深水":
                                {
                                    if (!IsTransAvailable("水上") && !IsTransAvailable("空") && get_Adaption(3) == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }
                        }

                        x = j;
                        y = k;
                        goto ExitFor;
                    NextLoop:
                        ;
                    }
                }
            }

        ExitFor:
            ;

            // 他の形態と格納したユニットの座標を更新
            foreach (var of in OtherForms)
            {
                of.x = x;
                of.y = y;
            }

            foreach (var u in UnitOnBoards)
            {
                u.x = x;
                u.y = y;
            }

            // 移動先によるユニット位置変更
            ResolveAreaForMoved();

            // マップにユニットを登録
            Map.MapDataForUnit[x, y] = this;

            // 情報更新
            Update();
            SRC.PList.UpdateSupportMod(this);

            // ユニット描画
            if (do_refresh)
            {
                GUI.PaintUnitBitmap(this);
            }
        }

        // マップ上から脱出
        public void Escape(string smode = "")
        {
            // 母艦に乗っていた場合は降りておく
            if (Status == "格納")
            {
                SRC.UList.Unload(this);
            }

            // 出撃している場合は画面上からユニットを消去
            if (Status == "出撃" || Status == "破壊")
            {
                if (ReferenceEquals(Map.MapDataForUnit[x, y], this))
                {
                    Map.MapDataForUnit[x, y] = null;
                    if (smode == "非同期" || GUI.IsPictureVisible || string.IsNullOrEmpty(Map.MapFileName))
                    {
                        GUI.EraseUnitBitmap(x, y, false);
                    }
                    else
                    {
                        GUI.EraseUnitBitmap(x, y, true);
                    }

                    SRC.PList.UpdateSupportMod(this);
                }
            }

            if (Status == "出撃" || Status == "格納")
            {
                Status = "待機";
            }

            // 破壊をキャンセル状態は解除
            if (IsConditionSatisfied("破壊キャンセル"))
            {
                DeleteCondition("破壊キャンセル");
            }

            // ユニットを格納していたら降ろす
            UnloadAllUnitForEscape();

            // 召喚したユニットを解放
            DismissServant();

            // 魅了・憑依したユニットを解放
            DismissSlave();

            // ステータス表示中の場合は表示を解除
            if (ReferenceEquals(this, SRC.GUIStatus.DisplayedUnit))
            {
                SRC.GUIStatus.ClearUnitStatus();
            }
        }

        // 母艦 u に着艦
        public void Land(Unit u, bool by_cancel = false, bool is_event = false)
        {
            // Landコマンドで着艦した場合
            if (is_event)
            {
                if (Status == "出撃" || Status == "格納")
                {
                    Escape();
                }
                else
                {
                    // 出撃のための前準備

                    // ユニットが存在する位置を決定
                    string tclass;
                    if (u.Status == "出撃")
                    {
                        tclass = Map.Terrain(u.x, u.y).Class;
                    }
                    else
                    {
                        tclass = Map.Terrain((Map.MapWidth / 2), (Map.MapHeight / 2)).Class;
                    }

                    switch (tclass ?? "")
                    {
                        case "空":
                            {
                                Area = "空中";
                                break;
                            }

                        case "陸":
                        case "屋内":
                            {
                                if (IsTransAvailable("空") && Strings.Mid(strAdaption, 1, 1) == "A")
                                {
                                    Area = "空中";
                                }
                                else if (IsTransAvailable("陸"))
                                {
                                    Area = "地上";
                                }
                                else
                                {
                                    Area = "空中";
                                }

                                break;
                            }

                        case "月面":
                            {
                                if ((IsTransAvailable("空") || IsTransAvailable("宇宙")) && Strings.Mid(strAdaption, 4, 1) == "A")
                                {
                                    Area = "宇宙";
                                }
                                else if (IsTransAvailable("陸"))
                                {
                                    Area = "地上";
                                }
                                else
                                {
                                    Area = "宇宙";
                                }

                                break;
                            }

                        case "水":
                        case "深水":
                            {
                                if (IsTransAvailable("空"))
                                {
                                    Area = "空中";
                                }
                                else if (IsTransAvailable("水上"))
                                {
                                    Area = "水上";
                                }
                                else
                                {
                                    Area = "水中";
                                }

                                break;
                            }

                        case "宇宙":
                            {
                                Area = "宇宙";
                                break;
                            }
                    }

                    // 行動回数等を回復
                    UsedAction = 0;
                    UsedSupportAttack = 0;
                    UsedSupportGuard = 0;
                    UsedSyncAttack = 0;
                    UsedCounterAttack = 0;

                    if (IsFeatureAvailable("制御不可"))
                    {
                        AddCondition("暴走", -1, Constants.DEFAULT_LEVEL, "");
                    }
                }
            }

            // 母艦に自分自身を格納
            u.LoadUnit(this);

            // 座標を母艦に合わせる
            x = u.x;
            y = u.y;
            Status = "格納";
            if (Area != "宇宙" && Area != "空中")
            {
                Area = "地上";
            }

            // 気力減少
            if (!by_cancel)
            {
                // TODO Impl 気力減少
                //{
                //    var withBlock1 = MainPilot();
                //    if (withBlock1.Personality != "機械")
                //    {
                //        if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                //        {
                //            withBlock1.Morale = GeneralLib.MinLng(withBlock1.Morale, GeneralLib.MaxLng(withBlock1.Morale - 5, 100));
                //        }
                //        else
                //        {
                //            withBlock1.Morale = (withBlock1.Morale - 5);
                //        }
                //    }
                //}

                //var loopTo = CountPilot();
                //for (i = 1; i <= loopTo; i++)
                //{
                //    {
                //        var withBlock2 = Pilot(i);
                //        if ((MainPilot().ID ?? "") != (withBlock2.ID ?? "") && withBlock2.Personality != "機械")
                //        {
                //            if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                //            {
                //                withBlock2.Morale = GeneralLib.MinLng(withBlock2.Morale, GeneralLib.MaxLng(withBlock2.Morale - 5, 100));
                //            }
                //            else
                //            {
                //                withBlock2.Morale = (withBlock2.Morale - 5);
                //            }
                //        }
                //    }
                //}

                //var loopTo1 = CountSupport();
                //for (i = 1; i <= loopTo1; i++)
                //{
                //    {
                //        var withBlock3 = Support(i);
                //        if (withBlock3.Personality != "機械")
                //        {
                //            if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                //            {
                //                withBlock3.Morale = GeneralLib.MinLng(withBlock3.Morale, GeneralLib.MaxLng(withBlock3.Morale - 5, 100));
                //            }
                //            else
                //            {
                //                withBlock3.Morale = (withBlock3.Morale - 5);
                //            }
                //        }
                //    }
                //}

                //if (IsFeatureAvailable("追加サポート"))
                //{
                //    {
                //        var withBlock4 = AdditionalSupport();
                //        if (withBlock4.Personality != "機械")
                //        {
                //            if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                //            {
                //                withBlock4.Morale = GeneralLib.MinLng(withBlock4.Morale, GeneralLib.MaxLng(withBlock4.Morale - 5, 100));
                //            }
                //            else
                //            {
                //                withBlock4.Morale = (withBlock4.Morale - 5);
                //            }
                //        }
                //    }
                //}
            }
        }

        // new_form へ変形（換装、ハイパーモード、パーツ分離＆合体を含む）
        public void Transform(string new_form)
        {
            //    string list;
            //    int i, idx, idx2, j;
            //    Unit u;
            //    string[] wname;
            //    int[] wbullet;
            //    int[] wmaxbullet;
            //    string[] aname;
            //    int[] astock;
            //    int[] amaxstock;
            //    double hp_ratio, en_ratio;
            //    int prev_x, prev_y;
            //    string buf;
            var hp_ratio = 100 * HP / (double)MaxHP;
            var en_ratio = 100 * EN / (double)MaxEN;
            var u = OtherForm(new_form);
            u.Status = Status;
            if (Status != "破棄")
            {
                Status = "他形態";
            }

            // 制御不可能な形態から元に戻る場合は暴走を解除
            if (IsFeatureAvailable("制御不可"))
            {
                if (IsConditionSatisfied("暴走"))
                {
                    DeleteCondition("暴走");
                }
            }

            // 元の形態に戻る？
            string normalModeData = FeatureData("ノーマルモード");
            if ((GeneralLib.LIndex(normalModeData, 1) ?? "") == (new_form ?? ""))
            {
                if (IsConditionSatisfied("ノーマルモード付加"))
                {
                    // 変身が解ける場合
                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        foreach (var opt in GeneralLib.ToL(normalModeData).Skip(1))
                        {
                            switch (opt ?? "")
                            {
                                case "消耗あり":
                                    AddCondition("消耗", 1, cdata: "");
                                    break;

                                case "気力低下":
                                    IncreaseMorale(-10);
                                    break;
                            }
                        }
                    }

                    DeleteCondition("ノーマルモード付加");
                    if (IsConditionSatisfied("能力コピー"))
                    {
                        DeleteCondition("能力コピー");
                        DeleteCondition("パイロット画像");
                        DeleteCondition("メッセージ");
                    }
                }
                // ハイパーモードが解ける場合
                else if (!Map.IsStatusView)
                {
                    AddCondition("消耗", 1, cdata: "");
                    foreach (var opt in GeneralLib.ToL(normalModeData).Skip(1))
                    {
                        switch (opt ?? "")
                        {
                            case "消耗なし":
                                DeleteCondition("消耗");
                                break;

                            case "気力低下":
                                IncreaseMorale(-10);
                                break;
                        }
                    }
                }

                if (IsConditionSatisfied("残り時間"))
                {
                    DeleteCondition("残り時間");
                }
            }

            // 戦闘アニメで変更されたユニット画像を元に戻す
            if (IsConditionSatisfied("ユニット画像"))
            {
                DeleteCondition("ユニット画像");
            }

            if (IsConditionSatisfied("非表示付加"))
            {
                DeleteCondition("非表示付加");
            }

            int counter;
            {
                // パラメータ受け継ぎ
                u.BossRank = BossRank;
                u.Rank = Rank;
                u.Mode = Mode;
                u.Area = Area;
                u.UsedSupportAttack = UsedSupportAttack;
                u.UsedSupportGuard = UsedSupportGuard;
                u.UsedSyncAttack = UsedSyncAttack;
                u.UsedCounterAttack = UsedCounterAttack;
                u.Master = Master;
                Master = null;
                u.Summoner = Summoner;
                Summoner = null;

                // アイテム受け継ぎ
                // TODO Impl アイテム受け継ぎ
                //var loopTo2 = CountItem();
                //for (i = 1; i <= loopTo2; i++)
                //{
                //    Item localItem() { object new_form = i; var ret = Item(new_form); return ret; }

                //    u.AddItem0(localItem());
                //}

                // スペシャルパワー効果のコピー
                CopySpecialPowerInEffect(u);
                RemoveAllSpecialPowerInEffect();

                // 特殊ステータスのコピー
                u.ClearCondition();
                foreach (var condition in Conditions
                    .Where(x => x.Lifetime > 0)
                    .Where(x => Strings.InStr(x.StrData, "パイロット能力付加") == 0)
                    .Where(x => Strings.InStr(x.StrData, "パイロット能力強化") == 0))
                {
                    u.AddCondition(condition.Name, condition.Lifetime, condition.Level, condition.StrData);
                }
                ClearCondition();

                // パイロットの乗せ換え
                var forms = GeneralLib.ToL(FeatureData("変形")).Skip(1).ToList();
                forms.Insert(0, Name);
                if (Data.PilotNum == -(forms.Count)
                    && CountPilot() == forms.Count)
                {
                    // XXX 動作確認しとらん、複数人乗りユニットまだ作れない
                    // 変形によりパイロットの順番が変化する場合
                    var tuForms = GeneralLib.ToL(u.FeatureData("変形")).Skip(1).ToList();
                    tuForms.Insert(0, u.Name);
                    if (forms.OrderBy(x => x).SequenceEqual(tuForms.OrderBy(x => x)))
                    {
                        tuForms.ForEach(tuForm =>
                        {
                            u.AddPilot(Pilots[forms.IndexOf(tuForm)]);
                        });
                    }
                    else
                    {
                        foreach (var p in Pilots)
                        {
                            u.AddPilot(p);
                        }
                    }
                }
                else
                {
                    foreach (var p in Pilots)
                    {
                        u.AddPilot(p);
                    }
                }

                foreach (var p in Supports)
                {
                    u.AddSupport(p);
                }


                foreach (var cu in UnitOnBoards)
                {
                    u.LoadUnit(cu);
                }

                //var loopTo13 = CountServant();
                //for (i = 1; i <= loopTo13; i++)
                //{
                //    Unit localServant() { object argIndex1 = i; var ret = Servant(argIndex1); return ret; }

                //    u.AddServant(localServant());
                //}

                //var loopTo14 = CountSlave();
                //for (i = 1; i <= loopTo14; i++)
                //{
                //    Unit localSlave() { object argIndex1 = i; var ret = Slave(argIndex1); return ret; }

                //    u.AddSlave(localSlave());
                //}

                colPilot.Clear();
                colSupport.Clear();
                colUnitOnBoard.Clear();
                //var loopTo15 = CountPilot();
                //for (i = 1; i <= loopTo15; i++)
                //{
                //    DeletePilot(1);
                //}

                //var loopTo16 = CountSupport();
                //for (i = 1; i <= loopTo16; i++)
                //{
                //    DeleteSupport(1);
                //}

                //var loopTo17 = CountUnitOnBoard();
                //for (i = 1; i <= loopTo17; i++)
                //{
                //    UnloadUnit(1);
                //}

                //var loopTo18 = CountServant();
                //for (i = 1; i <= loopTo18; i++)
                //{
                //    DeleteServant(1);
                //}

                //var loopTo19 = CountSlave();
                //for (i = 1; i <= loopTo19; i++)
                //{
                //    DeleteSlave(1);
                //}

                foreach (var p in u.Pilots)
                {
                    p.Unit = u;
                }

                // 合体ロボットの変形時に分離先のパイロットのインデックスを合わせる
                foreach (var p in u.Supports)
                {
                    if (p.SupportIndex > 0)
                    {
                        if (IsFeatureAvailable("分離") && u.IsFeatureAvailable("分離"))
                        {
                            var cuForms = GeneralLib.ToL(FeatureData("分離")).Skip(1).ToList();
                            var tuForms = GeneralLib.ToL(u.FeatureData("分離")).Skip(1).ToList();
                            var cuForm = cuForms[p.SupportIndex - 1];
                            for (var tuIndex = 0; tuForms.Count < tuIndex; tuIndex++)
                            {
                                var tuForm = tuForms[tuIndex];
                                if (tuForm == cuForm)
                                {
                                    p.SupportIndex = tuIndex + 1;
                                    break;
                                }
                            }
                        }
                    }
                }

                u.Update();

                // TODO Impl 弾数データを記録
                //// 弾数データを記録
                //wname = new string[(CountWeapon() + 1)];
                //wbullet = new int[(CountWeapon() + 1)];
                //wmaxbullet = new int[(CountWeapon() + 1)];
                //var loopTo23 = CountWeapon();
                //for (i = 1; i <= loopTo23; i++)
                //{
                //    wname[i] = Weapon(i).Name;
                //    wbullet[i] = Bullet(i);
                //    wmaxbullet[i] = MaxBullet(i);
                //}

                //aname = new string[(CountAbility() + 1)];
                //astock = new int[(CountAbility() + 1)];
                //amaxstock = new int[(CountAbility() + 1)];
                //var loopTo24 = CountAbility();
                //for (i = 1; i <= loopTo24; i++)
                //{
                //    aname[i] = Ability(i).Name;
                //    astock[i] = Stock(i);
                //    amaxstock[i] = MaxStock(i);
                //}

                //// 弾数の受け継ぎ
                //idx = 1;
                //var loopTo25 = u.CountWeapon();
                //for (i = 1; i <= loopTo25; i++)
                //{
                //    counter = idx;
                //    var loopTo26 = Information.UBound(wname);
                //    for (j = counter; j <= loopTo26; j++)
                //    {
                //        if ((u.Weapon(i).Name ?? "") == (wname[j] ?? "") && u.MaxBullet(i) > 0 && wmaxbullet[j] > 0)
                //        {
                //            u.SetBullet(i, ((wbullet[j] * u.MaxBullet(i)) / wmaxbullet[j]));
                //            idx = (j + 1);
                //            break;
                //        }
                //    }
                //}

                //idx = 1;
                //var loopTo27 = u.CountAbility();
                //for (i = 1; i <= loopTo27; i++)
                //{
                //    counter = idx;
                //    var loopTo28 = Information.UBound(aname);
                //    for (j = counter; j <= loopTo28; j++)
                //    {
                //        if ((u.Ability(i).Name ?? "") == (aname[j] ?? "") && u.MaxStock(i) > 0 && amaxstock[j] > 0)
                //        {
                //            u.SetStock(i, ((astock[j] * u.MaxStock(i)) / amaxstock[j]));
                //            idx = (j + 1);
                //            break;
                //        }
                //    }
                //}

                //// 弾数・使用回数共有の実現
                //u.SyncBullet();

                //// アイテムを削除
                //var loopTo29 = CountItem();
                //for (i = 1; i <= loopTo29; i++)
                //{
                //    DeleteItem(1);
                //}

                u.Update();

                // ＨＰ＆ＥＮの受け継ぎ
                if ((new_form ?? "") == (GeneralLib.LIndex(FeatureData("パーツ分離"), 2) ?? ""))
                {
                    u.HP = u.MaxHP;
                }
                else
                {
                    u.HP = (int)(u.MaxHP * hp_ratio / 100d);
                }

                u.EN = (int)(u.MaxEN * en_ratio / 100d);

                // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
                if (!u.IsConditionSatisfied("残り時間"))
                {
                    if (u.IsFeatureAvailable("ノーマルモード"))
                    {
                        var lastTime = GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 2);
                        if (Information.IsNumeric(lastTime))
                        {
                            if (u.IsConditionSatisfied("残り時間"))
                            {
                                u.DeleteCondition("残り時間");
                            }
                            u.AddCondition("残り時間", Conversions.ToInteger(lastTime), cdata: "");
                        }
                    }
                    else if (u.IsFeatureAvailable("制限時間"))
                    {
                        u.AddCondition("残り時間", Conversions.ToInteger(u.FeatureData("制限時間")), cdata: "");
                    }
                }
                else if (!u.IsFeatureAvailable("ノーマルモード") && !u.IsFeatureAvailable("制限時間"))
                {
                    // 残り時間が必要ない形態にTransformコマンドで強制変形された？
                    u.DeleteCondition("残り時間");
                }

                switch (u.Status ?? "")
                {
                    case "出撃":
                        // 変形後のユニットを出撃させる
                        Map.MapDataForUnit[x, y] = null;
                        var prev_x = x;
                        var prev_y = y;
                        u.UsedAction = UsedAction;
                        u.StandBy(x, y);
                        if (u.x != prev_x || u.y != prev_y)
                        {
                            GUI.EraseUnitBitmap(prev_x, prev_y, false);
                        }

                        break;

                    case "格納":
                        // 変形後のユニットを格納する
                        SRC.UList.SwapOnBardUnit(this, u);
                        break;
                }
            }

            if (Map.IsStatusView)
            {
                return;
            }

            // ハイパーモードが解ける場合
            foreach (var opt in GeneralLib.ToL(FeatureData("ノーマルモード")).Skip(1))
            {
                switch (opt ?? "")
                {
                    case "回数制限":
                        AddCondition("行動不能", -1, cdata: "");
                        break;
                }
            }
        }

        // 合体
        public void Combine(string uname = "", bool is_event = false)
        {
            Unit u;
            var prev_status = Status;
            FeatureData splitFeature = null;
            if (string.IsNullOrEmpty(uname))
            {
                // 合体形態が指定されてなければその場所にいるユニットと２体合体
                u = null;
                foreach (var fd in Features.Where(x => x.Name == "合体"))
                {
                    var combineData = new CombineFeature(fd);
                    if (combineData.PartUnitNames.Count == 1
                        && (Map.MapDataForUnit[x, y]?.IsEqual(combineData.PartUnitNames.First()) ?? false)
                        && SRC.UList.IsDefined(combineData.ConbineUnitName))
                    {
                        u = SRC.UList.Item(combineData.ConbineUnitName).CurrentForm();
                        break;
                    }
                }

                // 合体のパートナーを調べる
                foreach (var fd in u.Features.Where(x => x.Name == "分離"))
                {
                    if (fd.DataL.Count == 3
                        && (IsEqual(fd.DataL[1]) && Map.MapDataForUnit[x, y].IsEqual(fd.DataL[2])
                            || IsEqual(fd.DataL[2]) && Map.MapDataForUnit[x, y].IsEqual(fd.DataL[1])))
                    {
                        splitFeature = fd;
                        break;
                    }
                }
            }
            else
            {
                // 合体ユニットが作成されていない
                if (!SRC.UList.IsDefined(uname))
                {
                    GUI.ErrorMessage(uname + "が作成されていません");
                    SRC.ExitGame();
                }
                u = SRC.UList.Item(uname).CurrentForm();

                // 合体のパートナーを調べる
                foreach (var fd in u.Features.Where(x => x.Name == "分離"))
                {
                    if (fd.DataL.Count > 2)
                    {
                        splitFeature = fd;
                        break;
                    }
                }
            }

            // 合体するユニットの配列を作成
            if (splitFeature == null)
            {
                GUI.ErrorMessage(u.Name + "のデータに" + Name + "に対する分離指定がみつかりません。" + "書式を確認してください。");
                return;
            }
            var runits = new List<Unit>();
            foreach (var partuname in splitFeature.DataL.Skip(1))
            {
                if (!SRC.UList.IsDefined(partuname))
                {
                    GUI.ErrorMessage(partuname + "が作成されていません");
                    return;
                }
                runits.Add(SRC.UList.Item(partuname));
            }

            string BGM;
            if (!is_event)
            {
                if (Status == "出撃")
                {
                    // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                    if (u.IsFeatureAvailable("追加パイロット"))
                    {
                        var pname = u.FeatureData("追加パイロット");

                        if (!SRC.PList.IsDefined(pname))
                        {
                            if (!SRC.PDList.IsDefined(pname))
                            {
                                GUI.ErrorMessage(u.Name + "の追加パイロット「" + pname + "」のデータが見つかりません");
                                SRC.TerminateSRC();
                            }
                            SRC.PList.Add(pname, MainPilot().Level, Party0, gid: "");
                        }
                    }

                    // TODO Impl 出撃
                    //bool localIsMessageDefined1() { string argmain_situation = "合体(" + u.Name + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

                    //bool localIsMessageDefined2() { object argIndex1 = "合体"; string argmain_situation = "合体(" + FeatureName(argIndex1) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

                    //if (localIsMessageDefined1() || localIsMessageDefined2() || IsMessageDefined("合体"))
                    //{
                    //    if (IsFeatureAvailable("合体ＢＧＭ"))
                    //    {
                    //        var loopTo5 = CountFeature();
                    //        for (i = 1; i <= loopTo5; i++)
                    //        {
                    //            string localFeature2() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

                    //            string localFeatureData13() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                    //            string localLIndex8() { string arglist = hs8b8df815445f4c329f8d70f276cd46e5(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                    //            if (localFeature2() == "合体ＢＧＭ" && (localLIndex8() ?? "") == (u.Name ?? ""))
                    //            {
                    //                string localFeatureData11() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                    //                string localFeatureData12() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                    //                BGM = Sound.SearchMidiFile(Strings.Mid(localFeatureData11(), Strings.InStr(localFeatureData12(), " ") + 1));
                    //                if (Strings.Len(BGM) > 0)
                    //                {
                    //                    Sound.ChangeBGM(BGM);
                    //                    GUI.Sleep(500);
                    //                }

                    //                break;
                    //            }
                    //        }
                    //    }

                    GUI.OpenMessageForm(u1: null, u2: null);
                    if (IsMessageDefined("合体(" + u.Name + ")"))
                    {
                        PilotMessage("合体(" + u.Name + ")", msg_mode: "");
                    }
                    else if (IsMessageDefined("合体(" + FeatureName("合体") + ")"))
                    {
                        PilotMessage("合体(" + FeatureName("合体") + ")", msg_mode: "");
                    }
                    else
                    {
                        PilotMessage("合体", msg_mode: "");
                    }
                    GUI.CloseMessageForm();
                }
            }

            //// 分離ユニットと合体ユニットが同名の武器を持つ場合は弾数を累積するため
            //// このような武器の弾数を0にする
            //var loopTo6 = u.CountWeapon();
            //for (i = 1; i <= loopTo6; i++)
            //{
            //    var loopTo7 = Information.UBound(rarray);
            //    for (j = 1; j <= loopTo7; j++)
            //    {
            //        {
            //            var withBlock = rarray[j].CurrentForm();
            //            var loopTo8 = withBlock.CountWeapon();
            //            for (k = 1; k <= loopTo8; k++)
            //            {
            //                if ((u.Weapon(i).Name ?? "") == (withBlock.Weapon(k).Name ?? ""))
            //                {
            //                    u.SetBullet(i, 0);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
            //// 使用回数を合わせる
            //var loopTo9 = u.CountAbility();
            //for (i = 1; i <= loopTo9; i++)
            //{
            //    var loopTo10 = Information.UBound(rarray);
            //    for (j = 1; j <= loopTo10; j++)
            //    {
            //        {
            //            var withBlock1 = rarray[j].CurrentForm();
            //            var loopTo11 = withBlock1.CountAbility();
            //            for (k = 1; k <= loopTo11; k++)
            //            {
            //                if ((u.Ability(i).Name ?? "") == (withBlock1.Ability(k).Name ?? ""))
            //                {
            //                    u.SetStock(i, 0);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}

            // １番目のユニットのステータスを合体後のユニットに継承
            {
                var ru = runits.First().CurrentForm();
                ru.CopySpecialPowerInEffect(u);
                ru.RemoveAllSpecialPowerInEffect();
                //var loopTo12 = ru.CountItem();
                //for (i = 1; i <= loopTo12; i++)
                //{
                //    Item localItem3() { object argIndex1 = i; var ret = ru.Item(argIndex1); return ret; }

                //    u.AddItem(localItem3());
                //}

                u.Master = ru.Master;
                ru.Master = null;
                u.Summoner = ru.Summoner;
                ru.Summoner = null;
                u.UsedSupportAttack = ru.UsedSupportAttack;
                u.UsedSupportGuard = ru.UsedSupportGuard;
                u.UsedSyncAttack = ru.UsedSyncAttack;
                u.UsedCounterAttack = ru.UsedCounterAttack;
                //var loopTo13 = withBlock2.CountServant();
                //for (i = 1; i <= loopTo13; i++)
                //{
                //    Unit localServant() { object argIndex1 = i; var ret = withBlock2.Servant(argIndex1); return ret; }

                //    u.AddServant(localServant());
                //}

                //var loopTo14 = withBlock2.CountServant();
                //for (i = 1; i <= loopTo14; i++)
                //{
                //    withBlock2.DeleteServant(1);
                //}

                //var loopTo15 = withBlock2.CountSlave();
                //for (i = 1; i <= loopTo15; i++)
                //{
                //    Unit localSlave() { object argIndex1 = i; var ret = withBlock2.Slave(argIndex1); return ret; }

                //    u.AddSlave(localSlave());
                //}

                //var loopTo16 = withBlock2.CountSlave();
                //for (i = 1; i <= loopTo16; i++)
                //{
                //    withBlock2.DeleteSlave(1);
                //}
            }

            // 合体する各ユニットに対しての処理を行う
            var i = 0;
            var hp_ratio = 0d;
            var en_ratio = 0d;
            foreach (var ru in runits)
            {
                i++;
                // マップ上から撤退させる
                var currentForm = ru.CurrentForm();
                switch (currentForm.Status ?? "")
                {
                    case "出撃":
                        currentForm.Status = "待機";
                        Map.MapDataForUnit[currentForm.x, currentForm.y] = null;
                        GUI.EraseUnitBitmap(currentForm.x, currentForm.y);
                        break;

                    case "格納":
                        currentForm.Status = "待機";
                        SRC.UList.Unload(ru);
                        break;
                }

                // デフォルトの形態に変形させておく
                if (!ReferenceEquals(currentForm, ru))
                {
                    ru.CurrentForm().Transform(ru.Name);
                }

                if (i == 1)
                {
                    ru.Status = "旧主形態";
                }
                else
                {
                    ru.Status = "旧形態";
                }

                hp_ratio = hp_ratio + 100 * ru.HP / (double)ru.MaxHP;
                en_ratio = en_ratio + 100 * ru.EN / (double)ru.MaxEN;
                if (ru.Rank > u.Rank)
                {
                    u.Rank = ru.Rank;
                }

                if (ru.BossRank > u.BossRank)
                {
                    u.BossRank = ru.BossRank;
                    u.FullRecover();
                }

                if (ru.IsFeatureAvailable("召喚ユニット"))
                {
                    // 召喚ユニットの場合はパイロットの乗せ換えは行わない
                    if (Strings.InStr(ru.MainPilot().Name, "(ザコ)") > 0 || Strings.InStr(ru.MainPilot().Name, "(汎用)") > 0)
                    {
                        // 汎用パイロットの場合は削除
                        ru.MainPilot().Alive = false;
                    }
                }
                else
                {
                    // パイロットの乗せ換え
                    foreach (var p in ru.Pilots)
                    {
                        p.Ride(u);
                    }
                    ru.colPilot.Clear();

                    // サポートの乗せ換え
                    foreach (var p in ru.Supports)
                    {
                        p.Ride(u, true);
                        p.SupportIndex = i;
                    }
                    ru.colSupport.Clear();
                }

                // 搭載ユニットの乗せ換え
                foreach (var lu in UnitOnBoards.AsEnumerable().ToList())
                {
                    u.LoadUnit(lu);
                    UnloadUnit(lu.ID);
                }

                //// 分離ユニットと共通する武装の弾数は一旦0にクリア
                //var loopTo25 = u.CountWeapon();
                //for (j = 1; j <= loopTo25; j++)
                //{
                //    var loopTo26 = ru.CountWeapon();
                //    for (k = 1; k <= loopTo26; k++)
                //    {
                //        if ((u.Weapon(j).Name ?? "") == (ru.Weapon(k).Name ?? ""))
                //        {
                //            u.SetBullet(j, 0);
                //            break;
                //        }
                //    }

                //    var loopTo27 = ru.CountOtherForm();
                //    for (k = 1; k <= loopTo27; k++)
                //    {
                //        {
                //            var withBlock5 = ru.OtherForm(k);
                //            var loopTo28 = withBlock5.CountWeapon();
                //            for (l = 1; l <= loopTo28; l++)
                //            {
                //                if ((u.Weapon(j).Name ?? "") == (withBlock5.Weapon(l).Name ?? ""))
                //                {
                //                    u.SetBullet(j, 0);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                //// アビリティの使用回数も同様の処理を行う
                //var loopTo29 = u.CountAbility();
                //for (j = 1; j <= loopTo29; j++)
                //{
                //    var loopTo30 = ru.CountAbility();
                //    for (k = 1; k <= loopTo30; k++)
                //    {
                //        if ((u.Ability(j).Name ?? "") == (ru.Ability(k).Name ?? ""))
                //        {
                //            u.SetStock(j, 0);
                //            break;
                //        }
                //    }

                //    var loopTo31 = ru.CountOtherForm();
                //    for (k = 1; k <= loopTo31; k++)
                //    {
                //        {
                //            var withBlock6 = ru.OtherForm(k);
                //            var loopTo32 = withBlock6.CountAbility();
                //            for (l = 1; l <= loopTo32; l++)
                //            {
                //                if ((u.Ability(j).Name ?? "") == (withBlock6.Ability(l).Name ?? ""))
                //                {
                //                    u.SetStock(j, 0);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                // スペシャルパワーの効果を消去
                ru.RemoveAllSpecialPowerInEffect();
            }

            //// 合体後のユニットの武装の弾数及びアビリティの使用回数は分離ユニットの
            //// 弾数及び使用回数の合計に設定する
            //var loopTo33 = Information.UBound(rarray);
            //for (i = 1; i <= loopTo33; i++)
            //{
            //    {
            //        var withBlock7 = rarray[i];
            //        // 武装の弾数の処理
            //        var loopTo34 = u.CountWeapon();
            //        for (j = 1; j <= loopTo34; j++)
            //        {
            //            var loopTo35 = withBlock7.CountWeapon();
            //            for (k = 1; k <= loopTo35; k++)
            //            {
            //                if ((u.Weapon(j).Name ?? "") == (withBlock7.Weapon(k).Name ?? ""))
            //                {
            //                    u.SetBullet(j, (u.Bullet(j) + withBlock7.Bullet(k)));
            //                    goto NextWeapon;
            //                }
            //            }

            //            var loopTo36 = withBlock7.CountOtherForm();
            //            for (k = 1; k <= loopTo36; k++)
            //            {
            //                {
            //                    var withBlock8 = withBlock7.OtherForm(k);
            //                    var loopTo37 = withBlock8.CountWeapon();
            //                    for (l = 1; l <= loopTo37; l++)
            //                    {
            //                        if ((u.Weapon(j).Name ?? "") == (withBlock8.Weapon(l).Name ?? ""))
            //                        {
            //                            u.SetBullet(j, (u.Bullet(j) + withBlock8.Bullet(l)));
            //                            goto NextWeapon;
            //                        }
            //                    }
            //                }
            //            }

            //        NextWeapon:
            //            ;
            //        }

            //        // アビリティの使用回数の処理
            //        var loopTo38 = u.CountAbility();
            //        for (j = 1; j <= loopTo38; j++)
            //        {
            //            var loopTo39 = withBlock7.CountAbility();
            //            for (k = 1; k <= loopTo39; k++)
            //            {
            //                if ((u.Ability(j).Name ?? "") == (withBlock7.Ability(k).Name ?? ""))
            //                {
            //                    u.SetStock(j, (u.Stock(j) + withBlock7.Stock(k)));
            //                    goto NextAbility;
            //                }
            //            }

            //            var loopTo40 = withBlock7.CountOtherForm();
            //            for (k = 1; k <= loopTo40; k++)
            //            {
            //                {
            //                    var withBlock9 = withBlock7.OtherForm(k);
            //                    var loopTo41 = withBlock9.CountAbility();
            //                    for (l = 1; l <= loopTo41; l++)
            //                    {
            //                        if ((u.Ability(j).Name ?? "") == (withBlock9.Ability(l).Name ?? ""))
            //                        {
            //                            u.SetStock(j, (u.Stock(j) + withBlock9.Stock(l)));
            //                            goto NextAbility;
            //                        }
            //                    }
            //                }
            //            }

            //        NextAbility:
            //            ;
            //        }
            //    }
            //}

            //// １番目のユニットのアイテムを外す
            //{
            //    var withBlock10 = rarray[1];
            //    var loopTo42 = withBlock10.CountItem();
            //    for (i = 1; i <= loopTo42; i++)
            //    {
            //        withBlock10.DeleteItem(1);
            //    }
            //}

            // 合体後のユニットに関する処理
            u.Update();
            u.Party = Party0;
            foreach (var of in u.OtherForms)
            {
                of.Party = Party0;
            }
            foreach (var p in u.Pilots)
            {
                p.Party = Party0;
            }
            foreach (var p in u.Supports)
            {
                p.Party = Party0;
            }

            //u.HP = (int)(u.MaxHP * hp_ratio / 100d / Information.UBound(rarray));
            //u.EN = (int)(1 * u.MaxEN * en_ratio / 100d / Information.UBound(rarray));

            //// 弾数・使用回数共有の実現
            //u.SyncBullet();
            if (prev_status == "出撃")
            {
                u.StandBy(x, y);

                // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
                if (u.IsFeatureAvailable("ノーマルモード"))
                {
                    var lastTime = GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 2);
                    if (Information.IsNumeric(lastTime))
                    {
                        if (u.IsConditionSatisfied("残り時間"))
                        {
                            u.DeleteCondition("残り時間");
                        }
                        u.AddCondition("残り時間", Conversions.ToInteger(lastTime), cdata: "");
                    }
                }
                else if (u.IsFeatureAvailable("制限時間"))
                {
                    u.AddCondition("残り時間", Conversions.ToInteger(u.FeatureData("制限時間")), cdata: "");
                }
            }
            else
            {
                u.Status = prev_status;
            }

            // 分離ユニットの座標を合体後のユニットの座標に合わせる
            foreach (var pu in runits)
            {

                var cf = pu.CurrentForm();
                cf.x = u.x;
                cf.y = u.y;
            }
        }

        // 分離
        public void Split()
        {
            string pname;
            var hp_ratio = 100 * HP / (double)MaxHP;
            var en_ratio = 100 * EN / (double)MaxEN;

            // まずは撤退
            if (Status == "出撃")
            {
                Map.MapDataForUnit[x, y] = null;
                GUI.EraseUnitBitmap(x, y);
            }

            // 分離先のユニットを調べる
            var uarray = new List<Unit>();
            foreach (var splitForm in GeneralLib.ToL(FeatureData("分離")).Skip(1).ToList())
            {
                var u = SRC.UList.Item(splitForm);
                if (u == null)
                {
                    GUI.ErrorMessage(splitForm + "が存在しません");
                    return;
                }
                uarray.Add(u);
            }

            // 分離後の１番機を検索
            var firstUnit = uarray.FirstOrDefault(x => x.Status == "旧主形態") ?? uarray.First();

            // １番機に現在のステータスを継承
            CopySpecialPowerInEffect(firstUnit);
            RemoveAllSpecialPowerInEffect();
            {
                // TODO Impl 変形と共有できる？
                //var loopTo2 = CountItem();
                //for (j = 1; j <= loopTo2; j++)
                //{
                //    Item localItem() { object argIndex1 = j; var ret = Item(argIndex1); return ret; }

                //    firstUnit.AddItem(localItem());
                //}

                //var loopTo3 = CountItem();
                //for (j = 1; j <= loopTo3; j++)
                //{
                //    DeleteItem(1);
                //}

                firstUnit.Master = Master;
                Master = null;
                firstUnit.Summoner = Summoner;
                Summoner = null;
                firstUnit.UsedSupportAttack = UsedSupportAttack;
                firstUnit.UsedSupportGuard = UsedSupportGuard;
                firstUnit.UsedSyncAttack = UsedSyncAttack;
                firstUnit.UsedCounterAttack = UsedCounterAttack;
                //var loopTo4 = CountServant();
                //for (j = 1; j <= loopTo4; j++)
                //{
                //    Unit localServant() { object argIndex1 = j; var ret = Servant(argIndex1); return ret; }

                //    firstUnit.AddServant(localServant());
                //}

                //var loopTo5 = CountServant();
                //for (j = 1; j <= loopTo5; j++)
                //{
                //    DeleteServant(1);
                //}

                //var loopTo6 = CountSlave();
                //for (j = 1; j <= loopTo6; j++)
                //{
                //    Unit localSlave() { object argIndex1 = j; var ret = Slave(argIndex1); return ret; }

                //    firstUnit.AddSlave(localSlave());
                //}

                //var loopTo7 = CountSlave();
                //for (j = 1; j <= loopTo7; j++)
                //{
                //    DeleteSlave(1);
                //}
            }

            // 各分離ユニットに対する処理
            //n = 1;
            //int counter;
            var pilotIndex = 0;
            foreach (var u in uarray)
            {
                // 召喚ユニットでない場合は陣営を合わせる
                if (!u.IsFeatureAvailable("召喚ユニット"))
                {
                    u.Party = Party0;
                }

                // パイロットの搭乗
                if (CountPilot() > 0)
                {
                    for (var j = 1; j <= Math.Abs(u.Data.PilotNum); j++)
                    {
                        Pilot p;
                        if (u.IsFeatureAvailable("召喚ユニット"))
                        {
                            if (Status == "出撃" || Status == "格納")
                            {
                                pname = u.FeatureData("追加パイロット");
                                var addPilot = SRC.PDList.Item(pname);

                                if (Strings.InStr(addPilot.Name, "(ザコ)") > 0 || Strings.InStr(addPilot.Name, "(汎用)") > 0)
                                {
                                    p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
                                    p.FullRecover();
                                }
                                else
                                {
                                    if (!SRC.PList.IsDefined(pname))
                                    {
                                        p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
                                        p.FullRecover();
                                    }
                                    else
                                    {
                                        p = SRC.PList.Item(pname);
                                    }
                                }
                                p.Ride(u);
                            }
                        }
                        else
                        {
                            if (pilotIndex < CountPilot())
                            {
                                Pilots[pilotIndex].Ride(u);
                                pilotIndex = pilotIndex + 1;
                            }
                            else if (!u.IsFeatureAvailable("追加パイロット"))
                            {
                                if (CountSupport() > 0)
                                {
                                    Supports.First().Ride(u);
                                    DeleteSupport(Supports.First());
                                }
                                else
                                {
                                    GUI.ErrorMessage(Name + "分離後のユニットに載せる" + "パイロットが存在しません。" + "データのパイロット数を確認して下さい。");
                                    SRC.TerminateSRC();
                                }
                            }
                        }
                    }
                }

                u.Update();

                // 母艦の場合は格納したユニットを受け渡し
                if (u.IsFeatureAvailable("母艦"))
                {
                    // XXX 列挙子の複製どうすんだっけ
                    foreach (var lu in UnitOnBoards.AsEnumerable().ToList())
                    {
                        u.LoadUnit(lu);
                        UnloadUnit(lu.ID);
                    }
                }

                // ＨＰ＆ＥＮの同期
                u.HP = (int)(u.MaxHP * hp_ratio / 100d);
                u.EN = (int)(1 * u.MaxEN * en_ratio / 100d);

                //// 弾数を合わせる
                //idx = 1;
                //var loopTo12 = CountWeapon();
                //for (j = 1; j <= loopTo12; j++)
                //{
                //    counter = idx;
                //    var loopTo13 = u.CountWeapon();
                //    for (k = counter; k <= loopTo13; k++)
                //    {
                //        if ((Weapon(j).Name ?? "") == (u.Weapon(k).Name ?? "") && this.Weapon(j).Bullet > 0 && u.Weapon(k).Bullet > 0)
                //        {
                //            u.SetBullet(k, ((u.MaxBullet(k) * Bullet(j)) / MaxBullet(j)));
                //            idx = (k + 1);
                //            break;
                //        }
                //    }
                //}

                //var loopTo14 = u.CountOtherForm();
                //for (j = 1; j <= loopTo14; j++)
                //{
                //    {
                //        var withBlock2 = u.OtherForm(j);
                //        idx = 1;
                //        var loopTo15 = CountWeapon();
                //        for (k = 1; k <= loopTo15; k++)
                //        {
                //            counter = idx;
                //            var loopTo16 = withBlock2.CountWeapon();
                //            for (l = counter; l <= loopTo16; l++)
                //            {
                //                if ((Weapon(k).Name ?? "") == (withBlock2.Weapon(l).Name ?? "") && this.Weapon(k).Bullet > 0 && withBlock2.Weapon(l).Bullet > 0)
                //                {
                //                    withBlock2.SetBullet(l, ((withBlock2.MaxBullet(l) * Bullet(k)) / MaxBullet(k)));
                //                    idx = (l + 1);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                //// 使用回数を合わせる
                //idx = 1;
                //var loopTo17 = CountAbility();
                //for (j = 1; j <= loopTo17; j++)
                //{
                //    counter = idx;
                //    var loopTo18 = u.CountAbility();
                //    for (k = counter; k <= loopTo18; k++)
                //    {
                //        if ((Ability(j).Name ?? "") == (u.Ability(k).Name ?? "") && this.Ability(j).Stock > 0 && u.Ability(k).Stock > 0)
                //        {
                //            u.SetStock(k, ((u.Ability(k).Stock * Stock(j)) / MaxStock(j)));
                //            idx = (k + 1);
                //            break;
                //        }
                //    }
                //}

                //var loopTo19 = u.CountOtherForm();
                //for (j = 1; j <= loopTo19; j++)
                //{
                //    {
                //        var withBlock3 = u.OtherForm(j);
                //        idx = 1;
                //        var loopTo20 = CountAbility();
                //        for (k = 1; k <= loopTo20; k++)
                //        {
                //            counter = idx;
                //            var loopTo21 = withBlock3.CountAbility();
                //            for (l = counter; l <= loopTo21; l++)
                //            {
                //                if ((Ability(k).Name ?? "") == (withBlock3.Ability(l).Name ?? "") && this.Ability(k).Stock > 0 && withBlock3.Ability(l).Stock > 0)
                //                {
                //                    withBlock3.SetStock(l, ((withBlock3.Ability(l).Stock * Stock(k)) / MaxStock(k)));
                //                    idx = (l + 1);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                //// 弾数・使用回数共有の実現
                //u.SyncBullet();

                // 出撃 or 格納？
                u.Status = Status;
                switch (Status ?? "")
                {
                    case "出撃":
                        // XXX Unit同士の比較の標準化
                        if (u == firstUnit)
                        {
                            u.UsedAction = UsedAction;
                        }
                        else
                        {
                            u.UsedAction = GeneralLib.MaxLng(UsedAction, u.UsedAction);
                            u.UsedSupportAttack = 0;
                            u.UsedSupportGuard = 0;
                            u.UsedSyncAttack = 0;
                            u.UsedCounterAttack = 0;
                        }
                        u.StandBy(x, y);
                        break;

                    case "格納":
                        SRC.UList.LoadSameUnit(this, u);
                        break;
                }

                // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
                if (u.IsFeatureAvailable("ノーマルモード"))
                {
                    var lastTime = GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 2);
                    if (Information.IsNumeric(lastTime))
                    {
                        if (u.IsConditionSatisfied("残り時間"))
                        {
                            u.DeleteCondition("残り時間");
                        }
                        u.AddCondition("残り時間", Conversions.ToInteger(lastTime), cdata: "");
                    }
                }
                else if (u.IsFeatureAvailable("制限時間"))
                {
                    u.AddCondition("残り時間", Conversions.ToInteger(u.FeatureData("制限時間")), cdata: "");
                }
            }

            // パイロットを合体ユニットから削除
            colPilot.Clear();

            // サポートパイロットの乗り換え
            foreach (var sup in Supports)
            {
                if (sup.SupportIndex == 0)
                {
                    // XXX 要は先頭に乗る、だろ？
                    //Unit localItem3() { object argIndex1 = GeneralLib.LIndex(buf, 2); 
                    //    var ret = SRC.UList.Item(argIndex1); return ret; }

                    sup.Ride(uarray.First());
                }
                else
                {
                    sup.Ride(uarray[sup.SupportIndex - 1]);
                }
            }
            colSupport.Clear();

            // 格納されている場合は母艦から自分のエントリーを外しておく
            if (Status == "格納")
            {
                SRC.UList.Unload(this);
            }

            Status = "他形態";

            // ユニットステータスコマンドの場合以外は制限時間付き合体ユニットは
            // ２度とその形態を利用できない
            if (Map.IsStatusView)
            {
                return;
            }

            if (IsFeatureAvailable("制限時間"))
            {
                AddCondition("行動不能", -1, cdata: "");
            }
        }
    }
}
