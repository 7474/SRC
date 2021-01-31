using SRC.Core.Lib;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core
{
    public partial class SRC
    {
        // UPGRADE_WARNING: Sub Main() が完了したときにアプリケーションは終了します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E08DDC71-66BA-424F-A612-80AF11498FF8"' をクリックしてください。
        public void Initialize()
        {
            throw new NotImplementedException();
//            string fname;
//            int i;
//            string buf;

//            // ２重起動禁止
//            // UPGRADE_ISSUE: App プロパティ App.PrevInstance はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"' をクリックしてください。
//            if (App.PrevInstance)
//            {
//                Environment.Exit(0);
//            }

//            // SRC.exeのある場所を調べる
//            AppPath = My.MyProject.Application.Info.DirectoryPath;
//            if (Strings.Right(AppPath, 1) != @"\")
//            {
//                AppPath = AppPath + @"\";
//            }

//            // SRCが正しくインストールされているかをチェック

//            // Bitmap関係のチェック
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + "Bitmap", FileAttribute.Directory)) == 0)
//            {
//                string argmsg = "Bitmapフォルダがありません。" + Constants.vbCr + Constants.vbLf + "SRC.exeと同じフォルダに汎用グラフィック集をインストールしてください。";
//                GUI.ErrorMessage(argmsg);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + "Ｂｉｔｍａｐ", FileAttribute.Directory)) > 0)
//            {
//                string argmsg1 = "Bitmapフォルダのフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + AppPath + "Ｂｉｔｍａｐ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
//                GUI.ErrorMessage(argmsg1);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Bitmap", FileAttribute.Directory)) > 0)
//            {
//                string argmsg2 = "Bitmapフォルダ内にさらにBitmapフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Bitmap\Bitmap" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
//                GUI.ErrorMessage(argmsg2);
//                Environment.Exit(0);
//            }

//            // イベントグラフィック
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Event", FileAttribute.Directory)) == 0)
//            {
//                string argmsg3 = @"Bitmap\Eventフォルダが見つかりません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
//                GUI.ErrorMessage(argmsg3);
//                Environment.Exit(0);
//            }

//            // マップグラフィック
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map", FileAttribute.Directory)) == 0)
//            {
//                string argmsg4 = @"Bitmap\Mapフォルダがありません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
//                GUI.ErrorMessage(argmsg4);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain\plain0000.bmp")) == 0 & Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain0000.bmp")) == 0 & Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\plain0.bmp")) == 0)
//            {
//                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\Map", FileAttribute.Directory)) > 0)
//                {
//                    string argmsg5 = @"Bitmap\Mapフォルダ内にさらにMapフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Bitmap\Map\Map" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
//                    GUI.ErrorMessage(argmsg5);
//                    Environment.Exit(0);
//                }

//                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                if (Strings.Len(FileSystem.Dir(AppPath + @"Bitmap\Map\*", FileAttribute.Normal)) == 0)
//                {
//                    string argmsg6 = @"Bitmap\Mapフォルダ内にファイルがありません。" + Constants.vbCr + Constants.vbLf + "汎用グラフィック集が正しくインストールされていないと思われます。";
//                    GUI.ErrorMessage(argmsg6);
//                    Environment.Exit(0);
//                }

//                string argmsg7 = @"Bitmap\Mapフォルダ内にplain0000.bmpがありません。" + Constants.vbCr + Constants.vbLf + "一部のマップ画像ファイルしかインストールされていない恐れがあります。" + Constants.vbCr + Constants.vbLf + "新規インストールのファイルを使って汎用グラフィック集をインストールしてください。";
//                GUI.ErrorMessage(argmsg7);
//                Environment.Exit(0);
//            }

//            // 効果音
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + "Sound", FileAttribute.Directory)) == 0)
//            {
//                string argmsg8 = "Soundフォルダがありません。" + Constants.vbCr + Constants.vbLf + "SRC.exeと同じフォルダに効果音集をインストールしてください。";
//                GUI.ErrorMessage(argmsg8);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + "Ｓｏｕｎｄ", FileAttribute.Directory)) > 0)
//            {
//                string argmsg9 = "Soundフォルダのフォルダ名が全角文字になっています。" + Constants.vbCr + Constants.vbLf + AppPath + "Ｓｏｕｎｄ" + Constants.vbCr + Constants.vbLf + "フォルダ名を半角文字に直してください。";
//                GUI.ErrorMessage(argmsg9);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Sound\Sound", FileAttribute.Directory)) > 0)
//            {
//                string argmsg10 = "Soundフォルダ内にさらにSoundフォルダが存在します。" + Constants.vbCr + Constants.vbLf + AppPath + @"Sound\Sound" + Constants.vbCr + Constants.vbLf + "フォルダ構造を直してください。";
//                GUI.ErrorMessage(argmsg10);
//                Environment.Exit(0);
//            }
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            if (Strings.Len(FileSystem.Dir(AppPath + @"Sound\*", FileAttribute.Normal)) == 0)
//            {
//                string argmsg11 = "Soundフォルダ内にファイルがありません。" + Constants.vbCr + Constants.vbLf + "Soundフォルダ内に効果音集をインストールしてください。";
//                GUI.ErrorMessage(argmsg11);
//                Environment.Exit(0);
//            }

//            // メインウィンドウのロードとFlashの登録を実施
//            GUI.LoadMainFormAndRegisterFlash();

//            // Src.iniが無ければ作成
//            bool localFileExists() { string argfname = AppPath + "Src.ini"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            if (!localFileExists())
//            {
//                CreateIniFile();
//            }

//            // 乱数の初期化
//            VBMath.Randomize();

//            // 時間解像度を変更する
//            GeneralLib.timeBeginPeriod(1);

//            // フルスクリーンモードを使う？
//            string argini_section = "Option";
//            string argini_entry = "FullScreen";
//            if (Strings.LCase(GeneralLib.ReadIni(argini_section, argini_entry)) == "on")
//            {
//                GUI.ChangeDisplaySize(800, 600);
//            }

//            // マウスカーソルを砂時計に
//            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
//            Cursor.Current = Cursors.WaitCursor;

//            // タイトル画面を表示
//            GUI.OpenTitleForm();

//            // WAVE再生の手段は？
//            string argini_section1 = "Option";
//            string argini_entry1 = "UseDirectSound";
//            switch (Strings.LCase(GeneralLib.ReadIni(argini_section1, argini_entry1)) ?? "")
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
//            string argini_section5 = "Option";
//            string argini_entry5 = "UseDirectMusic";
//            switch (Strings.LCase(GeneralLib.ReadIni(argini_section5, argini_entry5)) ?? "")
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
//                                string argini_section2 = "Option";
//                                string argini_entry2 = "UseDirectMusic";
//                                string argini_data = "On";
//                                GeneralLib.WriteIni(argini_section2, argini_entry2, argini_data);
//                            }
//                            else
//                            {
//                                string argini_section3 = "Option";
//                                string argini_entry3 = "UseDirectMusic";
//                                string argini_data1 = "Off";
//                                GeneralLib.WriteIni(argini_section3, argini_entry3, argini_data1);
//                            }
//                        }
//                        else
//                        {
//                            // NT系OSでなければMCIを使う
//                            Sound.UseMCI = true;
//                            string argini_section4 = "Option";
//                            string argini_entry4 = "UseDirectMusic";
//                            string argini_data2 = "Off";
//                            GeneralLib.WriteIni(argini_section4, argini_entry4, argini_data2);
//                        }

//                        break;
//                    }
//            }

//            string argini_section7 = "Option";
//            string argini_entry7 = "MIDIPortID";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section7, argini_entry7)))
//            {
//                string argini_section6 = "Option";
//                string argini_entry6 = "MIDIPortID";
//                string argini_data3 = "0";
//                GeneralLib.WriteIni(argini_section6, argini_entry6, argini_data3);
//            }

//            // MP3の再生音量
//            string argini_section8 = "Option";
//            string argini_entry8 = "MP3Volume";
//            buf = GeneralLib.ReadIni(argini_section8, argini_entry8);
//            if (string.IsNullOrEmpty(buf))
//            {
//                string argini_section9 = "Option";
//                string argini_entry9 = "MP3Volume";
//                string argini_data4 = "50";
//                GeneralLib.WriteIni(argini_section9, argini_entry9, argini_data4);
//                Sound.MP3Volume = 50;
//            }
//            else
//            {
//                Sound.MP3Volume = GeneralLib.StrToLng(buf);
//                if (Sound.MP3Volume < 0)
//                {
//                    string argini_section10 = "Option";
//                    string argini_entry10 = "MP3Volume";
//                    string argini_data5 = "0";
//                    GeneralLib.WriteIni(argini_section10, argini_entry10, argini_data5);
//                    Sound.MP3Volume = 0;
//                }
//                else if (Sound.MP3Volume > 100)
//                {
//                    string argini_section11 = "Option";
//                    string argini_entry11 = "MP3Volume";
//                    string argini_data6 = "100";
//                    GeneralLib.WriteIni(argini_section11, argini_entry11, argini_data6);
//                    Sound.MP3Volume = 100;
//                }
//            }

//            // MP3の出力フレーム数
//            string argini_section12 = "Option";
//            string argini_entry12 = "MP3OutputBlock";
//            buf = GeneralLib.ReadIni(argini_section12, argini_entry12);
//            if (string.IsNullOrEmpty(buf))
//            {
//                string argini_section13 = "Option";
//                string argini_entry13 = "MP3OutputBlock";
//                string argini_data7 = "20";
//                GeneralLib.WriteIni(argini_section13, argini_entry13, argini_data7);
//            }

//            // MP3の入力直後のスリープ時間
//            string argini_section14 = "Option";
//            string argini_entry14 = "MP3InputSleep";
//            buf = GeneralLib.ReadIni(argini_section14, argini_entry14);
//            if (string.IsNullOrEmpty(buf))
//            {
//                string argini_section15 = "Option";
//                string argini_entry15 = "MP3InputSleep";
//                string argini_data8 = "5";
//                GeneralLib.WriteIni(argini_section15, argini_entry15, argini_data8);
//            }

//            // ＢＧＭ用MIDIファイル設定
//            string argini_section17 = "BGM";
//            string argini_entry17 = "Opening";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section17, argini_entry17)))
//            {
//                string argini_section16 = "BGM";
//                string argini_entry16 = "Opening";
//                string argini_data9 = "Opening.mid";
//                GeneralLib.WriteIni(argini_section16, argini_entry16, argini_data9);
//            }

//            string argini_section19 = "BGM";
//            string argini_entry19 = "Map1";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section19, argini_entry19)))
//            {
//                string argini_section18 = "BGM";
//                string argini_entry18 = "Map1";
//                string argini_data10 = "Map1.mid";
//                GeneralLib.WriteIni(argini_section18, argini_entry18, argini_data10);
//            }

//            string argini_section21 = "BGM";
//            string argini_entry21 = "Map2";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section21, argini_entry21)))
//            {
//                string argini_section20 = "BGM";
//                string argini_entry20 = "Map2";
//                string argini_data11 = "Map2.mid";
//                GeneralLib.WriteIni(argini_section20, argini_entry20, argini_data11);
//            }

//            string argini_section23 = "BGM";
//            string argini_entry23 = "Map3";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section23, argini_entry23)))
//            {
//                string argini_section22 = "BGM";
//                string argini_entry22 = "Map3";
//                string argini_data12 = "Map3.mid";
//                GeneralLib.WriteIni(argini_section22, argini_entry22, argini_data12);
//            }

//            string argini_section25 = "BGM";
//            string argini_entry25 = "Map4";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section25, argini_entry25)))
//            {
//                string argini_section24 = "BGM";
//                string argini_entry24 = "Map4";
//                string argini_data13 = "Map4.mid";
//                GeneralLib.WriteIni(argini_section24, argini_entry24, argini_data13);
//            }

//            string argini_section27 = "BGM";
//            string argini_entry27 = "Map5";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section27, argini_entry27)))
//            {
//                string argini_section26 = "BGM";
//                string argini_entry26 = "Map5";
//                string argini_data14 = "Map5.mid";
//                GeneralLib.WriteIni(argini_section26, argini_entry26, argini_data14);
//            }

//            string argini_section29 = "BGM";
//            string argini_entry29 = "Map6";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section29, argini_entry29)))
//            {
//                string argini_section28 = "BGM";
//                string argini_entry28 = "Map6";
//                string argini_data15 = "Map6.mid";
//                GeneralLib.WriteIni(argini_section28, argini_entry28, argini_data15);
//            }

//            string argini_section31 = "BGM";
//            string argini_entry31 = "Briefing";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section31, argini_entry31)))
//            {
//                string argini_section30 = "BGM";
//                string argini_entry30 = "Briefing";
//                string argini_data16 = "Briefing.mid";
//                GeneralLib.WriteIni(argini_section30, argini_entry30, argini_data16);
//            }

//            string argini_section33 = "BGM";
//            string argini_entry33 = "Intermission";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section33, argini_entry33)))
//            {
//                string argini_section32 = "BGM";
//                string argini_entry32 = "Intermission";
//                string argini_data17 = "Intermission.mid";
//                GeneralLib.WriteIni(argini_section32, argini_entry32, argini_data17);
//            }

//            string argini_section35 = "BGM";
//            string argini_entry35 = "Subtitle";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section35, argini_entry35)))
//            {
//                string argini_section34 = "BGM";
//                string argini_entry34 = "Subtitle";
//                string argini_data18 = "Subtitle.mid";
//                GeneralLib.WriteIni(argini_section34, argini_entry34, argini_data18);
//            }

//            string argini_section37 = "BGM";
//            string argini_entry37 = "End";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section37, argini_entry37)))
//            {
//                string argini_section36 = "BGM";
//                string argini_entry36 = "End";
//                string argini_data19 = "End.mid";
//                GeneralLib.WriteIni(argini_section36, argini_entry36, argini_data19);
//            }

//            string argini_section39 = "BGM";
//            string argini_entry39 = "default";
//            if (string.IsNullOrEmpty(GeneralLib.ReadIni(argini_section39, argini_entry39)))
//            {
//                string argini_section38 = "BGM";
//                string argini_entry38 = "default";
//                string argini_data20 = "default.mid";
//                GeneralLib.WriteIni(argini_section38, argini_entry38, argini_data20);
//            }


//            // 起動時の引数から読み込むファイルを探す
//            if (Strings.Left(Interaction.Command(), 1) == "\"")
//            {
//                fname = Strings.Mid(Interaction.Command(), 2, Strings.Len(Interaction.Command()) - 2);
//            }
//            else
//            {
//                fname = Interaction.Command();
//            }

//            if (Strings.LCase(Strings.Right(fname, 4)) != ".src" & Strings.LCase(Strings.Right(fname, 4)) != ".eve")
//            {
//                // ダイアログを表示して読み込むファイルを指定する場合

//                // ダイアログの初期フォルダを設定
//                i = 0;
//                string argini_section40 = "Log";
//                string argini_entry40 = "LastFolder";
//                ScenarioPath = GeneralLib.ReadIni(argini_section40, argini_entry40);
//                ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//                /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 18399


//                Input:
//                            On Error GoTo ErrorHandler

//                 */
//                if (string.IsNullOrEmpty(ScenarioPath))
//                {
//                    ScenarioPath = AppPath;
//                }
//                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                else if (FileSystem.Dir(ScenarioPath, FileAttribute.Directory) == ".")
//                {
//                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                    if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "*.src")))
//                    {
//                        i = 3;
//                    }

//                    if (Strings.InStr(ScenarioPath, "テストデータ") > 0)
//                    {
//                        i = 2;
//                    }

//                    if (Strings.InStr(ScenarioPath, "戦闘アニメテスト") > 0)
//                    {
//                        i = 2;
//                    }
//                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                    if (!string.IsNullOrEmpty(FileSystem.Dir(ScenarioPath + "test.eve")))
//                    {
//                        i = 2;
//                    }
//                }
//                else
//                {
//                    ScenarioPath = AppPath;
//                };
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//                /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 19815


//                Input:
//                            On Error GoTo 0

//                 */
//                goto SkipErrorHandler;
//            ErrorHandler:
//                ;
//                ScenarioPath = AppPath;
//            SkipErrorHandler:
//                ;
//                if (Strings.Right(ScenarioPath, 1) != @"\")
//                {
//                    ScenarioPath = ScenarioPath + @"\";
//                }

//                // 拡張データのフォルダを設定
//                string argini_section41 = "Option";
//                string argini_entry41 = "ExtDataPath";
//                ExtDataPath = GeneralLib.ReadIni(argini_section41, argini_entry41);
//                if (Strings.Len(ExtDataPath) > 0)
//                {
//                    if (Strings.Right(ExtDataPath, 1) != @"\")
//                    {
//                        ExtDataPath = ExtDataPath + @"\";
//                    }
//                }

//                string argini_section42 = "Option";
//                string argini_entry42 = "ExtDataPath2";
//                ExtDataPath2 = GeneralLib.ReadIni(argini_section42, argini_entry42);
//                if (Strings.Len(ExtDataPath2) > 0)
//                {
//                    if (Strings.Right(ExtDataPath2, 1) != @"\")
//                    {
//                        ExtDataPath2 = ExtDataPath2 + @"\";
//                    }
//                }

//                // オープニング曲演奏
//                Sound.StopBGM(true);
//                string argbgm_name = "Opening";
//                string argbgm_name1 = Sound.BGMName(argbgm_name);
//                Sound.StartBGM(argbgm_name1, true);

//                // イベントデータを初期化
//                Event_Renamed.InitEventData();

//                // タイトル画面を閉じる
//                GUI.CloseTitleForm();

//                // マウスカーソルを元に戻す
//                // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
//                Cursor.Current = Cursors.Default;

//                // シナリオパスは変更される可能性があるので、MIDIファイルのサーチパスをリセット
//                Sound.ResetMidiSearchPath();

//                // プレイヤーにロードするファイルを尋ねる
//                string argdtitle = "シナリオ／セーブファイルの指定";
//                string argdefault_file = "";
//                string argftype = "ｲﾍﾞﾝﾄﾃﾞｰﾀ";
//                string argfsuffix = "eve";
//                string argftype2 = "ｾｰﾌﾞﾃﾞｰﾀ";
//                string argfsuffix2 = "src";
//                string argftype3 = "";
//                string argfsuffix3 = "";
//                fname = FileDialog.LoadFileDialog(argdtitle, ScenarioPath, argdefault_file, i, argftype, argfsuffix, argftype2, argfsuffix2, ftype3: argftype3, fsuffix3: argfsuffix3);

//                // ファイルが指定されなかった場合はそのまま終了
//                if (string.IsNullOrEmpty(fname))
//                {
//                    TerminateSRC();
//                    Environment.Exit(0);
//                }

//                // シナリオのあるフォルダのパスを収得
//                if (Strings.InStr(fname, @"\") > 0)
//                {
//                    var loopTo = Strings.Len(fname);
//                    for (i = 1; i <= loopTo; i++)
//                    {
//                        if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
//                        {
//                            break;
//                        }
//                    }

//                    ScenarioPath = Strings.Left(fname, Strings.Len(fname) - i);
//                }
//                else
//                {
//                    ScenarioPath = AppPath;
//                }

//                if (Strings.Right(ScenarioPath, 1) != @"\")
//                {
//                    ScenarioPath = ScenarioPath + @"\";
//                }
//                // ADD START MARGE
//                // シナリオパスが決定した段階で拡張データフォルダパスを再設定するように変更
//                // 拡張データのフォルダを設定
//                string argini_section43 = "Option";
//                string argini_entry43 = "ExtDataPath";
//                ExtDataPath = GeneralLib.ReadIni(argini_section43, argini_entry43);
//                if (Strings.Len(ExtDataPath) > 0)
//                {
//                    if (Strings.Right(ExtDataPath, 1) != @"\")
//                    {
//                        ExtDataPath = ExtDataPath + @"\";
//                    }
//                }

//                string argini_section44 = "Option";
//                string argini_entry44 = "ExtDataPath2";
//                ExtDataPath2 = GeneralLib.ReadIni(argini_section44, argini_entry44);
//                if (Strings.Len(ExtDataPath2) > 0)
//                {
//                    if (Strings.Right(ExtDataPath2, 1) != @"\")
//                    {
//                        ExtDataPath2 = ExtDataPath2 + @"\";
//                    }
//                }
//            }
//            // ADD  END  MARGE
//            else
//            {
//                // ドラッグ＆ドロップで読み込むファイルが指定された場合

//                // ファイル名が無効の場合はそのまま終了
//                if (string.IsNullOrEmpty(fname))
//                {
//                    TerminateSRC();
//                    Environment.Exit(0);
//                }

//                // シナリオのあるフォルダのパスを収得
//                if (Strings.InStr(fname, @"\") > 0)
//                {
//                    var loopTo1 = Strings.Len(fname);
//                    for (i = 1; i <= loopTo1; i++)
//                    {
//                        if (Strings.Mid(fname, Strings.Len(fname) - i + 1, 1) == @"\")
//                        {
//                            break;
//                        }
//                    }

//                    ScenarioPath = Strings.Left(fname, Strings.Len(fname) - i);
//                }
//                else
//                {
//                    ScenarioPath = AppPath;
//                }

//                if (Strings.Right(ScenarioPath, 1) != @"\")
//                {
//                    ScenarioPath = ScenarioPath + @"\";
//                }

//                // 拡張データのフォルダを設定
//                string argini_section45 = "Option";
//                string argini_entry45 = "ExtDataPath";
//                ExtDataPath = GeneralLib.ReadIni(argini_section45, argini_entry45);
//                if (Strings.Len(ExtDataPath) > 0)
//                {
//                    if (Strings.Right(ExtDataPath, 1) != @"\")
//                    {
//                        ExtDataPath = ExtDataPath + @"\";
//                    }
//                }

//                string argini_section46 = "Option";
//                string argini_entry46 = "ExtDataPath2";
//                ExtDataPath2 = GeneralLib.ReadIni(argini_section46, argini_entry46);
//                if (Strings.Len(ExtDataPath2) > 0)
//                {
//                    if (Strings.Right(ExtDataPath2, 1) != @"\")
//                    {
//                        ExtDataPath2 = ExtDataPath2 + @"\";
//                    }
//                }

//                // オープニング曲演奏
//                Sound.StopBGM(true);
//                string argbgm_name2 = "Opening";
//                string argbgm_name3 = Sound.BGMName(argbgm_name2);
//                Sound.StartBGM(argbgm_name3, true);
//                Event_Renamed.InitEventData();
//                GUI.CloseTitleForm();

//                // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
//                Cursor.Current = Cursors.Default;
//            }

//            // ロングネームにしておく
//            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//            fname = ScenarioPath + FileSystem.Dir(fname);
//            if (!GeneralLib.FileExists(fname))
//            {
//                string argmsg12 = "指定したファイルが存在しません";
//                GUI.ErrorMessage(argmsg12);
//                TerminateSRC();
//            }

//            if (Strings.InStr(fname, "不要ファイル削除") == 0 & Strings.InStr(fname, "必須修正") == 0)
//            {
//                // 開いたフォルダをSrc.iniにセーブしておく
//                string argini_section47 = "Log";
//                string argini_entry47 = "LastFolder";
//                GeneralLib.WriteIni(argini_section47, argini_entry47, ScenarioPath);
//            }

//            // Src.iniから各種パラメータの読み込み

//            // スペシャルパワーアニメ
//            string argini_section48 = "Option";
//            string argini_entry48 = "SpecialPowerAnimation";
//            buf = GeneralLib.ReadIni(argini_section48, argini_entry48);
//            if (string.IsNullOrEmpty(buf))
//            {
//                string argini_section49 = "Option";
//                string argini_entry49 = "MindEffect";
//                buf = GeneralLib.ReadIni(argini_section49, argini_entry49);
//                if (!string.IsNullOrEmpty(buf))
//                {
//                    string argini_section50 = "Option";
//                    string argini_entry50 = "SpecialPowerAnimation";
//                    GeneralLib.WriteIni(argini_section50, argini_entry50, buf);
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
//                string argini_section51 = "Option";
//                string argini_entry51 = "SpecialPowerAnimation";
//                string argini_data21 = "On";
//                GeneralLib.WriteIni(argini_section51, argini_entry51, argini_data21);
//            }
//            else
//            {
//                string argini_section52 = "Option";
//                string argini_entry52 = "SpecialPowerAnimation";
//                string argini_data22 = "Off";
//                GeneralLib.WriteIni(argini_section52, argini_entry52, argini_data22);
//            }

//            // 戦闘アニメ
//            string argini_section53 = "Option";
//            string argini_entry53 = "BattleAnimation";
//            buf = Strings.LCase(GeneralLib.ReadIni(argini_section53, argini_entry53));
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
//                string argini_section54 = "Option";
//                string argini_entry54 = "BattleAnimation";
//                string argini_data23 = "On";
//                GeneralLib.WriteIni(argini_section54, argini_entry54, argini_data23);
//            }
//            else
//            {
//                string argini_section55 = "Option";
//                string argini_entry55 = "BattleAnimation";
//                string argini_data24 = "Off";
//                GeneralLib.WriteIni(argini_section55, argini_entry55, argini_data24);
//            }

//            // 拡大戦闘アニメ
//            string argini_section56 = "Option";
//            string argini_entry56 = "ExtendedAnimation";
//            buf = Strings.LCase(GeneralLib.ReadIni(argini_section56, argini_entry56));
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
//                string argini_section57 = "Option";
//                string argini_entry57 = "ExtendedAnimation";
//                string argini_data25 = "On";
//                GeneralLib.WriteIni(argini_section57, argini_entry57, argini_data25);
//            }

//            // 武器準備アニメ
//            string argini_section58 = "Option";
//            string argini_entry58 = "WeaponAnimation";
//            buf = Strings.LCase(GeneralLib.ReadIni(argini_section58, argini_entry58));
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
//                string argini_section59 = "Option";
//                string argini_entry59 = "WeaponAnimation";
//                string argini_data26 = "On";
//                GeneralLib.WriteIni(argini_section59, argini_entry59, argini_data26);
//            }

//            // 移動アニメ
//            string argini_section60 = "Option";
//            string argini_entry60 = "MoveAnimation";
//            buf = Strings.LCase(GeneralLib.ReadIni(argini_section60, argini_entry60));
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
//                string argini_section61 = "Option";
//                string argini_entry61 = "MoveAnimation";
//                string argini_data27 = "On";
//                GeneralLib.WriteIni(argini_section61, argini_entry61, argini_data27);
//            }

//            // メッセージ速度を設定
//            string argini_section62 = "Option";
//            string argini_entry62 = "MessageWait";
//            buf = GeneralLib.ReadIni(argini_section62, argini_entry62);
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
//                string argini_section63 = "Option";
//                string argini_entry63 = "MessageWait";
//                string argini_data28 = "700";
//                GeneralLib.WriteIni(argini_section63, argini_entry63, argini_data28);
//            }

//            // マス目を表示するかどうか
//            string argini_section64 = "Option";
//            string argini_entry64 = "Square";
//            buf = GeneralLib.ReadIni(argini_section64, argini_entry64);
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
//                string argini_section65 = "Option";
//                string argini_entry65 = "Square";
//                string argini_data29 = "Off";
//                GeneralLib.WriteIni(argini_section65, argini_entry65, argini_data29);
//            }

//            // 敵ターンにＢＧＭを変更するかどうか
//            string argini_section66 = "Option";
//            string argini_entry66 = "KeepEnemyBGM";
//            buf = GeneralLib.ReadIni(argini_section66, argini_entry66);
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
//                string argini_section67 = "Option";
//                string argini_entry67 = "KeepEnemyBGM";
//                string argini_data30 = "Off";
//                GeneralLib.WriteIni(argini_section67, argini_entry67, argini_data30);
//            }

//            // 音源のリセットデータの種類
//            string argini_section68 = "Option";
//            string argini_entry68 = "MidiReset";
//            MidiResetType = GeneralLib.ReadIni(argini_section68, argini_entry68);

//            // 自動反撃モード
//            string argini_section69 = "Option";
//            string argini_entry69 = "AutoDefense";
//            buf = GeneralLib.ReadIni(argini_section69, argini_entry69);
//            if (string.IsNullOrEmpty(buf))
//            {
//                string argini_section70 = "Option";
//                string argini_entry70 = "AutoDeffence";
//                buf = GeneralLib.ReadIni(argini_section70, argini_entry70);
//                if (!string.IsNullOrEmpty(buf))
//                {
//                    string argini_section71 = "Option";
//                    string argini_entry71 = "AutoDefense";
//                    GeneralLib.WriteIni(argini_section71, argini_entry71, buf);
//                }
//            }

//            if (!string.IsNullOrEmpty(buf))
//            {
//                if (Strings.LCase(buf) == "on")
//                {
//                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                    GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = true;
//                }
//                else
//                {
//                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                    GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = false;
//                }
//            }
//            else
//            {
//                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked = false;
//                string argini_section72 = "Option";
//                string argini_entry72 = "AutoDefense";
//                string argini_data31 = "Off";
//                GeneralLib.WriteIni(argini_section72, argini_entry72, argini_data31);
//            }

//            // カーソル自動移動
//            string argini_section73 = "Option";
//            string argini_entry73 = "AutoMoveCursor";
//            buf = GeneralLib.ReadIni(argini_section73, argini_entry73);
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
//                string argini_section74 = "Option";
//                string argini_entry74 = "AutoMoveCursor";
//                string argini_data32 = "On";
//                GeneralLib.WriteIni(argini_section74, argini_entry74, argini_data32);
//            }

//            // 各ウィンドウをロード (メインウィンドウは先にロード済み)
//            GUI.LoadForms();

//            // 画像バッファの枚数
//            string argini_section75 = "Option";
//            string argini_entry75 = "ImageBufferNum";
//            buf = GeneralLib.ReadIni(argini_section75, argini_entry75);
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
//                string argini_section76 = "Option";
//                string argini_entry76 = "ImageBufferNum";
//                string argini_data33 = "64";
//                GeneralLib.WriteIni(argini_section76, argini_entry76, argini_data33);
//            }

//            // 画像バッファを作成
//            GUI.MakePicBuf();

//            // 画像バッファの最大サイズ
//            string argini_section77 = "Option";
//            string argini_entry77 = "MaxImageBufferSize";
//            buf = GeneralLib.ReadIni(argini_section77, argini_entry77);
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
//                string argini_section78 = "Option";
//                string argini_entry78 = "MaxImageBufferSize";
//                string argini_data34 = "8";
//                GeneralLib.WriteIni(argini_section78, argini_entry78, argini_data34);
//            }

//            // 拡大画像を画像バッファに保存するか
//            string argini_section79 = "Option";
//            string argini_entry79 = "KeepStretchedImage";
//            buf = GeneralLib.ReadIni(argini_section79, argini_entry79);
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
//                string argini_section80 = "Option";
//                string argini_entry80 = "KeepStretchedImage";
//                string argini_data35 = "On";
//                GeneralLib.WriteIni(argini_section80, argini_entry80, argini_data35);
//            }
//            else
//            {
//                KeepStretchedImage = false;
//                string argini_section81 = "Option";
//                string argini_entry81 = "KeepStretchedImage";
//                string argini_data36 = "Off";
//                GeneralLib.WriteIni(argini_section81, argini_entry81, argini_data36);
//            }

//            // 透過描画にUseTransparentBltを使用するか
//            if (GeneralLib.GetWinVersion() >= 500)
//            {
//                string argini_section82 = "Option";
//                string argini_entry82 = "UseTransparentBlt";
//                buf = GeneralLib.ReadIni(argini_section82, argini_entry82);
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
//                    string argini_section83 = "Option";
//                    string argini_entry83 = "UseTransparentBlt";
//                    string argini_data37 = "On";
//                    GeneralLib.WriteIni(argini_section83, argini_entry83, argini_data37);
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
//            bool localFileExists1() { string argfname = AppPath + @"Data\System\alias.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            string argfname2 = ScenarioPath + @"Data\System\alias.txt";
//            if (GeneralLib.FileExists(argfname2))
//            {
//                string argfname = ScenarioPath + @"Data\System\alias.txt";
//                ALDList.Load(argfname);
//            }
//            else if (localFileExists1())
//            {
//                string argfname1 = AppPath + @"Data\System\alias.txt";
//                ALDList.Load(argfname1);
//            }
//            // スペシャルパワーデータをロード
//            bool localFileExists2() { string argfname = ScenarioPath + @"Data\System\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            bool localFileExists3() { string argfname = AppPath + @"Data\System\sp.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            bool localFileExists4() { string argfname = AppPath + @"Data\System\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            string argfname7 = ScenarioPath + @"Data\System\sp.txt";
//            if (GeneralLib.FileExists(argfname7))
//            {
//                string argfname3 = ScenarioPath + @"Data\System\sp.txt";
//                SPDList.Load(argfname3);
//            }
//            else if (localFileExists2())
//            {
//                string argfname4 = ScenarioPath + @"Data\System\mind.txt";
//                SPDList.Load(argfname4);
//            }
//            else if (localFileExists3())
//            {
//                string argfname5 = AppPath + @"Data\System\sp.txt";
//                SPDList.Load(argfname5);
//            }
//            else if (localFileExists4())
//            {
//                string argfname6 = AppPath + @"Data\System\mind.txt";
//                SPDList.Load(argfname6);
//            }
//            // 汎用アイテムデータをロード
//            bool localFileExists5() { string argfname = AppPath + @"Data\System\item.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            string argfname10 = ScenarioPath + @"Data\System\item.txt";
//            if (GeneralLib.FileExists(argfname10))
//            {
//                string argfname8 = ScenarioPath + @"Data\System\item.txt";
//                IDList.Load(argfname8);
//            }
//            else if (localFileExists5())
//            {
//                string argfname9 = AppPath + @"Data\System\item.txt";
//                IDList.Load(argfname9);
//            }
//            // 地形データをロード
//            string argfname12 = AppPath + @"Data\System\terrain.txt";
//            if (GeneralLib.FileExists(argfname12))
//            {
//                string argfname11 = AppPath + @"Data\System\terrain.txt";
//                TDList.Load(argfname11);
//            }
//            else
//            {
//                string argmsg13 = @"地形データファイル「Data\System\terrain.txt」が見つかりません";
//                GUI.ErrorMessage(argmsg13);
//                TerminateSRC();
//            }

//            string argfname14 = ScenarioPath + @"Data\System\terrain.txt";
//            if (GeneralLib.FileExists(argfname14))
//            {
//                string argfname13 = ScenarioPath + @"Data\System\terrain.txt";
//                TDList.Load(argfname13);
//            }
//            // バトルコンフィグデータをロード
//            bool localFileExists6() { string argfname = AppPath + @"Data\System\battle.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

//            string argfname17 = ScenarioPath + @"Data\System\battle.txt";
//            if (GeneralLib.FileExists(argfname17))
//            {
//                string argfname15 = ScenarioPath + @"Data\System\battle.txt";
//                BCList.Load(argfname15);
//            }
//            else if (localFileExists6())
//            {
//                string argfname16 = AppPath + @"Data\System\battle.txt";
//                BCList.Load(argfname16);
//            }

//            // マップを初期化
//            Map.InitMap();

//            // 乱数系列を初期化
//            GeneralLib.RndSeed = Conversion.Int(1000000f * VBMath.Rnd());
//            GeneralLib.RndReset();
//            if (Strings.LCase(Strings.Right(fname, 4)) == ".src")
//            {
//                SaveDataFileNumber = FileSystem.FreeFile();
//                FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
//                // 第１項目を読み込み
//                FileSystem.Input(SaveDataFileNumber, buf);
//                // 第１項目はセーブデータバージョン？
//                if (Information.IsNumeric(buf))
//                {
//                    if (Conversions.ToInteger(buf) > 10000)
//                    {
//                        // バージョンデータであれば第２項目を読み込み
//                        FileSystem.Input(SaveDataFileNumber, buf);
//                    }
//                }

//                FileSystem.FileClose(SaveDataFileNumber);

//                // データの種類を判定
//                if (Information.IsNumeric(buf))
//                {
//                    // セーブデータの読み込み
//                    GUI.OpenNowLoadingForm();
//                    LoadData(fname);
//                    GUI.CloseNowLoadingForm();

//                    // インターミッション
//                    InterMission.InterMissionCommand(true);
//                    if (!IsSubStage)
//                    {
//                        string argexpr = "次ステージ";
//                        if (string.IsNullOrEmpty(Expression.GetValueAsString(argexpr)))
//                        {
//                            string argmsg14 = "次のステージのファイル名が設定されていません";
//                            GUI.ErrorMessage(argmsg14);
//                            TerminateSRC();
//                        }

//                        string argexpr1 = "次ステージ";
//                        StartScenario(Expression.GetValueAsString(argexpr1));
//                    }
//                    else
//                    {
//                        IsSubStage = false;
//                    }
//                }
//                else
//                {
//                    // 中断データの読み込み
//                    GUI.LockGUI();
//                    bool argquick_load = false;
//                    RestoreData(fname, argquick_load);

//                    // 画面を書き直してステータスを表示
//                    GUI.RedrawScreen();
//                    Status.DisplayGlobalStatus();
//                    GUI.UnlockGUI();
//                }
//            }
//            else if (Strings.LCase(Strings.Right(fname, 4)) == ".eve")
//            {
//                // イベントファイルの実行
//                StartScenario(fname);
//            }
//            else
//            {
//                string argmsg15 = "「" + fname + "」はSRC用のファイルではありません！" + Constants.vbCr + Constants.vbLf + "拡張子が「.eve」のイベントファイル、" + "または拡張子が「.src」のセーブデータファイルを指定して下さい。";
//                GUI.ErrorMessage(argmsg15);
//                TerminateSRC();
//            }
        }
    }
}
