using SRCTestForm.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SRCTestForm
{
    public partial class frmTeatMain
    {
        // XXX ファイルシステム周りはインタフェースを切って環境毎に実装してやるのがよさそう。
        // SRCクラスにある XXDataPath もそちらに切り出すとよいはず。
        private bool init_draw_pitcure;
        private IList<string> existBitmapDirectories;
        private IList<string> existMapBitmapDirectories;

        // 画像をウィンドウに描画
        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            bool DrawPictureRet = default;
            string pic_option, opt, pic_option2;
            bool permanent = default, transparent = default;
            bool is_monotone = default, is_sepia = default;
            bool is_sunset = default, is_water = default;
            short bright_count = default, dark_count = default;
            bool is_sil = default, negpos = default;
            bool vrev = default, hrev = default;
            bool top_part = default, bottom_part = default;
            bool left_part = default, right_part = default;
            bool tleft_part = default, tright_part = default;
            bool bleft_part = default, bright_part = default;
            var angle = default(int);
            bool on_msg_window = default, on_status_window = default;
            var keep_picture = default(bool);
            int ret;
            short i, j;
            string pfname, fpath;
            PictureBox mask_pic = default;
            PictureBox stretched_pic, stretched_mask_pic = default;
            PictureBox orig_pic;
            int orig_width, orig_height;
            bool found_orig = default, in_history, load_only = default;
            var is_colorfilter = default(bool);
            var fcolor = default(int);
            double trans_par;
            string tnum, tdir, tname;

            // ダミーのファイル名？
            switch (fname ?? "")
            {
                case var @case when @case == "":
                case "-.bmp":
                case "EFFECT_Void.bmp":
                    {
                        return DrawPictureRet;
                    }
            }

            // Debug.Print fname, draw_option

            #region TODO
            //            // オプションの解析
            //            BGColor = ColorTranslator.ToOle(Color.White);
            //            // マスク画像に影響しないオプション
            //            pic_option = "";
            //            // マスク画像に影響するオプション
            //            pic_option2 = "";
            //            // フィルタ時の透過度を初期化
            //            trans_par = -1;
            //            i = 1;
            //            while (i <= GeneralLib.LLength(ref draw_option))
            //            {
            //                opt = GeneralLib.LIndex(ref draw_option, i);
            //                switch (opt ?? "")
            //                {
            //                    case "背景":
            //                        {
            //                            permanent = true;
            //                            // 背景書き込みで夜やセピア色のマップの場合は指定がなくても特殊効果を付ける
            //                            switch (Map.MapDrawMode ?? "")
            //                            {
            //                                case "夜":
            //                                    {
            //                                        dark_count = (short)(dark_count + 1);
            //                                        pic_option = pic_option + " 暗";
            //                                        break;
            //                                    }

            //                                case "白黒":
            //                                    {
            //                                        is_monotone = true;
            //                                        pic_option = pic_option + " 白黒";
            //                                        break;
            //                                    }

            //                                case "セピア":
            //                                    {
            //                                        is_sepia = true;
            //                                        pic_option = pic_option + " セピア";
            //                                        break;
            //                                    }

            //                                case "夕焼け":
            //                                    {
            //                                        is_sunset = true;
            //                                        pic_option = pic_option + " 夕焼け";
            //                                        break;
            //                                    }

            //                                case "水中":
            //                                    {
            //                                        is_water = true;
            //                                        pic_option = pic_option + " 水中";
            //                                        break;
            //                                    }

            //                                case "フィルタ":
            //                                    {
            //                                        is_colorfilter = true;
            //                                        fcolor = Map.MapDrawFilterColor;
            //                                        pic_option2 = pic_option2 + " フィルタ=" + Map.MapDrawFilterColor.ToString();
            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }

            //                    case "透過":
            //                        {
            //                            transparent = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "白黒":
            //                        {
            //                            is_monotone = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "セピア":
            //                        {
            //                            is_sepia = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "夕焼け":
            //                        {
            //                            is_sunset = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "水中":
            //                        {
            //                            is_water = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "明":
            //                        {
            //                            bright_count = (short)(bright_count + 1);
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "暗":
            //                        {
            //                            dark_count = (short)(dark_count + 1);
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "左右反転":
            //                        {
            //                            hrev = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "上下反転":
            //                        {
            //                            vrev = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "ネガポジ反転":
            //                        {
            //                            negpos = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "シルエット":
            //                        {
            //                            is_sil = true;
            //                            pic_option = pic_option + " " + opt;
            //                            break;
            //                        }

            //                    case "上半分":
            //                        {
            //                            top_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "下半分":
            //                        {
            //                            bottom_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "右半分":
            //                        {
            //                            right_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "左半分":
            //                        {
            //                            left_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "右上":
            //                        {
            //                            tright_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "左上":
            //                        {
            //                            tleft_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "右下":
            //                        {
            //                            bright_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "左下":
            //                        {
            //                            bleft_part = true;
            //                            pic_option2 = pic_option2 + " " + opt;
            //                            break;
            //                        }

            //                    case "メッセージ":
            //                        {
            //                            on_msg_window = true;
            //                            break;
            //                        }

            //                    case "ステータス":
            //                        {
            //                            on_status_window = true;
            //                            break;
            //                        }

            //                    case "保持":
            //                        {
            //                            keep_picture = true;
            //                            break;
            //                        }

            //                    case "右回転":
            //                        {
            //                            i = (short)(i + 1);
            //                            string argexpr = GeneralLib.LIndex(ref draw_option, i);
            //                            angle = GeneralLib.StrToLng(ref argexpr);
            //                            pic_option2 = pic_option2 + " 右回転=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(angle % 360);
            //                            break;
            //                        }

            //                    case "左回転":
            //                        {
            //                            i = (short)(i + 1);
            //                            int localStrToLng() { string argexpr = GeneralLib.LIndex(ref draw_option, i); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                            angle = -localStrToLng();
            //                            pic_option2 = pic_option2 + " 右回転=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(angle % 360);
            //                            break;
            //                        }

            //                    case "フィルタ":
            //                        {
            //                            is_colorfilter = true;
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (Strings.Right(opt, 1) == "%" & Information.IsNumeric(Strings.Left(opt, Strings.Len(opt) - 1)))
            //                            {
            //                                trans_par = GeneralLib.MaxDbl(0d, GeneralLib.MinDbl(1d, Conversions.ToDouble(Strings.Left(opt, Strings.Len(opt) - 1)) / 100d));
            //                                pic_option2 = pic_option2 + " フィルタ透過度=" + opt;
            //                            }
            //                            else if (is_colorfilter)
            //                            {
            //                                fcolor = Conversions.ToInteger(opt);
            //                                pic_option2 = pic_option2 + " フィルタ=" + opt;
            //                            }
            //                            else
            //                            {
            //                                BGColor = Conversions.ToInteger(opt);
            //                                pic_option2 = pic_option2 + " " + opt;
            //                            }

            //                            break;
            //                        }
            //                }

            //                i = (short)(i + 1);
            //            }

            //            pic_option = Strings.Trim(pic_option);
            //            pic_option2 = Strings.Trim(pic_option2);

            #endregion


            on_msg_window = true;

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
                //            var loopTo = (short)(SRC.ImageBufferSize - 1);
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
                //                        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock = orig_pic;
                //                            orig_width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width);
                //                            orig_height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height);
                //                        }
                //                        // Debug.Print "Reuse " & Format$(i) & " As Stretched"
                //                        goto EditedPicture;
                //                    }
                //                }
                //            }

                //            // 以前表示した画像が利用可能？
                //            var loopTo1 = (short)(SRC.ImageBufferSize - 1);
                //            for (i = 0; i <= loopTo1; i++)
                //            {
                //                // 同じファイル？
                //                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                {
                //                    // オプションも同じ？
                //                    if ((PicBufOption[i] ?? "") == (pic_option ?? "") & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                    {
                //                        // 同じファイル、オプションによる画像が見つかった

                //                        // 以前表示した画像をそのまま利用
                //                        UsePicBuf(i);
                //                        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock1 = orig_pic;
                //                            orig_width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock1.Width);
                //                            orig_height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock1.Height);
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
                //                var loopTo2 = (short)(SRC.ImageBufferSize - 1);
                //                for (i = 0; i <= loopTo2; i++)
                //                {
                //                    // 同じファイル？
                //                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                    {
                //                        if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                        {
                //                            // 以前使用した部分画像をそのまま利用
                //                            UsePicBuf(i);
                //                            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                            orig_pic = MainForm.picBuf(i);
                //                            {
                //                                var withBlock2 = orig_pic;
                //                                orig_width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock2.Width);
                //                                orig_height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock2.Height);
                //                            }
                //                            // Debug.Print "Reuse " & Format$(i) & " As Partial"
                //                            goto LoadedOrigPicture;
                //                        }
                //                    }
                //                }
                //            }

                //            // 以前使用した原画像が利用可能？
                //            var loopTo3 = (short)(SRC.ImageBufferSize - 1);
                //            for (i = 0; i <= loopTo3; i++)
                //            {
                //                // 同じファイル？
                //                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //                {
                //                    if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSW[i] == 0)
                //                    {
                //                        // 以前使用した原画像をそのまま利用
                //                        UsePicBuf(i);
                //                        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                        orig_pic = MainForm.picBuf(i);
                //                        {
                //                            var withBlock3 = orig_pic;
                //                            orig_width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock3.Width);
                //                            orig_height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock3.Height);
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
                //                        if (dx == SRC.DEFAULT_LEVEL)
                //                        {
                //                            dx = (int)((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width) - dw) / 2L);
                //                        }

                //                        if (dy == SRC.DEFAULT_LEVEL)
                //                        {
                //                            dy = (int)((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height) - dh) / 2L);
                //                        }
                //                        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, BLACKNESS);
                //                        goto DrewPicture;
                //                        break;
                //                    }

                //                case "white.bmp":
                //                case @"event\white.bmp":
                //                    {
                //                        // 白で塗りつぶし
                //                        if (dx == SRC.DEFAULT_LEVEL)
                //                        {
                //                            dx = (int)((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width) - dw) / 2L);
                //                        }

                //                        if (dy == SRC.DEFAULT_LEVEL)
                //                        {
                //                            dy = (int)((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height) - dh) / 2L);
                //                        }
                //                        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, WHITENESS);
                //                        goto DrewPicture;
                //                        break;
                //                    }

                //                case @"common\effect_tile(ally).bmp":
                //                case @"anime\common\effect_tile(ally).bmp":
                //                    {
                //                        // 味方ユニットタイル
                //                        // UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
                //                        // UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
                //                        // UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
                //                    if (GeneralLib.FileExists(ref argfname))
                //                    {
                //                        fpath = SRC.ScenarioPath + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata_anime_bitmap_dir_exists)
                //                {
                //                    string argfname1 = SRC.ExtDataPath + @"Bitmap\Anime\" + fname;
                //                    if (GeneralLib.FileExists(ref argfname1))
                //                    {
                //                        fpath = SRC.ExtDataPath + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata2_anime_bitmap_dir_exists)
                //                {
                //                    string argfname2 = SRC.ExtDataPath2 + @"Bitmap\Anime\" + fname;
                //                    if (GeneralLib.FileExists(ref argfname2))
                //                    {
                //                        fpath = SRC.ExtDataPath2 + @"Bitmap\Anime\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname3 = SRC.AppPath + @"Bitmap\Anime\" + fname;
                //                if (GeneralLib.FileExists(ref argfname3))
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
                //                if (GeneralLib.FileExists(ref argfname4))
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
                //                    if (GeneralLib.FileExists(ref argfname5))
                //                    {
                //                        fpath = SRC.ScenarioPath;
                //                        last_path = fpath;
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname6 = SRC.AppPath + fname;
                //                if (GeneralLib.FileExists(ref argfname6))
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
                //                    if (GeneralLib.FileExists(ref argfname7))
                //                    {
                //                        fpath = SRC.ScenarioPath + @"Bitmap\";
                //                        last_path = fpath;
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata_bitmap_dir_exists)
                //                {
                //                    string argfname8 = SRC.ExtDataPath + @"Bitmap\" + fname;
                //                    if (GeneralLib.FileExists(ref argfname8))
                //                    {
                //                        fpath = SRC.ExtDataPath + @"Bitmap\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                if (extdata2_bitmap_dir_exists)
                //                {
                //                    string argfname9 = SRC.ExtDataPath2 + @"Bitmap\" + fname;
                //                    if (GeneralLib.FileExists(ref argfname9))
                //                    {
                //                        fpath = SRC.ExtDataPath2 + @"Bitmap\";
                //                        last_path = "";
                //                        goto FoundPicture;
                //                    }
                //                }

                //                string argfname10 = SRC.AppPath + @"Bitmap\" + fname;
                //                if (GeneralLib.FileExists(ref argfname10))
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
                //                        i = (short)(Strings.Len(tname) - 5);
                //                        while (i > 0)
                //                        {
                //                            if (LikeOperator.LikeString(Strings.Mid(tname, i, 1), "[!-0-9]", CompareMethod.Binary))
                //                            {
                //                                break;
                //                            }

                //                            i = (short)(i - 1);
                //                        }

                //                        if (i > 0)
                //                        {
                //                            tdir = Strings.Left(tname, i) + @"\";
                //                            tnum = Strings.Mid(tname, i + 1, Strings.Len(tname) - i - 4);
                //                            tname = Strings.Left(tname, i) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.StrToLng(ref tnum), "0000") + ".bmp";
                //                        }
                //                    }
                //                }
                //            }
                //            // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
                //            else if (LikeOperator.LikeString(fname, "*#.bmp", CompareMethod.Binary) & LikeOperator.LikeString(Strings.Left(fname, 1), "[a-z]", CompareMethod.Binary))
                //            {
                //                i = (short)(Strings.Len(fname) - 5);
                //                while (i > 0)
                //                {
                //                    if (LikeOperator.LikeString(Strings.Mid(fname, i, 1), "[!-0-9]", CompareMethod.Binary))
                //                    {
                //                        break;
                //                    }

                //                    i = (short)(i - 1);
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
                //                                tname = Strings.Left(fname, i) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.StrToLng(ref tnum), "0000") + ".bmp";
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
                var image = imageBuffer.Get(fname);
                // XXX とりあえず宛先のサイズに合わせて描く
                g.DrawImage(image, g.VisibleClipBounds);

                #region TODO
                //    // 表示を中止
                //    last_fname = fname;
                //    last_exists = false;
                //    DrawPictureRet = false;
                //    return DrawPictureRet;
                //FoundPicture:
                //    ;


                //    // ファイル名を記録しておく
                //    last_fname = fname;


                //    last_exists = true;
                //    pfname = fpath + fname;

                //    // 使用するバッファを選択
                //    i = GetPicBuf();
                //    // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //    orig_pic = MainForm.picBuf(i);
                //    PicBufFname[i] = fname;
                //    PicBufOption[i] = "";
                //    PicBufOption2[i] = "";
                //    PicBufDW[i] = SRC.DEFAULT_LEVEL;
                //    PicBufDH[i] = SRC.DEFAULT_LEVEL;
                //    PicBufSX[i] = 0;
                //    PicBufSY[i] = 0;
                //    PicBufSW[i] = 0;
                //    PicBufSH[i] = 0;
                //    PicBufIsMask[i] = false;
                //    // Debug.Print "Use " & Format$(i) & " As Orig"

                //    Susie.LoadPicture2(ref orig_pic, ref pfname);

                //    // 読み込んだ画像のサイズ(バイト数)をバッファ情報に記録しておく
                //    {
                //        var withBlock5 = orig_pic;
                //        PicBufSize[i] = (int)((double)display_byte_pixel * Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock5.Width) * Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock5.Height));
                //    }

                //LoadedOrigPicture:
                //    ;
                //    {
                //        var withBlock6 = orig_pic;
                //        orig_width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock6.Width);
                //        orig_height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock6.Height);
                //    }

                //    // 原画像の一部のみを描画？
                //    if (sw != 0)
                //    {
                //        if (sw != orig_width | sh != orig_height)
                //        {
                //            // 使用するpicBufを選択
                //            i = GUI.GetPicBuf(display_byte_pixel * sw * sh);
                //            PicBufFname[i] = fname;
                //            PicBufOption[i] = "";
                //            PicBufOption2[i] = "";
                //            PicBufDW[i] = SRC.DEFAULT_LEVEL;
                //            PicBufDH[i] = SRC.DEFAULT_LEVEL;
                //            PicBufSX[i] = (short)sx;
                //            PicBufSY[i] = (short)sy;
                //            PicBufSW[i] = (short)sw;
                //            PicBufSH[i] = (short)sh;
                //            PicBufIsMask[i] = false;
                //            // Debug.Print "Use " & Format$(i) & " As Partial"

                //            // 原画像から描画部分をコピー
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            {
                //                var withBlock7 = MainForm.picBuf(i);
                //                // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                withBlock7.Picture = Image.FromFile("");
                //                // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                withBlock7.width = sw;
                //                // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                withBlock7.Height = sh;
                //                if (sx == SRC.DEFAULT_LEVEL)
                //                {
                //                    sx = (orig_width - sw) / 2;
                //                }

                //                if (sy == SRC.DEFAULT_LEVEL)
                //                {
                //                    sy = (orig_height - sh) / 2;
                //                }
                //                // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //                // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                ret = BitBlt(withBlock7.hDC, 0, 0, sw, sh, orig_pic.hDC, sx, sy, SRCCOPY);
                //            }

                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            orig_pic = MainForm.picBuf(i);
                //            orig_width = sw;
                //            orig_height = sh;
                //        }
                //    }

                //LoadedPicture:
                //    ;


                //    // 原画像を修正して使う場合は原画像を別のpicBufにコピーして修正する
                //    if (top_part | bottom_part | left_part | right_part | tleft_part | tright_part | bleft_part | bright_part | is_monotone | is_sepia | is_sunset | is_water | negpos | is_sil | vrev | hrev | bright_count > 0 | dark_count > 0 | angle % 360 != 0 | is_colorfilter)
                //    {
                //        // 使用するpicBufを選択
                //        i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                //        PicBufFname[i] = fname;
                //        PicBufOption[i] = pic_option;
                //        PicBufOption2[i] = pic_option2;
                //        PicBufDW[i] = SRC.DEFAULT_LEVEL;
                //        PicBufDH[i] = SRC.DEFAULT_LEVEL;
                //        PicBufSX[i] = (short)sx;
                //        PicBufSY[i] = (short)sy;
                //        PicBufSW[i] = (short)sw;
                //        PicBufSH[i] = (short)sh;
                //        PicBufIsMask[i] = false;
                //        // Debug.Print "Use " & Format$(i) & " As Edited"

                //        // 画像をコピー
                //        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        {
                //            var withBlock8 = MainForm.picBuf(i);
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock8.Picture = Image.FromFile("");
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock8.width = orig_width;
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock8.Height = orig_height;
                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            ret = BitBlt(withBlock8.hDC, 0, 0, orig_width, orig_height, orig_pic.hDC, 0, 0, SRCCOPY);
                //        }
                //        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        orig_pic = MainForm.picBuf(i);
                //    }

                //    // 画像の一部を塗りつぶして描画する場合
                //    if (top_part)
                //    {
                //        // 上半分
                //        // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        orig_pic.Line(0, orig_height / 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (bottom_part)
                //    {
                //        // 下半分
                //        // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (left_part)
                //    {
                //        // 左半分
                //        // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        orig_pic.Line(orig_width / 2, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (right_part)
                //    {
                //        // 右半分
                //        // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (tleft_part)
                //    {
                //        // 左上
                //        var loopTo5 = (short)(orig_width - 1);
                //        for (i = 0; i <= loopTo5; i++)
                //            // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            orig_pic.Line(i, orig_height - 1 - i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (tright_part)
                //    {
                //        // 右上
                //        var loopTo6 = (short)(orig_width - 1);
                //        for (i = 0; i <= loopTo6; i++)
                //            // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            orig_pic.Line(i, i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (bleft_part)
                //    {
                //        // 左下
                //        var loopTo7 = (short)(orig_width - 1);
                //        for (i = 0; i <= loopTo7; i++)
                //            // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    if (bright_part)
                //    {
                //        // 右下
                //        var loopTo8 = (short)(orig_width - 1);
                //        for (i = 0; i <= loopTo8; i++)
                //            // UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                //    }

                //    // 特殊効果
                //    if (is_monotone | is_sepia | is_sunset | is_water | is_colorfilter | bright_count > 0 | dark_count > 0 | negpos | is_sil | vrev | hrev | angle != 0)
                //    {
                //        // 画像のサイズをチェック
                //        if (orig_width * orig_height % 4 != 0)
                //        {
                //            string argmsg = fname + "の画像サイズが4の倍数になっていません";
                //            ErrorMessage(ref argmsg);
                //            return DrawPictureRet;
                //        }

                //        // イメージをバッファに取り込み
                //        Graphics.GetImage(ref orig_pic);

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

                //            Graphics.ColorFilter(ref fcolor, ref trans_par, transparent);
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
                //        Graphics.SetImage(ref orig_pic);

                //        // バッファを破棄
                //        Graphics.ClearImage();
                //    }

                //    last_angle = angle;
                //EditedPicture:
                //    ;


                //    // クリッピング処理
                //    if (dw == SRC.DEFAULT_LEVEL)
                //    {
                //        dw = orig_width;
                //    }

                //    if (dh == SRC.DEFAULT_LEVEL)
                //    {
                //        dh = orig_height;
                //    }

                //    if (permanent)
                //    {
                //        // 背景描画の場合、センタリングはマップ中央に
                //        if (dx == SRC.DEFAULT_LEVEL)
                //        {
                //            dx = (MapPWidth - dw) / 2;
                //        }

                //        if (dy == SRC.DEFAULT_LEVEL)
                //        {
                //            if (string.IsNullOrEmpty(Map.MapFileName))
                //            {
                //                dy = (32 * 15 - dh) / 2;
                //            }
                //            else
                //            {
                //                dy = (MapPHeight - dh) / 2;
                //            }
                //        }
                //    }
                //    // ユニット上で画像のセンタリングを行うことを意図している
                //    // 場合は修正が必要
                //    else if (Strings.InStr(fname, "EFFECT_") > 0 | Strings.InStr(fname, @"スペシャルパワー\") > 0 | Strings.InStr(fname, @"精神コマンド\") > 0)
                //    {
                //        if (dx == SRC.DEFAULT_LEVEL)
                //        {
                //            dx = (MainPWidth - dw) / 2;
                //            if (MainWidth % 2 == 0)
                //            {
                //                dx = dx - 16;
                //            }
                //        }

                //        if (dy == SRC.DEFAULT_LEVEL)
                //        {
                //            dy = (MainPHeight - dh) / 2;
                //            if (MainHeight % 2 == 0)
                //            {
                //                dy = dy - 16;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // 通常描画の場合、センタリングは画面中央に
                //        if (dx == SRC.DEFAULT_LEVEL)
                //        {
                //            dx = (MainPWidth - dw) / 2;
                //        }

                //        if (dy == SRC.DEFAULT_LEVEL)
                //        {
                //            dy = (MainPHeight - dh) / 2;
                //        }
                //    }

                //    // 描画先が画面外の場合や描画サイズが0の場合は画像のロードのみを行う
                //    if (dx >= Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width) | dy >= Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height) | dx + dw <= 0 | dy + dh <= 0 | dw <= 0 | dh <= 0)
                //    {
                //        load_only = true;
                //    }

                //    // 描画を最適化するため、描画方法を細かく分けている。
                //    // 描画方法は以下の通り。
                //    // (1) BitBltでそのまま描画 (拡大処理なし、透過処理なし)
                //    // (2) 拡大画像を作ってからバッファリングして描画 (拡大処理あり、透過処理なし)
                //    // (3) 拡大画像を作らずにStretchBltで直接拡大描画 (拡大処理あり、透過処理なし)
                //    // (4) TransparentBltで拡大透過描画 (拡大処理あり、透過処理あり)
                //    // (5) 原画像をそのまま透過描画 (拡大処理なし、透過処理あり)
                //    // (6) 拡大画像を作ってからバッファリングして透過描画 (拡大処理あり、透過処理あり)
                //    // (7) 拡大画像を作ってからバッファリングせずに透過描画 (拡大処理あり、透過処理あり)
                //    // (8) 拡大画像を作らずにStretchBltで直接拡大透過描画 (拡大処理あり、透過処理あり)

                //    // 画面に描画する
                //    if (!transparent & dw == orig_width & dh == orig_height)
                //    {
                //        // 原画像をそのまま描画

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を描画先に描画
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCCOPY);
                //    }
                //    else if (SRC.KeepStretchedImage & !transparent & (!found_orig | load_only) & dw <= 480 & dh <= 480)
                //    {
                //        // 拡大画像を作成し、バッファリングして描画

                //        // 拡大画像に使用するpicBufを選択
                //        i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                //        PicBufFname[i] = fname;
                //        PicBufIsMask[i] = false;
                //        PicBufOption[i] = pic_option;
                //        PicBufOption2[i] = pic_option2;
                //        PicBufDW[i] = (short)dw;
                //        PicBufDH[i] = (short)dh;
                //        PicBufSX[i] = (short)sx;
                //        PicBufSY[i] = (short)sy;
                //        PicBufSW[i] = (short)sw;
                //        PicBufSH[i] = (short)sh;
                //        // Debug.Print "Use " & Format$(i) & " As Stretched"

                //        // バッファの初期化
                //        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        stretched_pic = MainForm.picBuf(i);
                //        {
                //            var withBlock9 = stretched_pic;
                //            withBlock9.Image = Image.FromFile("");
                //            withBlock9.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(dw);
                //            withBlock9.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(dh);
                //        }

                //        // バッファに拡大した画像を保存
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 拡大した画像を描画先に描画
                //        // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCCOPY);
                //    }
                //    else if (!transparent)
                //    {
                //        // 拡大画像を作らずにStretchBltで直接拡大描画

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 拡大した画像を描画先に描画
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);
                //    }
                //    else if (SRC.UseTransparentBlt & (dw != orig_width | dh != orig_height) & found_orig & !load_only & (dw * dh < 40000 | orig_width * orig_height > 40000))
                //    {
                //        // TransparentBltの方が高速に描画できる場合に限り
                //        // TransparentBltを使って拡大透過描画

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を描画先に透過描画
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = TransparentBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, BGColor);
                //    }
                //    else if (dw == orig_width & dh == orig_height)
                //    {
                //        // 原画像をそのまま透過描画

                //        // 以前使用したマスク画像が利用可能？
                //        // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        mask_pic.Image = null;
                //        var loopTo12 = (short)(SRC.ImageBufferSize - 1);
                //        for (i = 0; i <= loopTo12; i++)
                //        {
                //            // 同じファイル？
                //            if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //            {
                //                // オプションも同じ？
                //                if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                {
                //                    // 以前使用したマスク画像をそのまま利用
                //                    UsePicBuf(i);
                //                    // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    mask_pic = MainForm.picBufmask_pic(i);
                //                    // Debug.Print "Reuse " & Format$(i) & " As Mask"
                //                    break;
                //                }
                //            }
                //        }

                //        if (mask_pic is null)
                //        {
                //            // マスク画像を新規に作成

                //            // マスク画像に使用するpicBufを選択
                //            i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                //            PicBufFname[i] = fname;
                //            PicBufIsMask[i] = true;
                //            PicBufOption[i] = "";
                //            PicBufOption2[i] = pic_option2;
                //            PicBufDW[i] = (short)orig_width;
                //            PicBufDH[i] = (short)orig_height;
                //            PicBufSX[i] = (short)sx;
                //            PicBufSY[i] = (short)sy;
                //            PicBufSW[i] = (short)sw;
                //            PicBufSH[i] = (short)sh;
                //            // Debug.Print "Use " & Format$(i) & " As Mask"

                //            // バッファの初期化
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            mask_pic = MainForm.picBuf(i);
                //            {
                //                var withBlock10 = mask_pic;
                //                withBlock10.Image = Image.FromFile("");
                //                withBlock10.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(orig_width);
                //                withBlock10.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(orig_height);
                //            }

                //            // マスク画像を作成
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                //        }

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を透過描画
                //        if (BGColor == ColorTranslator.ToOle(Color.White))
                //        {
                //            // 背景色が白
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCINVERT);
                //        }
                //        else
                //        {
                //            // 背景色が白以外
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCAND);

                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(mask_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCINVERT);

                //            // マスク画像が再利用できないのでバッファを開放
                //            ReleasePicBuf(i);
                //        }
                //    }
                //    else if (SRC.KeepStretchedImage & (!found_orig | load_only) & dw <= 480 & dh <= 480)
                //    {
                //        // 拡大画像を作成し、バッファリングして透過描画

                //        // 拡大画像用に使用するpicBufを選択
                //        i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                //        PicBufFname[i] = fname;
                //        PicBufIsMask[i] = false;
                //        PicBufOption[i] = pic_option;
                //        PicBufOption2[i] = pic_option2;
                //        PicBufDW[i] = (short)dw;
                //        PicBufDH[i] = (short)dh;
                //        PicBufSX[i] = (short)sx;
                //        PicBufSY[i] = (short)sy;
                //        PicBufSW[i] = (short)sw;
                //        PicBufSH[i] = (short)sh;
                //        // Debug.Print "Use " & Format$(i) & " As Stretched"

                //        // バッファの初期化
                //        // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        stretched_pic = MainForm.picBuf(i);
                //        {
                //            var withBlock11 = stretched_pic;
                //            withBlock11.Image = Image.FromFile("");
                //            withBlock11.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(dw);
                //            withBlock11.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(dh);
                //        }

                //        // バッファに拡大した画像を保存
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                //        // 以前使用した拡大マスク画像が利用可能？
                //        // UPGRADE_NOTE: オブジェクト stretched_mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        stretched_mask_pic.Image = null;
                //        var loopTo13 = (short)(SRC.ImageBufferSize - 1);
                //        for (i = 0; i <= loopTo13; i++)
                //        {
                //            // 同じファイル？
                //            if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //            {
                //                // オプションも同じ？
                //                if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == dw & PicBufDH[i] == dh & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                {
                //                    // 以前使用した拡大マスク画像をそのまま利用
                //                    UsePicBuf(i);
                //                    // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    stretched_mask_pic = MainForm.picBuf(i);
                //                    // Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
                //                    break;
                //                }
                //            }
                //        }

                //        if (stretched_mask_pic is null)
                //        {
                //            // 拡大マスク画像を新規に作成

                //            // マスク画像用の領域を初期化
                //            // UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            mask_pic = MainForm.picTmp;
                //            {
                //                var withBlock12 = mask_pic;
                //                withBlock12.Image = Image.FromFile("");
                //                withBlock12.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(orig_width);
                //                withBlock12.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(orig_height);
                //            }

                //            // マスク画像を作成
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);

                //            // 拡大マスク画像に使用するpicBufを選択
                //            i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                //            PicBufFname[i] = fname;
                //            PicBufIsMask[i] = true;
                //            PicBufOption[i] = "";
                //            PicBufOption2[i] = pic_option2;
                //            PicBufDW[i] = (short)dw;
                //            PicBufDH[i] = (short)dh;
                //            PicBufSX[i] = (short)sx;
                //            PicBufSY[i] = (short)sy;
                //            PicBufSW[i] = (short)sw;
                //            PicBufSH[i] = (short)sh;
                //            // Debug.Print "Use " & Format$(i) & " As StretchedMask"

                //            // バッファを初期化
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            stretched_mask_pic = MainForm.picBuf(i);
                //            {
                //                var withBlock13 = stretched_mask_pic;
                //                withBlock13.Image = Image.FromFile("");
                //                withBlock13.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(dw);
                //                withBlock13.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(dh);
                //            }

                //            // バッファに拡大したマスク画像を保存
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);
                //        }

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を透過描画
                //        if (BGColor == ColorTranslator.ToOle(Color.White))
                //        {
                //            // 背景色が白
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT);
                //        }
                //        else
                //        {
                //            // 背景色が白以外
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT);

                //            // 拡大マスク画像が再利用できないのでバッファを開放
                //            ReleasePicBuf(i);
                //        }
                //    }
                //    else if (dw <= 480 & dh <= 480)
                //    {
                //        // 拡大画像を作成した後、バッファリングせずに透過描画

                //        // 拡大画像用の領域を作成
                //        // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        stretched_pic = MainForm.picStretchedTmp(0);
                //        stretched_pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(dw);
                //        stretched_pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(dh);

                //        // バッファに拡大した画像を保存
                //        // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                //        // 以前使用したマスク画像が利用可能？
                //        // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        mask_pic.Image = null;
                //        var loopTo14 = (short)(SRC.ImageBufferSize - 1);
                //        for (i = 0; i <= loopTo14; i++)
                //        {
                //            // 同じファイル？
                //            if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //            {
                //                // オプションも同じ？
                //                if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                {
                //                    // 以前使用したマスク画像をそのまま利用
                //                    UsePicBuf(i);
                //                    // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    mask_pic = MainForm.picBuf(i);
                //                    // Debug.Print "Reuse " & Format$(i) & " As Mask"
                //                    break;
                //                }
                //            }
                //        }

                //        if (mask_pic is null)
                //        {
                //            // 新規にマスク画像作成

                //            // マスク画像に使用するpicBufを選択
                //            i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                //            PicBufFname[i] = fname;
                //            PicBufIsMask[i] = true;
                //            PicBufOption[i] = "";
                //            PicBufOption2[i] = pic_option2;
                //            PicBufDW[i] = (short)orig_width;
                //            PicBufDH[i] = (short)orig_height;
                //            PicBufSX[i] = (short)sx;
                //            PicBufSY[i] = (short)sy;
                //            PicBufSW[i] = (short)sw;
                //            PicBufSH[i] = (short)sh;
                //            // Debug.Print "Use " & Format$(i) & " As Mask"

                //            // バッファを初期化
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            mask_pic = MainForm.picBuf(i);
                //            {
                //                var withBlock14 = mask_pic;
                //                withBlock14.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(orig_width);
                //                withBlock14.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(orig_height);
                //            }

                //            // マスク画像を作成
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                //        }

                //        // 拡大マスク画像用の領域を作成
                //        // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        stretched_mask_pic = MainForm.picStretchedTmp(1);
                //        stretched_mask_pic.Image = Image.FromFile("");
                //        stretched_mask_pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(dw);
                //        stretched_mask_pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(dh);

                //        // マスク画像を拡大して拡大マスク画像を作成
                //        // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //        ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を透過描画
                //        if (BGColor == ColorTranslator.ToOle(Color.White))
                //        {
                //            // 背景色が白
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT);
                //        }
                //        else
                //        {
                //            // 背景色が白以外
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT);
                //        }

                //        // 使用した一時画像領域を開放
                //        // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        {
                //            var withBlock15 = MainForm.picStretchedTmp(0);
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock15.Picture = Image.FromFile("");
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock15.width = 32;
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock15.Height = 32;
                //        }
                //        // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        {
                //            var withBlock16 = MainForm.picStretchedTmp(1);
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock16.Picture = Image.FromFile("");
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock16.width = 32;
                //            // UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            withBlock16.Height = 32;
                //        }
                //    }
                //    else
                //    {
                //        // 拡大画像を作成せず、StretchBltで直接拡大透過描画

                //        // 以前使用したマスク画像が利用可能？
                //        // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        mask_pic.Image = null;
                //        var loopTo11 = (short)(SRC.ImageBufferSize - 1);
                //        for (i = 0; i <= loopTo11; i++)
                //        {
                //            // 同じファイル？
                //            if ((PicBufFname[i] ?? "") == (fname ?? ""))
                //            {
                //                // オプションも同じ？
                //                if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                //                {
                //                    // 以前使用したマスク画像をそのまま利用
                //                    UsePicBuf(i);
                //                    // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    mask_pic = MainForm.picBuf(i);
                //                    // Debug.Print "Reuse " & Format$(i) & " As Mask"
                //                    break;
                //                }
                //            }
                //        }

                //        if (mask_pic is null)
                //        {
                //            // 新規にマスク画像作成

                //            // マスク画像に使用するpicBufを選択
                //            i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                //            PicBufFname[i] = fname;
                //            PicBufIsMask[i] = true;
                //            PicBufOption[i] = "";
                //            PicBufOption2[i] = pic_option2;
                //            PicBufDW[i] = (short)orig_width;
                //            PicBufDH[i] = (short)orig_height;
                //            PicBufSX[i] = (short)sx;
                //            PicBufSY[i] = (short)sy;
                //            PicBufSW[i] = (short)sw;
                //            PicBufSH[i] = (short)sh;
                //            // Debug.Print "Use " & Format$(i) & " As Mask"

                //            // バッファを初期化
                //            // UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            mask_pic = MainForm.picBuf(i);
                //            mask_pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(orig_width);
                //            mask_pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(orig_height);

                //            // マスク画像を作成
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                //        }

                //        // 描画をキャンセル？
                //        if (load_only)
                //        {
                //            DrawPictureRet = true;
                //            return DrawPictureRet;
                //        }

                //        // 画像を透過描画
                //        if (BGColor == ColorTranslator.ToOle(Color.White))
                //        {
                //            // 背景色が白
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT);
                //        }
                //        else
                //        {
                //            // 背景色が白以外
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCAND);

                //            // UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = BitBlt(mask_pic.hDC, 0, 0, orig_width, orig_width, orig_pic.hDC, 0, 0, SRCERASE);

                //            // UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT);

                //            // マスク画像が再利用できないのでバッファを開放
                //            ReleasePicBuf(i);
                //        }
                //    }

                //DrewPicture:
                //    ;
                //    if (permanent)
                //    {
                //        // 背景への描き込み
                //        Map.IsMapDirty = true;
                //        {
                //            var withBlock17 = MainForm;
                //            // マスク入り背景画像画面にも画像を描き込む
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            ret = BitBlt(withBlock17.picMaskedBack.hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                //            var loopTo15 = (short)((dx + dw - 1) / 32);
                //            for (i = (short)(dx / 32); i <= loopTo15; i++)
                //            {
                //                var loopTo16 = (short)((dy + dh - 1) / 32);
                //                for (j = (short)(dy / 32); j <= loopTo16; j++)
                //                {
                //                    // UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    // UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask.hDC, 0, 0, SRCAND);
                //                    // UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    // UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask2.hDC, 0, 0, SRCINVERT);
                //                }
                //            }
                //        }
                //    }
                //    else if (!on_msg_window & !on_status_window)
                //    {
                //        // 表示画像を消去する際に使う描画領域を設定
                //        PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(dx, 0));
                //        PaintedAreaY1 = (short)GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(dy, 0));
                //        PaintedAreaX2 = (short)GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(dx + dw, MainPWidth - 1));
                //        PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(dy + dh, MainPHeight - 1));
                //        IsPictureDrawn = true;
                //        IsPictureVisible = true;
                //        IsCursorVisible = false;
                //        if (keep_picture)
                //        {
                //            // picMain(1)にも描画
                //            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                //            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //            ret = BitBlt(MainForm.picMain(1).hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                //        }
                //    }
                #endregion
            }
            finally
            {
                g.Dispose();
            }
            DrawPictureRet = true;
            return DrawPictureRet;
        }
    }
}
