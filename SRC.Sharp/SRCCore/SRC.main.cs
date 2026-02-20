using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.VB;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SRCCore
{
    public partial class SRC
    {
        public void Execute(string fname)
        {
            //            string fname;
            //            int i;
            //            string buf;

            //            // ２重起動禁止
            //            if (App.PrevInstance)
            //            {
            //                Environment.Exit(0);
            //            }


            // SRCが正しくインストールされているかをチェック
            ValidateEnvironment();

            // メインウィンドウのロードとFlashの登録を実施
            GUI.LoadMainFormAndRegisterFlash();

            //            // Src.iniが無ければ作成
            //            bool localFileExists() { string argfname = AppPath + "Src.ini"; var ret = FileSystem.FileExists(argfname); return ret; }

            //            if (!localFileExists())
            //            {
            //                CreateIniFile();
            //            }

            //            // 乱数の初期化
            //            VBMath.Randomize();

            //            // 時間解像度を変更する
            //            GeneralLib.timeBeginPeriod(1);

            //            // フルスクリーンモードを使う？
            //            if (Strings.LCase(GeneralLib.ReadIni("Option", "FullScreen")) == "on")
            //            {
            //                GUI.ChangeDisplaySize(800, 600);
            //            }

            //            // マウスカーソルを砂時計に
            //            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // タイトル画面を表示
            GUI.OpenTitleForm();

            ConfigureSound();

            ConfigureExecuteFile(fname);

            // Src.iniから各種パラメータの読み込み
            LoadIni();

            // マップを初期化
            Map.InitMap();

            //// 乱数系列を初期化
            GeneralLib.RndSeed = new Random().Next(1000000);
            GeneralLib.RndReset();

            ExecuteFile(fname);
        }

        private void ValidateEnvironment()
        {
            // Bitmap関係のチェック
            if (!FileSystem.GetFileSystemEntries(AppPath, "Bitmap", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmapフォルダがありません。" + Environment.NewLine + "SRC.exeと同じフォルダに汎用グラフィック集をインストールしてください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Bitmap", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmapフォルダ内にさらにBitmapフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Bitmap", "Bitmap") + Environment.NewLine + "フォルダ構造を直してください。");
                TerminateSRC();
                return;
            }

            // イベントグラフィック
            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Event", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmap\\Eventフォルダが見つかりません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                TerminateSRC();
                return;
            }

            // マップグラフィック
            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Map", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmap\\Mapフォルダがありません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                TerminateSRC();
                return;
            }

            if (!FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain", "plain0000.bmp")
                && !FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain0000.bmp")
                && !FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain0.bmp"))
            {
                if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap", "Map"), "Map", EntryOption.Directory).Any())
                {
                    GUI.ErrorMessage("Bitmap\\Mapフォルダ内にさらにMapフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Bitmap", "Map", "Map") + Environment.NewLine + "フォルダ構造を直してください。");
                    TerminateSRC();
                    return;
                }

                if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap", "Map"), "*", EntryOption.File).Any())
                {
                    GUI.ErrorMessage("Bitmap\\Mapフォルダ内にファイルがありません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                    TerminateSRC();
                    return;
                }

                GUI.ErrorMessage("Bitmap\\Mapフォルダ内にplain0000.bmpがありません。" + Environment.NewLine + "一部のマップ画像ファイルしかインストールされていない恐れがあります。" + Environment.NewLine + "新規インストールのファイルを使って汎用グラフィック集をインストールしてください。");
                TerminateSRC();
                return;
            }

            // 効果音
            if (!FileSystem.GetFileSystemEntries(AppPath, "Sound", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Soundフォルダがありません。" + Environment.NewLine + "SRC.exeと同じフォルダに効果音集をインストールしてください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Sound"), "Sound", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Soundフォルダ内にさらにSoundフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Sound", "Sound") + Environment.NewLine + "フォルダ構造を直してください。");
                TerminateSRC();
                return;
            }

            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Sound"), "*", EntryOption.File).Any())
            {
                GUI.ErrorMessage("Soundフォルダ内にファイルがありません。" + Environment.NewLine + "Soundフォルダ内に効果音集をインストールしてください。");
                TerminateSRC();
                return;
            }
        }

        private void ConfigureSound()
        {
            // TODO Impl この辺は実行環境に依存しそう

            //            // WAVE再生の手段は？
            //            switch (Strings.LCase(GeneralLib.ReadIni("Option", "UseDirectSound")) ?? "")
            //            {
            //                case "on":
            //                    {
            //                        // DirectSoundの初期化を試みる
            //                        Sound.InitDirectSound();
            //                        break;
            //                    }

            //                case "off":
            //                    {
            //                        Sound.UseDirectSound = false;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        // DirectSoundの初期化を試みる
            //                        Sound.InitDirectSound();
            //                        break;
            //                    }
            //                    // DirectSoundが使用可能かどうかで設定を切り替え
            //                    // If UseDirectSound Then
            //                    // WriteIni "Option", "UseDirectSound", "On"
            //                    // Else
            //                    // WriteIni "Option", "UseDirectSound", "Off"
            //                    // End If
            //            }

            //            // MIDI演奏の手段は？
            //            switch (Strings.LCase(GeneralLib.ReadIni("Option", "UseDirectMusic")) ?? "")
            //            {
            //                case "on":
            //                    {
            //                        // DirectMusicの初期化を試みる
            //                        Sound.InitDirectMusic();
            //                        break;
            //                    }

            //                case "off":
            //                    {
            //                        Sound.UseMCI = true;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (GeneralLib.GetWinVersion() >= 500)
            //                        {
            //                            // NT系のOSではデフォルトでDirectMusicを使う
            //                            // DirectMusicの初期化を試みる
            //                            Sound.InitDirectMusic();
            //                            // DirectMusicが使用可能かどうかで設定を切り替え
            //                            if (Sound.UseDirectMusic)
            //                            {
            //                                SystemConfig.SetItem("Option", "UseDirectMusic", "On");
            //                            }
            //                            else
            //                            {
            //                                SystemConfig.SetItem("Option", "UseDirectMusic", "Off");
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // NT系OSでなければMCIを使う
            //                            Sound.UseMCI = true;
            //                            SystemConfig.SetItem("Option", "UseDirectMusic", "Off");
            //                        }

            //                        break;
            //                    }
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("Option", "MIDIPortID")))
            //            {
            //                SystemConfig.SetItem("Option", "MIDIPortID", "0");
            //            }

            //            // MP3の再生音量
            //            buf = GeneralLib.ReadIni("Option", "MP3Volume");
            //            if (string.IsNullOrEmpty(buf))
            //            {
            //                SystemConfig.SetItem("Option", "MP3Volume", "50");
            //                Sound.MP3Volume = 50;
            //            }
            //            else
            //            {
            //                Sound.MP3Volume = GeneralLib.StrToLng(buf);
            //                if (Sound.MP3Volume < 0)
            //                {
            //                    SystemConfig.SetItem("Option", "MP3Volume", "0");
            //                    Sound.MP3Volume = 0;
            //                }
            //                else if (Sound.MP3Volume > 100)
            //                {
            //                    SystemConfig.SetItem("Option", "MP3Volume", "100");
            //                    Sound.MP3Volume = 100;
            //                }
            //            }

            //            // MP3の出力フレーム数
            //            buf = GeneralLib.ReadIni("Option", "MP3OutputBlock");
            //            if (string.IsNullOrEmpty(buf))
            //            {
            //                SystemConfig.SetItem("Option", "MP3OutputBlock", "20");
            //            }

            //            // MP3の入力直後のスリープ時間
            //            buf = GeneralLib.ReadIni("Option", "MP3InputSleep");
            //            if (string.IsNullOrEmpty(buf))
            //            {
            //                SystemConfig.SetItem("Option", "MP3InputSleep", "5");
            //            }

            //            // ＢＧＭ用MIDIファイル設定
            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Opening")))
            //            {
            //                SystemConfig.SetItem("BGM", "Opening", "Opening.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map1")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map1", "Map1.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map2")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map2", "Map2.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map3")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map3", "Map3.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map4")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map4", "Map4.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map5")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map5", "Map5.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Map6")))
            //            {
            //                SystemConfig.SetItem("BGM", "Map6", "Map6.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Briefing")))
            //            {
            //                SystemConfig.SetItem("BGM", "Briefing", "Briefing.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Intermission")))
            //            {
            //                SystemConfig.SetItem("BGM", "Intermission", "Intermission.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "Subtitle")))
            //            {
            //                SystemConfig.SetItem("BGM", "Subtitle", "Subtitle.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "End")))
            //            {
            //                SystemConfig.SetItem("BGM", "End", "End.mid");
            //            }

            //            if (string.IsNullOrEmpty(GeneralLib.ReadIni("BGM", "default")))
            //            {
            //                SystemConfig.SetItem("BGM", "default", "default.mid");
            //            }
        }

        private void LoadIni()
        {
            //            // スペシャルパワーアニメ
            //            buf = GeneralLib.ReadIni("Option", "SpecialPowerAnimation");
            //            if (string.IsNullOrEmpty(buf))
            //            {
            //                buf = GeneralLib.ReadIni("Option", "MindEffect");
            //                if (!string.IsNullOrEmpty(buf))
            //                {
            //                    SystemConfig.SetItem("Option", "SpecialPowerAnimation", buf);
            //                }
            //            }

            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    SpecialPowerAnimation = true;
            //                }
            //                else
            //                {
            //                    SpecialPowerAnimation = false;
            //                }
            //            }
            //            else if (SpecialPowerAnimation)
            //            {
            //                SystemConfig.SetItem("Option", "SpecialPowerAnimation", "On");
            //            }
            //            else
            //            {
            //                SystemConfig.SetItem("Option", "SpecialPowerAnimation", "Off");
            //            }

            //            // 戦闘アニメ
            //            buf = Strings.LCase(GeneralLib.ReadIni("Option", "BattleAnimation"));
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (buf == "on")
            //                {
            //                    BattleAnimation = true;
            //                }
            //                else
            //                {
            //                    BattleAnimation = false;
            //                }
            //            }
            //            else if (BattleAnimation)
            //            {
            //                SystemConfig.SetItem("Option", "BattleAnimation", "On");
            //            }
            //            else
            //            {
            //                SystemConfig.SetItem("Option", "BattleAnimation", "Off");
            //            }

            //            // 拡大戦闘アニメ
            //            buf = Strings.LCase(GeneralLib.ReadIni("Option", "ExtendedAnimation"));
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (buf == "on")
            //                {
            //                    ExtendedAnimation = true;
            //                }
            //                else
            //                {
            //                    ExtendedAnimation = false;
            //                }
            //            }
            //            else
            //            {
            //                ExtendedAnimation = true;
            //                SystemConfig.SetItem("Option", "ExtendedAnimation", "On");
            //            }

            //            // 武器準備アニメ
            //            buf = Strings.LCase(GeneralLib.ReadIni("Option", "WeaponAnimation"));
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (buf == "on")
            //                {
            //                    WeaponAnimation = true;
            //                }
            //                else
            //                {
            //                    WeaponAnimation = false;
            //                }
            //            }
            //            else
            //            {
            //                WeaponAnimation = true;
            //                SystemConfig.SetItem("Option", "WeaponAnimation", "On");
            //            }

            //            // 移動アニメ
            //            buf = Strings.LCase(GeneralLib.ReadIni("Option", "MoveAnimation"));
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (buf == "on")
            //                {
            //                    MoveAnimation = true;
            //                }
            //                else
            //                {
            //                    MoveAnimation = false;
            //                }
            //            }
            //            else
            //            {
            //                MoveAnimation = true;
            //                SystemConfig.SetItem("Option", "MoveAnimation", "On");
            //            }

            //            // メッセージ速度を設定
            //            buf = GeneralLib.ReadIni("Option", "MessageWait");
            //            if (Information.IsNumeric(buf))
            //            {
            //                GUI.MessageWait = Conversions.ToInteger(buf);
            //                if (GUI.MessageWait > 10000000)
            //                {
            //                    GUI.MessageWait = 10000000;
            //                }
            //            }
            //            else
            //            {
            //                GUI.MessageWait = 700;
            //                SystemConfig.SetItem("Option", "MessageWait", "700");
            //            }

            //            // マス目を表示するかどうか
            //            buf = GeneralLib.ReadIni("Option", "Square");
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    ShowSquareLine = true;
            //                }
            //                else
            //                {
            //                    ShowSquareLine = false;
            //                }
            //            }
            //            else
            //            {
            //                ShowSquareLine = false;
            //                SystemConfig.SetItem("Option", "Square", "Off");
            //            }

            //            // 敵ターンにＢＧＭを変更するかどうか
            //            buf = GeneralLib.ReadIni("Option", "KeepEnemyBGM");
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    KeepEnemyBGM = true;
            //                }
            //                else
            //                {
            //                    KeepEnemyBGM = false;
            //                }
            //            }
            //            else
            //            {
            //                KeepEnemyBGM = false;
            //                SystemConfig.SetItem("Option", "KeepEnemyBGM", "Off");
            //            }

            //            // 音源のリセットデータの種類
            //            MidiResetType = GeneralLib.ReadIni("Option", "MidiReset");

            //            // 自動反撃モード
            //            buf = GeneralLib.ReadIni("Option", "AutoDefense");
            //            if (string.IsNullOrEmpty(buf))
            //            {
            //                buf = GeneralLib.ReadIni("Option", "AutoDeffence");
            //                if (!string.IsNullOrEmpty(buf))
            //                {
            //                    SystemConfig.SetItem("Option", "AutoDefense", buf);
            //                }
            //            }

            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    SystemConfig.AutoDefense = true;
            //                }
            //                else
            //                {
            //                    SystemConfig.AutoDefense = false;
            //                }
            //            }
            //            else
            //            {
            //                SystemConfig.AutoDefense = false;
            //                SystemConfig.SetItem("Option", "AutoDefense", "Off");
            //            }

            //            // カーソル自動移動
            //            buf = GeneralLib.ReadIni("Option", "AutoMoveCursor");
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    AutoMoveCursor = true;
            //                }
            //                else
            //                {
            //                    AutoMoveCursor = false;
            //                }
            //            }
            //            else
            //            {
            //                AutoMoveCursor = true;
            //                SystemConfig.SetItem("Option", "AutoMoveCursor", "On");
            //            }

            // 各ウィンドウをロード (メインウィンドウは先にロード済み)
            GUI.LoadForms();

            //            // 画像バッファの枚数
            //            buf = GeneralLib.ReadIni("Option", "ImageBufferNum");
            //            if (Information.IsNumeric(buf))
            //            {
            //                ImageBufferSize = Conversions.Toint(buf);
            //                if (ImageBufferSize < 5)
            //                {
            //                    // 最低でも5枚のバッファを使う
            //                    ImageBufferSize = 5;
            //                }
            //            }
            //            else
            //            {
            //                // デフォルトは64枚
            //                ImageBufferSize = 64;
            //                SystemConfig.SetItem("Option", "ImageBufferNum", "64");
            //            }

            //            // 画像バッファを作成
            //            GUI.MakePicBuf();

            //            // 画像バッファの最大サイズ
            //            buf = GeneralLib.ReadIni("Option", "MaxImageBufferSize");
            //            if (Information.IsNumeric(buf))
            //            {
            //                MaxImageBufferByteSize = (Conversions.ToDouble(buf) * 1024d * 1024d);
            //                if (MaxImageBufferByteSize < 1 * 1024 * 1024)
            //                {
            //                    // 最低でも1MBのバッファを使う
            //                    MaxImageBufferByteSize = 1 * 1024 * 1024;
            //                }
            //            }
            //            else
            //            {
            //                // デフォルトは8MB
            //                MaxImageBufferByteSize = 8 * 1024 * 1024;
            //                SystemConfig.SetItem("Option", "MaxImageBufferSize", "8");
            //            }

            //            // 拡大画像を画像バッファに保存するか
            //            buf = GeneralLib.ReadIni("Option", "KeepStretchedImage");
            //            if (!string.IsNullOrEmpty(buf))
            //            {
            //                if (Strings.LCase(buf) == "on")
            //                {
            //                    KeepStretchedImage = true;
            //                }
            //                else
            //                {
            //                    KeepStretchedImage = false;
            //                }
            //            }
            //            // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            else if (Conversions.ToBoolean(IsBitBltFasterThanStretchBlt()))
            //            {
            //                KeepStretchedImage = true;
            //                SystemConfig.SetItem("Option", "KeepStretchedImage", "On");
            //            }
            //            else
            //            {
            //                KeepStretchedImage = false;
            //                SystemConfig.SetItem("Option", "KeepStretchedImage", "Off");
            //            }

            //            // 透過描画にUseTransparentBltを使用するか
            //            if (GeneralLib.GetWinVersion() >= 500)
            //            {
            //                buf = GeneralLib.ReadIni("Option", "UseTransparentBlt");
            //                if (!string.IsNullOrEmpty(buf))
            //                {
            //                    if (Strings.LCase(buf) == "on")
            //                    {
            //                        UseTransparentBlt = true;
            //                    }
            //                    else
            //                    {
            //                        UseTransparentBlt = false;
            //                    }
            //                }
            //                else
            //                {
            //                    UseTransparentBlt = true;
            //                    SystemConfig.SetItem("Option", "UseTransparentBlt", "On");
            //                }
            //            }


            //            // マウスボタンの利き腕設定
            //            if (GUI.GetSystemMetrics(GUI.SM_SWAPBUTTON) == 0)
            //            {
            //                // 右利き用
            //                GUI.RButtonID = 0x2;
            //                GUI.LButtonID = 0x1;
            //            }
            //            else
            //            {
            //                // 左利き用
            //                GUI.RButtonID = 0x1;
            //                GUI.LButtonID = 0x2;
            //            }

            //            GUI.ListItemComment = new string[1];

            //            // エリアスデータをロード
            //            bool localFileExists1() { string argfname = AppPath + @"Data\System\alias.txt"; var ret = FileSystem.FileExists(argfname); return ret; }

            //            if (FileSystem.FileExists(ScenarioPath + @"Data\System\alias.txt"))
            //            {
            //                ALDList.Load(ScenarioPath + @"Data\System\alias.txt");
            //            }
            //            else if (localFileExists1())
            //            {
            //                ALDList.Load(AppPath + @"Data\System\alias.txt");
            //            }

            // スペシャルパワーデータをロード
            var spFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "sp.txt"),
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "mind.txt"),
                FileSystem.PathCombine(AppPath, "Data", "System", "sp.txt"),
                FileSystem.PathCombine(AppPath, "Data", "System", "mind.txt"),
            };
            foreach (var spFile in spFiles)
            {

                if (FileSystem.FileExists(spFile))
                {
                    SPDList.Load(spFile);
                    break;
                }
            }

            // 汎用アイテムデータをロード
            if (FileSystem.FileExists(ScenarioPath, @"Data\System\item.txt"))
            {
                IDList.Load(FileSystem.PathCombine(ScenarioPath, @"Data\System\item.txt"));
            }
            else if (FileSystem.FileExists(AppPath, @"Data\System\item.txt"))
            {
                IDList.Load(FileSystem.PathCombine(AppPath, @"Data\System\item.txt"));
            }
            // 地形データをロード
            string appTerrainPath = FileSystem.PathCombine(AppPath, "Data", "System", "terrain.txt");
            if (FileSystem.FileExists(appTerrainPath))
            {
                TDList.Load(appTerrainPath);
            }
            else
            {
                throw new TerminateException(@"地形データファイル「Data\System\terrain.txt」が見つかりません");
            }
            string scenarioTerrainPath = FileSystem.PathCombine(ScenarioPath, "Data", "System", "terrain.txt");
            if (FileSystem.FileExists(scenarioTerrainPath))
            {
                TDList.Load(scenarioTerrainPath);
            }

            // バトルコンフィグデータをロード
            var bcFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "battle.txt"),
                FileSystem.PathCombine(AppPath, "Data", "System", "battle.txt"),
            };
            foreach (var bcFile in bcFiles)
            {
                if (FileSystem.FileExists(bcFile))
                {
                    BCList.Load(bcFile);
                    break;
                }
            }
        }

        private void ConfigureExecuteFile(string fname)
        {
            //if (Strings.LCase(Strings.Right(fname, 4)) != ".src" && Strings.LCase(Strings.Right(fname, 4)) != ".eve")
            //{
            #region            //    // ダイアログを表示して読み込むファイルを指定する場合

            //    // ダイアログの初期フォルダを設定
            //    i = 0;
            //    ScenarioPath = GeneralLib.ReadIni("Log", "LastFolder");

            //    if (string.IsNullOrEmpty(ScenarioPath))
            //    {
            //        ScenarioPath = AppPath;
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    else if (FileSystem.Dir(ScenarioPath, FileAttribute.Directory) == ".")
            //    {
            //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "*.src")))
            //        {
            //            i = 3;
            //        }

            //        if (Strings.InStr(ScenarioPath, "テストデータ") > 0)
            //        {
            //            i = 2;
            //        }

            //        if (Strings.InStr(ScenarioPath, "戦闘アニメテスト") > 0)
            //        {
            //            i = 2;
            //        }
            //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "test.eve")))
            //        {
            //            i = 2;
            //        }
            //    }
            //    else
            //    {
            //        ScenarioPath = AppPath;
            //    }
            //    //    goto SkipErrorHandler;
            //    //ErrorHandler:
            //    //    ;
            //    //    ScenarioPath = AppPath;
            //    //SkipErrorHandler:
            //    //    ;
            //    if (Strings.Right(ScenarioPath, 1) != @"\")
            //    {
            //        ScenarioPath = ScenarioPath + @"\";
            //    }

            //    // 拡張データのフォルダを設定
            //    ExtDataPath = GeneralLib.ReadIni("Option", "ExtDataPath");
            //    if (Strings.Len(ExtDataPath) > 0)
            //    {
            //        if (Strings.Right(ExtDataPath, 1) != @"\")
            //        {
            //            ExtDataPath = ExtDataPath + @"\";
            //        }
            //    }

            //    ExtDataPath2 = GeneralLib.ReadIni("Option", "ExtDataPath2");
            //    if (Strings.Len(ExtDataPath2) > 0)
            //    {
            //        if (Strings.Right(ExtDataPath2, 1) != @"\")
            //        {
            //            ExtDataPath2 = ExtDataPath2 + @"\";
            //        }
            //    }

            // オープニング曲演奏
            Sound.StopBGM(true);
            Sound.StartBGM(Sound.BGMName("Opening"), true);

            // イベントデータを初期化
            Event.InitEventData();

            // タイトル画面を閉じる
            GUI.CloseTitleForm();

            //    // マウスカーソルを元に戻す
            //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //    GUI.ChangeStatus(GuiStatus.Default);

            //    // シナリオパスは変更される可能性があるので、MIDIファイルのサーチパスをリセット
            //    Sound.ResetMidiSearchPath();

            //    // プレイヤーにロードするファイルを尋ねる
            //    fname = FileDialog.LoadFileDialog("シナリオ／セーブファイルの指定", ScenarioPath, "", i, "ｲﾍﾞﾝﾄﾃﾞｰﾀ", "eve", "ｲﾍﾞﾝﾄﾃﾞｰﾀ"2, "eve"2, ftype3: "ｲﾍﾞﾝﾄﾃﾞｰﾀ"3, fsuffix3: "eve"3);

            //    // ファイルが指定されなかった場合はそのまま終了
            //    if (string.IsNullOrEmpty(fname))
            //    {
            //        TerminateSRC();
            //        Environment.Exit(0);
            //    }

            //    // シナリオのあるフォルダのパスを収得
            //    if (Strings.InStr(fname, @"\") > 0)
            //    {
            //        var loopTo = Strings.Len(fname);
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
            //            {
            //                break;
            //            }
            //        }

            //        ScenarioPath = Strings.Left(fname, Strings.Len(fname) - i);
            //    }
            //    else
            //    {
            //        ScenarioPath = AppPath;
            //    }

            //    if (Strings.Right(ScenarioPath, 1) != @"\")
            //    {
            //        ScenarioPath = ScenarioPath + @"\";
            //    }
            //    // ADD START MARGE
            //    // シナリオパスが決定した段階で拡張データフォルダパスを再設定するように変更
            //    // 拡張データのフォルダを設定
            //    ExtDataPath = GeneralLib.ReadIni("Option", "ExtDataPath");
            //    if (Strings.Len(ExtDataPath) > 0)
            //    {
            //        if (Strings.Right(ExtDataPath, 1) != @"\")
            //        {
            //            ExtDataPath = ExtDataPath + @"\";
            //        }
            //    }

            //    ExtDataPath2 = GeneralLib.ReadIni("Option", "ExtDataPath2");
            //    if (Strings.Len(ExtDataPath2) > 0)
            //    {
            //        if (Strings.Right(ExtDataPath2, 1) != @"\")
            //        {
            //            ExtDataPath2 = ExtDataPath2 + @"\";
            //        }
            //    }
            #endregion
            //}
            //// ADD  END  MARGE
            //else
            //{
            //    // ドラッグ＆ドロップで読み込むファイルが指定された場合

            // ファイル名が無効の場合はそのまま終了
            if (string.IsNullOrEmpty(fname))
            {
                TerminateSRC();
                Environment.Exit(0);
            }

            // シナリオのあるフォルダのパスを収得
            ScenarioPath = Path.GetDirectoryName(fname);

            //// 拡張データのフォルダを設定
            //ExtDataPath = GeneralLib.ReadIni("Option", "ExtDataPath");
            //ExtDataPath2 = GeneralLib.ReadIni("Option", "ExtDataPath2");

            // オープニング曲演奏
            Sound.StopBGM(true);
            Sound.StartBGM(Sound.BGMName("Opening"), true);
            Event.InitEventData();
            GUI.CloseTitleForm();

            //// UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //GUI.ChangeStatus(GuiStatus.Default);
            //}

            //// ロングネームにしておく
            //// UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //fname = ScenarioPath + FileSystem.Dir(fname);
            //if (!FileSystem.FileExists(fname))
            //{
            //    GUI.ErrorMessage("指定したファイルが存在しません");
            //    TerminateSRC();
            //}

            //if (Strings.InStr(fname, "不要ファイル削除") == 0 && Strings.InStr(fname, "必須修正") == 0)
            //{
            //    // 開いたフォルダをSrc.iniにセーブしておく
            //    SystemConfig.SetItem("Log", "LastFolder", ScenarioPath);
            //}
        }

        private void ExecuteFile(string fname)
        {
            if (Strings.LCase(Strings.Right(fname, 5)) == ".srcs")
            {
                using (var stream = File.OpenRead(fname))
                {
                    // セーブデータの読み込み
                    GUI.OpenNowLoadingForm();
                    LoadData(stream);
                    GUI.CloseNowLoadingForm();

                    // インターミッション
                    InterMission.InterMissionCommand(true);
                    if (!IsSubStage)
                    {
                        if (string.IsNullOrEmpty(Expression.GetValueAsString("次ステージ")))
                        {
                            GUI.ErrorMessage("次のステージのファイル名が設定されていません");
                            TerminateSRC();
                        }

                        StartScenario(Expression.GetValueAsString("次ステージ"));
                    }
                    else
                    {
                        IsSubStage = false;
                    }
                }
            }
            else if (Strings.LCase(Strings.Right(fname, 5)) == ".srcq")
            {
                // 中断データの読み込み
                GUI.LockGUI();
                RestoreData(fname, SRCSaveKind.Suspend);

                // 画面を書き直してステータスを表示
                GUI.RedrawScreen();
                GUIStatus.DisplayGlobalStatus();
                GUI.UnlockGUI();
            }
            else if (Strings.LCase(Strings.Right(fname, 4)) == ".eve")
            {
                // イベントファイルの実行
                StartScenario(fname);
            }
            else
            {
                GUI.ErrorMessage("「" + fname + "」はSRC用のファイルではありません！" + Constants.vbCr + Constants.vbLf + "拡張子が「.eve」のイベントファイル、" + "または拡張子が「.src」のセーブデータファイルを指定して下さい。");
                TerminateSRC();
            }
        }
    }
}
