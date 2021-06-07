using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.IO;
using System.Linq;

namespace SRCCore
{
    public partial class SRC
    {
        // イベントファイルfnameを実行
        public void StartScenario(string fname)
        {
            // ファイルを検索
            if (!Path.IsPathRooted(fname))
            {
                fname = new string[]
                {
                    FileSystem.PathCombine(ScenarioPath, fname),
                    FileSystem.PathCombine(AppPath, fname),
                }.FirstOrDefault(x => FileSystem.FileExists(x)) ?? fname;
            }

            if (!FileSystem.FileExists(fname))
            {
                GUI.ErrorMessage(fname + "が見つかりません");
                TerminateSRC();
            }

            //// ウィンドウのタイトルを設定
            //if (My.MyProject.Application.Info.Version.Minor % 2 == 0)
            //{
            //    GUI.MainForm.Text = "SRC";
            //}
            //else
            //{
            //    GUI.MainForm.Text = "SRC開発版";
            //}

            ScenarioFileName = fname;
            //if (!IsSubStage)
            //{
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Date", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のDataフォルダ名がDateになっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Date" + Constants.vbCr + Constants.vbLf + "フォルダ名をDataに直してください。");
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｄａｔａ", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のDataフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｄａｔａ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。");
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｂｉｔｍａｐ", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のBitmapフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｂｉｔｍａｐ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。");
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｌｉｂ", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のLibフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｌｉｂ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。");
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｍｉｄｉ", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のMidiフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｍｉｄｉ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。");
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｓｏｕｎｄ", FileAttribute.Directory)) > 0)
            //    {
            //        GUI.ErrorMessage("シナリオ側のSoundフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｓｏｕｎｄ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。");
            //        TerminateSRC();
            //    }

            //    // 読み込むイベントファイル名に合わせて各種システム変数を設定
            //    if (!Expression.IsGlobalVariableDefined("次ステージ"))
            //    {
            //        Expression.DefineGlobalVariable("次ステージ");
            //    }

            //    Expression.SetVariableAsString("次ステージ", "");
            //    var loopTo = Strings.Len(fname);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
            //        {
            //            break;
            //        }
            //    }

            //    Expression.SetVariableAsString("ステージ", Strings.Mid(fname, Strings.Len(fname) - i + 2));
            //    if (!Expression.IsGlobalVariableDefined("セーブデータファイル名"))
            //    {
            //        Expression.DefineGlobalVariable("セーブデータファイル名");
            //    }

            //    Expression.SetVariableAsString("セーブデータファイル名", Strings.Mid(fname, Strings.Len(fname) - i + 2, i - 5) + "までクリア.src");

            //    // ウィンドウのタイトルにシナリオファイル名を表示
            //    GUI.MainForm.Text = GUI.MainForm.Text + " - " + Strings.Mid(fname, Strings.Len(fname) - i + 2, i - 5);
            //}

            // 画面をクリアしておく
            GUI.ClearScrean();

            // イベントデータの読み込み
            Event.LoadEventData(fname, "");

            // 各種変数の初期化
            Turn = 0;
            IsScenarioFinished = false;
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            LastSaveDataFileName = "";
            IsRestartSaveDataAvailable = false;
            IsQuickSaveDataAvailable = false;
            Commands.CommandState = "ユニット選択";
            //Commands.SelectedPartners.Clear();

            //// フォント設定をデフォルトに戻す
            //{
            //    var withBlock1 = GUI.MainForm.picMain(0);
            //    withBlock1.ForeColor = Information.RGB(255, 255, 255);
            //    if (withBlock1.Font.Name != "ＭＳ Ｐ明朝")
            //    {
            //        sf = (Font)Control.DefaultFont.Clone();
            //        sf = SrcFormatter.FontChangeName(sf, "ＭＳ Ｐ明朝");
            //        withBlock1.Font = sf;
            //    }
            //    withBlock1.Font.Size = 16;
            //    withBlock1.Font.Bold = true;
            //    withBlock1.Font.Italic = false;
            //    GUI.PermanentStringMode = false;
            //    GUI.KeepStringMode = false;
            //}

            //// 描画の基準座標位置をリセット
            //Event.ResetBasePoint();

            //// メモリを消費し過ぎないようにユニット画像をクリア
            //if (!IsSubStage)
            //{
            //    UList.ClearUnitBitmap();
            //}

            GUI.LockGUI();
            //if (Map.MapWidth == 1)
            //{
            //    Map.SetMapSize(15, 15);
            //}

            // プロローグ
            Stage = "プロローグ";
            if (!IsSubStage && Event.IsEventDefined("プロローグ", true))
            {
                Sound.StopBGM();
                Sound.StartBGM(Sound.BGMName("Briefing"), true);
            }

            Event.HandleEvent("プロローグ");
            if (IsScenarioFinished)
            {
                IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (!Event.IsEventDefined("スタート"))
            {
                GUI.ErrorMessage("スタートイベントが定義されていません");
                TerminateSRC();
            }

            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            Stage = "味方";
            Sound.StopBGM();

            // リスタート用にデータをセーブ
            // XXX パスセパレータマッチ
            if (Strings.InStr(fname, @"\Lib\ユニットステータス表示.eve") == 0
                && Strings.InStr(fname, @"\Lib\パイロットステータス表示.eve") == 0)
            {
                DumpData(Path.Combine(ScenarioPath, "_リスタート.srcq"), SRCSaveKind.Restart);
            }

            // スタートイベントが始まった場合は通常のステージとみなす
            IsSubStage = false;
            GUIStatus.ClearUnitStatus();
            if (!GUI.MainFormVisible)
            {
                GUI.MainFormShow();
                //GUI.MainForm.Refresh();
            }

            GUI.RedrawScreen();

            // スタートイベント
            Event.HandleEvent("スタート");
            if (IsScenarioFinished)
            {
                IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;

            // クイックロードを無効にする
            IsQuickSaveDataAvailable = false;
            StartTurn("味方");
        }

        // 陣営upartyのフェイズを実行
        public void StartTurn(string uparty)
        {
            // TODO Impl StartTurn
            Stage = uparty;
            Sound.BossBGM = false;
            if (uparty == "味方")
            {
                do
                {
                    // 味方フェイズ
                    Stage = "味方";

                    // ターン数を進める
                    if (!Map.IsStatusView)
                    {
                        Turn = Turn + 1;
                        TotalTurn = TotalTurn + 1;
                    }

                    // 状態回復
                    foreach (var currentUnit in UList.Items)
                    {
                        Commands.SelectedUnit = currentUnit;
                        switch (currentUnit.Status)
                        {
                            case "出撃":
                            case "格納":
                                {
                                    if ((currentUnit.Party ?? "") == (uparty ?? ""))
                                    {
                                        if (Map.IsStatusView)
                                        {
                                            currentUnit.UsedAction = 0;
                                        }
                                        else
                                        {
                                            currentUnit.Rest();
                                        }

                                        if (IsScenarioFinished)
                                        {
                                            GUI.UnlockGUI();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        currentUnit.UsedAction = 0;
                                    }

                                    break;
                                }

                            case "旧主形態":
                            case "旧形態":
                                {
                                    currentUnit.UsedAction = 0;
                                    break;
                                }
                        }
                    }

                    // 味方が敵にかけたスペシャルパワーを解除
                    foreach (var currentU in UList.Items)
                    {
                        switch (currentU.Status)
                        {
                            case "出撃":
                            case "格納":
                                {
                                    currentU.RemoveSpecialPowerInEffect("敵ターン");
                                    break;
                                }
                        }
                    }

                    GUI.RedrawScreen();

                    // 味方フェイズ用ＢＧＭを演奏
                    if (!Map.IsStatusView)
                    {
                        var terrain = Map.Terrain(1, 1);
                        switch (terrain?.Class ?? "")
                        {
                            case "屋内":
                                Sound.StartBGM(Sound.BGMName("Map3"));
                                break;

                            case "宇宙":
                                Sound.StartBGM(Sound.BGMName("Map5"));
                                break;

                            default:
                                if (terrain?.Name == "壁")
                                {
                                    Sound.StartBGM(Sound.BGMName("Map3"));
                                }
                                else
                                {
                                    Sound.StartBGM(Sound.BGMName("Map1"));
                                }
                                break;
                        }
                    }

                    // ターンイベント
                    Event.IsUnitCenter = false;
                    Event.HandleEvent("ターン", Turn.ToString(), "味方");
                    if (IsScenarioFinished)
                    {
                        GUI.UnlockGUI();
                        return;
                    }

                    // 操作可能なユニットがいるかどうかチェック
                    if (UList.Items.Count(x => x.Party == "味方" && x.CanAction) > 0
                        || Expression.IsOptionDefined("味方フェイズ強制発動"))
                    {
                        break;
                    }

                    // CPUが操作するユニットがいるかどうかチェック
                    if (UList.Items.Count(x => x.Party != "味方" && x.Status == "出撃") > 0)
                    {
                        break;
                    }

                    // 敵フェイズ
                    StartTurn("敵");
                    if (IsScenarioFinished)
                    {
                        IsScenarioFinished = false;
                        return;
                    }

                    // 中立フェイズ
                    StartTurn("中立");
                    if (IsScenarioFinished)
                    {
                        IsScenarioFinished = false;
                        return;
                    }

                    // ＮＰＣフェイズ
                    StartTurn("ＮＰＣ");
                    if (IsScenarioFinished)
                    {
                        IsScenarioFinished = false;
                        return;
                    }
                }
                while (true);
            }
            else
            {
                // 味方フェイズ以外

                // 状態回復
                foreach (var currentUnit in UList.Items)
                {
                    Commands.SelectedUnit = currentUnit;
                    switch (currentUnit.Status)
                    {
                        case "出撃":
                        case "格納":
                            {
                                if ((currentUnit.Party ?? "") == (uparty ?? ""))
                                {
                                    currentUnit.Rest();
                                    if (IsScenarioFinished)
                                    {
                                        GUI.UnlockGUI();
                                        return;
                                    }
                                }
                                else
                                {
                                    currentUnit.UsedAction = 0;
                                }

                                break;
                            }

                        case "旧主形態":
                        case "旧形態":
                            {
                                currentUnit.UsedAction = 0;
                                break;
                            }
                    }
                }

                // 敵ユニットが味方にかけたスペシャルパワーを解除
                foreach (var currentUnit in UList.Items)
                {
                    switch (currentUnit.Status)
                    {
                        case "出撃":
                        case "格納":
                            {
                                currentUnit.RemoveSpecialPowerInEffect("敵ターン");
                                break;
                            }
                    }
                }

                GUI.RedrawScreen();

                // 敵(ＮＰＣ)フェイズ用ＢＧＭを演奏
                var terrain = Map.Terrain(1, 1);
                if (Stage == "ＮＰＣ")
                {
                    switch (terrain?.Class ?? "")
                    {
                        case "屋内":
                            Sound.StartBGM(Sound.BGMName("Map3"));
                            break;

                        case "宇宙":
                            Sound.StartBGM(Sound.BGMName("Map5"));
                            break;

                        default:
                            if (terrain?.Name == "壁")
                            {
                                Sound.StartBGM(Sound.BGMName("Map3"));
                            }
                            else
                            {
                                Sound.StartBGM(Sound.BGMName("Map1"));
                            }
                            break;
                    }
                }
                else
                {

                    switch (terrain?.Class ?? "")
                    {
                        case "屋内":
                            Sound.StartBGM(Sound.BGMName("Map4"));
                            break;

                        case "宇宙":
                            Sound.StartBGM(Sound.BGMName("Map6"));
                            break;

                        default:
                            if (terrain?.Name == "壁")
                            {
                                Sound.StartBGM(Sound.BGMName("Map4"));
                            }
                            else
                            {
                                Sound.StartBGM(Sound.BGMName("Map2"));
                            }
                            break;
                    }
                }

                // ターンイベント
                Event.HandleEvent("ターン", Turn.ToString(), uparty);
                if (IsScenarioFinished)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            int max_lv;
            Unit max_unit = null;
            if (uparty == "味方")
            {
                // 味方フェイズのプレイヤーによるユニット操作前の処理

                // ターン数を表示
                if (Turn > 1 && Expression.IsOptionDefined("デバッグ"))
                {
                    GUI.DisplayTelop("ターン" + SrcFormatter.Format(Turn));
                }

                // 通常のステージでは母艦ユニットまたはレベルがもっとも高い
                // ユニットを中央に配置
                if (!Map.IsStatusView && !Event.IsUnitCenter)
                {
                    foreach (Unit currentUnit in UList.Items)
                    {
                        if (currentUnit.Party == "味方" && currentUnit.Status == "出撃" && currentUnit.Action > 0)
                        {
                            if (currentUnit.IsFeatureAvailable("母艦"))
                            {
                                GUI.Center(currentUnit.x, currentUnit.y);
                                GUIStatus.DisplayUnitStatus(currentUnit);
                                GUI.RedrawScreen();
                                GUI.UnlockGUI();
                                return;
                            }
                        }
                    }

                    max_lv = 0;
                    foreach (Unit cuttentUnit in UList.Items)
                    {
                        if (cuttentUnit.Party == "味方" && cuttentUnit.Status == "出撃")
                        {
                            if (cuttentUnit.MainPilot().Level > max_lv)
                            {
                                max_unit = cuttentUnit;
                                max_lv = cuttentUnit.MainPilot().Level;
                            }
                        }
                    }

                    if (max_unit != null)
                    {
                        GUI.Center(max_unit.x, max_unit.y);
                    }
                }

                // ステータスを表示
                if (!Map.IsStatusView)
                {
                    GUIStatus.DisplayGlobalStatus();
                }

                // プレイヤーによる味方ユニット操作に移行
                GUI.RedrawScreen();
                GUI.UnlockGUI();
                return;
            }

            GUI.LockGUI();

            CPUOperation(uparty);

            if (IsScenarioFinished)
            {
                return;
            }

            // ステータスウィンドウの表示を消去
            GUIStatus.ClearUnitStatus();
        }

        private void CPUOperation(string uparty)
        {
            // CPUによるユニット操作
            for (var phase = 1; phase <= 5; phase++)
            {
                foreach (var u in UList.Items)
                {
                    // フェイズ中に行動するユニットを選択
                    Commands.SelectedUnit = u;
                    if (u.Status != "出撃")
                    {
                        continue;
                    }

                    if (u.Action == 0)
                    {
                        continue;
                    }

                    if ((u.Party ?? "") != (uparty ?? ""))
                    {
                        continue;
                    }

                    var phaseJudgeUnit = u;

                    // 他のユニットを護衛しているユニットは護衛対象と同じ順に行動
                    if (PList.IsDefined(u.Mode))
                    {
                        var p = PList.Item(u.Mode);
                        if (p.Unit != null)
                        {
                            if (p.Unit.Party == uparty)
                            {
                                phaseJudgeUnit = p.Unit;
                            }
                        }
                    }
                    // XXX 二段階走査してるってこと？
                    if (PList.IsDefined(phaseJudgeUnit.Mode))
                    {
                        var p = PList.Item(phaseJudgeUnit.Mode);
                        if (p.Unit != null)
                        {
                            if (p.Unit.Party == uparty)
                            {
                                phaseJudgeUnit = p.Unit;
                            }
                        }
                    }

                    switch (phase)
                    {
                        case 1:
                            // 最初にサポート能力を持たないザコユニットが行動
                            if (phaseJudgeUnit.BossRank >= 0)
                            {
                                continue;
                            }
                            if (phaseJudgeUnit.MainPilot().HasSupportSkill())
                            {
                                continue;
                            }

                            break;

                        case 2:
                            // 次にサポート能力を持たないボスユニットが行動
                            if (phaseJudgeUnit.MainPilot().HasSupportSkill())
                            {
                                continue;
                            }

                            break;

                        case 3:
                            // 次に統率能力を持つユニットが行動
                            if (!phaseJudgeUnit.MainPilot().IsSkillAvailable("統率"))
                            {
                                continue;
                            }
                            break;

                        case 4:
                            // 次にサポート能力を持つザコユニットが行動
                            if (phaseJudgeUnit.BossRank >= 0)
                            {
                                continue;
                            }
                            break;

                        case 5:
                            // 最後にサポート能力を持つボスユニットが行動
                            break;
                    }

                    while (Commands.SelectedUnit.Action > 0)
                    {
                        // 途中で状態が変更された場合
                        if (Commands.SelectedUnit.Status != "出撃")
                        {
                            break;
                        }

                        // 途中で陣営が変更された場合
                        if ((Commands.SelectedUnit.Party ?? "") != (uparty ?? ""))
                        {
                            break;
                        }

                        if (!GUI.IsRButtonPressed())
                        {
                            GUIStatus.DisplayUnitStatus(Commands.SelectedUnit);
                            GUI.Center(Commands.SelectedUnit.x, Commands.SelectedUnit.y);
                            GUI.RedrawScreen();
                            // XXX
                            //Application.DoEvents();
                        }

                        IsCanceled = false; // Cancelコマンドのクリア

                        // ユニットを行動させる
                        COM.OperateUnit();
                        if (IsScenarioFinished)
                        {
                            return;
                        }

                        // ハイパーモード・ノーマルモードの自動発動チェック
                        UList.CheckAutoHyperMode();
                        UList.CheckAutoNormalMode();

                        // Cancelコマンドが実行されたらここで終了
                        if (IsCanceled)
                        {
                            if (Commands.SelectedUnit is null)
                            {
                                break;
                            }

                            if (Commands.SelectedUnit.Status != "出撃")
                            {
                                break;
                            }

                            IsCanceled = false;
                        }

                        // 行動数を減少
                        Commands.SelectedUnit.UseAction();

                        // 接触イベント
                        {
                            var withBlock13 = Commands.SelectedUnit;
                            if (withBlock13.Status == "出撃" && withBlock13.x > 1)
                            {
                                if (Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y];
                                    Event.HandleEvent("接触", withBlock13.MainPilot().ID, Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y].MainPilot().ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock14 = Commands.SelectedUnit;
                            if (withBlock14.Status == "出撃" && withBlock14.x < Map.MapWidth)
                            {
                                if (Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
                                    Event.HandleEvent("接触", withBlock14.MainPilot().ID, Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y].MainPilot().ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock15 = Commands.SelectedUnit;
                            if (withBlock15.Status == "出撃" && withBlock15.y > 1)
                            {
                                if (Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1];
                                    Event.HandleEvent("接触", withBlock15.MainPilot().ID, Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1].MainPilot().ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock16 = Commands.SelectedUnit;
                            if (withBlock16.Status == "出撃" && withBlock16.y < Map.MapHeight)
                            {
                                if (Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1];
                                    Event.HandleEvent("接触", withBlock16.MainPilot().ID, Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1].MainPilot().ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        // 進入イベント
                        {
                            var withBlock17 = Commands.SelectedUnit;
                            if (withBlock17.Status == "出撃")
                            {
                                Event.HandleEvent("進入", withBlock17.MainPilot().ID, "" + withBlock17.x, "" + withBlock17.y);
                                if (IsScenarioFinished)
                                {
                                    return;
                                }
                            }
                        }

                        // 行動終了イベント
                        {
                            var withBlock18 = Commands.SelectedUnit;
                            if (withBlock18.Status == "出撃")
                            {
                                Event.HandleEvent("行動終了", withBlock18.MainPilot().ID);
                                if (IsScenarioFinished)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        // ゲームオーバー
        public void GameOver()
        {
            var fname = default(string);
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StopBGM();
            GUI.MainFormHide();

            // GameOver.eveを探す
            var gameOverFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "GameOver.eve"),
                FileSystem.PathCombine(AppPath, "Data", "System", "GameOver.eve"),
            };
            var gameOverFile = gameOverFiles.FirstOrDefault(x => FileSystem.FileExists(x));
            if (string.IsNullOrEmpty(gameOverFile))
            {
                // GameOver.eveが無ければそのまま終了
                TerminateSRC();
            }

            var nonPilotFile = FileSystem.PathCombine(Path.GetDirectoryName(gameOverFile), "non_pilot.txt");
            if (FileSystem.FileExists(nonPilotFile))
            {
                NPDList.Load(nonPilotFile);
            }

            // GameOver.eveを読み込み
            Event.ClearEventData();
            Event.LoadEventData(gameOverFile, load_mode: "");
            ScenarioFileName = fname;
            if (!Event.IsEventDefined("プロローグ"))
            {
                GUI.ErrorMessage(fname + "中にプロローグイベントが定義されていません");
                TerminateSRC();
            }

            // GameOver.eveのプロローグイベントを実施
            Event.HandleEvent("プロローグ");
        }

        // ゲームクリア
        public void GameClear()
        {
            TerminateSRC();
        }

        // ゲームを途中終了
        public void ExitGame()
        {
            var fname = default(string);
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StopBGM();

            // Exit.eveを探す
            GUI.MainFormHide();
            var exitFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "Exit.eve"),
                FileSystem.PathCombine(AppPath, "Data", "System", "Exit.eve"),
            };
            var exitFile = exitFiles.FirstOrDefault(x => FileSystem.FileExists(x));
            if (string.IsNullOrEmpty(exitFile))
            {
                // Exit.eveが無ければそのまま終了
                TerminateSRC();
            }

            var nonPilotFile = FileSystem.PathCombine(Path.GetDirectoryName(exitFile), "non_pilot.txt");
            if (FileSystem.FileExists(nonPilotFile))
            {
                NPDList.Load(nonPilotFile);
            }

            // Exit.eveを読み込み
            Event.ClearEventData();
            Event.LoadEventData(exitFile, load_mode: "");
            ScenarioFileName = fname;
            if (!Event.IsEventDefined("プロローグ"))
            {
                GUI.ErrorMessage(fname + "中にプロローグイベントが定義されていません");
                TerminateSRC();
            }

            // Exit.eveのプロローグイベントを実施
            Event.HandleEvent("プロローグ");

            // SRCを終了
            TerminateSRC();
        }

        // SRCを終了
        public void TerminateSRC()
        {
            //// ウィンドウを閉じる
            //if (GUI.MainForm is object)
            //{
            //    GUI.MainForm.Hide();
            //}
            //Load(My.MyProject.Forms.frmMessage);
            //if (My.MyProject.Forms.frmMessage.Visible)
            //{
            //    GUI.CloseMessageForm();
            //}
            //Load(My.MyProject.Forms.frmListBox);
            //if (My.MyProject.Forms.frmListBox.Visible)
            //{
            //    My.MyProject.Forms.frmListBox.Hide();
            //}
            //Load(My.MyProject.Forms.frmNowLoading);
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            //    My.MyProject.Forms.frmNowLoading.Hide();
            //}

            //Application.DoEvents();

            //// 時間解像度を元に戻す
            //GeneralLib.timeEndPeriod(1);

            //// フルスクリーンモードを使っていた場合は解像度を元に戻す
            //if (GeneralLib.ReadIni("Option", "FullScreen") == "On")
            //{
            //    GUI.ChangeDisplaySize(0, 0);
            //}

            //// ＢＧＭ・効果音の再生を停止
            //Sound.FreeSoundModule();

            //// 各種データを解放

            //// なぜかこれがないと不正終了する……
            //Application.DoEvents();

            if (GUI?.Terminate() ?? true)
            {
                Environment.Exit(0);
            }
        }
    }
}
