using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore
{
    public partial class SRC
    {
        // INIファイルを作成する
        public void CreateIniFile()
        {
            // TODO Impl 別途設定管理する仕組みにする
            //try
            //{
            //    int f;
            //    f = FileSystem.FreeFile();
            //    FileSystem.FileOpen(f, AppPath + "Src.ini", OpenMode.Output, OpenAccess.Write);
            //    FileSystem.PrintLine(f, ";SRCの設定ファイルです。");
            //    FileSystem.PrintLine(f, ";項目の内容に関してはヘルプの");
            //    FileSystem.PrintLine(f, "; 操作方法 => マップコマンド => 設定変更");
            //    FileSystem.PrintLine(f, ";の項を参照して下さい。");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, "[Option]");
            //    FileSystem.PrintLine(f, ";メッセージのウェイト。標準は700");
            //    FileSystem.PrintLine(f, "MessageWait=700");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";ターン数の表示 [On|Off]");
            //    FileSystem.PrintLine(f, "Turn=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";マス目の表示 [On|Off]");
            //    FileSystem.PrintLine(f, "Square=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";敵フェイズにはＢＧＭを変更しない [On|Off]");
            //    FileSystem.PrintLine(f, "KeepEnemyBGM=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";自動防御モード [On|Off]");
            //    FileSystem.PrintLine(f, "AutoDefense=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";自動カーソル移動 [On|Off]");
            //    FileSystem.PrintLine(f, "AutoMoveCursor=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";スペシャルパワーアニメ [On|Off]");
            //    FileSystem.PrintLine(f, "SpecialPowerAnimation=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";戦闘アニメ [On|Off]");
            //    FileSystem.PrintLine(f, "BattleAnimation=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";戦闘アニメの拡張機能 [On|Off]");
            //    FileSystem.PrintLine(f, "ExtendedAnimation=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";武器準備アニメの自動選択表示 [On|Off]");
            //    FileSystem.PrintLine(f, "WeaponAnimation=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";移動アニメ [On|Off]");
            //    FileSystem.PrintLine(f, "MoveAnimation=On");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";MIDI音源リセットの種類 [None|GM|GS|XG]");
            //    FileSystem.PrintLine(f, "MidiReset=None");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";MIDI演奏にDirectMusicを使う [On|Off]");
            //    if (GeneralLib.GetWinVersion() >= 500)
            //    {
            //        // NT系のOSではデフォルトでDirectMusicを使う
            //        // DirectMusicの初期化を試みる
            //        Sound.InitDirectMusic();
            //        // DirectMusicが使用可能かどうかで設定を切り替え
            //        if (Sound.UseDirectMusic)
            //        {
            //            FileSystem.PrintLine(f, "UseDirectMusic=On");
            //        }
            //        else
            //        {
            //            FileSystem.PrintLine(f, "UseDirectMusic=Off");
            //        }
            //    }
            //    else
            //    {
            //        // NT系OSでなければMCIを使う
            //        Sound.UseMCI = true;
            //        FileSystem.PrintLine(f, "UseDirectMusic=Off");
            //    }

            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";DirectMusicで使うMIDI音源のポート番号 [自動検索=0]");
            //    FileSystem.PrintLine(f, "MIDIPortID=0");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";MP3再生時の音量 (0～100)");
            //    FileSystem.PrintLine(f, "MP3Volume=50");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";MP3の出力フレーム数");
            //    FileSystem.PrintLine(f, "MP3OutputBlock=20");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";MP3の入力直後のスリープ時間(ミリ秒)");
            //    FileSystem.PrintLine(f, "MP3IutputSleep=5");
            //    FileSystem.PrintLine(f, "");
            //    // Print #f, ";WAV再生にDirectSoundを使う [On|Off]"
            //    // Print #f, "UseDirectSound=On"
            //    // Print #f, ""
            //    FileSystem.PrintLine(f, ";画像バッファの枚数");
            //    FileSystem.PrintLine(f, "ImageBufferNum=64");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";画像バッファの最大サイズ (MB)");
            //    FileSystem.PrintLine(f, "MaxImageBufferSize=8");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";拡大画像を画像バッファに保存する [On|Off]");
            //    FileSystem.PrintLine(f, "KeepStretchedImage=");
            //    FileSystem.PrintLine(f, "");
            //    if (GeneralLib.GetWinVersion() >= 500)
            //    {
            //        FileSystem.PrintLine(f, ";透過描画にAPI関数TransparentBltを使う [On|Off]");
            //        FileSystem.PrintLine(f, "UseTransparentBlt=On");
            //        FileSystem.PrintLine(f, "");
            //    }

            //    FileSystem.PrintLine(f, ";拡張データのフォルダ (フルパスで指定)");
            //    FileSystem.PrintLine(f, "ExtDataPath=");
            //    FileSystem.PrintLine(f, "ExtDataPath2=");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";デバッグモード [On|Off]");
            //    FileSystem.PrintLine(f, "DebugMode=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, ";新ＧＵＩ(テスト中) [On|Off]");
            //    FileSystem.PrintLine(f, "NewGUI=Off");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, "[Log]");
            //    FileSystem.PrintLine(f, ";前回使用したフォルダ");
            //    FileSystem.PrintLine(f, "LastFolder=");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.PrintLine(f, "[BGM]");
            //    FileSystem.PrintLine(f, ";SRC起動時");
            //    FileSystem.PrintLine(f, "Opening=Opening.mid");
            //    FileSystem.PrintLine(f, ";味方フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map1=Map1.mid");
            //    FileSystem.PrintLine(f, ";敵フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map2=Map2.mid");
            //    FileSystem.PrintLine(f, ";屋内マップの味方フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map3=Map3.mid");
            //    FileSystem.PrintLine(f, ";屋内マップの敵フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map4=Map4.mid");
            //    FileSystem.PrintLine(f, ";宇宙マップの味方フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map5=Map5.mid");
            //    FileSystem.PrintLine(f, ";宇宙マップの敵フェイズ開始時");
            //    FileSystem.PrintLine(f, "Map6=Map6.mid");
            //    FileSystem.PrintLine(f, ";プロローグ・エピローグ開始時");
            //    FileSystem.PrintLine(f, "Briefing=Briefing.mid");
            //    FileSystem.PrintLine(f, ";インターミッション開始時");
            //    FileSystem.PrintLine(f, "Intermission=Intermission.mid");
            //    FileSystem.PrintLine(f, ";テロップ表示時");
            //    FileSystem.PrintLine(f, "Subtitle=Subtitle.mid");
            //    FileSystem.PrintLine(f, ";ゲームオーバー時");
            //    FileSystem.PrintLine(f, "End=End.mid");
            //    FileSystem.PrintLine(f, ";戦闘時のデフォルトMIDI");
            //    FileSystem.PrintLine(f, "default=default.mid");
            //    FileSystem.PrintLine(f, "");
            //    FileSystem.FileClose(f);
            //}
            //catch
            //{
            //    throw;
            //}
        }

        // KeepStretchedImageを使用すべきか決定するため、BitBltと
        // StretchBltの速度差を測定
        private object IsBitBltFasterThanStretchBlt()
        {
            // TODO Impl 多分消す
            object IsBitBltFasterThanStretchBltRet = default;
            //int stime, etime;
            //int bb_time, sb_time;
            //int ret;
            //int i;
            //{
            //    var withBlock = GUI.MainForm;
            //    // 描画領域を設定
            //    {
            //        var withBlock1 = withBlock.picStretchedTmp(0);
            //        withBlock1.width = 128;
            //        withBlock1.Height = 128;
            //    }
            //    {
            //        var withBlock2 = withBlock.picStretchedTmp(1);
            //        withBlock2.width = 128;
            //        withBlock2.Height = 128;
            //    }

            //    // StretchBltの転送速度を測定
            //    stime = GeneralLib.timeGetTime();
            //    for (i = 1; i <= 5; i++)
            //        ret = GUI.StretchBlt(withBlock.picStretchedTmp(0).hDC, 0, 0, 480, 480, withBlock.picUnit.hDC, 0, 0, 32, 32, GUI.SRCCOPY);
            //    etime = GeneralLib.timeGetTime();
            //    sb_time = etime - stime;

            //    // BitBltの転送速度を測定
            //    stime = GeneralLib.timeGetTime();
            //    for (i = 1; i <= 5; i++)
            //        ret = GUI.BitBlt(withBlock.picStretchedTmp(1).hDC, 0, 0, 480, 480, withBlock.picStretchedTmp(0).hDC, 0, 0, GUI.SRCCOPY);
            //    etime = GeneralLib.timeGetTime();
            //    bb_time = etime - stime;

            //    // 描画領域を開放
            //    {
            //        var withBlock3 = withBlock.picStretchedTmp(0);
            //        withBlock3.Picture = null;
            //        withBlock3.width = 32;
            //        withBlock3.Height = 32;
            //    }
            //    {
            //        var withBlock4 = withBlock.picStretchedTmp(1);
            //        withBlock4.Picture = null;
            //        withBlock4.width = 32;
            //        withBlock4.Height = 32;
            //    }
            //}

            //// BitBltがStretchBltより2倍以上速ければBitBltを優先して使用する
            //if (2 * bb_time < sb_time)
            //{
            //    // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //    IsBitBltFasterThanStretchBltRet = true;
            //}
            //else
            //{
            //    // UPGRADE_WARNING: オブジェクト IsBitBltFasterThanStretchBlt の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //    IsBitBltFasterThanStretchBltRet = false;
            //}

            return IsBitBltFasterThanStretchBltRet;
        }
    }
}
