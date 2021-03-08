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
                    Path.Combine(ScenarioPath, fname),
                    Path.Combine(AppPath, fname),
                }.FirstOrDefault(x => GeneralLib.FileExists(x)) ?? fname;
            }

            if (string.IsNullOrEmpty(Lib.FileSystem.Dir(fname, FileAttribute.Normal)))
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
            //        string argmsg = "シナリオ側のDataフォルダ名がDateになっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Date" + Constants.vbCr + Constants.vbLf + "フォルダ名をDataに直してください。";
            //        GUI.ErrorMessage(argmsg);
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｄａｔａ", FileAttribute.Directory)) > 0)
            //    {
            //        string argmsg1 = "シナリオ側のDataフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｄａｔａ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
            //        GUI.ErrorMessage(argmsg1);
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｂｉｔｍａｐ", FileAttribute.Directory)) > 0)
            //    {
            //        string argmsg2 = "シナリオ側のBitmapフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｂｉｔｍａｐ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
            //        GUI.ErrorMessage(argmsg2);
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｌｉｂ", FileAttribute.Directory)) > 0)
            //    {
            //        string argmsg3 = "シナリオ側のLibフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｌｉｂ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
            //        GUI.ErrorMessage(argmsg3);
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｍｉｄｉ", FileAttribute.Directory)) > 0)
            //    {
            //        string argmsg4 = "シナリオ側のMidiフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｍｉｄｉ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
            //        GUI.ErrorMessage(argmsg4);
            //        TerminateSRC();
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｓｏｕｎｄ", FileAttribute.Directory)) > 0)
            //    {
            //        string argmsg5 = "シナリオ側のSoundフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｓｏｕｎｄ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
            //        GUI.ErrorMessage(argmsg5);
            //        TerminateSRC();
            //    }

            //    // 読み込むイベントファイル名に合わせて各種システム変数を設定
            //    string argvname1 = "次ステージ";
            //    if (!Expression.IsGlobalVariableDefined(argvname1))
            //    {
            //        string argvname = "次ステージ";
            //        Expression.DefineGlobalVariable(argvname);
            //    }

            //    string argvname2 = "次ステージ";
            //    string argnew_value = "";
            //    Expression.SetVariableAsString(argvname2, argnew_value);
            //    var loopTo = Strings.Len(fname);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
            //        {
            //            break;
            //        }
            //    }

            //    string argvname3 = "ステージ";
            //    string argnew_value1 = Strings.Mid(fname, Strings.Len(fname) - i + 2);
            //    Expression.SetVariableAsString(argvname3, argnew_value1);
            //    string argvname5 = "セーブデータファイル名";
            //    if (!Expression.IsGlobalVariableDefined(argvname5))
            //    {
            //        string argvname4 = "セーブデータファイル名";
            //        Expression.DefineGlobalVariable(argvname4);
            //    }

            //    string argvname6 = "セーブデータファイル名";
            //    string argnew_value2 = Strings.Mid(fname, Strings.Len(fname) - i + 2, i - 5) + "までクリア.src";
            //    Expression.SetVariableAsString(argvname6, argnew_value2);

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
            //Commands.SelectedPartners = new Unit[1];

            //// フォント設定をデフォルトに戻す
            //// UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //{
            //    var withBlock1 = GUI.MainForm.picMain(0);
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    withBlock1.ForeColor = Information.RGB(255, 255, 255);
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    if (withBlock1.Font.Name != "ＭＳ Ｐ明朝")
            //    {
            //        sf = (Font)Control.DefaultFont.Clone();
            //        sf = SrcFormatter.FontChangeName(sf, "ＭＳ Ｐ明朝");
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock1.Font = sf;
            //    }
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    withBlock1.Font.Size = 16;
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    withBlock1.Font.Bold = true;
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
            if (!IsSubStage & Event.IsEventDefined("プロローグ", true))
            {
                //Sound.StopBGM();
                //string argbgm_name = "Briefing";
                //string argbgm_name1 = Sound.BGMName(argbgm_name);
                //Sound.StartBGM(argbgm_name1);
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
            //Sound.StopBGM();

            //// リスタート用にデータをセーブ
            //if (Strings.InStr(fname, @"\Lib\ユニットステータス表示.eve") == 0 & Strings.InStr(fname, @"\Lib\パイロットステータス表示.eve") == 0)
            //{
            //    string argfname = ScenarioPath + "_リスタート.src";
            //    DumpData(argfname);
            //}

            //// スタートイベントが始まった場合は通常のステージとみなす
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
            // TODO ImplS
            //int num, i, phase;
            //Unit u;
            Stage = uparty;
            //Sound.BossBGM = false;
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
                                    string argstype = "敵ターン";
                                    currentU.RemoveSpecialPowerInEffect(argstype);
                                    break;
                                }
                        }
                    }

                    GUI.RedrawScreen();

                    //// 味方フェイズ用ＢＧＭを演奏
                    //if (!string.IsNullOrEmpty(Map.MapFileName))
                    //{
                    //    switch (Map.TerrainClass(1, 1) ?? "")
                    //    {
                    //        case "屋内":
                    //            {
                    //                string argbgm_name = "Map3";
                    //                string argbgm_name1 = Sound.BGMName(argbgm_name);
                    //                Sound.StartBGM(argbgm_name1);
                    //                break;
                    //            }

                    //        case "宇宙":
                    //            {
                    //                string argbgm_name2 = "Map5";
                    //                string argbgm_name3 = Sound.BGMName(argbgm_name2);
                    //                Sound.StartBGM(argbgm_name3);
                    //                break;
                    //            }

                    //        default:
                    //            {
                    //                if (Map.TerrainName(1, 1) == "壁")
                    //                {
                    //                    string argbgm_name4 = "Map3";
                    //                    string argbgm_name5 = Sound.BGMName(argbgm_name4);
                    //                    Sound.StartBGM(argbgm_name5);
                    //                }
                    //                else
                    //                {
                    //                    string argbgm_name6 = "Map1";
                    //                    string argbgm_name7 = Sound.BGMName(argbgm_name6);
                    //                    Sound.StartBGM(argbgm_name7);
                    //                }

                    //                break;
                    //            }
                    //    }
                    //}

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
                                string argstype1 = "敵ターン";
                                currentUnit.RemoveSpecialPowerInEffect(argstype1);
                                break;
                            }
                    }
                }

                GUI.RedrawScreen();

                //// 敵(ＮＰＣ)フェイズ用ＢＧＭを演奏
                //switch (Map.TerrainClass(1, 1) ?? "")
                //{
                //    case "屋内":
                //        {
                //            if (Stage == "ＮＰＣ")
                //            {
                //                string argbgm_name8 = "Map3";
                //                string argbgm_name9 = Sound.BGMName(argbgm_name8);
                //                Sound.StartBGM(argbgm_name9);
                //            }
                //            else
                //            {
                //                string argbgm_name10 = "Map4";
                //                string argbgm_name11 = Sound.BGMName(argbgm_name10);
                //                Sound.StartBGM(argbgm_name11);
                //            }

                //            break;
                //        }

                //    case "宇宙":
                //        {
                //            if (Stage == "ＮＰＣ")
                //            {
                //                string argbgm_name12 = "Map5";
                //                string argbgm_name13 = Sound.BGMName(argbgm_name12);
                //                Sound.StartBGM(argbgm_name13);
                //            }
                //            else
                //            {
                //                string argbgm_name14 = "Map6";
                //                string argbgm_name15 = Sound.BGMName(argbgm_name14);
                //                Sound.StartBGM(argbgm_name15);
                //            }

                //            break;
                //        }

                //    default:
                //        {
                //            if (Stage == "ＮＰＣ")
                //            {
                //                if (Map.TerrainName(1, 1) == "壁")
                //                {
                //                    string argbgm_name16 = "Map3";
                //                    string argbgm_name17 = Sound.BGMName(argbgm_name16);
                //                    Sound.StartBGM(argbgm_name17);
                //                }
                //                else
                //                {
                //                    string argbgm_name18 = "Map1";
                //                    string argbgm_name19 = Sound.BGMName(argbgm_name18);
                //                    Sound.StartBGM(argbgm_name19);
                //                }
                //            }
                //            else if (Map.TerrainName(1, 1) == "壁")
                //            {
                //                string argbgm_name20 = "Map4";
                //                string argbgm_name21 = Sound.BGMName(argbgm_name20);
                //                Sound.StartBGM(argbgm_name21);
                //            }
                //            else
                //            {
                //                string argbgm_name22 = "Map2";
                //                string argbgm_name23 = Sound.BGMName(argbgm_name22);
                //                Sound.StartBGM(argbgm_name23);
                //            }

                //            break;
                //        }
                //}

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
                if (Turn > 1 & Expression.IsOptionDefined("デバッグ"))
                {
                    string argmsg = "ターン" + SrcFormatter.Format(Turn);
                    GUI.DisplayTelop(argmsg);
                }

                // 通常のステージでは母艦ユニットまたはレベルがもっとも高い
                // ユニットを中央に配置
                if (!Map.IsStatusView && !Event.IsUnitCenter)
                {
                    foreach (Unit currentUnit in UList.Items)
                    {
                        if (currentUnit.Party == "味方" & currentUnit.Status == "出撃" & currentUnit.Action > 0)
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
                        if (cuttentUnit.Party == "味方" & cuttentUnit.Status == "出撃")
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

            //GUI.LockGUI();

            //// CPUによるユニット操作
            //for (phase = 1; phase <= 5; phase++)
            //{
            //    var loopTo = UList.Count();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        // フェイズ中に行動するユニットを選択
            //        object argIndex1 = i;
            //        Commands.SelectedUnit = UList.Item(argIndex1);
            //        {
            //            var withBlock8 = Commands.SelectedUnit;
            //            if (withBlock8.Status != "出撃")
            //            {
            //                goto NextLoop;
            //            }

            //            if (withBlock8.Action == 0)
            //            {
            //                goto NextLoop;
            //            }

            //            if ((withBlock8.Party ?? "") != (uparty ?? ""))
            //            {
            //                goto NextLoop;
            //            }

            //            u = Commands.SelectedUnit;

            //            // 他のユニットを護衛しているユニットは護衛対象と同じ順に行動
            //            bool localIsDefined() { object argIndex1 = withBlock8.Mode; var ret = PList.IsDefined(argIndex1); withBlock8.Mode = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined())
            //            {
            //                object argIndex2 = withBlock8.Mode;
            //                {
            //                    var withBlock9 = PList.Item(argIndex2);
            //                    if (withBlock9.Unit is object)
            //                    {
            //                        if (withBlock9.Unit.Party == uparty)
            //                        {
            //                            u = withBlock9.Unit;
            //                        }
            //                    }
            //                }

            //                withBlock8.Mode = Conversions.ToString(argIndex2);
            //            }

            //            bool localIsDefined1() { object argIndex1 = u.Mode; var ret = PList.IsDefined(argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined1())
            //            {
            //                object argIndex3 = u.Mode;
            //                {
            //                    var withBlock10 = PList.Item(argIndex3);
            //                    if (withBlock10.Unit is object)
            //                    {
            //                        if (withBlock10.Unit.Party == uparty)
            //                        {
            //                            u = withBlock10.Unit;
            //                        }
            //                    }
            //                }

            //                u.Mode = Conversions.ToString(argIndex3);
            //            }

            //            switch (phase)
            //            {
            //                case 1:
            //                    {
            //                        // 最初にサポート能力を持たないザコユニットが行動
            //                        if (u.BossRank >= 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        {
            //                            var withBlock11 = u.MainPilot();
            //                            string argsname = "援護";
            //                            string argsname1 = "援護攻撃";
            //                            string argsname2 = "援護防御";
            //                            string argsname3 = "統率";
            //                            string argsname4 = "指揮";
            //                            string argsname5 = "広域サポート";
            //                            if (withBlock11.IsSkillAvailable(argsname) | withBlock11.IsSkillAvailable(argsname1) | withBlock11.IsSkillAvailable(argsname2) | withBlock11.IsSkillAvailable(argsname3) | withBlock11.IsSkillAvailable(argsname4) | withBlock11.IsSkillAvailable(argsname5))
            //                            {
            //                                goto NextLoop;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        // 次にサポート能力を持たないボスユニットが行動
            //                        {
            //                            var withBlock12 = u.MainPilot();
            //                            string argsname6 = "援護";
            //                            string argsname7 = "援護攻撃";
            //                            string argsname8 = "援護防御";
            //                            string argsname9 = "統率";
            //                            string argsname10 = "指揮";
            //                            string argsname11 = "広域サポート";
            //                            if (withBlock12.IsSkillAvailable(argsname6) | withBlock12.IsSkillAvailable(argsname7) | withBlock12.IsSkillAvailable(argsname8) | withBlock12.IsSkillAvailable(argsname9) | withBlock12.IsSkillAvailable(argsname10) | withBlock12.IsSkillAvailable(argsname11))
            //                            {
            //                                goto NextLoop;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        // 次に統率能力を持つユニットが行動
            //                        string argsname12 = "統率";
            //                        if (!u.MainPilot().IsSkillAvailable(argsname12))
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        // 次にサポート能力を持つザコユニットが行動
            //                        if (u.BossRank >= 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }

            //                case 5:
            //                    {
            //                        break;
            //                    }
            //                    // 最後にサポート能力を持つボスユニットが行動
            //            }
            //        }

            //        while (Commands.SelectedUnit.Action > 0)
            //        {
            //            // 途中で状態が変更された場合
            //            if (Commands.SelectedUnit.Status != "出撃")
            //            {
            //                break;
            //            }

            //            // 途中で陣営が変更された場合
            //            if ((Commands.SelectedUnit.Party ?? "") != (uparty ?? ""))
            //            {
            //                break;
            //            }

            //            if (!GUI.IsRButtonPressed())
            //            {
            //                Status.DisplayUnitStatus(Commands.SelectedUnit);
            //                GUI.Center(Commands.SelectedUnit.x, Commands.SelectedUnit.y);
            //                GUI.RedrawScreen();
            //                Application.DoEvents();
            //            }

            //            IsCanceled = false; // Cancelコマンドのクリア

            //            // ユニットを行動させる
            //            COM.OperateUnit();
            //            if (IsScenarioFinished)
            //            {
            //                return;
            //            }

            //            // ハイパーモード・ノーマルモードの自動発動チェック
            //            UList.CheckAutoHyperMode();
            //            UList.CheckAutoNormalMode();

            //            // Cancelコマンドが実行されたらここで終了
            //            if (IsCanceled)
            //            {
            //                if (Commands.SelectedUnit is null)
            //                {
            //                    break;
            //                }

            //                if (Commands.SelectedUnit.Status != "出撃")
            //                {
            //                    break;
            //                }

            //                IsCanceled = false;
            //            }

            //            // 行動数を減少
            //            Commands.SelectedUnit.UseAction();

            //            // 接触イベント
            //            {
            //                var withBlock13 = Commands.SelectedUnit;
            //                if (withBlock13.Status == "出撃" & withBlock13.x > 1)
            //                {
            //                    if (Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y] is object)
            //                    {
            //                        Commands.SelectedTarget = Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y];
            //                        Event.HandleEvent("接触", withBlock13.MainPilot().ID, Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y].MainPilot.ID);
            //                        if (IsScenarioFinished)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }

            //            {
            //                var withBlock14 = Commands.SelectedUnit;
            //                if (withBlock14.Status == "出撃" & withBlock14.x < Map.MapWidth)
            //                {
            //                    if (Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y] is object)
            //                    {
            //                        Commands.SelectedTarget = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
            //                        Event.HandleEvent("接触", withBlock14.MainPilot().ID, Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y].MainPilot.ID);
            //                        if (IsScenarioFinished)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }

            //            {
            //                var withBlock15 = Commands.SelectedUnit;
            //                if (withBlock15.Status == "出撃" & withBlock15.y > 1)
            //                {
            //                    if (Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1] is object)
            //                    {
            //                        Commands.SelectedTarget = Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1];
            //                        Event.HandleEvent("接触", withBlock15.MainPilot().ID, Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1].MainPilot.ID);
            //                        if (IsScenarioFinished)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }

            //            {
            //                var withBlock16 = Commands.SelectedUnit;
            //                if (withBlock16.Status == "出撃" & withBlock16.y < Map.MapHeight)
            //                {
            //                    if (Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1] is object)
            //                    {
            //                        Commands.SelectedTarget = Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1];
            //                        Event.HandleEvent("接触", withBlock16.MainPilot().ID, Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1].MainPilot.ID);
            //                        if (IsScenarioFinished)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }

            //            // 進入イベント
            //            {
            //                var withBlock17 = Commands.SelectedUnit;
            //                if (withBlock17.Status == "出撃")
            //                {
            //                    Event.HandleEvent("進入", withBlock17.MainPilot().ID, withBlock17.x, withBlock17.y);
            //                    if (IsScenarioFinished)
            //                    {
            //                        return;
            //                    }
            //                }
            //            }

            //            // 行動終了イベント
            //            {
            //                var withBlock18 = Commands.SelectedUnit;
            //                if (withBlock18.Status == "出撃")
            //                {
            //                    Event.HandleEvent("行動終了", withBlock18.MainPilot().ID);
            //                    if (IsScenarioFinished)
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}

            // ステータスウィンドウの表示を消去
            GUIStatus.ClearUnitStatus();
        }

        // ゲームオーバー
        public void GameOver()
        {
            throw new NotImplementedException();
            //var fname = default(string);
            //Sound.KeepBGM = false;
            //Sound.BossBGM = false;
            //Sound.StopBGM();
            //GUI.MainForm.Hide();

            //// GameOver.eveを探す
            //bool localFileExists() { string argfname = AppPath + @"Data\System\GameOver.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //string argfname4 = ScenarioPath + @"Data\System\GameOver.eve";
            //if (GeneralLib.FileExists(argfname4))
            //{
            //    fname = ScenarioPath + @"Data\System\GameOver.eve";
            //    string argfname1 = ScenarioPath + @"Data\System\non_pilot.txt";
            //    if (GeneralLib.FileExists(argfname1))
            //    {
            //        string argfname = ScenarioPath + @"Data\System\non_pilot.txt";
            //        NPDList.Load(argfname);
            //    }
            //}
            //else if (localFileExists())
            //{
            //    fname = AppPath + @"Data\System\GameOver.eve";
            //    string argfname3 = AppPath + @"Data\System\non_pilot.txt";
            //    if (GeneralLib.FileExists(argfname3))
            //    {
            //        string argfname2 = AppPath + @"Data\System\non_pilot.txt";
            //        NPDList.Load(argfname2);
            //    }
            //}
            //else
            //{
            //    // GameOver.eveが無ければそのまま終了
            //    TerminateSRC();
            //}

            //// GameOver.eveを読み込み
            //Event.ClearEventData();
            //string argload_mode = "";
            //Event.LoadEventData(fname, load_mode: argload_mode);
            //ScenarioFileName = fname;
            //string arglname = "プロローグ";
            //if (!Event.IsEventDefined(arglname))
            //{
            //    string argmsg = fname + "中にプロローグイベントが定義されていません";
            //    GUI.ErrorMessage(argmsg);
            //    TerminateSRC();
            //}

            //// GameOver.eveのプロローグイベントを実施
            //Event.HandleEvent("プロローグ");
        }

        // ゲームクリア
        public void GameClear()
        {
            TerminateSRC();
        }

        // ゲームを途中終了
        public void ExitGame()
        {
            throw new NotImplementedException();
            //var fname = default(string);
            //Sound.KeepBGM = false;
            //Sound.BossBGM = false;
            //Sound.StopBGM();

            //// Exit.eveを探す
            //GUI.MainForm.Hide();
            //bool localFileExists() { string argfname = AppPath + @"Data\System\Exit.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //string argfname4 = ScenarioPath + @"Data\System\Exit.eve";
            //if (GeneralLib.FileExists(argfname4))
            //{
            //    fname = ScenarioPath + @"Data\System\Exit.eve";
            //    string argfname1 = ScenarioPath + @"Data\System\non_pilot.txt";
            //    if (GeneralLib.FileExists(argfname1))
            //    {
            //        string argfname = ScenarioPath + @"Data\System\non_pilot.txt";
            //        NPDList.Load(argfname);
            //    }
            //}
            //else if (localFileExists())
            //{
            //    fname = AppPath + @"Data\System\Exit.eve";
            //    string argfname3 = AppPath + @"Data\System\non_pilot.txt";
            //    if (GeneralLib.FileExists(argfname3))
            //    {
            //        string argfname2 = AppPath + @"Data\System\non_pilot.txt";
            //        NPDList.Load(argfname2);
            //    }
            //}
            //else
            //{
            //    // Exit.eveが無ければそのまま終了
            //    TerminateSRC();
            //}

            //// Exit.eveを読み込み
            //Event.ClearEventData();
            //string argload_mode = "";
            //Event.LoadEventData(fname, load_mode: argload_mode);
            //string arglname = "プロローグ";
            //if (!Event.IsEventDefined(arglname))
            //{
            //    string argmsg = fname + "中にプロローグイベントが定義されていません";
            //    GUI.ErrorMessage(argmsg);
            //    TerminateSRC();
            //}

            //// Exit.eveのプロローグイベントを実施
            //Event.HandleEvent("プロローグ");

            //// SRCを終了
            //TerminateSRC();
        }

        // SRCを終了
        public void TerminateSRC()
        {
            //// ウィンドウを閉じる
            //if (GUI.MainForm is object)
            //{
            //    GUI.MainForm.Hide();
            //}
            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmMessage);
            //if (My.MyProject.Forms.frmMessage.Visible)
            //{
            //    GUI.CloseMessageForm();
            //}
            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmListBox);
            //if (My.MyProject.Forms.frmListBox.Visible)
            //{
            //    My.MyProject.Forms.frmListBox.Hide();
            //}
            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmNowLoading);
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            //    My.MyProject.Forms.frmNowLoading.Hide();
            //}

            //Application.DoEvents();

            //// 時間解像度を元に戻す
            //GeneralLib.timeEndPeriod(1);

            //// フルスクリーンモードを使っていた場合は解像度を元に戻す
            //string argini_section = "Option";
            //string argini_entry = "FullScreen";
            //if (GeneralLib.ReadIni(argini_section, argini_entry) == "On")
            //{
            //    GUI.ChangeDisplaySize(0, 0);
            //}

            //// ＢＧＭ・効果音の再生を停止
            //Sound.FreeSoundModule();

            //// 各種データを解放

            //// なぜかこれがないと不正終了する……
            //Application.DoEvents();

            Environment.Exit(0);
        }
    }
}
