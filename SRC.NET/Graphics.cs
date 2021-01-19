using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class Graphics
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 画像処理を行うモジュール

        // BITMAPINFO構造体
        public struct BITMAPINFOHEADER
        {
            public int biSize; // bmiHeaderのサイズ
            public int biWidth; // ビットマップの幅を表すピクセル数
            public int biHeight; // ビットマップの高さを表すピクセル数
            public short biPlanes; // 常に１
            public short biBitCount; // ピクセルあたりのビット数
            public int biCompression; // 圧縮の種類
            public int biSizeImage; // 画像データのサイズを表すバイト数
            public int biXPelsPerMeter; // 水平方向の解像度を表すメートルあたりのピクセル数
            public int biYPelsPerMeter; // 垂直方向の解像度を表すメートルあたりのピクセル数
            public int biClrUsed; // ビットマップが実際に使用する色の数
            public int biClrImportant; // 重要な色の数(0の場合はすべての色が重要)
        }

        // パレットエントリ構造体
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        // ビットマップ情報
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [VBFixedArray(255)]
            public RGBQUAD[] bmiColors;

            // UPGRADE_TODO: この構造体のインスタンスを初期化するには、"Initialize" を呼び出さなければなりません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"' をクリックしてください。
            public void Initialize()
            {
                bmiColors = new RGBQUAD[256];
            }
        }

        // UPGRADE_WARNING: 構造体 BITMAPINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("gdi32")]
        static extern int StretchDIBits(int hDC, int X, int Y, int dx, int dy, int SrcX, int SrcY, int wSrcWidth, int wSrcHeight, int lpBits, ref BITMAPINFO lpBitsInfo, int wUsage, int dwRop);
        [DllImport("gdi32")]
        static extern int SelectObject(int hDC, int hObject);
        [DllImport("gdi32")]
        static extern int DeleteObject(int hObject);
        // UPGRADE_WARNING: 構造体 BITMAPINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("gdi32")]
        static extern int CreateDIBSection(int hDC, ref BITMAPINFO pBitmapInfo, int un, ref int lplpVoid, int handle, int dw);
        [DllImport("gdi32")]
        static extern int CreateCompatibleDC(int hDC);
        [DllImport("gdi32")]
        static extern int BitBlt(int hdest_dc, int X, int Y, int nWidth, int nHeight, int hsrc_dc, int xsrc, int ysrc, int dwRop);
        [DllImport("gdi32")]
        static extern int DeleteDC(int hDC);
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("gdi32")]
        static extern int CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitCount, ref Any lpBits);
        [DllImport("gdi32")]
        static extern int SetTextColor(int hDC, int crColor);
        [DllImport("gdi32")]
        static extern int SetBkColor(int hDC, int crColor);

        private const int SRCCOPY = 0xCC0020;
        private const short DIB_RGB_COLORS = 0;
        private const short BI_RGB = 0;


        // ビットマップ構造体
        public struct Bitmap
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public int bmBits;
        }

        // UPGRADE_NOTE: GetObject は GetObject_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("gdi32", EntryPoint = "GetObjectA")]
        public static extern int GetObject_Renamed(int hObject, int nCount, ref Any lpObject);

        // UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("gdi32")]
        public static extern int SetDIBits(int hDC, int hBitmap, int nStartScan, int nNumScans, ref Any lpBits, ref BITMAPINFOHEADER lpBI, int wUsage);
        // UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("gdi32")]
        public static extern int GetDIBits(int aHDC, int hBitmap, int nStartScan, int nNumScans, ref Any lpBits, ref BITMAPINFOHEADER lpBI, int wUsage);
        // UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        // UPGRADE_WARNING: 構造体 BITMAPINFOHEADER に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("gdi32")]
        public static extern int CreateDIBitmap(int hDC, ref BITMAPINFOHEADER lpInfoHeader, int dwUsage, ref Any lpInitBits, ref BITMAPINFOHEADER lpInitInfo, int wUsage);

        // 指定位置のピクセル色を取得する
        [DllImport("gdi32")]
        public static extern int GetPixel(int hDC, int X, int Y);

        // RGB構造体
        public struct RGBq
        {
            public byte Blue;
            public byte Green;
            public byte Red;
        }

        private static RGBq[] PixBuf; // ピクセルの色情報配列
        private static RGBq[] PixBuf2; // ピクセルの色情報配列
        private static int PixWidth; // バッファの内容の幅
        private static int PixHeight; // バッファの内容の高さ
        private static int PicWidth; // 画像の幅
        private static int PicHeight; // 画像の高さ

        // フェードイン＆アウト用変数
        private static BITMAPINFO[] BmpInfo;
        private static int NewDC;
        private static int MemDC;
        private static int OrigPicDC;
        private static int lpBit;
        private static byte[] FadeCMap;


        // 
        // フェードイン＆フェードアウト
        // 
        public static void InitFade(ref PictureBox pic, int times, bool white_out = false)
        {
            int g = default, r = default, b = default;
            int k, i, j, l;
            int tx, ty;
            int ret;
            var cmap = new RGBq[256];
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int rgb_Renamed;

            // フェード処理は画像を256色に変換して行う
            // このための256色のカラーマップを作成する

            // まずは決め打ちで0～195番の色を作成
            i = 0;
            for (j = 0; j <= 6; j++)
            {
                switch (j)
                {
                    case 0:
                        {
                            g = 0;
                            break;
                        }

                    case 1:
                        {
                            g = 40;
                            break;
                        }

                    case 2:
                        {
                            g = 95;
                            break;
                        }

                    case 3:
                        {
                            g = 130;
                            break;
                        }

                    case 4:
                        {
                            g = 180;
                            break;
                        }

                    case 5:
                        {
                            g = 220;
                            break;
                        }

                    case 6:
                        {
                            g = 255;
                            break;
                        }
                }

                for (k = 0; k <= 6; k++)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                r = 0;
                                break;
                            }

                        case 1:
                            {
                                r = 40;
                                break;
                            }

                        case 2:
                            {
                                r = 95;
                                break;
                            }

                        case 3:
                            {
                                r = 130;
                                break;
                            }

                        case 4:
                            {
                                r = 180;
                                break;
                            }

                        case 5:
                            {
                                r = 220;
                                break;
                            }

                        case 6:
                            {
                                r = 255;
                                break;
                            }
                    }

                    for (l = 0; l <= 3; l++)
                    {
                        switch (l)
                        {
                            case 0:
                                {
                                    b = 0;
                                    break;
                                }

                            case 1:
                                {
                                    b = 85;
                                    break;
                                }

                            case 2:
                                {
                                    b = 170;
                                    break;
                                }

                            case 3:
                                {
                                    b = 255;
                                    break;
                                }
                        }

                        {
                            var withBlock = cmap[i];
                            withBlock.Red = (byte)r;
                            withBlock.Green = (byte)g;
                            withBlock.Blue = (byte)b;
                        }

                        i = i + 1;
                    }
                }
            }

            // 196～255番の色は元画像の色をサンプリングして作成
            j = 0;
            while (i <= 220)
            {
                tx = GeneralLib.Dice((int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width)) - 1;
                ty = GeneralLib.Dice((int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height)) - 1;

                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                rgb_Renamed = GetPixel(pic.hDC, tx, ty);
                if (rgb_Renamed != 0)
                {
                    r = rgb_Renamed % 0x100;
                    rgb_Renamed = rgb_Renamed - r;
                    g = rgb_Renamed % 0x10000;
                    rgb_Renamed = rgb_Renamed - g;
                    g = g / 0x100;
                    b = rgb_Renamed / 0x10000;
                    {
                        var withBlock1 = cmap[i];
                        withBlock1.Red = (byte)r;
                        withBlock1.Green = (byte)g;
                        withBlock1.Blue = (byte)b;
                    }

                    i = i + 1;
                }

                j = j + 1;
                if (j > 100)
                {
                    break;
                }
            }

            j = 0;
            while (i <= 254)
            {
                tx = GeneralLib.Dice((int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width)) - 1;
                ty = GeneralLib.Dice((int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height)) - 1;

                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                rgb_Renamed = GetPixel(pic.hDC, tx, ty);
                if (rgb_Renamed != 0)
                {
                    r = rgb_Renamed % 0x100;
                    rgb_Renamed = rgb_Renamed - r;
                    g = rgb_Renamed % 0x10000;
                    rgb_Renamed = rgb_Renamed - g;
                    g = g / 0x100;
                    b = rgb_Renamed / 0x10000;
                    {
                        var withBlock2 = cmap[i];
                        withBlock2.Red = (byte)r;
                        withBlock2.Green = (byte)g;
                        withBlock2.Blue = (byte)b;
                    }

                    i = i + 1;
                }

                j = j + 1;
                if (j > 100)
                {
                    break;
                }
            }

            rgb_Renamed = Event_Renamed.ObjColor;
            r = rgb_Renamed % 0x100;
            rgb_Renamed = rgb_Renamed - r;
            g = rgb_Renamed % 0x10000;
            rgb_Renamed = rgb_Renamed - g;
            g = g / 0x100;
            b = rgb_Renamed / 0x10000;
            {
                var withBlock3 = cmap[i];
                withBlock3.Red = (byte)r;
                withBlock3.Green = (byte)g;
                withBlock3.Blue = (byte)b;
            }

            // BmpInfoをカラーパレットを変えながらtimes+1個作成
            // UPGRADE_WARNING: 配列 BmpInfo で各要素を初期化する必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"' をクリックしてください。
            BmpInfo = new BITMAPINFO[times + 1];
            var loopTo = times;
            for (i = 0; i <= loopTo; i++)
            {
                {
                    var withBlock4 = BmpInfo[i].bmiHeader;
                    withBlock4.biSize = Strings.Len(BmpInfo[i].bmiHeader);
                    withBlock4.biWidth = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width);
                    withBlock4.biHeight = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height);
                    withBlock4.biPlanes = 1;
                    withBlock4.biBitCount = 8;
                    withBlock4.biCompression = BI_RGB;
                    withBlock4.biSizeImage = 0;
                    withBlock4.biXPelsPerMeter = 0;
                    withBlock4.biYPelsPerMeter = 0;
                    withBlock4.biClrUsed = 0;
                    withBlock4.biClrImportant = 0;
                }

                // カラーパレット設定
                if (white_out)
                {
                    for (j = 0; j <= 255; j++)
                    {
                        {
                            var withBlock5 = cmap[j];
                            r = withBlock5.Red;
                            g = withBlock5.Green;
                            b = withBlock5.Blue;
                        }

                        {
                            var withBlock6 = BmpInfo[i].bmiColors[j];
                            withBlock6.rgbBlue = (byte)(r + (255 - r) * (times - i) / times);
                            withBlock6.rgbGreen = (byte)(g + (255 - g) * (times - i) / times);
                            withBlock6.rgbRed = (byte)(b + (255 - b) * (times - i) / times);
                        }
                    }
                }
                else
                {
                    for (j = 0; j <= 255; j++)
                    {
                        {
                            var withBlock7 = cmap[j];
                            r = withBlock7.Red;
                            g = withBlock7.Green;
                            b = withBlock7.Blue;
                        }

                        {
                            var withBlock8 = BmpInfo[i].bmiColors[j];
                            withBlock8.rgbBlue = (byte)(r * i / times);
                            withBlock8.rgbGreen = (byte)(g * i / times);
                            withBlock8.rgbRed = (byte)(b * i / times);
                        }
                    }
                }
            }

            // DIBとウインドウDCからDIBSectionを作成
            // UPGRADE_ISSUE: Form プロパティ MainForm.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            NewDC = CreateDIBSection(GUI.MainForm.hDC, ref BmpInfo[times], DIB_RGB_COLORS, ref lpBit, 0, 0);

            // メモリDCの作成
            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            MemDC = CreateCompatibleDC(pic.hDC);

            // メモリDCにDIBSectionを選択し、元のビットマップのハンドルを保存
            OrigPicDC = SelectObject(MemDC, NewDC);

            // BitBltを使って元の画像をlpBitに反映
            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            ret = BitBlt(MemDC, 0, 0, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX((double)pic.Width), Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY((double)pic.Height), pic.hDC, 0, 0, SRCCOPY);
        }

        public static void DoFade(ref PictureBox pic, int times)
        {
            int ret;

            // 範囲外の場合は抜ける
            if (times < 0 | Information.UBound(BmpInfo) < times)
            {
                return;
            }

            // BmpInfoを変更してカラーパレットを変更
            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            ret = StretchDIBits(pic.hDC, 0, 0, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX((double)pic.Width), Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY((double)pic.Height), 0, 0, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX((double)pic.Width), Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY((double)pic.Height), lpBit, ref BmpInfo[times], DIB_RGB_COLORS, SRCCOPY);
        }

        public static void FinishFade()
        {
            int ret;

            // 元のビットマップのハンドルを選択
            ret = SelectObject(MemDC, OrigPicDC);
            // デバイスコンテキスト開放
            ret = DeleteDC(MemDC);
            // ビットマップ開放
            ret = DeleteObject(NewDC);
        }


        // 
        // マスク作成用のサブルーチン
        // 
        public static void MakeMask(ref int src_dc, ref int dest_dc, ref int w, ref int h, ref int tcolor)
        {
            int mask_dc;
            int mask_bmp, orig_mask_bmp;
            int ret;

            // メモリDCの作成
            mask_dc = CreateCompatibleDC(src_dc);
            // モノクロビットマップの作成
            int arglpBits = 0;
            mask_bmp = CreateBitmap(w, h, 1, 1, ref arglpBits);
            // メモリDCにビットマップを選択し元のビットマップのハンドルを保存
            orig_mask_bmp = SelectObject(mask_dc, mask_bmp);

            // 背景色(=透明色)の設定
            ret = SetBkColor(src_dc, tcolor);
            ret = BitBlt(mask_dc, 0, 0, w, h, src_dc, 0, 0, SRCCOPY);

            // 背景色を白に戻す
            if (tcolor != ColorTranslator.ToOle(Color.White))
            {
                ret = SetBkColor(dest_dc, ColorTranslator.ToOle(Color.White));
            }

            ret = BitBlt(dest_dc, 0, 0, w, h, mask_dc, 0, 0, SRCCOPY);

            // 元のビットマップのハンドルを選択
            ret = SelectObject(mask_dc, orig_mask_bmp);
            // デバイスコンテキスト開放
            ret = DeleteDC(mask_dc);
            // ビットマップ開放
            ret = DeleteObject(mask_bmp);
        }

        // 画像イメージpicをPixBufに収得
        public static void GetImage(ref PictureBox pic)
        {
            int pic_bmp, tmp_bmp;
            var bm_info = default(BITMAPINFOHEADER);
            int ret;
            int mem_dc;
            var bmp = default(Bitmap);
            // UPGRADE_WARNING: オブジェクト bmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            // UPGRADE_ISSUE: PictureBox プロパティ pic.Image はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            ret = GetObject_Renamed(Conversions.ToInteger(pic.Image), 24, ref bmp);
            PixWidth = bmp.bmWidth;
            PixHeight = bmp.bmHeight;
            PicWidth = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(pic.Width);
            PicHeight = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(pic.Height);
            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            mem_dc = pic.hDC;
            bm_info.biBitCount = 24;
            bm_info.biClrUsed = 0;
            bm_info.biCompression = 0;
            bm_info.biHeight = -PixHeight;
            bm_info.biWidth = PixWidth;
            bm_info.biPlanes = 1;
            bm_info.biSize = 40;
            bm_info.biSizeImage = bm_info.biWidth * bm_info.biHeight * 3;
            int arglpInitBits = 0;
            tmp_bmp = CreateDIBitmap(mem_dc, ref bm_info, 0, ref arglpInitBits, ref bm_info, 0);
            pic_bmp = SelectObject(mem_dc, tmp_bmp);
            PixBuf = new RGBq[PixWidth * PixHeight + 1];
            // UPGRADE_WARNING: オブジェクト PixBuf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            ret = GetDIBits(mem_dc, pic_bmp, 0, PixHeight, ref PixBuf[0], ref bm_info, 0);
            tmp_bmp = SelectObject(mem_dc, pic_bmp);
            ret = DeleteObject(tmp_bmp);
        }

        // PixBufの画像イメージをpicに描き込む
        public static void SetImage(ref PictureBox pic)
        {
            int pic_bmp, tmp_bmp;
            var bm_info = default(BITMAPINFOHEADER);
            int pic_dc;
            int ret;

            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            pic_dc = pic.hDC;
            bm_info.biBitCount = 24;
            bm_info.biClrUsed = 0;
            bm_info.biCompression = 0;
            bm_info.biHeight = -PixHeight;
            bm_info.biWidth = PixWidth;
            bm_info.biPlanes = 1;
            bm_info.biSize = 40;
            bm_info.biSizeImage = bm_info.biWidth * bm_info.biHeight * 3;
            int arglpInitBits = 0;
            tmp_bmp = CreateDIBitmap(pic_dc, ref bm_info, 0, ref arglpInitBits, ref bm_info, 0);
            pic_bmp = SelectObject(pic_dc, tmp_bmp);

            // UPGRADE_WARNING: オブジェクト PixBuf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            ret = SetDIBits(pic_dc, pic_bmp, 0, PixHeight, ref PixBuf[0], ref bm_info, 0);
            tmp_bmp = SelectObject(pic_dc, pic_bmp);
            ret = DeleteObject(tmp_bmp);
            pic.Refresh();
            // ReDim PixBuf(0)
        }

        // PixBufの内容を消去
        public static void ClearImage()
        {
            if (Information.UBound(PixBuf) > 0)
            {
                PixBuf = new RGBq[1];
            }

            PixBuf2 = new RGBq[1];
        }

        // PixBufの内容をPixBuf2にコピー
        public static void CopyImage()
        {
            int i;
            PixBuf2 = new RGBq[PixWidth * PixHeight + 1];
            var loopTo = PixWidth * PixHeight - 1;
            for (i = 0; i <= loopTo; i++)
                // UPGRADE_WARNING: オブジェクト PixBuf2(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                PixBuf2[i] = PixBuf[i];
        }

        // マスク画像の作成
        public static void CreateMask(ref int tcolor)
        {
            int j, i, k = default;
            var loopTo = PicHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                var loopTo1 = PicWidth - 1;
                for (j = 0; j <= loopTo1; j++)
                {
                    {
                        var withBlock = PixBuf[k];
                        if (tcolor == 256d * (256d * withBlock.Blue + withBlock.Green) + withBlock.Red)
                        {
                            withBlock.Green = 255;
                            withBlock.Blue = 255;
                            withBlock.Red = 255;
                        }
                        else
                        {
                            withBlock.Green = 0;
                            withBlock.Blue = 0;
                            withBlock.Red = 0;
                        }
                    }

                    k = k + 1;
                }
            }
        }

        // 描き込み画像の作成
        public static void CreateImage(ref int tcolor)
        {
            int j, i, k = default;
            var loopTo = PicHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                var loopTo1 = PicWidth - 1;
                for (j = 0; j <= loopTo1; j++)
                {
                    {
                        var withBlock = PixBuf[k];
                        if (tcolor == 256d * (256d * withBlock.Blue + withBlock.Green) + withBlock.Red)
                        {
                            withBlock.Green = 0;
                            withBlock.Blue = 0;
                            withBlock.Red = 0;
                        }
                    }

                    k = k + 1;
                }
            }
        }

        // フェード処理の初期化
        public static void FadeInit(int num)
        {
            short i, j;
            FadeCMap = new byte[num + 1, 256];
            var loopTo = (short)num;
            for (i = 1; i <= loopTo; i++)
            {
                for (j = 0; j <= 255; j++)
                    FadeCMap[i, j] = j * i / num;
            }
        }

        // フェードイン・アウト実行
        public static void FadeInOut(int ind, int num)
        {
            int g, i, r, b;
            var loopTo = PixWidth * PixHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                {
                    var withBlock = PixBuf2[i];
                    r = withBlock.Red;
                    g = withBlock.Green;
                    b = withBlock.Blue;
                }

                {
                    var withBlock1 = PixBuf[i];
                    withBlock1.Red = FadeCMap[ind, r];
                    withBlock1.Green = FadeCMap[ind, g];
                    withBlock1.Blue = FadeCMap[ind, b];
                }
                // With PixBuf(i)
                // .Red = r * ind \ num
                // .Green = g * ind \ num
                // .Blue = b * ind \ num
                // End With
            }
        }

        // ホワイトイン・アウト実行
        public static void WhiteInOut(int ind, int num)
        {
            int g, i, r, b;
            var loopTo = PixWidth * PixHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                {
                    var withBlock = PixBuf2[i];
                    r = withBlock.Red;
                    g = withBlock.Green;
                    b = withBlock.Blue;
                }

                {
                    var withBlock1 = PixBuf[i];
                    withBlock1.Red = (byte)(r + (255 - r) * (num - ind) / num);
                    withBlock1.Green = (byte)(g + (255 - g) * (num - ind) / num);
                    withBlock1.Blue = (byte)(b + (255 - b) * (num - ind) / num);
                }
            }
        }

        // 画像を明るく
        public static void Bright(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            if (withBlock.Red != 255)
                            {
                                withBlock.Red = (byte)GeneralLib.MinLng(withBlock.Red + 80, 254);
                            }

                            if (withBlock.Green != 255)
                            {
                                withBlock.Green = (byte)GeneralLib.MinLng(withBlock.Green + 80, 254);
                            }

                            if (withBlock.Blue != 255)
                            {
                                withBlock.Blue = (byte)GeneralLib.MinLng(withBlock.Blue + 80, 254);
                            }
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        withBlock1.Red = (byte)GeneralLib.MinLng(withBlock1.Red + 80, 255);
                        withBlock1.Green = (byte)GeneralLib.MinLng(withBlock1.Green + 80, 255);
                        withBlock1.Blue = (byte)GeneralLib.MinLng(withBlock1.Blue + 80, 255);
                    }
                }
            }
        }

        // 画像を暗く
        public static void Dark(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            withBlock.Red = (byte)(withBlock.Red / 2);
                            withBlock.Green = (byte)(withBlock.Green / 2);
                            withBlock.Blue = (byte)(withBlock.Blue / 2);
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        withBlock1.Red = (byte)(withBlock1.Red / 2);
                        withBlock1.Green = (byte)(withBlock1.Green / 2);
                        withBlock1.Blue = (byte)(withBlock1.Blue / 2);
                    }
                }
            }
        }

        // 画像を白黒に
        public static void Monotone(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            rgb_Renamed = (int)(0.299d * withBlock.Red + 0.587d * withBlock.Green + 0.114d * withBlock.Blue);
                            withBlock.Red = (byte)rgb_Renamed;
                            withBlock.Green = (byte)rgb_Renamed;
                            withBlock.Blue = (byte)rgb_Renamed;
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        rgb_Renamed = (int)(0.299d * withBlock1.Red + 0.587d * withBlock1.Green + 0.114d * withBlock1.Blue);
                        withBlock1.Red = (byte)rgb_Renamed;
                        withBlock1.Green = (byte)rgb_Renamed;
                        withBlock1.Blue = (byte)rgb_Renamed;
                    }
                }
            }
        }

        // 画像をセピア色に
        public static void Sepia(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            rgb_Renamed = (int)(0.299d * withBlock.Red + 0.587d * withBlock.Green + 0.114d * withBlock.Blue);
                            withBlock.Red = (byte)GeneralLib.MinLng((int)(1.1d * rgb_Renamed), 255);
                            withBlock.Green = (byte)(0.9d * rgb_Renamed);
                            withBlock.Blue = (byte)(0.7d * rgb_Renamed);
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        rgb_Renamed = (int)(0.299d * withBlock1.Red + 0.587d * withBlock1.Green + 0.114d * withBlock1.Blue);
                        withBlock1.Red = (byte)GeneralLib.MinLng((int)(1.1d * rgb_Renamed), 255);
                        withBlock1.Green = (byte)(0.9d * rgb_Renamed);
                        withBlock1.Blue = (byte)(0.7d * rgb_Renamed);
                    }
                }
            }
        }

        // 画像を夕焼け風に
        public static void Sunset(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            rgb_Renamed = (int)(0.299d * withBlock.Red + 0.587d * withBlock.Green + 0.114d * withBlock.Blue);
                            withBlock.Red = (byte)GeneralLib.MinLng((int)(0.2d * withBlock.Red + 1.3d * rgb_Renamed), 255);
                            withBlock.Green = (byte)(0.2d * withBlock.Green + 0.4d * rgb_Renamed);
                            withBlock.Blue = (byte)(0.2d * withBlock.Blue + 0.2d * rgb_Renamed);
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        rgb_Renamed = (int)(0.299d * withBlock1.Red + 0.587d * withBlock1.Green + 0.114d * withBlock1.Blue);
                        withBlock1.Red = (byte)GeneralLib.MinLng((int)(0.2d * withBlock1.Red + 1.3d * rgb_Renamed), 255);
                        withBlock1.Green = (byte)(0.2d * withBlock1.Green + 0.4d * rgb_Renamed);
                        withBlock1.Blue = (byte)(0.2d * withBlock1.Blue + 0.2d * rgb_Renamed);
                    }
                }
            }
        }

        // 画像を水中風に
        public static void Water(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            rgb_Renamed = (int)(0.299d * withBlock.Red + 0.587d * withBlock.Green + 0.114d * withBlock.Blue);
                            withBlock.Red = (byte)(0.6d * rgb_Renamed);
                            withBlock.Green = (byte)(0.8d * rgb_Renamed);
                            withBlock.Blue = (byte)rgb_Renamed;
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        rgb_Renamed = (int)(0.299d * withBlock1.Red + 0.587d * withBlock1.Green + 0.114d * withBlock1.Blue);
                        withBlock1.Red = (byte)(0.6d * rgb_Renamed);
                        withBlock1.Green = (byte)(0.8d * rgb_Renamed);
                        withBlock1.Blue = (byte)rgb_Renamed;
                    }
                }
            }
        }

        // 画像を左右反転
        public static void HReverse()
        {
            int i, j;
            RGBq tmp;
            var loopTo = PicHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                var loopTo1 = PicWidth / 2 - 1;
                for (j = 0; j <= loopTo1; j++)
                {
                    // UPGRADE_WARNING: オブジェクト tmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    tmp = PixBuf[PicWidth * i + j];
                    // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    PixBuf[PicWidth * i + j] = PixBuf[PicWidth * i + PicWidth - j - 1];
                    // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + PicWidth - j - 1) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    PixBuf[PicWidth * i + PicWidth - j - 1] = tmp;
                }
            }
        }

        // 画像を上下反転
        public static void VReverse()
        {
            int i, j;
            RGBq tmp;
            var loopTo = PicHeight / 2 - 1;
            for (i = 0; i <= loopTo; i++)
            {
                var loopTo1 = PicWidth - 1;
                for (j = 0; j <= loopTo1; j++)
                {
                    // UPGRADE_WARNING: オブジェクト tmp の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    tmp = PixBuf[PicWidth * i + j];
                    // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    PixBuf[PicWidth * i + j] = PixBuf[PicWidth * (PicHeight - i - 1) + j];
                    // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * PicHeight - i - 1 + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    PixBuf[PicWidth * (PicHeight - i - 1) + j] = tmp;
                }
            }
        }

        // 画像をネガポジ反転
        public static void NegPosReverse(bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;
                var loopTo = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    {
                        var withBlock = PixBuf[i];
                        if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                        {
                            withBlock.Red = (byte)(255 - withBlock.Red);
                            withBlock.Green = (byte)(255 - withBlock.Green);
                            withBlock.Blue = (byte)(255 - withBlock.Blue);
                        }
                    }
                }
            }
            else
            {
                var loopTo1 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = PixBuf[i];
                        withBlock1.Red = (byte)(255 - withBlock1.Red);
                        withBlock1.Green = (byte)(255 - withBlock1.Green);
                        withBlock1.Blue = (byte)(255 - withBlock1.Blue);
                    }
                }
            }
        }

        // 画像からシルエット抽出
        public static void Silhouette()
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;

            // 背景色をRGBに分解
            rgb_Renamed = GUI.BGColor;
            r = rgb_Renamed % 0x100;
            rgb_Renamed = rgb_Renamed - r;
            g = rgb_Renamed % 0x10000;
            rgb_Renamed = rgb_Renamed - g;
            g = g / 0x100;
            b = rgb_Renamed / 0x10000;
            var loopTo = PixWidth * PixHeight - 1;
            for (i = 0; i <= loopTo; i++)
            {
                {
                    var withBlock = PixBuf[i];
                    if (r == withBlock.Red & g == withBlock.Green & b == withBlock.Blue)
                    {
                        withBlock.Red = 255;
                        withBlock.Green = 255;
                        withBlock.Blue = 255;
                    }
                    else
                    {
                        withBlock.Red = 0;
                        withBlock.Green = 0;
                        withBlock.Blue = 0;
                    }
                }
            }
        }

        // 画像を右方向にangle度回転させる
        // do_sameがTrueの場合は回転角度が90度の倍数である際の描画最適化を行わない
        public static void Rotate(int angle, bool do_same = false)
        {
            int i, j;
            int xsrc, ysrc;
            double xsrc0, ysrc0;
            double xbase, ybase;
            double xoffset, yoffset;
            double dsin, rad, dcos;
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            RGBq bg;
            int g, rgb_Renamed, r, b;

            // 360度で一回転
            angle = angle % 360;
            // 負の場合は正の角度に
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            // 回転角度が90度の倍数である場合は処理が簡単。
            // ただし、90度以外の角度で連続回転させる場合は、処理時間を一定にするため
            // この最適化は使わない。
            if (!do_same)
            {
                switch (angle)
                {
                    case 0:
                        {
                            return;
                        }

                    case 90:
                        {
                            if (PicWidth == PicHeight)
                            {
                                CopyImage();
                                var loopTo = PicHeight - 1;
                                for (i = 0; i <= loopTo; i++)
                                {
                                    var loopTo1 = PicWidth - 1;
                                    for (j = 0; j <= loopTo1; j++)
                                        // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                        PixBuf[PicWidth * i + j] = PixBuf2[PicWidth * (PicWidth - j - 1) + i];
                                }

                                return;
                            }

                            break;
                        }

                    case 180:
                        {
                            CopyImage();
                            var loopTo2 = PicHeight - 1;
                            for (i = 0; i <= loopTo2; i++)
                            {
                                var loopTo3 = PicWidth - 1;
                                for (j = 0; j <= loopTo3; j++)
                                    // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    PixBuf[PicWidth * i + j] = PixBuf2[PicWidth * (PicHeight - i - 1) + PicWidth - j - 1];
                            }

                            return;
                        }

                    case 270:
                        {
                            if (PicWidth == PicHeight)
                            {
                                CopyImage();
                                var loopTo4 = PicHeight - 1;
                                for (i = 0; i <= loopTo4; i++)
                                {
                                    var loopTo5 = PicWidth - 1;
                                    for (j = 0; j <= loopTo5; j++)
                                        // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                        PixBuf[PicWidth * i + j] = PixBuf2[PicWidth * j + PicHeight - i - 1];
                                }

                                return;
                            }

                            break;
                        }
                }
            }

            // 任意の角度の場合は三角関数を使う必要がある

            // 座標の計算は画像の中心を座標原点にして行う
            xbase = (PicWidth - 1) / 2d;
            ybase = (PicHeight - 1) / 2d;

            // 回転によるベクトル
            angle = 90 - angle;
            rad = angle * 3.14159265d / 180d;
            dsin = Math.Sin(rad);
            dcos = Math.Cos(rad);

            // 背景色をRGBに分解
            rgb_Renamed = GUI.BGColor;
            r = rgb_Renamed % 0x100;
            rgb_Renamed = rgb_Renamed - r;
            g = rgb_Renamed % 0x10000;
            rgb_Renamed = rgb_Renamed - g;
            g = g / 0x100;
            b = rgb_Renamed / 0x10000;
            // 合成
            bg.Red = (byte)r;
            bg.Green = (byte)g;
            bg.Blue = (byte)b;

            // 画像データのコピーを取っておく
            CopyImage();

            // 各ピクセルに対して回転処理
            var loopTo6 = PicHeight - 1;
            for (i = 0; i <= loopTo6; i++)
            {
                yoffset = i - ybase;
                xsrc0 = xbase + yoffset * dcos;
                ysrc0 = ybase + yoffset * dsin;
                var loopTo7 = PicWidth - 1;
                for (j = 0; j <= loopTo7; j++)
                {
                    xoffset = j - xbase;

                    // 本当は下記の式で一度に計算できるが、高速化のため式を分割
                    // xsrc = xbase + xoffset * dsin + yoffset * dcos
                    // ysrc = ybase - xoffset * dcos + yoffset * dsin
                    xsrc = (int)(xsrc0 + xoffset * dsin);
                    ysrc = (int)(ysrc0 - xoffset * dcos);
                    if (xsrc < 0 | PicWidth <= xsrc | ysrc < 0 | PicHeight <= ysrc)
                    {
                        // 範囲外のピクセルの場合は背景色で描画
                        // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        PixBuf[PicWidth * i + j] = bg;
                    }
                    else
                    {
                        // UPGRADE_WARNING: オブジェクト PixBuf(PicWidth * i + j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        PixBuf[PicWidth * i + j] = PixBuf2[PicWidth * ysrc + xsrc];
                    }
                }
            }
        }

        // 透過率trans_parでfcolorによる半透明描画を行う
        public static void ColorFilter(ref int fcolor, ref double trans_par, bool is_transparent = false)
        {
            // UPGRADE_NOTE: rgb は rgb_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            int i, rgb_Renamed;
            int g, r, b;
            byte g2, r2, b2;
            int tratio;

            // 透過率をパーセントに直す
            tratio = GeneralLib.MinLng(GeneralLib.MaxLng((int)(100d * trans_par), 0), 100);
            if (tratio == 0)
            {
                // 透過しない場合はそのまま終了
                return;
            }

            // 半透明描画色をRGBに分解
            rgb_Renamed = fcolor;
            r = rgb_Renamed % 0x100;
            rgb_Renamed = rgb_Renamed - r;
            g = rgb_Renamed % 0x10000;
            rgb_Renamed = rgb_Renamed - g;
            g = g / 0x100;
            b = rgb_Renamed / 0x10000;
            r2 = (byte)r;
            g2 = (byte)g;
            b2 = (byte)b;
            if (is_transparent)
            {
                // 背景色をRGBに分解
                rgb_Renamed = GUI.BGColor;
                r = rgb_Renamed % 0x100;
                rgb_Renamed = rgb_Renamed - r;
                g = rgb_Renamed % 0x10000;
                rgb_Renamed = rgb_Renamed - g;
                g = g / 0x100;
                b = rgb_Renamed / 0x10000;

                // 背景色と半透明描画色が同一だった場合、半透明描画色を背景色から少しずらす
                // ただしこの処理が可能なのは背景色が白等の場合のみ
                if (r == r2 & g == g2 & b == b2)
                {
                    if (r2 == 255)
                    {
                        r2 = 254;
                    }
                    else if (g2 == 255)
                    {
                        g2 = 254;
                    }
                    else if (b2 == 255)
                    {
                        b2 = 254;
                    }
                }

                if (trans_par == 100d)
                {
                    var loopTo = PixWidth * PixHeight - 1;
                    for (i = 0; i <= loopTo; i++)
                    {
                        {
                            var withBlock = PixBuf[i];
                            if (r != withBlock.Red | g != withBlock.Green | b != withBlock.Blue)
                            {
                                withBlock.Red = r2;
                                withBlock.Green = g2;
                                withBlock.Blue = b2;
                            }
                        }
                    }
                }
                else
                {
                    var loopTo1 = PixWidth * PixHeight - 1;
                    for (i = 0; i <= loopTo1; i++)
                    {
                        {
                            var withBlock1 = PixBuf[i];
                            if (r != withBlock1.Red | g != withBlock1.Green | b != withBlock1.Blue)
                            {
                                withBlock1.Red = (byte)((withBlock1.Red * (100 - tratio) + r2 * tratio) / 100);
                                withBlock1.Green = (byte)((withBlock1.Green * (100 - tratio) + g2 * tratio) / 100);
                                withBlock1.Blue = (byte)((withBlock1.Blue * (100 - tratio) + b2 * tratio) / 100);
                            }
                        }
                    }
                }
            }
            else if (trans_par == 100d)
            {
                var loopTo2 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo2; i++)
                {
                    {
                        var withBlock2 = PixBuf[i];
                        withBlock2.Red = r2;
                        withBlock2.Green = g2;
                        withBlock2.Blue = b2;
                    }
                }
            }
            else
            {
                var loopTo3 = PixWidth * PixHeight - 1;
                for (i = 0; i <= loopTo3; i++)
                {
                    {
                        var withBlock3 = PixBuf[i];
                        withBlock3.Red = (byte)((withBlock3.Red * (100 - tratio) + r2 * tratio) / 100);
                        withBlock3.Green = (byte)((withBlock3.Green * (100 - tratio) + g2 * tratio) / 100);
                        withBlock3.Blue = (byte)((withBlock3.Blue * (100 - tratio) + b2 * tratio) / 100);
                    }
                }
            }
        }
    }
}