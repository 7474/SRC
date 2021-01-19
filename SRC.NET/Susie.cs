using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class Susie
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // Susieプラグインを利用して画像ファイルを読み込むためのモジュール

        // Susie 32-bit Plug-in API
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("ifpng.spi", EntryPoint = "GetPicture")]
        private static extern int GetPNGPicture(ref Any buf, int length, int flag, ref int pHBInfo, ref int pHBm, Any lpProgressCallback, int lData);

        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("gdi32")]
        private static extern int SetDIBits(int hDC, int hBitmap, int nStartScan, int nNumScans, ref Any lpBits, ref Any lpBI, int wUsage);
        [DllImport("kernel32")]
        private static extern int LocalFree(int hMem);
        [DllImport("kernel32")]
        private static extern int LocalLock(int hMem);
        [DllImport("kernel32")]
        private static extern int LocalUnlock(int hMem);

        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
        private static extern void MoveMemory(ref Any dest, ref Any Source, int length);

        // 画像ファイルを読み込む関数
        public static bool LoadPicture2(ref PictureBox pic, ref string fname)
        {
            bool LoadPicture2Ret = default;
            int HBInfo = default, HBm = default;
            int lpHBInfo, lpHBm;
            // UPGRADE_WARNING: 構造体 bmi の配列は、使用する前に初期化する必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"' をクリックしてください。
            var bmi = default(Graphics.BITMAPINFO);
            int ret;
            ;

            // 画像の取得
            switch (Strings.LCase(Strings.Right(fname, 4)) ?? "")
            {
                case ".bmp":
                case ".jpg":
                case ".gif":
                    {
                        // Susieプラグインを使わずにロード
                        pic.Image = Image.FromFile(fname);
                        LoadPicture2Ret = true;
                        return LoadPicture2Ret;
                    }

                case ".png":
                    {
                        // PNGファイル用SusieプラグインAPIを実行
                        ret = GetPNGPicture(ref fname, 0, 0, ref HBInfo, ref HBm, 0, 0);
                        break;
                    }

                default:
                    {
                        // 未サポートのファイル形式
                        string argmsg = "画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の画像フォーマットはサポートされていません。";
                        GUI.ErrorMessage(ref argmsg);
                        pic.Image = Image.FromFile("");
                        return LoadPicture2Ret;
                    }
            }

            // 読み込みに成功した？
            if (ret != 0)
            {
                string argmsg1 = "画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
                GUI.ErrorMessage(ref argmsg1);
                return LoadPicture2Ret;
            }

            // メモリのロック
            lpHBInfo = LocalLock(HBInfo);
            lpHBm = LocalLock(HBm);

            // なぜか画像を一旦消去しておく必要あり
            pic.Image = Image.FromFile("");
            // ピクチャボックスのサイズ変更
            // UPGRADE_WARNING: オブジェクト bmi の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            MoveMemory(ref bmi, ref lpHBInfo, Strings.Len(bmi));
            pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(bmi.bmiHeader.biWidth);
            pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(bmi.bmiHeader.biHeight);

            // 画像の表示
            // UPGRADE_ISSUE: PictureBox プロパティ pic.Image はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            ret = SetDIBits(pic.hDC, Conversions.ToInteger(pic.Image), 0, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY((double)pic.Height), ref lpHBm, ref lpHBInfo, 0);

            // メモリのロック解除
            LocalUnlock(HBInfo);
            LocalUnlock(HBm);

            // メモリハンドルの解放
            LocalFree(HBInfo);
            LocalFree(HBm);

            // 画像の読み出しに成功したかどうかを返す
            if (ret != 0)
            {
                LoadPicture2Ret = true;
            }

            return LoadPicture2Ret;
            ErrorHandler:
            ;

            // エラー処理
            switch (Strings.LCase(Strings.Right(fname, 4)) ?? "")
            {
                case ".bmp":
                case ".jpg":
                case ".gif":
                    {
                        string argmsg2 = "画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
                        GUI.ErrorMessage(ref argmsg2);
                        break;
                    }

                case ".png":
                    {
                        string argmsg3 = "画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "PNGファイル用Susie Plug-inがインストールされていません。";
                        GUI.ErrorMessage(ref argmsg3);
                        break;
                    }
            }
        }
    }
}