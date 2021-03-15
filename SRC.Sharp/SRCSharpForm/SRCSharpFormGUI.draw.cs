using SRCCore;
using SRCCore.Lib;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SRCSharpForm
{
    public partial class SRCSharpFormGUI
    {
        // XXX ファイルシステム周りはインタフェースを切って環境毎に実装してやるのがよさそう。
        // SRCクラスにある XXDataPath もそちらに切り出すとよいはず。
        private bool init_draw_pitcure;
        private IList<string> existBitmapDirectories;
        private IList<string> existMapBitmapDirectories;

        // 画像をウィンドウに描画
        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            // ダミーのファイル名？
            switch (fname ?? "")
            {
                case var @case when @case == "":
                case "-.bmp":
                case "EFFECT_Void.bmp":
                    return false;
            }

            // Debug.Print fname, draw_option

            // オプションの解析
            var BGColor = Color.White;
            // マスク画像に影響しないオプション
            var pic_option = "";
            // マスク画像に影響するオプション
            var pic_option2 = "";
            // フィルタ時の透過度を初期化
            double trans_par = -1;
            bool permanent = false;
            bool transparent = false;
            int bright_count = 0;
            int dark_count = 0;
            bool is_monotone = false;
            bool is_sepia = false;
            bool is_sunset = false;
            bool is_water = false;
            bool is_colorfilter = false;
            bool hrev = false;
            bool vrev = false;
            bool negpos = false;
            bool is_sil = false;
            bool top_part = false;
            bool bottom_part = false;
            bool right_part = false;
            bool left_part = false;
            bool tright_part = false;
            bool tleft_part = false;
            bool bright_part = false;
            bool bleft_part = false;
            bool on_msg_window = false;
            bool on_status_window = false;
            bool keep_picture = false;
            // XXX Color?
            int fcolor = 0;
            int angle = 0;
            var opts = GeneralLib.ToL(draw_option);
            for (var i = 0; i < opts.Count; i++)
            {
                var opt = opts[i];
                switch (opt ?? "")
                {
                    case "背景":
                        permanent = true;
                        // 背景書き込みで夜やセピア色のマップの場合は指定がなくても特殊効果を付ける
                        switch (Map.MapDrawMode ?? "")
                        {
                            case "夜":
                                dark_count = (dark_count + 1);
                                pic_option = pic_option + " 暗";
                                break;

                            case "白黒":
                                is_monotone = true;
                                pic_option = pic_option + " 白黒";
                                break;

                            case "セピア":
                                is_sepia = true;
                                pic_option = pic_option + " セピア";
                                break;

                            case "夕焼け":
                                is_sunset = true;
                                pic_option = pic_option + " 夕焼け";
                                break;

                            case "水中":
                                is_water = true;
                                pic_option = pic_option + " 水中";
                                break;

                            case "フィルタ":
                                is_colorfilter = true;
                                fcolor = Map.MapDrawFilterColor;
                                pic_option2 = pic_option2 + " フィルタ=" + Map.MapDrawFilterColor.ToString();
                                break;
                        }

                        break;

                    case "透過":
                        transparent = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "白黒":
                        is_monotone = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "セピア":
                        is_sepia = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "夕焼け":
                        is_sunset = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "水中":
                        is_water = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "明":
                        bright_count = (bright_count + 1);
                        pic_option = pic_option + " " + opt;
                        break;

                    case "暗":
                        dark_count = (dark_count + 1);
                        pic_option = pic_option + " " + opt;
                        break;

                    case "左右反転":
                        hrev = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "上下反転":
                        vrev = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "ネガポジ反転":
                        negpos = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "シルエット":
                        is_sil = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "上半分":
                        top_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "下半分":
                        bottom_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右半分":
                        right_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左半分":
                        left_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右上":
                        tright_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左上":
                        tleft_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右下":
                        bright_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左下":
                        bleft_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "メッセージ":
                        on_msg_window = true;
                        break;

                    case "ステータス":
                        on_status_window = true;
                        break;

                    case "保持":
                        keep_picture = true;
                        break;

                    case "右回転":
                        i = (i + 1);
                        angle = GeneralLib.StrToLng(opts[i]);
                        pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                        break;

                    case "左回転":
                        i = (i + 1);
                        angle = GeneralLib.StrToLng(opts[i]) * -1;
                        pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                        break;

                    case "フィルタ":
                        is_colorfilter = true;
                        break;

                    default:
                        if (Strings.Right(opt, 1) == "%" & Information.IsNumeric(Strings.Left(opt, Strings.Len(opt) - 1)))
                        {
                            trans_par = Math.Max(0d, Math.Min(1d, Conversions.ToDouble(Strings.Left(opt, Strings.Len(opt) - 1)) / 100d));
                            pic_option2 = pic_option2 + " フィルタ透過度=" + opt;
                        }
                        else if (is_colorfilter)
                        {
                            fcolor = Conversions.ToInteger(opt);
                            pic_option2 = pic_option2 + " フィルタ=" + opt;
                        }
                        else
                        {
                            // XXX 変換したくない
                            BGColor = ColorTranslator.FromOle(Conversions.ToInteger(opt));
                            pic_option2 = pic_option2 + " " + opt;
                        }

                        break;
                }
            }

            pic_option = Strings.Trim(pic_option);
            pic_option2 = Strings.Trim(pic_option2);

            // 描画先を設定
            Graphics g = null;
            if (on_msg_window)
            {
                // メッセージウィンドウへのパイロット画像の描画
                frmMessage.picFace.NewImageIfNull();
                g = Graphics.FromImage(frmMessage.picFace.Image);
                permanent = false;
            }
            else if (on_status_window)
            {
                //// ステータスウィンドウへのパイロット画像の描画
                MainForm.picUnitStatus.NewImageIfNull();
                g = Graphics.FromImage(MainForm.picUnitStatus.Image);
            }
            else if (permanent)
            {
                //// 背景への描画
                MainForm.picBack.NewImageIfNull();
                g = Graphics.FromImage(MainForm.picBack.Image);
            }
            else
            {
                //// マップウィンドウへの通常の描画
                //pic = MainForm.picMain(0);
                SaveScreen();
                g = Graphics.FromImage(MainForm.MainBuffer);
            }
            if (g == null)
            {
                throw new NotSupportedException();
            }
            try
            {

                #region TODO
                //            // 読み込むファイルの探索

                //            // 前回の画像ファイルと同じ？
                //            if ((fname ?? "") == (last_fname ?? ""))
                //            {
                //                // 前回ファイルは見つかっていたのか？
                //                if (!last_exists)
                //                {
                //                    DrawPictureRet = false;
                //                    return DrawPictureRet;
                //                }
                //            }

                //            // 以前表示した拡大画像が利用可能？
                //            var loopTo = (SRC.ImageBufferSize - 1);
                //            for (i = 0; i <= loopTo; i++)
                //            {
                //                // 同じファイル？
                //                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                {
                //                    // オプションも同じ？
                //                    if ((PicBufOption[i] ?? "") == (pic_option ?? "") & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & !PicBufIsMask[i] & PicBufDW[i] == dw & PicBufDH[i] == dh & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                    {
                //                        // 同じファイル、オプションによる画像が見つかった

                //                        // 以前表示した画像をそのまま利用
                //                        UsePicBuf(i);
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock = orig_pic;
                //                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock.Width);
                //                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock.Height);
                //                        }
                //                        // Debug.Print "Reuse " & Format$(i) & " As Stretched"
                //                        goto EditedPicture;
                //                    }
                //                }
                //            }

                //            // 以前表示した画像が利用可能？
                //            var loopTo1 = (SRC.ImageBufferSize - 1);
                //            for (i = 0; i <= loopTo1; i++)
                //            {
                //                // 同じファイル？
                //                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                {
                //                    // オプションも同じ？
                //                    if ((PicBufOption[i] ?? "") == (pic_option ?? "") & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & !PicBufIsMask[i] & PicBufDW[i] ==  Constants.DEFAULT_LEVEL & PicBufDH[i] ==  Constants.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                    {
                //                        // 同じファイル、オプションによる画像が見つかった

                //                        // 以前表示した画像をそのまま利用
                //                        UsePicBuf(i);
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock1 = orig_pic;
                //                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock1.Width);
                //                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock1.Height);
                //                        }
                //                        // Debug.Print "Reuse " & Format$(i) & " As Edited"
                //                        found_orig = true;
                //                        goto EditedPicture;
                //                    }
                //                }
                //            }

                //            // 以前使用した部分画像が利用可能？
                //            if (sw != 0)
                //            {
                //                var loopTo2 = (SRC.ImageBufferSize - 1);
                //                for (i = 0; i <= loopTo2; i++)
                //                {
                //                    // 同じファイル？
                //                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                    {
                //                        if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] ==  Constants.DEFAULT_LEVEL & PicBufDH[i] ==  Constants.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                        {
                //                            // 以前使用した部分画像をそのまま利用
                //                            UsePicBuf(i);
                //                            orig_pic = MainForm.picBuf(i);
                //                            {
                //                                var withBlock2 = orig_pic;
                //                                orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock2.Width);
                //                                orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock2.Height);
                //                            }
                //                            // Debug.Print "Reuse " & Format$(i) & " As Partial"
                //                            goto LoadedOrigPicture;
                //                        }
                //                    }
                //                }
                //            }

                //            // 以前使用した原画像が利用可能？
                //            var loopTo3 = (SRC.ImageBufferSize - 1);
                //            for (i = 0; i <= loopTo3; i++)
                //            {
                //                // 同じファイル？
                //                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                {
                //                    if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] ==  Constants.DEFAULT_LEVEL & PicBufDH[i] ==  Constants.DEFAULT_LEVEL & PicBufSW[i] == 0)
                //                    {
                //                        // 以前使用した原画像をそのまま利用
                //                        UsePicBuf(i);
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock3 = orig_pic;
                //                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock3.Width);
                //                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock3.Height);
                //                        }
                //                        // Debug.Print "Reuse " & Format$(i) & " As Orig"
                //                        goto LoadedOrigPicture;
                //                    }
                //                }
                //            }

                //            // 特殊なファイル名
                //            switch (Strings.LCase(fname) ?? "")
                //            {
                //                case "black.bmp":
                //                case @"event\black.bmp":
                //                    {
                //                        // 黒で塗りつぶし
                //                        if (dx ==  Constants.DEFAULT_LEVEL)
                //                        {
                //                            dx = (int)((long)(SrcFormatter.PixelsToTwipsX(pic.Width) - dw) / 2L);
                //                        }

                //                        if (dy ==  Constants.DEFAULT_LEVEL)
                //                        {
                //                            dy = (int)((long)(SrcFormatter.PixelsToTwipsY(pic.Height) - dh) / 2L);
                //                        }
                //                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, BLACKNESS);
                //                        goto DrewPicture;
                //                        break;
                //                    }

                //                case "white.bmp":
                //                case @"event\white.bmp":
                //                    {
                //                        // 白で塗りつぶし
                //                        if (dx ==  Constants.DEFAULT_LEVEL)
                //                        {
                //                            dx = (int)((long)(SrcFormatter.PixelsToTwipsX(pic.Width) - dw) / 2L);
                //                        }

                //                        if (dy ==  Constants.DEFAULT_LEVEL)
                //                        {
                //                            dy = (int)((long)(SrcFormatter.PixelsToTwipsY(pic.Height) - dh) / 2L);
                //                        }
                //                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, WHITENESS);
                //                        goto DrewPicture;
                //                        break;
                //                    }

                //                case @"common\effect_tile(ally).bmp":
                //                case @"anime\common\effect_tile(ally).bmp":
                //                    {
                //                        // 味方ユニットタイル
                //                        orig_pic = MainForm.picUnit;
                //                        orig_width = 32;
                //                        orig_height = 32;
                //                        goto LoadedOrigPicture;
                //                        break;
                //                    }

                //                case @"common\effect_tile(enemy).bmp":
                //                case @"anime\common\effect_tile(enemy).bmp":
                //                    {
                //                        // 敵ユニットタイル
                //                        orig_pic = MainForm.picEnemy;
                //                        orig_width = 32;
                //                        orig_height = 32;
                //                        goto LoadedOrigPicture;
                //                        break;
                //                    }

                //                case @"common\effect_tile(neutral).bmp":
                //                case @"anime\common\effect_tile(neutral).bmp":
                //                    {
                //                        // 中立ユニットタイル
                //                        orig_pic = MainForm.picNeautral;
                //                        orig_width = 32;
                //                        orig_height = 32;
                //                        goto LoadedOrigPicture;
                //                        break;
                //                    }
                //            }

                //            // フルパスで指定されている？
                //            if (Strings.InStr(fname, ":") == 2)
                //            {
                //                fpath = "";
                //                last_path = "";
                //                // 登録を避けるため
                //                in_history = true;
                //                goto FoundPicture;

                //                // 履歴を検索してみる
                //            };
                //            // UPGRADE_WARNING: オブジェクト fpath_history.Item() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            fpath = Conversions.ToString(fpath_history[fname]);

                //            // 履歴上にファイルを発見
                //            last_path = "";
                //            ;
                //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 333274


                //            Input:

                //                    '履歴上にファイルを発見
                //                    On Error GoTo 0

                //             */
                //            if (string.IsNullOrEmpty(fpath))
                //            {
                //                // ファイルは存在しない
                //                last_fname = fname;
                //                last_exists = false;
                //                DrawPictureRet = false;
                //                return DrawPictureRet;
                //            }

                //            in_history = true;
                //            goto FoundPicture;


                //        // 履歴になかった
                //        NotFound:
                //            ;
                //            ;

                //            // 戦闘アニメ用？
                //            if (Strings.InStr(fname, @"\EFFECT_") > 0)
                //            {
                //                if (scenario_anime_bitmap_dir_exists)
                //                {
                //                    string argfname = SRC.ScenarioPath + @"Bitmap\Anime\" + fname;
                //                    if (GeneralLib.FileExists(argfname))
                //                    {
                //                        fpath = SRC.ScenarioPath + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata_anime_bitmap_dir_exists)
                //                {
                //                    string argfname1 = SRC.ExtDataPath + @"Bitmap\Anime\" + fname;
                //                    if (GeneralLib.FileExists(argfname1))
                //                    {
                //                        fpath = SRC.ExtDataPath + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata2_anime_bitmap_dir_exists)
                //                {
                //                    string argfname2 = SRC.ExtDataPath2 + @"Bitmap\Anime\" + fname;
                //                    if (GeneralLib.FileExists(argfname2))
                //                    {
                //                        fpath = SRC.ExtDataPath2 + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname3 = SRC.AppPath + @"Bitmap\Anime\" + fname;
                //                if (GeneralLib.FileExists(argfname3))
                //                {
                //                    fpath = SRC.AppPath + @"Bitmap\Anime\";
                //                    last_path = "";
                //                    goto FoundPicture;
                //                }
                //            }

                //            // 前回と同じパス？
                //            if (Strings.Len(last_path) > 0)
                //            {
                //                string argfname4 = last_path + fname;
                //                if (GeneralLib.FileExists(argfname4))
                //                {
                //                    fpath = last_path;
                //                    goto FoundPicture;
                //                }
                //            }

                //            // パス名入り？
                //            if (Strings.InStr(fname, @"Bitmap\") > 0)
                //            {
                //                if (scenario_bitmap_dir_exists)
                //                {
                //                    string argfname5 = SRC.ScenarioPath + fname;
                //                    if (GeneralLib.FileExists(argfname5))
                //                    {
                //                        fpath = SRC.ScenarioPath;
                //                        last_path = fpath;
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname6 = SRC.AppPath + fname;
                //                if (GeneralLib.FileExists(argfname6))
                //                {
                //                    fpath = SRC.AppPath;
                //                    last_path = "";
                //                    goto FoundPicture;
                //                }

                //                if (Strings.Mid(fname, 2, 1) == ":")
                //                {
                //                    fpath = "";
                //                    last_path = "";
                //                    goto FoundPicture;
                //                }
                //            }

                //            // フォルダ指定あり？
                //            if (Strings.InStr(fname, @"\") > 0)
                //            {
                //                if (scenario_bitmap_dir_exists)
                //                {
                //                    string argfname7 = SRC.ScenarioPath + @"Bitmap\" + fname;
                //                    if (GeneralLib.FileExists(argfname7))
                //                    {
                //                        fpath = SRC.ScenarioPath + @"Bitmap\";
                //                        last_path = fpath;
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata_bitmap_dir_exists)
                //                {
                //                    string argfname8 = SRC.ExtDataPath + @"Bitmap\" + fname;
                //                    if (GeneralLib.FileExists(argfname8))
                //                    {
                //                        fpath = SRC.ExtDataPath + @"Bitmap\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata2_bitmap_dir_exists)
                //                {
                //                    string argfname9 = SRC.ExtDataPath2 + @"Bitmap\" + fname;
                //                    if (GeneralLib.FileExists(argfname9))
                //                    {
                //                        fpath = SRC.ExtDataPath2 + @"Bitmap\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname10 = SRC.AppPath + @"Bitmap\" + fname;
                //                if (GeneralLib.FileExists(argfname10))
                //                {
                //                    fpath = SRC.AppPath + @"Bitmap\";
                //                    last_path = "";
                //                    goto FoundPicture;
                //                }

                //                if (Strings.LCase(Strings.Left(fname, 4)) == @"map\")
                //                {
                //                    tname = Strings.Mid(fname, 5);
                //                    if (Strings.InStr(tname, @"\") == 0)
                //                    {
                //                        i = (Strings.Len(tname) - 5);
                //                        while (i > 0)
                //                        {
                //                            if (LikeOperator.LikeString(Strings.Mid(tname, i, 1), "[!-0-9]", CompareMethod.Binary))
                //                            {
                //                                break;
                //                            }

                //                            i = (i - 1);
                //                        }

                //                        if (i > 0)
                //                        {
                //                            tdir = Strings.Left(tname, i) + @"\";
                //                            tnum = Strings.Mid(tname, i + 1, Strings.Len(tname) - i - 4);
                //                            tname = Strings.Left(tname, i) + SrcFormatter.Format(GeneralLib.StrToLng(tnum), "0000") + ".bmp";
                //                        }
                //                    }
                //                }
                //            }
                //            // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
                //            else if (LikeOperator.LikeString(fname, "*#.bmp", CompareMethod.Binary) & LikeOperator.LikeString(Strings.Left(fname, 1), "[a-z]", CompareMethod.Binary))
                //            {
                //                i = (Strings.Len(fname) - 5);
                //                while (i > 0)
                //                {
                //                    if (LikeOperator.LikeString(Strings.Mid(fname, i, 1), "[!-0-9]", CompareMethod.Binary))
                //                    {
                //                        break;
                //                    }

                //                    i = (i - 1);
                //                }

                //                if (i > 0)
                //                {
                //                    tdir = Strings.Left(fname, i);
                //                    {
                //                        var withBlock4 = SRC.TDList;
                //                        var loopTo4 = withBlock4.Count;
                //                        for (j = 1; j <= loopTo4; j++)
                //                        {
                //                            if (tdir == withBlock4.Item(withBlock4.OrderedID(j)).Bitmap)
                //                            {
                //                                tnum = Strings.Mid(fname, i + 1, Strings.Len(fname) - i - 4);
                //                                tname = Strings.Left(fname, i) + SrcFormatter.Format(GeneralLib.StrToLng(tnum), "0000") + ".bmp";
                //                                break;
                //                            }
                //                        }

                //                        if (j <= withBlock4.Count)
                //                        {
                //                            tdir = tdir + @"\";
                //                        }
                //                        else
                //                        {
                //                            tdir = "";
                //                        }
                //                    }
                //                }
                //            }
                #endregion

                // 各フォルダを検索する
                var image = transparent && BGColor == Color.White
                    ? imageBuffer.GetTransparent(fname)
                    : imageBuffer.Get(fname);
                if (image == null)
                {
                    // 表示を中止
                    return false;
                }

                //FoundPicture:
                //    // ファイル名を記録しておく
                //    last_fname = fname;
                //    last_exists = true;
                //    pfname = fpath + fname;
                //LoadedOrigPicture:

                var orig_width = image.Width;
                var orig_height = image.Height;
                // XXX 使用するpicBufを選択
                Bitmap drawBuffer = null;

                // 原画像の一部のみを描画？
                if (sw != 0)
                {
                    if (sw != orig_width || sh != orig_height)
                    {
                        // 原画像から描画部分をコピー
                        {
                            if (sx == Constants.DEFAULT_LEVEL)
                            {
                                sx = (orig_width - sw) / 2;
                            }

                            if (sy == Constants.DEFAULT_LEVEL)
                            {
                                sy = (orig_height - sh) / 2;
                            }
                            drawBuffer = new Bitmap(sw, sh);
                            using (var gBuf = Graphics.FromImage(drawBuffer))
                            {
                                gBuf.DrawImage(image, new Rectangle(0, 0, sw, sh), new Rectangle(sx, sy, sw, sh), GraphicsUnit.Pixel);
                            }
                        }
                        orig_width = sw;
                        orig_height = sh;
                    }
                }
                if (drawBuffer == null)
                {
                    drawBuffer = new Bitmap(image);
                }
                if (transparent && BGColor != Color.White)
                {
                    drawBuffer.MakeTransparent(BGColor);
                }

                //LoadedPicture:
                {
                    //    // 原画像を修正して使う場合は原画像を別のpicBufにコピーして修正する
                    //    if (top_part | bottom_part | left_part | right_part | tleft_part | tright_part | bleft_part | bright_part | is_monotone | is_sepia | is_sunset | is_water | negpos | is_sil | vrev | hrev | bright_count > 0 | dark_count > 0 | angle % 360 != 0 | is_colorfilter)
                    //    {
                    //        // 使用するpicBufを選択
                    //        i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                    //        PicBufFname[i] = fname;
                    //        PicBufOption[i] = pic_option;
                    //        PicBufOption2[i] = pic_option2;
                    //        PicBufDW[i] =  Constants.DEFAULT_LEVEL;
                    //        PicBufDH[i] =  Constants.DEFAULT_LEVEL;
                    //        PicBufSX[i] = sx;
                    //        PicBufSY[i] = sy;
                    //        PicBufSW[i] = sw;
                    //        PicBufSH[i] = sh;
                    //        PicBufIsMask[i] = false;
                    //        // Debug.Print "Use " & Format$(i) & " As Edited"

                    //        // 画像をコピー
                    //        {
                    //            var withBlock8 = MainForm.picBuf(i);
                    //            withBlock8.Picture = Image.FromFile("");
                    //            withBlock8.width = orig_width;
                    //            withBlock8.Height = orig_height;
                    //            ret = BitBlt(withBlock8.hDC, 0, 0, orig_width, orig_height, orig_pic.hDC, 0, 0, SRCCOPY);
                    //        }
                    //        orig_pic = MainForm.picBuf(i);
                    //    }

                    //    // 画像の一部を塗りつぶして描画する場合
                    //    if (top_part)
                    //    {
                    //        // 上半分
                    //        orig_pic.Line(0, orig_height / 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (bottom_part)
                    //    {
                    //        // 下半分
                    //        orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (left_part)
                    //    {
                    //        // 左半分
                    //        orig_pic.Line(orig_width / 2, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (right_part)
                    //    {
                    //        // 右半分
                    //        orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (tleft_part)
                    //    {
                    //        // 左上
                    //        var loopTo5 = (orig_width - 1);
                    //        for (i = 0; i <= loopTo5; i++)
                    //            orig_pic.Line(i, orig_height - 1 - i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (tright_part)
                    //    {
                    //        // 右上
                    //        var loopTo6 = (orig_width - 1);
                    //        for (i = 0; i <= loopTo6; i++)
                    //            orig_pic.Line(i, i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (bleft_part)
                    //    {
                    //        // 左下
                    //        var loopTo7 = (orig_width - 1);
                    //        for (i = 0; i <= loopTo7; i++)
                    //            orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    if (bright_part)
                    //    {
                    //        // 右下
                    //        var loopTo8 = (orig_width - 1);
                    //        for (i = 0; i <= loopTo8; i++)
                    //            orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //    }

                    //    // 特殊効果
                    //    if (is_monotone | is_sepia | is_sunset | is_water | is_colorfilter | bright_count > 0 | dark_count > 0 | negpos | is_sil | vrev | hrev | angle != 0)
                    //    {
                    //        // 画像のサイズをチェック
                    //        if (orig_width * orig_height % 4 != 0)
                    //        {
                    //            string argmsg = fname + "の画像サイズが4の倍数になっていません";
                    //            ErrorMessage(argmsg);
                    //            return DrawPictureRet;
                    //        }

                    //        // イメージをバッファに取り込み
                    //        Graphics.GetImage(orig_pic);

                    //        // 白黒
                    //        if (is_monotone)
                    //        {
                    //            Graphics.Monotone(transparent);
                    //        }

                    //        // セピア
                    //        if (is_sepia)
                    //        {
                    //            Graphics.Sepia(transparent);
                    //        }

                    //        // 夕焼け
                    //        if (is_sunset)
                    //        {
                    //            Graphics.Sunset(transparent);
                    //        }

                    //        // 水中
                    //        if (is_water)
                    //        {
                    //            Graphics.Water(transparent);
                    //        }

                    //        // シルエット
                    //        if (is_sil)
                    //        {
                    //            Graphics.Silhouette();
                    //        }

                    //        // ネガポジ反転
                    //        if (negpos)
                    //        {
                    //            Graphics.NegPosReverse(transparent);
                    //        }

                    //        // フィルタ
                    //        if (is_colorfilter)
                    //        {
                    //            if (trans_par < 0d)
                    //            {
                    //                trans_par = 0.5d;
                    //            }

                    //            Graphics.ColorFilter(fcolor, trans_par, transparent);
                    //        }

                    //        // 明 (多段指定可能)
                    //        var loopTo9 = bright_count;
                    //        for (i = 1; i <= loopTo9; i++)
                    //            Graphics.Bright(transparent);

                    //        // 暗 (多段指定可能)
                    //        var loopTo10 = dark_count;
                    //        for (i = 1; i <= loopTo10; i++)
                    //            Graphics.Dark(transparent);

                    //        // 左右反転
                    //        if (vrev)
                    //        {
                    //            Graphics.VReverse();
                    //        }

                    //        // 上下反転
                    //        if (hrev)
                    //        {
                    //            Graphics.HReverse();
                    //        }

                    //        // 回転
                    //        if (angle != 0)
                    //        {
                    //            // 前回の回転角が90度の倍数かどうかで描画の際の最適化使用可否を決める
                    //            // (連続で回転させる場合に描画速度を一定にするため)
                    //            Graphics.Rotate(angle, last_angle % 90 != 0);
                    //        }

                    //        // 変更した内容をイメージに変換
                    //        Graphics.SetImage(orig_pic);

                    //        // バッファを破棄
                    //        Graphics.ClearImage();
                    //    }

                    //    last_angle = angle;
                }
                //EditedPicture:

                // クリッピング処理
                if (dw == Constants.DEFAULT_LEVEL)
                {
                    dw = orig_width;
                }

                if (dh == Constants.DEFAULT_LEVEL)
                {
                    dh = orig_height;
                }

                if (permanent)
                {
                    // 背景描画の場合、センタリングはマップ中央に
                    if (dx == Constants.DEFAULT_LEVEL)
                    {
                        dx = (MapPWidth - dw) / 2;
                    }

                    if (dy == Constants.DEFAULT_LEVEL)
                    {
                        if (Map.IsStatusView)
                        {
                            // XXX
                            dy = (frmMain.MapCellPx * 15 - dh) / 2;
                        }
                        else
                        {
                            dy = (MapPHeight - dh) / 2;
                        }
                    }
                }
                // ユニット上で画像のセンタリングを行うことを意図している
                // 場合は修正が必要
                else if (Strings.InStr(fname, "EFFECT_") > 0 | Strings.InStr(fname, @"スペシャルパワー\") > 0 | Strings.InStr(fname, @"精神コマンド\") > 0)
                {
                    if (dx == Constants.DEFAULT_LEVEL)
                    {
                        dx = (MainPWidth - dw) / 2;
                        if (MainWidth % 2 == 0)
                        {
                            dx = dx - 16;
                        }
                    }

                    if (dy == Constants.DEFAULT_LEVEL)
                    {
                        dy = (MainPHeight - dh) / 2;
                        if (MainHeight % 2 == 0)
                        {
                            dy = dy - 16;
                        }
                    }
                }
                else
                {
                    // 通常描画の場合、センタリングは画面中央に
                    if (dx == Constants.DEFAULT_LEVEL)
                    {
                        dx = (MainPWidth - dw) / 2;
                    }

                    if (dy == Constants.DEFAULT_LEVEL)
                    {
                        dy = (MainPHeight - dh) / 2;
                    }
                }

                // 描画先が画面外の場合や描画サイズが0の場合は画像のロードのみを行う
                if (dx >= g.VisibleClipBounds.Width
                    || dy >= g.VisibleClipBounds.Height
                    || dx + dw <= 0
                    || dy + dh <= 0
                    || dw <= 0
                    || dh <= 0)
                {
                    //load_only = true;
                    return true;
                }
                g.DrawImage(drawBuffer, new Rectangle(dx, dy, dw, dh));

                //DrewPicture:
                if (permanent)
                {
                    // 背景への描き込み
                    Map.IsMapDirty = true;
                    // XXX マスクは動的にやってるから要らん
                    //// マスク入り背景画像画面にも画像を描き込む
                    //ret = BitBlt(withBlock17.picMaskedBack.hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                    //var loopTo15 = ((dx + dw - 1) / 32);
                    //for (i = (dx / 32); i <= loopTo15; i++)
                    //{
                    //    var loopTo16 = ((dy + dh - 1) / 32);
                    //    for (j = (dy / 32); j <= loopTo16; j++)
                    //    {
                    //        ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask.hDC, 0, 0, SRCAND);
                    //        ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask2.hDC, 0, 0, SRCINVERT);
                    //    }
                    //}
                }
                else if (!on_msg_window && !on_status_window)
                {
                    // 表示画像を消去する際に使う描画領域を設定
                    // XXX ダーティエリアは使わない
                    //PaintedAreaX1 = GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(dx, 0));
                    //PaintedAreaY1 = GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(dy, 0));
                    //PaintedAreaX2 = GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(dx + dw, MainPWidth - 1));
                    //PaintedAreaY2 = GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(dy + dh, MainPHeight - 1));
                    IsPictureDrawn = true;
                    IsPictureVisible = true;
                    IsCursorVisible = false;
                    if (keep_picture)
                    {
                        // picMain(1)にも描画
                        using (var gBack = Graphics.FromImage(MainForm.MainBufferBack))
                        {
                            gBack.DrawImage(drawBuffer, new Rectangle(dx, dy, dw, dh));
                        }
                    }
                }
            }
            finally
            {
                g.Dispose();
            }

            return true;
        }

        // ＭＳ Ｐ明朝、16pt、Bold、白色
        private Font currentFont = new Font("ＭＳ Ｐ明朝", 16, FontStyle.Bold, GraphicsUnit.Point);
        private Brush currentFontColor = Brushes.White;
        private PointF currentStringPoint = new PointF();

        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
            Graphics g;
            if (PermanentStringMode)
            {
                // 背景書き込み
                //// 背景への描画
                MainForm.picBack.NewImageIfNull();
                g = Graphics.FromImage(MainForm.picBack.Image);
                //// フォント設定を変更
                //{
                //    var withBlock = MainForm.picBack;
                //    withBlock.ForeColor = MainForm.picMain(0).ForeColor;
                //    if (withBlock.Font.Name != MainForm.picMain(0).Font.Name)
                //    {
                //        sf = (Font)Control.DefaultFont.Clone();
                //        sf = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeName(sf, MainForm.picMain(0).Font.Name);
                //        withBlock.Font = sf;
                //    }
                //    withBlock.Font.Size = MainForm.picMain(0).Font.Size;
                //    withBlock.Font.Bold = MainForm.picMain(0).Font.Bold;
                //    withBlock.Font.Italic = MainForm.picMain(0).Font.Italic;
                //}
                //{
                //    var withBlock1 = MainForm.picMaskedBack;
                //    withBlock1.ForeColor = MainForm.picMain(0).ForeColor;
                //    if (withBlock1.Font.Name != MainForm.picMain(0).Font.Name)
                //    {
                //        sf = (Font)Control.DefaultFont.Clone();
                //        sf = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeName(sf, MainForm.picMain(0).Font.Name);
                //        withBlock1.Font = sf;
                //    }
                //    withBlock1.Font.Size = MainForm.picMain(0).Font.Size;
                //    withBlock1.Font.Bold = MainForm.picMain(0).Font.Bold;
                //    withBlock1.Font.Italic = MainForm.picMain(0).Font.Italic;
                //}
            }
            else
            {
                //// マップウィンドウへの通常の描画
                // 通常の書き込み
                g = Graphics.FromImage(MainForm.MainBuffer);
                SaveScreen();
            }

            try
            {
                //// フォントがスムージング表示されているか参照
                //if (!init_draw_string)
                //{
                //    GUI.GetSystemParametersInfo((int)SPI_GETFONTSMOOTHING, 0, font_smoothing, 0);
                //    init_draw_string = true;
                //}

                //// フォントをスムージングするように設定
                //if (font_smoothing == 0)
                //{
                //    SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 1, 0, 0);
                //}
                // 現在のX位置を記録しておく
                var prev_cx = currentStringPoint.X;
                float tx = currentStringPoint.X;
                float ty = currentStringPoint.Y;
                var msgSize = g.MeasureString(msg, currentFont);
                // 書き込み先の座標を求める
                if (HCentering)
                {
                    tx = (g.VisibleClipBounds.Width - msgSize.Width) / 2;
                }
                else if (X != Constants.DEFAULT_LEVEL)
                {
                    tx = X;
                }

                if (VCentering)
                {
                    ty = (g.VisibleClipBounds.Height - msgSize.Height) / 2;
                }
                else if (Y != Constants.DEFAULT_LEVEL)
                {
                    ty = Y;
                }

                g.DrawString(msg, currentFont, currentFontColor, tx, ty);

                // 背景書き込みの場合
                if (PermanentStringMode)
                {
                    // XXX マスクは使ってない
                    //{
                    //    var withBlock2 = MainForm.picMaskedBack;
                    //    withBlock2.CurrentX = tx;
                    //    withBlock2.CurrentY = ty;
                    //}
                    //MainForm.picMaskedBack.Print(msg);
                    Map.IsMapDirty = true;
                }

                // 保持オプション使用時
                if (KeepStringMode)
                {
                    // picMain(1)にも描画
                    using (var gBack = Graphics.FromImage(MainForm.MainBufferBack))
                    {
                        gBack.DrawString(msg, currentFont, currentFontColor, tx, ty);
                    }
                }

                // 次回の書き込みのため、X座標位置を設定し直す
                // XXX 改行って高さどうなん？
                currentStringPoint = new PointF(
                    X != Constants.DEFAULT_LEVEL ? X : prev_cx,
                    without_cr ? ty : ty + msgSize.Height);

                //// フォントのスムージングに関する設定を元に戻す
                //if (font_smoothing == 0)
                //{
                //    SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 0, 0, 0);
                //}

                if (!PermanentStringMode)
                {
                    IsPictureVisible = true;
                    PaintedAreaX1 = 0;
                    PaintedAreaY1 = 0;
                    PaintedAreaX2 = (MainPWidth - 1);
                    PaintedAreaY2 = (MainPHeight - 1);
                }
            }
            finally
            {
                g.Dispose();
            }
        }
    }
}
