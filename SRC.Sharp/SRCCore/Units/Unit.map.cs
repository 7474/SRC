using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

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
                                    string argarea_name = "空";
                                    if (!IsTransAvailable(argarea_name))
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case "水":
                                {
                                    string argarea_name1 = "水上";
                                    string argarea_name2 = "空";
                                    if (!IsTransAvailable(argarea_name1) && !IsTransAvailable(argarea_name2) & get_Adaption(3) == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case "深水":
                                {
                                    string argarea_name3 = "水上";
                                    string argarea_name4 = "空";
                                    string argarea_name5 = "水";
                                    if (!IsTransAvailable(argarea_name3) & !IsTransAvailable(argarea_name4) & !IsTransAvailable(argarea_name5))
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
            if (x == 0 & y == 0)
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

            // TODO Impl
            //var loopTo5 = CountUnitOnBoard();
            //for (i = 1; i <= loopTo5; i++)
            //{
            //    object argIndex2 = i;
            //    {
            //        var withBlock1 = UnitOnBoard(argIndex2);
            //        withBlock1.x = x;
            //        withBlock1.y = y;
            //    }
            //}

            //// 格納されていた場合はあらかじめ降ろしておく
            //if (Status_Renamed == "格納")
            //{
            //    foreach (Unit u in SRC.UList)
            //    {
            //        var loopTo6 = u.CountUnitOnBoard();
            //        for (i = 1; i <= loopTo6; i++)
            //        {
            //            Unit localUnitOnBoard() { object argIndex1 = i; var ret = u.UnitOnBoard(argIndex1); return ret; }

            //            if ((ID ?? "") == (localUnitOnBoard().ID ?? ""))
            //            {
            //                object argIndex3 = ID;
            //                u.UnloadUnit(argIndex3);
            //                goto EndLoop;
            //            }
            //        }
            //    }

            //EndLoop:
            //    ;
            //}

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
                        if (IsTransAvailable("地中") & Area == "地中")
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

            // TODO Impl
            //// ビットマップを作成
            //if (BitmapID == 0)
            //{
            //    var argu = this;
            //    BitmapID = GUI.MakeUnitBitmap(argu);
            //}

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
            //    string argwave_name = "UnitOn.wav";
            //    Sound.PlayWave(argwave_name);

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

            //    string argfname1 = SRC.AppPath + fname + "1.bmp";
            //    if (GeneralLib.FileExists(argfname1))
            //    {
            //        // アニメ表示開始時刻を記録
            //        start_time = GeneralLib.timeGetTime();
            //        for (i = 1; i <= 4; i++)
            //        {
            //            // 画像を透過表示
            //            string argfname = fname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + ".bmp";
            //            string argdraw_option = "透過";
            //            if (GUI.DrawPicture(argfname, GUI.MapToPixelX(x), GUI.MapToPixelY(y), 32, 32, 0, 0, 0, 0, argdraw_option) == false)
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
            //if (!GUI.IsPictureVisible & !string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // MOD START MARGE
            //    // If smode = "非同期" Then
            //    if (Strings.InStr(smode, "非同期") > 0)
            //    {
            //        // MOD END MARGE
            //        var argu1 = this;
            //        GUI.PaintUnitBitmap(argu1, "リフレッシュ無し");
            //    }
            //    else
            //    {
            //        var argu2 = this;
            //        GUI.PaintUnitBitmap(argu2);
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
            // TODO 未実装多いよ
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

            //    SRC.PList.UpdateSupportMod(this);

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

            //    var loopTo1 = CountUnitOnBoard();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        object argIndex2 = i;
            //        {
            //            var withBlock1 = UnitOnBoard(argIndex2);
            //            withBlock1.x = x;
            //            withBlock1.y = y;
            //        }
            //    }

            //    // 指定された場所に既にユニットが存在？
            //    if (Map.MapDataForUnit[x, y] is object)
            //    {
            //        {
            //            var withBlock2 = Map.MapDataForUnit[x, y];
            //            // 合体？
            //            var loopTo2 = withBlock2.CountFeature();
            //            for (i = 1; i <= loopTo2; i++)
            //            {
            //                string localFeature() { object argIndex1 = i; var ret = withBlock2.Feature(argIndex1); return ret; }

            //                string localFeatureData2() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                int localLLength() { string arglist = hsb631fea4c5cf49098946ae0a91f0346e(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                if (localFeature() == "合体" & localLLength() == 3)
            //                {
            //                    string localFeatureData1() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                    string localLIndex1() { string arglist = hsb6975299bb8a44cda80cb8aa733a682c(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                    object argIndex3 = localLIndex1();
            //                    if (SRC.UList.IsDefined(argIndex3))
            //                    {
            //                        string localFeatureData() { object argIndex1 = i; var ret = withBlock2.FeatureData(argIndex1); return ret; }

            //                        string localLIndex() { string arglist = hs429ff88444314337bcbc774b8db33f1c(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                        Unit localItem() { object argIndex1 = (object)hs60134dcbf45946b48db157b192860c4e(); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                        if (ReferenceEquals(localItem().CurrentForm(), this))
            //                        {
            //                            string arguname = "";
            //                            Combine(uname: arguname);
            //                            return;
            //                        }
            //                    }
            //                }
            //            }

            //            // 着艦？
            //            string argfname = "母艦";
            //            if (!withBlock2.IsFeatureAvailable(argfname))
            //            {
            //                string argmsg = "合体元ユニット「" + Name + "」が複数あるため合体処理が出来ません";
            //                GUI.ErrorMessage(argmsg);
            //                return;
            //            }
            //        }

            //        // 着艦処理
            //        Land(Map.MapDataForUnit[x, y], by_cancel);
            //        return;
            //    }

            //    // 移動先によるユニット位置変更
            //    switch (Map.TerrainClass(x, y) ?? "")
            //    {
            //        case "空":
            //            {
            //                Area = "空中";
            //                break;
            //            }

            //        case "陸":
            //        case "屋内":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    case "水中":
            //                    case "水上":
            //                        {
            //                            Area = "地上";
            //                            break;
            //                        }

            //                    case "宇宙":
            //                        {
            //                            string argarea_name = "空";
            //                            string argarea_name1 = "陸";
            //                            if (IsTransAvailable(argarea_name) & get_Adaption(1) >= get_Adaption(2))
            //                            {
            //                                Area = "空中";
            //                            }
            //                            else if (IsTransAvailable(argarea_name1))
            //                            {
            //                                Area = "地上";
            //                            }
            //                            else
            //                            {
            //                                Area = "空中";
            //                            }

            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "月面":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    // 変更なし
            //                    case "地上":
            //                    case "地中":
            //                        {
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            string argarea_name2 = "空";
            //                            string argarea_name3 = "宇宙";
            //                            string argarea_name4 = "陸";
            //                            if ((IsTransAvailable(argarea_name2) | IsTransAvailable(argarea_name3)) & get_Adaption(4) >= get_Adaption(2))
            //                            {
            //                                Area = "宇宙";
            //                            }
            //                            else if (IsTransAvailable(argarea_name4))
            //                            {
            //                                Area = "地上";
            //                            }
            //                            else
            //                            {
            //                                Area = "宇宙";
            //                            }

            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "水":
            //        case "深水":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    case "地上":
            //                        {
            //                            string argarea_name5 = "水上";
            //                            if (IsTransAvailable(argarea_name5))
            //                            {
            //                                Area = "水上";
            //                            }
            //                            else
            //                            {
            //                                Area = "水中";
            //                            }

            //                            break;
            //                        }

            //                    case "宇宙":
            //                        {
            //                            Area = "水中";
            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "宇宙":
            //            {
            //                Area = "宇宙";
            //                break;
            //            }
            //    }

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
            //    SRC.PList.UpdateSupportMod(this);
        }

        // ユニットを(new_x,new_y)にジャンプ
        public void Jump(int new_x, int new_y, bool do_refresh = true)
        {
            // TODO Impl
            //    int j, i, k;

            // ユニットを一旦マップから削除
            Map.MapDataForUnit[x, y] = null;
            GUI.EraseUnitBitmap(x, y, do_refresh);
            SRC.PList.UpdateSupportMod(this);

            //    // 空き位置を検索
            var prev_x = x;
            var prev_y = y;
            x = new_x;
            y = new_y;
            foreach (var of in OtherForms)
            {
                of.x = x;
                of.y = y;
            }
            //    for (i = 0; i <= 10; i++)
            //    {
            //        var loopTo = GeneralLib.MinLng(new_x + i, Map.MapWidth);
            //        for (j = GeneralLib.MaxLng(new_x - i, 1); j <= loopTo; j++)
            //        {
            //            var loopTo1 = GeneralLib.MinLng(new_y + i, Map.MapHeight);
            //            for (k = GeneralLib.MaxLng(new_y - i, 1); k <= loopTo1; k++)
            //            {
            //                if ((Math.Abs((new_x - j)) + Math.Abs((new_y - k))) != i)
            //                {
            //                    goto NextLoop;
            //                }

            //                if (Map.MapDataForUnit[j, k] is object)
            //                {
            //                    goto NextLoop;
            //                }

            //                if (Map.TerrainMoveCost(j, k) > 100)
            //                {
            //                    goto NextLoop;
            //                }

            //                switch (Map.TerrainClass(j, k) ?? "")
            //                {
            //                    case "空":
            //                        {
            //                            string argarea_name = "空";
            //                            if (!IsTransAvailable(argarea_name))
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "水":
            //                    case "深水":
            //                        {
            //                            string argarea_name1 = "水上";
            //                            string argarea_name2 = "空";
            //                            if (!IsTransAvailable(argarea_name1) & !IsTransAvailable(argarea_name2) & get_Adaption(3) == 0)
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            break;
            //                        }
            //                }

            //                x = j;
            //                y = k;
            //                goto ExitFor;
            //            NextLoop:
            //                ;
            //            }
            //        }
            //    }

            //ExitFor:
            //    ;


            //    // 他の形態と格納したユニットの座標を更新
            //    var loopTo2 = CountOtherForm();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        object argIndex1 = i;
            //        {
            //            var withBlock = OtherForm(argIndex1);
            //            withBlock.x = x;
            //            withBlock.y = y;
            //        }
            //    }

            //    var loopTo3 = CountUnitOnBoard();
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        object argIndex2 = i;
            //        {
            //            var withBlock1 = UnitOnBoard(argIndex2);
            //            withBlock1.x = x;
            //            withBlock1.y = y;
            //        }
            //    }

            //    // 移動先によるユニット位置変更
            //    switch (Map.TerrainClass(x, y) ?? "")
            //    {
            //        case "空":
            //            {
            //                Area = "空中";
            //                break;
            //            }

            //        case "陸":
            //        case "屋内":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    case "水中":
            //                    case "水上":
            //                        {
            //                            Area = "地上";
            //                            break;
            //                        }

            //                    case "宇宙":
            //                        {
            //                            string argarea_name3 = "空";
            //                            string argarea_name4 = "陸";
            //                            if (IsTransAvailable(argarea_name3) & get_Adaption(1) >= get_Adaption(2))
            //                            {
            //                                Area = "空中";
            //                            }
            //                            else if (IsTransAvailable(argarea_name4))
            //                            {
            //                                Area = "地上";
            //                            }
            //                            else
            //                            {
            //                                Area = "空中";
            //                            }

            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "月面":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    // 変更なし
            //                    case "地上":
            //                    case "地中":
            //                        {
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            string argarea_name5 = "空";
            //                            string argarea_name6 = "宇宙";
            //                            string argarea_name7 = "陸";
            //                            if ((IsTransAvailable(argarea_name5) | IsTransAvailable(argarea_name6)) & get_Adaption(4) >= get_Adaption(2))
            //                            {
            //                                Area = "宇宙";
            //                            }
            //                            else if (IsTransAvailable(argarea_name7))
            //                            {
            //                                Area = "地上";
            //                            }
            //                            else
            //                            {
            //                                Area = "宇宙";
            //                            }

            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "水":
            //        case "深水":
            //            {
            //                switch (Area ?? "")
            //                {
            //                    case "地上":
            //                        {
            //                            string argarea_name8 = "水上";
            //                            if (IsTransAvailable(argarea_name8))
            //                            {
            //                                Area = "水上";
            //                            }
            //                            else
            //                            {
            //                                Area = "水中";
            //                            }

            //                            break;
            //                        }

            //                    case "宇宙":
            //                        {
            //                            Area = "水中";
            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case "宇宙":
            //            {
            //                Area = "宇宙";
            //                break;
            //            }
            //    }

            // マップにユニットを登録
            Map.MapDataForUnit[x, y] = this;

            // 情報更新
            Update();
            SRC.PList.UpdateSupportMod(this);

            // ユニット描画
            if (do_refresh)
            {
                var argu = this;
                GUI.PaintUnitBitmap(argu);
            }
        }

        // マップ上から脱出
        public void Escape(string smode = "")
        {
            //    Unit u;
            //    int i, j;

            //    // 母艦に乗っていた場合は降りておく
            //    if (Status_Renamed == "格納")
            //    {
            //        foreach (Unit currentU in SRC.UList)
            //        {
            //            u = currentU;
            //            var loopTo = u.CountUnitOnBoard();
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                Unit localUnitOnBoard() { object argIndex1 = i; var ret = u.UnitOnBoard(argIndex1); return ret; }

            //                if ((ID ?? "") == (localUnitOnBoard().ID ?? ""))
            //                {
            //                    object argIndex1 = ID;
            //                    u.UnloadUnit(argIndex1);
            //                    goto EndLoop;
            //                }
            //            }
            //        }

            //    EndLoop:
            //        ;
            //    }

            //    // 出撃している場合は画面上からユニットを消去
            //    if (Status_Renamed == "出撃" | Status_Renamed == "破壊")
            //    {
            //        if (ReferenceEquals(Map.MapDataForUnit[x, y], this))
            //        {
            //            // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            Map.MapDataForUnit[x, y] = null;
            //            if (smode == "非同期" | GUI.IsPictureVisible | string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                GUI.EraseUnitBitmap(x, y, false);
            //            }
            //            else
            //            {
            //                GUI.EraseUnitBitmap(x, y, true);
            //            }

            //            SRC.PList.UpdateSupportMod(this);
            //        }
            //    }

            //    if (Status_Renamed == "出撃" | Status_Renamed == "格納")
            //    {
            //        Status_Renamed = "待機";
            //    }

            //    // 破壊をキャンセル状態は解除
            //    object argIndex3 = "破壊キャンセル";
            //    if (IsConditionSatisfied(argIndex3))
            //    {
            //        object argIndex2 = "破壊キャンセル";
            //        DeleteCondition(argIndex2);
            //    }

            //    // ユニットを格納していたら降ろす
            //    foreach (Unit currentU1 in colUnitOnBoard)
            //    {
            //        u = currentU1;
            //        u.Status_Renamed = "待機";
            //        colUnitOnBoard.Remove(u.ID);
            //    }

            //    // 召喚したユニットを解放
            //    DismissServant();

            //    // 魅了・憑依したユニットを解放
            //    DismissSlave();

            //    // ステータス表示中の場合は表示を解除
            //    if (ReferenceEquals(this, Status.DisplayedUnit))
            //    {
            //        Status.ClearUnitStatus();
            //    }
        }

        // 母艦 u に着艦
        public void Land(Unit u, bool by_cancel = false, bool is_event = false)
        {
            //    string tclass;
            //    int i;

            //    // Landコマンドで着艦した場合
            //    if (is_event)
            //    {
            //        if (Status_Renamed == "出撃" | Status_Renamed == "格納")
            //        {
            //            Escape();
            //        }
            //        else
            //        {
            //            // 出撃のための前準備

            //            // ユニットが存在する位置を決定
            //            if (u.Status_Renamed == "出撃")
            //            {
            //                tclass = Map.TerrainClass(u.x, u.y);
            //            }
            //            else
            //            {
            //                tclass = Map.TerrainClass((Map.MapWidth / 2), (Map.MapHeight / 2));
            //            }

            //            switch (tclass ?? "")
            //            {
            //                case "空":
            //                    {
            //                        Area = "空中";
            //                        break;
            //                    }

            //                case "陸":
            //                case "屋内":
            //                    {
            //                        string argarea_name = "空";
            //                        string argarea_name1 = "陸";
            //                        if (IsTransAvailable(argarea_name) & Strings.Mid(strAdaption, 1, 1) == "A")
            //                        {
            //                            Area = "空中";
            //                        }
            //                        else if (IsTransAvailable(argarea_name1))
            //                        {
            //                            Area = "地上";
            //                        }
            //                        else
            //                        {
            //                            Area = "空中";
            //                        }

            //                        break;
            //                    }

            //                case "月面":
            //                    {
            //                        string argarea_name2 = "空";
            //                        string argarea_name3 = "宇宙";
            //                        string argarea_name4 = "陸";
            //                        if ((IsTransAvailable(argarea_name2) | IsTransAvailable(argarea_name3)) & Strings.Mid(strAdaption, 4, 1) == "A")
            //                        {
            //                            Area = "宇宙";
            //                        }
            //                        else if (IsTransAvailable(argarea_name4))
            //                        {
            //                            Area = "地上";
            //                        }
            //                        else
            //                        {
            //                            Area = "宇宙";
            //                        }

            //                        break;
            //                    }

            //                case "水":
            //                case "深水":
            //                    {
            //                        string argarea_name5 = "空";
            //                        string argarea_name6 = "水上";
            //                        if (IsTransAvailable(argarea_name5))
            //                        {
            //                            Area = "空中";
            //                        }
            //                        else if (IsTransAvailable(argarea_name6))
            //                        {
            //                            Area = "水上";
            //                        }
            //                        else
            //                        {
            //                            Area = "水中";
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        Area = "宇宙";
            //                        break;
            //                    }
            //            }

            //            // 行動回数等を回復
            //            UsedAction = 0;
            //            UsedSupportAttack = 0;
            //            UsedSupportGuard = 0;
            //            UsedSyncAttack = 0;
            //            UsedCounterAttack = 0;
            //            if (BitmapID == 0)
            //            {
            //                object argIndex1 = Name;
            //                {
            //                    var withBlock = SRC.UList.Item(argIndex1);
            //                    if ((withBlock.Party0 ?? "") == (Party0 ?? "") & withBlock.BitmapID != 0 & (withBlock.get_Bitmap(false) ?? "") == (get_Bitmap(false) ?? ""))
            //                    {
            //                        BitmapID = withBlock.BitmapID;
            //                    }
            //                    else
            //                    {
            //                        var argu = this;
            //                        BitmapID = GUI.MakeUnitBitmap(argu);
            //                    }
            //                }

            //                Name = Conversions.ToString(argIndex1);
            //            }

            //            string argfname = "制御不可";
            //            if (IsFeatureAvailable(argfname))
            //            {
            //                string argcname = "暴走";
            //                string argcdata = "";
            //                AddCondition(argcname, -1, cdata: argcdata);
            //            }
            //        }
            //    }

            //    // 母艦に自分自身を格納
            //    var argu1 = this;
            //    u.LoadUnit(argu1);

            //    // 座標を母艦に合わせる
            //    x = u.x;
            //    y = u.y;
            //    Status_Renamed = "格納";
            //    if (Area != "宇宙" & Area != "空中")
            //    {
            //        Area = "地上";
            //    }

            //    // 気力減少
            //    if (!by_cancel)
            //    {
            //        {
            //            var withBlock1 = MainPilot();
            //            if (withBlock1.Personality != "機械")
            //            {
            //                string argoname = "母艦収納時気力低下小";
            //                if (Expression.IsOptionDefined(argoname))
            //                {
            //                    withBlock1.Morale = GeneralLib.MinLng(withBlock1.Morale, GeneralLib.MaxLng(withBlock1.Morale - 5, 100));
            //                }
            //                else
            //                {
            //                    withBlock1.Morale = (withBlock1.Morale - 5);
            //                }
            //            }
            //        }

            //        var loopTo = CountPilot();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            object argIndex2 = i;
            //            {
            //                var withBlock2 = Pilot(argIndex2);
            //                if ((MainPilot().ID ?? "") != (withBlock2.ID ?? "") & withBlock2.Personality != "機械")
            //                {
            //                    string argoname1 = "母艦収納時気力低下小";
            //                    if (Expression.IsOptionDefined(argoname1))
            //                    {
            //                        withBlock2.Morale = GeneralLib.MinLng(withBlock2.Morale, GeneralLib.MaxLng(withBlock2.Morale - 5, 100));
            //                    }
            //                    else
            //                    {
            //                        withBlock2.Morale = (withBlock2.Morale - 5);
            //                    }
            //                }
            //            }
            //        }

            //        var loopTo1 = CountSupport();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            object argIndex3 = i;
            //            {
            //                var withBlock3 = Support(argIndex3);
            //                if (withBlock3.Personality != "機械")
            //                {
            //                    string argoname2 = "母艦収納時気力低下小";
            //                    if (Expression.IsOptionDefined(argoname2))
            //                    {
            //                        withBlock3.Morale = GeneralLib.MinLng(withBlock3.Morale, GeneralLib.MaxLng(withBlock3.Morale - 5, 100));
            //                    }
            //                    else
            //                    {
            //                        withBlock3.Morale = (withBlock3.Morale - 5);
            //                    }
            //                }
            //            }
            //        }

            //        string argfname1 = "追加サポート";
            //        if (IsFeatureAvailable(argfname1))
            //        {
            //            {
            //                var withBlock4 = AdditionalSupport();
            //                if (withBlock4.Personality != "機械")
            //                {
            //                    string argoname3 = "母艦収納時気力低下小";
            //                    if (Expression.IsOptionDefined(argoname3))
            //                    {
            //                        withBlock4.Morale = GeneralLib.MinLng(withBlock4.Morale, GeneralLib.MaxLng(withBlock4.Morale - 5, 100));
            //                    }
            //                    else
            //                    {
            //                        withBlock4.Morale = (withBlock4.Morale - 5);
            //                    }
            //                }
            //            }
            //        }
            //    }
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
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;
            //    object argIndex1 = new_form;
            //    u = OtherForm(argIndex1);
            //    u.Status_Renamed = Status_Renamed;
            //    if (Status_Renamed != "破棄")
            //    {
            //        Status_Renamed = "他形態";
            //    }

            //    // 制御不可能な形態から元に戻る場合は暴走を解除
            //    string argfname = "制御不可";
            //    if (IsFeatureAvailable(argfname))
            //    {
            //        object argIndex3 = "暴走";
            //        if (IsConditionSatisfied(argIndex3))
            //        {
            //            object argIndex2 = "暴走";
            //            DeleteCondition(argIndex2);
            //        }
            //    }

            //    // 元の形態に戻る？
            //    object argIndex17 = "ノーマルモード";
            //    string arglist4 = FeatureData(argIndex17);
            //    if ((GeneralLib.LIndex(arglist4, 1) ?? "") == (new_form ?? ""))
            //    {
            //        object argIndex14 = "ノーマルモード付加";
            //        if (IsConditionSatisfied(argIndex14))
            //        {
            //            // 変身が解ける場合
            //            if (!string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                object argIndex5 = "ノーマルモード";
            //                string arglist1 = FeatureData(argIndex5);
            //                var loopTo = GeneralLib.LLength(arglist1);
            //                for (i = 2; i <= loopTo; i++)
            //                {
            //                    object argIndex4 = "ノーマルモード";
            //                    string arglist = FeatureData(argIndex4);
            //                    switch (GeneralLib.LIndex(arglist, i) ?? "")
            //                    {
            //                        case "消耗あり":
            //                            {
            //                                string argcname = "消耗";
            //                                string argcdata = "";
            //                                AddCondition(argcname, 1, cdata: argcdata);
            //                                break;
            //                            }

            //                        case "気力低下":
            //                            {
            //                                IncreaseMorale(-10);
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            object argIndex6 = "ノーマルモード付加";
            //            DeleteCondition(argIndex6);
            //            object argIndex10 = "能力コピー";
            //            if (IsConditionSatisfied(argIndex10))
            //            {
            //                object argIndex7 = "能力コピー";
            //                DeleteCondition(argIndex7);
            //                object argIndex8 = "パイロット画像";
            //                DeleteCondition(argIndex8);
            //                object argIndex9 = "メッセージ";
            //                DeleteCondition(argIndex9);
            //            }
            //        }
            //        // ハイパーモードが解ける場合
            //        else if (!string.IsNullOrEmpty(Map.MapFileName))
            //        {
            //            string argcname1 = "消耗";
            //            string argcdata1 = "";
            //            AddCondition(argcname1, 1, cdata: argcdata1);
            //            object argIndex13 = "ノーマルモード";
            //            string arglist3 = FeatureData(argIndex13);
            //            var loopTo1 = GeneralLib.LLength(arglist3);
            //            for (i = 2; i <= loopTo1; i++)
            //            {
            //                object argIndex12 = "ノーマルモード";
            //                string arglist2 = FeatureData(argIndex12);
            //                switch (GeneralLib.LIndex(arglist2, i) ?? "")
            //                {
            //                    case "消耗なし":
            //                        {
            //                            object argIndex11 = "消耗";
            //                            DeleteCondition(argIndex11);
            //                            break;
            //                        }

            //                    case "気力低下":
            //                        {
            //                            IncreaseMorale(-10);
            //                            break;
            //                        }
            //                }
            //            }
            //        }

            //        object argIndex16 = "残り時間";
            //        if (IsConditionSatisfied(argIndex16))
            //        {
            //            object argIndex15 = "残り時間";
            //            DeleteCondition(argIndex15);
            //        }
            //    }

            //    // 戦闘アニメで変更されたユニット画像を元に戻す
            //    object argIndex19 = "ユニット画像";
            //    if (IsConditionSatisfied(argIndex19))
            //    {
            //        object argIndex18 = "ユニット画像";
            //        DeleteCondition(argIndex18);
            //        var argu = this;
            //        BitmapID = GUI.MakeUnitBitmap(argu);
            //    }

            //    object argIndex21 = "非表示付加";
            //    if (IsConditionSatisfied(argIndex21))
            //    {
            //        object argIndex20 = "非表示付加";
            //        DeleteCondition(argIndex20);
            //        var argu1 = this;
            //        BitmapID = GUI.MakeUnitBitmap(argu1);
            //    }

            //    int counter;
            //    {
            //        var withBlock = u;
            //        // パラメータ受け継ぎ
            //        withBlock.BossRank = BossRank;
            //        withBlock.Rank = Rank;
            //        withBlock.Mode = Mode;
            //        withBlock.Area = Area;
            //        withBlock.UsedSupportAttack = UsedSupportAttack;
            //        withBlock.UsedSupportGuard = UsedSupportGuard;
            //        withBlock.UsedSyncAttack = UsedSyncAttack;
            //        withBlock.UsedCounterAttack = UsedCounterAttack;
            //        withBlock.Master = Master;
            //        // UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        Master = null;
            //        withBlock.Summoner = Summoner;
            //        // UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        Summoner = null;

            //        // アイテム受け継ぎ
            //        var loopTo2 = CountItem();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            Item localItem() { object argIndex1 = i; var ret = Item(argIndex1); return ret; }

            //            var argitm = localItem();
            //            withBlock.AddItem0(argitm);
            //        }

            //        // スペシャルパワー効果のコピー
            //        CopySpecialPowerInEffect(u);
            //        RemoveAllSpecialPowerInEffect();

            //        // 特殊ステータスのコピー
            //        var loopTo3 = withBlock.CountCondition();
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            object argIndex22 = 1;
            //            withBlock.DeleteCondition0(argIndex22);
            //        }

            //        var loopTo4 = CountCondition();
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            int localConditionLifetime1() { object argIndex1 = i; var ret = ConditionLifetime(argIndex1); return ret; }

            //            string localConditionData1() { object argIndex1 = i; var ret = ConditionData(argIndex1); return ret; }

            //            string localConditionData2() { object argIndex1 = i; var ret = ConditionData(argIndex1); return ret; }

            //            if (localConditionLifetime1() != 0 & Strings.InStr(localConditionData1(), "パイロット能力付加") == 0 & Strings.InStr(localConditionData2(), "パイロット能力強化") == 0)
            //            {
            //                string localCondition() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //                int localConditionLifetime() { object argIndex1 = i; var ret = ConditionLifetime(argIndex1); return ret; }

            //                double localConditionLevel() { object argIndex1 = i; var ret = ConditionLevel(argIndex1); return ret; }

            //                string localConditionData() { object argIndex1 = i; var ret = ConditionData(argIndex1); return ret; }

            //                string argcname2 = localCondition();
            //                string argcdata2 = localConditionData();
            //                withBlock.AddCondition(argcname2, localConditionLifetime(), localConditionLevel(), argcdata2);
            //            }
            //        }

            //        var loopTo5 = CountCondition();
            //        for (i = 1; i <= loopTo5; i++)
            //        {
            //            object argIndex23 = 1;
            //            DeleteCondition0(argIndex23);
            //        }

            //        // パイロットの乗せ換え
            //        object argIndex24 = "変形";
            //        list = FeatureData(argIndex24);
            //        if (GeneralLib.LLength(list) > 0 & Data.PilotNum == -GeneralLib.LLength(list) & CountPilot() == GeneralLib.LLength(list))
            //        {
            //            // 変形によりパイロットの順番が変化する場合
            //            var loopTo6 = GeneralLib.LLength(list);
            //            for (idx = 2; idx <= loopTo6; idx++)
            //            {
            //                if ((withBlock.Name ?? "") == (GeneralLib.LIndex(list, idx) ?? ""))
            //                {
            //                    break;
            //                }
            //            }

            //            if (idx <= GeneralLib.LLength(list))
            //            {
            //                object argIndex25 = "変形";
            //                list = withBlock.FeatureData(argIndex25);
            //                var loopTo7 = GeneralLib.LLength(list);
            //                for (idx2 = 2; idx2 <= loopTo7; idx2++)
            //                {
            //                    buf = GeneralLib.LIndex(list, idx2);
            //                    if ((Name ?? "") == (buf ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                j = 2;
            //                var loopTo8 = CountPilot();
            //                for (i = 1; i <= loopTo8; i++)
            //                {
            //                    switch (i)
            //                    {
            //                        case 1:
            //                            {
            //                                Pilot localPilot() { object argIndex1 = idx; var ret = Pilot(argIndex1); return ret; }

            //                                var argp = localPilot();
            //                                withBlock.AddPilot(argp);
            //                                break;
            //                            }

            //                        case var @case when @case == idx2:
            //                            {
            //                                object argIndex26 = 1;
            //                                var argp1 = Pilot(argIndex26);
            //                                withBlock.AddPilot(argp1);
            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                if (idx == j)
            //                                {
            //                                    j = (j + 1);
            //                                }

            //                                Pilot localPilot1() { object argIndex1 = j; var ret = Pilot(argIndex1); return ret; }

            //                                var argp2 = localPilot1();
            //                                withBlock.AddPilot(argp2);
            //                                j = (j + 1);
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var loopTo9 = CountPilot();
            //                for (i = 1; i <= loopTo9; i++)
            //                {
            //                    Pilot localPilot2() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                    var argp3 = localPilot2();
            //                    withBlock.AddPilot(argp3);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            var loopTo10 = CountPilot();
            //            for (i = 1; i <= loopTo10; i++)
            //            {
            //                Pilot localPilot3() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                var argp4 = localPilot3();
            //                withBlock.AddPilot(argp4);
            //            }
            //        }

            //        var loopTo11 = CountSupport();
            //        for (i = 1; i <= loopTo11; i++)
            //        {
            //            Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            var argp5 = localSupport();
            //            withBlock.AddSupport(argp5);
            //        }

            //        var loopTo12 = CountUnitOnBoard();
            //        for (i = 1; i <= loopTo12; i++)
            //        {
            //            Unit localUnitOnBoard() { object argIndex1 = i; var ret = UnitOnBoard(argIndex1); return ret; }

            //            var argu2 = localUnitOnBoard();
            //            withBlock.LoadUnit(argu2);
            //        }

            //        var loopTo13 = CountServant();
            //        for (i = 1; i <= loopTo13; i++)
            //        {
            //            Unit localServant() { object argIndex1 = i; var ret = Servant(argIndex1); return ret; }

            //            var argu3 = localServant();
            //            withBlock.AddServant(argu3);
            //        }

            //        var loopTo14 = CountSlave();
            //        for (i = 1; i <= loopTo14; i++)
            //        {
            //            Unit localSlave() { object argIndex1 = i; var ret = Slave(argIndex1); return ret; }

            //            var argu4 = localSlave();
            //            withBlock.AddSlave(argu4);
            //        }

            //        var loopTo15 = CountPilot();
            //        for (i = 1; i <= loopTo15; i++)
            //        {
            //            object argIndex27 = 1;
            //            DeletePilot(argIndex27);
            //        }

            //        var loopTo16 = CountSupport();
            //        for (i = 1; i <= loopTo16; i++)
            //        {
            //            object argIndex28 = 1;
            //            DeleteSupport(argIndex28);
            //        }

            //        var loopTo17 = CountUnitOnBoard();
            //        for (i = 1; i <= loopTo17; i++)
            //        {
            //            object argIndex29 = 1;
            //            UnloadUnit(argIndex29);
            //        }

            //        var loopTo18 = CountServant();
            //        for (i = 1; i <= loopTo18; i++)
            //        {
            //            object argIndex30 = 1;
            //            DeleteServant(argIndex30);
            //        }

            //        var loopTo19 = CountSlave();
            //        for (i = 1; i <= loopTo19; i++)
            //        {
            //            object argIndex31 = 1;
            //            DeleteSlave(argIndex31);
            //        }

            //        var loopTo20 = withBlock.CountPilot();
            //        for (i = 1; i <= loopTo20; i++)
            //        {
            //            Pilot localPilot4() { object argIndex1 = i; var ret = withBlock.Pilot(argIndex1); return ret; }

            //            localPilot4().Unit_Renamed = u;
            //        }

            //        var loopTo21 = withBlock.CountSupport();
            //        for (i = 1; i <= loopTo21; i++)
            //        {
            //            Pilot localSupport1() { object argIndex1 = i; var ret = withBlock.Support(argIndex1); return ret; }

            //            localSupport1().Unit_Renamed = u;
            //            Pilot localSupport4() { object argIndex1 = i; var ret = withBlock.Support(argIndex1); return ret; }

            //            if (localSupport4().SupportIndex > 0)
            //            {
            //                string argfname1 = "分離";
            //                string argfname2 = "分離";
            //                if (IsFeatureAvailable(argfname1) & withBlock.IsFeatureAvailable(argfname2))
            //                {
            //                    object argIndex33 = "分離";
            //                    string arglist6 = withBlock.FeatureData(argIndex33);
            //                    var loopTo22 = GeneralLib.LLength(arglist6);
            //                    for (j = 2; j <= loopTo22; j++)
            //                    {
            //                        Pilot localSupport3() { object argIndex1 = i; var ret = withBlock.Support(argIndex1); return ret; }

            //                        string localLIndex() { object argIndex1 = "分離"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                        object argIndex32 = "分離";
            //                        string arglist5 = FeatureData(argIndex32);
            //                        if ((GeneralLib.LIndex(arglist5, (localSupport3().SupportIndex + 1)) ?? "") == (localLIndex() ?? ""))
            //                        {
            //                            Pilot localSupport2() { object argIndex1 = i; var ret = withBlock.Support(argIndex1); return ret; }

            //                            localSupport2().SupportIndex = (j - 1);
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        withBlock.Update();

            //        // 弾数データを記録
            //        wname = new string[(CountWeapon() + 1)];
            //        wbullet = new int[(CountWeapon() + 1)];
            //        wmaxbullet = new int[(CountWeapon() + 1)];
            //        var loopTo23 = CountWeapon();
            //        for (i = 1; i <= loopTo23; i++)
            //        {
            //            wname[i] = Weapon(i).Name;
            //            wbullet[i] = Bullet(i);
            //            wmaxbullet[i] = MaxBullet(i);
            //        }

            //        aname = new string[(CountAbility() + 1)];
            //        astock = new int[(CountAbility() + 1)];
            //        amaxstock = new int[(CountAbility() + 1)];
            //        var loopTo24 = CountAbility();
            //        for (i = 1; i <= loopTo24; i++)
            //        {
            //            aname[i] = Ability(i).Name;
            //            astock[i] = Stock(i);
            //            amaxstock[i] = MaxStock(i);
            //        }

            //        // 弾数の受け継ぎ
            //        idx = 1;
            //        var loopTo25 = withBlock.CountWeapon();
            //        for (i = 1; i <= loopTo25; i++)
            //        {
            //            counter = idx;
            //            var loopTo26 = Information.UBound(wname);
            //            for (j = counter; j <= loopTo26; j++)
            //            {
            //                if ((withBlock.Weapon(i).Name ?? "") == (wname[j] ?? "") & withBlock.MaxBullet(i) > 0 & wmaxbullet[j] > 0)
            //                {
            //                    withBlock.SetBullet(i, ((wbullet[j] * withBlock.MaxBullet(i)) / wmaxbullet[j]));
            //                    idx = (j + 1);
            //                    break;
            //                }
            //            }
            //        }

            //        idx = 1;
            //        var loopTo27 = withBlock.CountAbility();
            //        for (i = 1; i <= loopTo27; i++)
            //        {
            //            counter = idx;
            //            var loopTo28 = Information.UBound(aname);
            //            for (j = counter; j <= loopTo28; j++)
            //            {
            //                if ((withBlock.Ability(i).Name ?? "") == (aname[j] ?? "") & withBlock.MaxStock(i) > 0 & amaxstock[j] > 0)
            //                {
            //                    withBlock.SetStock(i, ((astock[j] * withBlock.MaxStock(i)) / amaxstock[j]));
            //                    idx = (j + 1);
            //                    break;
            //                }
            //            }
            //        }

            //        // 弾数・使用回数共有の実現
            //        withBlock.SyncBullet();

            //        // アイテムを削除
            //        var loopTo29 = CountItem();
            //        for (i = 1; i <= loopTo29; i++)
            //        {
            //            object argIndex34 = 1;
            //            DeleteItem(argIndex34);
            //        }

            //        withBlock.Update();

            //        // ＨＰ＆ＥＮの受け継ぎ
            //        string localLIndex1() { object argIndex1 = "パーツ分離"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        if ((new_form ?? "") == (localLIndex1() ?? ""))
            //        {
            //            withBlock.HP = withBlock.MaxHP;
            //        }
            //        else
            //        {
            //            withBlock.HP = (int)(withBlock.MaxHP * hp_ratio / 100d);
            //        }

            //        withBlock.EN = (int)(withBlock.MaxEN * en_ratio / 100d);

            //        // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
            //        object argIndex40 = "残り時間";
            //        string argfname5 = "ノーマルモード";
            //        string argfname6 = "制限時間";
            //        if (!withBlock.IsConditionSatisfied(argIndex40))
            //        {
            //            string argfname3 = "ノーマルモード";
            //            string argfname4 = "制限時間";
            //            if (withBlock.IsFeatureAvailable(argfname3))
            //            {
            //                string localLIndex4() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                if (Information.IsNumeric(localLIndex4()))
            //                {
            //                    object argIndex36 = "残り時間";
            //                    if (withBlock.IsConditionSatisfied(argIndex36))
            //                    {
            //                        object argIndex35 = "残り時間";
            //                        withBlock.DeleteCondition(argIndex35);
            //                    }

            //                    string localLIndex2() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                    string localLIndex3() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                    string argcname3 = "残り時間";
            //                    string argcdata3 = "";
            //                    withBlock.AddCondition(argcname3, Conversions.Toint(localLIndex3()), cdata: argcdata3);
            //                }
            //            }
            //            else if (withBlock.IsFeatureAvailable(argfname4))
            //            {
            //                string argcname4 = "残り時間";
            //                object argIndex37 = "制限時間";
            //                object argIndex38 = "制限時間";
            //                string argcdata4 = "";
            //                withBlock.AddCondition(argcname4, Conversions.Toint(withBlock.FeatureData(argIndex38)), cdata: argcdata4);
            //            }
            //        }
            //        else if (!withBlock.IsFeatureAvailable(argfname5) & !withBlock.IsFeatureAvailable(argfname6))
            //        {
            //            // 残り時間が必要ない形態にTransformコマンドで強制変形された？
            //            object argIndex39 = "残り時間";
            //            withBlock.DeleteCondition(argIndex39);
            //        }

            //        switch (withBlock.Status_Renamed ?? "")
            //        {
            //            case "出撃":
            //                {
            //                    // 変形後のユニットを出撃させる
            //                    // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                    Map.MapDataForUnit[x, y] = null;
            //                    prev_x = x;
            //                    prev_y = y;
            //                    withBlock.UsedAction = UsedAction;
            //                    withBlock.StandBy(x, y);
            //                    if (withBlock.x != prev_x | withBlock.y != prev_y)
            //                    {
            //                        GUI.EraseUnitBitmap(prev_x, prev_y, false);
            //                    }

            //                    break;
            //                }

            //            case "格納":
            //                {
            //                    // 変形後のユニットを格納する
            //                    foreach (Unit eu in SRC.UList)
            //                    {
            //                        var loopTo30 = eu.CountUnitOnBoard();
            //                        for (j = 1; j <= loopTo30; j++)
            //                        {
            //                            Unit localUnitOnBoard1() { object argIndex1 = j; var ret = eu.UnitOnBoard(argIndex1); return ret; }

            //                            if ((ID ?? "") == (localUnitOnBoard1().ID ?? ""))
            //                            {
            //                                object argIndex41 = ID;
            //                                eu.UnloadUnit(argIndex41);
            //                                eu.LoadUnit(u);
            //                                goto EndLoop;
            //                            }
            //                        }
            //                    }

            //                EndLoop:
            //                    ;
            //                    break;
            //                }
            //        }
            //    }

            //    if (string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        return;
            //    }

            //    // ハイパーモードが解ける場合
            //    object argIndex42 = "ノーマルモード";
            //    buf = FeatureData(argIndex42);
            //    if ((GeneralLib.LIndex(buf, 1) ?? "") == (new_form ?? ""))
            //    {
            //        var loopTo31 = GeneralLib.LLength(buf);
            //        for (i = 2; i <= loopTo31; i++)
            //        {
            //            switch (GeneralLib.LIndex(buf, i) ?? "")
            //            {
            //                case "回数制限":
            //                    {
            //                        string argcname5 = "行動不能";
            //                        string argcdata5 = "";
            //                        AddCondition(argcname5, -1, cdata: argcdata5);
            //                        break;
            //                    }
            //            }
            //        }
            //    }
        }

        // 合体
        public void Combine(string uname = "", bool is_event = false)
        {
            //    int k, i, j, l;
            //    Unit u;
            //    Unit[] rarray;
            //    string prev_status;
            //    double hp_ratio = default, en_ratio = default;
            //    string fdata;
            //    prev_status = Status_Renamed;
            //    if (string.IsNullOrEmpty(uname))
            //    {
            //        // 合体形態が指定されてなければその場所にいるユニットと２体合体
            //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        u = null;
            //        var loopTo = CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            object argIndex2 = i;
            //            if (Feature(argIndex2) == "合体")
            //            {
            //                object argIndex1 = i;
            //                fdata = FeatureData(argIndex1);
            //                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                if (GeneralLib.LLength(fdata) == 3 & Map.MapDataForUnit[x, y].Name == GeneralLib.LIndex(fdata, 3) & localIsDefined())
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    string localLIndex() { string arglist = hs81b489053b0047fb8fab2715d76f0b3f(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                    Unit localItem() { object argIndex1 = (object)hse7677c879f2b418e8dbb67c5dad5db85(); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                    u = localItem().CurrentForm();
            //                    break;
            //                }
            //            }
            //        }

            //        if (u is null)
            //        {
            //            var loopTo1 = CountFeature();
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                object argIndex4 = i;
            //                if (Feature(argIndex4) == "合体")
            //                {
            //                    object argIndex3 = i;
            //                    fdata = FeatureData(argIndex3);
            //                    bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                    if (GeneralLib.LLength(fdata) == 3 & Map.MapDataForUnit[x, y].IsEqual(GeneralLib.LIndex(fdata, 3)) & localIsDefined1())
            //                    {
            //                        Unit localItem1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                        u = localItem1().CurrentForm();
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            //        // 合体のパートナーを調べる
            //        var loopTo2 = u.CountFeature();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            string localFeature() { object argIndex1 = i; var ret = u.Feature(argIndex1); return ret; }

            //            string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            int localLLength() { string arglist = hs51faf446da8e4981a1c2f1fe759168f4(); var ret = GeneralLib.LLength(arglist); return ret; }

            //            string localFeatureData2() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            string localLIndex1() { string arglist = hsb077af4743da47379ef57233cdc6fa2e(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //            string localFeatureData3() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            string localLIndex2() { string arglist = hs35a57c94693d489ca95b1b6c02b7f584(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //            string localFeatureData4() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            string localLIndex3() { string arglist = hscc5ae77b155c4db6b72a8ff201c4d383(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //            string localFeatureData5() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            string localLIndex4() { string arglist = hs55605004ddf144379941d1bb4dbdc993(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //            if (localFeature() == "分離" & localLLength() == 3 & (IsEqual(localLIndex1()) & Map.MapDataForUnit[x, y].IsEqual(localLIndex2()) | IsEqual(localLIndex3()) & Map.MapDataForUnit[x, y].IsEqual(localLIndex4())))
            //            {
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // 合体ユニットが作成されていない
            //        bool localIsDefined2() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //        if (!localIsDefined2())
            //        {
            //            string argmsg = uname + "が作成されていません";
            //            GUI.ErrorMessage(argmsg);
            //            SRC.ExitGame();
            //        }

            //        Unit localItem2() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //        u = localItem2().CurrentForm();

            //        // 合体のパートナーを調べる
            //        var loopTo3 = u.CountFeature();
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            string localFeature1() { object argIndex1 = i; var ret = u.Feature(argIndex1); return ret; }

            //            string localFeatureData6() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            int localLLength1() { string arglist = hs19d13409c3c14abeb6633b778b53f517(); var ret = GeneralLib.LLength(arglist); return ret; }

            //            if (localFeature1() == "分離" & localLLength1() > 2)
            //            {
            //                break;
            //            }
            //        }
            //    }

            //    // 合体するユニットの配列を作成
            //    if (i > u.CountFeature())
            //    {
            //        string argmsg1 = u.Name + "のデータに" + Name + "に対する分離指定がみつかりません。" + "書式を確認してください。";
            //        GUI.ErrorMessage(argmsg1);
            //        return;
            //    }

            //    string localFeatureData7() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //    int localLLength2() { string arglist = hs7e9583a3d5b64b0e89ed2b453203ed96(); var ret = GeneralLib.LLength(arglist); return ret; }

            //    rarray = new Unit[(localLLength2())];
            //    var loopTo4 = Information.UBound(rarray);
            //    for (j = 1; j <= loopTo4; j++)
            //    {
            //        string localFeatureData9() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //        string localLIndex6() { string arglist = hsdde5a2bab5114fb984e607877f33a598(); var ret = GeneralLib.LIndex(arglist, (j + 1)); return ret; }

            //        bool localIsDefined3() { object argIndex1 = (object)hs864ff7ed86044dd6afb3abdbc02521a4(); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //        if (!localIsDefined3())
            //        {
            //            string localFeatureData8() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //            string localLIndex5() { string arglist = hsd9f70e51b07d4e9fa7669aca81ec510a(); var ret = GeneralLib.LIndex(arglist, (j + 1)); return ret; }

            //            string argmsg2 = localLIndex5() + "が作成されていません";
            //            GUI.ErrorMessage(argmsg2);
            //            return;
            //        }

            //        string localFeatureData10() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //        string localLIndex7() { string arglist = hs00e33e973a4149d388ddb705e43ffed7(); var ret = GeneralLib.LIndex(arglist, (j + 1)); return ret; }

            //        object argIndex5 = localLIndex7();
            //        rarray[j] = SRC.UList.Item(argIndex5);
            //    }

            //    string BGM;
            //    if (!is_event)
            //    {
            //        if (Status_Renamed == "出撃")
            //        {
            //            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            //            string argfname = "追加パイロット";
            //            if (u.IsFeatureAvailable(argfname))
            //            {
            //                bool localIsDefined5() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

            //                if (!localIsDefined5())
            //                {
            //                    bool localIsDefined4() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(argIndex1); var ret = SRC.PDList.IsDefined(argIndex2); return ret; }

            //                    if (!localIsDefined4())
            //                    {
            //                        object argIndex6 = "追加パイロット";
            //                        string argmsg3 = u.Name + "の追加パイロット「" + u.FeatureData(argIndex6) + "」のデータが見つかりません";
            //                        GUI.ErrorMessage(argmsg3);
            //                        SRC.TerminateSRC();
            //                    }

            //                    object argIndex7 = "追加パイロット";
            //                    string argpname = u.FeatureData(argIndex7);
            //                    string argpparty = Party0;
            //                    string arggid = "";
            //                    SRC.PList.Add(argpname, MainPilot().Level, argpparty, gid: arggid);
            //                    this.Party0 = argpparty;
            //                }
            //            }

            //            bool localIsMessageDefined1() { string argmain_situation = "合体(" + u.Name + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //            bool localIsMessageDefined2() { object argIndex1 = "合体"; string argmain_situation = "合体(" + FeatureName(argIndex1) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //            string argmain_situation1 = "合体";
            //            if (localIsMessageDefined1() | localIsMessageDefined2() | IsMessageDefined(argmain_situation1))
            //            {
            //                string argfname1 = "合体ＢＧＭ";
            //                if (IsFeatureAvailable(argfname1))
            //                {
            //                    var loopTo5 = CountFeature();
            //                    for (i = 1; i <= loopTo5; i++)
            //                    {
            //                        string localFeature2() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //                        string localFeatureData13() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                        string localLIndex8() { string arglist = hs8b8df815445f4c329f8d70f276cd46e5(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                        if (localFeature2() == "合体ＢＧＭ" & (localLIndex8() ?? "") == (u.Name ?? ""))
            //                        {
            //                            string localFeatureData11() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                            string localFeatureData12() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                            string argmidi_name = Strings.Mid(localFeatureData11(), Strings.InStr(localFeatureData12(), " ") + 1);
            //                            BGM = Sound.SearchMidiFile(argmidi_name);
            //                            if (Strings.Len(BGM) > 0)
            //                            {
            //                                Sound.ChangeBGM(BGM);
            //                                GUI.Sleep(500);
            //                            }

            //                            break;
            //                        }
            //                    }
            //                }

            //                Unit argu1 = null;
            //                Unit argu2 = null;
            //                GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //                bool localIsMessageDefined() { object argIndex1 = "合体"; string argmain_situation = "合体(" + FeatureName(argIndex1) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

            //                string argmain_situation = "合体(" + u.Name + ")";
            //                if (IsMessageDefined(argmain_situation))
            //                {
            //                    string argSituation = "合体(" + u.Name + ")";
            //                    string argmsg_mode = "";
            //                    PilotMessage(argSituation, msg_mode: argmsg_mode);
            //                }
            //                else if (localIsMessageDefined())
            //                {
            //                    object argIndex8 = "合体";
            //                    string argSituation2 = "合体(" + FeatureName(argIndex8) + ")";
            //                    string argmsg_mode2 = "";
            //                    PilotMessage(argSituation2, msg_mode: argmsg_mode2);
            //                }
            //                else
            //                {
            //                    string argSituation1 = "合体";
            //                    string argmsg_mode1 = "";
            //                    PilotMessage(argSituation1, msg_mode: argmsg_mode1);
            //                }

            //                GUI.CloseMessageForm();
            //            }
            //        }
            //    }

            //    // 分離ユニットと合体ユニットが同名の武器を持つ場合は弾数を累積するため
            //    // このような武器の弾数を0にする
            //    var loopTo6 = u.CountWeapon();
            //    for (i = 1; i <= loopTo6; i++)
            //    {
            //        var loopTo7 = Information.UBound(rarray);
            //        for (j = 1; j <= loopTo7; j++)
            //        {
            //            {
            //                var withBlock = rarray[j].CurrentForm();
            //                var loopTo8 = withBlock.CountWeapon();
            //                for (k = 1; k <= loopTo8; k++)
            //                {
            //                    if ((u.Weapon(i).Name ?? "") == (withBlock.Weapon(k).Name ?? ""))
            //                    {
            //                        u.SetBullet(i, 0);
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    // 使用回数を合わせる
            //    var loopTo9 = u.CountAbility();
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        var loopTo10 = Information.UBound(rarray);
            //        for (j = 1; j <= loopTo10; j++)
            //        {
            //            {
            //                var withBlock1 = rarray[j].CurrentForm();
            //                var loopTo11 = withBlock1.CountAbility();
            //                for (k = 1; k <= loopTo11; k++)
            //                {
            //                    if ((u.Ability(i).Name ?? "") == (withBlock1.Ability(k).Name ?? ""))
            //                    {
            //                        u.SetStock(i, 0);
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // １番目のユニットのステータスを合体後のユニットに継承
            //    {
            //        var withBlock2 = rarray[1].CurrentForm();
            //        withBlock2.CopySpecialPowerInEffect(u);
            //        withBlock2.RemoveAllSpecialPowerInEffect();
            //        var loopTo12 = withBlock2.CountItem();
            //        for (i = 1; i <= loopTo12; i++)
            //        {
            //            Item localItem3() { object argIndex1 = i; var ret = withBlock2.Item(argIndex1); return ret; }

            //            var argitm = localItem3();
            //            u.AddItem(argitm);
            //        }

            //        u.Master = withBlock2.Master;
            //        // UPGRADE_NOTE: オブジェクト rarray().CurrentForm.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        withBlock2.Master = null;
            //        u.Summoner = withBlock2.Summoner;
            //        // UPGRADE_NOTE: オブジェクト rarray().CurrentForm.Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        withBlock2.Summoner = null;
            //        u.UsedSupportAttack = withBlock2.UsedSupportAttack;
            //        u.UsedSupportGuard = withBlock2.UsedSupportGuard;
            //        u.UsedSyncAttack = withBlock2.UsedSyncAttack;
            //        u.UsedCounterAttack = withBlock2.UsedCounterAttack;
            //        var loopTo13 = withBlock2.CountServant();
            //        for (i = 1; i <= loopTo13; i++)
            //        {
            //            Unit localServant() { object argIndex1 = i; var ret = withBlock2.Servant(argIndex1); return ret; }

            //            var argu = localServant();
            //            u.AddServant(argu);
            //        }

            //        var loopTo14 = withBlock2.CountServant();
            //        for (i = 1; i <= loopTo14; i++)
            //        {
            //            object argIndex9 = 1;
            //            withBlock2.DeleteServant(argIndex9);
            //        }

            //        var loopTo15 = withBlock2.CountSlave();
            //        for (i = 1; i <= loopTo15; i++)
            //        {
            //            Unit localSlave() { object argIndex1 = i; var ret = withBlock2.Slave(argIndex1); return ret; }

            //            var argu3 = localSlave();
            //            u.AddSlave(argu3);
            //        }

            //        var loopTo16 = withBlock2.CountSlave();
            //        for (i = 1; i <= loopTo16; i++)
            //        {
            //            object argIndex10 = 1;
            //            withBlock2.DeleteSlave(argIndex10);
            //        }

            //        // 合体する各ユニットに対しての処理を行う
            //    }

            //    var loopTo17 = Information.UBound(rarray);
            //    for (i = 1; i <= loopTo17; i++)
            //    {
            //        // マップ上から撤退させる
            //        {
            //            var withBlock3 = rarray[i].CurrentForm();
            //            switch (withBlock3.Status_Renamed ?? "")
            //            {
            //                case "出撃":
            //                    {
            //                        withBlock3.Status_Renamed = "待機";
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[withBlock3.x, withBlock3.y] = null;
            //                        GUI.EraseUnitBitmap(withBlock3.x, withBlock3.y);
            //                        break;
            //                    }

            //                case "格納":
            //                    {
            //                        withBlock3.Status_Renamed = "待機";
            //                        foreach (Unit eu in SRC.UList)
            //                        {
            //                            var loopTo18 = eu.CountUnitOnBoard();
            //                            for (j = 1; j <= loopTo18; j++)
            //                            {
            //                                Unit localUnitOnBoard() { object argIndex1 = j; var ret = eu.UnitOnBoard(argIndex1); return ret; }

            //                                if ((withBlock3.ID ?? "") == (localUnitOnBoard().ID ?? ""))
            //                                {
            //                                    object argIndex11 = withBlock3.ID;
            //                                    eu.UnloadUnit(argIndex11);
            //                                    goto EndLoop;
            //                                }
            //                            }
            //                        }

            //                    EndLoop:
            //                        ;
            //                        break;
            //                    }
            //            }
            //        }

            //        // デフォルトの形態に変形させておく
            //        if (!ReferenceEquals(rarray[i].CurrentForm(), rarray[i]))
            //        {
            //            string argnew_form = rarray[i].Name;
            //            rarray[i].CurrentForm().Transform(argnew_form);
            //            rarray[i].Name = argnew_form;
            //        }

            //        {
            //            var withBlock4 = rarray[i];
            //            if (i == 1)
            //            {
            //                withBlock4.Status_Renamed = "旧主形態";
            //            }
            //            else
            //            {
            //                withBlock4.Status_Renamed = "旧形態";
            //            }

            //            hp_ratio = hp_ratio + 100 * withBlock4.HP / (double)withBlock4.MaxHP;
            //            en_ratio = en_ratio + 100 * withBlock4.EN / (double)withBlock4.MaxEN;
            //            if (withBlock4.Rank > u.Rank)
            //            {
            //                u.Rank = withBlock4.Rank;
            //            }

            //            if (withBlock4.BossRank > u.BossRank)
            //            {
            //                u.BossRank = withBlock4.BossRank;
            //                u.FullRecover();
            //            }

            //            string argfname2 = "召喚ユニット";
            //            if (withBlock4.IsFeatureAvailable(argfname2))
            //            {
            //                // 召喚ユニットの場合はパイロットの乗せ換えは行わない
            //                if (Strings.InStr(withBlock4.MainPilot().Name, "(ザコ)") > 0 | Strings.InStr(withBlock4.MainPilot().Name, "(汎用)") > 0)
            //                {
            //                    // 汎用パイロットの場合は削除
            //                    withBlock4.MainPilot().Alive = false;
            //                }
            //            }
            //            else
            //            {
            //                // パイロットの乗せ換え
            //                var loopTo19 = withBlock4.CountPilot();
            //                for (j = 1; j <= loopTo19; j++)
            //                {
            //                    Pilot localPilot() { object argIndex1 = j; var ret = withBlock4.Pilot(argIndex1); return ret; }

            //                    localPilot().Ride(u);
            //                }

            //                var loopTo20 = withBlock4.CountPilot();
            //                for (j = 1; j <= loopTo20; j++)
            //                {
            //                    object argIndex12 = 1;
            //                    withBlock4.DeletePilot(argIndex12);
            //                }

            //                // サポートの乗せ換え
            //                var loopTo21 = withBlock4.CountSupport();
            //                for (j = 1; j <= loopTo21; j++)
            //                {
            //                    Pilot localSupport() { object argIndex1 = j; var ret = withBlock4.Support(argIndex1); return ret; }

            //                    localSupport().Ride(u, true);
            //                    Pilot localSupport1() { object argIndex1 = j; var ret = withBlock4.Support(argIndex1); return ret; }

            //                    localSupport1().SupportIndex = i;
            //                }

            //                var loopTo22 = withBlock4.CountSupport();
            //                for (j = 1; j <= loopTo22; j++)
            //                {
            //                    object argIndex13 = 1;
            //                    withBlock4.DeleteSupport(argIndex13);
            //                }
            //            }

            //            // 搭載ユニットの乗せ換え
            //            var loopTo23 = withBlock4.CountUnitOnBoard();
            //            for (j = 1; j <= loopTo23; j++)
            //            {
            //                Unit localUnitOnBoard1() { object argIndex1 = j; var ret = withBlock4.UnitOnBoard(argIndex1); return ret; }

            //                var argu4 = localUnitOnBoard1();
            //                u.LoadUnit(argu4);
            //            }

            //            var loopTo24 = u.CountUnitOnBoard();
            //            for (j = 1; j <= loopTo24; j++)
            //            {
            //                object argIndex14 = 1;
            //                withBlock4.UnloadUnit(argIndex14);
            //            }

            //            // 分離ユニットと共通する武装の弾数は一旦0にクリア
            //            var loopTo25 = u.CountWeapon();
            //            for (j = 1; j <= loopTo25; j++)
            //            {
            //                var loopTo26 = withBlock4.CountWeapon();
            //                for (k = 1; k <= loopTo26; k++)
            //                {
            //                    if ((u.Weapon(j).Name ?? "") == (withBlock4.Weapon(k).Name ?? ""))
            //                    {
            //                        u.SetBullet(j, 0);
            //                        break;
            //                    }
            //                }

            //                var loopTo27 = withBlock4.CountOtherForm();
            //                for (k = 1; k <= loopTo27; k++)
            //                {
            //                    object argIndex15 = k;
            //                    {
            //                        var withBlock5 = withBlock4.OtherForm(argIndex15);
            //                        var loopTo28 = withBlock5.CountWeapon();
            //                        for (l = 1; l <= loopTo28; l++)
            //                        {
            //                            if ((u.Weapon(j).Name ?? "") == (withBlock5.Weapon(l).Name ?? ""))
            //                            {
            //                                u.SetBullet(j, 0);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // アビリティの使用回数も同様の処理を行う
            //            var loopTo29 = u.CountAbility();
            //            for (j = 1; j <= loopTo29; j++)
            //            {
            //                var loopTo30 = withBlock4.CountAbility();
            //                for (k = 1; k <= loopTo30; k++)
            //                {
            //                    if ((u.Ability(j).Name ?? "") == (withBlock4.Ability(k).Name ?? ""))
            //                    {
            //                        u.SetStock(j, 0);
            //                        break;
            //                    }
            //                }

            //                var loopTo31 = withBlock4.CountOtherForm();
            //                for (k = 1; k <= loopTo31; k++)
            //                {
            //                    object argIndex16 = k;
            //                    {
            //                        var withBlock6 = withBlock4.OtherForm(argIndex16);
            //                        var loopTo32 = withBlock6.CountAbility();
            //                        for (l = 1; l <= loopTo32; l++)
            //                        {
            //                            if ((u.Ability(j).Name ?? "") == (withBlock6.Ability(l).Name ?? ""))
            //                            {
            //                                u.SetStock(j, 0);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // スペシャルパワーの効果を消去
            //            withBlock4.RemoveAllSpecialPowerInEffect();
            //        }
            //    }

            //    // 合体後のユニットの武装の弾数及びアビリティの使用回数は分離ユニットの
            //    // 弾数及び使用回数の合計に設定する
            //    var loopTo33 = Information.UBound(rarray);
            //    for (i = 1; i <= loopTo33; i++)
            //    {
            //        {
            //            var withBlock7 = rarray[i];
            //            // 武装の弾数の処理
            //            var loopTo34 = u.CountWeapon();
            //            for (j = 1; j <= loopTo34; j++)
            //            {
            //                var loopTo35 = withBlock7.CountWeapon();
            //                for (k = 1; k <= loopTo35; k++)
            //                {
            //                    if ((u.Weapon(j).Name ?? "") == (withBlock7.Weapon(k).Name ?? ""))
            //                    {
            //                        u.SetBullet(j, (u.Bullet(j) + withBlock7.Bullet(k)));
            //                        goto NextWeapon;
            //                    }
            //                }

            //                var loopTo36 = withBlock7.CountOtherForm();
            //                for (k = 1; k <= loopTo36; k++)
            //                {
            //                    object argIndex17 = k;
            //                    {
            //                        var withBlock8 = withBlock7.OtherForm(argIndex17);
            //                        var loopTo37 = withBlock8.CountWeapon();
            //                        for (l = 1; l <= loopTo37; l++)
            //                        {
            //                            if ((u.Weapon(j).Name ?? "") == (withBlock8.Weapon(l).Name ?? ""))
            //                            {
            //                                u.SetBullet(j, (u.Bullet(j) + withBlock8.Bullet(l)));
            //                                goto NextWeapon;
            //                            }
            //                        }
            //                    }
            //                }

            //            NextWeapon:
            //                ;
            //            }

            //            // アビリティの使用回数の処理
            //            var loopTo38 = u.CountAbility();
            //            for (j = 1; j <= loopTo38; j++)
            //            {
            //                var loopTo39 = withBlock7.CountAbility();
            //                for (k = 1; k <= loopTo39; k++)
            //                {
            //                    if ((u.Ability(j).Name ?? "") == (withBlock7.Ability(k).Name ?? ""))
            //                    {
            //                        u.SetStock(j, (u.Stock(j) + withBlock7.Stock(k)));
            //                        goto NextAbility;
            //                    }
            //                }

            //                var loopTo40 = withBlock7.CountOtherForm();
            //                for (k = 1; k <= loopTo40; k++)
            //                {
            //                    object argIndex18 = k;
            //                    {
            //                        var withBlock9 = withBlock7.OtherForm(argIndex18);
            //                        var loopTo41 = withBlock9.CountAbility();
            //                        for (l = 1; l <= loopTo41; l++)
            //                        {
            //                            if ((u.Ability(j).Name ?? "") == (withBlock9.Ability(l).Name ?? ""))
            //                            {
            //                                u.SetStock(j, (u.Stock(j) + withBlock9.Stock(l)));
            //                                goto NextAbility;
            //                            }
            //                        }
            //                    }
            //                }

            //            NextAbility:
            //                ;
            //            }
            //        }
            //    }

            //    // １番目のユニットのアイテムを外す
            //    {
            //        var withBlock10 = rarray[1];
            //        var loopTo42 = withBlock10.CountItem();
            //        for (i = 1; i <= loopTo42; i++)
            //        {
            //            object argIndex19 = 1;
            //            withBlock10.DeleteItem(argIndex19);
            //        }
            //    }

            //    // 合体後のユニットに関する処理
            //    u.Update();
            //    u.Party = Party0;
            //    var loopTo43 = u.CountOtherForm();
            //    for (i = 1; i <= loopTo43; i++)
            //    {
            //        Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(argIndex1); return ret; }

            //        localOtherForm().Party = Party0;
            //    }

            //    var loopTo44 = u.CountPilot();
            //    for (i = 1; i <= loopTo44; i++)
            //    {
            //        Pilot localPilot1() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

            //        localPilot1().Party = Party0;
            //    }

            //    var loopTo45 = u.CountSupport();
            //    for (i = 1; i <= loopTo45; i++)
            //    {
            //        Pilot localSupport2() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

            //        localSupport2().Party = Party0;
            //    }

            //    u.HP = (int)(u.MaxHP * hp_ratio / 100d / Information.UBound(rarray));
            //    u.EN = (int)(1 * u.MaxEN * en_ratio / 100d / Information.UBound(rarray));

            //    // 弾数・使用回数共有の実現
            //    u.SyncBullet();
            //    if (prev_status == "出撃")
            //    {
            //        u.StandBy(x, y);

            //        // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
            //        string argfname3 = "ノーマルモード";
            //        string argfname4 = "制限時間";
            //        if (u.IsFeatureAvailable(argfname3))
            //        {
            //            string localLIndex11() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //            if (Information.IsNumeric(localLIndex11()))
            //            {
            //                object argIndex21 = "残り時間";
            //                if (u.IsConditionSatisfied(argIndex21))
            //                {
            //                    object argIndex20 = "残り時間";
            //                    u.DeleteCondition(argIndex20);
            //                }

            //                string localLIndex9() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                string localLIndex10() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                string argcname = "残り時間";
            //                string argcdata = "";
            //                u.AddCondition(argcname, Conversions.Toint(localLIndex10()), cdata: argcdata);
            //            }
            //        }
            //        else if (u.IsFeatureAvailable(argfname4))
            //        {
            //            string argcname1 = "残り時間";
            //            object argIndex22 = "制限時間";
            //            object argIndex23 = "制限時間";
            //            string argcdata1 = "";
            //            u.AddCondition(argcname1, Conversions.Toint(u.FeatureData(argIndex23)), cdata: argcdata1);
            //        }
            //    }
            //    else
            //    {
            //        u.Status_Renamed = prev_status;
            //    }

            //    // 分離ユニットの座標を合体後のユニットの座標に合わせる
            //    var loopTo46 = Information.UBound(rarray);
            //    for (i = 1; i <= loopTo46; i++)
            //    {
            //        {
            //            var withBlock11 = rarray[i].CurrentForm();
            //            withBlock11.x = u.x;
            //            withBlock11.y = u.y;
            //        }
            //    }
        }

        // 分離
        // UPGRADE_NOTE: Split は Split_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public void Split_Renamed()
        {
            //int k, i, j, l;
            //int idx, n;
            //string buf;
            //Unit[] uarray;
            //double hp_ratio, en_ratio;
            //string pname;
            //Pilot p;
            //hp_ratio = 100 * HP / (double)MaxHP;
            //en_ratio = 100 * EN / (double)MaxEN;

            //// まずは撤退
            //if (Status_Renamed == "出撃")
            //{
            //    // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Map.MapDataForUnit[x, y] = null;
            //    GUI.EraseUnitBitmap(x, y);
            //}

            //// 分離先のユニットを調べる
            //object argIndex1 = "分離";
            //buf = FeatureData(argIndex1);
            //uarray = new Unit[(GeneralLib.LLength(buf))];
            //var loopTo = GeneralLib.LLength(buf);
            //for (i = 2; i <= loopTo; i++)
            //{
            //    object argIndex2 = GeneralLib.LIndex(buf, i);
            //    uarray[i - 1] = SRC.UList.Item(argIndex2);
            //    if (uarray[i - 1] is null)
            //    {
            //        string argmsg = GeneralLib.LIndex(buf, (i - 1)) + "が存在しません";
            //        GUI.ErrorMessage(argmsg);
            //        return;
            //    }
            //}

            //// 分離後の１番機を検索
            //var loopTo1 = Information.UBound(uarray);
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    if (uarray[i].Status_Renamed == "旧主形態")
            //    {
            //        break;
            //    }
            //}

            //if (i > Information.UBound(uarray))
            //{
            //    i = 1;
            //}

            //// １番機に現在のステータスを継承
            //CopySpecialPowerInEffect(uarray[i]);
            //RemoveAllSpecialPowerInEffect();
            //{
            //    var withBlock = uarray[i];
            //    var loopTo2 = CountItem();
            //    for (j = 1; j <= loopTo2; j++)
            //    {
            //        Item localItem() { object argIndex1 = j; var ret = Item(argIndex1); return ret; }

            //        var argitm = localItem();
            //        withBlock.AddItem(argitm);
            //    }

            //    var loopTo3 = CountItem();
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        object argIndex3 = 1;
            //        DeleteItem(argIndex3);
            //    }

            //    withBlock.Master = Master;
            //    // UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Master = null;
            //    withBlock.Summoner = Summoner;
            //    // UPGRADE_NOTE: オブジェクト Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Summoner = null;
            //    withBlock.UsedSupportAttack = UsedSupportAttack;
            //    withBlock.UsedSupportGuard = UsedSupportGuard;
            //    withBlock.UsedSyncAttack = UsedSyncAttack;
            //    withBlock.UsedCounterAttack = UsedCounterAttack;
            //    var loopTo4 = CountServant();
            //    for (j = 1; j <= loopTo4; j++)
            //    {
            //        Unit localServant() { object argIndex1 = j; var ret = Servant(argIndex1); return ret; }

            //        var argu = localServant();
            //        withBlock.AddServant(argu);
            //    }

            //    var loopTo5 = CountServant();
            //    for (j = 1; j <= loopTo5; j++)
            //    {
            //        object argIndex4 = 1;
            //        DeleteServant(argIndex4);
            //    }

            //    var loopTo6 = CountSlave();
            //    for (j = 1; j <= loopTo6; j++)
            //    {
            //        Unit localSlave() { object argIndex1 = j; var ret = Slave(argIndex1); return ret; }

            //        var argu1 = localSlave();
            //        withBlock.AddSlave(argu1);
            //    }

            //    var loopTo7 = CountSlave();
            //    for (j = 1; j <= loopTo7; j++)
            //    {
            //        object argIndex5 = 1;
            //        DeleteSlave(argIndex5);
            //    }
            //}

            //// 各分離ユニットに対する処理
            //n = 1;
            //int counter;
            //var loopTo8 = Information.UBound(uarray);
            //for (i = 1; i <= loopTo8; i++)
            //{
            //    {
            //        var withBlock1 = uarray[i];
            //        // 召喚ユニットでない場合は陣営を合わせる
            //        string argfname = "召喚ユニット";
            //        if (!withBlock1.IsFeatureAvailable(argfname))
            //        {
            //            withBlock1.Party = Party0;
            //        }

            //        // パイロットの搭乗
            //        if (CountPilot() > 0)
            //        {
            //            var loopTo9 = Math.Abs(withBlock1.Data.PilotNum);
            //            for (j = 1; j <= loopTo9; j++)
            //            {
            //                string argfname2 = "召喚ユニット";
            //                if (withBlock1.IsFeatureAvailable(argfname2))
            //                {
            //                    if (Status_Renamed == "出撃" | Status_Renamed == "格納")
            //                    {
            //                        object argIndex6 = "追加パイロット";
            //                        pname = withBlock1.FeatureData(argIndex6);
            //                        PilotData localItem1() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        PilotData localItem2() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        if (Strings.InStr(localItem1().Name, "(ザコ)") > 0 | Strings.InStr(localItem2().Name, "(汎用)") > 0)
            //                        {
            //                            string argpparty = Party;
            //                            string arggid = "";
            //                            p = SRC.PList.Add(pname, MainPilot().Level, argpparty, gid: arggid);
            //                            Party = argpparty;
            //                            p.FullRecover();
            //                        }
            //                        else
            //                        {
            //                            bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                            if (!localIsDefined())
            //                            {
            //                                string argpparty1 = Party;
            //                                string arggid1 = "";
            //                                p = SRC.PList.Add(pname, MainPilot().Level, argpparty1, gid: arggid1);
            //                                Party = argpparty1;
            //                                p.FullRecover();
            //                            }
            //                            else
            //                            {
            //                                object argIndex7 = pname;
            //                                p = SRC.PList.Item(argIndex7);
            //                            }
            //                        }

            //                        p.Ride(uarray[i]);
            //                    }
            //                }
            //                else
            //                {
            //                    string argfname1 = "追加パイロット";
            //                    if (n <= CountPilot())
            //                    {
            //                        Pilot localPilot() { object argIndex1 = n; var ret = Pilot(argIndex1); return ret; }

            //                        localPilot().Ride(uarray[i]);
            //                        n = (n + 1);
            //                    }
            //                    else if (!withBlock1.IsFeatureAvailable(argfname1))
            //                    {
            //                        if (CountSupport() > 0)
            //                        {
            //                            object argIndex8 = 1;
            //                            Support(argIndex8).Ride(uarray[i]);
            //                            object argIndex9 = 1;
            //                            DeleteSupport(argIndex9);
            //                        }
            //                        else
            //                        {
            //                            string argmsg1 = Name + "分離後のユニットに載せる" + "パイロットが存在しません。" + "データのパイロット数を確認して下さい。";
            //                            GUI.ErrorMessage(argmsg1);
            //                            SRC.TerminateSRC();
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        withBlock1.Update();

            //        // 母艦の場合は格納したユニットを受け渡し
            //        string argfname3 = "母艦";
            //        if (withBlock1.IsFeatureAvailable(argfname3))
            //        {
            //            var loopTo10 = CountUnitOnBoard();
            //            for (j = 1; j <= loopTo10; j++)
            //            {
            //                Unit localUnitOnBoard() { object argIndex1 = j; var ret = UnitOnBoard(argIndex1); return ret; }

            //                var argu2 = localUnitOnBoard();
            //                withBlock1.LoadUnit(argu2);
            //            }

            //            var loopTo11 = CountUnitOnBoard();
            //            for (j = 1; j <= loopTo11; j++)
            //            {
            //                object argIndex10 = 1;
            //                UnloadUnit(argIndex10);
            //            }
            //        }

            //        // ＨＰ＆ＥＮの同期
            //        withBlock1.HP = (int)(withBlock1.MaxHP * hp_ratio / 100d);
            //        withBlock1.EN = (int)(1 * withBlock1.MaxEN * en_ratio / 100d);

            //        // 弾数を合わせる
            //        idx = 1;
            //        var loopTo12 = CountWeapon();
            //        for (j = 1; j <= loopTo12; j++)
            //        {
            //            counter = idx;
            //            var loopTo13 = withBlock1.CountWeapon();
            //            for (k = counter; k <= loopTo13; k++)
            //            {
            //                if ((Weapon(j).Name ?? "") == (withBlock1.Weapon(k).Name ?? "") & this.Weapon(j).Bullet > 0 & withBlock1.Weapon(k).Bullet > 0)
            //                {
            //                    withBlock1.SetBullet(k, ((withBlock1.MaxBullet(k) * Bullet(j)) / MaxBullet(j)));
            //                    idx = (k + 1);
            //                    break;
            //                }
            //            }
            //        }

            //        var loopTo14 = withBlock1.CountOtherForm();
            //        for (j = 1; j <= loopTo14; j++)
            //        {
            //            object argIndex11 = j;
            //            {
            //                var withBlock2 = withBlock1.OtherForm(argIndex11);
            //                idx = 1;
            //                var loopTo15 = CountWeapon();
            //                for (k = 1; k <= loopTo15; k++)
            //                {
            //                    counter = idx;
            //                    var loopTo16 = withBlock2.CountWeapon();
            //                    for (l = counter; l <= loopTo16; l++)
            //                    {
            //                        if ((Weapon(k).Name ?? "") == (withBlock2.Weapon(l).Name ?? "") & this.Weapon(k).Bullet > 0 & withBlock2.Weapon(l).Bullet > 0)
            //                        {
            //                            withBlock2.SetBullet(l, ((withBlock2.MaxBullet(l) * Bullet(k)) / MaxBullet(k)));
            //                            idx = (l + 1);
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        // 使用回数を合わせる
            //        idx = 1;
            //        var loopTo17 = CountAbility();
            //        for (j = 1; j <= loopTo17; j++)
            //        {
            //            counter = idx;
            //            var loopTo18 = withBlock1.CountAbility();
            //            for (k = counter; k <= loopTo18; k++)
            //            {
            //                if ((Ability(j).Name ?? "") == (withBlock1.Ability(k).Name ?? "") & this.Ability(j).Stock > 0 & withBlock1.Ability(k).Stock > 0)
            //                {
            //                    withBlock1.SetStock(k, ((withBlock1.Ability(k).Stock * Stock(j)) / MaxStock(j)));
            //                    idx = (k + 1);
            //                    break;
            //                }
            //            }
            //        }

            //        var loopTo19 = withBlock1.CountOtherForm();
            //        for (j = 1; j <= loopTo19; j++)
            //        {
            //            object argIndex12 = j;
            //            {
            //                var withBlock3 = withBlock1.OtherForm(argIndex12);
            //                idx = 1;
            //                var loopTo20 = CountAbility();
            //                for (k = 1; k <= loopTo20; k++)
            //                {
            //                    counter = idx;
            //                    var loopTo21 = withBlock3.CountAbility();
            //                    for (l = counter; l <= loopTo21; l++)
            //                    {
            //                        if ((Ability(k).Name ?? "") == (withBlock3.Ability(l).Name ?? "") & this.Ability(k).Stock > 0 & withBlock3.Ability(l).Stock > 0)
            //                        {
            //                            withBlock3.SetStock(l, ((withBlock3.Ability(l).Stock * Stock(k)) / MaxStock(k)));
            //                            idx = (l + 1);
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        // 弾数・使用回数共有の実現
            //        withBlock1.SyncBullet();

            //        // 出撃 or 格納？
            //        withBlock1.Status_Renamed = Status_Renamed;
            //        switch (Status_Renamed ?? "")
            //        {
            //            case "出撃":
            //                {
            //                    if (i == 1)
            //                    {
            //                        withBlock1.UsedAction = UsedAction;
            //                    }
            //                    else
            //                    {
            //                        withBlock1.UsedAction = GeneralLib.MaxLng(UsedAction, withBlock1.UsedAction);
            //                        withBlock1.UsedSupportAttack = 0;
            //                        withBlock1.UsedSupportGuard = 0;
            //                        withBlock1.UsedSyncAttack = 0;
            //                        withBlock1.UsedCounterAttack = 0;
            //                    }

            //                    withBlock1.StandBy(x, y);
            //                    break;
            //                }

            //            case "格納":
            //                {
            //                    foreach (Unit eu in SRC.UList)
            //                    {
            //                        var loopTo22 = eu.CountOtherForm();
            //                        for (j = 1; j <= loopTo22; j++)
            //                        {
            //                            Unit localUnitOnBoard1() { object argIndex1 = j; var ret = eu.UnitOnBoard(argIndex1); return ret; }

            //                            if ((ID ?? "") == (localUnitOnBoard1().ID ?? ""))
            //                            {
            //                                eu.LoadUnit(uarray[i]);
            //                                goto EndLoop;
            //                            }
            //                        }
            //                    }

            //                EndLoop:
            //                    ;
            //                    break;
            //                }
            //        }

            //        // ノーマルモードや制限時間つきの形態の場合は残り時間を付加
            //        string argfname4 = "ノーマルモード";
            //        string argfname5 = "制限時間";
            //        if (withBlock1.IsFeatureAvailable(argfname4))
            //        {
            //            string localLIndex2() { object argIndex1 = "ノーマルモード"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //            if (Information.IsNumeric(localLIndex2()))
            //            {
            //                object argIndex14 = "残り時間";
            //                if (withBlock1.IsConditionSatisfied(argIndex14))
            //                {
            //                    object argIndex13 = "残り時間";
            //                    withBlock1.DeleteCondition(argIndex13);
            //                }

            //                string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                string localLIndex1() { object argIndex1 = "ノーマルモード"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                string argcname = "残り時間";
            //                string argcdata = "";
            //                withBlock1.AddCondition(argcname, Conversions.Toint(localLIndex1()), cdata: argcdata);
            //            }
            //        }
            //        else if (withBlock1.IsFeatureAvailable(argfname5))
            //        {
            //            string argcname1 = "残り時間";
            //            object argIndex15 = "制限時間";
            //            object argIndex16 = "制限時間";
            //            string argcdata1 = "";
            //            withBlock1.AddCondition(argcname1, Conversions.Toint(withBlock1.FeatureData(argIndex16)), cdata: argcdata1);
            //        }
            //    }
            //}

            //// パイロットを合体ユニットから削除
            //var loopTo23 = CountPilot();
            //for (i = 1; i <= loopTo23; i++)
            //{
            //    object argIndex17 = 1;
            //    DeletePilot(argIndex17);
            //}

            //// サポートパイロットの乗り換え
            //var loopTo24 = CountSupport();
            //for (i = 1; i <= loopTo24; i++)
            //{
            //    object argIndex18 = i;
            //    {
            //        var withBlock4 = Support(argIndex18);
            //        if (withBlock4.SupportIndex == 0)
            //        {
            //            Unit localItem3() { object argIndex1 = GeneralLib.LIndex(buf, 2); var ret = SRC.UList.Item(argIndex1); return ret; }

            //            var argu3 = localItem3();
            //            withBlock4.Ride(argu3);
            //        }
            //        else
            //        {
            //            withBlock4.Ride(uarray[withBlock4.SupportIndex]);
            //        }
            //    }
            //}

            //var loopTo25 = CountSupport();
            //for (i = 1; i <= loopTo25; i++)
            //{
            //    object argIndex19 = 1;
            //    DeleteSupport(argIndex19);
            //}

            //// 格納されている場合は母艦から自分のエントリーを外しておく
            //if (Status_Renamed == "格納")
            //{
            //    foreach (Unit u in SRC.UList)
            //    {
            //        var loopTo26 = u.CountUnitOnBoard();
            //        for (j = 1; j <= loopTo26; j++)
            //        {
            //            Unit localUnitOnBoard2() { object argIndex1 = j; var ret = u.UnitOnBoard(argIndex1); return ret; }

            //            if ((ID ?? "") == (localUnitOnBoard2().ID ?? ""))
            //            {
            //                object argIndex20 = ID;
            //                u.UnloadUnit(argIndex20);
            //                goto EndLoop2;
            //            }
            //        }
            //    }

            //EndLoop2:
            //    ;
            //}

            //Status_Renamed = "他形態";

            //// ユニットステータスコマンドの場合以外は制限時間付き合体ユニットは
            //// ２度とその形態を利用できない
            //if (string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    return;
            //}

            //string argfname6 = "制限時間";
            //if (IsFeatureAvailable(argfname6))
            //{
            //    string argcname2 = "行動不能";
            //    string argcdata2 = "";
            //    AddCondition(argcname2, -1, cdata: argcdata2);
            //}
        }
    }
}
