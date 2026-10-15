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
                SRC.UList.Unload(this);
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

            // 登場時アニメを表示
            string fname;
            int start_time, current_time;
            if ((Strings.InStr(smode, "出撃") > 0 || Strings.InStr(smode, "部隊配置") > 0)
                && GUI.MainFormVisible && !GUI.IsPictureVisible && !GUI.IsRButtonPressed() && !string.IsNullOrEmpty(get_Bitmap(false)))
            {
                // ユニット出現音
                Sound.PlayWave("UnitOn.wav");

                // 表示させる画像
                switch (Party0 ?? "")
                {
                    case "味方":
                    case "ＮＰＣ":
                        fname = @"Bitmap\Event\AUnitOn0";
                        break;

                    case "敵":
                        fname = @"Bitmap\Event\EUnitOn0";
                        break;

                    default:
                        fname = @"Bitmap\Event\NUnitOn0";
                        break;
                }

                if (SRC.FileSystem.FileExists(SRC.FileSystem.PathCombine(SRC.AppPath, fname + "1.bmp")))
                {
                    // アニメ表示開始時刻を記録
                    start_time = GeneralLib.timeGetTime();
                    for (int i = 1; i <= 4; i++)
                    {
                        // 画像を透過表示
                        if (!GUI.DrawPicture(fname + SrcFormatter.Format(i) + ".bmp", GUI.MapToPixelX(x), GUI.MapToPixelY(y), 32, 32, 0, 0, 0, 0, "透過"))
                        {
                            break;
                        }

                        // ウェイト
                        do
                        {
                            GUI.DoEvents();
                            current_time = GeneralLib.timeGetTime();
                        }
                        while (current_time < start_time + 15);
                        start_time = current_time;

                        // 画像を消去
                        GUI.ClearPicture();
                    }

                    // アニメ画像は上書きして消してしまうので……
                    GUI.IsPictureVisible = false;
                }
            }

            // ユニット画像をマップに描画
            if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            {
                if (Strings.InStr(smode, "非同期") > 0)
                {
                    GUI.PaintUnitBitmap(this, "リフレッシュ無し");
                }
                else
                {
                    GUI.PaintUnitBitmap(this);
                }
            }

            // 制御不能？
            if (IsFeatureAvailable("制御不可"))
            {
                AddCondition("暴走", -1, cdata: "");
            }
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
                // 気力減少
                {
                    var mp = MainPilot();
                    if (mp.Personality != "機械")
                    {
                        if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                        {
                            mp.Morale = GeneralLib.MinLng(mp.Morale, GeneralLib.MaxLng(mp.Morale - 5, 100));
                        }
                        else
                        {
                            mp.Morale = (mp.Morale - 5);
                        }
                    }
                }

                foreach (var p in SubPilots)
                {
                    if (p.ID != MainPilot().ID && p.Personality != "機械")
                    {
                        if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                        {
                            p.Morale = GeneralLib.MinLng(p.Morale, GeneralLib.MaxLng(p.Morale - 5, 100));
                        }
                        else
                        {
                            p.Morale = (p.Morale - 5);
                        }
                    }
                }

                foreach (var sp in Supports)
                {
                    if (sp.Personality != "機械")
                    {
                        if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                        {
                            sp.Morale = GeneralLib.MinLng(sp.Morale, GeneralLib.MaxLng(sp.Morale - 5, 100));
                        }
                        else
                        {
                            sp.Morale = (sp.Morale - 5);
                        }
                    }
                }

                if (IsFeatureAvailable("追加サポート"))
                {
                    var asp = AdditionalSupport();
                    if (asp != null && asp.Personality != "機械")
                    {
                        if (Expression.IsOptionDefined("母艦収納時気力低下小"))
                        {
                            asp.Morale = GeneralLib.MinLng(asp.Morale, GeneralLib.MaxLng(asp.Morale - 5, 100));
                        }
                        else
                        {
                            asp.Morale = (asp.Morale - 5);
                        }
                    }
                }
            }
        }

        // new_form へ変形（換装、ハイパーモード、パーツ分離＆合体を含む）
        public void Transform(string new_form)
        {
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
                for (var ii = 1; ii <= CountItem(); ii++)
                {
                    u.AddItem0(Item(ii));
                }

                // スペシャルパワー効果のコピー
                CopySpecialPowerInEffect(u);
                RemoveAllSpecialPowerInEffect();

                // 特殊ステータスのコピー
                u.ClearCondition();
                foreach (var condition in Conditions
                    .Where(x => x.Lifetime != 0)
                    .Where(x => Strings.InStr(x.StrData, "パイロット能力付加") == 0)
                    .Where(x => Strings.InStr(x.StrData, "パイロット能力強化") == 0))
                {
                    u.AddCondition(condition.Name, condition.Lifetime, condition.Level, condition.StrData);
                }
                ClearCondition();

                // パイロットの乗せ換え
                var list = FeatureData("変形");
                var llen = GeneralLib.LLength(list);
                var pilots = Pilots.ToList();
                var reordered = false;
                if (llen > 0
                    && Data.PilotNum == -llen
                    && pilots.Count == llen)
                {
                    // 変形によりパイロットの順番が変化する場合
                    int idx;
                    for (idx = 2; idx <= llen; idx++)
                    {
                        if (u.Name == GeneralLib.LIndex(list, idx))
                        {
                            break;
                        }
                    }

                    if (idx <= llen)
                    {
                        var tlist = u.FeatureData("変形");
                        var tllen = GeneralLib.LLength(tlist);
                        int idx2;
                        for (idx2 = 2; idx2 <= tllen; idx2++)
                        {
                            if (Name == GeneralLib.LIndex(tlist, idx2))
                            {
                                break;
                            }
                        }

                        var j = 2;
                        for (var i = 1; i <= pilots.Count; i++)
                        {
                            if (i == 1)
                            {
                                u.AddPilot(pilots[idx - 1]);
                            }
                            else if (i == idx2)
                            {
                                u.AddPilot(pilots[0]);
                            }
                            else
                            {
                                if (idx == j)
                                {
                                    j = j + 1;
                                }

                                u.AddPilot(pilots[j - 1]);
                                j = j + 1;
                            }
                        }
                        reordered = true;
                    }
                }

                if (!reordered)
                {
                    foreach (var p in pilots)
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

                foreach (var su in Servants)
                {
                    u.AddServant(su);
                }

                foreach (var su in colSlave.List)
                {
                    u.AddSlave(su);
                }

                colPilot.Clear();
                colSupport.Clear();
                colUnitOnBoard.Clear();
                colServant.Clear();
                colSlave.Clear();

                foreach (var p in u.Pilots)
                {
                    p.Unit = u;
                }

                foreach (var p in u.Supports)
                {
                    p.Unit = u;
                    // 合体ロボットの変形時に分離先のパイロットのインデックスを合わせる
                    if (p.SupportIndex > 0)
                    {
                        if (IsFeatureAvailable("分離") && u.IsFeatureAvailable("分離"))
                        {
                            var cuForm = GeneralLib.LIndex(FeatureData("分離"), p.SupportIndex + 1);
                            var tuForms = GeneralLib.ToL(u.FeatureData("分離")).Skip(1).ToList();
                            for (var tuIndex = 0; tuIndex < tuForms.Count; tuIndex++)
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

                // 弾数データを記録
                var wnames = new string[CountWeapon() + 1];
                var wbullets = new int[CountWeapon() + 1];
                var wmaxbullets = new int[CountWeapon() + 1];
                for (var wi = 1; wi <= CountWeapon(); wi++)
                {
                    wnames[wi] = Weapon(wi).Name;
                    wbullets[wi] = Weapon(wi).Bullet();
                    wmaxbullets[wi] = Weapon(wi).MaxBullet();
                }

                var anames = new string[CountAbility() + 1];
                var astocks = new int[CountAbility() + 1];
                var amaxstocks = new int[CountAbility() + 1];
                for (var ai = 1; ai <= CountAbility(); ai++)
                {
                    anames[ai] = Ability(ai).Data.Name;
                    astocks[ai] = Ability(ai).Stock();
                    amaxstocks[ai] = Ability(ai).MaxStock();
                }

                // 弾数の受け継ぎ
                {
                    var idx = 1;
                    for (var wi = 1; wi <= u.CountWeapon(); wi++)
                    {
                        for (var j = idx; j < wnames.Length; j++)
                        {
                            if (u.Weapon(wi).Name == wnames[j] && u.Weapon(wi).MaxBullet() > 0 && wmaxbullets[j] > 0)
                            {
                                u.Weapon(wi).SetBullet(wbullets[j] * u.Weapon(wi).MaxBullet() / wmaxbullets[j]);
                                idx = j + 1;
                                break;
                            }
                        }
                    }
                }

                {
                    var idx = 1;
                    for (var ai = 1; ai <= u.CountAbility(); ai++)
                    {
                        for (var j = idx; j < anames.Length; j++)
                        {
                            if (u.Ability(ai).Data.Name == anames[j] && u.Ability(ai).MaxStock() > 0 && amaxstocks[j] > 0)
                            {
                                u.Ability(ai).SetStock(astocks[j] * u.Ability(ai).MaxStock() / amaxstocks[j]);
                                idx = j + 1;
                                break;
                            }
                        }
                    }
                }

                // 弾数・使用回数共有の実現
                u.SyncBullet();

                // アイテムを削除
                var itemCount = CountItem();
                for (var ii = 1; ii <= itemCount; ii++)
                {
                    DeleteItem(Item(1));
                }

                u.Update();

                // ＨＰ＆ＥＮの受け継ぎ
                if ((new_form ?? "") == (GeneralLib.LIndex(FeatureData("パーツ分離"), 2) ?? ""))
                {
                    u.HP = u.MaxHP;
                }
                else
                {
                    u.HP = VBMath.CLng(u.MaxHP * hp_ratio / 100d);
                }

                u.EN = VBMath.CLng(u.MaxEN * en_ratio / 100d);

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
            var buf = FeatureData("ノーマルモード");
            if ((GeneralLib.LIndex(buf, 1) ?? "") == (new_form ?? ""))
            {
                foreach (var opt in GeneralLib.ToL(buf).Skip(1))
                {
                    switch (opt ?? "")
                    {
                        case "回数制限":
                            AddCondition("行動不能", -1, cdata: "");
                            break;
                    }
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

                    // 合体ＢＧＭの再生
                    if (IsMessageDefined("合体(" + u.Name + ")") || IsMessageDefined("合体(" + FeatureName("合体") + ")") || IsMessageDefined("合体"))
                    {
                        if (IsFeatureAvailable("合体ＢＧＭ"))
                        {
                            for (var bgmIdx = 1; bgmIdx <= CountFeature(); bgmIdx++)
                            {
                                if (Feature(bgmIdx).Name == "合体ＢＧＭ" && (GeneralLib.LIndex(FeatureData(bgmIdx), 1) ?? "") == (u.Name ?? ""))
                                {
                                    var fdata = FeatureData(bgmIdx);
                                    BGM = Sound.SearchMidiFile(Strings.Mid(fdata, Strings.InStr(fdata, " ") + 1));
                                    if (Strings.Len(BGM) > 0)
                                    {
                                        Sound.ChangeBGM(BGM);
                                        GUI.Sleep(500);
                                    }

                                    break;
                                }
                            }
                        }

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
            }

            // 分離ユニットと合体ユニットが同名の武器を持つ場合は弾数を累積するため
            // このような武器の弾数を0にする
            foreach (var uw in u.Weapons)
            {
                foreach (var ru in runits)
                {
                    if (ru.CurrentForm().Weapons.Any(rw => rw.Name == uw.Name))
                    {
                        uw.SetBullet(0);
                    }
                }
            }
            // 使用回数を合わせる
            foreach (var ua in u.Abilities)
            {
                foreach (var ru in runits)
                {
                    if (ru.CurrentForm().Abilities.Any(ra => ra.Data.Name == ua.Data.Name))
                    {
                        ua.SetStock(0);
                    }
                }
            }

            // １番目のユニットのステータスを合体後のユニットに継承
            {
                var ru = runits.First().CurrentForm();
                ru.CopySpecialPowerInEffect(u);
                ru.RemoveAllSpecialPowerInEffect();
                for (var ii = 1; ii <= ru.CountItem(); ii++)
                {
                    u.AddItem(ru.Item(ii));
                }

                u.Master = ru.Master;
                ru.Master = null;
                u.Summoner = ru.Summoner;
                ru.Summoner = null;
                u.UsedSupportAttack = ru.UsedSupportAttack;
                u.UsedSupportGuard = ru.UsedSupportGuard;
                u.UsedSyncAttack = ru.UsedSyncAttack;
                u.UsedCounterAttack = ru.UsedCounterAttack;
                foreach (var su in ru.Servants)
                {
                    u.AddServant(su);
                }
                ru.colServant.Clear();
                foreach (var su in ru.colSlave.List)
                {
                    u.AddSlave(su);
                }
                ru.colSlave.Clear();
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
                        SRC.UList.Unload(currentForm);
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
                foreach (var lu in ru.UnitOnBoards.ToList())
                {
                    u.LoadUnit(lu);
                }
                ru.colUnitOnBoard.Clear();

                // 分離ユニットと共通する武装の弾数は一旦0にクリア
                foreach (var uw in u.Weapons)
                {
                    if (ru.Weapons.Any(rw => rw.Name == uw.Name)
                        || ru.OtherForms.Any(of => of.Weapons.Any(ow => ow.Name == uw.Name)))
                    {
                        uw.SetBullet(0);
                    }
                }

                // アビリティの使用回数も同様の処理を行う
                foreach (var ua in u.Abilities)
                {
                    if (ru.Abilities.Any(ra => ra.Data.Name == ua.Data.Name)
                        || ru.OtherForms.Any(of => of.Abilities.Any(oa => oa.Data.Name == ua.Data.Name)))
                    {
                        ua.SetStock(0);
                    }
                }

                // スペシャルパワーの効果を消去
                ru.RemoveAllSpecialPowerInEffect();
            }

            // 合体後のユニットの武装の弾数及びアビリティの使用回数は分離ユニットの
            // 弾数及び使用回数の合計に設定する
            foreach (var ru in runits)
            {
                // 武装の弾数の処理
                foreach (var uw in u.Weapons)
                {
                    var rw = ru.Weapons.FirstOrDefault(x => x.Name == uw.Name)
                        ?? ru.OtherForms.SelectMany(of => of.Weapons).FirstOrDefault(x => x.Name == uw.Name);
                    if (rw != null)
                    {
                        uw.SetBullet(uw.Bullet() + rw.Bullet());
                    }
                }

                // アビリティの使用回数の処理
                foreach (var ua in u.Abilities)
                {
                    var ra = ru.Abilities.FirstOrDefault(x => x.Data.Name == ua.Data.Name)
                        ?? ru.OtherForms.SelectMany(of => of.Abilities).FirstOrDefault(x => x.Data.Name == ua.Data.Name);
                    if (ra != null)
                    {
                        ua.SetStock(ua.Stock() + ra.Stock());
                    }
                }
            }

            // １番目のユニットのアイテムを外す
            {
                var ru = runits.First();
                var itemCount = ru.CountItem();
                for (var ii = 1; ii <= itemCount; ii++)
                {
                    ru.DeleteItem(ru.Item(1));
                }
            }

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

            u.HP = VBMath.CLng(u.MaxHP * hp_ratio / 100d / runits.Count);
            u.EN = VBMath.CLng(u.MaxEN * en_ratio / 100d / runits.Count);

            // 弾数・使用回数共有の実現
            u.SyncBullet();

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

        // 分離後のユニット tu の武装の弾数を分離前の弾数の割合に合わせる
        private void SyncSplitBullet(Unit tu)
        {
            var idx = 0;
            foreach (var w in Weapons)
            {
                for (var k = idx; k < tu.Weapons.Count; k++)
                {
                    var tw = tu.Weapons[k];
                    if (w.Name == tw.Name && w.WeaponData.Bullet > 0 && tw.WeaponData.Bullet > 0)
                    {
                        tw.SetBullet(tw.MaxBullet() * w.Bullet() / w.MaxBullet());
                        idx = k + 1;
                        break;
                    }
                }
            }
        }

        // 分離後のユニット tu のアビリティの使用回数を分離前の使用回数の割合に合わせる
        private void SyncSplitStock(Unit tu)
        {
            var idx = 0;
            foreach (var a in Abilities)
            {
                for (var k = idx; k < tu.Abilities.Count; k++)
                {
                    var ta = tu.Abilities[k];
                    if (a.Data.Name == ta.Data.Name && a.Data.Stock > 0 && ta.Data.Stock > 0)
                    {
                        // SRC本体と同様に使用回数の基本値を基準にする
                        ta.SetStock(ta.Data.Stock * a.Stock() / a.MaxStock());
                        idx = k + 1;
                        break;
                    }
                }
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
                // アイテム受け継ぎ
                for (var j = 1; j <= CountItem(); j++)
                {
                    firstUnit.AddItem0(Item(j));
                }

                var splitItemCount = CountItem();
                for (var j = 1; j <= splitItemCount; j++)
                {
                    DeleteItem(Item(1));
                }

                firstUnit.Master = Master;
                Master = null;
                firstUnit.Summoner = Summoner;
                Summoner = null;
                firstUnit.UsedSupportAttack = UsedSupportAttack;
                firstUnit.UsedSupportGuard = UsedSupportGuard;
                firstUnit.UsedSyncAttack = UsedSyncAttack;
                firstUnit.UsedCounterAttack = UsedCounterAttack;
                foreach (var su in Servants)
                {
                    firstUnit.AddServant(su);
                }
                colServant.Clear();
                foreach (var su in colSlave.List)
                {
                    firstUnit.AddSlave(su);
                }
                colSlave.Clear();
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
                u.HP = VBMath.CLng(u.MaxHP * hp_ratio / 100d);
                u.EN = VBMath.CLng(u.MaxEN * en_ratio / 100d);

                // 弾数を合わせる
                SyncSplitBullet(u);
                foreach (var of in u.OtherForms)
                {
                    SyncSplitBullet(of);
                }

                // 使用回数を合わせる
                SyncSplitStock(u);
                foreach (var of in u.OtherForms)
                {
                    SyncSplitStock(of);
                }

                // 弾数・使用回数共有の実現
                u.SyncBullet();

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
