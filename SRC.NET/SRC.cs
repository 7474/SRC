using System;
using System.Drawing;
using System.Windows.Forms;
using VB = Microsoft.VisualBasic;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class SRC
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // パイロットデータのリスト
        public static PilotDataList PDList = new PilotDataList();
        // ノンパイロットデータのリスト
        public static NonPilotDataList NPDList = new NonPilotDataList();
        // ユニットデータのリスト
        public static UnitDataList UDList = new UnitDataList();
        // アイテムデータのリスト
        public static ItemDataList IDList = new ItemDataList();
        // メッセージデータのリスト
        public static MessageDataList MDList = new MessageDataList();
        // 特殊効果データのリスト
        public static MessageDataList EDList = new MessageDataList();
        // 戦闘アニメデータのリスト
        public static MessageDataList ADList = new MessageDataList();
        // 拡張戦闘アニメデータのリスト
        public static MessageDataList EADList = new MessageDataList();
        // ダイアログデータのリスト
        public static DialogDataList DDList = new DialogDataList();
        // スペシャルパワーデータのリスト
        public static SpecialPowerDataList SPDList = new SpecialPowerDataList();
        // エリアスデータのリスト
        public static AliasDataList ALDList = new AliasDataList();
        // 地形データのリスト
        public static TerrainDataList TDList = new TerrainDataList();
        // バトルコンフィグデータのリスト
        public static BattleConfigDataList BCList = new BattleConfigDataList();


        // パイロットのリスト
        public static Pilots PList = new Pilots();
        // ユニットのリスト
        public static Units UList = new Units();
        // アイテムのリスト
        public static Items IList = new Items();

        // イベントファイル名
        public static string ScenarioFileName;
        // イベントファイル名のあるフォルダ
        public static string ScenarioPath;
        // セーブデータのファイルディスクリプタ
        public static short SaveDataFileNumber;
        // セーブデータのバージョン
        public static int SaveDataVersion;

        // そのステージが終了したかを示すフラグ
        public static bool IsScenarioFinished;
        // インターミッションコマンドによるステージかどうかを示すフラグ
        public static bool IsSubStage;
        // コマンドがキャンセルされたかどうかを示すフラグ
        public static bool IsCanceled;

        // フェイズ名
        public static string Stage;
        // ターン数
        public static short Turn;
        // 総ターン数
        public static int TotalTurn;
        // 総資金
        public static int Money;
        // 読み込まれているデータ数
        public static string[] Titles;
        // ローカルデータが読み込まれているか？
        public static bool IsLocalDataLoaded;

        // 最新のセーブデータのファイル名
        public static string LastSaveDataFileName;
        // リスタート用セーブデータが利用可能かどうか
        public static bool IsRestartSaveDataAvailable;
        // クイックロード用セーブデータが利用可能かどうか
        public static bool IsQuickSaveDataAvailable;

        // システムオプション
        // マス目の表示をするか
        public static bool ShowSquareLine;
        // 敵フェイズにはＢＧＭを変更しないか
        public static bool KeepEnemyBGM;
        // 拡張データフォルダへのパス
        public static string ExtDataPath;
        public static string ExtDataPath2;
        // MIDI音源リセットの種類
        public static string MidiResetType;
        // 自動防御モードを使うか
        public static bool AutoMoveCursor;
        // スペシャルパワーアニメを表示するか
        public static bool SpecialPowerAnimation;
        // 戦闘アニメを表示するか
        public static bool BattleAnimation;
        // 武器準備アニメを表示するか
        public static bool WeaponAnimation;
        // 拡大戦闘アニメを表示するか
        public static bool ExtendedAnimation;
        // 移動アニメを表示するか
        public static bool MoveAnimation;
        // 画像バッファの枚数
        public static short ImageBufferSize;
        // 画像バッファの最大バイト数
        public static int MaxImageBufferByteSize;
        // 拡大画像を画像バッファに保存するか
        public static bool KeepStretchedImage;
        // 透過描画にTransparentBltを使うか
        public static bool UseTransparentBlt;

        // SRC.exeのある場所
        public static string AppPath;

        // データ中にレベル指定を省略した場合のデフォルトのレベル値
        public const short DEFAULT_LEVEL = -1000;

        // UPGRADE_WARNING: Sub Main() が完了したときにアプリケーションは終了します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E08DDC71-66BA-424F-A612-80AF11498FF8"' をクリックしてください。
        public static void Main()
        {
            string fname;
            short i;
            string buf;
            int ret;

            // ２重起動禁止
            // UPGRADE_ISSUE: App プロパティ App.PrevInstance はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"' をクリックしてください。
            if (App.PrevInstance)
            {
                Environment.Exit(0);
            }

            // SRC.exeのある場所を調べる
            AppPath = My.MyProject.Application.Info.DirectoryPath;
            if (Strings.Right(AppPath, 1) != @"\")
            {
                AppPath = AppPath + @"\";
            }

            // SRCが正しくインストールされているかをチェック

            // Bitmap関係のチェック
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + "Bitmap", FileAttribute.Directory)) == 0)
            {
                string argmsg = "Bitmapフォルダがありません。" + Constants.vbCr + Constants.vbLf + "SRC.exeと同じフォルダに汎用グラフィック集をインストールしてください。";
                GUI.ErrorMessage(ref argmsg);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + "Ｂｉｔｍａｐ", FileAttribute.Directory)) > 0)
            {
                string argmsg1 = "Bitmapフォルダのフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + AppPath + "Ｂｉｔｍａｐ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                GUI.ErrorMessage(ref argmsg1);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Bitmap", FileAttribute.Directory)) > 0)
            {
                string argmsg2 = "Bitmapフォルダ内にさらにBitmapフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Bitmap\Bitmap" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
                GUI.ErrorMessage(ref argmsg2);
                Environment.Exit(0);
            }

            // イベントグラフィック
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Event", FileAttribute.Directory)) == 0)
            {
                string argmsg3 = @"Bitmap\Eventフォルダが見つかりません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
                GUI.ErrorMessage(ref argmsg3);
                Environment.Exit(0);
            }

            // マップグラフィック
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map", FileAttribute.Directory)) == 0)
            {
                string argmsg4 = @"Bitmap\Mapフォルダがありません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
                GUI.ErrorMessage(ref argmsg4);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain\plain0000.bmp")) == 0 & Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain0000.bmp")) == 0 & Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain0.bmp")) == 0)
            {
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\Map", FileAttribute.Directory)) > 0)
                {
                    string argmsg5 = @"Bitmap\Mapフォルダ内にさらにMapフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Bitmap\Map\Map" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
                    GUI.ErrorMessage(ref argmsg5);
                    Environment.Exit(0);
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\*", FileAttribute.Normal)) == 0)
                {
                    string argmsg6 = @"Bitmap\Mapフォルダ内にファイルがありません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
                    GUI.ErrorMessage(ref argmsg6);
                    Environment.Exit(0);
                }

                string argmsg7 = @"Bitmap\Mapフォルダ内にplain0000.bmpがありません。" + Constants.vbCr + Constants.vbLf + "一部のマップ画像ファイルしかインストールされていない恐れがあります。" + Constants.vbCr + Constants.vbLf + "新規インストールのファイルを使って汎用グラフィック集をインストールしてください。";
                GUI.ErrorMessage(ref argmsg7);
                Environment.Exit(0);
            }

            // 効果音
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + "Sound", FileAttribute.Directory)) == 0)
            {
                string argmsg8 = "Soundフォルダがありません。" + Constants.vbCr + Constants.vbLf + "SRC.exeと同じフォルダに効果音集をインストールしてください。";
                GUI.ErrorMessage(ref argmsg8);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + "Ｓｏｕｎｄ", FileAttribute.Directory)) > 0)
            {
                string argmsg9 = "Soundフォルダのフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + AppPath + "Ｓｏｕｎｄ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                GUI.ErrorMessage(ref argmsg9);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Sound\Sound", FileAttribute.Directory)) > 0)
            {
                string argmsg10 = "Soundフォルダ内にさらにSoundフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Sound\Sound" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
                GUI.ErrorMessage(ref argmsg10);
                Environment.Exit(0);
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (Strings.Len(FileSystem.Dir(AppPath + @"Sound\*", FileAttribute.Normal)) == 0)
            {
                string argmsg11 = "Soundフォルダ内にファイルがありません。" + Constants.vbCr + Constants.vbLf + "Soundフォルダ内に効果音集をインストールしてください。";
                GUI.ErrorMessage(ref argmsg11);
                Environment.Exit(0);
            }

            // メインウィンドウのロードとFlashの登録を実施
            GUI.LoadMainFormAndRegisterFlash();

            // Src.iniが無ければ作成
            bool localFileExists() { string argfname = AppPath + "Src.ini"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            if (!localFileExists())
            {
                CreateIniFile();
            }

            // 乱数の初期化
            VBMath.Randomize();

            // 時間解像度を変更する
            GeneralLib.timeBeginPeriod(1);

            // フルスクリーンモードを使う？
            string argini_section = "Option";
            string argini_entry = "FullScreen";
            if (Strings.LCase(GeneralLib.ReadIni(ref argini_section, ref argini_entry)) == "on")
            {
                GUI.ChangeDisplaySize(800, 600);
            }

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // タイトル画面を表示
            GUI.OpenTitleForm();

            // WAVE再生の手段は？
            string argini_section1 = "Option";
            string argini_entry1 = "UseDirectSound";
            switch (Strings.LCase(GeneralLib.ReadIni(ref argini_section1, ref argini_entry1)) ?? "")
            {
                case "on":
                    {
                        // DirectSoundの初期化を試みる
                        Sound.InitDirectSound();
                        break;
                    }

                case "off":
                    {
                        Sound.UseDirectSound = false;
                        break;
                    }

                default:
                    {
                        // DirectSoundの初期化を試みる
                        Sound.InitDirectSound();
                        break;
                    }
                    // DirectSoundが使用可能かどうかで設定を切り替え
                    // If UseDirectSound Then
                    // WriteIni "Option", "UseDirectSound", "On"
                    // Else
                    // WriteIni "Option", "UseDirectSound", "Off"
                    // End If
            }

            // MIDI演奏の手段は？
            string argini_section5 = "Option";
            string argini_entry5 = "UseDirectMusic";
            switch (Strings.LCase(GeneralLib.ReadIni(ref argini_section5, ref argini_entry5)) ?? "")
            {
                case "on":
                    {
                        // DirectMusicの初期化を試みる
                        Sound.InitDirectMusic();
                        break;
                    }

                case "off":
                    {
                        Sound.UseMCI = true;
                        break;
                    }

                default:
                    {
                        if (GeneralLib.GetWinVersion() >= 500)
                        {
                            // NT系のOSではデフォルトでDirectMusicを使う
                            // DirectMusicの初期化を試みる
                            Sound.InitDirectMusic();
                            // DirectMusicが使用可能かどうかで設定を切り替え
                            if (Sound.UseDirectMusic)
                            {
                                string argini_section2 = "Option";
                                string argini_entry2 = "UseDirectMusic";
                                string argini_data = "On";
                                GeneralLib.WriteIni(ref argini_section2, ref argini_entry2, ref argini_data);
                            }
                            else
                            {
                                string argini_section3 = "Option";
                                string argini_entry3 = "UseDirectMusic";
                                string argini_data1 = "Off";
                                GeneralLib.WriteIni(ref argini_section3, ref argini_entry3, ref argini_data1);
                            }
                        }
                        else
                        {
                            // NT系OSでなければMCIを使う
                            Sound.UseMCI = true;
                            string argini_section4 = "Option";
                            string argini_entry4 = "UseDirectMusic";
                            string argini_data2 = "Off";
                            GeneralLib.WriteIni(ref argini_section4, ref argini_entry4, ref argini_data2);
                        }

                        break;
                    }
            }

            string argini_section7 = "Option";
            string argini_entry7 = "MIDIPortID";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section7, ref argini_entry7)))
            {
                string argini_section6 = "Option";
                string argini_entry6 = "MIDIPortID";
                string argini_data3 = "0";
                GeneralLib.WriteIni(ref argini_section6, ref argini_entry6, ref argini_data3);
            }

            // MP3の再生音量
            string argini_section8 = "Option";
            string argini_entry8 = "MP3Volume";
            buf = GeneralLib.ReadIni(ref argini_section8, ref argini_entry8);
            if (string.IsNullOrEmpty(buf))
            {
                string argini_section9 = "Option";
                string argini_entry9 = "MP3Volume";
                string argini_data4 = "50";
                GeneralLib.WriteIni(ref argini_section9, ref argini_entry9, ref argini_data4);
                Sound.MP3Volume = 50;
            }
            else
            {
                Sound.MP3Volume = (short)GeneralLib.StrToLng(ref buf);
                if (Sound.MP3Volume < 0)
                {
                    string argini_section10 = "Option";
                    string argini_entry10 = "MP3Volume";
                    string argini_data5 = "0";
                    GeneralLib.WriteIni(ref argini_section10, ref argini_entry10, ref argini_data5);
                    Sound.MP3Volume = 0;
                }
                else if (Sound.MP3Volume > 100)
                {
                    string argini_section11 = "Option";
                    string argini_entry11 = "MP3Volume";
                    string argini_data6 = "100";
                    GeneralLib.WriteIni(ref argini_section11, ref argini_entry11, ref argini_data6);
                    Sound.MP3Volume = 100;
                }
            }

            // MP3の出力フレーム数
            string argini_section12 = "Option";
            string argini_entry12 = "MP3OutputBlock";
            buf = GeneralLib.ReadIni(ref argini_section12, ref argini_entry12);
            if (string.IsNullOrEmpty(buf))
            {
                string argini_section13 = "Option";
                string argini_entry13 = "MP3OutputBlock";
                string argini_data7 = "20";
                GeneralLib.WriteIni(ref argini_section13, ref argini_entry13, ref argini_data7);
            }

            // MP3の入力直後のスリープ時間
            string argini_section14 = "Option";
            string argini_entry14 = "MP3InputSleep";
            buf = GeneralLib.ReadIni(ref argini_section14, ref argini_entry14);
            if (string.IsNullOrEmpty(buf))
            {
                string argini_section15 = "Option";
                string argini_entry15 = "MP3InputSleep";
                string argini_data8 = "5";
                GeneralLib.WriteIni(ref argini_section15, ref argini_entry15, ref argini_data8);
            }

            // ＢＧＭ用MIDIファイル設定
            string argini_section17 = "BGM";
            string argini_entry17 = "Opening";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section17, ref argini_entry17)))
            {
                string argini_section16 = "BGM";
                string argini_entry16 = "Opening";
                string argini_data9 = "Opening.mid";
                GeneralLib.WriteIni(ref argini_section16, ref argini_entry16, ref argini_data9);
            }

            string argini_section19 = "BGM";
            string argini_entry19 = "Map1";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section19, ref argini_entry19)))
            {
                string argini_section18 = "BGM";
                string argini_entry18 = "Map1";
                string argini_data10 = "Map1.mid";
                GeneralLib.WriteIni(ref argini_section18, ref argini_entry18, ref argini_data10);
            }

            string argini_section21 = "BGM";
            string argini_entry21 = "Map2";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section21, ref argini_entry21)))
            {
                string argini_section20 = "BGM";
                string argini_entry20 = "Map2";
                string argini_data11 = "Map2.mid";
                GeneralLib.WriteIni(ref argini_section20, ref argini_entry20, ref argini_data11);
            }

            string argini_section23 = "BGM";
            string argini_entry23 = "Map3";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section23, ref argini_entry23)))
            {
                string argini_section22 = "BGM";
                string argini_entry22 = "Map3";
                string argini_data12 = "Map3.mid";
                GeneralLib.WriteIni(ref argini_section22, ref argini_entry22, ref argini_data12);
            }

            string argini_section25 = "BGM";
            string argini_entry25 = "Map4";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section25, ref argini_entry25)))
            {
                string argini_section24 = "BGM";
                string argini_entry24 = "Map4";
                string argini_data13 = "Map4.mid";
                GeneralLib.WriteIni(ref argini_section24, ref argini_entry24, ref argini_data13);
            }

            string argini_section27 = "BGM";
            string argini_entry27 = "Map5";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section27, ref argini_entry27)))
            {
                string argini_section26 = "BGM";
                string argini_entry26 = "Map5";
                string argini_data14 = "Map5.mid";
                GeneralLib.WriteIni(ref argini_section26, ref argini_entry26, ref argini_data14);
            }

            string argini_section29 = "BGM";
            string argini_entry29 = "Map6";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section29, ref argini_entry29)))
            {
                string argini_section28 = "BGM";
                string argini_entry28 = "Map6";
                string argini_data15 = "Map6.mid";
                GeneralLib.WriteIni(ref argini_section28, ref argini_entry28, ref argini_data15);
            }

            string argini_section31 = "BGM";
            string argini_entry31 = "Briefing";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section31, ref argini_entry31)))
            {
                string argini_section30 = "BGM";
                string argini_entry30 = "Briefing";
                string argini_data16 = "Briefing.mid";
                GeneralLib.WriteIni(ref argini_section30, ref argini_entry30, ref argini_data16);
            }

            string argini_section33 = "BGM";
            string argini_entry33 = "Intermission";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section33, ref argini_entry33)))
            {
                string argini_section32 = "BGM";
                string argini_entry32 = "Intermission";
                string argini_data17 = "Intermission.mid";
                GeneralLib.WriteIni(ref argini_section32, ref argini_entry32, ref argini_data17);
            }

            string argini_section35 = "BGM";
            string argini_entry35 = "Subtitle";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section35, ref argini_entry35)))
            {
                string argini_section34 = "BGM";
                string argini_entry34 = "Subtitle";
                string argini_data18 = "Subtitle.mid";
                GeneralLib.WriteIni(ref argini_section34, ref argini_entry34, ref argini_data18);
            }

            string argini_section37 = "BGM";
            string argini_entry37 = "End";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section37, ref argini_entry37)))
            {
                string argini_section36 = "BGM";
                string argini_entry36 = "End";
                string argini_data19 = "End.mid";
                GeneralLib.WriteIni(ref argini_section36, ref argini_entry36, ref argini_data19);
            }

            string argini_section39 = "BGM";
            string argini_entry39 = "default";
            if (string.IsNullOrEmpty(GeneralLib.ReadIni(ref argini_section39, ref argini_entry39)))
            {
                string argini_section38 = "BGM";
                string argini_entry38 = "default";
                string argini_data20 = "default.mid";
                GeneralLib.WriteIni(ref argini_section38, ref argini_entry38, ref argini_data20);
            }


            // 起動時の引数から読み込むファイルを探す
            if (Strings.Left(Interaction.Command(), 1) == "\"")
            {
                fname = Strings.Mid(Interaction.Command(), 2, Strings.Len(Interaction.Command()) - 2);
            }
            else
            {
                fname = Interaction.Command();
            }

            if (Strings.LCase(Strings.Right(fname, 4)) != ".src" & Strings.LCase(Strings.Right(fname, 4)) != ".eve")
            {
                // ダイアログを表示して読み込むファイルを指定する場合

                // ダイアログの初期フォルダを設定
                i = (short)0;
                string argini_section40 = "Log";
                string argini_entry40 = "LastFolder";
                ScenarioPath = GeneralLib.ReadIni(ref argini_section40, ref argini_entry40);
                ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 18399


                Input:
                            On Error GoTo ErrorHandler

                 */
                if (string.IsNullOrEmpty(ScenarioPath))
                {
                    ScenarioPath = AppPath;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                else if (FileSystem.Dir(ScenarioPath, FileAttribute.Directory) == ".")
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "*.src")))
                    {
                        i = (short)3;
                    }

                    if (Strings.InStr(ScenarioPath, "テストデータ") > 0)
                    {
                        i = (short)2;
                    }

                    if (Strings.InStr(ScenarioPath, "戦闘アニメテスト") > 0)
                    {
                        i = (short)2;
                    }
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "test.eve")))
                    {
                        i = (short)2;
                    }
                }
                else
                {
                    ScenarioPath = AppPath;
                };
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 19815


                Input:
                            On Error GoTo 0

                 */
                goto SkipErrorHandler;
                ErrorHandler:
                ;
                ScenarioPath = AppPath;
                SkipErrorHandler:
                ;
                if (Strings.Right(ScenarioPath, 1) != @"\")
                {
                    ScenarioPath = ScenarioPath + @"\";
                }

                // 拡張データのフォルダを設定
                string argini_section41 = "Option";
                string argini_entry41 = "ExtDataPath";
                ExtDataPath = GeneralLib.ReadIni(ref argini_section41, ref argini_entry41);
                if (Strings.Len(ExtDataPath) > 0)
                {
                    if (Strings.Right(ExtDataPath, 1) != @"\")
                    {
                        ExtDataPath = ExtDataPath + @"\";
                    }
                }

                string argini_section42 = "Option";
                string argini_entry42 = "ExtDataPath2";
                ExtDataPath2 = GeneralLib.ReadIni(ref argini_section42, ref argini_entry42);
                if (Strings.Len(ExtDataPath2) > 0)
                {
                    if (Strings.Right(ExtDataPath2, 1) != @"\")
                    {
                        ExtDataPath2 = ExtDataPath2 + @"\";
                    }
                }

                // オープニング曲演奏
                Sound.StopBGM(true);
                string argbgm_name = "Opening";
                string argbgm_name1 = Sound.BGMName(ref argbgm_name);
                Sound.StartBGM(ref argbgm_name1, true);

                // イベントデータを初期化
                Event_Renamed.InitEventData();

                // タイトル画面を閉じる
                GUI.CloseTitleForm();

                // マウスカーソルを元に戻す
                // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
                Cursor.Current = Cursors.Default;

                // シナリオパスは変更される可能性があるので、MIDIファイルのサーチパスをリセット
                Sound.ResetMidiSearchPath();

                // プレイヤーにロードするファイルを尋ねる
                string argdtitle = "シナリオ／セーブファイルの指定";
                string argdefault_file = "";
                string argftype = "ｲﾍﾞﾝﾄﾃﾞｰﾀ";
                string argfsuffix = "eve";
                string argftype2 = "ｾｰﾌﾞﾃﾞｰﾀ";
                string argfsuffix2 = "src";
                string argftype3 = "";
                string argfsuffix3 = "";
                fname = FileDialog.LoadFileDialog(ref argdtitle, ref ScenarioPath, ref argdefault_file, i, ref argftype, ref argfsuffix, ref argftype2, ref argfsuffix2, ftype3: ref argftype3, fsuffix3: ref argfsuffix3);

                // ファイルが指定されなかった場合はそのまま終了
                if (string.IsNullOrEmpty(fname))
                {
                    TerminateSRC();
                    Environment.Exit(0);
                }

                // シナリオのあるフォルダのパスを収得
                if (Strings.InStr(fname, @"\") > 0)
                {
                    var loopTo = (short)Strings.Len(fname);
                    for (i = (short)1; i <= loopTo; i++)
                    {
                        if (Strings.Mid(fname, Strings.Len(fname) - (int)i + 1, 1) == @"\")
                        {
                            break;
                        }
                    }

                    ScenarioPath = Strings.Left(fname, Strings.Len(fname) - (int)i);
                }
                else
                {
                    ScenarioPath = AppPath;
                }

                if (Strings.Right(ScenarioPath, 1) != @"\")
                {
                    ScenarioPath = ScenarioPath + @"\";
                }
                // ADD START MARGE
                // シナリオパスが決定した段階で拡張データフォルダパスを再設定するように変更
                // 拡張データのフォルダを設定
                string argini_section43 = "Option";
                string argini_entry43 = "ExtDataPath";
                ExtDataPath = GeneralLib.ReadIni(ref argini_section43, ref argini_entry43);
                if (Strings.Len(ExtDataPath) > 0)
                {
                    if (Strings.Right(ExtDataPath, 1) != @"\")
                    {
                        ExtDataPath = ExtDataPath + @"\";
                    }
                }

                string argini_section44 = "Option";
                string argini_entry44 = "ExtDataPath2";
                ExtDataPath2 = GeneralLib.ReadIni(ref argini_section44, ref argini_entry44);
                if (Strings.Len(ExtDataPath2) > 0)
                {
                    if (Strings.Right(ExtDataPath2, 1) != @"\")
                    {
                        ExtDataPath2 = ExtDataPath2 + @"\";
                    }
                }
            }
            // ADD  END  MARGE
            else
            {
                // ドラッグ＆ドロップで読み込むファイルが指定された場合

                // ファイル名が無効の場合はそのまま終了
                if (string.IsNullOrEmpty(fname))
                {
                    TerminateSRC();
                    Environment.Exit(0);
                }

                // シナリオのあるフォルダのパスを収得
                if (Strings.InStr(fname, @"\") > 0)
                {
                    var loopTo1 = (short)Strings.Len(fname);
                    for (i = (short)1; i <= loopTo1; i++)
                    {
                        if (Strings.Mid(fname, Strings.Len(fname) - (int)i + 1, 1) == @"\")
                        {
                            break;
                        }
                    }

                    ScenarioPath = Strings.Left(fname, Strings.Len(fname) - (int)i);
                }
                else
                {
                    ScenarioPath = AppPath;
                }

                if (Strings.Right(ScenarioPath, 1) != @"\")
                {
                    ScenarioPath = ScenarioPath + @"\";
                }

                // 拡張データのフォルダを設定
                string argini_section45 = "Option";
                string argini_entry45 = "ExtDataPath";
                ExtDataPath = GeneralLib.ReadIni(ref argini_section45, ref argini_entry45);
                if (Strings.Len(ExtDataPath) > 0)
                {
                    if (Strings.Right(ExtDataPath, 1) != @"\")
                    {
                        ExtDataPath = ExtDataPath + @"\";
                    }
                }

                string argini_section46 = "Option";
                string argini_entry46 = "ExtDataPath2";
                ExtDataPath2 = GeneralLib.ReadIni(ref argini_section46, ref argini_entry46);
                if (Strings.Len(ExtDataPath2) > 0)
                {
                    if (Strings.Right(ExtDataPath2, 1) != @"\")
                    {
                        ExtDataPath2 = ExtDataPath2 + @"\";
                    }
                }

                // オープニング曲演奏
                Sound.StopBGM(true);
                string argbgm_name2 = "Opening";
                string argbgm_name3 = Sound.BGMName(ref argbgm_name2);
                Sound.StartBGM(ref argbgm_name3, true);
                Event_Renamed.InitEventData();
                GUI.CloseTitleForm();

                // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
                Cursor.Current = Cursors.Default;
            }

            // ロングネームにしておく
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            fname = ScenarioPath + FileSystem.Dir(fname);
            if (!GeneralLib.FileExists(ref fname))
            {
                string argmsg12 = "指定したファイルが存在しません";
                GUI.ErrorMessage(ref argmsg12);
                TerminateSRC();
            }

            if (Strings.InStr(fname, "不要ファイル削除") == 0 & Strings.InStr(fname, "必須修正") == 0)
            {
                // 開いたフォルダをSrc.iniにセーブしておく
                string argini_section47 = "Log";
                string argini_entry47 = "LastFolder";
                GeneralLib.WriteIni(ref argini_section47, ref argini_entry47, ref ScenarioPath);
            }

            // Src.iniから各種パラメータの読み込み

            // スペシャルパワーアニメ
            string argini_section48 = "Option";
            string argini_entry48 = "SpecialPowerAnimation";
            buf = GeneralLib.ReadIni(ref argini_section48, ref argini_entry48);
            if (string.IsNullOrEmpty(buf))
            {
                string argini_section49 = "Option";
                string argini_entry49 = "MindEffect";
                buf = GeneralLib.ReadIni(ref argini_section49, ref argini_entry49);
                if (!string.IsNullOrEmpty(buf))
                {
                    string argini_section50 = "Option";
                    string argini_entry50 = "SpecialPowerAnimation";
                    GeneralLib.WriteIni(ref argini_section50, ref argini_entry50, ref buf);
                }
            }

            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    SpecialPowerAnimation = true;
                }
                else
                {
                    SpecialPowerAnimation = false;
                }
            }
            else if (SpecialPowerAnimation)
            {
                string argini_section51 = "Option";
                string argini_entry51 = "SpecialPowerAnimation";
                string argini_data21 = "On";
                GeneralLib.WriteIni(ref argini_section51, ref argini_entry51, ref argini_data21);
            }
            else
            {
                string argini_section52 = "Option";
                string argini_entry52 = "SpecialPowerAnimation";
                string argini_data22 = "Off";
                GeneralLib.WriteIni(ref argini_section52, ref argini_entry52, ref argini_data22);
            }

            // 戦闘アニメ
            string argini_section53 = "Option";
            string argini_entry53 = "BattleAnimation";
            buf = Strings.LCase(GeneralLib.ReadIni(ref argini_section53, ref argini_entry53));
            if (!string.IsNullOrEmpty(buf))
            {
                if (buf == "on")
                {
                    BattleAnimation = true;
                }
                else
                {
                    BattleAnimation = false;
                }
            }
            else if (BattleAnimation)
            {
                string argini_section54 = "Option";
                string argini_entry54 = "BattleAnimation";
                string argini_data23 = "On";
                GeneralLib.WriteIni(ref argini_section54, ref argini_entry54, ref argini_data23);
            }
            else
            {
                string argini_section55 = "Option";
                string argini_entry55 = "BattleAnimation";
                string argini_data24 = "Off";
                GeneralLib.WriteIni(ref argini_section55, ref argini_entry55, ref argini_data24);
            }

            // 拡大戦闘アニメ
            string argini_section56 = "Option";
            string argini_entry56 = "ExtendedAnimation";
            buf = Strings.LCase(GeneralLib.ReadIni(ref argini_section56, ref argini_entry56));
            if (!string.IsNullOrEmpty(buf))
            {
                if (buf == "on")
                {
                    ExtendedAnimation = true;
                }
                else
                {
                    ExtendedAnimation = false;
                }
            }
            else
            {
                ExtendedAnimation = true;
                string argini_section57 = "Option";
                string argini_entry57 = "ExtendedAnimation";
                string argini_data25 = "On";
                GeneralLib.WriteIni(ref argini_section57, ref argini_entry57, ref argini_data25);
            }

            // 武器準備アニメ
            string argini_section58 = "Option";
            string argini_entry58 = "WeaponAnimation";
            buf = Strings.LCase(GeneralLib.ReadIni(ref argini_section58, ref argini_entry58));
            if (!string.IsNullOrEmpty(buf))
            {
                if (buf == "on")
                {
                    WeaponAnimation = true;
                }
                else
                {
                    WeaponAnimation = false;
                }
            }
            else
            {
                WeaponAnimation = true;
                string argini_section59 = "Option";
                string argini_entry59 = "WeaponAnimation";
                string argini_data26 = "On";
                GeneralLib.WriteIni(ref argini_section59, ref argini_entry59, ref argini_data26);
            }

            // 移動アニメ
            string argini_section60 = "Option";
            string argini_entry60 = "MoveAnimation";
            buf = Strings.LCase(GeneralLib.ReadIni(ref argini_section60, ref argini_entry60));
            if (!string.IsNullOrEmpty(buf))
            {
                if (buf == "on")
                {
                    MoveAnimation = true;
                }
                else
                {
                    MoveAnimation = false;
                }
            }
            else
            {
                MoveAnimation = true;
                string argini_section61 = "Option";
                string argini_entry61 = "MoveAnimation";
                string argini_data27 = "On";
                GeneralLib.WriteIni(ref argini_section61, ref argini_entry61, ref argini_data27);
            }

            // メッセージ速度を設定
            string argini_section62 = "Option";
            string argini_entry62 = "MessageWait";
            buf = GeneralLib.ReadIni(ref argini_section62, ref argini_entry62);
            if (Information.IsNumeric(buf))
            {
                GUI.MessageWait = Conversions.ToInteger(buf);
                if (GUI.MessageWait > 10000000)
                {
                    GUI.MessageWait = 10000000;
                }
            }
            else
            {
                GUI.MessageWait = 700;
                string argini_section63 = "Option";
                string argini_entry63 = "MessageWait";
                string argini_data28 = "700";
                GeneralLib.WriteIni(ref argini_section63, ref argini_entry63, ref argini_data28);
            }

            // マス目を表示するかどうか
            string argini_section64 = "Option";
            string argini_entry64 = "Square";
            buf = GeneralLib.ReadIni(ref argini_section64, ref argini_entry64);
            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    ShowSquareLine = true;
                }
                else
                {
                    ShowSquareLine = false;
                }
            }
            else
            {
                ShowSquareLine = false;
                string argini_section65 = "Option";
                string argini_entry65 = "Square";
                string argini_data29 = "Off";
                GeneralLib.WriteIni(ref argini_section65, ref argini_entry65, ref argini_data29);
            }

            // 敵ターンにＢＧＭを変更するかどうか
            string argini_section66 = "Option";
            string argini_entry66 = "KeepEnemyBGM";
            buf = GeneralLib.ReadIni(ref argini_section66, ref argini_entry66);
            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    KeepEnemyBGM = true;
                }
                else
                {
                    KeepEnemyBGM = false;
                }
            }
            else
            {
                KeepEnemyBGM = false;
                string argini_section67 = "Option";
                string argini_entry67 = "KeepEnemyBGM";
                string argini_data30 = "Off";
                GeneralLib.WriteIni(ref argini_section67, ref argini_entry67, ref argini_data30);
            }

            // 音源のリセットデータの種類
            string argini_section68 = "Option";
            string argini_entry68 = "MidiReset";
            MidiResetType = GeneralLib.ReadIni(ref argini_section68, ref argini_entry68);

            // 自動反撃モード
            string argini_section69 = "Option";
            string argini_entry69 = "AutoDefense";
            buf = GeneralLib.ReadIni(ref argini_section69, ref argini_entry69);
            if (string.IsNullOrEmpty(buf))
            {
                string argini_section70 = "Option";
                string argini_entry70 = "AutoDeffence";
                buf = GeneralLib.ReadIni(ref argini_section70, ref argini_entry70);
                if (!string.IsNullOrEmpty(buf))
                {
                    string argini_section71 = "Option";
                    string argini_entry71 = "AutoDefense";
                    GeneralLib.WriteIni(ref argini_section71, ref argini_entry71, ref buf);
                }
            }

            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = true;
                }
                else
                {
                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = false;
                }
            }
            else
            {
                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = false;
                string argini_section72 = "Option";
                string argini_entry72 = "AutoDefense";
                string argini_data31 = "Off";
                GeneralLib.WriteIni(ref argini_section72, ref argini_entry72, ref argini_data31);
            }

            // カーソル自動移動
            string argini_section73 = "Option";
            string argini_entry73 = "AutoMoveCursor";
            buf = GeneralLib.ReadIni(ref argini_section73, ref argini_entry73);
            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    AutoMoveCursor = true;
                }
                else
                {
                    AutoMoveCursor = false;
                }
            }
            else
            {
                AutoMoveCursor = true;
                string argini_section74 = "Option";
                string argini_entry74 = "AutoMoveCursor";
                string argini_data32 = "On";
                GeneralLib.WriteIni(ref argini_section74, ref argini_entry74, ref argini_data32);
            }

            // 各ウィンドウをロード (メインウィンドウは先にロード済み)
            GUI.LoadForms();

            // 画像バッファの枚数
            string argini_section75 = "Option";
            string argini_entry75 = "ImageBufferNum";
            buf = GeneralLib.ReadIni(ref argini_section75, ref argini_entry75);
            if (Information.IsNumeric(buf))
            {
                ImageBufferSize = Conversions.ToShort(buf);
                if (ImageBufferSize < 5)
                {
                    // 最低でも5枚のバッファを使う
                    ImageBufferSize = 5;
                }
            }
            else
            {
                // デフォルトは64枚
                ImageBufferSize = 64;
                string argini_section76 = "Option";
                string argini_entry76 = "ImageBufferNum";
                string argini_data33 = "64";
                GeneralLib.WriteIni(ref argini_section76, ref argini_entry76, ref argini_data33);
            }

            // 画像バッファを作成
            GUI.MakePicBuf();

            // 画像バッファの最大サイズ
            string argini_section77 = "Option";
            string argini_entry77 = "MaxImageBufferSize";
            buf = GeneralLib.ReadIni(ref argini_section77, ref argini_entry77);
            if (Information.IsNumeric(buf))
            {
                MaxImageBufferByteSize = (int)(Conversions.ToDouble(buf) * 1024d * 1024d);
                if (MaxImageBufferByteSize < 1 * 1024 * 1024)
                {
                    // 最低でも1MBのバッファを使う
                    MaxImageBufferByteSize = 1 * 1024 * 1024;
                }
            }
            else
            {
                // デフォルトは8MB
                MaxImageBufferByteSize = 8 * 1024 * 1024;
                string argini_section78 = "Option";
                string argini_entry78 = "MaxImageBufferSize";
                string argini_data34 = "8";
                GeneralLib.WriteIni(ref argini_section78, ref argini_entry78, ref argini_data34);
            }

            // 拡大画像を画像バッファに保存するか
            string argini_section79 = "Option";
            string argini_entry79 = "KeepStretchedImage";
            buf = GeneralLib.ReadIni(ref argini_section79, ref argini_entry79);
            if (!string.IsNullOrEmpty(buf))
            {
                if (Strings.LCase(buf) == "on")
                {
                    KeepStretchedImage = true;
                }
                else
                {
                    KeepStretchedImage = false;
                }
            }
            // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            else if (Conversions.ToBoolean(IsBitBltFasterThanStretchBlt()))
            {
                KeepStretchedImage = true;
                string argini_section80 = "Option";
                string argini_entry80 = "KeepStretchedImage";
                string argini_data35 = "On";
                GeneralLib.WriteIni(ref argini_section80, ref argini_entry80, ref argini_data35);
            }
            else
            {
                KeepStretchedImage = false;
                string argini_section81 = "Option";
                string argini_entry81 = "KeepStretchedImage";
                string argini_data36 = "Off";
                GeneralLib.WriteIni(ref argini_section81, ref argini_entry81, ref argini_data36);
            }

            // 透過描画にUseTransparentBltを使用するか
            if (GeneralLib.GetWinVersion() >= 500)
            {
                string argini_section82 = "Option";
                string argini_entry82 = "UseTransparentBlt";
                buf = GeneralLib.ReadIni(ref argini_section82, ref argini_entry82);
                if (!string.IsNullOrEmpty(buf))
                {
                    if (Strings.LCase(buf) == "on")
                    {
                        UseTransparentBlt = true;
                    }
                    else
                    {
                        UseTransparentBlt = false;
                    }
                }
                else
                {
                    UseTransparentBlt = true;
                    string argini_section83 = "Option";
                    string argini_entry83 = "UseTransparentBlt";
                    string argini_data37 = "On";
                    GeneralLib.WriteIni(ref argini_section83, ref argini_entry83, ref argini_data37);
                }
            }


            // マウスボタンの利き腕設定
            if (GUI.GetSystemMetrics(GUI.SM_SWAPBUTTON) == 0)
            {
                // 右利き用
                GUI.RButtonID = 0x2;
                GUI.LButtonID = 0x1;
            }
            else
            {
                // 左利き用
                GUI.RButtonID = 0x1;
                GUI.LButtonID = 0x2;
            }

            GUI.ListItemComment = new string[1];

            // エリアスデータをロード
            bool localFileExists1() { string argfname = AppPath + @"Data\System\alias.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname2 = ScenarioPath + @"Data\System\alias.txt";
            if (GeneralLib.FileExists(ref argfname2))
            {
                string argfname = ScenarioPath + @"Data\System\alias.txt";
                ALDList.Load(ref argfname);
            }
            else if (localFileExists1())
            {
                string argfname1 = AppPath + @"Data\System\alias.txt";
                ALDList.Load(ref argfname1);
            }
            // スペシャルパワーデータをロード
            bool localFileExists2() { string argfname = ScenarioPath + @"Data\System\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists3() { string argfname = AppPath + @"Data\System\sp.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists4() { string argfname = AppPath + @"Data\System\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname7 = ScenarioPath + @"Data\System\sp.txt";
            if (GeneralLib.FileExists(ref argfname7))
            {
                string argfname3 = ScenarioPath + @"Data\System\sp.txt";
                SPDList.Load(ref argfname3);
            }
            else if (localFileExists2())
            {
                string argfname4 = ScenarioPath + @"Data\System\mind.txt";
                SPDList.Load(ref argfname4);
            }
            else if (localFileExists3())
            {
                string argfname5 = AppPath + @"Data\System\sp.txt";
                SPDList.Load(ref argfname5);
            }
            else if (localFileExists4())
            {
                string argfname6 = AppPath + @"Data\System\mind.txt";
                SPDList.Load(ref argfname6);
            }
            // 汎用アイテムデータをロード
            bool localFileExists5() { string argfname = AppPath + @"Data\System\item.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname10 = ScenarioPath + @"Data\System\item.txt";
            if (GeneralLib.FileExists(ref argfname10))
            {
                string argfname8 = ScenarioPath + @"Data\System\item.txt";
                IDList.Load(ref argfname8);
            }
            else if (localFileExists5())
            {
                string argfname9 = AppPath + @"Data\System\item.txt";
                IDList.Load(ref argfname9);
            }
            // 地形データをロード
            string argfname12 = AppPath + @"Data\System\terrain.txt";
            if (GeneralLib.FileExists(ref argfname12))
            {
                string argfname11 = AppPath + @"Data\System\terrain.txt";
                TDList.Load(ref argfname11);
            }
            else
            {
                string argmsg13 = @"地形データファイル「Data\System\terrain.txt」が見つかりません";
                GUI.ErrorMessage(ref argmsg13);
                TerminateSRC();
            }

            string argfname14 = ScenarioPath + @"Data\System\terrain.txt";
            if (GeneralLib.FileExists(ref argfname14))
            {
                string argfname13 = ScenarioPath + @"Data\System\terrain.txt";
                TDList.Load(ref argfname13);
            }
            // バトルコンフィグデータをロード
            bool localFileExists6() { string argfname = AppPath + @"Data\System\battle.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname17 = ScenarioPath + @"Data\System\battle.txt";
            if (GeneralLib.FileExists(ref argfname17))
            {
                string argfname15 = ScenarioPath + @"Data\System\battle.txt";
                BCList.Load(ref argfname15);
            }
            else if (localFileExists6())
            {
                string argfname16 = AppPath + @"Data\System\battle.txt";
                BCList.Load(ref argfname16);
            }

            // マップを初期化
            Map.InitMap();

            // 乱数系列を初期化
            GeneralLib.RndSeed = (int)Conversion.Int(1000000f * VBMath.Rnd());
            GeneralLib.RndReset();
            if (Strings.LCase(Strings.Right(fname, 4)) == ".src")
            {
                SaveDataFileNumber = (short)FileSystem.FreeFile();
                FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
                // 第１項目を読み込み
                FileSystem.Input(SaveDataFileNumber, ref buf);
                // 第１項目はセーブデータバージョン？
                if (Information.IsNumeric(buf))
                {
                    if (Conversions.ToInteger(buf) > 10000)
                    {
                        // バージョンデータであれば第２項目を読み込み
                        FileSystem.Input(SaveDataFileNumber, ref buf);
                    }
                }

                FileSystem.FileClose((int)SaveDataFileNumber);

                // データの種類を判定
                if (Information.IsNumeric(buf))
                {
                    // セーブデータの読み込み
                    GUI.OpenNowLoadingForm();
                    LoadData(ref fname);
                    GUI.CloseNowLoadingForm();

                    // インターミッション
                    InterMission.InterMissionCommand(true);
                    if (!IsSubStage)
                    {
                        string argexpr = "次ステージ";
                        if (string.IsNullOrEmpty(Expression.GetValueAsString(ref argexpr)))
                        {
                            string argmsg14 = "次のステージのファイル名が設定されていません";
                            GUI.ErrorMessage(ref argmsg14);
                            TerminateSRC();
                        }

                        string argexpr1 = "次ステージ";
                        StartScenario(Expression.GetValueAsString(ref argexpr1));
                    }
                    else
                    {
                        IsSubStage = false;
                    }
                }
                else
                {
                    // 中断データの読み込み
                    GUI.LockGUI();
                    bool argquick_load = false;
                    RestoreData(ref fname, ref argquick_load);

                    // 画面を書き直してステータスを表示
                    GUI.RedrawScreen();
                    Status.DisplayGlobalStatus();
                    GUI.UnlockGUI();
                }
            }
            else if (Strings.LCase(Strings.Right(fname, 4)) == ".eve")
            {
                // イベントファイルの実行
                StartScenario(fname);
            }
            else
            {
                string argmsg15 = "「" + fname + "」はSRC用のファイルではありません！" + Constants.vbCr + Constants.vbLf + "拡張子が「.eve」のイベントファイル、" + "または拡張子が「.src」のセーブデータファイルを指定して下さい。";
                GUI.ErrorMessage(ref argmsg15);
                TerminateSRC();
            }
        }

        // INIファイルを作成する
        public static void CreateIniFile()
        {
            short f;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 40882


            Input:

                    On Error GoTo ErrorHandler

             */
            f = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(f, AppPath + "Src.ini", OpenMode.Output, OpenAccess.Write);
            FileSystem.PrintLine(f, ";SRCの設定ファイルです。");
            FileSystem.PrintLine(f, ";項目の内容に関してはヘルプの");
            FileSystem.PrintLine(f, "; 操作方法 => マップコマンド => 設定変更");
            FileSystem.PrintLine(f, ";の項を参照して下さい。");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, "[Option]");
            FileSystem.PrintLine(f, ";メッセージのウェイト。標準は700");
            FileSystem.PrintLine(f, "MessageWait=700");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";ターン数の表示 [On|Off]");
            FileSystem.PrintLine(f, "Turn=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";マス目の表示 [On|Off]");
            FileSystem.PrintLine(f, "Square=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";敵フェイズにはＢＧＭを変更しない [On|Off]");
            FileSystem.PrintLine(f, "KeepEnemyBGM=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";自動防御モード [On|Off]");
            FileSystem.PrintLine(f, "AutoDefense=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";自動カーソル移動 [On|Off]");
            FileSystem.PrintLine(f, "AutoMoveCursor=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";スペシャルパワーアニメ [On|Off]");
            FileSystem.PrintLine(f, "SpecialPowerAnimation=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";戦闘アニメ [On|Off]");
            FileSystem.PrintLine(f, "BattleAnimation=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";戦闘アニメの拡張機能 [On|Off]");
            FileSystem.PrintLine(f, "ExtendedAnimation=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";武器準備アニメの自動選択表示 [On|Off]");
            FileSystem.PrintLine(f, "WeaponAnimation=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";移動アニメ [On|Off]");
            FileSystem.PrintLine(f, "MoveAnimation=On");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";MIDI音源リセットの種類 [None|GM|GS|XG]");
            FileSystem.PrintLine(f, "MidiReset=None");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";MIDI演奏にDirectMusicを使う [On|Off]");
            if (GeneralLib.GetWinVersion() >= 500)
            {
                // NT系のOSではデフォルトでDirectMusicを使う
                // DirectMusicの初期化を試みる
                Sound.InitDirectMusic();
                // DirectMusicが使用可能かどうかで設定を切り替え
                if (Sound.UseDirectMusic)
                {
                    FileSystem.PrintLine(f, "UseDirectMusic=On");
                }
                else
                {
                    FileSystem.PrintLine(f, "UseDirectMusic=Off");
                }
            }
            else
            {
                // NT系OSでなければMCIを使う
                Sound.UseMCI = true;
                FileSystem.PrintLine(f, "UseDirectMusic=Off");
            }

            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";DirectMusicで使うMIDI音源のポート番号 [自動検索=0]");
            FileSystem.PrintLine(f, "MIDIPortID=0");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";MP3再生時の音量 (0～100)");
            FileSystem.PrintLine(f, "MP3Volume=50");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";MP3の出力フレーム数");
            FileSystem.PrintLine(f, "MP3OutputBlock=20");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";MP3の入力直後のスリープ時間(ミリ秒)");
            FileSystem.PrintLine(f, "MP3IutputSleep=5");
            FileSystem.PrintLine(f, "");
            // Print #f, ";WAV再生にDirectSoundを使う [On|Off]"
            // Print #f, "UseDirectSound=On"
            // Print #f, ""
            FileSystem.PrintLine(f, ";画像バッファの枚数");
            FileSystem.PrintLine(f, "ImageBufferNum=64");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";画像バッファの最大サイズ (MB)");
            FileSystem.PrintLine(f, "MaxImageBufferSize=8");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";拡大画像を画像バッファに保存する [On|Off]");
            FileSystem.PrintLine(f, "KeepStretchedImage=");
            FileSystem.PrintLine(f, "");
            if (GeneralLib.GetWinVersion() >= 500)
            {
                FileSystem.PrintLine(f, ";透過描画にAPI関数TransparentBltを使う [On|Off]");
                FileSystem.PrintLine(f, "UseTransparentBlt=On");
                FileSystem.PrintLine(f, "");
            }

            FileSystem.PrintLine(f, ";拡張データのフォルダ (フルパスで指定)");
            FileSystem.PrintLine(f, "ExtDataPath=");
            FileSystem.PrintLine(f, "ExtDataPath2=");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";デバッグモード [On|Off]");
            FileSystem.PrintLine(f, "DebugMode=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, ";新ＧＵＩ(テスト中) [On|Off]");
            FileSystem.PrintLine(f, "NewGUI=Off");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, "[Log]");
            FileSystem.PrintLine(f, ";前回使用したフォルダ");
            FileSystem.PrintLine(f, "LastFolder=");
            FileSystem.PrintLine(f, "");
            FileSystem.PrintLine(f, "[BGM]");
            FileSystem.PrintLine(f, ";SRC起動時");
            FileSystem.PrintLine(f, "Opening=Opening.mid");
            FileSystem.PrintLine(f, ";味方フェイズ開始時");
            FileSystem.PrintLine(f, "Map1=Map1.mid");
            FileSystem.PrintLine(f, ";敵フェイズ開始時");
            FileSystem.PrintLine(f, "Map2=Map2.mid");
            FileSystem.PrintLine(f, ";屋内マップの味方フェイズ開始時");
            FileSystem.PrintLine(f, "Map3=Map3.mid");
            FileSystem.PrintLine(f, ";屋内マップの敵フェイズ開始時");
            FileSystem.PrintLine(f, "Map4=Map4.mid");
            FileSystem.PrintLine(f, ";宇宙マップの味方フェイズ開始時");
            FileSystem.PrintLine(f, "Map5=Map5.mid");
            FileSystem.PrintLine(f, ";宇宙マップの敵フェイズ開始時");
            FileSystem.PrintLine(f, "Map6=Map6.mid");
            FileSystem.PrintLine(f, ";プロローグ・エピローグ開始時");
            FileSystem.PrintLine(f, "Briefing=Briefing.mid");
            FileSystem.PrintLine(f, ";インターミッション開始時");
            FileSystem.PrintLine(f, "Intermission=Intermission.mid");
            FileSystem.PrintLine(f, ";テロップ表示時");
            FileSystem.PrintLine(f, "Subtitle=Subtitle.mid");
            FileSystem.PrintLine(f, ";ゲームオーバー時");
            FileSystem.PrintLine(f, "End=End.mid");
            FileSystem.PrintLine(f, ";戦闘時のデフォルトMIDI");
            FileSystem.PrintLine(f, "default=default.mid");
            FileSystem.PrintLine(f, "");
            FileSystem.FileClose((int)f);
            ErrorHandler:
            ;

            // エラー発生
        }

        // KeepStretchedImageを使用すべきか決定するため、BitBltと
        // StretchBltの速度差を測定
        private static object IsBitBltFasterThanStretchBlt()
        {
            object IsBitBltFasterThanStretchBltRet = default;
            int stime, etime;
            int bb_time, sb_time;
            int ret;
            short i;
            {
                var withBlock = GUI.MainForm;
                // 描画領域を設定
                // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                {
                    var withBlock1 = withBlock.picStretchedTmp(0);
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock1.width = 128;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock1.Height = 128;
                }
                // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                {
                    var withBlock2 = withBlock.picStretchedTmp(1);
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock2.width = 128;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock2.Height = 128;
                }

                // StretchBltの転送速度を測定
                stime = GeneralLib.timeGetTime();
                for (i = 1; i <= 5; i++)
                    // UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = GUI.StretchBlt(withBlock.picStretchedTmp(0).hDC, 0, 0, 480, 480, withBlock.picUnit.hDC, 0, 0, 32, 32, GUI.SRCCOPY);
                etime = GeneralLib.timeGetTime();
                sb_time = etime - stime;

                // BitBltの転送速度を測定
                stime = GeneralLib.timeGetTime();
                for (i = 1; i <= 5; i++)
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = GUI.BitBlt(withBlock.picStretchedTmp(1).hDC, 0, 0, 480, 480, withBlock.picStretchedTmp(0).hDC, 0, 0, GUI.SRCCOPY);
                etime = GeneralLib.timeGetTime();
                bb_time = etime - stime;

                // 描画領域を開放
                // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                {
                    var withBlock3 = withBlock.picStretchedTmp(0);
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock3.Picture = null;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock3.width = 32;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock3.Height = 32;
                }
                // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                {
                    var withBlock4 = withBlock.picStretchedTmp(1);
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock4.Picture = null;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock4.width = 32;
                    // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock4.Height = 32;
                }
            }

            // BitBltがStretchBltより2倍以上速ければBitBltを優先して使用する
            if (2 * bb_time < sb_time)
            {
                // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                IsBitBltFasterThanStretchBltRet = true;
            }
            else
            {
                // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                IsBitBltFasterThanStretchBltRet = false;
            }

            return IsBitBltFasterThanStretchBltRet;
        }


        // イベントファイルfnameを実行
        public static void StartScenario(string fname)
        {
            short i;
            int ret;
            Font sf;

            // ファイルを検索
            bool localFileExists() { string argfname = ScenarioPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists1() { string argfname = AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            if (Strings.Len(fname) == 0)
            {
                TerminateSRC();
                Environment.Exit(0);
            }
            else if (localFileExists())
            {
                fname = ScenarioPath + fname;
            }
            else if (localFileExists1())
            {
                fname = AppPath + fname;
            }

            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if (string.IsNullOrEmpty(FileSystem.Dir(fname, FileAttribute.Normal)))
            {
                Interaction.MsgBox(fname + "が見つかりません");
                TerminateSRC();
            }

            // ウィンドウのタイトルを設定
            if (My.MyProject.Application.Info.Version.Minor % 2 == 0)
            {
                GUI.MainForm.Text = "SRC";
            }
            else
            {
                GUI.MainForm.Text = "SRC開発版";
            }

            ScenarioFileName = fname;
            if (!IsSubStage)
            {
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Date", FileAttribute.Directory)) > 0)
                {
                    string argmsg = "シナリオ側のDataフォルダ名がDateになっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Date" + Constants.vbCr + Constants.vbLf + "フォルダ名をDataに直してください。";
                    GUI.ErrorMessage(ref argmsg);
                    TerminateSRC();
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｄａｔａ", FileAttribute.Directory)) > 0)
                {
                    string argmsg1 = "シナリオ側のDataフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｄａｔａ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                    GUI.ErrorMessage(ref argmsg1);
                    TerminateSRC();
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｂｉｔｍａｐ", FileAttribute.Directory)) > 0)
                {
                    string argmsg2 = "シナリオ側のBitmapフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｂｉｔｍａｐ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                    GUI.ErrorMessage(ref argmsg2);
                    TerminateSRC();
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｌｉｂ", FileAttribute.Directory)) > 0)
                {
                    string argmsg3 = "シナリオ側のLibフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｌｉｂ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                    GUI.ErrorMessage(ref argmsg3);
                    TerminateSRC();
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｍｉｄｉ", FileAttribute.Directory)) > 0)
                {
                    string argmsg4 = "シナリオ側のMidiフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｍｉｄｉ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                    GUI.ErrorMessage(ref argmsg4);
                    TerminateSRC();
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Ｓｏｕｎｄ", FileAttribute.Directory)) > 0)
                {
                    string argmsg5 = "シナリオ側のSoundフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + ScenarioPath + "Ｓｏｕｎｄ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
                    GUI.ErrorMessage(ref argmsg5);
                    TerminateSRC();
                }

                // 読み込むイベントファイル名に合わせて各種システム変数を設定
                string argvname1 = "次ステージ";
                if (!Expression.IsGlobalVariableDefined(ref argvname1))
                {
                    string argvname = "次ステージ";
                    Expression.DefineGlobalVariable(ref argvname);
                }

                string argvname2 = "次ステージ";
                string argnew_value = "";
                Expression.SetVariableAsString(ref argvname2, ref argnew_value);
                var loopTo = (short)Strings.Len(fname);
                for (i = 1; i <= loopTo; i++)
                {
                    if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
                    {
                        break;
                    }
                }

                string argvname3 = "ステージ";
                string argnew_value1 = Strings.Mid(fname, Strings.Len(fname) - i + 2);
                Expression.SetVariableAsString(ref argvname3, ref argnew_value1);
                string argvname5 = "セーブデータファイル名";
                if (!Expression.IsGlobalVariableDefined(ref argvname5))
                {
                    string argvname4 = "セーブデータファイル名";
                    Expression.DefineGlobalVariable(ref argvname4);
                }

                string argvname6 = "セーブデータファイル名";
                string argnew_value2 = Strings.Mid(fname, Strings.Len(fname) - i + 2, i - 5) + "までクリア.src";
                Expression.SetVariableAsString(ref argvname6, ref argnew_value2);

                // ウィンドウのタイトルにシナリオファイル名を表示
                GUI.MainForm.Text = GUI.MainForm.Text + " - " + Strings.Mid(fname, Strings.Len(fname) - i + 2, i - 5);
            }

            // 画面をクリアしておく
            {
                var withBlock = GUI.MainForm;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.PatBlt(withBlock.picMain(0).hDC, 0, 0, GUI.MainPWidth, GUI.MainPHeight, GUI.BLACKNESS);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.PatBlt(withBlock.picMain(1).hDC, 0, 0, GUI.MainPWidth, GUI.MainPHeight, GUI.BLACKNESS);
            }

            GUI.ScreenIsSaved = true;

            // イベントデータの読み込み
            string argload_mode = "";
            Event_Renamed.LoadEventData(ref fname, load_mode: ref argload_mode);

            // 各種変数の初期化
            Turn = 0;
            IsScenarioFinished = false;
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            LastSaveDataFileName = "";
            IsRestartSaveDataAvailable = false;
            IsQuickSaveDataAvailable = false;
            Commands.CommandState = "ユニット選択";
            Commands.SelectedPartners = new Unit[1];

            // フォント設定をデフォルトに戻す
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock1 = GUI.MainForm.picMain(0);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock1.ForeColor = Information.RGB(255, 255, 255);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                if (withBlock1.Font.Name != "ＭＳ Ｐ明朝")
                {
                    sf = (Font)Control.DefaultFont.Clone();
                    sf = VB.Compatibility.VB6.Support.FontChangeName(sf, "ＭＳ Ｐ明朝");
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock1.Font = sf;
                }
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock1.Font.Size = 16;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock1.Font.Bold = true;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock1.Font.Italic = false;
                GUI.PermanentStringMode = false;
                GUI.KeepStringMode = false;
            }

            // 描画の基準座標位置をリセット
            Event_Renamed.ResetBasePoint();

            // メモリを消費し過ぎないようにユニット画像をクリア
            if (!IsSubStage)
            {
                UList.ClearUnitBitmap();
            }

            GUI.LockGUI();
            if (Map.MapWidth == 1)
            {
                Map.SetMapSize(15, 15);
            }

            // プロローグ
            Stage = "プロローグ";
            string arglname = "プロローグ";
            if (!IsSubStage & Event_Renamed.IsEventDefined(ref arglname, true))
            {
                Sound.StopBGM();
                string argbgm_name = "Briefing";
                string argbgm_name1 = Sound.BGMName(ref argbgm_name);
                Sound.StartBGM(ref argbgm_name1);
            }

            Event_Renamed.HandleEvent("プロローグ");
            if (IsScenarioFinished)
            {
                IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            string arglname1 = "スタート";
            if (!Event_Renamed.IsEventDefined(ref arglname1))
            {
                string argmsg6 = "スタートイベントが定義されていません";
                GUI.ErrorMessage(ref argmsg6);
                TerminateSRC();
            }

            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            Stage = "味方";
            Sound.StopBGM();

            // リスタート用にデータをセーブ
            if (Strings.InStr(fname, @"\Lib\ユニットステータス表示.eve") == 0 & Strings.InStr(fname, @"\Lib\パイロットステータス表示.eve") == 0)
            {
                string argfname = ScenarioPath + "_リスタート.src";
                DumpData(ref argfname);
            }

            // スタートイベントが始まった場合は通常のステージとみなす
            IsSubStage = false;
            Status.ClearUnitStatus();
            if (!GUI.MainForm.Visible)
            {
                GUI.MainForm.Show();
                GUI.MainForm.Refresh();
            }

            GUI.RedrawScreen();

            // スタートイベント
            Event_Renamed.HandleEvent("スタート");
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
            string arguparty = "味方";
            StartTurn(ref arguparty);
        }

        // 陣営upartyのフェイズを実行
        public static void StartTurn(ref string uparty)
        {
            short num, i, phase;
            Unit u;
            Stage = uparty;
            Sound.BossBGM = false;
            if (uparty == "味方")
            {
                do
                {
                    // 味方フェイズ
                    Stage = "味方";

                    // ターン数を進める
                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        Turn = (short)(Turn + 1);
                        TotalTurn = TotalTurn + 1;
                    }

                    // 状態回復
                    foreach (Unit currentSelectedUnit in UList)
                    {
                        Commands.SelectedUnit = currentSelectedUnit;
                        {
                            var withBlock = Commands.SelectedUnit;
                            switch (withBlock.Status)
                            {
                                case "出撃":
                                case "格納":
                                    {
                                        if ((withBlock.Party ?? "") == (uparty ?? ""))
                                        {
                                            if (string.IsNullOrEmpty(Map.MapFileName))
                                            {
                                                withBlock.UsedAction = 0;
                                            }
                                            else
                                            {
                                                withBlock.Rest();
                                            }

                                            if (IsScenarioFinished)
                                            {
                                                GUI.UnlockGUI();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            withBlock.UsedAction = 0;
                                        }

                                        break;
                                    }

                                case "旧主形態":
                                case "旧形態":
                                    {
                                        withBlock.UsedAction = 0;
                                        break;
                                    }
                            }
                        }
                    }

                    // 味方が敵にかけたスペシャルパワーを解除
                    foreach (Unit currentU in UList)
                    {
                        u = currentU;
                        {
                            var withBlock1 = u;
                            switch (withBlock1.Status)
                            {
                                case "出撃":
                                case "格納":
                                    {
                                        string argstype = "敵ターン";
                                        withBlock1.RemoveSpecialPowerInEffect(ref argstype);
                                        break;
                                    }
                            }
                        }
                    }

                    GUI.RedrawScreen();

                    // 味方フェイズ用ＢＧＭを演奏
                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        switch (Map.TerrainClass(1, 1) ?? "")
                        {
                            case "屋内":
                                {
                                    string argbgm_name = "Map3";
                                    string argbgm_name1 = Sound.BGMName(ref argbgm_name);
                                    Sound.StartBGM(ref argbgm_name1);
                                    break;
                                }

                            case "宇宙":
                                {
                                    string argbgm_name2 = "Map5";
                                    string argbgm_name3 = Sound.BGMName(ref argbgm_name2);
                                    Sound.StartBGM(ref argbgm_name3);
                                    break;
                                }

                            default:
                                {
                                    if (Map.TerrainName(1, 1) == "壁")
                                    {
                                        string argbgm_name4 = "Map3";
                                        string argbgm_name5 = Sound.BGMName(ref argbgm_name4);
                                        Sound.StartBGM(ref argbgm_name5);
                                    }
                                    else
                                    {
                                        string argbgm_name6 = "Map1";
                                        string argbgm_name7 = Sound.BGMName(ref argbgm_name6);
                                        Sound.StartBGM(ref argbgm_name7);
                                    }

                                    break;
                                }
                        }
                    }

                    // ターンイベント
                    Event_Renamed.IsUnitCenter = false;
                    Event_Renamed.HandleEvent("ターン", Turn, "味方");
                    if (IsScenarioFinished)
                    {
                        GUI.UnlockGUI();
                        return;
                    }

                    // 操作可能なユニットがいるかどうかチェック
                    num = 0;
                    foreach (Unit currentU1 in UList)
                    {
                        u = currentU1;
                        {
                            var withBlock2 = u;
                            if (withBlock2.Party == "味方" & (withBlock2.Status == "出撃" | withBlock2.Status == "格納") & withBlock2.Action > 0)
                            {
                                num = (short)(num + 1);
                            }
                        }
                    }

                    string argoname = "味方フェイズ強制発動";
                    if (num > 0 | Expression.IsOptionDefined(ref argoname))
                    {
                        break;
                    }

                    // CPUが操作するユニットがいるかどうかチェック
                    num = 0;
                    foreach (Unit currentU2 in UList)
                    {
                        u = currentU2;
                        {
                            var withBlock3 = u;
                            if (withBlock3.Party != "味方" & withBlock3.Status == "出撃")
                            {
                                num = (short)(num + 1);
                            }
                        }
                    }

                    if (num == 0)
                    {
                        break;
                    }

                    // 敵フェイズ
                    string arguparty = "敵";
                    StartTurn(ref arguparty);
                    if (IsScenarioFinished)
                    {
                        IsScenarioFinished = false;
                        return;
                    }

                    // 中立フェイズ
                    string arguparty1 = "中立";
                    StartTurn(ref arguparty1);
                    if (IsScenarioFinished)
                    {
                        IsScenarioFinished = false;
                        return;
                    }

                    // ＮＰＣフェイズ
                    string arguparty2 = "ＮＰＣ";
                    StartTurn(ref arguparty2);
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
                foreach (Unit currentSelectedUnit1 in UList)
                {
                    Commands.SelectedUnit = currentSelectedUnit1;
                    {
                        var withBlock4 = Commands.SelectedUnit;
                        switch (withBlock4.Status)
                        {
                            case "出撃":
                            case "格納":
                                {
                                    if ((withBlock4.Party ?? "") == (uparty ?? ""))
                                    {
                                        withBlock4.Rest();
                                        if (IsScenarioFinished)
                                        {
                                            GUI.UnlockGUI();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        withBlock4.UsedAction = 0;
                                    }

                                    break;
                                }

                            case "旧主形態":
                            case "旧形態":
                                {
                                    withBlock4.UsedAction = 0;
                                    break;
                                }
                        }
                    }
                }

                // 敵ユニットが味方にかけたスペシャルパワーを解除
                foreach (Unit currentU3 in UList)
                {
                    u = currentU3;
                    {
                        var withBlock5 = u;
                        switch (withBlock5.Status)
                        {
                            case "出撃":
                            case "格納":
                                {
                                    string argstype1 = "敵ターン";
                                    withBlock5.RemoveSpecialPowerInEffect(ref argstype1);
                                    break;
                                }
                        }
                    }
                }

                GUI.RedrawScreen();

                // 敵(ＮＰＣ)フェイズ用ＢＧＭを演奏
                switch (Map.TerrainClass(1, 1) ?? "")
                {
                    case "屋内":
                        {
                            if (Stage == "ＮＰＣ")
                            {
                                string argbgm_name8 = "Map3";
                                string argbgm_name9 = Sound.BGMName(ref argbgm_name8);
                                Sound.StartBGM(ref argbgm_name9);
                            }
                            else
                            {
                                string argbgm_name10 = "Map4";
                                string argbgm_name11 = Sound.BGMName(ref argbgm_name10);
                                Sound.StartBGM(ref argbgm_name11);
                            }

                            break;
                        }

                    case "宇宙":
                        {
                            if (Stage == "ＮＰＣ")
                            {
                                string argbgm_name12 = "Map5";
                                string argbgm_name13 = Sound.BGMName(ref argbgm_name12);
                                Sound.StartBGM(ref argbgm_name13);
                            }
                            else
                            {
                                string argbgm_name14 = "Map6";
                                string argbgm_name15 = Sound.BGMName(ref argbgm_name14);
                                Sound.StartBGM(ref argbgm_name15);
                            }

                            break;
                        }

                    default:
                        {
                            if (Stage == "ＮＰＣ")
                            {
                                if (Map.TerrainName(1, 1) == "壁")
                                {
                                    string argbgm_name16 = "Map3";
                                    string argbgm_name17 = Sound.BGMName(ref argbgm_name16);
                                    Sound.StartBGM(ref argbgm_name17);
                                }
                                else
                                {
                                    string argbgm_name18 = "Map1";
                                    string argbgm_name19 = Sound.BGMName(ref argbgm_name18);
                                    Sound.StartBGM(ref argbgm_name19);
                                }
                            }
                            else if (Map.TerrainName(1, 1) == "壁")
                            {
                                string argbgm_name20 = "Map4";
                                string argbgm_name21 = Sound.BGMName(ref argbgm_name20);
                                Sound.StartBGM(ref argbgm_name21);
                            }
                            else
                            {
                                string argbgm_name22 = "Map2";
                                string argbgm_name23 = Sound.BGMName(ref argbgm_name22);
                                Sound.StartBGM(ref argbgm_name23);
                            }

                            break;
                        }
                }

                // ターンイベント
                Event_Renamed.HandleEvent("ターン", Turn, uparty);
                if (IsScenarioFinished)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            short max_lv;
            var max_unit = default(Unit);
            if (uparty == "味方")
            {
                // 味方フェイズのプレイヤーによるユニット操作前の処理

                // ターン数を表示
                string argoname1 = "デバッグ";
                if (Turn > 1 & Expression.IsOptionDefined(ref argoname1))
                {
                    string argmsg = "ターン" + VB.Compatibility.VB6.Support.Format(Turn);
                    GUI.DisplayTelop(ref argmsg);
                }

                // 通常のステージでは母艦ユニットまたはレベルがもっとも高い
                // ユニットを中央に配置
                if (!string.IsNullOrEmpty(Map.MapFileName) & !Event_Renamed.IsUnitCenter)
                {
                    foreach (Unit currentU4 in UList)
                    {
                        u = currentU4;
                        {
                            var withBlock6 = u;
                            if (withBlock6.Party == "味方" & withBlock6.Status == "出撃" & withBlock6.Action > 0)
                            {
                                string argfname = "母艦";
                                if (withBlock6.IsFeatureAvailable(ref argfname))
                                {
                                    GUI.Center(withBlock6.x, withBlock6.y);
                                    Status.DisplayUnitStatus(ref u);
                                    GUI.RedrawScreen();
                                    GUI.UnlockGUI();
                                    return;
                                }
                            }
                        }
                    }

                    max_lv = 0;
                    foreach (Unit currentU5 in UList)
                    {
                        u = currentU5;
                        {
                            var withBlock7 = u;
                            if (withBlock7.Party == "味方" & withBlock7.Status == "出撃")
                            {
                                if (withBlock7.MainPilot().Level > max_lv)
                                {
                                    max_unit = u;
                                    max_lv = withBlock7.MainPilot().Level;
                                }
                            }
                        }
                    }

                    if (max_unit is object)
                    {
                        GUI.Center(max_unit.x, max_unit.y);
                    }
                }

                // ステータスを表示
                if (!string.IsNullOrEmpty(Map.MapFileName))
                {
                    Status.DisplayGlobalStatus();
                }

                // プレイヤーによる味方ユニット操作に移行
                GUI.RedrawScreen();
                Application.DoEvents();
                GUI.UnlockGUI();
                return;
            }

            GUI.LockGUI();

            // CPUによるユニット操作
            for (phase = 1; phase <= 5; phase++)
            {
                var loopTo = UList.Count();
                for (i = 1; i <= loopTo; i++)
                {
                    // フェイズ中に行動するユニットを選択
                    object argIndex1 = i;
                    Commands.SelectedUnit = UList.Item(ref argIndex1);
                    {
                        var withBlock8 = Commands.SelectedUnit;
                        if (withBlock8.Status != "出撃")
                        {
                            goto NextLoop;
                        }

                        if (withBlock8.Action == 0)
                        {
                            goto NextLoop;
                        }

                        if ((withBlock8.Party ?? "") != (uparty ?? ""))
                        {
                            goto NextLoop;
                        }

                        u = Commands.SelectedUnit;

                        // 他のユニットを護衛しているユニットは護衛対象と同じ順に行動
                        bool localIsDefined() { object argIndex1 = withBlock8.Mode; var ret = PList.IsDefined(ref argIndex1); withBlock8.Mode = Conversions.ToString(argIndex1); return ret; }

                        if (localIsDefined())
                        {
                            object argIndex2 = withBlock8.Mode;
                            {
                                var withBlock9 = PList.Item(ref argIndex2);
                                if (withBlock9.Unit is object)
                                {
                                    if (withBlock9.Unit.Party == uparty)
                                    {
                                        u = withBlock9.Unit;
                                    }
                                }
                            }

                            withBlock8.Mode = Conversions.ToString(argIndex2);
                        }

                        bool localIsDefined1() { object argIndex1 = u.Mode; var ret = PList.IsDefined(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                        if (localIsDefined1())
                        {
                            object argIndex3 = u.Mode;
                            {
                                var withBlock10 = PList.Item(ref argIndex3);
                                if (withBlock10.Unit is object)
                                {
                                    if (withBlock10.Unit.Party == uparty)
                                    {
                                        u = withBlock10.Unit;
                                    }
                                }
                            }

                            u.Mode = Conversions.ToString(argIndex3);
                        }

                        switch (phase)
                        {
                            case 1:
                                {
                                    // 最初にサポート能力を持たないザコユニットが行動
                                    if (u.BossRank >= 0)
                                    {
                                        goto NextLoop;
                                    }

                                    {
                                        var withBlock11 = u.MainPilot();
                                        string argsname = "援護";
                                        string argsname1 = "援護攻撃";
                                        string argsname2 = "援護防御";
                                        string argsname3 = "統率";
                                        string argsname4 = "指揮";
                                        string argsname5 = "広域サポート";
                                        if (withBlock11.IsSkillAvailable(ref argsname) | withBlock11.IsSkillAvailable(ref argsname1) | withBlock11.IsSkillAvailable(ref argsname2) | withBlock11.IsSkillAvailable(ref argsname3) | withBlock11.IsSkillAvailable(ref argsname4) | withBlock11.IsSkillAvailable(ref argsname5))
                                        {
                                            goto NextLoop;
                                        }
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    // 次にサポート能力を持たないボスユニットが行動
                                    {
                                        var withBlock12 = u.MainPilot();
                                        string argsname6 = "援護";
                                        string argsname7 = "援護攻撃";
                                        string argsname8 = "援護防御";
                                        string argsname9 = "統率";
                                        string argsname10 = "指揮";
                                        string argsname11 = "広域サポート";
                                        if (withBlock12.IsSkillAvailable(ref argsname6) | withBlock12.IsSkillAvailable(ref argsname7) | withBlock12.IsSkillAvailable(ref argsname8) | withBlock12.IsSkillAvailable(ref argsname9) | withBlock12.IsSkillAvailable(ref argsname10) | withBlock12.IsSkillAvailable(ref argsname11))
                                        {
                                            goto NextLoop;
                                        }
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    // 次に統率能力を持つユニットが行動
                                    string argsname12 = "統率";
                                    if (!u.MainPilot().IsSkillAvailable(ref argsname12))
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    // 次にサポート能力を持つザコユニットが行動
                                    if (u.BossRank >= 0)
                                    {
                                        goto NextLoop;
                                    }

                                    break;
                                }

                            case 5:
                                {
                                    break;
                                }
                                // 最後にサポート能力を持つボスユニットが行動
                        }
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
                            Status.DisplayUnitStatus(ref Commands.SelectedUnit);
                            GUI.Center(Commands.SelectedUnit.x, Commands.SelectedUnit.y);
                            GUI.RedrawScreen();
                            Application.DoEvents();
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
                            if (withBlock13.Status == "出撃" & withBlock13.x > 1)
                            {
                                if (Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock13.x - 1, withBlock13.y];
                                    Event_Renamed.HandleEvent("接触", withBlock13.MainPilot().ID, Map.MapDataForUnit[(int)withBlock13.x - 1, withBlock13.y].MainPilot.ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock14 = Commands.SelectedUnit;
                            if (withBlock14.Status == "出撃" & withBlock14.x < Map.MapWidth)
                            {
                                if (Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
                                    Event_Renamed.HandleEvent("接触", withBlock14.MainPilot().ID, Map.MapDataForUnit[(int)withBlock14.x + 1, withBlock14.y].MainPilot.ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock15 = Commands.SelectedUnit;
                            if (withBlock15.Status == "出撃" & withBlock15.y > 1)
                            {
                                if (Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock15.x, withBlock15.y - 1];
                                    Event_Renamed.HandleEvent("接触", withBlock15.MainPilot().ID, Map.MapDataForUnit[withBlock15.x, (int)withBlock15.y - 1].MainPilot.ID);
                                    if (IsScenarioFinished)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        {
                            var withBlock16 = Commands.SelectedUnit;
                            if (withBlock16.Status == "出撃" & withBlock16.y < Map.MapHeight)
                            {
                                if (Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1] is object)
                                {
                                    Commands.SelectedTarget = Map.MapDataForUnit[withBlock16.x, withBlock16.y + 1];
                                    Event_Renamed.HandleEvent("接触", withBlock16.MainPilot().ID, Map.MapDataForUnit[withBlock16.x, (int)withBlock16.y + 1].MainPilot.ID);
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
                                Event_Renamed.HandleEvent("進入", withBlock17.MainPilot().ID, withBlock17.x, withBlock17.y);
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
                                Event_Renamed.HandleEvent("行動終了", withBlock18.MainPilot().ID);
                                if (IsScenarioFinished)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    NextLoop:
                    ;
                }
            }

            // ステータスウィンドウの表示を消去
            Status.ClearUnitStatus();
        }

        // ゲームオーバー
        public static void GameOver()
        {
            var fname = default(string);
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StopBGM();
            GUI.MainForm.Hide();

            // GameOver.eveを探す
            bool localFileExists() { string argfname = AppPath + @"Data\System\GameOver.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname4 = ScenarioPath + @"Data\System\GameOver.eve";
            if (GeneralLib.FileExists(ref argfname4))
            {
                fname = ScenarioPath + @"Data\System\GameOver.eve";
                string argfname1 = ScenarioPath + @"Data\System\non_pilot.txt";
                if (GeneralLib.FileExists(ref argfname1))
                {
                    string argfname = ScenarioPath + @"Data\System\non_pilot.txt";
                    NPDList.Load(ref argfname);
                }
            }
            else if (localFileExists())
            {
                fname = AppPath + @"Data\System\GameOver.eve";
                string argfname3 = AppPath + @"Data\System\non_pilot.txt";
                if (GeneralLib.FileExists(ref argfname3))
                {
                    string argfname2 = AppPath + @"Data\System\non_pilot.txt";
                    NPDList.Load(ref argfname2);
                }
            }
            else
            {
                // GameOver.eveが無ければそのまま終了
                TerminateSRC();
            }

            // GameOver.eveを読み込み
            Event_Renamed.ClearEventData();
            string argload_mode = "";
            Event_Renamed.LoadEventData(ref fname, load_mode: ref argload_mode);
            ScenarioFileName = fname;
            string arglname = "プロローグ";
            if (!Event_Renamed.IsEventDefined(ref arglname))
            {
                string argmsg = fname + "中にプロローグイベントが定義されていません";
                GUI.ErrorMessage(ref argmsg);
                TerminateSRC();
            }

            // GameOver.eveのプロローグイベントを実施
            Event_Renamed.HandleEvent("プロローグ");
        }

        // ゲームクリア
        public static void GameClear()
        {
            TerminateSRC();
        }

        // ゲームを途中終了
        public static void ExitGame()
        {
            var fname = default(string);
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StopBGM();

            // Exit.eveを探す
            GUI.MainForm.Hide();
            bool localFileExists() { string argfname = AppPath + @"Data\System\Exit.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname4 = ScenarioPath + @"Data\System\Exit.eve";
            if (GeneralLib.FileExists(ref argfname4))
            {
                fname = ScenarioPath + @"Data\System\Exit.eve";
                string argfname1 = ScenarioPath + @"Data\System\non_pilot.txt";
                if (GeneralLib.FileExists(ref argfname1))
                {
                    string argfname = ScenarioPath + @"Data\System\non_pilot.txt";
                    NPDList.Load(ref argfname);
                }
            }
            else if (localFileExists())
            {
                fname = AppPath + @"Data\System\Exit.eve";
                string argfname3 = AppPath + @"Data\System\non_pilot.txt";
                if (GeneralLib.FileExists(ref argfname3))
                {
                    string argfname2 = AppPath + @"Data\System\non_pilot.txt";
                    NPDList.Load(ref argfname2);
                }
            }
            else
            {
                // Exit.eveが無ければそのまま終了
                TerminateSRC();
            }

            // Exit.eveを読み込み
            Event_Renamed.ClearEventData();
            string argload_mode = "";
            Event_Renamed.LoadEventData(ref fname, load_mode: ref argload_mode);
            string arglname = "プロローグ";
            if (!Event_Renamed.IsEventDefined(ref arglname))
            {
                string argmsg = fname + "中にプロローグイベントが定義されていません";
                GUI.ErrorMessage(ref argmsg);
                TerminateSRC();
            }

            // Exit.eveのプロローグイベントを実施
            Event_Renamed.HandleEvent("プロローグ");

            // SRCを終了
            TerminateSRC();
        }

        // SRCを終了
        public static void TerminateSRC()
        {
            short i, j;

            // ウィンドウを閉じる
            if (GUI.MainForm is object)
            {
                GUI.MainForm.Hide();
            }
            // UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            Load(My.MyProject.Forms.frmMessage);
            if (My.MyProject.Forms.frmMessage.Visible)
            {
                GUI.CloseMessageForm();
            }
            // UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            Load(My.MyProject.Forms.frmListBox);
            if (My.MyProject.Forms.frmListBox.Visible)
            {
                My.MyProject.Forms.frmListBox.Hide();
            }
            // UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            Load(My.MyProject.Forms.frmNowLoading);
            if (My.MyProject.Forms.frmNowLoading.Visible)
            {
                My.MyProject.Forms.frmNowLoading.Hide();
            }

            Application.DoEvents();

            // 時間解像度を元に戻す
            GeneralLib.timeEndPeriod(1);

            // フルスクリーンモードを使っていた場合は解像度を元に戻す
            string argini_section = "Option";
            string argini_entry = "FullScreen";
            if (GeneralLib.ReadIni(ref argini_section, ref argini_entry) == "On")
            {
                GUI.ChangeDisplaySize(0, 0);
            }

            // ＢＧＭ・効果音の再生を停止
            Sound.FreeSoundModule();

            // 各種データを解放

            // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedUnit = null;
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedTarget = null;
            // UPGRADE_NOTE: オブジェクト SelectedPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedPilot = null;
            // UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Status.DisplayedUnit = null;
            var loopTo = Map.MapWidth;
            for (i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (j = 1; j <= loopTo1; j++)
                    // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    Map.MapDataForUnit[i, j] = null;
            }

            {
                var withBlock = Event_Renamed.GlobalVariableList;
                var loopTo2 = (short)withBlock.Count;
                for (i = 1; i <= loopTo2; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト GlobalVariableList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.GlobalVariableList = null;
            {
                var withBlock1 = Event_Renamed.LocalVariableList;
                var loopTo3 = (short)withBlock1.Count;
                for (i = 1; i <= loopTo3; i++)
                    withBlock1.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト LocalVariableList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.LocalVariableList = null;

            // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.SelectedUnitForEvent = null;
            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.SelectedTargetForEvent = null;

            // UPGRADE_NOTE: オブジェクト UList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            UList = null;
            // UPGRADE_NOTE: オブジェクト PList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            PList = null;
            // UPGRADE_NOTE: オブジェクト IList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            IList = null;

            // なぜかこれがないと不正終了する……
            Application.DoEvents();

            // UPGRADE_NOTE: オブジェクト PDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            PDList = null;
            // UPGRADE_NOTE: オブジェクト NPDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            NPDList = null;
            // UPGRADE_NOTE: オブジェクト UDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            UDList = null;
            // UPGRADE_NOTE: オブジェクト IDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            IDList = null;
            // UPGRADE_NOTE: オブジェクト MDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            MDList = null;
            // UPGRADE_NOTE: オブジェクト EDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            EDList = null;
            // UPGRADE_NOTE: オブジェクト ADList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ADList = null;
            // UPGRADE_NOTE: オブジェクト EADList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            EADList = null;
            // UPGRADE_NOTE: オブジェクト DDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            DDList = null;
            // UPGRADE_NOTE: オブジェクト SPDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SPDList = null;
            // UPGRADE_NOTE: オブジェクト ALDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ALDList = null;
            // UPGRADE_NOTE: オブジェクト TDList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            TDList = null;
            // UPGRADE_NOTE: オブジェクト BCList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            BCList = null;
            Environment.Exit(0);
        }


        // データをセーブ
        public static void SaveData(ref string fname)
        {
            short i;
            int num;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 93004


            Input:

                    On Error GoTo ErrorHandler

             */
            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write);

            // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            {
                var withBlock = App;
                num = 10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision;
            }

            FileSystem.WriteLine(SaveDataFileNumber, (object)num);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Information.UBound(Titles));
            var loopTo = (short)Information.UBound(Titles);
            for (i = 1; i <= loopTo; i++)
                FileSystem.WriteLine(SaveDataFileNumber, Titles[i]);
            string argexpr = "次ステージ";
            FileSystem.WriteLine(SaveDataFileNumber, Expression.GetValueAsString(ref argexpr));
            FileSystem.WriteLine(SaveDataFileNumber, (object)TotalTurn);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Money);
            FileSystem.WriteLine(SaveDataFileNumber, (object)0); // パーツ用のダミー
            Event_Renamed.SaveGlobalVariables();
            PList.Save();
            UList.Save();
            IList.Save();
            FileSystem.FileClose((int)SaveDataFileNumber);
            return;
            ErrorHandler:
            ;
            string argmsg = "セーブ中にエラーが発生しました";
            GUI.ErrorMessage(ref argmsg);
            FileSystem.FileClose((int)SaveDataFileNumber);
        }

        // データをロード
        public static void LoadData(ref string fname)
        {
            short i, num = default;
            var fname2 = default(string);
            var dummy = default(string);
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 95360


            Input:

                    On Error GoTo ErrorHandler

             */
            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
            FileSystem.Input(SaveDataFileNumber, ref SaveDataVersion);
            if (SaveDataVersion > 10000)
            {
                FileSystem.Input(SaveDataFileNumber, ref num);
            }
            else
            {
                num = (short)SaveDataVersion;
            }

            GUI.SetLoadImageSize((short)(num * 2 + 5));
            Titles = new string[(num + 1)];
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                FileSystem.Input(SaveDataFileNumber, ref Titles[i]);
                IncludeData(ref Titles[i]);
            }

            string argfname1 = ScenarioPath + @"Data\alias.txt";
            if (GeneralLib.FileExists(ref argfname1))
            {
                string argfname = ScenarioPath + @"Data\alias.txt";
                ALDList.Load(ref argfname);
            }

            bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname4 = ScenarioPath + @"Data\sp.txt";
            if (GeneralLib.FileExists(ref argfname4))
            {
                string argfname2 = ScenarioPath + @"Data\sp.txt";
                SPDList.Load(ref argfname2);
            }
            else if (localFileExists())
            {
                string argfname3 = ScenarioPath + @"Data\mind.txt";
                SPDList.Load(ref argfname3);
            }

            string argfname6 = ScenarioPath + @"Data\pilot.txt";
            if (GeneralLib.FileExists(ref argfname6))
            {
                string argfname5 = ScenarioPath + @"Data\pilot.txt";
                PDList.Load(ref argfname5);
            }

            string argfname8 = ScenarioPath + @"Data\non_pilot.txt";
            if (GeneralLib.FileExists(ref argfname8))
            {
                string argfname7 = ScenarioPath + @"Data\non_pilot.txt";
                NPDList.Load(ref argfname7);
            }

            string argfname10 = ScenarioPath + @"Data\robot.txt";
            if (GeneralLib.FileExists(ref argfname10))
            {
                string argfname9 = ScenarioPath + @"Data\robot.txt";
                UDList.Load(ref argfname9);
            }

            string argfname12 = ScenarioPath + @"Data\unit.txt";
            if (GeneralLib.FileExists(ref argfname12))
            {
                string argfname11 = ScenarioPath + @"Data\unit.txt";
                UDList.Load(ref argfname11);
            }

            GUI.DisplayLoadingProgress();
            string argfname14 = ScenarioPath + @"Data\pilot_message.txt";
            if (GeneralLib.FileExists(ref argfname14))
            {
                string argfname13 = ScenarioPath + @"Data\pilot_message.txt";
                MDList.Load(ref argfname13);
            }

            string argfname16 = ScenarioPath + @"Data\pilot_dialog.txt";
            if (GeneralLib.FileExists(ref argfname16))
            {
                string argfname15 = ScenarioPath + @"Data\pilot_dialog.txt";
                DDList.Load(ref argfname15);
            }

            string argfname18 = ScenarioPath + @"Data\effect.txt";
            if (GeneralLib.FileExists(ref argfname18))
            {
                string argfname17 = ScenarioPath + @"Data\effect.txt";
                EDList.Load(ref argfname17);
            }

            string argfname20 = ScenarioPath + @"Data\animation.txt";
            if (GeneralLib.FileExists(ref argfname20))
            {
                string argfname19 = ScenarioPath + @"Data\animation.txt";
                ADList.Load(ref argfname19);
            }

            string argfname22 = ScenarioPath + @"Data\ext_animation.txt";
            if (GeneralLib.FileExists(ref argfname22))
            {
                string argfname21 = ScenarioPath + @"Data\ext_animation.txt";
                EADList.Load(ref argfname21);
            }

            string argfname24 = ScenarioPath + @"Data\item.txt";
            if (GeneralLib.FileExists(ref argfname24))
            {
                string argfname23 = ScenarioPath + @"Data\item.txt";
                IDList.Load(ref argfname23);
            }

            GUI.DisplayLoadingProgress();
            IsLocalDataLoaded = true;
            FileSystem.Input(SaveDataFileNumber, ref fname2);
            FileSystem.Input(SaveDataFileNumber, ref TotalTurn);
            FileSystem.Input(SaveDataFileNumber, ref Money);
            FileSystem.Input(SaveDataFileNumber, ref num); // パーツ用のダミー
            Event_Renamed.LoadGlobalVariables();
            string argvname1 = "次ステージ";
            if (!Expression.IsGlobalVariableDefined(ref argvname1))
            {
                string argvname = "次ステージ";
                Expression.DefineGlobalVariable(ref argvname);
            }

            string argvname2 = "次ステージ";
            Expression.SetVariableAsString(ref argvname2, ref fname2);
            PList.Load();
            UList.Load();
            IList.Load();
            FileSystem.FileClose((int)SaveDataFileNumber);

            // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み

            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
            if (SaveDataVersion > 10000)
            {
                FileSystem.Input(SaveDataFileNumber, ref dummy);
            }

            FileSystem.Input(SaveDataFileNumber, ref num);
            Titles = new string[(num + 1)];
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
                FileSystem.Input(SaveDataFileNumber, ref Titles[i]);
            FileSystem.Input(SaveDataFileNumber, ref dummy);
            FileSystem.Input(SaveDataFileNumber, ref TotalTurn);
            FileSystem.Input(SaveDataFileNumber, ref Money);
            FileSystem.Input(SaveDataFileNumber, ref num); // パーツ用のダミー
            FileSystem.Input(SaveDataFileNumber, ref num); // パーツ用のダミー
            var loopTo2 = num;
            for (i = 1; i <= loopTo2; i++)
                dummy = FileSystem.LineInput(SaveDataFileNumber);
            PList.LoadLinkInfo();
            UList.LoadLinkInfo();
            IList.LoadLinkInfo();
            FileSystem.FileClose((int)SaveDataFileNumber);
            GUI.DisplayLoadingProgress();

            // ユニットの状態を回復
            foreach (Unit u in UList)
                u.Reset_Renamed();
            GUI.DisplayLoadingProgress();

            // 追加されたシステム側イベントデータの読み込み
            string argfname25 = "";
            string argload_mode = "";
            Event_Renamed.LoadEventData(ref argfname25, load_mode: ref argload_mode);
            GUI.DisplayLoadingProgress();
            return;
            ErrorHandler:
            ;
            string argmsg = "ロード中にエラーが発生しました";
            GUI.ErrorMessage(ref argmsg);
            FileSystem.FileClose((int)SaveDataFileNumber);
            TerminateSRC();
        }


        // 一時中断用データをファイルにセーブする
        public static void DumpData(ref string fname)
        {
            short i;
            int num;
            ;

            // 中断データをセーブ
            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write);

            // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            {
                var withBlock = App;
                num = 10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision;
            }

            FileSystem.WriteLine(SaveDataFileNumber, (object)num);
            FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(ScenarioFileName, Strings.Len(ScenarioPath) + 1));
            FileSystem.WriteLine(SaveDataFileNumber, (object)Information.UBound(Titles));
            var loopTo = (short)Information.UBound(Titles);
            for (i = 1; i <= loopTo; i++)
                FileSystem.WriteLine(SaveDataFileNumber, Titles[i]);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Turn);
            FileSystem.WriteLine(SaveDataFileNumber, (object)TotalTurn);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Money);
            Event_Renamed.DumpEventData();
            PList.Dump();
            IList.Dump();
            UList.Dump();
            Map.DumpMapData();

            // Midi じゃなくて midi じゃないと検索失敗するようになってるので。
            if (Strings.InStr(Strings.LCase(Sound.BGMFileName), @"\midi\") > 0)
            {
                FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(Sound.BGMFileName, Strings.InStr(Strings.LCase(Sound.BGMFileName), @"\midi\") + 6));
            }
            else if (Strings.InStr(Sound.BGMFileName, @"\") > 0)
            {
                FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(Sound.BGMFileName, Strings.InStr(Sound.BGMFileName, @"\") + 1));
            }
            else
            {
                FileSystem.WriteLine(SaveDataFileNumber, Sound.BGMFileName);
            }

            FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.RepeatMode);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.KeepBGM);
            FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.BossBGM);
            FileSystem.WriteLine(SaveDataFileNumber, (object)GeneralLib.RndSeed);
            FileSystem.WriteLine(SaveDataFileNumber, (object)GeneralLib.RndIndex);
            FileSystem.FileClose((int)SaveDataFileNumber);
            LastSaveDataFileName = fname;
            if (Strings.InStr(fname, @"\_リスタート.src") > 0)
            {
                IsRestartSaveDataAvailable = true;
            }
            else if (Strings.InStr(fname, @"\_クイックセーブ.src") > 0)
            {
                IsQuickSaveDataAvailable = true;
            }

            return;
            ErrorHandler:
            ;
            string argmsg = "セーブ中にエラーが発生しました";
            GUI.ErrorMessage(ref argmsg);
            FileSystem.FileClose((int)SaveDataFileNumber);
        }

        // 一時中断用データをロード
        public static void RestoreData(ref string fname, ref bool quick_load)
        {
            short i, num = default;
            var fname2 = default(string);
            string dummy;
            var scenario_file_is_different = default(bool);
            ;

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;
            if (quick_load)
            {
                string argoname = "デバッグ";
                if (Expression.IsOptionDefined(ref argoname))
                {
                    string argload_mode = "クイックロード";
                    Event_Renamed.LoadEventData(ref ScenarioFileName, ref argload_mode);
                }
            }

            if (!quick_load)
            {
                GUI.OpenNowLoadingForm();
            }

            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
            FileSystem.Input(SaveDataFileNumber, ref fname2);
            if (Information.IsNumeric(fname2))
            {
                SaveDataVersion = Conversions.ToInteger(fname2);
                FileSystem.Input(SaveDataFileNumber, ref fname2);
            }
            else
            {
                SaveDataVersion = 1;
            }

            // ウィンドウのタイトルを設定
            if ((ScenarioFileName ?? "") != (ScenarioPath + fname2 ?? ""))
            {
                GUI.MainForm.Text = "SRC - " + Strings.Left(fname2, Strings.Len(fname2) - 4);
                ScenarioFileName = ScenarioPath + fname2;
                scenario_file_is_different = true;
            }

            FileSystem.Input(SaveDataFileNumber, ref num);

            // 使用するデータをロード
            if (!quick_load)
            {
                GUI.SetLoadImageSize((short)(num * 2 + 5));
                Titles = new string[(num + 1)];
                var loopTo = num;
                for (i = 1; i <= loopTo; i++)
                {
                    FileSystem.Input(SaveDataFileNumber, ref Titles[i]);
                    IncludeData(ref Titles[i]);
                }

                string argfname1 = ScenarioPath + @"Data\alias.txt";
                if (GeneralLib.FileExists(ref argfname1))
                {
                    string argfname = ScenarioPath + @"Data\alias.txt";
                    ALDList.Load(ref argfname);
                }

                bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                string argfname4 = ScenarioPath + @"Data\sp.txt";
                if (GeneralLib.FileExists(ref argfname4))
                {
                    string argfname2 = ScenarioPath + @"Data\sp.txt";
                    SPDList.Load(ref argfname2);
                }
                else if (localFileExists())
                {
                    string argfname3 = ScenarioPath + @"Data\mind.txt";
                    SPDList.Load(ref argfname3);
                }

                string argfname6 = ScenarioPath + @"Data\pilot.txt";
                if (GeneralLib.FileExists(ref argfname6))
                {
                    string argfname5 = ScenarioPath + @"Data\pilot.txt";
                    PDList.Load(ref argfname5);
                }

                string argfname8 = ScenarioPath + @"Data\non_pilot.txt";
                if (GeneralLib.FileExists(ref argfname8))
                {
                    string argfname7 = ScenarioPath + @"Data\non_pilot.txt";
                    NPDList.Load(ref argfname7);
                }

                string argfname10 = ScenarioPath + @"Data\robot.txt";
                if (GeneralLib.FileExists(ref argfname10))
                {
                    string argfname9 = ScenarioPath + @"Data\robot.txt";
                    UDList.Load(ref argfname9);
                }

                string argfname12 = ScenarioPath + @"Data\unit.txt";
                if (GeneralLib.FileExists(ref argfname12))
                {
                    string argfname11 = ScenarioPath + @"Data\unit.txt";
                    UDList.Load(ref argfname11);
                }

                GUI.DisplayLoadingProgress();
                string argfname14 = ScenarioPath + @"Data\pilot_message.txt";
                if (GeneralLib.FileExists(ref argfname14))
                {
                    string argfname13 = ScenarioPath + @"Data\pilot_message.txt";
                    MDList.Load(ref argfname13);
                }

                string argfname16 = ScenarioPath + @"Data\pilot_dialog.txt";
                if (GeneralLib.FileExists(ref argfname16))
                {
                    string argfname15 = ScenarioPath + @"Data\pilot_dialog.txt";
                    DDList.Load(ref argfname15);
                }

                string argfname18 = ScenarioPath + @"Data\effect.txt";
                if (GeneralLib.FileExists(ref argfname18))
                {
                    string argfname17 = ScenarioPath + @"Data\effect.txt";
                    EDList.Load(ref argfname17);
                }

                string argfname20 = ScenarioPath + @"Data\animation.txt";
                if (GeneralLib.FileExists(ref argfname20))
                {
                    string argfname19 = ScenarioPath + @"Data\animation.txt";
                    ADList.Load(ref argfname19);
                }

                string argfname22 = ScenarioPath + @"Data\ext_animation.txt";
                if (GeneralLib.FileExists(ref argfname22))
                {
                    string argfname21 = ScenarioPath + @"Data\ext_animation.txt";
                    EADList.Load(ref argfname21);
                }

                string argfname24 = ScenarioPath + @"Data\item.txt";
                if (GeneralLib.FileExists(ref argfname24))
                {
                    string argfname23 = ScenarioPath + @"Data\item.txt";
                    IDList.Load(ref argfname23);
                }

                GUI.DisplayLoadingProgress();
                IsLocalDataLoaded = true;
                string argload_mode1 = "リストア";
                Event_Renamed.LoadEventData(ref ScenarioFileName, ref argload_mode1);
                GUI.DisplayLoadingProgress();
            }
            else
            {
                var loopTo1 = num;
                for (i = 1; i <= loopTo1; i++)
                    dummy = FileSystem.LineInput(SaveDataFileNumber);
                if (scenario_file_is_different)
                {
                    string argload_mode2 = "リストア";
                    Event_Renamed.LoadEventData(ref ScenarioFileName, ref argload_mode2);
                }
            }

            FileSystem.Input(SaveDataFileNumber, ref Turn);
            FileSystem.Input(SaveDataFileNumber, ref TotalTurn);
            FileSystem.Input(SaveDataFileNumber, ref Money);
            Event_Renamed.RestoreEventData();
            PList.Restore();
            IList.Restore();
            UList.Restore();

            // MOD START 240a
            // RestoreMapData
            // 'ＢＧＭ関連の設定を復元
            // Input #SaveDataFileNumber, fname2
            // マップデータの互換性維持のため、RestoreMapDataでＢＧＭ関連の１行目まで読み込んで戻り値にした
            fname2 = Map.RestoreMapData();
            // MOD  END  240a
            string argmidi_name = "(" + fname2 + ")";
            fname2 = Sound.SearchMidiFile(ref argmidi_name);
            if (!string.IsNullOrEmpty(fname2))
            {
                Sound.KeepBGM = false;
                Sound.BossBGM = false;
                Sound.ChangeBGM(ref fname2);
                FileSystem.Input(SaveDataFileNumber, ref Sound.RepeatMode);
                FileSystem.Input(SaveDataFileNumber, ref Sound.KeepBGM);
                FileSystem.Input(SaveDataFileNumber, ref Sound.BossBGM);
            }
            else
            {
                Sound.StopBGM();
                dummy = FileSystem.LineInput(SaveDataFileNumber);
                dummy = FileSystem.LineInput(SaveDataFileNumber);
                dummy = FileSystem.LineInput(SaveDataFileNumber);
            }

            // 乱数系列を復元
            string argoname1 = "デバッグ";
            string argoname2 = "乱数系列非保存";
            if (!Expression.IsOptionDefined(ref argoname1) & !Expression.IsOptionDefined(ref argoname2) & !FileSystem.EOF(SaveDataFileNumber))
            {
                FileSystem.Input(SaveDataFileNumber, ref GeneralLib.RndSeed);
                GeneralLib.RndReset();
                FileSystem.Input(SaveDataFileNumber, ref GeneralLib.RndIndex);
            }

            if (!quick_load)
            {
                GUI.DisplayLoadingProgress();
            }

            FileSystem.FileClose((int)SaveDataFileNumber);

            // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み

            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);

            // SaveDataVersion
            if (SaveDataVersion > 10000)
            {
                dummy = FileSystem.LineInput(SaveDataFileNumber);
            }

            // ScenarioFileName
            dummy = FileSystem.LineInput(SaveDataFileNumber);

            // 使用するデータ名
            FileSystem.Input(SaveDataFileNumber, ref num);
            var loopTo2 = num;
            for (i = 1; i <= loopTo2; i++)
                dummy = FileSystem.LineInput(SaveDataFileNumber);

            // Turn
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            // TotalTurn
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            // Money
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            Event_Renamed.SkipEventData();
            PList.RestoreLinkInfo();
            IList.RestoreLinkInfo();
            UList.RestoreLinkInfo();
            FileSystem.FileClose((int)SaveDataFileNumber);

            // パラメータ情報を処理するため、セーブファイルを一旦閉じてから再度読み込み。
            // 霊力やＨＰ、ＥＮといったパラメータは最大値が特殊能力で変動するため、
            // 特殊能力の設定が終わってから改めて設定してやる必要がある。

            SaveDataFileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);

            // SaveDataVersion
            if (SaveDataVersion > 10000)
            {
                dummy = FileSystem.LineInput(SaveDataFileNumber);
            }

            // ScenarioFileName
            dummy = FileSystem.LineInput(SaveDataFileNumber);

            // 使用するデータ名
            FileSystem.Input(SaveDataFileNumber, ref num);
            var loopTo3 = num;
            for (i = 1; i <= loopTo3; i++)
                dummy = FileSystem.LineInput(SaveDataFileNumber);

            // Turn
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            // TotalTurn
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            // Money
            dummy = FileSystem.LineInput(SaveDataFileNumber);
            Event_Renamed.SkipEventData();
            PList.RestoreParameter();
            IList.RestoreParameter();
            UList.RestoreParameter();
            PList.UpdateSupportMod();

            // 背景書き換え
            short map_x, map_y;
            if (Map.IsMapDirty)
            {
                map_x = GUI.MapX;
                map_y = GUI.MapY;
                string argdraw_option = "非同期";
                int argfilter_color = 0;
                double argfilter_trans_par = 0d;
                GUI.SetupBackground(ref Map.MapDrawMode, ref argdraw_option, filter_color: ref argfilter_color, filter_trans_par: ref argfilter_trans_par);
                GUI.MapX = map_x;
                GUI.MapY = map_y;

                // 再開イベントによるマップ画像の書き換え処理を行う
                Event_Renamed.HandleEvent("再開");
                Map.IsMapDirty = false;
            }

            // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedUnit = null;
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedTarget = null;

            // ユニット画像生成
            foreach (Unit u in UList)
            {
                {
                    var withBlock = u;
                    if (withBlock.BitmapID == 0)
                    {
                        withBlock.BitmapID = GUI.MakeUnitBitmap(ref u);
                    }
                }
            }

            // 画面更新
            GUI.Center(GUI.MapX, GUI.MapY);
            FileSystem.FileClose((int)SaveDataFileNumber);
            if (!quick_load)
            {
                GUI.DisplayLoadingProgress();
            }

            if (!quick_load)
            {
                GUI.CloseNowLoadingForm();
            }

            if (!quick_load)
            {
                GUI.MainForm.Show();
            }

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
            Status.ClearUnitStatus();
            if (!GUI.MainForm.Visible)
            {
                GUI.MainForm.Show();
                GUI.MainForm.Refresh();
            }

            GUI.RedrawScreen();
            if (Turn == 0)
            {
                Event_Renamed.HandleEvent("スタート");

                // MOD START MARGE
                // StartTurn "味方"
                // スタートイベントから次のステージが開始された場合、StartTurnが上のHandleEventで
                // 実行されてしまう。
                // 味方ターンの処理が２重起動されるのを防ぐため、Turnをチェックしてから起動する
                if (Turn == 0)
                {
                    string arguparty = "味方";
                    StartTurn(ref arguparty);
                }
            }
            // MOD END MARGE
            else
            {
                Commands.CommandState = "ユニット選択";
                Stage = "味方";
            }

            LastSaveDataFileName = fname;
            if (Strings.InStr(fname, @"\_リスタート.src") > 0)
            {
                IsRestartSaveDataAvailable = true;
            }
            else if (Strings.InStr(fname, @"\_クイックセーブ.src") > 0)
            {
                IsQuickSaveDataAvailable = true;
            }

            return;
            ErrorHandler:
            ;
            string argmsg = "ロード中にエラーが発生しました";
            GUI.ErrorMessage(ref argmsg);
            FileSystem.FileClose((int)SaveDataFileNumber);
            TerminateSRC();
        }


        // 旧形式のユニットＩＤを新形式に変換
        // 旧形式）ユニット名称+数値
        // 新形式）ユニット名称+":"+数値
        public static void ConvertUnitID(ref string ID)
        {
            short i;
            if (Strings.InStr(ID, ":") > 0)
            {
                return;
            }

            // 数値部分を読み飛ばす
            i = (short)Strings.Len(ID);
            while (i > 0)
            {
                switch (Strings.Asc(Strings.Mid(ID, i, 1)))
                {
                    // 0-9
                    case var @case when 48 <= @case && @case <= 57:
                        {
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                i = (short)(i - 1);
            }

            // ユニット名称と数値部分の間に「:」を挿入
            ID = Strings.Left(ID, i) + ":" + Strings.Mid(ID, i + 1);
        }

        // 作品new_titleのデータを読み込み
        public static void IncludeData(ref string new_title)
        {
            string fpath;

            // ロードのインジケータ表示を行う
            if (My.MyProject.Forms.frmNowLoading.Visible)
            {
                GUI.DisplayLoadingProgress();
            }

            // Dataフォルダの場所を探す
            fpath = SearchDataFolder(ref new_title);
            if (Strings.Len(fpath) == 0)
            {
                string argmsg = "データ「" + new_title + "」のフォルダが見つかりません";
                GUI.ErrorMessage(ref argmsg);
                TerminateSRC();
            };
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 122284


            Input:

                    On Error GoTo ErrorHandler

             */
            string argfname1 = fpath + @"\alias.txt";
            if (GeneralLib.FileExists(ref argfname1))
            {
                string argfname = fpath + @"\alias.txt";
                ALDList.Load(ref argfname);
            }

            bool localFileExists() { string argfname = fpath + @"\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            string argfname4 = fpath + @"\sp.txt";
            if (GeneralLib.FileExists(ref argfname4))
            {
                string argfname2 = fpath + @"\sp.txt";
                SPDList.Load(ref argfname2);
            }
            else if (localFileExists())
            {
                string argfname3 = fpath + @"\mind.txt";
                SPDList.Load(ref argfname3);
            }

            string argfname6 = fpath + @"\pilot.txt";
            if (GeneralLib.FileExists(ref argfname6))
            {
                string argfname5 = fpath + @"\pilot.txt";
                PDList.Load(ref argfname5);
            }

            string argfname8 = fpath + @"\non_pilot.txt";
            if (GeneralLib.FileExists(ref argfname8))
            {
                string argfname7 = fpath + @"\non_pilot.txt";
                NPDList.Load(ref argfname7);
            }

            string argfname10 = fpath + @"\robot.txt";
            if (GeneralLib.FileExists(ref argfname10))
            {
                string argfname9 = fpath + @"\robot.txt";
                UDList.Load(ref argfname9);
            }

            string argfname12 = fpath + @"\unit.txt";
            if (GeneralLib.FileExists(ref argfname12))
            {
                string argfname11 = fpath + @"\unit.txt";
                UDList.Load(ref argfname11);
            }

            // ロードのインジケータ表示を行う
            if (My.MyProject.Forms.frmNowLoading.Visible)
            {
                GUI.DisplayLoadingProgress();
            }

            string argfname14 = fpath + @"\pilot_message.txt";
            if (GeneralLib.FileExists(ref argfname14))
            {
                string argfname13 = fpath + @"\pilot_message.txt";
                MDList.Load(ref argfname13);
            }

            string argfname16 = fpath + @"\pilot_dialog.txt";
            if (GeneralLib.FileExists(ref argfname16))
            {
                string argfname15 = fpath + @"\pilot_dialog.txt";
                DDList.Load(ref argfname15);
            }

            string argfname18 = fpath + @"\effect.txt";
            if (GeneralLib.FileExists(ref argfname18))
            {
                string argfname17 = fpath + @"\effect.txt";
                EDList.Load(ref argfname17);
            }

            string argfname20 = fpath + @"\animation.txt";
            if (GeneralLib.FileExists(ref argfname20))
            {
                string argfname19 = fpath + @"\animation.txt";
                ADList.Load(ref argfname19);
            }

            string argfname22 = fpath + @"\ext_animation.txt";
            if (GeneralLib.FileExists(ref argfname22))
            {
                string argfname21 = fpath + @"\ext_animation.txt";
                EADList.Load(ref argfname21);
            }

            string argfname24 = fpath + @"\item.txt";
            if (GeneralLib.FileExists(ref argfname24))
            {
                string argfname23 = fpath + @"\item.txt";
                IDList.Load(ref argfname23);
            }

            return;
            ErrorHandler:
            ;
            string argmsg1 = "Src.ini内のExtDataPathの値が不正です";
            GUI.ErrorMessage(ref argmsg1);
            TerminateSRC();
        }

        // データフォルダ fname を検索
        public static string SearchDataFolder(ref string fname)
        {
            string SearchDataFolderRet = default;
            string fname2;
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static init_search_data_folder As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static scenario_data_dir_exists As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static extdata_data_dir_exists As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static extdata2_data_dir_exists As Boolean

             */
            ;

            // 初めて実行する際に、各フォルダにDataフォルダがあるかチェック
            if (!init_search_data_folder)
            {
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(ScenarioPath + "Data", FileAttribute.Directory)) > 0)
                {
                    scenario_data_dir_exists = true;
                }

                if (Strings.Len(ExtDataPath) > 0 & (ScenarioPath ?? "") != (ExtDataPath ?? ""))
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(ExtDataPath + "Data", FileAttribute.Directory)) > 0)
                    {
                        extdata_data_dir_exists = true;
                    }
                }

                if (Strings.Len(ExtDataPath2) > 0 & (ScenarioPath ?? "") != (ExtDataPath2 ?? ""))
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(ExtDataPath2 + "Data", FileAttribute.Directory)) > 0)
                    {
                        extdata2_data_dir_exists = true;
                    }
                }

                if ((ScenarioPath ?? "") != (AppPath ?? ""))
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(AppPath + "Data", FileAttribute.Directory)) > 0)
                    {
                        src_data_dir_exists = true;
                    }
                }

                init_search_data_folder = true;
            }

            // フォルダを検索
            fname2 = @"Data\" + fname;
            if (scenario_data_dir_exists)
            {
                SearchDataFolderRet = ScenarioPath + fname2;
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata_data_dir_exists)
            {
                SearchDataFolderRet = ExtDataPath + fname2;
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata2_data_dir_exists)
            {
                SearchDataFolderRet = ExtDataPath2 + fname2;
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (src_data_dir_exists)
            {
                SearchDataFolderRet = AppPath + fname2;
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            // フォルダが見つからなかった
            SearchDataFolderRet = "";
            return SearchDataFolderRet;
        }

        // 資金の量を変更する
        public static void IncrMoney(int earnings)
        {
            Money = GeneralLib.MinLng(Money + earnings, 999999999);
            Money = GeneralLib.MaxLng(Money, 0);
        }
    }
}